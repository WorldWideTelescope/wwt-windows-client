namespace TerraViewer.Callibration
{
    partial class GroundTruthPointProperties
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
            this.AltText = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AzText = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.WeightText = new System.Windows.Forms.TextBox();
            this.OK = new TerraViewer.WwtButton();
            this.ConstraintTypeCombo = new TerraViewer.WwtCombo();
            this.SuspendLayout();
            // 
            // AltText
            // 
            this.AltText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.AltText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AltText.ForeColor = System.Drawing.Color.White;
            this.AltText.Location = new System.Drawing.Point(13, 83);
            this.AltText.Name = "AltText";
            this.AltText.Size = new System.Drawing.Size(53, 20);
            this.AltText.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Altitude";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Constraint Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(76, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Azimuth";
            // 
            // AzText
            // 
            this.AzText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.AzText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AzText.ForeColor = System.Drawing.Color.White;
            this.AzText.Location = new System.Drawing.Point(79, 83);
            this.AzText.Name = "AzText";
            this.AzText.Size = new System.Drawing.Size(53, 20);
            this.AzText.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(142, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Weight";
            // 
            // WeightText
            // 
            this.WeightText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.WeightText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.WeightText.ForeColor = System.Drawing.Color.White;
            this.WeightText.Location = new System.Drawing.Point(145, 83);
            this.WeightText.Name = "WeightText";
            this.WeightText.Size = new System.Drawing.Size(53, 20);
            this.WeightText.TabIndex = 7;
            // 
            // OK
            // 
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.None;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(207, 76);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(63, 33);
            this.OK.TabIndex = 8;
            this.OK.Text = "Ok";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // ConstraintTypeCombo
            // 
            this.ConstraintTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ConstraintTypeCombo.DateTimeValue = new System.DateTime(2010, 12, 20, 17, 24, 52, 54);
            this.ConstraintTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.ConstraintTypeCombo.FilterStyle = false;
            this.ConstraintTypeCombo.Location = new System.Drawing.Point(9, 28);
            this.ConstraintTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.ConstraintTypeCombo.MasterTime = true;
            this.ConstraintTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.ConstraintTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.ConstraintTypeCombo.Name = "ConstraintTypeCombo";
            this.ConstraintTypeCombo.SelectedIndex = -1;
            this.ConstraintTypeCombo.SelectedItem = null;
            this.ConstraintTypeCombo.Size = new System.Drawing.Size(248, 33);
            this.ConstraintTypeCombo.State = TerraViewer.State.Rest;
            this.ConstraintTypeCombo.TabIndex = 1;
            this.ConstraintTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.ConstraintTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.ConstraintTypeCombo_SelectionChanged);
            // 
            // GroundTruthPointProperties
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(276, 122);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.ConstraintTypeCombo);
            this.Controls.Add(this.WeightText);
            this.Controls.Add(this.AzText);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.AltText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GroundTruthPointProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GroundTruthPointProperties";
            this.Load += new System.EventHandler(this.GroundTruthPointProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox AltText;
        private System.Windows.Forms.Label label1;
        private WwtCombo ConstraintTypeCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox AzText;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox WeightText;
        private WwtButton OK;
    }
}