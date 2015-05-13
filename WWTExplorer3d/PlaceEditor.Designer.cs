namespace TerraViewer
{
    partial class PlaceEditor
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
            this.names = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ra = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dec = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mag = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.zoom = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.classification = new TerraViewer.WwtCombo();
            this.constellation = new TerraViewer.WwtCombo();
            this.Cancel = new TerraViewer.WwtButton();
            this.fromView = new TerraViewer.WwtButton();
            this.OK = new TerraViewer.WwtButton();
            this.distanceLabel = new System.Windows.Forms.Label();
            this.DistanceValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Names (separate names with a semicolon)";
            // 
            // names
            // 
            this.names.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.names.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.names.ForeColor = System.Drawing.Color.White;
            this.names.Location = new System.Drawing.Point(12, 25);
            this.names.Name = "names";
            this.names.Size = new System.Drawing.Size(242, 22);
            this.names.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "RA";
            // 
            // ra
            // 
            this.ra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ra.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ra.ForeColor = System.Drawing.Color.White;
            this.ra.Location = new System.Drawing.Point(11, 175);
            this.ra.Name = "ra";
            this.ra.Size = new System.Drawing.Size(79, 22);
            this.ra.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 208);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Dec";
            // 
            // dec
            // 
            this.dec.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dec.ForeColor = System.Drawing.Color.White;
            this.dec.Location = new System.Drawing.Point(12, 224);
            this.dec.Name = "dec";
            this.dec.Size = new System.Drawing.Size(79, 22);
            this.dec.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(106, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Visual Magnitude";
            // 
            // mag
            // 
            this.mag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.mag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mag.ForeColor = System.Drawing.Color.White;
            this.mag.Location = new System.Drawing.Point(109, 175);
            this.mag.Name = "mag";
            this.mag.Size = new System.Drawing.Size(79, 22);
            this.mag.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(106, 208);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Angular Diameter";
            // 
            // zoom
            // 
            this.zoom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.zoom.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.zoom.ForeColor = System.Drawing.Color.White;
            this.zoom.Location = new System.Drawing.Point(109, 224);
            this.zoom.Name = "zoom";
            this.zoom.Size = new System.Drawing.Size(79, 22);
            this.zoom.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 104);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(76, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Constellation";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Classification";
            // 
            // classification
            // 
            this.classification.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.classification.DateTimeValue = new System.DateTime(2008, 3, 13, 18, 27, 8, 129);
            this.classification.Filter = TerraViewer.Classification.Unfiltered;
            this.classification.FilterStyle = false;
            this.classification.Location = new System.Drawing.Point(11, 66);
            this.classification.Margin = new System.Windows.Forms.Padding(0);
            this.classification.MaximumSize = new System.Drawing.Size(248, 33);
            this.classification.MinimumSize = new System.Drawing.Size(35, 33);
            this.classification.Name = "classification";
            this.classification.SelectedIndex = -1;
            this.classification.Size = new System.Drawing.Size(248, 33);
            this.classification.State = TerraViewer.State.Rest;
            this.classification.TabIndex = 3;
            this.classification.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // constellation
            // 
            this.constellation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.constellation.DateTimeValue = new System.DateTime(2008, 3, 13, 18, 27, 8, 129);
            this.constellation.Filter = TerraViewer.Classification.Unfiltered;
            this.constellation.FilterStyle = false;
            this.constellation.Location = new System.Drawing.Point(12, 117);
            this.constellation.Margin = new System.Windows.Forms.Padding(0);
            this.constellation.MaximumSize = new System.Drawing.Size(248, 33);
            this.constellation.MinimumSize = new System.Drawing.Size(35, 33);
            this.constellation.Name = "constellation";
            this.constellation.SelectedIndex = -1;
            this.constellation.Size = new System.Drawing.Size(248, 33);
            this.constellation.State = TerraViewer.State.Rest;
            this.constellation.TabIndex = 3;
            this.constellation.Type = TerraViewer.WwtCombo.ComboType.List;
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(189, 356);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(79, 33);
            this.Cancel.TabIndex = 0;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // fromView
            // 
            this.fromView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.fromView.BackColor = System.Drawing.Color.Transparent;
            this.fromView.DialogResult = System.Windows.Forms.DialogResult.None;
            this.fromView.ImageDisabled = null;
            this.fromView.ImageEnabled = null;
            this.fromView.Location = new System.Drawing.Point(36, 297);
            this.fromView.MaximumSize = new System.Drawing.Size(140, 33);
            this.fromView.Name = "fromView";
            this.fromView.Selected = false;
            this.fromView.Size = new System.Drawing.Size(126, 33);
            this.fromView.TabIndex = 0;
            this.fromView.Text = "FromView";
            this.fromView.Click += new System.EventHandler(this.fromView_Click);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.None;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(113, 356);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(79, 33);
            this.OK.TabIndex = 0;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // distanceLabel
            // 
            this.distanceLabel.AutoSize = true;
            this.distanceLabel.Location = new System.Drawing.Point(11, 253);
            this.distanceLabel.Name = "distanceLabel";
            this.distanceLabel.Size = new System.Drawing.Size(51, 13);
            this.distanceLabel.TabIndex = 1;
            this.distanceLabel.Text = "Distance";
            // 
            // DistanceValue
            // 
            this.DistanceValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.DistanceValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DistanceValue.ForeColor = System.Drawing.Color.White;
            this.DistanceValue.Location = new System.Drawing.Point(11, 269);
            this.DistanceValue.Name = "DistanceValue";
            this.DistanceValue.Size = new System.Drawing.Size(79, 22);
            this.DistanceValue.TabIndex = 2;
            // 
            // PlaceEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(269, 392);
            this.ControlBox = false;
            this.Controls.Add(this.classification);
            this.Controls.Add(this.constellation);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.zoom);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.mag);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.DistanceValue);
            this.Controls.Add(this.distanceLabel);
            this.Controls.Add(this.dec);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ra);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.names);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.fromView);
            this.Controls.Add(this.OK);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PlaceEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Object Information";
            this.Load += new System.EventHandler(this.PlaceEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton OK;
        private WwtButton Cancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox names;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ra;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox dec;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox mag;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox zoom;
        private WwtCombo constellation;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private WwtCombo classification;
        private WwtButton fromView;
        private System.Windows.Forms.Label distanceLabel;
        private System.Windows.Forms.TextBox DistanceValue;
    }
}