namespace TerraViewer
{
    partial class TourProperties
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
            this.label1 = new System.Windows.Forms.Label();
            this.authorNameTextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tourDescriptionTextbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.AdvancedOption = new System.Windows.Forms.RadioButton();
            this.IntermediateOption = new System.Windows.Forms.RadioButton();
            this.BeginnerOption = new System.Windows.Forms.RadioButton();
            this.authorImagePicturebox = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tourTitleTextbox = new System.Windows.Forms.TextBox();
            this.tourIDText = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.orgUrl = new System.Windows.Forms.TextBox();
            this.ClassificationText = new System.Windows.Forms.TextBox();
            this.ClassificationTaxonomyLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.authorEmailText = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.keywords = new System.Windows.Forms.TextBox();
            this.ShowTaxTree = new TerraViewer.WwtButton();
            this.OK = new TerraViewer.WwtButton();
            this.Cancel = new TerraViewer.WwtButton();
            this.ImportAuthorImage = new TerraViewer.WwtButton();
            this.wwtCombo1 = new TerraViewer.WwtCombo();
            this.label9 = new System.Windows.Forms.Label();
            this.OrganizationName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.authorImagePicturebox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(94, 155);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Author Name";
            // 
            // authorNameTextbox
            // 
            this.authorNameTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.authorNameTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.authorNameTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.authorNameTextbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.authorNameTextbox.ForeColor = System.Drawing.Color.White;
            this.authorNameTextbox.Location = new System.Drawing.Point(97, 172);
            this.authorNameTextbox.MaxLength = 100;
            this.authorNameTextbox.Name = "authorNameTextbox";
            this.authorNameTextbox.Size = new System.Drawing.Size(216, 22);
            this.authorNameTextbox.TabIndex = 6;
            this.authorNameTextbox.TextChanged += new System.EventHandler(this.authorNameTextbox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Tour Description";
            // 
            // tourDescriptionTextbox
            // 
            this.tourDescriptionTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tourDescriptionTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.tourDescriptionTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tourDescriptionTextbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tourDescriptionTextbox.ForeColor = System.Drawing.Color.White;
            this.tourDescriptionTextbox.Location = new System.Drawing.Point(15, 76);
            this.tourDescriptionTextbox.MaxLength = 1024;
            this.tourDescriptionTextbox.Multiline = true;
            this.tourDescriptionTextbox.Name = "tourDescriptionTextbox";
            this.tourDescriptionTextbox.Size = new System.Drawing.Size(297, 73);
            this.tourDescriptionTextbox.TabIndex = 4;
            this.tourDescriptionTextbox.TextChanged += new System.EventHandler(this.tourDescriptionTextbox_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.label3.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(94, 201);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Import or Replace Author Image";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.groupBox1.Controls.Add(this.AdvancedOption);
            this.groupBox1.Controls.Add(this.IntermediateOption);
            this.groupBox1.Controls.Add(this.BeginnerOption);
            this.groupBox1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(15, 391);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(297, 50);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select the level for the tour";
            // 
            // AdvancedOption
            // 
            this.AdvancedOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.AdvancedOption.AutoSize = true;
            this.AdvancedOption.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AdvancedOption.ForeColor = System.Drawing.Color.White;
            this.AdvancedOption.Location = new System.Drawing.Point(205, 19);
            this.AdvancedOption.Name = "AdvancedOption";
            this.AdvancedOption.Size = new System.Drawing.Size(75, 17);
            this.AdvancedOption.TabIndex = 2;
            this.AdvancedOption.TabStop = true;
            this.AdvancedOption.Text = "Advanced";
            this.AdvancedOption.UseVisualStyleBackColor = true;
            // 
            // IntermediateOption
            // 
            this.IntermediateOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.IntermediateOption.AutoSize = true;
            this.IntermediateOption.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IntermediateOption.ForeColor = System.Drawing.Color.White;
            this.IntermediateOption.Location = new System.Drawing.Point(107, 19);
            this.IntermediateOption.Name = "IntermediateOption";
            this.IntermediateOption.Size = new System.Drawing.Size(90, 17);
            this.IntermediateOption.TabIndex = 1;
            this.IntermediateOption.TabStop = true;
            this.IntermediateOption.Text = "Intermediate";
            this.IntermediateOption.UseVisualStyleBackColor = true;
            // 
            // BeginnerOption
            // 
            this.BeginnerOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.BeginnerOption.AutoSize = true;
            this.BeginnerOption.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BeginnerOption.ForeColor = System.Drawing.Color.White;
            this.BeginnerOption.Location = new System.Drawing.Point(16, 19);
            this.BeginnerOption.Name = "BeginnerOption";
            this.BeginnerOption.Size = new System.Drawing.Size(72, 17);
            this.BeginnerOption.TabIndex = 0;
            this.BeginnerOption.TabStop = true;
            this.BeginnerOption.Text = "Beginner";
            this.BeginnerOption.UseVisualStyleBackColor = true;
            // 
            // authorImagePicturebox
            // 
            this.authorImagePicturebox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.authorImagePicturebox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.authorImagePicturebox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.authorImagePicturebox.Location = new System.Drawing.Point(15, 155);
            this.authorImagePicturebox.Name = "authorImagePicturebox";
            this.authorImagePicturebox.Size = new System.Drawing.Size(72, 96);
            this.authorImagePicturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.authorImagePicturebox.TabIndex = 2;
            this.authorImagePicturebox.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.label4.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(14, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Tour Title *";
            // 
            // tourTitleTextbox
            // 
            this.tourTitleTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tourTitleTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.tourTitleTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tourTitleTextbox.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tourTitleTextbox.ForeColor = System.Drawing.Color.White;
            this.tourTitleTextbox.Location = new System.Drawing.Point(15, 26);
            this.tourTitleTextbox.MaxLength = 100;
            this.tourTitleTextbox.Name = "tourTitleTextbox";
            this.tourTitleTextbox.Size = new System.Drawing.Size(297, 22);
            this.tourTitleTextbox.TabIndex = 1;
            this.tourTitleTextbox.TextChanged += new System.EventHandler(this.tourTitleTextbox_TextChanged);
            this.tourTitleTextbox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tourTitleTextbox_MouseClick);
            this.tourTitleTextbox.Enter += new System.EventHandler(this.tourTitleTextbox_Enter);
            // 
            // tourIDText
            // 
            this.tourIDText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tourIDText.AutoSize = true;
            this.tourIDText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.tourIDText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tourIDText.ForeColor = System.Drawing.Color.White;
            this.tourIDText.Location = new System.Drawing.Point(258, 9);
            this.tourIDText.Name = "tourIDText";
            this.tourIDText.Size = new System.Drawing.Size(48, 13);
            this.tourIDText.TabIndex = 2;
            this.tourIDText.Text = "ID: 0000";
            this.tourIDText.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.tourIDText.Visible = false;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(14, 343);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Organization URL";
            // 
            // orgUrl
            // 
            this.orgUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.orgUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.orgUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.orgUrl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.orgUrl.ForeColor = System.Drawing.Color.White;
            this.orgUrl.Location = new System.Drawing.Point(15, 359);
            this.orgUrl.MaxLength = 1024;
            this.orgUrl.Name = "orgUrl";
            this.orgUrl.Size = new System.Drawing.Size(297, 22);
            this.orgUrl.TabIndex = 12;
            // 
            // ClassificationText
            // 
            this.ClassificationText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ClassificationText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.ClassificationText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ClassificationText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClassificationText.ForeColor = System.Drawing.Color.White;
            this.ClassificationText.Location = new System.Drawing.Point(15, 509);
            this.ClassificationText.MaxLength = 2048;
            this.ClassificationText.Name = "ClassificationText";
            this.ClassificationText.ReadOnly = true;
            this.ClassificationText.Size = new System.Drawing.Size(200, 22);
            this.ClassificationText.TabIndex = 15;
            // 
            // ClassificationTaxonomyLabel
            // 
            this.ClassificationTaxonomyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ClassificationTaxonomyLabel.AutoSize = true;
            this.ClassificationTaxonomyLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ClassificationTaxonomyLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClassificationTaxonomyLabel.ForeColor = System.Drawing.Color.White;
            this.ClassificationTaxonomyLabel.Location = new System.Drawing.Point(12, 491);
            this.ClassificationTaxonomyLabel.Name = "ClassificationTaxonomyLabel";
            this.ClassificationTaxonomyLabel.Size = new System.Drawing.Size(129, 13);
            this.ClassificationTaxonomyLabel.TabIndex = 14;
            this.ClassificationTaxonomyLabel.Text = "Classification Taxonomy";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.label6.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(16, 546);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "* Required Field";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.label7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(12, 260);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(116, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Author Contact Email";
            // 
            // authorEmailText
            // 
            this.authorEmailText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.authorEmailText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.authorEmailText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.authorEmailText.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.authorEmailText.ForeColor = System.Drawing.Color.White;
            this.authorEmailText.Location = new System.Drawing.Point(15, 277);
            this.authorEmailText.MaxLength = 100;
            this.authorEmailText.Name = "authorEmailText";
            this.authorEmailText.Size = new System.Drawing.Size(297, 22);
            this.authorEmailText.TabIndex = 10;
            this.authorEmailText.TextChanged += new System.EventHandler(this.authorEmailText_TextChanged);
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.label8.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(12, 446);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(279, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Catalog Objects and Keywords (semicolon separated)";
            // 
            // keywords
            // 
            this.keywords.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.keywords.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.keywords.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.keywords.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.keywords.ForeColor = System.Drawing.Color.White;
            this.keywords.Location = new System.Drawing.Point(15, 464);
            this.keywords.MaxLength = 256;
            this.keywords.Name = "keywords";
            this.keywords.Size = new System.Drawing.Size(297, 22);
            this.keywords.TabIndex = 15;
            // 
            // ShowTaxTree
            // 
            this.ShowTaxTree.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowTaxTree.BackColor = System.Drawing.Color.Transparent;
            this.ShowTaxTree.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ShowTaxTree.ImageDisabled = null;
            this.ShowTaxTree.ImageEnabled = null;
            this.ShowTaxTree.Location = new System.Drawing.Point(218, 504);
            this.ShowTaxTree.MaximumSize = new System.Drawing.Size(140, 33);
            this.ShowTaxTree.Name = "ShowTaxTree";
            this.ShowTaxTree.Selected = false;
            this.ShowTaxTree.Size = new System.Drawing.Size(102, 33);
            this.ShowTaxTree.TabIndex = 16;
            this.ShowTaxTree.Text = "Classification";
            this.ShowTaxTree.Click += new System.EventHandler(this.Classification_Click);
            // 
            // OK
            // 
            this.OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OK.BackColor = System.Drawing.Color.Transparent;
            this.OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OK.Enabled = false;
            this.OK.ImageDisabled = null;
            this.OK.ImageEnabled = null;
            this.OK.Location = new System.Drawing.Point(154, 537);
            this.OK.MaximumSize = new System.Drawing.Size(140, 33);
            this.OK.Name = "OK";
            this.OK.Selected = false;
            this.OK.Size = new System.Drawing.Size(80, 33);
            this.OK.TabIndex = 19;
            this.OK.Text = "OK";
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(240, 537);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(80, 33);
            this.Cancel.TabIndex = 20;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // ImportAuthorImage
            // 
            this.ImportAuthorImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImportAuthorImage.BackColor = System.Drawing.Color.Transparent;
            this.ImportAuthorImage.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ImportAuthorImage.ImageDisabled = null;
            this.ImportAuthorImage.ImageEnabled = null;
            this.ImportAuthorImage.Location = new System.Drawing.Point(89, 217);
            this.ImportAuthorImage.MaximumSize = new System.Drawing.Size(140, 33);
            this.ImportAuthorImage.Name = "ImportAuthorImage";
            this.ImportAuthorImage.Selected = false;
            this.ImportAuthorImage.Size = new System.Drawing.Size(140, 33);
            this.ImportAuthorImage.TabIndex = 8;
            this.ImportAuthorImage.Text = "Import Image";
            this.ImportAuthorImage.Click += new System.EventHandler(this.ImportAuthorImage_Click);
            // 
            // wwtCombo1
            // 
            this.wwtCombo1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wwtCombo1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.wwtCombo1.DateTimeValue = new System.DateTime(2008, 2, 11, 21, 55, 29, 159);
            this.wwtCombo1.Filter = TerraViewer.Classification.Unfiltered;
            this.wwtCombo1.FilterStyle = true;
            this.wwtCombo1.Location = new System.Drawing.Point(116, 537);
            this.wwtCombo1.Margin = new System.Windows.Forms.Padding(0);
            this.wwtCombo1.MaximumSize = new System.Drawing.Size(248, 33);
            this.wwtCombo1.MinimumSize = new System.Drawing.Size(35, 33);
            this.wwtCombo1.Name = "wwtCombo1";
            this.wwtCombo1.SelectedIndex = -1;
            this.wwtCombo1.Size = new System.Drawing.Size(35, 33);
            this.wwtCombo1.State = TerraViewer.State.Rest;
            this.wwtCombo1.TabIndex = 18;
            this.wwtCombo1.Type = TerraViewer.WwtCombo.ComboType.List;
            this.wwtCombo1.Visible = false;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.label9.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(14, 302);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(107, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Organization Name";
            // 
            // OrganizationName
            // 
            this.OrganizationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.OrganizationName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.OrganizationName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OrganizationName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrganizationName.ForeColor = System.Drawing.Color.White;
            this.OrganizationName.Location = new System.Drawing.Point(15, 318);
            this.OrganizationName.MaxLength = 1024;
            this.OrganizationName.Name = "OrganizationName";
            this.OrganizationName.Size = new System.Drawing.Size(297, 22);
            this.OrganizationName.TabIndex = 12;
            // 
            // TourProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(324, 575);
            this.Controls.Add(this.ShowTaxTree);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.ImportAuthorImage);
            this.Controls.Add(this.wwtCombo1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.authorImagePicturebox);
            this.Controls.Add(this.tourDescriptionTextbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OrganizationName);
            this.Controls.Add(this.orgUrl);
            this.Controls.Add(this.tourTitleTextbox);
            this.Controls.Add(this.keywords);
            this.Controls.Add(this.ClassificationText);
            this.Controls.Add(this.authorEmailText);
            this.Controls.Add(this.authorNameTextbox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tourIDText);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ClassificationTaxonomyLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(340, 560);
            this.Name = "TourProperties";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tour Properties";
            this.Load += new System.EventHandler(this.TourProperties_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TourProperties_KeyUp);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.authorImagePicturebox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox authorNameTextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tourDescriptionTextbox;
        private System.Windows.Forms.PictureBox authorImagePicturebox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton AdvancedOption;
        private System.Windows.Forms.RadioButton IntermediateOption;
        private System.Windows.Forms.RadioButton BeginnerOption;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tourTitleTextbox;
        private WwtCombo wwtCombo1;
        private WwtButton ImportAuthorImage;
        private WwtButton Cancel;
        private WwtButton OK;
        private System.Windows.Forms.Label tourIDText;
        private WwtButton ShowTaxTree;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox orgUrl;
        private System.Windows.Forms.TextBox ClassificationText;
        private System.Windows.Forms.Label ClassificationTaxonomyLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox authorEmailText;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox keywords;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox OrganizationName;
    }
}