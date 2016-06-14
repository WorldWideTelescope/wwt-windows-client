using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Solver;

namespace TerraViewer.Callibration
{
    class SolveAlignment
    {

        List<Parameter> regressionParameters = new List<Parameter>();

        // independent parameter
        Parameter ParmeterIndex = new Parameter(0);

        List<SolverFunction> SolveList = new List<SolverFunction>();
        public List<SolveProjector> projectors = new List<SolveProjector>();


        LevenbergMarquardt lm = null;

        public void SetupSolve(CalibrationInfo ci, bool useConstraints, bool radialDistortion)
        {

            foreach (ProjectorEntry pe in ci.Projectors)
            {
                SolveProjector sp = new SolveProjector(pe, ci.DomeSize, ci.ScreenType == ScreenTypes.FishEye ? ProjectionType.FishEye : ProjectionType.Projector, ScreenTypes.Spherical, ci.ScreenType == ScreenTypes.FishEye ? SolveParameters.FishEye : (SolveParameters)ci.SolveParameters);
                sp.RadialDistorion = radialDistortion;
                projectors.Add(sp);

                if (useConstraints)
                {
                    foreach (GroundTruthPoint gt in pe.Constraints)
                    {
                        SolveList.Add(new Constraint(sp, gt));
                    }
                }

              
            }

            foreach (Edge edge in ci.Edges)
            {
                foreach (EdgePoint pnt in edge.Points)
                {
                    SolveList.Add( new Coorespondence(projectors[edge.Left-1],projectors[edge.Right-1],pnt));
                }
            }


            foreach (SolveProjector sp in projectors)
            {
                regressionParameters.AddRange(sp.Parameters);
            }


            int count = SolveList.Count;
            double[,] data = new double[2, count];
            for (int i = 0; i < count; i++)
            {
                data[0, i] = i;
                data[1, i] = 0;
            }
            Parameter[] observed = new Parameter[] { ParmeterIndex };

            lm = new LevenbergMarquardt(new functionDelegate(SolveFunction), regressionParameters.ToArray(), observed, data);



        }

        public double Iterate()
        {
            lm.Iterate();
            return lm.AverageError;
        }

        public double SolveFunction()
        {
            SolverFunction func = SolveList[(int)ParmeterIndex];

            return func.Calculate();
        }
     


      
    }

    class SolverFunction
    {
        public virtual double Calculate()
        {
            return 0;
        }
    }

    class Coorespondence : SolverFunction
    {
        SolveProjector left;
        SolveProjector right;

        EdgePoint point;

        public Coorespondence(SolveProjector left, SolveProjector right, EdgePoint point)
        {
            this.left = left;
            this.right = right;
            this.point = point;
        }

        public override double Calculate()
        {
            Vector2d resultLeft = left.GetCoordinatesForScreenPoint(point.Left.X, point.Left.Y);
            Vector2d resultRight = right.GetCoordinatesForScreenPoint(point.Right.X, point.Right.Y);



            double dist =  resultLeft.Distance3d(resultRight)*100*point.Left.Weight;
            return dist;
        }
    }

    class Constraint : SolverFunction
    {
        SolveProjector projector;
        GroundTruthPoint point = new GroundTruthPoint();
        public Constraint(SolveProjector projector, GroundTruthPoint point)
        {
            this.projector = projector;
            this.point = point;
        }

        public override double Calculate()
        {
            Vector2d result = projector.GetCoordinatesForScreenPoint(point.X, point.Y);

            switch (point.AxisType)
            {
                case AxisTypes.Alt:
                    {
                        double dist = (result.Y - point.Alt) * 100 * point.Weight;
                        return dist;
                    } 
                case AxisTypes.Az:
                    {

                        double dist = (result.X - (-point.Az + 270)) * 100 * point.Weight;
                        return dist;
                    } 
                case AxisTypes.Both:
                    {
                        Vector2d test = new Vector2d(-point.Az + 270, point.Alt);
                        double dist = result.Distance3d(test) * 100 * point.Weight;
                        return dist;
                    } 

                case AxisTypes.Edge:
                default:
                    return 0;
            }


        }
    }
    enum ProjectionType { View, Projector, Solved, FishEye };
    public enum ScreenTypes { Spherical, Cylindrical, Flat, FishEye };
    public enum SolveParameters { Fov = 1, Aspect = 2, Pitch = 4, Heading = 8, Roll = 16, X = 32, Y = 64, Z = 128, Radial = 256, Offset = 512, Default = 255, NoAspect = 253, FishEye = 767 };

