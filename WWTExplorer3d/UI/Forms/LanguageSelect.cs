using System;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class LanguageSelect : Form
    {
        public Language Language;
        public LanguageSelect()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            Text = Language.GetLocalizedText(1, "Select Your Language");
            selectLangLabel.Text = Language.GetLocalizedText(1, "Select Your Language");

            Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            OK.Text = Language.GetLocalizedText(156, "OK");
        }

        private void LanguageSelect_Load(object sender, EventArgs e)
        {
            var items = Language.GetAvailableLanguages();
            Language selected = null;
            foreach (var item in items)
            {
                langCombo.Items.Add(item);
                if (item.Url == Properties.Settings.Default.LanguageUrl || (item.Code == "ZZZZ" && Properties.Settings.Default.LanguageCode=="ZZZZ") )
                {
                    selected = item;
                }
            }
            langCombo.SelectedItem = selected;
        }

        private void OK_Click(object sender, EventArgs e)
        {

            Language = (Language)langCombo.SelectedItem;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}