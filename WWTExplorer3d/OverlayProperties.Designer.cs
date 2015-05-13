namespace TerraViewer
{
    partial class OverlayProperties
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
            this.OK = new TerraViewer.WwtButton();
            this.posXLabel = new System.Windows.Forms.Label();
            this.positionX = new System.Windows.Forms.TextBox();
            this.positionY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.sizeX = new System.Windows.Forms.TextBox();
            this.sizeY = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.OverlayName = new System.Windows.Forms.TextBox();
            this.Rotation = new System.Windows.Forms.TextBox();
            this.RotationLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.None;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(188, 110);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(78, 33);
            this.OK.TabIndex = 10;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // posXLabel
            // 
            this.posXLabel.AutoSize = true;
            this.posXLabel.Location = new System.Drawing.Point(12, 60);
            this.posXLabel.Name = "posXLabel";
            this.posXLabel.Size = new System.Drawing.Size(14, 13);
            this.posXLabel.TabIndex = 2;
            this.posXLabel.Text = "X";
            // 
            // positionX
            // 
            this.positionX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.positionX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.positionX.ForeColor = System.Drawing.Color.White;
            this.positionX.Location = new System.Drawing.Point(15, 79);
            this.positionX.Name = "positionX";
            this.positionX.Size = new System.Drawing.Size(44, 20);
            this.positionX.TabIndex = 3;
            this.positionX.TextChanged += new System.EventHandler(this.positionX_TextChanged);
            // 
            // positionY
            // 
            this.positionY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.positionY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.positionY.ForeColor = System.Drawing.Color.White;
            this.positionY.Location = new System.Drawing.Point(65, 79);
            this.positionY.Name = "positionY";
            this.positionY.Size = new System.Drawing.Size(44, 20);
            this.positionY.TabIndex = 5;
            this.positionY.TextChanged += new System.EventHandler(this.positionY_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Y";
            // 
            // sizeX
            // 
            this.sizeX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.sizeX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sizeX.ForeColor = System.Drawing.Color.White;
            this.sizeX.Location = new System.Drawing.Point(115, 79);
            this.sizeX.Name = "sizeX";
            this.sizeX.Size = new System.Drawing.Size(44, 20);
            this.sizeX.TabIndex = 7;
            this.sizeX.TextChanged += new System.EventHandler(this.sizeX_TextChanged);
            // 
            // sizeY
            // 
            this.sizeY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.sizeY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sizeY.ForeColor = System.Drawing.Color.White;
            this.sizeY.Location = new System.Drawing.Point(165, 79);
            this.sizeY.Name = "sizeY";
            this.sizeY.Size = new System.Drawing.Size(44, 20);
            this.sizeY.TabIndex = 9;
            this.sizeY.TextChanged += new System.EventHandler(this.sizeY_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(112, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Width";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(162, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Height";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(12, 9);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(35, 13);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Name";
            // 
            // OverlayName
            // 
            this.OverlayName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.OverlayName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OverlayName.ForeColor = System.Drawing.Color.White;
            this.OverlayName.Location = new System.Drawing.Point(15, 28);
            this.OverlayName.Name = "OverlayName";
            this.OverlayName.Size = new System.Drawing.Size(244, 20);
            this.OverlayName.TabIndex = 1;
            this.OverlayName.TextChanged += new System.EventHandler(this.Name_TextChanged);
            // 
            // Rotation
            // 
            this.Rotation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Rotation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Rotation.ForeColor = System.Drawing.Color.White;
            this.Rotation.Location = new System.Drawing.Point(215, 79);
            this.Rotation.Name = "Rotation";
            this.Rotation.Size = new System.Drawing.Size(44, 20);
            this.Rotation.TabIndex = 9;
            this.Rotation.TextChanged += new System.EventHandler(this.Rotation_TextChanged);
            // 
            // RotationLabel
            // 
            this.RotationLabel.AutoSize = true;
            this.RotationLabel.Location = new System.Drawing.Point(212, 60);
            this.RotationLabel.Name = "RotationLabel";
            this.RotationLabel.Size = new System.Drawing.Size(47, 13);
            this.RotationLabel.TabIndex = 8;
            this.RotationLabel.Text = "Rotation";
            // 
            // OverlayProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(278, 155);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.RotationLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.posXLabel);
            this.Controls.Add(this.Rotation);
            this.Controls.Add(this.sizeY);
            this.Controls.Add(this.positionY);
            this.Controls.Add(this.sizeX);
            this.Controls.Add(this.OverlayName);
            this.Controls.Add(this.positionX);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OverlayProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Overlay Properties";
            this.Load += new System.EventHandler(this.OverlayProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton OK;
        private System.Windows.Forms.Label posXLabel;
        private System.Windows.Forms.TextBox positionX;
        private System.Windows.Forms.TextBox positionY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox sizeX;
        private System.Windows.Forms.TextBox sizeY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.TextBox OverlayName;
        private System.Windows.Forms.TextBox Rotation;
        private System.Windows.Forms.Label RotationLabel;
    }
}