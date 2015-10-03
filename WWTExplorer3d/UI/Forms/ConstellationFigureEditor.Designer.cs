namespace TerraViewer
{
    partial class ConstellationFigureEditor
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
            this.figureTree = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.closeBox = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AddPoint = new TerraViewer.WwtButton();
            this.DeletePoint = new TerraViewer.WwtButton();
            this.SaveFigures = new TerraViewer.WwtButton();
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).BeginInit();
            this.SuspendLayout();
            // 
            // figureTree
            // 
            this.figureTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.figureTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.figureTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.figureTree.CheckBoxes = true;
            this.figureTree.ForeColor = System.Drawing.Color.White;
            this.figureTree.HideSelection = false;
            this.figureTree.HotTracking = true;
            this.figureTree.LineColor = System.Drawing.Color.White;
            this.figureTree.Location = new System.Drawing.Point(12, 27);
            this.figureTree.Name = "figureTree";
            this.figureTree.ShowNodeToolTips = true;
            this.figureTree.Size = new System.Drawing.Size(224, 319);
            this.figureTree.TabIndex = 1;
            this.figureTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.figureTree_AfterCheck);
            this.figureTree.NodeMouseHover += new System.Windows.Forms.TreeNodeMouseHoverEventHandler(this.figureTree_NodeMouseHover);
            this.figureTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.figureTree_AfterSelect);
            this.figureTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.figureTree_NodeMouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Constellation Figure Editor";
            // 
            // closeBox
            // 
            this.closeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeBox.Image = global::TerraViewer.Properties.Resources.CloseRest;
            this.closeBox.Location = new System.Drawing.Point(231, 5);
            this.closeBox.Name = "closeBox";
            this.closeBox.Size = new System.Drawing.Size(16, 16);
            this.closeBox.TabIndex = 3;
            this.closeBox.TabStop = false;
            this.closeBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.closeBox_MouseDown);
            this.closeBox.MouseEnter += new System.EventHandler(this.closeBox_MouseEnter);
            this.closeBox.MouseLeave += new System.EventHandler(this.closeBox_MouseLeave);
            this.closeBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.closeBox_MouseUp);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 349);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Right-click on star to add point";
            // 
            // AddPoint
            // 
            this.AddPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddPoint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.AddPoint.DialogResult = System.Windows.Forms.DialogResult.None;
            this.AddPoint.ImageDisabled = null;
            this.AddPoint.ImageEnabled = null;
            this.AddPoint.Location = new System.Drawing.Point(78, 370);
            this.AddPoint.MaximumSize = new System.Drawing.Size(140, 33);
            this.AddPoint.Name = "AddPoint";
            this.AddPoint.Selected = false;
            this.AddPoint.Size = new System.Drawing.Size(61, 33);
            this.AddPoint.TabIndex = 4;
            this.AddPoint.Text = "Add";
            this.AddPoint.Visible = false;
            this.AddPoint.Click += new System.EventHandler(this.AddPoint_Click);
            // 
            // DeletePoint
            // 
            this.DeletePoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DeletePoint.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.DeletePoint.DialogResult = System.Windows.Forms.DialogResult.None;
            this.DeletePoint.ImageDisabled = null;
            this.DeletePoint.ImageEnabled = null;
            this.DeletePoint.Location = new System.Drawing.Point(8, 370);
            this.DeletePoint.MaximumSize = new System.Drawing.Size(140, 33);
            this.DeletePoint.Name = "DeletePoint";
            this.DeletePoint.Selected = false;
            this.DeletePoint.Size = new System.Drawing.Size(73, 33);
            this.DeletePoint.TabIndex = 4;
            this.DeletePoint.Text = "Delete";
            this.DeletePoint.Click += new System.EventHandler(this.DeletePoint_Click);
            // 
            // SaveFigures
            // 
            this.SaveFigures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveFigures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.SaveFigures.DialogResult = System.Windows.Forms.DialogResult.None;
            this.SaveFigures.ImageDisabled = null;
            this.SaveFigures.ImageEnabled = null;
            this.SaveFigures.Location = new System.Drawing.Point(173, 370);
            this.SaveFigures.MaximumSize = new System.Drawing.Size(140, 33);
            this.SaveFigures.Name = "SaveFigures";
            this.SaveFigures.Selected = false;
            this.SaveFigures.Size = new System.Drawing.Size(69, 33);
            this.SaveFigures.TabIndex = 4;
            this.SaveFigures.Text = "Save";
            this.SaveFigures.Click += new System.EventHandler(this.SaveFigures_Click);
            // 
            // ConstellationFigureEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(248, 403);
            this.ControlBox = false;
            this.Controls.Add(this.AddPoint);
            this.Controls.Add(this.DeletePoint);
            this.Controls.Add(this.SaveFigures);
            this.Controls.Add(this.closeBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.figureTree);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(210, 180);
            this.Name = "ConstellationFigureEditor";
            this.Opacity = 0.8D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ConstellationFigureEditor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConstellationFigureEditor_FormClosed);
            this.Load += new System.EventHandler(this.ConstellationFigureEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.closeBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView figureTree;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox closeBox;
        private WwtButton SaveFigures;
        private WwtButton DeletePoint;
        private WwtButton AddPoint;
        private System.Windows.Forms.Label label2;
    }
}