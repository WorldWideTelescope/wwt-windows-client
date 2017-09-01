using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml;

namespace TerraViewer
{
    enum TransitionType { Slew, CrossFade, CrossCut, FadeOutIn,  FadeIn, FadeOut };
    public class TourStop : ISettings
    {
        public const string ClipboardFormat = "WorldWideTelescope.Slide";

        ImageSetType tourStopType;

        public KeyframeMover KeyFrameMover = new KeyframeMover();

        public List<AnimationTarget> AnimationTargets = new List<AnimationTarget>();
        private bool keyFramed = false;

        public bool KeyFramed
        {
            get { return keyFramed; }
            set
            { 
                if (keyFramed != value)
                {

                    keyFramed = value;

                    if (keyFramed)
                    {
                        //Create Camera Keyframe
                        CameraParameters savedCam = RenderEngine.Engine.viewCamera;
                        DateTime savedDate = SpaceTimeController.Now;
                        AnimationTarget at = new AnimationTarget(this);
                        at.Target = this.KeyFrameMover;
                        at.TargetType = AnimationTarget.AnimationTargetTypes.Camera;
                        at.ParameterNames.AddRange(at.Target.GetParamNames());

                        RenderEngine.Engine.viewCamera = target.CamParams;
                        SpaceTimeController.Now = startTime;

                        at.CurrentParameters = at.Target.GetParams();
                        at.SetKeyFrame(0, Key.KeyType.Linear);

                        if (endTarget != null)
                        {
                            RenderEngine.Engine.viewCamera = endTarget.CamParams;
                            SpaceTimeController.Now = endTime;
                            at.CurrentParameters = at.Target.GetParams();
                            at.SetKeyFrame(1, Key.KeyType.Linear);

                        }  
                        RenderEngine.Engine.viewCamera = savedCam;
                        SpaceTimeController.Now = savedDate; 
                        AnimationTargets.Add(at);
                        KeyFramer.ShowTimeline();
                        TimeLine.RefreshUi();
                    }
                }
            }
        }

        public void SetStartKeyFrames()
        {
            if (keyFramed)
            {
                foreach (AnimationTarget target in AnimationTargets)
                {
                    target.SetKeyFrame(0, Key.KeyType.Linear);
                }
            }
        }

        public void SetEndKeyFrames()
        {
            if (keyFramed)
            {
                foreach (AnimationTarget target in AnimationTargets)
                {
                    target.SetKeyFrame(1, Key.KeyType.Linear);
                }
            }
        }


        public bool IsTargetAnimated(string id)
        {
            foreach (AnimationTarget t in AnimationTargets)
            {
                if (t.TargetID == id)
                {
                    return true;
                }
            }
            return false;
        }

        public AnimationTarget FindTarget(string id)
        {
            foreach (AnimationTarget target in AnimationTargets)
            {
                if (target.TargetID == id)
                {
                    return target;
                }
            }

            return null;
        }

        public ImageSetType TourStopType
        {
            get
            {
                if (target.BackgroundImageSet != null)
                {
                    return target.BackgroundImageSet.DataSetType;
                }
                else
                {
                    return tourStopType;
                }
            }
            set
            {
                if (target.BackgroundImageSet != null)
                {
                    if (target.BackgroundImageSet.DataSetType != value)
                    {
                        target.BackgroundImageSet = null;
                    }
                }
                tourStopType = value;
            }
        }

        private float tweenPosition = 0;

        public float TweenPosition
        {
            get { return tweenPosition; }
            set
            {
                if (tweenPosition != value)
                {
                    tweenPosition = Math.Max(0,Math.Min(1,value));
                    UpdateTweenPosition();
                }
                    
            }
        }

        public double FaderOpacity = 0;

        public void UpdateTweenPosition()
        {
            if (KeyFramed)
            {
                KeyFrameMover.CurrentDateTime = this.StartTime;
                KeyFrameMover.ReferenceFrame = this.Target.CamParams.TargetReferenceFrame;
                KeyFrameMover.MoveTime = (double)(Duration.TotalMilliseconds / 1000.0);
                //update key framed elements
                foreach (AnimationTarget target in AnimationTargets)
                {
                    target.Tween(tweenPosition);
                }
                RenderEngine.Engine.UpdateMover(KeyFrameMover);
                SpaceTimeController.Now = KeyFrameMover.CurrentDateTime;
            }
        }

        public TourStop Copy()
        {
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter textWriter = new System.IO.StringWriter(sb))
            {
                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(textWriter))
                {
                    writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");

                    this.SaveToXml(writer, true);
                }
            }

