using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class Histogram : Form
    {
        public Histogram()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            Text = Language.GetLocalizedText(863, "Histogram");
        }

        static Histogram singleton;
        public static void ShowHistogram(IImageSet imageset, bool toggle, Point pnt)
        {
            if (imageset.WcsImage is FitsImage)
            {
                if (singleton == null)
                {
                    singleton = new Histogram();
                    singleton.Owner = Earth3d.MainWindow;
                }
                else
                {
                    if (toggle)
                    {

                        HideHistogram();
                    return;
                    }
                    singleton.Hide();
                }
                singleton.Top = pnt.Y - singleton.Height;
                singleton.Left = pnt.X;
                singleton.Show();
                singleton.Top = pnt.Y - singleton.Height;
                singleton.Left = pnt.X;
                singleton.Image = (FitsImage)imageset.WcsImage;
                singleton.Tile = (SkyImageTile)TileCache.GetTile(0, 0, 0, imageset, null);
            }
        }
        public static void HideHistogram()
        {

            if (singleton != null)
            {
                singleton.Close();
            }

        }
        private void Histogram_Load(object sender, EventArgs e)
        {
            string[] items = { "Linear", "Log", "Power", "Square Root", "Histogram Equalization" };
            scaleType.Items.AddRange(items);
            scaleType.SelectedIndex = 0;
        }
        private SkyImageTile tile;

        internal SkyImageTile Tile
        {
            get { return tile; }
            set { tile = value; }
        }


        private FitsImage image;

        internal FitsImage Image
        {
            get { return image; }
            set
            {
                image = value;
                if (HistogramView.Image != null)
                {
                    HistogramView.Image.Dispose();
                }
                HistogramView.Image = image.GetHistogramBitmap(image.HistogramMaxCount);
                highPosition = image.lastMax;
                lowPosition = image.lastMin;
                center = (lowPosition + highPosition) / 2;
                scaleType.SelectedIndex = (int)image.lastScale;

                HistogramView.Refresh();
            }
        }

        private void HistogramView_Click(object sender, EventArgs e)
        {

        }

        private void HistogramView_MouseDown(object sender, MouseEventArgs e)
        {
            //todo unhack this and move to constants
            if ((Math.Abs(e.X - center) < 10) && Math.Abs(e.Y - 75) < 10)
            {
                dragType = DragType.Center;
            }
            else if (Math.Abs(e.X - lowPosition) < 3)
            {
                dragType = DragType.Low;
            }
            else if (Math.Abs(e.X - highPosition) < 3)
            {
                dragType = DragType.High;
            }
            else
            {
                dragType = DragType.Range;
                downPosition = Math.Min(255, Math.Max(0, e.X));
                
                HistogramView.Refresh();
            }


        }

        private void HistogramView_MouseMove(object sender, MouseEventArgs e)
        {
            switch (dragType)
            {
                case DragType.Low:
                    lowPosition = Math.Min(255, Math.Max(0, e.X));
                    break;
                case DragType.High:
                    highPosition = Math.Min(255, Math.Max(0, e.X));
                    break;
                case DragType.Range:
                    lowPosition = downPosition;
                    highPosition = Math.Min(255, Math.Max(0, e.X));
                    break;
                case DragType.Center:
                    var hWidth = Math.Abs(highPosition-lowPosition)/2;
                    var adCenter = Math.Min(255 - hWidth, Math.Max(hWidth, e.X));
                    var moved = center - adCenter;
                    lowPosition -= moved;
                    highPosition -= moved;
                    break;
                case DragType.None:
                    return;
                default:
                    break;
            }
            center = (lowPosition + highPosition) / 2;
            HistogramView.Refresh();
            var factor = (image.MaxVal - image.MinVal) / 256.0;
            var low = image.MinVal + (lowPosition * factor);
            var hi = image.MinVal + (highPosition * factor);

            Tile = (SkyImageTile)TileCache.GetTile(Tile.Level, Tile.X, Tile.Y, Tile.Dataset, null);
            updateTimer.Enabled = false;
            updateTimer.Enabled = true;
            image.lastMax = highPosition;
            image.lastMin = lowPosition;

        }

        private void HistogramView_MouseUp(object sender, MouseEventArgs e)
        {
            if (dragType != DragType.None)
            {
                dragType = DragType.None;
                updateTimer.Enabled = false;
                updateTimer.Enabled = true;
            }

        }

        int downPosition;
        int lowPosition;
        int highPosition = 255;
        int center = 127;

        enum DragType { Low, High, Range, Center, None };
        DragType dragType = DragType.None;

        private void HistogramView_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(Pens.Red, new Point(lowPosition, 0), new Point(lowPosition, 150));
            e.Graphics.DrawLine(Pens.Green, new Point(highPosition, 0), new Point(highPosition, 150));
            e.Graphics.DrawEllipse(Pens.Blue, new Rectangle(center-10, 65, 20, 20));

            var Curve = new List<PointF>();
            switch (scaleType.SelectedIndex)
            {
                case 0: // Linear
                    {      
                        Curve.Clear();
                        Curve.Add(new Point(lowPosition, 150));
                        Curve.Add(new Point(highPosition, 0));
                        break;
                    }
                case 1: // Log
                    {
                        Curve.Clear();
                        var factor = 150 / Math.Log(255);
                        double diff = (highPosition - lowPosition);
                        var jump = Math.Sign(diff);
                        var step = Math.Abs(256.0 / (diff == 0 ? .000001 : diff));
                        var val = .000001;
                        for (var i = lowPosition; i != highPosition; i += jump)
                        {
                            Curve.Add(new PointF(i, (float)(150 - (Math.Log(val) * factor))));
                            val += step;
                        }
                    }
                    break;
                case 2: // Power 2
                    {
                        Curve.Clear();
                        var factor = 150 / Math.Pow(255, 2);
                        double diff = (highPosition - lowPosition);
                        var jump = Math.Sign(diff);
                        var step = Math.Abs(256.0 / (diff == 0 ? .000001 : diff));
                        var val = .000001;
                        for (var i = lowPosition; i != highPosition; i += jump)
                        {
                            Curve.Add(new PointF(i, (float)(150 - (Math.Pow(val, 2) * factor))));
                            val += step;
                        }
                    }

                    break;
                case 3: // Square Root
                    {
                        Curve.Clear();
                        var factor = 150 / Math.Sqrt(255);
                        double diff = (highPosition - lowPosition);
                        var jump = Math.Sign(diff);
                        var step = Math.Abs(256.0 / (diff == 0 ? .000001 : diff));
                        var val = .000001;
                        for (var i = lowPosition; i != highPosition; i += jump)
                        {
                            Curve.Add(new PointF(i, (float)(150 - (Math.Sqrt(val) * factor))));
                            val += step;
                        }
                    }

                    break;
            }
            if (Curve.Count > 1)
            {
                e.Graphics.DrawLines(Pens.Blue, Curve.ToArray());
            }

        }

        private void Histogram_FormClosed(object sender, FormClosedEventArgs e)
        {
            singleton = null;
        }

        private void scaleType_SelectionChanged(object sender, EventArgs e)
        {
            Refresh();

            if (image != null && scaleType.SelectedIndex != (int)image.lastScale)
            {
                updateTimer.Enabled = true;
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (image != null)
            {
                var factor = (image.MaxVal - image.MinVal) / 256.0;
                var low = image.MinVal + (lowPosition * factor);
                var hi = image.MinVal + (highPosition * factor);
                Tile.SetTexture(image.GetBitmap(low, hi, (ScaleTypes)scaleType.SelectedIndex));
            }
            updateTimer.Enabled = false;

        }
           
    }
}
