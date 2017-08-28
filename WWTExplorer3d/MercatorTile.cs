
#if !WINDOWS_UWP
using DSMRender;
#endif

using System;
using System.Collections.Generic;

#if !WINDOWS_UWP
using System.Drawing;
#endif
using System.Text;
namespace TerraViewer
{
    public class MercatorTile : Tile
    {
        private double latMin = 0;
        private double latMax = 0;
        private double lngMin = 0;
        private double lngMax = 0;

        static protected IndexBuffer11[,] sharredIndexBuffer = null;
        public override IndexBuffer11 GetIndexBuffer(int index, int accomidation)
        {
            return sharredIndexBuffer[index, accomidation];
        }
        static MercatorTile()
        {
            sharredIndexBuffer = new IndexBuffer11[4, 16];
            for (int i = 0; i < 4; i++)
            {
                for (int a = 0; a < 16; a++)
                {
                    sharredIndexBuffer[i, a] = new IndexBuffer11(typeof(short), ((SubDivisions / 2) * (SubDivisions / 2) * 6), RenderContext11.PrepDevice);
                }
            }

            for (int a = 0; a < 16; a++)
            {

                int index = 0;
                bool flipFlop = false;
                int quarterDivisions = SubDivisions / 2;
                int part = 0;
                for (int y2 = 0; y2 < 2; y2++)
                {
                    for (int x2 = 0; x2 < 2; x2++)
                    {
                        short[] indexArray = (short[])sharredIndexBuffer[part, a].Lock();
                        index = 0;
                        for (int y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                        {
                            for (int x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
                            {
                                if (flipFlop)
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
                                else
                                {
                                    //index = ((y1 * quarterDivisions * 6) + 6 * x1);
                                    // First triangle in quad
                                    indexArray[index] = (short)(y1 * (SubDivisions + 1) + x1);
                                    indexArray[index + 2] = (short)((y1 + 1) * (SubDivisions + 1) + (x1 + 1));
                                    indexArray[index + 1] = (short)(y1 * (SubDivisions + 1) + (x1 + 1));

                                    // Second triangle in quad
                                    indexArray[index + 3] = (short)(y1 * (SubDivisions + 1) + (x1));
                                    indexArray[index + 5] = (short)((y1 + 1) * (SubDivisions + 1) + x1);
                                    indexArray[index + 4] = (short)((y1 + 1) * (SubDivisions + 1) + (x1 + 1));
                                    index += 6;
                                }
                                flipFlop = !flipFlop;
                            }
                            flipFlop = !flipFlop;

                        }
                        ProcessAccomindations(indexArray, a);


                        sharredIndexBuffer[part,a].Unlock();
                        part++;
                    }
                }
            }
        }

        private static void ProcessAccomindations(short[] indexArray, int a)
        { 
            Dictionary<short, short> map = new Dictionary<short, short>();

            if ((a & 1) == 1)
            {
                for (int x = 1; x < SubDivisions; x += 2)
                {
                    int y = 0;
                    short key = (short)(y * (SubDivisions + 1) + x);
                    short val = (short)(y * (SubDivisions + 1) + x + 1);
                    map.Add(key, val);
                }
            }

            if ((a & 2) == 2)
            {
                for (int y = 1; y < SubDivisions; y += 2)
                {
                    int x = SubDivisions;
                    short key = (short)(y * (SubDivisions + 1) + x);
                    short val = (short)((y + 1) * (SubDivisions + 1) + x);
                    map.Add(key, val);
                }
            }

            if ((a & 4) == 4)
            {
                for (int x = 1; x < SubDivisions; x += 2)
                {
                    int y = SubDivisions;
                    short key = (short)(y * (SubDivisions + 1) + x);
                    short val = (short)(y * (SubDivisions + 1) + x + 1);
                    map.Add(key, val);
                }
            }

            if ((a & 8) == 8)
            {
                for (int y = 1; y < SubDivisions; y += 2)
                {
                    int x = 0;
                    short key = (short)(y * (SubDivisions + 1) + x);
                    short val = (short)((y + 1) * (SubDivisions + 1) + x);
                    map.Add(key, val);
                }
            }

            if (map.Count == 0)
            {
                //nothing to process
                return;
            }

            for (int i = 0; i < indexArray.Length; i++)
            {
                if (map.ContainsKey(indexArray[i]))
                {
                    indexArray[i] = map[indexArray[i]];
                }
            }
        }

        Vector3d center = new Vector3d();

        protected void ComputeBoundingSphere(MercatorTile parent, double altitude)
        {

            double tileDegrees = 360 / (Math.Pow(2, this.level));

            latMin = AbsoluteMetersToLatAtZoom(y * 256, level);
            latMax = AbsoluteMetersToLatAtZoom((y + 1) * 256, level);
            lngMin = (((double)this.x * tileDegrees) - 180.0);
            lngMax = ((((double)(this.x + 1)) * tileDegrees) - 180.0);

            double latCenter = AbsoluteMetersToLatAtZoom(((y * 2) + 1) * 256, level + 1);
            double lngCenter = (lngMin + lngMax) / 2.0;

           
            if (level == 12 || level == 17 )
            {
                localCenter = Coordinates.GeoTo3dDouble(latCenter, lngCenter);
            }
            else if (level > 12)
            {
                localCenter = parent.localCenter;
            }

           
            demIndex = 0;

            if (parent != null && parent.DemData != null)
            {
                this.sphereCenter = GeoTo3dWithAltitude(latCenter, lngCenter, parent.GetAltitudeAtLatLng(latCenter, lngCenter, 1), false);
                TopLeft = GeoTo3dWithAltitude(latMin, lngMin, parent.GetAltitudeAtLatLng(latMin, lngMin, 1), false);
                BottomRight = GeoTo3dWithAltitude(latMax, lngMax, parent.GetAltitudeAtLatLng(latMax, lngMax, 1), false);
                TopRight = GeoTo3dWithAltitude(latMin, lngMax, parent.GetAltitudeAtLatLng(latMin, lngMax, 1), false);
                BottomLeft = GeoTo3dWithAltitude(latMax, lngMin, parent.GetAltitudeAtLatLng(latMax, lngMin, 1), false);
            }
            else
            {
                this.sphereCenter = GeoTo3dWithAltitude(latCenter, lngCenter, altitude, false);

                TopLeft = GeoTo3dWithAltitude(latMin, lngMin, altitude, false);
                BottomRight = GeoTo3dWithAltitude(latMax, lngMax, altitude, false);
                TopRight = GeoTo3dWithAltitude(latMin, lngMax, altitude, false);
                BottomLeft = GeoTo3dWithAltitude(latMax, lngMin, altitude, false);
            }

            center = sphereCenter;

            if (Y == 0)
            {
                TopLeft = new Vector3d(0, 1, 0);
                TopRight = new Vector3d(0, 1, 0);
            }

            if (Y == Math.Pow(2, level) - 1)
            {
                BottomRight = new Vector3d(0, -1, 0);
                BottomLeft = new Vector3d(0, -1, 0);
                
            }

            Vector3d distVect1 = TopLeft;
            distVect1.Subtract(sphereCenter);
            Vector3d distVect2 = BottomRight;
            distVect2.Subtract(sphereCenter);
            Vector3d distVect3 = TopRight;
            distVect3.Subtract(sphereCenter);
            Vector3d distVect4 = BottomLeft;
            distVect4.Subtract(sphereCenter);
            
            
            this.sphereRadius = Math.Max(Math.Max(distVect1.Length(),distVect2.Length()),Math.Max(distVect3.Length(),distVect4.Length()));
            tileDegrees = lngMax - lngMin;

          //  if (level > 14)
            {
  //              this.sphereCenter.Add(localCenter);
            }
        }

        public override bool PreCreateGeometry(RenderContext11 renderContext)
        {

#if !WINDOWS_UWP
            if (Properties.Settings.Default.Show3dCities)
            {
                // Conditionalsupport for 3d Cities based on settings
                dsm = new DSMTile();
                Vector3d center = new Vector3d();
                double radius = 0;
                if (dsm != null)
                {
                    texture = dsm.LoadMeshFile(this.filename, localCenter, out center, out radius);
                }
                if (texture != null)
                {
                    sphereCenter = center;
                    sphereRadius = radius;
                    return true;
                }

            }
#endif
            return false;
        }

#if !WINDOWS_UWP
        DSMTile dsm = null;
#endif
        static public bool SaveDsmTiles = false;
        static public string SaveDsmDirectory = "";
        static public List<string> SaveDsmTileList = new List<string>();
        static Vector3d SaveDsmLocalCenter = new Vector3d();
        public override void RenderPart(RenderContext11 renderContext, int part, float opacity, bool combine)
        {

#if !WINDOWS_UWP
            if (dsm != null && dsm.DsmIndex != null)
            {
                renderContext.SetIndexBuffer(dsm.DsmIndex);
                renderContext.SetVertexBuffer(dsm.VertexBuffer);

                // This allows you to save currently viewed DSM tiles to a directory/mesh
                if (SaveDsmTiles)
                {
                    if (!SaveDsmTileList.Contains(string.Format("{0}_{1}_{2}", level, x, y)))
                    {
                        if (SaveDsmTileList.Count == 0)
                        {
                            SaveDsmLocalCenter = localCenter;
                        }

                        Vector3d offset = localCenter - SaveDsmLocalCenter;
                        dsm.VertexBuffer.SaveToFile(string.Format("{0}\\{1}_{2}_{3}.verts", SaveDsmDirectory, level, x, y), offset);
                        dsm.DsmIndex.SaveToFile(string.Format("{0}\\{1}_{2}_{3}.index", SaveDsmDirectory, level, x, y));
                        texture.SaveToFile(string.Format("{0}\\{1}_{2}_{3}.jpg", SaveDsmDirectory, level, x, y));
                        SaveDsmTileList.Add(string.Format("{0}_{1}_{2}", level, x, y));
                    }
                }


                if (dsm.Subsets.Length > 4)
                {
                    switch (part)
                    {
                        case 0:
                            part = 2;
                            break;
                        case 1:
                            part = 3;
                          //  return;
                            break;
                        case 2:
                            part = 0;
                            
                            break;
                        case 3:
                            part = 1; 
                            
                            break;
                    }


                    renderContext.devContext.DrawIndexed(dsm.Subsets[part + 1] - dsm.Subsets[part], dsm.Subsets[part], 0);
                }
                else
                {
                    if (dsm.Subsets.Length > 0)
                    {
                        renderContext.devContext.DrawIndexed(dsm.Subsets[0] , 0, 0);

                    }

                }
            }
            else
 #endif
            {
                base.RenderPart(renderContext, part, opacity, combine);
            }
        }

      


        public override bool IsPointInTile(double lat, double lng)
        {
            if ( !this.DemReady || this.DemData == null || lat < Math.Min(latMin, latMax) || lat > Math.Max(latMax, latMin) || lng < Math.Min(lngMin, lngMax) || lng > Math.Max(lngMin, lngMax))
            {
                return false;
            }
            return true;

        }
        public override double GetSurfacePointAltitude(double lat, double lng, bool meters)
        {

            if (level < lastDeepestLevel)
            {
                //interate children
                foreach (long childKey in childrenId)
                {
                    Tile child = TileCache.GetCachedTile(childKey);
                    if (child != null)
                    {
                        if (child.IsPointInTile(lat, lng))
                        {
                            double retVal = child.GetSurfacePointAltitude(lat, lng, meters);
                            if (retVal != 0)
                            {
                                return retVal;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            double alt = GetAltitudeAtLatLng(lat, lng, meters ? 1 : DemScaleFactor);

            return alt;

        }

        public override double GetSurfacePointAltitudeNow(double lat, double lng, bool meters, int targetLevel)
        {

            if (level < targetLevel)
            {
                int yOffset = 0;
                if (dataset.Mercator || dataset.BottomsUp)
                {
                    yOffset = 1;
                }
                int xOffset = 0;

                int xMax = 2;
                int childIndex = 0;
                for (int y1 = 0; y1 < 2; y1++)
                {
                    for (int x1 = 0; x1 < xMax; x1++)
                    {
                        //  if (level < (demEnabled ? 12 : dataset.Levels))
                        if (level < dataset.Levels && level < (targetLevel + 1))
                        {
                            Tile child = TileCache.GetTileNow(level + 1, x * 2 + ((x1 + xOffset) % 2), y * 2 + ((y1 + yOffset) % 2), dataset, this);
                            childrenId[childIndex++] = child.Key;
                            if (child != null)
                            {
                                if (child.IsPointInTile(lat, lng))
                                {
                                    double retVal = child.GetSurfacePointAltitudeNow(lat, lng, meters, targetLevel);
                                    if (retVal != 0)
                                    {
                                        return retVal;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            double alt = GetAltitudeAtLatLng(lat, lng, meters ? 1 : DemScaleFactor);

            return alt;
        }




        private double GetAltitudeAtLatLng(double lat, double lng, double scaleFactor)
        {

            double height = Math.Abs(latMax - latMin);
            double width = Math.Abs(lngMax - lngMin);

            double yy = ((lat - Math.Min(latMax, latMin)) / height * 32);
            double xx = ((lng - Math.Min(lngMax, lngMin)) / width * 32);


            int indexY = Math.Min(31,(int)yy);
            int indexX = Math.Min(31,(int)xx);

            double ha = xx - indexX;
            double va = yy - indexY;

            double ul = DemData[indexY * 33 + indexX];
            double ur = DemData[indexY * 33 + (indexX+1)];
            double ll = DemData[(indexY+1) * 33 + indexX];
            double lr = DemData[(indexY + 1) * 33 + (indexX + 1)];

            double top = ul * (1 - ha) + ha * ur;
            double bottom = ll * (1 - ha) + ha * lr;
            double val = top * (1 - va) + va * bottom;

            return val / scaleFactor;
           
        }


        public override void OnCreateVertexBuffer(VertexBuffer11 vb)
        {
            
           
            
            StringBuilder sb = new StringBuilder();

            try
            {
                double lat, lng;

                int index = 0;
                double tileDegrees = 360 / (Math.Pow(2, this.level));

                latMin = AbsoluteMetersToLatAtZoom(y * 256, level);
                latMax = AbsoluteMetersToLatAtZoom((y + 1) * 256, level);
                lngMin = (((double)this.x * tileDegrees) - 180.0);
                lngMax = ((((double)(this.x + 1)) * tileDegrees) - 180.0);

                double latCenter = AbsoluteMetersToLatAtZoom(((y * 2) + 1) * 256, level + 1);
                double lngCenter = (lngMin / lngMax) / 2;

                demIndex = 0;

                tileDegrees = lngMax - lngMin;
                
                // Create a vertex buffer 
                PositionNormalTexturedX2[] verts = (PositionNormalTexturedX2[])vb.Lock(0, 0); // Lock the buffer (which will return our structs)
                double dGrid = (tileDegrees / SubDivisions);
                int x1, y1;
                double textureStep = 1.0f / SubDivisions;

                double latDegrees = latMax - latCenter;

                demIndex = 0;
                for (y1 = 0; y1 < SubDivisions / 2; y1++)
                {

                    if (y1 != SubDivisions / 2)
                    {
                        //lat = latMax - (2 * textureStep * latDegrees * (double)y1);
                        lat = AbsoluteMetersToLatAtZoom(((y + 1) * 256) - y1 * 8, level);
                    }
                    else
                    {
                        lat = latCenter;
                    }

                    for (x1 = 0; x1 <= SubDivisions; x1++)
                    {
                        if (x1 != SubDivisions)
                        {
                            lng = lngMin + (textureStep * tileDegrees * (double)x1);
                        }
                        else
                        {
                            lng = lngMax;
                        }
                        index = y1 * (SubDivisions + 1) + x1;
                        verts[index].Position = GeoTo3dWithAltitude(lat, lng, true);// Add Altitude mapping here
                        verts[index].Normal = GeoTo3dWithAltitude(lat, lng, false);
                        verts[index].Tu = (float)(x1 * textureStep) + .002f;
                        verts[index].Lat = lat;
                        verts[index].Lng = lng;
                        // For top levels the geometry is vastly out of scale for v so we adjust with real Lat calculation, otherwise
                        // simple linear interpolation will do

                         verts[index].Tv = (float)((AbsoluteLatToMetersAtZoom(lat, level) - (y * 256)) / 256f) + .002f;

                        sb.Append(verts[index].Tv.ToString() + ", " + verts[index].Tu.ToString() + "\n");
                        demIndex++;

                    }
                }
                latDegrees = latMin - latCenter;

                for (y1 = SubDivisions / 2; y1 <= SubDivisions; y1++)
                {

                    if (y1 != SubDivisions)
                    {
                        //lat = latCenter + (2 * textureStep * latDegrees * (double)(y1 - (SubDivisions / 2)));
                        lat = AbsoluteMetersToLatAtZoom(((y + 1) * 256) - y1 * 8, level);
                    }
                    else
                    {
                        lat = latMin;
                    }

                    for (x1 = 0; x1 <= SubDivisions; x1++)
                    {
                        if (x1 != SubDivisions)
                        {
                            lng = lngMin + (textureStep * tileDegrees * (double)x1);
                        }
                        else
                        {
                            lng = lngMax;
                        }
                        index = y1 * (SubDivisions + 1) + x1;
                        verts[index].Position = GeoTo3dWithAltitude(lat, lng, true);// Add Altitude mapping here
                        verts[index].Normal = GeoTo3dWithAltitude(lat, lng, false);
                        verts[index].Tu = (float)(x1 * textureStep) + .002f;
                        verts[index].Lat = lat;
                        verts[index].Lng = lng;
                        // For top levels the geometry is vastly out of scale for v so we adjust with real Lat calculation, otherwise
                        // simple linear interpolation will do

                        verts[index].Tv = (float)((AbsoluteLatToMetersAtZoom(lat, level) - (y * 256)) / 256f) + .002f;


                        sb.Append(verts[index].Tv.ToString() + ", " + verts[index].Tu.ToString()  + "\n");

                        demIndex++;
                    }
                }

                if (Y == 0)
                {
                    // Send the tops to the pole to fill in the Bing Hole
                    y1 = SubDivisions;
                    for (x1 = 0; x1 <= SubDivisions; x1++)
                    {
                        index = y1 * (SubDivisions + 1) + x1;
                        verts[index].Position = new Vector3d(0, 1, 0);// Add Altitude mapping here
                        verts[index].Normal = new Vector3d(0, 1, 0);       
                    }
                }

                if (Y == Math.Pow(2, level)-1)
                {
                    // Send the tops to the pole to fill in the Bing Hole
                    y1 = 0;
                    for (x1 = 0; x1 <= SubDivisions; x1++)
                    {
                        index = y1 * (SubDivisions + 1) + x1;
                        verts[index].Position = new Vector3d(0, -1, 0);// Add Altitude mapping here
                        verts[index].Normal = new Vector3d(0, -1, 0);
                    }
                }




                vb.Unlock();
                #region createIndex
                TriangleCount = (SubDivisions) * (SubDivisions) * 2;
                //bool flipFlop = false;
                //int quarterDivisions = SubDivisions / 2;
                //int part = 0;
                //for (int y2 = 0; y2 < 2; y2++)
                //{
                //    for (int x2 = 0; x2 < 2; x2++)
                //    {
                //        short[] indexArray = (short[])this.indexBuffer[part].Lock(0, LockFlags.None);
                //        index = 0;
                //        for (y1 = (quarterDivisions * y2); y1 < (quarterDivisions * (y2 + 1)); y1++)
                //        {
                //            for (x1 = (quarterDivisions * x2); x1 < (quarterDivisions * (x2 + 1)); x1++)
                //            {
                //                if (flipFlop)
                //                {
                //                    //index = ((y1 * quarterDivisions * 6) + 6 * x1);
                //                    // First triangle in quad
                //                    indexArray[index] = (short)(y1 * (SubDivisions + 1) + x1);
                //                    indexArray[index + 2] = (short)((y1 + 1) * (SubDivisions + 1) + x1);
                //                    indexArray[index + 1] = (short)(y1 * (SubDivisions + 1) + (x1 + 1));

                //                    // Second triangle in quad
                //                    indexArray[index + 3] = (short)(y1 * (SubDivisions + 1) + (x1 + 1));
                //                    indexArray[index + 5] = (short)((y1 + 1) * (SubDivisions + 1) + x1);
                //                    indexArray[index + 4] = (short)((y1 + 1) * (SubDivisions + 1) + (x1 + 1));
                //                    index += 6;
                //                }
                //                else
                //                {
                //                    //index = ((y1 * quarterDivisions * 6) + 6 * x1);
                //                    // First triangle in quad
                //                    indexArray[index] = (short)(y1 * (SubDivisions + 1) + x1);
                //                    indexArray[index + 2] = (short)((y1 + 1) * (SubDivisions + 1) + (x1 + 1));
                //                    indexArray[index + 1] = (short)(y1 * (SubDivisions + 1) + (x1 + 1));

                //                    // Second triangle in quad
                //                    indexArray[index + 3] = (short)(y1 * (SubDivisions + 1) + (x1 ));
                //                    indexArray[index + 5] = (short)((y1 + 1) * (SubDivisions + 1) + x1);
                //                    indexArray[index + 4] = (short)((y1 + 1) * (SubDivisions + 1) + (x1 + 1));
                //                    index += 6;
                //                }
                //                flipFlop = !flipFlop;
                //            }
                //            flipFlop = !flipFlop;

                //        }
                //        this.indexBuffer[part].Unlock();
                //        part++;
                //    }
                //}
#endregion

                string data = sb.ToString();
              //  LoadMeshFile();
            }
            catch
            {
            }
        }

  

        public MercatorTile(int level, int x, int y, IImageSet dataset, Tile parent)
        {
            this.level = level;
            this.x = x;
            this.y = y;
            this.dataset = dataset;
            if (dataset.MeanRadius != 0)
            {
                this.DemScaleFactor = dataset.MeanRadius;
            }
            else
            {
                if (dataset.DataSetType == ImageSetType.Earth)
                {
                    DemScaleFactor = 6371000;
                }
                else
                {
                    DemScaleFactor = 3396000;
                }
            }


            if (parent != null)
            {
                ComputeBoundingSphere(parent as MercatorTile, parent.demAverage);
            }
            else
            {
                ComputeBoundingSphere(parent as MercatorTile, 0);

            }
            VertexCount = ((SubDivisions + 1) * (SubDivisions + 1));

            
        }

        public override bool CreateDemFromParent()
        {
            MercatorTile parent = Parent as MercatorTile;

            if (parent == null || parent.DemData == null)
            {
                return false;
            }

            DemGeneration = parent.DemGeneration + 1;

            if (DemGeneration < 4 && parent.isHdTile)
            {
                if (MakeDemFromHDParent())
                {
                    return true;
                }
            }

            int offsetX = ((X % 2) == 1 ? 16:0);
            int offsetY = ((Y % 2) == 1 ? 16:0);


            DemData = new double[demSize];
            // Interpolate accross 
            for (int y = 0; y < 33; y+=2)
            {
                bool copy = true;
                for (int x = 0; x < 33; x++)
                {
                    if (copy)
                    {
                        DemData[(32 - y) * 33 + x] = parent.GetDemSample((x / 2) + offsetX, (y / 2) + offsetY);
                    }
                    else
                    {
                        DemData[(32 - y) * 33 + x] =
                            (
                            (
                                parent.GetDemSample((x / 2) + offsetX, (y / 2) + offsetY) +
                                parent.GetDemSample(((x / 2) + offsetX) + 1, (y / 2) + offsetY)
                            ) / 2);
                    }
                    copy = !copy;

                }
            }
            // Interpolate down
            for (int y = 1; y < 33; y += 2)
            {
                for (int x = 0; x < 33; x++)
                {

                    DemData[(32 - y) * 33 + x] =
                        (
                        (
                            GetDemSample(x , y - 1) +
                            GetDemSample(x , y + 1)
                        ) / 2);

                }
            }

            foreach (double sample in DemData)
            {
                demAverage += sample;
            }

            demAverage /= DemData.Length;
            DemReady = true;
            return true;
        }

        private bool MakeDemFromHDParent()
        {
        
            hdTile = Parent.hdTile;
      
            int count = (int)Math.Pow(2, 3 - DemGeneration);
            int tileSize = (int)Math.Pow(2, DemGeneration);

            int offsetX = (X % tileSize) * count * 32;
            int offsetY = (Y % tileSize) * count * 32;
            
         
            if (hdTile != null)
            {
                isHdTile = true;
                DemData = new double[demSize];
                int yh = 0;
                for (int yl = 0; yl < 33; yl++)
                {
                    int xh = 0;
                    for (int xl = 0; xl < 33; xl++)
                    {
                        int indexI = xl + (32 - yl) * 33;
                        DemData[indexI] = hdTile.AltitudeInMeters(yh+offsetY, xh+offsetX);
                 
                        demAverage += DemData[indexI];
                        xh += count;              
                    }
                    yh += count; 
             
                }
                demAverage /= DemData.Length;

                return true;
            }
          
            return false;
        }



        private double GetDemSample(int x, int y)
        {
            return DemData[(32 - y) * 33 + x];
        }


        public static Vector2d GetCentrePointOffsetAsTileRatio(double lat, double lon, int zoom)
        {
            double metersX = AbsoluteLonToMetersAtZoom(lon, zoom);

            double relativeXIntoCell = (metersX / GRID_SIZE) -
                Math.Floor(metersX / GRID_SIZE);

            double metersY = AbsoluteLatToMetersAtZoom(lat, zoom);

            double relativeYIntoCell = (metersY / GRID_SIZE) -
                Math.Floor(metersY / GRID_SIZE);

            return (new Vector2d(relativeXIntoCell, relativeYIntoCell));
        }

        public static double RelativeMetersToLatAtZoom(double y,
            int zoom)
        {
            double metersPerPixel = MetersPerPixel2(zoom);
            double metersY = y * metersPerPixel;

            return (RadToDeg(Math.PI / 2 - 2 * Math.Atan(Math.Exp(0 - metersY / EARTH_RADIUS))));
        }

        public static double RelativeMetersToLonAtZoom(double x,
            int zoom)
        {
            double metersPerPixel = MetersPerPixel2(zoom);
            double metersX = x * metersPerPixel;

            return (RadToDeg(metersX / EARTH_RADIUS));
        }

        public static double AbsoluteLatToMetersAtZoom(double latitude, int zoom)
        {
            double sinLat = Math.Sin(DegToRad(latitude));
            double metersY = EARTH_RADIUS / 2 * Math.Log((1 + sinLat) / (1 - sinLat));
            double metersPerPixel = MetersPerPixel2(zoom);

            return ((OFFSET_METERS - metersY)/ metersPerPixel);
        }

        public static double AbsoluteMetersToLatAtZoom(int y, int zoom)
        {
            double metersPerPixel = MetersPerPixel2(zoom);
            double metersY = (double)OFFSET_METERS - (double)y * metersPerPixel;

            return (RadToDeg(Math.PI / 2 - 2 * Math.Atan(Math.Exp(0 - metersY / EARTH_RADIUS))));
        }

        public static int AbsoluteLonToMetersAtZoom(double longitude, int zoom)
        {
            double metersX = EARTH_RADIUS * DegToRad(longitude);
            double metersPerPixel = MetersPerPixel2(zoom);

            return (int)(((metersX + OFFSET_METERS) / metersPerPixel));
        }

        public static double AbsoluteMetersToLonAtZoom(int x, int zoom)
        {
            double metersPerPixel = MetersPerPixel2(zoom);
            double metersX = x * metersPerPixel - OFFSET_METERS;

            return (RadToDeg(metersX / EARTH_RADIUS));
        }


        public static int AbsoluteLonToMetersAtZoomTile(double longitude, int zoom, int tileX)
        {
            double metersX = EARTH_RADIUS * DegToRad(longitude);
            double metersPerPixel = MetersPerPixel2(zoom);
            return ((int)((metersX + OFFSET_METERS) / metersPerPixel));
        }

        public static int AbsoluteLatToMetersAtZoomTile(double latitude, int zoom, int tileX)
        {
            double sinLat = Math.Sin(DegToRad(latitude));
            double metersY = EARTH_RADIUS / 2 * Math.Log((1 + sinLat) / (1 - sinLat));
            double metersPerPixel = MetersPerPixel2(zoom);

            return ((int)(Math.Round(OFFSET_METERS - metersY) / metersPerPixel));
        }

        public static double AbsoluteMetersToLonAtZoom(int x, int zoom, int tileY)
        {
            double metersPerPixel = MetersPerPixel2(zoom);
            double metersX = x * metersPerPixel - OFFSET_METERS;

            return (RadToDeg(metersX / EARTH_RADIUS));
        }


        private static double DegToRad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        public static double MetersPerPixel2(int zoom)
        {
            return (BASE_METERS_PER_PIXEL / (double)(1 << zoom));
        }

        private static double RadToDeg(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }
        private const double EARTH_RADIUS = 6378137;
        private const double GRID_SIZE = 256.0d;
        private const double BASE_METERS_PER_PIXEL = 156543;//163840;
        private const double OFFSET_METERS = 20037508;

    }
}