namespace TerraViewer
{
    partial class Histogram
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
            this.HistogramView = new System.Windows.Forms.PictureBox();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.zoomIn = new TerraViewer.WwtButton();
            this.zoomOut = new TerraViewer.WwtButton();
            this.scaleType = new TerraViewer.WwtCombo();
            ((System.ComponentModel.ISupportInitialize)(this.HistogramView)).BeginInit();
            this.SuspendLayout();
            // 
            // HistogramView
            // 
            this.HistogramView.Location = new System.Drawing.Point(12, 12);
            this.HistogramView.Name = "HistogramView";
            this.HistogramView.Size = new System.Drawing.Size(256, 150);
            this.HistogramView.TabIndex = 0;
            this.HistogramView.TabStop = false;
            this.HistogramView.Click += new System.EventHandler(this.HistogramView_Click);
            this.HistogramView.Paint += new System.Windows.Forms.PaintEventHandler(this.HistogramView_Paint);
            this.HistogramView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HistogramView_MouseDown);
            this.HistogramView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HistogramView_MouseMove);
            this.HistogramView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HistogramView_MouseUp);
            // 
            // updateTimer
            // 
            this.updateTimer.Interval = 250;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // zoomIn
            // 
            this.zoomIn.BackColor = System.Drawing.Color.Transparent;
            this.zoomIn.DialogResult = System.Windows.Forms.DialogResult.None;
            this.zoomIn.ImageDisabled = null;
            this.zoomIn.ImageEnabled = global::TerraViewer.Properties.Resources.ZoomIn;
            this.zoomIn.Location = new System.Drawing.Point(240, 163);
            this.zoomIn.MaximumSize = new System.Drawing.Size(140, 33);
            this.zoomIn.Name = "zoomIn";
            this.zoomIn.Selected = false;
            this.zoomIn.Size = new System.Drawing.Size(34, 33);
            this.zoomIn.TabIndex = 2;
            // 
            // zoomOut
            // 
            this.zoomOut.BackColor = System.Drawing.Color.Transparent;
            this.zoomOut.DialogResult = System.Windows.Forms.DialogResult.None;
            this.zoomOut.ImageDisabled = null;
            this.zoomOut.ImageEnabled = global::TerraViewer.Properties.Resources.ZoomOut;
            this.zoomOut.Location = new System.Drawing.Point(211, 163);
            this.zoomOut.MaximumSize = new System.Drawing.Size(140, 33);
            this.zoomOut.Name = "zoomOut";
            this.zoomOut.Selected = false;
            this.zoomOut.Size = new System.Drawing.Size(34, 33);
            this.zoomOut.TabIndex = 2;
            // 
            // scaleType
            // 
            this.scaleType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.scaleType.DateTimeValue = new System.DateTime(2009, 6, 25, 18, 28, 4, 480);
            this.scaleType.Filter = TerraViewer.Classification.Unfiltered;
            this.scaleType.FilterStyle = false;
            this.scaleType.Location = new System.Drawing.Point(13, 163);
            this.scaleType.Margin = new System.Windows.Forms.Padding(0);
            this.scaleType.MasterTime = true;
            this.scaleType.MaximumSize = new System.Drawing.Size(248, 33);
            this.scaleType.MinimumSize = new System.Drawing.Size(35, 33);
            this.scaleType.Name = "scaleType";
            this.scaleType.SelectedIndex = -1;
            this.scaleType.SelectedItem = null;
            this.scaleType.Size = new System.Drawing.Size(195, 33);
            this.scaleType.State = TerraViewer.State.Rest;
            this.scaleType.TabIndex = 1;
            this.scaleType.Type = TerraViewer.WwtCombo.ComboType.List;
            this.scaleType.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.scaleType_SelectionChanged);
            // 
            // Histogram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(278, 200);
            this.Controls.Add(this.zoomIn);
            this.Controls.Add(this.zoomOut);
            this.Controls.Add(this.scaleType);
            this.Controls.Add(this.HistogramView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Histogram";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "w";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Histogram_FormClosed);
            this.Load += new System.EventHandler(this.Histogram_Load);
            ((System.ComponentModel.ISupportInitialize)(this.HistogramView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox HistogramView;
        private WwtCombo scaleType;
        private WwtButton zoomOut;
        private WwtButton zoomIn;
        private System.Windows.Forms.Timer updateTimer;
    }
}