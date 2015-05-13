namespace TerraViewer
{
    partial class PopupVolume
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
            this.volume = new System.Windows.Forms.TrackBar();
            this.ok = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.volume)).BeginInit();
            this.SuspendLayout();
            // 
            // volume
            // 
            this.volume.AutoSize = false;
            this.volume.Location = new System.Drawing.Point(7, 0);
            this.volume.Maximum = 100;
            this.volume.Name = "volume";
            this.volume.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.volume.Size = new System.Drawing.Size(34, 110);
            this.volume.TabIndex = 0;
            this.volume.TickFrequency = 10;
            this.volume.Value = 100;
            // 
            // ok
            // 
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Location = new System.Drawing.Point(3, 112);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(34, 23);
            this.ok.TabIndex = 1;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // PopupVolume
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(44, 138);
            this.ControlBox = false;
            this.Controls.Add(this.ok);
            this.Controls.Add(this.volume);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupVolume";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "PopupVolume";
            this.Load += new System.EventHandler(this.PopupVolume_Load);
            ((System.ComponentModel.ISupportInitialize)(this.volume)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar volume;
        private System.Windows.Forms.Button ok;
    }
}