
/*  
===============================================================================
 2007-2008 Copyright © Microsoft Corporation.  All rights reserved.
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
 OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
 LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 FITNESS FOR A PARTICULAR PURPOSE.
===============================================================================
*/
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;
using System.Drawing;
using System.Xml;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;

namespace TerraViewer
{
    public class HTTPLayerApi : RequestHandler
    {
        private string _sig = "get /layerapi.aspx?";
        public override bool Handles(string request)
        {
            if ( request.ToLower().StartsWith(_sig.Replace("get ","post ")))
            {
                return true;
            }
            return request.ToLower().StartsWith(_sig);
        }

        static Dictionary<string, ScriptableProperty> SettingsList;

        static HTTPLayerApi()
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

        public override void ProcessRequest(string request, ref Socket socket, bool authenticated, string body)
        {
           
            QueryString query = new QueryString(request);

            String sMimeType = "text/xml";
            string data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
            if (!authenticated)
            {
                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - IP Not Authorized by Client</Status></LayerApi>";
                SendHeaderAndData(data, ref socket, sMimeType);
                return;
            }

            string cmd = query["cmd"].ToLower();

            Guid layerID = Guid.Empty;

            if (!string.IsNullOrEmpty(query["id"]))
            {
                layerID = new Guid(query["id"]);
            }



            int color = Color.White.ToArgb();
            if (!string.IsNullOrEmpty(query["color"]))
            {
                color = int.Parse(query["color"],System.Globalization.NumberStyles.HexNumber);
            }


            int currentVersion = 0;
            if (!string.IsNullOrEmpty(query["version"]))
            {
                currentVersion = int.Parse(query["version"]);
            }

            string notifyType = "None";
            if (!string.IsNullOrEmpty(query["notifytype"]))
            {
                notifyType = query["notifytype"];
            }

            int notifyTimeout = 30000;
            if (!string.IsNullOrEmpty(query["notifytimeout"]))
            {
                notifyTimeout = Math.Min(120000,int.Parse(query["notifytimeout"]));
            }

            int notifyRate = 100;
            if (!string.IsNullOrEmpty(query["notifyrate"]))
            {
                notifyRate = Math.Min(10000,int.Parse(query["notifyrate"]));
            } 
            
            DateTime dateTime = DateTime.Now;      
            if (!string.IsNullOrEmpty(query["datetime"]))
            {
                dateTime = Convert.ToDateTime(query["datetime"]);
                SpaceTimeController.Now = dateTime;
            }

            DateTime beginDate = DateTime.MinValue;
            if (!string.IsNullOrEmpty(query["startdate"]))
            {
                beginDate = Convert.ToDateTime(query["startdate"]);
            }
            
            DateTime endDate = DateTime.MaxValue;
            if (!string.IsNullOrEmpty(query["enddate"]))
            {
                endDate = Convert.ToDateTime(query["enddate"]);
            }

            double timeRate = 1.0;
            if (!string.IsNullOrEmpty(query["timerate"]))
            {
                timeRate = Convert.ToDouble(query["timerate"]);
                SpaceTimeController.TimeRate = timeRate;
            }
   
            string name = "New Layer";
            if (!string.IsNullOrEmpty(query["name"]))
            {
                name = query["name"];
            }
            
            string propName = "";
            if (!string.IsNullOrEmpty(query["propname"]))
            {
                propName = query["propname"];
            }
            
            string propValue = "";
            if (!string.IsNullOrEmpty(query["propvalue"]))
            {
                propValue = query["propvalue"];
            }    
            
            string filename = "";
            if (!string.IsNullOrEmpty(query["filename"]))
            {
                filename = query["filename"];
            }

            string referenceFrame = "";
            if (!string.IsNullOrEmpty(query["frame"]))
            {
                referenceFrame = query["frame"];
            }

            string parent = "";
            if (!string.IsNullOrEmpty(query["parent"]))
            {
                parent = query["parent"];
            }

            string move = "";
            if (!string.IsNullOrEmpty(query["move"]))
            {
                move = query["move"];
            }
            FadeType fadeType = FadeType.None;
            if (!string.IsNullOrEmpty(query["fadetype"]))
            {
                fadeType = (FadeType)Enum.Parse(typeof(FadeType), query["fadetype"]);
            }

            double fadeRange = 0.0;
            if (!string.IsNullOrEmpty(query["faderange"]))
            {
                fadeRange = Convert.ToDouble(query["faderange"]);
            }

            bool showLayer = true;
            if (!string.IsNullOrEmpty(query["show"]))
            {
                showLayer = Convert.ToBoolean(query["show"]);
            }


            string flyTo = null;
            if (!string.IsNullOrEmpty(query["flyto"]))
            {
                flyTo = query["flyto"];
            }

            string lookat = null;
            if (!string.IsNullOrEmpty(query["lookat"]))
            {
                lookat = query["lookat"];
            }

            bool instant = false;
            if (!string.IsNullOrEmpty(query["instant"]))
            {
                instant = Convert.ToBoolean(query["instant"]);
            }

            bool noPurge = false;
            if (!string.IsNullOrEmpty(query["nopurge"]))
            {
                noPurge = Convert.ToBoolean(query["nopurge"]);
            }

            bool fromClipboard = false;
            if (!string.IsNullOrEmpty(query["fromclipboard"]))
            {
                fromClipboard = Convert.ToBoolean(query["fromclipboard"]);
            }

            bool purgeAll = false;
            if (!string.IsNullOrEmpty(query["purgeall"]))
            {
                purgeAll = Convert.ToBoolean(query["purgeall"]);
            }

            if (!string.IsNullOrEmpty(query["autoloop"]))
            {
                LayerManager.SetAutoloop(Convert.ToBoolean(query["autoloop"]));
            }

            bool layersOnly = false;
            if (!string.IsNullOrEmpty(query["layersonly"]))
            {
                layersOnly = Convert.ToBoolean(query["layersonly"]);
            }
            
            bool hasheader = false;
            if (!string.IsNullOrEmpty(query["hasheader"]))
            {
                hasheader = Convert.ToBoolean(query["hasheader"]);
            }

            string coordinates = "";
            if (!string.IsNullOrEmpty(query["coordinates"]))
            {
                coordinates = query["coordinates"];
            }

            string cmdtarget = "";
            if (!string.IsNullOrEmpty(query["cmdtarget"]))
            {
                cmdtarget = query["cmdtarget"];
            }

            string cmdtype = "";
            if (!string.IsNullOrEmpty(query["cmdtype"]))
            {
                cmdtype = query["cmdtype"];
            }
            // Update the time and rate
            if (layerID != Guid.Empty || !string.IsNullOrEmpty(referenceFrame) || cmd == "getprojectorconfig" || cmd == "setprojectorconfig" || cmd == "loadtour" || cmd == "layerlist" || cmd == "move" || cmd == "version" || cmd == "uisettings" || cmd == "new" || cmd == "load" || cmd == "group" || cmd == "state" || cmd == "mode" || cmd == "showlayermanager" || cmd == "hidelayermanager" || cmd == "getelevation" || cmd == "notify" || cmd == "dispatch")
            {
                switch (cmd)
                {
                    case "dispatch":
                        {
                            if (DispatchCommand(cmdtarget, cmdtype, propName, propValue))
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                            }
                            else
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid layer ID</Status></LayerApi>"; 
                            }
                        }
                        break;
                    case "notify":
                        switch (notifyType.ToLower())
                        {
                            case "none":
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid Notify Type</Status></LayerApi>";
                                break;
                            case "layer":
                                if (layerID == Guid.Empty)
                                {
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid layer ID</Status></LayerApi>";
                                }
                                else
                                {
                                    DateTime start = DateTime.Now;
                                    //default text
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Timeout</Status></LayerApi>";
                                    while ((DateTime.Now - start).TotalMilliseconds < notifyTimeout)
                                    {
                                        Thread.Sleep(notifyRate);
                                        if (!LayerManager.LayerList.ContainsKey(layerID))
                                        {
                                            data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid layer ID</Status></LayerApi>";
                                            break;
                                        }

                                        Layer target = LayerManager.LayerList[layerID];
                                        if (target == null)
                                        {
                                            data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid layer ID</Status></LayerApi>";
                                            break;
                                        }
                                       
                                        else
                                        {
                                            if (target.Version != currentVersion)
                                            {
                                                data = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status><Layer {0}=\"{1}\" Version=\"{2}\"></Layer></LayerApi>", "ID", target.ID, target.Version);
                                                break;
                                            }
                                        }
                                    }

                                }
                                break;
                            case "layerlist":
                                {
                                    DateTime start = DateTime.Now;
                                    //default text
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Timeout</Status></LayerApi>";
                                    while ((DateTime.Now - start).TotalMilliseconds < notifyTimeout)
                                    {
                                        Thread.Sleep(notifyRate);

                                        if (LayerManager.Version != currentVersion)
                                        {
                                            data = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status><LayerList Version=\"{0}\"></LayerList></LayerApi>",  LayerManager.Version);
                                            break;
                                        }
                                    }

                                }
                                break;
                        }

