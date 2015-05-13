namespace TerraViewer
{
    partial class TabForm
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
            this.components = new System.ComponentModel.Container();
            this.fadeTimer = new System.Windows.Forms.Timer(this.components);
            this.pinUp = new TerraViewer.PinUp();
            this.SuspendLayout();
            // 
            // fadeTimer
            // 
            this.fadeTimer.Enabled = true;
            this.fadeTimer.Interval = 10;
            this.fadeTimer.Tick += new System.EventHandler(this.fadeTimer_Tick);
            // 
            // pinUp
            // 
            this.pinUp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.pinUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(49)))), ((int)(((byte)(73)))));
            this.pinUp.Direction = TerraViewer.Direction.Expanding;
            this.pinUp.Location = new System.Drawing.Point(468, 94);
            this.pinUp.Margin = new System.Windows.Forms.Padding(0);
            this.pinUp.MaximumSize = new System.Drawing.Size(33, 15);
            this.pinUp.MinimumSize = new System.Drawing.Size(33, 15);
            this.pinUp.Name = "pinUp";
            this.pinUp.Placement = TerraViewer.Placement.Top;
            this.pinUp.Size = new System.Drawing.Size(33, 15);
            this.pinUp.State = TerraViewer.State.Rest;
            this.pinUp.TabIndex = 0;
            this.pinUp.Clicked += new TerraViewer.ClickedEventHandler(this.pinUp_Clicked);
            this.pinUp.Resize += new System.EventHandler(this.pinUp_Resize);
            // 
            // TabForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(39)))));
            this.ClientSize = new System.Drawing.Size(975, 110);
            this.ControlBox = false;
            this.Controls.Add(this.pinUp);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TabForm";
            this.Opacity = 0.8;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TabForm";
            this.ResizeBegin += new System.EventHandler(this.TabForm_ResizeBegin);
            this.MouseEnter += new System.EventHandler(this.TabForm_MouseEnter);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TabForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TabForm_KeyDown);
            this.MouseHover += new System.EventHandler(this.TabForm_MouseHover);
            this.Load += new System.EventHandler(this.TabForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        protected PinUp pinUp;
        private System.Windows.Forms.Timer fadeTimer;

    }
}