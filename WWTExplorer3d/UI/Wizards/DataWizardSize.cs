using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DataWizardSize : PropPage
    {
        public DataWizardSize()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.scaleFactorSlider.Text = Language.GetLocalizedText(854, "Scale Factor");
            this.scaleTypeLabel.Text = Language.GetLocalizedText(855, "Scale Type");
            this.label4.Text = Language.GetLocalizedText(856, "Size/Magnitude Column");
            this.label2.Text = Language.GetLocalizedText(857, "Scale Relative");
            this.label1.Text = Language.GetLocalizedText(853, "Use the scale type to specify how you want the markers displayed. You can have markers display relative to the screen, invariant of your zoom, or have them scale relative to the coordinates system. You can optionally select a column to control the scale.  Use the scale factor to adjust the relative unit values of the  column to the output marker size.");
        }
        TimeSeriesLayer layer;

        public override void SetData(object data)
        {

            layer = data as TimeSeriesLayer;
        }

        public override bool Save()
        {
            layer.SizeColumn = sizeColumnCombo.SelectedIndex - 1;
            layer.ScaleFactor = (float)Math.Pow(2, (scaleFactorTrackbar.Value - 50) / 4);
            layer.PointScaleType = (PointScaleTypes)scaleTypeCombo.SelectedIndex;
            layer.MarkerScale = (TimeSeriesLayer.MarkerScales)scaleRelative.SelectedIndex;
            return true;
        }

        private void DataWizardSize_Load(object sender, EventArgs e)
        {
            scaleRelative.Items.AddRange(Enum.GetNames(typeof(TimeSeriesLayer.MarkerScales)));
            scaleRelative.SelectedIndex = (int)layer.MarkerScale;

            sizeColumnCombo.Items.Add(Language.GetLocalizedText(832, "None"));
            sizeColumnCombo.Items.AddRange(layer.Header);
            sizeColumnCombo.SelectedIndex = layer.SizeColumn + 1;
            scaleTypeCombo.Items.AddRange(Enum.GetNames(typeof(PointScaleTypes)));
            scaleTypeCombo.SelectedIndex = (int)layer.PointScaleType;
            scaleFactorTrackbar.Value = (int)((Math.Log(layer.ScaleFactor, 2)) * 4 + 50.5);
            scaleText.Text = layer.ScaleFactor.ToString();
        }

        private void scaleTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            layer.PointScaleType = (PointScaleTypes)scaleTypeCombo.SelectedIndex;
            layer.CleanUp();
        }

        private void scaleFactorTrackbar_ValueChanged(object sender, EventArgs e)
        {
            layer.ScaleFactor = (float)Math.Pow(2, (scaleFactorTrackbar.Value - 50) / 4);
            scaleText.Text = layer.ScaleFactor.ToString();
        }

        private void sizeColumnCombo_SelectionChanged(object sender, EventArgs e)
        {
            layer.MarkerScale = (TimeSeriesLayer.MarkerScales)scaleRelative.SelectedIndex;

        }
    }
}
