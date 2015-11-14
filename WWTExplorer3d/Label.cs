using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

using Vector3 = SharpDX.Vector3;

namespace TerraViewer
{
    enum LabelSytle {Arrow, Telrad};
    class SkyLabel : IDisposable
    {
        static Texture11 texture;
        public double RA;
        public double Dec;
        public double Distance;
        public string Text;
        public LabelSytle Style;
        Vector3d pos;
        Vector3 center = new Vector3(0, 0, 0);
        Rectangle rect = new Rectangle(0, 0, 20, 20);

        Text3dBatch textBatch = null;
        public SkyLabel(RenderContext11 renderContext, double ra, double dec, string text, LabelSytle style, double distance)
        {
            RA = ra;
            Dec = dec;
            Text = text;
            Style = style;

            Distance = distance;

            if (texture == null)
            {
                texture = Texture11.FromBitmap(Properties.Resources.circle, 0);
            }

            Vector3d up = new Vector3d();
            Vector3d textPos = new Vector3d();
            if (Earth3d.MainWindow.SolarSystemMode)
            {
                pos = Coordinates.RADecTo3d(ra, -dec, distance);
                up = Coordinates.RADecTo3d(ra, -dec + 90, distance);

                pos.RotateX(Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI);
                pos.Add(Planets.GetPlanet3dLocation(SolarSystemObjects.Earth));
                
                up.RotateX(Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI);
                up.Add(Planets.GetPlanet3dLocation(SolarSystemObjects.Earth));
           }
            else
            {
                pos = Coordinates.RADecTo3d(ra+12, dec, distance);
                textPos = Coordinates.RADecTo3d(ra + 12, dec + 2, distance);
                up = Coordinates.RADecTo3d(ra+12, dec + 92, distance);

            }
            center = new Vector3(9, 9, 0);

            textBatch = new Text3dBatch(24);
            if (style == LabelSytle.Telrad)
            {
                // Telrad-style labels are always screen-aligned
                Text3d t3 = new Text3d(new Vector3d(0, 0, 0.1), new Vector3d(0, 1, 0), text, 20, .01);
                t3.alignment = Text3d.Alignment.Left;
                textBatch.Add(t3);
            }
            else
            {
                up.Normalize();
                textPos.Normalize();
                textBatch.Add(new Text3d(textPos, up, text, 20, .0005));
            }
        }

        public SkyLabel(RenderContext11 renderContext, Vector3d point, string text, LabelSytle style)
        {

            Text = text;
            Style = style;

            if (texture == null)
            {
                texture = Texture11.FromBitmap(Properties.Resources.circle, 0);
            }


            pos = point;

            center = new Vector3(9, 9, 0);

            textBatch = new Text3dBatch(80);

            if (style == LabelSytle.Telrad)
            {
                // Telrad-style labels are always screen-aligned
                Text3d t3 = new Text3d(new Vector3d(0, 0, 0.1), new Vector3d(0, 1, 0), text, 20, .01);
                t3.alignment = Text3d.Alignment.Left;
                textBatch.Add(t3);
            }
            else
            {
                // This will produce sky or orbit aligned text
                textBatch.Add(new Text3d(pos, new Vector3d(0,1,0), text, 20, .01));
            }
        }

