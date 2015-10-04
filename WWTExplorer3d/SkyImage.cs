using System;
using System.Drawing;
using System.IO;
using SharpDX.Direct3D;

namespace TerraViewer
{
    //todo11 Verify and test SkyImage tile
    class SkyImageTile : Tile
    {
        Color paintColor = Color.White;
        bool blend = true;
        public SkyImageTile(int level, int x, int y, IImageSet dataset, Tile parent)
        {
            this.level = level;
            this.x = x;
            this.y = y;
            this.dataset = dataset;
            demSize = 513;
            DemScaleFactor = 800000;

            GetParameters();
            Height = 0;
            Width = 0;


        }

        private void GetParameters()
        {
            PixelCenterX = dataset.OffsetX;
            PixelCenterY = dataset.OffsetY;
            LatCenter = dataset.CenterY;
            LngCenter = dataset.CenterX;
            Rotation = dataset.Rotation;
            ScaleX = -(ScaleY = dataset.BaseTileDegrees);
            if (dataset.BottomsUp)
            {
                ScaleX = -ScaleX;
                Rotation = 360 - Rotation;
            }
        }

        public Bitmap Image = null;
        protected new VertexBuffer11 vertexBuffer = null;
        protected new IndexBuffer11 indexBuffer = null;
        public Byte[] ImageData = null;

        public override bool Draw3D(RenderContext11 renderContext, float transparancy, Tile parent)
        {
            InViewFrustum = true;

            transparancy = transparancy / 100;
            if (transparancy > 1f)
            {
                transparancy = 1.0f;
            }

            if (transparancy < 0f)
            {
                transparancy = 0;
            }


            if (!ReadyToRender)
            {
                TileCache.AddTileToQueue(this);
                if (texture == null)
                {
                    return false;
                }
            }

            if (!CreateGeometry(renderContext, true))
            {
                return false;
            }


            renderContext.MainTexture = texture;
                
            renderContext.SetVertexBuffer(vertexBuffer);
            renderContext.SetIndexBuffer(indexBuffer);
            renderContext.PreDraw();
            renderContext.devContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderContext.devContext.DrawIndexed(6, 0, 0);


            return true;
        }
        public override void CleanUp(bool removeFromParent)
        {
            base.CleanUp(removeFromParent);

            ImageData = null;

            if (texture != null)
            {
                texture.Dispose();
                GC.SuppressFinalize(texture);
                texture = null;
            }

            if (vertexBuffer != null)
            {
                try
                {
                    vertexBuffer.Dispose();
                    GC.SuppressFinalize(vertexBuffer);
                    vertexBuffer = null;
                }
                catch
                {
                }
            }

            if (indexBuffer != null)
            {
                try
                {
                    indexBuffer.Dispose();
                    GC.SuppressFinalize(indexBuffer);
                    indexBuffer = null;
                }
                catch
                {
                }
            }

        }

        public override void CleanUpGeometryOnly()
        {
            base.CleanUpGeometryOnly(); 
            
            if (vertexBuffer != null)
            {
                vertexBuffer.Dispose();
                GC.SuppressFinalize(vertexBuffer);
                vertexBuffer = null;
            }
            
            if (indexBuffer != null)
            {
                indexBuffer.Dispose();
                GC.SuppressFinalize(indexBuffer);
                indexBuffer = null;
            }
        }

        public void SetTexture(Bitmap bmp)
        {
            if (texture != null)
            {
                texture.Dispose();
                GC.SuppressFinalize(texture);
                texture = null;
            }

            texture = Texture11.FromBitmap(RenderContext11.PrepDevice, bmp);
            Width = bmp.Width;
            Height = bmp.Height;
        }
        
        public override bool CreateGeometry(RenderContext11 renderContext, bool uiThread)
        {
            if (texture == null || (Volitile && !ReadyToRender))
            {
                GetParameters();
                if ( ImageData != null)
                {
                    try
                    {
                        var ms = new MemoryStream(ImageData);

                        var old = texture;

                        texture = Texture11.FromStream(RenderContext11.PrepDevice, ms);
                        
                        ReadyToRender = true;
                        if (old != null)
                        {
                            old.Dispose();
                            GC.SuppressFinalize(old);
                        }
                       
                        if (Width == 0 && Height == 0)
                        {
                            Width = texture.Width;
                            Height = texture.Height;
                            if (dataset.WcsImage != null)
                            {
                                if (dataset.WcsImage.SizeX != 0)
                                {
                                    Width = dataset.WcsImage.SizeX;
                                }
                                if (dataset.WcsImage.SizeY != 0)
                                {
                                    Height = dataset.WcsImage.SizeY;
                                }

                            }
                        }
                        ms.Dispose();
                        ImageData = null;
                       
                    }
                    catch
                    {
                        return false;
                    }
                }

                if (TextureReady)
                {
                    if (dataset.WcsImage != null)
                    {
                        if (dataset.WcsImage is FitsImage)
                        {
                            SetTexture(dataset.WcsImage.GetBitmap());
                            ReadyToRender = true;
                        }
                    }
                }
                if (texture == null)
                {
                        if (dataset.WcsImage != null)
                        {
                            if (dataset.WcsImage is FitsImage)
                            {
                                SetTexture(dataset.WcsImage.GetBitmap());
                                ReadyToRender = true;
                            }
                        }

                    if (TextureReady)
                    {
                        paintColor = Color.White;
                        if (dataset.WcsImage != null)
                        {
                            paintColor = dataset.WcsImage.Color;
                            blend = !dataset.WcsImage.ColorCombine;
                        }


                        if (string.IsNullOrEmpty(FileName))
                        {
                            texture = Texture11.FromBitmap(RenderContext11.PrepDevice, Image);
                            ReadyToRender = true;
                        }
                        else
                        {

                            try
                            {
                                texture = Texture11.FromFile(RenderContext11.PrepDevice, FileName);
                                ReadyToRender = true;
           

                                ReadyToRender = true;
                                if (Width == 0 && Height == 0)
                                {
                                    Width = texture.Width;
                                    Height = texture.Height;
                                    if (dataset.WcsImage != null)
                                    {
                                        if (dataset.WcsImage.SizeX != 0)
                                        {
                                            Width = dataset.WcsImage.SizeX;
                                        }
                                        if (dataset.WcsImage.SizeY != 0)
                                        {
                                            Height = dataset.WcsImage.SizeY;
                                        }

                                    }
                                }
                            }
                            catch
                            {
                                try
                                {
                                    //texture = Texture.FromBitmap(prepDevice, bmp, Usage.AutoGenerateMipMap, Tile.PoolToUse);

                                    texture = Texture11.FromFile(RenderContext11.PrepDevice, FileName);
                                    ReadyToRender = true;
                                    Width = texture.Width;
                                    Height = texture.Height;
                                    if (dataset.WcsImage.SizeX != 0)
                                    {
                                        Width = dataset.WcsImage.SizeX;
                                    }
                                    if (dataset.WcsImage.SizeY != 0)
                                    {
                                        Height = dataset.WcsImage.SizeY;
                                    }
                                }
                                catch
                                {
                                    errored = true;
                                }
                            }

                        }
                    }
                }
                
            }

            if (vertexBuffer == null)
            {
                GetParameters();
                vertexBuffer = new VertexBuffer11(typeof(PositionNormalTexturedX2), 4, RenderContext11.PrepDevice);
                indexBuffer = new IndexBuffer11(typeof(short), 6, RenderContext11.PrepDevice);
                OnCreateVertexBuffer(vertexBuffer);

            }
                

            
            
            return true;
        }
        public override bool IsTileInFrustum(PlaneD[] frustum)
        {
            return true;
        }

