using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.IO;

using Vector3 = SharpDX.Vector3;
using Matrix = SharpDX.Matrix;

namespace TerraViewer
{
    class KmlLayer : Layer, TerraViewer.ITimeSeriesDescription
    {
        public KmlRoot root = null;
        static Texture11 star = null;
        KmlLayerUI primaryUI = null;
        public override LayerUI GetPrimaryUI()
        {
            if (primaryUI == null)
            {
                primaryUI = new KmlLayerUI(this);
            }
            return primaryUI;
        }
        public static Texture11 Star
        {
            get
            {
                if (star == null)
                {
                    star = Texture11.FromBitmap( Properties.Resources.icon_rating_star_large_on, 0);
                }
                return KmlLayer.star; 
            }
        }
        private TriangleList triangles = new TriangleList();
        private LineList lines = new LineList();

        bool retainedVisualsDirty = true;

        public bool RetainedVisualsDirty
        {
            get { return retainedVisualsDirty; }
            set { retainedVisualsDirty = value; }
        }

        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {
            if (RetainedVisualsDirty)
            {
                UpdateRetainedVisuals();
                RetainedVisualsDirty = false;
            }

            triangles.Draw(renderContext, opacity, TriangleList.CullMode.Clockwise);
            lines.DrawLines(renderContext, opacity);
            DrawPlaceMarks();
            DrawScreenOverlays(renderContext);
            return true;
        }
        public override bool PreDraw(RenderContext11 renderContext, float opacity)
        {

            SetupGroundOverlays(renderContext);
            return true;
        }

        public override void CleanUp()
        {
            triangles.Clear();
            lines.Clear();
            Placemarks.Clear();
            ClearScreenOverlays();
            ClearGroundOverlays();

        }

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            base.AddFilesToCabinet(fc);
        }
        public override void WriteLayerProperties(System.Xml.XmlTextWriter xmlWriter)
        {
            base.WriteLayerProperties(xmlWriter);
        }
        public override void InitializeFromXml(System.Xml.XmlNode node)
        {
            base.InitializeFromXml(node);
        }

        public void UpdateRetainedVisuals()
        {
            triangles.Clear();
            lines.Clear();
            Placemarks.Clear();
            ClearScreenOverlays();
            ClearGroundOverlays();

            if (root.children != null)
            {
                foreach (KmlFeature child in root.children)
                {
                    AddFeatureToDisplay(child, false);
                }
            }

        }
        private void AddFeatureToDisplay(KmlFeature feature, bool sky)
        {
            KmlDocument doc = feature as KmlDocument;
            if (doc != null)
            {
                sky = doc.sky;
            }
            if (!(feature is KmlNetworkLink))
            {
                if (feature.visibility == false)
                {
                    return;
                }
                else if (feature is KmlPlacemark)
                {
                    KmlPlacemark placemark = (KmlPlacemark)feature;
                    KmlStyle style = placemark.Style.GetStyle(placemark.Selected);
                    Color lineColor = Color.White;
                    if (style != null)
                    {
                        lineColor = style.LineStyle.Color;
                    }
                    if (placemark.geometry is KmlPoint)
                    {
                        placemark.Point = (KmlPoint)placemark.geometry;
                        AddPlacemark(placemark);
                    }
                    else if (placemark.geometry is KmlMultiGeometry)
                    {
                        KmlMultiGeometry geo = (KmlMultiGeometry)placemark.geometry;
                        AddMultiGeometry(sky, placemark, (float)style.LineStyle.Width, style.PolyStyle.Color, lineColor, geo);
                    }
                    else if (placemark.geometry is KmlPolygon)
                    {
                        KmlPolygon geo = (KmlPolygon)placemark.geometry;
                        if (geo.OuterBoundary != null)
                        {
                            AddLines(sky, geo.OuterBoundary as KmlLineList, (float)style.LineStyle.Width, style.PolyStyle.Color, lineColor, geo.extrude);
                            // to do 3d work and subtract inner rings
                        }
                    }
                    else if (placemark.geometry is KmlLineString)
                    {
                        KmlLineString geo = (KmlLineString)placemark.geometry;

                        List<Vector3d> vertexList = new List<Vector3d>();
                        for (int i = 0; i < (geo.PointList.Count); i++)
                        {
                            vertexList.Add(Coordinates.GeoTo3dDouble(geo.PointList[i].Lat, geo.PointList[i].Lng, 1 + (geo.PointList[i].Alt / geo.MeanRadius)));
                        }
                        for (int i = 0; i < (geo.PointList.Count - 1); i++)
                        {
                            if (sky)
                            {
                                lines.AddLine(Coordinates.RADecTo3d(-(180.0 - geo.PointList[i].Lng) / 15 + 12, geo.PointList[i].Lat, 1), Coordinates.RADecTo3d(-(180.0 - geo.PointList[i + 1].Lng) / 15 + 12, geo.PointList[i + 1].Lat, 1), lineColor, new Dates());
                            }
                            else
                            {
                                lines.AddLine(vertexList[i], vertexList[i + 1], lineColor, new Dates());
                            }
                        }


                    }
                }
                if (feature is KmlGroundOverlay && feature.visibility)
                {
                    AddGroundOverlay(feature as KmlGroundOverlay);
                }

                if (feature is KmlScreenOverlay && feature.visibility)
                {
                    AddScreenOverlay(feature as KmlScreenOverlay);
                }
            }
            if (feature.visibility)
            {
                if (feature is KmlContainer)
                {
                    KmlContainer container = (KmlContainer)feature;
                    if (container.children != null)
                    {

                        foreach (KmlFeature child in container.children)
                        {
                            AddFeatureToDisplay(child, sky);
                        }
                    }
                }
                else
                {
                    if (feature is KmlNetworkLink)
                    {
                        KmlNetworkLink netLink = (KmlNetworkLink)feature;
                        if (netLink.LinkRoot != null)
                        {
                            foreach (KmlFeature child in netLink.LinkRoot.children)
                            {
                                AddFeatureToDisplay(child, sky);
                            }
                        }
                    }
                }
            }

        }
                        
                            