            // add try catch block
            try
            {
                string xml = sb.ToString();
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                System.Xml.XmlNode node = doc["TourStop"];
                TourStop ts = TourStop.FromXml(this.Owner, node);
                ts.Id = Guid.NewGuid().ToString();
                return ts;
            }
            catch
            {
            }
            return null;
        }


        public TourStop()
        {
            id = Guid.NewGuid().ToString();

        }

        public TourStop(TourPlace target)
        {
            this.target = target;
            id = Guid.NewGuid().ToString();

        }

        TourDocument owner = null;

        public TourDocument Owner
        {
            get { return owner; }
            set { owner = value; }
        }
        TransitionType transition = TransitionType.Slew;

        internal TransitionType Transition
        {
            get { return transition; }
            set
            {
                if (transition != value)
                {
                    transition = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        private double transitionTime = 2;

        internal double TransitionTime
        {
            get { return transitionTime; }
            set
            {
                if (transitionTime != value)
                {
                    transitionTime = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        private double transitionHoldTime = 4;

        internal double TransitionHoldTime
        {
            get { return transitionHoldTime; }
            set
            {
                if (transitionHoldTime != value)
                {
                    transitionHoldTime = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }


        private double transitionOutTime = 2;

        internal double TransitionOutTime
        {
            get { return transitionOutTime; }
            set
            {
                if (transitionOutTime != value)
                {
                    transitionOutTime = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        bool fadeInOverlays = false;

        public bool FadeInOverlays
        {
            get { return fadeInOverlays; }
            set { fadeInOverlays = value; }
        }

        bool masterSlide = false;

        public bool MasterSlide
        {
            get { return masterSlide; }
            set
            {
                if (masterSlide != value)
                {
                    masterSlide = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        string nextSlide = "Next";

        public string NextSlide
        {
            get { return nextSlide; }
            set { nextSlide = value; }
        }

        public bool IsLinked
        {
            get
            {
                if (nextSlide == null || nextSlide == "Next" || nextSlide == "")
                {
                    return false;
                }
                return true;
            }
        }

        string id;

        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                if (owner != null) { owner.TourDirty = true; }
            }
        }

        public override string ToString()
        {
            if (target != null)
            {
                return Target.Name;
            }
            else
            {
                return description;
            }
        }

        string description;

        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }
        private string name;

        public string Name
        {
            get
            {
                if (target != null)
                {
                    return target.Name;
                }
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        TimeSpan duration = new TimeSpan(0, 0, 0, 10, 0);

        public TimeSpan Duration
        {
            get { return duration; }
            set
            {
                if (duration != value)
                {
                    duration = value;
                    if (owner != null)
                    { 
                        owner.TourDirty = true;
                        TimeLine.RefreshUi();
                    }
                }
            }
        }

        TourPlace target;

        public TourPlace Target
        {
            get { return target; }
            set
            {
                if (target != value)
                {
                    target = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        TourPlace endTarget;

        public TourPlace EndTarget
        {
            get { return endTarget; }
            set
            {
                if (endTarget != value)
                {
                    endTarget = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        InterpolationType interpolationType = InterpolationType.Linear;

        public InterpolationType InterpolationType
        {
            get { return interpolationType; }
            set { interpolationType = value; }
        }


        // Settings

        bool hasLocation = true;

        public bool HasLocation
        {
            get { return hasTime; }
            set
            {
                if (hasLocation != value)
                {
                    hasLocation = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }
        bool hasTime = true;

        public bool HasTime
        {
            get { return hasTime; }
            set
            {
                if (hasTime != value)
                {
                    hasTime = hasLocation = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        DateTime startTime = SpaceTimeController.Now;

        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                startTime = value;
                if (startTime != value)
                {
                    startTime = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }
        DateTime endTime = SpaceTimeController.Now;

        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                if (endTime != value)
                {
                    endTime = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }


        bool actualPlanetScale = Properties.Settings.Default.ActualPlanetScale;
        double locationAltitude = Properties.Settings.Default.LocationAltitude;
        double locationLat = Properties.Settings.Default.LocationLat;
        double locationLng = Properties.Settings.Default.LocationLng;
        bool showClouds = Properties.Settings.Default.ShowClouds.TargetState;
        bool earthCutawayView = Properties.Settings.Default.EarthCutawayView.TargetState;
        bool showConstellationBoundries = Properties.Settings.Default.ShowConstellationBoundries.TargetState;
        bool showConstellationFigures = Properties.Settings.Default.ShowConstellationFigures.TargetState;
        bool showConstellationSelection = Properties.Settings.Default.ShowConstellationSelection.TargetState;
        bool showEcliptic = Properties.Settings.Default.ShowEcliptic.TargetState;
        bool showElevationModel = Properties.Settings.Default.ShowElevationModel;
        bool showFieldOfView = Properties.Settings.Default.ShowFieldOfView.TargetState;
        bool showGrid = Properties.Settings.Default.ShowGrid.TargetState;
        bool showHorizon = Properties.Settings.Default.ShowHorizon;
        bool showHorizonPanorama = Properties.Settings.Default.ShowHorizonPanorama;
        bool showMoonsAsPointSource = Properties.Settings.Default.ShowMoonsAsPointSource;
        bool showSolarSystem = Properties.Settings.Default.ShowSolarSystem.TargetState;
        int fovTelescope = Properties.Settings.Default.FovTelescope;
        int fovEyepiece = Properties.Settings.Default.FovEyepiece;
        int fovCamera = Properties.Settings.Default.FovCamera;
        bool localHorizonMode = Properties.Settings.Default.LocalHorizonMode;
        //bool milkyWayModel = Properties.Settings.Default.MilkyWayModel;
        bool galacticMode = Properties.Settings.Default.GalacticMode;
        ConstellationFilter constellationFiguresFilter = Properties.Settings.Default.ConstellationFiguresFilter.Clone();
        ConstellationFilter constellationBoundariesFilter = Properties.Settings.Default.ConstellationBoundariesFilter.Clone();
        ConstellationFilter constellationNamesFilter = Properties.Settings.Default.ConstellationNamesFilter.Clone();
        ConstellationFilter constellationArtFilter = Properties.Settings.Default.ConstellationArtFilter.Clone();


        bool solarSystemStars = Properties.Settings.Default.SolarSystemStars.TargetState;
        bool solarSystemMilkyWay = Properties.Settings.Default.SolarSystemMilkyWay.TargetState;
        bool solarSystemCosmos = Properties.Settings.Default.SolarSystemCosmos.TargetState;
        bool solarSystemCMB = Properties.Settings.Default.SolarSystemCMB.TargetState;
        bool solarSystemMinorPlanets = Properties.Settings.Default.SolarSystemMinorPlanets.TargetState;
        bool solarSystemPlanets = Properties.Settings.Default.SolarSystemPlanets.TargetState;
        bool solarSystemOrbits = Properties.Settings.Default.SolarSystemOrbits.TargetState;
        bool solarSystemMinorOrbits = Properties.Settings.Default.SolarSystemMinorOrbits.TargetState;
        bool solarSystemOverlays = Properties.Settings.Default.SolarSystemOverlays.TargetState;
        bool solarSystemLighting = Properties.Settings.Default.SolarSystemLighting;
        bool showISSModel = Properties.Settings.Default.ShowISSModel;
        int solarSystemScale = Properties.Settings.Default.SolarSystemScale;
        int minorPlanetsFilter = Properties.Settings.Default.MinorPlanetsFilter;
        int planetOrbitsFilter = Properties.Settings.Default.PlanetOrbitsFilter;
            
        bool solarSystemMultiRes = Properties.Settings.Default.SolarSystemMultiRes;
        bool showEarthSky = Properties.Settings.Default.ShowEarthSky.TargetState;

        bool showEquatorialGridText = Properties.Settings.Default.ShowEquatorialGridText.TargetState;
        bool showGalacticGrid = Properties.Settings.Default.ShowGalacticGrid.TargetState;
        bool showGalacticGridText = Properties.Settings.Default.ShowGalacticGridText.TargetState;
        bool showEclipticGrid = Properties.Settings.Default.ShowEclipticGrid.TargetState;
        bool showEclipticGridText = Properties.Settings.Default.ShowEclipticGridText.TargetState;
        bool showEclipticOverviewText = Properties.Settings.Default.ShowEclipticOverviewText.TargetState;
        bool showAltAzGrid = Properties.Settings.Default.ShowAltAzGrid.TargetState;
        bool showAltAzGridText = Properties.Settings.Default.ShowAltAzGridText.TargetState;
        bool showPrecessionChart = Properties.Settings.Default.ShowPrecessionChart.TargetState;
        bool showConstellationPictures = Properties.Settings.Default.ShowConstellationPictures.TargetState;
        bool showConstellationLabels = Properties.Settings.Default.ShowConstellationLabels.TargetState;
        string constellationsEnabled = Properties.Settings.Default.ConstellationsEnabled;
        bool showSkyOverlays = Properties.Settings.Default.ShowSkyOverlays.TargetState;
        bool showConstellations = Properties.Settings.Default.Constellations.TargetState;
        bool showSkyNode = Properties.Settings.Default.ShowSkyNode.TargetState;
        bool showSkyGrids = Properties.Settings.Default.ShowSkyGrids.TargetState;
        bool skyOverlaysIn3d = Properties.Settings.Default.ShowSkyOverlaysIn3d.TargetState;

        public void CaptureSettings()
        {
            startTime = SpaceTimeController.Now;
            actualPlanetScale = Properties.Settings.Default.ActualPlanetScale;
            locationAltitude = Properties.Settings.Default.LocationAltitude;
            locationLat = Properties.Settings.Default.LocationLat;
            locationLng = Properties.Settings.Default.LocationLng;
            showClouds = Properties.Settings.Default.ShowClouds.TargetState;
            earthCutawayView = Properties.Settings.Default.EarthCutawayView.TargetState;
            showConstellationBoundries = Properties.Settings.Default.ShowConstellationBoundries.TargetState;
            showConstellationFigures = Properties.Settings.Default.ShowConstellationFigures.TargetState;
            showConstellationSelection = Properties.Settings.Default.ShowConstellationSelection.TargetState;
            showEcliptic = Properties.Settings.Default.ShowEcliptic.TargetState;
            showElevationModel = Properties.Settings.Default.ShowElevationModel;
            showFieldOfView = Properties.Settings.Default.ShowFieldOfView.TargetState;
            showGrid = Properties.Settings.Default.ShowGrid.TargetState;
            showHorizon = Properties.Settings.Default.ShowHorizon;
            showHorizonPanorama = Properties.Settings.Default.ShowHorizonPanorama;
            showMoonsAsPointSource = Properties.Settings.Default.ShowMoonsAsPointSource;
            showSolarSystem = Properties.Settings.Default.ShowSolarSystem.TargetState;
            fovTelescope = Properties.Settings.Default.FovTelescope;
            fovEyepiece = Properties.Settings.Default.FovEyepiece;
            fovCamera = Properties.Settings.Default.FovCamera;
            localHorizonMode = Properties.Settings.Default.LocalHorizonMode;
            //milkyWayModel = Properties.Settings.Default.MilkyWayModel;
            galacticMode = Properties.Settings.Default.GalacticMode;

            solarSystemStars = Properties.Settings.Default.SolarSystemStars.TargetState;
            solarSystemMilkyWay = Properties.Settings.Default.SolarSystemMilkyWay.TargetState;
            solarSystemCosmos = Properties.Settings.Default.SolarSystemCosmos.TargetState;
            solarSystemCMB = Properties.Settings.Default.SolarSystemCMB.TargetState;
            solarSystemOrbits = Properties.Settings.Default.SolarSystemOrbits.TargetState;
            solarSystemMinorOrbits = Properties.Settings.Default.SolarSystemMinorOrbits.TargetState;
            solarSystemMinorPlanets = Properties.Settings.Default.SolarSystemMinorPlanets.TargetState;
            solarSystemOverlays = Properties.Settings.Default.SolarSystemOverlays.TargetState;
            solarSystemLighting = Properties.Settings.Default.SolarSystemLighting;
            solarSystemScale = Properties.Settings.Default.SolarSystemScale;
            minorPlanetsFilter = Properties.Settings.Default.MinorPlanetsFilter;
            planetOrbitsFilter = Properties.Settings.Default.PlanetOrbitsFilter;
         

            solarSystemMultiRes = Properties.Settings.Default.SolarSystemMultiRes;
            showEarthSky = Properties.Settings.Default.ShowEarthSky.TargetState;

            showEquatorialGridText = Properties.Settings.Default.ShowEquatorialGridText.TargetState;
            showGalacticGrid = Properties.Settings.Default.ShowGalacticGrid.TargetState;
            showGalacticGridText = Properties.Settings.Default.ShowGalacticGridText.TargetState;
            showEclipticGrid = Properties.Settings.Default.ShowEclipticGrid.TargetState;
            showEclipticGridText = Properties.Settings.Default.ShowEclipticGridText.TargetState;
            showEclipticOverviewText = Properties.Settings.Default.ShowEclipticOverviewText.TargetState;
            showAltAzGrid = Properties.Settings.Default.ShowAltAzGrid.TargetState;
            showAltAzGridText = Properties.Settings.Default.ShowAltAzGridText.TargetState;
            showPrecessionChart = Properties.Settings.Default.ShowPrecessionChart.TargetState;
            showConstellationPictures = Properties.Settings.Default.ShowConstellationPictures.TargetState;
            constellationsEnabled = Properties.Settings.Default.ConstellationsEnabled;
            showConstellationLabels = Properties.Settings.Default.ShowConstellationLabels.TargetState;
            showSkyOverlays = Properties.Settings.Default.ShowSkyOverlays.TargetState;
            showConstellations = Properties.Settings.Default.Constellations.TargetState;
            showSkyNode = Properties.Settings.Default.ShowSkyNode.TargetState;
            showSkyGrids = Properties.Settings.Default.ShowSkyGrids.TargetState;
            skyOverlaysIn3d = Properties.Settings.Default.ShowSkyOverlaysIn3d.TargetState;
            constellationFiguresFilter.Clone(Properties.Settings.Default.ConstellationFiguresFilter);
            constellationBoundariesFilter.Clone(Properties.Settings.Default.ConstellationBoundariesFilter);
            constellationNamesFilter.Clone(Properties.Settings.Default.ConstellationNamesFilter);
            constellationArtFilter.Clone(Properties.Settings.Default.ConstellationArtFilter);
        }


        Dictionary<StockSkyOverlayTypes, SettingsAnimator> SettingsAnimators = new Dictionary<StockSkyOverlayTypes, SettingsAnimator>();




        public IAnimatable GetSettingAnimator(string TargetID)
        {
            try
            {
                StockSkyOverlayTypes type = (StockSkyOverlayTypes)Enum.Parse(typeof(StockSkyOverlayTypes), TargetID);

                if (SettingsAnimators.ContainsKey(type))
                {
                    SettingsAnimator sa = SettingsAnimators[type];
                    return sa;
                }
                else
                {
                    SettingsAnimator sa = new SettingsAnimator(type);
                    SettingsAnimators.Add(type, sa);
                    return sa;
                }
            }
            catch
            {
            }

            return null;
        }
        
        public SettingParameter GetSetting(StockSkyOverlayTypes type)
        {
            bool edgeTrigger = true;
            double opacity = 0;
            bool animated = false;
            ConstellationFilter filter = null;
            bool targetState = false;
            if (SettingsAnimators.ContainsKey(type))
            {
                SettingsAnimator sa = SettingsAnimators[type];           
                edgeTrigger = sa.Constant;
                opacity = sa.CurrentValue;
                filter = sa.Filter;
                animated = true;
            }
            else
            {

                switch (type)
                {
                    case StockSkyOverlayTypes.FadeToBlack:
                        opacity = FaderOpacity;
                        break;
                    case StockSkyOverlayTypes.EquatorialGrid:
                        opacity = this.showGrid ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.EquatorialGridText:
                        opacity = this.showEquatorialGridText ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.GalacticGrid:
                        opacity = this.showGalacticGrid ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.GalacticGridText:
                        opacity = this.showGalacticGridText ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.EclipticGrid:
                        opacity = this.showEclipticGrid ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.EclipticGridText:
                        opacity = this.showEclipticGridText ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.EclipticOverview:
                        opacity = this.showEcliptic ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.EclipticOverviewText:
                        opacity = this.showEclipticOverviewText ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.PrecessionChart:
                        opacity = this.showPrecessionChart ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.AltAzGrid:
                        opacity = this.showAltAzGrid ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.AltAzGridText:
                        opacity = this.showAltAzGridText ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.ConstellationFigures:
                        opacity = this.showConstellationFigures ? 1 : 0;
                        filter = this.constellationFiguresFilter.Clone();
                        break;
                    case StockSkyOverlayTypes.ConstellationBoundaries:
                        opacity = this.showConstellationBoundries ? 1 : 0;
                        filter = this.constellationBoundariesFilter.Clone();
                        break;
                    case StockSkyOverlayTypes.ConstellationFocusedOnly:
                        opacity = this.showConstellationSelection ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.ConstellationNames:
                        opacity = this.showConstellationLabels ? 1 : 0;
                        filter = this.constellationNamesFilter.Clone();
                        break;
                    case StockSkyOverlayTypes.ConstellationPictures:
                        opacity = this.showConstellationPictures ? 1 : 0;
                        filter = this.constellationArtFilter.Clone();
                        break;
                    case StockSkyOverlayTypes.SkyGrids:
                        opacity = this.showSkyGrids ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.Constellations:
                        opacity = this.showConstellations ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.SolarSystemStars:
                        opacity = this.solarSystemStars ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.SolarSystemMilkyWay:
                        opacity = this.solarSystemMilkyWay ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.SolarSystemCosmos:
                        opacity = this.solarSystemCosmos ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.SolarSystemCMB:
                        opacity = this.solarSystemCMB ? 1 : 0;
                        break;             
                    case StockSkyOverlayTypes.SolarSystemOrbits:
                        opacity = this.solarSystemOrbits ? 1 : 0;
                        edgeTrigger = true;
                        break;
                    case StockSkyOverlayTypes.OrbitFilters:
                        opacity = Properties.Settings.Default.PlanetOrbitsFilter;
                        edgeTrigger = true;
                        break;

                    case StockSkyOverlayTypes.MPCZone1:
                    case StockSkyOverlayTypes.MPCZone2:
                    case StockSkyOverlayTypes.MPCZone3:
                    case StockSkyOverlayTypes.MPCZone4:
                    case StockSkyOverlayTypes.MPCZone5:
                    case StockSkyOverlayTypes.MPCZone6:
                    case StockSkyOverlayTypes.MPCZone7:
                        {
                            int id = (int)type - (int)StockSkyOverlayTypes.MPCZone1;
                            int bit = (int)Math.Pow(2, id);
                            targetState = (this.minorPlanetsFilter & bit) != 0;
                            opacity = targetState ? 1 : 0;
                            edgeTrigger = true;
                        }
                        break;

                    case StockSkyOverlayTypes.SolarSystemPlanets:
                        opacity = this.solarSystemPlanets ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.SolarSystemAsteroids:
                        opacity = this.solarSystemMinorPlanets ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.SolarSystemLighting:
                        opacity = this.solarSystemLighting ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.ShowISSModel:
                        opacity = this.showISSModel ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.SolarSystemMinorOrbits:
                        opacity = this.solarSystemMinorOrbits ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.ShowEarthCloudLayer:
                        opacity = this.showClouds ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.ShowElevationModel:
                        opacity = this.showElevationModel ? 1 : 0;
                        break;

                    case StockSkyOverlayTypes.MultiResSolarSystemBodies:
                        opacity = this.solarSystemMultiRes ? 1 : 0;
                        break;

                    case StockSkyOverlayTypes.EarthCutAway:
                        opacity = earthCutawayView ? 1 : 0;
                        break;
                    case StockSkyOverlayTypes.ShowSolarSystem:
                        opacity = this.showSolarSystem ? 1 : 0;
                        break;
                        
                    case StockSkyOverlayTypes.FiledOfView:
                        opacity = this.showFieldOfView ? 1 : 0;
                        break;
                    default:
                        return new SettingParameter(false, -1, false, filter);
                }
            }

            SettingParameter sp = new SettingParameter(edgeTrigger, opacity, opacity != 0, filter);
            sp.Animated = animated;
            return sp;
        }

        public void SyncSettings()
        {
            Settings.ignoreChanges = true;
            LayerManager.ProcessingUpdate = true;
            Properties.Settings.Default.ActualPlanetScale = actualPlanetScale;
            Properties.Settings.Default.LocationAltitude = locationAltitude;
            Properties.Settings.Default.LocationLat = locationLat;
            Properties.Settings.Default.LocationLng = locationLng;
            Properties.Settings.Default.ShowClouds.TargetState = showClouds;
            Properties.Settings.Default.EarthCutawayView.TargetState = earthCutawayView;
            Properties.Settings.Default.ShowConstellationBoundries.TargetState = showConstellationBoundries;
            Properties.Settings.Default.ShowConstellationFigures.TargetState = showConstellationFigures;
            Properties.Settings.Default.ShowConstellationSelection.TargetState = showConstellationSelection;
            Properties.Settings.Default.ShowEcliptic.TargetState = showEcliptic;
            Properties.Settings.Default.ShowElevationModel = showElevationModel;
            Properties.Settings.Default.ShowFieldOfView.TargetState = showFieldOfView;
            Properties.Settings.Default.ShowGrid.TargetState = showGrid;
            Properties.Settings.Default.ShowHorizon = showHorizon;
            Properties.Settings.Default.ShowHorizonPanorama = showHorizonPanorama;
            Properties.Settings.Default.ShowMoonsAsPointSource = showMoonsAsPointSource;
            Properties.Settings.Default.ShowSolarSystem.TargetState = showSolarSystem;
            Properties.Settings.Default.FovTelescope = fovTelescope;
            Properties.Settings.Default.FovEyepiece = fovEyepiece;
            Properties.Settings.Default.FovCamera = fovCamera;
            Properties.Settings.Default.LocalHorizonMode = localHorizonMode;
            //Properties.Settings.Default.MilkyWayModel = milkyWayModel;
            Properties.Settings.Default.GalacticMode = galacticMode;

            Properties.Settings.Default.SolarSystemStars.TargetState = solarSystemStars;
            Properties.Settings.Default.SolarSystemMilkyWay.TargetState = solarSystemMilkyWay;
            Properties.Settings.Default.SolarSystemCosmos.TargetState = solarSystemCosmos;
            Properties.Settings.Default.SolarSystemCMB.TargetState = solarSystemCMB;
            Properties.Settings.Default.SolarSystemOrbits.TargetState = solarSystemOrbits;
            Properties.Settings.Default.SolarSystemMinorOrbits.TargetState = solarSystemMinorOrbits;
            Properties.Settings.Default.SolarSystemMinorPlanets.TargetState = solarSystemMinorPlanets;
            Properties.Settings.Default.SolarSystemOverlays.TargetState = solarSystemOverlays;
            Properties.Settings.Default.SolarSystemLighting = solarSystemLighting;
            Properties.Settings.Default.ShowISSModel = showISSModel;
            Properties.Settings.Default.SolarSystemScale = solarSystemScale;
            Properties.Settings.Default.SolarSystemMultiRes = solarSystemMultiRes;
            Properties.Settings.Default.ShowEarthSky.TargetState = showEarthSky;
            Properties.Settings.Default.MinorPlanetsFilter = minorPlanetsFilter;
            Properties.Settings.Default.PlanetOrbitsFilter = planetOrbitsFilter;
         

            Properties.Settings.Default.ShowEquatorialGridText.TargetState = showEquatorialGridText;
            Properties.Settings.Default.ShowGalacticGrid.TargetState = showGalacticGrid;
            Properties.Settings.Default.ShowGalacticGridText.TargetState = showGalacticGridText;
            Properties.Settings.Default.ShowEclipticGrid.TargetState = showEclipticGrid;
            Properties.Settings.Default.ShowEclipticGridText.TargetState = showEclipticGridText;
            Properties.Settings.Default.ShowEclipticOverviewText.TargetState = showEclipticOverviewText;
            Properties.Settings.Default.ShowAltAzGrid.TargetState = showAltAzGrid;
            Properties.Settings.Default.ShowAltAzGridText.TargetState = showAltAzGridText;
            Properties.Settings.Default.ShowPrecessionChart.TargetState = showPrecessionChart;
            Properties.Settings.Default.ShowConstellationPictures.TargetState = showConstellationPictures;
            Properties.Settings.Default.ConstellationsEnabled = constellationsEnabled;

            Properties.Settings.Default.ShowSkyOverlays.TargetState = showSkyOverlays;
            Properties.Settings.Default.Constellations.TargetState = showConstellations;
            Properties.Settings.Default.ShowSkyNode.TargetState = showSkyNode;
            Properties.Settings.Default.ShowSkyGrids.TargetState = showSkyGrids;
            Properties.Settings.Default.ShowSkyOverlaysIn3d.TargetState = skyOverlaysIn3d;

            Properties.Settings.Default.ConstellationFiguresFilter.Clone(constellationFiguresFilter);
            Properties.Settings.Default.ConstellationBoundariesFilter.Clone(constellationBoundariesFilter);
            Properties.Settings.Default.ConstellationNamesFilter.Clone(constellationNamesFilter);
            Properties.Settings.Default.ConstellationArtFilter.Clone(constellationArtFilter);
            Settings.ignoreChanges = false;
            LayerManager.ProcessingUpdate = false;
            Properties.Settings.Default.PulseMeForUpdate = !Properties.Settings.Default.PulseMeForUpdate;
        }

        #region ISettings Members

        public bool SolarSystemStars
        {
            get { return solarSystemStars; }
        }
        public bool SolarSystemMultiRes
        {
            get { return solarSystemMultiRes; }
        }
        public bool SolarSystemMilkyWay
        {
            get { return solarSystemMilkyWay; }
        }

        public bool SolarSystemCosmos
        {
            get { return solarSystemCosmos; }
        }

        public bool SolarSystemCMB
        {
            get { return solarSystemCMB; }
        }

        public bool SolarSystemMinorPlanets
        {
            get { return solarSystemMinorPlanets; }
        }

        public bool SolarSystemPlanets
        {
            get { return solarSystemPlanets; }
        }  
        
        public bool SolarSystemOrbits
        {
            get { return solarSystemOrbits; }
        }
        public bool SolarSystemMinorOrbits
        {
            get { return solarSystemMinorOrbits; }
        }

        public bool ShowEquatorialGridText
        {
            get { return showEquatorialGridText; }
        }

        public bool ShowGalacticGrid
        {
            get { return showGalacticGrid; }
        }

        public bool ShowGalacticGridText
        {
            get { return showGalacticGridText; }
        }

        public bool ShowEclipticGrid
        {
            get { return showEclipticGrid; }
        }

        public bool ShowEclipticGridText
        {
            get { return showEclipticGridText; }
        }

        public bool ShowEclipticOverviewText
        {
            get { return showEclipticOverviewText; }
        }

        public bool ShowAltAzGrid
        {
            get { return showAltAzGrid; }
        }

        public bool ShowAltAzGridText
        {
            get { return showAltAzGridText; }
        }

        public bool ShowPrecessionChart
        {
            get { return showPrecessionChart; }
        }

        public bool ShowConstellationPictures
        {
            get { return showConstellationPictures; }
        }

        public string ConstellationsEnabled
        {
            get { return constellationsEnabled; }
        }

        public bool ShowConstellationLabels
        {
            get { return showConstellationLabels; }
        }




        public bool ShowSkyOverlays
        {
            get { return showSkyOverlays; }
        }

        public bool ShowConstellations
        {
            get { return showConstellations; }
        }

        public bool ShowSkyNode
        {
            get { return showSkyNode; }
        }

        public bool ShowSkyGrids
        {
            get { return showSkyGrids; }
        }

        public bool ShowSkyOverlaysIn3d
        {
            get { return skyOverlaysIn3d; }
        }

        public bool SolarSystemOverlays
        {
            get { return solarSystemOverlays; }
        }

        public bool SolarSystemLighting
        {
            get { return solarSystemLighting; }
        }

        public bool ShowISSModel
        {
            get { return showISSModel; }
        }

        public int SolarSystemScale
        {
            get { return solarSystemScale; }
        }


        public int MinorPlanetsFilter
        {
            get
            {
                return minorPlanetsFilter;
            }
        }

        public int PlanetOrbitsFilter
        {
            get
            {
                return planetOrbitsFilter;
            }
        }


        public bool ActualPlanetScale
        {
            get { return actualPlanetScale; }
        }

        public int FovCamera
        {
            get { return fovCamera; }
        }

        public int FovEyepiece
        {
            get { return fovEyepiece; }
        }

        public int FovTelescope
        {
            get { return fovTelescope; }
        }

        public double LocationAltitude
        {
            get
            {
                if (hasTime)
                {
                    return locationAltitude;
                }
                else
                {
                    return Properties.Settings.Default.LocationAltitude;
                }
            }
        }

        public double LocationLat
        {
            get
            {
                if (hasTime)
                {
                    return locationLat;
                }
                else
                {
                    return Properties.Settings.Default.LocationLat;
                }
            }
        }

        public double LocationLng
        {
            get
            {
                if (hasTime)
                {
                    return locationLng;
                }
                else
                {
                    return Properties.Settings.Default.LocationLng;
                }
            }
        }

        public bool ShowClouds
        {
            get
            {
                return showClouds;
            }
        }

        public bool EarthCutawayView
        {
            get
            {
                return earthCutawayView;
            }
        }
        public bool ShowConstellationBoundries
        {
            get
            {
                return showConstellationBoundries;
            }
        }

        public bool ShowConstellationFigures
        {
            get { return showConstellationFigures; }
        }

        public bool ShowConstellationSelection
        {
            get { return showConstellationSelection; }
        }

        public bool ShowEcliptic
        {
            get { return showEcliptic; }
        }

        public bool ShowElevationModel
        {
            get { return showElevationModel; }
        }

        public bool ShowFieldOfView
        {
            get { return showFieldOfView; }
        }

        public bool ShowGrid
        {
            get { return showGrid; }
        }

        public bool ShowHorizon
        {
            get { return showHorizon; }
        }

        public bool ShowHorizonPanorama
        {
            get { return showHorizonPanorama; }
        }

        public bool ShowMoonsAsPointSource
        {
            get { return showMoonsAsPointSource; }
        }

        public bool ShowSolarSystem
        {
            get { return showSolarSystem; }
        }

        public bool LocalHorizonMode
        {
            get { return localHorizonMode; }
        }

        public bool GalacticMode
        {
            get { return galacticMode; }
        }

        //public bool MilkyWayModel
        //{
        //    get { return milkyWayModel; }
        //}

        public bool ShowEarthSky
        {
            get { return showEarthSky; }
        }

        #endregion
        // End Settings
        string thumbnailString = "";
        Bitmap thumbnail = null;

        public Bitmap Thumbnail
        {
            get
            {
                if (target != null && thumbnail == null)
                {
                    return target.ThumbNail;
                }
                return thumbnail;
            }
            set
            {
                thumbnail = value;
                if (owner != null) { owner.TourDirty = true; }
            }
        }

        public Dictionary<Guid,LayerInfo> Layers = new Dictionary<Guid,LayerInfo>();


        List<Overlay> overlays = new List<Overlay>();

        public List<Overlay> Overlays
        {
            get { return overlays; }
        }

        AudioOverlay musicTrack = null;

        public AudioOverlay MusicTrack
        {
            get { return musicTrack; }
            set
            {
                if (musicTrack != value)
                {
                    musicTrack = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }
        AudioOverlay voiceTrack = null;

        public AudioOverlay VoiceTrack
        {
            get { return voiceTrack; }
            set
            {
                if (voiceTrack != value)
                {
                    voiceTrack = value;
                    if (owner != null) { owner.TourDirty = true; }
                }
            }
        }

        public void AddOverlay(Overlay overlay)
        {
            overlay.Owner = this;
            overlays.Add(overlay);

            if (overlay.AnimationTarget != null && !AnimationTargets.Contains(overlay.AnimationTarget))
            {
                AnimationTargets.Add(overlay.AnimationTarget);
            }

            if (owner != null) { owner.TourDirty = true; }

        }

        public void RemoveOverlay(Overlay overlay)
        {
            //todo clean up temp disk
            overlays.Remove(overlay);

            if (AnimationTargets != null && overlay.AnimationTarget != null)
            {
                if (AnimationTargets.Contains(overlay.AnimationTarget))
                {
                    AnimationTargets.Remove(overlay.AnimationTarget);
                    TimeLine.RefreshUi();
                }
            }


            if (owner != null) { owner.TourDirty = true; }
        }

        public void CleanUp()
        {
            foreach (Overlay overlay in Overlays)
            {
                overlay.CleanUp();
            }

            if (voiceTrack != null)
            {
                voiceTrack.CleanUp();
            }

            if (musicTrack != null)
            {
                musicTrack.CleanUp();
            }
        }

        public void SendToBack(Overlay target)
        {
            overlays.Remove(target);
            overlays.Insert(0, target);
            if (owner != null) { owner.TourDirty = true; }
        }

        public void BringToFront(Overlay target)
        {
            overlays.Remove(target);
            overlays.Add(target);
            if (owner != null) { owner.TourDirty = true; }
        }

        public void BringForward(Overlay target)
        {
            int index = overlays.FindIndex(delegate(Overlay overlay) { return target == overlay; });
            if (index < overlays.Count - 1)
            {
                overlays.Remove(target);
                overlays.Insert(index + 1, target);
            }
            if (owner != null) { owner.TourDirty = true; }
        }

        public void SendBackward(Overlay target)
        {
            int index = overlays.FindIndex(delegate(Overlay overlay) { return target == overlay; });
            if (index > 0)
            {
                overlays.Remove(target);
                overlays.Insert(index - 1, target);
            }
            if (owner != null) { owner.TourDirty = true; }
        }

        public Overlay GetNextOverlay(Overlay current)
        {
            if (current == null)
            {
                if (overlays.Count > 0)
                {
                    return overlays[0];
                }
                else
                {
                    return null;
                }
            }

            int index = overlays.FindIndex(delegate(Overlay overlay) { return current == overlay; });
            if (index < overlays.Count - 1)
            {
                return overlays[index + 1];
            }
            else
            {
                return overlays[0];
            }
        }

        public Overlay GetPerviousOverlay(Overlay current)
        {
            if (current == null)
            {
                if (overlays.Count > 0)
                {
                    return overlays[0];
                }
                else
                {
                    return null;
                }
            }
            int index = overlays.FindIndex(delegate(Overlay overlay) { return current == overlay; });
            if (index > 0)
            {
                return overlays[index - 1];
            }
            else
            {
                return overlays[overlays.Count - 1];
            }
        }

        public Overlay GetOverlayById(string id)
        {
            return overlays.Find(delegate(Overlay overlay) { return id == overlay.Id; });

        }

 
        public string TourStopThumbnailFilename
        {
            get
            {
                return string.Format("{0}{1}.thumb.png", this.owner.WorkingDirectory, id);
            }
        }

        public static string GetXmlText(TourStop ts)
        {
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter textWriter = new System.IO.StringWriter(sb))
            {
                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(textWriter))
                {
                    writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");

                    ts.SaveToXml(writer, true);
                }
            }
            return sb.ToString();
        }

        internal void SaveToXml(System.Xml.XmlTextWriter xmlWriter, bool saveContent)
        {
            if (saveContent)
            {
                if (thumbnail != null)
                {
                    thumbnail.Save(TourStopThumbnailFilename, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            xmlWriter.WriteStartElement("TourStop");
            xmlWriter.WriteAttributeString("Id", id);
            xmlWriter.WriteAttributeString("Name", name);
            xmlWriter.WriteAttributeString("Description", description);
            xmlWriter.WriteAttributeString("Thumbnail", thumbnailString);
            xmlWriter.WriteAttributeString("Duration", duration.ToString());
            xmlWriter.WriteAttributeString("Master", masterSlide.ToString());
            xmlWriter.WriteAttributeString("TransitionType", transition.ToString());
            xmlWriter.WriteAttributeString("TransitionTime", transitionTime.ToString());
            xmlWriter.WriteAttributeString("TransitionOutTime", transitionOutTime.ToString());
            xmlWriter.WriteAttributeString("TransitionHoldTime", transitionHoldTime.ToString());
            xmlWriter.WriteAttributeString("NextSlide", nextSlide.ToString());
            xmlWriter.WriteAttributeString("InterpolationType", interpolationType.ToString());

            xmlWriter.WriteAttributeString("HasLocation", hasLocation.ToString());
            if (hasLocation)
            {
                xmlWriter.WriteAttributeString("LocationAltitude", locationAltitude.ToString());
                xmlWriter.WriteAttributeString("LocationLat", locationLat.ToString());
                xmlWriter.WriteAttributeString("LocationLng", locationLng.ToString());
            }
            xmlWriter.WriteAttributeString("HasTime", hasTime.ToString());
            if (hasTime)
            {
                xmlWriter.WriteAttributeString("StartTime", startTime.ToString());
                xmlWriter.WriteAttributeString("EndTime", endTime.ToString());
            }
            xmlWriter.WriteAttributeString("ActualPlanetScale", actualPlanetScale.ToString());
            xmlWriter.WriteAttributeString("ShowClouds", showClouds.ToString());
            xmlWriter.WriteAttributeString("EarthCutawayView", earthCutawayView.ToString());
            xmlWriter.WriteAttributeString("ShowConstellationBoundries", showConstellationBoundries.ToString());
            xmlWriter.WriteAttributeString("ShowConstellationFigures", showConstellationFigures.ToString());
            xmlWriter.WriteAttributeString("ShowConstellationSelection", showConstellationSelection.ToString());
            xmlWriter.WriteAttributeString("ShowEcliptic", showEcliptic.ToString());
            xmlWriter.WriteAttributeString("ShowElevationModel", showElevationModel.ToString());
            showFieldOfView = false;
            xmlWriter.WriteAttributeString("ShowFieldOfView", showFieldOfView.ToString());
            xmlWriter.WriteAttributeString("ShowGrid", showGrid.ToString());
            xmlWriter.WriteAttributeString("ShowHorizon", showHorizon.ToString());
            xmlWriter.WriteAttributeString("ShowHorizonPanorama", showHorizonPanorama.ToString());
            xmlWriter.WriteAttributeString("ShowMoonsAsPointSource", showMoonsAsPointSource.ToString());
            xmlWriter.WriteAttributeString("ShowSolarSystem", showSolarSystem.ToString());
            xmlWriter.WriteAttributeString("FovTelescope", fovTelescope.ToString());
            xmlWriter.WriteAttributeString("FovEyepiece", fovEyepiece.ToString());
            xmlWriter.WriteAttributeString("FovCamera", fovCamera.ToString());
            xmlWriter.WriteAttributeString("LocalHorizonMode", localHorizonMode.ToString());
            //xmlWriter.WriteAttributeString("MilkyWayModel", milkyWayModel.ToString());
            xmlWriter.WriteAttributeString("GalacticMode", galacticMode.ToString());
            xmlWriter.WriteAttributeString("FadeInOverlays", fadeInOverlays.ToString());
            xmlWriter.WriteAttributeString("SolarSystemStars", solarSystemStars.ToString());
            xmlWriter.WriteAttributeString("SolarSystemMilkyWay", solarSystemMilkyWay.ToString());
            xmlWriter.WriteAttributeString("SolarSystemCosmos", solarSystemCosmos.ToString());
            xmlWriter.WriteAttributeString("SolarSystemCMB", solarSystemCMB.ToString());
            xmlWriter.WriteAttributeString("SolarSystemOrbits", solarSystemOrbits.ToString());
            xmlWriter.WriteAttributeString("SolarSystemMinorOrbits", solarSystemMinorOrbits.ToString());
            xmlWriter.WriteAttributeString("SolarSystemOverlays", solarSystemOverlays.ToString());
            xmlWriter.WriteAttributeString("SolarSystemLighting", solarSystemLighting.ToString());
            xmlWriter.WriteAttributeString("ShowISSModel", showISSModel.ToString());
            xmlWriter.WriteAttributeString("SolarSystemScale", solarSystemScale.ToString());
            xmlWriter.WriteAttributeString("MinorPlanetsFilter", minorPlanetsFilter.ToString());
            xmlWriter.WriteAttributeString("PlanetOrbitsFilter", planetOrbitsFilter.ToString());

            xmlWriter.WriteAttributeString("SolarSystemMultiRes", solarSystemMultiRes.ToString());
            xmlWriter.WriteAttributeString("SolarSystemMinorPlanets", solarSystemMinorPlanets.ToString());
            xmlWriter.WriteAttributeString("SolarSystemPlanets", solarSystemPlanets.ToString());
            xmlWriter.WriteAttributeString("ShowEarthSky", showEarthSky.ToString());

            xmlWriter.WriteAttributeString("ShowEquatorialGridText", ShowEquatorialGridText.ToString());
            xmlWriter.WriteAttributeString("ShowGalacticGrid", ShowGalacticGrid.ToString());
            xmlWriter.WriteAttributeString("ShowGalacticGridText", ShowGalacticGridText.ToString());
            xmlWriter.WriteAttributeString("ShowEclipticGrid", ShowEclipticGrid.ToString());
            xmlWriter.WriteAttributeString("ShowEclipticGridText", ShowEclipticGridText.ToString());
            xmlWriter.WriteAttributeString("ShowEclipticOverviewText", ShowEclipticOverviewText.ToString());
            xmlWriter.WriteAttributeString("ShowAltAzGrid", ShowAltAzGrid.ToString());
            xmlWriter.WriteAttributeString("ShowAltAzGridText", ShowAltAzGridText.ToString());
            xmlWriter.WriteAttributeString("ShowPrecessionChart", ShowPrecessionChart.ToString());
            xmlWriter.WriteAttributeString("ConstellationPictures", ShowConstellationPictures.ToString());
            xmlWriter.WriteAttributeString("ConstellationsEnabled", ConstellationsEnabled);
            xmlWriter.WriteAttributeString("ShowConstellationLabels", ShowConstellationLabels.ToString());
            xmlWriter.WriteAttributeString("ShowSkyOverlays", ShowSkyOverlays.ToString());
            xmlWriter.WriteAttributeString("ShowConstellations", ShowConstellations.ToString());
            xmlWriter.WriteAttributeString("ShowSkyNode", ShowSkyNode.ToString());
            xmlWriter.WriteAttributeString("ShowSkyGrids", ShowSkyGrids.ToString());
            xmlWriter.WriteAttributeString("SkyOverlaysIn3d", ShowSkyOverlaysIn3d.ToString());
            xmlWriter.WriteAttributeString("ConstellationFiguresFilter", constellationFiguresFilter.ToString());
            xmlWriter.WriteAttributeString("ConstellationBoundariesFilter", constellationBoundariesFilter.ToString());
            xmlWriter.WriteAttributeString("ConstellationNamesFilter", constellationNamesFilter.ToString());
            xmlWriter.WriteAttributeString("ConstellationArtFilter", constellationArtFilter.ToString());
          


            target.SaveToXml(xmlWriter, "Place");
            if (endTarget != null)
            {
                endTarget.SaveToXml(xmlWriter, "EndTarget");
            }

            xmlWriter.WriteStartElement("Overlays");

            foreach (Overlay overlay in overlays)
            {
                overlay.SaveToXml(xmlWriter, false);
            }
            xmlWriter.WriteEndElement();

            if (musicTrack != null)
            {
                xmlWriter.WriteStartElement("MusicTrack");

                musicTrack.SaveToXml(xmlWriter, false);

                xmlWriter.WriteEndElement();
            }

            if (voiceTrack != null)
            {
                xmlWriter.WriteStartElement("VoiceTrack");

                voiceTrack.SaveToXml(xmlWriter, false);

                xmlWriter.WriteEndElement();
            }

            //xmlWriter.WriteElementString("Credits", Credits);
            WriteLayerList(xmlWriter);

            if (KeyFramed)
            {
                xmlWriter.WriteStartElement("AnimationTargets");
                foreach (AnimationTarget aniTarget in AnimationTargets)
                {
                    aniTarget.SaveToXml(xmlWriter);
                }
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }

        private void WriteLayerList(XmlTextWriter xmlWriter)
        {
            if (Layers.Count > 0)
            {
                xmlWriter.WriteStartElement("VisibleLayers");

                foreach (LayerInfo info in Layers.Values)
                {
                    xmlWriter.WriteStartElement("Layer");
                    xmlWriter.WriteAttributeString("StartOpacity", info.StartOpacity.ToString());
                    xmlWriter.WriteAttributeString("EndOpacity", info.EndOpacity.ToString());
                    int len = info.StartParams.Length;

                    xmlWriter.WriteAttributeString("ParamCount", len.ToString());
                    for (int i = 0; i < len; i++)
                    {
                        xmlWriter.WriteAttributeString(string.Format("StartParam{0}",i), info.StartParams[i].ToString());
                        xmlWriter.WriteAttributeString(string.Format("EndParam{0}", i), info.EndParams[i].ToString());
                    }
                    xmlWriter.WriteValue(info.ID.ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
            }
        }

        

        internal void AddFilesToCabinet(FileCabinet fc, bool excludeAudio)
        {
            if (thumbnail != null)
            {
                fc.AddFile(string.Format("{0}{1}.thumb.png", this.owner.WorkingDirectory, id));
            }

            if (!excludeAudio)
            {
                if (musicTrack != null)
                {
                    musicTrack.AddFilesToCabinet(fc);
                }

                if (voiceTrack != null)
                {
                    voiceTrack.AddFilesToCabinet(fc);
                }
            }

            // TODO Add files that were loaded for display..

            foreach (Overlay overlay in overlays)
            {
                overlay.AddFilesToCabinet(fc);
            }

        }

        internal static TourStop FromXml(TourDocument owner, System.Xml.XmlNode tourStop)
        {

            TourStop newTourStop = new TourStop();
            newTourStop.owner = owner;

            newTourStop.Id = tourStop.Attributes["Id"].Value.ToString();
            newTourStop.Name = tourStop.Attributes["Name"].Value.ToString();
            newTourStop.Description = tourStop.Attributes["Description"].Value.ToString();
            newTourStop.thumbnailString = tourStop.Attributes["Thumbnail"].Value.ToString();
            newTourStop.duration = TimeSpan.Parse(tourStop.Attributes["Duration"].Value.ToString());
            if (tourStop.Attributes["Master"] != null)
            {
                newTourStop.masterSlide = Convert.ToBoolean(tourStop.Attributes["Master"].Value);
            }

            if (tourStop.Attributes["NextSlide"] != null)
            {
                newTourStop.nextSlide = tourStop.Attributes["NextSlide"].Value;
            }

            if (tourStop.Attributes["InterpolationType"] != null)
            {
                newTourStop.interpolationType = (InterpolationType)Enum.Parse(typeof(InterpolationType), tourStop.Attributes["InterpolationType"].Value);
            }


            newTourStop.fadeInOverlays = true;

            if (tourStop.Attributes["FadeInOverlays"] != null)
            {
                newTourStop.fadeInOverlays = Convert.ToBoolean(tourStop.Attributes["FadeInOverlays"].Value);
            }

            if (tourStop.Attributes["TransitionType"] != null)
            {
                newTourStop.transition = (TransitionType)Enum.Parse(typeof(TransitionType), tourStop.Attributes["TransitionType"].Value);
            }

            if (tourStop.Attributes["TransitionOutTime"] != null)
            {
                newTourStop.TransitionOutTime = double.Parse(tourStop.Attributes["TransitionOutTime"].Value);
            }

            if (tourStop.Attributes["TransitionHoldTime"] != null)
            {
                newTourStop.TransitionHoldTime = double.Parse(tourStop.Attributes["TransitionHoldTime"].Value);
            }

            if (tourStop.Attributes["TransitionTime"] != null)
            {
                newTourStop.TransitionTime = double.Parse(tourStop.Attributes["TransitionTime"].Value);
            }


            if (tourStop.Attributes["HasLocation"] != null)
            {
                newTourStop.hasLocation = Convert.ToBoolean(tourStop.Attributes["HasLocation"].Value);
            }



            if (newTourStop.hasLocation)
            {
                if (tourStop.Attributes["LocationAltitude"] != null)
                {
                    newTourStop.locationAltitude = Convert.ToDouble(tourStop.Attributes["LocationAltitude"].Value);
                }
                if (tourStop.Attributes["LocationLat"] != null)
                {
                    newTourStop.locationLat = Convert.ToDouble(tourStop.Attributes["LocationLat"].Value);
                }
                if (tourStop.Attributes["LocationLng"] != null)
                {
                    newTourStop.locationLng = Convert.ToDouble(tourStop.Attributes["LocationLng"].Value);
                }
            }

            if (tourStop.Attributes["HasTime"] != null)
            {
                newTourStop.hasTime = Convert.ToBoolean(tourStop.Attributes["HasTime"].Value);

                if (newTourStop.hasTime)
                {
                    if (tourStop.Attributes["StartTime"] != null)
                    {
                        newTourStop.startTime = DateTime.Parse(tourStop.Attributes["StartTime"].Value);
                    }
                    if (tourStop.Attributes["EndTime"] != null)
                    {
                        newTourStop.endTime = DateTime.Parse(tourStop.Attributes["EndTime"].Value);
                    }
                }
            }


            if (tourStop.Attributes["ActualPlanetScale"] != null)
            {
                newTourStop.actualPlanetScale = Convert.ToBoolean(tourStop.Attributes["ActualPlanetScale"].Value);
            }

            if (tourStop.Attributes["ShowClouds"] != null)
            {
                newTourStop.showClouds = Convert.ToBoolean(tourStop.Attributes["ShowClouds"].Value);
            }

            if (tourStop.Attributes["EarthCutawayView"] != null)
            {
                newTourStop.earthCutawayView = Convert.ToBoolean(tourStop.Attributes["EarthCutawayView"].Value);
            }

            if (tourStop.Attributes["ShowConstellationBoundries"] != null)
            {
                newTourStop.showConstellationBoundries = Convert.ToBoolean(tourStop.Attributes["ShowConstellationBoundries"].Value);
            }

            if (tourStop.Attributes["ShowConstellationFigures"] != null)
            {
                newTourStop.showConstellationFigures = Convert.ToBoolean(tourStop.Attributes["ShowConstellationFigures"].Value);
            }

            if (tourStop.Attributes["ShowConstellationSelection"] != null)
            {
                newTourStop.showConstellationSelection = Convert.ToBoolean(tourStop.Attributes["ShowConstellationSelection"].Value);
            }

            if (tourStop.Attributes["ShowEcliptic"] != null)
            {
                newTourStop.showEcliptic = Convert.ToBoolean(tourStop.Attributes["ShowEcliptic"].Value);
            }

            if (tourStop.Attributes["ShowElevationModel"] != null)
            {
                newTourStop.showElevationModel = Convert.ToBoolean(tourStop.Attributes["ShowElevationModel"].Value);
            }

            if (tourStop.Attributes["ShowFieldOfView"] != null)
            {
                newTourStop.showFieldOfView = Convert.ToBoolean(tourStop.Attributes["ShowFieldOfView"].Value);
            }

            if (tourStop.Attributes["ShowGrid"] != null)
            {
                newTourStop.showGrid = Convert.ToBoolean(tourStop.Attributes["ShowGrid"].Value);
            }

            if (tourStop.Attributes["ShowHorizon"] != null)
            {
                newTourStop.showHorizon = Convert.ToBoolean(tourStop.Attributes["ShowHorizon"].Value);
            }


            if (tourStop.Attributes["ShowHorizonPanorama"] != null)
            {
                newTourStop.showHorizonPanorama = Convert.ToBoolean(tourStop.Attributes["ShowHorizonPanorama"].Value);
            }

            if (tourStop.Attributes["ShowMoonsAsPointSource"] != null)
            {
                newTourStop.showMoonsAsPointSource = Convert.ToBoolean(tourStop.Attributes["ShowMoonsAsPointSource"].Value);
            }

            if (tourStop.Attributes["ShowSolarSystem"] != null)
            {
                newTourStop.showSolarSystem = Convert.ToBoolean(tourStop.Attributes["ShowSolarSystem"].Value);
            }

            if (tourStop.Attributes["FovTelescope"] != null)
            {
                newTourStop.fovTelescope = Convert.ToInt32(tourStop.Attributes["FovTelescope"].Value);
            }

            if (tourStop.Attributes["FovEyepiece"] != null)
            {
                newTourStop.fovEyepiece = Convert.ToInt32(tourStop.Attributes["FovEyepiece"].Value);
            }

            if (tourStop.Attributes["FovCamera"] != null)
            {
                newTourStop.fovCamera = Convert.ToInt32(tourStop.Attributes["FovCamera"].Value);
            }

            if (tourStop.Attributes["LocalHorizonMode"] != null)
            {
                newTourStop.localHorizonMode = Convert.ToBoolean(tourStop.Attributes["LocalHorizonMode"].Value);
            }
         

            if (tourStop.Attributes["GalacticMode"] != null)
            {
                newTourStop.galacticMode = Convert.ToBoolean(tourStop.Attributes["GalacticMode"].Value);
            }  
            else
            {
                newTourStop.galacticMode = false;
            }

            //if (tourStop.Attributes["MilkyWayModel"] != null)
            //{
            //    newTourStop.milkyWayModel = Convert.ToBoolean(tourStop.Attributes["MilkyWayModel"].Value);
            //}
            //else
            //{
            //    newTourStop.milkyWayModel = true;
            //}

            if (tourStop.Attributes["SolarSystemStars"] != null)
            {
                newTourStop.solarSystemStars = Convert.ToBoolean(tourStop.Attributes["SolarSystemStars"].Value);
            }

            if (tourStop.Attributes["SolarSystemMilkyWay"] != null)
            {
                newTourStop.solarSystemMilkyWay = Convert.ToBoolean(tourStop.Attributes["SolarSystemMilkyWay"].Value);
            }

            if (tourStop.Attributes["SolarSystemCosmos"] != null)
            {
                newTourStop.solarSystemCosmos = Convert.ToBoolean(tourStop.Attributes["SolarSystemCosmos"].Value);
            }

            if (tourStop.Attributes["SolarSystemCMB"] != null)
            {
                newTourStop.solarSystemCMB = Convert.ToBoolean(tourStop.Attributes["SolarSystemCMB"].Value);
            }

            if (tourStop.Attributes["SolarSystemOrbits"] != null)
            {
                newTourStop.solarSystemOrbits = Convert.ToBoolean(tourStop.Attributes["SolarSystemOrbits"].Value);
            }

            if (tourStop.Attributes["SolarSystemMinorOrbits"] != null)
            {
                newTourStop.solarSystemMinorOrbits = Convert.ToBoolean(tourStop.Attributes["SolarSystemMinorOrbits"].Value);
            }
            else
            {
                newTourStop.solarSystemMinorOrbits = false;
            }

            if (tourStop.Attributes["SolarSystemMinorPlanets"] != null)
            {
                newTourStop.solarSystemMinorPlanets = Convert.ToBoolean(tourStop.Attributes["SolarSystemMinorPlanets"].Value);
            }
            else
            {
                newTourStop.solarSystemMinorPlanets = false;
            }

            if (tourStop.Attributes["SolarSystemPlanets"] != null)
            {
                newTourStop.solarSystemPlanets = Convert.ToBoolean(tourStop.Attributes["SolarSystemPlanets"].Value);
            }
            else
            {
                newTourStop.solarSystemPlanets = true;
            }

            if (tourStop.Attributes["SolarSystemOverlays"] != null)
            {
                newTourStop.solarSystemOverlays = Convert.ToBoolean(tourStop.Attributes["SolarSystemOverlays"].Value);
            }

            if (tourStop.Attributes["SolarSystemLighting"] != null)
            {
                newTourStop.solarSystemLighting = Convert.ToBoolean(tourStop.Attributes["SolarSystemLighting"].Value);
            }

            if (tourStop.Attributes["ShowISSModel"] != null)
            {
                newTourStop.showISSModel = Convert.ToBoolean(tourStop.Attributes["ShowISSModel"].Value);
            }

            if (tourStop.Attributes["SolarSystemScale"] != null)
            {
                newTourStop.solarSystemScale = Convert.ToInt32(tourStop.Attributes["SolarSystemScale"].Value);
            }

            if (tourStop.Attributes["MinorPlanetsFilter"] != null)
            {
                newTourStop.minorPlanetsFilter = Convert.ToInt32(tourStop.Attributes["MinorPlanetsFilter"].Value);
            }
            else
            {
                newTourStop.minorPlanetsFilter = int.MaxValue;
            }

            if (tourStop.Attributes["PlanetOrbitsFilter"] != null)
            {
                newTourStop.planetOrbitsFilter = Convert.ToInt32(tourStop.Attributes["PlanetOrbitsFilter"].Value);
            }
            else
            {
                newTourStop.planetOrbitsFilter = int.MaxValue;
            }

            if (tourStop.Attributes["SolarSystemMultiRes"] != null)
            {
                newTourStop.solarSystemMultiRes = Convert.ToBoolean(tourStop.Attributes["SolarSystemMultiRes"].Value);
            }

            if (tourStop.Attributes["ShowEarthSky"] != null)
            {
                newTourStop.showEarthSky = Convert.ToBoolean(tourStop.Attributes["ShowEarthSky"].Value);
            }

            if (tourStop.Attributes["ShowEquatorialGridText"] != null)
            {
                newTourStop.showEquatorialGridText = Convert.ToBoolean(tourStop.Attributes["ShowEquatorialGridText"].Value);
            }
            else
            {
                newTourStop.showEquatorialGridText = false;
            }

            if (tourStop.Attributes["ShowGalacticGrid"] != null)
            {
                newTourStop.showGalacticGrid = Convert.ToBoolean(tourStop.Attributes["ShowGalacticGrid"].Value);
            }
            else
            {
                newTourStop.showGalacticGrid = false;
            }

            if (tourStop.Attributes["ShowGalacticGridText"] != null)
            {
                newTourStop.showGalacticGridText = Convert.ToBoolean(tourStop.Attributes["ShowGalacticGridText"].Value);
            }
            else
            {
                newTourStop.showGalacticGridText = false;
            }

            if (tourStop.Attributes["ShowEclipticGrid"] != null)
            {
                newTourStop.showEclipticGrid = Convert.ToBoolean(tourStop.Attributes["ShowEclipticGrid"].Value);
            }
            else
            {
                newTourStop.showEclipticGrid = false;
            }

            if (tourStop.Attributes["ShowEclipticGridText"] != null)
            {
                newTourStop.showEclipticGridText = Convert.ToBoolean(tourStop.Attributes["ShowEclipticGridText"].Value);
            }
            else
            {
                newTourStop.showEclipticGridText = false;
            }

            if (tourStop.Attributes["ShowEclipticOverviewText"] != null)
            {
                newTourStop.showEclipticOverviewText = Convert.ToBoolean(tourStop.Attributes["ShowEclipticOverviewText"].Value);
            }
            else
            {
                newTourStop.showEclipticOverviewText = false;
            }

            if (tourStop.Attributes["ShowAltAzGrid"] != null)
            {
                newTourStop.showAltAzGrid = Convert.ToBoolean(tourStop.Attributes["ShowAltAzGrid"].Value);
            }
            else
            {
                newTourStop.showAltAzGrid = false;
            }

            if (tourStop.Attributes["ShowAltAzGridText"] != null)
            {
                newTourStop.showAltAzGridText = Convert.ToBoolean(tourStop.Attributes["ShowAltAzGridText"].Value);
            }
            else
            {
                newTourStop.showAltAzGridText = false;
            }

            if (tourStop.Attributes["ShowPrecessionChart"] != null)
            {
                newTourStop.showPrecessionChart = Convert.ToBoolean(tourStop.Attributes["ShowPrecessionChart"].Value);
            }
            else
            {
                newTourStop.showPrecessionChart = false;
            }

            if (tourStop.Attributes["ConstellationPictures"] != null)
            {
                newTourStop.showConstellationPictures = Convert.ToBoolean(tourStop.Attributes["ConstellationPictures"].Value);
            }
            else
            {
                newTourStop.showConstellationPictures = false;
            }

            if (tourStop.Attributes["ShowConstellationLabels"] != null)
            {
                newTourStop.showConstellationLabels = Convert.ToBoolean(tourStop.Attributes["ShowConstellationLabels"].Value);
            }
            else
            {
                newTourStop.showConstellationLabels = false;
            }

            if (tourStop.Attributes["ConstellationsEnabled"] != null)
            {
                newTourStop.constellationsEnabled = tourStop.Attributes["ConstellationsEnabled"].Value;
            }
            else
            {
                newTourStop.constellationsEnabled = "";
            }

            if (tourStop.Attributes["ShowSkyOverlays"] != null)
            {
                newTourStop.showSkyOverlays = Convert.ToBoolean(tourStop.Attributes["ShowSkyOverlays"].Value);
            }
            else
            {
                newTourStop.showSkyOverlays = true;
            }

            if (tourStop.Attributes["ShowConstellations"] != null)
            {
                newTourStop.showConstellations = Convert.ToBoolean(tourStop.Attributes["ShowConstellations"].Value);
            }
            else
            {
                newTourStop.showConstellations = true;
            }

            if (tourStop.Attributes["ShowSkyNode"] != null)
            {
                newTourStop.showSkyNode = Convert.ToBoolean(tourStop.Attributes["ShowSkyNode"].Value);
            }
            else
            {
                newTourStop.showSkyNode = true;
            }

            if (tourStop.Attributes["ShowSkyGrids"] != null)
            {
                newTourStop.showSkyGrids = Convert.ToBoolean(tourStop.Attributes["ShowSkyGrids"].Value);
            }
            else
            {
                newTourStop.showSkyGrids = true;
            }

            if (tourStop.Attributes["SkyOverlaysIn3d"] != null)
            {
                newTourStop.skyOverlaysIn3d = Convert.ToBoolean(tourStop.Attributes["SkyOverlaysIn3d"].Value);
            }
            else
            {
                newTourStop.skyOverlaysIn3d = false;
            }

            if (tourStop.Attributes["ConstellationFiguresFilter"] != null)
            {
                newTourStop.constellationFiguresFilter = ConstellationFilter.Parse(tourStop.Attributes["ConstellationFiguresFilter"].Value);
            }
            else
            {
                newTourStop.constellationFiguresFilter = ConstellationFilter.AllConstellation;
            }

            if (tourStop.Attributes["ConstellationBoundariesFilter"] != null)
            {
                newTourStop.constellationBoundariesFilter = ConstellationFilter.Parse(tourStop.Attributes["ConstellationBoundariesFilter"].Value);
            }
            else
            {
                newTourStop.constellationBoundariesFilter = ConstellationFilter.AllConstellation;
            }

            if (tourStop.Attributes["ConstellationNamesFilter"] != null)
            {
                newTourStop.constellationNamesFilter = ConstellationFilter.Parse(tourStop.Attributes["ConstellationNamesFilter"].Value);
            }
            else
            {
                newTourStop.constellationNamesFilter = ConstellationFilter.AllConstellation;
            }

            if (tourStop.Attributes["ConstellationArtFilter"] != null)
            {
                newTourStop.constellationArtFilter = ConstellationFilter.Parse(tourStop.Attributes["ConstellationArtFilter"].Value);
            }
            else
            {
                newTourStop.constellationArtFilter = ConstellationFilter.AllConstellation;
            }

            XmlNode place = tourStop["Place"];

            newTourStop.target = TourPlace.FromXml(place);

            XmlNode endTarget = tourStop["EndTarget"];
            if (endTarget != null)
            {
                newTourStop.endTarget = TourPlace.FromXml(endTarget);
            }

            XmlNode overlays = tourStop["Overlays"];

            foreach (XmlNode overlay in overlays)
            {
                newTourStop.AddOverlay(Overlay.FromXml(newTourStop, overlay));
            }

            XmlNode musicNode = tourStop["MusicTrack"];

            if (musicNode != null)
            {
                newTourStop.musicTrack = (AudioOverlay)Overlay.FromXml(newTourStop, musicNode.FirstChild);
            }

            XmlNode voiceNode = tourStop["VoiceTrack"];

            if (voiceNode != null)
            {
                newTourStop.voiceTrack = (AudioOverlay)Overlay.FromXml(newTourStop, voiceNode.FirstChild);
            }
            XmlNode layerNode = tourStop["VisibleLayers"];
            if (layerNode != null)
            {
                newTourStop.LoadLayerList(layerNode);
            }

            XmlNode animationTargetsNode = tourStop["AnimationTargets"];
            if (animationTargetsNode != null)
            {
                newTourStop.LoadAnimationTargets(animationTargetsNode);
            }


            newTourStop.thumbnail = UiTools.LoadBitmap(string.Format("{0}{1}.thumb.png", newTourStop.owner.WorkingDirectory, newTourStop.id));

            return newTourStop;
        }

        private void LoadAnimationTargets(XmlNode animationTargetsNode)
        {
            keyFramed = true;

            foreach (XmlNode child in animationTargetsNode.ChildNodes)
            {
                AnimationTarget aniTarget = new AnimationTarget(this);
                aniTarget.FromXml(child);
                AnimationTargets.Add(aniTarget);

                switch (aniTarget.TargetType)
                {
                    case AnimationTarget.AnimationTargetTypes.Overlay:
                        aniTarget.Target = GetOverlayById(aniTarget.TargetID);
                        ((Overlay)aniTarget.Target).AnimationTarget = aniTarget;
                        break;
                    case AnimationTarget.AnimationTargetTypes.Layer:
                         break;
                    case AnimationTarget.AnimationTargetTypes.Setting:
                        break;
                    case AnimationTarget.AnimationTargetTypes.Camera:
                        aniTarget.Target = this.KeyFrameMover;
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadLayerList(XmlNode layersNode)
        {
            foreach (XmlNode layer in layersNode)
            {
                LayerInfo info = new LayerInfo();
                info.ID = new Guid(layer.InnerText);
                info.StartOpacity = Convert.ToSingle(layer.Attributes["StartOpacity"].Value);
                info.EndOpacity = Convert.ToSingle(layer.Attributes["EndOpacity"].Value);

                int len = 0;
                if (layer.Attributes["ParamCount"] != null)
                {
                    len = Convert.ToInt32(layer.Attributes["ParamCount"].Value);
                }
                info.StartParams = new double[len];
                info.EndParams = new double[len];
                info.FrameParams = new double[len];

                for (int i = 0; i < len; i++)
                {
                    info.StartParams[i] = double.Parse(layer.Attributes[string.Format("StartParam{0}", i)].Value);
                    info.EndParams[i] = double.Parse(layer.Attributes[string.Format("EndParam{0}", i)].Value);
                    info.FrameParams[i] = info.StartParams[i];
                }

                Layers.Add(info.ID,info);
            }
        }

        public string GetNextDefaultName(string baseName)
        {
            int suffixId = 1;
            foreach (Overlay overlay in overlays)
            {
                if (overlay.Name.StartsWith(baseName))
                {
                    int id = 0;
                    try
                    {
                        id = Convert.ToInt32(overlay.Name.Substring(baseName.Length));
                    }
                    catch
                    {
                    }

                    if (id >= suffixId)
                    {
                        suffixId = id + 1;
                    }
                }
            }

            return string.Format("{0} {1}", baseName, suffixId);
        }



        internal void UpdateLayerOpacity()
        {
            if (!KeyFramed)
            {
                foreach (LayerInfo info in Layers.Values)
                {
                    info.FrameOpacity = info.StartOpacity * (1 - tweenPosition) + info.EndOpacity * tweenPosition;
                    int len = info.StartParams.Length;
                    info.FrameParams = new double[len];
                    for (int i = 0; i < len; i++)
                    {
                        info.FrameParams[i] = info.StartParams[i] * (1 - tweenPosition) + info.EndParams[i] * tweenPosition;
                    }

                }
            }
            else
            {
                UpdateTweenPosition();
            }
        }

       

        public ConstellationFilter ConstellationFiguresFilter
        {
            get
            {
                SettingParameter sp = GetSetting(StockSkyOverlayTypes.ConstellationFigures);
                return sp.Filter;
            }
        }

        

        public ConstellationFilter ConstellationBoundariesFilter
        {
            get
            {
                SettingParameter sp = GetSetting(StockSkyOverlayTypes.ConstellationBoundaries);
                return sp.Filter;
            }
        }

       
        public ConstellationFilter ConstellationNamesFilter
        {
            get
            {
                SettingParameter sp = GetSetting(StockSkyOverlayTypes.ConstellationNames);
                return sp.Filter;
            }
        }

        

        public ConstellationFilter ConstellationArtFilter
        {
            get
            {
                SettingParameter sp = GetSetting(StockSkyOverlayTypes.ConstellationPictures);
                return sp.Filter;

               // return constellationArtFilter;
            }
        }




        internal void ExtendTimeline(TimeSpan oldDuration, TimeSpan newDuration)
        {
            Duration = newDuration;
            foreach (AnimationTarget at in AnimationTargets)
            {
                at.ExtendTimeline(oldDuration, newDuration);
            }
        }
    }

    public class LayerInfo
    {
        public Guid ID = Guid.Empty;
        public float StartOpacity=1;
        public float EndOpacity=1;
        public float FrameOpacity=1;
        public double[] StartParams = new double[0];
        public double[] EndParams = new double[0];
        public double[] FrameParams = new double[0];
    }

    public class UndoTourStopChange : TerraViewer.IUndoStep
    {
        string undoXml = "";
        string redoXml = "";
        int currentIndex = 0;
        string actionText = "";

        public string ActionText
        {
            get { return actionText; }
            set { actionText = value; }
        }
        TourDocument targetTour = null;
        public UndoTourStopChange(string text, TourDocument tour)
        {
            currentIndex = tour.CurrentTourstopIndex;
            actionText = text;
            targetTour = tour;
            undoXml = TourStop.GetXmlText(tour.CurrentTourStop);
            targetTour.TourDirty = true;

        }

        public void Undo()
        {
            TourStop tsRedo = targetTour.TourStops[currentIndex];
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(undoXml);
            System.Xml.XmlNode node = doc["TourStop"];
            targetTour.TourStops[currentIndex] = TourStop.FromXml(targetTour, node);

            targetTour.CurrentTourstopIndex = currentIndex;

            // Setup redo
            if (string.IsNullOrEmpty(redoXml))
            {
                redoXml = TourStop.GetXmlText(tsRedo);
            }
            targetTour.TourDirty = true;

        }

        public void Redo()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

            doc.LoadXml(redoXml);
            System.Xml.XmlNode node = doc["TourStop"];
            targetTour.TourStops[currentIndex] = TourStop.FromXml(targetTour, node);

            targetTour.CurrentTourstopIndex = currentIndex;
            targetTour.TourDirty = true;
        }

        override public string ToString()
        {
            return actionText;
        }
    }
}
