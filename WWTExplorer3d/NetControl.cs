using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using TerraViewer.Callibration;

namespace TerraViewer
{
    class NetControl
    {
        public delegate void MoveEvent(double lat, double lng, double zoom, double cameraRotate, double cameraAngle, int foregroundImageSetHash,
                                            int backgroundImageSetHash, float blendOpacity, bool runSetup, bool flush);

        public static MoveEvent MoveNotify;
        static NetControl()
        {
            MoveNotify = null;

        }
        private static Thread listenThread;
        private static Thread statusThread;
        private const int listenPort = 0;

        static UdpClient listener;

        public static void Abort()
        {
            running = false;
            if (listener != null)
            {
                listener.Close();
            }

            if (statusClient != null)
            {
                statusClient.Close();
            }
        }

        public static Dictionary<int, ClientNode> NodeList = new Dictionary<int, ClientNode>();

        public static void SaveNodeList()
        {
            ClientNodes.Save(NodeList, Properties.Settings.Default.CahceDirectory + "nodelist.xml");
        }

        public static void LoadNodeList()
        {
            var filename = Properties.Settings.Default.CahceDirectory + "nodelist.xml";
            if (File.Exists(filename))
            {
                NodeList = ClientNodes.Load(filename);
            }
        }

        public static bool NodeListDirty = false;

        public static string TargetName = "";
        public static int ErrorCount = 0;

        static bool running = true;
        static readonly AutoResetEvent sync = new AutoResetEvent(false);

        static bool FrameChanged()
        {
            return lastRederedFrame != currnetSyncFrame;
        }

        static int lastRederedFrame;
        public static int currnetSyncFrame = 0;
        public static void Start()
        {
            if (listener == null)
            {
                listenThread = new Thread(listenerThreadFunc);
                listenThread.Start();
            }

          

        }
        public static void StartStatusListner()
        {
            if (Settings.MasterController)
            {
                if (statusClient == null)
                {
                    statusThread = new Thread(statusThreadFunc);
                    statusThread.Start();
                }
            }
        }

        public static double domeTilt = Properties.Settings.Default.DomeTilt;
        public static double domeAngle = Properties.Settings.Default.DomeAngle;
        public static double domeAlt = 0;
        public static double domeAz = 0;
        public static int tileThrottling = 15;

        public static double lat = 0;
        public static double lng = 0;
        public static double zoom = 360;
        public static double cameraRotate = 0;
        public static double cameraAngle = 0;
        public static int foregroundImageSetHash = 0;
        public static int backgroundImageSetHash = 0;
        public static float blendOpacity = 0;
        public static bool runSetup = false;
        public static bool flush = false;

        public static int settingsFlags = 0;
        public static int solarSystemSettingsFlags = 0;
        public static Byte[] solarSystemData = new byte[13];
        public static Byte[] settingsData = new byte[35];
        public static Byte[] figuresFilter = new byte[12];
        public static Byte[] namesFilter = new byte[12];
        public static Byte[] bounderiesFilter = new byte[12];
        public static Byte[] artFilter = new byte[12];

        public static DateTime now = DateTime.Now;
        public static double timeRate = 1;
        public static double altitude = 0;
        public static double loclat = 1;
        public static double loclng = 1;

        public static Vector3d targetPoint = new Vector3d();
        public static SolarSystemObjects target;
        public static string TrackingFrame = "";
        public static int ColorVersionNumber = -1;
        public static int solarSystemScale = 100;
        public static double focusAltitude = 1;
        public static double reticleAlt = 0;
        public static double reticleAz = 0;


        public static string MasterAddress = "127.0.0.1";

