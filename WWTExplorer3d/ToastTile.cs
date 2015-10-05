using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;


namespace TerraViewer
{
    //todo11 Verify and test plot tile
    public class ToastTile : Tile
    {
        bool topDown = true;

        protected PositionTexture[,] bounds;
        protected bool backslash = false;
        List<PositionTexture> vertexList;
        readonly List<Triangle>[] childTriangleList = new List<Triangle>[4];

        protected double[] demArray;
        protected void ComputeBoundingSphere(Tile parent, double altitude)
        {
            InitializeGrids();

            var pointList = BufferPool11.GetVector3dBuffer(vertexList.Count);
            var scaleFactor = (1 + (altitude / DemScaleFactor));

            if (DemEnabled)
            {
                for (var i = 0; i < vertexList.Count; i++)
                {
                    pointList[i] = Vector3d.Scale(vertexList[i].Position, scaleFactor);
                }
            }
            else
            {
                for (var i = 0; i < vertexList.Count; i++)
                {
                    pointList[i] = vertexList[i].Position;
                }
            }

            TopLeft = new Vector3d(Vector3d.Scale(bounds[0, 0].Position, scaleFactor));
            BottomRight = new Vector3d(Vector3d.Scale(bounds[2, 2].Position, scaleFactor));
            TopRight = new Vector3d(Vector3d.Scale(bounds[2, 0].Position, scaleFactor));
            BottomLeft = new Vector3d(Vector3d.Scale(bounds[0, 2].Position, scaleFactor));
            CalcSphere(pointList);

            BufferPool11.ReturnVector3dBuffer(pointList);

            if (Level == 5 || Level == 12)
            {
                localCenter = sphereCenter;
                localCenter.Round();
            }
            else if (Level > 5)
            {
                localCenter = parent.localCenter;
            }
            ReturnBuffers();

        }

        static protected IndexBuffer11[,] slashIndexBuffer = new IndexBuffer11[4, 16];
        static protected IndexBuffer11[,] backSlashIndexBuffer = new IndexBuffer11[4, 16];
        static protected IndexBuffer11[] rootIndexBuffer = new IndexBuffer11[4];

        public override IndexBuffer11 GetIndexBuffer(int index, int accomidation)
        {
            if (level == 0)
            {
                return rootIndexBuffer[index];
            }

            if (backslash)
            {
                return backSlashIndexBuffer[index, accomidation];
            }
            return slashIndexBuffer[index, accomidation];
        }

        protected void CalcSphere(Vector3d[] list)
        {
            ConvexHull.FindEnclosingSphere(list, out sphereCenter, out sphereRadius);
        }

        public override bool IsPointInTile(double lat, double lng)
        {           
            if (level == 0)
            {
                return true;
            }

            if (level == 1)
            {
                if ((lng >= 0 && lng <= 90) && (X == 0 && Y == 1) )
                {
                    return true;
                }
                if ((lng > 90 && lng <= 180) && (X == 1 && Y == 1) )
                {
                    return true;
                }       
                if ((lng < 0 && lng >= -90) && (X == 0 && Y == 0) )
                {
                    return true;
                }             
                if ((lng < -90 && lng >= -180) && (X == 1 && Y == 0) )
                {
                    return true;
                }         
            }

            if (!DemReady || DemData == null )
            {
                return false;
            }

            var testPoint = Coordinates.GeoTo3dDouble(lat, lng);
            var top = IsLeftOfHalfSpace(TopLeft, TopRight, testPoint);
            var right = IsLeftOfHalfSpace(TopRight, BottomRight, testPoint);
            var bottom = IsLeftOfHalfSpace(BottomRight, BottomLeft, testPoint);
            var left = IsLeftOfHalfSpace(BottomLeft, TopLeft, testPoint);

            if (top && right && bottom && left)
            {
               // showSelected = true;
                return true;
            }
            return false; ;

        }

        private bool IsLeftOfHalfSpace(Vector3d pntA, Vector3d pntB, Vector3d pntTest)
        {
            pntA.Normalize();
            pntB.Normalize();
            var cross = Vector3d.Cross(pntA, pntB);

            var dot = Vector3d.Dot(cross, pntTest);

            return dot > 0;
        }
     
