using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TerraViewer
{

    public class LayerUI
    {
        virtual public bool HasTreeViewNodes
        {
            get
            {
                return false;
            }
        }

        virtual public List<LayerUITreeNode> GetTreeNodes()
        {
            return null;
        }


        virtual public List<LayerUIMenuItem> GetNodeContextMenu(LayerUITreeNode node)
        {
            return null;
        }

        virtual public void SetUICallbacks( IUIServicesCallbacks callbacks)
        {
        }
    }

    public interface IUIServicesCallbacks
    {
        void ShowRowData(Dictionary<String, String> rowData);
        void UpdateNode(Layer layer, LayerUITreeNode node);
    }

    public delegate void MenuItemSelectedDelegate( LayerUIMenuItem item);

    public class LayerUIMenuItem
    {
        private String name;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        private object tag;

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private bool isChecked;

        public bool Checked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }

        private bool isEnabled = true;

        public bool Enabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }


        public event MenuItemSelectedDelegate MenuItemSelected;

        public void FireMenuItemSelected()
        {
            if (MenuItemSelected != null)
            {
                MenuItemSelected.Invoke(this);
            }
        }

        List<LayerUIMenuItem> subMenus;
        public List<LayerUIMenuItem> SubMenus
        {
            get
            {
                if (subMenus == null)
                {
                    subMenus = new List<LayerUIMenuItem>();
                }
                return subMenus;
            }
        }
    }

    public delegate void LayerUITreeNodeCheckedDelegate(LayerUITreeNode node, bool newState);
    public delegate void LayerUITreeNodeUpdatedDelegate(LayerUITreeNode node);
    public delegate void LayerUITreeNodeSelectedDelegate(LayerUITreeNode node);
    public delegate void LayerUITreeNodeActivatedDelegate(LayerUITreeNode node);
    public delegate bool LayerUITreeNodeIsCheckedDelegate(LayerUITreeNode node);

    public class LayerUITreeNode
    {
        public event LayerUITreeNodeCheckedDelegate NodeChecked;

        public void FireNodeChecked(bool newState)
        {
            if (NodeChecked != null)
            {
                NodeChecked.Invoke(this, newState);
            }
        }

        public event LayerUITreeNodeUpdatedDelegate NodeUpdated;

        public void FireNodeUpdated()
        {
            if (NodeUpdated != null)
            {
                NodeUpdated.Invoke(this);
            }
        }

        public event LayerUITreeNodeSelectedDelegate NodeSelected;

        public void FireNodeSelected()
        {
            if (NodeSelected != null)
            {
                NodeSelected.Invoke(this);
            }
        }  
        public event LayerUITreeNodeActivatedDelegate NodeActivated;

        public void FireNodeActivated()
        {
            if (NodeActivated != null)
            {
                NodeActivated.Invoke(this);
            }
        }

        private String name;

        public String Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    FireNodeUpdated();
                }
            }
        }

        public bool UiUpdating = false;

        private LayerUITreeNode parent;

        public LayerUITreeNode Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        private int level;

        public int Level
        {
            get { return level; }
            set { level = value; }
        }


        private object tag;

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private object referenceTag;

        public object ReferenceTag
        {
            get { return referenceTag; }
            set { referenceTag = value; }
        }

        private bool open;

        public bool Opened
        {
            get { return open; }
            set
            {
                if (open != value)
                {
                    open = value;
                    FireNodeUpdated();
                }
            }
        }
        private bool isChecked;


        public event LayerUITreeNodeIsCheckedDelegate IsChecked;

        public bool Checked
        {
            get
            {
                if (IsChecked != null)
                {
                    isChecked = IsChecked.Invoke(this);
                }

                return isChecked;
            }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    FireNodeUpdated();
                }
            }
        }

        private bool bold;

        public bool Bold
        {
            get { return bold; }
            set
            {
                if (bold != value)
                {
                    bold = value;
                    FireNodeUpdated();
                }
            }
        }
        private Color color = Color.White;

        public Color Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    FireNodeUpdated();
                }
            }
        }


        public LayerUITreeNode Add(string name)
        {
            var node = new LayerUITreeNode();
            node.Name = name;
            node.Parent = this;
            node.Level = this.Level + 1;
            Nodes.Add(node);
            return node;
        }

        List<LayerUITreeNode> nodes;
        public List<LayerUITreeNode> Nodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new List<LayerUITreeNode>();
                }
                return nodes;
            }
        }
    }
}
