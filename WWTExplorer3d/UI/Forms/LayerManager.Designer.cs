namespace TerraViewer
{
    partial class LayerManager
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
            this.timeScrubber = new System.Windows.Forms.TrackBar();
            this.timeLabel = new System.Windows.Forms.Label();
            this.loopTimer = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.Plus = new System.Windows.Forms.Label();
            this.Minus = new System.Windows.Forms.Label();
            this.closeBox = new System.Windows.Forms.PictureBox();
            this.breadCrumbs = new System.Windows.Forms.Label();
            this.layerTree = new System.Windows.Forms.TreeView();
            this.NameValues = new System.Windows.Forms.ListView();
            this.NameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ValueColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.startDate = new System.Windows.Forms.Label();
            this.endDate = new System.Windows.Forms.Label();
            this.fadeTimer = new System.Windows.Forms.Timer(this.components);
            this.timeSeries = new TerraViewer.WWTCheckbox();
            this.autoLoopCheckbox = new TerraViewer.WWTCheckbox();
            this.resetLayers = new TerraViewer.WwtButton();
            this.pasteLayer = new TerraViewer.WwtButton();
            this.AddLayer = new TerraViewer.WwtButton();
            this.DeleteLayer = new TerraViewer.WwtButton();
            this.SaveLayers = new TerraViewer.WwtButton();
            ((System.ComponentModel.ISupportInitialize)(this.timeScrubber)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // timeScrubber
            // 
            this.timeScrubber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.timeScrubber.Location = new System.Drawing.Point(1, 627);
            this.timeScrubber.Maximum = 1000;
            this.timeScrubber.Name = "timeScrubber";
            this.timeScrubber.Size = new System.Drawing.Size(246, 45);
            this.timeScrubber.TabIndex = 3;
            this.timeScrubber.TickFrequency = 25;
            this.timeScrubber.Scroll += new System.EventHandler(this.timeScrubber_Scroll);
            // 
            // timeLabel
            // 
            this.timeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.timeLabel.AutoSize = true;
            this.timeLabel.Location = new System.Drawing.Point(7, 582);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(76, 13);
            this.timeLabel.TabIndex = 0;
            this.timeLabel.Text = "Time Scrubber";
            // 
            // loopTimer
            // 
            this.loopTimer.Enabled = true;
            this.loopTimer.Tick += new System.EventHandler(this.loopTimer_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(1, 2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.Plus);
            this.splitContainer1.Panel1.Controls.Add(this.Minus);
            this.splitContainer1.Panel1.Controls.Add(this.closeBox);
            this.splitContainer1.Panel1.Controls.Add(this.breadCrumbs);
            this.splitContainer1.Panel1.Controls.Add(this.layerTree);
            this.splitContainer1.Panel1.Controls.Add(this.resetLayers);
            this.splitContainer1.Panel1.Controls.Add(this.AddLayer);
            this.splitContainer1.Panel1.Controls.Add(this.pasteLayer);
            this.splitContainer1.Panel1.Controls.Add(this.DeleteLayer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.NameValues);
            this.splitContainer1.Size = new System.Drawing.Size(241, 577);
            this.splitContainer1.SplitterDistance = 512;
            this.splitContainer1.TabIndex = 6;
            // 
            // Plus
            // 
            this.Plus.AutoSize = true;
            this.Plus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(51)))));
            this.Plus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Plus.Location = new System.Drawing.Point(24, 5);
            this.Plus.Name = "Plus";
            this.Plus.Size = new System.Drawing.Size(16, 16);
            this.Plus.TabIndex = 5;
            this.Plus.Text = "+";
            this.Plus.Click += new System.EventHandler(this.Plus_Click);
            this.Plus.MouseEnter += new System.EventHandler(this.HighlightLabel_MouseEnter);
            this.Plus.MouseLeave += new System.EventHandler(this.HighlightLabel_MouseLeave);
            // 
            // Minus
            // 
            this.Minus.AutoSize = true;
            this.Minus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(51)))));
            this.Minus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Minus.Location = new System.Drawing.Point(9, 5);
            this.Minus.Name = "Minus";
            this.Minus.Size = new System.Drawing.Size(13, 16);
            this.Minus.TabIndex = 5;
            this.Minus.Text = "-";
            this.Minus.Click += new System.EventHandler(this.Minus_Click);
            this.Minus.MouseEnter += new System.EventHandler(this.HighlightLabel_MouseEnter);
            this.Minus.MouseLeave += new System.EventHandler(this.HighlightLabel_MouseLeave);
            // 
            // closeBox
            // 
            this.closeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBox.Image = global::TerraViewer.Properties.Resources.CloseRest;
            this.closeBox.Location = new System.Drawing.Point(226, 3);
            this.closeBox.Name = "closeBox";
            this.closeBox.Size = new System.Drawing.Size(16, 16);
            this.closeBox.TabIndex = 4;
            this.closeBox.TabStop = false;
            this.closeBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.closeBox_MouseDown);
            this.closeBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.closeBox_MouseUp);
            // 
            // breadCrumbs
            // 
            this.breadCrumbs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.breadCrumbs.Location = new System.Drawing.Point(41, 6);
            this.breadCrumbs.Name = "breadCrumbs";
            this.breadCrumbs.Size = new System.Drawing.Size(179, 13);
            this.breadCrumbs.TabIndex = 0;
            this.breadCrumbs.Text = "Layers";
            this.breadCrumbs.Click += new System.EventHandler(this.Minus_Click);
            this.breadCrumbs.MouseEnter += new System.EventHandler(this.HighlightLabel_MouseEnter);
            this.breadCrumbs.MouseLeave += new System.EventHandler(this.HighlightLabel_MouseLeave);
            // 
            // layerTree
            // 
            this.layerTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.layerTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.layerTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.layerTree.CheckBoxes = true;
            this.layerTree.ForeColor = System.Drawing.Color.White;
            this.layerTree.HideSelection = false;
            this.layerTree.HotTracking = true;
            this.layerTree.LineColor = System.Drawing.Color.White;
            this.layerTree.Location = new System.Drawing.Point(6, 25);
            this.layerTree.Name = "layerTree";
            this.layerTree.ShowNodeToolTips = true;
            this.layerTree.Size = new System.Drawing.Size(233, 441);
            this.layerTree.TabIndex = 0;
            this.layerTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.layerTree_AfterCheck);
            this.layerTree.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.layerTree_AfterCollapse);
            this.layerTree.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.layerTree_AfterExpand);
            this.layerTree.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.layerTree_DrawNode);
            this.layerTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.layerTree_AfterSelect);
            this.layerTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.layerTree_NodeMouseClick);
            this.layerTree.DoubleClick += new System.EventHandler(this.layerTree_DoubleClick);
            // 
            // NameValues
            // 
            this.NameValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.NameValues.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.NameValues.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NameValues.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumn,
            this.ValueColumn});
            this.NameValues.ForeColor = System.Drawing.Color.White;
            this.NameValues.FullRowSelect = true;
            this.NameValues.GridLines = true;
            this.NameValues.HideSelection = false;
            this.NameValues.Location = new System.Drawing.Point(6, 3);
            this.NameValues.MultiSelect = false;
            this.NameValues.Name = "NameValues";
            this.NameValues.Size = new System.Drawing.Size(233, 92);
            this.NameValues.TabIndex = 0;
            this.NameValues.UseCompatibleStateImageBehavior = false;
            this.NameValues.View = System.Windows.Forms.View.Details;
            this.NameValues.ItemActivate += new System.EventHandler(this.NameValues_ItemActivate);
            this.NameValues.SelectedIndexChanged += new System.EventHandler(this.NameValues_SelectedIndexChanged);
            // 
            // NameColumn
            // 
            this.NameColumn.Text = "Name";
            this.NameColumn.Width = 78;
            // 
            // ValueColumn
            // 
            this.ValueColumn.Text = "Value";
            this.ValueColumn.Width = 150;
            // 
            // startDate
            // 
            this.startDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.startDate.Location = new System.Drawing.Point(9, 605);
            this.startDate.Name = "startDate";
            this.startDate.Size = new System.Drawing.Size(115, 23);
            this.startDate.TabIndex = 1;
            this.startDate.Text = "startDate";
            // 
            // endDate
            // 
            this.endDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.endDate.Location = new System.Drawing.Point(121, 605);
            this.endDate.Name = "endDate";
            this.endDate.Size = new System.Drawing.Size(115, 13);
            this.endDate.TabIndex = 2;
            this.endDate.Text = "endDate";
            this.endDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // fadeTimer
            // 
            this.fadeTimer.Enabled = true;
            this.fadeTimer.Interval = 10;
            this.fadeTimer.Tick += new System.EventHandler(this.fadeTimer_Tick);
            // 
            // timeSeries
            // 
            this.timeSeries.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.timeSeries.Checked = false;
            this.timeSeries.Location = new System.Drawing.Point(15, 667);
            this.timeSeries.Name = "timeSeries";
            this.timeSeries.Size = new System.Drawing.Size(101, 25);
            this.timeSeries.TabIndex = 4;
            this.timeSeries.Text = "Time Series";
            this.timeSeries.CheckedChanged += new System.EventHandler(this.timeSeries_CheckedChanged);
            // 
            // autoLoopCheckbox
            // 
            this.autoLoopCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.autoLoopCheckbox.Checked = false;
            this.autoLoopCheckbox.Location = new System.Drawing.Point(139, 667);
            this.autoLoopCheckbox.Name = "autoLoopCheckbox";
            this.autoLoopCheckbox.Size = new System.Drawing.Size(97, 25);
            this.autoLoopCheckbox.TabIndex = 5;
            this.autoLoopCheckbox.Text = "Auto Loop";
            this.autoLoopCheckbox.CheckedChanged += new System.EventHandler(this.autoLoop_CheckedChanged);
            // 
            // resetLayers
            // 
            this.resetLayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resetLayers.BackColor = System.Drawing.Color.Transparent;
            this.resetLayers.DialogResult = System.Windows.Forms.DialogResult.None;
            this.resetLayers.ImageDisabled = null;
            this.resetLayers.ImageEnabled = null;
            this.resetLayers.Location = new System.Drawing.Point(177, 472);
            this.resetLayers.MaximumSize = new System.Drawing.Size(140, 33);
            this.resetLayers.Name = "resetLayers";
            this.resetLayers.Selected = false;
            this.resetLayers.Size = new System.Drawing.Size(66, 33);
            this.resetLayers.TabIndex = 9;
            this.resetLayers.Text = "Reset";
            this.resetLayers.Click += new System.EventHandler(this.ResetLayer_Click);
            // 
            // pasteLayer
            // 
            this.pasteLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pasteLayer.BackColor = System.Drawing.Color.Transparent;
            this.pasteLayer.DialogResult = System.Windows.Forms.DialogResult.None;
            this.pasteLayer.ImageDisabled = null;
            this.pasteLayer.ImageEnabled = null;
            this.pasteLayer.Location = new System.Drawing.Point(120, 472);
            this.pasteLayer.MaximumSize = new System.Drawing.Size(140, 33);
            this.pasteLayer.Name = "pasteLayer";
            this.pasteLayer.Selected = false;
            this.pasteLayer.Size = new System.Drawing.Size(62, 33);
            this.pasteLayer.TabIndex = 9;
            this.pasteLayer.Text = "Paste";
            this.pasteLayer.Click += new System.EventHandler(this.pasteLayer_Click);
            // 
            // AddLayer
            // 
            this.AddLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddLayer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.AddLayer.DialogResult = System.Windows.Forms.DialogResult.None;
            this.AddLayer.ImageDisabled = null;
            this.AddLayer.ImageEnabled = null;
            this.AddLayer.Location = new System.Drawing.Point(67, 472);
            this.AddLayer.MaximumSize = new System.Drawing.Size(140, 33);
            this.AddLayer.Name = "AddLayer";
            this.AddLayer.Selected = false;
            this.AddLayer.Size = new System.Drawing.Size(56, 33);
            this.AddLayer.TabIndex = 8;
            this.AddLayer.Text = "Add";
            this.AddLayer.Click += new System.EventHandler(this.AddLayer_Click);
            // 
            // DeleteLayer
            // 
            this.DeleteLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteLayer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.DeleteLayer.DialogResult = System.Windows.Forms.DialogResult.None;
            this.DeleteLayer.ImageDisabled = null;
            this.DeleteLayer.ImageEnabled = null;
            this.DeleteLayer.Location = new System.Drawing.Point(0, 472);
            this.DeleteLayer.MaximumSize = new System.Drawing.Size(140, 33);
            this.DeleteLayer.Name = "DeleteLayer";
            this.DeleteLayer.Selected = false;
            this.DeleteLayer.Size = new System.Drawing.Size(73, 33);
            this.DeleteLayer.TabIndex = 7;
            this.DeleteLayer.Text = "Delete";
            this.DeleteLayer.Click += new System.EventHandler(this.DeletePoint_Click);
            // 
            // SaveLayers
            // 
            this.SaveLayers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveLayers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.SaveLayers.DialogResult = System.Windows.Forms.DialogResult.None;
            this.SaveLayers.ImageDisabled = null;
            this.SaveLayers.ImageEnabled = null;
            this.SaveLayers.Location = new System.Drawing.Point(178, 648);
            this.SaveLayers.MaximumSize = new System.Drawing.Size(140, 33);
            this.SaveLayers.Name = "SaveLayers";
            this.SaveLayers.Selected = false;
            this.SaveLayers.Size = new System.Drawing.Size(69, 33);
            this.SaveLayers.TabIndex = 10;
            this.SaveLayers.Text = "Save";
            this.SaveLayers.Visible = false;
            this.SaveLayers.Click += new System.EventHandler(this.SaveFigures_Click);
            // 
            // LayerManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(248, 694);
            this.ControlBox = false;
            this.Controls.Add(this.endDate);
            this.Controls.Add(this.startDate);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.timeSeries);
            this.Controls.Add(this.autoLoopCheckbox);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.timeScrubber);
            this.Controls.Add(this.SaveLayers);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(210, 180);
            this.Name = "LayerManager";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ConstellationFigureEditor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LayerManager_FormClosed);
            this.Load += new System.EventHandler(this.Layers_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LayerManager_MouseDown);
            this.MouseEnter += new System.EventHandler(this.LayerManager_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.LayerManager_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LayerManager_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LayerManager_MouseUp);
            ((System.ComponentModel.ISupportInitialize)(this.timeScrubber)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton SaveLayers;
        private WwtButton DeleteLayer;
        private WwtButton AddLayer;
        private System.Windows.Forms.TrackBar timeScrubber;
        private System.Windows.Forms.Label timeLabel;
        private WwtButton pasteLayer;
        private WWTCheckbox autoLoopCheckbox;
        private System.Windows.Forms.Timer loopTimer;
        private WWTCheckbox timeSeries;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label breadCrumbs;
        private System.Windows.Forms.TreeView layerTree;
        private System.Windows.Forms.ListView NameValues;
        private System.Windows.Forms.ColumnHeader NameColumn;
        private System.Windows.Forms.ColumnHeader ValueColumn;
        private System.Windows.Forms.Label startDate;
        private System.Windows.Forms.Label endDate;
        private System.Windows.Forms.PictureBox closeBox;
        private System.Windows.Forms.Timer fadeTimer;
        private WwtButton resetLayers;
        private System.Windows.Forms.Label Plus;
        private System.Windows.Forms.Label Minus;
    }
}