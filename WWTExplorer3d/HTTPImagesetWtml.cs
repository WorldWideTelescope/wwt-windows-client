
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

namespace TerraViewer
{
    public class HTTPImagesetWtml : RequestHandler
    {
        private string _sig = "get /imagesetwtml?id=";
        public override bool Handles(string request)
        {
            if (request.ToLower().StartsWith(_sig))
            {
                return true;
            } 
            return request.ToLower().StartsWith(_sig);
        }


        public override void ProcessRequest(string request, ref Socket socket, bool authenticated, string body)
        {

            var query = new QueryString(request);

            var sMimeType = "text/xml";
            var data="";

            var id = query.GetValues("id")[0];
            var hash = 0;
            try
            {
                hash = Convert.ToInt32(id);
                if (Earth3d.ImagesetHashTable.ContainsKey(hash))
                {
                    
                    var sb = new StringBuilder();
                    var sw = new StringWriter(sb);
                    using (var xmlWriter = new XmlTextWriter( sw ))
                    {
                        xmlWriter.Formatting = Formatting.Indented;
                        xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                        xmlWriter.WriteStartElement("Folder");
                        
                        var imageset = (IImageSet)Earth3d.ImagesetHashTable[hash];
                        var alternateUrl = "";

                        try
                        {
                            if (File.Exists(imageset.Url))
                            {
                                alternateUrl = string.Format("http://{0}:5050/imageset/{1}/{2}", MyWebServer.IpAddress, hash, Path.GetFileName(imageset.Url));
                            }
                        }
                        catch
                        {
                        }
                        ImageSetHelper.SaveToXml(xmlWriter, imageset, alternateUrl);
                        xmlWriter.WriteFullEndElement();
                        xmlWriter.Close();
                    }
                    data = sb.ToString();
                }
            }
            catch
            {
            }
            

            SendHeaderAndData(data, ref socket, sMimeType);
        }
    }
}

