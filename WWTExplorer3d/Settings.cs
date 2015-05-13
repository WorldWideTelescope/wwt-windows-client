using System;
using System.Collections.Generic;
using System.Text;

namespace TerraViewer
{
    class Settings : TerraViewer.ISettings, IScriptable
    {
        static ISettings active = new Settings();

        internal static ISettings Ambient
        {
            get
            {
                return active;
            }
        }

        internal static ISettings Active
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

        internal static ISettings TourSettings
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
                return (Properties.Settings.Default.MasterController || masterController ) && !Earth3d.DomeViewer;
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
                return Properties.Settings.Default.DomeView || domeView || Earth3d.DomeViewer;
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
            return HTTPLayerApi.GetSettingsList();
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
            HTTPLayerApi.SetSetting(name, value, true);
        }

        string IScriptable.GetProperty(string name)
        {
            return HTTPLayerApi.GetSetting(name).ToString();
        }

        bool IScriptable.ToggleProperty(string name)
        {
            return HTTPLayerApi.ToggleSetting(name);
        }

        #endregion

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
                    opacity = Earth3d.MainWindow.Fader.Opacity;
                    targetState = Earth3d.MainWindow.Fader.TargetState;
                    break;
                default:
                    return new SettingParameter(false, -1, false, null);
            }

            return new SettingParameter(edgeTrigger, opacity, targetState, filter);
        }

        #endregion
    }
}
