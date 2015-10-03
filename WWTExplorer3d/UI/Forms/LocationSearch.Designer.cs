namespace TerraViewer
{
    partial class LocationSearch
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
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.resultsListLabel = new System.Windows.Forms.Label();
            this.resultsListbox = new System.Windows.Forms.ListBox();
            this.Cancel = new TerraViewer.WwtButton();
            this.OK = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.AcceptsReturn = true;
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.Location = new System.Drawing.Point(12, 27);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(388, 22);
            this.txtName.TabIndex = 8;
            this.txtName.Text = "<Type name here>";
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            this.txtName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyUp);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Place Name or Address";
            // 
            // resultsListLabel
            // 
            this.resultsListLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultsListLabel.ForeColor = System.Drawing.Color.White;
            this.resultsListLabel.Location = new System.Drawing.Point(9, 52);
            this.resultsListLabel.Name = "resultsListLabel";
            this.resultsListLabel.Size = new System.Drawing.Size(132, 16);
            this.resultsListLabel.TabIndex = 7;
            this.resultsListLabel.Text = "Results List (Please selecte the best Match)";
            // 
            // resultsListbox
            // 
            this.resultsListbox.AllowDrop = true;
            this.resultsListbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsListbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.resultsListbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.resultsListbox.ForeColor = System.Drawing.Color.White;
            this.resultsListbox.FormattingEnabled = true;
            this.resultsListbox.Location = new System.Drawing.Point(12, 71);
            this.resultsListbox.Name = "resultsListbox";
            this.resultsListbox.Size = new System.Drawing.Size(387, 208);
            this.resultsListbox.TabIndex = 17;
            this.resultsListbox.SelectedIndexChanged += new System.EventHandler(this.resultsListbox_SelectedIndexChanged);
            this.resultsListbox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.resultsListbox_MouseDoubleClick);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(331, 286);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(72, 33);
            this.Cancel.TabIndex = 16;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(258, 286);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(73, 33);
            this.OK.TabIndex = 15;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // LocationSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(411, 328);
            this.Controls.Add(this.resultsListbox);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.resultsListLabel);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LocationSearch";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Location Search";
            this.Load += new System.EventHandler(this.LocationSearch_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.LocationSearch_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private WwtButton Cancel;
        private WwtButton OK;
        private System.Windows.Forms.Label resultsListLabel;
        private System.Windows.Forms.ListBox resultsListbox;
    }
}