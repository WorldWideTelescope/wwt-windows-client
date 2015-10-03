namespace TerraViewer
{
    partial class TaxonomyTree
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.ScopeLabel = new System.Windows.Forms.Label();
            this.Classification = new System.Windows.Forms.Label();
            this.OK = new TerraViewer.WwtButton();
            this.Cancel = new TerraViewer.WwtButton();
            this.Scope = new TerraViewer.WwtCombo();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.treeView1.CheckBoxes = true;
            this.treeView1.ForeColor = System.Drawing.Color.White;
            this.treeView1.Location = new System.Drawing.Point(12, 79);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(260, 208);
            this.treeView1.TabIndex = 0;
            this.treeView1.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeCheck);
            // 
            // ScopeLabel
            // 
            this.ScopeLabel.AutoSize = true;
            this.ScopeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ScopeLabel.ForeColor = System.Drawing.Color.White;
            this.ScopeLabel.Location = new System.Drawing.Point(12, 9);
            this.ScopeLabel.Name = "ScopeLabel";
            this.ScopeLabel.Size = new System.Drawing.Size(45, 13);
            this.ScopeLabel.TabIndex = 2;
            this.ScopeLabel.Text = "Scope *";
            // 
            // Classification
            // 
            this.Classification.AutoSize = true;
            this.Classification.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.Classification.ForeColor = System.Drawing.Color.White;
            this.Classification.Location = new System.Drawing.Point(12, 60);
            this.Classification.Name = "Classification";
            this.Classification.Size = new System.Drawing.Size(68, 13);
            this.Classification.TabIndex = 3;
            this.Classification.Text = "Classification";
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Enabled = false;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(102, 293);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(89, 33);
            this.OK.TabIndex = 8;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(188, 293);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(90, 33);
            this.Cancel.TabIndex = 7;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Scope
            // 
            this.Scope.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.Scope.Filter = TerraViewer.Classification.Unfiltered;
            this.Scope.FilterStyle = false;
            this.Scope.Location = new System.Drawing.Point(15, 26);
            this.Scope.Margin = new System.Windows.Forms.Padding(0);
            this.Scope.MaximumSize = new System.Drawing.Size(248, 33);
            this.Scope.MinimumSize = new System.Drawing.Size(35, 33);
            this.Scope.Name = "Scope";
            this.Scope.SelectedIndex = -1;
            this.Scope.Size = new System.Drawing.Size(248, 33);
            this.Scope.State = TerraViewer.State.Rest;
            this.Scope.TabIndex = 6;
            this.Scope.Load += new System.EventHandler(this.Scope_Load);
            this.Scope.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.Scope_SelectionChanged);
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(11, 302);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(93, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = " * Required Field";
            // 
            // TaxonomyTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(284, 329);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.Scope);
            this.Controls.Add(this.Classification);
            this.Controls.Add(this.ScopeLabel);
            this.Controls.Add(this.treeView1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TaxonomyTree";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Classification";
            this.Load += new System.EventHandler(this.TaxonomyTree_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label ScopeLabel;
        private System.Windows.Forms.Label Classification;
        private WwtCombo Scope;
        private WwtButton Cancel;
        private WwtButton OK;
        private System.Windows.Forms.Label label6;
    }
}