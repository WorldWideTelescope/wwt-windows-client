namespace TerraViewer
{
    partial class LanguageSelect
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
            this.langCombo = new TerraViewer.WwtCombo();
            this.Cancel = new TerraViewer.WwtButton();
            this.OK = new TerraViewer.WwtButton();
            this.selectLangLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // langCombo
            // 
            this.langCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.langCombo.DateTimeValue = new System.DateTime(2008, 10, 6, 18, 49, 14, 758);
            this.langCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.langCombo.FilterStyle = false;
            this.langCombo.Location = new System.Drawing.Point(9, 31);
            this.langCombo.Margin = new System.Windows.Forms.Padding(0);
            this.langCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.langCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.langCombo.Name = "langCombo";
            this.langCombo.SelectedIndex = -1;
            this.langCombo.Size = new System.Drawing.Size(248, 33);
            this.langCombo.State = TerraViewer.State.Rest;
            this.langCombo.TabIndex = 1;
            this.langCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(181, 73);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(72, 33);
            this.Cancel.TabIndex = 3;
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
            this.OK.Location = new System.Drawing.Point(113, 73);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(73, 33);
            this.OK.TabIndex = 2;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // selectLangLabel
            // 
            this.selectLangLabel.AutoSize = true;
            this.selectLangLabel.ForeColor = System.Drawing.Color.White;
            this.selectLangLabel.Location = new System.Drawing.Point(6, 18);
            this.selectLangLabel.Name = "selectLangLabel";
            this.selectLangLabel.Size = new System.Drawing.Size(88, 13);
            this.selectLangLabel.TabIndex = 0;
            this.selectLangLabel.Text = "Select Language";
            // 
            // LanguageSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(265, 118);
            this.ControlBox = false;
            this.Controls.Add(this.selectLangLabel);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.langCombo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LanguageSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LanguageSelect";
            this.Load += new System.EventHandler(this.LanguageSelect_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtCombo langCombo;
        private WwtButton Cancel;
        private WwtButton OK;
        private System.Windows.Forms.Label selectLangLabel;
    }
}