namespace TerraViewer
{
    partial class MenuTabs
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
            // MenuTabs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::TerraViewer.Properties.Resources.tabBackground;
            this.DoubleBuffered = true;
            this.Name = "MenuTabs";
            this.Size = new System.Drawing.Size(1002, 34);
            this.Load += new System.EventHandler(this.MenuTabs_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MenuTabs_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MenuTabs_MouseMove);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MenuTabs_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MenuTabs_MouseDoubleClick);
            this.MouseEnter += new System.EventHandler(this.MenuTabs_MouseEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MenuTabs_Paint);
            this.MouseLeave += new System.EventHandler(this.MenuTabs_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MenuTabs_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
