using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using TerraViewer.CloudCommunitiesAPI;

namespace TerraViewer
{
    public partial class CloudCommunityCreate : Form
    {
        public static long? createdCommunityId;
        public CloudCommunityCreate()
        {
            InitializeComponent();
        }
        private void setUIStrings()
        {
            btnCreate.Text = Language.GetLocalizedText(-1, "Ok");
            lblName.Text = Language.GetLocalizedText(-1, "Name:");
            lblImage.Text = Language.GetLocalizedText(-1, "Image:");
            btnCancel.Text = Language.GetLocalizedText(-1, "Cancel");
            lblContent.Text = Language.GetLocalizedText(-1, "Order by:");
            lblAccess.Text = Language.GetLocalizedText(-1, "Public Access:");
        }
        private void CloudCommunityCreate_Load(object sender, EventArgs e)
        {
            setUIStrings();
            createdCommunityId = null;
            cmbAccess.Items.Clear();
            foreach (PublicPermission p in Enum.GetValues(typeof(PublicPermission)))
            {
                cmbAccess.Items.Add(p);
                if (p == PublicPermission.PUBLIC_READ)
                {
                    cmbAccess.SelectedIndex = cmbAccess.Items.Count - 1;
                }
            }
            cmbOrder.Items.Clear();
            foreach (OrderContentMethods o in Enum.GetValues(typeof(OrderContentMethods)))
            {
                cmbOrder.Items.Add(o);
                if (o == OrderContentMethods.Arbitrary)
                {
                    cmbOrder.SelectedIndex = cmbOrder.Items.Count - 1;
                }
            }
        }
        private void btnBrowser_Click(object sender, EventArgs e)
        {
            fileDialog.FileName = txtImage.Text;
            fileDialog.ShowDialog();
            txtImage.Text = fileDialog.FileName;
        }
        private byte[] getImageByteArray()
        {
            string filename = txtImage.Text;
            Bitmap bmp = new Bitmap(filename);
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }
        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] imageArray = null;
                try
                {
                    imageArray = getImageByteArray();
                }
                catch (Exception)
                {
                    MessageBox.Show(Language.GetLocalizedText(-1, "Error with selected image"));
                    return;
                }
                CloudCommunitiesUsersAPI.apiUsers uApi = new CloudCommunitiesUsersAPI.apiUsers();
                string logoUrl = uApi.UploadFile(CloudCommunities.GetTokenFromId(), imageArray, txtImage.Text, "image/jpeg", false);

                apiCommunities api = new apiCommunities();
                long newCommunityId = api.Create(CloudCommunities.GetTokenFromId(), txtName.Text, logoUrl,
                    (OrderContentMethods)cmbOrder.SelectedItem, (PublicPermission)cmbAccess.SelectedItem, txtDescription.Text);
                createdCommunityId = newCommunityId;
                this.Close();
            }
            catch (SoapException ex)
            {
                CloudCommunities.ProcessSoapException(ex);
            }
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
