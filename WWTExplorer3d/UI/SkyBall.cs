using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;


namespace TerraViewer
{
    public partial class SkyBall : UserControl
    {
        public SkyBall()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MainWndow_MouseWheel);

        }

        double ra;

        public double RA
        {
            get { return ra; }
            set 
            {
                if (ra != value)
                {
                    ra = value;
                    if (space)
                    {
                        this.raLabel.Text = Language.GetLocalizedText(364, "RA :   ") + Coordinates.FormatHMS(ra);
                    }
                    else
                    {
                        this.raLabel.Text = Language.GetLocalizedText(365, "Lng:   ") + Coordinates.FormatDMS(ra);
                    }
                    RefreshHint();
                }
            }
        }
        double dec;

        public double Dec
        {
            get { return dec; }
            set 
            {
                if (dec != value)
                {
                    dec = value;
                    if (space)
                    {
                        this.decLabel.Text = Language.GetLocalizedText(366, "Dec :  ") + Coordinates.FormatDMS(dec, true);
                    }
                    else
                    {
                        this.decLabel.Text = Language.GetLocalizedText(367, "Lat :  ") + Coordinates.FormatDMS(dec, true);
                    }
                    RefreshHint();
                }
            }
        }

        Coordinates[] corners;
        Coordinates[] cornersAltAz;

        public void RefreshHint()
        {
            this.Invalidate();
        }

        public void SetViewRect(Coordinates[] corners)
        {
            if (corners.Length != 4)
            {
                return;
            }
            var change = false;
            if (this.corners != null)
            {
                for (var i = 0; i < 4; i++)
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
            if (Settings.Active.LocalHorizonMode)
            {
                cornersAltAz = new Coordinates[4];
                for (var i = 0; i < 4; i++)
                {
                    this.cornersAltAz[i] = Coordinates.EquitorialToHorizon(corners[i], SpaceTimeController.Location, SpaceTimeController.Now);
                }
            }

            this.corners = corners;

            RefreshHint();
        }

        private void SkyBall_Load(object sender, EventArgs e)
        {
            north = Language.GetLocalizedText(368, "N");
            west = Language.GetLocalizedText(369, "W");
            east = Language.GetLocalizedText(370, "E");
            south = Language.GetLocalizedText(371, "S");
        }

        


        bool space = true;

        public bool Space
        {
            get { return space; }
            set
            {
                if (space != value)
                {
                    space = value;
                    RefreshHint();
                }
            }
        }

        bool showSkyball = true;

        public bool ShowSkyball
        {
            get { return showSkyball; }
            set 
            {
                if (showSkyball != value)
                {
                    showSkyball = value;
                    RefreshHint();
                }
            }
        }
        
        private void SkyBall_Paint(object sender, PaintEventArgs e)
        {
            if (showSkyball && Height > 100)
            {
                if (space)
                {
                    if (Settings.Active.LocalHorizonMode)
                    {
                        PaintAltAz(e);
                    }
                    else
                    {
                        PaintEquitorial(e);
                    }
                }
                else
                {
                    PaintEarth(e);
                }
            
            }
            else
            {
                var g = e.Graphics;
                g.Clear(BackColor);

            }

        }

        string north = "N";
        string west = "W";
        string east = "E";
        string south = "S";
        private void PaintAltAz(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var radius = 33;
            centerf = new PointF(48, 44);
            var center = new Point(48, 44);

            var rectSphere = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            var grayBrush = new SolidBrush(Color.Gray);
            var yellowBrush = new SolidBrush(Color.Yellow);
            var darkBlueBrush = new SolidBrush(Color.FromArgb(255, 0, 0, 24));
            var darkYellowPen = new Pen(Color.FromArgb(192, 192, 64));

            var rectMeridians = new Rectangle(center.X - 7, center.Y - radius, 14, radius * 2);
            var rectEquator = new Rectangle(center.X - radius, center.Y - 7, radius * 2, 14);

            g.FillEllipse(grayBrush, rectSphere);
            g.DrawEllipse(Pens.White, rectSphere);
            g.DrawString(north, UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(center.X - 4, -1));
            g.DrawString(west, UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(4, center.Y - 6));
            g.DrawString(east, UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(82, center.Y - 6));
            g.DrawString(south, UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(center.X-4, center.Y + 33));

            if (cornersAltAz != null)
            {
                // Draw the FOV indicator
                var points = new PointF[4];
                const double RC = (Math.PI / 180.0);

                for (var i = 0; i < 4; i++)
                {
                    points[i].X = centerf.X - (float)(Math.Cos((cornersAltAz[i].Az+90) * RC) * Math.Cos(cornersAltAz[i].Alt * RC) * radius);
                    points[i].Y = centerf.Y - (float)(Math.Sin((cornersAltAz[i].Az+90) * RC) * Math.Cos(cornersAltAz[i].Alt * RC) * radius);
                    g.DrawLine(Pens.Yellow, points[i], centerf);
                }
                if (Earth3d.MainWindow.Alt > 0)
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
        private void PaintEquitorial(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var radius = 33;
            centerf = new PointF(48, 47);
            var center = new Point(48, 47);

            var rectSphere = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            var grayBrush = new SolidBrush(Color.Gray);
            var yellowBrush = new SolidBrush(Color.Yellow);
            var darkYellowBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 0));
            var darkYellowPen = new Pen(Color.FromArgb(192, 192, 64));

            var rectMeridians = new Rectangle(center.X - 7, center.Y - radius, 14, radius * 2);
            var rectEquator = new Rectangle(center.X - radius, center.Y - 7, radius * 2, 14);
            g.DrawString(north, UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(center.X - 4, -1));
            g.FillEllipse(grayBrush, rectSphere);
            g.DrawEllipse(Pens.White, rectSphere);
            g.DrawEllipse(Pens.LightGray, rectMeridians);
            g.DrawEllipse(Pens.LightGray, rectEquator);
            g.DrawLine(Pens.LightGray, center.X, center.Y - radius, center.X, center.Y + radius);
            g.DrawLine(Pens.LightGray, 15, center.Y, center.X + radius, center.Y);
            g.DrawLine(Pens.LightGray, center.X - 7, center.Y - 7, center.X + 7, center.Y + 7);

            if (corners != null)
            {
                // Draw the FOV indicator
                var points = new PointF[4];
                const double RC = (Math.PI / 180.0);

                //return new Vector3((float)(Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (float)(Math.Sin(lat * RC) * radius), (float)(Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius));
                //return new Vector3((float)(Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (float)(Math.Sin(lat * RC) * radius), (float)(Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius));

                double z = 0;
                for (var i = 0; i < 4; i++)
                {
                    points[i].X = centerf.X - (float)(Math.Cos((corners[i].RA + 6) / 12 * 180 * RC) * Math.Cos(corners[i].Lat * RC) * radius);
                    points[i].Y = centerf.Y - (float)(Math.Sin(corners[i].Lat * RC) * radius);
                    z += (Math.Sin((corners[i].RA + 6) / 12 * 180 * RC) * Math.Cos(corners[i].Lat * RC) * radius);

                    g.DrawLine(Pens.Yellow, points[i], centerf);
                }
                if ((z / 4) > 0)
                {
                    g.FillPolygon(yellowBrush, points);
                }
                else
                {
                    g.FillPolygon(darkYellowBrush, points);
                }

            }

            darkYellowPen.Dispose();
            GC.SuppressFinalize(darkYellowPen);
            darkYellowBrush.Dispose();
            GC.SuppressFinalize(darkYellowBrush);
            yellowBrush.Dispose();
            GC.SuppressFinalize(yellowBrush);
            grayBrush.Dispose();
            GC.SuppressFinalize(grayBrush);
        }
        private void PaintEarth(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            var radius = 33;
            centerf = new PointF(48, 47);
            var center = new Point(48, 47);

            var rectSphere = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            var rectInnerSphere = new Rectangle(center.X - 16, center.Y - 16, 16 * 2, 16 * 2);
            var grayBrush = new SolidBrush(Color.Gray);
            var yellowBrush = new SolidBrush(Color.Yellow);
            var darkYellowBrush = new SolidBrush(Color.FromArgb(128, 255, 255, 0));
            var darkYellowPen = new Pen(Color.FromArgb(192, 192, 64));

            var rectMeridians = new Rectangle(center.X - 7, center.Y - radius, 14, radius * 2);
            var rectEquator = new Rectangle(center.X - radius, center.Y - 7, radius * 2, 14);
            g.DrawString(north, UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(center.X - 4, -1));
            g.FillEllipse(grayBrush, rectSphere);
            g.DrawEllipse(Pens.White, rectSphere);
            g.FillEllipse(yellowBrush, rectInnerSphere);

            darkYellowPen.Dispose();
            darkYellowBrush.Dispose();
            yellowBrush.Dispose();
            grayBrush.Dispose();
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            Earth3d.MainWindow.MoveView(-moveVector.X, moveVector.Y, false);
        }

        bool mouseDown;
        private void SkyBall_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                var contextMenu = new ContextMenuStrip();
                var copyMenu = new ToolStripMenuItem(Language.GetLocalizedText(1277, "Copy Coordinates"));
                copyMenu.Click += new EventHandler(copyMenu_Click);
                contextMenu.Items.Add(copyMenu);
                contextMenu.Show(Cursor.Position);
            }
            else
            {
                mouseDown = true;
                MoveTimer.Enabled = true;
            }
        }

        void copyMenu_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(dec.ToString() + ", " + RA.ToString());
        }
        PointF moveVector;
        private void SkyBall_MouseMove(object sender, MouseEventArgs e)
        {
            moveVector = new PointF(centerf.X - e.X, centerf.Y - e.Y);
        }

        private void SkyBall_MouseUp(object sender, MouseEventArgs e)
        {
            MoveTimer.Enabled = false;
            mouseDown = false;

        }
        private void MainWndow_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            if (e.Delta != 0)
            {                
                Earth3d.MainWindow.ZoomSpeed = (Earth3d.ZoomSpeeds)Properties.Settings.Default.ZoomSpeed;
                if (e.Delta < 0)
                {
                    Earth3d.MainWindow.ZoomOut();
                }
                else
                {
                    Earth3d.MainWindow.ZoomIn();
                }

            }
        }
    }
}
