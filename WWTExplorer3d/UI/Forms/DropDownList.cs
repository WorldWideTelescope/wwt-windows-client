using System;
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

        bool filterType;

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
                foreach (TreeNode node in DropList.Nodes)
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
                var filter = value;
                foreach (TreeNode node in DropList.Nodes)
                {
                    var type = (Classification)Enum.Parse(typeof(Classification), node.Name.Replace(" ", ""));
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
            var node = e.Node;
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
            Hide();

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
                Hide();
        }

        private void listBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Hide();
            }
        }

        private void FilterDropDown_Load(object sender, EventArgs e)
        {

        }

        private void FilterDropDown_Activated(object sender, EventArgs e)
        {
            if (!FilterType)
            {
                 var rect = Screen.GetBounds(this);


                 var height = rect.Height *10/9;

                var count = Items.Count;
                Height = Math.Min(height, listBox.ItemHeight * Math.Max(3, count + 1));
            }
        }


    }
}