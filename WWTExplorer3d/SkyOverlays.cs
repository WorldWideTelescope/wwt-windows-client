using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace TerraViewer
{
    public class SkyOverlays : Layer
    {
        public List<SkyOverlay> Children = new List<SkyOverlay>();
        public SkyOverlays()
        {

        }

        public SkyOverlays(SkyOverlaysType overlayType)
        {
            switch (overlayType)
            {
                case SkyOverlaysType.Sky:
                    InitForSky();
                    break;
                case SkyOverlaysType.Sky2d:
                    InitFor2dSky();
                    break;            
                case SkyOverlaysType.Dome:
                    InitForDome();
                    break;
                case SkyOverlaysType.SolarSystem:
                    InitForSolarSystem();
                    break;
                case SkyOverlaysType.Earth:
                    InitForEarth();
                    break;
                case SkyOverlaysType.Planet:
                    break;
                default:
                    break;
            }
        }

        void InitForSky()
        {
            Name = Language.GetLocalizedText(504, "Overlays");
            Opened = true;
            ReferenceFrame = "Sky";

            var grids = new StockSkyOverlay(Language.GetLocalizedText(1080, "Grids"), StockSkyOverlayTypes.SkyGrids);
            var temp = new StockSkyOverlay(Language.GetLocalizedText(496, "Equatorial Grid"), StockSkyOverlayTypes.EquatorialGrid);
            grids.Children.Add(temp);
            temp.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1081, "Axis Labels"), StockSkyOverlayTypes.EquatorialGridText));
            temp = new StockSkyOverlay(Language.GetLocalizedText(1082, "Galactic Grid"), StockSkyOverlayTypes.GalacticGrid);
            grids.Children.Add(temp);
            temp.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1081, "Axis Labels"), StockSkyOverlayTypes.GalacticGridText));
            temp = new StockSkyOverlay(Language.GetLocalizedText(1083, "AltAz Grid"), StockSkyOverlayTypes.AltAzGrid);
            grids.Children.Add(temp);
            temp.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1081, "Axis Labels"), StockSkyOverlayTypes.AltAzGridText));
            temp = new StockSkyOverlay(Language.GetLocalizedText(1084, "Ecliptic Grid"), StockSkyOverlayTypes.EclipticGrid);
            grids.Children.Add(temp);
            temp.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1081, "Axis Labels"), StockSkyOverlayTypes.EclipticGridText));
            temp = new StockSkyOverlay(Language.GetLocalizedText(1085, "Ecliptic Overview"), StockSkyOverlayTypes.EclipticOverview);
            grids.Children.Add(temp);
            temp.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1086, "Month Labels"), StockSkyOverlayTypes.EclipticOverviewText));
            grids.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1087, "Precession Chart"), StockSkyOverlayTypes.PrecessionChart));

            var constellations = new StockSkyOverlay(Language.GetLocalizedText(1088, "Constellations"), StockSkyOverlayTypes.Constellations);
            constellations.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1092, "Constellation Pictures"), StockSkyOverlayTypes.ConstellationPictures));
            constellations.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1089, "Constellation Figures"), StockSkyOverlayTypes.ConstellationFigures));
            temp = new StockSkyOverlay(Language.GetLocalizedText(1090, "Constellation Boundaries"), StockSkyOverlayTypes.ConstellationBoundaries);
            constellations.Children.Add(temp);
            temp.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(495, "Focused Only"), StockSkyOverlayTypes.ConstellationFocusedOnly));
            constellations.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1091, "Constellation Names"), StockSkyOverlayTypes.ConstellationNames));
            Children.Add(constellations);
            Children.Add(grids);
        }

        void InitForSolarSystem()
        {
            Name = Language.GetLocalizedText(559, "3d Solar System");
            Opened = true;
            ReferenceFrame = "Sky";
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1375, "Cosmic Microwave Background (Planck)"), StockSkyOverlayTypes.SolarSystemCMB));
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1093, "Cosmos (SDSS Galaxies)"), StockSkyOverlayTypes.SolarSystemCosmos));
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1094, "Milky Way (Dr. R. Hurt)"), StockSkyOverlayTypes.SolarSystemMilkyWay));
            Children.Add(new StockSkyOverlay((Language.GetLocalizedText(1388, "Volumetric Milky Way")), StockSkyOverlayTypes.VolumetricMilkyWay));
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1095, "Stars (Hipparcos, ESA)"), StockSkyOverlayTypes.SolarSystemStars));
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1096, "Planets (NASA, ETAL)"), StockSkyOverlayTypes.SolarSystemPlanets));


            var orbits = new StockSkyOverlay(Language.GetLocalizedText(1097, "Planetary Orbits"), StockSkyOverlayTypes.SolarSystemOrbits);
            orbits.Children.Add(new StockSkyOverlay("Orbit Filters", StockSkyOverlayTypes.OrbitFilters));

            Children.Add(orbits);

            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1098, "Moon & Satellite Orbits"), StockSkyOverlayTypes.SolarSystemMinorOrbits));

            var mpc = new StockSkyOverlay(Language.GetLocalizedText(1099, "Asteriods (IAU MPC)"), StockSkyOverlayTypes.SolarSystemAsteroids);
            mpc.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1389, "Zone 1 ( < 2.5 au)"), StockSkyOverlayTypes.MPCZone1));
            mpc.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1390, "Zone 2 (2.5-2.83 au)"), StockSkyOverlayTypes.MPCZone2));
            mpc.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1391, "Zone 3 (2.83-2.96 au)"), StockSkyOverlayTypes.MPCZone3));
            mpc.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1392, "Zone 4 (2.96-3.3 au)"), StockSkyOverlayTypes.MPCZone4));
            mpc.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1393, "Zone 5 (3.3-5.0 au)"), StockSkyOverlayTypes.MPCZone5));
            mpc.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1394, "Zone 6 (5.0 - 10 au)"), StockSkyOverlayTypes.MPCZone6));
            mpc.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1395, "Zone 7 ( > 10 au)"), StockSkyOverlayTypes.MPCZone7));

            Children.Add(mpc);
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1100, "Lighting and Shadows"), StockSkyOverlayTypes.SolarSystemLighting));
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(592, "Multi-Res Solar System Bodies"), StockSkyOverlayTypes.MultiResSolarSystemBodies));
        }

        void InitFor2dSky()
        {
            Name = Language.GetLocalizedText(1101, "2D Sky");
            Opened = true;
            ReferenceFrame = "Sky";
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1102, "Show Solar System"), StockSkyOverlayTypes.ShowSolarSystem));
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1103, "Field of View Indicators"), StockSkyOverlayTypes.FiledOfView));  
        }

        void InitForEarth()
        {
            Name = Language.GetLocalizedText(504, "Overlays");
            Opened = true;
            ReferenceFrame = "Earth";
            var clouds = new StockSkyOverlay(Language.GetLocalizedText(1104, "Cloud Layer"), StockSkyOverlayTypes.ShowEarthCloudLayer);
            clouds.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1105, "Use 8k Cloud Texture"), StockSkyOverlayTypes.Clouds8k));
            Children.Add(clouds);
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1106, "Elevation Model"), StockSkyOverlayTypes.ShowElevationModel));
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1107, "Sky & Atmosphere"), StockSkyOverlayTypes.ShowAtmosphere));
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1108, "Cutaway View"), StockSkyOverlayTypes.EarthCutAway));
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1387, "Show 3D Cities"), StockSkyOverlayTypes.Show3dCities));

        }

        void InitForDome()
        {
            Name = Language.GetLocalizedText(504, "Overlays");
            Opened = true;
            ReferenceFrame = Language.GetLocalizedText(1109, "Dome");
            var fade = new StockSkyOverlay(Language.GetLocalizedText(1110, "Fade to black"), StockSkyOverlayTypes.FadeToBlack);
            fade.Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1111, "Fade Dome Only"),StockSkyOverlayTypes.FadeRemoteOnly));
            Children.Add(fade);
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1112, "Fade to logo"), StockSkyOverlayTypes.FadeToLogo));
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1113, "Fade to gradient"), StockSkyOverlayTypes.FadeToGradient));
            Children.Add(new StockSkyOverlay(Language.GetLocalizedText(1114, "Screen Broadcast"), StockSkyOverlayTypes.ScreenBroadcast));
        }

        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {
            var matOld = renderContext.World;

            if (astronomical && !flat)
            {
                var obliquity = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow);
                var mat = Matrix3d.RotationX((float)(-obliquity / 360.0 * (Math.PI * 2)));
                mat.Multiply(Matrix3d.RotationY(Math.PI));
                mat.Multiply(matOld);
                renderContext.World = Matrix3d.Multiply(Matrix3d.Scaling(4000000000, 4000000000, 4000000000), mat);
            }
            foreach (var overlay in Children)
            {
                if (overlay.Enabled || overlay.BlendState.State)
                {
                    overlay.Draw(renderContext, opacity);
                }
            }

            renderContext.World = matOld;
            return true;
        }
        SkyOverlaysUI primaryUI;
        public override LayerUI GetPrimaryUI()
        {
            if (primaryUI == null)
            {
                primaryUI = new SkyOverlaysUI(this);
            }

            return primaryUI;
        }

        public override bool Enabled
        {
            get
            {
                return true;
            }
            set
            {
                
            }
        }
    }

    public enum SkyOverlaysType { Sky, Dome, SolarSystem, Earth, Planet, Sky2d };
    public class SkyOverlaysUI : LayerUI
    {
        public SkyOverlays Layer;
        public SkyOverlaysUI(SkyOverlays layer)
        {
            Layer = layer;
        }
        public override bool HasTreeViewNodes
        {
            get
            {
                return true;
            }
        }

        public override List<LayerUITreeNode> GetTreeNodes()
        {
            var nodes = new List<LayerUITreeNode>();
            foreach (var overlay in Layer.Children)
            {

                AddNode(nodes, overlay);
            }
            return nodes;
        }

        private void AddNode(List<LayerUITreeNode> nodes, SkyOverlay overlay)
        {
            var node = new LayerUITreeNode();
            node.Name = overlay.Name;
            node.Tag = overlay;
            node.Checked = overlay.Enabled;
            node.NodeSelected += node_NodeSelected;
            node.NodeChecked += node_NodeChecked;
            nodes.Add(node);
            foreach (var child in overlay.Children)
            {
                AddNode(node.Nodes, child);
            }
            var so = overlay as StockSkyOverlay;
            if (so != null)
            {
                if (so.StockType == StockSkyOverlayTypes.ConstellationPictures)
                {
                    AddFilters(node.Nodes, so, Properties.Settings.Default.ConstellationArtFilter);
                }

                if (so.StockType == StockSkyOverlayTypes.ConstellationFigures)
                {
                    AddFilters(node.Nodes, so, Properties.Settings.Default.ConstellationFiguresFilter);
                }

                if (so.StockType == StockSkyOverlayTypes.ConstellationBoundaries)
                {
                    AddFilters(node.Nodes, so, Properties.Settings.Default.ConstellationBoundariesFilter);
                }
            }
        }

        private void AddFilters(List<LayerUITreeNode> nodes, StockSkyOverlay so, ConstellationFilter filter)
        {

                var filterNode = new LayerUITreeNode();
                filterNode.Name = Language.GetLocalizedText(1115, "Filter");
                filterNode.Tag = filter;
                filterNode.Checked = true;
                filterNode.NodeSelected += filterNode_NodeSelected;
                filterNode.NodeChecked += filterNode_NodeChecked;
                nodes.Add(filterNode);
                AddConstellationParts(filterNode, ConstellationFilter.AllConstellation);

        }

   
        void filterNode_NodeChecked(LayerUITreeNode node, bool newState)
        {
            var filter = node.Tag as ConstellationFilter;
            if (filter != null)
            {
                filter.SetAll(newState);
            }
        }

        void filterNode_NodeSelected(LayerUITreeNode node)
        {
            
        }

        private void AddConstellationParts(LayerUITreeNode filterNode, ConstellationFilter constellationFilter)
        {
            foreach (var kv in ConstellationFilter.BitIDs)
            {
                if (constellationFilter.IsSet(kv.Key))
                {
                    var constellationNodes = filterNode.Add(Constellations.FullName(kv.Key));
                    constellationNodes.Tag = filterNode.Tag;
                    constellationNodes.Checked = true;
                    constellationNodes.NodeSelected += constellationNodes_NodeSelected;
                    constellationNodes.NodeChecked += constellationNodes_NodeChecked;
                    constellationNodes.IsChecked += constellationNodes_IsChecked;
                }
            }
        }

        bool constellationNodes_IsChecked(LayerUITreeNode node)
        {
            var filter = node.Tag as ConstellationFilter;
            if (filter != null)
            {
                return filter.IsSet(Constellations.Abbreviation(node.Name));
            }

            return false;
        }

        void constellationNodes_NodeChecked(LayerUITreeNode node, bool newState)
        {
            var filter = node.Tag as ConstellationFilter;
            if (filter != null)
            {
                filter.Set(Constellations.Abbreviation(node.Name), newState);
            }
        }

        void constellationNodes_NodeSelected(LayerUITreeNode node)
        {
            
        }

       

        void node_NodeChecked(LayerUITreeNode node, bool newState)
        {
            var child = node.Tag as SkyOverlay;

            if (child != null)
            {
                child.Enabled = newState;
            }

        }

        void node_NodeSelected(LayerUITreeNode node)
        {

        }

        public override List<LayerUIMenuItem> GetNodeContextMenu(LayerUITreeNode node)
        {
            var items = new List<LayerUIMenuItem>();

            var colorMenu = new LayerUIMenuItem();
            colorMenu.Name = Language.GetLocalizedText(1116, "Color");
            colorMenu.MenuItemSelected += ColorMenu_MenuItemSelected;
            colorMenu.Tag = node.Tag;
            var so = node.Tag as StockSkyOverlay;
            if (so != null && so.HasColor)
            {
                items.Add(colorMenu);
            }
         
            if (so != null && so.StockType == StockSkyOverlayTypes.FiledOfView)
            {
                var setupMenu = new LayerUIMenuItem();
                setupMenu.Name = Language.GetLocalizedText(379, "Setup");
                setupMenu.MenuItemSelected += setupMenu_MenuItemSelected;
                setupMenu.Tag = node.Tag;
                items.Add(setupMenu);
            }

            if (so != null && so.StockType == StockSkyOverlayTypes.ScreenBroadcast)
            {
                var setupMenuB = new LayerUIMenuItem();
                setupMenuB.Name = Language.GetLocalizedText(379, "Setup");
                setupMenuB.MenuItemSelected += setupMenuB_MenuItemSelected;
                setupMenuB.Tag = node.Tag;
                items.Add(setupMenuB);
            }

            if (so != null && so.StockType == StockSkyOverlayTypes.ConstellationPictures)
            {
                var Import = new LayerUIMenuItem();
                Import.Name = Language.GetLocalizedText(1117, "Import...");
                Import.MenuItemSelected += Import_MenuItemSelected;
                Import.Tag = node.Tag;
                items.Add(Import);
            }

            var filter = node.Tag as ConstellationFilter;
            if (filter != null && node.Parent== null)
            {
                var applyMenu = new LayerUIMenuItem();
                applyMenu.Name = Language.GetLocalizedText(195, "Apply");
                applyMenu.Tag = filter;
                AddFilterListApply(applyMenu);
                items.Add(applyMenu);


                var includeMenu = new LayerUIMenuItem();
                includeMenu.Name = Language.GetLocalizedText(1118, "Include");
                includeMenu.Tag = filter;
                AddFilterListInclude(includeMenu);
                items.Add(includeMenu);  
                
                var excludeMenu = new LayerUIMenuItem();
                excludeMenu.Name = Language.GetLocalizedText(1119, "Exclude");
                excludeMenu.Tag = filter;
                AddFilterListExclude(excludeMenu);
                items.Add(excludeMenu);

                var newFilterMenu = new LayerUIMenuItem();
                newFilterMenu.Name = Language.GetLocalizedText(1120, "New Filter...");
                newFilterMenu.MenuItemSelected += newFilterMenu_MenuItemSelected; ;
                newFilterMenu.Tag = node.Tag;
                items.Add(newFilterMenu);

                var deleteFilter = new LayerUIMenuItem();
                deleteFilter.Name = Language.GetLocalizedText(1269, "Delete Custom Filter");
                deleteFilter.Tag = filter;
                AddFilterListDelete(deleteFilter);
                items.Add(deleteFilter);

            }

            if (so != null) //todo && we are editing a keyframe slide )
            {
                if (Earth3d.MainWindow.TourEdit != null && Earth3d.MainWindow.TourEdit.Tour.EditMode && Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop != null)
                {

                    if (Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.IsTargetAnimated(so.StockType.ToString()))
                    {
                        var setupMenuL = new LayerUIMenuItem();
                        setupMenuL.Name = Language.GetLocalizedText(1280, "Add Keyframe");
                        setupMenuL.MenuItemSelected += setupMenuL_MenuItemSelected;
                        setupMenuL.Tag = node.Tag;
                        items.Add(setupMenuL);
                    }
                    else
                    {
                        var setupMenuK = new LayerUIMenuItem();
                        setupMenuK.Name = Language.GetLocalizedText(1290, "Add to Timeline");
                        setupMenuK.MenuItemSelected += setupMenuK_MenuItemSelected;
                        setupMenuK.Tag = node.Tag;
                        items.Add(setupMenuK);
                    }
                }
            }

            return items;
        }

        void setupMenuL_MenuItemSelected(LayerUIMenuItem item)
        {
            var sso = item.Tag as StockSkyOverlay;

            if (Earth3d.MainWindow.TourEdit != null && sso != null)
            {
                if (Earth3d.MainWindow.TourEdit.Tour.EditMode && Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop != null)
                { 
                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1290, "Add to Timeline"), Earth3d.MainWindow.TourEdit.Tour));

                    Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.KeyFramed = true;

                    var aniTarget = Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.FindTarget(sso.StockType.ToString());
                    aniTarget.SetKeyFrame(Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.TweenPosition, Key.KeyType.Linear);
                    TimeLine.RefreshUi();
                }
            }
        }

        void setupMenuK_MenuItemSelected(LayerUIMenuItem item)
        {
            var sso = item.Tag as StockSkyOverlay;

            if (Earth3d.MainWindow.TourEdit != null && sso != null)
            {
                if (Earth3d.MainWindow.TourEdit.Tour.EditMode && Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop != null)
                {

                    var target = Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.GetSettingAnimator(sso.StockType.ToString());
                    var type = AnimationTarget.AnimationTargetTypes.StockSkyOverlay;

                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1290, "Add to Timeline"), Earth3d.MainWindow.TourEdit.Tour));

                    Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.KeyFramed = true;

                    var aniTarget = new AnimationTarget(Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop);
                    aniTarget.Target = target;
                    aniTarget.TargetType = type;
                    aniTarget.ParameterNames.AddRange(target.GetParamNames());
                    aniTarget.CurrentParameters = target.GetParams();
                    aniTarget.SetKeyFrame(0, Key.KeyType.Linear);
                    //todo add end frames?

                    Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.AnimationTargets.Add(aniTarget);
                    TimeLine.RefreshUi();
                }
            }
        }

        void newFilterMenu_MenuItemSelected(LayerUIMenuItem item)
        {
            var input = new SimpleInput(Language.GetLocalizedText(1121, "Filter name"), Language.GetLocalizedText(238, "Name"), "", 32);
            var retry = false;
            do
            {
                if (input.ShowDialog() == DialogResult.OK)
                {
                    foreach(var name in ConstellationFilter.Families.Keys)
                    {
                        if (name.ToLower().Trim() == input.ResultText.ToLower().Trim())
                        {
                            MessageBox.Show(Language.GetLocalizedText(516, "Name already exists, type a different name."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                            retry = true;
                            break;
                        }
                       
                    }
                }
                else
                {
                    return;
                }
            } while (retry);

            var filterIn = item.Tag as ConstellationFilter;

            var newFilter = filterIn.Clone();

            ConstellationFilter.Families.Add(input.ResultText.Trim().Replace(";",""), newFilter);
            ConstellationFilter.SaveCustomFilters();
        }

        private void AddFilterListApply(LayerUIMenuItem applyMenu)
        {
            foreach (var id in ConstellationFilter.Families.Keys)
            {
                var filterMenuItem = new LayerUIMenuItem();

                filterMenuItem.Name = id;
                filterMenuItem.Tag = applyMenu.Tag;
                filterMenuItem.MenuItemSelected += filterMenuItem_ApplyMenuItemSelected;
                applyMenu.SubMenus.Add(filterMenuItem);
            }
        }

        void filterMenuItem_ApplyMenuItemSelected(LayerUIMenuItem item)
        {
            var filter = item.Tag as ConstellationFilter;
            filter.Clone(ConstellationFilter.Families[item.Name]);
        }

        private void AddFilterListInclude(LayerUIMenuItem includeMenu)
        {
            foreach (var id in ConstellationFilter.Families.Keys)
            {
                var filterMenuItem = new LayerUIMenuItem();

                filterMenuItem.Name = id;
                filterMenuItem.Tag = includeMenu.Tag;
                filterMenuItem.MenuItemSelected += filterMenuItem_IncludeMenuItemSelected;
                includeMenu.SubMenus.Add(filterMenuItem);
            }
        }

        void filterMenuItem_IncludeMenuItemSelected(LayerUIMenuItem item)
        {
            var filter = item.Tag as ConstellationFilter;
            filter.Combine(ConstellationFilter.Families[item.Name]);
        }


        private void AddFilterListExclude(LayerUIMenuItem excludeMenu)
        {
            foreach (var id in ConstellationFilter.Families.Keys)
            {
                var filterMenuItem = new LayerUIMenuItem();

                filterMenuItem.Name = id;
                filterMenuItem.Tag = excludeMenu.Tag;
                filterMenuItem.MenuItemSelected += filterMenuItem_ExcludeMenuItemSelected;
                excludeMenu.SubMenus.Add(filterMenuItem);
            }
        }

        void filterMenuItem_ExcludeMenuItemSelected(LayerUIMenuItem item)
        {
            var filter = item.Tag as ConstellationFilter;
            filter.Remove(ConstellationFilter.Families[item.Name]);
        }

        private void AddFilterListDelete(LayerUIMenuItem excludeMenu)
        {
            var anyCustomFilters = false;

            foreach (var id in ConstellationFilter.Families.Keys)
            {
                if (!ConstellationFilter.Families[id].Internal)
                {
                    var DeleteFilterMenuItem = new LayerUIMenuItem();

                    DeleteFilterMenuItem.Name = id;
                    DeleteFilterMenuItem.Tag = excludeMenu.Tag;
                    DeleteFilterMenuItem.MenuItemSelected += DeleteFilterMenuItem_MenuItemSelected;
                    excludeMenu.SubMenus.Add(DeleteFilterMenuItem);
                    anyCustomFilters = true;
                }
            }

            if (!anyCustomFilters)
            {
                var DeleteFilterMenuItem = new LayerUIMenuItem();

                DeleteFilterMenuItem.Name = Language.GetLocalizedText(1268, "No Custom Filters to Delete");
                DeleteFilterMenuItem.Tag = excludeMenu.Tag;
                excludeMenu.SubMenus.Add(DeleteFilterMenuItem);
                anyCustomFilters = true;
            }
        }

        void DeleteFilterMenuItem_MenuItemSelected(LayerUIMenuItem item)
        {
            if (ConstellationFilter.Families.ContainsKey(item.Name))
            {
                ConstellationFilter.Families.Remove(item.Name);
            }
        }

        void Import_MenuItemSelected(LayerUIMenuItem item)
        {
            Constellations.ImportArtFile();
        }

        void setupMenuB_MenuItemSelected(LayerUIMenuItem item)
        {
            var sb = new ScreenBroadcast();
            sb.Show();
        }

        void setupMenu_MenuItemSelected(LayerUIMenuItem item)
        {
            View.ShowFovSetup();
        }

        void ColorMenu_MenuItemSelected(LayerUIMenuItem item)
        {
            var overlay = item.Tag as SkyOverlay;


            var picker = new PopupColorPicker();

            picker.Location = Cursor.Position;

            picker.Color = overlay.Color;

            if (picker.ShowDialog() == DialogResult.OK)
            {
                overlay.Color = picker.Color;
            }
        }
    }


    public class SkyOverlay
    {
        public List<SkyOverlay> Children = new List<SkyOverlay>();
        public SkyOverlay()
        {
        }

        public SkyOverlay(string name)
        {
            Name = name;
        }

        private string name;

        public virtual string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual void Draw(RenderContext11 renderContext, float opacity)
        {
            foreach (var overlay in Children)
            {
                if (overlay.Enabled || overlay.BlendState.State)
                {
                    overlay.Draw(renderContext, opacity * overlay.BlendState.Opacity * BlendState.Opacity);
                }
            }
        }

        public virtual bool HasColor
        {
            get
            {
                return false;
            }
        }

        private Color color = Color.White;
        public virtual Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
        protected bool enabled = true;

        private float opacity;

        public float Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }

        public BlendState BlendState = new BlendState(true, 2000);

        public virtual bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    BlendState.TargetState = value;
                }
            }
        }
    }
     public class StockSkyOverlay : SkyOverlay
    {
       // public static Dictionary<StockSkyOverlayTypes, StockSkyOverlay> StockOverlays = new Dictionary<StockSkyOverlayTypes, StockSkyOverlay>();
        public StockSkyOverlayTypes StockType = StockSkyOverlayTypes.ConstellationBoundaries;
        public StockSkyOverlay(string name, StockSkyOverlayTypes stockType)
        {
            StockType = stockType;
            Name = name;
            enabled = true;
            BlendState.State = true;

            CopyBlendStates();
            //if (!StockOverlays.ContainsKey(stockType))
            //{
            //    StockOverlays.Add(stockType, this);
            //}
        }

        private void CopyBlendStates()
        {
            switch (StockType)
            {
                case StockSkyOverlayTypes.SkyGrids:
                    BlendState = Properties.Settings.Default.ShowSkyGrids;
                    break;
                case StockSkyOverlayTypes.Constellations:
                    BlendState = Properties.Settings.Default.Constellations;
                    break;
                case StockSkyOverlayTypes.EquatorialGrid:
                    BlendState = Properties.Settings.Default.ShowGrid;
                    break;
                case StockSkyOverlayTypes.EquatorialGridText:
                    BlendState = Properties.Settings.Default.ShowEquatorialGridText;
                    break;
                case StockSkyOverlayTypes.GalacticGrid:
                    BlendState = Properties.Settings.Default.ShowGalacticGrid;
                    break;
                case StockSkyOverlayTypes.GalacticGridText:
                    BlendState = Properties.Settings.Default.ShowGalacticGridText;
                    break;
                case StockSkyOverlayTypes.AltAzGrid:
                    BlendState = Properties.Settings.Default.ShowAltAzGrid;
                    break;
                case StockSkyOverlayTypes.AltAzGridText:
                    BlendState = Properties.Settings.Default.ShowAltAzGridText;
                    break;
                case StockSkyOverlayTypes.EclipticGrid:
                    BlendState = Properties.Settings.Default.ShowEclipticGrid;
                    break;
                case StockSkyOverlayTypes.EclipticGridText:
                    BlendState = Properties.Settings.Default.ShowEclipticGridText;
                    break;
                case StockSkyOverlayTypes.EclipticOverview:
                    BlendState = Properties.Settings.Default.ShowEcliptic;
                    break;
                case StockSkyOverlayTypes.EclipticOverviewText:
                    BlendState = Properties.Settings.Default.ShowEclipticOverviewText;
                    break;
                case StockSkyOverlayTypes.PrecessionChart:
                    BlendState = Properties.Settings.Default.ShowPrecessionChart;
                    break;
                case StockSkyOverlayTypes.ConstellationFigures:
                    BlendState = Properties.Settings.Default.ShowConstellationFigures;
                    break;
                case StockSkyOverlayTypes.ConstellationBoundaries:
                    BlendState = Properties.Settings.Default.ShowConstellationBoundries;
                    break;
                case StockSkyOverlayTypes.ConstellationFocusedOnly:
                    BlendState = Properties.Settings.Default.ShowConstellationSelection;
                    break;
                case StockSkyOverlayTypes.ConstellationNames:
                    BlendState = Properties.Settings.Default.ShowConstellationLabels;
                    break;
                case StockSkyOverlayTypes.ConstellationPictures:
                    BlendState = Properties.Settings.Default.ShowConstellationPictures;
                    break;
                case StockSkyOverlayTypes.SolarSystemStars:
                    BlendState = Properties.Settings.Default.SolarSystemStars;
                    break;
                case StockSkyOverlayTypes.SolarSystemMilkyWay:
                    BlendState = Properties.Settings.Default.SolarSystemMilkyWay;
                    break;      
                case StockSkyOverlayTypes.SolarSystemCosmos:
                    BlendState = Properties.Settings.Default.SolarSystemCosmos;
                    break;
                case StockSkyOverlayTypes.SolarSystemCMB:
                    BlendState = Properties.Settings.Default.SolarSystemCMB;
                    break;      
                case StockSkyOverlayTypes.SolarSystemOrbits:
                    BlendState = Properties.Settings.Default.SolarSystemOrbits;
                    break;
                case StockSkyOverlayTypes.ShowSolarSystem:
                    BlendState = Properties.Settings.Default.ShowSolarSystem;
                    break;
                case StockSkyOverlayTypes.FiledOfView:
                    BlendState = Properties.Settings.Default.ShowFieldOfView;
                    break;
                case StockSkyOverlayTypes.SolarSystemPlanets:
                    BlendState = Properties.Settings.Default.SolarSystemPlanets;
                    break;
                case StockSkyOverlayTypes.SolarSystemAsteroids:
                    BlendState = Properties.Settings.Default.SolarSystemMinorPlanets;
                    break;
                case StockSkyOverlayTypes.SolarSystemMinorOrbits:
                    BlendState = Properties.Settings.Default.SolarSystemMinorOrbits;
                    break;
                case StockSkyOverlayTypes.ShowEarthCloudLayer:
                    BlendState = Properties.Settings.Default.ShowClouds;
                    break;
                case StockSkyOverlayTypes.EarthCutAway:
                    BlendState = Properties.Settings.Default.EarthCutawayView;
                    break;
                case StockSkyOverlayTypes.ShowAtmosphere:
                    BlendState = Properties.Settings.Default.ShowEarthSky;
                    break;
                default:
                    break;
            }
        }


        public override void Draw(RenderContext11 renderContext, float opacity)
        {
            switch (StockType)
            {
                case StockSkyOverlayTypes.AuroraBorialis:
                    //Grids.DrawAuroraBorialis(renderContext, BlendState.Opacity * opacity);
                    break;
                case StockSkyOverlayTypes.EquatorialGrid:
                    Grids.DrawEquitorialGrid(renderContext, BlendState.Opacity * opacity, Color);
                    break;
                case StockSkyOverlayTypes.EquatorialGridText:
                    Grids.DrawEquitorialGridText(renderContext, BlendState.Opacity * opacity, Color);
                    break;
                case StockSkyOverlayTypes.GalacticGrid:
                    Grids.DrawGalacticGrid(renderContext, BlendState.Opacity * opacity, Color);
                    break;
                case StockSkyOverlayTypes.GalacticGridText:
                    Grids.DrawGalacticGridText(renderContext, BlendState.Opacity * opacity, Color);
                    break;
                case StockSkyOverlayTypes.AltAzGrid:
                    Grids.DrawAltAzGrid(renderContext, BlendState.Opacity * opacity, Color);
                    break;
                case StockSkyOverlayTypes.AltAzGridText:
                    Grids.DrawAltAzGridText(renderContext, BlendState.Opacity * opacity, Color);
                    break;
                case StockSkyOverlayTypes.EclipticGrid:
                    Grids.DrawEclipticGrid(renderContext, BlendState.Opacity * opacity, Color);
                    break;
                case StockSkyOverlayTypes.EclipticOverview:
                    Grids.DrawEcliptic(renderContext, BlendState.Opacity * opacity, Color);
                    break;
                case StockSkyOverlayTypes.EclipticOverviewText:
                    Grids.DrawEclipticText(renderContext, BlendState.Opacity * opacity, Color);
                    break;
                case StockSkyOverlayTypes.EclipticGridText:
                    Grids.DrawEclipticGridText(renderContext, BlendState.Opacity * opacity, Color);
                    break;
                case StockSkyOverlayTypes.PrecessionChart:
                    Grids.DrawPrecessionChart(renderContext, BlendState.Opacity * opacity, Color);
                    break;
                case StockSkyOverlayTypes.ConstellationFigures:
                    Earth3d.MainWindow.constellationsFigures.Draw3D(renderContext, Settings.Active.ShowConstellationSelection, BlendState.Opacity * opacity, Earth3d.MainWindow.Constellation, renderContext.RenderType != ImageSetType.Sky);
                    break;
                case StockSkyOverlayTypes.ConstellationBoundaries:
                    Earth3d.MainWindow.constellationsBoundries.Draw3D(renderContext, Settings.Active.ShowConstellationSelection, BlendState.Opacity * opacity, Earth3d.MainWindow.Constellation, renderContext.RenderType != ImageSetType.Sky);
                    break;
                case StockSkyOverlayTypes.ConstellationFocusedOnly:
                    break;
                case StockSkyOverlayTypes.ConstellationNames:
                    Constellations.DrawConstellationNames(renderContext, BlendState.Opacity * opacity, Color);
                    BlendState.debugTarget = BlendState;
                    break;
                case StockSkyOverlayTypes.ConstellationPictures:
                    Constellations.DrawConstellationArt(renderContext, BlendState.Opacity * opacity, Color);
                    
                    break;
                default:
                    break;
            }
            //supressAuto = false;
            base.Draw(renderContext, opacity);
        }
        public override bool HasColor
        {
            get
            {
                switch (StockType)
                {
                    case StockSkyOverlayTypes.EquatorialGrid:
                        return true;
                    case StockSkyOverlayTypes.EquatorialGridText:
                        return true;
                    case StockSkyOverlayTypes.GalacticGrid:
                        return true;
                    case StockSkyOverlayTypes.GalacticGridText:
                        return true;
                    case StockSkyOverlayTypes.EclipticGrid:
                        return true;
                    case StockSkyOverlayTypes.EclipticGridText:
                        return true;
                    case StockSkyOverlayTypes.EclipticOverview:
                        return true;
                    case StockSkyOverlayTypes.EclipticOverviewText:
                        return true;
                    case StockSkyOverlayTypes.PrecessionChart:
                        return true;
                    case StockSkyOverlayTypes.AltAzGrid:
                        return true;
                    case StockSkyOverlayTypes.AltAzGridText:
                        return true;
                    case StockSkyOverlayTypes.ConstellationFigures:
                        return true;
                    case StockSkyOverlayTypes.ConstellationBoundaries:
                        return true;
                    case StockSkyOverlayTypes.ConstellationFocusedOnly:
                        return true;
                    case StockSkyOverlayTypes.ConstellationNames:
                        return true;
                    case StockSkyOverlayTypes.ConstellationPictures:
                        return true;
                    case StockSkyOverlayTypes.FadeToBlack:
                        return true;
                    case StockSkyOverlayTypes.FadeToLogo:
                        return true;
                    case StockSkyOverlayTypes.FadeToGradient:
                        return true;

                    case StockSkyOverlayTypes.MPCZone1:
                    case StockSkyOverlayTypes.MPCZone2:
                    case StockSkyOverlayTypes.MPCZone3:
                    case StockSkyOverlayTypes.MPCZone4:
                    case StockSkyOverlayTypes.MPCZone5:
                    case StockSkyOverlayTypes.MPCZone6:
                    case StockSkyOverlayTypes.MPCZone7:
                        return true;

                    case StockSkyOverlayTypes.ScreenBroadcast:
                        break;
                    case StockSkyOverlayTypes.FadeRemoteOnly:
                        break;
                    case StockSkyOverlayTypes.SkyGrids:
                        break;
                    case StockSkyOverlayTypes.Constellations:
                        break;
                    case StockSkyOverlayTypes.SolarSystemStars:
                        break; 
                    case StockSkyOverlayTypes.SolarSystemMilkyWay:
                        break;
                    case StockSkyOverlayTypes.SolarSystemCosmos:
                        break;
                    case StockSkyOverlayTypes.SolarSystemCMB:
                        break;          
                    case StockSkyOverlayTypes.SolarSystemOrbits:
                        break;
                    case StockSkyOverlayTypes.SolarSystemPlanets:
                        break;
                    case StockSkyOverlayTypes.SolarSystemAsteroids:
                        break;
                    case StockSkyOverlayTypes.SolarSystemLighting:
                        break;
                    case StockSkyOverlayTypes.SolarSystemMinorOrbits:
                        break;
                    case StockSkyOverlayTypes.ShowEarthCloudLayer:
                        break;
                    case StockSkyOverlayTypes.ShowElevationModel:
                        break;
                    case StockSkyOverlayTypes.ShowAtmosphere:
                        break;
                    case StockSkyOverlayTypes.MultiResSolarSystemBodies:
                        break;
                    case StockSkyOverlayTypes.AuroraBorialis:
                        break;
                    case StockSkyOverlayTypes.EarthCutAway:
                        break;
                    case StockSkyOverlayTypes.ShowSolarSystem:
                        break;
                    case StockSkyOverlayTypes.Clouds8k:
                        break;
                    case StockSkyOverlayTypes.FiledOfView:
                        return true;
                        
                    default:
                        return false;
                        
                }
                return false;
            }
        }
        public override Color Color
        {
            get
            {
                switch (StockType)
                {
                    case StockSkyOverlayTypes.EquatorialGrid:
                        base.Color = Properties.Settings.Default.GridColor;
                        break;
                    case StockSkyOverlayTypes.EquatorialGridText:
                        base.Color = Properties.Settings.Default.EquatorialGridTextColor;
                        break;
                    case StockSkyOverlayTypes.GalacticGrid:
                        base.Color = Properties.Settings.Default.GalacticGridColor;
                        break;
                    case StockSkyOverlayTypes.GalacticGridText:
                        base.Color = Properties.Settings.Default.GalacticGridTextColor;
                        break;
                    case StockSkyOverlayTypes.EclipticGrid:
                        base.Color = Properties.Settings.Default.EclipticGridColor;
                        break;
                    case StockSkyOverlayTypes.EclipticGridText:
                        base.Color = Properties.Settings.Default.EclipticGridTextColor;
                        break;
                    case StockSkyOverlayTypes.EclipticOverview:
                        base.Color = Properties.Settings.Default.EclipticColor;
                        break;
                    case StockSkyOverlayTypes.EclipticOverviewText:
                        base.Color = Properties.Settings.Default.EclipticGridTextColor;
                        break;
                    case StockSkyOverlayTypes.PrecessionChart:
                        base.Color = Properties.Settings.Default.PrecessionChartColor;
                        break;
                    case StockSkyOverlayTypes.AltAzGrid:
                        base.Color = Properties.Settings.Default.AltAzGridColor;
                        break;
                    case StockSkyOverlayTypes.AltAzGridText:
                        base.Color = Properties.Settings.Default.AltAzGridTextColor;
                        break;
                    case StockSkyOverlayTypes.ConstellationFigures:
                        base.Color = Properties.Settings.Default.ConstellationFigureColor;
                        break;
                    case StockSkyOverlayTypes.ConstellationBoundaries:
                        base.Color = Properties.Settings.Default.ConstellationBoundryColor;
                        break;
                    case StockSkyOverlayTypes.ConstellationFocusedOnly:
                        base.Color = Properties.Settings.Default.ConstellationSelectionColor;
                        break;
                    case StockSkyOverlayTypes.ConstellationNames:
                        base.Color = Properties.Settings.Default.ConstellationnamesColor;
                        break;
                    case StockSkyOverlayTypes.ConstellationPictures:
                        base.Color = Properties.Settings.Default.ConstellationArtColor;
                        break;
                    case StockSkyOverlayTypes.FadeToBlack:
                        base.Color = Properties.Settings.Default.FadeColor;
                        break;
                    default:
                        break;
                }

                return base.Color;
            }
            set
            {
                switch (StockType)
                {
                    case StockSkyOverlayTypes.EquatorialGrid:
                        base.Color = Properties.Settings.Default.GridColor = value;
                        break;
                    case StockSkyOverlayTypes.EquatorialGridText:
                        base.Color = Properties.Settings.Default.EquatorialGridTextColor = value;
                        break;
                    case StockSkyOverlayTypes.GalacticGrid:
                        base.Color = Properties.Settings.Default.GalacticGridColor = value;
                        break;
                    case StockSkyOverlayTypes.GalacticGridText:
                        base.Color = Properties.Settings.Default.GalacticGridTextColor = value;
                        break;
                    case StockSkyOverlayTypes.EclipticGrid:
                        base.Color = Properties.Settings.Default.EclipticGridColor = value;
                        break;
                    case StockSkyOverlayTypes.EclipticGridText:
                        base.Color = Properties.Settings.Default.EclipticGridTextColor = value;
                        break;
                    case StockSkyOverlayTypes.EclipticOverview:
                        base.Color = Properties.Settings.Default.EclipticColor = value;
                        break;
                    case StockSkyOverlayTypes.EclipticOverviewText:
                        base.Color = Properties.Settings.Default.EclipticGridTextColor = value;
                        break;
                    case StockSkyOverlayTypes.PrecessionChart:
                        base.Color = Properties.Settings.Default.PrecessionChartColor = value;
                        break;
                    case StockSkyOverlayTypes.AltAzGrid:
                        base.Color = Properties.Settings.Default.AltAzGridColor = value;
                        break;
                    case StockSkyOverlayTypes.AltAzGridText:
                        base.Color = Properties.Settings.Default.AltAzGridTextColor = value;
                        break;
                    case StockSkyOverlayTypes.ConstellationFigures:
                        base.Color = Properties.Settings.Default.ConstellationFigureColor = value;
                        break;
                    case StockSkyOverlayTypes.ConstellationBoundaries:
                        base.Color = Properties.Settings.Default.ConstellationBoundryColor = value;
                        break;
                    case StockSkyOverlayTypes.ConstellationFocusedOnly:
                        base.Color = Properties.Settings.Default.ConstellationSelectionColor = value;
                        break;
                    case StockSkyOverlayTypes.ConstellationNames:
                        base.Color = Properties.Settings.Default.ConstellationnamesColor = value;
                        break;
                    case StockSkyOverlayTypes.ConstellationPictures:
                        base.Color = Properties.Settings.Default.ConstellationArtColor = value;
                        break;
                    case StockSkyOverlayTypes.FadeToBlack:
                        base.Color = Properties.Settings.Default.FadeColor = value;
                        break;
               
                    default:
                        break;
                }
            }
        }
        public bool supressAuto = false;
        public override bool Enabled
        {
            get
            {

                var enSetting = base.Enabled;
                var enLocal = enSetting;
                switch (StockType)
                {
                    case StockSkyOverlayTypes.FadeToBlack:
                         enLocal = Earth3d.MainWindow.Fader.TargetState;
                         break;
                    case StockSkyOverlayTypes.ScreenBroadcast:
                        enLocal = ScreenBroadcast.Capturing;
                        break;
                    case StockSkyOverlayTypes.FadeRemoteOnly:
                        enLocal = Properties.Settings.Default.FadeRemoteOnly;
                        break;
                    case StockSkyOverlayTypes.Clouds8k:
                        enLocal = Properties.Settings.Default.CloudMap8k;
                        break;
                    case StockSkyOverlayTypes.ShowElevationModel:
                        enLocal = Properties.Settings.Default.ShowElevationModel;
                        break;
                    case StockSkyOverlayTypes.ShowAtmosphere:
                        enLocal = Properties.Settings.Default.ShowEarthSky.TargetState;
                        break;
                    default:
  
                        var sp = Settings.Active.GetSetting(StockType);

                        if (sp.Opacity > -1)
                        {
                            enLocal = sp.TargetState;
                            if (sp.EdgeTrigger)
                            {
                                BlendState.Auto = true;
                                BlendState.TargetState = sp.TargetState;
                            }
                            else
                            {
                                if (!BlendState.Auto || BlendState.TargetState != sp.TargetState || sp.Animated)
                                {
                                    BlendState.TargetState = sp.TargetState;
                                    BlendState.Opacity = (float)sp.Opacity;
                                }
                            }
                            switch (StockType)
                            {
                                case StockSkyOverlayTypes.ConstellationFigures:
                                    Properties.Settings.Default.ConstellationFiguresFilter.Clone(sp.Filter);
                                    break;
                                case StockSkyOverlayTypes.ConstellationNames:
                                    Properties.Settings.Default.ConstellationNamesFilter.Clone(sp.Filter);
                                    break;
                                case StockSkyOverlayTypes.ConstellationPictures:
                                    Properties.Settings.Default.ConstellationArtFilter.Clone(sp.Filter);
                                    break;
                                case StockSkyOverlayTypes.ConstellationBoundaries:
                                    Properties.Settings.Default.ConstellationBoundariesFilter.Clone(sp.Filter);
                                    break;
                                case StockSkyOverlayTypes.OrbitFilters:
                                    Properties.Settings.Default.PlanetOrbitsFilter = (int)sp.Opacity;
                                    break;
                                case StockSkyOverlayTypes.MPCZone1:
                                case StockSkyOverlayTypes.MPCZone2:
                                case StockSkyOverlayTypes.MPCZone3:
                                case StockSkyOverlayTypes.MPCZone4:
                                case StockSkyOverlayTypes.MPCZone5:
                                case StockSkyOverlayTypes.MPCZone6:
                                case StockSkyOverlayTypes.MPCZone7:
                                    {
                                        var id = (int)StockType - (int)StockSkyOverlayTypes.MPCZone1;
                                        var bit = (int)Math.Pow(2, id);
                                        if (BlendState.TargetState)
                                        {
                                            Properties.Settings.Default.MinorPlanetsFilter |= bit;
                                        }
                                        else
                                        {
                                            Properties.Settings.Default.MinorPlanetsFilter &= ~bit;
                                        }
                                    }
                                    break;
                            }

                        }

                        break;
                }

               

                if (enLocal != enSetting)
                {
                    base.enabled = enLocal;
                }
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                switch (StockType)
                {
                    case StockSkyOverlayTypes.SkyGrids:
                        Properties.Settings.Default.ShowSkyGrids.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.Constellations:
                        Properties.Settings.Default.Constellations.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.EquatorialGrid:
                        Properties.Settings.Default.ShowGrid.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.EquatorialGridText:
                        Properties.Settings.Default.ShowEquatorialGridText.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.GalacticGrid:
                        Properties.Settings.Default.ShowGalacticGrid.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.GalacticGridText:
                        Properties.Settings.Default.ShowGalacticGridText.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.AltAzGrid:
                        Properties.Settings.Default.ShowAltAzGrid.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.AltAzGridText:
                        Properties.Settings.Default.ShowAltAzGridText.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.EclipticGrid:
                        Properties.Settings.Default.ShowEclipticGrid.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.EclipticGridText:
                        Properties.Settings.Default.ShowEclipticGridText.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.EclipticOverview:
                        Properties.Settings.Default.ShowEcliptic.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.EclipticOverviewText:
                        Properties.Settings.Default.ShowEclipticOverviewText.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.PrecessionChart:
                        Properties.Settings.Default.ShowPrecessionChart.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.ConstellationFigures:
                        Properties.Settings.Default.ShowConstellationFigures.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.ConstellationBoundaries:
                        Properties.Settings.Default.ShowConstellationBoundries.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.ConstellationFocusedOnly:
                        Properties.Settings.Default.ShowConstellationSelection.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.ConstellationNames:
                        Properties.Settings.Default.ShowConstellationLabels.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.ConstellationPictures:
                        Properties.Settings.Default.ShowConstellationPictures.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.FadeToBlack:
                        Earth3d.MainWindow.Fader.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.ScreenBroadcast:
                        ScreenBroadcast.Capturing = value;
                        break;               
                    case StockSkyOverlayTypes.FadeRemoteOnly:
                        Properties.Settings.Default.FadeRemoteOnly = value;
                        break;
                    case StockSkyOverlayTypes.SolarSystemStars:
                        Properties.Settings.Default.SolarSystemStars.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.SolarSystemMilkyWay:
                        Properties.Settings.Default.SolarSystemMilkyWay.TargetState = value;
                        break;

                    case StockSkyOverlayTypes.VolumetricMilkyWay:
                        Properties.Settings.Default.MilkyWayModel = value;
                        break;

                    case StockSkyOverlayTypes.Show3dCities:
                        Properties.Settings.Default.Show3dCities = value;
                        break;

                    case StockSkyOverlayTypes.SolarSystemCosmos:
                        Properties.Settings.Default.SolarSystemCosmos.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.SolarSystemCMB:
                        Properties.Settings.Default.SolarSystemCMB.TargetState = value;
                        break;      
                    case StockSkyOverlayTypes.MPCZone1:
                    case StockSkyOverlayTypes.MPCZone2:
                    case StockSkyOverlayTypes.MPCZone3:
                    case StockSkyOverlayTypes.MPCZone4:
                    case StockSkyOverlayTypes.MPCZone5:
                    case StockSkyOverlayTypes.MPCZone6:
                    case StockSkyOverlayTypes.MPCZone7:
                        {
                            var id = (int)StockType - (int)StockSkyOverlayTypes.MPCZone1;
                            var bit = (int)Math.Pow(2, id);
                            if (value)
                            {
                                Properties.Settings.Default.MinorPlanetsFilter |= bit;
                            }
                            else
                            {
                                Properties.Settings.Default.MinorPlanetsFilter &= ~bit;
                            }
                        }
                        break;
                    case StockSkyOverlayTypes.ShowSolarSystem:
                        Properties.Settings.Default.ShowSolarSystem.TargetState = value;
                        break;   
                    case StockSkyOverlayTypes.FiledOfView:
                        Properties.Settings.Default.ShowFieldOfView.TargetState = value;
                        break;   
                    case StockSkyOverlayTypes.SolarSystemPlanets:
                        Properties.Settings.Default.SolarSystemPlanets.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.SolarSystemAsteroids:
                        Properties.Settings.Default.SolarSystemMinorPlanets.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.SolarSystemLighting:
                        Properties.Settings.Default.SolarSystemLighting = value;
                        break;
                    case StockSkyOverlayTypes.SolarSystemMinorOrbits:
                        Properties.Settings.Default.SolarSystemMinorOrbits.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.ShowEarthCloudLayer:
                        Properties.Settings.Default.ShowClouds.TargetState = value;
                        break;
                    case StockSkyOverlayTypes.Clouds8k:
                        Properties.Settings.Default.CloudMap8k = value;
                        break;
                    case StockSkyOverlayTypes.ShowElevationModel:
                        Properties.Settings.Default.ShowElevationModel = value;
                        break;
                   case StockSkyOverlayTypes.EarthCutAway:
                        Properties.Settings.Default.EarthCutawayView.TargetState = value;
                        break;           
                    case StockSkyOverlayTypes.MultiResSolarSystemBodies:
                        Properties.Settings.Default.SolarSystemMultiRes = value;
                        break;
                    case StockSkyOverlayTypes.ShowAtmosphere:
                        Properties.Settings.Default.ShowEarthSky.TargetState = value;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
