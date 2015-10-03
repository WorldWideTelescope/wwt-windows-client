namespace TerraViewer
{
    partial class DomePreviewPopup
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AltAzText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // AltAzText
            // 
            this.AltAzText.AutoSize = true;
            this.AltAzText.Location = new System.Drawing.Point(0, 178);
            this.AltAzText.Name = "AltAzText";
            this.AltAzText.Size = new System.Drawing.Size(35, 13);
            this.AltAzText.TabIndex = 0;
            this.AltAzText.Text = "label1";
            // 
            // DomePreviewPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(192, 200);
            this.Controls.Add(this.AltAzText);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DomePreviewPopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Dome Preview";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DomePreviewPopup_FormClosed);
            this.Load += new System.EventHandler(this.DomePreviewPopup_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DomePreviewPopup_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DomePreviewPopup_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DomePreviewPopup_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DomePreviewPopup_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AltAzText;
    }
}