namespace TerraViewer
{
    partial class GreatCircleProperties
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
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Percentage = new System.Windows.Forms.TextBox();
            this.EndLng = new System.Windows.Forms.TextBox();
            this.EndLat = new System.Windows.Forms.TextBox();
            this.StartLng = new System.Windows.Forms.TextBox();
            this.StartLat = new System.Windows.Forms.TextBox();
            this.ok = new TerraViewer.WwtButton();
            this.LineWidth = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.StartFromView = new TerraViewer.WwtButton();
            this.EndFromView = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 103);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Percentage";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(79, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Lng";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(22, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Lat";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(79, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Lng";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Lat";
            // 
            // Percentage
            // 
            this.Percentage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Percentage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Percentage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Percentage.ForeColor = System.Drawing.Color.White;
            this.Percentage.Location = new System.Drawing.Point(16, 119);
            this.Percentage.MaxLength = 64;
            this.Percentage.Name = "Percentage";
            this.Percentage.Size = new System.Drawing.Size(60, 20);
            this.Percentage.TabIndex = 12;
            this.Percentage.Text = "0";
            this.Percentage.TextChanged += new System.EventHandler(this.Percentage_TextChanged);
            // 
            // EndLng
            // 
            this.EndLng.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.EndLng.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.EndLng.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EndLng.ForeColor = System.Drawing.Color.White;
            this.EndLng.Location = new System.Drawing.Point(82, 71);
            this.EndLng.MaxLength = 64;
            this.EndLng.Name = "EndLng";
            this.EndLng.Size = new System.Drawing.Size(60, 20);
            this.EndLng.TabIndex = 9;
            this.EndLng.Text = "0";
            this.EndLng.TextChanged += new System.EventHandler(this.EndLng_TextChanged);
            // 
            // EndLat
            // 
            this.EndLat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.EndLat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.EndLat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EndLat.ForeColor = System.Drawing.Color.White;
            this.EndLat.Location = new System.Drawing.Point(16, 71);
            this.EndLat.MaxLength = 64;
            this.EndLat.Name = "EndLat";
            this.EndLat.Size = new System.Drawing.Size(60, 20);
            this.EndLat.TabIndex = 8;
            this.EndLat.Text = "0";
            this.EndLat.TextChanged += new System.EventHandler(this.EndLat_TextChanged);
            // 
            // StartLng
            // 
            this.StartLng.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.StartLng.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.StartLng.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartLng.ForeColor = System.Drawing.Color.White;
            this.StartLng.Location = new System.Drawing.Point(82, 31);
            this.StartLng.MaxLength = 64;
            this.StartLng.Name = "StartLng";
            this.StartLng.Size = new System.Drawing.Size(60, 20);
            this.StartLng.TabIndex = 11;
            this.StartLng.Text = "0";
            this.StartLng.TextChanged += new System.EventHandler(this.StartLng_TextChanged);
            // 
            // StartLat
            // 
            this.StartLat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.StartLat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.StartLat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartLat.ForeColor = System.Drawing.Color.White;
            this.StartLat.Location = new System.Drawing.Point(16, 31);
            this.StartLat.MaxLength = 64;
            this.StartLat.Name = "StartLat";
            this.StartLat.Size = new System.Drawing.Size(60, 20);
            this.StartLat.TabIndex = 10;
            this.StartLat.Text = "0";
            this.StartLat.TextChanged += new System.EventHandler(this.StartLat_TextChanged);
            // 
            // ok
            // 
            this.ok.BackColor = System.Drawing.Color.Transparent;
            this.ok.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ok.ImageDisabled = null;
            this.ok.ImageEnabled = null;
            this.ok.Location = new System.Drawing.Point(197, 106);
            this.ok.MaximumSize = new System.Drawing.Size(140, 33);
            this.ok.Name = "ok";
            this.ok.Selected = false;
            this.ok.Size = new System.Drawing.Size(77, 33);
            this.ok.TabIndex = 7;
            this.ok.Text = "Ok";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // LineWidth
            // 
            this.LineWidth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.LineWidth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LineWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LineWidth.ForeColor = System.Drawing.Color.White;
            this.LineWidth.Location = new System.Drawing.Point(82, 119);
            this.LineWidth.MaxLength = 64;
            this.LineWidth.Name = "LineWidth";
            this.LineWidth.Size = new System.Drawing.Size(60, 20);
            this.LineWidth.TabIndex = 12;
            this.LineWidth.Text = "0";
            this.LineWidth.TextChanged += new System.EventHandler(this.LineWidth_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(79, 103);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Line Width";
            // 
            // StartFromView
            // 
            this.StartFromView.BackColor = System.Drawing.Color.Transparent;
            this.StartFromView.DialogResult = System.Windows.Forms.DialogResult.None;
            this.StartFromView.ImageDisabled = null;
            this.StartFromView.ImageEnabled = null;
            this.StartFromView.Location = new System.Drawing.Point(148, 24);
            this.StartFromView.MaximumSize = new System.Drawing.Size(140, 33);
            this.StartFromView.Name = "StartFromView";
            this.StartFromView.Selected = false;
            this.StartFromView.Size = new System.Drawing.Size(126, 33);
            this.StartFromView.TabIndex = 7;
            this.StartFromView.Text = "<< Get From View";
            this.StartFromView.Click += new System.EventHandler(this.StartFromView_Click);
            // 
            // EndFromView
            // 
            this.EndFromView.BackColor = System.Drawing.Color.Transparent;
            this.EndFromView.DialogResult = System.Windows.Forms.DialogResult.None;
            this.EndFromView.ImageDisabled = null;
            this.EndFromView.ImageEnabled = null;
            this.EndFromView.Location = new System.Drawing.Point(148, 63);
            this.EndFromView.MaximumSize = new System.Drawing.Size(140, 33);
            this.EndFromView.Name = "EndFromView";
            this.EndFromView.Selected = false;
            this.EndFromView.Size = new System.Drawing.Size(126, 33);
            this.EndFromView.TabIndex = 7;
            this.EndFromView.Text = "<< Get From View";
            this.EndFromView.Click += new System.EventHandler(this.EndFromView_Click);
            // 
            // GreatCircleProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(279, 151);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LineWidth);
            this.Controls.Add(this.Percentage);
            this.Controls.Add(this.EndLng);
            this.Controls.Add(this.EndLat);
            this.Controls.Add(this.StartLng);
            this.Controls.Add(this.StartLat);
            this.Controls.Add(this.EndFromView);
            this.Controls.Add(this.StartFromView);
            this.Controls.Add(this.ok);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GreatCircleProperties";
            this.Text = "GreatCircleProperties";
            this.Load += new System.EventHandler(this.GreatCircleProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Percentage;
        private System.Windows.Forms.TextBox EndLng;
        private System.Windows.Forms.TextBox EndLat;
        private System.Windows.Forms.TextBox StartLng;
        private System.Windows.Forms.TextBox StartLat;
        private WwtButton ok;
        private System.Windows.Forms.TextBox LineWidth;
        private System.Windows.Forms.Label label6;
        private WwtButton StartFromView;
        private WwtButton EndFromView;
    }
}