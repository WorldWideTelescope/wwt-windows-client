using System;
using System.Collections.Generic;
using System.Xml;
using System.Drawing;
using System.IO;
using System.Threading;

namespace TerraViewer
{
    public class KmlCollection
    {
        public List<KmlRoot> Roots = new List<KmlRoot>();

        public void Add(KmlRoot root)
        {
            Roots.Add(root);
        }
        
        public static void Save(string filename)
        {
        }
        
        public static void Load(string filename)
        {
        }

        public void UpdateNetworkLinks(KmlViewInformation viewInfo)
        {

            foreach (var root in Roots)
            {
                UpdateRootLinks(root, viewInfo);

            }
        }

        public static void UpdateRootLinks(KmlRoot root, KmlViewInformation viewInfo)
        {
            if (root.children != null)
            {
                foreach (var child in root.children)
                {
                    UpdateLinks(child, viewInfo);
                }
            }
        }

        private static void UpdateLinks(KmlFeature feature, KmlViewInformation viewInfo)
        {
            if (feature.visibility)
            {
                if (feature is KmlContainer)
                {
                    var container = (KmlContainer)feature;
                    if (container.children != null)
                    {
                        foreach (var child in container.children)
                        {
                            UpdateLinks(child, viewInfo);
                        }
                    }
                }
                else
                {
                    if (feature is KmlNetworkLink)
                    {
                        var netLink = (KmlNetworkLink)feature;
                        netLink.ConditionalUpdate(viewInfo);
                        if (netLink.LinkRoot != null)
                        {
                            UpdateRootLinks(netLink.LinkRoot, viewInfo);
                        }
                    }
                }
            }
        }
    }

    public enum KmlLoadStatus { NotLoaded, Loaded, Failed, PendingUpdate }

    public class KmlRoot
    {
        private bool sky;

        public KmlLoadStatus LoadStatus = KmlLoadStatus.NotLoaded;

        public bool Sky
        {
            get { return sky; }
            set { sky = value; }
        }

        DateTime lastUpdate;

        public DateTime LastUpdate
        {
            get { return lastUpdate; }
            set { lastUpdate = value; }
        }

        public KmlTimeSpan TimeSpan;

        public void UpdateTimeSpanRange(KmlTimeSpan span)
        {
            if (!span.UnBoundedBegin)
            {
                if (TimeSpan.BeginTime > span.BeginTime)
                {
                    TimeSpan.BeginTime = span.BeginTime;
                    TimeSpan.UnBoundedBegin = false;
                }

                if (TimeSpan.EndTime < span.BeginTime)
                {
                    TimeSpan.EndTime = span.BeginTime;
                    TimeSpan.UnBoundedEnd = false;
                }

            }

            if (!span.UnBoundedEnd)
            {
                if (TimeSpan.EndTime < span.EndTime)
                {
                    TimeSpan.EndTime = span.EndTime;
                    TimeSpan.UnBoundedEnd = false;
                }

                if (TimeSpan.BeginTime > span.EndTime)
                {
                    TimeSpan.BeginTime = span.EndTime;
                    TimeSpan.UnBoundedBegin = false;
                }

            }
        }

        public KmlRoot Owner = null;
        public Uri BaseUri = null;
        public ZipArchive Archive = null;
        public Dictionary<string, KmlStyleSelector> Styles = new Dictionary<string, KmlStyleSelector>();

        public KmlRoot(string filename, KmlRoot owner)
        {
            TimeSpan = new KmlTimeSpan();
            // Set high and low points.
            TimeSpan.BeginTime = new DateTime(3999, 1, 1);
            TimeSpan.EndTime = new DateTime(1, 1, 1);


            Owner = owner;
            if (Uri.IsWellFormedUriString(filename, UriKind.Absolute))
            {
                BaseUri = new Uri(filename);
            }

            if (!File.Exists(filename) && owner != null & owner.BaseUri != null)
            {
                var newUri = new Uri(owner.BaseUri, filename);
                filename = newUri.ToString();
            }

            var doc = new XmlDocument();
            var NamespaceManager = new XmlNamespaceManager(doc.NameTable);
            NamespaceManager.AddNamespace("atom", "http://www.w3.org/2005/Atom");

            if (filename.ToLower().Contains(".kmz"))
            {
                if (Uri.IsWellFormedUriString(filename, UriKind.Absolute))
                {
                    var fs = UiTools.GetMemoryStreamFromUrl(filename);
                    Archive = new ZipArchive(fs);
                    foreach (var entry in Archive.Files)
                    {
                        if (entry.Filename.ToLower().EndsWith(".kml"))
                        {
                            doc.Load(entry.GetFileStream());
                        }
                    }
                }
                else
                {
                    // using (FileStream fs = new FileStream(filename, FileMode.Open))
                    var fs = new FileStream(filename, FileMode.Open);
                    {
                        var archive = new ZipArchive(fs);
                        foreach (var entry in archive.Files)
                        {
                            if (entry.Filename.ToLower().EndsWith(".kml"))
                            {
                                doc.Load(entry.GetFileStream());
                                Archive = archive;
                            }
                        }
                    }
                }
            }
            else
            {
                try
                {
                    //todo this really needs to be fixed.
                    doc.Load(filename);
                }
                catch
                {
                }
            }
            XmlNode kml = doc["kml"];
            if (kml == null)
            {
                return;
            }

            if (kml.Attributes["hint"] != null)
            {
                if (kml.Attributes["hint"].InnerText.ToLower().Contains("sky"))
                {
                    sky = true;
                }
            }

            LoadDetails(kml);
            LoadStatus = KmlLoadStatus.Loaded;
        }

        public KmlRoot(string url, string tempDirectory)
        {
            // load from URL
        }

        public List<KmlFeature> children = new List<KmlFeature>();
        public KmlDocument Document = null;
        public void LoadDetails(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                KmlFeature newChild = null;
                switch (child.Name)
                {
                    case "NetworkLink":
                        newChild = new KmlNetworkLink();
                        break;
                    case "Placemark":
                        newChild = new KmlPlacemark();
                        break;
                    case "PhotoOverlay":
                        newChild = new KmlPhotoOverlay();
                        break;
                    case "ScreenOverlay":
                        newChild = new KmlScreenOverlay();
                        break;
                    case "GroundOverlay":
                        newChild = new KmlGroundOverlay();
                        break;
                    case "Folder":
                        newChild = new KmlFolder();
                        break;
                    case "Document":
                        newChild = Document = new KmlDocument();
                        break;
                }

                if (newChild != null)
                {
                    newChild.sky = sky;
                    newChild.LoadDetails(child, this);
                    
                    children.Add(newChild);
                }
            }

            lastUpdate = DateTime.Now;

        }

        internal Stream GetFileStream(string href)
        {
            Stream result = null;

            result = GetArchiveStream(href);

            if (result == null)
            {
                if (Uri.IsWellFormedUriString(href, UriKind.Absolute))
                {
                    return UiTools.GetMemoryStreamFromUrl(href);
                }
                if (File.Exists(href))
                {
                    return File.Open(href, FileMode.Open);
                }
                if (BaseUri != null)
                {
                    var newUri = new Uri(BaseUri, href);
                    return UiTools.GetMemoryStreamFromUrl(newUri.ToString());
                }
            }
            return result;
        }

        internal string GetStreamUrl(string href)
        {
            var result = href;
            if (CheckArchiveStream(href))
            {
                // This comes from the internal Zip file
                if (BaseUri != null)
                {
                    result = BaseUri + ":" + href;
                }
                else
                {
                    result = href;
                }
            }

            if (result == null)
            {
                if (Uri.IsWellFormedUriString(href, UriKind.Absolute))
                {
                    return href;
                }
                if (File.Exists(href))
                {
                    return href;
                }
                if (BaseUri != null)
                {
                    var newUri = new Uri(BaseUri, href);
                    return newUri.ToString();
                }
            }

            return result;
        }

        private Stream GetArchiveStream(string href)
        {
            if (Archive != null)
            {
                foreach (var file in Archive.Files)
                {
                    if (file.Filename.ToLower() == href.ToLower())
                    {
                        return file.GetFileStream();
                    }
                }
            }
            return null;
        }

