namespace TerraViewer
{
    partial class AudioTrack
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
            this.punchIn = new System.Windows.Forms.TextBox();
            this.label43 = new System.Windows.Forms.Label();
            this.punchOut = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Volume = new TerraViewer.WwtTrackBar();
            this.MuteButton = new TerraViewer.WwtButton();
            this.Browse = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // punchIn
            // 
            this.punchIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(60)))));
            this.punchIn.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.punchIn.Enabled = false;
            this.punchIn.ForeColor = System.Drawing.Color.White;
            this.punchIn.Location = new System.Drawing.Point(217, 8);
            this.punchIn.Name = "punchIn";
            this.punchIn.Size = new System.Drawing.Size(33, 13);
            this.punchIn.TabIndex = 3;
            this.punchIn.Text = "00:00";
            this.punchIn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.punchIn.Visible = false;
            this.punchIn.DoubleClick += new System.EventHandler(this.punchIn_DoubleClick);
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.ForeColor = System.Drawing.Color.White;
            this.label43.Location = new System.Drawing.Point(190, 7);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(19, 13);
            this.label43.TabIndex = 1;
            this.label43.Text = "In:";
            this.label43.Visible = false;
            // 
            // punchOut
            // 
            this.punchOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(35)))), ((int)(((byte)(60)))));
            this.punchOut.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.punchOut.Enabled = false;
            this.punchOut.ForeColor = System.Drawing.Color.White;
            this.punchOut.Location = new System.Drawing.Point(217, 28);
            this.punchOut.Name = "punchOut";
            this.punchOut.Size = new System.Drawing.Size(33, 13);
            this.punchOut.TabIndex = 3;
            this.punchOut.Text = "End";
            this.punchOut.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.punchOut.Visible = false;
            this.punchOut.DoubleClick += new System.EventHandler(this.punchOut_DoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(190, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Out:";
            this.label3.Visible = false;
            // 
            // Volume
            // 
            this.Volume.BackColor = System.Drawing.Color.Transparent;
            this.Volume.Enabled = false;
            this.Volume.Location = new System.Drawing.Point(108, 22);
            this.Volume.Max = 100;
            this.Volume.Name = "Volume";
            this.Volume.Size = new System.Drawing.Size(80, 20);
            this.Volume.TabIndex = 4;
            this.Volume.Value = 50;
            this.Volume.ValueChanged += new System.EventHandler(this.Volume_ValueChanged);
            // 
            // MuteButton
            // 
            this.MuteButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(85)))));
            this.MuteButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.MuteButton.Enabled = false;
            this.MuteButton.ImageDisabled = global::TerraViewer.Properties.Resources.glyph_mute_off_disabled;
            this.MuteButton.ImageEnabled = global::TerraViewer.Properties.Resources.glyph_mute_off_enabled;
            this.MuteButton.Location = new System.Drawing.Point(67, 16);
            this.MuteButton.MaximumSize = new System.Drawing.Size(140, 33);
            this.MuteButton.Name = "MuteButton";
            this.MuteButton.Selected = false;
            this.MuteButton.Size = new System.Drawing.Size(39, 33);
            this.MuteButton.TabIndex = 0;
            this.MuteButton.Click += new System.EventHandler(this.Mute_Click);
            this.MuteButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Mute_MouseDown);
            // 
            // Browse
            // 
            this.Browse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(85)))));
            this.Browse.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Browse.Enabled = false;
            this.Browse.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.Browse.ImageDisabled = null;
            this.Browse.ImageEnabled = null;
            this.Browse.Location = new System.Drawing.Point(-3, 16);
            this.Browse.MaximumSize = new System.Drawing.Size(140, 33);
            this.Browse.Name = "Browse";
            this.Browse.Selected = false;
            this.Browse.Size = new System.Drawing.Size(76, 33);
            this.Browse.TabIndex = 0;
            this.Browse.Text = "Add...";
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // AudioTrack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(85)))));
            this.Controls.Add(this.Volume);
            this.Controls.Add(this.punchOut);
            this.Controls.Add(this.punchIn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label43);
            this.Controls.Add(this.MuteButton);
            this.Controls.Add(this.Browse);
            this.DoubleBuffered = true;
            this.Enabled = false;
            this.MaximumSize = new System.Drawing.Size(193, 46);
            this.MinimumSize = new System.Drawing.Size(193, 46);
            this.Name = "AudioTrack";
            this.Size = new System.Drawing.Size(193, 46);
            this.Load += new System.EventHandler(this.AudioTrack_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.AudioTrack_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton Browse;
        private WwtButton MuteButton;
        private System.Windows.Forms.TextBox punchIn;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.TextBox punchOut;
        private System.Windows.Forms.Label label3;
        private WwtTrackBar Volume;
    }
}
