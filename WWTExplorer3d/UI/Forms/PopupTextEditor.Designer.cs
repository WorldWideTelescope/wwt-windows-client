namespace TerraViewer
{
    partial class PopupTextEditor
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
            this.textEdit = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textEdit
            // 
            this.textEdit.BackColor = System.Drawing.Color.Black;
            this.textEdit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEdit.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textEdit.ForeColor = System.Drawing.Color.White;
            this.textEdit.Location = new System.Drawing.Point(0, 0);
            this.textEdit.Name = "textEdit";
            this.textEdit.Size = new System.Drawing.Size(94, 15);
            this.textEdit.TabIndex = 0;
            this.textEdit.TextChanged += new System.EventHandler(this.textEdit_TextChanged);
            // 
            // PopupTextEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(94, 13);
            this.ControlBox = false;
            this.Controls.Add(this.textEdit);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(94, 13);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(94, 13);
            this.Name = "PopupTextEditor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.PopupTextEditor_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PopupTextEditor_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PopupTextEditor_FormClosed);
            this.Load += new System.EventHandler(this.PopupTextEditor_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PopupTextEditor_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textEdit;
    }
}