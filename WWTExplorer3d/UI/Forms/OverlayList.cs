using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class OverlayList : Form
    {
        public OverlayList()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.label1.Text = Language.GetLocalizedText(1052, "Overlay Item List");
            this.Text = Language.GetLocalizedText(1053, "Overlay List");
        }


        static OverlayList master;
        public static void ShowOverlayList()
        {
            if (master != null)
            {
                master.Show();
            }
            else
            {
                master = new OverlayList();
                master.Owner = Earth3d.MainWindow;
                master.Show();
            }

            Properties.Settings.Default.ShowClientNodeList = true;
        }

        public static void CloseOverlayList()
        {
            if (master != null)
            {
                master.Close();
                master.Dispose();
                master = null;
            }
        }

        public static void UpdateOverlayList(TourStop tourStop, Selection selection)
        {
            if (master != null)
            {
                master.ItemList.Nodes.Clear();
                if (tourStop != null)
                {
                    foreach (var overlay in tourStop.Overlays)
                    {
                        var item = new TreeNode(overlay.Name);
                        item.Tag = overlay;
                        item.Checked = selection.IsOverlaySelected(overlay);
                        master.ItemList.Nodes.Add(item);
                    }
                }
            }
            // Hack to update Keyframer UI
            TimeLine.RefreshUi();
        }

        public static void UpdateOverlayListSelection(Selection selection)
        {
            if (master != null)
            {
                foreach (TreeNode child in master.ItemList.Nodes)
                {
                    var overlay = child.Tag as Overlay;
                    child.Checked = selection.IsOverlaySelected(overlay);
                }
            }
        }

        private void OverlayList_Load(object sender, EventArgs e)
        {
            if (Earth3d.MainWindow.TourEdit != null)
            {
                UpdateOverlayList(Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop,
                                    Earth3d.MainWindow.TourEdit.TourEditorUI.Selection);
            }
        }

        private void ItemList_MouseClick(object sender, MouseEventArgs e)
        {
            if (Control.MouseButtons == System.Windows.Forms.MouseButtons.Right)
            {
                if (Earth3d.MainWindow.TourEdit != null)
                {
                    Earth3d.MainWindow.TourEdit.TourEditorUI.ShowSelectionContextMenu();
                }

            }
        }



        private void ItemList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
           
        }

        private void ItemList_Click(object sender, EventArgs e)
        {
            var mea = e as MouseEventArgs;    
        }

        private void ItemList_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            } 
            
            if (!(Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Shift))
            {
                foreach (TreeNode child in ItemList.Nodes)
                {
                    child.Checked = false;
                }

                if (Earth3d.MainWindow.TourEdit != null)
                {
                    Earth3d.MainWindow.TourEdit.TourEditorUI.Selection.ClearSelection();
                }
            }

            ItemList.SelectedNode = e.Node;
            ItemList.SelectedNode.Checked = true;

            Earth3d.MainWindow.TourEdit.TourEditorUI.Selection.AddSelection(e.Node.Tag as Overlay);
            Earth3d.MainWindow.TourEdit.TourEditorUI.Focus = e.Node.Tag as Overlay;

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (Earth3d.MainWindow.TourEdit != null)
                {
                    Earth3d.MainWindow.TourEdit.TourEditorUI.ShowSelectionContextMenu();
                }
            }
        }

        private void ItemList_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (Earth3d.MainWindow.TourEdit != null)
                {
                    Earth3d.MainWindow.TourEdit.TourEditorUI.Selection.ClearSelection();
                    foreach (TreeNode child in ItemList.Nodes)
                    {
                        if (child.Checked)
                        {
                            Earth3d.MainWindow.TourEdit.TourEditorUI.Selection.AddSelection(child.Tag as Overlay);
                        }
                    }
                }
            }
        }

        private void OverlayList_FormClosed(object sender, FormClosedEventArgs e)
        {
            master = null;
        }
    }
}
