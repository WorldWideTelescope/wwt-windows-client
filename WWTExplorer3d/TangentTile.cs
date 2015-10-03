
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
    public class TangentTile : Tile
    {
        readonly bool topDown = true;
        protected void ComputeBoundingSphere(Tile parent)
        {
            if (!topDown)
            {
                ComputeBoundingSphereBottomsUp(parent);
                return;
            }
            var tileDegrees = (float)this.dataset.BaseTileDegrees / ((float)Math.Pow(2, this.level));

            var latMin = (float)(((double)this.dataset.BaseTileDegrees / 2 - (((double)this.y) * tileDegrees)) + dataset.OffsetY);
            var latMax = (float)(((double)this.dataset.BaseTileDegrees / 2 - (((double)(this.y + 1)) * tileDegrees)) + dataset.OffsetY);
            var lngMin = (float)((((double)this.x * tileDegrees) - (float)this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX);
            var lngMax = (float)(((((double)(this.x + 1)) * tileDegrees) - (float)this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX);

            var latCenter = (latMin + latMax) / 2.0;
            var lngCenter = (lngMin + lngMax) / 2.0;

            this.sphereCenter = GeoTo3d(latCenter, lngCenter, false);
            TopLeft = GeoTo3d(latMin, lngMin, false);
            BottomRight = GeoTo3d(latMax, lngMax, false);
            TopRight = GeoTo3d(latMin, lngMax, false);
            BottomLeft = GeoTo3d(latMax, lngMin, false);

            var distVect = GeoTo3d(latMin, lngMin, false);
            distVect.Subtract(sphereCenter);
            this.sphereRadius = distVect.Length();
            tileDegrees = lngMax - lngMin;

            if (level == 0)
            {
                localCenter = sphereCenter;
            }
            else
            {
                localCenter = parent.localCenter;
            }

        }

        protected void ComputeBoundingSphereBottomsUp(Tile parent)
        {
            var tileDegrees = (double)this.dataset.BaseTileDegrees / ((double)Math.Pow(2, this.level));


            var latMin = ((double)this.dataset.BaseTileDegrees / 2 + (((double)(this.y + 1)) * tileDegrees)) + dataset.OffsetY;
            var latMax = ((double)this.dataset.BaseTileDegrees / 2 + (((double)this.y) * tileDegrees)) + dataset.OffsetY;
            var lngMin = (((double)this.x * tileDegrees) - this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX;
            var lngMax = ((((double)(this.x + 1)) * tileDegrees) - this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX;

            var latCenter = (latMin + latMax) / 2.0;
            var lngCenter = (lngMin + lngMax) / 2.0;

            this.sphereCenter = GeoTo3d(latCenter, lngCenter, false);

            TopLeft = GeoTo3d(latMin, lngMin, false);
            BottomRight = GeoTo3d(latMax, lngMax, false);
            TopRight = GeoTo3d(latMin, lngMax, false);
            BottomLeft = GeoTo3d(latMax, lngMin, false);
            var distVect = TopLeft;
            distVect.Subtract(sphereCenter);
            this.sphereRadius = distVect.Length();
            tileDegrees = lngMax - lngMin;
            if (level == 0)
            {
                localCenter = sphereCenter;
            }
            else
            {
                localCenter = parent.localCenter;
            }    
        }


        protected new Vector3d GeoTo3d(double lat, double lng, bool useLocalCenter)
        {

            lng = -lng;
            var fac1 = this.dataset.BaseTileDegrees / 2;
            var factor = Math.Tan(fac1*RC);

            var retPoint = Vector3d.TransformCoordinate(new Vector3d(1f, (lat/fac1*factor), (lng/fac1*factor)), dataset.Matrix) ;

            if (useLocalCenter)
            {
                return retPoint - localCenter;
            }
            else
            {
                return retPoint;
            }

        }
        protected Coordinates ThreeDeeToGeo(Vector3d vector)
        {
            var mat = dataset.Matrix;
            mat.Invert();

            var result = Coordinates.CartesianToSpherical(Vector3d.TransformCoordinate(vector, mat));
            result.Lng = -result.Lng;
            return result;
        }



        public Vector3d TransformPoint(double x, double y)
        {
            var tileDegrees = this.dataset.BaseTileDegrees / (Math.Pow(2, this.level));
            var pixelDegrees = tileDegrees / 256;
            var lat = (((double)this.dataset.BaseTileDegrees / 2.0 - (Y * pixelDegrees)) + this.dataset.OffsetY);
            var lng = ((X * pixelDegrees - (float)this.dataset.BaseTileDegrees / dataset.WidthFactor) + this.dataset.OffsetX);
            return GeoTo3d(lat, lng, false);
        }


        public void UnTransformPoint(Vector3d point, out double x, out double y)
        {
            var result = ThreeDeeToGeo(point);
            var tileDegrees = this.dataset.BaseTileDegrees / (Math.Pow(2, this.level));
            var pixelDegrees = tileDegrees / 256;

            y = (result.Lat - this.dataset.OffsetY + (dataset.BaseTileDegrees / 2.0)) / pixelDegrees;
            x = (result.Lng - this.dataset.OffsetX + (dataset.BaseTileDegrees / dataset.WidthFactor)) / pixelDegrees;
            return;
        }

        public override void OnCreateVertexBuffer(VertexBuffer11 vb)
		{
            for (var i = 0; i < 4; i++)
            {
                indexBuffer[i] = new IndexBuffer11(typeof(short), ((SubDivisions / 2) * (SubDivisions / 2) * 6), RenderContext11.PrepDevice);
            }
            try
            {
                if (!topDown)
                {
                    OnCreateVertexBufferBottomsUp(vb);
                    return;
                }
                double lat, lng;

                var index = 0;
                var tileDegrees = (float)this.dataset.BaseTileDegrees / ((float)Math.Pow(2, this.level));

                var latMin = (float)(((double)this.dataset.BaseTileDegrees / 2.0 - (((double)this.y) * tileDegrees)) + this.dataset.OffsetY);
                var latMax = (float)(((double)this.dataset.BaseTileDegrees / 2.0 - (((double)(this.y + 1)) * tileDegrees)) + this.dataset.OffsetY);
                var lngMin = (float)((((double)this.x * tileDegrees) - (float)this.dataset.BaseTileDegrees / dataset.WidthFactor) + this.dataset.OffsetX);
                var lngMax = (float)(((((double)(this.x + 1)) * tileDegrees) - (float)this.dataset.BaseTileDegrees / dataset.WidthFactor) + this.dataset.OffsetX);
                var tileDegreesX = lngMax - lngMin;
                var tileDegreesY = latMax - latMin;


                TopLeft = GeoTo3d(latMin, lngMin, false);
                BottomRight = GeoTo3d(latMax, lngMax, false);
                TopRight = GeoTo3d(latMin, lngMax, false);
                BottomLeft = GeoTo3d(latMax, lngMin, false);

                // Create a vertex buffer 
                var verts = (PositionNormalTexturedX2[])vb.Lock(0, 0); // Lock the buffer (which will return our structs)
                int x, y;

                var textureStep = 1.0f / SubDivisions;
                for (y = 0; y <= SubDivisions; y++)
                {
                    if (y != SubDivisions)
                    {
                        lat = latMin + (textureStep * tileDegreesY * y);
                    }
                    else
                    {
                        lat = latMax;
                    }
                    for (x = 0; x <= SubDivisions; x++)
                    {

                        if (x != SubDivisions)
                        {
                            lng = lngMin + (textureStep * tileDegreesX * x);
                        }
                        else
                        {
                            lng = lngMax;
                        }
                        index = y * (SubDivisions + 1) + x;
                        verts[index].Position = GeoTo3d(lat, lng, true);
                        verts[index].Normal = GeoTo3d(lat, lng, false);
                        verts[index].Tu = x * textureStep +.002f;
                        verts[index].Tv = y * textureStep +.002f;

                    }
                }


                vb.Unlock();
                TriangleCount = (SubDivisions) * (SubDivisions) * 2;

                var quarterDivisions = SubDivisions / 2;
                var part = 0;
                for (var y2 = 0; y2 < 2; y2++)
                {
                    for (var x2 = 0; x2 < 2; x2++)
                    {
                        var indexArray = (short[])this.indexBuffer[part].Lock();
                        index = 0;
                        for (var y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                        {
                            for (var x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
                            {
                                //index = ((y1 * quarterDivisions * 6) + 6 * x1);
                                // First triangle in quad
                                indexArray[index] = (short)(y1 * (SubDivisions + 1) + x1);
                                indexArray[index + 2] = (short)((y1 + 1) * (SubDivisions + 1) + x1);
                                indexArray[index + 1] = (short)(y1 * (SubDivisions + 1) + (x1 + 1));

                                // Second triangle in quad
                                indexArray[index + 3] = (short)(y1 * (SubDivisions + 1) + (x1 + 1));
                                indexArray[index + 5] = (short)((y1 + 1) * (SubDivisions + 1) + x1);
                                indexArray[index + 4] = (short)((y1 + 1) * (SubDivisions + 1) + (x1 + 1));
                                index += 6;
                            }
                        }
                        this.indexBuffer[part].Unlock();
                        part++;
                    }
                }
            }
            catch
            {
            }
		}

        public void OnCreateVertexBufferBottomsUp(VertexBuffer11 vb)
        {

            double lat, lng;

            var index = 0;
            var tileDegrees = (float)this.dataset.BaseTileDegrees / ((float)Math.Pow(2, this.level));


            var latMin = (float)((-90 + (((double)(this.y+1)) * tileDegrees))+dataset.OffsetY);
            var latMax = (float)((-90 + (((double)this.y) * tileDegrees))+dataset.OffsetY);
            var lngMin = (float)((((double)this.x * tileDegrees) - 180.0)+dataset.OffsetX);
            var lngMax = (float)(((((double)(this.x + 1)) * tileDegrees) - 180.0)+dataset.OffsetX);
            var tileDegreesX = lngMax - lngMin;
            var tileDegreesY = latMax - latMin;
            
            // Create a vertex buffer 
            var verts = (PositionNormalTexturedX2[])vb.Lock(0, 0); // Lock the buffer (which will return our structs)
            int x, y;

            var textureStep = 1.0f / SubDivisions;
            for (y = 0; y <= SubDivisions; y++)
            {
                if (y != SubDivisions)
                {
                    lat = latMin + (textureStep * tileDegreesY * y);
                }
                else
                {
                    lat = latMax;
                }
                for (x = 0; x <= SubDivisions; x++)
                {

                    if (x != SubDivisions)
                    {
                        lng = lngMin + (textureStep * tileDegreesX * x);
                    }
                    else
                    {
                        lng = lngMax;
                    }
                    index = y * (SubDivisions + 1) + x;
                    verts[index].Position = GeoTo3d(lat, lng, true);// Add Altitude mapping here
                    verts[index].Normal = GeoTo3d(lat, lng, false);
                    verts[index].Tu = x * textureStep;
                    verts[index].Tv = y * textureStep;

                }
            }


            vb.Unlock();
            TriangleCount = (SubDivisions) * (SubDivisions) * 2;

            var quarterDivisions = SubDivisions / 2;
            var part = 0;
            for (var y2 = 0; y2 < 2; y2++)
            {
                for (var x2 = 0; x2 < 2; x2++)
                {
                    var indexArray = (short[])this.indexBuffer[part].Lock();
                    index = 0;
                    for (var y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                    {
                        for (var x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
                        {
                            //index = ((y1 * quarterDivisions * 6) + 6 * x1);
                            // First triangle in quad
                            indexArray[index] = (short)(y1 * (SubDivisions + 1) + x1);
                            indexArray[index + 1] = (short)((y1 + 1) * (SubDivisions + 1) + x1);
                            indexArray[index + 2] = (short)(y1 * (SubDivisions + 1) + (x1 + 1));

                            // Second triangle in quad
                            indexArray[index + 3] = (short)(y1 * (SubDivisions + 1) + (x1 + 1));
                            indexArray[index + 4] = (short)((y1 + 1) * (SubDivisions + 1) + x1);
                            indexArray[index + 5] = (short)((y1 + 1) * (SubDivisions + 1) + (x1 + 1));
                            index += 6;
                        }
                    }
                    this.indexBuffer[part].Unlock();
                    part++;
                }
            }
        }
           

        public TangentTile(int level, int x, int y, IImageSet dataset, Tile parent)
        {
            this.level = level;
            this.x = x;
            this.y = y;
            this.dataset = dataset;
            this.topDown = !dataset.BottomsUp;     
            ComputeBoundingSphere(parent);
            VertexCount = ((SubDivisions + 1) * (SubDivisions + 1));

        }
    }
}
