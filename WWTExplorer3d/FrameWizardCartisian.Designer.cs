namespace TerraViewer
{
    partial class FrameWizardCartisian
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
            this.label3 = new System.Windows.Forms.Label();
            this.altDepthLabel = new System.Windows.Forms.Label();
            this.longRALable = new System.Windows.Forms.Label();
            this.latDecLabel = new System.Windows.Forms.Label();
            this.spreadsheetNameEdit = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(18, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(604, 47);
            this.label3.TabIndex = 16;
            this.label3.Text = "Select the Latitude, Longitude and Altitude";
            // 
            // altDepthLabel
            // 
            this.altDepthLabel.AutoSize = true;
            this.altDepthLabel.Location = new System.Drawing.Point(222, 140);
            this.altDepthLabel.Name = "altDepthLabel";
            this.altDepthLabel.Size = new System.Drawing.Size(83, 13);
            this.altDepthLabel.TabIndex = 18;
            this.altDepthLabel.Text = "Altitude (Meters)";
            // 
            // longRALable
            // 
            this.longRALable.AutoSize = true;
            this.longRALable.Location = new System.Drawing.Point(18, 140);
            this.longRALable.Name = "longRALable";
            this.longRALable.Size = new System.Drawing.Size(144, 13);
            this.longRALable.TabIndex = 19;
            this.longRALable.Text = "Longitude (Decimal Degrees)";
            // 
            // latDecLabel
            // 
            this.latDecLabel.AutoSize = true;
            this.latDecLabel.Location = new System.Drawing.Point(18, 84);
            this.latDecLabel.Name = "latDecLabel";
            this.latDecLabel.Size = new System.Drawing.Size(135, 13);
            this.latDecLabel.TabIndex = 17;
            this.latDecLabel.Text = "Latitude (Decimal Degrees)";
            // 
            // spreadsheetNameEdit
            // 
            this.spreadsheetNameEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.spreadsheetNameEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spreadsheetNameEdit.ForeColor = System.Drawing.Color.White;
            this.spreadsheetNameEdit.Location = new System.Drawing.Point(21, 100);
            this.spreadsheetNameEdit.Name = "spreadsheetNameEdit";
            this.spreadsheetNameEdit.Size = new System.Drawing.Size(139, 20);
            this.spreadsheetNameEdit.TabIndex = 20;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(21, 156);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(139, 20);
            this.textBox1.TabIndex = 20;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.ForeColor = System.Drawing.Color.White;
            this.textBox2.Location = new System.Drawing.Point(225, 156);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(139, 20);
            this.textBox2.TabIndex = 20;
            // 
            // FrameWizardCartisian
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.spreadsheetNameEdit);
            this.Controls.Add(this.altDepthLabel);
            this.Controls.Add(this.longRALable);
            this.Controls.Add(this.latDecLabel);
            this.Controls.Add(this.label3);
            this.Name = "FrameWizardCartisian";
            this.Load += new System.EventHandler(this.FrameWizardCartisian_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label altDepthLabel;
        private System.Windows.Forms.Label longRALable;
        private System.Windows.Forms.Label latDecLabel;
        private System.Windows.Forms.TextBox spreadsheetNameEdit;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
    }
}
