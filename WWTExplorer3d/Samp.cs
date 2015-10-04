using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;

namespace TerraViewer
{
    public class Samp
    {
        readonly Dictionary<string, string> dotSamp = new Dictionary<string, string>();
        public static Dictionary<string, VoTableLayer> sampKnownTableIds = new Dictionary<string, VoTableLayer>(); 
        public static Dictionary<string, VoTableLayer> sampKnownTableUrls = new Dictionary<string, VoTableLayer>();
        public Samp()
        {
            // Get .samp values
            InitDotSamp();

            // Gegister
            Register();
            

            DeclareMetadata();
            SetXmlrpcCallback(string.Format("http://{0}:5050/xmlrpc/", MyWebServer.IpAddress));
            SubscribeEvents();

            getSubscribedClients("table.load.votable");
        }

        private void InitDotSamp()
        {
            try
            {
                var personalFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                personalFolder = personalFolder.Substring(0, personalFolder.LastIndexOf("\\"));
                var filename = personalFolder + "\\.samp";
                if (!File.Exists(filename))
                {
                    return;
                }

                using (var sr = new StreamReader(filename))
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        if (!line.StartsWith("#"))
                        {
                            var split = line.Split(new[] { '=' });
                            dotSamp.Add(split[0], split[1]);
                        }
                    }
                }
                hubUrl = dotSamp["samp.hub.xmlrpc.url"];
                secret = dotSamp["samp.secret"];

            }
            catch
            {
            }
        }
        private bool connected;

        public bool Connected
        {
            get { return connected; }
            set { connected = value; }
        }
        string clientId = "";
        string hubUrl = "";
        string secret = "";
        public void Register()
        {
            if (string.IsNullOrEmpty(hubUrl))
            {
                return;
            }

            var callData = new StringBuilder();
            callData.Append("<?xml version=\"1.0\" ?>\r\n");
            callData.AppendLine("<methodCall>");
            callData.AppendLine("<methodName>samp.hub.register</methodName>");
            callData.AppendLine("<params>");
            callData.AppendLine("<param><value><string>"+secret+"</string></value></param>");
            callData.AppendLine("</params>");
            callData.AppendLine("</methodCall>");

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("User-Agent", "WWT");
                wc.Headers.Add("Content-Type", "text/xml");
                var rpcData = ASCIIEncoding.ASCII.GetBytes(callData.ToString());
                var response = wc.UploadData(hubUrl, "POST", rpcData);
                var text = ASCIIEncoding.ASCII.GetString(response);
                clientId = GetResponseParamValue(text, "samp.private-key");
                connected = true;
            }
            catch 
            {
                connected = false;
            }
            return ;
        } 
        public void Unregister()
        {
            if (!connected)
            {
                return;
            }
            var responseValue = "";
            var callData = new StringBuilder();
            callData.Append("<?xml version=\"1.0\" ?>\r\n");
            callData.AppendLine("<methodCall>");
            callData.AppendLine("<methodName>samp.hub.unregister</methodName>");
            callData.AppendLine("<params>");
            callData.AppendLine("<param><value><string>"+clientId+"</string></value></param>");
            callData.AppendLine("</params>");
            callData.AppendLine("</methodCall>");

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("User-Agent", "WWT");
                wc.Headers.Add("Content-Type", "text/xml");
                var rpcData = ASCIIEncoding.ASCII.GetBytes(callData.ToString());
                var response = wc.UploadData(hubUrl, "POST", rpcData);
                responseValue = ASCIIEncoding.ASCII.GetString(response);
            }
            catch 
            {

            }
            return;
        }
   
        public void DeclareMetadata()
        {
            if (!connected)
            {
                return;
            }
            var responseValue = "";
            var callData = new StringBuilder();
            callData.Append("<?xml version=\"1.0\" ?>\r\n");
            callData.AppendLine("<methodCall>");
            callData.AppendLine("<methodName>samp.hub.declareMetadata</methodName>");
            callData.AppendLine("<params>");
            callData.AppendLine("<param><value><string>" + clientId + "</string></value></param>");
            callData.AppendLine("<param><value><struct>");
            callData.AppendLine("<member><name>samp.name</name><value><string>WorldWideTelescope</string></value></member>");
            callData.AppendLine("<member><name>samp.description.text</name><value><string>Microsoft Research WorldWide Telescope Application</string></value></member>");
            callData.AppendLine("<member><name>samp.icon.url</name><value><string>http://www.worldwidetelescope.org/images/wwt_icon1.png</string></value></member>");
            callData.AppendLine("<member><name>worldwidetelescope.version</name><value><string>"+Assembly.GetExecutingAssembly().GetName().Version+"</string></value></member>");
            callData.AppendLine("<member><name>home.page</name><value><string>http://www.worldwidetelescope.org</string></value></member>");
            callData.AppendLine("</struct></value></param>");
            callData.AppendLine("</params>");
            callData.AppendLine("</methodCall>");

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("User-Agent", "WWT");
                wc.Headers.Add("Content-Type", "text/xml");
                var rpcData = ASCIIEncoding.ASCII.GetBytes(callData.ToString());
                var response = wc.UploadData(hubUrl, "POST", rpcData);
                responseValue = ASCIIEncoding.ASCII.GetString(response);
            }
            catch 
            {

            }
            return ;
        }

        public List<string> getSubscribedClients(string mType)
        {
            if (!connected)
            {
                return null;
            }
            var responseValue = "";
            var callData = new StringBuilder();
            callData.Append("<?xml version=\"1.0\" ?>\r\n");
            callData.AppendLine("<methodCall>");
            callData.AppendLine("<methodName>samp.hub.getSubscribedClients</methodName>");
            callData.AppendLine("<params>");
            callData.AppendLine("<param><value><string>" + clientId + "</string></value></param>");
            callData.AppendLine("<param><value><string>" + mType + "</string></value></param>");
            callData.AppendLine("</params>");
            callData.AppendLine("</methodCall>");
            var list = new List<string>();
            try
            {
                var wc = new WebClient();
                wc.Headers.Add("User-Agent", "WWT");
                wc.Headers.Add("Content-Type", "text/xml");
                var rpcData = ASCIIEncoding.ASCII.GetBytes(callData.ToString());
                var response = wc.UploadData(hubUrl, "POST", rpcData);
                responseValue = ASCIIEncoding.ASCII.GetString(response);
            }
            catch 
            {
                return null;
            }
            return  list;
        }

        public void GotoPoint(double ra, double dec)
        {
            if (!connected)
            {
                return;
            }
            
            var responseValue = "";
            var callData = new StringBuilder();
            callData.Append("<?xml version=\"1.0\" ?>\r\n");
            callData.AppendLine("<methodCall>");
            callData.AppendLine("<methodName>samp.hub.notifyAll</methodName>");
          //  callData.AppendLine("<methodName>coord.pointAt.sky</methodName>");
            callData.AppendLine("<params>");
            callData.AppendLine("<param><value><string>" + clientId + "</string></value></param>");
            callData.AppendLine("<param><value><struct>");
            callData.AppendLine("<member><name>samp.mtype</name><value><string>coord.pointAt.sky</string></value></member>");
            callData.AppendLine("<member><name>samp.params</name><value>");
            
            callData.AppendLine("<struct>");
            callData.AppendLine("<member><name>ra</name><value><string>" + (ra*15) + "</string></value></member>");
            callData.AppendLine("<member><name>dec</name><value><string>" + dec + "</string></value></member>");
            callData.AppendLine("</struct>");
            

            callData.AppendLine("</value></member>");
            callData.AppendLine("</struct></value></param>");
            callData.AppendLine("</params>");
            callData.AppendLine("</methodCall>");

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("User-Agent", "WWT");
                wc.Headers.Add("Content-Type", "text/xml");
                var rpcData = ASCIIEncoding.ASCII.GetBytes(callData.ToString());
                var response = wc.UploadData(hubUrl, "POST", rpcData);
                responseValue = ASCIIEncoding.ASCII.GetString(response);
            }
            catch 
            {

            }
            return;
        }
        public void SetXmlrpcCallback( string url)
        {
            if (!connected)
            {
                return;
            }
            
            var responseValue = "";
            var callData = new StringBuilder();
            callData.Append("<?xml version=\"1.0\" ?>\r\n");
            callData.AppendLine("<methodCall>");
            callData.AppendLine("<methodName>samp.hub.setXmlrpcCallback</methodName>");
            callData.AppendLine("<params>");
            callData.AppendLine("<param><value><string>" + clientId + "</string></value></param>");
            callData.AppendLine("<param><value><string>" + url + "</string></value></param>");
            callData.AppendLine("</params>");
            callData.AppendLine("</methodCall>");

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("User-Agent", "WWT");
                wc.Headers.Add("Content-Type", "text/xml");
                var rpcData = ASCIIEncoding.ASCII.GetBytes(callData.ToString());
                var response = wc.UploadData(hubUrl, "POST", rpcData);
                responseValue = ASCIIEncoding.ASCII.GetString(response);
            }
            catch 
            {

            }
            return;
        }
        public void LoadTable(string url, string id, string name)
        {
            if (!connected)
            {
                return;
            }

            var responseValue = "";
            var callData = new StringBuilder();
            callData.Append("<?xml version=\"1.0\" ?>\r\n");
            callData.AppendLine("<methodCall>");
            callData.AppendLine("<methodName>samp.hub.notifyAll</methodName>");
            //  callData.AppendLine("<methodName>coord.pointAt.sky</methodName>");
            callData.AppendLine("<params>");
            callData.AppendLine("<param><value><string>" + clientId + "</string></value></param>");
            callData.AppendLine("<param><value><struct>");
            callData.AppendLine("<member><name>samp.mtype</name><value><string>table.load.votable</string></value></member>");
            callData.AppendLine("<member><name>samp.params</name><value>");

            callData.AppendLine("<struct>");
            callData.AppendLine("<member><name>url</name><value><string>" + url + "</string></value></member>");
            callData.AppendLine("<member><name>table-id</name><value><string>" + id + "</string></value></member>");
            callData.AppendLine("<member><name>name</name><value><string>" + name + "</string></value></member>");
            callData.AppendLine("</struct>");


            callData.AppendLine("</value></member>");
            callData.AppendLine("</struct></value></param>");
            callData.AppendLine("</params>");
            callData.AppendLine("</methodCall>");
            try
            {
                var wc = new WebClient();
                wc.Headers.Add("User-Agent", "WWT");
                wc.Headers.Add("Content-Type", "text/xml");
                var rpcData = ASCIIEncoding.ASCII.GetBytes(callData.ToString());
                var response = wc.UploadData(hubUrl, "POST", rpcData);
                responseValue = ASCIIEncoding.ASCII.GetString(response);
            }
            catch
            {

            }
            return;
        }

        public void TableHighlightRow(string url, string id, int row)
        {
            if (!connected)
            {
                return;
            }

            var responseValue = "";
            var callData = new StringBuilder();
            callData.Append("<?xml version=\"1.0\" ?>\r\n");
            callData.AppendLine("<methodCall>");
            callData.AppendLine("<methodName>samp.hub.notifyAll</methodName>");
            
            callData.AppendLine("<params>");
            callData.AppendLine("<param><value><string>" + clientId + "</string></value></param>");
            callData.AppendLine("<param><value><struct>");
            callData.AppendLine("<member><name>samp.mtype</name><value><string>table.highlight.row</string></value></member>");
            callData.AppendLine("<member><name>samp.params</name><value>");

            callData.AppendLine("<struct>");
            if (!string.IsNullOrEmpty(url))
            {
                callData.AppendLine("<member><name>url</name><value><string>" + url + "</string></value></member>");
            }
            callData.AppendLine("<member><name>table-id</name><value><string>" + id + "</string></value></member>");
            callData.AppendLine("<member><name>row</name><value><string>" + row + "</string></value></member>");
            callData.AppendLine("</struct>");


            callData.AppendLine("</value></member>");
            callData.AppendLine("</struct></value></param>");
            callData.AppendLine("</params>");
            callData.AppendLine("</methodCall>");
            try
            {
                var wc = new WebClient();
                wc.Headers.Add("User-Agent", "WWT");
                wc.Headers.Add("Content-Type", "text/xml");
                var rpcData = ASCIIEncoding.ASCII.GetBytes(callData.ToString());
                var response = wc.UploadData(hubUrl, "POST", rpcData);
                responseValue = ASCIIEncoding.ASCII.GetString(response);
            }
            catch
            {

            }
            return;
        }

        public void LoadImageFits(string url, string id, string name)
        {
            if (!connected)
            {
                return;
            }

            var responseValue = "";
            var callData = new StringBuilder();
            callData.Append("<?xml version=\"1.0\" ?>\r\n");
            callData.AppendLine("<methodCall>");
            callData.AppendLine("<methodName>samp.hub.notifyAll</methodName>");
            //  callData.AppendLine("<methodName>coord.pointAt.sky</methodName>");
            callData.AppendLine("<params>");
            callData.AppendLine("<param><value><string>" + clientId + "</string></value></param>");
            callData.AppendLine("<param><value><struct>");
            callData.AppendLine("<member><name>samp.mtype</name><value><string>image.load.fits</string></value></member>");
            callData.AppendLine("<member><name>samp.params</name><value>");

            callData.AppendLine("<struct>");
            callData.AppendLine("<member><name>url</name><value><string>" + url + "</string></value></member>");
            callData.AppendLine("<member><name>image-id</name><value><string>" + id + "</string></value></member>");
            callData.AppendLine("<member><name>name</name><value><string>" + name + "</string></value></member>");
            callData.AppendLine("</struct>");


            callData.AppendLine("</value></member>");
            callData.AppendLine("</struct></value></param>");
            callData.AppendLine("</params>");
            callData.AppendLine("</methodCall>");
            try
            {
                var wc = new WebClient();
                wc.Headers.Add("User-Agent", "WWT");
                wc.Headers.Add("Content-Type", "text/xml");
                var rpcData = ASCIIEncoding.ASCII.GetBytes(callData.ToString());
                var response = wc.UploadData(hubUrl, "POST", rpcData);
                responseValue = ASCIIEncoding.ASCII.GetString(response);
            }
            catch
            {

            }
            return;
        }
        public void SubscribeEvents()
        {
            if (!connected)
            {
                return;
            }
            var responseValue = "";
            var callData = new StringBuilder();
            callData.Append("<?xml version=\"1.0\" ?>\r\n");
            callData.AppendLine("<methodCall>");
            callData.AppendLine("<methodName>samp.hub.declareSubscriptions</methodName>");
            callData.AppendLine("<params>");
            callData.AppendLine("<param><value><string>" + clientId + "</string></value></param>");
            callData.AppendLine("<param><value><struct>");
            callData.AppendLine("<member><name>samp.hub.*</name><value><struct /></value></member>");
            callData.AppendLine("<member><name>coord.*</name><value><struct /></value></member>");
            callData.AppendLine("<member><name>table.load.votable</name><value><struct /></value></member>");
            callData.AppendLine("<member><name>image.load.fits</name><value><struct /></value></member>");
            callData.AppendLine("<member><name>table.highlight.row</name><value><struct /></value></member>");
            callData.AppendLine("</struct></value></param>");
            callData.AppendLine("</params>");
            callData.AppendLine("</methodCall>");

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("User-Agent", "WWT");
                wc.Headers.Add("Content-Type", "text/xml");
                var rpcData = ASCIIEncoding.ASCII.GetBytes(callData.ToString());
                var response = wc.UploadData(hubUrl, "POST", rpcData);
                responseValue = ASCIIEncoding.ASCII.GetString(response);
            }
            catch
            {

            }
            return;
        }

        static string GetResponseParamValue(string xml, string key)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNode node;
                var root = doc.DocumentElement;
                node = root.SelectSingleNode("/methodResponse/params/param/value/struct/member[name='" + key + "']");
                XmlNode val = node["value"];
                //XmlNode node = doc["methodResponse"];

                return val.InnerText;
            }
            catch
            {
                return null;
            }
        }
    }
}
