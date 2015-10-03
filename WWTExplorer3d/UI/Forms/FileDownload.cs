using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace TerraViewer
{
    public partial class FileDownload : Form
    {
        public FileDownload()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.label1.Text = Language.GetLocalizedText(216, "Please wait while the file is being downloaded.");
            this.Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            this.Text = Language.GetLocalizedText(217, "File Download Progress");
        }


        
        static readonly WebClient client = new WebClient();
        static bool complete;
        static bool canceled;
        static FileDownload dialog;
        static FileDownload()
        {
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
        }

        static void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            complete = true;
            if (dialog != null)
            {
                if (dialog.DownloadComplete != null)
                {
                    dialog.DownloadComplete.Invoke(sender, e);
                }
                dialog = null;
            }
        }

        public static bool DownloadFile(string url, string filename, bool forceDownload)
        {

            if (File.Exists(filename))
            {
                if (!forceDownload)
                {
                    return true;
                }
                File.Delete(filename);
            }
            var uri = new Uri(url);
            if (uri.IsFile)
            {
                var source = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
                File.Copy(source, filename);
                return true;
            }
            complete = false;
            canceled = false;
            client.DownloadFileAsync( uri, filename);

            System.Threading.Thread.Sleep(250);
            if (!complete)
            {

                dialog = new FileDownload();
                dialog.ShowDialog();
            }
            if (canceled)
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
            }


            return !canceled;
        }

        static void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (dialog != null)
            {
                if (dialog.ProgressChanged != null)
                {
                    dialog.ProgressChanged.Invoke(sender, e);
                }
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            // try
            {
                client.CancelAsync();
            }
            // catch
            {
            }
            canceled = true;
            //complete = true;
            //this.Close();
        }
        public event DownloadProgressChangedEventHandler ProgressChanged;
        public event AsyncCompletedEventHandler DownloadComplete;

        private void FileDownload_Load(object sender, EventArgs e)
        {
            ProgressChanged += new DownloadProgressChangedEventHandler(FileDownload_ProgressChanged);
            DownloadComplete += new AsyncCompletedEventHandler(FileDownload_DownloadComplete);
        }

        void FileDownload_DownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(DownloadComplete, new object[] { sender, e });
            }
            else
            {
                this.Close();
            }
        }

        void FileDownload_ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(ProgressChanged, new object[] { sender, e });
            }
            else
            {
                progressBar.Value = e.ProgressPercentage;
            }
        }

        void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
        }
    }
}