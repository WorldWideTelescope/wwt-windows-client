namespace TerraViewer
{
    partial class ScreenBroadcast
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.Broadcast = new TerraViewer.WWTCheckbox();
            this.FrameRate = new TerraViewer.WwtCombo();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Alt = new System.Windows.Forms.TextBox();
            this.Az = new System.Windows.Forms.TextBox();
            this.scalelabel = new System.Windows.Forms.Label();
            this.Scale = new System.Windows.Forms.TextBox();
            this.ShowLocally = new TerraViewer.WWTCheckbox();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Broadcast
            // 
            this.Broadcast.Checked = false;
            this.Broadcast.Location = new System.Drawing.Point(10, 119);
            this.Broadcast.Name = "Broadcast";
            this.Broadcast.Size = new System.Drawing.Size(137, 25);
            this.Broadcast.TabIndex = 0;
            this.Broadcast.Text = "Broadcast Screen";
            this.Broadcast.CheckedChanged += new System.EventHandler(this.Broadcast_CheckedChanged);
            // 
            // FrameRate
            // 
            this.FrameRate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.FrameRate.DateTimeValue = new System.DateTime(2011, 6, 3, 10, 0, 0, 107);
            this.FrameRate.Filter = TerraViewer.Classification.Unfiltered;
            this.FrameRate.FilterStyle = false;
            this.FrameRate.Location = new System.Drawing.Point(15, 83);
            this.FrameRate.Margin = new System.Windows.Forms.Padding(0);
            this.FrameRate.MasterTime = true;
            this.FrameRate.MaximumSize = new System.Drawing.Size(248, 33);
            this.FrameRate.MinimumSize = new System.Drawing.Size(35, 33);
            this.FrameRate.Name = "FrameRate";
            this.FrameRate.SelectedIndex = -1;
            this.FrameRate.SelectedItem = null;
            this.FrameRate.Size = new System.Drawing.Size(248, 33);
            this.FrameRate.State = TerraViewer.State.Rest;
            this.FrameRate.TabIndex = 1;
            this.FrameRate.Type = TerraViewer.WwtCombo.ComboType.List;
            this.FrameRate.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.FrameRate_SelectionChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Frame Rate";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(260, 61);
            this.label2.TabIndex = 2;
            this.label2.Text = "When used in a Multi-Channel environment you can broadcast the screen contents fr" +
    "om local applications to show up on the projected display as a window.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 147);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Altitude";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(57, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Azimuth";
            // 
            // Alt
            // 
            this.Alt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Alt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Alt.ForeColor = System.Drawing.Color.White;
            this.Alt.Location = new System.Drawing.Point(15, 163);
            this.Alt.Name = "Alt";
            this.Alt.Size = new System.Drawing.Size(40, 20);
            this.Alt.TabIndex = 6;
            this.Alt.Text = "45";
            this.Alt.TextChanged += new System.EventHandler(this.Alt_TextChanged);
            // 
            // Az
            // 
            this.Az.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Az.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Az.ForeColor = System.Drawing.Color.White;
            this.Az.Location = new System.Drawing.Point(61, 163);
            this.Az.Name = "Az";
            this.Az.Size = new System.Drawing.Size(40, 20);
            this.Az.TabIndex = 6;
            this.Az.Text = "0";
            this.Az.TextChanged += new System.EventHandler(this.Az_TextChanged);
            // 
            // scalelabel
            // 
            this.scalelabel.AutoSize = true;
            this.scalelabel.Location = new System.Drawing.Point(103, 147);
            this.scalelabel.Name = "scalelabel";
            this.scalelabel.Size = new System.Drawing.Size(34, 13);
            this.scalelabel.TabIndex = 2;
            this.scalelabel.Text = "Scale";
            // 
            // Scale
            // 
            this.Scale.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.Scale.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Scale.ForeColor = System.Drawing.Color.White;
            this.Scale.Location = new System.Drawing.Point(107, 163);
            this.Scale.Name = "Scale";
            this.Scale.Size = new System.Drawing.Size(40, 20);
            this.Scale.TabIndex = 6;
            this.Scale.Text = "60";
            this.Scale.TextChanged += new System.EventHandler(this.Scale_TextChanged);
            // 
            // ShowLocally
            // 
            this.ShowLocally.Checked = false;
            this.ShowLocally.Location = new System.Drawing.Point(153, 119);
            this.ShowLocally.Name = "ShowLocally";
            this.ShowLocally.Size = new System.Drawing.Size(119, 25);
            this.ShowLocally.TabIndex = 0;
            this.ShowLocally.Text = "Show on Console";
            this.ShowLocally.CheckedChanged += new System.EventHandler(this.ShowLocally_CheckedChanged);
            // 
            // ScreenBroadcast
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(284, 209);
            this.Controls.Add(this.Scale);
            this.Controls.Add(this.Az);
            this.Controls.Add(this.Alt);
            this.Controls.Add(this.scalelabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FrameRate);
            this.Controls.Add(this.ShowLocally);
            this.Controls.Add(this.Broadcast);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScreenBroadcast";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ScreenBroadcast";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScreenBroadcast_FormClosed);
            this.Load += new System.EventHandler(this.ScreenBroadcast_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private WWTCheckbox Broadcast;
        private WwtCombo FrameRate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Alt;
        private System.Windows.Forms.TextBox Az;
        private System.Windows.Forms.Label scalelabel;
        private System.Windows.Forms.TextBox Scale;
        private WWTCheckbox ShowLocally;
    }
}