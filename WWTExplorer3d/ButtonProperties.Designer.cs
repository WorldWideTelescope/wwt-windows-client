namespace TerraViewer
{
    partial class ButtonProperties
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
            this.Cancel = new TerraViewer.WwtButton();
            this.OK = new TerraViewer.WwtButton();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BindTypeCombo = new TerraViewer.WwtCombo();
            this.TargetPropertyCombo = new TerraViewer.WwtCombo();
            this.TargetTypeCombo = new TerraViewer.WwtCombo();
            this.Property = new System.Windows.Forms.Label();
            this.BindTypeLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.PropertyNameText = new System.Windows.Forms.TextBox();
            this.buttonTypeCombo = new TerraViewer.WwtCombo();
            this.buttonTypeLabel = new System.Windows.Forms.Label();
            this.filterList = new TerraViewer.WwtCombo();
            this.filterLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(190, 251);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(72, 33);
            this.Cancel.TabIndex = 12;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.None;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(115, 251);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(73, 33);
            this.OK.TabIndex = 11;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // txtName
            // 
            this.txtName.AcceptsReturn = true;
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.Location = new System.Drawing.Point(12, 25);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(248, 22);
            this.txtName.TabIndex = 1;
            this.txtName.Text = "<Type name here>";
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // BindTypeCombo
            // 
            this.BindTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.BindTypeCombo.DateTimeValue = new System.DateTime(2012, 7, 3, 15, 36, 18, 947);
            this.BindTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.BindTypeCombo.FilterStyle = false;
            this.BindTypeCombo.Location = new System.Drawing.Point(150, 110);
            this.BindTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.BindTypeCombo.MasterTime = true;
            this.BindTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.BindTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.BindTypeCombo.Name = "BindTypeCombo";
            this.BindTypeCombo.SelectedIndex = -1;
            this.BindTypeCombo.SelectedItem = null;
            this.BindTypeCombo.Size = new System.Drawing.Size(110, 33);
            this.BindTypeCombo.State = TerraViewer.State.Rest;
            this.BindTypeCombo.TabIndex = 7;
            this.BindTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.BindTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.BindTypeCombo_SelectionChanged);
            // 
            // TargetPropertyCombo
            // 
            this.TargetPropertyCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.TargetPropertyCombo.DateTimeValue = new System.DateTime(2012, 7, 3, 15, 3, 58, 312);
            this.TargetPropertyCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.TargetPropertyCombo.FilterStyle = false;
            this.TargetPropertyCombo.Location = new System.Drawing.Point(12, 156);
            this.TargetPropertyCombo.Margin = new System.Windows.Forms.Padding(0);
            this.TargetPropertyCombo.MasterTime = true;
            this.TargetPropertyCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.TargetPropertyCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.TargetPropertyCombo.Name = "TargetPropertyCombo";
            this.TargetPropertyCombo.SelectedIndex = -1;
            this.TargetPropertyCombo.SelectedItem = null;
            this.TargetPropertyCombo.Size = new System.Drawing.Size(248, 33);
            this.TargetPropertyCombo.State = TerraViewer.State.Rest;
            this.TargetPropertyCombo.TabIndex = 9;
            this.TargetPropertyCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.TargetPropertyCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.TargetPropertyCombo_SelectionChanged);
            // 
            // TargetTypeCombo
            // 
            this.TargetTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.TargetTypeCombo.DateTimeValue = new System.DateTime(2012, 7, 3, 15, 3, 58, 312);
            this.TargetTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.TargetTypeCombo.FilterStyle = false;
            this.TargetTypeCombo.Location = new System.Drawing.Point(12, 110);
            this.TargetTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.TargetTypeCombo.MasterTime = true;
            this.TargetTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.TargetTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.TargetTypeCombo.Name = "TargetTypeCombo";
            this.TargetTypeCombo.SelectedIndex = -1;
            this.TargetTypeCombo.SelectedItem = null;
            this.TargetTypeCombo.Size = new System.Drawing.Size(138, 33);
            this.TargetTypeCombo.State = TerraViewer.State.Rest;
            this.TargetTypeCombo.TabIndex = 5;
            this.TargetTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.TargetTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.TargetTypeCombo_SelectionChanged);
            // 
            // Property
            // 
            this.Property.AutoSize = true;
            this.Property.Location = new System.Drawing.Point(9, 143);
            this.Property.Name = "Property";
            this.Property.Size = new System.Drawing.Size(46, 13);
            this.Property.TabIndex = 8;
            this.Property.Text = "Property";
            // 
            // BindTypeLabel
            // 
            this.BindTypeLabel.AutoSize = true;
            this.BindTypeLabel.Location = new System.Drawing.Point(147, 97);
            this.BindTypeLabel.Name = "BindTypeLabel";
            this.BindTypeLabel.Size = new System.Drawing.Size(55, 13);
            this.BindTypeLabel.TabIndex = 6;
            this.BindTypeLabel.Text = "Bind Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(103, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Binding Target Type";
            // 
            // PropertyNameText
            // 
            this.PropertyNameText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.PropertyNameText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PropertyNameText.ForeColor = System.Drawing.Color.White;
            this.PropertyNameText.Location = new System.Drawing.Point(16, 162);
            this.PropertyNameText.Name = "PropertyNameText";
            this.PropertyNameText.Size = new System.Drawing.Size(222, 20);
            this.PropertyNameText.TabIndex = 10;
            this.PropertyNameText.Visible = false;
            this.PropertyNameText.TextChanged += new System.EventHandler(this.PropertyNameText_TextChanged);
            // 
            // buttonTypeCombo
            // 
            this.buttonTypeCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.buttonTypeCombo.DateTimeValue = new System.DateTime(2013, 11, 26, 13, 54, 31, 539);
            this.buttonTypeCombo.Filter = TerraViewer.Classification.Unfiltered;
            this.buttonTypeCombo.FilterStyle = false;
            this.buttonTypeCombo.Location = new System.Drawing.Point(12, 63);
            this.buttonTypeCombo.Margin = new System.Windows.Forms.Padding(0);
            this.buttonTypeCombo.MasterTime = true;
            this.buttonTypeCombo.MaximumSize = new System.Drawing.Size(248, 33);
            this.buttonTypeCombo.MinimumSize = new System.Drawing.Size(35, 33);
            this.buttonTypeCombo.Name = "buttonTypeCombo";
            this.buttonTypeCombo.SelectedIndex = -1;
            this.buttonTypeCombo.SelectedItem = null;
            this.buttonTypeCombo.Size = new System.Drawing.Size(248, 33);
            this.buttonTypeCombo.State = TerraViewer.State.Rest;
            this.buttonTypeCombo.TabIndex = 3;
            this.buttonTypeCombo.Type = TerraViewer.WwtCombo.ComboType.List;
            this.buttonTypeCombo.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.buttonTypeCombo_SelectionChanged);
            // 
            // buttonTypeLabel
            // 
            this.buttonTypeLabel.AutoSize = true;
            this.buttonTypeLabel.Location = new System.Drawing.Point(9, 50);
            this.buttonTypeLabel.Name = "buttonTypeLabel";
            this.buttonTypeLabel.Size = new System.Drawing.Size(65, 13);
            this.buttonTypeLabel.TabIndex = 2;
            this.buttonTypeLabel.Text = "Button Type";
            // 
            // filterList
            // 
            this.filterList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.filterList.DateTimeValue = new System.DateTime(2014, 8, 8, 8, 48, 31, 871);
            this.filterList.Filter = TerraViewer.Classification.Unfiltered;
            this.filterList.FilterStyle = false;
            this.filterList.Location = new System.Drawing.Point(12, 202);
            this.filterList.Margin = new System.Windows.Forms.Padding(0);
            this.filterList.MasterTime = true;
            this.filterList.MaximumSize = new System.Drawing.Size(248, 33);
            this.filterList.MinimumSize = new System.Drawing.Size(35, 33);
            this.filterList.Name = "filterList";
            this.filterList.SelectedIndex = -1;
            this.filterList.SelectedItem = null;
            this.filterList.Size = new System.Drawing.Size(248, 33);
            this.filterList.State = TerraViewer.State.Rest;
            this.filterList.TabIndex = 20;
            this.filterList.Type = TerraViewer.WwtCombo.ComboType.List;
            this.filterList.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.filterList_SelectionChanged);
            // 
            // filterLabel
            // 
            this.filterLabel.AutoSize = true;
            this.filterLabel.Location = new System.Drawing.Point(9, 189);
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(34, 13);
            this.filterLabel.TabIndex = 19;
            this.filterLabel.Text = "Value";
            // 
            // ButtonProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(274, 296);
            this.ControlBox = false;
            this.Controls.Add(this.filterList);
            this.Controls.Add(this.filterLabel);
            this.Controls.Add(this.buttonTypeLabel);
            this.Controls.Add(this.buttonTypeCombo);
            this.Controls.Add(this.BindTypeCombo);
            this.Controls.Add(this.TargetPropertyCombo);
            this.Controls.Add(this.TargetTypeCombo);
            this.Controls.Add(this.Property);
            this.Controls.Add(this.BindTypeLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.PropertyNameText);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ButtonProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Button Properties";
            this.Load += new System.EventHandler(this.ButtonProperties_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton Cancel;
        private WwtButton OK;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private WwtCombo BindTypeCombo;
        private WwtCombo TargetPropertyCombo;
        private WwtCombo TargetTypeCombo;
        private System.Windows.Forms.Label Property;
        private System.Windows.Forms.Label BindTypeLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PropertyNameText;
        private WwtCombo buttonTypeCombo;
        private System.Windows.Forms.Label buttonTypeLabel;
        private WwtCombo filterList;
        private System.Windows.Forms.Label filterLabel;
    }
}