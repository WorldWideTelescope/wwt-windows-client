using Color = Windows.UI.Color;
using System.Collections.Generic;

namespace TerraViewer.Properties
{


    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.1.0.0")]
    internal sealed partial class Settings
    {

        private static Settings defaultInstance = new Settings();
        Dictionary<string, object> settingsTable = new Dictionary<string, object>();
        public object this[string index]
        {
            get
            {
                if (settingsTable.ContainsKey(index))
                {
                    return settingsTable[index];
                }
                else
                {
                    //todo fix impliment reflection
                    //get default with reflection
                    return null;
                }
            }
            set
            {
                settingsTable[index] = value;
            }
        }

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("MidnightBlue")]
        public Color ConstellationBoundryColor
        {
            get
            {
                return ((Color)(this["ConstellationBoundryColor"]));
            }
            set
            {
                this["ConstellationBoundryColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("DarkRed")]
        public Color ConstellationFigureColor
        {
            get
            {
                return ((Color)(this["ConstellationFigureColor"]));
            }
            set
            {
                this["ConstellationFigureColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("DarkGoldenrod")]
        public Color ConstellationSelectionColor
        {
            get
            {
                return ((Color)(this["ConstellationSelectionColor"]));
            }
            set
            {
                this["ConstellationSelectionColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowConstellationBoundries
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowConstellationBoundries"]));
            }
            set
            {
                this["ShowConstellationBoundries"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowConstellationFigures
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowConstellationFigures"]));
            }
            set
            {
                this["ShowConstellationFigures"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowConstellationSelection
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowConstellationSelection"]));
            }
            set
            {
                this["ShowConstellationSelection"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowClouds
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowClouds"]));
            }
            set
            {
                this["ShowClouds"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("1")]
        public int ZoomSpeed
        {
            get
            {
                return ((int)(this["ZoomSpeed"]));
            }
            set
            {
                this["ZoomSpeed"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool ShowElevationModel
        {
            get
            {
                return ((bool)(this["ShowElevationModel"]));
            }
            set
            {
                this["ShowElevationModel"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ShowOceanFloor
        {
            get
            {
                return ((bool)(this["ShowOceanFloor"]));
            }
            set
            {
                this["ShowOceanFloor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string TelescopeID
        {
            get
            {
                return ((string)(this["TelescopeID"]));
            }
            set
            {
                this["TelescopeID"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool ShowCrosshairs
        {
            get
            {
                return ((bool)(this["ShowCrosshairs"]));
            }
            set
            {
                this["ShowCrosshairs"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ShowDatasetNames
        {
            get
            {
                return ((bool)(this["ShowDatasetNames"]));
            }
            set
            {
                this["ShowDatasetNames"] = value;
            }
        }

        [ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        //[global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [DefaultSettingValueAttribute("http://cdsws.u-strasbg.fr/axis/services/Sesame")]
        public string WWTExplorer_fr_u_strasbg_cdsws_SesameService
        {
            get
            {
                return ((string)(this["WWTExplorer_fr_u_strasbg_cdsws_SesameService"]));
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int FovTelescope
        {
            get
            {
                return ((int)(this["FovTelescope"]));
            }
            set
            {
                this["FovTelescope"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int FovCamera
        {
            get
            {
                return ((int)(this["FovCamera"]));
            }
            set
            {
                this["FovCamera"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public global::TerraViewer.BlendState ShowFieldOfView
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowFieldOfView"]));
            }
            set
            {
                this["ShowFieldOfView"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string CahceDirectory
        {
            get
            {
                return "CahceDirectory";
            }
            set
            {
                
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("Unfiltered")]
        public string ContextSearchFilter
        {
            get
            {
                return ((string)(this["ContextSearchFilter"]));
            }
            set
            {
                this["ContextSearchFilter"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowEcliptic
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowEcliptic"]));
            }
            set
            {
                this["ShowEcliptic"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("White")]
        public Color FovColor
        {
            get
            {
                return ((Color)(this["FovColor"]));
            }
            set
            {
                this["FovColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("Microsoft Research Building 99")]
        public string LocationName
        {
            get
            {
                return ((string)(this["LocationName"]));
            }
            set
            {
                this["LocationName"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("47.64222")]
        public double LocationLat
        {
            get
            {
                return ((double)(this["LocationLat"]));
            }
            set
            {
                this["LocationLat"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("-122.142")]
        public double LocationLng
        {
            get
            {
                return ((double)(this["LocationLng"]));
            }
            set
            {
                this["LocationLng"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string UserEmail
        {
            get
            {
                return ((string)(this["UserEmail"]));
            }
            set
            {
                this["UserEmail"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string UserKey
        {
            get
            {
                return ((string)(this["UserKey"]));
            }
            set
            {
                this["UserKey"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ShowHorizon
        {
            get
            {
                return ((bool)(this["ShowHorizon"]));
            }
            set
            {
                this["ShowHorizon"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("1")]
        public double FullScreenGamma
        {
            get
            {
                return ((double)(this["FullScreenGamma"]));
            }
            set
            {
                this["FullScreenGamma"] = value;
            }
        }

        [ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
       // [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [DefaultSettingValueAttribute("http://www.worldwidetelescope.org/webservices/wwtwebservice.asmx")]
        public string WWTExplorer_org_worldwidetelescope_www_WWTWebService
        {
            get
            {
                return ((string)(this["WWTExplorer_org_worldwidetelescope_www_WWTWebService"]));
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("Default Figures")]
        public string ConstellationFiguresFile
        {
            get
            {
                return ((string)(this["ConstellationFiguresFile"]));
            }
            set
            {
                this["ConstellationFiguresFile"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("100")]
        public double LocationAltitude
        {
            get
            {
                return ((double)(this["LocationAltitude"]));
            }
            set
            {
                this["LocationAltitude"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int FovEyepiece
        {
            get
            {
                return ((int)(this["FovEyepiece"]));
            }
            set
            {
                this["FovEyepiece"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("CornflowerBlue")]
        public Color EclipticColor
        {
            get
            {
                return ((Color)(this["EclipticColor"]));
            }
            set
            {
                this["EclipticColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("64, 64, 64")]
        public Color GridColor
        {
            get
            {
                return ((Color)(this["GridColor"]));
            }
            set
            {
                this["GridColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool LocalHorizonMode
        {
            get
            {
                return ((bool)(this["LocalHorizonMode"]));
            }
            set
            {
                this["LocalHorizonMode"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool ActualPlanetScale
        {
            get
            {
                return ((bool)(this["ActualPlanetScale"]));
            }
            set
            {
                this["ActualPlanetScale"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowSolarSystem
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowSolarSystem"]));
            }
            set
            {
                this["ShowSolarSystem"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool HighPercitionPlanets
        {
            get
            {
                return ((bool)(this["HighPercitionPlanets"]));
            }
            set
            {
                this["HighPercitionPlanets"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ShowHorizonPanorama
        {
            get
            {
                return ((bool)(this["ShowHorizonPanorama"]));
            }
            set
            {
                this["ShowHorizonPanorama"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool SmoothPan
        {
            get
            {
                return ((bool)(this["SmoothPan"]));
            }
            set
            {
                this["SmoothPan"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ShowUTCTime
        {
            get
            {
                return ((bool)(this["ShowUTCTime"]));
            }
            set
            {
                this["ShowUTCTime"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ShowMoonsAsPointSource
        {
            get
            {
                return ((bool)(this["ShowMoonsAsPointSource"]));
            }
            set
            {
                this["ShowMoonsAsPointSource"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool PulseMeForUpdate
        {
            get
            {
                return ((bool)(this["PulseMeForUpdate"]));
            }
            set
            {
                this["PulseMeForUpdate"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool ShowSafeArea
        {
            get
            {
                return ((bool)(this["ShowSafeArea"]));
            }
            set
            {
                this["ShowSafeArea"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("00000000-0000-0000-0000-000000000000")]
        public global::System.Guid UserRatingGUID
        {
            get
            {
                return ((global::System.Guid)(this["UserRatingGUID"]));
            }
            set
            {
                this["UserRatingGUID"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("100")]
        public int ImageQuality
        {
            get
            {
                return ((int)(this["ImageQuality"]));
            }
            set
            {
                this["ImageQuality"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool FollowMouseOnZoom
        {
            get
            {
                return ((bool)(this["FollowMouseOnZoom"]));
            }
            set
            {
                this["FollowMouseOnZoom"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool AutoHideContext
        {
            get
            {
                return ((bool)(this["AutoHideContext"]));
            }
            set
            {
                this["AutoHideContext"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool AutoHideTabs
        {
            get
            {
                return ((bool)(this["AutoHideTabs"]));
            }
            set
            {
                this["AutoHideTabs"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ShowLinksInFullBrowser
        {
            get
            {
                return ((bool)(this["ShowLinksInFullBrowser"]));
            }
            set
            {
                this["ShowLinksInFullBrowser"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool TranparentWindows
        {
            get
            {
                return ((bool)(this["TranparentWindows"]));
            }
            set
            {
                this["TranparentWindows"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool CheckedForFlashingVideo
        {
            get
            {
                return ((bool)(this["CheckedForFlashingVideo"]));
            }
            set
            {
                this["CheckedForFlashingVideo"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool AutoRepeatTour
        {
            get
            {
                return ((bool)(this["AutoRepeatTour"]));
            }
            set
            {
                this["AutoRepeatTour"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool AutoRepeatTourAll
        {
            get
            {
                return ((bool)(this["AutoRepeatTourAll"]));
            }
            set
            {
                this["AutoRepeatTourAll"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool ShowNavHelp
        {
            get
            {
                return ((bool)(this["ShowNavHelp"]));
            }
            set
            {
                this["ShowNavHelp"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool UpgradeNeeded
        {
            get
            {
                return ((bool)(this["UpgradeNeeded"]));
            }
            set
            {
                this["UpgradeNeeded"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string ProxyServer
        {
            get
            {
                return ((string)(this["ProxyServer"]));
            }
            set
            {
                this["ProxyServer"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("80")]
        public string ProxyPort
        {
            get
            {
                return ((string)(this["ProxyPort"]));
            }
            set
            {
                this["ProxyPort"] = value;
            }
        }

        [ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
       // [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [DefaultSettingValueAttribute("http://masthla.stsci.edu/hla/footprints/fpws/hstfootprint.asmx")]
        public string WWTExplorer_edu_stsci_masthla_HSTFootprint
        {
            get
            {
                return ((string)(this["WWTExplorer_edu_stsci_masthla_HSTFootprint"]));
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool UseJoystick
        {
            get
            {
                return ((bool)(this["UseJoystick"]));
            }
            set
            {
                this["UseJoystick"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool ShowJoystickHelp
        {
            get
            {
                return ((bool)(this["ShowJoystickHelp"]));
            }
            set
            {
                this["ShowJoystickHelp"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("en")]
        public string LanguageCode
        {
            get
            {
                return ((string)(this["LanguageCode"]));
            }
            set
            {
                this["LanguageCode"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool LineSmoothing
        {
            get
            {
                return ((bool)(this["LineSmoothing"]));
            }
            set
            {
                this["LineSmoothing"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=lang_en")]
        public string LanguageUrl
        {
            get
            {
                return ((string)(this["LanguageUrl"]));
            }
            set
            {
                this["LanguageUrl"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?W=ExploreRoot")]
        public string ExploreRootUrl
        {
            get
            {
                return ((string)(this["ExploreRootUrl"]));
            }
            set
            {
                this["ExploreRootUrl"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?X=ImageSets5")]
        public string ImageSetUrl
        {
            get
            {
                return ((string)(this["ImageSetUrl"]));
            }
            set
            {
                this["ImageSetUrl"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("US English")]
        public string LanguageName
        {
            get
            {
                return ((string)(this["LanguageName"]));
            }
            set
            {
                this["LanguageName"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool SolarSystemLighting
        {
            get
            {
                return ((bool)(this["SolarSystemLighting"]));
            }
            set
            {
                this["SolarSystemLighting"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public global::TerraViewer.BlendState SolarSystemOverlays
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["SolarSystemOverlays"]));
            }
            set
            {
                this["SolarSystemOverlays"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState SolarSystemOrbits
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["SolarSystemOrbits"]));
            }
            set
            {
                this["SolarSystemOrbits"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState SolarSystemStars
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["SolarSystemStars"]));
            }
            set
            {
                this["SolarSystemStars"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState SolarSystemMilkyWay
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["SolarSystemMilkyWay"]));
            }
            set
            {
                this["SolarSystemMilkyWay"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState SolarSystemCosmos
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["SolarSystemCosmos"]));
            }
            set
            {
                this["SolarSystemCosmos"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("1")]
        public int SolarSystemScale
        {
            get
            {
                return ((int)(this["SolarSystemScale"]));
            }
            set
            {
                this["SolarSystemScale"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("64, 64, 64")]
        public Color SolarSystemOrbitColor
        {
            get
            {
                return ((Color)(this["SolarSystemOrbitColor"]));
            }
            set
            {
                this["SolarSystemOrbitColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool SolarSystemMultiRes
        {
            get
            {
                return ((bool)(this["SolarSystemMultiRes"]));
            }
            set
            {
                this["SolarSystemMultiRes"] = value;
            }
        }

        [ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
       // [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [DefaultSettingValueAttribute("http://nvo.stsci.edu/VORegistry/registry.asmx")]
        public string WWTExplorer_edu_stsci_nvo_Registry
        {
            get
            {
                return ((string)(this["WWTExplorer_edu_stsci_nvo_Registry"]));
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public double DomeTilt
        {
            get
            {
                return ((double)(this["DomeTilt"]));
            }
            set
            {
                this["DomeTilt"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool DomeView
        {
            get
            {
                return ((bool)(this["DomeView"]));
            }
            set
            {
                this["DomeView"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ListenMode
        {
            get
            {
                return ((bool)(this["ListenMode"]));
            }
            set
            {
                this["ListenMode"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool MasterController
        {
            get
            {
                return ((bool)(this["MasterController"]));
            }
            set
            {
                this["MasterController"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("1")]
        public int VOTableVerbosityDefault
        {
            get
            {
                return ((int)(this["VOTableVerbosityDefault"]));
            }
            set
            {
                this["VOTableVerbosityDefault"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("4")]
        public int StartUpLookAt
        {
            get
            {
                return ((int)(this["StartUpLookAt"]));
            }
            set
            {
                this["StartUpLookAt"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("2")]
        public int LastLookAtMode
        {
            get
            {
                return ((int)(this["LastLookAtMode"]));
            }
            set
            {
                this["LastLookAtMode"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool UsePlanetColorsForOrbits
        {
            get
            {
                return ((bool)(this["UsePlanetColorsForOrbits"]));
            }
            set
            {
                this["UsePlanetColorsForOrbits"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool LargeDomeTextures
        {
            get
            {
                return ((bool)(this["LargeDomeTextures"]));
            }
            set
            {
                this["LargeDomeTextures"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string LiveIdUser
        {
            get
            {
                return ((string)(this["LiveIdUser"]));
            }
            set
            {
                this["LiveIdUser"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string LiveIdToken
        {
            get
            {
                return ((string)(this["LiveIdToken"]));
            }
            set
            {
                this["LiveIdToken"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string LiveIdWWTId
        {
            get
            {
                return ((string)(this["LiveIdWWTId"]));
            }
            set
            {
                this["LiveIdWWTId"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("217041268855")]
        public string FBApplicationId
        {
            get
            {
                return ((string)(this["FBApplicationId"]));
            }
            set
            {
                this["FBApplicationId"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("55d5ef1761749fc8b46aca991fd3bb09")]
        public string FBApiKey
        {
            get
            {
                return ((string)(this["FBApiKey"]));
            }
            set
            {
                this["FBApiKey"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string FBApiSecret
        {
            get
            {
                return ((string)(this["FBApiSecret"]));
            }
            set
            {
                this["FBApiSecret"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public long FBUserId
        {
            get
            {
                return ((long)(this["FBUserId"]));
            }
            set
            {
                this["FBUserId"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string FBUserKey
        {
            get
            {
                return ((string)(this["FBUserKey"]));
            }
            set
            {
                this["FBUserKey"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string FBUserSecret
        {
            get
            {
                return ((string)(this["FBUserSecret"]));
            }
            set
            {
                this["FBUserSecret"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool AdvancedCommunities
        {
            get
            {
                return ((bool)(this["AdvancedCommunities"]));
            }
            set
            {
                this["AdvancedCommunities"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string CustomWarpFilename
        {
            get
            {
                return ((string)(this["CustomWarpFilename"]));
            }
            set
            {
                this["CustomWarpFilename"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int DomeTypeIndex
        {
            get
            {
                return ((int)(this["DomeTypeIndex"]));
            }
            set
            {
                this["DomeTypeIndex"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string SharedCacheServer
        {
            get
            {
                return ((string)(this["SharedCacheServer"]));
            }
            set
            {
                this["SharedCacheServer"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public global::TerraViewer.BlendState SolarSystemMinorPlanets
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["SolarSystemMinorPlanets"]));
            }
            set
            {
                this["SolarSystemMinorPlanets"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState SolarSystemPlanets
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["SolarSystemPlanets"]));
            }
            set
            {
                this["SolarSystemPlanets"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool FullScreenTours
        {
            get
            {
                return ((bool)(this["FullScreenTours"]));
            }
            set
            {
                this["FullScreenTours"] = value;
            }
        }

        [ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("1E-06")]
        public double MinZoonLimitSolar
        {
            get
            {
                return ((double)(this["MinZoonLimitSolar"]));
            }
        }

        [ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("1E+16")]
        public double MaxZoomLimitSolar
        {
            get
            {
                return ((double)(this["MaxZoomLimitSolar"]));
            }
        }

        [ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("89.999")]
        public double MaxLatLimit
        {
            get
            {
                return ((double)(this["MaxLatLimit"]));
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string LastDomeConfigFile
        {
            get
            {
                return ((string)(this["LastDomeConfigFile"]));
            }
            set
            {
                this["LastDomeConfigFile"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int LastVideoOutFormat
        {
            get
            {
                return ((int)(this["LastVideoOutFormat"]));
            }
            set
            {
                this["LastVideoOutFormat"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string VideoOutputPath
        {
            get
            {
                return ((string)(this["VideoOutputPath"]));
            }
            set
            {
                this["VideoOutputPath"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool ShowTouchControls
        {
            get
            {
                return ((bool)(this["ShowTouchControls"]));
            }
            set
            {
                this["ShowTouchControls"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState SolarSystemMinorOrbits
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["SolarSystemMinorOrbits"]));
            }
            set
            {
                this["SolarSystemMinorOrbits"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool AllowLocalHTTP
        {
            get
            {
                return ((bool)(this["AllowLocalHTTP"]));
            }
            set
            {
                this["AllowLocalHTTP"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string HTTPWhiteList
        {
            get
            {
                return ((string)(this["HTTPWhiteList"]));
            }
            set
            {
                this["HTTPWhiteList"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool AllowAllRemoteHTTP
        {
            get
            {
                return ((bool)(this["AllowAllRemoteHTTP"]));
            }
            set
            {
                this["AllowAllRemoteHTTP"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("45")]
        public double ScreenOverlayAlt
        {
            get
            {
                return ((double)(this["ScreenOverlayAlt"]));
            }
            set
            {
                this["ScreenOverlayAlt"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public double ScreenOverlayAz
        {
            get
            {
                return ((double)(this["ScreenOverlayAz"]));
            }
            set
            {
                this["ScreenOverlayAz"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("60")]
        public double ScreenOverlayScale
        {
            get
            {
                return ((double)(this["ScreenOverlayScale"]));
            }
            set
            {
                this["ScreenOverlayScale"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool ShowLayerManager
        {
            get
            {
                return ((bool)(this["ShowLayerManager"]));
            }
            set
            {
                this["ShowLayerManager"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool MonochromeImageStyle
        {
            get
            {
                return ((bool)(this["MonochromeImageStyle"]));
            }
            set
            {
                this["MonochromeImageStyle"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool FlatScreenWarp
        {
            get
            {
                return ((bool)(this["FlatScreenWarp"]));
            }
            set
            {
                this["FlatScreenWarp"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int FullScreenX
        {
            get
            {
                return ((int)(this["FullScreenX"]));
            }
            set
            {
                this["FullScreenX"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int FullScreenY
        {
            get
            {
                return ((int)(this["FullScreenY"]));
            }
            set
            {
                this["FullScreenY"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int FullScreenWidth
        {
            get
            {
                return ((int)(this["FullScreenWidth"]));
            }
            set
            {
                this["FullScreenWidth"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int FullScreenHeight
        {
            get
            {
                return ((int)(this["FullScreenHeight"]));
            }
            set
            {
                this["FullScreenHeight"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("http://www.worldwidetelescope.org")]
        public string CloudCommunityUrlNew
        {
            get
            {
                return ((string)(this["CloudCommunityUrlNew"]));
            }
            set
            {
                this["CloudCommunityUrlNew"] = value;
            }
        }

        [ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool CW
        {
            get
            {
                return ((bool)(this["CW"]));
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string OverlaySettings
        {
            get
            {
                return ((string)(this["OverlaySettings"]));
            }
            set
            {
                this["OverlaySettings"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowEquatorialGridText
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowEquatorialGridText"]));
            }
            set
            {
                this["ShowEquatorialGridText"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public global::TerraViewer.BlendState ShowGalacticGrid
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowGalacticGrid"]));
            }
            set
            {
                this["ShowGalacticGrid"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowGalacticGridText
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowGalacticGridText"]));
            }
            set
            {
                this["ShowGalacticGridText"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public global::TerraViewer.BlendState ShowEclipticGrid
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowEclipticGrid"]));
            }
            set
            {
                this["ShowEclipticGrid"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowEclipticGridText
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowEclipticGridText"]));
            }
            set
            {
                this["ShowEclipticGridText"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public global::TerraViewer.BlendState ShowAltAzGrid
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowAltAzGrid"]));
            }
            set
            {
                this["ShowAltAzGrid"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowAltAzGridText
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowAltAzGridText"]));
            }
            set
            {
                this["ShowAltAzGridText"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public global::TerraViewer.BlendState ShowPrecessionChart
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowPrecessionChart"]));
            }
            set
            {
                this["ShowPrecessionChart"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public global::TerraViewer.BlendState ShowConstellationPictures
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowConstellationPictures"]));
            }
            set
            {
                this["ShowConstellationPictures"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string ConstellationsEnabled
        {
            get
            {
                return ((string)(this["ConstellationsEnabled"]));
            }
            set
            {
                this["ConstellationsEnabled"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("White")]
        public Color EquatorialGridTextColor
        {
            get
            {
                return ((Color)(this["EquatorialGridTextColor"]));
            }
            set
            {
                this["EquatorialGridTextColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("DeepSkyBlue")]
        public Color GalacticGridColor
        {
            get
            {
                return ((Color)(this["GalacticGridColor"]));
            }
            set
            {
                this["GalacticGridColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("DeepSkyBlue")]
        public Color GalacticGridTextColor
        {
            get
            {
                return ((Color)(this["GalacticGridTextColor"]));
            }
            set
            {
                this["GalacticGridTextColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("Green")]
        public Color AltAzGridColor
        {
            get
            {
                return ((Color)(this["AltAzGridColor"]));
            }
            set
            {
                this["AltAzGridColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("Green")]
        public Color AltAzGridTextColor
        {
            get
            {
                return ((Color)(this["AltAzGridTextColor"]));
            }
            set
            {
                this["AltAzGridTextColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("CornflowerBlue")]
        public Color EclipticGridColor
        {
            get
            {
                return ((Color)(this["EclipticGridColor"]));
            }
            set
            {
                this["EclipticGridColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("CornflowerBlue")]
        public Color EclipticGridTextColor
        {
            get
            {
                return ((Color)(this["EclipticGridTextColor"]));
            }
            set
            {
                this["EclipticGridTextColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("255, 128, 0")]
        public Color PrecessionChartColor
        {
            get
            {
                return ((Color)(this["PrecessionChartColor"]));
            }
            set
            {
                this["PrecessionChartColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("255, 128, 0")]
        public Color ConstellationnamesColor
        {
            get
            {
                return ((Color)(this["ConstellationnamesColor"]));
            }
            set
            {
                this["ConstellationnamesColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("CornflowerBlue")]
        public Color EclipticOverviewTextColor
        {
            get
            {
                return ((Color)(this["EclipticOverviewTextColor"]));
            }
            set
            {
                this["EclipticOverviewTextColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public global::TerraViewer.BlendState ShowConstellationLabels
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowConstellationLabels"]));
            }
            set
            {
                this["ShowConstellationLabels"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("Black")]
        public Color FadeColor
        {
            get
            {
                return ((Color)(this["FadeColor"]));
            }
            set
            {
                this["FadeColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool FadeRemoteOnly
        {
            get
            {
                return ((bool)(this["FadeRemoteOnly"]));
            }
            set
            {
                this["FadeRemoteOnly"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int ColSettingsVersion
        {
            get
            {
                return ((int)(this["ColSettingsVersion"]));
            }
            set
            {
                this["ColSettingsVersion"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowSkyNode
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowSkyNode"]));
            }
            set
            {
                this["ShowSkyNode"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowSkyOverlays
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowSkyOverlays"]));
            }
            set
            {
                this["ShowSkyOverlays"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowSkyGrids
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowSkyGrids"]));
            }
            set
            {
                this["ShowSkyGrids"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState Constellations
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["Constellations"]));
            }
            set
            {
                this["Constellations"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("4")]
        public int MultiSampling
        {
            get
            {
                return ((int)(this["MultiSampling"]));
            }
            set
            {
                this["MultiSampling"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowConstellationNames
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowConstellationNames"]));
            }
            set
            {
                this["ShowConstellationNames"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False,0,2000")]
        public global::TerraViewer.BlendState ShowGrid
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowGrid"]));
            }
            set
            {
                this["ShowGrid"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True,0,2000")]
        public global::TerraViewer.BlendState ShowEarthSky
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowEarthSky"]));
            }
            set
            {
                this["ShowEarthSky"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public global::TerraViewer.BlendState ShowEclipticOverviewText
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowEclipticOverviewText"]));
            }
            set
            {
                this["ShowEclipticOverviewText"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public global::TerraViewer.BlendState ShowSkyOverlaysIn3d
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowSkyOverlaysIn3d"]));
            }
            set
            {
                this["ShowSkyOverlaysIn3d"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("636")]
        public int MidiEditWindowHeight
        {
            get
            {
                return ((int)(this["MidiEditWindowHeight"]));
            }
            set
            {
                this["MidiEditWindowHeight"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("784, 598")]
        public Size MidiEditClientSize
        {
            get
            {
                return ((Size)(this["MidiEditClientSize"]));
            }
            set
            {
                this["MidiEditClientSize"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("100, 140")]
        public Point MidiWindowLocation
        {
            get
            {
                return ((Point)(this["MidiWindowLocation"]));
            }
            set
            {
                this["MidiWindowLocation"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("200, 200")]
        public Point ClientNodeListLocation
        {
            get
            {
                return ((Point)(this["ClientNodeListLocation"]));
            }
            set
            {
                this["ClientNodeListLocation"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("190, 580")]
        public Size ClientNodeListSize
        {
            get
            {
                return ((Size)(this["ClientNodeListSize"]));
            }
            set
            {
                this["ClientNodeListSize"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool ShowClientNodeDetails
        {
            get
            {
                return ((bool)(this["ShowClientNodeDetails"]));
            }
            set
            {
                this["ShowClientNodeDetails"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool CloudMap8k
        {
            get
            {
                return ((bool)(this["CloudMap8k"]));
            }
            set
            {
                this["CloudMap8k"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ScreenOverlayShowLocal
        {
            get
            {
                return ((bool)(this["ScreenOverlayShowLocal"]));
            }
            set
            {
                this["ScreenOverlayShowLocal"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool AutoSyncTours
        {
            get
            {
                return ((bool)(this["AutoSyncTours"]));
            }
            set
            {
                this["AutoSyncTours"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False,0,2000")]
        public global::TerraViewer.BlendState ShowReticle
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["ShowReticle"]));
            }
            set
            {
                this["ShowReticle"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public double ReticleAlt
        {
            get
            {
                return ((double)(this["ReticleAlt"]));
            }
            set
            {
                this["ReticleAlt"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public double ReticleAz
        {
            get
            {
                return ((double)(this["ReticleAz"]));
            }
            set
            {
                this["ReticleAz"] = value;
            }
        }

        [ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("1")]
        public double TerrainScaling
        {
            get
            {
                return ((double)(this["TerrainScaling"]));
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public global::TerraViewer.BlendState EarthCutawayView
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["EarthCutawayView"]));
            }
            set
            {
                this["EarthCutawayView"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool FrameSync
        {
            get
            {
                return ((bool)(this["FrameSync"]));
            }
            set
            {
                this["FrameSync"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("60")]
        public int TargetFrameRate
        {
            get
            {
                return ((int)(this["TargetFrameRate"]));
            }
            set
            {
                this["TargetFrameRate"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ShowClientNodeList
        {
            get
            {
                return ((bool)(this["ShowClientNodeList"]));
            }
            set
            {
                this["ShowClientNodeList"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string MasterHostIPOverride
        {
            get
            {
                return ((string)(this["MasterHostIPOverride"]));
            }
            set
            {
                this["MasterHostIPOverride"] = value;
            }
        }

        [ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int ScreenHitTestOffsetX
        {
            get
            {
                return ((int)(this["ScreenHitTestOffsetX"]));
            }
        }

        [ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public int ScreenHitTestOffsetY
        {
            get
            {
                return ((int)(this["ScreenHitTestOffsetY"]));
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public float DomeAngle
        {
            get
            {
                return ((float)(this["DomeAngle"]));
            }
            set
            {
                this["DomeAngle"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("15")]
        public int TileThrottling
        {
            get
            {
                return ((int)(this["TileThrottling"]));
            }
            set
            {
                this["TileThrottling"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("default.wtml")]
        public string ConstellationArtFile
        {
            get
            {
                return ((string)(this["ConstellationArtFile"]));
            }
            set
            {
                this["ConstellationArtFile"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("-1,-1,-1")]
        public global::TerraViewer.ConstellationFilter ConstellationArtFilter
        {
            get
            {
                return ((global::TerraViewer.ConstellationFilter)(this["ConstellationArtFilter"]));
            }
            set
            {
                this["ConstellationArtFilter"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("-1,-1,-1")]
        public global::TerraViewer.ConstellationFilter ConstellationFiguresFilter
        {
            get
            {
                return ((global::TerraViewer.ConstellationFilter)(this["ConstellationFiguresFilter"]));
            }
            set
            {
                this["ConstellationFiguresFilter"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("-1,-1,-1")]
        public global::TerraViewer.ConstellationFilter ConstellationBoundariesFilter
        {
            get
            {
                return ((global::TerraViewer.ConstellationFilter)(this["ConstellationBoundariesFilter"]));
            }
            set
            {
                this["ConstellationBoundariesFilter"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("-1,-1,-1")]
        public global::TerraViewer.ConstellationFilter ConstellationNamesFilter
        {
            get
            {
                return ((global::TerraViewer.ConstellationFilter)(this["ConstellationNamesFilter"]));
            }
            set
            {
                this["ConstellationNamesFilter"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string SavedFilters
        {
            get
            {
                return ((string)(this["SavedFilters"]));
            }
            set
            {
                this["SavedFilters"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("64, 255, 255, 255")]
        public Color ConstellationArtColor
        {
            get
            {
                return ((Color)(this["ConstellationArtColor"]));
            }
            set
            {
                this["ConstellationArtColor"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool UnconstrainedTilt
        {
            get
            {
                return ((bool)(this["UnconstrainedTilt"]));
            }
            set
            {
                this["UnconstrainedTilt"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool FaceNorth
        {
            get
            {
                return ((bool)(this["FaceNorth"]));
            }
            set
            {
                this["FaceNorth"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ShowSlideNumbers
        {
            get
            {
                return ((bool)(this["ShowSlideNumbers"]));
            }
            set
            {
                this["ShowSlideNumbers"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0, 500")]
        public Point KeyPropsLocation
        {
            get
            {
                return ((Point)(this["KeyPropsLocation"]));
            }
            set
            {
                this["KeyPropsLocation"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool FloatTimeline
        {
            get
            {
                return ((bool)(this["FloatTimeline"]));
            }
            set
            {
                this["FloatTimeline"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool XboxCustomMapping
        {
            get
            {
                return ((bool)(this["XboxCustomMapping"]));
            }
            set
            {
                this["XboxCustomMapping"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool XboxModeMapping
        {
            get
            {
                return ((bool)(this["XboxModeMapping"]));
            }
            set
            {
                this["XboxModeMapping"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool ShowISSModel
        {
            get
            {
                return ((bool)(this["ShowISSModel"]));
            }
            set
            {
                this["ShowISSModel"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("180")]
        public double FisheyeAngle
        {
            get
            {
                return ((double)(this["FisheyeAngle"]));
            }
            set
            {
                this["FisheyeAngle"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False,0,2000")]
        public global::TerraViewer.BlendState SolarSystemCMB
        {
            get
            {
                return ((global::TerraViewer.BlendState)(this["SolarSystemCMB"]));
            }
            set
            {
                this["SolarSystemCMB"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool NavigationHold
        {
            get
            {
                return ((bool)(this["NavigationHold"]));
            }
            set
            {
                this["NavigationHold"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("255")]
        public int MinorPlanetsFilter
        {
            get
            {
                return ((int)(this["MinorPlanetsFilter"]));
            }
            set
            {
                this["MinorPlanetsFilter"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("2147483647")]
        public int PlanetOrbitsFilter
        {
            get
            {
                return ((int)(this["PlanetOrbitsFilter"]));
            }
            set
            {
                this["PlanetOrbitsFilter"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool MilkyWayModel
        {
            get
            {
                return ((bool)(this["MilkyWayModel"]));
            }
            set
            {
                this["MilkyWayModel"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool GalacticMode
        {
            get
            {
                return ((bool)(this["GalacticMode"]));
            }
            set
            {
                this["GalacticMode"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("True")]
        public bool Show3dCities
        {
            get
            {
                return ((bool)(this["Show3dCities"]));
            }
            set
            {
                this["Show3dCities"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string RefreshToken
        {
            get
            {
                return ((string)(this["RefreshToken"]));
            }
            set
            {
                this["RefreshToken"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool RiftMonoMode
        {
            get
            {
                return ((bool)(this["RiftMonoMode"]));
            }
            set
            {
                this["RiftMonoMode"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool RiftStartup
        {
            get
            {
                return ((bool)(this["RiftStartup"]));
            }
            set
            {
                this["RiftStartup"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public double PerspectiveOffsetX
        {
            get
            {
                return ((double)(this["PerspectiveOffsetX"]));
            }
            set
            {
                this["PerspectiveOffsetX"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("0")]
        public double PerspectiveOffsetY
        {
            get
            {
                return ((double)(this["PerspectiveOffsetY"]));
            }
            set
            {
                this["PerspectiveOffsetY"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("False")]
        public bool OverSampleTerrain
        {
            get
            {
                return ((bool)(this["OverSampleTerrain"]));
            }
            set
            {
                this["OverSampleTerrain"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("1")]
        public double SwingScaleFront
        {
            get
            {
                return ((double)(this["SwingScaleFront"]));
            }
            set
            {
                this["SwingScaleFront"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("1")]
        public double SwingScaleBack
        {
            get
            {
                return ((double)(this["SwingScaleBack"]));
            }
            set
            {
                this["SwingScaleBack"] = value;
            }
        }
    }
}
