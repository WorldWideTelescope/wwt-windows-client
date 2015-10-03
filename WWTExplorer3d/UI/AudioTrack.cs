using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class AudioTrack : UserControl
    {

        TourStop target;

        public TourStop Target
        {
            get { return target; }
            set
            {
                target = value;
                UpdateTrackStatus();
            }
        }

        private void UpdateTrackStatus()
        {
            var enable = Track != null;
            this.Enabled = target!=null;
            Browse.Enabled = this.Enabled;
            if (enable)
            {
                Volume.Value = Track.Volume;
            }
            MuteButton.Enabled = enable;
            Volume.Enabled = enable;
            punchIn.Enabled = enable;
            punchOut.Enabled = enable;

            
            if (enable)
            {
                Browse.Text = Language.GetLocalizedText(25, "Edit...");
            }
            else
            {
                Browse.Text = Language.GetLocalizedText(130, "Browse...");
            }

            Refresh();
        }

        internal AudioOverlay Track
        {
            get
            {
                if (target == null)
                {
                    return null;
                }

                if (trackType == AudioType.Music)
                {
                    return target.MusicTrack;
                }
                else
                {
                    return target.VoiceTrack;
                }
            }
        }


        AudioType trackType = AudioType.Music;

        public  AudioType TrackType
        {
            get { return trackType; }
            set { trackType = value; }
        }

        public AudioTrack()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.label43.Text = Language.GetLocalizedText(142, "In:");
            this.punchOut.Text = Language.GetLocalizedText(143, "End");
            this.label3.Text = Language.GetLocalizedText(144, "Out:");
            this.Volume.Name = Language.GetLocalizedText(145, "Volume");
            this.Browse.Text = Language.GetLocalizedText(130, "Browse...");
            
        }

        bool mute;

        public bool Mute
        {
            get { return mute; }
            set
            {
                mute = value;
                MuteButton.ImageDisabled = mute ? Properties.Resources.glyph_mute_on_disabled : Properties.Resources.glyph_mute_off_disabled;
                MuteButton.ImageEnabled = mute ? Properties.Resources.glyph_mute_on_enabled : Properties.Resources.glyph_mute_off_enabled;
            }
        }

        private void AudioTrack_Load(object sender, EventArgs e)
        {

        }

        private void Browse_Click(object sender, EventArgs e)
        {
            if (target != null)
            {
                if (trackType == AudioType.Music)
                {
                    if (target.MusicTrack != null)
                    {
                        //todo localize
                        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(524, "Edit Music"), target.Owner));
                        ShowAudioProperties();
                        return;
                    }
                }
                else
                {
                    if (target.VoiceTrack != null)
                    {
                        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(525, "Edit Voiceover"), target.Owner)); UpdateTrackStatus();
                        ShowAudioProperties();
                        return;
                    }
                }
                
                var fileDialog = new OpenFileDialog();
                fileDialog.Filter = Language.GetLocalizedText(526, "Sound/Music(*.MP3;*.WMA)|*.MP3;*.WMA");

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var filename = fileDialog.FileName;
                        var audio = new AudioOverlay( target, filename);
                        audio.Name = filename.Substring(filename.LastIndexOf("\\") + 1);
                        if (trackType == AudioType.Music)
                        {
                            //todo localize
                            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(527, "Add Music"), target.Owner));
                            target.MusicTrack = audio;
                        }
                        else
                        {
                            //todo localize
                            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(528, "Add Voiceover"), target.Owner));
                            target.VoiceTrack = audio;
                        }
                        UpdateTrackStatus();
                        ShowAudioProperties();
                    }
                    catch
                    {
                        MessageBox.Show(Language.GetLocalizedText(131, "Could not load audio file. Check to make sure it is valid, a supported type and of a reasonable size."));
                    }
                }
            }
        }

        private void ShowAudioProperties()
        {
            var props = new AudioProperties();
            props.Owner = Earth3d.MainWindow;
            props.Target = this;
            props.ShowDialog();
            UpdateTrackStatus();
        }

        public void RemoveTrack()
        {
            if (trackType == AudioType.Music)
            {
                if (target.MusicTrack != null)
                {
                    //todo localize
                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(524, "Remove Music"), target.Owner));
                    target.MusicTrack.CleanUp();
                    target.MusicTrack = null;
                    UpdateTrackStatus();
                    return;
                }
            }
            else
            {
                if (target.VoiceTrack != null)
                {
                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(525, "Remove Voiceover"), target.Owner)); UpdateTrackStatus();
                    target.VoiceTrack.CleanUp();
                    target.VoiceTrack = null;
                    //todo localize
                    return;
                }
            }
            return;
        }

        private void Mute_Click(object sender, EventArgs e)
        {
            if (Track != null)
            {
                Track.Mute = Mute;
            }
        }

        private void punchIn_DoubleClick(object sender, EventArgs e)
        {

        }

        private void punchOut_DoubleClick(object sender, EventArgs e)
        {

        }

        private void AudioTrack_Paint(object sender, PaintEventArgs e)
        {
            var p = new Pen(Color.FromArgb(62, 73, 92));
            e.Graphics.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
            p.Dispose();
            var label = (trackType == AudioType.Music ? Language.GetLocalizedText(132, "Music: ") : Language.GetLocalizedText(133, "Voiceover: "));

            if (Track != null)
            {
                label = label + Track.Name;
            }
            var rectText = new RectangleF(3,3, 185, 15);
            e.Graphics.DrawString(label, UiTools.StandardRegular, UiTools.StadardTextBrush, rectText, UiTools.StringFormatBottomLeft);

        }

        private void Mute_MouseDown(object sender, MouseEventArgs e)
        {
            Mute = !Mute;
        }

        private void Volume_ValueChanged(object sender, EventArgs e)
        {
            var eventArgs = (ScrollEventArgs)e;
            if (eventArgs.Type == ScrollEventType.EndScroll)
            {
                //todo localize
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(529, "Change Volume"), target.Owner));

                if (Track != null)
                {
                    Track.Volume = Volume.Value;
                }
            }

        }
    }
    public enum AudioType { Music, Voice };

}
