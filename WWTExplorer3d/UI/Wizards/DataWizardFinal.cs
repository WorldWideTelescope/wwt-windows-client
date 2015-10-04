using System;

namespace TerraViewer
{
    public partial class DataWizardFinal : PropPage
    {
        public DataWizardFinal()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            label1.Text = Language.GetLocalizedText(845, "Your data is now ready for viewing. If you need to change any options just right-click on the layer and select the Properties option on the context menu.");
        }
        TimeSeriesLayer layer;

        public override void SetData(object data)
        {

            layer = data as TimeSeriesLayer;
        }

        public override bool Save()
        {
            return true;
        }

        private void DataWizardFinal_Load(object sender, EventArgs e)
        {

        }
    }
}
