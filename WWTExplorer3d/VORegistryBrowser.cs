using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TerraViewer.edu.stsci.nvo;
using System.Xml;
using System.Net;
using System.IO;
using System.Web;

namespace TerraViewer
{
    public partial class VORegistryBrowser : Form
    {
        public VORegistryBrowser()
        {
            InitializeComponent();
            SetUiStrings();
        }

        void SetUiStrings()
        {
            searchUrlLabel.Text = Language.GetLocalizedText(603, "Base URL");
            fromRegistry.Text = Language.GetLocalizedText(604, "From Registry");
            keywordLabel.Text = Language.GetLocalizedText(605, "NVO Registry Title Like");
            raLabel.Text = Language.GetLocalizedText(310, "RA");
            label1.Text = Language.GetLocalizedText(311, "Dec");
            searchRadiusLabel.Text = Language.GetLocalizedText(606, "Search Radius");
            verbosityLabel.Text = Language.GetLocalizedText(607, "Verbosity");
            fromView.Text = Language.GetLocalizedText(608, "From View");
            search.Text = Language.GetLocalizedText(137, "Search");
            close.Text = Language.GetLocalizedText(212, "Close");

            findRegistry.Text = Language.GetLocalizedText(620, "NVO Registry Search");
            In.Text = Language.GetLocalizedText(620, "NVO Registry Search");
            
            // Is this robust? Peter
            this.ResourceList.TopItem.Text = Language.GetLocalizedText(619, "Enter a keyword above to search the US NVO registry for Cone Search services");

            this.Text = Language.GetLocalizedText(621, "VO Cone Search / Registry Browser");
        }

        static VoTable registry = null;
        static string lastKeyword="";
        static string lastConeSearch="";
        private void VORegistryBrowser_Load(object sender, EventArgs e)
        {
            // Changed to enable localization
          
            verbosity.Items.Add(Language.GetLocalizedText(624, "Low"));
            verbosity.Items.Add(Language.GetLocalizedText(625, "Medium"));
            verbosity.Items.Add(Language.GetLocalizedText(626, "High"));

            verbosity.SelectedIndex = Properties.Settings.Default.VOTableVerbosityDefault-1;
            double raVal = RenderEngine.Engine.RA * 15;
            ra.Text = raVal.ToString();
            dec.Text = RenderEngine.Engine.Dec.ToString();
            searchRadius.Text = RenderEngine.Engine.FovAngle.ToString();

            if (registry != null)
            {
                LoadRegistryResults();
                this.keyword.Text = lastKeyword;
                this.baseUrl.Text = lastConeSearch;
            }

            siapCheckbox.Checked = !coneSearch;
            coneSearchCheckbox.Checked = coneSearch;
    
        }

        private void VoRegistrySearch(string keyword)
        {
            try
            {
                lastKeyword = keyword;
                string filename = string.Format(@"{0}\NVOREG.XML", Path.GetTempPath());
                string url = String.Format("http://nvo.stsci.edu//vor10/NVORegInt.asmx/VOTCapabilityPredicate?predicate=(title%20like%20'%25{0}%25'%20or%20shortname%20like%20'%25{0}%25')&capability={1}", keyword, coneSearch ? "ConeSearch" : "SIAP");
        
                
                if (!FileDownload.DownloadFile(url, filename, true))
                {
                    return;
                }

                if (!File.Exists(filename))
                {
                    return;
                }

          
                string data = File.ReadAllText(filename);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);
                registry = new VoTable(doc);

                LoadRegistryResults();
            }
            catch
            {
                fromRegistry.Checked = false;
                fromRegistry.Enabled = false;
                ResourceList.Items.Add(new ListViewItem(Language.GetLocalizedText(915, "Can't access NVO Registry at this time")));
            }
        }

