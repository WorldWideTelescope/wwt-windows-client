using System;
using System.Collections.Generic;
#if WINDOWS_UWP
using SysColor = TerraViewer.Color;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;
using Windows.UI.Input.Spatial;

#else
using SysColor = System.Drawing.Color;
using Bitmap = System.Drawing.Bitmap;
using System.Xml;
using System.Windows.Forms;
using WwtDataUtils;
using OculusWrap;
#endif
using System.Text;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using System.IO;

namespace TerraViewer
{
    public class RenderEngine
    {
        public static RenderEngine Engine = null;

        public RenderEngine()
        {
            Engine = this;
        }

#if WINDOWS_UWP
        DataSetManager dsm;
        bool mixedReality = false;

        public void InitializeForUwp(Device device, SharpDX.WIC.ImagingFactory2 wicImagingFactory, int width, int height)
        {
            mixedReality = device != null;

            AppSettings.SettingsBase = Properties.Settings.Default;            
            //from constructor
            config = new Config();

            MonitorX = config.MonitorX;
            MonitorY = config.MonitorY;
            MonitorCountX = config.MonitorCountX;
            MonitorCountY = config.MonitorCountY;
            monitorHeight = config.Height;
            monitorWidth = config.Width;
            bezelSpacing = (float)config.Bezel;

            RenderContext11.MultiSampleCount = Math.Max(1, Properties.Settings.Default.MultiSampling);
            RenderContext11.MultiSampleCount = 8;     
            
            if (device !=null)
            {

                RenderContext11.ExternalProjection = true;
            }

            RenderContext11 = new RenderContext11(device,wicImagingFactory, width, height);




            Text3dBatch hold;
            hold = new Text3dBatch(80);
            hold.Add(new Text3d(new Vector3d(0, 0, 1), new Vector3d(0, 1, 0), " 0hr123456789-+", 80, .0001f));
            hold.Add(new Text3d(new Vector3d(0, 0, 1), new Vector3d(0, 1, 0), "JanuyFebMcApilg", 80, .0001f));
            hold.Add(new Text3d(new Vector3d(0, 0, 1), new Vector3d(0, 1, 0), "stSmOoNvDBCEdqV", 80, .0001f));
            hold.Add(new Text3d(new Vector3d(0, 0, 1), new Vector3d(0, 1, 0), "jxGHILPRTU", 80, .0001f));
            hold.PrepareBatch();

            // Load the EGM model
            EGM96Geoid.Height(0, 0);

            this.dsm = new DataSetManager();
            ContextSearch.InitializeDatabase(true);

            //from form load
            Constellations.InitializeConstellationNames();
            Constellations.Containment = constellationCheck;
            viewCamera.Target = SolarSystemObjects.Sun;
            SpaceTimeController.Altitude = Properties.Settings.Default.LocationAltitude;
            SpaceTimeController.Location = Coordinates.FromLatLng(Properties.Settings.Default.LocationLat, Properties.Settings.Default.LocationLng);

            fadeImageSet.State = false;
            fadeImageSet.State = true;
            fadeImageSet.TargetState = false;
            Properties.Settings.Default.ActualPlanetScale = true;
            Properties.Settings.Default.HighPercitionPlanets = true;
            Properties.Settings.Default.ShowMoonsAsPointSource = false;
            Properties.Settings.Default.ShowSolarSystem.TargetState = true;
            InitializeImageSets();

            ContextSearch.InitializeDatabase(true);
            Catalogs.InitSearchTable();

            ringMenu = new RingMenu();
            ringMenu.Initialize();
            folderPanel = new FolderPanel();
            ringMenu.AddPanel(folderPanel);
            finderPanel = new FinderPanel();
            ringMenu.AddPanel(finderPanel);

            var t = System.Threading.Tasks.Task.Run(() =>
            {

                LoadExploreRoot();
                if (explorerRoot != null)
                {
                    ContextSearch.AddFolderToSearch(explorerRoot, true);
                }
                ContextSearch.AddCatalogs(true);
                ContextSearch.Initialized = true;
            });

            if (mixedReality)
            {
                var t1 = System.Threading.Tasks.Task.Run(() =>
                {
                    settingModel = Object3d.LoadFromModelFileFromUrl("http://www.worldwidetelescope.org/data/sh.mdl.txt");
                    SpaceTimeController.TimeRate = 100;
                });
            }



            currentImageSetfield = GetDefaultImageset(ImageSetType.Sky, BandPass.Visible);
            //currentImageSetfield = GetDefaultImageset(ImageSetType.Earth, BandPass.Visible);
            //currentImageSetfield = GetDefaultImageset(ImageSetType.SolarSystem, BandPass.Visible);
            //currentImageSetfield = GetDefaultImageset(ImageSetType.Sandbox, BandPass.Visible);
            var t2 = System.Threading.Tasks.Task.Run(() =>
            {
                BackgroundInit();
            });

            //set settings to test

          //  Properties.Settings.Default.ConstellationArtColor = SysColor.FromArgb(20, 255, 255, 255);
            Properties.Settings.Default.ShowGrid.TargetState = false;
            Properties.Settings.Default.ShowEclipticGridText.TargetState = true;
            Properties.Settings.Default.ShowConstellationLabels.TargetState = true;
            Properties.Settings.Default.ShowConstellationFigures.TargetState = false;
            Properties.Settings.Default.ShowConstellationBoundries.TargetState = false;
            Properties.Settings.Default.ShowEcliptic.TargetState = false;
            Properties.Settings.Default.ShowConstellationPictures.TargetState = true;
            Properties.Settings.Default.ConstellationArtColor = Color.FromArgb(72, 255, 255, 255);
            Properties.Settings.Default.ShowISSModel = true;
            Properties.Settings.Default.CloudMap8k = true;
            Properties.Settings.Default.ShowSolarSystem.TargetState = false;
            Properties.Settings.Default.LocalHorizonMode = mixedReality;
            Properties.Settings.Default.ConstellationArtFilter = new ConstellationFilter();
            Properties.Settings.Default.ConstellationBoundariesFilter = new ConstellationFilter();
            Constellations.DrawNamesFiltered = true;

            LayerManager.InitLayers();
            TileCache.StartQueue();
            RenderEngine.Initialized = true;
            ReadyToRender = true;

            
           
        }
        

       
        private void LoadExploreRoot()
        {
            string url = Properties.Settings.Default.ExploreRootUrl;
            string filename = string.Format(@"{0}data\exploreRoot_{1}.wtml", Properties.Settings.Default.CahceDirectory, Math.Abs(url.GetHashCode32()));
            DataSetManager.DownloadFile(url, filename, false, true);
            explorerRoot = Folder.LoadFromFile(filename, true);
            MakeRingMenuFolder();
            folderPanel.LoadRootFoder(RingMenuRoot);

        }
#endif

        Object3d settingModel = null;
        Folder explorerRoot = null;
        FolderPanel folderPanel;
        FinderPanel finderPanel;
        ImageSetType LookAtType = ImageSetType.Sky;

        public void NextView()
        {
            int next = (((int)LookAtType) + 1) % 6;
            LookAtType = (ImageSetType)next;
            currentImageSetfield = GetDefaultImageset(LookAtType, BandPass.Visible);
            CameraParameters camParams = new CameraParameters(0, 0, 360, 0, 0, 100);
            
            GotoTarget(camParams, false, true);
            Alt = 0;
            Az = 0;
            targetAlt = 0;
            targetAz = 0;
        }

        public void PreviousView()
        {
            int next = (((int)LookAtType) + 5) % 6;
            LookAtType = (ImageSetType)next;
            currentImageSetfield = GetDefaultImageset(LookAtType, BandPass.Visible);
            CameraParameters camParams = new CameraParameters(0, 0, 360, 0, 0, 100);
            GotoTarget(camParams, false, true);
            Alt = 0;
            Az = 0;
            targetAlt = 0;
            targetAz = 0;
        }

        RingMenu ringMenu = null;

        public static void BackgroundInit()
        {

            Grids.InitStarVertexBuffer(RenderContext11.PrepDevice);
            Grids.MakeMilkyWay(RenderContext11.PrepDevice);
            Grids.InitCosmosVertexBuffer();
            Planets.InitPlanetResources();

        }

        Folder RingMenuRoot;

        public void MakeRingMenuFolder()
        {
            RingMenuRoot = new Folder();
            RingMenuRoot.Name = "Root Menu";
            var sky = new Folder();
            sky.Name = "Sky";
            var experiences = new Folder();
            experiences.Name = "Experiences";
            var settings = new Folder();
            settings.Name = "Settings";

            RingMenuRoot.AddChildFolder(sky);
            RingMenuRoot.AddChildFolder(experiences);
            RingMenuRoot.AddChildFolder(settings);

            settings.AddChildThumbnail(LayerManager.AllMaps["Sky"].Layers.Find(x => x.Name == "Overlays"));
            settings.AddChildThumbnail(LayerManager.AllMaps["Sky"].Layers.Find(x => x.Name == "2D Sky"));
            settings.AddChildThumbnail(LayerManager.AllMaps["Sky"].Layers.Find(x => x.Name == "3d Solar System"));
            foreach(var child in explorerRoot.Children)
            {
                sky.AddChildThumbnail(child as IThumbnail);
            }
        }

        public void TrackISS()
        {
            LayerMap target = LayerManager.AllMaps["ISS"];

            RenderEngine.Engine.SolarSystemTrack = SolarSystemObjects.Custom;
            RenderEngine.Engine.TrackingFrame = target.Name;
            RenderEngine.Engine.viewCamera.Zoom = RenderEngine.Engine.targetViewCamera.Zoom = .000000001;
            SpaceTimeController.TimeRate = 1;
            var scratch = SpaceTimeController.Now;
            SpaceTimeController.Now = scratch.AddHours(1);
            Properties.Settings.Default.SolarSystemMinorOrbits.TargetState = true;
            LayerManager.AllMaps["ISS"].Frame.ShowOrbitPath = true;
            //SpaceTimeController.SyncToClock = true;

        }

        const float FOVMULT = 343.774f;
        internal Config config;


        public void CleanUp()
        {
#if !WINDOWS_UWP
            CleanUpWarpBuffers();

            CleanupStereoAndDomeBuffers();
            CleanupDomeVertexBuffers();
#endif
        }
        public RenderContext11 RenderContext11 = null;

        IViewMover mover = null;

        internal IViewMover Mover
        {
            get { return mover; }
            set { mover = value; }
        }

        static public Matrix3d WorldMatrix;
        static public Matrix3d ViewMatrix;
        static public Matrix3d ProjMatrix;

        public void SetHeadPosition(Vector3d head)
        {
            // Need to filter this for noise and jitter;
            HeadPosition = head;

        }
        Vector3d HeadPosition = new Vector3d();


        public CameraParameters viewCamera = new CameraParameters(0, 0, 360, 0, 0, 100);
        public CameraParameters targetViewCamera = new CameraParameters(0, 0, 360, 0, 0, 100);

        public bool SandboxMode
        {
            get
            {
                if (currentImageSetfield == null)
                {
                    return false;
                }

                return currentImageSetfield.DataSetType == ImageSetType.Sandbox;
            }
        }

        public bool SolarSystemMode
        {
            get
            {
                if (currentImageSetfield == null)
                {
                    return false;
                }

                return currentImageSetfield.DataSetType == ImageSetType.SolarSystem;
            }
        }

        double fovAngle;

        public double FovAngle
        {
            get { return fovAngle; }
            set
            {
                fovAngle = value;
                degreesPerPixel = fovAngle / (double)RenderContext11.Height;
            }
        }

        private double degreesPerPixel;

        public double DegreesPerPixel
        {
            get { return degreesPerPixel; }
            set { degreesPerPixel = value; }
        }

        public bool zooming = false;
        public bool tracking = false;

        public bool Tracking
        {
            get { return tracking; }
            set { tracking = value; }
        }

        IPlace trackingObject = null;

        public IPlace TrackingObject
        {
            get { return trackingObject; }
            set { trackingObject = value; }
        }

        ZoomSpeeds zoomSpeed = ZoomSpeeds.MEDIUM;

        public ZoomSpeeds ZoomSpeed
        {
            get { return zoomSpeed; }
            set { zoomSpeed = value; }
        }

        bool zoomingUp = false;

        public delegate void NotifyComplete();

        public NotifyComplete NotifyMoveComplete = null;

        public double RAtoViewLng(double ra)
        {
            return (((180 - ((ra) / 24.0 * 360) - 180) + 540) % 360) - 180;
        }

        public double deltaLat = 0;
        public double deltaLong = 0;

        bool findingTargetGeo = false;

        /*
    * Place Holder             Meaning
    * RA                       Main View Center RA in decimal degrees
    * DEC                      Main View Center DEC in degrees degrees
    * FOV                      Main View Height in Decimal Degrees
    * UL.RA                    Upper Left RA
    * UL.DEC                   Upper Left Dec
    * UL, UR, LL, LR           Corners of view display
    * JD                       Decimal Julian Date
    * ROTATION                 Rotation angle East of North Decimal Degrees
    * OBJECT                   Catalog Name of Selected Object
    * SR                       Search Cone Radius that will Cover
    * LAT                      View From Latitude
    * LNG                      View From Longitude
    * ELEV                     View From Elevation
    * WIDTH                    Screen Width in Pixels
    * HEIGHT                   Screen Height in Pixels
    * CONST                    Current Constellation Abbreviation (Three letter codes)
    * ALT                      Local Altitude
    * AZ                       Local Azimuth
    * l                        Galactic Lat
    * b                        Galactic Long
    * CP1(a:b)Label            CP1, CP2, CP3, CP4 are custome sliders that show up when a folder specifies them
    *                          Slider values are send on each update, and URL refreshes with slider change

    * */
        /// <summary>
        /// Substitudes placeholders with live values in URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string PrepareUrl(string url)
        {
            url = url.Replace("{RA}", (RA * 15).ToString());
            url = url.Replace("{DEC}", Dec.ToString());
            url = url.Replace("{FOV}", FovAngle.ToString());
            if (CurrentViewCorners != null)
            {
                url = url.Replace("{UL.RA}", (CurrentViewCorners[0].RA * 15).ToString());
                url = url.Replace("{UL.DEC}", (CurrentViewCorners[0].Dec).ToString());
                url = url.Replace("{UR.RA}", (CurrentViewCorners[1].RA * 15).ToString());
                url = url.Replace("{UR.DEC}", (CurrentViewCorners[1].Dec).ToString());
                url = url.Replace("{LL.RA}", (CurrentViewCorners[2].RA * 15).ToString());
                url = url.Replace("{LL.DEC}", (CurrentViewCorners[2].Dec).ToString());
                url = url.Replace("{LR.RA}", (CurrentViewCorners[3].RA * 15).ToString());
                url = url.Replace("{LR.DEC}", (CurrentViewCorners[3].Dec).ToString());
            }

            url = url.Replace("{JD}", SpaceTimeController.JNow.ToString());
            url = url.Replace("{ROTATION}", CameraRotate.ToString());
            url = url.Replace("{SR}", (FovAngle * 1.5).ToString());
            url = url.Replace("{LAT}", SpaceTimeController.Location.Lat.ToString());
            url = url.Replace("{LNG}", SpaceTimeController.Location.Lng.ToString());
            url = url.Replace("{ELEV}", SpaceTimeController.Altitude.ToString());
            url = url.Replace("{WIDTH}", RenderContext11.Width.ToString());
            url = url.Replace("{HEIGHT}", RenderContext11.Height.ToString());
            url = url.Replace("{CONST}", Constellation);
            url = url.Replace("{ALT}", Alt.ToString());
            url = url.Replace("{AZ}", Az.ToString());
            //          url = url.Replace("{LiveToken}", CloudCommunities.GetTokenFromId(true));
            double[] gal = Coordinates.J2000toGalactic(RA * 15, Dec);

            url = url.Replace("{l}", gal[0].ToString());
            url = url.Replace("{b}", gal[1].ToString());

            return url;
        }

        public string Constellation;

        private void UpdateViewParameters()
        {
            double speed = 8;
            switch (zoomSpeed)
            {
                case ZoomSpeeds.FAST:
                    speed = 8;
                    break;
                case ZoomSpeeds.MEDIUM:
                    speed = 16;
                    break;
                case ZoomSpeeds.SLOW:
                    speed = 32;
                    break;
            }

            if (Math.Abs(ZoomFactor - TargetZoom) > (ZoomFactor / 2048))
            {
                ZoomFactor += (TargetZoom - ZoomFactor) / speed;
                zooming = true;
            }
            else
            {
                zoomingUp = false;
                ZoomFactor = TargetZoom;
                if (zooming)
                {
                    zooming = false;
                    if (NotifyMoveComplete != null)
                    {
                        NotifyMoveComplete();
                    }
                }
            }

            if (Math.Abs(CameraRotateTarget - CameraRotate) > (.1 * RC))
            {
                this.CameraRotate += (CameraRotateTarget - CameraRotate) / 10;
            }
            else
            {
                CameraRotate = CameraRotateTarget;
            }

            if (Math.Abs(CameraAngleTarget - CameraAngle) > (.1 * RC))
            {
                this.CameraAngle += (CameraAngleTarget - CameraAngle) / 10;
            }
            else
            {
                CameraAngle = CameraAngleTarget;
            }


            if (mover == null)
            {
                if (this.Space && tracking && trackingObject != null)
                {
                    if (Space && Settings.Active.GalacticMode)
                    {
                        double[] gPoint = Coordinates.J2000toGalactic(trackingObject.RA * 15, trackingObject.Dec);

                        targetAlt = alt = gPoint[1];
                        targetAz = az = gPoint[0];
                    }
                    else if (Space && Settings.Active.LocalHorizonMode)
                    {
                        Coordinates currentAltAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(trackingObject.RA, trackingObject.Dec), SpaceTimeController.Location, SpaceTimeController.Now);

                        targetAlt = alt = currentAltAz.Alt;
                        targetAz = az = currentAltAz.Az;
                    }
                    else
                    {
                        this.ViewLat = this.TargetLat = trackingObject.Dec;
                        this.ViewLong = this.TargetLong = this.RAtoViewLng(trackingObject.RA);
                    }
                }
                else if (!SolarSystemMode)
                {

                    //todo dome tilt looks fishey here...
                    if (Space && Settings.Active.LocalHorizonMode && Settings.DomeView)
                    {
                        targetAlt = alt = -config.TotalDomeTilt;
                        targetAz = az = 0;
                    }

                    tracking = false;
                    trackingObject = null;
                }
            }

            if (!zoomingUp && !tracking)
            {
                double minDelta = (ZoomFactor / 4000.0);
                if (ZoomFactor > 360)
                {
                    minDelta = (360.0 / 40000.0);
                }

                if (Space && (Settings.Active.LocalHorizonMode || Settings.Active.GalacticMode))
                {
                    if (((Math.Abs(this.targetAlt - this.alt) >= (minDelta)) |
                        ((Math.Abs(this.targetAz - this.az) >= (minDelta)))))
                    {
                        this.alt += (targetAlt - alt) / 10;

                        if (Math.Abs(targetAz - az) > 170)
                        {
                            if (targetAz > az)
                            {
                                this.az += (targetAz - (360 + az)) / 10;
                            }
                            else
                            {
                                this.az += ((360 + targetAz) - az) / 10;
                            }
                        }
                        else
                        {
                            this.az += (targetAz - az) / 10;
                        }

                        this.az = ((az + 720) % 360);
                    }
                }
                else
                {
                    if (((Math.Abs(this.TargetLat - this.ViewLat) >= (minDelta)) |
                        ((Math.Abs(this.TargetLong - this.ViewLong) >= (minDelta)))))
                    {
                        if (deltaLat != 0 | deltaLong != 0)
                        {
                            this.ViewLat += deltaLat;
                            this.ViewLong += deltaLong;
                        }
                        else
                        {
                            this.ViewLat += (TargetLat - ViewLat) / 10;

                            if (Math.Abs(TargetLong - ViewLong) > 170)
                            {
                                if (TargetLong > ViewLong)
                                {
                                    this.ViewLong += (TargetLong - (360 + ViewLong)) / 10;
                                }
                                else
                                {
                                    this.ViewLong += ((360 + TargetLong) - ViewLong) / 10;
                                }
                            }
                            else
                            {
                                this.ViewLong += (TargetLong - ViewLong) / 10;
                            }
                        }
                        this.ViewLong = ((ViewLong + 540) % 360) - 180;
                    }
                    else
                    {
                        if (this.ViewLat != this.TargetLat || this.ViewLong != this.TargetLong)
                        {
                            this.ViewLat = this.TargetLat;
                            this.ViewLong = this.TargetLong;

                            if (NotifyMoveComplete != null)
                            {
                                NotifyMoveComplete();
                            }
                        }
                        deltaLat = 0;
                        deltaLong = 0;
                        if (findingTargetGeo)
                        {
                            this.TargetZoom = finalZoom;
                            findingTargetGeo = false;
                        }
                    }
                }
            }
        }

        public double CameraRotateTarget
        {
            get { return targetViewCamera.Rotation; }
            set { targetViewCamera.Rotation = value; }
        }

        public double CameraRotate
        {
            get { return viewCamera.Rotation; }
            set { viewCamera.Rotation = value; }
        }

        public double CameraAngle
        {
            get { return viewCamera.Angle; }
            set { viewCamera.Angle = value; }
        }

        public double CameraAngleTarget
        {
            get { return targetViewCamera.Angle; }
            set { targetViewCamera.Angle = value; }
        }

        private int GetLevelForImageSet(IImageSet imageSet)
        {
            return (int)(Math.Log(imageSet.BaseTileDegrees / ZoomFactor, 2) + .01);
        }

        double planetFovWidth = Math.PI;

        public int viewTileLevel = 0;
        public double baseTileDegrees = 90;
        public int MaxLevels = 2;
        public int tileSizeX = 256;
        public int tileSizeY = 256;

        internal bool OnTarget(IPlace place)
        {
            bool ot = ((Math.Abs(ViewLat - TargetLat) < .0000000001 && Math.Abs(ViewLong - TargetLong) < .0000000001 && Math.Abs(ZoomFactor - TargetZoom) < .000000000001) && mover == null);
            return ot;
        }

        public void ComputeViewParameters(IImageSet imageSet)
        {

            this.MaxLevels = imageSet.Levels - 1;
            this.baseTileDegrees = (double)imageSet.BaseTileDegrees;


            double level = (double)Math.Log(baseTileDegrees / ZoomFactor, 2) + 2.01F;

            if ((int)level > MaxLevels)
            {
                viewTileLevel = MaxLevels;
            }
            else if (level < 0)
            {
                viewTileLevel = 0;
            }
            else
            {
                viewTileLevel = (int)level;
            }



            tileSizeY = (int)(256.0 * (1.0 * (((baseTileDegrees / ZoomFactor) / Math.Pow(2, viewTileLevel)))));

            tileSizeX = tileSizeY;


            if (TileCache.CurrentLevel != this.viewTileLevel)
            {
                TileCache.CurrentLevel = this.viewTileLevel;
            }

            return;
        }

        private int GetTileXFromLng(double lng)
        {
            double tile = ((lng + 180.0F) / (baseTileDegrees / ((double)Math.Pow(2, viewTileLevel))));
            if (tile < 0)
            {
                tile = -1;
            }
            return (int)tile;
        }

        private int GetTileYFromLat(double lat)
        {

            return (int)((lat + 90.0) / (baseTileDegrees / (Math.Pow(2, viewTileLevel)))) - 1;
        }





