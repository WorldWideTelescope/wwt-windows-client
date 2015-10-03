namespace TerraViewer
{
    partial class ContextPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContextPanel));
            this.levelLabel = new System.Windows.Forms.Label();
            this.ConstellationLabel = new System.Windows.Forms.Label();
            this.queueProgressBar = new System.Windows.Forms.ProgressBar();
            this.searchTimer = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.layerToggle = new System.Windows.Forms.PictureBox();
            this.studyOpacity = new TerraViewer.WwtTrackBar();
            this.viewTarget = new TerraViewer.WwtCombo();
            this.ImageDataSetsCombo = new TerraViewer.WwtCombo();
            this.FilterCombo = new TerraViewer.WwtCombo();
            this.contextResults = new TerraViewer.ThumbnailList();
            this.paginator1 = new TerraViewer.Paginator();
            this.overview = new TerraViewer.Overview();
            this.SkyBall = new TerraViewer.SkyBall();
            this.closeBox = new System.Windows.Forms.PictureBox();
            this.faderText = new System.Windows.Forms.Label();
            this.FadeTimer = new System.Windows.Forms.Timer(this.components);
            this.trackingText = new System.Windows.Forms.Label();
            this.trackingLabel = new System.Windows.Forms.Label();
            this.actualSizeLabel = new System.Windows.Forms.Label();
            this.bigSizeLabel = new System.Windows.Forms.Label();
            this.scaleText = new System.Windows.Forms.Label();
            this.scaleLabel = new System.Windows.Forms.Label();
            this.scaleButton = new TerraViewer.WwtButton();
            this.info = new TerraViewer.WwtButton();
            this.solarSystemScaleTrackbar = new TerraViewer.WwtTrackBar();
            this.trackingTarget = new TerraViewer.ThumbnailList();
            this.pinUp1 = new TerraViewer.PinUp();
            this.pinUp = new TerraViewer.PinUp();
            this.Timeline = new TerraViewer.TimeLine();
            this.PushPin = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.layerToggle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PushPin)).BeginInit();
            this.SuspendLayout();
            // 
            // levelLabel
            // 
            this.levelLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.levelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.levelLabel.ForeColor = System.Drawing.Color.White;
            this.levelLabel.Location = new System.Drawing.Point(1094, 9);
            this.levelLabel.Name = "levelLabel";
            this.levelLabel.Size = new System.Drawing.Size(63, 12);
            this.levelLabel.TabIndex = 14;
            this.levelLabel.Text = "90:00:00";
            this.levelLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.toolTips.SetToolTip(this.levelLabel, "Field of view height in degrees : minutes : seconds of arc");
            // 
            // ConstellationLabel
            // 
            this.ConstellationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ConstellationLabel.AutoSize = true;
            this.ConstellationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConstellationLabel.ForeColor = System.Drawing.Color.White;
            this.ConstellationLabel.Location = new System.Drawing.Point(1037, 9);
            this.ConstellationLabel.Name = "ConstellationLabel";
            this.ConstellationLabel.Size = new System.Drawing.Size(51, 12);
            this.ConstellationLabel.TabIndex = 13;
            this.ConstellationLabel.Text = "Ursa Major";
            this.toolTips.SetToolTip(this.ConstellationLabel, "Name of the constellation that the view center is in.");
            this.ConstellationLabel.Click += new System.EventHandler(this.ConstellationLabel_Click);
            // 
            // queueProgressBar
            // 
            this.queueProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.queueProgressBar.Location = new System.Drawing.Point(1040, 101);
            this.queueProgressBar.Name = "queueProgressBar";
            this.queueProgressBar.Size = new System.Drawing.Size(115, 7);
            this.queueProgressBar.TabIndex = 16;
            this.toolTips.SetToolTip(this.queueProgressBar, "Download Progress");
            this.queueProgressBar.Value = 50;
            // 
            // searchTimer
            // 
            this.searchTimer.Enabled = true;
            this.searchTimer.Interval = 1000;
            this.searchTimer.Tick += new System.EventHandler(this.searchTimer_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(216, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Imagery";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(94, 1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Look At";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(721, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Context Search Filter";
            // 
            // toolTips
            // 
            this.toolTips.ShowAlways = true;
            // 
            // layerToggle
            // 
            this.layerToggle.Image = global::TerraViewer.Properties.Resources.layersButton;
            this.layerToggle.InitialImage = null;
            this.layerToggle.Location = new System.Drawing.Point(9, 6);
            this.layerToggle.Name = "layerToggle";
            this.layerToggle.Size = new System.Drawing.Size(75, 39);
            this.layerToggle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.layerToggle.TabIndex = 54;
            this.layerToggle.TabStop = false;
            this.toolTips.SetToolTip(this.layerToggle, "Show/Hide Layer Manager");
            this.layerToggle.Click += new System.EventHandler(this.layerToggle_Click);
            this.layerToggle.MouseEnter += new System.EventHandler(this.layerToggle_MouseEnter);
            this.layerToggle.MouseLeave += new System.EventHandler(this.layerToggle_MouseLeave);
            // 
            // studyOpacity
            // 
            this.studyOpacity.BackColor = System.Drawing.Color.Transparent;
            this.studyOpacity.Location = new System.Drawing.Point(436, 21);
            this.studyOpacity.Max = 100;
            this.studyOpacity.Name = "studyOpacity";
            this.studyOpacity.Size = new System.Drawing.Size(80, 20);
            this.studyOpacity.TabIndex = 17;
            this.toolTips.SetToolTip(this.studyOpacity, "Crossfades background and foreground imagery");
            this.studyOpacity.Value = 100;
            this.studyOpacity.Visible = false;
            this.studyOpacity.ValueChanged += new System.EventHandler(this.studyOpacity_Scroll);
            this.studyOpacity.Scroll += new System.Windows.Forms.ScrollEventHandler(this.studyOpacity_Scroll);
            this.studyOpacity.VisibleChanged += new System.EventHandler(this.studyOpacity_VisibleChanged);
            // 
            // viewTarget
            // 
            this.viewTarget.BackColor = System.Drawing.Color.Transparent;
            this.viewTarget.DateTimeValue = new System.DateTime(2008, 1, 23, 18, 40, 31, 673);
            this.viewTarget.Filter = TerraViewer.Classification.Unfiltered;
            this.viewTarget.FilterStyle = false;
            this.viewTarget.Location = new System.Drawing.Point(94, 15);
            this.viewTarget.Margin = new System.Windows.Forms.Padding(0);
            this.viewTarget.MasterTime = true;
            this.viewTarget.MaximumSize = new System.Drawing.Size(248, 33);
            this.viewTarget.MinimumSize = new System.Drawing.Size(35, 33);
            this.viewTarget.Name = "viewTarget";
            this.viewTarget.SelectedIndex = -1;
            this.viewTarget.SelectedItem = null;
            this.viewTarget.Size = new System.Drawing.Size(120, 33);
            this.viewTarget.State = TerraViewer.State.Rest;
            this.viewTarget.TabIndex = 4;
            this.toolTips.SetToolTip(this.viewTarget, "Select the type of view, Sky, Earth, etc.");
            this.viewTarget.Type = TerraViewer.WwtCombo.ComboType.List;
            this.viewTarget.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.viewTarget_SelectionChanged);
            // 
            // ImageDataSetsCombo
            // 
            this.ImageDataSetsCombo.BackColor = System.Drawing.Color.Transparent;
            this.ImageDataSetsCombo.DateTimeValue = new System.DateTime(2008, 1, 23, 18, 40, 31, 706);
            this.ImageDataSetsCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.ImageDataSetsCombo.FilterStyle = false;
            this.ImageDataSetsCombo.Location = new System.Drawing.Point(216, 15);
            this.ImageDataSetsCombo.Margin = new System.Windows.Forms.Padding(0);
            this.ImageDataSetsCombo.MasterTime = true;
            this.ImageDataSetsCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.ImageDataSetsCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.ImageDataSetsCombo.Name = "ImageDataSetsCombo";
            this.ImageDataSetsCombo.SelectedIndex = -1;
            this.ImageDataSetsCombo.SelectedItem = null;
            this.ImageDataSetsCombo.Size = new System.Drawing.Size(184, 33);
            this.ImageDataSetsCombo.State = TerraViewer.State.Rest;
            this.ImageDataSetsCombo.TabIndex = 6;
            this.toolTips.SetToolTip(this.ImageDataSetsCombo, "Select the imagery to display");
            this.ImageDataSetsCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.ImageDataSetsCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.ImageDataSetsCombo_SelectionChanged);
            // 
            // FilterCombo
            // 
            this.FilterCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FilterCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.FilterCombo.DateTimeValue = new System.DateTime(2008, 1, 23, 18, 40, 31, 738);
            this.FilterCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.FilterCombo.FilterStyle = true;
            this.FilterCombo.Location = new System.Drawing.Point(722, 15);
            this.FilterCombo.Margin = new System.Windows.Forms.Padding(0);
            this.FilterCombo.MasterTime = true;
            this.FilterCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.FilterCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.FilterCombo.Name = "FilterCombo";
            this.FilterCombo.SelectedIndex = -1;
            this.FilterCombo.SelectedItem = null;
            this.FilterCombo.Size = new System.Drawing.Size(121, 33);
            this.FilterCombo.State = TerraViewer.State.Rest;
            this.FilterCombo.TabIndex = 2;
            this.FilterCombo.Tag = TerraViewer.Classification.Unfiltered;
            this.toolTips.SetToolTip(this.FilterCombo, "Filters context results");
            this.FilterCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.FilterCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.FilterCombo_FilterChanged);
            // 
            // contextResults
            // 
            this.contextResults.AddText = "Add New Item";
            this.contextResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contextResults.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.contextResults.ColCount = 8;
            this.contextResults.DontStealFocus = false;
            this.contextResults.EmptyAddText = "No Results";
            this.contextResults.Items = ((System.Collections.Generic.List<object>)(resources.GetObject("contextResults.Items")));
            this.contextResults.Location = new System.Drawing.Point(9, 48);
            this.contextResults.Margin = new System.Windows.Forms.Padding(0);
            this.contextResults.MaximumSize = new System.Drawing.Size(4096, 485);
            this.contextResults.MinimumSize = new System.Drawing.Size(100, 65);
            this.contextResults.Name = "contextResults";
            this.contextResults.Paginator = this.paginator1;
            this.contextResults.RowCount = 1;
            this.contextResults.ShowAddButton = false;
            this.contextResults.Size = new System.Drawing.Size(920, 65);
            this.contextResults.TabIndex = 12;
            this.contextResults.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            this.toolTips.SetToolTip(this.contextResults, "Context Search - Shows interesting places in the current view.");
            this.contextResults.ItemHover += new TerraViewer.ItemClickedEventHandler(this.contextResults_ItemHover);
            this.contextResults.ItemClicked += new TerraViewer.ItemClickedEventHandler(this.contextResults_ItemClicked);
            this.contextResults.ItemDoubleClicked += new TerraViewer.ItemClickedEventHandler(this.contextResults_ItemDoubleClicked);
            this.contextResults.ItemImageClicked += new TerraViewer.ItemClickedEventHandler(this.contextResults_ItemImageClicked);
            this.contextResults.ItemContextMenu += new TerraViewer.ItemClickedEventHandler(this.contextResults_ItemContextMenu);
            this.contextResults.Load += new System.EventHandler(this.contextResults_Load);
            // 
            // paginator1
            // 
            this.paginator1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.paginator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.paginator1.CurrentPage = 0;
            this.paginator1.Location = new System.Drawing.Point(844, 24);
            this.paginator1.Margin = new System.Windows.Forms.Padding(0);
            this.paginator1.Name = "paginator1";
            this.paginator1.Size = new System.Drawing.Size(85, 16);
            this.paginator1.TabIndex = 10;
            this.paginator1.TotalPages = 1;
            // 
            // overview
            // 
            this.overview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.overview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.overview.Constellation = "";
            this.overview.ForeColor = System.Drawing.Color.White;
            this.overview.Location = new System.Drawing.Point(1040, 25);
            this.overview.Margin = new System.Windows.Forms.Padding(0);
            this.overview.MaximumSize = new System.Drawing.Size(115, 66);
            this.overview.MinimumSize = new System.Drawing.Size(115, 66);
            this.overview.Name = "overview";
            this.overview.Size = new System.Drawing.Size(115, 66);
            this.overview.TabIndex = 15;
            this.toolTips.SetToolTip(this.overview, "Constellation Overview");
            this.overview.Click += new System.EventHandler(this.overview_Click);
            // 
            // SkyBall
            // 
            this.SkyBall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.SkyBall.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SkyBall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.SkyBall.Dec = 0D;
            this.SkyBall.ForeColor = System.Drawing.Color.White;
            this.SkyBall.Location = new System.Drawing.Point(935, 1);
            this.SkyBall.Margin = new System.Windows.Forms.Padding(0);
            this.SkyBall.MaximumSize = new System.Drawing.Size(96, 120);
            this.SkyBall.MinimumSize = new System.Drawing.Size(96, 50);
            this.SkyBall.Name = "SkyBall";
            this.SkyBall.RA = 0D;
            this.SkyBall.ShowSkyball = true;
            this.SkyBall.Size = new System.Drawing.Size(96, 120);
            this.SkyBall.Space = true;
            this.SkyBall.TabIndex = 11;
            this.toolTips.SetToolTip(this.SkyBall, "Shows field of view relative to the celestial sphere");
            this.SkyBall.Load += new System.EventHandler(this.SkyBall_Load);
            // 
            // closeBox
            // 
            this.closeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBox.Image = global::TerraViewer.Properties.Resources.CloseRest;
            this.closeBox.Location = new System.Drawing.Point(1146, 1);
            this.closeBox.Name = "closeBox";
            this.closeBox.Size = new System.Drawing.Size(16, 14);
            this.closeBox.TabIndex = 56;
            this.closeBox.TabStop = false;
            this.toolTips.SetToolTip(this.closeBox, "Hide Timeline");
            this.closeBox.Click += new System.EventHandler(this.closeBox_Click);
            // 
            // faderText
            // 
            this.faderText.AutoSize = true;
            this.faderText.BackColor = System.Drawing.Color.Transparent;
            this.faderText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.faderText.ForeColor = System.Drawing.Color.White;
            this.faderText.Location = new System.Drawing.Point(436, 2);
            this.faderText.Name = "faderText";
            this.faderText.Size = new System.Drawing.Size(92, 13);
            this.faderText.TabIndex = 5;
            this.faderText.Text = "Image Crossfade";
            this.faderText.Visible = false;
            // 
            // FadeTimer
            // 
            this.FadeTimer.Interval = 10;
            this.FadeTimer.Tick += new System.EventHandler(this.FadeTimer_Tick);
            // 
            // trackingText
            // 
            this.trackingText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackingText.AutoEllipsis = true;
            this.trackingText.BackColor = System.Drawing.Color.Transparent;
            this.trackingText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackingText.ForeColor = System.Drawing.Color.White;
            this.trackingText.Location = new System.Drawing.Point(585, 21);
            this.trackingText.Name = "trackingText";
            this.trackingText.Size = new System.Drawing.Size(110, 20);
            this.trackingText.TabIndex = 5;
            this.trackingText.Text = "Tracking";
            // 
            // trackingLabel
            // 
            this.trackingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trackingLabel.AutoSize = true;
            this.trackingLabel.BackColor = System.Drawing.Color.Transparent;
            this.trackingLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trackingLabel.ForeColor = System.Drawing.Color.White;
            this.trackingLabel.Location = new System.Drawing.Point(585, 1);
            this.trackingLabel.Name = "trackingLabel";
            this.trackingLabel.Size = new System.Drawing.Size(50, 13);
            this.trackingLabel.TabIndex = 5;
            this.trackingLabel.Text = "Tracking";
            this.trackingLabel.Visible = false;
            // 
            // actualSizeLabel
            // 
            this.actualSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.actualSizeLabel.AutoSize = true;
            this.actualSizeLabel.BackColor = System.Drawing.Color.Transparent;
            this.actualSizeLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.actualSizeLabel.ForeColor = System.Drawing.Color.White;
            this.actualSizeLabel.Location = new System.Drawing.Point(941, 45);
            this.actualSizeLabel.Name = "actualSizeLabel";
            this.actualSizeLabel.Size = new System.Drawing.Size(39, 13);
            this.actualSizeLabel.TabIndex = 21;
            this.actualSizeLabel.Text = "Actual";
            this.actualSizeLabel.Visible = false;
            // 
            // bigSizeLabel
            // 
            this.bigSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bigSizeLabel.AutoSize = true;
            this.bigSizeLabel.BackColor = System.Drawing.Color.Transparent;
            this.bigSizeLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bigSizeLabel.ForeColor = System.Drawing.Color.White;
            this.bigSizeLabel.Location = new System.Drawing.Point(1005, 45);
            this.bigSizeLabel.Name = "bigSizeLabel";
            this.bigSizeLabel.Size = new System.Drawing.Size(35, 13);
            this.bigSizeLabel.TabIndex = 21;
            this.bigSizeLabel.Text = "Large";
            this.bigSizeLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.bigSizeLabel.Visible = false;
            // 
            // scaleText
            // 
            this.scaleText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.scaleText.AutoSize = true;
            this.scaleText.BackColor = System.Drawing.Color.Transparent;
            this.scaleText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scaleText.ForeColor = System.Drawing.Color.White;
            this.scaleText.Location = new System.Drawing.Point(959, 5);
            this.scaleText.Name = "scaleText";
            this.scaleText.Size = new System.Drawing.Size(62, 13);
            this.scaleText.TabIndex = 52;
            this.scaleText.Text = "Planet Size";
            this.scaleText.Visible = false;
            // 
            // scaleLabel
            // 
            this.scaleLabel.AutoSize = true;
            this.scaleLabel.BackColor = System.Drawing.Color.Transparent;
            this.scaleLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scaleLabel.ForeColor = System.Drawing.Color.White;
            this.scaleLabel.Location = new System.Drawing.Point(540, 2);
            this.scaleLabel.Name = "scaleLabel";
            this.scaleLabel.Size = new System.Drawing.Size(33, 13);
            this.scaleLabel.TabIndex = 5;
            this.scaleLabel.Text = "Scale";
            this.scaleLabel.Visible = false;
            // 
            // scaleButton
            // 
            this.scaleButton.BackColor = System.Drawing.Color.Transparent;
            this.scaleButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.scaleButton.ImageDisabled = null;
            this.scaleButton.ImageEnabled = global::TerraViewer.Properties.Resources.hist;
            this.scaleButton.Location = new System.Drawing.Point(534, 14);
            this.scaleButton.MaximumSize = new System.Drawing.Size(140, 33);
            this.scaleButton.Name = "scaleButton";
            this.scaleButton.Selected = false;
            this.scaleButton.Size = new System.Drawing.Size(52, 33);
            this.scaleButton.TabIndex = 53;
            this.scaleButton.Visible = false;
            this.scaleButton.Click += new System.EventHandler(this.scaleButton_Click);
            // 
            // info
            // 
            this.info.BackColor = System.Drawing.Color.Transparent;
            this.info.DialogResult = System.Windows.Forms.DialogResult.None;
            this.info.ImageDisabled = null;
            this.info.ImageEnabled = global::TerraViewer.Properties.Resources.info;
            this.info.Location = new System.Drawing.Point(399, 14);
            this.info.MaximumSize = new System.Drawing.Size(140, 33);
            this.info.Name = "info";
            this.info.Selected = false;
            this.info.Size = new System.Drawing.Size(38, 33);
            this.info.TabIndex = 18;
            this.info.Click += new System.EventHandler(this.info_Click);
            // 
            // solarSystemScaleTrackbar
            // 
            this.solarSystemScaleTrackbar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.solarSystemScaleTrackbar.BackColor = System.Drawing.Color.Transparent;
            this.solarSystemScaleTrackbar.Location = new System.Drawing.Point(953, 21);
            this.solarSystemScaleTrackbar.Max = 100;
            this.solarSystemScaleTrackbar.Name = "solarSystemScaleTrackbar";
            this.solarSystemScaleTrackbar.Size = new System.Drawing.Size(87, 20);
            this.solarSystemScaleTrackbar.TabIndex = 20;
            this.solarSystemScaleTrackbar.Value = 100;
            this.solarSystemScaleTrackbar.Visible = false;
            this.solarSystemScaleTrackbar.ValueChanged += new System.EventHandler(this.solarSystemScaleTrackbar_ValueChanged);
            // 
            // trackingTarget
            // 
            this.trackingTarget.AddText = "Add New Item";
            this.trackingTarget.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.trackingTarget.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.trackingTarget.ColCount = 1;
            this.trackingTarget.DontStealFocus = false;
            this.trackingTarget.EmptyAddText = "No Results";
            this.trackingTarget.Items = ((System.Collections.Generic.List<object>)(resources.GetObject("trackingTarget.Items")));
            this.trackingTarget.Location = new System.Drawing.Point(1055, 25);
            this.trackingTarget.Margin = new System.Windows.Forms.Padding(0);
            this.trackingTarget.MaximumSize = new System.Drawing.Size(2500, 475);
            this.trackingTarget.MinimumSize = new System.Drawing.Size(100, 65);
            this.trackingTarget.Name = "trackingTarget";
            this.trackingTarget.Paginator = null;
            this.trackingTarget.RowCount = 1;
            this.trackingTarget.ShowAddButton = false;
            this.trackingTarget.Size = new System.Drawing.Size(100, 65);
            this.trackingTarget.TabIndex = 19;
            this.trackingTarget.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            this.trackingTarget.Visible = false;
            // 
            // pinUp1
            // 
            this.pinUp1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pinUp1.BackColor = System.Drawing.Color.Transparent;
            this.pinUp1.Direction = TerraViewer.Direction.Collapsing;
            this.pinUp1.Location = new System.Drawing.Point(887, 1);
            this.pinUp1.Margin = new System.Windows.Forms.Padding(0);
            this.pinUp1.MaximumSize = new System.Drawing.Size(33, 12);
            this.pinUp1.MinimumSize = new System.Drawing.Size(33, 12);
            this.pinUp1.Name = "pinUp1";
            this.pinUp1.Placement = TerraViewer.Placement.Bottom;
            this.pinUp1.Size = new System.Drawing.Size(33, 12);
            this.pinUp1.State = TerraViewer.State.Rest;
            this.pinUp1.TabIndex = 7;
            this.pinUp1.Clicked += new TerraViewer.ClickedEventHandler(this.pinUp1_Clicked);
            // 
            // pinUp
            // 
            this.pinUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pinUp.BackColor = System.Drawing.Color.Transparent;
            this.pinUp.Direction = TerraViewer.Direction.Expanding;
            this.pinUp.Location = new System.Drawing.Point(855, 1);
            this.pinUp.Margin = new System.Windows.Forms.Padding(0);
            this.pinUp.MaximumSize = new System.Drawing.Size(33, 12);
            this.pinUp.MinimumSize = new System.Drawing.Size(33, 12);
            this.pinUp.Name = "pinUp";
            this.pinUp.Placement = TerraViewer.Placement.Bottom;
            this.pinUp.Size = new System.Drawing.Size(33, 12);
            this.pinUp.State = TerraViewer.State.Rest;
            this.pinUp.TabIndex = 7;
            this.pinUp.Clicked += new TerraViewer.ClickedEventHandler(this.pinUp_Clicked_1);
            // 
            // Timeline
            // 
            this.Timeline.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Timeline.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.Timeline.ForeColor = System.Drawing.Color.White;
            this.Timeline.Location = new System.Drawing.Point(6, 15);
            this.Timeline.MinimumSize = new System.Drawing.Size(300, 90);
            this.Timeline.Name = "Timeline";
            this.Timeline.Size = new System.Drawing.Size(1151, 98);
            this.Timeline.TabIndex = 55;
            this.Timeline.Tour = null;
            this.Timeline.TweenPosition = 0D;
            this.Timeline.Visible = false;
            // 
            // PushPin
            // 
            this.PushPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PushPin.Image = global::TerraViewer.Properties.Resources.pushpin;
            this.PushPin.Location = new System.Drawing.Point(1133, 1);
            this.PushPin.Name = "PushPin";
            this.PushPin.Size = new System.Drawing.Size(11, 14);
            this.PushPin.TabIndex = 57;
            this.PushPin.TabStop = false;
            this.PushPin.Visible = false;
            this.PushPin.Click += new System.EventHandler(this.PushPin_Click);
            // 
            // ContextPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(1164, 120);
            this.Controls.Add(this.PushPin);
            this.Controls.Add(this.closeBox);
            this.Controls.Add(this.layerToggle);
            this.Controls.Add(this.scaleButton);
            this.Controls.Add(this.scaleText);
            this.Controls.Add(this.bigSizeLabel);
            this.Controls.Add(this.actualSizeLabel);
            this.Controls.Add(this.solarSystemScaleTrackbar);
            this.Controls.Add(this.trackingTarget);
            this.Controls.Add(this.info);
            this.Controls.Add(this.studyOpacity);
            this.Controls.Add(this.viewTarget);
            this.Controls.Add(this.ImageDataSetsCombo);
            this.Controls.Add(this.pinUp1);
            this.Controls.Add(this.pinUp);
            this.Controls.Add(this.FilterCombo);
            this.Controls.Add(this.paginator1);
            this.Controls.Add(this.contextResults);
            this.Controls.Add(this.overview);
            this.Controls.Add(this.levelLabel);
            this.Controls.Add(this.ConstellationLabel);
            this.Controls.Add(this.queueProgressBar);
            this.Controls.Add(this.SkyBall);
            this.Controls.Add(this.trackingText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.trackingLabel);
            this.Controls.Add(this.scaleLabel);
            this.Controls.Add(this.faderText);
            this.Controls.Add(this.Timeline);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ContextPanel";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Context Pane";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ContextPanel_FormClosing);
            this.Load += new System.EventHandler(this.ContextPanel_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ContextPanel_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.layerToggle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PushPin)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PinUp pinUp;
        private WwtCombo FilterCombo;
        private Paginator paginator1;
        protected Overview overview;
        private System.Windows.Forms.Label levelLabel;
        private System.Windows.Forms.Label ConstellationLabel;
        private System.Windows.Forms.ProgressBar queueProgressBar;
        public SkyBall SkyBall;
        private System.Windows.Forms.Timer searchTimer;
        private WwtCombo ImageDataSetsCombo;
        private System.Windows.Forms.Label label1;
        private WwtCombo viewTarget;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTips;
        public WwtTrackBar studyOpacity;
        public System.Windows.Forms.Label faderText;
        private System.Windows.Forms.Timer FadeTimer;
        public System.Windows.Forms.Label trackingText;
        public System.Windows.Forms.Label trackingLabel;
        private PinUp pinUp1;
        private WwtButton info;
        private ThumbnailList trackingTarget;
        private WwtTrackBar solarSystemScaleTrackbar;
        public System.Windows.Forms.Label actualSizeLabel;
        public System.Windows.Forms.Label bigSizeLabel;
        public System.Windows.Forms.Label scaleText;
        public System.Windows.Forms.Label scaleLabel;
        public WwtButton scaleButton;
        public ThumbnailList contextResults;
        private System.Windows.Forms.PictureBox layerToggle;
        private TimeLine Timeline;
        private System.Windows.Forms.PictureBox closeBox;
        private System.Windows.Forms.PictureBox PushPin;

    }
}