        public override double GetSurfacePointAltitude(double lat, double lng, bool meters)
        {

            if (level < lastDeepestLevel)
            {
                //interate children

                foreach (var childKey in childrenId)
                {
                    var child = TileCache.GetCachedTile(childKey);
                    if (child != null)
                    {
                        if (child.IsPointInTile(lat, lng))
                        {
                            var retVal = child.GetSurfacePointAltitude(lat, lng, meters);
                            if (retVal != 0)
                            {
                                return retVal;
                            }
                            break;
                        }
                    }
                }
            }
            return GetAltitudeFromLatLng(lat, lng, meters);
        }

        private double GetAltitudeFromLatLng(double lat, double lng, bool meters)
        {
            var testPoint = Coordinates.GeoTo3dDouble(lat, lng);
            var uv = DistanceCalc.GetUVFromInnerPoint(TopLeft, TopRight, BottomLeft, BottomRight, testPoint);

            // Get 4 samples and interpolate
            var uud = Math.Max(0, Math.Min(16, (uv.X * 16)));
            var vvd = Math.Max(0, Math.Min(16, (uv.Y * 16)));



            var uu = Math.Max(0, Math.Min(15, (int)(uv.X * 16)));
            var vv = Math.Max(0, Math.Min(15, (int)(uv.Y * 16)));

            var ha = uud - uu;
            var va = vvd - vv;

            if (demArray != null)
            {
                // 4 nearest neighbors
                var ul = demArray[uu + 17 * vv];
                var ur = demArray[(uu + 1) + 17 * vv];
                var ll = demArray[uu + 17 * (vv + 1)];
                var lr = demArray[(uu + 1) + 17 * (vv + 1)];

                var top = ul * (1 - ha) + ha * ur;
                var bottom = ll * (1 - ha) + ha * lr;
                var val = top * (1 - va) + va * bottom;

                return val / (meters ? 1 : DemScaleFactor);
            }

            return demAverage / (meters ? 1 : DemScaleFactor);
        }
        static int countCreatedForNow;
        public override double GetSurfacePointAltitudeNow(double lat, double lng, bool meters, int targetLevel)
        {

            if (level < targetLevel)
            {
                var yOffset = 0;
                if (dataset.Mercator || dataset.BottomsUp)
                {
                    yOffset = 1;
                }
                var xOffset = 0;

                var xMax = 2;
                var childIndex = 0;
                for (var y1 = 0; y1 < 2; y1++)
                {
                    for (var x1 = 0; x1 < xMax; x1++)
                    {
                        //  if (level < (demEnabled ? 12 : dataset.Levels))
                        if (level < dataset.Levels && level < (targetLevel + 1))
                        {
                            var child = TileCache.GetCachedTile(childrenId[childIndex]);
                            if (child == null || !child.ReadyToRender)
                            {
                                countCreatedForNow++;
                                child = TileCache.GetTileNow(level + 1, x * 2 + ((x1 + xOffset) % 2), y * 2 + ((y1 + yOffset) % 2), dataset, this);
                                childrenId[childIndex] = child.Key;
                            }
                            childIndex++;
                            if (child != null)
                            {
                                if (child.IsPointInTile(lat, lng))
                                {
                                    var retVal = child.GetSurfacePointAltitudeNow(lat, lng, meters, targetLevel);
                                    if (retVal != 0)
                                    {
                                        return retVal;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return GetAltitudeFromLatLng(lat, lng, meters);
        }




        private void InitializeGrids()
        {
            vertexList = BufferPool11.GetPositionTextureList();

            for (var i = 0; i < 4; i++)
            {
                if (childTriangleList[i] != null)
                {
                    childTriangleList[i].Clear();
                }
                else
                {
                    childTriangleList[i] = BufferPool11.GetTriagleList();
                }
            }

            bounds = new PositionTexture[3, 3];

            if (level > 0)
            {
                    
                
                var parent = Parent as ToastTile;
                if (parent == null)
                {
                    return;
                }

                var xIndex = x % 2;
                var yIndex = y % 2;

                if (level > 1)
                {
                    backslash = parent.backslash;
                }
                else
                {
                    backslash = xIndex == 1 ^ yIndex == 1;
                }


                bounds[0, 0] = parent.bounds[xIndex, yIndex];
                bounds[1, 0] = Midpoint(parent.bounds[xIndex, yIndex], parent.bounds[xIndex + 1, yIndex]);
                bounds[2, 0] = parent.bounds[xIndex + 1, yIndex];
                bounds[0, 1] = Midpoint(parent.bounds[xIndex, yIndex], parent.bounds[xIndex, yIndex + 1]);

                if (backslash)
                {
                    bounds[1, 1] = Midpoint(parent.bounds[xIndex, yIndex], parent.bounds[xIndex + 1, yIndex + 1]);
                }
                else
                {
                    bounds[1, 1] = Midpoint(parent.bounds[xIndex + 1, yIndex], parent.bounds[xIndex, yIndex + 1]);
                }

                bounds[2, 1] = Midpoint(parent.bounds[xIndex + 1, yIndex], parent.bounds[xIndex + 1, yIndex + 1]);
                bounds[0, 2] = parent.bounds[xIndex, yIndex + 1];
                bounds[1, 2] = Midpoint(parent.bounds[xIndex, yIndex + 1], parent.bounds[xIndex + 1, yIndex + 1]);
                bounds[2, 2] = parent.bounds[xIndex + 1, yIndex + 1];

                if (Properties.Settings.Default.ShowElevationModel)
                {
                    bounds[0, 0].Tu = 0 ;
                    bounds[0, 0].Tv = 0 ;
                    bounds[1, 0].Tu = .5f;
                    bounds[1, 0].Tv = 0;
                    bounds[2, 0].Tu = 1;
                    bounds[2, 0].Tv = 0;

                    bounds[0, 1].Tu = 0 ;
                    bounds[0, 1].Tv = .5f;
                    bounds[1, 1].Tu = .5f;
                    bounds[1, 1].Tv = .5f;
                    bounds[2, 1].Tu = 1 ;
                    bounds[2, 1].Tv = .5f;

                    bounds[0, 2].Tu = 0 ;
                    bounds[0, 2].Tv = 1 ;
                    bounds[1, 2].Tu = .5f ;
                    bounds[1, 2].Tv = 1;
                    bounds[2, 2].Tu = 1;
                    bounds[2, 2].Tv = 1;
                }
                else
                {
                    bounds[0, 0].Tu = 0 + .002f;
                    bounds[0, 0].Tv = 0 + .002f;
                    bounds[1, 0].Tu = .5f + .002f;
                    bounds[1, 0].Tv = 0 + .002f;
                    bounds[2, 0].Tu = 1 + .002f;
                    bounds[2, 0].Tv = 0 + .002f;

                    bounds[0, 1].Tu = 0 + .002f;
                    bounds[0, 1].Tv = .5f + .002f;
                    bounds[1, 1].Tu = .5f + .002f;
                    bounds[1, 1].Tv = .5f + .002f;
                    bounds[2, 1].Tu = 1 + .002f;
                    bounds[2, 1].Tv = .5f + .002f;

                    bounds[0, 2].Tu = 0 + .002f;
                    bounds[0, 2].Tv = 1 + .002f;
                    bounds[1, 2].Tu = .5f + .002f;
                    bounds[1, 2].Tv = 1 + .002f;
                    bounds[2, 2].Tu = 1 + .002f;
                    bounds[2, 2].Tv = 1 + .002f;
                }

                vertexList.Add(bounds[0, 0]);
                vertexList.Add(bounds[1, 0]);
                vertexList.Add(bounds[2, 0]);
                vertexList.Add(bounds[0, 1]);
                vertexList.Add(bounds[1, 1]);
                vertexList.Add(bounds[2, 1]);
                vertexList.Add(bounds[0, 2]);
                vertexList.Add(bounds[1, 2]);
                vertexList.Add(bounds[2, 2]);




                if (backslash)
                {
                    childTriangleList[0].Add(new Triangle(4, 1, 0));
                    childTriangleList[0].Add(new Triangle(3, 4, 0));
                    childTriangleList[1].Add(new Triangle(5, 2, 1));
                    childTriangleList[1].Add(new Triangle(4, 5, 1));
                    childTriangleList[2].Add(new Triangle(7, 4, 3));
                    childTriangleList[2].Add(new Triangle(6, 7, 3));
                    childTriangleList[3].Add(new Triangle(8, 5, 4));
                    childTriangleList[3].Add(new Triangle(7, 8, 4));
                }
                else
                {
                    childTriangleList[0].Add(new Triangle(3, 1, 0));
                    childTriangleList[0].Add(new Triangle(4, 1, 3));
                    childTriangleList[1].Add(new Triangle(4, 2, 1));
                    childTriangleList[1].Add(new Triangle(5, 2, 4));
                    childTriangleList[2].Add(new Triangle(6, 4, 3));
                    childTriangleList[2].Add(new Triangle(7, 4, 6));
                    childTriangleList[3].Add(new Triangle(7, 5, 4));
                    childTriangleList[3].Add(new Triangle(8, 5, 7));
                }
            }
            else
            {

                if (Properties.Settings.Default.ShowElevationModel)
                {
                    bounds[0, 0] = new PositionTexture(0, -1, 0, 0, 0);
                    bounds[1, 0] = new PositionTexture(0, 0, -1, .5f, 0);
                    bounds[2, 0] = new PositionTexture(0, -1, 0, 1, 0);
                    bounds[0, 1] = new PositionTexture(1, 0, 0, 0, .5f);
                    bounds[1, 1] = new PositionTexture(0, 1, 0, .5f, .5f);
                    bounds[2, 1] = new PositionTexture(-1, 0, 0, 1, .5f);
                    bounds[0, 2] = new PositionTexture(0, -1, 0, 0, 1);
                    bounds[1, 2] = new PositionTexture(0, 0, 1, .5f, 1);
                    bounds[2, 2] = new PositionTexture(0, -1, 0, 1, 1);

                }
                else
                {
                    bounds[0, 0] = new PositionTexture(0, -1, 0, 0 + .002f, 0 + .002f);
                    bounds[1, 0] = new PositionTexture(0, 0, -1, .5f + .002f, 0 + .002f);
                    bounds[2, 0] = new PositionTexture(0, -1, 0, 1 + .002f, 0 + .002f);
                    bounds[0, 1] = new PositionTexture(1, 0, 0, 0 + .002f, .5f + .002f);
                    bounds[1, 1] = new PositionTexture(0, 1, 0, .5f + .002f, .5f + .002f);
                    bounds[2, 1] = new PositionTexture(-1, 0, 0, 1 + .002f, .5f + .002f);
                    bounds[0, 2] = new PositionTexture(0, -1, 0, 0 + .002f, 1 + .002f);
                    bounds[1, 2] = new PositionTexture(0, 0, 1, .5f + .002f, 1 + .002f);
                    bounds[2, 2] = new PositionTexture(0, -1, 0, 1 + .002f, 1 + .002f);
                }
                vertexList.Add(bounds[0, 0]);
                vertexList.Add(bounds[1, 0]);
                vertexList.Add(bounds[2, 0]);
                vertexList.Add(bounds[0, 1]);
                vertexList.Add(bounds[1, 1]);
                vertexList.Add(bounds[2, 1]);
                vertexList.Add(bounds[0, 2]);
                vertexList.Add(bounds[1, 2]);
                vertexList.Add(bounds[2, 2]);

                childTriangleList[0].Add(new Triangle(3, 1, 0));
                childTriangleList[0].Add(new Triangle(4, 1, 3));
                childTriangleList[1].Add(new Triangle(5, 2, 1));
                childTriangleList[1].Add(new Triangle(4, 5, 1));
                childTriangleList[2].Add(new Triangle(7, 4, 3));
                childTriangleList[2].Add(new Triangle(6, 7, 3));
                childTriangleList[3].Add(new Triangle(7, 5, 4));
                childTriangleList[3].Add(new Triangle(8, 5, 7));
                // Setup default matrix of points.
            }
            VertexCount = (int)Math.Pow(4, subDivisionLevel) * 2 + 1;
        }

        private PositionTexture Midpoint(PositionTexture positionNormalTextured, PositionTexture positionNormalTextured_2)
        {
            var a1 = Vector3d.Lerp(positionNormalTextured.Position, positionNormalTextured_2.Position, .5f);
            var a1uv = Vector2d.Lerp(new Vector2d(positionNormalTextured.Tu, positionNormalTextured.Tv), new Vector2d(positionNormalTextured_2.Tu, positionNormalTextured_2.Tv), .5f);

            a1.Normalize();
            return new PositionTexture(a1, a1uv.X, a1uv.Y);
        }
        int subDivisionLevel = 4;
        bool subDivided;

        public static Mutex dumpMutex = new Mutex();
        public override void OnCreateVertexBuffer(VertexBuffer11 vb)
		{
            var dem = DemEnabled;

            if (!subDivided)
            {
                if (vertexList == null)
                {
                    InitializeGrids();
                }

                
                for (var i = 0; i < 4; i++)
                {
                    var count = subDivisionLevel;
                    while (count-- > 1)
                    {
                        var newList = BufferPool11.GetTriagleList();
                        foreach (var tri in childTriangleList[i])
                        {
                            tri.SubDivide(newList, vertexList);
                        }
                        BufferPool11.ReturnTriangleList(childTriangleList[i]);
                        childTriangleList[i] = newList;
                    }
                }
                subDivided = true;
            }

            //for (int i = 0; i < 4; i++)
            //{
            //    indexBuffer[i] = BufferPool11.GetShortIndexBuffer(childTriangleList[i].Count * 3);
            //}
            var indexCount = childTriangleList[0].Count * 3;


            demIndex = 0;
            try
            {
               
                // Create a vertex buffer 
                var verts = (PositionNormalTexturedX2[])vb.Lock(0, 0); // Lock the buffer (which will return our structs)

                var index = 0;
                if (dem && level > 1)
                {
                    demArray = new double[17 * 17];
                    if (backslash)
                    {
                        if (backslashYIndex == null)
                        {
                            tempBackslashYIndex = new byte[demSize];
                            tempBackslashXIndex = new byte[demSize];
                        }
                    }
                    else
                    {
                        if (slashYIndex == null)
                        {
                            tempSlashYIndex = new byte[demSize];
                            tempSlashXIndex = new byte[demSize];
                        }
                    }
                }

                foreach (var vert in vertexList)
                {
                    if (dem)
                    {
                        // todo map this with backslash as well
                        verts[index++] = GetMappedVertex(vert);
                    }
                    else
                    {
                        verts[index++] = vert.PositionNormalTextured(localCenter, backslash );
                    }
                    demIndex++;
                }

                vb.Unlock();
                TriangleCount = childTriangleList[0].Count*4;

                var quarterDivisions = SubDivisions / 2;
                var part = 0;
                foreach (var triList in childTriangleList)
                {
                    if (GetIndexBuffer(part, 0) == null)
                    {
                        var indexArray = new short[indexCount];
                        var indexer = 0;
                        //dumpMutex.WaitOne();
                        //System.Diagnostics.Debug.WriteLine("start Index dump:" + part.ToString() + ";" + (backslash ? "Backslash" : "Slash"));
                        foreach (var tri in triList)
                        {
                            indexArray[indexer++] = (short)tri.A;
                            indexArray[indexer++] = (short)tri.B;
                            indexArray[indexer++] = (short)tri.C;
                            //WriteDebugVertex(tri.A);
                            //WriteDebugVertex(tri.B);
                            //WriteDebugVertex(tri.C);
                        }
                        //System.Diagnostics.Debug.WriteLine("End Index dump");
                        //dumpMutex.ReleaseMutex();
                        ProcessIndexBuffer(indexArray, part);
                    }
                    part++;
                }

                if (backslash)
                {
                    if (tempBackslashXIndex != null)
                    {
                        backslashXIndex = tempBackslashXIndex;
                        backslashYIndex = tempBackslashYIndex;
                        tempBackslashXIndex = null;
                        tempBackslashYIndex = null;
                    }
                }
                else
                {
                    if (tempSlashYIndex != null)
                    {
                        slashXIndex = tempSlashXIndex;
                        slashYIndex = tempSlashYIndex;
                        tempSlashYIndex = null;
                        tempSlashXIndex = null;

                    }
                }

            }
            catch
            {
            }

            ReturnBuffers();
		}

        private void ProcessIndexBuffer(short[] indexArray, int part)
        {

            if (level == 0)
            {
                rootIndexBuffer[part] = new IndexBuffer11(RenderContext11.PrepDevice, indexArray);
                return;
            }

            for (var a = 0; a < 16; a++)
            {
                var partArray = indexArray.Clone() as short[];
                ProcessAccomindations(partArray, a);
                if (backslash)
                {
                    backSlashIndexBuffer[part, a] = new IndexBuffer11(RenderContext11.PrepDevice, partArray);
                }
                else
                {
                    slashIndexBuffer[part, a] = new IndexBuffer11(RenderContext11.PrepDevice, partArray);
                }
            }
        }

        private void ProcessAccomindations(short[] indexArray, int a)
        {
            var map = new Dictionary<short, short>();
            var gridMap = new Dictionary<int, short>();

            foreach (var index in indexArray)
            {
                var vert = vertexList[index];
                var arrayX = (int)(vert.Tu * 16 + .5);
                var arrayY = (int)(vert.Tv * 16 + .5);
                var ii = (arrayY << 8) + arrayX;

                if (!gridMap.ContainsKey(ii))
                {
                    gridMap.Add(ii, index);
                }

            }


            var sections = 16;

            if ((a & 1) == 1)
            {
                for (var x = 1; x < sections; x += 2)
                {
                    var y = sections;
                    var key = (y << 8) + x;
                    var val = (y << 8) + x + 1;
                    if (gridMap.ContainsKey(key))
                    {
                        map.Add(gridMap[key], (gridMap[val]));
                    }
                }
            }

            if ((a & 2) == 2)
            {
                for (var y = 1; y < sections; y += 2)
                {
                    var x = sections;
                    var key = (y << 8) + x;
                    var val = ((y + 1) << 8) + x;
                    if (gridMap.ContainsKey(key))
                    {
                        map.Add(gridMap[key], (gridMap[val]));
                    }
                }
            }

            if ((a & 4) == 4)
            {
                for (var x = 1; x < sections; x += 2)
                {
                    var y = 0;
                    var key = (y << 8) + x;
                    var val = (y << 8) + x + 1;
                    if (gridMap.ContainsKey(key))
                    {
                        map.Add(gridMap[key], (gridMap[val]));
                    }
                }
            }

            if ((a & 8) == 8)
            {
                for (var y = 1; y < sections; y += 2)
                {
                    var x = 0;
                    var key = (y << 8) + x;
                    var val = ((y + 1) << 8) + x;
                    if (gridMap.ContainsKey(key))
                    {
                        map.Add(gridMap[key], (gridMap[val]));
                    }
                }
            }

            if (map.Count == 0)
            {
                //nothing to process
                return;
            }

            for (var i = 0; i < indexArray.Length; i++)
            {
                if (map.ContainsKey(indexArray[i]))
                {
                    indexArray[i] = map[indexArray[i]];
                }
            }
        }

        public void WriteDebugVertex(int index)
        {
            var vert = vertexList[index];
            var arrayX = (byte)(int)(vert.Tu * 16 + .5);
            var arrayY = (byte)(int)(vert.Tv * 16 + .5);

            Debug.Write(index );
            Debug.Write("\t" );
            Debug.Write(arrayX );
            Debug.Write("\t" );
            Debug.WriteLine(arrayY );

        }

        private static byte[] slashXIndex;
        private static byte[] slashYIndex;
        private static byte[] backslashXIndex;
        private static byte[] backslashYIndex;

        private  byte[] tempSlashXIndex;
        private  byte[] tempSlashYIndex;
        private  byte[] tempBackslashXIndex;
        private  byte[] tempBackslashYIndex;


        internal PositionNormalTexturedX2 GetMappedVertex(PositionTexture vert)
        {
            var vertOut = new PositionNormalTexturedX2();
            var latLng = Coordinates.CartesianToSpherical2(vert.Position);
            //      latLng.Lng += 90;
            if (latLng.Lng < -180)
            {
                latLng.Lng += 360;
            }
            if (latLng.Lng > 180)
            {
                latLng.Lng -= 360;
            }
            //if (false)
            //{
            ////    System.Diagnostics.Debug.WriteLine(String.Format("{0},{1}", (int)(vert.Tu * 16 + .5), (int)(vert.Tv * 16 + .5)));
            //}

            if (level > 1)
            {
                var arrayX = (byte)(int)(vert.Tu * 16 + .5);
                var arrayY = (byte)(int)(vert.Tv * 16 + .5);
                demArray[arrayX + arrayY * 17] = DemData[demIndex];

                if (backslash)
                {
                    if (tempBackslashYIndex != null)
                    {
                        tempBackslashXIndex[demIndex] = arrayX;
                        tempBackslashYIndex[demIndex] = arrayY;
                    }
                }
                else
                {
                    if (tempSlashYIndex != null)
                    {
                        tempSlashXIndex[demIndex] = arrayX;
                        tempSlashYIndex[demIndex] = arrayY;
                    }
                }
            }
            
            var pos = GeoTo3dWithAltitude(latLng.Lat, latLng.Lng, false);
            vertOut.Tu = (float) vert.Tu;
            vertOut.Tv = (float) vert.Tv;

            vertOut.Lat = latLng.Lat;
            vertOut.Lng = latLng.Lng;
            vertOut.Normal = pos;
            pos = pos - localCenter;
            vertOut.Position = pos;    
            return vertOut;
        }

        public override bool CreateDemFromParent()
        {
            

            var parent = Parent as ToastTile;
            if (parent == null)
            {
                return false;
            }

            var offsetX = ((X % 2) == 1 ? 8 : 0);
            var offsetY = ((Y % 2) == 0 ? 8 : 0);


            demArray = new double[17 * 17];
            // Interpolate accross 
            for (var y = 0; y < 17; y += 2)
            {
                var copy = true;
                for (var x = 0; x < 17; x++)
                {
                    if (copy)
                    {
                        demArray[(16 - y) * 17 + x] = parent.GetDemSample((x / 2) + offsetX, (y / 2) + offsetY);
                    }
                    else
                    {
                        demArray[(16 - y) * 17 + x] =
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
            for (var y = 1; y < 17; y += 2)
            {
                for (var x = 0; x < 17; x++)
                {

                    demArray[(16 - y) * 17 + x] =
                        (
                        (
                            GetDemSample(x, y - 1) +
                            GetDemSample(x, y + 1)
                        ) / 2);

                }
            }

            // Convert the dem array back to the arranged DEM list thu slash/backslash mapping tables


            DemData = new double[demSize];
            for (var i = 0; i < demSize; i++)
            {
                if (backslash)
                {
                    DemData[i] = demArray[backslashXIndex[i] + backslashYIndex[i] * 17];
                }
                else
                {
                    DemData[i] = demArray[slashXIndex[i] + slashYIndex[i] * 17];
                }
                demAverage += DemData[i];
                    
            }

           // WriteDemIndexArrays();

            // Get Average value for new DemData table

            demAverage /= DemData.Length;

            DemReady = true;
            return true;
        }

        private static void WriteDemIndexArrays()
        {
            var sb = new StringBuilder();
            sb.Append(" byte[] backslashXIndex = new byte[] {");
            foreach (var b in backslashXIndex)
            {
                sb.Append(b.ToString());
                sb.Append(", ");
            }

            sb.Append( "};");
            sb.AppendLine("");

            sb.Append(" byte[] backslashYIndex = new byte[] {");
            foreach (var b in backslashYIndex)
            {
                sb.Append(b.ToString());
                sb.Append(", ");
            }

            sb.Append("};");
            sb.AppendLine("");

            sb.Append(" byte[] slashXIndex = new byte[] {");
            foreach (var b in slashXIndex)
            {
                sb.Append(b.ToString());
                sb.Append(", ");
            }

            sb.Append("};"); 
            
            sb.AppendLine("");

            sb.Append(" byte[] slashYIndex = new byte[] {");
            foreach (var b in slashYIndex)
            {
                sb.Append(b.ToString());
                sb.Append(", ");
            }

            sb.Append("};");

            File.WriteAllText("c:\\tmp\\demIndex.cs", sb.ToString());
            
        }

        private static void WriteArray(byte[] data)
        {
           
        }

        private double GetDemSample(int x, int y)
        {
            return demArray[(16 - y) * 17 + x];
        }

               
        int quadrant;

        private void ComputeQuadrant()
        {
            var xQuad = 0;
            var yQuad = 0;
            var tiles = (int)Math.Pow(2, level);

            if (x > (tiles / 2) - 1)
            {
                xQuad = 1;
            }

            if (y > (tiles / 2) - 1)
            {
                yQuad = 1;
            }
            quadrant = yQuad * 2 + xQuad;
        }

        public ToastTile(int level, int x, int y, IImageSet dataset, Tile parent)
        {
            this.level = level;
            this.x = x;
            this.y = y;
            this.dataset = dataset;
            topDown = !dataset.BottomsUp;
            demSize = 513;
            if (dataset.MeanRadius != 0)
            {
                DemScaleFactor = dataset.MeanRadius;
            }
            else
            {
                if (dataset.DataSetType == ImageSetType.Earth)
                {
                    DemScaleFactor = 6371000;
                }
                else
                {
                    DemScaleFactor = 3396010;
                }
            }

            ComputeQuadrant();


            ComputeBoundingSphere( parent, parent !=null ? parent.demAverage : 0);
            insideOut = Dataset.DataSetType == ImageSetType.Sky || Dataset.DataSetType == ImageSetType.Panorama;
        }

        public override void CleanUp(bool removeFromParent)
        {
            base.CleanUp(removeFromParent);
            ReturnBuffers();
            subDivided = false;
        }

        private void ReturnBuffers()
        {
            if (vertexList != null)
            {
                BufferPool11.ReturnPositionTextureList(vertexList);
                vertexList = null;
            }

            if (childTriangleList != null)
            {
                for (var i = 0; i < 4; i++)
                {
                    if (childTriangleList[i] != null)
                    {
                        BufferPool11.ReturnTriangleList(childTriangleList[i]);
                        childTriangleList[i] = null;
                    }
                }
            }
        }
    }

    class DistanceCalc
    {
        static public double LineToPoint(Vector3d l0, Vector3d l1, Vector3d p)
        {
 
            var v = l1 - l0;
            var w = p - l0;

            var dist = Vector3d.Cross(w, v).Length() / v.Length();

            return dist;
        }

        static public Vector2d GetUVFromInnerPoint(Vector3d ul, Vector3d ur, Vector3d ll, Vector3d lr, Vector3d pnt)
        {
            ul.Normalize();
            ur.Normalize();
            ll.Normalize();
            lr.Normalize();
            pnt.Normalize();

            var dUpper = LineToPoint(ul, ur, pnt);
            var dLower = LineToPoint(ll, lr, pnt);
            var dVert = dUpper + dLower;

            var dRight = LineToPoint(ur, lr, pnt);
            var dLeft = LineToPoint(ul, ll, pnt);
            var dHoriz = dRight + dLeft;

            return new Vector2d( dLeft/dHoriz, dUpper/dVert);
        }
    }

}
