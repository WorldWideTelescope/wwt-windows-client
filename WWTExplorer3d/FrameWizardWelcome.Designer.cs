namespace TerraViewer
{
    partial class FrameWizardWelcome
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
            this.label2 = new System.Windows.Forms.Label();
            this.OffsetType = new TerraViewer.WwtCombo();
            this.OffsetTypeLabel = new System.Windows.Forms.Label();
            this.ReferenceFrameName = new System.Windows.Forms.TextBox();
            this.ReferenceFrameNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(19, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(645, 50);
            this.label2.TabIndex = 11;
            this.label2.Text = "This wizard will guide you through the process of creating a new reference frame. A reference frame allows you to have local coordinates, scale and time realztive to the rest of the universe. The referernce can be based on fixed offsets, spherical coordinates, Keplarian orbits or a interpolated values from a list of date/time and position offsets.";
            // 
            // OffsetType
            // 
            this.OffsetType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.OffsetType.DateTimeValue = new System.DateTime(2010, 5, 14, 16, 0, 4, 793);
            this.OffsetType.Filter = TerraViewer.Classification.Unfiltered;
            this.OffsetType.FilterStyle = false;
            this.OffsetType.Location = new System.Drawing.Point(183, 110);
            this.OffsetType.Margin = new System.Windows.Forms.Padding(0);
            this.OffsetType.MasterTime = true;
            this.OffsetType.MaximumSize = new System.Drawing.Size(248, 33);
            this.OffsetType.MinimumSize = new System.Drawing.Size(35, 33);
            this.OffsetType.Name = "OffsetType";
            this.OffsetType.SelectedIndex = -1;
            this.OffsetType.SelectedItem = null;
            this.OffsetType.Size = new System.Drawing.Size(163, 33);
            this.OffsetType.State = TerraViewer.State.Rest;
            this.OffsetType.TabIndex = 10;
            this.OffsetType.Type = TerraViewer.WwtCombo.ComboType.List;
            this.OffsetType.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.OffsetType_SelectionChanged);
            // 
            // OffsetTypeLabel
            // 
            this.OffsetTypeLabel.AutoSize = true;
            this.OffsetTypeLabel.Location = new System.Drawing.Point(180, 98);
            this.OffsetTypeLabel.Name = "OffsetTypeLabel";
            this.OffsetTypeLabel.Size = new System.Drawing.Size(62, 13);
            this.OffsetTypeLabel.TabIndex = 8;
            this.OffsetTypeLabel.Text = "Offset Type";
            // 
            // ReferenceFrameName
            // 
            this.ReferenceFrameName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ReferenceFrameName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ReferenceFrameName.ForeColor = System.Drawing.Color.White;
            this.ReferenceFrameName.Location = new System.Drawing.Point(22, 117);
            this.ReferenceFrameName.Name = "ReferenceFrameName";
            this.ReferenceFrameName.Size = new System.Drawing.Size(139, 20);
            this.ReferenceFrameName.TabIndex = 9;
            this.ReferenceFrameName.TextChanged += new System.EventHandler(this.ReferenceFrameName_TextChanged);
            // 
            // ReferenceFrameNameLabel
            // 
            this.ReferenceFrameNameLabel.AutoSize = true;
            this.ReferenceFrameNameLabel.Location = new System.Drawing.Point(19, 98);
            this.ReferenceFrameNameLabel.Name = "ReferenceFrameNameLabel";
            this.ReferenceFrameNameLabel.Size = new System.Drawing.Size(123, 13);
            this.ReferenceFrameNameLabel.TabIndex = 7;
            this.ReferenceFrameNameLabel.Text = "Refrerence Frame Name";
            // 
            // FrameWizardWelcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OffsetType);
            this.Controls.Add(this.OffsetTypeLabel);
            this.Controls.Add(this.ReferenceFrameName);
            this.Controls.Add(this.ReferenceFrameNameLabel);
            this.Name = "FrameWizardWelcome";
            this.Load += new System.EventHandler(this.FrameWizardWelcome_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private WwtCombo OffsetType;
        private System.Windows.Forms.Label OffsetTypeLabel;
        private System.Windows.Forms.TextBox ReferenceFrameName;
        private System.Windows.Forms.Label ReferenceFrameNameLabel;
    }
}
