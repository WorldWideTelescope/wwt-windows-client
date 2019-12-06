using System;
using System.Collections.Generic;
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
        List<PositionTexture> vertexList = null;
        List<Triangle>[] childTriangleList = new List<Triangle>[4];

        protected double[] demArray;
        protected void ComputeBoundingSphere(Tile parent, double altitude)
        {
            InitializeGrids();

            Vector3d[] pointList = BufferPool11.GetVector3dBuffer(vertexList.Count);
            double scaleFactor = (1 + (altitude / DemScaleFactor));

            if (DemEnabled)
            {
                for (int i = 0; i < vertexList.Count; i++)
                {
                    pointList[i] = Vector3d.Scale(vertexList[i].Position, scaleFactor);
                }
            }
            else
            {
                for (int i = 0; i < vertexList.Count; i++)
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
            else
            {
                return slashIndexBuffer[index, accomidation];
            }
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

            if (!this.DemReady || this.DemData == null )
            {
                return false;
            }

            Vector3d testPoint = Coordinates.GeoTo3dDouble(lat, lng);
            bool top = IsLeftOfHalfSpace(TopLeft, TopRight, testPoint);
            bool right = IsLeftOfHalfSpace(TopRight, BottomRight, testPoint);
            bool bottom = IsLeftOfHalfSpace(BottomRight, BottomLeft, testPoint);
            bool left = IsLeftOfHalfSpace(BottomLeft, TopLeft, testPoint);

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
            Vector3d cross = Vector3d.Cross(pntA, pntB);

            double dot = Vector3d.Dot(cross, pntTest);

            return dot > 0;
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
            return GetAltitudeFromLatLng(lat, lng, meters);
        }

        private double GetAltitudeFromLatLng(double lat, double lng, bool meters)
        {
            Vector3d testPoint = Coordinates.GeoTo3dDouble(lat, lng);
            Vector2d uv = DistanceCalc.GetUVFromInnerPoint(TopLeft, TopRight, BottomLeft, BottomRight, testPoint);

            // Get 4 samples and interpolate
            double uud = Math.Max(0, Math.Min(16, (uv.X * 16)));
            double vvd = Math.Max(0, Math.Min(16, (uv.Y * 16)));

            int uu = Math.Max(0, Math.Min(15, (int)(uv.X * 16)));
            int vv = Math.Max(0, Math.Min(15, (int)(uv.Y * 16)));

            double ha = uud - uu;
            double va = vvd - vv;

            if (demArray != null)
            {
                // 4 nearest neighbors
                double ul = demArray[uu + 17 * vv];
                double ur = demArray[(uu + 1) + 17 * vv];
                double ll = demArray[uu + 17 * (vv + 1)];
                double lr = demArray[(uu + 1) + 17 * (vv + 1)];

                double top = ul * (1 - ha) + ha * ur;
                double bottom = ll * (1 - ha) + ha * lr;
                double val = top * (1 - va) + va * bottom;

                return val / (meters ? 1 : DemScaleFactor);
            }
            return demAverage / (meters ? 1 : DemScaleFactor);
        }
        static int countCreatedForNow = 0;
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
                            Tile child = TileCache.GetCachedTile(childrenId[childIndex]);
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

            return GetAltitudeFromLatLng(lat, lng, meters);
        }

        private void InitializeGrids()
        {
            vertexList = BufferPool11.GetPositionTextureList();

            for (int i = 0; i < 4; i++)
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
                    
                
                ToastTile parent = Parent as ToastTile;
                if (parent == null)
                {
                    return;
                }

                int xIndex = x % 2;
                int yIndex = y % 2;

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
            Vector3d a1 = Vector3d.Lerp(positionNormalTextured.Position, positionNormalTextured_2.Position, .5f);
            Vector2d a1uv = Vector2d.Lerp(new Vector2d(positionNormalTextured.Tu, positionNormalTextured.Tv), new Vector2d(positionNormalTextured_2.Tu, positionNormalTextured_2.Tv), .5f);

            a1.Normalize();
            return new PositionTexture(a1, a1uv.X, a1uv.Y);
        }
        int subDivisionLevel = 4;
        bool subDivided = false;

        public static Mutex dumpMutex = new Mutex();
        public override void OnCreateVertexBuffer(VertexBuffer11 vb)
		{
            bool dem = DemEnabled;

            if (!subDivided)
            {
                if (vertexList == null)
                {
                    InitializeGrids();
                }

                
                for (int i = 0; i < 4; i++)
                {
                    int count = subDivisionLevel;
                    while (count-- > 1)
                    {
                        List<Triangle> newList = BufferPool11.GetTriagleList();
                        foreach (Triangle tri in childTriangleList[i])
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
            int indexCount = childTriangleList[0].Count * 3;


            demIndex = 0;
            try
            {
               
                // Create a vertex buffer 
                PositionNormalTexturedX2[] verts = (PositionNormalTexturedX2[])vb.Lock(0, 0); // Lock the buffer (which will return our structs)

                int index = 0;
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

                foreach (PositionTexture vert in vertexList)
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

                int quarterDivisions = SubDivisions / 2;
                int part = 0;
                foreach (List<Triangle> triList in childTriangleList)
                {
                    if (GetIndexBuffer(part, 0) == null)
                    {
                        short[] indexArray = new short[indexCount];
                        int indexer = 0;

                        foreach (Triangle tri in triList)
                        {
                            indexArray[indexer++] = (short)tri.A;
                            indexArray[indexer++] = (short)tri.B;
                            indexArray[indexer++] = (short)tri.C;
                        }

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

            for (int a = 0; a < 16; a++)
            {
                short[] partArray = indexArray.Clone() as short[];
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
            Dictionary<short, short> map = new Dictionary<short, short>();
            Dictionary<int, short> gridMap = new Dictionary<int, short>();

            foreach (short index in indexArray)
            {
                PositionTexture vert = vertexList[index];
                int arrayX = (int)(vert.Tu * 16 + .5);
                int arrayY = (int)(vert.Tv * 16 + .5);
                int ii = (arrayY << 8) + arrayX;

                if (!gridMap.ContainsKey(ii))
                {
                    gridMap.Add(ii, index);
                }

            }


            int sections = 16;

            if ((a & 1) == 1)
            {
                for (int x = 1; x < sections; x += 2)
                {
                    int y = sections;
                    int key = (y << 8) + x;
                    int val = (y << 8) + x + 1;
                    if (gridMap.ContainsKey(key))
                    {
                        map.Add(gridMap[key], (gridMap[val]));
                    }
                }
            }

            if ((a & 2) == 2)
            {
                for (int y = 1; y < sections; y += 2)
                {
                    int x = sections;
                    int key = (y << 8) + x;
                    int val = ((y + 1) << 8) + x;
                    if (gridMap.ContainsKey(key))
                    {
                        map.Add(gridMap[key], (gridMap[val]));
                    }
                }
            }

            if ((a & 4) == 4)
            {
                for (int x = 1; x < sections; x += 2)
                {
                    int y = 0;
                    int key = (y << 8) + x;
                    int val = (y << 8) + x + 1;
                    if (gridMap.ContainsKey(key))
                    {
                        map.Add(gridMap[key], (gridMap[val]));
                    }
                }
            }

            if ((a & 8) == 8)
            {
                for (int y = 1; y < sections; y += 2)
                {
                    int x = 0;
                    int key = (y << 8) + x;
                    int val = ((y + 1) << 8) + x;
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

            for (int i = 0; i < indexArray.Length; i++)
            {
                if (map.ContainsKey(indexArray[i]))
                {
                    indexArray[i] = map[indexArray[i]];
                }
            }
        }

        public void WriteDebugVertex(int index)
        {
            PositionTexture vert = vertexList[index];
            byte arrayX = (byte)(int)(vert.Tu * 16 + .5);
            byte arrayY = (byte)(int)(vert.Tv * 16 + .5);

            System.Diagnostics.Debug.Write(index );
            System.Diagnostics.Debug.Write("\t" );
            System.Diagnostics.Debug.Write(arrayX );
            System.Diagnostics.Debug.Write("\t" );
            System.Diagnostics.Debug.WriteLine(arrayY );

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
            PositionNormalTexturedX2 vertOut = new PositionNormalTexturedX2();
            Coordinates latLng = Coordinates.CartesianToSpherical2(vert.Position);
            //      latLng.Lng += 90;
            if (latLng.Lng < -180)
            {
                latLng.Lng += 360;
            }
            if (latLng.Lng > 180)
            {
                latLng.Lng -= 360;
            }


            if (level > 1)
            {
                byte arrayX = (byte)(int)(vert.Tu * 16 + .5);
                byte arrayY = (byte)(int)(vert.Tv * 16 + .5);
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
            
            Vector3d pos = GeoTo3dWithAltitude(latLng.Lat, latLng.Lng, false);
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
            ToastTile parent = Parent as ToastTile;
            if (parent == null)
            {
                return false;
            }

            int offsetX = ((X % 2) == 1 ? 8 : 0);
            int offsetY = ((Y % 2) == 0 ? 8 : 0);


            demArray = new double[17 * 17];
            // Interpolate accross 
            for (int y = 0; y < 17; y += 2)
            {
                bool copy = true;
                for (int x = 0; x < 17; x++)
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
            for (int y = 1; y < 17; y += 2)
            {
                for (int x = 0; x < 17; x++)
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
            for (int i = 0; i < demSize; i++)
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
            StringBuilder sb = new StringBuilder();
            sb.Append(" byte[] backslashXIndex = new byte[] {");
            foreach (byte b in backslashXIndex)
            {
                sb.Append(b.ToString());
                sb.Append(", ");
            }

            sb.Append( "};");
            sb.AppendLine("");

            sb.Append(" byte[] backslashYIndex = new byte[] {");
            foreach (byte b in backslashYIndex)
            {
                sb.Append(b.ToString());
                sb.Append(", ");
            }

            sb.Append("};");
            sb.AppendLine("");

            sb.Append(" byte[] slashXIndex = new byte[] {");
            foreach (byte b in slashXIndex)
            {
                sb.Append(b.ToString());
                sb.Append(", ");
            }

            sb.Append("};"); 
            
            sb.AppendLine("");

            sb.Append(" byte[] slashYIndex = new byte[] {");
            foreach (byte b in slashYIndex)
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

               
        int quadrant = 0;

        private void ComputeQuadrant()
        {
            int xQuad = 0;
            int yQuad = 0;
            int tiles = (int)Math.Pow(2, this.level);

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
            this.topDown = !dataset.BottomsUp;
            demSize = 513;
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
                    DemScaleFactor = 3396010;
                }
            }

            ComputeQuadrant();


            ComputeBoundingSphere( parent, parent !=null ? parent.demAverage : 0);
            insideOut = this.Dataset.DataSetType == ImageSetType.Sky || this.Dataset.DataSetType == ImageSetType.Panorama;
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
                for (int i = 0; i < 4; i++)
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
 
            Vector3d v = l1 - l0;
            Vector3d w = p - l0;

            double dist = Vector3d.Cross(w, v).Length() / v.Length();

            return dist;
        }

        static public Vector2d GetUVFromInnerPoint(Vector3d ul, Vector3d ur, Vector3d ll, Vector3d lr, Vector3d pnt)
        {
            ul.Normalize();
            ur.Normalize();
            ll.Normalize();
            lr.Normalize();
            pnt.Normalize();

            double dUpper = LineToPoint(ul, ur, pnt);
            double dLower = LineToPoint(ll, lr, pnt);
            double dVert = dUpper + dLower;

            double dRight = LineToPoint(ur, lr, pnt);
            double dLeft = LineToPoint(ul, ll, pnt);
            double dHoriz = dRight + dLeft;

            return new Vector2d( dLeft/dHoriz, dUpper/dVert);
        }
    }

}
