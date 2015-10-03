namespace TerraViewer
{
    partial class FrameWizardMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrameWizardMain));
            this.Heading = new System.Windows.Forms.TextBox();
            this.HeadingLabel = new System.Windows.Forms.Label();
            this.PitchLabel = new System.Windows.Forms.Label();
            this.Pitch = new System.Windows.Forms.TextBox();
            this.RollLabel = new System.Windows.Forms.Label();
            this.Roll = new System.Windows.Forms.TextBox();
            this.RotationPeriodLabel = new System.Windows.Forms.Label();
            this.RotaionPeriod = new System.Windows.Forms.TextBox();
            this.ZeroRotationLabel = new System.Windows.Forms.Label();
            this.ZeroRotation = new System.Windows.Forms.TextBox();
            this.meanRadiusLabel = new System.Windows.Forms.Label();
            this.MeanRadius = new System.Windows.Forms.TextBox();
            this.OblatenessLabel = new System.Windows.Forms.Label();
            this.Oblateness = new System.Windows.Forms.TextBox();
            this.RepresentativeColor = new System.Windows.Forms.PictureBox();
            this.OrbitColorLabel = new System.Windows.Forms.Label();
            this.ShowAsPoint = new TerraViewer.WWTCheckbox();
            this.ShowOrbitPath = new TerraViewer.WWTCheckbox();
            this.ScaleLabel = new System.Windows.Forms.Label();
            this.Scale = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.StationKeeping = new TerraViewer.WWTCheckbox();
            ((System.ComponentModel.ISupportInitialize)(this.RepresentativeColor)).BeginInit();
            this.SuspendLayout();
            // 
            // Heading
            // 
            this.Heading.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Heading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Heading.ForeColor = System.Drawing.Color.White;
            this.Heading.Location = new System.Drawing.Point(353, 106);
            this.Heading.Name = "Heading";
            this.Heading.Size = new System.Drawing.Size(68, 20);
            this.Heading.TabIndex = 11;
            // 
            // HeadingLabel
            // 
            this.HeadingLabel.AutoSize = true;
            this.HeadingLabel.Location = new System.Drawing.Point(350, 90);
            this.HeadingLabel.Name = "HeadingLabel";
            this.HeadingLabel.Size = new System.Drawing.Size(47, 13);
            this.HeadingLabel.TabIndex = 10;
            this.HeadingLabel.Text = "Heading";
            // 
            // PitchLabel
            // 
            this.PitchLabel.AutoSize = true;
            this.PitchLabel.Location = new System.Drawing.Point(350, 133);
            this.PitchLabel.Name = "PitchLabel";
            this.PitchLabel.Size = new System.Drawing.Size(31, 13);
            this.PitchLabel.TabIndex = 12;
            this.PitchLabel.Text = "Pitch";
            // 
            // Pitch
            // 
            this.Pitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Pitch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Pitch.ForeColor = System.Drawing.Color.White;
            this.Pitch.Location = new System.Drawing.Point(353, 149);
            this.Pitch.Name = "Pitch";
            this.Pitch.Size = new System.Drawing.Size(68, 20);
            this.Pitch.TabIndex = 13;
            // 
            // RollLabel
            // 
            this.RollLabel.AutoSize = true;
            this.RollLabel.Location = new System.Drawing.Point(350, 172);
            this.RollLabel.Name = "RollLabel";
            this.RollLabel.Size = new System.Drawing.Size(25, 13);
            this.RollLabel.TabIndex = 14;
            this.RollLabel.Text = "Roll";
            // 
            // Roll
            // 
            this.Roll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Roll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Roll.ForeColor = System.Drawing.Color.White;
            this.Roll.Location = new System.Drawing.Point(353, 188);
            this.Roll.Name = "Roll";
            this.Roll.Size = new System.Drawing.Size(68, 20);
            this.Roll.TabIndex = 15;
            this.Roll.TextChanged += new System.EventHandler(this.Roll_TextChanged);
            // 
            // RotationPeriodLabel
            // 
            this.RotationPeriodLabel.AutoSize = true;
            this.RotationPeriodLabel.Location = new System.Drawing.Point(187, 90);
            this.RotationPeriodLabel.Name = "RotationPeriodLabel";
            this.RotationPeriodLabel.Size = new System.Drawing.Size(111, 13);
            this.RotationPeriodLabel.TabIndex = 6;
            this.RotationPeriodLabel.Text = "Rotation Period (days)";
            // 
            // RotaionPeriod
            // 
            this.RotaionPeriod.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.RotaionPeriod.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RotaionPeriod.ForeColor = System.Drawing.Color.White;
            this.RotaionPeriod.Location = new System.Drawing.Point(190, 106);
            this.RotaionPeriod.Name = "RotaionPeriod";
            this.RotaionPeriod.Size = new System.Drawing.Size(121, 20);
            this.RotaionPeriod.TabIndex = 7;
            // 
            // ZeroRotationLabel
            // 
            this.ZeroRotationLabel.AutoSize = true;
            this.ZeroRotationLabel.Location = new System.Drawing.Point(187, 133);
            this.ZeroRotationLabel.Name = "ZeroRotationLabel";
            this.ZeroRotationLabel.Size = new System.Drawing.Size(130, 13);
            this.ZeroRotationLabel.TabIndex = 8;
            this.ZeroRotationLabel.Text = "Zero Rotation (Julian Day)";
            // 
            // ZeroRotation
            // 
            this.ZeroRotation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ZeroRotation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ZeroRotation.ForeColor = System.Drawing.Color.White;
            this.ZeroRotation.Location = new System.Drawing.Point(190, 149);
            this.ZeroRotation.Name = "ZeroRotation";
            this.ZeroRotation.Size = new System.Drawing.Size(121, 20);
            this.ZeroRotation.TabIndex = 9;
            // 
            // meanRadiusLabel
            // 
            this.meanRadiusLabel.AutoSize = true;
            this.meanRadiusLabel.Location = new System.Drawing.Point(23, 90);
            this.meanRadiusLabel.Name = "meanRadiusLabel";
            this.meanRadiusLabel.Size = new System.Drawing.Size(110, 13);
            this.meanRadiusLabel.TabIndex = 0;
            this.meanRadiusLabel.Text = "Mean Radius (meters)";
            // 
            // MeanRadius
            // 
            this.MeanRadius.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.MeanRadius.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MeanRadius.ForeColor = System.Drawing.Color.White;
            this.MeanRadius.Location = new System.Drawing.Point(26, 106);
            this.MeanRadius.Name = "MeanRadius";
            this.MeanRadius.Size = new System.Drawing.Size(121, 20);
            this.MeanRadius.TabIndex = 1;
            // 
            // OblatenessLabel
            // 
            this.OblatenessLabel.AutoSize = true;
            this.OblatenessLabel.Location = new System.Drawing.Point(23, 128);
            this.OblatenessLabel.Name = "OblatenessLabel";
            this.OblatenessLabel.Size = new System.Drawing.Size(106, 13);
            this.OblatenessLabel.TabIndex = 2;
            this.OblatenessLabel.Text = "Oblateness (Percent)";
            // 
            // Oblateness
            // 
            this.Oblateness.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Oblateness.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Oblateness.ForeColor = System.Drawing.Color.White;
            this.Oblateness.Location = new System.Drawing.Point(26, 144);
            this.Oblateness.Name = "Oblateness";
            this.Oblateness.Size = new System.Drawing.Size(121, 20);
            this.Oblateness.TabIndex = 3;
            // 
            // RepresentativeColor
            // 
            this.RepresentativeColor.BackColor = System.Drawing.Color.Black;
            this.RepresentativeColor.Location = new System.Drawing.Point(459, 110);
            this.RepresentativeColor.Name = "RepresentativeColor";
            this.RepresentativeColor.Size = new System.Drawing.Size(82, 20);
            this.RepresentativeColor.TabIndex = 30;
            this.RepresentativeColor.TabStop = false;
            this.RepresentativeColor.Click += new System.EventHandler(this.RepresentativeColor_Click);
            this.RepresentativeColor.Paint += new System.Windows.Forms.PaintEventHandler(this.RepresentativeColor_Paint);
            // 
            // OrbitColorLabel
            // 
            this.OrbitColorLabel.AutoSize = true;
            this.OrbitColorLabel.Location = new System.Drawing.Point(456, 94);
            this.OrbitColorLabel.Name = "OrbitColorLabel";
            this.OrbitColorLabel.Size = new System.Drawing.Size(85, 13);
            this.OrbitColorLabel.TabIndex = 16;
            this.OrbitColorLabel.Text = "Orbit/Point Color";
            // 
            // ShowAsPoint
            // 
            this.ShowAsPoint.Checked = false;
            this.ShowAsPoint.Location = new System.Drawing.Point(459, 136);
            this.ShowAsPoint.Name = "ShowAsPoint";
            this.ShowAsPoint.Size = new System.Drawing.Size(190, 25);
            this.ShowAsPoint.TabIndex = 17;
            this.ShowAsPoint.Text = "Show as Point at Distance";
            this.ShowAsPoint.CheckedChanged += new System.EventHandler(this.showAsPoint_CheckedChanged);
            // 
            // ShowOrbitPath
            // 
            this.ShowOrbitPath.Checked = false;
            this.ShowOrbitPath.Location = new System.Drawing.Point(459, 167);
            this.ShowOrbitPath.Name = "ShowOrbitPath";
            this.ShowOrbitPath.Size = new System.Drawing.Size(190, 25);
            this.ShowOrbitPath.TabIndex = 18;
            this.ShowOrbitPath.Text = "Show Orbit Path";
            this.ShowOrbitPath.CheckedChanged += new System.EventHandler(this.ShowOrbitPath_CheckedChanged);
            // 
            // ScaleLabel
            // 
            this.ScaleLabel.AutoSize = true;
            this.ScaleLabel.Location = new System.Drawing.Point(23, 167);
            this.ScaleLabel.Name = "ScaleLabel";
            this.ScaleLabel.Size = new System.Drawing.Size(114, 13);
            this.ScaleLabel.TabIndex = 4;
            this.ScaleLabel.Text = "Scale (Meters to Units)";
            // 
            // Scale
            // 
            this.Scale.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Scale.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Scale.ForeColor = System.Drawing.Color.White;
            this.Scale.Location = new System.Drawing.Point(26, 183);
            this.Scale.Name = "Scale";
            this.Scale.Size = new System.Drawing.Size(121, 20);
            this.Scale.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(23, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(645, 56);
            this.label2.TabIndex = 31;
            this.label2.Text = resources.GetString("label2.Text");
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // StationKeeping
            // 
            this.StationKeeping.Checked = false;
            this.StationKeeping.Location = new System.Drawing.Point(190, 183);
            this.StationKeeping.Name = "StationKeeping";
            this.StationKeeping.Size = new System.Drawing.Size(140, 25);
            this.StationKeeping.TabIndex = 32;
            this.StationKeeping.Text = "Station Keeping";
            this.StationKeeping.CheckedChanged += new System.EventHandler(this.StationKeeping_CheckedChanged);
            // 
            // FrameWizardMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.StationKeeping);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ShowOrbitPath);
            this.Controls.Add(this.ShowAsPoint);
            this.Controls.Add(this.RepresentativeColor);
            this.Controls.Add(this.Oblateness);
            this.Controls.Add(this.MeanRadius);
            this.Controls.Add(this.OblatenessLabel);
            this.Controls.Add(this.Roll);
            this.Controls.Add(this.meanRadiusLabel);
            this.Controls.Add(this.RollLabel);
            this.Controls.Add(this.ZeroRotation);
            this.Controls.Add(this.ZeroRotationLabel);
            this.Controls.Add(this.Pitch);
            this.Controls.Add(this.RotaionPeriod);
            this.Controls.Add(this.PitchLabel);
            this.Controls.Add(this.RotationPeriodLabel);
            this.Controls.Add(this.Scale);
            this.Controls.Add(this.Heading);
            this.Controls.Add(this.ScaleLabel);
            this.Controls.Add(this.OrbitColorLabel);
            this.Controls.Add(this.HeadingLabel);
            this.Name = "FrameWizardMain";
            this.Load += new System.EventHandler(this.FrameWizardMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.RepresentativeColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Heading;
        private System.Windows.Forms.Label HeadingLabel;
        private System.Windows.Forms.Label PitchLabel;
        private System.Windows.Forms.TextBox Pitch;
        private System.Windows.Forms.Label RollLabel;
        private System.Windows.Forms.TextBox Roll;
        private System.Windows.Forms.Label RotationPeriodLabel;
        private System.Windows.Forms.TextBox RotaionPeriod;
        private System.Windows.Forms.Label ZeroRotationLabel;
        private System.Windows.Forms.TextBox ZeroRotation;
        private System.Windows.Forms.Label meanRadiusLabel;
        private System.Windows.Forms.TextBox MeanRadius;
        private System.Windows.Forms.Label OblatenessLabel;
        private System.Windows.Forms.TextBox Oblateness;
        private System.Windows.Forms.PictureBox RepresentativeColor;
        private System.Windows.Forms.Label OrbitColorLabel;
        private WWTCheckbox ShowAsPoint;
        private WWTCheckbox ShowOrbitPath;
        private System.Windows.Forms.Label ScaleLabel;
        private System.Windows.Forms.TextBox Scale;
        private System.Windows.Forms.Label label2;
        private WWTCheckbox StationKeeping;
    }
}
