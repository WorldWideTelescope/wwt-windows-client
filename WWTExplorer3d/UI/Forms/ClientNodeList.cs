using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
            this.Height = Properties.Settings.Default.ClientNodeListSize.Height;
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.label2.Text = Language.GetLocalizedText(1079, "Projector Server List");
            this.ShowDetail.Text = Language.GetLocalizedText(1133, "Show Details");     
            this.Text = Language.GetLocalizedText(1134, "Projector Servers");
        }  

        private void ClientNodeList_Load(object sender, EventArgs e)
        {
            ShowDetail.Checked = Properties.Settings.Default.ShowClientNodeDetails;
            UpdateNodeList();
           
            
        }

        static ClientNodeList master = null;
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
            bool details = ShowDetail.Checked;

            object selectedNode = null;

            if (nodeTree.SelectedNode != null)
            {
                selectedNode = nodeTree.SelectedNode.Tag;
            }


            TreeNode tn = nodeTree.TopNode;
            ClientNode TopClientNode = null;
            TreeNode TopNode = null;
            if (tn != null)
            {
                 TopClientNode = tn.Tag as ClientNode;
            }
            

            nodeTree.BeginUpdate();
            nodeTree.Nodes.Clear();

            TreeNode parent = new TreeNode(Language.GetLocalizedText(1132, "Cluster ID : ") + Earth3d.MainWindow.Config.ClusterID.ToString());

            nodeTree.Nodes.Add(parent);
            parent.Expand();

            int worstStatus = (int)ClientNodeStatus.Online+1;
            Color parentColor = Color.White;

            foreach (ClientNode node in NetControl.NodeList.Values)
            {
                TreeNode child = new TreeNode(node.Name);
                child.Tag = node;

                if (node == TopClientNode)
                {
                    TopNode = child;
                }

                Color fgColor = Color.White;
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
                    child.Nodes.Add(MakeNode(node.LastFPS.ToString() + Language.GetLocalizedText(886, "FPS"), fgColor));
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
            TreeNode node = new TreeNode(text);
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
           
            Properties.Settings.Default.ClientNodeListSize = new Size(this.Width, this.Height);
        }

        private void ShowDetail_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowClientNodeDetails = ShowDetail.Checked;
            UpdateNodeList();
        }
        
        ContextMenuStrip contextMenu = null;

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
                        ClientNode client = (ClientNode)nodeTree.SelectedNode.Tag;

                        contextMenu = new ContextMenuStrip();
                        ToolStripMenuItem PauseRendering = new ToolStripMenuItem(Language.GetLocalizedText(1136, "Pause Rendering"));
                        ToolStripMenuItem Rename = new ToolStripMenuItem(Language.GetLocalizedText(225, "Rename"));
                        ToolStripMenuItem Delete = new ToolStripMenuItem(Language.GetLocalizedText(167, "Delete"));
                        ToolStripMenuItem Restart = new ToolStripMenuItem(Language.GetLocalizedText(1137, "Restart"));
                        ToolStripMenuItem Shutdown = new ToolStripMenuItem(Language.GetLocalizedText(1138, "Shutdown"));
                        ToolStripMenuItem Launch = new ToolStripMenuItem(Language.GetLocalizedText(1139, "Launch"));
                        ToolStripMenuItem Close = new ToolStripMenuItem(Language.GetLocalizedText(212, "Close"));
                        Restart.Click += new EventHandler(Restart_Click);
                        Rename.Click += new EventHandler(Rename_Click);
                        Delete.Click += new EventHandler(Delete_Click);
                        Launch.Click += new EventHandler(Launch_Click);
                        Close.Click += new EventHandler(Close_Click);
                        PauseRendering.Click += new EventHandler(PauseRendering_Click);
                        contextMenu.Items.Add(Rename);
                        ToolStripSeparator sep1 = new ToolStripSeparator();
                        contextMenu.Items.Add(sep1);
                        contextMenu.Items.Add(Delete);
                        ToolStripSeparator sep2 = new ToolStripSeparator();
                        contextMenu.Items.Add(sep2);
                        contextMenu.Items.Add(Launch);
                        contextMenu.Items.Add(Close);
                        ToolStripSeparator sep3 = new ToolStripSeparator();
                        contextMenu.Items.Add(sep3);
                        contextMenu.Items.Add(Restart);
                        contextMenu.Items.Add(Shutdown);


                        contextMenu.Show(Cursor.Position);
                    }
                    else
                    {
                        ClientNode client = (ClientNode)nodeTree.SelectedNode.Tag;

                        contextMenu = new ContextMenuStrip();
                        ToolStripMenuItem PauseRendering = new ToolStripMenuItem(Language.GetLocalizedText(1136, "Pause Rendering"));
                        ToolStripMenuItem Restart = new ToolStripMenuItem(Language.GetLocalizedText(1137, "Restart"));
                        ToolStripMenuItem Shutdown = new ToolStripMenuItem(Language.GetLocalizedText(1138, "Shutdown"));
                        ToolStripMenuItem Launch = new ToolStripMenuItem(Language.GetLocalizedText(1139, "Launch"));
                        ToolStripMenuItem Close = new ToolStripMenuItem(Language.GetLocalizedText(212, "Close"));
                        Restart.Click += new EventHandler(Restart_Click);

                        Launch.Click += new EventHandler(Launch_Click);
                        Close.Click += new EventHandler(Close_Click);
                        PauseRendering.Click += new EventHandler(PauseRendering_Click);
                        contextMenu.Items.Add(PauseRendering);
                        ToolStripSeparator sep2 = new ToolStripSeparator();
                        contextMenu.Items.Add(sep2);
                        contextMenu.Items.Add(Launch);
                        contextMenu.Items.Add(Close);
                        ToolStripSeparator sep3 = new ToolStripSeparator();
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
                ClientNode node = nodeTree.SelectedNode.Tag as ClientNode;

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
                ClientNode node = nodeTree.SelectedNode.Tag as ClientNode;

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
                ClientNode node = nodeTree.SelectedNode.Tag as ClientNode;

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

            IPEndPoint bindEPA = new IPEndPoint(IPAddress.Parse(NetControl.GetThisHostIP()), 8099);
            sockA.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            sockA.Bind(bindEPA);



            EndPoint destinationEPA = (EndPoint)new IPEndPoint(target, 8089);

            string output = "WWTCONTROL2" + "," + Earth3d.MainWindow.Config.ClusterID + "," + command + "," + param;

            Byte[] header = Encoding.ASCII.GetBytes(output);

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
            ClientNode client = (ClientNode)nodeTree.SelectedNode.Tag;
            SimpleInput input = new SimpleInput(Language.GetLocalizedText(225, "Rename"), Language.GetLocalizedText(228, "New Name"), client.Name, 32);

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
