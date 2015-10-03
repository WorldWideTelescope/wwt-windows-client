namespace TerraViewer
{
    partial class SelectLink
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
            this.ReturnToCallerCheckbox = new TerraViewer.WWTCheckbox();
            this.LinkToNextCheckbox = new TerraViewer.WWTCheckbox();
            this.LinkToSlideCheckbox = new TerraViewer.WWTCheckbox();
            this.cancel = new TerraViewer.WwtButton();
            this.Ok = new TerraViewer.WwtButton();
            this.tourStopList = new TerraViewer.TourStopList();
            this.SuspendLayout();
            // 
            // ReturnToCallerCheckbox
            // 
            this.ReturnToCallerCheckbox.Checked = false;
            this.ReturnToCallerCheckbox.Location = new System.Drawing.Point(413, 28);
            this.ReturnToCallerCheckbox.Name = "ReturnToCallerCheckbox";
            this.ReturnToCallerCheckbox.Size = new System.Drawing.Size(130, 25);
            this.ReturnToCallerCheckbox.TabIndex = 2;
            this.ReturnToCallerCheckbox.Text = "Return to Caller";
            this.ReturnToCallerCheckbox.CheckedChanged += new System.EventHandler(this.ReturnToCallerCheckbox_CheckedChanged);
            // 
            // LinkToNextCheckbox
            // 
            this.LinkToNextCheckbox.Checked = false;
            this.LinkToNextCheckbox.Location = new System.Drawing.Point(226, 28);
            this.LinkToNextCheckbox.Name = "LinkToNextCheckbox";
            this.LinkToNextCheckbox.Size = new System.Drawing.Size(190, 25);
            this.LinkToNextCheckbox.TabIndex = 2;
            this.LinkToNextCheckbox.Text = "Link to Next Slide";
            this.LinkToNextCheckbox.CheckedChanged += new System.EventHandler(this.LinkToNextCheckbox_CheckedChanged);
            // 
            // LinkToSlideCheckbox
            // 
            this.LinkToSlideCheckbox.Checked = false;
            this.LinkToSlideCheckbox.Location = new System.Drawing.Point(12, 28);
            this.LinkToSlideCheckbox.Name = "LinkToSlideCheckbox";
            this.LinkToSlideCheckbox.Size = new System.Drawing.Size(190, 25);
            this.LinkToSlideCheckbox.TabIndex = 2;
            this.LinkToSlideCheckbox.Text = "Link to Slide (Select below)";
            this.LinkToSlideCheckbox.CheckedChanged += new System.EventHandler(this.LinkToSlideCheckbox_CheckedChanged);
            // 
            // cancel
            // 
            this.cancel.BackColor = System.Drawing.Color.Transparent;
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.cancel.ImageDisabled = null;
            this.cancel.ImageEnabled = null;
            this.cancel.Location = new System.Drawing.Point(511, 188);
            this.cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.cancel.Name = "cancel";
            this.cancel.Selected = false;
            this.cancel.Size = new System.Drawing.Size(77, 33);
            this.cancel.TabIndex = 1;
            this.cancel.Text = "Cancel";
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // Ok
            // 
            this.Ok.BackColor = System.Drawing.Color.Transparent;
            this.Ok.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Ok.ImageDisabled = null;
            this.Ok.ImageEnabled = null;
            this.Ok.Location = new System.Drawing.Point(436, 188);
            this.Ok.MaximumSize = new System.Drawing.Size(140, 33);
            this.Ok.Name = "Ok";
            this.Ok.Selected = false;
            this.Ok.Size = new System.Drawing.Size(77, 33);
            this.Ok.TabIndex = 1;
            this.Ok.Text = "Ok";
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // tourStopList
            // 
            this.tourStopList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.tourStopList.ColCount = 5;
            this.tourStopList.Location = new System.Drawing.Point(11, 80);
            this.tourStopList.Margin = new System.Windows.Forms.Padding(0);
            this.tourStopList.MaximumSize = new System.Drawing.Size(2500, 105);
            this.tourStopList.MinimumSize = new System.Drawing.Size(100, 105);
            this.tourStopList.Name = "tourStopList";
            this.tourStopList.Paginator = null;
            this.tourStopList.RowCount = 1;
            this.tourStopList.SelectedItem = -1;
            this.tourStopList.ShowAddButton = true;
            this.tourStopList.Size = new System.Drawing.Size(570, 105);
            this.tourStopList.TabIndex = 0;
            this.tourStopList.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            this.tourStopList.TotalItems = 0;
            // 
            // SelectLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(39)))));
            this.ClientSize = new System.Drawing.Size(596, 229);
            this.Controls.Add(this.ReturnToCallerCheckbox);
            this.Controls.Add(this.LinkToNextCheckbox);
            this.Controls.Add(this.LinkToSlideCheckbox);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.Ok);
            this.Controls.Add(this.tourStopList);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectLink";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Link Navigation";
            this.Load += new System.EventHandler(this.SelectLink_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private TourStopList tourStopList;
        private WwtButton Ok;
        private WwtButton cancel;
        private WWTCheckbox LinkToSlideCheckbox;
        private WWTCheckbox LinkToNextCheckbox;
        private WWTCheckbox ReturnToCallerCheckbox;
    }
}