        public void GotoTarget(IPlace place, bool noZoom, bool instant, bool trackObject)
        {
            if (place == null)
            {
                return;
            }
            if ((trackObject && SolarSystemMode))
            {
                if ((place.Classification == Classification.SolarSystem && place.Type != ImageSetType.SolarSystem) || (place.Classification == Classification.Star) || (place.Classification == Classification.Galaxy) && place.Distance > 0)
                {
                    SolarSystemObjects target = SolarSystemObjects.Undefined;

                    if (place.Classification == Classification.Star || place.Classification == Classification.Galaxy)
                    {
                        target = SolarSystemObjects.Custom;
                    }
                    else
                    {
                        try
                        {
                            if (place.Target != SolarSystemObjects.Undefined)
                            {
                                target = place.Target;
                            }
                            else
                            {
                                target = (SolarSystemObjects)Enum.Parse(typeof(SolarSystemObjects), place.Name, true);
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (target != SolarSystemObjects.Undefined)
                    {
                        trackingObject = place;
                        double jumpTime = 4;

                        if (target == SolarSystemObjects.Custom)
                        {
                            jumpTime = 17;
                        }
                        else
                        {
                            jumpTime += 13 * (101 - Settings.Active.SolarSystemScale) / 100;
                        }

                        if (instant)
                        {
                            jumpTime = 1;
                        }

                        CameraParameters camTo = viewCamera;
                        camTo.TargetReferenceFrame = "";
                        camTo.Target = target;
                        double zoom = 10;
                        if (target == SolarSystemObjects.Custom)
                        {
                            if (place.Classification == Classification.Galaxy)
                            {
                                zoom = 1404946007758;
                            }
                            else
                            {
                                zoom = 63239.6717 * 100;
                            }
                            // Star or something outside of SS
                            Vector3d vect = Coordinates.RADecTo3d(place.RA, place.Dec, place.Distance);
                            double ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

                            vect.RotateX(ecliptic);
                            camTo.ViewTarget = -vect;
                        }
                        else
                        {

                            camTo.ViewTarget = Planets.GetPlanet3dLocation(target, SpaceTimeController.GetJNowForFutureTime(jumpTime));

                            switch (target)
                            {
                                case SolarSystemObjects.Sun:
                                    zoom = .6;
                                    break;
                                case SolarSystemObjects.Mercury:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Venus:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Mars:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Jupiter:
                                    zoom = .007;
                                    break;
                                case SolarSystemObjects.Saturn:
                                    zoom = .007;
                                    break;
                                case SolarSystemObjects.Uranus:
                                    zoom = .004;
                                    break;
                                case SolarSystemObjects.Neptune:
                                    zoom = .004;
                                    break;
                                case SolarSystemObjects.Pluto:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Moon:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Io:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Europa:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Ganymede:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Callisto:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Earth:
                                    zoom = .0004;
                                    break;
                                case SolarSystemObjects.Custom:
                                    zoom = 10;
                                    break;

                                default:
                                    break;
                            }

                            zoom = zoom * Settings.Active.SolarSystemScale;

                        }

                        CameraParameters fromParams = viewCamera;
                        if (SolarSystemTrack == SolarSystemObjects.Custom && !string.IsNullOrEmpty(TrackingFrame))
                        {
                            fromParams = CustomTrackingParams;
                            TrackingFrame = "";
                        }
                        camTo.Zoom = zoom;
                        Vector3d toVector = camTo.ViewTarget;
                        toVector.Subtract(fromParams.ViewTarget);


                        if (place.Classification == Classification.Star)
                        {
                            toVector = -toVector;
                        }

                        if (toVector.Length() != 0)
                        {

                            Vector2d raDec = toVector.ToRaDec();

                            if (target == SolarSystemObjects.Custom)
                            {
                                camTo.Lat = -raDec.Y;
                            }
                            else
                            {
                                camTo.Lat = raDec.Y;
                            }
                            camTo.Lng = raDec.X * 15 - 90;
                        }
                        else
                        {
                            camTo.Lat = viewCamera.Lat;
                            camTo.Lng = viewCamera.Lng;
                        }

                        if (target != SolarSystemObjects.Custom)
                        {
                            // replace with planet surface
                            camTo.ViewTarget = Planets.GetPlanetTargetPoint(target, camTo.Lat, camTo.Lng, SpaceTimeController.GetJNowForFutureTime(jumpTime));
                        }



                        ViewMoverKenBurnsStyle solarMover = new ViewMoverKenBurnsStyle(fromParams, camTo, jumpTime, SpaceTimeController.Now, SpaceTimeController.GetTimeForFutureTime(jumpTime), InterpolationType.EaseInOut);
                        solarMover.FastDirectionMove = true;
                        mover = solarMover;

                        return;
                    }
                }
            }


            Tracking = false;
            trackingObject = null;
            CameraParameters camParams = place.CamParams;



            if (place.Type != currentImageSetfield.DataSetType)
            {
                ZoomFactor = TargetZoom = ZoomMax;
                CameraRotateTarget = CameraRotate = 0;
                CameraAngleTarget = CameraAngle = 0;
                viewCamera = place.CamParams;
                if (place.BackgroundImageSet != null)
                {
                    FadeInImageSet(GetRealImagesetFromGeneric(place.BackgroundImageSet));
                }
                else
                {
                    currentImageSetfield = GetDefaultImageset(place.Type, BandPass.Visible);
                }
                instant = true;
            }
            else if (SolarSystemMode && place.Target != SolarSystemTrack)
            {
                ZoomFactor = TargetZoom = ZoomMax;
                CameraRotateTarget = CameraRotate = 0;
                CameraAngleTarget = CameraAngle = 0;
                viewCamera = targetViewCamera = place.CamParams;
                SolarSystemTrack = place.Target;
                instant = true;
            }


            if (place.Classification == Classification.Constellation)
            {
                camParams.Zoom = ZoomMax;
                GotoTarget(false, instant, camParams, null, null);
            }
            else
            {
                SolarSystemTrack = place.Target;
                GotoTarget(noZoom, instant, camParams, place.StudyImageset, place.BackgroundImageSet);

                if (trackObject)
                {
                    Tracking = true;
                    TrackingObject = place;
                }
            }

        }

        public void FreezeView()
        {
            targetAlt = alt;
            targetAz = az;
            TargetLat = ViewLat;
            TargetLong = ViewLong;
            TargetZoom = ZoomFactor;
            CameraRotateTarget = CameraRotate;
        }

        public void GotoTarget(CameraParameters camParams, bool noZoom, bool instant)
        {
            tracking = false;
            trackingObject = null;
            GotoTarget(noZoom, instant, camParams, this.studyImageset, this.currentImageSetfield);

        }
        public void GotoTargetRADec(double ra, double dec, bool noZoom, bool instant)
        {
            tracking = false;
            trackingObject = null;
            GotoTarget(noZoom, instant, new CameraParameters(dec, RAtoViewLng(ra), -1, viewCamera.Rotation, viewCamera.Angle, (float)viewCamera.Opacity), studyImageset, currentImageSetfield);
        }

        IImageSet targetStudyImageset = null;
        IImageSet targetBackgroundImageset = null;

        public delegate void NotifyStudyChanged();

        public NotifyStudyChanged NotifyStudyImagesetChanged = null;

        private void SetStudy(IImageSet study)
        {
            studyImageset = study;
            if (NotifyStudyImagesetChanged != null)
            {
                NotifyStudyImagesetChanged();
            }
        }

        public void SetStudyImageset(IImageSet studyImageSet, IImageSet backgroundImageSet)
        {
            targetStudyImageset = studyImageSet;
            targetBackgroundImageset = backgroundImageSet;
            if ((targetStudyImageset != null && studyImageset == null) || (studyImageset != null && !studyImageset.Equals(targetStudyImageset)))
            {
                SetStudy(targetStudyImageset);
            }

            if (targetBackgroundImageset != null && !currentImageSetfield.Equals(targetBackgroundImageset))
            {
                if (targetBackgroundImageset != null && targetBackgroundImageset.Generic)
                {

                    FadeInImageSet(GetRealImagesetFromGeneric(targetBackgroundImageset));
                }
                else
                {
                    FadeInImageSet(targetBackgroundImageset);
                }
            }
        }

        public void GotoTarget(bool noZoom, bool instant, CameraParameters cameraParams, IImageSet studyImageSet, IImageSet backgroundImageSet)
        {
            tracking = false;
            trackingObject = null;
            targetStudyImageset = studyImageSet;
            targetBackgroundImageset = backgroundImageSet;


            if (noZoom)
            {
                cameraParams.Zoom = viewCamera.Zoom;
                cameraParams.Angle = viewCamera.Angle;
                cameraParams.Rotation = viewCamera.Rotation;
            }
            else
            {
                if (cameraParams.Zoom == -1)
                {
                    if (Space)
                    {
                        cameraParams.Zoom = 1.40625;
                    }
                    else
                    {
                        cameraParams.Zoom = 0.09F;
                    }
                }
            }

            if (instant || (Math.Abs(ViewLat - cameraParams.Lat) < .000000000001 && Math.Abs(ViewLong - cameraParams.Lng) < .000000000001 && Math.Abs(ZoomFactor - cameraParams.Zoom) < .000000000001))
            {
                mover = null;
                viewCamera = targetViewCamera = cameraParams;

                if (Space && Settings.Active.GalacticMode)
                {
                    double[] gPoint = Coordinates.J2000toGalactic(viewCamera.RA * 15, viewCamera.Dec);
                    targetAlt = alt = gPoint[1];
                    targetAz = az = gPoint[0];
                }
                else if (Space && Settings.Active.LocalHorizonMode)
                {
                    Coordinates currentAltAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(viewCamera.RA, viewCamera.Dec), SpaceTimeController.Location, SpaceTimeController.Now);

                    targetAlt = alt = currentAltAz.Alt;
                    targetAz = az = currentAltAz.Az;
                }
                mover_Midpoint(this, new EventArgs());
            }
            else
            {
#if !BASICWWT
                if (TourPlayer.Playing)
                {
                    mover = new ViewMoverSlew(this.viewCamera, cameraParams);
                }
                else
#endif
                {
                    mover = new ViewMoverSlew(this.viewCamera, cameraParams, 1.2);
                }
                mover.Midpoint += new EventHandler(mover_Midpoint);
            }
        }

        void mover_Midpoint(object sender, EventArgs e)
        {
            if ((targetStudyImageset != null && studyImageset == null) || (studyImageset != null && !studyImageset.Equals(targetStudyImageset)))
            {
                SetStudy(targetStudyImageset);
            }

            if (targetBackgroundImageset != null && !currentImageSetfield.Equals(targetBackgroundImageset))
            {
                if (targetBackgroundImageset != null && targetBackgroundImageset.Generic)
                {

                    FadeInImageSet(GetRealImagesetFromGeneric(targetBackgroundImageset));
                }
                else
                {
                    FadeInImageSet(targetBackgroundImageset);
                }


            }
        }

        public BlendState fadeImageSet = new BlendState(true, 2000);
        public void FadeInImageSet(IImageSet newImageSet)
        {
            if (newImageSet.DataSetType != currentImageSetfield.DataSetType)
            {
                fadeImageSet.State = true;
                fadeImageSet.TargetState = false;
            }
            currentImageSetfield = newImageSet;
        }

        public bool InitializeImageSets()
        {

            string url = Properties.Settings.Default.ImageSetUrl;
            string filename = String.Format(@"{0}data\imagesets_5_{1}.wtml", Properties.Settings.Default.CahceDirectory, Math.Abs(url.GetHashCode32()));

            try
            {
                ImageSets.Clear();
                DataSetManager.DownloadFile(url, filename, false, true);
                XmlDocument doc = new XmlDocument();

                doc.Load(filename);
                XmlNode node = doc.GetChildByName("Folder");

                foreach (XmlNode child in node.ChildNodes)
                {
                    ImageSetHelper ish = ImageSetHelper.FromXMLNode(child);

                    ImageSets.Add(ish);
                    if (!String.IsNullOrEmpty(ish.AltUrl))
                    {
                        ReplacementImageSets.Add(ish.AltUrl, ish);
                    }
                }

                ImageSets.Add(new ImageSetHelper("SandBox", "", ImageSetType.Sandbox, BandPass.Visible, ProjectionType.Toast, 0, 0, 0, 0, 0, "", false, "", 0, 0, 0, false, "", false, false, 0, 0, 0, "", "", "", "", 1, "SandBox"));
                return true;
            }
            catch
            {
#if !WINDOWS_UWP
                File.Delete(filename);
                UiTools.ShowMessageBox(Language.GetLocalizedText(93, "The Imagery data file could not be downloaded or has been corrupted. WorldWide Telescope must close. You need a working internet connection to update this file. Try again later"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
#endif
                return false;
            }


        }

        public IImageSet GetImagesetByName(string name)
        {
            foreach (IImageSet imageset in ImageSets)
            {
                if (imageset.Name.ToLower() == name.ToLower())
                {
                    return imageset;
                }
            }
            return null;
        }

        public IImageSet GetDefaultImageset(ImageSetType imageSetType, BandPass bandPass)
        {
            foreach (IImageSet imageset in ImageSets)
            {
                if (imageset.DefaultSet && imageset.BandPass == bandPass && imageset.DataSetType == imageSetType)
                {
                    return imageset;
                }

            }
            foreach (IImageSet imageset in ImageSets)
            {
                if (imageset.BandPass == bandPass && imageset.DataSetType == imageSetType)
                {
                    return imageset;
                }

            }
            foreach (IImageSet imageset in ImageSets)
            {
                if (imageset.DataSetType == imageSetType)
                {
                    return imageset;
                }

            }
            return ImageSets[0];
        }

        private IImageSet GetRealImagesetFromGeneric(IImageSet generic)
        {
            foreach (IImageSet imageset in ImageSets)
            {
                if (imageset.DefaultSet && imageset.BandPass == generic.BandPass && imageset.DataSetType == generic.DataSetType)
                {
                    return imageset;
                }

            }

            foreach (IImageSet imageset in ImageSets)
            {
                if (imageset.BandPass == generic.BandPass && imageset.DataSetType == generic.DataSetType)
                {
                    return imageset;
                }

            }
            return ImageSets[0];
        }



        // Begin Set View Mode

        public void SetViewMode(IImageSet newImageSet)
        {
            if (newImageSet != null && currentImageSetfield != null && currentImageSetfield.DataSetType != newImageSet.DataSetType)
            {
                ZoomFactor = TargetZoom = ZoomMax;
                CameraRotate = CameraRotateTarget = 0;
            }
            currentImageSetfield = newImageSet;
            //TileCache.PurgeQueue();
            //TileCache.ClearCache();
        }


        public void SetViewMode()
        {

        }

        internal Constellations constellationsBoundries = new Constellations("Constellations", "http://www.worldwidetelescope.org/data/constellations.txt", true, false);
        internal Constellations constellationsFigures = new Constellations("Default Figures", "http://www.worldwidetelescope.org/data/figures.txt", false, false);
        internal Constellations constellationCheck = new Constellations("Constellations", "http://www.worldwidetelescope.org/data/constellations.txt", true, true);


        static public List<IImageSet> ImageSets = new List<IImageSet>();
        static public Dictionary<string, IImageSet> ReplacementImageSets = new Dictionary<string, IImageSet>();

        FieldOfView fov = null;
        BlendState fovBlend = new BlendState();

        public FieldOfView Fov
        {
            get { return fov; }
            set { fov = value; }
        }

        static public Dictionary<int, IImageSet> ImagesetHashTable = new Dictionary<int, IImageSet>();

        public static void AddImageSetToTable(int hash, IImageSet set)
        {
            if (!ImagesetHashTable.ContainsKey(hash))
            {
                if (set != null)
                {
                    ImagesetHashTable.Add(hash, set);
                }
            }
        }


        public IImageSet previewImageset = null;

        public IImageSet PreviewImageset
        {
            get { return previewImageset; }
            set { previewImageset = value; }
        }

        public BlendState PreviewBlend = new BlendState(false, 500);

        public IImageSet studyImageset = null;
        public IImageSet currentImageSetfield;


        public IImageSet videoOverlay = null;
        public float StudyOpacity
        {
            get { return (float)this.viewCamera.Opacity; }
            set { this.viewCamera.Opacity = value; }
        }

        public double ViewLat
        {
            get { return viewCamera.Lat; }
            set { viewCamera.Lat = value; }
        }

        public int ViewWidth
        {
            get
            {
#if !WINDOWS_UWP
                if (rift)
                {
                    return leftEyeWidth;
                }
#endif
                if ((!Space || rift) && (StereoMode == StereoModes.CrossEyed || StereoMode == StereoModes.SideBySide || StereoMode == StereoModes.OculusRift))
                {
                    return RenderContext11.Width / 2;
                }
                else
                {
                    return RenderContext11.Width;
                }
            }
        }
        public int ViewHeight
        {
            get
            {
#if !WINDOWS_UWP
                if (rift)
                {
                    return rightEyeHeight;
                }
#endif
                return RenderContext11.Height;
            }
        }

        public double GetEarthAltitude()
        {
            if (SolarSystemMode)
            {

                Vector3d pnt = Coordinates.GeoTo3dDouble(ViewLat, ViewLong + 90);

                Matrix3d EarthMat = Planets.EarthMatrixInv;

                pnt = Vector3d.TransformCoordinate(pnt, EarthMat);
                pnt.Normalize();

                Vector2d point = Coordinates.CartesianToLatLng(pnt);

                return GetAltitudeForLatLongForPlanet((int)viewCamera.Target, point.Y, point.X);

            }
            else if (currentImageSetfield.DataSetType == ImageSetType.Earth)
            {
                return TargetAltitude;
            }
            else if (currentImageSetfield.DataSetType == ImageSetType.Planet)
            {
                return GetAltitudeForLatLong(ViewLat, ViewLong);
            }
            else
            {
                return 0;
            }
        }

        public Vector2d GetEarthCoordinates()
        {
            if (SolarSystemMode)
            {
                Vector3d pnt = Coordinates.GeoTo3dDouble(ViewLat, ViewLong + 90);
                Matrix3d EarthMat = Planets.EarthMatrixInv;
                pnt = Vector3d.TransformCoordinate(pnt, EarthMat);
                pnt.Normalize();

                return Coordinates.CartesianToLatLng(pnt);

            }
            else if (currentImageSetfield.DataSetType == ImageSetType.Earth || currentImageSetfield.DataSetType == ImageSetType.Planet)
            {
                return new Vector2d(viewCamera.Lng, viewCamera.Lat);
            }
            else
            {
                return new Vector2d();
            }
        }


        public enum ViewTypes { Equatorial, AltAz, Galactic, Ecliptic, Planet };

        private ViewTypes viewType = ViewTypes.Equatorial;

        public ViewTypes ViewType
        {
            get { return viewType; }
            set { viewType = value; }
        }


        public bool Space
        {
            get
            {
                if (currentImageSetfield != null)
                {
                    return currentImageSetfield.DataSetType == ImageSetType.Sky;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool PlanetLike
        {
            get
            {
                if (currentImageSetfield != null)
                {
                    return currentImageSetfield.DataSetType == ImageSetType.Earth || currentImageSetfield.DataSetType == ImageSetType.Planet;
                }
                else
                {
                    return true;
                }
            }
        }

        public double RA
        {
            get
            {
                return ((((180 - (ViewLong - 180)) / 360) * 24.0) % 24);
            }
            set
            {
                if (double.NaN == value)
                {
                    // Break Here
                    value = 0;
                }
                double temp = 180 - ((value) / 24.0 * 360) - 180;
                if (temp != this.TargetLong)
                {
                    this.TargetLong = temp;
                }
            }
        }


        public double Dec
        {
            get
            {
                return this.ViewLat;
            }
            set
            {
                if (TargetLat != value)
                {
                    if (double.NaN == value)
                    {
                        // Break Here
                        value = 0;
                    }
                    this.TargetLat = value;
                }
            }
        }

        public double ViewLong
        {
            get { return viewCamera.Lng; }
            set
            {
                if (double.NaN == value)
                {
                    // Break Here
                    value = 0;
                }
                viewCamera.Lng = value;
            }
        }


        public double ZoomFactor
        {
            get { return viewCamera.Zoom; }
            set { viewCamera.Zoom = value; }
        }


        public double TargetZoom
        {
            get { return targetViewCamera.Zoom; }
            set { targetViewCamera.Zoom = value; }
        }
        double finalZoom = 360;
        double targetLat = 0;

        public double TargetLat
        {
            get { return targetViewCamera.Lat; }
            set
            {
                if (double.NaN == value)
                {
                    // Break Here
                    value = 0;
                }
                targetViewCamera.Lat = value;
            }
        }

        public double TargetLong
        {
            get { return targetViewCamera.Lng; }
            set
            {
                if (double.NaN == value)
                {
                    // Break Here
                    value = 0;
                }
                targetViewCamera.Lng = value;
            }
        }





        // Matricies and Tranforms

        const double RC = (double)(3.1415927 / 180);
        const int subDivisionsX = 48 * 4;
        const int subDivisionsY = 24 * 4;
        public bool showWireFrame = false;

        SysColor SkyColor = SysColor.FromArgb(255, 0, 0, 0);
        public static double front = -1;
        public static double back = 0;
        public static Vector3d cameraTarget = new Vector3d(0f, 0f, 1f);
        double colorBlend = 0.0;

        double m_nearPlane;

        public int MonitorX = 0;
        public int MonitorY = 0;
        public int MonitorCountX = 3;
        public int MonitorCountY = 3;

        public static bool multiMonClient = false;
        public static bool ProjectorServer = false;





        public int monitorWidth = 1920;
        public int monitorHeight = 1200;

        double alt = 0;
        public double targetAlt = 0;

        public double Alt
        {
            get { return alt; }
            set { alt = value; }
        }
        double az = 0;
        public double targetAz = 0;
        public double Az
        {
            get { return az; }
            set { az = value; }
        }

        public float bezelSpacing = 1.07f;
        static Vector3d viewPoint;

        static public Vector3d ViewPoint
        {
            get { return viewPoint; }
            set { viewPoint = value; }
        }

        private void SetupMatricesFisheye()
        {

            RenderContext11.World = Matrix3d.Identity;

            Matrix3d view = Matrix3d.Identity;
            Matrix3d ProjMatrix = Matrix3d.Identity;
            RenderContext11.View = view;



            m_nearPlane = 0f;
            if (ViewWidth > ViewHeight)
            {
                ProjMatrix.Matrix11 = SharpDX.Matrix.OrthoLH(((float)ViewWidth / (float)RenderContext11.Height) * 1f, 1f, 1, -1);
            }
            else
            {
                ProjMatrix.Matrix11 = SharpDX.Matrix.OrthoLH(1f, ((float)RenderContext11.Height / (float)ViewWidth) * 1f, 1, -1);
            }
            RenderContext11.Projection = ProjMatrix;

        }

        private void SetupMatricesWarpFisheye(float width)
        {

            RenderContext11.World = Matrix3d.Identity;

            Matrix3d view = Matrix3d.Identity;

            RenderContext11.View = view;

            m_nearPlane = 0f;



            if (ViewWidth > ViewHeight)
            {
                ProjMatrix.Matrix11 = SharpDX.Matrix.OrthoLH(width, 1f, 1, -1);
            }
            else
            {
                ProjMatrix.Matrix11 = SharpDX.Matrix.OrthoLH(width, 1f, 1, -1);
            }

            RenderContext11.Projection = ProjMatrix;

        }

        private void SetupMatricesDistort()
        {
            RenderContext11.World = Matrix3d.Identity;

            Matrix3d view = Matrix3d.Identity;

            RenderContext11.View = view;

            m_nearPlane = 0f;
            ProjMatrix.Matrix11 = SharpDX.Matrix.OrthoLH(1f, 1f, 1, -1);

            RenderContext11.Projection = ProjMatrix;

        }
        Matrix3d domeMatrix;
        bool domeMatrixFresh = false;
        bool domeAngleMatrixFresh = false;

        public bool DomeMatrixFresh
        {
            get { return domeMatrixFresh; }
            set
            {
                domeMatrixFresh = value;
                domeAngleMatrixFresh = value;
            }
        }


        Matrix3d DomeMatrix
        {
            get
            {
                if (!domeMatrixFresh)
                {
                    domeMatrix = Matrix3d.RotationX(((-(config.TotalDomeTilt + viewCamera.DomeAlt)) / 180 * Math.PI)) * Matrix3d.RotationY((config.DomeAngle + viewCamera.DomeAz) / 180 * Math.PI);
                    domeMatrixFresh = true;
                }
                return domeMatrix;
            }
        }

        Matrix3d DomeAngleMatrix
        {
            get
            {
                if (!domeAngleMatrixFresh)
                {
                    domeMatrix = Matrix3d.RotationX((-viewCamera.DomeAlt / 180 * Math.PI)) * Matrix3d.RotationY((config.DomeAngle + viewCamera.DomeAz) / 180 * Math.PI);
                    domeAngleMatrixFresh = true;
                }
                return domeMatrix;
            }
        }

        public void SetupMatricesOverlays()
        {

            RenderContext11.World = Matrix3d.Identity;

            Matrix3d lookAtAdjust = Matrix3d.Identity;

            Vector3d lookFrom = new Vector3d(0, 0, 0);
            Vector3d lookAt = new Vector3d(0, 0, 1);
            Vector3d lookUp = new Vector3d(0, 1, 0);

            bool dome = false;

            Matrix3d view;

            switch (CurrentRenderType)
            {
                case RenderTypes.DomeUp:
                    dome = true;
                    lookAtAdjust.Multiply(Matrix3d.RotationX(Math.PI / 2));
                    break;
                case RenderTypes.DomeLeft:
                    dome = true;
                    lookAtAdjust.Multiply(Matrix3d.RotationY(Math.PI / 2));
                    break;
                case RenderTypes.DomeRight:
                    dome = true;
                    lookAtAdjust.Multiply(Matrix3d.RotationY(-Math.PI / 2));
                    break;
                case RenderTypes.DomeFront:
                    dome = true;
                    break;
                case RenderTypes.DomeBack:
                    lookAtAdjust.Multiply(Matrix3d.RotationY(Math.PI));
                    dome = true;
                    break;
                default:
                    break;
            }
#if !WINDOWS_UWP
            if (config.MultiChannelDome1)
            {
                Matrix3d matHeadingPitchRoll = Matrix3d.Identity;

                if (config.UsingSgcWarpMap)
                {
                    matHeadingPitchRoll.Matrix = config.ProjectorMatrixSGC;
                }
                else
                {
                    matHeadingPitchRoll =
                    Matrix3d.RotationZ((config.Roll / 180 * Math.PI)) *
                    Matrix3d.RotationY((config.Heading / 180 * Math.PI)) *
                    Matrix3d.RotationX(((config.Pitch) / 180 * Math.PI));
                }

                view = Matrix3d.LookAtLH(lookFrom, lookAt, lookUp) * DomeMatrix * matHeadingPitchRoll;
            }
            else
#endif
            {
#if !WINDOWS_UWP
                if (Settings.DomeView)
                {
                    view = Matrix3d.LookAtLH(lookFrom, lookAt, lookUp) * DomeMatrix * lookAtAdjust;

                }
                else
#endif
                {
#if !WINDOWS_UWP
                    if (DomePreviewPopup.Active && !dome)
                    {
                        Matrix3d matDomePreview =
                             Matrix3d.RotationY((DomePreviewPopup.Az / 180 * Math.PI)) *
                             Matrix3d.RotationX((DomePreviewPopup.Alt / 180 * Math.PI));
                        view = Matrix3d.LookAtLH(lookFrom, lookAt, lookUp) * DomeMatrix * matDomePreview;
                    }
                    else if (rift)
                    {

                        Matrix3d matRiftView = Matrix3d.Identity;

                        var rotationQuaternion = SharpDXHelpers.ToQuaternion(eyeRenderPose[CurrentRenderType == RenderTypes.LeftEye ? 0 : 1].Orientation);
                        matRiftView.Matrix11 = (SharpDX.Matrix.RotationQuaternion(rotationQuaternion) * SharpDX.Matrix.Scaling(1, 1, 1));

                        view = Matrix3d.LookAtLH(lookFrom, lookAt, lookUp) * lookAtAdjust * matRiftView;
                    }
                    else
#endif
                    {
                        view = Matrix3d.LookAtLH(lookFrom, lookAt, lookUp) * lookAtAdjust;
                    }

                }
#if !WINDOWS_UWP
                if (multiMonClient)
                {
                    RenderContext11.View = RenderContext11.View * Matrix3d.RotationY((config.Heading / 180 * Math.PI));
                }
#endif
            }

            Matrix3d viewXform = Matrix3d.Scaling(1, -1, 1);

            view = viewXform * view;

            RenderContext11.View = view;

            double back = 10000;
            m_nearPlane = .1f;
#if !WINDOWS_UWP
            if (config.MultiChannelDome1)
            {
                double aspect = config.Aspect;
                double top = m_nearPlane * 2 / ((1 / Math.Tan(config.UpFov / 180 * Math.PI))) / 2;
                double bottom = m_nearPlane * 2 / -(1 / Math.Tan(config.DownFov / 180 * Math.PI)) / 2;
                double right = m_nearPlane * 2 / (1 / Math.Tan((config.UpFov + config.DownFov) / 2 / 180 * Math.PI)) * aspect / 2;
                double left = -right;



                ProjMatrix = Matrix3d.PerspectiveOffCenterLH(
                    left,
                    right,
                    bottom,
                    top,
                    m_nearPlane,
                    back);

            }
            else if (config.MultiProjector)
            {
                RenderContext11.View = RenderContext11.View * config.ViewMatrix;
                ProjMatrix = Matrix3d.PerspectiveFovLH((75f / 180f) * Math.PI, 1.777778, m_nearPlane, back);
                RenderContext11.ViewBase = RenderContext11.View;
            }
            else if (multiMonClient)
            {
                double fov = (((config.UpFov + config.DownFov) / 2 / 180 * Math.PI));
                if (fov == 0)
                {
                    fov = (Math.PI / 4.0);
                }
                ProjMatrix = Matrix3d.PerspectiveFovLH(fov, (double)(monitorWidth * MonitorCountX) / ((double)monitorHeight * (double)MonitorCountY), m_nearPlane, back);
            }
            else if (dome)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((Math.PI / 2.0), 1.0f, m_nearPlane, back);
            }
            else if (rift)
            {
                var fovPort = eyeTextures[CurrentRenderType == RenderTypes.LeftEye ? 0 : 1].FieldOfView;
                var projMat = wrap.Matrix4f_Projection(fovPort, (float)m_nearPlane, (float)back, OVRTypes.ProjectionModifier.LeftHanded).ToMatrix();

                RenderContext11.PerspectiveFov = Math.Atan(fovPort.UpTan + fovPort.DownTan);
                projMat.Transpose();

                ProjMatrix = new Matrix3d();
                ProjMatrix.Matrix11 = projMat;
            }
            else if (megaFrameDump)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH(fovLocal, (double)megaWidth / (double)megaHeight, m_nearPlane, back);

            }
            else
#endif
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH(fovLocal, (double)ViewWidth / (double)RenderContext11.Height, m_nearPlane, back);
            }
#if !WINDOWS_UWP
            if (multiMonClient && !config.MultiChannelDome1 && !config.MultiProjector && !config.MultiChannelGlobe)
            {
                ProjMatrix.M11 *= MonitorCountX * bezelSpacing;
                ProjMatrix.M22 *= MonitorCountY * bezelSpacing;
                ProjMatrix.M31 = (MonitorCountX - 1) - (MonitorX * bezelSpacing * 2);
                ProjMatrix.M32 = -((MonitorCountY - 1) - (MonitorY * bezelSpacing * 2));
            }

            if (rift)
            {
                if (CurrentRenderType == RenderTypes.LeftEye)
                {

                    ProjMatrix.M31 += iod;
                }
                else
                {
                    ProjMatrix.M31 -= iod;
                }
            }
#endif
            RenderContext11.Projection = ProjMatrix;
        }




        public void SetupMatricesAltAz()
        {
            RenderContext11.World = Matrix3d.Identity;

            Matrix3d lookAtAdjust = Matrix3d.Identity;

            Vector3d lookFrom = new Vector3d(0, 0, 0);
            Vector3d lookAt = new Vector3d(0, 0, 1);
            Vector3d lookUp = new Vector3d(0, 1, 0);

            bool dome = false;

            Matrix3d view;
            Matrix3d ProjMatrix;
#if !WINDOWS_UWP
            switch (CurrentRenderType)
            {
                case RenderTypes.DomeUp:
                    dome = true;
                    lookAtAdjust.Multiply(Matrix3d.RotationX(Math.PI / 2));
                    break;
                case RenderTypes.DomeLeft:
                    dome = true;
                    lookAtAdjust.Multiply(Matrix3d.RotationY(Math.PI / 2));
                    break;
                case RenderTypes.DomeRight:
                    dome = true;
                    lookAtAdjust.Multiply(Matrix3d.RotationY(-Math.PI / 2));
                    break;
                case RenderTypes.DomeFront:
                    dome = true;
                    break;
                case RenderTypes.DomeBack:
                    lookAtAdjust.Multiply(Matrix3d.RotationY(Math.PI));
                    dome = true;
                    break;
                default:
                    break;
            }

            if (config.MultiChannelDome1)
            {
                Matrix3d matHeadingPitchRoll = Matrix3d.Identity;

                if (config.UsingSgcWarpMap)
                {
                    matHeadingPitchRoll.Matrix = config.ProjectorMatrixSGC;
                }
                else
                {
                    matHeadingPitchRoll = 
                    Matrix3d.RotationZ((config.Roll / 180 * Math.PI)) *
                    Matrix3d.RotationY((config.Heading / 180 * Math.PI)) *
                    Matrix3d.RotationX(((config.Pitch) / 180 * Math.PI));
                }
                view = Matrix3d.LookAtLH(lookFrom, lookAt, lookUp) * matHeadingPitchRoll;
            }
            else
            {
                if (Settings.DomeView)
                {
                    view = Matrix3d.LookAtLH(lookFrom, lookAt, lookUp) * lookAtAdjust;

                }
                else
                {
                    if (DomePreviewPopup.Active && !dome)
                    {
                        Matrix3d matDomePreview =
                             Matrix3d.RotationY((DomePreviewPopup.Az / 180 * Math.PI)) *
                             Matrix3d.RotationX((DomePreviewPopup.Alt / 180 * Math.PI));
                        view = Matrix3d.LookAtLH(lookFrom, lookAt, lookUp) * matDomePreview;
                    }

                    else if (rift)
                    {
                        Matrix3d matRiftView = Matrix3d.Identity;

                        var rotationQuaternion = SharpDXHelpers.ToQuaternion(eyeRenderPose[CurrentRenderType == RenderTypes.LeftEye ? 0 : 1].Orientation);
                        matRiftView.Matrix11 = (SharpDX.Matrix.RotationQuaternion(rotationQuaternion) * SharpDX.Matrix.Scaling(1, 1, 1));

                        view = Matrix3d.LookAtLH(lookFrom, lookAt, lookUp) * lookAtAdjust * matRiftView;
                    }
                    else
#endif
                    {
                        view = Matrix3d.LookAtLH(lookFrom, lookAt, lookUp) * DomeMatrix * lookAtAdjust;
                    }

                    if (multiMonClient)
                    {
                        RenderContext11.View = RenderContext11.View * Matrix3d.RotationY((config.Heading / 180 * Math.PI));
                    }
#if !WINDOWS_UWP
                }
            }
#endif
            Matrix3d viewXform = Matrix3d.Scaling(1, 1, 1);

            view = viewXform * view;

            RenderContext11.View = view;

            double back = 10000;
            m_nearPlane = .1f;
#if !WINDOWS_UWP
            if (config.MultiChannelDome1)
            {
                double aspect = config.Aspect;
                double top = m_nearPlane * 2 / ((1 / Math.Tan(config.UpFov / 180 * Math.PI))) / 2;
                double bottom = m_nearPlane * 2 / -(1 / Math.Tan(config.DownFov / 180 * Math.PI)) / 2;
                double right = m_nearPlane * 2 / (1 / Math.Tan((config.UpFov + config.DownFov) / 2 / 180 * Math.PI)) * aspect / 2;
                double left = -right;



                ProjMatrix = Matrix3d.PerspectiveOffCenterLH(
                    left,
                    right,
                    bottom,
                    top,
                    m_nearPlane,
                    back);

            }
            else if (config.MultiProjector)
            {
                RenderContext11.View = RenderContext11.View * config.ViewMatrix;
                ProjMatrix = Matrix3d.PerspectiveFovLH((75f / 180f) * Math.PI, 1.777778, m_nearPlane, back);
                RenderContext11.ViewBase = RenderContext11.View;
            }
            else if (multiMonClient)
            {
                double fov = (((config.UpFov + config.DownFov) / 2 / 180 * Math.PI));
                if (fov == 0)
                {
                    fov = (Math.PI / 4.0);
                }
                ProjMatrix = Matrix3d.PerspectiveFovLH(fov, (double)(monitorWidth * MonitorCountX) / ((double)monitorHeight * (double)MonitorCountY), m_nearPlane, back);
            }
            else if (dome)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((Math.PI / 2.0), 1.0f, m_nearPlane, back);
            }
            else if (rift)
            {
                var fovPort = eyeTextures[CurrentRenderType == RenderTypes.LeftEye ? 0 : 1].FieldOfView;
                var projMat = wrap.Matrix4f_Projection(fovPort, (float)m_nearPlane, (float)back, OVRTypes.ProjectionModifier.LeftHanded).ToMatrix();

                RenderContext11.PerspectiveFov = Math.Atan(fovPort.UpTan + fovPort.DownTan);
                projMat.Transpose();

                ProjMatrix = new Matrix3d();
                ProjMatrix.Matrix11 = projMat;
            }
            else if (megaFrameDump)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH(RenderContext11.PerspectiveFov, (double)megaWidth / (double)megaHeight, m_nearPlane, back);

            }
            else
#endif
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH(RenderContext11.PerspectiveFov, (double)ViewWidth / (double)RenderContext11.Height, m_nearPlane, back);
            }
#if !WINDOWS_UWP
            if (multiMonClient && !config.MultiChannelDome1 && !config.MultiProjector)
            {
                ProjMatrix.M11 *= MonitorCountX * bezelSpacing;
                ProjMatrix.M22 *= MonitorCountY * bezelSpacing;
                ProjMatrix.M31 = (MonitorCountX - 1) - (MonitorX * bezelSpacing * 2);
                ProjMatrix.M32 = -((MonitorCountY - 1) - (MonitorY * bezelSpacing * 2));
            }

#endif

            RenderContext11.Projection = ProjMatrix;
        }

        bool galMatInit = false;
        Matrix3d galacticMatrix = Matrix3d.Identity;

        private void SetupMatricesSpace11(double localZoomFactor, RenderTypes renderType )
        {
            //todo uwp do we user MultiChannel for MR Headset?
#if !WINDOWS_UWP
            if (config.MultiChannelDome1 || config.MultiProjector || DomePreviewPopup.Active || rift)
            {
                SetupMatricesSpaceMultiChannel(localZoomFactor, renderType);
                return; 
            }
#endif

            if ((Settings.Active.GalacticMode && !Settings.Active.LocalHorizonMode) && currentImageSetfield.DataSetType == ImageSetType.Sky)
            {
                // Show in galactic coordinates
                if (!galMatInit)
                {
                    galacticMatrix = Matrix3d.Identity;
                    galacticMatrix.Multiply(Matrix3d.RotationY(-(90 - (17.7603329867975 * 15)) / 180.0 * Math.PI));
                    galacticMatrix.Multiply(Matrix3d.RotationX(-((-28.9361739586894)) / 180.0 * Math.PI));
                    galacticMatrix.Multiply(Matrix3d.RotationZ(((31.422052860102041270114993238783) - 90) / 180.0 * Math.PI));
                    galMatInit = true;
                }

                WorldMatrix = galacticMatrix;
                WorldMatrix.Multiply(Matrix3d.RotationY(((az)) / 180.0 * Math.PI));
                WorldMatrix.Multiply(Matrix3d.RotationX(-((alt)) / 180.0 * Math.PI));


                double[] gPoint = Coordinates.GalactictoJ2000(az, alt);

                this.RA = gPoint[0] / 15;
                this.Dec = gPoint[1];
                viewCamera.Lat = targetViewCamera.Lat;
                viewCamera.Lng = targetViewCamera.Lng;

            }
            else
            {
                // Show in Ecliptic

                WorldMatrix = Matrix3d.RotationY(-((this.ViewLong + 90.0) / 180.0 * Math.PI));
                WorldMatrix.Multiply(Matrix3d.RotationX(((-this.ViewLat) / 180.0 * Math.PI)));
            }
            double camLocal = CameraRotate;

            // altaz
            if ((Settings.Active.LocalHorizonMode && !Settings.Active.GalacticMode) && currentImageSetfield.DataSetType == ImageSetType.Sky)
            {
                Coordinates zenithAltAz = new Coordinates(0, 0);

                zenithAltAz.Az = 0;

                zenithAltAz.Alt = 0;



                if (!config.Master)
                {
                    alt = 0;
                    az = 0;
                    config.DomeTilt = 0;
                    if (Properties.Settings.Default.DomeTilt != 0)
                    {
                        Properties.Settings.Default.DomeTilt = 0;
                    }
                }

                Coordinates zenith = Coordinates.HorizonToEquitorial(zenithAltAz, SpaceTimeController.Location, SpaceTimeController.Now);

                double raPart = -((zenith.RA - 6) / 24.0 * (Math.PI * 2));
                double decPart = -(((zenith.Dec)) / 360.0 * (Math.PI * 2));
                string raText = Coordinates.FormatDMS(zenith.RA);
                WorldMatrix = Matrix3d.RotationY(-raPart);
                WorldMatrix.Multiply(Matrix3d.RotationX(decPart));

                if (SpaceTimeController.Location.Lat < 0)
                {
                    WorldMatrix.Multiply(Matrix3d.RotationY(((az) / 180.0 * Math.PI)));

                    WorldMatrix.Multiply(Matrix3d.RotationX(((alt) / 180.0 * Math.PI)));
                    camLocal += Math.PI;
                }
                else
                {
                    WorldMatrix.Multiply(Matrix3d.RotationY(((-az) / 180.0 * Math.PI)));

                    WorldMatrix.Multiply(Matrix3d.RotationX(((-alt) / 180.0 * Math.PI)));
                }

                Coordinates currentRaDec = Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(alt, az), SpaceTimeController.Location, SpaceTimeController.Now);

                TargetLat = ViewLat = currentRaDec.Dec;
                TargetLong = ViewLong = RAtoViewLng(currentRaDec.RA);
            }

            RenderContext11.World = WorldMatrix;
            RenderContext11.WorldBase = WorldMatrix;
            // altaz

            ViewPoint = Coordinates.RADecTo3d(this.RA, -this.Dec, 1.0);



            double distance = (4.0 * (localZoomFactor / 180)) + 0.000001;

            FovAngle = ((localZoomFactor/**16*/) / FOVMULT) / Math.PI * 180;
            RenderContext11.CameraPosition = new Vector3d(0.0, 0.0, 0.0);
            // This is for distance Calculation. For space everything is the same distance, so camera target is key.

            RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, new Vector3d(0.0, 0.0, -1.0), new Vector3d(Math.Sin(camLocal), Math.Cos(camLocal), 0.0));
#if !WINDOWS_UWP
            if (config.MultiChannelGlobe)
            {
                Matrix3d globeCameraRotation = Matrix3d.Identity;

                if (config.UsingSgcWarpMap)
                {
                    globeCameraRotation.Matrix = config.ProjectorMatrixSGC;
                }
                else
                {
                    globeCameraRotation =
                    Matrix3d.RotationZ((config.Roll / 180 * Math.PI)) *
                    Matrix3d.RotationY((config.Heading / 180 * Math.PI)) *
                    Matrix3d.RotationX(((config.Pitch) / 180 * Math.PI));
                }
                RenderContext11.View = RenderContext11.View * globeCameraRotation;
            }



            if (multiMonClient)
            {
                RenderContext11.View = RenderContext11.View * Matrix3d.RotationY((config.Heading / 180 * Math.PI));
            }
#endif

            RenderContext11.ViewBase = RenderContext11.View;
#if !WINDOWS_UWP
            m_nearPlane = 0f;
            if (multiMonClient)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((localZoomFactor/**16*/) / FOVMULT, (double)(monitorWidth * MonitorCountX) / (double)(monitorHeight * MonitorCountY), .1, -2.0);

            }
            else if (megaFrameDump)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((localZoomFactor/**16*/) / FOVMULT, (double)megaWidth / (double)megaHeight, .1, -2.0);

            }
            else
#endif
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((localZoomFactor/**16*/) / FOVMULT, (double)ViewWidth / (double)RenderContext11.Height, .1, -120.0);
                if (ProjectAtInfinity)
                {
                    RenderContext11.ProjectAtInfinity = true;
                }
            }
            RenderContext11.PerspectiveFov = (localZoomFactor) / FOVMULT;

#if !WINDOWS_UWP
            if (multiMonClient)
            {
                ProjMatrix.M11 *= MonitorCountX * bezelSpacing;
                ProjMatrix.M22 *= MonitorCountY * bezelSpacing;
                ProjMatrix.M31 = (MonitorCountX - 1) - (MonitorX * bezelSpacing * 2);
                ProjMatrix.M32 = -((MonitorCountY - 1) - (MonitorY * bezelSpacing * 2));
            }

            if (config.MultiChannelGlobe)
            {
                ProjMatrix = Matrix3d.OrthoLH(config.Aspect * 2.0, 2.0, 0.0, 2.0);
            }
#endif


            RenderContext11.Projection = ProjMatrix;

            ViewMatrix = RenderContext11.View;

            MakeFrustum();

        }


        private void SetupMatricesSpaceForZoomWindow(CameraParameters camera)
        {
            double camLocal = camera.Rotation;
            WorldMatrix = Matrix3d.RotationY(-((camera.Lng + 90.0) / 180.0 * Math.PI));
            WorldMatrix.Multiply(Matrix3d.RotationX(((-camera.Lat) / 180.0 * Math.PI)));

            RenderContext11.World = WorldMatrix;
            RenderContext11.WorldBase = WorldMatrix;
            // altaz

            ViewPoint = Coordinates.RADecTo3d(camera.RA, -camera.Dec, 1.0);

            double distance = (4.0 * (camera.Zoom / 180)) + 0.000001;

            FovAngle = ((camera.Zoom/**16*/) / FOVMULT) / Math.PI * 180;
            RenderContext11.CameraPosition = new Vector3d(0.0, 0.0, 0.0);

            RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, new Vector3d(0.0, 0.0, -1.0), new Vector3d(Math.Sin(camLocal), Math.Cos(camLocal), 0.0));

            RenderContext11.ViewBase = RenderContext11.View;

            ProjMatrix = Matrix3d.PerspectiveFovLH((camera.Zoom) / FOVMULT, (double)RenderContext11.Width / (double)RenderContext11.Height, .1, -2.0);

            RenderContext11.PerspectiveFov = (camera.Zoom) / FOVMULT;

            RenderContext11.Projection = ProjMatrix;

            ViewMatrix = RenderContext11.View;

            MakeFrustum();
        }

        bool ProjectAtInfinity = false;
        bool megaFrameDump = false;
        int megaWidth = 4096;
        int megaHeight = 4096;
