using System;
using System.ComponentModel;
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
            var client = new WebClient();
            var layer = imageSet;
            fileCount = 0;
            maxFileCount = 0;
            for (var level = layer.BaseLevel; level < (levelCount + 1); level++)
            {
                var maxX = Earth3d.GetTilesXForLevel(layer, level);
                var maxY = Earth3d.GetTilesYForLevel(layer, level);

                for (var x = 0; x < maxX; x++)
                {
                    for (var y = 0; y < maxY; y++)
                    {
                        maxFileCount++;
                        
                    }
                }
            }


            for (var level = layer.BaseLevel; level < (levelCount+1); level++)
            {
                var maxX = Earth3d.GetTilesXForLevel(layer, level);
                var maxY = Earth3d.GetTilesYForLevel(layer, level);

                for (var x = 0; x < maxX; x++)
                {
                    for (var y = 0; y < maxY; y++)
                    {
                        fileCount++;

                        statusText = string.Format(Language.GetLocalizedText(1045, "Downloading ({0} of {1})"), fileCount, maxFileCount);
                        backgroundWorker.ReportProgress(100 * fileCount / maxFileCount);
                        if (backgroundWorker.CancellationPending)
                        {
                            statusText = Language.GetLocalizedText(1046, "Download Canceled");
                        }
                        try
                        {
                            var url = Tile.GetUrl(layer, level, x, y);
                            var filename = Tile.GetFilename(layer, level, x, y);
                            var dir = Path.GetDirectoryName(filename);
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
                                var urlDem = Tile.GetDemUrl(layer, level, x, y);
                                var filenameDem = Tile.GetDemFilename(layer, level, x, y);

                                var dirDem = Path.GetDirectoryName(filenameDem);
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
            Invalidate();
        }

    
        int fileCount;
        int maxFileCount;
        int levelCount;
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
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }
    }
}
