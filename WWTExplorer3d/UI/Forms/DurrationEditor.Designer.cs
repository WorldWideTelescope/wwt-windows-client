namespace TerraViewer
{
    partial class DurrationEditor
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
            this.timeEdit = new System.Windows.Forms.MaskedTextBox();
            this.arrowDown = new TerraViewer.ArrowButton();
            this.arrowUp = new TerraViewer.ArrowButton();
            this.SuspendLayout();
            // 
            // timeEdit
            // 
            this.timeEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.timeEdit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.timeEdit.ForeColor = System.Drawing.Color.White;
            this.timeEdit.Location = new System.Drawing.Point(39, 2);
            this.timeEdit.Mask = "00:00.0";
            this.timeEdit.Name = "timeEdit";
            this.timeEdit.Size = new System.Drawing.Size(35, 15);
            this.timeEdit.TabIndex = 7;
            this.timeEdit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.timeEdit.Validated += new System.EventHandler(this.timeEdit_Validated);
            this.timeEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.timeEdit_KeyPress);
            this.timeEdit.KeyUp += new System.Windows.Forms.KeyEventHandler(this.timeEdit_KeyUp);
            this.timeEdit.Leave += new System.EventHandler(this.timeEdit_Leave);
            // 
            // arrowDown
            // 
            this.arrowDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arrowDown.ButtonType = TerraViewer.ArrowButton.ButtonTypes.Down;
            this.arrowDown.Location = new System.Drawing.Point(77, 0);
            this.arrowDown.MaximumSize = new System.Drawing.Size(34, 19);
            this.arrowDown.MinimumSize = new System.Drawing.Size(34, 19);
            this.arrowDown.Name = "arrowDown";
            this.arrowDown.Size = new System.Drawing.Size(34, 19);
            this.arrowDown.TabIndex = 8;
            this.arrowDown.Pushed += new System.EventHandler(this.arrowDown_Pushed);
            // 
            // arrowUp
            // 
            this.arrowUp.ButtonType = TerraViewer.ArrowButton.ButtonTypes.Up;
            this.arrowUp.Location = new System.Drawing.Point(0, 0);
            this.arrowUp.MaximumSize = new System.Drawing.Size(34, 19);
            this.arrowUp.MinimumSize = new System.Drawing.Size(34, 19);
            this.arrowUp.Name = "arrowUp";
            this.arrowUp.Size = new System.Drawing.Size(34, 19);
            this.arrowUp.TabIndex = 8;
            this.arrowUp.Pushed += new System.EventHandler(this.arrowUp_Pushed);
            // 
            // DurrationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(111, 19);
            this.ControlBox = false;
            this.Controls.Add(this.arrowDown);
            this.Controls.Add(this.arrowUp);
            this.Controls.Add(this.timeEdit);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(111, 19);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(111, 19);
            this.Name = "DurrationEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.DurrationEditor_Deactivate);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DurrationEditor_FormClosed);
            this.Leave += new System.EventHandler(this.DurrationEditor_Leave);
            this.Load += new System.EventHandler(this.DurrationEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.MaskedTextBox timeEdit;
        private ArrowButton arrowUp;
        private ArrowButton arrowDown;
    }
}