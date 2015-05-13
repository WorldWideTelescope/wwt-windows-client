using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DataWizardDateTime : PropPage
    {
        public DataWizardDateTime()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.decaySliderLabel.Text = Language.GetLocalizedText(844, "Time Decay");
            this.EndDateRangeLabel.Text = Language.GetLocalizedText(819, "End Date Range");
            this.beginDateRangeLabel.Text = Language.GetLocalizedText(820, "Begin Date Range");
            this.label5.Text = Language.GetLocalizedText(843, "Start Date/Time");
            this.label1.Text = Language.GetLocalizedText(842, "End Date/Time");
            this.label3.Text = Language.GetLocalizedText(841, "The Start and End Dates used with a Time decay allow you to visualize time series data as animation using the Time controls. The time decay controls how long it takes to fade out an event after it triggers. Select the optional start and end data columns to enable this feature and get a preview of the date range for the column.");
        }
        TimeSeriesLayer layer = null;

        public override void SetData(object data)
        {

            layer = data as TimeSeriesLayer;
        }

        public override bool Save()
        {
            layer.StartDateColumn = dateColumnCombo.SelectedIndex - 1;
            layer.EndDateColumn = endDateTimeColumnCombo.SelectedIndex - 1;
            layer.Decay = (float)Math.Pow(2, (decayTrackbar.Value - 50) / 4);
            return true;

        }

        private void DataWizardDateTime_Load(object sender, EventArgs e)
        {
            endDateTimeColumnCombo.Items.Add(Language.GetLocalizedText(832, "None"));
            endDateTimeColumnCombo.Items.AddRange(layer.Header);
            endDateTimeColumnCombo.SelectedIndex = layer.EndDateColumn + 1;

            decayTrackbar.Value = (int)((Math.Log(layer.Decay, 2)) * 4 + 50.5);
            dateColumnCombo.Items.Add(Language.GetLocalizedText(832, "None"));
            dateColumnCombo.Items.AddRange(layer.Header);
            dateColumnCombo.SelectedIndex = layer.StartDateColumn + 1;



        }

        private void dateColumnCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (dateColumnCombo.SelectedIndex > 0)
            {
                layer.ComputeDateDomainRange(dateColumnCombo.SelectedIndex - 1, endDateTimeColumnCombo.SelectedIndex - 1);
                beginDateRangeEdit.Text = layer.BeginRange.ToString("yyyy/MM/dd   HH:mm:ss");
                endDateRangeEdit.Text = layer.EndRange.ToString("yyyy/MM/dd   HH:mm:ss");
            }
        }

        private void decayTrackbar_ValueChanged(object sender, EventArgs e)
        {
            layer.Decay = (float)Math.Pow(2, (decayTrackbar.Value - 50) / 4);
            decayDescription.Text = layer.Decay.ToString();

        }

        private void endDateTimeColumnCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (dateColumnCombo.SelectedIndex > 0)
            {
                layer.ComputeDateDomainRange(dateColumnCombo.SelectedIndex - 1, endDateTimeColumnCombo.SelectedIndex - 1);
                beginDateRangeEdit.Text = layer.BeginRange.ToString("yyyy/MM/dd   HH:mm:ss");
                endDateRangeEdit.Text = layer.EndRange.ToString("yyyy/MM/dd   HH:mm:ss");
            }
        }

    }
}
