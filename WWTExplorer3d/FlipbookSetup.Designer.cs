namespace TerraViewer
{
    partial class FlipbookSetup
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
            this.startFrame = new System.Windows.Forms.TextBox();
            this.startFrameLabel = new System.Windows.Forms.Label();
            this.endFrame = new System.Windows.Forms.TextBox();
            this.endFrameLabel = new System.Windows.Forms.Label();
            this.gridXlabel = new System.Windows.Forms.Label();
            this.gridXText = new System.Windows.Forms.TextBox();
            this.gridYLabel = new System.Windows.Forms.Label();
            this.gridYText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.loopTypeLabel = new System.Windows.Forms.Label();
            this.frameSequenceLabel = new System.Windows.Forms.Label();
            this.frameSequenceText = new System.Windows.Forms.TextBox();
            this.flipbookStyle = new TerraViewer.WwtCombo();
            this.Cancel = new TerraViewer.WwtButton();
            this.OK = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // startFrame
            // 
            this.startFrame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.startFrame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.startFrame.ForeColor = System.Drawing.Color.White;
            this.startFrame.Location = new System.Drawing.Point(13, 31);
            this.startFrame.Name = "startFrame";
            this.startFrame.Size = new System.Drawing.Size(67, 20);
            this.startFrame.TabIndex = 4;
            this.startFrame.Text = "0";
            // 
            // startFrameLabel
            // 
            this.startFrameLabel.AutoSize = true;
            this.startFrameLabel.Location = new System.Drawing.Point(10, 15);
            this.startFrameLabel.Name = "startFrameLabel";
            this.startFrameLabel.Size = new System.Drawing.Size(58, 13);
            this.startFrameLabel.TabIndex = 3;
            this.startFrameLabel.Text = "StartFrame";
            // 
            // endFrame
            // 
            this.endFrame.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.endFrame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.endFrame.ForeColor = System.Drawing.Color.White;
            this.endFrame.Location = new System.Drawing.Point(13, 81);
            this.endFrame.Name = "endFrame";
            this.endFrame.Size = new System.Drawing.Size(67, 20);
            this.endFrame.TabIndex = 4;
            this.endFrame.Text = "64";
            // 
            // endFrameLabel
            // 
            this.endFrameLabel.AutoSize = true;
            this.endFrameLabel.Location = new System.Drawing.Point(10, 65);
            this.endFrameLabel.Name = "endFrameLabel";
            this.endFrameLabel.Size = new System.Drawing.Size(67, 13);
            this.endFrameLabel.TabIndex = 3;
            this.endFrameLabel.Text = "Frame Count";
            this.endFrameLabel.Click += new System.EventHandler(this.endFrameLabel_Click);
            // 
            // gridXlabel
            // 
            this.gridXlabel.AutoSize = true;
            this.gridXlabel.Location = new System.Drawing.Point(115, 15);
            this.gridXlabel.Name = "gridXlabel";
            this.gridXlabel.Size = new System.Drawing.Size(39, 13);
            this.gridXlabel.TabIndex = 3;
            this.gridXlabel.Text = "Across";
            // 
            // gridXText
            // 
            this.gridXText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.gridXText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridXText.ForeColor = System.Drawing.Color.White;
            this.gridXText.Location = new System.Drawing.Point(117, 31);
            this.gridXText.Name = "gridXText";
            this.gridXText.Size = new System.Drawing.Size(37, 20);
            this.gridXText.TabIndex = 4;
            this.gridXText.Text = "8";
            // 
            // gridYLabel
            // 
            this.gridYLabel.AutoSize = true;
            this.gridYLabel.Location = new System.Drawing.Point(187, 15);
            this.gridYLabel.Name = "gridYLabel";
            this.gridYLabel.Size = new System.Drawing.Size(35, 13);
            this.gridYLabel.TabIndex = 3;
            this.gridYLabel.Text = "Down";
            // 
            // gridYText
            // 
            this.gridYText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.gridYText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridYText.ForeColor = System.Drawing.Color.White;
            this.gridYText.Location = new System.Drawing.Point(189, 31);
            this.gridYText.Name = "gridYText";
            this.gridYText.Size = new System.Drawing.Size(33, 20);
            this.gridYText.TabIndex = 4;
            this.gridYText.Text = "8";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(163, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "X";
            // 
            // loopTypeLabel
            // 
            this.loopTypeLabel.AutoSize = true;
            this.loopTypeLabel.Location = new System.Drawing.Point(14, 117);
            this.loopTypeLabel.Name = "loopTypeLabel";
            this.loopTypeLabel.Size = new System.Drawing.Size(58, 13);
            this.loopTypeLabel.TabIndex = 3;
            this.loopTypeLabel.Text = "Loop Type";
            // 
            // frameSequenceLabel
            // 
            this.frameSequenceLabel.AutoSize = true;
            this.frameSequenceLabel.Location = new System.Drawing.Point(14, 171);
            this.frameSequenceLabel.Name = "frameSequenceLabel";
            this.frameSequenceLabel.Size = new System.Drawing.Size(178, 13);
            this.frameSequenceLabel.TabIndex = 3;
            this.frameSequenceLabel.Text = "Frame Sequence (Comma Delimited)";
            // 
            // frameSequenceText
            // 
            this.frameSequenceText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.frameSequenceText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.frameSequenceText.ForeColor = System.Drawing.Color.White;
            this.frameSequenceText.Location = new System.Drawing.Point(15, 187);
            this.frameSequenceText.Multiline = true;
            this.frameSequenceText.Name = "frameSequenceText";
            this.frameSequenceText.Size = new System.Drawing.Size(227, 124);
            this.frameSequenceText.TabIndex = 4;
            this.frameSequenceText.Validating += new System.ComponentModel.CancelEventHandler(this.frameSequenceText_Validating);
            // 
            // flipbookStyle
            // 
            this.flipbookStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.flipbookStyle.DateTimeValue = new System.DateTime(2008, 10, 26, 20, 8, 25, 389);
            this.flipbookStyle.Filter = TerraViewer.Classification.Unfiltered;
            this.flipbookStyle.FilterStyle = false;
            this.flipbookStyle.Location = new System.Drawing.Point(13, 130);
            this.flipbookStyle.Margin = new System.Windows.Forms.Padding(0);
            this.flipbookStyle.MaximumSize = new System.Drawing.Size(248, 33);
            this.flipbookStyle.MinimumSize = new System.Drawing.Size(35, 33);
            this.flipbookStyle.Name = "flipbookStyle";
            this.flipbookStyle.SelectedIndex = -1;
            this.flipbookStyle.SelectedItem = null;
            this.flipbookStyle.Size = new System.Drawing.Size(229, 33);
            this.flipbookStyle.State = TerraViewer.State.Rest;
            this.flipbookStyle.TabIndex = 6;
            this.flipbookStyle.Type = TerraViewer.WwtCombo.ComboType.List;
            this.flipbookStyle.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.flipbookStyle_SelectionChanged);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(165, 329);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(79, 33);
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "Cancel";
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(89, 329);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(79, 33);
            this.OK.TabIndex = 1;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // FlipbookSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(256, 374);
            this.ControlBox = false;
            this.Controls.Add(this.flipbookStyle);
            this.Controls.Add(this.endFrame);
            this.Controls.Add(this.gridYText);
            this.Controls.Add(this.gridXText);
            this.Controls.Add(this.frameSequenceText);
            this.Controls.Add(this.startFrame);
            this.Controls.Add(this.gridYLabel);
            this.Controls.Add(this.loopTypeLabel);
            this.Controls.Add(this.endFrameLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.frameSequenceLabel);
            this.Controls.Add(this.gridXlabel);
            this.Controls.Add(this.startFrameLabel);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FlipbookSetup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Flipbook Setup";
            this.Load += new System.EventHandler(this.FlipbookSetup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton Cancel;
        private WwtButton OK;
        private System.Windows.Forms.TextBox startFrame;
        private System.Windows.Forms.Label startFrameLabel;
        private System.Windows.Forms.TextBox endFrame;
        private System.Windows.Forms.Label endFrameLabel;
        private System.Windows.Forms.Label gridXlabel;
        private System.Windows.Forms.TextBox gridXText;
        private System.Windows.Forms.Label gridYLabel;
        private System.Windows.Forms.TextBox gridYText;
        private System.Windows.Forms.Label label1;
        private WwtCombo flipbookStyle;
        private System.Windows.Forms.Label loopTypeLabel;
        private System.Windows.Forms.Label frameSequenceLabel;
        private System.Windows.Forms.TextBox frameSequenceText;

    }
}