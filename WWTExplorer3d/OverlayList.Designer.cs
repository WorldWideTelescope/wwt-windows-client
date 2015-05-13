namespace TerraViewer
{
    partial class OverlayList
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
            this.ItemList = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Overlay Item List";
            // 
            // ItemList
            // 
            this.ItemList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ItemList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ItemList.CheckBoxes = true;
            this.ItemList.ForeColor = System.Drawing.Color.White;
            this.ItemList.Location = new System.Drawing.Point(16, 29);
            this.ItemList.Name = "ItemList";
            this.ItemList.Size = new System.Drawing.Size(253, 705);
            this.ItemList.TabIndex = 1;
            this.ItemList.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.ItemList_AfterCheck);
            this.ItemList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ItemList_AfterSelect);
            this.ItemList.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.ItemList_NodeMouseClick);
            // 
            // OverlayList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(22)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(281, 746);
            this.Controls.Add(this.ItemList);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.White;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OverlayList";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Overlay List";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OverlayList_FormClosed);
            this.Load += new System.EventHandler(this.OverlayList_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TreeView ItemList;
    }
}