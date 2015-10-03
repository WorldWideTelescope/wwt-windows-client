using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class AudioProperties : Form
    {
        public AudioProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.ok.Text = Language.GetLocalizedText(759, "Ok");
            this.Remove.Text = Language.GetLocalizedText(129, "Remove");
            this.AudioFileNameLabel.Text = Language.GetLocalizedText(1126, "Audio Filename");
            this.fadeOutLabel.Text = Language.GetLocalizedText(1127, "Fade Out");
            this.fadeInLabel.Text = Language.GetLocalizedText(1128, "Fade In");
            this.endLabel.Text = Language.GetLocalizedText(143, "End");
            this.beginLabel.Text = Language.GetLocalizedText(1129, "Begin");
            this.Repeat.Text = Language.GetLocalizedText(39, "Auto Repeat");
            this.Text = Language.GetLocalizedText(1130, "Audio Properties");
        }

        public AudioTrack Target = null;

        bool playing;

        private void Preview_Click(object sender, EventArgs e)
        {
            playing = !playing;

            if (playing)
            {             
                Target.Track.Play();
                Target.Track.Audio.PlaybackComplete += new EventHandler(Audio_Ending);
                Preview.Image = global::TerraViewer.Properties.Resources.button_pause_normal;
            }
            else
            {
                Target.Track.Stop();
                Target.Track.CleanUp();
                Preview.Image = global::TerraViewer.Properties.Resources.button_play_normal;
            }
        }

        void Audio_Ending(object sender, EventArgs e)
        {
            playing = false;
            Preview.Image = global::TerraViewer.Properties.Resources.button_play_normal;
        }

        private void Preview_MouseDown(object sender, MouseEventArgs e)
        {
            if (playing)
            {
                Preview.Image = global::TerraViewer.Properties.Resources.button_pause_pressed;
            }
            else
            {
                Preview.Image = global::TerraViewer.Properties.Resources.button_play_pressed;
            }
        }

        private void Preview_MouseEnter(object sender, EventArgs e)
        {
            if (playing)
            {
                Preview.Image = global::TerraViewer.Properties.Resources.button_pause_hover;
            }
            else
            {
                Preview.Image = global::TerraViewer.Properties.Resources.button_play_hover;
            }
        }

        private void Preview_MouseLeave(object sender, EventArgs e)
        {
            if (playing)
            {
                Preview.Image = global::TerraViewer.Properties.Resources.button_pause_normal;
            }
            else
            {
                Preview.Image = global::TerraViewer.Properties.Resources.button_play_normal;
            }
        }

        private void Preview_MouseUp(object sender, MouseEventArgs e)
        {
            if (playing)
            {
                Preview.Image = global::TerraViewer.Properties.Resources.button_pause_hover;
            }
            else
            {
                Preview.Image = global::TerraViewer.Properties.Resources.button_play_hover;
            }
        }

        private void Remove_Click(object sender, EventArgs e)
        {
            Target.RemoveTrack();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void AudioProperties_Load(object sender, EventArgs e)
        {
            AudioFileName.Text = Target.Track.Name;
            Begin.Text = Target.Track.Begin.ToString();
            End.Text = Target.Track.End.ToString();
            FadeIn.Text = Target.Track.FadeIn.ToString();
            FadeOut.Text = Target.Track.FadeOut.ToString();
            Repeat.Checked = Target.Track.Loop;
        }

        private void Repeat_CheckedChanged(object sender, EventArgs e)
        {
            Target.Track.Loop = Repeat.Checked;
        }

        private void Begin_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            Target.Track.Begin = UiTools.ParseAndValidateDouble(Begin, Target.Track.Begin, ref failed);
        }

        private void End_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            Target.Track.End = UiTools.ParseAndValidateDouble(End, Target.Track.End, ref failed);
        }

        private void FadeIn_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            Target.Track.FadeIn = UiTools.ParseAndValidateDouble(FadeIn, Target.Track.FadeIn, ref failed);
        }

        private void FadeOut_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            Target.Track.FadeOut = UiTools.ParseAndValidateDouble(FadeOut, Target.Track.FadeOut, ref failed);
        }

        private void AudioProperties_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (playing)
            {
                Target.Track.Stop();
                Target.Track.CleanUp();
            }
        }
    }
}
