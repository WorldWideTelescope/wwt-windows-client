namespace TerraViewer
{
    partial class NewBinding
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
            this.StatusText = new System.Windows.Forms.Label();
            this.controlTypeLabel = new System.Windows.Forms.Label();
            this.Ok = new TerraViewer.WwtButton();
            this.ControlTypeCombo = new TerraViewer.WwtCombo();
            this.Cancel = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // StatusText
            // 
            this.StatusText.AutoSize = true;
            this.StatusText.Location = new System.Drawing.Point(13, 13);
            this.StatusText.Name = "StatusText";
            this.StatusText.Size = new System.Drawing.Size(264, 13);
            this.StatusText.TabIndex = 1;
            this.StatusText.Text = "Listening for unmapped controls: Activate Control now.";
            // 
            // controlTypeLabel
            // 
            this.controlTypeLabel.AutoSize = true;
            this.controlTypeLabel.Location = new System.Drawing.Point(13, 50);
            this.controlTypeLabel.Name = "controlTypeLabel";
            this.controlTypeLabel.Size = new System.Drawing.Size(67, 13);
            this.controlTypeLabel.TabIndex = 3;
            this.controlTypeLabel.Text = "Control Type";
            this.controlTypeLabel.Visible = false;
            // 
            // Ok
            // 
            this.Ok.BackColor = System.Drawing.Color.Transparent;
            this.Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Ok.ImageDisabled = null;
            this.Ok.ImageEnabled = null;
            this.Ok.Location = new System.Drawing.Point(102, 99);
            this.Ok.MaximumSize = new System.Drawing.Size(140, 33);
            this.Ok.Name = "Ok";
            this.Ok.Selected = false;
            this.Ok.Size = new System.Drawing.Size(83, 33);
            this.Ok.TabIndex = 4;
            this.Ok.Text = "OK";
            this.Ok.Visible = false;
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // ControlTypeCombo
            // 
            this.ControlTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ControlTypeCombo.DateTimeValue = new System.DateTime(2012, 7, 9, 22, 31, 4, 155);
            this.ControlTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.ControlTypeCombo.FilterStyle = false;
            this.ControlTypeCombo.Location = new System.Drawing.Point(16, 63);
            this.ControlTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.ControlTypeCombo.MasterTime = true;
            this.ControlTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.ControlTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.ControlTypeCombo.Name = "ControlTypeCombo";
            this.ControlTypeCombo.SelectedIndex = -1;
            this.ControlTypeCombo.SelectedItem = null;
            this.ControlTypeCombo.Size = new System.Drawing.Size(248, 33);
            this.ControlTypeCombo.State = TerraViewer.State.Rest;
            this.ControlTypeCombo.TabIndex = 2;
            this.ControlTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.ControlTypeCombo.Visible = false;
            // 
            // Cancel
            // 
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(182, 99);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(84, 33);
            this.Cancel.TabIndex = 0;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // NewBinding
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(276, 144);
            this.Controls.Add(this.Ok);
            this.Controls.Add(this.controlTypeLabel);
            this.Controls.Add(this.ControlTypeCombo);
            this.Controls.Add(this.StatusText);
            this.Controls.Add(this.Cancel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewBinding";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Control";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NewBinding_FormClosed);
            this.Load += new System.EventHandler(this.NewBinding_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton Cancel;
        private System.Windows.Forms.Label StatusText;
        private WwtCombo ControlTypeCombo;
        private System.Windows.Forms.Label controlTypeLabel;
        private WwtButton Ok;

    }
}