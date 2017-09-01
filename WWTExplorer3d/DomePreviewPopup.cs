using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DomePreviewPopup : Form
    {
        public DomePreviewPopup()
        {
            InitializeComponent();
        }

        private void SetUiStrings()
        {
            this.Text = Language.GetLocalizedText(1360, "Dome Preview");
        }

        private void DomePreviewPopup_Load(object sender, EventArgs e)
        {
            Active = true;
            Alt = 0;
            Az = 0;
            TopMost = true;
            UpdateText();
        }

        public static bool Active = false;
        public static double Alt = 0;
        public static double Az = 0;

        private void DomePreviewPopup_FormClosed(object sender, FormClosedEventArgs e)
        {
            Active = false;
            Alt = 0;
            Az = 0;
        }

        bool mouseDown = false;
        Point pointDown;
        private void DomePreviewPopup_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            pointDown = e.Location;
        }

        private void DomePreviewPopup_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
        }

        private void DomePreviewPopup_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Alt = Alt + (pointDown.Y - e.Location.Y);
                Az = Az + (pointDown.X - e.Location.X);

                Az = (Az + 360) % 360;

                Alt = Math.Min(90, Math.Max(0, Alt));

                UpdateText();
                pointDown = e.Location;
            }
        }

        private void UpdateText()
        {
            AltAzText.Text = string.Format(Language.GetLocalizedText(1358, "Alt") + ": {0}  " + Language.GetLocalizedText(1359, "Az") + ": {1}", Alt, Az);
        }

        Coordinates[] cornersAltAz = null;
        string north = "N";
        string west = "W";
        string east = "E";
        string south = "S";
        private void DomePreviewPopup_Paint(object sender, PaintEventArgs e)
        {
           //cornersAltAz = new Coordinates[4];
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int radius = 66;
            centerf = new PointF(96, 88);
            Point center = new Point(96, 88);

            Rectangle rectSphere = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            SolidBrush grayBrush = new SolidBrush(Color.Gray);
            SolidBrush yellowBrush = new SolidBrush(Color.Yellow);
            SolidBrush darkBlueBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 24));
            Pen darkYellowPen = new Pen(Color.FromArgb(192, 192, 64));

            Rectangle rectMeridians = new Rectangle(center.X - 7, center.Y - radius, 14, radius * 2);
            Rectangle rectEquator = new Rectangle(center.X - radius, center.Y - 7, radius * 2, 14);

            g.FillEllipse(grayBrush, rectSphere);
            g.DrawEllipse(Pens.White, rectSphere);
            g.DrawString(north, UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(center.X - 4, 10));
            g.DrawString(west, UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(18, center.Y - 6));
            g.DrawString(east, UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(center.X + radius, center.Y - 6));
            g.DrawString(south, UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(center.X - 4, center.Y + radius));

            if (cornersAltAz != null)
            {
                // Draw the FOV indicator
                PointF[] points = new PointF[4];
                const double RC = (Math.PI / 180.0);
                for (int i = 0; i < 4; i++)
                {
                    points[i].X = centerf.X - (float)(Math.Cos((cornersAltAz[i].Az+90) * RC) * Math.Cos(cornersAltAz[i].Alt * RC) * radius);
                    points[i].Y = centerf.Y - (float)(Math.Sin((cornersAltAz[i].Az+90) * RC) * Math.Cos(cornersAltAz[i].Alt * RC) * radius);
                    g.DrawLine(Pens.Yellow, points[i], centerf);
                }
                if (RenderEngine.Engine.Alt > 0)
                {
                    g.FillPolygon(yellowBrush, points);
                }
                else
                {
                    g.FillPolygon(darkBlueBrush, points);
                }
            }

            darkYellowPen.Dispose();
            GC.SuppressFinalize(darkYellowPen);
            darkBlueBrush.Dispose();
            GC.SuppressFinalize(darkBlueBrush);
            yellowBrush.Dispose();
            GC.SuppressFinalize(yellowBrush);
            grayBrush.Dispose();
            GC.SuppressFinalize(grayBrush);
        }
        PointF centerf = new PointF(48, 47);
    }  
}
