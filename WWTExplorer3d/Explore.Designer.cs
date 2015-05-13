namespace TerraViewer
{
    partial class Explore
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Explore));
            this.BrowseList = new TerraViewer.ThumbnailList();
            this.paginator = new TerraViewer.Paginator();
            this.exploreText = new System.Windows.Forms.Label();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // BrowseList
            // 
            this.BrowseList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.BrowseList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.BrowseList.ColCount = 8;
            this.BrowseList.Items = ((System.Collections.Generic.List<object>)(resources.GetObject("BrowseList.Items")));
            this.BrowseList.Location = new System.Drawing.Point(9, 29);
            this.BrowseList.Margin = new System.Windows.Forms.Padding(0);
            this.BrowseList.MaximumSize = new System.Drawing.Size(2500, 475);
            this.BrowseList.MinimumSize = new System.Drawing.Size(100, 65);
            this.BrowseList.Name = "BrowseList";
            this.BrowseList.Paginator = null;
            this.BrowseList.RowCount = 1;
            this.BrowseList.Size = new System.Drawing.Size(972, 65);
            this.BrowseList.TabIndex = 3;
            this.BrowseList.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            this.BrowseList.ItemDoubleClicked += new TerraViewer.ItemClickedEventHandler(this.ConstellationList_ItemDoubleClicked);
            this.BrowseList.ItemContextMenu += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemContextMenu);
            this.BrowseList.ItemHover += new TerraViewer.ItemClickedEventHandler(this.ConstellationList_ItemHover);
            this.BrowseList.ItemClicked += new TerraViewer.ItemClickedEventHandler(this.ConstellationList_ItemClicked);
            this.BrowseList.ItemImageClicked += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemImageClicked);
            // 
            // paginator
            // 
            this.paginator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.paginator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.paginator.CurrentPage = 0;
            this.paginator.Location = new System.Drawing.Point(883, 6);
            this.paginator.Margin = new System.Windows.Forms.Padding(0);
            this.paginator.Name = "paginator";
            this.paginator.Size = new System.Drawing.Size(98, 18);
            this.paginator.TabIndex = 4;
            this.paginator.TotalPages = 0;
            // 
            // exploreText
            // 
            this.exploreText.AutoSize = true;
            this.exploreText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exploreText.ForeColor = System.Drawing.Color.White;
            this.exploreText.Location = new System.Drawing.Point(12, 6);
            this.exploreText.Name = "exploreText";
            this.exploreText.Size = new System.Drawing.Size(165, 13);
            this.exploreText.TabIndex = 5;
            this.exploreText.Text = "Select a Collection to explore...";
            this.toolTips.SetToolTip(this.exploreText, "Click to navigate back");
            this.exploreText.MouseLeave += new System.EventHandler(this.exploreText_MouseLeave);
            this.exploreText.Click += new System.EventHandler(this.exploreText_Click);
            this.exploreText.MouseEnter += new System.EventHandler(this.exploreText_MouseEnter);
            // 
            // Explore
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(990, 110);
            this.Controls.Add(this.exploreText);
            this.Controls.Add(this.BrowseList);
            this.Controls.Add(this.paginator);
            this.Name = "Explore";
            this.Opacity = 0;
            this.Text = "Explore";
            this.Load += new System.EventHandler(this.Explore_Load);
            this.Controls.SetChildIndex(this.pinUp, 0);
            this.Controls.SetChildIndex(this.paginator, 0);
            this.Controls.SetChildIndex(this.BrowseList, 0);
            this.Controls.SetChildIndex(this.exploreText, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ThumbnailList BrowseList;
        private Paginator paginator;
        private System.Windows.Forms.Label exploreText;
        private System.Windows.Forms.ToolTip toolTips;
    }
}