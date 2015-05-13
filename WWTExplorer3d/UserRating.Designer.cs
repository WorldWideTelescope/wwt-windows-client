namespace TerraViewer
{
    partial class UserRating
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
            // UserRating
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.DoubleBuffered = true;
            this.Name = "UserRating";
            this.Size = new System.Drawing.Size(128, 24);
            this.Load += new System.EventHandler(this.UserRating_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UserRating_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UserRating_MouseMove);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UserRating_Paint);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UserRating_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
