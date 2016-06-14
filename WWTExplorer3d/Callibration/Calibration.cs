using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

namespace TerraViewer.Callibration
{
    public partial class Calibration : Form
    {
        public Calibration()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.AddProjector.Text = Language.GetLocalizedText(166, "Add");
            this.EditProjector.Text = Language.GetLocalizedText(502, "Edit");
            this.DeleteProjector.Text = Language.GetLocalizedText(167, "Delete");
            this.label1.Text = Language.GetLocalizedText(701, "Projectors");
            this.label2.Text = Language.GetLocalizedText(702, "Points");
            this.PointWeightLabel.Text = Language.GetLocalizedText(703, "Point Weight");
            this.addToolStripMenuItem.Text = Language.GetLocalizedText(166, "Add");
            this.deleteToolStripMenuItem.Text = Language.GetLocalizedText(167, "Delete");
            this.transferFromEdgesToolStripMenuItem.Text = Language.GetLocalizedText(700, "Transfer from Edges");
            this.moveUpToolStripMenuItem.Text = Language.GetLocalizedText(685, "Move Up");
            this.moveDownToolStripMenuItem.Text = Language.GetLocalizedText(686, "Move Down");
            this.blendPointToolStripMenuItem.Text = Language.GetLocalizedText(704, "Blend Point");
            this.propertiesToolStripMenuItem.Text = Language.GetLocalizedText(20, "Properties");
            this.label6.Text = Language.GetLocalizedText(705, "Iterations");
            this.label4.Text = Language.GetLocalizedText(706, "Blur Size");
            this.MakeBlendMap.Text = Language.GetLocalizedText(707, "Make Blend Maps");
            this.solveZ.Text = Language.GetLocalizedText(708, "Solve Z");
            this.solveY.Text = Language.GetLocalizedText(709, "Solve Y");
            this.solveX.Text = Language.GetLocalizedText(710, "Solve X");
            this.solveRoll.Text = Language.GetLocalizedText(711, "Solve Roll");
            this.solveHeading.Text = Language.GetLocalizedText(712, "Solve Heading");
            this.solvePitch.Text = Language.GetLocalizedText(713, "Solve Pitch");
            this.solveAspect.Text = Language.GetLocalizedText(714, "Solve Aspect");
            this.solveFOV.Text = Language.GetLocalizedText(715, "Solve Fov");
            this.UseRadial.Text = Language.GetLocalizedText(716, "Solve Radial Distortion");
            this.MakeWarpMaps.Text = Language.GetLocalizedText(717, "Make Warp Maps");
            this.useConstraints.Text = Language.GetLocalizedText(718, "Use Constraints");
            this.errorLabel.Text = Language.GetLocalizedText(719, "Average Error");
            this.SolveDistortion.Text = Language.GetLocalizedText(720, "Solve Alignment");
            this.label7.Text = Language.GetLocalizedText(721, "Screen Type");
            this.wwtButton1.Text = Language.GetLocalizedText(722, "Software Update");
            this.SendNewMaps.Text = Language.GetLocalizedText(723, "Send New Maps");
            this.label5.Text = Language.GetLocalizedText(724, "Tilt");
            this.label3.Text = Language.GetLocalizedText(725, "Screen Radius");
            this.ShowOutlines.Text = Language.GetLocalizedText(726, "Outlines");
            this.blackBackground.Text = Language.GetLocalizedText(727, "Black Background");
            this.showGrid.Text = Language.GetLocalizedText(728, "Dome Grid");
            this.showProjectorUI.Text = Language.GetLocalizedText(729, "Calibratis");
            this.Save.Text = Language.GetLocalizedText(168, "Save");
            this.LoadConfig.Text = Language.GetLocalizedText(730, "Load");
            this.label8.Text = Language.GetLocalizedText(731, "Color Correction");
            this.label9.Text = Language.GetLocalizedText(732, "Red");
            this.label10.Text = Language.GetLocalizedText(733, "Green");
            this.label11.Text = Language.GetLocalizedText(734, "Blue");
            this.Text = Language.GetLocalizedText(669, "Multi-Channel Calibration");
            this.Align.Title = Language.GetLocalizedText(790, "Align");
            this.Blend.Title = Language.GetLocalizedText(791, "Blend");



        }

        public CalibrationInfo CalibrationInfo = new CalibrationInfo();

        private CalibrationImageType calibrationImageType = CalibrationImageType.None;
        private int CalibrationProjectorTarget = 0;


        public static void SendViewConfig(int projectorID, ProjectorEntry pe, double domeTilt)
        {
            string command = string.Format("CONFIG,{0},{1},{2},{3},{4},{5},{6},{7},{8}",
                Earth3d.MainWindow.Config.ClusterID.ToString(),
                pe.ID,
                pe.ViewTransform.Heading,
                pe.ViewTransform.Pitch,
                pe.ViewTransform.Roll,
                pe.ViewProjection.FOV / 2,
                pe.ViewProjection.FOV / 2,
                pe.ViewProjection.Aspect,
                domeTilt);

            NetControl.SendCommand(command);
        }

        private void AddProjector_Click(object sender, EventArgs e)
        {
            int maxID = -1;

            foreach (ProjectorEntry p in CalibrationInfo.Projectors)
            {
                if (p.ID > maxID)
                {
                    maxID = p.ID;
                }
            }
            maxID++;


            ProjectorProperties projProps = new ProjectorProperties();
            ProjectorEntry pe = new ProjectorEntry();
            pe.ID = maxID;
            projProps.Projector = pe;
            projProps.CalibrationInfo = CalibrationInfo;
            projProps.AddMode = true;
            bool allGood = false;

            while (!allGood)
            {
                if (projProps.ShowDialog() == DialogResult.OK)
                {
                    if (!CalibrationInfo.ProjLookup.ContainsKey(pe.ID))
                    {
                        CalibrationInfo.Projectors.Add(pe);
                        CalibrationInfo.ProjLookup.Add(pe.ID, pe);
                        allGood = true;
                    }
                    else
                    {
                        UiTools.ShowMessageBox(Language.GetLocalizedText(696, "That ID currently exists. Please choose a unique ID"));
                        allGood = false;
                    }
                }
                else
                {
                    allGood = true;
                }
            }
            ReloadListBox();
        }


        private void DeleteProjector_Click(object sender, EventArgs e)
        {
            ProjectorEntry pe = ProjectorList.SelectedItem as ProjectorEntry;

            if (pe == null)
            {
                return;
            }

            for (int i = CalibrationInfo.Edges.Count - 1; i > -1; i--)
            {
                Edge edge = CalibrationInfo.Edges[i];
                if (edge.Left == pe.ID || edge.Right == pe.ID)
                {
                    CalibrationInfo.Edges.RemoveAt(i);
                }
            }
            CalibrationInfo.Projectors.Remove(pe);
            ReloadListBox();
        }



        void ReloadListBox()
        {
            int currentIndex = ProjectorList.SelectedIndex;

            screenType.Items.Clear();
            screenType.Items.AddRange(Enum.GetNames(typeof(ScreenTypes)));
            screenType.SelectedIndex = (int)CalibrationInfo.ScreenType;

            ProjectorServerPaternPiicker.Items.Clear();
            ProjectorServerPaternPiicker.Items.Add("All Projectors");

            ProjectorList.Items.Clear();
            foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
            {
                ProjectorList.Items.Add(pe);

                ProjectorServerPaternPiicker.Items.Add(pe);
            }

            if (ProjectorList.Items.Count > currentIndex)
            {
                ProjectorList.SelectedIndex = currentIndex;
            }

            ProjectorServerPaternPiicker.SelectedIndex = 0;

            pattern.Items.Clear();
            pattern.Items.AddRange(Enum.GetNames(typeof(CalibrationImageType)));
            pattern.SelectedIndex = (int)calibrationImageType;

            ReloadPointTree();
            DomeRadius.Text = CalibrationInfo.DomeSize.ToString();
            domeTilt.Text = CalibrationInfo.DomeTilt.ToString();
            BlurSize.Value = CalibrationInfo.BlendMarkBlurAmount / 2;
            BlurIterations.Value = CalibrationInfo.BlendMarkBlurIterations;
            BlurSizeText.Text = CalibrationInfo.BlendMarkBlurAmount.ToString();
            blurIterationsText.Text = CalibrationInfo.BlendMarkBlurIterations.ToString();
            useConstraints.Checked = CalibrationInfo.UseConstraints;
            UseRadial.Checked = CalibrationInfo.SolveRadialDistortion;

            solveFOV.Checked = (CalibrationInfo.SolveParameters & (int)SolveParameters.Fov) == (int)SolveParameters.Fov;
            solveAspect.Checked = (CalibrationInfo.SolveParameters & (int)SolveParameters.Aspect) == (int)SolveParameters.Aspect;
            solvePitch.Checked = (CalibrationInfo.SolveParameters & (int)SolveParameters.Pitch) == (int)SolveParameters.Pitch;
            solveHeading.Checked = (CalibrationInfo.SolveParameters & (int)SolveParameters.Heading) == (int)SolveParameters.Heading;
            solveRoll.Checked = (CalibrationInfo.SolveParameters & (int)SolveParameters.Roll) == (int)SolveParameters.Roll;
            solveX.Checked = (CalibrationInfo.SolveParameters & (int)SolveParameters.X) == (int)SolveParameters.X;
            solveY.Checked = (CalibrationInfo.SolveParameters & (int)SolveParameters.Y) == (int)SolveParameters.Y;
            solveZ.Checked = (CalibrationInfo.SolveParameters & (int)SolveParameters.Z) == (int)SolveParameters.Z;

            MousePad.Invalidate();
        }

        private void Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = Language.GetLocalizedText(1176, "WWT Calibration files (*.wtc)|*.wtc");

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;

