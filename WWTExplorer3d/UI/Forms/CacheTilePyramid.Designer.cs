namespace TerraViewer
{
    partial class CacheTilePyramid
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CacheTilePyramid));
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressText = new System.Windows.Forms.Label();
            this.Cancel = new TerraViewer.WwtButton();
            this.Download = new TerraViewer.WwtButton();
            this.HelpText = new System.Windows.Forms.Label();
            this.levelsLabel = new System.Windows.Forms.Label();
            this.levels = new System.Windows.Forms.TextBox();
            this.maxLevelsLabel = new System.Windows.Forms.Label();
            this.maxLevels = new System.Windows.Forms.TextBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 177);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(260, 23);
            this.progressBar.TabIndex = 21;
            // 
            // progressText
            // 
            this.progressText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressText.ForeColor = System.Drawing.Color.White;
            this.progressText.Location = new System.Drawing.Point(12, 158);
            this.progressText.Name = "progressText";
            this.progressText.Size = new System.Drawing.Size(260, 16);
            this.progressText.TabIndex = 20;
            this.progressText.Text = "Downloading Tiles";
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(200, 217);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(72, 33);
            this.Cancel.TabIndex = 23;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Download
            // 
            this.Download.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Download.BackColor = System.Drawing.Color.Transparent;
            this.Download.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Download.ImageDisabled = null;
            this.Download.ImageEnabled = null;
            this.Download.Location = new System.Drawing.Point(120, 217);
            this.Download.MaximumSize = new System.Drawing.Size(140, 33);
            this.Download.Name = "Download";
            this.Download.Selected = false;
            this.Download.Size = new System.Drawing.Size(78, 33);
            this.Download.TabIndex = 22;
            this.Download.Text = "Download";
            this.Download.Click += new System.EventHandler(this.Download_Click);
            // 
            // HelpText
            // 
            this.HelpText.ForeColor = System.Drawing.Color.White;
            this.HelpText.Location = new System.Drawing.Point(12, 13);
            this.HelpText.Name = "HelpText";
            this.HelpText.Size = new System.Drawing.Size(260, 82);
            this.HelpText.TabIndex = 24;
            this.HelpText.Text = resources.GetString("HelpText.Text");
            // 
            // levelsLabel
            // 
            this.levelsLabel.AutoSize = true;
            this.levelsLabel.Location = new System.Drawing.Point(96, 113);
            this.levelsLabel.Name = "levelsLabel";
            this.levelsLabel.Size = new System.Drawing.Size(38, 13);
            this.levelsLabel.TabIndex = 25;
            this.levelsLabel.Text = "Levels";
            // 
            // levels
            // 
            this.levels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.levels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.levels.ForeColor = System.Drawing.Color.White;
            this.levels.Location = new System.Drawing.Point(99, 129);
            this.levels.Name = "levels";
            this.levels.Size = new System.Drawing.Size(35, 20);
            this.levels.TabIndex = 26;
            this.levels.Text = "3";
            // 
            // maxLevelsLabel
            // 
            this.maxLevelsLabel.AutoSize = true;
            this.maxLevelsLabel.Location = new System.Drawing.Point(12, 113);
            this.maxLevelsLabel.Name = "maxLevelsLabel";
            this.maxLevelsLabel.Size = new System.Drawing.Size(61, 13);
            this.maxLevelsLabel.TabIndex = 25;
            this.maxLevelsLabel.Text = "Max Levels";
            // 
            // maxLevels
            // 
            this.maxLevels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.maxLevels.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maxLevels.ForeColor = System.Drawing.Color.White;
            this.maxLevels.Location = new System.Drawing.Point(15, 129);
            this.maxLevels.Name = "maxLevels";
            this.maxLevels.Size = new System.Drawing.Size(35, 20);
            this.maxLevels.TabIndex = 26;
            this.maxLevels.Text = "3";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // CacheTilePyramid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.maxLevels);
            this.Controls.Add(this.levels);
            this.Controls.Add(this.maxLevelsLabel);
            this.Controls.Add(this.levelsLabel);
            this.Controls.Add(this.HelpText);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Download);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.progressText);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CacheTilePyramid";
            this.ShowIcon = false;
            this.Text = "Download Image Tile Pyramid";
            this.Load += new System.EventHandler(this.CacheTilePyramid_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressText;
        private WwtButton Cancel;
        private WwtButton Download;
        private System.Windows.Forms.Label HelpText;
        private System.Windows.Forms.Label levelsLabel;
        private System.Windows.Forms.TextBox levels;
        private System.Windows.Forms.Label maxLevelsLabel;
        private System.Windows.Forms.TextBox maxLevels;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
    }
}