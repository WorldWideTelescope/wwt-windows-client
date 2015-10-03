using System;
using System.Collections.Generic;
using Solver;

namespace TerraViewer.Callibration
{
    class SolveAlignment
    {
        readonly List<Parameter> regressionParameters = new List<Parameter>();

        // independent parameter
        readonly Parameter ParmeterIndex = new Parameter(0);

        readonly List<SolverFunction> SolveList = new List<SolverFunction>();
        public List<SolveProjector> projectors = new List<SolveProjector>();


        LevenbergMarquardt lm;

        public void SetupSolve(CalibrationInfo ci, bool useConstraints, bool radialDistortion)
        {

            foreach (var pe in ci.Projectors)
            {
                var sp = new SolveProjector(pe, ci.DomeSize, ci.ScreenType == ScreenTypes.FishEye ? ProjectionType.FishEye : ProjectionType.Projector, ScreenTypes.Spherical, ci.ScreenType == ScreenTypes.FishEye ? SolveParameters.FishEye : (SolveParameters)ci.SolveParameters)
                {
                    RadialDistorion = radialDistortion
                };
                projectors.Add(sp);

                if (useConstraints)
                {
                    foreach (var gt in pe.Constraints)
                    {
                        SolveList.Add(new Constraint(sp, gt));
                    }
                }

              
            }

            foreach (var edge in ci.Edges)
            {
                foreach (var pnt in edge.Points)
                {
                    SolveList.Add( new Coorespondence(projectors[edge.Left-1],projectors[edge.Right-1],pnt));
                }
            }


            foreach (var sp in projectors)
            {
                regressionParameters.AddRange(sp.Parameters);
            }


            var count = SolveList.Count;
            var data = new double[2, count];
            for (var i = 0; i < count; i++)
            {
                data[0, i] = i;
                data[1, i] = 0;
            }
            var observed = new[] { ParmeterIndex };

            lm = new LevenbergMarquardt(SolveFunction, regressionParameters.ToArray(), observed, data);
        }

        public double Iterate()
        {
            lm.Iterate();
            return lm.AverageError;
        }

        public double SolveFunction()
        {
            var func = SolveList[(int)ParmeterIndex];

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
        readonly SolveProjector left;
        readonly SolveProjector right;

        readonly EdgePoint point;

        public Coorespondence(SolveProjector left, SolveProjector right, EdgePoint point)
        {
            this.left = left;
            this.right = right;
            this.point = point;
        }

        public override double Calculate()
        {
            var resultLeft = left.GetCoordinatesForScreenPoint(point.Left.X, point.Left.Y);
            var resultRight = right.GetCoordinatesForScreenPoint(point.Right.X, point.Right.Y);



            var dist =  resultLeft.Distance3d(resultRight)*100*point.Left.Weight;
            return dist;
        }
    }

    class Constraint : SolverFunction
    {
        readonly SolveProjector projector;
        readonly GroundTruthPoint point = new GroundTruthPoint();
        public Constraint(SolveProjector projector, GroundTruthPoint point)
        {
            this.projector = projector;
            this.point = point;
        }

        public override double Calculate()
        {
            var result = projector.GetCoordinatesForScreenPoint(point.X, point.Y);

            switch (point.AxisType)
            {
                case AxisTypes.Alt:
                    {
                        var dist = (result.Y - point.Alt) * 100 * point.Weight;
                        return dist;
                    } 
                case AxisTypes.Az:
                    {

                        var dist = (result.X - (-point.Az + 270)) * 100 * point.Weight;
                        return dist;
                    } 
                case AxisTypes.Both:
                    {
                        var test = new Vector2d(-point.Az + 270, point.Alt);
                        var dist = result.Distance3d(test) * 100 * point.Weight;
                        return dist;
                    }

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
        readonly Parameter Fov = new Parameter();
        readonly Parameter Aspect = new Parameter();
        readonly Parameter OffsetX = new Parameter();
        readonly Parameter OffsetY = new Parameter();
        readonly Parameter Pitch = new Parameter();
        readonly Parameter Heading = new Parameter();
        readonly Parameter Roll = new Parameter();
        readonly Parameter X = new Parameter();
        readonly Parameter Y = new Parameter();
        readonly Parameter Z = new Parameter();

        readonly Parameter RadialCenterX = new Parameter();
        readonly Parameter RadialCenterY = new Parameter();
        readonly Parameter RadialAmountX = new Parameter();
        readonly Parameter RadialAmountY = new Parameter();


        readonly int width;
        readonly int height;

        readonly double sphereRadius = 1;

        readonly ScreenTypes screenType = ScreenTypes.Cylindrical;
        readonly SolveParameters solveParameters = SolveParameters.Default;
        readonly ProjectionType projectionType = ProjectionType.View;
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
            }

           

            width = pe.Width;
            height = pe.Height;
            sphereRadius = radius;

        }