        public void Draw(RenderContext11 renderContext, bool space3d)
        {
            Vector3d cam = Vector3d.TransformCoordinate(Earth3d.MainWindow.RenderContext11.CameraPosition, Matrix3d.Invert(Earth3d.WorldMatrix));

            if (!space3d)
            {
                if (Vector3d.Dot(cam, pos) < 0)
                {
                    return;
                }
            }
            Vector3d temp = pos;

            if (Earth3d.MainWindow.SolarSystemMode)
            {
                temp.Add( Earth3d.MainWindow.viewCamera.ViewTarget);
            }

            Matrix3d wvp = renderContext.World * renderContext.View * renderContext.Projection;

            Vector3 screenPos = Vector3.Project(temp.Vector311, renderContext.ViewPort.X, renderContext.ViewPort.Y, renderContext.ViewPort.Width, renderContext.ViewPort.Height, 0, 1, wvp.Matrix11);
            
            // Get the w component of the transformed object position; if it's negative the
            // object is behind the viewer.
            double w = wvp.M14 * temp.X + wvp.M24 * temp.Y + wvp.M34 * temp.Z + wvp.M44;
            if (w < 0.0 && Earth3d.MainWindow.SolarSystemMode)
            {
                // Don't show labels that are behind the viewer
                return;
            }


            screenPos = new Vector3((float)(int)screenPos.X, (float)(int)screenPos.Y, 1);

            Sprite2d.Draw2D(renderContext, texture, new SizeF(20, 20), new PointF(0, 0), 0, new PointF(screenPos.X, screenPos.Y), Color.White);

            if (Earth3d.MainWindow.SolarSystemMode || Style == LabelSytle.Telrad)
            {
                Matrix3d worldMatrix = renderContext.World;
                Matrix3d viewMatrix = renderContext.View;
                Matrix3d projectionMatrix = renderContext.Projection;

                double labelScale = Earth3d.MainWindow.SolarSystemMode ? 8.0 : 30.0;
                renderContext.World =
                    Matrix3d.Scaling(labelScale, labelScale, 1.0) *
                    Matrix3d.Translation(screenPos.X + 10.0, -screenPos.Y, 0.0) *
                    Matrix3d.Translation(-renderContext.ViewPort.Width / 2, renderContext.ViewPort.Height / 2, 0);
                renderContext.View = Matrix3d.Identity;
                renderContext.Projection = Matrix3d.OrthoLH(renderContext.ViewPort.Width, renderContext.ViewPort.Height, 1, -1);

                renderContext.BlendMode = BlendMode.PremultipliedAlpha;
                textBatch.Draw(renderContext, 1, Color.White);

                renderContext.World = worldMatrix;
                renderContext.View = viewMatrix;
                renderContext.Projection = projectionMatrix;
            }
            else
            {
                renderContext.BlendMode = BlendMode.PremultipliedAlpha;
                textBatch.Draw(renderContext, 1, Color.White);
            }

            //todo11 Implement this 
            //sprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortTexture);

            //sprite.Draw(texture, rect, center, screenPos, Color.White);
            //Rectangle recttext = new Rectangle((int)(screenPos.X + 15), (int)(screenPos.Y - 8), 0, 0);
            //Earth3d.MainWindow.labelFont.DrawText(sprite, Text, recttext,
            //DrawTextFormat.NoClip, System.Drawing.Color.White);


            //sprite.End();

        }


        #region IDisposable Members

        public void Dispose()
        {
            if (textBatch != null)
            {
                textBatch.Dispose();
            }

        }

        #endregion
    }


    public class KmlLabels : IDisposable
    {

        Texture11 texture;
        public double RA;
        public double Dec;
        public string Text;
        List<Vector3> positions;
        List<String> names;
        Vector3[] points = null;
        Vector3 center = new Vector3(0, 0, 0);
        Rectangle rect = new Rectangle(0, 0, 32, 32);

        Texture11 star = null;
        public KmlLabels()
        {
       
            texture = Texture11.FromBitmap( Properties.Resources.circle, 0);
            star = Texture11.FromBitmap(Properties.Resources.icon_rating_star_large_on, 0);

            center = new Vector3(10, 10, 0);
            positions = new List<Vector3>();
            names = new List<string>();
        }

        public void AddPoint(string name, double ra, double dec)
        {
            positions.Add( Coordinates.RADecTo3d(ra, -dec,-1).Vector311);
            names.Add(name);
            EmptyLabelBuffer();
        }


        public void AddPoint(string name, double ra, double dec, double altitude)
        {
            positions.Add( Coordinates.RADecTo3d(ra, -dec,-1-(altitude/6371000)).Vector311);
            names.Add(name);
            EmptyLabelBuffer();
        }

   
        public int CurrentTextureIndex = 2;

