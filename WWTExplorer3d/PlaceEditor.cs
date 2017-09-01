using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class PlaceEditor : Form
    {
        public PlaceEditor()
        {
            InitializeComponent();
        }
        IPlace editTarget = null;

        public IPlace EditTarget
        {
            get { return editTarget; }
            set { editTarget = value; }
        }

        private void PlaceEditor_Load(object sender, EventArgs e)
        {
            LoadTargetValues();
            // todo localize entire dialog box..
        }

        private void LoadTargetValues()
        {
            names.Text = UiTools.GetNamesStringFromArray(editTarget.Names);
            ra.Text = Coordinates.FormatHMS(editTarget.RA);
            dec.Text = Coordinates.FormatDMS(editTarget.Dec);
            mag.Text = editTarget.Magnitude.ToString();
            zoom.Text = Coordinates.FormatDMS(editTarget.ZoomLevel / 6);
            // todo localize and format
            DistanceValue.Text = editTarget.Distance.ToString();
            string fullName = "";

            if (Constellations.FullNames.ContainsKey(editTarget.Constellation))
            {
                fullName = Constellations.FullNames[editTarget.Constellation];
            }

            constellation.Items.Add("Undefined/Not Applicable");
            constellation.SelectedIndex = 0;
            foreach (string name in Constellations.FullNames.Values)
            {
                int index = constellation.Items.Add(name);
                if (name == fullName)
                {
                    constellation.SelectedIndex = index;
                }
            }
            SortedList<string, string> list = new SortedList<string, string>();

            foreach (string s in Enum.GetNames(typeof(Classification)))
            {
                list.Add(s, s);
            }

            foreach (string s in list.Values)
            {
                classification.Items.Add(s);
            }
            classification.SelectedIndex = classification.Items.IndexOf(editTarget.Classification.ToString());
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            try
            {
                editTarget.Names = names.Text.Split(new char[] { ';' }); ;
                editTarget.RA = Coordinates.Parse(ra.Text);
                editTarget.Dec = Coordinates.Parse(dec.Text);
                editTarget.Magnitude = Convert.ToDouble(mag.Text);
                editTarget.Distance = Convert.ToDouble(DistanceValue.Text);
                editTarget.ZoomLevel = Coordinates.Parse(zoom.Text)*6;
                editTarget.Classification = (Classification)Enum.Parse((typeof(Classification)), classification.SelectedItem.ToString());
                editTarget.ThumbNail = null;
                if (Constellations.Abbreviations.ContainsKey(constellation.SelectedItem.ToString()))
                {
                    editTarget.Constellation = Constellations.Abbreviations[constellation.SelectedItem.ToString()];
                }
                else
                {
                    editTarget.Constellation = "NA";
                }
                FolderBrowser.AllDirty = true;
                this.Close();
            }
            catch
            {
                MessageBox.Show("There are errors in the properties. Properties must be in the correct format.", "Object Properties Editor");
            }
        }

        private void fromView_Click(object sender, EventArgs e)
        {
            ra.Text = Coordinates.FormatHMS(RenderEngine.Engine.RA);
            dec.Text = Coordinates.FormatDMS(RenderEngine.Engine.Dec);
            mag.Text = editTarget.Magnitude.ToString();
            zoom.Text = Coordinates.FormatDMS(RenderEngine.Engine.ZoomFactor / 6);
        }
    }
}