using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace TerraViewer
{
    public partial class SettingsTab : TabForm
    {
        public SettingsTab()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            groupBox2.Text = Language.GetLocalizedText(333, "Experience");
            toolTips.SetToolTip(zoomSpeed, Language.GetLocalizedText(334, "Select how rapidly the view changes zoom levels"));
            toolTips.SetToolTip(imageQuality, Language.GetLocalizedText(335, "Allows tradeoff of update performance against display quality for slower or faster computers"));
            label9.Text = Language.GetLocalizedText(336, "Slow");
            label8.Text = Language.GetLocalizedText(337, "Fast");
            label7.Text = Language.GetLocalizedText(338, "Zoom Speed");
            label1.Text = Language.GetLocalizedText(339, "Image Quality");
            label3.Text = Language.GetLocalizedText(340, "Sharper");
            label2.Text = Language.GetLocalizedText(341, "Faster");
            transparentTabs.Text = Language.GetLocalizedText(342, "Transparent Tabs");
            toolTips.SetToolTip(transparentTabs, Language.GetLocalizedText(343, "Show tabs and context panel with partial transparency."));
            autoHideContext.Text = Language.GetLocalizedText(344, "Auto Hide Context");
            toolTips.SetToolTip(autoHideContext, Language.GetLocalizedText(345, "Fades out the context panel when the mouse is over the main view area"));
            useFullBrowser.Text = Language.GetLocalizedText(346, "Full Web Browser");
            toolTips.SetToolTip(useFullBrowser, "Launches a full browser for web links rather than the web window");
            autoHideTabs.Text = Language.GetLocalizedText(348, "Auto Hide Tabs");
            toolTips.SetToolTip(autoHideTabs, Language.GetLocalizedText(349, "Fades out tab pane when the mouse is in the Field of View"));
            zoomToCursor.Text = Language.GetLocalizedText(350, "Zoom on Mouse");
            toolTips.SetToolTip(zoomToCursor, Language.GetLocalizedText(351, "Follows the mouse cursor when using the mouse wheel to zoom"));
            smoothPan.Text = Language.GetLocalizedText(352, "Smooth Panning");
            toolTips.SetToolTip(smoothPan, Language.GetLocalizedText(353, "Selects smooth panning rather than snapping to mouse movement"));
            groupBox3.Text = Language.GetLocalizedText(354, "Network and Cache");
            toolTips.SetToolTip(proxyName, "Enter your proxy server name here.");
            proxyText.Text = Language.GetLocalizedText(356, "Proxy Server");
            label5.Text = Language.GetLocalizedText(357, "Port");
            ClearCache.Text = Language.GetLocalizedText(359, "Manage Data Cache");

            Delete.Text = Language.GetLocalizedText(167, "Delete");
            EditFigure.Text = Language.GetLocalizedText(502, "Edit");
            newFigures.Text = Language.GetLocalizedText(52, "New");
            label6.Text = Language.GetLocalizedText(503, "Figure Library");
            ConstellationGroup.Text = Language.GetLocalizedText(498, "Constellation Lines");
  

            FullScreenTours.Text = Language.GetLocalizedText(659, "Full Screen Tours");
            showCrosshairs.Text = Language.GetLocalizedText(506, "Reticle/Crosshairs");
        }

        protected override void SetFocusedChild()
        {
            
        }

        private void SettingsTab_Load(object sender, EventArgs e)
        {
            UpdateProperties();
            Properties.Settings.Default.PropertyChanged += Default_PropertyChanged;


            ignoreChanges = true;
            LoadFigures();
            ignoreChanges = false;

        }

        bool ignoreChanges = true;

        private void LoadFigures()
        {
            figureLibrary.Items.Clear();
            var dir = new DirectoryInfo(Properties.Settings.Default.CahceDirectory + @"data\figures");
            foreach (var fi in dir.GetFiles("*.wwtfig"))
            {
                figureLibrary.Items.Add(fi.Name.Substring(0,fi.Name.Length-fi.Extension.Length));
            }

            figureLibrary.SelectedIndex = figureLibrary.Items.IndexOf(Properties.Settings.Default.ConstellationFiguresFile);
            if (figureLibrary.SelectedIndex == -1 && figureLibrary.Items.Count > 0)
            {
                figureLibrary.SelectedIndex = 0;
            }

        }

        public void InstallNewFigureFile(string launchTourFile)
        {

            if (UiTools.ShowMessageBox(Language.GetLocalizedText(514, "Do you want to add these constellation figures to your Figure Library?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var newName = launchTourFile.ToLower().Replace(".wwtfig","");

                newName = newName.Substring(newName.LastIndexOf("\\")+1);
                var input = new SimpleInput(Language.GetLocalizedText(515, "Figure Library Name"), Language.GetLocalizedText(238, "Name"), newName, 32);
                var retry = false;
                do
                {
                    if (input.ShowDialog() == DialogResult.OK)
                    {
                        if (!File.Exists(Constellations.GetFigurePath(input.ResultText)))
                        {
                            var index = figureLibrary.Items.IndexOf(input.ResultText);
                            if (index > -1)
                            {
                                MessageBox.Show(Language.GetLocalizedText(516, "Name already exists, type a different name."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                                retry = true;
                            }
                            else
                            {
                                try
                                {
                                    File.Copy(launchTourFile, Constellations.GetFigurePath(input.ResultText));
                                    var newFigures = new Constellations(input.ResultText, null, false, false);
                                    newFigures.Save(input.ResultText);
                                    figureLibrary.Items.Add(input.ResultText);
                                    figureLibrary.SelectedIndex = figureLibrary.Items.IndexOf(input.ResultText);
                                    retry = false;
                                }
                                catch
                                {
                                    if (File.Exists(Constellations.GetFigurePath(input.ResultText)))
                                    {
                                        File.Delete(Constellations.GetFigurePath(input.ResultText));
                                    }

                                    UiTools.ShowMessageBox(Language.GetLocalizedText(517, "The file is not a valid WorldWide Telescope Constellation Figure File"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                                    return;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(Language.GetLocalizedText(518, "Name already exists, type a different name."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                            retry = true;
                        }
                    }
                    else
                    {
                        return;
                    }
                } while (retry);
            }
        }

        private void figuerLibrary_SelectionChanged(object sender, EventArgs e)
        {
            if (Earth3d.MainWindow.figureEditor != null)
            {
                if (Earth3d.MainWindow.figureEditor.SaveAndClose() != DialogResult.OK)
                {
                    return;
                }
                Earth3d.MainWindow.figureEditor = null;
            }


            if (figureLibrary.SelectedItem == null || figureLibrary.SelectedItem.ToString() == Language.GetLocalizedText(519, "Default Figures"))
            {
                if (figureLibrary.SelectedItem != null)
                {
                    Earth3d.MainWindow.constellationsFigures = new Constellations(Language.GetLocalizedText(519, "Default Figures"), "http://www.worldwidetelescope.org/data/figures.txt", false, false);
                }
                Delete.Enabled = false;
            }
            else
            {
                Delete.Enabled = true;
                Earth3d.MainWindow.constellationsFigures = new Constellations(figureLibrary.SelectedItem.ToString(), null, false, false);
            }

            if (ignoreChanges)
            {
                return;
            }

            if (figureLibrary.SelectedItem != null)
            {
                Properties.Settings.Default.ConstellationFiguresFile = figureLibrary.SelectedItem.ToString();
            }
        }

        private void EditFigure_Click(object sender, EventArgs e)
        {
            if (Earth3d.MainWindow.figureEditor != null)
            {
                if (Earth3d.MainWindow.figureEditor.SaveAndClose() != DialogResult.OK)
                {
                    return;
                }
                Earth3d.MainWindow.figureEditor = null;
            }

            if (figureLibrary.SelectedItem.ToString() == Language.GetLocalizedText(519, "Default Figures"))
            {
                var input = new SimpleInput(Language.GetLocalizedText(515, "Figure Library Name"), Language.GetLocalizedText(238, "Name"), "", 32);
                var retry = false;
                do
                {
                    if (input.ShowDialog() == DialogResult.OK)
                    {
                        if (!File.Exists(Constellations.GetFigurePath(input.ResultText)))
                        {
                            var index = figureLibrary.Items.IndexOf(input.ResultText);
                            if (index > -1)
                            {
                                MessageBox.Show(Language.GetLocalizedText(516, "Name already exists, type a different name."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                                retry = true;
                            }
                            else
                            {
                                Earth3d.MainWindow.constellationsFigures.Save(input.ResultText);
                                figureLibrary.Items.Add(input.ResultText);
                                figureLibrary.SelectedIndex = figureLibrary.Items.IndexOf(input.ResultText);
                                Earth3d.MainWindow.ShowFigureEditorWindow(Earth3d.MainWindow.constellationsFigures);
                                retry = false;
                            }
                        }
                        else
                        {
                            MessageBox.Show(Language.GetLocalizedText(516, "Name already exists, type a different name."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                            retry = true;
                        }
                    }
                    else
                    {
                        return;
                    }
                } while (retry);

            }
            else
            {
                Earth3d.MainWindow.ShowFigureEditorWindow(Earth3d.MainWindow.constellationsFigures);
            }

        }


        private void newFigures_Click(object sender, EventArgs e)
        {
            if (Earth3d.MainWindow.figureEditor != null)
            {
                if (Earth3d.MainWindow.figureEditor.SaveAndClose() != DialogResult.OK)
                {
                    return;
                }
                Earth3d.MainWindow.figureEditor = null;
            }

            var input = new SimpleInput(Language.GetLocalizedText(515, "Figure Library Name"), Language.GetLocalizedText(238, "Name"), "", 32);
            var retry = false;
            do
            {
                if (input.ShowDialog() == DialogResult.OK)
                {
                    if (!File.Exists(Constellations.GetFigurePath(input.ResultText)))
                    {
                        var index = figureLibrary.Items.IndexOf(input.ResultText);
                        if (index > -1)
                        {
                            MessageBox.Show(Language.GetLocalizedText(516, "Name already exists, type a different name."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                            retry = true;
                        }
                        else
                        {
                            var validfileName = @"^[A-Za-z0-9_ ]*$";
                            if (input.ResultText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || !UiTools.ValidateString(input.ResultText, validfileName))
                            {
                                MessageBox.Show(Language.GetLocalizedText(231, "A name can not contain any of the following characters:") + " \\ / : * ? \" < > |", "Constellation Figure Editor");
                                retry = true;
                            }
                            else
                            {
                                var figures = new Constellations(input.ResultText);
                                figures.Save(input.ResultText);
                                figures = null;
                                figureLibrary.Items.Add(input.ResultText);
                                figureLibrary.SelectedIndex = figureLibrary.Items.IndexOf(input.ResultText);
                                Earth3d.MainWindow.ShowFigureEditorWindow(Earth3d.MainWindow.constellationsFigures);
                                retry = false;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(Language.GetLocalizedText(516, "Name already exists, type a different name."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                        retry = true;
                    }
                }
                else
                {
                    return;
                }
            } while (retry);

        }

        private void Delete_Click(object sender, EventArgs e)
        {


            if (MessageBox.Show(Language.GetLocalizedText(520, "Deleting this figure library will delete the constellation figures you created. Do you want to continue?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                if (Earth3d.MainWindow.figureEditor != null)
                {
                    if (Earth3d.MainWindow.figureEditor.SaveAndClose() != DialogResult.OK)
                    {
                        return;
                    }
                    Earth3d.MainWindow.figureEditor = null;
                }
                var filename = Constellations.GetFigurePath(figureLibrary.SelectedItem.ToString());
                try
                {
                    File.Delete(filename);
                    figureLibrary.Items.Remove(figureLibrary.SelectedItem);
                    figureLibrary.SelectedIndex = 0;
                }
                catch
                {
                    MessageBox.Show("The constellation figures file cannot be deleted because the file cannot be found.", "Constellation Figure Editor");
                }
            }
        }
        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            FullScreenTours.Checked = Properties.Settings.Default.FullScreenTours;
            smoothPan.Checked = Properties.Settings.Default.SmoothPan;
            zoomSpeed.Value = Properties.Settings.Default.ZoomSpeed * 50;
            imageQuality.Value = Properties.Settings.Default.ImageQuality;
            zoomToCursor.Checked = Properties.Settings.Default.FollowMouseOnZoom;
            autoHideContext.Checked = Properties.Settings.Default.AutoHideContext;
            autoHideTabs.Checked = Properties.Settings.Default.AutoHideTabs;
            useFullBrowser.Checked = Properties.Settings.Default.ShowLinksInFullBrowser;
            transparentTabs.Checked = Properties.Settings.Default.TranparentWindows;
            proxyName.Text = Properties.Settings.Default.ProxyServer;
            ProxyPort.Text = Properties.Settings.Default.ProxyPort;
            showCrosshairs.Checked = Properties.Settings.Default.ShowCrosshairs;
            ignoreChanges = false;
        }



        private void smoothPan_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            Properties.Settings.Default.SmoothPan = smoothPan.Checked;
        }

        private void zoomSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            var speed = (int)((double)(zoomSpeed.Value) / 50);
            zoomSpeed.Value = speed * 50;
            Properties.Settings.Default.ZoomSpeed = speed;
        }

        private void meshComplexity_ValueChanged(object sender, EventArgs e)
        {

        }

        private void imageQuality_ValueChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            Properties.Settings.Default.ImageQuality = imageQuality.Value;
        }

        private void zoomToCursor_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            Properties.Settings.Default.FollowMouseOnZoom = zoomToCursor.Checked;
        }

        private void autoHideTabs_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            Properties.Settings.Default.AutoHideTabs = autoHideTabs.Checked;

        }

        private void autoHideContext_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            Properties.Settings.Default.AutoHideContext = autoHideContext.Checked;
        }

        private void useFullBrowser_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            Properties.Settings.Default.ShowLinksInFullBrowser = useFullBrowser.Checked;
        }

        private void proxyName_TextChanged(object sender, EventArgs e)
        {

        }

        private void ProxyPort_TextChanged(object sender, EventArgs e)
        {

        }

        private void transparentTabs_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            Properties.Settings.Default.TranparentWindows = transparentTabs.Checked;
        }

        private void ClearCache_Click(object sender, EventArgs e)
        {
            var dialog = new ClearCache();
       
            if (dialog.ShowDialog( Earth3d.MainWindow) == DialogResult.OK)
            {
                Earth3d.MainWindow.Close();
            }
        }

        private void cacheLimit_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void proxyName_Validated(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }          
            
            Properties.Settings.Default.ProxyServer = proxyName.Text;
            Earth3d.UpdateProxySettings();

        }

        private void ProxyPort_Validated(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }      
   
            Properties.Settings.Default.ProxyPort = ProxyPort.Text;
            Earth3d.UpdateProxySettings();

        }

        private void proxyName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProxyPort.Focus();
            }
        }

        private void ProxyPort_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                smoothPan.Focus();
            }
        }

        private void proxyName_Enter(object sender, EventArgs e)
        {
            Earth3d.NoStealFocus = true;
        }

        private void ProxyPort_Enter(object sender, EventArgs e)
        {
            Earth3d.NoStealFocus = true;
        }

        private void proxyName_Leave(object sender, EventArgs e)
        {
            Earth3d.NoStealFocus = false;
        }

        private void ProxyPort_Leave(object sender, EventArgs e)
        {
            Earth3d.NoStealFocus = false;
        }

        private void ProxyPort_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                int x = Convert.ToUInt16(ProxyPort.Text);
                ProxyPort.BackColor = Color.FromArgb(68, 88, 105);
            }
            catch
            {
                ProxyPort.BackColor = Color.Red;
                e.Cancel = true;
            }
        }

        private void proxyName_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(proxyName.Text))
                {
                    var proxyURI = new Uri(String.Format("http://{0}", proxyName.Text));
                }
                proxyName.BackColor = Color.FromArgb(68, 88, 105);
            }
            catch
            {
                proxyName.BackColor = Color.Red;
                e.Cancel = true;  
            }
        }

        private void SettingsTab_Deactivate(object sender, EventArgs e)
        {
            if (proxyName.Focused)
            {
                smoothPan.Focus();
            }
            if (ProxyPort.Focused)
            {
                smoothPan.Focus();
            }



        }

        private void FullScreenTours_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            Properties.Settings.Default.FullScreenTours = FullScreenTours.Checked;

        }


        private void ExportCache_Click(object sender, EventArgs e)
        {
            Earth3d.ExtractCache();
        }

        private void ImportCache_Click(object sender, EventArgs e)
        {
            Earth3d.RestoreCache();
        }

        private void showCrosshairs_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges)
            {
                return;
            }
            Properties.Settings.Default.ShowCrosshairs = showCrosshairs.Checked;
        }
    }
}