                        break;
                    case "version":
                        {
                            data = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Version>{0}</Version></LayerApi>", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());

                        }
                        break;
                    case "group":
                        {
                            if (LayerManager.CreateLayerGroup(name, referenceFrame))
                            {

                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                            }
                            else
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid layer ID</Status></LayerApi>";
                            }
                        }
                        break;
                    case "load":
                        {
                            Guid id = LayerManager.LoadLayer(name, referenceFrame, filename, color, beginDate, endDate, fadeType, fadeRange);
                            if (id != Guid.Empty)
                            {
                                data = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><NewLayerID>{0}</NewLayerID></LayerApi>", id.ToString());
                            }
                            else
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Could not Load Layer</Status></LayerApi>";
                            }

                        }
                        break;
                    case "loadtour":
                        {
                            MethodInvoker doIt = delegate
                            {
                                Earth3d.MainWindow.NoShowTourEndPage = true;
                                Earth3d.MainWindow.LoadTourFromFile(filename, false, "");
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

                            if (File.Exists(filename))
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                            }
                            else
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Could not Load Tour</Status></LayerApi>";
                            }

                        }
                        break;

                    case "newframe":
                        {
                            if (LayerManager.CreateReferenceFrame(referenceFrame, parent, body))
                            {
                                 data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                            }
                            else
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid Parameter</Status></LayerApi>";
                            }
                        }
                        break;

                    case "new":
                        {
                            Guid id = LayerManager.CreateLayerFromString(body, name, referenceFrame, false, color, beginDate, endDate, fadeType, fadeRange);
                            data = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><NewLayerID>{0}</NewLayerID></LayerApi>", id.ToString());
                        }
                        break;
                    case "get":
                        {
                            data = LayerManager.GetLayerDataID(layerID);
                            if (string.IsNullOrEmpty(data))
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid layer ID</Status></LayerApi>";
                            }
                        }
                        break;
                    case "update":
                        {
                            if (name == "New Layer")
                            {
                                name = null;
                            }

                            if (LayerManager.UpdateLayer(layerID, body, showLayer, name, noPurge, purgeAll, hasheader))
                            {

                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                            }
                            else
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid layer ID</Status></LayerApi>";
                            }
                        }


                        break;
                    case "layerlist":
                        {
                            data = LayerManager.GetLayerList(layersOnly);
                            if (data == null)
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid layer ID</Status></LayerApi>";
                            }
                        }
                        break;
                    case "activate":
                        {
                            if (LayerManager.ActivateLayer(layerID))
                            {

                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                            }
                            else
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid layer ID</Status></LayerApi>";
                            }
                        }
                        break;
                    case "setprop":
                        {
                            if (layerID != Guid.Empty)
                            {                         
                                if (LayerManager.SetLayerPropByID(layerID, propName, propValue))
                                {

                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                                }
                                else
                                {
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid parameter</Status></LayerApi>";
                                }
                            }   
                            else
                            {
                                if (LayerManager.SetFramePropByName(referenceFrame, propName, propValue))
                                {

                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                                }
                                else
                                {
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid parameter</Status></LayerApi>";
                                }
                               }
                        }
                        break;
                    case "setprops":
                        {
                            if (layerID != Guid.Empty)
                            {
                                if (LayerManager.SetLayerPropsByID(layerID, body))
                                {

                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                                }
                                else
                                {
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid parameter</Status></LayerApi>";
                                }
                            }
                            else
                            {
                                if (LayerManager.SetFramePropsByName(referenceFrame, body))
                                {

                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                                }
                                else
                                {
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid parameter</Status></LayerApi>";
                                }
                            }
                        }
                        break;
                    case "setprojectorconfig":
                        {
                            //if (layerID != Guid.Empty)
                            //{
                            //    if (LayerManager.SetLayerPropsByID(layerID, body))
                            //    {

                            //        data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                            //    }
                            //    else
                            //    {
                            //        data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid parameter</Status></LayerApi>";
                            //    }
                            //}
                            //else
                            //{
                            //    if (LayerManager.SetFramePropsByName(referenceFrame, body))
                            //    {

                            //        data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                            //    }
                            //    else
                            //    {
                            //        data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid parameter</Status></LayerApi>";
                            //    }
                            //}
                        }
                        break;

                    case "getprojectorconfig":
                        {
                            data = ClientNodes.GetXML(NetControl.NodeList);
                        }
                        break;

                    case "getprop":
                        {
                            if (layerID != Guid.Empty)
                            {
                                Layer layer = null;
                                string val = LayerManager.GetLayerPropByID(layerID, propName, out layer);
                                if (!string.IsNullOrEmpty(val))
                                {

                                    data = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status><Layer {0}=\"{1}\" Version=\"{2}\"></Layer></LayerApi>", propName, val, layer.Version);
                                }
                                else
                                {
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid parameter</Status></LayerApi>";
                                }
                            }
                            else
                            {
                                ReferenceFrame frame = null;
                                string val = LayerManager.GetFramePropByName(referenceFrame, propName, out frame);
                                if (!string.IsNullOrEmpty(val))
                                {
                                    data = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status><Frame {0}=\"{1}\"></Frame></LayerApi>", propName, val);
                                }
                                else
                                {
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid parameter</Status></LayerApi>";
                                }
                            }
                        }
                        break;
                    case "getprops":
                        {
                            if (layerID != Guid.Empty)
                            {
                                data = LayerManager.GetLayerPropsByID(layerID);
                                if (string.IsNullOrEmpty(data))
                                {
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid layer ID</Status></LayerApi>";
                                }
                            }
                            else
                            {
                                data = LayerManager.GetFramePropsByName(referenceFrame);
                                if (string.IsNullOrEmpty(data))
                                {
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid Frame Name</Status></LayerApi>";
                                }
                            }

                        }
                        break;
                    case "delete":
                        {
                            if (layerID != Guid.Empty)
                            {
                                if (LayerManager.DeleteLayerByID(layerID, true, true))
                                {

                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                                }
                                else
                                {
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid layer ID</Status></LayerApi>";
                                }
                            }
                            else
                            {
                                if (LayerManager.DeleteFrameByName(referenceFrame))
                                {

                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                                }
                                else
                                {
                                    data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid Frame Name</Status></LayerApi>";
                                }
                            }
                        }
                        break;
                    case "state":
                        {
                            CameraParameters cam = Earth3d.MainWindow.viewCamera;
                            if (Earth3d.MainWindow.Space)
                            {
                                data = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status><ViewState lookat=\"{7}\" ra=\"{0}\" dec=\"{1}\" zoom=\"{2}\" rotation=\"{4}\" time=\"{5}\" timerate=\"{6}\" ReferenceFrame=\"Sky\" ViewToken=\"SD8834DFA\" ZoomText=\"{8}\"></ViewState></LayerApi>",
                                    cam.RA, cam.Dec, cam.Zoom, cam.Angle, cam.Rotation, SpaceTimeController.Now.ToString(), SpaceTimeController.TimeRate.ToString(), Earth3d.MainWindow.CurrentImageSet.DataSetType.ToString(), Earth3d.MainWindow.contextPanel.ViewLabelText);
                            }
                            else
                            {
                                data = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status><ViewState lookat=\"{7}\" lat=\"{0}\" lng=\"{1}\" zoom=\"{2}\" angle=\"{3}\" rotation=\"{4}\" time=\"{5}\" timerate=\"{6}\" ReferenceFrame=\"{8}\" ViewToken=\"{10}\" ZoomText=\"{9}\"></ViewState></LayerApi>",
                                    cam.Lat, cam.Lng, cam.Zoom, cam.Angle, cam.Rotation, SpaceTimeController.Now.ToString(), SpaceTimeController.TimeRate.ToString(), Earth3d.MainWindow.CurrentImageSet.DataSetType.ToString(), Earth3d.MainWindow.FocusReferenceFrame(), Earth3d.MainWindow.contextPanel.ViewLabelText, Earth3d.MainWindow.viewCamera.ToToken());
                            }
                        }
                        break;
                    case "mode":
                        {
                            ImageSetType lookAt = (ImageSetType)Enum.Parse(typeof(ImageSetType), lookat);
                            Earth3d.MainWindow.contextPanel.SetLookAtTarget(lookAt);

                            data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";

                        }
                        break;
                    case "uisettings":
                        {
                            if (String.IsNullOrEmpty(propName) || SetSetting(propName, propValue, false))
                            {

                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                            }
                            else
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid parameter</Status></LayerApi>";
                            }
                        }
                        break;
                    case "showlayermanager":
                        {
                            MethodInvoker doIt = delegate
                            {
                                Earth3d.MainWindow.ShowLayersWindows = true;
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
                        break;
                    case "hidelayermanager":
                        {
                            MethodInvoker doIt = delegate
                              {
                                  Earth3d.MainWindow.ShowLayersWindows = false;
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

                        break;
                    case "move":
                        {
                            data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status></LayerApi>";
                            if (move.ToLower().StartsWith("reticle"))
                            {
                                string[] parts = move.Split(new char[] { ':' });
                                if (parts.Length > 1)
                                {
                                    int id = int.Parse(parts[1]);
                                    Coordinates result = Earth3d.MainWindow.GetCoordinatesForReticle(id);

                                    if (Earth3d.MainWindow.SolarSystemMode)
                                    {
                                        Earth3d.MainWindow.GotoReticlePoint(id);
                                    }
                                    else
                                    {
                                        CameraParameters cameraParams;
                                        double lat = result.Lat;
                                        double lng = result.Lng;
                                        double zoom = Convert.ToDouble(Earth3d.MainWindow.ZoomFactor);
                                        double rotation = Convert.ToDouble(Earth3d.MainWindow.CameraRotate);
                                        double angle = Convert.ToDouble(Earth3d.MainWindow.CameraAngle);
                                        cameraParams = new CameraParameters(lat, lng, zoom, rotation, angle, 100);
                                        if (Earth3d.MainWindow.Space)
                                        {
                                            cameraParams.RA = result.RA;
                                        }

                                        MethodInvoker doIt = delegate
                                        {
                                            Earth3d.MainWindow.GotoTarget(cameraParams, false, instant);
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
                                }
                            }

                            else
                            {
                                switch (move)
                                {
                                    case "ZoomIn":
                                        Earth3d.MainWindow.ZoomIn();
                                        break;
                                    case "ZoomOut":
                                        Earth3d.MainWindow.ZoomOut();
                                        break;
                                    case "Up":
                                        Earth3d.MainWindow.MoveUp();
                                        break;
                                    case "Down":
                                        Earth3d.MainWindow.MoveDown();
                                        break;
                                    case "Left":
                                        Earth3d.MainWindow.MoveLeft();
                                        break;
                                    case "Right":
                                        Earth3d.MainWindow.MoveRight();
                                        break;
                                    case "Clockwise":
                                        Earth3d.MainWindow.RotateView(0, .2);
                                        break;
                                    case "CounterClockwise":
                                        Earth3d.MainWindow.RotateView(0, -.2);
                                        break;
                                    case "TiltUp":
                                        Earth3d.MainWindow.RotateView(-.2, 0);
                                        break;
                                    case "TiltDown":
                                        Earth3d.MainWindow.RotateView(.2, 0);
                                        break;
                                    case "Finder":
                                        break;
                                    case "Play":
                                        ((IScriptable)Earth3d.MainWindow).InvokeAction("PlayTour", "");
                                        break;
                                    case "Pause":
                                        ((IScriptable)Earth3d.MainWindow).InvokeAction("PauseTour", "");
                                        break;
                                    case "PreviousSlide":
                                        ((IScriptable)Earth3d.MainWindow).InvokeAction("PreviousSlide", "");
                                        break;
                                    case "NextSlide":
                                        ((IScriptable)Earth3d.MainWindow).InvokeAction("NextSlide", "");
                                        break;
                                    default:
                                        data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid parameter</Status></LayerApi>";
                                        break;
                                }
                            }
                        }
                        break;
                    case "getelevation":
                        {
                            string[] parts = coordinates.Split(new char[] { ',' });
                            StringBuilder sb = new StringBuilder();
                            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status><Elevations>");
                            try
                            {
                                foreach (string part in parts)
                                {
                                    string[] latLng = part.Split(new char[] { ' ' });
                                    if (latLng.Length > 1)
                                    {

                                        double lat = double.Parse(latLng[0]);
                                        double lng = double.Parse(latLng[1]);
                                        double alt = Earth3d.MainWindow.GetAltitudeForLatLong(lat, lng) - EGM96Geoid.Height(lat, lng);
                                        sb.Append(string.Format("<Coordinates Lat=\"{0}\" Lng=\"{1}\" Altitude=\"{2}\" />", lat, lng, alt));

                                    }
                                }
                                sb.Append("</Elevations></LayerApi>");

                                data = sb.ToString();
                            }
                            catch
                            {
                                data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - Invalid parameter</Status></LayerApi>";
                            }
                        }
                        break;
                    default:
                        data = "<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Error - No Command</Status></LayerApi>";
                        break;
                }
            }

            if (!string.IsNullOrEmpty(flyTo))
            {
                string[] lines = flyTo.Split(new char[] { ',' });
                if (lines.Length == 1)
                {
                    MethodInvoker doIt = delegate
                    {
                        Earth3d.MainWindow.SetBackgroundByName(lines[0]);
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

                if (lines.Length == 5 )
                {
                    CameraParameters cameraParams;
                    double lat = Convert.ToDouble(lines[0]);
                    double lng = Convert.ToDouble(lines[1]);
                    double zoom = Convert.ToDouble(lines[2]);
                    double rotation = Convert.ToDouble(lines[3]);
                    double angle = Convert.ToDouble(lines[4]);
                    cameraParams = new CameraParameters(lat, lng, zoom, rotation, angle, 100);
                    if (Earth3d.MainWindow.Space)
                    {
                        cameraParams.RA = Convert.ToDouble(lines[1]);
                    }
                    

                    MethodInvoker doIt = delegate
                    {
                        Earth3d.MainWindow.GotoTarget(cameraParams, false, instant);
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

                if (lines.Length > 5)
                {
                    CameraParameters cameraParams;
                    double lat = Convert.ToDouble(lines[0]);
                    double lng = Convert.ToDouble(lines[1]);
                    double zoom = Convert.ToDouble(lines[2]);
                    double rotation = Convert.ToDouble(lines[3]);
                    double angle = Convert.ToDouble(lines[4]);
                    string frame = lines[5];
                    string token = "";
                    cameraParams = new CameraParameters(lat, lng, zoom, rotation, angle, 100);
                    bool done = false;
                    if (frame == "Sky")
                    {
                        cameraParams.RA = Convert.ToDouble(lines[1]);
                    }
                    else
                    {
                        if (Earth3d.MainWindow.CurrentImageSet.DataSetType == ImageSetType.Planet)
                        {
                            if (!Earth3d.MainWindow.CurrentImageSet.Name.ToLower().Contains(frame.ToLower()))
                            {
                                MethodInvoker doIt3 = delegate
                                {
                                    Earth3d.MainWindow.SetBackgroundByName(frame);
                                };
                                if (Earth3d.MainWindow.InvokeRequired)
                                {
                                    try
                                    {
                                        Earth3d.MainWindow.Invoke(doIt3);
                                    }
                                    catch
                                    {
                                    }
                                }
                                else
                                {
                                    doIt3();
                                }
                            }
                            MethodInvoker doIt2 = delegate
                            {
                                Earth3d.MainWindow.GotoTarget(cameraParams, false, instant);
                            };

                            if (Earth3d.MainWindow.InvokeRequired)
                            {
                                try
                                {
                                    Earth3d.MainWindow.Invoke(doIt2);
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                doIt2();
                            }
                            done = true;

                        }
                        else
                        {
                            try
                            {
                                cameraParams.Target = (SolarSystemObjects)Enum.Parse(typeof(SolarSystemObjects), frame, true);
                            }
                            catch
                            {
                                cameraParams.Target = SolarSystemObjects.Custom;
                                //cameraParams.ViewTarget =  */Stuff here */
                                //todo get custom location and insert 
                            }
                        }
                    }
                    if (!done)
                    {
                        TourPlace pl = new TourPlace(frame, 0, 0, Classification.SolarSystem, "UMA", Earth3d.MainWindow.CurrentImageSet.DataSetType == ImageSetType.SolarSystem ? ImageSetType.Sky : Earth3d.MainWindow.CurrentImageSet.DataSetType, zoom);
                        if (lines.Length > 6)
                        {
                            token = lines[6];
                            if (!string.IsNullOrEmpty(token))
                            {
                                cameraParams = CameraParameters.FromToken(token);
                            }
                        }
                        pl.CamParams = cameraParams;

                        MethodInvoker doIt = delegate
                        {
                            Earth3d.MainWindow.GotoTarget(pl, false, false, true);
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
                }   
            }


            SendHeaderAndData(data, ref socket, sMimeType);
        }
        public static bool SetSetting(string name, string value, bool quickChange)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (quickChange)
            {
                Earth3d.ignoreChanges = true;
            }
            try
            {

                bool safeToSet = SettingsList.ContainsKey(name);

                if (safeToSet)
                {
                    Type thisType = Properties.Settings.Default.GetType();
                    PropertyInfo pi = thisType.GetProperty(name);
                    if (pi.PropertyType.BaseType == typeof(Enum))
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
                Earth3d.ignoreChanges = false;
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

        public static ScriptableProperty[] GetSettingsList()
        {
            List<ScriptableProperty> settingsList = new List<ScriptableProperty>();

            foreach (ScriptableProperty setting in SettingsList.Values)
            {
                settingsList.Add(setting);
            }
            return settingsList.ToArray();
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

        public static bool DispatchCommand(string targetType, string bindingType, string propertyName, string value )
        {
            try
            {
                IScriptable scriptInterface = null;
                switch (targetType.ToLower())
                {
                    case "setting":
                        scriptInterface = Settings.Active as IScriptable;
                        break;
                    case "spaceTimeController":
                        scriptInterface = SpaceTimeController.ScriptInterface;
                        break;
                    case "goto":
                        Earth3d.MainWindow.GotoCatalogObject(propertyName);
                        break;
                    case "layer":
                        scriptInterface = LayerManager.ScriptInterface;
                        break;
                    case "actions":
                        break;
                    case "navigation":
                        scriptInterface = Earth3d.MainWindow as IScriptable;
                        break;
                    default:
                        break;
                }

                if (scriptInterface != null && !string.IsNullOrEmpty(propertyName))
                {
                    switch (bindingType.ToLower())
                    {
                        case "action":
                            scriptInterface.InvokeAction(propertyName, value);
                            break;
                        case "toggle":
                            scriptInterface.ToggleProperty(propertyName);
                            break;
                        case "syncvalue":
                            scriptInterface.SetProperty(propertyName, value);
                            break;
                        case "setvalue":
                            scriptInterface.SetProperty(propertyName, value);
                            break;
                        default:
                            break;
                    }

                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

