using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using System.Drawing;
namespace TerraViewer
{
    public class Sprite2d
    {
        static Buffer VertexBuffer;
        static VertexBufferBinding VertexBufferBinding;

        public static void Draw(RenderContext11 renderContext, PositionColoredTextured[] points, int count, Texture11 texture, SharpDX.Direct3D.PrimitiveTopology primitiveType, float opacity = 1.0f)
        {
            if (VertexBuffer == null)
            {
                VertexBuffer = new Buffer(renderContext.Device, System.Runtime.InteropServices.Marshal.SizeOf(points[0]) * 2500, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, System.Runtime.InteropServices.Marshal.SizeOf(points[0]));
                VertexBufferBinding = new VertexBufferBinding(VertexBuffer, System.Runtime.InteropServices.Marshal.SizeOf((points[0])), 0);

            }
            renderContext.devContext.InputAssembler.PrimitiveTopology = primitiveType;
            renderContext.BlendMode = BlendMode.Alpha;
            renderContext.setRasterizerState(TriangleCullMode.Off);
            SharpDX.Matrix mat = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            mat.Transpose();

            WarpOutputShader.MatWVP = mat;
            WarpOutputShader.Use(renderContext.devContext, texture != null, opacity);

            renderContext.SetVertexBuffer(VertexBufferBinding);

            DataBox box = renderContext.devContext.MapSubresource(VertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Utilities.Write(box.DataPointer, points, 0, count);

            renderContext.devContext.UnmapSubresource(VertexBuffer, 0);
            if (texture != null)
            {
                renderContext.devContext.PixelShader.SetShaderResource(0, texture.ResourceView);
            }
            else
            {
                renderContext.devContext.PixelShader.SetShaderResource(0, null);
            }
            renderContext.devContext.Draw(count, 0);
        }

        public static void DrawForScreen(RenderContext11 renderContext, PositionColoredTextured[] points, int count, Texture11 texture, SharpDX.Direct3D.PrimitiveTopology primitiveType)
        {
            if (VertexBuffer == null)
            {
                VertexBuffer = new Buffer(renderContext.Device, System.Runtime.InteropServices.Marshal.SizeOf(points[0]) * 2500, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, System.Runtime.InteropServices.Marshal.SizeOf(points[0]));
                VertexBufferBinding = new VertexBufferBinding(VertexBuffer, System.Runtime.InteropServices.Marshal.SizeOf((points[0])), 0);

            }

            renderContext.devContext.InputAssembler.PrimitiveTopology = primitiveType;
            renderContext.BlendMode = BlendMode.Alpha;
            renderContext.setRasterizerState(TriangleCullMode.Off);

            SharpDX.Matrix mat =
                SharpDX.Matrix.Translation(-renderContext.ViewPort.Width / 2, -renderContext.ViewPort.Height / 2, 0) *
                SharpDX.Matrix.Scaling(1f, -1f, 1f)
            * SharpDX.Matrix.OrthoLH(renderContext.ViewPort.Width, renderContext.ViewPort.Height, 1, -1);
            mat.Transpose();

            WarpOutputShader.MatWVP = mat;
            WarpOutputShader.Use(renderContext.devContext, texture != null);

            renderContext.SetVertexBuffer(VertexBufferBinding);

            DataBox box = renderContext.devContext.MapSubresource(VertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Utilities.Write(box.DataPointer, points, 0, count);

            renderContext.devContext.UnmapSubresource(VertexBuffer, 0);
            if (texture != null)
            {
                renderContext.devContext.PixelShader.SetShaderResource(0, texture.ResourceView);
            }
            else
            {
                renderContext.devContext.PixelShader.SetShaderResource(0, null);
            }
            renderContext.devContext.Draw(count, 0);
        }

        static PositionColoredTextured[] cornerPoints = new PositionColoredTextured[4];

        static Matrix3d domeMatrix = Matrix3d.Identity;

        public static Vector3d MakePosition(float centerX, float centerY, float offsetX, float offsetY, float angle)
        {

                Vector3d point = new Vector3d(centerX + offsetX, centerY + offsetY, 1);

                return point;

        }

        public static void Draw2D(RenderContext11 renderContext, Texture11 srcTexture, SizeF size, PointF rotationCenter, float rotationAngle, PointF position, System.Drawing.Color color)
        {

            cornerPoints[0].Position = MakePosition(position.X, position.Y, -size.Width / 2, -size.Height / 2, rotationAngle).Vector4;
            cornerPoints[0].Tu = 0;
            cornerPoints[0].Tv = 0;
            cornerPoints[0].Color = color;


            cornerPoints[1].Position = MakePosition(position.X, position.Y, size.Width / 2, -size.Height / 2, rotationAngle).Vector4;
            cornerPoints[1].Tu = 1;
            cornerPoints[1].Tv = 0;
            cornerPoints[1].Color = color;


            cornerPoints[2].Position = MakePosition(position.X, position.Y, -size.Width / 2, size.Height / 2, rotationAngle).Vector4;
            cornerPoints[2].Tu = 0;
            cornerPoints[2].Tv = 1;
            cornerPoints[2].Color = color;

            cornerPoints[3].Position = MakePosition(position.X, position.Y, size.Width / 2, size.Height / 2, rotationAngle).Vector4;
            cornerPoints[3].Tu = 1;
            cornerPoints[3].Tv = 1;
            cornerPoints[3].Color = color;

            renderContext.setRasterizerState(TriangleCullMode.Off);
            renderContext.DepthStencilMode = DepthStencilMode.Off;
            DrawForScreen(renderContext, cornerPoints, 4, srcTexture, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);

        }

        public static void Draw(RenderContext11 renderContext, PositionColoredTextured[] points, int count, Matrix mat, bool triangleStrip)
        {
            if (VertexBuffer == null)
            {
                VertexBuffer = new Buffer(renderContext.Device, System.Runtime.InteropServices.Marshal.SizeOf(points[0]) * 2500, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, System.Runtime.InteropServices.Marshal.SizeOf(points[0]));
                VertexBufferBinding = new VertexBufferBinding(VertexBuffer, System.Runtime.InteropServices.Marshal.SizeOf((points[0])), 0);

            }

            renderContext.devContext.InputAssembler.PrimitiveTopology = triangleStrip ? SharpDX.Direct3D.PrimitiveTopology.TriangleStrip : SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            renderContext.BlendMode = BlendMode.Alpha;
            renderContext.setRasterizerState(TriangleCullMode.Off);
            
            mat.Transpose();

            WarpOutputShader.MatWVP = mat;
            WarpOutputShader.Use(renderContext.devContext, false);

            renderContext.SetVertexBuffer(VertexBufferBinding);

            DataBox box = renderContext.devContext.MapSubresource(VertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Utilities.Write(box.DataPointer, points, 0, count);

            renderContext.devContext.UnmapSubresource(VertexBuffer, 0);
            
            renderContext.devContext.PixelShader.SetShaderResource(0, null);
            
            renderContext.devContext.Draw(count, 0);
        }

        public static void DrawLines(RenderContext11 renderContext, PositionColoredTextured[] points, int count, Matrix mat, bool strip)
        {
            if (VertexBuffer == null)
            {
                VertexBuffer = new Buffer(renderContext.Device, System.Runtime.InteropServices.Marshal.SizeOf(points[0]) * 2500, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, System.Runtime.InteropServices.Marshal.SizeOf(points[0]));
                VertexBufferBinding = new VertexBufferBinding(VertexBuffer, System.Runtime.InteropServices.Marshal.SizeOf((points[0])), 0);

            }

            renderContext.devContext.InputAssembler.PrimitiveTopology = strip ? SharpDX.Direct3D.PrimitiveTopology.LineStrip : SharpDX.Direct3D.PrimitiveTopology.LineList;
            renderContext.BlendMode = BlendMode.Alpha;
            renderContext.setRasterizerState(TriangleCullMode.Off);

            mat.Transpose();

            WarpOutputShader.MatWVP = mat;
            WarpOutputShader.Use(renderContext.devContext, false);

            renderContext.SetVertexBuffer(VertexBufferBinding);

            DataBox box = renderContext.devContext.MapSubresource(VertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            Utilities.Write(box.DataPointer, points, 0, count);

            renderContext.devContext.UnmapSubresource(VertexBuffer, 0);

            renderContext.devContext.PixelShader.SetShaderResource(0, null);

            renderContext.devContext.Draw(count, 0);
        }
    }
}
