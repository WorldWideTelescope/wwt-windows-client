using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DomeSetup : Form
    {
        public DomeSetup()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.label1.Text = Language.GetLocalizedText(942, "Dome Tilt");
            this.domeTypeLabel.Text = Language.GetLocalizedText(943, "Dome Type");
            this.CustomFilenameLabel.Text = Language.GetLocalizedText(944, "Custom Warp Filename");
            this.largeTextures.Text = " " + Language.GetLocalizedText(945, "Large Textures");
            this.OK.Text = Language.GetLocalizedText(156, "OK");
            this.Text = Language.GetLocalizedText(575, "Dome Setup");
            this.browseButton.Text = Language.GetLocalizedText(884, "Browse");
            this.flatScreenWarp.Text = Language.GetLocalizedText(1353, "Warp from flat screen");
            this.label2.Text = Language.GetLocalizedText(1354, "Dome Alt");
            this.DomeAz.Name = Language.GetLocalizedText(1355, "DomeAz");
            this.DomeNorth.Text = Language.GetLocalizedText(1356, "Face North in Sky Mode");
            this.label4.Text = Language.GetLocalizedText(1357, "Scriptable Parameters");

        }

        private void domeTilt_ValueChanged(object sender, EventArgs e)
        {
            tiltEdit.Text = domeTilt.Value.ToString();

        }

        private void tiltEdit_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Properties.Settings.Default.DomeTilt = Convert.ToDouble(tiltEdit.Text);
                Earth3d.MainWindow.Config.DomeTilt = (float)Properties.Settings.Default.DomeTilt;
                tiltEdit.BackColor = customWarpFilename.BackColor;
            }
            catch
            {
                tiltEdit.BackColor = Color.Red;

            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CustomWarpFilename = customWarpFilename.Text;
            Properties.Settings.Default.DomeTypeIndex = domeTypeCombo.SelectedIndex;
            Earth3d.MainWindow.CreateWarpVertexBuffer();

            if (domeTypeCombo.SelectedIndex == 3 && String.IsNullOrEmpty(Properties.Settings.Default.CustomWarpFilename))
            {
                Properties.Settings.Default.DomeTypeIndex = 0;
            }
            this.Close();
        }

        private void largeTextures_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LargeDomeTextures = largeTextures.Checked;

        }

        private void domeTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.DomeTypeIndex = domeTypeCombo.SelectedIndex;
            browseButton.Enabled = domeTypeCombo.SelectedIndex == 3;
            customWarpFilename.Enabled = domeTypeCombo.SelectedIndex == 3;
            Earth3d.MainWindow.CreateWarpVertexBuffer();

        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Language.GetLocalizedText(1178, "Dome Warp Files") + "|*.data";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                customWarpFilename.Text = openFile.FileName;
                Properties.Settings.Default.CustomWarpFilename = openFile.FileName;
                Earth3d.MainWindow.CreateWarpVertexBuffer();
            }
        }

        private void DomeSetup_Load(object sender, EventArgs e)
        {
            tiltEdit.Text = Properties.Settings.Default.DomeTilt.ToString();
            domeTilt.Value = (int)Properties.Settings.Default.DomeTilt;
            largeTextures.Checked = Properties.Settings.Default.LargeDomeTextures;
            domeTypeCombo.Items.AddRange(new string[] { "Fisheye", "Mirrordome 16:9", "Mirrordome 4:3", "<Custom Warp>" });
            domeTypeCombo.SelectedIndex = Properties.Settings.Default.DomeTypeIndex;
            customWarpFilename.Text = Properties.Settings.Default.CustomWarpFilename;
            flatScreenWarp.Checked = Properties.Settings.Default.FlatScreenWarp;
            DomeAlt.Text = Earth3d.MainWindow.viewCamera.DomeAlt.ToString();
            DomeAz.Text = Earth3d.MainWindow.viewCamera.DomeAz.ToString();
            DomeNorth.Checked = Properties.Settings.Default.FaceNorth;
        }

        private void flatScreenWarp_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.FlatScreenWarp = flatScreenWarp.Checked;
        }

        private void DomeAlt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Earth3d.MainWindow.viewCamera.DomeAlt = Convert.ToDouble(DomeAlt.Text);
                DomeAlt.BackColor = customWarpFilename.BackColor;
            }
            catch
            {
                DomeAlt.BackColor = Color.Red;
            }
        }

        private void DomeAz_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Earth3d.MainWindow.viewCamera.DomeAz = Convert.ToDouble(DomeAz.Text);
                DomeAz.BackColor = customWarpFilename.BackColor;
            }
            catch
            {
                DomeAz.BackColor = Color.Red;
            }
        }

        private void DomeNorth_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.FaceNorth = DomeNorth.Checked;
            Properties.Settings.Default.ColSettingsVersion++;
        }
    }
    
}
