
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Text;

namespace TerraViewer
{

    public interface IUiController
    {
        void Render(RenderEngine renderEngine);
        void PreRender(RenderEngine renderEngine);
        bool MouseDown(object sender, System.Windows.Forms.MouseEventArgs e);
        bool MouseUp(object sender, System.Windows.Forms.MouseEventArgs e);
        bool MouseMove(object sender, System.Windows.Forms.MouseEventArgs e);
        bool MouseClick(object sender, System.Windows.Forms.MouseEventArgs e);
        bool Click(object sender, EventArgs e);
        bool MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e);
        bool KeyDown(object sender, System.Windows.Forms.KeyEventArgs e);
        bool KeyUp(object sender, System.Windows.Forms.KeyEventArgs e);
        bool Hover(Point pnt);
    }

    public class TourEditor : IUiController , IDisposable
    {
        public TourEditor()
        {
        }

        Selection selection = new Selection();

        public Selection Selection
        {
            get { return selection; }
            set { selection = value; }
        }
        
        ContextMenuStrip contextMenu = new ContextMenuStrip();

        public static bool Capturing = false;

        public static bool Scrubbing = false;
        public static bool ScrubbingBackwards = false;
        public static DateTime ScrubStartTime = DateTime.Now;

        public static IUiController CurrentEditor = null;

        public void PreRender(RenderEngine renderEngine)
        {
            if (tour.CurrentTourStop != null)
            {
                if (Scrubbing)
                {
                    Settings.TourSettings = tour.CurrentTourStop;
                    TimeSpan slideElapsedTime = SpaceTimeController.MetaNow - ScrubStartTime;

                    if (ScrubbingBackwards)
                    {
                        if (slideElapsedTime > tour.CurrentTourStop.Duration)
                        {
                            Scrubbing = false;
                            tour.CurrentTourStop.TweenPosition = 0.0f;
                        }
                        else
                        {
                            tour.CurrentTourStop.TweenPosition = Math.Min(1, 1 - (float)(slideElapsedTime.TotalMilliseconds / tour.CurrentTourStop.Duration.TotalMilliseconds));
                            TimeLine.RefreshUi();
                        }
                    }
                    else
                    {
                        if (slideElapsedTime > tour.CurrentTourStop.Duration)
                        {
                            Scrubbing = false;
                            tour.CurrentTourStop.TweenPosition = 1.0f;
                        }
                        else
                        {
                            tour.CurrentTourStop.TweenPosition = (float)(slideElapsedTime.TotalMilliseconds / tour.CurrentTourStop.Duration.TotalMilliseconds);
                            TimeLine.RefreshUi();
                        }
                    }
                }
                else
                {
                    if (CurrentEditor != null)
                    {
                        CurrentEditor.PreRender(renderEngine);
                    }
                }
            }
        }

        public void StartScrubbing(bool reverse)
        {
            if (tour.CurrentTourStop != null)
            {
                Scrubbing = true;
                ScrubbingBackwards = reverse;
                
                if (ScrubbingBackwards)
                {
                    if (tour.CurrentTourStop.TweenPosition == 0)
                    {
                        tour.CurrentTourStop.TweenPosition = 1.0f;
                    }
                    ScrubStartTime = SpaceTimeController.MetaNow - TimeSpan.FromSeconds(tour.CurrentTourStop.Duration.TotalSeconds * (1 - tour.CurrentTourStop.TweenPosition));
                }
                else
                {
                    if (tour.CurrentTourStop.TweenPosition == 1.0f)
                    {
                        tour.CurrentTourStop.TweenPosition = 0;
                    }
                    ScrubStartTime = SpaceTimeController.MetaNow - TimeSpan.FromSeconds(tour.CurrentTourStop.Duration.TotalSeconds * (tour.CurrentTourStop.TweenPosition));
                }
            }
        }


        internal void PauseScrubbing(bool p)
        {
            if (Scrubbing)
            {
                Scrubbing = false;
            }
            else
            {
                StartScrubbing(ScrubbingBackwards);
            }
        }

        public void Render(RenderEngine renderEngine)
        {
            renderEngine.SetupMatricesOverlays();
            renderEngine.RenderContext11.DepthStencilMode = DepthStencilMode.Off;


            if (tour == null || tour.CurrentTourStop == null)
            {
                if (Properties.Settings.Default.ShowSafeArea && tour != null && tour.EditMode && !Capturing)
                {
                    DrawSafeZone(renderEngine);
                }
                return;
            }

            foreach (Overlay overlay in tour.CurrentTourStop.Overlays)
            {
                if (overlay.Animate && Tour.CurrentTourStop.KeyFramed)
                {
                    overlay.TweenFactor = tour.CurrentTourStop.TweenPosition;
                }
                else if (!Tour.CurrentTourStop.KeyFramed)
                {
                    overlay.TweenFactor = tour.CurrentTourStop.TweenPosition < .5f ? 0f : 1f;
                }
                overlay.Draw3D(renderEngine.RenderContext11, 1.0f, true);
            }

            if (Properties.Settings.Default.ShowSafeArea && tour != null && tour.EditMode && !Capturing)
            {
                DrawSafeZone(renderEngine);
            }
            selection.Draw3D(renderEngine.RenderContext11, 1.0f);

            if (!Scrubbing)
            {
                if (CurrentEditor != null)
                {
                    CurrentEditor.Render(renderEngine);
                }

                Settings.TourSettings = null;
            }
        }
        
        private void DrawSafeZone(RenderEngine renderEngine)
        {
            Rectangle rect = new Rectangle(0, 0, renderEngine.RenderContext11.Width, renderEngine.RenderContext11.Height);

            int x = rect.Width / 2;
            int y = rect.Height / 2;

            int ratioWidth = rect.Height* 4 /3;
            int halfWidth = (rect.Width - ratioWidth)/2;
            

            DrawTranparentBox(renderEngine.RenderContext11, new Rectangle(-x, -y, halfWidth, rect.Height));
            DrawTranparentBox(renderEngine.RenderContext11, new Rectangle((rect.Width - halfWidth)-x, -y, halfWidth, rect.Height));
        }

        PositionColoredTextured[] boxPoints = new PositionColoredTextured[4];
        private void DrawTranparentBox(RenderContext11 renderContext, Rectangle rect)
        {

            
            Color Color = Color.FromArgb(128, 32, 32, 128);
            boxPoints[0].X = rect.X;
            boxPoints[0].Y = rect.Y;
            boxPoints[0].Z = .9f;
            boxPoints[0].W = 1;
            boxPoints[0].Tu = 0;
            boxPoints[0].Tv = 0;
            boxPoints[0].Color = Color;

            boxPoints[1].X = (float)(rect.X + (rect.Width));
            boxPoints[1].Y = (float)(rect.Y);
            boxPoints[1].Tu = 1;
            boxPoints[1].Tv = 0;
            boxPoints[1].Color = Color;
            boxPoints[1].Z = .9f;
            boxPoints[1].W = 1;

            boxPoints[2].X = (float)(rect.X );
            boxPoints[2].Y = (float)(rect.Y + (rect.Height ));
            boxPoints[2].Tu = 0;
            boxPoints[2].Tv = 1;
            boxPoints[2].Color = Color;
            boxPoints[2].Z = .9f;
            boxPoints[2].W = 1;

            boxPoints[3].X = (float)(rect.X + (rect.Width ));
            boxPoints[3].Y = (float)(rect.Y + (rect.Height ));
            boxPoints[3].Tu = 1;
            boxPoints[3].Tv = 1;
            boxPoints[3].Color = Color;
            boxPoints[3].Z = .9f;
            boxPoints[3].W = 1;

            SharpDX.Matrix mat = SharpDX.Matrix.OrthoLH(renderContext.ViewPort.Width, renderContext.ViewPort.Height, 1, -1);

            Sprite2d.Draw(renderContext, boxPoints, 4, mat, true);

        }
 
        TourDocument tour = null;

        public TourDocument Tour
        {
            get { return tour; }
            set { tour = value; }
        }

        public void Close()
        {
            if (tour != null)
            {
                // todo check for changes
                tour = null;
                Focus = null;
            }
        }

        public void ClearSelection()
        {
            selection.ClearSelection();
            OverlayList.UpdateOverlayListSelection(selection);
            Focus = null;
        }

        bool mouseDown = false;

        

        public Overlay Focus
        {
            get { return selection.Focus; }
            set { selection.Focus = value; }
        }

        PointF pointDown;


        public PointF PointToView(Point pnt)
        {
            float clientHeight = Earth3d.MainWindow.RenderWindow.ClientRectangle.Height;
            float clientWidth = Earth3d.MainWindow.RenderWindow.ClientRectangle.Width;
            float viewWidth = ((float)Earth3d.MainWindow.RenderWindow.ClientRectangle.Width / (float)Earth3d.MainWindow.RenderWindow.ClientRectangle.Height) * 1116f;
            float x = (((float)pnt.X) / ((float)clientWidth) * viewWidth)- ((viewWidth - 1920) / 2);
            float y = ((float)pnt.Y) / clientHeight * 1116;

            return new PointF(x, y);
        }

        SelectionAnchor selectionAction = SelectionAnchor.None;
        bool needUndoFrame = false;
        public bool MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            brokeThreshold = false;
            needUndoFrame = true;
            PointF location = PointToView(e.Location);

            if (tour == null || tour.CurrentTourStop == null)
            {
                needUndoFrame = false;
                return false;
            }

            if (CurrentEditor != null)
            {
                if (CurrentEditor.MouseDown(sender, e))
                {
                    return true;
                }
            }


//            if (e.Button == MouseButtons.Left)
            {
                if (Focus != null)
                {
                    if (selection.MultiSelect)
                    {
                        foreach (Overlay overlay in selection.SelectionSet)
                        {
                            if (overlay.HitTest(location))
                            {
                                selectionAction = SelectionAnchor.Move;
                                mouseDown = true;
                                pointDown = location;
                                Focus = overlay;
                                if (Control.ModifierKeys == Keys.Control)
                                {
                                    dragCopying = true;
                                }

                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (Focus.HitTest(location))
                        {
                            selectionAction = SelectionAnchor.Move;
                            mouseDown = true;
                            pointDown = location;

                            if (Control.ModifierKeys == Keys.Control)
                            {
                                dragCopying = true;
                            }

                            return true;
                        }
                    }

                    SelectionAnchor hit = selection.HitTest(location);
                    if (hit != SelectionAnchor.None)
                    {
                        selectionAction = hit;
                        mouseDown = true;
                        if (hit == SelectionAnchor.Rotate)
                        {
                            pointDown = location;
                        }
                        else
                        {
                            pointDown = selection.PointToSelectionSpace(location);
                        }
                        return true;
                    }

                }

                for (int i = tour.CurrentTourStop.Overlays.Count - 1; i >= 0; i--)
                {
                    if (tour.CurrentTourStop.Overlays[i].HitTest(location))
                    {
                        selectionAction = SelectionAnchor.Move;
                        Focus = tour.CurrentTourStop.Overlays[i];
                        if (Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Shift)
                        {
                            selection.AddSelection(Focus);
                        }
                        else 
                        {
                            selection.SetSelection(Focus);
                        }
                        OverlayList.UpdateOverlayListSelection(selection);
                        mouseDown = true;
                        pointDown = location;
                        return true;
                    }
                }
                Focus = null;
                ClearSelection();

            }
            needUndoFrame = false;
            return false;

        }
        Point contextPoint = new Point();
        public bool MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            brokeThreshold = false;
            if (CurrentEditor != null)
            {
                if (CurrentEditor.MouseUp(sender, e))
                {
                    return true;
                }
            }

            contextPoint = e.Location;
            if (mouseDown)
            {
                mouseDown = false;
                if (e.Button == MouseButtons.Right)
                {
                    if (Focus != null)
                    {
                        
                        ShowSelectionContextMenu();

                    }
                }  
                return true;
            }
            if (e.Button == MouseButtons.Right)
            {
                if (Focus == null)
                {
                    ShowNoSelectionContextMenu();
                }
                return true;
            }  
            return false;
        }
        bool dragCopying = false;
        bool brokeThreshold = false;
        public bool MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.MouseMove(sender, e))
                {
                    return true;
                }
            }


            PointF location = PointToView(e.Location);

            if (mouseDown && Focus != null)
            {
                UndoTourStopChange undoFrame = null;
                //todo localize
                string actionText = Language.GetLocalizedText(502, "Edit");
                if (needUndoFrame)
                {
                    undoFrame = new UndoTourStopChange(Language.GetLocalizedText(502, "Edit"), tour);
                }

                float moveX;
                float moveY;
                if (selectionAction != SelectionAnchor.Move && selectionAction != SelectionAnchor.Rotate)
                {
                    PointF newPoint = selection.PointToSelectionSpace(location);
                    moveX = newPoint.X - pointDown.X;
                    moveY = newPoint.Y - pointDown.Y;
                    pointDown = newPoint;
                }
                else
                {
                    moveX = location.X - pointDown.X;
                    moveY = location.Y - pointDown.Y; 
                    if (selectionAction == SelectionAnchor.Move && !brokeThreshold)
                    {
                        if (Math.Abs(moveX) > 3 || Math.Abs(moveY) > 3)
                        {
                            brokeThreshold = true;
                        }
                        else
                        {
                            return true;
                        }
                    }


                    pointDown = location;

                }
                
                if (dragCopying)
                {

                    if (selection.MultiSelect)
                    {
                        Overlay[] set = selection.SelectionSet;

                        ClearSelection();
                        foreach (Overlay overlay in set)
                        {
                            Overlay newOverlay = AddOverlay(overlay);
                            newOverlay.X = overlay.X;
                            newOverlay.Y = overlay.Y;
                            Focus = newOverlay;
                            selection.AddSelection(Focus);
                           
                        }
                        OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
                        dragCopying = false;
                    }
                    else
                    {
                        Overlay newOverlay = AddOverlay(Focus);
                        newOverlay.X = Focus.X;
                        newOverlay.Y = Focus.Y;
                        Focus = newOverlay;
                        selection.SetSelection(Focus);
                        OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
                        dragCopying = false;
                    }
                }

                float aspect = Focus.Width / Focus.Height;

                PointF center = new PointF(Focus.X, Focus.Y);
                if (Control.ModifierKeys == Keys.Control)
                {
                    actionText = Language.GetLocalizedText(537, "Resize");
                    switch (selectionAction)
                    {
                        case SelectionAnchor.TopLeft:
                            Focus.Width = Math.Max(2,Focus.Width-moveX*2);
                            Focus.Height = Math.Max(2, Focus.Height - (moveX / aspect) * 2);
                            break;
                        case SelectionAnchor.Top:
                            Focus.Height = Math.Max(2,Focus.Height-moveY*2);
                            break;
                        case SelectionAnchor.TopRight:
                            Focus.Width = Math.Max(2,Focus.Width+moveX * 2);
                            Focus.Height = Math.Max(2, Focus.Height + (moveX / aspect) * 2);
                            break;
                        case SelectionAnchor.Right:
                            Focus.Width = Math.Max(2,Focus.Width+moveX * 2);
                            break;
                        case SelectionAnchor.BottomRight:
                            Focus.Width = Math.Max(2,Focus.Width+moveX * 2);
                            Focus.Height = Math.Max(2, Focus.Height + (moveX / aspect) * 2);
                            break;
                        case SelectionAnchor.Bottom:
                            Focus.Height = Math.Max(2,Focus.Height+moveY * 2);
                            break;
                        case SelectionAnchor.BottomLeft:
                            Focus.Width = Math.Max(2,Focus.Width-moveX*2);
                            Focus.Height = Math.Max(2, Focus.Height - (moveX / aspect) * 2);
                            break;
                        case SelectionAnchor.Left:
                            Focus.Width = Math.Max(2,Focus.Width - moveX * 2);
                            break;
                        case SelectionAnchor.Rotate:
                            actionText = Language.GetLocalizedText(538, "Rotate");
                            Focus.RotationAngle = Focus.RotationAngle + moveX/10;
                            break;
                        case SelectionAnchor.Move:
                            actionText = Language.GetLocalizedText(539, "Drag Copy");
                            center.X += moveX;
                            center.Y += moveY;
                            break;
                        case SelectionAnchor.Center:
                            break;
                        case SelectionAnchor.None:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    if (selectionAction != SelectionAnchor.Rotate && selectionAction != SelectionAnchor.Move)
                    {
                        if (moveX > (Focus.Width-2))
                        {
                            moveX = 0;
                        }

                        if (moveY > (Focus.Height-2))
                        {
                            moveY = 0;
                        }
                    }

                    //todo localize
                    actionText = Language.GetLocalizedText(537, "Resize");
                    switch (selectionAction)
                    {
                        case SelectionAnchor.TopLeft:
                            Focus.Width -= moveX;
                            Focus.Height -= (moveX / aspect);
                            center.X += (moveX / 2);
                            center.Y += ((moveX / aspect) / 2);
                            break;
                        case SelectionAnchor.Top:
                            Focus.Height -= moveY;
                            center.Y += (moveY / 2);
                            break;
                        case SelectionAnchor.TopRight:
                            Focus.Width += moveX;
                            Focus.Height += (moveX / aspect);
                            center.X += (moveX / 2);
                            center.Y -= ((moveX / aspect) / 2);
                            break;
                        case SelectionAnchor.Right:
                            Focus.Width += moveX;
                            center.X += (moveX / 2);
                            break;
                        case SelectionAnchor.BottomRight:
                            Focus.Width += moveX;
                            Focus.Height += (moveX / aspect);
                            center.X += (moveX / 2);
                            center.Y += ((moveX / aspect) / 2);
                            break;
                        case SelectionAnchor.Bottom:
                            Focus.Height += moveY;
                            center.Y += (moveY / 2);
                            break;
                        case SelectionAnchor.BottomLeft:
                            Focus.Width -= moveX;
                            Focus.Height -= (moveX/aspect);
                            center.X += (moveX / 2);
                            center.Y -= ((moveX/aspect) / 2);
                            break;
                        case SelectionAnchor.Left:
                            Focus.Width -= moveX;
                            center.X += (moveX / 2);
                            break;
                        case SelectionAnchor.Rotate:
                            actionText = Language.GetLocalizedText(538, "Rotate");
                            Focus.RotationAngle = Focus.RotationAngle + moveX;
                            break;
                        case SelectionAnchor.Move:
                            actionText = Language.GetLocalizedText(540, "Move");
                            center.X += moveX;
                            center.Y += moveY;
                            break;
                        case SelectionAnchor.Center:
                            break;
                        case SelectionAnchor.None:
                            break;
                        default:
                            break;
                    }
                }


                if (selectionAction != SelectionAnchor.Move && selectionAction != SelectionAnchor.Rotate)
                {
                    center = selection.PointToScreenSpace(center);
                }
                if (selection.MultiSelect)
                {
                    foreach (Overlay overlay in selection.SelectionSet)
                    {
                        overlay.X += moveX;
                        overlay.Y += moveY;
                    }
                }
                else
                {
                    Focus.X = center.X;
                    Focus.Y = center.Y;
                }
                if (needUndoFrame)
                {
                    needUndoFrame = false;
                    undoFrame.ActionText = actionText;
                    Undo.Push(undoFrame);
                }
            }
            else
            {
                if (Focus != null)
                {
                    if ( Focus.HitTest(location))
                    {
                        Cursor.Current = Cursors.SizeAll;
                        return false;
                    }

                    SelectionAnchor hit = selection.HitTest(location);
                    if (hit == SelectionAnchor.None)
                    {
                        return false;
                    }
                    switch (hit)
                    {
                        case SelectionAnchor.TopLeft:
                            Cursor.Current = Cursors.SizeNWSE;
                            break;
                        case SelectionAnchor.Top:
                            Cursor.Current = Cursors.SizeNS;
                            break;
                        case SelectionAnchor.TopRight:
                            Cursor.Current = Cursors.SizeNESW;
                            break;
                        case SelectionAnchor.Right:
                            Cursor.Current = Cursors.SizeWE;
                            break;
                        case SelectionAnchor.BottomRight:
                            Cursor.Current = Cursors.SizeNWSE;
                            break;
                        case SelectionAnchor.Bottom:
                            Cursor.Current = Cursors.SizeNS;
                            break;
                        case SelectionAnchor.BottomLeft:
                            Cursor.Current = Cursors.SizeNESW;
                            break;
                        case SelectionAnchor.Left:
                            Cursor.Current = Cursors.SizeWE;
                            break;
                        case SelectionAnchor.Rotate:
                            Cursor.Current = Cursors.SizeWE;
                            break;
                        case SelectionAnchor.Center:
                            break;
                        case SelectionAnchor.None:
                            break;
                        default:
                            break;
                    }

                }
            }
            return false;
        }
  
        private void ShowNoSelectionContextMenu()
        {
            if (contextMenu != null)
            {
                contextMenu.Dispose();
            }
            if (tour.CurrentTourStop == null)
            {
                return;
            }

            contextMenu = new ContextMenuStrip();

            ToolStripMenuItem pasteMenu = new ToolStripMenuItem(Language.GetLocalizedText(425, "Paste"));
            IDataObject data = Clipboard.GetDataObject();

            pasteMenu.Enabled = Clipboard.ContainsImage() | Clipboard.ContainsText() | Clipboard.ContainsAudio() | data.GetDataPresent(Overlay.ClipboardFormat);

            pasteMenu.Click +=new EventHandler(pasteMenu_Click);
            contextMenu.Items.Add(pasteMenu);
            contextMenu.Show(Cursor.Position);
        }



        public void ShowSelectionContextMenu()
        {
            if (Focus == null)
            {
                return;
            }

            bool multiSelect = selection.MultiSelect;

            if (contextMenu != null)
            {
                contextMenu.Dispose();
            }

            contextMenu = new ContextMenuStrip();

            ToolStripMenuItem cutMenu = new ToolStripMenuItem(Language.GetLocalizedText(427, "Cut"));
            ToolStripMenuItem copyMenu = new ToolStripMenuItem(Language.GetLocalizedText(428, "Copy"));
            ToolStripMenuItem pasteMenu = new ToolStripMenuItem(Language.GetLocalizedText(425, "Paste"));
            ToolStripMenuItem deleteMenu = new ToolStripMenuItem(Language.GetLocalizedText(167, "Delete"));
            ToolStripSeparator sep1 = new ToolStripSeparator();
            ToolStripSeparator sep2 = new ToolStripSeparator();
            ToolStripSeparator sep3 = new ToolStripSeparator();
            ToolStripMenuItem bringToFront = new ToolStripMenuItem(Language.GetLocalizedText(452, "Bring to Front"));
            ToolStripMenuItem sendToBack = new ToolStripMenuItem(Language.GetLocalizedText(453, "Send to Back"));
            ToolStripMenuItem bringForward = new ToolStripMenuItem(Language.GetLocalizedText(454, "Bring Forward"));
            ToolStripMenuItem sendBackward = new ToolStripMenuItem(Language.GetLocalizedText(455, "Send Backward"));
            ToolStripMenuItem properties = new ToolStripMenuItem(Language.GetLocalizedText(20, "Properties"));
            ToolStripMenuItem editText = new ToolStripMenuItem(Language.GetLocalizedText(502, "Edit"));
            ToolStripMenuItem fullDome = new ToolStripMenuItem(Language.GetLocalizedText(574, "Full Dome"));
            ToolStripMenuItem url = new ToolStripMenuItem(Language.GetLocalizedText(587, "Hyperlink"));
            string linkString = Focus.LinkID;
            switch (Focus.LinkID)
            {
                case "":
                case null:
                    //linkString = " (No Link)";
                    linkString = " (" + Language.GetLocalizedText(609, "No Link") + ")";
                    break;
                case "Next":
                    //linkString = " (Next Slide)";
                    linkString = " (" + Language.GetLocalizedText(610, "Next Slide") + ")";
                    break;
                case "Return":
                    //linkString = " (Return to Caller)";
                    linkString = " (" + Language.GetLocalizedText(602, "Return to Caller") + ")";
                    break;
                default:
                    int index = Tour.GetTourStopIndexByID(Focus.LinkID);
                    if (index > -1)
                    {
                        if (String.IsNullOrEmpty(tour.TourStops[index].Description))
                        {
                            linkString = String.Format(" (" + Language.GetLocalizedText(1340, "Slide") + " {0})", index);
                        }
                        else
                        {
                            linkString = " (" + tour.TourStops[index].Description + ")";
                        }
                    }
                    break;
            }

            ToolStripMenuItem animateMenu = new ToolStripMenuItem(Language.GetLocalizedText(588, "Animate"));
            ToolStripMenuItem addKeyframes = new ToolStripMenuItem(Language.GetLocalizedText(1280, "Add Keyframe"));
            ToolStripMenuItem addToTimeline = new ToolStripMenuItem(Language.GetLocalizedText(1290, "Add to Timeline"));

            ToolStripMenuItem linkID = new ToolStripMenuItem(Language.GetLocalizedText(589, "Link to Slide") + linkString);
            ToolStripMenuItem pickColor = new ToolStripMenuItem(Language.GetLocalizedText(458, "Color/Opacity"));
            ToolStripMenuItem addAction = new ToolStripMenuItem(Language.GetLocalizedText(1378, "Add Quiz Action"));
            ToolStripMenuItem flipbookProperties = new ToolStripMenuItem(Language.GetLocalizedText(630, "Flipbook Properties"));
            ToolStripMenuItem interpolateMenu = new ToolStripMenuItem(Language.GetLocalizedText(1029, "Animation Tween Type"));

            ToolStripMenuItem Linear = new ToolStripMenuItem(Language.GetLocalizedText(1030, "Linear"));
            ToolStripMenuItem Ease = new ToolStripMenuItem(Language.GetLocalizedText(1031, "Ease In/Out"));
            ToolStripMenuItem EaseIn = new ToolStripMenuItem(Language.GetLocalizedText(1032, "Ease In"));
            ToolStripMenuItem EaseOut = new ToolStripMenuItem(Language.GetLocalizedText(1033, "Ease Out"));
            ToolStripMenuItem Exponential = new ToolStripMenuItem(Language.GetLocalizedText(1034, "Exponential"));
            ToolStripMenuItem Default = new ToolStripMenuItem(Language.GetLocalizedText(1035, "Slide Default"));

            ToolStripMenuItem Align = new ToolStripMenuItem(Language.GetLocalizedText(790, "Align"));
            ToolStripMenuItem AlignTop = new ToolStripMenuItem(Language.GetLocalizedText(1333, "Top"));
            ToolStripMenuItem AlignBottom = new ToolStripMenuItem(Language.GetLocalizedText(1334, "Bottom"));
            ToolStripMenuItem AlignLeft = new ToolStripMenuItem(Language.GetLocalizedText(1335, "Left"));
            ToolStripMenuItem AlignRight = new ToolStripMenuItem(Language.GetLocalizedText(1336, "Right"));
            ToolStripMenuItem AlignHorizon = new ToolStripMenuItem(Language.GetLocalizedText(1337, "Horizontal"));
            ToolStripMenuItem AlignVertical = new ToolStripMenuItem(Language.GetLocalizedText(1338, "Vertical"));
            ToolStripMenuItem AlignCenter = new ToolStripMenuItem(Language.GetLocalizedText(1339, "Centered"));

            Align.DropDownItems.Add(AlignTop);
            Align.DropDownItems.Add(AlignBottom);
            Align.DropDownItems.Add(AlignLeft);
            Align.DropDownItems.Add(AlignRight);
            Align.DropDownItems.Add(AlignHorizon);
            Align.DropDownItems.Add(AlignVertical);
            Align.DropDownItems.Add(AlignCenter);



            Linear.Tag = InterpolationType.Linear;
            Ease.Tag = InterpolationType.EaseInOut;
            EaseIn.Tag = InterpolationType.EaseIn;
            EaseOut.Tag = InterpolationType.EaseOut;
            Exponential.Tag = InterpolationType.Exponential;
            Default.Tag = InterpolationType.Default;

            Linear.Click += new EventHandler(Interpolation_Click);
            Ease.Click += new EventHandler(Interpolation_Click);
            EaseIn.Click += new EventHandler(Interpolation_Click);
            EaseOut.Click += new EventHandler(Interpolation_Click);
            Exponential.Click += new EventHandler(Interpolation_Click);
            Default.Click += new EventHandler(Interpolation_Click);

            switch (Focus.InterpolationType)
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
                case InterpolationType.Default:
                    Default.Checked = true;
                    break;
                default:
                    break;
            }


            interpolateMenu.DropDownItems.Add(Default);
            interpolateMenu.DropDownItems.Add(Linear);
            interpolateMenu.DropDownItems.Add(Ease);
            interpolateMenu.DropDownItems.Add(EaseIn);
            interpolateMenu.DropDownItems.Add(EaseOut);
            interpolateMenu.DropDownItems.Add(Exponential);



            cutMenu.Click += new EventHandler(cutMenu_Click);
            copyMenu.Click += new EventHandler(copyMenu_Click);
            deleteMenu.Click += new EventHandler(deleteMenu_Click);
            bringToFront.Click += new EventHandler(bringToFront_Click);
            sendToBack.Click += new EventHandler(sendToBack_Click);
            sendBackward.Click += new EventHandler(sendBackward_Click);
            bringForward.Click += new EventHandler(bringForward_Click);
            properties.Click += new EventHandler(properties_Click);
            editText.Click += new EventHandler(editText_Click);
            url.Click += new EventHandler(url_Click);
            pickColor.Click += new EventHandler(pickColor_Click);
            addAction.Click += new EventHandler(addAction_Click);
            pasteMenu.Click += new EventHandler(pasteMenu_Click);
            animateMenu.Click += new EventHandler(animateMenu_Click);

            addToTimeline.Click += new EventHandler(addKeyframes_Click);
            addKeyframes.Click += new EventHandler(addKeyframes_Click);
            
            fullDome.Click += new EventHandler(fullDome_Click);
            flipbookProperties.Click += new EventHandler(flipbookProperties_Click);
            linkID.Click += new EventHandler(linkID_Click);

            AlignTop.Click += new EventHandler(AlignTop_Click);
            AlignBottom.Click += new EventHandler(AlignBottom_Click);
            AlignLeft.Click += new EventHandler(AlignLeft_Click);
            AlignRight.Click += new EventHandler(AlignRight_Click);
            AlignHorizon.Click += new EventHandler(AlignHorizon_Click);
            AlignVertical.Click += new EventHandler(AlignVertical_Click);
            AlignCenter.Click += new EventHandler(AlignCenter_Click);



            contextMenu.Items.Add(cutMenu);
            contextMenu.Items.Add(copyMenu);
            contextMenu.Items.Add(pasteMenu);
            contextMenu.Items.Add(deleteMenu);
            contextMenu.Items.Add(sep1);
            contextMenu.Items.Add(bringToFront);
            contextMenu.Items.Add(sendToBack);
            contextMenu.Items.Add(bringForward);
            contextMenu.Items.Add(sendBackward);
            contextMenu.Items.Add(Align);
            contextMenu.Items.Add(sep2);

            IDataObject data = Clipboard.GetDataObject();

            pasteMenu.Enabled = Clipboard.ContainsImage() | Clipboard.ContainsText() | Clipboard.ContainsAudio() | data.GetDataPresent(Overlay.ClipboardFormat);

            contextMenu.Items.Add(pickColor);
            contextMenu.Items.Add(url);
            contextMenu.Items.Add(linkID);
            contextMenu.Items.Add(animateMenu);
            contextMenu.Items.Add(addAction);

            if (Focus.AnimationTarget == null)
            {
                contextMenu.Items.Add(addToTimeline);
            }
            else
            {
                contextMenu.Items.Add(addKeyframes);
            }
 
            
            contextMenu.Items.Add(fullDome);
            contextMenu.Items.Add(sep3);
            contextMenu.Items.Add(flipbookProperties);
            animateMenu.Checked = Focus.Animate;
            fullDome.Checked = Focus.Anchor == OverlayAnchor.Dome;
            contextMenu.Items.Add(interpolateMenu);
            interpolateMenu.Enabled = Focus.Animate;

            flipbookProperties.Visible = (Focus is FlipbookOverlay);
            sep3.Visible = (Focus is FlipbookOverlay);

            if (multiSelect)
            {
                url.Visible = false;
                linkID.Visible = false;
                properties.Visible = false;
                flipbookProperties.Visible = false;
                bringForward.Visible = false;
                sendBackward.Visible = false;
            }
            else
            {
                Align.Visible = false;
            }

            contextMenu.Items.Add(properties);
            if (Focus != null)
            {
                if (Focus.GetType() == typeof(TextOverlay))
                {
                    contextMenu.Items.Add(editText);
                }
            }

            contextMenu.Show(Cursor.Position);
        }

        void editText_Click(object sender, EventArgs e)
        {
            if (Focus != null)
            {
                if (Focus.GetType() == typeof(TextOverlay))
                {
                    EditText();
                }
            }
        }

        void addKeyframes_Click(object sender, EventArgs e)
        {
            if (Focus != null)
            {
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1280, "Add Keyframe"), tour));

                Tour.CurrentTourStop.KeyFramed = true;

                foreach (Overlay overlay in selection.SelectionSet)
                {

                    if (overlay.AnimationTarget == null)
                    {
                        float savedTween = overlay.TweenFactor;
                        overlay.TweenFactor = 0;
                        overlay.AnimationTarget = new AnimationTarget(Tour.CurrentTourStop);
                        overlay.AnimationTarget.Target = overlay;
                        overlay.AnimationTarget.ParameterNames.AddRange(overlay.GetParamNames());
                        overlay.AnimationTarget.CurrentParameters = overlay.GetParams();
                        overlay.AnimationTarget.SetKeyFrame(0, Key.KeyType.Linear);
                        if (overlay.Animate)
                        {
                            overlay.TweenFactor = 1;
                            overlay.AnimationTarget.CurrentParameters = overlay.GetParams();
                            overlay.AnimationTarget.SetKeyFrame(1, Key.KeyType.Linear);
                            overlay.TweenFactor = savedTween;
                            overlay.Animate = false;
                        }

                        Tour.CurrentTourStop.AnimationTargets.Add(overlay.AnimationTarget);
                        TimeLine.RefreshUi();
                    }
                    else
                    {
                        overlay.AnimationTarget.SetKeyFrame(Tour.CurrentTourStop.TweenPosition, Key.KeyType.Linear);
                    }
                }
                TimeLine.RefreshUi();
            }
        }

        void fullDome_Click(object sender, EventArgs e)
        {
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1341, "Anchor Full Dome"), tour));
            if (Focus != null)
            {
                bool fullDome = Focus.Anchor != OverlayAnchor.Dome;

                foreach (Overlay overlay in selection.SelectionSet)
                {
                    overlay.Anchor = fullDome ? OverlayAnchor.Dome : OverlayAnchor.Screen;
                }
            }
        }

        void AlignVertical_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1036, "Vertical Align"), tour));

            float xCenter = Focus.X;

            foreach (Overlay overlay in selection.SelectionSet)
            {
                overlay.X = xCenter;
            }
        }

        void AlignHorizon_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1037, "Horizontal Align"), tour));

            float yCenter = Focus.Y;

            foreach (Overlay overlay in selection.SelectionSet)
            {
                overlay.Y = yCenter;
            }
        }

        void AlignCenter_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1038, "Align Centers"), tour));

            float yCenter = Focus.Y;
            float xCenter = Focus.X;
            foreach (Overlay overlay in selection.SelectionSet)
            {
                overlay.Y = yCenter;
                overlay.X = xCenter;
            }
        }

        void AlignRight_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1040, "Align Right"), tour));

            float left = Focus.X + Focus.Width / 2;

            foreach (Overlay overlay in selection.SelectionSet)
            {
                overlay.X = left - overlay.Width / 2;
            }
        }

        void AlignLeft_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1041, "Align Left"), tour));

            float right = Focus.X - Focus.Width / 2;

            foreach (Overlay overlay in selection.SelectionSet)
            {
                overlay.X = right + overlay.Width / 2;
            }

        }

        void AlignBottom_Click(object sender, EventArgs e)
        {
            
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1042, "Align Bottoms"), tour));

            float top = Focus.Y + Focus.Height / 2;

            foreach (Overlay overlay in selection.SelectionSet)
            {
                overlay.Y = top - overlay.Height / 2;
            }

        }

        void AlignTop_Click(object sender, EventArgs e)
        {
            if (Focus == null)
            {
                return;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1039, "Align Tops"), tour));

            float top = Focus.Y - Focus.Height/2;

            foreach (Overlay overlay in selection.SelectionSet)
            {
                overlay.Y = top + overlay.Height/2;
            }
        }

        void Interpolation_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            if (Focus != null)
            {
                foreach (Overlay overlay in selection.SelectionSet)
                {
                    overlay.InterpolationType = (InterpolationType)item.Tag;
                }
            }
        }

        void linkID_Click(object sender, EventArgs e)
        {
            SelectLink selectDialog = new SelectLink();
            selectDialog.Owner = Earth3d.MainWindow;
            selectDialog.Tour = tour;
            selectDialog.ID = Focus.LinkID;
            if (selectDialog.ShowDialog() == DialogResult.OK)
            {
                Focus.LinkID = selectDialog.ID;
            }
        }

        void flipbookProperties_Click(object sender, EventArgs e)
        {
            FlipbookOverlay flipbook = (FlipbookOverlay)Focus;
            FlipbookSetup properties = new FlipbookSetup();

            properties.LoopType = flipbook.LoopType;
            properties.FramesY = flipbook.FramesY;
            properties.FramesX = flipbook.FramesX;
            properties.FrameSequence = flipbook.FrameSequence;
            properties.StartFrame = flipbook.StartFrame;
            properties.Frames = flipbook.Frames;

            if (properties.ShowDialog() == DialogResult.OK)
            {
                flipbook.LoopType = properties.LoopType;
                flipbook.FramesY = properties.FramesY;
                flipbook.FramesX = properties.FramesX;
                flipbook.FrameSequence = properties.FrameSequence;
                flipbook.StartFrame = properties.StartFrame;
                flipbook.Frames = properties.Frames;
            }
        }

        void animateMenu_Click(object sender, EventArgs e)
        {
            if (Focus != null)
            {
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(588, "Animate"), tour));

                bool animate = !Focus.Animate;

                foreach (Overlay overlay in selection.SelectionSet)
                {
                    overlay.Animate = animate;
                }
                
            }
        }

        void url_Click(object sender, EventArgs e)
        {
            if (Focus != null)
            {
                SimpleInput input = new SimpleInput(Language.GetLocalizedText(541, "Edit Hyperlink"), Language.GetLocalizedText(542, "Url"), Focus.Url, 2048);
                if (input.ShowDialog() == DialogResult.OK)
                {
                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(541, "Edit Hyperlink"), tour));
                    Focus.Url = input.ResultText;
                }
            }
        }

        void pickColor_Click(object sender, EventArgs e)
        {
            PopupColorPicker picker = new PopupColorPicker();

            picker.Location = Cursor.Position;

            picker.Color = Focus.Color;

            if (picker.ShowDialog() == DialogResult.OK)
            {        
                //todo localize
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(543, "Edit Color"), tour));
                foreach (Overlay overlay in selection.SelectionSet)
                {
                    overlay.Color = picker.Color;
                }
            }
        }

        void addAction_Click(object sender, EventArgs e)
        {
            ActionEdit actionEdit = new ActionEdit();
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1378, "Add/Edit Quiz Action"), tour));
            actionEdit.Overlay = Focus;
            actionEdit.ShowDialog();
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void volume_Click(object sender, EventArgs e)
        {
            PopupVolume vol = new PopupVolume();
            vol.Volume = ((AudioOverlay)Focus).Volume;
            vol.ShowDialog();
            ((AudioOverlay)Focus).Volume = vol.Volume;
        }

        void deleteMenu_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(167, "Delete"), tour));

            foreach (Overlay overlay in selection.SelectionSet)
            {
                tour.CurrentTourStop.RemoveOverlay(overlay);
            }

            Focus = null;
            ClearSelection();
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void properties_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {

            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(549, "Properties Edit"), tour));
            OverlayProperties props = new OverlayProperties();
            props.Overlay = Focus;

            props.ShowDialog();
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void bringForward_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(454, "Bring Forward"), tour));
            foreach (Overlay overlay in GetSortedSelection(false))
            {
                tour.CurrentTourStop.BringForward(overlay);
            }
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void sendBackward_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(455, "Send Backward"), tour));
            foreach (Overlay overlay in GetSortedSelection(true))
            {
                tour.CurrentTourStop.SendBackward(overlay);
            }
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void sendToBack_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(453, "Send to Back"), tour));
            foreach (Overlay overlay in GetSortedSelection(true))
            {
                tour.CurrentTourStop.SendToBack(overlay);
            }
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        void bringToFront_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(452, "Bring to Front"), tour));
            foreach (Overlay overlay in GetSortedSelection(false))
            {
                tour.CurrentTourStop.BringToFront(overlay);
            }
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        Overlay[] GetSortedSelection(bool reverse)
        {
            List<Overlay> sorted = new List<Overlay>();

            sorted.AddRange(selection.SelectionSet);

            if (reverse)
            {
                sorted.Sort(delegate(Overlay p1, Overlay p2) { return -p1.ZOrder.CompareTo(p2.ZOrder); });
            }
            else
            {

                sorted.Sort(delegate(Overlay p1, Overlay p2) { return p1.ZOrder.CompareTo(p2.ZOrder); });
            }
            return sorted.ToArray();
        }


        void copyMenu_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter textWriter = new System.IO.StringWriter(sb))
            {
                using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(textWriter))
                {
                    writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                    writer.WriteStartElement("Overlays");
                    foreach (Overlay overlay in selection.SelectionSet)
                    {
                        overlay.SaveToXml(writer, true);
                    }

                    writer.WriteEndElement();
                }
            }
            DataFormats.Format format = DataFormats.GetFormat(Overlay.ClipboardFormat);

            IDataObject dataObject = new DataObject();
            dataObject.SetData(format.Name, false, sb.ToString());

            Clipboard.SetDataObject(dataObject, false);
        }

        void cutMenu_Click(object sender, EventArgs e)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(427, "Cut"), tour));
            copyMenu_Click(sender, e);

            foreach (Overlay overlay in selection.SelectionSet)
            {
                tour.CurrentTourStop.RemoveOverlay(overlay);
            }
            Focus = null;
            ClearSelection();
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection); 
        }

        void pasteMenu_Click(object sender, EventArgs e)
        {

            Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(544, "Paste Object"), tour));

            IDataObject dataObject = Clipboard.GetDataObject();

            if (dataObject.GetDataPresent(Overlay.ClipboardFormat))
            {
                // add try catch block
                string xml = dataObject.GetData(Overlay.ClipboardFormat) as string;
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                ClearSelection();
                System.Xml.XmlNode parent = doc["Overlays"];
                foreach (XmlNode child in parent.ChildNodes)
                {
                    Overlay copy = Overlay.FromXml(tour.CurrentTourStop, child);
                    if (copy.AnimationTarget != null)
                    {
                        copy.Id = Guid.NewGuid().ToString();
                        copy.AnimationTarget.TargetID = copy.Id;
                        tour.CurrentTourStop.AnimationTargets.Add(copy.AnimationTarget);
                    }
                    bool found = false;
                    float maxX = 0;
                    float maxY = 0;
                    foreach (Overlay item in tour.CurrentTourStop.Overlays)
                    {
                        if (item.Id == copy.Id && item.GetType() == copy.GetType())
                        {
                            found = true;
                            if (maxY < item.Y || maxX < item.X)
                            {
                                maxX = item.X;
                                maxY = item.Y;
                            }
                        }
                    }

                    if (found)
                    {
                        copy.X = maxX + 20;
                        copy.Y = maxY + 20;
                    }

                    tour.CurrentTourStop.AddOverlay(copy);
                    Focus = copy;
                    selection.AddSelection(Focus);
                    OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
                }
            }
            else if (UiTools.IsMetaFileAvailable())
            {
                Image img = UiTools.GetMetafileFromClipboard();
                if (img != null)
                {
                    BitmapOverlay bmp = new BitmapOverlay( tour.CurrentTourStop, img);
                    tour.CurrentTourStop.AddOverlay(bmp);
                    bmp.X = contextPoint.X;
                    bmp.Y = contextPoint.Y;
                    Focus = bmp;
                    selection.SetSelection(Focus);
                    OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
                }
            }
            else if (Clipboard.ContainsText() && Clipboard.GetText().Length > 0)
            {
                TextObject temp = TextEditor.DefaultTextobject;
                temp.Text = Clipboard.GetText();

                TextOverlay text = new TextOverlay( temp);
                //text.X = Earth3d.MainWindow.ClientRectangle.Width / 2;
                //text.Y = Earth3d.MainWindow.ClientRectangle.Height / 2;
                text.X = contextPoint.X;
                text.Y = contextPoint.Y;
                tour.CurrentTourStop.AddOverlay(text);
                Focus = text;
                selection.SetSelection(Focus);
                OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            }
            else if (Clipboard.ContainsImage())
            {
                Image img = Clipboard.GetImage();
                BitmapOverlay bmp = new BitmapOverlay( tour.CurrentTourStop, img);
                tour.CurrentTourStop.AddOverlay(bmp);
                bmp.X = contextPoint.X;
                bmp.Y = contextPoint.Y;
                Focus = bmp;
                selection.SetSelection(Focus);
                OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            }
            
        }

        public bool MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.MouseClick(sender, e))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Click(object sender, EventArgs e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.Click(sender, e))
                {
                    return true;
                }
            }
            return false;
        }

        public bool MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.MouseDoubleClick(sender, e))
                {
                    return true;
                }
            }

            if (Focus != null)
            {
                if (Focus.GetType() == typeof(TextOverlay))
                {
                    EditText();
                    return true;
                }
            }
            return true;
        }

        private void EditText()
        {
            TextEditor te = new TextEditor();
            te.TextObject = ((TextOverlay)Focus).TextObject;
            if (te.ShowDialog() == DialogResult.OK)
            {
                //todo localize
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(545, "Text Edit"), tour));
                ((TextOverlay)Focus).TextObject = te.TextObject;
                ((TextOverlay)Focus).Width = 0;
                ((TextOverlay)Focus).Height = 0;
                Focus.Color = te.TextObject.ForegroundColor;
                Focus.CleanUp();
            }
        }

        public bool KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.KeyDown(sender, e))
                {
                    return true;
                }
            }

            int increment = 1;
            if (e.Control)
            {
                increment = 10;
            }

            switch (e.KeyCode)
            {
                case Keys.A:
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        ClearSelection();
                        selection.AddSelection(tour.CurrentTourStop.Overlays.ToArray());
                        OverlayList.UpdateOverlayListSelection(selection);
                        if (tour.CurrentTourStop.Overlays.Count > 0)
                        {
                            Focus = tour.CurrentTourStop.Overlays[0];
                        }
                    }
                    break;
                case Keys.Z:
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        if (Undo.PeekAction())
                        {
                            Earth3d.MainWindow.TourEdit.UndoStep();
                        }
                        else
                        {
                            UiTools.Beep();
                        }
                    }
                    break;
                case Keys.Y:
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        if (Undo.PeekRedoAction())
                        {
                            Earth3d.MainWindow.TourEdit.RedoStep();
                        }
                        else
                        {
                            UiTools.Beep();
                        }
                    }
                    break;
                case Keys.C:
                    if (e.Control)
                    {
                        this.copyMenu_Click(this, new EventArgs());
                    }
                    break;
                case Keys.V:
                    if (e.Control)
                    {
                        this.pasteMenu_Click(this, new EventArgs());
                    }
                    break;
                case Keys.X:
                    if (e.Control)
                    {
                        this.cutMenu_Click(this, new EventArgs());
                    }
                    break;
                case Keys.Delete:
                    this.deleteMenu_Click(null, null);
                    return true;
                case Keys.Tab:
                    if (e.Shift)
                    {
                        SelectLast();
                    }
                    else
                    {
                        SelectNext();
                    }
                    return true;
                case Keys.Left:
                    if (Focus != null)
                    {
                        foreach (Overlay overlay in selection.SelectionSet)
                        {
                            if (e.Shift)
                            {
                                if (e.Alt)
                                {
                                    if (overlay.Width > increment)
                                    {
                                        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                        overlay.Width -= increment;
                                    }
                                }
                                else
                                {
                                    float aspect = overlay.Width / overlay.Height;
                                    if (overlay.Width > increment && overlay.Height > (increment * aspect))
                                    {
                                        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                        overlay.Width -= increment;
                                        overlay.Height -= increment * aspect;
                                    }
                                }
                            }
                            else if (e.Alt)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(538, "Rotate"), tour));
                                overlay.RotationAngle -= increment;
                            }
                            else
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(540, "Move"), tour));
                                overlay.X -= increment;
                            }
                        }
                        return true;
                    }
                    break;
                case Keys.Right:
                    if (Focus != null)
                    {
                        foreach (Overlay overlay in selection.SelectionSet)
                        {
                            if (e.Shift)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                if (e.Alt)
                                {
                                    overlay.Width += increment;

                                }
                                else
                                {
                                    float aspect = overlay.Width / overlay.Height;
                                    overlay.Width += increment;
                                    overlay.Height += increment * aspect;
                                }
                            }
                            else if (e.Alt)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(538, "Rotate"), tour));
                                overlay.RotationAngle += increment;
                            }
                            else
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(540, "Move"), tour));
                                overlay.X += increment;
                            }
                        }
                        return true;
                    }
                    break;
                case Keys.Up:
                    if (Focus != null)
                    {
                        foreach (Overlay overlay in selection.SelectionSet)
                        {
                            if (e.Shift)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                if (e.Alt)
                                {
                                    overlay.Height += increment;
                                }
                                else
                                {
                                    float aspect = overlay.Width / overlay.Height;
                                    overlay.Width += increment;
                                    overlay.Height += increment * aspect;
                                }
                            }
                            else if (!e.Alt)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(540, "Move"), tour));
                                overlay.Y -= increment;
                            }
                        }
                        return true;
                    }
                    break;
                case Keys.Down:
                    if (Focus != null)
                    {
                        foreach (Overlay overlay in selection.SelectionSet)
                        {
                            if (e.Shift)
                            {
                                if (e.Alt)
                                {
                                    if (overlay.Height > increment)
                                    {
                                        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                        overlay.Height -= increment;
                                    }
                                }
                                else
                                {
                                    float aspect = overlay.Width / overlay.Height;
                                    if (overlay.Width > increment && overlay.Height > (increment * aspect))
                                    {
                                        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(537, "Resize"), tour));
                                        overlay.Width -= increment;
                                        overlay.Height -= increment * aspect;
                                    }
                                }
                            }
                            else if (!e.Alt)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(540, "Move"), tour));
                                overlay.Y += increment;
                            }
                        }
                        return true;
                    }
                    break;
                case Keys.PageDown:
                    // Next Slide
                    if (e.Alt)
                    {
                        if (tour.CurrentTourstopIndex < (tour.TourStops.Count - 1))
                        {
                            tour.CurrentTourstopIndex++;
                            Earth3d.MainWindow.TourEdit.SelectCurrent();
                            Earth3d.MainWindow.TourEdit.EnsureSelectedVisible();
                        }
                        return true;
                    }

                    break;
                case Keys.PageUp:
                    // Prev Slide
                    if (e.Alt)
                    {
                        if (tour.CurrentTourstopIndex > 0)
                        {
                            tour.CurrentTourstopIndex--;
                            Earth3d.MainWindow.TourEdit.SelectCurrent();
                            Earth3d.MainWindow.TourEdit.EnsureSelectedVisible();
                        }
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void SelectNext()
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }

            Focus = tour.CurrentTourStop.GetNextOverlay(Focus);
            selection.SetSelection(Focus);
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        private void SelectLast()
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }

            Focus = tour.CurrentTourStop.GetPerviousOverlay(Focus);
            selection.SetSelection(Focus);
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
        }

        public bool KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.KeyUp(sender, e))
                {
                    return true;
                }
            }
            return false;
        }

        public bool AddPicture(string filename)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(546, "Insert Picture"), tour));
            BitmapOverlay bmp = new BitmapOverlay(tour.CurrentTourStop, filename);
            bmp.X = 960;
            bmp.Y = 600;
            tour.CurrentTourStop.AddOverlay(bmp);
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            return true;
        }

        public bool AddFlipbook(string filename)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            //todo localize

            FlipbookSetup flipSetup = new FlipbookSetup();

            
            if (flipSetup.ShowDialog() == DialogResult.OK)
            {
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1342, "Insert Flipbook"), tour));
                FlipbookOverlay flipbook = new FlipbookOverlay( tour.CurrentTourStop, filename);
                flipbook.X = 960;
                flipbook.Y = 600;
                flipbook.LoopType = flipSetup.LoopType;
                flipbook.FramesY = flipSetup.FramesY;
                flipbook.FramesX = flipSetup.FramesX;
                flipbook.FrameSequence = flipSetup.FrameSequence;
                flipbook.StartFrame = flipSetup.StartFrame;
                flipbook.Frames = flipSetup.Frames;

                tour.CurrentTourStop.AddOverlay(flipbook);
                OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
                return true;
            }
            return false;
        }

        public bool AddAudio(string filename)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            AudioOverlay audio = new AudioOverlay( tour.CurrentTourStop, filename);
            audio.X = 900;
            audio.Y = 600;
            tour.CurrentTourStop.AddOverlay(audio);
            return true;
        }

        public bool AddVideo(string filename)
        {
            // depracted video type
            //if (tour == null || tour.CurrentTourStop == null)
            //{
            //    return false;
            //}

            //VideoClip video = new VideoClip(Earth3d.MainWindow.Device, tour.CurrentTourStop, filename);
            //video.X = 960;
            //video.Y = 600;
            //tour.CurrentTourStop.AddOverlay(video);
            return true;

        }

        public bool AddText(string p, TextObject textObject)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            TextOverlay text = new TextOverlay( textObject);
            text.Color = textObject.ForegroundColor;
            text.X = 960;
            text.Y = 600;
            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(547, "Insert Text"), tour));
            tour.CurrentTourStop.AddOverlay(text);
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            return true;
        }

        public Overlay AddOverlay(Overlay ol)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return null;
            }
            if(ol.GetType() == typeof(ShapeOverlay))
            {
                ShapeOverlay srcShapeOverlay = (ShapeOverlay)ol;
                if (srcShapeOverlay != null)
                {
                    ShapeOverlay shape = new ShapeOverlay(tour.CurrentTourStop, srcShapeOverlay.ShapeType);
                    shape.Width = srcShapeOverlay.Width;
                    shape.Height = srcShapeOverlay.Height;
                    shape.X = contextPoint.X;
                    shape.Y = contextPoint.Y;
                    shape.Color = srcShapeOverlay.Color;
                    shape.RotationAngle = srcShapeOverlay.RotationAngle;
                    if (ol.AnimationTarget != null)
                    {
                        shape.AnimationTarget = ol.AnimationTarget.Clone(shape);
                    }
                    tour.CurrentTourStop.AddOverlay(shape);
                    return shape;
                }
            }
            else if (ol.GetType() == typeof(TextOverlay))
            {
                TextOverlay srcTxtOverlay = (TextOverlay)ol;
                if (srcTxtOverlay != null)
                {
                    TextOverlay text = new TextOverlay(srcTxtOverlay.TextObject);
                    text.X = contextPoint.X;
                    text.Y = contextPoint.Y;
                    text.Color = srcTxtOverlay.Color;
                    if (ol.AnimationTarget != null)
                    {
                        text.AnimationTarget = ol.AnimationTarget.Clone(text);
                    }
                    tour.CurrentTourStop.AddOverlay(text);
                    return text;
                }
            }
            else if (ol.GetType() == typeof(BitmapOverlay))
            {
                BitmapOverlay srcBmpOverlay = (BitmapOverlay)ol;
                if (srcBmpOverlay != null)
                {
                    BitmapOverlay bitmap = srcBmpOverlay.Copy(tour.CurrentTourStop);
                    bitmap.X = contextPoint.X;
                    bitmap.Y = contextPoint.Y;
                    if (ol.AnimationTarget != null)
                    {
                        bitmap.AnimationTarget = ol.AnimationTarget.Clone(bitmap);
                    }
                    tour.CurrentTourStop.AddOverlay(bitmap);
                    return bitmap;
                }
            }
            else if (ol.GetType() == typeof(FlipbookOverlay))
            {
                FlipbookOverlay srcFlipbookOverlay = (FlipbookOverlay)ol;
                if (srcFlipbookOverlay != null)
                {
                    FlipbookOverlay bitmap = srcFlipbookOverlay.Copy(tour.CurrentTourStop);
                    bitmap.X = contextPoint.X;
                    bitmap.Y = contextPoint.Y;
                    if (ol.AnimationTarget != null)
                    {
                        bitmap.AnimationTarget = ol.AnimationTarget.Clone(bitmap);
                    }
                    tour.CurrentTourStop.AddOverlay(bitmap);
                    return bitmap;
                }
            }
            return null;
        }

        public bool AddShape(string p, ShapeType shapeType)
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return false;
            }

            //todo localize
            Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(548, "Insert Shape"), tour));

            ShapeOverlay shape = new ShapeOverlay(tour.CurrentTourStop, shapeType);
            shape.Width = 200;
            shape.Height = 200;

            if (shapeType == ShapeType.Arrow)
            {
                shape.Height /= 2;
            }
            if (shapeType == ShapeType.Line)
            {
                shape.Height = 12;
            }
            shape.X = 960;
            shape.Y = 600;
            tour.CurrentTourStop.AddOverlay(shape);

            Focus = shape;
            selection.SetSelection(Focus);
            OverlayList.UpdateOverlayList(tour.CurrentTourStop, Selection);
            return true;
        }
        Color defaultColor = Color.White;
        public Color GetCurrentColor()
        {
            if (tour == null || tour.CurrentTourStop == null)
            {
                return defaultColor;
            }
            if (Focus != null)
            {
               return Focus.Color;
            }
            else
            {
                return defaultColor;
            }
        }

        public void SetCurrentColor(Color color)
        {
            defaultColor = color;
            if (tour == null || tour.CurrentTourStop == null)
            {
                return;
            }
            if (Focus != null)
            {
                Focus.Color = color;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (contextMenu != null)
            {
                contextMenu.Dispose();
                contextMenu = null;
            }
        }

        #endregion


        public bool Hover(Point pnt)
        {
            if (CurrentEditor != null)
            {
                if (CurrentEditor.Hover(pnt))
                {
                    return true;
                }
            }
            return true;
        }

    }
}