        public static void WaitForNetworkSync()
        {


            if (Earth3d.MainWindow.Config.Master || currnetSyncFrame != lastRederedFrame)
            {
                lastRederedFrame = currnetSyncFrame;
                return;

            }


            lastRederedFrame = currnetSyncFrame;


        }
        static UdpClient statusClient;
        public static void statusThreadFunc()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            statusClient = new UdpClient(8091);
            var destinationEP = new IPEndPoint(IPAddress.Any, 8091);
            while (running)
            {
                try
                {
                    var bytes = statusClient.Receive(ref destinationEP);
                    if (bytes.Length > 2)
                    {
                        if (bytes[0] == 42 && bytes[1] == 42 && bytes[2] == 13)
                        {
                            var ms = new MemoryStream(bytes);
                            var br = new BinaryReader(ms);
                            // Eat the three leading bytes
                            br.ReadBytes(3);

                            if (br.ReadInt32() == Earth3d.MainWindow.Config.ClusterID)
                            {
                                var ip = destinationEP.Address.ToString();
                                var nodeID = br.ReadInt32();
                                var name = br.ReadString();
                                var status = br.ReadString();
                                var StatusText = br.ReadString();
                                var FPS = br.ReadSingle();
                                var error = br.ReadString();
                               
                                LogStatusReport(nodeID, name, ip, (ClientNodeStatus)Enum.Parse(typeof(ClientNodeStatus), status), StatusText, FPS, error);
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }


        public static void listenerThreadFunc()
        {

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            while (running)
            {
                GetClient();
                var destinationEP = new IPEndPoint(IPAddress.Any, Settings.MasterController ? 8088 : 8087);
                while (running)
                {
                    try
                    {
                        var bytes = listener.Receive(ref destinationEP);


                        if (bytes.Length > 2)
                        {
                            if (bytes[0] == 42 && bytes[1] == 42 && bytes[2] == 1 && !Settings.MasterController)
                            {
                                
                                var ms = new MemoryStream(bytes);
                                var br = new BinaryReader(ms);
                                // Eat the three leading bytes
                                br.ReadBytes(3);

                                if (br.ReadInt32() == Earth3d.MainWindow.Config.ClusterID)
                                {
                                    MasterAddress = destinationEP.Address.ToString();
                                    lat = br.ReadDouble();
                                    lng = br.ReadDouble();
                                    zoom = br.ReadDouble();
                                    cameraRotate = br.ReadDouble();
                                    cameraAngle = br.ReadDouble();
                                    foregroundImageSetHash = br.ReadInt32();
                                    backgroundImageSetHash = br.ReadInt32();
                                    blendOpacity = br.ReadSingle();
                                    settingsData = br.ReadBytes(35);
                                    flush = br.ReadBoolean();
                                    now = DateTime.FromBinary(br.ReadInt64());
                                    altitude = br.ReadDouble();
                                    loclat = br.ReadDouble();
                                    loclng = br.ReadDouble();
                                    solarSystemData = br.ReadBytes(13);
                                    target = (SolarSystemObjects)br.ReadInt32();
                                    targetPoint.X = br.ReadDouble();
                                    targetPoint.Y = br.ReadDouble();
                                    targetPoint.Z = br.ReadDouble();
                                    solarSystemScale = br.ReadInt32();
                                    focusAltitude = br.ReadDouble();
                                    timeRate = br.ReadDouble();
                                    domeTilt = br.ReadSingle();
                                    domeAngle = br.ReadSingle();
                                    domeAlt = br.ReadSingle();
                                    domeAz = br.ReadSingle();
                                    tileThrottling = br.ReadInt32();
                                    ColorVersionNumber = br.ReadInt32();
                                    TrackingFrame = br.ReadString();
                                    LayerManager.CurrentSlideID = br.ReadInt32();
                                    LayerManager.SlideTweenPosition = br.ReadSingle();                             
                                    Earth3d.masterSyncFrameNumber = br.ReadInt32();
                                    Earth3d.Logging = br.ReadBoolean();

                                    figuresFilter = br.ReadBytes(12);
                                    namesFilter = br.ReadBytes(12);
                                    bounderiesFilter = br.ReadBytes(12);
                                    artFilter = br.ReadBytes(12);

                                    int count = br.ReadByte();
                                    for (var i = 0; i < count; i++)
                                    {
                                        var enabled = br.ReadBoolean();
                                        int id = br.ReadByte();
                                        double alt = br.ReadSingle();
                                        double az = br.ReadSingle();
                                        var color = Color.FromArgb(br.ReadInt32());
                                        if (enabled)
                                        {
                                            Reticle.Set(id, alt, az, color);
                                        }
                                        else
                                        {
                                            Reticle.Hide(id, false);
                                        }
                                    }
                                    currnetSyncFrame++;

                                    //Check for restart
                                    if (LayerManager.CurrentSlideID > -1 && Earth3d.RestartedWithoutTour)
                                    {
                                        Earth3d.RestartedWithoutTour = false;
                                        Earth3d.MainWindow.SyncTourNeeded = true;
                                    }

                                    continue;
                                }
                                continue;
                            }
                            //Convert Byte to String
                            var sBuffer = Encoding.ASCII.GetString(bytes);
                            var values = sBuffer.Split(new[] { ',' });
                            if (values.Length > 1 && values[0] == "CAL" && Earth3d.MainWindow.Config.ClusterID.ToString() == values[1] && !Earth3d.MainWindow.Config.Master)
                            {
                                CalibrationScreen.ParseCommandString(values);
                                continue;
                            }
                            if (values.Length > 1 && values[0] == "SCREEN" && Earth3d.MainWindow.Config.ClusterID.ToString() == values[1] && !Earth3d.MainWindow.Config.Master)
                            {
                                double alt = Convert.ToSingle(values[2]);
                                double az = Convert.ToSingle(values[3]);
                                double scale = Convert.ToSingle(values[4]);
                                var url = values[5];

                                if (!string.IsNullOrEmpty(url))
                                {
                                    url = "http://" + MasterAddress + url.Substring(url.IndexOf(":5050"));


                                    if (Earth3d.MainWindow.videoOverlay == null)
                                    {
                                        Earth3d.MainWindow.videoOverlay = new ImageSetHelper("video", url, ImageSetType.Sky,
                                            BandPass.Visible, ProjectionType.SkyImage,
                                            Math.Abs(url.GetHashCode32()), 0, 0, 256, scale / 1000,
                                            ".tif", false, "", az, alt, 0, false, "", false, false, 2,
                                            960, 600, "", "", "", "", 0, "");
                                    }
                                    var dirty = false;
                                    if (Earth3d.MainWindow.videoOverlay.CenterX != az)
                                    {
                                        dirty = true;
                                    }
                                    if (Earth3d.MainWindow.videoOverlay.CenterY != alt)
                                    {
                                        dirty = true;
                                    }
                                    if (Earth3d.MainWindow.videoOverlay.BaseTileDegrees != scale / 1000)
                                    {
                                        dirty = true;
                                    }

                                    Earth3d.MainWindow.videoOverlay.CenterX = az;
                                    Earth3d.MainWindow.videoOverlay.CenterY = alt;
                                    Earth3d.MainWindow.videoOverlay.BaseTileDegrees = scale / 1000;
                                    var tile = TileCache.GetTile(0, 0, 0, Earth3d.MainWindow.videoOverlay, null);
                                    tile.ReadyToRender = false;
                                    tile.Volitile = true;
                                    tile.TextureReady = false;
                                    if (dirty)
                                    {
                                        TileCache.RemoveTile(tile);
                                    }

                                }
                                else
                                {
                                    if (Earth3d.MainWindow.videoOverlay != null)
                                    {
                                        var tile = TileCache.GetTile(0, 0, 0, Earth3d.MainWindow.videoOverlay, null);
                                        tile.CleanUp(false);
                                        TileCache.RemoveTile(tile);
                                    }

                                    Earth3d.MainWindow.videoOverlay = null;

                                }
                                continue;
                            }
                            if (values.Length > 1 && values[0] == "CONFIG" && Earth3d.MainWindow.Config.ClusterID.ToString() == values[1])
                            {
                                if (values.Length == 10)
                                {
                                    if (Convert.ToInt32(values[2]) == Earth3d.MainWindow.Config.NodeID)
                                    {
                                        Earth3d.MainWindow.Config.Heading = Convert.ToSingle(values[3]);
                                        Earth3d.MainWindow.Config.Pitch = Convert.ToSingle(values[4]);
                                        Earth3d.MainWindow.Config.Roll = Convert.ToSingle(values[5]);
                                        Earth3d.MainWindow.Config.UpFov = Convert.ToSingle(values[6]);
                                        Earth3d.MainWindow.Config.DownFov = Convert.ToSingle(values[7]);
                                        Earth3d.MainWindow.Config.Aspect = Convert.ToSingle(values[8]);
                                        Properties.Settings.Default.DomeTilt = Convert.ToSingle(values[9]);
                                        Earth3d.MainWindow.Config.DomeTilt = Convert.ToSingle(values[9]);
                                        Earth3d.MainWindow.Config.MultiChannelDome1 = true;
                                    }
                                }
                            }
                            if (values.Length > 1 && values[0] == "SYNCLAYERS" && Earth3d.MainWindow.Config.ClusterID.ToString() == values[1] && !Earth3d.MainWindow.Config.Master)
                            {
                                SyncLayers();
                                continue;
                            }

                            if (values.Length > 1 && values[0] == "SYNCTOUR" && Earth3d.MainWindow.Config.ClusterID.ToString() == values[1] && !Earth3d.MainWindow.Config.Master)
                            {
                                SyncTour();
                                continue;
                            }

                            if (values.Length == 27 && !Settings.MasterController)
                            {
                               
                                if (values[0] == "SYNC" && Earth3d.MainWindow.Config.ClusterID.ToString() == values[1])
                                {
                                    MasterAddress = destinationEP.Address.ToString();
                                    lat = Convert.ToDouble(values[2]);
                                    lng = Convert.ToDouble(values[3]);
                                    zoom = Convert.ToDouble(values[4]);
                                    cameraRotate = Convert.ToDouble(values[5]);
                                    cameraAngle = Convert.ToDouble(values[6]);
                                    foregroundImageSetHash = Convert.ToInt32(values[7]);
                                    backgroundImageSetHash = Convert.ToInt32(values[8]);
                                    blendOpacity = Convert.ToSingle(values[9]);
                                    settingsFlags = Convert.ToInt32(values[10]);
                                    flush = Convert.ToBoolean(values[11]);
                                    now = DateTime.Parse(values[12]);
                                    altitude = Convert.ToDouble(values[13]);
                                    loclat = Convert.ToDouble(values[14]);
                                    loclng = Convert.ToDouble(values[15]);
                                    solarSystemSettingsFlags = Convert.ToInt32(values[16]);
                                    target = (SolarSystemObjects)Convert.ToInt32(values[17]);
                                    targetPoint.X = Convert.ToDouble(values[18]);
                                    targetPoint.Y = Convert.ToDouble(values[19]);
                                    targetPoint.Z = Convert.ToDouble(values[20]);
                                    solarSystemScale = Convert.ToInt32(values[21]);
                                    focusAltitude = Convert.ToDouble(values[22]);
                                    timeRate = Convert.ToDouble(values[23]);
                                    domeTilt = Convert.ToDouble(values[24]);
                                    TrackingFrame = values[25];
                                    ColorVersionNumber = int.Parse(values[26]);
                                    currnetSyncFrame++;
                                    sync.Set();
                                }

                            }
                            else if (values.Length == 3)
                            {
                                var leftRight = Convert.ToDouble(values[0]);
                                var upDown = Convert.ToDouble(values[1]);
                                var zoom = Convert.ToDouble(values[2]);
                                Earth3d.MainWindow.MoveAndZoom(leftRight, upDown, zoom);
                            }
                            else if (values.Length == 4)
                            {
                                var leftRight = Convert.ToDouble(values[0]);
                                var upDown = Convert.ToDouble(values[1]);
                                var zoom = Convert.ToDouble(values[2]);
                                var name = values[3];
                                Earth3d.MainWindow.MoveAndZoomRate(leftRight, upDown, zoom, "");
                            }
                            else if (values.Length == 5)
                            {
                                Earth3d.MainWindow.Config.Heading = Convert.ToSingle(values[0]);
                                Earth3d.MainWindow.Config.Pitch = Convert.ToSingle(values[1]);
                                Earth3d.MainWindow.Config.Roll = Convert.ToSingle(values[2]);
                                Earth3d.MainWindow.Config.UpFov = Convert.ToSingle(values[3]);
                                Earth3d.MainWindow.Config.DownFov = Convert.ToSingle(values[4]);
                            }
                            else if (values.Length == 6)
                            {
                                if (Convert.ToInt32(values[5]) == Earth3d.MainWindow.Config.NodeID)
                                {
                                    Earth3d.MainWindow.Config.Heading = Convert.ToSingle(values[0]);
                                    Earth3d.MainWindow.Config.Pitch = Convert.ToSingle(values[1]);
                                    Earth3d.MainWindow.Config.Roll = Convert.ToSingle(values[2]);
                                    Earth3d.MainWindow.Config.UpFov = Convert.ToSingle(values[3]);
                                    Earth3d.MainWindow.Config.DownFov = Convert.ToSingle(values[4]);
                                }
                            }
                            else if (values.Length == 8)
                            {
                                var leftRight = Convert.ToDouble(values[0]);
                                var upDown = Convert.ToDouble(values[1]);
                                var zoom = Convert.ToDouble(values[2]);
                                var name = values[3];
                                var dome = Convert.ToBoolean(values[4]);
                                var domeTilt = Convert.ToDouble(values[5]);
                                var viewTilt = Convert.ToDouble(values[6]);
                                var mode = values[7];
                                Earth3d.MainWindow.CameraAngle = viewTilt;
                                if (Properties.Settings.Default.DomeView != dome)
                                {
                                    Properties.Settings.Default.DomeView = dome;
                                    Settings.DomeView = false;
                                }

                                if (Properties.Settings.Default.DomeTilt != domeTilt)
                                {
                                    Properties.Settings.Default.DomeTilt = domeTilt;
                                }

                                Earth3d.MainWindow.MoveAndZoomRate(leftRight, upDown, zoom, mode);

                                if (!String.IsNullOrEmpty(name))
                                {
                                    Earth3d.MainWindow.SetBackgroundByName(name);
                                }
                            }
                            else if ((values.Length == 15 ) && values[0] == "Kinect" && Earth3d.MainWindow.Config.ClusterID.ToString() == values[1])
                            {
                                var leftRight = Convert.ToDouble(values[2]);
                                var upDown = Convert.ToDouble(values[3]);
                                var zoom = Convert.ToDouble(values[4]);
                                var name = values[5];
                                var dome = Convert.ToBoolean(values[6]);
                                var domeTilt = Convert.ToDouble(values[7]);
                                var viewTilt = Convert.ToDouble(values[8]);
                                var mode = values[9];
                                Earth3d.MainWindow.CameraAngleTarget = viewTilt;
                                if (Properties.Settings.Default.DomeView != dome)
                                {
                                    Properties.Settings.Default.DomeView = dome;
                                }

                                if (Properties.Settings.Default.DomeTilt != domeTilt)
                                {
                                    Properties.Settings.Default.DomeTilt = domeTilt;
                                }

                                Earth3d.MainWindow.MoveAndZoomRate(leftRight, upDown, zoom, mode);

                                if (!String.IsNullOrEmpty(name))
                                {
                                    Earth3d.MainWindow.SetBackgroundByName(name);
                                }

                                var retId = int.Parse(values[10]);
                                var state = Boolean.Parse(values[11]);
                                var alt = Convert.ToDouble(values[12]);
                                var az = Convert.ToDouble(values[13]);
                                var color = Color.White;
                                try
                                {
                                    color = Color.FromArgb(int.Parse(values[14]));
                                }
                                catch
                                {
                                }

                                if (state)
                                {
                                    Reticle.Set(retId, alt, az, color);
                                }
                                else
                                {
                                    Reticle.Hide(retId, false);
                                }
                            }
                            else if ((values.Length == 18) && values[0] == "Kinect" && Earth3d.MainWindow.Config.ClusterID.ToString() == values[1])
                            {
                                var leftRight = Convert.ToDouble(values[2]);
                                var upDown = Convert.ToDouble(values[3]);
                                var zoom = Convert.ToDouble(values[4]);
                                var name = values[5];
                                var dome = Convert.ToBoolean(values[6]);
                                var domeTilt = Convert.ToDouble(values[7]);
                                var viewTilt = Convert.ToDouble(values[8]);
                                var mode = values[9];
                                Earth3d.MainWindow.CameraAngleTarget = viewTilt;
                                if (Properties.Settings.Default.DomeView != dome)
                                {
                                    Properties.Settings.Default.DomeView = dome;
                                }

                                if (Properties.Settings.Default.DomeTilt != domeTilt)
                                {
                                    Properties.Settings.Default.DomeTilt = domeTilt;
                                }

                                Earth3d.MainWindow.MoveAndZoomRate(leftRight, upDown, zoom, mode);

                                if (!String.IsNullOrEmpty(name))
                                {
                                    Earth3d.MainWindow.SetBackgroundByName(name);
                                }

                                var retId = int.Parse(values[10]);
                                var state = Boolean.Parse(values[11]);
                                var alt = Convert.ToDouble(values[12]);
                                var az = Convert.ToDouble(values[13]);
                                var color = Color.White;
                                try
                                {
                                    color = Color.FromArgb(int.Parse(values[14]));
                                }
                                catch
                                {
                                }

                                if (state)
                                {
                                    Reticle.Set(retId, alt, az, color);
                                }
                                else
                                {
                                    Reticle.Hide(retId, false);
                                }

                                Earth3d.MainWindow.SetHeadPosition(new Vector3d(-double.Parse(values[15])*2, -double.Parse(values[16])*2, (double.Parse(values[17])-1.5))*6);
                            }

                        }
                    }
                    catch
                    {
                        if (Earth3d.Logging) { Earth3d.WriteLogMessage("NetControl: Exception on receive"); }
                        if (!running)
                        {
                            return;
                        }
                    }
                }
                listener.Close();
                listener = null;
            }
        }

        private static void SyncLayers()
        {
            Earth3d.MainWindow.SyncLayerNeeded = true;
        }

        private static void SyncTour()
        {
            Earth3d.MainWindow.SyncTourNeeded = true;
        }

        public static void SyncLayersUiThread()
        {

            // Synce Layers
            try
            {
                ReportStatus(ClientNodeStatus.Working, "Loading Layers", "");
                var layerClient = new WebClient();
                var url = string.Format("http://{0}:5050/Configuration/images/layermap", MasterAddress);
                layerClient.DownloadDataCompleted += layerClient_DownloadDataCompleted;
                layerClient.DownloadDataAsync(new Uri(url));

            }
            catch
            {
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("Exception loading Layers"); }
            }
            Earth3d.MainWindow.SyncLayerNeeded = false;
        }

        static void layerClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    var filename = Properties.Settings.Default.CahceDirectory + "\\serverlayers.layers";
                    var data = e.Result;
                    File.WriteAllBytes(filename, data);
                    LayerManager.InitLayers();
                    var layers = LayerContainer.FromFile(filename, false, null, false);
                    layers.Dispose();
                    ReportStatus(ClientNodeStatus.Online, "Layers Loaded", "");
                }
                catch
                {
                    ReportStatus(ClientNodeStatus.Online, "Layers Threw Exception", "Layer Load Failed :" + e.Error.Message);
                }
            }
            else
            {
                ReportStatus(ClientNodeStatus.Online, "Layers Loaded", "Layer Load Failed:" + e.Error.Message);
            }
        }

