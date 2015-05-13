namespace TerraViewer
{
    partial class ButtonGroupControl
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
            this.components = new System.ComponentModel.Container();
            this.EditButtons = new System.Windows.Forms.Label();
            this.Plus = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // EditButtons
            // 
            this.EditButtons.AutoSize = true;
            this.EditButtons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(51)))));
            this.EditButtons.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EditButtons.ForeColor = System.Drawing.Color.White;
            this.EditButtons.Location = new System.Drawing.Point(5, 25);
            this.EditButtons.Name = "EditButtons";
            this.EditButtons.Size = new System.Drawing.Size(15, 17);
            this.EditButtons.TabIndex = 60;
            this.EditButtons.Text = "E";
            this.toolTip1.SetToolTip(this.EditButtons, "Activates Edit Mode. In Edit mode drag buttons to arrange, right click button for" +
                    " menu.");
            this.EditButtons.Click += new System.EventHandler(this.EditButtons_Click);
            // 
            // Plus
            // 
            this.Plus.AutoSize = true;
            this.Plus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(51)))));
            this.Plus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Plus.ForeColor = System.Drawing.Color.White;
            this.Plus.Location = new System.Drawing.Point(5, 4);
            this.Plus.Name = "Plus";
            this.Plus.Size = new System.Drawing.Size(16, 16);
            this.Plus.TabIndex = 59;
            this.Plus.Text = "+";
            this.toolTip1.SetToolTip(this.Plus, "Add Custom Button");
            this.Plus.Click += new System.EventHandler(this.Plus_Click);
            // 
            // ButtonGroupControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(39)))));
            this.Controls.Add(this.EditButtons);
            this.Controls.Add(this.Plus);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "ButtonGroupControl";
            this.Size = new System.Drawing.Size(701, 150);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ButtonGroupControl_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label EditButtons;
        private System.Windows.Forms.Label Plus;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
