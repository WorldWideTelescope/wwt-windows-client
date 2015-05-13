using System;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class CloudUploader : Form
    {
        private string filename;
        public string resultUrl;
        private string contentType;
        void uploader_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            resultUrl = Encoding.UTF8.GetString(e.Result).Trim();
            this.Close();
        }
        void uploader_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            progress.Value = e.ProgressPercentage;
        }
        public CloudUploader(string _filename, string _contentType)
        {
            resultUrl = null;
            filename = _filename;
            contentType = _contentType;
            InitializeComponent();
        }
        private void CloudUploader_Load(object sender, EventArgs e)
        {
            progress.Value = 0;
            bool isImage = contentType.IndexOf("image") == 0;
            WebClient uploader = new WebClient();
            string param = "ContentType=" + Uri.EscapeUriString(contentType);
            param += "&isAttachment=" + Uri.EscapeUriString((!isImage).ToString());
            param += "&LiveToken=" + CloudCommunities.GetTokenFromId(true);
            uploader.UploadProgressChanged += new UploadProgressChangedEventHandler(uploader_UploadProgressChanged);
            uploader.UploadFileCompleted += new UploadFileCompletedEventHandler(uploader_UploadFileCompleted);
            uploader.UploadFileAsync(new Uri(Properties.Settings.Default.WWTCommunityServer + "FileUploader.aspx?" + param), 
                filename);
        }
    }
}
