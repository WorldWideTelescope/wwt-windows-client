namespace TerraViewer
{
    partial class WwtTrackBar
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
            this.SuspendLayout();
            // 
            // WwtTrackBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "WwtTrackBar";
            this.Size = new System.Drawing.Size(80, 20);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WwtTrackBar_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.WwtTrackBar_MouseMove);
            this.MouseEnter += new System.EventHandler(this.WwtTrackBar_MouseEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.WwtTrackBar_Paint);
            this.MouseLeave += new System.EventHandler(this.WwtTrackBar_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.WwtTrackBar_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

    }
}
