namespace TerraViewer.Callibration
{
    partial class EdgeProperties
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
            this.Ok = new TerraViewer.WwtButton();
            this.leftLabel = new System.Windows.Forms.Label();
            this.leftProjectorCombo = new TerraViewer.WwtCombo();
            this.label2 = new System.Windows.Forms.Label();
            this.rightProjectorCombo = new TerraViewer.WwtCombo();
            this.SuspendLayout();
            // 
            // Ok
            // 
            this.Ok.BackColor = System.Drawing.Color.Transparent;
            this.Ok.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Ok.ImageDisabled = null;
            this.Ok.ImageEnabled = null;
            this.Ok.Location = new System.Drawing.Point(180, 116);
            this.Ok.MaximumSize = new System.Drawing.Size(140, 33);
            this.Ok.Name = "Ok";
            this.Ok.Selected = false;
            this.Ok.Size = new System.Drawing.Size(83, 33);
            this.Ok.TabIndex = 0;
            this.Ok.Text = "Ok";
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // leftLabel
            // 
            this.leftLabel.AutoSize = true;
            this.leftLabel.Location = new System.Drawing.Point(12, 9);
            this.leftLabel.Name = "leftLabel";
            this.leftLabel.Size = new System.Drawing.Size(70, 13);
            this.leftLabel.TabIndex = 1;
            this.leftLabel.Text = "Left Projector";
            // 
            // leftProjectorCombo
            // 
            this.leftProjectorCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.leftProjectorCombo.DateTimeValue = new System.DateTime(2011, 1, 16, 22, 30, 33, 290);
            this.leftProjectorCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.leftProjectorCombo.FilterStyle = false;
            this.leftProjectorCombo.Location = new System.Drawing.Point(15, 26);
            this.leftProjectorCombo.Margin = new System.Windows.Forms.Padding(0);
            this.leftProjectorCombo.MasterTime = true;
            this.leftProjectorCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.leftProjectorCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.leftProjectorCombo.Name = "leftProjectorCombo";
            this.leftProjectorCombo.SelectedIndex = -1;
            this.leftProjectorCombo.SelectedItem = null;
            this.leftProjectorCombo.Size = new System.Drawing.Size(248, 33);
            this.leftProjectorCombo.State = TerraViewer.State.Rest;
            this.leftProjectorCombo.TabIndex = 2;
            this.leftProjectorCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.leftProjectorCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.leftProjectorCombo_SelectionChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Right Projector";
            // 
            // rightProjectorCombo
            // 
            this.rightProjectorCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.rightProjectorCombo.DateTimeValue = new System.DateTime(2011, 1, 16, 22, 30, 33, 290);
            this.rightProjectorCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.rightProjectorCombo.FilterStyle = false;
            this.rightProjectorCombo.Location = new System.Drawing.Point(15, 80);
            this.rightProjectorCombo.Margin = new System.Windows.Forms.Padding(0);
            this.rightProjectorCombo.MasterTime = true;
            this.rightProjectorCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.rightProjectorCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.rightProjectorCombo.Name = "rightProjectorCombo";
            this.rightProjectorCombo.SelectedIndex = -1;
            this.rightProjectorCombo.SelectedItem = null;
            this.rightProjectorCombo.Size = new System.Drawing.Size(248, 33);
            this.rightProjectorCombo.State = TerraViewer.State.Rest;
            this.rightProjectorCombo.TabIndex = 2;
            this.rightProjectorCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.rightProjectorCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.rightProjectorCombo_SelectionChanged);
            // 
            // EdgeProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(275, 162);
            this.Controls.Add(this.rightProjectorCombo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.leftProjectorCombo);
            this.Controls.Add(this.leftLabel);
            this.Controls.Add(this.Ok);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "EdgeProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Edge Properties";
            this.Load += new System.EventHandler(this.EdgeProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton Ok;
        private System.Windows.Forms.Label leftLabel;
        private WwtCombo leftProjectorCombo;
        private System.Windows.Forms.Label label2;
        private WwtCombo rightProjectorCombo;
    }
}