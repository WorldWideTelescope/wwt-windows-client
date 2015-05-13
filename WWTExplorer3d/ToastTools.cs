using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraViewer
{
    class ToastTools
    {

        public int Level = 0;
        public int X = 0;
        public int Y = 0;

        protected Vector3d TopLeft;
        protected Vector3d BottomRight;
        protected Vector3d TopRight;
        protected Vector3d BottomLeft;
        protected PositionTexture[,] bounds;
        protected bool backslash = false;

        public object Tag = null;

        ToastTools[] children = new ToastTools[4];
        
        public ToastTools(ToastTools parent, int level, int x, int y)
        {
            Parent = parent;
            X = x;
            Y = y;
            Level = level;
            InitializeGrids();
            TopLeft = new Vector3d(Vector3d.Scale(bounds[0, 0].Position, 1));
            BottomRight = new Vector3d(Vector3d.Scale(bounds[2, 2].Position, 1));
            TopRight = new Vector3d(Vector3d.Scale(bounds[2, 0].Position, 1));
            BottomLeft = new Vector3d(Vector3d.Scale(bounds[0, 2].Position, 1));
        }

        public ToastTools GetChild(int id)
        {
            if (children[id] == null)
            {
                int x1 =0;
                int y1 = 0;
                switch(id)
                {
                    case 0:
                        y1=0;
                        x1=0;
                        break;
                    case 1:
                        y1=0;
                        x1=1;
                        break;
                    case 2:
                        y1=1;
                        x1=0;
                        break;
                    case 3:
                        y1=1;
                        x1=1;
                        break;
                }

                children[id] = new ToastTools(this, Level + 1, X * 2 + x1, Y * 2 + y1);
            }

            return children[id];
        }

        ToastTools Parent = null;
        private void InitializeGrids()
        {

            bounds = new PositionTexture[3, 3];

            if (Level > 0)
            {
                if (Parent == null)
                {
                    return;
                }

                int xIndex = X % 2;
                int yIndex = Y % 2;

                if (Level > 1)
                {
                    backslash = Parent.backslash;
                }
                else
                {
                    backslash = xIndex == 1 ^ yIndex == 1;
                }


                bounds[0, 0] = Parent.bounds[xIndex, yIndex];
                bounds[1, 0] = Midpoint(Parent.bounds[xIndex, yIndex], Parent.bounds[xIndex + 1, yIndex]);
                bounds[2, 0] = Parent.bounds[xIndex + 1, yIndex];
                bounds[0, 1] = Midpoint(Parent.bounds[xIndex, yIndex], Parent.bounds[xIndex, yIndex + 1]);

                if (backslash)
                {
                    bounds[1, 1] = Midpoint(Parent.bounds[xIndex, yIndex], Parent.bounds[xIndex + 1, yIndex + 1]);
                }
                else
                {
                    bounds[1, 1] = Midpoint(Parent.bounds[xIndex + 1, yIndex], Parent.bounds[xIndex, yIndex + 1]);
                }

                bounds[2, 1] = Midpoint(Parent.bounds[xIndex + 1, yIndex], Parent.bounds[xIndex + 1, yIndex + 1]);
                bounds[0, 2] = Parent.bounds[xIndex, yIndex + 1];
                bounds[1, 2] = Midpoint(Parent.bounds[xIndex, yIndex + 1], Parent.bounds[xIndex + 1, yIndex + 1]);
                bounds[2, 2] = Parent.bounds[xIndex + 1, yIndex + 1];

                if (Properties.Settings.Default.ShowElevationModel)
                {
                    bounds[0, 0].Tu = 0;
                    bounds[0, 0].Tv = 0;
                    bounds[1, 0].Tu = .5f;
                    bounds[1, 0].Tv = 0;
                    bounds[2, 0].Tu = 1;
                    bounds[2, 0].Tv = 0;

                    bounds[0, 1].Tu = 0;
                    bounds[0, 1].Tv = .5f;
                    bounds[1, 1].Tu = .5f;
                    bounds[1, 1].Tv = .5f;
                    bounds[2, 1].Tu = 1;
                    bounds[2, 1].Tv = .5f;

                    bounds[0, 2].Tu = 0;
                    bounds[0, 2].Tv = 1;
                    bounds[1, 2].Tu = .5f;
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
            }

        }

        private PositionTexture Midpoint(PositionTexture positionNormalTextured, PositionTexture positionNormalTextured_2)
        {
            Vector3d a1 = Vector3d.Lerp(positionNormalTextured.Position, positionNormalTextured_2.Position, .5f);
            Vector2d a1uv = Vector2d.Lerp(new Vector2d(positionNormalTextured.Tu, positionNormalTextured.Tv), new Vector2d(positionNormalTextured_2.Tu, positionNormalTextured_2.Tv), .5f);

            a1.Normalize();
            return new PositionTexture(a1, a1uv.X, a1uv.Y);
        }

        public bool IsPointInTile(double lat, double lng)
        {
            if (Level == 0)
            {
                return true;
            }

            if (Level == 1)
            {
                if ((lng >= 0 && lng <= 90) && (X == 0 && Y == 1))
                {
                    return true;
                }
                if ((lng > 90 && lng <= 180) && (X == 1 && Y == 1))
                {
                    return true;
                }
                if ((lng < 0 && lng >= -90) && (X == 0 && Y == 0))
                {
                    return true;
                }
                if ((lng < -90 && lng >= -180) && (X == 1 && Y == 0))
                {
                    return true;
                }
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

        static ToastTools root = new ToastTools(null, 0, 0, 0);

        public static ToastTools GetTileForLevelPoint(int targetLevel, double lat, double lng)
        {

            int level = 0;

            ToastTools tile = root;


            while (level < targetLevel+1)
            {
                //iterate children
                for (int i = 0; i < 4; i++ )
                {
                    ToastTools child = tile.GetChild(i);
                    if (child != null)
                    {
                        if (child.IsPointInTile(lat, lng))
                        {
                            tile = child;
                            if (child.Level == targetLevel)
                            {
                                return tile;
                            }
                            break;
                        }
                    }
                }
                level++;
            }

            return null;
        }
    }
}
