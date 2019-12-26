namespace TerraViewer
{
    partial class VOTableViewer
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.raSourceLabel = new System.Windows.Forms.Label();
            this.decSourceLabel = new System.Windows.Forms.Label();
            this.distanceSouceLabel = new System.Windows.Forms.Label();
            this.typeSourceLabel = new System.Windows.Forms.Label();
            this.sizeSourceLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.loadImage = new TerraViewer.WwtButton();
            this.markerTypeCombo = new TerraViewer.WwtCombo();
            this.sizeSource = new TerraViewer.WwtCombo();
            this.typeSource = new TerraViewer.WwtCombo();
            this.distanceSource = new TerraViewer.WwtCombo();
            this.decSource = new TerraViewer.WwtCombo();
            this.raSource = new TerraViewer.WwtCombo();
            this.sync = new TerraViewer.WwtButton();
            this.save = new TerraViewer.WwtButton();
            this.refreshTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.listView1.CheckBoxes = true;
            this.listView1.ForeColor = System.Drawing.Color.White;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 102);
            this.listView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(1366, 602);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.VirtualMode = true;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked);
            this.listView1.ItemMouseHover += new System.Windows.Forms.ListViewItemMouseHoverEventHandler(this.listView1_ItemMouseHover);
            this.listView1.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.listView1_RetrieveVirtualItem);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.Click += new System.EventHandler(this.listView1_Click);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // raSourceLabel
            // 
            this.raSourceLabel.AutoSize = true;
            this.raSourceLabel.Location = new System.Drawing.Point(184, 15);
            this.raSourceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.raSourceLabel.Name = "raSourceLabel";
            this.raSourceLabel.Size = new System.Drawing.Size(87, 20);
            this.raSourceLabel.TabIndex = 3;
            this.raSourceLabel.Text = "RA Source";
            // 
            // decSourceLabel
            // 
            this.decSourceLabel.AutoSize = true;
            this.decSourceLabel.Location = new System.Drawing.Point(357, 15);
            this.decSourceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.decSourceLabel.Name = "decSourceLabel";
            this.decSourceLabel.Size = new System.Drawing.Size(93, 20);
            this.decSourceLabel.TabIndex = 3;
            this.decSourceLabel.Text = "Dec Source";
            // 
            // distanceSouceLabel
            // 
            this.distanceSouceLabel.AutoSize = true;
            this.distanceSouceLabel.Location = new System.Drawing.Point(530, 15);
            this.distanceSouceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.distanceSouceLabel.Name = "distanceSouceLabel";
            this.distanceSouceLabel.Size = new System.Drawing.Size(127, 20);
            this.distanceSouceLabel.TabIndex = 3;
            this.distanceSouceLabel.Text = "Distance Source";
            // 
            // typeSourceLabel
            // 
            this.typeSourceLabel.AutoSize = true;
            this.typeSourceLabel.Location = new System.Drawing.Point(699, 14);
            this.typeSourceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.typeSourceLabel.Name = "typeSourceLabel";
            this.typeSourceLabel.Size = new System.Drawing.Size(98, 20);
            this.typeSourceLabel.TabIndex = 3;
            this.typeSourceLabel.Text = "Type Source";
            // 
            // sizeSourceLabel
            // 
            this.sizeSourceLabel.AutoSize = true;
            this.sizeSourceLabel.Location = new System.Drawing.Point(867, 14);
            this.sizeSourceLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.sizeSourceLabel.Name = "sizeSourceLabel";
            this.sizeSourceLabel.Size = new System.Drawing.Size(130, 20);
            this.sizeSourceLabel.TabIndex = 3;
            this.sizeSourceLabel.Text = "Size/Mag Source";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Marker Type";
            // 
            // loadImage
            // 
            this.loadImage.BackColor = System.Drawing.Color.Transparent;
            this.loadImage.DialogResult = System.Windows.Forms.DialogResult.None;
            this.loadImage.ImageDisabled = null;
            this.loadImage.ImageEnabled = null;
            this.loadImage.Location = new System.Drawing.Point(1059, 49);
            this.loadImage.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.loadImage.MaximumSize = new System.Drawing.Size(210, 51);
            this.loadImage.Name = "loadImage";
            this.loadImage.Selected = false;
            this.loadImage.Size = new System.Drawing.Size(170, 51);
            this.loadImage.TabIndex = 5;
            this.loadImage.Text = "Load Image";
            this.loadImage.Visible = false;
            this.loadImage.Click += new System.EventHandler(this.loadImage_Click);
            // 
            // markerTypeCombo
            // 
            this.markerTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.markerTypeCombo.DateTimeValue = new System.DateTime(2008, 11, 1, 21, 10, 58, 878);
            this.markerTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.markerTypeCombo.FilterStyle = false;
            this.markerTypeCombo.Location = new System.Drawing.Point(14, 40);
            this.markerTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.markerTypeCombo.MasterTime = true;
            this.markerTypeCombo.MaximumSize = new System.Drawing.Size(372, 51);
            this.markerTypeCombo.MinimumSize = new System.Drawing.Size(52, 51);
            this.markerTypeCombo.Name = "markerTypeCombo";
            this.markerTypeCombo.SelectedIndex = -1;
            this.markerTypeCombo.SelectedItem = null;
            this.markerTypeCombo.Size = new System.Drawing.Size(159, 51);
            this.markerTypeCombo.State = TerraViewer.State.Rest;
            this.markerTypeCombo.TabIndex = 4;
            this.markerTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.markerTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.markerTypeCombo_SelectionChanged);
            // 
            // sizeSource
            // 
            this.sizeSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.sizeSource.DateTimeValue = new System.DateTime(2008, 11, 1, 21, 10, 58, 878);
            this.sizeSource.Filter = TerraViewer.Classification.Unfiltered;
            this.sizeSource.FilterStyle = false;
            this.sizeSource.Location = new System.Drawing.Point(872, 40);
            this.sizeSource.Margin = new System.Windows.Forms.Padding(0);
            this.sizeSource.MasterTime = true;
            this.sizeSource.MaximumSize = new System.Drawing.Size(372, 51);
            this.sizeSource.MinimumSize = new System.Drawing.Size(52, 51);
            this.sizeSource.Name = "sizeSource";
            this.sizeSource.SelectedIndex = -1;
            this.sizeSource.SelectedItem = null;
            this.sizeSource.Size = new System.Drawing.Size(159, 51);
            this.sizeSource.State = TerraViewer.State.Rest;
            this.sizeSource.TabIndex = 4;
            this.sizeSource.Type = TerraViewer.WwtCombo.ComboType.List;
            this.sizeSource.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.sizeSource_SelectionChanged);
            // 
            // typeSource
            // 
            this.typeSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.typeSource.DateTimeValue = new System.DateTime(2008, 11, 1, 21, 10, 58, 878);
            this.typeSource.Filter = TerraViewer.Classification.Unfiltered;
            this.typeSource.FilterStyle = false;
            this.typeSource.Location = new System.Drawing.Point(704, 40);
            this.typeSource.Margin = new System.Windows.Forms.Padding(0);
            this.typeSource.MasterTime = true;
            this.typeSource.MaximumSize = new System.Drawing.Size(372, 51);
            this.typeSource.MinimumSize = new System.Drawing.Size(52, 51);
            this.typeSource.Name = "typeSource";
            this.typeSource.SelectedIndex = -1;
            this.typeSource.SelectedItem = null;
            this.typeSource.Size = new System.Drawing.Size(159, 51);
            this.typeSource.State = TerraViewer.State.Rest;
            this.typeSource.TabIndex = 4;
            this.typeSource.Type = TerraViewer.WwtCombo.ComboType.List;
            this.typeSource.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.typeSource_SelectionChanged);
            // 
            // distanceSource
            // 
            this.distanceSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.distanceSource.DateTimeValue = new System.DateTime(2008, 11, 1, 21, 10, 58, 878);
            this.distanceSource.Filter = TerraViewer.Classification.Unfiltered;
            this.distanceSource.FilterStyle = false;
            this.distanceSource.Location = new System.Drawing.Point(534, 42);
            this.distanceSource.Margin = new System.Windows.Forms.Padding(0);
            this.distanceSource.MasterTime = true;
            this.distanceSource.MaximumSize = new System.Drawing.Size(372, 51);
            this.distanceSource.MinimumSize = new System.Drawing.Size(52, 51);
            this.distanceSource.Name = "distanceSource";
            this.distanceSource.SelectedIndex = -1;
            this.distanceSource.SelectedItem = null;
            this.distanceSource.Size = new System.Drawing.Size(159, 51);
            this.distanceSource.State = TerraViewer.State.Rest;
            this.distanceSource.TabIndex = 4;
            this.distanceSource.Type = TerraViewer.WwtCombo.ComboType.List;
            this.distanceSource.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.distanceSource_SelectionChanged);
            // 
            // decSource
            // 
            this.decSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.decSource.DateTimeValue = new System.DateTime(2008, 11, 1, 21, 10, 58, 878);
            this.decSource.Filter = TerraViewer.Classification.Unfiltered;
            this.decSource.FilterStyle = false;
            this.decSource.Location = new System.Drawing.Point(362, 42);
            this.decSource.Margin = new System.Windows.Forms.Padding(0);
            this.decSource.MasterTime = true;
            this.decSource.MaximumSize = new System.Drawing.Size(372, 51);
            this.decSource.MinimumSize = new System.Drawing.Size(52, 51);
            this.decSource.Name = "decSource";
            this.decSource.SelectedIndex = -1;
            this.decSource.SelectedItem = null;
            this.decSource.Size = new System.Drawing.Size(159, 51);
            this.decSource.State = TerraViewer.State.Rest;
            this.decSource.TabIndex = 4;
            this.decSource.Type = TerraViewer.WwtCombo.ComboType.List;
            this.decSource.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.decSource_SelectionChanged);
            // 
            // raSource
            // 
            this.raSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.raSource.DateTimeValue = new System.DateTime(2008, 11, 1, 21, 10, 58, 878);
            this.raSource.Filter = TerraViewer.Classification.Unfiltered;
            this.raSource.FilterStyle = false;
            this.raSource.Location = new System.Drawing.Point(189, 42);
            this.raSource.Margin = new System.Windows.Forms.Padding(0);
            this.raSource.MasterTime = true;
            this.raSource.MaximumSize = new System.Drawing.Size(372, 51);
            this.raSource.MinimumSize = new System.Drawing.Size(52, 51);
            this.raSource.Name = "raSource";
            this.raSource.SelectedIndex = -1;
            this.raSource.SelectedItem = null;
            this.raSource.Size = new System.Drawing.Size(159, 51);
            this.raSource.State = TerraViewer.State.Rest;
            this.raSource.TabIndex = 4;
            this.raSource.Type = TerraViewer.WwtCombo.ComboType.List;
            this.raSource.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.raSource_SelectionChanged);
            // 
            // sync
            // 
            this.sync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sync.BackColor = System.Drawing.Color.Transparent;
            this.sync.DialogResult = System.Windows.Forms.DialogResult.None;
            this.sync.ImageDisabled = null;
            this.sync.ImageEnabled = null;
            this.sync.Location = new System.Drawing.Point(1224, 49);
            this.sync.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.sync.MaximumSize = new System.Drawing.Size(210, 51);
            this.sync.Name = "sync";
            this.sync.Selected = false;
            this.sync.Size = new System.Drawing.Size(144, 51);
            this.sync.TabIndex = 1;
            this.sync.Text = "Broadcast";
            this.sync.Click += new System.EventHandler(this.sync_Click);
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.save.BackColor = System.Drawing.Color.Transparent;
            this.save.DialogResult = System.Windows.Forms.DialogResult.None;
            this.save.ImageDisabled = null;
            this.save.ImageEnabled = null;
            this.save.Location = new System.Drawing.Point(1224, 0);
            this.save.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.save.MaximumSize = new System.Drawing.Size(210, 51);
            this.save.Name = "save";
            this.save.Selected = false;
            this.save.Size = new System.Drawing.Size(144, 51);
            this.save.TabIndex = 1;
            this.save.Text = "Save As...";
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // refreshTimer
            // 
            this.refreshTimer.Enabled = true;
            this.refreshTimer.Interval = 500;
            this.refreshTimer.Tick += new System.EventHandler(this.refreshTimer_Tick);
            // 
            // VOTableViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(1368, 706);
            this.Controls.Add(this.loadImage);
            this.Controls.Add(this.markerTypeCombo);
            this.Controls.Add(this.sizeSource);
            this.Controls.Add(this.typeSource);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.distanceSource);
            this.Controls.Add(this.sizeSourceLabel);
            this.Controls.Add(this.typeSourceLabel);
            this.Controls.Add(this.decSource);
            this.Controls.Add(this.distanceSouceLabel);
            this.Controls.Add(this.raSource);
            this.Controls.Add(this.decSourceLabel);
            this.Controls.Add(this.raSourceLabel);
            this.Controls.Add(this.sync);
            this.Controls.Add(this.save);
            this.Controls.Add(this.listView1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "VOTableViewer";
            this.ShowIcon = false;
            this.Text = "Microsoft WorldWide Telescope - VO Table Viewer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.VOTableViewer_FormClosed);
            this.Load += new System.EventHandler(this.VOTableViewer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private WwtButton save;
        private System.Windows.Forms.Label raSourceLabel;
        private System.Windows.Forms.Label decSourceLabel;
        private WwtCombo raSource;
        private WwtCombo decSource;
        private System.Windows.Forms.Label distanceSouceLabel;
        private WwtCombo distanceSource;
        private System.Windows.Forms.Label typeSourceLabel;
        private WwtCombo typeSource;
        private System.Windows.Forms.Label sizeSourceLabel;
        private WwtCombo sizeSource;
        private WwtButton sync;
        private System.Windows.Forms.Label label1;
        private WwtCombo markerTypeCombo;
        private WwtButton loadImage;
        private System.Windows.Forms.Timer refreshTimer;
    }
}