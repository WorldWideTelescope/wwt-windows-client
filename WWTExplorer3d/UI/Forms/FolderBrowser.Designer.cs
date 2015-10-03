namespace TerraViewer
{
    partial class FolderBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderBrowser));
            this.exploreText = new System.Windows.Forms.Label();
            this.BrowseList = new TerraViewer.ThumbnailList();
            this.paginator = new TerraViewer.Paginator();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // pinUp
            // 
            this.pinUp.Location = new System.Drawing.Point(472, 94);
            // 
            // exploreText
            // 
            this.exploreText.AutoSize = true;
            this.exploreText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exploreText.ForeColor = System.Drawing.Color.White;
            this.exploreText.Location = new System.Drawing.Point(6, 6);
            this.exploreText.Name = "exploreText";
            this.exploreText.Size = new System.Drawing.Size(165, 13);
            this.exploreText.TabIndex = 6;
            this.exploreText.Text = "Select a Collection to explore...";
            this.exploreText.Click += new System.EventHandler(this.exploreText_Click);
            this.exploreText.MouseEnter += new System.EventHandler(this.exploreText_MouseEnter);
            this.exploreText.MouseLeave += new System.EventHandler(this.exploreText_MouseLeave);
            // 
            // BrowseList
            // 
            this.BrowseList.AddText = "Add New Item";
            this.BrowseList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.BrowseList.ColCount = 8;
            this.BrowseList.DontStealFocus = false;
            this.BrowseList.EmptyAddText = "No Results";
            this.BrowseList.Items = ((System.Collections.Generic.List<object>)(resources.GetObject("BrowseList.Items")));
            this.BrowseList.Location = new System.Drawing.Point(9, 29);
            this.BrowseList.Margin = new System.Windows.Forms.Padding(0);
            this.BrowseList.MaximumSize = new System.Drawing.Size(4096, 475);
            this.BrowseList.MinimumSize = new System.Drawing.Size(100, 65);
            this.BrowseList.Name = "BrowseList";
            this.BrowseList.Paginator = this.paginator;
            this.BrowseList.RowCount = 1;
            this.BrowseList.ShowAddButton = false;
            this.BrowseList.Size = new System.Drawing.Size(965, 65);
            this.BrowseList.TabIndex = 7;
            this.BrowseList.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            this.BrowseList.ItemHover += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemHover);
            this.BrowseList.ItemClicked += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemClicked);
            this.BrowseList.ItemDoubleClicked += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemDoubleClicked);
            this.BrowseList.ItemImageClicked += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemImageClicked);
            this.BrowseList.ItemContextMenu += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemContextMenu);
            this.BrowseList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BrowseList_KeyDown);
            // 
            // paginator
            // 
            this.paginator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.paginator.BackColor = System.Drawing.Color.Transparent;
            this.paginator.CurrentPage = 0;
            this.paginator.Location = new System.Drawing.Point(876, 6);
            this.paginator.Margin = new System.Windows.Forms.Padding(0);
            this.paginator.Name = "paginator";
            this.paginator.Size = new System.Drawing.Size(98, 18);
            this.paginator.TabIndex = 8;
            this.paginator.TotalPages = 1;
            // 
            // FolderBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(983, 110);
            this.Controls.Add(this.BrowseList);
            this.Controls.Add(this.paginator);
            this.Controls.Add(this.exploreText);
            this.Name = "FolderBrowser";
            this.Opacity = 0D;
            this.Text = "layer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FolderBrowser_FormClosed);
            this.Controls.SetChildIndex(this.pinUp, 0);
            this.Controls.SetChildIndex(this.exploreText, 0);
            this.Controls.SetChildIndex(this.paginator, 0);
            this.Controls.SetChildIndex(this.BrowseList, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label exploreText;
        private ThumbnailList BrowseList;
        private Paginator paginator;
        private System.Windows.Forms.ToolTip toolTips;

    }
}