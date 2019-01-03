
using ShapefileTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace TerraViewer
{
    public partial class LayerManager : Form, IUIServicesCallbacks
    {

        public LayerManager()
        {
            InitializeComponent();
            SetUiStrings();
            master = this;
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

            if (lutn != null)
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

                    if (layerTree.SelectedNode.Tag is SpreadSheetLayer || layerTree.SelectedNode.Tag is Object3dLayer || layerTree.SelectedNode.Tag is GroundOverlayLayer || layerTree.SelectedNode.Tag is GreatCirlceRouteLayer || layerTree.SelectedNode.Tag is OrbitLayer || layerTree.SelectedNode.Tag is ImageSetLayer)
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
                    addToTimeline.Click += new EventHandler(addToTimeline_Click);
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

            IPlace place = Catalogs.FindCatalogObjectExact(target.Frame.Reference.ToString());
            if (place != null)
            {
                RenderEngine.Engine.GotoTarget(place, false, false, true);
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
                            GetMpc(input.ResultText, target);
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
                            GetMpcAsTLE(input.ResultText, target);
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
            isl.OverrideDefaultLayer = !isl.OverrideDefaultLayer;
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

            RenderEngine.Engine.SolarSystemTrack = SolarSystemObjects.Custom;
            RenderEngine.Engine.TrackingFrame = target.Name;
            RenderEngine.Engine.viewCamera.Zoom = RenderEngine.Engine.targetViewCamera.Zoom = .000000001;


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

        private void ImportTLEFile(string filename)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;
            ImportTLEFile(filename, target);
        }

        private void MakeLayerGroup(string name)
        {
            LayerMap target = (LayerMap)layerTree.SelectedNode.Tag;
            MakeLayerGroup(name, target);
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



        private void timeScrubber_Scroll(object sender, EventArgs e)
        {
            if (layerTree.SelectedNode != null)
            {
                if (layerTree.SelectedNode.Tag as ITimeSeriesDescription != null)
                {
                    ITimeSeriesDescription iTimeSeries = layerTree.SelectedNode.Tag as ITimeSeriesDescription;
                    TimeSpan ts = iTimeSeries.SeriesEndTime - iTimeSeries.SeriesStartTime;

                    long ticksPerUnit = ts.Ticks / 1000;

                    SpaceTimeController.Now = iTimeSeries.SeriesStartTime + new TimeSpan((long)timeScrubber.Value * ticksPerUnit);
                    timeLabel.Text = Language.GetLocalizedText(667, "Time Scrubber");
                }
                else
                {
                    ImageSetLayer layer = layerTree.SelectedNode.Tag as ImageSetLayer;
                    if (layer != null && layer.ImageSet.WcsImage is FitsImage)
                    {
                        Histogram.UpdateImage(layer, timeScrubber.Value);
                        timeLabel.Text = layer.FitsImage.GetZDescription();
                    }
                }
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



        private void UpdateLayerTimeLocal()
        {
            if (layerTree.SelectedNode != null)
            {
                if (layerTree.SelectedNode.Tag as ITimeSeriesDescription != null)
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

                        timeScrubber.Maximum = 1000;

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
                else 
                {

                    ImageSetLayer layer = layerTree.SelectedNode.Tag as ImageSetLayer;
                    if (layer != null && layer.ImageSet.WcsImage is FitsImage)
                    {
                        timeScrubber.Maximum = layer.FitsImage.Depth-1;
                        timeScrubber.Minimum = 0;
                        timeScrubber.Value = layer.FitsImage.lastBitmapZ;

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


            if (layerTree.SelectedNode != null)
            {
                if (layerTree.SelectedNode.Tag as ITimeSeriesDescription != null)
                {
                    timeScrubber.Maximum = 1000;
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
                else if ( layerTree.SelectedNode.Tag is LayerMap)
                {
                    LayerMap map = layerTree.SelectedNode.Tag as LayerMap;
                    if (map != null)
                    {
                        CurrentMap = map.Name;
                    }
                }
                else
                {
                    ImageSetLayer layer = layerTree.SelectedNode.Tag as ImageSetLayer;
                    if (layer != null && layer.ImageSet.WcsImage is FitsImage)
                    {
                        Histogram.UpdateImage(layer, timeScrubber.Value);
                        timeSeries.Checked = false;
                        startDate.Text = "0";
                        timeScrubber.Maximum = layer.FitsImage.Depth-1;
                        timeScrubber.Value = layer.FitsImage.lastBitmapZ;
                        endDate.Text = timeScrubber.Maximum.ToString();
                        return;
                    }                 
                }
            }

            timeSeries.Checked = false;
            startDate.Text = "";
            endDate.Text = "";
            timeLabel.Text = Language.GetLocalizedText(667, "Time Scrubber");
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



        private void ResetLayer_Click(object sender, EventArgs e)
        {
            if (UiTools.ShowMessageBox(Language.GetLocalizedText(681, "This will delete all current reference frames and all layers and reset layers to startup defaults. Are you sure you want to do this?"), Language.GetLocalizedText(682, "Reset layers Manager"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                LayerManager.InitLayers();
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

        private void NameValues_SelectedIndexChanged(object sender, EventArgs e)
        {

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

                    foreach (TreeNode child in results)
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
    }
}