        public static void SyncTourUiThread()
        {
            var client = new WebClient();
            // Synce Layers
            try
            {
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("SyncTourUIThread"); }

                ReportStatus(ClientNodeStatus.Working, "Loading Tour", "");

                var url = string.Format("http://{0}:5050/Configuration/images/tour.wtt", MasterAddress);


                try
                {
                    var filename = Properties.Settings.Default.CahceDirectory + "\\servertour.wtt";
               
                    client.DownloadFile(new Uri(url), filename);
                    var oldPlayer = Earth3d.MainWindow.UiController as TourPlayer;

                    if (oldPlayer != null)
                    {
                        LayerManager.CloseAllTourLoadedLayers();
                        oldPlayer.Tour.CleanUp();
                        oldPlayer.Tour.ClearTempFiles();
                    }

                    var tour = TourDocument.FromFile(filename, false);
                    tour.ProjectorServer = true;
                    var player = new TourPlayer();
                    player.Tour = tour;
                    player.ProjectorServer = true;
                    player.Play();
                    Earth3d.MainWindow.UiController = player;
                    ReportStatus(ClientNodeStatus.Online, "Tour Loaded", "");
                }
                catch
                {
                    ReportStatus(ClientNodeStatus.Online, "Tour Threw Exception", "Tour Load Failed :" );
                }
            }
            catch
            {
                ReportStatus(ClientNodeStatus.Online, "Layers Loaded", "Layer Load Failed:");
            }
            Earth3d.MainWindow.SyncTourNeeded = false;
        }

        static void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    var data = e.Result;
                    var filename = Properties.Settings.Default.CahceDirectory + "\\servertour.wtt";
                    File.WriteAllBytes(filename, data);
                    var oldPlayer = Earth3d.MainWindow.UiController as TourPlayer;

                    if (oldPlayer != null)
                    {
                        LayerManager.CloseAllTourLoadedLayers();
                        oldPlayer.Tour.CleanUp();
                        oldPlayer.Tour.ClearTempFiles();
                    }

