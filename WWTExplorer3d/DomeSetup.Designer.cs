namespace TerraViewer
{
    partial class DomeSetup
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
            this.label1 = new System.Windows.Forms.Label();
            this.tiltEdit = new System.Windows.Forms.TextBox();
            this.domeTypeLabel = new System.Windows.Forms.Label();
            this.customWarpFilename = new System.Windows.Forms.TextBox();
            this.CustomFilenameLabel = new System.Windows.Forms.Label();
            this.browseButton = new TerraViewer.WwtButton();
            this.domeTypeCombo = new TerraViewer.WwtCombo();
            this.largeTextures = new TerraViewer.WWTCheckbox();
            this.domeTilt = new TerraViewer.WwtTrackBar();
            this.OK = new TerraViewer.WwtButton();
            this.flatScreenWarp = new TerraViewer.WWTCheckbox();
            this.DomeAlt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.DomeAz = new System.Windows.Forms.TextBox();
            this.DomeNorth = new TerraViewer.WWTCheckbox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 173);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Dome Tilt";
            // 
            // tiltEdit
            // 
            this.tiltEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.tiltEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tiltEdit.Location = new System.Drawing.Point(94, 189);
            this.tiltEdit.Name = "tiltEdit";
            this.tiltEdit.Size = new System.Drawing.Size(43, 20);
            this.tiltEdit.TabIndex = 3;
            this.tiltEdit.TextChanged += new System.EventHandler(this.tiltEdit_TextChanged);
            // 
            // domeTypeLabel
            // 
            this.domeTypeLabel.AutoSize = true;
            this.domeTypeLabel.Location = new System.Drawing.Point(9, 9);
            this.domeTypeLabel.Name = "domeTypeLabel";
            this.domeTypeLabel.Size = new System.Drawing.Size(62, 13);
            this.domeTypeLabel.TabIndex = 2;
            this.domeTypeLabel.Text = "Dome Type";
            // 
            // customWarpFilename
            // 
            this.customWarpFilename.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.customWarpFilename.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.customWarpFilename.Enabled = false;
            this.customWarpFilename.Location = new System.Drawing.Point(9, 87);
            this.customWarpFilename.Name = "customWarpFilename";
            this.customWarpFilename.Size = new System.Drawing.Size(248, 20);
            this.customWarpFilename.TabIndex = 3;
            this.customWarpFilename.TextChanged += new System.EventHandler(this.tiltEdit_TextChanged);
            // 
            // CustomFilenameLabel
            // 
            this.CustomFilenameLabel.AutoSize = true;
            this.CustomFilenameLabel.Location = new System.Drawing.Point(6, 65);
            this.CustomFilenameLabel.Name = "CustomFilenameLabel";
            this.CustomFilenameLabel.Size = new System.Drawing.Size(116, 13);
            this.CustomFilenameLabel.TabIndex = 2;
            this.CustomFilenameLabel.Text = "Custom Warp Filename";
            // 
            // browseButton
            // 
            this.browseButton.BackColor = System.Drawing.Color.Transparent;
            this.browseButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.browseButton.Enabled = false;
            this.browseButton.ImageDisabled = null;
            this.browseButton.ImageEnabled = null;
            this.browseButton.Location = new System.Drawing.Point(263, 80);
            this.browseButton.MaximumSize = new System.Drawing.Size(140, 33);
            this.browseButton.Name = "browseButton";
            this.browseButton.Selected = false;
            this.browseButton.Size = new System.Drawing.Size(82, 33);
            this.browseButton.TabIndex = 6;
            this.browseButton.Text = "Browse";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // domeTypeCombo
            // 
            this.domeTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.domeTypeCombo.DateTimeValue = new System.DateTime(2010, 4, 14, 12, 33, 47, 676);
            this.domeTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.domeTypeCombo.FilterStyle = false;
            this.domeTypeCombo.Location = new System.Drawing.Point(9, 29);
            this.domeTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.domeTypeCombo.MasterTime = true;
            this.domeTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.domeTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.domeTypeCombo.Name = "domeTypeCombo";
            this.domeTypeCombo.SelectedIndex = -1;
            this.domeTypeCombo.SelectedItem = null;
            this.domeTypeCombo.Size = new System.Drawing.Size(248, 33);
            this.domeTypeCombo.State = TerraViewer.State.Rest;
            this.domeTypeCombo.TabIndex = 5;
            this.domeTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.domeTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.domeTypeCombo_SelectionChanged);
            // 
            // largeTextures
            // 
            this.largeTextures.Checked = false;
            this.largeTextures.Location = new System.Drawing.Point(8, 134);
            this.largeTextures.Name = "largeTextures";
            this.largeTextures.Size = new System.Drawing.Size(130, 25);
            this.largeTextures.TabIndex = 4;
            this.largeTextures.Text = " Large Textures";
            this.largeTextures.CheckedChanged += new System.EventHandler(this.largeTextures_CheckedChanged);
            // 
            // domeTilt
            // 
            this.domeTilt.BackColor = System.Drawing.Color.Transparent;
            this.domeTilt.Location = new System.Drawing.Point(8, 189);
            this.domeTilt.Max = 180;
            this.domeTilt.Name = "domeTilt";
            this.domeTilt.Size = new System.Drawing.Size(80, 20);
            this.domeTilt.TabIndex = 1;
            this.domeTilt.Value = 0;
            this.domeTilt.ValueChanged += new System.EventHandler(this.domeTilt_ValueChanged);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.None;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(260, 221);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(78, 33);
            this.OK.TabIndex = 0;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // flatScreenWarp
            // 
            this.flatScreenWarp.Checked = false;
            this.flatScreenWarp.Location = new System.Drawing.Point(8, 113);
            this.flatScreenWarp.Name = "flatScreenWarp";
            this.flatScreenWarp.Size = new System.Drawing.Size(159, 25);
            this.flatScreenWarp.TabIndex = 4;
            this.flatScreenWarp.Text = "Warp from flat screen";
            this.flatScreenWarp.CheckedChanged += new System.EventHandler(this.flatScreenWarp_CheckedChanged);
            // 
            // DomeAlt
            // 
            this.DomeAlt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.DomeAlt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DomeAlt.Location = new System.Drawing.Point(232, 189);
            this.DomeAlt.Name = "DomeAlt";
            this.DomeAlt.Size = new System.Drawing.Size(43, 20);
            this.DomeAlt.TabIndex = 3;
            this.DomeAlt.TextChanged += new System.EventHandler(this.DomeAlt_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(229, 173);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Dome Alt";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(288, 173);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Dome Az";
            // 
            // DomeAz
            // 
            this.DomeAz.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.DomeAz.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DomeAz.Location = new System.Drawing.Point(291, 189);
            this.DomeAz.Name = "DomeAz";
            this.DomeAz.Size = new System.Drawing.Size(43, 20);
            this.DomeAz.TabIndex = 3;
            this.DomeAz.TextChanged += new System.EventHandler(this.DomeAz_TextChanged);
            // 
            // DomeNorth
            // 
            this.DomeNorth.Checked = false;
            this.DomeNorth.Location = new System.Drawing.Point(8, 221);
            this.DomeNorth.Name = "DomeNorth";
            this.DomeNorth.Size = new System.Drawing.Size(165, 25);
            this.DomeNorth.TabIndex = 7;
            this.DomeNorth.Text = "Face North in Sky Mode";
            this.DomeNorth.CheckedChanged += new System.EventHandler(this.DomeNorth_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(229, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Scriptable Parameters";
            // 
            // DomeSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(351, 266);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.DomeNorth);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.domeTypeCombo);
            this.Controls.Add(this.flatScreenWarp);
            this.Controls.Add(this.largeTextures);
            this.Controls.Add(this.customWarpFilename);
            this.Controls.Add(this.DomeAz);
            this.Controls.Add(this.DomeAlt);
            this.Controls.Add(this.tiltEdit);
            this.Controls.Add(this.CustomFilenameLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.domeTypeLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.domeTilt);
            this.Controls.Add(this.OK);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DomeSetup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Dome Setup";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DomeSetup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton OK;
        private WwtTrackBar domeTilt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tiltEdit;
        private WWTCheckbox largeTextures;
        private WwtCombo domeTypeCombo;
        private System.Windows.Forms.Label domeTypeLabel;
        private System.Windows.Forms.TextBox customWarpFilename;
        private System.Windows.Forms.Label CustomFilenameLabel;
        private WwtButton browseButton;
        private WWTCheckbox flatScreenWarp;
        private System.Windows.Forms.TextBox DomeAlt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox DomeAz;
        private WWTCheckbox DomeNorth;
        private System.Windows.Forms.Label label4;
    }
}