namespace TerraViewer
{
    partial class ImageStack
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageStack));
            this.stackList = new TerraViewer.ThumbnailList();
            this.closeBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fadeTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // stackList
            // 
            this.stackList.AddText = "Add New Item";
            this.stackList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stackList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.stackList.ColCount = 1;
            this.stackList.DontStealFocus = false;
            this.stackList.EmptyAddText = "No Results";
            this.stackList.Items = ((System.Collections.Generic.List<object>)(resources.GetObject("stackList.Items")));
            this.stackList.Location = new System.Drawing.Point(9, 26);
            this.stackList.Margin = new System.Windows.Forms.Padding(0);
            this.stackList.MaximumSize = new System.Drawing.Size(2500, 2475);
            this.stackList.MinimumSize = new System.Drawing.Size(100, 65);
            this.stackList.Name = "stackList";
            this.stackList.Paginator = null;
            this.stackList.RowCount = 8;
            this.stackList.ShowAddButton = false;
            this.stackList.Size = new System.Drawing.Size(101, 582);
            this.stackList.TabIndex = 0;
            this.stackList.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            // 
            // closeBox
            // 
            this.closeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBox.Image = global::TerraViewer.Properties.Resources.CloseRest;
            this.closeBox.Location = new System.Drawing.Point(102, 3);
            this.closeBox.Name = "closeBox";
            this.closeBox.Size = new System.Drawing.Size(16, 16);
            this.closeBox.TabIndex = 4;
            this.closeBox.TabStop = false;
            this.closeBox.MouseLeave += new System.EventHandler(this.closeBox_MouseLeave);
            this.closeBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.closeBox_MouseDown);
            this.closeBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.closeBox_MouseUp);
            this.closeBox.MouseEnter += new System.EventHandler(this.closeBox_MouseEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Image Stack";
            // 
            // fadeTimer
            // 
            this.fadeTimer.Enabled = true;
            this.fadeTimer.Interval = 10;
            this.fadeTimer.Tick += new System.EventHandler(this.fadeTimer_Tick);
            // 
            // ImageStack
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(119, 617);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.closeBox);
            this.Controls.Add(this.stackList);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImageStack";
            this.Opacity = 0.8;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ImageStack";
            this.MouseEnter += new System.EventHandler(this.TabForm_MouseEnter);
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ThumbnailList stackList;
        private System.Windows.Forms.PictureBox closeBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer fadeTimer;
    }
}