using System;
using System.Windows.Forms;
using System.IO;

namespace TerraViewer
{
    public partial class OutputTourToVideo : Form
    {
        public OutputTourToVideo()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            WaitForDownloads.Text = Language.GetLocalizedText(882, "Wait for all downloads");
            Close.Text = Language.GetLocalizedText(212, "Close");
            PathLabel.Text = Language.GetLocalizedText(883, "Filename Out (Use {NNNN} notation for auto frame numbering)");
            Browse.Text = Language.GetLocalizedText(884, "Browse");
            label1.Text = Language.GetLocalizedText(209, "Width");
            label2.Text = Language.GetLocalizedText(208, "Height");
            OutputFormatLabel.Text = Language.GetLocalizedText(885, "Output Format");
            fpsLabel.Text = Language.GetLocalizedText(886, "FPS");
            Render.Text = Language.GetLocalizedText(887, "Render");
            label3.Text = Language.GetLocalizedText(424, "Run Time");
            totalTimeLabel.Text = Language.GetLocalizedText(888, "Total Frames");
            label4.Text = Language.GetLocalizedText(889, "Start Frame");
            DomeMaster.Text = Language.GetLocalizedText(890, "Dome Master");
            Text = Language.GetLocalizedText(891, "Render Tour to Video");
        }

        private void OutputTourToVideo_Load(object sender, EventArgs e)
        {
            OutputFormatCombo.Items.Add(new VideoOutputType(Language.GetLocalizedText(892, "Standard Definition"), 640, 480, 30, false));
            OutputFormatCombo.Items.Add(new VideoOutputType(Language.GetLocalizedText(893, "High Definition 720P"), 1280, 720, 30, false));
            OutputFormatCombo.Items.Add(new VideoOutputType(Language.GetLocalizedText(894, "High Definition 1080P"), 1920, 1080, 30, false));
            OutputFormatCombo.Items.Add(new VideoOutputType(Language.GetLocalizedText(895, "Dome Master 1k"), 1024, 1024, 30, true));
            OutputFormatCombo.Items.Add(new VideoOutputType(Language.GetLocalizedText(896, "Dome Master 2k"), 2048, 2048, 30, true));
            OutputFormatCombo.Items.Add(new VideoOutputType(Language.GetLocalizedText(897, "Dome Master 3k"), 3072, 3072, 30, true));
            OutputFormatCombo.Items.Add(new VideoOutputType(Language.GetLocalizedText(898, "Dome Master 4k"), 4096, 4096, 30, true));
            OutputFormatCombo.Items.Add(new VideoOutputType(Language.GetLocalizedText(1044, "Dome Master 8k"), 8192, 8192, 30, true));
            OutputFormatCombo.Items.Add(new VideoOutputType(Language.GetLocalizedText(899, "Custom"), 0, 0, 0, false));
            OutputFormatCombo.SelectedIndex = Properties.Settings.Default.LastVideoOutFormat;
            var ts = Target.RunTime;
            runTimeEdit.Text = String.Format("{0:0}:{1:00}", ts.Minutes, ts.Seconds);
            if (string.IsNullOrEmpty(Properties.Settings.Default.VideoOutputPath))
            {
                Properties.Settings.Default.VideoOutputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\VideoFrames\\VideoOut.png";
            }
            PathEdit.Text = Properties.Settings.Default.VideoOutputPath;
        }

        private void Close_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void Render_Click(object sender, EventArgs e)
        {
            try
            {
                var pathPart = Path.GetDirectoryName(PathEdit.Text);
                if (!Directory.Exists(pathPart))
                {
                    Directory.CreateDirectory(pathPart);
                }

                Properties.Settings.Default.VideoOutputPath = PathEdit.Text;
                RenderValues = new VideoOutputType(PathEdit.Text, int.Parse(widthEdit.Text), int.Parse(heightEdit.Text), double.Parse(fpsEdit.Text), DomeMaster.Checked);
                RenderValues.WaitDownload = WaitForDownloads.Checked;
                RenderValues.TotalFrames = int.Parse(totalFramesEdit.Text);
                RenderValues.StartFrame = int.Parse(startFrameEdit.Text);
                DialogResult = DialogResult.OK;
            }
            catch
            {
                UiTools.ShowMessageBox(Language.GetLocalizedText(758, "Check your values. You must enter valid numbers and a filename"));
            }

        }

        private void OutputFormatCombo_SelectionChanged(object sender, EventArgs e)
        {
            var vidType = (VideoOutputType)OutputFormatCombo.SelectedItem;

            if (vidType.Name.StartsWith(Language.GetLocalizedText(899, "Custom")))
            {
                widthEdit.ReadOnly = false;
                heightEdit.ReadOnly = false;
                fpsEdit.ReadOnly = false;
            }
            else
            {
                widthEdit.Text = vidType.Width.ToString();
                heightEdit.Text = vidType.Height.ToString();
                fpsEdit.Text = vidType.Fps.ToString();
                DomeMaster.Checked = vidType.Dome;
                widthEdit.ReadOnly = true;
                heightEdit.ReadOnly = true;
                fpsEdit.ReadOnly = true;
            }
            Properties.Settings.Default.LastVideoOutFormat = OutputFormatCombo.SelectedIndex;

        }

        public TourDocument Target = null;

        public VideoOutputType RenderValues = null;
        private void fpsEdit_TextChanged(object sender, EventArgs e)
        {
            double val;
            if (double.TryParse(fpsEdit.Text, out val))
            {
                var ts = Target.RunTime;
                totalFramesEdit.Text = ((int)(ts.TotalSeconds * val + .5)).ToString();
            }

        }

        private void Browse_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = Language.GetLocalizedText(978, "Portable Network Graphics(*.png)|*.png");
            sfd.AddExtension = true;
            sfd.DefaultExt = ".png";
            sfd.FileName = "VideoFrame.png";
    
            sfd.FileName = PathEdit.Text;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                PathEdit.Text = sfd.FileName;
            }
        }

    }

    public class VideoOutputType
    {
        public string Name;
        public double Fps;
        public int Width;
        public int Height;
        public bool Dome;
        public int TotalFrames = 0;
        public int StartFrame = 0;
        public bool WaitDownload = false;
        public VideoOutputType()
        {
           
        }

        public VideoOutputType(string name, int width, int height, double fps, bool dome)
        {
            Name = name;
            Width = width;
            Height = height;
            Fps = fps;
            Dome = dome;
        }
        public override string ToString()
        {
            return string.Format("{0} ({1} x {2} : {3}{4})", Name, Width, Height, Fps, Language.GetLocalizedText(886, "FPS"));
        }
    }
}
