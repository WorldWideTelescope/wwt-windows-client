namespace TerraViewer
{
    partial class FrameWizardTrajectory
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
            this.label1 = new System.Windows.Forms.Label();
            this.UnitsLabel = new System.Windows.Forms.Label();
            this.SemiMajorAxisUnits = new TerraViewer.WwtCombo();
            this.Import = new TerraViewer.WwtButton();
            this.endDateRangeEdit = new System.Windows.Forms.TextBox();
            this.EndDateRangeLabel = new System.Windows.Forms.Label();
            this.beginDateRangeEdit = new System.Windows.Forms.TextBox();
            this.beginDateRangeLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(14, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(656, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "Trajectory Reference Frames are based on a time series table of Julian Date/Times and heliocentric X,Y,Z coordinates in the referenced units. The reference frame will orient itself on the path described based on interpolating positions for the current time.";
            // 
            // UnitsLabel
            // 
            this.UnitsLabel.AutoSize = true;
            this.UnitsLabel.Location = new System.Drawing.Point(151, 58);
            this.UnitsLabel.Name = "UnitsLabel";
            this.UnitsLabel.Size = new System.Drawing.Size(31, 13);
            this.UnitsLabel.TabIndex = 9;
            this.UnitsLabel.Text = "Units";
            // 
            // SemiMajorAxisUnits
            // 
            this.SemiMajorAxisUnits.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.SemiMajorAxisUnits.DateTimeValue = new System.DateTime(2010, 7, 6, 14, 22, 50, 802);
            this.SemiMajorAxisUnits.Filter = TerraViewer.Classification.Unfiltered;
            this.SemiMajorAxisUnits.FilterStyle = false;
            this.SemiMajorAxisUnits.Location = new System.Drawing.Point(154, 72);
            this.SemiMajorAxisUnits.Margin = new System.Windows.Forms.Padding(0);
            this.SemiMajorAxisUnits.MasterTime = true;
            this.SemiMajorAxisUnits.MaximumSize = new System.Drawing.Size(248, 33);
            this.SemiMajorAxisUnits.MinimumSize = new System.Drawing.Size(35, 33);
            this.SemiMajorAxisUnits.Name = "SemiMajorAxisUnits";
            this.SemiMajorAxisUnits.SelectedIndex = -1;
            this.SemiMajorAxisUnits.SelectedItem = null;
            this.SemiMajorAxisUnits.Size = new System.Drawing.Size(139, 33);
            this.SemiMajorAxisUnits.State = TerraViewer.State.Rest;
            this.SemiMajorAxisUnits.TabIndex = 10;
            this.SemiMajorAxisUnits.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // Import
            // 
            this.Import.BackColor = System.Drawing.Color.Transparent;
            this.Import.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Import.ImageDisabled = null;
            this.Import.ImageEnabled = null;
            this.Import.Location = new System.Drawing.Point(17, 72);
            this.Import.MaximumSize = new System.Drawing.Size(140, 33);
            this.Import.Name = "Import";
            this.Import.Selected = false;
            this.Import.Size = new System.Drawing.Size(112, 33);
            this.Import.TabIndex = 11;
            this.Import.Text = "Import Path";
            this.Import.Click += new System.EventHandler(this.Import_Click);
            // 
            // endDateRangeEdit
            // 
            this.endDateRangeEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.endDateRangeEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.endDateRangeEdit.ForeColor = System.Drawing.Color.White;
            this.endDateRangeEdit.Location = new System.Drawing.Point(154, 173);
            this.endDateRangeEdit.Name = "endDateRangeEdit";
            this.endDateRangeEdit.Size = new System.Drawing.Size(125, 20);
            this.endDateRangeEdit.TabIndex = 14;
            // 
            // EndDateRangeLabel
            // 
            this.EndDateRangeLabel.AutoSize = true;
            this.EndDateRangeLabel.Location = new System.Drawing.Point(151, 157);
            this.EndDateRangeLabel.Name = "EndDateRangeLabel";
            this.EndDateRangeLabel.Size = new System.Drawing.Size(87, 13);
            this.EndDateRangeLabel.TabIndex = 13;
            this.EndDateRangeLabel.Text = "End Date Range";
            // 
            // beginDateRangeEdit
            // 
            this.beginDateRangeEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.beginDateRangeEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.beginDateRangeEdit.ForeColor = System.Drawing.Color.White;
            this.beginDateRangeEdit.Location = new System.Drawing.Point(154, 128);
            this.beginDateRangeEdit.Name = "beginDateRangeEdit";
            this.beginDateRangeEdit.Size = new System.Drawing.Size(125, 20);
            this.beginDateRangeEdit.TabIndex = 15;
            // 
            // beginDateRangeLabel
            // 
            this.beginDateRangeLabel.AutoSize = true;
            this.beginDateRangeLabel.Location = new System.Drawing.Point(151, 112);
            this.beginDateRangeLabel.Name = "beginDateRangeLabel";
            this.beginDateRangeLabel.Size = new System.Drawing.Size(95, 13);
            this.beginDateRangeLabel.TabIndex = 12;
            this.beginDateRangeLabel.Text = "Begin Date Range";
            // 
            // FrameWizardTrajectory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.endDateRangeEdit);
            this.Controls.Add(this.EndDateRangeLabel);
            this.Controls.Add(this.beginDateRangeEdit);
            this.Controls.Add(this.beginDateRangeLabel);
            this.Controls.Add(this.Import);
            this.Controls.Add(this.UnitsLabel);
            this.Controls.Add(this.SemiMajorAxisUnits);
            this.Controls.Add(this.label1);
            this.Name = "FrameWizardTrajectory";
            this.Load += new System.EventHandler(this.FrameWizardTrajectory_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label UnitsLabel;
        private WwtCombo SemiMajorAxisUnits;
        private WwtButton Import;
        private System.Windows.Forms.TextBox endDateRangeEdit;
        private System.Windows.Forms.Label EndDateRangeLabel;
        private System.Windows.Forms.TextBox beginDateRangeEdit;
        private System.Windows.Forms.Label beginDateRangeLabel;
    }
}