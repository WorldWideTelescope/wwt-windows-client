namespace TerraViewer
{
    partial class TrackButton
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
            this.label = new System.Windows.Forms.Label();
            this.slider = new TerraViewer.WwtTrackBar();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(0, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(140, 13);
            this.label.TabIndex = 1;
            this.label.Text = "Slider";
            this.label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // slider
            // 
            this.slider.BackColor = System.Drawing.Color.Transparent;
            this.slider.Location = new System.Drawing.Point(29, 13);
            this.slider.Max = 100;
            this.slider.Name = "slider";
            this.slider.Size = new System.Drawing.Size(80, 20);
            this.slider.TabIndex = 0;
            this.slider.Value = 50;
            this.slider.ValueChanged += new System.EventHandler(this.slider_ValueChanged);
            // 
            // TrackButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.label);
            this.Controls.Add(this.slider);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "TrackButton";
            this.Size = new System.Drawing.Size(140, 33);
            this.Load += new System.EventHandler(this.TrackButton_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private WwtTrackBar slider;
        private System.Windows.Forms.Label label;
    }
}
