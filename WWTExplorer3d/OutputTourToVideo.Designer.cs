namespace TerraViewer
{
    partial class OutputTourToVideo
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
            this.WaitForDownloads = new TerraViewer.WWTCheckbox();
            this.Close = new TerraViewer.WwtButton();
            this.PathEdit = new System.Windows.Forms.TextBox();
            this.PathLabel = new System.Windows.Forms.Label();
            this.Browse = new TerraViewer.WwtButton();
            this.label1 = new System.Windows.Forms.Label();
            this.widthEdit = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.heightEdit = new System.Windows.Forms.TextBox();
            this.OutputFormatCombo = new TerraViewer.WwtCombo();
            this.OutputFormatLabel = new System.Windows.Forms.Label();
            this.fpsLabel = new System.Windows.Forms.Label();
            this.fpsEdit = new System.Windows.Forms.TextBox();
            this.Render = new TerraViewer.WwtButton();
            this.label3 = new System.Windows.Forms.Label();
            this.runTimeEdit = new System.Windows.Forms.TextBox();
            this.totalTimeLabel = new System.Windows.Forms.Label();
            this.totalFramesEdit = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.startFrameEdit = new System.Windows.Forms.TextBox();
            this.DomeMaster = new TerraViewer.WWTCheckbox();
            this.SuspendLayout();
            // 
            // WaitForDownloads
            // 
            this.WaitForDownloads.Checked = false;
            this.WaitForDownloads.Location = new System.Drawing.Point(12, 124);
            this.WaitForDownloads.Name = "WaitForDownloads";
            this.WaitForDownloads.Size = new System.Drawing.Size(160, 25);
            this.WaitForDownloads.TabIndex = 17;
            this.WaitForDownloads.Text = "Wait for all downloads";
            // 
            // Close
            // 
            this.Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Close.BackColor = System.Drawing.Color.Transparent;
            this.Close.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Close.ImageDisabled = null;
            this.Close.ImageEnabled = null;
            this.Close.Location = new System.Drawing.Point(458, 176);
            this.Close.MaximumSize = new System.Drawing.Size(140, 33);
            this.Close.Name = "Close";
            this.Close.Selected = false;
            this.Close.Size = new System.Drawing.Size(94, 33);
            this.Close.TabIndex = 19;
            this.Close.Text = "Close";
            this.Close.Click += new System.EventHandler(this.Close_Click);
            // 
            // PathEdit
            // 
            this.PathEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.PathEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PathEdit.ForeColor = System.Drawing.Color.White;
            this.PathEdit.Location = new System.Drawing.Point(15, 25);
            this.PathEdit.Name = "PathEdit";
            this.PathEdit.Size = new System.Drawing.Size(441, 20);
            this.PathEdit.TabIndex = 1;
            // 
            // PathLabel
            // 
            this.PathLabel.AutoSize = true;
            this.PathLabel.Location = new System.Drawing.Point(12, 9);
            this.PathLabel.Name = "PathLabel";
            this.PathLabel.Size = new System.Drawing.Size(301, 13);
            this.PathLabel.TabIndex = 0;
            this.PathLabel.Text = "Filename Out (Use {NNNN} notation for auto frame numbering)";
            // 
            // Browse
            // 
            this.Browse.BackColor = System.Drawing.Color.Transparent;
            this.Browse.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Browse.ImageDisabled = null;
            this.Browse.ImageEnabled = null;
            this.Browse.Location = new System.Drawing.Point(464, 18);
            this.Browse.MaximumSize = new System.Drawing.Size(140, 33);
            this.Browse.Name = "Browse";
            this.Browse.Selected = false;
            this.Browse.Size = new System.Drawing.Size(88, 33);
            this.Browse.TabIndex = 2;
            this.Browse.Text = "Browse";
            this.Browse.Click += new System.EventHandler(this.Browse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(272, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Width";
            // 
            // widthEdit
            // 
            this.widthEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.widthEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.widthEdit.ForeColor = System.Drawing.Color.White;
            this.widthEdit.Location = new System.Drawing.Point(275, 73);
            this.widthEdit.Name = "widthEdit";
            this.widthEdit.ReadOnly = true;
            this.widthEdit.Size = new System.Drawing.Size(80, 20);
            this.widthEdit.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(367, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Height";
            // 
            // heightEdit
            // 
            this.heightEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.heightEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.heightEdit.ForeColor = System.Drawing.Color.White;
            this.heightEdit.Location = new System.Drawing.Point(370, 73);
            this.heightEdit.Name = "heightEdit";
            this.heightEdit.ReadOnly = true;
            this.heightEdit.Size = new System.Drawing.Size(80, 20);
            this.heightEdit.TabIndex = 8;
            // 
            // OutputFormatCombo
            // 
            this.OutputFormatCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.OutputFormatCombo.DateTimeValue = new System.DateTime(2011, 1, 28, 21, 30, 32, 365);
            this.OutputFormatCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.OutputFormatCombo.FilterStyle = false;
            this.OutputFormatCombo.Location = new System.Drawing.Point(15, 66);
            this.OutputFormatCombo.Margin = new System.Windows.Forms.Padding(0);
            this.OutputFormatCombo.MasterTime = true;
            this.OutputFormatCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.OutputFormatCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.OutputFormatCombo.Name = "OutputFormatCombo";
            this.OutputFormatCombo.SelectedIndex = -1;
            this.OutputFormatCombo.SelectedItem = null;
            this.OutputFormatCombo.Size = new System.Drawing.Size(248, 33);
            this.OutputFormatCombo.State = TerraViewer.State.Rest;
            this.OutputFormatCombo.TabIndex = 4;
            this.OutputFormatCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.OutputFormatCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.OutputFormatCombo_SelectionChanged);
            // 
            // OutputFormatLabel
            // 
            this.OutputFormatLabel.AutoSize = true;
            this.OutputFormatLabel.Location = new System.Drawing.Point(12, 53);
            this.OutputFormatLabel.Name = "OutputFormatLabel";
            this.OutputFormatLabel.Size = new System.Drawing.Size(74, 13);
            this.OutputFormatLabel.TabIndex = 3;
            this.OutputFormatLabel.Text = "Output Format";
            // 
            // fpsLabel
            // 
            this.fpsLabel.AutoSize = true;
            this.fpsLabel.Location = new System.Drawing.Point(462, 57);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(27, 13);
            this.fpsLabel.TabIndex = 9;
            this.fpsLabel.Text = "FPS";
            // 
            // fpsEdit
            // 
            this.fpsEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.fpsEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fpsEdit.ForeColor = System.Drawing.Color.White;
            this.fpsEdit.Location = new System.Drawing.Point(465, 73);
            this.fpsEdit.Name = "fpsEdit";
            this.fpsEdit.ReadOnly = true;
            this.fpsEdit.Size = new System.Drawing.Size(50, 20);
            this.fpsEdit.TabIndex = 10;
            this.fpsEdit.TextChanged += new System.EventHandler(this.fpsEdit_TextChanged);
            // 
            // Render
            // 
            this.Render.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Render.BackColor = System.Drawing.Color.Transparent;
            this.Render.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Render.ImageDisabled = null;
            this.Render.ImageEnabled = null;
            this.Render.Location = new System.Drawing.Point(362, 176);
            this.Render.MaximumSize = new System.Drawing.Size(140, 33);
            this.Render.Name = "Render";
            this.Render.Selected = false;
            this.Render.Size = new System.Drawing.Size(94, 33);
            this.Render.TabIndex = 18;
            this.Render.Text = "Render";
            this.Render.Click += new System.EventHandler(this.Render_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(272, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Run Time";
            // 
            // runTimeEdit
            // 
            this.runTimeEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.runTimeEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.runTimeEdit.ForeColor = System.Drawing.Color.White;
            this.runTimeEdit.Location = new System.Drawing.Point(275, 122);
            this.runTimeEdit.Name = "runTimeEdit";
            this.runTimeEdit.ReadOnly = true;
            this.runTimeEdit.Size = new System.Drawing.Size(80, 20);
            this.runTimeEdit.TabIndex = 12;
            this.runTimeEdit.Text = "1:00";
            // 
            // totalTimeLabel
            // 
            this.totalTimeLabel.AutoSize = true;
            this.totalTimeLabel.Location = new System.Drawing.Point(367, 106);
            this.totalTimeLabel.Name = "totalTimeLabel";
            this.totalTimeLabel.Size = new System.Drawing.Size(68, 13);
            this.totalTimeLabel.TabIndex = 13;
            this.totalTimeLabel.Text = "Total Frames";
            // 
            // totalFramesEdit
            // 
            this.totalFramesEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.totalFramesEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.totalFramesEdit.ForeColor = System.Drawing.Color.White;
            this.totalFramesEdit.Location = new System.Drawing.Point(370, 122);
            this.totalFramesEdit.Name = "totalFramesEdit";
            this.totalFramesEdit.ReadOnly = true;
            this.totalFramesEdit.Size = new System.Drawing.Size(80, 20);
            this.totalFramesEdit.TabIndex = 14;
            this.totalFramesEdit.Text = "0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(462, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Start Frame";
            // 
            // startFrameEdit
            // 
            this.startFrameEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.startFrameEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.startFrameEdit.ForeColor = System.Drawing.Color.White;
            this.startFrameEdit.Location = new System.Drawing.Point(465, 122);
            this.startFrameEdit.Name = "startFrameEdit";
            this.startFrameEdit.Size = new System.Drawing.Size(80, 20);
            this.startFrameEdit.TabIndex = 16;
            this.startFrameEdit.Text = "0";
            // 
            // DomeMaster
            // 
            this.DomeMaster.Checked = false;
            this.DomeMaster.Location = new System.Drawing.Point(12, 102);
            this.DomeMaster.Name = "DomeMaster";
            this.DomeMaster.Size = new System.Drawing.Size(190, 25);
            this.DomeMaster.TabIndex = 20;
            this.DomeMaster.Text = "Dome Master";
            // 
            // OutputTourToVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(558, 221);
            this.ControlBox = false;
            this.Controls.Add(this.DomeMaster);
            this.Controls.Add(this.OutputFormatCombo);
            this.Controls.Add(this.Browse);
            this.Controls.Add(this.WaitForDownloads);
            this.Controls.Add(this.Render);
            this.Controls.Add(this.Close);
            this.Controls.Add(this.startFrameEdit);
            this.Controls.Add(this.totalFramesEdit);
            this.Controls.Add(this.runTimeEdit);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.heightEdit);
            this.Controls.Add(this.totalTimeLabel);
            this.Controls.Add(this.fpsEdit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.widthEdit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OutputFormatLabel);
            this.Controls.Add(this.fpsLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PathEdit);
            this.Controls.Add(this.PathLabel);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OutputTourToVideo";
            this.Text = "Render Tour to Video";
            this.Load += new System.EventHandler(this.OutputTourToVideo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WWTCheckbox WaitForDownloads;
        private WwtButton Close;
        private System.Windows.Forms.TextBox PathEdit;
        private System.Windows.Forms.Label PathLabel;
        private WwtButton Browse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox widthEdit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox heightEdit;
        private WwtCombo OutputFormatCombo;
        private System.Windows.Forms.Label OutputFormatLabel;
        private System.Windows.Forms.Label fpsLabel;
        private System.Windows.Forms.TextBox fpsEdit;
        private WwtButton Render;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox runTimeEdit;
        private System.Windows.Forms.Label totalTimeLabel;
        private System.Windows.Forms.TextBox totalFramesEdit;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox startFrameEdit;
        private WWTCheckbox DomeMaster;
    }
}