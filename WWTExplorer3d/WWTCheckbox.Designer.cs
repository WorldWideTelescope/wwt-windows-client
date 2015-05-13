namespace TerraViewer
{
    partial class WWTCheckbox
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
            // WWTCheckbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.DoubleBuffered = true;
            this.Name = "WWTCheckbox";
            this.Size = new System.Drawing.Size(190, 25);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.WWTCheckbox_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.WWTCheckbox_MouseDown);
            this.MouseEnter += new System.EventHandler(this.WWTCheckbox_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.WWTCheckbox_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.WWTCheckbox_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
