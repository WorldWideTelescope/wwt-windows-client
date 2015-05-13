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
    public class HttpCrossDomain : RequestHandler
    {
        private string _sig = "get /clientaccesspolicy.xml";
        public override bool Handles(string request)
        {
            return request.ToLower().StartsWith(_sig);
        }


        public override void ProcessRequest(string request, ref Socket socket, bool authenticated, string body)
        {

            QueryString query = new QueryString(request);

            String sMimeType = "text/xml";

            string fileName = request.Substring(request.LastIndexOf("/")+1);

            String data = "<?xml version = \"1.0\" encoding=\"utf-8\"?>" +
                            "<access-policy>" +
                            "<cross-domain-access>" +
                            "<policy>" +
                            "<allow-from http-request-headers=\"*\">" +
                            "<domain uri=\"*\"/>" +
                            "</allow-from>" +
                            "<grant-to>" +
                            "<resource path=\"/\" include-subpaths=\"true\"/>" +
                            "</grant-to>" +
                            "</policy>" +
                            "</cross-domain-access>" +
                            "</access-policy>";
            SendHeaderAndData(data, ref socket, sMimeType);
        }
    }
}

