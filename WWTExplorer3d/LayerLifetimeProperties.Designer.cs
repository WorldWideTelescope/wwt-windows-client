namespace TerraViewer
{
    partial class LayerLifetimeProperties
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
            this.label2 = new System.Windows.Forms.Label();
            this.fadeTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.OK = new TerraViewer.WwtButton();
            this.fadeType = new TerraViewer.WwtCombo();
            this.endDate = new TerraViewer.WwtCombo();
            this.startDate = new TerraViewer.WwtCombo();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Start DateTime";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "End DateTime";
            // 
            // fadeTime
            // 
            this.fadeTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.fadeTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.fadeTime.ForeColor = System.Drawing.Color.White;
            this.fadeTime.Location = new System.Drawing.Point(149, 127);
            this.fadeTime.Name = "fadeTime";
            this.fadeTime.Size = new System.Drawing.Size(102, 20);
            this.fadeTime.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(146, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Fade In/Out Time";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Fade Type";
            // 
            // OK
            // 
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(173, 157);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(85, 33);
            this.OK.TabIndex = 18;
            this.OK.Text = "Close";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // fadeType
            // 
            this.fadeType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.fadeType.DateTimeValue = new System.DateTime(2010, 9, 30, 17, 1, 57, 959);
            this.fadeType.Filter = TerraViewer.Classification.Unfiltered;
            this.fadeType.FilterStyle = false;
            this.fadeType.Location = new System.Drawing.Point(12, 122);
            this.fadeType.Margin = new System.Windows.Forms.Padding(0);
            this.fadeType.MasterTime = true;
            this.fadeType.MaximumSize = new System.Drawing.Size(248, 33);
            this.fadeType.MinimumSize = new System.Drawing.Size(35, 33);
            this.fadeType.Name = "fadeType";
            this.fadeType.SelectedIndex = -1;
            this.fadeType.SelectedItem = null;
            this.fadeType.Size = new System.Drawing.Size(122, 33);
            this.fadeType.State = TerraViewer.State.Rest;
            this.fadeType.TabIndex = 0;
            this.fadeType.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // endDate
            // 
            this.endDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.endDate.DateTimeValue = new System.DateTime(2010, 9, 30, 17, 1, 57, 959);
            this.endDate.Filter = TerraViewer.Classification.Unfiltered;
            this.endDate.FilterStyle = false;
            this.endDate.Location = new System.Drawing.Point(10, 76);
            this.endDate.Margin = new System.Windows.Forms.Padding(0);
            this.endDate.MasterTime = false;
            this.endDate.MaximumSize = new System.Drawing.Size(248, 33);
            this.endDate.MinimumSize = new System.Drawing.Size(35, 33);
            this.endDate.Name = "endDate";
            this.endDate.SelectedIndex = -1;
            this.endDate.SelectedItem = null;
            this.endDate.Size = new System.Drawing.Size(248, 33);
            this.endDate.State = TerraViewer.State.Rest;
            this.endDate.TabIndex = 0;
            this.endDate.Type = TerraViewer.WwtCombo.ComboType.DateTime;
            // 
            // startDate
            // 
            this.startDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.startDate.DateTimeValue = new System.DateTime(2010, 9, 30, 17, 1, 57, 959);
            this.startDate.Filter = TerraViewer.Classification.Unfiltered;
            this.startDate.FilterStyle = false;
            this.startDate.Location = new System.Drawing.Point(10, 29);
            this.startDate.Margin = new System.Windows.Forms.Padding(0);
            this.startDate.MasterTime = false;
            this.startDate.MaximumSize = new System.Drawing.Size(248, 33);
            this.startDate.MinimumSize = new System.Drawing.Size(35, 33);
            this.startDate.Name = "startDate";
            this.startDate.SelectedIndex = -1;
            this.startDate.SelectedItem = null;
            this.startDate.Size = new System.Drawing.Size(248, 33);
            this.startDate.State = TerraViewer.State.Rest;
            this.startDate.TabIndex = 0;
            this.startDate.Type = TerraViewer.WwtCombo.ComboType.DateTime;
            this.startDate.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.startDate_SelectionChanged);
            // 
            // LayerLifetimeProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(266, 198);
            this.ControlBox = false;
            this.Controls.Add(this.OK);
            this.Controls.Add(this.fadeTime);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fadeType);
            this.Controls.Add(this.endDate);
            this.Controls.Add(this.startDate);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LayerLifetimeProperties";
            this.Text = "Layer Lifetime";
            this.Load += new System.EventHandler(this.LayerLifetimeProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtCombo startDate;
        private WwtCombo endDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fadeTime;
        private System.Windows.Forms.Label label3;
        private WwtCombo fadeType;
        private System.Windows.Forms.Label label4;
        private WwtButton OK;
    }
}