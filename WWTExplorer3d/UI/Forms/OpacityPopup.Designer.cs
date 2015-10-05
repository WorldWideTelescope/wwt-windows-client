namespace TerraViewer
{
    partial class OpacityPopup
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
            this.valueSlider = new TerraViewer.WwtTrackBar();
            this.closeBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // valueSlider
            // 
            this.valueSlider.BackColor = System.Drawing.Color.Transparent;
            this.valueSlider.Location = new System.Drawing.Point(11, 5);
            this.valueSlider.Max = 100;
            this.valueSlider.Name = "valueSlider";
            this.valueSlider.Size = new System.Drawing.Size(80, 20);
            this.valueSlider.TabIndex = 0;
            this.valueSlider.Value = 50;
            this.valueSlider.ValueChanged += new System.EventHandler(this.valueSlider_ValueChanged);
            this.valueSlider.Leave += new System.EventHandler(this.valueSlider_Leave);
            // 
            // closeBox
            // 
            this.closeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBox.Image = global::TerraViewer.Properties.Resources.CloseRest;
            this.closeBox.Location = new System.Drawing.Point(92, 1);
            this.closeBox.Name = "closeBox";
            this.closeBox.Size = new System.Drawing.Size(16, 16);
            this.closeBox.TabIndex = 5;
            this.closeBox.TabStop = false;
            this.closeBox.Click += new System.EventHandler(this.closeBox_Click);
            // 
            // OpacityPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(108, 29);
            this.Controls.Add(this.closeBox);
            this.Controls.Add(this.valueSlider);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "OpacityPopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "OpacityPopup";
            this.Load += new System.EventHandler(this.OpacityPopup_Load);
            this.Click += new System.EventHandler(this.OpacityPopup_Click);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OpacityPopup_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private WwtTrackBar valueSlider;
        private System.Windows.Forms.PictureBox closeBox;
    }
}