        private void AddMultiGeometry(bool sky, KmlPlacemark placemark, float width, Color polyColor, Color lineColor, KmlMultiGeometry geo)
        {

            foreach (KmlGeometry childGeo in geo.Children)
            {
                if (childGeo is KmlPoint)
                {
                    KmlPoint point = (KmlPoint)childGeo;

                    placemark.Point = (KmlPoint)childGeo;
                    AddPlacemark(placemark);
                }
                else if (childGeo is KmlLineList)
                {
                    AddLines(sky, childGeo as KmlLineList, width, lineColor, lineColor, false);

                }
                else if (childGeo is KmlPolygon)
                {
                    KmlPolygon child = (KmlPolygon)childGeo;
                    if (child.OuterBoundary != null)
                    {
                        AddLines(sky, child.OuterBoundary as KmlLineList, width, polyColor, lineColor, child.extrude);
                        // to do 3d work and subtract inner rings
                    }
                }
                else if (childGeo is KmlMultiGeometry)
                {
                    AddMultiGeometry(sky, placemark, width, polyColor, lineColor, childGeo as KmlMultiGeometry);
                }
            }
        }

        private void AddLines(bool sky, KmlLineList geo, float lineWidth, Color polyColor, Color lineColor, bool extrude)
        {
            //todo can we save this work for later?
            List<Vector3d> vertexList = new List<Vector3d>();
            List<Vector3d> vertexListGround = new List<Vector3d>();

            //todo list 
            // We need to Wrap Around for complete polygone
            // we aldo need to do intereor
            //todo space? using RA/DEC



            for (int i = 0; i < (geo.PointList.Count); i++)
            {
                vertexList.Add(Coordinates.GeoTo3dDouble(geo.PointList[i].Lat, geo.PointList[i].Lng, 1 + (geo.PointList[i].Alt / geo.MeanRadius)));
                vertexListGround.Add(Coordinates.GeoTo3dDouble(geo.PointList[i].Lat, geo.PointList[i].Lng, 1));
            }


            for (int i = 0; i < (geo.PointList.Count - 1); i++)
            {
                if (sky)
                {
                    lines.AddLine(Coordinates.RADecTo3d(-(180.0 - geo.PointList[i].Lng) / 15 + 12, geo.PointList[i].Lat, 1), Coordinates.RADecTo3d(-(180.0 - geo.PointList[i + 1].Lng) / 15 + 12, geo.PointList[i + 1].Lat, 1), lineColor, new Dates());
                }
                else
                {
                    if (extrude)
                    {

                        triangles.AddQuad(vertexList[i], vertexList[i + 1], vertexListGround[i], vertexListGround[i + 1], polyColor, new Dates());

                    }

                    if (lineWidth > 0)
                    {
                        lines.AddLine
                            (vertexList[i], vertexList[i + 1], lineColor, new Dates());
                        if (extrude)
                        {
                            lines.AddLine(vertexListGround[i], vertexListGround[i + 1], lineColor, new Dates());
                            lines.AddLine(vertexList[i], vertexListGround[i], lineColor, new Dates());
                            lines.AddLine(vertexList[i + 1], vertexListGround[i + 1], lineColor, new Dates());
                        }
                    }
                }
            }

            List<int> indexes = Glu.TesselateSimplePoly(vertexList);

            for (int i = 0; i < indexes.Count; i += 3)
            {
                triangles.AddTriangle(vertexList[indexes[i]], vertexList[indexes[i + 1]], vertexList[indexes[i + 2]], polyColor, new Dates());
            }


        }
        public void AddPlacemark(KmlPlacemark placemark)
        {
            Placemarks.Add(placemark);
        }

