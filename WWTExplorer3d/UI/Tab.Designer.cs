namespace TerraViewer
{
    partial class Tab
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TitleLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TitleLabel
            // 
            this.TitleLabel.BackColor = System.Drawing.Color.Transparent;
            this.TitleLabel.ForeColor = System.Drawing.Color.White;
            this.TitleLabel.Location = new System.Drawing.Point(1, 10);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(96, 13);
            this.TitleLabel.TabIndex = 0;
            this.TitleLabel.Text = "label1";
            this.TitleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.TitleLabel.MouseLeave += new System.EventHandler(this.Tab_MouseLeave);
            this.TitleLabel.Click += new System.EventHandler(this.TitleLabel_Click);
            this.TitleLabel.MouseEnter += new System.EventHandler(this.Tab_MouseEnter);
            // 
            // Tab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::TerraViewer.Properties.Resources.tabPlain;
            this.Controls.Add(this.TitleLabel);
            this.MaximumSize = new System.Drawing.Size(100, 34);
            this.MinimumSize = new System.Drawing.Size(100, 34);
            this.Name = "Tab";
            this.Size = new System.Drawing.Size(100, 34);
            this.Load += new System.EventHandler(this.Tab_Load);
            this.MouseLeave += new System.EventHandler(this.Tab_MouseLeave);
            this.MouseEnter += new System.EventHandler(this.Tab_MouseEnter);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label TitleLabel;
    }
}
