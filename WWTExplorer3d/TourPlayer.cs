using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;

namespace TerraViewer
{


    public class TourPlayer : IUiController , IDisposable
    {
        public TourPlayer()
        {
            callStack.Clear();
        }
        BlendState overlayBlend = new BlendState(false, 1000);

        public bool ProjectorServer = false;

       

        public void PreRender(Earth3d window)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }

            UpdateSlideStates();

        }

        public void Render(Earth3d window)
        {
            window.SetupMatricesOverlays();
            window.RenderContext11.DepthStencilMode = DepthStencilMode.Off;


            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }

            if (ProjectorServer)
            {
                overlayBlend.State = true;
            }

            if (!onTarget && !ProjectorServer)
            {
                slideStartTime = SpaceTimeController.MetaNow;
                if (Earth3d.MainWindow.OnTarget(Tour.CurrentTourStop.Target))
                {
                    onTarget = true;
                    overlayBlend.State = !Tour.CurrentTourStop.FadeInOverlays;
                    overlayBlend.TargetState = true;

                    if (!PreRoll)
                    {
                        if (tour.CurrentTourStop.MusicTrack != null)
                        {
                            tour.CurrentTourStop.MusicTrack.Play();
                        }

                        if (tour.CurrentTourStop.VoiceTrack != null)
                        {
                            tour.CurrentTourStop.VoiceTrack.Play();
                        }

                        foreach (Overlay overlay in tour.CurrentTourStop.Overlays)
                        {
                            overlay.Play();
                        }
                    }

                    LayerManager.SetVisibleLayerList(tour.CurrentTourStop.Layers);


                    if (tour.CurrentTourStop.KeyFramed)
                    {
                        tour.CurrentTourStop.KeyFrameMover.CurrentDateTime = tour.CurrentTourStop.StartTime;
                        tour.CurrentTourStop.KeyFrameMover.CurrentPosition = tour.CurrentTourStop.Target.CamParams;
                        tour.CurrentTourStop.KeyFrameMover.MoveTime = (double)(tour.CurrentTourStop.Duration.TotalMilliseconds / 1000.0);
                        Earth3d.MainWindow.Mover = tour.CurrentTourStop.KeyFrameMover;
                    }
                    else if (tour.CurrentTourStop.EndTarget != null && tour.CurrentTourStop.EndTarget.ZoomLevel != -1)
                    {
                        if (tour.CurrentTourStop.Target.Type == ImageSetType.SolarSystem)
                        {
                            tour.CurrentTourStop.Target.UpdatePlanetLocation(SpaceTimeController.UtcToJulian(tour.CurrentTourStop.StartTime));
                            tour.CurrentTourStop.EndTarget.UpdatePlanetLocation(SpaceTimeController.UtcToJulian(tour.CurrentTourStop.EndTime));
                        }
                        

                        Earth3d.MainWindow.Mover = new ViewMoverKenBurnsStyle(tour.CurrentTourStop.Target.CamParams, tour.CurrentTourStop.EndTarget.CamParams, tour.CurrentTourStop.Duration.TotalMilliseconds / 1000.0, tour.CurrentTourStop.StartTime, tour.CurrentTourStop.EndTime, tour.CurrentTourStop.InterpolationType);

                    }

                    Settings.TourSettings = tour.CurrentTourStop;
                    SpaceTimeController.Now = tour.CurrentTourStop.StartTime;
                    SpaceTimeController.SyncToClock = false;
                }
            }

            if (currentMasterSlide != null)
            {
                foreach (Overlay overlay in currentMasterSlide.Overlays)
                {
                    overlay.TweenFactor = 1f;

                    overlay.Draw3D(window.RenderContext11, 1.0f, false);
                }
            }

 

            if (onTarget || ProjectorServer)
            {
                foreach (Overlay overlay in tour.CurrentTourStop.Overlays)
                {
                    if (!Tour.CurrentTourStop.KeyFramed || (overlay.Animate && overlay.AnimationTarget == null))
                    {
                        overlay.TweenFactor = (float)CameraParameters.EaseCurve(tour.CurrentTourStop.TweenPosition, overlay.InterpolationType == InterpolationType.Default ? tour.CurrentTourStop.InterpolationType : overlay.InterpolationType);
                    }
                    overlay.Draw3D(window.RenderContext11, overlayBlend.Opacity, false);
                }
            }     
        }

        TourDocument tour = null;

        public TourDocument Tour
        {
            get { return tour; }
            set { tour = value; }
        }

        static bool playing = false;

        static public bool Playing
        {
            get { return playing; }
            set { playing = value; }
        }

        bool onTarget = false;
        DateTime slideStartTime;
        TourStop currentMasterSlide = null;
        public void NextSlide()
        {
            // Stop any existing current Slide
            if (tour.CurrentTourStop != null)
            {
                if (!tour.CurrentTourStop.MasterSlide)
                {
                    if (tour.CurrentTourStop.MusicTrack != null)
                    {
                        tour.CurrentTourStop.MusicTrack.Stop();
                    }

                    if (tour.CurrentTourStop.VoiceTrack != null)
                    {
                        tour.CurrentTourStop.VoiceTrack.Stop();
                    }

                    foreach (Overlay overlay in tour.CurrentTourStop.Overlays)
                    {
                        overlay.Stop();
                    }
                }
                else
                {
                    currentMasterSlide = tour.CurrentTourStop;
                }
            }
            // Check if this is the last slide in the deck
            if ((tour.CurrentTourstopIndex < (tour.TourStops.Count - 1)) || tour.CurrentTourStop.IsLinked)
            {
                // If there are more slides then move on..
                if (tour.CurrentTourStop.EndTarget != null)
                {
                    Earth3d.MainWindow.GotoTarget(false, true, tour.CurrentTourStop.EndTarget.CamParams, tour.CurrentTourStop.Target.StudyImageset, tour.CurrentTourStop.Target.BackgroundImageSet);
                    Earth3d.MainWindow.Mover = null;
                }
                onTarget = false;


                if (tour.CurrentTourStop.IsLinked && !PreRoll)
                {
                    try
                    {
                        switch (tour.CurrentTourStop.NextSlide)
                        {
                            case "Return":
                                if (callStack.Count > 0)
                                {
                                    Earth3d.MainWindow.TourEdit.PlayFromTourstop(tour.TourStops[callStack.Pop()]);

                                }
                                else
                                {
                                    tour.CurrentTourstopIndex = tour.TourStops.Count - 1;
                                }
                                break;
                            default:
                                Earth3d.MainWindow.TourEdit.PlayFromTourstop(tour.TourStops[tour.GetTourStopIndexByID(tour.CurrentTourStop.NextSlide)]);
                                break;
                        }
                    }
                    catch
                    {
                        if ((tour.CurrentTourstopIndex < (tour.TourStops.Count - 1)))
                        {
                            tour.CurrentTourstopIndex++;
                        }
                    }

                }
                else
                {
                    tour.CurrentTourstopIndex++;
                }

                Earth3d.MainWindow.TourEdit.EnsureSelectedVisible();


                if (currentMasterSlide != null && tour.CurrentTourStop.MasterSlide)
                {
                    StopCurrentMaster();
                }

                bool instant = false;
                switch (tour.CurrentTourStop.Transition)
                {
                    case TransitionType.Slew:
                        break;
                    case TransitionType.CrossFade:
                        instant = true;
                        break;
                    case TransitionType.CrossCut:
                        instant = true;
                        break;
                    case TransitionType.FadeOutIn:
                        instant = true;
                        break;
                    case TransitionType.FadeOut:
                        instant = true;
                        break;
                    case TransitionType.FadeIn:
                        instant = true;
                        break;
                    default:
                        break;
                }

                if (PreRoll)
                {
                    if (instant)
                    {
                        PreRollTime = 2;
                    }
                    else
                    {
                        PreRollTime = .2;
                    }
                    instant = true;
                }

                Earth3d.MainWindow.GotoTarget(tour.CurrentTourStop.Target, false, Earth3d.NoUi ? true : instant, false);

                slideStartTime = SpaceTimeController.MetaNow;
                // Move to new settings
                Settings.TourSettings = tour.CurrentTourStop;
                SpaceTimeController.Now = tour.CurrentTourStop.StartTime;
                SpaceTimeController.SyncToClock = false;


            }
            else
            {
                StopCurrentMaster();

                playing = false;

                if (PreRoll)
                {
                    PreRoll = false;
                    tour.CurrentTourstopIndex = -1;
                    Play();
                    return;
                }

                if (Properties.Settings.Default.AutoRepeatTour && !tour.EditMode)
                {
                    tour.CurrentTourstopIndex = -1;
                    Play();
                }
                else
                {
                    Earth3d.MainWindow.FreezeView();
                    if (TourEnded != null)
                    {
                        TourEnded.Invoke(this, new EventArgs());
                    }
                }
            }
        }

        private void StopCurrentMaster()
        {
            if (currentMasterSlide != null)
            {
                if (currentMasterSlide.MusicTrack != null)
                {
                    currentMasterSlide.MusicTrack.Stop();
                }

                if (currentMasterSlide.VoiceTrack != null)
                {
                    currentMasterSlide.VoiceTrack.Stop();
                }

                foreach (Overlay overlay in currentMasterSlide.Overlays)
                {
                    overlay.Stop();
                }
                currentMasterSlide = null;
            }
        }

        static public event EventHandler TourEnded;
        static bool switchedToFullScreen = false;
        Stack<int> callStack = new Stack<int>();
        public void Play()
        {


            if (tour == null)
            {
                return;
            }

            if (ProjectorServer)
            {
                slideStartTime = SpaceTimeController.MetaNow;
                playing = true;
                onTarget = true;
                tour.CurrentTourstopIndex = -1;
                return;
            }

            if (playing)
            {
                Stop(true);
            }
            else
            {
                playing = true;
                if (!Settings.DomeView && Properties.Settings.Default.FullScreenTours)
                {
                    switchedToFullScreen = !Earth3d.FullScreen;
                    if (switchedToFullScreen)
                    {
                        Earth3d.MainWindow.ShowFullScreen(true);
                    }
                }
                else
                {
                    switchedToFullScreen = false;
                }
            }
          
            playing = true;
            

            if (tour.TourStops.Count > 0)
            {
                onTarget = false;
                if (tour.CurrentTourstopIndex == -1)
                {
                    tour.CurrentTourStop = tour.TourStops[0];
                }

                if (tour.CurrentTourstopIndex > 0)
                {
                    PlayMasterForCurrent();

                }
               
                Earth3d.MainWindow.GotoTarget(tour.CurrentTourStop.Target, false, Earth3d.NoUi ? false : true, false);

            }

            slideStartTime = SpaceTimeController.MetaNow;
            playing = true;

        }

        private void PlayMasterForCurrent()
        {
            if (PreRoll)
            {
                return;
            }

            if (!tour.CurrentTourStop.MasterSlide)
            {
                double elapsed = tour.ElapsedTimeSinceLastMaster(tour.CurrentTourstopIndex, out currentMasterSlide);
                if (currentMasterSlide != null)
                {
                    if (currentMasterSlide.MusicTrack != null)
                    { 
                        currentMasterSlide.MusicTrack.Play();
                        currentMasterSlide.MusicTrack.Seek(elapsed);
                    }

                    if (currentMasterSlide.VoiceTrack != null)
                    {
                        
                        currentMasterSlide.VoiceTrack.Play();
                        currentMasterSlide.VoiceTrack.Seek(elapsed);
                    }

                    foreach (Overlay overlay in currentMasterSlide.Overlays)
                    {
                        overlay.Play();
                        overlay.Seek(elapsed);
                    }
                }
            }
        }

        public void Stop(bool noSwitchBackFullScreen)
        {


            if (switchedToFullScreen && !noSwitchBackFullScreen)
            {
                Earth3d.MainWindow.ShowFullScreen(false);
            }

            Settings.TourSettings = null;
            playing = false;
            if (tour.CurrentTourStop != null)
            {
                if (tour.CurrentTourStop.MusicTrack != null)
                {
                    tour.CurrentTourStop.MusicTrack.Stop();
                }

                if (tour.CurrentTourStop.VoiceTrack != null)
                {
                    tour.CurrentTourStop.VoiceTrack.Stop();
                }

                foreach (Overlay overlay in tour.CurrentTourStop.Overlays)
                {
                    overlay.Stop();
                }
            }
            if (currentMasterSlide != null)
            {
                if (currentMasterSlide.MusicTrack != null)
                {
                    currentMasterSlide.MusicTrack.Stop();
                }

                if (currentMasterSlide.VoiceTrack != null)
                {
                    currentMasterSlide.VoiceTrack.Stop();
                }

                foreach (Overlay overlay in currentMasterSlide.Overlays)
                {
                    overlay.Stop();
                }
            }
        }
        private int lastSlideIndex = -1;

        public bool PreRoll = false;
        public double PreRollTime = 2;
        public void UpdateSlideStates()
        {
            bool slideChanging = false;


            if (!ProjectorServer)
            {
                TimeSpan slideElapsedTime = SpaceTimeController.MetaNow - slideStartTime;

                if ((slideElapsedTime > tour.CurrentTourStop.Duration && playing) || (slideElapsedTime.TotalSeconds > PreRollTime && PreRoll))
                {
                    NextSlide();
                    slideChanging = true;
                }

                slideElapsedTime = SpaceTimeController.MetaNow - slideStartTime;

                if (tour.CurrentTourStop != null)
                {
                    tour.CurrentTourStop.TweenPosition = Math.Min(1, (float)(slideElapsedTime.TotalMilliseconds / tour.CurrentTourStop.Duration.TotalMilliseconds));
                }
            }
            else
            {
                try
                {
                    if (tour.CurrentTourStop != null)
                    {
                        if (lastSlideIndex != LayerManager.CurrentSlideID)
                        {
                            slideChanging = true;
                            lastSlideIndex = LayerManager.CurrentSlideID;
                            tour.CurrentTourstopIndex = LayerManager.CurrentSlideID;
                            if (tour.CurrentTourStop.MasterSlide)
                            {
                                currentMasterSlide = null;
                            }
                            else
                            {
                                currentMasterSlide = tour.GetMasterSlideForIndex(tour.CurrentTourstopIndex);
                            }
                            LayerManager.SetVisibleLayerList(tour.CurrentTourStop.Layers);
                        }
                        tour.CurrentTourStop.TweenPosition = LayerManager.SlideTweenPosition;
                    }
                }
                catch
                {
                }
                onTarget = true;
            }
   

            if (tour.CurrentTourStop != null)
            {
                tour.CurrentTourStop.FaderOpacity = 0;
                Tile.fastLoad = false;
                double elapsedSeconds = tour.CurrentTourStop.TweenPosition * tour.CurrentTourStop.Duration.TotalSeconds;
                if (slideChanging)
                {
                    Earth3d.MainWindow.CrossFadeFrame = false;
                }

                switch (tour.CurrentTourStop.Transition)
                {
                    case TransitionType.Slew:
                        tour.CurrentTourStop.FaderOpacity = 0;
                        Earth3d.MainWindow.CrossFadeFrame = false;
                        break;
                    case TransitionType.CrossCut:
                        {
                            if (slideChanging)
                            {
                                Tile.fastLoad = true;
                                Tile.fastLoadAutoReset = false;
                            }
                            if (elapsedSeconds < (elapsedSeconds - tour.CurrentTourStop.TransitionHoldTime))
                            {
                                Earth3d.MainWindow.CrossFadeFrame = true;
                                tour.CurrentTourStop.FaderOpacity = 1;
                               
                            }
                            else
                            {
                                tour.CurrentTourStop.FaderOpacity = 0;
                                Earth3d.MainWindow.CrossFadeFrame = false;
                            }
                        }
                        break;
                    case TransitionType.CrossFade:
                        {
                            Earth3d.MainWindow.CrossFadeFrame = true;
                            double opacity = Math.Max(0, 1 - Math.Min(1, (elapsedSeconds-tour.CurrentTourStop.TransitionHoldTime) / tour.CurrentTourStop.TransitionTime));
                            tour.CurrentTourStop.FaderOpacity = (float)opacity;
                            if (slideChanging)
                            {
                                Tile.fastLoad = true;
                                Tile.fastLoadAutoReset = false;
                            }
                        }
                        break;
                    case TransitionType.FadeOutIn:
                    case TransitionType.FadeIn:
                        {
                            Earth3d.MainWindow.CrossFadeFrame = false;
                            double opacity = Math.Max(0, 1 - Math.Min(1, (elapsedSeconds - tour.CurrentTourStop.TransitionHoldTime) / tour.CurrentTourStop.TransitionTime));
                            tour.CurrentTourStop.FaderOpacity = (float)opacity;
                        }
                        break;
                    case TransitionType.FadeOut:
                        Earth3d.MainWindow.CrossFadeFrame = false;
                        break;

                    default:
                        break;
                }

                if (!tour.CurrentTourStop.IsLinked && tour.CurrentTourstopIndex < (tour.TourStops.Count-1))
                {
                    TransitionType nextTrans = tour.TourStops[tour.CurrentTourstopIndex + 1].Transition;
                    double nextTransTime = tour.TourStops[tour.CurrentTourstopIndex + 1].TransitionOutTime;


                    switch (nextTrans)
                    {

                        case TransitionType.FadeOut:
                        case TransitionType.FadeOutIn:
                            {
                                if (tour.CurrentTourStop.FaderOpacity == 0)
                                {
                                    Earth3d.MainWindow.CrossFadeFrame = false;
                                    double opacity = Math.Max(0, 1 - Math.Min(1, (tour.CurrentTourStop.Duration.TotalSeconds - elapsedSeconds) / nextTransTime));
                                    tour.CurrentTourStop.FaderOpacity = (float)opacity;
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        public float UpdateTweenPosition(float tween)
        {
            TimeSpan slideElapsedTime = SpaceTimeController.MetaNow - slideStartTime;

            if (tween > -1)
            {
                return tour.CurrentTourStop.TweenPosition = Math.Min(1,tween);
            }
            else
            {
                return tour.CurrentTourStop.TweenPosition = Math.Min(1, (float)(slideElapsedTime.TotalMilliseconds / tour.CurrentTourStop.Duration.TotalMilliseconds));
            }
        }

        public void Close()
        {
            if (tour != null)
            {
                // todo check for changes
                tour = null;
            }
        }

        public bool MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            PointF location = PointToView(e.Location);

            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            for (int i = tour.CurrentTourStop.Overlays.Count - 1; i >= 0; i--)
            {

                if (tour.CurrentTourStop.Overlays[i].HitTest(location))
                {
                    if (!string.IsNullOrEmpty(tour.CurrentTourStop.Overlays[i].Url))
                    {
                        Overlay linkItem = tour.CurrentTourStop.Overlays[i];
                        WebWindow.OpenUrl(linkItem.Url, true);
                        return true;
                    }
                    if (!string.IsNullOrEmpty(tour.CurrentTourStop.Overlays[i].LinkID))
                    {
                        try
                        {
                            switch (tour.CurrentTourStop.Overlays[i].LinkID)
                            {

                                case "Return":
                                    if (callStack.Count > 0)
                                    {
                                        Earth3d.MainWindow.TourEdit.PlayFromTourstop(tour.TourStops[callStack.Pop()]);

                                    }
                                    else
                                    {
                                        tour.CurrentTourstopIndex = tour.TourStops.Count - 1;
                                    }
                                    break;
                                default:
                                    callStack.Push(tour.CurrentTourstopIndex);
                                    Earth3d.MainWindow.TourEdit.PlayFromTourstop(tour.TourStops[tour.GetTourStopIndexByID(tour.CurrentTourStop.Overlays[i].LinkID)]);
                                    break;
                            }

                            return true;
                        }

                        catch
                        {
                            return false;
                        }
                    }
                }
            }

            
            return false;
        }

        public bool MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            return false;
        }

        public bool MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            PointF location = PointToView(e.Location);

            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            for (int i = tour.CurrentTourStop.Overlays.Count - 1; i >= 0; i--)
            {
                if (tour.CurrentTourStop.Overlays[i].HitTest(location) && (!string.IsNullOrEmpty(tour.CurrentTourStop.Overlays[i].Url) ||!string.IsNullOrEmpty(tour.CurrentTourStop.Overlays[i].LinkID)))
                {
                    Cursor.Current = Cursors.Hand;
                    return true;
                }
            }
            return false;
        }

        public bool MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            return false;
        }

        public bool Click(object sender, EventArgs e)
        {
            return false;
        }

        public bool MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            return false;
        }

        public bool KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (Earth3d.MainWindow.TourEdit == null)
            {
                return false;
            }
            switch (e.KeyCode)
            {
                case Keys.Escape:
                case Keys.Space:

                    Earth3d.MainWindow.TourEdit.PauseTour();
                    return true;
                case Keys.PageDown:
                case Keys.Right:
                    MoveNextSlide();
                    Earth3d.MainWindow.TourEdit.EnsureSelectedVisible();
                    return true;
                case Keys.PageUp:
                case Keys.Left:
                    MovePreviousSlide();
                    Earth3d.MainWindow.TourEdit.EnsureSelectedVisible();
                    return true;
                case Keys.End:
                    MoveToEndSlide();
                    Earth3d.MainWindow.TourEdit.EnsureSelectedVisible();
                    return true;
                case Keys.Home:
                    MoveToFirstSlide();
                    Earth3d.MainWindow.TourEdit.EnsureSelectedVisible();
                    return true;
            }

            return false;
        }

        public void MoveToFirstSlide()
        {
            if (tour.TourStops.Count > 0)
            {
                Earth3d.MainWindow.TourEdit.PlayFromTourstop(tour.TourStops[0]);

            }
        }

        public void MoveToEndSlide()
        {
            if (tour.TourStops.Count > 0)
            {
                Earth3d.MainWindow.TourEdit.PlayFromTourstop(tour.TourStops[tour.TourStops.Count - 1]);
            }
        }

        public void MovePreviousSlide()
        {
            if (tour.CurrentTourstopIndex > 0)
            {
                Earth3d.MainWindow.TourEdit.PlayFromTourstop(tour.TourStops[tour.CurrentTourstopIndex - 1]);
            }
        }
        
        public void MoveNextSlide()
        {
            if ((tour.CurrentTourstopIndex < tour.TourStops.Count - 1) && tour.TourStops.Count > 0)
            {
                Earth3d.MainWindow.TourEdit.PlayFromTourstop(tour.TourStops[tour.CurrentTourstopIndex + 1]);
            }
        }

        public void MoveToSlide(int slideID)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }

            if (tour.TourStops.Count > slideID && slideID > -1)
            {
                Earth3d.MainWindow.TourEdit.PlayFromTourstop(tour.TourStops[slideID]);
            }
        }                     

        public bool KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            return false;
        }

         
        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion


        public bool Hover(Point pnt)
        {
            if (playing)
            {
                return true;
            }
            return false;
        }

        public PointF PointToView(Point pnt)
        {
            float clientHeight = Earth3d.MainWindow.RenderWindow.ClientRectangle.Height;
            float clientWidth = Earth3d.MainWindow.RenderWindow.ClientRectangle.Width;
            float viewWidth = ((float)Earth3d.MainWindow.RenderWindow.ClientRectangle.Width / (float)Earth3d.MainWindow.RenderWindow.ClientRectangle.Height) * 1116f;
            float x = (((float)pnt.X) / ((float)clientWidth) * viewWidth) - ((viewWidth - 1920) / 2);
            float y = ((float)pnt.Y) / clientHeight * 1116;

            return new PointF(x, y);
        }
    }

}
