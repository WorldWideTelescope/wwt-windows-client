namespace TerraViewer
{
    partial class WwtCombo
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
            // WwtCombo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(248, 33);
            this.MinimumSize = new System.Drawing.Size(35, 33);
            this.Name = "WwtCombo";
            this.Size = new System.Drawing.Size(248, 33);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WwtCombo_MouseDown);
            this.MouseEnter += new System.EventHandler(this.WwtCombo_MouseEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.WwtCombo_Paint);
            this.MouseLeave += new System.EventHandler(this.WwtCombo_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.WwtCombo_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion


    }
}
