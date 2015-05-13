namespace TerraViewer
{
    partial class DataWizardMarkers
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.selectMarkerLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.markerType = new TerraViewer.WwtCombo();
            this.markerMix = new TerraViewer.WwtCombo();
            this.label1 = new System.Windows.Forms.Label();
            this.markerSelect = new System.Windows.Forms.PictureBox();
            this.markerColumn = new TerraViewer.WwtCombo();
            this.label2 = new System.Windows.Forms.Label();
            this.domainList = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ShowFarSide = new TerraViewer.WWTCheckbox();
            ((System.ComponentModel.ISupportInitialize)(this.markerSelect)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(32, 23);
            this.imageList.TransparentColor = System.Drawing.Color.Black;
            // 
            // selectMarkerLabel
            // 
            this.selectMarkerLabel.AutoSize = true;
            this.selectMarkerLabel.Location = new System.Drawing.Point(296, 55);
            this.selectMarkerLabel.Name = "selectMarkerLabel";
            this.selectMarkerLabel.Size = new System.Drawing.Size(82, 13);
            this.selectMarkerLabel.TabIndex = 7;
            this.selectMarkerLabel.Text = "Select a Marker";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(154, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Marker Type";
            // 
            // markerType
            // 
            this.markerType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.markerType.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.markerType.Filter = TerraViewer.Classification.Unfiltered;
            this.markerType.FilterStyle = false;
            this.markerType.Location = new System.Drawing.Point(157, 71);
            this.markerType.Margin = new System.Windows.Forms.Padding(0);
            this.markerType.MasterTime = true;
            this.markerType.MaximumSize = new System.Drawing.Size(248, 33);
            this.markerType.MinimumSize = new System.Drawing.Size(35, 33);
            this.markerType.Name = "markerType";
            this.markerType.SelectedIndex = -1;
            this.markerType.SelectedItem = null;
            this.markerType.Size = new System.Drawing.Size(126, 33);
            this.markerType.State = TerraViewer.State.Rest;
            this.markerType.TabIndex = 6;
            this.markerType.Type = TerraViewer.WwtCombo.ComboType.List;
            this.markerType.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.markerType_SelectionChanged);
            // 
            // markerMix
            // 
            this.markerMix.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.markerMix.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.markerMix.Filter = TerraViewer.Classification.Unfiltered;
            this.markerMix.FilterStyle = false;
            this.markerMix.Location = new System.Drawing.Point(22, 72);
            this.markerMix.Margin = new System.Windows.Forms.Padding(0);
            this.markerMix.MasterTime = true;
            this.markerMix.MaximumSize = new System.Drawing.Size(248, 33);
            this.markerMix.MinimumSize = new System.Drawing.Size(35, 33);
            this.markerMix.Name = "markerMix";
            this.markerMix.SelectedIndex = -1;
            this.markerMix.SelectedItem = null;
            this.markerMix.Size = new System.Drawing.Size(126, 33);
            this.markerMix.State = TerraViewer.State.Rest;
            this.markerMix.TabIndex = 2;
            this.markerMix.Type = TerraViewer.WwtCombo.ComboType.List;
            this.markerMix.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.markerMix_SelectionChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Marker Mix";
            // 
            // markerSelect
            // 
            this.markerSelect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.markerSelect.Location = new System.Drawing.Point(316, 71);
            this.markerSelect.Name = "markerSelect";
            this.markerSelect.Size = new System.Drawing.Size(34, 34);
            this.markerSelect.TabIndex = 27;
            this.markerSelect.TabStop = false;
            this.markerSelect.Click += new System.EventHandler(this.markerSelect_Click);
            // 
            // markerColumn
            // 
            this.markerColumn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.markerColumn.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.markerColumn.Filter = TerraViewer.Classification.Unfiltered;
            this.markerColumn.FilterStyle = false;
            this.markerColumn.Location = new System.Drawing.Point(22, 135);
            this.markerColumn.Margin = new System.Windows.Forms.Padding(0);
            this.markerColumn.MasterTime = true;
            this.markerColumn.MaximumSize = new System.Drawing.Size(248, 33);
            this.markerColumn.MinimumSize = new System.Drawing.Size(35, 33);
            this.markerColumn.Name = "markerColumn";
            this.markerColumn.SelectedIndex = -1;
            this.markerColumn.SelectedItem = null;
            this.markerColumn.Size = new System.Drawing.Size(126, 33);
            this.markerColumn.State = TerraViewer.State.Rest;
            this.markerColumn.TabIndex = 4;
            this.markerColumn.Type = TerraViewer.WwtCombo.ComboType.List;
            this.markerColumn.Visible = false;
            this.markerColumn.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.markerColumn_SelectionChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 119);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Marker Column";
            this.label2.Visible = false;
            // 
            // domainList
            // 
            this.domainList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.domainList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.domainList.ForeColor = System.Drawing.Color.White;
            this.domainList.FormattingEnabled = true;
            this.domainList.ItemHeight = 36;
            this.domainList.Location = new System.Drawing.Point(392, 67);
            this.domainList.Name = "domainList";
            this.domainList.Size = new System.Drawing.Size(279, 148);
            this.domainList.TabIndex = 8;
            this.domainList.Visible = false;
            this.domainList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
            this.domainList.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox1_MeasureItem);
            this.domainList.SelectedIndexChanged += new System.EventHandler(this.domainList_SelectedIndexChanged);
            this.domainList.DoubleClick += new System.EventHandler(this.domainList_DoubleClick);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(22, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(649, 37);
            this.label3.TabIndex = 0;
            this.label3.Text = "Select the way you want to organize markers and their shapes. You can use one mar" +
                "ker type for all the data or select a column to show different markers for range" +
                "s of values, or discrete values. ";
            // 
            // ShowFarSide
            // 
            this.ShowFarSide.Checked = false;
            this.ShowFarSide.Location = new System.Drawing.Point(165, 135);
            this.ShowFarSide.Name = "ShowFarSide";
            this.ShowFarSide.Size = new System.Drawing.Size(152, 25);
            this.ShowFarSide.TabIndex = 28;
            this.ShowFarSide.Text = "Show Far Side Markers";
            this.ShowFarSide.CheckedChanged += new System.EventHandler(this.ShowFarSide_CheckedChanged);
            // 
            // DataWizardMarkers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ShowFarSide);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.domainList);
            this.Controls.Add(this.markerSelect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.markerColumn);
            this.Controls.Add(this.markerMix);
            this.Controls.Add(this.markerType);
            this.Controls.Add(this.selectMarkerLabel);
            this.Name = "DataWizardMarkers";
            this.Load += new System.EventHandler(this.DataWizardMarkers_Load);
            ((System.ComponentModel.ISupportInitialize)(this.markerSelect)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Label selectMarkerLabel;
        private System.Windows.Forms.Label label5;
        private WwtCombo markerType;
        private WwtCombo markerMix;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox markerSelect;
        private WwtCombo markerColumn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox domainList;
        private System.Windows.Forms.Label label3;
        private WWTCheckbox ShowFarSide;

    }
}
