namespace TerraViewer
{
    partial class GroundOverlayProperties
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
            this.North = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.South = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.West = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.East = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Rotation = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ok = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // North
            // 
            this.North.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.North.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.North.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.North.ForeColor = System.Drawing.Color.White;
            this.North.Location = new System.Drawing.Point(12, 25);
            this.North.MaxLength = 64;
            this.North.Name = "North";
            this.North.Size = new System.Drawing.Size(60, 20);
            this.North.TabIndex = 5;
            this.North.Text = "0";
            this.North.TextChanged += new System.EventHandler(this.North_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "North";
            // 
            // South
            // 
            this.South.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.South.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.South.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.South.ForeColor = System.Drawing.Color.White;
            this.South.Location = new System.Drawing.Point(78, 25);
            this.South.MaxLength = 64;
            this.South.Name = "South";
            this.South.Size = new System.Drawing.Size(60, 20);
            this.South.TabIndex = 5;
            this.South.Text = "0";
            this.South.TextChanged += new System.EventHandler(this.South_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(75, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "South";
            // 
            // West
            // 
            this.West.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.West.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.West.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.West.ForeColor = System.Drawing.Color.White;
            this.West.Location = new System.Drawing.Point(144, 25);
            this.West.MaxLength = 64;
            this.West.Name = "West";
            this.West.Size = new System.Drawing.Size(60, 20);
            this.West.TabIndex = 5;
            this.West.Text = "0";
            this.West.TextChanged += new System.EventHandler(this.West_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(141, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "West";
            // 
            // East
            // 
            this.East.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.East.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.East.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.East.ForeColor = System.Drawing.Color.White;
            this.East.Location = new System.Drawing.Point(210, 25);
            this.East.MaxLength = 64;
            this.East.Name = "East";
            this.East.Size = new System.Drawing.Size(60, 20);
            this.East.TabIndex = 5;
            this.East.Text = "0";
            this.East.TextChanged += new System.EventHandler(this.East_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(207, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(28, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "East";
             // 
            // Rotation
            // 
            this.Rotation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Rotation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Rotation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Rotation.ForeColor = System.Drawing.Color.White;
            this.Rotation.Location = new System.Drawing.Point(12, 70);
            this.Rotation.MaxLength = 64;
            this.Rotation.Name = "Rotation";
            this.Rotation.Size = new System.Drawing.Size(60, 20);
            this.Rotation.TabIndex = 5;
            this.Rotation.Text = "0";
            this.Rotation.TextChanged += new System.EventHandler(this.Rotation_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "Rotation";
            // 
            // ok
            // 
            this.ok.BackColor = System.Drawing.Color.Transparent;
            this.ok.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ok.ImageDisabled = null;
            this.ok.ImageEnabled = null;
            this.ok.Location = new System.Drawing.Point(207, 96);
            this.ok.MaximumSize = new System.Drawing.Size(140, 33);
            this.ok.Name = "ok";
            this.ok.Selected = false;
            this.ok.Size = new System.Drawing.Size(73, 33);
            this.ok.TabIndex = 0;
            this.ok.Text = "Ok";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // GroundOverlayProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(289, 141);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Rotation);
            this.Controls.Add(this.East);
            this.Controls.Add(this.West);
            this.Controls.Add(this.South);
            this.Controls.Add(this.North);
            this.Controls.Add(this.ok);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GroundOverlayProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Ground Overlay Properties";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GroundOverlayProperties_FormClosed);
            this.Load += new System.EventHandler(this.GroundOverlayProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton ok;
        private System.Windows.Forms.TextBox North;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox South;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox West;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox East;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Rotation;
        private System.Windows.Forms.Label label5;
    }
}