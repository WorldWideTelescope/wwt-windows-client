namespace TerraViewer
{
    partial class ThumbnailList
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
            // ThumbnailList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(4096, 2500);
            this.MinimumSize = new System.Drawing.Size(100, 65);
            this.Name = "ThumbnailList";
            this.Size = new System.Drawing.Size(645, 65);
            this.Load += new System.EventHandler(this.ThumbnailList_Load);
            this.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ThumbnailList_Scroll);
            this.VisibleChanged += new System.EventHandler(this.ThumbnailList_VisibleChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ThumbnailList_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ThumbnailList_KeyDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ThumbnailList_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ThumbnailList_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ThumbnailList_MouseDown);
            this.MouseEnter += new System.EventHandler(this.ThumbnailList_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.ThumbnailList_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ThumbnailList_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ThumbnailList_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
