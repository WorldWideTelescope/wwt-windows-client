using System;
using System.Collections.Generic;
using Solver;
using TerraViewer.Callibration;

namespace TerraViewer
{
    class WcsFitter
    {
        readonly double width;
        readonly double height;

        readonly List<CorresponencePoint> Points = new List<CorresponencePoint>();

        public WcsFitter(double width, double height)
        {
            this.width = width;
            this.height = height;
        }

        public void AddPoint(Coordinates sky, Vector2d image)
        {
            Points.Add(new CorresponencePoint(sky, image));
        }
        public WcsSolution Solution;
        public void Solve()
        {
            //Solution = Solve(Points[0],Points[1]);
            Solution = SolveLM();
        }

        private WcsSolution Solve(CorresponencePoint a, CorresponencePoint b)
        {
            var s = new WcsSolution();
            var center = new Vector2d(width / 2, height / 2);
            var temp = a.Image - b.Image;
            var imageLength = temp.Length;
            var angularSperation = CAAAngularSeparation.Separation(a.Celestial.RA, a.Celestial.Dec, b.Celestial.RA, b.Celestial.Dec);

            // Degrees per pixel
            s.Scale = angularSperation / imageLength;
            var imageRotation = Math.Atan2(temp.X, temp.Y) / Math.PI * 180;

            temp = center - b.Image;

            var centerRotation = Math.Atan2(temp.X, temp.Y) / Math.PI * 180;

            s.OffsetX = width / 2;
            s.OfsetY = height / 2;

            var cent = a.Celestial;

            var iters = 4;

            while (iters-- > 0)
            {

                // Calculate Center
                var tanA = Coordinates.RaDecToTan(cent, a.Celestial);
                var tanB = Coordinates.RaDecToTan(cent, b.Celestial);

                temp = tanA - tanB;
                var tanLength = temp.Length;


                s.Scale = (tanLength/Math.PI*180) / imageLength;

                var tanRotation = Math.Atan2(temp.X, temp.Y) / Math.PI * 180;

                var tRotRad = -((imageRotation - tanRotation) / 180 * Math.PI);

                var centerDistA = center - a.Image;
                var centerRotaionA = Math.Atan2(centerDistA.X, centerDistA.Y);

                var ratio = tanLength / imageLength;

                var tanCx = tanA.X + Math.Sin(centerRotaionA + tRotRad) * ratio * centerDistA.Length;
                var tanCy = tanA.Y + Math.Cos(centerRotaionA + tRotRad) * ratio * centerDistA.Length;

                var result = Coordinates.TanToRaDec(cent, new Vector2d(tanCx, tanCy));
                s.CenterX = result.X;
                s.CenterY = result.Y;

                cent = Coordinates.FromRaDec(result.X, result.Y);
            }

            var positionAngle = CAAAngularSeparation.PositionAngle(s.CenterX, s.CenterY, b.Celestial.RA, b.Celestial.Dec);
            s.Rotation = -((centerRotation - positionAngle));



            s.Flip = false;

            return s;
        }

        readonly List<Parameter> regressionParameters = new List<Parameter>();

        // independent parameter
        readonly Parameter ParmeterIndex = new Parameter(0);

        readonly List<SolverFunction> SolveList = new List<SolverFunction>();

        LevenbergMarquardt lm;

        private WcsSolution SolveLM()
        {
            
            var temp = Points[0].Image - Points[1].Image;
            var imageLength = temp.Length;
            var angularSperation = CAAAngularSeparation.Separation(Points[0].Celestial.RA, Points[0].Celestial.Dec, Points[1].Celestial.RA, Points[1].Celestial.Dec);

            // Degrees per pixel
            var scale = angularSperation / imageLength;

            var sinit = Solve(Points[0], Points[1]);


            var ts = new TanSolver(sinit.CenterX, sinit.CenterY, sinit.Rotation-180, sinit.Scale);

            foreach (var cp in Points)
            {
                SolveList.Add(new CoorespondenceSolver(ts, cp, width, height));
            }


            regressionParameters.AddRange(ts.Parameters);

            var count = SolveList.Count;
            var data = new double[2, count];
            for (var i = 0; i < count; i++)
            {
                data[0, i] = i;
                data[1, i] = 0;
            }
            var observed = new[] { ParmeterIndex };

            lm = new LevenbergMarquardt(SolveFunction, regressionParameters.ToArray(), observed, data);

            for (var d = 0; d < 50; d++)
            {
                lm.Iterate();
            }

            var s = ts.GetSolution();
            s.OffsetX = width / 2;
            s.OfsetY = height / 2;
           

            return s;
        }

        public double SolveFunction()
        {
            var func = SolveList[(int)ParmeterIndex];

            return func.Calculate();
        }

        public class TanSolver
        {
            public Parameter RA = new Parameter();
            public Parameter Dec = new Parameter();
            public Parameter Rotation = new Parameter();
            public Parameter Scale = new Parameter();

            public TanSolver(double ra, double dec, double rotation, double scale)
            {
                RA.Value = ra;
                Dec.Value = dec;
                Rotation.Value = rotation;
                Scale.Value = scale;
            }

