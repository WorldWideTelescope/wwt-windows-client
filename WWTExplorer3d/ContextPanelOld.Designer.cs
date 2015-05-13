namespace TerraViewer
{
    partial class ContextPanelOld
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
            this.queueProgressBar = new System.Windows.Forms.ProgressBar();
            this.ContellationLabel = new System.Windows.Forms.Label();
            this.levelLabel = new System.Windows.Forms.Label();
            this.searchTimer = new System.Windows.Forms.Timer(this.components);
            this.wwtButton1 = new TerraViewer.WwtButton();
            this.FilterCombo = new TerraViewer.WwtCombo();
            this.paginator1 = new TerraViewer.Paginator();
            this.contextResults = new TerraViewer.ThumbnailList();
            this.pinUp = new TerraViewer.PinUp();
            this.overview = new TerraViewer.Overview();
            this.SkyBall = new TerraViewer.SkyBall();
            this.SuspendLayout();
            // 
            // queueProgressBar
            // 
            this.queueProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.queueProgressBar.Location = new System.Drawing.Point(773, 101);
            this.queueProgressBar.Name = "queueProgressBar";
            this.queueProgressBar.Size = new System.Drawing.Size(115, 7);
            this.queueProgressBar.TabIndex = 2;
            this.queueProgressBar.Value = 50;
            // 
            // ContellationLabel
            // 
            this.ContellationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ContellationLabel.AutoSize = true;
            this.ContellationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContellationLabel.Location = new System.Drawing.Point(770, 9);
            this.ContellationLabel.Name = "ContellationLabel";
            this.ContellationLabel.Size = new System.Drawing.Size(51, 12);
            this.ContellationLabel.TabIndex = 3;
            this.ContellationLabel.Text = "Ursa Major";
            // 
            // levelLabel
            // 
            this.levelLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.levelLabel.AutoSize = true;
            this.levelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.levelLabel.Location = new System.Drawing.Point(869, 9);
            this.levelLabel.Name = "levelLabel";
            this.levelLabel.Size = new System.Drawing.Size(25, 13);
            this.levelLabel.TabIndex = 4;
            this.levelLabel.Text = "L12";
            this.levelLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // searchTimer
            // 
            this.searchTimer.Enabled = true;
            this.searchTimer.Interval = 1000;
            this.searchTimer.Tick += new System.EventHandler(this.searchTimer_Tick);
            // 
            // wwtButton1
            // 
            this.wwtButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.wwtButton1.Location = new System.Drawing.Point(137, 2);
            this.wwtButton1.MaximumSize = new System.Drawing.Size(140, 33);
            this.wwtButton1.MinimumSize = new System.Drawing.Size(10, 33);
            this.wwtButton1.Name = "wwtButton1";
            this.wwtButton1.Selected = false;
            this.wwtButton1.Size = new System.Drawing.Size(93, 33);
            this.wwtButton1.TabIndex = 9;
            // 
            // FilterCombo
            // 
            this.FilterCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.FilterCombo.Location = new System.Drawing.Point(10, 4);
            this.FilterCombo.Margin = new System.Windows.Forms.Padding(0);
            this.FilterCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.FilterCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.FilterCombo.Name = "FilterCombo";
            this.FilterCombo.Size = new System.Drawing.Size(121, 33);
            this.FilterCombo.State = TerraViewer.State.Rest;
            this.FilterCombo.TabIndex = 8;
            // 
            // paginator1
            // 
            this.paginator1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.paginator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.paginator1.CurrentPage = 0;
            this.paginator1.Location = new System.Drawing.Point(575, 7);
            this.paginator1.Margin = new System.Windows.Forms.Padding(0);
            this.paginator1.Name = "paginator1";
            this.paginator1.Size = new System.Drawing.Size(85, 16);
            this.paginator1.TabIndex = 7;
            this.paginator1.TotalPages = 0;
            // 
            // contextResults
            // 
            this.contextResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.contextResults.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.contextResults.Location = new System.Drawing.Point(9, 46);
            this.contextResults.Margin = new System.Windows.Forms.Padding(0);
            this.contextResults.MaximumSize = new System.Drawing.Size(2500, 294);
            this.contextResults.MinimumSize = new System.Drawing.Size(100, 65);
            this.contextResults.Name = "contextResults";
            this.contextResults.Size = new System.Drawing.Size(645, 65);
            this.contextResults.TabIndex = 6;
            this.contextResults.ItemDoubleClicked += new TerraViewer.ItemClickedEventHandler(this.contextResults_ItemDoubleClicked);
            this.contextResults.ItemClicked += new TerraViewer.ItemClickedEventHandler(this.contextResults_ItemClicked);
            // 
            // pinUp
            // 
            this.pinUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.pinUp.Direction = TerraViewer.Direction.Expanding;
            this.pinUp.Location = new System.Drawing.Point(416, 0);
            this.pinUp.Margin = new System.Windows.Forms.Padding(0);
            this.pinUp.MaximumSize = new System.Drawing.Size(33, 12);
            this.pinUp.MinimumSize = new System.Drawing.Size(33, 12);
            this.pinUp.Name = "pinUp";
            this.pinUp.Placement = TerraViewer.Placement.Bottom;
            this.pinUp.Size = new System.Drawing.Size(33, 12);
            this.pinUp.State = TerraViewer.State.Rest;
            this.pinUp.TabIndex = 10;
            this.pinUp.Clicked += new TerraViewer.ClickedEventHandler(this.pinUp_Clicked);
            // 
            // overview
            // 
            this.overview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.overview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.overview.Constellation = null;
            this.overview.ForeColor = System.Drawing.Color.White;
            this.overview.Location = new System.Drawing.Point(773, 25);
            this.overview.Margin = new System.Windows.Forms.Padding(0);
            this.overview.MaximumSize = new System.Drawing.Size(115, 66);
            this.overview.MinimumSize = new System.Drawing.Size(115, 66);
            this.overview.Name = "overview";
            this.overview.Size = new System.Drawing.Size(115, 66);
            this.overview.TabIndex = 5;
            // 
            // SkyBall
            // 
            this.SkyBall.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.SkyBall.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SkyBall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.SkyBall.Dec = 0;
            this.SkyBall.ForeColor = System.Drawing.Color.White;
            this.SkyBall.Location = new System.Drawing.Point(666, 0);
            this.SkyBall.Margin = new System.Windows.Forms.Padding(0);
            this.SkyBall.MaximumSize = new System.Drawing.Size(96, 120);
            this.SkyBall.MinimumSize = new System.Drawing.Size(96, 120);
            this.SkyBall.Name = "SkyBall";
            this.SkyBall.RA = 0;
            this.SkyBall.Size = new System.Drawing.Size(96, 120);
            this.SkyBall.TabIndex = 0;
            // 
            // ContextPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(900, 120);
            this.ControlBox = false;
            this.Controls.Add(this.pinUp);
            this.Controls.Add(this.wwtButton1);
            this.Controls.Add(this.FilterCombo);
            this.Controls.Add(this.paginator1);
            this.Controls.Add(this.contextResults);
            this.Controls.Add(this.overview);
            this.Controls.Add(this.levelLabel);
            this.Controls.Add(this.ContellationLabel);
            this.Controls.Add(this.queueProgressBar);
            this.Controls.Add(this.SkyBall);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ContextPanel";
            this.Opacity = 0.8;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Load += new System.EventHandler(this.ContextPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public SkyBall SkyBall;

        private System.Windows.Forms.ProgressBar queueProgressBar;
        private System.Windows.Forms.Label ContellationLabel;
        private System.Windows.Forms.Label levelLabel;
        private ThumbnailList contextResults;
        private System.Windows.Forms.Timer searchTimer;
        private Paginator paginator1;
        private WwtCombo FilterCombo;
        protected Overview overview;
        private PinUp pinUp;
        protected WwtButton wwtButton1;

    }
}