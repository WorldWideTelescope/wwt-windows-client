namespace TerraViewer
{
    partial class VORegistryBrowser
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
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Enter a keyword above to search the US NVO registry for Cone Search services");
            this.ResourceList = new System.Windows.Forms.ListView();
            this.In = new System.Windows.Forms.ColumnHeader();
            this.searchUrlLabel = new System.Windows.Forms.Label();
            this.baseUrl = new System.Windows.Forms.TextBox();
            this.raLabel = new System.Windows.Forms.Label();
            this.ra = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dec = new System.Windows.Forms.TextBox();
            this.searchRadiusLabel = new System.Windows.Forms.Label();
            this.searchRadius = new System.Windows.Forms.TextBox();
            this.verbosityLabel = new System.Windows.Forms.Label();
            this.keywordLabel = new System.Windows.Forms.Label();
            this.keyword = new System.Windows.Forms.TextBox();
            this.fromView = new TerraViewer.WWTCheckbox();
            this.fromRegistry = new TerraViewer.WWTCheckbox();
            this.findRegistry = new TerraViewer.WwtButton();
            this.close = new TerraViewer.WwtButton();
            this.search = new TerraViewer.WwtButton();
            this.verbosity = new TerraViewer.WwtCombo();
            this.coneSearchCheckbox = new TerraViewer.WWTCheckbox();
            this.siapCheckbox = new TerraViewer.WWTCheckbox();
            this.SuspendLayout();
            // 
            // ResourceList
            // 
            this.ResourceList.AllowColumnReorder = true;
            this.ResourceList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ResourceList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ResourceList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ResourceList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.In});
            this.ResourceList.ForeColor = System.Drawing.Color.White;
            this.ResourceList.FullRowSelect = true;
            this.ResourceList.GridLines = true;
            this.ResourceList.HideSelection = false;
            this.ResourceList.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem2});
            this.ResourceList.Location = new System.Drawing.Point(0, 152);
            this.ResourceList.MultiSelect = false;
            this.ResourceList.Name = "ResourceList";
            this.ResourceList.Size = new System.Drawing.Size(838, 292);
            this.ResourceList.TabIndex = 1;
            this.ResourceList.UseCompatibleStateImageBehavior = false;
            this.ResourceList.View = System.Windows.Forms.View.Details;
            this.ResourceList.ItemActivate += new System.EventHandler(this.ResourceList_ItemActivate);
            this.ResourceList.SelectedIndexChanged += new System.EventHandler(this.ResourceList_SelectedIndexChanged);
            // 
            // In
            // 
            this.In.Text = "NVO Registry Search";
            this.In.Width = 1500;
            // 
            // searchUrlLabel
            // 
            this.searchUrlLabel.AutoSize = true;
            this.searchUrlLabel.Location = new System.Drawing.Point(9, 6);
            this.searchUrlLabel.Name = "searchUrlLabel";
            this.searchUrlLabel.Size = new System.Drawing.Size(56, 13);
            this.searchUrlLabel.TabIndex = 3;
            this.searchUrlLabel.Text = "Base URL";
            // 
            // baseUrl
            // 
            this.baseUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.baseUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.baseUrl.Enabled = false;
            this.baseUrl.ForeColor = System.Drawing.Color.White;
            this.baseUrl.Location = new System.Drawing.Point(12, 25);
            this.baseUrl.Name = "baseUrl";
            this.baseUrl.ReadOnly = true;
            this.baseUrl.Size = new System.Drawing.Size(280, 20);
            this.baseUrl.TabIndex = 4;
            this.baseUrl.TextChanged += new System.EventHandler(this.baseUrl_TextChanged);
            // 
            // raLabel
            // 
            this.raLabel.AutoSize = true;
            this.raLabel.Location = new System.Drawing.Point(325, 6);
            this.raLabel.Name = "raLabel";
            this.raLabel.Size = new System.Drawing.Size(22, 13);
            this.raLabel.TabIndex = 3;
            this.raLabel.Text = "RA";
            // 
            // ra
            // 
            this.ra.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ra.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ra.ForeColor = System.Drawing.Color.White;
            this.ra.Location = new System.Drawing.Point(328, 25);
            this.ra.Name = "ra";
            this.ra.ReadOnly = true;
            this.ra.Size = new System.Drawing.Size(69, 20);
            this.ra.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(400, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Dec";
            // 
            // dec
            // 
            this.dec.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dec.ForeColor = System.Drawing.Color.White;
            this.dec.Location = new System.Drawing.Point(403, 25);
            this.dec.Name = "dec";
            this.dec.ReadOnly = true;
            this.dec.Size = new System.Drawing.Size(69, 20);
            this.dec.TabIndex = 4;
            // 
            // searchRadiusLabel
            // 
            this.searchRadiusLabel.AutoSize = true;
            this.searchRadiusLabel.Location = new System.Drawing.Point(475, 6);
            this.searchRadiusLabel.Name = "searchRadiusLabel";
            this.searchRadiusLabel.Size = new System.Drawing.Size(77, 13);
            this.searchRadiusLabel.TabIndex = 3;
            this.searchRadiusLabel.Text = "Search Radius";
            // 
            // searchRadius
            // 
            this.searchRadius.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.searchRadius.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchRadius.ForeColor = System.Drawing.Color.White;
            this.searchRadius.Location = new System.Drawing.Point(478, 25);
            this.searchRadius.Name = "searchRadius";
            this.searchRadius.ReadOnly = true;
            this.searchRadius.Size = new System.Drawing.Size(69, 20);
            this.searchRadius.TabIndex = 4;
            // 
            // verbosityLabel
            // 
            this.verbosityLabel.AutoSize = true;
            this.verbosityLabel.Location = new System.Drawing.Point(553, 6);
            this.verbosityLabel.Name = "verbosityLabel";
            this.verbosityLabel.Size = new System.Drawing.Size(50, 13);
            this.verbosityLabel.TabIndex = 3;
            this.verbosityLabel.Text = "Verbosity";
            // 
            // keywordLabel
            // 
            this.keywordLabel.AutoSize = true;
            this.keywordLabel.Location = new System.Drawing.Point(9, 97);
            this.keywordLabel.Name = "keywordLabel";
            this.keywordLabel.Size = new System.Drawing.Size(117, 13);
            this.keywordLabel.TabIndex = 3;
            this.keywordLabel.Text = "NVO Registry Title Like";
            // 
            // keyword
            // 
            this.keyword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.keyword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.keyword.ForeColor = System.Drawing.Color.White;
            this.keyword.Location = new System.Drawing.Point(12, 116);
            this.keyword.Name = "keyword";
            this.keyword.Size = new System.Drawing.Size(280, 20);
            this.keyword.TabIndex = 4;
            this.keyword.TextChanged += new System.EventHandler(this.keyword_TextChanged);
            // 
            // fromView
            // 
            this.fromView.Checked = true;
            this.fromView.Location = new System.Drawing.Point(323, 52);
            this.fromView.Name = "fromView";
            this.fromView.Size = new System.Drawing.Size(114, 25);
            this.fromView.TabIndex = 2;
            this.fromView.Text = "From View";
            this.fromView.CheckedChanged += new System.EventHandler(this.fromView_CheckedChanged);
            // 
            // fromRegistry
            // 
            this.fromRegistry.Checked = true;
            this.fromRegistry.Location = new System.Drawing.Point(6, 52);
            this.fromRegistry.Name = "fromRegistry";
            this.fromRegistry.Size = new System.Drawing.Size(114, 25);
            this.fromRegistry.TabIndex = 2;
            this.fromRegistry.Text = "From Registry";
            this.fromRegistry.CheckedChanged += new System.EventHandler(this.fromRegistry_CheckedChanged);
            // 
            // findRegistry
            // 
            this.findRegistry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.findRegistry.BackColor = System.Drawing.Color.Transparent;
            this.findRegistry.DialogResult = System.Windows.Forms.DialogResult.None;
            this.findRegistry.ImageDisabled = null;
            this.findRegistry.ImageEnabled = null;
            this.findRegistry.Location = new System.Drawing.Point(327, 108);
            this.findRegistry.MaximumSize = new System.Drawing.Size(140, 33);
            this.findRegistry.Name = "findRegistry";
            this.findRegistry.Selected = false;
            this.findRegistry.Size = new System.Drawing.Size(140, 33);
            this.findRegistry.TabIndex = 0;
            this.findRegistry.Text = "NVO Registry Search";
            this.findRegistry.Click += new System.EventHandler(this.findRegistry_Click);
            // 
            // close
            // 
            this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.close.BackColor = System.Drawing.Color.Transparent;
            this.close.DialogResult = System.Windows.Forms.DialogResult.None;
            this.close.ImageDisabled = null;
            this.close.ImageEnabled = null;
            this.close.Location = new System.Drawing.Point(761, 108);
            this.close.MaximumSize = new System.Drawing.Size(140, 33);
            this.close.Name = "close";
            this.close.Selected = false;
            this.close.Size = new System.Drawing.Size(77, 33);
            this.close.TabIndex = 0;
            this.close.Text = "Close";
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // search
            // 
            this.search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.search.BackColor = System.Drawing.Color.Transparent;
            this.search.DialogResult = System.Windows.Forms.DialogResult.None;
            this.search.Enabled = false;
            this.search.ImageDisabled = null;
            this.search.ImageEnabled = null;
            this.search.Location = new System.Drawing.Point(761, 74);
            this.search.MaximumSize = new System.Drawing.Size(140, 33);
            this.search.Name = "search";
            this.search.Selected = false;
            this.search.Size = new System.Drawing.Size(77, 33);
            this.search.TabIndex = 0;
            this.search.Text = "Search";
            this.search.Click += new System.EventHandler(this.search_Click);
            // 
            // verbosity
            // 
            this.verbosity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.verbosity.DateTimeValue = new System.DateTime(2008, 11, 6, 13, 59, 48, 477);
            this.verbosity.Filter = TerraViewer.Classification.Unfiltered;
            this.verbosity.FilterStyle = false;
            this.verbosity.Location = new System.Drawing.Point(556, 19);
            this.verbosity.Margin = new System.Windows.Forms.Padding(0);
            this.verbosity.MasterTime = true;
            this.verbosity.MaximumSize = new System.Drawing.Size(248, 33);
            this.verbosity.MinimumSize = new System.Drawing.Size(35, 33);
            this.verbosity.Name = "verbosity";
            this.verbosity.SelectedIndex = -1;
            this.verbosity.SelectedItem = null;
            this.verbosity.Size = new System.Drawing.Size(99, 33);
            this.verbosity.State = TerraViewer.State.Rest;
            this.verbosity.TabIndex = 5;
            this.verbosity.Type = TerraViewer.WwtCombo.ComboType.List;
            this.verbosity.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.verbosity_SelectionChanged);
            // 
            // coneSearchCheckbox
            // 
            this.coneSearchCheckbox.Checked = false;
            this.coneSearchCheckbox.Location = new System.Drawing.Point(661, 12);
            this.coneSearchCheckbox.Name = "coneSearchCheckbox";
            this.coneSearchCheckbox.Size = new System.Drawing.Size(173, 25);
            this.coneSearchCheckbox.TabIndex = 6;
            this.coneSearchCheckbox.Text = "Catalog (VO Cone Search)";
            this.coneSearchCheckbox.CheckedChanged += new System.EventHandler(this.coneSearchCheckbox_CheckedChanged);
            // 
            // siapCheckbox
            // 
            this.siapCheckbox.Checked = false;
            this.siapCheckbox.Location = new System.Drawing.Point(661, 34);
            this.siapCheckbox.Name = "siapCheckbox";
            this.siapCheckbox.Size = new System.Drawing.Size(202, 25);
            this.siapCheckbox.TabIndex = 7;
            this.siapCheckbox.Text = "Images {VO SIAP)";
            this.siapCheckbox.CheckedChanged += new System.EventHandler(this.siapCheckbox_CheckedChanged);
            // 
            // VORegistryBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(838, 446);
            this.Controls.Add(this.siapCheckbox);
            this.Controls.Add(this.coneSearchCheckbox);
            this.Controls.Add(this.searchRadius);
            this.Controls.Add(this.verbosityLabel);
            this.Controls.Add(this.searchRadiusLabel);
            this.Controls.Add(this.dec);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ra);
            this.Controls.Add(this.raLabel);
            this.Controls.Add(this.keyword);
            this.Controls.Add(this.keywordLabel);
            this.Controls.Add(this.baseUrl);
            this.Controls.Add(this.searchUrlLabel);
            this.Controls.Add(this.fromView);
            this.Controls.Add(this.fromRegistry);
            this.Controls.Add(this.ResourceList);
            this.Controls.Add(this.findRegistry);
            this.Controls.Add(this.close);
            this.Controls.Add(this.search);
            this.Controls.Add(this.verbosity);
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VORegistryBrowser";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "VO Cone Search / Registry Browser";
            this.Load += new System.EventHandler(this.VORegistryBrowser_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VORegistryBrowser_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private WwtButton search;
        private System.Windows.Forms.ListView ResourceList;
        private WWTCheckbox fromRegistry;
        private System.Windows.Forms.Label searchUrlLabel;
        private System.Windows.Forms.TextBox baseUrl;
        private System.Windows.Forms.Label raLabel;
        private System.Windows.Forms.TextBox ra;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox dec;
        private System.Windows.Forms.Label searchRadiusLabel;
        private System.Windows.Forms.TextBox searchRadius;
        private WwtCombo verbosity;
        private System.Windows.Forms.Label verbosityLabel;
        private WwtButton close;
        private WWTCheckbox fromView;
        private System.Windows.Forms.Label keywordLabel;
        private System.Windows.Forms.TextBox keyword;
        private WwtButton findRegistry;
        private System.Windows.Forms.ColumnHeader In;
        private WWTCheckbox coneSearchCheckbox;
        private WWTCheckbox siapCheckbox;
    }
}