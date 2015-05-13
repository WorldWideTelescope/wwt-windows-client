using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TerraViewer
{
    public partial class ConstellationFigureEditor : Form
    {
        public ConstellationFigureEditor()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.label1.Text = Language.GetLocalizedText(164, "Constellation Figure Editor");
            this.label2.Text = Language.GetLocalizedText(165, "Right-click on star to add point");
            this.AddPoint.Text = Language.GetLocalizedText(166, "Add");
            this.DeletePoint.Text = Language.GetLocalizedText(167, "Delete");
            this.SaveFigures.Text = Language.GetLocalizedText(168, "Save");
            this.Text = Language.GetLocalizedText(164, "Constellation Figure Editor");
        }
        bool initialized = false;
        private void ConstellationFigureEditor_Load(object sender, EventArgs e)
        {
            if (figures != null)
            {
                LoadTree();
            }
        }

        private void LoadTree()
        {
            figureTree.Nodes.Clear();

            if (figures != null)
            {
                foreach (Lineset ls in figures.lines)
                {
                    TreeNode parent = figureTree.Nodes.Add(Constellations.FullName(ls.Name));
                    parent.Tag = ls;

                    foreach (Linepoint pnt in ls.Points)
                    {
                        TreeNode child = parent.Nodes.Add( pnt.ToString());
                        child.Tag = pnt;
                        child.Checked = pnt.PointType != PointType.Move;
                    }
                }
            }
            initialized = true;
        }

        Constellations figures;

        public Constellations Figures
        {
            get { return figures; }
            set { figures = value; }
        }

        private void figureTree_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            if (e.Node != null && e.Node.Tag is Linepoint)
            {
                if (e.Node.Tag is Linepoint)
                {
                    Linepoint lp = (Linepoint)e.Node.Tag;
                    //((Convert.ToDouble(line.Substring(0, 10)) / 24.0 * 360) - 180)
                    TourPlace p = new TourPlace(lp.ToString(), (lp.RA + 180) / 360 * 24, lp.Dec, Classification.Unidentified, "", ImageSetType.Sky, -1);
                    p.Distance = 1.0;
                    //Earth3d.MainWindow.SetLabelText(lp.ToString(), (lp.RA + 180) / 360 * 24, lp.Dec, 1.0);
                    Constellations.SelectedSegment = lp;

                }
            }
            else
            {
                if (e.Node.Checked)
                {
                    Earth3d.MainWindow.SetLabelText(null, false);
                    Constellations.SelectedSegment = null;
                }
            }
        }

        private void figureTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!initialized)
            {
                return;
            }

            if (e.Node.Tag != null && e.Node.Tag is Linepoint)
            {
                Linepoint lp = (Linepoint)e.Node.Tag;

                lp.PointType = e.Node.Checked ? PointType.Line : PointType.Move;

                e.Node.Text = lp.ToString();
                
                TreeNode parent = figureTree.SelectedNode.Parent;
                if (parent != null)
                {
                    Lineset ls = (Lineset)parent.Tag;
                    Earth3d.MainWindow.constellationsFigures.ResetConstellation(ls.Name);
                }
            }
            else
            {
                if (e.Node.Checked)
                {
                    e.Node.Checked = false;
                }
            }




        }

        private void DeletePoint_Click(object sender, EventArgs e)
        {
            DeleteSelectedPoint();

        }

        private void DeleteSelectedPoint()
        {
            if (figureTree.SelectedNode.Tag != null && figureTree.SelectedNode.Tag is Linepoint)
            {
                TreeNode parent = figureTree.SelectedNode.Parent;

                Lineset ls = (Lineset)parent.Tag;

                ls.Points.Remove((Linepoint)figureTree.SelectedNode.Tag);
                parent.Nodes.Remove(figureTree.SelectedNode);
                Earth3d.MainWindow.constellationsFigures.ResetConstellation(ls.Name);
            }
            else if (figureTree.SelectedNode.Tag != null && figureTree.SelectedNode.Tag is Lineset)
            {
                if (MessageBox.Show(Language.GetLocalizedText(169, "This will remove all points from the selected constellation.\nAre you sure you want to do this?"), Language.GetLocalizedText(170, "Confirm Delete all Constellation Points"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Lineset ls = (Lineset)figureTree.SelectedNode.Tag;
                    ls.Points.Clear();
                    figureTree.SelectedNode.Nodes.Clear();
                    Earth3d.MainWindow.constellationsFigures.ResetConstellation(ls.Name);
                }
                
            }

        }

        private void AddPoint_Click(object sender, EventArgs e)
        {
            AddFigurePoint();
        }

        private void AddFigurePoint()
        {
            TreeNode parent = figureTree.SelectedNode.Parent;

            Lineset ls = (Lineset)parent.Tag;

            ls.Points.Remove((Linepoint)figureTree.SelectedNode.Tag);
            parent.Nodes.Remove(figureTree.SelectedNode);
            Earth3d.MainWindow.constellationsFigures.ResetConstellation(ls.Name);

        }

        private void figureTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null && e.Node.Tag is Linepoint)
            {
                DeletePoint.Enabled = true;
            }
            else
            {
                DeletePoint.Enabled = true;
            }
        }
        ContextMenuStrip contextMenu = null;

        private void figureTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }

            figureTree.SelectedNode = e.Node;

            if (e.Button == MouseButtons.Right)
            {
                contextMenu = new ContextMenuStrip();
                ToolStripMenuItem deleteMenu = new ToolStripMenuItem(Language.GetLocalizedText(167, "Delete"));
                deleteMenu.Click += new EventHandler(deleteMenu_Click);
                contextMenu.Items.Add(deleteMenu);
                contextMenu.Show(Cursor.Position);
            }
            else if (e.Node.Tag is Lineset)
            {
                Lineset ls = (Lineset)figureTree.SelectedNode.Tag;

                Earth3d.MainWindow.GotoTarget(Constellations.ConstellationCentroids[ls.Name], false, false, true);
            }
        }

        void addMenu_Click(object sender, EventArgs e)
        {
            AddFigurePoint();
        }

        void deleteMenu_Click(object sender, EventArgs e)
        {
            DeleteSelectedPoint();
        }





        internal void AddFigurePoint(IPlace place)
        {
            TreeNode parent;
            Linepoint pnt = new Linepoint(place.RA * 15 - 180, place.Dec, PointType.Line, place.Name != Language.GetLocalizedText(90, "No Object") ? place.Name : null);
            if (figureTree.SelectedNode.Tag is Linepoint)
            {
                parent = figureTree.SelectedNode.Parent;

                Lineset ls = (Lineset)parent.Tag;

                Linepoint lp = (Linepoint)figureTree.SelectedNode.Tag;

                int index = ls.Points.FindIndex(delegate(Linepoint target) { return target == lp; }) + 1;

                ls.Points.Insert(index, pnt);

                TreeNode child;
                if (index >= parent.Nodes.Count)
                {
                    child = parent.Nodes.Add(pnt.ToString());
                }
                else
                {
                    child = parent.Nodes.Insert(index, pnt.ToString());
                }
                child.Tag = pnt;
                child.Checked = pnt.PointType != PointType.Move;
                figureTree.SelectedNode = child;
                Earth3d.MainWindow.constellationsFigures.ResetConstellation(ls.Name);
            }
            else
            {
                parent = figureTree.SelectedNode;
                Lineset ls = (Lineset)figureTree.SelectedNode.Tag;
                ls.Points.Add( pnt);
                TreeNode child = parent.Nodes.Add( pnt.ToString());
                child.Tag = pnt;
                child.Checked = pnt.PointType != PointType.Move;
                figureTree.SelectedNode = child;
                Earth3d.MainWindow.constellationsFigures.ResetConstellation(ls.Name);
            }
        }

        private void SaveFigures_Click(object sender, EventArgs e)
        {
            figures.Save(figures.Name);

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
            figures.Save(figures.Name);

            this.Close();

            return DialogResult.OK;
        }

        private void ConstellationFigureEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Earth3d.MainWindow.figureEditor = null;
        }


    }
}