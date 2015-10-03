namespace TerraViewer
{
    partial class ArrowButton
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
            this.repeatTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // repeatTimer
            // 
            this.repeatTimer.Interval = 250;
            this.repeatTimer.Tick += new System.EventHandler(this.repeatTimer_Tick);
            // 
            // ArrowButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.MaximumSize = new System.Drawing.Size(34, 19);
            this.MinimumSize = new System.Drawing.Size(33, 15);
            this.Name = "ArrowButton";
            this.Size = new System.Drawing.Size(34, 19);
            this.Load += new System.EventHandler(this.ArrowButton_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ArrowButton_MouseDown);
            this.MouseEnter += new System.EventHandler(this.ArrowButton_MouseEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ArrowButton_Paint);
            this.MouseLeave += new System.EventHandler(this.ArrowButton_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ArrowButton_MouseUp);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer repeatTimer;
    }
}