#if !WINDOWS_UWP
        private void SetupMatricesSpaceMultiChannel(double localZoomFactor, RenderTypes renderType)
        {
            bool faceSouth = false;

            if ((Settings.Active.LocalHorizonMode && !Settings.Active.GalacticMode) && currentImageSetfield.DataSetType == ImageSetType.Sky)
            {
                faceSouth = !Properties.Settings.Default.FaceNorth;
                Coordinates currentRaDec = Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(0, 0), SpaceTimeController.Location, SpaceTimeController.Now);

                alt = 0;
                az = 0;
                config.DomeTilt = 0;
                if (Properties.Settings.Default.DomeTilt != 0)
                {
                    Properties.Settings.Default.DomeTilt = 0;
                }

                TargetLat = ViewLat = currentRaDec.Dec;
                TargetLong = ViewLong = RAtoViewLng(currentRaDec.RA);

            }

            if (SolarSystemTrack != SolarSystemObjects.Custom && SolarSystemTrack != SolarSystemObjects.Undefined)
            {
                viewCamera.ViewTarget = Planets.GetPlanetTargetPoint(SolarSystemTrack, ViewLat, ViewLong, 0);
            }

            RenderContext11.LightingEnabled = false;

            double localZoom = ZoomFactor * 20;
            Vector3d lookAt = new Vector3d(0, 0, -1);
            FovAngle = ((ZoomFactor/**16*/) / FOVMULT) / Math.PI * 180;

            // for constellations
            ViewPoint = Coordinates.RADecTo3d(this.RA, -this.Dec, 1.0);


            double distance = (Math.Min(1, (.5 * (ZoomFactor / 180)))) - 1 + 0.0001;

            RenderContext11.CameraPosition = new Vector3d(0, 0, distance);
            Vector3d lookUp = new Vector3d(Math.Sin(CameraRotate), Math.Cos(CameraRotate), 0.0001f);

            Matrix3d lookAtAdjust = Matrix3d.Identity;

            if ((Settings.Active.GalacticMode && !Settings.Active.LocalHorizonMode) && currentImageSetfield.DataSetType == ImageSetType.Sky)
            {
                if (!galMatInit)
                {
                    galacticMatrix = Matrix3d.Identity;
                    galacticMatrix.Multiply(Matrix3d.RotationY(-(90 - (17.7603329867975 * 15)) / 180.0 * Math.PI));
                    galacticMatrix.Multiply(Matrix3d.RotationX(-((-28.9361739586894)) / 180.0 * Math.PI));
                    galacticMatrix.Multiply(Matrix3d.RotationZ(((31.422052860102041270114993238783) - 90) / 180.0 * Math.PI));
                    galMatInit = true;
                }

                WorldMatrix = galacticMatrix;
                WorldMatrix.Multiply(Matrix3d.RotationY(((az)) / 180.0 * Math.PI));
                WorldMatrix.Multiply(Matrix3d.RotationX(-((alt)) / 180.0 * Math.PI));


                double[] gPoint = Coordinates.GalactictoJ2000(az, alt);

                this.RA = gPoint[0] / 15;
                this.Dec = gPoint[1];
                targetViewCamera.Lat = viewCamera.Lat;
                targetViewCamera.Lng = viewCamera.Lng;
            }
            else
            {
                // Show in Ecliptic

                WorldMatrix = Matrix3d.RotationY(-((this.ViewLong + 90) / 180.0 * Math.PI));
                WorldMatrix.Multiply(Matrix3d.RotationX(((-this.ViewLat) / 180.0 * Math.PI)));
            }


            RenderContext11.World = WorldMatrix;
            RenderContext11.WorldBase = WorldMatrix;



            lookAt.TransformCoordinate(lookAtAdjust);
            Matrix3d matHeadingPitchRoll;

            if (DomePreviewPopup.Active)
            {
                matHeadingPitchRoll =

                      Matrix3d.RotationY((DomePreviewPopup.Az / 180 * Math.PI)) *
                      Matrix3d.RotationX((DomePreviewPopup.Alt / 180 * Math.PI));
            }
            else
            {
                matHeadingPitchRoll = Matrix3d.Identity;

                if (config.UsingSgcWarpMap)
                {
                    matHeadingPitchRoll.Matrix = config.ProjectorMatrixSGC;
                }
                else
                {
                    matHeadingPitchRoll =
                    Matrix3d.RotationZ((config.Roll / 180 * Math.PI)) *
                    Matrix3d.RotationY((config.Heading / 180 * Math.PI)) *
                    Matrix3d.RotationX(((config.Pitch) / 180 * Math.PI));
                }
            }
            if (rift)
            {
                Matrix3d matRiftView = Matrix3d.Identity;

                var rotationQuaternion = SharpDXHelpers.ToQuaternion(eyeRenderPose[0].Orientation);
                matRiftView.Matrix11 = (SharpDX.Matrix.RotationQuaternion(rotationQuaternion) * SharpDX.Matrix.Scaling(1, 1, 1));
                RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * lookAtAdjust * matRiftView;
            }
            else
            {
                Matrix3d matNorth = Matrix3d.RotationY(faceSouth ? Math.PI : 0);

                RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * matNorth * DomeMatrix * matHeadingPitchRoll;
            }

            Vector3d temp = lookAt - RenderContext11.CameraPosition;
            temp.Normalize();
            ViewPoint = temp;

            // Set the near clip plane close enough that the sky dome isn't clipped
            double cameraZ = (Math.Min(1, (.5 * (ZoomFactor / 180)))) - 1 + 0.0001;
            m_nearPlane = (float)(1.0 + cameraZ) * 0.5f;

            back = 12;
            double aspect = config.Aspect;
            double top = m_nearPlane * 2 / ((1 / Math.Tan(config.UpFov / 180 * Math.PI))) / 2;
            double bottom = m_nearPlane * 2 / -(1 / Math.Tan(config.DownFov / 180 * Math.PI)) / 2;
            double right = m_nearPlane * 2 / (1 / Math.Tan((config.UpFov + config.DownFov) / 2 / 180 * Math.PI)) * aspect / 2;
            double left = -right;


            if (config.MultiChannelDome1)
            {
                ProjMatrix = Matrix3d.PerspectiveOffCenterLH(
                    left,
                    right,
                    bottom,
                    top,
                    m_nearPlane,
                    back);
            }
            else if (rift)
            {
                var fovPort = eyeTextures[renderType == RenderTypes.LeftEye ? 0 : 1].FieldOfView;
                var projMat = wrap.Matrix4f_Projection(fovPort, (float)m_nearPlane, (float)back, OVRTypes.ProjectionModifier.LeftHanded).ToMatrix();

                RenderContext11.PerspectiveFov = Math.Atan(fovPort.UpTan + fovPort.DownTan);
                projMat.Transpose();

                ProjMatrix = new Matrix3d();
                ProjMatrix.Matrix11 = projMat;
            }
            else
            {
                RenderContext11.View = RenderContext11.View * config.ViewMatrix;
                ProjMatrix = Matrix3d.PerspectiveFovLH((75f / 180f) * Math.PI, 1.777778, m_nearPlane, back);

            }

            if (rift)
            {
                if (renderType == RenderTypes.LeftEye)
                {

                    ProjMatrix.M31 += iod;
                }
                else
                {
                    ProjMatrix.M31 -= iod;
                }
            }


            RenderContext11.Projection = ProjMatrix;

            ViewMatrix = RenderContext11.View;
            RenderContext11.ViewBase = RenderContext11.View;
            MakeFrustum();
        }

#endif

        // video
        private void SetupMatricesVideoOverlay(double localZoomFactor)
        {
#if !WINDOWS_UWP
            if (config.MultiChannelDome1 || config.MultiProjector || DomePreviewPopup.Active)
            {
                SetupMatricesVideoOverlayMultiChannel(localZoomFactor);
                return;
            }
#endif
            WorldMatrix = Matrix3d.RotationY(-((0 + 90) / 180.0 * Math.PI));
            WorldMatrix.Multiply(Matrix3d.RotationX(((-0) / 180.0 * Math.PI)));

            double camLocal = 0;


            RenderContext11.World = WorldMatrix;
            RenderContext11.WorldBase = WorldMatrix;
            // altaz

            ViewPoint = Coordinates.RADecTo3d(0, 0, 1.0);


            FovAngle = ((360) / FOVMULT) / Math.PI * 180;
            RenderContext11.CameraPosition = new Vector3d(0.0, 0.0, 0.0);
#if !WINDOWS_UWP
            // This is for distance Calculation. For space everything is the same distance, so camera target is key.
            if (rift)
            {
                Matrix3d matRiftView = Matrix3d.Identity;
                var rotationQuaternion = SharpDXHelpers.ToQuaternion(eyeRenderPose[CurrentRenderType == RenderTypes.LeftEye ? 0 : 1].Orientation);
                matRiftView.Matrix11 = (SharpDX.Matrix.RotationQuaternion(rotationQuaternion) * SharpDX.Matrix.Scaling(1, 1, 1));
                RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, new Vector3d(0.0, 0.0, -1.0), new Vector3d(Math.Sin(camLocal), Math.Cos(camLocal), 0.0)) * matRiftView;
                RenderContext11.ViewBase = RenderContext11.View;
            }
            else
#endif
            {
                RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, new Vector3d(0.0, 0.0, -1.0), new Vector3d(Math.Sin(camLocal), Math.Cos(camLocal), 0.0));
                RenderContext11.ViewBase = RenderContext11.View;
            }
            m_nearPlane = 0f;
#if !WINDOWS_UWP
            if (multiMonClient)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((360/**16*/) / FOVMULT, (double)(monitorWidth * MonitorCountX) / (double)(monitorHeight * MonitorCountY), 0, -2.0);

            }
            else if (rift)
            {
                var fovPort = eyeTextures[CurrentRenderType == RenderTypes.LeftEye ? 0 : 1].FieldOfView;
                var projMat = wrap.Matrix4f_Projection(fovPort, (float)m_nearPlane, (float)back, OVRTypes.ProjectionModifier.LeftHanded).ToMatrix();

                RenderContext11.PerspectiveFov = Math.Atan(fovPort.UpTan + fovPort.DownTan);
                projMat.Transpose();

                ProjMatrix = new Matrix3d();
                ProjMatrix.Matrix11 = projMat;
            }
            else if (megaFrameDump)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((360/**16*/) / FOVMULT, (double)megaWidth / (double)megaHeight, 0, -2.0);

            }
            else
#endif
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((360/**16*/) / FOVMULT, (double)ViewWidth / (double)RenderContext11.Height, 0, -2.0);

            }


#if !WINDOWS_UWP
            if (multiMonClient)
            {
                ProjMatrix.M11 *= MonitorCountX * bezelSpacing;
                ProjMatrix.M22 *= MonitorCountY * bezelSpacing;
                ProjMatrix.M31 = (MonitorCountX - 1) - (MonitorX * bezelSpacing * 2);
                ProjMatrix.M32 = -((MonitorCountY - 1) - (MonitorY * bezelSpacing * 2));
            }
#endif
            RenderContext11.Projection = ProjMatrix;

            ViewMatrix = RenderContext11.View;

            MakeFrustum();

        }
#if !WINDOWS_UWP
        private void SetupMatricesVideoOverlayMultiChannel(double localZoomFactor)
        {

            RenderContext11.LightingEnabled = false;

            Vector3d lookAt = new Vector3d(-1, 0, 0);

            RenderContext11.CameraPosition = new Vector3d(0, 0, 0);
            Vector3d lookUp = new Vector3d(0, 1, 0);

            Matrix3d lookAtAdjust = Matrix3d.Identity;

            WorldMatrix = Matrix3d.Identity;

            RenderContext11.World = WorldMatrix;
            RenderContext11.WorldBase = WorldMatrix;



            lookAt.TransformCoordinate(lookAtAdjust);
            Matrix3d matHeadingPitchRoll;

            if (DomePreviewPopup.Active)
            {
                matHeadingPitchRoll =

                      Matrix3d.RotationY((DomePreviewPopup.Az / 180 * Math.PI)) *
                      Matrix3d.RotationX((DomePreviewPopup.Alt / 180 * Math.PI));
            }
            else
            {
                matHeadingPitchRoll = Matrix3d.Identity;

                if (config.UsingSgcWarpMap)
                {
                    matHeadingPitchRoll.Matrix = config.ProjectorMatrixSGC;
                }
                else
                {
                    matHeadingPitchRoll =
                        Matrix3d.RotationZ((config.Roll / 180 * Math.PI)) *
                        Matrix3d.RotationY((config.Heading / 180 * Math.PI)) *
                        Matrix3d.RotationX(((config.Pitch) / 180 * Math.PI));
                }
            }

            RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * DomeMatrix * matHeadingPitchRoll;

            m_nearPlane = .000000001;
            back = 12;
            double aspect = config.Aspect;
            double top = m_nearPlane * 2 / ((1 / Math.Tan(config.UpFov / 180 * Math.PI))) / 2;
            double bottom = m_nearPlane * 2 / -(1 / Math.Tan(config.DownFov / 180 * Math.PI)) / 2;
            double right = m_nearPlane * 2 / (1 / Math.Tan((config.UpFov + config.DownFov) / 2 / 180 * Math.PI)) * aspect / 2;
            double left = -right;


            if (config.MultiChannelDome1)
            {
                ProjMatrix = Matrix3d.PerspectiveOffCenterLH(
                    left,
                    right,
                    bottom,
                    top,
                    m_nearPlane,
                    back);
            }
            else
            {
                RenderContext11.View = RenderContext11.View * config.ViewMatrix;

                ProjMatrix = Matrix3d.PerspectiveFovLH((75f / 180f) * Math.PI, 1.777778, m_nearPlane, back);

            }

            RenderContext11.Projection = ProjMatrix;

            ViewMatrix = RenderContext11.View;
            RenderContext11.ViewBase = RenderContext11.View;
            MakeFrustum();
        }
#endif

        public void MakeFrustum()
        {
            RenderContext11.UpdateProjectionConstantBuffers();
            RenderContext11.MakeFrustum();
        }


        public double targetHeight = 1;
        public double targetAltitude = 0;

        public double TargetAltitude
        {
            get { return targetAltitude; }
            set { targetAltitude = value; }
        }


        public double fovLocal = (Math.PI / 4.0);

        private void SetupMatricesLand11(RenderTypes renderType)
        {
            WorldMatrix = Matrix3d.RotationY(((this.ViewLong + 90f) / 180f * Math.PI));
            WorldMatrix.Multiply(Matrix3d.RotationX(((-this.ViewLat) / 180f * Math.PI)));
            RenderContext11.World = WorldMatrix;
            RenderContext11.WorldBase = WorldMatrix;


            double distance = 0;
            if (currentImageSetfield.IsMandelbrot)
            {

                distance = (4.0 * (ZoomFactor / 180)) + 0.00000000000000000000000000000000000000001;
            }
            else
            {

                distance = (4.0 * (ZoomFactor / 180)) + 0.000001;
            }

            if (Settings.Active.ShowElevationModel)
            {
                targetAltitude = GetScaledAltitudeForLatLong(ViewLat, ViewLong);
                double heightNow = 1 + targetAltitude;
                targetAltitude *= RenderContext11.NominalRadius;
                if ((double.IsNaN(heightNow)))
                {
                    heightNow = 0;
                }

                if (targetHeight < heightNow)
                {
                    targetHeight = (((targetHeight * 2) + heightNow) / 3);
                }
                else
                {
                    targetHeight = (((targetHeight * 9) + heightNow) / 10);
                }
                if (double.IsNaN(targetHeight))
                {
                    targetHeight = 0;
                }
#if !WINDOWS_UWP
                if (config.MultiChannelDome1 || config.MultiProjector)
                {
                    targetHeight = heightNow = NetControl.focusAltitude;
                }
#endif
            }
            else
            {
                targetAltitude = 0;
                targetHeight = 1;
            }
            double rotLocal = CameraRotate;
            if (!rift)
            {
                if (renderType == RenderTypes.RightEye)
                {
                    rotLocal -= .008;
                }
                if (renderType == RenderTypes.LeftEye)
                {
                    rotLocal += .008;
                }
            }

            RenderContext11.CameraPosition = new Vector3d(
                (Math.Sin(rotLocal) * Math.Sin(CameraAngle) * distance),
                (Math.Cos(rotLocal) * Math.Sin(CameraAngle) * distance),
                (-targetHeight - (Math.Cos(CameraAngle) * distance)));
            cameraTarget = new Vector3d(0.0f, 0.0f, -targetHeight);

            double camHeight = RenderContext11.CameraPosition.Length();
            if (Tile.GrayscaleStyle)
            {
                if (currentImageSetfield.Projection == ProjectionType.Toast && (currentImageSetfield.MeanRadius > 0 && currentImageSetfield.MeanRadius < 4000000))
                {
                    int val = (int)Math.Max(0, Math.Min(255, 255 - Math.Min(255, (camHeight - 1) * 5000)));
                    SkyColor = SysColor.FromArgb(255, (byte)(213 * val / 255), (byte)(165 * val / 255), (byte)(118 * val / 255));
                }
                else if (currentImageSetfield.DataSetType == ImageSetType.Earth)
                {
                    SkyColor = SysColor.FromArgb(255, 184, 184, 184);
                }
                else
                {
                    SkyColor = SysColor.FromArgb(255, 0, 0, 0);
                }
            }
            else
            {
                if (currentImageSetfield.ReferenceFrame == "Mars" && Settings.Active.ShowEarthSky)
                {
                    int val = (int)Math.Max(0, Math.Min(255, 255 - Math.Min(255, (camHeight - 1) * 5000)));
                    SkyColor = SysColor.FromArgb(255, (byte)(213 * val / 255), (byte)(165 * val / 255), (byte)(118 * val / 255));
                }
                else if (currentImageSetfield.DataSetType == ImageSetType.Earth && Settings.Active.ShowEarthSky)
                {
                    int val = (int)Math.Max(0, Math.Min(255, 255 - Math.Min(255, (camHeight - 1) * 5000)));
                    SkyColor = SysColor.FromArgb(255, (byte)(val / 3), (byte)(val / 3), (byte)(val));
                }
                else
                {
                    SkyColor = SysColor.FromArgb(255, 0, 0, 0);
                }
            }

            Matrix3d trackingMatrix = Matrix3d.Identity;

            if (config.MultiChannelGlobe)
            {
                // Move the camera to some fixed distance from the globe
                RenderContext11.CameraPosition *= 50.0 / RenderContext11.CameraPosition.Length();

                // Modify camera position in globe mode
                Matrix3d globeCameraRotation =
                    Matrix3d.RotationZ((config.Roll / 180 * Math.PI)) *
                    Matrix3d.RotationY((config.Heading / 180 * Math.PI)) *
                    Matrix3d.RotationX(((config.Pitch) / 180 * Math.PI));
                RenderContext11.CameraPosition = globeCameraRotation.Transform(RenderContext11.CameraPosition);
                cameraTarget = globeCameraRotation.Transform(cameraTarget);
                RenderContext11.View = Matrix3d.LookAtLH(
                    RenderContext11.CameraPosition,
                    cameraTarget,
                    new Vector3d(Math.Sin(rotLocal) * Math.Cos(CameraAngle), Math.Cos(rotLocal) * Math.Cos(CameraAngle), Math.Sin(CameraAngle)));
            }
            else if (config.MultiChannelDome1)
            {
                Matrix3d matHeadingPitchRoll = Matrix3d.Identity;

                if (config.UsingSgcWarpMap)
                {
                    matHeadingPitchRoll.Matrix = config.ProjectorMatrixSGC;
                }
                else
                {
                    matHeadingPitchRoll =
                        Matrix3d.RotationZ((config.Roll / 180 * Math.PI)) *
                        Matrix3d.RotationY((config.Heading / 180 * Math.PI)) *
                        Matrix3d.RotationX(((config.Pitch) / 180 * Math.PI));
                }
                RenderContext11.View = Matrix3d.LookAtLH(
                            RenderContext11.CameraPosition,
                            cameraTarget,
                            new Vector3d(Math.Sin(rotLocal) * Math.Cos(CameraAngle), Math.Cos(rotLocal) * Math.Cos(CameraAngle), Math.Sin(CameraAngle)))
                            * DomeMatrix
                            * matHeadingPitchRoll;
                RenderContext11.ViewBase = RenderContext11.View;
            }
            else
            {

                Vector3d lookUp = new Vector3d(Math.Sin(rotLocal) * Math.Cos(CameraAngle), Math.Cos(rotLocal) * Math.Cos(CameraAngle), Math.Sin(CameraAngle));
#if !WINDOWS_UWP
                if (DomePreviewPopup.Active)
                {
                    Matrix3d matDomePreview =
                         Matrix3d.RotationY((DomePreviewPopup.Az / 180 * Math.PI)) *
                         Matrix3d.RotationX((DomePreviewPopup.Alt / 180 * Math.PI));
                    RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, cameraTarget, lookUp) * Matrix3d.RotationX(((-config.TotalDomeTilt) / 180 * Math.PI)) * matDomePreview;
                }
                else if (rift)
                {
                    double amount = distance / 100;
                    Matrix3d stereoTranslate = Matrix3d.Translation(renderType == RenderTypes.LeftEye ? amount : -amount, 0, 0);
                    Matrix3d matRiftView = Matrix3d.Identity;

                    if (rift)
                    {
                        SharpDX.Vector3 pos = eyeRenderPose[renderType == RenderTypes.LeftEye ? 0 : 1].Position.ToVector3();
                        amount *= 10;
                        stereoTranslate = Matrix3d.Translation(-pos.X * amount, -pos.Y * amount, pos.Z * amount);

                        var rotationQuaternion = SharpDXHelpers.ToQuaternion(eyeRenderPose[renderType == RenderTypes.LeftEye ? 0 : 1].Orientation);
                        matRiftView.Matrix11 = (SharpDX.Matrix.RotationQuaternion(rotationQuaternion) * SharpDX.Matrix.Scaling(1, 1, 1));


                        //float yaw = 0;
                        //float pitch = 0;
                        //float roll = 0;
                        //SharpDXHelpers.ToQuaternion(eyeRenderPose[renderType == RenderTypes.LeftEye ? 0 : 1].Orientation).GetEulerAngles(out yaw, out pitch, out roll);

                        // matRiftView = Matrix3d.RotationY(yaw) * Matrix3d.RotationX(pitch) * Matrix3d.RotationZ(-roll);
                    }
                    RenderContext11.View = trackingMatrix * Matrix3d.LookAtLH(RenderContext11.CameraPosition, cameraTarget, lookUp) * matRiftView * stereoTranslate;
                }
                else
#endif
                {
                    RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, cameraTarget, lookUp) * Matrix3d.Translation(HeadPosition);
                }

                if (multiMonClient)
                {
                    RenderContext11.View = RenderContext11.View * Matrix3d.RotationY((config.Heading / 180 * Math.PI));
                }


                RenderContext11.ViewBase = RenderContext11.View;


            }

            back = Math.Sqrt((distance + 1f) * (distance + 1f) - 1);
            back = Math.Max(.5, back);

            if (Properties.Settings.Default.EarthCutawayView.State)
            {
                back = 20;
            }
            m_nearPlane = distance * .05f;

#if !WINDOWS_UWP
            if (config.MultiChannelGlobe)
            {
                m_nearPlane = RenderContext11.CameraPosition.Length() - 2.0;
                back = m_nearPlane + 4.0;
                ProjMatrix = Matrix3d.OrthoLH(config.Aspect * 2.0, 2.0, m_nearPlane, back);
            }
            else if (config.MultiChannelDome1)
            {
                double aspect = config.Aspect;
                double top = m_nearPlane * 2 / ((1 / Math.Tan(config.UpFov / 180 * Math.PI))) / 2;
                double bottom = m_nearPlane * 2 / -(1 / Math.Tan(config.DownFov / 180 * Math.PI)) / 2;
                double right = m_nearPlane * 2 / (1 / Math.Tan((config.UpFov + config.DownFov) / 2 / 180 * Math.PI)) * aspect / 2;
                double left = -right;

                ProjMatrix = Matrix3d.PerspectiveOffCenterLH(
                    left,
                    right,
                    bottom,
                    top,
                    m_nearPlane,
                    back);


            }
            else if (config.MultiProjector)
            {
                RenderContext11.View = RenderContext11.View * config.ViewMatrix;
                ProjMatrix = Matrix3d.PerspectiveFovLH((75f / 180f) * Math.PI, 1.777778, m_nearPlane, back);
                RenderContext11.ViewBase = RenderContext11.View;

            }
            else if (multiMonClient)
            {
                double fov = (((config.UpFov + config.DownFov) / 2 / 180 * Math.PI));
                if (fov == 0)
                {
                    fov = (Math.PI / 4.0);
                }

                m_nearPlane = distance * .05f;
                ProjMatrix = Matrix3d.PerspectiveFovLH(fov, (monitorWidth * MonitorCountX) / (monitorHeight * MonitorCountY), m_nearPlane, back);
            }
            else if (rift)
            {
                var fovPort = eyeTextures[renderType == RenderTypes.LeftEye ? 0 : 1].FieldOfView;
                var projMat = wrap.Matrix4f_Projection(fovPort, (float)m_nearPlane, (float)back, OVRTypes.ProjectionModifier.LeftHanded).ToMatrix();

                RenderContext11.PerspectiveFov = Math.Atan(fovPort.UpTan + fovPort.DownTan);
                projMat.Transpose();

                ProjMatrix = new Matrix3d();
                ProjMatrix.Matrix11 = projMat;
            }
            else if (megaFrameDump)
            {

                m_nearPlane = distance * .05f;
                ProjMatrix = Matrix3d.PerspectiveFovLH(fovLocal, megaWidth / megaHeight, m_nearPlane, back);
                RenderContext11.PerspectiveFov = fovLocal;
            }
            else
#endif
            {

                m_nearPlane = distance * .05f;
                ProjMatrix = Matrix3d.PerspectiveFovLH(fovLocal, (double)ViewWidth / (double)RenderContext11.Height, m_nearPlane, back);
                RenderContext11.PerspectiveFov = fovLocal;
            }

#if !WINDOWS_UWP
            if (multiMonClient && !config.MultiChannelDome1 && !config.MultiProjector && !config.MultiChannelGlobe)
            {
                ProjMatrix.M11 *= MonitorCountX * bezelSpacing;
                ProjMatrix.M22 *= MonitorCountY * bezelSpacing;
                ProjMatrix.M31 = (MonitorCountX - 1) - (MonitorX * bezelSpacing * 2);
                ProjMatrix.M32 = -((MonitorCountY - 1) - (MonitorY * bezelSpacing * 2));
            }

            if (rift)
            {
                if (renderType == RenderTypes.LeftEye)
                {

                    ProjMatrix.M31 += iod;
                }
                else
                {
                    ProjMatrix.M31 -= iod;
                }
            }

#endif
            RenderContext11.Projection = ProjMatrix;

            colorBlend = 1 / distance;

            ViewMatrix = RenderContext11.View;


            MakeFrustum();
        }


        public double GetScaledAltitudeForLatLong(double viewLat, double viewLong)
        {
            IImageSet layer = currentImageSetfield;

            if (layer == null)
            {
                return 0;
            }

            int maxX = GetTilesXForLevel(layer, layer.BaseLevel);
            int maxY = GetTilesYForLevel(layer, layer.BaseLevel);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    Tile tile = TileCache.GetTile(layer.BaseLevel, x, y, layer, null);
                    if (tile != null)
                    {
                        if (tile.IsPointInTile(viewLat, viewLong))
                        {
                            return tile.GetSurfacePointAltitude(viewLat, viewLong, false);
                        }
                    }
                }
            }
            return 0;
        }

        public double GetAltitudeForLatLong(double viewLat, double viewLong)
        {
            IImageSet layer = currentImageSetfield;

            if (layer == null)
            {
                return 0;
            }

            int maxX = GetTilesXForLevel(layer, layer.BaseLevel);
            int maxY = GetTilesYForLevel(layer, layer.BaseLevel);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    Tile tile = TileCache.GetTile(layer.BaseLevel, x, y, layer, null);
                    if (tile != null)
                    {
                        if (tile.IsPointInTile(viewLat, viewLong))
                        {
                            return tile.GetSurfacePointAltitude(viewLat, viewLong, true);
                        }
                    }
                }
            }
            return 0;
        }

        public double GetAltitudeForLatLongNow(double viewLat, double viewLong)
        {
            IImageSet layer = currentImageSetfield;

            if (layer == null)
            {
                return 0;
            }

            int maxX = GetTilesXForLevel(layer, layer.BaseLevel);
            int maxY = GetTilesYForLevel(layer, layer.BaseLevel);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    Tile tile = TileCache.GetTile(layer.BaseLevel, x, y, layer, null);
                    if (tile != null)
                    {
                        if (tile.IsPointInTile(viewLat, viewLong))
                        {
                            return tile.GetSurfacePointAltitudeNow(viewLat, viewLong, true, Tile.lastDeepestLevel + 1);
                        }
                    }
                }
            }
            return 0;
        }

        public double GetAltitudeForLatLongForPlanet(int planetID, double viewLat, double viewLong)
        {

            IImageSet layer = GetImagesetByName(Planets.GetNameFrom3dId(planetID));

            if (layer == null)
            {
                return 0;
            }

            int maxX = GetTilesXForLevel(layer, layer.BaseLevel);
            int maxY = GetTilesYForLevel(layer, layer.BaseLevel);

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    Tile tile = TileCache.GetTile(layer.BaseLevel, x, y, layer, null);
                    if (tile != null)
                    {
                        if (tile.IsPointInTile(viewLat, viewLong))
                        {
                            return tile.GetSurfacePointAltitude(viewLat, viewLong, true);
                        }
                    }
                }
            }
            return 0;
        }

        private void SetupMatricesLandDome(RenderTypes renderType)
        {
            FovAngle = 60;

            RenderContext11.LightingEnabled = false;

            double localZoom = ZoomFactor * 20;
            double distance = (4.0 * (ZoomFactor / 180)) + 0.000001;


            Vector3d lookAt = new Vector3d(0.0f, 0.0f, -targetHeight);

            if (Settings.Active.ShowElevationModel)
            {
                double heightNow = 1 + GetScaledAltitudeForLatLong(ViewLat, ViewLong);
                if (targetHeight < heightNow)
                {
                    targetHeight = (((targetHeight * 2) + heightNow) / 3);
                }
                else
                {
                    targetHeight = (((targetHeight * 9) + heightNow) / 10);
                }

            }
            else
            {
                targetHeight = 1;
            }

            double rotLocal = CameraRotate;

            if (renderType == RenderTypes.RightEye)
            {
                rotLocal -= .008;
            }
            if (renderType == RenderTypes.LeftEye)
            {
                rotLocal += .008;
            }

            RenderContext11.CameraPosition = new Vector3d(
                (Math.Sin(rotLocal) * Math.Sin(CameraAngle) * distance),
                (Math.Cos(rotLocal) * Math.Sin(CameraAngle) * distance),
                (-targetHeight - (Math.Cos(CameraAngle) * distance)));

            Matrix3d lookAtAdjust = Matrix3d.Identity;

            Vector3d lookUp = new Vector3d(Math.Sin(rotLocal) * Math.Cos(CameraAngle), Math.Cos(rotLocal) * Math.Cos(CameraAngle), Math.Sin(CameraAngle));

            Matrix3d cubeMat = Matrix3d.Identity;

            switch (renderType)
            {
                case RenderTypes.DomeUp:
                    cubeMat = Matrix3d.RotationX((Math.PI / 2));
                    break;
                case RenderTypes.DomeLeft:
                    cubeMat = Matrix3d.RotationY((Math.PI / 2));
                    break;
                case RenderTypes.DomeRight:
                    cubeMat = Matrix3d.RotationY(-(Math.PI / 2));
                    break;
                case RenderTypes.DomeFront:
                    break;
                case RenderTypes.DomeBack:
                    cubeMat = Matrix3d.RotationY((Math.PI));
                    break;
                default:
                    break;
            }
            double camHeight = RenderContext11.CameraPosition.Length();

            if (currentImageSetfield.Projection == ProjectionType.Toast && (currentImageSetfield.MeanRadius > 0 && currentImageSetfield.MeanRadius < 4000000))
            {
                int val = (int)Math.Max(0, Math.Min(255, 255 - Math.Min(255, (camHeight - 1) * 5000)));
                SkyColor = SysColor.FromArgb(255, (byte)(213 * val / 255), (byte)(165 * val / 255), (byte)(118 * val / 255));
            }
            else if (currentImageSetfield.DataSetType == ImageSetType.Earth && Settings.Active.ShowEarthSky)
            {
                int val = (int)Math.Max(0, Math.Min(255, 255 - Math.Min(255, (camHeight - 1) * 5000)));
                SkyColor = SysColor.FromArgb(255, (byte)(val / 3), (byte)(val / 3), (byte)(val));
            }
            else
            {
                SkyColor = SysColor.Black;
            }

            WorldMatrix = Matrix3d.RotationY(((this.ViewLong + 90f) / 180f * Math.PI));
            WorldMatrix.Multiply(Matrix3d.RotationX(((-this.ViewLat) / 180f * Math.PI)));
            RenderContext11.World = WorldMatrix;
            RenderContext11.WorldBase = WorldMatrix;

            if (config.MultiChannelDome1)
            {
                Matrix3d matHeadingPitchRoll = Matrix3d.Identity;

                if (config.UsingSgcWarpMap)
                {
                    matHeadingPitchRoll.Matrix = config.ProjectorMatrixSGC;
                }
                else
                {
                    matHeadingPitchRoll =
                    Matrix3d.RotationZ((config.Roll / 180 * Math.PI)) *
                    Matrix3d.RotationY((config.Heading / 180 * Math.PI)) *
                    Matrix3d.RotationX(((config.Pitch) / 180 * Math.PI));
                }
            }
            else
            {
                RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * DomeMatrix * cubeMat;
            }

            Vector3d temp = lookAt - RenderContext11.CameraPosition;
            temp.Normalize();
            ViewPoint = temp;


            back = Math.Sqrt((distance + 1f) * (distance + 1f) - 1);
            m_nearPlane = distance * .1f;


            ProjMatrix = Matrix3d.PerspectiveFovLH((Math.PI / 2.0), 1.0f, m_nearPlane, back);
            if (config.MultiChannelDome1)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH(((config.UpFov + config.DownFov) / 180 * Math.PI), (double)ViewWidth / (double)RenderContext11.Height, m_nearPlane, back);
            }

            else if (multiMonClient)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((Math.PI / 2.0), 1.0f, m_nearPlane, back);
                RenderContext11.PerspectiveFov = (Math.PI / 2.0);
            }

            RenderContext11.Projection = ProjMatrix;


            ViewMatrix = RenderContext11.View;
            RenderContext11.ViewBase = RenderContext11.View;

            MakeFrustum();
        }


        public SolarSystemObjects SolarSystemTrack
        {
            get
            {
                return viewCamera.Target;
            }
            set
            {
                viewCamera.Target = value;
            }
        }
        public double SolarSystemCameraDistance
        {
            get
            {
                return (4.0 * (ZoomFactor / 9)) + 0.000001;
            }

        }



        public string TrackingFrame
        {
            get { return viewCamera.TargetReferenceFrame; }
            set { viewCamera.TargetReferenceFrame = value; }
        }





        bool useSolarSystemTilt = true;

        public CameraParameters CustomTrackingParams = new CameraParameters();

        Vector3d cameraOffset = new Vector3d();


        private void SetupMatricesSolarSystem11(bool forStars, RenderTypes renderType)
        {

            if (SandboxMode)
            {
                if (SolarSystemTrack != SolarSystemObjects.Custom && SolarSystemTrack != SolarSystemObjects.Undefined)
                {
                    viewCamera.ViewTarget = new Vector3d();
                }
            }
            else
            {
                if (SolarSystemTrack != SolarSystemObjects.Custom && SolarSystemTrack != SolarSystemObjects.Undefined)
                {
                    viewCamera.ViewTarget = Planets.GetPlanetTargetPoint(SolarSystemTrack, ViewLat, ViewLong, 0);
                }
            }


            double cameraDistance = SolarSystemCameraDistance;



            Matrix3d trackingMatrix = Matrix3d.Identity;
            cameraDistance -= 0.000001;

            double DistanceOffsetPercent = 0;
#if !WINDOWS_UWP
            if (NetControl.DistanceOffsetPercent == NetControl.LastDistanceOffsetPercent)
            {
                //no distnace update since we sample 30 FPS but render 60FPS
                NetControl.DistanceOffsetPercent += NetControl.DeltaDistanceOffset;
            }

            DistanceOffsetPercent = NetControl.DistanceOffsetPercent;
#endif


            cameraDistance += cameraDistance * DistanceOffsetPercent;


            bool activeTrackingFrame = false;

            if (SolarSystemTrack == SolarSystemObjects.Custom && !string.IsNullOrEmpty(TrackingFrame))
            {
                activeTrackingFrame = true;
                viewCamera.ViewTarget = LayerManager.GetFrameTarget(RenderContext11, TrackingFrame, out trackingMatrix);
            }
            else if (!string.IsNullOrEmpty(TrackingFrame))
            {
                TrackingFrame = "";
            }


            Vector3d center = viewCamera.ViewTarget;
            Vector3d lightPosition = -center;

            double localZoom = ZoomFactor * 20;
            Vector3d lookAt = new Vector3d(0, 0, 0);

            Matrix3d viewAdjust = Matrix3d.Identity;
            viewAdjust.Multiply(Matrix3d.RotationX(((-this.ViewLat) / 180f * Math.PI)));
            viewAdjust.Multiply(Matrix3d.RotationY(((-this.ViewLong) / 180f * Math.PI)));

            Matrix3d lookAtAdjust = Matrix3d.Identity;


            bool dome = false;

            Vector3d lookUp;





            if (useSolarSystemTilt && !SandboxMode)
            {
                double angle = CameraAngle;
                if (cameraDistance > 0.0008)
                {
                    angle = 0;
                }
                else if (cameraDistance > 0.00001)
                {
                    double val = Math.Min(1.903089987, Math.Log(cameraDistance, 10) + 5) / 1.903089987;

                    angle = angle * Math.Max(0, 1 - val);
                }



                RenderContext11.CameraPosition = new Vector3d(
                (Math.Sin(-CameraRotate) * Math.Sin(angle) * cameraDistance),
                (Math.Cos(-CameraRotate) * Math.Sin(angle) * cameraDistance),
                ((Math.Cos(angle) * cameraDistance)));
                lookUp = new Vector3d(Math.Sin(-CameraRotate), Math.Cos(-CameraRotate), 0.00001f);
            }
            else
            {
                RenderContext11.CameraPosition = new Vector3d(0, 0, ((cameraDistance)));

                lookUp = new Vector3d(Math.Sin(-CameraRotate), Math.Cos(-CameraRotate), 0.0001f);
            }


            RenderContext11.CameraPosition.TransformCoordinate(viewAdjust);

            cameraOffset = RenderContext11.CameraPosition;

            cameraOffset.TransformCoordinate(Matrix3d.Invert(trackingMatrix));



            lookUp.TransformCoordinate(viewAdjust);


#if !WINDOWS_UWP
            switch (renderType)
            {
                case RenderTypes.DomeUp:
                    dome = true;
                    lookAtAdjust.Multiply(Matrix3d.RotationX(Math.PI / 2));
                    break;
                case RenderTypes.DomeLeft:
                    dome = true;
                    lookAtAdjust.Multiply(Matrix3d.RotationY(Math.PI / 2));
                    break;
                case RenderTypes.DomeRight:
                    dome = true;
                    lookAtAdjust.Multiply(Matrix3d.RotationY(-Math.PI / 2));
                    break;
                case RenderTypes.DomeFront:
                    dome = true;
                    break;
                case RenderTypes.DomeBack:
                    lookAtAdjust.Multiply(Matrix3d.RotationY(Math.PI));
                    dome = true;
                    break;
                default:
                    break;
            }
#endif
            WorldMatrix = Matrix3d.Identity;
            RenderContext11.World = WorldMatrix;
            RenderContext11.WorldBase = RenderContext11.World;
#if !WINDOWS_UWP
            if (config.MultiChannelDome1)
            {
                Matrix3d matHeadingPitchRoll = Matrix3d.Identity;

                if (config.UsingSgcWarpMap)
                {
                    matHeadingPitchRoll.Matrix = config.ProjectorMatrixSGC;
                }
                else
                {
                    matHeadingPitchRoll =
                    Matrix3d.RotationZ((config.Roll / 180 * Math.PI)) *
                    Matrix3d.RotationY((config.Heading / 180 * Math.PI)) *
                    Matrix3d.RotationX(((config.Pitch) / 180 * Math.PI));
                }

                RenderContext11.View = trackingMatrix * Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * DomeMatrix * matHeadingPitchRoll;
            }
            else
            {
                if (Settings.DomeView)
                {
                    RenderContext11.View = trackingMatrix * Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * DomeMatrix * lookAtAdjust;

                }
                else
                {
                    if (DomePreviewPopup.Active && !dome)
                    {
                        Matrix3d matDomePreview =
                             Matrix3d.RotationY((DomePreviewPopup.Az / 180 * Math.PI)) *
                             Matrix3d.RotationX((DomePreviewPopup.Alt / 180 * Math.PI));
                        RenderContext11.View = trackingMatrix * Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * DomeMatrix * matDomePreview;
                    }
                    else if (rift || renderType == RenderTypes.RightEye || renderType == RenderTypes.LeftEye)
                    {
                        double amount = cameraDistance / 100;
                        Matrix3d stereoTranslate = Matrix3d.Translation(renderType == RenderTypes.LeftEye ? amount : -amount, 0, 0);
                        Matrix3d matRiftView = Matrix3d.Identity;
                        if (rift)
                        {
                            SharpDX.Vector3 pos = eyeRenderPose[renderType == RenderTypes.LeftEye ? 0 : 1].Position.ToVector3();
                            amount *= 10;
                            var pose = this.trackingState.HeadPose.ThePose.Position;
                            stereoTranslate = Matrix3d.Translation((-pos.X + pose.X * 10) * amount, (pos.Y + pose.Y * 10) * amount, (-pos.Z + pose.Z * 10) * amount);

                            var rotationQuaternion = SharpDXHelpers.ToQuaternion(eyeRenderPose[renderType == RenderTypes.LeftEye ? 0 : 1].Orientation);
                            matRiftView.Matrix11 = (SharpDX.Matrix.RotationQuaternion(rotationQuaternion) * SharpDX.Matrix.Scaling(1, 1, 1));
                        }
                        RenderContext11.View = trackingMatrix * Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * lookAtAdjust * matRiftView * stereoTranslate;
                    }
                    else
#endif
                    {
                        Matrix3d swingTranslation;

                        if (DistanceOffsetPercent < 0)
                        {
                            swingTranslation = Matrix3d.Translation(0, -(1 - Math.Cos(DistanceOffsetPercent)) * Properties.Settings.Default.SwingScaleFront * SolarSystemCameraDistance / 4, 0);
                        }
                        else
                        {
                            swingTranslation = Matrix3d.Translation(0, -(1 - Math.Cos(DistanceOffsetPercent)) * Properties.Settings.Default.SwingScaleBack * SolarSystemCameraDistance / 4, 0);
                        }
                        var tt = Properties.Settings.Default.SwingScaleFront;

                        RenderContext11.View = trackingMatrix * Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * lookAtAdjust * swingTranslation;
                    }
#if !WINDOWS_UWP
            if (multiMonClient)
                    {
                        RenderContext11.View = RenderContext11.View * Matrix3d.RotationY((config.Heading / 180 * Math.PI));
                    }

                }
            }
#endif
            RenderContext11.ViewBase = RenderContext11.View;


            Vector3d temp = lookAt - RenderContext11.CameraPosition;
            temp.Normalize();
            temp = Vector3d.TransformCoordinate(temp, trackingMatrix);
            temp.Normalize();
            ViewPoint = temp;



            if (activeTrackingFrame)
            {
                Vector3d atfCamPos = RenderContext11.CameraPosition;
                Vector3d atfLookAt = lookAt;
                Vector3d atfLookUp = lookUp;
                Matrix3d mat = trackingMatrix;
                mat.Invert();

                atfCamPos.TransformCoordinate(mat);
                atfLookAt.TransformCoordinate(mat);
                atfLookUp.TransformCoordinate(mat);
                atfLookAt.Normalize();
                atfLookUp.Normalize();

                CustomTrackingParams.Angle = 0;
                CustomTrackingParams.Rotation = 0;
                CustomTrackingParams.DomeAlt = viewCamera.DomeAlt;
                CustomTrackingParams.DomeAz = viewCamera.DomeAz;
                CustomTrackingParams.TargetReferenceFrame = "";
                CustomTrackingParams.ViewTarget = viewCamera.ViewTarget;
                CustomTrackingParams.Zoom = viewCamera.Zoom;
                CustomTrackingParams.Target = SolarSystemObjects.Custom;


                Vector3d atfLook = atfCamPos - atfLookAt;
                atfLook.Normalize();



                Coordinates latlng = Coordinates.CartesianToSpherical2(atfLook);
                CustomTrackingParams.Lat = latlng.Lat;
                CustomTrackingParams.Lng = latlng.Lng - 90;

                Vector3d up = Coordinates.GeoTo3dDouble(latlng.Lat + 90, latlng.Lng - 90);
                Vector3d left = Vector3d.Cross(atfLook, up);

                double dotU = Math.Acos(Vector3d.Dot(atfLookUp, up));
                double dotL = Math.Acos(Vector3d.Dot(atfLookUp, left));

                CustomTrackingParams.Rotation = dotU;// -Math.PI / 2;
            }


            double radius = Planets.GetAdjustedPlanetRadius((int)SolarSystemTrack);

            if (cameraDistance < radius * 2.0 && !forStars)
            {
                m_nearPlane = cameraDistance * 0.03;

                m_nearPlane = Math.Max(m_nearPlane, .00000000001);
                back = 1900;
            }
            else
            {
                if (forStars)
                {
                    back = 900056;
                    back = cameraDistance > 900056 ? cameraDistance * 3 : 900056;
                    m_nearPlane = .00003f;

                }
                else
                {
                    // Github Issue #149
                    // Orbits past Neptune are clipping 

                    back = cameraDistance > 950 ? cameraDistance + 2500 : 1900;

                    if (Settings.Active.SolarSystemScale < 13)
                    {
                        m_nearPlane = (float)Math.Min(cameraDistance * 0.03, 0.01);
                    }
                    else
                    {
                        m_nearPlane = .001f;
                    }
                }
            }
#if !WINDOWS_UWP
            if (config.MultiChannelDome1)
            {
                double aspect = config.Aspect;
                double top = m_nearPlane * 2 / ((1 / Math.Tan(config.UpFov / 180 * Math.PI))) / 2;
                double bottom = m_nearPlane * 2 / -(1 / Math.Tan(config.DownFov / 180 * Math.PI)) / 2;
                double right = m_nearPlane * 2 / (1 / Math.Tan((config.UpFov + config.DownFov) / 2 / 180 * Math.PI)) * aspect / 2;
                double left = -right;


                ProjMatrix = Matrix3d.PerspectiveOffCenterLH(
                    left,
                    right,
                    bottom,
                    top,
                    m_nearPlane,
                    back);
            }
            else if (config.MultiProjector)
            {

                RenderContext11.View = RenderContext11.View * config.ViewMatrix;

                ProjMatrix = Matrix3d.PerspectiveFovLH((75f / 180f) * Math.PI, 1.777778, m_nearPlane, back);

                RenderContext11.ViewBase = RenderContext11.View;
            }
            else if (multiMonClient && !dome)
            {
                double fov = (((config.UpFov + config.DownFov) / 2 / 180 * Math.PI));
                if (fov == 0)
                {
                    fov = (Math.PI / 4.0);
                }
                ProjMatrix = Matrix3d.PerspectiveFovLH((Math.PI / 4.0), (double)(monitorWidth * MonitorCountX) / ((double)monitorHeight * (double)MonitorCountY), m_nearPlane, back);
            }
            else if (dome)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((Math.PI / 2.0), 1.0f, m_nearPlane, back);
            }

            else if (rift)
            {
                var fovPort = eyeTextures[renderType == RenderTypes.LeftEye ? 0 : 1].FieldOfView;
                var projMat = wrap.Matrix4f_Projection(fovPort, (float)m_nearPlane, (float)back, OVRTypes.ProjectionModifier.LeftHanded).ToMatrix();

                RenderContext11.PerspectiveFov = Math.Atan(fovPort.UpTan + fovPort.DownTan);
                projMat.Transpose();

                ProjMatrix = new Matrix3d();
                ProjMatrix.Matrix11 = projMat;
            }
            else if (megaFrameDump)
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((fovLocal), megaWidth / megaHeight, m_nearPlane, back);
                RenderContext11.PerspectiveFov = fovLocal;
            }
            else
