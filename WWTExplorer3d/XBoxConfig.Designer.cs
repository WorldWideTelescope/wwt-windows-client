namespace TerraViewer
{
    partial class XBoxConfig
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.MapsView = new System.Windows.Forms.ListView();
            this.Control = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Binding = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Repeat = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Property = new System.Windows.Forms.Label();
            this.BindTypeLabel = new System.Windows.Forms.Label();
            this.BindingTargetTypeLabel = new System.Windows.Forms.Label();
            this.PropertyNameText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.modeLabel = new System.Windows.Forms.Label();
            this.export = new TerraViewer.WwtButton();
            this.Import = new TerraViewer.WwtButton();
            this.RepeatCheckbox = new TerraViewer.WWTCheckbox();
            this.ModeCombo = new TerraViewer.WwtCombo();
            this.ModeDependentMaps = new TerraViewer.WWTCheckbox();
            this.UseCustomMaps = new TerraViewer.WWTCheckbox();
            this.BindTypeCombo = new TerraViewer.WwtCombo();
            this.TargetPropertyCombo = new TerraViewer.WwtCombo();
            this.TargetTypeCombo = new TerraViewer.WwtCombo();
            this.Cancel = new TerraViewer.WwtButton();
            this.OK = new TerraViewer.WwtButton();
            this.filterList = new TerraViewer.WwtCombo();
            this.filterLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::TerraViewer.Properties.Resources.xbox3601;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(443, 248);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // MapsView
            // 
            this.MapsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.MapsView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.MapsView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MapsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Control,
            this.Binding,
            this.Repeat});
            this.MapsView.ForeColor = System.Drawing.Color.White;
            this.MapsView.FullRowSelect = true;
            this.MapsView.GridLines = true;
            this.MapsView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.MapsView.HideSelection = false;
            this.MapsView.LabelWrap = false;
            this.MapsView.Location = new System.Drawing.Point(13, 321);
            this.MapsView.MultiSelect = false;
            this.MapsView.Name = "MapsView";
            this.MapsView.ShowGroups = false;
            this.MapsView.Size = new System.Drawing.Size(445, 276);
            this.MapsView.TabIndex = 4;
            this.MapsView.UseCompatibleStateImageBehavior = false;
            this.MapsView.View = System.Windows.Forms.View.Details;
            this.MapsView.SelectedIndexChanged += new System.EventHandler(this.MapsView_SelectedIndexChanged);
            // 
            // Control
            // 
            this.Control.Text = "Control";
            this.Control.Width = 106;
            // 
            // Binding
            // 
            this.Binding.Text = "Binding";
            this.Binding.Width = 288;
            // 
            // Repeat
            // 
            this.Repeat.Text = "Repeat";
            this.Repeat.Width = 49;
            // 
            // Property
            // 
            this.Property.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Property.AutoSize = true;
            this.Property.Location = new System.Drawing.Point(255, 601);
            this.Property.Name = "Property";
            this.Property.Size = new System.Drawing.Size(46, 13);
            this.Property.TabIndex = 9;
            this.Property.Text = "Property";
            // 
            // BindTypeLabel
            // 
            this.BindTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BindTypeLabel.AutoSize = true;
            this.BindTypeLabel.Location = new System.Drawing.Point(151, 601);
            this.BindTypeLabel.Name = "BindTypeLabel";
            this.BindTypeLabel.Size = new System.Drawing.Size(55, 13);
            this.BindTypeLabel.TabIndex = 7;
            this.BindTypeLabel.Text = "Bind Type";
            // 
            // BindingTargetTypeLabel
            // 
            this.BindingTargetTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BindingTargetTypeLabel.AutoSize = true;
            this.BindingTargetTypeLabel.Location = new System.Drawing.Point(7, 600);
            this.BindingTargetTypeLabel.Name = "BindingTargetTypeLabel";
            this.BindingTargetTypeLabel.Size = new System.Drawing.Size(103, 13);
            this.BindingTargetTypeLabel.TabIndex = 5;
            this.BindingTargetTypeLabel.Text = "Binding Target Type";
            // 
            // PropertyNameText
            // 
            this.PropertyNameText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PropertyNameText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.PropertyNameText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PropertyNameText.ForeColor = System.Drawing.Color.White;
            this.PropertyNameText.Location = new System.Drawing.Point(259, 620);
            this.PropertyNameText.Name = "PropertyNameText";
            this.PropertyNameText.Size = new System.Drawing.Size(194, 20);
            this.PropertyNameText.TabIndex = 11;
            this.PropertyNameText.Visible = false;
            this.PropertyNameText.TextChanged += new System.EventHandler(this.PropertyNameText_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Left Sholder";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(132, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Left Trigger";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(274, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Right Trigger";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(348, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(71, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Right Sholder";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(386, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 37);
            this.label7.TabIndex = 23;
            this.label7.Text = "A, B, X, Y Buttons";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(231, 226);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Right Thumb";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(171, 226);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "D-Pad";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(26, 121);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(61, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Left Thumb";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Transparent;
            this.label11.Location = new System.Drawing.Point(183, 49);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 13);
            this.label11.TabIndex = 19;
            this.label11.Text = "Back";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(239, 49);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(29, 13);
            this.label12.TabIndex = 20;
            this.label12.Text = "Start";
            // 
            // modeLabel
            // 
            this.modeLabel.AutoSize = true;
            this.modeLabel.Location = new System.Drawing.Point(215, 266);
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.Size = new System.Drawing.Size(34, 13);
            this.modeLabel.TabIndex = 2;
            this.modeLabel.Text = "Mode";
            // 
            // export
            // 
            this.export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.export.BackColor = System.Drawing.Color.Transparent;
            this.export.DialogResult = System.Windows.Forms.DialogResult.None;
            this.export.ImageDisabled = null;
            this.export.ImageEnabled = null;
            this.export.Location = new System.Drawing.Point(225, 696);
            this.export.MaximumSize = new System.Drawing.Size(140, 33);
            this.export.Name = "export";
            this.export.Selected = false;
            this.export.Size = new System.Drawing.Size(76, 33);
            this.export.TabIndex = 13;
            this.export.Text = "Export";
            this.export.Click += new System.EventHandler(this.export_Click);
            // 
            // Import
            // 
            this.Import.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Import.BackColor = System.Drawing.Color.Transparent;
            this.Import.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Import.ImageDisabled = null;
            this.Import.ImageEnabled = null;
            this.Import.Location = new System.Drawing.Point(150, 696);
            this.Import.MaximumSize = new System.Drawing.Size(140, 33);
            this.Import.Name = "Import";
            this.Import.Selected = false;
            this.Import.Size = new System.Drawing.Size(78, 33);
            this.Import.TabIndex = 12;
            this.Import.Text = "Import";
            this.Import.Click += new System.EventHandler(this.Import_Click);
            // 
            // RepeatCheckbox
            // 
            this.RepeatCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RepeatCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.RepeatCheckbox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.RepeatCheckbox.Checked = false;
            this.RepeatCheckbox.Location = new System.Drawing.Point(13, 660);
            this.RepeatCheckbox.Name = "RepeatCheckbox";
            this.RepeatCheckbox.Size = new System.Drawing.Size(75, 25);
            this.RepeatCheckbox.TabIndex = 11;
            this.RepeatCheckbox.Text = "Repeat";
            this.RepeatCheckbox.CheckedChanged += new System.EventHandler(this.RepeatCheckbox_CheckedChanged);
            // 
            // ModeCombo
            // 
            this.ModeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ModeCombo.DateTimeValue = new System.DateTime(2013, 12, 1, 10, 37, 54, 89);
            this.ModeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.ModeCombo.FilterStyle = false;
            this.ModeCombo.Location = new System.Drawing.Point(217, 282);
            this.ModeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.ModeCombo.MasterTime = true;
            this.ModeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.ModeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.ModeCombo.Name = "ModeCombo";
            this.ModeCombo.SelectedIndex = -1;
            this.ModeCombo.SelectedItem = null;
            this.ModeCombo.Size = new System.Drawing.Size(244, 33);
            this.ModeCombo.State = TerraViewer.State.Rest;
            this.ModeCombo.TabIndex = 3;
            this.ModeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.ModeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.ModeCombo_SelectionChanged);
            // 
            // ModeDependentMaps
            // 
            this.ModeDependentMaps.BackColor = System.Drawing.Color.Transparent;
            this.ModeDependentMaps.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ModeDependentMaps.Checked = false;
            this.ModeDependentMaps.Location = new System.Drawing.Point(7, 290);
            this.ModeDependentMaps.Name = "ModeDependentMaps";
            this.ModeDependentMaps.Size = new System.Drawing.Size(201, 25);
            this.ModeDependentMaps.TabIndex = 1;
            this.ModeDependentMaps.Text = "Use Mode Dependent Mappings";
            this.ModeDependentMaps.CheckedChanged += new System.EventHandler(this.ModeDependentMaps_CheckedChanged);
            // 
            // UseCustomMaps
            // 
            this.UseCustomMaps.BackColor = System.Drawing.Color.Transparent;
            this.UseCustomMaps.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.UseCustomMaps.Checked = false;
            this.UseCustomMaps.Location = new System.Drawing.Point(7, 266);
            this.UseCustomMaps.Name = "UseCustomMaps";
            this.UseCustomMaps.Size = new System.Drawing.Size(152, 25);
            this.UseCustomMaps.TabIndex = 0;
            this.UseCustomMaps.Text = "Use Custom Mappings";
            this.UseCustomMaps.CheckedChanged += new System.EventHandler(this.UseCustomMaps_CheckedChanged);
            // 
            // BindTypeCombo
            // 
            this.BindTypeCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.BindTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.BindTypeCombo.DateTimeValue = new System.DateTime(2012, 7, 3, 15, 36, 18, 947);
            this.BindTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.BindTypeCombo.FilterStyle = false;
            this.BindTypeCombo.Location = new System.Drawing.Point(154, 614);
            this.BindTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.BindTypeCombo.MasterTime = true;
            this.BindTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.BindTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.BindTypeCombo.Name = "BindTypeCombo";
            this.BindTypeCombo.SelectedIndex = -1;
            this.BindTypeCombo.SelectedItem = null;
            this.BindTypeCombo.Size = new System.Drawing.Size(99, 33);
            this.BindTypeCombo.State = TerraViewer.State.Rest;
            this.BindTypeCombo.TabIndex = 8;
            this.BindTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.BindTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.BindTypeCombo_SelectionChanged);
            // 
            // TargetPropertyCombo
            // 
            this.TargetPropertyCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TargetPropertyCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.TargetPropertyCombo.DateTimeValue = new System.DateTime(2012, 7, 3, 15, 3, 58, 312);
            this.TargetPropertyCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.TargetPropertyCombo.FilterStyle = false;
            this.TargetPropertyCombo.Location = new System.Drawing.Point(258, 614);
            this.TargetPropertyCombo.Margin = new System.Windows.Forms.Padding(0);
            this.TargetPropertyCombo.MasterTime = true;
            this.TargetPropertyCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.TargetPropertyCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.TargetPropertyCombo.Name = "TargetPropertyCombo";
            this.TargetPropertyCombo.SelectedIndex = -1;
            this.TargetPropertyCombo.SelectedItem = null;
            this.TargetPropertyCombo.Size = new System.Drawing.Size(205, 33);
            this.TargetPropertyCombo.State = TerraViewer.State.Rest;
            this.TargetPropertyCombo.TabIndex = 10;
            this.TargetPropertyCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.TargetPropertyCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.TargetPropertyCombo_SelectionChanged);
            // 
            // TargetTypeCombo
            // 
            this.TargetTypeCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.TargetTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.TargetTypeCombo.DateTimeValue = new System.DateTime(2012, 7, 3, 15, 3, 58, 312);
            this.TargetTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.TargetTypeCombo.FilterStyle = false;
            this.TargetTypeCombo.Location = new System.Drawing.Point(10, 614);
            this.TargetTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.TargetTypeCombo.MasterTime = true;
            this.TargetTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.TargetTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.TargetTypeCombo.Name = "TargetTypeCombo";
            this.TargetTypeCombo.SelectedIndex = -1;
            this.TargetTypeCombo.SelectedItem = null;
            this.TargetTypeCombo.Size = new System.Drawing.Size(140, 33);
            this.TargetTypeCombo.State = TerraViewer.State.Rest;
            this.TargetTypeCombo.TabIndex = 6;
            this.TargetTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.TargetTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.TargetTypeCombo_SelectionChanged);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(391, 696);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(72, 33);
            this.Cancel.TabIndex = 15;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.None;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(320, 696);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(73, 33);
            this.OK.TabIndex = 14;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // filterList
            // 
            this.filterList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.filterList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.filterList.DateTimeValue = new System.DateTime(2014, 8, 8, 8, 48, 31, 871);
            this.filterList.Filter = TerraViewer.Classification.Unfiltered;
            this.filterList.FilterStyle = false;
            this.filterList.Location = new System.Drawing.Point(258, 662);
            this.filterList.Margin = new System.Windows.Forms.Padding(0);
            this.filterList.MasterTime = true;
            this.filterList.MaximumSize = new System.Drawing.Size(248, 33);
            this.filterList.MinimumSize = new System.Drawing.Size(35, 33);
            this.filterList.Name = "filterList";
            this.filterList.SelectedIndex = -1;
            this.filterList.SelectedItem = null;
            this.filterList.Size = new System.Drawing.Size(205, 33);
            this.filterList.State = TerraViewer.State.Rest;
            this.filterList.TabIndex = 27;
            this.filterList.Type = TerraViewer.WwtCombo.ComboType.List;
            this.filterList.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.filterList_SelectionChanged);
            // 
            // filterLabel
            // 
            this.filterLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.filterLabel.AutoSize = true;
            this.filterLabel.Location = new System.Drawing.Point(255, 649);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(34, 13);
            this.filterLabel.TabIndex = 26;
            this.filterLabel.Text = "Value";
            this.filterLabel.Click += new System.EventHandler(this.filterLabel_Click);
            // 
            // XBoxConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(470, 741);
            this.Controls.Add(this.filterList);
            this.Controls.Add(this.filterLabel);
            this.Controls.Add(this.export);
            this.Controls.Add(this.Import);
            this.Controls.Add(this.RepeatCheckbox);
            this.Controls.Add(this.modeLabel);
            this.Controls.Add(this.ModeCombo);
            this.Controls.Add(this.ModeDependentMaps);
            this.Controls.Add(this.UseCustomMaps);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BindTypeCombo);
            this.Controls.Add(this.TargetPropertyCombo);
            this.Controls.Add(this.TargetTypeCombo);
            this.Controls.Add(this.Property);
            this.Controls.Add(this.BindTypeLabel);
            this.Controls.Add(this.BindingTargetTypeLabel);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.PropertyNameText);
            this.Controls.Add(this.MapsView);
            this.Controls.Add(this.pictureBox1);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(486, 780);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(486, 500);
            this.Name = "XBoxConfig";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "XBox 360 Controller Configuration";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.XBoxConfig_FormClosed);
            this.Load += new System.EventHandler(this.XBoxConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListView MapsView;
        private System.Windows.Forms.ColumnHeader Control;
        private System.Windows.Forms.ColumnHeader Binding;
        private WwtCombo BindTypeCombo;
        private WwtCombo TargetPropertyCombo;
        private WwtCombo TargetTypeCombo;
        private System.Windows.Forms.Label Property;
        private System.Windows.Forms.Label BindTypeLabel;
        private System.Windows.Forms.Label BindingTargetTypeLabel;
        private WwtButton Cancel;
        private WwtButton OK;
        private System.Windows.Forms.TextBox PropertyNameText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private WWTCheckbox UseCustomMaps;
        private WWTCheckbox ModeDependentMaps;
        private WwtCombo ModeCombo;
        private System.Windows.Forms.Label modeLabel;
        private WWTCheckbox RepeatCheckbox;
        private System.Windows.Forms.ColumnHeader Repeat;
        private WwtButton Import;
        private WwtButton export;
        private WwtCombo filterList;
        private System.Windows.Forms.Label filterLabel;
    }
}