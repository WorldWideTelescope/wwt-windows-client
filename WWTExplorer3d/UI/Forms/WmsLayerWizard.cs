using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using WMS;
using System.Globalization;

namespace TerraViewer
{
    public partial class WmsLayerWizard : Form
    {
        public WmsLayerWizard()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.label1.Text = Language.GetLocalizedText(902, "Web Mapping Service URL");
            this.label2.Text = Language.GetLocalizedText(903, "Layers and Styles");
            this.label3.Text = Language.GetLocalizedText(904, "Server List");
            this.ServerName.Text = Language.GetLocalizedText(905, "<Type Server Name Here>");
            this.label4.Text = Language.GetLocalizedText(906, "Server Name");
            this.Delete.Text = Language.GetLocalizedText(167, "Delete");
            this.close.Text = Language.GetLocalizedText(212, "Close");
            this.add.Text = Language.GetLocalizedText(166, "Add");
            this.AddServer.Text = Language.GetLocalizedText(907, "Add Server");
            this.GetCapabilities.Text = Language.GetLocalizedText(908, "Get Layers");
            this.Text = Language.GetLocalizedText(909, "WMS Layers");
            this.dontParse.Text = Language.GetLocalizedText(1054, "Don\'t Parse");
            this.TiledWMS.Text = Language.GetLocalizedText(1140, "Tiled WMS");
            this.AddAsLayer.Text = Language.GetLocalizedText(1141, "Add As Layer");
            this.AbstractLabel.Text = Language.GetLocalizedText(1142, "Abstract");
        }

        private void GetCapabilities_Click(object sender, EventArgs e)
        {
            ShowCapababilities(true);

        }

        private void ShowCapababilities( bool download )
        {
            if (ServerList.SelectedIndex < 0)
            {
                return;
            }

            add.Enabled = false;

            LayersTree.Nodes.Clear();
            Abstract.Text = "";

            var wse = (WmsServerEntry)ServerList.SelectedItem;
            var req = "REQUEST=GetCapabilities&SERVICE=WMS&VERSION=1.3.0";

            if (wse.Url.Contains("?"))
            {
                req = "&" + req;
            }
            else
            {
                req = "?" + req;
            }

            var url = wse.Url + req;

            var filename = Properties.Settings.Default.CahceDirectory + "data\\wms\\" + ((uint)url.GetHashCode32()).ToString() + ".xml";
            var tiledFilename = Properties.Settings.Default.CahceDirectory + "data\\wms\\" + ((uint)url.GetHashCode32()).ToString() + ".tiled.xml";

            if (!File.Exists(filename) && !download)
            {
                LayersTree.Nodes.Add(Language.GetLocalizedText(910, "Not loaded -  Click Get Layers to download now"));
                return;
            }

            if (!FileDownload.DownloadFile(url, filename, download))
            {
                return;
            }

            var tiled = false;
            var tiledUrl = "";
            try
            {
                using (Stream stream = File.Open(filename, FileMode.Open))
                {
                    var caps = WMS_Capabilities.LoadFromSream(stream);

                    serviceUrl = caps.Capability.Request.GetMap.DCPType[0].HTTP.Get.OnlineResource.href;
                    if (!serviceUrl.Contains("?"))
                    {
                        serviceUrl += "?";
                    }
                    wmsVersion = caps.version;
                    stream.Close();

                    if (caps.Capability.Request.GetTileService != null && !string.IsNullOrEmpty(caps.Capability.Request.GetTileService.DCPType[0].HTTP.Get.OnlineResource.href))
                    {
                        tiled = true;
                        tiledUrl = caps.Capability.Request.GetTileService.DCPType[0].HTTP.Get.OnlineResource.href + "request=GetTileService";
                        serviceUrl = caps.Capability.Request.GetTileService.DCPType[0].HTTP.Get.OnlineResource.href;
                        if (!serviceUrl.Contains("?"))
                        {
                            serviceUrl += "?";
                        }
                    }

                    if (!tiled)
                    {
                        AddChildren(caps.Capability.Layer, LayersTree.Nodes);
                    }
                }
            }
            catch
            {
                try
                {

                    using (Stream stream = File.Open(filename, FileMode.Open))
                    {
                        var caps = WMT_MS_Capabilities.LoadFromSream(stream);

                        serviceUrl = caps.Capability.Request.GetMap.DCPType[0].HTTP.Get.OnlineResource.href;
                        if (!serviceUrl.Contains("?"))
                        {
                            serviceUrl += "?";
                        }
                        wmsVersion = caps.version;
                        stream.Close();
                        if ( caps.Capability.Request.GetTileService != null && !string.IsNullOrEmpty(caps.Capability.Request.GetTileService.DCPType[0].HTTP.Get.OnlineResource.href))
                        {
                            tiled = true;
                            tiledUrl = caps.Capability.Request.GetTileService.DCPType[0].HTTP.Get.OnlineResource.href + "request=GetTileService" ;
                            serviceUrl = caps.Capability.Request.GetTileService.DCPType[0].HTTP.Get.OnlineResource.href;
                            if (!serviceUrl.Contains("?"))
                            {
                                serviceUrl += "?";
                            }
                        }

                        if (!tiled)
                        {
                            AddChildren(caps.Capability.Layer, LayersTree.Nodes);
                        }
                    }
                    
                }
                catch
                {
                }
            }
            // Add Tiled if supported
            try
            {
                if (tiled)
                {
                    if (!FileDownload.DownloadFile(tiledUrl, tiledFilename, false))
                    {
                        return;
                    }
                    using (Stream stream = File.Open(tiledFilename, FileMode.Open))
                    {
                        var wts = WMS_Tile_Service.LoadFromSream(stream);
                        AddChildren(wts.TiledPatterns[0].TiledGroup, LayersTree.Nodes);
                    }

                }


            }
            catch
            {
            }


           
            
        }

