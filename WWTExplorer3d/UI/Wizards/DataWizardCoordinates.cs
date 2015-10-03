using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DataWizardCoordinates : PropPage
    {
        public DataWizardCoordinates()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.longRALable.Text = Language.GetLocalizedText(840, "Longitude");
            this.latDecLabel.Text = Language.GetLocalizedText(839, "Latitude");
            this.AltitudeDepthLabel.Text = Language.GetLocalizedText(838, "Altitude / Depth / Distance");
            this.label1.Text = Language.GetLocalizedText(837, "Spatial Coordinates");
            this.altDepthLabel.Text = Language.GetLocalizedText(941, "Altitude / Depth Column");
            this.altDepthTypeLabel.Text = Language.GetLocalizedText(613, "Type");
            this.label2.Text = Language.GetLocalizedText(817, "Units");
            this.raUnitsLabel.Text = Language.GetLocalizedText(817, "Units");
            this.label3.Text = Language.GetLocalizedText(836, "Use the drop-downs to select the columns that specify the coordinates, select the altitude/depth mapping type, and optionally the column that specifies altitude/depth and the units in which the data is specified, Positive depth values offset downward. Positive altitude values offset upward.");
        }
        TimeSeriesLayer layer;

        public override void SetData(object data)
        {
            
            layer = data as TimeSeriesLayer;
        }

        public override bool Save()
        {
            layer.LatColumn = latColumnCombo.SelectedIndex - 1;
            layer.LngColumn = lngColumnCombo.SelectedIndex - 1;
            layer.AltColumn = altColumnCombo.SelectedIndex - 1;
            layer.AltUnit =  (AltUnits) AltDepthUnitsCombo.SelectedIndex;
            layer.AltType =  (TimeSeriesLayer.AltTypes) AltitudeDepthTypeCombo.SelectedIndex;
            layer.RaUnits = (TimeSeriesLayer.RAUnits)raUnits.SelectedIndex;
            return true;

        }

        private void DataWizardCoordinates_Load(object sender, EventArgs e)
        {
            latColumnCombo.Items.Add(Language.GetLocalizedText(832, "None"));
            latColumnCombo.Items.AddRange(layer.Header);
            latColumnCombo.SelectedIndex = layer.LatColumn + 1;
            lngColumnCombo.Items.Add(Language.GetLocalizedText(832, "None"));
            lngColumnCombo.Items.AddRange(layer.Header);
            lngColumnCombo.SelectedIndex = layer.LngColumn + 1;

            altColumnCombo.Items.Add(Language.GetLocalizedText(832, "None"));
            altColumnCombo.Items.AddRange(layer.Header);
            altColumnCombo.SelectedIndex = layer.AltColumn + 1;

            AltitudeDepthTypeCombo.Items.AddRange(Enum.GetNames(typeof(TimeSeriesLayer.AltTypes)));
            AltitudeDepthTypeCombo.SelectedIndex = (int)layer.AltType;

            raUnits.Items.AddRange(Enum.GetNames(typeof(TimeSeriesLayer.RAUnits)));
            raUnits.SelectedIndex = (int)layer.RaUnits;

            AltDepthUnitsCombo.Items.AddRange(Enum.GetNames(typeof(AltUnits)));
            AltDepthUnitsCombo.SelectedIndex = (int)layer.AltUnit;

            if (layer.Astronomical)
            {
                latDecLabel.Text = Language.GetLocalizedText(635, "Dec Source");
                longRALable.Text = Language.GetLocalizedText(634, "RA Source");
                raUnits.Visible = true;
                raUnitsLabel.Visible = true;
            }
            else
            {
                latDecLabel.Text = Language.GetLocalizedText(367, "Lat :  ");
                longRALable.Text = Language.GetLocalizedText(365, "Lng:   ");
                raUnits.Visible = false;
                raUnitsLabel.Visible = false;

            }
        }


        private void AltitudeDepthTypeCombo_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void latColumnCombo_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void raUnits_SelectionChanged(object sender, EventArgs e)
        {

        }
    }
}
