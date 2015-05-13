namespace TerraViewer.Callibration
{
    partial class CalibrationScreen
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
            this.SuspendLayout();
            // 
            // CalibrationScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(734, 620);
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CalibrationScreen";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CalibrationScreen";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.CalibrationScreen_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.CalibrationScreen_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CalibrationScreen_Paint);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CalibrationScreen_FormClosed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CalibrationScreen_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CalibrationScreen_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

    }
}