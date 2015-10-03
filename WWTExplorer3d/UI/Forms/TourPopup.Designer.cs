namespace TerraViewer
{
    partial class TourPopup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TourPopup));
            this.ratingLabel = new System.Windows.Forms.Label();
            this.TourDescription = new System.Windows.Forms.TextBox();
            this.authorImage = new System.Windows.Forms.PictureBox();
            this.authorUrl = new System.Windows.Forms.Label();
            this.orgUrl = new System.Windows.Forms.Label();
            this.runLengthLabel = new System.Windows.Forms.Label();
            this.runLength = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TourTitle = new System.Windows.Forms.Label();
            this.averageStars = new TerraViewer.UserRating();
            this.Preview = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ttTourPopup = new System.Windows.Forms.ToolTip(this.components);
            this.fadeInTimer = new System.Windows.Forms.Timer(this.components);
            this.tourWrapPanel = new System.Windows.Forms.Panel();
            this.MyRating = new TerraViewer.UserRating();
            this.CloseTour = new TerraViewer.WwtButton();
            this.yourRatingLabel = new System.Windows.Forms.Label();
            this.WatchAgain = new TerraViewer.WwtButton();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.paginator1 = new TerraViewer.Paginator();
            this.relatedTours = new TerraViewer.ThumbnailList();
            ((System.ComponentModel.ISupportInitialize)(this.authorImage)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Preview)).BeginInit();
            this.tourWrapPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ratingLabel
            // 
            this.ratingLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ratingLabel.AutoSize = true;
            this.ratingLabel.Location = new System.Drawing.Point(355, 10);
            this.ratingLabel.Name = "ratingLabel";
            this.ratingLabel.Size = new System.Drawing.Size(41, 13);
            this.ratingLabel.TabIndex = 1;
            this.ratingLabel.Text = "Rating:";
            // 
            // TourDescription
            // 
            this.TourDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TourDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.TourDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TourDescription.Enabled = false;
            this.TourDescription.ForeColor = System.Drawing.Color.White;
            this.TourDescription.Location = new System.Drawing.Point(100, 117);
            this.TourDescription.Multiline = true;
            this.TourDescription.Name = "TourDescription";
            this.TourDescription.ReadOnly = true;
            this.TourDescription.Size = new System.Drawing.Size(349, 98);
            this.TourDescription.TabIndex = 3;
            this.TourDescription.Text = resources.GetString("TourDescription.Text");
            // 
            // authorImage
            // 
            this.authorImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.authorImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.authorImage.Location = new System.Drawing.Point(12, 75);
            this.authorImage.Name = "authorImage";
            this.authorImage.Size = new System.Drawing.Size(72, 96);
            this.authorImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.authorImage.TabIndex = 3;
            this.authorImage.TabStop = false;
            // 
            // authorUrl
            // 
            this.authorUrl.AutoSize = true;
            this.authorUrl.BackColor = System.Drawing.Color.Transparent;
            this.authorUrl.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.authorUrl.Location = new System.Drawing.Point(96, 72);
            this.authorUrl.Name = "authorUrl";
            this.authorUrl.Size = new System.Drawing.Size(85, 20);
            this.authorUrl.TabIndex = 1;
            this.authorUrl.TabStop = true;
            this.authorUrl.Text = "I. M. Author";
            this.authorUrl.Click += new System.EventHandler(this.authorUrl_LinkClicked);
            // 
            // orgUrl
            // 
            this.orgUrl.AutoSize = true;
            this.orgUrl.BackColor = System.Drawing.Color.Transparent;
            this.orgUrl.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orgUrl.Location = new System.Drawing.Point(96, 92);
            this.orgUrl.Name = "orgUrl";
            this.orgUrl.Size = new System.Drawing.Size(0, 20);
            this.orgUrl.TabIndex = 2;
            this.orgUrl.TabStop = true;
            this.orgUrl.Click += new System.EventHandler(this.orgUrl_LinkClicked);
            // 
            // runLengthLabel
            // 
            this.runLengthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.runLengthLabel.AutoSize = true;
            this.runLengthLabel.Location = new System.Drawing.Point(355, 30);
            this.runLengthLabel.Name = "runLengthLabel";
            this.runLengthLabel.Size = new System.Drawing.Size(66, 13);
            this.runLengthLabel.TabIndex = 3;
            this.runLengthLabel.Text = "Run Length:";
            // 
            // runLength
            // 
            this.runLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.runLength.Location = new System.Drawing.Point(419, 30);
            this.runLength.Name = "runLength";
            this.runLength.Size = new System.Drawing.Size(50, 13);
            this.runLength.TabIndex = 4;
            this.runLength.Text = "3:00";
            this.runLength.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(69)))), ((int)(((byte)(91)))));
            this.panel1.Controls.Add(this.TourTitle);
            this.panel1.Controls.Add(this.averageStars);
            this.panel1.Controls.Add(this.Preview);
            this.panel1.Controls.Add(this.runLengthLabel);
            this.panel1.Controls.Add(this.ratingLabel);
            this.panel1.Controls.Add(this.runLength);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(470, 55);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // TourTitle
            // 
            this.TourTitle.AutoEllipsis = true;
            this.TourTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TourTitle.ForeColor = System.Drawing.Color.White;
            this.TourTitle.Location = new System.Drawing.Point(54, 9);
            this.TourTitle.Name = "TourTitle";
            this.TourTitle.Size = new System.Drawing.Size(266, 32);
            this.TourTitle.TabIndex = 0;
            this.TourTitle.Text = "Galactic Collisions";
            // 
            // averageStars
            // 
            this.averageStars.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.averageStars.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(69)))), ((int)(((byte)(91)))));
            this.averageStars.Location = new System.Drawing.Point(396, 9);
            this.averageStars.Mode = TerraViewer.UserRating.Interactivity.ReadOnly;
            this.averageStars.Name = "averageStars";
            this.averageStars.Size = new System.Drawing.Size(72, 16);
            this.averageStars.Stars = 2.5;
            this.averageStars.StarSize = TerraViewer.UserRating.StarSizes.Small;
            this.averageStars.TabIndex = 2;
            this.toolTips.SetToolTip(this.averageStars, "Average rating from users");
            // 
            // Preview
            // 
            this.Preview.BackColor = System.Drawing.Color.Transparent;
            this.Preview.Image = global::TerraViewer.Properties.Resources.button_play_normal;
            this.Preview.Location = new System.Drawing.Point(-5, -5);
            this.Preview.Name = "Preview";
            this.Preview.Size = new System.Drawing.Size(64, 64);
            this.Preview.TabIndex = 9;
            this.Preview.TabStop = false;
            this.Preview.MouseLeave += new System.EventHandler(this.Preview_MouseLeave);
            this.Preview.Click += new System.EventHandler(this.Preview_Click);
            this.Preview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Preview_MouseDown);
            this.Preview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Preview_MouseUp);
            this.Preview.MouseEnter += new System.EventHandler(this.Preview_MouseEnter);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(12, 292);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 21);
            this.label3.TabIndex = 5;
            this.label3.Text = "Related Tours:";
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Location = new System.Drawing.Point(24, 318);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(437, 1);
            this.panel2.TabIndex = 7;
            // 
            // fadeInTimer
            // 
            this.fadeInTimer.Enabled = true;
            this.fadeInTimer.Interval = 10;
            this.fadeInTimer.Tick += new System.EventHandler(this.fadeInTimer_Tick);
            // 
            // tourWrapPanel
            // 
            this.tourWrapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tourWrapPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(69)))), ((int)(((byte)(91)))));
            this.tourWrapPanel.Controls.Add(this.MyRating);
            this.tourWrapPanel.Controls.Add(this.CloseTour);
            this.tourWrapPanel.Controls.Add(this.yourRatingLabel);
            this.tourWrapPanel.Controls.Add(this.WatchAgain);
            this.tourWrapPanel.Location = new System.Drawing.Point(16, 225);
            this.tourWrapPanel.Name = "tourWrapPanel";
            this.tourWrapPanel.Size = new System.Drawing.Size(445, 55);
            this.tourWrapPanel.TabIndex = 4;
            // 
            // MyRating
            // 
            this.MyRating.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(69)))), ((int)(((byte)(91)))));
            this.MyRating.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("MyRating.BackgroundImage")));
            this.MyRating.Location = new System.Drawing.Point(74, 16);
            this.MyRating.Mode = TerraViewer.UserRating.Interactivity.ReadWrite;
            this.MyRating.Name = "MyRating";
            this.MyRating.Size = new System.Drawing.Size(128, 24);
            this.MyRating.Stars = -1;
            this.MyRating.StarSize = TerraViewer.UserRating.StarSizes.Big;
            this.MyRating.TabIndex = 1;
            this.toolTips.SetToolTip(this.MyRating, "Click to set your own rating.");
            this.MyRating.ValueChanged += new System.EventHandler(this.MyRating_ValueChanged);
            // 
            // CloseTour
            // 
            this.CloseTour.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseTour.BackColor = System.Drawing.Color.Transparent;
            this.CloseTour.DialogResult = System.Windows.Forms.DialogResult.None;
            this.CloseTour.ImageDisabled = null;
            this.CloseTour.ImageEnabled = null;
            this.CloseTour.Location = new System.Drawing.Point(353, 11);
            this.CloseTour.MaximumSize = new System.Drawing.Size(140, 33);
            this.CloseTour.Name = "CloseTour";
            this.CloseTour.Selected = false;
            this.CloseTour.Size = new System.Drawing.Size(89, 33);
            this.CloseTour.TabIndex = 3;
            this.CloseTour.Text = "Close Tour";
            this.CloseTour.Click += new System.EventHandler(this.CloseTour_Click);
            // 
            // yourRatingLabel
            // 
            this.yourRatingLabel.AutoSize = true;
            this.yourRatingLabel.Location = new System.Drawing.Point(6, 21);
            this.yourRatingLabel.Name = "yourRatingLabel";
            this.yourRatingLabel.Size = new System.Drawing.Size(66, 13);
            this.yourRatingLabel.TabIndex = 0;
            this.yourRatingLabel.Text = "Your Rating:";
            // 
            // WatchAgain
            // 
            this.WatchAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.WatchAgain.BackColor = System.Drawing.Color.Transparent;
            this.WatchAgain.DialogResult = System.Windows.Forms.DialogResult.None;
            this.WatchAgain.ImageDisabled = null;
            this.WatchAgain.ImageEnabled = null;
            this.WatchAgain.Location = new System.Drawing.Point(253, 11);
            this.WatchAgain.MaximumSize = new System.Drawing.Size(140, 33);
            this.WatchAgain.Name = "WatchAgain";
            this.WatchAgain.Selected = false;
            this.WatchAgain.Size = new System.Drawing.Size(94, 33);
            this.WatchAgain.TabIndex = 2;
            this.WatchAgain.Text = "Watch Again";
            this.WatchAgain.Click += new System.EventHandler(this.WatchAgain_Click);
            // 
            // paginator1
            // 
            this.paginator1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.paginator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.paginator1.CurrentPage = 0;
            this.paginator1.Location = new System.Drawing.Point(363, 295);
            this.paginator1.Margin = new System.Windows.Forms.Padding(0);
            this.paginator1.Name = "paginator1";
            this.paginator1.Size = new System.Drawing.Size(98, 18);
            this.paginator1.TabIndex = 6;
            this.paginator1.TotalPages = 1;
            // 
            // relatedTours
            // 
            this.relatedTours.AddText = "Add New Item";
            this.relatedTours.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.relatedTours.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.relatedTours.ColCount = 4;
            this.relatedTours.DontStealFocus = false;
            this.relatedTours.EmptyAddText = "No Results";
            this.relatedTours.Items = ((System.Collections.Generic.List<object>)(resources.GetObject("relatedTours.Items")));
            this.relatedTours.Location = new System.Drawing.Point(16, 330);
            this.relatedTours.Margin = new System.Windows.Forms.Padding(0);
            this.relatedTours.MaximumSize = new System.Drawing.Size(2500, 475);
            this.relatedTours.MinimumSize = new System.Drawing.Size(100, 65);
            this.relatedTours.Name = "relatedTours";
            this.relatedTours.Paginator = this.paginator1;
            this.relatedTours.RowCount = 1;
            this.relatedTours.ShowAddButton = false;
            this.relatedTours.Size = new System.Drawing.Size(450, 65);
            this.relatedTours.TabIndex = 8;
            this.relatedTours.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            this.relatedTours.ItemDoubleClicked += new TerraViewer.ItemClickedEventHandler(this.relatedTours_ItemDoubleClicked);
            this.relatedTours.ItemClicked += new TerraViewer.ItemClickedEventHandler(this.relatedTours_ItemClicked);
            this.relatedTours.ItemHover += new TerraViewer.ItemClickedEventHandler(this.relatedTours_ItemHover);
            // 
            // TourPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(475, 405);
            this.Controls.Add(this.paginator1);
            this.Controls.Add(this.relatedTours);
            this.Controls.Add(this.tourWrapPanel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.orgUrl);
            this.Controls.Add(this.authorUrl);
            this.Controls.Add(this.authorImage);
            this.Controls.Add(this.TourDescription);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TourPopup";
            this.Opacity = 0;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TourPopup";
            this.Load += new System.EventHandler(this.TourPopup_Load);
            this.MouseEnter += new System.EventHandler(this.TourPopup_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.TourPopup_MouseLeave);
            ((System.ComponentModel.ISupportInitialize)(this.authorImage)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Preview)).EndInit();
            this.tourWrapPanel.ResumeLayout(false);
            this.tourWrapPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ratingLabel;
        private System.Windows.Forms.TextBox TourDescription;
        private System.Windows.Forms.PictureBox authorImage;
        private System.Windows.Forms.Label runLengthLabel;
        private System.Windows.Forms.Label runLength;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox Preview;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolTip ttTourPopup;
        private System.Windows.Forms.Label TourTitle;
        private System.Windows.Forms.Timer fadeInTimer;
        private System.Windows.Forms.Panel tourWrapPanel;
        private UserRating MyRating;
        private WwtButton CloseTour;
        private System.Windows.Forms.Label yourRatingLabel;
        private WwtButton WatchAgain;
        private UserRating averageStars;
        private System.Windows.Forms.ToolTip toolTips;
        private ThumbnailList relatedTours;
        private Paginator paginator1;
        private System.Windows.Forms.Label authorUrl;
        private System.Windows.Forms.Label orgUrl;
    }
}