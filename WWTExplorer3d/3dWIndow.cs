
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using TerraViewer.org.worldwidetelescope.www;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Diagnostics;
using AstroCalc;
//using Microsoft.Win32;
using System.Reflection;
using System.Globalization;
using System.Text;
using TerraViewer.edu.stsci.masthla;
using Microsoft.XInput;
using WwtDataUtils;
using System.Net.NetworkInformation;
using RegistryKey = Microsoft.Win32.RegistryKey;
using Registry = Microsoft.Win32.Registry;
using System.Security.Permissions;
using MSAuth;
using System.Threading.Tasks;
using OculusWrap;

namespace TerraViewer
{
    public class Earth3d : Form, IScriptable
    {
        const float FOVMULT = 343.774f;
       

        private System.Windows.Forms.Timer timer;
        private ToolStripMenuItem viewOverlayTopo;
        private System.Windows.Forms.ToolStripMenuItem menuItem7;

        DataSetManager dsm;

        public RenderContext11 RenderContext11
        {
            get
            {
                return RenderEngine.RenderContext11;
            }
            set
            {
                RenderEngine.RenderContext11 = value;
            }
        }


        public RenderEngine RenderEngine = new RenderEngine();

        public static bool NoStealFocus = false;


        public bool SandboxMode
        {
            get
            {
                if (CurrentImageSet == null)
                {
                    return false;
                }

                return CurrentImageSet.DataSetType == ImageSetType.Sandbox;
            }
        }

        public bool SolarSystemMode
        {
            get
            {
                if (CurrentImageSet == null)
                {
                    return false;
                }

                return CurrentImageSet.DataSetType == ImageSetType.SolarSystem;
            }
        }

        private System.ComponentModel.IContainer components;
        static bool pause = false;

        public event EventHandler ImageSetChanged;

        

        public IImageSet CurrentImageSet
        {
            get { return RenderEngine.currentImageSetfield; }
            set
            {
                if (RenderEngine.currentImageSetfield != value)
                {
                    bool solarSytemOld = (RenderEngine.currentImageSetfield != null && RenderEngine.currentImageSetfield.DataSetType == ImageSetType.SolarSystem);
                    RenderEngine.currentImageSetfield = value;

                    if (RenderEngine.currentImageSetfield.DataSetType == ImageSetType.SolarSystem && !solarSytemOld)
                    {
                        if (contextPanel != null)
                        {
                            contextPanel.Constellation = "Error";
                        }
                    }
                    if (ImageSetChanged != null)
                    {
                        ImageSetChanged.Invoke(this, new EventArgs());
                        if (imageStackVisible)
                        {
                            stack.UpdateList();
                        }
                    }
                    if (value != null)
                    {
                        int hash = value.GetHash();
                        RenderEngine.AddImageSetToTable(hash, value);
                    }
                }
            }
        }



        private ToolStripMenuItem toggleFullScreenModeF11ToolStripMenuItem;
        private ToolStripMenuItem nEDSearchToolStripMenuItem;
        private ToolStripMenuItem sDSSSearchToolStripMenuItem;
        private ToolStripMenuItem vOTableToolStripMenuItem;
        private ToolStripMenuItem vORegistryToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem stereoToolStripMenuItem;
        private ToolStripMenuItem enabledToolStripMenuItem;
        private ToolStripMenuItem anaglyphToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator20;
        private ToolStripMenuItem sideBySideProjectionToolStripMenuItem;
        private ToolStripMenuItem sideBySideCrossEyedToolStripMenuItem;
        private ToolStripMenuItem expermentalToolStripMenuItem;
        private ToolStripMenuItem fullDomeToolStripMenuItem;
        private ToolStripMenuItem lookUpOnNEDToolStripMenuItem;
        private ToolStripMenuItem domeSetupToolStripMenuItem;
        private ToolStripMenuItem anaglyphYellowBlueToolStripMenuItem;
        private ToolStripMenuItem listenUpBoysToolStripMenuItem;
        private ToolStripMenuItem sAMPToolStripMenuItem;
        private ToolStripMenuItem sendImageToToolStripMenuItem;
        private ToolStripMenuItem broadcastToolStripMenuItem;
        private ToolStripMenuItem sendTableToToolStripMenuItem;
        private ToolStripMenuItem broadcastToolStripMenuItem1;
        private ToolStripMenuItem imageStackToolStripMenuItem;
        private ToolStripMenuItem addToImageStackToolStripMenuItem;
        private ToolStripMenuItem startupToolStripMenuItem;
        private ToolStripMenuItem earthToolStripMenuItem;
        private ToolStripMenuItem skyToolStripMenuItem;
        private ToolStripMenuItem planetToolStripMenuItem;
        private ToolStripMenuItem solarSystemToolStripMenuItem;
        private ToolStripMenuItem panoramaToolStripMenuItem;
        private ToolStripMenuItem lastToolStripMenuItem;
        private ToolStripMenuItem randomToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem5;
        private ToolStripMenuItem musicAndOtherTourResourceToolStripMenuItem;
        private ToolStripMenuItem lookUpOnSDSSToolStripMenuItem;
        private ToolStripMenuItem detachMainViewToSecondMonitor;
        private ToolStripMenuItem shapeFileToolStripMenuItem;
        private ToolStripMenuItem showLayerManagerToolStripMenuItem;
        private ToolStripMenuItem regionalDataCacheToolStripMenuItem;
        private ToolStripMenuItem addAsNewLayerToolStripMenuItem;
        private ToolStripMenuItem addCollectionAsTourStopsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem8;
        private ToolStripMenuItem multiChanelCalibrationToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripMenuItem sendLayersToProjectorServersToolStripMenuItem;
        private ToolStripMenuItem renderToVideoToolStripMenuItem;
        private ToolStripMenuItem showTouchControlsToolStripMenuItem;
        private KioskTitleBar kioskTitleBar;
        private ToolStripMenuItem saveCurrentViewImageToFileToolStripMenuItem;
        private ToolStripMenuItem remoteAccessControlToolStripMenuItem;
        private ToolStripMenuItem layerManagerToolStripMenuItem;
        private ToolStripMenuItem screenBroadcastToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator21;
        private ToolStripMenuItem monochromeStyleToolStripMenuItem;
        private ToolStripMenuItem layersToolStripMenuItem;
        private ToolStripMenuItem publishTourToCommunityToolStripMenuItem;
        private ToolStripMenuItem findEarthBasedLocationToolStripMenuItem;
        private ToolStripMenuItem fullDomePreviewToolStripMenuItem;
        private Timer DeviceHeartbeat;
        private ToolStripMenuItem mIDIControllerSetupToolStripMenuItem;
        private ToolStripMenuItem multiSampleAntialiasingToolStripMenuItem;
        private ToolStripMenuItem noneToolStripMenuItem;
        private ToolStripMenuItem fourSamplesToolStripMenuItem;
        private ToolStripMenuItem eightSamplesToolStripMenuItem;
        private ToolStripMenuItem sendTourToProjectorServersToolStripMenuItem;
        private ToolStripMenuItem clientNodeListToolStripMenuItem;
        private ToolStripMenuItem restoreCacheFromCabinetFileToolStripMenuItem;
        private ToolStripMenuItem saveCacheAsCabinetFileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator22;
        private ToolStripMenuItem cacheManagementToolStripMenuItem1;
        private ToolStripMenuItem cacheImageryTilePyramidToolStripMenuItem;
        private ToolStripMenuItem showCacheSpaceUsedToolStripMenuItem;
        private ToolStripMenuItem removeFromImageCacheToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator23;
        private ToolStripMenuItem automaticTourSyncWithProjectorServersToolStripMenuItem;
        private ToolStripMenuItem alternatingLinesOddToolStripMenuItem;
        private ToolStripMenuItem alternatingLinesEvenToolStripMenuItem;
        private ToolStripMenuItem lockVerticalSyncToolStripMenuItem;
        private ToolStripMenuItem targetFrameRateToolStripMenuItem;
        private ToolStripMenuItem fPSToolStripMenuItem60;
        private ToolStripMenuItem fPSToolStripMenuItem30;
        private ToolStripMenuItem fpsToolStripMenuItemUnlimited;
        private ToolStripMenuItem fPSToolStripMenuItem24;
        private ToolStripMenuItem showOverlayListToolStripMenuItem;
        private ToolStripMenuItem tileLoadingThrottlingToolStripMenuItem;
        private ToolStripMenuItem tpsToolStripMenuItem15;
        private ToolStripMenuItem tpsToolStripMenuItem30;
        private ToolStripMenuItem tpsToolStripMenuItem60;
        private ToolStripMenuItem tpsToolStripMenuItem120;
        private ToolStripMenuItem tpsToolStripMenuItemUnlimited;
        private ToolStripMenuItem oculusRiftToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem11;
        private ToolStripMenuItem detachMainViewToThirdMonitorToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem10;
        private ToolStripMenuItem allowUnconstrainedTiltToolStripMenuItem;
        private ToolStripMenuItem showSlideNumbersToolStripMenuItem;
        private ToolStripMenuItem showKeyframerToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem12;
        private ToolStripSeparator toolStripMenuItem13;
        private ToolStripMenuItem xBoxControllerSetupToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem14;
        private ToolStripMenuItem ShowWelcomeTips;
        private ToolStripMenuItem customGalaxyFileToolStripMenuItem;
        private ToolStripMenuItem newFullDomeViewInstanceToolStripMenuItem;
        private ToolStripMenuItem monitorOneToolStripMenuItem;
        private ToolStripMenuItem monitorTwoToolStripMenuItem;
        private ToolStripMenuItem monitorThreeToolStripMenuItem;
        private ToolStripMenuItem monitorFourToolStripMenuItem;
        private ToolStripMenuItem monitorFiveToolStripMenuItem;
        private ToolStripMenuItem monitorSixToolStripMenuItem;
        private ToolStripMenuItem monitorSevenToolStripMenuItem;
        private ToolStripMenuItem monitorEightToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem15;
        private ToolStripMenuItem exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem;
        private ToolStripMenuItem oculusVRHeadsetToolStripMenuItem;
        private ToolStripMenuItem monoModeToolStripMenuItem;
        private ToolStripMenuItem startInOculusModeToolStripMenuItem;
        private ToolStripMenuItem DownloadMPC;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem exportCurrentCitiesViewAs3DMeshToolStripMenuItem;
        private ToolStripMenuItem enableExport3dCitiesModeToolStripMenuItem;
 

        public void StartFadeTransition(double milliseconds)
        {
            RenderEngine.Render();
            if (milliseconds > 0)
            {
                RenderEngine.fadeImageSet.DelayTime = milliseconds;
            }
            RenderEngine.fadeImageSet.State = true;
            RenderEngine.fadeImageSet.TargetState = false;
        }

       

        public static bool IsLoggedIn
        {
            get
            {
                return !String.IsNullOrEmpty(Properties.Settings.Default.LiveIdToken);
            }
        }

     
        bool smoothZoom = true;
        private Timer InputTimer;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem nameToolStripMenuItem;
        public TerraViewer.MenuTabs menuTabs;
        private Timer HoverTimer;
        private ContextMenuStrip communitiesMenu;
        private ContextMenuStrip searchMenu;
        private ContextMenuStrip toursMenu;
        private ContextMenuStrip telescopeMenu;
        private ContextMenuStrip exploreMenu;
        private ContextMenuStrip settingsMenu;
        private ContextMenuStrip viewMenu;
        private ToolStripMenuItem openFileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem exitMenuItem;
        private ToolStripMenuItem createNewObservingListToolStripMenuItem;
        private ToolStripMenuItem newObservingListpMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem newSimpleTourMenuItem;
        private ToolStripMenuItem openTourMenuItem;
        private ToolStripMenuItem openObservingListMenuItem;
        private ToolStripMenuItem openImageMenuItem;
        private ToolStripMenuItem openKMLMenuItem;
        private ToolStripMenuItem tourHomeMenuItem;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem tourSearchWebPageMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem homepageMenuItem;
        private ToolStripMenuItem aboutMenuItem;
        private ToolStripMenuItem publishTourMenuItem;
        private ToolStripMenuItem joinCoomunityMenuItem;
        private ToolStripMenuItem updateLoginCredentialsMenuItem;
        private ToolStripMenuItem logoutMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem uploadObservingListToCommunityMenuItem;
        private ToolStripMenuItem uploadImageToCommunityMenuItem;
        private ToolStripMenuItem resetCameraMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem slewTelescopeMenuItem;
        private ToolStripMenuItem centerTelescopeMenuItem;
        private ToolStripMenuItem SyncTelescopeMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem connectTelescopeMenuItem;
        private ToolStripMenuItem trackScopeMenuItem;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripMenuItem parkTelescopeMenuItem;
        private ToolStripSeparator toolStripSeparator13;
        private ToolStripMenuItem ASCOMPlatformHomePage;
        private ToolStripMenuItem chooseTelescopeMenuItem;
        private ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator14;
        public Timer StatupTimer;
        private ToolStripMenuItem sIMBADSearchToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem feedbackToolStripMenuItem;
        private ToolStripMenuItem createANewTourToolStripMenuItem;
        private ToolStripMenuItem editTourToolStripMenuItem;
        private ToolStripMenuItem copyCurrentViewToClipboardToolStripMenuItem;
        private ToolStripMenuItem showFinderToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripSeparator toolStripSeparator15;
        private ToolStripMenuItem informationToolStripMenuItem;
        private ToolStripMenuItem lookupOnSimbadToolStripMenuItem;
        private ToolStripMenuItem propertiesToolStripMenuItem;
        private ToolStripMenuItem lookupOnSEDSToolStripMenuItem;
        private ToolStripMenuItem lookupOnWikipediaToolStripMenuItem;
        private ToolStripMenuItem publicationsToolStripMenuItem;
        private ToolStripMenuItem imageryToolStripMenuItem;
        private ToolStripMenuItem getDSSImageToolStripMenuItem;
        private ToolStripMenuItem getSDSSImageToolStripMenuItem;
        private ToolStripMenuItem getDSSFITSToolStripMenuItem;
        private ToolStripMenuItem virtualObservatorySearchesToolStripMenuItem;
        private ToolStripMenuItem uSNONVOConeSearchToolStripMenuItem;
        private ToolStripMenuItem restoreDefaultsToolStripMenuItem;
        private ToolStripMenuItem advancedToolStripMenuItem;
        private ToolStripMenuItem downloadQueueToolStripMenuItem;
        private ToolStripMenuItem startQueueToolStripMenuItem;
        private ToolStripMenuItem stopQueueToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripSeparator toolStripSeparator16;
        private ToolStripMenuItem showPerformanceDataToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripSeparator toolStripSeparator18;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripMenuItem flushCacheToolStripMenuItem;
        private RenderTarget renderWindow;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem autoRepeatToolStripMenuItem;
        private ToolStripMenuItem playCollectionAsSlideShowToolStripMenuItem;
        private Timer SlideAdvanceTimer;
        private ToolStripMenuItem oneToolStripMenuItem;
        private ToolStripMenuItem allToolStripMenuItem;
        private ToolStripMenuItem offToolStripMenuItem;
        private ToolStripMenuItem addToCollectionsToolStripMenuItem;
        private ToolStripMenuItem newCollectionToolStripMenuItem;
        private ToolStripMenuItem removeFromCollectionToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private Timer TourEndCheck;
        private ToolStripMenuItem gettingStarteMenuItem;
        private ToolStripMenuItem copyShortcutToolStripMenuItem;
        private Timer autoSaveTimer;
        private ToolStripMenuItem hLAFootprintsToolStripMenuItem;
        private ToolStripMenuItem copyShortCutToThisViewToClipboardToolStripMenuItem;

        public RenderTarget RenderWindow
        {
            get { return renderWindow; }
            set { renderWindow = value; }
        }


        private ToolStripMenuItem setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem;
        private ToolStripMenuItem selectLanguageToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripMenuItem saveTourAsToolStripMenuItem;
        private ToolStripMenuItem setAsForegroundImageryToolStripMenuItem;
        private ToolStripMenuItem setAsBackgroundImageryToolStripMenuItem;
        private ToolStripSeparator ImagerySeperator;


        public bool ControllerConnected()
        {
            XInputState state = new XInputState();
            if (XInputMethods.GetState(0, out state))
            {
                return true;
            }

            return false;

        }


        bool JoyInMotion = false;

        bool rSholderDown = false;
        bool lSholderDown = false;
        bool startDown = false;
        bool backDown = false;
        bool aDown = false;
        bool bDown = false;
        bool xDown = false;
        bool yDown = false;
        bool rightThumbDown = false;
        bool leftThumbDown = false;
        bool dPadUpDown = false;
        bool dPadDownDown = false;
        bool dPadLeftDown = false;
        bool dPadRightDown = false;
        bool slowRates = true;

        bool reticleControl = false;
        int retId = 0;
        public void UpdateXInputState()
        {

            if (Properties.Settings.Default.XboxCustomMapping)
            {
                ProcessCustomXboxMapping();
                return;
            }

            // lastFrameTime gives us fraction of seconds for update of motion factor for zooms
            double factor = RenderEngine.lastFrameTime / (1.0 / 60.0);
            JoyInMotion = false;
            XInputState state;
            try
            {
                if (!XInputMethods.GetState(0, out state))
                {
                    return;
                }
            }
            catch
            {
                return;
            }
            double trigger = 0;

            if (state.Gamepad.RightTrigger > 0)
            {
                trigger = -state.Gamepad.RightTrigger;
            }
            else
            {
                trigger = +state.Gamepad.LeftTrigger;
            }

            if (Math.Abs(trigger) > 4)
            {
                RenderEngine.ZoomFactor = RenderEngine.TargetZoom = RenderEngine.ZoomFactor * (1 + (trigger / 16000) * factor);

                if (RenderEngine.ZoomFactor > RenderEngine.ZoomMax)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom = RenderEngine.ZoomMax;
                }

                if (RenderEngine.ZoomFactor < RenderEngine.ZoomMin)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom = RenderEngine.ZoomMin;
                }
                JoyInMotion = true;
            }

            if (state.Gamepad.IsDPadRightButtonDown)
            {
                if (SpaceTimeController.TimeRate > .9)
                {
                    SpaceTimeController.TimeRate *= 1.1;
                }
                else if (SpaceTimeController.TimeRate < -1)
                {
                    SpaceTimeController.TimeRate /= 1.1;
                }
                else
                {
                    SpaceTimeController.TimeRate = 1.0;
                }

                if (SpaceTimeController.TimeRate > 1000000000)
                {
                    SpaceTimeController.TimeRate = 1000000000;
                }
                JoyInMotion = true;
            }

            if (state.Gamepad.IsDPadLeftButtonDown)
            {
                if (SpaceTimeController.TimeRate < -.9)
                {
                    SpaceTimeController.TimeRate *= 1.1;
                }
                else if (SpaceTimeController.TimeRate > 1)
                {
                    SpaceTimeController.TimeRate /= 1.1;
                }
                else
                {
                    SpaceTimeController.TimeRate = -1.0;
                }

                if (SpaceTimeController.TimeRate < -1000000000)
                {
                    SpaceTimeController.TimeRate = -1000000000;
                }
            }

            if ((state.Gamepad.Buttons & XInputButtons.LeftThumb) == XInputButtons.LeftThumb)
            {
                if (!leftThumbDown)
                {
                    leftThumbDown = true;
                    CameraParameters camParams = new CameraParameters(0, 0, 360, 0, 0, 100);
                    RenderEngine.GotoTarget(camParams, false, false);

                }
            }
            else
            {
                leftThumbDown = false;
            }



            if ((state.Gamepad.Buttons & XInputButtons.RightThumb) == XInputButtons.RightThumb)
            {
                if (!rightThumbDown)
                {
                    rightThumbDown = true;
                    reticleControl = !reticleControl;

                    if (reticleControl)
                    {
                        Reticle.Show(retId, false);
                    }
                    else
                    {
                        Reticle.Hide(retId, false);
                    }

                    Properties.Settings.Default.ShowReticle.TargetState = reticleControl;
                }
            }
            else
            {
                rightThumbDown = false;
            }



            if (state.Gamepad.IsStartButtonDown)
            {
                if (!startDown)
                {
                    NextMode();
                    startDown = true;

                }
            }
            else
            {
                startDown = false;
            }

            if (state.Gamepad.IsBackButtonDown)
            {
                if (!backDown)
                {
                    PreviousMode();
                    backDown = true;

                }
            }
            else
            {
                backDown = false;
            }

            if (state.Gamepad.IsDPadUpButtonDown)
            {
                SpaceTimeController.TimeRate = 0;
            }

            if (state.Gamepad.IsDPadDownButtonDown)
            {
                SpaceTimeController.TimeRate = 1;
                SpaceTimeController.SyncTime();
                SpaceTimeController.SyncToClock = true;
            }

            double zoomRate = slowRates ? 16000 : 8000;
            double moveRate = slowRates ? 8000 : 2000;

            if (!reticleControl)
            {
                if (Math.Abs((double)state.Gamepad.RightThumbX) > 8000)
                {
                    RenderEngine.CameraRotateTarget = (RenderEngine.CameraRotateTarget + ((((double)state.Gamepad.RightThumbX / (zoomRate * 100)) * factor)));
                }

                if (Math.Abs((double)state.Gamepad.RightThumbY) > 8000)
                {
                    RenderEngine.CameraAngleTarget = (RenderEngine.CameraAngleTarget + ((((double)state.Gamepad.RightThumbY / (zoomRate * 100)) * factor)));
                }
            }
            else
            {
                if (!Reticle.Reticles.ContainsKey(retId))
                {
                    Reticle.Set(retId, 0, 0, Color.Red);
                }


                if (Math.Abs((double)state.Gamepad.RightThumbX) > 7000)
                {
                    Reticle reticle = Reticle.Reticles[retId];
                    reticle.Az = (reticle.Az - ((((double)state.Gamepad.RightThumbX / (zoomRate * 3)) * factor)) / Math.Cos(reticle.Alt * Math.PI / 180));
                }

                if (Math.Abs((double)state.Gamepad.RightThumbY) > 7000)
                {
                    Reticle reticle = Reticle.Reticles[retId];
                    reticle.Alt = (reticle.Alt + ((((double)state.Gamepad.RightThumbY / (zoomRate * 3)) * factor)));
                }
            }



            if (RenderEngine.CameraAngleTarget < TiltMin)
            {
                RenderEngine.CameraAngleTarget = TiltMin;
            }

            if (RenderEngine.CameraAngleTarget > 0)
            {
                RenderEngine.CameraAngleTarget = 0;
            }

            if (Math.Abs((double)state.Gamepad.LeftThumbX) > 8000 || Math.Abs((double)state.Gamepad.LeftThumbY) > 8000)
            {
                MoveView(((double)state.Gamepad.LeftThumbX / moveRate) * factor, ((double)state.Gamepad.LeftThumbY / moveRate) * factor, false);
                JoyInMotion = true;
            }

            if (state.Gamepad.IsRightShoulderButtonDown)
            {
                // Edge trigger
                if (!rSholderDown)
                {
                    contextPanel.ShowNextObject();
                    rSholderDown = true;
                }
            }
            else
            {
                rSholderDown = false;
            }

            if (state.Gamepad.IsLeftShoulderButtonDown)
            {
                // Edge trigger
                if (!lSholderDown)
                {
                    contextPanel.ShowPreviousObject();
                    lSholderDown = true;
                }
            }
            else
            {
                lSholderDown = false;
            }

            if (state.Gamepad.IsXButtonDown)
            {
                if (!xDown)
                {
                    xDown = true;
                    switch (CurrentImageSet.DataSetType)
                    {
                        case ImageSetType.Earth:
                            Properties.Settings.Default.ShowClouds.TargetState = !Properties.Settings.Default.ShowClouds.TargetState;
                            break;
                        case ImageSetType.Planet:
                            break;
                        case ImageSetType.Sky:
                            Properties.Settings.Default.ShowEcliptic.TargetState = !Properties.Settings.Default.ShowEcliptic.TargetState;
                            break;
                        case ImageSetType.Panorama:
                            break;
                        case ImageSetType.SolarSystem:
                            {
                                if (Properties.Settings.Default.SolarSystemOrbits.TargetState)
                                {
                                    if (Properties.Settings.Default.SolarSystemMinorOrbits.TargetState)
                                    {
                                        Properties.Settings.Default.SolarSystemOrbits.TargetState = false;
                                        Properties.Settings.Default.SolarSystemMinorOrbits.TargetState = false;
                                    }
                                    else
                                    {
                                        Properties.Settings.Default.SolarSystemMinorOrbits.TargetState = true;
                                    }
                                }
                                else
                                {
                                    Properties.Settings.Default.SolarSystemOrbits.TargetState = true;
                                    Properties.Settings.Default.SolarSystemMinorOrbits.TargetState = false;
                                }

                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                xDown = false;
            }

            if (state.Gamepad.IsYButtonDown)
            {
                if (!yDown)
                {
                    yDown = true;
                    switch (CurrentImageSet.DataSetType)
                    {
                        case ImageSetType.Earth:
                            Properties.Settings.Default.ShowClouds.TargetState = !Properties.Settings.Default.ShowClouds.TargetState;
                            break;
                        case ImageSetType.Planet:
                            break;
                        case ImageSetType.Sky:
                            Properties.Settings.Default.ShowConstellationFigures.TargetState = !Properties.Settings.Default.ShowConstellationFigures.TargetState;
                            break;
                        case ImageSetType.Panorama:
                            break;
                        case ImageSetType.SolarSystem:
                            Properties.Settings.Default.SolarSystemStars.TargetState = !Properties.Settings.Default.SolarSystemStars.TargetState;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                yDown = false;
            }


            if (state.Gamepad.IsAButtonDown)
            {
                if (!aDown)
                {
                    aDown = true;
                    switch (CurrentImageSet.DataSetType)
                    {
                        case ImageSetType.Earth:
                            break;
                        case ImageSetType.Planet:
                            break;
                        case ImageSetType.Sky:
                            Properties.Settings.Default.ShowGrid.TargetState = !Properties.Settings.Default.ShowGrid.TargetState;
                            break;
                        case ImageSetType.Panorama:
                            break;
                        case ImageSetType.SolarSystem:
                            Properties.Settings.Default.SolarSystemMinorPlanets.TargetState = !Properties.Settings.Default.SolarSystemMinorPlanets.TargetState;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                aDown = false;
            }

            if (state.Gamepad.IsBButtonDown)
            {
                if (!bDown)
                {
                    bDown = true;
                    switch (CurrentImageSet.DataSetType)
                    {
                        case ImageSetType.Earth:
                            break;
                        case ImageSetType.Planet:
                            break;
                        case ImageSetType.Sky:
                            Properties.Settings.Default.ShowConstellationBoundries.TargetState = !Properties.Settings.Default.ShowConstellationBoundries.TargetState;
                            Properties.Settings.Default.ShowConstellationSelection.TargetState = false;
                            break;
                        case ImageSetType.Panorama:
                            break;
                        case ImageSetType.SolarSystem:
                            Properties.Settings.Default.SolarSystemMilkyWay.TargetState = !Properties.Settings.Default.SolarSystemMilkyWay.TargetState;
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                bDown = false;
            }

            return;

        }

        private void NextMode()
        {
            switch (CurrentImageSet.DataSetType)
            {
                case ImageSetType.Earth:
                    Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.Planet);
                    break;
                case ImageSetType.Planet:
                    Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.Sky);
                    break;
                case ImageSetType.Sky:
                    if (!Properties.Settings.Default.LocalHorizonMode)
                    {
                        Properties.Settings.Default.LocalHorizonMode = true;
                    }
                    else
                    {
                        Properties.Settings.Default.LocalHorizonMode = false;
                        Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.Panorama);

                    }
                    break;
                case ImageSetType.Panorama:
                    Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.SolarSystem);
                    break;
                case ImageSetType.SolarSystem:
                    Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.Earth);
                    break;
                case ImageSetType.Sandbox:
                    Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.Earth);
                    break;
                default:
                    break;
            }
        }

        private void PreviousMode()
        {
            switch (CurrentImageSet.DataSetType)
            {
                case ImageSetType.Earth:
                    Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.Sandbox);
                    break;
                case ImageSetType.Planet:
                    Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.Earth);
                    break;
                case ImageSetType.Sky:
                    if (Properties.Settings.Default.LocalHorizonMode)
                    {
                        Properties.Settings.Default.LocalHorizonMode = false;
                    }
                    else
                    {
                        Properties.Settings.Default.LocalHorizonMode = false;
                        Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.Planet);
                    }
                    break;
                case ImageSetType.Panorama:
                    Properties.Settings.Default.LocalHorizonMode = true;
                    Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.Sky);
                    break;
                case ImageSetType.SolarSystem:
                    Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.Panorama);
                    break;
                case ImageSetType.Sandbox:
                    Earth3d.MainWindow.contextPanel.SetLookAtTarget(ImageSetType.SolarSystem);
                    break;
                default:
                    break;
            }
        }

        public void ProcessCustomXboxMapping()
        {
            // lastFrameTime gives us fraction of seconds for update of motion factor for zooms
            double factor = RenderEngine.lastFrameTime / (1.0 / 60.0);
            JoyInMotion = false;
            XInputState state;
            try
            {
                if (!XInputMethods.GetState(0, out state))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            this.NetZoomRate = 0;

            if (state.Gamepad.RightTrigger > 4)
            {
                XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.RightTrigger, state.Gamepad.RightTrigger / 255.0);
            }

            if (state.Gamepad.LeftTrigger > 4)
            {
                XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.LeftTrigger, state.Gamepad.LeftTrigger / 255.0);
            }


            if (state.Gamepad.IsDPadRightButtonDown)
            {
                if (!dPadRightDown)
                {
                    dPadRightDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.DirectionPadRight, 1);
                }

            }
            else
            {
                dPadRightDown = false;
            }

            if (state.Gamepad.IsDPadLeftButtonDown)
            {
                if (!dPadLeftDown)
                {
                    dPadLeftDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.DirectionPadLeft, 1);
                }

            }
            else
            {
                dPadLeftDown = false;
            }

            if (state.Gamepad.IsLeftThumbClick)
            {
                if (!leftThumbDown)
                {
                    leftThumbDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.LeftThumbClick, 1);
                }
            }
            else
            {
                leftThumbDown = false;
            }



            if (state.Gamepad.IsRightThumbClick)
            {
                if (!rightThumbDown)
                {
                    rightThumbDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.RightThumbClick, 1);
                }
            }
            else
            {
                rightThumbDown = false;
            }



            if (state.Gamepad.IsStartButtonDown)
            {
                if (!startDown)
                {
                    startDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.Start, 1);
                }
            }
            else
            {
                startDown = false;
            }

            if (state.Gamepad.IsBackButtonDown)
            {
                if (!backDown)
                {
                    backDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.Back, 1);
                }
            }
            else
            {
                backDown = false;
            }

            if (state.Gamepad.IsDPadUpButtonDown)
            {
                if (!dPadUpDown)
                {
                    dPadUpDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.DirectionPadUp, 1);
                }
            }
            else
            {
                dPadUpDown = false;
            }

            if (state.Gamepad.IsDPadDownButtonDown)
            {
                if (!dPadDownDown)
                {
                    dPadDownDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.DirectionPadDown, 1);
                }
            }
            else
            {
                dPadDownDown = false;
            }



            if (Math.Abs((double)state.Gamepad.RightThumbX) > 8000)
            {
                XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.RightThumbX, (state.Gamepad.RightThumbX - 8000) / (32767.0 - 8000));
            }

            if (Math.Abs((double)state.Gamepad.RightThumbY) > 8000)
            {
                XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.RightThumbY, (state.Gamepad.RightThumbY - 8000) / (32767.0 - 8000));
            }

            if (Math.Abs((double)state.Gamepad.LeftThumbX) > 8000)
            {
                XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.LeftThumbX, (state.Gamepad.LeftThumbX - 8000) / (32767.0 - 8000));
            }

            if (Math.Abs((double)state.Gamepad.LeftThumbY) > 8000)
            {
                XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.LeftThumbY, (state.Gamepad.LeftThumbY - 8000) / (32767.0 - 8000));
            }

            if (state.Gamepad.IsRightShoulderButtonDown)
            {
                // Edge trigger
                if (!rSholderDown)
                {
                    rSholderDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.RightShoulder, 1);
                }
            }
            else
            {
                rSholderDown = false;
            }

            if (state.Gamepad.IsLeftShoulderButtonDown)
            {
                // Edge trigger
                if (!lSholderDown)
                {
                    lSholderDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.LeftShoulder, 1);
                }
            }
            else
            {
                lSholderDown = false;
            }

            if (state.Gamepad.IsXButtonDown)
            {
                if (!xDown)
                {
                    xDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.X, 1);
                }
            }
            else
            {
                xDown = false;
            }

            if (state.Gamepad.IsYButtonDown)
            {
                if (!yDown)
                {
                    yDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.Y, 1);
                }
            }
            else
            {
                yDown = false;
            }


            if (state.Gamepad.IsAButtonDown)
            {
                if (!aDown)
                {
                    aDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.A, 1);

                }
            }
            else
            {
                aDown = false;
            }

            if (state.Gamepad.IsBButtonDown)
            {
                if (!bDown)
                {
                    bDown = XBoxConfig.DispatchXboxEvent(XBoxConfig.XboxButtons.B, 1);

                }
            }
            else
            {
                bDown = false;
            }
        }


        Config config = null;

        public Config Config
        {
            get { return config; }
            set { config = value; }
        }
        MainMenu holder = null;
        string mainWindowText = Language.GetLocalizedText(3, "Microsoft WorldWide Telescope");

        public static bool HideSplash = false;
        public Earth3d()
        {

            AudioPlayer.Initialize();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
            if (!HideSplash)
            {
                Splash.ShowSplashScreen();
            }

            // Set the initial size of our form
            this.ClientSize = new System.Drawing.Size(400, 300);
            // And its caption



            config = new Config();

            RenderEngine.MonitorX = config.MonitorX;
            RenderEngine.MonitorY = config.MonitorY;
            RenderEngine.MonitorCountX = config.MonitorCountX;
            RenderEngine.MonitorCountY = config.MonitorCountY;
            RenderEngine.monitorHeight = config.Height;
            RenderEngine.monitorWidth = config.Width;
            RenderEngine.bezelSpacing = (float)config.Bezel;
            RenderEngine.config = config;

            RenderEngine.ProjectorServer = !config.Master;
            if (DomeViewer)
            {
                RenderEngine.ProjectorServer = true;
            }
            RenderEngine.multiMonClient = !config.Master && (RenderEngine.MonitorCountX > 1 || RenderEngine.MonitorCountY > 1);

            InitializeComponent();
            SetUiStrings();

            this.Text = Language.GetLocalizedText(3, "Microsoft WorldWide Telescope");

            if (!InitializeGraphics())
            {
                UiTools.ShowMessageBox(Language.GetLocalizedText(4, "You need 3d Graphics and DirectX installed to run this application"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
            }


            // This code is used for dumping shader code when porting to Windows RT/phone where compiling shaders is not possible at runtime
            if (DumpShaders)
            {
                //ShaderLibrary.DumpShaderLibrary();
            }




            if (!RenderEngine.InitializeImageSets())
            {
                Close();
            }


            BackInitDelegate initBackground = BackgroundInit;


            initBackground.BeginInvoke(null, null);


            if (TargetScreenId != -1)
            {
                try
                {
                    this.StartPosition = FormStartPosition.Manual;
                    Screen screenTarget;
                    screenTarget = Screen.AllScreens[TargetScreenId];

                    this.Top = screenTarget.WorkingArea.Y;
                    this.Left = screenTarget.WorkingArea.X;
                }
                catch
                {
                }
            }


        }
       
        public static void BackgroundInit()
        {
            Grids.InitStarVertexBuffer(RenderContext11.PrepDevice);
            Grids.MakeMilkyWay(RenderContext11.PrepDevice);
            Grids.InitCosmosVertexBuffer();
            Planets.InitPlanetResources();

        }

        public static void SearchInit()
        {
            Catalogs.InitSearchTable();
        }


        SpaceNavigator._3DxMouse myMouse;

        int spaceDeviceType = 0;

        bool InitSpaceNavigator()
        {
            try
            {
                myMouse = new SpaceNavigator._3DxMouse(this.Handle);
                int NumberOf3DxMice = myMouse.EnumerateDevices();

                // Setup event handlers to be called when something happens
                myMouse.MotionEvent += new SpaceNavigator._3DxMouse.MotionEventHandler(myMouse_MotionEvent);
                myMouse.ButtonEvent += new SpaceNavigator._3DxMouse.ButtonEventHandler(myMouse_ButtonEvent);
                return true;

            }
            catch
            {
                myMouse = null;
                return false;
            }
        }

        Vector3d SensorTranslation;
        Vector3d SensorRotation;

        void myMouse_ButtonEvent(object sender, SpaceNavigator._3DxMouse.ButtonEventArgs e)
        {
            CameraParameters cameraParams;
            if ((e.ButtonMask.Pressed & 1) == 1)
            {
                RenderEngine.CameraRotateTarget = 0;
            }
            if ((e.ButtonMask.Pressed & 2) == 2)
            {
                cameraParams = new CameraParameters(0, 0, 360, 0, 0, 100);
                RenderEngine.GotoTarget(cameraParams, false, false);

            }
        }

        void myMouse_MotionEvent(object sender, SpaceNavigator._3DxMouse.MotionEventArgs e)
        {
            if (e.TranslationVector != null)
            {

                // Swap axes from HID orientation to a right handed coordinate system that matches WPF model space
                SensorTranslation.X = e.TranslationVector.X;
                SensorTranslation.Y = -e.TranslationVector.Z;
                SensorTranslation.Z = e.TranslationVector.Y;
            }

            // Rotation Vector?
            if (e.RotationVector != null)
            {
                // Swap axes from HID orientation to a right handed coordinate system that matches WPF model space
                SensorRotation.X = e.RotationVector.X;
                SensorRotation.Y = -e.RotationVector.Z;
                SensorRotation.Z = e.RotationVector.Y;
            }
        }


        double sensitivity = 3000;

        public double TiltMin
        {
            get
            {
                if (Properties.Settings.Default.UnconstrainedTilt || Control.ModifierKeys == (Keys.Shift | Keys.Control))
                {
                    return -1.52 * 2;
                }
                else
                {
                    return -1.52;
                }
            }
        }

        public void UpdateSpaceNavigator()
        {
            bool interupt = false;

            double factor = RenderEngine.lastFrameTime / (1.0 / 15.0);
            double units = .15;
            try
            {
                if (Math.Abs(SensorTranslation.Y) > 0)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom = RenderEngine.ZoomFactor * (1 + ((SensorTranslation.Y / sensitivity) * factor));

                    if (RenderEngine.ZoomFactor > RenderEngine.ZoomMax)
                    {
                        RenderEngine.ZoomFactor = RenderEngine.ZoomMax;
                    }

                    if (RenderEngine.ZoomFactor < RenderEngine.ZoomMin)
                    {
                        RenderEngine.ZoomFactor = RenderEngine.ZoomMin;
                    }
                    interupt = true;
                }

                if (Math.Abs(SensorRotation.Y) > 0)
                {
                    double angle = ((((double)SensorRotation.Y / sensitivity) * factor));
                    if (!RenderEngine.PlanetLike)
                    {
                        angle = -angle;
                    }
                    RenderEngine.CameraRotateTarget = (RenderEngine.CameraRotateTarget + angle);
                    interupt = true;
                }

                if (Math.Abs(SensorRotation.X) > 0)
                {
                    double angle = ((((double)SensorRotation.X / sensitivity) * factor));

                    RenderEngine.CameraAngleTarget = (RenderEngine.CameraAngleTarget + angle);
                    if (RenderEngine.CameraAngleTarget < TiltMin)
                    {
                        RenderEngine.CameraAngleTarget = TiltMin;
                    }
                    if (RenderEngine.CameraAngleTarget > 0)
                    {
                        RenderEngine.CameraAngleTarget = 0;
                    }
                    interupt = true;
                }

                if (Math.Abs(SensorTranslation.X) > 0 || Math.Abs(SensorTranslation.Z) > 0)
                {
                    MoveView(SensorTranslation.X * factor * units, -SensorTranslation.Z * factor * units, false);
                    if (SolarSystemMode)
                    {
                        if (RenderEngine.TargetLat > 87)
                        {
                            RenderEngine.TargetLat = 87;
                        }
                        if (RenderEngine.TargetLat < -87)
                        {
                            RenderEngine.TargetLat = -87;
                        }

                    }
                    interupt = true;
                }
            }
            catch
            {
            }
            if (interupt)
            {
                UserInterupt();
            }
        }
        // User is taking control...stop automated moves
        void UserInterupt()
        {
            if (playingSlideShow)
            {
                SlideAdvanceTimer.Enabled = false;
                SlideAdvanceTimer.Enabled = true;
            }
            if (RenderEngine.Mover != null)
            {
                CameraParameters newCam = RenderEngine.Mover.CurrentPosition;

                RenderEngine.viewCamera = RenderEngine.targetViewCamera = newCam;

                RenderEngine.Mover = null;
            }
        }

        void keyboard_KeyDown(int keyCode)
        {
            CameraParameters cameraParams;
            switch (keyCode)
            {
                case 1:
                    RenderEngine.CameraRotateTarget = 0;
                    break;
                case 2:
                    cameraParams = new CameraParameters(0, 0, 360, 0, 0, 100);
                    RenderEngine.GotoTarget(cameraParams, false, false);
                    break;
            }
        }
        

        public static Earth3d MainWindow = null;

        public void CheckOSVersion()
        {
            OperatingSystem os = Environment.OSVersion;

            // Get the version information
            Version vs = os.Version;

            if (vs.Major < 6 && !Properties.Settings.Default.CheckedForFlashingVideo)
            {
                Properties.Settings.Default.TranparentWindows = false;
            }
            Properties.Settings.Default.CheckedForFlashingVideo = true;

        }

        //MSScriptControl.ScriptControlClass script = null;
        private static void UnitTestWCS()
        {
            WcsFitter fitter = new WcsFitter(686, 1024);
            fitter.AddPoint(Coordinates.FromRaDec(5.533958, -0.30028), new Vector2d(400, 254));
            fitter.AddPoint(Coordinates.FromRaDec(5.59, -5.89722), new Vector2d(258, 836));
            fitter.Solve();
        }
        private void PreRenderStage()
        {
            if (contextPanel != null)
            {
                contextPanel.QueueProgress = TileCache.QueuePercent;
            }

            UpdateSpaceNavigator();
            UpdateXInputState();
            UpdateNetControlState();
        }

        private void PostRenderStage()
        {
            if (blink)
            {
                TimeSpan ts = DateTime.Now - lastBlink;
                if (ts.TotalMilliseconds > 500)
                {
                    if (RenderEngine.StudyOpacity > 0)
                    {
                        RenderEngine.StudyOpacity = 0;
                    }
                    else
                    {
                        RenderEngine.StudyOpacity = 100;
                    }
                    lastBlink = DateTime.Now;
                }
            }
            if (Settings.MasterController)
            {
                SendMove();
            }



            if (contextPanel != null)
            {

                contextPanel.QueueProgress = TileCache.QueuePercent;

                if (RenderEngine.Space)
                {
                    contextPanel.ViewLevel = RenderEngine.FovAngle;
                    contextPanel.RA = RenderEngine.RA;
                    contextPanel.Dec = RenderEngine.Dec;

                    if (RenderEngine.constellationCheck != null)
                    {
                        RenderEngine.Constellation = RenderEngine.constellationCheck.FindConstellationForPoint(RenderEngine.RA, RenderEngine.Dec);
                        contextPanel.Constellation = Constellations.FullName(Constellation);
                    }
                }
                else if (SolarSystemMode || SandboxMode)
                {
                    if (SandboxMode)
                    {
                        contextPanel.Sandbox = true;
                        contextPanel.Distance = RenderEngine.SolarSystemCameraDistance;
                    }
                    else
                    {
                        contextPanel.Sandbox = false;
                        contextPanel.Distance = RenderEngine.SolarSystemCameraDistance;
                    }


                    if (!SandboxMode && (RenderEngine.viewCamera.Target != SolarSystemObjects.Custom && RenderEngine.viewCamera.Target != SolarSystemObjects.Undefined))
                    {
                        Vector3d pnt = Coordinates.GeoTo3dDouble(RenderEngine.ViewLat, RenderEngine.ViewLong + 90);

                        Matrix3d EarthMat = Planets.EarthMatrixInv;


                        pnt = Vector3d.TransformCoordinate(pnt, EarthMat);
                        pnt.Normalize();


                        Vector2d radec = Coordinates.CartesianToLatLng(pnt);

                        if (RenderEngine.viewCamera.Target != SolarSystemObjects.Earth)
                        {
                            if (radec.X < 0)
                            {
                                radec.X += 360;
                            }
                        }

                        contextPanel.RA = radec.X;
                        contextPanel.Dec = radec.Y;
                    }
                    else
                    {
                        contextPanel.RA = RenderEngine.ViewLong;
                        contextPanel.Dec = RenderEngine.ViewLat;
                    }
                    contextPanel.Constellation = null;
                }
                else if (RenderEngine.PlanetLike)
                {
                    contextPanel.Sandbox = false;
                    contextPanel.Distance = RenderEngine.SolarSystemCameraDistance / UiTools.KilometersPerAu * 370;
                    contextPanel.RA = RenderEngine.ViewLong;
                    contextPanel.Dec = RenderEngine.ViewLat;
                    contextPanel.Constellation = null;
                }
                else
                {
                    contextPanel.Sandbox = false;
                    contextPanel.ViewLevel = RenderEngine.FovAngle;
                    contextPanel.RA = RenderEngine.ViewLong;
                    contextPanel.Dec = RenderEngine.ViewLat;
                    contextPanel.Constellation = null;
                }
            }
        }
        private void OpacityChanged()
        {
            if (contextPanel != null)
            {
                contextPanel.studyOpacity.Value = (int)RenderEngine.viewCamera.Opacity;
            }
        }

        private void Earth3d_Load(object sender, System.EventArgs e)
        {
            CheckOSVersion();
            string path = Properties.Settings.Default.ImageSetUrl;
            RenderEngine.NotifyMoveComplete = new RenderEngine.NotifyComplete(NotifyMoveComplete);
            RenderEngine.NotifyStudyImagesetChanged = new RenderEngine.NotifyStudyChanged(StudySetChanged);
            RenderEngine.PreRenderStage = new RenderEngine.RenderNotify(PreRenderStage);
            RenderEngine.PostRenderStage = new RenderEngine.RenderNotify(PostRenderStage);
            RenderEngine.OpacityChanged = new RenderEngine.NotifyOpacityUpdate(OpacityChanged);
            RenderEngine.EndRenderStage= new RenderEngine.RenderNotify(UpdateStats);

            RenderEngine.zoomMaxSolarSystem = Properties.Settings.Default.MaxZoomLimitSolar;
            RenderEngine.zoomMinSolarSystem = Properties.Settings.Default.MinZoonLimitSolar;
            RenderEngine.config = config;

            if (Properties.Settings.Default.ImageSetUrl.ToLower().Contains("imagesetsnew"))
            {
                Properties.Settings.Default.ImageSetUrl = "http://www.worldwidetelescope.org/wwtweb/catalog.aspx?X=ImageSets5";
            }

            Earth3d.MainWindow = this;
            this.dsm = new DataSetManager();
            Constellations.Containment = RenderEngine.constellationCheck;

            ContextSearch.InitializeDatabase(true);

            var t = System.Threading.Tasks.Task.Run(() =>
            {
                Catalogs.Initializing = true;
                LoadExploreRoot();
                if (explorerRoot != null)
                {
                    ContextSearch.AddFolderToSearch(explorerRoot, true);
                }
                ContextSearch.AddCatalogs(true);
                Catalogs.LoadSearchTable();
                ContextSearch.Initialized = true;
            });


            //BackInitDelegate initBackground = SearchInit;

            //initBackground.BeginInvoke(null, null);

            this.WindowState = FormWindowState.Maximized;



            this.FormBorderStyle = TouchKiosk ? FormBorderStyle.None : FormBorderStyle.Sizable;
            TileCache.StartQueue();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            Earth3d.MainWindow.Config.DomeTilt = (float)Properties.Settings.Default.DomeTilt;
            if (RenderEngine.ProjectorServer)
            {
                ShowFullScreen(true);
                this.timer.Interval = 1000;
                this.InputTimer.Enabled = false;
                Cursor.Hide();
                Properties.Settings.Default.ShowCrosshairs = false;
                Properties.Settings.Default.SolarSystemMultiRes = true;
                NetControl.Start();

            }
            else
            {
                if (Properties.Settings.Default.ListenMode || Settings.DomeView)
                {
                    NetControl.Start();
                }
            }
            if (Settings.MasterController)
            {
                NetControl.StartStatusListner();
            }

            if (Settings.MasterController)
            {
                NetControl.LoadNodeList();
            }

            if (Earth3d.TouchKiosk)
            {
                this.menuTabs.IsVisible = false;
                this.kioskTitleBar.Visible = true;
                Properties.Settings.Default.ShowTouchControls = true;
                ShowFullScreen(true);
            }

            if (NoUi)
            {
                this.menuTabs.IsVisible = false;
                Properties.Settings.Default.ShowTouchControls = true;
                ShowFullScreen(true);
            }

            Tile.GrayscaleStyle = Properties.Settings.Default.MonochromeImageStyle;



            // This forces a init at startup does not do anything but force the static contstuctor to fire now
            LayerManager.LoadTree();

            listenUpBoysToolStripMenuItem.Checked = Properties.Settings.Default.ListenMode;
            int id = Properties.Settings.Default.StartUpLookAt;
            if (Properties.Settings.Default.StartUpLookAt == 5)
            {
                id = Properties.Settings.Default.LastLookAtMode;
            }

            if (Properties.Settings.Default.StartUpLookAt == 6)
            {
                Random rnd = new Random();
                id = rnd.Next(-1, 5);
                Properties.Settings.Default.LastLookAtMode = id;
            }

            CurrentImageSet = RenderEngine.GetDefaultImageset((ImageSetType)id, BandPass.Visible);

            Properties.Settings.Default.SettingChanging += new System.Configuration.SettingChangingEventHandler(Default_SettingChanging);
            Properties.Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Default_PropertyChanged);

            if (Properties.Settings.Default.LocalHorizonMode)
            {
                RenderEngine.ViewType = RenderEngine.ViewTypes.AltAz;
            }
            else
            {
                RenderEngine.ViewType = RenderEngine.ViewTypes.Equatorial;
            }
            InitSpaceNavigator();
            RenderEngine.ReadyToRender = true;
            Refresh();

            try
            {
                RenderEngine.Fov = new FieldOfView(Properties.Settings.Default.FovTelescope, Properties.Settings.Default.FovCamera, Properties.Settings.Default.FovEyepiece);
            }
            catch
            {
            }

            SpaceTimeController.Altitude = Properties.Settings.Default.LocationAltitude;
            SpaceTimeController.Location = Coordinates.FromLatLng(Properties.Settings.Default.LocationLat, Properties.Settings.Default.LocationLng);

            TourPlayer.TourEnded += new EventHandler(TourPlayer_TourEnded);
            if (RenderEngine.KmlMarkers == null)
            {
                RenderEngine.KmlMarkers = new KmlLabels();
            }
            RenderEngine.ReadyToRender = true;
            RenderEngine.Initialized = true;
            this.Activate();
            RenderEngine.fadeImageSet.State = false;
            RenderEngine.fadeImageSet.State = true;
            RenderEngine.fadeImageSet.TargetState = false;

            // Force settings 
            Properties.Settings.Default.ActualPlanetScale = true;
            Properties.Settings.Default.HighPercitionPlanets = true;
            Properties.Settings.Default.ShowMoonsAsPointSource = false;
            Properties.Settings.Default.ShowSolarSystem.TargetState = true;

            toolStripMenuItem2.Checked = Settings.MasterController;

            RenderEngine.viewCamera.Target = SolarSystemObjects.Sun;

            if (!RenderEngine.ProjectorServer)
            {
                webServer.Startup();

                sampConnection = new Samp();

                // Register goto
                SampMessageHandler.RegiseterMessage(new SampCoordPointAtSky(new CoordPointAtSkyDelegate(SampGoto)));
                SampMessageHandler.RegiseterMessage(new SampTableLoadVoTable(new TableLoadVoTableDelegate(SampLoadTable)));
                SampMessageHandler.RegiseterMessage(new SampImageLoadFits(new ImageLoadFitsDelegate(SampLoadFitsImage)));
                SampMessageHandler.RegiseterMessage(new SampTableHighlightRow(new TableHighlightRowDelegate(SampHighlightRow)));

                NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);

                MidiMapManager.Startup();

            }

            RenderEngine.Fader.TargetState = false;

            hold = new Text3dBatch(80);
            hold.Add(new Text3d(new Vector3d(0, 0, 1), new Vector3d(0, 1, 0), " 0hr123456789-+", 80, .0001f));
            hold.Add(new Text3d(new Vector3d(0, 0, 1), new Vector3d(0, 1, 0), "JanuyFebMcApilg", 80, .0001f));
            hold.Add(new Text3d(new Vector3d(0, 0, 1), new Vector3d(0, 1, 0), "stSmOoNvDBCEdqV", 80, .0001f));
            hold.Add(new Text3d(new Vector3d(0, 0, 1), new Vector3d(0, 1, 0), "jxGHILPRTU", 80, .0001f));
            hold.PrepareBatch();


            Text3dBatch asciiTable;
            asciiTable = new Text3dBatch(12);
            string table = "";
            for (char c = ' '; c < 127; c++)
            {
                table += c;
            }

            asciiTable.Add(new Text3d(new Vector3d(0, 0, 1), new Vector3d(0, 1, 0), table, 80, .0001f));
            asciiTable.PrepareBatch();
            asciiTable.Draw(RenderContext11, 1, Color.White);

            Constellations.InitializeConstellationNames();

            if (Properties.Settings.Default.ShowClientNodeList && !RenderEngine.ProjectorServer)
            {

                ClientNodeList.ShowNodeList();
            }

            if (DetachScreenId > -1)
            {
                FreeFloatRenderWindow(DetachScreenId);
            }

            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.RefreshToken))
            {
                WindowsLiveSignIn();
            }
            if (Properties.Settings.Default.RiftStartup)
            {
                RenderEngine.StartRift();
            }
        }
        Text3dBatch hold;
        void SampHighlightRow(string url, string id, int row)
        {
            if (Samp.sampKnownTableIds.ContainsKey(id))
            {
                VoTableLayer layer = Samp.sampKnownTableIds[id];

                if (layer.Viewer != null)
                {
                    MethodInvoker doIt = delegate
                    {
                        layer.Viewer.HighlightRow(row);
                    };

                    if (this.InvokeRequired)
                    {
                        try
                        {
                            this.Invoke(doIt);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        doIt();
                    }
                }



            }
            else if (Samp.sampKnownTableUrls.ContainsKey(url))
            {

            }

        }

        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            webServer.Shutdown();
            webServer.Startup();
        }

        void SampLoadFitsImage(string url, string id, string name)
        {
            MethodInvoker doIt = delegate
            {
                DownloadFitsImage(url);
            };

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(doIt);
                }
                catch
                {
                }
            }
            else
            {
                doIt();
            }

        }

        public void DownloadFitsImage(string url)
        {
            string filename = Path.GetTempFileName() + ".FITS";
            Cursor.Current = Cursors.WaitCursor;

            if (!FileDownload.DownloadFile(url, filename, true))
            {
                return;
            }
            try
            {
                LoadImage(filename);
            }
            catch
            {
                UiTools.ShowMessageBox(Language.GetLocalizedText(1003, "The image file did not download or is invalid."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
            }
        }


        void SampLoadTable(string url, string id, string name)
        {
            MethodInvoker doIt = delegate
            {
                RunVoSearch(url, id);
            };

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(doIt);
                }
                catch
                {
                }
            }
            else
            {
                doIt();
            }

        }
        void SampGoto(double ra, double dec)
        {
            MethodInvoker doIt = delegate
            {
                RenderEngine.GotoTargetRADec(ra, dec, true, false);
            };

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(doIt);
                }
                catch
                {
                }
            }
            else
            {
                doIt();
            }
        }

        public static MyWebServer webServer = new MyWebServer();

        public Samp sampConnection = null;
        public static int TargetScreenId = -1;
        public static int DetachScreenId = -1;
        public void ShowFullScreen(bool showFull)
        {
            // This might be useful to look into for full screen mode but can't be used with RIFT
            //RenderContext11.SetFullScreenState(showFull);
            //RenderContext11.Resize(renderWindow);
            //return;

            this.SuspendLayout();
            menuTabs.IsVisible = !showFull && !TouchKiosk;
            if (showFull)
            {
                bool doubleWide = (RenderEngine.StereoMode == RenderEngine.StereoModes.SideBySide || RenderEngine.StereoMode == RenderEngine.StereoModes.CrossEyed) && !RenderEngine.rift;
                this.FormBorderStyle = FormBorderStyle.None;
                if (doubleWide || Properties.Settings.Default.FullScreenHeight != 0)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.TopMost = true;
                }
                else
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                UiTools.ShowFullScreen(this, doubleWide, TargetScreenId);
            }
            else
            {
                this.FormBorderStyle = TouchKiosk ? FormBorderStyle.None : FormBorderStyle.Sizable;
            }
            if (showFull)
            {
                holder = this.Menu;
                this.Menu = null;
            }
            else
            {
                if (holder != null)
                {
                    this.Menu = holder;
                }
            }
            fullScreen = showFull;
            this.ResumeLayout();
            RenderContext11.Resize(renderWindow.ClientSize.Height, renderWindow.ClientSize.Width);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hwnd, StringBuilder text, int length);


        [DllImport("kernel32.dll")]
        static extern uint RegisterApplicationRecoveryCallback(IntPtr pRecoveryCallback, IntPtr pvParameter, int dwPingInterval, int dwFlags);

        [DllImport("kernel32.dll")]
        static extern uint ApplicationRecoveryInProgress(out bool pbCancelled);

        [DllImport("kernel32.dll")]
        static extern uint ApplicationRecoveryFinished(bool bSuccess);

        delegate int ApplicationRecoveryCallback(IntPtr pvParameter);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        static extern uint RegisterApplicationRestart(string pszCommandline, int dwFlags);

        public bool IsWindowOrChildFocused()
        {
            IntPtr hwndFG = GetForegroundWindow();
            bool bFocused = hwndFG == this.Handle || hwndFG == renderWindow.Handle || ((currentTab != null && currentTab.Handle == hwndFG) || (contextPanel != null && contextPanel.Handle == hwndFG));

            return bFocused;
        }

        private void ShowContextPanel()
        {
            try
            {
                if (RenderEngine.ProjectorServer || NoUi)
                {
                    return;
                }
                if (contextPanel == null)
                {
                    contextPanel = new ContextPanel();
                }
                if (Properties.Settings.Default.TranparentWindows)
                {
                    contextPanel.SetOpacity();
                    contextPanel.Parent = null;
                    contextPanel.TopLevel = true;
                    contextPanel.Owner = this;
                    contextPanel.Dock = DockStyle.None;
                    contextPanel.Show();
                    contextPanel.Location = PointToScreen(new Point(0, ClientRectangle.Bottom - contextPanel.Height));
                    contextPanel.Width = ClientRectangle.Width;
                }
                else
                {
                    contextPanel.SetOpacity();
                    contextPanel.TopLevel = false;
                    contextPanel.Owner = null;
                    contextPanel.Parent = this;
                    contextPanel.Dock = DockStyle.Bottom;
                    contextPanel.Show();
                }
            }
            catch
            {
            }

        }

       
        public bool KmlAutoRefresh = false;

        public ConstellationFigureEditor figureEditor = null;


        public LayerManager layerManager = null;
        private void ShowLayersWindow()
        {
            if (layerManager == null)
            {
                layerManager = new LayerManager();
                layerManager.Owner = this;
            }

            Rectangle rectContext = contextPanel.Bounds;
            Rectangle rectCurrentTab = currentTab.Bounds;

            Point pnt = PointToScreen(new Point(0, (ClientRectangle.Bottom - contextPanel.Height)));
            Point pntTab = rectCurrentTab.Location;

            if (!Properties.Settings.Default.TranparentWindows)
            {
                pntTab = PointToScreen(pntTab);
            }
            layerManager.Location = new Point(pnt.X, pntTab.Y + rectCurrentTab.Height + 1);
            layerManager.Height = pnt.Y - layerManager.Top;
            layerManager.Show();
        }

        public void ShowFigureEditorWindow(Constellations figures)
        {
            if (figureEditor == null)
            {
                figureEditor = new ConstellationFigureEditor();
                figureEditor.Owner = this;
            }
            Properties.Settings.Default.ShowConstellationFigures.TargetState = true;

            figureEditor.Figures = figures;



            ShowFiguresEditorWindow();

        }

        private void ShowFiguresEditorWindow()
        {
            if (figureEditor != null)
            {
                Rectangle rectContext = contextPanel.Bounds;
                Rectangle rectCurrentTab = currentTab.Bounds;


                Point pnt = PointToScreen(new Point(ClientRectangle.Right, (ClientRectangle.Bottom - contextPanel.Height)));
                Point pntTab = rectCurrentTab.Location;

                if (!Properties.Settings.Default.TranparentWindows)
                {
                    pntTab = PointToScreen(pntTab);
                }
                figureEditor.Location = new Point(pnt.X - figureEditor.Width, pntTab.Y + rectCurrentTab.Height + 1);
                figureEditor.Height = pnt.Y - figureEditor.Top;
                figureEditor.Show();
            }
        }
        ImageStack stack = null;

        public ImageStack Stack
        {
            get { return stack; }
            set { stack = value; }
        }
        bool imageStackVisible = false;

        public bool ImageStackVisible
        {
            get { return imageStackVisible; }
            set
            {
                imageStackVisible = value;
                RenderEngine.imageStackVisible = value;
                ShowImageStack();
            }
        }
        public void ShowImageStack()
        {
            if (stack == null)
            {
                stack = new ImageStack();
                stack.Owner = this;
                stack.UpdateList();
            }

            if (!imageStackVisible)
            {
                stack.Visible = false;
            }
            else if (!RenderEngine.ProjectorServer)
            {
                Rectangle rectContext = contextPanel.Bounds;
                Rectangle rectCurrentTab = currentTab.Bounds;

                stack.Show();
                Point pnt = PointToScreen(new Point(ClientRectangle.Right - stack.Width, ClientRectangle.Bottom - contextPanel.Height));
                stack.Location = new Point(pnt.X, rectCurrentTab.Top + rectCurrentTab.Height + 1);
                stack.Height = pnt.Y - stack.Top;
            }
        }

        Search searchPane = null;
        FolderBrowser toursTab = null;

        FolderBrowser explorePane = null;
        FolderBrowser communitiesPane = null;
        View viewPane = null;
        SettingsTab settingsPane = null;
        TelescopeTab telescopePane = null;
        public ContextPanel contextPanel = null;
        TourEditTab tourEdit = null;

        public TourEditTab TourEdit
        {
            get { return tourEdit; }
            set { tourEdit = value; }
        }
        IUiController uiController = null;
        public IUiController UiController
        {
            get
            {

                return uiController;
            }
            set
            {
                uiController = value;
                RenderEngine.uiController = uiController;
           }
        }

        void menuTabs_MenuClicked(object sender, ApplicationMode e)
        {
            Point menuPoint = new Point((int)e * 100 + menuTabs.StartX + 2, 36);

            switch (e)
            {
                case ApplicationMode.Explore:
                    {
                        menuTabs.Freeze();
                        this.exploreMenu.Show(menuTabs.PointToScreen(menuPoint));
                    }
                    break;
                case ApplicationMode.Tours:
                    {
                        menuTabs.Freeze();
                        toursMenu.Show(menuTabs.PointToScreen(menuPoint));
                    }
                    break;
                case ApplicationMode.Search:
                    {
                        menuTabs.Freeze();
                        searchMenu.Show(menuTabs.PointToScreen(menuPoint));
                    }
                    break;
                case ApplicationMode.Community:
                    {
                        menuTabs.Freeze();
                        communitiesMenu.Show(menuTabs.PointToScreen(menuPoint));
                    }
                    break;

                case ApplicationMode.Telescope:
                    {
                        menuTabs.Freeze();
                        telescopeMenu.Show(menuTabs.PointToScreen(menuPoint));
                    }
                    break;
                case ApplicationMode.View:
                    {
                        menuTabs.Freeze();
                        viewMenu.Show(menuTabs.PointToScreen(menuPoint));
                    }
                    break;
                case ApplicationMode.Settings:
                    {
                        menuTabs.Freeze();
                        settingsMenu.Show(menuTabs.PointToScreen(menuPoint));
                    }
                    break;
                case ApplicationMode.Tour1:

                case ApplicationMode.Tour2:

                case ApplicationMode.Tour3:
                case ApplicationMode.Tour4:
                case ApplicationMode.Tour5:
                default:
                    break;
            }
        }

        private void menuTabs_TabClicked(object sender, ApplicationMode e)
        {
            if (currentMode == e)
            {
                //Show Menus
            }
            else
            {
                //switch modes
                SetAppMode(e);
            }
            switch (e)
            {
                default:
                    break;

            }
        }

        private async void menuTabs_ControlEvent(object sender, ControlAction e)
        {
            switch (e)
            {
                case ControlAction.AppMenu:
                    break;
                case ControlAction.Close:
                    CloseOpenToursOrAbort(false);
                    TourPopup.CloseTourPopups();
                    this.Close();
                    break;
                case ControlAction.Maximize:
                case ControlAction.Restore:
                    if (WindowState == FormWindowState.Maximized)
                    {
                        this.WindowState = FormWindowState.Normal;
                    }
                    else
                    {
                        this.WindowState = FormWindowState.Maximized;
                    }
                    break;
                case ControlAction.CloseTour:
                    if (tourEdit != null)
                    {
                        CloseOpenToursOrAbort(false);
                    }
                    break;
                case ControlAction.SignOut:
                    WindowsLiveSignOut();
                    if (communitiesPane != null)
                    {
                        communitiesPane.SetCommunitiesMode();
                        communitiesPane.Refresh();
                        communitiesPane.LoadCommunities();
                    }
                    break;
                case ControlAction.SignIn:
                    await WindowsLiveSignIn();
                    menuTabs.SetSelectedIndex((int)ApplicationMode.Community, false);
                    break;
            }
        }

        public void CloseTour(bool silent)
        {
            Undo.Clear();
            RenderEngine.FreezeView();
            if (tourEdit != null)
            {
                TourPopup.CloseTourPopups();
                RenderEngine.Mover = null;
                TourPlayer.Playing = false;
                KeyFramer.HideTimeline();
                if (LayerManager.TourLayers == false && !silent)
                {
                    if (LayerManager.CheckForTourLoadedLayers())
                    {
                        if (UiTools.ShowMessageBox(Language.GetLocalizedText(1004, "Close layers loaded with the tour as well?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            LayerManager.CloseAllTourLoadedLayers();
                        }
                        else
                        {
                            tourEdit.Tour.DontCleanUpTempFiles = true;
                            LayerManager.CleanAllTourLoadedLayers();
                        }


                    }
                }
                tourEdit.Tour.CurrentTourstopIndex = -1;
                tourEdit.Tour.CleanUp();
                tourEdit.Tour.ClearTempFiles();
                menuTabs.RemoveTour(tourEdit.Tour);
                tourEdit.TourEditorUI.Close();
                tourEdit.Close();
                tourEdit = null;
                UiController = null;
                Settings.TourSettings = null;
                if (toursTab == null)
                {
                    menuTabs.SetSelectedIndex(0, false);
                }
                else
                {
                    menuTabs.SetSelectedIndex(1, false);
                }

                LayerManager.TourLayers = false;
            }
        }

        ApplicationMode currentMode = ApplicationMode.Tour1;

        public void SetAppMode()
        {
            SetAppMode(currentMode);
        }

        void SetAppMode(ApplicationMode mode)
        {
            if (RenderEngine.ProjectorServer || NoUi)
            {
                return;
            }

            this.SuspendLayout();

            if (mode != currentMode)
            {
                switch (currentMode)
                {
                    case ApplicationMode.Search:
                        {
                            if (searchPane != null)
                            {
                                searchPane.Hide();
                            }
                        }
                        break;
                    case ApplicationMode.Tours:
                        {
                            if (toursTab != null)
                            {
                                toursTab.Hide();
                            }
                        }
                        break;
                    case ApplicationMode.Explore:
                        {
                            if (explorePane != null)
                            {
                                explorePane.Hide();
                            }
                        }
                        break;
                    case ApplicationMode.Community:
                        {
                            if (communitiesPane != null)
                            {
                                communitiesPane.Hide();
                            }
                        }
                        break;
                    case ApplicationMode.View:
                        {
                            if (viewPane != null)
                            {
                                viewPane.Hide();
                            }
                        }
                        break;
                    case ApplicationMode.Settings:
                        {
                            if (settingsPane != null)
                            {
                                settingsPane.Hide();
                            }
                        }
                        break;
                    case ApplicationMode.Telescope:
                        {
                            if (telescopePane != null)
                            {
                                telescopePane.Hide();
                            }
                        }
                        break;
                    case ApplicationMode.Tour1:
                    case ApplicationMode.Tour2:
                    case ApplicationMode.Tour3:
                    case ApplicationMode.Tour4:
                    case ApplicationMode.Tour5:
                        {
                            if (tourEdit != null)
                            {
                                tourEdit.Hide();
                            }
                        }

                        break;
                }
            }

            currentMode = mode;
            bool loadTours = false;

            switch (mode)
            {
                case ApplicationMode.Tours:
                    {
                        if (toursTab == null)
                        {
                            toursTab = new FolderBrowser();
                            toursTab.Owner = this;
                            loadTours = true;
                        }
                        ShowPane(toursTab);

                    }
                    break;
                case ApplicationMode.Community:
                    {
                        if (communitiesPane == null)
                        {
                            communitiesPane = new FolderBrowser();
                            communitiesPane.SetCommunitiesMode();
                            communitiesPane.Owner = this;
                            ShowPane(communitiesPane);
                            communitiesPane.Refresh();
                            communitiesPane.LoadCommunities();
                        }
                        else
                        {
                            ShowPane(communitiesPane);
                        }
                    }
                    break;
                case ApplicationMode.View:
                    {
                        if (viewPane == null)
                        {
                            viewPane = new View();
                            viewPane.Owner = this;
                        }
                        ShowPane(viewPane);
                    }
                    break;
                case ApplicationMode.Settings:
                    {
                        if (settingsPane == null)
                        {
                            settingsPane = new SettingsTab();
                            settingsPane.Owner = this;
                        }
                        ShowPane(settingsPane);
                    }
                    break;
                case ApplicationMode.Telescope:
                    {
                        if (telescopePane == null)
                        {
                            telescopePane = new TelescopeTab();
                            telescopePane.Owner = this;
                        }
                        ShowPane(telescopePane);
                    }
                    break;
                case ApplicationMode.Explore:
                    {
                        if (explorePane == null)
                        {
                            explorePane = new FolderBrowser();
                            explorePane.ShowMyFolders = true;
                            explorePane.SetExploreMode();
                            explorePane.LoadRootFoder(explorerRoot);
                            explorePane.Owner = this;
                        }
                        ShowPane(explorePane);
                    }
                    break;
                case ApplicationMode.Search:
                    {
                        if (searchPane == null)
                        {
                            searchPane = new Search();
                            searchPane.Owner = this;
                        }
                        ShowPane(searchPane);
                    }

                    break;
                case ApplicationMode.Tour1:
                case ApplicationMode.Tour2:
                case ApplicationMode.Tour3:
                case ApplicationMode.Tour4:
                case ApplicationMode.Tour5:
                    {
                        if (figureEditor != null)
                        {
                            figureEditor.SaveAndClose();
                        }

                        if (menuTabs.CurrentTour != null)
                        {
                            if (tourEdit == null)
                            {
                                tourEdit = new TourEditTab();
                                tourEdit.Owner = this;
                            }

                            if (tourEdit.Tour != menuTabs.CurrentTour)
                            {
                                tourEdit.Tour = menuTabs.CurrentTour;
                            }
                            ShowPane(tourEdit);

                            if (tourEdit.Tour.EditMode && !TourPlayer.Playing)
                            {
                                UiController = tourEdit.TourEditorUI;
                            }
                            TimeLine.SetTour(tourEdit.Tour);
                        }
                    }
                    break;
            }

            ShowContextPanel();


            if (imageStackVisible)
            {
                ShowImageStack();
            }

            ResumeLayout(true);

            if (Properties.Settings.Default.ShowLayerManager)
            {
                ShowLayersWindow();
            }

            if (figureEditor != null)
            {
                ShowFiguresEditorWindow();
            }


            if (currentTab != null)
            {
                currentTab.SetOpacity();
            }

            if (loadTours)
            {
                toursTab.LoadTours();
            }

            ClearClientArea = this.ClientRectangle;

            if (Properties.Settings.Default.TranparentWindows)
            {
                int widthUsed = 0;


                if (Properties.Settings.Default.ShowLayerManager)
                {
                    widthUsed += layerManager.Width;
                }

                ClearClientArea.Height -= (currentTab.Height + contextPanel.Height);
                ClearClientArea.Width -= widthUsed;
                ClearClientArea.Location = new Point(ClearClientArea.Location.X + widthUsed, ClearClientArea.Location.Y + currentTab.Height);
            }

            if (this.WindowState != FormWindowState.Minimized)
            {
                KeyFramer.ShowZOrder();
            }

        }
        public Rectangle ClearClientArea;
        Folder explorerRoot = null;

        public Folder ExplorerRoot
        {
            get { return explorerRoot; }
            set { explorerRoot = value; }
        }

        private void LoadExploreRoot()
        {
            string url = Properties.Settings.Default.ExploreRootUrl;
            string filename = string.Format(@"{0}data\exploreRoot_{1}.wtml", Properties.Settings.Default.CahceDirectory, Math.Abs(url.GetHashCode32()));
            DataSetManager.DownloadFile(url, filename, false, true);
            explorerRoot = Folder.LoadFromFile(filename, true);
        }


        public bool ShowLayersWindows
        {
            get
            {
                return Properties.Settings.Default.ShowLayerManager;
            }
            set
            {
                if (Properties.Settings.Default.ShowLayerManager != value)
                {
                    Properties.Settings.Default.ShowLayerManager = value;
                    if (Properties.Settings.Default.ShowLayerManager)
                    {
                        SetAppMode(currentMode);
                    }
                    else
                    {
                        layerManager.Close();
                        layerManager.Dispose();
                        layerManager = null;
                    }
                    showLayerManagerToolStripMenuItem.Checked = value;
                }

            }
        }
        TabForm currentTab = null;

        private void ShowPane(TabForm pane)
        {
            try
            {
                currentTab = pane;
                TabForm.CurrentForm = pane;

                if (Properties.Settings.Default.TranparentWindows)
                {
                    pane.Parent = null;
                    pane.TopLevel = true;
                    pane.Owner = this;
                    pane.Dock = DockStyle.None;
                    pane.Show();
                    pane.SetOpacity();
                    pane.Location = PointToScreen(new Point(0, 34));
                    pane.Width = ClientRectangle.Width;
                }
                else
                {
                    menuTabs.Parent = null;
                    pane.SetOpacity();
                    pane.TopLevel = false;
                    pane.Owner = null;
                    pane.Parent = this;
                    pane.Dock = DockStyle.Top;
                    pane.Show();
                    menuTabs.Parent = this;
                }
            }
            catch
            {
            }
        }
        int changeCount = 0;
       

        void Default_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Settings.ignoreChanges)
            {
                return;
            }
            changeCount++;

            if (e.PropertyName.Contains("Color") || e.PropertyName == "ShowEarthSky" || e.PropertyName == "CloudMap8k" || e.PropertyName == "PulseMeForUpdate")
            {
                Properties.Settings.Default.ColSettingsVersion++;
            }

            if (e.PropertyName.Contains("Elevation") || e.PropertyName.Contains("Show3dCities"))
            {
                TileCache.PurgeQueue();
                TileCache.ClearCache();
            }

            ProcessChanged();
            if (e.PropertyName == "TranparentWindows")
            {
                SetAppMode(currentMode);
            }

            CheckDefaultProperties(false);
            CacheProxy.BaseUrl = Properties.Settings.Default.SharedCacheServer;
        }

        public void SuspendChanges()
        {
            Settings.ignoreChanges = true;
        }

        public void ProcessChanged()
        {
            Settings.ignoreChanges = false;


            Planets.ShowActualSize = Settings.Active.ActualPlanetScale;

            if (Properties.Settings.Default.LocalHorizonMode)
            {
                RenderEngine.ViewType = RenderEngine.ViewTypes.AltAz;
            }
            else
            {
                RenderEngine.ViewType = RenderEngine.ViewTypes.Equatorial;
            }

        }



        void Default_SettingChanging(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            settingsDirty = true;
        }

        DateTime lastMessage = DateTime.Now;
        public void SetLocation(double lat, double lng, double zoom, double cameraRotate, double cameraAngle, int foregroundImageSetHash,
                                            int backgroundImageSetHash, float blendOpacity, bool runSetup, bool flush, SolarSystemObjects target, Vector3d targetPoint, int solarSystemScale, string targetReferenceFrame)
        {
            if (!Settings.MasterController)
            {
                bool resetViewmode = false;
                if (!imageStackVisible)
                {
                    if (CurrentImageSet.GetHash() != backgroundImageSetHash && CurrentImageSet.ThumbnailUrl.GetHashCode32() != backgroundImageSetHash)
                    {
                        SetImageSetByHash(backgroundImageSetHash);
                        resetViewmode = true;
                    }

                    if (StudyImageset == null || (StudyImageset.GetHash() != foregroundImageSetHash && StudyImageset.ThumbnailUrl.GetHashCode32() != foregroundImageSetHash))
                    {
                        if (foregroundImageSetHash != 0)
                        {
                            SetStudyImagesetByHash(foregroundImageSetHash);
                        }
                    }
                }

                if (resetViewmode)
                {
                    RenderEngine.SetViewMode();
                }
                RenderEngine.TrackingFrame = targetReferenceFrame;
                RenderEngine.TargetLat = RenderEngine.ViewLat = lat;
                RenderEngine.TargetLong = RenderEngine.ViewLong = lng;
                RenderEngine.ZoomFactor = RenderEngine.TargetZoom = zoom;
                if (RenderEngine.Space && Settings.Active.GalacticMode)
                {
                    double[] gPoint = Coordinates.J2000toGalactic(RenderEngine.viewCamera.RA * 15, RenderEngine.viewCamera.Dec);
                    RenderEngine.targetAlt = RenderEngine.Alt = gPoint[1];
                    RenderEngine.targetAz = RenderEngine.Az = gPoint[0];
                }
                else if (RenderEngine.Space && Settings.Active.LocalHorizonMode)
                {
                    Coordinates currentAltAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(RenderEngine.viewCamera.RA, RenderEngine.viewCamera.Dec), SpaceTimeController.Location, SpaceTimeController.Now);

                    RenderEngine.targetAlt = RenderEngine.Alt = currentAltAz.Alt;
                    RenderEngine.targetAz = RenderEngine.Az = currentAltAz.Az;
                }
                RenderEngine.CameraAngle = cameraAngle;
                RenderEngine.CameraRotate = cameraRotate;
                RenderEngine.StudyOpacity = blendOpacity;
                RenderEngine.SolarSystemTrack = target;
                RenderEngine.viewCamera.ViewTarget = targetPoint;
                if (Properties.Settings.Default.SolarSystemScale != solarSystemScale)
                {
                    Properties.Settings.Default.SolarSystemScale = solarSystemScale;
                }
                TimeSpan ts = DateTime.Now.Subtract(lastMessage);

                lastMessage = DateTime.Now;
            }

            if (runSetup)
            {
                runUpdate();
            }
        }


        

        public static int bgImagesetGets = 0;
        public static int bgImagesetFails = 0;

        private void SetImageSetByHash(int backgroundImageSetHash)
        {
            if (RenderEngine.ImagesetHashTable.ContainsKey(backgroundImageSetHash))
            {
                CurrentImageSet = RenderEngine.ImagesetHashTable[backgroundImageSetHash];
            }
            else
            {
                try
                {
                    if (backgroundImageSetHash != 0)
                    {
                        bgImagesetGets++;
                        if (Utils.Logging) { Utils.WriteLogMessage("Get Background Imageset from Server"); }
                        WebClient client = new WebClient();
                        string url = string.Format("http://{0}:5050/imagesetwtml?id={1}", NetControl.MasterAddress, backgroundImageSetHash);
                        string wtml = client.DownloadString(url);
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(wtml);
                        XmlNode folder = doc["Folder"];
                        XmlNode imageSetXml = folder.FirstChild;
                        ImageSetHelper ish = ImageSetHelper.FromXMLNode(imageSetXml);
                        CurrentImageSet = ish;
                    }
                }
                catch
                {
                    bgImagesetFails++;
                }

                if (CurrentImageSet == null)
                {
                    CurrentImageSet = RenderEngine.ImageSets[0];
                }
            }
        }

        public static int fgImagesetGets = 0;
        public static int fgImagesetFails = 0;

        private void SetStudyImagesetByHash(int foregroundImageSetHash)
        {
            if (RenderEngine.ImagesetHashTable.ContainsKey(foregroundImageSetHash))
            {
                StudyImageset = RenderEngine.ImagesetHashTable[foregroundImageSetHash];
            }
            else
            {
                try
                {
                    if (foregroundImageSetHash != 0)
                    {
                        if (Utils.Logging) { Utils.WriteLogMessage("Get Background Imageset from Server"); }
                        fgImagesetGets++;
                        WebClient client = new WebClient();
                        string url = string.Format("http://{0}:5050/imagesetwtml?id={1}", NetControl.MasterAddress, foregroundImageSetHash);
                        string wtml = client.DownloadString(url);
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(wtml);
                        XmlNode folder = doc["Folder"];
                        XmlNode imageSetXml = folder.FirstChild;
                        ImageSetHelper ish = ImageSetHelper.FromXMLNode(imageSetXml);
                        StudyImageset = ish;
                    }
                }
                catch
                {
                    fgImagesetFails++;
                    StudyImageset = null;
                }

            }
        }

        private IPlace getContextMenuTargetObject()
        {
            return contextMenuTargetObject;
        }

        private void SetUiStrings()
        {
            // Set UI strings
            nameToolStripMenuItem.Text = Language.GetLocalizedText(7, "Name:");
            informationToolStripMenuItem.Text = Language.GetLocalizedText(8, "Information");
            lookupOnSimbadToolStripMenuItem.Text = Language.GetLocalizedText(9, "Look up on SIMBAD");
            lookupOnSEDSToolStripMenuItem.Text = Language.GetLocalizedText(10, "Look up on SEDS");
            lookupOnWikipediaToolStripMenuItem.Text = Language.GetLocalizedText(11, "Look up on Wikipedia");
            publicationsToolStripMenuItem.Text = Language.GetLocalizedText(12, "Look up publications on ADS");
            imageryToolStripMenuItem.Text = Language.GetLocalizedText(13, "Imagery");
            getDSSImageToolStripMenuItem.Text = Language.GetLocalizedText(14, "Get DSS image");
            getSDSSImageToolStripMenuItem.Text = Language.GetLocalizedText(15, "Get SDSS image");
            getDSSFITSToolStripMenuItem.Text = Language.GetLocalizedText(16, "Get DSS FITS");
            virtualObservatorySearchesToolStripMenuItem.Text = Language.GetLocalizedText(17, "Virtual Observatory Searches");
            hLAFootprintsToolStripMenuItem.Text = Language.GetLocalizedText(1, "HLA Footprints");
            propertiesToolStripMenuItem.Text = Language.GetLocalizedText(20, "Properties");
            copyShortcutToolStripMenuItem.Text = Language.GetLocalizedText(21, "Copy Shortcut");
            addToCollectionsToolStripMenuItem.Text = Language.GetLocalizedText(22, "Add to Collection");
            newCollectionToolStripMenuItem.Text = Language.GetLocalizedText(23, "New Collection...");
            removeFromCollectionToolStripMenuItem.Text = Language.GetLocalizedText(24, "Remove from Collection");
            editToolStripMenuItem.Text = Language.GetLocalizedText(25, "Edit...");
            joinCoomunityMenuItem.Text = Language.GetLocalizedText(26, "Join a Community...");
            updateLoginCredentialsMenuItem.Text = Language.GetLocalizedText(27, "Update Login Credentials...");
            logoutMenuItem.Text = Language.GetLocalizedText(28, "Logout");
            uploadObservingListToCommunityMenuItem.Text = Language.GetLocalizedText(30, "Upload Observing List to Community...");
            uploadImageToCommunityMenuItem.Text = Language.GetLocalizedText(31, "Upload Image to Community...");

            sIMBADSearchToolStripMenuItem.Text = Language.GetLocalizedText(34, "SIMBAD Search...");
            tourHomeMenuItem.Text = Language.GetLocalizedText(35, "Tour Home");
            tourSearchWebPageMenuItem.Text = Language.GetLocalizedText(36, "Tour Search Web Page");
            createANewTourToolStripMenuItem.Text = Language.GetLocalizedText(37, "Create a New Tour...");
            publishTourMenuItem.Text = Language.GetLocalizedText(38, "Submit Tour for Publication...");
            saveTourAsToolStripMenuItem.Text = Language.GetLocalizedText(554, "Save Tour As...");
            autoRepeatToolStripMenuItem.Text = Language.GetLocalizedText(39, "Auto Repeat");
            oneToolStripMenuItem.Text = Language.GetLocalizedText(40, "One");
            allToolStripMenuItem.Text = Language.GetLocalizedText(41, "All");
            offToolStripMenuItem.Text = Language.GetLocalizedText(42, "Off");
            editTourToolStripMenuItem.Text = Language.GetLocalizedText(43, "Edit Tour");
            slewTelescopeMenuItem.Text = Language.GetLocalizedText(44, "Slew To Object");
            centerTelescopeMenuItem.Text = Language.GetLocalizedText(45, "Center on Scope");
            uSNONVOConeSearchToolStripMenuItem.Text = Language.GetLocalizedText(18, "USNO NVO cone search");
            SyncTelescopeMenuItem.Text = Language.GetLocalizedText(46, "Sync Scope to Current Location");
            chooseTelescopeMenuItem.Text = Language.GetLocalizedText(47, "Choose Telescope");
            connectTelescopeMenuItem.Text = Language.GetLocalizedText(48, "Connect");
            trackScopeMenuItem.Text = Language.GetLocalizedText(49, "Track Telescope");

            parkTelescopeMenuItem.Text = Language.GetLocalizedText(50, "Park");
            ASCOMPlatformHomePage.Text = Language.GetLocalizedText(51, "ASCOM Platform");
            createNewObservingListToolStripMenuItem.Text = Language.GetLocalizedText(52, "New");
            newObservingListpMenuItem.Text = Language.GetLocalizedText(53, "Collection...");
            newSimpleTourMenuItem.Text = Language.GetLocalizedText(54, "Slide-Based Tour...");
            openFileToolStripMenuItem.Text = Language.GetLocalizedText(55, "&Open");
            openTourMenuItem.Text = Language.GetLocalizedText(56, "Tour...");
            openObservingListMenuItem.Text = Language.GetLocalizedText(57, "Collection...");
            // openImageMenuItem.Text = Language.GetLocalizedText(58, "Image...");
            openImageMenuItem.Text = Language.GetLocalizedText(948, "Astronomical Image...");
            this.remoteAccessControlToolStripMenuItem.Text = Language.GetLocalizedText(1024, "Remote Access Control...");
            this.screenBroadcastToolStripMenuItem.Text = Language.GetLocalizedText(1023, "Screen Broadcast...");
            this.publishTourToCommunityToolStripMenuItem.Text = Language.GetLocalizedText(1022, "Publish Tour to Community...");
            this.layersToolStripMenuItem.Text = Language.GetLocalizedText(1021, "Layers...");
            showFinderToolStripMenuItem.Text = Language.GetLocalizedText(59, "Show Finder");
            playCollectionAsSlideShowToolStripMenuItem.Text = Language.GetLocalizedText(60, "Play Collection as Slide Show");
            aboutMenuItem.Text = Language.GetLocalizedText(61, "About WorldWide Telescope");
            homepageMenuItem.Text = Language.GetLocalizedText(63, "WorldWide Telescope Home Page");
            exitMenuItem.Text = Language.GetLocalizedText(64, "E&xit");
            checkForUpdatesToolStripMenuItem.Text = Language.GetLocalizedText(65, "Check for Updates...");
            feedbackToolStripMenuItem.Text = Language.GetLocalizedText(66, "Product Support...");
            restoreDefaultsToolStripMenuItem.Text = Language.GetLocalizedText(67, "Restore Defaults");
            advancedToolStripMenuItem.Text = Language.GetLocalizedText(68, "Advanced");
            downloadQueueToolStripMenuItem.Text = Language.GetLocalizedText(69, "Show Download Queue");
            startQueueToolStripMenuItem.Text = Language.GetLocalizedText(70, "Start Queue");
            stopQueueToolStripMenuItem.Text = Language.GetLocalizedText(71, "Stop Queue");
            showPerformanceDataToolStripMenuItem.Text = Language.GetLocalizedText(72, "Show Performance Data");
            toolStripMenuItem2.Text = Language.GetLocalizedText(73, "Master Controller");
            flushCacheToolStripMenuItem.Text = Language.GetLocalizedText(74, "Flush Cache");
            gettingStarteMenuItem.Text = Language.GetLocalizedText(62, "Getting Started (Help)");
            resetCameraMenuItem.Text = Language.GetLocalizedText(75, "Reset Camera");
            copyCurrentViewToClipboardToolStripMenuItem.Text = Language.GetLocalizedText(76, "Copy Current View Image");
            copyShortCutToThisViewToClipboardToolStripMenuItem.Text = Language.GetLocalizedText(77, "Copy Shortcut to this View");
            removeFromImageCacheToolStripMenuItem.Text = Language.GetLocalizedText(82, "Remove from Image Cache");
            setAsForegroundImageryToolStripMenuItem.Text = Language.GetLocalizedText(552, "Set as Foreground Imagery");
            setAsBackgroundImageryToolStripMenuItem.Text = Language.GetLocalizedText(553, "Set as Background Imagery");

            setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem.Text = Language.GetLocalizedText(83, "Set Current View as Windows Desktop Background");
            Text = Language.GetLocalizedText(3, "Microsoft WorldWide Telescope");
            selectLanguageToolStripMenuItem.Text = Language.GetLocalizedText(1, "Select Your Language") + "...";
            toggleFullScreenModeF11ToolStripMenuItem.Text = Language.GetLocalizedText(565, "Toggle Full Screen Mode");
            vOTableToolStripMenuItem.Text = Language.GetLocalizedText(566, "VOTable...");
            stereoToolStripMenuItem.Text = Language.GetLocalizedText(567, "Stereo");
            enabledToolStripMenuItem.Text = Language.GetLocalizedText(568, "Disabled");
            anaglyphToolStripMenuItem.Text = Language.GetLocalizedText(569, "Anaglyph (Red-Cyan)");
            anaglyphYellowBlueToolStripMenuItem.Text = Language.GetLocalizedText(570, "Anaglyph (Yellow-Blue)");
            sideBySideProjectionToolStripMenuItem.Text = Language.GetLocalizedText(571, "Side by Side Projection");
            sideBySideCrossEyedToolStripMenuItem.Text = Language.GetLocalizedText(572, "Side by Side Cross-Eyed");
            expermentalToolStripMenuItem.Text = Language.GetLocalizedText(574, "Full Dome");
            fullDomeToolStripMenuItem.Text = Language.GetLocalizedText(574, "Full Dome");
            domeSetupToolStripMenuItem.Text = Language.GetLocalizedText(575, "Dome Setup");
            toolStripMenuItem3.Text = Language.GetLocalizedText(576, "VO Cone Search / Registry Lookup...");

            earthToolStripMenuItem.Text = Language.GetLocalizedText(581, "Earth");
            planetToolStripMenuItem.Text = Language.GetLocalizedText(582, "Planet");
            skyToolStripMenuItem.Text = Language.GetLocalizedText(583, "Sky");
            panoramaToolStripMenuItem.Text = Language.GetLocalizedText(286, "Panorama");
            solarSystemToolStripMenuItem.Text = Language.GetLocalizedText(373, "Solar System");
            lastToolStripMenuItem.Text = Language.GetLocalizedText(584, "Last");
            randomToolStripMenuItem.Text = Language.GetLocalizedText(585, "Random XX");
            musicAndOtherTourResourceToolStripMenuItem.Text = Language.GetLocalizedText(586, "Music and other Tour Resource");

            startupToolStripMenuItem.Text = Language.GetLocalizedText(591, "Startup Look At");
            lookUpOnNEDToolStripMenuItem.Text = Language.GetLocalizedText(593, "Look up on NED");
            nEDSearchToolStripMenuItem.Text = Language.GetLocalizedText(594, "NED Search");
            sDSSSearchToolStripMenuItem.Text = Language.GetLocalizedText(595, "SDSS Search");
            sendImageToToolStripMenuItem.Text = Language.GetLocalizedText(596, "Send Image To");
            broadcastToolStripMenuItem.Text = Language.GetLocalizedText(597, "Broadcast");
            broadcastToolStripMenuItem1.Text = Language.GetLocalizedText(597, "Broadcast");
            sendTableToToolStripMenuItem.Text = Language.GetLocalizedText(598, "Send Table To");
            imageStackToolStripMenuItem.Text = Language.GetLocalizedText(622, "Image Stack");
            addToImageStackToolStripMenuItem.Text = Language.GetLocalizedText(623, "Add to Image Stack");


            vORegistryToolStripMenuItem.Text = Language.GetLocalizedText(576, "VO Cone Search / Registry Lookup...");
            shapeFileToolStripMenuItem.Text = Language.GetLocalizedText(631, "Shape File...");

            lookUpOnSDSSToolStripMenuItem.Text = Language.GetLocalizedText(632, "Look up on SDSS");

            addCollectionAsTourStopsToolStripMenuItem.Text = Language.GetLocalizedText(645, "Add Collection as Tour Stops");
            showLayerManagerToolStripMenuItem.Text = Language.GetLocalizedText(655, "Show Layer Manager");
            listenUpBoysToolStripMenuItem.Text = Language.GetLocalizedText(656, "Start Listener");
            detachMainViewToSecondMonitor.Text = Language.GetLocalizedText(657, "Detach Main View to Second Monitor");
            regionalDataCacheToolStripMenuItem.Text = Language.GetLocalizedText(658, "Regional Data Cache...");
            multiChanelCalibrationToolStripMenuItem.Text = Language.GetLocalizedText(669, "Multi-Channel Calibration");
            sendLayersToProjectorServersToolStripMenuItem.Text = Language.GetLocalizedText(670, "Send Layers to Projector Servers");
            showTouchControlsToolStripMenuItem.Text = Language.GetLocalizedText(671, "Show On-Screen Controls");
            saveCurrentViewImageToFileToolStripMenuItem.Text = Language.GetLocalizedText(672, "Save Current View Image to File...");
            this.renderToVideoToolStripMenuItem.Text = Language.GetLocalizedText(673, "Render to Video...");
            layerManagerToolStripMenuItem.Text = Language.GetLocalizedText(949, "Layer Manager");
            this.showOverlayListToolStripMenuItem.Text = Language.GetLocalizedText(1057, "Show Overlay List");
            this.sendTourToProjectorServersToolStripMenuItem.Text = Language.GetLocalizedText(1058, "Send Tour to Projector Servers");
            this.automaticTourSyncWithProjectorServersToolStripMenuItem.Text = Language.GetLocalizedText(1059, "Automatic Tour Sync with Projector Servers");
            this.findEarthBasedLocationToolStripMenuItem.Text = Language.GetLocalizedText(1060, "Find Earth Based Location...");
            this.multiSampleAntialiasingToolStripMenuItem.Text = Language.GetLocalizedText(1061, "Multi-Sample Antialiasing");
            this.noneToolStripMenuItem.Text = Language.GetLocalizedText(832, "None");
            this.fourSamplesToolStripMenuItem.Text = Language.GetLocalizedText(1062, "Four Samples");
            this.eightSamplesToolStripMenuItem.Text = Language.GetLocalizedText(1063, "Eight Samples");
            this.lockVerticalSyncToolStripMenuItem.Text = Language.GetLocalizedText(1064, "Lock Vertical Sync");
            this.targetFrameRateToolStripMenuItem.Text = Language.GetLocalizedText(1065, "Target Frame Rate");
            this.fpsToolStripMenuItemUnlimited.Text = Language.GetLocalizedText(1066, "Unlimited");
            this.fPSToolStripMenuItem60.Text = Language.GetLocalizedText(1067, "60 FPS");
            this.fPSToolStripMenuItem30.Text = Language.GetLocalizedText(1068, "30 FPS");
            this.fPSToolStripMenuItem24.Text = Language.GetLocalizedText(1069, "24 FPS");
            this.monochromeStyleToolStripMenuItem.Text = Language.GetLocalizedText(1070, "Monochrome Style");
            this.mIDIControllerSetupToolStripMenuItem.Text = Language.GetLocalizedText(1071, "Controller Setup...");
            this.tileLoadingThrottlingToolStripMenuItem.Text = Language.GetLocalizedText(1072, "Tile Loading Throttling");
            this.tpsToolStripMenuItem15.Text = Language.GetLocalizedText(1073, "15 tps");
            this.tpsToolStripMenuItem30.Text = Language.GetLocalizedText(1074, "30 tps");
            this.tpsToolStripMenuItem60.Text = Language.GetLocalizedText(1075, "60 tps");
            this.tpsToolStripMenuItem120.Text = Language.GetLocalizedText(1076, "120 tps");
            this.tpsToolStripMenuItemUnlimited.Text = Language.GetLocalizedText(1066, "Unlimited");
            this.saveCacheAsCabinetFileToolStripMenuItem.Text = Language.GetLocalizedText(1077, "Save Cache as Cabinet File...");
            this.restoreCacheFromCabinetFileToolStripMenuItem.Text = Language.GetLocalizedText(1078, "Restore Cache from Cabinet File...");
            this.clientNodeListToolStripMenuItem.Text = Language.GetLocalizedText(1079, "Projector Server List");
            allowUnconstrainedTiltToolStripMenuItem.Name = Language.GetLocalizedText(1270, "allowUnconstrainedTiltToolStripMenuItem");
            this.ShowWelcomeTips.Text = Language.GetLocalizedText(1305, "Show Welcome Tips");
            this.allowUnconstrainedTiltToolStripMenuItem.Text = Language.GetLocalizedText(1306, "Allow Unconstrained Tilt");
            this.alternatingLinesEvenToolStripMenuItem.Text = Language.GetLocalizedText(1307, "Alternating Lines Even");
            this.alternatingLinesOddToolStripMenuItem.Text = Language.GetLocalizedText(1308, "Alternating Lines Odd");
            this.oculusRiftToolStripMenuItem.Text = Language.GetLocalizedText(1309, "Oculus Rift");
            this.detachMainViewToThirdMonitorToolStripMenuItem.Text = Language.GetLocalizedText(1310, "Detach Main View to Third Monitor");
            this.fullDomePreviewToolStripMenuItem.Text = Language.GetLocalizedText(1311, "Full Dome Preview");
            this.xBoxControllerSetupToolStripMenuItem.Text = Language.GetLocalizedText(1312, "Xbox Controller Setup...");
            this.showKeyframerToolStripMenuItem.Text = Language.GetLocalizedText(1343, "Show Timeline Editor");
            this.showSlideNumbersToolStripMenuItem.Text = Language.GetLocalizedText(1344, "Show Slide Numbers");
            this.newFullDomeViewInstanceToolStripMenuItem.Text = Language.GetLocalizedText(1376, "New Full Dome View Instance");
            this.customGalaxyFileToolStripMenuItem.Text = Language.GetLocalizedText(1377, "Custom Galaxy File...");
            this.monitorOneToolStripMenuItem.Text = Language.GetLocalizedText(1378, "Monitor One");
            this.monitorTwoToolStripMenuItem.Text = Language.GetLocalizedText(1379, "Monitor Two");
            this.monitorThreeToolStripMenuItem.Text = Language.GetLocalizedText(1380, "Monitor Three");
            this.monitorFourToolStripMenuItem.Text = Language.GetLocalizedText(1381, "Monitor Four");
            this.monitorFiveToolStripMenuItem.Text = Language.GetLocalizedText(1382, "Monitor Five");
            this.monitorSixToolStripMenuItem.Text = Language.GetLocalizedText(1383, "Monitor Six");
            this.monitorSevenToolStripMenuItem.Text = Language.GetLocalizedText(1384, "Monitor Seven");
            this.monitorEightToolStripMenuItem.Text = Language.GetLocalizedText(1385, "Monitor Eight");
            this.exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem.Text = Language.GetLocalizedText(1386, "Export Current View as STL File for 3D Printing...");
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Earth3d));
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.menuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewOverlayTopo = new System.Windows.Forms.ToolStripMenuItem();
            this.InputTimer = new System.Windows.Forms.Timer(this.components);
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.nameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lookupOnSimbadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lookupOnSEDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lookupOnWikipediaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publicationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lookUpOnNEDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lookUpOnSDSSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getDSSImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getSDSSImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getDSSFITSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.virtualObservatorySearchesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uSNONVOConeSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hLAFootprintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nEDSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sDSSSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.setAsForegroundImageryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAsBackgroundImageryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToImageStackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addAsNewLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cacheManagementToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cacheImageryTilePyramidToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showCacheSpaceUsedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFromImageCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImagerySeperator = new System.Windows.Forms.ToolStripSeparator();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyShortcutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addToCollectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeFromCollectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sAMPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendImageToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.broadcastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendTableToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.broadcastToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.HoverTimer = new System.Windows.Forms.Timer(this.components);
            this.communitiesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.joinCoomunityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateLoginCredentialsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.uploadObservingListToCommunityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uploadImageToCommunityMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sIMBADSearchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vORegistryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findEarthBasedLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toursMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tourHomeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tourSearchWebPageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.musicAndOtherTourResourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.createANewTourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTourAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publishTourMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renderToVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.autoRepeatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.offToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editTourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showOverlayListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showKeyframerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSlideNumbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.publishTourToCommunityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.sendTourToProjectorServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automaticTourSyncWithProjectorServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.telescopeMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.slewTelescopeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.centerTelescopeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SyncTelescopeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.chooseTelescopeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectTelescopeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackScopeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.parkTelescopeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.ASCOMPlatformHomePage = new System.Windows.Forms.ToolStripMenuItem();
            this.exploreMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createNewObservingListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newObservingListpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.newSimpleTourMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTourMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openObservingListMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openKMLMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vOTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shapeFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.layerManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customGalaxyFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.showFinderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playCollectionAsSlideShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addCollectionAsTourStopsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ShowWelcomeTips = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gettingStarteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.homepageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.feedbackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.restoreDefaultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.advancedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadQueueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startQueueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopQueueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileLoadingThrottlingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tpsToolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.tpsToolStripMenuItem30 = new System.Windows.Forms.ToolStripMenuItem();
            this.tpsToolStripMenuItem60 = new System.Windows.Forms.ToolStripMenuItem();
            this.tpsToolStripMenuItem120 = new System.Windows.Forms.ToolStripMenuItem();
            this.tpsToolStripMenuItemUnlimited = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.DownloadMPC = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.saveCacheAsCabinetFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreCacheFromCabinetFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.flushCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.showPerformanceDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.multiChanelCalibrationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clientNodeListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.sendLayersToProjectorServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mIDIControllerSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xBoxControllerSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.remoteAccessControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripSeparator();
            this.selectLanguageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regionalDataCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetCameraMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTouchControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monochromeStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allowUnconstrainedTiltToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.startupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.earthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.planetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panoramaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.solarSystemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.copyCurrentViewToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyShortCutToThisViewToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCurrentViewImageToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCurrentCitiesViewAs3DMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.screenBroadcastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.imageStackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showLayerManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.stereoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.enabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.anaglyphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.anaglyphYellowBlueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sideBySideProjectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sideBySideCrossEyedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alternatingLinesOddToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alternatingLinesEvenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oculusRiftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oculusVRHeadsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monoModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startInOculusModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expermentalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullDomeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFullDomeViewInstanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorOneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorTwoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorThreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorFourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorFiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorSixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorSevenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.monitorEightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripSeparator();
            this.domeSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listenUpBoysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.detachMainViewToSecondMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.detachMainViewToThirdMonitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
            this.fullDomePreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleFullScreenModeF11ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multiSampleAntialiasingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fourSamplesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eightSamplesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lockVerticalSyncToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.targetFrameRateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fpsToolStripMenuItemUnlimited = new System.Windows.Forms.ToolStripMenuItem();
            this.fPSToolStripMenuItem60 = new System.Windows.Forms.ToolStripMenuItem();
            this.fPSToolStripMenuItem30 = new System.Windows.Forms.ToolStripMenuItem();
            this.fPSToolStripMenuItem24 = new System.Windows.Forms.ToolStripMenuItem();
            this.enableExport3dCitiesModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StatupTimer = new System.Windows.Forms.Timer(this.components);
            this.SlideAdvanceTimer = new System.Windows.Forms.Timer(this.components);
            this.TourEndCheck = new System.Windows.Forms.Timer(this.components);
            this.autoSaveTimer = new System.Windows.Forms.Timer(this.components);
            this.DeviceHeartbeat = new System.Windows.Forms.Timer(this.components);
            this.kioskTitleBar = new TerraViewer.KioskTitleBar();
            this.renderWindow = new TerraViewer.RenderTarget();
            this.menuTabs = new TerraViewer.MenuTabs();
            this.contextMenu.SuspendLayout();
            this.communitiesMenu.SuspendLayout();
            this.searchMenu.SuspendLayout();
            this.toursMenu.SuspendLayout();
            this.telescopeMenu.SuspendLayout();
            this.exploreMenu.SuspendLayout();
            this.settingsMenu.SuspendLayout();
            this.viewMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // menuItem7
            // 
            this.menuItem7.Name = "menuItem7";
            this.menuItem7.Size = new System.Drawing.Size(32, 19);
            this.menuItem7.Text = "-";
            // 
            // viewOverlayTopo
            // 
            this.viewOverlayTopo.Name = "viewOverlayTopo";
            this.viewOverlayTopo.Size = new System.Drawing.Size(32, 19);
            // 
            // InputTimer
            // 
            this.InputTimer.Enabled = true;
            this.InputTimer.Interval = 350;
            this.InputTimer.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nameToolStripMenuItem,
            this.toolStripSeparator11,
            this.informationToolStripMenuItem,
            this.imageryToolStripMenuItem,
            this.virtualObservatorySearchesToolStripMenuItem,
            this.toolStripSeparator15,
            this.setAsForegroundImageryToolStripMenuItem,
            this.setAsBackgroundImageryToolStripMenuItem,
            this.addToImageStackToolStripMenuItem,
            this.addAsNewLayerToolStripMenuItem,
            this.cacheManagementToolStripMenuItem1,
            this.ImagerySeperator,
            this.propertiesToolStripMenuItem,
            this.copyShortcutToolStripMenuItem,
            this.addToCollectionsToolStripMenuItem,
            this.removeFromCollectionToolStripMenuItem,
            this.editToolStripMenuItem,
            this.sAMPToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(225, 352);
            this.contextMenu.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.contextMenu_Closing);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // nameToolStripMenuItem
            // 
            this.nameToolStripMenuItem.Name = "nameToolStripMenuItem";
            this.nameToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.nameToolStripMenuItem.Text = "Name:";
            this.nameToolStripMenuItem.Click += new System.EventHandler(this.nameToolStripMenuItem_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(221, 6);
            // 
            // informationToolStripMenuItem
            // 
            this.informationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lookupOnSimbadToolStripMenuItem,
            this.lookupOnSEDSToolStripMenuItem,
            this.lookupOnWikipediaToolStripMenuItem,
            this.publicationsToolStripMenuItem,
            this.lookUpOnNEDToolStripMenuItem,
            this.lookUpOnSDSSToolStripMenuItem});
            this.informationToolStripMenuItem.Name = "informationToolStripMenuItem";
            this.informationToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.informationToolStripMenuItem.Text = "Information";
            // 
            // lookupOnSimbadToolStripMenuItem
            // 
            this.lookupOnSimbadToolStripMenuItem.Name = "lookupOnSimbadToolStripMenuItem";
            this.lookupOnSimbadToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.lookupOnSimbadToolStripMenuItem.Text = "Look up on SIMBAD";
            this.lookupOnSimbadToolStripMenuItem.Click += new System.EventHandler(this.lookupOnSimbadToolStripMenuItem_Click);
            // 
            // lookupOnSEDSToolStripMenuItem
            // 
            this.lookupOnSEDSToolStripMenuItem.Name = "lookupOnSEDSToolStripMenuItem";
            this.lookupOnSEDSToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.lookupOnSEDSToolStripMenuItem.Text = "Look up on SEDS";
            this.lookupOnSEDSToolStripMenuItem.Click += new System.EventHandler(this.lookupOnSEDSToolStripMenuItem_Click);
            // 
            // lookupOnWikipediaToolStripMenuItem
            // 
            this.lookupOnWikipediaToolStripMenuItem.Name = "lookupOnWikipediaToolStripMenuItem";
            this.lookupOnWikipediaToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.lookupOnWikipediaToolStripMenuItem.Text = "Look up on Wikipedia";
            this.lookupOnWikipediaToolStripMenuItem.Click += new System.EventHandler(this.lookupOnWikipediaToolStripMenuItem_Click);
            // 
            // publicationsToolStripMenuItem
            // 
            this.publicationsToolStripMenuItem.Name = "publicationsToolStripMenuItem";
            this.publicationsToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.publicationsToolStripMenuItem.Text = "Look up publications on ADS";
            this.publicationsToolStripMenuItem.Click += new System.EventHandler(this.publicationsToolStripMenuItem_Click);
            // 
            // lookUpOnNEDToolStripMenuItem
            // 
            this.lookUpOnNEDToolStripMenuItem.Name = "lookUpOnNEDToolStripMenuItem";
            this.lookUpOnNEDToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.lookUpOnNEDToolStripMenuItem.Text = "Look up on NED";
            this.lookUpOnNEDToolStripMenuItem.Click += new System.EventHandler(this.lookUpOnNEDToolStripMenuItem_Click);
            // 
            // lookUpOnSDSSToolStripMenuItem
            // 
            this.lookUpOnSDSSToolStripMenuItem.Name = "lookUpOnSDSSToolStripMenuItem";
            this.lookUpOnSDSSToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.lookUpOnSDSSToolStripMenuItem.Text = "Look up on SDSS";
            this.lookUpOnSDSSToolStripMenuItem.Click += new System.EventHandler(this.lookUpOnSDSSToolStripMenuItem_Click);
            // 
            // imageryToolStripMenuItem
            // 
            this.imageryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getDSSImageToolStripMenuItem,
            this.getSDSSImageToolStripMenuItem,
            this.getDSSFITSToolStripMenuItem});
            this.imageryToolStripMenuItem.Name = "imageryToolStripMenuItem";
            this.imageryToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.imageryToolStripMenuItem.Text = "Imagery";
            // 
            // getDSSImageToolStripMenuItem
            // 
            this.getDSSImageToolStripMenuItem.Name = "getDSSImageToolStripMenuItem";
            this.getDSSImageToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.getDSSImageToolStripMenuItem.Text = "Get DSS image";
            this.getDSSImageToolStripMenuItem.Click += new System.EventHandler(this.getDSSImageToolStripMenuItem_Click);
            // 
            // getSDSSImageToolStripMenuItem
            // 
            this.getSDSSImageToolStripMenuItem.Name = "getSDSSImageToolStripMenuItem";
            this.getSDSSImageToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.getSDSSImageToolStripMenuItem.Text = "Get SDSS image";
            this.getSDSSImageToolStripMenuItem.Click += new System.EventHandler(this.getSDSSImageToolStripMenuItem_Click);
            // 
            // getDSSFITSToolStripMenuItem
            // 
            this.getDSSFITSToolStripMenuItem.Name = "getDSSFITSToolStripMenuItem";
            this.getDSSFITSToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.getDSSFITSToolStripMenuItem.Text = "Get DSS FITS";
            this.getDSSFITSToolStripMenuItem.Click += new System.EventHandler(this.getDSSFITSToolStripMenuItem_Click);
            // 
            // virtualObservatorySearchesToolStripMenuItem
            // 
            this.virtualObservatorySearchesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uSNONVOConeSearchToolStripMenuItem,
            this.hLAFootprintsToolStripMenuItem,
            this.nEDSearchToolStripMenuItem,
            this.sDSSSearchToolStripMenuItem,
            this.toolStripMenuItem3});
            this.virtualObservatorySearchesToolStripMenuItem.Name = "virtualObservatorySearchesToolStripMenuItem";
            this.virtualObservatorySearchesToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.virtualObservatorySearchesToolStripMenuItem.Text = "Virtual Observatory Searches";
            this.virtualObservatorySearchesToolStripMenuItem.Click += new System.EventHandler(this.virtualObservatorySearchesToolStripMenuItem_Click);
            // 
            // uSNONVOConeSearchToolStripMenuItem
            // 
            this.uSNONVOConeSearchToolStripMenuItem.Name = "uSNONVOConeSearchToolStripMenuItem";
            this.uSNONVOConeSearchToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.uSNONVOConeSearchToolStripMenuItem.Text = "USNO NVO cone search";
            this.uSNONVOConeSearchToolStripMenuItem.Visible = false;
            this.uSNONVOConeSearchToolStripMenuItem.Click += new System.EventHandler(this.uSNONVOConeSearchToolStripMenuItem_Click);
            // 
            // hLAFootprintsToolStripMenuItem
            // 
            this.hLAFootprintsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.hLAFootprintsToolStripMenuItem.Name = "hLAFootprintsToolStripMenuItem";
            this.hLAFootprintsToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.hLAFootprintsToolStripMenuItem.Text = "HLA Footprints";
            this.hLAFootprintsToolStripMenuItem.Visible = false;
            this.hLAFootprintsToolStripMenuItem.Click += new System.EventHandler(this.hLAFootprintsToolStripMenuItem_Click);
            // 
            // nEDSearchToolStripMenuItem
            // 
            this.nEDSearchToolStripMenuItem.Name = "nEDSearchToolStripMenuItem";
            this.nEDSearchToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.nEDSearchToolStripMenuItem.Text = "NED Search";
            this.nEDSearchToolStripMenuItem.Click += new System.EventHandler(this.NEDSearchToolStripMenuItem_Click);
            // 
            // sDSSSearchToolStripMenuItem
            // 
            this.sDSSSearchToolStripMenuItem.Name = "sDSSSearchToolStripMenuItem";
            this.sDSSSearchToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.sDSSSearchToolStripMenuItem.Text = "SDSS Search";
            this.sDSSSearchToolStripMenuItem.Click += new System.EventHandler(this.sDSSSearchToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(264, 22);
            this.toolStripMenuItem3.Text = "VO Cone Search / Registry Lookup...";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.vORegistryToolStripMenuItem_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(221, 6);
            // 
            // setAsForegroundImageryToolStripMenuItem
            // 
            this.setAsForegroundImageryToolStripMenuItem.Name = "setAsForegroundImageryToolStripMenuItem";
            this.setAsForegroundImageryToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.setAsForegroundImageryToolStripMenuItem.Text = "Set as Forground Imagery";
            this.setAsForegroundImageryToolStripMenuItem.Click += new System.EventHandler(this.setAsForegroundImageryToolStripMenuItem_Click);
            // 
            // setAsBackgroundImageryToolStripMenuItem
            // 
            this.setAsBackgroundImageryToolStripMenuItem.Name = "setAsBackgroundImageryToolStripMenuItem";
            this.setAsBackgroundImageryToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.setAsBackgroundImageryToolStripMenuItem.Text = "Set as Background Imagery";
            this.setAsBackgroundImageryToolStripMenuItem.Click += new System.EventHandler(this.setAsBackgroundImageryToolStripMenuItem_Click);
            // 
            // addToImageStackToolStripMenuItem
            // 
            this.addToImageStackToolStripMenuItem.Name = "addToImageStackToolStripMenuItem";
            this.addToImageStackToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addToImageStackToolStripMenuItem.Text = "Add to Image Stack";
            this.addToImageStackToolStripMenuItem.Click += new System.EventHandler(this.addToImageStackToolStripMenuItem_Click);
            // 
            // addAsNewLayerToolStripMenuItem
            // 
            this.addAsNewLayerToolStripMenuItem.Name = "addAsNewLayerToolStripMenuItem";
            this.addAsNewLayerToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addAsNewLayerToolStripMenuItem.Text = "Add as New Layer";
            this.addAsNewLayerToolStripMenuItem.Click += new System.EventHandler(this.addAsNewLayerToolStripMenuItem_Click);
            // 
            // cacheManagementToolStripMenuItem1
            // 
            this.cacheManagementToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cacheImageryTilePyramidToolStripMenuItem,
            this.showCacheSpaceUsedToolStripMenuItem,
            this.removeFromImageCacheToolStripMenuItem});
            this.cacheManagementToolStripMenuItem1.Name = "cacheManagementToolStripMenuItem1";
            this.cacheManagementToolStripMenuItem1.Size = new System.Drawing.Size(224, 22);
            this.cacheManagementToolStripMenuItem1.Text = "Cache Management";
            // 
            // cacheImageryTilePyramidToolStripMenuItem
            // 
            this.cacheImageryTilePyramidToolStripMenuItem.Name = "cacheImageryTilePyramidToolStripMenuItem";
            this.cacheImageryTilePyramidToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.cacheImageryTilePyramidToolStripMenuItem.Text = "Cache Imagery Tile Pyramid...";
            this.cacheImageryTilePyramidToolStripMenuItem.Click += new System.EventHandler(this.cacheImageryTilePyramidToolStripMenuItem_Click);
            // 
            // showCacheSpaceUsedToolStripMenuItem
            // 
            this.showCacheSpaceUsedToolStripMenuItem.Name = "showCacheSpaceUsedToolStripMenuItem";
            this.showCacheSpaceUsedToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.showCacheSpaceUsedToolStripMenuItem.Text = "Show Cache Space Used...";
            this.showCacheSpaceUsedToolStripMenuItem.Click += new System.EventHandler(this.showCacheSpaceUsedToolStripMenuItem_Click);
            // 
            // removeFromImageCacheToolStripMenuItem
            // 
            this.removeFromImageCacheToolStripMenuItem.Name = "removeFromImageCacheToolStripMenuItem";
            this.removeFromImageCacheToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.removeFromImageCacheToolStripMenuItem.Text = "Remove from Image Cache";
            this.removeFromImageCacheToolStripMenuItem.Click += new System.EventHandler(this.removeFromImageCacheToolStripMenuItem_Click);
            // 
            // ImagerySeperator
            // 
            this.ImagerySeperator.Name = "ImagerySeperator";
            this.ImagerySeperator.Size = new System.Drawing.Size(221, 6);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // copyShortcutToolStripMenuItem
            // 
            this.copyShortcutToolStripMenuItem.Name = "copyShortcutToolStripMenuItem";
            this.copyShortcutToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.copyShortcutToolStripMenuItem.Text = "Copy Shortcut";
            this.copyShortcutToolStripMenuItem.Click += new System.EventHandler(this.copyShortcutToolStripMenuItem_Click);
            // 
            // addToCollectionsToolStripMenuItem
            // 
            this.addToCollectionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newCollectionToolStripMenuItem});
            this.addToCollectionsToolStripMenuItem.Name = "addToCollectionsToolStripMenuItem";
            this.addToCollectionsToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.addToCollectionsToolStripMenuItem.Text = "Add to Collection";
            this.addToCollectionsToolStripMenuItem.DropDownOpening += new System.EventHandler(this.addToCollectionsToolStripMenuItem_DropDownOpening);
            this.addToCollectionsToolStripMenuItem.Click += new System.EventHandler(this.addToCollectionsToolStripMenuItem_Click);
            // 
            // newCollectionToolStripMenuItem
            // 
            this.newCollectionToolStripMenuItem.Name = "newCollectionToolStripMenuItem";
            this.newCollectionToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.newCollectionToolStripMenuItem.Text = "New Collection...";
            // 
            // removeFromCollectionToolStripMenuItem
            // 
            this.removeFromCollectionToolStripMenuItem.Name = "removeFromCollectionToolStripMenuItem";
            this.removeFromCollectionToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.removeFromCollectionToolStripMenuItem.Text = "Remove from Collection";
            this.removeFromCollectionToolStripMenuItem.Click += new System.EventHandler(this.removeFromCollectionToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.editToolStripMenuItem.Text = "Edit...";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // sAMPToolStripMenuItem
            // 
            this.sAMPToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sendImageToToolStripMenuItem,
            this.sendTableToToolStripMenuItem});
            this.sAMPToolStripMenuItem.Name = "sAMPToolStripMenuItem";
            this.sAMPToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.sAMPToolStripMenuItem.Text = "SAMP";
            // 
            // sendImageToToolStripMenuItem
            // 
            this.sendImageToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.broadcastToolStripMenuItem});
            this.sendImageToToolStripMenuItem.Name = "sendImageToToolStripMenuItem";
            this.sendImageToToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.sendImageToToolStripMenuItem.Text = "Send Image To";
            // 
            // broadcastToolStripMenuItem
            // 
            this.broadcastToolStripMenuItem.Name = "broadcastToolStripMenuItem";
            this.broadcastToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.broadcastToolStripMenuItem.Text = "Broadcast";
            this.broadcastToolStripMenuItem.Click += new System.EventHandler(this.broadcastToolStripMenuItem_Click);
            // 
            // sendTableToToolStripMenuItem
            // 
            this.sendTableToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.broadcastToolStripMenuItem1});
            this.sendTableToToolStripMenuItem.Name = "sendTableToToolStripMenuItem";
            this.sendTableToToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.sendTableToToolStripMenuItem.Text = "Send Table To";
            // 
            // broadcastToolStripMenuItem1
            // 
            this.broadcastToolStripMenuItem1.Name = "broadcastToolStripMenuItem1";
            this.broadcastToolStripMenuItem1.Size = new System.Drawing.Size(126, 22);
            this.broadcastToolStripMenuItem1.Text = "Broadcast";
            // 
            // HoverTimer
            // 
            this.HoverTimer.Enabled = true;
            this.HoverTimer.Interval = 500;
            this.HoverTimer.Tick += new System.EventHandler(this.HoverTimer_Tick);
            // 
            // communitiesMenu
            // 
            this.communitiesMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.communitiesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.joinCoomunityMenuItem,
            this.updateLoginCredentialsMenuItem,
            this.logoutMenuItem,
            this.toolStripSeparator8,
            this.uploadObservingListToCommunityMenuItem,
            this.uploadImageToCommunityMenuItem});
            this.communitiesMenu.Name = "communitiesMenu";
            this.communitiesMenu.Size = new System.Drawing.Size(281, 120);
            this.communitiesMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.PopupClosed);
            this.communitiesMenu.Opening += new System.ComponentModel.CancelEventHandler(this.communitiesMenu_Opening);
            this.communitiesMenu.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.exploreMenu_PreviewKeyDown);
            // 
            // joinCoomunityMenuItem
            // 
            this.joinCoomunityMenuItem.Name = "joinCoomunityMenuItem";
            this.joinCoomunityMenuItem.Size = new System.Drawing.Size(280, 22);
            this.joinCoomunityMenuItem.Text = "Join a Community...";
            this.joinCoomunityMenuItem.Click += new System.EventHandler(this.joinCoomunityMenuItem_Click);
            // 
            // updateLoginCredentialsMenuItem
            // 
            this.updateLoginCredentialsMenuItem.Name = "updateLoginCredentialsMenuItem";
            this.updateLoginCredentialsMenuItem.Size = new System.Drawing.Size(280, 22);
            this.updateLoginCredentialsMenuItem.Text = "Update Login Credentials...";
            this.updateLoginCredentialsMenuItem.Click += new System.EventHandler(this.associateLiveIDToolStripMenuItem_Click);
            // 
            // logoutMenuItem
            // 
            this.logoutMenuItem.Enabled = false;
            this.logoutMenuItem.Name = "logoutMenuItem";
            this.logoutMenuItem.Size = new System.Drawing.Size(280, 22);
            this.logoutMenuItem.Text = "Logout";
            this.logoutMenuItem.Visible = false;
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(277, 6);
            this.toolStripSeparator8.Visible = false;
            // 
            // uploadObservingListToCommunityMenuItem
            // 
            this.uploadObservingListToCommunityMenuItem.Name = "uploadObservingListToCommunityMenuItem";
            this.uploadObservingListToCommunityMenuItem.Size = new System.Drawing.Size(280, 22);
            this.uploadObservingListToCommunityMenuItem.Text = "Upload Observing List to Community...";
            this.uploadObservingListToCommunityMenuItem.Visible = false;
            // 
            // uploadImageToCommunityMenuItem
            // 
            this.uploadImageToCommunityMenuItem.Name = "uploadImageToCommunityMenuItem";
            this.uploadImageToCommunityMenuItem.Size = new System.Drawing.Size(280, 22);
            this.uploadImageToCommunityMenuItem.Text = "Upload Image to Community... ";
            this.uploadImageToCommunityMenuItem.Visible = false;
            // 
            // searchMenu
            // 
            this.searchMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.searchMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sIMBADSearchToolStripMenuItem,
            this.vORegistryToolStripMenuItem,
            this.findEarthBasedLocationToolStripMenuItem});
            this.searchMenu.Name = "contextMenuStrip1";
            this.searchMenu.Size = new System.Drawing.Size(265, 70);
            this.searchMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.PopupClosed);
            this.searchMenu.Opening += new System.ComponentModel.CancelEventHandler(this.searchMenu_Opening);
            this.searchMenu.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.exploreMenu_PreviewKeyDown);
            // 
            // sIMBADSearchToolStripMenuItem
            // 
            this.sIMBADSearchToolStripMenuItem.Name = "sIMBADSearchToolStripMenuItem";
            this.sIMBADSearchToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.sIMBADSearchToolStripMenuItem.Text = "SIMBAD Search...";
            this.sIMBADSearchToolStripMenuItem.Click += new System.EventHandler(this.sIMBADSearchToolStripMenuItem_Click);
            // 
            // vORegistryToolStripMenuItem
            // 
            this.vORegistryToolStripMenuItem.Name = "vORegistryToolStripMenuItem";
            this.vORegistryToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.vORegistryToolStripMenuItem.Text = "VO Cone Search / Registry Lookup...";
            this.vORegistryToolStripMenuItem.Click += new System.EventHandler(this.vORegistryToolStripMenuItem_Click);
            // 
            // findEarthBasedLocationToolStripMenuItem
            // 
            this.findEarthBasedLocationToolStripMenuItem.Name = "findEarthBasedLocationToolStripMenuItem";
            this.findEarthBasedLocationToolStripMenuItem.Size = new System.Drawing.Size(264, 22);
            this.findEarthBasedLocationToolStripMenuItem.Text = "Find Earth Based Location...";
            this.findEarthBasedLocationToolStripMenuItem.Click += new System.EventHandler(this.findEarthBasedLocationToolStripMenuItem_Click);
            // 
            // toursMenu
            // 
            this.toursMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toursMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tourHomeMenuItem,
            this.tourSearchWebPageMenuItem,
            this.musicAndOtherTourResourceToolStripMenuItem,
            this.toolStripSeparator6,
            this.createANewTourToolStripMenuItem,
            this.saveTourAsToolStripMenuItem,
            this.publishTourMenuItem,
            this.renderToVideoToolStripMenuItem,
            this.toolStripSeparator1,
            this.autoRepeatToolStripMenuItem,
            this.editTourToolStripMenuItem,
            this.showOverlayListToolStripMenuItem,
            this.showKeyframerToolStripMenuItem,
            this.showSlideNumbersToolStripMenuItem,
            this.toolStripMenuItem12,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripMenuItem13,
            this.publishTourToCommunityToolStripMenuItem,
            this.toolStripSeparator23,
            this.sendTourToProjectorServersToolStripMenuItem,
            this.automaticTourSyncWithProjectorServersToolStripMenuItem});
            this.toursMenu.Name = "contextMenuStrip1";
            this.toursMenu.Size = new System.Drawing.Size(303, 408);
            this.toursMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.PopupClosed);
            this.toursMenu.Opening += new System.ComponentModel.CancelEventHandler(this.toursMenu_Opening);
            this.toursMenu.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.exploreMenu_PreviewKeyDown);
            // 
            // tourHomeMenuItem
            // 
            this.tourHomeMenuItem.Name = "tourHomeMenuItem";
            this.tourHomeMenuItem.Size = new System.Drawing.Size(302, 22);
            this.tourHomeMenuItem.Text = "Tour Home";
            this.tourHomeMenuItem.Click += new System.EventHandler(this.tourHomeMenuItem_Click);
            // 
            // tourSearchWebPageMenuItem
            // 
            this.tourSearchWebPageMenuItem.Name = "tourSearchWebPageMenuItem";
            this.tourSearchWebPageMenuItem.Size = new System.Drawing.Size(302, 22);
            this.tourSearchWebPageMenuItem.Text = "Tour Search Web Page";
            this.tourSearchWebPageMenuItem.Visible = false;
            this.tourSearchWebPageMenuItem.Click += new System.EventHandler(this.tourSearchWebPageMenuItem_Click);
            // 
            // musicAndOtherTourResourceToolStripMenuItem
            // 
            this.musicAndOtherTourResourceToolStripMenuItem.Name = "musicAndOtherTourResourceToolStripMenuItem";
            this.musicAndOtherTourResourceToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.musicAndOtherTourResourceToolStripMenuItem.Text = "Music and other Tour Resource";
            this.musicAndOtherTourResourceToolStripMenuItem.Click += new System.EventHandler(this.musicAndOtherTourResourceToolStripMenuItem_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(299, 6);
            // 
            // createANewTourToolStripMenuItem
            // 
            this.createANewTourToolStripMenuItem.Name = "createANewTourToolStripMenuItem";
            this.createANewTourToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.createANewTourToolStripMenuItem.Text = "Create a New Tour...";
            this.createANewTourToolStripMenuItem.Click += new System.EventHandler(this.newSlideBasedTour);
            // 
            // saveTourAsToolStripMenuItem
            // 
            this.saveTourAsToolStripMenuItem.Name = "saveTourAsToolStripMenuItem";
            this.saveTourAsToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.saveTourAsToolStripMenuItem.Text = "Save Tour As...";
            this.saveTourAsToolStripMenuItem.Click += new System.EventHandler(this.saveTourAsToolStripMenuItem_Click);
            // 
            // publishTourMenuItem
            // 
            this.publishTourMenuItem.Name = "publishTourMenuItem";
            this.publishTourMenuItem.Size = new System.Drawing.Size(302, 22);
            this.publishTourMenuItem.Text = "Submit Tour for Publication...";
            this.publishTourMenuItem.Click += new System.EventHandler(this.publishTourMenuItem_Click);
            // 
            // renderToVideoToolStripMenuItem
            // 
            this.renderToVideoToolStripMenuItem.Name = "renderToVideoToolStripMenuItem";
            this.renderToVideoToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.renderToVideoToolStripMenuItem.Text = "Render to Video...";
            this.renderToVideoToolStripMenuItem.Click += new System.EventHandler(this.renderToVideoToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(299, 6);
            // 
            // autoRepeatToolStripMenuItem
            // 
            this.autoRepeatToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oneToolStripMenuItem,
            this.allToolStripMenuItem,
            this.offToolStripMenuItem});
            this.autoRepeatToolStripMenuItem.Name = "autoRepeatToolStripMenuItem";
            this.autoRepeatToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.autoRepeatToolStripMenuItem.Text = "Auto Repeat";
            // 
            // oneToolStripMenuItem
            // 
            this.oneToolStripMenuItem.Name = "oneToolStripMenuItem";
            this.oneToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.oneToolStripMenuItem.Text = "One";
            this.oneToolStripMenuItem.Click += new System.EventHandler(this.oneToolStripMenuItem_Click);
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.allToolStripMenuItem.Text = "All";
            this.allToolStripMenuItem.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
            // 
            // offToolStripMenuItem
            // 
            this.offToolStripMenuItem.Name = "offToolStripMenuItem";
            this.offToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.offToolStripMenuItem.Text = "Off";
            this.offToolStripMenuItem.Click += new System.EventHandler(this.offToolStripMenuItem_Click);
            // 
            // editTourToolStripMenuItem
            // 
            this.editTourToolStripMenuItem.Name = "editTourToolStripMenuItem";
            this.editTourToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.editTourToolStripMenuItem.Text = "Edit Tour";
            this.editTourToolStripMenuItem.Click += new System.EventHandler(this.editTourToolStripMenuItem_Click);
            // 
            // showOverlayListToolStripMenuItem
            // 
            this.showOverlayListToolStripMenuItem.Name = "showOverlayListToolStripMenuItem";
            this.showOverlayListToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.showOverlayListToolStripMenuItem.Text = "Show Overlay List";
            this.showOverlayListToolStripMenuItem.Click += new System.EventHandler(this.showOverlayListToolStripMenuItem_Click);
            // 
            // showKeyframerToolStripMenuItem
            // 
            this.showKeyframerToolStripMenuItem.Name = "showKeyframerToolStripMenuItem";
            this.showKeyframerToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.showKeyframerToolStripMenuItem.Text = "Show Timeline Editor";
            this.showKeyframerToolStripMenuItem.Click += new System.EventHandler(this.showKeyframerToolStripMenuItem_Click);
            // 
            // showSlideNumbersToolStripMenuItem
            // 
            this.showSlideNumbersToolStripMenuItem.Name = "showSlideNumbersToolStripMenuItem";
            this.showSlideNumbersToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.showSlideNumbersToolStripMenuItem.Text = "Show Slide Numbers";
            this.showSlideNumbersToolStripMenuItem.Click += new System.EventHandler(this.showSlideNumbersToolStripMenuItem_Click);
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(299, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.undoToolStripMenuItem.Text = "&Undo:";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.redoToolStripMenuItem.Text = "Redo:";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(299, 6);
            // 
            // publishTourToCommunityToolStripMenuItem
            // 
            this.publishTourToCommunityToolStripMenuItem.Name = "publishTourToCommunityToolStripMenuItem";
            this.publishTourToCommunityToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.publishTourToCommunityToolStripMenuItem.Text = "Publish Tour to Community...";
            this.publishTourToCommunityToolStripMenuItem.Click += new System.EventHandler(this.publishTourToCommunityToolStripMenuItem_Click);
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Size = new System.Drawing.Size(299, 6);
            // 
            // sendTourToProjectorServersToolStripMenuItem
            // 
            this.sendTourToProjectorServersToolStripMenuItem.Name = "sendTourToProjectorServersToolStripMenuItem";
            this.sendTourToProjectorServersToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.sendTourToProjectorServersToolStripMenuItem.Text = "Send Tour to Projector Servers";
            this.sendTourToProjectorServersToolStripMenuItem.Click += new System.EventHandler(this.sendTourToProjectorServersToolStripMenuItem_Click);
            // 
            // automaticTourSyncWithProjectorServersToolStripMenuItem
            // 
            this.automaticTourSyncWithProjectorServersToolStripMenuItem.Name = "automaticTourSyncWithProjectorServersToolStripMenuItem";
            this.automaticTourSyncWithProjectorServersToolStripMenuItem.Size = new System.Drawing.Size(302, 22);
            this.automaticTourSyncWithProjectorServersToolStripMenuItem.Text = "Automatic Tour Sync with Projector Servers";
            this.automaticTourSyncWithProjectorServersToolStripMenuItem.Click += new System.EventHandler(this.automaticTourSyncWithProjectorServersToolStripMenuItem_Click);
            // 
            // telescopeMenu
            // 
            this.telescopeMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.telescopeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slewTelescopeMenuItem,
            this.centerTelescopeMenuItem,
            this.SyncTelescopeMenuItem,
            this.toolStripSeparator3,
            this.chooseTelescopeMenuItem,
            this.connectTelescopeMenuItem,
            this.trackScopeMenuItem,
            this.toolStripSeparator12,
            this.parkTelescopeMenuItem,
            this.toolStripSeparator13,
            this.ASCOMPlatformHomePage});
            this.telescopeMenu.Name = "contextMenuStrip1";
            this.telescopeMenu.Size = new System.Drawing.Size(241, 198);
            this.telescopeMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.PopupClosed);
            this.telescopeMenu.Opening += new System.ComponentModel.CancelEventHandler(this.telescopeMenu_Opening);
            this.telescopeMenu.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.exploreMenu_PreviewKeyDown);
            // 
            // slewTelescopeMenuItem
            // 
            this.slewTelescopeMenuItem.MergeIndex = 0;
            this.slewTelescopeMenuItem.Name = "slewTelescopeMenuItem";
            this.slewTelescopeMenuItem.Size = new System.Drawing.Size(240, 22);
            this.slewTelescopeMenuItem.Text = "Slew To Object";
            this.slewTelescopeMenuItem.Click += new System.EventHandler(this.slewTelescopeMenuItem_Click);
            // 
            // centerTelescopeMenuItem
            // 
            this.centerTelescopeMenuItem.MergeIndex = 1;
            this.centerTelescopeMenuItem.Name = "centerTelescopeMenuItem";
            this.centerTelescopeMenuItem.Size = new System.Drawing.Size(240, 22);
            this.centerTelescopeMenuItem.Text = "Center on Scope";
            this.centerTelescopeMenuItem.Click += new System.EventHandler(this.centerTelescopeMenuItem_Click);
            // 
            // SyncTelescopeMenuItem
            // 
            this.SyncTelescopeMenuItem.MergeIndex = 2;
            this.SyncTelescopeMenuItem.Name = "SyncTelescopeMenuItem";
            this.SyncTelescopeMenuItem.Size = new System.Drawing.Size(240, 22);
            this.SyncTelescopeMenuItem.Text = "Sync Scope to Current Location";
            this.SyncTelescopeMenuItem.Click += new System.EventHandler(this.SyncTelescopeMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.MergeIndex = 3;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(237, 6);
            // 
            // chooseTelescopeMenuItem
            // 
            this.chooseTelescopeMenuItem.MergeIndex = 4;
            this.chooseTelescopeMenuItem.Name = "chooseTelescopeMenuItem";
            this.chooseTelescopeMenuItem.Size = new System.Drawing.Size(240, 22);
            this.chooseTelescopeMenuItem.Text = "Choose Telescope";
            this.chooseTelescopeMenuItem.Click += new System.EventHandler(this.chooseTelescopeMenuItem_Click);
            // 
            // connectTelescopeMenuItem
            // 
            this.connectTelescopeMenuItem.AccessibleName = "";
            this.connectTelescopeMenuItem.MergeIndex = 5;
            this.connectTelescopeMenuItem.Name = "connectTelescopeMenuItem";
            this.connectTelescopeMenuItem.Size = new System.Drawing.Size(240, 22);
            this.connectTelescopeMenuItem.Text = "Connect";
            this.connectTelescopeMenuItem.Click += new System.EventHandler(this.connectTelescopeMenuItem_Click);
            // 
            // trackScopeMenuItem
            // 
            this.trackScopeMenuItem.MergeIndex = 6;
            this.trackScopeMenuItem.Name = "trackScopeMenuItem";
            this.trackScopeMenuItem.Size = new System.Drawing.Size(240, 22);
            this.trackScopeMenuItem.Text = "Track Telescope";
            this.trackScopeMenuItem.Click += new System.EventHandler(this.trackScopeMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.MergeIndex = 7;
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(237, 6);
            // 
            // parkTelescopeMenuItem
            // 
            this.parkTelescopeMenuItem.MergeIndex = 8;
            this.parkTelescopeMenuItem.Name = "parkTelescopeMenuItem";
            this.parkTelescopeMenuItem.Size = new System.Drawing.Size(240, 22);
            this.parkTelescopeMenuItem.Text = "Park";
            this.parkTelescopeMenuItem.Click += new System.EventHandler(this.parkTelescopeMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(237, 6);
            // 
            // ASCOMPlatformHomePage
            // 
            this.ASCOMPlatformHomePage.Name = "ASCOMPlatformHomePage";
            this.ASCOMPlatformHomePage.Size = new System.Drawing.Size(240, 22);
            this.ASCOMPlatformHomePage.Text = "ASCOM Platform";
            this.ASCOMPlatformHomePage.Click += new System.EventHandler(this.AscomPlatformMenuItem_Click);
            // 
            // exploreMenu
            // 
            this.exploreMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.exploreMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createNewObservingListToolStripMenuItem,
            this.openFileToolStripMenuItem,
            this.toolStripSeparator7,
            this.showFinderToolStripMenuItem,
            this.playCollectionAsSlideShowToolStripMenuItem,
            this.addCollectionAsTourStopsToolStripMenuItem,
            this.toolStripSeparator2,
            this.ShowWelcomeTips,
            this.aboutMenuItem,
            this.gettingStarteMenuItem,
            this.homepageMenuItem,
            this.toolStripSeparator4,
            this.exitMenuItem});
            this.exploreMenu.Name = "contextMenuStrip1";
            this.exploreMenu.Size = new System.Drawing.Size(254, 242);
            this.exploreMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.PopupClosed);
            this.exploreMenu.Opening += new System.ComponentModel.CancelEventHandler(this.exploreMenu_Opening);
            this.exploreMenu.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.exploreMenu_PreviewKeyDown);
            // 
            // createNewObservingListToolStripMenuItem
            // 
            this.createNewObservingListToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newObservingListpMenuItem,
            this.toolStripSeparator5,
            this.newSimpleTourMenuItem});
            this.createNewObservingListToolStripMenuItem.Name = "createNewObservingListToolStripMenuItem";
            this.createNewObservingListToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.createNewObservingListToolStripMenuItem.Text = "New";
            // 
            // newObservingListpMenuItem
            // 
            this.newObservingListpMenuItem.Name = "newObservingListpMenuItem";
            this.newObservingListpMenuItem.Size = new System.Drawing.Size(171, 22);
            this.newObservingListpMenuItem.Text = "Collection...";
            this.newObservingListpMenuItem.Click += new System.EventHandler(this.newObservingListpMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(168, 6);
            // 
            // newSimpleTourMenuItem
            // 
            this.newSimpleTourMenuItem.Name = "newSimpleTourMenuItem";
            this.newSimpleTourMenuItem.Size = new System.Drawing.Size(171, 22);
            this.newSimpleTourMenuItem.Text = "Slide-Based Tour...";
            this.newSimpleTourMenuItem.Click += new System.EventHandler(this.newSlideBasedTour);
            // 
            // openFileToolStripMenuItem
            // 
            this.openFileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openTourMenuItem,
            this.openObservingListMenuItem,
            this.layersToolStripMenuItem,
            this.openImageMenuItem,
            this.openKMLMenuItem,
            this.vOTableToolStripMenuItem,
            this.shapeFileToolStripMenuItem,
            this.layerManagerToolStripMenuItem,
            this.customGalaxyFileToolStripMenuItem});
            this.openFileToolStripMenuItem.Name = "openFileToolStripMenuItem";
            this.openFileToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.openFileToolStripMenuItem.Text = "&Open";
            // 
            // openTourMenuItem
            // 
            this.openTourMenuItem.Name = "openTourMenuItem";
            this.openTourMenuItem.Size = new System.Drawing.Size(190, 22);
            this.openTourMenuItem.Text = "Tour...";
            this.openTourMenuItem.Click += new System.EventHandler(this.openTourMenuItem_Click);
            // 
            // openObservingListMenuItem
            // 
            this.openObservingListMenuItem.Name = "openObservingListMenuItem";
            this.openObservingListMenuItem.Size = new System.Drawing.Size(190, 22);
            this.openObservingListMenuItem.Text = "Collection...";
            this.openObservingListMenuItem.Click += new System.EventHandler(this.openObservingListMenuItem_Click);
            // 
            // layersToolStripMenuItem
            // 
            this.layersToolStripMenuItem.Name = "layersToolStripMenuItem";
            this.layersToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.layersToolStripMenuItem.Text = "Layers...";
            this.layersToolStripMenuItem.Click += new System.EventHandler(this.layersToolStripMenuItem_Click);
            // 
            // openImageMenuItem
            // 
            this.openImageMenuItem.Name = "openImageMenuItem";
            this.openImageMenuItem.Size = new System.Drawing.Size(190, 22);
            this.openImageMenuItem.Text = "Astronomical Image...";
            this.openImageMenuItem.Click += new System.EventHandler(this.openImageMenuItem_Click);
            // 
            // openKMLMenuItem
            // 
            this.openKMLMenuItem.Name = "openKMLMenuItem";
            this.openKMLMenuItem.Size = new System.Drawing.Size(190, 22);
            this.openKMLMenuItem.Text = "KML...";
            this.openKMLMenuItem.Visible = false;
            this.openKMLMenuItem.Click += new System.EventHandler(this.openKMLMenuItem_Click);
            // 
            // vOTableToolStripMenuItem
            // 
            this.vOTableToolStripMenuItem.Name = "vOTableToolStripMenuItem";
            this.vOTableToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.vOTableToolStripMenuItem.Text = "VO Table...";
            this.vOTableToolStripMenuItem.Click += new System.EventHandler(this.vOTableToolStripMenuItem_Click);
            // 
            // shapeFileToolStripMenuItem
            // 
            this.shapeFileToolStripMenuItem.Name = "shapeFileToolStripMenuItem";
            this.shapeFileToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.shapeFileToolStripMenuItem.Text = "Shape File...";
            this.shapeFileToolStripMenuItem.Visible = false;
            this.shapeFileToolStripMenuItem.Click += new System.EventHandler(this.shapeFileToolStripMenuItem_Click);
            // 
            // layerManagerToolStripMenuItem
            // 
            this.layerManagerToolStripMenuItem.Name = "layerManagerToolStripMenuItem";
            this.layerManagerToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.layerManagerToolStripMenuItem.Text = "Layer Manager";
            this.layerManagerToolStripMenuItem.Click += new System.EventHandler(this.layerManagerToolStripMenuItem_Click);
            // 
            // customGalaxyFileToolStripMenuItem
            // 
            this.customGalaxyFileToolStripMenuItem.Name = "customGalaxyFileToolStripMenuItem";
            this.customGalaxyFileToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.customGalaxyFileToolStripMenuItem.Text = "Custom Galaxy File...";
            this.customGalaxyFileToolStripMenuItem.Click += new System.EventHandler(this.customGalaxyFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(250, 6);
            // 
            // showFinderToolStripMenuItem
            // 
            this.showFinderToolStripMenuItem.Name = "showFinderToolStripMenuItem";
            this.showFinderToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.showFinderToolStripMenuItem.Text = "Show Finder";
            this.showFinderToolStripMenuItem.Click += new System.EventHandler(this.showFinderToolStripMenuItem_Click);
            // 
            // playCollectionAsSlideShowToolStripMenuItem
            // 
            this.playCollectionAsSlideShowToolStripMenuItem.Name = "playCollectionAsSlideShowToolStripMenuItem";
            this.playCollectionAsSlideShowToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.playCollectionAsSlideShowToolStripMenuItem.Text = "Play Collection as Slide Show";
            this.playCollectionAsSlideShowToolStripMenuItem.Click += new System.EventHandler(this.playCollectionAsSlideShowToolStripMenuItem_Click);
            // 
            // addCollectionAsTourStopsToolStripMenuItem
            // 
            this.addCollectionAsTourStopsToolStripMenuItem.Name = "addCollectionAsTourStopsToolStripMenuItem";
            this.addCollectionAsTourStopsToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.addCollectionAsTourStopsToolStripMenuItem.Text = "Add Collection as Tour Stops";
            this.addCollectionAsTourStopsToolStripMenuItem.Click += new System.EventHandler(this.addCollectionAsTourStopsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(250, 6);
            // 
            // ShowWelcomeTips
            // 
            this.ShowWelcomeTips.Name = "ShowWelcomeTips";
            this.ShowWelcomeTips.Size = new System.Drawing.Size(253, 22);
            this.ShowWelcomeTips.Text = "Show Welcome Tips";
            this.ShowWelcomeTips.Click += new System.EventHandler(this.ShowWelcomeTips_Click);
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(253, 22);
            this.aboutMenuItem.Text = "About WorldWide Telescope";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // gettingStarteMenuItem
            // 
            this.gettingStarteMenuItem.Name = "gettingStarteMenuItem";
            this.gettingStarteMenuItem.Size = new System.Drawing.Size(253, 22);
            this.gettingStarteMenuItem.Text = "Getting Started (Help)";
            this.gettingStarteMenuItem.Click += new System.EventHandler(this.gettingStarteMenuItem_Click);
            // 
            // homepageMenuItem
            // 
            this.homepageMenuItem.Name = "homepageMenuItem";
            this.homepageMenuItem.Size = new System.Drawing.Size(253, 22);
            this.homepageMenuItem.Text = "WorldWide Telescope Home Page";
            this.homepageMenuItem.Click += new System.EventHandler(this.homepageMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(250, 6);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(253, 22);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // settingsMenu
            // 
            this.settingsMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.settingsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.feedbackToolStripMenuItem,
            this.toolStripSeparator17,
            this.restoreDefaultsToolStripMenuItem,
            this.toolStripSeparator16,
            this.advancedToolStripMenuItem,
            this.mIDIControllerSetupToolStripMenuItem,
            this.xBoxControllerSetupToolStripMenuItem,
            this.remoteAccessControlToolStripMenuItem,
            this.toolStripMenuItem14,
            this.selectLanguageToolStripMenuItem,
            this.regionalDataCacheToolStripMenuItem});
            this.settingsMenu.Name = "contextMenuStrip1";
            this.settingsMenu.Size = new System.Drawing.Size(207, 226);
            this.settingsMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.PopupClosed);
            this.settingsMenu.Opening += new System.ComponentModel.CancelEventHandler(this.settingsMenu_Opening);
            this.settingsMenu.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.exploreMenu_PreviewKeyDown);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check for Updates...";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(203, 6);
            // 
            // feedbackToolStripMenuItem
            // 
            this.feedbackToolStripMenuItem.Name = "feedbackToolStripMenuItem";
            this.feedbackToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.feedbackToolStripMenuItem.Text = "Product Support...";
            this.feedbackToolStripMenuItem.Click += new System.EventHandler(this.feedbackToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(203, 6);
            // 
            // restoreDefaultsToolStripMenuItem
            // 
            this.restoreDefaultsToolStripMenuItem.Name = "restoreDefaultsToolStripMenuItem";
            this.restoreDefaultsToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.restoreDefaultsToolStripMenuItem.Text = "Restore Defaults";
            this.restoreDefaultsToolStripMenuItem.Click += new System.EventHandler(this.restoreDefaultsToolStripMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(203, 6);
            // 
            // advancedToolStripMenuItem
            // 
            this.advancedToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadQueueToolStripMenuItem,
            this.startQueueToolStripMenuItem,
            this.stopQueueToolStripMenuItem,
            this.tileLoadingThrottlingToolStripMenuItem,
            this.toolStripMenuItem8,
            this.DownloadMPC,
            this.toolStripSeparator10,
            this.saveCacheAsCabinetFileToolStripMenuItem,
            this.restoreCacheFromCabinetFileToolStripMenuItem,
            this.toolStripSeparator22,
            this.flushCacheToolStripMenuItem,
            this.toolStripSeparator18,
            this.showPerformanceDataToolStripMenuItem,
            this.toolStripSeparator19,
            this.toolStripMenuItem2,
            this.multiChanelCalibrationToolStripMenuItem,
            this.clientNodeListToolStripMenuItem,
            this.toolStripMenuItem6,
            this.sendLayersToProjectorServersToolStripMenuItem});
            this.advancedToolStripMenuItem.Name = "advancedToolStripMenuItem";
            this.advancedToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.advancedToolStripMenuItem.Text = "Advanced";
            this.advancedToolStripMenuItem.DropDownOpening += new System.EventHandler(this.advancedToolStripMenuItem_DropDownOpening);
            // 
            // downloadQueueToolStripMenuItem
            // 
            this.downloadQueueToolStripMenuItem.Name = "downloadQueueToolStripMenuItem";
            this.downloadQueueToolStripMenuItem.Size = new System.Drawing.Size(291, 22);
            this.downloadQueueToolStripMenuItem.Text = "Show Download Queue";
            this.downloadQueueToolStripMenuItem.Click += new System.EventHandler(this.showQueue_Click);
            // 
            // startQueueToolStripMenuItem
            // 
            this.startQueueToolStripMenuItem.Name = "startQueueToolStripMenuItem";
            this.startQueueToolStripMenuItem.Size = new System.Drawing.Size(291, 22);
            this.startQueueToolStripMenuItem.Text = "Start Queue";
            this.startQueueToolStripMenuItem.Click += new System.EventHandler(this.startQueue_Click);
            // 
            // stopQueueToolStripMenuItem
            // 
            this.stopQueueToolStripMenuItem.Name = "stopQueueToolStripMenuItem";
            this.stopQueueToolStripMenuItem.Size = new System.Drawing.Size(291, 22);
            this.stopQueueToolStripMenuItem.Text = "Stop Queue";
            this.stopQueueToolStripMenuItem.Click += new System.EventHandler(this.stopQueue_Click);
            // 
            // tileLoadingThrottlingToolStripMenuItem
            // 
            this.tileLoadingThrottlingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tpsToolStripMenuItem15,
            this.tpsToolStripMenuItem30,
            this.tpsToolStripMenuItem60,
            this.tpsToolStripMenuItem120,
            this.tpsToolStripMenuItemUnlimited});
            this.tileLoadingThrottlingToolStripMenuItem.Name = "tileLoadingThrottlingToolStripMenuItem";
            this.tileLoadingThrottlingToolStripMenuItem.Size = new System.Drawing.Size(291, 22);
            this.tileLoadingThrottlingToolStripMenuItem.Text = "Tile Loading Throttling";
            this.tileLoadingThrottlingToolStripMenuItem.DropDownOpening += new System.EventHandler(this.tileLoadingThrottlingToolStripMenuItem_DropDownOpening);
            // 
            // tpsToolStripMenuItem15
            // 
            this.tpsToolStripMenuItem15.Name = "tpsToolStripMenuItem15";
            this.tpsToolStripMenuItem15.Size = new System.Drawing.Size(126, 22);
            this.tpsToolStripMenuItem15.Text = "15 tps";
            this.tpsToolStripMenuItem15.Click += new System.EventHandler(this.tpsToolStripMenuItem15_Click);
            // 
            // tpsToolStripMenuItem30
            // 
            this.tpsToolStripMenuItem30.Name = "tpsToolStripMenuItem30";
            this.tpsToolStripMenuItem30.Size = new System.Drawing.Size(126, 22);
            this.tpsToolStripMenuItem30.Text = "30 tps";
            this.tpsToolStripMenuItem30.Click += new System.EventHandler(this.tpsToolStripMenuItem30_Click);
            // 
            // tpsToolStripMenuItem60
            // 
            this.tpsToolStripMenuItem60.Name = "tpsToolStripMenuItem60";
            this.tpsToolStripMenuItem60.Size = new System.Drawing.Size(126, 22);
            this.tpsToolStripMenuItem60.Text = "60 tps";
            this.tpsToolStripMenuItem60.Click += new System.EventHandler(this.tpsToolStripMenuItem60_Click);
            // 
            // tpsToolStripMenuItem120
            // 
            this.tpsToolStripMenuItem120.Name = "tpsToolStripMenuItem120";
            this.tpsToolStripMenuItem120.Size = new System.Drawing.Size(126, 22);
            this.tpsToolStripMenuItem120.Text = "120 tps";
            this.tpsToolStripMenuItem120.Click += new System.EventHandler(this.tpsToolStripMenuItem120_Click);
            // 
            // tpsToolStripMenuItemUnlimited
            // 
            this.tpsToolStripMenuItemUnlimited.Name = "tpsToolStripMenuItemUnlimited";
            this.tpsToolStripMenuItemUnlimited.Size = new System.Drawing.Size(126, 22);
            this.tpsToolStripMenuItemUnlimited.Text = "Unlimited";
            this.tpsToolStripMenuItemUnlimited.Click += new System.EventHandler(this.tpsToolStripMenuItemUnlimited_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(288, 6);
            // 
            // DownloadMPC
            // 
            this.DownloadMPC.Name = "DownloadMPC";
            this.DownloadMPC.Size = new System.Drawing.Size(291, 22);
            this.DownloadMPC.Text = "Download New Minor Planet Center Data";
            this.DownloadMPC.Click += new System.EventHandler(this.DownloadMPC_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(288, 6);
            // 
            // saveCacheAsCabinetFileToolStripMenuItem
            // 
            this.saveCacheAsCabinetFileToolStripMenuItem.Name = "saveCacheAsCabinetFileToolStripMenuItem";
            this.saveCacheAsCabinetFileToolStripMenuItem.Size = new System.Drawing.Size(291, 22);
            this.saveCacheAsCabinetFileToolStripMenuItem.Text = "Save Cache as Cabinet File...";
            this.saveCacheAsCabinetFileToolStripMenuItem.Click += new System.EventHandler(this.saveCacheAsCabinetFileToolStripMenuItem_Click);
            // 
            // restoreCacheFromCabinetFileToolStripMenuItem
            // 
            this.restoreCacheFromCabinetFileToolStripMenuItem.Name = "restoreCacheFromCabinetFileToolStripMenuItem";
            this.restoreCacheFromCabinetFileToolStripMenuItem.Size = new System.Drawing.Size(291, 22);
            this.restoreCacheFromCabinetFileToolStripMenuItem.Text = "Restore Cache from Cabinet File...";
            this.restoreCacheFromCabinetFileToolStripMenuItem.Click += new System.EventHandler(this.restoreCacheFromCabinetFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            this.toolStripSeparator22.Size = new System.Drawing.Size(288, 6);
            // 
            // flushCacheToolStripMenuItem
            // 
            this.flushCacheToolStripMenuItem.Name = "flushCacheToolStripMenuItem";
            this.flushCacheToolStripMenuItem.Size = new System.Drawing.Size(291, 22);
            this.flushCacheToolStripMenuItem.Text = "Flush Cache";
            this.flushCacheToolStripMenuItem.Click += new System.EventHandler(this.helpFlush_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(288, 6);
            // 
            // showPerformanceDataToolStripMenuItem
            // 
            this.showPerformanceDataToolStripMenuItem.CheckOnClick = true;
            this.showPerformanceDataToolStripMenuItem.Name = "showPerformanceDataToolStripMenuItem";
            this.showPerformanceDataToolStripMenuItem.Size = new System.Drawing.Size(291, 22);
            this.showPerformanceDataToolStripMenuItem.Text = "Show Performance Data";
            this.showPerformanceDataToolStripMenuItem.Click += new System.EventHandler(this.showPerformanceDataToolStripMenuItem_Click);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(288, 6);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.MergeIndex = 1;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(291, 22);
            this.toolStripMenuItem2.Text = "Master Controller";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.menuMasterControler_Click);
            // 
            // multiChanelCalibrationToolStripMenuItem
            // 
            this.multiChanelCalibrationToolStripMenuItem.Name = "multiChanelCalibrationToolStripMenuItem";
            this.multiChanelCalibrationToolStripMenuItem.Size = new System.Drawing.Size(291, 22);
            this.multiChanelCalibrationToolStripMenuItem.Text = "Multi-Channel Calibration";
            this.multiChanelCalibrationToolStripMenuItem.Click += new System.EventHandler(this.multiChanelCalibrationToolStripMenuItem_Click);
            // 
            // clientNodeListToolStripMenuItem
            // 
            this.clientNodeListToolStripMenuItem.Name = "clientNodeListToolStripMenuItem";
            this.clientNodeListToolStripMenuItem.Size = new System.Drawing.Size(291, 22);
            this.clientNodeListToolStripMenuItem.Text = "Projector Server List";
            this.clientNodeListToolStripMenuItem.Click += new System.EventHandler(this.clientNodeListToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(288, 6);
            // 
            // sendLayersToProjectorServersToolStripMenuItem
            // 
            this.sendLayersToProjectorServersToolStripMenuItem.Name = "sendLayersToProjectorServersToolStripMenuItem";
            this.sendLayersToProjectorServersToolStripMenuItem.Size = new System.Drawing.Size(291, 22);
            this.sendLayersToProjectorServersToolStripMenuItem.Text = "Send Layers to Projector Servers";
            this.sendLayersToProjectorServersToolStripMenuItem.Click += new System.EventHandler(this.sendLayersToProjectorServersToolStripMenuItem_Click);
            // 
            // mIDIControllerSetupToolStripMenuItem
            // 
            this.mIDIControllerSetupToolStripMenuItem.Name = "mIDIControllerSetupToolStripMenuItem";
            this.mIDIControllerSetupToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.mIDIControllerSetupToolStripMenuItem.Text = "Controller Setup...";
            this.mIDIControllerSetupToolStripMenuItem.Click += new System.EventHandler(this.mIDIControllerSetupToolStripMenuItem_Click);
            // 
            // xBoxControllerSetupToolStripMenuItem
            // 
            this.xBoxControllerSetupToolStripMenuItem.Name = "xBoxControllerSetupToolStripMenuItem";
            this.xBoxControllerSetupToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.xBoxControllerSetupToolStripMenuItem.Text = "Xbox Controller Setup...";
            this.xBoxControllerSetupToolStripMenuItem.Click += new System.EventHandler(this.xBoxControllerSetupToolStripMenuItem_Click);
            // 
            // remoteAccessControlToolStripMenuItem
            // 
            this.remoteAccessControlToolStripMenuItem.Name = "remoteAccessControlToolStripMenuItem";
            this.remoteAccessControlToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.remoteAccessControlToolStripMenuItem.Text = "Remote Access Control...";
            this.remoteAccessControlToolStripMenuItem.Click += new System.EventHandler(this.remoteAccessControlToolStripMenuItem_Click);
            // 
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(203, 6);
            // 
            // selectLanguageToolStripMenuItem
            // 
            this.selectLanguageToolStripMenuItem.Name = "selectLanguageToolStripMenuItem";
            this.selectLanguageToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.selectLanguageToolStripMenuItem.Text = "Select Language...";
            this.selectLanguageToolStripMenuItem.Click += new System.EventHandler(this.selectLanguageToolStripMenuItem_Click);
            // 
            // regionalDataCacheToolStripMenuItem
            // 
            this.regionalDataCacheToolStripMenuItem.Name = "regionalDataCacheToolStripMenuItem";
            this.regionalDataCacheToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
            this.regionalDataCacheToolStripMenuItem.Text = "Regional Data Cache...";
            this.regionalDataCacheToolStripMenuItem.Click += new System.EventHandler(this.regionalDataCacheToolStripMenuItem_Click);
            // 
            // viewMenu
            // 
            this.viewMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.viewMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetCameraMenuItem,
            this.showTouchControlsToolStripMenuItem,
            this.monochromeStyleToolStripMenuItem,
            this.allowUnconstrainedTiltToolStripMenuItem,
            this.toolStripSeparator9,
            this.startupToolStripMenuItem,
            this.toolStripMenuItem5,
            this.copyCurrentViewToClipboardToolStripMenuItem,
            this.copyShortCutToThisViewToClipboardToolStripMenuItem,
            this.saveCurrentViewImageToFileToolStripMenuItem,
            this.setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem,
            this.exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem,
            this.enableExport3dCitiesModeToolStripMenuItem,
            this.exportCurrentCitiesViewAs3DMeshToolStripMenuItem,
            this.toolStripSeparator21,
            this.screenBroadcastToolStripMenuItem,
            this.toolStripSeparator14,
            this.imageStackToolStripMenuItem,
            this.showLayerManagerToolStripMenuItem,
            this.toolStripSeparator20,
            this.stereoToolStripMenuItem,
            this.oculusVRHeadsetToolStripMenuItem,
            this.expermentalToolStripMenuItem,
            this.toggleFullScreenModeF11ToolStripMenuItem,
            this.multiSampleAntialiasingToolStripMenuItem,
            this.lockVerticalSyncToolStripMenuItem,
            this.targetFrameRateToolStripMenuItem});
            this.viewMenu.Name = "contextMenuStrip1";
            this.viewMenu.Size = new System.Drawing.Size(341, 540);
            this.viewMenu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.PopupClosed);
            this.viewMenu.Opening += new System.ComponentModel.CancelEventHandler(this.viewMenu_Opening);
            this.viewMenu.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.exploreMenu_PreviewKeyDown);
            // 
            // resetCameraMenuItem
            // 
            this.resetCameraMenuItem.Name = "resetCameraMenuItem";
            this.resetCameraMenuItem.Size = new System.Drawing.Size(340, 22);
            this.resetCameraMenuItem.Text = "Reset Camera";
            this.resetCameraMenuItem.Click += new System.EventHandler(this.resetCameraToolStripMenuItem_Click);
            // 
            // showTouchControlsToolStripMenuItem
            // 
            this.showTouchControlsToolStripMenuItem.Name = "showTouchControlsToolStripMenuItem";
            this.showTouchControlsToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.showTouchControlsToolStripMenuItem.Text = "Show On-Screen Controls";
            this.showTouchControlsToolStripMenuItem.Click += new System.EventHandler(this.showTouchControlsToolStripMenuItem_Click);
            // 
            // monochromeStyleToolStripMenuItem
            // 
            this.monochromeStyleToolStripMenuItem.Name = "monochromeStyleToolStripMenuItem";
            this.monochromeStyleToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.monochromeStyleToolStripMenuItem.Text = "Monochrome Style";
            this.monochromeStyleToolStripMenuItem.Click += new System.EventHandler(this.monochromeStyleToolStripMenuItem_Click);
            // 
            // allowUnconstrainedTiltToolStripMenuItem
            // 
            this.allowUnconstrainedTiltToolStripMenuItem.Name = "allowUnconstrainedTiltToolStripMenuItem";
            this.allowUnconstrainedTiltToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.allowUnconstrainedTiltToolStripMenuItem.Text = "Allow Unconstrained Tilt";
            this.allowUnconstrainedTiltToolStripMenuItem.Click += new System.EventHandler(this.allowUnconstrainedTiltToolStripMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(337, 6);
            // 
            // startupToolStripMenuItem
            // 
            this.startupToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.earthToolStripMenuItem,
            this.planetToolStripMenuItem,
            this.skyToolStripMenuItem,
            this.panoramaToolStripMenuItem,
            this.solarSystemToolStripMenuItem,
            this.lastToolStripMenuItem,
            this.randomToolStripMenuItem});
            this.startupToolStripMenuItem.Name = "startupToolStripMenuItem";
            this.startupToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.startupToolStripMenuItem.Text = "Startup Look At";
            this.startupToolStripMenuItem.DropDownOpening += new System.EventHandler(this.startupToolStripMenuItem_DropDownOpening);
            // 
            // earthToolStripMenuItem
            // 
            this.earthToolStripMenuItem.Name = "earthToolStripMenuItem";
            this.earthToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.earthToolStripMenuItem.Text = "Earth";
            this.earthToolStripMenuItem.Click += new System.EventHandler(this.earthToolStripMenuItem_Click);
            // 
            // planetToolStripMenuItem
            // 
            this.planetToolStripMenuItem.Name = "planetToolStripMenuItem";
            this.planetToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.planetToolStripMenuItem.Text = "Planet";
            this.planetToolStripMenuItem.Click += new System.EventHandler(this.planetToolStripMenuItem_Click);
            // 
            // skyToolStripMenuItem
            // 
            this.skyToolStripMenuItem.Name = "skyToolStripMenuItem";
            this.skyToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.skyToolStripMenuItem.Text = "Sky";
            this.skyToolStripMenuItem.Click += new System.EventHandler(this.skyToolStripMenuItem_Click);
            // 
            // panoramaToolStripMenuItem
            // 
            this.panoramaToolStripMenuItem.Name = "panoramaToolStripMenuItem";
            this.panoramaToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.panoramaToolStripMenuItem.Text = "Panorama";
            this.panoramaToolStripMenuItem.Click += new System.EventHandler(this.panoramaToolStripMenuItem_Click);
            // 
            // solarSystemToolStripMenuItem
            // 
            this.solarSystemToolStripMenuItem.Name = "solarSystemToolStripMenuItem";
            this.solarSystemToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.solarSystemToolStripMenuItem.Text = "Solar System";
            this.solarSystemToolStripMenuItem.Click += new System.EventHandler(this.solarSystemToolStripMenuItem_Click);
            // 
            // lastToolStripMenuItem
            // 
            this.lastToolStripMenuItem.Name = "lastToolStripMenuItem";
            this.lastToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.lastToolStripMenuItem.Text = "Last";
            this.lastToolStripMenuItem.Click += new System.EventHandler(this.lastToolStripMenuItem_Click);
            // 
            // randomToolStripMenuItem
            // 
            this.randomToolStripMenuItem.Name = "randomToolStripMenuItem";
            this.randomToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.randomToolStripMenuItem.Text = "Random";
            this.randomToolStripMenuItem.Click += new System.EventHandler(this.randomToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(337, 6);
            // 
            // copyCurrentViewToClipboardToolStripMenuItem
            // 
            this.copyCurrentViewToClipboardToolStripMenuItem.Name = "copyCurrentViewToClipboardToolStripMenuItem";
            this.copyCurrentViewToClipboardToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.copyCurrentViewToClipboardToolStripMenuItem.Text = "Copy Current View Image";
            this.copyCurrentViewToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyCurrentViewToClipboardToolStripMenuItem_Click);
            // 
            // copyShortCutToThisViewToClipboardToolStripMenuItem
            // 
            this.copyShortCutToThisViewToClipboardToolStripMenuItem.Name = "copyShortCutToThisViewToClipboardToolStripMenuItem";
            this.copyShortCutToThisViewToClipboardToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.copyShortCutToThisViewToClipboardToolStripMenuItem.Text = "Copy Shortcut to this View";
            this.copyShortCutToThisViewToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyShortcutMenuItem_Click);
            // 
            // saveCurrentViewImageToFileToolStripMenuItem
            // 
            this.saveCurrentViewImageToFileToolStripMenuItem.Name = "saveCurrentViewImageToFileToolStripMenuItem";
            this.saveCurrentViewImageToFileToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.saveCurrentViewImageToFileToolStripMenuItem.Text = "Save Current View Image to File...";
            this.saveCurrentViewImageToFileToolStripMenuItem.Click += new System.EventHandler(this.saveCurrentViewImageToFileToolStripMenuItem_Click);
            // 
            // setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem
            // 
            this.setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem.Name = "setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem";
            this.setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem.Text = "Set Current View as Windows Desktop Background";
            this.setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem.Click += new System.EventHandler(this.setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem_Click);
            // 
            // exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem
            // 
            this.exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem.Name = "exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem";
            this.exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem.Text = "Export Current View as STL File for 3D Printing...";
            this.exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem.Click += new System.EventHandler(this.exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem_Click);
            // 
            // exportCurrentCitiesViewAs3DMeshToolStripMenuItem
            // 
            this.exportCurrentCitiesViewAs3DMeshToolStripMenuItem.Name = "exportCurrentCitiesViewAs3DMeshToolStripMenuItem";
            this.exportCurrentCitiesViewAs3DMeshToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.exportCurrentCitiesViewAs3DMeshToolStripMenuItem.Text = "Export Current Cities View as 3D Mesh...";
            this.exportCurrentCitiesViewAs3DMeshToolStripMenuItem.Click += new System.EventHandler(this.exportCurrentCitiesViewAs3DMeshToolStripMenuItem_Click);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(337, 6);
            // 
            // screenBroadcastToolStripMenuItem
            // 
            this.screenBroadcastToolStripMenuItem.Name = "screenBroadcastToolStripMenuItem";
            this.screenBroadcastToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.screenBroadcastToolStripMenuItem.Text = "Screen Broadcast...";
            this.screenBroadcastToolStripMenuItem.Click += new System.EventHandler(this.screenBroadcastToolStripMenuItem_Click);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(337, 6);
            this.toolStripSeparator14.Visible = false;
            // 
            // imageStackToolStripMenuItem
            // 
            this.imageStackToolStripMenuItem.Name = "imageStackToolStripMenuItem";
            this.imageStackToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.imageStackToolStripMenuItem.Text = "Image Stack";
            this.imageStackToolStripMenuItem.Click += new System.EventHandler(this.imageStackToolStripMenuItem_Click);
            // 
            // showLayerManagerToolStripMenuItem
            // 
            this.showLayerManagerToolStripMenuItem.Name = "showLayerManagerToolStripMenuItem";
            this.showLayerManagerToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.showLayerManagerToolStripMenuItem.Text = "Show Layer Manager";
            this.showLayerManagerToolStripMenuItem.Click += new System.EventHandler(this.showLayerManagerToolStripMenuItem_Click);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(337, 6);
            // 
            // stereoToolStripMenuItem
            // 
            this.stereoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enabledToolStripMenuItem,
            this.anaglyphToolStripMenuItem,
            this.anaglyphYellowBlueToolStripMenuItem,
            this.sideBySideProjectionToolStripMenuItem,
            this.sideBySideCrossEyedToolStripMenuItem,
            this.alternatingLinesOddToolStripMenuItem,
            this.alternatingLinesEvenToolStripMenuItem,
            this.oculusRiftToolStripMenuItem});
            this.stereoToolStripMenuItem.Name = "stereoToolStripMenuItem";
            this.stereoToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.stereoToolStripMenuItem.Text = "Stereo";
            this.stereoToolStripMenuItem.DropDownOpening += new System.EventHandler(this.stereoToolStripMenuItem_DropDownOpening);
            // 
            // enabledToolStripMenuItem
            // 
            this.enabledToolStripMenuItem.Name = "enabledToolStripMenuItem";
            this.enabledToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.enabledToolStripMenuItem.Text = "Disabled";
            this.enabledToolStripMenuItem.Click += new System.EventHandler(this.enabledToolStripMenuItem_Click);
            // 
            // anaglyphToolStripMenuItem
            // 
            this.anaglyphToolStripMenuItem.Name = "anaglyphToolStripMenuItem";
            this.anaglyphToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.anaglyphToolStripMenuItem.Text = "Anaglyph (Red-Cyan)";
            this.anaglyphToolStripMenuItem.Click += new System.EventHandler(this.anaglyphToolStripMenuItem_Click);
            // 
            // anaglyphYellowBlueToolStripMenuItem
            // 
            this.anaglyphYellowBlueToolStripMenuItem.Name = "anaglyphYellowBlueToolStripMenuItem";
            this.anaglyphYellowBlueToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.anaglyphYellowBlueToolStripMenuItem.Text = "Anaglyph (Yellow-Blue)";
            this.anaglyphYellowBlueToolStripMenuItem.Click += new System.EventHandler(this.anaglyphYellowBlueToolStripMenuItem_Click);
            // 
            // sideBySideProjectionToolStripMenuItem
            // 
            this.sideBySideProjectionToolStripMenuItem.Name = "sideBySideProjectionToolStripMenuItem";
            this.sideBySideProjectionToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.sideBySideProjectionToolStripMenuItem.Text = "Side by Side Projection";
            this.sideBySideProjectionToolStripMenuItem.Click += new System.EventHandler(this.sideBySideProjectionToolStripMenuItem_Click);
            // 
            // sideBySideCrossEyedToolStripMenuItem
            // 
            this.sideBySideCrossEyedToolStripMenuItem.Name = "sideBySideCrossEyedToolStripMenuItem";
            this.sideBySideCrossEyedToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.sideBySideCrossEyedToolStripMenuItem.Text = "Side by Side Cross-Eyed";
            this.sideBySideCrossEyedToolStripMenuItem.Click += new System.EventHandler(this.sideBySideCrossEyedToolStripMenuItem_Click);
            // 
            // alternatingLinesOddToolStripMenuItem
            // 
            this.alternatingLinesOddToolStripMenuItem.Name = "alternatingLinesOddToolStripMenuItem";
            this.alternatingLinesOddToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.alternatingLinesOddToolStripMenuItem.Text = "Alternating Lines Odd";
            this.alternatingLinesOddToolStripMenuItem.Click += new System.EventHandler(this.alternatingLinesOddToolStripMenuItem_Click);
            // 
            // alternatingLinesEvenToolStripMenuItem
            // 
            this.alternatingLinesEvenToolStripMenuItem.Name = "alternatingLinesEvenToolStripMenuItem";
            this.alternatingLinesEvenToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.alternatingLinesEvenToolStripMenuItem.Text = "Alternating Lines Even";
            this.alternatingLinesEvenToolStripMenuItem.Click += new System.EventHandler(this.alternatingLinesEvenToolStripMenuItem_Click);
            // 
            // oculusRiftToolStripMenuItem
            // 
            this.oculusRiftToolStripMenuItem.Name = "oculusRiftToolStripMenuItem";
            this.oculusRiftToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.oculusRiftToolStripMenuItem.Text = "Oculus Rift";
            this.oculusRiftToolStripMenuItem.Click += new System.EventHandler(this.oculusRiftToolStripMenuItem_Click);
            // 
            // oculusVRHeadsetToolStripMenuItem
            // 
            this.oculusVRHeadsetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.monoModeToolStripMenuItem,
            this.startInOculusModeToolStripMenuItem});
            this.oculusVRHeadsetToolStripMenuItem.Name = "oculusVRHeadsetToolStripMenuItem";
            this.oculusVRHeadsetToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.oculusVRHeadsetToolStripMenuItem.Text = "Oculus VR Headset";
            this.oculusVRHeadsetToolStripMenuItem.DropDownOpening += new System.EventHandler(this.oculusVRHeadsetToolStripMenuItem_DropDownOpening);
            // 
            // monoModeToolStripMenuItem
            // 
            this.monoModeToolStripMenuItem.Name = "monoModeToolStripMenuItem";
            this.monoModeToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.monoModeToolStripMenuItem.Text = "Mono Mode";
            this.monoModeToolStripMenuItem.Click += new System.EventHandler(this.monoModeToolStripMenuItem_Click);
            // 
            // startInOculusModeToolStripMenuItem
            // 
            this.startInOculusModeToolStripMenuItem.Name = "startInOculusModeToolStripMenuItem";
            this.startInOculusModeToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.startInOculusModeToolStripMenuItem.Text = "Start in Oculus Mode";
            this.startInOculusModeToolStripMenuItem.Click += new System.EventHandler(this.startInOculusModeToolStripMenuItem_Click);
            // 
            // expermentalToolStripMenuItem
            // 
            this.expermentalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullDomeToolStripMenuItem,
            this.newFullDomeViewInstanceToolStripMenuItem,
            this.toolStripMenuItem15,
            this.domeSetupToolStripMenuItem,
            this.listenUpBoysToolStripMenuItem,
            this.toolStripMenuItem11,
            this.detachMainViewToSecondMonitor,
            this.detachMainViewToThirdMonitorToolStripMenuItem,
            this.toolStripMenuItem10,
            this.fullDomePreviewToolStripMenuItem});
            this.expermentalToolStripMenuItem.Name = "expermentalToolStripMenuItem";
            this.expermentalToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.expermentalToolStripMenuItem.Text = "Single Channel Full Dome";
            // 
            // fullDomeToolStripMenuItem
            // 
            this.fullDomeToolStripMenuItem.Name = "fullDomeToolStripMenuItem";
            this.fullDomeToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.fullDomeToolStripMenuItem.Text = "Dome View";
            this.fullDomeToolStripMenuItem.Click += new System.EventHandler(this.fullDomeToolStripMenuItem_Click);
            // 
            // newFullDomeViewInstanceToolStripMenuItem
            // 
            this.newFullDomeViewInstanceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.monitorOneToolStripMenuItem,
            this.monitorTwoToolStripMenuItem,
            this.monitorThreeToolStripMenuItem,
            this.monitorFourToolStripMenuItem,
            this.monitorFiveToolStripMenuItem,
            this.monitorSixToolStripMenuItem,
            this.monitorSevenToolStripMenuItem,
            this.monitorEightToolStripMenuItem});
            this.newFullDomeViewInstanceToolStripMenuItem.Name = "newFullDomeViewInstanceToolStripMenuItem";
            this.newFullDomeViewInstanceToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.newFullDomeViewInstanceToolStripMenuItem.Text = "New Full Dome View Instance";
            this.newFullDomeViewInstanceToolStripMenuItem.DropDownOpening += new System.EventHandler(this.newFullDomeViewInstanceToolStripMenuItem_DropDownOpening);
            // 
            // monitorOneToolStripMenuItem
            // 
            this.monitorOneToolStripMenuItem.Name = "monitorOneToolStripMenuItem";
            this.monitorOneToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.monitorOneToolStripMenuItem.Tag = "1";
            this.monitorOneToolStripMenuItem.Text = "Monitor One";
            this.monitorOneToolStripMenuItem.Click += new System.EventHandler(this.CreateDomeInstanceToolStripMenuItem_Click);
            // 
            // monitorTwoToolStripMenuItem
            // 
            this.monitorTwoToolStripMenuItem.Name = "monitorTwoToolStripMenuItem";
            this.monitorTwoToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.monitorTwoToolStripMenuItem.Tag = "2";
            this.monitorTwoToolStripMenuItem.Text = "Monitor Two";
            this.monitorTwoToolStripMenuItem.Click += new System.EventHandler(this.CreateDomeInstanceToolStripMenuItem_Click);
            // 
            // monitorThreeToolStripMenuItem
            // 
            this.monitorThreeToolStripMenuItem.Name = "monitorThreeToolStripMenuItem";
            this.monitorThreeToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.monitorThreeToolStripMenuItem.Tag = "3";
            this.monitorThreeToolStripMenuItem.Text = "Monitor Three";
            this.monitorThreeToolStripMenuItem.Click += new System.EventHandler(this.CreateDomeInstanceToolStripMenuItem_Click);
            // 
            // monitorFourToolStripMenuItem
            // 
            this.monitorFourToolStripMenuItem.Name = "monitorFourToolStripMenuItem";
            this.monitorFourToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.monitorFourToolStripMenuItem.Tag = "4";
            this.monitorFourToolStripMenuItem.Text = "Monitor Four";
            this.monitorFourToolStripMenuItem.Click += new System.EventHandler(this.CreateDomeInstanceToolStripMenuItem_Click);
            // 
            // monitorFiveToolStripMenuItem
            // 
            this.monitorFiveToolStripMenuItem.Name = "monitorFiveToolStripMenuItem";
            this.monitorFiveToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.monitorFiveToolStripMenuItem.Tag = "5";
            this.monitorFiveToolStripMenuItem.Text = "Monitor Five";
            this.monitorFiveToolStripMenuItem.Click += new System.EventHandler(this.CreateDomeInstanceToolStripMenuItem_Click);
            // 
            // monitorSixToolStripMenuItem
            // 
            this.monitorSixToolStripMenuItem.Name = "monitorSixToolStripMenuItem";
            this.monitorSixToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.monitorSixToolStripMenuItem.Tag = "6";
            this.monitorSixToolStripMenuItem.Text = "Monitor Six";
            this.monitorSixToolStripMenuItem.Click += new System.EventHandler(this.CreateDomeInstanceToolStripMenuItem_Click);
            // 
            // monitorSevenToolStripMenuItem
            // 
            this.monitorSevenToolStripMenuItem.Name = "monitorSevenToolStripMenuItem";
            this.monitorSevenToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.monitorSevenToolStripMenuItem.Tag = "7";
            this.monitorSevenToolStripMenuItem.Text = "Monitor Seven";
            this.monitorSevenToolStripMenuItem.Click += new System.EventHandler(this.CreateDomeInstanceToolStripMenuItem_Click);
            // 
            // monitorEightToolStripMenuItem
            // 
            this.monitorEightToolStripMenuItem.Name = "monitorEightToolStripMenuItem";
            this.monitorEightToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.monitorEightToolStripMenuItem.Tag = "8";
            this.monitorEightToolStripMenuItem.Text = "Monitor Eight";
            this.monitorEightToolStripMenuItem.Click += new System.EventHandler(this.CreateDomeInstanceToolStripMenuItem_Click);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(268, 6);
            // 
            // domeSetupToolStripMenuItem
            // 
            this.domeSetupToolStripMenuItem.Name = "domeSetupToolStripMenuItem";
            this.domeSetupToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.domeSetupToolStripMenuItem.Text = "Dome Setup";
            this.domeSetupToolStripMenuItem.Click += new System.EventHandler(this.domeSetupToolStripMenuItem_Click);
            // 
            // listenUpBoysToolStripMenuItem
            // 
            this.listenUpBoysToolStripMenuItem.Name = "listenUpBoysToolStripMenuItem";
            this.listenUpBoysToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.listenUpBoysToolStripMenuItem.Text = "Start Listener";
            this.listenUpBoysToolStripMenuItem.Click += new System.EventHandler(this.listenUpBoysToolStripMenuItem_Click);
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(268, 6);
            // 
            // detachMainViewToSecondMonitor
            // 
            this.detachMainViewToSecondMonitor.Name = "detachMainViewToSecondMonitor";
            this.detachMainViewToSecondMonitor.Size = new System.Drawing.Size(271, 22);
            this.detachMainViewToSecondMonitor.Text = "Detach Main View to Second Monitor";
            this.detachMainViewToSecondMonitor.Click += new System.EventHandler(this.detatchMainViewMenuItem_Click);
            // 
            // detachMainViewToThirdMonitorToolStripMenuItem
            // 
            this.detachMainViewToThirdMonitorToolStripMenuItem.Name = "detachMainViewToThirdMonitorToolStripMenuItem";
            this.detachMainViewToThirdMonitorToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.detachMainViewToThirdMonitorToolStripMenuItem.Text = "Detach Main View to Third Monitor";
            this.detachMainViewToThirdMonitorToolStripMenuItem.Click += new System.EventHandler(this.detachMainViewToThirdMonitorToolStripMenuItem_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(268, 6);
            // 
            // fullDomePreviewToolStripMenuItem
            // 
            this.fullDomePreviewToolStripMenuItem.Name = "fullDomePreviewToolStripMenuItem";
            this.fullDomePreviewToolStripMenuItem.Size = new System.Drawing.Size(271, 22);
            this.fullDomePreviewToolStripMenuItem.Text = "Full Dome Preview";
            this.fullDomePreviewToolStripMenuItem.Click += new System.EventHandler(this.fullDomePreviewToolStripMenuItem_Click);
            // 
            // toggleFullScreenModeF11ToolStripMenuItem
            // 
            this.toggleFullScreenModeF11ToolStripMenuItem.Name = "toggleFullScreenModeF11ToolStripMenuItem";
            this.toggleFullScreenModeF11ToolStripMenuItem.ShortcutKeyDisplayString = "F11";
            this.toggleFullScreenModeF11ToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.toggleFullScreenModeF11ToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.toggleFullScreenModeF11ToolStripMenuItem.Text = "Toggle Full Screen Mode";
            this.toggleFullScreenModeF11ToolStripMenuItem.Click += new System.EventHandler(this.toggleFullScreenModeF11ToolStripMenuItem_Click);
            // 
            // multiSampleAntialiasingToolStripMenuItem
            // 
            this.multiSampleAntialiasingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem,
            this.fourSamplesToolStripMenuItem,
            this.eightSamplesToolStripMenuItem});
            this.multiSampleAntialiasingToolStripMenuItem.Name = "multiSampleAntialiasingToolStripMenuItem";
            this.multiSampleAntialiasingToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.multiSampleAntialiasingToolStripMenuItem.Text = "Multi-Sample Antialiasing";
            this.multiSampleAntialiasingToolStripMenuItem.DropDownOpening += new System.EventHandler(this.multiSampleAntialiasingToolStripMenuItem_DropDownOpening);
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.noneToolStripMenuItem.Text = "None";
            this.noneToolStripMenuItem.Click += new System.EventHandler(this.noneToolStripMenuItem_Click);
            // 
            // fourSamplesToolStripMenuItem
            // 
            this.fourSamplesToolStripMenuItem.Name = "fourSamplesToolStripMenuItem";
            this.fourSamplesToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.fourSamplesToolStripMenuItem.Text = "Four Samples";
            this.fourSamplesToolStripMenuItem.Click += new System.EventHandler(this.fourSamplesToolStripMenuItem_Click);
            // 
            // eightSamplesToolStripMenuItem
            // 
            this.eightSamplesToolStripMenuItem.Name = "eightSamplesToolStripMenuItem";
            this.eightSamplesToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.eightSamplesToolStripMenuItem.Text = "Eight Samples";
            this.eightSamplesToolStripMenuItem.Click += new System.EventHandler(this.eightSamplesToolStripMenuItem_Click);
            // 
            // lockVerticalSyncToolStripMenuItem
            // 
            this.lockVerticalSyncToolStripMenuItem.Name = "lockVerticalSyncToolStripMenuItem";
            this.lockVerticalSyncToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.lockVerticalSyncToolStripMenuItem.Text = "Lock Vertical Sync";
            this.lockVerticalSyncToolStripMenuItem.Click += new System.EventHandler(this.lockVerticalSyncToolStripMenuItem_Click);
            // 
            // targetFrameRateToolStripMenuItem
            // 
            this.targetFrameRateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fpsToolStripMenuItemUnlimited,
            this.fPSToolStripMenuItem60,
            this.fPSToolStripMenuItem30,
            this.fPSToolStripMenuItem24});
            this.targetFrameRateToolStripMenuItem.Name = "targetFrameRateToolStripMenuItem";
            this.targetFrameRateToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.targetFrameRateToolStripMenuItem.Text = "Target Frame Rate";
            this.targetFrameRateToolStripMenuItem.DropDownOpening += new System.EventHandler(this.targetFrameRateToolStripMenuItem_DropDownOpening);
            // 
            // fpsToolStripMenuItemUnlimited
            // 
            this.fpsToolStripMenuItemUnlimited.Name = "fpsToolStripMenuItemUnlimited";
            this.fpsToolStripMenuItemUnlimited.Size = new System.Drawing.Size(126, 22);
            this.fpsToolStripMenuItemUnlimited.Text = "Unlimited";
            this.fpsToolStripMenuItemUnlimited.Click += new System.EventHandler(this.fpsToolStripMenuItemUnlimited_Click);
            // 
            // fPSToolStripMenuItem60
            // 
            this.fPSToolStripMenuItem60.Name = "fPSToolStripMenuItem60";
            this.fPSToolStripMenuItem60.Size = new System.Drawing.Size(126, 22);
            this.fPSToolStripMenuItem60.Text = "60 FPS";
            this.fPSToolStripMenuItem60.Click += new System.EventHandler(this.fPSToolStripMenuItem60_Click);
            // 
            // fPSToolStripMenuItem30
            // 
            this.fPSToolStripMenuItem30.Name = "fPSToolStripMenuItem30";
            this.fPSToolStripMenuItem30.Size = new System.Drawing.Size(126, 22);
            this.fPSToolStripMenuItem30.Text = "30 FPS";
            this.fPSToolStripMenuItem30.Click += new System.EventHandler(this.fPSToolStripMenuItem30_Click);
            // 
            // fPSToolStripMenuItem24
            // 
            this.fPSToolStripMenuItem24.Name = "fPSToolStripMenuItem24";
            this.fPSToolStripMenuItem24.Size = new System.Drawing.Size(126, 22);
            this.fPSToolStripMenuItem24.Text = "24 FPS";
            this.fPSToolStripMenuItem24.Click += new System.EventHandler(this.fPSToolStripMenuItem24_Click);
            // 
            // enableExport3dCitiesModeToolStripMenuItem
            // 
            this.enableExport3dCitiesModeToolStripMenuItem.Name = "enableExport3dCitiesModeToolStripMenuItem";
            this.enableExport3dCitiesModeToolStripMenuItem.Size = new System.Drawing.Size(340, 22);
            this.enableExport3dCitiesModeToolStripMenuItem.Text = "Enable Export 3d Cities Mode";
            this.enableExport3dCitiesModeToolStripMenuItem.Click += new System.EventHandler(this.enableExport3dCitiesModeToolStripMenuItem_Click);
            // 
            // StatupTimer
            // 
            this.StatupTimer.Enabled = true;
            this.StatupTimer.Interval = 1000;
            this.StatupTimer.Tick += new System.EventHandler(this.StatupTimer_Tick);
            // 
            // SlideAdvanceTimer
            // 
            this.SlideAdvanceTimer.Interval = 10000;
            this.SlideAdvanceTimer.Tick += new System.EventHandler(this.SlideAdvanceTimer_Tick);
            // 
            // TourEndCheck
            // 
            this.TourEndCheck.Enabled = true;
            this.TourEndCheck.Interval = 1000;
            this.TourEndCheck.Tick += new System.EventHandler(this.TourEndCheck_Tick);
            // 
            // autoSaveTimer
            // 
            this.autoSaveTimer.Enabled = true;
            this.autoSaveTimer.Interval = 60000;
            this.autoSaveTimer.Tick += new System.EventHandler(this.autoSaveTimer_Tick);
            // 
            // DeviceHeartbeat
            // 
            this.DeviceHeartbeat.Enabled = true;
            this.DeviceHeartbeat.Tick += new System.EventHandler(this.DeviceHeartbeat_Tick);
            // 
            // kioskTitleBar
            // 
            this.kioskTitleBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("kioskTitleBar.BackgroundImage")));
            this.kioskTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.kioskTitleBar.Location = new System.Drawing.Point(0, 34);
            this.kioskTitleBar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.kioskTitleBar.Name = "kioskTitleBar";
            this.kioskTitleBar.Size = new System.Drawing.Size(1442, 34);
            this.kioskTitleBar.TabIndex = 9;
            this.kioskTitleBar.Visible = false;
            // 
            // renderWindow
            // 
            this.renderWindow.BackColor = System.Drawing.Color.Black;
            this.renderWindow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderWindow.Location = new System.Drawing.Point(0, 34);
            this.renderWindow.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.renderWindow.Name = "renderWindow";
            this.renderWindow.Size = new System.Drawing.Size(1442, 328);
            this.renderWindow.TabIndex = 8;
            this.renderWindow.TabStop = false;
            this.renderWindow.Click += new System.EventHandler(this.renderWindow_Click);
            this.renderWindow.Paint += new System.Windows.Forms.PaintEventHandler(this.renderWindow_Paint);
            this.renderWindow.MouseClick += new System.Windows.Forms.MouseEventHandler(this.renderWindow_MouseClick);
            this.renderWindow.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.renderWindow_MouseDoubleClick);
            this.renderWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.renderWindow_MouseDown);
            this.renderWindow.MouseEnter += new System.EventHandler(this.renderWindow_MouseEnter);
            this.renderWindow.MouseLeave += new System.EventHandler(this.renderWindow_MouseLeave);
            this.renderWindow.MouseHover += new System.EventHandler(this.renderWindow_MouseHover);
            this.renderWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.renderWindow_MouseMove);
            this.renderWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.renderWindow_MouseUp);
            this.renderWindow.Resize += new System.EventHandler(this.renderWindow_Resize);
            // 
            // menuTabs
            // 
            this.menuTabs.BackColor = System.Drawing.Color.Black;
            this.menuTabs.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("menuTabs.BackgroundImage")));
            this.menuTabs.CurrentTour = null;
            this.menuTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuTabs.IsVisible = true;
            this.menuTabs.Location = new System.Drawing.Point(0, 0);
            this.menuTabs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.menuTabs.Name = "menuTabs";
            this.menuTabs.SelectedTabIndex = 0;
            this.menuTabs.Size = new System.Drawing.Size(1442, 34);
            this.menuTabs.StartX = 140;
            this.menuTabs.TabIndex = 4;
            this.menuTabs.TabClicked += new TerraViewer.TabClickedEventHandler(this.menuTabs_TabClicked);
            this.menuTabs.MenuClicked += new TerraViewer.MenuClickedEventHandler(this.menuTabs_MenuClicked);
            this.menuTabs.ControlEvent += new TerraViewer.ControlEventHandler(this.menuTabs_ControlEvent);
            this.menuTabs.Load += new System.EventHandler(this.menuTabs_Load);
            // 
            // Earth3d
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1442, 362);
            this.Controls.Add(this.kioskTitleBar);
            this.Controls.Add(this.renderWindow);
            this.Controls.Add(this.menuTabs);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "Earth3d";
            this.Text = "Microsoft WorldWide Telescope";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Earth3d_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Earth3d_FormClosed);
            this.Load += new System.EventHandler(this.Earth3d_Load);
            this.Shown += new System.EventHandler(this.Earth3d_Shown);
            this.ResizeBegin += new System.EventHandler(this.Earth3d_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.Earth3d_ResizeEnd);
            this.Click += new System.EventHandler(this.Earth3d_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Earth3d_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWndow_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Earth3d_KeyUp);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Earth3d_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainWndow_MouseDown);
            this.MouseEnter += new System.EventHandler(this.Earth3d_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.Earth3d_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainWndow_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainWndow_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MainWndow_MouseWheel);
            this.Move += new System.EventHandler(this.Earth3d_Move);
            this.Resize += new System.EventHandler(this.Earth3d_Resize);
            this.contextMenu.ResumeLayout(false);
            this.communitiesMenu.ResumeLayout(false);
            this.searchMenu.ResumeLayout(false);
            this.toursMenu.ResumeLayout(false);
            this.telescopeMenu.ResumeLayout(false);
            this.exploreMenu.ResumeLayout(false);
            this.settingsMenu.ResumeLayout(false);
            this.viewMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        RenderHost renderHost;
        public void FreeFloatRenderWindow(int targetMonitor)
        {

            if (renderHost == null)
            {
                this.Controls.Remove(this.renderWindow);
                renderHost = new RenderHost();
                renderHost.Show();
                renderHost.Controls.Add(renderWindow);
                int id = 0;
                if (Screen.FromControl(this).DeviceName == Screen.AllScreens[0].DeviceName)
                {
                    id = targetMonitor;
                }

                if (id == 0 && targetMonitor > 1)
                {
                    if (Screen.FromControl(this).DeviceName == Screen.AllScreens[1].DeviceName)
                    {
                        id = targetMonitor;
                    }
                }


                UiTools.ShowFullScreen(renderHost, false, id);
                RenderContext11.Resize(renderWindow.ClientSize.Height, renderWindow.ClientSize.Width);
            }
        }

        public void FreeFloatRenderWindow(string deviceName)
        {

            if (renderHost == null)
            {
                this.Controls.Remove(this.renderWindow);
                int id = 0;
                int foundId = -1;
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (deviceName.StartsWith(screen.DeviceName))
                    {
                        foundId = id;
                        break;
                    }
                    id++;
                }
                if (foundId > -1)
                {
                    renderHost = new RenderHost();
                    renderHost.Show();
                    renderHost.Controls.Add(renderWindow);
                    UiTools.ShowFullScreen(renderHost, false, id);
                    RenderContext11.Resize(renderWindow.ClientSize.Height, renderWindow.ClientSize.Width);
                }
            }
        }

        public void AttachRenderWindow()
        {
            if (renderHost != null)
            {
                renderHost.Controls.Remove(renderWindow);
                this.Controls.Add(renderWindow);
                renderHost.Hide();
                renderHost.Close();
                renderHost = null;
            }
        }

        private void detatchMainViewMenuItem_Click(object sender, EventArgs e)
        {
            if (detachMainViewToSecondMonitor.Checked || detachMainViewToThirdMonitorToolStripMenuItem.Checked)
            {
                AttachRenderWindow();
                detachMainViewToThirdMonitorToolStripMenuItem.Checked = false;
                detachMainViewToSecondMonitor.Checked = false;
            }
            else
            {
                FreeFloatRenderWindow(1);
                detachMainViewToThirdMonitorToolStripMenuItem.Checked = false;
                detachMainViewToSecondMonitor.Checked = true;
            }
        }

        private void detachMainViewToThirdMonitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (detachMainViewToSecondMonitor.Checked || detachMainViewToThirdMonitorToolStripMenuItem.Checked)
            {
                AttachRenderWindow();
                detachMainViewToThirdMonitorToolStripMenuItem.Checked = false;
            }
            else
            {
                FreeFloatRenderWindow(2);
                detachMainViewToThirdMonitorToolStripMenuItem.Checked = true;
                detachMainViewToSecondMonitor.Checked = false;
            }
        }

       
        public bool InitializeGraphics()
        {
            if (RenderEngine.ReadyToRender)
            {
                return true;
            }

            try
            {
                if (Properties.Settings.Default.MultiSampling == 0)
                {
                    Properties.Settings.Default.MultiSampling = 1;
                }
                RenderContext11.MultiSampleCount = Math.Max(1, Properties.Settings.Default.MultiSampling);
                RenderContext11 = new RenderContext11(renderWindow, Properties.Settings.Default.RiftStartup);

                RenderEngine.ReadyToRender = true;
                pause = false;
                return true;
            }
            catch
            {
                // Catch any errors and return a failure
                return false;
            }
        }


        

        void device_DeviceResizing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (renderWindow.Height == 0 | renderWindow.Width == 0)
            {
                e.Cancel = true;
            }
        }
        public KmlViewInformation KmlViewInfo
        {
            get { return RenderEngine.KmlViewInfo; }
            set { RenderEngine.kmlViewInfo = value; }
        }


    

      


        public void SetLabelText(IPlace place, bool showText)
        {
            if (RenderEngine.label != null)
            {
                RenderEngine.label.Dispose();
                GC.SuppressFinalize(RenderEngine.label);
                RenderEngine.label = null;
            }
            if (place != null)
            {

                if (SolarSystemMode || RenderEngine.PlanetLike)
                {
                    if (RenderEngine.PlanetLike)
                    {
                        RenderEngine.label = new SkyLabel(RenderContext11, ((place.Lng / 15)) + 180, place.Lat, showText ? place.Name : "", LabelSytle.Telrad, 1.0);
                    }
                    else if (place.Classification == Classification.SolarSystem)
                    {
                        Vector3d temp = Planets.GetPlanet3dLocation((SolarSystemObjects)Planets.GetPlanetIDFromName(place.Name));

                        temp.Subtract(RenderEngine.viewCamera.ViewTarget);
                        temp.Subtract(RenderEngine.viewCamera.ViewTarget);

                        RenderEngine.label = new SkyLabel(RenderContext11, temp, showText ? place.Name : "", LabelSytle.Telrad);

                    }
                    else
                    {
                        RenderEngine.label = new SkyLabel(RenderContext11, place.RA, -place.Dec, showText ? place.Name : "", LabelSytle.Telrad, place.Distance != 0 ? place.Distance : 100000000.0);
                    }
                }
                else
                {
                    RenderEngine.label = new SkyLabel(RenderContext11, place.RA, place.Dec, showText ? place.Name : "", LabelSytle.Telrad, place.Distance != 0 ? place.Distance : 1.0);
                }
            }
        }


        public string Constellation
        {
            get { return RenderEngine.Constellation; }
        }

     

        int frameCount = 0;

        long lastSampleTime;

        
     
       
        static int lastGcCount = 0;
        public static float LastFPS = 0;
        static DateTime lastPing = DateTime.Now;

        public static int statusReport = 0;

        private void UpdateStats()
        {
            frameCount++;
            long ticks = HiResTimer.TickCount - lastSampleTime;

            double seconds = (double)(ticks / HiResTimer.Frequency);


            if (seconds > 1.0)
            {
                double frameRate = frameCount / seconds;
                sendMoveCount = 0;
                lastSampleTime = HiResTimer.TickCount;
                LastFPS = (float)frameRate;
                frameCount = 0;

                if (showPerfData)
                {
                    this.Text = Language.GetLocalizedText(3, "Microsoft WorldWide Telescope") + " - FPS:" + frameRate.ToString("0.0") + Language.GetLocalizedText(84, "  Tiles in View:") + Tile.TilesInView.ToString() + Language.GetLocalizedText(85, " Triangles Rendered:") + Tile.TrianglesRendered.ToString() + " Tiled Ready" + TileCache.GetReadyCount().ToString() + "Total Tiles:" + TileCache.GetTotalCount();
                }
            }

            if (config.Master == false)
            {
                TimeSpan ts = DateTime.Now - lastPing;
                if (ts.TotalSeconds > 20)
                {
                    NetControl.PingStatus();
                    statusReport++;
                    lastPing = DateTime.Now.AddMilliseconds(pingRandom.NextDouble() * 5000);
                }

            }


            Utils.frameNumber++;
            if (Utils.Logging)
            {
                if (Utils.logFilestream != null)
                {
                    ticks = HiResTimer.TickCount - Utils.lastRender;

                    int ms = (int)((ticks * 1000) / HiResTimer.Frequency);
                    int gcCount = GC.CollectionCount(2);
                    int thisCount = gcCount - lastGcCount;
                    lastGcCount = gcCount;


                    long memNow = GC.GetTotalMemory(false);
                    if (lastMem == 0)
                    {
                        lastMem = memNow;
                    }
                    long mem = memNow - lastMem;
                    lastMem = memNow;
                    Utils.logFilestream.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", Utils.frameNumber, Utils.masterSyncFrameNumber, ms, TileCache.tilesLoadedThisFrame, Tile.TexturesLoaded, thisCount, mem, statusReport);
                    TileCache.tilesLoadedThisFrame = 0;
                    Tile.TexturesLoaded = 0;
                    bgImagesetGets = 0;
                    bgImagesetFails = 0;
                    fgImagesetGets = 0;
                    fgImagesetFails = 0;
                    statusReport = 0;
                }
            }
        }

        long lastMem = 0;
        Random pingRandom = new Random();
        BlendState panoramaBlend = new BlendState();
       
        
       
        delegate void RenderDelegate();
        delegate void BackInitDelegate();
        public void RenderCrossThread()
        {
            Invoke(new RenderDelegate(RenderEngine.Render));
        }

        bool blink = false;
        DateTime lastBlink = DateTime.Now;

  



        double NetZoomRate = 0;
        private void UpdateNetControlState()
        {
            double factor = RenderEngine.lastFrameTime / (1.0 / 60.0);



            if (Math.Abs(NetZoomRate) > 4)
            {
                RenderEngine.ZoomFactor = RenderEngine.TargetZoom = RenderEngine.ZoomFactor * (1 + (NetZoomRate / 8000) * factor);

                if (RenderEngine.ZoomFactor > RenderEngine.ZoomMax)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom = RenderEngine.ZoomMax;
                }

                if (RenderEngine.ZoomFactor < RenderEngine.ZoomMin)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom = RenderEngine.ZoomMin;
                }
            }


            return;


        }

      

        int lastTimeSyncFrame = 0;

        public void UpdateNetworkStatus()
        {
            Settings.ignoreChanges = true;
            NetControl.SetSettingsBelndStates();
            NetControl.SetSolarSystemsSettingsBlendStates();

            if (lastTimeSyncFrame != NetControl.currnetSyncFrame)
            {
                SpaceTimeController.Now = NetControl.now;
            }

            if (Properties.Settings.Default.DomeTilt != NetControl.domeTilt)
            {
                Properties.Settings.Default.DomeTilt = NetControl.domeTilt;
                Earth3d.MainWindow.Config.DomeTilt = (float)NetControl.domeTilt;
            }

            if (Properties.Settings.Default.DomeAngle != NetControl.domeAngle)
            {
                Properties.Settings.Default.DomeAngle = (float)NetControl.domeAngle;
                Earth3d.MainWindow.Config.DomeAngle = (float)NetControl.domeAngle;
            }

            lastTimeSyncFrame = NetControl.currnetSyncFrame;
            SpaceTimeController.TimeRate = NetControl.timeRate;
            SpaceTimeController.SyncToClock = NetControl.timeRate != 0;
            SpaceTimeController.Altitude = NetControl.altitude;
            SpaceTimeController.Location = Coordinates.FromLatLng(NetControl.loclat, NetControl.loclng);
            this.SetLocation(NetControl.lat, NetControl.lng, NetControl.zoom, NetControl.cameraRotate, NetControl.cameraAngle, NetControl.foregroundImageSetHash,
                NetControl.backgroundImageSetHash, NetControl.blendOpacity, NetControl.runSetup, NetControl.flush, NetControl.target, NetControl.targetPoint, NetControl.solarSystemScale, NetControl.TrackingFrame);

            RenderEngine.viewCamera.DomeAlt = NetControl.domeAlt;
            RenderEngine.viewCamera.DomeAz = NetControl.domeAz;

            int currentVersion = Properties.Settings.Default.ColSettingsVersion;

            Properties.Settings.Default.ReticleAlt = NetControl.reticleAlt;
            Properties.Settings.Default.ReticleAz = NetControl.reticleAz;

            Properties.Settings.Default.ConstellationFiguresFilter.SetBits(NetControl.figuresFilter);
            Properties.Settings.Default.ConstellationNamesFilter.SetBits(NetControl.namesFilter);
            Properties.Settings.Default.ConstellationBoundariesFilter.SetBits(NetControl.bounderiesFilter);
            Properties.Settings.Default.ConstellationArtFilter.SetBits(NetControl.artFilter);

            if (NetControl.ColorVersionNumber != currentVersion)
            {
                NetControl.GetColorSettings();
            }
            Settings.ignoreChanges = false;
        }

        

        private void NotifyMoveComplete()
        {
            if (KmlAutoRefresh)
            {
                MyPlaces.UpdateNetworkLinks(KmlViewInfo);
            }
            SendMoveComplete();
        }

        internal void UpdateKmlViewInfo()
        {
            KmlViewInfo.bboxNorth = Math.Max(RenderEngine.CurrentViewCorners[0].Dec, RenderEngine.CurrentViewCorners[1].Dec);
            KmlViewInfo.bboxSouth = Math.Min(RenderEngine.CurrentViewCorners[2].Dec, RenderEngine.CurrentViewCorners[3].Dec);
            KmlViewInfo.bboxEast = Math.Max(RenderEngine.CurrentViewCorners[1].Lng, RenderEngine.CurrentViewCorners[2].Lng);
            KmlViewInfo.bboxWest = Math.Min(RenderEngine.CurrentViewCorners[0].Lng, RenderEngine.CurrentViewCorners[2].Lng);
            KmlViewInfo.viewMoving = false;
            KmlViewInfo.viewJustStopped = true;
            //todo Fill in completely from camera parameters..
            if (RenderEngine.KmlMarkers == null)
            {
                RenderEngine.KmlMarkers = new KmlLabels();
            }
        }


        private SharpDX.Direct3D11.InputLayout layout = null;
     

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {

        }
        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {

        }

        private void Earth3d_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                if (tourEdit != null)
                {
                    tourEdit.PauseTour();
                }

                TileCache.PurgeQueue();
                TileCache.ClearCache();
            }
            if (RenderContext11 != null)
            {
                RenderContext11.Resize(renderWindow.ClientSize.Height, renderWindow.ClientSize.Width);
            }

            if (RenderEngine.ReadyToRender)
            {

                SetAppMode(currentMode);
            }
        }

        private bool IsPaused()
        {
            return ((this.WindowState == FormWindowState.Minimized) || !this.Visible || pause);
        }

       


        static private void RegisterKnownFileTypes()
        {
            RegisterFileType(".wtt", Language.GetLocalizedText(87, "WorldWide Telescope Tour"));
            RegisterFileType(".wtml", Language.GetLocalizedText(88, "WorldWide Telescope Media List"));

        }

        static private void RegisterFileType(string extension, string friendlyName)
        {
            RegistryKey root = Registry.ClassesRoot;

            RegistryKey extensionKey = root.OpenSubKey(extension);
            if (extensionKey != null)
            {
                return;
            }
            Assembly runningAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            extensionKey = root.CreateSubKey(extension);

            extensionKey.SetValue("", "WorldWideTelescope" + extension);

            RegistryKey typeInfoKey = root.CreateSubKey("WorldWideTelescope" + extension);
            typeInfoKey.SetValue("", friendlyName);
            RegistryKey icon = typeInfoKey.CreateSubKey("DefaultIcon");
            icon.SetValue("", runningAssembly.Location + ",0");

            RegistryKey shellSubkey = typeInfoKey.CreateSubKey("shell");

            // Create a subkey for the "Open" verb
            RegistryKey openSubKey = shellSubkey.CreateSubKey("Open");

            openSubKey.SetValue("", "&Play Tour");


            RegistryKey cmdSubkey = openSubKey.CreateSubKey("command");
            cmdSubkey.SetValue("", runningAssembly.Location + " %1");

        }


        static private bool ShouldAutoUpdate()
        {
            RegistryKey root = Registry.CurrentUser;

            RegistryKey wwtKey = root.OpenSubKey("Software\\Microsoft\\WorldWide Telescope");
            if (wwtKey == null)
            {
                return true;
            }

            return ((int)wwtKey.GetValue("AutoUpdates", 1)) != 0;
        }

        static private string GetIDCrlPath()
        {
            RegistryKey root = Registry.LocalMachine;

            RegistryKey wwtKey = root.OpenSubKey(@"Software\Microsoft\IdentityCRL");
            if (wwtKey == null)
            {
                return "";
            }

            return (string)wwtKey.GetValue("TargetDir", "");
        }

        /*
         * Cross process block
         * 
         * 
         * 
         */

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        [DllImport("user32")]
        public static extern uint RegisterWindowMessage(string message);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);

        public static uint AlertMessage;

        private static void SendWMData(IntPtr hwnd, string argData)
        {

            File.WriteAllText(Properties.Settings.Default.CahceDirectory + @"\launchfile.txt", argData);
            PostMessage(hwnd, AlertMessage, 0, IntPtr.Zero);

        }
        public static bool closeWelcome = false;

        public bool CloseWelcome
        {
            get { return closeWelcome; }
            set { closeWelcome = value; }
        }

        Message message = new Message();
        protected override void WndProc(ref Message m)
        {

            if (myMouse != null)
            {
                message = m;
                myMouse.ProcessMessage(message);
            }


            base.WndProc(ref m);
        }
        // end

        protected override void OnResize(EventArgs e)
        {
            //if (RenderContext.Device != null)
            //{

            //    RestoreDevice();
            //}
            try
            {
                base.OnResize(e);
            }
            catch
            {
            }
        }


        public static string launchTourFile = "";

        [DllImport("User32.dll")]

        public static extern int ShowWindowAsync(IntPtr hWnd, int swCommand);
        enum ShowWindowConstants
        {
            SW_HIDE = 0, SW_SHOWNORMAL = 1, SW_NORMAL = 1, SW_SHOWMINIMIZED = 2, SW_SHOWMAXIMIZED = 3, SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4, SW_SHOW = 5, SW_MINIMIZE = 6, SW_SHOWMINNOACTIVE = 7, SW_SHOWNA = 8, SW_RESTORE = 9, SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11, SW_MAX = 11
        };



        public static bool TouchKiosk = false;
        public static bool NoUi = false;

        public static bool DomeViewer = false;
        static bool DumpShaders = false;
        public static bool RestartedWithoutTour = false;

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, UInt32 dwFlags);

        [DllImport("user32")]
        public static extern bool ChangeWindowMessageFilter(uint msg, uint flags);


        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetProcessDPIAware();


        public static void PulseForUpdate()
        {
            Properties.Settings.Default.PulseMeForUpdate = !Properties.Settings.Default.PulseMeForUpdate;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main(string[] args)
        {
            CultureInfo culture = new CultureInfo("en-US", false);
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            Application.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.AboveNormal;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            DateTime now = DateTime.Now;
            bool singleInstance = true;


            foreach (string arg in args)
            {

                if (arg == "-logging")
                {
                    Utils.Logging = true;
                }


                if (arg == "-kiosk")
                {
                    TouchKiosk = true;
                }

                if (arg == "-domeviewer")
                {
                    DomeViewer = true;
                    singleInstance = false;
                    HideSplash = true;
                    TileCache.NodeID++;
                }

                if (arg == "-noui")
                {
                    NoUi = true;
                }

                if (arg == "-dumpshaders")
                {
                    DumpShaders = true;
                }

                if (arg.StartsWith("-screen:"))
                {
                    try
                    {
                        TargetScreenId = int.Parse(arg.Substring(arg.LastIndexOf(":") + 1));
                    }
                    catch
                    {
                    }
                }

                if (arg.StartsWith("-detach:"))
                {
                    try
                    {
                        DetachScreenId = int.Parse(arg.Substring(arg.LastIndexOf(":") + 1));
                    }
                    catch
                    {
                    }
                }
            }

            AlertMessage = RegisterWindowMessage("WWT Launch Tour");
            ChangeWindowMessageFilter(AlertMessage, 1);

            if (args.Length > 0)
            {
                if (args[0] == "launch")
                {
                    Process.Start("wwtexplorer.exe");
                    return;
                }
                if (args[0] == "restart")
                {
                    singleInstance = false;
                    HideSplash = true;
                    RestartedWithoutTour = true;
                }

                if (args[0].ToLower().EndsWith(".kml") || args[0].ToLower().EndsWith(".kmz") || args[0].ToLower().EndsWith(".wtt") || args[0].ToLower().EndsWith(".wtml") || args[0].ToLower().EndsWith(".wwtfig") || args[0].ToLower().EndsWith(".wwtl"))
                {
                    if (File.Exists(args[0]))
                    {
                        launchTourFile = args[0];
                    }
                }
            }

            if (singleInstance)
            {
                Process[] RunningProcesses = Process.GetProcessesByName("WWTExplorer");


                if (RunningProcesses.Length > 1)
                {
                    foreach (Process p in RunningProcesses)
                    {
                        if (p.Id != Process.GetCurrentProcess().Id)
                        {
                            SendWMData(p.MainWindowHandle, launchTourFile);
                        }
                    }
                    return;
                }
            }
            AppSettings.SettingsBase = new SettingsWrapper();

            PulseMe.PulseForUpdate = Earth3d.PulseForUpdate;

            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            CheckDefaultProperties(true);
            defaultWebProxy = WebRequest.DefaultWebProxy;
            UpdateProxySettings();


            Language.CurrentLanguage =
                new Language(Properties.Settings.Default.LanguageName,
                             Properties.Settings.Default.LanguageUrl,
                             Properties.Settings.Default.LanguageCode,
                             Properties.Settings.Default.ExploreRootUrl,
                             Properties.Settings.Default.ImageSetUrl,
                             Properties.Settings.Default.SharedCacheServer);

            CacheProxy.BaseUrl = Properties.Settings.Default.SharedCacheServer;

            try
            {
                if (!Earth3d.CheckForUpdates(false))
                {
                    return;
                }
            }
            catch
            {
            }

            try
            {
                RegisterKnownFileTypes();
            }
            catch
            {
            }


            // Check for failed Startup
            if (CheckStartFlag())
            {
                if (UiTools.ShowMessageBox(Language.GetLocalizedText(688, "WorldWide Telescope failed to complete the last startup attempt. Would you like WorldWide Telescope to attempt an auto reset of the data directory?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    ResetDataDirectory();
                }
            }
            SetStartFlag();



            using (Earth3d frm = new Earth3d())
            {
                //Stopwatch sw = new Stopwatch();
                try
                {
                    frm.Show();
                    frm.Refresh();
                    if (!frm.Created)
                    {
                        return;
                    }
                }
                catch
                {
                    return;
                }


                MainLoop(frm);

            }
            if (LanguageReboot)
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

                Process.Start(path, "restart");
            }
        }


        private static ApplicationRecoveryCallback crashCallback;

        private static void SetupRecovery()
        {
            crashCallback = new ApplicationRecoveryCallback(CrashCallback);

            IntPtr cb = Marshal.GetFunctionPointerForDelegate(crashCallback);

            RegisterApplicationRecoveryCallback(cb, IntPtr.Zero, 6000, 0);
        }


        private static int CrashCallback(IntPtr pvParameter)
        {
            // if (ProjectorServer)
            {

                File.WriteAllText(@"c:\wwtconfig\crashdump.txt", "Unhandled Exception in Current Domain");

                LanguageReboot = true;
                Earth3d.CloseNow = true;
                //     return 0;
            }

            ApplicationRecoveryFinished(true);
            return 0;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (RenderEngine.ProjectorServer)
            {

                File.WriteAllText(@"c:\wwtconfig\crashdump.txt", "Unhandled Exception in Current Domain");

                LanguageReboot = true;
                Earth3d.CloseNow = true;
                return;
            }
        }

        public static void MainLoop(Earth3d frm)
        {
            // Hook the application's idle event     
            System.Windows.Forms.Application.AddMessageFilter(new DataMessageFilter());
            System.Windows.Forms.Application.Idle += new EventHandler(OnApplicationIdle);
            System.Windows.Forms.Application.Run(frm);

        }

        private static void OnApplicationIdle(object sender, EventArgs e)
        {
            while (AppStillIdle)
            {
                if (CloseNow)
                {
                    Earth3d.MainWindow.Close();
                    CloseNow = false;
                }
                else
                {
                    if (Earth3d.MainWindow != null)
                    {
                        RenderEngine.Engine.Render();
                    }
                }
            }
        }

        private static bool AppStillIdle
        {
            get
            {
                MessageS msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }


        //
        [StructLayout(LayoutKind.Sequential)]
        public struct MessageS
        {
            public IntPtr hWnd;
            public int msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [System.Security.SuppressUnmanagedCodeSecurity] // We won't use this maliciously
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PeekMessage(out MessageS msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);


        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (RenderEngine.ProjectorServer)
            {
                LanguageReboot = true;
                Earth3d.CloseNow = true;

                File.WriteAllText(@"c:\wwtconfig\crashdump.txt", e.Exception.Message + e.Exception.Source + e.Exception.StackTrace);

                return;
            }

            if (starting)
            {
                if (UiTools.ShowMessageBox(Language.GetLocalizedText(689, "WorldWide Telescope encountered an error starting up. Would you like WorldWide Telescope to attempt a reset of the data directory and restart?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    ResetDataDirectory();
                    ResetStartFlag();
                    Process.Start("wwtexplorer.exe", "restart");
                    Process.GetCurrentProcess().Kill();

                }
            }
            else
            {
                if (UiTools.ShowMessageBox(Language.GetLocalizedText(690, "WorldWide Telescope has encountered an error. Click OK to restart or Cancel to attempt to ignore the error and continue"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    Process.Start("wwtexplorer.exe", "restart");
                    Process.GetCurrentProcess().Kill();

                }
            }
        }

        static bool starting = true;
        private static void SetStartFlag()
        {
            string startFlagFilename = string.Format(@"{0}\wwtstartup.flag", Path.GetTempPath());
            File.WriteAllText(startFlagFilename, "Starting");
            starting = true;
        }

        private static void ResetDataDirectory()
        {
            ExtractDataCabinet(true);
        }

        private static void ResetStartFlag()
        {
            string startFlagFilename = string.Format(@"{0}\wwtstartup.flag", Path.GetTempPath());
            File.Delete(startFlagFilename);
            starting = false;
        }

        private static bool CheckStartFlag()
        {
            string startFlagFilename = string.Format(@"{0}\wwtstartup.flag", Path.GetTempPath());

            return File.Exists(startFlagFilename);

        }

        static IWebProxy defaultWebProxy = null;

        public static void UpdateProxySettings()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.ProxyServer))
            {
                try
                {
                    Uri proxyURI = new Uri(String.Format("http://{0}:{1}", Properties.Settings.Default.ProxyServer, Properties.Settings.Default.ProxyPort));
                    System.Net.WebProxy proxy = new System.Net.WebProxy(proxyURI);
                    proxy.UseDefaultCredentials = true;
                    WebRequest.DefaultWebProxy = proxy;
                    return;
                }
                catch
                {
                }

            }

            WebRequest.DefaultWebProxy = defaultWebProxy;

        }

        static bool resetProperties = false;
        private static void CheckDefaultProperties(bool checkDataCabinet)
        {
            if (Properties.Settings.Default.UpgradeNeeded && !resetProperties)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.SolarSystemCosmos.TargetState = true;
                Properties.Settings.Default.SolarSystemOverlays.TargetState = false;
                Properties.Settings.Default.ImageQuality = 100;
                Properties.Settings.Default.ImageSetUrl = "http://www.worldwidetelescope.org/wwtweb/catalog.aspx?X=ImageSets5";
                Properties.Settings.Default.UpgradeNeeded = false;
            }

            if (string.IsNullOrEmpty(Properties.Settings.Default.CahceDirectory))
            {
                Properties.Settings.Default.CahceDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\WorldWideTelescope\\";
                string tempString = Properties.Settings.Default.CahceDirectory;
            }

            if (Properties.Settings.Default.UserRatingGUID == Guid.Empty)
            {
                Properties.Settings.Default.UserRatingGUID = Guid.NewGuid();
            }
            bool extractData = false;

            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + "data"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + "data");
                extractData = true;
            }

            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + "data\\figures"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + "data\\figures");
            }

            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + "data\\wms"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + "data\\wms");
            }

            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + "data\\figures"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + "data\\figures");
            }

            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + "buttons"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + "buttons");
            }

            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + "thumbnails"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + "thumbnails");
            }

            //TODO We need to update this via a flag to force upgrades past v5.1 to work
            if (!File.Exists(Properties.Settings.Default.CahceDirectory + "data\\wwtv5.2.7.txt"))
            {
                extractData = true;
            }

            if (extractData && checkDataCabinet)
            {
                ExtractDataCabinet(false);
            }


        }

        private static void ExtractDataCabinet(bool eraseFirst)
        {

            string appdir = Path.GetDirectoryName(Application.ExecutablePath);
            string dataDir = Properties.Settings.Default.CahceDirectory + "data";
            if (eraseFirst)
            {
                if (Directory.Exists(dataDir))
                {
                    Directory.Delete(dataDir, true);
                }
            }
            string filename = appdir + "\\datafiles.cabinet";
            FileCabinet cab = new FileCabinet(filename, dataDir);
            cab.Extract();
            File.WriteAllText(Properties.Settings.Default.CahceDirectory + "data\\wwtv5.2.7.txt", "WWT Version 5.5.7 installed");
        }

        int mouseDownX;
        int mouseDownY;


      

        
        Coordinates measureStart;
        Coordinates measureEnd;

       
        public bool Measuring
        {
            get { return RenderEngine.measuring; }
            set
            {
                if (RenderEngine.measureLines != null)
                {
                    RenderEngine.measureLines.Clear();
                }
                 RenderEngine.measuring = value;
            }
        }

        private bool dragging = false;
        private bool spinning = false;
        private bool angle = false;
        private bool moved = false;


        private void MainWndow_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {


        }
        IPlace contextMenuTargetObject = null;
        private void MainWndow_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        void ShowPropertiesForPoint(Point pntCenter)
        {
            if (contextPanel != null)
            {
                // TODO fix this for earth, plantes, panoramas
                Coordinates result = GetCoordinatesForScreenPoint(pntCenter.X, pntCenter.Y);
                string constellation = RenderEngine.constellationCheck.FindConstellationForPoint(result.RA, result.Dec);
                contextPanel.Constellation = Constellations.FullName(constellation);
                IPlace closetPlace = ContextSearch.FindClosestMatch(constellation, result.RA, result.Dec, RenderEngine.ZoomFactor / 1300);

                if (closetPlace == null)
                {
                    closetPlace = new TourPlace(Language.GetLocalizedText(90, "No Object"), result.Dec, result.RA, Classification.Unidentified, constellation, ImageSetType.Sky, -1);
                }

                ShowPropertiesMenu(closetPlace, pntCenter);
            }
        }


        private void ShowObjectResolveMenu(IPlace[] resultList, Point pntShow)
        {
            pntShow = new Point(pntShow.X + 100, pntShow.Y + 100);
            ContextMenuStrip ResolveMenu = new ContextMenuStrip();

            foreach (IPlace place in resultList)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(place.Name);
                menuItem.Tag = place;
                menuItem.Click += new EventHandler(ResolveAmbiguityMenu_Click);
                menuItem.MouseEnter += new EventHandler(ResolveAmbiguityMenu_MouseEnter);
                ResolveMenu.Items.Add(menuItem);

            }
            ResolveMenu.Show(pntShow);
        }

        void ResolveAmbiguityMenu_MouseEnter(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

            IPlace placePicked = (IPlace)menuItem.Tag;

            SetLabelText(placePicked, true);

        }

        void ResolveAmbiguityMenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

            IPlace placePicked = (IPlace)menuItem.Tag;

            ShowPropertiesMenu(placePicked, Cursor.Position);
        }
        bool starRegAdded = false;
        public void ShowContextMenu(IPlace place, Point pntShow, bool forCollection, bool readOnly)
        {
            getSDSSImageToolStripMenuItem.Enabled = IsInSDSSFootprint(place.RA, place.Dec);

            if (place.Name == Language.GetLocalizedText(90, "No Object"))
            {
                lookupOnSEDSToolStripMenuItem.Enabled = false;
                lookupOnWikipediaToolStripMenuItem.Enabled = false;
                publicationsToolStripMenuItem.Enabled = false;
                lookupOnSimbadToolStripMenuItem.Enabled = true;
                lookUpOnSDSSToolStripMenuItem.Enabled = getSDSSImageToolStripMenuItem.Enabled;
            }
            else
            {
                if (place.Name.ToLower().StartsWith("ngc") || place.Name.ToLower().StartsWith("ic") || place.Name.ToLower().StartsWith("m"))
                {
                    lookupOnSEDSToolStripMenuItem.Enabled = true;
                }
                else
                {
                    lookupOnSEDSToolStripMenuItem.Enabled = false;
                }

                getSDSSImageToolStripMenuItem.Enabled = getSDSSImageToolStripMenuItem.Enabled;
                lookupOnWikipediaToolStripMenuItem.Enabled = true;
                publicationsToolStripMenuItem.Enabled = true;
                lookupOnSimbadToolStripMenuItem.Enabled = place.Classification != Classification.SolarSystem;

            }

            // TODO replace this with another way of knowing whats near Maybe popup data near object
            nameToolStripMenuItem.Text = Language.GetLocalizedText(7, "Name:") + " " + place.Name;
            contextMenuTargetObject = place;

            if (AddFigurePointMenuItem == null)
            {
                AddFigurePointMenuItem = new ToolStripMenuItem(Language.GetLocalizedText(91, "Add Point to Constellation Figure"));
                AddFigurePointMenuItem.Click += new EventHandler(AddFigurePointMenuItem_Click);
            }

            if (contextMenu.Items.Contains(AddFigurePointMenuItem))
            {
                contextMenu.Items.Remove(AddFigurePointMenuItem);
            }

            if (figureEditor != null)
            {
                contextMenu.Items.Add(AddFigurePointMenuItem);
            }


            removeFromCollectionToolStripMenuItem.Visible = forCollection && !readOnly;
            editToolStripMenuItem.Visible = forCollection && !readOnly;
            removeFromImageCacheToolStripMenuItem.Visible = place.IsImage;
            setAsBackgroundImageryToolStripMenuItem.Visible = place.IsImage;
            setAsForegroundImageryToolStripMenuItem.Visible = place.IsImage;
            addToImageStackToolStripMenuItem.Visible = place.IsImage && imageStackVisible;
            ImagerySeperator.Visible = place.IsImage;

            // override items for communities
            Place communitiesPlace = contextMenuTargetObject as Place;
            if (communitiesPlace != null)
            {
                if (communitiesPlace.MSRComponentId > 0)
                {
                    removeFromCollectionToolStripMenuItem.Visible = true;
                }
            }

            contextMenu.Show(this, pntShow.X, pntShow.Y);
            return;
        }

        void starReg_Click(object sender, EventArgs e)
        {
            string url = String.Format("http://www.worldwidetelescope.org/wwtweb/starReg.aspx?ra={0}&dec={1}", Coordinates.FormatDMS(contextMenuTargetObject.RA), Coordinates.FormatDMS(contextMenuTargetObject.Dec));
            WebWindow.OpenUrl(url, false);

        }

        private void ShowPropertiesMenu(IPlace place, Point pntShow)
        {
            ObjectProperties.ShowAt(place, renderWindow.PointToScreen(pntShow));

            return;


        }


        ToolStripMenuItem AddFigurePointMenuItem;

        void AddFigurePointMenuItem_Click(object sender, EventArgs e)
        {
            figureEditor.AddFigurePoint(contextMenuTargetObject);
        }

        private void Earth3d_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        public double RAtoViewLng(double ra)
        {
            return (((180 - ((ra) / 24.0 * 360) - 180) + 540) % 360) - 180;
        }

        public bool IsMoving
        {
            get
            {
                return (RenderEngine.Mover != null || RenderEngine.zooming == true || RenderEngine.targetViewCamera != RenderEngine.viewCamera) || JoyInMotion;
            }
        }

        Point lastMousePosition = new Point(-1, -1);
        private void MainWndow_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        private void Earth3d_Click(object sender, EventArgs e)
        {

        }



        public Coordinates GetCoordinatesForScreenPoint(int x, int y)
        {
            Coordinates result = new Coordinates(0, 0);
            Rectangle rect = renderWindow.ClientRectangle;
            Vector3d PickRayOrig;
            Vector3d PickRayDir;
            Vector2d pt = new Vector2d(x, y);
            RenderEngine.TransformPickPointToWorldSpace(pt, rect.Width, rect.Height, out PickRayOrig, out PickRayDir);
            if (RenderEngine.Space)
            {
                result = Coordinates.CartesianToSpherical(PickRayDir);

            }
            else if (RenderEngine.PlanetLike)
            {
                bool inThere = RenderEngine.SphereIntersectRay(PickRayOrig, PickRayDir, out result);

            }

            return result;
        }

        public Coordinates GetCoordinatesForReticle(int id)
        {
            Coordinates result = new Coordinates(0, 0);
            Vector3d PickRayOrig;
            Vector3d PickRayDir;

            if (!Reticle.Reticles.ContainsKey(id))
            {
                return result;
            }
            Reticle ret = Reticle.Reticles[id];


            Vector3d pick = Coordinates.RADecTo3d(ret.Az / 15 - 6, ret.Alt, 1);

            double distance = (Math.Min(1, (.5 * (RenderEngine.ZoomFactor / 180)))) - 1 + 0.0001;

            PickRayOrig = new Vector3d(0, 0, distance);

            Matrix3d mat = RenderEngine.WorldMatrix * Matrix3d.RotationX(((config.TotalDomeTilt) / 180 * Math.PI));
            Matrix3d mat2 = RenderEngine.WorldMatrix * Matrix3d.RotationZ(((config.TotalDomeTilt) / 180 * Math.PI));

            mat.Invert();
            mat2.Invert();
            mat.MultiplyVector(ref pick);
            mat2.MultiplyVector(ref PickRayOrig);
            PickRayDir = pick;
            RenderEngine.SphereIntersectRay(PickRayOrig.Vector3, PickRayDir.Vector3, out result);
            return result;
        }


        public void GotoReticlePoint(int id)
        {
            Coordinates result = new Coordinates(0, 0);
            Vector3d PickRayOrig;
            Vector3d PickRayDir;

            if (!Reticle.Reticles.ContainsKey(id))
            {
                return;
            }
            Reticle ret = Reticle.Reticles[id];


            Vector3d pick = Coordinates.RADecTo3d(ret.Az / 15 - 6, ret.Alt, 1);

            double distance = (Math.Min(1, (.5 * (RenderEngine.ZoomFactor / 180)))) - 1 + 0.0001;

            PickRayOrig = new Vector3d(0, -distance, 0);

            Matrix3d mat = RenderEngine.WorldMatrix * Matrix3d.RotationX(((config.TotalDomeTilt) / 180 * Math.PI));

            mat.Invert();

            mat.MultiplyVector(ref pick);
            mat.MultiplyVector(ref PickRayOrig);
            PickRayDir = pick;
            Vector3d temp = new Vector3d(PickRayOrig);
            temp.Subtract(RenderEngine.viewCamera.ViewTarget);

            IPlace closetPlace = Grids.FindClosestObject(temp, new Vector3d(PickRayDir));

            if (closetPlace != null)
            {
                RenderEngine.GotoTarget(closetPlace, false, false, true);
            }
        }

      

        
        private void MainWndow_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (Properties.Settings.Default.FollowMouseOnZoom && !RenderEngine.PlanetLike)
                {
                    Coordinates point = GetCoordinatesForScreenPoint(e.X, e.Y);
                    if (RenderEngine.Space && Settings.Active.LocalHorizonMode && !RenderEngine.tracking)
                    {
                        Coordinates currentAltAz = Coordinates.EquitorialToHorizon(point, SpaceTimeController.Location, SpaceTimeController.Now);

                        RenderEngine.targetAlt = currentAltAz.Alt;
                        RenderEngine.targetAz = currentAltAz.Az;

                    }
                    else
                    {
                        RenderEngine.TargetLong = RAtoViewLng(point.RA);
                        RenderEngine.TargetLat = point.Lat;
                    }
                }
                RenderEngine.ZoomSpeed = (ZoomSpeeds)Properties.Settings.Default.ZoomSpeed;

                if (Math.Abs(e.Delta) == 120)
                {
                    if (e.Delta < 0)
                    {
                        ZoomOut();
                    }
                    else
                    {
                        ZoomIn();
                    }
                }
                else
                {
                    if (e.Delta < 0)
                    {
                        ZoomOut((double)Math.Abs(e.Delta) / 120.0);
                    }
                    else
                    {
                        ZoomIn((double)Math.Abs(e.Delta) / 120.0);
                    }
                }

            }
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {

            MoveView(-moveVector.X, moveVector.Y, false);
            ZoomView(ZoomVector / 400);
            this.RotateView(0, OrbitVector / 1000);





            switch (activeTouch)
            {
                case TouchControls.ZoomIn:
                    RenderEngine.ZoomSpeed = (ZoomSpeeds)Properties.Settings.Default.ZoomSpeed;
                    ZoomIn();
                    break;
                case TouchControls.ZoomOut:
                    RenderEngine.ZoomSpeed = (ZoomSpeeds)Properties.Settings.Default.ZoomSpeed;
                    ZoomOut();
                    break;
                case TouchControls.Up:
                    MoveUp();
                    break;
                case TouchControls.Down:
                    MoveDown();
                    break;
                case TouchControls.Left:
                    MoveLeft();
                    break;
                case TouchControls.Right:
                    MoveRight();
                    break;
                case TouchControls.Clockwise:
                    RotateView(0, .05);
                    break;
                case TouchControls.CounterClockwise:
                    RotateView(0, -.05);
                    break;
                case TouchControls.TiltUp:
                    RotateView(-.04, 0);
                    break;
                case TouchControls.TiltDown:
                    RotateView(.04, 0);
                    break;
                case TouchControls.TrackBall:
                    //Earth3d.MainWindow.MoveView(-moveVector.X, moveVector.Y, false);
                    break;
                case TouchControls.None:

                    break;
                default:
                    break;
            }


        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Tab:
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                    return true;
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                    return true;

            }
            return base.IsInputKey(keyData);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Shift | Keys.Right:
                case Keys.Shift | Keys.Left:
                case Keys.Shift | Keys.Up:
                case Keys.Shift | Keys.Down:
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    return false;
            }
            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    return false;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }




        protected override bool ProcessTabKey(bool forward)
        {
            return false;
        }

        public void RotateView(double upDown, double leftRight)
        {
            RenderEngine.CameraRotateTarget = (RenderEngine.CameraRotateTarget + leftRight);
            RenderEngine.CameraAngleTarget = (RenderEngine.CameraAngleTarget + upDown);

            if (RenderEngine.CameraAngleTarget < TiltMin)
            {
                RenderEngine.CameraAngleTarget = TiltMin;
            }

            if (RenderEngine.CameraAngleTarget > 0)
            {
                RenderEngine.CameraAngleTarget = 0;
            }
        }
        bool useAsymetricProj = false;

        private void MainWndow_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {


            if (uiController != null)
            {
                if (uiController.KeyDown(sender, e))
                {
                    return;
                }
            }
            if (Control.ModifierKeys == Keys.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.OemMinus:
                    case Keys.PageUp:
                    case Keys.Subtract:
                        RenderEngine.ZoomSpeed = (ZoomSpeeds)Properties.Settings.Default.ZoomSpeed;
                        ZoomOut();
                        break;
                    case Keys.PageDown:
                    case Keys.Oemplus:
                    case Keys.Add:
                        RenderEngine.ZoomSpeed = (ZoomSpeeds)Properties.Settings.Default.ZoomSpeed;
                        ZoomIn();
                        break;
                    case Keys.F1:
                        LaunchHelp();
                        break;
                    case Keys.F5:
                        TileCache.ClearCache();
                        Tile.fastLoad = true;
                        Tile.iTileBuildCount = 0;
                        break;
                    case Keys.Left:
                        RotateView(0, -.05);
                        break;
                    case Keys.Right:
                        RotateView(0, .05);
                        break;
                    case Keys.Up:
                        RotateView(-.01, 0);
                        break;
                    case Keys.Down:
                        RotateView(.01, 0);
                        break;

                }
            }
            else if (Control.ModifierKeys == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.F9:
                        config = new Config();
                        break;
                    case Keys.A:
                        useAsymetricProj = !useAsymetricProj;
                        break;
                    case Keys.F5:
                        Tile.PurgeRefresh = true;
                        RenderEngine.Render();
                        Tile.PurgeRefresh = false;
                        TileCache.ClearCache();
                        break;
                    case Keys.E:
                        if (uiController == null)
                        {
                            // We can't really tell where this image came from so dirty everything.
                            FolderBrowser.AllDirty = true;
                            UiController = new ImageAlignmentUI();
                            RenderEngine.StudyOpacity = 50;

                        }
                        else
                        {
                            if (uiController != null && uiController is ImageAlignmentUI)
                            {
                                ((ImageAlignmentUI)uiController).Close();
                            }
                            UiController = null;
                        }
                        break;

                    case Keys.L:
                        Utils.Logging = !Utils.Logging;

                        break;
                    case Keys.T:
                        RenderEngine.targetViewCamera = RenderEngine.viewCamera = RenderEngine.CustomTrackingParams;
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.C:
                        RenderEngine.iod *= .99f;
                        break;
                    case Keys.V:
                        RenderEngine.iod *= 1.01f;
                        break;
                    case Keys.F:
                        SpaceTimeController.Faster();
                        break;
                    case Keys.S:
                        SpaceTimeController.Slower();
                        break;
                    case Keys.H:
                        // turn friction on and off
                        Friction = !Friction;
                        break;
                    case Keys.P:
                        SpaceTimeController.PauseTime();
                        break;
                    case Keys.N:
                        SpaceTimeController.SetRealtimeNow();
                        break;
                    case Keys.B:
                        blink = !blink;
                        break;
                    case Keys.L:
                        //RenderContext11.SetLatency(3);
                        break;
                    case Keys.K:
                       // RenderContext11.SetLatency(1);
                        break;
                    case Keys.F5:
                        TileCache.ClearCache();
                        Tile.iTileBuildCount = 0;
                        break;
                    case Keys.F11:
                        ShowFullScreen(!fullScreen);
                        break;
                    case Keys.OemMinus:
                    case Keys.PageUp:
                    case Keys.Subtract:
                        RenderEngine.ZoomSpeed = (ZoomSpeeds)Properties.Settings.Default.ZoomSpeed;
                        ZoomOut();
                        break;
                    case Keys.Oemplus:
                    case Keys.PageDown:
                    case Keys.Add:
                        RenderEngine.ZoomSpeed = (ZoomSpeeds)Properties.Settings.Default.ZoomSpeed;
                        ZoomIn();
                        break;
                    case Keys.F3:

                        break;
                    case Keys.F1:
                        LaunchHelp();
                        break;
                    case Keys.F2:
                        RenderEngine.showWireFrame = !RenderEngine.showWireFrame;
                        break;
                    case Keys.Left:
                        Control c = UiTools.GetFocusControl();
                        MoveLeft();
                        break;
                    case Keys.Right:
                        MoveRight();
                        break;
                    case Keys.Up:
                        MoveUp();
                        break;
                    case Keys.Down:
                        MoveDown();
                        break;
                    case Keys.Space:
                    case Keys.Play:
                        if (TourEdit != null)
                        {
                            TourEdit.PlayNow(false);
                        }
                        break;
                    case Keys.Escape:
                        if (fullScreen)
                        {
                            ShowFullScreen(false);
                        }
                        break;
                }
            }
        }



        bool reverseMatrix = false;
        private static bool fullScreen = false;

        public static bool FullScreen
        {
            get { return Earth3d.fullScreen; }
            set { Earth3d.fullScreen = value; }
        }
        public void MoveDown()
        {
            MoveView(0, -50, false);
        }


        public void MoveUp()
        {
            MoveView(0, 50, false);

        }

        public void MoveRight()
        {
            MoveView(50, 0, false);

        }

        public void MoveLeft()
        {
            MoveView(-50, 0, false);
        }


        internal void MoveAndZoom(double leftRight, double upDown, double zoom)
        {
            MoveView(leftRight, upDown, false);
            ZoomView(zoom);
        }
        const double RC = (double)(3.1415927 / 180);

        public double GetPixelScaleX(bool mouseRelative)
        {
            double lat = RenderEngine.ViewLat;

            if (mouseRelative)
            {
                if (RenderEngine.Space && Settings.Active.GalacticMode)
                {
                    Point cursor = renderWindow.PointToClient(Cursor.Position);
                    Coordinates result = GetCoordinatesForScreenPoint(cursor.X, cursor.Y);

                    double[] gPoint = Coordinates.J2000toGalactic(result.RA * 15, result.Dec);

                    lat = gPoint[1];
                }
                else if (RenderEngine.Space && Settings.Active.LocalHorizonMode)
                {
                    Point cursor = renderWindow.PointToClient(Cursor.Position);
                    Coordinates currentAltAz = Coordinates.EquitorialToHorizon(GetCoordinatesForScreenPoint(cursor.X, cursor.Y), SpaceTimeController.Location, SpaceTimeController.Now);

                    lat = currentAltAz.Alt;
                }
                else
                {
                    Point cursor = renderWindow.PointToClient(Cursor.Position);
                    Coordinates result = GetCoordinatesForScreenPoint(cursor.X, cursor.Y);
                    lat = result.Lat;
                }
            }

            if (RenderEngine.currentImageSetfield != null && (RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Sky || RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Panorama || SandboxMode || SolarSystemMode || RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Earth || RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Planet))
            {
                double cosLat = 1;
                if (RenderEngine.ViewLat > 89.9999)
                {
                    cosLat = Math.Cos(89.9999 * RC);
                }
                else
                {
                    cosLat = Math.Cos(lat * RC);

                }

                double zz = (90 - RenderEngine.ZoomFactor / 6);
                double zcos = Math.Cos(zz * RC);

                return GetPixelScaleY() / Math.Max(zcos, cosLat);
            }
            else
            {
                return (((RenderEngine.baseTileDegrees / ((double)Math.Pow(2, RenderEngine.viewTileLevel))) / RenderEngine.tileSizeX) / 5) / Math.Max(.2, Math.Cos(RenderEngine.TargetLat));
            }

        }

        public double GetPixelScaleY()
        {
            if (SolarSystemMode)
            {
                if ((int)RenderEngine.SolarSystemTrack < (int)SolarSystemObjects.Custom)
                {
                    return Math.Min(.06, 545000 * Math.Tan(Math.PI / 4) * RenderEngine.ZoomFactor / renderWindow.ClientRectangle.Height);
                }
                else
                {

                    return .06;
                }
            }
            else if (RenderEngine.currentImageSetfield != null && (RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Sky || RenderEngine.currentImageSetfield.DataSetType == ImageSetType.Panorama))
            {
                double val = RenderEngine.FovAngle / renderWindow.ClientRectangle.Height;

                return val;
            }
            else if (SandboxMode)
            {
                return .06;
            }
            else
            {
                return ((RenderEngine.baseTileDegrees / ((double)Math.Pow(2, RenderEngine.viewTileLevel))) / (double)RenderEngine.tileSizeY) / 5;
            }
        }

        public void MoveView(double amountX, double amountY, bool mouseDrag)
        {
            if (RenderEngine.currentImageSetfield == null)
            {
                return;
            }
            RenderEngine.Tracking = false;
            double angle = Math.Atan2(amountY, amountX);
            double distance = Math.Sqrt(amountY * amountY + amountX * amountX);
            if (SolarSystemMode)
            {
                amountX = Math.Cos(angle - RenderEngine.CameraRotate) * distance;
                amountY = Math.Sin(angle - RenderEngine.CameraRotate) * distance;
            }
            else if (!RenderEngine.PlanetLike)
            {
                amountX = Math.Cos(angle + RenderEngine.CameraRotate) * distance;
                amountY = Math.Sin(angle + RenderEngine.CameraRotate) * distance;
            }
            else
            {
                amountX = Math.Cos(angle - RenderEngine.CameraRotate) * distance;
                amountY = Math.Sin(angle - RenderEngine.CameraRotate) * distance;
            }

            MoveViewNative(amountX, amountY, mouseDrag);
        }



        public void MoveViewNative(double amountX, double amountY, bool mouseDrag)
        {
            double scaleY = GetPixelScaleY();
            double scaleX = GetPixelScaleX(mouseDrag);


            if (RenderEngine.currentImageSetfield.DataSetType == ImageSetType.SolarSystem || SandboxMode)
            {
                if (scaleY > .05999)
                {
                    scaleX = scaleY;
                }
            }

            if (RenderEngine.Space && Settings.Active.GalacticMode)
            {
                amountX = -amountX;
            }

            if (RenderEngine.Space && (Settings.Active.LocalHorizonMode || Settings.Active.GalacticMode))
            {
                RenderEngine.targetAlt += (amountY) * scaleY;
                if (RenderEngine.targetAlt > Properties.Settings.Default.MaxLatLimit)
                {
                    RenderEngine.targetAlt = Properties.Settings.Default.MaxLatLimit;
                }
                if (RenderEngine.targetAlt < -Properties.Settings.Default.MaxLatLimit)
                {
                    RenderEngine.targetAlt = -Properties.Settings.Default.MaxLatLimit;
                }

            }
            else
            {
                RenderEngine.TargetLat += (amountY) * scaleY;

                if (RenderEngine.TargetLat > Properties.Settings.Default.MaxLatLimit)
                {
                    RenderEngine.TargetLat = Properties.Settings.Default.MaxLatLimit;
                }
                if (RenderEngine.TargetLat < -Properties.Settings.Default.MaxLatLimit)
                {
                    RenderEngine.TargetLat = -Properties.Settings.Default.MaxLatLimit;
                }
            }
            if (RenderEngine.Space && (Settings.Active.LocalHorizonMode || Settings.Active.GalacticMode))
            {
                RenderEngine.targetAz = ((RenderEngine.targetAz + amountX * scaleX) + 720) % 360;
            }
            else
            {
                RenderEngine.TargetLong += (amountX) * scaleX;

                RenderEngine.TargetLong = ((RenderEngine.TargetLong + 900.0) % 360.0) - 180.0;
            }
        }

        [DllImport("hhctrl.ocx", EntryPoint = "HtmlHelp", CharSet = CharSet.Unicode)]
        internal static extern IntPtr HtmlHelp(IntPtr hWndCaller, string helpFile, int command, string topic);

        public static void LaunchHelp()
        {
            WebWindow.OpenUrl("http://www.worldwidetelescope.org/Support/Index", true);

        }

        bool settingsDirty = false;
        public static bool FormIsClosing = false;
        private void Earth3d_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormIsClosing = true;
            if (RenderEngine.videoOut != null)
            {
                RenderEngine.videoOut.Close();
            }

            if (!CloseOpenToursOrAbort(true))
            {
                e.Cancel = true;
            }

            TourPopup.CloseTourPopups();

            RenderEngine.Initialized = false;

            if (renderHost != null)
            {
                renderHost.Close();
            }
        }

        private void Earth3d_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {

                if (sampConnection.Connected)
                {
                    sampConnection.Unregister();
                }
            }
            catch
            {
            }

            try
            {
                webServer.Shutdown();
            }
            catch
            {
            }

            if (Settings.MasterController)
            {
                NetControl.SaveNodeList();
            }

            MidiMapManager.Shutdown();

            if (config.Master == false)
            {
                NetControl.ReportStatus(ClientNodeStatus.Offline, "Shutting Down", "");
            }


            try
            {
                pause = true;
                config.SaveToXml();
            }
            catch
            {
            }
            try
            {

                ShutdownServices();

                MainWindow = null;
            }
            catch
            {
            }

            try
            {
                TourDocument.ClearTempDirectory();
            }
            catch
            {
            }

            if (settingsDirty)
            {
                Properties.Settings.Default.Save();
            }
        }


        private void ShutdownServices()
        {
            TileCache.ShutdownQueue();
            TileCache.PurgeQueue();
            TileCache.ClearCache();
            NetControl.Abort();
            BufferPool11.DisposeBuffers();
            RenderEngine.CleanUp();
            Grids.CleanupGrids();
            GlyphCache.CleanUpAll();
            Constellations.CleanUpAll();
            ScreenBroadcast.Shutdown();

            RenderContext11.Dispose();
            RenderContext11 = null;
            AudioPlayer.Shutdown();
        }

        private void showQueue_Click(object sender, System.EventArgs e)
        {
            Queue_List queueList = new Queue_List();
            queueList.Show();
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();

        }
        public double GetNetzoom(double amount)
        {
            double net = 1;
            switch (RenderEngine.ZoomSpeed)
            {
                case ZoomSpeeds.SLOW:
                    net = .1;
                    break;
                case ZoomSpeeds.MEDIUM:
                    net = .25;
                    break;
                case ZoomSpeeds.FAST:
                    net = 1;
                    break;
                default:
                    break;
            }
            if (Control.ModifierKeys == Keys.Shift)
            {
                net /= 5;
            }

            return 1 + (net * amount / 4);
        }

        public double NetZoomFactor
        {
            get
            {
                double net = 1;
                switch (RenderEngine.ZoomSpeed)
                {
                    case ZoomSpeeds.SLOW:
                        net = .1;
                        break;
                    case ZoomSpeeds.MEDIUM:
                        net = .25;
                        break;
                    case ZoomSpeeds.FAST:
                        net = 1;
                        break;
                    default:
                        break;
                }
                if (Control.ModifierKeys == Keys.Shift)
                {
                    net /= 5;
                }

                return 1 + net;
            }

        }
        public void ZoomView(double amount)
        {
            if (amount == 99999)
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomFactor;
                return;
            }
            if (amount > 0)
            {
                if (RenderEngine.TargetZoom > RenderEngine.ZoomMin)
                {
                    RenderEngine.TargetZoom /= (1 + amount);

                    RenderEngine.ComputeViewParameters(CurrentImageSet);
                }
                else
                {
                    RenderEngine.TargetZoom = RenderEngine.ZoomMin;
                }
            }
            if (amount < 0)
            {
                if ((RenderEngine.TargetZoom * (1 - amount)) <= RenderEngine.ZoomMax)
                {
                    RenderEngine.TargetZoom *= (1 - amount);

                    RenderEngine.ComputeViewParameters(CurrentImageSet);
                }
                else
                {
                    RenderEngine.TargetZoom = RenderEngine.ZoomMax;
                }
            }

        }

        public void DomeLeft(double amount)
        {
            RenderEngine.viewCamera.DomeAz += (float)amount;
        }

        public void DomeRight(double amount)
        {
            RenderEngine.viewCamera.DomeAz -= (float)amount;
        }

        public void DomeUp(double amount)
        {
            RenderEngine.viewCamera.DomeAlt += (float)amount;
        }

        public void DomeDown(double amount)
        {
            RenderEngine.viewCamera.DomeAlt -= (float)amount;
        }

        public void ZoomRateIn(double amount)
        {
            NetZoomRate = -amount * 500;
        }

        public void ZoomRateOut(double amount)
        {
            NetZoomRate = amount * 500;
        }

        public void RotateRight(double amount)
        {
            RotateView(0, -amount / 100);
        }

        public void RotateLeft(double amount)
        {
            RotateView(0, amount / 100);
        }

        public void TiltUp(double amount)
        {
            RotateView(amount / 100, 0);

        }

        public void TiltDown(double amount)
        {
            RotateView(-amount / 100, 0);
        }

        public void ZoomIn(double amount)
        {
            if (RenderEngine.TargetZoom > RenderEngine.ZoomFactor)
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomFactor;
                return;
            }

            if (RenderEngine.TargetZoom > RenderEngine.ZoomMin)
            {
                RenderEngine.TargetZoom /= 1 + GetNetzoom(amount);

                if (!smoothZoom)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom;
                }
                RenderEngine.ComputeViewParameters(CurrentImageSet);
            }
            else
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomMin;
            }

        }

        public void ZoomOut(double amount)
        {
            if (RenderEngine.TargetZoom < RenderEngine.ZoomFactor)
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomFactor;
                return;
            }

            if ((RenderEngine.TargetZoom * GetNetzoom(amount)) <= RenderEngine.ZoomMax)
            {
                RenderEngine.TargetZoom *= GetNetzoom(amount);
                if (!smoothZoom)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom;
                }
                RenderEngine.ComputeViewParameters(CurrentImageSet);
            }
            else
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomMax;
            }
        }

        public void ZoomIn()
        {
            if (RenderEngine.TargetZoom > RenderEngine.ZoomFactor)
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomFactor;
                return;
            }

            if (RenderEngine.TargetZoom > RenderEngine.ZoomMin)
            {
                RenderEngine.TargetZoom /= NetZoomFactor;

                if (!smoothZoom)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom;
                }
                RenderEngine.ComputeViewParameters(CurrentImageSet);
            }
            else
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomMin;
            }

        }

        public void ZoomOut()
        {
            if (RenderEngine.TargetZoom < RenderEngine.ZoomFactor)
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomFactor;
                return;
            }

            if ((RenderEngine.TargetZoom * NetZoomFactor) <= RenderEngine.ZoomMax)
            {
                RenderEngine.TargetZoom *= NetZoomFactor;
                if (!smoothZoom)
                {
                    RenderEngine.ZoomFactor = RenderEngine.TargetZoom;
                }
                RenderEngine.ComputeViewParameters(CurrentImageSet);
            }
            else
            {
                RenderEngine.TargetZoom = RenderEngine.ZoomMax;
            }
        }
       


        private void zoomTimer_Tick(object sender, EventArgs e)
        {

        }
        
        double lastMoveCompleteLat = 0;
        double lastMoveCompleteLng = 0;
        private void SendMoveComplete()
        {
            if (RenderEngine.ViewLat != lastMoveCompleteLat || RenderEngine.ViewLong != lastMoveCompleteLng)
            {
                lastMoveCompleteLat = RenderEngine.ViewLat;
                lastMoveCompleteLng = RenderEngine.ViewLong;
                if (RenderEngine.Space)
                {

                    UpdateSampClients();
                }
            }
        }

        private UpdateSampClientsDelegate invokeUpdateSampClients;

        private delegate void UpdateSampClientsDelegate();

        private void UpdateSampClients()
        {
            if (invokeUpdateSampClients == null)
            {
                invokeUpdateSampClients = new UpdateSampClientsDelegate(this.UpdateSampClientsCaller);
            }

            invokeUpdateSampClients.BeginInvoke(null, null);
        }

        private void UpdateSampClientsCaller()
        {
            sampConnection.GotoPoint(RenderEngine.RA, RenderEngine.Dec);
        }
        int sendMoveCount = 0;

        private void SendMove()
        {
            int fgHash = 0;
            int bgHash = 0;

            sendMoveCount++;

            if (RenderEngine.studyImageset != null)
            {
                fgHash = RenderEngine.studyImageset.GetHash();
            }
            if (CurrentImageSet != null)
            {
                bgHash = CurrentImageSet.GetHash();
            }

            // Moving to Binary Sync
            NetControl.SendMoveBinary(RenderEngine.ViewLat, RenderEngine.ViewLong, RenderEngine.ZoomFactor, RenderEngine.CameraRotate, RenderEngine.CameraAngle, fgHash, bgHash, RenderEngine.StudyOpacity, autoUpdate, autoFlush, Settings.Active.LocalHorizonMode, (int)RenderEngine.SolarSystemTrack, RenderEngine.viewCamera.ViewTarget, Settings.Active.SolarSystemScale, RenderEngine.targetHeight, RenderEngine.TrackingFrame, Properties.Settings.Default.ReticleAlt, Properties.Settings.Default.ReticleAz);
            autoFlush = false;
        }

        private void startQueue_Click(object sender, System.EventArgs e)
        {
            TileCache.StartQueue();
        }

        private void stopQueue_Click(object sender, System.EventArgs e)
        {
            TileCache.ShutdownQueue();
        }
        

        private void timer2_Tick(object sender, EventArgs e)
        {


            //if (spaceNavigatorDevice == null)
            //{
            //    GetData();
            //}
            // Make sure we render when dialogs are up
            long ticks = HiResTimer.TickCount - Utils.lastRender;

            int ms = (int)((ticks * 1000) / HiResTimer.Frequency);

            if (ms > 350 && !pause && RenderEngine.Initialized && !SpaceTimeController.FrameDumping)
            {
                RenderEngine.Render();
            }
        }


        private void menuMasterControler_Click(object sender, EventArgs e)
        {
            Settings.MasterController = !Settings.MasterController;
            toolStripMenuItem2.Checked = Settings.MasterController;
        }


  
        public Constellations ConstellationCheck
        {
            get { return RenderEngine.constellationCheck; }
            set { RenderEngine.constellationCheck = value; }
        }


        bool autoUpdate = false;
        bool autoFlush = false;
        private void runUpdate()
        {
            helpAutoUpdate_Click(null, null);
        }

        private void helpAutoUpdate_Click(object sender, EventArgs e)
        {
            if (!Settings.MasterController)
            {
                if (!CheckForUpdates(true))
                {
                    this.Close();
                }
            }
            else
            {
                autoUpdate = true;
                SendMove();
            }
        }

        private static bool CheckForUpdates(bool interactive)
        {
            bool versionChecked = true;
            try
            {

                //TODO move this to the real temp
                if (!Directory.Exists(Path.GetTempPath()))
                {
                    Directory.CreateDirectory(Path.GetTempPath());
                }
                WebClient Client = new WebClient();

                string yourVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                string url = String.Format("http://www.worldwidetelescope.org/wwtweb/login.aspx?user={0}&Version={1}&Equinox=true", Properties.Settings.Default.UserRatingGUID.ToString("D"), yourVersion);
                string data = Client.DownloadString(url);

                string[] lines = data.Split(new char[] { '\n' });



                string version = lines[0].Substring(lines[0].IndexOf(':') + 1).Trim();
                string dataVersion = lines[1].Substring(lines[1].IndexOf(':') + 1).Trim();
                string message = lines[2].Substring(lines[2].IndexOf(':') + 1).Trim();
                string updateUrl = "http://www.worldwidetelescope.org/wwtweb/setup.aspx";
                string warnVersion = version;
                if (lines.GetLength(0) > 3)
                {
                    warnVersion = lines[3].Substring(lines[3].IndexOf(':') + 1).Trim();
                }
                string minVersion = version;
                if (lines.GetLength(0) > 4)
                {
                    minVersion = lines[4].Substring(lines[4].IndexOf(':') + 1).Trim();
                }
                if (lines.GetLength(0) > 5)
                {
                    updateUrl = lines[5].Substring(lines[5].IndexOf(':') + 1).Trim();
                }

                if (!lines[0].StartsWith("ClientVersion"))
                {
                    throw new System.Exception();
                }
                if (!lines[1].StartsWith("DataVersion"))
                {
                    throw new System.Exception();
                }
                string myDataDir = Properties.Settings.Default.CahceDirectory + "\\data";
                if (!Directory.Exists(myDataDir))
                {
                    Directory.CreateDirectory(myDataDir);
                }

                if (!String.IsNullOrEmpty(message))
                {
                    UiTools.ShowMessageBox(message, Language.GetLocalizedText(94, "WorldWide Telescope Notification"));
                }


                if (IsNewerVersion(minVersion, yourVersion))
                {
                    if (UiTools.ShowMessageBox(string.Format(Language.GetLocalizedText(95, "You must Update your client to connect to WorldWide Telescope.\n(Your version: {0}, Update version: {1})"), yourVersion, version), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        System.OperatingSystem osInfo = System.Environment.OSVersion;
                        if (osInfo.Version.Major < 6)
                        {
                            WebWindow.OpenUrl(updateUrl, true);
                            return false;
                        }
                        versionChecked = true;
                        pause = true;
                        if (!FileDownload.DownloadFile(updateUrl, string.Format(@"{0}\wwtsetup.msi", Path.GetTempPath()), true))
                        {
                            return false;
                        }
                        pause = false;

                        if (RenderEngine.multiMonClient)
                        {
                            System.Diagnostics.Process.Start(@"msiexec.exe", string.Format(@"/i {0}\wwtsetup.msi /q", Path.GetTempPath()));
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(@"msiexec.exe", string.Format(@"/i {0}\wwtsetup.msi", Path.GetTempPath()));
                        }




                        return false;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (IsNewerVersion(warnVersion, yourVersion) || (IsNewerVersion(version, yourVersion) && interactive))
                {
                    if (interactive || ShouldAutoUpdate())
                    {
                        if (UiTools.ShowMessageBox(string.Format(Language.GetLocalizedText(96, "There is a new software update available.\n(Your version: {0}, Update version: {1})"), yourVersion, version), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.OKCancel) == DialogResult.OK)
                        {
                            System.OperatingSystem osInfo = System.Environment.OSVersion;
                            if (osInfo.Version.Major < 6)
                            {
                                WebWindow.OpenUrl(updateUrl, true);
                                return false;
                            }
                            versionChecked = true;
                            pause = true;
                            if (!FileDownload.DownloadFile(updateUrl, string.Format(@"{0}\wwtsetup.msi", Path.GetTempPath()), true))
                            {
                                return true;
                            }
                            pause = false;

                            if (RenderEngine.multiMonClient)
                            {
                                System.Diagnostics.Process.Start(@"msiexec.exe", string.Format(@"/i {0}\wwtsetup.msi /q", Path.GetTempPath()));
                            }
                            else
                            {
                                System.Diagnostics.Process.Start(@"msiexec.exe", string.Format(@"/i {0}\wwtsetup.msi", Path.GetTempPath()));
                            }

                            return false;
                        }
                    }
                }
                else
                {
                    if (interactive)
                    {
                        UiTools.ShowMessageBox(string.Format(Language.GetLocalizedText(97, "You have the latest version.\n(Your version: {0}, Server version: {1})"), yourVersion, version), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.OK);
                    }
                    versionChecked = true;

                }
                DataSetManager.DataFresh = true;

                string myDataVersionFilename = Properties.Settings.Default.CahceDirectory + "\\data\\dataversion.txt";
                if (File.Exists(myDataVersionFilename))
                {
                    string yourDataVersion = File.ReadAllText(myDataVersionFilename);
                    if (yourDataVersion != dataVersion)
                    {
                        DataSetManager.DataFresh = false;
                    }
                }
                else
                {
                    DataSetManager.DataFresh = false;
                }
                File.WriteAllText(myDataVersionFilename, dataVersion);
                return true;

            }
            catch
            {
                versionChecked = false;
                return true;
            }
            finally
            {
                if (!versionChecked)
                {
                    DataSetManager.DataFresh = true;
                }
            }
        }

        private static bool IsNewerVersion(string newVersion, string oldVersion)
        {
            string[] partsOld = oldVersion.Split(new char[] { '.' });
            string[] partsNew = newVersion.Split(new char[] { '.' });

            int oldNum = Convert.ToInt32(partsOld[0]) * 10000000 + Convert.ToInt32(partsOld[1]) * 10000 + Convert.ToInt32(partsOld[2]) * 10 + Convert.ToInt32(partsOld[3]);
            int newNum = Convert.ToInt32(partsNew[0]) * 10000000 + Convert.ToInt32(partsNew[1]) * 10000 + Convert.ToInt32(partsNew[2]) * 10 + Convert.ToInt32(partsNew[3]);

            return newNum > oldNum;
        }



        void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            contextPanel.QueueProgress = e.ProgressPercentage;

        }

        private void helpFlush_Click(object sender, EventArgs e)
        {
            TileCache.ClearCache();
        }


        //TOdo use Coordinate versions of these instead?
        public static double[] J2000toGalactic(double J2000RA, double J2000DEC)
        {
            double[] J2000pos = new double[] { Math.Cos(J2000RA / 180.0 * Math.PI) * Math.Cos(J2000DEC / 180.0 * Math.PI), Math.Sin(J2000RA / 180.0 * Math.PI) * Math.Cos(J2000DEC / 180.0 * Math.PI), Math.Sin(J2000DEC / 180.0 * Math.PI) };

            double[][] RotationMatrix = new double[3][];
            RotationMatrix[0] = new double[] { -.0548755604, -.8734370902, -.4838350155 };
            RotationMatrix[1] = new double[] { .4941094279, -.4448296300, .7469822445 };
            RotationMatrix[2] = new double[] { -.8676661490, -.1980763734, .4559837762 };



            double[] Galacticpos = new double[3];
            for (int i = 0; i < 3; i++)
            {
                Galacticpos[i] = J2000pos[0] * RotationMatrix[i][0] + J2000pos[1] * RotationMatrix[i][1] + J2000pos[2] * RotationMatrix[i][2];
            }

            double GalacticL2 = Math.Atan2(Galacticpos[1], Galacticpos[0]);
            if (GalacticL2 < 0)
            {
                GalacticL2 = GalacticL2 + 2 * Math.PI;
            }
            if (GalacticL2 > 2 * Math.PI)
            {
                GalacticL2 = GalacticL2 - 2 * Math.PI;
            }

            double GalacticB2 = Math.Atan2(Galacticpos[2], Math.Sqrt(Galacticpos[0] * Galacticpos[0] + Galacticpos[1] * Galacticpos[1]));

            return new double[] { GalacticL2 / Math.PI * 180.0, GalacticB2 / Math.PI * 180.0 };
        }



       //TOdo use Coordinate versions of these instead?
        public static double[] GalactictoJ2000(double GalacticL2, double GalacticB2)
        {
            double[] Galacticpos = new double[] { Math.Cos(GalacticL2 / 180.0 * Math.PI) * Math.Cos(GalacticB2 / 180.0 * Math.PI), Math.Sin(GalacticL2 / 180.0 * Math.PI) * Math.Cos(GalacticB2 / 180.0 * Math.PI), Math.Sin(GalacticB2 / 180.0 * Math.PI) };
            double[][] RotationMatrix = new double[3][];
            RotationMatrix[0] = new double[] { -.0548755604, -.8734370902, -.4838350155 };
            RotationMatrix[1] = new double[] { .4941094279, -.4448296300, .7469822445 };
            RotationMatrix[2] = new double[] { -.8676661490, -.1980763734, .4559837762 };

            double[] J2000pos = new double[3];
            for (int i = 0; i < 3; i++)
            {
                J2000pos[i] = Galacticpos[0] * RotationMatrix[0][i] + Galacticpos[1] * RotationMatrix[1][i] + Galacticpos[2] * RotationMatrix[2][i];
            }

            double J2000RA = Math.Atan2(J2000pos[1], J2000pos[0]);
            if (J2000RA < 0)
            {
                J2000RA = J2000RA + 2 * Math.PI;
            }
            if (J2000RA > 2 * Math.PI)
            {
                J2000RA = J2000RA - 2 * Math.PI;
            }

            double J2000DEC = Math.Atan2(J2000pos[2], Math.Sqrt(J2000pos[0] * J2000pos[0] + J2000pos[1] * J2000pos[1]));

            return new double[] { J2000RA / Math.PI * 180.0, J2000DEC / Math.PI * 180.0 };

        }


        private void TelescopeConnect_Click(object sender, EventArgs e)
        {


        }



        private void lookupOnWikipediaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String name = contextMenuTargetObject.Name.Replace("NGC ", "NGC");

            if (name.Length > 1 && name[0] == 'M' && Char.IsNumber(name[1]))
            {
                name = name.Replace("M", "Messier ");
            }

            WebWindow.OpenUrl(String.Format("http://en.wikipedia.org/wiki/{0}", name), false);

        }

        private void copyShortcutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string link = string.Format("http://www.worldwidetelescope.org/wwtweb/goto.aspx?object={0}&ra={1}&dec={2}&zoom={3}", contextMenuTargetObject.Name, contextMenuTargetObject.RA.ToString(), contextMenuTargetObject.Dec, RenderEngine.ZoomFactor);
            Clipboard.SetText(link);
        }

        private void copyShortcutMenuItem_Click(object sender, EventArgs e)
        {

            string constellation = RenderEngine.constellationCheck.FindConstellationForPoint(RenderEngine.RA, RenderEngine.Dec);
            contextPanel.Constellation = Constellations.FullName(constellation);
            contextMenuTargetObject = new TourPlace("ViewShortcut", RenderEngine.Dec, RenderEngine.RA, Classification.Unidentified, constellation, ImageSetType.Sky, -1);
            string link = string.Format("http://www.worldwidetelescope.org/wwtweb/goto.aspx?object={0}&ra={1}&dec={2}&zoom={3}", contextMenuTargetObject.Name, contextMenuTargetObject.RA.ToString(), contextMenuTargetObject.Dec, RenderEngine.ZoomFactor);
            Clipboard.SetText(link);
        }
        private void lookupOnAladinToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void lookupOnSEDSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebWindow.OpenUrl(String.Format("http://seds.org/~spider/ngc/ngc.cgi?{0}", contextMenuTargetObject.Name), false);
        }

        private void lookupOnSimbadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextMenuTargetObject.Classification == Classification.Unidentified)
            {
                WebWindow.OpenUrl(String.Format("http://simbad.u-strasbg.fr/simbad/sim-coo?CooEpoch=2000&Coord={0}h{1}d&submit=submit%20query&Radius.unit=arcmin&CooEqui=2000&CooFrame=FK5&Radius=10", contextMenuTargetObject.RA, contextMenuTargetObject.Dec > 0 ? "%2b" + contextMenuTargetObject.Dec.ToString() : contextMenuTargetObject.Dec.ToString()), false);
            }
            else
            {
                WebWindow.OpenUrl(String.Format("http://simbad.u-strasbg.fr/sim-id.pl?Ident={0}", contextMenuTargetObject.Name), false);
            }

        }

        private void AscomPlatformMenuItem_Click(object sender, EventArgs e)
        {
            WebWindow.OpenUrl("http://www.ascom-standards.org", true);
        }

        private void nameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void getDSSImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebWindow.OpenUrl(String.Format("http://archive.stsci.edu/cgi-bin/dss_search?v=poss2ukstu_red&r={0}&d={1}&e=J2000&h=15.0&w=15.0&f=gif&c=none&fov=NONE&v3=", Coordinates.FormatDMS(contextMenuTargetObject.RA), Coordinates.FormatDMS(contextMenuTargetObject.Dec)), false);
        }

        private void publicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebWindow.OpenUrl(String.Format("http://adsabs.harvard.edu/cgi-bin/abs_connect?db_key=AST&sim_query=YES&object={0}", contextMenuTargetObject.Name), false);

        }


        private void getDSSFITSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = String.Format("http://archive.stsci.edu/cgi-bin/dss_search?v=poss2ukstu_red&r={0}&d={1}&e=J2000&h=15.0&w=15.0&f=fits&c=none&fov=NONE&v3=", Coordinates.FormatDMS(contextMenuTargetObject.RA), Coordinates.FormatDMS(contextMenuTargetObject.Dec));

            string filename = Path.GetTempFileName();

            if (!FileDownload.DownloadFile(url, filename, true))
            {
                return;
            }
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = Language.GetLocalizedText(1055, "Fits Image(*.FIT)|*.FIT");
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = ".FIT";
            saveDialog.FileName = contextMenuTargetObject.Name;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Move(filename, saveDialog.FileName);
                }
                catch
                {
                    UiTools.ShowMessageBox(Language.GetLocalizedText(99, "There was a problem saving the downloaded FITS file. Please make sure you specified a path where you have permisions to save and that has free space."), Language.GetLocalizedText(100, "Download FITS Image File"));
                }
            }
        }

        private void Earth3d_Paint(object sender, PaintEventArgs e)
        {

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {

        }


 

        public IImageSet StudyImageset
        {
            get { return RenderEngine.studyImageset; }
            set
            {
                RenderEngine.studyImageset = value;           
            }
        }

        private void StudySetChanged()
        {
            if (contextPanel != null)
            {
                if ((RenderEngine.studyImageset != null) != contextPanel.studyOpacity.Visible)
                {
                    contextPanel.studyOpacity.Visible = (RenderEngine.studyImageset != null);

                    if (imageStackVisible)
                    {
                        stack.UpdateList();
                    }
                }

                bool showHist = false;
                if (contextPanel.studyOpacity.Visible && RenderEngine.studyImageset != null && RenderEngine.studyImageset.WcsImage is FitsImage)
                {
                    showHist = true;
                }

                if (contextPanel.scaleButton.Visible != showHist)
                {
                    contextPanel.scaleButton.Visible = contextPanel.scaleLabel.Visible = showHist;
                }
            }
        }

        private void Earth3d_Move(object sender, EventArgs e)
        {
            if (RenderEngine.ReadyToRender)
            {
                SetAppMode(currentMode);
            }
        }

        private void Earth3d_ResizeBegin(object sender, EventArgs e)
        {

        }

        private void Earth3d_ResizeEnd(object sender, EventArgs e)
        {
            pause = false;
        }

        private void Hover()
        {
            if (pause)
            {
                return;
            }

            if (!RenderEngine.ProjectorServer)
            {

                if (CurrentImageSet.ReferenceFrame == null)
                {
                    switch (CurrentImageSet.DataSetType)
                    {
                        case ImageSetType.Earth:
                            CurrentImageSet.ReferenceFrame = "Earth";
                            break;
                        case ImageSetType.Planet:
                            break;
                        case ImageSetType.Sky:
                            CurrentImageSet.ReferenceFrame = "Sky";
                            break;
                        case ImageSetType.Panorama:
                            CurrentImageSet.ReferenceFrame = "Panorama";
                            break;
                        case ImageSetType.SolarSystem:
                            CurrentImageSet.ReferenceFrame = RenderEngine.SolarSystemTrack.ToString();
                            break;
                        default:
                            break;
                    }

                }
                Point cursor = renderWindow.PointToClient(Cursor.Position);
                if (uiController != null)
                {
                    if (uiController.Hover(cursor))
                    {
                        return;
                    }
                }
                Coordinates result = GetCoordinatesForScreenPoint(cursor.X, cursor.Y);
                // todo unify this with findclosest
                LayerManager.HoverCheckScreenSpace(cursor, CurrentImageSet.ReferenceFrame);



                if (RenderEngine.Space)
                {
                    if (CurrentImageSet.DataSetType == ImageSetType.Sky)
                    {

                        if (RenderEngine.constellationCheck != null)
                        {
                            IPlace closetPlace = LayerManager.FindClosest(result, (float)(RenderEngine.ZoomFactor / 18000.00), true, "Sky");

                            if (closetPlace == null)
                            {
                                string constellation = RenderEngine.constellationCheck.FindConstellationForPoint(result.RA, result.Dec);
                                closetPlace = ContextSearch.FindClosestMatch(constellation, result.RA, result.Dec, RenderEngine.ZoomFactor / 900);
                            }

                            if (RenderEngine.ShowKmlMarkers && RenderEngine.KmlMarkers != null)
                            {
                                closetPlace = RenderEngine.KmlMarkers.HoverCheck(Coordinates.RADecTo3dDouble(result, 1.0).Vector311, closetPlace, (float)(RenderEngine.ZoomFactor / 900.0));
                            }

                            if (closetPlace != null)
                            {
                                Earth3d.MainWindow.SetLabelText(closetPlace, true);
                            }
                            else
                            {
                                Earth3d.MainWindow.SetLabelText(null, false);
                            }
                        }
                    }
                }
                else
                {


                    // todo unify this with hover check..
                    IPlace closetPlace = LayerManager.FindClosest(result, (float)(RenderEngine.ZoomFactor / 900.00), false, CurrentImageSet.ReferenceFrame);
                    if (closetPlace != null)
                    {
                        Earth3d.MainWindow.SetLabelText(closetPlace, true);
                    }
                    else
                    {
                        Earth3d.MainWindow.SetLabelText(null, false);
                    }
                }
            }
        }

        private void Earth3d_MouseLeave(object sender, EventArgs e)
        {

        }

        private void Earth3d_MouseEnter(object sender, EventArgs e)
        {

        }
        bool CursorVisible = true;

        bool showTourCompleteDialog = false;

        public bool ShowTourCompleteDialog
        {
            get { return showTourCompleteDialog; }
            set { showTourCompleteDialog = value; }
        }

        private void HoverTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = DateTime.Now - lastMouseMove;

            if (mouseMoved && ts.TotalMilliseconds > 500)
            {
                if ((TourPlayer.Playing || (fullScreen && kinectHeard)) && (Properties.Settings.Default.FullScreenTours == true && !Settings.MasterController))
                {
                    if (CursorVisible)
                    {
                        Cursor.Hide();
                        CursorVisible = false;
                    }
                }
                else
                {
                    if (!CursorVisible)
                    {
                        Cursor.Show();
                        CursorVisible = true;
                    }
                }

                Hover();
                mouseMoved = false;
            }



        }

        private void getSDSSImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = String.Format("http://casjobs.sdss.org/ImgCutoutDR7/getjpeg.aspx?ra={0}&dec={1}&scale=0.79224&width=800&height=800&opt=&query=", Coordinates.FormatDMS(contextMenuTargetObject.RA), Coordinates.FormatDMS(contextMenuTargetObject.Dec));
            WebWindow.OpenUrl(url, false);


        }

        public Bitmap GetScreenThumbnail()
        {

            if (Properties.Settings.Default.ShowSafeArea || Properties.Settings.Default.ShowTouchControls)
            {
                // Draw without safe areas
                TourEditor.Capturing = true;
                RenderEngine.Render();
                System.Threading.Thread.Sleep(100);
                RenderEngine.Render();
                TourEditor.Capturing = false;
            }

            try
            {
                Bitmap imgOrig = RenderContext11.GetScreenBitmap();

                Bitmap bmpThumb = new Bitmap(96, 45);

                Graphics g = Graphics.FromImage(bmpThumb);

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                double imageAspect = ((double)imgOrig.Width) / (imgOrig.Height);

                double clientAspect = ((double)bmpThumb.Width) / bmpThumb.Height;

                int cw = bmpThumb.Width;
                int ch = bmpThumb.Height;

                if (imageAspect < clientAspect)
                {
                    ch = (int)((double)cw / imageAspect);
                }
                else
                {
                    cw = (int)((double)ch * imageAspect);
                }

                int cx = (bmpThumb.Width - cw) / 2;
                int cy = ((bmpThumb.Height - ch) / 2);// - 1;
                Rectangle destRect = new Rectangle(cx, cy, cw, ch);

                Rectangle srcRect = new Rectangle(0, 0, imgOrig.Width, imgOrig.Height);
                g.DrawImage(imgOrig, destRect, srcRect, System.Drawing.GraphicsUnit.Pixel);
                g.Dispose();
                GC.SuppressFinalize(g);
                imgOrig.Dispose();
                GC.SuppressFinalize(imgOrig);

                return bmpThumb;
            }
            catch
            {
                Bitmap bmp = new Bitmap(96, 45);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.Blue);

                g.DrawString("Can't Capture", UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(3, 15));
                return bmp;
            }
        }

       


        private void newSlideBasedTour(object sender, EventArgs e)
        {
            if (!CloseOpenToursOrAbort(false))
            {
                return;
            }
            TourDocument tour = new TourDocument();
            TourProperties tourProps = new TourProperties();
            tourProps.EditTour = tour;
            if (tourProps.ShowDialog() == DialogResult.OK)
            {
                tour.EditMode = true;
                this.menuTabs.AddTour(tour);
                this.menuTabs.FocusTour(tour);
                Undo.Clear();
            }

        }

        private void newObservingListpMenuItem_Click(object sender, EventArgs e)
        {
            explorePane.NewCollection();
        }

        private void newTimelineTour_Click(object sender, EventArgs e)
        {


        }

        private void newInteractiveTour_Click(object sender, EventArgs e)
        {


        }
        private bool CloseOpenToursOrAbort(bool silent)
        {
            if (menuTabs.CurrentTour != null)
            {
                if (tourEdit != null)
                {
                    KeyFramer.HideTimeline();
                    if (tourEdit.Tour.EditMode && tourEdit.Tour.TourDirty)
                    {
                        DialogResult result = MessageBox.Show(Language.GetLocalizedText(5, "Your tour has unsaved changes. Do you want to save the changes before closing?"), Language.GetLocalizedText(6, "Tour Editor"), MessageBoxButtons.YesNoCancel);
                        if (result == DialogResult.Yes)
                        {
                            if (!tourEdit.Save(false))
                            {
                                return false;
                            }
                        }
                        if (result == DialogResult.Cancel)
                        {
                            return false;
                        }
                    }
                    CloseTour(silent);
                    return true;
                }
            }
            return true;


        }

        private void openTourMenuItem_Click(object sender, EventArgs e)
        {
            if (!CloseOpenToursOrAbort(false))
            {
                return;
            }

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Language.GetLocalizedText(101, "WorldWide Telescope Tours") + "|*.wtt";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filename = openFile.FileName;
                try
                {
                    LoadTourFromFile(filename, true, "");
                }
                catch
                {
                    MessageBox.Show(Language.GetLocalizedText(102, "This file does not seem to be a valid tour"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                }
            }
        }

        public TourDocument LoadTourFromFile(string filename, bool editMode, string tagId)
        {

            if (!CloseOpenToursOrAbort(false))
            {
                return null;
            }

            if (!File.Exists(filename))
            {
                MessageBox.Show(Language.GetLocalizedText(103, "The tour file could not be downloaded and was not cached. Check you network connection."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                return null;
            }

            FileInfo fi = new FileInfo(filename);
            if (fi.Length == 0)
            {
                File.Delete(filename);
                MessageBox.Show(Language.GetLocalizedText(104, "The tour file could not be downloaded and was not cached. Check you network connection."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                return null;
            }
            if (fi.Length < 100)
            {
                MessageBox.Show(Language.GetLocalizedText(105, "The tour file is invalid. Check you network connection."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                File.Delete(filename);
                return null;
            }

            if (Settings.MasterController && Properties.Settings.Default.AutoSyncTours)
            {
                editMode = true;
            }


            Undo.Clear();
            LayerManager.TourLayers = !editMode;
            TourDocument tour = TourDocument.FromFile(filename, editMode);



            if (tour != null)
            {
                tour.TagId = tagId;
                tour.EditMode = editMode;
                this.menuTabs.AddTour(tour);
                this.menuTabs.FocusTour(tour);

                if (NoUi)
                {
                    if (tourEdit == null)
                    {
                        tourEdit = new TourEditTab();
                        tourEdit.Owner = this;
                    }
                    tourEdit.Tour = tour;


                    Properties.Settings.Default.AutoRepeatTour = true;
                    tourEdit.PlayNow(true);

                }
                if (Settings.MasterController && Properties.Settings.Default.AutoSyncTours)
                {
                    SyncTour();
                }
            }
            else
            {
                MessageBox.Show(Language.GetLocalizedText(106, "Could not load tour"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
            }
            return tour;
        }

        private void openObservingListMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Language.GetLocalizedText(107, "WorldWide Telescope Collection") + "|*.wtml";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filename = openFile.FileName;

                try
                {
                    LoadFolder(filename);
                }
                catch
                {
                    MessageBox.Show(Language.GetLocalizedText(109, "This file does not seem to be a valid collection"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                }
            }

        }

        private void LoadFolder(string filename)
        {
            Folder newFolder = Folder.LoadFromFile(filename, false);

            if (newFolder.Group == FolderGroup.Goto)
            {
                if (newFolder.Children[0] is Place)
                {
                    RenderEngine.GotoTarget((IPlace)newFolder.Children[0], false, false, true);
                }
            }
            else if (newFolder.Group == FolderGroup.Community)
            {
                AddCommunity(newFolder);
            }
            else if (newFolder.Group == FolderGroup.ImageStack)
            {
                ImageStackVisible = true;
                LoadImageStack(newFolder, true);
                if (newFolder.Browseable == FolderBrowseable.True && !RenderEngine.ProjectorServer)
                {
                    if (!explorePane.IsCollectionLoaded(filename, true))
                    {
                        explorePane.LoadCollection(newFolder);
                    }
                    PlayCollection();
                    ShowFullScreen(true);
                }

            }
            else
            {
                if (!explorePane.IsCollectionLoaded(filename, true))
                {
                    explorePane.LoadCollection(newFolder);
                }
            }
        }
        bool firstImageLoaded = true;
        private void LoadImageStack(Folder newFolder, bool showFirstAsBackground)
        {
            firstImageLoaded = showFirstAsBackground;
            AddClidrenToStack(newFolder, showFirstAsBackground);

            Stack.UpdateList();
        }

        IPlace lastAddedToStack = null;
        public void AddClidrenToStack(Folder folder, bool showFirstAsBackground)
        {

            foreach (object o in folder.Children)
            {
                if (o is Folder)
                {
                    AddClidrenToStack((Folder)o, false);
                }
                else
                {
                    if (o is Place)
                    {
                        if (showFirstAsBackground && firstImageLoaded)
                        {
                            SetCurrentBackgroundForStack((Place)o);
                        }
                        else
                        {
                            AddPlaceToStack((Place)o, false);
                        }
                    }
                    else if (o is IImageSet)
                    {
                        IImageSet imageSet = (IImageSet)o;
                        TourPlace tp = new TourPlace(imageSet.Name, imageSet.CenterX, imageSet.CenterY, Classification.Unidentified, "", imageSet.DataSetType, 360);
                        if (showFirstAsBackground && firstImageLoaded)
                        {
                            SetCurrentBackgroundForStack(tp);
                        }
                        else
                        {
                            AddPlaceToStack(tp, false);
                        }
                    }
                }
            }
        }

        void SetCurrentBackgroundForStack(IPlace place)
        {
            IImageSet imageSet = null;
            if (place.StudyImageset != null)
            {
                imageSet = place.StudyImageset;
            }
            if (place.BackgroundImageSet != null)
            {
                imageSet = place.BackgroundImageSet;
            }

            CurrentImageSet = imageSet;
            firstImageLoaded = false;
        }


        private void AddCommunity(Folder newFolder)
        {
            string filename = CommuinitiesDirectory + Math.Abs(newFolder.Url.GetHashCode32()).ToString() + ".wtml";
            if (!Directory.Exists(CommuinitiesDirectory))
            {
                Directory.CreateDirectory(CommuinitiesDirectory);
            }

            if (File.Exists(filename))
            {
                UiTools.ShowMessageBox(Language.GetLocalizedText(110, "The file opened is a community registration file and this community is already registered."));
                communitiesPane.LoadCommunities();
                menuTabs.SetSelectedIndex((int)ApplicationMode.Explore, false);
                this.Refresh();
                menuTabs.SetSelectedIndex((int)ApplicationMode.Community, false);
                return;
            }

            if (UiTools.ShowMessageBox(Language.GetLocalizedText(111, "The file opened is a community registration file. Would you like to add this to your communities list?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            newFolder.SaveToFile(filename);
            communitiesPane.LoadCommunities();
            menuTabs.SetSelectedIndex((int)ApplicationMode.Explore, false);
            this.Refresh();
            menuTabs.SetSelectedIndex((int)ApplicationMode.Community, false);
        }


        public static string CommuinitiesDirectory
        {
            get { return Properties.Settings.Default.CahceDirectory + "Communities\\"; }
        }

        private void homepageMenuItem_Click(object sender, EventArgs e)
        {
            WebWindow.OpenUrl("http://www.worldwidetelescope.org/", true);
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog(this);
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        internal KmlCollection MyPlaces = new KmlCollection();
        private void openKMLMenuItem_Click(object sender, EventArgs e)
        {

        }

        private bool OpenKmlFile(string filename)
        {
            try
            {
                ShowLayersWindows = true;
                LayerManager.LoadLayer(filename, "Earth", true, false);
                return true;
            }
            catch
            {
                return false;
            }


        }
        private void openImageMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Language.GetLocalizedText(979, "Images(*.JPG;*.PNG;*.TIF;*.TIFF;*.FITS;*.FIT)|*.JPG;*.PNG;*.TIF;*.TIFF;*.FITS;*.FIT");
            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(openFile.FileName))
                {
                    string filename = openFile.FileName;

                    LoadImage(filename);
                }

            }
        }

        private void LoadImage(string filename)
        {
            WcsImage wcsImage = WcsImage.FromFile(filename);

            bool hasAvm = wcsImage.ValidWcs;
            {
                Bitmap bmp = wcsImage.GetBitmap();
                wcsImage.AdjustScale(bmp.Width, bmp.Height);

                ImageSetHelper imageSet = null;
                TourPlace place = null;
                if (hasAvm)
                {
                    imageSet = new ImageSetHelper(
                        wcsImage.Description,
                        filename,
                        ImageSetType.Sky,
                        BandPass.Visible,
                        ProjectionType.SkyImage,
                        Math.Abs(filename.GetHashCode32()),
                        0,
                        0,
                        256,
                        wcsImage.ScaleY,
                        ".tif",
                        wcsImage.ScaleX > 0,
                        "",
                        wcsImage.CenterX,
                        wcsImage.CenterY,
                        wcsImage.Rotation,
                        false,
                        "",
                        false,
                        false,
                        1,
                        wcsImage.ReferenceX,
                        wcsImage.ReferenceY,
                        wcsImage.Copyright,
                        wcsImage.CreditsUrl,
                        "",
                        "",
                        0,
                        ""
                        );
                    place = new TourPlace(UiTools.GetNamesStringFromArray(wcsImage.Keywords.ToArray()), wcsImage.CenterY, wcsImage.CenterX / 15, Classification.Unidentified, RenderEngine.constellationCheck.FindConstellationForPoint(wcsImage.CenterX, wcsImage.CenterY), ImageSetType.Sky, -1);
                }
                else
                {

                    imageSet = new ImageSetHelper(wcsImage.Description, filename, ImageSetType.Sky, BandPass.Visible, ProjectionType.SkyImage, Math.Abs(filename.GetHashCode32()), 0, 0, 256, .001, ".tif", false, "", RenderEngine.RA * 15, RenderEngine.ViewLat, 0, false, "", false, false, 1, bmp.Width / 2, bmp.Height / 2, wcsImage.Copyright, wcsImage.CreditsUrl, "", "", 0, "");
                    place = new TourPlace(UiTools.GetNamesStringFromArray(wcsImage.Keywords.ToArray()), RenderEngine.ViewLat, RenderEngine.RA, Classification.Unidentified, RenderEngine.constellationCheck.FindConstellationForPoint(wcsImage.CenterX, wcsImage.CenterY), ImageSetType.Sky, -1);
                }
                imageSet.WcsImage = wcsImage;
                place.StudyImageset = imageSet;
                place.Tag = wcsImage;
                Place pl = Place.FromIPlace(place);

                pl.ThumbNail = UiTools.MakeThumbnail(bmp);
                StudyImageset = pl.StudyImageset;
                RenderEngine.GotoTarget(pl, false, false, true);



                explorePane.OpenImages.AddChildPlace(pl);

                bmp.Dispose();
                GC.SuppressFinalize(bmp);
                bmp = null;

                explorePane.ShowOpenImages();

            }
            if (!hasAvm)
            {
                MessageBox.Show(Language.GetLocalizedText(112, "The image file did not contain recognizable WCS or AVM Metadata to position it in the sky"), Language.GetLocalizedText(113, "Load Image"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }



        private void menuTabs_Load(object sender, EventArgs e)
        {

        }

        private void tourHomeMenuItem_Click(object sender, EventArgs e)
        {
            WebWindow.OpenUrl("http://www.worldwidetelescope.org/Learn/Exploring#guidedtours", true);
        }

        private void tourSearchWebPageMenuItem_Click(object sender, EventArgs e)
        {
            WebWindow.OpenUrl("http://www.worldwidetelescope.org/Community", true);
        }

        private void favoriteMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void gettingStarteMenuItem_Click(object sender, EventArgs e)
        {
            WebWindow.OpenUrl("http://www.worldwidetelescope.org/Support/Index", true);
        }

        private void exportTourToFileMenuItem_Click(object sender, EventArgs e)
        {
            // Export
            if (tourEdit != null)
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = Language.GetLocalizedText(101, "WorldWide Telescope Tours") + "|*.wtt";
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveDialog.AddExtension = true;
                saveDialog.DefaultExt = ".WTT";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    tourEdit.Tour.SaveToFile(saveDialog.FileName);
                }
            }
        }

        private void publishTourMenuItem_Click(object sender, EventArgs e)
        {
            if (tourEdit != null)
            {
                UiTools.ShowMessageBox(Language.GetLocalizedText(114, "Respect Copyright. Please respect the rights of artists and creators. Content such as music is protected by copyright. The music made available to you in WorldWide Telescope is protected by copyright and may be used for the sole purpose of creating tours in WorldWide Telescope. You may not share other people's content unless you own the rights or have permission from the owner."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                WWTWebService webService = new WWTWebService();
                webService.Timeout = 1000000;
                string tempFile = Path.GetTempFileName();
                try
                {
                    tourEdit.Tour.SaveToFile(tempFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Language.GetLocalizedText(115, "There was a problem saving the tour.") + ex.Message, Language.GetLocalizedText(116, "Submission Failed"));
                }
                string tourXML = tourEdit.Tour.GetXmlSubmitString();
                byte[] tourBlob = UiTools.LoadBlob(tempFile);
                File.Delete(tempFile);

                byte[] tourThumbBlob = UiTools.LoadBlob(tourEdit.Tour.TourThumbnailFilename);
                byte[] authorThumbBlob = null;
                try
                {

                    authorThumbBlob = UiTools.LoadBlob(tourEdit.Tour.AuthorThumbnailFilename);
                }
                catch
                {
                    MessageBox.Show(Language.GetLocalizedText(117, "There was no author image. Please add one and submit again."), Language.GetLocalizedText(116, "Submission Failed"));
                    return;
                }
                TerraViewer.org.worldwidetelescope.www.Tour[] results = null;
                try
                {
                    results = webService.ImportTour(tourXML, tourBlob, tourThumbBlob, authorThumbBlob);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Language.GetLocalizedText(118, "There was a problem submitting the tour.") + ex.Message, Language.GetLocalizedText(116, "Submission Failed"));
                    return;
                }

                if (results != null && results.Length > 0)
                {
                    MessageBox.Show(Language.GetLocalizedText(119, "The tour was successfully submitted for publication. It must undergo technical review and approval before it will appear on the tour directory."), Language.GetLocalizedText(120, "Submit Tour for Publication"));
                }
                else
                {
                    MessageBox.Show(Language.GetLocalizedText(121, "There was a problem submitting the tour. Please review the tour content and make sure all fields are filled in correctly and that the total content size is below in maximum tour size."), Language.GetLocalizedText(120, "Submit Tour for Publication"));
                }

            }

        }

        private void slewTelescopeMenuItem_Click(object sender, EventArgs e)
        {
            if (telescopePane != null)
            {
                telescopePane.SlewScope_Click(sender, e);
            }
        }

        private void centerTelescopeMenuItem_Click(object sender, EventArgs e)
        {
            if (telescopePane != null)
            {
                telescopePane.CenterScope_Click(sender, e);
            }
        }

        private void SyncTelescopeMenuItem_Click(object sender, EventArgs e)
        {
            if (telescopePane != null)
            {
                telescopePane.SyncScope_Click(sender, e);
            }
        }

        private void chooseTelescopeMenuItem_Click(object sender, EventArgs e)
        {
            if (telescopePane != null)
            {
                telescopePane.Choose_Click(sender, e);
            }
        }

        private void connectTelescopeMenuItem_Click(object sender, EventArgs e)
        {
            if (telescopePane != null)
            {
                telescopePane.ConnectScope_Click(sender, e);
            }
        }

        private void trackScopeMenuItem_Click(object sender, EventArgs e)
        {
            if (telescopePane != null)
            {
                telescopePane.TrackScope.Checked = !telescopePane.TrackScope.Checked;
                trackScopeMenuItem.Checked = telescopePane.TrackScope.Checked;
            }
        }

        private void parkTelescopeMenuItem_Click(object sender, EventArgs e)
        {
            if (telescopePane != null)
            {
                telescopePane.Park_Click(sender, e);
            }

        }



        private void telescopeMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (telescopePane == null)
            {
                telescopePane = new TelescopeTab();
                telescopePane.Owner = this;
            }
            if (telescopePane != null)
            {
                trackScopeMenuItem.Checked = telescopePane.TrackScope.Checked;
                bool state = telescopePane.TelescopeConnected;

                this.centerTelescopeMenuItem.Enabled = state;
                this.slewTelescopeMenuItem.Enabled = state;
                this.trackScopeMenuItem.Enabled = state;
                if (state)
                {
                    this.SyncTelescopeMenuItem.Enabled = telescopePane.Scope.CanSync && telescopePane.Scope.Tracking;
                    if (telescopePane.Scope.AtPark)
                    {
                        this.parkTelescopeMenuItem.Text = Language.GetLocalizedText(122, "Unpark");
                        this.parkTelescopeMenuItem.Enabled = telescopePane.Scope.CanUnpark;
                    }
                    else
                    {
                        this.parkTelescopeMenuItem.Text = Language.GetLocalizedText(50, "Park");
                        this.parkTelescopeMenuItem.Enabled = telescopePane.Scope.CanPark;
                    }
                }
                else
                {
                    this.SyncTelescopeMenuItem.Enabled = state;
                    this.parkTelescopeMenuItem.Enabled = state;
                }

                if (state)
                {
                    connectTelescopeMenuItem.Text = Language.GetLocalizedText(123, "Disconnect");
                }
                else
                {
                    connectTelescopeMenuItem.Text = Language.GetLocalizedText(48, "Connect");
                }
            }
        }

        private void resetCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetCamera();

        }

        private void ResetCamera()
        {
            CameraParameters camParams = new CameraParameters(0, 0, 360, 0, 0, 100);
            RenderEngine.GotoTarget(camParams, false, true);
        }



        bool showPerfData = false;
        private void showPerformanceDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showPerfData = !showPerfData;
            showPerformanceDataToolStripMenuItem.Checked = showPerfData;
            Text = Language.GetLocalizedText(3, "Microsoft WorldWide Telescope");
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Settings.MasterController)
            {
                try
                {
                    if (!CheckForUpdates(true))
                    {
                        this.Close();
                    }
                }
                catch
                {
                    MessageBox.Show(Language.GetLocalizedText(124, "Could not connect to the server to check for updates. Try again later."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                }
            }
            else
            {
                autoUpdate = true;
                SendMove();
            }
        }



        private void StatupTimer_Tick(object sender, EventArgs e)
        {
            if (RenderEngine.Initialized)
            {
                StatupTimer.Enabled = false;
                this.Activate();
                SetAppMode(ApplicationMode.Explore);
                ResetStartFlag();
                try
                {
                    if (!string.IsNullOrEmpty(launchTourFile))
                    {
                        if (Path.GetExtension(launchTourFile) == ".wwtl")
                        {
                            LayerManager.LoadLayerFile(launchTourFile, "Sun", false);
                        }
                        else if (Path.GetExtension(launchTourFile) == ".wtml")
                        {
                            LoadFolder(launchTourFile);
                        }
                        else if (Path.GetExtension(launchTourFile) == ".wwtfig")
                        {
                            SetAppMode(ApplicationMode.View);
                            if (viewPane != null)
                            {
                                settingsPane.InstallNewFigureFile(launchTourFile);
                            }
                        }
                        else if (Path.GetExtension(launchTourFile) == ".wtt")
                        {
                            LoadTourFromFile(launchTourFile, false, "");
                        }
                        else if (Path.GetExtension(launchTourFile) == ".kml" || Path.GetExtension(launchTourFile) == ".kmz")
                        {
                            OpenKmlFile(launchTourFile);
                        }
                        launchTourFile = "";
                    }
                    else
                    {
                        if (config.Master == true)
                        {
                            if (Properties.Settings.Default.ShowNavHelp)
                            {
                                ShowWelcome();
                            }

                            if (Properties.Settings.Default.ShowJoystickHelp && ControllerConnected())
                            {
                                JoystickHelp joystick = new JoystickHelp();
                                joystick.ShowDialog();
                            }
                        }
                    }
                    if (config.Master == false)
                    {
                        NetControl.ReportStatus(ClientNodeStatus.Online, "Ready", "");
                    }

                }
                catch
                {
                }
            }
        }

        private static void ShowWelcome()
        {
            Welcome welcome = new Welcome();
            welcome.ShowDialog();
        }



        private void PopupClosed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            menuTabs.Thaw();

        }

        static public bool IsInSDSSFootprint(double ra, double dec)
        {
            if (!((ra * 15) > 270 | dec < -3 | (ra * 15) < 105 | dec > 75))
            {
                return true;
            }
            return false;
        }

        private void downloadQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Queue_List queueList = new Queue_List();
            queueList.Show();
        }

        private void sIMBADSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SimbadSearch searchDialog;
                bool foundOrCanceled = false;
                bool notFound = false;
                String targetName = "";

                if (searchPane != null)
                {
                    targetName = searchPane.SearchStringText;
                }
                while (!foundOrCanceled)
                {
                    searchDialog = new SimbadSearch();
                    searchDialog.ObejctName = targetName;

                    if (notFound)
                    {
                        searchDialog.Text = Language.GetLocalizedText(125, "SIMBAD Search - Not Found");
                    }
                    if (searchDialog.ShowDialog() == DialogResult.OK)
                    {
                        targetName = searchDialog.ObejctName;
                        ObjectLookup lookup = new ObjectLookup();

                        AstroObjectResult result = null;

                        if (RenderEngine.Space)
                        {
                            result = lookup.SkyLookup(targetName);
                        }


                        if (result != null)
                        {
                            if (RenderEngine.Space)
                            {
                                RenderEngine.GotoTarget(false, false, new CameraParameters(result.Dec, RAtoViewLng(result.RA), -1, 0, 0, 1.0f), null, null);
                            }
                            else
                            {
                                RenderEngine.GotoTarget(false, false, new CameraParameters(result.Dec, result.RA, -1, 0, 0, 1.0f), null, null);
                            }
                            foundOrCanceled = true;

                        }
                        else
                        {
                            foundOrCanceled = false;
                            notFound = true;
                        }
                    }
                    else
                    {
                        foundOrCanceled = true;
                    }

                }
            }
            catch
            {
                UiTools.ShowMessageBox(Language.GetLocalizedText(126, "Could not connect to SIMBAD Name Resolution Server. Check you internet connection"));
            }
        }

        private void feedbackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebWindow.OpenUrl("http://www.worldwidetelescope.org/Support/IssuesAndBugs", true);
        }



        private void editTourToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (menuTabs.CurrentTour != null)
            {
                if (!menuTabs.CurrentTour.EditMode)
                {
                    LayerManager.MergeToursLayers();
                }

                menuTabs.CurrentTour.EditMode = true;
                if (tourEdit != null)
                {
                    tourEdit.SetEditMode(true);
                    tourEdit.PauseTour();
                }
                this.menuTabs.FocusTour(menuTabs.CurrentTour);
            }
        }
        private void hLAFootprintsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HSTFootprint footprint = new HSTFootprint();

            STCRegion region = footprint.ACS_ConeFootprintL1((contextMenuTargetObject.RA * 15), contextMenuTargetObject.Dec, RenderEngine.FovAngle);

        }


        private void uSNONVOConeSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string url = String.Format("http://nedwww.ipac.caltech.edu/cgi-bin/nph-objsearch?search_type=Near+Position+Search&of=xml_main&RA={0}&DEC={1}&SR={2}", (contextMenuTargetObject.RA * 15).ToString(), contextMenuTargetObject.Dec.ToString(), RenderEngine.FovAngle.ToString());
            WebClient client = new WebClient();

            try
            {
                string data = client.DownloadString(url);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);
                VoTable voTable = new VoTable(doc);

                VoTableLayer layer = LayerManager.AddVoTableLayer(voTable, "VO Table");
                VOTableViewer viewer = new VOTableViewer();
                viewer.Layer = layer;

                viewer.Show();
                ShowLayersWindows = true;
            }
            catch
            {
                WebWindow.OpenUrl(url, true);
            }

        }

        private void saveCurrentViewImageToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool megaCap = false;
            int width = 1920;
            int height = 1080;
            if (Control.ModifierKeys == Keys.Shift)
            {
                megaCap = true;

                bool valid = false;

                string widthString = "1920";
                string heightString = "1080";
                SimpleInput input;
                while (!valid)
                {
                    input = new SimpleInput("Image Capture Size", "Width", widthString, 4);

                    if (input.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            width = int.Parse(input.ResultText);
                            if (width > 0 || width < 9999)
                            {
                                valid = true;
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        return;
                    }
                }

                valid = false;

                while (!valid)
                {
                    input = new SimpleInput("Image Capture Size", "Height", heightString, 4);

                    if (input.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            height = int.Parse(input.ResultText);
                            if (height > 0 || height < 9999)
                            {
                                valid = true;
                            }
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = Language.GetLocalizedText(978, "Portable Network Graphics(*.png)|*.png|JPEG Image(*.jpg)|*.jpg");
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveDialog.AddExtension = true;
                saveDialog.DefaultExt = ".png";
                saveDialog.FileName = "SavedView.png";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    if (Properties.Settings.Default.ShowSafeArea || Properties.Settings.Default.ShowTouchControls)
                    {
                        // Draw without safe areas
                        TourEditor.Capturing = true;
                        RenderEngine.Render();
                        System.Threading.Thread.Sleep(100);
                        RenderEngine.Render();
                        TourEditor.Capturing = false;
                    }
                    if (megaCap)
                    {
                        RenderEngine.CaptureMegaShot(saveDialog.FileName, width, height);
                    }
                    else
                    {
                        if (saveDialog.FileName.ToLower().EndsWith(".jpg") || saveDialog.FileName.ToLower().EndsWith(".jpeg"))
                        {
                            RenderContext11.SaveBackBuffer(saveDialog.FileName, SharpDX.Direct3D11.ImageFileFormat.Jpg);
                        }
                        else
                        {
                            RenderContext11.SaveBackBuffer(saveDialog.FileName, SharpDX.Direct3D11.ImageFileFormat.Png);
                        }
                    }
                }
            }
            catch
            {
                UiTools.ShowMessageBox(Language.GetLocalizedText(1051, "There was a problem capturing the screen contents"));
            }
        }
        private void copyCurrentViewToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.ShowSafeArea || Properties.Settings.Default.ShowTouchControls)
            {
                // Draw without safe areas
                TourEditor.Capturing = true;
                RenderEngine.Render();
                System.Threading.Thread.Sleep(100);
                RenderEngine.Render();
                TourEditor.Capturing = false;
            }

            try
            {
                Bitmap bmp = RenderContext11.GetScreenBitmap();
                Clipboard.SetImage(bmp);
                bmp.Dispose();
                GC.SuppressFinalize(bmp);
            }
            catch
            {
                using (Bitmap bmp = new Bitmap(196, 45))
                {
                    Graphics g = Graphics.FromImage(bmp);
                    g.Clear(Color.Blue);

                    g.DrawString("Can't Capture Screenshot", UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(3, 15));
                    Clipboard.SetImage(bmp);
                }
            }
        }

        private void setCurrentViewAsWindowsDesktopBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ShowFullScreen(true);
                bool showCrossHairs = Properties.Settings.Default.ShowCrosshairs;
                Properties.Settings.Default.ShowCrosshairs = false;
                RenderEngine.Render();
                RenderEngine.Render();
                Properties.Settings.Default.ShowCrosshairs = showCrossHairs;
                string path = Properties.Settings.Default.CahceDirectory + "wallpaper.bmp";

                RenderContext11.SaveBackBuffer(path, SharpDX.Direct3D11.ImageFileFormat.Bmp);
                UiTools.SetWallpaper(path);
                ShowFullScreen(false);
            }
            catch
            {

            }
        }
        bool enable3dCitiesExport = false;
        private void viewMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            enableExport3dCitiesModeToolStripMenuItem.Visible = !enable3dCitiesExport;
            exportCurrentCitiesViewAs3DMeshToolStripMenuItem.Visible = enable3dCitiesExport;
            fullDomeToolStripMenuItem.Checked = Properties.Settings.Default.DomeView;
            detachMainViewToSecondMonitor.Enabled = Screen.AllScreens.Length > 1;
            detachMainViewToThirdMonitorToolStripMenuItem.Enabled = Screen.AllScreens.Length > 2;
            int id = -1;
            int index = 0;
            if (renderHost != null)
            {
                Screen screen = Screen.FromControl(renderHost);
                foreach (Screen s in Screen.AllScreens)
                {
                    if (s.DeviceName == screen.DeviceName)
                    {
                        id = index;
                    }
                    index++;
                }
            }

            detachMainViewToSecondMonitor.Checked = (id == 1 || id == 0);
            detachMainViewToThirdMonitorToolStripMenuItem.Checked = (id == 2);

            showTouchControlsToolStripMenuItem.Checked = Properties.Settings.Default.ShowTouchControls;
            monochromeStyleToolStripMenuItem.Checked = Properties.Settings.Default.MonochromeImageStyle;
            monochromeStyleToolStripMenuItem.Visible = false;
            lockVerticalSyncToolStripMenuItem.Checked = Properties.Settings.Default.FrameSync;
            allowUnconstrainedTiltToolStripMenuItem.Checked = Properties.Settings.Default.UnconstrainedTilt;

            exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem.Enabled = RenderEngine.PlanetLike;
        }

        internal void joinCoomunityMenuItem_Click(object sender, EventArgs e)
        {

            JoinCommunity();

        }

        internal void JoinCommunity()
        {
            //RG: The layerscape home page is now {rootdomain}/Community
            WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + "/Community", true);
        }

        private void showFinderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFinder();
        }

        private void ShowFinder()
        {
            ShowPropertiesForPoint(new Point(renderWindow.ClientSize.Width / 2, renderWindow.ClientSize.Height / 2));
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point pnt = Cursor.Position;
            this.Focus();
            ObjectProperties.ShowNofinder(contextMenuTargetObject, pnt);
        }

        private void restoreDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Language.GetLocalizedText(2, "This will restore user settings to their default values. Are you sure you want to proceed?"), Language.GetLocalizedText(936, "Restore Default Settings"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                resetProperties = true;
                Properties.Settings.Default.Reset();
                Properties.Settings.Default.UpgradeNeeded = false;
            }
        }

        private void exploreMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            playCollectionAsSlideShowToolStripMenuItem.Checked = playingSlideShow;
            showFinderToolStripMenuItem.Enabled = CurrentImageSet.DataSetType == ImageSetType.Sky || SolarSystemMode;
        }

        private void renderWindow_Click(object sender, EventArgs e)
        {
            if (uiController != null)
            {
                if (uiController.Click(sender, e))
                {
                    return;
                }
            }
        }

        private void renderWindow_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void renderWindow_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (activeTouch != TouchControls.None)
            {
                return;
            }

            if (uiController != null)
            {
                if (uiController.MouseDoubleClick(sender, e))
                {
                    return;
                }
            }

            Coordinates result = GetCoordinatesForScreenPoint(e.X, e.Y);
            if (RenderEngine.Space)
            {
                RenderEngine.GotoTarget(false, false, new CameraParameters(result.Dec, RAtoViewLng(result.RA), RenderEngine.viewCamera.Zoom > RenderEngine.ZoomMin ? RenderEngine.viewCamera.Zoom / 2 : RenderEngine.viewCamera.Zoom, RenderEngine.viewCamera.Rotation, RenderEngine.viewCamera.Angle, (float)RenderEngine.viewCamera.Opacity), RenderEngine.studyImageset, CurrentImageSet);
            }
            else
            {
                RenderEngine.TargetLong += (double)(e.X - (this.Width / 2)) * GetPixelScaleX(false);
                RenderEngine.TargetLat -= (double)(e.Y - (this.Height / 2)) * GetPixelScaleY();
                RenderEngine.TargetLong = ((RenderEngine.TargetLong + 180.0) % 360.0) - 180.0;
                RenderEngine.TargetLat = ((RenderEngine.TargetLat + 90.0) % 180.0) - 90.0;
                RenderEngine.deltaLat = (RenderEngine.TargetLat - RenderEngine.ViewLat) / 20;
                RenderEngine.deltaLong = (RenderEngine.TargetLong - RenderEngine.ViewLong) / 20;
                RenderEngine.ZoomSpeed = (ZoomSpeeds)Properties.Settings.Default.ZoomSpeed;
                ZoomIn();
            }
        }




        private void renderWindow_MouseDown(object sender, MouseEventArgs e)
        {
            this.Focus();
            if (uiController != null)
            {
                if (uiController.MouseDown(sender, new MouseEventArgs(e.Button, e.Clicks, e.X + Properties.Settings.Default.ScreenHitTestOffsetX, e.Y + Properties.Settings.Default.ScreenHitTestOffsetY, e.Delta)))
                {
                    return;
                }
            }



            if (Control.ModifierKeys == (Keys.Control | Keys.Alt | Keys.Shift))
            {
                // Contrast code here
                contrastMode = true;
                return;
            }
            contrastMode = false;



            if (ProcessTouchControls(e))
            {
                return;
            }


            if (e.Button == MouseButtons.Left)
            {
                if (CurrentImageSet != null && CurrentImageSet.ReferenceFrame == null)
                {
                    switch (CurrentImageSet.DataSetType)
                    {
                        case ImageSetType.Earth:
                            CurrentImageSet.ReferenceFrame = "Earth";
                            break;
                        case ImageSetType.Planet:
                            break;
                        case ImageSetType.Sky:
                            CurrentImageSet.ReferenceFrame = "Sky";
                            break;
                        case ImageSetType.Panorama:
                            CurrentImageSet.ReferenceFrame = "Panorama";
                            break;
                        case ImageSetType.SolarSystem:
                            CurrentImageSet.ReferenceFrame = RenderEngine.SolarSystemTrack.ToString();
                            break;
                        default:
                            break;
                    }
                }
                if (CurrentImageSet != null)
                {
                    if (LayerManager.ClickCheckScreenSpace(new Point(e.X, e.Y), CurrentImageSet.ReferenceFrame))
                    {
                        return;
                    }
                }


            }


            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
            {

                if (Control.ModifierKeys == Keys.Alt || Control.ModifierKeys == Keys.Control || e.Button == MouseButtons.Middle)
                {
                    spinning = true;
                    angle = true;
                }
                else if (Control.ModifierKeys == Keys.Shift || RenderEngine.measuring)
                {
                    if (RenderEngine.Space)
                    {
                        RenderEngine.measuringDrag = true;
                        measureEnd = measureStart = GetCoordinatesForScreenPoint(e.X, e.Y);
                        if (RenderEngine.measureLines != null)
                        {
                            RenderEngine.measureLines.Clear();
                        }
                    }
                }
                else
                {
                    dragging = true;
                }
                moved = false;
                this.mouseDownX = e.X;
                this.mouseDownY = e.Y;
            }


        }

        enum TouchControls { ZoomIn, ZoomOut, Up, Down, Left, Right, Clockwise, CounterClockwise, TiltUp, TiltDown, TrackBall, Finder, Home, None, ZoomTrack, PanTrack, OrbitTrack };
        List<Vector2d> touchPoints = new List<Vector2d>();
        TouchControls activeTouch = TouchControls.None;
        Point touchTrackBallCenter = new Point();

        bool contrastMode = false;

        double zoomTrackerRadius = 76;
        Vector2d zoomTracker = new Vector2d(99, -15);
        Vector2d panTracker = new Vector2d(98, 77);
        Vector2d orbitTracker = new Vector2d(99, 187);

        public bool Friction
        {
            get
            {
                return Properties.Settings.Default.NavigationHold;
            }
            set
            {
                if (Properties.Settings.Default.NavigationHold != value)
                {
                    Properties.Settings.Default.NavigationHold = value;

                    if (Properties.Settings.Default.NavigationHold)
                    {
                        activeTouch = TouchControls.None;
                    }
                }

            }
        }


        private bool ProcessTouchControls(MouseEventArgs e)
        {
            activeTouch = TouchControls.None;
            if (Properties.Settings.Default.ShowTouchControls)
            {
                MakeTouchPoints();

                int tX = e.X + Properties.Settings.Default.ScreenHitTestOffsetX;
                int tY = e.Y + Properties.Settings.Default.ScreenHitTestOffsetY;

                if (tX > (renderWindow.Width - 207) && tX < (renderWindow.Width - 10))
                {
                    if (tY > renderWindow.Height - (234 + 120 + 30) && tY < (renderWindow.Height - 10))
                    {

                        Point hit = new Point(tX - (renderWindow.Width - 207), tY - (renderWindow.Height - (234 + 120)));

                        for (int i = touchPoints.Count - 1; i >= 0; i--)
                        {
                            Vector2d test = new Vector2d(hit.X, hit.Y);
                            test = test - touchPoints[i];
                            if (test.Length < 15)
                            {
                                activeTouch = (TouchControls)i;
                                if (activeTouch == TouchControls.TrackBall)
                                {
                                    touchTrackBallCenter = new Point(tX, tY);
                                    moveVector = new PointF();
                                    touchPoints[(int)TouchControls.PanTrack] = panTracker;
                                }
                                timer.Enabled = true;
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void MakeTouchPoints()
        {
            if (touchPoints == null || touchPoints.Count == 0)
            {
                touchPoints = new List<Vector2d>();
                touchPoints.Add(new Vector2d(45, 23)); // ZoomIn
                touchPoints.Add(new Vector2d(153, 23));// ZoomOut
                touchPoints.Add(new Vector2d(97, 32));// Up
                touchPoints.Add(new Vector2d(97, 120));// Down
                touchPoints.Add(new Vector2d(52, 77));// Left
                touchPoints.Add(new Vector2d(143, 77));// Right
                touchPoints.Add(new Vector2d(29, 117));// Rotate Left
                touchPoints.Add(new Vector2d(166, 117));// Rotate Right
                touchPoints.Add(new Vector2d(97, 161));// Tilt Up
                touchPoints.Add(new Vector2d(97, 212));// Tilt Down
                touchPoints.Add(new Vector2d(97, 76));// Track Ball
                touchPoints.Add(new Vector2d(172, 199));// Finder
                touchPoints.Add(new Vector2d(25, 199));// Home
                touchPoints.Add(new Vector2d(500, 500));// None
                touchPoints.Add(zoomTracker);// zoomTracker
                touchPoints.Add(panTracker);// panTracker
                touchPoints.Add(orbitTracker);// orbitTracker
            }
        }

        private bool ProcessKioskControls(MouseEventArgs e)
        {
            activeTouch = TouchControls.None;
            timer.Enabled = false;
            if (Properties.Settings.Default.ShowTouchControls)
            {
                MakeTouchPoints();

                int tX = e.X + Properties.Settings.Default.ScreenHitTestOffsetX;
                int tY = e.Y + Properties.Settings.Default.ScreenHitTestOffsetY;

                if (tX > (renderWindow.Width - 207) && tX < (renderWindow.Width - 10))
                {
                    if (tY > renderWindow.Height - (234 + 120) && tY < (renderWindow.Height - 10))
                    {
                        moveVector = new PointF();
                        Point hit = PointToTouch(new Point(tX, tY));
                        for (int i = 0; i < touchPoints.Count; i++)
                        {
                            Vector2d test = new Vector2d(hit.X, hit.Y);
                            test = test - touchPoints[i];
                            if (test.Length < 11)
                            {
                                activeTouch = (TouchControls)i;
                                if (activeTouch == TouchControls.TrackBall)
                                {
                                    touchTrackBallCenter = new Point(tX, tY);
                                }
                                timer.Enabled = true;
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private Point PointToTouch(Point pnt)
        {
            return new Point(pnt.X - (renderWindow.Width - 207), pnt.Y - (renderWindow.Height - (234 + 120)));
        }

        private Point TouchToScreen(Point pnt)
        {
            return new Point(pnt.X + (renderWindow.Width - 207), pnt.Y + (renderWindow.Height - (234 + 120)));
        }

        private Vector2d PointToTouch(Vector2d pnt)
        {
            return new Vector2d(pnt.X - (renderWindow.Width - 207), pnt.Y - (renderWindow.Height - (234 + 120)));
        }

        private Vector2d TouchToScreen(Vector2d pnt)
        {
            return new Vector2d(pnt.X + (renderWindow.Width - 207), pnt.Y + (renderWindow.Height - (234 + 120)));
        }

        private void renderWindow_MouseEnter(object sender, EventArgs e)
        {
            if (!NoStealFocus)
            {
                IntPtr hwndFG = GetForegroundWindow();
                if ((currentTab != null && currentTab.Handle == hwndFG) || (contextPanel != null && contextPanel.Handle == hwndFG) || (ObjectProperties.Props != null && ObjectProperties.Props.Handle == hwndFG))
                {
                    renderWindow.Focus();
                }
            }
        }

        private void renderWindow_MouseHover(object sender, EventArgs e)
        {

        }

        private void renderWindow_MouseLeave(object sender, EventArgs e)
        {
        }

        PointF moveVector = new PointF();
        bool mouseMoved = false;
        DateTime lastMouseMove = DateTime.Now;
        float ZoomVector = 0;
        float OrbitVector = 0;
        private void renderWindow_MouseMove(object sender, MouseEventArgs e)
        {



            if (contrastMode)
            {
                RenderEngine.contrast = (1 - (e.Y / (float)ClientSize.Height));
                RenderEngine.brightness = e.X / (float)ClientSize.Width;
                return;
            }




            if (activeTouch != TouchControls.None)
            {
                if (activeTouch == TouchControls.TrackBall)
                {
                    moveVector = new PointF(touchTrackBallCenter.X - (e.X + Properties.Settings.Default.ScreenHitTestOffsetX), touchTrackBallCenter.Y - (e.Y + Properties.Settings.Default.ScreenHitTestOffsetY));
                }

                if (activeTouch == TouchControls.PanTrack)
                {
                    Vector2d panTrack = TouchToScreen(this.panTracker);


                    Vector2d mv = new Vector2d(panTrack.X - (e.X + Properties.Settings.Default.ScreenHitTestOffsetX), panTrack.Y - (e.Y + Properties.Settings.Default.ScreenHitTestOffsetY));

                    if (mv.Length > 50)
                    {
                        mv.Normalize();
                        mv.Scale(50);
                    }

                    moveVector = new PointF((float)mv.X, (float)mv.Y);


                }

                if (activeTouch == TouchControls.ZoomTrack)
                {
                    Vector2d zoomTrack = TouchToScreen(this.zoomTracker);
                    double zoomDrag = zoomTrack.X - (e.X + Properties.Settings.Default.ScreenHitTestOffsetX);
                    if (Math.Abs(zoomDrag) > 54)
                    {
                        ZoomVector = 54 * Math.Sign(zoomDrag);
                    }
                    else
                    {
                        ZoomVector = (float)zoomDrag;
                    }
                }

                if (activeTouch == TouchControls.OrbitTrack)
                {
                    Vector2d orbitTrack = TouchToScreen(this.orbitTracker);
                    double orbitDrag = orbitTrack.X - (e.X + Properties.Settings.Default.ScreenHitTestOffsetX);
                    if (Math.Abs(orbitDrag) > 70)
                    {
                        OrbitVector = 70 * Math.Sign(orbitDrag);
                    }
                    else
                    {
                        OrbitVector = (float)orbitDrag;
                    }
                }

                return;
            }


            if (uiController != null)
            {
                if (uiController.MouseMove(sender, new MouseEventArgs(e.Button, e.Clicks, e.X + Properties.Settings.Default.ScreenHitTestOffsetX, e.Y + Properties.Settings.Default.ScreenHitTestOffsetY, e.Delta)))
                {
                    return;
                }
            }

            moved = true;


            if (lastMousePosition == e.Location)
            {
                return;
            }
            else
            {
                mouseMoved = true;
                lastMouseMove = DateTime.Now;

                if (!CursorVisible && !RenderEngine.ProjectorServer)
                {
                    Cursor.Show();
                    CursorVisible = true;
                }

            }

            if (RenderEngine.measuringDrag)
            {
                measureEnd = GetCoordinatesForScreenPoint(e.X, e.Y);

                if (RenderEngine.measureLines == null)
                {

                    RenderEngine.measureLines = new SimpleLineList11();
                    RenderEngine.measureLines.DepthBuffered = false;

                }
                RenderEngine.measureLines.Clear();
                RenderEngine.measureLines.AddLine(Coordinates.RADecTo3d(measureStart.RA + 12, measureStart.Dec, 1), Coordinates.RADecTo3d(measureEnd.RA + 12, measureEnd.Dec, 1));
                double angularSperation = CAAAngularSeparation.Separation(measureStart.RA, measureStart.Dec, measureEnd.RA, measureEnd.Dec);



                TourPlace pl = new TourPlace(Language.GetLocalizedText(977, "Seperation: ") + Coordinates.FormatDMS(angularSperation), measureEnd.Dec, measureEnd.RA, Classification.Star, Constellations.Containment.FindConstellationForPoint(measureEnd.RA, measureEnd.Dec), ImageSetType.Sky, -1);
                SetLabelText(pl, true);

            }
            else if (RenderEngine.Space && Settings.Active.GalacticMode)
            {
                if (dragging)
                {
                    RenderEngine.Tracking = false;

                    MoveView(-(e.X - this.mouseDownX), (e.Y - this.mouseDownY), true);
                    if (!Properties.Settings.Default.SmoothPan)
                    {
                        RenderEngine.Az = RenderEngine.targetAz;
                        RenderEngine.Alt = RenderEngine.targetAlt;
                        double[] gPoint = Coordinates.GalactictoJ2000(RenderEngine.Az, RenderEngine.Alt);
                        RenderEngine.TargetLat = RenderEngine.ViewLat = gPoint[1];
                        RenderEngine.TargetLong = RenderEngine.ViewLong = RAtoViewLng(gPoint[0] / 15);
                        NotifyMoveComplete();
                    }
                    this.mouseDownX = e.X;
                    this.mouseDownY = e.Y;
                }
                else if (spinning || angle)
                {

                    RenderEngine.CameraRotateTarget = (RenderEngine.CameraRotateTarget + (((double)(e.X - this.mouseDownX)) / 1000 * Math.PI));

                    RenderEngine.CameraAngleTarget = (RenderEngine.CameraAngleTarget + (((double)(e.Y - this.mouseDownY)) / 1000 * Math.PI));

                    if (RenderEngine.CameraAngleTarget < TiltMin)
                    {
                        RenderEngine.CameraAngleTarget = TiltMin;
                    }

                    if (RenderEngine.CameraAngleTarget > 0)
                    {
                        RenderEngine.CameraAngleTarget = 0;
                    }

                    if (!Properties.Settings.Default.SmoothPan)
                    {
                        RenderEngine.CameraRotate = RenderEngine.CameraRotateTarget;
                        RenderEngine.CameraAngle = RenderEngine.CameraAngleTarget;
                    }

                    this.mouseDownX = e.X;
                    this.mouseDownY = e.Y;
                }
                else
                {
                    mouseMoved = true;
                    lastMouseMove = DateTime.Now;
                }
            }
            else if (RenderEngine.Space && Settings.Active.LocalHorizonMode)
            {
                if (dragging)
                {
                    if (!SolarSystemMode)
                    {
                        RenderEngine.Tracking = false;
                    }

                    MoveView(-(e.X - this.mouseDownX), (e.Y - this.mouseDownY), true);
                    if (!Properties.Settings.Default.SmoothPan)
                    {
                        RenderEngine.Az = RenderEngine.targetAz;
                        RenderEngine.Alt = RenderEngine.targetAlt;
                        Coordinates currentRaDec = Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(RenderEngine.Alt, RenderEngine.Az), SpaceTimeController.Location, SpaceTimeController.Now);

                        RenderEngine.TargetLat = RenderEngine.ViewLat = currentRaDec.Dec;
                        RenderEngine.TargetLong = RenderEngine.ViewLong = RAtoViewLng(currentRaDec.RA);
                        NotifyMoveComplete();
                    }
                    this.mouseDownX = e.X;
                    this.mouseDownY = e.Y;
                }
                else
                {
                    mouseMoved = true;
                    lastMouseMove = DateTime.Now;
                }
            }
            else
            {
                if (dragging)
                {
                    if (!SolarSystemMode)
                    {
                        RenderEngine.Tracking = false;
                    }

                    MoveView(-(e.X - this.mouseDownX), (e.Y - this.mouseDownY), true);
                    if (!Properties.Settings.Default.SmoothPan)
                    {
                        RenderEngine.ViewLat = RenderEngine.TargetLat;
                        RenderEngine.ViewLong = RenderEngine.TargetLong;
                        NotifyMoveComplete();
                    }
                    this.mouseDownX = e.X;
                    this.mouseDownY = e.Y;
                }
                else if (spinning || angle)
                {

                    RenderEngine.CameraRotateTarget = (RenderEngine.CameraRotateTarget + (((double)(e.X - this.mouseDownX)) / 1000 * Math.PI));

                    RenderEngine.CameraAngleTarget = (RenderEngine.CameraAngleTarget + (((double)(e.Y - this.mouseDownY)) / 1000 * Math.PI));

                    if (RenderEngine.CameraAngleTarget < TiltMin)
                    {
                        RenderEngine.CameraAngleTarget = TiltMin;
                    }

                    if (RenderEngine.CameraAngleTarget > 0)
                    {
                        RenderEngine.CameraAngleTarget = 0;
                    }

                    if (!Properties.Settings.Default.SmoothPan)
                    {
                        RenderEngine.CameraRotate = RenderEngine.CameraRotateTarget;
                        RenderEngine.CameraAngle = RenderEngine.CameraAngleTarget;
                    }

                    this.mouseDownX = e.X;
                    this.mouseDownY = e.Y;
                }
                else
                {
                    mouseMoved = true;
                    lastMouseMove = DateTime.Now;

                }
            }

            lastMousePosition = e.Location;
        }

        private void renderWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (contrastMode)
            {
                contrastMode = false;
                return;
            }


            if (activeTouch != TouchControls.None)
            {
                if (activeTouch == TouchControls.Finder)
                {
                    if (kioskControl)
                    {
                        if (ObjectProperties.Active)
                        {
                            ObjectProperties.HideProperties();
                        }
                        else
                        {
                            ShowFinder();
                        }
                    }
                    else
                    {
                        Friction = !Friction;
                    }
                }
                if (activeTouch == TouchControls.Home)
                {
                    if (kioskControl)
                    {
                        if (TouchKiosk)
                        {
                            Properties.Settings.Default.SolarSystemScale = 1;
                            RenderEngine.FadeInImageSet(RenderEngine.GetDefaultImageset(ImageSetType.SolarSystem, BandPass.Visible));
                            CameraParameters camParams = new CameraParameters(45, 0, 360, 0, 0, 100);
                            RenderEngine.GotoTarget(camParams, false, true);
                        }
                        else
                        {
                            CameraParameters camParams = new CameraParameters(0, 0, 360, 0, 0, 100);
                            RenderEngine.GotoTarget(camParams, false, true);
                        }
                    }
                    else
                    {
                        TouchAllStop();
                    }
                }

                activeTouch = TouchControls.None;

                return;
            }


            if (uiController != null)
            {
                if (uiController.MouseUp(sender, new MouseEventArgs(e.Button, e.Clicks, e.X + Properties.Settings.Default.ScreenHitTestOffsetX, e.Y + Properties.Settings.Default.ScreenHitTestOffsetY, e.Delta)))
                {
                    return;
                }
            }

            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
            {
                dragging = false;
                spinning = false;
                RenderEngine.measuringDrag = false;
                RenderEngine.measuring = false;
                angle = false;
                if (!moved && RenderEngine.ShowKmlMarkers && RenderEngine.Space)
                {

                    Point cursor = renderWindow.PointToClient(Cursor.Position);

                    Coordinates result = GetCoordinatesForScreenPoint(cursor.X, cursor.Y);

                    if (CurrentImageSet.DataSetType == ImageSetType.Sky)
                    {
                        if (!RenderEngine.ProjectorServer)
                        {
                            if (RenderEngine.constellationCheck != null)
                            {
                                if ( RenderEngine.ShowKmlMarkers && RenderEngine.KmlMarkers != null)
                                {
                                    RenderEngine.KmlMarkers.ItemClick(Coordinates.RADecTo3dDouble(result, 1.0).Vector311, (float)(RenderEngine.ZoomFactor / 900.0));
                                }
                            }
                        }
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if ((CurrentImageSet.DataSetType == ImageSetType.Sky || CurrentImageSet.DataSetType == ImageSetType.SolarSystem /*|| CurrentImageSet.DataSetType == ImageSetType.Planet || CurrentImageSet.DataSetType == ImageSetType.Earth*/ ) && !TourPlayer.Playing)
                {


                    if (figureEditor != null)
                    {
                        // TODO fix this for earth, plantes, panoramas
                        Coordinates result = GetCoordinatesForScreenPoint(e.X, e.Y);
                        string constellation = RenderEngine.constellationCheck.FindConstellationForPoint(result.RA, result.Dec);
                        contextPanel.Constellation = Constellations.FullName(constellation);
                        IPlace closetPlace = ContextSearch.FindClosestMatch(constellation, result.RA, result.Dec, RenderEngine.DegreesPerPixel * 80);
                        if (closetPlace == null)
                        {
                            closetPlace = new TourPlace(Language.GetLocalizedText(90, "No Object"), result.Dec, result.RA, Classification.Unidentified, constellation, ImageSetType.Sky, -1);
                        }
                        figureEditor.AddFigurePoint(closetPlace);
                    }
                    else
                    {
                        Point pntShow = new Point(e.X, e.Y);

                        if (SolarSystemMode)
                        {
                            ObjectProperties.ShowAt(renderWindow.PointToScreen(pntShow));

                        }
                        else
                        {
                            ShowPropertiesForPoint(pntShow);
                        }
                    }
                }
            }
        }

        private void TouchAllStop()
        {
            moveVector = new PointF();
            touchPoints[(int)TouchControls.PanTrack] = panTracker;
            touchPoints[(int)TouchControls.OrbitTrack] = orbitTracker;
            touchPoints[(int)TouchControls.ZoomTrack] = zoomTracker;
            ZoomVector = 0;
            OrbitVector = 0;
        }

        private void renderWindow_Resize(object sender, EventArgs e)
        {
        }

        private void renderWindow_Paint(object sender, PaintEventArgs e)
        {
            if (RenderContext11 != null && RenderContext11.Device != null && RenderEngine.ReadyToRender && !pause && !SpaceTimeController.FrameDumping)
            {
                RenderEngine.Render();
            }
            else
            {
                e.Graphics.Clear(Color.Black);
            }
        }

        bool playingSlideShow = false;
        private void playCollectionAsSlideShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (playingSlideShow)
            {
                StopSlideShow();
            }
            else
            {
                PlayCollection();
            }
        }

        private void extractThumb(Folder folder)
        {
            foreach (object child in folder.Children)
            {
                if (child is Folder)
                {
                    Folder ims = child as Folder;
                    IThumbnail imt = child as IThumbnail;
                    if (ims != null && imt != null)
                    {
                        if (!string.IsNullOrEmpty(ims.Thumbnail) && ims.Thumbnail.Contains("wwt.nasa"))
                        {
                            string filename = ims.Thumbnail.Substring(ims.Thumbnail.LastIndexOf("/") + 1);
                            if (!File.Exists("c:\\marsthumbs\\" + filename))
                            {
                                Bitmap bmp = imt.ThumbNail;
                                bmp.Save("c:\\marsthumbs\\" + filename);
                                bmp.Dispose();
                            }
                        }
                    }


                    extractThumb(child as Folder);
                }
                if (child is IImageSet)
                {
                    IImageSet ims = child as IImageSet;
                    IThumbnail imt = child as IThumbnail;
                    if (ims != null && imt != null)
                    {
                        if (ims.ThumbnailUrl.Contains("wwt.nasa"))
                        {
                            string filename = ims.ThumbnailUrl.Substring(ims.ThumbnailUrl.LastIndexOf("/") + 1);
                            if (!File.Exists("c:\\marsthumbs\\" + filename))
                            {
                                Bitmap bmp = imt.ThumbNail;
                                bmp.Save("c:\\marsthumbs\\" + filename);
                                bmp.Dispose();
                            }
                        }
                    }
                }
                if (child is IPlace)
                {
                    IPlace ims = child as IPlace;
                    IThumbnail imt = child as IThumbnail;
                    if (ims != null && imt != null)
                    {
                        if (!string.IsNullOrEmpty(ims.Thumbnail) && ims.Thumbnail.Contains("wwt.nasa"))
                        {
                            string filename = ims.Thumbnail.Substring(ims.Thumbnail.LastIndexOf("/") + 1);
                            Bitmap bmp = imt.ThumbNail;
                            bmp.Save("c:\\marsthumbs\\" + filename);
                            bmp.Dispose();
                        }
                    }
                }
            }
        }

        private void StopSlideShow()
        {
            playingSlideShow = false;
            SlideAdvanceTimer.Enabled = false;
        }

        private void PlayCollection()
        {
            playingSlideShow = true;
            SlideAdvanceTimer.Enabled = true;
            SlideAdvanceTimer.Interval = 500;
            if (!currentTab.AdvanceSlide(true))
            {
                StopSlideShow();
            }
        }

        private void SlideAdvanceTimer_Tick(object sender, EventArgs e)
        {
            if (playingSlideShow)
            {
                if (SlideAdvanceTimer.Interval == 500)
                {
                    if (RenderEngine.Mover == null)
                    {
                        SlideAdvanceTimer.Interval = 10000;
                    }
                }
                else
                {
                    if (!currentTab.AdvanceSlide(false))
                    {
                        StopSlideShow();
                        if (Properties.Settings.Default.AutoRepeatTour)
                        {
                            PlayCollection();
                        }
                    }
                    else
                    {
                        SlideAdvanceTimer.Interval = 500;
                    }
                }
            }
            else
            {
                SlideAdvanceTimer.Enabled = false;
            }
        }

        void TourPlayer_TourEnded(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.AutoRepeatTourAll && (tourEdit != null && !tourEdit.Tour.EditMode))
            {
                toursTab.PlayNext();
            }
        }

        private void oneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoRepeatTour = true;
            Properties.Settings.Default.AutoRepeatTourAll = false;

        }

        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoRepeatTour = false;
            Properties.Settings.Default.AutoRepeatTourAll = true;

        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoRepeatTour = false;
            Properties.Settings.Default.AutoRepeatTourAll = false;
        }

        private void toursMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Control.ModifierKeys == (Keys.Shift | Keys.Control))
            {
                Properties.Settings.Default.AdvancedCommunities = true;
            }

            if (menuTabs.CurrentTour != null)
            {
                if (Control.ModifierKeys == (Keys.Shift | Keys.Control))
                {
                    publishTourMenuItem.Visible = menuTabs.CurrentTour.EditMode;
                    Properties.Settings.Default.AdvancedCommunities = true;
                }
                else
                {
                    publishTourMenuItem.Visible = false;
                }
                editTourToolStripMenuItem.Visible = !menuTabs.CurrentTour.EditMode;

                publishTourToCommunityToolStripMenuItem.Enabled = Earth3d.IsLoggedIn;

                renderToVideoToolStripMenuItem.Enabled = true;
                showOverlayListToolStripMenuItem.Enabled = true;
                showSlideNumbersToolStripMenuItem.Visible = true;
                showSlideNumbersToolStripMenuItem.Checked = Properties.Settings.Default.ShowSlideNumbers;
                showKeyframerToolStripMenuItem.Visible = menuTabs.CurrentTour.EditMode;
                showKeyframerToolStripMenuItem.Checked = TimeLine.AreOpenTimelines;
            }
            else
            {
                publishTourToCommunityToolStripMenuItem.Enabled = false;
                renderToVideoToolStripMenuItem.Enabled = false;
                publishTourMenuItem.Visible = false;
                editTourToolStripMenuItem.Visible = false;
                showOverlayListToolStripMenuItem.Enabled = false;
                showSlideNumbersToolStripMenuItem.Visible = false;
                showKeyframerToolStripMenuItem.Visible = false;
            }

            undoToolStripMenuItem.Text = "&" + Language.GetLocalizedText(643, "Undo:") + " " + Undo.PeekActionString();
            undoToolStripMenuItem.Enabled = Undo.PeekAction();

            redoToolStripMenuItem.Text = "&" + Language.GetLocalizedText(644, "Redo:") + " " + Undo.PeekRedoActionString();
            redoToolStripMenuItem.Enabled = Undo.PeekRedoAction();

            saveTourAsToolStripMenuItem.Visible = tourEdit != null;

            oneToolStripMenuItem.Checked = Properties.Settings.Default.AutoRepeatTour;
            allToolStripMenuItem.Checked = Properties.Settings.Default.AutoRepeatTourAll;
            offToolStripMenuItem.Checked = !oneToolStripMenuItem.Checked && !allToolStripMenuItem.Checked;

            sendTourToProjectorServersToolStripMenuItem.Enabled = Settings.MasterController;
            automaticTourSyncWithProjectorServersToolStripMenuItem.Enabled = Settings.MasterController;
            automaticTourSyncWithProjectorServersToolStripMenuItem.Checked = Properties.Settings.Default.AutoSyncTours;
        }

        private void Earth3d_KeyUp(object sender, KeyEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.E:
                        menuTabs.ShowTabMenu(ApplicationMode.Explore);
                        break;
                    case Keys.G:
                        menuTabs.ShowTabMenu(ApplicationMode.Tours);
                        break;
                    case Keys.S:
                        menuTabs.ShowTabMenu(ApplicationMode.Search);
                        break;
                    case Keys.C:
                        menuTabs.ShowTabMenu(ApplicationMode.Community);
                        break;
                    case Keys.T:
                        menuTabs.ShowTabMenu(ApplicationMode.Telescope);
                        break;
                    case Keys.V:
                        menuTabs.ShowTabMenu(ApplicationMode.View);
                        break;
                    case Keys.N:
                        menuTabs.ShowTabMenu(ApplicationMode.Settings);
                        break;
                }
            }
        }

        private void exploreMenu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    menuTabs.ShowTabMenu(+1);
                    break;

                case Keys.Left:
                    menuTabs.ShowTabMenu(-1);
                    break;
            }
        }

        private void addToCollectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        void newCollectionMenu_Click(object sender, EventArgs e)
        {
            Folder folder = explorePane.NewCollection();
            if (folder != null)
            {
                folder.AddChildPlace(Place.FromIPlace(contextMenuTargetObject));
            }
        }

        void AddTocollectionMenu_Click(object sender, EventArgs e)
        {
            Folder folder = (Folder)((ToolStripMenuItem)sender).Tag;
            folder.AddChildPlace(Place.FromIPlace(contextMenuTargetObject));

            explorePane.ReloadFolder();

            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            contextMenu.Close();
        }

        private void addToCollectionsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            addToCollectionsToolStripMenuItem.DropDownItems.Clear();

            ToolStripMenuItem newCollectionMenu = new ToolStripMenuItem(Language.GetLocalizedText(23, "New Collection..."));
            newCollectionMenu.Click += new EventHandler(newCollectionMenu_Click);
            addToCollectionsToolStripMenuItem.DropDownItems.Add(newCollectionMenu);

            ToolStripMenuItem menuItem = addToCollectionsToolStripMenuItem;
            addToCollectionsToolStripMenuItem.Tag = this.explorePane.MyCollections;

            CreatePickFolderMenu(menuItem);
        }

        private void CreatePickFolderMenu(ToolStripMenuItem menuItem)
        {
            Folder collections = (Folder)menuItem.Tag;

            foreach (Folder f in collections.Folder1)
            {
                ToolStripMenuItem tempMenu = new ToolStripMenuItem(f.Name);
                tempMenu.Click += new EventHandler(AddTocollectionMenu_Click);
                tempMenu.Tag = f;
                menuItem.DropDownItems.Add(tempMenu);
                CreatePickFolderMenu(tempMenu);
            }
        }
        private void removeFromCollectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (explorePane != null)
            {
                explorePane.RemoveSelected();
            }
        }

        private void contextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Earth3d.NoStealFocus = true;

        }

        private void contextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            Earth3d.NoStealFocus = false;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaceEditor editor = new PlaceEditor();
            editor.EditTarget = contextMenuTargetObject;
            if (editor.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void TourEndCheck_Tick(object sender, EventArgs e)
        {
            if (showTourCompleteDialog && !NoShowTourEndPage)
            {
                showTourCompleteDialog = false;
                if (tourEdit != null)
                {
                    DialogResult dlgResult = TourPopup.ShowEndTourPopupModal(tourEdit.Tour);
                    if (dlgResult == DialogResult.OK)
                    {
                        tourEdit.PlayNow(true);
                    }
                    else if (dlgResult == DialogResult.Cancel)
                    {
                        CloseTour(false);
                    }
                }
            }
        }

        private void autoSaveTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (explorePane != null)
                {
                    explorePane.AutoSave();
                }
            }
            catch
            {
            }

        }

        private void removeFromImageCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IPlace place = (IPlace)contextMenuTargetObject;
                IImageSet imageSet = null;
                if (place.StudyImageset != null)
                {
                    imageSet = place.StudyImageset;
                }
                if (place.BackgroundImageSet != null)
                {
                    imageSet = place.BackgroundImageSet;
                }

                long totalBytes = 0;
                long demBytes = 0;
                string path = Properties.Settings.Default.CahceDirectory + @"Imagery\" + imageSet.ImageSetID.ToString();

                string demPath = Properties.Settings.Default.CahceDirectory + @"dem\" + Math.Abs(imageSet.DemUrl != null ? imageSet.DemUrl.GetHashCode32() : 0).ToString();

                Cursor.Current = Cursors.WaitCursor;

                if (!Directory.Exists(path) && !Directory.Exists(demPath))
                {
                    UiTools.ShowMessageBox(Language.GetLocalizedText(127, "There is no image data in the disk cache to purge."));
                }

                ClearCache.PurgeDirecotryNoProgress(path, ref totalBytes);


                if (Directory.Exists(demPath))
                {
                    ClearCache.PurgeDirecotryNoProgress(demPath, ref demBytes);
                }

                Cursor.Current = Cursors.Default;
                UiTools.ShowMessageBox(String.Format(Language.GetLocalizedText(128, "The imageset was removed from cache and {0}MB was freed"), ((double)(totalBytes + demBytes) / 1024.0 / 1024.0).ToString("f")));

            }
            catch
            {


            }

        }

        private void selectLanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LanguageSelect dialog = new LanguageSelect();

            dialog.Language = Language.CurrentLanguage;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.Language.Url != Language.CurrentLanguage.Url)
                {
                    if (dialog.Language.Code == "ZZZZ")
                    {
                        OpenFileDialog openFile = new OpenFileDialog();
                        openFile.Filter = "WWT Language File|*.tdf";
                        if (openFile.ShowDialog() == DialogResult.OK)
                        {
                            string filename = openFile.FileName;

                            try
                            {
                                Uri uri = new Uri(filename);
                                if (uri.IsFile)
                                {
                                    filename = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

                                }
                                dialog.Language.Url = "file://" + filename;
                            }
                            catch
                            {
                                //todo localization
                                UiTools.ShowMessageBox("Could Not Open Language Table File. Please ensure it is valid");
                            }
                        }
                    }
                    bool useProxy = false;

                    if (!string.IsNullOrEmpty(dialog.Language.Proxy) && dialog.Language.Proxy != Properties.Settings.Default.SharedCacheServer)
                    {
                        DialogResult result1 = UiTools.ShowMessageBox(Language.GetLocalizedText(691, "This Language Pack has an optional regional data cache that will allow improved performance when in that region. Do you wish to use it?"), Language.GetLocalizedText(692, "Use Regional Data Cache"), MessageBoxButtons.YesNo);
                        if (result1 == DialogResult.Yes)
                        {
                            useProxy = true;
                        }
                    }


                    //todo Localize Text
                    DialogResult result = UiTools.ShowMessageBox(Language.GetLocalizedText(693, "WorldWide Telescope must restart to load a new language file. Do you want to restart now?"), Language.GetLocalizedText(694, "Restart Now?"), MessageBoxButtons.YesNoCancel);
                    switch (result)
                    {
                        case DialogResult.Cancel:
                            return;
                        case DialogResult.No:
                            Properties.Settings.Default.ImageSetUrl = dialog.Language.ImageSetsUrl;
                            Properties.Settings.Default.ExploreRootUrl = dialog.Language.RootUrl;
                            Properties.Settings.Default.LanguageCode = dialog.Language.Code;
                            Properties.Settings.Default.LanguageName = dialog.Language.Name;
                            Properties.Settings.Default.LanguageUrl = dialog.Language.Url;
                            if (useProxy)
                            {
                                Properties.Settings.Default.SharedCacheServer = dialog.Language.Proxy;
                            }
                            break;

                        case DialogResult.Yes:
                            Properties.Settings.Default.ImageSetUrl = dialog.Language.ImageSetsUrl;
                            Properties.Settings.Default.ExploreRootUrl = dialog.Language.RootUrl;
                            Properties.Settings.Default.LanguageCode = dialog.Language.Code;
                            Properties.Settings.Default.LanguageName = dialog.Language.Name;
                            Properties.Settings.Default.LanguageUrl = dialog.Language.Url;
                            LanguageReboot = true;
                            if (useProxy)
                            {
                                Properties.Settings.Default.SharedCacheServer = dialog.Language.Proxy;
                            }
                            this.Close();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        static bool LanguageReboot = false;
        static bool CloseNow = false;
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tourEdit != null)
            {
                tourEdit.UndoStep();
            }

        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tourEdit != null)
            {
                tourEdit.RedoStep();
            }
        }

        private void saveTourAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tourEdit != null)
            {
                tourEdit.Save(true);
            }
        }

        private void setAsForegroundImageryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IPlace place = (IPlace)contextMenuTargetObject;
                IImageSet imageSet = null;
                if (place.StudyImageset != null)
                {
                    imageSet = place.StudyImageset;
                }
                if (place.BackgroundImageSet != null)
                {
                    imageSet = place.BackgroundImageSet;
                }

                RenderEngine.SetStudyImageset(imageSet, null);
            }
            catch
            {


            }
        }

        private void setAsBackgroundImageryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IPlace place = (IPlace)contextMenuTargetObject;
                IImageSet imageSet = null;
                if (place.StudyImageset != null)
                {
                    imageSet = place.StudyImageset;
                }
                if (place.BackgroundImageSet != null)
                {
                    imageSet = place.BackgroundImageSet;
                }

                if (imageSet != null)
                {
                    this.CurrentImageSet = imageSet;
                }
            }
            catch
            {


            }
        }
        
        private void addToImageStackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IPlace place = (IPlace)contextMenuTargetObject;
                AddPlaceToStack(place, true);
                ShowImageStack();
            }
            catch
            {


            }
        }

        public void AddPlaceToStack(IPlace place, bool refresh)
        {
            IImageSet imageSet = null;
            if (place.StudyImageset != null)
            {
                imageSet = place.StudyImageset;
            }
            if (place.BackgroundImageSet != null)
            {
                imageSet = place.BackgroundImageSet;
            }

            if (imageSet != null)
            {
                RenderEngine.ImageStackList.Add(ImageSet.FromIImage(imageSet));
                if (refresh)
                {
                    stack.UpdateList();
                }
            }
        }

        public void RemovePlaceFromStack(IPlace place, bool refresh)
        {
            if (place == null)
            {
                return;
            }

            IImageSet imageSet = null;
            if (place.StudyImageset != null)
            {
                imageSet = place.StudyImageset;
            }
            if (place.BackgroundImageSet != null)
            {
                imageSet = place.BackgroundImageSet;
            }

            if (imageSet != null)
            {
                IImageSet itemToRemove = null;
                foreach (IImageSet set in RenderEngine.ImageStackList)
                {
                    if (set.GetHash() == imageSet.GetHash())
                    {
                        itemToRemove = set;
                    }
                }
                RenderEngine.ImageStackList.Remove(itemToRemove);
                if (refresh)
                {
                    stack.UpdateList();
                }
            }
        }
        private void toggleFullScreenModeF11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowFullScreen(!fullScreen);
        }

        private void virtualObservatorySearchesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void NEDSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = String.Format("http://nedwww.ipac.caltech.edu/cgi-bin/nph-objsearch?search_type=Near+Position+Search&of=xml_main&RA={0}&DEC={1}&SR={2}", (contextMenuTargetObject.RA * 15).ToString(), contextMenuTargetObject.Dec.ToString(), (RenderEngine.FovAngle) < (1.0 / 60.0) ? (RenderEngine.FovAngle).ToString() : (1.0 / 60.0).ToString());

            RunVoSearch(url, null);

        }

        public void RunVoSearch(string url, string ID)
        {
            string filename = Path.GetTempFileName();
            Cursor.Current = Cursors.WaitCursor;

            if (!FileDownload.DownloadFile(url, filename, true))
            {
                return;
            }

            try
            {
                VoTable voTable = new VoTable(filename);
                voTable.SampId = ID;
                if (!voTable.error)
                {
                    VOTableViewer viewer = new VOTableViewer();
                    VoTableLayer layer = LayerManager.AddVoTableLayer(voTable, "VO Table");

                    if (ID != null)
                    {
                        Samp.sampKnownTableIds.Add(ID, layer);

                    }
                    try
                    {
                        Samp.sampKnownTableUrls.Add(url, layer);
                    }
                    catch
                    {

                    }
                    viewer.Layer = layer;
                    viewer.Show();
                    ShowLayersWindows = true;

                }
                else
                {
                    UiTools.ShowMessageBox(voTable.errorText);
                }
            }
            catch
            {
                WebWindow.OpenUrl(url, true);
            }
            Cursor.Current = Cursors.Default;
        }

        private void vOTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "VOTable|*.xml";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filename = openFile.FileName;
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    VoTable voTable = new VoTable(filename);
                    VoTableLayer layer = LayerManager.AddVoTableLayer(voTable, Path.GetFileName(filename));

                    VOTableViewer viewer = new VOTableViewer();
                    viewer.Layer = layer;
                    viewer.Show();
                    ShowLayersWindows = true;
                }
                catch
                {
                    //todo localization
                    UiTools.ShowMessageBox(Language.GetLocalizedText(695, "Could Not Open VO Table File. Please ensure it is valid"));
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void searchMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void vORegistryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VORegistryBrowser browser = new VORegistryBrowser();
            if (browser.ShowDialog() == DialogResult.OK)
            {
                RunVoSearch(browser.URL, null);
            }
        }

        private void sDSSSearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = String.Format("http://casjobs.sdss.org/vo/dr5cone/sdssConeSearch.asmx/ConeSearch?ra={0}&dec={1}&sr={2}", (contextMenuTargetObject.RA * 15).ToString(), contextMenuTargetObject.Dec.ToString(), RenderEngine.FovAngle.ToString());

            RunVoSearch(url, null);
        }

        private void stereoToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            oculusRiftToolStripMenuItem.Checked = RenderEngine.StereoMode == RenderEngine.StereoModes.OculusRift;
            enabledToolStripMenuItem.Checked = RenderEngine.StereoMode == RenderEngine.StereoModes.Off;
            anaglyphToolStripMenuItem.Checked = RenderEngine.StereoMode == RenderEngine.StereoModes.AnaglyphRedCyan;
            anaglyphYellowBlueToolStripMenuItem.Checked = RenderEngine.StereoMode == RenderEngine.StereoModes.AnaglyphYellowBlue;
            sideBySideProjectionToolStripMenuItem.Checked = RenderEngine.StereoMode == RenderEngine.StereoModes.SideBySide;
            sideBySideCrossEyedToolStripMenuItem.Checked = RenderEngine.StereoMode == RenderEngine.StereoModes.CrossEyed;
            alternatingLinesEvenToolStripMenuItem.Checked = RenderEngine.StereoMode == RenderEngine.StereoModes.InterlineEven;
            alternatingLinesOddToolStripMenuItem.Checked = RenderEngine.StereoMode == RenderEngine.StereoModes.InterlineOdd;
        }

        private void enabledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderEngine.rift)
            {
                RenderEngine.rift = false;
                AttachRenderWindow();
            }
            RenderEngine.StereoMode = RenderEngine.StereoModes.Off;
            Properties.Settings.Default.ColSettingsVersion++;
        }



        private void anaglyphYellowBlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderEngine.rift)
            {
                RenderEngine.rift = false;
                AttachRenderWindow();
            }
            RenderEngine.StereoMode = RenderEngine.StereoModes.AnaglyphYellowBlue;
            Properties.Settings.Default.ColSettingsVersion++;
        }
        private void anaglyphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderEngine.rift)
            {
                RenderEngine.rift = false;
                AttachRenderWindow();
            }
            RenderEngine.StereoMode = RenderEngine.StereoModes.AnaglyphRedCyan;
            Properties.Settings.Default.ColSettingsVersion++;
        }

        private void sideBySideProjectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RenderEngine.rift)
            {
                RenderEngine.rift = false;
                AttachRenderWindow();
            }
            RenderEngine.StereoMode = RenderEngine.StereoModes.SideBySide;
            Properties.Settings.Default.ColSettingsVersion++;
        }

       

        private void sideBySideCrossEyedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderEngine.StereoMode = RenderEngine.StereoModes.CrossEyed;
            Properties.Settings.Default.ColSettingsVersion++;

        }


        private void alternatingLinesOddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderEngine.StereoMode = RenderEngine.StereoModes.InterlineOdd;
            Properties.Settings.Default.ColSettingsVersion++;
        }

        private void alternatingLinesEvenToolStripMenuItem_Click(object sender, EventArgs e)
        {

            RenderEngine.StereoMode = RenderEngine.StereoModes.InterlineEven;
            Properties.Settings.Default.ColSettingsVersion++;
        }

        private void fullDomeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DomeView = !Properties.Settings.Default.DomeView;
            Settings.DomeView = false;

            if (Properties.Settings.Default.DomeView)
            {
                NetControl.Start();
            }
        }



        private void lookUpOnNEDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextMenuTargetObject.Classification == Classification.Unidentified)
            {
                string url = "http://nedwww.ipac.caltech.edu/cgi-bin/nph-objsearch?in_csys=Equatorial&in_equinox=J2000.0&lon={0}d&lat={1}d&radius={2}&hconst=73&omegam=0.27&omegav=0.73&corr_z=1&search_type=Near+Position+Search&z_constraint=Unconstrained&z_value1=&z_value2=&z_unit=z&ot_include=ANY&nmp_op=ANY&out_csys=Equatorial&out_equinox=J2000.0&obj_sort=Distance+to+search+center&of=pre_text&zv_breaker=30000.0&list_limit=5&img_stamp=YES";
                WebWindow.OpenUrl(String.Format(url, contextMenuTargetObject.RA, contextMenuTargetObject.Dec.ToString(),
                    (RenderEngine.FovAngle * 60) < 1 ? (RenderEngine.FovAngle * 60).ToString() : "1.0"), false);
            }
            else
            {
                string url = "http://nedwww.ipac.caltech.edu/cgi-bin/nph-imgdata?objname={0}";
                WebWindow.OpenUrl(String.Format(url, contextMenuTargetObject.Name), false);
            }

        }

        private void domeSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DomeSetup dome = new DomeSetup();
            dome.Show();
        }
        bool menu = false;
        bool showNextObject = false;
        bool contact = false;
        bool kinectHeard = false;
        internal void MoveAndZoomRate(double leftRight, double upDown, double zoom, string mode)
        {
            kinectHeard = true;
            if (mode == "Menu")
            {
                menu = true;
                kinectPickValue = 0;
            }



            if (!menu)
            {
                MoveView(leftRight, upDown, false);
                NetZoomRate = zoom;
            }
            else
            {
                contact = mode == "Contact";
                if (contact)
                {
                    kinectListOffsetTarget -= leftRight;
                    kinectPickValue += upDown;
                    if (kinectPickValue > 600)
                    {
                        showNextObject = true;
                    }
                    else if (kinectPickValue < -600)
                    {
                        menu = false;
                    }
                    else
                    {
                        kinectPickValue *= .95;
                    }
                }
            }
        }

        private void listenUpBoysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ListenMode = !Properties.Settings.Default.ListenMode;
            listenUpBoysToolStripMenuItem.Checked = Properties.Settings.Default.ListenMode;
        }

        internal void SetBackgroundByName(string name)
        {
            Folder target = Earth3d.MainWindow.ExplorerRoot;


            foreach (IImageSet set in RenderEngine.ImageSets)
            {
                if (set.Name.ToLower().Contains(name.ToLower()))
                {
                    CurrentImageSet = set;
                    return;
                }
            }


            foreach (object o in target.Children)
            {
                if (o is Folder)
                {
                    if (SetBackgroundByName(o as Folder, name))
                    {
                        return;
                    }
                }
            }

            target = explorePane.MyCollections;

            foreach (object o in target.Children)
            {
                if (o is Folder)
                {
                    if (SetBackgroundByName(o as Folder, name))
                    {
                        return;
                    }
                }
            }
        }

        private bool SetBackgroundByName(Folder folder, string name)
        {
            foreach (object o in folder.Children)
            {
                if (o is IPlace)
                {

                    IPlace place = (IPlace)o;

                    if (place.Name.ToLower().Contains(name.ToLower()))
                    {
                        if (place.BackgroundImageSet != null && place.BackgroundImageSet.DataSetType != Earth3d.MainWindow.CurrentImageSet.DataSetType)
                        {
                            continue;
                        }
                        if (place.Classification == Classification.SolarSystem && Earth3d.MainWindow.CurrentImageSet.DataSetType == ImageSetType.SolarSystem)
                        {
                            CurrentImageSet = new ImageSetHelper(ImageSetType.SolarSystem, BandPass.Visible);
                        }
                        IImageSet imageSet = null;
                        if (place.StudyImageset != null)
                        {
                            imageSet = place.StudyImageset;
                        }
                        if (place.BackgroundImageSet != null)
                        {
                            imageSet = place.BackgroundImageSet;
                        }

                        if (imageSet != null)
                        {
                            this.CurrentImageSet = imageSet;
                        }
                        RenderEngine.GotoTarget(place, false, false, true);
                        return true;
                    }
                }
            }
            return false;
        }

        private void broadcastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IPlace place = (IPlace)contextMenuTargetObject;

                if (place.Tag is FitsImage)
                {
                    FitsImage image = (FitsImage)place.Tag;
                    Uri path = new Uri(image.Filename);

                    sampConnection.LoadImageFits(path.ToString(), image.Filename, place.Name);
                }
            }
            catch
            {


            }
        }

        private void imageStackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageStackVisible = !imageStackVisible;
            ShowImageStack();
        }

        private void earthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.StartUpLookAt = (int)ImageSetType.Earth;
        }

        private void planetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.StartUpLookAt = (int)ImageSetType.Planet;
        }

        private void skyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.StartUpLookAt = (int)ImageSetType.Sky;
        }

        private void panoramaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.StartUpLookAt = (int)ImageSetType.Panorama;
        }



        private void solarSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.StartUpLookAt = (int)ImageSetType.SolarSystem;
        }

        private void lastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.StartUpLookAt = 5;
        }
        private void randomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.StartUpLookAt = 6;
        }

        private void startupToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            earthToolStripMenuItem.Checked = Properties.Settings.Default.StartUpLookAt == (int)ImageSetType.Earth;
            planetToolStripMenuItem.Checked = Properties.Settings.Default.StartUpLookAt == (int)ImageSetType.Planet;
            skyToolStripMenuItem.Checked = Properties.Settings.Default.StartUpLookAt == (int)ImageSetType.Sky;
            panoramaToolStripMenuItem.Checked = Properties.Settings.Default.StartUpLookAt == (int)ImageSetType.Panorama;
            solarSystemToolStripMenuItem.Checked = Properties.Settings.Default.StartUpLookAt == (int)ImageSetType.SolarSystem;
            lastToolStripMenuItem.Checked = Properties.Settings.Default.StartUpLookAt == 5;
            randomToolStripMenuItem.Checked = Properties.Settings.Default.StartUpLookAt == 6;
        }

        private void musicAndOtherTourResourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebWindow.OpenUrl("http://www.worldwidetelescope.org/Download/TourAssets", true);

        }

        private void lookUpOnSDSSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string url = "http://cas.sdss.org/dr7/en/tools/quicklook/quickobj.asp?id={0}";
            url = String.Format(url, contextMenuTargetObject.Name.Replace("SDSS ", ""));

            if (contextMenuTargetObject.Name == "No Object")
            {
                url = String.Format("http://cas.sdss.org/dr7/en/tools/quicklook/quickobj.asp?ra={0}&dec={1}", contextMenuTargetObject.RA * 15, contextMenuTargetObject.Dec);
            }

            WebWindow.OpenUrl(url, false);

        }

        private void shapeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadLayerFile(false);
            ShowLayersWindows = true;
        }

        public static void LoadLayerFile(bool referenceFrameRightClick)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "All Data Files|*.wwtl;*.txt;*.csv;*.tdf;*.3ds;*.obj;*.shp;*.png;*.jpg;*.tle;*.glb|WorldWide Telescope Layer File(*.wwtl)|*.wwtl|Data Table(*.txt;*.csv;*.tdf)|*.txt;*.csv;*.tdf|ESRI Shape File(*.shp)|*.shp|3d Model(*.3ds;*.obj;*.gltfmodel)|*.3ds;*.obj;*.glb|Image Overlays (*.png;*.jpg)|*.png;*.jpg";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filename = openFile.FileName;

                try
                {
                    LayerManager.LoadLayer(filename, LayerManager.CurrentMap, true, referenceFrameRightClick);
                }
                catch
                {
                    MessageBox.Show(Language.GetLocalizedText(109, "This file does not seem to be a valid collection"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                }
            }
        }


        public static void LoadODATAFeed()
        {


        }

        private void Earth3d_Shown(object sender, EventArgs e)
        {
            if (config.MultiChannelDome1 | RenderEngine.ProjectorServer | config.MultiProjector | NoUi)
            {
                this.TopMost = true;
                this.Refresh();
                this.TopMost = false;
            }
        }

        private void showLayerManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowLayersWindows = !ShowLayersWindows;
        }

        private void uploadTourToCommunityToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void associateLiveIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowsLiveSignIn();
        }

        public async Task WindowsLiveSignIn()
        {
 
            await SignIn();


            menuTabs.Refresh();
            if (communitiesPane != null)
            {
                communitiesPane.ReloadFolder();
            }
        }

        private OAuthTicket Connection { get; set; }

        //object IAppSettings.this[string key]
        //{
        //    get
        //    {
        //        return Properties.Settings.Default[key];
        //    }
        //    set
        //    {
        //        Properties.Settings.Default[key] = value;
        //    }
        //}

        private async Task SignIn()
        {
            Connection = await OAuthAuthenticator.SignInToMicrosoftAccount(this);
            if (null != Connection)
            {
                WebClient wc = new WebClient();
                string profile = "";

                profile = wc.DownloadString("https://apis.live.net/v5.0//me?access_token=" + Connection.AccessToken);
                var user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserObject>(profile);
  
                //ready
             
                Properties.Settings.Default.LiveIdWWTId = user.Id;
                Properties.Settings.Default.LiveIdUser = user.Name;
                Properties.Settings.Default.LiveIdToken = Connection.AccessToken;

            }

        }

        public void WindowsLiveSignOut()
        {
            Properties.Settings.Default.LiveIdToken = null;
            Properties.Settings.Default.RefreshToken = null;
            menuTabs.Refresh();
            if (communitiesPane != null)
            {
                communitiesPane.ReloadFolder();
            }
        }


        private void OpenTourProperties()
        {
            if (tourEdit == null || tourEdit.Tour == null)
            {
                MessageBox.Show(Language.GetLocalizedText(-1, "There was a problem loading the tour..."));
            }
            else
            {
                if (string.IsNullOrEmpty(tourEdit.Tour.Author) ||
                    string.IsNullOrEmpty(tourEdit.Tour.AuthorEmail) ||
                    string.IsNullOrEmpty(tourEdit.Tour.Description) ||
                    tourEdit.Tour.AuthorImage == null)
                {
                    TourProperties tourProps = new TourProperties();
                    tourProps.EditTour = tourEdit.Tour;
                    tourProps.Strict = tourProps.highlightNeeded = tourProps.authorImageNeeded = true;
                    tourProps.highlightReqFields();
                    if (tourProps.ShowDialog() == DialogResult.OK)
                    {
                        Earth3d.MainWindow.Refresh();
                    }
                }
                else
                {
                    if (tourEdit.Tour.TourStops.Count == 0)
                    {
                        MessageBox.Show(Language.GetLocalizedText(-1, "Tour must have stops..."));
                    }
                }
            }
        }

        private Tour getTourFromCurrentTour(string filename)
        {
            if (tourEdit == null || tourEdit.Tour == null)
            {
                return null;
            }
            TourDocument editTour = tourEdit.Tour;
            if (string.IsNullOrEmpty(editTour.Author) || string.IsNullOrEmpty(editTour.AuthorEmail) ||
                string.IsNullOrEmpty(editTour.Description) || editTour.AuthorImage == null ||
                editTour.TourStops.Count == 0)
            {
                return null;
            }

            editTour.SaveToFile(filename);
            return new Tour()
            {
                // AttributesAndCredits = editTour.AttributesAndCredits,
                Author = editTour.Author,
                AuthorContactText = editTour.AuthorContactText,
                AuthorEmail = editTour.AuthorEmail,
                // AuthorEmailOther = editTour.AuthorEmailOther,
                AuthorImage = editTour.AuthorImage,
                AuthorImageUrl = "", // editTour.AuthorThumbnailFilename,
                AuthorUrl = editTour.AuthorUrl,
                AuthorURL = editTour.AuthorUrl,
                // AverageRating = editTour.AverageRating,
                // AverageUserRating = editTour.AverageUserRating,
                // Bounds = editTour.Bounds,
                // Children = editTour.Children,
                // Classification = editTour.Classification,
                Description = editTour.Description,
                Id = editTour.Id,
                ID = editTour.Id,
                Keywords = editTour.Keywords,
                LengthInSecs = editTour.RunTime.TotalSeconds,
                LengthInSeconds = editTour.RunTime.TotalSeconds,
                OrganizationName = editTour.OrgName,
                OrganizationUrl = editTour.OrganizationUrl,
                OrgName = editTour.OrgName,
                OrgUrl = editTour.OrgUrl,
                // RelatedTours = editTour.RelatedTours,
                // Taxonomy = editTour.Taxonomy,
                ThumbNail = editTour.TourStops[0].Thumbnail,
                ThumbnailUrl = "", // editTour.TourThumbnailFilename,
                Title = editTour.Title,
                // TourAttributionAndCredits = editTour.TourAttributionAndCredits,
                TourDuration = editTour.RunTime.ToString(),
                TourUrl = filename
                // UserLevel = editTour.UserLevel
            };
        }

        private void addToCommunityToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void communitiesMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            updateLoginCredentialsMenuItem.Visible = Properties.Settings.Default.AdvancedCommunities;
        }

        private void regionalDataCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SimpleInput sharedCache = new SimpleInput(Language.GetLocalizedText(660, "Shared Data Cache Settings"), Language.GetLocalizedText(661, "Shared Data Cache URL (leave empty for none)"), Properties.Settings.Default.SharedCacheServer, 1024);
            sharedCache.MinLength = 0;
            if (sharedCache.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.SharedCacheServer = sharedCache.ResultText;
                CacheProxy.BaseUrl = Properties.Settings.Default.SharedCacheServer;
            }
        }

        private void addAsNewLayerToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                IPlace place = (IPlace)contextMenuTargetObject;
                AddPlaceAsLayer(place, true);
            }
            catch
            {


            }
        }

        public void AddPlaceAsLayer(IPlace place, bool refresh)
        {
            IImageSet imageSet = null;
            if (place.StudyImageset != null)
            {
                imageSet = place.StudyImageset;
            }
            if (place.BackgroundImageSet != null)
            {
                imageSet = place.BackgroundImageSet;
            }

            if (imageSet != null)
            {
                LayerManager.AddImagesetLayer(imageSet);

            }
        }

        private void addCollectionAsTourStopsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void multiChanelCalibrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            TerraViewer.Callibration.Calibration calibrate = new TerraViewer.Callibration.Calibration();

            calibrate.Show();
        }


        public void ShowCalibrationScreen()
        {


            if (InvokeRequired)
            {
                MethodInvoker ShowMeCall = delegate
                {
                    TerraViewer.Callibration.CalibrationScreen.ShowWindow();
                };
                try
                {
                    Invoke(ShowMeCall);
                }
                catch
                {
                }
            }
            else
            {
                TerraViewer.Callibration.CalibrationScreen.ShowWindow();
            }
        }

        private void sendLayersToProjectorServersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveLayerSyncFile();
            string command = "SYNCLAYERS," + Earth3d.MainWindow.Config.ClusterID.ToString() + ",-1";
            NetControl.SendCommand(command);
        }

        private static void SaveLayerSyncFile()
        {
            LayerContainer layers = new LayerContainer();
            layers.SaveToFile(Properties.Settings.Default.CahceDirectory + "\\layerSync.layers");
            layers.Dispose();
            GC.SuppressFinalize(layers);
        }

        private void sendTourToProjectorServersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SyncTour();
        }

        private void SyncTour()
        {
            if (TourEdit != null)
            {
                if (TourEdit.Tour != null)
                {
                    TourEdit.Tour.SaveToFile(Properties.Settings.Default.CahceDirectory + "\\tourSync.wtt", true, true);

                    string command = "SYNCTOUR," + Earth3d.MainWindow.Config.ClusterID.ToString() + ",-1";
                    NetControl.SendCommand(command);
                }
            }
        }

        private void automaticTourSyncWithProjectorServersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoSyncTours = !Properties.Settings.Default.AutoSyncTours;
            automaticTourSyncWithProjectorServersToolStripMenuItem.Checked = Properties.Settings.Default.AutoSyncTours;
        }

        public VideoOutputType dumpFrameParams = new VideoOutputType();

        RenderProgress RenderProgress = null;
        private void renderToVideoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OutputTourToVideo videoDialog = new OutputTourToVideo();
            videoDialog.Target = this.menuTabs.CurrentTour;


            if (TourEdit != null)
            {
                if (videoDialog.ShowDialog() == DialogResult.OK)
                {
                    SpaceTimeController.MetaNow = DateTime.Now;
                    SpaceTimeController.FrameDumping = true;
                    SpaceTimeController.FramesPerSecond = videoDialog.RenderValues.Fps;
                    SpaceTimeController.TotalFrames = videoDialog.RenderValues.TotalFrames;
                    SpaceTimeController.CurrentFrameNumber = videoDialog.RenderValues.StartFrame;
                    dumpFrameParams = videoDialog.RenderValues;
                    RenderEngine.dumpFrameParams = dumpFrameParams;
                    RenderEngine.CaptureVideo = true;
                    RenderProgress = new RenderProgress();
                    RenderProgress.Owner = this;
                    RenderProgress.Show();
                    SpaceTimeController.CancelFrameDump = false;
                    TourEdit.PlayNow(false);
                }

            }
        }

        Texture11 touchControl = null;
        Texture11 touchControlNoHold = null;
        Texture11 trackerButton = null;
        bool kioskControl = false;
        PositionColoredTextured[] TouchControlPoints = new PositionColoredTextured[4];
        public void DrawTouchControls()
        {

            if (Properties.Settings.Default.ShowTouchControls && !TourEditor.Capturing && !RenderEngine.CaptureVideo)
            {

                MakeTouchPoints();

                if (trackerButton == null)
                {
                    trackerButton = Planets.LoadPlanetTexture(Properties.Resources.TrackerButton);
                }

                if (touchControl == null)
                {
                    string appdir = Path.GetDirectoryName(Application.ExecutablePath);
                    string customImageFile = appdir + "\\TouchControls.png";
                    string customImageFileNoHold = appdir + "\\TouchControlsNoHold.png";
                    if (File.Exists(customImageFile))
                    {
                        Bitmap bmp = new Bitmap(customImageFile);
                        touchControl = Planets.LoadPlanetTexture(bmp); 
                        bmp.Dispose();
                    }
                    else
                    {
                        touchControl = Planets.LoadPlanetTexture(Properties.Resources.TouchControls);
                    }

                    if (File.Exists(customImageFileNoHold))
                    {
                        Bitmap bmp = new Bitmap(customImageFileNoHold);
                        touchControlNoHold = Planets.LoadPlanetTexture(bmp); 
                        bmp.Dispose();
                    }
                    else
                    {
                        touchControlNoHold = Planets.LoadPlanetTexture(Properties.Resources.TouchControlsNoHold);
                    }

                }

                float x = RenderContext11.ViewPort.Width - 207;
                float y = RenderContext11.ViewPort.Height - (234 + 120);
                float w = 197;
                float h = 224;

                TouchControlPoints[0].X = x;
                TouchControlPoints[0].Y = y;
                TouchControlPoints[0].Z = .9f;
                TouchControlPoints[0].W = 1;
                TouchControlPoints[0].Tu = 0;
                TouchControlPoints[0].Tv = 0;
                TouchControlPoints[0].Color = Color.FromArgb(64, 255, 255, 255);

                TouchControlPoints[1].X = (float)(x + w);
                TouchControlPoints[1].Y = (float)(y);
                TouchControlPoints[1].Tu = 1;
                TouchControlPoints[1].Tv = 0;
                TouchControlPoints[1].Color = Color.FromArgb(64, 255, 255, 255);
                TouchControlPoints[1].Z = .9f;
                TouchControlPoints[1].W = 1;

                TouchControlPoints[2].X = (float)(x);
                TouchControlPoints[2].Y = (float)(y + h);
                TouchControlPoints[2].Tu = 0;
                TouchControlPoints[2].Tv = 1;
                TouchControlPoints[2].Color = Color.FromArgb(64, 255, 255, 255);
                TouchControlPoints[2].Z = .9f;
                TouchControlPoints[2].W = 1;

                TouchControlPoints[3].X = (float)(x + w);
                TouchControlPoints[3].Y = (float)(y + h);
                TouchControlPoints[3].Tu = 1;
                TouchControlPoints[3].Tv = 1;
                TouchControlPoints[3].Color = Color.FromArgb(64, 255, 255, 255);
                TouchControlPoints[3].Z = .9f;
                TouchControlPoints[3].W = 1;

                if (Friction)
                {
                    Sprite2d.DrawForScreen(RenderContext11, TouchControlPoints, 4, touchControlNoHold, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);
                }
                else
                {
                    Sprite2d.DrawForScreen(RenderContext11, TouchControlPoints, 4, touchControl, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);
                }

                if (!kioskControl)
                {
                    if (Friction && activeTouch == TouchControls.None)
                    {
                        float frictionFactor = (float)(1 - (RenderEngine.lastFrameTime / 2));
                        moveVector.X *= frictionFactor;
                        moveVector.Y *= frictionFactor;
                        OrbitVector *= frictionFactor;
                        ZoomVector *= frictionFactor;
                    }


                    // Calculate Arc for Zoom
                    Vector2d zoomArc = new Vector2d(zoomTracker.X - ZoomVector, (zoomTracker.Y - Math.Cos(ZoomVector / 54 * Math.PI / 2) * 20) + 39);
                    touchPoints[(int)TouchControls.ZoomTrack] = zoomArc;

                    // Calculate Arc for Orbit
                    Vector2d orbitArc = new Vector2d(99 - (Math.Sin(OrbitVector / 70 * Math.PI / 2) * 68), 113 + (Math.Cos(OrbitVector / 70 * Math.PI / 2) * 75));
                    touchPoints[(int)TouchControls.OrbitTrack] = orbitArc;

                    // Calculate Current Pan Position
                    Vector2d panPos = new Vector2d(panTracker.X - moveVector.X, panTracker.Y - moveVector.Y);

                    touchPoints[(int)TouchControls.PanTrack] = panPos;

                    Sprite2d.Draw2D(RenderContext11, trackerButton, new SizeF(32, 32), new PointF(15, 15), 0, new PointF((float)touchPoints[(int)TouchControls.PanTrack].X + x, (float)touchPoints[(int)TouchControls.PanTrack].Y + y), Color.FromArgb(128, 255, 255, 255));
                    Sprite2d.Draw2D(RenderContext11, trackerButton, new SizeF(32, 32), new PointF(15, 15), 0, new PointF((float)touchPoints[(int)TouchControls.OrbitTrack].X + x, (float)touchPoints[(int)TouchControls.OrbitTrack].Y + y), Color.FromArgb(128, 255, 255, 255));
                    Sprite2d.Draw2D(RenderContext11, trackerButton, new SizeF(32, 32), new PointF(15, 15), 0, new PointF((float)touchPoints[(int)TouchControls.ZoomTrack].X + x, (float)touchPoints[(int)TouchControls.ZoomTrack].Y + y), Color.FromArgb(128, 255, 255, 255));
                }
            }
        }

        double kinectListOffsetTarget = 0;
        double kinectPickValue = 0;

        Folder kinectUi = null;

        bool kinectInit = false;
        public void DrawKinectUI()
        {
            int index = 0;
            int itemStride = 600;

            if (kinectUi == null && !kinectInit)
            {
                kinectInit = true;

                try
                {
                   kinectUi = Folder.LoadFromUrl("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?w=kinect", false);
                }
                catch
                {
                }
            }

            if (kinectUi != null && menu)
            {
                int currentId = Math.Abs((int)((kinectListOffsetTarget - itemStride / 2) / itemStride));
                Place currentPlace = null;
                Place earthPlace = null;
                double gap = ((int)((kinectListOffsetTarget - itemStride / 2) / itemStride) * itemStride) - kinectListOffsetTarget;

                if (Math.Abs(gap) > .5 && !contact)
                {
                    kinectListOffsetTarget += Math.Min(10, Math.Abs(gap)) * Math.Sign(gap);
                }

                foreach (object item in kinectUi.Children)
                {
                    Place place = item as Place;

                    if (place != null && place.Classification == Classification.SolarSystem)
                    {
                        int id = Planets.GetPlanetIDFromName(place.Name);
                        if (place.Name == "Solar Eclipse 2017")
                        {
                            id = 18;
                        }

                        if (place.Name == "Earth")
                        {
                            earthPlace = place;
                        }

                        if (id > -1)
                        {
                            if (index == currentId)
                            {
                                currentPlace = place;
                            }
                            Texture11 tex = Planets.PlanetTextures[id];

                            if (tex != null)
                            {

                                if (id > 9 && id < 18)
                                {
                                    Sprite2d.Draw2D(
                                      RenderContext11,
                                      tex,
                                      new SizeF(512, 512),
                                      new PointF(.5f, .5f),
                                      0,
                                      new PointF(
                                          (RenderContext11.ViewPort.Width / 2) +
                                          (index * itemStride + (int)kinectListOffsetTarget),
                                          RenderContext11.ViewPort.Height / 2
                                          ),
                                      Color.White
                                      );
                                 }
                                else if (id > 0)// && id != 18)
                                {
                                    Sprite2d.Draw2D(
                                       RenderContext11,
                                       tex,
                                       new SizeF(512, 512),
                                       new PointF(.5f, .5f),
                                       0,
                                       new PointF(
                                           (RenderContext11.ViewPort.Width / 2) +
                                           (index * itemStride + (int)kinectListOffsetTarget),
                                           RenderContext11.ViewPort.Height / 2
                                           ),
                                       Color.White
                                       );

                                }
                                else
                                {
                                    Sprite2d.Draw2D(
                                        RenderContext11,
                                        tex,
                                        new SizeF(1024, 1024),
                                        new PointF(.5f, .5f),
                                        0,
                                        new PointF(
                                            (RenderContext11.ViewPort.Width / 2) +
                                            (index * itemStride + (int)kinectListOffsetTarget),
                                            RenderContext11.ViewPort.Height / 2
                                            ),
                                        Color.White
                                        );
                                }
                            }
                            index++;
                        }
                    }
                }



                if (showNextObject)
                {
                    if (currentPlace != null)
                    {
                        if (currentPlace.Name == "Solar Eclipse 2017")
                        {
                            kinectEclipseMode = true;
                            SpaceTimeController.Now = new DateTime(2017, 08, 21, 16, 00, 00);
                            SpaceTimeController.TimeRate = 200;
                            SpaceTimeController.SyncToClock = true;
                            RenderEngine.GotoTarget(earthPlace, false, false, true);
                        }
                        else
                        {
                            kinectEclipseMode = false;
                            RenderEngine.GotoTarget(currentPlace, false, false, true);
                        }
                    }
                    showNextObject = false;
                    menu = false;
                }
            }
        }
        bool kinectEclipseMode = false;
        public bool NoShowTourEndPage = false;


        private void showTouchControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Properties.Settings.Default.ShowTouchControls = !Properties.Settings.Default.ShowTouchControls;
        }



        public string FocusReferenceFrame()
        {

            return CurrentImageSet.ReferenceFrame;

        }

        private void remoteAccessControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoteAccessControl wal = new RemoteAccessControl();

            wal.ShowDialog();
        }



        private void layerManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowLayersWindows = true;
        }


        private void screenBroadcastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScreenBroadcast sb = new ScreenBroadcast();
            sb.Show();
        }

        private void monochromeStyleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.MonochromeImageStyle = !Properties.Settings.Default.MonochromeImageStyle;
            monochromeStyleToolStripMenuItem.Checked = Properties.Settings.Default.MonochromeImageStyle;
            Tile.GrayscaleStyle = Properties.Settings.Default.MonochromeImageStyle;
            TileCache.PurgeQueue();
            TileCache.ClearCache();
        }

        private void layersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Language.GetLocalizedText(976, "WorldWide Telescope Layer File(*.wwtl)|*.wwtl");
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filename = openFile.FileName;

                try
                {
                    LayerManager.LoadLayerFile(filename, LayerManager.CurrentMap, false);
                }
                catch
                {
                    MessageBox.Show(Language.GetLocalizedText(109, "This file does not seem to be a valid collection"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                }
            }
        }

        private void publishTourToCommunityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Earth3d.IsLoggedIn)
            {
                string filename = Path.GetTempFileName();
                TourEdit.Tour.SaveToFile(filename, true, false);
                EOCalls.InvokePublishFile(filename, TourEdit.Tour.Title + ".wtt");
                RefreshCommunityLocal();
            }
        }


        public static void RefreshCommunity()
        {
            MethodInvoker doIt = delegate
            {
                Earth3d.MainWindow.RefreshCommunityLocal();
            };

            if (Earth3d.MainWindow.InvokeRequired)
            {
                try
                {
                    Earth3d.MainWindow.Invoke(doIt);
                }
                catch
                {
                }
            }
            else
            {
                doIt();
            }
        }

        public void RefreshCommunityLocal()
        {
            if (communitiesPane != null)
            {
                communitiesPane.ReloadFolder();
            }
        }

        private void findEarthBasedLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LocationSearch locationSearch = new LocationSearch();
            if (searchPane != null)
            {
                locationSearch.ObejctName = searchPane.SearchStringText;
            }
            if (locationSearch.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (SolarSystemMode)
                {
                    Vector3d pnt = Coordinates.GeoTo3dDouble(locationSearch.Result.Lat, locationSearch.Result.Lng);

                    pnt = Vector3d.TransformCoordinate(pnt, Planets.EarthMatrix);
                    pnt.Normalize();
                    Vector2d radec = Coordinates.CartesianToLatLng(pnt);


                    RenderEngine.TargetLat = radec.Y;
                    RenderEngine.TargetLong = radec.X - 90;
                }
                else
                {
                    RenderEngine.GotoTarget(locationSearch.Result, false, false, false);
                }
            }
        }

        private void fullDomePreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DomePreviewPopup domePopup = new DomePreviewPopup();
            domePopup.Show();
        }

        private void xBoxControllerSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            XBoxConfig.ShowSetupWindow();
        }

        private void DeviceHeartbeat_Tick(object sender, EventArgs e)
        {
            MidiMapManager.Heartbeat();
        }

        private void mIDIControllerSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MidiSetup.ShowSetupWindow();
        }

        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MultiSampling != 1)
            {
                Properties.Settings.Default.MultiSampling = 1;
                RestartNow();
            }
        }

        private void fourSamplesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MultiSampling != 4)
            {
                Properties.Settings.Default.MultiSampling = 4;
                RestartNow();
            }
        }

        private void eightSamplesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.MultiSampling != 8)
            {
                Properties.Settings.Default.MultiSampling = 8;
                RestartNow();
            }
        }

        public void RestartNow()
        {
            if (UiTools.ShowMessageBox(Language.GetLocalizedText(1043, "WorldWide Telescope must restart for new settings to take effect. Do you want to restart now?"), Language.GetLocalizedText(694, "Restart Now?"), MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                LanguageReboot = true;
                this.Close();
            }
        }

        private void multiSampleAntialiasingToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            switch (Properties.Settings.Default.MultiSampling)
            {
                case 4:
                    noneToolStripMenuItem.Checked = false;
                    fourSamplesToolStripMenuItem.Checked = true;
                    eightSamplesToolStripMenuItem.Checked = false;
                    break;
                case 8:
                    noneToolStripMenuItem.Checked = false;
                    fourSamplesToolStripMenuItem.Checked = false;
                    eightSamplesToolStripMenuItem.Checked = true;
                    break;
                default:
                    noneToolStripMenuItem.Checked = true;
                    fourSamplesToolStripMenuItem.Checked = false;
                    eightSamplesToolStripMenuItem.Checked = false;
                    break;
            }
        }



        #region IScriptable Members
        // Ra, Declinations, Latitude, Longitude, Zoom, Angle, Rotation, ImageCrossfade, FadeToBlack
        ScriptableProperty[] IScriptable.GetProperties()
        {
            List<ScriptableProperty> props = new List<ScriptableProperty>();

            props.Add(new ScriptableProperty("Ra", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, 0, 24, false));
            props.Add(new ScriptableProperty("Declination", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -90, +90, false));
            props.Add(new ScriptableProperty("Latitude", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -90, 90, false));
            props.Add(new ScriptableProperty("Longitude", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -180, 180, false));
            props.Add(new ScriptableProperty("Zoom", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Log, RenderEngine.ZoomMin, RenderEngine.ZoomMax, false));
            props.Add(new ScriptableProperty("Angle", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -Math.PI / 2, 0, false));
            props.Add(new ScriptableProperty("Rotation", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -3.14, 3.14, false));
            props.Add(new ScriptableProperty("ZoomRate", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -54, 54, false));
            props.Add(new ScriptableProperty("PanUpDownRate", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -50, 50, false));
            props.Add(new ScriptableProperty("PanLeftRightRate", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -50, 50, false));
            props.Add(new ScriptableProperty("RotationRate", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -70, 70, false));
            props.Add(new ScriptableProperty("ImageCrossfade", ScriptablePropertyTypes.BlendState, ScriptablePropertyScale.Linear, 0, 100, true));
            props.Add(new ScriptableProperty("FadeToBlack", ScriptablePropertyTypes.BlendState, ScriptablePropertyScale.Linear, 0, 1, true));
            props.Add(new ScriptableProperty("SystemVolume", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, 0, 1, false));
            props.Add(new ScriptableProperty("DomeTilt", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, 0, 90, false));
            props.Add(new ScriptableProperty("DomeAngle", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -180, 180, false));
            props.Add(new ScriptableProperty("DomeAlt", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, 0, 90, false));
            props.Add(new ScriptableProperty("DomeAz", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, -180, 180, false));
            props.Add(new ScriptableProperty("FisheyeAngle", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, 0, 270, false));
            props.Add(new ScriptableProperty("NavigationHold", ScriptablePropertyTypes.Bool, ScriptablePropertyScale.Linear, 0, 1, true));
            props.Add(new ScriptableProperty("ScreenFOV", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Linear, 1, 89, false));
            return props.ToArray();
        }

        string[] IScriptable.GetActions()
        {
            return Enum.GetNames(typeof(NavigationActions));
        }

        void IScriptable.InvokeAction(string name, string value)
        {
            MethodInvoker updatePlace = delegate
            {
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }
                try
                {
                    NavigationActions action = (NavigationActions)Enum.Parse(typeof(NavigationActions), name, true);

                    switch (action)
                    {
                        case NavigationActions.ResetRiftView:
                            if (RenderEngine.rift)
                            {
                              //  ResetRift();
                            }
                            break;
                        case NavigationActions.AllStop:
                            TouchAllStop();
                            break;
                        case NavigationActions.MoveUp:
                            if (!string.IsNullOrEmpty(value))
                            {
                                MoveView(0, double.Parse(value) * 10, false);
                            }
                            else
                            {
                                MoveUp();
                            }
                            break;
                        case NavigationActions.MoveDown:
                            if (!string.IsNullOrEmpty(value))
                            {
                                MoveView(0, -double.Parse(value) * 10, false);
                            }
                            else
                            {
                                MoveDown();
                            }

                            break;
                        case NavigationActions.MoveRight:
                            if (!string.IsNullOrEmpty(value))
                            {
                                MoveView(double.Parse(value) * 10, 0, false);
                            }
                            else
                            {
                                MoveRight();
                            }
                            break;
                        case NavigationActions.MoveLeft:
                            if (!string.IsNullOrEmpty(value))
                            {
                                MoveView(-double.Parse(value) * 10, 0, false);
                            }
                            else
                            {
                                MoveLeft();
                            }
                            break;
                        case NavigationActions.ZoomIn:
                            if (!string.IsNullOrEmpty(value))
                            {
                                ZoomIn(double.Parse(value));
                            }
                            else
                            {
                                ZoomIn();
                            }
                            break;
                        case NavigationActions.ZoomOut:
                            if (!string.IsNullOrEmpty(value))
                            {
                                ZoomOut(double.Parse(value));
                             }
                            else
                            {
                                ZoomOut();
                            }
                            break;
                        case NavigationActions.RotateLeft:
                            if (!string.IsNullOrEmpty(value))
                            {
                                RotateLeft(double.Parse(value));
                            }
                            else
                            {
                                RotateLeft(1);
                            }
                            break;
                        case NavigationActions.RotateRight:
                            if (!string.IsNullOrEmpty(value))
                            {
                                RotateRight(double.Parse(value));
                            }
                            else
                            {
                                RotateRight(1);
                            }
                            break;
                        case NavigationActions.DomeLeft:
                            if (!string.IsNullOrEmpty(value))
                            {
                                DomeLeft(double.Parse(value));
                            }
                            else
                            {
                                DomeLeft(1);
                            }
                            break;

                        case NavigationActions.DomeRight:
                            if (!string.IsNullOrEmpty(value))
                            {
                                DomeRight(double.Parse(value));
                            }
                            else
                            {
                                DomeRight(1);
                            }
                            break;

                        case NavigationActions.DomeUp:
                            if (!string.IsNullOrEmpty(value))
                            {
                                DomeUp(double.Parse(value));
                            }
                            else
                            {
                                DomeUp(1);
                            }
                            break;

                        case NavigationActions.DomeDown:
                            if (!string.IsNullOrEmpty(value))
                            {
                                DomeUp(double.Parse(value));
                            }
                            else
                            {
                                DomeUp(1);
                            }
                            break;


                        case NavigationActions.TiltUp:
                            if (!string.IsNullOrEmpty(value))
                            {
                                TiltUp(double.Parse(value));
                            }
                            else
                            {
                                TiltUp(1);
                            }
                            break;
                        case NavigationActions.TiltDown:
                            if (!string.IsNullOrEmpty(value))
                            {
                                TiltDown(double.Parse(value));
                            }
                            else
                            {
                                TiltDown(1);
                            }
                            break;
                        case NavigationActions.ResetCamera:
                            ResetCamera();
                            break;
                        case NavigationActions.NextItem:
                            explorePane.MoveNext();
                            break;
                        case NavigationActions.LastItem:
                            explorePane.MovePrevious();
                            break;
                        case NavigationActions.Select:
                            explorePane.SelectItem();
                            break;
                        case NavigationActions.Back:
                            explorePane.Back();
                            break;
                        case NavigationActions.ShowNextContext:
                            contextPanel.ShowNextObject();
                            break;
                        case NavigationActions.ShowPreviousContext:
                            contextPanel.ShowPreviousObject();
                            break;
                        case NavigationActions.ShowNextExplore:
                            explorePane.MoveNext();
                            explorePane.ShowCurrent();
                            break;
                        case NavigationActions.ShowPreviousExplore:
                            explorePane.MovePrevious();
                            explorePane.ShowCurrent();
                            break;
                        case NavigationActions.SetForeground:
                            explorePane.SelectForeground();
                            break;
                        case NavigationActions.SetBackground:
                            explorePane.SelectBackground();
                            break;
                        case NavigationActions.PreviousMode:
                            PreviousMode();
                            break;
                        case NavigationActions.NextMode:
                            NextMode();
                            break;
                        case NavigationActions.SolarSystemMode:
                            {
                                ImageSetType lookAt = ImageSetType.SolarSystem;
                                contextPanel.SetLookAtTarget(lookAt);
                            }
                            break;
                        case NavigationActions.SkyMode:
                            {
                                ImageSetType lookAt = ImageSetType.Sky;
                                contextPanel.SetLookAtTarget(lookAt);
                            }
                            break;
                        case NavigationActions.EarthMode:
                            {
                                ImageSetType lookAt = ImageSetType.Earth;
                                contextPanel.SetLookAtTarget(lookAt);
                            }
                            break;
                        case NavigationActions.PlanetMode:
                            {
                                ImageSetType lookAt = ImageSetType.Planet;
                                contextPanel.SetLookAtTarget(lookAt);
                            }
                            break;
                        case NavigationActions.PanoramaMode:
                            {
                                ImageSetType lookAt = ImageSetType.Panorama;
                                contextPanel.SetLookAtTarget(lookAt);
                            }
                            break;
                        case NavigationActions.SandboxMode:
                            {
                                ImageSetType lookAt = ImageSetType.Sandbox;
                                contextPanel.SetLookAtTarget(lookAt);
                            }
                            break;
                        case NavigationActions.PlayTour:
                            {
                                if (tourEdit != null)
                                {
                                    tourEdit.PlayNow(false);
                                }
                            }
                            break;
                        case NavigationActions.PauseTour:
                            {
                                if (tourEdit != null)
                                {
                                    tourEdit.PauseTour();
                                }
                            }
                            break;
                        case NavigationActions.StopTour:
                            {
                                if (tourEdit != null)
                                {
                                    tourEdit.PauseTour();
                                    CloseTour(false);
                                }
                            }
                            break;
                        case NavigationActions.NextSlide:
                            {
                                if (TourPlayer.Playing)
                                {
                                    TourPlayer player = uiController as TourPlayer;
                                    if (player != null)
                                    {
                                        player.MoveNextSlide();
                                    }

                                }
                            }
                            break;
                        case NavigationActions.GotoSlide:
                            if (!string.IsNullOrEmpty(value))
                            {
                                int slideID = -1;
                                int.TryParse(value, out slideID);

                                if (TourPlayer.Playing && slideID > -1)
                                {
                                    TourPlayer player = uiController as TourPlayer;
                                    if (player != null)
                                    {
                                        player.MoveToSlide(slideID);
                                    }

                                }
                            }
                            break;
                        case NavigationActions.PreviousSlide:
                            {
                                if (TourPlayer.Playing)
                                {
                                    TourPlayer player = uiController as TourPlayer;
                                    if (player != null)
                                    {
                                        player.MovePreviousSlide();
                                    }

                                }
                            }
                            break;
                        case NavigationActions.MoveToEndSlide:
                            {
                                if (TourPlayer.Playing)
                                {
                                    TourPlayer player = uiController as TourPlayer;
                                    if (player != null)
                                    {
                                        player.MoveToEndSlide();
                                    }

                                }
                            }
                            break;
                        case NavigationActions.MoveToFirstSlide:
                            {
                                if (TourPlayer.Playing)
                                {
                                    TourPlayer player = uiController as TourPlayer;
                                    if (player != null)
                                    {
                                        player.MoveToEndSlide();
                                    }

                                }
                            }
                            break;
                        case NavigationActions.GotoSun:
                            {
                                IPlace place = Catalogs.FindCatalogObjectExact("Sun");
                                RenderEngine.GotoTarget(place, false, false, true);

                            }
                            break;
                        case NavigationActions.GotoMercury:
                            {
                                IPlace place = Catalogs.FindCatalogObjectExact("Mercury");
                                RenderEngine.GotoTarget(place, false, false, true);

                            }
                            break;
                        case NavigationActions.GotoVenus:
                            {
                                IPlace place = Catalogs.FindCatalogObjectExact("Venus");
                                RenderEngine.GotoTarget(place, false, false, true);

                            }
                            break;
                        case NavigationActions.GotoEarth:
                            {
                                IPlace place = Catalogs.FindCatalogObjectExact("Earth");
                                RenderEngine.GotoTarget(place, false, false, true);

                            }
                            break;
                        case NavigationActions.GotoMars:
                            {
                                IPlace place = Catalogs.FindCatalogObjectExact("Mars");
                                RenderEngine.GotoTarget(place, false, false, true);

                            }
                            break;
                        case NavigationActions.GotoJupiter:
                            {
                                IPlace place = Catalogs.FindCatalogObjectExact("Jupiter");
                                RenderEngine.GotoTarget(place, false, false, true);

                            }
                            break;
                        case NavigationActions.GotoSaturn:
                            {
                                IPlace place = Catalogs.FindCatalogObjectExact("Saturn");
                                RenderEngine.GotoTarget(place, false, false, true);

                            }
                            break;
                        case NavigationActions.GotoUranus:
                            {
                                IPlace place = Catalogs.FindCatalogObjectExact("Uranus");
                                RenderEngine.GotoTarget(place, false, false, true);

                            }
                            break;
                        case NavigationActions.GotoNeptune:
                            {
                                IPlace place = Catalogs.FindCatalogObjectExact("Neptune");
                                RenderEngine.GotoTarget(place, false, false, true);

                            }
                            break;
                        case NavigationActions.GotoPluto:
                            {
                                IPlace place = Catalogs.FindCatalogObjectExact("Pluto");
                                RenderEngine.GotoTarget(place, false, false, true);

                            }
                            break;
                        case NavigationActions.SolarSystemOverview:
                            {
                                CameraParameters cameraParams = new CameraParameters(45, 45, 300, 0, 0, 100);

                                RenderEngine.GotoTarget(cameraParams, false, false);

                            }
                            break;
                        case NavigationActions.GotoMilkyWay:
                            {
                                CameraParameters cameraParams = new CameraParameters(45, 45, 10000000000, 0, 0, 100);

                                RenderEngine.GotoTarget(cameraParams, false, false);

                            }
                            break;
                        case NavigationActions.GotoSDSSGalaxies:
                            {
                                CameraParameters cameraParams = new CameraParameters(45, 45, 300000000000000, 0, 0, 100);

                                RenderEngine.GotoTarget(cameraParams, false, false);

                            }
                            break;

                        default:
                            break;

                    }
                }
                catch
                {
                }
            };
            try
            {
                Invoke(updatePlace);
            }
            catch
            {
            }
        }

        void IScriptable.SetProperty(string name, string value)
        {
            try
            {
                NavigationProperties prop = (NavigationProperties)Enum.Parse(typeof(NavigationProperties), name, true);

                switch (prop)
                {
                    case NavigationProperties.Ra:
                        RenderEngine.TargetLong = RAtoViewLng(double.Parse(value));
                        break;
                    case NavigationProperties.Declination:
                        RenderEngine.TargetLat = double.Parse(value);
                        break;
                    case NavigationProperties.Latitude:
                        RenderEngine.TargetLat = double.Parse(value);
                        break;
                    case NavigationProperties.Longitude:
                        RenderEngine.TargetLong = double.Parse(value);
                        break;
                    case NavigationProperties.Zoom:
                        break;
                    case NavigationProperties.Angle:
                        {
                            double val = double.Parse(value);
                            RenderEngine.targetViewCamera.Angle = val;
                        }
                        break;
                    case NavigationProperties.Rotation:
                        {
                            double val = double.Parse(value);
                            RenderEngine.targetViewCamera.Rotation = val;
                        }
                        break;
                    case NavigationProperties.ZoomRate:
                        {
                            double val = double.Parse(value);
                            ZoomVector = (float)val;
                            timer.Enabled = true;
                        }
                        break;
                    case NavigationProperties.PanUpDownRate:
                        {
                            double val = double.Parse(value);
                            moveVector.Y = (float)val;
                            timer.Enabled = true;
                        }
                        break;
                    case NavigationProperties.PanLeftRightRate:
                        {
                            double val = double.Parse(value);
                            moveVector.X = (float)val;
                            timer.Enabled = true;
                        }
                        break;
                    case NavigationProperties.RotationRate:
                        {
                            double val = double.Parse(value);
                            OrbitVector = (float)val;
                            timer.Enabled = true;
                        }
                        break;


                    case NavigationProperties.DomeAlt:
                        {
                            double val = double.Parse(value);
                            RenderEngine.viewCamera.DomeAlt = val;
                        }
                        break;

                    case NavigationProperties.DomeAz:
                        {
                            double val = double.Parse(value);
                            RenderEngine.viewCamera.DomeAz = val;
                        }
                        break;

                    case NavigationProperties.DomeTilt:
                        {
                            double val = double.Parse(value);
                            Earth3d.MainWindow.Config.DomeTilt = (float)val;
                        }
                        break;

                    case NavigationProperties.DomeAngle:
                        {
                            double val = double.Parse(value);
                            Earth3d.MainWindow.Config.DomeAngle = (float)val;
                        }
                        break;

                    case NavigationProperties.FisheyeAngle:
                        {
                            double val = double.Parse(value);
                            Properties.Settings.Default.FisheyeAngle = (float)val;
                        }
                        break;
                    case NavigationProperties.ImageCrossfade:
                        {
                            double val = double.Parse(value);
                            RenderEngine.StudyOpacity = (float)(val);

                        }
                        break;
                    case NavigationProperties.FadeToBlack:
                        {
                            double val = double.Parse(value);
                            RenderEngine.Fader.Opacity = 1f - (float)(val);
                        }
                        break;
                    case NavigationProperties.SystemVolume:
                        {
                            double val = double.Parse(value);
                            UiTools.SetSystemVolume((float)val);
                        }
                        break;

                    case NavigationProperties.NavigationHold:
                        {
                            double val = double.Parse(value);
                            Friction = val != 0;
                        }
                        break;
                    case NavigationProperties.ScreenFOV:
                        {
                            double val = double.Parse(value);
                            RenderEngine.fovLocal = (float)val / 180 * Math.PI;
                        }
                        break;

                    default:
                        break;
                }
            }
            catch
            {
            }
        }

        string IScriptable.GetProperty(string name)
        {
            try
            {
                NavigationProperties prop = (NavigationProperties)Enum.Parse(typeof(NavigationProperties), name, true);
                switch (prop)
                {
                    //todo Fix maps for Get properties to match set
                    case NavigationProperties.ImageCrossfade:
                        {
                            if (RenderEngine.StudyOpacity > 0)
                            {
                                RenderEngine.StudyOpacity = 100;
                                return true.ToString();
                            }
                            else
                            {
                                RenderEngine.StudyOpacity = 0;
                                return false.ToString();
                            }
                        }

                    case NavigationProperties.FadeToBlack:
                        {

                            return RenderEngine.Fader.TargetState.ToString();
                        }
                    case NavigationProperties.SystemVolume:
                        {
                            return UiTools.GetSystemVolume().ToString();
                        }

                    case NavigationProperties.NavigationHold:
                        {
                            return Friction ? "1" : "0";
                        }

                    case NavigationProperties.ScreenFOV:
                        return (RenderEngine.fovLocal / Math.PI * 180).ToString();

                    case NavigationProperties.Ra:
                        return RenderEngine.RA.ToString();

                    case NavigationProperties.Declination:
                        return RenderEngine.TargetLat.ToString();

                    case NavigationProperties.Latitude:
                        return RenderEngine.TargetLat.ToString();

                    case NavigationProperties.Longitude:
                        return RenderEngine.TargetLong.ToString();

                    case NavigationProperties.Zoom:
                        return RenderEngine.ZoomFactor.ToString();

                    case NavigationProperties.Angle:
                        {
                            return RenderEngine.targetViewCamera.Angle.ToString();
                        }

                    case NavigationProperties.Rotation:
                        {
                            return RenderEngine.targetViewCamera.Rotation.ToString();
                        }


                    case NavigationProperties.DomeAlt:
                        {
                            return RenderEngine.viewCamera.DomeAlt.ToString();
                        }


                    case NavigationProperties.DomeAz:
                        {
                            return RenderEngine.viewCamera.DomeAz.ToString();
                        }

                    case NavigationProperties.DomeTilt:
                        {
                            return Config.DomeTilt.ToString();
                        }


                    case NavigationProperties.DomeAngle:
                        {
                            return Config.DomeAngle.ToString();
                        }


                    case NavigationProperties.FisheyeAngle:
                        {
                            return Properties.Settings.Default.FisheyeAngle.ToString();
                        }


                    default:
                        return "0";

                }
            }
            catch
            {
                return "0";
            }
        }

        bool IScriptable.ToggleProperty(string name)
        {
            NavigationProperties prop = (NavigationProperties)Enum.Parse(typeof(NavigationProperties), name);

            switch (prop)
            {

                case NavigationProperties.ImageCrossfade:
                    {
                        if (RenderEngine.StudyOpacity > 0)
                        {
                            RenderEngine.StudyOpacity = 100;
                            return true;
                        }
                        else
                        {
                            RenderEngine.StudyOpacity = 0;
                            return false;
                        }
                    }

                case NavigationProperties.FadeToBlack:
                    {
                        RenderEngine.Fader.TargetState = !RenderEngine.Fader.TargetState;
                        return RenderEngine.Fader.TargetState;
                    }


                case NavigationProperties.NavigationHold:
                    {
                        Friction = !Friction;
                        return Friction;
                    }
                default:
                    break;
            }
            return false;
        }

        #endregion

        public void GotoCatalogObject(string name)
        {
            MethodInvoker updatePlace = delegate
            {
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }
                try
                {
                    IPlace place = Catalogs.FindCatalogObjectExact(name);
                    RenderEngine.GotoTarget(place, false, false, true);
                }
                catch
                {
                }
            };
            try
            {
                Invoke(updatePlace);
            }
            catch
            {
            }
        }

        private void settingsMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void clientNodeListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClientNodeList.IsNodeListVisible())
            {
                ClientNodeList.HideNodeList();
            }
            else
            {
                ClientNodeList.ShowNodeList();
            }
        }

        private void cacheImageryTilePyramidToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IPlace place = (IPlace)contextMenuTargetObject;
                IImageSet imageSet = null;
                if (place.StudyImageset != null)
                {
                    imageSet = place.StudyImageset;
                }
                if (place.BackgroundImageSet != null)
                {
                    imageSet = place.BackgroundImageSet;
                }
                if (imageSet != null)
                {
                    CacheTilePyramid ctp = new CacheTilePyramid();
                    ctp.imageSet = imageSet;
                    ctp.ShowDialog();
                }
            }
            catch
            {


            }
        }

        private void restoreCacheFromCabinetFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestoreCache();
        }

        public static void RestoreCache()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Language.GetLocalizedText(1056, "WWT Cabinet File(*.cabinet)|*.cabinet");
            openFile.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(openFile.FileName))
                {
                    ExtractCacheCabinet(openFile.FileName, false);
                }
            }
        }

        private static void ExtractCacheCabinet(string filename, bool eraseFirst)
        {
            string dataDir = Properties.Settings.Default.CahceDirectory;
            if (eraseFirst)
            {
                if (Directory.Exists(dataDir))
                {
                    Directory.Delete(dataDir, true);
                }
            }

            FileCabinet cab = new FileCabinet(filename, dataDir);
            cab.Extract();

        }

        private void saveCacheAsCabinetFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtractCache();
        }

        public static void ExtractCache()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = Language.GetLocalizedText(1056, "WWT Cabinet File(*.cabinet)|*.cabinet");
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = ".cabinet";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string path = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\WorldWideTelescope";

                    FileCabinet cab = new FileCabinet(saveDialog.FileName, Properties.Settings.Default.CahceDirectory);

                    InjestDirectory(cab, path);
                    cab.Package();
                }
                catch (Exception ex)
                {
                    UiTools.ShowMessageBox(ex.Message, "Error Saving cache");
                }
            }
        }

        private static void InjestDirectory(FileCabinet cab, string path)
        {
            foreach (string dir in Directory.GetDirectories(path))
            {
                InjestDirectory(cab, dir);

            }
            foreach (string file in Directory.GetFiles(path))
            {
                cab.AddFile(file);

            }
        }

        private void showCacheSpaceUsedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IPlace place = (IPlace)contextMenuTargetObject;
                IImageSet imageSet = null;
                if (place.StudyImageset != null)
                {
                    imageSet = place.StudyImageset;
                }
                if (place.BackgroundImageSet != null)
                {
                    imageSet = place.BackgroundImageSet;
                }

                long totalBytes = 0;
                long demBytes = 0;
                string path = Properties.Settings.Default.CahceDirectory + @"Imagery\" + imageSet.ImageSetID.ToString();

                string demPath = Properties.Settings.Default.CahceDirectory + @"dem\" + Math.Abs(imageSet.DemUrl != null ? imageSet.DemUrl.GetHashCode32() : 0).ToString();

                Cursor.Current = Cursors.WaitCursor;

                if (!Directory.Exists(path) && !Directory.Exists(demPath))
                {
                    UiTools.ShowMessageBox(Language.GetLocalizedText(127, "There is no image data in the disk cache to purge."));
                    return;
                }

                ClearCache.TotalDir(path, ref totalBytes, 0);


                if (Directory.Exists(demPath))
                {
                    ClearCache.TotalDir(demPath, ref demBytes, 0);
                }

                Cursor.Current = Cursors.Default;
                UiTools.ShowMessageBox(String.Format(Language.GetLocalizedText(1048, "The imageset uses {0}MB of disk space."), ((double)(totalBytes + demBytes) / 1024.0 / 1024.0).ToString("f")));

            }
            catch
            {


            }

        }

        public void ShowDebugBitmap(Bitmap bmp)
        {

            MethodInvoker ShowMeCall = delegate
            {
                DBmp.ShowBitmap(bmp);
            };
            try
            {
                Invoke(ShowMeCall);
            }
            catch
            {
            }

        }

        private void lockVerticalSyncToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.FrameSync = !Properties.Settings.Default.FrameSync;
            lockVerticalSyncToolStripMenuItem.Checked = Properties.Settings.Default.FrameSync;
        }

        private void fpsToolStripMenuItemUnlimited_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TargetFrameRate = 0;
        }

        private void fPSToolStripMenuItem60_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TargetFrameRate = 60;
        }
        private void fPSToolStripMenuItem30_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TargetFrameRate = 30;
        }

        private void fPSToolStripMenuItem24_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TargetFrameRate = 24;
        }

        private void targetFrameRateToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            fpsToolStripMenuItemUnlimited.Checked = Properties.Settings.Default.TargetFrameRate == 0;
            fPSToolStripMenuItem60.Checked = Properties.Settings.Default.TargetFrameRate == 60;
            fPSToolStripMenuItem30.Checked = Properties.Settings.Default.TargetFrameRate == 30;
            fPSToolStripMenuItem24.Checked = Properties.Settings.Default.TargetFrameRate == 24;
        }

        private void showOverlayListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OverlayList.ShowOverlayList();
        }

        private void tileLoadingThrottlingToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            tpsToolStripMenuItem15.Checked = Properties.Settings.Default.TileThrottling == 15;
            tpsToolStripMenuItem30.Checked = Properties.Settings.Default.TileThrottling == 30;
            tpsToolStripMenuItem60.Checked = Properties.Settings.Default.TileThrottling == 60;
            tpsToolStripMenuItem120.Checked = Properties.Settings.Default.TileThrottling == 120;
            tpsToolStripMenuItemUnlimited.Checked = Properties.Settings.Default.TileThrottling == 12000;
        }

        private void tpsToolStripMenuItem15_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TileThrottling = 15;
        }

        private void tpsToolStripMenuItem30_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TileThrottling = 30;
        }

        private void tpsToolStripMenuItem60_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TileThrottling = 60;
        }

        private void tpsToolStripMenuItem120_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TileThrottling = 120;
        }

        private void tpsToolStripMenuItemUnlimited_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.TileThrottling = 12000;
        }

        private void allowUnconstrainedTiltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.UnconstrainedTilt = !Properties.Settings.Default.UnconstrainedTilt;
        }

        private void showSlideNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowSlideNumbers = !Properties.Settings.Default.ShowSlideNumbers;

            if (tourEdit != null)
            {
                tourEdit.Refresh();
            }
        }

        private void showKeyframerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TimeLine.AreOpenTimelines)
            {
                KeyFramer.HideTimeline();
            }
            else
            {
                KeyFramer.ShowTimeline();
            }
        }

        private void ShowWelcomeTips_Click(object sender, EventArgs e)
        {
            ShowWelcome();
        }

        private void customGalaxyFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Delimeted Text(*.csv;*.tdf;*.txt)|*.csv;*.tdf;*.txt";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filename = openFile.FileName;
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    Grids.InitializeCustomCosmos(filename);
                }
                catch
                {
                    //todo localization
                    UiTools.ShowMessageBox("Could not load file");
                }
                Cursor.Current = Cursors.Default;
            }
        }

        private void newFullDomeViewInstanceToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            monitorEightToolStripMenuItem.Visible = Screen.AllScreens.Length > 7;
            monitorSevenToolStripMenuItem.Visible = Screen.AllScreens.Length > 6;
            monitorSixToolStripMenuItem.Visible = Screen.AllScreens.Length > 5;
            monitorFiveToolStripMenuItem.Visible = Screen.AllScreens.Length > 4;
            monitorFourToolStripMenuItem.Visible = Screen.AllScreens.Length > 3;
            monitorThreeToolStripMenuItem.Visible = Screen.AllScreens.Length > 2;
            monitorTwoToolStripMenuItem.Visible = Screen.AllScreens.Length > 1;
            monitorOneToolStripMenuItem.Visible = Screen.AllScreens.Length > 0;
        }

        private void CreateDomeInstanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(((ToolStripMenuItem)sender).Tag);
            CreateDomeInstance(id);
        }

        private static void CreateDomeInstance(int id)
        {
            Process.Start("wwtexplorer.exe", string.Format("-screen:{0} -domeviewer", id));
        }



       
        private void exportCurrentViewAsSTLFileFor3DPrintingToolStripMenuItem_Click(object sender, EventArgs e)
        {

            GeoRect rect = new GeoRect();

            double amount = RenderEngine.ZoomFactor /10;

            rect.North = RenderEngine.viewCamera.Lat + amount;
            rect.South = RenderEngine.viewCamera.Lat - amount;
            rect.West =  RenderEngine.viewCamera.Lng - amount;
            rect.East = RenderEngine.viewCamera.Lng + amount;

            ExportSTL props = new ExportSTL();
            props.Rect = rect;
            props.Owner = Earth3d.MainWindow;
            props.Show();
        }

        private void advancedToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            clientNodeListToolStripMenuItem.Checked = ClientNodeList.IsNodeListVisible();
        }

        private void monoModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.RiftMonoMode = !Properties.Settings.Default.RiftMonoMode;
        }

        private void startInOculusModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.RiftStartup = !Properties.Settings.Default.RiftStartup;
            RestartNow();
        }

        private void oculusRiftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenderEngine.StartRift();
        }

        private void oculusVRHeadsetToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            monoModeToolStripMenuItem.Checked = Properties.Settings.Default.RiftMonoMode;
            startInOculusModeToolStripMenuItem.Checked = Properties.Settings.Default.RiftStartup;
        }

        private void DownloadMPC_Click(object sender, EventArgs e)
        {
            MinorPlanets.DownloadNewMPSCoreFile();
        }

        private void exportCurrentCitiesViewAs3DMeshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                MercatorTile.SaveDsmDirectory = fbd.SelectedPath;
                MercatorTile.SaveDsmTileList.Clear();
                MercatorTile.SaveDsmTiles = true;
                RenderEngine.Render();
                MercatorTile.SaveDsmTiles = false;

                StringBuilder sb = new StringBuilder();

                foreach(string item in MercatorTile.SaveDsmTileList)
                {
                    sb.AppendLine(item);
                }

                MercatorTile.SaveDsmTileList.Clear();

                File.WriteAllText(string.Format("{0}\\SavedView.mesh", MercatorTile.SaveDsmDirectory), sb.ToString());
            }

        }

        private void enableExport3dCitiesModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Enable 3d Cities Saving ** This is not on by default as it takes lots of memory, only works till end of session
            IndexBuffer11.SaveIndexArray = true;
            VertexBuffer11.SaveVertexArray = true;
            Texture11.SaveStream = true;
            TileCache.PurgeQueue();
            TileCache.ClearCache();
            enable3dCitiesExport = true;
        }
    }





    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
    public class DataMessageFilter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {

            if (m.Msg == Earth3d.AlertMessage)
            {
               
                if (File.Exists(Properties.Settings.Default.CahceDirectory + @"\launchfile.txt"))
                {

                    Earth3d.launchTourFile = File.ReadAllText(Properties.Settings.Default.CahceDirectory + @"\launchfile.txt");

                    // This causes the welcome screen to go away if anther instance sends us data
                     Earth3d.closeWelcome = true;

                    Earth3d.MainWindow.StatupTimer.Enabled = true;
                    if (Earth3d.MainWindow.WindowState == FormWindowState.Minimized)
                    {
                        Earth3d.MainWindow.WindowState = FormWindowState.Maximized;
                    }
                    Earth3d.MainWindow.Focus();
                    return true;
                }
            }

            return false;
        }
    }



    enum NavigationProperties { Ra, Declination, Latitude, Longitude, Zoom, Angle, Rotation, ZoomRate, PanUpDownRate, PanLeftRightRate, RotationRate, NavigationHold, ImageCrossfade, FadeToBlack, SystemVolume, DomeAlt, DomeAz, DomeTilt, DomeAngle, FisheyeAngle, ScreenFOV };
    enum NavigationActions { MoveUp, MoveDown, MoveRight, MoveLeft, ZoomIn, ZoomOut, TiltUp, TiltDown, RotateLeft, RotateRight, DomeLeft, DomeRight, DomeUp, DomeDown, AllStop, NextItem, LastItem, Select, Back, SetForeground, SetBackground, NextMode, PreviousMode, ResetCamera, SolarSystemMode, SkyMode, EarthMode, PlanetMode, PanoramaMode, SandboxMode, PlayTour, PauseTour, StopTour, NextSlide, PreviousSlide, MoveToFirstSlide, MoveToEndSlide, ShowNextContext, ShowPreviousContext, ShowNextExplore, ShowPreviousExplore, ResetRiftView, GotoSun, GotoMercury, GotoVenus, GotoEarth, GotoMars, GotoJupiter, GotoSaturn, GotoUranus, GotoNeptune, GotoPluto, SolarSystemOverview, GotoMilkyWay, GotoSDSSGalaxies, GotoSlide };

    public struct RiftInfo
    {
        // Size of the entire screen, in pixels.
        public UInt32 HResolution;
        public UInt32 VResolution;
        // Physical dimensions of the active screen in meters. Can be used to calculate
        // projection center while considering IPD.
        public float HScreenSize;
        public float VScreenSize;
        // Physical offset from the top of the screen to the eye center, in meters.
        // This will usually, but not necessarily be half of VScreenSize.
        public float VScreenCenter;
        // Distance from the eye to screen surface, in meters.
        // Useful for calculating FOV and projection.
        public float EyeToScreenDistance;
        // Distance between physical lens centers useful for calculating distortion center.
        public float LensSeparationDistance;
        // Configured distance between the user's eye centers, in meters. Defaults to 0.064.
        public float InterpupillaryDistance;

        // Radial distortion correction coefficients.
        // The distortion assumes that the input texture coordinates will be scaled
        // by the following equation:    
        //   uvResult = uvInput * (K0 + K1 * uvLength^2 + K2 * uvLength^4)
        // Where uvInput is the UV vector from the center of distortion in direction
        // of the mapped pixel, uvLength is the magnitude of that vector, and uvResult
        // the corresponding location after distortion.
        public float DistortionK0;
        public float DistortionK1;
        public float DistortionK2;
        public float DistortionK3;

        public float ChromaAbCorrection0;
        public float ChromaAbCorrection1;
        public float ChromaAbCorrection2;
        public float ChromaAbCorrection3;

        // Desktop coordinate position of the screen (can be negative; may not be present on all platforms)
        public int DesktopX;
        public int DesktopY;
    }


    public class SphereTest
    {
        SharpDX.Direct3D11.Buffer vertices;
        SharpDX.Direct3D11.Buffer indexBuffer;
        SharpDX.Direct3D11.VertexBufferBinding vertexBufferBinding;

        Texture11 texture;
        public SphereTest()
        {
        }

        private SharpDX.Direct3D11.InputLayout layout = null;

        public void Draw(RenderContext11 renderContext)
        {
            renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            renderContext.devContext.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
            renderContext.devContext.InputAssembler.SetIndexBuffer(indexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);



            renderContext.SunPosition = new Vector3d(500, 500, 0.0);
            renderContext.SunlightColor = System.Drawing.Color.White;
            renderContext.AmbientLightColor = System.Drawing.Color.DarkGray;

            renderContext.SetupBasicEffect(BasicEffect.TextureOnly, 1.0f, System.Drawing.Color.White);
            renderContext.MainTexture = texture;

            renderContext.PreDraw();


            if (layout == null)
            {
                layout = new SharpDX.Direct3D11.InputLayout(renderContext.Device, renderContext.Shader.InputSignature, new[]
                           {
                               new SharpDX.Direct3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float,     0, 0),
                               new SharpDX.Direct3D11.InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float,       16, 0),
                           });
                renderContext.Device.ImmediateContext.InputAssembler.InputLayout = layout;
            }


            // Draw the cube
            renderContext.devContext.DrawIndexed(triangleCount * 3, 0, 0);
        }

        const double RC = (double)(3.1415927 / 180);
        const int subDivisionsX = 1000;
        const int subDivisionsY = 500;
        const double latMin = -90;
        const double latMax = 90;
        const double lngMin = -180;
        const double lngMax = 180;
        const double dGrid = 10;
        int iCount = (int)((((latMax - latMin) / dGrid) * ((lngMax - lngMin) / dGrid) * 6));
  
        int triangleCount = subDivisionsX * subDivisionsY * 2;

        public void MakeSphere(SharpDX.Direct3D11.Device d3dDevice)
        {
            // Layout from VertexShader input signature


            texture = Texture11.FromFile(d3dDevice, "earthMap.jpg");

            uint[] indexes = new uint[subDivisionsX * subDivisionsY * 6];
            float[] verts = new float[((subDivisionsX + 1) * (subDivisionsY + 1)) * 6];


            double lat, lng;

            uint index = 0;
            double latMin = 90;
            double latMax = -90;
            double lngMin = -180;
            double lngMax = 180;


            uint x1, y1;

            double latDegrees = latMax - latMin;
            double lngDegrees = lngMax - lngMin;

            double textureStepX = 1.0f / subDivisionsX;
            double textureStepY = 1.0f / subDivisionsY;
            for (y1 = 0; y1 <= subDivisionsY; y1++)
            {

                if (y1 != subDivisionsY)
                {
                    lat = latMax - (textureStepY * latDegrees * (double)y1);
                }
                else
                {
                    lat = latMin;
                }

                for (x1 = 0; x1 <= subDivisionsX; x1++)
                {
                    if (x1 != subDivisionsX)
                    {
                        lng = lngMin + (textureStepX * lngDegrees * (double)x1);
                    }
                    else
                    {
                        lng = lngMax;
                    }
                    index = (y1 * (subDivisionsX + 1) + x1) * 6;

                    GeoTo3d(verts, (int)index, lat, lng);
                }
            }

            triangleCount = (subDivisionsX) * (subDivisionsY) * 2;

            for (y1 = 0; y1 < subDivisionsY; y1++)
            {
                for (x1 = 0; x1 < subDivisionsX; x1++)
                {
                    index = (y1 * subDivisionsX * 6) + 6 * x1;
                    // First triangle in quad
                    indexes[index] = (y1 * (subDivisionsX + 1) + x1);
                    indexes[index + 1] = ((y1 + 1) * (subDivisionsX + 1) + x1);
                    indexes[index + 2] = (y1 * (subDivisionsX + 1) + (x1 + 1));

                    // Second triangle in quad
                    indexes[index + 3] = (y1 * (subDivisionsX + 1) + (x1 + 1));
                    indexes[index + 4] = ((y1 + 1) * (subDivisionsX + 1) + x1);
                    indexes[index + 5] = ((y1 + 1) * (subDivisionsX + 1) + (x1 + 1));
                }
            }

            vertices = SharpDX.Direct3D11.Buffer.Create(d3dDevice, SharpDX.Direct3D11.BindFlags.VertexBuffer, verts);

            vertexBufferBinding = new SharpDX.Direct3D11.VertexBufferBinding(vertices, sizeof(float) * 6, 0);

            indexBuffer = SharpDX.Direct3D11.Buffer.Create(d3dDevice, SharpDX.Direct3D11.BindFlags.IndexBuffer, indexes);


        }

        static public void GeoTo3d(float[] data, int baseIndex, double lat, double lng)
        {
            data[baseIndex] = (float)((Math.Cos(lng * RC)) * (Math.Cos(lat * RC)));
            data[baseIndex + 1] = (float)((Math.Sin(lat * RC)));
            data[baseIndex + 2] = (float)((Math.Sin(lng * RC)) * (Math.Cos(lat * RC)));
            data[baseIndex + 3] = 1f;
            data[baseIndex + 4] = (float)((lng + 180) / 360);
            data[baseIndex + 5] = 1 - (float)((lat + 90) / 180);
        }
    }



}
