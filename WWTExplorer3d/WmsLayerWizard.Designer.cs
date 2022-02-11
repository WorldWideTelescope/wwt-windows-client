namespace TerraViewer
{
    partial class WmsLayerWizard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.wmsUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.LayersTree = new System.Windows.Forms.TreeView();
            this.ServerList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ServerName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.AbstractLabel = new System.Windows.Forms.Label();
            this.Delete = new TerraViewer.WwtButton();
            this.close = new TerraViewer.WwtButton();
            this.add = new TerraViewer.WwtButton();
            this.AddServer = new TerraViewer.WwtButton();
            this.GetCapabilities = new TerraViewer.WwtButton();
            this.Abstract = new System.Windows.Forms.TextBox();
            this.TiledWMS = new TerraViewer.WWTCheckbox();
            this.AddAsLayer = new TerraViewer.WWTCheckbox();
            this.dontParse = new TerraViewer.WWTCheckbox();
            this.SuspendLayout();
            // 
            // wmsUrl
            // 
            this.wmsUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wmsUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.wmsUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.wmsUrl.ForeColor = System.Drawing.Color.White;
            this.wmsUrl.Location = new System.Drawing.Point(272, 30);
            this.wmsUrl.Name = "wmsUrl";
            this.wmsUrl.Size = new System.Drawing.Size(339, 20);
            this.wmsUrl.TabIndex = 3;
            this.wmsUrl.Text = "https://gibs.earthdata.nasa.gov/wms/epsg4326/best/wms.cgi";
            this.wmsUrl.TextChanged += new System.EventHandler(this.wmsUrl_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(269, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Web Mapping Service URL";
            // 
            // LayersTree
            // 
            this.LayersTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LayersTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.LayersTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LayersTree.ForeColor = System.Drawing.Color.White;
            this.LayersTree.Location = new System.Drawing.Point(12, 215);
            this.LayersTree.Name = "LayersTree";
            this.LayersTree.Size = new System.Drawing.Size(693, 286);
            this.LayersTree.TabIndex = 5;
            this.LayersTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.LayersTree_AfterSelect);
            // 
            // ServerList
            // 
            this.ServerList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ServerList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ServerList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ServerList.ForeColor = System.Drawing.Color.White;
            this.ServerList.FormattingEnabled = true;
            this.ServerList.Location = new System.Drawing.Point(12, 82);
            this.ServerList.Name = "ServerList";
            this.ServerList.Size = new System.Drawing.Size(599, 104);
            this.ServerList.TabIndex = 9;
            this.ServerList.SelectedIndexChanged += new System.EventHandler(this.ServerList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Layers and Styles";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Server List";
            // 
            // ServerName
            // 
            this.ServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ServerName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ServerName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ServerName.ForeColor = System.Drawing.Color.White;
            this.ServerName.Location = new System.Drawing.Point(15, 30);
            this.ServerName.Name = "ServerName";
            this.ServerName.Size = new System.Drawing.Size(246, 20);
            this.ServerName.TabIndex = 3;
            this.ServerName.Text = "<Type Server Name Here>";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Server Name";
            // 
            // AbstractLabel
            // 
            this.AbstractLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AbstractLabel.AutoSize = true;
            this.AbstractLabel.Location = new System.Drawing.Point(12, 508);
            this.AbstractLabel.Name = "AbstractLabel";
            this.AbstractLabel.Size = new System.Drawing.Size(46, 13);
            this.AbstractLabel.TabIndex = 12;
            this.AbstractLabel.Text = "Abstract";
            // 
            // Delete
            // 
            this.Delete.BackColor = System.Drawing.Color.Transparent;
            this.Delete.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Delete.ImageDisabled = null;
            this.Delete.ImageEnabled = null;
            this.Delete.Location = new System.Drawing.Point(618, 82);
            this.Delete.MaximumSize = new System.Drawing.Size(140, 33);
            this.Delete.Name = "Delete";
            this.Delete.Selected = false;
            this.Delete.Size = new System.Drawing.Size(87, 33);
            this.Delete.TabIndex = 10;
            this.Delete.Text = "Delete";
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.close.BackColor = System.Drawing.Color.Transparent;
            this.close.DialogResult = System.Windows.Forms.DialogResult.None;
            this.close.ImageDisabled = null;
            this.close.ImageEnabled = null;
            this.close.Location = new System.Drawing.Point(615, 610);
            this.close.MaximumSize = new System.Drawing.Size(140, 33);
            this.close.Name = "close";
            this.close.Selected = false;
            this.close.Size = new System.Drawing.Size(94, 33);
            this.close.TabIndex = 8;
            this.close.Text = "Close";
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // add
            // 
            this.add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.add.BackColor = System.Drawing.Color.Transparent;
            this.add.DialogResult = System.Windows.Forms.DialogResult.None;
            this.add.ImageDisabled = null;
            this.add.ImageEnabled = null;
            this.add.Location = new System.Drawing.Point(524, 610);
            this.add.MaximumSize = new System.Drawing.Size(140, 33);
            this.add.Name = "add";
            this.add.Selected = false;
            this.add.Size = new System.Drawing.Size(94, 33);
            this.add.TabIndex = 6;
            this.add.Text = "Add";
            this.add.Click += new System.EventHandler(this.add_Click);
            // 
            // AddServer
            // 
            this.AddServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddServer.BackColor = System.Drawing.Color.Transparent;
            this.AddServer.DialogResult = System.Windows.Forms.DialogResult.None;
            this.AddServer.ImageDisabled = null;
            this.AddServer.ImageEnabled = null;
            this.AddServer.Location = new System.Drawing.Point(617, 23);
            this.AddServer.MaximumSize = new System.Drawing.Size(140, 33);
            this.AddServer.Name = "AddServer";
            this.AddServer.Selected = false;
            this.AddServer.Size = new System.Drawing.Size(88, 33);
            this.AddServer.TabIndex = 4;
            this.AddServer.Text = "Add Server";
            this.AddServer.Click += new System.EventHandler(this.AddServer_Click);
            // 
            // GetCapabilities
            // 
            this.GetCapabilities.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GetCapabilities.BackColor = System.Drawing.Color.Transparent;
            this.GetCapabilities.DialogResult = System.Windows.Forms.DialogResult.None;
            this.GetCapabilities.ImageDisabled = null;
            this.GetCapabilities.ImageEnabled = null;
            this.GetCapabilities.Location = new System.Drawing.Point(618, 157);
            this.GetCapabilities.MaximumSize = new System.Drawing.Size(140, 33);
            this.GetCapabilities.Name = "GetCapabilities";
            this.GetCapabilities.Selected = false;
            this.GetCapabilities.Size = new System.Drawing.Size(93, 33);
            this.GetCapabilities.TabIndex = 4;
            this.GetCapabilities.Text = "Get Layers";
            this.GetCapabilities.Click += new System.EventHandler(this.GetCapabilities_Click);
            // 
            // Abstract
            // 
            this.Abstract.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Abstract.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.Abstract.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Abstract.ForeColor = System.Drawing.Color.White;
            this.Abstract.Location = new System.Drawing.Point(12, 526);
            this.Abstract.Multiline = true;
            this.Abstract.Name = "Abstract";
            this.Abstract.Size = new System.Drawing.Size(693, 78);
            this.Abstract.TabIndex = 11;
            // 
            // TiledWMS
            // 
            this.TiledWMS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TiledWMS.BackColor = System.Drawing.Color.Transparent;
            this.TiledWMS.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.TiledWMS.Checked = false;
            this.TiledWMS.Location = new System.Drawing.Point(355, 610);
            this.TiledWMS.Name = "TiledWMS";
            this.TiledWMS.Size = new System.Drawing.Size(163, 25);
            this.TiledWMS.TabIndex = 13;
            this.TiledWMS.Text = "Tiled WMS";
            this.TiledWMS.CheckedChanged += new System.EventHandler(this.TiledWMS_CheckedChanged);
            // 
            // AddAsLayer
            // 
            this.AddAsLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddAsLayer.BackColor = System.Drawing.Color.Transparent;
            this.AddAsLayer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.AddAsLayer.Checked = false;
            this.AddAsLayer.Location = new System.Drawing.Point(355, 632);
            this.AddAsLayer.Name = "AddAsLayer";
            this.AddAsLayer.Size = new System.Drawing.Size(163, 25);
            this.AddAsLayer.TabIndex = 13;
            this.AddAsLayer.Text = "Add As Layer";
            this.AddAsLayer.CheckedChanged += new System.EventHandler(this.AddAsLayer_CheckedChanged);
            // 
            // dontParse
            // 
            this.dontParse.BackColor = System.Drawing.Color.Transparent;
            this.dontParse.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.dontParse.Checked = false;
            this.dontParse.Location = new System.Drawing.Point(618, 51);
            this.dontParse.Name = "dontParse";
            this.dontParse.Size = new System.Drawing.Size(87, 25);
            this.dontParse.TabIndex = 14;
            this.dontParse.Text = "Don\'t Parse";
            // 
            // WmsLayerWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(717, 662);
            this.Controls.Add(this.dontParse);
            this.Controls.Add(this.AddAsLayer);
            this.Controls.Add(this.TiledWMS);
            this.Controls.Add(this.AbstractLabel);
            this.Controls.Add(this.Abstract);
            this.Controls.Add(this.Delete);
            this.Controls.Add(this.ServerList);
            this.Controls.Add(this.close);
            this.Controls.Add(this.add);
            this.Controls.Add(this.LayersTree);
            this.Controls.Add(this.AddServer);
            this.Controls.Add(this.GetCapabilities);
            this.Controls.Add(this.ServerName);
            this.Controls.Add(this.wmsUrl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WmsLayerWizard";
            this.ShowIcon = false;
            this.Text = "WMS Layers";
            this.Load += new System.EventHandler(this.WmsLayerWizard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox wmsUrl;
        private System.Windows.Forms.Label label1;
        private WwtButton GetCapabilities;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TreeView LayersTree;
        private WwtButton add;
        private WwtButton close;
        private System.Windows.Forms.ListBox ServerList;
        private System.Windows.Forms.Label label2;
        private WwtButton AddServer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ServerName;
        private System.Windows.Forms.Label label4;
        private WwtButton Delete;
        private System.Windows.Forms.Label AbstractLabel;
        private System.Windows.Forms.TextBox Abstract;
        private WWTCheckbox TiledWMS;
        private WWTCheckbox AddAsLayer;
        private WWTCheckbox dontParse;
    }
}