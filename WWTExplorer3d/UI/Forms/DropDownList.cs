using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public delegate void SelectionChangedEventHandler(object sender, EventArgs e);

    public partial class FilterDropDown : Form
    {
        public event SelectionChangedEventHandler SelectionChanged;
        public FilterDropDown()
        {
            InitializeComponent();
        }

        bool filterType = false;

        public bool FilterType
        {
            get { return filterType; }
            set
            {
                filterType = value;

                listBox.Visible = !value;
                DropList.Visible = value;
            }
        }

        public object SelectedItem
        {
            get
            {
                
                return listBox.SelectedItem;
            }

        }


        public int SelectedIndex
        {
            get 
            {
                return listBox.SelectedIndex;
            }
            set
            {
                listBox.SelectedIndex = value;
            }
        }


        public ListBox.ObjectCollection Items
        {
            get
            {
                return listBox.Items; 
            }
        }
        
        
        private void DropList_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }


        public Classification FilterValue
        {

            get
            {
                Classification filter = 0;
                foreach (TreeNode node in this.DropList.Nodes)
                {
                    if (node.Checked)
                    {
                        filter |= (Classification)Enum.Parse(typeof(Classification), node.Name.Replace(" ",""));
                    }
                    else
                    {
                        foreach (TreeNode child in node.Nodes)
                        {
                            if (child.Checked)
                            {
                                filter |= (Classification)Enum.Parse(typeof(Classification), child.Name.Replace(" ", ""));
                            }
                        }
                    }
                }
                return filter;
            }
            set
            {
                Classification filter = value;
                foreach (TreeNode node in this.DropList.Nodes)
                {
                    Classification type = (Classification)Enum.Parse(typeof(Classification), node.Name.Replace(" ", ""));
                    node.Checked = (filter & type) == type;
                    
                    foreach (TreeNode child in node.Nodes)
                    {
                        type = (Classification)Enum.Parse(typeof(Classification), child.Name.Replace(" ", ""));
                        child.Checked = (filter & type) == type;

                    }
                }
            }
        }

        private void DropList_AfterCheck(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            if (e.Action != TreeViewAction.ByKeyboard && e.Action != TreeViewAction.ByMouse)
            {
                return;
            }

            foreach(TreeNode child in node.Nodes)
            {
                child.Checked = node.Checked;
            }

            if (!node.Checked)
            {
                if (node.Parent != null)
                {
                    node.Parent.Checked = false;
                }
            }

            if (SelectionChanged != null)
            {
                SelectionChanged.Invoke(this, new EventArgs());
            }
        }


        private void FilterDropDown_Deactivate(object sender, EventArgs e)
        {
            this.Hide();

        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged.Invoke(this, new EventArgs());
            }
        }

        private void listBox_MouseClick(object sender, MouseEventArgs e)
        {
                this.Hide();
        }

        private void listBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                this.Hide();
            }
        }

        private void FilterDropDown_Load(object sender, EventArgs e)
        {

        }

        private void FilterDropDown_Activated(object sender, EventArgs e)
        {
            if (!FilterType)
            {
                 Rectangle rect = Screen.GetBounds(this);


                 int height = rect.Height *10/9;

                int count = Items.Count;
                this.Height = Math.Min(height, listBox.ItemHeight * Math.Max(3, count + 1));
            }
        }


    }
}