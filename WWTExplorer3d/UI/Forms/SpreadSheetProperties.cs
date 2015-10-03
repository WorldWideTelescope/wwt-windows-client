using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class SpreadSheetProperties : Form
    {
        SpreadSheetLayer target;

        internal SpreadSheetLayer Target
        {
            get { return target; }
            set { target = value; }
        }


        public SpreadSheetProperties()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            target.NameColumn = nameColumnCombo.SelectedIndex - 1;
            target.LatColumn = latColumnCombo.SelectedIndex - 1;
            target.LngColumn = lngColumnCombo.SelectedIndex - 1;
            target.SizeColumn = sizeColumnCombo.SelectedIndex - 1;
            target.StartDateColumn = dateColumnCombo.SelectedIndex - 1;
            target.Astronomical = astronomicalCheckbox.Checked;
            target.Name = spreadsheetNameEdit.Text;
            target.ScaleFactor = (float)Math.Pow(2, (scaleFactorTrackbar.Value - 50) / 4);
            target.Decay = (float)Math.Pow(2, (decayTrackbar.Value - 50) / 4);
            target.PointScaleType = (PointScaleTypes)scaleTypeCombo.SelectedIndex;


        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void SpreadSheetProperties_Load(object sender, EventArgs e)
        {
            spreadsheetNameEdit.Text = target.Name;

            nameColumnCombo.Items.Add("None");
            nameColumnCombo.Items.AddRange(target.Header);
            nameColumnCombo.SelectedIndex = target.NameColumn + 1;
            latColumnCombo.Items.Add("None");
            latColumnCombo.Items.AddRange(target.Header);
            latColumnCombo.SelectedIndex = target.LatColumn + 1;
            lngColumnCombo.Items.Add("None");
            lngColumnCombo.Items.AddRange(target.Header);
            lngColumnCombo.SelectedIndex = target.LngColumn + 1;
            dateColumnCombo.Items.Add("None");
            dateColumnCombo.Items.AddRange(target.Header);
            dateColumnCombo.SelectedIndex = target.StartDateColumn + 1;
            astronomicalCheckbox.Checked = false;
            astronomicalCheckbox.Checked = target.Astronomical;
            sizeColumnCombo.Items.Add("None");
            sizeColumnCombo.Items.AddRange(target.Header);
            sizeColumnCombo.SelectedIndex = target.SizeColumn + 1;
            scaleTypeCombo.Items.AddRange(Enum.GetNames(typeof(PointScaleTypes)));
            scaleTypeCombo.SelectedIndex = (int)target.PointScaleType;
            scaleFactorTrackbar.Value = (int)((Math.Log(target.ScaleFactor, 2)) * 4 + 50.5);
            scaleText.Text = target.ScaleFactor.ToString();
           
            decayTrackbar.Value = (int)((Math.Log(target.Decay, 2)) * 4 + 50.5);


        }

        private void astronomicalCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (astronomicalCheckbox.Checked)
            {
                latDecLabel.Text = Language.GetLocalizedText(635, "Dec Source");
                longRALable.Text = Language.GetLocalizedText(634, "RA Source");
            }
            else
            {
                latDecLabel.Text = Language.GetLocalizedText(367, "Lat :  ");
                longRALable.Text = Language.GetLocalizedText(365, "Lng:   ");

            }
        }

        private void dateColumnCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (dateColumnCombo.SelectedIndex > 0)
            {
                target.ComputeDateDomainRange(dateColumnCombo.SelectedIndex-1, -1);
                beginDateRangeEdit.Text = target.BeginRange.ToString("yyyy/MM/dd   HH:mm:ss");
                endDateRangeEdit.Text = target.EndRange.ToString("yyyy/MM/dd   HH:mm:ss");
            }
        }

        private void scaleFactorTrackbar_ValueChanged(object sender, EventArgs e)
        {
            target.ScaleFactor = (float)Math.Pow(2, (scaleFactorTrackbar.Value-50)/4);
            scaleText.Text = target.ScaleFactor.ToString();
        }

        private void decayTrackbar_ValueChanged(object sender, EventArgs e)
        {
            target.Decay = (float)Math.Pow(2, (decayTrackbar.Value - 50) / 4);
            decayDescription.Text = target.Decay.ToString();

        }

        private void scaleTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            target.PointScaleType = (PointScaleTypes)scaleTypeCombo.SelectedIndex;
            target.CleanUp();
        }


    }
}
