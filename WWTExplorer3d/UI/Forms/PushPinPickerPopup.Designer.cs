namespace TerraViewer
{
    partial class PushPinPickerPopup
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
            this.vertScroll = new System.Windows.Forms.VScrollBar();
            this.SuspendLayout();
            // 
            // vertScroll
            // 
            this.vertScroll.Dock = System.Windows.Forms.DockStyle.Right;
            this.vertScroll.LargeChange = 9;
            this.vertScroll.Location = new System.Drawing.Point(293, 0);
            this.vertScroll.Maximum = 8;
            this.vertScroll.Name = "vertScroll";
            this.vertScroll.Size = new System.Drawing.Size(17, 185);
            this.vertScroll.SmallChange = 8;
            this.vertScroll.TabIndex = 0;
            this.vertScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vertScroll_Scroll);
            // 
            // PushPinPickerPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(310, 185);
            this.ControlBox = false;
            this.Controls.Add(this.vertScroll);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PushPinPickerPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PushPinPickerPopup";
            this.Load += new System.EventHandler(this.PushPinPickerPopup_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PushPinPickerPopup_MouseUp);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.PushPinPickerPopup_Paint);
            this.Click += new System.EventHandler(this.PushPinPickerPopup_Click);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PushPinPickerPopup_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PushPinPickerPopup_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar vertScroll;
    }
}