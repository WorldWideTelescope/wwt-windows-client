namespace TerraViewer
{
    partial class RemoteAccessControl
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
            this.AcceptLabel = new System.Windows.Forms.Label();
            this.AcceptList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ipPart1 = new System.Windows.Forms.TextBox();
            this.ipPart2 = new System.Windows.Forms.TextBox();
            this.ipPart3 = new System.Windows.Forms.TextBox();
            this.ipPart4 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.AcceptRemote = new TerraViewer.WWTCheckbox();
            this.AcceptLocal = new TerraViewer.WWTCheckbox();
            this.Add = new TerraViewer.WwtButton();
            this.DeleteAccept = new TerraViewer.WwtButton();
            this.ClearAccept = new TerraViewer.WwtButton();
            this.Ok = new TerraViewer.WwtButton();
            this.Cancel = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // AcceptLabel
            // 
            this.AcceptLabel.AutoSize = true;
            this.AcceptLabel.Location = new System.Drawing.Point(12, 119);
            this.AcceptLabel.Name = "AcceptLabel";
            this.AcceptLabel.Size = new System.Drawing.Size(60, 13);
            this.AcceptLabel.TabIndex = 11;
            this.AcceptLabel.Text = "Accept List";
            // 
            // AcceptList
            // 
            this.AcceptList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.AcceptList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(21)))), ((int)(((byte)(31)))));
            this.AcceptList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AcceptList.ForeColor = System.Drawing.Color.White;
            this.AcceptList.FormattingEnabled = true;
            this.AcceptList.Location = new System.Drawing.Point(12, 135);
            this.AcceptList.Name = "AcceptList";
            this.AcceptList.Size = new System.Drawing.Size(236, 143);
            this.AcceptList.TabIndex = 12;
            this.AcceptList.SelectedIndexChanged += new System.EventHandler(this.AcceptList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(255, 48);
            this.label2.TabIndex = 0;
            this.label2.Text = "Using the accept list you can manage what client host are allowed remote applicat" +
                "ion control of WorldWide Telescope.";
            // 
            // ipPart1
            // 
            this.ipPart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ipPart1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ipPart1.ForeColor = System.Drawing.Color.White;
            this.ipPart1.Location = new System.Drawing.Point(15, 88);
            this.ipPart1.Name = "ipPart1";
            this.ipPart1.Size = new System.Drawing.Size(26, 20);
            this.ipPart1.TabIndex = 2;
            this.ipPart1.Text = "255";
            this.ipPart1.TextChanged += new System.EventHandler(this.ipPart1_TextChanged);
            this.ipPart1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ipPart1_KeyDown);
            // 
            // ipPart2
            // 
            this.ipPart2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ipPart2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ipPart2.ForeColor = System.Drawing.Color.White;
            this.ipPart2.Location = new System.Drawing.Point(57, 87);
            this.ipPart2.Name = "ipPart2";
            this.ipPart2.Size = new System.Drawing.Size(26, 20);
            this.ipPart2.TabIndex = 4;
            this.ipPart2.Text = "255";
            this.ipPart2.TextChanged += new System.EventHandler(this.ipPart2_TextChanged);
            this.ipPart2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ipPart2_KeyDown);
            // 
            // ipPart3
            // 
            this.ipPart3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ipPart3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ipPart3.ForeColor = System.Drawing.Color.White;
            this.ipPart3.Location = new System.Drawing.Point(99, 87);
            this.ipPart3.Name = "ipPart3";
            this.ipPart3.Size = new System.Drawing.Size(26, 20);
            this.ipPart3.TabIndex = 6;
            this.ipPart3.Text = "255";
            this.ipPart3.TextChanged += new System.EventHandler(this.ipPart3_TextChanged);
            this.ipPart3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ipPart3_KeyDown);
            // 
            // ipPart4
            // 
            this.ipPart4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
            this.ipPart4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ipPart4.ForeColor = System.Drawing.Color.White;
            this.ipPart4.Location = new System.Drawing.Point(141, 87);
            this.ipPart4.Name = "ipPart4";
            this.ipPart4.Size = new System.Drawing.Size(26, 20);
            this.ipPart4.TabIndex = 8;
            this.ipPart4.Text = "255";
            this.ipPart4.TextChanged += new System.EventHandler(this.ipPart4_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(84, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(15, 24);
            this.label4.TabIndex = 5;
            this.label4.Text = ".";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(41, 86);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(15, 24);
            this.label5.TabIndex = 3;
            this.label5.Text = ".";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(127, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 24);
            this.label6.TabIndex = 7;
            this.label6.Text = ".";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 69);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(150, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "IP Address (Use * for wildcard)";
            // 
            // AcceptRemote
            // 
            this.AcceptRemote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AcceptRemote.Checked = false;
            this.AcceptRemote.Location = new System.Drawing.Point(9, 344);
            this.AcceptRemote.Name = "AcceptRemote";
            this.AcceptRemote.Size = new System.Drawing.Size(190, 25);
            this.AcceptRemote.TabIndex = 20;
            this.AcceptRemote.Text = "Accept All Remote Connections";
            // 
            // AcceptLocal
            // 
            this.AcceptLocal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AcceptLocal.Checked = false;
            this.AcceptLocal.Location = new System.Drawing.Point(9, 323);
            this.AcceptLocal.Name = "AcceptLocal";
            this.AcceptLocal.Size = new System.Drawing.Size(190, 25);
            this.AcceptLocal.TabIndex = 19;
            this.AcceptLocal.Text = "Accept Local Connections";
            // 
            // Add
            // 
            this.Add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Add.BackColor = System.Drawing.Color.Transparent;
            this.Add.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Add.ImageDisabled = null;
            this.Add.ImageEnabled = null;
            this.Add.Location = new System.Drawing.Point(177, 78);
            this.Add.MaximumSize = new System.Drawing.Size(140, 33);
            this.Add.Name = "Add";
            this.Add.Selected = false;
            this.Add.Size = new System.Drawing.Size(81, 33);
            this.Add.TabIndex = 9;
            this.Add.Text = "Add";
            this.Add.Click += new System.EventHandler(this.Accept_Click);
            // 
            // DeleteAccept
            // 
            this.DeleteAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DeleteAccept.BackColor = System.Drawing.Color.Transparent;
            this.DeleteAccept.DialogResult = System.Windows.Forms.DialogResult.None;
            this.DeleteAccept.ImageDisabled = null;
            this.DeleteAccept.ImageEnabled = null;
            this.DeleteAccept.Location = new System.Drawing.Point(131, 284);
            this.DeleteAccept.MaximumSize = new System.Drawing.Size(140, 33);
            this.DeleteAccept.Name = "DeleteAccept";
            this.DeleteAccept.Selected = false;
            this.DeleteAccept.Size = new System.Drawing.Size(60, 33);
            this.DeleteAccept.TabIndex = 13;
            this.DeleteAccept.Text = "Delete";
            this.DeleteAccept.Click += new System.EventHandler(this.DeleteAccept_Click);
            // 
            // ClearAccept
            // 
            this.ClearAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearAccept.BackColor = System.Drawing.Color.Transparent;
            this.ClearAccept.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ClearAccept.ImageDisabled = null;
            this.ClearAccept.ImageEnabled = null;
            this.ClearAccept.Location = new System.Drawing.Point(193, 284);
            this.ClearAccept.MaximumSize = new System.Drawing.Size(140, 33);
            this.ClearAccept.Name = "ClearAccept";
            this.ClearAccept.Selected = false;
            this.ClearAccept.Size = new System.Drawing.Size(60, 33);
            this.ClearAccept.TabIndex = 14;
            this.ClearAccept.Text = "Clear";
            this.ClearAccept.Click += new System.EventHandler(this.ClearAccept_Click);
            // 
            // Ok
            // 
            this.Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Ok.BackColor = System.Drawing.Color.Transparent;
            this.Ok.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Ok.ImageDisabled = null;
            this.Ok.ImageEnabled = null;
            this.Ok.Location = new System.Drawing.Point(102, 372);
            this.Ok.MaximumSize = new System.Drawing.Size(140, 33);
            this.Ok.Name = "Ok";
            this.Ok.Selected = false;
            this.Ok.Size = new System.Drawing.Size(81, 33);
            this.Ok.TabIndex = 21;
            this.Ok.Text = "Ok";
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // Cancel
            // 
            this.Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Cancel.BackColor = System.Drawing.Color.Transparent;
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.None;
            this.Cancel.ImageDisabled = null;
            this.Cancel.ImageEnabled = null;
            this.Cancel.Location = new System.Drawing.Point(177, 372);
            this.Cancel.MaximumSize = new System.Drawing.Size(140, 33);
            this.Cancel.Name = "Cancel";
            this.Cancel.Selected = false;
            this.Cancel.Size = new System.Drawing.Size(81, 33);
            this.Cancel.TabIndex = 22;
            this.Cancel.Text = "Cancel";
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // RemoteAccessControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(30)))), ((int)(((byte)(39)))));
            this.ClientSize = new System.Drawing.Size(270, 417);
            this.ControlBox = false;
            this.Controls.Add(this.ipPart4);
            this.Controls.Add(this.ipPart3);
            this.Controls.Add(this.ipPart2);
            this.Controls.Add(this.ipPart1);
            this.Controls.Add(this.AcceptRemote);
            this.Controls.Add(this.AcceptLocal);
            this.Controls.Add(this.AcceptList);
            this.Controls.Add(this.Add);
            this.Controls.Add(this.DeleteAccept);
            this.Controls.Add(this.ClearAccept);
            this.Controls.Add(this.Ok);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.AcceptLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RemoteAccessControl";
            this.Text = "Remote Access Control";
            this.Load += new System.EventHandler(this.WebAccessList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AcceptLabel;
        private WwtButton Cancel;
        private WwtButton Ok;
        private System.Windows.Forms.ListBox AcceptList;
        private System.Windows.Forms.Label label2;
        private WwtButton ClearAccept;
        private WwtButton DeleteAccept;
        private WWTCheckbox AcceptLocal;
        private WWTCheckbox AcceptRemote;
        private System.Windows.Forms.TextBox ipPart1;
        private System.Windows.Forms.TextBox ipPart2;
        private System.Windows.Forms.TextBox ipPart3;
        private System.Windows.Forms.TextBox ipPart4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private WwtButton Add;
    }
}