    class SolveProjector
    {
        Parameter Fov = new Parameter();
        Parameter Aspect = new Parameter();
        Parameter OffsetX = new Parameter();
        Parameter OffsetY = new Parameter();
        Parameter Pitch = new Parameter();
        Parameter Heading = new Parameter();
        Parameter Roll = new Parameter();
        Parameter X = new Parameter();
        Parameter Y = new Parameter();
        Parameter Z = new Parameter();

        Parameter RadialCenterX = new Parameter();
        Parameter RadialCenterY = new Parameter();
        Parameter RadialAmountX = new Parameter();
        Parameter RadialAmountY = new Parameter();

        bool useGrid = false;

        int width = 0;
        int height = 0;

        double sphereRadius = 1;

        ScreenTypes screenType = ScreenTypes.Cylindrical;
        SolveParameters solveParameters = SolveParameters.Default;
        ProjectionType projectionType = ProjectionType.View;
        public SolveProjector(ProjectorEntry pe, double radius, ProjectionType type, ScreenTypes tranformType, SolveParameters solveParameters)
        {
            this.projectionType = type;
            this.screenType = tranformType;
            this.solveParameters = solveParameters;

            switch (type)
            {
                case ProjectionType.View:
                    {

                        Fov.Value = pe.ViewProjection.FOV;
                        Aspect.Value = pe.ViewProjection.Aspect;
                        OffsetX.Value = pe.ViewProjection.XOffset;
                        OffsetY.Value = pe.ViewProjection.YOffset;
                        RadialCenterX.Value = pe.ViewProjection.RadialCenterX;
                        RadialCenterY.Value = pe.ViewProjection.RadialCenterY;
                        RadialAmountX.Value = pe.ViewProjection.RadialAmountX;
                        RadialAmountY.Value = pe.ViewProjection.RadialAmountY;

                        Pitch.Value = pe.ViewTransform.Pitch;
                        Heading.Value = pe.ViewTransform.Heading;
                        Roll.Value = pe.ViewTransform.Roll;
                        X.Value = -pe.ViewTransform.X;
                        Y.Value = pe.ViewTransform.Y;
                        Z.Value = pe.ViewTransform.Z;
                    }
                    break;
                case ProjectionType.FishEye:
                case ProjectionType.Projector:
                    {
                        Fov.Value = pe.ProjectorProjection.FOV;
                        Aspect.Value = pe.ProjectorProjection.Aspect;
                        OffsetX.Value = pe.ProjectorProjection.XOffset;
                        OffsetY.Value = pe.ProjectorProjection.YOffset;
                        RadialCenterX.Value = pe.ProjectorProjection.RadialCenterX;
                        RadialCenterY.Value = pe.ProjectorProjection.RadialCenterY;
                        RadialAmountX.Value = pe.ProjectorProjection.RadialAmountX;
                        RadialAmountY.Value = pe.ProjectorProjection.RadialAmountY;

                        Pitch.Value = pe.ProjectorTransform.Pitch;
                        Heading.Value = pe.ProjectorTransform.Heading;
                        Roll.Value = pe.ProjectorTransform.Roll;
                        X.Value = -pe.ProjectorTransform.X;
                        Y.Value = pe.ProjectorTransform.Y;
                        Z.Value = pe.ProjectorTransform.Z;
                    }
                    break;
                case ProjectionType.Solved:
                    {
                        Fov.Value = pe.SolvedProjection.FOV;
                        Aspect.Value = pe.SolvedProjection.Aspect;
                        OffsetX.Value = pe.SolvedProjection.XOffset;
                        OffsetY.Value = pe.SolvedProjection.YOffset;
                        RadialCenterX.Value = pe.ProjectorProjection.RadialCenterX;
                        RadialCenterY.Value = pe.ProjectorProjection.RadialCenterY;
                        RadialAmountX.Value = pe.ProjectorProjection.RadialAmountX;
                        RadialAmountY.Value = pe.ProjectorProjection.RadialAmountY;

                        Pitch.Value = pe.SolvedTransform.Pitch;
                        Heading.Value = pe.SolvedTransform.Heading;
                        Roll.Value = pe.SolvedTransform.Roll;
                        X.Value = -pe.SolvedTransform.X;
                        Y.Value = pe.SolvedTransform.Y;
                        Z.Value = pe.SolvedTransform.Z;
                    }
                    break;
                default:
                    break;
            }

            useGrid = pe.UseGrid;

            width = pe.Width;
            height = pe.Height;
            sphereRadius = radius;

            if (useGrid)
            {
                LoadGrid(pe.Constraints);
            }

        }

