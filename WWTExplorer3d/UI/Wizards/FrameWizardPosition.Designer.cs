namespace TerraViewer
{
    partial class FrameWizardPosition
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
            this.Altitude = new System.Windows.Forms.TextBox();
            this.Longitude = new System.Windows.Forms.TextBox();
            this.Lattitude = new System.Windows.Forms.TextBox();
            this.altDepthLabel = new System.Windows.Forms.Label();
            this.LongitudeLabel = new System.Windows.Forms.Label();
            this.LatitudeLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.GetFromView = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // Altitude
            // 
            this.Altitude.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Altitude.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Altitude.ForeColor = System.Drawing.Color.White;
            this.Altitude.Location = new System.Drawing.Point(195, 101);
            this.Altitude.Name = "Altitude";
            this.Altitude.Size = new System.Drawing.Size(139, 20);
            this.Altitude.TabIndex = 25;
            // 
            // Longitude
            // 
            this.Longitude.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Longitude.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Longitude.ForeColor = System.Drawing.Color.White;
            this.Longitude.Location = new System.Drawing.Point(22, 157);
            this.Longitude.Name = "Longitude";
            this.Longitude.Size = new System.Drawing.Size(139, 20);
            this.Longitude.TabIndex = 26;
            // 
            // Lattitude
            // 
            this.Lattitude.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Lattitude.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Lattitude.ForeColor = System.Drawing.Color.White;
            this.Lattitude.Location = new System.Drawing.Point(22, 101);
            this.Lattitude.Name = "Lattitude";
            this.Lattitude.Size = new System.Drawing.Size(139, 20);
            this.Lattitude.TabIndex = 27;
            // 
            // altDepthLabel
            // 
            this.altDepthLabel.AutoSize = true;
            this.altDepthLabel.Location = new System.Drawing.Point(192, 85);
            this.altDepthLabel.Name = "altDepthLabel";
            this.altDepthLabel.Size = new System.Drawing.Size(83, 13);
            this.altDepthLabel.TabIndex = 23;
            this.altDepthLabel.Text = "Altitude (Meters)";
            // 
            // LongitudeLabel
            // 
            this.LongitudeLabel.AutoSize = true;
            this.LongitudeLabel.Location = new System.Drawing.Point(19, 141);
            this.LongitudeLabel.Name = "LongitudeLabel";
            this.LongitudeLabel.Size = new System.Drawing.Size(144, 13);
            this.LongitudeLabel.TabIndex = 24;
            this.LongitudeLabel.Text = "Longitude (Decimal Degrees)";
            // 
            // LatitudeLabel
            // 
            this.LatitudeLabel.AutoSize = true;
            this.LatitudeLabel.Location = new System.Drawing.Point(19, 85);
            this.LatitudeLabel.Name = "LatitudeLabel";
            this.LatitudeLabel.Size = new System.Drawing.Size(135, 13);
            this.LatitudeLabel.TabIndex = 22;
            this.LatitudeLabel.Text = "Latitude (Decimal Degrees)";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(19, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(604, 47);
            this.label3.TabIndex = 21;
            this.label3.Text = "Select the Latitude, Longitude and Altitude";
            // 
            // GetFromView
            // 
            this.GetFromView.BackColor = System.Drawing.Color.Transparent;
            this.GetFromView.DialogResult = System.Windows.Forms.DialogResult.None;
            this.GetFromView.ImageDisabled = null;
            this.GetFromView.ImageEnabled = null;
            this.GetFromView.Location = new System.Drawing.Point(195, 144);
            this.GetFromView.MaximumSize = new System.Drawing.Size(140, 33);
            this.GetFromView.Name = "GetFromView";
            this.GetFromView.Selected = false;
            this.GetFromView.Size = new System.Drawing.Size(140, 33);
            this.GetFromView.TabIndex = 28;
            this.GetFromView.Text = "Get From View";
            this.GetFromView.Click += new System.EventHandler(this.GetFromView_Click);
            // 
            // FrameWizardPosition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.GetFromView);
            this.Controls.Add(this.Altitude);
            this.Controls.Add(this.Longitude);
            this.Controls.Add(this.Lattitude);
            this.Controls.Add(this.altDepthLabel);
            this.Controls.Add(this.LongitudeLabel);
            this.Controls.Add(this.LatitudeLabel);
            this.Controls.Add(this.label3);
            this.Name = "FrameWizardPosition";
            this.Load += new System.EventHandler(this.FrameWizardPosition_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Altitude;
        private System.Windows.Forms.TextBox Longitude;
        private System.Windows.Forms.TextBox Lattitude;
        private System.Windows.Forms.Label altDepthLabel;
        private System.Windows.Forms.Label LongitudeLabel;
        private System.Windows.Forms.Label LatitudeLabel;
        private System.Windows.Forms.Label label3;
        private WwtButton GetFromView;
    }
}
