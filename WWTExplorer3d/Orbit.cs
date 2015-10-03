
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Drawing;

using System.Windows.Forms;

namespace TerraViewer
{
    public class Orbit
    {

        private readonly CAAEllipticalObjectElements elements;

        readonly Color orbitColor = Color.White;
        readonly float scale;
        public Orbit(CAAEllipticalObjectElements elements, int segments, Color color, float thickness, float scale)
        {
            this.elements = elements;
            this.segmentCount = segments;
            this.orbitColor = color;
            this.scale = scale;
        }
        public void CleanUp()
        {
        }

        // Get the radius of a sphere (centered at a focus of the ellipse) that is
        // large enough to contain the orbit. The value returned has units of the orbit scale.
        public double BoundingRadius
        {
            get
            {
                if (elements != null)
                {
                    return (elements.a * (1.0 + elements.e)) / scale;
                }
                else
                {
                    return 0.0;
                }
            }
        }

      
  
        // Convert from standard coordinate system with z normal to the orbital plane
        // to WWT's system where y is the normal. Note that this transformation is not
        // a pure rotation: it incorporates a reflection, because the two systems have
        // different handedness.
        static readonly Matrix3d orbitalToWwt = new Matrix3d(1.0, 0.0, 0.0, 0.0,
                                                    0.0, 0.0, 1.0, 0.0,
                                                    0.0, 1.0, 0.0, 0.0,
                                                    0.0, 0.0, 0.0, 1.0);
        // ** Begin 
        public void Draw3D(RenderContext11 renderContext, float opacity, Vector3d centerPoint)
        {
            var orbitalPlaneOrientation = Matrix3d.RotationZ(Coordinates.DegreesToRadians(elements.w)) *
                                               Matrix3d.RotationX(Coordinates.DegreesToRadians(elements.i)) *
                                               Matrix3d.RotationZ(Coordinates.DegreesToRadians(elements.omega));

            // Extra transformation required because the ellipse shader uses the xy-plane, but WWT uses the
            // xz-plane as the reference.
            orbitalPlaneOrientation = orbitalPlaneOrientation * orbitalToWwt;

            var worldMatrix = orbitalPlaneOrientation * Matrix3d.Translation(centerPoint) * renderContext.World;

            var M = elements.n * (SpaceTimeController.JNow - elements.T);
            double F = 1;
            if (M < 0)
            {
                F = -1;
            }
            M = Math.Abs(M) / 360.0;
            M = (M - (int)(M)) * 360.0 * F;

            var color = Color.FromArgb((int) (opacity * 255.0f), orbitColor);

            // Newton-Raphson iteration to solve Kepler's equation.
            // This is faster than calling CAAKepler.Calculate(), and 5 steps
            // is more than adequate for draw the orbit paths of small satellites
            // (which are ultimately rendered using single-precision floating point.)
            M = Coordinates.DegreesToRadians(M);
            var E = M;
            for (var i = 0; i < 5; i++)
            {
                E += (M - E + elements.e * Math.Sin(E)) / (1 - elements.e * Math.Cos(E));
            }

            renderContext.DepthStencilMode = DepthStencilMode.ZReadOnly;
            renderContext.BlendMode = BlendMode.Alpha;
            EllipseRenderer.DrawEllipse(renderContext, elements.a / scale, elements.e, E, color, worldMatrix);
            renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;
        }

        private int segmentCount;
    }


