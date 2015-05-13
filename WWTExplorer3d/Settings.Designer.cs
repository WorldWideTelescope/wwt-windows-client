namespace TerraViewer
{
    partial class Settings
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabConstellations = new System.Windows.Forms.TabPage();
            this.HighLightedColorBox = new System.Windows.Forms.PictureBox();
            this.FiguresColorBox = new System.Windows.Forms.PictureBox();
            this.BoundryColorBox = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabData = new System.Windows.Forms.TabPage();
            this.tabPlanet = new System.Windows.Forms.TabPage();
            this.OK = new System.Windows.Forms.Button();
            this.Apply = new System.Windows.Forms.Button();
            this.Default = new System.Windows.Forms.Button();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.tabControl1.SuspendLayout();
            this.tabConstellations.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HighLightedColorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FiguresColorBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BoundryColorBox)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabConstellations);
            this.tabControl1.Controls.Add(this.tabData);
            this.tabControl1.Controls.Add(this.tabPlanet);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(433, 406);
            this.tabControl1.TabIndex = 0;
            // 
            // tabConstellations
            // 
            this.tabConstellations.Controls.Add(this.HighLightedColorBox);
            this.tabConstellations.Controls.Add(this.FiguresColorBox);
            this.tabConstellations.Controls.Add(this.BoundryColorBox);
            this.tabConstellations.Controls.Add(this.label3);
            this.tabConstellations.Controls.Add(this.label2);
            this.tabConstellations.Controls.Add(this.label1);
            this.tabConstellations.Location = new System.Drawing.Point(4, 22);
            this.tabConstellations.Name = "tabConstellations";
            this.tabConstellations.Padding = new System.Windows.Forms.Padding(3);
            this.tabConstellations.Size = new System.Drawing.Size(425, 380);
            this.tabConstellations.TabIndex = 0;
            this.tabConstellations.Text = "Constellation";
            this.tabConstellations.UseVisualStyleBackColor = true;
            this.tabConstellations.Click += new System.EventHandler(this.tabConstellations_Click);
            // 
            // HighLightedColorBox
            // 
            this.HighLightedColorBox.Location = new System.Drawing.Point(10, 128);
            this.HighLightedColorBox.Name = "HighLightedColorBox";
            this.HighLightedColorBox.Size = new System.Drawing.Size(166, 23);
            this.HighLightedColorBox.TabIndex = 3;
            this.HighLightedColorBox.TabStop = false;
            this.HighLightedColorBox.Click += new System.EventHandler(this.HighLightedColorBox_Click);
            this.HighLightedColorBox.Paint += new System.Windows.Forms.PaintEventHandler(this.HighLightedColorBox_Paint);
            // 
            // FiguresColorBox
            // 
            this.FiguresColorBox.Location = new System.Drawing.Point(10, 72);
            this.FiguresColorBox.Name = "FiguresColorBox";
            this.FiguresColorBox.Size = new System.Drawing.Size(166, 23);
            this.FiguresColorBox.TabIndex = 3;
            this.FiguresColorBox.TabStop = false;
            this.FiguresColorBox.Click += new System.EventHandler(this.FiguresColorBox_Click);
            this.FiguresColorBox.Paint += new System.Windows.Forms.PaintEventHandler(this.FiguresColorBox_Paint);
            // 
            // BoundryColorBox
            // 
            this.BoundryColorBox.Location = new System.Drawing.Point(10, 24);
            this.BoundryColorBox.Name = "BoundryColorBox";
            this.BoundryColorBox.Size = new System.Drawing.Size(166, 23);
            this.BoundryColorBox.TabIndex = 3;
            this.BoundryColorBox.TabStop = false;
            this.BoundryColorBox.Click += new System.EventHandler(this.BoundryColorBox_Click);
            this.BoundryColorBox.Paint += new System.Windows.Forms.PaintEventHandler(this.BoundryColorBox_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Highlighted Color";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Figures Color";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Boundry Color";
            // 
            // tabData
            // 
            this.tabData.Location = new System.Drawing.Point(4, 22);
            this.tabData.Name = "tabData";
            this.tabData.Padding = new System.Windows.Forms.Padding(3);
            this.tabData.Size = new System.Drawing.Size(425, 380);
            this.tabData.TabIndex = 1;
            this.tabData.Text = "Data";
            this.tabData.UseVisualStyleBackColor = true;
            // 
            // tabPlanet
            // 
            this.tabPlanet.Location = new System.Drawing.Point(4, 22);
            this.tabPlanet.Name = "tabPlanet";
            this.tabPlanet.Size = new System.Drawing.Size(425, 380);
            this.tabPlanet.TabIndex = 2;
            this.tabPlanet.Text = "Planet";
            this.tabPlanet.UseVisualStyleBackColor = true;
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Location = new System.Drawing.Point(204, 425);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 1;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Apply
            // 
            this.Apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Apply.Location = new System.Drawing.Point(366, 425);
            this.Apply.Name = "Apply";
            this.Apply.Size = new System.Drawing.Size(75, 23);
            this.Apply.TabIndex = 1;
            this.Apply.Text = "Apply";
            this.Apply.UseVisualStyleBackColor = true;
            this.Apply.Click += new System.EventHandler(this.Apply_Click);
            // 
            // Default
            // 
            this.Default.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Default.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.Default.Location = new System.Drawing.Point(285, 425);
            this.Default.Name = "Default";
            this.Default.Size = new System.Drawing.Size(75, 23);
            this.Default.TabIndex = 1;
            this.Default.Text = "Default";
            this.Default.UseVisualStyleBackColor = true;
            this.Default.Click += new System.EventHandler(this.Default_Click);
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 458);
            this.Controls.Add(this.Apply);
            this.Controls.Add(this.Default);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.Settings_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabConstellations.ResumeLayout(false);
            this.tabConstellations.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HighLightedColorBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FiguresColorBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BoundryColorBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabConstellations;
        private System.Windows.Forms.TabPage tabData;
        private System.Windows.Forms.TabPage tabPlanet;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Apply;
        private System.Windows.Forms.Button Default;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox HighLightedColorBox;
        private System.Windows.Forms.PictureBox FiguresColorBox;
        private System.Windows.Forms.PictureBox BoundryColorBox;
    }
}