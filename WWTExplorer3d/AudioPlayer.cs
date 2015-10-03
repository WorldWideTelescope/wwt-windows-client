using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;



namespace TerraViewer
{
    public class AudioPlayer : IDisposable
    {
        System.Windows.Media.MediaPlayer me;
        public static void Initialize()
        {

        }

        public static void Shutdown()
        {  
        }

        public AudioPlayer(string filename)
        {
            try
            {

                me = new System.Windows.Media.MediaPlayer();

                me.Open(new Uri(filename, UriKind.RelativeOrAbsolute));
                me.MediaEnded += new EventHandler(me_MediaEnded);
            }
            catch
            {
            }
            
        }

        void me_MediaEnded(object sender, EventArgs e)
        {
            if (PlaybackComplete != null)
            {
                PlaybackComplete.Invoke(this, new EventArgs());
            }
        }

        public void Dispose()
        {
            if (me != null)
            {
                me.Stop();
                me.Close();
                me = null;
            }
        }

        public void Play()
        {
            me.Play();
        }

        public void Pause()
        {
            me.Pause();
        }

        public void Stop()
        {
            me.Stop();
        }

        public event EventHandler PlaybackComplete;

 
        public double Volume
        {
            get
            {
                return 0;
            }
            set
            {
                if (me != null)
                {
                    me.Volume = value;
                }
            }
        }


        public double CurrentPosition
        {
            get
            {
                if (me != null)
                {
                    return me.Position.TotalSeconds;
                }
                return 0;
            }
            set
            {
                if (me != null)
                {
                    me.Position = TimeSpan.FromSeconds(value);
                }
            }
        }

        public double Duration
        {
            get
            {
                if (me != null)
                {
                    var d = me.NaturalDuration;
                    var ts = new TimeSpan();
                    if (d.HasTimeSpan)
                    {
                        ts = d.TimeSpan;
                    }

                    return ts.TotalSeconds;
                }
                return 0;
            }
        }


        public void Seek(double position)
        {

            me.Position = TimeSpan.FromSeconds(position);
            CurrentPosition = position;
        }
    }
}
