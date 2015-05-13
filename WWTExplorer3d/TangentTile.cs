
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
        bool topDown = true;
        protected void ComputeBoundingSphere(Tile parent)
        {
            if (!topDown)
            {
                ComputeBoundingSphereBottomsUp(parent);
                return;
            }
            float tileDegrees = (float)this.dataset.BaseTileDegrees / ((float)Math.Pow(2, this.level));

            float latMin = (float)(((double)this.dataset.BaseTileDegrees / 2 - (((double)this.y) * tileDegrees)) + dataset.OffsetY);
            float latMax = (float)(((double)this.dataset.BaseTileDegrees / 2 - (((double)(this.y + 1)) * tileDegrees)) + dataset.OffsetY);
            float lngMin = (float)((((double)this.x * tileDegrees) - (float)this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX);
            float lngMax = (float)(((((double)(this.x + 1)) * tileDegrees) - (float)this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX);

            double latCenter = (latMin + latMax) / 2.0;
            double lngCenter = (lngMin + lngMax) / 2.0;

            this.sphereCenter = GeoTo3d(latCenter, lngCenter, false);
            TopLeft = GeoTo3d(latMin, lngMin, false);
            BottomRight = GeoTo3d(latMax, lngMax, false);
            TopRight = GeoTo3d(latMin, lngMax, false);
            BottomLeft = GeoTo3d(latMax, lngMin, false);

            Vector3d distVect = GeoTo3d(latMin, lngMin, false);
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
            double tileDegrees = (double)this.dataset.BaseTileDegrees / ((double)Math.Pow(2, this.level));


            double latMin = ((double)this.dataset.BaseTileDegrees / 2 + (((double)(this.y + 1)) * tileDegrees)) + dataset.OffsetY;
            double latMax = ((double)this.dataset.BaseTileDegrees / 2 + (((double)this.y) * tileDegrees)) + dataset.OffsetY;
            double lngMin = (((double)this.x * tileDegrees) - this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX;
            double lngMax = ((((double)(this.x + 1)) * tileDegrees) - this.dataset.BaseTileDegrees / dataset.WidthFactor) + dataset.OffsetX;

            double latCenter = (latMin + latMax) / 2.0;
            double lngCenter = (lngMin + lngMax) / 2.0;

            this.sphereCenter = GeoTo3d(latCenter, lngCenter, false);

            TopLeft = GeoTo3d(latMin, lngMin, false);
            BottomRight = GeoTo3d(latMax, lngMax, false);
            TopRight = GeoTo3d(latMin, lngMax, false);
            BottomLeft = GeoTo3d(latMax, lngMin, false);
            Vector3d distVect = TopLeft;
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
            double fac1 = this.dataset.BaseTileDegrees / 2;
            double factor = Math.Tan(fac1*RC);

            Vector3d retPoint = Vector3d.TransformCoordinate(new Vector3d(1f, (lat/fac1*factor), (lng/fac1*factor)), dataset.Matrix) ;

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
            Matrix3d mat = dataset.Matrix;
            mat.Invert();

            Coordinates result = Coordinates.CartesianToSpherical(Vector3d.TransformCoordinate(vector, mat));
            result.Lng = -result.Lng;
            return result;
        }



        public Vector3d TransformPoint(double x, double y)
        {
            double tileDegrees = this.dataset.BaseTileDegrees / (Math.Pow(2, this.level));
            double pixelDegrees = tileDegrees / 256;
            double lat = (((double)this.dataset.BaseTileDegrees / 2.0 - (Y * pixelDegrees)) + this.dataset.OffsetY);
            double lng = ((X * pixelDegrees - (float)this.dataset.BaseTileDegrees / dataset.WidthFactor) + this.dataset.OffsetX);
            return GeoTo3d(lat, lng, false);
        }


        public void UnTransformPoint(Vector3d point, out double x, out double y)
        {
            Coordinates result = ThreeDeeToGeo(point);
            double tileDegrees = this.dataset.BaseTileDegrees / (Math.Pow(2, this.level));
            double pixelDegrees = tileDegrees / 256;

            y = (result.Lat - this.dataset.OffsetY + (dataset.BaseTileDegrees / 2.0)) / pixelDegrees;
            x = (result.Lng - this.dataset.OffsetX + (dataset.BaseTileDegrees / dataset.WidthFactor)) / pixelDegrees;
            return;
        }

        public override void OnCreateVertexBuffer(VertexBuffer11 vb)
		{
            for (int i = 0; i < 4; i++)
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

                int index = 0;
                float tileDegrees = (float)this.dataset.BaseTileDegrees / ((float)Math.Pow(2, this.level));

                float latMin = (float)(((double)this.dataset.BaseTileDegrees / 2.0 - (((double)this.y) * tileDegrees)) + this.dataset.OffsetY);
                float latMax = (float)(((double)this.dataset.BaseTileDegrees / 2.0 - (((double)(this.y + 1)) * tileDegrees)) + this.dataset.OffsetY);
                float lngMin = (float)((((double)this.x * tileDegrees) - (float)this.dataset.BaseTileDegrees / dataset.WidthFactor) + this.dataset.OffsetX);
                float lngMax = (float)(((((double)(this.x + 1)) * tileDegrees) - (float)this.dataset.BaseTileDegrees / dataset.WidthFactor) + this.dataset.OffsetX);
                float tileDegreesX = lngMax - lngMin;
                float tileDegreesY = latMax - latMin;


                TopLeft = GeoTo3d(latMin, lngMin, false);
                BottomRight = GeoTo3d(latMax, lngMax, false);
                TopRight = GeoTo3d(latMin, lngMax, false);
                BottomLeft = GeoTo3d(latMax, lngMin, false);

                // Create a vertex buffer 
                PositionNormalTexturedX2[] verts = (PositionNormalTexturedX2[])vb.Lock(0, 0); // Lock the buffer (which will return our structs)
                int x, y;

                float textureStep = 1.0f / SubDivisions;
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

                int quarterDivisions = SubDivisions / 2;
                int part = 0;
                for (int y2 = 0; y2 < 2; y2++)
                {
                    for (int x2 = 0; x2 < 2; x2++)
                    {
                        short[] indexArray = (short[])this.indexBuffer[part].Lock();
                        index = 0;
                        for (int y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                        {
                            for (int x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
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

            int index = 0;
            float tileDegrees = (float)this.dataset.BaseTileDegrees / ((float)Math.Pow(2, this.level));


            float latMin = (float)((-90 + (((double)(this.y+1)) * tileDegrees))+dataset.OffsetY);
            float latMax = (float)((-90 + (((double)this.y) * tileDegrees))+dataset.OffsetY);
            float lngMin = (float)((((double)this.x * tileDegrees) - 180.0)+dataset.OffsetX);
            float lngMax = (float)(((((double)(this.x + 1)) * tileDegrees) - 180.0)+dataset.OffsetX);
            float tileDegreesX = lngMax - lngMin;
            float tileDegreesY = latMax - latMin;
            
            // Create a vertex buffer 
            PositionNormalTexturedX2[] verts = (PositionNormalTexturedX2[])vb.Lock(0, 0); // Lock the buffer (which will return our structs)
            int x, y;

            float textureStep = 1.0f / SubDivisions;
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

            int quarterDivisions = SubDivisions / 2;
            int part = 0;
            for (int y2 = 0; y2 < 2; y2++)
            {
                for (int x2 = 0; x2 < 2; x2++)
                {
                    short[] indexArray = (short[])this.indexBuffer[part].Lock();
                    index = 0;
                    for (int y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                    {
                        for (int x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
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