            public WcsSolution GetSolution()
            {
                var s = new WcsSolution();

                s.CenterX = RA;
                s.CenterY = Dec;
                s.Scale = Scale;
                s.Rotation = Rotation;


                return s;
            }
            protected const double RC = (Math.PI / 180.0);

            public Coordinates Project(Vector2d point, double width, double height)
            {
             
                var lat = point.Y;
                var lng = point.X;
                lng = -lng;
                var matrix = Matrix3d.Identity;
                matrix.Multiply(Matrix3d.RotationX((((Rotation-180)) / 180f * Math.PI)));
                matrix.Multiply(Matrix3d.RotationZ(((Dec) / 180f * Math.PI)));
                matrix.Multiply(Matrix3d.RotationY((((360 - RA*15) + 180) / 180f * Math.PI)));

                //lng = -lng;
                var fac1 = (Scale*height) / 2;
                var factor = Math.Tan(fac1 * RC);

                var retPoint = Vector3d.TransformCoordinate(new Vector3d(1f, (lat / fac1 * factor), (lng / fac1 * factor)), matrix);
                retPoint.Normalize();
                return Coordinates.CartesianToSpherical2(retPoint);
            }


            public Parameter[] Parameters
            {
                get
                {
                    var paramList = new List<Parameter>();

                    paramList.Add(RA);
                    paramList.Add(Dec);
                    paramList.Add(Rotation);
                    paramList.Add(Scale);

                    return paramList.ToArray();
                }
            }
        }

        class CoorespondenceSolver : SolverFunction
        {
            CorresponencePoint point;
            readonly TanSolver ts;
            readonly double width;
            readonly double height;

            public CoorespondenceSolver(TanSolver tanSolver, CorresponencePoint point, double width, double height)
            {
                this.width = width;
                this.height = height;
                this.point = point;
                ts = tanSolver;
            }

            public override double Calculate()
            {
                var pnt = ts.Project(new Vector2d(-(point.Image.X - width / 2) * ts.Scale, (point.Image.Y - height / 2) * ts.Scale), width, height);

                var vect1 = Coordinates.RADecTo3dDouble(point.Celestial,1);
                var Vect2 = Coordinates.RADecTo3dDouble(pnt,1);

                var vect3 = vect1 - Vect2;
                return vect3.Length();
            }
        }

        private WcsSolution SolveOld(CorresponencePoint a, CorresponencePoint b)
        {
            var s = new WcsSolution();
            var center = new Vector2d(width / 2, height / 2);
            var temp = a.Image - b.Image;
            var imageLength = temp.Length;
            var angularSperation = CAAAngularSeparation.Separation(a.Celestial.RA, a.Celestial.Dec, b.Celestial.RA, b.Celestial.Dec);

            // Degrees per pixel
            s.Scale = angularSperation / imageLength;
            var imageRotation = Math.Atan2(temp.X, temp.Y) / Math.PI * 180;
            var positionAngle = CAAAngularSeparation.PositionAngle(a.Celestial.RA, a.Celestial.Dec, b.Celestial.RA, b.Celestial.Dec);
            //          Earth3d.MainWindow.Text = "pa:" + positionAngle.ToString() + ", imrot:" + imageRotation.ToString() + ", scale:" + s.Scale.ToString();
            s.Rotation = -((imageRotation - positionAngle));
            var rotationRads = s.Rotation / 180 * Math.PI;
            s.OffsetX = width / 2;
            s.OfsetY = height / 2;

            // Calculate center point
            var centerDistA = center - a.Image;
            var centerDistB = center - b.Image;
            var centerRotaionA = Math.Atan2(centerDistA.X, centerDistA.Y);
            var centerRotaionB = Math.Atan2(centerDistB.X, centerDistB.Y);
            var raA = a.Celestial.RA + (Math.Sin(centerRotaionA + rotationRads) * s.Scale / 15 * centerDistA.Length / Math.Cos(a.Celestial.Dec / 180 * Math.PI));
            var raB = b.Celestial.RA + (Math.Sin(centerRotaionB + rotationRads) * s.Scale / 15 * centerDistB.Length / Math.Cos(b.Celestial.Dec / 180 * Math.PI));
            var decA = a.Celestial.Dec + (Math.Cos(centerRotaionA + rotationRads) * s.Scale * centerDistA.Length);
            var decB = b.Celestial.Dec + (Math.Cos(centerRotaionB + rotationRads) * s.Scale * centerDistB.Length);

            s.CenterX = (raA + raB) / 2;
            s.CenterY = (decA + decB) / 2;

            s.Flip = false;

            return s;
        }

    }

    public class WcsSolution
    {
        public double CenterX;
        public double CenterY;
        public double OffsetX;
        public double OfsetY;
        public double Rotation;
        public double Scale;
        public bool Flip;
        public double Width;
        public double Height;
        public WcsSolution()
        {
            CenterX = 0;
            CenterY = 0;
            OffsetX = 0;
            OfsetY = 0;
            Rotation = 0;
            Scale = 0;
            Flip = false;
            Width = 0;
            Height = 0;
        }
    }

    public struct CorresponencePoint
    {
        public Coordinates Celestial;
        public Vector2d Image;
        public CorresponencePoint(Coordinates celetial, Vector2d image)
        {
            Celestial = celetial;
            Image = image;
        }
    }
}
