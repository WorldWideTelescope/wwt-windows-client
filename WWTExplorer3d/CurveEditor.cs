using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class CurveEditor : UserControl
    {
        public CurveEditor()
        {
            InitializeComponent();

        }

        private double p1 = 0;

        public double P1
        {
            get { return p1; }

            set
            {
                p1 = value;
                Invalidate();
            }
        }

        private double p2 = 1;

        public double P2
        {
            get { return p2; }
            set
            {
                p2 = value;
                Invalidate();
            }
        }

        private double p3 = 1;

        public double P3
        {
            get { return p3; }
            set
            {
                p3 = value;
                Invalidate();
            }
        }

        private double p4 = 0;

        public double P4
        {
            get { return p4; }
            set
            {
                p4 = value;
                Invalidate();
            }
        }

        Key.KeyType curveType = Key.KeyType.Custom;

        public Key.KeyType CurveType
        {
            get { return curveType; }
            set
            {
                curveType = value;
                Invalidate();
            }
        }


        public event EventHandler ValueChanged;


        private void CurveEditor_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;


            int step = Width / 10;
            for (int x = 0; x <= Width; x += step)
            {
                g.DrawLine(Pens.Gray, x, 0, x, Height);
            }

            for (int y = 0; y <= Height; y += step)
            {
                g.DrawLine(Pens.Gray, 0, y, Width, y);
            }

            g.DrawLine(Pens.White, Width / 2, 0, Width / 2, Height);

            g.DrawLine(Pens.White, 0, Height / 2, Width, Height / 2);

            Vector2d first = new Vector2d(0, 0);
            Vector2d control1 = new Vector2d(P1, P2);
            Vector2d control2 = new Vector2d(P3, P4);
            Vector2d last = new Vector2d(1, 1);


            Vector2d pnt1 = first;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            float w = Width - 1;

            for (double tween = 0; tween <= 1.05; tween += (1 / 44.0))
            {
                Vector2d pnt2 = ComputeSpline(first, control1, control2, last, tween);
                g.DrawLine(Pens.White, (float)pnt1.X * w, w - (float)pnt1.Y * w, (float)pnt2.X * w, w - (float)pnt2.Y * w);
                pnt1 = pnt2;
            }

            if (CurveType == Key.KeyType.Custom)
            {
                g.DrawLine(Pens.Yellow, 0, w, (float)P1 * w, w - (float)P2 * w);
                g.DrawLine(Pens.Yellow, w, 0, (float)P3 * w, w - (float)P4 * w);
                g.DrawRectangle(Pens.Yellow, ((float)P1 * w) - 4, (w - (float)P2 * w) - 4, 8, 8);
                g.DrawRectangle(Pens.Yellow, ((float)P3 * w) - 4, (w - (float)P4 * w) - 4, 8, 8);
            }
        }

        bool dragging1 = false;
        bool dragging2 = false;
        private void CurveEditor_MouseDown(object sender, MouseEventArgs e)
        {
            double x = Math.Max(0, Math.Min(1, (double)e.X / (double)Width));
            double y = Math.Max(0, Math.Min(1, 1.0 - (double)e.Y / (double)Height));

            if (Math.Sqrt((P1 - x) * (P1 - x) + (P2 - y) * (P2 - y)) < .07)
            {
                dragging1 = true;
                return;

            }

            if (Math.Sqrt((P3 - x) * (P3 - x) + (P4 - y) * (P4 - y)) < .07)
            {
                dragging2 = true;
                return;

            }
        }

        private void CurveEditor_MouseMove(object sender, MouseEventArgs e)
        {
            double x = Math.Max(0, Math.Min(1, (double)e.X / (double)Width));
            double y = Math.Max(0, Math.Min(1, 1.0 - (double)e.Y / (double)Height));

            if (dragging1)
            {
                P1 = x;
                P2 = y;
                Refresh();
            }
            if (dragging2)
            {
                P3 = x;
                P4 = y;
                Refresh();
            }
            Refresh();
        }

        private void CurveEditor_MouseUp(object sender, MouseEventArgs e)
        {
            if (dragging1 || dragging2)
            {
                if (ValueChanged != null)
                {
                    ValueChanged.Invoke(this, new EventArgs());
                }
            }
            dragging1 = false;
            dragging2 = false;
        }

        private Vector2d ComputeSpline(Vector2d begin, Vector2d control1, Vector2d control2, Vector2d end, double tween)
        {
            if (curveType == Key.KeyType.Custom)
            {
                Vector2d A1 = Vector2d.Lerp(begin, control1, tween);
                Vector2d A2 = Vector2d.Lerp(control1, control2, tween);
                Vector2d A3 = Vector2d.Lerp(control2, end, tween);

                Vector2d B1 = Vector2d.Lerp(A1, A2, tween);
                Vector2d B2 = Vector2d.Lerp(A2, A3, tween);
                return Vector2d.Lerp(B1, B2, tween);
            }
            else
            {
                return new Vector2d(tween, Key.EaseCurve(curveType, tween, control1.X, control1.Y, control2.X, control2.Y));
            }
        }
    }
}
