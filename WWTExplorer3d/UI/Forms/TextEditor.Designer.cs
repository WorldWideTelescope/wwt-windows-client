namespace TerraViewer
{
    partial class TextEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEditor));
            this.textBox = new System.Windows.Forms.TextBox();
            this.ToolBar = new System.Windows.Forms.ToolStrip();
            this.SaveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.FontName = new System.Windows.Forms.ToolStripComboBox();
            this.FontSize = new System.Windows.Forms.ToolStripComboBox();
            this.FontBold = new System.Windows.Forms.ToolStripButton();
            this.FontItalic = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.FontColor = new System.Windows.Forms.ToolStripSplitButton();
            this.BackgroundColor = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.BackgroundStyle = new System.Windows.Forms.ToolStripDropDownButton();
            this.noBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tightFitBackgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallBorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largerBoarderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripInsertItems = new System.Windows.Forms.ToolStripDropDownButton();
            this.dateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.latitudeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.longitudeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fieldOfViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.BackColor = System.Drawing.Color.Black;
            this.textBox.ForeColor = System.Drawing.Color.White;
            this.textBox.Location = new System.Drawing.Point(-1, 28);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(587, 264);
            this.textBox.TabIndex = 0;
            // 
            // ToolBar
            // 
            this.ToolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveButton,
            this.toolStripSeparator1,
            this.FontName,
            this.FontSize,
            this.FontBold,
            this.FontItalic,
            this.toolStripSeparator3,
            this.FontColor,
            this.BackgroundColor,
            this.toolStripSeparator4,
            this.BackgroundStyle,
            this.toolStripInsertItems});
            this.ToolBar.Location = new System.Drawing.Point(0, 0);
            this.ToolBar.Name = "ToolBar";
            this.ToolBar.Size = new System.Drawing.Size(586, 25);
            this.ToolBar.TabIndex = 0;
            this.ToolBar.Text = "toolStrip1";
            this.ToolBar.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ToolBar_ItemClicked);
            // 
            // SaveButton
            // 
            this.SaveButton.Image = global::TerraViewer.Properties.Resources.saveHS;
            this.SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(51, 22);
            this.SaveButton.Text = "Save";
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // FontName
            // 
            this.FontName.DropDownHeight = 250;
            this.FontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FontName.IntegralHeight = false;
            this.FontName.Name = "FontName";
            this.FontName.Size = new System.Drawing.Size(140, 25);
            this.FontName.SelectedIndexChanged += new System.EventHandler(this.FontName_SelectedIndexChanged);
            // 
            // FontSize
            // 
            this.FontSize.Items.AddRange(new object[] {
            "8",
            "9",
            "10",
            "11",
            "12",
            "14",
            "16",
            "18",
            "20",
            "24",
            "28",
            "32",
            "36",
            "40",
            "44",
            "48",
            "54",
            "60",
            "66",
            "72",
            "80",
            "88",
            "96",
            "128",
            "150",
            "200"});
            this.FontSize.Name = "FontSize";
            this.FontSize.Size = new System.Drawing.Size(75, 25);
            this.FontSize.Text = "Font Size";
            this.FontSize.ToolTipText = "Font Size";
            this.FontSize.TextChanged += new System.EventHandler(this.FontSize_TextChanged);
            // 
            // FontBold
            // 
            this.FontBold.CheckOnClick = true;
            this.FontBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.FontBold.Image = global::TerraViewer.Properties.Resources.boldhs;
            this.FontBold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FontBold.Name = "FontBold";
            this.FontBold.Size = new System.Drawing.Size(23, 22);
            this.FontBold.Text = "Bold";
            this.FontBold.ToolTipText = "Bold";
            this.FontBold.CheckedChanged += new System.EventHandler(this.FontBold_CheckedChanged);
            this.FontBold.Click += new System.EventHandler(this.FontBold_Click);
            // 
            // FontItalic
            // 
            this.FontItalic.CheckOnClick = true;
            this.FontItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.FontItalic.Image = global::TerraViewer.Properties.Resources.ItalicHS;
            this.FontItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FontItalic.Name = "FontItalic";
            this.FontItalic.Size = new System.Drawing.Size(23, 22);
            this.FontItalic.Text = "Italic";
            this.FontItalic.CheckedChanged += new System.EventHandler(this.FontItalic_CheckedChanged);
            this.FontItalic.Click += new System.EventHandler(this.FontItalic_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // FontColor
            // 
            this.FontColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.FontColor.Image = global::TerraViewer.Properties.Resources.Color_font;
            this.FontColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.FontColor.Name = "FontColor";
            this.FontColor.Size = new System.Drawing.Size(32, 22);
            this.FontColor.Text = "Text Color";
            this.FontColor.Click += new System.EventHandler(this.FontColor_Click);
            // 
            // BackgroundColor
            // 
            this.BackgroundColor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BackgroundColor.Image = global::TerraViewer.Properties.Resources.ColorHS;
            this.BackgroundColor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BackgroundColor.Name = "BackgroundColor";
            this.BackgroundColor.Size = new System.Drawing.Size(32, 22);
            this.BackgroundColor.Text = "Background Preview Color";
            this.BackgroundColor.Click += new System.EventHandler(this.BackgroundColor_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // BackgroundStyle
            // 
            this.BackgroundStyle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BackgroundStyle.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noBackgroundToolStripMenuItem,
            this.tightFitBackgroundToolStripMenuItem,
            this.smallBorderToolStripMenuItem,
            this.largerBoarderToolStripMenuItem});
            this.BackgroundStyle.Image = global::TerraViewer.Properties.Resources.tool_icon_text_24;
            this.BackgroundStyle.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BackgroundStyle.Name = "BackgroundStyle";
            this.BackgroundStyle.Size = new System.Drawing.Size(29, 22);
            this.BackgroundStyle.Text = "Text Background Options";
            this.BackgroundStyle.ToolTipText = "Backgound Options";
            this.BackgroundStyle.Click += new System.EventHandler(this.BackgroundStyle_Click);
            // 
            // noBackgroundToolStripMenuItem
            // 
            this.noBackgroundToolStripMenuItem.Name = "noBackgroundToolStripMenuItem";
            this.noBackgroundToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.noBackgroundToolStripMenuItem.Text = "No Background";
            this.noBackgroundToolStripMenuItem.Click += new System.EventHandler(this.noBackgroundToolStripMenuItem_Click);
            // 
            // tightFitBackgroundToolStripMenuItem
            // 
            this.tightFitBackgroundToolStripMenuItem.Name = "tightFitBackgroundToolStripMenuItem";
            this.tightFitBackgroundToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.tightFitBackgroundToolStripMenuItem.Text = "Tight Fit Background";
            this.tightFitBackgroundToolStripMenuItem.Click += new System.EventHandler(this.tightFitBackgroundToolStripMenuItem_Click);
            // 
            // smallBorderToolStripMenuItem
            // 
            this.smallBorderToolStripMenuItem.Name = "smallBorderToolStripMenuItem";
            this.smallBorderToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.smallBorderToolStripMenuItem.Text = "Small Border";
            this.smallBorderToolStripMenuItem.Click += new System.EventHandler(this.smallBorderToolStripMenuItem_Click);
            // 
            // largerBoarderToolStripMenuItem
            // 
            this.largerBoarderToolStripMenuItem.Name = "largerBoarderToolStripMenuItem";
            this.largerBoarderToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.largerBoarderToolStripMenuItem.Text = "Larger Border";
            this.largerBoarderToolStripMenuItem.Click += new System.EventHandler(this.largerBoarderToolStripMenuItem_Click);
            // 
            // toolStripInsertItems
            // 
            this.toolStripInsertItems.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripInsertItems.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dateToolStripMenuItem,
            this.timeToolStripMenuItem,
            this.distanceToolStripMenuItem,
            this.latitudeToolStripMenuItem,
            this.longitudeToolStripMenuItem,
            this.rAToolStripMenuItem,
            this.decToolStripMenuItem,
            this.fieldOfViewToolStripMenuItem});
            this.toolStripInsertItems.Image = ((System.Drawing.Image)(resources.GetObject("toolStripInsertItems.Image")));
            this.toolStripInsertItems.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripInsertItems.Name = "toolStripInsertItems";
            this.toolStripInsertItems.Size = new System.Drawing.Size(29, 22);
            this.toolStripInsertItems.Text = "Insert Field";
            // 
            // dateToolStripMenuItem
            // 
            this.dateToolStripMenuItem.Name = "dateToolStripMenuItem";
            this.dateToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.dateToolStripMenuItem.Text = "Date";
            this.dateToolStripMenuItem.Click += new System.EventHandler(this.dateToolStripMenuItem_Click);
            // 
            // timeToolStripMenuItem
            // 
            this.timeToolStripMenuItem.Name = "timeToolStripMenuItem";
            this.timeToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.timeToolStripMenuItem.Text = "Time";
            this.timeToolStripMenuItem.Click += new System.EventHandler(this.timeToolStripMenuItem_Click);
            // 
            // distanceToolStripMenuItem
            // 
            this.distanceToolStripMenuItem.Name = "distanceToolStripMenuItem";
            this.distanceToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.distanceToolStripMenuItem.Text = "Distance";
            this.distanceToolStripMenuItem.Click += new System.EventHandler(this.distanceToolStripMenuItem_Click);
            // 
            // latitudeToolStripMenuItem
            // 
            this.latitudeToolStripMenuItem.Name = "latitudeToolStripMenuItem";
            this.latitudeToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.latitudeToolStripMenuItem.Text = "Latitude";
            this.latitudeToolStripMenuItem.Click += new System.EventHandler(this.latitudeToolStripMenuItem_Click);
            // 
            // longitudeToolStripMenuItem
            // 
            this.longitudeToolStripMenuItem.Name = "longitudeToolStripMenuItem";
            this.longitudeToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.longitudeToolStripMenuItem.Text = "Longitude";
            this.longitudeToolStripMenuItem.Click += new System.EventHandler(this.longitudeToolStripMenuItem_Click);
            // 
            // rAToolStripMenuItem
            // 
            this.rAToolStripMenuItem.Name = "rAToolStripMenuItem";
            this.rAToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.rAToolStripMenuItem.Text = "RA";
            this.rAToolStripMenuItem.Click += new System.EventHandler(this.rAToolStripMenuItem_Click);
            // 
            // decToolStripMenuItem
            // 
            this.decToolStripMenuItem.Name = "decToolStripMenuItem";
            this.decToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.decToolStripMenuItem.Text = "Dec";
            this.decToolStripMenuItem.Click += new System.EventHandler(this.decToolStripMenuItem_Click);
            // 
            // fieldOfViewToolStripMenuItem
            // 
            this.fieldOfViewToolStripMenuItem.Name = "fieldOfViewToolStripMenuItem";
            this.fieldOfViewToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.fieldOfViewToolStripMenuItem.Text = "Field of View";
            this.fieldOfViewToolStripMenuItem.Click += new System.EventHandler(this.fieldOfViewToolStripMenuItem_Click);
            // 
            // TextEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 292);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.ToolBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 105);
            this.Name = "TextEditor";
            this.Opacity = 0.8D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Text Editor";
            this.Load += new System.EventHandler(this.TextEditor_Load);
            this.ToolBar.ResumeLayout(false);
            this.ToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.ToolStrip ToolBar;
        private System.Windows.Forms.ToolStripComboBox FontName;
        private System.Windows.Forms.ToolStripComboBox FontSize;
        private System.Windows.Forms.ToolStripButton FontBold;
        private System.Windows.Forms.ToolStripButton FontItalic;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSplitButton FontColor;
        private System.Windows.Forms.ToolStripSplitButton BackgroundColor;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton SaveButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton BackgroundStyle;
        private System.Windows.Forms.ToolStripMenuItem noBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tightFitBackgroundToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallBorderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largerBoarderToolStripMenuItem;
        private System.Windows.Forms.ToolStripDropDownButton toolStripInsertItems;
        private System.Windows.Forms.ToolStripMenuItem dateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem timeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem distanceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem latitudeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem longitudeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fieldOfViewToolStripMenuItem;
    }
}