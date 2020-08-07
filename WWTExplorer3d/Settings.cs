using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection;

namespace TerraViewer
{
    public class Settings : TerraViewer.ISettings, IScriptable
    {
        static public Dictionary<string, ScriptableProperty> SettingsList;

        static Settings()
        {
            // These are the settings allowed access thru the LCAPI
            SettingsList = new Dictionary<string, ScriptableProperty>();
            SettingsList.Add("AutoHideContext", new ScriptableProperty("AutoHideContext", ScriptablePropertyTypes.Bool));
            SettingsList.Add("AutoHideTabs", new ScriptableProperty("AutoHideTabs", ScriptablePropertyTypes.Bool));
            SettingsList.Add("AutoRepeatTour", new ScriptableProperty("AutoRepeatTour", ScriptablePropertyTypes.Bool));
            SettingsList.Add("AutoRepeatTourAll", new ScriptableProperty("AutoRepeatTourAll", ScriptablePropertyTypes.Bool));
            SettingsList.Add("ConstellationBoundryColor", new ScriptableProperty("ConstellationBoundryColor", ScriptablePropertyTypes.Color));
            SettingsList.Add("ConstellationFigureColor", new ScriptableProperty("ConstellationFigureColor", ScriptablePropertyTypes.Color));
            SettingsList.Add("Constellations", new ScriptableProperty("Constellations", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ConstellationSelectionColor", new ScriptableProperty("ConstellationSelectionColor", ScriptablePropertyTypes.Color));
            SettingsList.Add("ConstellationsEnabled", new ScriptableProperty("ConstellationsEnabled", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("DomeTilt", new ScriptableProperty("DomeTilt", ScriptablePropertyTypes.Float, ScriptablePropertyScale.Linear, -90, +180, false));
            SettingsList.Add("DomeView", new ScriptableProperty("DomeView", ScriptablePropertyTypes.Bool));
            SettingsList.Add("EarthCutawayView", new ScriptableProperty("EarthCutawayView", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("EclipticColor", new ScriptableProperty("EclipticColor", ScriptablePropertyTypes.Color));
            SettingsList.Add("FollowMouseOnZoom", new ScriptableProperty("FollowMouseOnZoom", ScriptablePropertyTypes.Bool));
            SettingsList.Add("FovCamera", new ScriptableProperty("FovCamera", ScriptablePropertyTypes.Bool));
            SettingsList.Add("FovColor", new ScriptableProperty("FovColor", ScriptablePropertyTypes.Color));
            SettingsList.Add("FovEyepiece", new ScriptableProperty("FovEyepiece", ScriptablePropertyTypes.Integer));
            SettingsList.Add("FovTelescope", new ScriptableProperty("FovTelescope", ScriptablePropertyTypes.Integer));
            SettingsList.Add("FullScreenTours", new ScriptableProperty("FullScreenTours", ScriptablePropertyTypes.Bool));
            SettingsList.Add("GridColor", new ScriptableProperty("GridColor", ScriptablePropertyTypes.Color));
            SettingsList.Add("ImageQuality", new ScriptableProperty("ImageQuality", ScriptablePropertyTypes.Integer, ScriptablePropertyScale.Linear, 0, 100, false));
            SettingsList.Add("LargeDomeTextures", new ScriptableProperty("LargeDomeTextures", ScriptablePropertyTypes.Bool));
            SettingsList.Add("LineSmoothing", new ScriptableProperty("LineSmoothing", ScriptablePropertyTypes.Bool));
            SettingsList.Add("ListenMode", new ScriptableProperty("ListenMode", ScriptablePropertyTypes.Bool));
            SettingsList.Add("LocalHorizonMode", new ScriptableProperty("LocalHorizonMode", ScriptablePropertyTypes.Bool));
            SettingsList.Add("GalacticMode", new ScriptableProperty("GalacticMode", ScriptablePropertyTypes.Bool));
            SettingsList.Add("MilkyWayModel", new ScriptableProperty("MilkyWayModel", ScriptablePropertyTypes.Bool));
            SettingsList.Add("LocationAltitude", new ScriptableProperty("LocationAltitude", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, 0, 10000, false));
            SettingsList.Add("LocationLat", new ScriptableProperty("LocationLat", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -90, 90, true));
            SettingsList.Add("LocationLng", new ScriptableProperty("LocationLng", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -180, 180, false));
            SettingsList.Add("LocationName", new ScriptableProperty("LocationName", ScriptablePropertyTypes.String));
            SettingsList.Add("MasterController", new ScriptableProperty("MasterController", ScriptablePropertyTypes.Bool));
            SettingsList.Add("ShowAltAzGrid", new ScriptableProperty("ShowAltAzGrid", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowAltAzGridText", new ScriptableProperty("ShowAltAzGridText", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowClouds", new ScriptableProperty("ShowClouds", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowConstellationBoundries", new ScriptableProperty("ShowConstellationBoundries", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowConstellationFigures", new ScriptableProperty("ShowConstellationFigures", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowConstellationLabels", new ScriptableProperty("ShowConstellationLabels", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowConstellationNames", new ScriptableProperty("ShowConstellationNames", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowConstellationPictures", new ScriptableProperty("ShowConstellationPictures", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowConstellationSelection", new ScriptableProperty("ShowConstellationSelection", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowCrosshairs", new ScriptableProperty("ShowCrosshairs", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowDatasetNames", new ScriptableProperty("ShowDatasetNames", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowEarthSky", new ScriptableProperty("ShowEarthSky", ScriptablePropertyTypes.Bool));
            SettingsList.Add("ShowEcliptic", new ScriptableProperty("ShowEcliptic", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowEclipticGrid", new ScriptableProperty("ShowEclipticGrid", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowEclipticGridText", new ScriptableProperty("ShowEclipticGridText", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowEclipticOverviewText", new ScriptableProperty("ShowEclipticOverviewText", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowElevationModel", new ScriptableProperty("ShowElevationModel", ScriptablePropertyTypes.Bool));
            SettingsList.Add("ShowEquatorialGridText", new ScriptableProperty("ShowEquatorialGridText", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowFieldOfView", new ScriptableProperty("ShowFieldOfView", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowGalacticGrid", new ScriptableProperty("ShowGalacticGrid", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowGalacticGridText", new ScriptableProperty("ShowGalacticGridText", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowGrid", new ScriptableProperty("ShowGrid", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowISSModel", new ScriptableProperty("ShowISSModel", ScriptablePropertyTypes.Bool));
            SettingsList.Add("ShowPrecessionChart", new ScriptableProperty("ShowPrecessionChart", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowSkyGrids", new ScriptableProperty("ShowSkyGrids", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowSkyNode", new ScriptableProperty("ShowSkyNode", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowSkyOverlays", new ScriptableProperty("ShowSkyOverlays", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowSkyOverlaysIn3d", new ScriptableProperty("ShowSkyOverlaysIn3d", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowSolarSystem", new ScriptableProperty("ShowSolarSystem", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("ShowTouchControls", new ScriptableProperty("ShowTouchControls", ScriptablePropertyTypes.Bool));
            SettingsList.Add("ShowUTCTime", new ScriptableProperty("ShowUTCTime", ScriptablePropertyTypes.Bool));
            SettingsList.Add("SolarSystemCosmos", new ScriptableProperty("SolarSystemCosmos", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("SolarSystemCMB", new ScriptableProperty("SolarSystemCMB", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("SolarSystemLighting", new ScriptableProperty("SolarSystemLighting", ScriptablePropertyTypes.Bool));
            SettingsList.Add("SolarSystemMilkyWay", new ScriptableProperty("SolarSystemMilkyWay", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("SolarSystemMinorOrbits", new ScriptableProperty("SolarSystemMinorOrbits", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("SolarSystemMinorPlanets", new ScriptableProperty("SolarSystemMinorPlanets", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("SolarSystemMultiRes", new ScriptableProperty("SolarSystemMultiRes", ScriptablePropertyTypes.Bool));
            SettingsList.Add("SolarSystemOrbitColor", new ScriptableProperty("SolarSystemOrbitColor", ScriptablePropertyTypes.Color));
            SettingsList.Add("SolarSystemOrbits", new ScriptableProperty("SolarSystemOrbits", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("SolarSystemOverlays", new ScriptableProperty("SolarSystemOverlays", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("SolarSystemPlanets", new ScriptableProperty("SolarSystemPlanets", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("SolarSystemScale", new ScriptableProperty("SolarSystemScale", ScriptablePropertyTypes.Integer, ScriptablePropertyScale.Linear, 1, 100, false));
            SettingsList.Add("SolarSystemStars", new ScriptableProperty("SolarSystemStars", ScriptablePropertyTypes.BlendState));
            SettingsList.Add("StartUpLookAt", new ScriptableProperty("StartUpLookAt", ScriptablePropertyTypes.Enum));

            SettingsList.Add("ConstellationFiguresFilter", new ScriptableProperty("ConstellationFiguresFilter", ScriptablePropertyTypes.ConstellationFilter));
            SettingsList.Add("ConstellationBoundariesFilter", new ScriptableProperty("ConstellationBoundariesFilter", ScriptablePropertyTypes.ConstellationFilter));
            SettingsList.Add("ConstellationNamesFilter", new ScriptableProperty("ConstellationNamesFilter", ScriptablePropertyTypes.ConstellationFilter));
            SettingsList.Add("ConstellationArtFilter", new ScriptableProperty("ConstellationArtFilter", ScriptablePropertyTypes.ConstellationFilter));
            SettingsList.Add("PerspectiveOffsetX", new ScriptableProperty("PerspectiveOffsetX", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -1, +1, false));
            SettingsList.Add("PerspectiveOffsetY", new ScriptableProperty("PerspectiveOffsetY", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -1, +1, false));
            SettingsList.Add("OverSampleTerrain", new ScriptableProperty("OverSampleTerrain", ScriptablePropertyTypes.Bool));
            SettingsList.Add("SwingScaleFront", new ScriptableProperty("SwingScaleFront", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, 0, 10, false));
            SettingsList.Add("SwingScaleBack", new ScriptableProperty("SwingScaleBack", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, 0, 10, false));

        }

        static ISettings active = new Settings();

        public static ISettings Ambient
        {
            get
            {
                return active;
            }
        }

        public static ISettings Active
        {
            get
            {
                if (tourSettings != null)
                {
                    return tourSettings;
                }
                else
                {
                    return Settings.active;
                }

            }
        }

        static ISettings tourSettings = null;

        public static ISettings TourSettings
        {
            get { return Settings.tourSettings; }
            set
            {
                if (Settings.TourSettings != value)
                {
                    Settings.tourSettings = value;
                    Properties.Settings.Default.PulseMeForUpdate = !Properties.Settings.Default.PulseMeForUpdate;
                }



            }
        }
        static bool masterController = false;
        public static bool MasterController
        {
            get
            {
#if !WINDOWS_UWP
                return (Properties.Settings.Default.MasterController || masterController ) && !Earth3d.DomeViewer;
#else

                return false;
#endif
            }
            set
            {
                Properties.Settings.Default.MasterController = value;
                masterController = value;
            }
        }

        static bool domeView = false;
        public static bool DomeView
        {
            get
            {
#if !WINDOWS_UWP
                return Properties.Settings.Default.DomeView || domeView || Earth3d.DomeViewer;
#else
                return false;
#endif
            }
            set
            {
                domeView = value;

            }
        }

 
        public bool ShowEcliptic
        {
            get { return Properties.Settings.Default.ShowEcliptic.TargetState; }
            set { Properties.Settings.Default.ShowEcliptic.TargetState = value; }
        }



   
        public bool ActualPlanetScale
        {
            get
            {

                return Properties.Settings.Default.ActualPlanetScale; ;
            }
            set
            {
                Properties.Settings.Default.ActualPlanetScale = value;
            }
        }
 
        public double LocationAltitude
        {
            get { return Properties.Settings.Default.LocationAltitude; }
            set { Properties.Settings.Default.LocationAltitude = value; }
        }
 
        public double LocationLat
        {
            get { return Properties.Settings.Default.LocationLat; }
            set { Properties.Settings.Default.LocationLat = value; }
        }
        public double LocationLng
        {
            get { return Properties.Settings.Default.LocationLng; }
            set { Properties.Settings.Default.LocationLng = value; }
        }
   
        public bool ShowClouds
        {
            get { return Properties.Settings.Default.ShowClouds.TargetState; }
            set { Properties.Settings.Default.ShowClouds.TargetState = value; }
        }

        public bool EarthCutawayView
        {
            get { return Properties.Settings.Default.EarthCutawayView.TargetState; }
            set { Properties.Settings.Default.EarthCutawayView.TargetState = value; }
        }

        public bool ShowConstellationBoundries
        {
            get { return Properties.Settings.Default.ShowConstellationBoundries.TargetState; }
            set { Properties.Settings.Default.ShowConstellationBoundries.TargetState = value; }
        }

        public bool ShowConstellationFigures
        {
            get { return Properties.Settings.Default.ShowConstellationFigures.TargetState; }
            set { Properties.Settings.Default.ShowConstellationFigures.TargetState = value; }
        }

        public bool ShowConstellationSelection
        {
            get { return Properties.Settings.Default.ShowConstellationSelection.TargetState; }
            set { Properties.Settings.Default.ShowConstellationSelection.TargetState = value; }
        }

        public bool ShowElevationModel
        {
            get { return Properties.Settings.Default.ShowElevationModel; }
            set { Properties.Settings.Default.ShowElevationModel = value; } 
            //set { showElevationModel = value; }
        }
 
        public bool ShowFieldOfView
        {
            get { return Properties.Settings.Default.ShowFieldOfView.TargetState; }
           // set { showFieldOfView = value; }
            set { Properties.Settings.Default.ShowFieldOfView.TargetState = value; }
        }
 
        public bool ShowGrid
        {
            get { return Properties.Settings.Default.ShowGrid.TargetState; }
            //set { showGrid = value; }
            set { Properties.Settings.Default.ShowGrid.TargetState = value; }
        }
  
        public bool ShowHorizon
        {
            get { return Properties.Settings.Default.ShowHorizon; }
            set { Properties.Settings.Default.ShowHorizon = value; }
        }
 
        public bool ShowHorizonPanorama
        {
            get { return Properties.Settings.Default.ShowHorizonPanorama; }
            set { Properties.Settings.Default.ShowHorizonPanorama = value; }
        }
   

        public bool ShowMoonsAsPointSource
        {
            get { return Properties.Settings.Default.ShowMoonsAsPointSource; }
            set { Properties.Settings.Default.ShowMoonsAsPointSource = value; }
        }


        public bool ShowSolarSystem
        {
            get { return Properties.Settings.Default.ShowSolarSystem.TargetState; }
            set { Properties.Settings.Default.ShowSolarSystem.TargetState = value; }
        }


        public int FovTelescope
        {
            get { return Properties.Settings.Default.FovTelescope; }
            set { Properties.Settings.Default.FovTelescope = value; }
        }


        public int FovEyepiece
        {
            get { return Properties.Settings.Default.FovEyepiece; }
            set { Properties.Settings.Default.FovEyepiece = value; }
        }


        public int FovCamera
        {
            get { return Properties.Settings.Default.FovCamera; }
            set { Properties.Settings.Default.FovCamera = value; }
        }


        public bool LocalHorizonMode
        {
            get { return Properties.Settings.Default.LocalHorizonMode; }
            set { Properties.Settings.Default.LocalHorizonMode = value; }
        }


        public bool GalacticMode
        {
            get { return Properties.Settings.Default.GalacticMode; }
            set { Properties.Settings.Default.GalacticMode = value; }
        }

#region ISettings Members


        public bool SolarSystemStars
        {
            get { return Properties.Settings.Default.SolarSystemStars.TargetState; }
            set { Properties.Settings.Default.SolarSystemStars.TargetState = value; }
        }

        public bool SolarSystemMilkyWay
        {
            get { return Properties.Settings.Default.SolarSystemMilkyWay.TargetState; }
            set { Properties.Settings.Default.SolarSystemMilkyWay.TargetState = value; }
        }

        public bool SolarSystemCosmos
        {
            get { return Properties.Settings.Default.SolarSystemCosmos.TargetState; }
            set { Properties.Settings.Default.SolarSystemCosmos.TargetState = value; }
        }

        public bool SolarSystemCMB
        {
            get { return Properties.Settings.Default.SolarSystemCMB.TargetState; }
            set { Properties.Settings.Default.SolarSystemCMB.TargetState = value; }
        }

        public bool SolarSystemMinorPlanets
        {
            get { return Properties.Settings.Default.SolarSystemMinorPlanets.TargetState; }
            set { Properties.Settings.Default.SolarSystemMinorPlanets.TargetState = value; }
        }

        public bool SolarSystemPlanets
        {
            get { return Properties.Settings.Default.SolarSystemPlanets.TargetState; }
            set { Properties.Settings.Default.SolarSystemPlanets.TargetState = value; }
        }  

        public bool SolarSystemOrbits
        {
            get { return Properties.Settings.Default.SolarSystemOrbits.TargetState; }
            set { Properties.Settings.Default.SolarSystemOrbits.TargetState = value; }
        }

        public bool SolarSystemMinorOrbits
        {
            get { return Properties.Settings.Default.SolarSystemMinorOrbits.TargetState; }
            set { Properties.Settings.Default.SolarSystemMinorOrbits.TargetState = value; }
        }

        public bool SolarSystemOverlays
        {
            get { return Properties.Settings.Default.SolarSystemOverlays.TargetState; }
            set { Properties.Settings.Default.SolarSystemOverlays.TargetState = value; }
        }

        public bool SolarSystemLighting
        {
            get { return Properties.Settings.Default.SolarSystemLighting; }
            set { Properties.Settings.Default.SolarSystemLighting = value; }
        }

        public bool ShowISSModel
        {
            get { return Properties.Settings.Default.ShowISSModel; }
            set { Properties.Settings.Default.ShowISSModel = value; }
        }

        public int SolarSystemScale
        {
            get { return Properties.Settings.Default.SolarSystemScale; }
            set { Properties.Settings.Default.SolarSystemScale = value; }
        }


        public int MinorPlanetsFilter
        {
            get
            {
                return Properties.Settings.Default.MinorPlanetsFilter;
            }
        }

        public int PlanetOrbitsFilter
        {
            get
            {
                return Properties.Settings.Default.PlanetOrbitsFilter;
            }
        }


        public bool SolarSystemMultiRes
        {
            get { return Properties.Settings.Default.SolarSystemMultiRes; }
            set { Properties.Settings.Default.SolarSystemMultiRes = value; }
        } 
        public bool ShowEarthSky
        {
            get { return Properties.Settings.Default.ShowEarthSky.TargetState; }
            set { Properties.Settings.Default.ShowEarthSky.TargetState = value; }
        }
#endregion


        public bool ShowEquatorialGridText
        {
            get { return Properties.Settings.Default.ShowEquatorialGridText.TargetState; }
            set { Properties.Settings.Default.ShowEquatorialGridText.TargetState = value; }
        }

        public bool ShowGalacticGrid
        {
            get { return Properties.Settings.Default.ShowGalacticGrid.TargetState; }
            set { Properties.Settings.Default.ShowGalacticGrid.TargetState = value; }
        }

        public bool ShowGalacticGridText
        {
            get { return Properties.Settings.Default.ShowGalacticGridText.TargetState; }
            set { Properties.Settings.Default.ShowGalacticGridText.TargetState = value; }
        }

        public bool ShowEclipticGrid
        {
            get { return Properties.Settings.Default.ShowEclipticGrid.TargetState; }
            set { Properties.Settings.Default.ShowEclipticGrid.TargetState = value; }
        }

        public bool ShowEclipticGridText
        {
            get { return Properties.Settings.Default.ShowEclipticGridText.TargetState; }
            set { Properties.Settings.Default.ShowEclipticGridText.TargetState = value; }
        }

        public bool ShowEclipticOverviewText
        {
            get { return Properties.Settings.Default.ShowEclipticOverviewText.TargetState; }
            set { Properties.Settings.Default.ShowEclipticOverviewText.TargetState = value; }
        }

        public bool ShowAltAzGrid
        {
            get { return Properties.Settings.Default.ShowAltAzGrid.TargetState; }
            set { Properties.Settings.Default.ShowAltAzGrid.TargetState = value; }
        }

        public bool ShowAltAzGridText
        {
            get { return Properties.Settings.Default.ShowAltAzGridText.TargetState; }
            set { Properties.Settings.Default.ShowAltAzGridText.TargetState = value; }
        }

        public bool ShowPrecessionChart
        {
            get { return Properties.Settings.Default.ShowPrecessionChart.TargetState; }
            set { Properties.Settings.Default.ShowPrecessionChart.TargetState = value; }
        }

        public bool ShowConstellationPictures
        {
            get { return Properties.Settings.Default.ShowConstellationPictures.TargetState; }
            set { Properties.Settings.Default.ShowConstellationPictures.TargetState = value; }
        }

        public bool ShowConstellationLabels
        {
            get { return Properties.Settings.Default.ShowConstellationLabels.TargetState; }
            set { Properties.Settings.Default.ShowConstellationLabels.TargetState = value; }
        }

        public string ConstellationsEnabled
        {
            get { return Properties.Settings.Default.ConstellationsEnabled; }
            set { Properties.Settings.Default.ConstellationsEnabled = value; }
        }

        public bool ShowSkyNode
        {
            get { return Properties.Settings.Default.ShowSkyNode.TargetState; }
            set { Properties.Settings.Default.ShowSkyNode.TargetState = value; }
        }

        public bool ShowSkyGrids
        {
            get { return Properties.Settings.Default.ShowSkyGrids.TargetState; }
            set { Properties.Settings.Default.ShowSkyGrids.TargetState = value; }
        }

        public bool ShowSkyOverlaysIn3d
        {
            get { return Properties.Settings.Default.ShowSkyOverlaysIn3d.TargetState; }
            set { Properties.Settings.Default.ShowSkyGrids.TargetState = value; }
        }

        public bool ShowSkyOverlays
        {
            get { return Properties.Settings.Default.ShowSkyOverlays.TargetState; }
            set { Properties.Settings.Default.ShowSkyOverlays.TargetState = value; }
        }

        public bool ShowConstellations
        {
            get { return Properties.Settings.Default.Constellations.TargetState; }
            set { Properties.Settings.Default.Constellations.TargetState = value; }
        }

#region IScriptable Members

        ScriptableProperty[] IScriptable.GetProperties()
        {
           return GetSettingsList();
        }

        public static ScriptableProperty[] GetSettingsList()
        {
            List<ScriptableProperty> settingsList = new List<ScriptableProperty>();

            foreach (ScriptableProperty setting in SettingsList.Values)
            {
                settingsList.Add(setting);
            }
            return settingsList.ToArray();
        }

        string[] IScriptable.GetActions()
        {
            return new string[0];
        }

        void IScriptable.InvokeAction(string name, string value)
        {
            return;
        }

        void IScriptable.SetProperty(string name, string value)
        {
            SetSetting(name, value, true);
        }

        string IScriptable.GetProperty(string name)
        {
            return GetSetting(name).ToString();
        }

        bool IScriptable.ToggleProperty(string name)
        {
            return ToggleSetting(name);
        }

        #endregion
        public static bool ignoreChanges = false;

        public static bool SetSetting(string name, string value, bool quickChange)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (quickChange)
            {
                Settings.ignoreChanges = true;
            }
            try
            {

                bool safeToSet = SettingsList.ContainsKey(name);

                if (safeToSet)
                {
                    Type thisType = Properties.Settings.Default.GetType();
                    PropertyInfo pi = thisType.GetProperty(name);
                    if (pi.PropertyType.BaseType() == typeof(Enum))
                    {
                        pi.SetValue(Properties.Settings.Default, Enum.Parse(pi.PropertyType, value), null);
                    }
                    else if (pi.PropertyType == typeof(TimeSpan))
                    {
                        pi.SetValue(Properties.Settings.Default, TimeSpan.Parse(value), null);
                    }
                    else if (pi.PropertyType == typeof(BlendState))
                    {
                        try
                        {


                            BlendState blendState = Properties.Settings.Default[name] as BlendState;
                            if (blendState != null)
                            {
                                if (value.ToLower() == "true" || value.ToLower() == "false")
                                {
                                    blendState.TargetState = value.ToLower() == "true";
                                }
                                else
                                {
                                    float val = float.Parse(value);
                                    blendState.Opacity = val;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    else if (pi.PropertyType == typeof(ConstellationFilter))
                    {
                        try
                        {
                            ConstellationFilter filter = Properties.Settings.Default[name] as ConstellationFilter;
                            if (filter != null)
                            {
                                filter.Clone(ConstellationFilter.Families[value]);
                            }
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        pi.SetValue(Properties.Settings.Default, Convert.ChangeType(value, pi.PropertyType), null);
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                Settings.ignoreChanges = false;
            }
        }

        public static object GetSetting(string name)
        {
            try
            {
                bool safeToSet = SettingsList.ContainsKey(name);

                if (safeToSet)
                {
                    Type thisType = Properties.Settings.Default.GetType();
                    PropertyInfo pi = thisType.GetProperty(name);

                    return pi.GetValue(Properties.Settings.Default, null);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static bool ToggleSetting(string name)
        {
            bool status = false;
            try
            {
                bool safeToSet = SettingsList.ContainsKey(name);

                if (safeToSet)
                {
                    Type thisType = Properties.Settings.Default.GetType();
                    PropertyInfo pi = thisType.GetProperty(name);
                    if (pi.PropertyType == typeof(bool))
                    {
                        Boolean val = (bool)pi.GetValue(Properties.Settings.Default, null);
                        pi.SetValue(Properties.Settings.Default, (Boolean)(!val), null);
                        status = !val;
                    }
                    else if (pi.PropertyType == typeof(BlendState))
                    {
                        BlendState bval = (BlendState)pi.GetValue(Properties.Settings.Default, null);
                        bval.TargetState = !bval.TargetState;
                        status = bval.TargetState;
                    }
                    else
                    {
                        return status;
                    }

                    return status;
                }
                return status;
            }
            catch
            {
                return status;
            }
        }

        public static string[] GetBoolSettings()
        {
            List<string> boolSettings = new List<string>();

            foreach (string setting in SettingsList.Keys)
            {
                Type thisType = Properties.Settings.Default.GetType();
                PropertyInfo pi = thisType.GetProperty(setting);
                if (pi.PropertyType == typeof(bool))
                {
                    boolSettings.Add(setting);
                }
            }
            return boolSettings.ToArray();
        }



        public static string[] GetSettingsByType(Type type)
        {
            List<string> SettingsByType = new List<string>();

            foreach (string setting in SettingsList.Keys)
            {
                Type thisType = Properties.Settings.Default.GetType();
                PropertyInfo pi = thisType.GetProperty(setting);
                if (pi.PropertyType == type)
                {
                    SettingsByType.Add(setting);
                }
            }
            return SettingsByType.ToArray();
        }


        #region ISettings Members


        public ConstellationFilter ConstellationFiguresFilter
        {
            get { return Properties.Settings.Default.ConstellationFiguresFilter; }
        }

        public ConstellationFilter ConstellationBoundariesFilter
        {
            get { return Properties.Settings.Default.ConstellationBoundariesFilter; }
        }

        public ConstellationFilter ConstellationNamesFilter
        {
            get { return Properties.Settings.Default.ConstellationNamesFilter; }
        }

        public ConstellationFilter ConstellationArtFilter
        {
            get { return Properties.Settings.Default.ConstellationArtFilter; }
        }

        public SettingParameter GetSetting(StockSkyOverlayTypes type)
        {
            bool edgeTrigger = false;
            double opacity = 0;
            bool targetState = false;

            ConstellationFilter filter = null;

            switch (type)
            {
                case StockSkyOverlayTypes.EquatorialGrid:
                    opacity = Properties.Settings.Default.ShowGrid.Opacity;
                    targetState = Properties.Settings.Default.ShowGrid.TargetState;
                    break;
                case StockSkyOverlayTypes.EquatorialGridText:
                    opacity = Properties.Settings.Default.ShowEquatorialGridText.Opacity;
                    targetState = Properties.Settings.Default.ShowEquatorialGridText.TargetState;
                    break;
                case StockSkyOverlayTypes.GalacticGrid:
                    opacity = Properties.Settings.Default.ShowGalacticGrid.Opacity;
                    targetState = Properties.Settings.Default.ShowGalacticGrid.TargetState;
                    break;
                case StockSkyOverlayTypes.GalacticGridText:
                    opacity = Properties.Settings.Default.ShowGalacticGridText.Opacity;
                    targetState = Properties.Settings.Default.ShowGalacticGridText.TargetState;
                    break;
                case StockSkyOverlayTypes.EclipticGrid:
                    opacity = Properties.Settings.Default.ShowEclipticGrid.Opacity;
                    targetState = Properties.Settings.Default.ShowEclipticGrid.TargetState;
                    break;
                case StockSkyOverlayTypes.EclipticGridText:
                    opacity = Properties.Settings.Default.ShowEclipticGridText.Opacity;
                    targetState = Properties.Settings.Default.ShowEclipticGridText.TargetState;
                    break;
                case StockSkyOverlayTypes.EclipticOverview:
                    opacity = Properties.Settings.Default.ShowEcliptic.Opacity;
                    targetState = Properties.Settings.Default.ShowEcliptic.TargetState;
                    break;
                case StockSkyOverlayTypes.EclipticOverviewText:
                    opacity = Properties.Settings.Default.ShowEclipticOverviewText.Opacity;
                    targetState = Properties.Settings.Default.ShowEclipticOverviewText.TargetState;
                    break;
                case StockSkyOverlayTypes.PrecessionChart:
                    opacity = Properties.Settings.Default.ShowPrecessionChart.Opacity;
                    targetState = Properties.Settings.Default.ShowPrecessionChart.TargetState;
                    break;
                case StockSkyOverlayTypes.AltAzGrid:
                    opacity = Properties.Settings.Default.ShowAltAzGrid.Opacity;
                    targetState = Properties.Settings.Default.ShowAltAzGrid.TargetState;
                    break;
                case StockSkyOverlayTypes.AltAzGridText:
                    opacity = Properties.Settings.Default.ShowAltAzGridText.Opacity;
                    targetState = Properties.Settings.Default.ShowAltAzGridText.TargetState;
                    break;
                case StockSkyOverlayTypes.ConstellationFigures:
                    opacity = Properties.Settings.Default.ShowConstellationFigures.Opacity;
                    targetState = Properties.Settings.Default.ShowConstellationFigures.TargetState;
                    filter = Properties.Settings.Default.ConstellationFiguresFilter.Clone();
                    break;
                case StockSkyOverlayTypes.ConstellationBoundaries:
                    opacity = Properties.Settings.Default.ShowConstellationBoundries.Opacity;
                    targetState = Properties.Settings.Default.ShowConstellationBoundries.TargetState;
                    filter = Properties.Settings.Default.ConstellationBoundariesFilter.Clone();
                    break;
                case StockSkyOverlayTypes.ConstellationFocusedOnly:
                    opacity = Properties.Settings.Default.ShowConstellationSelection.Opacity;
                    targetState = Properties.Settings.Default.ShowConstellationSelection.TargetState;
                    break;
                case StockSkyOverlayTypes.ConstellationNames:
                    opacity = Properties.Settings.Default.ShowConstellationLabels.Opacity;
                    targetState = Properties.Settings.Default.ShowConstellationLabels.TargetState;
                    filter = Properties.Settings.Default.ConstellationNamesFilter.Clone();
                    break;
                case StockSkyOverlayTypes.ConstellationPictures:
                    opacity = Properties.Settings.Default.ShowConstellationPictures.Opacity;
                    targetState = Properties.Settings.Default.ShowConstellationPictures.TargetState;
                    filter = Properties.Settings.Default.ConstellationArtFilter.Clone();
                    break;                
                case StockSkyOverlayTypes.SkyGrids:
                    opacity = Properties.Settings.Default.ShowSkyGrids.Opacity;
                    targetState = Properties.Settings.Default.ShowSkyGrids.TargetState;
                    break;
                case StockSkyOverlayTypes.Constellations:
                    opacity = Properties.Settings.Default.Constellations.Opacity;
                    targetState = Properties.Settings.Default.Constellations.TargetState;
                    break;
                case StockSkyOverlayTypes.SolarSystemStars:
                    opacity = Properties.Settings.Default.SolarSystemStars.Opacity;
                    targetState = Properties.Settings.Default.SolarSystemStars.TargetState;
                    break;
                case StockSkyOverlayTypes.SolarSystemMilkyWay:
                    opacity = Properties.Settings.Default.SolarSystemMilkyWay.Opacity;
                    targetState = Properties.Settings.Default.SolarSystemMilkyWay.TargetState;
                    break;
                case StockSkyOverlayTypes.SolarSystemCosmos:
                    opacity = Properties.Settings.Default.SolarSystemCosmos.Opacity;
                    targetState = Properties.Settings.Default.SolarSystemCosmos.TargetState;
                    break;
                case StockSkyOverlayTypes.SolarSystemCMB:
                    opacity = Properties.Settings.Default.SolarSystemCMB.Opacity;
                    targetState = Properties.Settings.Default.SolarSystemCMB.TargetState;
                    break;    
                case StockSkyOverlayTypes.SolarSystemOrbits:
                    opacity = Properties.Settings.Default.SolarSystemOrbits.Opacity;
                    targetState = Properties.Settings.Default.SolarSystemOrbits.TargetState;
                    break;
                case StockSkyOverlayTypes.OrbitFilters:
                    opacity = Properties.Settings.Default.PlanetOrbitsFilter;
                    targetState = Properties.Settings.Default.PlanetOrbitsFilter != 0;
                    edgeTrigger = true;
                    break;
                case StockSkyOverlayTypes.SolarSystemPlanets:
                    opacity = Properties.Settings.Default.SolarSystemPlanets.Opacity;
                    targetState = Properties.Settings.Default.SolarSystemPlanets.TargetState;
                    break;
                case StockSkyOverlayTypes.SolarSystemAsteroids:
                    opacity = Properties.Settings.Default.SolarSystemMinorPlanets.Opacity;
                    targetState = Properties.Settings.Default.SolarSystemMinorPlanets.TargetState;
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
                        targetState = (Properties.Settings.Default.MinorPlanetsFilter & bit) != 0;
                        opacity = targetState ? 1 : 0;
                        edgeTrigger = true;
                    }
                    break;

                case StockSkyOverlayTypes.SolarSystemLighting:
                    opacity = Properties.Settings.Default.SolarSystemLighting ? 1 : 0;
                    targetState = Properties.Settings.Default.SolarSystemLighting;
                    break;
                case StockSkyOverlayTypes.ShowISSModel:
                    opacity = Properties.Settings.Default.ShowISSModel ? 1 : 0;
                    targetState = Properties.Settings.Default.ShowISSModel;
                    break;
                case StockSkyOverlayTypes.SolarSystemMinorOrbits:
                    opacity = Properties.Settings.Default.SolarSystemMinorOrbits.Opacity;
                    targetState = Properties.Settings.Default.SolarSystemMinorOrbits.TargetState;
                    break;
                case StockSkyOverlayTypes.ShowEarthCloudLayer:
                    opacity = Properties.Settings.Default.ShowClouds.Opacity;
                    targetState = Properties.Settings.Default.ShowClouds.TargetState;
                    break;
                case StockSkyOverlayTypes.ShowElevationModel:
                    opacity = Properties.Settings.Default.ShowElevationModel ? 1 : 0;
                    targetState = Properties.Settings.Default.ShowElevationModel;
                    break;
                case StockSkyOverlayTypes.MultiResSolarSystemBodies:
                    opacity = Properties.Settings.Default.SolarSystemMultiRes ? 1 : 0;
                    targetState = Properties.Settings.Default.SolarSystemMultiRes;
                    break;
                case StockSkyOverlayTypes.EarthCutAway:
                    opacity = Properties.Settings.Default.EarthCutawayView.Opacity;
                    targetState = Properties.Settings.Default.EarthCutawayView.TargetState;
                    break;
                case StockSkyOverlayTypes.ShowSolarSystem:
                    opacity = Properties.Settings.Default.ShowSolarSystem.Opacity;
                    targetState = Properties.Settings.Default.ShowSolarSystem.TargetState;
                    break;
                case StockSkyOverlayTypes.Clouds8k:
                    opacity = Properties.Settings.Default.CloudMap8k ? 1 : 0;
                    targetState = Properties.Settings.Default.CloudMap8k;
                    break;
                case StockSkyOverlayTypes.FiledOfView:
                    opacity = Properties.Settings.Default.ShowFieldOfView.Opacity;
                    targetState = Properties.Settings.Default.ShowFieldOfView.TargetState;
                    break;
                case StockSkyOverlayTypes.FadeToBlack:
                    opacity = RenderEngine.Engine.Fader.Opacity;
                    targetState = RenderEngine.Engine.Fader.TargetState;
                    break;
                default:
                    return new SettingParameter(false, -1, false, null);
            }

            return new SettingParameter(edgeTrigger, opacity, targetState, filter);
        }

#endregion
    }
}
