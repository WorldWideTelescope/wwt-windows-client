using System;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class RemoteAccessControl : Form
    {
        public RemoteAccessControl()
        {
            InitializeComponent();
            SetUiStrings();
        }
        public static string ipTarget = "0.0.0.0";
        private void SetUiStrings()
        {
            Cancel.Text = "Cancel";
            Ok.Text = "Ok";
            label2.Text = "Using the accept list you can manage what client host are allowed remote application control of WorldWide Telescope.";
            ClearAccept.Text = "Clear";
            DeleteAccept.Text = "Delete";
            AcceptLocal.Text = "Accept Local Connections";
            AcceptRemote.Text = "Accept All Remote Connections";
            label7.Text = "IP Address (Use * for wildcard)";
            Add.Text = "Add";
            Text = "Remote Access Control";

        }

        private void WebAccessList_Load(object sender, EventArgs e)
        {
            AcceptLocal.Checked = Properties.Settings.Default.AllowLocalHTTP;
            AcceptRemote.Checked = Properties.Settings.Default.AllowAllRemoteHTTP;

            //todo Load accept/Reject lists


            var parts = ipTarget.Split(new[] { '.' });

            if (parts.Length == 4)
            {
                ipPart1.Text = parts[0];
                ipPart2.Text = parts[1];
                ipPart3.Text = parts[2];
                ipPart4.Text = parts[3];
            }
            if (!string.IsNullOrEmpty(Properties.Settings.Default.HTTPWhiteList))
            {
                AcceptList.Items.AddRange(Properties.Settings.Default.HTTPWhiteList.Split(new[] { ';' }));
            }

        }

        private void Accept_Click(object sender, EventArgs e)
        {
            var ip = ipPart1.Text + "." + ipPart2.Text + "." + ipPart3.Text + "." + ipPart4.Text;

            if (!AcceptList.Items.Contains(ip))
            {
                AcceptList.Items.Add(ip);
            }
        }

        private void DeleteAccept_Click(object sender, EventArgs e)
        {
            if (AcceptList.SelectedIndex > -1)
            {
                AcceptList.Items.Remove(AcceptList.SelectedItem);
            }
        }

        private void ClearAccept_Click(object sender, EventArgs e)
        {
            if (UiTools.ShowMessageBox("This will clear the Accept List. Are you sure you want to continue?", "Application Access List", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                AcceptList.Items.Clear();
            }
        }

     

        private void Ok_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.AllowLocalHTTP = AcceptLocal.Checked;
            Properties.Settings.Default.AllowAllRemoteHTTP = AcceptRemote.Checked;
            DialogResult = DialogResult.OK;

            //Save the white list
            var sb = new StringBuilder();
            var first = true;
            foreach (var ip in AcceptList.Items)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    sb.Append(";");
                }
                sb.Append(ip);
            }
            Properties.Settings.Default.HTTPWhiteList = sb.ToString();


            MyWebServer.SetAccessLists();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }


        private void ipPart1_TextChanged(object sender, EventArgs e)
        {
            if (ipPart1.Text.Contains("."))
            {
                var parts = ipPart1.Text.Split(new[] { '.' });
                if (parts.Length == 4)
                {
                    ipPart1.Text = parts[0].Trim();
                    ipPart2.Text = parts[1].Trim();
                    ipPart3.Text = parts[2].Trim();
                    ipPart4.Text = parts[3].Trim();

                }
            }
            // Filter all but numbers and WildCard
            var text = "";
            foreach (var c in ipPart1.Text)
            {
                if (char.IsNumber(c) || c == '*')
                {
                    text += c;
                }
            }
            if (ipPart1.Text != text)
            {
                ipPart1.Text = text;
            }

            if (ipPart1.Text == "*")
            {
                ipPart2.Text = "*";
            }
        }
        private void ipPart2_TextChanged(object sender, EventArgs e)
        {
            if (ipPart2.Text == "*")
            {
                ipPart3.Text = "*";
            }
            // Filter all but numbers and WildCard
            var text = "";
            foreach (var c in ipPart2.Text)
            {
                if (char.IsNumber(c) || c == '*')
                {
                    text += c;
                }
            }
            if (ipPart2.Text != text)
            {
                ipPart2.Text = text;
            }
        }
        private void ipPart3_TextChanged(object sender, EventArgs e)
        {


            if (ipPart3.Text == "*")
            {
                ipPart4.Text = "*";
            }
            // Filter all but numbers and WildCard
            var text = "";
            foreach (var c in ipPart3.Text)
            {
                if (char.IsNumber(c) || c == '*')
                {
                    text += c;
                }
            }
            if (ipPart3.Text != text)
            {
                ipPart3.Text = text;
            }
        }

        private void ipPart4_TextChanged(object sender, EventArgs e)
        {  // Filter all but numbers and WildCard
            var text = "";
            foreach (var c in ipPart4.Text)
            {
                if (char.IsNumber(c) || c == '*')
                {
                    text += c;
                }
            }
            if (ipPart4.Text != text)
            {
                ipPart4.Text = text;
            }

        }
        private void ipPart1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Decimal || e.KeyCode == Keys.OemPeriod)
            {
                ipPart2.Focus();
            }
        }

        private void ipPart2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Decimal || e.KeyCode == Keys.OemPeriod)
            {
                ipPart3.Focus();
            }
        }

        private void ipPart3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Decimal || e.KeyCode == Keys.OemPeriod)
            {
                ipPart4.Focus();
            }
        }

        private void AcceptList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