        GroundTruthPoint[,] grid = new GroundTruthPoint[40,24];

        private void LoadGrid(List<GroundTruthPoint> points)
        {
            foreach(var pnt in points)
            {
                Point address = GetGridAddressFromPoint(pnt.X, pnt.Y);
                grid[address.X, address.Y] = pnt;
            }

            for(int y = 12; y<24; y++)
            {
                for(int x = 20; x < 40; x++)
                {
                    if (grid[x,y] == null)
                    {
                        GroundTruthPoint gt = new GroundTruthPoint();
                        gt.X = x * 50 + 30.5 - 50;
                        gt.Y = y * 50 + 10.5 - 50;

                    }
                }
            }

        }

        private Point GetGridAddressFromPoint(double x, double y)
        {
            int size = 50;

            return new Point((int)((x - 30.5) / size) + 1, (int)((y - 10.5) / size) + 1);
        }

        public Projection Projection
        {
            get
            {
                Projection proj = new Projection();
                proj.FOV = Fov;
                proj.Aspect = Aspect;
                proj.XOffset = OffsetX;
                proj.YOffset = OffsetY;
                proj.RadialCenterX = RadialCenterX;
                proj.RadialCenterY = RadialCenterY;
                proj.RadialAmountX = RadialAmountX;
                proj.RadialAmountY = RadialAmountY;

                return proj;
            }

        }

        public Transform Transform
        {
            get
            {
                Transform trans = new Transform();
                trans.Heading = Heading;
                trans.Pitch = Pitch;
                trans.Roll = Roll;
                trans.X = -X;
                trans.Z = Z;
                trans.Y = Y;
                return trans;
            }
        }

        public Matrix3d ProjMatrix;
        public Matrix3d TranMatrix;
        public Matrix3d CamMatrix; 
        public double near = .1;
        public double far = 100;
        private double lastVals = 0;

        public bool RadialDistorion = false;

        public void MakeMatrixes()
        {

            if (Fov + Aspect + OffsetX + OffsetY + Pitch + Heading + Roll + X + Y + Z + RadialAmountX + RadialAmountY + RadialCenterX + RadialCenterY != lastVals)
            // if (true)
            {
                if (projectionType != ProjectionType.FishEye)
                {
                    ProjMatrix = MakeProjection(near, far, Fov, Aspect, OffsetX, OffsetY);
                }
                TranMatrix = MakeTransform(Pitch, Heading, Roll, X, Y, Z);
                if (projectionType != ProjectionType.FishEye)
                {
                    CamMatrix = TranMatrix * ProjMatrix;
                }
                lastVals = Fov + Aspect + OffsetX + OffsetY + Pitch + Heading + Roll + X + Y + Z + RadialAmountX + RadialAmountY + RadialCenterX + RadialCenterY;
            }

        }

        public Matrix3d GetCameraMatrix()
        {
            MakeMatrixes();
            return CamMatrix;
        }

