namespace TerraViewer
{
    partial class WwtUpDown
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
            this.up = new TerraViewer.ArrowButton();
            this.down = new TerraViewer.ArrowButton();
            this.SuspendLayout();
            // 
            // up
            // 
            this.up.ButtonSize = TerraViewer.ArrowButton.ButtonSizes.Small;
            this.up.ButtonType = TerraViewer.ArrowButton.ButtonTypes.Up;
            this.up.Location = new System.Drawing.Point(0, 0);
            this.up.MaximumSize = new System.Drawing.Size(34, 19);
            this.up.MinimumSize = new System.Drawing.Size(33, 15);
            this.up.Name = "up";
            this.up.Size = new System.Drawing.Size(33, 15);
            this.up.TabIndex = 0;
            this.up.Pushed += new System.EventHandler(this.up_Pushed);
            // 
            // down
            // 
            this.down.ButtonSize = TerraViewer.ArrowButton.ButtonSizes.Small;
            this.down.ButtonType = TerraViewer.ArrowButton.ButtonTypes.Down;
            this.down.Location = new System.Drawing.Point(0, 16);
            this.down.MaximumSize = new System.Drawing.Size(34, 19);
            this.down.MinimumSize = new System.Drawing.Size(33, 15);
            this.down.Name = "down";
            this.down.Size = new System.Drawing.Size(33, 15);
            this.down.TabIndex = 0;
            this.down.Pushed += new System.EventHandler(this.down_Pushed);
            // 
            // WwtUpDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.down);
            this.Controls.Add(this.up);
            this.MaximumSize = new System.Drawing.Size(33, 30);
            this.MinimumSize = new System.Drawing.Size(33, 30);
            this.Name = "WwtUpDown";
            this.Size = new System.Drawing.Size(33, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private ArrowButton up;
        private ArrowButton down;

    }
}
