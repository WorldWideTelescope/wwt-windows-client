using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class TourEditTab : TabForm
    {
        public TourEditTab()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.toolTip.SetToolTip(this.tourStopList, Language.GetLocalizedText(416, "Slides"));
            this.AddText.Text = Language.GetLocalizedText(417, "Text");
            this.EditTourProperties.Text = Language.GetLocalizedText(418, "Tour Properties");
            this.AddShape.Text = Language.GetLocalizedText(419, "Shapes");
            this.AddPicture.Text = Language.GetLocalizedText(420, "Picture");
            this.AddVideo.Text = Language.GetLocalizedText(421, "Video");
            this.SaveTour.Text = Language.GetLocalizedText(422, " Save");
            this.ShowSafeArea.Text = Language.GetLocalizedText(423, "Show Safe Area");
            this.runTimeLabel.Text = Language.GetLocalizedText(424, "Run Time");
            this.Dome.Text = Language.GetLocalizedText(1109, "Dome");
        }
        TourDocument tour = null;
        public TourEditor TourEditorUI = new TourEditor();
        public TourDocument Tour
        {
            get
            {
                return tour;
            }
            set
            {
                tour = value;
                TourEditorUI.Tour = tour;
                tourStopList.Tour = tour;
                
                Dome.Checked = Tour.DomeMode;
                tour.CurrentTourstopChanged += new EventHandler(tour_CurrentTourstopChanged);
                Overlay.DefaultAnchor = Tour.DomeMode ? OverlayAnchor.Dome : OverlayAnchor.Screen;
                if (tour.TourStops.Count > 0)
                {
                    Earth3d.MainWindow.GotoTarget(tour.TourStops[0].Target, false, true, false);
                    tour.CurrentTourstopIndex = 0;
                    tourStopList.SelectedItem = tour.CurrentTourstopIndex;
                    MusicTrack.Target = tour.CurrentTourStop;
                    VoiceTrack.Target = tour.CurrentTourStop;
                    LayerManager.SetVisibleLayerList(tour.CurrentTourStop.Layers);
                }
                SetEditMode(tour.EditMode);
             

            }
        }

        void tour_CurrentTourstopChanged(object sender, EventArgs e)
        {
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, TourEditorUI.Selection);
            tourStopList.Refresh();
        }

        protected override void SetFocusedChild()
        {
            tourStopList.Focus();
        }

        public void SelectCurrent()
        {
            tourStopList.SelectedItem = tour.CurrentTourstopIndex;
            tourStopList.Refresh();
        }

        private void TourEdit_Load(object sender, EventArgs e)
        {
            if (tour != null && !tour.EditMode)
            {
                PlayNow(true);
            }
            TourPlayer.TourEnded += new EventHandler(TourPlayer_TourEnded);
            ShowSafeArea.Checked = Properties.Settings.Default.ShowSafeArea;

            if (tour.EditMode)
            {
                tourStopList.AllowMultipleSelection = true;
            }
        }

        public void PlayNow(bool fromStart)
        {
            playing = true;
            if (Tour.EditMode || fromStart)
            {
                Tour.CurrentTourstopIndex = -1;
            }
            SetPlayPauseMode();
        }

        void TourPlayer_TourEnded(object sender, EventArgs e)
        {
            if (tour != null && !tour.EditMode && !Properties.Settings.Default.AutoRepeatTourAll && !Settings.DomeView)
            {
                //TourPopup endTour = TourPopup.ShowEndTourPopup(tour);
                //endTour.LaunchTour += new EventHandler(endTour_LaunchTour);
                //endTour.ClosedTour += new EventHandler(endTour_CloseTour);
                Earth3d.MainWindow.ShowTourCompleteDialog = true;
            }
        }

        void endTour_CloseTour(object sender, EventArgs e)
        {
            Earth3d.MainWindow.CloseTour(false);
        }

        void endTour_LaunchTour(object sender, EventArgs e)
        {
            PlayNow(true);
        }


        public void SetEditMode(bool visible)
        {
            tour.EditMode = visible;

            EditTourProperties.Visible = visible;
            SaveTour.Visible = visible;
            AddText.Visible = visible;
            AddShape.Visible = visible;
            AddPicture.Visible = visible;
            MusicTrack.Visible = visible;
            VoiceTrack.Visible = visible;
            ShowSafeArea.Visible = visible;
            Dome.Visible = visible;

            if (visible)
            {
                tourStopList.Width = (EditTourProperties.Left - 3) - tourStopList.Location.X;
                tourStopList.AllowMultipleSelection = true;
            }
            else
            {
                tourStopList.Width = (MusicTrack.Right) - tourStopList.Location.X;
                tourStopList.AllowMultipleSelection = false;
            }
            tourStopList.ShowAddButton = visible;

        }

        private void tourStopList_ItemClicked(object sender, TourStop e)
        {
            if (tour.CurrentTourStop != e)
            {
                tour.CurrentTourStop = e;

                if (e != null)
                {
                    MusicTrack.Target = tour.CurrentTourStop;
                    VoiceTrack.Target = tour.CurrentTourStop;
                }
                else
                {
                    MusicTrack.Target = null;
                    VoiceTrack.Target = null;

                }
                TourEditorUI.ClearSelection();
            }
            if (playing)
            {
                playFromHere_Click(sender, new EventArgs());
            }

        }


        private void tourStopList_ItemDoubleClicked(object sender, TourStop e)
        {
            ShowSlideStartPosition(e);

        }

        public void ShowSlideStartPosition(TourStop ts)
        {
            tour.CurrentTourStop = ts;
            //tourText.Text = tour.CurrentTourStop.Description;
            if (ts != null)
            {
                MusicTrack.Target = tour.CurrentTourStop;
                VoiceTrack.Target = tour.CurrentTourStop;
            }
            else
            {
                MusicTrack.Target = null;
                VoiceTrack.Target = null;

            }
            TourEditorUI.ClearSelection();
            if (tour.CurrentTourStop != null)
            {
                tour.CurrentTourStop.SyncSettings();
                SpaceTimeController.Now = tour.CurrentTourStop.StartTime;
                SpaceTimeController.SyncToClock = false;
                Earth3d.MainWindow.GotoTarget(ts.Target, false, true, false);
                tour.CurrentTourStop.TweenPosition = 0f;
                tour.CurrentTourStop.UpdateLayerOpacity();
                LayerManager.SetVisibleLayerList(tour.CurrentTourStop.Layers);

            }
        }


        ContextMenuStrip contextMenu = new ContextMenuStrip();

        private void tourStopList_MouseClick(object sender, MouseEventArgs e)
        {
            if (!tour.EditMode)
            {
                return;
            }
            IDataObject dataObject = Clipboard.GetDataObject();

            tour.CurrentTourstopIndex = tourStopList.SelectedItem;


            if (tourStopList.HitType == TourStopList.HitPosition.Transition)
            {
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                if (tourStopList.MultipleSelection)
                {
                    if (contextMenu != null)
                    {
                        contextMenu.Dispose();
                    }
                    // MultiSelection Menu
                    contextMenu = new ContextMenuStrip();
                    ToolStripMenuItem selectAllMenu = new ToolStripMenuItem(Language.GetLocalizedText(1345, "Select All"));
                    ToolStripMenuItem cutMenu = new ToolStripMenuItem(Language.GetLocalizedText(427, "Cut"));
                    ToolStripMenuItem copyMenu = new ToolStripMenuItem(Language.GetLocalizedText(428, "Copy"));
                    ToolStripMenuItem pasteMenu = new ToolStripMenuItem(Language.GetLocalizedText(429, "Paste"));
                    ToolStripMenuItem deleteMenu = new ToolStripMenuItem(Language.GetLocalizedText(167, "Delete"));
                    cutMenu.Click += new EventHandler(cutMenu_Click);
                    copyMenu.Click += new EventHandler(copyMenu_Click);
                    pasteMenu.Click += new EventHandler(pasteMenu_Click);
                    deleteMenu.Click += new EventHandler(deleteMenu_Click);
                    selectAllMenu.Click += new EventHandler(selectAllMenu_Click);
                    ToolStripSeparator sep1 = new ToolStripSeparator();

                    contextMenu.Items.Add(selectAllMenu);
                    contextMenu.Items.Add(sep1);
                    contextMenu.Items.Add(cutMenu);
                    contextMenu.Items.Add(copyMenu);
                    pasteMenu.Enabled = dataObject.GetDataPresent(TourStop.ClipboardFormat);
                    contextMenu.Items.Add(pasteMenu);
                    contextMenu.Items.Add(deleteMenu);
                    contextMenu.Show(Cursor.Position);
                }
                else if (tour.CurrentTourStop == null)
                {
                    // Context menu for no selection
                    if (contextMenu != null)
                    {
                        contextMenu.Dispose();
                    }

                    contextMenu = new ContextMenuStrip();
                    ToolStripMenuItem selectAllMenu = new ToolStripMenuItem(Language.GetLocalizedText(1345, "Select All"));
                    ToolStripMenuItem pasteMenu = new ToolStripMenuItem(Language.GetLocalizedText(425, "Paste"));
                    ToolStripSeparator sep1 = new ToolStripSeparator();
                    ToolStripSeparator sep2 = new ToolStripSeparator();
                    ToolStripMenuItem insertSlide = new ToolStripMenuItem(Language.GetLocalizedText(426, "Add New Slide"));

                    pasteMenu.Click += new EventHandler(pasteMenu_Click);
                    selectAllMenu.Click += new EventHandler(selectAllMenu_Click);

                    insertSlide.Click += new EventHandler(AddNewSlide_Click);


                    // todo check for clibboard format first
                    pasteMenu.Enabled = dataObject.GetDataPresent(TourStop.ClipboardFormat);
                    contextMenu.Items.Add(selectAllMenu);
                    contextMenu.Items.Add(sep1);
                    contextMenu.Items.Add(pasteMenu);
                    contextMenu.Items.Add(sep2);
                    contextMenu.Items.Add(insertSlide);
                    contextMenu.Show(Cursor.Position);
                }
                else
                {
                    if (contextMenu != null)
                    {
                        contextMenu.Dispose();
                    }

                    contextMenu = new ContextMenuStrip();
                    ToolStripMenuItem selectAllMenu = new ToolStripMenuItem(Language.GetLocalizedText(1345, "Select All"));

                    ToolStripMenuItem cutMenu = new ToolStripMenuItem(Language.GetLocalizedText(427, "Cut"));
                    ToolStripMenuItem copyMenu = new ToolStripMenuItem(Language.GetLocalizedText(428, "Copy"));
                    ToolStripMenuItem pasteMenu = new ToolStripMenuItem(Language.GetLocalizedText(429, "Paste"));
                    ToolStripMenuItem deleteMenu = new ToolStripMenuItem(Language.GetLocalizedText(167, "Delete"));
                    ToolStripSeparator sep1 = new ToolStripSeparator();
                    ToolStripSeparator sep3 = new ToolStripSeparator();
                    ToolStripSeparator sep4 = new ToolStripSeparator();
                    ToolStripSeparator sep5 = new ToolStripSeparator();
                    ToolStripSeparator sep6 = new ToolStripSeparator();
                    ToolStripSeparator sep7 = new ToolStripSeparator();
                    ToolStripMenuItem insertSlide = new ToolStripMenuItem(Language.GetLocalizedText(431, "Insert New Slide"));
                    ToolStripMenuItem insertDuplicate = new ToolStripMenuItem(Language.GetLocalizedText(627, "Duplicate Slide at End Position"));
                    ToolStripMenuItem insertSlideshow = new ToolStripMenuItem(Language.GetLocalizedText(628, "Merge Tour after slide..."));
                    ToolStripMenuItem playFromHere = new ToolStripMenuItem(Language.GetLocalizedText(432, "Preview Tour From Here"));
                    ToolStripSeparator sep2 = new ToolStripSeparator();
                    ToolStripMenuItem captureThumbnail = new ToolStripMenuItem(Language.GetLocalizedText(433, "Capture New Thumbnail"));
                    ToolStripMenuItem setSkyPosition = new ToolStripMenuItem(Language.GetLocalizedText(434, "Set Start Camera Position"));
                    ToolStripMenuItem setEndSkyPosition = new ToolStripMenuItem(Language.GetLocalizedText(435, "Set End Camera Position"));
                    ToolStripMenuItem showSkyPosition = new ToolStripMenuItem(Language.GetLocalizedText(436, "Show Start Camera Position"));
                    ToolStripMenuItem showEndSkyPosition = new ToolStripMenuItem(Language.GetLocalizedText(437, "Show End Camera Position"));
                    ToolStripMenuItem masterSlide = new ToolStripMenuItem(Language.GetLocalizedText(438, "Master Slide"));
                    ToolStripMenuItem makeTimeline = new ToolStripMenuItem(Language.GetLocalizedText(1346, "Create Timeline"));
                    ToolStripMenuItem showTimeline = new ToolStripMenuItem(Language.GetLocalizedText(1347, "Show Timeline"));
                    string linkString = tour.CurrentTourStop.NextSlide;
                    switch (linkString)
                    {
                        case "":
                        case null:
                        case "Next":

                            //linkString = " (Next Slide)";
                            linkString = " (" + Language.GetLocalizedText(610, "Next Slide") + ")";
                            break;
                        case "Return":
                            //linkString = " (Return to Caller)";
                            linkString = " (" + Language.GetLocalizedText(602, "Return to Caller") + ")";
                            break;
                        default:
                            int index = Tour.GetTourStopIndexByID(linkString);
                            if (index > -1)
                            {
                                if (String.IsNullOrEmpty(tour.TourStops[index].Description))
                                {
                                    linkString = String.Format(" (Slide {0})", index);
                                }
                                else
                                {
                                    linkString = " (" + tour.TourStops[index].Description + ")";
                                }
                            }
                            break;
                    }


                    ToolStripMenuItem setNextSlide = new ToolStripMenuItem(Language.GetLocalizedText(590, "Set Next Slide") + linkString);
                    ToolStripMenuItem trackSpaceTime = new ToolStripMenuItem(Language.GetLocalizedText(439, "Track Date/Time/Location"));
                    ToolStripMenuItem fadeInOverlays = new ToolStripMenuItem(Language.GetLocalizedText(629, "Fade In Slide Elements"));
                    ToolStripMenuItem properties = new ToolStripMenuItem(Language.GetLocalizedText(20, "Properties"));
                    ToolStripMenuItem interpolation = new ToolStripMenuItem(Language.GetLocalizedText(1029, "Animation Tween Type"));

                    ToolStripMenuItem Linear = new ToolStripMenuItem(Language.GetLocalizedText(1030, "Linear"));
                    ToolStripMenuItem Ease = new ToolStripMenuItem(Language.GetLocalizedText(1031, "Ease In/Out"));
                    ToolStripMenuItem EaseIn = new ToolStripMenuItem(Language.GetLocalizedText(1032, "Ease In"));
                    ToolStripMenuItem EaseOut = new ToolStripMenuItem(Language.GetLocalizedText(1033, "Ease Out"));
                    ToolStripMenuItem Exponential = new ToolStripMenuItem(Language.GetLocalizedText(1034, "Exponential"));

                    Linear.Tag = InterpolationType.Linear;
                    Ease.Tag = InterpolationType.EaseInOut;
                    EaseIn.Tag = InterpolationType.EaseIn;
                    EaseOut.Tag = InterpolationType.EaseOut;
                    Exponential.Tag = InterpolationType.Exponential;

                    Linear.Click += new EventHandler(Interpolation_Click);
                    Ease.Click += new EventHandler(Interpolation_Click);
                    EaseIn.Click += new EventHandler(Interpolation_Click);
                    EaseOut.Click += new EventHandler(Interpolation_Click);
                    Exponential.Click += new EventHandler(Interpolation_Click);


                    switch (tour.CurrentTourStop.InterpolationType)
                    {
                        case InterpolationType.Linear:
                            Linear.Checked = true;
                            break;
                        case InterpolationType.EaseIn:
                            EaseIn.Checked = true;
                            break;
                        case InterpolationType.EaseOut:
                            EaseOut.Checked = true;
                            break;
                        case InterpolationType.EaseInOut:
                            Ease.Checked = true;
                            break;
                        case InterpolationType.Exponential:
                            Exponential.Checked = true;
                            break;
                        default:
                            break;
                    }


                    interpolation.DropDownItems.Add(Linear);
                    interpolation.DropDownItems.Add(Ease);
                    interpolation.DropDownItems.Add(EaseIn);
                    interpolation.DropDownItems.Add(EaseOut);
                    interpolation.DropDownItems.Add(Exponential);

                    selectAllMenu.Click += new EventHandler(selectAllMenu_Click);
                    insertDuplicate.Click += new EventHandler(insertDuplicate_Click);
                    cutMenu.Click += new EventHandler(cutMenu_Click);
                    copyMenu.Click += new EventHandler(copyMenu_Click);
                    pasteMenu.Click += new EventHandler(pasteMenu_Click);
                    deleteMenu.Click += new EventHandler(deleteMenu_Click);
                    insertSlide.Click += new EventHandler(InsertNewSlide_Click);
                    properties.Click += new EventHandler(properties_Click);
                    captureThumbnail.Click += new EventHandler(captureThumbnail_Click);
                    setSkyPosition.Click += new EventHandler(setSkyPosition_Click);
                    setEndSkyPosition.Click += new EventHandler(setEndSkyPosition_Click);
                    showEndSkyPosition.Click += new EventHandler(showEndSkyPosition_Click);
                    showSkyPosition.Click += new EventHandler(showSkyPosition_Click);
                    playFromHere.Click += new EventHandler(playFromHere_Click);
                    masterSlide.Click += new EventHandler(masterSlide_Click);
                    setNextSlide.Click += new EventHandler(setNextSlide_Click);
                    trackSpaceTime.Click += new EventHandler(trackSpaceTime_Click);
                    insertSlideshow.Click += new EventHandler(insertSlideshow_Click);
                    fadeInOverlays.Click += new EventHandler(fadeInOverlays_Click);

                    makeTimeline.Click += new EventHandler(makeTimeline_Click);
                    showTimeline.Click += new EventHandler(makeTimeline_Click);

                    if (tour.CurrentTourStop.MasterSlide)
                    {
                        masterSlide.Checked = true;
                    }

                    if (tour.CurrentTourStop.HasTime)
                    {
                        trackSpaceTime.Checked = true;
                    }

                    fadeInOverlays.Checked = tour.CurrentTourStop.FadeInOverlays;

                    contextMenu.Items.Add(selectAllMenu);
                    contextMenu.Items.Add(sep7);
                    contextMenu.Items.Add(cutMenu);
                    contextMenu.Items.Add(copyMenu);
                    pasteMenu.Enabled = dataObject.GetDataPresent(TourStop.ClipboardFormat);
                    contextMenu.Items.Add(pasteMenu);
                    contextMenu.Items.Add(deleteMenu);
                    contextMenu.Items.Add(sep1);
                    contextMenu.Items.Add(insertSlide);
                    contextMenu.Items.Add(insertDuplicate);
                    contextMenu.Items.Add(insertSlideshow);
                    contextMenu.Items.Add(sep2);
                    contextMenu.Items.Add(playFromHere);
                    contextMenu.Items.Add(sep3);
                    contextMenu.Items.Add(setSkyPosition);
                    contextMenu.Items.Add(setEndSkyPosition);
                    contextMenu.Items.Add(sep4);
                    contextMenu.Items.Add(showSkyPosition);
                    contextMenu.Items.Add(showEndSkyPosition);
                    contextMenu.Items.Add(sep5);
                    contextMenu.Items.Add(captureThumbnail);
                    contextMenu.Items.Add(sep6);

                    if (!tour.CurrentTourStop.KeyFramed)
                    {
                        contextMenu.Items.Add(makeTimeline);
                    }
                    else
                    {
                        contextMenu.Items.Add(showTimeline);
                    }

                    contextMenu.Items.Add(masterSlide);
                    contextMenu.Items.Add(setNextSlide);
                    contextMenu.Items.Add(fadeInOverlays);
                    contextMenu.Items.Add(trackSpaceTime);
                    contextMenu.Items.Add(interpolation);
                    // contextMenu.Items.Add(properties);
                    contextMenu.Show(Cursor.Position);
                }
            }
        }

        void makeTimeline_Click(object sender, EventArgs e)
        {
            if (tour.CurrentTourStop != null)
            {
                tour.CurrentTourStop.KeyFramed = true;
                KeyFramer.ShowTimeline();
            }
        }

        void selectAllMenu_Click(object sender, EventArgs e)
        {
            tourStopList.SelectAll();
        }

        void Interpolation_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            tour.CurrentTourStop.InterpolationType = (InterpolationType)item.Tag;
        }

        void setNextSlide_Click(object sender, EventArgs e)
        {
            SelectLink selectDialog = new SelectLink();
            selectDialog.Owner = Earth3d.MainWindow;
            selectDialog.Tour = tour;
            selectDialog.ID = tour.CurrentTourStop.NextSlide;
            if (selectDialog.ShowDialog() == DialogResult.OK)
            {
                tour.CurrentTourStop.NextSlide = selectDialog.ID;
            }
        }

        void insertDuplicate_Click(object sender, EventArgs e)
        {
            Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(530, "Duplicate Slide at End Position"), tour));

            TourStop ts = tour.CurrentTourStop.Copy();

            if (ts == null)
            {
                return;
            }

            if (ts.EndTarget != null)
            {
                ts.EndTarget.BackgroundImageSet = ts.Target.BackgroundImageSet;
                ts.EndTarget.StudyImageset = ts.Target.StudyImageset;
                ts.Target = ts.EndTarget;
                ts.StartTime = ts.EndTime;
                ts.EndTarget = null;
            }

            foreach (Overlay overlay in ts.Overlays)
            {
                overlay.TweenFactor = 1f;
                overlay.Animate = !overlay.Animate;
                overlay.Animate = !overlay.Animate;
            }
            ts.TweenPosition = 0f;
            ts.FadeInOverlays = false;
            tour.InsertAfterTourStop(ts);
            tourStopList.Refresh();

        }

        void fadeInOverlays_Click(object sender, EventArgs e)
        {
            tour.CurrentTourStop.FadeInOverlays = !tour.CurrentTourStop.FadeInOverlays;
        }

        void insertSlideshow_Click(object sender, EventArgs e)
        {
            //todo localize
            Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(531, "Merge Slide Show"), tour));
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Language.GetLocalizedText(101, "WorldWide Telescope Tours") + "|*.wtt";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filename = openFile.FileName;
                try
                {
                    tour.MergeTour(filename);
                }
                catch
                {
                    MessageBox.Show(Language.GetLocalizedText(102, "This file does not seem to be a valid tour"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                }
            }
            tourStopList.Refresh();
        }

        void trackSpaceTime_Click(object sender, EventArgs e)
        {
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(532, "Track Time Edit"), tour));
            tour.CurrentTourStop.HasTime = !tour.CurrentTourStop.HasTime;
        }

        void masterSlide_Click(object sender, EventArgs e)
        {
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(533, "Master Slide State Edit"), tour));
            tour.CurrentTourStop.MasterSlide = !tour.CurrentTourStop.MasterSlide;
            tourStopList.Refresh();
        }

        void playFromHere_Click(object sender, EventArgs e)
        {
            PlayFromCurrentTourstop();
        }

        private void PlayFromCurrentTourstop()
        {
            playing = true;
            Earth3d.MainWindow.GotoTarget(tour.CurrentTourStop.Target, false, Earth3d.NoUi ? false : true, false);
            //tour.CurrentTourStop.SyncSettings();
            SpaceTimeController.Now = tour.CurrentTourStop.StartTime;
            SpaceTimeController.SyncToClock = false;
            SetPlayPauseMode();
        }

        public void PlayFromTourstop(TourStop ts)
        {
            tour.CurrentTourStop = ts;
            PlayFromCurrentTourstop();
        }


        void showSkyPosition_Click(object sender, EventArgs e)
        {
            if (tour.CurrentTourStop != null)
            {
                Earth3d.MainWindow.GotoTarget(tour.CurrentTourStop.Target, false, true, false);
                tour.CurrentTourStop.SyncSettings();
                SpaceTimeController.Now = tour.CurrentTourStop.StartTime;
                SpaceTimeController.SyncToClock = false;
                tour.CurrentTourStop.TweenPosition = 0f;
                LayerManager.SetVisibleLayerList(tour.CurrentTourStop.Layers);
                tourStopList.Refresh();

            }
        }

        void showEndSkyPosition_Click(object sender, EventArgs e)
        {
            tour.CurrentTourStop.TweenPosition = 1f;
            tour.CurrentTourStop.UpdateLayerOpacity();
            if (tour.CurrentTourStop != null && tour.CurrentTourStop.EndTarget != null)
            {
                //Earth3d.MainWindow.GotoTarget(tour.CurrentTourStop.EndTarget, false, false);
                Earth3d.MainWindow.GotoTarget(false, true, tour.CurrentTourStop.EndTarget.CamParams, tour.CurrentTourStop.Target.StudyImageset, tour.CurrentTourStop.Target.BackgroundImageSet);
                Earth3d.MainWindow.SolarSystemTrack = tour.CurrentTourStop.EndTarget.Target;
                SpaceTimeController.Now = tour.CurrentTourStop.EndTime;
                tour.CurrentTourStop.SyncSettings();
                LayerManager.SetVisibleLayerList(tour.CurrentTourStop.Layers);
                SpaceTimeController.SyncToClock = false;
                tourStopList.Refresh();
                TourEditorUI.ClearSelection();
            }
        }

        void setEndSkyPosition_Click(object sender, EventArgs e)
        {
            if (tour.CurrentTourStop != null)
            {
                //todo localize
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(435, "Set End Camera Position"), tour));

                TourPlace newPlace = new TourPlace("End Place", Earth3d.MainWindow.viewCamera, Classification.Unidentified, Earth3d.MainWindow.Constellation, Earth3d.MainWindow.CurrentImageSet.DataSetType, Earth3d.MainWindow.SolarSystemTrack);
                tour.CurrentTourStop.EndTarget = newPlace;
                tour.CurrentTourStop.EndTarget.Constellation = Earth3d.MainWindow.Constellation;
                tour.CurrentTourStop.EndTime = SpaceTimeController.Now;
                tour.CurrentTourStop.SetEndKeyFrames();
                tour.CurrentTourStop.TweenPosition = 1f;
                foreach (LayerInfo info in tour.CurrentTourStop.Layers.Values)
                {
                    if (LayerManager.LayerList.ContainsKey(info.ID))
                    {
                        info.EndOpacity = LayerManager.LayerList[info.ID].Opacity;
                        info.EndParams = LayerManager.LayerList[info.ID].GetParams();
                    }
                }
                tour.CurrentTourStop.UpdateLayerOpacity();

                tourStopList.Refresh();
                TimeLine.RefreshUi();
                TourEditorUI.ClearSelection();
            }
        }

        void setSkyPosition_Click(object sender, EventArgs e)
        {
            if (tour.CurrentTourStop != null)
            {
                //todo localize
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(434, "Set Start Camera Position"), tour));

                tour.CurrentTourStop.Target.Target = Earth3d.MainWindow.SolarSystemTrack;
                tour.CurrentTourStop.Target.Type = Earth3d.MainWindow.CurrentImageSet.DataSetType;
                tour.CurrentTourStop.Target.CamParams = Earth3d.MainWindow.viewCamera;
                tour.CurrentTourStop.Target.Constellation = Earth3d.MainWindow.Constellation;
                tour.CurrentTourStop.Target.StudyImageset = Earth3d.MainWindow.StudyImageset;
                tour.CurrentTourStop.Target.Type = Earth3d.MainWindow.CurrentImageSet.DataSetType;
                tour.CurrentTourStop.Target.BackgroundImageSet = Earth3d.MainWindow.CurrentImageSet.StockImageSet;
                tour.CurrentTourStop.CaptureSettings();
                tour.CurrentTourStop.Layers = LayerManager.GetVisibleLayerList(tour.CurrentTourStop.Layers);
                tour.CurrentTourStop.SetStartKeyFrames();
                tour.CurrentTourStop.TweenPosition = 0f;
                tourStopList.Refresh();
                TimeLine.RefreshUi();
                TourEditorUI.ClearSelection();
            }
        }

        void captureThumbnail_Click(object sender, EventArgs e)
        {
            if (tour.CurrentTourStop != null)
            {
                //TODO undo issues with image bitmaps. Must serialize images
                tour.CurrentTourStop.Thumbnail = Earth3d.MainWindow.GetScreenThumbnail();
                tourStopList.Refresh();
            }
        }

        void properties_Click(object sender, EventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        private void tourStopList_AddNewSlide(object sender, TourStop e)
        {
            AddSlide(false);
            tourStopList.EnsureAddVisible();
        }

        void AddNewSlide_Click(object sender, EventArgs e)
        {
            AddSlide(false);
            tourStopList.EnsureAddVisible();
        }

        void InsertNewSlide_Click(object sender, EventArgs e)
        {
            AddSlide(true);

        }

        private void AddSlide(bool insert)
        {
            //todo localize
            Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(426, "Add New Slide"), tour));

            Cursor.Current = Cursors.WaitCursor;
            string placeName = "Current Screen";
            TourPlace newPlace = new TourPlace(placeName, Earth3d.MainWindow.viewCamera, Classification.Unidentified, Earth3d.MainWindow.Constellation, Earth3d.MainWindow.CurrentImageSet.DataSetType, Earth3d.MainWindow.SolarSystemTrack);
            newPlace.ThumbNail = null;
            newPlace.StudyImageset = Earth3d.MainWindow.StudyImageset;
            newPlace.BackgroundImageSet = Earth3d.MainWindow.CurrentImageSet.StockImageSet;

            TourStop newTourStop = new TourStop(newPlace);


            if (insert)
            {
                tour.InsertTourStop(newTourStop);
            }
            else
            {
                tour.AddTourStop(newTourStop);
            }

            if (tour.CurrentTourStop != null)
            {
                MusicTrack.Target = tour.CurrentTourStop;
                VoiceTrack.Target = tour.CurrentTourStop;
            }
            else
            {
                MusicTrack.Target = null;
                VoiceTrack.Target = null;

            }
            tour.CurrentTourStop.Layers = LayerManager.GetVisibleLayerList(tour.CurrentTourStop.Layers);

            newTourStop.Thumbnail = newPlace.ThumbNail = Earth3d.MainWindow.GetScreenThumbnail();

            tourStopList.SelectedItem = tourStopList.FindItem(newTourStop);
            tourStopList.Refresh();
            TourEditorUI.ClearSelection();
            Cursor.Current = Cursors.Default;
            TimeLine.RefreshUi();
        }

        void deleteMenu_Click(object sender, EventArgs e)
        {
            //todo localize
            Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(534, "Delete Slide"), tour));

            foreach (TourStop item in tourStopList.SelectedItems.Values)
            {
                tour.RemoveTourStop(item);
            }

            tourStopList.SelectedItems.Clear();
            tourStopList.SelectedItem = -1;
            tour.CurrentTourStop = null;


            MusicTrack.Target = null;
            VoiceTrack.Target = null;


            tourStopList.Refresh();
            TourEditorUI.ClearSelection();
        }

        void pasteMenu_Click(object sender, EventArgs e)
        {
            //todo localize

            IDataObject dataObject = Clipboard.GetDataObject();

            if (dataObject.GetDataPresent(TourStop.ClipboardFormat))
            {
                Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(535, "Paste Slide"), tour));
                // add try catch block
                string xml = dataObject.GetData(TourStop.ClipboardFormat) as string;
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                System.Xml.XmlNode node = doc["TourStops"];

                Stack<TourStop> pasteStack = new Stack<TourStop>();

                foreach (System.Xml.XmlNode child in node.ChildNodes)
                {
                    TourStop ts = TourStop.FromXml(tour, child);
                    ts.Id = Guid.NewGuid().ToString();
                    pasteStack.Push(ts);
                }
                tourStopList.SelectedItems.Clear();
                int curIndex = tourStopList.SelectedItem + pasteStack.Count - 1;

                while (pasteStack.Count > 0)
                {
                    TourStop ts = pasteStack.Pop();
                    tour.InsertTourStop(ts);
                    tourStopList.SelectedItems.Add(curIndex--, ts);
                }
                tourStopList.Refresh();
                TourEditorUI.ClearSelection();
            }
        }

        void copyMenu_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter textWriter = new System.IO.StringWriter(sb))
            {
                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(textWriter))
                {
                    writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                    writer.WriteStartElement("TourStops");
                    foreach (TourStop item in tourStopList.SelectedItems.Values)
                    {
                        item.SaveToXml(writer, true);
                    }
                    writer.WriteEndElement();
                }
            }
            DataFormats.Format format = DataFormats.GetFormat(TourStop.ClipboardFormat);

            IDataObject dataObject = new DataObject();
            dataObject.SetData(format.Name, false, sb.ToString());

            Clipboard.SetDataObject(dataObject, false);

        }

        void cutMenu_Click(object sender, EventArgs e)
        {
            //todo localize
            Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(536, "Cut Slide"), tour));

            copyMenu_Click(sender, e);
            foreach (TourStop item in tourStopList.SelectedItems.Values)
            {
                tour.RemoveTourStop(item);
            }

            tourStopList.SelectedItems.Clear();
            tourStopList.Refresh();
            TourEditorUI.ClearSelection();
        }

        //private void tourText_TextChanged(object sender, EventArgs e)
        //{
        //    if (tour.CurrentTourStop != null)
        //    {
        //        tour.CurrentTourStop.Description = tourText.Text;
        //        tourStopList.Refresh();
        //    }
        //}

        public void PauseTour()
        {
            if (playing)
            {
                playing = false;
            }
            SetPlayPauseMode();
        }


        bool playing = false;
        private void Preview_Click(object sender, EventArgs e)
        {
            playing = !playing;
            if (playing && tour.EditMode)
            {
                Tour.CurrentTourstopIndex = -1;
            }
            SetPlayPauseMode();
        }

        TourPlayer player = null;
        private void SetPlayPauseMode()
        {
            if (tour.EditMode)
            {
                if (playing)
                {
                    if (player == null)
                    {
                        player = new TourPlayer();
                    }
                    player.Tour = tour;
                    Earth3d.MainWindow.UiController = player;
                    player.Play();
                    Preview.Image = global::TerraViewer.Properties.Resources.button_pause_normal;
                    Preview.Text = Language.GetLocalizedText(440, "Pause");
                    tourStopList.ShowAddButton = false;
                }
                else
                {
                    Earth3d.MainWindow.UiController = TourEditorUI;

                    Preview.Image = global::TerraViewer.Properties.Resources.button_play_normal;
                    Preview.Text = Language.GetLocalizedText(441, "Play");
                    if (player != null)
                    {
                        player.Stop(false);
                        player.Dispose();
                    }
                    player = null;
                    Earth3d.MainWindow.Mover = null;
                    tourStopList.ShowAddButton = tour.EditMode;
                }
            }
            else
            {
                if (playing)
                {
                    if (player == null)
                    {
                        player = new TourPlayer();
                    }
                    player.Tour = tour;
                    Earth3d.MainWindow.UiController = player;
                    player.Play();
                    Preview.Image = global::TerraViewer.Properties.Resources.button_pause_normal;
                    Preview.Text = Language.GetLocalizedText(440, "Pause");
                    tourStopList.ShowAddButton = false;
                }
                else
                {

                    Earth3d.MainWindow.UiController = null;
                    Earth3d.MainWindow.FreezeView();

                    Preview.Image = global::TerraViewer.Properties.Resources.button_play_normal;
                    Preview.Text = Language.GetLocalizedText(441, "Play");
                    if (player != null)
                    {
                        player.Stop(false);
                        player.Dispose();
                    }
                    player = null;
                    Earth3d.MainWindow.Mover = null;
                    tourStopList.ShowAddButton = tour.EditMode;
                }
            }
        }

        private void PlayerTimer_Tick(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }
            if (playing)
            {
                if (player != null)
                {
                    if (!TourPlayer.Playing)
                    {
                        playing = false;
                        SetPlayPauseMode();
                    }
                    else
                    {
                        if (tourStopList.SelectedItem != tour.CurrentTourstopIndex)
                        {
                            tourStopList.SelectedItem = tour.CurrentTourstopIndex;
                        }
                    }
                }
            }
            TimeSpan ts = Tour.RunTime;
            totalTimeText.Text = String.Format("{0:0}:{1:00}", ts.Minutes, ts.Seconds);

        }

        private void InsertShapeCircle_Click(object sender, EventArgs e)
        {
            TourEditorUI.AddShape("", ShapeType.Circle);

        }

        private void InsertShapeRectangle_Click(object sender, EventArgs e)
        {
            TourEditorUI.AddShape("", ShapeType.Rectagle);

        }

        private void InsertShapeLine_Click(object sender, EventArgs e)
        {
            TourEditorUI.AddShape("", ShapeType.Line);

        }

        private void insertDonut_Click(object sender, EventArgs e)
        {
            TourEditorUI.AddShape("", ShapeType.Donut);

        }


        void AddArrow_Click(object sender, EventArgs e)
        {
            TourEditorUI.AddShape("", ShapeType.Arrow);

        }

        private void InsertVideo_Click(object sender, EventArgs e)
        {

        }

        private void InsertAudio_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = Language.GetLocalizedText(442, "Sound/Music") + "(*.MP3;*.WMA)|*.MP3;*.WMA";

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = fileDialog.FileName;
                TourEditorUI.AddAudio(filename);
            }
        }

        private void InsertHyperlink_Click(object sender, EventArgs e)
        {

        }
        Color defultColor = Color.White;

        private void ColorPicker_Click(object sender, EventArgs e)
        {
            PopupColorPicker picker = new PopupColorPicker();

            picker.Location = Cursor.Position;

            picker.Color = TourEditorUI.GetCurrentColor();

            if (picker.ShowDialog() == DialogResult.OK)
            {
                TourEditorUI.SetCurrentColor(picker.Color);
            }
        }

        private void toolkit_MouseEnter(object sender, EventArgs e)
        {
            if (Earth3d.MainWindow.IsWindowOrChildFocused())
            {
                this.Focus();
            }
        }

        private void TourEditTab_Leave(object sender, EventArgs e)
        {
            
        }

        private void EditTourProperties_Click(object sender, EventArgs e)
        {

            TourProperties tourProps = new TourProperties();
            tourProps.EditTour = tour;
            if (tourProps.ShowDialog() == DialogResult.OK)
            {
                Earth3d.MainWindow.Refresh();
            }
        }

        private void SaveTour_Click(object sender, EventArgs e)
        {
            //tour.SaveToXml(false);

            Save(false);

        }

        public bool Save(bool saveAs)
        {
            if (string.IsNullOrEmpty(tour.SaveFileName) || saveAs)
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = Language.GetLocalizedText(101, "WorldWide Telescope Tours") + "|*.wtt";
                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveDialog.AddExtension = true;
                saveDialog.DefaultExt = ".WTT";
                saveDialog.FileName = tour.SaveFileName;
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    tour.SaveToFile(saveDialog.FileName);
                }
                else
                {
                    return false;
                }
                return true;
            }
            else
            {
                tour.SaveToFile(tour.SaveFileName);
            }
            return true;
        }

        private void AddVideo_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = fileDialog.FileName;
                TourEditorUI.AddVideo(filename);
            }
        }

        private void AddPicture_Click(object sender, EventArgs e)
        {
            if (tour == null || Tour.CurrentTourStop == null)
            {
                return;
            }


            bool flipBook = false;
            if (Control.ModifierKeys == (Keys.Control | Keys.Shift))
            {
                flipBook = true;
            }

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = Language.GetLocalizedText(443, "Images") + "(*.JPG;*.PNG;*.TIF;*.TIFF;*.FITS;*.FIT)|*.JPG;*.PNG;*.TIF;*.TIFF;*.FITS;*.FIT";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string filename = fileDialog.FileName;
                if (flipBook)
                {
                    TourEditorUI.AddFlipbook(filename);
                }
                else
                {
                    TourEditorUI.AddPicture(filename);
                }
            }
        }

        private void AddShape_Click(object sender, EventArgs e)
        {
            if (tour.CurrentTourStop != null)
            {
                // Context menu for no selection
                if (contextMenu != null)
                {
                    contextMenu.Dispose();
                }

                contextMenu = new ContextMenuStrip();

                ToolStripMenuItem AddCircle = new ToolStripMenuItem(Language.GetLocalizedText(444, "Circle"));
                ToolStripMenuItem AddRectangle = new ToolStripMenuItem(Language.GetLocalizedText(445, "Rectangle"));
                ToolStripMenuItem AddOpenRectangle = new ToolStripMenuItem(Language.GetLocalizedText(446, "Open Rectangle"));
                ToolStripMenuItem AddRing = new ToolStripMenuItem(Language.GetLocalizedText(447, "Ring"));
                ToolStripMenuItem AddLine = new ToolStripMenuItem(Language.GetLocalizedText(448, "Line"));
                ToolStripMenuItem AddArrow = new ToolStripMenuItem(Language.GetLocalizedText(449, "Arrow"));
                ToolStripMenuItem AddStar = new ToolStripMenuItem(Language.GetLocalizedText(450, "Star"));

                AddCircle.Click += new EventHandler(InsertShapeCircle_Click);
                AddRectangle.Click += new EventHandler(InsertShapeRectangle_Click);
                AddOpenRectangle.Click += new EventHandler(AddOpenRectangle_Click);
                AddRing.Click += new EventHandler(insertDonut_Click);
                AddLine.Click += new EventHandler(InsertShapeLine_Click);
                AddArrow.Click += new EventHandler(AddArrow_Click);
                AddStar.Click += new EventHandler(AddStar_Click);


                contextMenu.Items.Add(AddCircle);
                contextMenu.Items.Add(AddRectangle);
                contextMenu.Items.Add(AddOpenRectangle);
                contextMenu.Items.Add(AddRing);
                contextMenu.Items.Add(AddLine);
                contextMenu.Items.Add(AddArrow);
                contextMenu.Items.Add(AddStar);
                contextMenu.Show(AddShape.PointToScreen(new Point(0, AddShape.Height)));
            }
        }

        void AddOpenRectangle_Click(object sender, EventArgs e)
        {
            TourEditorUI.AddShape("", ShapeType.OpenRectagle);
        }

        void AddStar_Click(object sender, EventArgs e)
        {
            TourEditorUI.AddShape("", ShapeType.Star);

        }


        private void AddText_Click(object sender, EventArgs e)
        {
            if (tour != null && Tour.CurrentTourStop != null)
            {

                TextEditor textEdit = new TextEditor();
                if (textEdit.ShowDialog() == DialogResult.OK)
                {
                    TourEditorUI.AddText(textEdit.Text, textEdit.TextObject);
                }
            }

        }

        private void Preview_EnabledChanged(object sender, EventArgs e)
        {
            if (playing)
            {
                //todo change image based on enable/disable state
            }
            else
            {
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
        Bitmap menuArrow = global::TerraViewer.Properties.Resources.menuArrow;

        private void AddShape_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(menuArrow, new Point((AddShape.Width / 2) - (menuArrow.Width / 2), (AddShape.Height - menuArrow.Height) - 2));
        }

        private void tourStopList_ItemHover(object sender, TourStop e)
        {
            if (e != null)
            {
                toolTip.SetToolTip(tourStopList, e.Description);
            }
            else
            {
                toolTip.SetToolTip(tourStopList, Language.GetLocalizedText(451, "Slide List"));
            }

        }

        private void TourEditTab_FormClosed(object sender, FormClosedEventArgs e)
        {
            TourPlayer.Playing = false;
            TourPlayer.TourEnded -= new EventHandler(TourPlayer_TourEnded);
        }

        private void EditTourProperties_Load(object sender, EventArgs e)
        {

        }

        private void ShowSafeArea_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowSafeArea = ShowSafeArea.Checked;
        }

        public void UndoStep()
        {
            if (Undo.PeekAction())
            {
                Undo.StepBack();
                tourStopList.Refresh();
                tourStopList.SelectedItem = tour.CurrentTourstopIndex;
                ShowSlideStartPosition(tour.CurrentTourStop);
                this.Refresh();
                OverlayList.UpdateOverlayList(tour.CurrentTourStop, TourEditorUI.Selection);
            }
        }

        internal void RedoStep()
        {
            if (Undo.PeekRedoAction())
            {
                Undo.StepForward();
                tourStopList.Refresh();
                tourStopList.SelectedItem = tour.CurrentTourstopIndex;
                ShowSlideStartPosition(tour.CurrentTourStop);
                this.Refresh();
                OverlayList.UpdateOverlayList(tour.CurrentTourStop, TourEditorUI.Selection);
            }
        }

        private void tourStopList_ShowEndPosition(object sender, TourStop e)
        {
            showEndSkyPosition_Click(this, new EventArgs());
        }

        private void tourStopList_ShowStartPosition(object sender, TourStop e)
        {
            ShowSlideStartPosition(Tour.CurrentTourStop);
            TourEditorUI.ClearSelection();
        }

        private void Dome_CheckedChanged(object sender, EventArgs e)
        {
            Undo.Push(new UndoTourPropertiesChange(Language.GetLocalizedText(549, "Properties Edit"), Tour));
            Tour.DomeMode = Dome.Checked;
            Overlay.DefaultAnchor = Tour.DomeMode ? OverlayAnchor.Dome : OverlayAnchor.Screen;
        }

        private void tourStopList_KeyDown(object sender, KeyEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.C:
                        copyMenu_Click(null, new EventArgs());
                        break;
                    case Keys.V:
                        pasteMenu_Click(null, new EventArgs());

                        break;
                    case Keys.X:
                        cutMenu_Click(null, new EventArgs());
                        break;

                    case Keys.Z:
                        if (Undo.PeekAction())
                        {
                            Earth3d.MainWindow.TourEdit.UndoStep();
                        }
                        else
                        {
                            UiTools.Beep();
                        }
                        break;
                    case Keys.Y:

                        if (Undo.PeekRedoAction())
                        {
                            Earth3d.MainWindow.TourEdit.RedoStep();
                        }
                        else
                        {
                            UiTools.Beep();
                        }

                        break;
                }
            }

            if (e.KeyCode == Keys.Delete)
            {
                deleteMenu_Click(null, new EventArgs());
            }
        }

        internal void EnsureSelectedVisible()
        {
            tourStopList.EnsureSelectedVisible();
        }
    }
}
