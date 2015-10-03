namespace TerraViewer
{
    partial class AudioProperties
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
            this.ok = new TerraViewer.WwtButton();
            this.Remove = new TerraViewer.WwtButton();
            this.AudioFileNameLabel = new System.Windows.Forms.Label();
            this.AudioFileName = new System.Windows.Forms.TextBox();
            this.fadeOutLabel = new System.Windows.Forms.Label();
            this.fadeInLabel = new System.Windows.Forms.Label();
            this.endLabel = new System.Windows.Forms.Label();
            this.beginLabel = new System.Windows.Forms.Label();
            this.FadeOut = new System.Windows.Forms.TextBox();
            this.FadeIn = new System.Windows.Forms.TextBox();
            this.End = new System.Windows.Forms.TextBox();
            this.Begin = new System.Windows.Forms.TextBox();
            this.Preview = new System.Windows.Forms.PictureBox();
            this.Repeat = new TerraViewer.WWTCheckbox();
            ((System.ComponentModel.ISupportInitialize)(this.Preview)).BeginInit();
            this.SuspendLayout();
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.BackColor = System.Drawing.Color.Transparent;
            this.ok.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ok.ImageDisabled = null;
            this.ok.ImageEnabled = null;
            this.ok.Location = new System.Drawing.Point(149, 168);
            this.ok.MaximumSize = new System.Drawing.Size(140, 33);
            this.ok.Name = "ok";
            this.ok.Selected = false;
            this.ok.Size = new System.Drawing.Size(73, 33);
            this.ok.TabIndex = 12;
            this.ok.Text = "Ok";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // Remove
            // 
            this.Remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Remove.BackColor = System.Drawing.Color.Transparent;
            this.Remove.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Remove.ImageDisabled = null;
            this.Remove.ImageEnabled = null;
            this.Remove.Location = new System.Drawing.Point(79, 168);
            this.Remove.MaximumSize = new System.Drawing.Size(140, 33);
            this.Remove.Name = "Remove";
            this.Remove.Selected = false;
            this.Remove.Size = new System.Drawing.Size(73, 33);
            this.Remove.TabIndex = 11;
            this.Remove.Text = "Remove";
            this.Remove.Click += new System.EventHandler(this.Remove_Click);
            // 
            // AudioFileNameLabel
            // 
            this.AudioFileNameLabel.AutoSize = true;
            this.AudioFileNameLabel.BackColor = System.Drawing.Color.Transparent;
            this.AudioFileNameLabel.Location = new System.Drawing.Point(12, 9);
            this.AudioFileNameLabel.Name = "AudioFileNameLabel";
            this.AudioFileNameLabel.Size = new System.Drawing.Size(79, 13);
            this.AudioFileNameLabel.TabIndex = 0;
            this.AudioFileNameLabel.Text = "Audio Filename";
            // 
            // AudioFileName
            // 
            this.AudioFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AudioFileName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.AudioFileName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AudioFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AudioFileName.ForeColor = System.Drawing.Color.White;
            this.AudioFileName.Location = new System.Drawing.Point(15, 25);
            this.AudioFileName.MaxLength = 64;
            this.AudioFileName.Name = "AudioFileName";
            this.AudioFileName.ReadOnly = true;
            this.AudioFileName.Size = new System.Drawing.Size(207, 20);
            this.AudioFileName.TabIndex = 1;
            // 
            // fadeOutLabel
            // 
            this.fadeOutLabel.AutoSize = true;
            this.fadeOutLabel.BackColor = System.Drawing.Color.Transparent;
            this.fadeOutLabel.Location = new System.Drawing.Point(80, 93);
            this.fadeOutLabel.Name = "fadeOutLabel";
            this.fadeOutLabel.Size = new System.Drawing.Size(51, 13);
            this.fadeOutLabel.TabIndex = 8;
            this.fadeOutLabel.Text = "Fade Out";
            // 
            // fadeInLabel
            // 
            this.fadeInLabel.AutoSize = true;
            this.fadeInLabel.BackColor = System.Drawing.Color.Transparent;
            this.fadeInLabel.Location = new System.Drawing.Point(14, 93);
            this.fadeInLabel.Name = "fadeInLabel";
            this.fadeInLabel.Size = new System.Drawing.Size(43, 13);
            this.fadeInLabel.TabIndex = 6;
            this.fadeInLabel.Text = "Fade In";
            // 
            // endLabel
            // 
            this.endLabel.AutoSize = true;
            this.endLabel.BackColor = System.Drawing.Color.Transparent;
            this.endLabel.Location = new System.Drawing.Point(80, 50);
            this.endLabel.Name = "endLabel";
            this.endLabel.Size = new System.Drawing.Size(26, 13);
            this.endLabel.TabIndex = 4;
            this.endLabel.Text = "End";
            // 
            // beginLabel
            // 
            this.beginLabel.AutoSize = true;
            this.beginLabel.BackColor = System.Drawing.Color.Transparent;
            this.beginLabel.Location = new System.Drawing.Point(14, 50);
            this.beginLabel.Name = "beginLabel";
            this.beginLabel.Size = new System.Drawing.Size(34, 13);
            this.beginLabel.TabIndex = 2;
            this.beginLabel.Text = "Begin";
            // 
            // FadeOut
            // 
            this.FadeOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.FadeOut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FadeOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FadeOut.ForeColor = System.Drawing.Color.White;
            this.FadeOut.Location = new System.Drawing.Point(83, 109);
            this.FadeOut.MaxLength = 64;
            this.FadeOut.Name = "FadeOut";
            this.FadeOut.Size = new System.Drawing.Size(60, 20);
            this.FadeOut.TabIndex = 9;
            this.FadeOut.Text = "0";
            this.FadeOut.TextChanged += new System.EventHandler(this.FadeOut_TextChanged);
            // 
            // FadeIn
            // 
            this.FadeIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.FadeIn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FadeIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FadeIn.ForeColor = System.Drawing.Color.White;
            this.FadeIn.Location = new System.Drawing.Point(17, 109);
            this.FadeIn.MaxLength = 64;
            this.FadeIn.Name = "FadeIn";
            this.FadeIn.Size = new System.Drawing.Size(60, 20);
            this.FadeIn.TabIndex = 7;
            this.FadeIn.Text = "0";
            this.FadeIn.TextChanged += new System.EventHandler(this.FadeIn_TextChanged);
            // 
            // End
            // 
            this.End.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.End.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.End.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.End.ForeColor = System.Drawing.Color.White;
            this.End.Location = new System.Drawing.Point(83, 66);
            this.End.MaxLength = 64;
            this.End.Name = "End";
            this.End.Size = new System.Drawing.Size(60, 20);
            this.End.TabIndex = 5;
            this.End.Text = "0";
            this.End.TextChanged += new System.EventHandler(this.End_TextChanged);
            // 
            // Begin
            // 
            this.Begin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Begin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Begin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Begin.ForeColor = System.Drawing.Color.White;
            this.Begin.Location = new System.Drawing.Point(17, 66);
            this.Begin.MaxLength = 64;
            this.Begin.Name = "Begin";
            this.Begin.Size = new System.Drawing.Size(60, 20);
            this.Begin.TabIndex = 3;
            this.Begin.Text = "0";
            this.Begin.TextChanged += new System.EventHandler(this.Begin_TextChanged);
            // 
            // Preview
            // 
            this.Preview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Preview.BackColor = System.Drawing.Color.Transparent;
            this.Preview.Image = global::TerraViewer.Properties.Resources.button_play_normal;
            this.Preview.Location = new System.Drawing.Point(158, 65);
            this.Preview.Name = "Preview";
            this.Preview.Size = new System.Drawing.Size(64, 64);
            this.Preview.TabIndex = 18;
            this.Preview.TabStop = false;
            this.Preview.Click += new System.EventHandler(this.Preview_Click);
            this.Preview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Preview_MouseDown);
            this.Preview.MouseEnter += new System.EventHandler(this.Preview_MouseEnter);
            this.Preview.MouseLeave += new System.EventHandler(this.Preview_MouseLeave);
            this.Preview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Preview_MouseUp);
            // 
            // Repeat
            // 
            this.Repeat.BackColor = System.Drawing.Color.Transparent;
            this.Repeat.Checked = false;
            this.Repeat.Location = new System.Drawing.Point(13, 136);
            this.Repeat.Name = "Repeat";
            this.Repeat.Size = new System.Drawing.Size(118, 25);
            this.Repeat.TabIndex = 10;
            this.Repeat.Text = "Auto Repeat";
            this.Repeat.CheckedChanged += new System.EventHandler(this.Repeat_CheckedChanged);
            // 
            // AudioProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(234, 213);
            this.Controls.Add(this.Repeat);
            this.Controls.Add(this.Preview);
            this.Controls.Add(this.FadeOut);
            this.Controls.Add(this.FadeIn);
            this.Controls.Add(this.End);
            this.Controls.Add(this.Begin);
            this.Controls.Add(this.AudioFileNameLabel);
            this.Controls.Add(this.AudioFileName);
            this.Controls.Add(this.Remove);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.fadeOutLabel);
            this.Controls.Add(this.fadeInLabel);
            this.Controls.Add(this.endLabel);
            this.Controls.Add(this.beginLabel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AudioProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Audio Properties";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AudioProperties_FormClosed);
            this.Load += new System.EventHandler(this.AudioProperties_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Preview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton ok;
        private WwtButton Remove;
        private System.Windows.Forms.Label AudioFileNameLabel;
        private System.Windows.Forms.TextBox AudioFileName;
        private System.Windows.Forms.Label fadeOutLabel;
        private System.Windows.Forms.Label fadeInLabel;
        private System.Windows.Forms.Label endLabel;
        private System.Windows.Forms.Label beginLabel;
        private System.Windows.Forms.TextBox FadeOut;
        private System.Windows.Forms.TextBox FadeIn;
        private System.Windows.Forms.TextBox End;
        private System.Windows.Forms.TextBox Begin;
        private System.Windows.Forms.PictureBox Preview;
        private WWTCheckbox Repeat;
    }
}