using System;

namespace TerraViewer
{
    public partial class DataWizardOrbits : PropPage
    {
        public DataWizardOrbits()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
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

        private void DataWizardOrbits_Load(object sender, EventArgs e)
        {

        }
    }
}
