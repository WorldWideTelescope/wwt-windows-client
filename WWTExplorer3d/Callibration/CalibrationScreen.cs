using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace TerraViewer.Callibration
{
    public enum GeometryStyles { Points, Lines, Polygon, SoftPolygon };
    public enum CalibrationImageType { None, Bkack, White, Squares, SmallSquares, Points, DensePoints, Checkerboard };
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
            sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            targetNodeID = Earth3d.MainWindow.Config.NodeID.ToString();

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
        static public bool ShowCalibrationImage = false;
        static public CalibrationImageType CalibrationType = CalibrationImageType.None;

        static bool Calibrating = false;


        internal static void ParseCommandString(string[] values)
        {

            if (values.Length > 3)
            {
                if (!Calibrating && master == null && values[3] != "RELOADWARPS" && values[3] != "UPDATESOFTWARE")
                {
                    Earth3d.MainWindow.ShowCalibrationScreen();
                }
                if ((values[2] == "-1") || (values[2] == Earth3d.MainWindow.Config.NodeID.ToString()))
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
            WebClient client = new WebClient();
            // Get Distortion map
            try
            {
                string url = string.Format("http://{0}:5050/Configuration/images/executable", NetControl.MasterAddress);
                Byte[] data = client.DownloadData(url);
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
                WebClient client = new WebClient();
                // Get Distortion map
                try
                {
                    string url = string.Format("http://{0}:5050/Configuration/images/distort_{1}.data", NetControl.MasterAddress, Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                    string filename = "";// Earth3d.MainWindow.Config.DistortionGrid;
                    if (string.IsNullOrEmpty(filename))
                    {
                        if (!Directory.Exists("c:\\wwtconfig"))
                        {
                            Directory.CreateDirectory("c:\\wwtconfig");
                        }
                        filename = string.Format("c:\\wwtconfig\\distort_{0}.data", Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                        Properties.Settings.Default.CustomWarpFilename = filename;
                        Properties.Settings.Default.DomeTypeIndex = 3;
                        Byte[] data = client.DownloadData(url);
                        File.WriteAllBytes(filename, data);


                        if (Earth3d.MainWindow.InvokeRequired)
                        {
                            MethodInvoker InvalidateMe = delegate
                            {
                                Earth3d.MainWindow.CreateWarpVertexBuffer();
                            };
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

                WebClient client = new WebClient();
                // Get Distortion map
                try
                {
                    string url = string.Format("http://{0}:5050/Configuration/images/distort_{1}.png", NetControl.MasterAddress, Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                    string filename = Earth3d.MainWindow.Config.DistortionGrid;
                    if (string.IsNullOrEmpty(filename))
                    {
                        if (!Directory.Exists("c:\\wwtconfig"))
                        {
                            Directory.CreateDirectory("c:\\wwtconfig");
                        }
                        filename = string.Format("c:\\wwtconfig\\distort_{0}.png", Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                        Earth3d.MainWindow.Config.DistortionGrid = filename;
                    }
                    Byte[] data = client.DownloadData(url);
                    File.WriteAllBytes(filename, data);
                }
                catch
                {
                }
                // Get Blend Map
                try
                {
                    string url = string.Format("http://{0}:5050/Configuration/images/blend_{1}.png", NetControl.MasterAddress, Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                    string filename = Earth3d.MainWindow.Config.BlendFile;
                    if (string.IsNullOrEmpty(filename))
                    {
                        if (!Directory.Exists("c:\\wwtconfig"))
                        {
                            Directory.CreateDirectory("c:\\wwtconfig");
                        }
                        filename = string.Format("c:\\wwtconfig\\blend_{0}.png", Earth3d.MainWindow.Config.NodeDiplayName.Replace(" ","_"));
                        Earth3d.MainWindow.Config.BlendFile = filename;

                    }
                    Byte[] data = client.DownloadData(url);
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
                master = new CalibrationScreen();
                master.Owner = Earth3d.MainWindow;
                master.Show();
            }
        }

        static CalibrationScreen master = null;
        static void UpdateScreen()
        {
            if (master != null)
            {

                if (master.InvokeRequired)
                {
                    MethodInvoker InvalidateMe = delegate
                        {
                            master.Invalidate();
                        };
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
                    MethodInvoker InvalidateMe = delegate
                    {
                        master.Close();
                    };
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
                    string[] pointListText = values[8].Split(new char[] { ';' });
                    foreach (string pointText in pointListText)
                    {
                        string[] parts = pointText.Split(new char[] { ' ' });
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
            if (values.Length > 8)
            {
                Earth3d.MainWindow.Config.NodeDiplayName = values[4];
                Background = SavedColor.Load(values[5]);
                DrawOutline = Boolean.Parse(values[6]);
                CalibrationType = (CalibrationImageType)int.Parse(values[7]);
                int node = int.Parse(values[8]);
                ShowCalibrationImage = CalibrationType != CalibrationImageType.None;
                if (!(node == -1 || node == Earth3d.MainWindow.Config.NodeID) && ShowCalibrationImage)
                {
                    CalibrationType = CalibrationImageType.Bkack;
                }
            }
        }

        


        private void CalibrationScreen_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = this.ClientRectangle;

            e.Graphics.Clear(ShowCalibrationImage ? Color.Black : Background);

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            if (ShowCalibrationImage)
            {
                switch (CalibrationType)
                {
                    case CalibrationImageType.Bkack:
                        break;
                    case CalibrationImageType.White:
                        e.Graphics.Clear(Color.White);
                        break;
                    case CalibrationImageType.Squares:
                        DrawGrid(e.Graphics, 50, true);
                        break;
                    case CalibrationImageType.SmallSquares:
                        DrawGrid(e.Graphics, 25, true);
                        break;
                    case CalibrationImageType.Points:
                        DrawGrid(e.Graphics, 50, false);
                        break;
                    case CalibrationImageType.DensePoints:
                        DrawGrid(e.Graphics, 25, false);
                        break;
                    case CalibrationImageType.Checkerboard:
                        break;
                    default:
                        break;
                }     
            }
            else
            {
                if (DrawOutline)
                {
                    Pen linePen = new Pen(Color.Red, 4);
                    Rectangle smaller = rect;
                    smaller.Inflate(-2, -2);
                    e.Graphics.DrawRectangle(linePen, smaller);
                    linePen.Dispose();
                }


                switch (GeometryStyle)
                {
                    case GeometryStyles.Points:
                        DrawPoints(e.Graphics, false);
                        break;
                    case GeometryStyles.Lines:
                        DrawLines(e.Graphics);
                        break;
                    case GeometryStyles.Polygon:
                        DrawPolygon(e.Graphics, false);
                        break;
                    case GeometryStyles.SoftPolygon:
                        DrawPolygon(e.Graphics, true);
                        break;
                    default:
                        break;
                }

                if (selectedPoint > -1 && selectedPoint < points.Count)
                {
                    Pen linePen = new Pen(LineColor, 3);

                    DrawPoint(e.Graphics, linePen, points[selectedPoint]);

                    linePen.Dispose();
                }

                Brush brush = new SolidBrush(Color.LightGreen);

                e.Graphics.DrawString(Earth3d.MainWindow.Config.NodeDiplayName, textFont, brush, new RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height), sf);


            }
        }

        private void DrawGrid(Graphics g, int size, bool squares)
        {
            int countX = 1920 / (size);
            int countY = 1050 / (size);
            int skip = 1;

            if (squares)
            {
                skip = 2;
            }
            bool thisBit = false;

            Brush white = new SolidBrush(Color.White);
            for (int y = 0; y < countY; y += skip)
            {
                for (int x = 0; x < countX; x += skip)
                {
                    if (squares)
                    {
                        g.DrawRectangle(Pens.White, x * size + 35.5f, y * size + 15.5f, size, size);

                        g.FillRectangle(white, new RectangleF(x * size + 35.5f, y * size + 15.5f, size, size));

                        float margin = (float)size / 4.0f;
                        float diam = (float)size / 2.0f;

                        if (x == 18 && y == 10)
                        {
                             g.FillEllipse(Brushes.Red, new RectangleF(x * size + 35.5f+ margin, y * size + 15.5f + margin, diam, diam));
                        }

                        if (x == 18 && y == 8)
                        {
                            g.FillEllipse(Brushes.Blue, new RectangleF(x * size + 35.5f + margin, y * size + 15.5f + margin, diam, diam));
                        }

                        if (x == 16 && y == 10)
                        {
                              g.FillEllipse(Brushes.Green, new RectangleF(x * size + 35.5f + margin, y * size + 15.5f + margin, diam, diam));
                        }

                        // Bits of the Projector ID
                        if (x == 14 && y == 12)
                        {
                            thisBit = (Earth3d.MainWindow.Config.NodeID & 1) == 1;
                            g.FillEllipse(thisBit ? Brushes.Yellow : Brushes.Cyan, new RectangleF(x * size + 35.5f + margin, y * size + 15.5f + margin, diam, diam));
                        }

                        if (x == 16 && y == 12)
                        {
                            thisBit = (Earth3d.MainWindow.Config.NodeID & 2) == 2;
                            g.FillEllipse(thisBit ? Brushes.Yellow : Brushes.Cyan, new RectangleF(x * size + 35.5f + margin, y * size + 15.5f + margin, diam, diam));
                        }

                        if (x == 18 && y == 12)
                        {
                            thisBit = (Earth3d.MainWindow.Config.NodeID & 4) == 4;
                            g.FillEllipse(thisBit ? Brushes.Yellow : Brushes.Cyan, new RectangleF(x * size + 35.5f + margin, y * size + 15.5f + margin, diam, diam));
                        }

                        if (x == 20 && y == 12)
                        {
                            thisBit = (Earth3d.MainWindow.Config.NodeID & 8) == 8;
                            g.FillEllipse(thisBit ? Brushes.Yellow : Brushes.Cyan, new RectangleF(x * size + 35.5f + margin, y * size + 15.5f + margin, diam, diam));
                        }

                        if (x == 22 && y == 12)
                        {
                            thisBit = (Earth3d.MainWindow.Config.NodeID & 16) == 16;
                            g.FillEllipse(thisBit ? Brushes.Yellow : Brushes.Cyan, new RectangleF(x * size + 35.5f + margin, y * size + 15.5f + margin, diam, diam));
                        }

                    }
                    else
                    {
                         g.FillEllipse(white, new RectangleF(x * size + 30.5f, y * size + 10.5f, 10, 10));
                    }

                }
            }
            white.Dispose();

        }

        private void DrawPolygon(Graphics g, bool soft)
        {
            Brush brush = new SolidBrush(FillColor);

            if (points.Count > 2)
            {
                g.FillPolygon(brush, points.ToArray());
            }
            brush.Dispose();
        }

        private void DrawLines(Graphics g)
        {
            Pen linePen = new Pen(LineColor, 1);
            if (points.Count > 1)
            {

                g.DrawLines(linePen, points.ToArray());
            }
            linePen.Dispose();
        }

        private void DrawPoints(Graphics g, bool p)
        {
            Pen linePen = new Pen(FillColor, 2);
            try
            {
                foreach (PointF point in points)
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

            RectangleF rect = new RectangleF(PointF.Subtract(point, new Size(15, 15)), new SizeF(30, 30));
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
                int x = (int)GeometryStyle;
                x++;
                x = x % 3;
                GeometryStyle = (GeometryStyles)x;
            }
            else
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    selectedPoint++;
                    selectedPoint = selectedPoint % points.Count;
                }
                else if (Control.ModifierKeys == Keys.Alt)
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
        bool drag = false;
        private void CalibrationScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            master = null;
           // Earth3d.ReadyToRender = true;
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