        public Projection Projection
        {
            get
            {
                var proj = new Projection
                {
                    FOV = Fov,
                    Aspect = Aspect,
                    XOffset = OffsetX,
                    YOffset = OffsetY,
                    RadialCenterX = RadialCenterX,
                    RadialCenterY = RadialCenterY,
                    RadialAmountX = RadialAmountX,
                    RadialAmountY = RadialAmountY
                };

                return proj;
            }

        }

        public Transform Transform
        {
            get
            {
                return new Transform {Heading = Heading, Pitch = Pitch, Roll = Roll, X = -X, Z = Z, Y = Y};
            }
        }

        public Matrix3d ProjMatrix;
        public Matrix3d TranMatrix;
        public Matrix3d CamMatrix; 
        public double near = .1;
        public double far = 100;
        private double lastVals;

        public bool RadialDistorion = false;

        public void MakeMatrixes()
        {

            if (Fov + Aspect + OffsetX + OffsetY + Pitch + Heading + Roll + X + Y + Z + RadialAmountX + RadialAmountY + RadialCenterX + RadialCenterY != lastVals)
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
            var mInit = TranMatrix;

            var m = Matrix3d.Invert(mInit);

            // Transform the screen space pick ray into 3D space
            vPickRayDir.X = v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31;
            vPickRayDir.Y = v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32;
            vPickRayDir.Z = v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33;


            vPickRayDir.Normalize();

            vPickRayOrig.X = m.M41;
            vPickRayOrig.Y = m.M42;
            vPickRayOrig.Z = m.M43;

            // Calculate the origin as intersection with nearValue frustum

            vPickRayOrig.X += vPickRayDir.X * near;
            vPickRayOrig.Y += vPickRayDir.Y * near;
            vPickRayOrig.Z += vPickRayDir.Z * near;
        }


        public void TransformPickPointToWorldSpaceFishEye(Vector2d ptCursor, int backBufferWidth, int backBufferHeight, out Vector3d vPickRayOrig, out Vector3d vPickRayDir)
        {
            var point = new Vector2d((((ptCursor.X / backBufferWidth) - .5) - OffsetX) / (Fov/Aspect),
                                (((ptCursor.Y / backBufferHeight) - .5) - OffsetY) / Fov);

            var altAz = GetAltAzFromPoint(point);

            var pnt3d = Coordinates.GeoTo3dDouble(altAz.Y, altAz.X);

            var matInv = TranMatrix;
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
            var x = point.X ;
            var y = point.Y ;
            var dist = Math.Sqrt(x * x + y * y);

            double alt = 90 - ( dist / .5) * 90;
            double az = ((Math.Atan2(y, x) / Math.PI * 180) + 630) % 360;
            return new Vector2d(az, alt);
        }

        public Matrix3d MakeProjection(double nearValue, double farValue, double  fov, double aspectRatio, double offaxisX, double offaxisY)
        {
            fov = fov / 180 * Math.PI;
            return Matrix3d.PerspectiveFovLH(fov, aspectRatio, nearValue, farValue);
        }
        protected const double RC = (Math.PI / 180.0);

