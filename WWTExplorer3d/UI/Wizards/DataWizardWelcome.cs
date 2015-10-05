using System;

namespace TerraViewer
{
    public partial class DataWizardWelcome : PropPage
    {
        public DataWizardWelcome()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            spreadsheetNameLabel.Text = Language.GetLocalizedText(939, "Layer Name");
            label3.Text = Language.GetLocalizedText(940, "CoordinatesType");
            label2.Text = Language.GetLocalizedText(858, "This wizard will guide you through the process of importing your data for visualization. On this first step enter a friendly name for your layer, this is how it will appear in the Layer Manager. Select a reference frame for the coordinates in your data. This determines how WWT will plot your data and where.");
            dataUrlLabel.Text = Language.GetLocalizedText(1009, "Data Source Url");
            autoUpdateCheckbox.Text = Language.GetLocalizedText(1028, "Auto Update");
        }
        TimeSeriesLayer layer;
        public override void SetData(object data)
        {
            layer = data as TimeSeriesLayer;
        }

        public override bool Save()
        {
            if (spreadsheetNameEdit.Text.Length > 0)
            {
                layer.Name = spreadsheetNameEdit.Text;


                if (layer.DynamicData && downloadUrl.Text.Length > 0)
                {
                    layer.AutoUpdate = autoUpdateCheckbox.Checked;
                    layer.DataSourceUrl = downloadUrl.Text;
                    if (!layer.DynamicUpdate())
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;

        }

        private void DataWizardWelcome_Load(object sender, EventArgs e)
        {
            spreadsheetNameEdit.Text = layer.Name;
            coordinatesType.Items.AddRange(Enum.GetNames(typeof(TimeSeriesLayer.CoordinatesTypes)));
            coordinatesType.Items.RemoveAt(2);
            coordinatesType.SelectedIndex = (int)layer.CoordinatesType;
            

            dataUrlLabel.Visible = downloadUrl.Visible = autoUpdateCheckbox.Visible = layer.DynamicData;
               
            downloadUrl.Text = layer.DataSourceUrl;
            autoUpdateCheckbox.Checked = layer.AutoUpdate;

            CheckReadyStatus();
        }

        private void coordinatesType_SelectionChanged(object sender, EventArgs e)
        {
            layer.CoordinatesType = (TimeSeriesLayer.CoordinatesTypes)coordinatesType.SelectedIndex;

            if (layer.CoordinatesType == TimeSeriesLayer.CoordinatesTypes.Rectangular)
            {
                layer.ShowFarSide = true;
            }
            else
            {
                layer.ShowFarSide = false;

            }
        }

        private void spreadsheetNameEdit_TextChanged(object sender, EventArgs e)
        {
            CheckReadyStatus();
        }

        private void downloadUrl_TextChanged(object sender, EventArgs e)
        {
            CheckReadyStatus();
        }

        private void CheckReadyStatus()
        {
            Binding.SendReadyStatus(this, (!layer.DynamicData || !string.IsNullOrEmpty(downloadUrl.Text.Trim())) && (!string.IsNullOrEmpty(spreadsheetNameEdit.Text)));
        }


    }
}
