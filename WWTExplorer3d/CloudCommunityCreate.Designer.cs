namespace TerraViewer
{
    partial class CloudCommunityCreate
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
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblImage = new System.Windows.Forms.Label();
            this.fileDialog = new System.Windows.Forms.OpenFileDialog();
            this.txtImage = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblAccess = new System.Windows.Forms.Label();
            this.lblContent = new System.Windows.Forms.Label();
            this.cmbOrder = new TerraViewer.WwtCombo();
            this.cmbAccess = new TerraViewer.WwtCombo();
            this.btnBrowser = new TerraViewer.WwtButton();
            this.btnCreate = new TerraViewer.WwtButton();
            this.btnCancel = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(4, 7);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(48, 16);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.txtName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.ForeColor = System.Drawing.Color.Cyan;
            this.txtName.Location = new System.Drawing.Point(58, 4);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(312, 22);
            this.txtName.TabIndex = 1;
            // 
            // lblImage
            // 
            this.lblImage.AutoSize = true;
            this.lblImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblImage.Location = new System.Drawing.Point(4, 45);
            this.lblImage.Name = "lblImage";
            this.lblImage.Size = new System.Drawing.Size(49, 16);
            this.lblImage.TabIndex = 2;
            this.lblImage.Text = "Image:";
            // 
            // fileDialog
            // 
            this.fileDialog.FileName = "openFileDialog1";
            // 
            // txtImage
            // 
            this.txtImage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.txtImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtImage.ForeColor = System.Drawing.Color.Cyan;
            this.txtImage.Location = new System.Drawing.Point(59, 42);
            this.txtImage.Name = "txtImage";
            this.txtImage.Size = new System.Drawing.Size(275, 22);
            this.txtImage.TabIndex = 3;
            // 
            // txtDescription
            // 
            this.txtDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.txtDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescription.ForeColor = System.Drawing.Color.Cyan;
            this.txtDescription.Location = new System.Drawing.Point(7, 72);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(665, 278);
            this.txtDescription.TabIndex = 4;
            // 
            // lblAccess
            // 
            this.lblAccess.AutoSize = true;
            this.lblAccess.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccess.Location = new System.Drawing.Point(376, 7);
            this.lblAccess.Name = "lblAccess";
            this.lblAccess.Size = new System.Drawing.Size(96, 16);
            this.lblAccess.TabIndex = 7;
            this.lblAccess.Text = "Public Access:";
            // 
            // lblContent
            // 
            this.lblContent.AutoSize = true;
            this.lblContent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContent.Location = new System.Drawing.Point(376, 45);
            this.lblContent.Name = "lblContent";
            this.lblContent.Size = new System.Drawing.Size(63, 16);
            this.lblContent.TabIndex = 8;
            this.lblContent.Text = "Order by:";
            // 
            // cmbOrder
            // 
            this.cmbOrder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.cmbOrder.DateTimeValue = new System.DateTime(2010, 2, 9, 21, 11, 46, 413);
            this.cmbOrder.Filter = TerraViewer.Classification.Unfiltered;
            this.cmbOrder.FilterStyle = false;
            this.cmbOrder.Location = new System.Drawing.Point(475, 0);
            this.cmbOrder.Margin = new System.Windows.Forms.Padding(0);
            this.cmbOrder.MaximumSize = new System.Drawing.Size(248, 33);
            this.cmbOrder.MinimumSize = new System.Drawing.Size(35, 33);
            this.cmbOrder.Name = "cmbOrder";
            this.cmbOrder.SelectedIndex = -1;
            this.cmbOrder.SelectedItem = null;
            this.cmbOrder.Size = new System.Drawing.Size(133, 33);
            this.cmbOrder.State = TerraViewer.State.Rest;
            this.cmbOrder.TabIndex = 16;
            this.cmbOrder.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // cmbAccess
            // 
            this.cmbAccess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.cmbAccess.DateTimeValue = new System.DateTime(2010, 2, 9, 21, 11, 39, 548);
            this.cmbAccess.Filter = TerraViewer.Classification.Unfiltered;
            this.cmbAccess.FilterStyle = false;
            this.cmbAccess.Location = new System.Drawing.Point(474, 32);
            this.cmbAccess.Margin = new System.Windows.Forms.Padding(0);
            this.cmbAccess.MaximumSize = new System.Drawing.Size(248, 33);
            this.cmbAccess.MinimumSize = new System.Drawing.Size(35, 33);
            this.cmbAccess.Name = "cmbAccess";
            this.cmbAccess.SelectedIndex = -1;
            this.cmbAccess.SelectedItem = null;
            this.cmbAccess.Size = new System.Drawing.Size(134, 33);
            this.cmbAccess.State = TerraViewer.State.Rest;
            this.cmbAccess.TabIndex = 15;
            this.cmbAccess.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // btnBrowser
            // 
            this.btnBrowser.BackColor = System.Drawing.Color.Transparent;
            this.btnBrowser.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnBrowser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBrowser.ImageDisabled = null;
            this.btnBrowser.ImageEnabled = null;
            this.btnBrowser.Location = new System.Drawing.Point(334, 33);
            this.btnBrowser.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnBrowser.MaximumSize = new System.Drawing.Size(140, 33);
            this.btnBrowser.Name = "btnBrowser";
            this.btnBrowser.Selected = false;
            this.btnBrowser.Size = new System.Drawing.Size(36, 33);
            this.btnBrowser.TabIndex = 14;
            this.btnBrowser.Text = "...";
            this.btnBrowser.Click += new System.EventHandler(this.btnBrowser_Click);
            // 
            // btnCreate
            // 
            this.btnCreate.BackColor = System.Drawing.Color.Transparent;
            this.btnCreate.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnCreate.ImageDisabled = null;
            this.btnCreate.ImageEnabled = null;
            this.btnCreate.Location = new System.Drawing.Point(611, 0);
            this.btnCreate.MaximumSize = new System.Drawing.Size(140, 33);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Selected = false;
            this.btnCreate.Size = new System.Drawing.Size(65, 33);
            this.btnCreate.TabIndex = 13;
            this.btnCreate.Text = "Ok";
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnCancel.ImageDisabled = null;
            this.btnCancel.ImageEnabled = null;
            this.btnCancel.Location = new System.Drawing.Point(611, 33);
            this.btnCancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Selected = false;
            this.btnCancel.Size = new System.Drawing.Size(65, 33);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // CloudCommunityCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(677, 352);
            this.ControlBox = false;
            this.Controls.Add(this.cmbOrder);
            this.Controls.Add(this.cmbAccess);
            this.Controls.Add(this.btnBrowser);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblContent);
            this.Controls.Add(this.lblAccess);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.txtImage);
            this.Controls.Add(this.lblImage);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.ForeColor = System.Drawing.Color.Cyan;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CloudCommunityCreate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create New Community";
            this.Load += new System.EventHandler(this.CloudCommunityCreate_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblImage;
        private System.Windows.Forms.OpenFileDialog fileDialog;
        private System.Windows.Forms.TextBox txtImage;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblAccess;
        private System.Windows.Forms.Label lblContent;
        private WwtButton btnCancel;
        private WwtButton btnCreate;
        private WwtButton btnBrowser;
        private WwtCombo cmbAccess;
        private WwtCombo cmbOrder;
    }
}