        public void DeselectAll()
        {
            TicksAtLastSelect = HiResTimer.TickCount;
            retainedVisualsDirty = true;
            if (!ActiveSelection)
            {
                return;
            }

            foreach (KmlPlacemark placemark in Placemarks)
            {
                if (placemark.Selected)
                {
                    ActiveSelection = false;
                }
            }
        }

        public override IPlace FindClosest(Coordinates target, float distance, IPlace closestPlace, bool astronomical)
        {
            return base.FindClosest(target, distance, closestPlace, astronomical);
        }


        List<KmlPlacemark> Placemarks = new List<KmlPlacemark>();
        public static Int64 TicksAtLastSelect = HiResTimer.TickCount;

        public bool ActiveSelection = false;
        override public bool HoverCheckScreenSpace(Point cursor)
        {
            bool found = false;

            bool refresh = false;
            ActiveSelection = false;
            try
            {
                foreach (KmlPlacemark placemark in Placemarks)
                {
                    if (placemark.Selected)
                    {
                        if (placemark.hitTestRect.Contains(cursor))
                        {
                            ActiveSelection = true;
                            return true;
                        }
                        ActiveSelection = false;
                        placemark.Selected = false;
                        refresh = true;
                    }
                }

                foreach (KmlPlacemark placemark in Placemarks)
                {
                    if (placemark.hitTestRect.Contains(cursor))
                    {
                        if (!placemark.Selected)
                        {
                            placemark.Selected = true;
                            refresh = true;
                            TicksAtLastSelect = HiResTimer.TickCount;
                        }
                        ActiveSelection = true;

                        found = true;
                        break;
                    }
                }


                return found;
            }
            finally
            {
                if (refresh)
                {
                    RetainedVisualsDirty = true;
                }
            }

        }
        override public bool ClickCheckScreenSpace(Point cursor)
        {
            if (!ActiveSelection)
            {
                return false;
            }

            bool found = false;

            foreach (KmlPlacemark placemark in Placemarks)
            {
                if (placemark.Selected)
                {
                    if (placemark.hitTestRect.Contains(cursor))
                    {
                        if (!string.IsNullOrEmpty(placemark.description))
                        {
                            WebWindow.OpenHtmlString(placemark.description.Replace("&amp;", "&"));
                        }
                    }
                }
            }

            return found;

        }
        public void DrawPlaceMarks()
        {
           // todo11 port this Maybe instancing later?
            Matrix projection = Earth3d.MainWindow.RenderContext11.Projection.Matrix11;
            Matrix view = Earth3d.MainWindow.RenderContext11.View.Matrix11;
            Matrix world = Earth3d.MainWindow.RenderContext11.World.Matrix11;
            Matrix3d worldD = Earth3d.MainWindow.RenderContext11.World;
            Matrix wvp = (world * view * projection);
            try
            {
                Vector3 center = new Vector3(0f, 0f, 0);

                foreach (KmlPlacemark placemark in Placemarks)
                {
                    if (placemark.ShouldDisplay())
                    {
                        SharpDX.ViewportF vp = Earth3d.MainWindow.RenderContext11.ViewPort;
                        double alt = placemark.Point.altitude + EGM96Geoid.Height(placemark.Point.latitude, placemark.Point.longitude);
                        Vector3d point3d = Coordinates.GeoTo3dDouble(placemark.Point.latitude, placemark.Point.longitude, 1 + (alt / Earth3d.MainWindow.RenderContext11.NominalRadius));
                        Vector3 point = Vector3.Project(point3d.Vector311, vp.X, vp.Y, vp.Width, vp.Height, 0, 1, wvp);
                        // point.Z = 1;
                        KmlStyle style = placemark.Style.GetStyle(placemark.Selected);
                        Texture11 texture = style.IconStyle.Icon.Texture;

                        if (String.IsNullOrEmpty(style.IconStyle.Icon.Href))
                        {
                            texture = Star;
                        }

                        double sizeFactor = 1;

                        if (placemark.Selected)
                        {
                            double ticks = HiResTimer.TickCount;
                            double elapsedSeconds = ((double)(ticks - TicksAtLastSelect)) / HiResTimer.Frequency;
                            sizeFactor = 1 + .3 * (Math.Sin(elapsedSeconds * 15) * Math.Max(0, (1 - elapsedSeconds)));
                        }

                        point3d.TransformCoordinate(worldD);
                        Vector3d dist = Earth3d.MainWindow.RenderContext11.CameraPosition - point3d;
                        double distance = dist.Length() * Earth3d.MainWindow.RenderContext11.NominalRadius;
                        dist.Normalize();
                        double dot = Vector3d.Dot(point3d, dist);
                        // if (dot > -.2)
                        {
                            double baseSize = Math.Min(40, 25 * ((2 * Math.Atan(.5 * (5884764 / distance))) / .7853)) * sizeFactor;
                            float size = (float)baseSize * style.IconStyle.Scale;
                            //todo fix this with real centers and offset by KML data
                            placemark.hitTestRect = new Rectangle((int)(point.X - (size / 2)), (int)(point.Y - (size / 2)), (int)(size + .5), (int)(size + .5));
                            if (texture != null)
                            {
                                center = new Vector3((float)texture.Width / 2f, (float)texture.Height / 2f, 0);

                                Sprite2d.Draw2D(Earth3d.MainWindow.RenderContext11, texture, new SizeF(size, size), new PointF(center.X, center.Y), (float)(style.IconStyle.Heading * Math.PI / 180f), new PointF(point.X, point.Y), Color.White);
                            }

                            if (style.LabelStyle.Color.A > 0 && style.LabelStyle.Scale > 0)
                            {
                                Rectangle recttext = new Rectangle((int)(point.X + (size / 2) + 10), (int)(point.Y - (size / 2)), 1000, 100);
                                //todo11 Earth3d.MainWindow.labelFont.DrawText(null, placemark.Name, recttext, DrawTextFormat.NoClip, style.LabelStyle.Color);
                            }
                        }
                    }
                }
            }
            finally
            {
             
            }

        }

