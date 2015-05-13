namespace TerraViewer
{
    partial class WwtButton
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
            // WwtButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(140, 33);
            this.Name = "WwtButton";
            this.Size = new System.Drawing.Size(140, 33);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WwtButton_MouseDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.WwtButton_MouseClick);
            this.MouseEnter += new System.EventHandler(this.WwtButton_MouseEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.WwtButton_Paint);
            this.MouseLeave += new System.EventHandler(this.WwtButton_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.WwtButton_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
