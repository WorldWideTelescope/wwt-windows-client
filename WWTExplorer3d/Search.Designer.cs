namespace TerraViewer
{
    partial class Search
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Search));
            this.searchText = new System.Windows.Forms.TextBox();
            this.raText = new System.Windows.Forms.TextBox();
            this.raLabel = new System.Windows.Forms.Label();
            this.decText = new System.Windows.Forms.TextBox();
            this.decLabel = new System.Windows.Forms.Label();
            this.searchResults = new TerraViewer.ThumbnailList();
            this.paginator = new TerraViewer.Paginator();
            this.SearchView = new TerraViewer.WwtButton();
            this.searchTimer = new System.Windows.Forms.Timer(this.components);
            this.wwtButton1 = new TerraViewer.WwtButton();
            this.plotResults = new TerraViewer.WWTCheckbox();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.measureTool = new TerraViewer.WwtButton();
            this.GoToRADec = new TerraViewer.WwtButton();
            this.coordinateType = new TerraViewer.WwtCombo();
            this.SuspendLayout();
            // 
            // pinUp
            // 
            this.pinUp.Location = new System.Drawing.Point(606, 94);
            // 
            // searchText
            // 
            this.searchText.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.searchText.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.searchText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.searchText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchText.ForeColor = System.Drawing.Color.White;
            this.searchText.Location = new System.Drawing.Point(6, 4);
            this.searchText.MaxLength = 64;
            this.searchText.Name = "searchText";
            this.searchText.Size = new System.Drawing.Size(178, 20);
            this.searchText.TabIndex = 0;
            this.searchText.Text = "Type your search here";
            this.toolTips.SetToolTip(this.searchText, "Enter your search terms here.");
            this.searchText.MouseClick += new System.Windows.Forms.MouseEventHandler(this.searchText_MouseClick);
            this.searchText.TextChanged += new System.EventHandler(this.searchText_TextChanged);
            this.searchText.Enter += new System.EventHandler(this.searchText_Enter);
            this.searchText.KeyUp += new System.Windows.Forms.KeyEventHandler(this.searchText_KeyUp);
            this.searchText.Leave += new System.EventHandler(this.searchText_Leave);
            // 
            // raText
            // 
            this.raText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.raText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.raText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.raText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.raText.ForeColor = System.Drawing.Color.White;
            this.raText.Location = new System.Drawing.Point(583, 4);
            this.raText.MaxLength = 64;
            this.raText.Name = "raText";
            this.raText.Size = new System.Drawing.Size(60, 20);
            this.raText.TabIndex = 4;
            this.raText.Text = "0";
            this.raText.Enter += new System.EventHandler(this.raText_Enter);
            this.raText.Validating += new System.ComponentModel.CancelEventHandler(this.raText_Validating);
            // 
            // raLabel
            // 
            this.raLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.raLabel.AutoSize = true;
            this.raLabel.BackColor = System.Drawing.Color.Transparent;
            this.raLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.raLabel.Location = new System.Drawing.Point(556, 6);
            this.raLabel.Name = "raLabel";
            this.raLabel.Size = new System.Drawing.Size(21, 13);
            this.raLabel.TabIndex = 3;
            this.raLabel.Text = "RA";
            // 
            // decText
            // 
            this.decText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.decText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.decText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.decText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.decText.ForeColor = System.Drawing.Color.White;
            this.decText.Location = new System.Drawing.Point(681, 4);
            this.decText.MaxLength = 64;
            this.decText.Name = "decText";
            this.decText.Size = new System.Drawing.Size(60, 20);
            this.decText.TabIndex = 6;
            this.decText.Text = "0";
            this.decText.Enter += new System.EventHandler(this.decText_Enter);
            this.decText.Validating += new System.ComponentModel.CancelEventHandler(this.decText_Validating);
            // 
            // decLabel
            // 
            this.decLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.decLabel.AutoSize = true;
            this.decLabel.BackColor = System.Drawing.Color.Transparent;
            this.decLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.decLabel.Location = new System.Drawing.Point(649, 6);
            this.decLabel.Name = "decLabel";
            this.decLabel.Size = new System.Drawing.Size(26, 13);
            this.decLabel.TabIndex = 5;
            this.decLabel.Text = "Dec";
            // 
            // searchResults
            // 
            this.searchResults.AddText = "Add New Item";
            this.searchResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.searchResults.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.searchResults.ColCount = 9;
            this.searchResults.DontStealFocus = false;
            this.searchResults.EmptyAddText = "No Results";
            this.searchResults.Items = ((System.Collections.Generic.List<object>)(resources.GetObject("searchResults.Items")));
            this.searchResults.Location = new System.Drawing.Point(6, 30);
            this.searchResults.Margin = new System.Windows.Forms.Padding(0);
            this.searchResults.MaximumSize = new System.Drawing.Size(4096, 475);
            this.searchResults.MinimumSize = new System.Drawing.Size(100, 65);
            this.searchResults.Name = "searchResults";
            this.searchResults.Paginator = this.paginator;
            this.searchResults.RowCount = 1;
            this.searchResults.ShowAddButton = false;
            this.searchResults.Size = new System.Drawing.Size(1009, 65);
            this.searchResults.TabIndex = 10;
            this.searchResults.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            this.searchResults.ItemHover += new TerraViewer.ItemClickedEventHandler(this.searchResults_ItemHover);
            this.searchResults.ItemClicked += new TerraViewer.ItemClickedEventHandler(this.searchResults_ItemClicked);
            this.searchResults.ItemDoubleClicked += new TerraViewer.ItemClickedEventHandler(this.searchResults_ItemDoubleClicked);
            this.searchResults.ItemContextMenu += new TerraViewer.ItemClickedEventHandler(this.searchResults_ItemContextMenu);
            this.searchResults.Enter += new System.EventHandler(this.searchResults_Enter);
            // 
            // paginator
            // 
            this.paginator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.paginator.BackColor = System.Drawing.Color.Transparent;
            this.paginator.CurrentPage = 0;
            this.paginator.Location = new System.Drawing.Point(917, 4);
            this.paginator.Margin = new System.Windows.Forms.Padding(0);
            this.paginator.Name = "paginator";
            this.paginator.Size = new System.Drawing.Size(98, 18);
            this.paginator.TabIndex = 9;
            this.paginator.TotalPages = 1;
            // 
            // SearchView
            // 
            this.SearchView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchView.BackColor = System.Drawing.Color.Transparent;
            this.SearchView.DialogResult = System.Windows.Forms.DialogResult.None;
            this.SearchView.ImageDisabled = null;
            this.SearchView.ImageEnabled = null;
            this.SearchView.Location = new System.Drawing.Point(811, -2);
            this.SearchView.MaximumSize = new System.Drawing.Size(140, 33);
            this.SearchView.Name = "SearchView";
            this.SearchView.Selected = false;
            this.SearchView.Size = new System.Drawing.Size(101, 33);
            this.SearchView.TabIndex = 8;
            this.SearchView.Text = "Search View";
            this.SearchView.Click += new System.EventHandler(this.SearchView_Click);
            // 
            // searchTimer
            // 
            this.searchTimer.Enabled = true;
            this.searchTimer.Interval = 500;
            this.searchTimer.Tick += new System.EventHandler(this.searchTimer_Tick);
            // 
            // wwtButton1
            // 
            this.wwtButton1.BackColor = System.Drawing.Color.Transparent;
            this.wwtButton1.DialogResult = System.Windows.Forms.DialogResult.None;
            this.wwtButton1.ImageDisabled = null;
            this.wwtButton1.ImageEnabled = null;
            this.wwtButton1.Location = new System.Drawing.Point(300, 76);
            this.wwtButton1.MaximumSize = new System.Drawing.Size(140, 33);
            this.wwtButton1.Name = "wwtButton1";
            this.wwtButton1.Selected = false;
            this.wwtButton1.Size = new System.Drawing.Size(70, 33);
            this.wwtButton1.TabIndex = 4;
            this.wwtButton1.Text = "Server";
            this.wwtButton1.Visible = false;
            // 
            // plotResults
            // 
            this.plotResults.BackColor = System.Drawing.Color.Transparent;
            this.plotResults.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.plotResults.Checked = false;
            this.plotResults.Location = new System.Drawing.Point(191, 2);
            this.plotResults.Name = "plotResults";
            this.plotResults.Size = new System.Drawing.Size(98, 25);
            this.plotResults.TabIndex = 1;
            this.plotResults.Text = "Plot Results";
            this.toolTips.SetToolTip(this.plotResults, "Show the location of each result on the sky map.");
            this.plotResults.CheckedChanged += new System.EventHandler(this.plotResults_CheckedChanged);
            // 
            // measureTool
            // 
            this.measureTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.measureTool.BackColor = System.Drawing.Color.Transparent;
            this.measureTool.DialogResult = System.Windows.Forms.DialogResult.None;
            this.measureTool.ImageDisabled = null;
            this.measureTool.ImageEnabled = null;
            this.measureTool.Location = new System.Drawing.Point(401, -2);
            this.measureTool.MaximumSize = new System.Drawing.Size(140, 33);
            this.measureTool.Name = "measureTool";
            this.measureTool.Selected = false;
            this.measureTool.Size = new System.Drawing.Size(75, 33);
            this.measureTool.TabIndex = 11;
            this.measureTool.Text = "Distance";
            this.toolTips.SetToolTip(this.measureTool, "Measure Angular Distance by dragging mouse ( Shift + Drag)");
            this.measureTool.Click += new System.EventHandler(this.measureTool_Click);
            // 
            // GoToRADec
            // 
            this.GoToRADec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GoToRADec.BackColor = System.Drawing.Color.Transparent;
            this.GoToRADec.DialogResult = System.Windows.Forms.DialogResult.None;
            this.GoToRADec.ImageDisabled = null;
            this.GoToRADec.ImageEnabled = null;
            this.GoToRADec.Location = new System.Drawing.Point(749, -2);
            this.GoToRADec.MaximumSize = new System.Drawing.Size(140, 33);
            this.GoToRADec.Name = "GoToRADec";
            this.GoToRADec.Selected = false;
            this.GoToRADec.Size = new System.Drawing.Size(52, 33);
            this.GoToRADec.TabIndex = 7;
            this.GoToRADec.Text = "Go";
            this.GoToRADec.Click += new System.EventHandler(this.GoToRADec_Click);
            // 
            // coordinateType
            // 
            this.coordinateType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.coordinateType.BackColor = System.Drawing.Color.Transparent;
            this.coordinateType.DateTimeValue = new System.DateTime(2008, 9, 22, 14, 52, 48, 761);
            this.coordinateType.Filter = TerraViewer.Classification.Unfiltered;
            this.coordinateType.FilterStyle = false;
            this.coordinateType.Location = new System.Drawing.Point(479, -2);
            this.coordinateType.Margin = new System.Windows.Forms.Padding(0);
            this.coordinateType.MasterTime = true;
            this.coordinateType.MaximumSize = new System.Drawing.Size(248, 33);
            this.coordinateType.MinimumSize = new System.Drawing.Size(35, 33);
            this.coordinateType.Name = "coordinateType";
            this.coordinateType.SelectedIndex = -1;
            this.coordinateType.SelectedItem = null;
            this.coordinateType.Size = new System.Drawing.Size(74, 33);
            this.coordinateType.State = TerraViewer.State.Rest;
            this.coordinateType.TabIndex = 2;
            this.coordinateType.Type = TerraViewer.WwtCombo.ComboType.List;
            this.coordinateType.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.coordinateType_SelectionChanged);
            // 
            // Search
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(1024, 110);
            this.Controls.Add(this.searchResults);
            this.Controls.Add(this.GoToRADec);
            this.Controls.Add(this.coordinateType);
            this.Controls.Add(this.plotResults);
            this.Controls.Add(this.measureTool);
            this.Controls.Add(this.wwtButton1);
            this.Controls.Add(this.paginator);
            this.Controls.Add(this.searchText);
            this.Controls.Add(this.decLabel);
            this.Controls.Add(this.decText);
            this.Controls.Add(this.SearchView);
            this.Controls.Add(this.raLabel);
            this.Controls.Add(this.raText);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "Search";
            this.Opacity = 0D;
            this.Load += new System.EventHandler(this.Search_Load);
            this.Shown += new System.EventHandler(this.Search_Shown);
            this.Controls.SetChildIndex(this.raText, 0);
            this.Controls.SetChildIndex(this.raLabel, 0);
            this.Controls.SetChildIndex(this.SearchView, 0);
            this.Controls.SetChildIndex(this.decText, 0);
            this.Controls.SetChildIndex(this.decLabel, 0);
            this.Controls.SetChildIndex(this.searchText, 0);
            this.Controls.SetChildIndex(this.paginator, 0);
            this.Controls.SetChildIndex(this.wwtButton1, 0);
            this.Controls.SetChildIndex(this.measureTool, 0);
            this.Controls.SetChildIndex(this.plotResults, 0);
            this.Controls.SetChildIndex(this.coordinateType, 0);
            this.Controls.SetChildIndex(this.GoToRADec, 0);
            this.Controls.SetChildIndex(this.pinUp, 0);
            this.Controls.SetChildIndex(this.searchResults, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox searchText;
        private System.Windows.Forms.TextBox raText;
        private System.Windows.Forms.Label raLabel;
        private System.Windows.Forms.TextBox decText;
        private System.Windows.Forms.Label decLabel;
        private ThumbnailList searchResults;
        private Paginator paginator;
        private WwtButton SearchView;
        private System.Windows.Forms.Timer searchTimer;
        private WwtButton wwtButton1;
        private WWTCheckbox plotResults;
        private System.Windows.Forms.ToolTip toolTips;
        private WwtButton GoToRADec;
        private WwtCombo coordinateType;
        private WwtButton measureTool;

    }
}