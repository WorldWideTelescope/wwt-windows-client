namespace TerraViewer
{
    partial class WizardShell
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
            this.StepTitle = new System.Windows.Forms.Label();
            this.Contents = new System.Windows.Forms.Panel();
            this.Finish = new TerraViewer.WwtButton();
            this.Next = new TerraViewer.WwtButton();
            this.Back = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // StepTitle
            // 
            this.StepTitle.AutoSize = true;
            this.StepTitle.BackColor = System.Drawing.Color.Transparent;
            this.StepTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StepTitle.Location = new System.Drawing.Point(17, 8);
            this.StepTitle.Name = "StepTitle";
            this.StepTitle.Size = new System.Drawing.Size(108, 24);
            this.StepTitle.TabIndex = 1;
            this.StepTitle.Text = "Title of Step";
            // 
            // Contents
            // 
            this.Contents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Contents.Location = new System.Drawing.Point(0, 40);
            this.Contents.Name = "Contents";
            this.Contents.Size = new System.Drawing.Size(690, 230);
            this.Contents.TabIndex = 2;
            // 
            // Finish
            // 
            this.Finish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Finish.BackColor = System.Drawing.Color.Transparent;
            this.Finish.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Finish.ImageDisabled = null;
            this.Finish.ImageEnabled = null;
            this.Finish.Location = new System.Drawing.Point(595, 277);
            this.Finish.MaximumSize = new System.Drawing.Size(140, 33);
            this.Finish.Name = "Finish";
            this.Finish.Selected = false;
            this.Finish.Size = new System.Drawing.Size(83, 33);
            this.Finish.TabIndex = 0;
            this.Finish.Text = "Finish";
            this.Finish.Click += new System.EventHandler(this.Finish_Click);
            // 
            // Next
            // 
            this.Next.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Next.BackColor = System.Drawing.Color.Transparent;
            this.Next.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Next.ImageDisabled = null;
            this.Next.ImageEnabled = null;
            this.Next.Location = new System.Drawing.Point(506, 277);
            this.Next.MaximumSize = new System.Drawing.Size(140, 33);
            this.Next.Name = "Next";
            this.Next.Selected = false;
            this.Next.Size = new System.Drawing.Size(83, 33);
            this.Next.TabIndex = 0;
            this.Next.Text = "Next";
            this.Next.Click += new System.EventHandler(this.Next_Click);
            // 
            // Back
            // 
            this.Back.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Back.BackColor = System.Drawing.Color.Transparent;
            this.Back.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Back.ImageDisabled = null;
            this.Back.ImageEnabled = null;
            this.Back.Location = new System.Drawing.Point(417, 277);
            this.Back.MaximumSize = new System.Drawing.Size(140, 33);
            this.Back.Name = "Back";
            this.Back.Selected = false;
            this.Back.Size = new System.Drawing.Size(83, 33);
            this.Back.TabIndex = 0;
            this.Back.Text = "Back";
            this.Back.Click += new System.EventHandler(this.Back_Click);
            // 
            // WizardShell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(690, 322);
            this.Controls.Add(this.Contents);
            this.Controls.Add(this.StepTitle);
            this.Controls.Add(this.Finish);
            this.Controls.Add(this.Next);
            this.Controls.Add(this.Back);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WizardShell";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wizard Name";
            this.Load += new System.EventHandler(this.WizardShell_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton Back;
        private WwtButton Next;
        private WwtButton Finish;
        private System.Windows.Forms.Label StepTitle;
        private System.Windows.Forms.Panel Contents;
    }
}