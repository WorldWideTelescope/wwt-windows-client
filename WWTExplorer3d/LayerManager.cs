
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ShapefileTools;
using System.Xml;
using System.Net;

namespace TerraViewer
{
    public partial class LayerManager : Form, IUIServicesCallbacks
    {
        static LayerManager master = null;
        static int version = 0;

        public static int Version
        {
            get { return LayerManager.version; }
            set { LayerManager.version = value; }
        }

        public LayerManager()
        {
            InitializeComponent();
            SetUiStrings();
            master = this;
        }

        public static IScriptable ScriptInterface
        {
            get
            {
                return new LayerScripting();
            }
        }

        private void SetUiStrings()
        {
            this.breadCrumbs.Text = Language.GetLocalizedText(664, "Layers");
            this.AddLayer.Text = Language.GetLocalizedText(166, "Add");
            this.DeleteLayer.Text = Language.GetLocalizedText(167, "Delete");
            this.SaveLayers.Text = Language.GetLocalizedText(168, "Save");
            this.pasteLayer.Text = Language.GetLocalizedText(429, "Paste");
            this.resetLayers.Text = Language.GetLocalizedText(663, "Reset");
            this.Text = Language.GetLocalizedText(664, "Layers");
            this.autoLoopCheckbox.Text = Language.GetLocalizedText(665, "Auto Loop");
            this.timeSeries.Text = Language.GetLocalizedText(666, "Time Series");
            this.timeLabel.Text = Language.GetLocalizedText(667, "Time Scrubber");
            this.NameColumn.Text = Language.GetLocalizedText(238, "Name");
            this.ValueColumn.Text = Language.GetLocalizedText(668, "Value");
        }

        public static Layer LoadShapeFile(string path, string currentMap)
        {


            ShapeFile shapefile = new ShapeFile(path);
            shapefile.Read();

            ShapeFileRenderer layer = new ShapeFileRenderer(shapefile);
            layer.Enabled = true;
            layer.Name = path.Substring(path.LastIndexOf('\\') + 1);
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();
            return layer;
        }

        internal static Layer Load3dModelFile(string path, string currentMap)
        {


            Object3dLayer layer = new Object3dLayer();
            layer.LoadData(path);

            layer.Enabled = true;
            layer.Name = path.Substring(path.LastIndexOf('\\') + 1);
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();
            return layer;

        }

        internal static Layer LoadDataTable(string path, string currentMap, bool noInteraction)
        {


            string data = File.ReadAllText(path);
            string layerName = path.Substring(path.LastIndexOf('\\') + 1);


            SpreadSheetLayer layer = new SpreadSheetLayer((string)data, true);
            layer.Enabled = true;
            layer.Name = layerName;


            if (noInteraction || DataWizard.ShowWizard(layer) == DialogResult.OK)
            {
                LayerList.Add(layer.ID, layer);
                layer.ReferenceFrame = currentMap;
                AllMaps[currentMap].Layers.Add(layer);
                AllMaps[currentMap].Open = true;
                LoadTree();

            }
            version++;
            return layer;
        }


        internal static Layer AddImagesetLayer(IImageSet imageSet)
        {
            if (!string.IsNullOrEmpty(CurrentMap))
            {
                ImageSetLayer layer = new ImageSetLayer(imageSet);
                layer.Name = imageSet.Name;
                LayerList.Add(layer.ID, layer);
                layer.ReferenceFrame = CurrentMap;
                AllMaps[CurrentMap].Layers.Add(layer);
                AllMaps[CurrentMap].Open = true;
                LoadTree();
                version++;
                return layer;
            }
            return null;
        }


        public static void Add(Layer layer, bool updateTree)
        {
            if (!LayerList.ContainsKey(layer.ID))
            {
                if (AllMaps.ContainsKey(layer.ReferenceFrame))
                {
                    LayerList.Add(layer.ID, layer);

                    AllMaps[layer.ReferenceFrame].Layers.Add(layer);
                    version++;
                    if (updateTree)
                    {
                        LoadTree();
                    }
                }
            }
        }

        public static VoTableLayer AddVoTableLayer(VoTable table, string title)
        {

            VoTableLayer layer = new VoTableLayer(table);
            layer.Name = title;
            layer.Astronomical = true;
            layer.ReferenceFrame = "Sky";
            LayerList.Add(layer.ID, layer);
            AllMaps["Sky"].Layers.Add(layer);
            AllMaps["Sky"].Open = true;
            layer.Enabled = true;
            version++;
            LoadTree();

            return layer;
        }

        static bool tourLayers = false;

        public static bool TourLayers
        {
            get { return LayerManager.tourLayers; }
            set
            {
                if (LayerManager.tourLayers != value && value == false)
                {
                    ClearLayers();
                    LayerManager.tourLayers = value;
                    LoadTree();
                }
                else if (LayerManager.tourLayers != value && value == true)
                {
                    LayerManager.tourLayers = value;
                    InitLayers();
                }

            }
        }

        static Dictionary<string, LayerMap> layerMaps = new Dictionary<string, LayerMap>();
        static Dictionary<string, LayerMap> layerMapsTours = new Dictionary<string, LayerMap>();

        public static Dictionary<string, LayerMap> LayerMaps
        {
            get
            {
                if (TourLayers)
                {
                    return LayerManager.layerMapsTours;
                }
                else
                {
                    return LayerManager.layerMaps;
                }
            }
            set
            {
                if (TourLayers)
                {
                    LayerManager.layerMapsTours = value;
                }
                else
                {
                    LayerManager.layerMaps = value;
                }
            }
        }

        private static Dictionary<string, LayerMap> allMaps = new Dictionary<string, LayerMap>();
        private static Dictionary<string, LayerMap> allMapsTours = new Dictionary<string, LayerMap>();

        public static Dictionary<string, LayerMap> AllMaps
        {
            get
            {
                if (TourLayers)
                {
                    return LayerManager.allMapsTours;
                }
                else
                {
                    return LayerManager.allMaps;
                }
            }
            set
            {
                if (TourLayers)
                {
                    LayerManager.allMapsTours = value;
                }
                else
                {
                    LayerManager.allMaps = value;
                }
            }
        }

        static string currentMap = "Earth";

        public static string CurrentMap
        {
            get { return LayerManager.currentMap; }
            set { LayerManager.currentMap = value; }
        }

        private static Dictionary<Guid, Layer> layerList = new Dictionary<Guid, Layer>();
        static Dictionary<Guid, Layer> layerListTours = new Dictionary<Guid, Layer>();

        public static Dictionary<Guid, Layer> LayerList
        {
            get
            {
                if (TourLayers)
                {
                    return LayerManager.layerListTours;
                }
                else
                {
                    return LayerManager.layerList;
                }
            }
            set
            {
                if (TourLayers)
                {
                    LayerManager.layerListTours = value;
                }
                else
                {
                    LayerManager.layerList = value;
                }
            }
        }



        //static List<Layer> layers = new List<Layer>();
        static LayerManager()
        {
            InitLayers();
        }

        static void AddIss()
        {
            ISSLayer layer = new ISSLayer();

            layer.Name = Language.GetLocalizedText(1314, "ISS Model  (Toshiyuki Takahei)");
            layer.Enabled = Properties.Settings.Default.ShowISSModel;
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = "ISS";
            AllMaps["ISS"].Layers.Add(layer);
            AllMaps["ISS"].Open = true;
        }

        static public Vector3d GetPrimarySandboxLight()
        {
            LayerMap sandbox = AllMaps["Sandbox"];
            if (sandbox != null)
            {
                foreach (Layer layer in sandbox.Layers)
                {
                    Object3dLayer light = layer as Object3dLayer;
                    if (light != null && light.LightID == 1)
                    {
                        return light.Translate;
                    }
                }
            }

            return new Vector3d(10, 10, 10);
        }
        static public Color GetPrimarySandboxLightColor()
        {
            LayerMap sandbox = AllMaps["Sandbox"];
            if (sandbox != null)
            {
                foreach (Layer layer in sandbox.Layers)
                {
                    Object3dLayer light = layer as Object3dLayer;
                    if (light != null && light.LightID == 1)
                    {
                        return light.Color;
                    }
                }
            }

            return Color.White;
        }

