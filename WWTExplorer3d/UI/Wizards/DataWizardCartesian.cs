using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DataWizardCartesian : PropPage
    {
        public DataWizardCartesian()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.label3.Text = Language.GetLocalizedText(825, "Select the Columns for cartesian coordinates for X, Y and optionally Z. The reverse checkbox may be used to match the sign vector for each axis. ");
            this.zAxisLabel.Text = Language.GetLocalizedText(826, "Z Axis Column (optional)");
            this.yAxisLabel.Text = Language.GetLocalizedText(827, "Y Axis Column");
            this.xAxisLabel.Text = Language.GetLocalizedText(828, "X Axis Column");
            this.reverseXCheckbox.Text = Language.GetLocalizedText(829, "Reverse X");
            this.reverseYCheckbox.Text = Language.GetLocalizedText(830, "Reverse Y");
            this.reverseZCheckbox.Text = Language.GetLocalizedText(831, "Reverse Z");
            this.label2.Text = Language.GetLocalizedText(817, "Units");
        }
        TimeSeriesLayer layer;

        public override void SetData(object data)
        {

            layer = data as TimeSeriesLayer;
        }

        public override bool Save()
        {
            layer.XAxisColumn = xAxisColumn.SelectedIndex - 1;
            layer.YAxisColumn = yAxisColumn.SelectedIndex - 1;
            layer.ZAxisColumn = zAxisColumn.SelectedIndex - 1;
            layer.XAxisReverse = reverseXCheckbox.Checked;
            layer.YAxisReverse = reverseYCheckbox.Checked;
            layer.ZAxisReverse = reverseZCheckbox.Checked;
            layer.CartesianScale = (AltUnits)AltDepthUnitsCombo.SelectedIndex;
            return true;

        }



        private void reverseXCheckbox_CheckedChanged(object sender, EventArgs e)
        {
           

        }

        private void reverseYCheckbox_CheckedChanged(object sender, EventArgs e)
        {
           

        }

        private void reverseZCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void xAxisColumn_SelectionChanged(object sender, EventArgs e)
        {
            

        }

        private void yAxisColumn_SelectionChanged(object sender, EventArgs e)
        {
           

        }

        private void zAxisColumn_SelectionChanged(object sender, EventArgs e)
        {
           
        }

        private void DataWizardCartesian_Load(object sender, EventArgs e)
        {
            xAxisColumn.Items.Add(Language.GetLocalizedText(832, "None"));
            xAxisColumn.Items.AddRange(layer.Header);
            xAxisColumn.SelectedIndex = layer.XAxisColumn + 1;

            yAxisColumn.Items.Add(Language.GetLocalizedText(832, "None"));
            yAxisColumn.Items.AddRange(layer.Header);
            yAxisColumn.SelectedIndex = layer.YAxisColumn + 1;

            zAxisColumn.Items.Add(Language.GetLocalizedText(832, "None"));
            zAxisColumn.Items.AddRange(layer.Header);
            zAxisColumn.SelectedIndex = layer.ZAxisColumn + 1;

            AltDepthUnitsCombo.Items.AddRange(Enum.GetNames(typeof(AltUnits)));
            AltDepthUnitsCombo.SelectedIndex = (int)layer.CartesianScale;

            reverseXCheckbox.Checked = layer.XAxisReverse;
            reverseYCheckbox.Checked = layer.YAxisReverse;
            reverseZCheckbox.Checked = layer.ZAxisReverse;

        }
    }


}
