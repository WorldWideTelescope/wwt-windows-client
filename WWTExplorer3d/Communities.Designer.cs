namespace TerraViewer
{
    partial class Communities
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Communities));
            this.resultsList = new TerraViewer.ThumbnailList();
            this.paginator = new TerraViewer.Paginator();
            this.selectText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // resultsList
            // 
            this.resultsList.AddText = "Add New Item";
            this.resultsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.resultsList.ColCount = 8;
            this.resultsList.EmptyAddText = "No Results";
            this.resultsList.Items = ((System.Collections.Generic.List<object>)(resources.GetObject("resultsList.Items")));
            this.resultsList.Location = new System.Drawing.Point(9, 29);
            this.resultsList.Margin = new System.Windows.Forms.Padding(0);
            this.resultsList.MaximumSize = new System.Drawing.Size(2500, 475);
            this.resultsList.MinimumSize = new System.Drawing.Size(100, 65);
            this.resultsList.Name = "resultsList";
            this.resultsList.Paginator = this.paginator;
            this.resultsList.RowCount = 1;
            this.resultsList.ShowAddButton = false;
            this.resultsList.Size = new System.Drawing.Size(957, 65);
            this.resultsList.TabIndex = 1;
            this.resultsList.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            this.resultsList.AddNewItem += new TerraViewer.ItemClickedEventHandler(this.resultsList_AddNewItem);
            this.resultsList.ItemDoubleClicked += new TerraViewer.ItemClickedEventHandler(this.resultsList_ItemDoubleClicked);
            this.resultsList.Load += new System.EventHandler(this.resultsList_Load);
            this.resultsList.ItemClicked += new TerraViewer.ItemClickedEventHandler(this.resultsList_ItemDoubleClicked);
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
            // selectText
            // 
            this.selectText.AutoSize = true;
            this.selectText.BackColor = System.Drawing.Color.Transparent;
            this.selectText.ForeColor = System.Drawing.Color.White;
            this.selectText.Location = new System.Drawing.Point(6, 5);
            this.selectText.Name = "selectText";
            this.selectText.Size = new System.Drawing.Size(109, 13);
            this.selectText.TabIndex = 3;
            this.selectText.Text = "Select a Community...";
            // 
            // Communities
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 110);
            this.Controls.Add(this.resultsList);
            this.Controls.Add(this.selectText);
            this.Controls.Add(this.paginator);
            this.Name = "Communities";
            this.Opacity = 0;
            this.Text = "Communities";
            this.Load += new System.EventHandler(this.Communities_Load);
            this.Controls.SetChildIndex(this.pinUp, 0);
            this.Controls.SetChildIndex(this.paginator, 0);
            this.Controls.SetChildIndex(this.selectText, 0);
            this.Controls.SetChildIndex(this.resultsList, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ThumbnailList resultsList;
        private Paginator paginator;
        private System.Windows.Forms.Label selectText;
    }
}