namespace TerraViewer
{
    partial class PopupColorPicker
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
            this.colors = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.opacity = new System.Windows.Forms.NumericUpDown();
            this.oldColor = new System.Windows.Forms.PictureBox();
            this.newColor = new System.Windows.Forms.PictureBox();
            this.ok = new TerraViewer.WwtButton();
            ((System.ComponentModel.ISupportInitialize)(this.colors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.opacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.oldColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.newColor)).BeginInit();
            this.SuspendLayout();
            // 
            // colors
            // 
            this.colors.Cursor = System.Windows.Forms.Cursors.Cross;
            this.colors.Dock = System.Windows.Forms.DockStyle.Top;
            this.colors.Image = global::TerraViewer.Properties.Resources.ColorPickerHex;
            this.colors.Location = new System.Drawing.Point(0, 0);
            this.colors.Name = "colors";
            this.colors.Size = new System.Drawing.Size(190, 208);
            this.colors.TabIndex = 0;
            this.colors.TabStop = false;
            this.colors.MouseMove += new System.Windows.Forms.MouseEventHandler(this.colors_MouseMove);
            this.colors.Click += new System.EventHandler(this.colors_Click);
            this.colors.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.colors_MouseDoubleClick);
            this.colors.MouseClick += new System.Windows.Forms.MouseEventHandler(this.colors_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 215);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Opacity";
            // 
            // opacity
            // 
            this.opacity.BackColor = System.Drawing.Color.Black;
            this.opacity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opacity.ForeColor = System.Drawing.Color.White;
            this.opacity.Location = new System.Drawing.Point(6, 230);
            this.opacity.Name = "opacity";
            this.opacity.Size = new System.Drawing.Size(43, 20);
            this.opacity.TabIndex = 2;
            this.opacity.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // oldColor
            // 
            this.oldColor.BackColor = System.Drawing.Color.White;
            this.oldColor.Location = new System.Drawing.Point(56, 215);
            this.oldColor.Name = "oldColor";
            this.oldColor.Size = new System.Drawing.Size(40, 32);
            this.oldColor.TabIndex = 3;
            this.oldColor.TabStop = false;
            this.oldColor.Click += new System.EventHandler(this.oldColor_Click);
            // 
            // newColor
            // 
            this.newColor.BackColor = System.Drawing.Color.Black;
            this.newColor.Location = new System.Drawing.Point(96, 215);
            this.newColor.Name = "newColor";
            this.newColor.Size = new System.Drawing.Size(40, 32);
            this.newColor.TabIndex = 3;
            this.newColor.TabStop = false;
            this.newColor.Click += new System.EventHandler(this.newColor_Click);
            // 
            // ok
            // 
            this.ok.BackColor = System.Drawing.Color.Transparent;
            this.ok.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ok.ImageDisabled = null;
            this.ok.ImageEnabled = null;
            this.ok.Location = new System.Drawing.Point(133, 215);
            this.ok.MaximumSize = new System.Drawing.Size(140, 33);
            this.ok.Name = "ok";
            this.ok.Selected = false;
            this.ok.Size = new System.Drawing.Size(53, 33);
            this.ok.TabIndex = 4;
            this.ok.Text = "OK";
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // PopupColorPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(190, 255);
            this.ControlBox = false;
            this.Controls.Add(this.ok);
            this.Controls.Add(this.newColor);
            this.Controls.Add(this.oldColor);
            this.Controls.Add(this.opacity);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.colors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupColorPicker";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.PopupColorPicker_Deactivate);
            this.Load += new System.EventHandler(this.PopupColorPicker_Load);
            this.Leave += new System.EventHandler(this.PopupColorPicker_Leave);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PopupColorPicker_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.colors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.opacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.oldColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.newColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox colors;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown opacity;
        private System.Windows.Forms.PictureBox oldColor;
        private System.Windows.Forms.PictureBox newColor;
        private WwtButton ok;
    }
}