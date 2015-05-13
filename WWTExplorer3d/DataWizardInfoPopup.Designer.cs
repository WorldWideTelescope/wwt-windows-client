namespace TerraViewer
{
    partial class DataWizardInfoPopup
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nameColumnCombo = new TerraViewer.WwtCombo();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(652, 33);
            this.label1.TabIndex = 0;
            this.label1.Text = "You can select which data column you want shown when hovering over a marker, and " +
                "optionally add a hyperlink to allow a user to drill into related data thru a web" +
                " page.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Hover Text Column";
            // 
            // nameColumnCombo
            // 
            this.nameColumnCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.nameColumnCombo.DateTimeValue = new System.DateTime(2009, 12, 11, 8, 2, 12, 711);
            this.nameColumnCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.nameColumnCombo.FilterStyle = false;
            this.nameColumnCombo.Location = new System.Drawing.Point(15, 65);
            this.nameColumnCombo.Margin = new System.Windows.Forms.Padding(0);
            this.nameColumnCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.nameColumnCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.nameColumnCombo.Name = "nameColumnCombo";
            this.nameColumnCombo.SelectedIndex = -1;
            this.nameColumnCombo.SelectedItem = null;
            this.nameColumnCombo.Size = new System.Drawing.Size(126, 33);
            this.nameColumnCombo.State = TerraViewer.State.Rest;
            this.nameColumnCombo.TabIndex = 2;
            this.nameColumnCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.nameColumnCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.nameColumnCombo_SelectionChanged);
            // 
            // DataWizardInfoPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nameColumnCombo);
            this.Controls.Add(this.label1);
            this.Name = "DataWizardInfoPopup";
            this.Load += new System.EventHandler(this.DataWizardInfoPopup_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private WwtCombo nameColumnCombo;
    }
}