        private static void AddChildren(WMS.Layer layer, TreeNodeCollection nodes)
        {
            if (layer.Children != null)
            {
                foreach (var child in layer.Children)
                {
                    var childNode = nodes.Add(child.Title);
                    childNode.Tag = child;
                    AddChildren(child, childNode.Nodes);
                }
            }

            if (layer.Style != null)
            {
                foreach (var style in layer.Style)
                {

                    //   layer.Style[0].
                    var styleNode = nodes.Add(style.Title);
                    styleNode.Tag = style;
                }
            }
        }

        private static void AddChildren(WMS_Tile_ServiceTiledPatternsTiledGroup[] layer, TreeNodeCollection nodes)
        {
            if (layer != null)
            {
                foreach (var child in layer)
                {
                    var childNode = nodes.Add(child.Title);
                    childNode.Tag = child;
                }
            }
        }

        private void SaveServerList()
        {
            var path = Properties.Settings.Default.CahceDirectory + "data\\WMS_Servers.txt";

            var sw = new StreamWriter(path);
            foreach (var item in ServerList.Items)
            {
                var wse = (WmsServerEntry)item;
                sw.WriteLine(wse.Name + "\t" + wse.Url);
            }
            sw.Close();
        }

        private void WmsLayerWizard_Load(object sender, EventArgs e)
        {
            var path = Properties.Settings.Default.CahceDirectory + "data\\WMS_Servers.txt";

            if (!File.Exists(path))
            {
                return;
            }

            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var parts = line.Split(new char[] { '\t' });

                var entry = new WmsServerEntry();
                entry.Name = parts[0];
                entry.Url = parts[1];
                var index = ServerList.Items.Add(entry);
            }
            if (ServerList.Items.Count > 0)
            {
                ServerList.SelectedIndex = 0;
            }

