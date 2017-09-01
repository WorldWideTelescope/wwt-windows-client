using System;
using System.Collections.Generic;

#if WINDOWS_UWP
using Color = Windows.UI.Color;
#else
using Color = System.Drawing.Color;
#endif

using Vector3 = SharpDX.Vector3;

namespace TerraViewer
{
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
        //  Rectangle rect = new Rectangle(0, 0, 32, 32);

        Texture11 star = null;
        public KmlLabels()
        {

            texture = Texture11.FromBitmap(Properties.Resources.circle, 0);
            star = Texture11.FromBitmap(Properties.Resources.icon_rating_star_large_on, 0);

            center = new Vector3(10, 10, 0);
            positions = new List<Vector3>();
            names = new List<string>();
        }

        public void AddPoint(string name, double ra, double dec)
        {
            positions.Add(Coordinates.RADecTo3d(ra, -dec, -1).Vector311);
            names.Add(name);
            EmptyLabelBuffer();
        }


        public void AddPoint(string name, double ra, double dec, double altitude)
        {
            positions.Add(Coordinates.RADecTo3d(ra, -dec, -1 - (altitude / 6371000)).Vector311);
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
                if (texture != null)
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
                return;
            }

            LabelItemClicked.Invoke(closestItem);
        }
    }
}