        List<KmlGroundOverlay> GroundOverlays = new List<KmlGroundOverlay>();

        public void ClearGroundOverlays()
        {
            GroundOverlays.Clear();
        }

        public void AddGroundOverlay(KmlGroundOverlay overlay)
        {
            GroundOverlays.Add(overlay);
        }

        public void SetupGroundOverlays(RenderContext11 renderContext)
        {
            foreach (KmlGroundOverlay overlay in GroundOverlays)
            {
                if (Earth3d.MainWindow.KmlMarkers != null)
                {
                    if (overlay.ShouldDisplay())
                    {
                        Earth3d.MainWindow.KmlMarkers.AddGroundOverlay(overlay);
                    }
                }
            }
        }



        public void ClearScreenOverlays()
        {
            ScreenOverlays.Clear();
        }

        List<KmlScreenOverlay> ScreenOverlays = new List<KmlScreenOverlay>();
        public void AddScreenOverlay(KmlScreenOverlay overlay)
        {
            ScreenOverlays.Add(overlay);

        }

        public void DrawScreenOverlays(RenderContext11 renderContext)
        {
            foreach (KmlScreenOverlay overlay in ScreenOverlays)
            {

                Texture11 texture = overlay.Icon.Texture;

                if (texture != null)
                {
                    PointF center = new PointF();
                    PointF screen = new PointF();

                    SizeF size = new SizeF();

                    if (overlay.RotationSpot.UnitsX == KmlPixelUnits.Fraction)
                    {
                        center.X = overlay.RotationSpot.X * texture.Width;
                    }


                    if (overlay.RotationSpot.UnitsY == KmlPixelUnits.Fraction)
                    {
                        center.Y = overlay.RotationSpot.Y * texture.Height;
                    }

                    Rectangle clientRect = Earth3d.MainWindow.ClearClientArea;

                    Size clientSize = clientRect.Size;

                    if (overlay.ScreenSpot.UnitsX == KmlPixelUnits.Fraction)
                    {
                        screen.X = overlay.ScreenSpot.X * clientSize.Width;
                    }
                    else if (overlay.Size.UnitsX == KmlPixelUnits.Pixels)
                    {
                        screen.X = overlay.ScreenSpot.X;
                    }

                    if (overlay.ScreenSpot.UnitsY == KmlPixelUnits.Fraction)
                    {
                        screen.Y = overlay.ScreenSpot.Y * clientSize.Height;
                    }
                    else if (overlay.Size.UnitsY == KmlPixelUnits.Pixels)
                    {
                        screen.Y = overlay.ScreenSpot.Y;
                    }

                    if (overlay.Size.UnitsX == KmlPixelUnits.Fraction)
                    {
                        if (overlay.Size.X == -1)
                        {
                            size.Width = texture.Width;
                        }
                        else
                        {
                            size.Width = overlay.Size.X * clientSize.Width;
                        }
                    }
                    else if (overlay.Size.UnitsX == KmlPixelUnits.Pixels)
                    {
                        if (overlay.Size.X == -1)
                        {
                            size.Width = texture.Width;
                        }
                        else
                        {
                            size.Width = overlay.Size.X;
                        }
                    }

                    if (overlay.Size.UnitsY == KmlPixelUnits.Fraction)
                    {
                        size.Height = overlay.Size.Y * clientSize.Height;
                    }
                    else if (overlay.Size.UnitsY == KmlPixelUnits.Pixels)
                    {
                        if (overlay.Size.Y == -1)
                        {
                            size.Height = texture.Height;
                        }
                        else
                        {
                            size.Height = overlay.Size.Y;
                        }
                    }
                    screen.Y = clientRect.Bottom - screen.Y - size.Height + center.Y;
                    screen.X = clientRect.Left + screen.X + center.X;


                    Sprite2d.Draw2D(renderContext, texture, size, center, (float)overlay.Rotation, screen, overlay.color);
                    //sprite.Draw2D(texture, Rectangle.Empty, size, center, (float)overlay.Rotation, screen, Color.White);

                }

            }
        }