        List<KmlGroundOverlay> GroundOverlays = new List<KmlGroundOverlay>();

        public void AddGroundOverlay(KmlGroundOverlay overlay)
        {
            GroundOverlays.Add(overlay);
        }

        public void ClearGroundOverlays()
        {
            foreach (KmlGroundOverlay overlay in GroundOverlays)
            {
                //overlay.Dispose();
            }        
            GroundOverlays.Clear();
        }

        public int GroundOverlayCount
        {
            get
            {
                int count = 0;
                foreach (KmlGroundOverlay overlay in GroundOverlays)
                {
                    if (overlay.Icon.Texture != null)
                        ++count;
                }

                return count;
            }
        }



        // Set up render context state required for drawing ground overlays when shaders
        // are enabled.
        public void SetupGroundOverlays(RenderContext11 renderContext)
        {
            // A shader should be set up already
            if (renderContext.Shader == null)
            {
                renderContext.SetupBasicEffect(BasicEffect.TextureOnly, 1.0f, Color.White);
            }

            // Count the number of overlays so that we can choose the appropriate shader
            int overlayCount = 0;
            foreach (KmlGroundOverlay overlay in GroundOverlays)
            {
                if (overlay.Icon.Texture != null)
                {
                    ++overlayCount;
                }
            }

            overlayCount = Math.Min(PlanetShader11.MaxOverlayTextures, overlayCount);
            if (overlayCount == 0)
            {
                // No work to do
                return;
            }

            // Get a shader identical to the one currently in use, but which supports
            // the required number of overlays.
            PlanetShaderKey key = renderContext.Shader.Key;
            key.overlayTextureCount = overlayCount;
            PlanetShader11 overlayShader = PlanetShader11.GetPlanetShader(renderContext.Device, key);
            renderContext.Shader = overlayShader;
            renderContext.Shader.DiffuseColor = new SharpDX.Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            SharpDX.Direct3D11.DeviceContext devContext = renderContext.Device.ImmediateContext;
            int overlayIndex = 0;
            foreach (KmlGroundOverlay overlay in GroundOverlays)
            {
                Texture11 texture = overlay.Icon.Texture;
                if ( texture != null)
                {
                    if (overlayIndex < PlanetShader11.MaxOverlayTextures)
                    {
                        renderContext.Shader.SetOverlayTextureMatrix(overlayIndex, overlay.GetMatrix().Matrix11);
                        renderContext.Shader.SetOverlayTextureColor(overlayIndex, overlay.color);
                        renderContext.Shader.SetOverlayTexture(overlayIndex, texture.ResourceView);
                    }
                    ++overlayIndex;
                }
            }

        }

    



        public void ClearPoints()
        {
            LabelItemClicked = null;
            lastHoverIndex = -1;
            positions.Clear();
            names.Clear();        
            EmptyLabelBuffer();
        }


        public void DrawLabels(RenderContext11 renderContext)
        {
            if (positions.Count < 1)
            {
                return;
            }
            InitLabelBuffer();


            renderContext.SetVertexBuffer(labelBuffer);
            renderContext.BlendMode = BlendMode.Alpha;
            renderContext.DepthStencilMode = DepthStencilMode.Off;
            renderContext.setRasterizerState(TriangleCullMode.Off);
            SharpDX.Matrix mvp = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            mvp.Transpose();
            PointSpriteShader11.WVPMatrix = mvp;
            PointSpriteShader11.Color = SharpDX.Color.White;

            float adjustedScale = .01f; // (float)(1 / (Earth3d.MainWindow.ZoomFactor / 360));

            PointSpriteShader11.ViewportScale = new SharpDX.Vector2((2.0f / renderContext.ViewPort.Width) * adjustedScale, (2.0f / renderContext.ViewPort.Height) * adjustedScale);
            PointSpriteShader11.PointScaleFactors = new SharpDX.Vector3(0.0f, 0.0f, 10000.0f);
            PointSpriteShader11.Use(renderContext.Device.ImmediateContext);

            renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, texture.ResourceView);

            renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.PointList;
            renderContext.devContext.Draw(labelBuffer.Count, 0);

            renderContext.Device.ImmediateContext.GeometryShader.Set(null);


        }

  

        int lastHoverIndex = -1;
        public IPlace HoverCheck(Vector3 searchPoint, IPlace defaultPlace, float distance)
        {
            searchPoint = -searchPoint;
            Vector3 dist;
            if (defaultPlace != null)
            {
                Vector3 testPoint = Coordinates.RADecTo3d(defaultPlace.RA, -defaultPlace.Dec, -1.0).Vector311;
                dist = searchPoint - testPoint;
                distance = dist.Length();
            }

            int closestItem = -1;
            int index = 0;
            foreach (Vector3 point in positions)
            {
                dist = searchPoint - point;
                if (dist.Length() < distance)
                {
                    distance = dist.Length();
                    closestItem = index;
                }
                index++;
            }

            lastHoverIndex = closestItem;

            if (closestItem == -1)
            {
                return defaultPlace;
            }

            Coordinates pnt = Coordinates.CartesianToSpherical(positions[closestItem]);
            string name = this.names[closestItem];
            if (String.IsNullOrEmpty(name))
            {
                name = string.Format("RA={0}, Dec={1}", Coordinates.FormatHMS(pnt.RA), Coordinates.FormatDMS(pnt.Dec));
            }
            TourPlace place = new TourPlace(name, pnt.Dec, pnt.RA, Classification.Unidentified, "", ImageSetType.Sky, -1);
            return place;
        }

        public static Int64 TicksAtLastSelect = HiResTimer.TickCount;

        


        void EmptyLabelBuffer()
        {
            //todo11
            if (labelBuffer != null)
            {
                labelBuffer.Dispose();
                GC.SuppressFinalize(labelBuffer);
                labelBuffer = null;
            }
   
        }
               
               
        PositionColorSizeVertexBuffer11 labelBuffer = null;

        void InitLabelBuffer()
        {
            if (labelBuffer == null)
            {
                int count = positions.Count;

                labelBuffer = new PositionColorSizeVertexBuffer11(count, RenderContext11.PrepDevice);

                PositionColorSize[] labelPoints = (PositionColorSize[])labelBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)

                int index = 0;
                foreach (Vector3 point in positions)
                {
                    labelPoints[index].Position = new SharpDX.Vector3(point.X, point.Y, point.Z);
                    labelPoints[index].Color = Color.White;
                    labelPoints[index].size = 20f;
                    index++;
                }

                labelBuffer.Unlock();

            }
        }
        #region IDisposable Members

        public void Dispose()
        {
            if (texture != null)
            {
                texture.Dispose();
                GC.SuppressFinalize(texture);
                texture = null;
            }

            if (labelBuffer != null)
            {
                labelBuffer.Dispose();
                GC.SuppressFinalize(labelBuffer);
                labelBuffer = null;
            }
        }

        #endregion

        public delegate void LabelItemClickedDelegate(int index);
        
        LabelItemClickedDelegate LabelItemClicked;


        public void RegisterClick(LabelItemClickedDelegate callBack)
        {
            LabelItemClicked += callBack;
        }

        internal void ItemClick(Vector3 searchPoint, float distance)
        {
            if (LabelItemClicked == null)
            {
                return;
            }
    
            if (lastHoverIndex > -1)
            {
                LabelItemClicked.Invoke(lastHoverIndex);
            }

            searchPoint = -searchPoint;
            Vector3 dist;
            
            int closestItem = -1;
            int index = 0;
            foreach (Vector3 point in positions)
            {
                dist = searchPoint - point;
                if (dist.Length() < distance)
                {
                    distance = dist.Length();
                    closestItem = index;
                }
                index++;
            }
            if (closestItem == -1)
            {
                return ;
            }

            LabelItemClicked.Invoke(closestItem);
        }
    }
}
