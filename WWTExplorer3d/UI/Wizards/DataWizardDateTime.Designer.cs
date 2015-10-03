namespace TerraViewer
{
    partial class DataWizardDateTime
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataWizardDateTime));
            this.decayDescription = new System.Windows.Forms.Label();
            this.decaySliderLabel = new System.Windows.Forms.Label();
            this.decayTrackbar = new TerraViewer.WwtTrackBar();
            this.endDateRangeEdit = new System.Windows.Forms.TextBox();
            this.EndDateRangeLabel = new System.Windows.Forms.Label();
            this.beginDateRangeEdit = new System.Windows.Forms.TextBox();
            this.beginDateRangeLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dateColumnCombo = new TerraViewer.WwtCombo();
            this.endDateTimeColumnCombo = new TerraViewer.WwtCombo();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // decayDescription
            // 
            this.decayDescription.AutoSize = true;
            this.decayDescription.Location = new System.Drawing.Point(183, 116);
            this.decayDescription.Name = "decayDescription";
            this.decayDescription.Size = new System.Drawing.Size(0, 13);
            this.decayDescription.TabIndex = 7;
            // 
            // decaySliderLabel
            // 
            this.decaySliderLabel.AutoSize = true;
            this.decaySliderLabel.Location = new System.Drawing.Point(174, 77);
            this.decaySliderLabel.Name = "decaySliderLabel";
            this.decaySliderLabel.Size = new System.Drawing.Size(64, 13);
            this.decaySliderLabel.TabIndex = 5;
            this.decaySliderLabel.Text = "Time Decay";
            // 
            // decayTrackbar
            // 
            this.decayTrackbar.BackColor = System.Drawing.Color.Transparent;
            this.decayTrackbar.Location = new System.Drawing.Point(177, 93);
            this.decayTrackbar.Max = 100;
            this.decayTrackbar.Name = "decayTrackbar";
            this.decayTrackbar.Size = new System.Drawing.Size(84, 20);
            this.decayTrackbar.TabIndex = 6;
            this.decayTrackbar.Value = 50;
            this.decayTrackbar.ValueChanged += new System.EventHandler(this.decayTrackbar_ValueChanged);
            // 
            // endDateRangeEdit
            // 
            this.endDateRangeEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.endDateRangeEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.endDateRangeEdit.ForeColor = System.Drawing.Color.White;
            this.endDateRangeEdit.Location = new System.Drawing.Point(526, 138);
            this.endDateRangeEdit.Name = "endDateRangeEdit";
            this.endDateRangeEdit.ReadOnly = true;
            this.endDateRangeEdit.Size = new System.Drawing.Size(125, 20);
            this.endDateRangeEdit.TabIndex = 11;
            // 
            // EndDateRangeLabel
            // 
            this.EndDateRangeLabel.AutoSize = true;
            this.EndDateRangeLabel.Location = new System.Drawing.Point(523, 122);
            this.EndDateRangeLabel.Name = "EndDateRangeLabel";
            this.EndDateRangeLabel.Size = new System.Drawing.Size(87, 13);
            this.EndDateRangeLabel.TabIndex = 10;
            this.EndDateRangeLabel.Text = "End Date Range";
            // 
            // beginDateRangeEdit
            // 
            this.beginDateRangeEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.beginDateRangeEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.beginDateRangeEdit.ForeColor = System.Drawing.Color.White;
            this.beginDateRangeEdit.Location = new System.Drawing.Point(526, 93);
            this.beginDateRangeEdit.Name = "beginDateRangeEdit";
            this.beginDateRangeEdit.ReadOnly = true;
            this.beginDateRangeEdit.Size = new System.Drawing.Size(125, 20);
            this.beginDateRangeEdit.TabIndex = 9;
            // 
            // beginDateRangeLabel
            // 
            this.beginDateRangeLabel.AutoSize = true;
            this.beginDateRangeLabel.Location = new System.Drawing.Point(523, 77);
            this.beginDateRangeLabel.Name = "beginDateRangeLabel";
            this.beginDateRangeLabel.Size = new System.Drawing.Size(95, 13);
            this.beginDateRangeLabel.TabIndex = 8;
            this.beginDateRangeLabel.Text = "Begin Date Range";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Start Date/Time";
            // 
            // dateColumnCombo
            // 
            this.dateColumnCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.dateColumnCombo.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.dateColumnCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.dateColumnCombo.FilterStyle = false;
            this.dateColumnCombo.Location = new System.Drawing.Point(25, 93);
            this.dateColumnCombo.Margin = new System.Windows.Forms.Padding(0);
            this.dateColumnCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.dateColumnCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.dateColumnCombo.Name = "dateColumnCombo";
            this.dateColumnCombo.SelectedIndex = -1;
            this.dateColumnCombo.SelectedItem = null;
            this.dateColumnCombo.Size = new System.Drawing.Size(126, 33);
            this.dateColumnCombo.State = TerraViewer.State.Rest;
            this.dateColumnCombo.TabIndex = 2;
            this.dateColumnCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.dateColumnCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.dateColumnCombo_SelectionChanged);
            // 
            // endDateTimeColumnCombo
            // 
            this.endDateTimeColumnCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.endDateTimeColumnCombo.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.endDateTimeColumnCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.endDateTimeColumnCombo.FilterStyle = false;
            this.endDateTimeColumnCombo.Location = new System.Drawing.Point(25, 143);
            this.endDateTimeColumnCombo.Margin = new System.Windows.Forms.Padding(0);
            this.endDateTimeColumnCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.endDateTimeColumnCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.endDateTimeColumnCombo.Name = "endDateTimeColumnCombo";
            this.endDateTimeColumnCombo.SelectedIndex = -1;
            this.endDateTimeColumnCombo.SelectedItem = null;
            this.endDateTimeColumnCombo.Size = new System.Drawing.Size(126, 33);
            this.endDateTimeColumnCombo.State = TerraViewer.State.Rest;
            this.endDateTimeColumnCombo.TabIndex = 4;
            this.endDateTimeColumnCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.endDateTimeColumnCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.endDateTimeColumnCombo_SelectionChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 127);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "End Date/Time";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(22, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(626, 39);
            this.label3.TabIndex = 0;
            this.label3.Text = resources.GetString("label3.Text");
            // 
            // DataWizardDateTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.decayDescription);
            this.Controls.Add(this.decaySliderLabel);
            this.Controls.Add(this.decayTrackbar);
            this.Controls.Add(this.endDateRangeEdit);
            this.Controls.Add(this.EndDateRangeLabel);
            this.Controls.Add(this.beginDateRangeEdit);
            this.Controls.Add(this.beginDateRangeLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.endDateTimeColumnCombo);
            this.Controls.Add(this.dateColumnCombo);
            this.Name = "DataWizardDateTime";
            this.Load += new System.EventHandler(this.DataWizardDateTime_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label decayDescription;
        private System.Windows.Forms.Label decaySliderLabel;
        private WwtTrackBar decayTrackbar;
        private System.Windows.Forms.TextBox endDateRangeEdit;
        private System.Windows.Forms.Label EndDateRangeLabel;
        private System.Windows.Forms.TextBox beginDateRangeEdit;
        private System.Windows.Forms.Label beginDateRangeLabel;
        private System.Windows.Forms.Label label5;
        private WwtCombo dateColumnCombo;
        private WwtCombo endDateTimeColumnCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
    }
}
