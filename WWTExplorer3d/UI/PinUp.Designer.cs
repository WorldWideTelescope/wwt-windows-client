namespace TerraViewer
{
    partial class PinUp
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
            // PinUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(33, 15);
            this.MinimumSize = new System.Drawing.Size(33, 15);
            this.Name = "PinUp";
            this.Size = new System.Drawing.Size(33, 15);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PinUp_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.PinUp_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PinUp_MouseDown);
            this.MouseEnter += new System.EventHandler(this.PinUp_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.PinUp_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PinUp_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
