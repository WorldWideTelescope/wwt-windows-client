namespace TerraViewer
{
    partial class DataWizardColorMap
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataWizardColorMap));
            this.ColorMapTypeLabel = new System.Windows.Forms.Label();
            this.ColorMapType = new TerraViewer.WwtCombo();
            this.ColorMapColumn = new TerraViewer.WwtCombo();
            this.label1 = new System.Windows.Forms.Label();
            this.domainList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ColorMapTypeLabel
            // 
            this.ColorMapTypeLabel.AutoSize = true;
            this.ColorMapTypeLabel.Location = new System.Drawing.Point(13, 69);
            this.ColorMapTypeLabel.Name = "ColorMapTypeLabel";
            this.ColorMapTypeLabel.Size = new System.Drawing.Size(82, 13);
            this.ColorMapTypeLabel.TabIndex = 1;
            this.ColorMapTypeLabel.Text = "Color Map Type";
            // 
            // ColorMapType
            // 
            this.ColorMapType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ColorMapType.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.ColorMapType.Filter = TerraViewer.Classification.Unfiltered;
            this.ColorMapType.FilterStyle = false;
            this.ColorMapType.Location = new System.Drawing.Point(16, 85);
            this.ColorMapType.Margin = new System.Windows.Forms.Padding(0);
            this.ColorMapType.MaximumSize = new System.Drawing.Size(248, 33);
            this.ColorMapType.MinimumSize = new System.Drawing.Size(35, 33);
            this.ColorMapType.Name = "ColorMapType";
            this.ColorMapType.SelectedIndex = -1;
            this.ColorMapType.SelectedItem = null;
            this.ColorMapType.Size = new System.Drawing.Size(126, 33);
            this.ColorMapType.State = TerraViewer.State.Rest;
            this.ColorMapType.TabIndex = 2;
            this.ColorMapType.Type = TerraViewer.WwtCombo.ComboType.List;
            this.ColorMapType.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.ColorMapType_SelectionChanged);
            // 
            // ColorMapColumn
            // 
            this.ColorMapColumn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ColorMapColumn.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.ColorMapColumn.Filter = TerraViewer.Classification.Unfiltered;
            this.ColorMapColumn.FilterStyle = false;
            this.ColorMapColumn.Location = new System.Drawing.Point(158, 85);
            this.ColorMapColumn.Margin = new System.Windows.Forms.Padding(0);
            this.ColorMapColumn.MaximumSize = new System.Drawing.Size(248, 33);
            this.ColorMapColumn.MinimumSize = new System.Drawing.Size(35, 33);
            this.ColorMapColumn.Name = "ColorMapColumn";
            this.ColorMapColumn.SelectedIndex = -1;
            this.ColorMapColumn.SelectedItem = null;
            this.ColorMapColumn.Size = new System.Drawing.Size(126, 33);
            this.ColorMapColumn.State = TerraViewer.State.Rest;
            this.ColorMapColumn.TabIndex = 4;
            this.ColorMapColumn.Type = TerraViewer.WwtCombo.ComboType.List;
            this.ColorMapColumn.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.ColorMapColumn_SelectionChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(155, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Color Map Column";
            // 
            // domainList
            // 
            this.domainList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.domainList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.domainList.ForeColor = System.Drawing.Color.White;
            this.domainList.FormattingEnabled = true;
            this.domainList.ItemHeight = 36;
            this.domainList.Location = new System.Drawing.Point(398, 69);
            this.domainList.Name = "domainList";
            this.domainList.Size = new System.Drawing.Size(279, 148);
            this.domainList.TabIndex = 5;
            this.domainList.Visible = false;
            this.domainList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
            this.domainList.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox1_MeasureItem);
            this.domainList.SelectedIndexChanged += new System.EventHandler(this.domainList_SelectedIndexChanged);
            this.domainList.DoubleClick += new System.EventHandler(this.domainList_DoubleClick);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(655, 38);
            this.label2.TabIndex = 0;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // DataWizardColorMap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.domainList);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ColorMapTypeLabel);
            this.Controls.Add(this.ColorMapColumn);
            this.Controls.Add(this.ColorMapType);
            this.Name = "DataWizardColorMap";
            this.Load += new System.EventHandler(this.DataWizardColorMap_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ColorMapTypeLabel;
        private WwtCombo ColorMapType;
        private WwtCombo ColorMapColumn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox domainList;
        private System.Windows.Forms.Label label2;
    }
}
