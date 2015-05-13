#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#endregion

namespace TerraViewer
{
	public class Queue_List : Form
	{
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.IContainer components;
        private ListBox listBox;
        private WwtButton wwtButton1;
        private WwtButton ClearCache;
        private CheckBox checkTilesInView;
    
		public Queue_List()
		{
            InitializeComponent(); 
            SetUiStrings();
		}

        private void SetUiStrings()
        {
            this.checkTilesInView.Text = Language.GetLocalizedText(306, "Tile in View Frustum");
            this.wwtButton1.Text = Language.GetLocalizedText(307, "Flush Queue");
            this.ClearCache.Text = Language.GetLocalizedText(937, "Clear Cache");
            this.Text = Language.GetLocalizedText(938, "Download Queue");
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.checkTilesInView = new System.Windows.Forms.CheckBox();
            this.listBox = new System.Windows.Forms.ListBox();
            this.wwtButton1 = new TerraViewer.WwtButton();
            this.ClearCache = new TerraViewer.WwtButton();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // checkTilesInView
            // 
            this.checkTilesInView.AutoSize = true;
            this.checkTilesInView.Location = new System.Drawing.Point(12, 716);
            this.checkTilesInView.Name = "checkTilesInView";
            this.checkTilesInView.Size = new System.Drawing.Size(123, 17);
            this.checkTilesInView.TabIndex = 3;
            this.checkTilesInView.Text = "Tile in View Frustrum";
            this.checkTilesInView.UseVisualStyleBackColor = true;
            this.checkTilesInView.Visible = false;
            this.checkTilesInView.CheckedChanged += new System.EventHandler(this.FrustromTileView_CheckedChanged);
            // 
            // listBox
            // 
            this.listBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(82)))), ((int)(((byte)(105)))));
            this.listBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox.ForeColor = System.Drawing.Color.White;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(8, 12);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(272, 715);
            this.listBox.TabIndex = 4;
            this.listBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox_MouseDoubleClick);
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.listBox_SelectedIndexChanged);
            // 
            // wwtButton1
            // 
            this.wwtButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.wwtButton1.BackColor = System.Drawing.Color.Transparent;
            this.wwtButton1.DialogResult = System.Windows.Forms.DialogResult.None;
            this.wwtButton1.ImageDisabled = null;
            this.wwtButton1.ImageEnabled = null;
            this.wwtButton1.Location = new System.Drawing.Point(3, 737);
            this.wwtButton1.MaximumSize = new System.Drawing.Size(140, 33);
            this.wwtButton1.Name = "wwtButton1";
            this.wwtButton1.Selected = false;
            this.wwtButton1.Size = new System.Drawing.Size(115, 33);
            this.wwtButton1.TabIndex = 5;
            this.wwtButton1.Text = "Flush Queue";
            this.wwtButton1.Click += new System.EventHandler(this.PurgeQueue_Click);
            // 
            // ClearCache
            // 
            this.ClearCache.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearCache.BackColor = System.Drawing.Color.Transparent;
            this.ClearCache.DialogResult = System.Windows.Forms.DialogResult.None;
            this.ClearCache.ImageDisabled = null;
            this.ClearCache.ImageEnabled = null;
            this.ClearCache.Location = new System.Drawing.Point(173, 737);
            this.ClearCache.MaximumSize = new System.Drawing.Size(140, 33);
            this.ClearCache.Name = "ClearCache";
            this.ClearCache.Selected = false;
            this.ClearCache.Size = new System.Drawing.Size(112, 33);
            this.ClearCache.TabIndex = 6;
            this.ClearCache.Text = "Clear Cache";
            this.ClearCache.Click += new System.EventHandler(this.ClearCache_Click);
            // 
            // Queue_List
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(23)))), ((int)(((byte)(31)))));
            this.ClientSize = new System.Drawing.Size(292, 774);
            this.Controls.Add(this.ClearCache);
            this.Controls.Add(this.wwtButton1);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.checkTilesInView);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Queue_List";
            this.Opacity = 0.7;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Download Queue";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

		}


        private void timer1_Tick(object sender, System.EventArgs e)
        {
            object[] tiles = TileCache.GetQueueList();
            this.listBox.Items.Clear();
            this.listBox.Items.AddRange(tiles);
        }

		private void PurgeQueue_Click(object sender, EventArgs e)
		{
			TileCache.PurgeQueue();
		}

		private void ClearCache_Click(object sender, EventArgs e)
		{
			TileCache.ClearCache();
		}

        bool showFursumTiles = false;
        private void FrustromTileView_CheckedChanged(object sender, EventArgs e)
        {
            showFursumTiles = checkTilesInView.Checked;
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                Tile tile = (Tile)listBox.SelectedItem;

                System.Diagnostics.Process.Start(tile.URL);
            }
        }
	}
}