        public void TransformPickPointToWorldSpace(Vector2d ptCursor, int backBufferWidth, int backBufferHeight, out Vector3d vPickRayOrig, out Vector3d vPickRayDir)
        {
            // Credit due to the DirectX 9 C++ Pick sample and MVP Robert Dunlop
            // Get the pick ray from the mouse position

            // Compute the vector of the pick ray in screen space
            Vector3d v;
            v.X = (((2.0f * ptCursor.X) / backBufferWidth) - 1) / ProjMatrix.M11;
            v.Y = -(((2.0f * ptCursor.Y) / backBufferHeight) - 1) / ProjMatrix.M22;
            v.Z = 1.0f;
            Matrix3d mInit = TranMatrix;

            Matrix3d m = Matrix3d.Invert(mInit);

            // Transform the screen space pick ray into 3D space
            vPickRayDir.X = v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31;
            vPickRayDir.Y = v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32;
            vPickRayDir.Z = v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33;


            vPickRayDir.Normalize();

            vPickRayOrig.X = m.M41;
            vPickRayOrig.Y = m.M42;
            vPickRayOrig.Z = m.M43;

            // Calculate the origin as intersection with near frustum

            vPickRayOrig.X += vPickRayDir.X * near;
            vPickRayOrig.Y += vPickRayDir.Y * near;
            vPickRayOrig.Z += vPickRayDir.Z * near;
        }


        public void TransformPickPointToWorldSpaceFishEye(Vector2d ptCursor, int backBufferWidth, int backBufferHeight, out Vector3d vPickRayOrig, out Vector3d vPickRayDir)
        {
            Vector2d point = new Vector2d((((ptCursor.X / backBufferWidth) - .5) - OffsetX) / (Fov/Aspect),
                                (((ptCursor.Y / backBufferHeight) - .5) - OffsetY) / Fov);

            Vector2d altAz = GetAltAzFromPoint(point);

            Vector3d pnt3d = Coordinates.GeoTo3dDouble(altAz.Y, altAz.X);

            Matrix3d matInv = TranMatrix;
            matInv.Invert();

            pnt3d = Vector3d.TransformCoordinate(pnt3d, TranMatrix);
            
            // Transform the screen space pick ray into 3D space
            vPickRayDir = pnt3d;
            vPickRayDir.Normalize();

            vPickRayOrig.X = X;
            vPickRayOrig.Y = Y;
            vPickRayOrig.Z = Z;
        }


        Vector2d GetAltAzFromPoint(Vector2d point)
        {
            double alt = 0;
            double az = 0;

            double x = point.X ;
            double y = point.Y ;
            double dist = Math.Sqrt(x * x + y * y);

            alt = 90 - ( dist / .5) * 90;
            az = ((Math.Atan2(y, x) / Math.PI * 180) + 630) % 360;
            return new Vector2d(az, alt);
        }

        Vector2d GetPointFromAltAz(Vector2d point)
        {
            Vector2d retPoint = new Vector2d();
            point.X += 90;
            retPoint.X =  Math.Cos(point.X / 180 * Math.PI) * ((90 - point.Y) / 90) * .5;
            retPoint.Y =  Math.Sin(point.X / 180 * Math.PI) * ((90 - point.Y) / 90) * .5;
            return retPoint;
        }
        

        //public Matrix3d MakeProjection(double near, double far, double fov, double aspect, double offsetX, double offsetY)
        //{
        //    fov = fov / 180 * Math.PI;

        //    double left = (-Math.Tan(fov / 2.0f * aspect) - offsetX) * near;
        //   // double right2 = near * 2 / (1 / Math.Tan(fov/2))*aspect / 2;
        //    double right = (Math.Tan(fov / 2.0f * aspect) - offsetX) * near;
        //    double top = (Math.Tan(fov / 2.0f) + offsetY) * near;
        //    double bottom = (-Math.Tan(fov / 2.0f) + offsetY) * near;

        //    return Matrix3d.PerspectiveOffCenterLH(left, right, bottom, top, near, far);
        //}
       // const double RC = Math.PI / 180;
        public Matrix3d MakeProjection(double near, double far, double  fov, double aspectRatio, double offaxisX, double offaxisY)
        {
            fov = fov / 180 * Math.PI;
            //double left =   (-Math.Tan(fov / 2.0f * aspectRatio) - offaxisX) * near;
            //double right =  ( Math.Tan(fov / 2.0f * aspectRatio) - offaxisX) * near;
            //double top =    ( Math.Tan(fov / 2.0f) + offaxisY) * near;
            //double bottom = (-Math.Tan(fov / 2.0f) + offaxisY) * near;

       

            //return Matrix3d.PerspectiveOffCenterLH(left, right, bottom, top, near, far);

            return Matrix3d.PerspectiveFovLH(fov, aspectRatio, near, far);
        }
        protected const double RC = (Math.PI / 180.0);

