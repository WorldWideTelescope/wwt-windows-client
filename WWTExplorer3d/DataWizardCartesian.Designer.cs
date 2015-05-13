namespace TerraViewer
{
    partial class DataWizardCartesian
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
            this.label3 = new System.Windows.Forms.Label();
            this.zAxisLabel = new System.Windows.Forms.Label();
            this.yAxisLabel = new System.Windows.Forms.Label();
            this.xAxisLabel = new System.Windows.Forms.Label();
            this.zAxisColumn = new TerraViewer.WwtCombo();
            this.yAxisColumn = new TerraViewer.WwtCombo();
            this.xAxisColumn = new TerraViewer.WwtCombo();
            this.reverseXCheckbox = new TerraViewer.WWTCheckbox();
            this.reverseYCheckbox = new TerraViewer.WWTCheckbox();
            this.reverseZCheckbox = new TerraViewer.WWTCheckbox();
            this.label2 = new System.Windows.Forms.Label();
            this.AltDepthUnitsCombo = new TerraViewer.WwtCombo();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(21, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(604, 47);
            this.label3.TabIndex = 0;
            this.label3.Text = "Select the Columns for cartesian coordinates for X, Y and optionally Z. The reverse checkbox may be used to match the sign vector for each axis. ";
            // 
            // zAxisLabel
            // 
            this.zAxisLabel.AutoSize = true;
            this.zAxisLabel.Location = new System.Drawing.Point(273, 62);
            this.zAxisLabel.Name = "zAxisLabel";
            this.zAxisLabel.Size = new System.Drawing.Size(120, 13);
            this.zAxisLabel.TabIndex = 7;
            this.zAxisLabel.Text = "Z Axis Column (optional)";
            // 
            // yAxisLabel
            // 
            this.yAxisLabel.AutoSize = true;
            this.yAxisLabel.Location = new System.Drawing.Point(147, 62);
            this.yAxisLabel.Name = "yAxisLabel";
            this.yAxisLabel.Size = new System.Drawing.Size(74, 13);
            this.yAxisLabel.TabIndex = 4;
            this.yAxisLabel.Text = "Y Axis Column";
            // 
            // xAxisLabel
            // 
            this.xAxisLabel.AutoSize = true;
            this.xAxisLabel.Location = new System.Drawing.Point(21, 62);
            this.xAxisLabel.Name = "xAxisLabel";
            this.xAxisLabel.Size = new System.Drawing.Size(74, 13);
            this.xAxisLabel.TabIndex = 1;
            this.xAxisLabel.Text = "X Axis Column";
            // 
            // zAxisColumn
            // 
            this.zAxisColumn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.zAxisColumn.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.zAxisColumn.Filter = TerraViewer.Classification.Unfiltered;
            this.zAxisColumn.FilterStyle = false;
            this.zAxisColumn.Location = new System.Drawing.Point(276, 78);
            this.zAxisColumn.Margin = new System.Windows.Forms.Padding(0);
            this.zAxisColumn.MaximumSize = new System.Drawing.Size(248, 33);
            this.zAxisColumn.MinimumSize = new System.Drawing.Size(35, 33);
            this.zAxisColumn.Name = "zAxisColumn";
            this.zAxisColumn.SelectedIndex = -1;
            this.zAxisColumn.SelectedItem = null;
            this.zAxisColumn.Size = new System.Drawing.Size(126, 33);
            this.zAxisColumn.State = TerraViewer.State.Rest;
            this.zAxisColumn.TabIndex = 8;
            this.zAxisColumn.Type = TerraViewer.WwtCombo.ComboType.List;
            this.zAxisColumn.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.zAxisColumn_SelectionChanged);
            // 
            // yAxisColumn
            // 
            this.yAxisColumn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.yAxisColumn.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.yAxisColumn.Filter = TerraViewer.Classification.Unfiltered;
            this.yAxisColumn.FilterStyle = false;
            this.yAxisColumn.Location = new System.Drawing.Point(150, 78);
            this.yAxisColumn.Margin = new System.Windows.Forms.Padding(0);
            this.yAxisColumn.MaximumSize = new System.Drawing.Size(248, 33);
            this.yAxisColumn.MinimumSize = new System.Drawing.Size(35, 33);
            this.yAxisColumn.Name = "yAxisColumn";
            this.yAxisColumn.SelectedIndex = -1;
            this.yAxisColumn.SelectedItem = null;
            this.yAxisColumn.Size = new System.Drawing.Size(126, 33);
            this.yAxisColumn.State = TerraViewer.State.Rest;
            this.yAxisColumn.TabIndex = 5;
            this.yAxisColumn.Type = TerraViewer.WwtCombo.ComboType.List;
            this.yAxisColumn.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.yAxisColumn_SelectionChanged);
            // 
            // xAxisColumn
            // 
            this.xAxisColumn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.xAxisColumn.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.xAxisColumn.Filter = TerraViewer.Classification.Unfiltered;
            this.xAxisColumn.FilterStyle = false;
            this.xAxisColumn.Location = new System.Drawing.Point(24, 78);
            this.xAxisColumn.Margin = new System.Windows.Forms.Padding(0);
            this.xAxisColumn.MaximumSize = new System.Drawing.Size(248, 33);
            this.xAxisColumn.MinimumSize = new System.Drawing.Size(35, 33);
            this.xAxisColumn.Name = "xAxisColumn";
            this.xAxisColumn.SelectedIndex = -1;
            this.xAxisColumn.SelectedItem = null;
            this.xAxisColumn.Size = new System.Drawing.Size(126, 33);
            this.xAxisColumn.State = TerraViewer.State.Rest;
            this.xAxisColumn.TabIndex = 2;
            this.xAxisColumn.Type = TerraViewer.WwtCombo.ComboType.List;
            this.xAxisColumn.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.xAxisColumn_SelectionChanged);
            // 
            // reverseXCheckbox
            // 
            this.reverseXCheckbox.Checked = false;
            this.reverseXCheckbox.Location = new System.Drawing.Point(24, 114);
            this.reverseXCheckbox.Name = "reverseXCheckbox";
            this.reverseXCheckbox.Size = new System.Drawing.Size(113, 25);
            this.reverseXCheckbox.TabIndex = 3;
            this.reverseXCheckbox.Text = "Reverse X";
            this.reverseXCheckbox.CheckedChanged += new System.EventHandler(this.reverseXCheckbox_CheckedChanged);
            // 
            // reverseYCheckbox
            // 
            this.reverseYCheckbox.Checked = false;
            this.reverseYCheckbox.Location = new System.Drawing.Point(150, 114);
            this.reverseYCheckbox.Name = "reverseYCheckbox";
            this.reverseYCheckbox.Size = new System.Drawing.Size(113, 25);
            this.reverseYCheckbox.TabIndex = 6;
            this.reverseYCheckbox.Text = "Reverse Y";
            this.reverseYCheckbox.CheckedChanged += new System.EventHandler(this.reverseYCheckbox_CheckedChanged);
            // 
            // reverseZCheckbox
            // 
            this.reverseZCheckbox.Checked = false;
            this.reverseZCheckbox.Location = new System.Drawing.Point(276, 114);
            this.reverseZCheckbox.Name = "reverseZCheckbox";
            this.reverseZCheckbox.Size = new System.Drawing.Size(113, 25);
            this.reverseZCheckbox.TabIndex = 9;
            this.reverseZCheckbox.Text = "Reverse Z";
            this.reverseZCheckbox.CheckedChanged += new System.EventHandler(this.reverseZCheckbox_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(399, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Units";
            // 
            // AltDepthUnitsCombo
            // 
            this.AltDepthUnitsCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.AltDepthUnitsCombo.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.AltDepthUnitsCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.AltDepthUnitsCombo.FilterStyle = false;
            this.AltDepthUnitsCombo.Location = new System.Drawing.Point(402, 78);
            this.AltDepthUnitsCombo.Margin = new System.Windows.Forms.Padding(0);
            this.AltDepthUnitsCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.AltDepthUnitsCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.AltDepthUnitsCombo.Name = "AltDepthUnitsCombo";
            this.AltDepthUnitsCombo.SelectedIndex = -1;
            this.AltDepthUnitsCombo.SelectedItem = null;
            this.AltDepthUnitsCombo.Size = new System.Drawing.Size(126, 33);
            this.AltDepthUnitsCombo.State = TerraViewer.State.Rest;
            this.AltDepthUnitsCombo.TabIndex = 14;
            this.AltDepthUnitsCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // DataWizardCartesian
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.AltDepthUnitsCombo);
            this.Controls.Add(this.reverseZCheckbox);
            this.Controls.Add(this.reverseYCheckbox);
            this.Controls.Add(this.reverseXCheckbox);
            this.Controls.Add(this.zAxisLabel);
            this.Controls.Add(this.yAxisLabel);
            this.Controls.Add(this.xAxisLabel);
            this.Controls.Add(this.zAxisColumn);
            this.Controls.Add(this.yAxisColumn);
            this.Controls.Add(this.xAxisColumn);
            this.Controls.Add(this.label3);
            this.Name = "DataWizardCartesian";
            this.Load += new System.EventHandler(this.DataWizardCartesian_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label zAxisLabel;
        private System.Windows.Forms.Label yAxisLabel;
        private System.Windows.Forms.Label xAxisLabel;
        private WwtCombo zAxisColumn;
        private WwtCombo yAxisColumn;
        private WwtCombo xAxisColumn;
        private WWTCheckbox reverseXCheckbox;
        private WWTCheckbox reverseYCheckbox;
        private WWTCheckbox reverseZCheckbox;
        private System.Windows.Forms.Label label2;
        private WwtCombo AltDepthUnitsCombo;
    }
}
