namespace TerraViewer
{
    partial class MidiSetup
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
            this.DeviceList = new System.Windows.Forms.ListBox();
            this.MapsView = new System.Windows.Forms.ListView();
            this.Control = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Channel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Binding = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.DeviceImage = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.BindTypeLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.toolTipProvider = new System.Windows.Forms.ToolTip(this.components);
            this.Monitor = new TerraViewer.WWTCheckbox();
            this.DeviceProperties = new TerraViewer.WwtButton();
            this.RepeatCheckbox = new TerraViewer.WWTCheckbox();
            this.delete = new TerraViewer.WwtButton();
            this.BindTypeCombo = new TerraViewer.WwtCombo();
            this.TargetPropertyCombo = new TerraViewer.WwtCombo();
            this.TargetTypeCombo = new TerraViewer.WwtCombo();
            this.NewBinding = new TerraViewer.WwtButton();
            this.Save = new TerraViewer.WwtButton();
            this.LoadMap = new TerraViewer.WwtButton();
            this.PropertyNameText = new System.Windows.Forms.TextBox();
            this.filterLabel = new System.Windows.Forms.Label();
            this.filterList = new TerraViewer.WwtCombo();
            ((System.ComponentModel.ISupportInitialize)(this.DeviceImage)).BeginInit();
            this.SuspendLayout();
            // 
            // DeviceList
            // 
            this.DeviceList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.DeviceList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DeviceList.ForeColor = System.Drawing.Color.White;
            this.DeviceList.FormattingEnabled = true;
            this.DeviceList.Location = new System.Drawing.Point(12, 28);
            this.DeviceList.Name = "DeviceList";
            this.DeviceList.Size = new System.Drawing.Size(153, 145);
            this.DeviceList.TabIndex = 1;
            this.DeviceList.SelectedIndexChanged += new System.EventHandler(this.DeviceList_SelectedIndexChanged);
            // 
            // MapsView
            // 
            this.MapsView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.MapsView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MapsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Control,
            this.Channel,
            this.ID,
            this.Type,
            this.Binding});
            this.MapsView.ForeColor = System.Drawing.Color.White;
            this.MapsView.FullRowSelect = true;
            this.MapsView.GridLines = true;
            this.MapsView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.MapsView.HideSelection = false;
            this.MapsView.LabelEdit = true;
            this.MapsView.Location = new System.Drawing.Point(171, 28);
            this.MapsView.MultiSelect = false;
            this.MapsView.Name = "MapsView";
            this.MapsView.ShowGroups = false;
            this.MapsView.Size = new System.Drawing.Size(601, 210);
            this.MapsView.TabIndex = 3;
            this.MapsView.UseCompatibleStateImageBehavior = false;
            this.MapsView.View = System.Windows.Forms.View.Details;
            this.MapsView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.MapsView_AfterLabelEdit);
            this.MapsView.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.MapsView_BeforeLabelEdit);
            this.MapsView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.MapsView_ItemDrag);
            this.MapsView.SelectedIndexChanged += new System.EventHandler(this.MapsView_SelectedIndexChanged);
            // 
            // Control
            // 
            this.Control.Text = "Control";
            this.Control.Width = 69;
            // 
            // Channel
            // 
            this.Channel.Text = "Chan";
            this.Channel.Width = 40;
            // 
            // ID
            // 
            this.ID.Text = "ID";
            this.ID.Width = 30;
            // 
            // Type
            // 
            this.Type.Text = "Type";
            this.Type.Width = 80;
            // 
            // Binding
            // 
            this.Binding.Text = "Binding";
            this.Binding.Width = 350;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "MIDI Devices";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(168, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Control Bindings";
            // 
            // DeviceImage
            // 
            this.DeviceImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DeviceImage.ErrorImage = null;
            this.DeviceImage.InitialImage = null;
            this.DeviceImage.Location = new System.Drawing.Point(0, 294);
            this.DeviceImage.Name = "DeviceImage";
            this.DeviceImage.Size = new System.Drawing.Size(784, 315);
            this.DeviceImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.DeviceImage.TabIndex = 4;
            this.DeviceImage.TabStop = false;
            this.toolTipProvider.SetToolTip(this.DeviceImage, "Drag Control Bindings on image to place control");
            this.DeviceImage.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.DeviceImage_LoadCompleted);
            this.DeviceImage.Click += new System.EventHandler(this.DeviceImage_Click);
            this.DeviceImage.DragDrop += new System.Windows.Forms.DragEventHandler(this.DeviceImage_DragDrop);
            this.DeviceImage.DragEnter += new System.Windows.Forms.DragEventHandler(this.DeviceImage_DragEnter);
            this.DeviceImage.DragOver += new System.Windows.Forms.DragEventHandler(this.DeviceImage_DragOver);
            this.DeviceImage.DragLeave += new System.EventHandler(this.DeviceImage_DragLeave);
            this.DeviceImage.Paint += new System.Windows.Forms.PaintEventHandler(this.DeviceImage_Paint);
            this.DeviceImage.Resize += new System.EventHandler(this.DeviceImage_Resize);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(168, 244);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Binding Target Type";
            // 
            // BindTypeLabel
            // 
            this.BindTypeLabel.AutoSize = true;
            this.BindTypeLabel.Location = new System.Drawing.Point(326, 245);
            this.BindTypeLabel.Name = "BindTypeLabel";
            this.BindTypeLabel.Size = new System.Drawing.Size(55, 13);
            this.BindTypeLabel.TabIndex = 12;
            this.BindTypeLabel.Text = "Bind Type";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(446, 245);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Property";
            // 
            // Monitor
            // 
            this.Monitor.BackColor = System.Drawing.Color.Transparent;
            this.Monitor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Monitor.Checked = false;
            this.Monitor.Location = new System.Drawing.Point(7, 266);
            this.Monitor.Name = "Monitor";
            this.Monitor.Size = new System.Drawing.Size(87, 25);
            this.Monitor.TabIndex = 9;
            this.Monitor.Text = "Monitor";
            this.Monitor.CheckedChanged += new System.EventHandler(this.Monitor_CheckedChanged);
            // 
            // DeviceProperties
            // 
            this.DeviceProperties.BackColor = System.Drawing.Color.Transparent;
            this.DeviceProperties.DialogResult = System.Windows.Forms.DialogResult.None;
            this.DeviceProperties.ImageDisabled = null;
            this.DeviceProperties.ImageEnabled = null;
            this.DeviceProperties.Location = new System.Drawing.Point(7, 175);
            this.DeviceProperties.MaximumSize = new System.Drawing.Size(140, 33);
            this.DeviceProperties.Name = "DeviceProperties";
            this.DeviceProperties.Selected = false;
            this.DeviceProperties.Size = new System.Drawing.Size(99, 33);
            this.DeviceProperties.TabIndex = 6;
            this.DeviceProperties.Text = "Properties";
            this.DeviceProperties.Click += new System.EventHandler(this.DeviceProperties_Click);
            // 
            // RepeatCheckbox
            // 
            this.RepeatCheckbox.BackColor = System.Drawing.Color.Transparent;
            this.RepeatCheckbox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.RepeatCheckbox.Checked = false;
            this.RepeatCheckbox.Location = new System.Drawing.Point(709, 238);
            this.RepeatCheckbox.Name = "RepeatCheckbox";
            this.RepeatCheckbox.Size = new System.Drawing.Size(75, 25);
            this.RepeatCheckbox.TabIndex = 16;
            this.RepeatCheckbox.Text = "Repeat";
            this.RepeatCheckbox.CheckedChanged += new System.EventHandler(this.RepeatCheckbox_CheckedChanged);
            // 
            // delete
            // 
            this.delete.BackColor = System.Drawing.Color.Transparent;
            this.delete.DialogResult = System.Windows.Forms.DialogResult.None;
            this.delete.ImageDisabled = null;
            this.delete.ImageEnabled = null;
            this.delete.Location = new System.Drawing.Point(38, 237);
            this.delete.MaximumSize = new System.Drawing.Size(140, 33);
            this.delete.Name = "delete";
            this.delete.Selected = false;
            this.delete.Size = new System.Drawing.Size(36, 33);
            this.delete.TabIndex = 8;
            this.delete.Text = "-";
            this.delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // BindTypeCombo
            // 
            this.BindTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.BindTypeCombo.DateTimeValue = new System.DateTime(2012, 7, 3, 15, 36, 18, 947);
            this.BindTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.BindTypeCombo.FilterStyle = false;
            this.BindTypeCombo.Location = new System.Drawing.Point(329, 258);
            this.BindTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.BindTypeCombo.MasterTime = true;
            this.BindTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.BindTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.BindTypeCombo.Name = "BindTypeCombo";
            this.BindTypeCombo.SelectedIndex = -1;
            this.BindTypeCombo.SelectedItem = null;
            this.BindTypeCombo.Size = new System.Drawing.Size(117, 33);
            this.BindTypeCombo.State = TerraViewer.State.Rest;
            this.BindTypeCombo.TabIndex = 13;
            this.BindTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.BindTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.BindTypeCombo_SelectionChanged);
            // 
            // TargetPropertyCombo
            // 
            this.TargetPropertyCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.TargetPropertyCombo.DateTimeValue = new System.DateTime(2012, 7, 3, 15, 3, 58, 312);
            this.TargetPropertyCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.TargetPropertyCombo.FilterStyle = false;
            this.TargetPropertyCombo.Location = new System.Drawing.Point(449, 258);
            this.TargetPropertyCombo.Margin = new System.Windows.Forms.Padding(0);
            this.TargetPropertyCombo.MasterTime = true;
            this.TargetPropertyCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.TargetPropertyCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.TargetPropertyCombo.Name = "TargetPropertyCombo";
            this.TargetPropertyCombo.SelectedIndex = -1;
            this.TargetPropertyCombo.SelectedItem = null;
            this.TargetPropertyCombo.Size = new System.Drawing.Size(158, 33);
            this.TargetPropertyCombo.State = TerraViewer.State.Rest;
            this.TargetPropertyCombo.TabIndex = 15;
            this.TargetPropertyCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.TargetPropertyCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.TargetPropertyCombo_SelectionChanged);
            // 
            // TargetTypeCombo
            // 
            this.TargetTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.TargetTypeCombo.DateTimeValue = new System.DateTime(2012, 7, 3, 15, 3, 58, 312);
            this.TargetTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.TargetTypeCombo.FilterStyle = false;
            this.TargetTypeCombo.Location = new System.Drawing.Point(171, 258);
            this.TargetTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.TargetTypeCombo.MasterTime = true;
            this.TargetTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.TargetTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.TargetTypeCombo.Name = "TargetTypeCombo";
            this.TargetTypeCombo.SelectedIndex = -1;
            this.TargetTypeCombo.SelectedItem = null;
            this.TargetTypeCombo.Size = new System.Drawing.Size(158, 33);
            this.TargetTypeCombo.State = TerraViewer.State.Rest;
            this.TargetTypeCombo.TabIndex = 11;
            this.TargetTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.TargetTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.TargetTypeCombo_SelectionChanged);
            // 
            // NewBinding
            // 
            this.NewBinding.BackColor = System.Drawing.Color.Transparent;
            this.NewBinding.DialogResult = System.Windows.Forms.DialogResult.None;
            this.NewBinding.ImageDisabled = null;
            this.NewBinding.ImageEnabled = null;
            this.NewBinding.Location = new System.Drawing.Point(6, 237);
            this.NewBinding.MaximumSize = new System.Drawing.Size(140, 33);
            this.NewBinding.Name = "NewBinding";
            this.NewBinding.Selected = false;
            this.NewBinding.Size = new System.Drawing.Size(36, 33);
            this.NewBinding.TabIndex = 7;
            this.NewBinding.Text = "+";
            this.NewBinding.Click += new System.EventHandler(this.NewBinding_Click);
            // 
            // Save
            // 
            this.Save.BackColor = System.Drawing.Color.Transparent;
            this.Save.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Save.ImageDisabled = null;
            this.Save.ImageEnabled = null;
            this.Save.Location = new System.Drawing.Point(88, 207);
            this.Save.MaximumSize = new System.Drawing.Size(140, 33);
            this.Save.Name = "Save";
            this.Save.Selected = false;
            this.Save.Size = new System.Drawing.Size(82, 33);
            this.Save.TabIndex = 5;
            this.Save.Text = "Save";
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // LoadMap
            // 
            this.LoadMap.BackColor = System.Drawing.Color.Transparent;
            this.LoadMap.DialogResult = System.Windows.Forms.DialogResult.None;
            this.LoadMap.ImageDisabled = null;
            this.LoadMap.ImageEnabled = null;
            this.LoadMap.Location = new System.Drawing.Point(6, 207);
            this.LoadMap.MaximumSize = new System.Drawing.Size(140, 33);
            this.LoadMap.Name = "LoadMap";
            this.LoadMap.Selected = false;
            this.LoadMap.Size = new System.Drawing.Size(81, 33);
            this.LoadMap.TabIndex = 4;
            this.LoadMap.Text = "Load";
            this.LoadMap.Click += new System.EventHandler(this.Load_Click);
            // 
            // PropertyNameText
            // 
            this.PropertyNameText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.PropertyNameText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PropertyNameText.ForeColor = System.Drawing.Color.White;
            this.PropertyNameText.Location = new System.Drawing.Point(449, 264);
            this.PropertyNameText.Name = "PropertyNameText";
            this.PropertyNameText.Size = new System.Drawing.Size(152, 20);
            this.PropertyNameText.TabIndex = 17;
            this.PropertyNameText.Visible = false;
            this.PropertyNameText.TextChanged += new System.EventHandler(this.PropertyNameText_TextChanged);
            // 
            // filterLabel
            // 
            this.filterLabel.AutoSize = true;
            this.filterLabel.Location = new System.Drawing.Point(613, 245);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(34, 13);
            this.filterLabel.TabIndex = 14;
            this.filterLabel.Text = "Value";
            // 
            // filterList
            // 
            this.filterList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.filterList.DateTimeValue = new System.DateTime(2014, 8, 8, 8, 48, 31, 871);
            this.filterList.Filter = TerraViewer.Classification.Unfiltered;
            this.filterList.FilterStyle = false;
            this.filterList.Location = new System.Drawing.Point(616, 258);
            this.filterList.Margin = new System.Windows.Forms.Padding(0);
            this.filterList.MasterTime = true;
            this.filterList.MaximumSize = new System.Drawing.Size(248, 33);
            this.filterList.MinimumSize = new System.Drawing.Size(35, 33);
            this.filterList.Name = "filterList";
            this.filterList.SelectedIndex = -1;
            this.filterList.SelectedItem = null;
            this.filterList.Size = new System.Drawing.Size(155, 33);
            this.filterList.State = TerraViewer.State.Rest;
            this.filterList.TabIndex = 18;
            this.filterList.Type = TerraViewer.WwtCombo.ComboType.List;
            this.filterList.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.filterList_SelectionChanged);
            // 
            // MidiSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(784, 609);
            this.Controls.Add(this.filterList);
            this.Controls.Add(this.Monitor);
            this.Controls.Add(this.DeviceProperties);
            this.Controls.Add(this.RepeatCheckbox);
            this.Controls.Add(this.delete);
            this.Controls.Add(this.BindTypeCombo);
            this.Controls.Add(this.TargetPropertyCombo);
            this.Controls.Add(this.TargetTypeCombo);
            this.Controls.Add(this.NewBinding);
            this.Controls.Add(this.Save);
            this.Controls.Add(this.LoadMap);
            this.Controls.Add(this.filterLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.BindTypeLabel);
            this.Controls.Add(this.DeviceImage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MapsView);
            this.Controls.Add(this.DeviceList);
            this.Controls.Add(this.PropertyNameText);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::TerraViewer.Properties.Settings.Default, "MidiWindowLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Location = global::TerraViewer.Properties.Settings.Default.MidiWindowLocation;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 2048);
            this.MinimumSize = new System.Drawing.Size(800, 400);
            this.Name = "MidiSetup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Controller Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MidiSetup_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MidiSetup_FormClosed);
            this.Load += new System.EventHandler(this.MidiSetup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DeviceImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox DeviceList;
        private System.Windows.Forms.ListView MapsView;
        private System.Windows.Forms.ColumnHeader Control;
        private System.Windows.Forms.ColumnHeader Type;
        private System.Windows.Forms.ColumnHeader Binding;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox DeviceImage;
        private WwtButton LoadMap;
        private WwtButton Save;
        private WwtButton NewBinding;
        private System.Windows.Forms.ColumnHeader ID;
        private WwtCombo TargetTypeCombo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label BindTypeLabel;
        private WwtCombo TargetPropertyCombo;
        private WwtCombo BindTypeCombo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ColumnHeader Channel;
        private WwtButton delete;
        private WWTCheckbox RepeatCheckbox;
        private WwtButton DeviceProperties;
        private System.Windows.Forms.ToolTip toolTipProvider;
        private WWTCheckbox Monitor;
        private System.Windows.Forms.TextBox PropertyNameText;
        private System.Windows.Forms.Label filterLabel;
        private WwtCombo filterList;
    }
}