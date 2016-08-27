namespace TerraViewer
{
    partial class TourEditTab
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
            this.tourStopList = new TerraViewer.TourStopList();
            this.PlayerTimer = new System.Windows.Forms.Timer(this.components);
            this.AddText = new System.Windows.Forms.Button();
            this.EditTourProperties = new TerraViewer.WwtButton();
            this.AddShape = new System.Windows.Forms.Button();
            this.AddPicture = new System.Windows.Forms.Button();
            this.AddVideo = new System.Windows.Forms.Button();
            this.SaveTour = new TerraViewer.WwtButton();
            this.Preview = new System.Windows.Forms.PictureBox();
            this.MusicTrack = new TerraViewer.AudioTrack();
            this.VoiceTrack = new TerraViewer.AudioTrack();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ShowSafeArea = new TerraViewer.WWTCheckbox();
            this.runTimeLabel = new System.Windows.Forms.Label();
            this.totalTimeText = new System.Windows.Forms.Label();
            this.Dome = new TerraViewer.WWTCheckbox();
            ((System.ComponentModel.ISupportInitialize)(this.Preview)).BeginInit();
            this.SuspendLayout();
            // 
            // pinUp
            // 
            this.pinUp.Enabled = false;
            this.pinUp.Location = new System.Drawing.Point(719, 154);
            this.pinUp.MaximumSize = new System.Drawing.Size(75, 35);
            this.pinUp.MinimumSize = new System.Drawing.Size(75, 35);
            this.pinUp.Size = new System.Drawing.Size(75, 35);
            this.pinUp.TabIndex = 12;
            this.pinUp.Visible = false;
            // 
            // tourStopList
            // 
            this.tourStopList.AllowMultipleSelection = false;
            this.tourStopList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tourStopList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.tourStopList.ColCount = 5;
            this.tourStopList.HitType = TerraViewer.TourStopList.HitPosition.Default;
            this.tourStopList.Location = new System.Drawing.Point(108, 12);
            this.tourStopList.Margin = new System.Windows.Forms.Padding(0);
            this.tourStopList.MaximumSize = new System.Drawing.Size(6144, 731);
            this.tourStopList.MinimumSize = new System.Drawing.Size(150, 100);
            this.tourStopList.MultipleSelection = false;
            this.tourStopList.Name = "tourStopList";
            this.tourStopList.Paginator = null;
            this.tourStopList.RowCount = 2;
            this.tourStopList.SelectedItem = -1;
            this.tourStopList.ShowAddButton = true;
            this.tourStopList.Size = new System.Drawing.Size(836, 162);
            this.tourStopList.TabIndex = 2;
            this.tourStopList.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            this.toolTip.SetToolTip(this.tourStopList, "Slides");
            this.tourStopList.TotalItems = 0;
            this.tourStopList.ItemHover += new TerraViewer.TourStopClickedEventHandler(this.tourStopList_ItemHover);
            this.tourStopList.ItemClicked += new TerraViewer.TourStopClickedEventHandler(this.tourStopList_ItemClicked);
            this.tourStopList.ItemDoubleClicked += new TerraViewer.TourStopClickedEventHandler(this.tourStopList_ItemDoubleClicked);
            this.tourStopList.AddNewSlide += new TerraViewer.TourStopClickedEventHandler(this.tourStopList_AddNewSlide);
            this.tourStopList.ShowEndPosition += new TerraViewer.TourStopClickedEventHandler(this.tourStopList_ShowEndPosition);
            this.tourStopList.ShowStartPosition += new TerraViewer.TourStopClickedEventHandler(this.tourStopList_ShowStartPosition);
            this.tourStopList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tourStopList_KeyDown);
            this.tourStopList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tourStopList_MouseClick);
            // 
            // PlayerTimer
            // 
            this.PlayerTimer.Enabled = true;
            this.PlayerTimer.Interval = 250;
            this.PlayerTimer.Tick += new System.EventHandler(this.PlayerTimer_Tick);
            // 
            // AddText
            // 
            this.AddText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddText.ForeColor = System.Drawing.Color.White;
            this.AddText.Image = global::TerraViewer.Properties.Resources.tool_icon_text_24;
            this.AddText.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.AddText.Location = new System.Drawing.Point(956, 97);
            this.AddText.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AddText.Name = "AddText";
            this.AddText.Size = new System.Drawing.Size(81, 69);
            this.AddText.TabIndex = 7;
            this.AddText.Text = "Text";
            this.AddText.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.AddText.UseVisualStyleBackColor = true;
            this.AddText.Click += new System.EventHandler(this.AddText_Click);
            // 
            // EditTourProperties
            // 
            this.EditTourProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EditTourProperties.BackColor = System.Drawing.Color.Transparent;
            this.EditTourProperties.DialogResult = System.Windows.Forms.DialogResult.None;
            this.EditTourProperties.ImageDisabled = null;
            this.EditTourProperties.ImageEnabled = null;
            this.EditTourProperties.Location = new System.Drawing.Point(948, 6);
            this.EditTourProperties.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.EditTourProperties.MaximumSize = new System.Drawing.Size(210, 51);
            this.EditTourProperties.Name = "EditTourProperties";
            this.EditTourProperties.Selected = false;
            this.EditTourProperties.Size = new System.Drawing.Size(168, 51);
            this.EditTourProperties.TabIndex = 3;
            this.EditTourProperties.Text = "Tour Properties";
            this.EditTourProperties.Click += new System.EventHandler(this.EditTourProperties_Click);
            // 
            // AddShape
            // 
            this.AddShape.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddShape.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddShape.ForeColor = System.Drawing.Color.White;
            this.AddShape.Image = global::TerraViewer.Properties.Resources.tool_icon_shape_24;
            this.AddShape.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.AddShape.Location = new System.Drawing.Point(1036, 97);
            this.AddShape.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AddShape.Name = "AddShape";
            this.AddShape.Size = new System.Drawing.Size(81, 69);
            this.AddShape.TabIndex = 8;
            this.AddShape.Text = "Shapes";
            this.AddShape.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.AddShape.UseVisualStyleBackColor = true;
            this.AddShape.Click += new System.EventHandler(this.AddShape_Click);
            this.AddShape.Paint += new System.Windows.Forms.PaintEventHandler(this.AddShape_Paint);
            // 
            // AddPicture
            // 
            this.AddPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddPicture.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddPicture.ForeColor = System.Drawing.Color.White;
            this.AddPicture.Image = global::TerraViewer.Properties.Resources.tool_icon_picture_24;
            this.AddPicture.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.AddPicture.Location = new System.Drawing.Point(1118, 97);
            this.AddPicture.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AddPicture.Name = "AddPicture";
            this.AddPicture.Size = new System.Drawing.Size(81, 69);
            this.AddPicture.TabIndex = 9;
            this.AddPicture.Text = "Picture";
            this.AddPicture.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.AddPicture.UseVisualStyleBackColor = true;
            this.AddPicture.Click += new System.EventHandler(this.AddPicture_Click);
            // 
            // AddVideo
            // 
            this.AddVideo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddVideo.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddVideo.ForeColor = System.Drawing.Color.White;
            this.AddVideo.Image = global::TerraViewer.Properties.Resources.tool_icon_video_24;
            this.AddVideo.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.AddVideo.Location = new System.Drawing.Point(1118, 97);
            this.AddVideo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.AddVideo.Name = "AddVideo";
            this.AddVideo.Size = new System.Drawing.Size(81, 69);
            this.AddVideo.TabIndex = 6;
            this.AddVideo.Text = "Video";
            this.AddVideo.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.AddVideo.UseVisualStyleBackColor = true;
            this.AddVideo.Visible = false;
            this.AddVideo.Click += new System.EventHandler(this.AddVideo_Click);
            // 
            // SaveTour
            // 
            this.SaveTour.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveTour.BackColor = System.Drawing.Color.Transparent;
            this.SaveTour.DialogResult = System.Windows.Forms.DialogResult.None;
            this.SaveTour.ImageDisabled = null;
            this.SaveTour.ImageEnabled = null;
            this.SaveTour.Location = new System.Drawing.Point(1108, 6);
            this.SaveTour.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.SaveTour.MaximumSize = new System.Drawing.Size(210, 51);
            this.SaveTour.Name = "SaveTour";
            this.SaveTour.Selected = false;
            this.SaveTour.Size = new System.Drawing.Size(100, 51);
            this.SaveTour.TabIndex = 4;
            this.SaveTour.Text = " Save";
            this.SaveTour.Click += new System.EventHandler(this.SaveTour_Click);
            // 
            // Preview
            // 
            this.Preview.BackColor = System.Drawing.Color.Transparent;
            this.Preview.Image = global::TerraViewer.Properties.Resources.button_play_normal;
            this.Preview.Location = new System.Drawing.Point(6, 25);
            this.Preview.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Preview.Name = "Preview";
            this.Preview.Size = new System.Drawing.Size(96, 98);
            this.Preview.TabIndex = 8;
            this.Preview.TabStop = false;
            this.Preview.EnabledChanged += new System.EventHandler(this.Preview_EnabledChanged);
            this.Preview.Click += new System.EventHandler(this.Preview_Click);
            this.Preview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Preview_MouseDown);
            this.Preview.MouseEnter += new System.EventHandler(this.Preview_MouseEnter);
            this.Preview.MouseLeave += new System.EventHandler(this.Preview_MouseLeave);
            this.Preview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Preview_MouseUp);
            // 
            // MusicTrack
            // 
            this.MusicTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MusicTrack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(85)))));
            this.MusicTrack.Enabled = false;
            this.MusicTrack.Location = new System.Drawing.Point(1212, 15);
            this.MusicTrack.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.MusicTrack.MaximumSize = new System.Drawing.Size(290, 71);
            this.MusicTrack.MinimumSize = new System.Drawing.Size(290, 71);
            this.MusicTrack.Mute = false;
            this.MusicTrack.Name = "MusicTrack";
            this.MusicTrack.Size = new System.Drawing.Size(290, 71);
            this.MusicTrack.TabIndex = 10;
            this.MusicTrack.Target = null;
            this.MusicTrack.TrackType = TerraViewer.AudioType.Music;
            this.MusicTrack.Load += new System.EventHandler(this.MusicTrack_Load);
            // 
            // VoiceTrack
            // 
            this.VoiceTrack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.VoiceTrack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(63)))), ((int)(((byte)(85)))));
            this.VoiceTrack.Enabled = false;
            this.VoiceTrack.Location = new System.Drawing.Point(1212, 105);
            this.VoiceTrack.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.VoiceTrack.MaximumSize = new System.Drawing.Size(290, 71);
            this.VoiceTrack.MinimumSize = new System.Drawing.Size(290, 71);
            this.VoiceTrack.Mute = false;
            this.VoiceTrack.Name = "VoiceTrack";
            this.VoiceTrack.Size = new System.Drawing.Size(290, 71);
            this.VoiceTrack.TabIndex = 11;
            this.VoiceTrack.Target = null;
            this.VoiceTrack.TrackType = TerraViewer.AudioType.Voice;
            // 
            // ShowSafeArea
            // 
            this.ShowSafeArea.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowSafeArea.BackColor = System.Drawing.Color.Transparent;
            this.ShowSafeArea.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ShowSafeArea.Checked = false;
            this.ShowSafeArea.Location = new System.Drawing.Point(948, 54);
            this.ShowSafeArea.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.ShowSafeArea.Name = "ShowSafeArea";
            this.ShowSafeArea.Size = new System.Drawing.Size(168, 38);
            this.ShowSafeArea.TabIndex = 5;
            this.ShowSafeArea.Text = "Show Safe Area";
            this.ShowSafeArea.CheckedChanged += new System.EventHandler(this.ShowSafeArea_CheckedChanged);
            // 
            // runTimeLabel
            // 
            this.runTimeLabel.AutoSize = true;
            this.runTimeLabel.BackColor = System.Drawing.Color.Transparent;
            this.runTimeLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.runTimeLabel.ForeColor = System.Drawing.Color.White;
            this.runTimeLabel.Location = new System.Drawing.Point(14, 128);
            this.runTimeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.runTimeLabel.Name = "runTimeLabel";
            this.runTimeLabel.Size = new System.Drawing.Size(82, 23);
            this.runTimeLabel.TabIndex = 0;
            this.runTimeLabel.Text = "Run Time";
            // 
            // totalTimeText
            // 
            this.totalTimeText.AutoSize = true;
            this.totalTimeText.BackColor = System.Drawing.Color.Transparent;
            this.totalTimeText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalTimeText.ForeColor = System.Drawing.Color.White;
            this.totalTimeText.Location = new System.Drawing.Point(33, 155);
            this.totalTimeText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.totalTimeText.Name = "totalTimeText";
            this.totalTimeText.Size = new System.Drawing.Size(41, 23);
            this.totalTimeText.TabIndex = 1;
            this.totalTimeText.Text = "3:34";
            // 
            // Dome
            // 
            this.Dome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Dome.BackColor = System.Drawing.Color.Transparent;
            this.Dome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Dome.Checked = false;
            this.Dome.Location = new System.Drawing.Point(1108, 54);
            this.Dome.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.Dome.Name = "Dome";
            this.Dome.Size = new System.Drawing.Size(98, 38);
            this.Dome.TabIndex = 6;
            this.Dome.Text = "Dome";
            this.Dome.CheckedChanged += new System.EventHandler(this.Dome_CheckedChanged);
            // 
            // TourEditTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1512, 189);
            this.Controls.Add(this.Dome);
            this.Controls.Add(this.totalTimeText);
            this.Controls.Add(this.runTimeLabel);
            this.Controls.Add(this.ShowSafeArea);
            this.Controls.Add(this.VoiceTrack);
            this.Controls.Add(this.MusicTrack);
            this.Controls.Add(this.Preview);
            this.Controls.Add(this.SaveTour);
            this.Controls.Add(this.EditTourProperties);
            this.Controls.Add(this.AddPicture);
            this.Controls.Add(this.AddShape);
            this.Controls.Add(this.AddText);
            this.Controls.Add(this.tourStopList);
            this.Controls.Add(this.AddVideo);
            this.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.Name = "TourEditTab";
            this.Text = "Create";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TourEditTab_FormClosed);
            this.Load += new System.EventHandler(this.TourEdit_Load);
            this.Leave += new System.EventHandler(this.TourEditTab_Leave);
            this.Controls.SetChildIndex(this.AddVideo, 0);
            this.Controls.SetChildIndex(this.pinUp, 0);
            this.Controls.SetChildIndex(this.tourStopList, 0);
            this.Controls.SetChildIndex(this.AddText, 0);
            this.Controls.SetChildIndex(this.AddShape, 0);
            this.Controls.SetChildIndex(this.AddPicture, 0);
            this.Controls.SetChildIndex(this.EditTourProperties, 0);
            this.Controls.SetChildIndex(this.SaveTour, 0);
            this.Controls.SetChildIndex(this.Preview, 0);
            this.Controls.SetChildIndex(this.MusicTrack, 0);
            this.Controls.SetChildIndex(this.VoiceTrack, 0);
            this.Controls.SetChildIndex(this.ShowSafeArea, 0);
            this.Controls.SetChildIndex(this.runTimeLabel, 0);
            this.Controls.SetChildIndex(this.totalTimeText, 0);
            this.Controls.SetChildIndex(this.Dome, 0);
            ((System.ComponentModel.ISupportInitialize)(this.Preview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TourStopList tourStopList;
        private System.Windows.Forms.Timer PlayerTimer;
        private System.Windows.Forms.Button AddText;
        private WwtButton EditTourProperties;
        private System.Windows.Forms.Button AddShape;
        private System.Windows.Forms.Button AddPicture;
        private System.Windows.Forms.Button AddVideo;
        private WwtButton SaveTour;
        private System.Windows.Forms.PictureBox Preview;
        private AudioTrack MusicTrack;
        private AudioTrack VoiceTrack;
        private System.Windows.Forms.ToolTip toolTip;
        private WWTCheckbox ShowSafeArea;
        private System.Windows.Forms.Label runTimeLabel;
        private System.Windows.Forms.Label totalTimeText;
        private WWTCheckbox Dome;
    }
}