namespace TerraViewer
{
    partial class TransitionPicker
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
            // TransitionPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(18)))), ((int)(((byte)(25)))));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "TransitionPicker";
            this.Size = new System.Drawing.Size(251, 74);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TransitionPicker_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TransitionPicker_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
