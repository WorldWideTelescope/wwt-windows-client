namespace TerraViewer
{
    partial class ClientNodeList
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
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.nodeTree = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.ShowDetail = new TerraViewer.WWTCheckbox();
            this.SuspendLayout();
            // 
            // updateTimer
            // 
            this.updateTimer.Enabled = true;
            this.updateTimer.Interval = 1000;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // nodeTree
            // 
            this.nodeTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nodeTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.nodeTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nodeTree.ForeColor = System.Drawing.Color.White;
            this.nodeTree.HideSelection = false;
            this.nodeTree.HotTracking = true;
            this.nodeTree.LineColor = System.Drawing.Color.White;
            this.nodeTree.Location = new System.Drawing.Point(12, 28);
            this.nodeTree.Name = "nodeTree";
            this.nodeTree.ShowNodeToolTips = true;
            this.nodeTree.Size = new System.Drawing.Size(167, 476);
            this.nodeTree.TabIndex = 1;
            this.nodeTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.nodeTree_NodeMouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Projector Server List";
            // 
            // ShowDetail
            // 
            this.ShowDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ShowDetail.Checked = true;
            this.ShowDetail.Location = new System.Drawing.Point(12, 510);
            this.ShowDetail.Name = "ShowDetail";
            this.ShowDetail.Size = new System.Drawing.Size(102, 25);
            this.ShowDetail.TabIndex = 4;
            this.ShowDetail.Text = "Show Details";
            this.ShowDetail.CheckedChanged += new System.EventHandler(this.ShowDetail_CheckedChanged);
            // 
            // ClientNodeList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(190, 573);
            this.Controls.Add(this.ShowDetail);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nodeTree);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::TerraViewer.Properties.Settings.Default, "ClientNodeListLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.Location = global::TerraViewer.Properties.Settings.Default.ClientNodeListLocation;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(206, 1600);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(206, 200);
            this.Name = "ClientNodeList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Projector Servers";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ClientNodeList_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ClientNodeList_FormClosed);
            this.Load += new System.EventHandler(this.ClientNodeList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.TreeView nodeTree;
        private System.Windows.Forms.Label label2;
        private WWTCheckbox ShowDetail;
    }
}