        public bool IsTimeSeries
        {
            get
            {
                if (root != null)
                {
                    if(!root.TimeSpan.UnBoundedEnd && !root.TimeSpan.UnBoundedBegin)
                    {
                        return true;
                    }
                }
                return false;
            }
            set
            {
                
            }
        }

        public DateTime SeriesStartTime
        {
            get 
            {
                if (root != null)
                {
                    if (!root.TimeSpan.UnBoundedBegin)
                    {
                        return root.TimeSpan.BeginTime;
                    }
                }
                return DateTime.Now;
            }
        }

        public DateTime SeriesEndTime
        {
            get 
            {
                if (root != null)
                {
                    if (!root.TimeSpan.UnBoundedEnd)
                    {
                        return root.TimeSpan.EndTime;
                    }
                }
                return DateTime.Now;
            }
        }

        public TimeSpan TimeStep
        {
            get
            {
                return TimeSpan.FromSeconds(1);
            }
        }
    }

    class KmlLayerUI : LayerUI
    {
        private KmlLayer layer = null;

        public KmlLayerUI(KmlLayer layer)
        {
            this.layer = layer;
        }

        public override List<LayerUIMenuItem> GetNodeContextMenu(LayerUITreeNode node)
        {
            return base.GetNodeContextMenu(node);
        }

        public override List<LayerUITreeNode> GetTreeNodes()
        {

            LayerUITreeNode parent = new LayerUITreeNode();

            AddRootToTree(layer.root, parent);

            return parent.Nodes;
        }

        private void AddRootToTree(KmlRoot root, LayerUITreeNode node)
        {
            if (root.children != null)
            {
                foreach (KmlFeature child in root.children)
                {
                    AddFeatureToTree(child, node);
                }
            }
        }

        private void AddFeatureToTree(KmlFeature feature, LayerUITreeNode parent)
        {
            feature.Dirty = false;
            string text = feature.Name;

            //if (!string.IsNullOrEmpty(feature.description))
            //{
            //    text += Environment.NewLine + feature.description;
            //}
            bool bold = false;
            if (text.Contains("<b>"))
            {
                bold = true;
                text = text.Replace("<b>", "").Replace("</b>", "");
            }

            LayerUITreeNode node = null;

            if (!(feature is KmlNetworkLink))
            {
                node = parent.Add(text);
                node.Tag = feature;
                node.Checked = feature.visibility;
                node.Opened = feature.open;
                node.NodeChecked += new LayerUITreeNodeCheckedDelegate(node_NodeChecked);
                node.NodeUpdated += new LayerUITreeNodeUpdatedDelegate(node_NodeUpdated);
                node.NodeActivated += new LayerUITreeNodeActivatedDelegate(node_NodeActivated);
                node.NodeSelected += new LayerUITreeNodeSelectedDelegate(node_NodeSelected);
                if (bold)
                {
                    node.Bold = true;
                }
            }

            if (feature is KmlContainer)
            {
                KmlContainer container = (KmlContainer)feature;
                if (container.children != null)
                {
                    //  if (feature.Style.GetStyle(false).ListStyle.ListItemType != KmlListItemTypes.CheckHideChildren)
                    {

                        foreach (KmlFeature child in container.children)
                        {
                            AddFeatureToTree(child, node);
                        }
                    }
                }
            }
            else
            {
                if (feature is KmlNetworkLink)
                {
                    KmlNetworkLink netLink = (KmlNetworkLink)feature;
                    if (netLink.LinkRoot != null)
                    {
                        AddRootToTree(netLink.LinkRoot, parent);
                    }
                }
            }

        }

        void node_NodeSelected(LayerUITreeNode node)
        {
            if (node != null)
            {
                KmlPlacemark feature = node.Tag as KmlPlacemark;

                if (feature != null)
                {
                    feature.Selected = true;
                    layer.DeselectAll();
                    layer.RetainedVisualsDirty = true;
                }
            }
        }

        void node_NodeActivated(LayerUITreeNode node)
        {
            if (node != null && node.Tag is KmlFeature)
            {
                KmlFeature feature = (KmlFeature)node.Tag;
                if (feature.LookAt != null)
                {
                    if (feature.sky)
                    {
                        Earth3d.MainWindow.GotoTarget(new TourPlace(feature.Name, feature.LookAt.latitude, (feature.LookAt.longitude + 180) / 15, Classification.Unidentified, "", ImageSetType.Sky, .8), false, false, true);
                    }
                    else
                    {
                        GotoLookAt(feature);
                    }
                }
                else if (node.Tag is KmlPlacemark)
                {
                    KmlPlacemark placemark = (KmlPlacemark)node.Tag;
                    if (placemark.geometry != null)
                    {
                        KmlCoordinate point = placemark.geometry.GetCenterPoint();
                        if (placemark.sky)
                        {
                            Earth3d.MainWindow.GotoTarget(new TourPlace(placemark.Name, point.Lat, (point.Lng + 180) / 15, Classification.Unidentified, "", ImageSetType.Sky, .8), false, false, true);
                        }
                        else
                        {
                            Earth3d.MainWindow.GotoTarget(new TourPlace(placemark.Name, point.Lat, point.Lng, Classification.Unidentified, "", ImageSetType.Earth, .8), false, false, true);
                        }
                    }
                    //if (placemark.geometry is KmlPoint)
                    //{
                    //    KmlPoint point = (KmlPoint)placemark.geometry;
                    //    if (placemark.sky)
                    //    {
                    //        Earth3d.MainWindow.GotoTarget(new TourPlace(placemark.Name, point.latitude, (point.longitude + 180) / 15, Classification.Unidentified, "", ImageSetType.Sky, .8), false, false, true);
                    //    }
                    //    else
                    //    {
                    //        Earth3d.MainWindow.GotoTarget(new TourPlace(placemark.Name, point.latitude, point.longitude, Classification.Unidentified, "", ImageSetType.Earth, .8), false, false, true);
                    //    }
                    //}           
                }
            }
        }

        void node_NodeUpdated(LayerUITreeNode node)
        {
            layer.RetainedVisualsDirty = true;
        }

        void node_NodeChecked(LayerUITreeNode node, bool newState)
        {
            layer.RetainedVisualsDirty = true;
            KmlFeature item = node.Tag as KmlFeature;
            if (item != null)
            {
                KmlFeature feature = node.Tag as KmlFeature;
                if (node.Tag is KmlFeature)
                {
                    // todo wire up true selection
                    bool selected = false;

                    KmlFeature parent = node.Parent.Tag as KmlFeature;
                    bool radioFolder = false;


                    radioFolder = parent == null ? false : parent.Style.GetStyle(selected).ListStyle.ListItemType == KmlListItemTypes.RadioFolder;
                    if (radioFolder)
                    {
                        // Radio folders can't uncheck themselves
                        if (feature.visibility && !node.Checked )
                        {
                            node.Checked = true;
                        }
                        else
                        {
                            feature.visibility = node.Checked;
                        }

                    }
                    else
                    {
                        // Radio folders can't uncheck themselves
                        feature.visibility = node.Checked;

                    }


                    if (feature.visibility)
                    {
                        if (feature.Style.GetStyle(selected).ListStyle.ListItemType != KmlListItemTypes.CheckOffOnly)
                        {
                            if (feature.Style.GetStyle(selected).ListStyle.ListItemType != KmlListItemTypes.RadioFolder)
                            {
                                CheckAllChildren(node);
                            }
                        }

                        // Uncheck siblings if we are a radio button
                        if (node.Parent.Tag is KmlFeature)
                        {
                            if (radioFolder)
                            {
                                foreach (LayerUITreeNode sibling in node.Parent.Nodes)
                                {
                                    if (sibling != node)
                                    {
                                        sibling.Checked = false;
                                        KmlFeature siblingFeature = sibling.Tag as KmlFeature;
                                        if (siblingFeature != null)
                                        {
                                            siblingFeature.visibility = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CheckAllChildren(LayerUITreeNode node)
        {
            // deep check children
            foreach (LayerUITreeNode child in node.Nodes)
            {
                child.Checked = true;
                KmlFeature childFeature = child.Tag as KmlFeature;
                if (childFeature != null)
                {
                    childFeature.visibility = true;
                }
                CheckAllChildren(child);
            }
        }

        public override bool HasTreeViewNodes
        {
            get
            {
                return true;
            }
        }
       


        //todo this stuff below is too tightly coupled to implementtion for winforms
        public static void GotoLookAt(KmlFeature feature)
        {

            //todo add sky support
            CameraParameters camera = new CameraParameters();
            camera.Lat = feature.LookAt.latitude;
            camera.Lng = feature.LookAt.longitude;
            camera.Rotation = feature.LookAt.heading / 180 * Math.PI;
            camera.Angle = -feature.LookAt.tilt / 180 * Math.PI;
            camera.Zoom = UiTools.MetersToZoom(feature.LookAt.range);
            TourPlace p = new TourPlace(feature.Name, camera, Classification.Unidentified, "", ImageSetType.Earth, SolarSystemObjects.Earth);
            Earth3d.MainWindow.GotoTarget(p, false, false, true);
        }


    }


 
}
