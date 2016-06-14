namespace TerraViewer
{
    partial class KioskTitleBar
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
            this.titleBar = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // titleBar
            // 
            this.titleBar.AutoSize = true;
            this.titleBar.BackColor = System.Drawing.Color.Transparent;
            this.titleBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleBar.ForeColor = System.Drawing.Color.White;
            this.titleBar.Location = new System.Drawing.Point(14, 9);
            this.titleBar.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.titleBar.Name = "titleBar";
            this.titleBar.Size = new System.Drawing.Size(918, 29);
            this.titleBar.TabIndex = 0;
            this.titleBar.Text = "WorldWide Telescope - Take it home with you at http://www.worldwidetelescope.org";
            // 
            // KioskTitleBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TerraViewer.Properties.Resources.tabBackground;
            this.Controls.Add(this.titleBar);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "KioskTitleBar";
            this.Size = new System.Drawing.Size(1200, 52);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleBar;
    }
}