        public Matrix3d MakeTransform(double pitch, double heading, double roll, double x, double y, double z)
        {
            if (projectionType == ProjectionType.FishEye)
            {

                Matrix3d mat = Matrix3d.LookAtLH(new Vector3d(0, 0, 0), new Vector3d(0, -1, 0), new Vector3d(
                    -1, 0, 0))
                    * Matrix3d.RotationY(heading * RC)
                    * Matrix3d.RotationX(pitch * RC)
                    * Matrix3d.RotationZ(roll * RC)

                    ;


                return mat;
            }
            else
            {

                Matrix3d mat = Matrix3d.LookAtLH(new Vector3d(0, 0, 0), new Vector3d(0, 0, -1), new Vector3d(0, 1, 0))
                    * Matrix3d.Translation(new Vector3d(x, y, z))
                    * Matrix3d.RotationY(heading * RC)
                    * Matrix3d.RotationX(pitch * RC)
                    * Matrix3d.RotationZ(roll * RC)

                    ;

               return mat;
            }
        }

        public Vector2d ProjectPoint(Vector2d pnt)
        {
            if (projectionType == ProjectionType.FishEye)
            {

                MakeMatrixes();
                Vector3d pnt3d = Coordinates.GeoTo3dDouble(pnt.Y, pnt.X, sphereRadius);
                Vector3d pntOut = TranMatrix.Transform(pnt3d);
                Coordinates cord = Coordinates.CartesianToSpherical(pntOut);
                Vector2d pntOut2d = new Vector2d(cord.Lng, cord.Lat);

                return pntOut2d;
            }
            else
            {
                MakeMatrixes();
                Vector3d pnt3d = Coordinates.GeoTo3dDouble(pnt.Y, pnt.X, sphereRadius);
                Vector3d pntOut = CamMatrix.Transform(pnt3d);

                Vector2d pntOut2d = new Vector2d(pntOut.X, pntOut.Y);
                if (RadialDistorion)
                {
                    pntOut2d = ProjectRadialWarp(new Vector2d(pntOut.X, pntOut.Y));
                }

                return pntOut2d;
            }
        }

        public Vector2d GetCoordinatesForScreenPoint(double x, double y)
        {
            if (projectionType == ProjectionType.FishEye)
            {
                MakeMatrixes();
                Vector2d result = new Vector2d(0, 0);
                Vector3d PickRayOrig;
                Vector3d PickRayDir;
                Vector2d pt = new Vector2d(x, y);

                //if (RadialDistorion)
                //{
                //    pt = UnprojectRadialWarp(pt);
                //}

                TransformPickPointToWorldSpaceFishEye(pt, width, height, out PickRayOrig, out PickRayDir);


                SphereIntersectRay(PickRayOrig, PickRayDir, this.sphereRadius, out result);


                return result;
            }
            else
            {
                MakeMatrixes();
                Vector2d result = new Vector2d(0, 0);
                Vector3d PickRayOrig;
                Vector3d PickRayDir;
                Vector2d pt = new Vector2d(x, y);

                if (RadialDistorion)
                {
                    pt = UnprojectRadialWarp(pt);
                }

                TransformPickPointToWorldSpace(pt, width, height, out PickRayOrig, out PickRayDir);

                if (screenType == ScreenTypes.Cylindrical)
                {
                    CylinderIntersectRay(PickRayOrig, PickRayDir, this.sphereRadius, out result);
                }
                else
                {
                    SphereIntersectRay(PickRayOrig, PickRayDir, this.sphereRadius, out result);
                }

                return result;
            }
        }

