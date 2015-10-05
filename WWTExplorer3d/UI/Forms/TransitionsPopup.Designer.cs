namespace TerraViewer
{
    partial class TransitionsPopup
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
            this.TimeA = new System.Windows.Forms.TextBox();
            this.spreadsheetNameLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TimeB = new System.Windows.Forms.TextBox();
            this.transitionPicker = new TerraViewer.TransitionPicker();
            this.label2 = new System.Windows.Forms.Label();
            this.TimeHold = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TimeA
            // 
            this.TimeA.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.TimeA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TimeA.ForeColor = System.Drawing.Color.White;
            this.TimeA.Location = new System.Drawing.Point(36, 137);
            this.TimeA.Name = "TimeA";
            this.TimeA.Size = new System.Drawing.Size(37, 20);
            this.TimeA.TabIndex = 3;
            this.TimeA.TextChanged += new System.EventHandler(this.TimeA_TextChanged);
            // 
            // spreadsheetNameLabel
            // 
            this.spreadsheetNameLabel.AutoSize = true;
            this.spreadsheetNameLabel.Location = new System.Drawing.Point(33, 118);
            this.spreadsheetNameLabel.Name = "spreadsheetNameLabel";
            this.spreadsheetNameLabel.Size = new System.Drawing.Size(40, 13);
            this.spreadsheetNameLabel.TabIndex = 2;
            this.spreadsheetNameLabel.Text = "A Time";
            this.spreadsheetNameLabel.Click += new System.EventHandler(this.spreadsheetNameLabel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(190, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "B Time";
            // 
            // TimeB
            // 
            this.TimeB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.TimeB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TimeB.ForeColor = System.Drawing.Color.White;
            this.TimeB.Location = new System.Drawing.Point(193, 137);
            this.TimeB.Name = "TimeB";
            this.TimeB.Size = new System.Drawing.Size(37, 20);
            this.TimeB.TabIndex = 3;
            this.TimeB.TextChanged += new System.EventHandler(this.TimeB_TextChanged);
            // 
            // transitionPicker
            // 
            this.transitionPicker.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(18)))), ((int)(((byte)(25)))));
            this.transitionPicker.ForeColor = System.Drawing.Color.White;
            this.transitionPicker.Location = new System.Drawing.Point(12, 12);
            this.transitionPicker.Name = "transitionPicker";
            this.transitionPicker.SelectedIndex = 0;
            this.transitionPicker.Size = new System.Drawing.Size(268, 74);
            this.transitionPicker.TabIndex = 0;
            this.transitionPicker.Load += new System.EventHandler(this.transitionPicker_Load);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(113, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Hold Time";
            // 
            // timeHold
            // 
            this.TimeHold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.TimeHold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TimeHold.ForeColor = System.Drawing.Color.White;
            this.TimeHold.Location = new System.Drawing.Point(116, 137);
            this.TimeHold.Name = "timeHold";
            this.TimeHold.Size = new System.Drawing.Size(37, 20);
            this.TimeHold.TabIndex = 3;
            this.TimeHold.TextChanged += new System.EventHandler(this.timeHold_TextChanged);
            // 
            // TransitionsPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(38)))), ((int)(((byte)(56)))));
            this.ClientSize = new System.Drawing.Size(289, 189);
            this.Controls.Add(this.TimeB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TimeHold);
            this.Controls.Add(this.TimeA);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.spreadsheetNameLabel);
            this.Controls.Add(this.transitionPicker);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TransitionsPopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Transitions";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.TransitionsPopup_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TransitionsPopup_FormClosed);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TransitionsPopup_Paint);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TransitionsPopup_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TransitionPicker transitionPicker;
        private System.Windows.Forms.TextBox TimeA;
        private System.Windows.Forms.Label spreadsheetNameLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TimeB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TimeHold;
    }
}