        public Matrix3d MakeTransform(double pitch, double heading, double roll, double x, double y, double z)
        {
            if (projectionType == ProjectionType.FishEye)
            {

                var mat = Matrix3d.LookAtLH(new Vector3d(0, 0, 0), new Vector3d(0, -1, 0), new Vector3d(
                    -1, 0, 0))
                    * Matrix3d.RotationY(heading * RC)
                    * Matrix3d.RotationX(pitch * RC)
                    * Matrix3d.RotationZ(roll * RC)

                    ;


                return mat;
            }
            else
            {

                var mat = Matrix3d.LookAtLH(new Vector3d(0, 0, 0), new Vector3d(0, 0, -1), new Vector3d(0, 1, 0))
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
                var pnt3d = Coordinates.GeoTo3dDouble(pnt.Y, pnt.X, sphereRadius);
                var pntOut = TranMatrix.Transform(pnt3d);
                var cord = Coordinates.CartesianToSpherical(pntOut);
                var pntOut2d = new Vector2d(cord.Lng, cord.Lat);

                return pntOut2d;
            }
            else
            {
                MakeMatrixes();
                var pnt3d = Coordinates.GeoTo3dDouble(pnt.Y, pnt.X, sphereRadius);
                var pntOut = CamMatrix.Transform(pnt3d);

                var pntOut2d = new Vector2d(pntOut.X, pntOut.Y);
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
                Vector2d result;
                Vector3d PickRayOrig;
                Vector3d PickRayDir;
                var pt = new Vector2d(x, y);

                TransformPickPointToWorldSpaceFishEye(pt, width, height, out PickRayOrig, out PickRayDir);


                SphereIntersectRay(PickRayOrig, PickRayDir, this.sphereRadius, out result);


                return result;
            }
            else
            {
                MakeMatrixes();
                Vector2d result;
                Vector3d PickRayOrig;
                Vector3d PickRayDir;
                var pt = new Vector2d(x, y);

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
            var x = (2 * (point.X) - (width + RadialCenterX * 2)) / width;
            var y = (2 * (point.Y) - (height + RadialCenterY * 2)) / height;
            var r = x * x + y * y;
            var x5 = x / (1 - RadialAmountX * r);
            var y5 = y / (1 - RadialAmountY * r);
            var x4 = x / (1 - RadialAmountX * (x5 * x5 + y5 * y5));
            var y4 = y / (1 - RadialAmountY * (x5 * x5 + y5 * y5));
            var x3 = x / (1 - RadialAmountX * (x4 * x4 + y4 * y4));
            var y3 = y / (1 - RadialAmountY * (x4 * x4 + y4 * y4));
            var x2 = x / (1 - RadialAmountX * (x3 * x3 + y3 * y3));
            var y2 = y / (1 - RadialAmountY * (x3 * x3 + y3 * y3));
            var i2 = (x2 + 1) * width / 2 + RadialCenterX;
            var j2 = (y2 + 1) * height / 2 + RadialCenterY;
            return new Vector2d(i2, j2);
        }

        public Vector2d ProjectRadialWarp(Vector2d point)
        {
            var x = (2 * (point.X) - (width + RadialCenterX * 2)) / width;
            var y = (2 * (point.Y) - (height + RadialCenterY * 2)) / height;
            var r = x * x + y * y;

            var x2 = x * (1 - RadialAmountX * r);
            var y2 = y * (1 - RadialAmountY * r);
            var i2 = (x2 + 1) * width / 2 + RadialCenterX;
            var j2 = (y2 + 1) * height / 2 + RadialCenterY;
            return new Vector2d(i2, j2);
        }

        public Vector2d GetCoordinatesForProjector()
        {
            MakeMatrixes();
            Vector3d PickRayOrig;
            Vector3d PickRayDir;
            var pt = new Vector2d(960,540);
            TransformPickPointToWorldSpace(pt, width, height, out PickRayOrig, out PickRayDir);

            Vector2d result = Vector2d.CartesianToSpherical2(PickRayOrig);

            return result;
        }

        // for interor points only
        public bool SphereIntersectRay(Vector3d pickRayOrig, Vector3d pickRayDir, double sphereRadiusValue, out Vector2d pointCoordinate)
        {
            // bool SpherePrimitive::intersect(const Ray& ray, double* t)
            pointCoordinate = new Vector2d(0, 0);
            var r = sphereRadiusValue;
            //Compute A, B and C coefficients
            var a = Vector3d.Dot(pickRayDir, pickRayDir);
            var b = 2 * Vector3d.Dot(pickRayDir, pickRayOrig);
            var c = Vector3d.Dot(pickRayOrig, pickRayOrig) - (r * r);

            //Find discriminant
            var disc = b * b - 4 * a * c;

            // if discriminant is negative there are no real roots, so return 
            // false as ray misses sphere
            if (disc < 0)
            {
                return false;
            }

            // compute q as described above
            var distSqrt = Math.Sqrt(disc);
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
            var t0 = q / a;
            var t1 = c / q;

            // make sure t0 is smaller than t1
            if (t0 > t1)
            {
                // if t0 is bigger than t1 swap them around
                var temp = t0;
                t0 = t1;
                t1 = temp;
            }

            // if t1 is less than zero, the object is in the ray's negative direction
            // and consequently the ray misses the sphere
            if (t1 < 0)
            {
                return false;
            }
            double t;
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

            var point = Vector3d.Scale(pickRayDir, t);

            point.Add(pickRayOrig);

            point.Normalize();

            pointCoordinate = Vector2d.CartesianToSpherical2(point);

         
            return true;
        }

        // for interor points only
        public bool CylinderIntersectRay(Vector3d pickRayOrig, Vector3d pickRayDir, double cylinderRadius, out Vector2d pointCoordinate)
        {
            pointCoordinate = new Vector2d(0, 0);
            //Compute A, B and C coefficients
            var a = (pickRayDir.X * pickRayDir.X) + (pickRayDir.Z * pickRayDir.Z);
            var b = 2 * (pickRayOrig.X * pickRayDir.X) + 2 * (pickRayOrig.Z * pickRayDir.Z);
            var c = (pickRayOrig.X * pickRayOrig.X) + (pickRayOrig.Z * pickRayOrig.Z) - (cylinderRadius*cylinderRadius);

            //Find discriminant
            var disc = b * b - 4 * a * c;

            // if discriminant is negative there are no real roots, so return 
            // false as ray misses sphere
            if (disc < 0)
            {
                return false;
            }

            // compute q as described above
            var distSqrt = Math.Sqrt(disc);
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
            var t0 = q / a;
            var t1 = c / q;

            // make sure t0 is smaller than t1
            if (t0 > t1)
            {
                // if t0 is bigger than t1 swap them around
                var temp = t0;
                t0 = t1;
                t1 = temp;
            }

            // if t1 is less than zero, the object is in the ray's negative direction
            // and consequently the ray misses the sphere
            if (t1 < 0)
            {
                return false;
            }
            double t;
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

            var point = Vector3d.Scale(pickRayDir, t);

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
                var paramList = new List<Parameter>();

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

                return paramList.ToArray();
            }
        }

       
    }
}
