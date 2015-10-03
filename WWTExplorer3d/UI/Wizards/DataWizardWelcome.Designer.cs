namespace TerraViewer
{
    partial class DataWizardWelcome : PropPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataWizardWelcome));
            this.spreadsheetNameEdit = new System.Windows.Forms.TextBox();
            this.spreadsheetNameLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.coordinatesType = new TerraViewer.WwtCombo();
            this.downloadUrl = new System.Windows.Forms.TextBox();
            this.dataUrlLabel = new System.Windows.Forms.Label();
            this.autoUpdateCheckbox = new TerraViewer.WWTCheckbox();
            this.SuspendLayout();
            // 
            // spreadsheetNameEdit
            // 
            this.spreadsheetNameEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.spreadsheetNameEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.spreadsheetNameEdit.ForeColor = System.Drawing.Color.White;
            this.spreadsheetNameEdit.Location = new System.Drawing.Point(22, 117);
            this.spreadsheetNameEdit.Name = "spreadsheetNameEdit";
            this.spreadsheetNameEdit.Size = new System.Drawing.Size(139, 20);
            this.spreadsheetNameEdit.TabIndex = 2;
            this.spreadsheetNameEdit.TextChanged += new System.EventHandler(this.spreadsheetNameEdit_TextChanged);
            // 
            // spreadsheetNameLabel
            // 
            this.spreadsheetNameLabel.AutoSize = true;
            this.spreadsheetNameLabel.Location = new System.Drawing.Point(19, 98);
            this.spreadsheetNameLabel.Name = "spreadsheetNameLabel";
            this.spreadsheetNameLabel.Size = new System.Drawing.Size(64, 13);
            this.spreadsheetNameLabel.TabIndex = 1;
            this.spreadsheetNameLabel.Text = "Layer Name";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(19, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(645, 50);
            this.label2.TabIndex = 0;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(180, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "CoordinatesType";
            // 
            // coordinatesType
            // 
            this.coordinatesType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.coordinatesType.DateTimeValue = new System.DateTime(2010, 5, 14, 16, 0, 4, 793);
            this.coordinatesType.Filter = TerraViewer.Classification.Unfiltered;
            this.coordinatesType.FilterStyle = false;
            this.coordinatesType.Location = new System.Drawing.Point(183, 110);
            this.coordinatesType.Margin = new System.Windows.Forms.Padding(0);
            this.coordinatesType.MasterTime = true;
            this.coordinatesType.MaximumSize = new System.Drawing.Size(248, 33);
            this.coordinatesType.MinimumSize = new System.Drawing.Size(35, 33);
            this.coordinatesType.Name = "coordinatesType";
            this.coordinatesType.SelectedIndex = -1;
            this.coordinatesType.SelectedItem = null;
            this.coordinatesType.Size = new System.Drawing.Size(163, 33);
            this.coordinatesType.State = TerraViewer.State.Rest;
            this.coordinatesType.TabIndex = 4;
            this.coordinatesType.Type = TerraViewer.WwtCombo.ComboType.List;
            this.coordinatesType.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.coordinatesType_SelectionChanged);
            // 
            // downloadUrl
            // 
            this.downloadUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.downloadUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.downloadUrl.ForeColor = System.Drawing.Color.White;
            this.downloadUrl.Location = new System.Drawing.Point(22, 178);
            this.downloadUrl.Name = "downloadUrl";
            this.downloadUrl.Size = new System.Drawing.Size(462, 20);
            this.downloadUrl.TabIndex = 2;
            this.downloadUrl.TextChanged += new System.EventHandler(this.downloadUrl_TextChanged);
            // 
            // dataUrlLabel
            // 
            this.dataUrlLabel.AutoSize = true;
            this.dataUrlLabel.Location = new System.Drawing.Point(19, 162);
            this.dataUrlLabel.Name = "dataUrlLabel";
            this.dataUrlLabel.Size = new System.Drawing.Size(83, 13);
            this.dataUrlLabel.TabIndex = 5;
            this.dataUrlLabel.Text = "Data Source Url";
            // 
            // autoUpdateCheckbox
            // 
            this.autoUpdateCheckbox.Checked = false;
            this.autoUpdateCheckbox.Location = new System.Drawing.Point(512, 178);
            this.autoUpdateCheckbox.Name = "autoUpdateCheckbox";
            this.autoUpdateCheckbox.Size = new System.Drawing.Size(118, 25);
            this.autoUpdateCheckbox.TabIndex = 6;
            this.autoUpdateCheckbox.Text = "Auto Update";
            // 
            // DataWizardWelcome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.autoUpdateCheckbox);
            this.Controls.Add(this.dataUrlLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.coordinatesType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.downloadUrl);
            this.Controls.Add(this.spreadsheetNameEdit);
            this.Controls.Add(this.spreadsheetNameLabel);
            this.Name = "DataWizardWelcome";
            this.Load += new System.EventHandler(this.DataWizardWelcome_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox spreadsheetNameEdit;
        private System.Windows.Forms.Label spreadsheetNameLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private WwtCombo coordinatesType;
        private System.Windows.Forms.TextBox downloadUrl;
        private System.Windows.Forms.Label dataUrlLabel;
        private WWTCheckbox autoUpdateCheckbox;


    }
}