        static public void InitLayers()
        {
            ClearLayers();
            //StockSkyOverlay.StockOverlays.Clear();

            LayerMap iss = null;
            LayerMap ol = null;
            LayerMap l1 = null;
            LayerMap l2 = null;
            if (!TourLayers)
            {
                string[] isstle = new string[0];
                try
                {
                    //This is downloaded now on startup
                    string url = "http://www.worldwidetelescope.org/wwtweb/isstle.aspx";
                    string filename = string.Format(@"{0}data\isstle.txt", Properties.Settings.Default.CahceDirectory);
                    DataSetManager.DownloadFile(url, filename, false, false);

                    isstle = File.ReadAllLines(filename);
                }
                catch
                {
                }

                iss = new LayerMap("ISS", ReferenceFrames.Custom);
                iss.Frame.Epoch = SpaceTimeController.TwoLineDateToJulian("10184.51609218");
                iss.Frame.SemiMajorAxis = 6728829.41;
                iss.Frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
                iss.Frame.Inclination = 51.6442;
                iss.Frame.LongitudeOfAscendingNode = 147.0262;
                iss.Frame.Eccentricity = .0009909;
                iss.Frame.MeanAnomolyAtEpoch = 325.5563;
                iss.Frame.MeanDailyMotion = 360 * 15.72172655;
                iss.Frame.ArgumentOfPeriapsis = 286.4623;
                iss.Frame.Scale = 1;
                iss.Frame.SemiMajorAxisUnits = AltUnits.Meters;
                iss.Frame.MeanRadius = 130;
                iss.Frame.Oblateness = 0;
                iss.Frame.ShowOrbitPath = false;
                if (isstle.Length > 1)
                {
                    try
                    {
                        iss.Frame.FromTLE(isstle[0], isstle[1], 398600441800000);
                    }
                    catch
                    {
                    }
                }
                iss.Enabled = true;
            }
            ol = new LayerMap("Observing Location", ReferenceFrames.Custom);
            ol.Frame.ReferenceFrameType = ReferenceFrameTypes.FixedSherical;
            ol.Frame.ObservingLocation = true;
            ol.Frame.Lat = SpaceTimeController.Location.Lat;
            ol.Frame.Lng = SpaceTimeController.Location.Lng;
            ol.Frame.Altitude = SpaceTimeController.Altitude;
            ol.Frame.SystemGenerated = true;

            l1 = new LayerMap("Sun-Earth L1 Point", ReferenceFrames.Custom);
            l1.Frame.ReferenceFrameType = ReferenceFrameTypes.Synodic;
            l1.Frame.Translation = new Vector3d(-1.5e6, 0.0, 0.0);
            l1.Enabled = false;
            l1.Frame.SystemGenerated = true;

            l2 = new LayerMap("Sun-Earth L2 Point", ReferenceFrames.Custom);
            l2.Frame.ReferenceFrameType = ReferenceFrameTypes.Synodic;
            l2.Frame.Translation = new Vector3d(1.5e6, 0.0, 0.0);
            l2.Enabled = false;
            l2.Frame.SystemGenerated = true;

            LayerMaps.Add("Sun", new LayerMap("Sun", ReferenceFrames.Sun));
            LayerMaps["Sun"].AddChild(new LayerMap("Mercury", ReferenceFrames.Mercury));
            LayerMaps["Sun"].AddChild(new LayerMap("Venus", ReferenceFrames.Venus));
            LayerMaps["Sun"].AddChild(new LayerMap("Earth", ReferenceFrames.Earth));
            LayerMaps["Sun"].ChildMaps["Earth"].Layers.Add(new SkyOverlays(SkyOverlaysType.Earth));
            LayerMaps["Sun"].ChildMaps["Earth"].AddChild(new LayerMap("Moon", ReferenceFrames.Moon));

            if (!TourLayers)
            {
                 LayerMaps["Sun"].ChildMaps["Earth"].AddChild(iss);
            }
            LayerMaps["Sun"].ChildMaps["Earth"].AddChild(ol);  
            LayerMaps["Sun"].ChildMaps["Earth"].AddChild(l1);
            LayerMaps["Sun"].ChildMaps["Earth"].AddChild(l2);

            LayerMaps["Sun"].AddChild(new LayerMap("Mars", ReferenceFrames.Mars));
            LayerMaps["Sun"].AddChild(new LayerMap("Jupiter", ReferenceFrames.Jupiter));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Io", ReferenceFrames.Io));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Europa", ReferenceFrames.Europa));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Ganymede", ReferenceFrames.Ganymede));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Callisto", ReferenceFrames.Callisto));
            LayerMaps["Sun"].AddChild(new LayerMap("Saturn", ReferenceFrames.Saturn));
            LayerMaps["Sun"].AddChild(new LayerMap("Uranus", ReferenceFrames.Uranus));
            LayerMaps["Sun"].AddChild(new LayerMap("Neptune", ReferenceFrames.Neptune));
            LayerMaps["Sun"].AddChild(new LayerMap("Pluto", ReferenceFrames.Pluto));

            AddMoons();

            LayerMaps.Add("Sky", new LayerMap("Sky", ReferenceFrames.Sky));

            LayerMaps["Sky"].Layers.Add(new SkyOverlays(SkyOverlaysType.Sky));
            LayerMaps["Sky"].Layers.Add(new SkyOverlays(SkyOverlaysType.Sky2d));
            LayerMaps["Sky"].Layers.Add(new SkyOverlays(SkyOverlaysType.SolarSystem));
            LayerMaps["Sky"].Open = true;
            LayerMaps["Sun"].Open = true;

            LayerMaps.Add("Sandbox", new LayerMap("Sandbox", ReferenceFrames.Sandbox));

            LayerMaps.Add("Dome", new LayerMap("Dome", ReferenceFrames.Identity));
            LayerMaps["Dome"].Layers.Add(new SkyOverlays(SkyOverlaysType.Dome));
            LayerMaps["Dome"].Open = true;




            AllMaps.Clear();

            AddAllMaps(LayerMaps, null);
            if (!TourLayers)
            {
                AddIss();
            }
            version++;
            LoadTree();


        }

        private static void ClearLayers()
        {
            foreach (Layer layer in LayerList.Values)
            {
                layer.CleanUp();
            }

            LayerList.Clear();
            LayerMaps.Clear();
        }

        private static void AddMoons()
        {
            string filename = Properties.Settings.Default.CahceDirectory + "\\data\\moons.txt";
            DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=moons", filename, false, true);

            string[] data = File.ReadAllLines(filename);

            bool first = true;
            foreach (string line in data)
            {
                if (first)
                {
                    first = false;
                    continue;
                }
                string[] parts = line.Split(new char[] { '\t' });
                string planet = parts[0];
                LayerMap frame = new LayerMap(parts[2], ReferenceFrames.Custom);
                frame.Frame.SystemGenerated = true;
                frame.Frame.Epoch = double.Parse(parts[1]);
                frame.Frame.SemiMajorAxis = double.Parse(parts[3]) * 1000;
                frame.Frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
                frame.Frame.Inclination = double.Parse(parts[7]);
                frame.Frame.LongitudeOfAscendingNode = double.Parse(parts[8]);
                frame.Frame.Eccentricity = double.Parse(parts[4]);
                frame.Frame.MeanAnomolyAtEpoch = double.Parse(parts[6]);
                frame.Frame.MeanDailyMotion = double.Parse(parts[9]);
                frame.Frame.ArgumentOfPeriapsis = double.Parse(parts[5]);
                frame.Frame.Scale = 1;
                frame.Frame.SemiMajorAxisUnits = AltUnits.Meters;
                frame.Frame.MeanRadius = double.Parse(parts[16]) * 1000;
                frame.Frame.RotationalPeriod = double.Parse(parts[17]);
                frame.Frame.ShowAsPoint = false;
                frame.Frame.ShowOrbitPath = true;
                frame.Frame.RepresentativeColor = Color.LightBlue;
                frame.Frame.Oblateness = 0;

                LayerMaps["Sun"].ChildMaps[planet].AddChild(frame);
            }
        }

        private static void AddAllMaps(Dictionary<string, LayerMap> maps, String parent)
        {
            foreach (LayerMap map in maps.Values)
            {
                map.Frame.Parent = parent;
                AllMaps.Add(map.Name, map);
                AddAllMaps(map.ChildMaps, map.Name);
            }
        }



        bool initialized = false;
        private void Layers_Load(object sender, EventArgs e)
        {
            breadcrumbs.Push(Language.GetLocalizedText(664, "Layers"));
            if (LayerMaps != null)
            {
                LoadTree();
            }
            startDate.Text = "";
            endDate.Text = "";

            Properties.Settings.Default.PropertyChanged += new PropertyChangedEventHandler(Default_PropertyChanged);
        }

        public static bool ProcessingUpdate = false;

        public static int updateCount = 0;

        bool needTreeUpdate = false;

        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Prevent recursion... and stack overflow.
            if (!ProcessingUpdate)
            {
                needTreeUpdate = true;
            }
        }


        public static void SyncLayerState()
        {
            if (master != null)
            {
                if (master.InvokeRequired)
                {
                    MethodInvoker CallSync = delegate
                    {
                        master.SyncEnabledStatus();
                    };
                    try
                    {
                        master.Invoke(CallSync);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    master.SyncEnabledStatus();
                }
            }
        }

        private void SyncEnabledStatus()
        {
            SyncTree(layerTree.Nodes["Sky"]);
            SyncTree(layerTree.Nodes["Dome"]);
            SyncTree(layerTree.Nodes["Sun"].Nodes["Earth"]);
        }

        private void SyncTree(TreeNode node)
        {

            Layer layer = node.Tag as Layer;

            if (layer != null && layer.Enabled != node.Checked)
            {
                node.Checked = layer.Enabled;
            }

            LayerUITreeNode lutn = node.Tag as LayerUITreeNode;

            if (lutn != null )
            {
                if (lutn.Checked != node.Checked)
                {
                    node.Checked = lutn.Checked;
                }
                SkyOverlay sol = lutn.Tag as SkyOverlay;

                if (sol != null && sol.Enabled != node.Checked)
                {
                    node.Checked = sol.Enabled;
                }
            }
            

            foreach (TreeNode child in node.Nodes)
            {
                SyncTree(child);
            }
        }

        public static void LoadTree()
        {
            if (master != null)
            {

                if (master.InvokeRequired)
                {
                    MethodInvoker updatePlace = delegate
                        {
                            master.LoadTreeLocal();
                        };
                    try
                    {
                        master.Invoke(updatePlace);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    master.LoadTreeLocal();
                }
            }
        }


        static object currentSelection = null;

        public static object CurrentSelection
        {
            get { return LayerManager.currentSelection; }
            set { LayerManager.currentSelection = value; }
        }
        TreeNode nodeCurrentSelection = null;
        Stack<object> breadcrumbs = new Stack<object>();
        private void LoadTreeLocal()
        {
            string text = "";
            foreach (object item in breadcrumbs)
            {
                if (tourLayers && text == "")
                {
                    text = Language.GetLocalizedText(980, "Tour Layers") + " >";
                }
                else
                {
                    text = item.ToString() + "  > " + text;
                }
            }
            //todo add ellipsis if this is too long.. not here but where it draws it
            breadCrumbs.Text = text;

            if (breadcrumbs.Count < 2)
            {
                foundRoot = true;
            }
            else
            {
                foundRoot = false;
            }

            layerTree.BeginUpdate();
            nodeCurrentSelection = null;

            if (layerTree.SelectedNode != null)
            {
                currentSelection = layerTree.SelectedNode.Tag;
            }
            layerTree.Nodes.Clear();

            bool solarSystem = Earth3d.MainWindow.SolarSystemMode;

            LoadTreeMaps(LayerMaps, layerTree.Nodes);

            try
            {
                layerTree.SelectedNode = nodeCurrentSelection;
            }
            catch
            {
            }
            initialized = true;
            layerTree.EndUpdate();
        }
        bool foundRoot = false;

        private void LoadTreeMaps(Dictionary<string, LayerMap> LayerMaps, TreeNodeCollection treeNodeCollection)
        {
            foreach (LayerMap map in LayerMaps.Values)
            {
                bool keepLooking = true;
                if (!foundRoot && map == breadcrumbs.Peek())
                {
                    foundRoot = true;
                    keepLooking = false;
                }



                TreeNodeCollection nodeCollextion = treeNodeCollection;
                TreeNode frame = null;
                if (foundRoot && keepLooking)
                {
                    frame = treeNodeCollection.Add(map.Name);
                    nodeCollextion = frame.Nodes;
                    frame.Tag = map;
                    frame.Name = map.Name;
                    frame.Checked = map.Enabled;
                    if (map.Frame.Reference == ReferenceFrames.Identity)
                    {
                        frame.ForeColor = Color.LightBlue;
                    }
                    else
                    {

                        frame.ForeColor = Color.CornflowerBlue;
                    }
                    if (currentSelection == map)
                    {
                        nodeCurrentSelection = frame;
                    }
                }

                List<Layer> layers = map.Layers;
                foreach (Layer layer in layers)
                {
                    bool loadChildred = true;
                    if (!foundRoot && layer == breadcrumbs.Peek())
                    {
                        foundRoot = false;
                        loadChildred = false;
                        keepLooking = false;
                    }

                    if (foundRoot && loadChildred)
                    {
                        TreeNode node = nodeCollextion.Add(layer.Name);
                        node.Tag = layer;
                        node.Checked = layer.Enabled;
                        node.Name = node.Text;

                        Layer sel = currentSelection as Layer;
                        if (currentSelection == layer || (sel != null && layer.ID == sel.ID))
                        {
                            nodeCurrentSelection = node;
                        }
                        if (layer.Opened)
                        {
                            LoadLayerChildren(layer, node);
                        }

                    }
                    if (!loadChildred)
                    {
                        foundRoot = false;
                        break;
                    }

                }


                LoadTreeMaps(map.ChildMaps, nodeCollextion);

                if (frame != null && map.Open)
                {
                    frame.Expand();
                }

                if (!keepLooking)
                {
                    foundRoot = false;
                    break;
                }
            }
        }

        private void LoadLayerChildren(Layer layer, TreeNode node)
        {
            LayerUI layerUI = layer.GetPrimaryUI();
            layerUI.SetUICallbacks(this);

            if (layerUI == null || !layerUI.HasTreeViewNodes)
            {
                return;
            }
            List<LayerUITreeNode> nodes = layerUI.GetTreeNodes();
            foreach (LayerUITreeNode layerNode in nodes)
            {
                LoadLayerChild(layerNode, node);
            }
            node.Expand();
        }

        private void LoadLayerChild(LayerUITreeNode layerNode, TreeNode parent)
        {
            TreeNode node = parent.Nodes.Add(layerNode.Name);
            node.Tag = layerNode;
            node.Checked = layerNode.Checked;
            node.Name = node.Text;
            layerNode.ReferenceTag = node;
            layerNode.NodeUpdated += new LayerUITreeNodeUpdatedDelegate(layerNode_NodeUpdated);

            foreach (LayerUITreeNode child in layerNode.Nodes)
            {
                LoadLayerChild(child, node);
            }

            if (node.IsExpanded != layerNode.Opened)
            {
                node.Expand();
            }
        }

        void layerNode_NodeUpdated(LayerUITreeNode layerNode)
        {
            if (layerNode.UiUpdating)
            {
                // ignore events we started.
                return;
            }
            TreeNode node = layerNode.ReferenceTag as TreeNode;
            if (node != null)
            {
                if (node.Checked != layerNode.Checked)
                {
                    node.Checked = layerNode.Checked;
                }

                if (node.Text != layerNode.Name)
                {
                    node.Text = layerNode.Name;
                }

                if (node.IsExpanded != layerNode.Opened)
                {
                    if (layerNode.Opened)
                    {
                        node.Expand();
                    }
                    else
                    {
                        node.Collapse();
                    }
                }
            }
        }


        private void LoadTreeLocalOld()
        {
            TreeNode currentSelection = layerTree.SelectedNode;

            layerTree.Nodes.Clear();

            bool solarSystem = Earth3d.MainWindow.SolarSystemMode;


            foreach (string name in Enum.GetNames(typeof(ReferenceFrames)))
            {
                LayerMap map = LayerMaps[name];
                List<Layer> layers = map.Layers;
                TreeNode frame = layerTree.Nodes.Add(name);
                frame.Tag = map;
                frame.Checked = map.Enabled;

                foreach (Layer layer in layers)
                {
                    TreeNode node = frame.Nodes.Add(layer.Name);
                    node.Tag = layer;
                    node.Checked = layer.Enabled;
                }

                if (map.Open)
                {
                    frame.Expand();
                }
            }
            try
            {
                layerTree.SelectedNode = currentSelection;
            }
            catch
            {
            }
            initialized = true;
        }

        // This is only for Edit Mode Tours... Not for Tours Layers
        public static bool CheckForTourLoadedLayers()
        {
            foreach (Layer layer in layerList.Values)
            {
                if (layer.LoadedFromTour)
                {
                    return true;
                }
            }
            return false;
        }

        internal static void CloseAllTourLoadedLayers()
        {
            List<Guid> purgeTargets = new List<Guid>();
            foreach (Layer layer in layerList.Values)
            {
                if (layer.LoadedFromTour)
                {
                    purgeTargets.Add(layer.ID);
                }
            }

            foreach (Guid guid in purgeTargets)
            {
                DeleteLayerByID(guid, true, false);
            }

            List<string> purgeMapsNames = new List<string>();

            foreach (LayerMap map in AllMaps.Values)
            {
                if (map.LoadedFromTour && map.Layers.Count == 0)
                {
                    purgeMapsNames.Add(map.Name);
                }
            }

            foreach (string name in purgeMapsNames)
            {
                PurgeLayerMapDeep(AllMaps[name], true);
            }



            Version++;
            LoadTree();

        }

        internal static void CleanAllTourLoadedLayers()
        {
            foreach (Layer layer in layerList.Values)
            {
                if (layer.LoadedFromTour)
                {
                    //todo We may want to copy layers into a temp directory later, for now we are just leaving the layer data files in the temp tour directory. 
                    layer.LoadedFromTour = false;
                }
            }
        }

        // Merged layers from Tour Player Alternate universe into the real layer manager layers list
        public static void MergeToursLayers()
        {

            tourLayers = false;
            bool OverWrite = false;
            bool CollisionChecked = false;

            foreach (LayerMap map in allMapsTours.Values)
            {
                if (!allMaps.ContainsKey(map.Name))
                {
                    LayerMap newMap = new LayerMap(map.Name, ReferenceFrames.Custom);
                    newMap.Frame = map.Frame;
                    newMap.LoadedFromTour = true;
                    LayerManager.AllMaps.Add(newMap.Name, newMap);
                }
            }
            ConnectAllChildren();


            foreach (Layer layer in layerListTours.Values)
            {
                if (LayerList.ContainsKey(layer.ID))
                {
                    if (!CollisionChecked)
                    {
                        if (UiTools.ShowMessageBox(Language.GetLocalizedText(958, "There are layers with the same name. Overwrite existing layers?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            OverWrite = true;
                        }
                        else
                        {
                            OverWrite = false;
                        }
                        CollisionChecked = true;
                    }

                    if (OverWrite)
                    {
                        LayerManager.DeleteLayerByID(layer.ID, true, false);
                    }
                }

                if (!LayerList.ContainsKey(layer.ID))
                {
                    if (AllMaps.ContainsKey(layer.ReferenceFrame))
                    {
                        LayerList.Add(layer.ID, layer);

                        AllMaps[layer.ReferenceFrame].Layers.Add(layer);
                    }
                }
                else
                {
                    layer.CleanUp();
                }
            }

            layerListTours.Clear();
            allMapsTours.Clear();
            layerMapsTours.Clear();
            LoadTree();
        }

        private void CheckAllChildNodes(TreeNode node)
        {
            foreach (TreeNode child in node.Nodes)
            {
                LayerUITreeNode uiNode = child.Tag as LayerUITreeNode;
                if (uiNode != null)
                {
                    uiNode.Checked = node.Checked;
                    uiNode.FireNodeChecked(node.Checked);
                }
                child.Checked = node.Checked;
                CheckAllChildNodes(child);
            }
        }

        private void layerTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!initialized)
            {
                return;
            }

            TreeNode node = e.Node;
            if (node != null && node.Tag is LayerUITreeNode)
            {
                if (e.Action != TreeViewAction.Unknown)
                {

                    LayerUITreeNode layerNode = node.Tag as LayerUITreeNode;
                    layerNode.UiUpdating = true;

                    layerNode.Checked = node.Checked;
                    layerNode.FireNodeChecked(node.Checked);
                    layerNode.UiUpdating = false;
                }
            }


            if (e.Node.Tag != null && e.Node.Tag is Layer)
            {
                Layer layer = (Layer)e.Node.Tag;
                layer.Enabled = e.Node.Checked;
                if (e.Node.Checked != layer.Enabled)
                {
                    e.Node.Checked = layer.Enabled;
                }
                version++;
                if (Control.ModifierKeys == Keys.Shift && !(e.Node.Tag is SkyOverlay))
                {
                    CheckAllChildNodes(e.Node);
                }
            }

            if (e.Node.Tag != null && e.Node.Tag is LayerMap)
            {
                LayerMap layerMap = (LayerMap)e.Node.Tag;
                layerMap.Enabled = e.Node.Checked;
                version++;

                if (layerMap.Frame.Reference == ReferenceFrames.Identity)
                {
                    foreach (Layer layer in layerMap.Layers)
                    {
                        layer.Enabled = layerMap.Enabled;
                    }

                    foreach (TreeNode child in e.Node.Nodes)
                    {
                        child.Checked = layerMap.Enabled;
                    }
                }
            }

        }


        private void AddLayer_Click(object sender, EventArgs e)
        {
            Earth3d.LoadLayerFile(false);
            version++;
        }

        private void AddFeed_Click(object sender, EventArgs e)
        {
            Earth3d.LoadODATAFeed();
            version++;
        }

        ContextMenuStrip contextMenu = null;

        private void layerTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }

            layerTree.SelectedNode = e.Node;

            if (e.Button == MouseButtons.Right)
            {
                if (layerTree.SelectedNode != null && ((layerTree.SelectedNode.Tag is Layer) && !(layerTree.SelectedNode.Tag is SkyOverlays)))
                {
                    Layer selectedLayer = (Layer)layerTree.SelectedNode.Tag;

                    contextMenu = new ContextMenuStrip();
                    ToolStripMenuItem renameMenu = new ToolStripMenuItem(Language.GetLocalizedText(225, "Rename"));
                    ToolStripMenuItem Expand = new ToolStripMenuItem(Language.GetLocalizedText(981, "Expand"));
                    ToolStripMenuItem Collapse = new ToolStripMenuItem(Language.GetLocalizedText(982, "Collapse"));
                    ToolStripMenuItem copyMenu = new ToolStripMenuItem(Language.GetLocalizedText(428, "Copy"));
                    ToolStripMenuItem deleteMenu = new ToolStripMenuItem(Language.GetLocalizedText(167, "Delete"));
                    ToolStripMenuItem saveMenu = new ToolStripMenuItem(Language.GetLocalizedText(960, "Save..."));
                    ToolStripMenuItem publishMenu = new ToolStripMenuItem(Language.GetLocalizedText(983, "Publish to Community..."));
                    ToolStripMenuItem colorMenu = new ToolStripMenuItem(Language.GetLocalizedText(458, "Color/Opacity"));
                    ToolStripMenuItem opacityMenu = new ToolStripMenuItem(Language.GetLocalizedText(305, "Opacity"));
                    ToolStripMenuItem addToTimeline = new ToolStripMenuItem(Language.GetLocalizedText(1290, "Add to Timeline"));
                    ToolStripMenuItem addKeyframe = new ToolStripMenuItem(Language.GetLocalizedText(1280, "Add Keyframe"));

                    ToolStripMenuItem popertiesMenu = new ToolStripMenuItem(Language.GetLocalizedText(20, "Properties"));
                    ToolStripMenuItem scaleMenu = new ToolStripMenuItem(Language.GetLocalizedText(1291, "Scale/Histogram"));
                    ToolStripMenuItem showGraphTool = new ToolStripMenuItem(Language.GetLocalizedText(1292, "Show Graph Tool"));
                    ToolStripMenuItem barChartChoose = new ToolStripMenuItem(Language.GetLocalizedText(1293, "Bar Chart Columns"));
                    ToolStripMenuItem lifeTimeMenu = new ToolStripMenuItem(Language.GetLocalizedText(683, "Lifetime"));
                    ToolStripSeparator spacer1 = new ToolStripSeparator();
                    ToolStripMenuItem top = new ToolStripMenuItem(Language.GetLocalizedText(684, "Move to Top"));
                    ToolStripMenuItem up = new ToolStripMenuItem(Language.GetLocalizedText(685, "Move Up"));
                    ToolStripMenuItem down = new ToolStripMenuItem(Language.GetLocalizedText(686, "Move Down"));
                    ToolStripMenuItem bottom = new ToolStripMenuItem(Language.GetLocalizedText(687, "Move to Bottom"));
                    ToolStripMenuItem showViewer = new ToolStripMenuItem(Language.GetLocalizedText(957, "VO Table Viewer"));

                    ToolStripSeparator spacer2 = new ToolStripSeparator();
                    ToolStripMenuItem dynamicData = new ToolStripMenuItem(Language.GetLocalizedText(984, "Dynamic Data"));

                    ToolStripMenuItem autoRefresh = new ToolStripMenuItem(Language.GetLocalizedText(985, "Auto Refresh"));
                    ToolStripMenuItem refreshNow = new ToolStripMenuItem(Language.GetLocalizedText(986, "Refresh Now"));
                    
                    ToolStripMenuItem defaultImageset = new ToolStripMenuItem(Language.GetLocalizedText(1294, "Background Image Set"));
                    

                    top.Click += new EventHandler(top_Click);
                    up.Click += new EventHandler(up_Click);
                    down.Click += new EventHandler(down_Click);
                    bottom.Click += new EventHandler(bottom_Click);
                    saveMenu.Click += new EventHandler(saveMenu_Click);
                    publishMenu.Click += new EventHandler(publishMenu_Click);
                    Expand.Click += new EventHandler(Expand_Click);
                    Collapse.Click += new EventHandler(Collapse_Click);
                    copyMenu.Click += new EventHandler(copyMenu_Click);
                    colorMenu.Click += new EventHandler(colorMenu_Click);
                    deleteMenu.Click += new EventHandler(deleteMenu_Click);
                    renameMenu.Click += new EventHandler(renameMenu_Click);
                    addToTimeline.Click += new EventHandler(addToTimeline_Click);
                    addKeyframe.Click += new EventHandler(addKeyframe_Click);
                    popertiesMenu.Click += new EventHandler(popertiesMenu_Click);
                    scaleMenu.Click += new EventHandler(scaleMenu_Click);

                    autoRefresh.Click += new EventHandler(autoRefresh_Click);
                    refreshNow.Click += new EventHandler(refreshNow_Click);

                    defaultImageset.Click += new EventHandler(defaultImageset_Click);

                    barChartChoose.DropDownOpening += new EventHandler(barChartChoose_DropDownOpening);

                    ToolStripMenuItem Histogram = new ToolStripMenuItem(Language.GetLocalizedText(863, "Histogram"));
                    ToolStripMenuItem DomainBarchar = new ToolStripMenuItem(Language.GetLocalizedText(1295, "Barchart by Domain Values"));
                    ToolStripMenuItem TimeChart = new ToolStripMenuItem(Language.GetLocalizedText(1296, "Time Chart"));
                    ToolStripMenuItem OpenedCharts = new ToolStripMenuItem(Language.GetLocalizedText(1297, "Current Filters"));


                    DomainBarchar.DropDownOpening += new EventHandler(showGraphTool_DropDownOpening);
                    TimeChart.DropDownOpening += new EventHandler(TimeChart_DropDownOpening);
                    Histogram.DropDownOpening += new EventHandler(Histogram_DropDownOpened);
                    showGraphTool.DropDownItems.Add(Histogram);
                    showGraphTool.DropDownItems.Add(DomainBarchar);
                    showGraphTool.DropDownItems.Add(TimeChart);
                    showGraphTool.DropDownItems.Add(OpenedCharts);

                    opacityMenu.Click += new EventHandler(opacityMenu_Click);
                    lifeTimeMenu.Click += new EventHandler(lifeTimeMenu_Click);
                    showViewer.Click += new EventHandler(showViewer_Click);
                    OpenedCharts.DropDownOpening += new EventHandler(OpenedCharts_DropDownOpening);
                    contextMenu.Items.Add(renameMenu);

                    if (!selectedLayer.Opened && selectedLayer.GetPrimaryUI() != null && selectedLayer.GetPrimaryUI().HasTreeViewNodes)
                    {
                        contextMenu.Items.Add(Expand);

                    }

                    if (selectedLayer.Opened)
                    {
                        contextMenu.Items.Add(Collapse);
                    }


                    if (selectedLayer.CanCopyToClipboard())
                    {
                        contextMenu.Items.Add(copyMenu);
                    }

                    contextMenu.Items.Add(deleteMenu);
                    contextMenu.Items.Add(saveMenu);

                    if (Earth3d.IsLoggedIn)
                    {
                        contextMenu.Items.Add(publishMenu);
                    }

                    contextMenu.Items.Add(spacer2);
                    contextMenu.Items.Add(colorMenu);
                    contextMenu.Items.Add(opacityMenu);

                    // ToDo Should we have this only show up in layers under Identity Reference Frames?
                    contextMenu.Items.Add(lifeTimeMenu);

                    if (selectedLayer is SpreadSheetLayer)
                    {

                        SpreadSheetLayer sslayer = selectedLayer as SpreadSheetLayer;
                        if (sslayer.DynamicData)
                        {
                            autoRefresh.Checked = sslayer.AutoUpdate;
                            dynamicData.DropDownItems.Add(autoRefresh);
                            dynamicData.DropDownItems.Add(refreshNow);
                            contextMenu.Items.Add(dynamicData);
                        }
                    }


                    if (layerTree.SelectedNode.Tag is ImageSetLayer)
                    {
                        contextMenu.Items.Add(defaultImageset);

                        ImageSetLayer isl = layerTree.SelectedNode.Tag as ImageSetLayer;
                        defaultImageset.Checked = isl.OverrideDefaultLayer;
                    }

                    if (layerTree.SelectedNode.Tag is SpreadSheetLayer || layerTree.SelectedNode.Tag is Object3dLayer || layerTree.SelectedNode.Tag is GroundOverlayLayer || layerTree.SelectedNode.Tag is GreatCirlceRouteLayer || layerTree.SelectedNode.Tag is OrbitLayer)
                    {
                        if (Earth3d.MainWindow.TourEdit != null && Earth3d.MainWindow.TourEdit.Tour.EditMode && Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop != null)
                        {
                            if (Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.IsTargetAnimated(selectedLayer.GetIndentifier()))
                            {
                                contextMenu.Items.Add(addKeyframe);
                            }
                            else
                            {
                                contextMenu.Items.Add(addToTimeline);
                            }
                        }
                        contextMenu.Items.Add(popertiesMenu);
                    }

                    if (layerTree.SelectedNode.Tag is VoTableLayer)
                    {
                        contextMenu.Items.Add(showViewer);
                    }

                    if (layerTree.SelectedNode.Tag is ImageSetLayer)
                    {
                        ImageSetLayer isl = layerTree.SelectedNode.Tag as ImageSetLayer;
                        if (isl.FitsImage != null)
                        {
                            contextMenu.Items.Add(scaleMenu);
                        }
                    }

                    if (AllMaps[selectedLayer.ReferenceFrame].Layers.Count > 1)
                    {
                        contextMenu.Items.Add(spacer1);
                        contextMenu.Items.Add(top);
                        contextMenu.Items.Add(up);
                        contextMenu.Items.Add(down);
                        contextMenu.Items.Add(bottom);
                    }


                    contextMenu.Show(Cursor.Position);
                }
                else if (layerTree.SelectedNode != null && layerTree.SelectedNode.Tag is LayerMap)
                {
                    LayerMap map = layerTree.SelectedNode.Tag as LayerMap;
                    bool sandbox = map.Frame.Reference.ToString() == "Sandbox";
                    bool Dome = map.Frame.Name == "Dome";
                    bool Sky = map.Frame.Name == "Sky";

                    if (Dome)
                    {
                        return;
                    }
                    contextMenu = new ContextMenuStrip();
                    ToolStripMenuItem trackFrame = new ToolStripMenuItem(Language.GetLocalizedText(1298, "Track this frame"));
                    ToolStripMenuItem goTo = new ToolStripMenuItem(Language.GetLocalizedText(1299, "Fly Here"));
                    ToolStripMenuItem showOrbit = new ToolStripMenuItem("Show Orbit");
                    ToolStripMenuItem newMenu = new ToolStripMenuItem(Language.GetLocalizedText(674, "New Reference Frame"));
                    ToolStripMenuItem newLayerGroupMenu = new ToolStripMenuItem(Language.GetLocalizedText(675, "New Layer Group"));
                    ToolStripMenuItem addMenu = new ToolStripMenuItem(Language.GetLocalizedText(166, "Add"));
                    ToolStripMenuItem newLight = new ToolStripMenuItem("Add Light");
                    ToolStripMenuItem addFeedMenu = new ToolStripMenuItem(Language.GetLocalizedText(956, "Add OData/table feed as Layer"));
                    ToolStripMenuItem addWmsLayer = new ToolStripMenuItem(Language.GetLocalizedText(987, "New WMS Layer"));
                    ToolStripMenuItem addGirdLayer = new ToolStripMenuItem(Language.GetLocalizedText(1300, "New Lat/Lng Grid"));
                    ToolStripMenuItem addGreatCircle = new ToolStripMenuItem(Language.GetLocalizedText(988, "New Great Circle"));
                    ToolStripMenuItem importTLE = new ToolStripMenuItem(Language.GetLocalizedText(989, "Import Orbital Elements"));
                    ToolStripMenuItem addMpc = new ToolStripMenuItem(Language.GetLocalizedText(1301, "Add Minor Planet"));
                    ToolStripMenuItem deleteFrameMenu = new ToolStripMenuItem(Language.GetLocalizedText(167, "Delete"));
                    ToolStripMenuItem pasteMenu = new ToolStripMenuItem(Language.GetLocalizedText(425, "Paste"));
                    ToolStripMenuItem addToTimeline = new ToolStripMenuItem(Language.GetLocalizedText(1290, "Add to Timeline"));
                    ToolStripMenuItem addKeyframe = new ToolStripMenuItem(Language.GetLocalizedText(1280, "Add Keyframe"));

                    ToolStripMenuItem popertiesMenu = new ToolStripMenuItem(Language.GetLocalizedText(20, "Properties"));
                    ToolStripMenuItem saveMenu = new ToolStripMenuItem(Language.GetLocalizedText(990, "Save Layers"));
                    ToolStripMenuItem publishLayers = new ToolStripMenuItem(Language.GetLocalizedText(991, "Publish Layers to Community"));
                    ToolStripSeparator spacer1 = new ToolStripSeparator();
                    ToolStripSeparator spacer0 = new ToolStripSeparator();
                    ToolStripSeparator spacer2 = new ToolStripSeparator();
                    ToolStripMenuItem asReferenceFrame = new ToolStripMenuItem("As Reference Frame");
                    ToolStripMenuItem asOrbitalLines = new ToolStripMenuItem("As Orbital Line");


                    trackFrame.Click += new EventHandler(trackFrame_Click);
                    goTo.Click += new EventHandler(goTo_Click);
                    asReferenceFrame.Click += new EventHandler(addMpc_Click);
                    asOrbitalLines.Click += AsOrbitalLines_Click; 
                    // Ad Sub Menus
                    addMpc.DropDownItems.Add(asReferenceFrame);
                    addMpc.DropDownItems.Add(asOrbitalLines);




                    addMenu.Click += new EventHandler(addMenu_Click);
                    newLight.Click += new EventHandler(newLight_Click);
                    addFeedMenu.Click += new EventHandler(addFeedMenu_Click);
                    newLayerGroupMenu.Click += new EventHandler(newLayerGroupMenu_Click);
                    pasteMenu.Click += new EventHandler(pasteLayer_Click);
                    newMenu.Click += new EventHandler(newMenu_Click);
                    deleteFrameMenu.Click += new EventHandler(deleteFrameMenu_Click);
                    addToTimeline.Click +=new EventHandler(addToTimeline_Click);
                    addKeyframe.Click += new EventHandler(addKeyframe_Click);
                    popertiesMenu.Click += new EventHandler(FramePropertiesMenu_Click);
                    addWmsLayer.Click += new EventHandler(addWmsLayer_Click);
                    importTLE.Click += new EventHandler(importTLE_Click);
                    addGreatCircle.Click += new EventHandler(addGreatCircle_Click);
                    saveMenu.Click += new EventHandler(SaveLayers_Click);
                    publishLayers.Click += new EventHandler(publishLayers_Click);
                    addGirdLayer.Click += new EventHandler(addGirdLayer_Click);


                    ToolStripMenuItem convertToOrbit = new ToolStripMenuItem("Extract Orbit Layer");
                    convertToOrbit.Click += ConvertToOrbit_Click;


                    if (map.Frame.Reference != ReferenceFrames.Identity)
                    {
                        if (Earth3d.MainWindow.SolarSystemMode | Earth3d.MainWindow.SandboxMode) //&& Control.ModifierKeys == Keys.Control)
                        {
                            bool spacerNeeded = false;
                            if (map.Frame.Reference != ReferenceFrames.Custom && !Earth3d.MainWindow.SandboxMode)
                            {
                                // fly to
                                if (!Sky)
                                {
                                    contextMenu.Items.Add(goTo);
                                    spacerNeeded = true;
                                }
                                
                                try
                                {
                                    string name = map.Frame.Reference.ToString();
                                    if (name != "Sandbox")
                                    {
                                        SolarSystemObjects ssObj = (SolarSystemObjects)Enum.Parse(typeof(SolarSystemObjects), name, true);
                                        int id = (int)ssObj;

                                        int bit = (int)Math.Pow(2, id);

                                        showOrbit.Checked = (Properties.Settings.Default.PlanetOrbitsFilter & bit) != 0;
                                        showOrbit.Click += new EventHandler(showOrbitPlanet_Click);
                                        showOrbit.Tag = bit.ToString();
                                    }
                                }
                                catch
                                {
                                }
                            }
                            else
                            {
                                // track
                                if (!sandbox && !Sky)
                                {
                                    contextMenu.Items.Add(trackFrame);
                                    spacerNeeded = true;
                                }
                                showOrbit.Checked = map.Frame.ShowOrbitPath;
                                showOrbit.Click += new EventHandler(showOrbit_Click);
                            }

                            if (spacerNeeded)
                            {
                                contextMenu.Items.Add(spacer2);
                            }

                            if (!Sky && !sandbox)
                            {
                                contextMenu.Items.Add(showOrbit);

                                contextMenu.Items.Add(spacer0);
                            }

                            if (map.Frame.Reference.ToString() == "Sandbox")
                            {
                                contextMenu.Items.Add(newLight);
                            }
                        }

                        if (!Sky)
                        {
                            contextMenu.Items.Add(newMenu);
                        }
                        contextMenu.Items.Add(newLayerGroupMenu);
                        
                    }
                    
                    contextMenu.Items.Add(addMenu);
                    contextMenu.Items.Add(addFeedMenu);
                    if (!Sky)
                    {
                        contextMenu.Items.Add(addGreatCircle);
                        contextMenu.Items.Add(addGirdLayer);
                    }

                    if ((map.Frame.Reference != ReferenceFrames.Identity && map.Frame.Name == "Sun") ||
                        (map.Frame.Reference == ReferenceFrames.Identity && map.Parent != null && map.Parent.Frame.Name == "Sun"))
                    {
                        contextMenu.Items.Add(addMpc);
                    }

                    if (map.Frame.Reference == ReferenceFrames.Custom && map.Frame.ReferenceFrameType == ReferenceFrameTypes.Orbital && map.Parent != null && map.Parent.Frame.Name == "Sun")
                    {
                        contextMenu.Items.Add(convertToOrbit);
                    }


                    if (!Sky)
                    {
                        contextMenu.Items.Add(addWmsLayer);
                    }

                    
                    contextMenu.Items.Add(pasteMenu);
                   

                    if (map.Frame.Reference == ReferenceFrames.Identity)
                    {
                        contextMenu.Items.Add(deleteFrameMenu);
                    }

                    if (map.Frame.Reference == ReferenceFrames.Custom)
                    {
                        contextMenu.Items.Add(deleteFrameMenu);

                        if (Earth3d.MainWindow.TourEdit != null && Earth3d.MainWindow.TourEdit.Tour.EditMode && Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop != null)
                        {
                            if (Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.IsTargetAnimated(map.Frame.GetIndentifier()))
                            {
                                contextMenu.Items.Add(addKeyframe);
                            }
                            else
                            {
                                contextMenu.Items.Add(addToTimeline);
                            }
                        }
                        contextMenu.Items.Add(popertiesMenu);

                    }

                    //if (!Sky)
                    {
                        contextMenu.Items.Add(spacer1);
                    }
                    contextMenu.Items.Add(saveMenu);
                    if (Earth3d.IsLoggedIn)
                    {
                        contextMenu.Items.Add(publishLayers);
                    }


                    contextMenu.Show(Cursor.Position);
                }
                else if (layerTree.SelectedNode != null && layerTree.SelectedNode.Tag is LayerUITreeNode)
                {
                    LayerUITreeNode node = layerTree.SelectedNode.Tag as LayerUITreeNode;
                    contextMenu = new ContextMenuStrip();

                    Layer layer = GetParentLayer(layerTree.SelectedNode);

                    if (layer != null)
                    {
                        LayerUI ui = layer.GetPrimaryUI();
                        List<LayerUIMenuItem> items = ui.GetNodeContextMenu(node);

                        if (items != null)
                        {
                            foreach (LayerUIMenuItem item in items)
                            {
                                ToolStripMenuItem menuItem = new ToolStripMenuItem(item.Name);
                                menuItem.Tag = item;
                                menuItem.Click += new EventHandler(menuItem_Click);
                                contextMenu.Items.Add(menuItem);

                                if (item.SubMenus != null)
                                {
                                    foreach (LayerUIMenuItem subItem in item.SubMenus)
                                    {
                                        ToolStripMenuItem subMenuItem = new ToolStripMenuItem(subItem.Name);
                                        subMenuItem.Tag = subItem;
                                        subMenuItem.Click += new EventHandler(menuItem_Click);
                                        menuItem.DropDownItems.Add(subMenuItem);
                                    }
                                }
                            }
                            contextMenu.Show(Cursor.Position);
                        }

                        
                    }
                }
            }

        }

        private void ConvertToOrbit_Click(object sender, EventArgs e)
        {
            LayerMap map = layerTree.SelectedNode.Tag as LayerMap;
            this.ConvertToTLE(map);

        }

        void newLight_Click(object sender, EventArgs e)
        {
            LayerMap map = layerTree.SelectedNode.Tag as LayerMap;

            Object3dLayer layer = new Object3dLayer();
            layer.LightID = 1;
            layer.Name = "Primary Light";
            layer.ReferenceFrame = map.Name;
            map.Layers.Add(layer);
            LayerList.Add(layer.ID, layer);
            LoadTree();

        }

        void showOrbit_Click(object sender, EventArgs e)
        {
            // Flip the state
            LayerMap map = layerTree.SelectedNode.Tag as LayerMap;

            map.Frame.ShowOrbitPath = !map.Frame.ShowOrbitPath;
        }

        void showOrbitPlanet_Click(object sender, EventArgs e)
        {
            try
            {
                int bit = int.Parse(((ToolStripMenuItem)sender).Tag.ToString());

                // Flip the state
                if ((Properties.Settings.Default.PlanetOrbitsFilter & bit) == 0)
                {
                    Properties.Settings.Default.PlanetOrbitsFilter |= bit;
                }
                else
                {
                    Properties.Settings.Default.PlanetOrbitsFilter &= ~bit;
                }

            }
            catch
            {
            }
        }

        void goTo_Click(object sender, EventArgs e)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;

            IPlace place = Search.FindCatalogObjectExact(target.Frame.Reference.ToString());
            if (place != null)
            {
                Earth3d.MainWindow.GotoTarget(place, false, false, true);
            }
        }

        void addKeyframe_Click(object sender, EventArgs e)
        {
            IAnimatable target = layerTree.SelectedNode.Tag as IAnimatable;
            if (target == null)
            {
                if (layerTree.SelectedNode.Tag is LayerMap)
                {
                    target = ((LayerMap)layerTree.SelectedNode.Tag).Frame;
                }
            }

            if (Earth3d.MainWindow.TourEdit != null && target != null)
            {
                if (Earth3d.MainWindow.TourEdit.Tour.EditMode && Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop != null)
                {
                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1280, "Add Keyframe"), Earth3d.MainWindow.TourEdit.Tour));

                    Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.KeyFramed = true;

                    AnimationTarget aniTarget = Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.FindTarget(target.GetIndentifier());
                    if (aniTarget != null)
                    {
                        aniTarget.SetKeyFrame(Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.TweenPosition, Key.KeyType.Linear);
                        TimeLine.RefreshUi(false);
                    }
                }
            }
        }

        void addToTimeline_Click(object sender, EventArgs e)
        {
            IAnimatable target = layerTree.SelectedNode.Tag as IAnimatable;
            AnimationTarget.AnimationTargetTypes type = AnimationTarget.AnimationTargetTypes.Layer;
            if (target == null)
            {
                if (layerTree.SelectedNode.Tag is LayerMap)
                {
                    target = ((LayerMap)layerTree.SelectedNode.Tag).Frame;
                    type = AnimationTarget.AnimationTargetTypes.ReferenceFrame;
                }
            }

            if (Earth3d.MainWindow.TourEdit != null && target != null)
            {
                if (Earth3d.MainWindow.TourEdit.Tour.EditMode && Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop != null)
                {
                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1290, "Add to Timeline"), Earth3d.MainWindow.TourEdit.Tour));

                    Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.KeyFramed = true;

                    AnimationTarget aniTarget = new AnimationTarget(Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop);
                    aniTarget.Target = target;
                    aniTarget.TargetType = type;
                    aniTarget.ParameterNames.AddRange(target.GetParamNames());
                    aniTarget.CurrentParameters = target.GetParams();
                    aniTarget.SetKeyFrame(0, Key.KeyType.Linear);
                    //todo add end frames?

                    Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.AnimationTargets.Add(aniTarget);
                    TimeLine.RefreshUi(false);
                }
            }
        }

        void addMpc_Click(object sender, EventArgs e)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;
            SimpleInput input = new SimpleInput(Language.GetLocalizedText(1302, "Minor planet name or designation"), Language.GetLocalizedText(238, "Name"), "", 32);
            bool retry = false;
            do
            {
                if (input.ShowDialog() == DialogResult.OK)
                {
                    if (target.ChildMaps.ContainsKey(input.ResultText))
                    {
                        retry = true;
                        UiTools.ShowMessageBox("That Name already exists");
                    }
                    else
                    {
                        try
                        {
                            GetMpc(input.ResultText);
                            retry = false;
                        }
                        catch
                        {
                            retry = true;
                            UiTools.ShowMessageBox(Language.GetLocalizedText(1303, "The designation was not found or the MPC service was unavailable"));
                        }
                    }
                }
                else
                {
                    retry = false;
                }
            } while (retry);
            return;
        }
        private void AsOrbitalLines_Click(object sender, EventArgs e)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;
            SimpleInput input = new SimpleInput(Language.GetLocalizedText(1302, "Minor planet name or designation"), Language.GetLocalizedText(238, "Name"), "", 32);
            bool retry = false;
            do
            {
                if (input.ShowDialog() == DialogResult.OK)
                {
                    if (target.ChildMaps.ContainsKey(input.ResultText))
                    {
                        retry = true;
                        UiTools.ShowMessageBox("That Name already exists");
                    }
                    else
                    {
                        try
                        {
                            GetMpcAsTLE(input.ResultText);
                            retry = false;
                        }
                        catch
                        {
                            retry = true;
                            UiTools.ShowMessageBox(Language.GetLocalizedText(1303, "The designation was not found or the MPC service was unavailable"));
                        }
                    }
                }
                else
                {
                    retry = false;
                }
            } while (retry);
            return;
        }


        string GetMpcAsTLE(string id)
        {

            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;
            WebClient client = new WebClient();

            string data = client.DownloadString("http://www.minorplanetcenter.net/db_search/show_object?object_id=" + id);


            int startform = data.IndexOf("show-orbit-button");

            int lastForm = data.IndexOf("/form", startform);

            string formpart = data.Substring(startform, lastForm - startform);

            string name = id;

            ReferenceFrame frame = new ReferenceFrame();

            frame.Oblateness = 0;
            frame.ShowOrbitPath = true;
            frame.ShowAsPoint = true;
            
            frame.Epoch = SpaceTimeController.UtcToJulian(DateTime.Parse(GetValueByID(formpart, "epoch").Substring(0, 10)));
            frame.SemiMajorAxis = double.Parse(GetValueByID(formpart, "a")) * UiTools.KilometersPerAu * 1000;
            frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
            frame.Inclination = double.Parse(GetValueByID(formpart, "incl"));
            frame.LongitudeOfAscendingNode = double.Parse(GetValueByID(formpart, "node"));
            frame.Eccentricity = double.Parse(GetValueByID(formpart, "e"));
            frame.MeanAnomolyAtEpoch = double.Parse(GetValueByID(formpart, "m"));
            frame.MeanDailyMotion = CAAElliptical.MeanMotionFromSemiMajorAxis(double.Parse(GetValueByID(formpart, "a")));
            frame.ArgumentOfPeriapsis = double.Parse(GetValueByID(formpart, "peri"));
            frame.Scale = 1;
            frame.SemiMajorAxisUnits = AltUnits.Meters;
            frame.MeanRadius = 10;
            frame.Oblateness = 0;

            String TLE = name + "\n" + frame.ToTLE();

            String filename = Path.GetTempPath() + "\\" + name;

            File.WriteAllText(filename, TLE);

            LoadOrbitsFile(filename, target.Name);

            LoadTree();

            return null;
        }

        string ConvertToTLE( LayerMap map)
        {

            LayerMap target = map.Parent;

            ReferenceFrame frame = map.Frame;
            string name = frame.Name;

            String TLE = name + "\n" + frame.ToTLE();

            String filename = Path.GetTempPath() + "\\" + name;

            File.WriteAllText(filename, TLE);

            LoadOrbitsFile(filename, target.Name);

            LoadTree();

            return null;
        }

        string GetMpc(string id)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;


            WebClient client = new WebClient();

            string data = client.DownloadString("http://www.minorplanetcenter.net/db_search/show_object?object_id=" + id);


            int startform = data.IndexOf("show-orbit-button");

            int lastForm = data.IndexOf("/form", startform);

            string formpart = data.Substring(startform, lastForm - startform);

            string name = id;

            LayerMap orbit = new LayerMap(name.Trim(), ReferenceFrames.Custom);
            orbit.Frame.Oblateness = 0;
            orbit.Frame.ShowOrbitPath = true;
            orbit.Frame.ShowAsPoint = true;

            orbit.Frame.Epoch = SpaceTimeController.UtcToJulian(DateTime.Parse(GetValueByID(formpart, "epoch").Substring(0,10)));
            orbit.Frame.SemiMajorAxis = double.Parse(GetValueByID(formpart, "a")) * UiTools.KilometersPerAu * 1000;
            orbit.Frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
            orbit.Frame.Inclination = double.Parse(GetValueByID(formpart, "incl"));
            orbit.Frame.LongitudeOfAscendingNode = double.Parse(GetValueByID(formpart, "node"));
            orbit.Frame.Eccentricity = double.Parse(GetValueByID(formpart, "e"));
            orbit.Frame.MeanAnomolyAtEpoch = double.Parse(GetValueByID(formpart, "m"));
            orbit.Frame.MeanDailyMotion = CAAElliptical.MeanMotionFromSemiMajorAxis(double.Parse(GetValueByID(formpart, "a")));
            orbit.Frame.ArgumentOfPeriapsis = double.Parse(GetValueByID(formpart, "peri"));
            orbit.Frame.Scale = 1;
            orbit.Frame.SemiMajorAxisUnits = AltUnits.Meters;
            orbit.Frame.MeanRadius = 10;
            orbit.Frame.Oblateness = 0;

            if (!AllMaps[target.Name].ChildMaps.ContainsKey(name.Trim()))
            {
                AllMaps[target.Name].AddChild(orbit);
            }

            AllMaps.Add(orbit.Name, orbit);

            orbit.Frame.Parent = target.Name;

            MakeLayerGroup("Minor Planet", orbit);

            LoadTree();

            return null;
        }

        string GetValueByID(string data, string id)
        {
          
            int valStart = data.IndexOf("id=\"" + id + "\"");
            valStart = data.IndexOf("value=", valStart)+7;
            int valEnd = data.IndexOf("\"",valStart);
            return data.Substring(valStart, valEnd - valStart);
        }

        void addGirdLayer_Click(object sender, EventArgs e)
        {
            GridLayer layer = new GridLayer();

            layer.Enabled = true;
            layer.Name = "Lat-Lng Grid";
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();

        }

        void defaultImageset_Click(object sender, EventArgs e)
        {
            ImageSetLayer isl = layerTree.SelectedNode.Tag as ImageSetLayer;
            isl.OverrideDefaultLayer = ! isl.OverrideDefaultLayer;
        }

        void menuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item != null)
            {
                LayerUIMenuItem menuItem = item.Tag as LayerUIMenuItem;
                if (menuItem != null)
                {
                    menuItem.FireMenuItemSelected();
                }
            }
        }

        Layer GetParentLayer(TreeNode node)
        {
            if (node == null)
            {
                return null;
            }

            if (node.Tag is Layer)
            {
                return node.Tag as Layer;
            }
            else
            {
                return GetParentLayer(node.Parent);
            }
        }

        void OpenedCharts_DropDownOpening(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = layerTree.SelectedNode.Tag as SpreadSheetLayer;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            int index = 0;
            if (item.DropDownItems.Count == 0)
            {
                if (layer.Filters.Count > 0)
                {
                    foreach (FilterGraphTool fgt in layer.Filters)
                    {
                        ToolStripMenuItem filterItem = new ToolStripMenuItem(fgt.Title);
                        filterItem.Click += new EventHandler(filterItem_Click);
                        item.DropDownItems.Add(filterItem);
                        filterItem.Tag = fgt;
                        index++;
                    }
                }
                else
                {
                    item.Enabled = false;
                }

            }

        }

        void filterItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            Earth3d.MainWindow.UiController = item.Tag as FilterGraphTool;
        }


        void trackFrame_Click(object sender, EventArgs e)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;

            Earth3d.MainWindow.SolarSystemTrack = SolarSystemObjects.Custom;
            Earth3d.MainWindow.TrackingFrame = target.Name;
            Earth3d.MainWindow.viewCamera.Zoom = Earth3d.MainWindow.targetViewCamera.Zoom = .000000001;


        }

        void scaleMenu_Click(object sender, EventArgs e)
        {
            ImageSetLayer isl = layerTree.SelectedNode.Tag as ImageSetLayer;
            Histogram.ShowHistogram(isl.ImageSet, false, Cursor.Position);
        }

        void publishLayers_Click(object sender, EventArgs e)
        {
            if (Earth3d.IsLoggedIn)
            {

                LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;

                string name = target.Name + ".wwtl";
                string filename = Path.GetTempFileName();

                LayerContainer layers = new LayerContainer();
                layers.TopLevel = target.Name;
                layers.SaveToFile(filename);
                layers.Dispose();
                GC.SuppressFinalize(layers);
                EOCalls.InvokePublishFile(filename, name);
                File.Delete(filename);

                Earth3d.RefreshCommunity();

            }
        }

        void refreshNow_Click(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = layerTree.SelectedNode.Tag as SpreadSheetLayer;

            if (layer != null)
            {
                layer.DynamicUpdate();
            }
        }

        void autoRefresh_Click(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = layerTree.SelectedNode.Tag as SpreadSheetLayer;

            if (layer != null)
            {
                layer.AutoUpdate = !layer.AutoUpdate;
            }
        }

        void publishMenu_Click(object sender, EventArgs e)
        {

            if (Earth3d.IsLoggedIn)
            {

                Layer target = (Layer)layerTree.SelectedNode.Tag;

                string name = target.Name + ".wwtl";
                string filename = Path.GetTempFileName();

                LayerContainer layers = new LayerContainer();
                layers.SoloGuid = target.ID;

                layers.SaveToFile(filename);
                layers.Dispose();
                GC.SuppressFinalize(layers);
                EOCalls.InvokePublishFile(filename, name);
                File.Delete(filename);

                Earth3d.RefreshCommunity();

            }
        }

        void SaveLayers_Click(object sender, EventArgs e)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = Language.GetLocalizedText(992, "WorldWide Telescope Layer File") + "(*.wwtl)|*.wwtl";
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = ".wwtl";
            saveDialog.FileName = target.Name + ".wwtl";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                // Todo add dialog for dynamic content options.
                LayerContainer layers = new LayerContainer();
                layers.TopLevel = target.Name;
                layers.SaveToFile(saveDialog.FileName);
                layers.Dispose();
                GC.SuppressFinalize(layers);
            }
        }

        void saveMenu_Click(object sender, EventArgs e)
        {
            Layer layer = (Layer)layerTree.SelectedNode.Tag;
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = Language.GetLocalizedText(993, "WorldWide Telescope Layer File(*.wwtl)") + "|*.wwtl";
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = ".wwtl";
            saveDialog.FileName = layer.Name + ".wwtl";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                // Todo add dialog for dynamic content options.
                LayerContainer layers = new LayerContainer();
                layers.SoloGuid = layer.ID;
                layers.SaveToFile(saveDialog.FileName);
                layers.Dispose();
                GC.SuppressFinalize(layers);
            }
        }

        void barChartChoose_DropDownOpening(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = (SpreadSheetLayer)layerTree.SelectedNode.Tag;

            ToolStripMenuItem item = sender as ToolStripMenuItem;
            int index = 0;
            if (item.DropDownItems.Count == 0)
            {
                foreach (string col in layer.Header)
                {
                    ToolStripMenuItem barChartColumn = new ToolStripMenuItem(col);
                    barChartColumn.Click += new EventHandler(barChartColumn_Click);
                    item.DropDownItems.Add(barChartColumn);
                    barChartColumn.Checked = (layer.BarChartBitmask & (int)Math.Pow(2, index)) > 0;
                    barChartColumn.Tag = index;
                    index++;
                }
            }
        }

        void barChartColumn_Click(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = (SpreadSheetLayer)layerTree.SelectedNode.Tag;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            int col = 0;
            foreach (string headerName in layer.Header)
            {
                if (headerName == item.Text)
                {
                    break;
                }
                col++;
            }

            layer.BarChartBitmask = layer.BarChartBitmask ^ (int)Math.Pow(2, col);
            layer.CleanUp();
        }

        void TimeChart_DropDownOpening(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = (SpreadSheetLayer)layerTree.SelectedNode.Tag;

            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if (item.DropDownItems.Count == 0)
            {
                foreach (string col in layer.Header)
                {
                    ToolStripMenuItem timeChild = new ToolStripMenuItem(col);
                    timeChild.DropDownOpening += new EventHandler(timeChild_Click);
                    item.DropDownItems.Add(timeChild);
                }
            }
        }

        void timeChild_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            targetItemText = item.Text;


            if (item.DropDownItems.Count == 0)
            {
                foreach (string dateFilter in Enum.GetNames(typeof(DateFilter)))
                {
                    ToolStripMenuItem dateFilterChild = new ToolStripMenuItem(dateFilter);

                    dateFilterChild.Click += new EventHandler(dateFilterChild_Click);

                    item.DropDownItems.Add(dateFilterChild);
                }
            }

        }

        void dateFilterChild_Click(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = (SpreadSheetLayer)layerTree.SelectedNode.Tag;

            ToolStripMenuItem item = sender as ToolStripMenuItem;

            FilterGraphTool fgt = new FilterGraphTool((SpreadSheetLayer)layerTree.SelectedNode.Tag);
            fgt.ChartType = ChartTypes.TimeChart;
            fgt.StatType = StatTypes.Count;
            Earth3d.MainWindow.UiController = fgt;
           
            int col = 0;
            foreach (string headerName in layer.Header)
            {
                if (headerName == targetItemText)
                {
                    fgt.TargetColumn = col;
                    break;
                }
                col++;
            }

            fgt.DateFilter = (DateFilter)Enum.Parse(typeof(DateFilter), item.Text);
            ConnectLayerUi(layer);
            layer.AddFilter(fgt);
        }


        void Histogram_DropDownOpened(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = (SpreadSheetLayer)layerTree.SelectedNode.Tag;

            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if (item.DropDownItems.Count == 0)
            {
                foreach (string col in layer.Header)
                {
                    ToolStripMenuItem histogramChild = new ToolStripMenuItem(col);
                    histogramChild.Click += new EventHandler(histogramChild_Click);
                    item.DropDownItems.Add(histogramChild);
                }
            }
        }

        void histogramChild_Click(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = (SpreadSheetLayer)layerTree.SelectedNode.Tag;

            ToolStripMenuItem item = sender as ToolStripMenuItem;

            FilterGraphTool fgt = new FilterGraphTool((SpreadSheetLayer)layerTree.SelectedNode.Tag);
            fgt.ChartType = ChartTypes.Histogram;
            fgt.StatType = StatTypes.Count;
            Earth3d.MainWindow.UiController = fgt;
           
            int col = 0;
            foreach (string headerName in layer.Header)
            {
                if (headerName == item.Text)
                {
                    fgt.TargetColumn = col;
                    break;
                }
                col++;
            }
            ConnectLayerUi(layer);
            layer.AddFilter(fgt);
        }

        void ConnectLayerUi(Layer layer)
        {
            LayerUI layerUI = layer.GetPrimaryUI();
            layerUI.SetUICallbacks(this);
        }

        void addGreatCircle_Click(object sender, EventArgs e)
        {
            AddGreatCircleLayer();
        }

        void showGraphTool_DropDownOpening(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = (SpreadSheetLayer)layerTree.SelectedNode.Tag;

            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if (item.DropDownItems.Count == 0)
            {
                foreach (string col in layer.Header)
                {
                    ToolStripMenuItem child = new ToolStripMenuItem(col);
                    child.DropDownOpening += new EventHandler(child_DropDownOpening);
                    item.DropDownItems.Add(child);
                }
            }



        }

        string targetItemText = "";
        string statTypeText = "";
        void child_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            targetItemText = item.Text;


            if (item.DropDownItems.Count == 0)
            {
                foreach (string statType in Enum.GetNames(typeof(StatTypes)))
                {
                    ToolStripMenuItem statTypeChild = new ToolStripMenuItem(statType);
                    if (statType == StatTypes.Ratio.ToString())
                    {
                        statTypeChild.DropDownOpening += new EventHandler(statTypeChild_DropDownOpening);
                    }
                    else
                    {
                        statTypeChild.Click += new EventHandler(child_Click);
                    }
                    item.DropDownItems.Add(statTypeChild);
                }
            }

        }

        void statTypeChild_DropDownOpening(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = (SpreadSheetLayer)layerTree.SelectedNode.Tag;
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            statTypeText = item.Text;
            if (item.DropDownItems.Count == 0)
            {
                foreach (string col in layer.Header)
                {
                    ToolStripMenuItem denominatorMenu = new ToolStripMenuItem(col);
                    denominatorMenu.Click += new EventHandler(denominatorMenu_Click);
                    item.DropDownItems.Add(denominatorMenu);
                }
            }
        }

        void denominatorMenu_Click(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = (SpreadSheetLayer)layerTree.SelectedNode.Tag;

            ToolStripMenuItem item = sender as ToolStripMenuItem;

            FilterGraphTool fgt = new FilterGraphTool((SpreadSheetLayer)layerTree.SelectedNode.Tag);
            fgt.StatType = (StatTypes)Enum.Parse(typeof(StatTypes), statTypeText);
            fgt.ChartType = ChartTypes.BarChart;
            Earth3d.MainWindow.UiController = fgt;
            
            fgt.DomainColumn = layer.NameColumn;
            int col = 0;
            foreach (string headerName in layer.Header)
            {
                if (headerName == targetItemText)
                {
                    fgt.TargetColumn = col;
                    break;
                }
                col++;
            }

            col = 0;
            foreach (string headerName in layer.Header)
            {
                if (headerName == item.Text)
                {
                    fgt.DenominatorColumn = col;
                    break;
                }
                col++;
            }
            ConnectLayerUi(layer);
            layer.AddFilter(fgt);
        }

        void child_Click(object sender, EventArgs e)
        {
            SpreadSheetLayer layer = (SpreadSheetLayer)layerTree.SelectedNode.Tag;

            ToolStripMenuItem item = sender as ToolStripMenuItem;

            FilterGraphTool fgt = new FilterGraphTool((SpreadSheetLayer)layerTree.SelectedNode.Tag);
            fgt.StatType = (StatTypes)Enum.Parse(typeof(StatTypes), item.Text);
            fgt.ChartType = ChartTypes.BarChart;
            Earth3d.MainWindow.UiController = fgt;
            
            fgt.DomainColumn = layer.NameColumn;
            int col = 0;
            foreach (string headerName in layer.Header)
            {
                if (headerName == targetItemText)
                {
                    fgt.TargetColumn = col;
                    break;
                }
                col++;
            }
            ConnectLayerUi(layer);
            layer.AddFilter(fgt);
        }



        void showViewer_Click(object sender, EventArgs e)
        {
            if (layerTree.SelectedNode.Tag is VoTableLayer)
            {
                VoTableLayer layer = layerTree.SelectedNode.Tag as VoTableLayer;

                if (layer.Viewer != null)
                {
                    layer.Viewer.Show();
                }
                else
                {
                    VOTableViewer viewer = new VOTableViewer();
                    viewer.Layer = layer;
                    viewer.Show();
                }
            }
        }

        void bottom_Click(object sender, EventArgs e)
        {
            Layer layer = layerTree.SelectedNode.Tag as Layer;
            if (layer != null)
            {
                AllMaps[layer.ReferenceFrame].Layers.Remove(layer);
                AllMaps[layer.ReferenceFrame].Layers.Add(layer);
            }
            version++;
            LoadTree();
        }

        void down_Click(object sender, EventArgs e)
        {
            Layer layer = layerTree.SelectedNode.Tag as Layer;
            if (layer != null)
            {
                int index = AllMaps[layer.ReferenceFrame].Layers.LastIndexOf(layer);
                if (index < (AllMaps[layer.ReferenceFrame].Layers.Count - 1))
                {
                    AllMaps[layer.ReferenceFrame].Layers.Remove(layer);
                    AllMaps[layer.ReferenceFrame].Layers.Insert(index + 1, layer);
                }
            }
            version++;
            LoadTree();
        }

        void up_Click(object sender, EventArgs e)
        {
            Layer layer = layerTree.SelectedNode.Tag as Layer;
            if (layer != null)
            {
                int index = AllMaps[layer.ReferenceFrame].Layers.LastIndexOf(layer);
                if (index > 0)
                {
                    AllMaps[layer.ReferenceFrame].Layers.Remove(layer);
                    AllMaps[layer.ReferenceFrame].Layers.Insert(index - 1, layer);
                }
            }
            version++;
            LoadTree();
        }

        void top_Click(object sender, EventArgs e)
        {
            Layer layer = layerTree.SelectedNode.Tag as Layer;
            if (layer != null)
            {
                AllMaps[layer.ReferenceFrame].Layers.Remove(layer);
                AllMaps[layer.ReferenceFrame].Layers.Insert(0, layer);
            }
            version++;
            LoadTree();
        }

        void importTLE_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Language.GetLocalizedText(994, "Orbital Elements File (TLE)") + "|*.tle;*.txt";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filename = openFile.FileName;
                try
                {
                    ImportTLEFile(filename);
                    version++;
                }
                catch
                {
                    MessageBox.Show(Language.GetLocalizedText(1304, "This file does not seem to be valid"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                }
            }
        }


        private void ImportTLEFile(string filename)
        {
            string name = filename.Substring(filename.LastIndexOf("\\") + 1);
            name = name.Substring(0, name.LastIndexOf("."));

            MakeLayerGroup(name);


            string[] data = File.ReadAllLines(filename);

            for (int i = 0; i < data.Length; i += 3)
            {
                LayerMap orbit = new LayerMap(data[i].Trim(), ReferenceFrames.Custom);
                orbit.Frame.Oblateness = 0;
                orbit.Frame.ShowOrbitPath = true;
                orbit.Frame.ShowAsPoint = true;
                orbit.Frame.Epoch = SpaceTimeController.TwoLineDateToJulian("10184.51609218");
                orbit.Frame.SemiMajorAxis = 6728829.41;
                orbit.Frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
                orbit.Frame.Inclination = 51.6442;
                orbit.Frame.LongitudeOfAscendingNode = 147.0262;
                orbit.Frame.Eccentricity = .0009909;
                orbit.Frame.MeanAnomolyAtEpoch = 325.5563;
                orbit.Frame.MeanDailyMotion = 360 * 15.72172655;
                orbit.Frame.ArgumentOfPeriapsis = 286.4623;
                orbit.Frame.Scale = 1;
                orbit.Frame.SemiMajorAxisUnits = AltUnits.Meters;
                orbit.Frame.MeanRadius = 10;
                orbit.Frame.Oblateness = 0;                   
                orbit.Frame.FromTLE(data[i + 1], data[i + 2], 398600441800000);
                if (!AllMaps[name].ChildMaps.ContainsKey(data[i].Trim()))
                {
                    AllMaps[name].AddChild(orbit);
                }
            }
            version++;
        }

        void addWmsLayer_Click(object sender, EventArgs e)
        {
            WmsLayerWizard wms = new WmsLayerWizard();
            wms.ShowDialog();
        }

        void Collapse_Click(object sender, EventArgs e)
        {
            Layer selectedLayer = (Layer)layerTree.SelectedNode.Tag;
            selectedLayer.Opened = false;
            layerTree.SelectedNode.Nodes.Clear();
        }

        void Expand_Click(object sender, EventArgs e)
        {
            Layer selectedLayer = (Layer)layerTree.SelectedNode.Tag;
            selectedLayer.Opened = true;
            LoadLayerChildren(selectedLayer, layerTree.SelectedNode);
            layerTree.SelectedNode.Expand();
            version++;
        }

        void copyMenu_Click(object sender, EventArgs e)
        {
            if (layerTree.SelectedNode != null && layerTree.SelectedNode.Tag != null && layerTree.SelectedNode.Tag is Layer)
            {
                Layer node = (Layer)layerTree.SelectedNode.Tag;
                node.CopyToClipboard();
            }
        }

        void newLayerGroupMenu_Click(object sender, EventArgs e)
        {
            bool badName = true;
            string name = Language.GetLocalizedText(676, "Enter Layer Group Name");
            while (badName)
            {
                SimpleInput input = new SimpleInput(name, Language.GetLocalizedText(238, "Name"), Language.GetLocalizedText(677, "Layer Group"), 100);
                if (input.ShowDialog() == DialogResult.OK)
                {
                    name = input.ResultText;
                    if (!AllMaps.ContainsKey(name))
                    {
                        MakeLayerGroup(name);
                        version++;
                        badName = false;
                        LoadTreeLocal();
                    }
                    else
                    {
                        UiTools.ShowMessageBox(Language.GetLocalizedText(1374, "Choose a unique name"), Language.GetLocalizedText(676, "Enter Layer Group Name"));
                    }

                }
                else
                {
                    badName = false;
                }
            }
        }

        private void MakeLayerGroup(string name)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;
            MakeLayerGroup(name, target);
        }

        private static void MakeLayerGroup(string name, LayerMap target)
        {
            ReferenceFrame frame = new ReferenceFrame();
            frame.Name = name;
            frame.Reference = ReferenceFrames.Identity;
            LayerMap newMap = new LayerMap(frame.Name, ReferenceFrames.Identity);
            newMap.Frame = frame;
            newMap.Frame.SystemGenerated = false;
            target.AddChild(newMap);

            newMap.Frame.Parent = target.Name;
            AllMaps.Add(frame.Name, newMap);
            version++;
        }

        void lifeTimeMenu_Click(object sender, EventArgs e)
        {
            if (layerTree.SelectedNode.Tag is Layer)
            {
                LayerLifetimeProperties props = new LayerLifetimeProperties();
                props.Target = (Layer)layerTree.SelectedNode.Tag;
                if (props.ShowDialog() == DialogResult.OK)
                {
                    // This might be moot
                    props.Target.CleanUp();
                }
            }

        }

        void deleteFrameMenu_Click(object sender, EventArgs e)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;
            if (UiTools.ShowMessageBox(Language.GetLocalizedText(678, "This will delete this reference frame and all nested reference frames and layers. Do you want to continue?"), Language.GetLocalizedText(680, "Delete Reference Frame"), MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                PurgeLayerMapDeep(target, true);
                version++;
                LoadTreeLocal();
            }


        }

        public static void PurgeLayerMapDeep(LayerMap target, bool topLevel)
        {

            foreach (Layer layer in target.Layers)
            {
                LayerManager.DeleteLayerByID(layer.ID, false, false);
            }

            target.Layers.Clear();

            foreach (LayerMap map in target.ChildMaps.Values)
            {
                PurgeLayerMapDeep(map, false);
            }

            target.ChildMaps.Clear();
            if (topLevel)
            {
                if (!String.IsNullOrEmpty(target.Frame.Parent))
                {
                    if (AllMaps.ContainsKey(target.Frame.Parent))
                    {
                        AllMaps[target.Frame.Parent].ChildMaps.Remove(target.Name);
                    }
                }
                else
                {
                    if (LayerMaps.ContainsKey(target.Name))
                    {
                        LayerMaps.Remove(target.Name);
                    }
                }
            }
            AllMaps.Remove(target.Name);
            version++;
        }


        void FramePropertiesMenu_Click(object sender, EventArgs e)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;

            ReferenceFrame frame = new ReferenceFrame();
            if (FrameWizard.ShowPropertiesSheet(target.Frame) == DialogResult.OK)
            {

            }
        }

        void newMenu_Click(object sender, EventArgs e)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;
            ReferenceFrame frame = new ReferenceFrame();
            frame.SystemGenerated = false;
            if (FrameWizard.ShowWizard(frame) == DialogResult.OK)
            {
                LayerMap newMap = new LayerMap(frame.Name, ReferenceFrames.Custom);
                if (!AllMaps.ContainsKey(frame.Name))
                {
                    newMap.Frame = frame;

                    target.AddChild(newMap);
                    newMap.Frame.Parent = target.Name;
                    AllMaps.Add(frame.Name, newMap);
                    version++;
                    LoadTreeLocal();
                }
            }
        }


        public static bool CreateReferenceFrame(string name, string parent, string xml)
        {
            if (!AllMaps.ContainsKey(name) && AllMaps.ContainsKey(parent))
            {
                LayerMap target = AllMaps[parent];
                ReferenceFrame frame = new ReferenceFrame();
                frame.Name = name;

                if (!string.IsNullOrEmpty(xml))
                {
                    if (!frame.SetProps(xml))
                    {
                        return false;
                    }
                }

                LayerMap newMap = new LayerMap(frame.Name, ReferenceFrames.Custom);
                newMap.Frame = frame;
                target.AddChild(newMap);
                newMap.Frame.Parent = target.Name;
                AllMaps.Add(frame.Name, newMap);

                version++;
                LoadTree();
                return true;
            }
            return false;
        }



        public static string GetFramePropsByName(string name)
        {
            if (AllMaps.ContainsKey(name))
            {
                ReferenceFrame frame = AllMaps[name].Frame;

                return frame.GetProps();
            }
            else
            {
                return "";
            }
        }

        public static string GetFramePropByName(string name, string propName, out ReferenceFrame frame)
        {
            if (AllMaps.ContainsKey(name))
            {
                frame = AllMaps[name].Frame;

                return frame.GetProp(propName);
            }
            else
            {
                frame = null;
                return "";
            }

        }

        public static bool SetFramePropsByName(string name, string xml)
        {
            if (AllMaps.ContainsKey(name))
            {
                ReferenceFrame frame = AllMaps[name].Frame;
             
                bool retVal = frame.SetProps(xml);
                //LoadTree();
                return retVal;
            }
            else
            {
                return false;
            }
        }

        public static bool SetFramePropByName(string name, string propName, string propValue)
        {
            if (AllMaps.ContainsKey(name))
            {
                ReferenceFrame frame = AllMaps[name].Frame;
                return frame.SetProp(propName, propValue);
            }
            else
            {
                return false;
            }
        }

        void opacityMenu_Click(object sender, EventArgs e)
        {
            OpacityPopup popup = new OpacityPopup();
            popup.Target = (Layer)layerTree.SelectedNode.Tag;
            popup.Location = Cursor.Position;
            popup.StartPosition = FormStartPosition.Manual;
            popup.Show();

        }

        void popertiesMenu_Click(object sender, EventArgs e)
        {
            if (layerTree.SelectedNode.Tag is SpreadSheetLayer)
            {
                SpreadSheetLayer target = (SpreadSheetLayer)layerTree.SelectedNode.Tag;
                DataWizard.ShowPropertiesSheet(target);

                target.CleanUp();
                LoadTree();
            }
            else if (layerTree.SelectedNode.Tag is SpreadSheetLayer || layerTree.SelectedNode.Tag is Object3dLayer)
            {
                Object3dProperties props = new Object3dProperties();
                props.layer = (Object3dLayer)layerTree.SelectedNode.Tag;
                //   props.ShowDialog();
                props.Owner = Earth3d.MainWindow;
                props.Show();
            }
            else if (layerTree.SelectedNode.Tag is GroundOverlayLayer)
            {
                GroundOverlayProperties props = new GroundOverlayProperties();
                props.Overlay = ((GroundOverlayLayer)layerTree.SelectedNode.Tag).Overlay;
                props.OverlayLayer = ((GroundOverlayLayer)layerTree.SelectedNode.Tag);
                props.Owner = Earth3d.MainWindow;
                props.Show();
            }
            else if (layerTree.SelectedNode.Tag is GreatCirlceRouteLayer)
            {
                GreatCircleProperties props = new GreatCircleProperties();
                props.Layer = ((GreatCirlceRouteLayer)layerTree.SelectedNode.Tag);
                props.Owner = Earth3d.MainWindow;
                props.Show();
            }
        }

        void renameMenu_Click(object sender, EventArgs e)
        {
            Layer layer = (Layer)layerTree.SelectedNode.Tag;
            SimpleInput input = new SimpleInput(Language.GetLocalizedText(225, "Rename"), Language.GetLocalizedText(228, "New Name"), layer.Name, 32);

            if (input.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(input.ResultText))
                {
                    layer.Name = input.ResultText;
                    version++;
                    LoadTree();
                }
            }

        }

        void colorMenu_Click(object sender, EventArgs e)
        {
            Layer layer = (Layer)layerTree.SelectedNode.Tag;
            PopupColorPicker picker = new PopupColorPicker();

            picker.Location = Cursor.Position;

            picker.Color = layer.Color;

            if (picker.ShowDialog() == DialogResult.OK)
            {
                layer.Color = picker.Color;
            }
        }

        void addMenu_Click(object sender, EventArgs e)
        {
            bool overridable = false;
            if (layerTree.SelectedNode != null && layerTree.SelectedNode.Tag is LayerMap)
            {
                LayerMap map = layerTree.SelectedNode.Tag as LayerMap;
                if (map.Frame.reference == ReferenceFrames.Custom)
                {
                    overridable = true;
                }
            }
            Earth3d.LoadLayerFile(overridable);

        }

        void addFeedMenu_Click(object sender, EventArgs e)
        {
            NewDynamicLayer();
        }

        void deleteMenu_Click(object sender, EventArgs e)
        {
            DeleteSelectedLayer();
        }

        private void DeleteSelectedLayer()
        {
            if (layerTree.SelectedNode != null && layerTree.SelectedNode.Tag != null && layerTree.SelectedNode.Tag is Layer)
            {
                Layer node = (Layer)layerTree.SelectedNode.Tag;
                TreeNode parent = layerTree.SelectedNode.Parent;
                node.CleanUp();
                LayerList.Remove(node.ID);
                AllMaps[CurrentMap].Layers.Remove(node);
                layerTree.SelectedNode.Remove();
                layerTree.SelectedNode = parent;
                version++;
            }
        }

        public static bool DeleteLayerByID(Guid ID, bool removeFromParent, bool updateTree)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];
                layer.CleanUp();
                if (removeFromParent)
                {
                    AllMaps[layer.ReferenceFrame].Layers.Remove(layer);
                }
                LayerList.Remove(ID);
                version++;
                if (updateTree)
                {
                    LoadTree();
                }
                return true;
            }
            else
            {
                return false;
            }

        }
        internal static bool UpdateLayer(Guid layerID, object data, bool show, string name, bool noPurge, bool purgeAll, bool hasHeader)
        {

            if (LayerList.ContainsKey(layerID))
            {
                Layer layer = LayerList[layerID];
                layer.UpadteData(data, !noPurge, purgeAll, hasHeader);
                layer.CleanUp();
                layer.Enabled = show;
                if (!string.IsNullOrEmpty(name))
                {
                    layer.Name = name;
                }
                LoadTree();
                return true;
            }
            else
            {
                return false;
            }
        }


        private void SaveFigures_Click(object sender, EventArgs e)
        {


        }
        private void closeBox_MouseEnter(object sender, EventArgs e)
        {
            closeBox.Image = Properties.Resources.CloseHover;
        }

        private void closeBox_MouseLeave(object sender, EventArgs e)
        {
            closeBox.Image = Properties.Resources.CloseRest;

        }

        private void closeBox_MouseDown(object sender, MouseEventArgs e)
        {
            closeBox.Image = Properties.Resources.ClosePush;

        }

        private void closeBox_MouseUp(object sender, MouseEventArgs e)
        {
            closeBox.Image = Properties.Resources.CloseHover;
            this.Close();

        }
        internal DialogResult SaveAndClose()
        {
            this.Close();

            return DialogResult.OK;
        }

        private void LayerManager_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Earth3d.MainWindow.ShowLayersWindows = false;
            }
            master = null;
        }

        internal static IPlace FindClosest(Coordinates target, float distance, bool astronomical, string referenceLayer)
        {
            IPlace closestPlace = null;
            if (referenceLayer == null)
            {
                return closestPlace;
            }
            if (AllMaps.ContainsKey(referenceLayer))
            {
                float currentDistance = distance;

                foreach (Layer layer in AllMaps[referenceLayer].Layers)
                {
                    if (layer.Enabled && astronomical == layer.Astronomical)
                    {
                        closestPlace = layer.FindClosest(target, distance, closestPlace, astronomical);
                    }
                }
                if (closestPlace != null && master != null)
                {
                    master.ShowPlace(closestPlace);
                }

            }
            return closestPlace;

        }

        public static bool HoverCheckScreenSpace(System.Drawing.Point cursor, string referenceFrame)
        {
            if (referenceFrame == null)
            {
                return false;
            }
            if (AllMaps.ContainsKey(referenceFrame))
            {
                foreach (Layer layer in AllMaps[referenceFrame].Layers)
                {
                    if (layer.Enabled)
                    {
                        if (layer.HoverCheckScreenSpace(cursor))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool ClickCheckScreenSpace(System.Drawing.Point cursor, string referenceFrame)
        {
            if (referenceFrame == null)
            {
                return false;
            }
            if (AllMaps.ContainsKey(referenceFrame))
            {
                foreach (Layer layer in AllMaps[referenceFrame].Layers)
                {
                    if (layer.Enabled)
                    {
                        if (layer.ClickCheckScreenSpace(cursor))
                        {
                            return true;
                        }
                    }
                }

            }
            return false;
        }

        internal void ShowPlace(IPlace closestPlace)
        {
            if (closestPlace != null)
            {
                if (closestPlace.Tag != null)
                {
                    Dictionary<String, String> rowData = (Dictionary<String, String>)closestPlace.Tag;
                    ShowRow(rowData);
                }
            }
        }

        private void ShowRow(Dictionary<String, String> rowData)
        {
            NameValues.Items.Clear();

            foreach (KeyValuePair<string, string> kvp in rowData)
            {
                ListViewItem item = new ListViewItem(new string[] { kvp.Key, kvp.Value });
                NameValues.Items.Add(item);
            }
        }


        
        internal static Vector3d GetFrameTarget(RenderContext11 renderContext, string TrackingFrame)
        {
            Vector3d targetPoint = Vector3d.Empty;

            if (!AllMaps.ContainsKey(TrackingFrame))
            {
                return targetPoint;
            }

            List<LayerMap> mapList = new List<LayerMap>();

            LayerMap current = AllMaps[TrackingFrame];

            mapList.Add(current);

            while (current.Frame.Reference == ReferenceFrames.Custom)
            {
                current = current.Parent;
                mapList.Insert(0, current);
            }

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;
            Matrix3d matOldBase = renderContext.WorldBase;
            double oldNominalRadius = renderContext.NominalRadius;

            foreach (LayerMap map in mapList)
            {
                if (map.Frame.Reference != ReferenceFrames.Custom)
                {
                    Planets.SetupPlanetMatrix(renderContext, (int)Enum.Parse(typeof(SolarSystemObjects), map.Frame.Name), Vector3d.Empty, false);
                }
                else
                {
                    map.ComputeFrame(renderContext);
                    if (map.Frame.useRotatingParentFrame())
                    {
                        renderContext.World = map.Frame.WorldMatrix * renderContext.World;
                    }
                    else
                    {
                        renderContext.World = map.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;

                    }
                    if (map.Frame.ReferenceFrameType == ReferenceFrameTypes.Synodic)
                    {
                        renderContext.WorldBaseNonRotating = renderContext.World;
                    }
                    renderContext.NominalRadius = map.Frame.MeanRadius;
                }
            }

            targetPoint = renderContext.World.Transform(targetPoint);


            renderContext.NominalRadius = oldNominalRadius;
            renderContext.World = matOld;
            renderContext.WorldBaseNonRotating = matOldNonRotating;
            renderContext.WorldBase = matOldBase;

            return targetPoint;
        }
        

        internal static Vector3d GetFrameTarget(RenderContext11 renderContext, string TrackingFrame, out Matrix3d matOut)
        {
            Vector3d targetPoint = Vector3d.Empty;
            matOut = Matrix3d.Identity;

            if (!AllMaps.ContainsKey(TrackingFrame))
            {
                return targetPoint;
            }

            List<LayerMap> mapList = new List<LayerMap>();

            LayerMap current = AllMaps[TrackingFrame];

            mapList.Add(current);

            while (current.Frame.Reference == ReferenceFrames.Custom)
            {
                current = current.Parent;
                mapList.Insert(0, current);
            }

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;
            Matrix3d matOldBase = renderContext.WorldBase;
            double oldNominalRadius = renderContext.NominalRadius;

            foreach (LayerMap map in mapList)
            {
                if (map.Frame.Reference != ReferenceFrames.Custom && map.Frame.Reference != ReferenceFrames.Sandbox)
                {
                    Planets.SetupPlanetMatrix(renderContext, (int)Enum.Parse(typeof(SolarSystemObjects), map.Frame.Name), Vector3d.Empty, false);
                }
                else
                {
                    map.ComputeFrame(renderContext);
                    if (map.Frame.useRotatingParentFrame())
                    {
                        renderContext.World = map.Frame.WorldMatrix * renderContext.World;
                    }
                    else
                    {
                        renderContext.World = map.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;

                    }
                    if (map.Frame.ReferenceFrameType == ReferenceFrameTypes.Synodic)
                    {
                        renderContext.WorldBaseNonRotating = renderContext.World;
                    }

                    renderContext.NominalRadius = map.Frame.MeanRadius;
                }
            }

            targetPoint = renderContext.World.Transform(targetPoint);

            Vector3d lookAt = renderContext.World.Transform(new Vector3d(0, 0, 1));

            Vector3d lookUp = renderContext.World.Transform(new Vector3d(0, 1, 0)) - targetPoint;


            lookUp.Normalize();


            matOut = Matrix3d.LookAtLH(new Vector3d(0, 0, 0), lookAt - targetPoint, lookUp);


            renderContext.NominalRadius = oldNominalRadius;
            renderContext.World = matOld;
            renderContext.WorldBaseNonRotating = matOldNonRotating;
            renderContext.WorldBase = matOldBase;

            return targetPoint;
        }

        private static int currentSlideID = -1;

        internal static int CurrentSlideID
        {
            get { return LayerManager.currentSlideID; }
            set
            {
                if (currentSlideID != value)
                {
                    SlideChanged = true;
                }
                else
                {
                    SlideChanged = false;
                }
                LayerManager.currentSlideID = value;
            }
        }

        internal static float SlideTweenPosition = 0;

        internal static bool SlideChanged = false;

        internal static void PrepTourLayers()
        {
            if (TourPlayer.Playing)
            {
                TourPlayer player = Earth3d.MainWindow.UiController as TourPlayer;
                if (player != null)
                {
                    TourDocument tour = player.Tour;

                    if (tour.ProjectorServer)
                    {
                        tour.CurrentTourstopIndex = CurrentSlideID;
                    }

                    if (tour.CurrentTourStop != null)
                    {
                        SlideTweenPosition = player.UpdateTweenPosition(Earth3d.ProjectorServer ? SlideTweenPosition : -1);
                        CurrentSlideID = tour.CurrentTourstopIndex;

                        if (!tour.CurrentTourStop.KeyFramed)
                        {
                            tour.CurrentTourStop.UpdateLayerOpacity();
                            
                            foreach (LayerInfo info in tour.CurrentTourStop.Layers.Values)
                            {
                                if (LayerList.ContainsKey(info.ID))
                                {
                                    LayerList[info.ID].Opacity = info.FrameOpacity;
                                    LayerList[info.ID].SetParams(info.FrameParams);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                CurrentSlideID = -1;
                SlideTweenPosition = 0;
            }

           
        }

        private static bool IsSphereInFrustum(Vector3d sphereCenter, double sphereRadius, PlaneD[] frustum)
        {
            Vector4d center4 = new Vector4d(sphereCenter.X, sphereCenter.Y, sphereCenter.Z, 1.0);

            for (int i = 0; i < 6; i++)
            {
                if (frustum[i].Dot(center4) < -sphereRadius)
                {
                    return false;
                }
            }

            return true;  
        }

        // Get the radius in pixels of a sphere with the specified center and radius
        // The sphere center should be a point in camera space
        private static double ProjectedSizeInPixels(RenderContext11 renderContext, Vector3d center, double radius)
        {
            Matrix3d projection = renderContext.Projection;
            SharpDX.ViewportF viewport = renderContext.ViewPort;

            double distance = center.Length();

            // Calculate pixelsPerUnit which is the number of pixels covered
            // by an object 1 AU at the distance of the planet center from
            // the camera. This calculation works regardless of the projection
            // type.
            double viewportHeight = viewport.Height;
            double p11 = projection.M11;
            double p34 = projection.M34;
            double p44 = projection.M44;
            double w = Math.Abs(p34) * distance + p44;
            double pixelsPerUnit = (p11 / w) * viewportHeight;

            return radius * pixelsPerUnit;
        }

        internal static void Draw(RenderContext11 renderContext, float opacity, bool astronomical, string referenceFrame, bool nested, bool flatSky)
        {
           
            if (!AllMaps.ContainsKey(referenceFrame))
            {
                return;
            }

            LayerMap thisMap = AllMaps[referenceFrame];

            if (!thisMap.Enabled || (thisMap.ChildMaps.Count == 0 && thisMap.Layers.Count == 0 && !(thisMap.Frame.ShowAsPoint || thisMap.Frame.ShowOrbitPath)))
            {
                return;
            }

            //PrepTourLayers();

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;
            double oldNominalRadius = renderContext.NominalRadius;
            if (thisMap.Frame.Reference == ReferenceFrames.Custom | thisMap.Frame.Reference == ReferenceFrames.Sandbox)
            {
                thisMap.ComputeFrame(renderContext);
                if (thisMap.Frame.useRotatingParentFrame())
                {
                    renderContext.World = thisMap.Frame.WorldMatrix * renderContext.World;
                }
                else
                {
                    renderContext.World = thisMap.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;

                }
                if (thisMap.Frame.ReferenceFrameType == ReferenceFrameTypes.Synodic)
                {
                    renderContext.WorldBaseNonRotating = renderContext.World;
                }

                renderContext.NominalRadius = thisMap.Frame.MeanRadius;
            }


            if (thisMap.Frame.ShowAsPoint)
            {
                Planets.DrawPointPlanet(renderContext, new Vector3d(0, 0, 0), (float).2, thisMap.Frame.RepresentativeColor, true, opacity);
            }


            PlaneD[] viewFrustum = new PlaneD[6];
            RenderContext11.ComputeFrustum(renderContext.Projection, viewFrustum);

            for (int pass = 0; pass < 2; pass++)
            {
                foreach (Layer layer in AllMaps[referenceFrame].Layers)
                {
                    if ((pass == 0 && layer is ImageSetLayer) || (pass == 1 && !(layer is ImageSetLayer)))
                    {
                        bool skipLayer = false;
                        if (pass == 0)
                        {
                            // Skip default image set layer so that it's not drawn twice
                            skipLayer = !astronomical && ((ImageSetLayer)layer).OverrideDefaultLayer;
                        }

                        if (layer.Enabled && !skipLayer) // && astronomical == layer.Astronomical)
                        {
                            double layerStart = SpaceTimeController.UtcToJulian(layer.StartTime);
                            double layerEnd = SpaceTimeController.UtcToJulian(layer.EndTime);
                            double fadeIn = SpaceTimeController.UtcToJulian(layer.StartTime) - ((layer.FadeType == FadeType.In || layer.FadeType == FadeType.Both) ? layer.FadeSpan.TotalDays : 0);
                            double fadeOut = SpaceTimeController.UtcToJulian(layer.EndTime) + ((layer.FadeType == FadeType.Out || layer.FadeType == FadeType.Both) ? layer.FadeSpan.TotalDays : 0);

                            if (SpaceTimeController.JNow > fadeIn && SpaceTimeController.JNow < fadeOut)
                            {
                                float fadeOpacity = 1;
                                if (SpaceTimeController.JNow < layerStart)
                                {
                                    fadeOpacity = (float)((SpaceTimeController.JNow - fadeIn) / layer.FadeSpan.TotalDays);
                                }

                                if (SpaceTimeController.JNow > layerEnd)
                                {
                                    fadeOpacity = (float)((fadeOut - SpaceTimeController.JNow) / layer.FadeSpan.TotalDays);
                                }
                                layer.Astronomical = astronomical;
 
                                layer.Draw(renderContext, opacity * fadeOpacity, flatSky);
                            }
                        }
                    }
                }
            }
            if (nested)
            {
                foreach (LayerMap map in AllMaps[referenceFrame].ChildMaps.Values)
                {
                    if (map.Enabled && map.Frame.ShowOrbitPath && Properties.Settings.Default.SolarSystemMinorOrbits.State)
                    {
                        if (map.Frame.ReferenceFrameType == ReferenceFrameTypes.Orbital)
                        {
                            if (map.Frame.Orbit == null)
                            {
                                map.Frame.Orbit = new Orbit(map.Frame.Elements, 360, map.Frame.RepresentativeColor, 1,/* referenceFrame == "Sun" ? (float)(UiTools.KilometersPerAu*1000.0):*/ (float)renderContext.NominalRadius);
                            }

                            double dd = renderContext.NominalRadius;
                            
                            double distss = UiTools.SolarSystemToMeters(Earth3d.MainWindow.SolarSystemCameraDistance);



                            Matrix3d matSaved = renderContext.World;
                            renderContext.World = thisMap.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;

                            // orbitCenter is a position in camera space
                            Vector3d orbitCenter = Vector3d.TransformCoordinate(new Vector3d(0, 0, 0), Matrix3d.Multiply(renderContext.World, renderContext.View));
                            double worldScale = Math.Sqrt(renderContext.World.M11 * renderContext.World.M11 + renderContext.World.M12 * renderContext.World.M12 + renderContext.World.M13 * renderContext.World.M13) * UiTools.KilometersPerAu;



                            double orbitRadius = map.Frame.Orbit.BoundingRadius / UiTools.KilometersPerAu * worldScale;
                            bool cull = !IsSphereInFrustum(orbitCenter, orbitRadius, viewFrustum);



                            float fade = (float)Math.Min(1, Math.Max(Math.Log(UiTools.SolarSystemToMeters(Earth3d.MainWindow.SolarSystemCameraDistance), 10) - 7.3, 0));
                            if (Earth3d.MainWindow.TrackingFrame == map.Frame.Name)
                            {
                                double ratio = map.Frame.MeanRadius / distss;

                                double val = Math.Log(ratio, 10) + 2.7;
                                fade = (float)Math.Min(1, Math.Max(-val, 0));

                            }


                            fade *= Properties.Settings.Default.SolarSystemMinorOrbits.Opacity;
                            if ( fade > 0)
                            {


                                map.Frame.Orbit.Draw3D(renderContext, fade, new Vector3d(0, 0, 0));
                                renderContext.World = matSaved;
                            }
                        }
                        else if (map.Frame.ReferenceFrameType == ReferenceFrameTypes.Trajectory)
                        {
                            if (map.Frame.trajectoryLines == null)
                            {
                                map.Frame.trajectoryLines = new LineList();
                                map.Frame.trajectoryLines.ShowFarSide = true;
                                map.Frame.trajectoryLines.UseNonRotatingFrame = true;

                                int count = map.Frame.Trajectory.Count - 1;
                                for (int i = 0; i < count; i++)
                                {
                                    Vector3d pos1 = map.Frame.Trajectory[i].Position;
                                    Vector3d pos2 = map.Frame.Trajectory[i + 1].Position;
                                    pos1.Multiply(1 / renderContext.NominalRadius);
                                    pos2.Multiply(1 / renderContext.NominalRadius);
                                    map.Frame.trajectoryLines.AddLine(pos1, pos2, map.Frame.RepresentativeColor, new Dates());
                                }
                            }
                            Matrix3d matSaved = renderContext.World;
                            renderContext.World = thisMap.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;
                            double distss = UiTools.SolarSystemToMeters(Earth3d.MainWindow.SolarSystemCameraDistance);

                            float fade = (float)Math.Min(1, Math.Max(Math.Log(distss, 10) - 7.3, 0));
                            if (Earth3d.MainWindow.TrackingFrame == map.Frame.Name)
                            {
                                double ratio = map.Frame.MeanRadius / distss;

                                double val = Math.Log(ratio, 10) + 2.7;
                                fade = (float)Math.Min(1, Math.Max(-val, 0));
                            }

                            map.Frame.trajectoryLines.DrawLines(renderContext, Properties.Settings.Default.SolarSystemMinorOrbits.Opacity * fade);
                            renderContext.World = matSaved;
                        }
                    }

                    if ((map.Frame.Reference == ReferenceFrames.Custom || map.Frame.Reference == ReferenceFrames.Identity))
                    {
                        Draw(renderContext, opacity, astronomical, map.Name, nested, flatSky);
                    }
                }
            }
            renderContext.NominalRadius = oldNominalRadius;
            renderContext.World = matOld;
            renderContext.WorldBaseNonRotating = matOldNonRotating;
        }

        //todo remove the stuff from draw that is redundant once predraw has run
        internal static void PreDraw(RenderContext11 renderContext, float opacity, bool astronomical, string referenceFrame, bool nested)
        {


            if (!AllMaps.ContainsKey(referenceFrame))
            {
                return;
            }



            LayerMap thisMap = AllMaps[referenceFrame];

            if (thisMap.ChildMaps.Count == 0 && thisMap.Layers.Count == 0)
            {
                return;
            }

            //PrepTourLayers();

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;
            double oldNominalRadius = renderContext.NominalRadius;
            if (thisMap.Frame.Reference == ReferenceFrames.Custom)
            {
                thisMap.ComputeFrame(renderContext);
                if (thisMap.Frame.useRotatingParentFrame())
                {
                    renderContext.World = thisMap.Frame.WorldMatrix * renderContext.World;
                }
                else
                {
                    renderContext.World = thisMap.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;
                }
                if (thisMap.Frame.ReferenceFrameType == ReferenceFrameTypes.Synodic)
                {
                    renderContext.WorldBaseNonRotating = renderContext.World;
                }

                renderContext.NominalRadius = thisMap.Frame.MeanRadius;
            }



            for (int pass = 0; pass < 2; pass++)
            {
                foreach (Layer layer in AllMaps[referenceFrame].Layers)
                {
                    if ((pass == 0 && layer is ImageSetLayer) || (pass == 1 && !(layer is ImageSetLayer)))
                    {
                        if (layer.Enabled) // && astronomical == layer.Astronomical)
                        {
                            double layerStart = SpaceTimeController.UtcToJulian(layer.StartTime);
                            double layerEnd = SpaceTimeController.UtcToJulian(layer.EndTime);
                            double fadeIn = SpaceTimeController.UtcToJulian(layer.StartTime) - ((layer.FadeType == FadeType.In || layer.FadeType == FadeType.Both) ? layer.FadeSpan.TotalDays : 0);
                            double fadeOut = SpaceTimeController.UtcToJulian(layer.EndTime) + ((layer.FadeType == FadeType.Out || layer.FadeType == FadeType.Both) ? layer.FadeSpan.TotalDays : 0);

                            if (SpaceTimeController.JNow > fadeIn && SpaceTimeController.JNow < fadeOut)
                            {
                                float fadeOpacity = 1;
                                if (SpaceTimeController.JNow < layerStart)
                                {
                                    fadeOpacity = (float)((SpaceTimeController.JNow - fadeIn) / layer.FadeSpan.TotalDays);
                                }

                                if (SpaceTimeController.JNow > layerEnd)
                                {
                                    fadeOpacity = (float)((fadeOut - SpaceTimeController.JNow) / layer.FadeSpan.TotalDays);
                                }
                                if (thisMap.Frame.Reference == ReferenceFrames.Sky)
                                {
                                    layer.Astronomical = true;
                                }
                                layer.PreDraw(renderContext, opacity * fadeOpacity);
                            }
                        }
                    }

                }
            }
            if (nested)
            {
                foreach (LayerMap map in AllMaps[referenceFrame].ChildMaps.Values)
                {
                    if ((map.Frame.Reference == ReferenceFrames.Custom || map.Frame.Reference == ReferenceFrames.Identity))
                    {
                        PreDraw(renderContext, opacity, astronomical, map.Name, nested);
                    }
                }
            }
            renderContext.NominalRadius = oldNominalRadius;
            renderContext.World = matOld;
            renderContext.WorldBaseNonRotating = matOldNonRotating;
        }

        private void DeletePoint_Click(object sender, EventArgs e)
        {
            DeleteSelectedLayer();
        }

        private void pasteLayer_Click(object sender, EventArgs e)
        {


            IDataObject dataObject = Clipboard.GetDataObject();
            if (dataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                string[] formats = dataObject.GetFormats();
                object data = dataObject.GetData(DataFormats.UnicodeText);
                if (data is String)
                {
                    string layerName = "Pasted Layer";

                    SpreadSheetLayer layer = new SpreadSheetLayer((string)data, true);
                    layer.Enabled = true;
                    layer.Name = layerName;

                    if (DataWizard.ShowWizard(layer) == DialogResult.OK)
                    {
                        LayerList.Add(layer.ID, layer);
                        layer.ReferenceFrame = CurrentMap;
                        AllMaps[CurrentMap].Layers.Add(layer);
                        AllMaps[CurrentMap].Open = true;
                        version++;
                        LoadTree();

                    }
                }
            }

        }

        public static Guid CreateLayerFromString(string data, string name, string referenceFrame, bool showUI, int color, DateTime beginDate, DateTime endDate, FadeType fadeType, double fadeRange)
        {
            SpreadSheetLayer layer = new SpreadSheetLayer((string)data, true);
            layer.Enabled = true;
            layer.Name = name;
            layer.TimeSeries = true;
            layer.ReferenceFrame = referenceFrame;
            layer.Color = Color.FromArgb(color);
            layer.BeginRange = beginDate;
            layer.EndRange = endDate;

            if (showUI)
            {
                if (DataWizard.ShowWizard(layer) != DialogResult.OK)
                {
                    return Guid.Empty;
                }
            }
            LayerList.Add(layer.ID, layer);
            AllMaps[referenceFrame].Layers.Add(layer);
            AllMaps[referenceFrame].Open = true;
            version++;
            LoadTree();


            return layer.ID;
        }

        private void timeScrubber_Scroll(object sender, EventArgs e)
        {
            if (layerTree.SelectedNode != null && layerTree.SelectedNode.Tag as ITimeSeriesDescription != null)
            {
                ITimeSeriesDescription iTimeSeries = layerTree.SelectedNode.Tag as ITimeSeriesDescription;
                TimeSpan ts = iTimeSeries.SeriesEndTime - iTimeSeries.SeriesStartTime;

                long ticksPerUnit = ts.Ticks / 1000;

                SpaceTimeController.Now = iTimeSeries.SeriesStartTime + new TimeSpan((long)timeScrubber.Value * ticksPerUnit);
            }
        }
        bool autoLoop = false;

        private void autoLoop_CheckedChanged(object sender, EventArgs e)
        {
            autoLoop = autoLoopCheckbox.Checked;
        }

        private void loopTimer_Tick(object sender, EventArgs e)
        {
            if (needTreeUpdate)
            {
                if (!ProcessingUpdate)
                {
                    ProcessingUpdate = true;
                    SyncLayerState();
                    needTreeUpdate = false;
                    ProcessingUpdate = false;
                }
            }

        }

        public static void UpdateLayerTime()
        {
            if (LayerManager.master != null && master.autoLoop)
            {
                master.UpdateLayerTimeLocal();
            }
        }

        private void UpdateLayerTimeLocal()
        {
            if (layerTree.SelectedNode != null && layerTree.SelectedNode.Tag as ITimeSeriesDescription != null)
            {
                ITimeSeriesDescription iTimeSeries = layerTree.SelectedNode.Tag as ITimeSeriesDescription;
                if (iTimeSeries.IsTimeSeries)
                {
                    if (SpaceTimeController.Now > iTimeSeries.SeriesEndTime)
                    {
                        SpaceTimeController.Now = iTimeSeries.SeriesStartTime;
                    }

                    TimeSpan ts = iTimeSeries.SeriesEndTime - iTimeSeries.SeriesStartTime;

                    long ticksPerUnit = ts.Ticks / 1001;

                    if (SpaceTimeController.Now < iTimeSeries.SeriesStartTime)
                    {
                        timeScrubber.Value = 0;
                    }
                    else if (SpaceTimeController.Now > iTimeSeries.SeriesEndTime)
                    {
                        timeScrubber.Value = 1000;
                    }
                    else
                    {
                        ts = SpaceTimeController.Now - iTimeSeries.SeriesStartTime;
                        try
                        {
                            if (ticksPerUnit == 0)
                            {
                                timeScrubber.Value = 0;
                            }
                            else
                            {
                                timeScrubber.Value = Math.Min(timeScrubber.Maximum, (int)(ts.Ticks / ticksPerUnit));
                            }
                        }
                        catch
                        { }
                    }
                }
            }
        }

        private void timeSeries_CheckedChanged(object sender, EventArgs e)
        {
            if (layerTree.SelectedNode != null && layerTree.SelectedNode.Tag as ITimeSeriesDescription != null)
            {
                ITimeSeriesDescription iTimeSeries = layerTree.SelectedNode.Tag as ITimeSeriesDescription;

                if (iTimeSeries.IsTimeSeries != timeSeries.Checked)
                {
                    iTimeSeries.IsTimeSeries = timeSeries.Checked;
                }
                timeSeries.Checked = iTimeSeries.IsTimeSeries;
            }
        }

        private void layerTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (layerTree.SelectedNode != null)
            {
                currentSelection = layerTree.SelectedNode.Tag;
            }
            DeleteLayer.Enabled = (layerTree.SelectedNode != null);
            TreeNode node = e.Node;
            if (layerTree.SelectedNode.Level > 0)
            {
                int level = layerTree.SelectedNode.Level;


                while (!(node.Tag is LayerMap) && level > 0)
                {
                    node = node.Parent;
                    level--;
                }
                LayerMap map = node.Tag as LayerMap;
                if (map != null)
                {
                    CurrentMap = map.Name;
                }
            }

            node = e.Node;

            if (node != null && node.Tag is LayerUITreeNode)
            {
                LayerUITreeNode layerNode = node.Tag as LayerUITreeNode;
                layerNode.FireNodeSelected();

            }

            
            if (layerTree.SelectedNode != null && layerTree.SelectedNode.Tag as ITimeSeriesDescription != null)
            {
                ITimeSeriesDescription iTimeSeries = layerTree.SelectedNode.Tag as ITimeSeriesDescription;

                timeSeries.Checked = iTimeSeries.IsTimeSeries;
                if (iTimeSeries.SeriesStartTime.ToString("HH:mm:ss") == "00:00:00")
                {
                    startDate.Text = iTimeSeries.SeriesStartTime.ToString("yyyy/MM/dd");
                }
                else
                {
                    startDate.Text = iTimeSeries.SeriesStartTime.ToString("yyyy/MM/dd HH:mm:ss");
                }

                if (iTimeSeries.SeriesEndTime.ToString("HH:mm:ss") == "00:00:00")
                {
                    endDate.Text = iTimeSeries.SeriesEndTime.ToString("yyyy/MM/dd");
                }
                else
                {
                    endDate.Text = iTimeSeries.SeriesEndTime.ToString("yyyy/MM/dd HH:mm:ss");
                }
                return;
            }
            else if (layerTree.SelectedNode != null && layerTree.SelectedNode.Tag is LayerMap)
            {
                LayerMap map = layerTree.SelectedNode.Tag as LayerMap;
                if (map != null)
                {
                    CurrentMap = map.Name;
                }
            }

            timeSeries.Checked = false;
            startDate.Text = "";
            endDate.Text = "";

        }

        private void layerTree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node != null && node.Tag is LayerUITreeNode)
            {
                if (e.Action != TreeViewAction.Unknown)
                {

                    LayerUITreeNode layerNode = node.Tag as LayerUITreeNode;
                    if (layerNode.Opened != node.IsExpanded)
                    {
                        layerNode.Opened = node.IsExpanded;
                    }
                }
            }
            else if (node != null && node.Tag is LayerMap)
            {
                LayerMap map = node.Tag as LayerMap;
                if (map != null)
                {
                    map.Open = false;
                }
            }
        }

        private void layerTree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (node != null && node.Tag is LayerUITreeNode)
            {
                if (e.Action != TreeViewAction.Unknown)
                {
                    LayerUITreeNode layerNode = node.Tag as LayerUITreeNode;
                    if (layerNode.Opened != node.IsExpanded)
                    {
                        layerNode.Opened = node.IsExpanded;
                    }
                }
            }
            else if (node != null && node.Tag is LayerMap)
            {
                LayerMap map = node.Tag as LayerMap;
                if (map != null)
                {
                    map.Open = true;
                }
            }
        }





        internal static Dictionary<Guid, LayerInfo> GetVisibleLayerList(Dictionary<Guid, LayerInfo> previous)
        {
            Dictionary<Guid, LayerInfo> list = new Dictionary<Guid, LayerInfo>();

            foreach (Layer layer in LayerList.Values)
            {
                if (layer.Enabled)
                {
                    LayerInfo info = new LayerInfo();
                    info.StartOpacity = info.EndOpacity = layer.Opacity;
                    info.ID = layer.ID;
                    info.StartParams = layer.GetParams();


                    if (previous.ContainsKey(info.ID))
                    {
                        info.EndOpacity = previous[info.ID].EndOpacity;
                        info.EndParams = previous[info.ID].EndParams;
                    }
                    else
                    {
                        info.EndParams = layer.GetParams();
                    }
                    list.Add(layer.ID, info);
                }
            }
            return list;
        }

        internal static void SetVisibleLayerList(Dictionary<Guid, LayerInfo> list)
        {
            foreach (Layer layer in LayerList.Values)
            {
                layer.Enabled = list.ContainsKey(layer.ID);
                try
                {
                    if (layer.Enabled)
                    {
                        layer.Opacity = list[layer.ID].FrameOpacity;
                        layer.SetParams(list[layer.ID].FrameParams);
                    }
                }
                catch
                {
                }
            }
            SyncLayerState();
        }
        public static bool InsideLayerManagerRect = false;

        private void fadeTimer_Tick(object sender, EventArgs e)
        {
            Rectangle rect = this.RectangleToScreen(this.ClientRectangle);
            rect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);

            InsideLayerManagerRect = rect.Contains(Cursor.Position);

            bool inside = MenuTabs.MouseInTabs || TabForm.InsideTabRect || rect.Contains(Cursor.Position) || !((TourPlayer.Playing && !Settings.DomeView) || Earth3d.FullScreen || Properties.Settings.Default.AutoHideTabs);

            if (inside != fader.TargetState)
            {
                fader.TargetState = inside;
                fadeTimer.Enabled = true;
                fadeTimer.Interval = 10;
            }

            SetOpacity();

            if ((!fader.TargetState && fader.Opacity == 0.0) || (fader.TargetState && fader.Opacity == 1.0))
            {
                if (true)
                {
                    if (Properties.Settings.Default.TranparentWindows)
                    {

                        this.Visible = true;
                    }
                    else
                    {
                        this.Visible = fader.TargetState;
                    }
                    if (Earth3d.FullScreen)
                    {
                        Earth3d.MainWindow.menuTabs.IsVisible = fader.TargetState;
                    }
                }
                fadeTimer.Enabled = true;
                fadeTimer.Interval = 250;
            }
        }
        public void SetOpacity()
        {
            if (Properties.Settings.Default.TranparentWindows)
            {
                try
                {
                    Opacity = .0 + .8 * fader.Opacity;
                }
                catch
                {
                    Opacity = 1.0;
                }
            }
            else
            {
                Opacity = 1.0;
            }
        }
        BlendState fader = new BlendState(false, 1000.0);

        private void LayerManager_MouseEnter(object sender, EventArgs e)
        {
            fader.TargetState = true;
            fadeTimer.Enabled = true;
            fadeTimer.Interval = 10;
        }

        public static void ConnectAllChildren()
        {
            foreach (LayerMap map in AllMaps.Values)
            {
                if (String.IsNullOrEmpty(map.Frame.Parent) && !LayerMaps.ContainsKey(map.Frame.Name))
                {
                    LayerMaps.Add(map.Name, map);
                }
                else if (!String.IsNullOrEmpty(map.Frame.Parent) && AllMaps.ContainsKey(map.Frame.Parent))
                {
                    if (!AllMaps[map.Frame.Parent].ChildMaps.ContainsKey(map.Frame.Name))
                    {
                        AllMaps[map.Frame.Parent].AddChild(map);
                    }
                }
            }
        }

        private void ResetLayer_Click(object sender, EventArgs e)
        {
            if (UiTools.ShowMessageBox(Language.GetLocalizedText(681, "This will delete all current reference frames and all layers and reset layers to startup defaults. Are you sure you want to do this?"), Language.GetLocalizedText(682, "Reset layers Manager"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LayerManager.InitLayers();
            }
        }

        internal static bool CreateLayerGroup(string name, string referenceFrame)
        {
            LayerMap parent = AllMaps[referenceFrame];
            if (parent == null)
            {
                return false;
            }
            try
            {
                ReferenceFrame frame = new ReferenceFrame();
                frame.Name = name;
                frame.Reference = ReferenceFrames.Identity;
                LayerMap newMap = new LayerMap(frame.Name, ReferenceFrames.Identity);
                newMap.Frame = frame;
                newMap.Frame.SystemGenerated = false;
                parent.AddChild(newMap);
                newMap.Frame.Parent = parent.Name;
                AllMaps.Add(frame.Name, newMap);
                LoadTree();
                version++;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Layer LoadLayer(string filename, string parentFrame, bool interactive, bool referenceFrameRightClick)
        {
            version++;
            if (filename.ToLower().EndsWith(".wwtl"))
            {
                return LoadLayerFile(filename, parentFrame, referenceFrameRightClick);
            }
            else if (filename.ToLower().EndsWith(".png") || filename.ToLower().EndsWith(".jpg"))
            {
                return LoadGroundOverlayFile(filename, parentFrame, interactive);
            }
            else if (filename.ToLower().EndsWith(".shp"))
            {
                return LoadShapeFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".3ds"))
            {
                return Load3dModelFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".obj"))
            {
                return Load3dModelFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".wtml"))
            {
                return LoadWtmlFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".kml") || filename.ToLower().EndsWith(".kmz"))
            {
                return LoadKmlFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".tle"))
            {
                return LoadOrbitsFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".layers"))
            {
                LayerContainer layers = LayerContainer.FromFile(filename, false, parentFrame, referenceFrameRightClick);
                layers.Dispose();
                GC.SuppressFinalize(layers);
                LoadTree();
                return null;
            }
            else
            {
                return LoadDataTable(filename, parentFrame, interactive);
            }
        }

        public static Layer LoadLayerFile(string filename, string parentFrame, bool referenceFrameRightClick)
        {
            LayerContainer layers = LayerContainer.FromFile(filename, false, parentFrame, referenceFrameRightClick);
            //   layers.ClearTempFiles();
            Earth3d.MainWindow.ShowLayersWindows = true;
            return layers.LastLoadedLayer;
        }


        internal static Layer LoadOrbitsFile(string path, string currentMap)
        {
            OrbitLayer layer = new OrbitLayer();
            layer.LoadData(path);
            layer.Enabled = true;
            layer.Name = path.Substring(path.LastIndexOf('\\') + 1);
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();
            return layer;
        }

        private static Layer LoadGroundOverlayFile(string path, string parentFrame, bool interactive)
        {
            GroundOverlayLayer layer = new GroundOverlayLayer();

            layer.CreateFromFile(path);
            layer.Overlay.north = Earth3d.MainWindow.viewCamera.Lat + 5;
            layer.Overlay.south = Earth3d.MainWindow.viewCamera.Lat - 5;
            layer.Overlay.west = Earth3d.MainWindow.viewCamera.Lng - 5;
            layer.Overlay.east = Earth3d.MainWindow.viewCamera.Lng + 5;

            layer.Enabled = true;
            layer.Name = path.Substring(path.LastIndexOf('\\') + 1);
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();

            if (interactive)
            {
                GroundOverlayProperties props = new GroundOverlayProperties();
                props.Overlay = layer.Overlay;
                props.OverlayLayer = layer;
                props.Owner = Earth3d.MainWindow;
                props.Show();
            }
            return layer;
        }

        private static void AddGreatCircleLayer()
        {
            GreatCirlceRouteLayer layer = new GreatCirlceRouteLayer();


            layer.LatStart = Earth3d.MainWindow.viewCamera.Lat;
            layer.LatEnd = Earth3d.MainWindow.viewCamera.Lat - 5;
            layer.LngStart = Earth3d.MainWindow.viewCamera.Lng;
            layer.LngEnd = Earth3d.MainWindow.viewCamera.Lng + 5;
            layer.Width = 4;
            layer.Enabled = true;
            layer.Name = Language.GetLocalizedText(1144, "Great Circle Route");
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();

            GreatCircleProperties props = new GreatCircleProperties();
            props.Layer = layer;
            props.Owner = Earth3d.MainWindow;
            props.Show();
        }


        private static Layer LoadKmlFile(string path, string parentFrame)
        {
            KmlRoot newRoot = new KmlRoot(path, (KmlRoot)null);
            KmlLayer layer = new KmlLayer();
            layer.root = newRoot;
            KmlCollection.UpdateRootLinks(layer.root, Earth3d.MainWindow.KmlViewInfo);
            layer.UpdateRetainedVisuals();
            layer.Enabled = true;
            layer.Name = path.Substring(path.LastIndexOf('\\') + 1);
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();
            if (newRoot.children != null && newRoot.children.Count > 0)
            {
                if (newRoot.children[0].LookAt != null)
                {
                    KmlLayerUI.GotoLookAt(newRoot.children[0]);
                }
            }
            return layer;
        }

        private static Layer LoadWtmlFile(string filename, string parentFrame)
        {
            IImageSet imageset = null;
            Folder newFolder = Folder.LoadFromFile(filename, false);

            if (newFolder.Children.Length > 0)
            {

                if (newFolder.Children[0] is Place)
                {
                    Place place = (Place)newFolder.Children[0];
                    if (place.BackgroundImageSet != null && place.BackgroundImageSet.ImageSet != null)
                    {
                        imageset = place.BackgroundImageSet.ImageSet;
                    }
                    else if (place.StudyImageset != null && place.StudyImageset != null)
                    {
                        imageset = place.StudyImageset;
                    }

                }
                else if (newFolder.Children[0] is IImageSet)
                {
                    imageset = (IImageSet)newFolder.Children[0];
                }
            }
            else
            {
                return null;
            }

            Layer layer = AddImagesetLayer(imageset);
            version++;
            return layer;
        }

        public static Guid LoadLayer(string name, string referenceFrame, string filename, int color, DateTime beginDate, DateTime endDate, FadeType fadeType, double fadeRange)
        {
            try
            {
                Layer layer = LoadLayer(filename, referenceFrame, false, false);
                layer.StartTime = beginDate;
                layer.EndTime = endDate;
                layer.Color = Color.FromArgb(color);
                if (!string.IsNullOrEmpty(name))
                {
                    layer.Name = name;
                    version++;
                    LoadTree();
                }
                return layer.ID;
            }
            catch
            {
                return Guid.Empty;
            }
        }



        public static Layer NewDynamicLayer()
        {
            SpreadSheetLayer layer = new SpreadSheetLayer();
            layer.Enabled = true;
            layer.DynamicData = true;
            layer.Name = Language.GetLocalizedText(1143, "New Dynamic layer");

            if (DataWizard.ShowWizard(layer) == DialogResult.OK)
            {
                LayerList.Add(layer.ID, layer);
                layer.ReferenceFrame = currentMap;
                AllMaps[currentMap].Layers.Add(layer);
                AllMaps[currentMap].Open = true;
                LoadTree();
            }

            return layer;
        }


   

        public static string GetLayerPropByID(Guid ID, string propName, out Layer layer)
        {
            if (LayerList.ContainsKey(ID))
            {
                layer = LayerList[ID];
                return layer.GetProp(propName);
            }
            else
            {
                layer = null;
                return "";
            }

        }

        public static string GetLayerPropsByID(Guid ID)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];
                return layer.GetProps();
            }
            else
            {
                return "";
            }
        }



        public static bool SetLayerPropByID(Guid ID, string propName, string propValue)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];
                bool retVal = layer.SetProp(propName, propValue);
                layer.CleanUp();
                LoadTree();
                return retVal;
            }
            else
            {
                return false;
            }

        }



        internal static bool SetLayerPropsByID(Guid ID, string xml)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];

                bool retVal = layer.SetProps(xml);
                layer.CleanUp();
                LoadTree();
                return retVal;
            }
            else
            {
                return false;
            }
        }

        internal static bool ActivateLayer(Guid ID)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];

                if (master != null)
                {

                    if (master.InvokeRequired)
                    {
                        MethodInvoker updatePlace = delegate
                        {
                            master.ActivateLayerLocal(layer);
                        };
                        try
                        {
                            master.Invoke(updatePlace);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        master.ActivateLayerLocal(layer);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ActivateLayerLocal(Layer layer)
        {
            TreeNode selectNode = FindLayerNode(layerTree.Nodes, layer);
            if (selectNode != null)
            {
                layerTree.SelectedNode = selectNode;
                return true;
            }

            return false;
        }

        private TreeNode FindLayerNode(TreeNodeCollection nodes, Layer layer)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == layer)
                {
                    return node;
                }
                TreeNode found = FindLayerNode(node.Nodes, layer);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }



        internal static void SetAutoloop(bool shouldLoop)
        {
            if (master != null)
            {

                if (master.InvokeRequired)
                {
                    MethodInvoker updatePlace = delegate
                    {
                        master.autoLoop = master.autoLoopCheckbox.Checked = shouldLoop;
                    };
                    try
                    {
                        master.Invoke(updatePlace);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    master.autoLoop = master.autoLoopCheckbox.Checked = shouldLoop;
                }
            }
        }

        internal static string GetLayerList(bool layersOnly)
        {
            MemoryStream ms = new MemoryStream();
            using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, System.Text.Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                xmlWriter.WriteStartElement("LayerApi");
                xmlWriter.WriteElementString("Status", "Success");
                xmlWriter.WriteStartElement("LayerList");
                xmlWriter.WriteAttributeString("Version", LayerManager.Version.ToString());

                PrintLayers(xmlWriter, layersOnly, LayerMaps);

                xmlWriter.WriteEndElement();
                xmlWriter.WriteFullEndElement();
                xmlWriter.Close();

            }
            byte[] data = ms.GetBuffer();
            return Encoding.UTF8.GetString(data);
        }

        private static void PrintLayers(XmlTextWriter xmlWriter, bool layersOnly, Dictionary<string, LayerMap> LayerMaps)
        {
            foreach (LayerMap map in LayerMaps.Values)
            {
                List<Layer> layers = map.Layers;
                if (!layersOnly)
                {
                    if (map.Frame.Reference == ReferenceFrames.Identity)
                    {
                        xmlWriter.WriteStartElement("LayerGroup");
                    }
                    else
                    {
                        xmlWriter.WriteStartElement("ReferenceFrame");
                    }
                    xmlWriter.WriteAttributeString("Name", map.Name);
                    xmlWriter.WriteAttributeString("Enabled", map.Enabled.ToString());
                }
                foreach (Layer layer in layers)
                {
                    xmlWriter.WriteStartElement("Layer");
                    xmlWriter.WriteAttributeString("Name", layer.Name);
                    xmlWriter.WriteAttributeString("ID", layer.ID.ToString());
                    xmlWriter.WriteAttributeString("Type", layer.GetType().ToString().Replace("TerraViewer.", ""));
                    xmlWriter.WriteAttributeString("Enabled", layer.Enabled.ToString());
                    xmlWriter.WriteAttributeString("Version", layer.Version.ToString());
                    xmlWriter.WriteEndElement();
                }

                PrintLayers(xmlWriter, layersOnly, map.ChildMaps);

                if (!layersOnly)
                {
                    xmlWriter.WriteEndElement();
                }
            }
        }

        private void NameValues_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        internal static string GetLayerDataID(Guid ID)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];

                SpreadSheetLayer sheet = layer as SpreadSheetLayer;
                if (sheet != null)
                {
                    return sheet.Table.ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return "";
            }
        }

        private void Minus_Click(object sender, EventArgs e)
        {
            if (breadcrumbs.Count > 1)
            {
                breadcrumbs.Pop();
                LoadTree();
            }
        }

        private void Plus_Click(object sender, EventArgs e)
        {

            TreeNode node = layerTree.SelectedNode;
            if (node == null)
            {
                return;
            }

            while (node.Parent != null && (node.Parent != breadcrumbs.Peek()))
            {
                if (node == null)
                {
                    break;
                }
                node = node.Parent;
            }
            if (node != null && node.Tag != breadcrumbs.Peek())
            {
                breadcrumbs.Push(node.Tag);
            }
            LoadTree();
        }

        private void HighlightLabel_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).ForeColor = Color.Yellow;
        }

        private void HighlightLabel_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).ForeColor = Color.White;
        }
        bool dragging = false;
        System.Drawing.Point downPoint;
        private void LayerManager_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X > this.Width - 5)
            {
                dragging = true;
                downPoint = e.Location;
            }
        }

        private void LayerManager_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void LayerManager_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                int change = downPoint.X - e.X;
                this.Width = Math.Min(600, Math.Max(150, Width - change));

                downPoint = e.Location;
            }

            if (e.X > this.Width - 5)
            {
                this.Cursor = Cursors.SizeWE;
            }
            else
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void LayerManager_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void layerTree_DoubleClick(object sender, EventArgs e)
        {
            TreeNode node = layerTree.SelectedNode as TreeNode;
            if (node != null && node.Tag is LayerUITreeNode)
            {
                LayerUITreeNode layerNode = node.Tag as LayerUITreeNode;
                layerNode.FireNodeActivated();
            }
        }

        private void NameValues_ItemActivate(object sender, EventArgs e)
        {
            if (NameValues.SelectedItems.Count > 0)
            {
                string url = NameValues.SelectedItems[0].SubItems[1].Text;

                if (url.ToLower().StartsWith("http:") || url.ToLower().StartsWith("https:"))
                {
                    WebWindow.OpenUrl(url, true);
                }
            }
        }






        private void layerTree_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            Brush back = new SolidBrush(BackColor);

            e.Graphics.FillRectangle(back, e.Bounds);
            e.Graphics.DrawString(e.Node.Text, UiTools.StandardRegular, UiTools.StadardTextBrush, new System.Drawing.Point(e.Bounds.Location.X, e.Bounds.Location.Y + 1), UiTools.StringFormatTopLeft);
            //e.DrawDefault = true;
        }




        void IUIServicesCallbacks.ShowRowData(Dictionary<string, string> rowData)
        {
            ShowRow(rowData);
        }


        public void UpdateNode(Layer layer, LayerUITreeNode node)
        {
            if (node == null)
            {
                if (layer != null)
                {
                    TreeNode[] results = layerTree.Nodes.Find(layer.Name, true);

                    foreach(TreeNode child in results)
                    {
                        if (child.Tag == layer)
                        {
                            child.Nodes.Clear();
                            this.LoadLayerChildren(layer, child);
                        }
                    }
                }
            }
        }

        internal static bool DeleteFrameByName(string referenceFrame)
        {
            if (AllMaps.ContainsKey(referenceFrame))
            {
                LayerMap target = AllMaps[referenceFrame];
                PurgeLayerMapDeep(target, true);
                version++;
                LoadTree();
                return true;
            }
            return false;
        }
    }

    public class LayerMap
    {
        public LayerMap(string name, ReferenceFrames reference)
        {
            Name = name;
            Frame.Reference = reference;
            double radius = 6371000;

            switch (reference)
            {
                case ReferenceFrames.Sky:
                    break;
                case ReferenceFrames.Ecliptic:
                    break;
                case ReferenceFrames.Galactic:
                    break;
                case ReferenceFrames.Sun:
                    radius = 696000000;
                    break;
                case ReferenceFrames.Mercury:
                    radius = 2439700;
                    break;
                case ReferenceFrames.Venus:
                    radius = 6051800;
                    break;
                case ReferenceFrames.Earth:
                    radius = 6371000;
                    break;
                case ReferenceFrames.Mars:
                    radius = 3390000;
                    break;
                case ReferenceFrames.Jupiter:
                    radius = 69911000;
                    break;
                case ReferenceFrames.Saturn:
                    radius = 58232000;
                    break;
                case ReferenceFrames.Uranus:
                    radius = 25362000;
                    break;
                case ReferenceFrames.Neptune:
                    radius = 24622000;
                    break;
                case ReferenceFrames.Pluto:
                    radius = 1161000;
                    break;
                case ReferenceFrames.Moon:
                    radius = 1737100;
                    break;
                case ReferenceFrames.Io:
                    radius = 1821500;
                    break;
                case ReferenceFrames.Europa:
                    radius = 1561000;
                    break;
                case ReferenceFrames.Ganymede:
                    radius = 2631200;
                    break;
                case ReferenceFrames.Callisto:
                    radius = 2410300;
                    break;
                case ReferenceFrames.Custom:
                    Frame.SystemGenerated = false;
                    break;
                case ReferenceFrames.Identity:
                    break;
                case ReferenceFrames.Sandbox:
                    radius = 1;
                    break;
                default:
                    break;
            }
            Frame.MeanRadius = radius;

        }
        public Dictionary<string, LayerMap> ChildMaps = new Dictionary<string, LayerMap>();
        public void AddChild(LayerMap child)
        {
            child.Parent = this;
            ChildMaps.Add(child.Name, child);
        }

        public LayerMap Parent = null;
        public List<Layer> Layers = new List<Layer>();
        public bool Open = false;
        public bool Enabled = true;
        public bool LoadedFromTour = false;
        public string Name
        {
            get { return Frame.Name; }
            set { Frame.Name = value; }
        }


        public ReferenceFrame Frame = new ReferenceFrame();
        public void ComputeFrame(RenderContext11 renderContext)
        {
            if (Frame.Reference == ReferenceFrames.Custom | Frame.Reference == ReferenceFrames.Sandbox)
            {
                Frame.ComputeFrame(renderContext);
            }
        }

        public override string ToString()
        {
            return Name;
        }


    }


    public enum ReferenceFrames
    {
        Sky,
        Ecliptic,
        Galactic,
        Sun,
        Mercury,
        Venus,
        Earth,
        Mars,
        Jupiter,
        Saturn,
        Uranus,
        Neptune,
        Pluto,
        Moon,
        Io,
        Europa,
        Ganymede,
        Callisto,
        Custom,
        Identity,
        Sandbox
    };
}