#endif
            {
                ProjMatrix = Matrix3d.PerspectiveFovLH((fovLocal), (double)ViewWidth / (double)RenderContext11.Height, m_nearPlane, back);
                RenderContext11.PerspectiveFov = fovLocal;
                if (Properties.Settings.Default.PerspectiveOffsetX != 0 || Properties.Settings.Default.PerspectiveOffsetY != 0)
                {
                    ProjMatrix.M31 += Properties.Settings.Default.PerspectiveOffsetX;
                    ProjMatrix.M32 += Properties.Settings.Default.PerspectiveOffsetY;
                }
            }

#if !WINDOWS_UWP
            if (multiMonClient && !config.MultiChannelDome1 && !config.MultiProjector)
            {
                ProjMatrix.M11 *= MonitorCountX * bezelSpacing;
                ProjMatrix.M22 *= MonitorCountY * bezelSpacing;
                ProjMatrix.M31 = (MonitorCountX - 1) - (MonitorX * bezelSpacing * 2);
                ProjMatrix.M32 = -((MonitorCountY - 1) - (MonitorY * bezelSpacing * 2));
            }

            if (rift)
            {
                if (renderType == RenderTypes.LeftEye)
                {

                    ProjMatrix.M31 += iod;
                }
                else
                {
                    ProjMatrix.M31 -= iod;
                }
            }
#endif

            RenderContext11.Projection = ProjMatrix;

            ViewMatrix = RenderContext11.View;
            RenderContext11.ViewBase = RenderContext11.View;

            MakeFrustum();
        }

        public float iod = .07f;
        private void SetupMatricesSpaceDome(bool forStars, RenderTypes renderType)
        {

            if (SolarSystemTrack != SolarSystemObjects.Custom && SolarSystemTrack != SolarSystemObjects.Undefined)
            {
                viewCamera.ViewTarget = Planets.GetPlanetTargetPoint(SolarSystemTrack, ViewLat, ViewLong, 0);
            }

            double camLocal = CameraRotate;
            if ((Settings.Active.LocalHorizonMode && !Settings.Active.GalacticMode) && currentImageSetfield.DataSetType == ImageSetType.Sky)
            {
                if (Properties.Settings.Default.ShowHorizon != false)
                {
                    Properties.Settings.Default.ShowHorizon = false;
                }
                Coordinates zenithAltAz = new Coordinates(0, 0);

                zenithAltAz.Az = 0;

                zenithAltAz.Alt = 0;

                ZoomFactor = TargetZoom = ZoomMax;
                alt = 0;
                az = 0;

                Coordinates zenith = Coordinates.HorizonToEquitorial(zenithAltAz, SpaceTimeController.Location, SpaceTimeController.Now);

                double raPart = -((zenith.RA - 6) / 24.0 * (Math.PI * 2));
                double decPart = -(((zenith.Dec)) / 360.0 * (Math.PI * 2));
                string raText = Coordinates.FormatDMS(zenith.RA);
                WorldMatrix = Matrix3d.RotationY(-raPart);
                WorldMatrix.Multiply(Matrix3d.RotationX(decPart));

                if (SpaceTimeController.Location.Lat < 0)
                {
                    WorldMatrix.Multiply(Matrix3d.RotationY(((az) / 180.0 * Math.PI)));

                    WorldMatrix.Multiply(Matrix3d.RotationX(((alt) / 180.0 * Math.PI)));
                    camLocal += Math.PI;
                }
                else
                {
                    WorldMatrix.Multiply(Matrix3d.RotationY(((-az) / 180.0 * Math.PI)));

                    WorldMatrix.Multiply(Matrix3d.RotationX(((-alt) / 180.0 * Math.PI)));
                }

                Coordinates currentRaDec = Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(alt, az), SpaceTimeController.Location, SpaceTimeController.Now);

                TargetLat = ViewLat = currentRaDec.Dec;
                TargetLong = ViewLong = RAtoViewLng(currentRaDec.RA);

            }

            Vector3d center = viewCamera.ViewTarget;
            RenderContext11.LightingEnabled = false;

            double localZoom = ZoomFactor * 20;
            Vector3d lookAt = new Vector3d(0, 0, -1);
            FovAngle = ((ZoomFactor/**16*/) / FOVMULT) / Math.PI * 180;


            double distance = (Math.Min(1, (.5 * (ZoomFactor / 180)))) - 1 + 0.0001;

            RenderContext11.CameraPosition = new Vector3d(0, 0, distance);
            Vector3d lookUp = new Vector3d(Math.Sin(-CameraRotate), Math.Cos(-CameraRotate), 0.0001f);

            Matrix3d lookAtAdjust = Matrix3d.Identity;

            switch (renderType)
            {
                case RenderTypes.DomeUp:
                    lookAtAdjust.Multiply(Matrix3d.RotationX(Math.PI / 2));
                    break;
                case RenderTypes.DomeLeft:
                    lookAtAdjust.Multiply(Matrix3d.RotationY(Math.PI / 2));
                    break;
                case RenderTypes.DomeRight:
                    lookAtAdjust.Multiply(Matrix3d.RotationY(-Math.PI / 2));
                    break;
                case RenderTypes.DomeFront:
                    break;
                case RenderTypes.DomeBack:
                    lookAtAdjust.Multiply(Matrix3d.RotationY(Math.PI));
                    break;
                default:
                    break;
            }


            if ((Settings.Active.GalacticMode && !Settings.Active.LocalHorizonMode) && currentImageSetfield.DataSetType == ImageSetType.Sky)
            {
                if (!galMatInit)
                {
                    galacticMatrix = Matrix3d.Identity;
                    galacticMatrix.Multiply(Matrix3d.RotationY(-(90 - (17.7603329867975 * 15)) / 180.0 * Math.PI));
                    galacticMatrix.Multiply(Matrix3d.RotationX(-((-28.9361739586894)) / 180.0 * Math.PI));
                    galacticMatrix.Multiply(Matrix3d.RotationZ(((31.422052860102041270114993238783) - 90) / 180.0 * Math.PI));
                    galMatInit = true;
                }

                WorldMatrix = galacticMatrix;
                WorldMatrix.Multiply(Matrix3d.RotationY(((az)) / 180.0 * Math.PI));
                WorldMatrix.Multiply(Matrix3d.RotationX(-((alt)) / 180.0 * Math.PI));


                double[] gPoint = Coordinates.GalactictoJ2000(az, alt);

                this.RA = gPoint[0] / 15;
                this.Dec = gPoint[1];
                targetViewCamera.Lat = viewCamera.Lat;
                targetViewCamera.Lng = viewCamera.Lng;
            }
            else
            {
                // Show in Ecliptic

                WorldMatrix = Matrix3d.RotationY(-((this.ViewLong + 90.0) / 180.0 * Math.PI));
                WorldMatrix.Multiply(Matrix3d.RotationX(((-this.ViewLat) / 180.0 * Math.PI)));
            }

            RenderContext11.World = WorldMatrix;
            RenderContext11.WorldBase = WorldMatrix;


            if (Settings.Active.LocalHorizonMode)
            {
                Matrix3d matNorth = Matrix3d.RotationY(Properties.Settings.Default.FaceNorth ? 0 : Math.PI);
                RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * matNorth * DomeAngleMatrix * lookAtAdjust;
            }
            else
            {
                RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * DomeMatrix * lookAtAdjust;
            }
            Vector3d temp = lookAt - RenderContext11.CameraPosition;
            temp.Normalize();
            ViewPoint = temp;

            // Set the near clip plane close enough that the sky dome isn't clipped
            double cameraZ = (Math.Min(1, (.5 * (ZoomFactor / 180)))) - 1 + 0.0001;
            m_nearPlane = (float)(1.0 + cameraZ) * 0.5f;

            ProjMatrix = Matrix3d.PerspectiveFovLH((Math.PI / 2.0), 1.0f, m_nearPlane, -1f);
            RenderContext11.PerspectiveFov = (Math.PI / 2.0);
            if (multiMonClient)
            {
                ProjMatrix.M11 *= MonitorCountX * bezelSpacing;
                ProjMatrix.M22 *= MonitorCountY * bezelSpacing;
                ProjMatrix.M31 = (MonitorCountX - 1) - (MonitorX * bezelSpacing * 2);
                ProjMatrix.M32 = -((MonitorCountY - 1) - (MonitorY * bezelSpacing * 2));
            }

            RenderContext11.Projection = ProjMatrix;

            ViewMatrix = RenderContext11.View;
            RenderContext11.ViewBase = RenderContext11.View;
            MakeFrustum();
        }

        private void SetupMatricesVideoOverlayDome(bool forStars, RenderTypes renderType)
        {


            Vector3d center = viewCamera.ViewTarget;
            RenderContext11.LightingEnabled = false;

            double localZoom = ZoomFactor * 20;
            Vector3d lookAt = new Vector3d(0, 0, -1);
            FovAngle = ((360) / FOVMULT) / Math.PI * 180;

            double distance = 1;

            RenderContext11.CameraPosition = new Vector3d(0, 0, distance);
            Vector3d lookUp = new Vector3d(Math.Sin(-0), Math.Cos(-0), 0.0001f);

            Matrix3d lookAtAdjust = Matrix3d.Identity;

            switch (renderType)
            {
                case RenderTypes.DomeUp:
                    lookAtAdjust.Multiply(Matrix3d.RotationX(Math.PI / 2));
                    break;
                case RenderTypes.DomeLeft:
                    lookAtAdjust.Multiply(Matrix3d.RotationY(Math.PI / 2));
                    break;
                case RenderTypes.DomeRight:
                    lookAtAdjust.Multiply(Matrix3d.RotationY(-Math.PI / 2));
                    break;
                case RenderTypes.DomeFront:
                    break;
                case RenderTypes.DomeBack:
                    lookAtAdjust.Multiply(Matrix3d.RotationY(Math.PI));
                    break;
                default:
                    break;
            }

            WorldMatrix = Matrix3d.RotationY(-((0 + 90) / 180f * Math.PI));
            WorldMatrix.Multiply(Matrix3d.RotationX(((0) / 180f * Math.PI)));
            RenderContext11.World = WorldMatrix;
            RenderContext11.WorldBase = WorldMatrix;


            RenderContext11.View = Matrix3d.LookAtLH(RenderContext11.CameraPosition, lookAt, lookUp) * DomeMatrix * lookAtAdjust;

            Vector3d temp = lookAt - RenderContext11.CameraPosition;
            temp.Normalize();
            ViewPoint = temp;


            m_nearPlane = ((.000000001));


            ProjMatrix = Matrix3d.PerspectiveFovLH((Math.PI / 2.0), 1.0f, m_nearPlane, -1f);


            if (multiMonClient)
            {
                ProjMatrix.M11 *= MonitorCountX * bezelSpacing;
                ProjMatrix.M22 *= MonitorCountY * bezelSpacing;
                ProjMatrix.M31 = (MonitorCountX - 1) - (MonitorX * bezelSpacing * 2);
                ProjMatrix.M32 = -((MonitorCountY - 1) - (MonitorY * bezelSpacing * 2));
            }
#if !WINDOWS_UWP
            if (rift)
            {
                if (renderType == RenderTypes.LeftEye)
                {

                    ProjMatrix.M31 += iod;
                }
                else
                {
                    ProjMatrix.M31 -= iod;
                }
            }
#endif
            RenderContext11.Projection = ProjMatrix;

            ViewMatrix = RenderContext11.View;
            RenderContext11.ViewBase = RenderContext11.View;
            MakeFrustum();
        }

        //public bool IsSphereInViewFrustum(SharpDX.Vector3 center, float radius)
        //{
        //    Vector4d centerV4 = new Vector4d(center.X, center.Y, center.Z, 1f);
        //    for (int i = 0; i < 6; i++)
        //    {
        //        if (frustum[i].Dot(centerV4) + radius < 0)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        public bool SphereIntersectRay(Vector3d pickRayOrig, Vector3d pickRayDir, out Coordinates pointCoordinate)
        {
            pointCoordinate = new Coordinates(0, 0);
            double r = 1;
            //Compute A, B and C coefficients
            double a = Vector3d.Dot(pickRayDir, pickRayDir);
            double b = 2 * Vector3d.Dot(pickRayDir, pickRayOrig);
            double c = Vector3d.Dot(pickRayOrig, pickRayOrig) - (r * r);

            //Find discriminant
            double disc = b * b - 4 * a * c;

            // if discriminant is negative there are no real roots, so return 
            // false as ray misses sphere
            if (disc < 0)
            {
                return false;
            }

            // compute q as described above
            double distSqrt = (double)Math.Sqrt(disc);
            double q;
            if (b < 0)
            {
                q = (-b - distSqrt) / 2.0f;
            }
            else
            {
                q = (-b + distSqrt) / 2.0f;
            }

            // compute t0 and t1
            double t0 = q / a;
            double t1 = c / q;

            // make sure t0 is smaller than t1
            if (t0 > t1)
            {
                // if t0 is bigger than t1 swap them around
                double temp = t0;
                t0 = t1;
                t1 = temp;
            }

            // if t1 is less than zero, the object is in the ray's negative direction
            // and consequently the ray misses the sphere
            if (t1 < 0)
            {
                return false;
            }
            double t = 0;
            // if t0 is less than zero, the intersection point is at t1
            if (t0 < 0)
            {
                t = t1;
            }
            // else the intersection point is at t0
            else
            {
                t = t0;
            }

            Vector3d point = pickRayDir * t;

            point = pickRayOrig + point;

            pointCoordinate = Coordinates.CartesianToSpherical2(point);

            return true;
        }


        public bool SphereIntersectRay(SharpDX.Vector3 pickRayOrig, SharpDX.Vector3 pickRayDir, out Coordinates pointCoordinate)
        {
            pointCoordinate = new Coordinates(0, 0);
            float r = 1;
            //Compute A, B and C coefficients
            float a = SharpDX.Vector3.Dot(pickRayDir, pickRayDir);
            float b = 2 * SharpDX.Vector3.Dot(pickRayDir, pickRayOrig);
            float c = SharpDX.Vector3.Dot(pickRayOrig, pickRayOrig) - (r * r);

            //Find discriminant
            float disc = b * b - 4 * a * c;

            // if discriminant is negative there are no real roots, so return 
            // false as ray misses sphere
            if (disc < 0)
            {
                return false;
            }

            // compute q as described above
            float distSqrt = (float)Math.Sqrt(disc);
            float q;
            if (b < 0)
            {
                q = (-b - distSqrt) / 2.0f;
            }
            else
            {
                q = (-b + distSqrt) / 2.0f;
            }

            // compute t0 and t1
            float t0 = q / a;
            float t1 = c / q;

            // make sure t0 is smaller than t1
            if (t0 > t1)
            {
                // if t0 is bigger than t1 swap them around
                float temp = t0;
                t0 = t1;
                t1 = temp;
            }

            // if t1 is less than zero, the object is in the ray's negative direction
            // and consequently the ray misses the sphere
            if (t1 < 0)
            {
                return false;
            }
            float t = 0;
            // if t0 is less than zero, the intersection point is at t1
            if (t0 < 0)
            {
                t = t1;
            }
            // else the intersection point is at t0
            else
            {
                t = t0;
            }

            SharpDX.Vector3 point = pickRayDir * t;

            point = pickRayOrig + point;

            pointCoordinate = Coordinates.CartesianToSpherical2(point);

            return true;
        }


        public void TransformPickPointToWorldSpace(Vector2d ptCursor, int backBufferWidth, int backBufferHeight, out Vector3d vPickRayOrig, out Vector3d vPickRayDir)
        {
            // Credit due to the DirectX 9 C++ Pick sample and MVP Robert Dunlop
            // Get the pick ray from the mouse position

            // Compute the vector of the pick ray in screen space
            Vector3d v;
            v.X = (((2.0 * (double)ptCursor.X) / (double)backBufferWidth) - 1) / ProjMatrix.M11;
            v.Y = -(((2.0 * (double)ptCursor.Y) / backBufferHeight) - 1) / ProjMatrix.M22;
            v.Z = 1.0;

            //Matrix3d mInit = WorldMatrix * ViewMatrix;
            Matrix3d mInit = RenderContext11.WorldBase * ViewMatrix;

            Matrix3d m = Matrix3d.Invert(mInit);

            // Transform the screen space pick ray into 3D space
            vPickRayDir.X = v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31;
            vPickRayDir.Y = v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32;
            vPickRayDir.Z = v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33;


            vPickRayDir.Normalize();

            vPickRayOrig.X = m.M41;
            vPickRayOrig.Y = m.M42;
            vPickRayOrig.Z = m.M43;
        }

        public void TransformStarPickPointToWorldSpace(Vector2d ptCursor, int backBufferWidth, int backBufferHeight, out Vector3d vPickRayOrig, out Vector3d vPickRayDir)
        {

            Vector3d v;
            v.X = (((2.0f * ptCursor.X) / backBufferWidth) - 1) / ProjMatrix.M11;
            v.Y = -(((2.0f * ptCursor.Y) / backBufferHeight) - 1) / ProjMatrix.M22;
            v.Z = 1.0f;

            Matrix3d mInit = WorldMatrix * ViewMatrix;

            Matrix3d m = Matrix3d.Invert(mInit);

            // Transform the screen space pick ray into 3D space
            vPickRayDir.X = v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31;
            vPickRayDir.Y = v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32;
            vPickRayDir.Z = v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33;


            vPickRayDir.Normalize();

            // Transform the screen space pick ray into 3D space
            vPickRayDir.X = v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31;
            vPickRayDir.Y = v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32;
            vPickRayDir.Z = v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33;


            vPickRayDir.Normalize();

            vPickRayOrig.X = m.M41;
            vPickRayOrig.Y = m.M42;
            vPickRayOrig.Z = m.M43;

            // Calculate the origin as intersection with near frustum

            vPickRayOrig.X += vPickRayDir.X * m_nearPlane;
            vPickRayOrig.Y += vPickRayDir.Y * m_nearPlane;
            vPickRayOrig.Z += vPickRayDir.Z * m_nearPlane;
        }



        //End Matricies and Transforms


        //Begin Main rendering Section
        public static int LoadTileBudget = 1;


        bool refreshDomeTextures = true;
        bool usingLargeTextures = true;
        int currentCubeFaceSize = 0;
        public bool SyncLayerNeeded = false;
        public bool SyncTourNeeded = false;
        public bool ChronoZoomOpen = false;

        static public bool readyToRender = false;
        public static bool Initialized = false;
        static public bool ReadyToRender
        {
            get { return readyToRender && Initialized; }
            set { readyToRender = value; }
        }


        //todo Wire pause up for real!
        bool paused = false;

        public delegate void RenderNotify();

        public RenderNotify PreRenderStage = null;
        public RenderNotify PostRenderStage = null;
        public RenderNotify EndRenderStage = null;

        public IUiController uiController = null;

        public void Render()
        {
            if (!readyToRender)
            {
                return;
            }

#if !WINDOWS_UWP
            if (SyncLayerNeeded)
            {
                NetControl.SyncLayersUiThread();
            }

            if (SyncTourNeeded)
            {
                NetControl.SyncTourUiThread();
            }

#endif
            if (Tile.fastLoad)
            {
                Tile.fastLoadAutoReset = true;
            }
#if !BASICWWT
            if (!TourPlayer.Playing)
#endif
            {
                CrossFadeFrame = false;
            }



            Int64 ticks = HiResTimer.TickCount;

            double elapsedSeconds = ((double)(ticks - lastRenderTickCount)) / HiResTimer.Frequency;
#if !WINDOWS_UWP
            if (!rift)
            {

                if (Properties.Settings.Default.TargetFrameRate != 0 && !(Properties.Settings.Default.FrameSync && Properties.Settings.Default.TargetFrameRate == 60))
                {
                    int frameRate = Properties.Settings.Default.TargetFrameRate;


                    if (elapsedSeconds < (1.0 / (double)frameRate))
                    {
                        return;
                    }
                }
            }
#endif
            lastRenderTickCount = ticks;

            lastFrameTime = (Math.Min(.1, elapsedSeconds));

            //Update MetaNow to current realtime for entire frame to render exactly on time
            SpaceTimeController.MetaNow = DateTime.Now;


            LoadTileBudget = 1;
#if !WINDOWS_UWP
            if (paused || !Initialized)
            {
                System.Threading.Thread.Sleep(100);
                return;
            }
#endif

#if !WINDOWS_UWP
            if (ProjectorServer)
            {
                Earth3d.MainWindow.UpdateNetworkStatus();
            }


            //oculus rift support
            rift = StereoMode == StereoModes.OculusRift;
            if (rift)
            {
                GetRiftSample();
            }
#endif
            if (!megaFrameDump)
            {
                TileCache.PurgeLRU();
            }

            TileCache.DecimateQueue();

            Tile.imageQuality = Properties.Settings.Default.ImageQuality;

            Tile.CurrentRenderGeneration++;

#if !WINDOWS_UWP
            IconCacheEntry.CurrentFrame = Tile.CurrentRenderGeneration;
#endif
            Tile.lastDeepestLevel = Tile.deepestLevel;
            Tile.TilesInView = 0;
            Tile.TrianglesRendered = 0;
            Tile.TilesTouched = 0;


            if (ZoomFactor == 0 || TargetZoom == 0 || double.IsNaN(ZoomFactor) || double.IsNaN(TargetZoom))
            {
                ZoomFactor = TargetZoom = 360;
            }
            

            TileCache.InitNextWaitingTile();

            // reset dome matrix Cache
            DomeMatrixFresh = false;


            if (mover != null)
            {
                SpaceTimeController.Now = mover.CurrentDateTime;
            }
            else
            {

                SpaceTimeController.UpdateClock();
          
                LayerManager.UpdateLayerTime();

            }

            if (uiController != null)
            {
                {
                    uiController.PreRender(this);
                }
            }

            if (Space)
            {
                Planets.UpdatePlanetLocations(false);
            }
            else if (currentImageSetfield.DataSetType == ImageSetType.SolarSystem)
            {
                // todo allow update of focus planet
                Planets.UpdatePlanetLocations(true);
                Planets.UpdateOrbits(0);
            }


            if (PreRenderStage != null)
            {
                PreRenderStage();
            }

            
            if (mover != null)
            {
                UpdateMover(mover);
            }
            else
            {
                if (!SandboxMode)
                {
                    if (SolarSystemTrack == SolarSystemObjects.Undefined | (SolarSystemTrack == SolarSystemObjects.Custom && viewCamera.ViewTarget == Vector3d.Empty))
                    {
                        SolarSystemTrack = SolarSystemObjects.Sun;
                    }
                }
            }

            UpdateViewParameters();

            if (SolarSystemMode)
            {

                if (SolarSystemTrack != SolarSystemObjects.Custom)
                {
                    viewCamera.ViewTarget = Planets.GetPlanet3dLocation(SolarSystemTrack);
                }
            }

            ClampZoomValues();



            LayerManager.PrepTourLayers();

            if (PostRenderStage != null)
            {
                PostRenderStage();
            }

            //Capture this state once to avoid race condition where its false now, but changes before the frame is done
            bool tilesAllLoaded = TileCache.QueuePercent == 100;

            if (!megaFrameDump)
            {

                if (ZoomWindowVisible)
                {
                    if (zoomWindowRenderTarget == null)
                    {
                        zoomWindowRenderTarget = new RenderTargetTexture(506, 450, RenderContext11.DefaultColorFormat);
                    }

                    if (ZoomWindowRefresh)
                    {
                        RenderZoomWindow(zoomWindowRenderTarget.renderView, ZoomWindowCamera, 506, 450);
                    }
                }


#if !WINDOWS_UWP
                if (StereoMode != StereoModes.Off && (!Space || rift))
                {
                    RenderContext11.ViewPort = new SharpDX.ViewportF(0, 0, ViewWidth, ViewHeight, 0.0f, 1.0f);

                    // Ensure that the dome depth/stencil buffer matches our requirements
                    if (domeZbuffer != null)
                    {
                        if (domeZbuffer.Width != ViewWidth || domeZbuffer.Height != ViewHeight)
                        {
                            domeZbuffer.Dispose();
                            GC.SuppressFinalize(domeZbuffer);
                            domeZbuffer = null;
                        }
                    }

                    if (leftEye != null)
                    {
                        if (leftEye.RenderTexture.Height != ViewHeight || leftEye.RenderTexture.Width != ViewWidth)
                        {
                            leftEye.Dispose();
                            GC.SuppressFinalize(leftEye);
                            leftEye = null;
                        }
                    }

                    if (rightEye != null)
                    {
                        if (rightEye.RenderTexture.Height != ViewHeight || rightEye.RenderTexture.Width != ViewWidth)
                        {
                            rightEye.Dispose();
                            GC.SuppressFinalize(rightEye);
                            rightEye = null;
                        }
                    }

                    if (stereoRenderTextureLeft != null)
                    {
                        if (stereoRenderTextureLeft.RenderTexture.Height != ViewHeight || stereoRenderTextureLeft.RenderTexture.Width != ViewWidth)
                        {
                            stereoRenderTextureLeft.Dispose();
                            GC.SuppressFinalize(stereoRenderTextureLeft);
                            stereoRenderTextureLeft = null;

                            stereoRenderTextureRight.Dispose();
                            GC.SuppressFinalize(stereoRenderTextureRight);
                            stereoRenderTextureRight = null;
                        }
                    }


                    if (leftEye == null)
                    {
                        leftEye = new RenderTargetTexture(ViewWidth, ViewHeight, 1);
                    }

                    if (rightEye == null)
                    {
                        rightEye = new RenderTargetTexture(ViewWidth, ViewHeight, 1);
                    }

                    if (leftDepthBuffer != null)
                    {
                        if (leftDepthBuffer.Width != ViewWidth || leftDepthBuffer.Height != ViewHeight)
                        {
                            leftDepthBuffer = new DepthBuffer(ViewWidth, ViewHeight);

                            rightDepthBuffer = new DepthBuffer(ViewWidth, ViewHeight);

                            leftDepthBuffer.Dispose();
                            GC.SuppressFinalize(leftDepthBuffer);
                            leftDepthBuffer = null;

                            rightDepthBuffer.Dispose();
                            GC.SuppressFinalize(rightDepthBuffer);
                            rightDepthBuffer = null;
                        }
                    }

                    if (RenderContext11.MultiSampleCount > 1)
                    {
#if !WINDOWS_UWP
                        if (rift)
                        {
                            // When multisample anti-aliasing is enabled, render to an offscreen buffer and then
                            // resolve to the left and then the right eye textures. 
                            if (stereoRenderTextureLeft == null)
                            {
                                stereoRenderTextureLeft = new RenderTargetTexture(leftEyeWidth, leftEyeHeight, riftFormat);
                            }

                            if (stereoRenderTextureRight == null)
                            {
                                stereoRenderTextureRight = new RenderTargetTexture(leftEyeWidth, leftEyeHeight, riftFormat);
                            }

                            if (leftDepthBuffer == null)
                            {
                                leftDepthBuffer = new DepthBuffer(leftEyeWidth, leftEyeHeight);
                            }

                            if (rightDepthBuffer == null)
                            {
                                rightDepthBuffer = new DepthBuffer(leftEyeWidth, leftEyeHeight);
                            }

                            int eye = 0;
                            var swapTexture = eyeTextures[(int)eye];
                            int textureIndex;
                            eyeTextures[eye].SwapTextureSet.GetCurrentIndex(out textureIndex);
                            RenderFrame(stereoRenderTextureLeft.renderView, leftDepthBuffer.DepthView, RenderTypes.LeftEye, ViewWidth, ViewHeight);
                            SharpDX.Direct3D11.Resource dest = eyeTextures[eye].RenderTargetViews[textureIndex].Resource;
                            RenderContext11.PrepDevice.ImmediateContext.ResolveSubresource(stereoRenderTextureLeft.RenderTexture.Texture, 0,
                                                                                            dest, 0,
                                                                                           riftFormat);
                            eyeTextures[eye].SwapTextureSet.Commit();
                            eye = 1;
                            swapTexture = eyeTextures[(int)eye];

                            eyeTextures[eye].SwapTextureSet.GetCurrentIndex(out textureIndex);

                            if (Properties.Settings.Default.RiftMonoMode)
                            {
                                // Resolve a single buffer for each eye, cuts rendering cost in half

                                dest = eyeTextures[eye].RenderTargetViews[textureIndex].Resource;
                                RenderContext11.PrepDevice.ImmediateContext.ResolveSubresource(stereoRenderTextureLeft.RenderTexture.Texture, 0,
                                                                                                 dest, 0,
                                                                                                riftFormat);
                                eyeTextures[eye].SwapTextureSet.Commit();
                            }
                            else
                            {
                                RenderFrame(stereoRenderTextureRight.renderView, rightDepthBuffer.DepthView, RenderTypes.RightEye, ViewWidth, ViewHeight);

                                dest = eyeTextures[eye].RenderTargetViews[textureIndex].Resource;
                                RenderContext11.PrepDevice.ImmediateContext.ResolveSubresource(stereoRenderTextureRight.RenderTexture.Texture, 0,
                                                                                                 dest, 0,
                                                                                                riftFormat);
                                eyeTextures[eye].SwapTextureSet.Commit();
                            }
                        }
                        else
#endif
                        {
                            // When multisample anti-aliasing is enabled, render to an offscreen buffer and then
                            // resolve to the left and then the right eye textures. 
                            if (stereoRenderTextureLeft == null)
                            {
                                stereoRenderTextureLeft = new RenderTargetTexture(ViewWidth, ViewHeight);
                            }

                            if (stereoRenderTextureRight == null)
                            {
                                stereoRenderTextureRight = new RenderTargetTexture(ViewWidth, ViewHeight);
                            }

                            if (leftDepthBuffer == null)
                            {
                                leftDepthBuffer = new DepthBuffer(ViewWidth, ViewHeight);
                            }

                            if (rightDepthBuffer == null)
                            {
                                rightDepthBuffer = new DepthBuffer(ViewWidth, ViewHeight);
                            }

                            RenderFrame(stereoRenderTextureLeft.renderView, leftDepthBuffer.DepthView, RenderTypes.LeftEye, ViewWidth, ViewHeight);

                            RenderContext11.PrepDevice.ImmediateContext.ResolveSubresource(stereoRenderTextureLeft.RenderTexture.Texture, 0,
                                                                                           leftEye.RenderTexture.Texture, 0,
                                                                                           RenderContext11.DefaultColorFormat);

                            RenderFrame(stereoRenderTextureRight.renderView, rightDepthBuffer.DepthView, RenderTypes.RightEye, ViewWidth, ViewHeight);

                            RenderContext11.PrepDevice.ImmediateContext.ResolveSubresource(stereoRenderTextureRight.RenderTexture.Texture, 0,
                                                                                           rightEye.RenderTexture.Texture, 0,
                                                                                           RenderContext11.DefaultColorFormat);

                        }
                    }
                    else
                    {
#if !WINDOWS_UWP
                        if (rift)
                        {
                            int eye = 0;
                            int textureIndex;
                            eyeTextures[eye].SwapTextureSet.GetCurrentIndex(out textureIndex);
                            var swapTexture = eyeTextures[eye].RenderTargetViews[textureIndex];

                            RenderFrame(swapTexture, eyeTextures[eye].DepthStencilView, RenderTypes.LeftEye, leftEyeWidth, leftEyeHeight);

                            eyeTextures[eye].SwapTextureSet.Commit();
                            eye = 1;
                            eyeTextures[eye].SwapTextureSet.GetCurrentIndex(out textureIndex);
                            swapTexture = eyeTextures[eye].RenderTargetViews[textureIndex];

                            RenderFrame(swapTexture, eyeTextures[eye].DepthStencilView, RenderTypes.RightEye, rightEyeWidth, rightEyeHeight);

                            eyeTextures[eye].SwapTextureSet.Commit();

                        }
                        else
#endif
                        {
                            // When anti-aliasing is not enabled, render directly to the left and right eye textures.
                            RenderFrame(leftEye.renderView, domeZbuffer.DepthView, RenderTypes.LeftEye, ViewWidth, ViewHeight);
                            RenderFrame(rightEye.renderView, domeZbuffer.DepthView, RenderTypes.RightEye, ViewWidth, ViewHeight);
                        }
                    }

                    if (StereoMode == StereoModes.InterlineEven || StereoMode == StereoModes.InterlineOdd)
                    {
                        RenderSteroPairInterline(leftEye, rightEye);
                    }
                    else if (StereoMode == StereoModes.AnaglyphMagentaGreen || StereoMode == StereoModes.AnaglyphRedCyan || StereoMode == StereoModes.AnaglyphYellowBlue)
                    {
                        RenderSteroPairAnaglyph(leftEye, rightEye);
                    }
#if !WINDOWS_UWP
                    else if (StereoMode == StereoModes.OculusRift)
                    {
                        var result = hmd.SubmitFrame(0, layers);
                        riftFrameIndex++;

                        RenderTextureToScreen(mirror.ResourceView, mirrorTexture.Description.Width, mirrorTexture.Description.Height);

                    }
#endif
                    else
                    {
                        if (StereoMode == StereoModes.CrossEyed)
                        {

                            RenderSteroPairSideBySide(rightEye, leftEye);
                        }
                        else
                        {
                            RenderSteroPairSideBySide(leftEye, rightEye);
                        }
                    }
                }
                else if (Settings.DomeView)
                {
                    int cubeFaceSize = 512;
                    if (usingLargeTextures)
                    {
                        cubeFaceSize = 1024;
                    }

                    if (CaptureVideo && dumpFrameParams.Dome)
                    {
                        cubeFaceSize = 2048;
                    }



                    if (usingLargeTextures != Properties.Settings.Default.LargeDomeTextures)
                    {
                        refreshDomeTextures = true;
                    }

                    if (currentCubeFaceSize != cubeFaceSize)
                    {
                        refreshDomeTextures = true;
                    }

                    if (refreshDomeTextures)
                    {
                        usingLargeTextures = Properties.Settings.Default.LargeDomeTextures;
                        for (int face = 0; face < 5; face++)
                        {
                            if (domeCube[face] != null)
                            {
                                domeCube[face].Dispose();
                                GC.SuppressFinalize(domeCube[face]);
                                domeCube[face] = null;
                            }
                        }
                        if (domeZbuffer != null)
                        {
                            domeZbuffer.Dispose();
                            GC.SuppressFinalize(domeZbuffer);
                            domeZbuffer = null;
                        }
                        if (domeCubeFaceMultisampled != null)
                        {
                            domeCubeFaceMultisampled.Dispose();
                            GC.SuppressFinalize(domeCubeFaceMultisampled);
                            domeCubeFaceMultisampled = null;
                        }
                    }


                    // Ensure that the dome depth/stencil buffer matches our requirements
                    if (domeZbuffer != null)
                    {
                        if (domeZbuffer.Width != cubeFaceSize || domeZbuffer.Height != cubeFaceSize)
                        {
                            domeZbuffer.Dispose();
                            GC.SuppressFinalize(domeZbuffer);
                            domeZbuffer = null;
                        }
                    }

                    if (domeZbuffer == null)
                    {
                        domeZbuffer = new DepthBuffer(cubeFaceSize, cubeFaceSize);
                    }

                    if (domeCubeFaceMultisampled == null && RenderContext11.MultiSampleCount > 1)
                    {
                        domeCubeFaceMultisampled = new RenderTargetTexture(cubeFaceSize, cubeFaceSize);
                    }

                    for (int face = 0; face < 5; face++)
                    {
                        if (domeCube[face] == null)
                        {
                            domeCube[face] = new RenderTargetTexture(cubeFaceSize, cubeFaceSize, 1);
                            currentCubeFaceSize = cubeFaceSize;
                            refreshDomeTextures = false;
                        }

                        if (RenderContext11.MultiSampleCount > 1)
                        {
                            // When MSAA is enabled, we render each face to the same multisampled render target,
                            // then resolve to a different texture for each face. This saves memory and works around
                            // the fact that multisample textures are not permitted to have mipmaps.
                            RenderFrame(domeCubeFaceMultisampled.renderView, domeZbuffer.DepthView, (RenderTypes)face, cubeFaceSize, cubeFaceSize);
                            RenderContext11.PrepDevice.ImmediateContext.ResolveSubresource(domeCubeFaceMultisampled.RenderTexture.Texture, 0,
                                                                                           domeCube[face].RenderTexture.Texture, 0,
                                                                                           RenderContext11.DefaultColorFormat);
                        }
                        else
                        {
                            RenderFrame(domeCube[face].renderView, domeZbuffer.DepthView, (RenderTypes)face, cubeFaceSize, cubeFaceSize);
                        }
                        RenderContext11.PrepDevice.ImmediateContext.GenerateMips(domeCube[face].RenderTexture.ResourceView);
                    }

                    if (Properties.Settings.Default.DomeTypeIndex > 0)
                    {
                        RenderWarpedFisheye();
                    }
                    else
                    {
                        if (CaptureVideo && dumpFrameParams.Dome)
                        {
                            if (!dumpFrameParams.WaitDownload || tilesAllLoaded)
                            {
                                RenderDomeMaster();
                            }
                        }
                        RenderFisheye(false);
                    }

                }
                else if (config.UseDistrotionAndBlend)
                {
                    if (undistorted == null)
                    {
                        undistorted = new RenderTargetTexture(config.Width, config.Height, 1);
                    }

                    if (undistortedAA == null && RenderContext11.MultiSampleCount > 1)
                    {
                        undistortedAA = new RenderTargetTexture(config.Width, config.Height);
                    }


                    // Ensure that the dome depth/stencil buffer matches our requirements
                    if (domeZbuffer != null)
                    {
                        if (domeZbuffer.Width != config.Width || domeZbuffer.Height != config.Height)
                        {
                            domeZbuffer.Dispose();
                            GC.SuppressFinalize(domeZbuffer);
                            domeZbuffer = null;
                        }
                    }

                    if (domeZbuffer == null)
                    {
                        domeZbuffer = new DepthBuffer(config.Width, config.Height);
                    }


                    // * If there's no multisampling, draw directly into the undistorted texture
                    // * When multisampling is on, draw into an intermediate buffer, then resolve 
                    //   it into the undistorted texture
                    if (RenderContext11.MultiSampleCount > 1)
                    {
                        RenderFrame(undistortedAA.renderView, domeZbuffer.DepthView, RenderTypes.Normal, config.Width, config.Height);

                        RenderContext11.PrepDevice.ImmediateContext.ResolveSubresource(undistortedAA.RenderTexture.Texture, 0,
                                                                                       undistorted.RenderTexture.ResourceView.Resource, 0,
                                                                                       RenderContext11.DefaultColorFormat);
                    }
                    else
                    {
                        RenderFrame(undistorted.renderView, domeZbuffer.DepthView, RenderTypes.Normal, config.Width, config.Height);
                    }

                    RenderContext11.PrepDevice.ImmediateContext.GenerateMips(undistorted.RenderTexture.ResourceView);


                    if (config.UsingSgcWarpMap)
                    {
                        RenderDistortWithBlend();
                    }
                    else
                    {
                        RenderDistort();
                    }
                }
                else if (Properties.Settings.Default.FlatScreenWarp)
                {
                    if (undistorted == null)
                    {
                        undistorted = new RenderTargetTexture(ViewWidth, RenderContext11.Height);
                    }

                    if (domeZbuffer != null)
                    {
                        if (domeZbuffer.Width != ViewWidth || domeZbuffer.Height != RenderContext11.Height)
                        {
                            domeZbuffer.Dispose();
                            GC.SuppressFinalize(domeZbuffer);
                            domeZbuffer = null;
                        }
                    }

                    if (domeZbuffer == null)
                    {
                        domeZbuffer = new DepthBuffer(ViewWidth, RenderContext11.Height);

                    }


                    RenderFrame(undistorted.renderView, domeZbuffer.DepthView, RenderTypes.Normal, ViewWidth, RenderContext11.Height);
                    RenderFlatDistort();

                }
                else
#endif
                {
                    if (RenderContext11.ExternalProjection && RenderContext11.MultiSampleCount > 1)
                    {

                        // When multisample anti-aliasing is enabled, render to an offscreen buffer and then
                        // resolve to the left and then the right eye textures. 
                        if (stereoRenderTextureBoth == null)
                        {
                            stereoRenderTextureBoth = new RenderTargetTexture(ViewWidth, ViewHeight, true);
                        }

                        if (stereoDepthBufferBoth == null)
                        {
                            stereoDepthBufferBoth = new DepthBuffer(ViewWidth, ViewHeight, true);
                        }


                        RenderFrame(stereoRenderTextureBoth.renderView, stereoDepthBufferBoth.DepthView, RenderTypes.Normal, ViewWidth, ViewHeight);

                        RenderContext11.devContext.OutputMerger.ResetTargets();
                        SharpDX.Direct3D11.Resource dest = RenderContext11.externalTargetView.Resource;
                        RenderContext11.PrepDevice.ImmediateContext.ResolveSubresource(stereoRenderTextureBoth.RenderTexture.Texture, 0,
                                                                                        dest, 0,
                                                                                        Format.B8G8R8A8_UNorm);

                        RenderContext11.PrepDevice.ImmediateContext.ResolveSubresource(stereoRenderTextureBoth.RenderTexture.Texture, 1,
                                                                                        dest, 1,
                                                                                        Format.B8G8R8A8_UNorm);

                    }
                    else
                    {

                        if (RenderContext11.Height != RenderContext11.DisplayViewport.Height ||
                                    RenderContext11.Width != RenderContext11.DisplayViewport.Width)
                        {
                            RenderContext11.Resize(RenderContext11.Height, RenderContext11.Width);
                        }
                        RenderFrame(null, null, RenderTypes.Normal, RenderContext11.DisplayViewport.Width, RenderContext11.DisplayViewport.Height);
                    }
                }
            }

            if (EndRenderStage!= null)
            {
                EndRenderStage();
            }
#if !WINDOWS_UWP
            Utils.lastRender = HiResTimer.TickCount;


            if (CaptureVideo)
            {
                if (!dumpFrameParams.WaitDownload || tilesAllLoaded)
                {
                    if (!dumpFrameParams.Dome)
                    {
                        Int64 ticksa = HiResTimer.TickCount;
                        SaveFrame();
                    }
                    SpaceTimeController.NextFrame();
                }
                if (SpaceTimeController.DoneDumping())
                {
                    SpaceTimeController.CancelFrameDump = false;
                    DomeFrameDumping();
                }
            }
#endif
            if (Tile.fastLoadAutoReset)
            {
                Tile.fastLoad = false;
                Tile.fastLoadAutoReset = false;
            }

        }

        CameraParameters ZoomWindowCamera = new CameraParameters();
        bool ZoomWindowVisible = false;
        bool ZoomWindowRefresh = false;
        RenderTargetTexture zoomWindowRenderTarget;
        public void CaptureMegaShot(string filename, int width, int height)
        {
#if !WINDOWS_UWP
            megaHeight = height;
            megaWidth = width;
            megaFrameDump = true;

            RenderTargetTexture megaTextureAA = new RenderTargetTexture(width, height);
            RenderTargetTexture megaTexture = new RenderTargetTexture(width, height, 1);

            DepthBuffer megaZbuffer = new DepthBuffer(width, height);

            while (true)
            {

                if (RenderContext11.MultiSampleCount > 1)
                {
                    // When MSAA is enabled, we render each face to the same multisampled render target,
                    // then resolve to a different texture for each face. This saves memory and works around
                    // the fact that multisample textures are not permitted to have mipmaps.
                    RenderFrame(megaTextureAA.renderView, megaZbuffer.DepthView, RenderTypes.Normal, width, height);

                    RenderContext11.PrepDevice.ImmediateContext.ResolveSubresource(megaTextureAA.RenderTexture.Texture, 0,
                                                                                  megaTexture.RenderTexture.Texture, 0,
                                                                                  RenderContext11.DefaultColorFormat);
                }
                else
                {
                    RenderFrame(megaTexture.renderView, megaZbuffer.DepthView, RenderTypes.Normal, width, height);
                }

                if (TileCache.QueuePercent == 100)
                {
                    break;
                }

                System.Windows.Forms.Application.DoEvents();
            }
            SharpDX.Direct3D11.Texture2D.ToFile(RenderContext11.devContext, megaTexture.RenderTexture.Texture, SharpDX.Direct3D11.ImageFileFormat.Png, filename);
            megaFrameDump = false;
            megaTexture.Dispose();
            megaTextureAA.Dispose();
            megaZbuffer.Dispose();
#endif
        }

        public delegate void NotifyOpacityUpdate();

        public NotifyOpacityUpdate OpacityChanged = null;

        public void UpdateMover(IViewMover mover)
        {
            CameraParameters newCam = mover.CurrentPosition;
            bool opacityChanged = false;

            if (viewCamera.Opacity != newCam.Opacity)
            {
                opacityChanged = true;
            }

            viewCamera = targetViewCamera = newCam;

            if (OpacityChanged != null && opacityChanged)
            {
                OpacityChanged();
            }

            if (Space && Settings.Active.GalacticMode)
            {
                double[] gPoint = Coordinates.J2000toGalactic(newCam.RA * 15, newCam.Dec);

                targetAlt = alt = gPoint[1];
                targetAz = az = gPoint[0];
            }
            else if (Space && Settings.Active.LocalHorizonMode)
            {
                Coordinates currentAltAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(newCam.RA, newCam.Dec), SpaceTimeController.Location, SpaceTimeController.Now);

                targetAlt = alt = currentAltAz.Alt;
                targetAz = az = currentAltAz.Az;
            }

            if (mover.Complete)
            {
                targetViewCamera = viewCamera = newCam;
                Mover = null;
                //Todo Notify interested parties that move is complete

                if (NotifyMoveComplete != null)
                {
                    NotifyMoveComplete();
                }
            }
        }

