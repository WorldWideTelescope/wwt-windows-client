namespace TerraViewer
{
    partial class ClearCache
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClearCache));
            this.imagerySize = new System.Windows.Forms.Label();
            this.tourSize = new System.Windows.Forms.Label();
            this.warningText = new System.Windows.Forms.Label();
            this.catalogSize = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressText = new System.Windows.Forms.Label();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.catalogs = new TerraViewer.WWTCheckbox();
            this.tours = new TerraViewer.WWTCheckbox();
            this.imagery = new TerraViewer.WWTCheckbox();
            this.Cancel = new TerraViewer.WwtButton();
            this.Purge = new TerraViewer.WwtButton();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // imagerySize
            // 
            this.imagerySize.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.imagerySize.ForeColor = System.Drawing.Color.White;
            this.imagerySize.Location = new System.Drawing.Point(146, 37);
            this.imagerySize.Name = "imagerySize";
            this.imagerySize.Size = new System.Drawing.Size(100, 16);
            this.imagerySize.TabIndex = 18;
            this.imagerySize.Text = "Size: 10MB";
            // 
            // tourSize
            // 
            this.tourSize.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tourSize.ForeColor = System.Drawing.Color.White;
            this.tourSize.Location = new System.Drawing.Point(146, 60);
            this.tourSize.Name = "tourSize";
            this.tourSize.Size = new System.Drawing.Size(100, 16);
            this.tourSize.TabIndex = 18;
            this.tourSize.Text = "Size: 10MB";
            // 
            // warningText
            // 
            this.warningText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.warningText.ForeColor = System.Drawing.Color.White;
            this.warningText.Location = new System.Drawing.Point(9, 104);
            this.warningText.Name = "warningText";
            this.warningText.Size = new System.Drawing.Size(269, 118);
            this.warningText.TabIndex = 18;
            this.warningText.Text = resources.GetString("warningText.Text");
            this.warningText.Click += new System.EventHandler(this.catalogSize_Click);
            // 
            // catalogSize
            // 
            this.catalogSize.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.catalogSize.ForeColor = System.Drawing.Color.White;
            this.catalogSize.Location = new System.Drawing.Point(146, 83);
            this.catalogSize.Name = "catalogSize";
            this.catalogSize.Size = new System.Drawing.Size(100, 16);
            this.catalogSize.TabIndex = 18;
            this.catalogSize.Text = "Size: 10MB";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 241);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(260, 23);
            this.progressBar.TabIndex = 19;
            // 
            // progressText
            // 
            this.progressText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressText.ForeColor = System.Drawing.Color.White;
            this.progressText.Location = new System.Drawing.Point(12, 222);
            this.progressText.Name = "progressText";
            this.progressText.Size = new System.Drawing.Size(260, 16);
            this.progressText.TabIndex = 18;
            this.progressText.Text = "Calculating Cache Use";
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 250;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // catalogs
            // 
            this.catalogs.Checked = false;
            this.catalogs.Location = new System.Drawing.Point(12, 76);
            this.catalogs.Name = "catalogs";
            this.catalogs.Size = new System.Drawing.Size(110, 25);
            this.catalogs.TabIndex = 17;
            this.catalogs.Text = "Catalogs";
            // 
            // tours
            // 
            this.tours.Checked = false;
            this.tours.Location = new System.Drawing.Point(12, 54);
            this.tours.Name = "tours";
            this.tours.Size = new System.Drawing.Size(110, 25);
            this.tours.TabIndex = 17;
            this.tours.Text = "Cached Tours";
            // 
            // imagery
            // 
            this.imagery.Checked = false;
            this.imagery.Location = new System.Drawing.Point(12, 32);
            this.imagery.Name = "imagery";
            this.imagery.Size = new System.Drawing.Size(110, 25);
            this.imagery.TabIndex = 17;
            this.imagery.Text = "Imagery";
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(206, 289);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(72, 33);
            this.Cancel.TabIndex = 16;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Purge
            // 
            this.Purge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Purge.BackColor = System.Drawing.Color.Transparent;
            this.Purge.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Purge.ImageDisabled = null;
            this.Purge.ImageEnabled = null;
            this.Purge.Location = new System.Drawing.Point(131, 289);
            this.Purge.MaximumSize = new System.Drawing.Size(140, 33);
            this.Purge.Name = "Purge";
            this.Purge.Selected = false;
            this.Purge.Size = new System.Drawing.Size(73, 33);
            this.Purge.TabIndex = 15;
            this.Purge.Text = "Purge";
            this.Purge.Click += new System.EventHandler(this.OK_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(260, 16);
            this.label1.TabIndex = 18;
            this.label1.Text = "Select Data to Purge ";
            // 
            // ClearCache
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(284, 330);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.warningText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressText);
            this.Controls.Add(this.catalogSize);
            this.Controls.Add(this.tourSize);
            this.Controls.Add(this.imagerySize);
            this.Controls.Add(this.catalogs);
            this.Controls.Add(this.tours);
            this.Controls.Add(this.imagery);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Purge);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClearCache";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Data Cache";
            this.Load += new System.EventHandler(this.ClearCache_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private WwtButton Cancel;
        private WwtButton Purge;
        private WWTCheckbox imagery;
        private WWTCheckbox tours;
        private WWTCheckbox catalogs;
        private System.Windows.Forms.Label imagerySize;
        private System.Windows.Forms.Label tourSize;
        private System.Windows.Forms.Label warningText;
        private System.Windows.Forms.Label catalogSize;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressText;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label1;
    }
}