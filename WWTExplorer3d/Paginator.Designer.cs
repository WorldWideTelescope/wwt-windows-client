namespace TerraViewer
{
    partial class Paginator
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
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Paginator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "Paginator";
            this.Size = new System.Drawing.Size(98, 18);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Paginator_MouseDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Paginator_MouseClick);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Paginator_MouseDoubleClick);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Paginator_Paint);
            this.MouseLeave += new System.EventHandler(this.Paginator_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Paginator_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
    }
}