            add.Enabled = false;
        }
        string serviceUrl = "";
        string wmsVersion = "";
        string MakeWmsGetMapUrl(string layers, string styles, double west, double north, double east, double south, int width, int height, string time, string elevation)
        {
            return string.Format("{0}version=1.3.0&service=WMS&request=GetMap&layers={1}&styles={2}&crs=CRS:84&srs=EPSG:4326&bbox={3},{4},{5},{6}&width={7}&height={8}&format=image/png&transparent=TRUE",
                                            serviceUrl, layers, styles, west, south, east, north, width, height);
        }

        private void AddTiledLayer(WMS_Tile_ServiceTiledPatternsTiledGroup tileGroup)
        {
            var tilePatern = GetPattern(tileGroup.TilePattern[0].Value);

            var degrees = GetLngDegrees(tilePatern);

            foreach(var pat in tileGroup.TilePattern)
            {
                var deg = GetLngDegrees(pat.Value);

                if (deg < degrees)
                {
                    degrees = deg;
                }
            }



            var baseTileDegrees = degrees;
            var totalLevels = -1;

            while (baseTileDegrees < 180)
            {
                totalLevels++;
                baseTileDegrees *= 2;
            }


            var url = serviceUrl + FillBoundingBoxUrl(tilePatern.Replace("&time=${time}",""));
            var baseLevel = 3;

            //if (tileGroup.TilePattern.Length > totalLevels)
            {
           //     baseLevel = (totalLevels +8) - tileGroup.TilePattern.Length;
            }


            var imageType = ".png";
            double meanRadius = 300000000; //todo

            var referenceFrame = LayerManager.CurrentMap;

            var thumbUrl = "http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=" + referenceFrame;

            var ish = new ImageSetHelper(tileGroup.Title, url, ImageSetType.Planet, BandPass.Visible, ProjectionType.Equirectangular, (int)(uint)url.GetHashCode32(), baseLevel, totalLevels, 512, baseTileDegrees, imageType, true,"", 0, 0, 0, false, thumbUrl, false, false, 1, 0, 0, tileGroup.Abstract, "", "", "", meanRadius, referenceFrame);

            if (AddAsLayer.Checked == false)
            {
                Earth3d.MainWindow.CurrentImageSet = ish;
            }
            else
            {
                var layer = new ImageSetLayer(ish);
                layer.Enabled = true;
                layer.Name = tileGroup.Title;
                layer.ReferenceFrame = LayerManager.CurrentMap;

                LayerManager.Add(layer, true);
            }

        }

        private string GetPattern(string pattern)
        {
            var tilePaterns = pattern.Split(new char[] { '\n' });

            for (var i = 0; i < tilePaterns.Length; i++)
            {
                var part = tilePaterns[i].Trim();
                if (!String.IsNullOrEmpty(part))
                {
                    return part;
                }
            }

            return "";
        }

        private double GetLngDegrees(string url)
        {
          
            var start = url.IndexOf("bbox=", StringComparison.OrdinalIgnoreCase) + 5;

            var end = url.IndexOf("&", start);

            if (end == -1)
            {
                end = url.Length;
            }

            var bbox = url.Substring(start, end - start);

            var parts = bbox.Split(new char[] { ',' });

            var lngMin = double.Parse(parts[0]);
            var lngMax = double.Parse(parts[2]);
            var degrees = lngMax - lngMin;
            return degrees;
        }

        private string FillBoundingBoxUrl(string url)
        {
        
            var firstPart = url.Substring(0, url.IndexOf("bbox=", StringComparison.OrdinalIgnoreCase)+5);

            var indexEnd = url.IndexOf("&", firstPart.Length);

            var lastPart= "";
            
            if (indexEnd > -1)
            {
                lastPart = url.Substring(indexEnd);
            }

            return firstPart + "{lngMin},{latMin},{lngMax},{latMax}" + lastPart;
        }


        private void add_Click(object sender, EventArgs e)
        {
            if (LayersTree.SelectedNode == null)
            {
                return;
            }
            
            var tileGroup = LayersTree.SelectedNode.Tag as WMS_Tile_ServiceTiledPatternsTiledGroup;

            if (tileGroup != null)
            {
                AddTiledLayer(tileGroup);
                return;
            }

            var wmsLayer = LayersTree.SelectedNode.Tag as WMS.Layer;

            var style = LayersTree.SelectedNode.Tag as WMS.Style;

            if (style != null && LayersTree.SelectedNode.Parent != null)
            {
                wmsLayer = LayersTree.SelectedNode.Parent.Tag as WMS.Layer;
            }

            if (wmsLayer != null)
            {
                if (TiledWMS.Checked)
                {
                    AddTiledLayer(wmsLayer, style);
                    return;
                }

                double west = -180;
                double east = 180;
                double north = 90;
                double south = -90;

                var width = 2048;
                var height = 2048;

                if (wmsLayer.EX_GeographicBoundingBox != null)
                {
                    west = wmsLayer.EX_GeographicBoundingBox.westBoundLongitude;
                    north = wmsLayer.EX_GeographicBoundingBox.northBoundLatitude;
                    east = wmsLayer.EX_GeographicBoundingBox.eastBoundLongitude;
                    south = wmsLayer.EX_GeographicBoundingBox.southBoundLatitude;
                }

                if (wmsLayer.LatLonBoundingBox != null)
                {
                    west = wmsLayer.LatLonBoundingBox.minx;
                    north = wmsLayer.LatLonBoundingBox.maxy;
                    east = wmsLayer.LatLonBoundingBox.maxx;
                    south = wmsLayer.LatLonBoundingBox.miny;
                }

                if (wmsLayer.fixedHeight != null )
                {
                    var h = int.Parse(wmsLayer.fixedHeight);
                    if (h > 0)
                    {
                        height = h;
                    }
                }

                if (wmsLayer.fixedWidth != null)
                {
                    var w = int.Parse(wmsLayer.fixedWidth);

                    if (w > 0)
                    {
                        width = w;
                    }

                }
                
                var layer = new WmsLayer();

                //string path = MakeWmsGetMapUrl(wmsLayer.Name, style != null ? style.Name : "",
                //    west,north, east, south,
                //    width, height,
                //    "",
                //    "");

                layer.ServiceUrl = serviceUrl;
                layer.WmsVersion = wmsVersion;
                layer.Layers = wmsLayer.Name;
                layer.Styles = style != null ? style.Name : "";
                layer.Overlay.north = north;
                layer.Overlay.south = south;
                layer.Overlay.west = west;
                layer.Overlay.east = east;
                layer.Height = height;
                layer.Width = width;
                if (wmsLayer.Dimension != null)
                {
                    foreach (var dim in wmsLayer.Dimension)
                    {
                        if (dim.name == "time")
                        {
                            var dates = wmsLayer.Dimension[0].Value.Split(new char[] { ',' });

                            foreach (var date in dates)
                            {
                                layer.TimeRanges.Add(new TimeRange(date));
                            }
                            layer.UpdateTimeRange();
                        }

                    }
                }
                layer.Enabled = true;
                layer.Name = wmsLayer.Title;
                layer.ReferenceFrame = LayerManager.CurrentMap;

                LayerManager.Add(layer, true);

            }

        }

        private void AddTiledLayer(WMS.Layer wmsLayer, WMS.Style style)
        {
       
            double baseTileDegrees = 180;
            var totalLevels = 12;

            var styles = style != null ? style.Name : "";

            var url = FillBoundingBoxUrl(MakeWmsGetMapUrl(wmsLayer.Name, styles, 0, 0, 0, 0, 256, 256, "", ""));
            
            var baseLevel = 1;


            var imageType = ".png";
            double meanRadius = 300000000; //todo

            var referenceFrame = LayerManager.CurrentMap;

            var thumbUrl = "http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=" + referenceFrame;

            var ish = new ImageSetHelper(wmsLayer.Title, url, ImageSetType.Planet, BandPass.Visible, ProjectionType.Equirectangular, (int)(uint)url.GetHashCode32(), baseLevel, totalLevels, 512, baseTileDegrees, imageType, true, "", 0, 0, 0, false, thumbUrl, false, false, 1, 0, 0, wmsLayer.Abstract, "", "", "", meanRadius, referenceFrame);

            if (AddAsLayer.Checked == false)
            {
                Earth3d.MainWindow.CurrentImageSet = ish;
            }
            else
            {
                var layer = new ImageSetLayer(ish);
                layer.Enabled = true;
                layer.Name = wmsLayer.Title;
                layer.ReferenceFrame = LayerManager.CurrentMap;

                LayerManager.Add(layer, true);
            }
        }



        private void AddServer_Click(object sender, EventArgs e)
        {

            if (wmsUrl.Text.Length < 5 && ServerName.Text.Length < 1)
            {
                return;
            }
            var url = wmsUrl.Text;



            var index = url.IndexOf("?");

            if (index > -1)
            {
                url = url.Substring(0, index);
            }
            var entry = new WmsServerEntry();
            entry.Name = ServerName.Text;

            if (dontParse.Checked)
            {
                entry.Url = wmsUrl.Text;
            }
            else
            {
                entry.Url = url;
            }


            ServerList.Items.Add(entry);
            SaveServerList();
 
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (ServerList.SelectedIndex > -1)
            {
                ServerList.Items.RemoveAt(ServerList.SelectedIndex);
            }
            SaveServerList();
        }

        private void ServerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowCapababilities(false);
        }

        private void close_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void wmsUrl_TextChanged(object sender, EventArgs e)
        {

        }

        private void SetAbstractText(string text)
        {
            if (text != null)
            {
                text = text.Replace("<br />", "\n").Replace("<b>", "").Replace("</b>", "");
            }
            Abstract.Text = text;
        }

        private void LayersTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var wmsLayer = LayersTree.SelectedNode.Tag as WMS.Layer;

            var style = LayersTree.SelectedNode.Tag as WMS.Style;

            var tileGroup = LayersTree.SelectedNode.Tag as WMS_Tile_ServiceTiledPatternsTiledGroup;

 
            if (style != null && LayersTree.SelectedNode.Parent != null)
            {
                wmsLayer = LayersTree.SelectedNode.Parent.Tag as WMS.Layer;
            }

            if (wmsLayer != null)
            { 
                add.Enabled = true;

                if (style != null && !string.IsNullOrEmpty(style.Abstract))
                {
                    SetAbstractText(style.Abstract);
                }
                else
                {
                    SetAbstractText(wmsLayer.Abstract);
                }
            } 
            else if (tileGroup != null)
            {
                SetAbstractText(tileGroup.Abstract);
                add.Enabled = true;
            }
            else
            {
                SetAbstractText("");
                add.Enabled = false;
            }

            if (tileGroup != null || (wmsLayer != null && !string.IsNullOrEmpty(wmsLayer.Name)))
            {
                add.Enabled = true;
                if (tileGroup != null)
                {
                    TiledWMS.Enabled = false;
                    TiledWMS.Checked = true;
                    AddAsLayer.Checked = true;
                    AddAsLayer.Enabled = true;
                }
                else if (wmsLayer.IsTileable)
                {
                    TiledWMS.Enabled = true;
                    AddAsLayer.Checked = true;
                    AddAsLayer.Enabled = true;
                }
                else
                {
                    AddAsLayer.Checked = true;
                    AddAsLayer.Enabled = false;
                    TiledWMS.Enabled = false;
                }
                AddAsLayer.Visible = true;
                TiledWMS.Visible = true;
            }
            else
            {
                add.Enabled = false;
                AddAsLayer.Visible = false;
                TiledWMS.Visible = false;
            }
        }

        private void AddAsLayer_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void TiledWMS_CheckedChanged(object sender, EventArgs e)
        {
            if (TiledWMS.Checked == false)
            {
                AddAsLayer.Checked = true;
            }
        }
    }

    struct WmsServerEntry
    {
        public string Name;
        public string Url;

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Url);
        }
    }
}
