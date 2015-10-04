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
using System.Net.Sockets;

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

            var cache = true;
            var query = new QueryString(request);

            var sMimeType = "image/JPEG";

            if (request.ToLower().IndexOf(".png") > -1)
            {
                sMimeType = "image/PNG";
            }

            var iTotBytes = 0;

            var fileName = request.Substring(request.LastIndexOf("/")+1);

            var data = new byte[0];
            if (request.Contains("/tour/"))
            {
                if (Earth3d.MainWindow.TourEdit != null)
                {
                    var tour = Earth3d.MainWindow.TourEdit.Tour;
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
                var parts = request.Split(new[] { '/' });
                var imageset = Earth3d.ImagesetHashTable[Convert.ToInt32(parts[2])];
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
                var path = String.Format("{0}\\ProjetorWarpMaps\\", Properties.Settings.Default.CahceDirectory);

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