        // protected const double RC = (Math.PI / 180.0);
        protected new float radius = .8f;
        protected new Vector3d GeoTo3d(double lat, double lng)
        {
            lng = -lng;

            var fac1 = (dataset.BaseTileDegrees*Height) / 2;
            var factor = Math.Tan(fac1 * RC);

            return Vector3d.TransformCoordinate(new Vector3d(1f, (lat / fac1 * factor), (lng / fac1 * factor)), Matrix);

        }



        public Vector2d GetImagePixel(Coordinates sky)
        {
            var result = new Vector2d();
            //Vector3 tangent = GeoTo3dWithAltitude(sky.Lat, sky.Lng);
            var tangent = Coordinates.RADecTo3d(sky.RA + 12, sky.Dec, 1);
            var mat = dataset.Matrix;
            mat.Invert();

            tangent = Vector3d.TransformCoordinate(tangent, mat);

            var imagePoint = Coordinates.CartesianToSpherical(tangent);
         
            result.X = (float)((imagePoint.Lng / ScaleX) + PixelCenterX);
            result.Y = (float)((imagePoint.Lat / -ScaleY) + PixelCenterY);

             return result;
        }

        public Matrix3d Matrix;
        public void ComputeMatrix()
        {
            Matrix = Matrix3d.Identity;
            Matrix = Matrix3d.Multiply(Matrix, Matrix3d.RotationX((float)(((Rotation)) / 180f * Math.PI)));
            Matrix = Matrix3d.Multiply(Matrix, Matrix3d.RotationZ((float)((LatCenter) / 180f * Math.PI)));
            Matrix = Matrix3d.Multiply(Matrix, Matrix3d.RotationY((float)(((360 - LngCenter) + 180) / 180f * Math.PI)));
        }
        public double PixelCenterX = 0.0;
        public double PixelCenterY = 0.0;
        public double LatCenter = 0.0;
        public double LngCenter = 0.0;
        public double Rotation = 0.0;
        public double ScaleX = .01;
        public double ScaleY = .01;
        public double Height = 0;
        public double Width = 0;
 
        public override void OnCreateVertexBuffer(VertexBuffer11 vb)
        {
            ComputeMatrix();
            var latMin = 0 + (ScaleY * (Height - PixelCenterY));
            var latMax = 0 - (ScaleY * PixelCenterY);
            var lngMin = 0 + (ScaleX * PixelCenterX);
            var lngMax = 0 - (ScaleX * (Width - PixelCenterX));


            var TopLeft = GeoTo3d(latMin, lngMin);
            var BottomRight = GeoTo3d(latMax, lngMax);
            var TopRight = GeoTo3d(latMin, lngMax);
            var BottomLeft = GeoTo3d(latMax, lngMin);

           
            // Create a vertex buffer 
            var verts = (PositionNormalTexturedX2[])vb.Lock(0, 0); // Lock the buffer (which will return our structs)

            verts[0].Position = TopLeft;
            verts[0].Normal = TopLeft;
            verts[0].Tu = 0;
            verts[0].Tv = 0;
            verts[1].Position = TopRight;
            verts[1].Normal = TopRight;
            verts[1].Tu = 1;
            verts[1].Tv = 0;
            verts[2].Position = BottomRight;
            verts[2].Normal = BottomRight;
            verts[2].Tu = 1;
            verts[2].Tv = 1;
            verts[3].Position = BottomLeft;
            verts[3].Normal = BottomLeft;
            verts[3].Tu = 0;
            verts[3].Tv = 1;
            vb.Unlock();

            var indexArray = (short[])indexBuffer.Lock();
            indexArray[0] = 3;
            indexArray[1] = 1;
            indexArray[2] = 0;
            indexArray[3] = 1;
            indexArray[4] = 3;
            indexArray[5] = 2;


            indexBuffer.Unlock();
        }
    }
}
