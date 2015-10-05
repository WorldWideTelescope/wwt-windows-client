using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace TerraViewer
{
    public partial class ClientNodeList : Form
    {
        public ClientNodeList()
        {
            InitializeComponent();
            Height = Properties.Settings.Default.ClientNodeListSize.Height;
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            label2.Text = Language.GetLocalizedText(1079, "Projector Server List");
            ShowDetail.Text = Language.GetLocalizedText(1133, "Show Details");     
            Text = Language.GetLocalizedText(1134, "Projector Servers");
        }  

        private void ClientNodeList_Load(object sender, EventArgs e)
        {
            ShowDetail.Checked = Properties.Settings.Default.ShowClientNodeDetails;
            UpdateNodeList();
           
            
        }

        static ClientNodeList master;
        public static void ShowNodeList()
        {
            if (master != null)
            {
                master.Show();
            }
            else
            {
                master = new ClientNodeList();
                master.Owner = Earth3d.MainWindow;
                master.Show();
            }

            Properties.Settings.Default.ShowClientNodeList = true;
        }

        public static void HideNodeList()
        {
            if (master != null)
            {
                master.Close();
            }
        }

        public static bool IsNodeListVisible()
        {
            return master != null;
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (NetControl.NodeListDirty)
            {
                UpdateNodeList();
                NetControl.NodeListDirty = false;
            }
        }

        private void UpdateNodeList()
        {
            var details = ShowDetail.Checked;

            object selectedNode = null;

            if (nodeTree.SelectedNode != null)
            {
                selectedNode = nodeTree.SelectedNode.Tag;
            }


            var tn = nodeTree.TopNode;
            ClientNode TopClientNode = null;
            TreeNode TopNode = null;
            if (tn != null)
            {
                 TopClientNode = tn.Tag as ClientNode;
            }
            

            nodeTree.BeginUpdate();
            nodeTree.Nodes.Clear();

            var parent = new TreeNode(Language.GetLocalizedText(1132, "Cluster ID : ") + Earth3d.MainWindow.Config.ClusterID);

            nodeTree.Nodes.Add(parent);
            parent.Expand();

            var worstStatus = (int)ClientNodeStatus.Online+1;
            var parentColor = Color.White;

            foreach (var node in NetControl.NodeList.Values)
            {
                var child = new TreeNode(node.Name);
                child.Tag = node;

                if (node == TopClientNode)
                {
                    TopNode = child;
                }

                var fgColor = Color.White;
                switch (node.Status)
                {
                    case ClientNodeStatus.Offline:
                        fgColor = Color.Red;
                        break;
                    case ClientNodeStatus.Online:
                        fgColor = Color.Green;
                        break;
                    case ClientNodeStatus.Working:
                        fgColor = Color.Yellow;
                        break;
                    case ClientNodeStatus.StandBy:
                        fgColor = Color.Blue;
                        break;
                    default:
                        break;
                } 

                if ((int)node.Status < worstStatus)
                {
                    worstStatus = (int)node.Status;
                    parentColor = fgColor;
                }        
                child.ForeColor = fgColor;
                
                parent.Nodes.Add(child);
                if (node == selectedNode)
                {
                    nodeTree.SelectedNode = child;
                }

                if (details)
                {
                    child.Nodes.Add(MakeNode(node.Status.ToString(), fgColor));
                    child.Nodes.Add(MakeNode(node.StatusText, fgColor));
                    child.Nodes.Add(MakeNode(node.IpAddress, fgColor));
                    child.Nodes.Add(MakeNode(Language.GetLocalizedText(1135, "Node ID: ") + node.NodeID, fgColor));
                    child.Nodes.Add(MakeNode(node.LastFPS + Language.GetLocalizedText(886, "FPS"), fgColor));
                    child.Expand();
                }
            }

            if (selectedNode == null)
            {
                nodeTree.SelectedNode = parent;
            }


            parent.ForeColor = parentColor;
            parent.Expand();
            nodeTree.TopNode = TopNode;
            nodeTree.EndUpdate();
        }

        TreeNode MakeNode(string text, Color color)
        {
            var node = new TreeNode(text);
            node.ForeColor = color;

            return node;
        }

        private void ClientNodeList_FormClosed(object sender, FormClosedEventArgs e)
        {
            master = null;
            if (!Earth3d.FormIsClosing)
            {
                Properties.Settings.Default.ShowClientNodeList = false;
            }

          
        }

        private void ClientNodeList_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            Properties.Settings.Default.ClientNodeListSize = new Size(Width, Height);
        }

        private void ShowDetail_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowClientNodeDetails = ShowDetail.Checked;
            UpdateNodeList();
        }
        
        ContextMenuStrip contextMenu;

        private void nodeTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }

            nodeTree.SelectedNode = e.Node;

            if (e.Button == MouseButtons.Right)
            {
                if (nodeTree.SelectedNode != null)
                {
                    if (nodeTree.SelectedNode.Tag is ClientNode)
                    {
                        var client = (ClientNode)nodeTree.SelectedNode.Tag;

                        contextMenu = new ContextMenuStrip();
                        var PauseRendering = new ToolStripMenuItem(Language.GetLocalizedText(1136, "Pause Rendering"));
                        var Rename = new ToolStripMenuItem(Language.GetLocalizedText(225, "Rename"));
                        var Delete = new ToolStripMenuItem(Language.GetLocalizedText(167, "Delete"));
                        var Restart = new ToolStripMenuItem(Language.GetLocalizedText(1137, "Restart"));
                        var Shutdown = new ToolStripMenuItem(Language.GetLocalizedText(1138, "Shutdown"));
                        var Launch = new ToolStripMenuItem(Language.GetLocalizedText(1139, "Launch"));
                        var Close = new ToolStripMenuItem(Language.GetLocalizedText(212, "Close"));
                        Restart.Click += Restart_Click;
                        Rename.Click += Rename_Click;
                        Delete.Click += Delete_Click;
                        Launch.Click += Launch_Click;
                        Close.Click += Close_Click;
                        PauseRendering.Click += PauseRendering_Click;
                        contextMenu.Items.Add(Rename);
                        var sep1 = new ToolStripSeparator();
                        contextMenu.Items.Add(sep1);
                        contextMenu.Items.Add(Delete);
                        var sep2 = new ToolStripSeparator();
                        contextMenu.Items.Add(sep2);
                        contextMenu.Items.Add(Launch);
                        contextMenu.Items.Add(Close);
                        var sep3 = new ToolStripSeparator();
                        contextMenu.Items.Add(sep3);
                        contextMenu.Items.Add(Restart);
                        contextMenu.Items.Add(Shutdown);


                        contextMenu.Show(Cursor.Position);
                    }
                    else
                    {
                        var client = (ClientNode)nodeTree.SelectedNode.Tag;

                        contextMenu = new ContextMenuStrip();
                        var PauseRendering = new ToolStripMenuItem(Language.GetLocalizedText(1136, "Pause Rendering"));
                        var Restart = new ToolStripMenuItem(Language.GetLocalizedText(1137, "Restart"));
                        var Shutdown = new ToolStripMenuItem(Language.GetLocalizedText(1138, "Shutdown"));
                        var Launch = new ToolStripMenuItem(Language.GetLocalizedText(1139, "Launch"));
                        var Close = new ToolStripMenuItem(Language.GetLocalizedText(212, "Close"));
                        Restart.Click += Restart_Click;

                        Launch.Click += Launch_Click;
                        Close.Click += Close_Click;
                        PauseRendering.Click += PauseRendering_Click;
                        contextMenu.Items.Add(PauseRendering);
                        var sep2 = new ToolStripSeparator();
                        contextMenu.Items.Add(sep2);
                        contextMenu.Items.Add(Launch);
                        contextMenu.Items.Add(Close);
                        var sep3 = new ToolStripSeparator();
                        contextMenu.Items.Add(sep3);
                        contextMenu.Items.Add(Restart);
                        contextMenu.Items.Add(Shutdown);


                        contextMenu.Show(Cursor.Position);
                    }
                }
            }
        }

        void Close_Click(object sender, EventArgs e)
        {
            if (nodeTree.SelectedNode != null && nodeTree.SelectedNode.Tag is ClientNode)
            {
                var node = nodeTree.SelectedNode.Tag as ClientNode;

                SendWWTRemoteCommand(node.IpAddress, "KILL", "");
            }
            if (nodeTree.SelectedNode != null && nodeTree.SelectedNode.Tag == null)
            {
                SendWWTRemoteCommand("255.255.255.255", "KILL", "");
            }
            
        }

        void Launch_Click(object sender, EventArgs e)
        {
            if (nodeTree.SelectedNode != null && nodeTree.SelectedNode.Tag is ClientNode)
            {
                var node = nodeTree.SelectedNode.Tag as ClientNode;

                SendWWTRemoteCommand(node.IpAddress, "LAUNCH", "");
            }
            if (nodeTree.SelectedNode != null && nodeTree.SelectedNode.Tag == null)
            {
                SendWWTRemoteCommand("255.255.255.255", "LAUNCH", "");
            }
        }

        void Restart_Click(object sender, EventArgs e)
        {
            if (nodeTree.SelectedNode != null && nodeTree.SelectedNode.Tag is ClientNode)
            {
                var node = nodeTree.SelectedNode.Tag as ClientNode;

                SendWWTRemoteCommand(node.IpAddress, "RESTART", "");
            }
            if (nodeTree.SelectedNode != null && nodeTree.SelectedNode.Tag == null)
            {
                SendWWTRemoteCommand("255.255.255.255", "RESTART", "");
            }
        }

        
     
        

        public static void SendWWTRemoteCommand(string targetIP, string command, string param)
        {
            Socket sockA = null;
            IPAddress target = null;
            sockA = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IP);
            if(targetIP == "255.255.255.255")
            {
                sockA.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                target = IPAddress.Broadcast;

            }
            else
            {
                target = IPAddress.Parse(targetIP);
            }

            var bindEPA = new IPEndPoint(IPAddress.Parse(NetControl.GetThisHostIP()), 8099);
            sockA.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            sockA.Bind(bindEPA);



            var destinationEPA = (EndPoint)new IPEndPoint(target, 8089);

            var output = "WWTCONTROL2" + "," + Earth3d.MainWindow.Config.ClusterID + "," + command + "," + param;

            var header = Encoding.ASCII.GetBytes(output);

            sockA.SendTo(header, destinationEPA);
            sockA.Close();
            
        }

        void Delete_Click(object sender, EventArgs e)
        {
            if (nodeTree.SelectedNode != null && nodeTree.SelectedNode.Tag is ClientNode)
            {
                NetControl.NodeList.Remove(((ClientNode)nodeTree.SelectedNode.Tag).NodeID);
                UpdateNodeList();
            }
        }


        void Rename_Click(object sender, EventArgs e)
        {
            var client = (ClientNode)nodeTree.SelectedNode.Tag;
            var input = new SimpleInput(Language.GetLocalizedText(225, "Rename"), Language.GetLocalizedText(228, "New Name"), client.Name, 32);

            if (input.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(input.ResultText))
                {
                    client.Name = input.ResultText;
                    UpdateNodeList();
                }
            }
        }

        void PauseRendering_Click(object sender, EventArgs e)
        {
    
        }



    }
}
