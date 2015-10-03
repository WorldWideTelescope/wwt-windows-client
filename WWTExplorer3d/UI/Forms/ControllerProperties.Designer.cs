namespace TerraViewer
{
    partial class ControllerProperties
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
            this.Cancel = new TerraViewer.WwtButton();
            this.OK = new TerraViewer.WwtButton();
            this.label4 = new System.Windows.Forms.Label();
            this.DeviceImageUrl = new System.Windows.Forms.TextBox();
            this.DeviceImage = new System.Windows.Forms.PictureBox();
            this.DeviceName = new System.Windows.Forms.TextBox();
            this.DeviceNameLabel = new System.Windows.Forms.Label();
            this.DeviceTypeLabel = new System.Windows.Forms.Label();
            this.DeviceType = new System.Windows.Forms.TextBox();
            this.DeviceStatusLabel = new System.Windows.Forms.Label();
            this.DeviceStatus = new System.Windows.Forms.TextBox();
            this.SentStatus = new TerraViewer.WWTCheckbox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.DeviceImage)).BeginInit();
            this.SuspendLayout();
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(691, 604);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(79, 33);
            this.Cancel.TabIndex = 10;
            this.Cancel.Text = "Cancel";
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(615, 604);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(79, 33);
            this.OK.TabIndex = 9;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Image URL";
            // 
            // DeviceImageUrl
            // 
            this.DeviceImageUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DeviceImageUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.DeviceImageUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DeviceImageUrl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeviceImageUrl.ForeColor = System.Drawing.Color.White;
            this.DeviceImageUrl.Location = new System.Drawing.Point(12, 69);
            this.DeviceImageUrl.MaxLength = 512;
            this.DeviceImageUrl.Name = "DeviceImageUrl";
            this.DeviceImageUrl.Size = new System.Drawing.Size(597, 20);
            this.DeviceImageUrl.TabIndex = 7;
            this.DeviceImageUrl.Text = "http://";
            this.DeviceImageUrl.TextChanged += new System.EventHandler(this.DeviceImageUrl_TextChanged);
            // 
            // DeviceImage
            // 
            this.DeviceImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DeviceImage.ImageLocation = "";
            this.DeviceImage.Location = new System.Drawing.Point(12, 96);
            this.DeviceImage.Name = "DeviceImage";
            this.DeviceImage.Size = new System.Drawing.Size(597, 538);
            this.DeviceImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.DeviceImage.TabIndex = 9;
            this.DeviceImage.TabStop = false;
            // 
            // DeviceName
            // 
            this.DeviceName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.DeviceName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DeviceName.ForeColor = System.Drawing.Color.White;
            this.DeviceName.Location = new System.Drawing.Point(12, 25);
            this.DeviceName.Name = "DeviceName";
            this.DeviceName.ReadOnly = true;
            this.DeviceName.Size = new System.Drawing.Size(139, 20);
            this.DeviceName.TabIndex = 1;
            this.DeviceName.Text = "Device Name Here";
            // 
            // DeviceNameLabel
            // 
            this.DeviceNameLabel.AutoSize = true;
            this.DeviceNameLabel.Location = new System.Drawing.Point(9, 9);
            this.DeviceNameLabel.Name = "DeviceNameLabel";
            this.DeviceNameLabel.Size = new System.Drawing.Size(72, 13);
            this.DeviceNameLabel.TabIndex = 0;
            this.DeviceNameLabel.Text = "Device Name";
            // 
            // DeviceTypeLabel
            // 
            this.DeviceTypeLabel.AutoSize = true;
            this.DeviceTypeLabel.Location = new System.Drawing.Point(154, 9);
            this.DeviceTypeLabel.Name = "DeviceTypeLabel";
            this.DeviceTypeLabel.Size = new System.Drawing.Size(68, 13);
            this.DeviceTypeLabel.TabIndex = 2;
            this.DeviceTypeLabel.Text = "Device Type";
            // 
            // DeviceType
            // 
            this.DeviceType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.DeviceType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DeviceType.ForeColor = System.Drawing.Color.White;
            this.DeviceType.Location = new System.Drawing.Point(157, 25);
            this.DeviceType.Name = "DeviceType";
            this.DeviceType.ReadOnly = true;
            this.DeviceType.Size = new System.Drawing.Size(139, 20);
            this.DeviceType.TabIndex = 3;
            this.DeviceType.Text = "MIDI Controller";
            // 
            // DeviceStatusLabel
            // 
            this.DeviceStatusLabel.AutoSize = true;
            this.DeviceStatusLabel.Location = new System.Drawing.Point(299, 9);
            this.DeviceStatusLabel.Name = "DeviceStatusLabel";
            this.DeviceStatusLabel.Size = new System.Drawing.Size(37, 13);
            this.DeviceStatusLabel.TabIndex = 4;
            this.DeviceStatusLabel.Text = "Status";
            // 
            // DeviceStatus
            // 
            this.DeviceStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.DeviceStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DeviceStatus.ForeColor = System.Drawing.Color.White;
            this.DeviceStatus.Location = new System.Drawing.Point(302, 25);
            this.DeviceStatus.Name = "DeviceStatus";
            this.DeviceStatus.ReadOnly = true;
            this.DeviceStatus.Size = new System.Drawing.Size(84, 20);
            this.DeviceStatus.TabIndex = 5;
            this.DeviceStatus.Text = "Connected";
            // 
            // SentStatus
            // 
            this.SentStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SentStatus.Checked = false;
            this.SentStatus.Location = new System.Drawing.Point(616, 95);
            this.SentStatus.Name = "SentStatus";
            this.SentStatus.Size = new System.Drawing.Size(154, 25);
            this.SentStatus.TabIndex = 8;
            this.SentStatus.Text = "Allows Status Update";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 250;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ControllerProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(782, 649);
            this.Controls.Add(this.SentStatus);
            this.Controls.Add(this.DeviceStatus);
            this.Controls.Add(this.DeviceStatusLabel);
            this.Controls.Add(this.DeviceType);
            this.Controls.Add(this.DeviceTypeLabel);
            this.Controls.Add(this.DeviceName);
            this.Controls.Add(this.DeviceNameLabel);
            this.Controls.Add(this.DeviceImage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.DeviceImageUrl);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ControllerProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Controller Properties";
            this.Load += new System.EventHandler(this.ControllerProperties_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DeviceImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton Cancel;
        private WwtButton OK;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox DeviceImageUrl;
        private System.Windows.Forms.PictureBox DeviceImage;
        private System.Windows.Forms.TextBox DeviceName;
        private System.Windows.Forms.Label DeviceNameLabel;
        private System.Windows.Forms.Label DeviceTypeLabel;
        private System.Windows.Forms.TextBox DeviceType;
        private System.Windows.Forms.Label DeviceStatusLabel;
        private System.Windows.Forms.TextBox DeviceStatus;
        private WWTCheckbox SentStatus;
        private System.Windows.Forms.Timer timer1;
    }
}