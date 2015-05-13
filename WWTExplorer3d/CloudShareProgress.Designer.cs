namespace TerraViewer
{
    partial class CloudShareProgress
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
            this.progress = new System.Windows.Forms.ProgressBar();
            this.backgroundUpdater = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(0, 0);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(284, 35);
            this.progress.TabIndex = 0;
            // 
            // backgroundUpdater
            // 
            this.backgroundUpdater.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundUpdater_DoWork);
            // 
            // CloudShareProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 35);
            this.ControlBox = false;
            this.Controls.Add(this.progress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CloudShareProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sharing...";
            this.Load += new System.EventHandler(this.CloudShareProgress_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progress;
        private System.ComponentModel.BackgroundWorker backgroundUpdater;
    }
}