#if !WINDOWS_UWP
        private void RenderFlatDistort()
        {
            if (warpTexture == null)
            {
                warpTexture = new RenderTargetTexture(2048, 2048);
            }

            SetupMatricesFisheye();

            if (warpIndexBuffer == null)
            {
                CreateWarpVertexBuffer();
            }

            RenderContext11.SetDisplayRenderTargets();

            RenderContext11.ClearRenderTarget(SharpDX.Color.Black);
            RenderContext11.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            RenderContext11.BlendMode = BlendMode.None;
            RenderContext11.setRasterizerState(TriangleCullMode.Off);
            SharpDX.Matrix mat = (RenderContext11.World * RenderContext11.View * RenderContext11.Projection).Matrix11;
            mat.Transpose();

            WarpOutputShader.MatWVP = mat;
            WarpOutputShader.Use(RenderContext11.devContext, true);

            RenderContext11.SetIndexBuffer(warpIndexBuffer);
            RenderContext11.SetVertexBuffer(warpVertexBuffer);

            RenderContext11.devContext.PixelShader.SetShaderResource(0, undistorted.RenderTexture.ResourceView);
            RenderContext11.devContext.DrawIndexed(warpIndexBuffer.Count, 0, 0);

            PresentFrame11(false);

        }

        private void DomeFrameDumping()
        {
            SpaceTimeController.FrameDumping = false;
            CaptureVideo = false;
            if (domeMasterTexture != null)
            {
                domeMasterTexture.Dispose();
                GC.SuppressFinalize(domeMasterTexture);
                domeMasterTexture = null;
            }

            //todo fix this for UWP

            Earth3d.MainWindow.TourEdit.PauseTour();

        }
#endif
        private void SaveFrame()
        {
#if !WINDOWS_UWP
            RenderContext11.SaveBackBuffer(dumpFrameParams.Name.Replace(".", string.Format("_{0:0000}.", SpaceTimeController.CurrentFrameNumber)), SharpDX.Direct3D11.ImageFileFormat.Png);
#endif
        }
#if !WINDOWS_UWP
        double lastFisheyAngle = 0;

        private void RenderFisheye(bool forTexture)
        {

            if (!forTexture)
            {
                SetupMatricesFisheye();
                RenderContext11.SetDisplayRenderTargets();
            }


            RenderContext11.ClearRenderTarget(SharpDX.Color.Black);

            if (domeVertexBuffer == null || lastFisheyAngle != Properties.Settings.Default.FisheyeAngle)
            {
                lastFisheyAngle = Properties.Settings.Default.FisheyeAngle;

                domeVertexBuffer = new PositionColorTexturedVertexBuffer11[5];
                domeIndexBuffer = new IndexBuffer11[5];

                for (int face = 0; face < 5; face++)
                {
                    CreateDomeFaceVertexBuffer(face);
                }
            }


            RenderContext11.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            RenderContext11.BlendMode = BlendMode.None;
            RenderContext11.setRasterizerState(TriangleCullMode.Off);
            SharpDX.Matrix mat = (RenderContext11.World * RenderContext11.View * RenderContext11.Projection).Matrix11;
            mat.Transpose();

            WarpOutputShader.MatWVP = mat;
            WarpOutputShader.Use(RenderContext11.devContext, true);


            for (int face = 0; face < 5; face++)
            {
                RenderContext11.SetIndexBuffer(domeIndexBuffer[face]);
                RenderContext11.SetVertexBuffer(domeVertexBuffer[face]);
                RenderContext11.devContext.PixelShader.SetShaderResource(0, domeCube[face].RenderTexture.ResourceView);
                RenderContext11.devContext.DrawIndexed(domeIndexBuffer[face].Count, 0, 0);
            }

            PresentFrame11(forTexture);

        }

        RenderTargetTexture domeMasterTexture = null;
#endif
#if !WINDOWS_UWP
        private void RenderDomeMaster()
        {

            if (domeMasterTexture == null)
            {
                domeMasterTexture = new RenderTargetTexture(dumpFrameParams.Width, dumpFrameParams.Height, 1);
            }

            RenderContext11.DepthStencilMode = DepthStencilMode.Off;

            RenderContext11.SetOffscreenRenderTargets(domeMasterTexture, null);

            RenderContext11.ClearRenderTarget(SharpDX.Color.Black);

            SetupMatricesWarpFisheye(1f);

            if (domeVertexBuffer == null)
            {
                domeVertexBuffer = new PositionColorTexturedVertexBuffer11[5];
                domeIndexBuffer = new IndexBuffer11[5];

                for (int face = 0; face < 5; face++)
                {
                    CreateDomeFaceVertexBuffer(face);
                }
            }

            RenderContext11.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            RenderContext11.BlendMode = BlendMode.None;
            RenderContext11.setRasterizerState(TriangleCullMode.Off);
            RenderContext11.DepthStencilMode = DepthStencilMode.Off;

            SharpDX.Matrix mat = (RenderContext11.World * RenderContext11.View * RenderContext11.Projection).Matrix11;
            mat.Transpose();

            WarpOutputShader.MatWVP = mat;
            WarpOutputShader.Use(RenderContext11.devContext, true);



            for (int face = 0; face < 5; face++)
            {
                RenderContext11.SetIndexBuffer(domeIndexBuffer[face]);
                RenderContext11.SetVertexBuffer(domeVertexBuffer[face]);
                RenderContext11.devContext.PixelShader.SetShaderResource(0, domeCube[face].RenderTexture.ResourceView);
                RenderContext11.devContext.DrawIndexed(domeIndexBuffer[face].Count, 0, 0);
            }

            FadeDomeTexture();

            SharpDX.Direct3D11.Texture2D.ToFile(RenderContext11.devContext, domeMasterTexture.RenderTexture.Texture, SharpDX.Direct3D11.ImageFileFormat.Png, dumpFrameParams.Name.Replace(".", string.Format("_{0:0000}.", SpaceTimeController.CurrentFrameNumber)));

        }

        RenderTargetTexture warpTexture;
        RenderTargetTexture warpTextureMSAA;
        private void RenderWarpedFisheye()
        {
            if (warpTexture == null)
            {
                warpTexture = new RenderTargetTexture(2048, 2048, 1);
                if (RenderContext11.MultiSampleCount > 1)
                {
                    warpTextureMSAA = new RenderTargetTexture(2048, 2048);
                }
            }

            // If MSAA is enabled, render to an MSAA target and perform a resolve to a non-MSAA texture.
            // Otherwise, render directly to the non-MSAA texture
            RenderTargetTexture warpRenderTarget = warpTextureMSAA != null ? warpTextureMSAA : warpTexture;

            SetupMatricesWarpFisheye(1);
            RenderContext11.SetOffscreenRenderTargets(warpRenderTarget, null);
            RenderContext11.DepthStencilMode = DepthStencilMode.Off;
            RenderFisheye(true);

            if (warpTextureMSAA != null)
            {
                RenderContext11.Device.ImmediateContext.ResolveSubresource(warpTextureMSAA.RenderTexture.Texture, 0, warpTexture.RenderTexture.Texture, 0, RenderContext11.DefaultColorFormat);
            }
            RenderContext11.Device.ImmediateContext.GenerateMips(warpTexture.RenderTexture.ResourceView);

            RenderContext11.SetDisplayRenderTargets();

            SetupMatricesWarpFisheye((float)RenderContext11.Width / (float)RenderContext11.Height);

            if (warpIndexBuffer == null)
            {
                CreateWarpVertexBuffer();
            }


 
            RenderContext11.ClearRenderTarget(SharpDX.Color.Black);
            RenderContext11.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            RenderContext11.BlendMode = BlendMode.None;
            RenderContext11.setRasterizerState(TriangleCullMode.Off);
            SharpDX.Matrix mat = (RenderContext11.World * RenderContext11.View * RenderContext11.Projection).Matrix11;

            mat.Transpose();

            WarpOutputShader.MatWVP = mat;
            WarpOutputShader.Use(RenderContext11.devContext, true);

            RenderContext11.SetIndexBuffer(warpIndexBuffer);
            RenderContext11.SetVertexBuffer(warpVertexBuffer);
            RenderContext11.devContext.PixelShader.SetShaderResource(0, warpTexture.RenderTexture.ResourceView);
            RenderContext11.devContext.DrawIndexed(warpIndexBuffer.Count, 0, 0);

            PresentFrame11(false);

        }


        // Stereo buffers
        RenderTargetTexture leftEye;
        RenderTargetTexture rightEye;
        RenderTargetTexture stereoRenderTextureLeft;
        RenderTargetTexture stereoRenderTextureRight;

        // Distortion buffers
        RenderTargetTexture undistortedAA;
        RenderTargetTexture undistorted;
        Texture11 blendTexture;
        // Full-dome buffers
        RenderTargetTexture domeCubeFaceMultisampled = null;
        RenderTargetTexture[] domeCube = new RenderTargetTexture[5];
        DepthBuffer domeZbuffer = null;
        DepthBuffer leftDepthBuffer = null;
        DepthBuffer rightDepthBuffer = null;
#endif
        RenderTargetTexture stereoRenderTextureBoth= null;
        DepthBuffer stereoDepthBufferBoth = null;
        public enum StereoModes { Off, AnaglyphRedCyan, AnaglyphYellowBlue, AnaglyphMagentaGreen, CrossEyed, SideBySide, InterlineEven, InterlineOdd, OculusRift, Right, Left };
        enum RenderTypes { DomeFront, DomeRight, DomeUp, DomeLeft, DomeBack, Normal, RightEye, LeftEye };

        public StereoModes StereoMode = StereoModes.Off;
        static RenderTypes CurrentRenderType = RenderTypes.Normal;
        public bool measuringDrag = false;
        public bool measuring = false;
        public SimpleLineList11 measureLines = null;
#if !WINDOWS_UWP
        public SkyLabel label = null;
#endif
       

        public KmlLabels KmlMarkers = null;

        public bool ShowKmlMarkers = true;
        IImageSet milkyWayBackground = null;
        IImageSet cmbBackground = null;
        public bool imageStackVisible = false;
        public List<IImageSet> ImageStackList = new List<IImageSet>();

        Coordinates[] currentViewCorners = null;

        public Coordinates[] CurrentViewCorners
        {
            get { return currentViewCorners; }
            set { currentViewCorners = value; }
        }

        public KmlViewInformation kmlViewInfo = new KmlViewInformation();
        public KmlViewInformation KmlViewInfo
        {
            get { return kmlViewInfo; }
            set { kmlViewInfo = value; }
        }

        bool hemisphereView = false;

        private void RenderFrame(SharpDX.Direct3D11.RenderTargetView targetTextureView, SharpDX.Direct3D11.DepthStencilView depthBufferView, RenderTypes renderType, int width, int height)
        {
            CurrentRenderType = renderType;

            bool offscreenRender = targetTextureView != null;

            Tile.deepestLevel = 0;
            RenderContext11.ExternalViewScale = Matrix3d.Identity;
            try
            {
                if (offscreenRender)
                {
                    RenderContext11.SetOffscreenRenderTargets(targetTextureView, depthBufferView, width, height);
                }
                else
                {
                    RenderContext11.SetDisplayRenderTargets();
                }

                //Clear the backbuffer to a black color 

                RenderContext11.ClearRenderTarget(new SharpDX.Color(SkyColor.R, SkyColor.G, SkyColor.B, SkyColor.A));



                RenderContext11.RenderType = currentImageSetfield.DataSetType;

                RenderContext11.BlendMode = BlendMode.Alpha;
                if (currentImageSetfield.DataSetType == ImageSetType.Sandbox)
                {

                    // Start Sandbox mode
                    if (RenderContext11.ExternalProjection)
                    {
                        RenderContext11.ExternalScalingFactor = Matrix.Scaling(1, 1, -1);
                    }

                    RenderContext11.SunPosition = LayerManager.GetPrimarySandboxLight();
                    RenderContext11.SunlightColor = LayerManager.GetPrimarySandboxLightColor();

                    RenderContext11.ReflectedLightColor = SysColor.FromArgb(255, 0, 0, 0);
                    RenderContext11.HemisphereLightColor = SysColor.FromArgb(255, 0, 0, 0);

                    SkyColor = SysColor.FromArgb(255, 0, 0, 0);

                    if ((int)SolarSystemTrack < (int)SolarSystemObjects.Custom)
                    {
                        double radius = Planets.GetAdjustedPlanetRadius((int)SolarSystemTrack);
                        double distance = SolarSystemCameraDistance;
                        double camAngle = fovLocal;
                        double distrad = distance / (radius * Math.Tan(.5 * camAngle));
                        if (distrad < 1)
                        {
                            planetFovWidth = Math.Asin(distrad);
                        }
                        else
                        {
                            planetFovWidth = Math.PI;
                        }
                    }
                    else

                    {
                        planetFovWidth = Math.PI;
                    }


                    SetupMatricesSolarSystem11(false, renderType);


                    Matrix3d matLocal = RenderContext11.World;
                    matLocal.Multiply(Matrix3d.Translation(-viewCamera.ViewTarget));
                    RenderContext11.World = matLocal;

                    RenderContext11.WorldBase = RenderContext11.World;
                    RenderContext11.WorldBaseNonRotating = RenderContext11.World;
                    RenderContext11.NominalRadius = 1;

                    MakeFrustum();

                    double zoom = ZoomFactor;

                    LayerManager.Draw(RenderContext11, 1.0f, false, "Sandbox", true, false);

                    //todo uwp replace labels with uwp friendly ones
#if !WINDOWS_UWP
                    if ((SolarSystemMode) && label != null && !TourPlayer.Playing)
                    {
                        label.Draw(RenderContext11, true);
                    }
#endif
                    RenderContext11.setRasterizerState(TriangleCullMode.Off);
                    // end Sandbox Mode
                }
                else if (currentImageSetfield.DataSetType == ImageSetType.SolarSystem)
                {
                    if (RenderContext11.ExternalProjection)
                    {
                        RenderContext11.ExternalScalingFactor = Matrix.Scaling(1, 1, -1);
                        //double sf = (SolarSystemCameraDistance / UiTools.MetersToSolarSystemDistance(1)  )*1000;
                       double sf = 1/(UiTools.SolarSystemToMeters(SolarSystemCameraDistance)*1000000);
                     
                       RenderContext11.ExternalViewScale = Matrix3d.Scaling(sf, sf, sf);
                    }
                    {
                        SkyColor = SysColor.FromArgb(255, 0, 0, 0); //black

                        if ((int)SolarSystemTrack < (int)SolarSystemObjects.Custom)
                        {
                            double radius = Planets.GetAdjustedPlanetRadius((int)SolarSystemTrack);
                            double distance = SolarSystemCameraDistance;
                            double camAngle = fovLocal;
                            double distrad = distance / (radius * Math.Tan(.5 * camAngle));
                            if (distrad < 1)
                            {
                                planetFovWidth = Math.Asin(distrad);
                            }
                            else
                            {
                                planetFovWidth = Math.PI;
                            }
                        }
                        else

                        {
                            planetFovWidth = Math.PI;
                        }


                        if (trackingObject == null)
                        {
                            trackingObject = Catalogs.FindCatalogObjectExact("Sun");
                        }

                        SetupMatricesSolarSystem11(true, renderType);



                        float skyOpacity = 1.0f - Planets.CalculateSkyBrightnessFactor(RenderContext11.View, viewCamera.ViewTarget);
                        if (float.IsNaN(skyOpacity))
                        {
                            skyOpacity = 0f;
                        }


                        double zoom = ZoomFactor;
                        float milkyWayBlend = (float)Math.Min(1, Math.Max(0, (Math.Log(zoom) - 8.4)) / 4.2);
                        float milkyWayBlendIn = (float)Math.Min(1, Math.Max(0, (Math.Log(zoom) - 17.9)) / 2.3);


                        if (Properties.Settings.Default.SolarSystemMilkyWay.State)
                        {
#if !WINDOWS_UWP
                            if (milkyWayBlend < 1) // Solar System mode Milky Way background
                            {
                                if (milkyWayBackground == null)
                                {
                                    milkyWayBackground = GetImagesetByName("Digitized Sky Survey (Color)");
                                }

                                if (milkyWayBackground != null)
                                {
                                    float c = ((1 - milkyWayBlend)) / 4;
                                    Matrix3d matOldMW = RenderContext11.World;
                                    Matrix3d matLocalMW = RenderContext11.World;
                                    matLocalMW.Multiply(Matrix3d.Scaling(100000, 100000, 100000));
                                    matLocalMW.Multiply(Matrix3d.RotationX(-23.5 / 180 * Math.PI));
                                    matLocalMW.Multiply(Matrix3d.RotationY(Math.PI));
                                    matLocalMW.Multiply(Matrix3d.Translation(cameraOffset));
                                    RenderContext11.World = matLocalMW;
                                    RenderContext11.WorldBase = matLocalMW;
                                    MakeFrustum();

                                    RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, 1, SysColor.FromArgb(255, 255, 255, 255)); //white);
                                    RenderContext11.DepthStencilMode = DepthStencilMode.Off;
                                    DrawTiledSphere(milkyWayBackground, c * Properties.Settings.Default.SolarSystemMilkyWay.Opacity, SysColor.FromArgb(255, 255, 255, 255));
                                    RenderContext11.World = matOldMW;
                                    RenderContext11.WorldBase = matOldMW;
                                    RenderContext11.DepthStencilMode = DepthStencilMode.ZReadWrite;
                                }
                            }
#endif
                        }

                        // CMB

                        float cmbBlend = (float)Math.Min(1, Math.Max(0, (Math.Log(zoom) - 33)) / 2.3);


                        double cmbLog = Math.Log(zoom);

                        if (Properties.Settings.Default.SolarSystemCMB.State)
                        {
                            if (cmbBlend > 0) // Solar System mode Milky Way background
                            {
                                if (cmbBackground == null)
                                {
                                    cmbBackground = GetImagesetByName("Planck CMB");
                                }

                                if (cmbBackground != null)
                                {
                                    float c = ((cmbBlend)) / 16;
                                    Matrix3d matOldMW = RenderContext11.World;
                                    Matrix3d matLocalMW = RenderContext11.World;

                                    matLocalMW.Multiply(Matrix3d.Scaling(2.9090248982E+15, 2.9090248982E+15, 2.9090248982E+15));
                                    matLocalMW.Multiply(Matrix3d.RotationX(-23.5 / 180 * Math.PI));
                                    matLocalMW.Multiply(Matrix3d.RotationY(Math.PI));

                                    RenderContext11.World = matLocalMW;
                                    RenderContext11.WorldBase = matLocalMW;
                                    MakeFrustum();

                                    RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, 1, SysColor.FromArgb(255, 255, 255, 255));

                                    RenderContext11.DepthStencilMode = DepthStencilMode.Off;
                                    DrawTiledSphere(cmbBackground, c * Properties.Settings.Default.SolarSystemCMB.Opacity, SysColor.FromArgb(255, 255, 255, 255));
                                    RenderContext11.World = matOldMW;
                                    RenderContext11.WorldBase = matOldMW;
                                    RenderContext11.DepthStencilMode = DepthStencilMode.ZReadWrite;
                                }
                            }
                        }




                        {
                            Matrix3d matOld = RenderContext11.World;

                            Matrix3d matLocal = RenderContext11.World;
                            matLocal.Multiply(Matrix3d.Translation(viewCamera.ViewTarget));
                            RenderContext11.World = matLocal;
                            MakeFrustum();

                            if (Properties.Settings.Default.SolarSystemCosmos.State)
                            {
                                RenderContext11.DepthStencilMode = DepthStencilMode.Off;

                                Grids.DrawCosmos3D(RenderContext11, Properties.Settings.Default.SolarSystemCosmos.Opacity * skyOpacity);
                                RenderContext11.DepthStencilMode = DepthStencilMode.ZReadWrite;
                            }

#if !WINDOWS_UWP
                            if (true)
                            {
                                RenderContext11.DepthStencilMode = DepthStencilMode.Off;

                                Grids.DrawCustomCosmos3D(RenderContext11, skyOpacity);

                                RenderContext11.DepthStencilMode = DepthStencilMode.ZReadWrite;
                            }

#endif
                            if (Properties.Settings.Default.SolarSystemMilkyWay.State && milkyWayBlendIn > 0)
                            {
                                Grids.DrawGalaxy3D(RenderContext11, Properties.Settings.Default.SolarSystemMilkyWay.Opacity * skyOpacity * milkyWayBlendIn);
                            }


                            if (Properties.Settings.Default.SolarSystemStars.State)
                            {
                                Grids.DrawStars3D(RenderContext11, Properties.Settings.Default.SolarSystemStars.Opacity * skyOpacity);
                            }


                            LayerManager.Draw(RenderContext11, 1.0f, true, "Sky", true, false);

                            RenderContext11.World = matOld;
                            MakeFrustum();
                        }


                        if (SolarSystemCameraDistance < 15000)
                        {
                            SetupMatricesSolarSystem11(false, renderType);

#if !BASICWWT
                            if (Properties.Settings.Default.SolarSystemMinorPlanets.State)
                            {
                                MinorPlanets.DrawMPC3D(RenderContext11, Properties.Settings.Default.SolarSystemMinorPlanets.Opacity, viewCamera.ViewTarget);
                            }
#endif
                            Planets.DrawPlanets3D(RenderContext11, Properties.Settings.Default.SolarSystemPlanets.Opacity, viewCamera.ViewTarget);

                        }

                        double p = Math.Log(zoom);
                        double d = (180 / SolarSystemCameraDistance) * 100;

                        float sunAtDistance = (float)Math.Min(1, Math.Max(0, (Math.Log(zoom) - 7.5)) / 3);

                        if (sunAtDistance > 0 && Settings.Active.SolarSystemPlanets)
                        {
                            Planets.DrawPointPlanet(RenderContext11, new Vector3d(0, 0, 0), (float)d * sunAtDistance, SysColor.FromArgb(192, 191, 128), false, 1);
                        }

                        //todo uwp replace with uwp method
#if !WINDOWS_UWP
                        if ((SolarSystemMode) && label != null && !TourPlayer.Playing)
                        {
                            label.Draw(RenderContext11, true);
                        }
#endif
                    }

                    RenderContext11.setRasterizerState(TriangleCullMode.Off);
                }
                else
                {

                    if (currentImageSetfield.DataSetType == ImageSetType.Panorama || currentImageSetfield.DataSetType == ImageSetType.Sky)
                    {
                        if (RenderContext11.ExternalProjection)
                        {
                            RenderContext11.ExternalScalingFactor = Matrix.Scaling(100, 100, -100);
                        }

                        SkyColor = SysColor.FromArgb(255, 0, 0, 0);

                        if ((int)renderType < 5)
                        {
                            SetupMatricesSpaceDome(false, renderType);
                        }
                        else
                        {
                            SetupMatricesSpace11(ZoomFactor, renderType);
                            RenderContext11.ProjectAtInfinity = ProjectAtInfinity;
                        }
                        RenderContext11.DepthStencilMode = DepthStencilMode.Off;
                    }
                    else
                    {
                        if (RenderContext11.ExternalProjection)
                        {
                            RenderContext11.ExternalScalingFactor = Matrix.Scaling(1, 1, -1);
                        }

                        if (Settings.DomeView)
                        {
                            SetupMatricesLandDome(renderType);
                        }
                        else
                        {
                            SetupMatricesLand11(renderType);
                        }
                        RenderContext11.DepthStencilMode = DepthStencilMode.ZReadWrite;
                    }

                    ComputeViewParameters(currentImageSetfield);

                    // Update Context pane
                    CurrentViewCorners = new Coordinates[]
                    {
                        GetCoordinatesForScreenPoint(0, 0),
                        GetCoordinatesForScreenPoint(ViewWidth, 0),
                        GetCoordinatesForScreenPoint(ViewWidth, RenderContext11.Height),
                        GetCoordinatesForScreenPoint(0, RenderContext11.Height)
                    };

                    Coordinates temp = GetCoordinatesForScreenPoint(ViewWidth / 2, RenderContext11.Height / 2);
#if !WINDOWS_UWP
                    if (Earth3d.MainWindow.contextPanel != null && ((int)renderType > 4 || renderType == RenderTypes.DomeFront))
                    {
                        Earth3d.MainWindow.contextPanel.SetViewRect(CurrentViewCorners);
                    }
                    Earth3d.MainWindow.UpdateKmlViewInfo();             

                    if (KmlMarkers != null)
                    {
                        KmlMarkers.ClearGroundOverlays();
                    }
#endif
                    string referenceFrame = GetCurrentReferenceFrame();


                    if (PlanetLike || Space)
                    {
                        LayerManager.PreDraw(RenderContext11, 1.0f, Space, referenceFrame, true);
                    }



                    if (Properties.Settings.Default.EarthCutawayView.State && !Space && currentImageSetfield.DataSetType == ImageSetType.Earth)
                    {
                        Grids.DrawEarthStructure(RenderContext11, 1f);
                    }

                    RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, 1, SysColor.FromArgb(255, 255, 255, 255));
