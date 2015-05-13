namespace TerraViewer
{
    partial class ToursTab
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToursTab));
            this.resultsList = new TerraViewer.ThumbnailList();
            this.paginator = new TerraViewer.Paginator();
            this.BreadCrumbs = new System.Windows.Forms.Label();
            this.LoadTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // resultsList
            // 
            this.resultsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.resultsList.ColCount = 8;
            this.resultsList.Items = ((System.Collections.Generic.List<object>)(resources.GetObject("resultsList.Items")));
            this.resultsList.Location = new System.Drawing.Point(9, 27);
            this.resultsList.Margin = new System.Windows.Forms.Padding(0);
            this.resultsList.MaximumSize = new System.Drawing.Size(2500, 475);
            this.resultsList.MinimumSize = new System.Drawing.Size(100, 65);
            this.resultsList.Name = "resultsList";
            this.resultsList.Paginator = this.paginator;
            this.resultsList.RowCount = 1;
            this.resultsList.Size = new System.Drawing.Size(957, 65);
            this.resultsList.TabIndex = 1;
            this.resultsList.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            this.resultsList.ItemDoubleClicked += new TerraViewer.ItemClickedEventHandler(this.resultsList_ItemDoubleClicked);
            this.resultsList.ItemHover += new TerraViewer.ItemClickedEventHandler(this.resultsList_ItemHover);
            this.resultsList.ItemClicked += new TerraViewer.ItemClickedEventHandler(this.resultsList_ItemClicked);
            // 
            // paginator
            // 
            this.paginator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.paginator.BackColor = System.Drawing.Color.Transparent;
            this.paginator.CurrentPage = 0;
            this.paginator.Location = new System.Drawing.Point(868, 5);
            this.paginator.Margin = new System.Windows.Forms.Padding(0);
            this.paginator.Name = "paginator";
            this.paginator.Size = new System.Drawing.Size(98, 18);
            this.paginator.TabIndex = 2;
            this.paginator.TotalPages = 1;
            // 
            // BreadCrumbs
            // 
            this.BreadCrumbs.AutoSize = true;
            this.BreadCrumbs.BackColor = System.Drawing.Color.Transparent;
            this.BreadCrumbs.ForeColor = System.Drawing.Color.White;
            this.BreadCrumbs.Location = new System.Drawing.Point(6, 5);
            this.BreadCrumbs.Name = "BreadCrumbs";
            this.BreadCrumbs.Size = new System.Drawing.Size(100, 13);
            this.BreadCrumbs.TabIndex = 3;
            this.BreadCrumbs.Text = "Select a Category...";
            // 
            // LoadTimer
            // 
            this.LoadTimer.Enabled = true;
            this.LoadTimer.Tick += new System.EventHandler(this.LoadTimer_Tick);
            // 
            // ToursTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 110);
            this.Controls.Add(this.resultsList);
            this.Controls.Add(this.paginator);
            this.Controls.Add(this.BreadCrumbs);
            this.KeyPreview = true;
            this.Name = "ToursTab";
            this.Text = "Discover";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Discover_MouseClick);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToursTab_KeyDown);
            this.Load += new System.EventHandler(this.Discover_Load);
            this.Controls.SetChildIndex(this.pinUp, 0);
            this.Controls.SetChildIndex(this.BreadCrumbs, 0);
            this.Controls.SetChildIndex(this.paginator, 0);
            this.Controls.SetChildIndex(this.resultsList, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ThumbnailList resultsList;
        private Paginator paginator;
        private System.Windows.Forms.Label BreadCrumbs;
        private System.Windows.Forms.Timer LoadTimer;
    }
}