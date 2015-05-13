using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace TerraViewer
{
    public partial class CacheTilePyramid : Form
    {
        public CacheTilePyramid()
        {
            InitializeComponent();
        }

        public IImageSet imageSet = null;

        private void CacheTilePyramid_Load(object sender, EventArgs e)
        {
            maxLevels.Text = imageSet.Levels.ToString();
            levels.Text = (Math.Min(imageSet.Levels, 6)).ToString();

        }

        static string statusText = "";
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            WebClient client = new WebClient();
            IImageSet layer = imageSet;
            fileCount = 0;
            maxFileCount = 0;
            for (int level = layer.BaseLevel; level < (levelCount + 1); level++)
            {
                int maxX = Earth3d.GetTilesXForLevel(layer, level);
                int maxY = Earth3d.GetTilesYForLevel(layer, level);

                for (int x = 0; x < maxX; x++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        maxFileCount++;
                        
                    }
                }
            }


            for (int level = layer.BaseLevel; level < (levelCount+1); level++)
            {
                int maxX = Earth3d.GetTilesXForLevel(layer, level);
                int maxY = Earth3d.GetTilesYForLevel(layer, level);

                for (int x = 0; x < maxX; x++)
                {
                    for (int y = 0; y < maxY; y++)
                    {
                        fileCount++;

                        statusText = string.Format(Language.GetLocalizedText(1045, "Downloading ({0} of {1})"), fileCount, maxFileCount);
                        backgroundWorker.ReportProgress((int)(100 * fileCount / maxFileCount));
                        if (backgroundWorker.CancellationPending)
                        {
                            statusText = Language.GetLocalizedText(1046, "Download Canceled");
                        }
                        try
                        {
                            string url = Tile.GetUrl(layer, level, x, y);
                            string filename = Tile.GetFilename(layer, level, x, y);
                            string dir = Path.GetDirectoryName(filename);
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                            if (!File.Exists(filename))
                            {
                                client.DownloadFile(url, filename);
                            }
                            if (!string.IsNullOrEmpty(layer.DemUrl))
                            {
                                //do it for DEM as well
                                string urlDem = Tile.GetDemUrl(layer, level, x, y);
                                string filenameDem = Tile.GetDemFilename(layer, level, x, y);

                                string dirDem = Path.GetDirectoryName(filenameDem);
                                if (!Directory.Exists(dirDem))
                                {
                                    Directory.CreateDirectory(dirDem);
                                }
                                if (!File.Exists(filenameDem))
                                {
                                    client.DownloadFile(urlDem, filenameDem);
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = Math.Min(100, e.ProgressPercentage);
            progressText.Text = statusText;
            this.Invalidate();
        }

    
        int fileCount = 0;
        int maxFileCount = 0;
        int levelCount = 0;
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressText.Text = Language.GetLocalizedText(1047, "Download Complete");
            progressBar.Value = 100;
        }

        private void Download_Click(object sender, EventArgs e)
        {
            levelCount = (int)double.Parse(levels.Text);
            backgroundWorker.RunWorkerAsync();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
            }
            else
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                Close();
            }
        }
    }
}
