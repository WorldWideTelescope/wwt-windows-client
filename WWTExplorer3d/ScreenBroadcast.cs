using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace TerraViewer
{
    public partial class ScreenBroadcast : Form
    {
        public ScreenBroadcast()
        {
            InitializeComponent();
            
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.Broadcast.Text = Language.GetLocalizedText(1010, "Broadcast Screen");
            this.label1.Text = Language.GetLocalizedText(1011, "Frame Rate");
            this.label2.Text = Language.GetLocalizedText(1012, "When used in a Multi-Channel environment you can broadcast the screen contents from local applications to show up on the projected display as a window.");
            this.label3.Text = Language.GetLocalizedText(763, "Altitude");
            this.label4.Text = Language.GetLocalizedText(765, "Azimuth");
            this.scalelabel.Text = Language.GetLocalizedText(920, "Scale");
            this.Text = Language.GetLocalizedText(1013, "ScreenBroadcast");
            this.ShowLocally.Text = Language.GetLocalizedText(1131, "Show on Console");

        }  
        
        public static void Shutdown()
        {
            if (capturing)
            {
                
                ShutdownClients();
                if (bmp != null)
                {
                    bmp.Dispose();
                    bmp = null;
                }
                capturing = false;
            }

            CleanUpImageSets();
            
        }

        public static void StartUp()
        {
            if (bmp == null)
            {
                bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            }

            if (ScreenImage == null)
            {
                ScreenImage = new Byte[0];
            }

            if (CaptureThread == null)
            {
                CaptureThread = new Thread(new ThreadStart(CaptureThreadFunction));
                CaptureThread.IsBackground = true;
                CaptureThread.Start();
            }
        }


        public static byte[] ScreenImage = null;
        static Bitmap bmp = null;
        private void timer1_Tick(object sender, EventArgs e)
        {


            
        }

        private void ScreenBroadcast_FormClosed(object sender, FormClosedEventArgs e)
        {
            Alt.Text = Properties.Settings.Default.ScreenOverlayAlt.ToString();
            Az.Text = Properties.Settings.Default.ScreenOverlayAz.ToString();
            Scale.Text = Properties.Settings.Default.ScreenOverlayScale.ToString();

            
        }


        private void ScreenBroadcast_Load(object sender, EventArgs e)
        {
            FrameRate.Items.Add(Language.GetLocalizedText(1014, "10 Hertz"));
            FrameRate.Items.Add(Language.GetLocalizedText(1015, "5 Hertz"));
            FrameRate.Items.Add(Language.GetLocalizedText(1016, "2 Hertz"));
            FrameRate.Items.Add(Language.GetLocalizedText(1017, "1 Hz"));
            FrameRate.Items.Add(Language.GetLocalizedText(1018, "2 Seconds"));
            FrameRate.Items.Add(Language.GetLocalizedText(1019, "5 Seconds"));
            FrameRate.Items.Add(Language.GetLocalizedText(1020, "10 Seconds"));

            FrameRate.SelectedIndex = 3;

            Broadcast.Checked = capturing;

            Alt.Text = Properties.Settings.Default.ScreenOverlayAlt.ToString();
            Az.Text = Properties.Settings.Default.ScreenOverlayAz.ToString();
            Scale.Text = Properties.Settings.Default.ScreenOverlayScale.ToString();
            ShowLocally.Checked = Properties.Settings.Default.ScreenOverlayShowLocal;
        }
        
        static int msPerFrame = 1000;
        static bool capturing = false;

        public static bool Capturing
        {
            get { return ScreenBroadcast.capturing; }
            set
            {
                if (value != ScreenBroadcast.capturing)
                {
                   
                    if (value)
                    {
                        StartUp();
                    }
                    else
                    {
                        Shutdown();
                    }
                    ScreenBroadcast.capturing = value;
                }
            }
        }
        private void FrameRate_SelectionChanged(object sender, EventArgs e)
        {
            switch (FrameRate.SelectedIndex)
            {
                case 0:
                    msPerFrame = 100;
                    break;
                case 1:
                    msPerFrame = 200;
                    break;
                case 2:
                    msPerFrame = 500;
                    break;
                case 3:
                    msPerFrame = 1000;
                    break;
                case 4:
                    msPerFrame = 2000;
                    break;
                case 5:
                    msPerFrame = 5000;
                    break;
                case 6:
                    msPerFrame = 10000;
                    break;
                default:
                    msPerFrame= 1000;
                    break;
            }
        }

        private void Broadcast_CheckedChanged(object sender, EventArgs e)
        {
            Capturing = Broadcast.Checked;
        }

        private static void ShutdownClients()
        {
            NetControl.SendCommand(String.Format("SCREEN,{0},0,0,0,,", Earth3d.MainWindow.Config.ClusterID.ToString()));
            CleanUpImageSets();

        }

        private static void CleanUpImageSets()
        {
            if (Earth3d.MainWindow != null && Earth3d.MainWindow.RenderEngine.videoOverlay != null)
            {
                Tile tile = TileCache.GetTile(0, 0, 0, Earth3d.MainWindow.RenderEngine.videoOverlay, null);
                tile.CleanUp(false);
                Earth3d.MainWindow.RenderEngine.videoOverlay = null;
            }
        }
        private static Thread CaptureThread;
        private static bool Dirty = true;
        private static void CaptureThreadFunction()
        {
            int frameNumber = 0;
            while (Earth3d.MainWindow != null)
            {
                
                Thread.Sleep(msPerFrame);
                if (capturing && Earth3d.MainWindow != null)
                {
                    frameNumber++;
                    Graphics g = Graphics.FromImage(bmp);

                    g.CopyFromScreen(0, 0, 0, 0, new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
                    g.Dispose();

                    MemoryStream ms = new MemoryStream();

                    bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    ScreenImage = ms.ToArray();
                    ms.Dispose();
                    ms = null;
                    string url = string.Format(String.Format("http://{0}:5050/images/{1}/screenshot.png",
                                MyWebServer.IpAddress.ToString(),
                                frameNumber.ToString()));



                    NetControl.SendCommand(String.Format("SCREEN,{4},{0},{1},{2},http://{3}:5050/images/{5}/screenshot.png",
                        Properties.Settings.Default.ScreenOverlayAlt,
                        Properties.Settings.Default.ScreenOverlayAz,
                        Properties.Settings.Default.ScreenOverlayScale,
                        MyWebServer.IpAddress.ToString(),
                        Earth3d.MainWindow.Config.ClusterID.ToString(), frameNumber.ToString()));


                    if (Properties.Settings.Default.ScreenOverlayShowLocal)
                    {
                        if (Earth3d.MainWindow.RenderEngine.videoOverlay == null)
                        {
                            Earth3d.MainWindow.RenderEngine.videoOverlay = new ImageSetHelper("video", url, ImageSetType.Sky,
                                  BandPass.Visible, ProjectionType.SkyImage,
                                      Math.Abs(url.GetHashCode32()), 0, 0, 256, Properties.Settings.Default.ScreenOverlayScale / 1000,
                                      ".tif", false, "", Properties.Settings.Default.ScreenOverlayAz, Properties.Settings.Default.ScreenOverlayAlt, 0, false, "", false, false, 2,
                                      960, 600, "", "", "", "", 0, "");
                        }

                        Earth3d.MainWindow.RenderEngine.videoOverlay.CenterX = Properties.Settings.Default.ScreenOverlayAz;
                        Earth3d.MainWindow.RenderEngine.videoOverlay.CenterY = Properties.Settings.Default.ScreenOverlayAlt;
                        Earth3d.MainWindow.RenderEngine.videoOverlay.BaseTileDegrees = Properties.Settings.Default.ScreenOverlayScale / 1000;
                        Tile tile = TileCache.GetTile(0, 0, 0, Earth3d.MainWindow.RenderEngine.videoOverlay, null);
                        if (Dirty)
                        {
                            tile.CleanUpGeometryOnly();
                            Dirty = false;
                        }

                        tile.ReadyToRender = false;
                        tile.TextureReady = false;
                        tile.Volitile = true;
                    }
                    else if (Earth3d.MainWindow.RenderEngine.videoOverlay != null)
                    {
                        CleanUpImageSets();
                    }
                }
                else
                {
                    CleanUpImageSets();
                }
            }
        }

        private void Alt_TextChanged(object sender, EventArgs e)
        {
            bool valid = false;

            Properties.Settings.Default.ScreenOverlayAlt = UiTools.ParseAndValidateDouble(Alt, Properties.Settings.Default.ScreenOverlayAlt, ref valid);
            Dirty = true;
        }

        private void Az_TextChanged(object sender, EventArgs e)
        {
            bool valid = false;

            Properties.Settings.Default.ScreenOverlayAz = UiTools.ParseAndValidateDouble(Az, Properties.Settings.Default.ScreenOverlayAz, ref valid);
            Dirty = true;
        }

        private void Scale_TextChanged(object sender, EventArgs e)
        {
            bool valid = false;

            Properties.Settings.Default.ScreenOverlayScale = UiTools.ParseAndValidateDouble(Scale, Properties.Settings.Default.ScreenOverlayScale, ref valid);
            Dirty = true;
        }

        private void ShowLocally_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ScreenOverlayShowLocal = ShowLocally.Checked;
            Dirty = true;
            if (!Properties.Settings.Default.ScreenOverlayShowLocal)
            {
                CleanUpImageSets();
            }
        }

    }
}
