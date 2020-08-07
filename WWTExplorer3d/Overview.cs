using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Vector3 = SharpDX.Vector3;
using Matrix = SharpDX.Matrix;

namespace TerraViewer
{
    public partial class Overview : UserControl
    {
        public Overview()
        {
            InitializeComponent();
        }
        static Pen lightBluePen = new Pen(Color.LightBlue, 2);
        private void Overview_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawConstellation(g);
        }

        private void DrawConstellation(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.DrawRectangle(Pens.White, new Rectangle(0, 0, Width - 1, Height - 1));
            if (lines != null)
            {
                g.DrawLines(Pens.Yellow, lines);
            }
            if (fov != null)
            {
                g.DrawLines(lightBluePen, fov);
            }
        }
        private void DrawConstellationThumbnail(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (lines != null)
            {
                g.DrawLines(Pens.Yellow, lines);
            }
            if (fov != null)
            {
                g.DrawLines(lightBluePen, fov);
            }
        }
        string currentConstellation = "";

        public string Constellation
        {
            get { return currentConstellation; }
            set
            {
                if (value == null  || value == "Error")
                {
                    return;
                }
                string newValue = Constellations.Abbreviation(value);
                if (newValue != currentConstellation)
                {
                    currentConstellation = newValue;
                    ComputeDisplayParameters();
                    RefreshHint();
                }
            }
        }

        public void RefreshHint()
        {
            this.Invalidate();
        }

        Coordinates[] corners;

        public void SetViewRect(Coordinates[] corners)
        {
            if (corners.Length != 4)
            {
                return;
            }

            if (string.IsNullOrEmpty(currentConstellation))
            {
                return;
            }

            bool change = false;
            if (this.corners != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (this.corners[i] != corners[i])
                    {
                        change = true;
                        break;
                    }
                }
            }
            else
            {
                change = true;
            }

            if (!change)
            {
                return;
            }

            this.corners = corners;

            IPlace target = Constellations.ConstellationCentroids[currentConstellation];

            Vector3[] points = new Vector3[4];
            Vector3[] pointsOut = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {

                points[i] = Coordinates.GeoTo3d(this.corners[i].Dec, ((this.corners[i].RA / 24.0 * 360) - 180));

            }


            Matrix mat = Matrix.RotationY((float)(-(24 - (target.RA + 6)) / 12 * Math.PI));
            mat = Matrix.Multiply(mat, Matrix.RotationX((float)(target.Lat / 180f * Math.PI)));
            mat = Matrix.Multiply(mat, Matrix.Scaling(60, -60, 60));
            mat = Matrix.Multiply(mat, Matrix.Translation(58, 33, 0));

            Vector3.TransformCoordinate(points, ref mat, pointsOut);
            fov = new PointF[5];

            int index = 0;
            foreach (Vector3 point in pointsOut)
            {
                fov[index++] = new PointF((float)point.X, (float)point.Y);
            }
            fov[4] = fov[0];

            //Calculate view length
            double a = fov[0].X - fov[1].X;
            double b = fov[0].Y - fov[1].Y;
            double c = Math.Sqrt(a * a + b * b);

            RefreshHint();
        }

        public void SetViewRect( Coordinates topLeft, Coordinates topRight, Coordinates bottomLeft, Coordinates bottomRight)
        {
            if (    TopLeft != topLeft ||
                    TopRight != topRight ||
                    BottomLeft != bottomLeft ||
                    BottomRight != bottomRight )
            {
                TopLeft = topLeft;
                TopRight = topRight;
                BottomLeft = bottomLeft;
                BottomRight = bottomRight;

                RefreshHint();
            }
        }

        public Coordinates TopLeft;
        public Coordinates TopRight;
        public Coordinates BottomLeft;
        public Coordinates BottomRight;


        PointF[] lines = null;
        PointF[] fov = null;

        private void ComputeDisplayParameters()
        {
            if (currentConstellation == "Error")
            {
                return;
            }
            IPlace target = Constellations.ConstellationCentroids[currentConstellation];
            Lineset boundries = Constellations.boundries[currentConstellation];

            Vector3[] points = new Vector3[boundries.Points.Count];
            Vector3[] pointsOut = new Vector3[boundries.Points.Count];
            for (int i = 0; i< points.Length; i++)
            {
                points[i] = Coordinates.GeoTo3d(boundries.Points[i].Dec, boundries.Points[i].RA);
            }

            Matrix mat = Matrix.RotationY((float)(-(24-(target.RA+6)) /12 * Math.PI));
            mat = Matrix.Multiply(mat, Matrix.RotationX((float)(target.Lat / 180f * Math.PI)));
            mat = Matrix.Multiply(mat, Matrix.Scaling(60, -60, 60));
            mat = Matrix.Multiply(mat, Matrix.Translation(58, 33, 0));


            Vector3.TransformCoordinate(points, ref mat, pointsOut);
            lines = new PointF[points.Length+1];
            
            int index = 0;
            foreach (Vector3 point in pointsOut)
            {
                lines[index++] = new PointF((float)point.X, (float)point.Y);
            }
            lines[index] = lines[0];
            
        }

        private void ComputeDisplayParametersForThumbnail()
        {
            if (currentConstellation == "Error")
            {
                return;
            }
            IPlace target = Constellations.ConstellationCentroids[currentConstellation];
            Lineset boundries = Constellations.boundries[currentConstellation];

            Vector3[] points = new Vector3[boundries.Points.Count];
            Vector3[] pointsOut = new Vector3[boundries.Points.Count]; 
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = Coordinates.GeoTo3d(boundries.Points[i].Dec, boundries.Points[i].RA);
            }

            Matrix mat = Matrix.RotationY((float)(-(24 - (target.RA + 6)) / 12 * Math.PI));

            mat = Matrix.Multiply(mat, Matrix.RotationX((float)(target.Lat / 180f * Math.PI)));
            mat = Matrix.Multiply(mat, Matrix.Scaling(50, -50, 50));
            mat = Matrix.Multiply(mat, Matrix.Translation(48, 22, 0));
            // mat.Translate(58, 33,0);

            Vector3.TransformCoordinate(points, ref mat, pointsOut);

            lines = new PointF[points.Length + 1];

            int index = 0;
            foreach (Vector3 point in pointsOut)
            {
                lines[index++] = new PointF((float)point.X, (float)point.Y);
            }
            lines[index] = lines[0];

        }
        

        public void ThumbnailAllConstellations()
        {
            fov = null;
            foreach (KeyValuePair<string,string> kv in Constellations.Abbreviations)
            {
                currentConstellation = kv.Value;
                ComputeDisplayParametersForThumbnail();
                Bitmap bmp = new Bitmap(96, 45);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.Black);
                DrawConstellationThumbnail(g);
                g.Dispose();


                ThumbnailCache.ConstellationThumbnails[kv.Value] = bmp;
            }
        }
    }
}
