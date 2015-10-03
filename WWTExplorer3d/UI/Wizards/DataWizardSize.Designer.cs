namespace TerraViewer
{
    partial class DataWizardSize
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataWizardSize));
            this.scaleText = new System.Windows.Forms.Label();
            this.scaleFactorSlider = new System.Windows.Forms.Label();
            this.scaleFactorTrackbar = new TerraViewer.WwtTrackBar();
            this.scaleTypeLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.scaleTypeCombo = new TerraViewer.WwtCombo();
            this.sizeColumnCombo = new TerraViewer.WwtCombo();
            this.label2 = new System.Windows.Forms.Label();
            this.scaleRelative = new TerraViewer.WwtCombo();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // scaleText
            // 
            this.scaleText.AutoSize = true;
            this.scaleText.Location = new System.Drawing.Point(526, 144);
            this.scaleText.Name = "scaleText";
            this.scaleText.Size = new System.Drawing.Size(22, 13);
            this.scaleText.TabIndex = 9;
            this.scaleText.Text = "1.0";
            // 
            // scaleFactorSlider
            // 
            this.scaleFactorSlider.AutoSize = true;
            this.scaleFactorSlider.Location = new System.Drawing.Point(500, 97);
            this.scaleFactorSlider.Name = "scaleFactorSlider";
            this.scaleFactorSlider.Size = new System.Drawing.Size(67, 13);
            this.scaleFactorSlider.TabIndex = 7;
            this.scaleFactorSlider.Text = "Scale Factor";
            // 
            // scaleFactorTrackbar
            // 
            this.scaleFactorTrackbar.BackColor = System.Drawing.Color.Transparent;
            this.scaleFactorTrackbar.Location = new System.Drawing.Point(503, 113);
            this.scaleFactorTrackbar.Max = 100;
            this.scaleFactorTrackbar.Name = "scaleFactorTrackbar";
            this.scaleFactorTrackbar.Size = new System.Drawing.Size(84, 20);
            this.scaleFactorTrackbar.TabIndex = 8;
            this.scaleFactorTrackbar.Value = 50;
            this.scaleFactorTrackbar.ValueChanged += new System.EventHandler(this.scaleFactorTrackbar_ValueChanged);
            // 
            // scaleTypeLabel
            // 
            this.scaleTypeLabel.AutoSize = true;
            this.scaleTypeLabel.Location = new System.Drawing.Point(26, 97);
            this.scaleTypeLabel.Name = "scaleTypeLabel";
            this.scaleTypeLabel.Size = new System.Drawing.Size(61, 13);
            this.scaleTypeLabel.TabIndex = 1;
            this.scaleTypeLabel.Text = "Scale Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(310, 97);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(120, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Size/Magnitude Column";
            // 
            // scaleTypeCombo
            // 
            this.scaleTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.scaleTypeCombo.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.scaleTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.scaleTypeCombo.FilterStyle = false;
            this.scaleTypeCombo.Location = new System.Drawing.Point(29, 113);
            this.scaleTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.scaleTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.scaleTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.scaleTypeCombo.Name = "scaleTypeCombo";
            this.scaleTypeCombo.SelectedIndex = -1;
            this.scaleTypeCombo.SelectedItem = null;
            this.scaleTypeCombo.Size = new System.Drawing.Size(126, 33);
            this.scaleTypeCombo.State = TerraViewer.State.Rest;
            this.scaleTypeCombo.TabIndex = 2;
            this.scaleTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.scaleTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.scaleTypeCombo_SelectionChanged);
            // 
            // sizeColumnCombo
            // 
            this.sizeColumnCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.sizeColumnCombo.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.sizeColumnCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.sizeColumnCombo.FilterStyle = false;
            this.sizeColumnCombo.Location = new System.Drawing.Point(313, 113);
            this.sizeColumnCombo.Margin = new System.Windows.Forms.Padding(0);
            this.sizeColumnCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.sizeColumnCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.sizeColumnCombo.Name = "sizeColumnCombo";
            this.sizeColumnCombo.SelectedIndex = -1;
            this.sizeColumnCombo.SelectedItem = null;
            this.sizeColumnCombo.Size = new System.Drawing.Size(126, 33);
            this.sizeColumnCombo.State = TerraViewer.State.Rest;
            this.sizeColumnCombo.TabIndex = 6;
            this.sizeColumnCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.sizeColumnCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.sizeColumnCombo_SelectionChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(166, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Scale Relative";
            // 
            // scaleRelative
            // 
            this.scaleRelative.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.scaleRelative.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.scaleRelative.Filter = TerraViewer.Classification.Unfiltered;
            this.scaleRelative.FilterStyle = false;
            this.scaleRelative.Location = new System.Drawing.Point(169, 113);
            this.scaleRelative.Margin = new System.Windows.Forms.Padding(0);
            this.scaleRelative.MaximumSize = new System.Drawing.Size(248, 33);
            this.scaleRelative.MinimumSize = new System.Drawing.Size(35, 33);
            this.scaleRelative.Name = "scaleRelative";
            this.scaleRelative.SelectedIndex = -1;
            this.scaleRelative.SelectedItem = null;
            this.scaleRelative.Size = new System.Drawing.Size(126, 33);
            this.scaleRelative.State = TerraViewer.State.Rest;
            this.scaleRelative.TabIndex = 4;
            this.scaleRelative.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(26, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(642, 51);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // DataWizardSize
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.scaleRelative);
            this.Controls.Add(this.scaleText);
            this.Controls.Add(this.scaleFactorSlider);
            this.Controls.Add(this.scaleFactorTrackbar);
            this.Controls.Add(this.scaleTypeLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.scaleTypeCombo);
            this.Controls.Add(this.sizeColumnCombo);
            this.Name = "DataWizardSize";
            this.Load += new System.EventHandler(this.DataWizardSize_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label scaleText;
        private System.Windows.Forms.Label scaleFactorSlider;
        private WwtTrackBar scaleFactorTrackbar;
        private System.Windows.Forms.Label scaleTypeLabel;
        private System.Windows.Forms.Label label4;
        private WwtCombo scaleTypeCombo;
        private WwtCombo sizeColumnCombo;
        private System.Windows.Forms.Label label2;
        private WwtCombo scaleRelative;
        private System.Windows.Forms.Label label1;
    }
}