        public Vector2d UnprojectRadialWarp(Vector2d point)
        {
            double x = (2 * (point.X) - (width + RadialCenterX * 2)) / width;
            double y = (2 * (point.Y) - (height + RadialCenterY * 2)) / height;
            double r = x * x + y * y;
            double x5 = x / (1 - RadialAmountX * r);
            double y5 = y / (1 - RadialAmountY * r);
            double x4 = x / (1 - RadialAmountX * (x5 * x5 + y5 * y5));
            double y4 = y / (1 - RadialAmountY * (x5 * x5 + y5 * y5));
            double x3 = x / (1 - RadialAmountX * (x4 * x4 + y4 * y4));
            double y3 = y / (1 - RadialAmountY * (x4 * x4 + y4 * y4));
            double x2 = x / (1 - RadialAmountX * (x3 * x3 + y3 * y3));
            double y2 = y / (1 - RadialAmountY * (x3 * x3 + y3 * y3));
            double i2 = (x2 + 1) * width / 2 + RadialCenterX;
            double j2 = (y2 + 1) * height / 2 + RadialCenterY;
            return new Vector2d(i2, j2);
        }

        public Vector2d ProjectRadialWarp(Vector2d point)
        {
            double x = (2 * (point.X) - (width + RadialCenterX * 2)) / width;
            double y = (2 * (point.Y) - (height + RadialCenterY * 2)) / height;
            double r = x * x + y * y;

            double x2 = x * (1 - RadialAmountX * r);
            double y2 = y * (1 - RadialAmountY * r);
            double i2 = (x2 + 1) * width / 2 + RadialCenterX;
            double j2 = (y2 + 1) * height / 2 + RadialCenterY;
            return new Vector2d(i2, j2);
        }

        public Vector2d GetCoordinatesForProjector()
        {
            MakeMatrixes();
            Vector2d result = new Vector2d(0, 0);
            Vector3d PickRayOrig;
            Vector3d PickRayDir;
            Vector2d pt = new Vector2d(960,540);
            TransformPickPointToWorldSpace(pt, width, height, out PickRayOrig, out PickRayDir);

            result = Vector2d.CartesianToSpherical2(PickRayOrig);

            return result;
        }

        // for interor points only
        public bool SphereIntersectRay(Vector3d pickRayOrig, Vector3d pickRayDir, double sphereRadius, out Vector2d pointCoordinate)
        {
            // bool SpherePrimitive::intersect(const Ray& ray, double* t)
            pointCoordinate = new Vector2d(0, 0);
            double r = sphereRadius;
            //Compute A, B and C coefficients
            double a = Vector3d.Dot(pickRayDir, pickRayDir);
            double b = 2 * Vector3d.Dot(pickRayDir, pickRayOrig);
            double c = Vector3d.Dot(pickRayOrig, pickRayOrig) - (r * r);

            //Find discriminant
            double disc = b * b - 4 * a * c;

            // if discriminant is negative there are no real roots, so return 
            // false as ray misses sphere
            if (disc < 0)
            {
                return false;
            }

            // compute q as described above
            double distSqrt = Math.Sqrt(disc);
            double q;
            if (b < 0)
            {
                q = (-b - distSqrt) / 2.0f;
            }
            else
            {
                q = (-b + distSqrt) / 2.0f;
            }

            // compute t0 and t1
            double t0 = q / a;
            double t1 = c / q;

            // make sure t0 is smaller than t1
            if (t0 > t1)
            {
                // if t0 is bigger than t1 swap them around
                double temp = t0;
                t0 = t1;
                t1 = temp;
            }

            // if t1 is less than zero, the object is in the ray's negative direction
            // and consequently the ray misses the sphere
            if (t1 < 0)
            {
                return false;
            }
            double t = 0;
            // if t0 is less than zero, the intersection point is at t1
            if (t0 < 0)
            {
                t = t1;
            }
            // else the intersection point is at t0
            else
            {
                t = t1;
            }

            Vector3d point = Vector3d.Scale(pickRayDir, t);

            point.Add(pickRayOrig);

            point.Normalize();

            pointCoordinate = Vector2d.CartesianToSpherical2(point);

         
            return true;
        }