#if !WINDOWS_UWP
                    if (KmlMarkers != null)
                    {
                        KmlMarkers.SetupGroundOverlays(RenderContext11);
                    }
#endif
                    //if (PlanetLike)
                    {
                        RenderContext11.setRasterizerState(TriangleCullMode.Off);
                    }

                    // Call DrawTiledSphere instead of PaintLayerFull, because PaintLayerFull
                    // will reset ground layer state
                    DrawTiledSphere(currentImageSetfield, 1.0f, SysColor.FromArgb(255, 255, 255, 255));


                    if (imageStackVisible)
                    {
                        foreach (ImageSet set in ImageStackList)
                        {
                            PaintLayerFull11(set, StudyOpacity);
                        }
                    }

                    if (studyImageset != null)
                    {
                        if (studyImageset.DataSetType != currentImageSetfield.DataSetType)
                        {
                            SetStudy(null);
                        }
                        else
                        {
                            PaintLayerFull11(studyImageset, StudyOpacity);
                        }
                    }


                    if (previewImageset != null && PreviewBlend.State)
                    {
                        if (previewImageset.DataSetType != currentImageSetfield.DataSetType)
                        {
                            previewImageset = null;
                        }
                        else
                        {
                            PaintLayerFull11(previewImageset, PreviewBlend.Opacity * 100.0f);
                        }
                    }
                    else
                    {
                        PreviewBlend.State = false;
                        previewImageset = null;
                    }


                    if (Space && (currentImageSetfield.Name == "Plotted Sky"))
                    {

                        Grids.DrawStars(RenderContext11, 1f);
                    }

                    if (Space && Properties.Settings.Default.ShowSolarSystem.State)
                    {
                        Planets.DrawPlanets(RenderContext11, Properties.Settings.Default.ShowSolarSystem.Opacity);
                    }


                    if (PlanetLike || Space)
                    {
                        if (!Space)
                        {
                            //todo fix this for other planets..
                            double angle = Coordinates.MstFromUTC2(SpaceTimeController.Now, 0) / 180.0 * Math.PI;
                            RenderContext11.WorldBaseNonRotating = Matrix3d.RotationY(angle) * RenderContext11.WorldBase;
                            RenderContext11.NominalRadius = currentImageSetfield.MeanRadius;
                        }
                        else
                        {
                            RenderContext11.WorldBaseNonRotating = RenderContext11.World;
                            RenderContext11.NominalRadius = currentImageSetfield.MeanRadius;
                            RenderContext11.DepthStencilMode = DepthStencilMode.Off;
                        }

                        LayerManager.Draw(RenderContext11, 1.0f, Space, referenceFrame, true, Space);
                    }

                    RenderContext11.ProjectAtInfinity = false;

                    if (Space && !hemisphereView && Settings.Active.LocalHorizonMode && !Settings.DomeView && !ProjectorServer && !config.UseDistrotionAndBlend)
                    {
                        Grids.DrawHorizon(RenderContext11, 1f);
                    }

                    if (Settings.Active.ShowClouds && !Space && currentImageSetfield.DataSetType == ImageSetType.Earth)
                    {
                        DrawClouds();
                    }


                    // Draw Field of view indicator

                    if (Settings.Active.ShowFieldOfView)
                    {
                        fovBlend.TargetState = true;
                    }
                    else
                    {
                        fovBlend.TargetState = false;
                    }

                    if (fovBlend.State)
                    {
                        if (fov != null && Space)
                        {
                            fov.Draw3D(RenderContext11, fovBlend.Opacity, RA, Dec);
                        }
                    }
                    //todo uwm replace with uwp methiod of labels
#if !WINDOWS_UWP
                    if (label != null && !TourPlayer.Playing)
                    {
                        label.Draw(RenderContext11, PlanetLike);
                    }

                    if (ShowKmlMarkers && KmlMarkers != null)
                    {
                        KmlMarkers.DrawLabels(RenderContext11);
                    }
#endif


                    // End Planet & space
                }

                if (uiController != null)
                {
                    {
                        uiController.Render(this);
                    }
                }

                if (videoOverlay != null)
                {
                    if ((int)renderType < 5)
                    {
                        SetupMatricesVideoOverlayDome(false, renderType);
                    }
                    else
                    {
                        SetupMatricesVideoOverlay(ZoomFactor);
                    }
                    DepthStencilMode mode = RenderContext11.DepthStencilMode = DepthStencilMode.Off;
                    PaintLayerFull11(videoOverlay, 100f);
                    RenderContext11.DepthStencilMode = mode;
                }

                if (measuringDrag && measureLines != null)
                {
                    measureLines.DrawLines(RenderContext11, 1.0f, SysColor.FromArgb(255, 255, 255, 0));

                }

                if (Space)
                {
                    if (settingModel != null)
                    {
                        Matrix3d worldSaved = RenderContext11.World;
                        Matrix3d viewSaved = RenderContext11.View;
                        RenderContext11.SunlightColor = SysColor.FromArgb(255, 13, 5, 5);
                        RenderContext11.AmbientLightColor = SysColor.FromArgb(255, 1, 1, 13);
                        RenderContext11.SunPosition = new Vector3d(0, 30, 0);
                        RenderContext11.View = Matrix3d.RotationX(-Math.PI / 2) * Matrix3d.Scaling(.54, .54, .54);
                        RenderContext11.World = Matrix3d.Identity;
                        settingModel.UseCurrentAmbient = true;
                        settingModel.Render(RenderContext11, 1);
                        RenderContext11.World = worldSaved;
                        RenderContext11.View = viewSaved;
                    }
                }

                RenderContext11.ExternalViewScale = Matrix3d.Identity;
                RenderContext11.UpdateProjectionConstantBuffers();

                if (LeftController.Active || RightController.Active)
                {
                    HandleController(LeftController);
                    HandleController(RightController);                 
                }


#if !BASICWWT
                if (Properties.Settings.Default.ShowCrosshairs && !TourPlayer.Playing && renderType == RenderTypes.Normal && !megaFrameDump)
                {
                    float aspect = RenderContext11.ViewPort.Height / RenderContext11.ViewPort.Width;


                    crossHairPoints[0].X = .01f * aspect;
                    crossHairPoints[1].X = -.01f * aspect;
                    crossHairPoints[0].Y = 0;
                    crossHairPoints[1].Y = 0;
                    crossHairPoints[0].Z = .9f;
                    crossHairPoints[1].Z = .9f;
                    crossHairPoints[0].W = 1f;
                    crossHairPoints[1].W = 1f;
                    crossHairPoints[0].Color = SysColor.FromArgb(255, 255, 255, 255);
                    crossHairPoints[1].Color = SysColor.FromArgb(255, 255, 255, 255);

                    crossHairPoints[2].X = 0;
                    crossHairPoints[3].X = 0;
                    crossHairPoints[2].Y = -.01f;
                    crossHairPoints[3].Y = .01f;
                    crossHairPoints[2].Z = .9f;
                    crossHairPoints[3].Z = .9f;
                    crossHairPoints[2].W = 1f;
                    crossHairPoints[3].W = 1f;
                    crossHairPoints[2].Color = SysColor.FromArgb(255, 255, 255, 255);
                    crossHairPoints[3].Color = SysColor.FromArgb(255, 255, 255, 255);

                    Sprite2d.DrawLines(RenderContext11, crossHairPoints, 4, SharpDX.Matrix.OrthoLH(1f, 1f, 1, -1), false);

                }
#endif
#if !WINDOWS_UWP

                if (Properties.Settings.Default.ShowTouchControls && (!TourPlayer.Playing || mover == null) && (renderType == RenderTypes.Normal || renderType == RenderTypes.LeftEye || renderType == RenderTypes.RightEye) && !rift && !megaFrameDump && !config.UseDistrotionAndBlend)
                {
                    Earth3d.MainWindow.DrawTouchControls();
                }


                Earth3d.MainWindow.DrawKinectUI();

                SetupMatricesAltAz();
                Reticle.DrawAll(RenderContext11);
#endif

            }
            catch (Exception e)
            {
                if (Utils.Logging) { Utils.WriteLogMessage("RenderFrame: Exception"); }
                if (offscreenRender)
                {
                    throw e;
                }
            }
            finally
            {
                if (offscreenRender)
                {
                    RenderContext11.SetDisplayRenderTargets();
                }
            }

#if !WINDOWS_UWP
            PresentFrame11(offscreenRender);
#endif
        }

        private void HandleController (HandController controller)
        {
            RenderContext11.ExternalViewScale = Matrix3d.Identity;
            if (controller.Active)
            {

                if (controller.Events.HasFlag(HandControllerStatus.MenuDown) && controller.Hand == HandType.Left)
                {
                    PreviousView();
                }

                if (controller.Events.HasFlag(HandControllerStatus.MenuDown) && controller.Hand == HandType.Right )
                {
                    NextView();
                }


                var leftPos = controller.Position;
                var endPos = controller.Forward;
                var up = controller.Up;

                Coordinates result = new Coordinates();

                float scale = 1.0f / RenderContext11.ExternalScalingFactor.M11;
                Vector3d pos1 = new Vector3d(leftPos.X * scale, leftPos.Y * scale, leftPos.Z * scale);
                Vector3d pos = new Vector3d(leftPos.X, leftPos.Y, leftPos.Z);

                var mwv = RenderContext11.World * RenderContext11.View;
                mwv.Invert();
                var pos1t = new Vector3d(pos1.X, pos1.Y, -pos1.Z);
                var endPost = new Vector3d(endPos.X, endPos.Y, -endPos.Z);
                var upVector = new Vector3d(up.X, up.Y, -up.Z);


                pos1t.TransformCoordinate(mwv);
                mwv.MultiplyVector(ref endPost);
                mwv.MultiplyVector(ref upVector);

                var upNorth = new Vector3d(0, 1, 0);
                var crossNorthEnd = Vector3d.Cross(endPost, upNorth);
                crossNorthEnd.Normalize();

                var trueNorthUp = Vector3d.Cross(endPost, crossNorthEnd);
                var crossUpEnd = Vector3d.Cross(endPost, upVector);

                var v1 = trueNorthUp;
                var v2 = crossUpEnd;
                var vz = Vector3d.Cross(v1, v2);
                vz.Normalize();
                var v3 = Vector3d.Cross(vz, v1);

                var xp = Vector3d.Dot(v2, v1);
                var yp = Vector3d.Dot(v2, v3);

                var angle = Math.Atan2(xp, yp);
                if (Vector3d.Dot(upVector, trueNorthUp) > 0)
                {
                    angle = Math.PI - angle;
                }

                pos1t = new Vector3d(pos1t.X, pos1t.Y, pos1t.Z);
                endPost = new Vector3d(endPost.X, endPost.Y, endPost.Z);

                bool inThere = SphereIntersectRay(pos1t, endPost, out result);

                string constellation = "uma";
                if (controller.Hand == HandType.Left)
                {
                    constellation = constellationCheck.FindConstellationForPoint(result.RA, result.Dec);
                    var v = Constellations.FullName(constellation);
                    var filter = new ConstellationFilter();
                    filter.Set(constellation, true);
                    Properties.Settings.Default.ConstellationArtFilter = filter;
                    Properties.Settings.Default.ConstellationNamesFilter = filter;

                    RenderEngine.pointerConstellation = v;

                }

                if (!(Space && Properties.Settings.Default.LocalHorizonMode))
                {
                    if ((Math.Abs(controller.ThumbX) > .2 || Math.Abs(controller.ThumbY) > .2) && controller.Hand == HandType.Right)
                    {
                        MoveView(controller.ThumbX * 5, controller.ThumbY * 5, false);
                    }
                }

                if (Space)
                {
                    if (!controller.Status.HasFlag(HandControllerStatus.TriggerDown) && controller.Trigger > 0)
                    {
                        // Start capture of zoom Window
                        ZoomWindowVisible = true;
                        ZoomWindowRefresh = true;

                        if (controller.Trigger < .4)
                        {
                            ZoomWindowCamera.RA = result.RA;
                            ZoomWindowCamera.Dec = result.Dec;
                            ZoomWindowCamera.Rotation = angle;
                        }
                        ZoomWindowCamera.Zoom = Math.Pow(2, (.8 - controller.Trigger) * 10.614);
                        showRingMenuLeft = true;
                    }

                    if (controller.Status.HasFlag(HandControllerStatus.TriggerDown) && controller.Hand == HandType.Left)
                    {

                        if (zoomWindowRenderTarget != null)
                        {
                            finderPanel.ZoomTexture = zoomWindowRenderTarget.RenderTexture;
                        }
                        IPlace closetPlace = ContextSearch.FindClosestMatch(constellation, result.RA, result.Dec, .5f);

                        if (closetPlace == null)
                        {
                            closetPlace = new TourPlace(Language.GetLocalizedText(90, "No Object"), result.Dec, result.RA, Classification.Unidentified, constellation, ImageSetType.Sky, -1);
                        }

                        finderPanel.Target = closetPlace;
                        if (controller.Events.HasFlag(HandControllerStatus.TriggerDown))
                        {
                            // Start capture of zoom Window
                            ZoomWindowVisible = true;
                            ZoomWindowRefresh = true;
                            ZoomWindowCamera = new CameraParameters();
                            ZoomWindowCamera.RA = closetPlace.RA;
                            ZoomWindowCamera.Dec = closetPlace.Dec;
                            ZoomWindowCamera.Zoom = 10;
                            ZoomWindowCamera.Rotation = angle;
                        }

                        showRingMenuLeft = true;
                        ringMenu.SetActivePanel(1);
                    }
                }
                else
                {
                    

                    if (controller.Events.HasFlag(HandControllerStatus.GripDown))
                    {
                        if (viewCamera.Target != SolarSystemObjects.Earth)
                        {
                            viewCamera.Target = SolarSystemObjects.Earth;
                            viewCamera.Zoom = .1;
                        }
                        else
                        {
                            TrackISS();
                        }
                    }
                    SpaceTimeController.SyncToClock = false;

                    if (controller.Status.HasFlag(HandControllerStatus.TriggerDown) && controller.Hand == HandType.Right)
                    {
                        TargetZoom *= .98;
                    }

                    if (controller.Status.HasFlag(HandControllerStatus.TriggerDown) && controller.Hand == HandType.Left)
                    {
                        TargetZoom /= .98;
                    }
                    double factor = .05;

                    if (Math.Abs(controller.ThumbX) > .2 && controller.Hand == HandType.Left)
                    {
                        CameraRotateTarget = CameraRotateTarget + controller.ThumbX * factor;
                    }

                    if (Math.Abs(controller.ThumbY) > .2 && controller.Hand == HandType.Left)
                    {
                        CameraAngleTarget = CameraAngleTarget + controller.ThumbY * factor;
                    }

                    if (controller.Status.HasFlag(HandControllerStatus.StickDown) && controller.Hand == HandType.Left)
                    {
                        TargetZoom /= .98;
                    }

                    if (CameraAngleTarget < -1.52)
                    {
                        CameraAngleTarget = -1.52;
                    }

                    if (CameraAngleTarget > 0)
                    {
                        CameraAngleTarget = 0;
                    }
                }

                Matrix3d worldSaved = RenderContext11.World;
                Matrix3d localWorld = new Matrix3d();

                Matrix m1 = Matrix.LookAtLH(new Vector3(), new Vector3d(endPos.X, endPos.Y, -endPos.Z).Vector3, new Vector3d(up.X, up.Y, -up.Z).Vector3);
                m1.Invert();
                m1 = Matrix.Multiply(m1, Matrix.Translation(new Vector3d(pos.X, pos.Y, -pos.Z).Vector3));
                if (scale != 1)
                {
                    m1 = m1 * Matrix.Scaling(scale);
                }


                localWorld.Matrix = m1;
                RenderContext11.World = localWorld;
                RenderContext11.View = Matrix3d.Identity;

                if (Space && controller.Hand == HandType.Left)
                {
                    leftPointerRay.Draw(RenderContext11, 1, SysColor.Green);
                }

                RenderContext11.DepthStencilMode = DepthStencilMode.ZReadWrite;
                RenderContext11.setRasterizerState(TriangleCullMode.CullCounterClockwise);
                DrawHandControllerModel(controller);

                RenderContext11.DepthStencilMode = DepthStencilMode.ZWriteOnly;
                RenderContext11.BlendMode = BlendMode.Alpha;


                if (controller.Hand == HandType.Left)
                {
                    if (controller.Events.HasFlag(HandControllerStatus.TouchDown) && !showRingMenuLeft)
                    {
                        showRingMenuLeft = true;
                        ringMenu.SetActivePanel(0);
                    }
                }

                if (controller.Hand == HandType.Right)
                {
                    if (controller.Events.HasFlag(HandControllerStatus.TouchDown) && !showRingMenuRight)
                    {
                        showRingMenuRight = true;
                        ringMenu.SetActivePanel(0);
                    }
                }

                if (Space)
                {
                    if ((showRingMenuLeft && controller.Hand == HandType.Left) || (showRingMenuRight && controller.Hand == HandType.Right))
                    {
                        ringMenu.HandleControlerInput(controller);

                        m1 = Matrix.LookAtLH(new Vector3(), new Vector3d(endPos.X, endPos.Y, -endPos.Z).Vector3, new Vector3d(up.X, up.Y, -up.Z).Vector3);
                        m1.Invert();
                        m1 = Matrix.RotationX(.5f) * m1;
                        m1 = Matrix.Scaling(1, -1, 1) * m1;
                        m1 = Matrix.Translation(-200, -500, -70) * m1;


                        m1 = m1 * Matrix.Scaling(.00050f);

                        m1 = Matrix.Multiply(m1, Matrix.Translation(new Vector3d(pos.X, pos.Y, -pos.Z).Vector3));
                        if (scale != 1)
                        {
                            m1 = m1 * Matrix.Scaling(scale);
                        }
                        localWorld.Matrix = m1;
                        RenderContext11.World = localWorld;
                        ringMenu.Draw(RenderContext11, 1, SysColor.BlueViolet);
                    }
                }
                RenderContext11.World = worldSaved;

            }
        }
        
        public void MoveView(double amountX, double amountY, bool mouseDrag)
        {
            if (currentImageSetfield == null)
            {
                return;
            }
            Tracking = false;
            double angle = Math.Atan2(amountY, amountX);
            double distance = Math.Sqrt(amountY * amountY + amountX * amountX);
            if (SolarSystemMode)
            {
                amountX = Math.Cos(angle - CameraRotate) * distance;
                amountY = Math.Sin(angle - CameraRotate) * distance;
            }
            else if (!PlanetLike)
            {
                amountX = Math.Cos(angle + CameraRotate) * distance;
                amountY = Math.Sin(angle + CameraRotate) * distance;
            }
            else
            {
                amountX = Math.Cos(angle - CameraRotate) * distance;
                amountY = Math.Sin(angle - CameraRotate) * distance;
            }

            MoveViewNative(amountX, amountY, mouseDrag);
        }

        public void MoveViewNative(double amountX, double amountY, bool mouseDrag)
        {
            double scaleY = GetPixelScaleY();
            double scaleX = GetPixelScaleX();


            if (currentImageSetfield.DataSetType == ImageSetType.SolarSystem || SandboxMode)
            {
                if (scaleY > .05999)
                {
                    scaleX = scaleY;
                }
            }

            if (Space && Settings.Active.GalacticMode)
            {
                amountX = -amountX;
            }

            if (Space && (Settings.Active.LocalHorizonMode || Settings.Active.GalacticMode))
            {
                targetAlt += (amountY) * scaleY;
                if (targetAlt > Properties.Settings.Default.MaxLatLimit)
                {
                    targetAlt = Properties.Settings.Default.MaxLatLimit;
                }
                if (targetAlt < -Properties.Settings.Default.MaxLatLimit)
                {
                    targetAlt = -Properties.Settings.Default.MaxLatLimit;
                }

            }
            else
            {
                TargetLat += (amountY) * scaleY;

                if (TargetLat > Properties.Settings.Default.MaxLatLimit)
                {
                    TargetLat = Properties.Settings.Default.MaxLatLimit;
                }
                if (TargetLat < -Properties.Settings.Default.MaxLatLimit)
                {
                    TargetLat = -Properties.Settings.Default.MaxLatLimit;
                }
            }
            if (Space && (Settings.Active.LocalHorizonMode || Settings.Active.GalacticMode))
            {
                targetAz = ((targetAz + amountX * scaleX) + 720) % 360;
            }
            else
            {
                TargetLong += (amountX) * scaleX;

                TargetLong = ((TargetLong + 900.0) % 360.0) - 180.0;
            }
        }

        public double GetPixelScaleX()
        {
            double lat = ViewLat;

            double cosLat = 1;
            if (ViewLat > 89.9999)
            {
                cosLat = Math.Cos(89.9999 * RC);
            }
            else
            {
                cosLat = Math.Cos(lat * RC);

            }

            double zz = (90 - ZoomFactor / 6);
            double zcos = Math.Cos(zz * RC);

            return GetPixelScaleY() / Math.Max(zcos, cosLat);
        }

        public double GetPixelScaleY()
        {
            if (SolarSystemMode)
            {
                if ((int)SolarSystemTrack < (int)SolarSystemObjects.Custom)
                {
                    return Math.Min(.06, 545000 * Math.Tan(Math.PI / 4) * ZoomFactor / RenderContext11.Height);
                }
                else
                {

                    return .06;
                }
            }
            else if (currentImageSetfield != null && (currentImageSetfield.DataSetType == ImageSetType.Sky || currentImageSetfield.DataSetType == ImageSetType.Panorama))
            {
                double val = FovAngle / RenderContext11.Height;

                return val;
            }
            else if (SandboxMode)
            {
                return .06;
            }
            else
            {
                return ((baseTileDegrees / ((double)Math.Pow(2, viewTileLevel))) / (double)tileSizeY) / 5;
            }
        }

        private void RenderZoomWindow(SharpDX.Direct3D11.RenderTargetView targetTextureView, CameraParameters camera, int width, int height)
        {
            Tile.MakeHighPriority = true;
            Tile.NoBlend = true;
            var saveEP = RenderContext11.ExternalScalingFactor;
            bool useEP = RenderContext11.ExternalProjection;
            RenderContext11.ExternalProjection = false;
            RenderContext11.ExternalViewScale = Matrix3d.Identity;
            CurrentRenderType = RenderTypes.Normal;

            bool offscreenRender = targetTextureView != null;

            Tile.deepestLevel = 0;

            try
            {

                RenderContext11.SetOffscreenRenderTargets(targetTextureView, null, width, height);

                //Clear the backbuffer to a black color 

                RenderContext11.ClearRenderTarget(new SharpDX.Color(255, 0, 0, 255));

                RenderContext11.RenderType = ImageSetType.Sky;

                RenderContext11.BlendMode = BlendMode.Alpha;

                SkyColor = SysColor.FromArgb(255, 0, 0, 0);

                SetupMatricesSpaceForZoomWindow(camera);

                RenderContext11.DepthStencilMode = DepthStencilMode.Off;

                //ComputeViewParameters(currentImageSetfield);

                string referenceFrame = GetCurrentReferenceFrame();

                if (Space)
                {
                //    LayerManager.PreDraw(RenderContext11, 1.0f, Space, referenceFrame, true);
                }

                RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, 1, SysColor.FromArgb(255, 255, 255, 255));
                {
                    RenderContext11.setRasterizerState(TriangleCullMode.Off);
                }

                // Call DrawTiledSphere instead of PaintLayerFull, because PaintLayerFull
                // will reset ground layer state
                DrawTiledSphere(currentImageSetfield, 1.0f, SysColor.FromArgb(255, 255, 255, 255));

                if (studyImageset != null)
                {
                    if (studyImageset.DataSetType != currentImageSetfield.DataSetType)
                    {
                        SetStudy(null);
                    }
                    else
                    {
                        PaintLayerFull11(studyImageset, StudyOpacity);
                    }
                }

                if (Space && (currentImageSetfield.Name == "Plotted Sky"))
                {

                    Grids.DrawStars(RenderContext11, 1f);
                }

                if (Space && Properties.Settings.Default.ShowSolarSystem.State)
                {
                    Planets.DrawPlanets(RenderContext11, Properties.Settings.Default.ShowSolarSystem.Opacity);
                }


                if (PlanetLike || Space)
                {
                    if (!Space)
                    {
                        //todo fix this for other planets..
                        double angle = Coordinates.MstFromUTC2(SpaceTimeController.Now, 0) / 180.0 * Math.PI;
                        RenderContext11.WorldBaseNonRotating = Matrix3d.RotationY(angle) * RenderContext11.WorldBase;
                        RenderContext11.NominalRadius = currentImageSetfield.MeanRadius;
                    }
                    else
                    {
                        RenderContext11.WorldBaseNonRotating = RenderContext11.World;
                        RenderContext11.NominalRadius = currentImageSetfield.MeanRadius;
                        RenderContext11.DepthStencilMode = DepthStencilMode.Off;
                    }

                    //LayerManager.Draw(RenderContext11, 1.0f, Space, referenceFrame, true, Space);
                }
            }
            catch (Exception e)
            {
                if (Utils.Logging) { Utils.WriteLogMessage("RenderFrame: Exception"); }
                if (offscreenRender)
                {
                    throw e;
                }
            }
            finally
            {

                RenderContext11.SetDisplayRenderTargets();

                RenderContext11.ExternalScalingFactor = saveEP;
                RenderContext11.ExternalProjection = useEP;
                Tile.MakeHighPriority = false;
                Tile.NoBlend = false ;
            }
        }

        bool HandControlModelLoading = false;

        private void DrawHandControllerModel(HandController controller)
        {
            if (LeftHandControllerModel == null)
            {
                if (!HandControlModelLoading)
                {
#if WINDOWS_UWP
                    HandControlModelLoading = true;
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        string path = System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "UwpRenderEngine\\Resources");
                        LeftHandControllerModel = new Object3d(path + "\\HandControllerRight.glb", false, false, true, SysColor.White);
                        RightHandControllerModel = new Object3d(path + "\\HandControllerRight.glb", false, true, true, SysColor.White);
                    });
#endif
                }
            }
            else
            {
                var leftPos = controller.Position;
                var endPos = controller.Forward;
                var up = controller.Up;

                float scale = 1 / RenderContext11.ExternalScalingFactor.M11;
                Vector3d pos = new Vector3d(leftPos.X, leftPos.Y, leftPos.Z);
                Matrix3d worldSaved = RenderContext11.World;
                Matrix3d localWorld = new Matrix3d();

                Matrix m1 = Matrix.LookAtLH(new Vector3(), new Vector3d(endPos.X, endPos.Y, -endPos.Z).Vector3, new Vector3d(up.X, up.Y, -up.Z).Vector3);
                m1.Invert();
                m1 = Matrix.Multiply(m1, Matrix.Translation(new Vector3d(pos.X, pos.Y, -pos.Z).Vector3));
                if (scale != 1)
                {
                    m1 = m1 * Matrix.Scaling(scale);
                }
                bool left = controller.Hand == HandType.Left;



                Matrix m2 = Matrix.RotationY((float)Math.PI);
                Matrix m3 = Matrix.Scaling(.106f,.106f,.104f);
                Matrix m4 = Matrix.RotationX(.612f);
                Matrix m5 = Matrix.Translation(left ? -.112f : .112f, .185f, .48f);
                    m1 = m2 * m1;
                m1 = m3 * m1;
                m1 = m4 * m1;
                m1 = m5 * m1;
                localWorld.Matrix = m1;
                RenderContext11.World = localWorld;
                RenderContext11.View = Matrix3d.Identity;

                if (left)
                {
                    if (LeftHandControllerModel != null)
                    {
                        LeftHandControllerModel.Render(RenderContext11, 1);
                    }
                }
                else
                {
                    if (RightHandControllerModel != null)
                    {
                        RightHandControllerModel.Render(RenderContext11, 1);
                    }
                }
                RenderContext11.World = worldSaved; 
            }
        }

        static public string pointerConstellation = "";

        public Coordinates GetCoordinatesForScreenPoint(int x, int y)
        {
            Coordinates result = new Coordinates(0, 0);
            Rectangle rect = new Rectangle(0, 0, RenderContext11.Width, RenderContext11.Height);
            Vector3d PickRayOrig;
            Vector3d PickRayDir;
            Vector2d pt = new Vector2d(x, y);
            TransformPickPointToWorldSpace(pt, rect.Width, rect.Height, out PickRayOrig, out PickRayDir);
            if (Space)
            {
                result = Coordinates.CartesianToSpherical(PickRayDir);

            }
            else if (PlanetLike)
            {
                bool inThere = SphereIntersectRay(PickRayOrig, PickRayDir, out result);

            }

            return result;
        }

        PositionColoredTextured[] crossHairPoints = new PositionColoredTextured[4];

        private string GetCurrentReferenceFrame()
        {
            if (!string.IsNullOrEmpty(currentImageSetfield.ReferenceFrame))
            {
                return currentImageSetfield.ReferenceFrame;
            }
            if (currentImageSetfield.DataSetType == ImageSetType.Earth)
            {
                return "Earth";
            }
            if (currentImageSetfield.Name == "Visible Imagery" && currentImageSetfield.Url.ToLower().Contains("mars"))
            {

                currentImageSetfield.ReferenceFrame = "Mars";
                return currentImageSetfield.ReferenceFrame;
            }

            if (currentImageSetfield.DataSetType == ImageSetType.Planet)
            {
                foreach (string name in Enum.GetNames(typeof(SolarSystemObjects)))
                {
                    if (currentImageSetfield.Name.ToLower().Contains(name.ToLower()))
                    {
                        currentImageSetfield.ReferenceFrame = name;
                        return name;
                    }
                }
            }
            if (currentImageSetfield.DataSetType == ImageSetType.Sky)
            {
                return "Sky";
            }
            return "";
        }

        private static Matrix3d bias = Matrix3d.Scaling(.5f, -.5f, .5f) * Matrix3d.Translation(.5f, .5f, .5f);

        bool flush = true;

        SharpDX.Direct3D11.Query query = null;

        private void PresentFrame11(bool renderToTexture)
        {
            // Update the screen
            if (!renderToTexture)
            {
#if !WINDOWS_UWP
                //todo uwp find alternative way to do this in uwp
                FadeFrame();

                NetControl.WaitForNetworkSync();
#endif
                RenderContext11.Present(Properties.Settings.Default.FrameSync);

                if (flush)
                {
                    RenderContext11.devContext.Flush();
                    SharpDX.Direct3D11.QueryDescription qd = new SharpDX.Direct3D11.QueryDescription();

                    qd.Type = SharpDX.Direct3D11.QueryType.Event;


                    query = new SharpDX.Direct3D11.Query(RenderContext11.Device, qd);

                    RenderContext11.devContext.End(query);

                    bool result = false;
                    bool retVal = false;
                    while (!result && !retVal)
                    {
                        SharpDX.DataStream ds = RenderContext11.devContext.GetData(query);
#if !WINDOWS_UWP
                        result = ds.ReadBoolean();
#endif
                        ds.Close();
                        ds.Dispose();

                    }
                    query.Dispose();
                }
            }

        }
        public BlendState Fader = new BlendState(true, 2000);

        public VideoOutputType dumpFrameParams = new VideoOutputType();

        private bool crossFadeFrame = false;

        public bool CrossFadeFrame
        {
            set
            {
#if !WINDOWS_UWP
                if (value && crossFadeFrame != value)
                {
                    if (crossFadeTexture != null)
                    {
                        crossFadeTexture.Dispose();
                    }
                    crossFadeTexture = RenderContext11.GetScreenTexture();

                }
                crossFadeFrame = value;

                if (!value)
                {
                    if (crossFadeTexture != null)
                    {
                        crossFadeTexture.Dispose();
                        crossFadeTexture = null;
                    }
                }
#endif
            }
            get
            {
                return crossFadeFrame;
            }
        }

