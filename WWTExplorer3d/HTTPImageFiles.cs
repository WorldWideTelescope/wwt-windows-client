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

namespace TerraViewer
{
    public class HttpImageFiles : RequestHandler
    {
        private string _sig = "get /image";
        public override bool Handles(string request)
        {
            if (request.ToLower().StartsWith( "get /configuration/images/"))
            {
                return true;
            } 
            return request.ToLower().StartsWith(_sig);
        }


        public override void ProcessRequest(string request, ref Socket socket, bool authenticated, string body)
        {

            bool cache = true;
            QueryString query = new QueryString(request);

            String sMimeType = "image/JPEG";

            if (request.ToLower().IndexOf(".png") > -1)
            {
                sMimeType = "image/PNG";
            }

            int iTotBytes = 0;

            string fileName = request.Substring(request.LastIndexOf("/")+1);

            byte[] data = new byte[0];
            if (request.Contains("/tour/"))
            {
                if (Earth3d.MainWindow.TourEdit != null)
                {
                    TourDocument tour = Earth3d.MainWindow.TourEdit.Tour;
                    if (fileName.ToLower() == "slidelist.xml")
                    {
                        sMimeType = "text/xml";
                        data = tour.GetSlideListXML();
                    }
                    else if (tour != null)
                    {
                        data = ReadBinaryWebFileFromDisk(tour.WorkingDirectory + fileName);
                    }
                }
                else
                {
                    if (fileName.ToLower() == "slidelist.xml")
                    {
                        sMimeType = "text/xml";
                        data = TourDocument.GetEmptySlideListXML();
                    }
                }
            }
            else if (fileName == "refresh.png")
            {
                Earth3d.RefreshCommunity();
            }
            else if (fileName == "screenshot.png")
            {
                if (ScreenBroadcast.Capturing)
                {
                    data = ScreenBroadcast.ScreenImage;
                }
            }
            else if (request.Contains("/imageset/"))
            {
                string[] parts = request.Split(new char[] { '/' });
                IImageSet imageset = (IImageSet)RenderEngine.ImagesetHashTable[Convert.ToInt32(parts[2])];
                if (imageset != null)
                {
                    data = ReadBinaryWebFileFromDisk(imageset.Url);
                }
            }
            else if (fileName == "layermap")
            {
                SendBinaryFileFromDisk(Properties.Settings.Default.CahceDirectory + "\\layerSync.layers", ref socket, sMimeType);
                return;
            }
            else if (fileName == "tour.wtt")
            {
                SendBinaryFileFromDisk(Properties.Settings.Default.CahceDirectory + "\\tourSync.wtt", ref socket, sMimeType);
                return;
            }
            else if (fileName == "colorsettings")
            {
                data = NetControl.GetColorSettingsData();
            }
            else if (fileName == "executable")
            {
                SendBinaryFileFromDisk("wwtexplorer.exe", ref socket, sMimeType);
                return;
            }
            else if (fileName.StartsWith("distort") || fileName.StartsWith("blend"))
            {
                string path = String.Format("{0}\\ProjetorWarpMaps\\", Properties.Settings.Default.CahceDirectory);

                data = ReadBinaryWebFileFromDisk(path + fileName);
            }
            else
            {
                data = ReadBinaryWebFile(fileName);
            }

            iTotBytes = data.Length;
            SendHeader(HttpVersion, sMimeType, iTotBytes, " 200 OK", ref socket, cache);
            SendToBrowser(data, iTotBytes, ref socket);
        }
    }
}

