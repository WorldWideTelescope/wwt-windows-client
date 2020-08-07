
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

        static Dictionary<string, ScriptableProperty> SettingsList
        {
            get
            {
                return Settings.SettingsList;
            }
        }

        static HTTPLayerApi()
        {
           

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
                            CameraParameters cam = RenderEngine.Engine.viewCamera;
                            if (RenderEngine.Engine.Space)
                            {
                                data = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status><ViewState lookat=\"{7}\" ra=\"{0}\" dec=\"{1}\" zoom=\"{2}\" rotation=\"{4}\" time=\"{5}\" timerate=\"{6}\" ReferenceFrame=\"Sky\" ViewToken=\"SD8834DFA\" ZoomText=\"{8}\"></ViewState></LayerApi>",
                                    cam.RA, cam.Dec, cam.Zoom, cam.Angle, cam.Rotation, SpaceTimeController.Now.ToString(), SpaceTimeController.TimeRate.ToString(), Earth3d.MainWindow.CurrentImageSet.DataSetType.ToString(), Earth3d.MainWindow.contextPanel.ViewLabelText);
                            }
                            else
                            {
                                data = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\"?><LayerApi><Status>Success</Status><ViewState lookat=\"{7}\" lat=\"{0}\" lng=\"{1}\" zoom=\"{2}\" angle=\"{3}\" rotation=\"{4}\" time=\"{5}\" timerate=\"{6}\" ReferenceFrame=\"{8}\" ViewToken=\"{10}\" ZoomText=\"{9}\"></ViewState></LayerApi>",
                                    cam.Lat, cam.Lng, cam.Zoom, cam.Angle, cam.Rotation, SpaceTimeController.Now.ToString(), SpaceTimeController.TimeRate.ToString(), Earth3d.MainWindow.CurrentImageSet.DataSetType.ToString(), Earth3d.MainWindow.FocusReferenceFrame(), Earth3d.MainWindow.contextPanel.ViewLabelText, RenderEngine.Engine.viewCamera.ToToken());
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
                            if (String.IsNullOrEmpty(propName) || Settings.SetSetting(propName, propValue, false))
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
                                        double zoom = Convert.ToDouble(RenderEngine.Engine.ZoomFactor);
                                        double rotation = Convert.ToDouble(RenderEngine.Engine.CameraRotate);
                                        double angle = Convert.ToDouble(RenderEngine.Engine.CameraAngle);
                                        cameraParams = new CameraParameters(lat, lng, zoom, rotation, angle, 100);
                                        if (RenderEngine.Engine.Space)
                                        {
                                            cameraParams.RA = result.RA;
                                        }

                                        MethodInvoker doIt = delegate
                                        {
                                            RenderEngine.Engine.GotoTarget(cameraParams, false, instant);
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
                                        double alt = RenderEngine.Engine.GetAltitudeForLatLong(lat, lng) - EGM96Geoid.Height(lat, lng);
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
                    if (RenderEngine.Engine.Space)
                    {
                        cameraParams.RA = Convert.ToDouble(lines[1]);
                    }
                    

                    MethodInvoker doIt = delegate
                    {
                        RenderEngine.Engine.GotoTarget(cameraParams, false, instant);
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
                                RenderEngine.Engine.GotoTarget(cameraParams, false, instant);
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
                            RenderEngine.Engine.GotoTarget(pl, false, false, true);
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