#if !WINDOWS_UWP
        PositionColoredTextured[] fadePoints = new PositionColoredTextured[4];




        private Texture11 crossFadeTexture = null;
        

        private void FadeFrame()
        {

            SettingParameter sp = Settings.Active.GetSetting(StockSkyOverlayTypes.FadeToBlack);



            if ((sp.Opacity > 0) && !(Settings.MasterController && Properties.Settings.Default.FadeRemoteOnly))
            {
                SysColor color = SysColor.FromArgb(255 - (int)UiTools.Gamma(255 - (int)(sp.Opacity * 255), 1 / 2.2f), SysColor.FromArgb(255, 0, 0, 0));

                if (!(sp.Opacity > 0))
                {
                    color = SysColor.FromArgb(255 - (int)UiTools.Gamma(255 - (int)(sp.Opacity * 255), 1 / 2.2f), SysColor.FromArgb(255, 0, 0, 0));
                }


                if (crossFadeFrame)
                {
                    color = SysColor.FromArgb((int)UiTools.Gamma((int)((sp.Opacity) * 255), 1 / 2.2f), SysColor.FromArgb(255, 255, 255, 255));
                }
                else
                {
                    if (crossFadeTexture != null)
                    {
                        crossFadeTexture.Dispose();
                        crossFadeTexture = null;
                    }
                }

                fadePoints[0].X = 0;
                fadePoints[0].Y = RenderContext11.Height;
                fadePoints[0].Z = 0;
                fadePoints[0].Tu = 0;
                fadePoints[0].Tv = 1;
                fadePoints[0].W = 1;
                fadePoints[0].Color = color;
                fadePoints[1].X = 0;
                fadePoints[1].Y = 0;
                fadePoints[1].Z = 0;
                fadePoints[1].Tu = 0;
                fadePoints[1].Tv = 0;
                fadePoints[1].W = 1;
                fadePoints[1].Color = color;
                fadePoints[2].X = RenderContext11.Width;
                fadePoints[2].Y = RenderContext11.Height;
                fadePoints[2].Z = 0;
                fadePoints[2].Tu = 1;
                fadePoints[2].Tv = 1;
                fadePoints[2].W = 1;
                fadePoints[2].Color = color;
                fadePoints[3].X = RenderContext11.Width;
                fadePoints[3].Y = 0;
                fadePoints[3].Z = 0;
                fadePoints[3].Tu = 1;
                fadePoints[3].Tv = 0;
                fadePoints[3].W = 1;
                fadePoints[3].Color = color;

                Sprite2d.DrawForScreen(RenderContext11, fadePoints, 4, crossFadeTexture, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);
            }
        }


        private void FadeDomeTexture()
        {

            SettingParameter sp = Settings.Active.GetSetting(StockSkyOverlayTypes.FadeToBlack);



            if ((sp.Opacity > 0) && !(Settings.MasterController && Properties.Settings.Default.FadeRemoteOnly))
            {
                SysColor color = SysColor.FromArgb((byte)(255 - (int)UiTools.Gamma(255 - (int)(sp.Opacity * 255), 1 / 2.2f)), 0, 0, 0);

                if (!(sp.Opacity > 0))
                {
                    color = SysColor.FromArgb((byte)(255 - (int)UiTools.Gamma(255 - (int)(sp.Opacity * 255), 1 / 2.2f)), 0, 0, 0);
                }


                if (crossFadeFrame)
                {
                    color = SysColor.FromArgb((byte)((int)UiTools.Gamma((int)((sp.Opacity) * 255), 1 / 2.2f)), 255, 255, 255);
                }
                else
                {
                    if (crossFadeTexture != null)
                    {
                        crossFadeTexture.Dispose();
                        crossFadeTexture = null;
                    }
                }

                fadePoints[0].X = 0;
                fadePoints[0].Y = dumpFrameParams.Height;
                fadePoints[0].Z = 0;
                fadePoints[0].Tu = 0;
                fadePoints[0].Tv = 1;
                fadePoints[0].W = 1;
                fadePoints[0].Color = color;
                fadePoints[1].X = 0;
                fadePoints[1].Y = 0;
                fadePoints[1].Z = 0;
                fadePoints[1].Tu = 0;
                fadePoints[1].Tv = 0;
                fadePoints[1].W = 1;
                fadePoints[1].Color = color;
                fadePoints[2].X = dumpFrameParams.Width;
                fadePoints[2].Y = dumpFrameParams.Height;
                fadePoints[2].Z = 0;
                fadePoints[2].Tu = 1;
                fadePoints[2].Tv = 1;
                fadePoints[2].W = 1;
                fadePoints[2].Color = color;
                fadePoints[3].X = dumpFrameParams.Width;
                fadePoints[3].Y = 0;
                fadePoints[3].Z = 0;
                fadePoints[3].Tu = 1;
                fadePoints[3].Tv = 0;
                fadePoints[3].W = 1;
                fadePoints[3].Color = color;

                Sprite2d.DrawForScreen(RenderContext11, fadePoints, 4, crossFadeTexture, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);
            }
        }

        TansformedPositionTexturedVertexBuffer11 ScreenVertexBuffer;

        void RenderSteroPairAnaglyph(RenderTargetTexture left, RenderTargetTexture right)
        {

            RenderContext11.SetDisplayRenderTargets();
            RenderContext11.ClearRenderTarget(SharpDX.Color.Black);


            if (ScreenVertexBuffer == null)
            {
                if (ScreenVertexBuffer != null)
                {
                    ScreenVertexBuffer.Dispose();
                    GC.SuppressFinalize(ScreenVertexBuffer);
                    ScreenVertexBuffer = null;
                }

                ScreenVertexBuffer = new TansformedPositionTexturedVertexBuffer11(6, RenderContext11.PrepDevice);

                //PreTransformed
                TansformedPositionTextured[] quad = (TansformedPositionTextured[])ScreenVertexBuffer.Lock(0, 0);


                quad[0].Position = new SharpDX.Vector4(-1, 1, .9f, 1);
                quad[0].Tu = 0;
                quad[0].Tv = 0;

                quad[1].Position = new SharpDX.Vector4(1, 1, .9f, 1);
                quad[1].Tu = 1;
                quad[1].Tv = 0;

                quad[2].Position = new SharpDX.Vector4(-1, -1, .9f, 1);
                quad[2].Tu = 0;
                quad[2].Tv = 1;

                quad[3].Position = new SharpDX.Vector4(-1, -1, .9f, 1);
                quad[3].Tu = 0;
                quad[3].Tv = 1;

                quad[4].Position = new SharpDX.Vector4(1, 1, .9f, 1);
                quad[4].Tu = 1;
                quad[4].Tv = 0;

                quad[5].Position = new SharpDX.Vector4(1, -1, .9f, 1);
                quad[5].Tu = 1;
                quad[5].Tv = 1;

                ScreenVertexBuffer.Unlock();

            }
            SysColor leftEyeColor = SysColor.FromArgb(255, 255, 0, 0);//red
            SysColor rightEyeColor = SysColor.FromArgb(255, 0, 255, 255);//cyan



            if (StereoMode == StereoModes.AnaglyphYellowBlue)
            {
                leftEyeColor = SysColor.FromArgb(255, 255, 255, 0);//yellow
                rightEyeColor = SysColor.FromArgb(255, 0, 0, 255);//blue

            }

            if (StereoMode == StereoModes.AnaglyphMagentaGreen)
            {
                leftEyeColor = SysColor.FromArgb(255, 255, 0, 255);//magenta
                rightEyeColor = SysColor.FromArgb(255, 0, 255, 0);//green

            }


            RenderContext11.SetVertexBuffer(ScreenVertexBuffer);

            RenderContext11.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            RenderContext11.BlendMode = BlendMode.Additive;

            RenderContext11.setRasterizerState(TriangleCullMode.Off);


            //Left Eye

            AnaglyphStereoShader.Color = new SharpDX.Color4(leftEyeColor.R / 255f, leftEyeColor.G / 255f, leftEyeColor.B / 255f, leftEyeColor.A / 255f);
            AnaglyphStereoShader.Use(RenderContext11.devContext);


            RenderContext11.devContext.PixelShader.SetShaderResource(0, leftEye.RenderTexture.ResourceView);


            RenderContext11.devContext.Draw(ScreenVertexBuffer.Count, 0);

            //Right Eye
            RenderContext11.devContext.PixelShader.SetShaderResource(0, rightEye.RenderTexture.ResourceView);
            AnaglyphStereoShader.Color = new SharpDX.Color4(rightEyeColor.R / 255f, rightEyeColor.G / 255f, rightEyeColor.B / 255f, rightEyeColor.A / 255f);
            AnaglyphStereoShader.Use(RenderContext11.devContext);
            RenderContext11.devContext.Draw(ScreenVertexBuffer.Count, 0);

            RenderContext11.BlendMode = BlendMode.Alpha;

            PresentFrame11(false);
        }

        void RenderSteroPairInterline(RenderTargetTexture left, RenderTargetTexture right)
        {
            if (RenderContext11.Height != RenderContext11.DisplayViewport.Height ||
                RenderContext11.Width != RenderContext11.DisplayViewport.Width)
            {
                RenderContext11.Resize(RenderContext11.Height, RenderContext11.Width);
            }

            RenderContext11.SetDisplayRenderTargets();
            RenderContext11.ClearRenderTarget(SharpDX.Color.Black);


            if (ScreenVertexBuffer == null)
            {
                if (ScreenVertexBuffer != null)
                {
                    ScreenVertexBuffer.Dispose();
                    GC.SuppressFinalize(ScreenVertexBuffer);
                    ScreenVertexBuffer = null;
                }

                ScreenVertexBuffer = new TansformedPositionTexturedVertexBuffer11(6, RenderContext11.PrepDevice);

                //PreTransformed
                TansformedPositionTextured[] quad = (TansformedPositionTextured[])ScreenVertexBuffer.Lock(0, 0);


                quad[0].Position = new SharpDX.Vector4(-1, 1, .9f, 1);
                quad[0].Tu = 0;
                quad[0].Tv = 0;

                quad[1].Position = new SharpDX.Vector4(1, 1, .9f, 1);
                quad[1].Tu = 1;
                quad[1].Tv = 0;

                quad[2].Position = new SharpDX.Vector4(-1, -1, .9f, 1);
                quad[2].Tu = 0;
                quad[2].Tv = 1;

                quad[3].Position = new SharpDX.Vector4(-1, -1, .9f, 1);
                quad[3].Tu = 0;
                quad[3].Tv = 1;

                quad[4].Position = new SharpDX.Vector4(1, 1, .9f, 1);
                quad[4].Tu = 1;
                quad[4].Tv = 0;

                quad[5].Position = new SharpDX.Vector4(1, -1, .9f, 1);
                quad[5].Tu = 1;
                quad[5].Tv = 1;

                ScreenVertexBuffer.Unlock();

            }


            RenderContext11.SetVertexBuffer(ScreenVertexBuffer);

            RenderContext11.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            RenderContext11.BlendMode = BlendMode.Additive;

            RenderContext11.setRasterizerState(TriangleCullMode.Off);


            InterlineStereoShader.Lines = RenderContext11.Height;
            InterlineStereoShader.Odd = StereoMode == StereoModes.InterlineOdd ? 1.0f : 0.0f;

            InterlineStereoShader.Use(RenderContext11.devContext);

            RenderContext11.devContext.PixelShader.SetShaderResource(0, right.RenderTexture.ResourceView);
            RenderContext11.devContext.PixelShader.SetShaderResource(1, left.RenderTexture.ResourceView);

            RenderContext11.devContext.Draw(ScreenVertexBuffer.Count, 0);

            RenderContext11.BlendMode = BlendMode.Alpha;

            PresentFrame11(false);
        }
        void RenderSteroPairSideBySide(RenderTargetTexture left, RenderTargetTexture right)
        {
            RenderContext11.SetDisplayRenderTargets();
            RenderContext11.ClearRenderTarget(SharpDX.Color.Black);


            if (ScreenVertexBuffer == null)
            {
                if (ScreenVertexBuffer != null)
                {
                    ScreenVertexBuffer.Dispose();
                    GC.SuppressFinalize(ScreenVertexBuffer);
                    ScreenVertexBuffer = null;
                }

                ScreenVertexBuffer = new TansformedPositionTexturedVertexBuffer11(6, RenderContext11.PrepDevice);

                //PreTransformed
                TansformedPositionTextured[] quad = (TansformedPositionTextured[])ScreenVertexBuffer.Lock(0, 0);


                quad[0].Position = new SharpDX.Vector4(-1, 1, .9f, 1);
                quad[0].Tu = 0;
                quad[0].Tv = 0;

                quad[1].Position = new SharpDX.Vector4(1, 1, .9f, 1);
                quad[1].Tu = 1;
                quad[1].Tv = 0;

                quad[2].Position = new SharpDX.Vector4(-1, -1, .9f, 1);
                quad[2].Tu = 0;
                quad[2].Tv = 1;

                quad[3].Position = new SharpDX.Vector4(-1, -1, .9f, 1);
                quad[3].Tu = 0;
                quad[3].Tv = 1;

                quad[4].Position = new SharpDX.Vector4(1, 1, .9f, 1);
                quad[4].Tu = 1;
                quad[4].Tv = 0;

                quad[5].Position = new SharpDX.Vector4(1, -1, .9f, 1);
                quad[5].Tu = 1;
                quad[5].Tv = 1;

                ScreenVertexBuffer.Unlock();

            }


            RenderContext11.SetVertexBuffer(ScreenVertexBuffer);

            RenderContext11.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            RenderContext11.BlendMode = BlendMode.Additive;

            RenderContext11.setRasterizerState(TriangleCullMode.Off);

            SideBySideStereoShader.Use(RenderContext11.devContext);


            RenderContext11.devContext.PixelShader.SetShaderResource(0, right.RenderTexture.ResourceView);
            RenderContext11.devContext.PixelShader.SetShaderResource(1, left.RenderTexture.ResourceView);


            RenderContext11.devContext.Draw(ScreenVertexBuffer.Count, 0);

            RenderContext11.BlendMode = BlendMode.Alpha;

            PresentFrame11(false);
        }


        void RenderTextureToScreen(SharpDX.Direct3D11.ShaderResourceView eye, int width, int height)
        {
            RenderContext11.SetDisplayRenderTargets();

            RenderContext11.ClearRenderTarget(SharpDX.Color.Black);


            if (ScreenVertexBuffer == null)
            {
                if (ScreenVertexBuffer != null)
                {
                    ScreenVertexBuffer.Dispose();
                    GC.SuppressFinalize(ScreenVertexBuffer);
                    ScreenVertexBuffer = null;
                }

                ScreenVertexBuffer = new TansformedPositionTexturedVertexBuffer11(6, RenderContext11.PrepDevice);

                //PreTransformed
                TansformedPositionTextured[] quad = (TansformedPositionTextured[])ScreenVertexBuffer.Lock(0, 0);


                quad[0].Position = new SharpDX.Vector4(-1, 1, .9f, 1);
                quad[0].Tu = 0;
                quad[0].Tv = 0;

                quad[1].Position = new SharpDX.Vector4(1, 1, .9f, 1);
                quad[1].Tu = 1;
                quad[1].Tv = 0;

                quad[2].Position = new SharpDX.Vector4(-1, -1, .9f, 1);
                quad[2].Tu = 0;
                quad[2].Tv = 1;

                quad[3].Position = new SharpDX.Vector4(-1, -1, .9f, 1);
                quad[3].Tu = 0;
                quad[3].Tv = 1;

                quad[4].Position = new SharpDX.Vector4(1, 1, .9f, 1);
                quad[4].Tu = 1;
                quad[4].Tv = 0;

                quad[5].Position = new SharpDX.Vector4(1, -1, .9f, 1);
                quad[5].Tu = 1;
                quad[5].Tv = 1;

                ScreenVertexBuffer.Unlock();

            }


            RenderContext11.SetVertexBuffer(ScreenVertexBuffer);

            RenderContext11.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            RenderContext11.BlendMode = BlendMode.None;

            RenderContext11.setRasterizerState(TriangleCullMode.Off);
            // RenderContext11.DepthStencilMode = DepthStencilMode.Off;



            RenderContext11.devContext.PixelShader.SetShaderResource(0, eye);

            SimpleShader.Use(RenderContext11.devContext);

            RenderContext11.devContext.Draw(ScreenVertexBuffer.Count, 0);


            RenderContext11.BlendMode = BlendMode.Alpha;


            PresentFrame11(false);
        }
#endif

        public void DrawClouds()
        {
#if !BASICWWT
            Texture11 cloudTexture = Planets.CloudTexture;
            if (cloudTexture != null)
            {
                RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, 1.0f, SysColor.FromArgb(255, 255, 255, 255));

                RenderContext11.MainTexture = cloudTexture;

                Matrix3d savedWorld = RenderContext11.World;
                double cloudScale = 1.0 + Planets.EarthCloudHeightMeters / 6378100.0;
                RenderContext11.World = Matrix3d.Scaling(cloudScale, cloudScale, cloudScale) * RenderContext11.World;

                RenderContext11.setRasterizerState(TriangleCullMode.CullCounterClockwise);
                RenderContext11.DepthStencilMode = DepthStencilMode.ZReadOnly;
                RenderContext11.BlendMode = BlendMode.Alpha;

                Planets.DrawFixedResolutionSphere(RenderContext11, 0);

                RenderContext11.World = savedWorld;
                RenderContext11.setRasterizerState(TriangleCullMode.CullClockwise);
                RenderContext11.DepthStencilMode = DepthStencilMode.ZReadWrite;
                RenderContext11.BlendMode = BlendMode.None;
            }
#endif
        }

        private void ClampZoomValues()
        {
            if (ZoomFactor > ZoomMax)
            {
                ZoomFactor = ZoomMax;
            }
            if (ZoomFactor < ZoomMin)
            {
                ZoomFactor = ZoomMin;
            }
            if (TargetZoom > ZoomMax)
            {
                TargetZoom = ZoomMax;
            }
            if (TargetZoom < ZoomMin)
            {
                TargetZoom = ZoomMin;
            }
        }

        public double zoomMax = 360;
        public double zoomMaxSolarSystem = 1E+16;

        public double ZoomMax
        {
            get
            {
                if (currentImageSetfield.DataSetType == ImageSetType.SolarSystem)
                {
                    return zoomMaxSolarSystem;
                }
                else
                {
                    return zoomMax;
                }
            }
        }


        public double ZoomMin
        {
            get
            {
                if (currentImageSetfield.DataSetType == ImageSetType.SolarSystem)
                {
                    return (zoomMinSolarSystem / 10000000000) * Settings.Active.SolarSystemScale;
                }
                else
                {
                    if (currentImageSetfield.IsMandelbrot)
                    {

                        return 0.00000000000000000000000000000001;
                    }
                    return zoomMin / 64;
                }
            }
            set { zoomMin = value; }
        }

        Object3d LeftHandControllerModel;
        Object3d RightHandControllerModel;

        public HandController LeftController { get; set; } = new HandController(HandType.Left);
        public HandController RightController { get; set; } = new HandController(HandType.Right);
        public static Matrix3d ExternalProjectionLeft { get; set; }
        public static Matrix3d ExternalProjectionRight { get; set; }

        PointerRay leftPointerRay = new PointerRay();
        PointerRay rightPointerRay = new PointerRay();

        public double zoomMin = 0.001373291015625;
        public double zoomMinSolarSystem = 1E-06;

        public double lastFrameTime = .1;
        public Int64 lastRenderTickCount = 0;

        public bool CaptureVideo = false;
        public bool ScreenShot = false;
#if !WINDOWS_UWP
        public VideoOut videoOut = null;
        Bitmap bmpVideoOut = null;
#endif

        public void PaintLayerFull11(IImageSet layer, float opacityPercentage)
        {
            float opacity = opacityPercentage / 100.0f;
            RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, opacity, SysColor.FromArgb(255, 255, 255, 255));
            DrawTiledSphere(layer, opacity, SysColor.FromArgb(255, 255, 255, 255));
        }

        public void PaintLayerFullTint11(IImageSet layer, float opacityPercentage, SysColor color)
        {
            float opacity = opacityPercentage / 100.0f;
            RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, opacity, color);
            DrawTiledSphere(layer, opacity, color);
        }

        public float brightness = .5f;
        public float contrast = .5f;

        public void DrawTiledSphere(IImageSet layer, float opacity, SysColor color)
        {
            int maxX = GetTilesXForLevel(layer, layer.BaseLevel);
            int maxY = GetTilesYForLevel(layer, layer.BaseLevel);

            // Set up the input assembler; match the layout of the current shader
            RenderContext11.Device.ImmediateContext.InputAssembler.InputLayout = RenderContext11.Shader.inputLayout(PlanetShader11.StandardVertexLayout.PositionNormalTex2);
            RenderContext11.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            Tile.Viewport = RenderContext11.ViewPort;

            if (RenderContext11.ExternalProjection)
            {
                Tile.wvp = (RenderContext11.WorldBase * RenderContext11.ViewBase * RenderContext11.ExternalProjectionLeft).Matrix11; 
            }
            else
            {
                Tile.wvp = (RenderContext11.WorldBase * RenderContext11.ViewBase * RenderContext11.Projection).Matrix11;
            }
            RenderContext11.PreDraw();
#if !WINDOWS_UWP
            if (layer.DataSetType == ImageSetType.Sky)
            {
                HDRPixelShader.constants.a = brightness;
                HDRPixelShader.constants.b = contrast;
                HDRPixelShader.constants.opacity = opacity;
                HDRPixelShader.constants.tint = new SharpDX.Color4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);

                HDRPixelShader.Use(RenderContext11.devContext);
            }
#endif
            if (Properties.Settings.Default.EarthCutawayView.State && !SolarSystemMode && !Space && layer.DataSetType == ImageSetType.Earth)
            {

                RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, opacity, SysColor.FromArgb(255, 255, 255, 255));
                if (layer.Projection == ProjectionType.Toast)
                {

                    Tile tile = TileCache.GetTile(layer.BaseLevel + 1, 1, 0, layer, null);
                    if (tile != null && tile.IsTileInFrustum(RenderContext11.Frustum))
                    {
                        tile.Draw3D(RenderContext11, opacity, null);
                    }
                    tile = TileCache.GetTile(layer.BaseLevel + 1, 1, 1, layer, null);
                    if (tile != null && tile.IsTileInFrustum(RenderContext11.Frustum))
                    {
                        tile.Draw3D(RenderContext11, opacity, null);
                    }
                    tile = TileCache.GetTile(layer.BaseLevel + 1, 0, 0, layer, null);
                    if (tile != null && tile.IsTileInFrustum(RenderContext11.Frustum))
                    {
                        tile.Draw3D(RenderContext11, opacity, null);
                    }
                    //Show if partially transparent
                    if (Properties.Settings.Default.EarthCutawayView.Opacity != 1.0)
                    {
                        tile = TileCache.GetTile(layer.BaseLevel + 1, 0, 1, layer, null);

                        if (tile != null && (RenderContext11.ExternalProjection || tile.IsTileInFrustum(RenderContext11.Frustum)))
                        {
                            RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, 1.0f - Properties.Settings.Default.EarthCutawayView.Opacity, SysColor.FromArgb(255, 255, 255, 255));
                            tile.Draw3D(RenderContext11, opacity, null);
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < maxX; x++)
                    {
                        for (int y = 0; y < maxY; y++)
                        {

                            if (!(x == 1))
                            {
                                Tile tile = TileCache.GetTile(layer.BaseLevel, x, y, layer, null);
                                if (tile != null && tile.IsTileInFrustum(RenderContext11.Frustum))
                                {
                                    tile.Draw3D(RenderContext11, opacity, null);
                                }
                            }
                            else
                            {
                                Tile tile = TileCache.GetTile(layer.BaseLevel + 1, x * 2 + 1, y * 2, layer, null);
                                if (tile != null && tile.IsTileInFrustum(RenderContext11.Frustum))
                                {
                                    tile.Draw3D(RenderContext11, opacity, null);
                                }
                                tile = TileCache.GetTile(layer.BaseLevel + 1, x * 2 + 1, y * 2 + 1, layer, null);
                                if (tile != null && tile.IsTileInFrustum(RenderContext11.Frustum))
                                {
                                    tile.Draw3D(RenderContext11, opacity, null);
                                }
                                if (Properties.Settings.Default.EarthCutawayView.Opacity != 1.0 && !Space)
                                {
                                    RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, 1.0f - Properties.Settings.Default.EarthCutawayView.Opacity, SysColor.FromArgb(255, 255, 255, 255));

                                    tile = TileCache.GetTile(layer.BaseLevel + 1, x * 2, y * 2, layer, null);
                                    if (tile != null && tile.IsTileInFrustum(RenderContext11.Frustum))
                                    {
                                        tile.Draw3D(RenderContext11, 1, null);
                                    }
                                    tile = TileCache.GetTile(layer.BaseLevel + 1, x * 2, y * 2 + 1, layer, null);
                                    if (tile != null && tile.IsTileInFrustum(RenderContext11.Frustum))
                                    {
                                        tile.Draw3D(RenderContext11, 1, null);
                                    }
                                    RenderContext11.SetupBasicEffect(BasicEffect.TextureColorOpacity, 1.0f, SysColor.FromArgb(255, 255, 255, 255));

                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < maxX; x++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        Tile tile = TileCache.GetTile(layer.BaseLevel, x, y, layer, null);
                        if (tile != null && (RenderContext11.ExternalProjection || tile.IsTileInFrustum(RenderContext11.Frustum)))
                        {
                            tile.Draw3D(RenderContext11, opacity, null);
                        }
                    }
                }
            }

            RenderContext11.DisableEffect();

            RenderContext11.LocalCenter = Vector3d.Empty;
        }


        internal static int GetTilesYForLevel(IImageSet layer, int level)
        {
            int maxY;

            switch (layer.Projection)
            {
                case ProjectionType.Mercator:
                    maxY = (int)Math.Pow(2, level);
                    break;
                case ProjectionType.Equirectangular:
                    maxY = (int)(Math.Pow(2, level) * (180 / layer.BaseTileDegrees) + .9);
                    break;
                case ProjectionType.Tangent:
                    maxY = (int)Math.Pow(2, level);

                    break;
                case ProjectionType.SkyImage:
                case ProjectionType.Spherical:
                    maxY = 1;
                    break;
                default:
                    maxY = (int)Math.Pow(2, level);
                    break;
            }
            return maxY;
        }

        internal static int GetTilesXForLevel(IImageSet layer, int level)
        {
            int maxX;
            switch (layer.Projection)
            {
                case ProjectionType.Plotted:
                case ProjectionType.Toast:
                    maxX = (int)Math.Pow(2, level);
                    break;
                case ProjectionType.Mercator:
                    maxX = (int)Math.Pow(2, level) * (int)(layer.BaseTileDegrees / 360.0);
                    break;
                case ProjectionType.Equirectangular:
                    maxX = (int)(Math.Pow(2, level) * (layer.BaseTileDegrees / 90.0));
                    maxX = (int)(Math.Pow(2, level) * (360 / layer.BaseTileDegrees) + .9);

                    break;

                case ProjectionType.Tangent:
                    if (layer.WidthFactor == 1)
                    {
                        maxX = (int)Math.Pow(2, level) * 2;
                    }
                    else
                    {
                        maxX = (int)Math.Pow(2, level);
                    }
                    break;
                case ProjectionType.SkyImage:
                    maxX = 1;
                    break;
                case ProjectionType.Spherical:
                    maxX = 1;
                    break;
                default:
                    maxX = (int)Math.Pow(2, level) * 2;
                    break;
            }


            return maxX;
        }

        public static bool renderingVideo = false;

        //End Main rendering Section

        private void LoadCurrentFigures()
        {
            constellationsFigures = new Constellations("Default Figures", "http://www.worldwidetelescope.org/data/figures.txt", false, false);
        }

