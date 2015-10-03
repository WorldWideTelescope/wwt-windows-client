using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class SelectLink : Form
    {
        public SelectLink()
        {
            InitializeComponent();
            SetUiStrings();
            tourStopList.ItemClicked += new TourStopClickedEventHandler(tourStopList_ItemClicked);
            tourStopList.ShowAddButton = false;
        }

        void SetUiStrings()
        {
            LinkToSlideCheckbox.Text = Language.GetLocalizedText(599, "Link to Slide (Select below)");
            LinkToNextCheckbox.Text = Language.GetLocalizedText(600, "Link to Next Slide");
            this.Text = Language.GetLocalizedText(601, "Select Link Navigation");
            ReturnToCallerCheckbox.Text = Language.GetLocalizedText(602, "Return to Caller");
            cancel.Text = Language.GetLocalizedText(157, "Cancel");
            Ok.Text = Language.GetLocalizedText(156, "OK");
        }

        void tourStopList_ItemClicked(object sender, TourStop e)
        {
            id = e.Id;
            LinkToSlideCheckbox.Checked = true;
            LinkToNextCheckbox.Checked = false;
            ReturnToCallerCheckbox.Checked = false;
        }
        TourDocument tour;

        public TourDocument Tour
        {
            get { return tour; }
            set
            {
                tour = value;
                tourStopList.Tour = value;
                SetOptions();
                tourStopList.Refresh();
            }
        }

        string id;

        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                SetOptions();
            }
        }

        private void SelectLink_Load(object sender, EventArgs e)
        {
            SetOptions();
        }

        private void SetOptions()
        {
            if (tour != null)
            {
                switch (id)
                {
                    case "":
                        LinkToSlideCheckbox.Checked = false;
                        LinkToNextCheckbox.Checked = false;
                        ReturnToCallerCheckbox.Checked = false;
                        break;
                    case "Next":
                        id = "Next";
                        LinkToNextCheckbox.Checked = true;
                        break;
                    case "Return":
                        ReturnToCallerCheckbox.Checked = true;
                        break;
                    default:
                        LinkToSlideCheckbox.Checked = true;
                        tourStopList.SelectedItem = Tour.GetTourStopIndexByID(id);
                        tourStopList.EnsureSelectedVisible();
                        break;
                }
            }
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            if ( LinkToSlideCheckbox.Checked == false &&
                        LinkToNextCheckbox.Checked == false &&
                        ReturnToCallerCheckbox.Checked == false)
            {
                id = "";
            }
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void LinkToNextCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (LinkToNextCheckbox.Checked)
            {
                id = "Next";
                LinkToSlideCheckbox.Checked = false;
                ReturnToCallerCheckbox.Checked = false;
            }
        }

        private void LinkToSlideCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (LinkToSlideCheckbox.Checked)
            {
                if (tourStopList.SelectedItem != -1)
                {
                    id = tour.TourStops[tourStopList.SelectedItem].Id;
                }
                LinkToNextCheckbox.Checked = false;
                ReturnToCallerCheckbox.Checked = false;
            }
        }



        private void ReturnToCallerCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (ReturnToCallerCheckbox.Checked)
            {
                id = "Return";
                LinkToSlideCheckbox.Checked = false;
                LinkToNextCheckbox.Checked = false;
            }
        }
    }
}