        // for interor points only
        public bool CylinderIntersectRay(Vector3d pickRayOrig, Vector3d pickRayDir, double cylinderRadius, out Vector2d pointCoordinate)
        {
            pointCoordinate = new Vector2d(0, 0);
            double r = cylinderRadius;
            //Compute A, B and C coefficients
            double a = (pickRayDir.X * pickRayDir.X) + (pickRayDir.Z * pickRayDir.Z);
            double b = 2 * (pickRayOrig.X * pickRayDir.X) + 2 * (pickRayOrig.Z * pickRayDir.Z);
            double c = (pickRayOrig.X * pickRayOrig.X) + (pickRayOrig.Z * pickRayOrig.Z) - (cylinderRadius*cylinderRadius);

            //Find discriminant
            double disc = b * b - 4 * a * c;

            // if discriminant is negative there are no real roots, so return 
            // false as ray misses sphere
            if (disc < 0)
            {
                return false;
            }

            // compute q as described above
            double distSqrt = Math.Sqrt(disc);
            double q;
            if (b < 0)
            {
                q = (-b - distSqrt) / 2.0f;
            }
            else
            {
                q = (-b + distSqrt) / 2.0f;
            }

            // compute t0 and t1
            double t0 = q / a;
            double t1 = c / q;

            // make sure t0 is smaller than t1
            if (t0 > t1)
            {
                // if t0 is bigger than t1 swap them around
                double temp = t0;
                t0 = t1;
                t1 = temp;
            }

            // if t1 is less than zero, the object is in the ray's negative direction
            // and consequently the ray misses the sphere
            if (t1 < 0)
            {
                return false;
            }
            double t = 0;
            // if t0 is less than zero, the intersection point is at t1
            if (t0 < 0)
            {
                t = t1;
            }
            // else the intersection point is at t0
            else
            {
                t = t1;
            }

            Vector3d point = Vector3d.Scale(pickRayDir, t);

            point.Add(pickRayOrig);

            point.Normalize();

            pointCoordinate = Vector2d.CartesianToSpherical2(point);

            return true;
        }
        public Parameter[] Parameters
        {
            get
            {
                //Parameter[] paramList = null;
                //if (RadialDistorion)
                //{
                //    paramList = new Parameter[10];
                //}
                //else
                //{
                //    paramList = new Parameter[8];
                //}
                List<Parameter> paramList = new List<Parameter>();

                if ((solveParameters & SolveParameters.Fov) == SolveParameters.Fov)
                {
                    paramList.Add(Fov);
                }

                if ((solveParameters & SolveParameters.Aspect) == SolveParameters.Aspect)
                {
                    paramList.Add(Aspect);
                }

                if ((solveParameters & SolveParameters.Pitch) == SolveParameters.Pitch)
                {
                    paramList.Add(Pitch);
                }

                if ((solveParameters & SolveParameters.Heading) == SolveParameters.Heading)
                {
                    paramList.Add(Heading);
                }

                if ((solveParameters & SolveParameters.Roll) == SolveParameters.Roll)
                {
                    paramList.Add(Roll);
                }

                if ((solveParameters & SolveParameters.X) == SolveParameters.X)
                {
                    paramList.Add(X);
                }

                if ((solveParameters & SolveParameters.Y) == SolveParameters.Y)
                {
                    paramList.Add(Y);
                }

                if ((solveParameters & SolveParameters.Z) == SolveParameters.Z)
                {
                    paramList.Add(Z);
                }

                if ((solveParameters & SolveParameters.Offset) == SolveParameters.Offset)
                {
                    paramList.Add(OffsetX);
                    paramList.Add(OffsetY);
                }

                if (RadialDistorion)
                {
                    paramList.Add(RadialAmountX);
                    paramList.Add(RadialAmountY);
                }
                else
                {
                    RadialAmountX.Value = 0;
                    RadialAmountY.Value = 0;
                }

                //paramList[10] = RadialCenterX;
                //paramList[11] = RadialCenterY;
                //paramList[12] = OffsetX;
                // paramList[13] = OffsetY;
                return paramList.ToArray();
            }
        }

       
    }
}