                    var tour = TourDocument.FromFile(filename, false);
                    tour.ProjectorServer = true;
                    var player = new TourPlayer();
                    player.Tour = tour;
                    player.ProjectorServer = true;
                    player.Play();
                    Earth3d.MainWindow.UiController = player;
                    ReportStatus(ClientNodeStatus.Online, "Tour Loaded", "");
                }
                catch
                {
                    ReportStatus(ClientNodeStatus.Online, "Tour Threw Exception", "Tour Load Failed :" + e.Error.Message);
                }
            }
            else
            {
                ReportStatus(ClientNodeStatus.Online, "Tour Failed", "Tour Load Failed :" + e.Error.Message);
            }
        }

        private static void GetClient()
        {
            try
            {
                if (Settings.MasterController)
                {
                    listener = new UdpClient(8088);
                }
                else
                {
                    listener = new UdpClient(8087);
                }
            }
            catch
            {
                listener = new UdpClient(8089);
            }
        }
        static Socket sockA;
        static Socket sockB = null;
        static DateTime lastMessage = DateTime.Now;

        public static void SendMove(double lat, double lng, double zoom, double cameraRotate, double cameraAngle, int fgImageHash, int bkImageHash, float blendOpacity, bool runSetup, bool autoFlush, bool altAzMode, int target, Vector3d targetPoint, int SolarSystemScale, double focusAltitude, string trackingFrame)
        {
            double timeRate = 0;


            var ts = DateTime.Now.Subtract(lastMessage);

            //if (ts.Milliseconds < 50)
            //{
            //    return;
            //}
            lastMessage = DateTime.Now;

            timeRate = SpaceTimeController.SyncToClock ? SpaceTimeController.TimeRate : 0;

            var settingsFlags = GetSettingsFlags();
            var solarSystemFlags = GetSolarSystemSettingsFlags();

            var output =
                "SYNC"
                + "," + Earth3d.MainWindow.Config.ClusterID
                + "," + lat
                + "," + lng
                + "," + zoom
                + "," + cameraRotate
                + "," + cameraAngle
                + "," + fgImageHash
                + "," + bkImageHash
                + "," + blendOpacity
                + "," + settingsFlags
                + "," + autoFlush
                + "," + SpaceTimeController.Now.ToString("MM/dd/yyyy hh:mm:ss.FFFFF tt")
                + "," + SpaceTimeController.Altitude
                + "," + SpaceTimeController.Location.Lat
                + "," + SpaceTimeController.Location.Lng
                + "," + solarSystemFlags
                + "," + target
                + "," + targetPoint.X
                + "," + targetPoint.Y
                + "," + targetPoint.Z
                + "," + SolarSystemScale
                + "," + focusAltitude
                + "," + timeRate
                + "," + Earth3d.MainWindow.Config.DomeTilt
                + "," + trackingFrame
                + "," + Properties.Settings.Default.ColSettingsVersion;
            SendCommand(output);
        }

        static DateTime lastHeartBeatCheck = DateTime.Now;

        private static void CheckHeartbeat()
        {
            var ts = DateTime.Now - lastHeartBeatCheck;

            if (ts.TotalSeconds > 15)
            {
                foreach (var node in NodeList.Values)
                {
                    if (node.Status != ClientNodeStatus.Offline)
                    {
                        var timeSinceContact = DateTime.Now - node.LastReport;
                        if (timeSinceContact.TotalSeconds > 30)
                        {
                            node.Status = ClientNodeStatus.Offline;
                            node.LastFPS = 0;
                            node.StatusText = "Lost Contact";
                            NodeListDirty = true;
                        }
                    }
                }

                lastHeartBeatCheck = DateTime.Now;
            }
        }

        public static void SendMoveBinary(double lat, double lng, double zoom, double cameraRotate, double cameraAngle, int foregroundImageSetHash, int backgroundImageSetHash, float blendOpacity, bool runSetup, bool autoFlush, bool altAzMode, int target, Vector3d targetPoint, int SolarSystemScale, double focusAltitude, string trackingFrame, double reticleAlt, double reticleAz)
        {
            CheckHeartbeat();

            var stream = new MemoryStream();

            var bw = new BinaryWriter(stream);
            double timeRate = 0;


            var ts = DateTime.Now.Subtract(lastMessage);

            lastMessage = DateTime.Now;

            timeRate = SpaceTimeController.SyncToClock ? SpaceTimeController.TimeRate : 0;

            var settingsData = GetSettingsBlendStates();
            var solarSystemData = GetSolarSystemSettingsBlendStates();
            bw.Write((byte)42);
            bw.Write((byte)42);
            bw.Write((byte)1); // "SYNC" packet
            bw.Write(Earth3d.MainWindow.Config.ClusterID);
            bw.Write(lat);
            bw.Write(lng);
            bw.Write(zoom);
            bw.Write(cameraRotate);
            bw.Write(cameraAngle);
            bw.Write(foregroundImageSetHash);
            bw.Write(backgroundImageSetHash);
            bw.Write(blendOpacity);
            bw.Write(settingsData);
            bw.Write(autoFlush);
            bw.Write(SpaceTimeController.Now.ToBinary());
            bw.Write(SpaceTimeController.Altitude);
            bw.Write(SpaceTimeController.Location.Lat);
            bw.Write(SpaceTimeController.Location.Lng);
            bw.Write(solarSystemData);
            bw.Write(target);
            bw.Write(targetPoint.X);
            bw.Write(targetPoint.Y);
            bw.Write(targetPoint.Z);
            bw.Write(SolarSystemScale);
            bw.Write(focusAltitude);
            bw.Write(timeRate);
            bw.Write(Earth3d.MainWindow.Config.DomeTilt);
            bw.Write(Earth3d.MainWindow.Config.DomeAngle);
            bw.Write((float)Earth3d.MainWindow.viewCamera.DomeAlt);
            bw.Write((float)Earth3d.MainWindow.viewCamera.DomeAz);
            bw.Write(Properties.Settings.Default.TileThrottling);
            bw.Write(Properties.Settings.Default.ColSettingsVersion);
            bw.Write(trackingFrame); //todo consider making this a hash id
            bw.Write(LayerManager.CurrentSlideID);
            bw.Write(LayerManager.SlideTweenPosition);      
            bw.Write(Earth3d.FrameNumber);
            bw.Write(Earth3d.Logging);
            bw.Write(Properties.Settings.Default.ConstellationFiguresFilter.GetBits());
            bw.Write(Properties.Settings.Default.ConstellationNamesFilter.GetBits());
            bw.Write(Properties.Settings.Default.ConstellationBoundariesFilter.GetBits());
            bw.Write(Properties.Settings.Default.ConstellationArtFilter.GetBits());

            var count = Reticle.Reticles.Count;
            bw.Write((byte)count);

            foreach (var reticle in Reticle.Reticles.Values)
            {
                bw.Write(reticle.Visible.TargetState);
                bw.Write((byte)reticle.Id);
                bw.Write((Single)reticle.Alt);
                bw.Write((Single)reticle.Az);
                bw.Write(reticle.Color.ToArgb());
            }

            bw.Flush();

            SendCommand(stream.GetBuffer(), (int)stream.Length);
        }

        public static int GetSolarSystemSettingsFlags()
        {
            var flags = 0;
            flags += Settings.Active.ShowSolarSystem ? 1 : 0;
            flags += Settings.Active.SolarSystemStars ? 2 : 0;
            flags += Settings.Active.SolarSystemMilkyWay ? 4 : 0;
            flags += Settings.Active.SolarSystemCosmos ? 8 : 0;
            flags += Settings.Active.SolarSystemMinorPlanets ? 16 : 0;
            flags += Settings.Active.SolarSystemOrbits ? 32 : 0;
            flags += Settings.Active.SolarSystemLighting ? 64 : 0;
            flags += Settings.Active.SolarSystemMultiRes ? 128 : 0;
            flags += Settings.Active.SolarSystemPlanets ? 256 : 0;
            flags += Settings.Active.SolarSystemMinorOrbits ? 512 : 0;
            return flags;
        }

        public static byte[] GetSolarSystemSettingsBlendStates()
        {
            var data = new byte[13];
            data[0] = (byte)(Properties.Settings.Default.ShowSolarSystem.Opacity * 255);
            data[1] = (byte)(Properties.Settings.Default.SolarSystemStars.Opacity * 255);
            data[2] = (byte)(Properties.Settings.Default.SolarSystemMilkyWay.Opacity * 255);
            data[3] = (byte)(Properties.Settings.Default.SolarSystemCosmos.Opacity * 255);
            data[4] = (byte)(Properties.Settings.Default.SolarSystemMinorPlanets.Opacity * 255);
            data[5] = (byte)(Properties.Settings.Default.SolarSystemOrbits.Opacity * 255);
            data[6] = (byte)(Properties.Settings.Default.SolarSystemLighting ? 1 : 0);
            data[7] = (byte)(Properties.Settings.Default.SolarSystemMultiRes ? 1 : 0);
            data[8] = (byte)(Properties.Settings.Default.SolarSystemPlanets.Opacity * 255);
            data[9] = (byte)(Properties.Settings.Default.SolarSystemMinorOrbits.Opacity * 255);
            data[10] = (byte)(Properties.Settings.Default.ShowISSModel ? 1 : 0);
            data[11] = (byte)(Properties.Settings.Default.SolarSystemCMB.Opacity * 255);
            data[12] = (byte)(Properties.Settings.Default.Show3dCities ? 1 : 0);
           return data;
        }
        static int currentSolarSystemSettingsFlags = -1;

        internal static void SetSolarSystemsSettingsFlags()
        {
            if (solarSystemSettingsFlags == currentSolarSystemSettingsFlags)
            {
                return;
            }
            currentSolarSystemSettingsFlags = solarSystemSettingsFlags;

            if (Properties.Settings.Default.ShowSolarSystem.TargetState != ((solarSystemSettingsFlags & 1) == 1))
            {
                Properties.Settings.Default.ShowSolarSystem.TargetState = (solarSystemSettingsFlags & 1) == 1;
            }
            if (Properties.Settings.Default.SolarSystemStars.TargetState != ((solarSystemSettingsFlags & 2) == 2))
            {
                Properties.Settings.Default.SolarSystemStars.TargetState = (solarSystemSettingsFlags & 2) == 2;
            }
            if (Properties.Settings.Default.SolarSystemMilkyWay.TargetState != ((solarSystemSettingsFlags & 4) == 4))
            {
                Properties.Settings.Default.SolarSystemMilkyWay.TargetState = (solarSystemSettingsFlags & 4) == 4;
            }
            if (Properties.Settings.Default.SolarSystemCosmos.TargetState != ((solarSystemSettingsFlags & 8) == 8))
            {
                Properties.Settings.Default.SolarSystemCosmos.TargetState = (solarSystemSettingsFlags & 8) == 8;
            }
            if (Properties.Settings.Default.SolarSystemMinorPlanets.TargetState != ((solarSystemSettingsFlags & 16) == 16))
            {
                Properties.Settings.Default.SolarSystemMinorPlanets.TargetState = (solarSystemSettingsFlags & 16) == 16;
            }
            if (Properties.Settings.Default.SolarSystemOrbits.TargetState != ((solarSystemSettingsFlags & 32) == 32))
            {
                Properties.Settings.Default.SolarSystemOrbits.TargetState = (solarSystemSettingsFlags & 32) == 32;
            }
            if (Properties.Settings.Default.SolarSystemLighting != ((solarSystemSettingsFlags & 64) == 64))
            {
                Properties.Settings.Default.SolarSystemLighting = (solarSystemSettingsFlags & 64) == 64;
            }
            if (Properties.Settings.Default.SolarSystemMultiRes != ((solarSystemSettingsFlags & 128) == 128))
            {
                Properties.Settings.Default.SolarSystemMultiRes = (solarSystemSettingsFlags & 128) == 128;
            }
            if (Properties.Settings.Default.SolarSystemPlanets.TargetState != ((solarSystemSettingsFlags & 256) == 256))
            {
                Properties.Settings.Default.SolarSystemPlanets.TargetState = (solarSystemSettingsFlags & 256) == 256;
            }
            if (Properties.Settings.Default.SolarSystemMinorOrbits.TargetState != ((solarSystemSettingsFlags & 512) == 512))
            {
                Properties.Settings.Default.SolarSystemMinorOrbits.TargetState = (solarSystemSettingsFlags & 512) == 512;
            }
            
        }

        static readonly byte[] currentSolarSystemBlendStates = new byte[13];

        internal static void SetSolarSystemsSettingsBlendStates()
        {
            var data = solarSystemData;
            var changed = false;

            for (var i = 0; i < 13; i++)
            {
                if (data[i] != currentSolarSystemBlendStates[i])
                {
                    changed = true;
                }
                currentSolarSystemBlendStates[i] = data[i];
            }

            if (!changed)
            {
                return;
            }
            Properties.Settings.Default.ShowSolarSystem.Opacity = data[0] / 255.0f;
            Properties.Settings.Default.SolarSystemStars.Opacity = data[1] / 255.0f;
            Properties.Settings.Default.SolarSystemMilkyWay.Opacity = data[2] / 255.0f;
            Properties.Settings.Default.SolarSystemCosmos.Opacity = data[3] / 255.0f;
            Properties.Settings.Default.SolarSystemMinorPlanets.Opacity = data[4] / 255.0f;
            Properties.Settings.Default.SolarSystemOrbits.Opacity = data[5] / 255.0f;

            if (Properties.Settings.Default.SolarSystemLighting != (data[6] == 0 ? false : true))
            {
                Properties.Settings.Default.SolarSystemLighting = (data[6] == 0 ? false : true);
            }

            if (Properties.Settings.Default.SolarSystemMultiRes != (data[7] == 0 ? false : true))
            {

                Properties.Settings.Default.SolarSystemMultiRes = (data[7] == 0 ? false : true);
            }

            Properties.Settings.Default.SolarSystemPlanets.Opacity = data[8] / 255.0f;
            Properties.Settings.Default.SolarSystemMinorOrbits.Opacity = data[9] / 255.0f;
            
 
            if (Properties.Settings.Default.ShowISSModel != (data[10] == 0 ? false : true))
            {
                Properties.Settings.Default.ShowISSModel = (data[10] == 0 ? false : true);
            }

            Properties.Settings.Default.SolarSystemCMB.Opacity = data[11] / 255.0f;
 
            if (Properties.Settings.Default.Show3dCities != (data[12] == 0 ? false : true))
            {
                TileCache.PurgeQueue();
                TileCache.ClearCache();
                Properties.Settings.Default.Show3dCities = (data[12] == 0 ? false : true);
            }

        }


        public static int GetSettingsFlags()
        {
            var flags = 0;
            flags += Settings.Active.ShowElevationModel ? 1 : 0;
            flags += Properties.Settings.Default.LineSmoothing ? 2 : 0;
            flags += Settings.Active.LocalHorizonMode ? 4 : 0;
            flags += Settings.Active.ShowClouds ? 8 : 0;
            flags += Settings.Active.ShowConstellationBoundries ? 16 : 0;
            flags += Settings.Active.ShowConstellationFigures ? 32 : 0;
            flags += Settings.Active.ShowConstellationSelection ? 64 : 0;
            flags += Settings.Active.ShowFieldOfView ? 128 : 0;
            flags += Settings.Active.ShowGrid ? 256 : 0;
            flags += Settings.Active.ShowHorizon ? 512 : 0;
            flags += Settings.Active.ShowEcliptic ? 1024 : 0;
            flags += Settings.Active.ShowGalacticGrid ? 2048 : 0;
            flags += Settings.Active.ShowGalacticGridText ? 4096 : 0;
            flags += Settings.Active.ShowEclipticGrid ? 8192 : 0;
            flags += Settings.Active.ShowEclipticGridText ? 16384 : 0;
            flags += Settings.Active.ShowEclipticOverviewText ? 32768 : 0;
            flags += Settings.Active.ShowAltAzGrid ? 65536 : 0;
            flags += Settings.Active.ShowAltAzGridText ? 131072 : 0;
            flags += Settings.Active.ShowPrecessionChart ? 262144 : 0;
            flags += Settings.Active.ShowConstellationPictures ? 524288 : 0;
            flags += Settings.Active.ShowConstellationLabels ? 1048576 : 0;
            flags += Earth3d.MainWindow.Fader.TargetState ? 2097152 : 0;
            flags += Settings.Active.ShowEquatorialGridText ? 4194304 : 0;
            flags += Properties.Settings.Default.ShowSkyOverlays.TargetState ? 8388608 : 0;
            flags += Properties.Settings.Default.Constellations.TargetState ? 16777216 : 0;
            flags += Properties.Settings.Default.ShowSkyNode.TargetState ? 33554432 : 0;
            flags += Properties.Settings.Default.ShowSkyGrids.TargetState ? 67108864 : 0;
            flags += Properties.Settings.Default.ShowSkyOverlaysIn3d.TargetState ? 134217728 : 0;
            flags += Properties.Settings.Default.MultiSampling != 1 ? 268435456 : 0;

            return flags;
        }

        public static byte[] GetSettingsBlendStates()
        {
            var data = new byte[35];
            data[0] = (byte)(Properties.Settings.Default.ShowElevationModel == true ? 1 : 0);
            data[1] = (byte)(Properties.Settings.Default.LineSmoothing == true ? 1 : 0);
            data[2] = (byte)(Properties.Settings.Default.LocalHorizonMode ? 1 : 0);
            data[3] = (byte)(Properties.Settings.Default.ShowClouds.Opacity * 255);
            data[4] = (byte)(Properties.Settings.Default.ShowConstellationBoundries.Opacity * 255);
            data[5] = (byte)(Properties.Settings.Default.ShowConstellationFigures.Opacity * 255);
            data[6] = (byte)(Properties.Settings.Default.ShowConstellationSelection.Opacity * 255);
            data[7] = (byte)(Properties.Settings.Default.ShowFieldOfView.Opacity * 255);
            data[8] = (byte)(Properties.Settings.Default.ShowGrid.Opacity * 255);
            data[9] = (byte)(Properties.Settings.Default.ShowHorizon == true ? 1 : 0);
            data[10] = (byte)(Properties.Settings.Default.ShowEcliptic.Opacity * 255);
            data[11] = (byte)(Properties.Settings.Default.ShowGalacticGrid.Opacity * 255);
            data[12] = (byte)(Properties.Settings.Default.ShowGalacticGridText.Opacity * 255);
            data[13] = (byte)(Properties.Settings.Default.ShowEclipticGrid.Opacity * 255);
            data[14] = (byte)(Properties.Settings.Default.ShowEclipticGridText.Opacity * 255);
            data[15] = (byte)(Properties.Settings.Default.ShowEclipticOverviewText.Opacity * 255);
            data[16] = (byte)(Properties.Settings.Default.ShowAltAzGrid.Opacity * 255);
            data[17] = (byte)(Properties.Settings.Default.ShowAltAzGridText.Opacity * 255);
            data[18] = (byte)(Properties.Settings.Default.ShowPrecessionChart.Opacity * 255);
            data[19] = (byte)(Properties.Settings.Default.ShowConstellationPictures.Opacity * 255);
            data[20] = (byte)(Properties.Settings.Default.ShowConstellationLabels.Opacity * 255);
            data[21] = (byte)(Earth3d.MainWindow.Fader.Opacity * 255);
            data[22] = (byte)(Properties.Settings.Default.ShowEquatorialGridText.Opacity * 255);
            data[23] = (byte)(Properties.Settings.Default.ShowSkyOverlays.Opacity * 255);
            data[24] = (byte)(Properties.Settings.Default.Constellations.Opacity * 255);
            data[25] = (byte)(Properties.Settings.Default.ShowSkyNode.Opacity * 255);
            data[26] = (byte)(Properties.Settings.Default.ShowSkyGrids.Opacity * 255);
            data[27] = (byte)(Properties.Settings.Default.ShowSkyOverlaysIn3d.Opacity * 255);
            data[28] = (byte)(Properties.Settings.Default.MultiSampling);
            data[29] = (byte)(Properties.Settings.Default.ShowReticle.Opacity * 255);
            data[30] = (byte)(Properties.Settings.Default.EarthCutawayView.Opacity * 255);
            data[31] = (byte)(Properties.Settings.Default.FrameSync == true ? 1 : 0);
            data[32] = (byte)(Properties.Settings.Default.TargetFrameRate);
            data[33] = (byte)(Properties.Settings.Default.MilkyWayModel == true ? 1 : 0);
            data[34] = (byte)(Properties.Settings.Default.GalacticMode == true ? 1 : 0);
            return data;
        }

        static int currentSettingsFlags = -1;

        internal static void SetSettingsFlags()
        {
            if (currentSettingsFlags == settingsFlags)
            {
                return;
            }
            if (Settings.Active.ShowElevationModel != ((settingsFlags & 1) == 1))
            {
                TileCache.PurgeQueue();
                TileCache.ClearCache();
                Properties.Settings.Default.ShowElevationModel = (settingsFlags & 1) == 1;
            }

            if (Properties.Settings.Default.LineSmoothing != ((settingsFlags & 2) == 2))
            {
                Properties.Settings.Default.LineSmoothing = (settingsFlags & 2) == 2;
            }
            if (Properties.Settings.Default.LocalHorizonMode != ((settingsFlags & 4) == 4))
            {
                Properties.Settings.Default.LocalHorizonMode = (settingsFlags & 4) == 4;
            }
            if (Properties.Settings.Default.ShowClouds.TargetState != ((settingsFlags & 8) == 8))
            {
                Properties.Settings.Default.ShowClouds.TargetState = (settingsFlags & 8) == 8;
            }
            if (Properties.Settings.Default.ShowConstellationBoundries.TargetState != ((settingsFlags & 16) == 16))
            {
                Properties.Settings.Default.ShowConstellationBoundries.TargetState = (settingsFlags & 16) == 16;
            }
            if (Properties.Settings.Default.ShowConstellationFigures.TargetState != ((settingsFlags & 32) == 32))
            {
                Properties.Settings.Default.ShowConstellationFigures.TargetState = (settingsFlags & 32) == 32;
            }
            if (Properties.Settings.Default.ShowConstellationSelection.TargetState != ((settingsFlags & 64) == 64))
            {
                Properties.Settings.Default.ShowConstellationSelection.TargetState = (settingsFlags & 64) == 64;
            }
            if (Properties.Settings.Default.ShowFieldOfView.TargetState != ((settingsFlags & 128) == 128))
            {
                Properties.Settings.Default.ShowFieldOfView.TargetState = (settingsFlags & 128) == 128;
            }
            if (Properties.Settings.Default.ShowGrid.TargetState != ((settingsFlags & 256) == 256))
            {
                Properties.Settings.Default.ShowGrid.TargetState = (settingsFlags & 256) == 256;
            }
            if (Properties.Settings.Default.ShowHorizon != ((settingsFlags & 512) == 512))
            {
                Properties.Settings.Default.ShowHorizon = (settingsFlags & 512) == 512;
            }
            if (Properties.Settings.Default.ShowEcliptic.TargetState != ((settingsFlags & 1024) == 1024))
            {
                Properties.Settings.Default.ShowEcliptic.TargetState = (settingsFlags & 1024) == 1024;
            }

            if (Properties.Settings.Default.ShowGalacticGrid.TargetState != ((settingsFlags & 2048) == 2048))
            {
                Properties.Settings.Default.ShowGalacticGrid.TargetState = (settingsFlags & 2048) == 2048;
            }

            if (Properties.Settings.Default.ShowGalacticGridText.TargetState != ((settingsFlags & 4096) == 4096))
            {
                Properties.Settings.Default.ShowGalacticGridText.TargetState = (settingsFlags & 4096) == 4096;
            }

            if (Properties.Settings.Default.ShowEclipticGrid.TargetState != ((settingsFlags & 8192) == 8192))
            {
                Properties.Settings.Default.ShowEclipticGrid.TargetState = (settingsFlags & 8192) == 8192;
            }

            if (Properties.Settings.Default.ShowEclipticGridText.TargetState != ((settingsFlags & 16384) == 16384))
            {
                Properties.Settings.Default.ShowEclipticGridText.TargetState = (settingsFlags & 16384) == 16384;
            }

            if (Properties.Settings.Default.ShowEclipticOverviewText.TargetState != ((settingsFlags & 32768) == 32768))
            {
                Properties.Settings.Default.ShowEclipticOverviewText.TargetState = (settingsFlags & 32768) == 32768;
            }

            if (Properties.Settings.Default.ShowAltAzGrid.TargetState != ((settingsFlags & 65536) == 65536))
            {
                Properties.Settings.Default.ShowAltAzGrid.TargetState = (settingsFlags & 65536) == 65536;
            }

            if (Properties.Settings.Default.ShowAltAzGridText.TargetState != ((settingsFlags & 131072) == 131072))
            {
                Properties.Settings.Default.ShowAltAzGridText.TargetState = (settingsFlags & 131072) == 131072;
            }

            if (Properties.Settings.Default.ShowPrecessionChart.TargetState != ((settingsFlags & 262144) == 262144))
            {
                Properties.Settings.Default.ShowPrecessionChart.TargetState = (settingsFlags & 262144) == 262144;
            }

            if (Properties.Settings.Default.ShowConstellationPictures.TargetState != ((settingsFlags & 524288) == 524288))
            {
                Properties.Settings.Default.ShowConstellationPictures.TargetState = (settingsFlags & 524288) == 524288;
            }

            if (Properties.Settings.Default.ShowConstellationLabels.TargetState != ((settingsFlags & 1048576) == 1048576))
            {
                Properties.Settings.Default.ShowConstellationLabels.TargetState = (settingsFlags & 1048576) == 1048576;
            }

            if (Earth3d.MainWindow.Fader.TargetState != ((settingsFlags & 2097152) == 2097152))
            {
                Earth3d.MainWindow.Fader.TargetState = (settingsFlags & 2097152) == 2097152;
            }

            if (Properties.Settings.Default.ShowEquatorialGridText.TargetState != ((settingsFlags & 4194304) == 4194304))
            {
                Properties.Settings.Default.ShowEquatorialGridText.TargetState = (settingsFlags & 4194304) == 4194304;
            }

            if (Properties.Settings.Default.ShowSkyOverlays.TargetState != ((settingsFlags & 8388608) == 8388608))
            {
                Properties.Settings.Default.ShowSkyOverlays.TargetState = (settingsFlags & 8388608) == 8388608;
            }

            if (Properties.Settings.Default.Constellations.TargetState != ((settingsFlags & 16777216) == 16777216))
            {
                Properties.Settings.Default.Constellations.TargetState = (settingsFlags & 16777216) == 16777216;
            }

            if (Properties.Settings.Default.ShowSkyNode.TargetState != ((settingsFlags & 33554432) == 33554432))
            {
                Properties.Settings.Default.ShowSkyNode.TargetState = (settingsFlags & 33554432) == 33554432;
            }

            if (Properties.Settings.Default.ShowSkyGrids.TargetState != ((settingsFlags & 67108864) == 67108864))
            {
                Properties.Settings.Default.ShowSkyGrids.TargetState = (settingsFlags & 67108864) == 67108864;
            }

            if (Properties.Settings.Default.ShowSkyOverlaysIn3d.TargetState != ((settingsFlags & 134217728) == 134217728))
            {
                Properties.Settings.Default.ShowSkyOverlaysIn3d.TargetState = (settingsFlags & 134217728) == 134217728;
            }

            if ((Properties.Settings.Default.MultiSampling == 1) != ((settingsFlags & 268435456) == 268435456))
            {
                Properties.Settings.Default.MultiSampling = ((settingsFlags & 268435456) == 268435456) ? 4 : 1;
            }
        }

        static readonly byte[] currentSettingsBlendStates = new byte[35];
        internal static void SetSettingsBelndStates()
        {
            var data = settingsData;

            var changed = false;

            for (var i = 0; i < 35; i++)
            {
                if (data[i] != currentSettingsBlendStates[i])
                {
                    changed = true;
                }
                currentSettingsBlendStates[i] = data[i];
            }

            if (!changed)
            {
                return;
            }




            if (Settings.Active.ShowElevationModel != (data[0] != 0))
            {
                TileCache.PurgeQueue();
                TileCache.ClearCache();
                Properties.Settings.Default.ShowElevationModel = (data[0] != 0);
            }

            if (Properties.Settings.Default.LineSmoothing != (data[1] != 0))
            {
                Properties.Settings.Default.LineSmoothing = (data[1] != 0);
            }

            if (Properties.Settings.Default.LocalHorizonMode != (data[2] != 0))
            {
                Properties.Settings.Default.LocalHorizonMode = (data[2] != 0);
            }

            Properties.Settings.Default.ShowClouds.Opacity = data[3] / 255.0f;
            Properties.Settings.Default.ShowConstellationBoundries.Opacity = data[4] / 255.0f;
            Properties.Settings.Default.ShowConstellationFigures.Opacity = data[5] / 255.0f;
            Properties.Settings.Default.ShowConstellationSelection.Opacity = data[6] / 255.0f;
            Properties.Settings.Default.ShowFieldOfView.Opacity = data[7] / 255.0f;
            Properties.Settings.Default.ShowGrid.Opacity = data[8] / 255.0f;

            if (Properties.Settings.Default.ShowHorizon != (data[9] != 0))
            {
                Properties.Settings.Default.ShowHorizon = (data[9] != 0);
            }

            Properties.Settings.Default.ShowEcliptic.Opacity = data[10] / 255.0f;
            Properties.Settings.Default.ShowGalacticGrid.Opacity = data[11] / 255.0f;
            Properties.Settings.Default.ShowGalacticGridText.Opacity = data[12] / 255.0f;
            Properties.Settings.Default.ShowEclipticGrid.Opacity = data[13] / 255.0f;
            Properties.Settings.Default.ShowEclipticGridText.Opacity = data[14] / 255.0f;
            Properties.Settings.Default.ShowEclipticOverviewText.Opacity = data[15] / 255.0f;
            Properties.Settings.Default.ShowAltAzGrid.Opacity = data[16] / 255.0f;
            Properties.Settings.Default.ShowAltAzGridText.Opacity = data[17] / 255.0f;
            Properties.Settings.Default.ShowPrecessionChart.Opacity = data[18] / 255.0f;
            Properties.Settings.Default.ShowConstellationPictures.Opacity = data[19] / 255.0f;
            Properties.Settings.Default.ShowConstellationLabels.Opacity = data[20] / 255.0f;
            Earth3d.MainWindow.Fader.Opacity = data[21] / 255.0f;
            Properties.Settings.Default.ShowEquatorialGridText.Opacity = data[22] / 255.0f;
            Properties.Settings.Default.ShowSkyOverlays.Opacity = data[23] / 255.0f;
            Properties.Settings.Default.Constellations.Opacity = data[24] / 255.0f;
            Properties.Settings.Default.ShowSkyNode.Opacity = data[25] / 255.0f;
            Properties.Settings.Default.ShowSkyGrids.Opacity = data[26] / 255.0f;
            Properties.Settings.Default.ShowSkyOverlaysIn3d.Opacity = data[27] / 255.0f;

            if (Properties.Settings.Default.MultiSampling != data[28])
            {
                Properties.Settings.Default.MultiSampling = data[28];
            }

            Properties.Settings.Default.ShowReticle.Opacity = data[29] / 255.0f;

            Properties.Settings.Default.EarthCutawayView.Opacity = data[30] / 255.0f;

            Properties.Settings.Default.FrameSync = data[31] != 0;
            Properties.Settings.Default.TargetFrameRate = data[32];
            Properties.Settings.Default.MilkyWayModel = data[33] != 0;
            Properties.Settings.Default.GalacticMode = data[34] != 0;
        }


        public static void SendCommand(string output)
        {
            try
            {
                if (sockA == null)
                {
                    sockA = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
                    sockA.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                    sockA.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    var bindEPA = new IPEndPoint(IPAddress.Parse(GetThisHostIP()), 8090);
                    sockA.Bind(bindEPA);
                }
                var destinationEPA = (EndPoint)new IPEndPoint(IPAddress.Broadcast, 8087);


                var header = Encoding.ASCII.GetBytes(output);

                sockA.SendTo(header, destinationEPA);
            }
            catch
            {
            }
        }

        public static void SendCommand(Byte[] output, int length)
        {
            try
            {
                if (sockA == null)
                {
                    sockA = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
                    sockA.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                    sockA.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    var bindEPA = new IPEndPoint(IPAddress.Parse(GetThisHostIP()), 8090);
                    sockA.Bind(bindEPA);

                }
                var destinationEPA = (EndPoint)new IPEndPoint(IPAddress.Broadcast, 8087);



                sockA.SendTo(output, 0, length, SocketFlags.None, destinationEPA);
            }
            catch
            {
            }
        }

        public static string GetThisHostIP()
        {
            var hostIp = "127.1.1.1";

            if (!String.IsNullOrEmpty(Properties.Settings.Default.MasterHostIPOverride))
            {
                return Properties.Settings.Default.MasterHostIPOverride;
            }

            try
            {
                var strHostName = Dns.GetHostName();

                // Find host by name
                var iphostentry = Dns.GetHostByName(strHostName);

                // Enumerate IP addresses
                foreach (var ipaddress in iphostentry.AddressList)
                {
                    if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        hostIp = ipaddress.ToString();
                        if (!hostIp.StartsWith("169"))
                        {
                            break;
                        }
                    }
                }
            }
            catch
            {
            }

            return hostIp;
        }

        internal static void GetColorSettings()
        {
            var client = new WebClient();
            //sync color properties
            try
            {
                if (MasterAddress == "127.0.0.1")
                {
                    return;
                }

                if (Earth3d.Logging) { Earth3d.WriteLogMessage("Get Color Settings from Server"); }

                var url = string.Format("http://{0}:5050/Configuration/images/colorsettings", MasterAddress);
                var data = client.DownloadString(url);

                var lines = data.Split(new[] { ',' });
                if (lines.Length == 21)
                {
                    var index = 0;
                    Earth3d.MainWindow.SuspendChanges();
                    var version = Properties.Settings.Default.ColSettingsVersion = int.Parse(lines[index++]);
                    Properties.Settings.Default.GridColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.EquatorialGridTextColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.GalacticGridColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.GalacticGridTextColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.EclipticGridColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.EclipticGridTextColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.EclipticColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.EclipticGridTextColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.PrecessionChartColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.AltAzGridColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.AltAzGridTextColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.ConstellationFigureColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.ConstellationBoundryColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.ConstellationSelectionColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.ConstellationnamesColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.ConstellationArtColor = SavedColor.Load(lines[index++]);
                    Properties.Settings.Default.ShowEarthSky.State = bool.Parse(lines[index++]);
                    Properties.Settings.Default.CloudMap8k =  bool.Parse(lines[index++]);
                    Earth3d.MainWindow.StereoMode = (Earth3d.StereoModes)Enum.Parse(typeof(Earth3d.StereoModes), lines[index++]);
                    Properties.Settings.Default.FaceNorth = bool.Parse(lines[index++]);

                    Properties.Settings.Default.ColSettingsVersion = version;
                    Earth3d.MainWindow.ProcessChanged();
                }

            }
            catch
            {
            }
            Earth3d.MainWindow.SyncLayerNeeded = false;
        }

        internal static byte[] GetColorSettingsData()
        {
            string data;

            data = Properties.Settings.Default.ColSettingsVersion + "," +
                       SavedColor.Save(Properties.Settings.Default.GridColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.EquatorialGridTextColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.GalacticGridColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.GalacticGridTextColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.EclipticGridColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.EclipticGridTextColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.EclipticColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.EclipticGridTextColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.PrecessionChartColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.AltAzGridColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.AltAzGridTextColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.ConstellationFigureColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.ConstellationBoundryColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.ConstellationSelectionColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.ConstellationnamesColor) + "," +
                       SavedColor.Save(Properties.Settings.Default.ConstellationArtColor) + "," +
                       Properties.Settings.Default.ShowEarthSky.TargetState + "," +
                       Properties.Settings.Default.CloudMap8k + "," +
                       Earth3d.MainWindow.StereoMode + "," +
                       Properties.Settings.Default.FaceNorth;

            return Encoding.UTF8.GetBytes(data);
        }

        internal static void PingStatus()
        {
            ReportStatus(currentStatus, CurrentStatusText, null);
        }

        static ClientNodeStatus currentStatus = ClientNodeStatus.Online;
        static string CurrentStatusText = "Ready";
        delegate void ReportDelegate(string Uri);


        internal static void ReportStatus(ClientNodeStatus status, string statusText, string error)
        {
            var stream = new MemoryStream();
            if (string.IsNullOrEmpty(error))
            {
                error = "";
            }
            var bw = new BinaryWriter(stream);
            bw.Write((byte)42);
            bw.Write((byte)42);
            bw.Write((byte)13); //Status Packet
            bw.Write(Earth3d.MainWindow.Config.ClusterID);
            bw.Write(Earth3d.MainWindow.Config.NodeID);
            bw.Write(Earth3d.MainWindow.Config.NodeDiplayName);
            bw.Write(status.ToString());
            bw.Write(statusText);
            bw.Write(Earth3d.LastFPS);
            bw.Write(error);
            bw.Flush();

            SendStatus(stream.GetBuffer(), (int)stream.Length);
        }


        static Socket sockStat;
        public static void SendStatus(Byte[] output, int length)
        {
            if (MasterAddress == "127.0.0.1")
            {
                return;
            }
            try
            {
                if (sockStat == null)
                {
                    sockStat = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
                    //sockA.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                    sockStat.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                    var bindEPA = new IPEndPoint(IPAddress.Parse(GetThisHostIP()), 8091);
                    sockStat.Bind(bindEPA);
                }
                var destinationEPA = (EndPoint)new IPEndPoint(IPAddress.Parse(MasterAddress), 8091);

                sockStat.SendTo(output, 0, length, SocketFlags.None, destinationEPA);
            }
            catch
            {
            }
        }

        internal static void ReportStatusOld(ClientNodeStatus status, string statusText, string error)
        {
            try
            {
                if (MasterAddress == "127.0.0.1" || Settings.MasterController)
                {
                    return;
                }

                if (Earth3d.Logging) { Earth3d.WriteLogMessage("Reporting Status:" + statusText); }

                currentStatus = status;
                CurrentStatusText = statusText;
                var url = string.Format("http://{0}:5050/status?NodeID={1}&NodeName={2}&FPS={3}&Error={4}&Status={5}&StatusText={6}"
                    , MasterAddress, Earth3d.MainWindow.Config.NodeID, Earth3d.MainWindow.Config.NodeDiplayName, Earth3d.LastFPS, error, status, statusText);

                var report = new ReportDelegate(Report);
                report.BeginInvoke(url, null, null);
                //WebClient client = new WebClient();
                //client.DownloadStringAsync(new Uri(url));

            }
            catch
            {
            }
        }

        internal static void Report(string uri)
        {
            var client = new WebClient();
            client.DownloadStringAsync(new Uri(uri));
        }


        internal static void LogStatusReport(int nodeID, string name, string ipAddress, ClientNodeStatus status, string statusText, float FPS, string error)
        {
            if (Earth3d.Logging) { Earth3d.WriteLogMessage("Received Status Report"); }
            NodeListDirty = true;
            ClientNode node = null;
            if (NodeList.ContainsKey(nodeID))
            {
                node = NodeList[nodeID];
            }
            else
            {
                node = new ClientNode(name, nodeID, ipAddress, DateTime.Now, status);
                NodeList[nodeID] = node;
            }

            if (string.IsNullOrEmpty(node.MacAddress))
            {
                var mac = Arp(IPAddress.Parse(ipAddress));

                node.MacAddress = mac.ToString();
            }


            node.Name = name;
            node.Status = status;

            if (!string.IsNullOrEmpty(statusText))
            {
                node.StatusText = statusText;
            }

            node.IpAddress = ipAddress;
            node.LastFPS = FPS;
            node.LastReport = DateTime.Now;


            if (!string.IsNullOrEmpty(error))
            {
                node.ReportError(error);
            }
        }


        public static PhysicalAddress Arp(IPAddress ipAddress)
        {
            try
            {
                if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
                {
                    return null;
                }

                var convertedIp = IpToInt32(ipAddress);
                var src = IpToInt32(IPAddress.Parse(GetThisHostIP()));
                //byte array
                var data = new byte[6]; // 48 bit
                var len = data.Length;

                var result = SendArp(convertedIp, src, data, ref len);

                if (result != 0)
                {
                    return null;
                }

                //return the MAC address in a PhysicalAddress format
                return new PhysicalAddress(data);
            }
            catch 
            {

                return null;
            }
        }

        private static Int32 IpToInt32(IPAddress ipIn)
        {
            var data = ipIn.GetAddressBytes();
            return BitConverter.ToInt32(data, 0);
        }

        [DllImport("Iphlpapi.dll", EntryPoint = "SendARP")]
        internal extern static Int32 SendArp(Int32 destIpAddress, Int32 srcIpAddress, byte[] macAddress, ref Int32 macAddressLength);

    }

    public enum ClientNodeStatus { Offline, StandBy, Working, Online };
    [Serializable]
    public class ClientNodes
    {
        [XmlArray("ClientNodes")]
        public List<ClientNode> NodeList = new List<ClientNode>();
        public ClientNodes()
        {
        }

        public ClientNodes(Dictionary<int, ClientNode> nodeList)
        {
            NodeList.AddRange(nodeList.Values);

        }

        public static Dictionary<int, ClientNode> Load(string filename)
        {
            ClientNodes loader;
            var serializer = new XmlSerializer(typeof(ClientNodes));
            var fs = new FileStream(filename, FileMode.Open);

            loader = (ClientNodes)serializer.Deserialize(fs);

            fs.Close();
            var loaded = new Dictionary<int, ClientNode>();

            foreach (var node in loader.NodeList)
            {
                loaded.Add(node.NodeID, node);
            }


            return loaded;
        }



        public static void Save(Dictionary<int, ClientNode> nodeList, string filename)
        {
            var serializer = new XmlSerializer(typeof(ClientNodes));
            var sw = new StreamWriter(filename);
            var saver = new ClientNodes(nodeList);
            serializer.Serialize(sw, saver);

            sw.Close();
        }

        public static string GetXML(Dictionary<int, ClientNode> nodeList)
        {
            var serializer = new XmlSerializer(typeof(ClientNodes));

            var sw = new StringWriter();

            var saver = new ClientNodes(nodeList);
            serializer.Serialize(sw, saver);

            return sw.ToString();
        }
    }

    [Serializable]
    public class ClientNode
    {
        public string Name;
        public int NodeID;
        public string IpAddress;
        public string MacAddress;
        public DateTime LastReport = new DateTime();
        public ClientNodeStatus Status = ClientNodeStatus.Offline;
        [XmlIgnore]
        public string StatusText;
        [XmlIgnore]
        public float LastFPS;
        [XmlIgnore]
        public List<string> ErrorLog = new List<string>();

        public ClientNode()
        {
        }

        public ClientNode(string name, int nodeID, string ipAddress, DateTime lastReport, ClientNodeStatus status)
        {
            Name = name;
            NodeID = nodeID;
            IpAddress = ipAddress;
            LastReport = lastReport;
            Status = status;
        }

        public void ReportError(string errorText)
        {
            ErrorLog.Add(string.Format("{0} : {1}", DateTime.Now, errorText));
        }
    }
}
