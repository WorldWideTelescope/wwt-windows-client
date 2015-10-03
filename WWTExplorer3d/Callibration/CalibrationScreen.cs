using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace TerraViewer.Callibration
{
    public enum GeometryStyles { Points, Lines, Polygon, SoftPolygon };
    public partial class CalibrationScreen : Form
    {
        public CalibrationScreen()
        {
            InitializeComponent();
        }
        Font textFont;
        StringFormat sf;
        private void CalibrationScreen_Load(object sender, EventArgs e)
        {
            Calibrating = true;

            textFont = new Font("MS Sans Serif", 72, FontStyle.Bold, GraphicsUnit.Pixel);
            sf = new StringFormat {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center};
            targetNodeID = Earth3d.MainWindow.Config.NodeID.ToString(CultureInfo.InvariantCulture);

        }

        static public List<PointF> points = new List<PointF>();

        static public GeometryStyles GeometryStyle = GeometryStyles.Points;

        static public Color Background;
        static public int selectedPoint = -1;
        static public bool DrawOutline = false;
        static public Color LineColor = Color.Green;
        static public Color FillColor = Color.White;
        static public string projectorName = "Projector";
        static public string targetNodeID = "0";

        static bool Calibrating;


        internal static void ParseCommandString(string[] values)
        {

            if (values.Length > 3)
            {
                if (!Calibrating && master == null && values[3] != "RELOADWARPS" && values[3] != "UPDATESOFTWARE")
                {
                    Earth3d.MainWindow.ShowCalibrationScreen();
                }
                if ((values[2] == "-1") || (values[2] == Earth3d.MainWindow.Config.NodeID.ToString(CultureInfo.InvariantCulture)))
                {
                    switch (values[3])
                    {
                        case "METADATA":
                            PareseMetaData(values);
                            UpdateScreen();
                            break;
                        case "GEOMETRY":
                            ParseGeometyCommand(values);
                            UpdateScreen();
                            break;
                        case "CLOSE":
                            CloseScreen();
                            break;
                        case "RELOADWARPS":
                            ReloadWarps();
                            break;
                        case "UPDATESOFTWARE":
                            UpdateSoftware();
                            break;
                    }
                }     
                

            }
        }

        public static void UpdateSoftware()
        {
            var client = new WebClient();
            // Get Distortion map
            try
            {
                var url = string.Format("http://{0}:5050/Configuration/images/executable", NetControl.MasterAddress);
                var data = client.DownloadData(url);
                File.WriteAllBytes("wwtexplorer.exe", data);
                System.Diagnostics.Process.Start("wwtexplorer.exe", "restart");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch
            {
            }

        }

        private static void ReloadWarps()
        {
            if (Settings.DomeView)
            {
                var client = new WebClient();
                // Get Distortion map
                try
                {
                    var url = string.Format("http://{0}:5050/Configuration/images/distort_{1}.data", NetControl.MasterAddress, Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                    var filename = "";// Earth3d.MainWindow.Config.DistortionGrid;
                    if (string.IsNullOrEmpty(filename))
                    {
                        if (!Directory.Exists("c:\\wwtconfig"))
                        {
                            Directory.CreateDirectory("c:\\wwtconfig");
                        }
                        filename = string.Format("c:\\wwtconfig\\distort_{0}.data", Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                        Properties.Settings.Default.CustomWarpFilename = filename;
                        Properties.Settings.Default.DomeTypeIndex = 3;
                        var data = client.DownloadData(url);
                        File.WriteAllBytes(filename, data);


                        if (Earth3d.MainWindow.InvokeRequired)
                        {
                            MethodInvoker InvalidateMe = () => Earth3d.MainWindow.CreateWarpVertexBuffer();
                            try
                            {
                                Earth3d.MainWindow.Invoke(InvalidateMe);
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            Earth3d.MainWindow.CreateWarpVertexBuffer();
                        }

                    }
                   
                }
                catch
                {
                }
            }
            else
            {

                var client = new WebClient();
                // Get Distortion map
                try
                {
                    var url = string.Format("http://{0}:5050/Configuration/images/distort_{1}.png", NetControl.MasterAddress, Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                    var filename = Earth3d.MainWindow.Config.DistortionGrid;
                    if (string.IsNullOrEmpty(filename))
                    {
                        if (!Directory.Exists("c:\\wwtconfig"))
                        {
                            Directory.CreateDirectory("c:\\wwtconfig");
                        }
                        filename = string.Format("c:\\wwtconfig\\distort_{0}.png", Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                        Earth3d.MainWindow.Config.DistortionGrid = filename;
                    }
                    var data = client.DownloadData(url);
                    File.WriteAllBytes(filename, data);
                }
                catch
                {
                }
                // Get Blend Map
                try
                {
                    var url = string.Format("http://{0}:5050/Configuration/images/blend_{1}.png", NetControl.MasterAddress, Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                    var filename = Earth3d.MainWindow.Config.BlendFile;
                    if (string.IsNullOrEmpty(filename))
                    {
                        if (!Directory.Exists("c:\\wwtconfig"))
                        {
                            Directory.CreateDirectory("c:\\wwtconfig");
                        }
                        filename = string.Format("c:\\wwtconfig\\blend_{0}.png", Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                        Earth3d.MainWindow.Config.BlendFile = filename;

                    }
                    var data = client.DownloadData(url);
                    File.WriteAllBytes(filename, data);
                }
                catch
                {
                }



                Earth3d.MainWindow.refreshWarp = true;
            }
        }

        public static void ShowWindow()
        {
            if (!Calibrating && master == null)
            {
                master = new CalibrationScreen {Owner = Earth3d.MainWindow};
                master.Show();
            }
        }

        static CalibrationScreen master;
        static void UpdateScreen()
        {
            if (master != null)
            {

                if (master.InvokeRequired)
                {
                    MethodInvoker InvalidateMe = () => master.Invalidate();
                    try
                    {
                        master.Invoke(InvalidateMe);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    master.Invalidate();
                }
            }
        }


        static void CloseScreen()
        {
            if (master != null)
            {

                if (master.InvokeRequired)
                {
                    MethodInvoker InvalidateMe = () => master.Close();
                    try
                    {
                        master.Invoke(InvalidateMe);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    master.Close();
                    master = null;
                }
            }
        }



        private static void ParseGeometyCommand(string[] values)
        {
            if (values.Length > 5)
            {
                GeometryStyle = (GeometryStyles)Enum.Parse(typeof(GeometryStyles), values[4]);
                selectedPoint = int.Parse(values[5]);
                if (values.Length > 7)
                {
                    FillColor = SavedColor.Load(values[6]);
                    LineColor = SavedColor.Load(values[7]);
                }
                if (values.Length > 8)
                {
                    points.Clear();
                    var pointListText = values[8].Split(new[] { ';' });
                    foreach (var pointText in pointListText)
                    {
                        var parts = pointText.Split(new[] { ' ' });
                        if (parts.Length > 1)
                        {
                            points.Add(new PointF(float.Parse(parts[0]), float.Parse(parts[1])));
                        }
                    }
                }

            }

        }

        private static void PareseMetaData(string[] values)
        {
            if (values.Length > 6)
            {
                Earth3d.MainWindow.Config.NodeDiplayName = values[4];
                Background = SavedColor.Load(values[5]);
                DrawOutline = Boolean.Parse(values[6]);
            }
        }


        private void CalibrationScreen_Paint(object sender, PaintEventArgs e)
        {
            var rect = this.ClientRectangle;

            e.Graphics.Clear(Background);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality; 

            if (DrawOutline)
            {
                var linePen = new Pen(Color.Red, 4);
                var smaller = rect;
                smaller.Inflate(-2, -2);
                e.Graphics.DrawRectangle(linePen, smaller);
                linePen.Dispose();
            }


            switch (GeometryStyle)
            {
                case GeometryStyles.Points:
                    DrawPoints(e.Graphics);
                    break;
                case GeometryStyles.Lines:
                    DrawLines(e.Graphics);
                    break;
                case GeometryStyles.Polygon:
                    DrawPolygon(e.Graphics);
                    break;
                case GeometryStyles.SoftPolygon:
                    DrawPolygon(e.Graphics);
                    break;
            }

            if (selectedPoint > -1 && selectedPoint < points.Count)
            {
                var linePen = new Pen(LineColor, 3);

                DrawPoint(e.Graphics, linePen, points[selectedPoint]);

                linePen.Dispose();
            }
           
            Brush brush = new SolidBrush(Color.LightGreen);

            e.Graphics.DrawString(Earth3d.MainWindow.Config.NodeDiplayName, textFont, brush, new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), sf);



        }

        private static void DrawPolygon(Graphics g)
        {
            Brush brush = new SolidBrush(FillColor);

            if (points.Count > 2)
            {
                g.FillPolygon(brush, points.ToArray());
            }
            brush.Dispose();
        }

        private static void DrawLines(Graphics g)
        {
            var linePen = new Pen(LineColor, 1);
            if (points.Count > 1)
            {

                g.DrawLines(linePen, points.ToArray());
            }
            linePen.Dispose();
        }

        private static void DrawPoints(Graphics g)
        {
            var linePen = new Pen(FillColor, 2);
            try
            {
                foreach (var point in points)
                {
                    DrawPoint(g, linePen, point);
                }
            }
            catch
            {
            }
            linePen.Dispose();
        }

        private static void DrawPoint(Graphics g, Pen linePen, PointF point)
        {
            g.DrawLine(linePen, PointF.Add(point, new Size(0, 15)), PointF.Subtract(point, new Size(0, 15)));
            g.DrawLine(linePen, PointF.Add(point, new Size(15, 0)), PointF.Subtract(point, new Size(15, 0)));

            var rect = new RectangleF(PointF.Subtract(point, new Size(15, 15)), new SizeF(30, 30));
            g.DrawEllipse(linePen, rect);
        }

        private void CalibrationScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                points.Add(new PointF(e.X, e.Y));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                var x = (int)GeometryStyle;
                x++;
                x = x % 3;
                GeometryStyle = (GeometryStyles)x;
            }
            else
            {
                if (ModifierKeys == Keys.Shift)
                {
                    selectedPoint++;
                    selectedPoint = selectedPoint % points.Count;
                }
                else if (ModifierKeys == Keys.Alt)
                {
                    selectedPoint = -1;
                }
                else
                {
                    drag = true;
                }
            }
            Invalidate();
        }
        bool drag;
        private void CalibrationScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            master = null;
            Calibrating = false;
        }

        private void CalibrationScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag && selectedPoint > -1)
            {
                points[selectedPoint] = new PointF(e.X, e.Y);
            }
            Invalidate();
        }

        private void CalibrationScreen_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
        }

    }
}
