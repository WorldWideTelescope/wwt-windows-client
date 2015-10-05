namespace TerraViewer
{
    partial class KeyFramer
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
            this.TimeLine = new TerraViewer.TimeLine();
            this.PushPin = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PushPin)).BeginInit();
            this.SuspendLayout();
            // 
            // TimeLine
            // 
            this.TimeLine.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TimeLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.TimeLine.ForeColor = System.Drawing.Color.White;
            this.TimeLine.Location = new System.Drawing.Point(12, 12);
            this.TimeLine.MinimumSize = new System.Drawing.Size(300, 120);
            this.TimeLine.Name = "TimeLine";
            this.TimeLine.Size = new System.Drawing.Size(1582, 204);
            this.TimeLine.TabIndex = 4;
            this.TimeLine.Tour = null;
            this.TimeLine.TweenPosition = 0D;
            this.TimeLine.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TimeLine_KeyDown);
            // 
            // PushPin
            // 
            this.PushPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PushPin.Image = global::TerraViewer.Properties.Resources.pushpin;
            this.PushPin.Location = new System.Drawing.Point(1583, -2);
            this.PushPin.Name = "PushPin";
            this.PushPin.Size = new System.Drawing.Size(11, 14);
            this.PushPin.TabIndex = 58;
            this.PushPin.TabStop = false;
            this.PushPin.Click += new System.EventHandler(this.PushPin_Click);
            // 
            // KeyFramer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(44)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(1603, 225);
            this.Controls.Add(this.PushPin);
            this.Controls.Add(this.TimeLine);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyFramer";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Timeline Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KeyFramer_FormClosing);
            this.Load += new System.EventHandler(this.KeyFramer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PushPin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TimeLine TimeLine;
        private System.Windows.Forms.PictureBox PushPin;
    }
}