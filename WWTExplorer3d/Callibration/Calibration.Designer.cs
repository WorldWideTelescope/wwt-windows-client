namespace TerraViewer.Callibration
{
    partial class Calibration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Calibration));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ProjectorList = new System.Windows.Forms.ListBox();
            this.AddProjector = new TerraViewer.WwtButton();
            this.EditProjector = new TerraViewer.WwtButton();
            this.DeleteProjector = new TerraViewer.WwtButton();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Align = new TerraViewer.Tab();
            this.label2 = new System.Windows.Forms.Label();
            this.PointWeightLabel = new System.Windows.Forms.Label();
            this.Blend = new TerraViewer.Tab();
            this.WeightTrackBar = new TerraViewer.WwtTrackBar();
            this.AddPoint = new TerraViewer.WwtButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.PointTree = new System.Windows.Forms.TreeView();
            this.PointsTreeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transferFromEdgesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextSeperator2 = new System.Windows.Forms.ToolStripSeparator();
            this.blendPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BlendPanelButtons = new System.Windows.Forms.Panel();
            this.blueAmount = new System.Windows.Forms.Label();
            this.greenAmount = new System.Windows.Forms.Label();
            this.redAmount = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.blueSlider = new TerraViewer.WwtTrackBar();
            this.label10 = new System.Windows.Forms.Label();
            this.greenSlider = new TerraViewer.WwtTrackBar();
            this.label9 = new System.Windows.Forms.Label();
            this.redSlider = new TerraViewer.WwtTrackBar();
            this.label8 = new System.Windows.Forms.Label();
            this.TansferFromEdges = new TerraViewer.WwtButton();
            this.GammaText = new System.Windows.Forms.TextBox();
            this.GammaLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.blurIterationsText = new System.Windows.Forms.Label();
            this.BlurSizeText = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BlurIterations = new TerraViewer.WwtTrackBar();
            this.BlurSize = new TerraViewer.WwtTrackBar();
            this.MakeBlendMap = new TerraViewer.WwtButton();
            this.AlignButtonPanel = new System.Windows.Forms.Panel();
            this.solveZ = new TerraViewer.WWTCheckbox();
            this.solveY = new TerraViewer.WWTCheckbox();
            this.solveX = new TerraViewer.WWTCheckbox();
            this.solveRoll = new TerraViewer.WWTCheckbox();
            this.solveHeading = new TerraViewer.WWTCheckbox();
            this.solvePitch = new TerraViewer.WWTCheckbox();
            this.solveAspect = new TerraViewer.WWTCheckbox();
            this.solveFOV = new TerraViewer.WWTCheckbox();
            this.UseRadial = new TerraViewer.WWTCheckbox();
            this.MakeWarpMaps = new TerraViewer.WwtButton();
            this.useConstraints = new TerraViewer.WWTCheckbox();
            this.errorLabel = new System.Windows.Forms.Label();
            this.AverageError = new System.Windows.Forms.Label();
            this.SolveDistortion = new TerraViewer.WwtButton();
            this.UpdateSoftware = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.screenType = new TerraViewer.WwtCombo();
            this.wwtButton1 = new TerraViewer.WwtButton();
            this.SendNewMaps = new TerraViewer.WwtButton();
            this.domeTilt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.DomeRadius = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.blackBackground = new TerraViewer.WWTCheckbox();
            this.ShowOutlines = new TerraViewer.WWTCheckbox();
            this.showGrid = new TerraViewer.WWTCheckbox();
            this.showProjectorUI = new TerraViewer.WWTCheckbox();
            this.Save = new TerraViewer.WwtButton();
            this.LoadConfig = new TerraViewer.WwtButton();
            this.MousePad = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.PointsTreeContextMenu.SuspendLayout();
            this.BlendPanelButtons.SuspendLayout();
            this.AlignButtonPanel.SuspendLayout();
            this.UpdateSoftware.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MousePad)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Left;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.MinimumSize = new System.Drawing.Size(249, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.ProjectorList);
            this.splitContainer1.Panel1.Controls.Add(this.AddProjector);
            this.splitContainer1.Panel1.Controls.Add(this.EditProjector);
            this.splitContainer1.Panel1.Controls.Add(this.DeleteProjector);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Panel1_Paint);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Panel2.Controls.Add(this.BlendPanelButtons);
            this.splitContainer1.Panel2.Controls.Add(this.AlignButtonPanel);
            this.splitContainer1.Size = new System.Drawing.Size(249, 806);
            this.splitContainer1.SplitterDistance = 180;
            this.splitContainer1.TabIndex = 0;
            // 
            // ProjectorList
            // 
            this.ProjectorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ProjectorList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ProjectorList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ProjectorList.ForeColor = System.Drawing.Color.White;
            this.ProjectorList.FormattingEnabled = true;
            this.ProjectorList.Location = new System.Drawing.Point(12, 27);
            this.ProjectorList.Name = "ProjectorList";
            this.ProjectorList.Size = new System.Drawing.Size(222, 91);
            this.ProjectorList.TabIndex = 1;
            this.ProjectorList.SelectedIndexChanged += new System.EventHandler(this.ProjectorList_SelectedIndexChanged);
            this.ProjectorList.DoubleClick += new System.EventHandler(this.ProjectorList_DoubleClick);
            // 
            // AddProjector
            // 
            this.AddProjector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddProjector.BackColor = System.Drawing.Color.Transparent;
            this.AddProjector.DialogResult = System.Windows.Forms.DialogResult.None;
            this.AddProjector.ImageDisabled = null;
            this.AddProjector.ImageEnabled = null;
            this.AddProjector.Location = new System.Drawing.Point(27, 138);
            this.AddProjector.MaximumSize = new System.Drawing.Size(140, 33);
            this.AddProjector.Name = "AddProjector";
            this.AddProjector.Selected = false;
            this.AddProjector.Size = new System.Drawing.Size(71, 33);
            this.AddProjector.TabIndex = 2;
            this.AddProjector.Text = "Add";
            this.AddProjector.Click += new System.EventHandler(this.AddProjector_Click);
            // 
            // EditProjector
            // 
            this.EditProjector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.EditProjector.BackColor = System.Drawing.Color.Transparent;
            this.EditProjector.DialogResult = System.Windows.Forms.DialogResult.None;
            this.EditProjector.ImageDisabled = null;
            this.EditProjector.ImageEnabled = null;
            this.EditProjector.Location = new System.Drawing.Point(162, 138);
            this.EditProjector.MaximumSize = new System.Drawing.Size(140, 33);
            this.EditProjector.Name = "EditProjector";
            this.EditProjector.Selected = false;
            this.EditProjector.Size = new System.Drawing.Size(74, 33);
            this.EditProjector.TabIndex = 4;
            this.EditProjector.Text = "Edit";
            this.EditProjector.Click += new System.EventHandler(this.EditProjector_Click);
            // 
            // DeleteProjector
            // 
            this.DeleteProjector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteProjector.BackColor = System.Drawing.Color.Transparent;
            this.DeleteProjector.DialogResult = System.Windows.Forms.DialogResult.None;
            this.DeleteProjector.ImageDisabled = null;
            this.DeleteProjector.ImageEnabled = null;
            this.DeleteProjector.Location = new System.Drawing.Point(93, 138);
            this.DeleteProjector.MaximumSize = new System.Drawing.Size(140, 33);
            this.DeleteProjector.Name = "DeleteProjector";
            this.DeleteProjector.Selected = false;
            this.DeleteProjector.Size = new System.Drawing.Size(74, 33);
            this.DeleteProjector.TabIndex = 3;
            this.DeleteProjector.Text = "Delete";
            this.DeleteProjector.Click += new System.EventHandler(this.DeleteProjector_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Projectors";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Align);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.PointWeightLabel);
            this.panel2.Controls.Add(this.Blend);
            this.panel2.Controls.Add(this.WeightTrackBar);
            this.panel2.Controls.Add(this.AddPoint);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Controls.Add(this.PointTree);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(249, 234);
            this.panel2.TabIndex = 0;
            // 
            // Align
            // 
            this.Align.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Align.BackgroundImage")));
            this.Align.Location = new System.Drawing.Point(15, 3);
            this.Align.MaximumSize = new System.Drawing.Size(100, 34);
            this.Align.MinimumSize = new System.Drawing.Size(100, 34);
            this.Align.Name = "Align";
            this.Align.Selected = true;
            this.Align.Size = new System.Drawing.Size(100, 34);
            this.Align.TabIndex = 0;
            this.Align.Title = "Align";
            this.Align.Click += new System.EventHandler(this.Align_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Points";
            // 
            // PointWeightLabel
            // 
            this.PointWeightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PointWeightLabel.AutoSize = true;
            this.PointWeightLabel.Location = new System.Drawing.Point(122, 42);
            this.PointWeightLabel.Name = "PointWeightLabel";
            this.PointWeightLabel.Size = new System.Drawing.Size(68, 13);
            this.PointWeightLabel.TabIndex = 2;
            this.PointWeightLabel.Text = "Point Weight";
            // 
            // Blend
            // 
            this.Blend.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Blend.BackgroundImage")));
            this.Blend.Location = new System.Drawing.Point(129, 3);
            this.Blend.MaximumSize = new System.Drawing.Size(100, 34);
            this.Blend.MinimumSize = new System.Drawing.Size(100, 34);
            this.Blend.Name = "Blend";
            this.Blend.Selected = false;
            this.Blend.Size = new System.Drawing.Size(100, 34);
            this.Blend.TabIndex = 1;
            this.Blend.Title = "Blend";
            this.Blend.Click += new System.EventHandler(this.Blend_Click);
            // 
            // WeightTrackBar
            // 
            this.WeightTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.WeightTrackBar.BackColor = System.Drawing.Color.Transparent;
            this.WeightTrackBar.Location = new System.Drawing.Point(122, 58);
            this.WeightTrackBar.Max = 100;
            this.WeightTrackBar.Name = "WeightTrackBar";
            this.WeightTrackBar.Size = new System.Drawing.Size(80, 20);
            this.WeightTrackBar.TabIndex = 3;
            this.WeightTrackBar.Value = 50;
            this.WeightTrackBar.ValueChanged += new System.EventHandler(this.WeightTrackBar_ValueChanged);
            // 
            // AddPoint
            // 
            this.AddPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddPoint.BackColor = System.Drawing.Color.Transparent;
            this.AddPoint.DialogResult = System.Windows.Forms.DialogResult.None;
            this.AddPoint.ImageDisabled = null;
            this.AddPoint.ImageEnabled = null;
            this.AddPoint.Location = new System.Drawing.Point(208, 51);
            this.AddPoint.MaximumSize = new System.Drawing.Size(140, 33);
            this.AddPoint.Name = "AddPoint";
            this.AddPoint.Selected = false;
            this.AddPoint.Size = new System.Drawing.Size(34, 33);
            this.AddPoint.TabIndex = 4;
            this.AddPoint.Text = "+";
            this.AddPoint.Click += new System.EventHandler(this.AddPoint_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(256, 34);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // PointTree
            // 
            this.PointTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.PointTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.PointTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PointTree.ContextMenuStrip = this.PointsTreeContextMenu;
            this.PointTree.ForeColor = System.Drawing.Color.White;
            this.PointTree.HideSelection = false;
            this.PointTree.HotTracking = true;
            this.PointTree.LineColor = System.Drawing.Color.White;
            this.PointTree.Location = new System.Drawing.Point(12, 84);
            this.PointTree.Name = "PointTree";
            this.PointTree.ShowNodeToolTips = true;
            this.PointTree.Size = new System.Drawing.Size(224, 144);
            this.PointTree.TabIndex = 6;
            this.PointTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.PointTree_AfterSelect);
            this.PointTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PointTree_MouseDown);
            // 
            // PointsTreeContextMenu
            // 
            this.PointsTreeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.transferFromEdgesToolStripMenuItem,
            this.toolStripSeparator1,
            this.moveUpToolStripMenuItem,
            this.moveDownToolStripMenuItem,
            this.contextSeperator2,
            this.blendPointToolStripMenuItem,
            this.propertiesToolStripMenuItem});
            this.PointsTreeContextMenu.Name = "PoitsTreeContextMenu";
            this.PointsTreeContextMenu.Size = new System.Drawing.Size(181, 170);
            this.PointsTreeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.PointsTreeContextMenu_Opening);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // transferFromEdgesToolStripMenuItem
            // 
            this.transferFromEdgesToolStripMenuItem.Name = "transferFromEdgesToolStripMenuItem";
            this.transferFromEdgesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.transferFromEdgesToolStripMenuItem.Text = "Transfer from Edges";
            this.transferFromEdgesToolStripMenuItem.Click += new System.EventHandler(this.transferFromEdgesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            // 
            // moveUpToolStripMenuItem
            // 
            this.moveUpToolStripMenuItem.Name = "moveUpToolStripMenuItem";
            this.moveUpToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.moveUpToolStripMenuItem.Text = "Move Up";
            this.moveUpToolStripMenuItem.Click += new System.EventHandler(this.moveUpToolStripMenuItem_Click);
            // 
            // moveDownToolStripMenuItem
            // 
            this.moveDownToolStripMenuItem.Name = "moveDownToolStripMenuItem";
            this.moveDownToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.moveDownToolStripMenuItem.Text = "Move Down";
            this.moveDownToolStripMenuItem.Click += new System.EventHandler(this.moveDownToolStripMenuItem_Click);
            // 
            // contextSeperator2
            // 
            this.contextSeperator2.Name = "contextSeperator2";
            this.contextSeperator2.Size = new System.Drawing.Size(177, 6);
            // 
            // blendPointToolStripMenuItem
            // 
            this.blendPointToolStripMenuItem.Name = "blendPointToolStripMenuItem";
            this.blendPointToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.blendPointToolStripMenuItem.Text = "Blend Point";
            this.blendPointToolStripMenuItem.Click += new System.EventHandler(this.blendPointToolStripMenuItem_Click);
            // 
            // propertiesToolStripMenuItem
            // 
            this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
            this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.propertiesToolStripMenuItem.Text = "Properties";
            this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
            // 
            // BlendPanelButtons
            // 
            this.BlendPanelButtons.Controls.Add(this.blueAmount);
            this.BlendPanelButtons.Controls.Add(this.greenAmount);
            this.BlendPanelButtons.Controls.Add(this.redAmount);
            this.BlendPanelButtons.Controls.Add(this.label11);
            this.BlendPanelButtons.Controls.Add(this.blueSlider);
            this.BlendPanelButtons.Controls.Add(this.label10);
            this.BlendPanelButtons.Controls.Add(this.greenSlider);
            this.BlendPanelButtons.Controls.Add(this.label9);
            this.BlendPanelButtons.Controls.Add(this.redSlider);
            this.BlendPanelButtons.Controls.Add(this.label8);
            this.BlendPanelButtons.Controls.Add(this.TansferFromEdges);
            this.BlendPanelButtons.Controls.Add(this.GammaText);
            this.BlendPanelButtons.Controls.Add(this.GammaLabel);
            this.BlendPanelButtons.Controls.Add(this.label6);
            this.BlendPanelButtons.Controls.Add(this.blurIterationsText);
            this.BlendPanelButtons.Controls.Add(this.BlurSizeText);
            this.BlendPanelButtons.Controls.Add(this.label4);
            this.BlendPanelButtons.Controls.Add(this.BlurIterations);
            this.BlendPanelButtons.Controls.Add(this.BlurSize);
            this.BlendPanelButtons.Controls.Add(this.MakeBlendMap);
            this.BlendPanelButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BlendPanelButtons.Location = new System.Drawing.Point(0, 234);
            this.BlendPanelButtons.Name = "BlendPanelButtons";
            this.BlendPanelButtons.Size = new System.Drawing.Size(249, 188);
            this.BlendPanelButtons.TabIndex = 1;
            // 
            // blueAmount
            // 
            this.blueAmount.AutoSize = true;
            this.blueAmount.Location = new System.Drawing.Point(147, 69);
            this.blueAmount.Name = "blueAmount";
            this.blueAmount.Size = new System.Drawing.Size(13, 13);
            this.blueAmount.TabIndex = 13;
            this.blueAmount.Text = "0";
            // 
            // greenAmount
            // 
            this.greenAmount.AutoSize = true;
            this.greenAmount.Location = new System.Drawing.Point(147, 47);
            this.greenAmount.Name = "greenAmount";
            this.greenAmount.Size = new System.Drawing.Size(13, 13);
            this.greenAmount.TabIndex = 13;
            this.greenAmount.Text = "0";
            // 
            // redAmount
            // 
            this.redAmount.AutoSize = true;
            this.redAmount.Location = new System.Drawing.Point(148, 23);
            this.redAmount.Name = "redAmount";
            this.redAmount.Size = new System.Drawing.Size(13, 13);
            this.redAmount.TabIndex = 13;
            this.redAmount.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 69);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(28, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "Blue";
            // 
            // blueSlider
            // 
            this.blueSlider.BackColor = System.Drawing.Color.Transparent;
            this.blueSlider.Location = new System.Drawing.Point(61, 65);
            this.blueSlider.Max = 100;
            this.blueSlider.Name = "blueSlider";
            this.blueSlider.Size = new System.Drawing.Size(80, 20);
            this.blueSlider.TabIndex = 11;
            this.blueSlider.Value = 50;
            this.blueSlider.ValueChanged += new System.EventHandler(this.blueSlider_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 46);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(36, 13);
            this.label10.TabIndex = 12;
            this.label10.Text = "Green";
            // 
            // greenSlider
            // 
            this.greenSlider.BackColor = System.Drawing.Color.Transparent;
            this.greenSlider.Location = new System.Drawing.Point(61, 43);
            this.greenSlider.Max = 100;
            this.greenSlider.Name = "greenSlider";
            this.greenSlider.Size = new System.Drawing.Size(80, 20);
            this.greenSlider.TabIndex = 11;
            this.greenSlider.Value = 50;
            this.greenSlider.ValueChanged += new System.EventHandler(this.greenSlider_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Red";
            // 
            // redSlider
            // 
            this.redSlider.BackColor = System.Drawing.Color.Transparent;
            this.redSlider.Location = new System.Drawing.Point(61, 19);
            this.redSlider.Max = 100;
            this.redSlider.Name = "redSlider";
            this.redSlider.Size = new System.Drawing.Size(80, 20);
            this.redSlider.TabIndex = 11;
            this.redSlider.Value = 50;
            this.redSlider.ValueChanged += new System.EventHandler(this.redSlider_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 13);
            this.label8.TabIndex = 10;
            this.label8.Text = "Color Correction";
            // 
            // TansferFromEdges
            // 
            this.TansferFromEdges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TansferFromEdges.BackColor = System.Drawing.Color.Transparent;
            this.TansferFromEdges.DialogResult = System.Windows.Forms.DialogResult.None;
            this.TansferFromEdges.ImageDisabled = null;
            this.TansferFromEdges.ImageEnabled = null;
            this.TansferFromEdges.Location = new System.Drawing.Point(3, 149);
            this.TansferFromEdges.MaximumSize = new System.Drawing.Size(140, 33);
            this.TansferFromEdges.Name = "TansferFromEdges";
            this.TansferFromEdges.Selected = false;
            this.TansferFromEdges.Size = new System.Drawing.Size(117, 33);
            this.TansferFromEdges.TabIndex = 9;
            this.TansferFromEdges.Text = "Trasfer from Edges";
            this.TansferFromEdges.Click += new System.EventHandler(this.TansferFromEdges_Click);
            // 
            // GammaText
            // 
            this.GammaText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.GammaText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GammaText.CausesValidation = false;
            this.GammaText.ForeColor = System.Drawing.Color.White;
            this.GammaText.Location = new System.Drawing.Point(180, 118);
            this.GammaText.Name = "GammaText";
            this.GammaText.Size = new System.Drawing.Size(54, 20);
            this.GammaText.TabIndex = 8;
            this.GammaText.Text = "2.2";
            this.GammaText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.GammaText.TextChanged += new System.EventHandler(this.GammaText_TextChanged);
            // 
            // GammaLabel
            // 
            this.GammaLabel.AutoSize = true;
            this.GammaLabel.Location = new System.Drawing.Point(179, 97);
            this.GammaLabel.Name = "GammaLabel";
            this.GammaLabel.Size = new System.Drawing.Size(43, 13);
            this.GammaLabel.TabIndex = 7;
            this.GammaLabel.Text = "Gamma";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Iterations";
            // 
            // blurIterationsText
            // 
            this.blurIterationsText.AutoSize = true;
            this.blurIterationsText.Location = new System.Drawing.Point(149, 123);
            this.blurIterationsText.Name = "blurIterationsText";
            this.blurIterationsText.Size = new System.Drawing.Size(13, 13);
            this.blurIterationsText.TabIndex = 5;
            this.blurIterationsText.Text = "1";
            // 
            // BlurSizeText
            // 
            this.BlurSizeText.AutoSize = true;
            this.BlurSizeText.Location = new System.Drawing.Point(149, 101);
            this.BlurSizeText.Name = "BlurSizeText";
            this.BlurSizeText.Size = new System.Drawing.Size(13, 13);
            this.BlurSizeText.TabIndex = 2;
            this.BlurSizeText.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Blur Size";
            // 
            // BlurIterations
            // 
            this.BlurIterations.BackColor = System.Drawing.Color.Transparent;
            this.BlurIterations.Location = new System.Drawing.Point(61, 123);
            this.BlurIterations.Max = 10;
            this.BlurIterations.Name = "BlurIterations";
            this.BlurIterations.Size = new System.Drawing.Size(80, 20);
            this.BlurIterations.TabIndex = 4;
            this.BlurIterations.Value = 1;
            this.BlurIterations.ValueChanged += new System.EventHandler(this.BlurIterations_ValueChanged);
            // 
            // BlurSize
            // 
            this.BlurSize.BackColor = System.Drawing.Color.Transparent;
            this.BlurSize.Location = new System.Drawing.Point(61, 97);
            this.BlurSize.Max = 50;
            this.BlurSize.Name = "BlurSize";
            this.BlurSize.Size = new System.Drawing.Size(80, 20);
            this.BlurSize.TabIndex = 1;
            this.BlurSize.Value = 1;
            this.BlurSize.ValueChanged += new System.EventHandler(this.BlurSize_ValueChanged);
            // 
            // MakeBlendMap
            // 
            this.MakeBlendMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.MakeBlendMap.BackColor = System.Drawing.Color.Transparent;
            this.MakeBlendMap.DialogResult = System.Windows.Forms.DialogResult.None;
            this.MakeBlendMap.ImageDisabled = null;
            this.MakeBlendMap.ImageEnabled = null;
            this.MakeBlendMap.Location = new System.Drawing.Point(125, 149);
            this.MakeBlendMap.MaximumSize = new System.Drawing.Size(140, 33);
            this.MakeBlendMap.Name = "MakeBlendMap";
            this.MakeBlendMap.Selected = false;
            this.MakeBlendMap.Size = new System.Drawing.Size(118, 33);
            this.MakeBlendMap.TabIndex = 6;
            this.MakeBlendMap.Text = "Make Blend Maps";
            this.MakeBlendMap.Click += new System.EventHandler(this.MakeBlendMap_Click);
            // 
            // AlignButtonPanel
            // 
            this.AlignButtonPanel.Controls.Add(this.solveZ);
            this.AlignButtonPanel.Controls.Add(this.solveY);
            this.AlignButtonPanel.Controls.Add(this.solveX);
            this.AlignButtonPanel.Controls.Add(this.solveRoll);
            this.AlignButtonPanel.Controls.Add(this.solveHeading);
            this.AlignButtonPanel.Controls.Add(this.solvePitch);
            this.AlignButtonPanel.Controls.Add(this.solveAspect);
            this.AlignButtonPanel.Controls.Add(this.solveFOV);
            this.AlignButtonPanel.Controls.Add(this.UseRadial);
            this.AlignButtonPanel.Controls.Add(this.MakeWarpMaps);
            this.AlignButtonPanel.Controls.Add(this.useConstraints);
            this.AlignButtonPanel.Controls.Add(this.errorLabel);
            this.AlignButtonPanel.Controls.Add(this.AverageError);
            this.AlignButtonPanel.Controls.Add(this.SolveDistortion);
            this.AlignButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.AlignButtonPanel.Location = new System.Drawing.Point(0, 422);
            this.AlignButtonPanel.Name = "AlignButtonPanel";
            this.AlignButtonPanel.Size = new System.Drawing.Size(249, 200);
            this.AlignButtonPanel.TabIndex = 2;
            // 
            // solveZ
            // 
            this.solveZ.BackColor = System.Drawing.Color.Transparent;
            this.solveZ.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.solveZ.Checked = false;
            this.solveZ.Location = new System.Drawing.Point(129, 75);
            this.solveZ.Name = "solveZ";
            this.solveZ.Size = new System.Drawing.Size(112, 25);
            this.solveZ.TabIndex = 6;
            this.solveZ.Text = "Solve Z";
            this.solveZ.CheckedChanged += new System.EventHandler(this.solveZ_CheckedChanged);
            // 
            // solveY
            // 
            this.solveY.BackColor = System.Drawing.Color.Transparent;
            this.solveY.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.solveY.Checked = false;
            this.solveY.Location = new System.Drawing.Point(129, 50);
            this.solveY.Name = "solveY";
            this.solveY.Size = new System.Drawing.Size(112, 25);
            this.solveY.TabIndex = 6;
            this.solveY.Text = "Solve Y";
            this.solveY.CheckedChanged += new System.EventHandler(this.solveY_CheckedChanged);
            // 
            // solveX
            // 
            this.solveX.BackColor = System.Drawing.Color.Transparent;
            this.solveX.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.solveX.Checked = false;
            this.solveX.Location = new System.Drawing.Point(129, 25);
            this.solveX.Name = "solveX";
            this.solveX.Size = new System.Drawing.Size(112, 25);
            this.solveX.TabIndex = 6;
            this.solveX.Text = "Solve X";
            this.solveX.CheckedChanged += new System.EventHandler(this.solveX_CheckedChanged);
            // 
            // solveRoll
            // 
            this.solveRoll.BackColor = System.Drawing.Color.Transparent;
            this.solveRoll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.solveRoll.Checked = false;
            this.solveRoll.Location = new System.Drawing.Point(8, 75);
            this.solveRoll.Name = "solveRoll";
            this.solveRoll.Size = new System.Drawing.Size(112, 25);
            this.solveRoll.TabIndex = 6;
            this.solveRoll.Text = "Solve Roll";
            this.solveRoll.CheckedChanged += new System.EventHandler(this.solveRoll_CheckedChanged);
            // 
            // solveHeading
            // 
            this.solveHeading.BackColor = System.Drawing.Color.Transparent;
            this.solveHeading.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.solveHeading.Checked = false;
            this.solveHeading.Location = new System.Drawing.Point(8, 50);
            this.solveHeading.Name = "solveHeading";
            this.solveHeading.Size = new System.Drawing.Size(112, 25);
            this.solveHeading.TabIndex = 6;
            this.solveHeading.Text = "Solve Heading";
            this.solveHeading.CheckedChanged += new System.EventHandler(this.solveHeading_CheckedChanged);
            // 
            // solvePitch
            // 
            this.solvePitch.BackColor = System.Drawing.Color.Transparent;
            this.solvePitch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.solvePitch.Checked = false;
            this.solvePitch.Location = new System.Drawing.Point(8, 25);
            this.solvePitch.Name = "solvePitch";
            this.solvePitch.Size = new System.Drawing.Size(112, 25);
            this.solvePitch.TabIndex = 6;
            this.solvePitch.Text = "Solve Pitch";
            this.solvePitch.CheckedChanged += new System.EventHandler(this.solvePitch_CheckedChanged);
            // 
            // solveAspect
            // 
            this.solveAspect.BackColor = System.Drawing.Color.Transparent;
            this.solveAspect.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.solveAspect.Checked = false;
            this.solveAspect.Location = new System.Drawing.Point(129, 0);
            this.solveAspect.Name = "solveAspect";
            this.solveAspect.Size = new System.Drawing.Size(112, 25);
            this.solveAspect.TabIndex = 6;
            this.solveAspect.Text = "Solve Aspect";
            this.solveAspect.CheckedChanged += new System.EventHandler(this.solveAspect_CheckedChanged);
            // 
            // solveFOV
            // 
            this.solveFOV.BackColor = System.Drawing.Color.Transparent;
            this.solveFOV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.solveFOV.Checked = false;
            this.solveFOV.Location = new System.Drawing.Point(8, 1);
            this.solveFOV.Name = "solveFOV";
            this.solveFOV.Size = new System.Drawing.Size(112, 25);
            this.solveFOV.TabIndex = 6;
            this.solveFOV.Text = "Solve Fov";
            this.solveFOV.CheckedChanged += new System.EventHandler(this.solveFOV_CheckedChanged);
            // 
            // UseRadial
            // 
            this.UseRadial.BackColor = System.Drawing.Color.Transparent;
            this.UseRadial.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.UseRadial.Checked = false;
            this.UseRadial.Location = new System.Drawing.Point(8, 100);
            this.UseRadial.Name = "UseRadial";
            this.UseRadial.Size = new System.Drawing.Size(138, 25);
            this.UseRadial.TabIndex = 5;
            this.UseRadial.Text = "Solve Radial Distortion";
            this.UseRadial.CheckedChanged += new System.EventHandler(this.UseRadial_CheckedChanged);
            // 
            // MakeWarpMaps
            // 
            this.MakeWarpMaps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.MakeWarpMaps.BackColor = System.Drawing.Color.Transparent;
            this.MakeWarpMaps.DialogResult = System.Windows.Forms.DialogResult.None;
            this.MakeWarpMaps.ImageDisabled = null;
            this.MakeWarpMaps.ImageEnabled = null;
            this.MakeWarpMaps.Location = new System.Drawing.Point(126, 164);
            this.MakeWarpMaps.MaximumSize = new System.Drawing.Size(140, 33);
            this.MakeWarpMaps.Name = "MakeWarpMaps";
            this.MakeWarpMaps.Selected = false;
            this.MakeWarpMaps.Size = new System.Drawing.Size(118, 33);
            this.MakeWarpMaps.TabIndex = 4;
            this.MakeWarpMaps.Text = "Make Warp Maps";
            this.MakeWarpMaps.Click += new System.EventHandler(this.MakeWarpMaps_Click);
            // 
            // useConstraints
            // 
            this.useConstraints.BackColor = System.Drawing.Color.Transparent;
            this.useConstraints.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.useConstraints.Checked = false;
            this.useConstraints.Location = new System.Drawing.Point(8, 126);
            this.useConstraints.Name = "useConstraints";
            this.useConstraints.Size = new System.Drawing.Size(115, 25);
            this.useConstraints.TabIndex = 0;
            this.useConstraints.Text = "Use Constraints";
            this.useConstraints.CheckedChanged += new System.EventHandler(this.useConstraints_CheckedChanged);
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Location = new System.Drawing.Point(162, 123);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(72, 13);
            this.errorLabel.TabIndex = 1;
            this.errorLabel.Text = "Average Error";
            // 
            // AverageError
            // 
            this.AverageError.Location = new System.Drawing.Point(163, 143);
            this.AverageError.Name = "AverageError";
            this.AverageError.Size = new System.Drawing.Size(71, 13);
            this.AverageError.TabIndex = 2;
            this.AverageError.Text = "0";
            this.AverageError.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // SolveDistortion
            // 
            this.SolveDistortion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SolveDistortion.BackColor = System.Drawing.Color.Transparent;
            this.SolveDistortion.DialogResult = System.Windows.Forms.DialogResult.None;
            this.SolveDistortion.ImageDisabled = null;
            this.SolveDistortion.ImageEnabled = null;
            this.SolveDistortion.Location = new System.Drawing.Point(3, 164);
            this.SolveDistortion.MaximumSize = new System.Drawing.Size(140, 33);
            this.SolveDistortion.Name = "SolveDistortion";
            this.SolveDistortion.Selected = false;
            this.SolveDistortion.Size = new System.Drawing.Size(117, 33);
            this.SolveDistortion.TabIndex = 3;
            this.SolveDistortion.Text = "Solve Alignment";
            this.SolveDistortion.Click += new System.EventHandler(this.SolveDistortion_Click);
            // 
            // UpdateSoftware
            // 
            this.UpdateSoftware.Controls.Add(this.label7);
            this.UpdateSoftware.Controls.Add(this.screenType);
            this.UpdateSoftware.Controls.Add(this.wwtButton1);
            this.UpdateSoftware.Controls.Add(this.SendNewMaps);
            this.UpdateSoftware.Controls.Add(this.domeTilt);
            this.UpdateSoftware.Controls.Add(this.label5);
            this.UpdateSoftware.Controls.Add(this.DomeRadius);
            this.UpdateSoftware.Controls.Add(this.label3);
            this.UpdateSoftware.Controls.Add(this.blackBackground);
            this.UpdateSoftware.Controls.Add(this.ShowOutlines);
            this.UpdateSoftware.Controls.Add(this.showGrid);
            this.UpdateSoftware.Controls.Add(this.showProjectorUI);
            this.UpdateSoftware.Controls.Add(this.Save);
            this.UpdateSoftware.Controls.Add(this.LoadConfig);
            this.UpdateSoftware.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.UpdateSoftware.Location = new System.Drawing.Point(249, 706);
            this.UpdateSoftware.Name = "UpdateSoftware";
            this.UpdateSoftware.Size = new System.Drawing.Size(731, 100);
            this.UpdateSoftware.TabIndex = 1;
            this.UpdateSoftware.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(276, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Screen Type";
            // 
            // screenType
            // 
            this.screenType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.screenType.DateTimeValue = new System.DateTime(2011, 3, 6, 15, 52, 5, 394);
            this.screenType.Filter = TerraViewer.Classification.Unfiltered;
            this.screenType.FilterStyle = false;
            this.screenType.Location = new System.Drawing.Point(279, 23);
            this.screenType.Margin = new System.Windows.Forms.Padding(0);
            this.screenType.MasterTime = true;
            this.screenType.MaximumSize = new System.Drawing.Size(248, 33);
            this.screenType.MinimumSize = new System.Drawing.Size(35, 33);
            this.screenType.Name = "screenType";
            this.screenType.SelectedIndex = -1;
            this.screenType.SelectedItem = null;
            this.screenType.Size = new System.Drawing.Size(158, 33);
            this.screenType.State = TerraViewer.State.Rest;
            this.screenType.TabIndex = 10;
            this.screenType.Type = TerraViewer.WwtCombo.ComboType.List;
            this.screenType.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.screenType_SelectionChanged);
            // 
            // wwtButton1
            // 
            this.wwtButton1.BackColor = System.Drawing.Color.Transparent;
            this.wwtButton1.DialogResult = System.Windows.Forms.DialogResult.None;
            this.wwtButton1.ImageDisabled = null;
            this.wwtButton1.ImageEnabled = null;
            this.wwtButton1.Location = new System.Drawing.Point(496, 64);
            this.wwtButton1.MaximumSize = new System.Drawing.Size(140, 33);
            this.wwtButton1.Name = "wwtButton1";
            this.wwtButton1.Selected = false;
            this.wwtButton1.Size = new System.Drawing.Size(112, 33);
            this.wwtButton1.TabIndex = 9;
            this.wwtButton1.Text = "Software Update";
            this.wwtButton1.Visible = false;
            this.wwtButton1.Click += new System.EventHandler(this.wwtButton1_Click);
            // 
            // SendNewMaps
            // 
            this.SendNewMaps.BackColor = System.Drawing.Color.Transparent;
            this.SendNewMaps.DialogResult = System.Windows.Forms.DialogResult.None;
            this.SendNewMaps.ImageDisabled = null;
            this.SendNewMaps.ImageEnabled = null;
            this.SendNewMaps.Location = new System.Drawing.Point(496, 34);
            this.SendNewMaps.MaximumSize = new System.Drawing.Size(140, 33);
            this.SendNewMaps.Name = "SendNewMaps";
            this.SendNewMaps.Selected = false;
            this.SendNewMaps.Size = new System.Drawing.Size(112, 33);
            this.SendNewMaps.TabIndex = 7;
            this.SendNewMaps.Text = "Send New Maps";
            this.SendNewMaps.Click += new System.EventHandler(this.SendNewMaps_Click);
            // 
            // domeTilt
            // 
            this.domeTilt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.domeTilt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.domeTilt.ForeColor = System.Drawing.Color.White;
            this.domeTilt.Location = new System.Drawing.Point(362, 72);
            this.domeTilt.Name = "domeTilt";
            this.domeTilt.Size = new System.Drawing.Size(68, 20);
            this.domeTilt.TabIndex = 6;
            this.domeTilt.Text = "0";
            this.domeTilt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.domeTilt.TextChanged += new System.EventHandler(this.domeTilt_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(359, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Tilt";
            // 
            // DomeRadius
            // 
            this.DomeRadius.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.DomeRadius.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DomeRadius.ForeColor = System.Drawing.Color.White;
            this.DomeRadius.Location = new System.Drawing.Point(279, 72);
            this.DomeRadius.Name = "DomeRadius";
            this.DomeRadius.Size = new System.Drawing.Size(68, 20);
            this.DomeRadius.TabIndex = 6;
            this.DomeRadius.Text = "1";
            this.DomeRadius.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.DomeRadius.TextChanged += new System.EventHandler(this.DomeRadius_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(276, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Screen Radius";
            // 
            // blackBackground
            // 
            this.blackBackground.BackColor = System.Drawing.Color.Transparent;
            this.blackBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.blackBackground.Checked = false;
            this.blackBackground.Location = new System.Drawing.Point(145, 6);
            this.blackBackground.Name = "blackBackground";
            this.blackBackground.Size = new System.Drawing.Size(131, 25);
            this.blackBackground.TabIndex = 4;
            this.blackBackground.Text = "Black Background";
            this.blackBackground.CheckedChanged += new System.EventHandler(this.wwtCheckbox1_CheckedChanged);
            // 
            // ShowOutlines
            // 
            this.ShowOutlines.BackColor = System.Drawing.Color.Transparent;
            this.ShowOutlines.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ShowOutlines.Checked = false;
            this.ShowOutlines.Location = new System.Drawing.Point(7, 32);
            this.ShowOutlines.Name = "ShowOutlines";
            this.ShowOutlines.Size = new System.Drawing.Size(131, 25);
            this.ShowOutlines.TabIndex = 2;
            this.ShowOutlines.Text = "Outlines";
            this.ShowOutlines.CheckedChanged += new System.EventHandler(this.ShowOutlines_CheckedChanged);
            // 
            // showGrid
            // 
            this.showGrid.BackColor = System.Drawing.Color.Transparent;
            this.showGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.showGrid.Checked = false;
            this.showGrid.Location = new System.Drawing.Point(7, 56);
            this.showGrid.Name = "showGrid";
            this.showGrid.Size = new System.Drawing.Size(131, 25);
            this.showGrid.TabIndex = 3;
            this.showGrid.Text = "Dome Grid";
            this.showGrid.Visible = false;
            this.showGrid.CheckedChanged += new System.EventHandler(this.showGrid_CheckedChanged);
            // 
            // showProjectorUI
            // 
            this.showProjectorUI.BackColor = System.Drawing.Color.Transparent;
            this.showProjectorUI.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.showProjectorUI.Checked = false;
            this.showProjectorUI.Location = new System.Drawing.Point(7, 7);
            this.showProjectorUI.Name = "showProjectorUI";
            this.showProjectorUI.Size = new System.Drawing.Size(132, 25);
            this.showProjectorUI.TabIndex = 1;
            this.showProjectorUI.Text = "Calibration Screens";
            this.showProjectorUI.CheckedChanged += new System.EventHandler(this.showProjectorUI_CheckedChanged);
            // 
            // Save
            // 
            this.Save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Save.BackColor = System.Drawing.Color.Transparent;
            this.Save.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Save.ImageDisabled = null;
            this.Save.ImageEnabled = null;
            this.Save.Location = new System.Drawing.Point(649, 64);
            this.Save.MaximumSize = new System.Drawing.Size(140, 33);
            this.Save.Name = "Save";
            this.Save.Selected = false;
            this.Save.Size = new System.Drawing.Size(79, 33);
            this.Save.TabIndex = 0;
            this.Save.Text = "Save";
            this.Save.Click += new System.EventHandler(this.SaveConfig_Click);
            // 
            // LoadConfig
            // 
            this.LoadConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.LoadConfig.BackColor = System.Drawing.Color.Transparent;
            this.LoadConfig.DialogResult = System.Windows.Forms.DialogResult.None;
            this.LoadConfig.ImageDisabled = null;
            this.LoadConfig.ImageEnabled = null;
            this.LoadConfig.Location = new System.Drawing.Point(649, 34);
            this.LoadConfig.MaximumSize = new System.Drawing.Size(140, 33);
            this.LoadConfig.Name = "LoadConfig";
            this.LoadConfig.Selected = false;
            this.LoadConfig.Size = new System.Drawing.Size(79, 33);
            this.LoadConfig.TabIndex = 8;
            this.LoadConfig.Text = "Load";
            this.LoadConfig.Click += new System.EventHandler(this.Load_Click);
            // 
            // MousePad
            // 
            this.MousePad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.MousePad.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MousePad.Image = ((System.Drawing.Image)(resources.GetObject("MousePad.Image")));
            this.MousePad.Location = new System.Drawing.Point(249, 0);
            this.MousePad.Name = "MousePad";
            this.MousePad.Size = new System.Drawing.Size(731, 706);
            this.MousePad.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.MousePad.TabIndex = 3;
            this.MousePad.TabStop = false;
            this.MousePad.Paint += new System.Windows.Forms.PaintEventHandler(this.MousePad_Paint);
            this.MousePad.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MousePad_MouseDown);
            this.MousePad.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MousePad_MouseMove);
            this.MousePad.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MousePad_MouseUp);
            this.MousePad.Resize += new System.EventHandler(this.MousePad_Resize);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Calibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(980, 806);
            this.Controls.Add(this.MousePad);
            this.Controls.Add(this.UpdateSoftware);
            this.Controls.Add(this.splitContainer1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Calibration";
            this.ShowIcon = false;
            this.Text = "Multi-projector Calibration";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Calibration_FormClosed);
            this.Load += new System.EventHandler(this.Calibration_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.PointsTreeContextMenu.ResumeLayout(false);
            this.BlendPanelButtons.ResumeLayout(false);
            this.BlendPanelButtons.PerformLayout();
            this.AlignButtonPanel.ResumeLayout(false);
            this.AlignButtonPanel.PerformLayout();
            this.UpdateSoftware.ResumeLayout(false);
            this.UpdateSoftware.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MousePad)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel UpdateSoftware;
        private System.Windows.Forms.Label label2;
        private WwtButton AddProjector;
        private WwtButton DeleteProjector;
        private System.Windows.Forms.PictureBox MousePad;
        private WwtButton Save;
        private WwtButton LoadConfig;
        private Tab Blend;
        private Tab Align;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListBox ProjectorList;
        private WwtButton EditProjector;
        private System.Windows.Forms.TreeView PointTree;
        private WWTCheckbox showProjectorUI;
        private System.Windows.Forms.Timer timer1;
        private WWTCheckbox ShowOutlines;
        private WWTCheckbox showGrid;
        private WwtButton AddPoint;
        private WwtButton MakeBlendMap;
        private WwtButton SolveDistortion;
        private WWTCheckbox blackBackground;
        private System.Windows.Forms.ContextMenuStrip PointsTreeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator contextSeperator2;
        private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
        private System.Windows.Forms.TextBox DomeRadius;
        private System.Windows.Forms.Label label3;
        private WWTCheckbox useConstraints;
        private System.Windows.Forms.Label AverageError;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.Label PointWeightLabel;
        private WwtTrackBar WeightTrackBar;
        private WwtButton MakeWarpMaps;
        private WwtButton SendNewMaps;
        private System.Windows.Forms.Panel AlignButtonPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel BlendPanelButtons;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label blurIterationsText;
        private System.Windows.Forms.Label BlurSizeText;
        private System.Windows.Forms.Label label4;
        private WwtTrackBar BlurIterations;
        private WwtTrackBar BlurSize;
        private WWTCheckbox UseRadial;
        private System.Windows.Forms.TextBox domeTilt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label GammaLabel;
        private System.Windows.Forms.TextBox GammaText;
        private WwtButton wwtButton1;
        private System.Windows.Forms.ToolStripMenuItem moveUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blendPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem transferFromEdgesToolStripMenuItem;
        private WwtButton TansferFromEdges;
        private System.Windows.Forms.Label label7;
        private WwtCombo screenType;
        private WWTCheckbox solveFOV;
        private WWTCheckbox solveAspect;
        private WWTCheckbox solvePitch;
        private WWTCheckbox solveHeading;
        private WWTCheckbox solveRoll;
        private WWTCheckbox solveZ;
        private WWTCheckbox solveY;
        private WWTCheckbox solveX;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private WwtTrackBar redSlider;
        private System.Windows.Forms.Label blueAmount;
        private System.Windows.Forms.Label greenAmount;
        private System.Windows.Forms.Label redAmount;
        private System.Windows.Forms.Label label11;
        private WwtTrackBar blueSlider;
        private System.Windows.Forms.Label label10;
        private WwtTrackBar greenSlider;
    }
}