    public class EllipseRenderer
    {
        private static PositionVertexBuffer11 ellipseVertexBuffer;
        private static PositionVertexBuffer11 ellipseWithoutStartPointVertexBuffer;
        private static EllipseShader11 ellipseShader;

  
        // Draw an ellipse with the specified semi-major axis and eccentricity. The orbit is drawn over a single period,
        // fading from full brightness at the given eccentric anomaly.
        //
        // In order to match exactly the position at which a planet is drawn, the planet's position at the current time
        // must be passed as a parameter. positionNow is in the current coordinate system of the render context, not the
        // translated and rotated system of the orbital plane.
        public static void DrawEllipse(RenderContext11 renderContext, double semiMajorAxis, double eccentricity, double eccentricAnomaly, Color color, Matrix3d worldMatrix, Vector3d positionNow)
        {
            if (ellipseShader == null)
            {
                ellipseShader = new EllipseShader11();
            }

            if (ellipseVertexBuffer == null)
            {
                ellipseVertexBuffer = CreateEllipseVertexBuffer( 500);
            }

            var savedWorld = renderContext.World;
            renderContext.World = worldMatrix;

            renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineStrip;

            renderContext.SetVertexBuffer(ellipseVertexBuffer);

            ellipseShader.UseShader(renderContext, semiMajorAxis, eccentricity, eccentricAnomaly, new SharpDX.Color(color.R, color.G, color.B, color.A), savedWorld, positionNow);

            renderContext.devContext.Draw(ellipseVertexBuffer.Count, 0);
            
            renderContext.World = savedWorld;
        }


        // This version of DrawEllipse works without a 'head' point
        public static void DrawEllipse(RenderContext11 renderContext, double semiMajorAxis, double eccentricity, double eccentricAnomaly, Color color, Matrix3d worldMatrix)
        {
            if (ellipseShader == null)
            {
                ellipseShader = new EllipseShader11();
            }

            if (ellipseWithoutStartPointVertexBuffer == null)
            {
                ellipseWithoutStartPointVertexBuffer = CreateEllipseVertexBufferWithoutStartPoint(360);
            }

            var savedWorld = renderContext.World;
            renderContext.World = worldMatrix;

            renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineStrip;

            renderContext.SetVertexBuffer(ellipseWithoutStartPointVertexBuffer);

            ellipseShader.UseShader(renderContext, semiMajorAxis, eccentricity, eccentricAnomaly, new SharpDX.Color(color.R, color.G, color.B, color.A), savedWorld, new Vector3d(0.0, 0.0, 0.0));

            renderContext.devContext.Draw(ellipseWithoutStartPointVertexBuffer.Count, 0);

            renderContext.World = savedWorld;
        }


        public static PositionVertexBuffer11 CreateEllipseVertexBuffer(int vertexCount)
        {
            var vb = new PositionVertexBuffer11( vertexCount,RenderContext11.PrepDevice);
            var verts = (SharpDX.Vector3[])vb.Lock(0,0);
            var index = 0;
            // Pack extra samples into the front of the orbit to avoid obvious segmentation
            // when viewed from near the planet or moon.
            for (var i = 0; i < vertexCount / 2; ++i)
            {
                verts[index++] = new SharpDX.Vector3(2.0f * (float)i / (float)vertexCount * 0.05f, 0.0f, 0.0f);
            }
            for (var i = 0; i < vertexCount / 2; ++i)
            {
                verts[index++] = new SharpDX.Vector3(2.0f * (float)i / (float)vertexCount * 0.95f + 0.05f, 0.0f, 0.0f);
            }

            vb.Unlock();


            return vb;
        }

        public static PositionVertexBuffer11 CreateEllipseVertexBufferWithoutStartPoint(int vertexCount)
        {
            var vb = new PositionVertexBuffer11(vertexCount, RenderContext11.PrepDevice);
            var verts = (SharpDX.Vector3[])vb.Lock(0, 0);

            // Setting a non-zero value will prevent the ellipse shader from using the 'head' point
            verts[0] = new SharpDX.Vector3(1.0e-6f, 0.0f, 0.0f);

            for (var i = 1; i < vertexCount; ++i)
            {
                verts[i] = new SharpDX.Vector3(2.0f * (float)i / (float)vertexCount, 0.0f, 0.0f);
            }

            vb.Unlock();

            return vb;
        }
    }
}