                OpenConfigFile(fileName);
            }
        }

        private void OpenConfigFile(string fileName)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CalibrationInfo));
                FileStream fs = new FileStream(fileName, FileMode.Open);

                CalibrationInfo = (CalibrationInfo)serializer.Deserialize(fs);

                fs.Close();
                CalibrationInfo.SyncLookupsAndOwners();
                ReloadListBox();
                Properties.Settings.Default.LastDomeConfigFile = fileName;
            }
            catch
            {
                UiTools.ShowMessageBox(Language.GetLocalizedText(697, "Could not open the file. Ensure it is a valid WorldWide Telescope configuration file."), Language.GetLocalizedText(698, "Open Configuration File"));
            }
        }
        private void SaveConfig_Click(object sender, EventArgs e)
        {


            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = Language.GetLocalizedText(1176, "WWT Calibration files (*.wtc)|*.wtc");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CalibrationInfo));
                StreamWriter sw = new StreamWriter(sfd.FileName);

                Properties.Settings.Default.LastDomeConfigFile = sfd.FileName;
                serializer.Serialize(sw, CalibrationInfo);

                sw.Close();
            }
        }

        private void Show_Click(object sender, EventArgs e)
        {
            foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
            {
                string command = "CAL," + Earth3d.MainWindow.Config.ClusterID.ToString() + "," + pe.ID + ",METADATA," + pe.Name + "," + SavedColor.Save(Color.DarkGray) + ",True";
                NetControl.SendCommand(command);
            }
        }

        private void Hide_Click(object sender, EventArgs e)
        {
            string command = "CAL," + Earth3d.MainWindow.Config.ClusterID.ToString() + ",-1,CLOSE";
            NetControl.SendCommand(command);

        }

        private void ProjectorList_DoubleClick(object sender, EventArgs e)
        {
            ShowProjectorProperties();
        }

        private void ShowProjectorProperties()
        {
            if (ProjectorList.SelectedItem != null)
            {
                ProjectorEntry pe = (ProjectorEntry)ProjectorList.SelectedItem;
                ProjectorProperties projProps = new ProjectorProperties();
                projProps.Projector = pe;
                projProps.CalibrationInfo = CalibrationInfo;
                if (projProps.ShowDialog() == DialogResult.OK)
                {
                    ReloadListBox();
                }
            }
        }

        private void Align_Click(object sender, EventArgs e)
        {
            SetAlignBlendMode(true);
        }

        private void Blend_Click(object sender, EventArgs e)
        {
            SetAlignBlendMode(false);

        }

        private void SetAlignBlendMode(bool align)
        {
            if (align)
            {
                Align.Selected = true;
                Blend.Selected = false;
                AlignButtonPanel.Visible = true;
                BlendPanelButtons.Visible = false;
                AlignButtonPanel.Dock = DockStyle.Bottom;
                BlendPanelButtons.Dock = DockStyle.None;
                ReloadPointTree();
            }
            else
            {
                Align.Selected = false;
                Blend.Selected = true;
                AlignButtonPanel.Visible = false;
                BlendPanelButtons.Visible = true;
                AlignButtonPanel.Dock = DockStyle.None;
                BlendPanelButtons.Dock = DockStyle.Bottom;

                ReloadPointTree();
            }
        }

        private void ReloadPointTree()
        {
            PointTree.Nodes.Clear();
            if (Align.Selected)
            {
                TreeNode constraints = PointTree.Nodes.Add(Language.GetLocalizedText(736, "Constraints"));
                // First all the Constraints
                foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
                {
                    TreeNode node = constraints.Nodes.Add(pe.Name);
                    node.Tag = pe;
                    // First all the Constraints

                    foreach (GroundTruthPoint pnt in pe.Constraints)
                    {
                        TreeNode pntNode = node.Nodes.Add(pnt.ToString());
                        pntNode.Tag = pnt;
                    }

                }
                constraints.Expand();
                // Not the Edges
                TreeNode edges = PointTree.Nodes.Add(Language.GetLocalizedText(737, "Edges"));
                // First all the Constraints
                foreach (Edge edge in CalibrationInfo.Edges)
                {
                    TreeNode node = edges.Nodes.Add(CalibrationInfo.GetEdgeDisplayName(edge));
                    node.Tag = edge;
                    foreach (EdgePoint edgePoint in edge.Points)
                    {
                        TreeNode pntNode = node.Nodes.Add(edgePoint.ToString());
                        pntNode.Tag = edgePoint;
                    }
                }
                edges.Expand();
            }
            else
            {
                // First all the Constraints
                foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
                {
                    TreeNode node = PointTree.Nodes.Add(pe.Name);
                    node.Tag = pe;
                    foreach (BlendPoint bp in pe.BlendPolygon)
                    {
                        TreeNode pntNode = node.Nodes.Add(bp.ToString());
                        pntNode.Tag = bp;
                    }
                }
            }
            MousePad.Invalidate();
            SliderTargetNode = null;
            ColorCorrectTarget = null;
            SliderTarget = null;
            WeightTrackBar.Visible = false;
            PointWeightLabel.Visible = false;

        }



        private string GetNodeText(int nodeID)
        {
            if (CalibrationInfo.ProjLookup.ContainsKey(nodeID))
            {
                return CalibrationInfo.ProjLookup[nodeID].Name;
            }
            return "Node " + nodeID.ToString();
        }

        List<RectangleF> regions = new List<RectangleF>();
        List<Rectangle> regionRects = new List<Rectangle>();

        Brush leftBrush;
        Brush rightBrush;
        private void Calibration_Load(object sender, EventArgs e)
        {
            leftBrush = new SolidBrush(Color.Red);
            rightBrush = new SolidBrush(Color.Green);

            MakeRegions();


            for (int i = 1; i < 7; i++)
            {
                ProjectorEntry pe = new ProjectorEntry();
                pe.Name = "Projector " + i.ToString();
                pe.ID = i;

                CalibrationInfo.Projectors.Add(pe);
                CalibrationInfo.ProjLookup.Add(pe.ID, pe);
            }

            CalibrationInfo.AddEdge(new Edge(2, 1));
            CalibrationInfo.AddEdge(new Edge(5, 1));
            CalibrationInfo.AddEdge(new Edge(1, 4));
            CalibrationInfo.AddEdge(new Edge(2, 5));
            CalibrationInfo.AddEdge(new Edge(5, 4));
            CalibrationInfo.AddEdge(new Edge(2, 6));
            CalibrationInfo.AddEdge(new Edge(6, 4));
            CalibrationInfo.AddEdge(new Edge(2, 3));
            CalibrationInfo.AddEdge(new Edge(3, 6));
            CalibrationInfo.AddEdge(new Edge(5, 6));
            CalibrationInfo.AddEdge(new Edge(3, 4));

            ReloadListBox();

            if (File.Exists(Properties.Settings.Default.LastDomeConfigFile))
            {
                OpenConfigFile(Properties.Settings.Default.LastDomeConfigFile);
            }




            BlendPanelButtons.Visible = false;
            BlendPanelButtons.Dock = DockStyle.None;
        }

        private void MakeRegions()
        {
            regions.Clear();
            // 6 Channel Dome First is dummy
            regions.Add(new RectangleF(.287f, .707f, .42f, .287f));
            regions.Add(new RectangleF(0, .287f, .287f, .42f));
            regions.Add(new RectangleF(.287f, 0, .42f, .287f));
            regions.Add(new RectangleF(.707f, .287f, .287f, .42f));
            regions.Add(new RectangleF(.287f, .5f, .42f, .207f));
            regions.Add(new RectangleF(.287f, .287f, .42f, .207f));
            MakeRegionRects();
        }

        private void MousePad_Resize(object sender, EventArgs e)
        {
            MakeRegionRects();

        }
        PointF center;
        PointF bmpCenter;
        float radius = 1;
        float circle = 1;
        PointF GetAltAzFromPoint(PointF point)
        {
            double alt = 0;
            double az = 0;

            double x = point.X - center.X;
            double y = point.Y - center.Y;
            double dist = Math.Sqrt(x * x + y * y);

            alt = 90 - Math.Min(1, dist / radius) * 90;
            az = ((Math.Atan2(y, x) / Math.PI * 180) + 630) % 360;
            return new PointF((float)az, (float)alt);
        }

        PointF GetPointFromAltAz(PointF point)
        {
            PointF retPoint = new PointF();
            point.X += 90;
            retPoint.X = center.X + (float)Math.Cos(point.X / 180 * Math.PI) * ((90 - point.Y) / 90) * radius;
            retPoint.Y = center.Y + (float)Math.Sin(point.X / 180 * Math.PI) * ((90 - point.Y) / 90) * radius;
            return retPoint;
        }

        PointF GetPointFromXYZ(Vector3d point)
        {
            PointF retPoint = new PointF();
            retPoint.X = (float)(center.X + (point.X / CalibrationInfo.DomeSize) * radius);
            retPoint.Y = (float)(center.Y + (-point.Z / CalibrationInfo.DomeSize) * radius);
            return retPoint;
        }

        private void MakeRegionRects()
        {
            regionRects.Clear();
            Rectangle rectClient = MousePad.ClientRectangle;

            float min = (float)Math.Min(rectClient.Width, rectClient.Height);
            circle = min;
            radius = (min / 2) * .95f;
            center = new PointF(rectClient.Width / 2, rectClient.Height / 2);
            float left = center.X - min / 2;
            float top = center.Y - min / 2;

            bmpCenter = new PointF(center.X - left, center.Y - top);

            foreach (RectangleF rectIn in regions)
            {
                regionRects.Add(new Rectangle((int)(left + min * rectIn.Left), (int)(top + min * rectIn.Top), (int)(rectIn.Width * min), (int)(rectIn.Height * min)));
            }
        }

        private void MousePad_Paint(object sender, PaintEventArgs e)
        {
            if (regionRects.Count > 5)
            {
                for (int i = 1; i < 7; i++)
                {
                    RectangleF rect = regionRects[i - 1];

                    Brush textBrush = UiTools.StadardTextBrush;
                    if (i == leftNode)
                    {
                        textBrush = leftBrush;
                    }
                    else if (i == rightNode)
                    {
                        textBrush = rightBrush;
                    }
                    e.Graphics.DrawString(GetNodeText(i), UiTools.StandardHuge, textBrush, rect, UiTools.StringFormatCenterCenter);
                }
            }
            object tag = null;
            if (PointTree.SelectedNode != null)
            {
                tag = PointTree.SelectedNode.Tag;
            }
            if (Align.Selected)
            {
                foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
                {
                    SolveProjector sp = GetSolveProjector(pe.ID);
                    foreach (GroundTruthPoint gt in pe.Constraints)
                    {
                        int size = 2;
                        if (gt == tag)
                        {
                            size = 8;
                        }
                        Vector2d pntAltAz = sp.GetCoordinatesForScreenPoint(gt.X, gt.Y);
                        PointF pnt = GetPointFromAltAz(new PointF((float)pntAltAz.X + 90, (float)pntAltAz.Y));
                        e.Graphics.DrawLine(Pens.Blue, PointF.Add(pnt, new Size(-size, 0)), PointF.Add(pnt, new Size(size, 0)));
                        e.Graphics.DrawLine(Pens.Blue, PointF.Add(pnt, new Size(0, -size)), PointF.Add(pnt, new Size(0, size)));
                    }
                }
                foreach (Edge edge in CalibrationInfo.Edges)
                {
                    SolveProjector left = GetSolveProjector(edge.Left);
                    SolveProjector right = GetSolveProjector(edge.Right);

                    foreach (EdgePoint ep in edge.Points)
                    {
                        {
                            int size = 2;
                            if (ep == tag)
                            {
                                size = 8;
                            }
                            Vector2d pntLeft = left.GetCoordinatesForScreenPoint(ep.Left.X, ep.Left.Y);
                            PointF pnt = GetPointFromAltAz(new PointF((float)pntLeft.X + 90, (float)pntLeft.Y));
                            e.Graphics.DrawLine(Pens.Red, PointF.Add(pnt, new Size(-size, 0)), PointF.Add(pnt, new Size(size, 0)));
                            e.Graphics.DrawLine(Pens.Red, PointF.Add(pnt, new Size(0, -size)), PointF.Add(pnt, new Size(0, size)));
                        }
                        {
                            int size = 2;
                            if (ep == tag)
                            {
                                size = 8;
                            }
                            Vector2d pntRight = right.GetCoordinatesForScreenPoint(ep.Right.X, ep.Right.Y);
                            PointF pnt = GetPointFromAltAz(new PointF((float)pntRight.X + 90, (float)pntRight.Y));
                            e.Graphics.DrawLine(Pens.Green, PointF.Add(pnt, new Size(-size, 0)), PointF.Add(pnt, new Size(size, 0)));
                            e.Graphics.DrawLine(Pens.Green, PointF.Add(pnt, new Size(0, -size)), PointF.Add(pnt, new Size(0, size)));
                        }
                    }
                }
            }
            else
            {
                foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
                {
                    SolveProjector sp = GetSolveProjector(pe.ID);

                    List<PointF> points = new List<PointF>();
                    List<PointF> polyPoints = new List<PointF>();
                    int selected = -1;
                    int index = 0;
                    foreach (BlendPoint bp in pe.BlendPolygon)
                    {
                        if (bp == tag)
                        {
                            selected = index;
                        }
                        index++;
                        polyPoints.Add(new PointF((float)bp.X, (float)bp.Y));
                        Vector2d pntAltAz = sp.GetCoordinatesForScreenPoint(bp.X, bp.Y);
                        PointF pnt = GetPointFromAltAz(new PointF((float)pntAltAz.X + 90, (float)pntAltAz.Y));
                        points.Add(pnt);

                    }

                    if (points.Count > 2)
                    {
                        polyPoints = InterpolatePolygon(polyPoints);

                        List<PointF> polyPointsScreenSpace = new List<PointF>();
                        foreach (PointF polyPnt in polyPoints)
                        {
                            Vector2d pntAltAz = sp.GetCoordinatesForScreenPoint(polyPnt.X, polyPnt.Y);
                            PointF pnt = GetPointFromAltAz(new PointF((float)pntAltAz.X + 90, (float)pntAltAz.Y));
                            polyPointsScreenSpace.Add(pnt);
                        }

                        SolidBrush white = new SolidBrush(Color.FromArgb(129, 255, 255, 255));

                        e.Graphics.FillPolygon(white, polyPointsScreenSpace.ToArray());

                        white.Dispose();
                    }

                    index = 0;
                    foreach (PointF point in points)
                    {
                        int size = 2;
                        if (index == selected)
                        {
                            size = 8;
                        }
                        index++;
                        e.Graphics.DrawLine(Pens.Yellow, PointF.Add(point, new Size(-size, 0)), PointF.Add(point, new Size(size, 0)));
                        e.Graphics.DrawLine(Pens.Yellow, PointF.Add(point, new Size(0, -size)), PointF.Add(point, new Size(0, size)));

                    }
                }
            }
        }

        List<PointF> InterpolatePolygon(List<PointF> pointsIn)
        {
            List<PointF> pointsOut = new List<PointF>();

            PointF lastPoint = pointsIn[pointsIn.Count - 1];
            foreach (PointF point in pointsIn)
            {
                float distX = point.X - lastPoint.X;
                float distY = point.Y - lastPoint.Y;

                float distance = (float)Math.Sqrt(distX * distX + distY * distY);
                int steps = (int)(distance / 10);

                for (int i = 1; i < steps; i++)
                {
                    pointsOut.Add(new PointF(lastPoint.X + (distX / steps * i), lastPoint.Y + (distY / steps * i)));
                }
                pointsOut.Add(point);
                lastPoint = point;
            }

            return pointsOut;

        }

        List<SolveProjector> solveProjectors = new List<SolveProjector>();



        private SolveProjector GetSolveProjector(int index)
        {
            if (solveProjectors.Count == 0)
            {
                ProjectorEntry pe = CalibrationInfo.Projectors.Find(delegate (ProjectorEntry p) { return p.ID == index; });

                SolveProjector sp = new SolveProjector(pe, CalibrationInfo.DomeSize, CalibrationInfo.ScreenType == ScreenTypes.FishEye ? ProjectionType.FishEye : ProjectionType.Projector, CalibrationInfo.ScreenType, CalibrationInfo.ScreenType == ScreenTypes.FishEye ? SolveParameters.FishEye : SolveParameters.Default);
                sp.RadialDistorion = CalibrationInfo.ScreenType == ScreenTypes.FishEye ? false : UseRadial.Checked;
                return sp;
            }
            else
            {
                return solveProjectors[index - 1];
            }
        }

        private void EditProjector_Click(object sender, EventArgs e)
        {
            ShowProjectorProperties();
        }

        private void showProjectorUI_CheckedChanged(object sender, EventArgs e)
        {
            showUI = showProjectorUI.Checked;
            if (!showUI)
            {
                string command = "CAL," + Earth3d.MainWindow.Config.ClusterID.ToString() + ",-1,CLOSE";
                NetControl.SendCommand(command);
            }
        }
        bool showUI = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (showUI)
            {
                Color backgroundColor = blackBackground.Checked ? Color.Black : Color.DarkGray;
                foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
                {
                    string command = "CAL," + Earth3d.MainWindow.Config.ClusterID.ToString()
                        + "," + pe.ID
                        + ",METADATA,"
                        + pe.Name + ","
                        + SavedColor.Save(backgroundColor)
                        + "," + outline.ToString()
                        + "," + ((int)calibrationImageType).ToString()
                        + "," + CalibrationProjectorTarget.ToString();

                    NetControl.SendCommand(command);
                    if (Align.Selected)
                    {
                        if (rightNode == -1)
                        {
                            SendAlignPointEditUpdate(pe, false);
                        }
                        else
                        {
                            // if this is neither the right or left node for align points clear the projector
                            if (pe.ID != rightNode && pe.ID != leftNode)
                            {
                                SendAlignPointEditUpdate(pe, true);
                            }
                        }
                    }
                    else
                    {
                        SendBlendPointEditUpdate(pe);
                    }
                }
                if (rightNode > -1 && selectedEdge != null)
                {
                    SendEdgePointEditUpdate(selectedEdge);
                }
            }
        }

        private void showGrid_CheckedChanged(object sender, EventArgs e)
        {

        }
        bool outline = false;
        private void ShowOutlines_CheckedChanged(object sender, EventArgs e)
        {
            outline = ShowOutlines.Checked;
        }

        int leftNode = -1;

        int rightNode = -1;
        Edge selectedEdge = null;
        GroundTruthPoint sliderTarget = null;

        private void WeightTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (sliderTarget != null)
            {
                if (WeightTrackBar.Value < 50)
                {
                    sliderTarget.Weight = WeightTrackBar.Value / 50.0;
                }
                else
                {
                    sliderTarget.Weight = (WeightTrackBar.Value - 50.0) / 10.0 + 1.0;
                }
                SliderTargetNode.Text = SliderTargetNode.Tag.ToString();
            }
        }

        public TreeNode SliderTargetNode = null;

        public GroundTruthPoint SliderTarget
        {
            get { return sliderTarget; }
            set
            {
                sliderTarget = null;
                if (value != null)
                {
                    if (value.Weight < 1)
                    {
                        WeightTrackBar.Value = (int)Math.Max(0, value.Weight * 50);
                    }
                    else
                    {
                        WeightTrackBar.Value = (int)Math.Min(100, (value.Weight - 1) * 10 + 50);
                    }
                }
                WeightTrackBar.Visible = (value != null);
                PointWeightLabel.Visible = WeightTrackBar.Visible;
                sliderTarget = value;
            }
        }

        ProjectorEntry colorCorrectTarget = null;

        public ProjectorEntry ColorCorrectTarget
        {
            get
            {
                return colorCorrectTarget;
            }
            set
            {
                if (colorCorrectTarget != value)
                {
                    colorCorrectTarget = value;
                    bool enabled = value != null;

                    redSlider.Enabled = enabled;
                    greenSlider.Enabled = enabled;
                    blueSlider.Enabled = enabled;
                    redAmount.Enabled = enabled;
                    greenAmount.Enabled = enabled;
                    blueAmount.Enabled = enabled;
                    if (enabled)
                    {
                        label8.Text = Language.GetLocalizedText(859, "Color Correction: ") + colorCorrectTarget.Name;

                        redSlider.Value = Math.Max(0, colorCorrectTarget.WhiteBalance.Red - 155);
                        greenSlider.Value = Math.Max(0, colorCorrectTarget.WhiteBalance.Green - 155);
                        blueSlider.Value = Math.Max(0, colorCorrectTarget.WhiteBalance.Blue - 155);
                        redAmount.Text = colorCorrectTarget.WhiteBalance.Red.ToString();
                        greenAmount.Text = colorCorrectTarget.WhiteBalance.Green.ToString();
                        blueAmount.Text = colorCorrectTarget.WhiteBalance.Blue.ToString();
                    }
                    else
                    {
                        label8.Text = Language.GetLocalizedText(860, "Color Correction: (No Selection)");
                        redSlider.Value = 100;
                        greenSlider.Value = 100;
                        blueSlider.Value = 100;
                        redAmount.Text = "";
                        greenAmount.Text = "";
                        blueAmount.Text = "";
                    }
                }
            }
        }

        private void PointTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SliderTargetNode = null;
            SliderTarget = null;
            ColorCorrectTarget = null;
            rightNode = -1;
            leftNode = -1;
            selectedEdge = null;
            if (e.Node != null && e.Node.Tag is Edge)
            {
                Edge edge = e.Node.Tag as Edge;
                selectedEdge = edge;
                leftNode = edge.Left;
                rightNode = edge.Right;
            }
            if (e.Node != null && e.Node.Tag is EdgePoint)
            {
                Edge edge = e.Node.Parent.Tag as Edge;
                EdgePoint ep = e.Node.Tag as EdgePoint;
                if (edge != null)
                {
                    selectedEdge = edge;
                    leftNode = edge.Left;
                    rightNode = edge.Right;

                    SliderTarget = ep.Left;
                    SliderTargetNode = e.Node;
                    edge.SelectedPoint = edge.Points.IndexOf(ep);
                }
            }
            if (e.Node != null && e.Node.Tag is ProjectorEntry)
            {
                ProjectorEntry pe = e.Node.Tag as ProjectorEntry;
                rightNode = -1;
                leftNode = pe.ID;
                ColorCorrectTarget = pe;
            }

            if (e.Node != null && e.Node.Parent != null && e.Node.Parent.Tag is ProjectorEntry)
            {
                ProjectorEntry pe = e.Node.Parent.Tag as ProjectorEntry;
                rightNode = -1;
                leftNode = pe.ID;

                if (Align.Selected)
                {
                    GroundTruthPoint gt = e.Node.Tag as GroundTruthPoint;
                    if (gt != null)
                    {

                        int index = pe.Constraints.IndexOf(gt);

                        SliderTarget = gt;
                        SliderTargetNode = e.Node;
                        pe.SelectedGroundTruth = index;
                        SendAlignPointEditUpdate(pe, false);
                    }
                }
                else
                {
                    BlendPoint bp = e.Node.Tag as BlendPoint;
                    if (bp != null)
                    {

                        int index = pe.BlendPolygon.IndexOf(bp);

                        pe.SelectedBlendPoint = index;
                        SendBlendPointEditUpdate(pe);
                    }
                }


            }

            MousePad.Invalidate();

        }

        private void Calibration_FormClosed(object sender, FormClosedEventArgs e)
        {
            leftBrush.Dispose();
            rightBrush.Dispose();
        }

        private void AddPoint_Click(object sender, EventArgs e)
        {
            AddPointToTree();
        }

        double[] alts = new double[] { 0, 25, 50, 75 };
        double[] centers = new double[] { 180, 90, 0, 270, 180, 0 };
        double[] altCenter = new double[] { 30, 30, 30, 30, 70, 70 };
        private void AddPointToTree()
        {
            if (PointTree.SelectedNode == null)
            {
                return;
            }

            if (Align.Selected)
            {
                // Code to build sets of points 

                //if (Control.ModifierKeys == Keys.Alt || Control.ModifierKeys == (Keys.Alt | Keys.Shift))
                //{
                //    bool North = Control.ModifierKeys == (Keys.Alt | Keys.Shift);

                //    if (PointTree.SelectedNode.Tag is ProjectorEntry)
                //    {
                //        Random rnd = new Random();

                //        ProjectorEntry pe = PointTree.SelectedNode.Tag as ProjectorEntry;

                //        for (double alt = 0; alt < 90; alt+=15)
                //        {

                //            //double alt = alts[i];
                //            double steps = 25;

                //            //if (i == 3)
                //            //{
                //            //    steps = 5;
                //            //}


                //            for (double az = 0; az < 360; az += 360 / steps)
                //            {
                //                GroundTruthPoint pnt = new GroundTruthPoint();

                //                int cX = pe.Width / 2;
                //                int cY = pe.Height / 2;
                //                cY = 120;

                //                double rad = pe.Height / 2;
                //                rad = pe.Width / 2;

                //                double az2 = az - 90;
                //                pnt.X = cX + (float)Math.Cos(az2 / 180 * Math.PI) * ((90 - alt) / 90) * rad;
                //                pnt.Y = cY - (float)Math.Sin(az2 / 180 * Math.PI) * ((90 - alt) / 90) * rad;

                //                pnt.X += rnd.NextDouble() / 1000f;
                //                pnt.Y += rnd.NextDouble() / 1000f;
                //                //pnt.X = pe.Width / 2;
                //                //pnt.Y = pe.Height / 2;

                //                pnt.Alt = alt;
                //                pnt.Az = az;
                //                pnt.AxisType = AxisTypes.Both;

                //                if (pnt.Y > 0)
                //                {
                //                    pe.Constraints.Add(pnt);
                //                    pe.SelectedGroundTruth = pe.Constraints.Count - 1;

                //                    TreeNode child = PointTree.SelectedNode.Nodes.Add(pnt.ToString());
                //                    child.Tag = pnt;
                //                }

                //            }
                //        }
                //    }

                if (Control.ModifierKeys == Keys.Alt || Control.ModifierKeys == (Keys.Alt | Keys.Shift))
                {
                    bool North = Control.ModifierKeys == (Keys.Alt | Keys.Shift);

                    if (PointTree.SelectedNode.Tag is ProjectorEntry)
                    {
                        Random rnd = new Random();

                        ProjectorEntry pe = PointTree.SelectedNode.Tag as ProjectorEntry;

                        for (int i = 0; i < 4; i++)
                        {

                            double alt = alts[i];
                            double steps = 25;

                            if (i == 3)
                            {
                                steps = 5;
                            }


                            for (double az = 7.2; az < 360; az += 360 / steps)
                            {
                                GroundTruthPoint pnt = new GroundTruthPoint();

                                int cX = pe.Width / 2;
                                int cY = pe.Height / 2;
                                cY = 120;

                                double rad = pe.Height / 2;
                                rad = pe.Width / 2;

                                double az2 = az - 90;
                                //pnt.X = cX - (float)Math.Cos(az2 / 180 * Math.PI) * ((90 - alt) / 90) * rad;
                                //pnt.Y = cY + (float)Math.Sin(az2 / 180 * Math.PI) * ((90 - alt) / 90) * rad;

                                //pnt.X += rnd.NextDouble() / 1000f;
                                //pnt.Y += rnd.NextDouble() / 1000f;
                                pnt.X = pe.Width / 2;
                                pnt.Y = pe.Height / 2;

                                pnt.Alt = alt;
                                pnt.Az = az;
                                pnt.AxisType = AxisTypes.Both;
                                if (altCenter[pe.ID - 1] < 70)
                                {
                                    if (((Math.Abs(az - centers[pe.ID - 1]) < 55) || (Math.Abs((az - 360) - centers[pe.ID - 1]) < 55)) && alt < 75)
                                    {
                                        pe.Constraints.Add(pnt);
                                        pe.SelectedGroundTruth = pe.Constraints.Count - 1;

                                        TreeNode child = PointTree.SelectedNode.Nodes.Add(pnt.ToString());
                                        child.Tag = pnt;
                                    }
                                }
                                else if (altCenter[pe.ID - 1] == 70)
                                {
                                    if (((Math.Abs(az - centers[pe.ID - 1]) < 95) || (Math.Abs((az - 360) - centers[pe.ID - 1]) < 95)) && alt > 25)
                                    {
                                        pe.Constraints.Add(pnt);
                                        pe.SelectedGroundTruth = pe.Constraints.Count - 1;

                                        TreeNode child = PointTree.SelectedNode.Nodes.Add(pnt.ToString());
                                        child.Tag = pnt;
                                    }
                                }

                            }
                        }
                    }


                    return;
                }

                if (PointTree.SelectedNode.Tag is ProjectorEntry)
                {
                    ProjectorEntry pe = PointTree.SelectedNode.Tag as ProjectorEntry;

                    GroundTruthPointProperties props = new GroundTruthPointProperties();

                    GroundTruthPoint pnt = new GroundTruthPoint();
                    pnt.X = pe.Width / 2;
                    pnt.Y = pe.Height / 2;

                    props.Target = pnt;
                    if (props.ShowDialog() == DialogResult.OK)
                    {

                        pe.Constraints.Add(pnt);

                        pe.SelectedGroundTruth = pe.Constraints.Count - 1;

                        TreeNode child = PointTree.SelectedNode.Nodes.Add(pnt.ToString());
                        child.Tag = pnt;
                        PointTree.SelectedNode = child;
                    }
                    return;
                }
                if (PointTree.SelectedNode.Tag is GroundTruthPoint)
                {
                    ProjectorEntry pe = PointTree.SelectedNode.Parent.Tag as ProjectorEntry;

                    GroundTruthPointProperties props = new GroundTruthPointProperties();

                    GroundTruthPoint pnt = new GroundTruthPoint();
                    pnt.X = pe.Width / 2;
                    pnt.Y = pe.Height / 2;

                    props.Target = pnt;
                    if (props.ShowDialog() == DialogResult.OK)
                    {
                        int index = pe.Constraints.IndexOf((GroundTruthPoint)PointTree.SelectedNode.Tag) + 1;

                        pe.Constraints.Insert(index, pnt);



                        TreeNode child = PointTree.SelectedNode.Parent.Nodes.Insert(PointTree.SelectedNode.Index + 1, pnt.ToString());
                        child.Tag = pnt;
                        PointTree.SelectedNode = child;
                    }
                    return;
                }


                if (PointTree.SelectedNode.Tag is Edge)
                {
                    Edge edge = PointTree.SelectedNode.Tag as Edge;

                    EdgePoint ep = new EdgePoint();

                    ep.Left = new GroundTruthPoint();
                    ep.Right = new GroundTruthPoint();

                    ep.Left.X = CalibrationInfo.ProjLookup[edge.Left].Width / 2;
                    ep.Left.Y = CalibrationInfo.ProjLookup[edge.Left].Height / 2;
                    ep.Right.X = CalibrationInfo.ProjLookup[edge.Left].Width / 2;
                    ep.Right.Y = CalibrationInfo.ProjLookup[edge.Left].Height / 2;
                    edge.Points.Add(ep);
                    TreeNode child = PointTree.SelectedNode.Nodes.Add(ep.ToString());
                    child.Tag = ep;
                    PointTree.SelectedNode = child;
                    return;
                }
                if (PointTree.SelectedNode.Tag is EdgePoint)
                {
                    Edge edge = PointTree.SelectedNode.Parent.Tag as Edge;

                    EdgePoint ep = new EdgePoint();

                    ep.Left = new GroundTruthPoint();
                    ep.Right = new GroundTruthPoint();

                    ep.Left.X = CalibrationInfo.ProjLookup[edge.Left].Width / 2;
                    ep.Left.Y = CalibrationInfo.ProjLookup[edge.Left].Height / 2;
                    ep.Right.X = CalibrationInfo.ProjLookup[edge.Left].Width / 2;
                    ep.Right.Y = CalibrationInfo.ProjLookup[edge.Left].Height / 2;

                    edge.Points.Insert(PointTree.SelectedNode.Index + 1, ep);
                    TreeNode child = PointTree.SelectedNode.Parent.Nodes.Insert(PointTree.SelectedNode.Index + 1, ep.ToString());
                    child.Tag = ep;
                    PointTree.SelectedNode = child;
                    return;
                }

                if (PointTree.SelectedNode.Text == "Edges")
                {
                    Edge edge = new Edge(-1, -1);
                    EdgeProperties props = new EdgeProperties();
                    props.Edge = edge;
                    edge.Owner = CalibrationInfo;

                    if (props.ShowDialog() == DialogResult.OK)
                    {
                        if (edge.Left != -1 && edge.Right != -1 && edge.Right != edge.Left)
                        {
                            CalibrationInfo.AddEdge(edge);
                            ReloadPointTree();
                        }
                    }
                    return;
                }
            }
            else
            {
                if (PointTree.SelectedNode.Tag is ProjectorEntry)
                {
                    ProjectorEntry pe = PointTree.SelectedNode.Tag as ProjectorEntry;
                    BlendPoint pnt = new BlendPoint();
                    pnt.X = pe.Width / 2;
                    pnt.Y = pe.Height / 2;

                    pe.BlendPolygon.Add(pnt);

                    pe.SelectedBlendPoint = pe.BlendPolygon.Count - 1;

                    TreeNode child = PointTree.SelectedNode.Nodes.Add(pnt.ToString());
                    child.Tag = pnt;
                    PointTree.SelectedNode = child;
                    return;

                }

                if (PointTree.SelectedNode.Parent != null && PointTree.SelectedNode.Parent.Tag is ProjectorEntry)
                {
                    BlendPoint bp = (BlendPoint)PointTree.SelectedNode.Tag;

                    ProjectorEntry pe = PointTree.SelectedNode.Parent.Tag as ProjectorEntry;
                    BlendPoint pnt = new BlendPoint();
                    pnt.X = pe.Width / 2;
                    pnt.Y = pe.Height / 2;

                    int index = pe.BlendPolygon.IndexOf(bp) + 1;
                    pe.BlendPolygon.Insert(index, pnt);

                    pe.SelectedBlendPoint = index + 1;


                    TreeNode child = PointTree.SelectedNode.Parent.Nodes.Insert(index, pnt.ToString());
                    child.Tag = pnt;
                    PointTree.SelectedNode = child;
                    return;
                }
            }
        }

        public void RefreshTreeeNames(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null)
                {
                    node.Text = node.Tag.ToString();
                }
                RefreshTreeeNames(node.Nodes);
            }

        }



        bool drag = false;
        Point pntLast = Point.Empty;

        private void MousePad_MouseDown(object sender, MouseEventArgs e)
        {
            if (PointTree.SelectedNode == null)
            {
                return;
            }

            if (Align.Selected)
            {
                GroundTruthPoint gt = PointTree.SelectedNode.Tag as GroundTruthPoint;
                if (gt != null)
                {
                    drag = true;
                    pntLast = e.Location;
                }
                else
                {
                    EdgePoint ep = PointTree.SelectedNode.Tag as EdgePoint;
                    if (ep != null)
                    {
                        drag = true;
                        pntLast = e.Location;
                    }
                }
            }
            else
            {
                BlendPoint bp = PointTree.SelectedNode.Tag as BlendPoint;
                if (bp != null)
                {
                    drag = true;
                    pntLast = e.Location;
                }
            }
        }

        private void MousePad_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                if (Align.Selected)
                {
                    GroundTruthPoint gt = PointTree.SelectedNode.Tag as GroundTruthPoint;
                    ProjectorEntry pe = PointTree.SelectedNode.Parent.Tag as ProjectorEntry;
                    EdgePoint ep = PointTree.SelectedNode.Tag as EdgePoint;
                    Edge edge = PointTree.SelectedNode.Parent.Tag as Edge;

                    if (gt != null && pe != null)
                    {
                        Point delta = new Point(e.Location.X - pntLast.X, e.Location.Y - pntLast.Y);

                        gt.X += delta.X;
                        gt.Y += delta.Y;
                        SendAlignPointEditUpdate(pe, false);
                        MousePad.Invalidate();
                    }
                    if (ep != null && edge != null)
                    {
                        Point delta = new Point(e.Location.X - pntLast.X, e.Location.Y - pntLast.Y);
                        if (e.Button == MouseButtons.Left)
                        {
                            ep.Left.X += delta.X;
                            ep.Left.Y += delta.Y;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            ep.Right.X += delta.X;
                            ep.Right.Y += delta.Y;
                        }
                        SendEdgePointEditUpdate(edge);
                        MousePad.Invalidate();

                    }

                }
                else
                {
                    BlendPoint bp = PointTree.SelectedNode.Tag as BlendPoint;
                    ProjectorEntry pe = PointTree.SelectedNode.Parent.Tag as ProjectorEntry;

                    if (bp != null && pe != null)
                    {
                        Point delta = new Point(e.Location.X - pntLast.X, e.Location.Y - pntLast.Y);

                        bp.X += delta.X;
                        bp.Y += delta.Y;
                        SendBlendPointEditUpdate(pe);
                        MousePad.Invalidate();
                    }
                }
                pntLast = e.Location;
            }
            else
            {
                //PointF pnt = GetAltAzFromPoint(new PointF(e.X, e.Y));
                //PointF pntBack = GetPointFromAltAz(pnt);

                //this.Text = string.Format("X={0}, Y={1}, alt/az = {2}, pntBack = {3}", e.X, e.Y, pnt, pntBack);
            }
        }

        private void MousePad_MouseUp(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                RefreshTreeeNames(PointTree.Nodes);
            }
            drag = false;
        }

        private void SendBlendPointEditUpdate(ProjectorEntry pe)
        {
            StringBuilder sb = new StringBuilder();

            bool first = true;
            foreach (BlendPoint bp in pe.BlendPolygon)
            {
                if (!first)
                {
                    sb.Append(";");
                }
                else
                {
                    first = false;
                }
                sb.Append(bp.X.ToString());
                sb.Append(" ");
                sb.Append(bp.Y.ToString());
                sb.Append(" ");
                sb.Append(bp.Softness.ToString());
            }

            string command = "CAL," + Earth3d.MainWindow.Config.ClusterID.ToString() + "," + pe.ID + ",GEOMETRY," + GeometryStyles.Polygon.ToString() + "," + pe.SelectedBlendPoint.ToString() + "," + SavedColor.Save(Color.FromArgb(255, Gamma(pe.WhiteBalance.Red), Gamma(pe.WhiteBalance.Green), Gamma(pe.WhiteBalance.Blue))) + "," + SavedColor.Save(Color.Yellow) + "," + sb.ToString();
            NetControl.SendCommand(command);

        }

        public int Gamma(int val)
        {
            return (byte)Math.Min(255, (int)((255.0 * Math.Pow(val / 255.0, 1.0 / CalibrationInfo.BlendGamma)) + 0.5));
        }

        private void SendAlignPointEditUpdate(ProjectorEntry pe, bool emptyList)
        {

            StringBuilder sb = new StringBuilder();
            if (!emptyList)
            {
                bool first = true;
                foreach (GroundTruthPoint gt in pe.Constraints)
                {
                    if (!first)
                    {
                        sb.Append(";");
                    }
                    else
                    {
                        first = false;
                    }
                    sb.Append(gt.X.ToString());
                    sb.Append(" ");
                    sb.Append(gt.Y.ToString());
                }
            }
            Color pointColor = Color.Red;


            string command = "CAL," + Earth3d.MainWindow.Config.ClusterID.ToString() + "," + pe.ID + ",GEOMETRY," + GeometryStyles.Points.ToString() + "," + pe.SelectedGroundTruth.ToString() + "," + SavedColor.Save(Color.White) + "," + SavedColor.Save(pointColor) + "," + sb.ToString();
            NetControl.SendCommand(command);

        }

        private void SendEdgePointEditUpdate(Edge edge)
        {
            StringBuilder sbLeft = new StringBuilder();
            StringBuilder sbRight = new StringBuilder();

            bool first = true;
            foreach (EdgePoint ep in edge.Points)
            {
                if (!first)
                {
                    sbLeft.Append(";");
                    sbRight.Append(";");
                }
                else
                {
                    first = false;
                }
                sbLeft.Append(ep.Left.X.ToString());
                sbLeft.Append(" ");
                sbLeft.Append(ep.Left.Y.ToString());

                sbRight.Append(ep.Right.X.ToString());
                sbRight.Append(" ");
                sbRight.Append(ep.Right.Y.ToString());


            }
            Color pointColor = Color.Red;


            string commandLeft = "CAL," + Earth3d.MainWindow.Config.ClusterID.ToString() + "," + edge.Left.ToString() + ",GEOMETRY," + GeometryStyles.Points.ToString() + "," + edge.SelectedPoint.ToString() + "," + SavedColor.Save(Color.White) + "," + SavedColor.Save(Color.Red) + "," + sbLeft.ToString();
            NetControl.SendCommand(commandLeft);
            string commandRight = "CAL," + Earth3d.MainWindow.Config.ClusterID.ToString() + "," + edge.Right.ToString() + ",GEOMETRY," + GeometryStyles.Points.ToString() + "," + edge.SelectedPoint.ToString() + "," + SavedColor.Save(Color.White) + "," + SavedColor.Save(Color.Green) + "," + sbRight.ToString();
            NetControl.SendCommand(commandRight);

        }




        private void wwtCheckbox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void PointsTreeContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (PointTree.SelectedNode != null)
            {
                bool move = true; ;

                if (PointTree.SelectedNode.Tag is Edge || PointTree.SelectedNode.Tag is ProjectorEntry)
                {
                    move = false;
                }

                moveDownToolStripMenuItem.Visible = move;
                moveUpToolStripMenuItem.Visible = move;
                toolStripSeparator1.Visible = move;
                deleteToolStripMenuItem.Enabled = true;
                if (PointTree.SelectedNode.Tag is EdgePoint)
                {
                    blendPointToolStripMenuItem.Visible = true;
                    EdgePoint ep = PointTree.SelectedNode.Tag as EdgePoint;
                    blendPointToolStripMenuItem.Checked = ep.Blend;
                    blendPointToolStripMenuItem.Visible = true;
                }
                else
                {
                    blendPointToolStripMenuItem.Visible = false;
                }

                if (PointTree.SelectedNode.Tag is ProjectorEntry && !Align.Selected)
                {
                    transferFromEdgesToolStripMenuItem.Visible = true;
                }
                else
                {
                    transferFromEdgesToolStripMenuItem.Visible = false;
                }

                if (PointTree.SelectedNode.Tag is GroundTruthPoint)
                {
                    contextSeperator2.Visible = true;
                    propertiesToolStripMenuItem.Visible = true;
                }
                else
                {
                    contextSeperator2.Visible = false;
                    propertiesToolStripMenuItem.Visible = false;
                }
            }
        }
        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = 0;
            int nodeIndex = 0;
            TreeNode node = PointTree.SelectedNode;
            if (Align.Selected)
            {
                if (PointTree.SelectedNode != null)
                {
                    if (PointTree.SelectedNode.Tag is GroundTruthPoint)
                    {
                        ProjectorEntry pe = PointTree.SelectedNode.Parent.Tag as ProjectorEntry;
                        GroundTruthPoint pnt = (GroundTruthPoint)PointTree.SelectedNode.Tag;
                        index = pe.Constraints.IndexOf(pnt);
                        if (index > 0)
                        {
                            pe.Constraints.Remove(pnt);
                            pe.Constraints.Insert(index - 1, pnt);
                            TreeNode parentNode = PointTree.SelectedNode.Parent;
                            nodeIndex = parentNode.Nodes.IndexOf(node);
                            node.Remove();
                            parentNode.Nodes.Insert(nodeIndex - 1, node);
                            PointTree.SelectedNode = node;
                        }
                    }
                    else if (PointTree.SelectedNode.Tag is EdgePoint)
                    {
                        Edge edge = PointTree.SelectedNode.Parent.Tag as Edge;
                        EdgePoint pnt = (EdgePoint)PointTree.SelectedNode.Tag;

                        index = edge.Points.IndexOf(pnt);
                        if (index > 0)
                        {
                            edge.Points.Remove(pnt);
                            edge.Points.Insert(index - 1, pnt);
                            TreeNode parentNode = PointTree.SelectedNode.Parent;
                            nodeIndex = parentNode.Nodes.IndexOf(node);
                            node.Remove();
                            parentNode.Nodes.Insert(nodeIndex - 1, node);
                            PointTree.SelectedNode = node;
                        }

                    }

                }
            }
            else
            {
                if (PointTree.SelectedNode != null)
                {
                    if (PointTree.SelectedNode.Tag is BlendPoint)
                    {
                        ProjectorEntry pe = PointTree.SelectedNode.Parent.Tag as ProjectorEntry;
                        BlendPoint pnt = (BlendPoint)PointTree.SelectedNode.Tag;
                        index = pe.BlendPolygon.IndexOf(pnt);
                        if (index > 0)
                        {
                            pe.BlendPolygon.Remove(pnt);
                            pe.BlendPolygon.Insert(index - 1, pnt);
                            TreeNode parentNode = PointTree.SelectedNode.Parent;
                            nodeIndex = parentNode.Nodes.IndexOf(node);
                            node.Remove();
                            parentNode.Nodes.Insert(nodeIndex - 1, node);
                            PointTree.SelectedNode = node;
                        }
                    }

                }
            }
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = 0;
            int nodeIndex = 0;
            TreeNode node = PointTree.SelectedNode;
            if (Align.Selected)
            {
                if (PointTree.SelectedNode != null)
                {
                    if (PointTree.SelectedNode.Tag is GroundTruthPoint)
                    {
                        ProjectorEntry pe = PointTree.SelectedNode.Parent.Tag as ProjectorEntry;
                        GroundTruthPoint pnt = (GroundTruthPoint)PointTree.SelectedNode.Tag;
                        index = pe.Constraints.IndexOf(pnt);
                        if (index < pe.Constraints.Count - 1)
                        {
                            pe.Constraints.Remove(pnt);
                            pe.Constraints.Insert(index + 1, pnt);
                            TreeNode parentNode = PointTree.SelectedNode.Parent;
                            nodeIndex = parentNode.Nodes.IndexOf(node);
                            node.Remove();
                            parentNode.Nodes.Insert(nodeIndex + 1, node);
                            PointTree.SelectedNode = node;
                        }
                    }
                    else if (PointTree.SelectedNode.Tag is EdgePoint)
                    {
                        Edge edge = PointTree.SelectedNode.Parent.Tag as Edge;
                        EdgePoint pnt = (EdgePoint)PointTree.SelectedNode.Tag;

                        index = edge.Points.IndexOf(pnt);
                        if (index < edge.Points.Count - 1)
                        {
                            edge.Points.Remove(pnt);
                            edge.Points.Insert(index + 1, pnt);
                            TreeNode parentNode = PointTree.SelectedNode.Parent;
                            nodeIndex = parentNode.Nodes.IndexOf(node);
                            node.Remove();
                            parentNode.Nodes.Insert(nodeIndex + 1, node);
                            PointTree.SelectedNode = node;
                        }

                    }

                }
            }
            else
            {
                if (PointTree.SelectedNode != null)
                {
                    if (PointTree.SelectedNode.Tag is BlendPoint)
                    {
                        ProjectorEntry pe = PointTree.SelectedNode.Parent.Tag as ProjectorEntry;
                        BlendPoint pnt = (BlendPoint)PointTree.SelectedNode.Tag;
                        index = pe.BlendPolygon.IndexOf(pnt);
                        if (index < pe.BlendPolygon.Count - 1)
                        {
                            pe.BlendPolygon.Remove(pnt);
                            pe.BlendPolygon.Insert(index + 1, pnt);
                            TreeNode parentNode = PointTree.SelectedNode.Parent;
                            nodeIndex = parentNode.Nodes.IndexOf(node);
                            node.Remove();
                            parentNode.Nodes.Insert(nodeIndex + 1, node);
                            PointTree.SelectedNode = node;
                        }
                    }

                }
            }
        }

        private void transferFromEdgesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (UiTools.ShowMessageBox(Language.GetLocalizedText(699, "This will clear existing blend polygons for this projector and create new ones by trasfering edge points from align. This operation can not be undone, are you sure you want to proceed?"), Language.GetLocalizedText(700, "Transfer from Edges"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                ProjectorEntry pe = PointTree.SelectedNode.Tag as ProjectorEntry;
                pe.BlendPolygon.Clear();
                TransferEdges(pe);
                ReloadPointTree();
            }
        }

        private void TransferEdges(ProjectorEntry pe)
        {
            SortedList<double, BlendPoint> pointList = new SortedList<double, BlendPoint>();

            foreach (Edge edge in CalibrationInfo.Edges)
            {
                bool right = false;
                if (edge.Right == pe.ID)
                {
                    right = true;
                }
                else if (edge.Left != pe.ID)
                {
                    continue;
                }

                foreach (EdgePoint edgePoint in edge.Points)
                {
                    GroundTruthPoint pnt;
                    if (!edgePoint.Blend)
                    {
                        continue;
                    }

                    if (right)
                    {
                        pnt = edgePoint.Right;
                    }
                    else
                    {
                        pnt = edgePoint.Left;
                    }

                    BlendPoint blend = new BlendPoint();
                    blend.X = pnt.X;
                    blend.Y = pnt.Y;
                    double angle = Math.Atan2(pnt.X - (pe.Width / 2), pnt.Y - (pe.Height / 2));
                    if (!pointList.ContainsKey(angle))
                    {
                        pointList.Add(angle, blend);
                    }
                }
            }

            foreach (BlendPoint blend in pointList.Values)
            {
                pe.BlendPolygon.Add(blend);
            }
        }

        private void blendPointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EdgePoint ep = PointTree.SelectedNode.Tag as EdgePoint;
            ep.Blend = blendPointToolStripMenuItem.Checked = !ep.Blend;
            RefreshTreeeNames(PointTree.Nodes);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PointTree.SelectedNode.Tag is GroundTruthPoint)
            {
                GroundTruthPointProperties props = new GroundTruthPointProperties();

                GroundTruthPoint pnt = (GroundTruthPoint)PointTree.SelectedNode.Tag;
                props.Target = pnt;
                if (props.ShowDialog() == DialogResult.OK)
                {
                }
                RefreshTreeeNames(PointTree.Nodes);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PointTree.SelectedNode != null)
            {
                if (PointTree.SelectedNode.Tag is GroundTruthPoint)
                {
                    ProjectorEntry pe = PointTree.SelectedNode.Parent.Tag as ProjectorEntry;
                    GroundTruthPoint pnt = (GroundTruthPoint)PointTree.SelectedNode.Tag;
                    pe.Constraints.Remove(pnt);
                    PointTree.SelectedNode.Remove();
                }
                else if (PointTree.SelectedNode.Tag is BlendPoint)
                {
                    ProjectorEntry pe = PointTree.SelectedNode.Parent.Tag as ProjectorEntry;
                    BlendPoint pnt = (BlendPoint)PointTree.SelectedNode.Tag;
                    pe.BlendPolygon.Remove(pnt);
                    PointTree.SelectedNode.Remove();
                }
                else if (PointTree.SelectedNode.Tag is EdgePoint)
                {
                    Edge edge = PointTree.SelectedNode.Parent.Tag as Edge;
                    EdgePoint pnt = (EdgePoint)PointTree.SelectedNode.Tag;
                    edge.Points.Remove(pnt);
                    PointTree.SelectedNode.Remove();
                }
                else if (PointTree.SelectedNode.Tag is Edge)
                {
                    if (UiTools.ShowMessageBox(Language.GetLocalizedText(738, "Are you sure you want to remove this edge and all it's points? This operation can not be undone."), Language.GetLocalizedText(739, "Delete Edge"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CalibrationInfo.Edges.Remove((Edge)PointTree.SelectedNode.Tag);
                        PointTree.SelectedNode.Nodes.Clear();
                        PointTree.SelectedNode.Remove();
                    }
                }
                else if (PointTree.SelectedNode.Tag is ProjectorEntry)
                {
                    if (Align.Selected)
                    {
                        if (UiTools.ShowMessageBox(Language.GetLocalizedText(740, "Are you sure you want to remove all constraints from this projector? This operation can not be undone."), Language.GetLocalizedText(741, "Delete Constraints"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ((ProjectorEntry)PointTree.SelectedNode.Tag).Constraints.Clear();
                            PointTree.SelectedNode.Nodes.Clear();
                        }
                    }
                    else
                    {
                        if (UiTools.ShowMessageBox(Language.GetLocalizedText(742, "Are you sure you want to remove all points from the blend mask? This operation can not be undone."), Language.GetLocalizedText(739, "Delete Edge"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ((ProjectorEntry)PointTree.SelectedNode.Tag).BlendPolygon.Clear();
                            PointTree.SelectedNode.Nodes.Clear();
                        }
                    }
                }
            }
            MousePad.Invalidate();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddPointToTree();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        bool running = false;

        private void SolveDistortion_Click(object sender, EventArgs e)
        {
            if (!running)
            {
                running = true;
                SolveDistortion.Text = Language.GetLocalizedText(1177, "Stop Solving");
                SolveAlignment sa = new SolveAlignment();
                sa.SetupSolve(CalibrationInfo, useConstraints.Checked, UseRadial.Checked);

                solveProjectors = sa.projectors;

                for (int i = 0; i < 2550; i++)
                {
                    if (!running)
                    {
                        break;
                    }
                    try
                    {
                        double error = sa.Iterate();
                        AverageError.Text = error.ToString();
                    }
                    catch
                    {
                        UiTools.ShowMessageBox(Language.GetLocalizedText(743, "The current Solution can't be improved on by further iteration with from the current alignment. Either reset the initial estimated parameters for the projectors, or this will be the best alignment achievable."), Language.GetLocalizedText(744, "Alignment Solver"));
                        running = false;
                    }
                    MousePad.Refresh();

                    Application.DoEvents();

                }

                for (int x = 0; x < CalibrationInfo.Projectors.Count; x++)
                {
                    CalibrationInfo.Projectors[x].SolvedProjection = solveProjectors[x].Projection;
                    CalibrationInfo.Projectors[x].SolvedTransform = solveProjectors[x].Transform;
                }

                solveProjectors = new List<SolveProjector>();
                SolveDistortion.Text = Language.GetLocalizedText(754, "Solve Distortion");

                if (UiTools.ShowMessageBox(Language.GetLocalizedText(745, "Would you like to use the solved Projector parameters? You can always use the 'Copy Solved' button in the Projector Edit to do this manually."), Language.GetLocalizedText(746, "Use Solved Parameters as Projector Defaults"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
                    {
                        pe.ProjectorProjection = pe.SolvedProjection.Copy();
                        pe.ProjectorTransform = pe.SolvedTransform.Copy();

                    }
                    MousePad.Invalidate();
                }

                running = false;
            }
            else
            {
                SolveDistortion.Text = Language.GetLocalizedText(754, "Solve Distortion");
                running = false;
            }
        }



        private void DomeRadius_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;

            CalibrationInfo.DomeSize = UiTools.ParseAndValidateDouble(DomeRadius, CalibrationInfo.DomeSize, ref failed);

        }

        private void ProjectorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProjectorEntry pe = (ProjectorEntry)ProjectorList.SelectedItem;

            SolveProjector sp = new SolveProjector(pe, CalibrationInfo.DomeSize, CalibrationInfo.ScreenType == ScreenTypes.FishEye ? ProjectionType.FishEye : ProjectionType.Projector, ScreenTypes.Spherical, SolveParameters.Default);

            Vector2d pnt = sp.GetCoordinatesForScreenPoint(pe.Width / 2, pe.Height / 2);
            Vector2d pnt2 = sp.GetCoordinatesForScreenPoint(pe.Width / 2 + 1, pe.Height / 2);
            Vector2d pntCamera = sp.GetCoordinatesForProjector();


            PointF pntRayDirection = GetPointFromAltAz(new PointF((float)pnt.X + 90, (float)pnt.Y));
            //PointF pntProj = GetPointFromXYZ(new Vector3d(pe.ProjectorTransform.X,pe.ProjectorTransform.Y,pe.ProjectorTransform.Z));
            PointF pntProj = GetPointFromAltAz(new PointF((float)pntCamera.X + 90, (float)pntCamera.Y));

            Refresh();
            MousePad.Refresh();
            Graphics g = MousePad.CreateGraphics();

            g.DrawLine(Pens.Yellow, pntRayDirection, pntProj);
            g.DrawRectangle(Pens.Red, pntProj.X - 5, pntProj.Y - 5, 10, 10);

            g.Dispose();


        }

        private void MakeWarpMaps_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            if (CalibrationInfo.ScreenType == ScreenTypes.FishEye)
            {
                GenerateFisheyeWarpMaps();
            }
            else
            {
                GenerateWarpMaps();
            }
            this.Enabled = true;
            this.Activate();

        }

        private void GenerateWarpMaps()
        {
            ProgressPopup.Show(this, Language.GetLocalizedText(747, "Building Maps"), Language.GetLocalizedText(748, "Building Warp Maps"));

            string path = String.Format("{0}\\ProjetorWarpMaps", Properties.Settings.Default.CahceDirectory);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            int index = 0;

            foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
            {
                List<GroundTruthPoint> gtPoints = new List<GroundTruthPoint>();
                //foreach (GroundTruthPoint gt in pe.Constraints)
                //{
                //    gtPoints.Add(gt);
                //}
                foreach (Edge edge in CalibrationInfo.Edges)
                {
                    bool isRight = edge.Right == pe.ID;
                    if (!isRight && edge.Left != pe.ID)
                    {
                        continue;
                    }
                    SolveProjector spRight = GetSolveProjector(edge.Right);

                    SolveProjector spLeft = GetSolveProjector(edge.Left);

                    foreach (EdgePoint ep in edge.Points)
                    {
                        Vector2d pntLeft = spLeft.GetCoordinatesForScreenPoint(ep.Left.X, ep.Left.Y);
                        Vector2d pntRight = spRight.GetCoordinatesForScreenPoint(ep.Right.X, ep.Right.Y);
                        Vector2d pntAvg = Vector2d.Average3d(pntLeft, pntRight);

                        GroundTruthPoint gt = new GroundTruthPoint();
                        gt.X = isRight ? ep.Right.X : ep.Left.X;
                        gt.Y = isRight ? ep.Right.Y : ep.Left.Y;
                        gt.AxisType = AxisTypes.Both;
                        gt.Az = pntAvg.X;
                        gt.Alt = pntAvg.Y;
                        gtPoints.Add(gt);
                    }

                }

                Bitmap bmp = WarpMapper.MakeWarpMap(pe, CalibrationInfo.DomeSize, UseRadial.Checked, gtPoints, (CalibrationInfo.ScreenType == ScreenTypes.Spherical) ? ScreenTypes.Spherical : ScreenTypes.Cylindrical);
                bmp.Save(string.Format("{0}\\distort_{1}.png", path, pe.Name.Replace(" ", "_")));
                bmp.Dispose();
                index++;
                ProgressPopup.SetProgress(index * 100 / CalibrationInfo.Projectors.Count, string.Format(Language.GetLocalizedText(749, "Warp") + " {0} / {1}.", index, CalibrationInfo.Projectors.Count));
            }
            ProgressPopup.Done();

        }

        private void GenerateFisheyeWarpMaps()
        {
            ProgressPopup.Show(this, Language.GetLocalizedText(747, "Building Maps"), Language.GetLocalizedText(748, "Building Warp Maps"));

            string path = String.Format("{0}\\ProjetorWarpMaps", Properties.Settings.Default.CahceDirectory);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            int index = 0;

            foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
            {
                SolveProjector sp = GetSolveProjector(pe.ID);

                StreamWriter sw = new StreamWriter(string.Format("{0}\\distort_{1}.data", path, pe.Name.Replace(" ", "_")));

                int stepsX = 100;
                int stepsY = 100;
                int count = stepsX * stepsY;
                int total = 0;

                sw.WriteLine("2");
                sw.WriteLine(string.Format("{0} {1}", stepsX, stepsY));

                double stepSizeX = (double)pe.Width / (stepsX - 1);
                double stepSizeY = (double)pe.Height / (stepsY - 1);

                for (double y = 0; (int)(y + .5) <= pe.Height; y += stepSizeY)
                {
                    for (double x = 0; (int)(x + .5) <= pe.Width; x += stepSizeX)
                    {
                        Vector2d altAz = sp.GetCoordinatesForScreenPoint(x, y);
                        Vector2d pnt = GetPointFromAltAz(altAz);
                        float color = 255;
                        if (altAz.Y < 0)
                        {
                            color = 0;
                        }
                        else
                        {
                            color = 1;
                        }

                        sw.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", -((x - pe.Width / 2) / (pe.Height / 2)), -((y - pe.Height / 2) / (pe.Height / 2)), pnt.X, pnt.Y, color));
                        total++;
                    }
                }

                sw.Close();
                index++;
                ProgressPopup.SetProgress(index * 100 / CalibrationInfo.Projectors.Count, string.Format(Language.GetLocalizedText(749, "Warp") + " {0} / {1}.", index, CalibrationInfo.Projectors.Count));

            }
            ProgressPopup.Done();

        }


        Vector2d GetPointFromAltAz(Vector2d point)
        {
            Vector2d retPoint = new Vector2d();
            // point.X += 90;
            retPoint.X = .5 + Math.Cos(point.X / 180 * Math.PI) * ((90 - point.Y) / 90) * .5;
            retPoint.Y = .5 + Math.Sin(point.X / 180 * Math.PI) * ((90 - point.Y) / 90) * .5;
            return retPoint;
        }


        private void SendNewMaps_Click(object sender, EventArgs e)
        {
            if (UiTools.ShowMessageBox(Language.GetLocalizedText(750, "Would you like to build new maps with the current parameters? Answering 'No' will send the exsiting maps"), Language.GetLocalizedText(751, "Build new Maps"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                ProgressPopup.Show(this, Language.GetLocalizedText(747, "Building Maps"), Language.GetLocalizedText(748, "Building Warp Maps"));
                if (CalibrationInfo.ScreenType == ScreenTypes.FishEye)
                {
                    GenerateFisheyeWarpMaps();
                }
                else
                {
                    GenerateWarpMaps();
                    ProgressPopup.SetProgress(50, Language.GetLocalizedText(752, "Building Blend Maps"));
                    MakeBlendMaps();
                }

                ProgressPopup.Done();
                this.Cursor = Cursors.Default;
                this.Enabled = true;
            }

            if (CalibrationInfo.ScreenType != ScreenTypes.FishEye)
            {
                foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
                {
                    SendViewConfig(pe.ID, pe, CalibrationInfo.DomeTilt);
                }
            }

            string command = "CAL," + Earth3d.MainWindow.Config.ClusterID.ToString() + ",-1,RELOADWARPS";
            NetControl.SendCommand(command);
            this.Activate();

        }

        private void MakeBlendMap_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            MakeBlendMaps();
            this.Enabled = true;
            this.Activate();
        }

        private void MakeBlendMaps()
        {
            ProgressPopup.Show(this, Language.GetLocalizedText(747, "Building Maps"), Language.GetLocalizedText(752, "Building Blend Maps"));

            string path = String.Format("{0}\\ProjetorWarpMaps", Properties.Settings.Default.CahceDirectory);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            int index = 0;
            foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
            {
                Bitmap bmp = WarpMapper.MakeBlendMap(pe, CalibrationInfo.BlendMarkBlurAmount, CalibrationInfo.BlendMarkBlurIterations, (float)CalibrationInfo.BlendGamma);
                bmp.Save(string.Format("{0}\\blend_{1}.png", path, pe.Name.Replace(" ", "_")));
                bmp.Dispose();
                index++;
                ProgressPopup.SetProgress(index * 100 / CalibrationInfo.Projectors.Count, string.Format(Language.GetLocalizedText(753, "Building Blend") + "{0} / {1}.", index, CalibrationInfo.Projectors.Count));
            }

            ProgressPopup.Done();
        }

        private void BlurSize_ValueChanged(object sender, EventArgs e)
        {
            CalibrationInfo.BlendMarkBlurAmount = BlurSize.Value * 2 + 1;
            BlurSizeText.Text = CalibrationInfo.BlendMarkBlurAmount.ToString();
        }

        private void BlurIterations_ValueChanged(object sender, EventArgs e)
        {
            CalibrationInfo.BlendMarkBlurIterations = BlurIterations.Value;
            blurIterationsText.Text = CalibrationInfo.BlendMarkBlurIterations.ToString();
        }

        private void domeTilt_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;

            CalibrationInfo.DomeTilt = UiTools.ParseAndValidateDouble(domeTilt, CalibrationInfo.DomeTilt, ref failed);

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void GammaText_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;

            CalibrationInfo.BlendGamma = UiTools.ParseAndValidateDouble(GammaText, CalibrationInfo.BlendGamma, ref failed);

        }

        private void useConstraints_CheckedChanged(object sender, EventArgs e)
        {
            CalibrationInfo.UseConstraints = useConstraints.Checked;
        }



        private void wwtButton1_Click(object sender, EventArgs e)
        {
            string command = "CAL," + Earth3d.MainWindow.Config.ClusterID.ToString() + ",-1,UPDATESOFTWARE";
            NetControl.SendCommand(command);
        }

        private void PointTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                PointTree.SelectedNode = PointTree.GetNodeAt(e.X, e.Y);
            }
        }

        private void TansferFromEdges_Click(object sender, EventArgs e)
        {
            if (UiTools.ShowMessageBox(Language.GetLocalizedText(756, "This will clear all existing blend polygons for all projectors and create new ones by trasfering edge points from align. This operation can not be undone, are you sure you want to proceed?"), Language.GetLocalizedText(700, "Transfer from Edges"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (ProjectorEntry pe in CalibrationInfo.Projectors)
                {
                    pe.BlendPolygon.Clear();
                    TransferEdges(pe);
                }
                ReloadPointTree();
            }
        }

        private void screenType_SelectionChanged(object sender, EventArgs e)
        {
            CalibrationInfo.ScreenType = (ScreenTypes)screenType.SelectedIndex;

            switch (CalibrationInfo.ScreenType)
            {
                case ScreenTypes.Spherical:
                case ScreenTypes.Cylindrical:
                case ScreenTypes.Flat:
                    Blend.Enabled = true;
                    break;
                case ScreenTypes.FishEye:
                    Blend.Enabled = false;
                    SetAlignBlendMode(true);
                    break;
                default:
                    break;
            }

        }

        private void solveFOV_CheckedChanged(object sender, EventArgs e)
        {
            if (solveFOV.Checked)
            {
                CalibrationInfo.SolveParameters |= (int)SolveParameters.Fov;
            }
            else
            {
                CalibrationInfo.SolveParameters &= ~(int)SolveParameters.Fov;
            }
        }
        private void solveAspect_CheckedChanged(object sender, EventArgs e)
        {
            if (solveAspect.Checked)
            {
                CalibrationInfo.SolveParameters |= (int)SolveParameters.Aspect;
            }
            else
            {
                CalibrationInfo.SolveParameters &= ~(int)SolveParameters.Aspect;
            }
        }
        private void solvePitch_CheckedChanged(object sender, EventArgs e)
        {
            if (solvePitch.Checked)
            {
                CalibrationInfo.SolveParameters |= (int)SolveParameters.Pitch;
            }
            else
            {
                CalibrationInfo.SolveParameters &= ~(int)SolveParameters.Pitch;
            }
        }
        private void solveHeading_CheckedChanged(object sender, EventArgs e)
        {
            if (solveHeading.Checked)
            {
                CalibrationInfo.SolveParameters |= (int)SolveParameters.Heading;
            }
            else
            {
                CalibrationInfo.SolveParameters &= ~(int)SolveParameters.Heading;
            }
        }

        private void solveRoll_CheckedChanged(object sender, EventArgs e)
        {
            if (solveRoll.Checked)
            {
                CalibrationInfo.SolveParameters |= (int)SolveParameters.Roll;
            }
            else
            {
                CalibrationInfo.SolveParameters &= ~(int)SolveParameters.Roll;
            }
        }



        private void solveX_CheckedChanged(object sender, EventArgs e)
        {
            if (solveX.Checked)
            {
                CalibrationInfo.SolveParameters |= (int)SolveParameters.X;
            }
            else
            {
                CalibrationInfo.SolveParameters &= ~(int)SolveParameters.X;
            }
        }

        private void solveY_CheckedChanged(object sender, EventArgs e)
        {
            if (solveY.Checked)
            {
                CalibrationInfo.SolveParameters |= (int)SolveParameters.Y;
            }
            else
            {
                CalibrationInfo.SolveParameters &= ~(int)SolveParameters.Y;
            }
        }

        private void solveZ_CheckedChanged(object sender, EventArgs e)
        {
            if (solveZ.Checked)
            {
                CalibrationInfo.SolveParameters |= (int)SolveParameters.Z;
            }
            else
            {
                CalibrationInfo.SolveParameters &= ~(int)SolveParameters.Z;
            }
        }

        private void UseRadial_CheckedChanged(object sender, EventArgs e)
        {
            CalibrationInfo.SolveRadialDistortion = UseRadial.Checked;
            if (UseRadial.Checked)
            {
                CalibrationInfo.SolveParameters |= (int)SolveParameters.Radial;
            }
            else
            {
                CalibrationInfo.SolveParameters &= ~(int)SolveParameters.Radial;
            }
        }

        private void redSlider_ValueChanged(object sender, EventArgs e)
        {
            redAmount.Text = (155 + redSlider.Value).ToString();
            if (colorCorrectTarget != null)
            {
                colorCorrectTarget.WhiteBalance.Red = 155 + redSlider.Value;
                SendBlendPointEditUpdate(colorCorrectTarget);
            }
        }

        private void greenSlider_ValueChanged(object sender, EventArgs e)
        {
            greenAmount.Text = (155 + greenSlider.Value).ToString();
            if (colorCorrectTarget != null)
            {
                colorCorrectTarget.WhiteBalance.Green = 155 + greenSlider.Value;
                SendBlendPointEditUpdate(colorCorrectTarget);
            }
        }

        private void blueSlider_ValueChanged(object sender, EventArgs e)
        {
            blueAmount.Text = (155 + blueSlider.Value).ToString();
            if (colorCorrectTarget != null)
            {
                colorCorrectTarget.WhiteBalance.Blue = 155 + blueSlider.Value;
                SendBlendPointEditUpdate(colorCorrectTarget);
            }
        }

        private void ScalableCalibration_Click(object sender, EventArgs e)
        {

        }

        private void pattern_SelectionChanged(object sender, EventArgs e)
        {
            calibrationImageType = (CalibrationImageType)pattern.SelectedIndex;
        }


        private void paternServer_SelectionChanged(object sender, EventArgs e)
        {
            ProjectorEntry pe = ProjectorServerPaternPiicker.SelectedItem as ProjectorEntry;

            if (pe != null)
            {
                CalibrationProjectorTarget = pe.ID;
            }
            else
            {
                CalibrationProjectorTarget = -1;
            }
        }

        private void wwtButton2_Click(object sender, EventArgs e)
        {

            //CalibrationInfo.DomeSize
            //UseRadial.Checked,
            //gtPoints,
            //(CalibrationInfo.ScreenType == ScreenTypes.Spherical) ? ScreenTypes.Spherical : ScreenTypes.Cylindrical

            ProjectorEntry pe = ProjectorServerPaternPiicker.SelectedItem as ProjectorEntry;

            if (pe == null)
            {
                return;
            }

            float min = 3200;
            circle = min;
            radius = (min / 2) * .95f;
            center = new PointF(3200 / 2, 3200 / 2);
            float left = center.X - min / 2;
            float top = center.Y - min / 2;

            bmpCenter = new PointF(center.X - left, center.Y - top);


            Bitmap bmp = new Bitmap((int)circle, (int)circle);

            SolveProjector spProjector = new SolveProjector(pe, CalibrationInfo.DomeSize, ProjectionType.Projector, ScreenTypes.Spherical, SolveParameters.Default);
            spProjector.RadialDistorion = false;

            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.Black);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            SolidBrush domeBackground = new SolidBrush(Color.FromArgb(15, 15, 15));

            g.FillEllipse(domeBackground, new RectangleF((circle / 2) - radius, (circle / 2) - radius, radius * 2, radius * 2));
            domeBackground.Dispose();
            // DrawGrid(g, spProjector, 50, true);

            switch (this.calibrationImageType)
            {
                case CalibrationImageType.Bkack:
                    break;
                case CalibrationImageType.White:
                    //todo make a warped rect in the dome master space
                    DrawOutline(g, spProjector, pe.Width, pe.Height);
                    break;
                case CalibrationImageType.Squares:
                    DrawGrid(g, spProjector, 50, true, pe.Width, pe.Height, pe.ID);
                    break;
                case CalibrationImageType.SmallSquares:
                    DrawGrid(g, spProjector, 25, true, pe.Width, pe.Height, pe.ID);
                    break;
                case CalibrationImageType.Points:
                    DrawGrid(g, spProjector, 50, false, pe.Width, pe.Height, pe.ID);
                    break;
                case CalibrationImageType.DensePoints:
                    DrawGrid(g, spProjector, 25, false, pe.Width, pe.Height, pe.ID);
                    break;
                case CalibrationImageType.Checkerboard:
                    break;
                default:
                    break;
            }

            g.Flush();
            g.Dispose();

            bmp.Save("c:\\temp\\proj" + pe.Name + "_" + calibrationImageType.ToString() + ".png");
            MousePad.Image = bmp;
            MakeRegionRects();
        }

        private void DrawOutline(Graphics g, SolveProjector sp, int width, int height)
        {
            List<PointF> points = new List<PointF>();

            for (int x = 0; x < width; x++)
            {
                points.Add(WarpPoint(sp, new PointF(x, 0)));
            }
            for (int y = 0; y < height; y++)
            {
                points.Add(WarpPoint(sp, new PointF(width - 1, y)));
            }

            for (int x = width - 1; x > -1; x--)
            {
                points.Add(WarpPoint(sp, new PointF(x, height - 1)));
            }
            for (int y = height; y > -1; y--)
            {
                points.Add(WarpPoint(sp, new PointF(0, y)));
            }

            g.FillPolygon(Brushes.White, points.ToArray());
            g.DrawPolygon(Pens.White, points.ToArray());

        }

        private void DrawGrid(Graphics g, SolveProjector sp, int size, bool squares, int width, int height, int id)
        {

            int countX = width / (size);
            int countY = height / (size);
            int skip = 1;

            if (squares)
            {
                skip = 2;
            }
            else
            {
                countY++;
            }

            Brush white = Brushes.White;
            Brush red = Brushes.Red;
            Brush blue = Brushes.Blue;
            Brush green = Brushes.Green;

            for (int y = 0; y < countY; y += skip)
            {
                for (int x = 0; x < countX; x += skip)
                {
                    if (squares)
                    {
                        bool thisBit = false;

                        DrawWarpedRect(g, sp, new RectangleF(x * size + 35.5f, y * size + 15.5f, size, size), white, Pens.White);
                        //g.DrawRectangle(Pens.White, x * size + 35.5f, y * size + 15.5f, size, size);

                        //g.FillRectangle(white, new RectangleF(x * size + 35.5f, y * size + 15.5f, size, size));

                        if (x == 18 && y == 10)
                        {
                            PointF pnt = WarpPoint(sp, new PointF(x * size + 35.5f + (size / 2), y * size + 15.5f + (size / 2)));
                            g.FillEllipse(red, new RectangleF(pnt.X - 5, pnt.Y - 5, 10, 10));
                        }

                        if (x == 18 && y == 8)
                        {
                            PointF pnt = WarpPoint(sp, new PointF(x * size + 35.5f + (size / 2), y * size + 15.5f + (size / 2)));
                            g.FillEllipse(blue, new RectangleF(pnt.X - 5, pnt.Y - 5, 10, 10));
                        }

                        if (x == 16 && y == 10)
                        {
                            PointF pnt = WarpPoint(sp, new PointF(x * size + 35.5f + (size / 2), y * size + 15.5f + (size / 2)));
                            g.FillEllipse(green, new RectangleF(pnt.X - 5, pnt.Y - 5, 10, 10));
                        }

                        // Bits of the Projector ID
                        if (x == 14 && y == 12)
                        {
                            thisBit = (Earth3d.MainWindow.Config.NodeID & 1) == 1;
                            PointF pnt = WarpPoint(sp, new PointF(x * size + 35.5f + (size / 2), y * size + 15.5f + (size / 2)));
                            g.FillEllipse(thisBit ? Brushes.Yellow : Brushes.Cyan, new RectangleF(pnt.X - 5, pnt.Y - 5, 10, 10));
                        }

                        if (x == 16 && y == 12)
                        {
                            thisBit = (id & 2) == 2;
                            PointF pnt = WarpPoint(sp, new PointF(x * size + 35.5f + (size / 2), y * size + 15.5f + (size / 2)));
                            g.FillEllipse(thisBit ? Brushes.Yellow : Brushes.Cyan, new RectangleF(pnt.X - 5, pnt.Y - 5, 10, 10));
                        }

                        if (x == 18 && y == 12)
                        {
                            thisBit = (id & 4) == 4;
                            PointF pnt = WarpPoint(sp, new PointF(x * size + 35.5f + (size / 2), y * size + 15.5f + (size / 2)));
                            g.FillEllipse(thisBit ? Brushes.Yellow : Brushes.Cyan, new RectangleF(pnt.X - 5, pnt.Y - 5, 10, 10));
                        }

                        if (x == 20 && y == 12)
                        {
                            thisBit = (id & 8) == 8;
                            PointF pnt = WarpPoint(sp, new PointF(x * size + 35.5f + (size / 2), y * size + 15.5f + (size / 2)));
                            g.FillEllipse(thisBit ? Brushes.Yellow : Brushes.Cyan, new RectangleF(pnt.X - 5, pnt.Y - 5, 10, 10));
                        }

                        if (x == 22 && y == 12)
                        {
                            thisBit = (id & 16) == 16;
                            PointF pnt = WarpPoint(sp, new PointF(x * size + 35.5f + (size / 2), y * size + 15.5f + (size / 2)));
                            g.FillEllipse(thisBit ? Brushes.Yellow : Brushes.Cyan, new RectangleF(pnt.X - 5, pnt.Y - 5, 10, 10));
                        }

                    }
                    else
                    {
                        PointF pnt = WarpPoint(sp, new PointF(x * size + 35.5f, y * size + 15.5f));
                        PointF pntB = WarpPoint(sp, new PointF((x + 1) * size + 35.5f, (y + 1) * size + 15.5f));
                        PointF pntD = PointF.Subtract(pnt, new SizeF(pntB));
                        double dist = Math.Sqrt(pntD.X * pntD.X + pntD.Y * pntD.Y) / 70.7;
                        float pntSz = (float)(10 * dist);
                        g.FillEllipse(white, new RectangleF(pnt.X - 5, pnt.Y - 5, pntSz, pntSz));
                    }

                }
            }


        }

        void DrawWarpedRect(Graphics g, SolveProjector sp, RectangleF rect, Brush brush, Pen pen)
        {
            List<PointF> points = new List<PointF>();

            points.Add(WarpPoint(sp, rect.Location));
            points.Add(WarpPoint(sp, new PointF(rect.Left + rect.Width, rect.Top)));
            points.Add(WarpPoint(sp, new PointF(rect.Left + rect.Width, rect.Top + rect.Height)));
            points.Add(WarpPoint(sp, new PointF(rect.Left, rect.Top + rect.Height)));

            g.FillPolygon(brush, points.ToArray());
            g.DrawPolygon(pen, points.ToArray());

        }

        PointF WarpPoint(SolveProjector sp, PointF pnt)
        {
            //double width = 2048;
            //double height = 2048;
            //double centerX = 1024;
            //double centerY = 1024;
            //double radius = 900;

            PointF pntOut = new PointF();

            Vector2d altaz = sp.GetCoordinatesForScreenPoint(pnt.X, pnt.Y);
            altaz.X += 180;
            pntOut.X = (float)(bmpCenter.X + (1.0 - (altaz.Y / 90)) * Math.Cos(altaz.X / 180 * Math.PI) * radius);
            pntOut.Y = (float)(bmpCenter.Y + (1.0 - (altaz.Y / 90)) * Math.Sin(altaz.X / 180 * Math.PI) * radius);

            return pntOut;
        }

        private void LoadGrid_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "WWT Calibration Grids (*.grd)|*.grd";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string[] lines = File.ReadAllLines(ofd.FileName);
                int currentID = -1;
                List<GroundTruthPoint> gtPoints = new List<GroundTruthPoint>();
                int id = 1;
                foreach(string line in lines)
                {
                    string[] parts = line.Split(new char[] { ',' });

                    if (parts.Length > 0)
                    {
                        if (parts[0] == "Group")
                        {
                            currentID = int.Parse(parts[1]);
                            gtPoints = new List<GroundTruthPoint>();

                            foreach(var pe in this.CalibrationInfo.Projectors)
                            {
                                if (pe.ID == currentID)
                                {
                                    pe.Constraints = gtPoints;
                                    break;
                                }
                            }
                        }

                        if (parts.Length == 4)
                        {
                            GroundTruthPoint pnt = new GroundTruthPoint();
                            pnt.X = double.Parse(parts[0]);
                            pnt.Y = double.Parse(parts[1]);
                            pnt.Az = double.Parse(parts[2]);
                            pnt.Alt = double.Parse(parts[3]);
                            pnt.AxisType = AxisTypes.Both;
                            pnt.Weight = 1;
                            pnt.ID = id++;
                            gtPoints.Add(pnt);
                        }
                    }
                }

            }
        }
    }
}