        private bool CheckArchiveStream(string href)
        {
            if (Archive != null)
            {
                foreach (var file in Archive.Files)
                {
                    if (file.Filename.ToLower() == href.ToLower())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public class KmlObject
    {
        public string ID;

        public KmlRoot Owner = null;

        public virtual void LoadDetails(XmlNode node, KmlRoot owner)
        {

            Owner = owner;
            if (node.Attributes["id"] != null)
            {
                ID = node.Attributes["id"].Value;
            }  
        }

        public static Color GetColor(XmlNode node)
        {
            try
            {
                if (node["color"] != null)
                {

                    var abgr = Convert.ToInt32(node["color"].InnerText, 16);

                    var alpha = (byte)(abgr >> 24);
                    var blue = (byte)(abgr >> 16);
                    var green = (byte)(abgr >> 8);
                    var red = (byte)(abgr);
                    return Color.FromArgb(alpha, red, green, blue);
                }
            }
            catch
            {
            }
            return Color.White;
        }

        public static Color GetBgColor(XmlNode node)
        {
            if (node["bgColor"] != null)
            {
                var abgr = Convert.ToInt32(node["bgColor"].InnerText, 16);

                var alpha = (byte)(abgr >> 24);
                var blue = (byte)(abgr >> 16);
                var green = (byte)(abgr >> 8);
                var red = (byte)(abgr);
                return Color.FromArgb(alpha, red, green, blue);
            }
            return Color.White;
        }

        public static bool GetBoolValue(XmlNode node, string name)
        {
            if (node[name] != null)
            {
                if (node[name].InnerText.Trim() == "1")
                {
                    return true;
                }

            }
            return false;
        }

        public static KmlColorModes GetColorMode(XmlNode node)
        {
            if (node["colorMode"] != null)
            {
                if (node["colorMode"].InnerText == "random")
                {
                    return KmlColorModes.Random;
                }

            }
            return KmlColorModes.Normal;
        }
    }

    //<Link id="ID">
    //  <!-- specific to Link -->
    //  <href>...</href>                      <!-- string -->
    //  <refreshMode>onChange</refreshMode>   
    //    <!-- refreshModeEnum: onChange, onInterval, or onExpire -->   
    //  <refreshInterval>4</refreshInterval>  <!-- float -->
    //  <viewRefreshMode>never</viewRefreshMode> 
    //    <!-- viewRefreshModeEnum: never, onStop, onRequest, onRegion -->
    //  <viewRefreshTime>4</viewRefreshTime>  <!-- float -->
    //  <viewBoundScale>1</viewBoundScale>    <!-- float -->
    //  <viewFormat>BBOX=[bboxWest],[bboxSouth],[bboxEast],[bboxNorth]</viewFormat>
    //                                        <!-- string -->
    //  <httpQuery>...</httpQuery>            <!-- string -->
    //</Link>
    public enum refreshModeEnum { onChange, onInterval, onExpire }
    public enum viewRefreshModeEnum { never, onStop, onRequest, onRegion }

    public class KmlLink : KmlObject
    {
        public string Href;
        public refreshModeEnum RefreshMode = refreshModeEnum.onChange;
        public float RefreshInterval=0;
        public viewRefreshModeEnum ViewRefreshMode = viewRefreshModeEnum.never;
        public float ViewRefreshTime=0;
        public float ViewBoundScale=1;
        public string ViewFormat = "BBOX=[bboxWest],[bboxSouth],[bboxEast],[bboxNorth]";
        public string HttpQuery;

        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);
            if (node["href"] != null)
            {
                Href = node["href"].InnerText;
            }

            if (node["refreshMode"] != null)
            {
                RefreshMode = (refreshModeEnum)Enum.Parse(typeof(refreshModeEnum), node["refreshMode"].InnerText);
            }

            if (node["refreshInterval"] != null)
            {
                RefreshInterval = Convert.ToSingle(node["refreshInterval"].InnerText);
            }  

            if (node["viewRefreshMode"] != null)
            {
                ViewRefreshMode = (viewRefreshModeEnum)Enum.Parse(typeof(viewRefreshModeEnum), node["viewRefreshMode"].InnerText);
            }

            if (node["viewRefreshTime"] != null)
            {
                ViewRefreshTime = Convert.ToSingle(node["viewRefreshTime"].InnerText);
            }

            if (node["viewBoundScale"] != null)
            {
                ViewBoundScale = Convert.ToSingle(node["viewBoundScale"].InnerText);
            }  

            if (node["viewFormat"] != null)
            {
                ViewFormat = node["viewFormat"].InnerText;
            }   

            if (node["httpQuery"] != null)
            {
                HttpQuery = node["httpQuery"].InnerText;
            }    
        }

    }

    //<Region id="ID"> 
    //  <LatLonAltBox> 
    //    <north></north>                            <!-- required; kml:angle90 -->
    //    <south></south>                            <!-- required; kml:angle90 --> 
    //    <east></east>                              <!-- required; kml:angle180 -->
    //    <west></west>                              <!-- required; kml:angle180 -->
    //    <minAltitude>0</minAltitude>               <!-- float -->
    //    <maxAltitude>0</maxAltitude>               <!-- float -->
    //    <altitudeMode>clampToGround</altitudeMode> 
    //      <!-- kml:altitudeModeEnum: clampToGround, relativeToGround, or absolute --> 
    //  </LatLonAltBox> 
    //  <Lod>
    //    <minLodPixels>0</minLodPixels>             <!-- float -->
    //    <maxLodPixels>-1</maxLodPixels>            <!-- float -->
    //    <minFadeExtent>0</minFadeExtent>           <!-- float --> 
    //    <maxFadeExtent>0</maxFadeExtent>           <!-- float -->
    //  </Lod>
    //</Region> 
    public class KmlRegion : KmlObject
    {
        public double north;
        public double south;
        public double east;
        public double west;
        public double minAltitude;
        public double maxAltitude;
        public altitudeModeEnum altitudeMode = altitudeModeEnum.clampToGround;

        public float minLodPixels;
        public float maxLodPixels=-1;
        public float minFadeExtent;
        public float maxFadeExtent;

        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);


            if (node["LatLonAltBox"] != null)
            {
                XmlNode box = node["LatLonAltBox"];
                if (box["north"] != null)
                {
                    north = Convert.ToDouble(box["north"].InnerText);
                }

                if (box["south"] != null)
                {
                    south = Convert.ToDouble(box["south"].InnerText);
                }

                if (box["east"] != null)
                {
                    east = Convert.ToDouble(box["east"].InnerText);
                }

                if (box["west"] != null)
                {
                    west = Convert.ToDouble(box["west"].InnerText);
                }

                if (box["minAltitude"] != null)
                {
                    minAltitude = Convert.ToDouble(box["minAltitude"].InnerText);
                }

                if (box["maxAltitude"] != null)
                {
                    maxAltitude = Convert.ToDouble(box["maxAltitude"].InnerText);
                }

                if (box["altitudeMode"] != null)
                {
                    altitudeMode = (altitudeModeEnum)Enum.Parse(typeof(altitudeModeEnum), box["altitudeMode"].InnerText);
                }

            }

            if (node["Lod"] != null)
            {
                XmlNode lod = node["Lod"];
                if (lod["minLodPixels"] != null)
                {
                    minLodPixels = Convert.ToSingle(lod["minLodPixels"].InnerText);
                }
                if (lod["maxLodPixels"] != null)
                {
                    maxLodPixels = Convert.ToSingle(lod["maxLodPixels"].InnerText);
                }
                if (lod["minFadeExtent"] != null)
                {
                    minFadeExtent = Convert.ToSingle(lod["minFadeExtent"].InnerText);
                }
                if (lod["maxFadeExtent"] != null)
                {
                    maxFadeExtent = Convert.ToSingle(lod["maxFadeExtent"].InnerText);
                }  
            }  
        }

    }


    public class KmlTimePrimitive
    {

        
    }

    public class KmlTimeSpan : KmlTimePrimitive
    {
        public DateTime BeginTime = new DateTime(1, 1, 1);
        public DateTime EndTime = new DateTime(3999, 1, 1);
        public bool UnBoundedBegin = true;
        public bool UnBoundedEnd = true;
        public void LoadDetails(XmlElement node, KmlRoot owner)
        {
            if (node["begin"] != null)
            {
                UnBoundedBegin = !DateTime.TryParse(node["begin"].InnerText, out BeginTime);
                if (UnBoundedBegin)
                {
                    var year = 0;
                    if (int.TryParse(node["begin"].InnerText, out year))
                    {
                        year = Math.Max(1, Math.Min(3999,year));
                        BeginTime = new DateTime(year, 1, 1);
                        UnBoundedBegin = false;
                    }
                }
                if (BeginTime.Kind == DateTimeKind.Local)
                {
                    BeginTime = BeginTime.ToUniversalTime();
                }

            }
            // alternative timestamp
            if (node["when"] != null)
            {
                UnBoundedBegin = !DateTime.TryParse(node["when"].InnerText, out BeginTime);
                if (UnBoundedBegin)
                {
                    var year = 0;
                    if (int.TryParse(node["when"].InnerText, out year))
                    {
                        year = Math.Max(1, Math.Min(3999, year));
                        BeginTime = new DateTime(year, 1, 1);
                        UnBoundedBegin = false;
                    }
                }
                if (BeginTime.Kind == DateTimeKind.Local)
                {
                    BeginTime = BeginTime.ToUniversalTime();
                }

            }

            if (node["end"] != null)
            {
                UnBoundedEnd = !DateTime.TryParse(node["end"].InnerText, out EndTime);
                if (UnBoundedEnd)
                {
                    var year = 0;
                    if (int.TryParse(node["end"].InnerText, out year))
                    {
                        year = Math.Max(1, Math.Min(3999,year));
                        EndTime = new DateTime(year, 12, 31, 23, 59, 59);
                        UnBoundedEnd = false;
                    }
                }
                if (EndTime.Kind == DateTimeKind.Local)
                {
                    EndTime = EndTime.ToUniversalTime();
                }
            }

        }
    }

    //<!-- abstract element; do not create -->
    //<!-- Feature id="ID" -->                <!-- Document,Folder,
    //                                         NetworkLink,Placemark,
    //                                         GroundOverlay,PhotoOverlay,ScreenOverlay -->
    //<name>...</name>                      <!-- string -->
    //<visibility>1</visibility>            <!-- boolean -->
    //<open>0</open>                        <!-- boolean -->
    //<atom:author>...<atom:author>         <!-- xmlns:atom -->
    //<atom:link>...</atom:link>            <!-- xmlns:atom -->
    //<address>...</address>                <!-- string -->
    //<xal:AddressDetails>...</xal:AddressDetails>  <!-- xmlns:xal -->  <phoneNumber>...</phoneNumber>        <!-- string -->  <Snippet maxLines="2">...</Snippet>   <!-- string -->
    //<description>...</description>        <!-- string -->
    //<AbstractView>...</AbstractView>      <!-- Camera or LookAt -->
    //<TimePrimitive>...</TimePrimitive>
    //<styleUrl>...</styleUrl>              <!-- anyURI -->
    //<StyleSelector>...</StyleSelector>
    //<Region>...</Region>
    //<Metadata>...</Metadata>              <!-- deprecated in KML 2.2 -->
    //<ExtendedData>...</ExtendedData>      <!-- new in KML 2.2 --><-- /Feature -->
    public class KmlFeature : KmlObject
    {
        public bool sky = false;
        private string name;

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    return "[no name]";
                }
                return name;
            }
            set { name = value; }
        }
        public bool visibility = true;
        public bool open = false;
        public string atom_author;
        public string atom_link;
        public string address;
        public string xal_AddressDetails;
        public string description;
        public string Snippet;
        public KmlRegion region;
        public KmlLookAt LookAt;
        public KmlTimeSpan Time = new KmlTimeSpan();

        public bool Dirty = true;

        public KmlStyleSelector Style = new KmlStyle();

        public bool ShouldDisplay()
        {
            var display = visibility;
            var now = SpaceTimeController.Now;


            if (!Time.UnBoundedBegin && Time.BeginTime > now)
            {
                return false;
            }

            if (!Time.UnBoundedEnd && Time.EndTime < now)
            {
                return false;
            }

            return display;
        }

        // todo Define remainder of vars
        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            if (node["name"] != null)
            {
                Name = node["name"].InnerText;
            }  
            
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "Style")
                {
                    var style = new KmlStyle();
                    style.LoadDetails(child, owner);
                    Style = style;
                    if (!string.IsNullOrEmpty(style.ID))
                    {
                        owner.Styles.Add(style.ID, style);
                    }
                }
                if (child.Name == "StyleMap")
                {
                    var style = new KmlStyleMap();
                    style.LoadDetails(child, owner);
                    if (!string.IsNullOrEmpty(style.ID))
                    {
                        owner.Styles.Add(style.ID, style);
                    }
                }       
            }

            base.LoadDetails(node, owner);

            if (node["open"] != null)
            {
                open = node["open"].InnerText.Trim() == "1";
            }

            if (node["visibility"] != null)
            {
                visibility = node["visibility"].InnerText.Trim() == "1";
            }

            if (node["atom:author"] != null)
            {
                atom_author = node["atom:author"].InnerText;
            }

            if (node["atom:link"] != null)
            {
                atom_link = node["atom:link"].InnerText;
            }

            if (node["address"] != null)
            {
                address = node["address"].InnerText;
            }

            if (node["xal:AddressDetails"] != null)
            {
                xal_AddressDetails = node["xal:AddressDetails"].InnerText;
            }

            if (node["description"] != null)
            {
                description = node["description"].InnerText;
            }

            if (node["Snippet"] != null)
            {
                Snippet = node["Snippet"].InnerText;
            }

            if (node["Region"] != null)
            {    
                region = new KmlRegion();
                region.LoadDetails(node["Region"], owner);
            }
            if (node["LookAt"] != null)
            {    
                LookAt = new KmlLookAt();
                LookAt.LoadDetails(node["LookAt"], owner);
            }

            if (node["TimeSpan"] != null)
            {
                Time = new KmlTimeSpan();
                Time.LoadDetails(node["TimeSpan"], owner);
                owner.UpdateTimeSpanRange(Time);
            }

            if (node["TimeStamp"] != null)
            {
                Time = new KmlTimeSpan();
                Time.LoadDetails(node["TimeStamp"], owner);
                owner.UpdateTimeSpanRange(Time);
            }

            if (node["styleUrl"] != null)
            {
                var url = node["styleUrl"].InnerText;

                if (url.StartsWith("#"))
                {
                    // Internal reference
                    if (owner != null)
                    {
                        if (owner.Document != null)
                        {
                            Style = owner.Styles[url.Remove(0, 1)];
                        }
                    }
                }
            }

            if (node["StyleSelector"] != null)
            {
                Style = new KmlStyle();
                if (node["StyleSelector"]["Style"] != null)
                {
                    Style.LoadDetails(node["StyleSelector"]["Style"], owner);
                }

                //todo add stle options

            }
            //todo finish up all of this ! Missing fields and types galore
        }
    }


    //[bboxWest]
    //[bboxSouth]
    //[bboxEast]
    //[bboxNorth]
    //[lookatLon]
    //[lookatLat]
    //[lookatRange]
    //[lookatTilt]
    //[lookatHeading]
    //[lookatTerrainLon]
    //[lookatTerrainLat]
    //[lookatTerrainAlt]
    //[cameraLon]
    //[cameraLat]
    //[cameraAlt]
    //[horizFov]
    //[vertFov]
    //[horizPixels]
    //[vertPixels]
    //[terrainEnabled] 

    //BBOX=[bboxWest],[bboxSouth],[bboxEast],[bboxNorth]
        //    [lookatLon]
        //[lookatLat]
        //[lookatRange]
        //[lookatTilt]
        //[lookatHeading]
        //[lookatTerrainLon]
        //[lookatTerrainLat]
        //[lookatTerrainAlt]
        //[cameraLon]
        //[cameraLat]
        //[cameraAlt]
        //[horizFov]
        //[vertFov]
        //[horizPixels]
        //[vertPixels]
        //[terrainEnabled] 
    //This information matches the Web Map Service (WMS) bounding box specification. 
    //If you specify an empty <viewFormat> tag, no information is appended to the query string. 
    //You can also specify a custom set of viewing parameters to add to the query string. If you supply a format string, it is used instead of the BBOX information. If you also want the BBOX information, you need to add those parameters along with the custom parameters. 
    //You can use any of the following parameters in your format string (and Google Earth will substitute the appropriate current value at the time it creates the query string): 
    //[lookatLon], [lookatLat] - longitude and latitude of the point that <LookAt> is viewing 
    //[lookatRange], [lookatTilt], [lookatHeading] - values used by the <LookAt> element (see descriptions of <range>, <tilt>, and <heading> in <LookAt>) 
    //[lookatTerrainLon], [lookatTerrainLat], [lookatTerrainAlt] - point on the terrain in degrees/meters that <LookAt> is viewing 
    //[cameraLon], [cameraLat], [cameraAlt] - degrees/meters of the eyepoint for the camera 
    //[horizFov], [vertFov] - horizontal, vertical field of view for the camera 
    //[horizPixels], [vertPixels] - size in pixels of the 3D viewer 
    //[terrainEnabled] - indicates whether the 3D viewer is showing terrain 

    public class KmlViewInformation
    {
        public double bboxWest;
        public double bboxSouth;
        public double bboxEast;
        public double bboxNorth;

        public double lookatLon;
        public double lookatLat;
        public double lookatRange;
        public double lookatTilt;

        public double lookatHeading;
        public double lookatTerrainLon;
        public double lookatTerrainLat;
        public double lookatTerrainAlt;
        public double cameraLon;
        public double cameraLat;
        public double cameraAlt;
        public double horizFov;
        public double vertFov;
        public double horizPixels;
        public double vertPixels;
        public double terrainEnabled;

        public bool viewJustStopped = false;
        public bool viewMoving = false;

        static string[] parameterList;

        public static string[] ParameterList
        {
            get
            {
                if (parameterList == null)
                {
                    parameterList = new[]
                                {
                                    "[bboxWest]",
                                    "[bboxSouth]",
                                    "[bboxEast]",
                                    "[bboxNorth]",
                                    "[lookatLon]",
                                    "[lookatLat]",
                                    "[lookatRange]",
                                    "[lookatTilt]",
                                    "[lookatHeading]",
                                    "[lookatTerrainLon]",
                                    "[lookatTerrainLat]",
                                    "[lookatTerrainAlt]",
                                    "[cameraLon]",
                                    "[cameraLat]",
                                    "[cameraAlt]",
                                    "[horizFov]",
                                    "[vertFov]",
                                    "[horizPixels]",
                                    "[vertPixels]",
                                    "[terrainEnabled]"
                                };
                }
                return parameterList;
            
            }
        }

        public string PrepLinkUrl(string inputLink)
        {
            var list = ParameterList;

            // converts string from KML place holders to c# compatible ones.
            for (var i = 0; i < list.Length; i++)
            {
                inputLink = inputLink.Replace(list[i], "{" + i + "}");
            }

            return String.Format(inputLink, bboxWest, bboxSouth, bboxEast, bboxNorth, lookatLon, lookatLat, lookatRange, lookatTilt, lookatHeading, lookatTerrainLon,
                                            lookatTerrainLat, lookatTerrainAlt, cameraLon, cameraLat, cameraAlt, horizFov, vertFov, horizPixels, vertPixels, terrainEnabled);


        }
    }   

    //<NetworkLink id="ID">

    //  <!-- specific to NetworkLink -->
    //  <refreshVisibility>0</refreshVisibility> <!-- boolean -->
    //  <flyToView>0</flyToView>                 <!-- boolean -->
    //  <Link>...</Link>
    //</NetworkLink>
    public class KmlNetworkLink : KmlFeature
    {
        bool refreshVisibility;
        bool flyToView;
        KmlLink Link;

        DateTime lastUpdate;
        DateTime stopTime;
        bool pendingUpdate;

        public KmlLoadStatus LoadStatus = KmlLoadStatus.NotLoaded;

        // The result of the network link
        public KmlRoot LinkRoot = null;

        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);
            if (node["refreshVisibility"] != null)
            {
                refreshVisibility = node["refreshVisibility"].InnerText.Trim() == "1";
            }

            if (node["flyToView"] != null)
            {
                flyToView = node["flyToView"].InnerText.Trim() == "1";
            }

            if (node["Link"] != null)
            {
                Link = new KmlLink();
                Link.LoadDetails(node["Link"], owner);
            }
            else if (node["Url"] != null)
            {
                Link = new KmlLink();
                Link.LoadDetails(node["Url"], owner);
            }

        }

        internal void ConditionalUpdate(KmlViewInformation viewInfo)
        {
            if (Link != null)
            {
                if ((Link.RefreshMode == refreshModeEnum.onInterval ) && DateTime.Now.Subtract(lastUpdate).TotalSeconds > Link.RefreshInterval)
                {
                    UpdateLink(viewInfo);
                }
                else if (pendingUpdate && DateTime.Now.Subtract(stopTime).TotalSeconds > Link.RefreshInterval)
                {
                    UpdateLink(viewInfo);
                }
                else if (Link.ViewRefreshMode == viewRefreshModeEnum.onStop && viewInfo.viewJustStopped)
                {
                    stopTime = DateTime.Now;
                    pendingUpdate = true;
                }
                else if (Link.ViewRefreshMode == viewRefreshModeEnum.onRequest && CheckRegionOverlap(viewInfo))
                {
                    UpdateLink(viewInfo);           
                }
                else if (LinkRoot == null)
                {
                    UpdateLink(viewInfo);           
                }
                
                // TOdo OnExpire


                if (viewInfo != null &&viewInfo.viewMoving)
                {
                    pendingUpdate = false;
                }
            }
        }

        private bool CheckRegionOverlap(KmlViewInformation viewInfo)
        {
            if (region != null)
            {
                if (!( region.west > viewInfo.bboxEast
                    || region.east < viewInfo.bboxWest
                    || region.north > viewInfo.bboxSouth
                    || region.south < viewInfo.bboxNorth) )
                {
                    return true;
                }
            }
           
            return false;
           
        }

        private void UpdateLink(KmlViewInformation viewInfo)
        {
            try
            {
                if (Link != null)
                {
                    string url;
                    if (Link.ViewRefreshMode == viewRefreshModeEnum.onStop)
                    //if (!String.IsNullOrEmpty(Link.viewFormat))
                    {
                        url = viewInfo.PrepLinkUrl(Link.Href + "?" + Link.ViewFormat);
                    }
                    else
                    {
                        url = Link.Href;
                    }

                    LinkRoot = new KmlRoot(url, Owner);
                    Dirty = true;

                    //if (open)
                    //{
                    //    if (LinkRoot.children != null)
                    //    {
                    //        foreach (KmlFeature feature in LinkRoot.children)
                    //        {
                    //            feature.open = true;
                    //        }
                    //    }
                    //}

                    //if (visibility)
                    //{
                    //    if (LinkRoot.children != null)
                    //    {
                    //        foreach (KmlFeature feature in LinkRoot.children)
                    //        {
                    //            feature.visibility = true;
                    //        }
                    //    }
                    //}
                }
            }
            catch
            {
            }
        }
    }

    public class KmlPlacemark : KmlFeature
    {
        public KmlGeometry geometry = null;
        public Rectangle hitTestRect = Rectangle.Empty;
        public KmlPoint Point = null;
        private bool selected;

        public bool Selected
        {
            get
            { 
                return selected; 
            }
            set { selected = value; }
        }
        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);

            if (node["Point"] != null)
            {
                geometry = new KmlPoint();
                geometry.LoadDetails(node["Point"], owner);
            }
            else if (node["LineString"] != null)
            {
                geometry = new KmlLineString();
                geometry.LoadDetails(node["LineString"], owner);
            }
            else if (node["LinearRing"] != null)
            {
                geometry = new KmlLinearRing();
                geometry.LoadDetails(node["LinearRing"], owner);
            } 
            else if (node["Polygon"] != null)
            {
                geometry = new KmlPolygon();
                geometry.LoadDetails(node["Polygon"], owner);
            }    
            else if (node["MultiGeometry"] != null)
            {
                geometry = new KmlMultiGeometry();
                geometry.LoadDetails(node["MultiGeometry"], owner);
            }           
            //else if (node["Model"] != null)
            //{
            //    geometry = new KmlModel();
            //    geometry.LoadDetails(node["Model"]);
            //}           
            //todo support models someday
        }


    }

    //<!-- abstract element; do not create -->
    //<!-- AbstractView -->                   <!-- Camera, LookAt -->                
    //<!-- extends Object -->
    //<-- /AbstractView -->
    public class KmlAbstractView : KmlObject
    {

    }

    //<LookAt id="ID">
    //<longitude>0</longitude>      <!-- kml:angle180 -->
    //<latitude>0</latitude>        <!-- kml:angle90 -->
    //<altitude>0</altitude>        <!-- double --> 
    //<heading>0</heading>          <!-- kml:angle360 -->
    //<tilt>0</tilt>                <!-- kml:anglepos90 -->
    //<range></range>               <!-- double -->
    //<altitudeMode>clampToGround</altitudeMode> 
    //<!--kml:altitudeModeEnum:clampToGround, relativeToGround, absolute -->
    //</LookAt>

    public class KmlLookAt : KmlAbstractView
    {
        public double longitude;
        public double latitude;
        public double altitude;
        public double heading;
        public double tilt;
        public double range;
        public altitudeModeEnum altitudeMode;

        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);

            if (node["longitude"] != null)
            {
                Double.TryParse(node["longitude"].InnerText, out longitude);
            }
            if (node["latitude"] != null)
            {
                Double.TryParse(node["latitude"].InnerText, out latitude);
            }
            if (node["altitude"] != null)
            {
                Double.TryParse(node["altitude"].InnerText, out altitude);
            }
            if (node["heading"] != null)
            {
                Double.TryParse(node["heading"].InnerText, out heading);

            }
            if (node["tilt"] != null)
            {
                Double.TryParse(node["tilt"].InnerText, out tilt);
            }

            if (node["range"] != null)
            {
                Double.TryParse(node["range"].InnerText, out range);
            }

            if (node["altitudeMode"] != null)
            {
                try
                {
                    altitudeMode = (altitudeModeEnum)Enum.Parse(typeof(altitudeModeEnum), node["altitudeMode"].InnerText);
                }
                catch
                {
                }
            }
        }
    }

    public class KmlGeometry : KmlObject
    {
        public virtual KmlCoordinate GetCenterPoint()
        {
            return new KmlCoordinate();
        }
    }

    //<Point id="ID">
    //  <!-- specific to Point -->
    //  <extrude>0</extrude>                        <!-- boolean -->
    //  <altitudeMode>clampToGround</altitudeMode>  <!-- kml:altitudeModeEnum: clampToGround, relativeToGround, or absolute -->
    //  <coordinates>...</coordinates>              <!-- lon,lat[,alt] -->
    //</Point>

    public enum altitudeModeEnum { clampToGround, relativeToGround, absolute, clampedToGround = clampToGround }
    public class KmlPoint : KmlGeometry
    {
        public double latitude;
        public double longitude;
        public double altitude;

        public bool extrude;

        public altitudeModeEnum altitudeMode = altitudeModeEnum.clampToGround;

        public override void  LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);

            if (node["coordinates"] != null)
            {
                var split = node["coordinates"].InnerText.Split(new[] { ',' });

                if (split.Length > 0)
                {
                    double.TryParse(split[0], out longitude);
                }
                if (split.Length > 1)
                {
                    double.TryParse(split[1], out latitude);
   
                }
                if (split.Length > 2)
                {
                    double.TryParse(split[2], out altitude);
                }             
            }

            if (node["extrude"] != null)
            {
                extrude = node["extrude"].InnerText.Trim() == "1";
            }

            if (node["altitudeMode"] != null)
            {
                try
                {
                    altitudeMode = (altitudeModeEnum)Enum.Parse(typeof(altitudeModeEnum), node["altitudeMode"].InnerText.Trim());
                }
                catch
                {
                }
            }
        }

        public override KmlCoordinate GetCenterPoint()
        {
            var point = new KmlCoordinate();
            point.Lat = latitude;
            point.Lng = longitude;
            point.Alt = altitude;

            return point;
        }
    }

    public class KmlLineString : KmlLineList
    {
        
    }

    public class KmlLinearRing : KmlLineList
    {

    }
    public class KmlLineList : KmlGeometry
    {
        public bool extrude;
        public bool Astronomical = false;
        public altitudeModeEnum altitudeMode;
        public double MeanRadius = 6371000;

        public List<KmlCoordinate> PointList = new List<KmlCoordinate>();

        public void ParseWkt(string geoText, string option, double alt, Dates date)
        {

            var parts = geoText.Split(new[] { '(', ',', ')' });
            foreach (var part in parts)
            {
                var coordinates = part.Trim().Split(new[] { ' ' });
                if (coordinates.Length > 1)
                {
                    var pnt = new KmlCoordinate();
                    pnt.Lng = double.Parse(coordinates[0]);
                    if (Astronomical)
                    {
                        pnt.Lng -= 180;
                    }
                    pnt.Lat = double.Parse(coordinates[1]);
                    if (coordinates.Length > 2 && alt == 0)
                    {
                        pnt.Alt = double.Parse(coordinates[2]);
                    }
                    else
                    {
                        pnt.Alt = alt;
                    }
                    pnt.Date = date;
                    PointList.Add(pnt);
                }
            }
        }

        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);

            if (node["coordinates"] != null)
            {
                var data = node["coordinates"].InnerText;
                data = data.Replace(", ", ",").Replace(" ,", ",").Replace(" , ", ",").Replace("(", "").Replace(")", "");
                var lines = data.Split(new[] { '\n', '\r', ' ' });
                foreach (var line in lines)
                {
                    var parts = line.Split(new[] { ',' });
                    if (parts.Length > 1)
                    {
                        var pnt = new KmlCoordinate();
                        pnt.Lng = double.Parse(parts[0]);
                        pnt.Lat = double.Parse(parts[1]);
                        if (parts.Length > 2)
                        {
                            pnt.Alt = double.Parse(parts[2]);
                        }
                        else
                        {
                            pnt.Alt = 0;
                        }
                        PointList.Add(pnt);
                    }
                }
            }
            if (node["extrude"] != null)
            {
                extrude = node["extrude"].InnerText.Trim() == "1";
            }

            if (node["altitudeMode"] != null)
            {
                try
                {
                    altitudeMode = (altitudeModeEnum)Enum.Parse(typeof(altitudeModeEnum), node["altitudeMode"].InnerText.Trim());
                }
                catch
                {
                }
            }
        }
        public override KmlCoordinate GetCenterPoint()
        {
            var point = new KmlCoordinate();
            point.Lat = 0;
            point.Lng = 0;
            point.Alt = 0;


            foreach (var pnt in PointList)
            {
                point.Lat += pnt.Lat;
                point.Lng += pnt.Lng;
                point.Alt += pnt.Alt;
            }
            point.Lat /= PointList.Count;
            point.Lng /= PointList.Count;
            point.Alt /= PointList.Count;

            return point;
        }
    }

    public class KmlPolygon : KmlGeometry
    {
        public bool extrude;

        public altitudeModeEnum altitudeMode;

        public KmlLinearRing OuterBoundary = null;
        public List<KmlLinearRing> InnerBoundarys = new List<KmlLinearRing>();
        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);

            if (node["outerBoundaryIs"] != null)
            {
                if (node["outerBoundaryIs"]["LinearRing"] != null)
                {
                    OuterBoundary = new KmlLinearRing();
                    OuterBoundary.LoadDetails(node["outerBoundaryIs"]["LinearRing"], owner);
                }
            }

            foreach(XmlNode child in node.ChildNodes)
            {
                if (child.Name == "innerBoundaryIs")
                {
                    var innerRing = new KmlLinearRing();
                    innerRing.LoadDetails(child, owner);
                    InnerBoundarys.Add(innerRing);
                }
            }
            
            if (node["extrude"] != null)
            {
                extrude = node["extrude"].InnerText.Trim() == "1";
            }

            if (node["altitudeMode"] != null)
            {
                altitudeMode = (altitudeModeEnum)Enum.Parse(typeof(altitudeModeEnum), node["altitudeMode"].InnerText.Trim());
            }
        }
        public override KmlCoordinate GetCenterPoint()
        {
            return OuterBoundary.GetCenterPoint();
        }
    }

    public class KmlMultiGeometry : KmlGeometry
    {
        public bool extrude;

        public altitudeModeEnum altitudeMode;

        public List<KmlGeometry> Children = new List<KmlGeometry>();

        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);

            foreach (XmlNode child in node.ChildNodes)
            {
                KmlGeometry geometry = null;
                switch (child.Name)
                {
                    case "Point":
                        {
                            geometry = new KmlPoint();
                            geometry.LoadDetails(child, owner);
                        }
                        break;
                    case "LineString":
                        {
                            geometry = new KmlLineString();
                            geometry.LoadDetails(child, owner);
                        }
                        break;
                    case "LinearRing":
                        {
                            geometry = new KmlLinearRing();
                            geometry.LoadDetails(child, owner);
                        }
                        break;
                    case "Polygon":
                        {
                            geometry = new KmlPolygon();
                            geometry.LoadDetails(child, owner);
                        }
                        break;
                    case "MultiGeometry":
                        {
                            geometry = new KmlMultiGeometry();
                            geometry.LoadDetails(child, owner);
                        }
                        break;
                }

                if (geometry != null)
                {
                    Children.Add(geometry);
                }
            }

            if (node["extrude"] != null)
            {
                extrude = node["extrude"].InnerText.Trim() == "1";
            }

            if (node["altitudeMode"] != null)
            {
                altitudeMode = (altitudeModeEnum)Enum.Parse(typeof(altitudeModeEnum), node["altitudeMode"].InnerText.Trim());
            }
        }

        public override KmlCoordinate GetCenterPoint()
        {
            var point = new KmlCoordinate();
            point.Lat = 0;
            point.Lng = 0;
            point.Alt = 0;

            var count = 0;
            foreach (var child in Children)
            {
                count++;
                var pnt = child.GetCenterPoint();
                point.Lat += pnt.Lat;
                point.Lng += pnt.Lng;
                point.Alt += pnt.Alt;
            }

            point.Lat /= count;
            point.Lng /= count;
            point.Alt /= count;

            return point;
        }
    }

    public class KmlContainer : KmlFeature
    {
        public List<KmlFeature> children = null;

        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);

            foreach (XmlNode child in node.ChildNodes)
            {
                KmlFeature newChild = null;
                switch (child.Name)
                {
                    case "NetworkLink":
                        newChild = new KmlNetworkLink();
                        break;
                    case "Placemark":
                        newChild = new KmlPlacemark();
                        break;
                    case "PhotoOverlay":
                        newChild = new KmlPhotoOverlay();
                        break;
                    case "ScreenOverlay":
                        newChild = new KmlScreenOverlay();
                        break;
                    case "GroundOverlay":
                        newChild = new KmlGroundOverlay();
                        break;
                    case "Folder":
                        newChild = new KmlFolder();
                        break;
                    case "Document":
                        newChild = new KmlDocument();
                        break;
                }

                if (newChild != null)
                {
                    newChild.sky = sky;
                    newChild.LoadDetails(child, owner);
                    if (children == null)
                    {
                        children = new List<KmlFeature>();
                    }
                    children.Add(newChild);
                }
            }
        }
    }

    public class KmlFolder : KmlContainer
    {

    }

    public class KmlDocument : KmlContainer
    {
        //todo <!-- 0 or more Schema elements -->
        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
           
            base.LoadDetails(node, owner);
        }
    }


    //<Icon id="ID">
    //<!-- specific to Icon -->  
    //<href>...</href>                         <!-- anyURI -->
    //<gx:x>0<gx:x/>                           <!-- int -->
    //<gx:y>0<gx:y/>                           <!-- int -->
    //<gx:w>...<gx:w/>                         <!-- int -->
    //<gx:h>...<gx:h/>                         <!-- int -->
    //<refreshMode>onChange</refreshMode>   
    //<!-- kml:refreshModeEnum: onChange, onInterval, or onExpire -->   
    //<refreshInterval>4</refreshInterval>     <!-- float -->
    //<viewRefreshMode>never</viewRefreshMode> 
    //<!-- kml:viewRefreshModeEnum: never, onStop, onRequest, onRegion -->
    //<viewRefreshTime>4</viewRefreshTime>     <!-- float -->
    //<viewBoundScale>1</viewBoundScale>       <!-- float -->
    //<viewFormat>...</viewFormat>             <!-- string -->
    //<httpQuery>...</httpQuery>               <!-- string -->
    //</Icon>
    public class KmlIcon : KmlLink
    {
        int X;
        int Y;
        int W;
        int H;

        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);
            if (node["gx:x"] != null)
            {
                X = Convert.ToInt32(node["gx:x"].InnerText);
            }
            if (node["gx:y"] != null)
            {
                Y = Convert.ToInt32(node["gx:y"].InnerText);
            }
            if (node["gx:w"] != null)
            {
                W = Convert.ToInt32(node["gx:w"].InnerText);
            }
            if (node["gx:h"] != null)
            {
                H = Convert.ToInt32(node["gx:h"].InnerText);
            }
        }
        public Texture11 texture = null;

        string cacheKey;
        public Texture11 Texture
        {
            get
            {
                if (texture != null)
                {
                    return texture;
                }

                if (string.IsNullOrEmpty(Href))
                {
                    return null;
                }

                if (cacheKey == null)
                {
                    if (Owner != null)
                    {
                        cacheKey = Owner.GetStreamUrl(Href);
                    }
                    else
                    {
                        cacheKey = Href;
                    }
                    if (!IconCache.ContainsKey(cacheKey))
                    {
                        // Add the new entry
                        IconCache.Add(cacheKey, new IconCacheEntry(Owner, Href)); 
                    }
                }

                var entry = IconCache[cacheKey];
                if (entry != null)
                {
                    return entry.Texture;
                }
                return null;
            }
            set
            {
                texture = value;
            }
        }

        private Stream GetStream()
        {
            return Owner.GetFileStream(Href);
        }

        // Icon Cache functions & Members
        static readonly Dictionary<string, IconCacheEntry> IconCache = new Dictionary<string, IconCacheEntry>();

    }

    public class IconCacheEntry
    {
        public static int CurrentFrame = 0;
        public bool Loaded = false;
        public bool Requested = false;
        public string Href = null;
        public KmlRoot Owner = null;
            //todo if owner goes away it should clean out cache entries or pass them to another owner.
        public int LastRequestFrame = 0;
        public Texture11 texture = null;
        const int MaxErrorCount = 3;
        public int ErrorCount = 0;
        public IconCacheEntry(KmlRoot owner, string href)
        {
            Owner = owner;
            Href = href;
        }
        public Texture11 Texture
        {
            get
            {
                if (!Loaded && !Requested)
                {
                    Requested = true;
                    // Do a background load on this
                    ThreadPool.QueueUserWorkItem(LoadTexture, this);

                }
                LastRequestFrame = CurrentFrame;
                return texture;
            }
        }

      //  public static Mutex mut = new Mutex();

        public static void LoadTexture( object objEntry)
        {
            var entry = objEntry as IconCacheEntry;
            entry.Requested = true;
           // mut.WaitOne();
            try
            {
                var dir = Properties.Settings.Default.CahceDirectory + "Data\\KmlCache\\";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                // This is a expanded timeout version of WebClient
                var Client = new MyWebClient();
                

                var filename = dir + ((uint)entry.Href.GetHashCode32()) + ".png";

                //if (File.Exists(filename))
                //{
                //    FileInfo fi = new FileInfo(filename);

                //    if (fi.Length != 8 && fi.Length < 100)
                //    {
                //        try
                //        {
                //            File.Delete(filename);
                //        }
                //        catch
                //        {
                //        }
                //    }
                //}

			
                //Client.DownloadFile(CacheProxy.GetCacheUrl(url), filename);

                Stream stream = null;
                if (entry.Owner == null)
                {

                    if (Uri.IsWellFormedUriString(entry.Href, UriKind.Absolute))
                    {
                        var data = Client.DownloadData(entry.Href);
                        stream = new MemoryStream(data);
                    }
                    else
                    {
                        stream = File.Open(entry.Href, FileMode.Open);
                    }
                }
                else
                {
                    stream = entry.Owner.GetFileStream(entry.Href);
                }

                var fileStream = File.OpenWrite(filename);
                var buffer = new byte[32768];
                while (true) 
                {
                    var read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)    
                        break;
                    fileStream.Write(buffer, 0, read);
                }
                stream.Close();
                fileStream.Close();
                //todo this was loaded from the kmlicon API. Any Problems with that?
                entry.texture = Texture11.FromFile(filename);

                entry.Loaded = true;

            }
            catch
            {
                
                entry.ErrorCount++;
                
                // retry until sucsess or MaxError Count exceeded
                if (entry.ErrorCount < MaxErrorCount)
                {
                    entry.Requested = false;
                    entry.Loaded = false;

                }
                else
                {
                    entry.Loaded = true;
                }
            }
            finally
            {
            //    mut.ReleaseMutex();
            }
            

        }
    }


    public class KmlOverlay : KmlFeature
    {
        public Color color = Color.White;
        public int drawOrder = 0;
        public KmlIcon Icon = null;
        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);
            if (node["geomColor"] != null)
            {
                var abgr = Convert.ToInt32(node["geomColor"].InnerText, 16);

                var alpha = (byte)(abgr >> 24);
                var blue = (byte)(abgr >> 16);
                var green = (byte)(abgr >> 8);
                var red = (byte)(abgr);
                color = Color.FromArgb(alpha, red, green, blue);
            }
            else if (node["color"] != null)
            {
                color = GetColor(node);
            }

            if (node["drawOrder"] != null)
            {
                drawOrder = Convert.ToInt32(node["drawOrder"].InnerText);
            }

            if (node["Icon"] != null)
            {
                Icon = new KmlIcon();
                Icon.LoadDetails(node["Icon"], owner);
            }
        }
    }
      // <!-- specific to Overlay -->
      //<color>ffffffff</color>                   <!-- kml:color -->
      //<drawOrder>0</drawOrder>                  <!-- int -->
      //<Icon>
      //  <href>...</href>
      //</Icon>

    public class KmlPhotoOverlay : KmlOverlay
    {


        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);
            
        }
    }
    public class KmlSpot
    {
        public float X = .5f;
        public float Y = .5f;
        public KmlPixelUnits UnitsX = KmlPixelUnits.Fraction;
        public KmlPixelUnits UnitsY = KmlPixelUnits.Fraction;

        public KmlSpot()
        {
        }

        public KmlSpot(XmlNode node, string nodeName)
        {
            if (node[nodeName] != null)
            {
                if (node[nodeName].Attributes["x"] != null)
                {
                    X = Convert.ToSingle(node[nodeName].Attributes["x"].Value);
                }
                if (node[nodeName].Attributes["y"] != null)
                {
                    Y = Convert.ToSingle(node[nodeName].Attributes["y"].Value);
                }
                if (node[nodeName].Attributes["xunits"] != null)
                {
                    switch (node[nodeName].Attributes["xunits"].Value)
                    {
                        case "pixels":
                            {
                                UnitsX = KmlPixelUnits.Pixels;
                            }
                            break;
                        case "insetPixels":
                            {
                                UnitsX = KmlPixelUnits.InsetPixels;
                            }
                            break;
                        default:
                        case "fraction":
                            {
                                UnitsX = KmlPixelUnits.Fraction;
                            }
                            break;
                    }
                }
                if (node[nodeName].Attributes["yunits"] != null)
                {
                    switch (node[nodeName].Attributes["yunits"].Value)
                    {
                        case "pixels":
                            {
                                UnitsY = KmlPixelUnits.Pixels;
                            }
                            break;
                        case "insetPixels":
                            {
                                UnitsY = KmlPixelUnits.InsetPixels;
                            }
                            break;
                        default:
                        case "fraction":
                            {
                                UnitsY = KmlPixelUnits.Fraction;
                            }
                            break;
                    }
                }     
            }
        }
    }

     //<ScreenOverlay id="ID">
     // <!-- inherited from Feature element -->
     // <name>...</name>                      <!-- string -->
     // <visibility>1</visibility>            <!-- boolean -->
     // <open>0</open>                        <!-- boolean -->
     // <atom:author>...<atom:author>         <!-- xmlns:atom -->
     // <atom:link href=" "/>                <!-- xmlns:atom -->
     // <address>...</address>                <!-- string -->
     // <xal:AddressDetails>...</xal:AddressDetails>  <!-- xmlns:xal -->  <phoneNumber>...</phoneNumber>        <!-- string -->  <Snippet maxLines="2">...</Snippet>   <!-- string -->
     // <description>...</description>        <!-- string -->
     // <AbstractView>...</AbstractView>      <!-- Camera or LookAt -->
     // <TimePrimitive>...</TimePrimitive>
     // <styleUrl>...</styleUrl>              <!-- anyURI -->
     // <StyleSelector>...</StyleSelector>
     // <Region>...</Region>
     // <Metadata>...</Metadata>              <!-- deprecated in KML 2.2 -->
     // <ExtendedData>...</ExtendedData>      <!-- new in KML 2.2 -->

     // <!-- inherited from Overlay element -->
     // <color>ffffffff</color>                  <!-- kml:color -->
     // <drawOrder>0</drawOrder>                 <!-- int -->
     // <Icon>...</Icon>

     // <!-- specific to ScreenOverlay -->
     // <overlayXY x="double" y="double" xunits="fraction" yunits="fraction"/>    
     //   <!-- vec2 -->
     //   <!-- xunits and yunits can be one of: fraction, pixels, or insetPixels -->
     // <screenXY x="double" y="double" xunits="fraction" yunits="fraction"/>      
     //   <!-- vec2 -->
     // <rotationXY x="double" y="double" xunits="fraction" yunits"fraction"/>  
     //   <!-- vec2 -->
     // <size x="double" y="double" xunits="fraction" yunits="fraction"/>              
     //   <!-- vec2 --> 
     // <rotation>0</rotation>                   <!-- float -->
     //</ScreenOverlay>
    public class KmlScreenOverlay : KmlOverlay
    {
        public KmlSpot ScreenSpot = new KmlSpot();
        public KmlSpot OverlaySpot = new KmlSpot();
        public KmlSpot RotationSpot = new KmlSpot();
        public KmlSpot Size = new KmlSpot();
        public double Rotation = 0;
        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);
            if (node["overlayXY"] != null)
            {
                OverlaySpot = new KmlSpot(node, "overlayXY");
            }

            if (node["screenXY"] != null)
            {
                ScreenSpot = new KmlSpot(node,"screenXY");
            }

            if (node["rotationXY"] != null)
            {
                RotationSpot = new KmlSpot(node, "rotationXY");
            }

            if (node["size"] != null)
            {
                Size = new KmlSpot(node, "size");
            }

            if (node["rotation"] != null)
            {
                Double.TryParse(node["rotation"].Value, out Rotation);  
            }
        }
    }

    public struct KmlCoordinate
    {
        public double Lat;
        public double Lng;
        public double Alt;
        public Dates Date;
    }


    


      //<!-- specific to GroundOverlay -->
      //<altitude>0</altitude>                    <!-- double -->
      //<altitudeMode>clampToGround</altitudeMode>
      //   <!-- kml:altitudeModeEnum: clampToGround or absolute --> 
      //<LatLonBox>
      //  <north>...</north>                      <! kml:angle90 -->
      //  <south>...</south>                      <! kml:angle90 -->
      //  <east>...</east>                        <! kml:angle180 -->
      //  <west>...</west>                        <! kml:angle180 -->
      //  <rotation>0</rotation>                  <! kml:angle180 -->
      //</LatLonBox>

    public class KmlGroundOverlay : KmlOverlay
    {
        public double north;
        public double south;
        public double east;
        public double west;
        public double rotation;
        public double altitude;
        public altitudeModeEnum altitudeMode = altitudeModeEnum.clampToGround;


        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);

            if (node["altitude"] != null)
            {
                altitude = Convert.ToDouble(node["altitude"].InnerText);
            }

            if (node["altitudeMode"] != null)
            {
                altitudeMode = (altitudeModeEnum)Enum.Parse(typeof(altitudeModeEnum), node["altitudeMode"].InnerText);
            }

            if (node["LatLonBox"] != null)
            {
                XmlNode box = node["LatLonBox"];
                if (box["north"] != null)
                {
                    north = Convert.ToDouble(box["north"].InnerText);
                }

                if (box["south"] != null)
                {
                    south = Convert.ToDouble(box["south"].InnerText);
                }

                if (box["east"] != null)
                {
                    east = Convert.ToDouble(box["east"].InnerText);
                }

                if (box["west"] != null)
                {
                    west = Convert.ToDouble(box["west"].InnerText);
                }

                if (box["rotation"] != null)
                {
                    rotation = Convert.ToSingle(box["rotation"].InnerText);
                }
            }
        }


        private Matrix3d Matrix;
        bool MatrixFresh = false;

        public Matrix3d GetMatrix()
        {
            if (!MatrixFresh)
            {
                var fieldWidth = east - west;
                var fieldHeight = north - south;

                var center = new Coordinates(0, 0);
                center.Lat = south + fieldHeight / 2;
                center.Lng = west + fieldWidth / 2;

                Matrix = Matrix3d.GetMapMatrix(center, fieldWidth, fieldHeight, rotation/180*Math.PI);
            }

            return Matrix;
        }


    }

    public class KmlSubStyle : KmlObject
    {
    }

    public enum KmlListItemTypes { Check, RadioFolder, CheckOffOnly, CheckHideChildren };

    //<ListStyle id="ID">
    //    <!-- specific to ListStyle -->
    //    <listItemType>check</listItemType> <!-- kml:listItemTypeEnum:check,
    //                                          checkOffOnly,checkHideChildren,
    //                                         radioFolder -->
    //    <bgColor>ffffffff</bgColor>        <!-- kml:color -->
    //    <ItemIcon>                         <!-- 0 or more ItemIcon elements -->
    //    <state>open</state>   
    //      <!-- kml:itemIconModeEnum:open, closed, error, fetching0, fetching1, or fetching2 -->
    //    <href>...</href>                 <!-- anyURI -->
    //    </ItemIcon>
    //</ListStyle>
    public class KmlListStyle : KmlSubStyle
    {
        public KmlListItemTypes ListItemType = KmlListItemTypes.Check;
        public Color Background = Color.White;

        public string ItemIconState = "open";
        public string ItemIconHref = "";
        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);

            if (node["listItemType"] != null)
            {
                switch (node["listItemType"].InnerText)
                {
                    case "radioFolder":
                        ListItemType = KmlListItemTypes.RadioFolder;
                        break;
                    case "checkOffOnly":
                        ListItemType = KmlListItemTypes.CheckOffOnly;
                        break;
                    case "checkHideChildren":
                        ListItemType = KmlListItemTypes.CheckHideChildren;
                        break;
                    default:
                    case "check":
                        ListItemType = KmlListItemTypes.Check;
                        break;

                }
            }

            Background = GetBgColor(node);

            if (node["ItemIcon"] != null)
            {
                if (node["ItemIcon"]["state"] != null)
                {
                    ItemIconState = node["ItemIcon"]["state"].InnerText;
                }
                if (node["ItemIcon"]["href"] != null)
                {
                    ItemIconHref = node["ItemIcon"]["href"].InnerText;
                }
            }
        }
    }

    public enum KmlColorModes { Normal, Random };

    public class KmlColorStyle : KmlSubStyle
    {
        readonly Random rnd = new Random();
        public Color Color = Color.White;
        public KmlColorModes ColorMode = KmlColorModes.Normal;

        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);

            Color = GetColor(node);

            ColorMode = GetColorMode(node);

            if (ColorMode == KmlColorModes.Random)
            {
                var red = (Byte)(Color.R * rnd.NextDouble());
                var green = (Byte)(Color.G * rnd.NextDouble());
                var blue = (Byte)(Color.B * rnd.NextDouble());
                var alpha = (Byte)(Color.A * rnd.NextDouble());
                Color = Color.FromArgb(alpha, red, green, blue);
            }
        }
    }
    //<PolyStyle id="ID">
    //    <!-- inherited from ColorStyle -->
    //    <color>ffffffff</color>            <!-- kml:color -->
    //    <colorMode>normal</colorMode>      <!-- kml:colorModeEnum: normal or random -->

    //    <!-- specific to PolyStyle -->
    //    <fill>1</fill>                     <!-- boolean -->
    //    <outline>1</outline>               <!-- boolean -->
    //</PolyStyle>
    public class KmlPolyStyle : KmlColorStyle
    {
        public bool Fill = false;
        public bool Outline = true;
        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);
            Fill = GetBoolValue(node, "fill");
            Outline = GetBoolValue(node, "outline");   
        }
    }


    //<LineStyle id="ID">
    //    <!-- inherited from ColorStyle -->
    //    <color>ffffffff</color>            <!-- kml:color -->
    //    <colorMode>normal</colorMode>      <!-- colorModeEnum: normal or random -->
    //    <!-- specific to LineStyle -->
    //    <width>1</width>                   <!-- float -->
    //</LineStyle>
    public class KmlLineStyle : KmlColorStyle
    {
        public double Width = 1;
        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);
            if (node["width"] != null)
            {
                Width = Convert.ToDouble(node["width"].InnerText);
            }
        }
    }
    //<IconStyle id="ID">
    //  <!-- inherited from ColorStyle -->
    //  <color>ffffffff</color>            <!-- kml:color -->
    //  <colorMode>normal</colorMode>      <!-- kml:colorModeEnum:normal or random -->

    //  <!-- specific to IconStyle -->
    //  <scale>1</scale>                   <!-- float -->
    //  <heading>0</heading>               <!-- float -->
    //  <Icon>
    //    <href>...</href>
    //  </Icon> 
    //  <hotSpot x="0.5"  y="0.5" 
    //    xunits="fraction" yunits="fraction"/>    <!-- kml:vec2 -->                    
    //</IconStyle>

    public enum KmlPixelUnits { Fraction, Pixels, InsetPixels };

    public class KmlIconStyle : KmlColorStyle
    {
        public float Scale = 1;
        public float Heading = 0;
        public KmlIcon Icon = new KmlIcon();
        public KmlSpot HotSpot = new KmlSpot();

        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);
            if (node["scale"] != null)
            {
                Scale = Convert.ToSingle(node["scale"].InnerText);
            }

            if (node["Icon"] != null)
            {
                Icon.LoadDetails(node["Icon"], owner);
            }

            if (node["hotSpot"] != null)
            {
                HotSpot = new KmlSpot(node,"hotSpot");
            }
         
            
        }
        //public Stream GetIconStream()
        //{
        //    return Owner.GetFileStream(Icon.Href);
        //}
    }

    //<LabelStyle id="ID">
    //  <!-- inherited from ColorStyle -->
    //  <color>ffffffff</color>            <!-- kml:color -->
    //  <colorMode>normal</colorMode>      <!-- kml:colorModeEnum: normal or random -->

    //  <!-- specific to LabelStyle -->
    //  <scale>1</scale>                   <!-- float -->
    //</LabelStyle>

    public class KmlLabelStyle : KmlColorStyle
    {
        public float Scale = 1;

        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);

            if (node["scale"] != null)
            {
                float.TryParse(node["scale"].InnerText, out Scale);
            }
        }
    }


    public class KmlStyleSelector : KmlObject
    {
        virtual public KmlStyle GetStyle(bool selected)
        {

            return new KmlStyle();
        }
    }

    public class KmlStyle : KmlStyleSelector
    {
        public KmlPolyStyle PolyStyle = new KmlPolyStyle();
        public KmlLineStyle LineStyle = new KmlLineStyle();
        public KmlListStyle ListStyle = new KmlListStyle();
        public KmlLabelStyle LabelStyle = new KmlLabelStyle();
        public KmlIconStyle IconStyle = new KmlIconStyle();
        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);
            if (node["PolyStyle"] != null)
            {
                PolyStyle.LoadDetails(node["PolyStyle"], owner);
            }
            if (node["LineStyle"] != null)
            {
                LineStyle.LoadDetails(node["LineStyle"], owner);
            }
            if (node["ListStyle"] != null)
            {
                ListStyle.LoadDetails(node["ListStyle"], owner);
            }
            if (node["LabelStyle"] != null)
            {
                LabelStyle.LoadDetails(node["LabelStyle"], owner);
            }
            if (node["IconStyle"] != null)
            {
                IconStyle.LoadDetails(node["IconStyle"], owner);
            }
        }

        public override KmlStyle GetStyle(bool selected)
        {
            return this;
        }
    }
    public class KmlStyleMap : KmlStyleSelector
    {
        string normal;
        string highlight;
        public override void LoadDetails(XmlNode node, KmlRoot owner)
        {
            base.LoadDetails(node, owner);
            foreach (XmlNode child in node.ChildNodes)
            {

                if (child.Name == "Pair" )
                {
                    if (child["key"] != null && child["styleUrl"] != null)
                    {
                        if (child["key"].InnerText.Trim() == "normal")
                        {
                            normal = child["styleUrl"].InnerText.Trim();
                        }
                        else
                        {
                            highlight = child["styleUrl"].InnerText.Trim();
                        }

                    }
                }
            }
        }

        public override KmlStyle GetStyle(bool highlighted)
        {
            if (highlighted && highlight != null)
            {
                return (KmlStyle)Owner.Styles[highlight.Remove(0,1)];
            }
            return (KmlStyle)Owner.Styles[normal.Remove(0,1)];
        }
    }
}