        private void LoadRegistryResults()
        {

            ResourceList.Columns.Clear();
            ResourceList.Items.Clear();
            ResourceList.Columns.Add(Language.GetLocalizedText(611, "Title"));
            ResourceList.Columns.Add(Language.GetLocalizedText(612, "ServiceType"));
            ResourceList.Columns.Add(Language.GetLocalizedText(613, "Type"));
            ResourceList.Columns.Add(Language.GetLocalizedText(614, "Publisher"));
            ResourceList.Columns.Add(Language.GetLocalizedText(615, "Wave Band"));
            ResourceList.Columns.Add(Language.GetLocalizedText(616, "Identifier"));
            ResourceList.Columns.Add(Language.GetLocalizedText(617, "Max Radius"));
            ResourceList.Columns.Add(Language.GetLocalizedText(618, "Max Rows"));
            // ResourceList.Columns.Add("Contact E-Mail");

            ResourceList.Columns[0].Width = 200;
            ResourceList.Columns[1].Width = 80;
            ResourceList.Columns[2].Width = 80;
            ResourceList.Columns[3].Width = 100;
            ResourceList.Columns[4].Width = 100;
            ResourceList.Columns[5].Width = 100;
            ResourceList.Columns[6].Width = 70;
            ResourceList.Columns[7].Width = 70;

            foreach (VoRow row in registry.Rows)
            {
                ListViewItem item = new ListViewItem(row["title"].ToString());
                item.SubItems.Add(row["type"].ToString());
                item.SubItems.Add("Cone Search");
                item.SubItems.Add(row["publisher"].ToString());
                item.SubItems.Add(row["waveband"].ToString());
                item.SubItems.Add(row["identifier"].ToString());
                item.SubItems.Add(row["maxSearchRadius"].ToString());
                item.SubItems.Add(row["maxRecords"].ToString());
                //item.SubItems.Add(res.ContactEmail);
                item.Tag = row;

                ResourceList.Items.Add(item);
            }
        }

         
        private void ResourceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fromRegistry.Checked)
            {
                if (ResourceList.SelectedIndices.Count > 0)
                {
                    VoRow selected = (VoRow)ResourceList.SelectedItems[0].Tag;
                    baseUrl.Text = selected["accessURL"].ToString().Replace("&amp;", "&");
                }
            }
        }
        private void fromRegistry_CheckedChanged(object sender, EventArgs e)
        {
            if (fromRegistry.Checked && ResourceList.SelectedIndices.Count > 0)
            {
                VoRow selected = (VoRow)ResourceList.SelectedItems[0].Tag;
                baseUrl.Text = selected["accessURL"].ToString().Replace("&amp;", "&");
            }
            baseUrl.ReadOnly = fromRegistry.Checked;
            baseUrl.Enabled = !fromRegistry.Checked;
        }
        public string URL = "";
        static string lastBaseUrl = "";
        private void search_Click(object sender, EventArgs e)
        {
            // The following line will not work with localized strings for low, medium, high
            // VerbosityTypes verb = (VerbosityTypes)Enum.Parse(typeof(VerbosityTypes), verbosity.SelectedItem.ToString());
            // Low = 1, Medium = 2, High = 3

            if (baseUrl.Text.Length > 0)
            {
                string adjustedBase = baseUrl.Text;

                if (!adjustedBase.EndsWith(@"&") && !adjustedBase.EndsWith(@"?"))
                {
                    if (adjustedBase.Contains(@"?"))
                    {
                        adjustedBase += "&";
                    }
                    else
                    {
                        adjustedBase += "?";
                    }
                }

                int verb = verbosity.SelectedIndex + 1;
                string tempURL = "";
                if (coneSearch)
                {
                    tempURL = string.Format("{0}RA={1}&DEC={2}&SR={3}&VERB={4}", adjustedBase, ra.Text, dec.Text, searchRadius.Text, verb);               
                }
                else
                {
                    tempURL = string.Format("{0}POS={1},{2}&SIZE={3}&VERB={4}", adjustedBase, ra.Text, dec.Text, searchRadius.Text, verb);
                }

                if (!Uri.IsWellFormedUriString(tempURL, UriKind.Absolute))
                {
                    UiTools.ShowMessageBox(Language.GetLocalizedText(914, "The URL does not appear to be well formed"));
                    return;
                }

                lastBaseUrl = baseUrl.Text;
                URL = tempURL;

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void close_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void fromView_CheckedChanged(object sender, EventArgs e)
        {
            if (fromView.Checked)
            {
                double raVal = RenderEngine.Engine.RA * 15;
                ra.Text = raVal.ToString();
                dec.Text = RenderEngine.Engine.Dec.ToString();
                searchRadius.Text = RenderEngine.Engine.FovAngle.ToString();

                ra.ReadOnly = true;
                dec.ReadOnly = true;
                searchRadius.ReadOnly = true;
            }
            else
            {
                ra.ReadOnly = false;
                dec.ReadOnly = false;
                searchRadius.ReadOnly = false;
            }
        }

        private void ResourceList_ItemActivate(object sender, EventArgs e)
        {

        }

        private void findRegistry_Click(object sender, EventArgs e)
        {
            VoRegistrySearch(keyword.Text);
        }

        private void keyword_TextChanged(object sender, EventArgs e)
        {
            findRegistry.Enabled = (keyword.Text.Length > 0);

        }

        private void VORegistryBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
             lastConeSearch = this.baseUrl.Text;

        }

        private void verbosity_SelectionChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.VOTableVerbosityDefault = this.verbosity.SelectedIndex+1;
        }

        private void baseUrl_TextChanged(object sender, EventArgs e)
        {
            if (baseUrl.Text.Length > 0)
            {
                int verb = verbosity.SelectedIndex + 1;

                string tempURL = string.Format("{0}RA={1}&DEC={2}&SR={3}&VERB={4}", baseUrl.Text, ra.Text, dec.Text, searchRadius.Text, verb);

                if (!Uri.IsWellFormedUriString(tempURL, UriKind.Absolute))
                {
                    search.Enabled = false;
                    return;
                }
                search.Enabled = true;
            }
            else
            {
                search.Enabled = false;
            }
        }
        static bool coneSearch = true;

        private void coneSearchCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            coneSearch = coneSearchCheckbox.Checked;
            if (siapCheckbox.Checked == coneSearchCheckbox.Checked)
            {
                siapCheckbox.Checked = !coneSearchCheckbox.Checked;
            }
            ResourceList.Columns.Clear();

        }

        private void siapCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            coneSearch = !siapCheckbox.Checked;
            if (coneSearchCheckbox.Checked == siapCheckbox.Checked)
            {
                coneSearchCheckbox.Checked = !siapCheckbox.Checked;
            }
            ResourceList.Columns.Clear();
        }

    }
    public enum VerbosityTypes { Low=1, Medium, High };
}