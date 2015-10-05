namespace TerraViewer
{
    partial class EulaReader
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EulaReader));
            this.eulaText = new System.Windows.Forms.RichTextBox();
            this.Accept = new TerraViewer.WwtButton();
            this.Decline = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // eulaText
            // 
            this.eulaText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eulaText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.eulaText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.eulaText.ForeColor = System.Drawing.Color.White;
            this.eulaText.Location = new System.Drawing.Point(0, -1);
            this.eulaText.Name = "eulaText";
            this.eulaText.Size = new System.Drawing.Size(805, 378);
            this.eulaText.TabIndex = 0;
            this.eulaText.Text = "";
            this.eulaText.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.eulaText_LinkClicked);
            // 
            // Accept
            // 
            this.Accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Accept.BackColor = System.Drawing.Color.Transparent;
            this.Accept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Accept.ImageDisabled = null;
            this.Accept.ImageEnabled = null;
            this.Accept.Location = new System.Drawing.Point(566, 384);
            this.Accept.MaximumSize = new System.Drawing.Size(140, 33);
            this.Accept.Name = "Accept";
            this.Accept.Selected = false;
            this.Accept.Size = new System.Drawing.Size(110, 33);
            this.Accept.TabIndex = 1;
            this.Accept.Text = "I accept";
            this.Accept.Click += new System.EventHandler(this.Accept_Click);
            // 
            // Decline
            // 
            this.Decline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Decline.BackColor = System.Drawing.Color.Transparent;
            this.Decline.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Decline.ImageDisabled = null;
            this.Decline.ImageEnabled = null;
            this.Decline.Location = new System.Drawing.Point(682, 384);
            this.Decline.MaximumSize = new System.Drawing.Size(140, 33);
            this.Decline.Name = "Decline";
            this.Decline.Selected = false;
            this.Decline.Size = new System.Drawing.Size(110, 33);
            this.Decline.TabIndex = 1;
            this.Decline.Text = "I Decline";
            this.Decline.Click += new System.EventHandler(this.Decline_Click);
            // 
            // EulaReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(804, 427);
            this.Controls.Add(this.Decline);
            this.Controls.Add(this.Accept);
            this.Controls.Add(this.eulaText);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EulaReader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Terms of Use";
            this.Load += new System.EventHandler(this.EulaReader_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox eulaText;
        private WwtButton Accept;
        private WwtButton Decline;
    }
}