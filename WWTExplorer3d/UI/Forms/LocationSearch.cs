using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.Xml;

namespace TerraViewer
{
    public partial class LocationSearch : Form
    {
        public LocationSearch ()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            txtName.Text = Language.GetLocalizedText(255, "<Type name here>");
            label1.Text = Language.GetLocalizedText(1145, "Place Name or Address");
            Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            OK.Text = Language.GetLocalizedText(137, "Search");
            Text = Language.GetLocalizedText(1146, "Location Search - Powered by Bing");
        }
        string objectName = "";

        public string ObejctName
        {
            get { return objectName; }
            set { objectName = value; }
        }

        bool searchMode = true;

        private void OK_Click(object sender, EventArgs e)
        {
            if (searchMode)
            {
                RunSearch();
            }
            else
            {
                if (resultsListbox.SelectedIndex > -1)
                {
                    Result = (IPlace)resultsListbox.SelectedItem;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        private void RunSearch()
        {
            objectName = txtName.Text;

            var client = new WebClient();

            var url = string.Format("http://dev.virtualearth.net/REST/v1/Locations?o=xml&q={0}&key=AnEbWVZBhPHNm4_fSF7sklQZYODmK0o4_kkm6AId7V0RXtPnmAXfIJn3EUJbMbmm", HttpUtility.UrlEncode(txtName.Text));
            var data = client.DownloadString(url);
            resultsListbox.Items.Clear();
            var count = ParseResults(data);

            if (count == 0)
            {
                resultsListbox.Items.Add(Language.GetLocalizedText(1147, "There was nothing found with the given text."));
                resultsListbox.Items.Add(Language.GetLocalizedText(1148, "Change your search text and try again."));
                resultsListbox.Visible = true;
                resultsListLabel.Visible = true;
                Height = 356;
                searchMode = true;
                OK.Text = Language.GetLocalizedText(137, "Search");

            }
            else if (count == 1)
            {
                Result = resultslist[0];
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                foreach (var place in resultslist)
                {
                    resultsListbox.Items.Add(place);
                    resultsListbox.Visible = true;
                    resultsListLabel.Visible = true;
                    Height = 356;
                }
                resultsListbox.SelectedIndex = 0;
                searchMode = false;
                OK.Text = Language.GetLocalizedText(156, "OK");
                searchMode = false;
            }
        }

        private void LocationSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OK_Click(this, e);
            }
        }

        private void LocationSearch_Load(object sender, EventArgs e)
        {
            resultsListLabel.Visible = false;
            resultsListbox.Visible = false;
            Height = 132;

            if (!string.IsNullOrEmpty(ObejctName))
            {
                txtName.Text = ObejctName;
                RunSearch();
            }
           
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OK_Click(this, e);
            }
        }



        public IPlace Result = null;


        readonly List<IPlace> resultslist = new List<IPlace>();

        private int ParseResults(string data)
        {
            resultslist.Clear();

            var doc = new XmlDocument();
            var index = 0;
            try
            {
                data = data.Substring(data.IndexOf("<?xml"));

                doc.LoadXml(data);

                XmlNode resources = doc["Response"]["ResourceSets"]["ResourceSet"]["Resources"];

                if (resources != null)
                {
                    foreach (XmlNode node in resources.ChildNodes)
                    {
                        if (node.Name == "Location")
                        {
                            index++;
                            var name = node["Name"].InnerText;
                            var lat = double.Parse(node["Point"]["Latitude"].InnerText);
                            var lng = double.Parse(node["Point"]["Longitude"].InnerText);
                            var type = node["EntityType"].InnerText;

                            var place = new TourPlace(name, lat, lng,  Classification.Unidentified, "", ImageSetType.Earth, -1);

                            resultslist.Add(place);
                        }
                    }
                }

            }
            catch
            {
            }
            return index;
        }

        private void resultsListbox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            searchMode = true;
            OK.Text = Language.GetLocalizedText(137, "Search");
        }

        private void resultsListbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (resultsListbox.SelectedIndex > -1)
            {
                Result = (IPlace)resultsListbox.SelectedItem;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

    }

  

}