using System.Windows.Forms;

namespace TerraViewer
{
    public partial class VORegistryProperties : Form
    {
        public VORegistryProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }

        void SetUiStrings()
        {
            searchUrlLabel.Text = Language.GetLocalizedText(603, "Base URL");
        }
    }
}