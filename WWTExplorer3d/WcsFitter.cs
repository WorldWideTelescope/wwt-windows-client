using System;
using System.Collections.Generic;
using System.Text;
using Solver;
using TerraViewer.Callibration;

namespace TerraViewer
{
    class WcsFitter
    {

        double width;
        double height;

        List<CorresponencePoint> Points = new List<CorresponencePoint>();

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
            WcsSolution s = new WcsSolution();
            Vector2d center = new Vector2d(width / 2, height / 2);
            Vector2d temp = a.Image - b.Image;
            double imageLength = temp.Length;
            double angularSperation = CAAAngularSeparation.Separation(a.Celestial.RA, a.Celestial.Dec, b.Celestial.RA, b.Celestial.Dec);

            // Degrees per pixel
            s.Scale = angularSperation / imageLength;
            double imageRotation = Math.Atan2(temp.X, temp.Y) / Math.PI * 180;

            temp = center - b.Image;

            double centerRotation = Math.Atan2(temp.X, temp.Y) / Math.PI * 180;

            s.OffsetX = width / 2;
            s.OfsetY = height / 2;

            Coordinates cent = a.Celestial;

            int iters = 4;

            while (iters-- > 0)
            {

                // Calculate Center
                Vector2d tanA = Coordinates.RaDecToTan(cent, a.Celestial);
                Vector2d tanB = Coordinates.RaDecToTan(cent, b.Celestial);

                temp = tanA - tanB;
                double tanLength = temp.Length;


                s.Scale = (tanLength/Math.PI*180) / imageLength;

                double tanRotation = Math.Atan2(temp.X, temp.Y) / Math.PI * 180;

                double tRotRad = -((imageRotation - tanRotation) / 180 * Math.PI);

                Vector2d centerDistA = center - a.Image;
                double centerRotaionA = Math.Atan2(centerDistA.X, centerDistA.Y);

                double ratio = tanLength / imageLength;

                double tanCx = tanA.X + Math.Sin(centerRotaionA + tRotRad) * ratio * centerDistA.Length;
                double tanCy = tanA.Y + Math.Cos(centerRotaionA + tRotRad) * ratio * centerDistA.Length;

                Vector2d result = Coordinates.TanToRaDec(cent, new Vector2d(tanCx, tanCy));
                s.CenterX = result.X;
                s.CenterY = result.Y;

                cent = Coordinates.FromRaDec(result.X, result.Y);
            }

            double positionAngle = CAAAngularSeparation.PositionAngle(s.CenterX, s.CenterY, b.Celestial.RA, b.Celestial.Dec);
            s.Rotation = -((centerRotation - positionAngle));



            s.Flip = false;

            return s;
        }

        List<Parameter> regressionParameters = new List<Parameter>();

        // independent parameter
        Parameter ParmeterIndex = new Parameter(0);

        List<SolverFunction> SolveList = new List<SolverFunction>();

        LevenbergMarquardt lm = null;

        private WcsSolution SolveLM()
        {
            
            Vector2d temp = Points[0].Image - Points[1].Image;
            double imageLength = temp.Length;
            double angularSperation = CAAAngularSeparation.Separation(Points[0].Celestial.RA, Points[0].Celestial.Dec, Points[1].Celestial.RA, Points[1].Celestial.Dec);

            // Degrees per pixel
            double scale = angularSperation / imageLength;

            WcsSolution sinit = Solve(Points[0], Points[1]);


            TanSolver ts = new TanSolver(sinit.CenterX, sinit.CenterY, sinit.Rotation-180, sinit.Scale);

            foreach (CorresponencePoint cp in Points)
            {
                SolveList.Add(new CoorespondenceSolver(ts, cp, width, height));
            }


            regressionParameters.AddRange(ts.Parameters);

            int count = SolveList.Count;
            double[,] data = new double[2, count];
            for (int i = 0; i < count; i++)
            {
                data[0, i] = i;
                data[1, i] = 0;
            }
            Parameter[] observed = new Parameter[] { ParmeterIndex };

            lm = new LevenbergMarquardt(new functionDelegate(SolveFunction), regressionParameters.ToArray(), observed, data);

            for (int d = 0; d < 50; d++)
            {
                lm.Iterate();
            }

            WcsSolution s = ts.GetSolution();
            s.OffsetX = width / 2;
            s.OfsetY = height / 2;
           

            return s;
        }

