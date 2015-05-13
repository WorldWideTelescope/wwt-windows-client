namespace TerraViewer.Callibration
{
    partial class ProjectorProperties
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectorProperties));
            this.label1 = new System.Windows.Forms.Label();
            this.projectorID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.projectorName = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.projectorListLabel = new System.Windows.Forms.Label();
            this.projectionTransformEditor = new TerraViewer.Callibration.ProjectionTransformEditor();
            this.projectorList = new TerraViewer.WwtCombo();
            this.ProjectorTab = new TerraViewer.Tab();
            this.ViewTab = new TerraViewer.Tab();
            this.OK = new TerraViewer.WwtButton();
            this.Solved = new TerraViewer.Tab();
            this.CopySolved = new TerraViewer.WwtButton();
            this.CopyView = new TerraViewer.WwtButton();
            this.updateClient = new TerraViewer.WwtButton();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ID";
            // 
            // projectorID
            // 
            this.projectorID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.projectorID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.projectorID.ForeColor = System.Drawing.Color.White;
            this.projectorID.Location = new System.Drawing.Point(12, 28);
            this.projectorID.Name = "projectorID";
            this.projectorID.Size = new System.Drawing.Size(100, 20);
            this.projectorID.TabIndex = 1;
            this.projectorID.TextChanged += new System.EventHandler(this.projectorID_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name";
            // 
            // projectorName
            // 
            this.projectorName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.projectorName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.projectorName.ForeColor = System.Drawing.Color.White;
            this.projectorName.Location = new System.Drawing.Point(118, 28);
            this.projectorName.Name = "projectorName";
            this.projectorName.Size = new System.Drawing.Size(149, 20);
            this.projectorName.TabIndex = 3;
            this.projectorName.TextChanged += new System.EventHandler(this.projectorName_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::TerraViewer.Properties.Resources.tabBackground;
            this.pictureBox1.Location = new System.Drawing.Point(0, 69);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(256, 34);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = global::TerraViewer.Properties.Resources.tabBackground;
            this.pictureBox2.Location = new System.Drawing.Point(252, 69);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(256, 34);
            this.pictureBox2.TabIndex = 7;
            this.pictureBox2.TabStop = false;
            // 
            // projectorListLabel
            // 
            this.projectorListLabel.AutoSize = true;
            this.projectorListLabel.Location = new System.Drawing.Point(285, 7);
            this.projectorListLabel.Name = "projectorListLabel";
            this.projectorListLabel.Size = new System.Drawing.Size(68, 13);
            this.projectorListLabel.TabIndex = 4;
            this.projectorListLabel.Text = "Projector List";
            // 
            // projectionTransformEditor
            // 
            this.projectionTransformEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.projectionTransformEditor.ForeColor = System.Drawing.Color.White;
            this.projectionTransformEditor.Location = new System.Drawing.Point(12, 119);
            this.projectionTransformEditor.Name = "projectionTransformEditor";
            this.projectionTransformEditor.ProjTarget = null;
            this.projectionTransformEditor.Size = new System.Drawing.Size(450, 143);
            this.projectionTransformEditor.TabIndex = 8;
            this.projectionTransformEditor.TransTarget = null;
            // 
            // projectorList
            // 
            this.projectorList.BackColor = System.Drawing.Color.Transparent;
            this.projectorList.DateTimeValue = new System.DateTime(2010, 12, 21, 14, 3, 36, 357);
            this.projectorList.Filter = TerraViewer.Classification.Unfiltered;
            this.projectorList.FilterStyle = false;
            this.projectorList.Location = new System.Drawing.Point(287, 22);
            this.projectorList.Margin = new System.Windows.Forms.Padding(0);
            this.projectorList.MasterTime = true;
            this.projectorList.MaximumSize = new System.Drawing.Size(248, 33);
            this.projectorList.MinimumSize = new System.Drawing.Size(35, 33);
            this.projectorList.Name = "projectorList";
            this.projectorList.SelectedIndex = -1;
            this.projectorList.SelectedItem = null;
            this.projectorList.Size = new System.Drawing.Size(185, 33);
            this.projectorList.State = TerraViewer.State.Rest;
            this.projectorList.TabIndex = 5;
            this.projectorList.Type = TerraViewer.WwtCombo.ComboType.List;
            this.projectorList.SelectionChanged += new TerraViewer.SelectionChangedEventHandler(this.projectorList_SelectionChanged);
            // 
            // ProjectorTab
            // 
            this.ProjectorTab.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ProjectorTab.BackgroundImage")));
            this.ProjectorTab.Location = new System.Drawing.Point(134, 69);
            this.ProjectorTab.MaximumSize = new System.Drawing.Size(100, 34);
            this.ProjectorTab.MinimumSize = new System.Drawing.Size(100, 34);
            this.ProjectorTab.Name = "ProjectorTab";
            this.ProjectorTab.Selected = false;
            this.ProjectorTab.Size = new System.Drawing.Size(100, 34);
            this.ProjectorTab.TabIndex = 7;
            this.ProjectorTab.Title = "Projector";
            this.ProjectorTab.Click += new System.EventHandler(this.ProjectorTab_Click);
            // 
            // ViewTab
            // 
            this.ViewTab.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ViewTab.BackgroundImage")));
            this.ViewTab.Location = new System.Drawing.Point(29, 69);
            this.ViewTab.MaximumSize = new System.Drawing.Size(100, 34);
            this.ViewTab.MinimumSize = new System.Drawing.Size(100, 34);
            this.ViewTab.Name = "ViewTab";
            this.ViewTab.Selected = true;
            this.ViewTab.Size = new System.Drawing.Size(100, 34);
            this.ViewTab.TabIndex = 6;
            this.ViewTab.Title = "View";
            this.ViewTab.Click += new System.EventHandler(this.ViewTab_Click);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(386, 280);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(87, 33);
            this.OK.TabIndex = 9;
            this.OK.Text = "Ok";
            this.OK.Click += new System.EventHandler(this.ok_Click);
            // 
            // Solved
            // 
            this.Solved.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("Solved.BackgroundImage")));
            this.Solved.Location = new System.Drawing.Point(239, 69);
            this.Solved.MaximumSize = new System.Drawing.Size(100, 34);
            this.Solved.MinimumSize = new System.Drawing.Size(100, 34);
            this.Solved.Name = "Solved";
            this.Solved.Selected = false;
            this.Solved.Size = new System.Drawing.Size(100, 34);
            this.Solved.TabIndex = 10;
            this.Solved.Title = "Solved";
            this.Solved.Click += new System.EventHandler(this.Solved_Click);
            // 
            // CopySolved
            // 
            this.CopySolved.BackColor = System.Drawing.Color.Transparent;
            this.CopySolved.DialogResult = System.Windows.Forms.DialogResult.None;
            this.CopySolved.ImageDisabled = null;
            this.CopySolved.ImageEnabled = null;
            this.CopySolved.Location = new System.Drawing.Point(280, 280);
            this.CopySolved.MaximumSize = new System.Drawing.Size(140, 33);
            this.CopySolved.Name = "CopySolved";
            this.CopySolved.Selected = false;
            this.CopySolved.Size = new System.Drawing.Size(100, 33);
            this.CopySolved.TabIndex = 11;
            this.CopySolved.Text = "Copy Solved";
            this.CopySolved.Click += new System.EventHandler(this.CopySolved_Click);
            // 
            // CopyView
            // 
            this.CopyView.BackColor = System.Drawing.Color.Transparent;
            this.CopyView.DialogResult = System.Windows.Forms.DialogResult.None;
            this.CopyView.ImageDisabled = null;
            this.CopyView.ImageEnabled = null;
            this.CopyView.Location = new System.Drawing.Point(175, 280);
            this.CopyView.MaximumSize = new System.Drawing.Size(140, 33);
            this.CopyView.Name = "CopyView";
            this.CopyView.Selected = false;
            this.CopyView.Size = new System.Drawing.Size(99, 33);
            this.CopyView.TabIndex = 12;
            this.CopyView.Text = "Copy View";
            this.CopyView.Click += new System.EventHandler(this.CopyView_Click);
            // 
            // updateClient
            // 
            this.updateClient.BackColor = System.Drawing.Color.Transparent;
            this.updateClient.DialogResult = System.Windows.Forms.DialogResult.None;
            this.updateClient.ImageDisabled = null;
            this.updateClient.ImageEnabled = null;
            this.updateClient.Location = new System.Drawing.Point(69, 280);
            this.updateClient.MaximumSize = new System.Drawing.Size(140, 33);
            this.updateClient.Name = "updateClient";
            this.updateClient.Selected = false;
            this.updateClient.Size = new System.Drawing.Size(100, 33);
            this.updateClient.TabIndex = 13;
            this.updateClient.Text = "Send Update";
            this.updateClient.Click += new System.EventHandler(this.updateClient_Click);
            // 
            // ProjectorProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(32)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(485, 325);
            this.Controls.Add(this.updateClient);
            this.Controls.Add(this.CopyView);
            this.Controls.Add(this.CopySolved);
            this.Controls.Add(this.Solved);
            this.Controls.Add(this.projectionTransformEditor);
            this.Controls.Add(this.projectorListLabel);
            this.Controls.Add(this.projectorList);
            this.Controls.Add(this.ProjectorTab);
            this.Controls.Add(this.ViewTab);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.projectorName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.projectorID);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProjectorProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Projector Properties";
            this.Load += new System.EventHandler(this.ProjectorProperties_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox projectorID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox projectorName;
        private WwtButton OK;
        private Tab ProjectorTab;
        private Tab ViewTab;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private WwtCombo projectorList;
        private System.Windows.Forms.Label projectorListLabel;
        private ProjectionTransformEditor projectionTransformEditor;
        private Tab Solved;
        private WwtButton CopySolved;
        private WwtButton CopyView;
        private WwtButton updateClient;
    }
}