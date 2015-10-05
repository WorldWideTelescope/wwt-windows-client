using System;
using System.Collections.Generic;
using System.IO;
using SharpDX;
using SharpDX.Direct3D;
using TerraViewer.Properties;

namespace TerraViewer 
{
    class PlotTile : Tile
    {
        public PlotTile(int level, int x, int y, IImageSet Imageimageset, Tile parent)
        {
            this.level = level;
            this.x = x;
            this.y = y;
            dataset = Imageimageset;
            InViewFrustum = true;
        }

        public override void CleanUp(bool removeFromParent)
        {
            if (starVertexBuffer != null)
            {
                starVertexBuffer.Dispose();
                starVertexBuffer = null;
            }


            if (starProfile != null)
            {
                starProfile.Dispose();
                GC.SuppressFinalize(starProfile);
                starProfile = null;
            }
            TextureReady = false;
        }

        public override void CleanUpGeometryOnly()
        {
        }

        public override void CleanUpGeometryRecursive()
        {
        }

        public override bool IsTileBigEnough(RenderContext11 renderContext)
        {
            return true;
        }
    
        public override bool CreateGeometry(RenderContext11 renderContext, bool uiThread)
        {
            InitializeStarDB();


            ReadyToRender = true;
            return true;
        }
        List<Star> stars = new List<Star>();

        public void InitializeStarDB()
        {
            stars = new List<Star>();
            //string filename = Properties.Settings.Default.CahceDirectory + @"data\hip.txt";
            
            //DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=hip", filename, false, true);
            var sr = new StreamReader(FileName);
            string line;
            Star star;
            while (sr.Peek() >= 0)
            {
                line = sr.ReadLine();

                star = new Star(line);
                stars.Add(star);
            }
            sr.Close();
        }
        PositionColorSizeVertexBuffer11 starVertexBuffer;
        int starCount;
        Texture11 starProfile;
        public override bool IsTileInFrustum(PlaneD[] frustum)
        {
            return true;
        }

        public override bool Draw3D(RenderContext11 renderContext, float opacity, Tile parent)
        {
            InViewFrustum = true;
            RenderedGeneration = CurrentRenderGeneration;
            if (!ReadyToRender)
            {
                TileCache.AddTileToQueue(this);
                return false;
            }

            InViewFrustum = true;

            if (starVertexBuffer == null)
            {
                starProfile = Texture11.FromBitmap( Resources.StarProfile);
  
                var count = stars.Count;
                var index = 0;
                starCount = count;

                starVertexBuffer = new PositionColorSizeVertexBuffer11(count, RenderContext11.PrepDevice);

                var points = (PositionColorSize[])starVertexBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)
                foreach (var star in stars)
                {
                    var pos = Coordinates.RADecTo3d(star.RA + 12, star.Dec, 1f);
                    points[index].Position = pos.Vector3;
                    points[index].Color = star.Col;
                    var radDec = (.5) / Math.Pow(1.6, star.Magnitude);
                    points[index].size = (float)radDec;
                    index++;
                }
                starVertexBuffer.Unlock();
            }

            renderContext.SetVertexBuffer(starVertexBuffer);
            renderContext.BlendMode = BlendMode.Additive;
            renderContext.DepthStencilMode = DepthStencilMode.Off;
            renderContext.setRasterizerState(TriangleCullMode.Off);
            var mvp = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            mvp.Transpose();
            PointSpriteShader11.WVPMatrix = mvp;
            PointSpriteShader11.Color = Color.White;

            var adjustedScale = (float)(1 / (Earth3d.MainWindow.ZoomFactor / 360));

            PointSpriteShader11.ViewportScale = new Vector2((2.0f / renderContext.ViewPort.Width) * adjustedScale, (2.0f / renderContext.ViewPort.Height) * adjustedScale);
            PointSpriteShader11.PointScaleFactors = new Vector3(0.0f, 0.0f, 10000.0f);
            PointSpriteShader11.Use(renderContext.Device.ImmediateContext);

            renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, starProfile.ResourceView);

      
            renderContext.devContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
            renderContext.devContext.Draw(starCount, 0);

            renderContext.Device.ImmediateContext.GeometryShader.Set(null);

            // Reset blend mode so we don't mess up subsequent sky layer rendering
            renderContext.BlendMode = BlendMode.Alpha;

            return true;
        }
    }
}
