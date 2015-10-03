using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.IO;
using System.Text;


namespace TerraViewer
{
    class SphericalTile : Tile
    {
        bool topDown = true;
        protected void ComputeBoundingSphere()
        {
            this.sphereCenter = new Vector3d(0,0,0);
            this.sphereRadius = 1.1f;
        }

        const int subDivisionsX = 120;
        const int subDivisionsY = 60;

        public override void OnCreateVertexBuffer(VertexBuffer11 vb)
		{
            indexBuffer[0] = new IndexBuffer11(typeof(short), subDivisionsX * subDivisionsY * 6, RenderContext11.PrepDevice);

            double lat, lng;

            var index = 0;
            //            double tileDegrees = 360;

            double latMin = 90;
            double latMax = -90;
            double lngMin = -180;
            double lngMax = 180;
            
            // Create a vertex buffer 
            var verts = (PositionNormalTexturedX2[])vb.Lock(0, 0); // Lock the buffer (which will return our structs)
            int x1, y1;

            var latDegrees = latMax - latMin;
            var lngDegrees = lngMax - lngMin;

            double textureStepX = 1.0f / subDivisionsX;
            double textureStepY = 1.0f / subDivisionsY;
            for (y1 = 0; y1 <= subDivisionsY; y1++)
            {

                if (y1 != subDivisionsY)
                {
                    lat = latMax - (textureStepY * latDegrees * (double)y1);
                }
                else
                {
                    lat = latMin;
                }

                for (x1 = 0; x1 <= subDivisionsX; x1++)
                {
                    if (x1 != subDivisionsX)
                    {
                        lng = lngMin + (textureStepX * lngDegrees * (double)x1);
                    }
                    else
                    {
                        lng = lngMax;
                    }
                    index = y1 * (subDivisionsX + 1) + x1;
                    verts[index].Position = GeoTo3d(lat, lng, false);// Add Altitude mapping here
                    verts[index].Normal = verts[index].Position;
                    if (domeMaster)
                    {
                        var dist = (90-lat) / 180;
                        verts[index].Tu = (float)(.5 +Math.Sin((lng+180)/180*Math.PI)*dist);
                        verts[index].Tv = (float)(.5 + Math.Cos((lng+180) / 180 * Math.PI) * dist);
                    }
                    else
                    {
                        verts[index].Tu = (float)(x1 * textureStepX);
                        verts[index].Tv = (float)(1f - (y1 * textureStepY));
                    }
                }
            }
            vb.Unlock();
            TriangleCount = (subDivisionsX) * (subDivisionsY) * 2;
            var indexArray = (short[])this.indexBuffer[0].Lock();

            for (y1 = 0; y1 < subDivisionsY; y1++)
            {
                if (!(domeMaster && y1 < subDivisionsY / 2))
                {
                    for (x1 = 0; x1 < subDivisionsX; x1++)
                    {
                        index = (y1 * subDivisionsX * 6) + 6 * x1;
                        // First triangle in quad
                        indexArray[index] = (short)(y1 * (subDivisionsX + 1) + x1);
                        indexArray[index + 2] = (short)((y1 + 1) * (subDivisionsX + 1) + x1);
                        indexArray[index + 1] = (short)(y1 * (subDivisionsX + 1) + (x1 + 1));

                        // Second triangle in quad
                        indexArray[index + 3] = (short)(y1 * (subDivisionsX + 1) + (x1 + 1));
                        indexArray[index + 5] = (short)((y1 + 1) * (subDivisionsX + 1) + x1);
                        indexArray[index + 4] = (short)((y1 + 1) * (subDivisionsX + 1) + (x1 + 1));
                    }
                }
            }
            this.indexBuffer[0].Unlock();
		}

        public override bool IsTileInFrustum(PlaneD[] frustum)
        {
            return true;
        }

        public override bool Draw3D(RenderContext11 renderContext, float transparancy, Tile parent)
        {
            RenderedGeneration = CurrentRenderGeneration;
            TilesTouched++;

            InViewFrustum = true;

            if (!ReadyToRender)
            {
                TileCache.AddTileToQueue(this);

                return false;
            }

            TilesInView++;
   
            if (!CreateGeometry(renderContext, true))
            {
                return false;
            }

            renderContext.SetVertexBuffer(vertexBuffer);

            renderContext.MainTexture = texture;

            renderContext.SetIndexBuffer( indexBuffer[0]);

            var partCount = this.TriangleCount;
            TrianglesRendered += partCount;

            renderContext.devContext.DrawIndexed(indexBuffer[0].Count, 0, 0);

            return true;
        }
        bool domeMaster;

        public override bool CreateGeometry(RenderContext11 renderContext, bool uiThread)
        {
            if (texture == null)
            {
                if (this.texture == null)
                {
                    if (TextureReady)
                    {

                        texture = BufferPool11.GetTexture(FileName);

                        var aspect = (double)texture.Width / (double)texture.Height;

                        if (aspect < 1.5)
                        {
                            domeMaster = true;
                        }

                    }
                    else
                    {
                        return false;
                    }
                }


                iTileBuildCount++;

                if (vertexBuffer == null)
                {
                    vertexBuffer = new VertexBuffer11(typeof(PositionNormalTexturedX2), VertexCount, RenderContext11.PrepDevice);
                    indexBuffer = new IndexBuffer11[4];

                    OnCreateVertexBuffer(vertexBuffer);
                    sphereRadius = 1;
                    sphereCenter = new Vector3d(0, 0, 0);
                }
            }
            ReadyToRender = true;
            return true;
        }


        public SphericalTile(int level, int x, int y, IImageSet dataset, Tile parent)
        {
            this.level = level;
            this.x = x;
            this.y = y;
            this.dataset = dataset;
            this.topDown = !dataset.BottomsUp;     
            ComputeBoundingSphere();
            VertexCount = ((subDivisionsX + 1) * (subDivisionsY + 1));
        }

    }
}