#region Using directives

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace TerraViewer
{
	public class Queue_List : Form
	{
        private Timer timer1;
        private IContainer components;
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
            checkTilesInView.Text = Language.GetLocalizedText(306, "Tile in View Frustum");
            wwtButton1.Text = Language.GetLocalizedText(307, "Flush Queue");
            ClearCache.Text = Language.GetLocalizedText(937, "Clear Cache");
            Text = Language.GetLocalizedText(938, "Download Queue");
        }

        private void InitializeComponent()
        {
            components = new Container();
            timer1 = new Timer(components);
            checkTilesInView = new CheckBox();
            listBox = new ListBox();
            wwtButton1 = new WwtButton();
            ClearCache = new WwtButton();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // checkTilesInView
            // 
            checkTilesInView.AutoSize = true;
            checkTilesInView.Location = new Point(12, 716);
            checkTilesInView.Name = "checkTilesInView";
            checkTilesInView.Size = new Size(123, 17);
            checkTilesInView.TabIndex = 3;
            checkTilesInView.Text = "Tile in View Frustrum";
            checkTilesInView.UseVisualStyleBackColor = true;
            checkTilesInView.Visible = false;
            checkTilesInView.CheckedChanged += FrustromTileView_CheckedChanged;
            // 
            // listBox
            // 
            listBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom)
                                   | AnchorStyles.Left)
                                  | AnchorStyles.Right;
            listBox.BackColor = Color.FromArgb(68, 82, 105);
            listBox.BorderStyle = BorderStyle.None;
            listBox.ForeColor = Color.White;
            listBox.FormattingEnabled = true;
            listBox.Location = new Point(8, 12);
            listBox.Name = "listBox";
            listBox.Size = new Size(272, 715);
            listBox.TabIndex = 4;
            listBox.MouseDoubleClick += listBox_MouseDoubleClick;
            listBox.SelectedIndexChanged += listBox_SelectedIndexChanged;
            // 
            // wwtButton1
            // 
            wwtButton1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            wwtButton1.BackColor = Color.Transparent;
            wwtButton1.DialogResult = DialogResult.None;
            wwtButton1.ImageDisabled = null;
            wwtButton1.ImageEnabled = null;
            wwtButton1.Location = new Point(3, 737);
            wwtButton1.MaximumSize = new Size(140, 33);
            wwtButton1.Name = "wwtButton1";
            wwtButton1.Selected = false;
            wwtButton1.Size = new Size(115, 33);
            wwtButton1.TabIndex = 5;
            wwtButton1.Text = "Flush Queue";
            wwtButton1.Click += PurgeQueue_Click;
            // 
            // ClearCache
            // 
            ClearCache.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ClearCache.BackColor = Color.Transparent;
            ClearCache.DialogResult = DialogResult.None;
            ClearCache.ImageDisabled = null;
            ClearCache.ImageEnabled = null;
            ClearCache.Location = new Point(173, 737);
            ClearCache.MaximumSize = new Size(140, 33);
            ClearCache.Name = "ClearCache";
            ClearCache.Selected = false;
            ClearCache.Size = new Size(112, 33);
            ClearCache.TabIndex = 6;
            ClearCache.Text = "Clear Cache";
            ClearCache.Click += ClearCache_Click;
            // 
            // Queue_List
            // 
            AutoScaleBaseSize = new Size(5, 13);
            BackColor = Color.FromArgb(20, 23, 31);
            ClientSize = new Size(292, 774);
            Controls.Add(ClearCache);
            Controls.Add(wwtButton1);
            Controls.Add(listBox);
            Controls.Add(checkTilesInView);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "Queue_List";
            Opacity = 0.7;
            ShowIcon = false;
            ShowInTaskbar = false;
            Text = "Download Queue";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();

		}


        private void timer1_Tick(object sender, EventArgs e)
        {
            var tiles = TileCache.GetQueueList();
            listBox.Items.Clear();
            listBox.Items.AddRange(tiles);
        }

		private void PurgeQueue_Click(object sender, EventArgs e)
		{
			TileCache.PurgeQueue();
		}

		private void ClearCache_Click(object sender, EventArgs e)
		{
			TileCache.ClearCache();
		}

        bool showFursumTiles;
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
                var tile = (Tile)listBox.SelectedItem;

                Process.Start(tile.URL);
            }
        }
	}
}