#if !WINDOWS_UWP
        // Warp and Blend code
        PositionColorTexturedVertexBuffer11 distortVertexBuffer;
        IndexBuffer11 distortIndexBuffer;
        int distortVertexCount = 0;
        int distortTriangleCount = 0;

        public bool refreshWarp = true;
        private void RenderDistort()
        {
            SetupMatricesDistort();

            if (distortVertexBuffer == null || refreshWarp)
            {
                MakeDistortionGrid();
                refreshWarp = false;

            }

            RenderContext11.SetDisplayRenderTargets();
            RenderContext11.ClearRenderTarget(SharpDX.Color.Black);

            RenderContext11.SetVertexBuffer(distortVertexBuffer);
            RenderContext11.SetIndexBuffer(distortIndexBuffer);

            RenderContext11.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            RenderContext11.BlendMode = BlendMode.Alpha;
            RenderContext11.setRasterizerState(TriangleCullMode.Off);

            SharpDX.Matrix mat = (RenderContext11.World * RenderContext11.View * RenderContext11.Projection).Matrix11;
            mat.Transpose();

            WarpOutputShader.MatWVP = mat;

            WarpOutputShader.Use(RenderContext11.devContext, true);

            RenderContext11.devContext.PixelShader.SetShaderResource(0, undistorted.RenderTexture.ResourceView);
            RenderContext11.devContext.DrawIndexed(distortIndexBuffer.Count, 0, 0);
            PresentFrame11(false);

        }
        private void RenderDistortWithBlend()
        {
            SetupMatricesDistort();

            if (blendTexture == null)
            {
                blendTexture = Texture11.FromFile(config.BlendFile);
            }

            if (distortVertexBuffer == null || refreshWarp)
            {
                MakeDistortionGridSgcWithBlend3();
                refreshWarp = false;
            }

            RenderContext11.SetDisplayRenderTargets();
            RenderContext11.ClearRenderTarget(SharpDX.Color.Red);

            RenderContext11.SetVertexBuffer(distortVertexBuffer);
            RenderContext11.SetIndexBuffer(distortIndexBuffer);

            RenderContext11.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            RenderContext11.BlendMode = BlendMode.Alpha;
            RenderContext11.setRasterizerState(TriangleCullMode.Off);

            SharpDX.Matrix mat = (RenderContext11.World * RenderContext11.View * RenderContext11.Projection).Matrix11;
            mat.Transpose();

            WarpOutputShaderWithBlendTexture.MatWVP = mat;
            WarpOutputShaderWithBlendTexture.Use(RenderContext11.devContext, true);

            RenderContext11.devContext.PixelShader.SetShaderResource(0, undistorted.RenderTexture.ResourceView);
            RenderContext11.devContext.PixelShader.SetShaderResource(1, blendTexture.ResourceView);
            RenderContext11.devContext.DrawIndexed(distortIndexBuffer.Count, 0, 0);
            RenderContext11.devContext.PixelShader.SetShaderResource(1, null);
            PresentFrame11(false);
        }



        private void MakeDistortionGrid()
        { 
            Bitmap bmpBlend = new Bitmap(config.BlendFile);
            FastBitmap fastBlend = new FastBitmap(bmpBlend);
            Bitmap bmpDistort = new Bitmap(config.DistortionGrid);
            FastBitmap fastDistort = new FastBitmap(bmpDistort);


            fastBlend.LockBitmapRgb();
            fastDistort.LockBitmapRgb();
            int subX = bmpBlend.Width - 1;
            int subY = subX;

            if (distortIndexBuffer != null)
            {
                distortIndexBuffer.Dispose();
                GC.SuppressFinalize(distortIndexBuffer);
            }

            if (distortVertexBuffer != null)
            {
                distortVertexBuffer.Dispose();
                GC.SuppressFinalize(distortVertexBuffer);
            }


            distortIndexBuffer = new IndexBuffer11(typeof(int), (subX * subY * 6), RenderContext11.PrepDevice);
            distortVertexBuffer = new PositionColorTexturedVertexBuffer11(((subX + 1) * (subY + 1)), RenderContext11.PrepDevice);

            distortVertexCount = (subX + 1) * (subY + 1);


            int index = 0;


            // Create a vertex buffer 
            PositionColoredTextured[] verts = (PositionColoredTextured[])distortVertexBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)
            int x1, y1;

            unsafe
            {
                double maxU = 0;
                double maxV = 0;
                double textureStepX = 1.0f / subX;
                double textureStepY = 1.0f / subY;
                for (y1 = 0; y1 <= subY; y1++)
                {
                    double tv;
                    for (x1 = 0; x1 <= subX; x1++)
                    {
                        double tu;


                        index = y1 * (subX + 1) + x1;
                        PixelDataRgb* pdata = fastDistort.GetRgbPixel(x1, y1);

                        tu = (float)(pdata->blue + ((uint)pdata->red % 16) * 256) / 4095f;
                        tv = (float)(pdata->green + ((uint)pdata->red / 16) * 256) / 4095f;

                        //tu = (tu - .5f) * 1.7777778 + .5f;

                        if (tu > maxU)
                        {
                            maxU = tu;
                        }
                        if (tv > maxV)
                        {
                            maxV = tv;
                        }

                        verts[index].Position = new SharpDX.Vector4(((float)x1 / subX) - .5f, (1f - ((float)y1 / subY)) - .5f, .9f, 1f);
                        verts[index].Tu = (float)tu;
                        verts[index].Tv = (float)tv;
                        PixelDataRgb* pPixel = fastBlend.GetRgbPixel(x1, y1);

                        verts[index].Color = SysColor.FromArgb(255, pPixel->red, pPixel->green, pPixel->blue);

                    }
                }
                distortVertexBuffer.Unlock();
                distortTriangleCount = (subX) * (subY) * 2;
                uint[] indexArray = (uint[])distortIndexBuffer.Lock();
                index = 0;
                for (y1 = 0; y1 < subY; y1++)
                {
                    for (x1 = 0; x1 < subX; x1++)
                    {
                        // First triangle in quad
                        indexArray[index] = (uint)(y1 * (subX + 1) + x1);
                        indexArray[index + 1] = (uint)((y1 + 1) * (subX + 1) + x1);
                        indexArray[index + 2] = (uint)(y1 * (subX + 1) + (x1 + 1));

                        // Second triangle in quad
                        indexArray[index + 3] = (uint)(y1 * (subX + 1) + (x1 + 1));
                        indexArray[index + 4] = (uint)((y1 + 1) * (subX + 1) + x1);
                        indexArray[index + 5] = (uint)((y1 + 1) * (subX + 1) + (x1 + 1));
                        index += 6;
                    }
                }
                this.distortIndexBuffer.Unlock();
            }
            fastDistort.UnlockBitmap();
            fastBlend.UnlockBitmap();
            fastDistort.Dispose();
            GC.SuppressFinalize(fastDistort);
            fastBlend.Dispose();
            GC.SuppressFinalize(fastBlend);
        }

        private void MakeDistortionGridSgc()
        {
            int subX = config.DistortionGridWidth - 1;
            int subY = config.DistortionGridHeight - 1;

            if (distortIndexBuffer != null)
            {
                distortIndexBuffer.Dispose();
                GC.SuppressFinalize(distortIndexBuffer);
            }

            if (distortVertexBuffer != null)
            {
                distortVertexBuffer.Dispose();
                GC.SuppressFinalize(distortVertexBuffer);
            }


            distortIndexBuffer = new IndexBuffer11(typeof(int), (subX * subY * 6), RenderContext11.PrepDevice);
            distortVertexBuffer = new PositionColorTexturedVertexBuffer11(((subX + 1) * (subY + 1)), RenderContext11.PrepDevice);

            distortVertexCount = (subX + 1) * (subY + 1);


            int index = 0;


            // Create a vertex buffer 
            PositionColoredTextured[] verts = (PositionColoredTextured[])distortVertexBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)
            int x1, y1;

            unsafe
            {
                double maxU = 0;
                double maxV = 0;
                double textureStepX = 1.0f / subX;
                double textureStepY = 1.0f / subY;
                for (y1 = 0; y1 <= subY; y1++)
                {
                    double tv;
                    for (x1 = 0; x1 <= subX; x1++)
                    {
                        double tu;


                        index = y1 * (subX + 1) + x1;
                        Vector6 vec = config.DistortionGridVertices[x1, y1];
                        tu = vec.T;
                        tv = 1 - vec.U;

                        //tu = (tu - .5f) * 1.7777778 + .5f;

                        if (tu > maxU)
                        {
                            maxU = tu;
                        }
                        if (tv > maxV)
                        {
                            maxV = tv;
                        }
                        float xx = ((float)x1 / subX) - .5f;
                        float yy = (((float)y1 / subY)) - .5f;

                        float difX = xx - vec.X;
                        float difY = vec.Y + yy;

                        verts[index].Position = new SharpDX.Vector4(xx, yy, .9f, 1f);
                        verts[index].Tu = (float)tu;
                        verts[index].Tv = (float)tv;


                        verts[index].Color = SysColor.FromArgb(255, 255, 255, 255);

                    }
                }
                distortVertexBuffer.Unlock();
                distortTriangleCount = (subX) * (subY) * 2;
                uint[] indexArray = (uint[])distortIndexBuffer.Lock();
                index = 0;
                for (y1 = 0; y1 < subY; y1++)
                {
                    for (x1 = 0; x1 < subX; x1++)
                    {
                        // First triangle in quad
                        indexArray[index] = (uint)(y1 * (subX + 1) + x1);
                        indexArray[index + 1] = (uint)((y1 + 1) * (subX + 1) + x1);
                        indexArray[index + 2] = (uint)(y1 * (subX + 1) + (x1 + 1));

                        // Second triangle in quad
                        indexArray[index + 3] = (uint)(y1 * (subX + 1) + (x1 + 1));
                        indexArray[index + 4] = (uint)((y1 + 1) * (subX + 1) + x1);
                        indexArray[index + 5] = (uint)((y1 + 1) * (subX + 1) + (x1 + 1));
                        index += 6;
                    }
                }
                this.distortIndexBuffer.Unlock();
            }


        }
        private void MakeDistortionGridSgcWithBlend()
        {

            Bitmap bmpBlend = new Bitmap(config.BlendFile);
            FastBitmap fastBlend = new FastBitmap(bmpBlend);


            fastBlend.LockBitmapRgb();

            int subX = bmpBlend.Width - 1;
            int subY = bmpBlend.Height - 1;

            if (distortIndexBuffer != null)
            {
                distortIndexBuffer.Dispose();
                GC.SuppressFinalize(distortIndexBuffer);
            }

            if (distortVertexBuffer != null)
            {
                distortVertexBuffer.Dispose();
                GC.SuppressFinalize(distortVertexBuffer);
            }

            GridSampler gridSampler = new GridSampler(bmpBlend.Width, bmpBlend.Height, config.DistortionGridWidth, config.DistortionGridHeight, config.DistortionGridVertices);

            distortIndexBuffer = new IndexBuffer11(typeof(int), (subX * subY * 6), RenderContext11.PrepDevice);
            distortVertexBuffer = new PositionColorTexturedVertexBuffer11(((subX + 1) * (subY + 1)), RenderContext11.PrepDevice);

            distortVertexCount = (subX + 1) * (subY + 1);


            int index = 0;


            // Create a vertex buffer 
            PositionColoredTextured[] verts = (PositionColoredTextured[])distortVertexBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)
            int x1, y1;

            unsafe
            {
                double maxU = 0;
                double maxV = 0;
                double textureStepX = 1.0f / subX;
                double textureStepY = 1.0f / subY;
                for (y1 = 0; y1 <= subY; y1++)
                {
                    double tv;
                    for (x1 = 0; x1 <= subX; x1++)
                    {
                        double tu;


                        index = y1 * (subX + 1) + x1;

                        SharpDX.Vector2 sample = gridSampler.Sample(x1, y1);

                        tu = sample.X;
                        tv = 1 - sample.Y;

                        if (tu > maxU)
                        {
                            maxU = tu;
                        }
                        if (tv > maxV)
                        {
                            maxV = tv;
                        }

                        verts[index].Position = new SharpDX.Vector4(((float)x1 / subX) - .5f, (((float)y1 / subY)) - .5f, .9f, 1f);
                        verts[index].Tu = (float)tu;
                        verts[index].Tv = (float)tv;
                        PixelDataRgb* pPixel = fastBlend.GetRgbPixel(x1, subY - y1);

                        verts[index].Color = SysColor.FromArgb(255, pPixel->red, pPixel->green, pPixel->blue);

                    }
                }
                distortVertexBuffer.Unlock();
                distortTriangleCount = (subX) * (subY) * 2;
                uint[] indexArray = (uint[])distortIndexBuffer.Lock();
                index = 0;
                for (y1 = 0; y1 < subY; y1++)
                {
                    for (x1 = 0; x1 < subX; x1++)
                    {
                        // First triangle in quad
                        indexArray[index] = (uint)(y1 * (subX + 1) + x1);
                        indexArray[index + 1] = (uint)((y1 + 1) * (subX + 1) + x1);
                        indexArray[index + 2] = (uint)(y1 * (subX + 1) + (x1 + 1));

                        // Second triangle in quad
                        indexArray[index + 3] = (uint)(y1 * (subX + 1) + (x1 + 1));
                        indexArray[index + 4] = (uint)((y1 + 1) * (subX + 1) + x1);
                        indexArray[index + 5] = (uint)((y1 + 1) * (subX + 1) + (x1 + 1));
                        index += 6;
                    }
                }
                this.distortIndexBuffer.Unlock();
            }

            fastBlend.UnlockBitmap();
            fastBlend.Dispose();
            GC.SuppressFinalize(fastBlend);
        }


        private void MakeDistortionGridSgcWithBlend2()
        {

            Bitmap bmpBlend = new Bitmap(config.BlendFile);
            FastBitmap fastBlend = new FastBitmap(bmpBlend);


            fastBlend.LockBitmapRgb();

            int subX = bmpBlend.Width - 1;
            int subY = bmpBlend.Height - 1;

            if (distortIndexBuffer != null)
            {
                distortIndexBuffer.Dispose();
                GC.SuppressFinalize(distortIndexBuffer);
            }

            if (distortVertexBuffer != null)
            {
                distortVertexBuffer.Dispose();
                GC.SuppressFinalize(distortVertexBuffer);
            }

            GridSampler gridSampler = new GridSampler(bmpBlend.Width, bmpBlend.Height, config.DistortionGridWidth, config.DistortionGridHeight, config.DistortionGridVertices);

            distortIndexBuffer = new IndexBuffer11(typeof(int), (subX * subY * 6), RenderContext11.PrepDevice);
            distortVertexBuffer = new PositionColorTexturedVertexBuffer11(((subX + 1) * (subY + 1)), RenderContext11.PrepDevice);

            distortVertexCount = (subX + 1) * (subY + 1);


            int index = 0;


            // Create a vertex buffer 
            PositionColoredTextured[] verts = (PositionColoredTextured[])distortVertexBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)
            int x1, y1;

            unsafe
            {
                double maxU = 0;
                double maxV = 0;
                double textureStepX = 1.0f / subX;
                double textureStepY = 1.0f / subY;
                for (y1 = 0; y1 <= subY; y1++)
                {
                    double tv;
                    for (x1 = 0; x1 <= subX; x1++)
                    {
                        double tu;


                        index = y1 * (subX + 1) + x1;

                        SharpDX.Vector2 sample = gridSampler.Sample(x1, y1);

                        tu = sample.X;
                        tv = sample.Y;

                        if (tu > maxU)
                        {
                            maxU = tu;
                        }
                        if (tv > maxV)
                        {
                            maxV = tv;
                        }

                        verts[index].Position = new SharpDX.Vector4(((float)x1 / subX) - .5f, (1f - ((float)y1 / subY)) - .5f, .9f, 1f);
                        verts[index].Tu = (float)tu;
                        verts[index].Tv = (float)tv;
                        PixelDataRgb* pPixel = fastBlend.GetRgbPixel(x1, y1);

                        verts[index].Color = SysColor.FromArgb(255, pPixel->red, pPixel->green, pPixel->blue);

                    }
                }
                distortVertexBuffer.Unlock();
                distortTriangleCount = (subX) * (subY) * 2;
                uint[] indexArray = (uint[])distortIndexBuffer.Lock();
                index = 0;
                for (y1 = 0; y1 < subY; y1++)
                {
                    for (x1 = 0; x1 < subX; x1++)
                    {
                        // First triangle in quad
                        indexArray[index] = (uint)(y1 * (subX + 1) + x1);
                        indexArray[index + 1] = (uint)((y1 + 1) * (subX + 1) + x1);
                        indexArray[index + 2] = (uint)(y1 * (subX + 1) + (x1 + 1));

                        // Second triangle in quad
                        indexArray[index + 3] = (uint)(y1 * (subX + 1) + (x1 + 1));
                        indexArray[index + 4] = (uint)((y1 + 1) * (subX + 1) + x1);
                        indexArray[index + 5] = (uint)((y1 + 1) * (subX + 1) + (x1 + 1));
                        index += 6;
                    }
                }
                this.distortIndexBuffer.Unlock();
            }

            fastBlend.UnlockBitmap();
            fastBlend.Dispose();
            GC.SuppressFinalize(fastBlend);
        }

      

        private void MakeDistortionGridSgcWithBlend3()
        {
            if (distortIndexBuffer != null)
            {
                distortIndexBuffer.Dispose();
                GC.SuppressFinalize(distortIndexBuffer);
            }

            if (distortVertexBuffer != null)
            {
                distortVertexBuffer.Dispose();
                GC.SuppressFinalize(distortVertexBuffer);
            }

            int subX = config.DistortionGridWidth - 1;
            int subY = config.DistortionGridHeight - 1;
            distortIndexBuffer = new IndexBuffer11(typeof(int), (subX * subY * 6), RenderContext11.PrepDevice);
            distortVertexBuffer = new PositionColorTexturedVertexBuffer11(((subX + 1) * (subY + 1)), RenderContext11.PrepDevice);

            distortVertexCount = config.DistortionGridVertices.Length;
            // Create a vertex buffer 
            PositionColoredTextured[] verts = (PositionColoredTextured[])distortVertexBuffer.Lock(0, 0); // Lock the buffer (which will return our structs
                                                                                                         // unsafe
            {
                int index = 0;
                for (int y1 = 0; y1 < config.DistortionGridHeight; y1++)
                {
                    for (int x1 = 0; x1 < config.DistortionGridWidth; x1++)
                    {
                        index = y1 * config.DistortionGridWidth + x1;
                        verts[index].X = 1.0f * config.DistortionGridVertices[x1, y1].X - .5f;
                        verts[index].Y = -1.0f * config.DistortionGridVertices[x1, y1].Y + .5f;
                        verts[index].Z = 1f;
                        verts[index].W = 1f;
                        verts[index].Tu = config.DistortionGridVertices[x1, y1].T;
                        verts[index].Tv = 1f - config.DistortionGridVertices[x1, y1].U;
                        verts[index].Color = SysColor.White;
                    }
                }
                distortVertexBuffer.Unlock();

                distortTriangleCount = (subX) * (subY) * 2;
                uint[] indexArray = (uint[])distortIndexBuffer.Lock();
                index = 0;
                for (int y1 = 0; y1 < subY; y1++)
                {
                    for (int x1 = 0; x1 < subX; x1++)
                    {
                        // First triangle in quad
                        indexArray[index] = (uint)(y1 * (subX + 1) + x1);
                        indexArray[index + 1] = (uint)((y1 + 1) * (subX + 1) + x1);
                        indexArray[index + 2] = (uint)(y1 * (subX + 1) + (x1 + 1));

                        // Second triangle in quad
                        indexArray[index + 3] = (uint)(y1 * (subX + 1) + (x1 + 1));
                        indexArray[index + 4] = (uint)((y1 + 1) * (subX + 1) + x1);
                        indexArray[index + 5] = (uint)((y1 + 1) * (subX + 1) + (x1 + 1));
                        index += 6;
                    }
                }
                this.distortIndexBuffer.Unlock();
            }
        }

        PositionColorTexturedVertexBuffer11[] domeVertexBuffer;
        IndexBuffer11[] domeIndexBuffer;
        int domeVertexCount;

        int domeTriangleCount;


        void CreateDomeFaceVertexBuffer(int face)
        {
            int domeSubX = 50;
            int domeSubY = 50;

            CleanupDomeVertexBuffer(face);

            double fea = Math.Min(250, Properties.Settings.Default.FisheyeAngle) / 180;
            double fa = Math.Min(250, Properties.Settings.Default.FisheyeAngle);

            domeIndexBuffer[face] = new IndexBuffer11(typeof(short), (domeSubX * domeSubY * 6), RenderContext11.PrepDevice);
            domeVertexBuffer[face] = new PositionColorTexturedVertexBuffer11(((domeSubX + 1) * (domeSubY + 1)), RenderContext11.PrepDevice);

            domeVertexCount = domeSubX * domeSubY * 6;


            int index = 0;

            PositionColorTexturedVertexBuffer11 vb = domeVertexBuffer[face];

            PositionColoredTextured[] verts = (PositionColoredTextured[])vb.Lock(0, 0);
            int x1, y1;



            Vector3d topLeft = new Vector3d();
            Vector3d topRight = new Vector3d();
            Vector3d bottomLeft = new Vector3d();
            Vector3d bottomRight = new Vector3d();

            RenderTypes faceType = (RenderTypes)face;

            switch (faceType)
            {
                case RenderTypes.DomeFront:
                    topLeft = new Vector3d(-1, 1, 1);
                    topRight = new Vector3d(1, 1, 1);
                    bottomLeft = new Vector3d(-1, -1, 1);
                    bottomRight = new Vector3d(1, -1, 1);
                    break;
                case RenderTypes.DomeRight:
                    topLeft = new Vector3d(1, 1, 1);
                    topRight = new Vector3d(1, 1, -1);
                    bottomLeft = new Vector3d(1, -1, 1);
                    bottomRight = new Vector3d(1, -1, -1);
                    break;
                case RenderTypes.DomeUp:
                    topLeft = new Vector3d(-1, 1, -1);
                    topRight = new Vector3d(1, 1, -1);
                    bottomLeft = new Vector3d(-1, 1, 1);
                    bottomRight = new Vector3d(1, 1, 1);
                    break;
                case RenderTypes.DomeLeft:
                    topLeft = new Vector3d(-1, 1, -1);
                    topRight = new Vector3d(-1, 1, 1);
                    bottomLeft = new Vector3d(-1, -1, -1);
                    bottomRight = new Vector3d(-1, -1, 1);
                    break;
                case RenderTypes.DomeBack:
                    topLeft = new Vector3d(1, 1, -1);
                    topRight = new Vector3d(-1, 1, -1);
                    bottomLeft = new Vector3d(1, -1, -1);
                    bottomRight = new Vector3d(-1, -1, -1);
                    break;
            }

            double textureStepX = 1.0f / domeSubX;
            double textureStepY = 1.0f / domeSubY;
            for (y1 = 0; y1 <= domeSubY; y1++)
            {
                double tv;
                if (y1 != domeSubY)
                {
                    tv = textureStepY * y1;
                }
                else
                {
                    tv = 1;
                }

                for (x1 = 0; x1 <= domeSubX; x1++)
                {
                    double tu;
                    if (x1 != domeSubX)
                    {
                        tu = textureStepX * x1;
                    }
                    else
                    {
                        tu = 1;
                    }

                    Vector3d top = Vector3d.Lerp(topLeft, topRight, tu);
                    Vector3d bottom = Vector3d.Lerp(bottomLeft, bottomRight, tu);
                    Vector3d net = Vector3d.Lerp(top, bottom, tv);
                    net.Normalize();
                    Coordinates netNet = Coordinates.CartesianToSpherical2(net.Vector3);
                    double dist = (180 - (netNet.Lat + 90)) / (180 * fea);
                    dist = Math.Min(.5, dist);

                    double x = Math.Sin((netNet.Lng + 90) / 180 * Math.PI) * dist;
                    double y = Math.Cos((netNet.Lng + 90) / 180 * Math.PI) * dist;

                    index = y1 * (domeSubX + 1) + x1;
                    verts[index].Position = new SharpDX.Vector4((float)x, (float)y, .9f, 1);
                    verts[index].Tu = (float)tu;
                    verts[index].Tv = (float)tv;
                    verts[index].Color = SysColor.FromArgb(255, 255, 255, 255);
                }
            }
            vb.Unlock();
            domeTriangleCount = (domeSubX) * (domeSubY) * 2;
            short[] indexArray = (short[])domeIndexBuffer[face].Lock();
            index = 0;
            for (y1 = 0; y1 < domeSubY; y1++)
            {
                for (x1 = 0; x1 < domeSubX; x1++)
                {
                    //index = (y1 * domeSubX * 6) + 6 * x1;
                    // First triangle in quad
                    indexArray[index] = (short)(y1 * (domeSubX + 1) + x1);
                    indexArray[index + 1] = (short)((y1 + 1) * (domeSubX + 1) + x1);
                    indexArray[index + 2] = (short)(y1 * (domeSubX + 1) + (x1 + 1));

                    // Second triangle in quad
                    indexArray[index + 3] = (short)(y1 * (domeSubX + 1) + (x1 + 1));
                    indexArray[index + 4] = (short)((y1 + 1) * (domeSubX + 1) + x1);
                    indexArray[index + 5] = (short)((y1 + 1) * (domeSubX + 1) + (x1 + 1));
                    index += 6;
                }
            }
            this.domeIndexBuffer[face].Unlock();
        }

        private void CleanupDomeVertexBuffers()
        {
            if (domeIndexBuffer != null)
            {
                for (int face = 0; face < 5; face++)
                {
                    CleanupDomeVertexBuffer(face);
                }
            }
        }

        private void CleanupDomeVertexBuffer(int face)
        {
            if (domeIndexBuffer[face] != null)
            {
                domeIndexBuffer[face].Dispose();
                GC.SuppressFinalize(domeIndexBuffer[face]);
            }

            if (domeVertexBuffer[face] != null)
            {
                domeVertexBuffer[face].Dispose();
                GC.SuppressFinalize(domeVertexBuffer[face]);
            }
        }

        PositionColorTexturedVertexBuffer11 warpVertexBuffer;
        IndexBuffer11 warpIndexBuffer;
        int warpVertexCount;
        int warpIndexCount;
        int warpTriangleCount;

        public void CreateWarpVertexBuffer()
        {
            ReadWarpMeshFile();
            int warpSubX = meshX - 1;
            int warpSubY = meshY - 1;

            CleanUpWarpBuffers();


            warpIndexBuffer = new IndexBuffer11(typeof(short), (warpSubX * warpSubY * 6), RenderContext11.PrepDevice);
            warpVertexBuffer = new PositionColorTexturedVertexBuffer11(((warpSubX + 1) * (warpSubY + 1)), RenderContext11.PrepDevice);

            warpVertexCount = ((warpSubX + 1) * (warpSubY + 1));


            int index = 0;

            PositionColorTexturedVertexBuffer11 vb = warpVertexBuffer;
            // Create a vertex buffer 
            PositionColoredTextured[] verts = (PositionColoredTextured[])vb.Lock(0, 0); // Lock the buffer (which will return our structs)
            int x1, y1;



            double textureStepX = 1.0f / warpSubX;
            double textureStepY = 1.0f / warpSubY;
            for (y1 = 0; y1 <= warpSubY; y1++)
            {

                for (x1 = 0; x1 <= warpSubX; x1++)
                {

                    index = y1 * (warpSubX + 1) + x1;
                    verts[index].Position = mesh[x1, y1].Position;
                    verts[index].Tu = mesh[x1, y1].Tu;
                    verts[index].Tv = mesh[x1, y1].Tv;
                    verts[index].Color = mesh[x1, y1].Color;
                }
            }
            vb.Unlock();
            warpTriangleCount = (warpSubX) * (warpSubY) * 2;
            short[] indexArray = (short[])warpIndexBuffer.Lock();
            index = 0;
            for (y1 = 0; y1 < warpSubY; y1++)
            {
                for (x1 = 0; x1 < warpSubX; x1++)
                {
                    // First triangle in quad
                    indexArray[index] = (short)(y1 * (warpSubX + 1) + x1);
                    indexArray[index + 1] = (short)((y1 + 1) * (warpSubX + 1) + x1);
                    indexArray[index + 2] = (short)(y1 * (warpSubX + 1) + (x1 + 1));

                    // Second triangle in quad
                    indexArray[index + 3] = (short)(y1 * (warpSubX + 1) + (x1 + 1));
                    indexArray[index + 4] = (short)((y1 + 1) * (warpSubX + 1) + x1);
                    indexArray[index + 5] = (short)((y1 + 1) * (warpSubX + 1) + (x1 + 1));
                    index += 6;
                }
            }
            this.warpIndexBuffer.Unlock();
        }

        private void CleanUpWarpBuffers()
        {
            if (warpIndexBuffer != null)
            {
                warpIndexBuffer.Dispose();
                GC.SuppressFinalize(warpIndexBuffer);
            }

            if (warpVertexBuffer != null)
            {
                warpVertexBuffer.Dispose();
                GC.SuppressFinalize(warpVertexBuffer);
            }
        }

        PositionColoredTextured[,] mesh;
        // bool WarpedDome = true;
        int meshX = 0;
        int meshY = 0;


        public void ReadWarpMeshFile()
        {

            string filename = Properties.Settings.Default.CahceDirectory + "meshwarp.txt";
            string appdir = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            if (Properties.Settings.Default.DomeTypeIndex == 3 && String.IsNullOrEmpty(Properties.Settings.Default.CustomWarpFilename))
            {
                Properties.Settings.Default.DomeTypeIndex = 1;
            }


            switch (Properties.Settings.Default.DomeTypeIndex)
            {
                default:
                case 1:
                    filename = appdir + "\\MeshWarps\\warp_mirror_16x9.data";
                    break;
                case 2:
                    filename = appdir + "\\MeshWarps\\warp_mirror_4x3.data";
                    break;
                case 3:
                    filename = Properties.Settings.Default.CustomWarpFilename;
                    break;
            }

            if (!File.Exists(filename))
            {
                return;
            }


            StreamReader sr = new StreamReader(filename, Encoding.ASCII);
            string buffer = sr.ReadLine();
            buffer = sr.ReadLine();
            string[] parts = buffer.Split(new char[] { ' ' });

            meshX = Convert.ToInt32(parts[0]);
            meshY = Convert.ToInt32(parts[1]);
            mesh = new PositionColoredTextured[meshX, meshY];

            for (int y = 0; y < meshY; y++)
            {
                for (int x = 0; x < meshX; x++)
                {
                    buffer = sr.ReadLine();
                    parts = buffer.Split(new char[] { ' ', '\t' });
                    mesh[x, y].Position = new SharpDX.Vector4((Convert.ToSingle(parts[0])) / 2, Convert.ToSingle(parts[1]) / 2, .9f, 1);
                    mesh[x, y].Tu = Convert.ToSingle(parts[2]);
                    mesh[x, y].Tv = 1.0f - Convert.ToSingle(parts[3]);
                    byte col = (Byte)(Convert.ToSingle(parts[4]) * 255);
                    mesh[x, y].Color = SysColor.FromArgb(255, col, col, col);
                }
            }

            sr.Close();
        }

        private void CleanupStereoAndDomeBuffers()
        {


            if (leftEye != null)
            {
                leftEye.Dispose();
                GC.SuppressFinalize(leftEye);
                leftEye = null;
            }

            if (rightEye != null)
            {
                rightEye.Dispose();
                GC.SuppressFinalize(rightEye);
                rightEye = null;
            }

            if (stereoRenderTextureLeft != null)
            {
                stereoRenderTextureLeft.Dispose();
                GC.SuppressFinalize(stereoRenderTextureLeft);
                stereoRenderTextureLeft = null;
            }

            if (stereoRenderTextureRight != null)
            {
                stereoRenderTextureRight.Dispose();
                GC.SuppressFinalize(stereoRenderTextureRight);
                stereoRenderTextureRight = null;
            }

            if (leftDepthBuffer != null)
            {
                leftDepthBuffer.Dispose();
                GC.SuppressFinalize(leftDepthBuffer);
                leftDepthBuffer = null;
            }

            if (rightDepthBuffer != null)
            {
                rightDepthBuffer.Dispose();
                GC.SuppressFinalize(rightDepthBuffer);
                rightDepthBuffer = null;
            }

            if (domeZbuffer != null)
            {
                domeZbuffer.Dispose();
                GC.SuppressFinalize(domeZbuffer);
                domeZbuffer = null;
            }

            for (int face = 0; face < 5; face++)
            {
                if (domeCube[face] != null)
                {
                    domeCube[face].Dispose();
                    GC.SuppressFinalize(domeCube[face]);
                    domeCube[face] = null;
                }
            }

            if (domeCubeFaceMultisampled != null)
            {
                domeCubeFaceMultisampled.Dispose();
                GC.SuppressFinalize(domeCubeFaceMultisampled);
                domeCubeFaceMultisampled = null;
            }

            if (undistorted != null)
            {
                undistorted.Dispose();
                GC.SuppressFinalize(undistorted);
                undistorted = null;
            }
        }
#endif
        // End Warp and Blend Code

        //oculus rift specifc code  
        public bool rift = false;
        private bool showRingMenuLeft;
        private bool showRingMenuRight;
#if !WINDOWS_UWP

        bool riftInit = false;
        private Wrap wrap = new Wrap();
        private Hmd hmd;
        private EyeTexture[] eyeTextures = null;

        private SharpDX.Direct3D11.Texture2D mirrorTexture;
        private Texture11 mirror;

        private OVRTypes.Posef[] eyeRenderPose = new OVRTypes.Posef[2];

        private SharpDX.Vector3[] hmdToEyeViewOffset = new SharpDX.Vector3[2];

        private SharpDX.Vector3 headPos = new SharpDX.Vector3(0f, 0f, -5f);
        private float bodyYaw = 3.141592f;
        private Layers layers = null;
        LayerEyeFov layerEyeFov;
        private int leftEyeWidth = 1;
        private int leftEyeHeight = 1;
        private int rightEyeWidth = 1;
        private int rightEyeHeight = 1;

        uint riftFrameIndex = 0;

        SharpDX.DXGI.Format riftFormat = SharpDX.DXGI.Format.R8G8B8A8_UNorm;

        protected void InitializeRift()
        {



            SharpDX.DXGI.Factory factory = null;
            MirrorTexture mirrorTextureWrap = null;
            Guid textureInterfaceId = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c"); // Interface ID of the Direct3D Texture2D interface.

            // Define initialization parameters with debug flag.
            OVRTypes.InitParams initializationParameters = new OVRTypes.InitParams();

            //todo remove detbug flag
            //initializationParameters.Flags = OVRTypes.InitFlags.Debug;
            initializationParameters.Flags = OVRTypes.InitFlags.RequestVersion;
            initializationParameters.RequestedMinorVersion = 8;

            // Initialize the Oculus runtime.
            bool success = wrap.Initialize(initializationParameters);
            if (!success)
            {
                MessageBox.Show("Failed to initialize the Oculus runtime library.", "Uh oh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Use the head mounted display.
            OVRTypes.GraphicsLuid graphicsLuid;
            hmd = wrap.Hmd_Create(out graphicsLuid);
            if (hmd == null)
            {
                MessageBox.Show("Oculus Rift not detected.", "Uh oh", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (hmd.ProductName == string.Empty)
            {
                MessageBox.Show("The HMD is not enabled.", "There's a tear in the Rift", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Create a set of layers to submit.
                eyeTextures = new EyeTexture[2];
                OVRTypes.Result result;

                // Create DirectX drawing device.
                SharpDX.Direct3D11.Device device = RenderContext11.Device;

                // Create DirectX Graphics Interface factory, used to create the swap chain.
                factory = new SharpDX.DXGI.Factory();


                // Retrieve the DXGI device, in order to set the maximum frame latency.
                using (SharpDX.DXGI.Device1 dxgiDevice = device.QueryInterface<SharpDX.DXGI.Device1>())
                {
                    dxgiDevice.MaximumFrameLatency = 1;
                }

                layers = new Layers();
                layerEyeFov = layers.AddLayerEyeFov();

                for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
                {
                    OVRTypes.EyeType eye = (OVRTypes.EyeType)eyeIndex;
                    EyeTexture eyeTexture = new EyeTexture();
                    eyeTextures[eyeIndex] = eyeTexture;

                    // Retrieve size and position of the texture for the current eye.
                    eyeTexture.FieldOfView = hmd.DefaultEyeFov[eyeIndex];
                    eyeTexture.TextureSize = hmd.GetFovTextureSize(eye, hmd.DefaultEyeFov[eyeIndex], 1.0f);
                    eyeTexture.RenderDescription = hmd.GetRenderDesc(eye, hmd.DefaultEyeFov[eyeIndex]);
                    eyeTexture.HmdToEyeViewOffset = eyeTexture.RenderDescription.HmdToEyeOffset;
                    eyeTexture.ViewportSize.Position = new OVRTypes.Vector2i(0, 0);
                    eyeTexture.ViewportSize.Size = eyeTexture.TextureSize;
                    eyeTexture.Viewport = new SharpDX.Viewport(0, 0, eyeTexture.TextureSize.Width, eyeTexture.TextureSize.Height, 0.0f, 1.0f);

                    // Define a texture at the size recommended for the eye texture.
                    eyeTexture.Texture2DDescription = new SharpDX.Direct3D11.Texture2DDescription();
                    eyeTexture.Texture2DDescription.Width = eyeTexture.TextureSize.Width;
                    eyeTexture.Texture2DDescription.Height = eyeTexture.TextureSize.Height;
                    eyeTexture.Texture2DDescription.ArraySize = 1;
                    eyeTexture.Texture2DDescription.MipLevels = 1;
                    eyeTexture.Texture2DDescription.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
                    eyeTexture.Texture2DDescription.SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0);
                    eyeTexture.Texture2DDescription.Usage = SharpDX.Direct3D11.ResourceUsage.Default;
                    eyeTexture.Texture2DDescription.CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None;
                    eyeTexture.Texture2DDescription.BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource | SharpDX.Direct3D11.BindFlags.RenderTarget;

                    // Convert the SharpDX texture description to the Oculus texture swap chain description.
                    OVRTypes.TextureSwapChainDesc textureSwapChainDesc = SharpDXHelpers.CreateTextureSwapChainDescription(eyeTexture.Texture2DDescription);

                    // Create a texture swap chain, which will contain the textures to render to, for the current eye.
                    result = hmd.CreateTextureSwapChainDX(device.NativePointer, textureSwapChainDesc, out eyeTexture.SwapTextureSet);


                    // Retrieve the number of buffers of the created swap chain.
                    int textureSwapChainBufferCount;
                    result = eyeTexture.SwapTextureSet.GetLength(out textureSwapChainBufferCount);

                    // Create room for each DirectX texture in the SwapTextureSet.
                    eyeTexture.Textures = new SharpDX.Direct3D11.Texture2D[textureSwapChainBufferCount];
                    eyeTexture.RenderTargetViews = new SharpDX.Direct3D11.RenderTargetView[textureSwapChainBufferCount];

                    // Create a texture 2D and a render target view, for each unmanaged texture contained in the SwapTextureSet.
                    for (int textureIndex = 0; textureIndex < textureSwapChainBufferCount; textureIndex++)
                    {
                        // Retrieve the Direct3D texture contained in the Oculus TextureSwapChainBuffer.
                        IntPtr swapChainTextureComPtr = IntPtr.Zero;
                        result = eyeTexture.SwapTextureSet.GetBufferDX(textureIndex, textureInterfaceId, out swapChainTextureComPtr);

                        // Create a managed Texture2D, based on the unmanaged texture pointer.
                        eyeTexture.Textures[textureIndex] = new SharpDX.Direct3D11.Texture2D(swapChainTextureComPtr);

                        // Create a render target view for the current Texture2D.
                        eyeTexture.RenderTargetViews[textureIndex] = new SharpDX.Direct3D11.RenderTargetView(device, eyeTexture.Textures[textureIndex]);
                    }

                    // Define the depth buffer, at the size recommended for the eye texture.
                    eyeTexture.DepthBufferDescription = new SharpDX.Direct3D11.Texture2DDescription();
                    eyeTexture.DepthBufferDescription.Format = SharpDX.DXGI.Format.D32_Float;
                    eyeTexture.DepthBufferDescription.Width = eyeTexture.TextureSize.Width;
                    eyeTexture.DepthBufferDescription.Height = eyeTexture.TextureSize.Height;
                    eyeTexture.DepthBufferDescription.ArraySize = 1;
                    eyeTexture.DepthBufferDescription.MipLevels = 1;
                    eyeTexture.DepthBufferDescription.SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0);
                    eyeTexture.DepthBufferDescription.Usage = SharpDX.Direct3D11.ResourceUsage.Default;
                    eyeTexture.DepthBufferDescription.BindFlags = SharpDX.Direct3D11.BindFlags.DepthStencil;
                    eyeTexture.DepthBufferDescription.CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None;
                    eyeTexture.DepthBufferDescription.OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None;

                    // Create the depth buffer.
                    eyeTexture.DepthBuffer = new SharpDX.Direct3D11.Texture2D(device, eyeTexture.DepthBufferDescription);
                    eyeTexture.DepthStencilView = new SharpDX.Direct3D11.DepthStencilView(device, eyeTexture.DepthBuffer);

                    // Specify the texture to show on the HMD.
                    layerEyeFov.ColorTexture[eyeIndex] = eyeTexture.SwapTextureSet.TextureSwapChainPtr;
                    layerEyeFov.Viewport[eyeIndex].Position = new OVRTypes.Vector2i(0, 0);
                    layerEyeFov.Viewport[eyeIndex].Size = eyeTexture.TextureSize;
                    layerEyeFov.Fov[eyeIndex] = eyeTexture.FieldOfView;
                    layerEyeFov.Header.Flags = OVRTypes.LayerFlags.None;
                }



                OVRTypes.MirrorTextureDesc mirrorTextureDescription = new OVRTypes.MirrorTextureDesc();
                mirrorTextureDescription.Format = OVRTypes.TextureFormat.R8G8B8A8_UNORM_SRGB;
                mirrorTextureDescription.Width = RenderContext11.BackBuffer.Description.Width;
                mirrorTextureDescription.Height = RenderContext11.BackBuffer.Description.Height;
                mirrorTextureDescription.MiscFlags = OVRTypes.TextureMiscFlags.None;

                // Create the texture used to display the rendered result on the computer monitor.
                result = hmd.CreateMirrorTextureDX(device.NativePointer, mirrorTextureDescription, out mirrorTextureWrap);

                // Retrieve the Direct3D texture contained in the Oculus MirrorTexture.
                IntPtr mirrorTextureComPtr = IntPtr.Zero;
                result = mirrorTextureWrap.GetBufferDX(textureInterfaceId, out mirrorTextureComPtr);

                // Create a managed Texture2D, based on the unmanaged texture pointer.
                mirrorTexture = new SharpDX.Direct3D11.Texture2D(mirrorTextureComPtr);
                mirror = new Texture11(mirrorTexture);
                riftInit = true;
                leftEyeWidth = eyeTextures[0].TextureSize.Width;
                leftEyeHeight = eyeTextures[0].TextureSize.Height;
                rightEyeWidth = eyeTextures[1].TextureSize.Width;
                rightEyeHeight = eyeTextures[1].TextureSize.Height;
            }
            catch
            {

            }
        }




        OVRTypes.TrackingState trackingState;
        void GetRiftSample()
        {


            OVRTypes.Vector3f[] hmdToEyeViewOffsets = { eyeTextures[0].HmdToEyeViewOffset, eyeTextures[1].HmdToEyeViewOffset };
            double displayMidpoint = hmd.GetPredictedDisplayTime(0);
            trackingState = hmd.GetTrackingState(displayMidpoint, true);

            // Calculate the position and orientation of each eye.
            wrap.CalcEyePoses(trackingState.HeadPose.ThePose, hmdToEyeViewOffsets, ref eyeRenderPose);

            LeftController.Position = new Vector3d(trackingState.HandPoses[0].ThePose.Position.ToVector3());
            LeftController.Active = true;
            LeftController.Forward = new Vector3d(0, 0, 1);
            LeftController.Up = new Vector3d(0, 1, 0);

            for (int eyeIndex = 0; eyeIndex < 2; eyeIndex++)
            {
                OVRTypes.EyeType eye = (OVRTypes.EyeType)eyeIndex;
                EyeTexture eyeTexture = eyeTextures[eyeIndex];

                layerEyeFov.RenderPose[eyeIndex] = eyeRenderPose[eyeIndex];

                // Update the render description at each frame, as the HmdToEyeOffset can change at runtime.
                eyeTexture.RenderDescription = hmd.GetRenderDesc(eye, hmd.DefaultEyeFov[eyeIndex]);

            }
        }

      

        public void StartRift()
        {
            try
            {
                if (!riftInit)
                {
                    InitializeRift();
                }

                rift = true;
                StereoMode = RenderEngine.StereoModes.OculusRift;

                Properties.Settings.Default.ColSettingsVersion++;
            }
            catch
            {
                UiTools.ShowMessageBox("Unable to connect to Oculus Rift. Please make sure its not already in use or check setup using the Rift Configuration tool and try the test scene.");
            }
        }

        //end Oculus Rift code
#endif

    }    
}
