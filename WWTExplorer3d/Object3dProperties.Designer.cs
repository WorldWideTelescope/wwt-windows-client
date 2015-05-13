namespace TerraViewer
{
    partial class Object3dProperties
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
            this.PitchLabel = new System.Windows.Forms.Label();
            this.Pitch = new System.Windows.Forms.TextBox();
            this.HeadingLabel = new System.Windows.Forms.Label();
            this.Heading = new System.Windows.Forms.TextBox();
            this.RollLabel = new System.Windows.Forms.Label();
            this.Roll = new System.Windows.Forms.TextBox();
            this.TranslationYLabel = new System.Windows.Forms.Label();
            this.TranslationY = new System.Windows.Forms.TextBox();
            this.TranslationZLabel = new System.Windows.Forms.Label();
            this.TranslationZ = new System.Windows.Forms.TextBox();
            this.TranslationXLabel = new System.Windows.Forms.Label();
            this.TranslationX = new System.Windows.Forms.TextBox();
            this.ScaleYLabel = new System.Windows.Forms.Label();
            this.ScaleY = new System.Windows.Forms.TextBox();
            this.ScaleZLabel = new System.Windows.Forms.Label();
            this.ScaleZ = new System.Windows.Forms.TextBox();
            this.ScaleXLabel = new System.Windows.Forms.Label();
            this.ScaleX = new System.Windows.Forms.TextBox();
            this.TwoSidedGeometry = new TerraViewer.WWTCheckbox();
            this.Smooth = new TerraViewer.WWTCheckbox();
            this.FlipV = new TerraViewer.WWTCheckbox();
            this.UniformScaling = new TerraViewer.WWTCheckbox();
            this.Close = new TerraViewer.WwtButton();
            this.rightHanded = new TerraViewer.WWTCheckbox();
            this.SuspendLayout();
            // 
            // PitchLabel
            // 
            this.PitchLabel.AutoSize = true;
            this.PitchLabel.Location = new System.Drawing.Point(266, 60);
            this.PitchLabel.Name = "PitchLabel";
            this.PitchLabel.Size = new System.Drawing.Size(31, 13);
            this.PitchLabel.TabIndex = 14;
            this.PitchLabel.Text = "Pitch";
            // 
            // Pitch
            // 
            this.Pitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Pitch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Pitch.ForeColor = System.Drawing.Color.White;
            this.Pitch.Location = new System.Drawing.Point(269, 76);
            this.Pitch.Name = "Pitch";
            this.Pitch.Size = new System.Drawing.Size(69, 20);
            this.Pitch.TabIndex = 15;
            this.Pitch.TextChanged += new System.EventHandler(this.TextValueChanged);
            // 
            // HeadingLabel
            // 
            this.HeadingLabel.AutoSize = true;
            this.HeadingLabel.Location = new System.Drawing.Point(266, 19);
            this.HeadingLabel.Name = "HeadingLabel";
            this.HeadingLabel.Size = new System.Drawing.Size(47, 13);
            this.HeadingLabel.TabIndex = 12;
            this.HeadingLabel.Text = "Heading";
            // 
            // Heading
            // 
            this.Heading.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Heading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Heading.ForeColor = System.Drawing.Color.White;
            this.Heading.Location = new System.Drawing.Point(269, 35);
            this.Heading.Name = "Heading";
            this.Heading.Size = new System.Drawing.Size(69, 20);
            this.Heading.TabIndex = 13;
            this.Heading.TextChanged += new System.EventHandler(this.TextValueChanged);
            // 
            // RollLabel
            // 
            this.RollLabel.AutoSize = true;
            this.RollLabel.Location = new System.Drawing.Point(266, 102);
            this.RollLabel.Name = "RollLabel";
            this.RollLabel.Size = new System.Drawing.Size(25, 13);
            this.RollLabel.TabIndex = 16;
            this.RollLabel.Text = "Roll";
            // 
            // Roll
            // 
            this.Roll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Roll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Roll.ForeColor = System.Drawing.Color.White;
            this.Roll.Location = new System.Drawing.Point(269, 118);
            this.Roll.Name = "Roll";
            this.Roll.Size = new System.Drawing.Size(69, 20);
            this.Roll.TabIndex = 17;
            this.Roll.TextChanged += new System.EventHandler(this.TextValueChanged);
            // 
            // TranslationYLabel
            // 
            this.TranslationYLabel.AutoSize = true;
            this.TranslationYLabel.Location = new System.Drawing.Point(9, 60);
            this.TranslationYLabel.Name = "TranslationYLabel";
            this.TranslationYLabel.Size = new System.Drawing.Size(69, 13);
            this.TranslationYLabel.TabIndex = 2;
            this.TranslationYLabel.Text = "Translation Y";
            // 
            // TranslationY
            // 
            this.TranslationY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.TranslationY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TranslationY.ForeColor = System.Drawing.Color.White;
            this.TranslationY.Location = new System.Drawing.Point(12, 76);
            this.TranslationY.Name = "TranslationY";
            this.TranslationY.Size = new System.Drawing.Size(69, 20);
            this.TranslationY.TabIndex = 3;
            this.TranslationY.TextChanged += new System.EventHandler(this.TextValueChanged);
            // 
            // TranslationZLabel
            // 
            this.TranslationZLabel.AutoSize = true;
            this.TranslationZLabel.Location = new System.Drawing.Point(9, 102);
            this.TranslationZLabel.Name = "TranslationZLabel";
            this.TranslationZLabel.Size = new System.Drawing.Size(69, 13);
            this.TranslationZLabel.TabIndex = 4;
            this.TranslationZLabel.Text = "Translation Z";
            // 
            // TranslationZ
            // 
            this.TranslationZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.TranslationZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TranslationZ.ForeColor = System.Drawing.Color.White;
            this.TranslationZ.Location = new System.Drawing.Point(12, 118);
            this.TranslationZ.Name = "TranslationZ";
            this.TranslationZ.Size = new System.Drawing.Size(69, 20);
            this.TranslationZ.TabIndex = 5;
            this.TranslationZ.TextChanged += new System.EventHandler(this.TextValueChanged);
            // 
            // TranslationXLabel
            // 
            this.TranslationXLabel.AutoSize = true;
            this.TranslationXLabel.Location = new System.Drawing.Point(9, 19);
            this.TranslationXLabel.Name = "TranslationXLabel";
            this.TranslationXLabel.Size = new System.Drawing.Size(69, 13);
            this.TranslationXLabel.TabIndex = 0;
            this.TranslationXLabel.Text = "Translation X";
            // 
            // TranslationX
            // 
            this.TranslationX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.TranslationX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TranslationX.ForeColor = System.Drawing.Color.White;
            this.TranslationX.Location = new System.Drawing.Point(12, 35);
            this.TranslationX.Name = "TranslationX";
            this.TranslationX.Size = new System.Drawing.Size(69, 20);
            this.TranslationX.TabIndex = 1;
            this.TranslationX.TextChanged += new System.EventHandler(this.TextValueChanged);
            // 
            // ScaleYLabel
            // 
            this.ScaleYLabel.AutoSize = true;
            this.ScaleYLabel.Location = new System.Drawing.Point(129, 60);
            this.ScaleYLabel.Name = "ScaleYLabel";
            this.ScaleYLabel.Size = new System.Drawing.Size(44, 13);
            this.ScaleYLabel.TabIndex = 8;
            this.ScaleYLabel.Text = "Scale Y";
            // 
            // ScaleY
            // 
            this.ScaleY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ScaleY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ScaleY.Enabled = false;
            this.ScaleY.ForeColor = System.Drawing.Color.White;
            this.ScaleY.Location = new System.Drawing.Point(132, 76);
            this.ScaleY.Name = "ScaleY";
            this.ScaleY.Size = new System.Drawing.Size(69, 20);
            this.ScaleY.TabIndex = 9;
            this.ScaleY.TextChanged += new System.EventHandler(this.TextValueChanged);
            // 
            // ScaleZLabel
            // 
            this.ScaleZLabel.AutoSize = true;
            this.ScaleZLabel.Location = new System.Drawing.Point(129, 102);
            this.ScaleZLabel.Name = "ScaleZLabel";
            this.ScaleZLabel.Size = new System.Drawing.Size(44, 13);
            this.ScaleZLabel.TabIndex = 10;
            this.ScaleZLabel.Text = "Scale Z";
            // 
            // ScaleZ
            // 
            this.ScaleZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ScaleZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ScaleZ.Enabled = false;
            this.ScaleZ.ForeColor = System.Drawing.Color.White;
            this.ScaleZ.Location = new System.Drawing.Point(132, 118);
            this.ScaleZ.Name = "ScaleZ";
            this.ScaleZ.Size = new System.Drawing.Size(69, 20);
            this.ScaleZ.TabIndex = 11;
            this.ScaleZ.TextChanged += new System.EventHandler(this.TextValueChanged);
            // 
            // ScaleXLabel
            // 
            this.ScaleXLabel.AutoSize = true;
            this.ScaleXLabel.Location = new System.Drawing.Point(129, 19);
            this.ScaleXLabel.Name = "ScaleXLabel";
            this.ScaleXLabel.Size = new System.Drawing.Size(44, 13);
            this.ScaleXLabel.TabIndex = 6;
            this.ScaleXLabel.Text = "Scale X";
            // 
            // ScaleX
            // 
            this.ScaleX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ScaleX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ScaleX.ForeColor = System.Drawing.Color.White;
            this.ScaleX.Location = new System.Drawing.Point(132, 35);
            this.ScaleX.Name = "ScaleX";
            this.ScaleX.Size = new System.Drawing.Size(69, 20);
            this.ScaleX.TabIndex = 7;
            this.ScaleX.TextChanged += new System.EventHandler(this.ScaleX_TextChanged);
            // 
            // TwoSidedGeometry
            // 
            this.TwoSidedGeometry.BackColor = System.Drawing.Color.Transparent;
            this.TwoSidedGeometry.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.TwoSidedGeometry.Checked = false;
            this.TwoSidedGeometry.Location = new System.Drawing.Point(127, 165);
            this.TwoSidedGeometry.Name = "TwoSidedGeometry";
            this.TwoSidedGeometry.Size = new System.Drawing.Size(110, 25);
            this.TwoSidedGeometry.TabIndex = 21;
            this.TwoSidedGeometry.Text = "Two-sided";
            this.TwoSidedGeometry.CheckedChanged += new System.EventHandler(this.TwoSidedGeometry_CheckedChanged);
            // 
            // Smooth
            // 
            this.Smooth.BackColor = System.Drawing.Color.Transparent;
            this.Smooth.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Smooth.Checked = false;
            this.Smooth.Location = new System.Drawing.Point(7, 185);
            this.Smooth.Name = "Smooth";
            this.Smooth.Size = new System.Drawing.Size(127, 25);
            this.Smooth.TabIndex = 22;
            this.Smooth.Text = "Smooth Normals";
            this.Smooth.CheckedChanged += new System.EventHandler(this.Smooth_CheckedChanged);
            // 
            // FlipV
            // 
            this.FlipV.BackColor = System.Drawing.Color.Transparent;
            this.FlipV.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.FlipV.Checked = false;
            this.FlipV.Location = new System.Drawing.Point(7, 144);
            this.FlipV.Name = "FlipV";
            this.FlipV.Size = new System.Drawing.Size(99, 25);
            this.FlipV.TabIndex = 18;
            this.FlipV.Text = "Flip Textures";
            this.FlipV.CheckedChanged += new System.EventHandler(this.FlipV_CheckedChanged);
            // 
            // UniformScaling
            // 
            this.UniformScaling.BackColor = System.Drawing.Color.Transparent;
            this.UniformScaling.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.UniformScaling.Checked = true;
            this.UniformScaling.Location = new System.Drawing.Point(127, 144);
            this.UniformScaling.Name = "UniformScaling";
            this.UniformScaling.Size = new System.Drawing.Size(110, 25);
            this.UniformScaling.TabIndex = 19;
            this.UniformScaling.Text = "Uniform Scaling";
            this.UniformScaling.CheckedChanged += new System.EventHandler(this.UniformScaling_CheckedChanged);
            // 
            // Close
            // 
            this.Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Close.BackColor = System.Drawing.Color.Transparent;
            this.Close.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Close.ImageDisabled = null;
            this.Close.ImageEnabled = null;
            this.Close.Location = new System.Drawing.Point(261, 167);
            this.Close.MaximumSize = new System.Drawing.Size(140, 33);
            this.Close.Name = "Close";
            this.Close.Selected = false;
            this.Close.Size = new System.Drawing.Size(94, 33);
            this.Close.TabIndex = 23;
            this.Close.Text = "Close";
            this.Close.Click += new System.EventHandler(this.Close_Click);
            // 
            // rightHanded
            // 
            this.rightHanded.BackColor = System.Drawing.Color.Transparent;
            this.rightHanded.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rightHanded.Checked = false;
            this.rightHanded.Location = new System.Drawing.Point(7, 165);
            this.rightHanded.Name = "rightHanded";
            this.rightHanded.Size = new System.Drawing.Size(114, 25);
            this.rightHanded.TabIndex = 20;
            this.rightHanded.Text = "Right Handed";
            this.rightHanded.CheckedChanged += new System.EventHandler(this.rightHanded_CheckedChanged);
            // 
            // Object3dProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(367, 212);
            this.Controls.Add(this.rightHanded);
            this.Controls.Add(this.TwoSidedGeometry);
            this.Controls.Add(this.Smooth);
            this.Controls.Add(this.FlipV);
            this.Controls.Add(this.UniformScaling);
            this.Controls.Add(this.Close);
            this.Controls.Add(this.ScaleX);
            this.Controls.Add(this.ScaleXLabel);
            this.Controls.Add(this.TranslationX);
            this.Controls.Add(this.TranslationXLabel);
            this.Controls.Add(this.ScaleZ);
            this.Controls.Add(this.Heading);
            this.Controls.Add(this.TranslationZ);
            this.Controls.Add(this.ScaleZLabel);
            this.Controls.Add(this.HeadingLabel);
            this.Controls.Add(this.TranslationZLabel);
            this.Controls.Add(this.ScaleY);
            this.Controls.Add(this.Roll);
            this.Controls.Add(this.TranslationY);
            this.Controls.Add(this.ScaleYLabel);
            this.Controls.Add(this.RollLabel);
            this.Controls.Add(this.TranslationYLabel);
            this.Controls.Add(this.Pitch);
            this.Controls.Add(this.PitchLabel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Object3dProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "3d Model Properties";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Object3dProperties_FormClosed);
            this.Load += new System.EventHandler(this.Object3dProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label PitchLabel;
        private System.Windows.Forms.TextBox Pitch;
        private System.Windows.Forms.Label HeadingLabel;
        private System.Windows.Forms.TextBox Heading;
        private System.Windows.Forms.Label RollLabel;
        private System.Windows.Forms.TextBox Roll;
        private System.Windows.Forms.Label TranslationYLabel;
        private System.Windows.Forms.TextBox TranslationY;
        private System.Windows.Forms.Label TranslationZLabel;
        private System.Windows.Forms.TextBox TranslationZ;
        private System.Windows.Forms.Label TranslationXLabel;
        private System.Windows.Forms.TextBox TranslationX;
        private WwtButton Close;
        private System.Windows.Forms.Label ScaleYLabel;
        private System.Windows.Forms.TextBox ScaleY;
        private System.Windows.Forms.Label ScaleZLabel;
        private System.Windows.Forms.TextBox ScaleZ;
        private System.Windows.Forms.Label ScaleXLabel;
        private System.Windows.Forms.TextBox ScaleX;
        private WWTCheckbox UniformScaling;
        private WWTCheckbox FlipV;
        private WWTCheckbox Smooth;
        private WWTCheckbox TwoSidedGeometry;
        private WWTCheckbox rightHanded;
    }
}