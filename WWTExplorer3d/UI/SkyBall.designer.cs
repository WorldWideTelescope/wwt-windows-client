namespace TerraViewer
{
    partial class SkyBall
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
            this.components = new System.ComponentModel.Container();
            this.raLabel = new System.Windows.Forms.Label();
            this.decLabel = new System.Windows.Forms.Label();
            this.MoveTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // raLabel
            // 
            this.raLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.raLabel.AutoSize = true;
            this.raLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.raLabel.Location = new System.Drawing.Point(11, 89);
            this.raLabel.Name = "raLabel";
            this.raLabel.Size = new System.Drawing.Size(66, 12);
            this.raLabel.TabIndex = 0;
            this.raLabel.Text = "RA   : 00:00:00";
            this.raLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SkyBall_MouseDown);
            this.raLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SkyBall_MouseMove);
            this.raLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SkyBall_MouseUp);
            // 
            // decLabel
            // 
            this.decLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.decLabel.AutoSize = true;
            this.decLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.decLabel.Location = new System.Drawing.Point(11, 100);
            this.decLabel.Name = "decLabel";
            this.decLabel.Size = new System.Drawing.Size(65, 12);
            this.decLabel.TabIndex = 0;
            this.decLabel.Text = "Dec : 00:00:00";
            this.decLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SkyBall_MouseDown);
            this.decLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SkyBall_MouseMove);
            this.decLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SkyBall_MouseUp);
            // 
            // MoveTimer
            // 
            this.MoveTimer.Tick += new System.EventHandler(this.MoveTimer_Tick);
            // 
            // SkyBall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.Controls.Add(this.decLabel);
            this.Controls.Add(this.raLabel);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(96, 120);
            this.MinimumSize = new System.Drawing.Size(96, 50);
            this.Name = "SkyBall";
            this.Size = new System.Drawing.Size(96, 120);
            this.Load += new System.EventHandler(this.SkyBall_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SkyBall_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SkyBall_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SkyBall_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.SkyBall_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label raLabel;
        private System.Windows.Forms.Label decLabel;
        private System.Windows.Forms.Timer MoveTimer;
    }
}
