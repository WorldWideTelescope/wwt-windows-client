namespace TerraViewer
{
    partial class SettingsTab
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.FullScreenTours = new TerraViewer.WWTCheckbox();
            this.zoomSpeed = new TerraViewer.WwtTrackBar();
            this.imageQuality = new TerraViewer.WwtTrackBar();
            this.label9 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.transparentTabs = new TerraViewer.WWTCheckbox();
            this.autoHideContext = new TerraViewer.WWTCheckbox();
            this.useFullBrowser = new TerraViewer.WWTCheckbox();
            this.autoHideTabs = new TerraViewer.WWTCheckbox();
            this.zoomToCursor = new TerraViewer.WWTCheckbox();
            this.smoothPan = new TerraViewer.WWTCheckbox();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.proxyName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.ExportCache = new TerraViewer.WwtButton();
            this.ImportCache = new TerraViewer.WwtButton();
            this.ProxyPort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.proxyText = new System.Windows.Forms.Label();
            this.ClearCache = new TerraViewer.WwtButton();
            this.ConstellationGroup = new System.Windows.Forms.GroupBox();
            this.Delete = new TerraViewer.WwtButton();
            this.EditFigure = new TerraViewer.WwtButton();
            this.newFigures = new TerraViewer.WwtButton();
            this.figureLibrary = new TerraViewer.WwtCombo();
            this.label6 = new System.Windows.Forms.Label();
            this.showCrosshairs = new TerraViewer.WWTCheckbox();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.ConstellationGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // pinUp
            // 
            this.pinUp.Enabled = false;
            this.pinUp.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.showCrosshairs);
            this.groupBox2.Controls.Add(this.FullScreenTours);
            this.groupBox2.Controls.Add(this.zoomSpeed);
            this.groupBox2.Controls.Add(this.imageQuality);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.transparentTabs);
            this.groupBox2.Controls.Add(this.autoHideContext);
            this.groupBox2.Controls.Add(this.useFullBrowser);
            this.groupBox2.Controls.Add(this.autoHideTabs);
            this.groupBox2.Controls.Add(this.zoomToCursor);
            this.groupBox2.Controls.Add(this.smoothPan);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(184, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(517, 100);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Experience";
            // 
            // FullScreenTours
            // 
            this.FullScreenTours.Checked = false;
            this.FullScreenTours.Location = new System.Drawing.Point(180, 72);
            this.FullScreenTours.Name = "FullScreenTours";
            this.FullScreenTours.Size = new System.Drawing.Size(149, 25);
            this.FullScreenTours.TabIndex = 6;
            this.FullScreenTours.Text = "Full Screen Tours";
            this.FullScreenTours.CheckedChanged += new System.EventHandler(this.FullScreenTours_CheckedChanged);
            // 
            // zoomSpeed
            // 
            this.zoomSpeed.BackColor = System.Drawing.Color.Transparent;
            this.zoomSpeed.Location = new System.Drawing.Point(38, 31);
            this.zoomSpeed.Max = 100;
            this.zoomSpeed.Name = "zoomSpeed";
            this.zoomSpeed.Size = new System.Drawing.Size(80, 20);
            this.zoomSpeed.TabIndex = 3;
            this.toolTips.SetToolTip(this.zoomSpeed, "Select how rapidly the view changes zoom levels");
            this.zoomSpeed.Value = 50;
            this.zoomSpeed.ValueChanged += new System.EventHandler(this.zoomSpeed_ValueChanged);
            // 
            // imageQuality
            // 
            this.imageQuality.BackColor = System.Drawing.Color.Transparent;
            this.imageQuality.Location = new System.Drawing.Point(38, 66);
            this.imageQuality.Max = 100;
            this.imageQuality.Name = "imageQuality";
            this.imageQuality.Size = new System.Drawing.Size(80, 20);
            this.imageQuality.TabIndex = 3;
            this.toolTips.SetToolTip(this.imageQuality, "Allows tradeoff of update performance against display quality for slower or faste" +
                    "r computers");
            this.imageQuality.Value = 50;
            this.imageQuality.ValueChanged += new System.EventHandler(this.imageQuality_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(30, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Slow";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(118, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Sharper";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(118, 32);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(27, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Fast";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(43, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Zoom Speed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Faster";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Image Quality";
            // 
            // transparentTabs
            // 
            this.transparentTabs.BackColor = System.Drawing.Color.Transparent;
            this.transparentTabs.Checked = false;
            this.transparentTabs.Location = new System.Drawing.Point(335, 53);
            this.transparentTabs.Name = "transparentTabs";
            this.transparentTabs.Size = new System.Drawing.Size(176, 25);
            this.transparentTabs.TabIndex = 1;
            this.transparentTabs.Text = "Transparent Tabs";
            this.toolTips.SetToolTip(this.transparentTabs, "Show tabs and context panel with partial transparency.");
            this.transparentTabs.CheckedChanged += new System.EventHandler(this.transparentTabs_CheckedChanged);
            // 
            // autoHideContext
            // 
            this.autoHideContext.BackColor = System.Drawing.Color.Transparent;
            this.autoHideContext.Checked = false;
            this.autoHideContext.Location = new System.Drawing.Point(335, 32);
            this.autoHideContext.Name = "autoHideContext";
            this.autoHideContext.Size = new System.Drawing.Size(176, 25);
            this.autoHideContext.TabIndex = 1;
            this.autoHideContext.Text = "Auto Hide Context";
            this.toolTips.SetToolTip(this.autoHideContext, "Fades out the context panel when the mouse is over the main view area");
            this.autoHideContext.CheckedChanged += new System.EventHandler(this.autoHideContext_CheckedChanged);
            // 
            // useFullBrowser
            // 
            this.useFullBrowser.BackColor = System.Drawing.Color.Transparent;
            this.useFullBrowser.Checked = false;
            this.useFullBrowser.Location = new System.Drawing.Point(180, 51);
            this.useFullBrowser.Name = "useFullBrowser";
            this.useFullBrowser.Size = new System.Drawing.Size(144, 25);
            this.useFullBrowser.TabIndex = 1;
            this.useFullBrowser.Text = "Full Web Browser";
            this.toolTips.SetToolTip(this.useFullBrowser, "Launches a full browser for web links rather than the web window");
            this.useFullBrowser.CheckedChanged += new System.EventHandler(this.useFullBrowser_CheckedChanged);
            // 
            // autoHideTabs
            // 
            this.autoHideTabs.BackColor = System.Drawing.Color.Transparent;
            this.autoHideTabs.Checked = false;
            this.autoHideTabs.Location = new System.Drawing.Point(335, 11);
            this.autoHideTabs.Name = "autoHideTabs";
            this.autoHideTabs.Size = new System.Drawing.Size(176, 25);
            this.autoHideTabs.TabIndex = 1;
            this.autoHideTabs.Text = "Auto Hide Tabs";
            this.toolTips.SetToolTip(this.autoHideTabs, "Fades out tab pane when the mouse is in the Field of View");
            this.autoHideTabs.CheckedChanged += new System.EventHandler(this.autoHideTabs_CheckedChanged);
            // 
            // zoomToCursor
            // 
            this.zoomToCursor.BackColor = System.Drawing.Color.Transparent;
            this.zoomToCursor.Checked = false;
            this.zoomToCursor.Location = new System.Drawing.Point(180, 32);
            this.zoomToCursor.Name = "zoomToCursor";
            this.zoomToCursor.Size = new System.Drawing.Size(149, 25);
            this.zoomToCursor.TabIndex = 1;
            this.zoomToCursor.Text = "Zoom on Mouse";
            this.toolTips.SetToolTip(this.zoomToCursor, "Follows the mouse cursor when using the mouse wheel to zoom");
            this.zoomToCursor.CheckedChanged += new System.EventHandler(this.zoomToCursor_CheckedChanged);
            // 
            // smoothPan
            // 
            this.smoothPan.BackColor = System.Drawing.Color.Transparent;
            this.smoothPan.Checked = false;
            this.smoothPan.Location = new System.Drawing.Point(180, 11);
            this.smoothPan.Name = "smoothPan";
            this.smoothPan.Size = new System.Drawing.Size(149, 25);
            this.smoothPan.TabIndex = 1;
            this.smoothPan.Text = "Smooth Panning";
            this.toolTips.SetToolTip(this.smoothPan, "Selects smooth panning rather than snapping to mouse movement");
            this.smoothPan.CheckedChanged += new System.EventHandler(this.smoothPan_CheckedChanged);
            // 
            // proxyName
            // 
            this.proxyName.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.proxyName.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.proxyName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.proxyName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.proxyName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.proxyName.ForeColor = System.Drawing.Color.White;
            this.proxyName.Location = new System.Drawing.Point(10, 31);
            this.proxyName.MaxLength = 64;
            this.proxyName.Name = "proxyName";
            this.proxyName.Size = new System.Drawing.Size(108, 20);
            this.proxyName.TabIndex = 6;
            this.toolTips.SetToolTip(this.proxyName, "Enter your proxy server name here.");
            this.proxyName.TextChanged += new System.EventHandler(this.proxyName_TextChanged);
            this.proxyName.Enter += new System.EventHandler(this.proxyName_Enter);
            this.proxyName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.proxyName_KeyDown);
            this.proxyName.Leave += new System.EventHandler(this.proxyName_Leave);
            this.proxyName.Validating += new System.ComponentModel.CancelEventHandler(this.proxyName_Validating);
            this.proxyName.Validated += new System.EventHandler(this.proxyName_Validated);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.ExportCache);
            this.groupBox3.Controls.Add(this.ImportCache);
            this.groupBox3.Controls.Add(this.ProxyPort);
            this.groupBox3.Controls.Add(this.proxyName);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.proxyText);
            this.groupBox3.Controls.Add(this.ClearCache);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(707, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(256, 100);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Network and Cache";
            // 
            // ExportCache
            // 
            this.ExportCache.BackColor = System.Drawing.Color.Transparent;
            this.ExportCache.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ExportCache.ImageDisabled = null;
            this.ExportCache.ImageEnabled = null;
            this.ExportCache.Location = new System.Drawing.Point(153, 63);
            this.ExportCache.MaximumSize = new System.Drawing.Size(140, 33);
            this.ExportCache.Name = "ExportCache";
            this.ExportCache.Selected = false;
            this.ExportCache.Size = new System.Drawing.Size(103, 33);
            this.ExportCache.TabIndex = 7;
            this.ExportCache.Text = "Export Cache";
            this.ExportCache.Click += new System.EventHandler(this.ExportCache_Click);
            // 
            // ImportCache
            // 
            this.ImportCache.BackColor = System.Drawing.Color.Transparent;
            this.ImportCache.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ImportCache.ImageDisabled = null;
            this.ImportCache.ImageEnabled = null;
            this.ImportCache.Location = new System.Drawing.Point(153, 24);
            this.ImportCache.MaximumSize = new System.Drawing.Size(140, 33);
            this.ImportCache.Name = "ImportCache";
            this.ImportCache.Selected = false;
            this.ImportCache.Size = new System.Drawing.Size(103, 33);
            this.ImportCache.TabIndex = 7;
            this.ImportCache.Text = "Import Cache";
            this.ImportCache.Click += new System.EventHandler(this.ImportCache_Click);
            // 
            // ProxyPort
            // 
            this.ProxyPort.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ProxyPort.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.ProxyPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ProxyPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ProxyPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProxyPort.ForeColor = System.Drawing.Color.White;
            this.ProxyPort.Location = new System.Drawing.Point(124, 31);
            this.ProxyPort.MaxLength = 64;
            this.ProxyPort.Name = "ProxyPort";
            this.ProxyPort.Size = new System.Drawing.Size(26, 20);
            this.ProxyPort.TabIndex = 6;
            this.ProxyPort.Text = "80";
            this.ProxyPort.TextChanged += new System.EventHandler(this.ProxyPort_TextChanged);
            this.ProxyPort.Enter += new System.EventHandler(this.ProxyPort_Enter);
            this.ProxyPort.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ProxyPort_KeyDown);
            this.ProxyPort.Leave += new System.EventHandler(this.ProxyPort_Leave);
            this.ProxyPort.Validating += new System.ComponentModel.CancelEventHandler(this.ProxyPort_Validating);
            this.ProxyPort.Validated += new System.EventHandler(this.ProxyPort_Validated);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(121, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Port";
            // 
            // proxyText
            // 
            this.proxyText.AutoSize = true;
            this.proxyText.Location = new System.Drawing.Point(7, 15);
            this.proxyText.Name = "proxyText";
            this.proxyText.Size = new System.Drawing.Size(67, 13);
            this.proxyText.TabIndex = 5;
            this.proxyText.Text = "Proxy Server";
            // 
            // ClearCache
            // 
            this.ClearCache.BackColor = System.Drawing.Color.Transparent;
            this.ClearCache.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ClearCache.ImageDisabled = null;
            this.ClearCache.ImageEnabled = null;
            this.ClearCache.Location = new System.Drawing.Point(6, 64);
            this.ClearCache.MaximumSize = new System.Drawing.Size(140, 33);
            this.ClearCache.Name = "ClearCache";
            this.ClearCache.Selected = false;
            this.ClearCache.Size = new System.Drawing.Size(140, 33);
            this.ClearCache.TabIndex = 0;
            this.ClearCache.Text = "Manage Data Cache";
            this.ClearCache.Click += new System.EventHandler(this.ClearCache_Click);
            // 
            // ConstellationGroup
            // 
            this.ConstellationGroup.BackColor = System.Drawing.Color.Transparent;
            this.ConstellationGroup.Controls.Add(this.Delete);
            this.ConstellationGroup.Controls.Add(this.EditFigure);
            this.ConstellationGroup.Controls.Add(this.newFigures);
            this.ConstellationGroup.Controls.Add(this.figureLibrary);
            this.ConstellationGroup.Controls.Add(this.label6);
            this.ConstellationGroup.ForeColor = System.Drawing.Color.White;
            this.ConstellationGroup.Location = new System.Drawing.Point(6, 4);
            this.ConstellationGroup.Name = "ConstellationGroup";
            this.ConstellationGroup.Size = new System.Drawing.Size(172, 100);
            this.ConstellationGroup.TabIndex = 9;
            this.ConstellationGroup.TabStop = false;
            this.ConstellationGroup.Text = "Constellation Lines";
            // 
            // Delete
            // 
            this.Delete.BackColor = System.Drawing.Color.Transparent;
            this.Delete.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Delete.ImageDisabled = null;
            this.Delete.ImageEnabled = null;
            this.Delete.Location = new System.Drawing.Point(106, 65);
            this.Delete.MaximumSize = new System.Drawing.Size(140, 33);
            this.Delete.Name = "Delete";
            this.Delete.Selected = false;
            this.Delete.Size = new System.Drawing.Size(61, 33);
            this.Delete.TabIndex = 4;
            this.Delete.Text = "Delete";
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // EditFigure
            // 
            this.EditFigure.BackColor = System.Drawing.Color.Transparent;
            this.EditFigure.DialogResult = System.Windows.Forms.DialogResult.None;
            this.EditFigure.ImageDisabled = null;
            this.EditFigure.ImageEnabled = null;
            this.EditFigure.Location = new System.Drawing.Point(57, 65);
            this.EditFigure.MaximumSize = new System.Drawing.Size(140, 33);
            this.EditFigure.Name = "EditFigure";
            this.EditFigure.Selected = false;
            this.EditFigure.Size = new System.Drawing.Size(54, 33);
            this.EditFigure.TabIndex = 4;
            this.EditFigure.Text = "Edit";
            this.EditFigure.Click += new System.EventHandler(this.EditFigure_Click);
            // 
            // newFigures
            // 
            this.newFigures.BackColor = System.Drawing.Color.Transparent;
            this.newFigures.DialogResult = System.Windows.Forms.DialogResult.None;
            this.newFigures.ImageDisabled = null;
            this.newFigures.ImageEnabled = null;
            this.newFigures.Location = new System.Drawing.Point(4, 65);
            this.newFigures.MaximumSize = new System.Drawing.Size(140, 33);
            this.newFigures.Name = "newFigures";
            this.newFigures.Selected = false;
            this.newFigures.Size = new System.Drawing.Size(56, 33);
            this.newFigures.TabIndex = 4;
            this.newFigures.Text = "New";
            this.newFigures.Click += new System.EventHandler(this.newFigures_Click);
            // 
            // figureLibrary
            // 
            this.figureLibrary.BackColor = System.Drawing.Color.Transparent;
            this.figureLibrary.DateTimeValue = new System.DateTime(2008, 1, 17, 18, 43, 57, 377);
            this.figureLibrary.Filter = TerraViewer.Classification.Unfiltered;
            this.figureLibrary.FilterStyle = false;
            this.figureLibrary.Location = new System.Drawing.Point(10, 32);
            this.figureLibrary.Margin = new System.Windows.Forms.Padding(0);
            this.figureLibrary.MasterTime = true;
            this.figureLibrary.MaximumSize = new System.Drawing.Size(248, 33);
            this.figureLibrary.MinimumSize = new System.Drawing.Size(35, 33);
            this.figureLibrary.Name = "figureLibrary";
            this.figureLibrary.SelectedIndex = -1;
            this.figureLibrary.SelectedItem = null;
            this.figureLibrary.Size = new System.Drawing.Size(153, 33);
            this.figureLibrary.State = TerraViewer.State.Rest;
            this.figureLibrary.TabIndex = 2;
            this.figureLibrary.Type = TerraViewer.WwtCombo.ComboType.List;
            this.figureLibrary.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.figuerLibrary_SelectionChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Figure Library";
            // 
            // showCrosshairs
            // 
            this.showCrosshairs.Checked = false;
            this.showCrosshairs.Location = new System.Drawing.Point(335, 72);
            this.showCrosshairs.Name = "showCrosshairs";
            this.showCrosshairs.Size = new System.Drawing.Size(176, 25);
            this.showCrosshairs.TabIndex = 7;
            this.showCrosshairs.Text = "Show Reticle/Crosshairs";
            this.showCrosshairs.CheckedChanged += new System.EventHandler(this.showCrosshairs_CheckedChanged);
            // 
            // SettingsTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 110);
            this.Controls.Add(this.ConstellationGroup);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Name = "SettingsTab";
            this.Opacity = 0D;
            this.Text = "SettingsTab";
            this.Deactivate += new System.EventHandler(this.SettingsTab_Deactivate);
            this.Load += new System.EventHandler(this.SettingsTab_Load);
            this.Controls.SetChildIndex(this.pinUp, 0);
            this.Controls.SetChildIndex(this.groupBox2, 0);
            this.Controls.SetChildIndex(this.groupBox3, 0);
            this.Controls.SetChildIndex(this.ConstellationGroup, 0);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ConstellationGroup.ResumeLayout(false);
            this.ConstellationGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private WWTCheckbox smoothPan;
        private WwtTrackBar imageQuality;
        private System.Windows.Forms.Label label1;
        private WwtTrackBar zoomSpeed;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private WWTCheckbox zoomToCursor;
        private WWTCheckbox autoHideContext;
        private WWTCheckbox autoHideTabs;
        private WWTCheckbox useFullBrowser;
        private System.Windows.Forms.ToolTip toolTips;
        private WWTCheckbox transparentTabs;
        private System.Windows.Forms.GroupBox groupBox3;
        private WwtButton ClearCache;
        private System.Windows.Forms.Label proxyText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox ProxyPort;
        private System.Windows.Forms.TextBox proxyName;
        private System.Windows.Forms.GroupBox ConstellationGroup;
        private WwtButton Delete;
        private WwtButton EditFigure;
        private WwtButton newFigures;
        private WwtCombo figureLibrary;
        private System.Windows.Forms.Label label6;
        private WWTCheckbox FullScreenTours;
        private WwtButton ExportCache;
        private WwtButton ImportCache;
        private WWTCheckbox showCrosshairs;
    }
}