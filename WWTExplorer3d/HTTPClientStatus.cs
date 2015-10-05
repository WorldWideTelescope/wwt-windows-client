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
using System.Net;
using System.Net.Sockets;

namespace TerraViewer
{
    public class HttpClientStatus : RequestHandler
    {
        private string _sig = "get /status";
        public override bool Handles(string request)
        {
            return request.ToLower().StartsWith(_sig);
        }


        public override void ProcessRequest(string request, ref Socket socket, bool authenticated, string body)
        {
            var query = new QueryString(request);

            var sMimeType = "text/xml";

            var name = "";
            var nodeID = -1;
            float FPS = 0;
            string error = null;
            var status = ClientNodeStatus.Online;
            var StatusText = "";

            if (!string.IsNullOrEmpty(query["NodeID"]))
            {
                nodeID = int.Parse(query["NodeID"]);
            }

            if (!string.IsNullOrEmpty(query["NodeName"]))
            {
                name = query["NodeName"];
            }

            if (!string.IsNullOrEmpty(query["FPS"]))
            {
                FPS = float.Parse(query["FPS"]);
            }

            if (!string.IsNullOrEmpty(query["Error"]))
            {
                error = query["Error"];
            }

            if (!string.IsNullOrEmpty(query["Status"]))
            {
                status = (ClientNodeStatus)Enum.Parse(typeof(ClientNodeStatus), query["Status"]);
            }

            if (!string.IsNullOrEmpty(query["StatusText"]))
            {
                StatusText = query["StatusText"];
            }

            if (nodeID != -1 && !string.IsNullOrEmpty(name))
            {
                var ip = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();

                NetControl.LogStatusReport(nodeID, name, ip, status, StatusText, FPS, error);
            }
            
            var data = "<?xml version = \"1.0\" encoding=\"utf-8\"?>" +
                            "<Status>Ok</Status>";
            SendHeaderAndData(data, ref socket, sMimeType);
        }
    }
}

