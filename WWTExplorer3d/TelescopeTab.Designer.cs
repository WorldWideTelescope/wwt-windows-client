namespace TerraViewer
{
    partial class TelescopeTab
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Setup = new TerraViewer.WwtButton();
            this.ra = new System.Windows.Forms.Label();
            this.telescopeGroup = new System.Windows.Forms.GroupBox();
            this.TrackScope = new TerraViewer.WWTCheckbox();
            this.Park = new TerraViewer.WwtButton();
            this.SyncScope = new TerraViewer.WwtButton();
            this.ScopeWest = new TerraViewer.WwtButton();
            this.ScopeEast = new TerraViewer.WwtButton();
            this.ScopeSouth = new TerraViewer.WwtButton();
            this.ScopeNorth = new TerraViewer.WwtButton();
            this.SlewScope = new TerraViewer.WwtButton();
            this.CenterScope = new TerraViewer.WwtButton();
            this.azText = new System.Windows.Forms.Label();
            this.altText = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.decText = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.raText = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Choose = new TerraViewer.WwtButton();
            this.ConnectScope = new TerraViewer.WwtButton();
            this.PlatformStatus = new System.Windows.Forms.Label();
            this.ScopeStatus = new System.Windows.Forms.GroupBox();
            this.TelescopeTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.telescopeGroup.SuspendLayout();
            this.ScopeStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // pinUp
            // 
            this.pinUp.Location = new System.Drawing.Point(365, 96);
            this.pinUp.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = global::TerraViewer.Properties.Resources.ascom;
            this.pictureBox1.Location = new System.Drawing.Point(901, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 56);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox1, "Click to install ASCOM platform to allow telescope control.");
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseHover += new System.EventHandler(this.pictureBox1_MouseHover);
            // 
            // Setup
            // 
            this.Setup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Setup.BackColor = System.Drawing.Color.Transparent;
            this.Setup.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Setup.Location = new System.Drawing.Point(797, 65);
            this.Setup.MaximumSize = new System.Drawing.Size(140, 33);
            this.Setup.Name = "Setup";
            this.Setup.Selected = false;
            this.Setup.Size = new System.Drawing.Size(79, 33);
            this.Setup.TabIndex = 2;
            this.Setup.Text = "Setup";
            this.Setup.Click += new System.EventHandler(this.Setup_Click);
            // 
            // ra
            // 
            this.ra.AutoSize = true;
            this.ra.BackColor = System.Drawing.Color.Transparent;
            this.ra.ForeColor = System.Drawing.Color.White;
            this.ra.Location = new System.Drawing.Point(25, 19);
            this.ra.Name = "ra";
            this.ra.Size = new System.Drawing.Size(25, 13);
            this.ra.TabIndex = 3;
            this.ra.Text = "RA:";
            // 
            // telescopeGroup
            // 
            this.telescopeGroup.BackColor = System.Drawing.Color.Transparent;
            this.telescopeGroup.Controls.Add(this.TrackScope);
            this.telescopeGroup.Controls.Add(this.Park);
            this.telescopeGroup.Controls.Add(this.SyncScope);
            this.telescopeGroup.Controls.Add(this.ScopeWest);
            this.telescopeGroup.Controls.Add(this.ScopeEast);
            this.telescopeGroup.Controls.Add(this.ScopeSouth);
            this.telescopeGroup.Controls.Add(this.ScopeNorth);
            this.telescopeGroup.Controls.Add(this.SlewScope);
            this.telescopeGroup.Controls.Add(this.CenterScope);
            this.telescopeGroup.ForeColor = System.Drawing.Color.White;
            this.telescopeGroup.Location = new System.Drawing.Point(209, 6);
            this.telescopeGroup.Name = "telescopeGroup";
            this.telescopeGroup.Size = new System.Drawing.Size(423, 92);
            this.telescopeGroup.TabIndex = 4;
            this.telescopeGroup.TabStop = false;
            this.telescopeGroup.Text = "Telescope Control - Not Connected";
            // 
            // TrackScope
            // 
            this.TrackScope.BackColor = System.Drawing.Color.Transparent;
            this.TrackScope.Checked = false;
            this.TrackScope.Enabled = false;
            this.TrackScope.Location = new System.Drawing.Point(16, 24);
            this.TrackScope.Name = "TrackScope";
            this.TrackScope.Size = new System.Drawing.Size(113, 25);
            this.TrackScope.TabIndex = 4;
            this.TrackScope.Text = "Track Telescope";
            this.TrackScope.CheckedChanged += new System.EventHandler(this.TrackScope_CheckedChanged);
            // 
            // Park
            // 
            this.Park.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Park.BackColor = System.Drawing.Color.Transparent;
            this.Park.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Park.Enabled = false;
            this.Park.Location = new System.Drawing.Point(338, 12);
            this.Park.MaximumSize = new System.Drawing.Size(140, 33);
            this.Park.Name = "Park";
            this.Park.Selected = false;
            this.Park.Size = new System.Drawing.Size(79, 33);
            this.Park.TabIndex = 2;
            this.Park.Text = "Park";
            this.Park.Click += new System.EventHandler(this.Park_Click);
            // 
            // SyncScope
            // 
            this.SyncScope.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SyncScope.BackColor = System.Drawing.Color.Transparent;
            this.SyncScope.DialogResult = System.Windows.Forms.DialogResult.None;
            this.SyncScope.Enabled = false;
            this.SyncScope.Location = new System.Drawing.Point(338, 53);
            this.SyncScope.MaximumSize = new System.Drawing.Size(140, 33);
            this.SyncScope.Name = "SyncScope";
            this.SyncScope.Selected = false;
            this.SyncScope.Size = new System.Drawing.Size(79, 33);
            this.SyncScope.TabIndex = 2;
            this.SyncScope.Text = "Sync";
            this.SyncScope.Click += new System.EventHandler(this.SyncScope_Click);
            // 
            // ScopeWest
            // 
            this.ScopeWest.BackColor = System.Drawing.Color.Transparent;
            this.ScopeWest.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ScopeWest.Enabled = false;
            this.ScopeWest.Location = new System.Drawing.Point(174, 32);
            this.ScopeWest.MaximumSize = new System.Drawing.Size(140, 33);
            this.ScopeWest.Name = "ScopeWest";
            this.ScopeWest.Selected = false;
            this.ScopeWest.Size = new System.Drawing.Size(54, 33);
            this.ScopeWest.TabIndex = 2;
            this.ScopeWest.Text = "West";
            this.ScopeWest.Click += new System.EventHandler(this.ScopeWest_Click);
            // 
            // ScopeEast
            // 
            this.ScopeEast.BackColor = System.Drawing.Color.Transparent;
            this.ScopeEast.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ScopeEast.Enabled = false;
            this.ScopeEast.Location = new System.Drawing.Point(285, 33);
            this.ScopeEast.MaximumSize = new System.Drawing.Size(140, 33);
            this.ScopeEast.Name = "ScopeEast";
            this.ScopeEast.Selected = false;
            this.ScopeEast.Size = new System.Drawing.Size(54, 33);
            this.ScopeEast.TabIndex = 2;
            this.ScopeEast.Text = "East";
            this.ScopeEast.Click += new System.EventHandler(this.ScopeEast_Click);
            // 
            // ScopeSouth
            // 
            this.ScopeSouth.BackColor = System.Drawing.Color.Transparent;
            this.ScopeSouth.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ScopeSouth.Enabled = false;
            this.ScopeSouth.Location = new System.Drawing.Point(229, 56);
            this.ScopeSouth.MaximumSize = new System.Drawing.Size(140, 33);
            this.ScopeSouth.Name = "ScopeSouth";
            this.ScopeSouth.Selected = false;
            this.ScopeSouth.Size = new System.Drawing.Size(54, 33);
            this.ScopeSouth.TabIndex = 2;
            this.ScopeSouth.Text = "South";
            this.ScopeSouth.Click += new System.EventHandler(this.ScopeSouth_Click);
            // 
            // ScopeNorth
            // 
            this.ScopeNorth.BackColor = System.Drawing.Color.Transparent;
            this.ScopeNorth.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ScopeNorth.Enabled = false;
            this.ScopeNorth.Location = new System.Drawing.Point(229, 9);
            this.ScopeNorth.MaximumSize = new System.Drawing.Size(140, 33);
            this.ScopeNorth.Name = "ScopeNorth";
            this.ScopeNorth.Selected = false;
            this.ScopeNorth.Size = new System.Drawing.Size(54, 33);
            this.ScopeNorth.TabIndex = 2;
            this.ScopeNorth.Text = "North";
            this.ScopeNorth.Click += new System.EventHandler(this.ScopeNorth_Click);
            // 
            // SlewScope
            // 
            this.SlewScope.BackColor = System.Drawing.Color.Transparent;
            this.SlewScope.DialogResult = System.Windows.Forms.DialogResult.None;
            this.SlewScope.Enabled = false;
            this.SlewScope.Location = new System.Drawing.Point(89, 55);
            this.SlewScope.MaximumSize = new System.Drawing.Size(140, 33);
            this.SlewScope.Name = "SlewScope";
            this.SlewScope.Selected = false;
            this.SlewScope.Size = new System.Drawing.Size(79, 33);
            this.SlewScope.TabIndex = 2;
            this.SlewScope.Text = "Slew";
            this.SlewScope.Click += new System.EventHandler(this.SlewScope_Click);
            // 
            // CenterScope
            // 
            this.CenterScope.BackColor = System.Drawing.Color.Transparent;
            this.CenterScope.DialogResult = System.Windows.Forms.DialogResult.None;
            this.CenterScope.Enabled = false;
            this.CenterScope.Location = new System.Drawing.Point(5, 55);
            this.CenterScope.MaximumSize = new System.Drawing.Size(140, 33);
            this.CenterScope.Name = "CenterScope";
            this.CenterScope.Selected = false;
            this.CenterScope.Size = new System.Drawing.Size(79, 33);
            this.CenterScope.TabIndex = 2;
            this.CenterScope.Text = "Center";
            this.CenterScope.Click += new System.EventHandler(this.CenterScope_Click);
            // 
            // azText
            // 
            this.azText.BackColor = System.Drawing.Color.Transparent;
            this.azText.ForeColor = System.Drawing.Color.White;
            this.azText.Location = new System.Drawing.Point(77, 66);
            this.azText.Name = "azText";
            this.azText.Size = new System.Drawing.Size(79, 13);
            this.azText.TabIndex = 3;
            this.azText.Text = "00 : 00 : 00";
            this.azText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // altText
            // 
            this.altText.BackColor = System.Drawing.Color.Transparent;
            this.altText.ForeColor = System.Drawing.Color.White;
            this.altText.Location = new System.Drawing.Point(77, 50);
            this.altText.Name = "altText";
            this.altText.Size = new System.Drawing.Size(79, 13);
            this.altText.TabIndex = 3;
            this.altText.Text = "00 : 00 : 00";
            this.altText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(25, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(22, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Az:";
            // 
            // decText
            // 
            this.decText.BackColor = System.Drawing.Color.Transparent;
            this.decText.ForeColor = System.Drawing.Color.White;
            this.decText.Location = new System.Drawing.Point(77, 34);
            this.decText.Name = "decText";
            this.decText.Size = new System.Drawing.Size(79, 13);
            this.decText.TabIndex = 3;
            this.decText.Text = "00 : 00 : 00";
            this.decText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(25, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Alt:";
            // 
            // raText
            // 
            this.raText.BackColor = System.Drawing.Color.Transparent;
            this.raText.ForeColor = System.Drawing.Color.White;
            this.raText.Location = new System.Drawing.Point(77, 19);
            this.raText.Name = "raText";
            this.raText.Size = new System.Drawing.Size(79, 13);
            this.raText.TabIndex = 3;
            this.raText.Text = "00 : 00 : 00";
            this.raText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(25, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Dec:";
            // 
            // Choose
            // 
            this.Choose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Choose.BackColor = System.Drawing.Color.Transparent;
            this.Choose.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Choose.Location = new System.Drawing.Point(797, 35);
            this.Choose.MaximumSize = new System.Drawing.Size(140, 33);
            this.Choose.Name = "Choose";
            this.Choose.Selected = false;
            this.Choose.Size = new System.Drawing.Size(79, 33);
            this.Choose.TabIndex = 2;
            this.Choose.Text = "Choose";
            this.Choose.Click += new System.EventHandler(this.Choose_Click);
            // 
            // ConnectScope
            // 
            this.ConnectScope.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConnectScope.BackColor = System.Drawing.Color.Transparent;
            this.ConnectScope.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ConnectScope.Location = new System.Drawing.Point(797, 4);
            this.ConnectScope.MaximumSize = new System.Drawing.Size(140, 33);
            this.ConnectScope.Name = "ConnectScope";
            this.ConnectScope.Selected = false;
            this.ConnectScope.Size = new System.Drawing.Size(79, 33);
            this.ConnectScope.TabIndex = 4;
            this.ConnectScope.Text = "Connect";
            this.ConnectScope.Click += new System.EventHandler(this.ConnectScope_Click);
            // 
            // PlatformStatus
            // 
            this.PlatformStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PlatformStatus.BackColor = System.Drawing.Color.Transparent;
            this.PlatformStatus.ForeColor = System.Drawing.Color.White;
            this.PlatformStatus.Location = new System.Drawing.Point(874, 79);
            this.PlatformStatus.Name = "PlatformStatus";
            this.PlatformStatus.Size = new System.Drawing.Size(99, 16);
            this.PlatformStatus.TabIndex = 3;
            this.PlatformStatus.Text = "Not Installed";
            this.PlatformStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ScopeStatus
            // 
            this.ScopeStatus.BackColor = System.Drawing.Color.Transparent;
            this.ScopeStatus.Controls.Add(this.azText);
            this.ScopeStatus.Controls.Add(this.altText);
            this.ScopeStatus.Controls.Add(this.label4);
            this.ScopeStatus.Controls.Add(this.ra);
            this.ScopeStatus.Controls.Add(this.label1);
            this.ScopeStatus.Controls.Add(this.decText);
            this.ScopeStatus.Controls.Add(this.raText);
            this.ScopeStatus.Controls.Add(this.label2);
            this.ScopeStatus.ForeColor = System.Drawing.Color.White;
            this.ScopeStatus.Location = new System.Drawing.Point(12, 6);
            this.ScopeStatus.Name = "ScopeStatus";
            this.ScopeStatus.Size = new System.Drawing.Size(181, 92);
            this.ScopeStatus.TabIndex = 4;
            this.ScopeStatus.TabStop = false;
            this.ScopeStatus.Text = "Telescope Status";
            // 
            // TelescopeTimer
            // 
            this.TelescopeTimer.Enabled = true;
            this.TelescopeTimer.Interval = 500;
            this.TelescopeTimer.Tick += new System.EventHandler(this.TelescopeTimer_Tick);
            // 
            // TelescopeTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 110);
            this.Controls.Add(this.PlatformStatus);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.ConnectScope);
            this.Controls.Add(this.Setup);
            this.Controls.Add(this.Choose);
            this.Controls.Add(this.ScopeStatus);
            this.Controls.Add(this.telescopeGroup);
            this.Name = "TelescopeTab";
            this.Text = "TelescopeTab";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TelescopeTab_FormClosing);
            this.Load += new System.EventHandler(this.TelescopeTab_Load);
            this.Controls.SetChildIndex(this.telescopeGroup, 0);
            this.Controls.SetChildIndex(this.ScopeStatus, 0);
            this.Controls.SetChildIndex(this.Choose, 0);
            this.Controls.SetChildIndex(this.Setup, 0);
            this.Controls.SetChildIndex(this.ConnectScope, 0);
            this.Controls.SetChildIndex(this.pictureBox1, 0);
            this.Controls.SetChildIndex(this.pinUp, 0);
            this.Controls.SetChildIndex(this.PlatformStatus, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.telescopeGroup.ResumeLayout(false);
            this.ScopeStatus.ResumeLayout(false);
            this.ScopeStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private WwtButton Setup;
        private System.Windows.Forms.Label ra;
        private System.Windows.Forms.GroupBox telescopeGroup;
        private WwtButton ConnectScope;
        private WwtButton Choose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label azText;
        private System.Windows.Forms.Label altText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label decText;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label raText;
        private System.Windows.Forms.Label PlatformStatus;
        public WWTCheckbox TrackScope;
        private WwtButton CenterScope;
        private WwtButton SyncScope;
        private WwtButton SlewScope;
        private System.Windows.Forms.GroupBox ScopeStatus;
        private WwtButton Park;
        private WwtButton ScopeWest;
        private WwtButton ScopeEast;
        private WwtButton ScopeSouth;
        private WwtButton ScopeNorth;
        private System.Windows.Forms.Timer TelescopeTimer;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}