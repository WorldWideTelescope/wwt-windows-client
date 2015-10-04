using System;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class SimpleInput : Form
    {
        public SimpleInput(string title, string label, string defaultText, int limit )
        {
            this.title = title;
            this.label = label;
            ResultText = defaultText;
            this.limit = limit;
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            OK.Text = Language.GetLocalizedText(156, "OK");
            Cancel.Text = Language.GetLocalizedText(157, "Cancel");
        }

        readonly string title;
        readonly string label;
        public string ResultText;
        readonly int limit = 256;
        public int MinLength = 1;
        private void OK_Click(object sender, EventArgs e)
        {
            ResultText = inputText.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            ResultText = inputText.Text;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SimpleInput_Load(object sender, EventArgs e)
        {
            textLabel.Text = label;
            Text = title;
            inputText.Text = ResultText;
            inputText.MaxLength = limit;

        }

        private void inputText_TextChanged(object sender, EventArgs e)
        {
            if (inputText.Text.Length >= MinLength)
            {
                OK.Enabled = true;
            }
            else
            {
                OK.Enabled = false;
            }
        }

        private void SimpleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OK_Click(this, new EventArgs());
            }

            if (e.KeyCode == Keys.Escape)
            {
                Cancel_Click(this, new EventArgs());
            }
        }
    }
}