        public double SolveFunction()
        {
            SolverFunction func = SolveList[(int)ParmeterIndex];

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
                WcsSolution s = new WcsSolution();

                s.CenterX = RA;
                s.CenterY = Dec;
                s.Scale = Scale;
                s.Rotation = Rotation;


                return s;
            }
            protected const double RC = (Math.PI / 180.0);

            public Coordinates Project(Vector2d point, double width, double height)
            {
             
                double lat = point.Y;
                double lng = point.X;
                lng = -lng;
                Matrix3d matrix = Matrix3d.Identity;
                matrix.Multiply(Matrix3d.RotationX((((Rotation-180)) / 180f * Math.PI)));
                matrix.Multiply(Matrix3d.RotationZ(((Dec) / 180f * Math.PI)));
                matrix.Multiply(Matrix3d.RotationY((((360 - RA*15) + 180) / 180f * Math.PI)));

                //lng = -lng;
                double fac1 = (Scale*height) / 2;
                double factor = Math.Tan(fac1 * RC);

                Vector3d retPoint = Vector3d.TransformCoordinate(new Vector3d(1f, (lat / fac1 * factor), (lng / fac1 * factor)), matrix);
                retPoint.Normalize();
                return Coordinates.CartesianToSpherical2(retPoint);
            }


            public Parameter[] Parameters
            {
                get
                {
                    List<Parameter> paramList = new List<Parameter>();

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
            TanSolver ts;
            double width = 0;
            double height = 0;

            public CoorespondenceSolver(TanSolver tanSolver, CorresponencePoint point, double width, double height)
            {
                this.width = width;
                this.height = height;
                this.point = point;
                ts = tanSolver;
            }

            public override double Calculate()
            {
                Coordinates pnt = ts.Project(new Vector2d(-(point.Image.X - width / 2) * ts.Scale, (point.Image.Y - height / 2) * ts.Scale), width, height);

                Vector3d vect1 = Coordinates.RADecTo3dDouble(point.Celestial,1);
                Vector3d Vect2 = Coordinates.RADecTo3dDouble(pnt,1);

                Vector3d vect3 = vect1 - Vect2;
                return vect3.Length();
            }
        }

        private WcsSolution SolveOld(CorresponencePoint a, CorresponencePoint b)
        {
            WcsSolution s = new WcsSolution();
            Vector2d center = new Vector2d(width / 2, height / 2);
            Vector2d temp = a.Image - b.Image;
            double imageLength = temp.Length;
            double angularSperation = CAAAngularSeparation.Separation(a.Celestial.RA, a.Celestial.Dec, b.Celestial.RA, b.Celestial.Dec);

            // Degrees per pixel
            s.Scale = angularSperation / imageLength;
            double imageRotation = Math.Atan2(temp.X, temp.Y) / Math.PI * 180;
            double positionAngle = CAAAngularSeparation.PositionAngle(a.Celestial.RA, a.Celestial.Dec, b.Celestial.RA, b.Celestial.Dec);
            //          Earth3d.MainWindow.Text = "pa:" + positionAngle.ToString() + ", imrot:" + imageRotation.ToString() + ", scale:" + s.Scale.ToString();
            s.Rotation = -((imageRotation - positionAngle));
            double rotationRads = s.Rotation / 180 * Math.PI;
            s.OffsetX = width / 2;
            s.OfsetY = height / 2;

            // Calculate center point
            Vector2d centerDistA = center - a.Image;
            Vector2d centerDistB = center - b.Image;
            double centerRotaionA = Math.Atan2(centerDistA.X, centerDistA.Y);
            double centerRotaionB = Math.Atan2(centerDistB.X, centerDistB.Y);
            double raA = a.Celestial.RA + (Math.Sin(centerRotaionA + rotationRads) * s.Scale / 15 * centerDistA.Length / Math.Cos(a.Celestial.Dec / 180 * Math.PI));
            double raB = b.Celestial.RA + (Math.Sin(centerRotaionB + rotationRads) * s.Scale / 15 * centerDistB.Length / Math.Cos(b.Celestial.Dec / 180 * Math.PI));
            double decA = a.Celestial.Dec + (Math.Cos(centerRotaionA + rotationRads) * s.Scale * centerDistA.Length);
            double decB = b.Celestial.Dec + (Math.Cos(centerRotaionB + rotationRads) * s.Scale * centerDistB.Length);

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
