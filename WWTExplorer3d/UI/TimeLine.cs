using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class TimeLine : UserControl
    {
        public TimeLine()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.toolTip.SetToolTip(this.End, Language.GetLocalizedText(1285, "Jump to End"));
            this.toolTip.SetToolTip(this.DelKey, Language.GetLocalizedText(1286, "Delete Key"));
            this.toolTip.SetToolTip(this.AddKey, Language.GetLocalizedText(1287, "Add Key"));
            this.toolTip.SetToolTip(this.Start, Language.GetLocalizedText(1288, "Jump to Begin"));
            this.toolTip.SetToolTip(this.Forward, Language.GetLocalizedText(441, "Play"));
            this.toolTip.SetToolTip(this.Back, Language.GetLocalizedText(1289, "Play Backward"));
            this.toolTip.SetToolTip(this.Pause, Language.GetLocalizedText(440, "Pause"));
        }

        private TourDocument tour = null;

        public static TourDocument activeTour = null;

        public TourDocument Tour
        {
            get { return tour; }
            set
            {
                if (tour != value)
                {
                    tour = value;
                    Invalidate();
                }
            }
        }

        static Bitmap bmpScrollBackLeft = global::TerraViewer.Properties.Resources.scroll_background_left;
        static Bitmap bmpScrollBackMiddle = global::TerraViewer.Properties.Resources.scroll_background_middle;
        static Bitmap bmpScrollBackRight = global::TerraViewer.Properties.Resources.scroll_background_right;

        static Bitmap bmpScrollBarLeft = global::TerraViewer.Properties.Resources.scroll_bar_left;
        static Bitmap bmpScrollBarMiddle = global::TerraViewer.Properties.Resources.scroll_bar_middle;
        static Bitmap bmpScrollBarRight = global::TerraViewer.Properties.Resources.scroll_bar_right;


        public static void RefreshUi()
        {
            foreach (TimeLine tl in instances)
            {
                tl.EnsureVisible();
                tl.Invalidate();
            }
        }
        
        public static void RefreshUi(bool ensureVisible)
        {
            foreach (TimeLine tl in instances)
            {
                if (ensureVisible)
                {
                    tl.EnsureVisible();
                }
                tl.Invalidate();
            }
        }

        public static void SetTour(TourDocument tour)
        {
            activeTour = tour;
            
            foreach (TimeLine tl in instances)
            {
                tl.Tour = tour;
                tl.EnsureVisible();
                tl.Invalidate();
            }
        }

        public static bool AreOpenTimelines
        {
            get
            {
                foreach (TimeLine tl in instances)
                {
                    if (tl.Visible)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        static List<TimeLine> instances = new List<TimeLine>();

        List<string> Selection = new List<string>();
        string[] lineIds = new string[0];
        List<VisibleKey> VisibleKeys = new List<VisibleKey>();
        Dictionary<string, VisibleKey> selectedKeys = new Dictionary<string, VisibleKey>();

        public int ScrollPos = 0;
        int left = 250;
        int bottom = 30;
        int tick = 8;
        int step = 8;
        int offset = 0;
        int startSecond = 0;
        int pixelsPerSecond = 8/*tick*/ * 30;
        int totalPixelWith = 2400;
        double secondsOnscreen = 1;
        double slideTime = 10;
        double lastKeyTime = 1;
        int totalFrames = 300;
        double frameTime = 1 / 300;
        double currentTween = 0;
        int lineHeight = 16;
        int displayLines = 1;

        int totalLines = 1;
        int startLine = 0;
        int nextLine = 0;
        bool vScrollBarShown = false;

        public double TweenPosition
        {
            get { return currentTween; }
            set
            {
                if (currentTween != value)
                {
                    currentTween = value;
                    Refresh();
                }
            }
        }

        public void SetTweenPosition(double tween)
        {

            if (Tour != null)
            {
                if (Tour.CurrentTourStop != null)
                {
                    Tour.CurrentTourStop.TweenPosition = Math.Min(1,(float)tween);
                    Settings.TourSettings = Tour.CurrentTourStop;
                }
            }
            TweenPosition = tween;
       }

        private int TimeToPixel(double time)
        {
            return ((int)((slideTime * 30) * time + .5) * tick) + offset - (pixelsPerSecond * startSecond);
        }

        private double PixelToTime(int pixel)
        {
            pixel = (pixel / tick) * tick;
            return Math.Min(lastKeyTime, Math.Max(0, (pixel - offset + (pixelsPerSecond * startSecond)) / (double)totalPixelWith));
        }

        public int TimeToFrame(double time)
        {
            return (int)((slideTime*30)*time);
            
        }

        private void TimeLine_Paint(object sender, PaintEventArgs e)
        {
            if (!DesignMode)
            {
                if (Earth3d.MainWindow != null && Earth3d.MainWindow.TourEdit != null && Earth3d.MainWindow.TourEdit.Tour != null)
                {
                    Tour = Earth3d.MainWindow.TourEdit.Tour;
                }
                else
                {
                    Tour = null;
                }
            }

            
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.FromArgb(71, 84, 108));

            offset = left - (int)(((double)ScrollPos / ((double)scrollbarWidth / (double)(Width - left))) % pixelsPerSecond);

            if (Tour != null && Tour.CurrentTourStop != null)
            {
                slideTime = Tour.CurrentTourStop.Duration.TotalSeconds;
                currentTween = Tour.CurrentTourStop.TweenPosition;
            }

            totalFrames = (int)(slideTime * 30);
            frameTime = 1.0 / totalFrames;
            lastKeyTime = 1;// -frameTime;

            pixelsPerSecond = 30 * tick;
            totalPixelWith = (int)(pixelsPerSecond * slideTime);
            secondsOnscreen = (double)(Width - left) / (double)pixelsPerSecond;
            ScrollPos = (int)Math.Max(0, Math.Min(ScrollPos, (slideTime * pixelsPerSecond - secondsOnscreen * pixelsPerSecond)));
            scrollbarWidth = (int)((Width - left) * Math.Min(1, (secondsOnscreen / slideTime)));
            scrollbarStart = ScrollPos + left;
            startSecond = (int)(((double)ScrollPos / ((double)scrollbarWidth / (double)(Width - left))) / (double)pixelsPerSecond);

            SolidBrush background = new SolidBrush(Color.FromArgb(40, 44, 60));
            SolidBrush darkBackground = new SolidBrush(this.BackColor);
            SolidBrush lightBackground = new SolidBrush(Color.FromArgb(30, 33, 46));
            g.FillRectangle(background, new Rectangle(0, 0, Width, bottom));

            Rectangle buttons = new Rectangle(-3,-3, left+5,37);

            bool buttonsOnly = buttons.Contains(e.ClipRectangle);

            if (!buttonsOnly)
            {


                for (int y = bottom + lineHeight * 3; y < Height; y += lineHeight * 6)
                {
                    g.FillRectangle(lightBackground, new Rectangle(left, y, Width - left, lineHeight * 3));
                }


                step = (tick > 7) ? tick : (tick > 3) ? (tick * 2) : (tick * 5);
                int endPos = Math.Min(Width - offset, totalPixelWith) + step;

                for (int x = 0; x < endPos; x += step)
                {
                    bool big = (x % (5 * step)) == 0;

                    //g.DrawLine(Pens.White, x, big ? 15 : 20, x, bottom);

                    if (big)
                    {
                        g.DrawLine(Pens.DarkGray, x + offset, bottom, x + offset, Height);
                        g.DrawLine(Pens.LightGray, x + offset, 15, x + offset, bottom);
                    }
                    else
                    {
                        g.DrawLine(Pens.Gray, x + offset, bottom, x + offset, Height);
                        g.DrawLine(Pens.LightGray, x + offset, 20, x + offset, bottom);
                    }
                }

                TimeSpan ts = new TimeSpan(0, 0, startSecond);
                TimeSpan inc = new TimeSpan(0, 0, 1);

                for (int x = 0; x < endPos; x += (30 * tick))
                {
                    g.DrawString(string.Format("{0:0#}:{1:0#}", ts.Minutes, ts.Seconds), UiTools.StandardRegular, UiTools.StadardTextBrush, new PointF((x + offset) + 1, 2));
                    g.DrawLine(Pens.White, x + offset, 0, x + offset, Height);
                    ts = ts.Add(inc);
                }
                if (tick > 3)
                {
                    for (int x = 0; x < endPos; x += (5 * step))
                    {
                        int mark = (x / tick) % 30;

                        if (mark != 0)
                        {

                            g.DrawString(string.Format("{0}", mark), UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF((x + offset) - ((mark < 10) ? 4 : 7), 2));
                        }
                    }
                }

                nextLine = -startLine;

                int indent = 2;

                bool displayLine = false;
                totalLines = 0;

                displayLines = (Height - bottom) / lineHeight;

                lineIds = new string[displayLines];

                g.FillRectangle(background, new Rectangle(0, 0, left, bottom));
                g.FillRectangle(darkBackground, new Rectangle(0, bottom, left, (Height - bottom)));

                if (nextLine >= 0)
                {
                    displayLine = true;
                }

                bool selected = false;

                int displayedLineIndex = 0;

                VisibleKeys.Clear();

                for (int y = bottom + lineHeight * 3; y < Height; y += lineHeight * 6)
                {
                    g.FillRectangle(lightBackground, new Rectangle(0, y, left, lineHeight * 3));
                }

                // Draw Text
                if (Tour != null && tour.CurrentTourStop != null && tour.CurrentTourStop.KeyFramed)
                {
                    foreach (AnimationTarget target in tour.CurrentTourStop.AnimationTargets)
                    {
                        if (target.Target == null)
                        {
                            continue;
                        }
                        string name = target.Target.GetName();
                        string targetID = target.Target.GetIndentifier();
                        if (displayLine)
                        {
                            selected = Selection.Contains(targetID);
                            g.DrawString((target.Expanded ? "-   " : "+  ") + name, UiTools.StandardRegular, selected ? UiTools.YellowTextBrush : UiTools.StadardTextBrush, new PointF(indent, nextLine * lineHeight + bottom + 2));
                            lineIds[displayedLineIndex] = targetID;
                            displayedLineIndex++;
                        }
                        indent += 20;

                        if (target.Expanded)
                        {
                            nextLine++;
                            totalLines++;

                            if (nextLine * lineHeight + bottom + 2 + 16 > Height)
                            {
                                displayLine = false;
                            }
                            else
                            {
                                if (nextLine >= 0)
                                {
                                    displayLine = true;
                                }
                            }
                        }

                        for (int i = 0; i < target.ParameterNames.Count; i++)
                        {
                            string line = target.ParameterNames[i];
                            if (displayLine)
                            {
                                if (target.Expanded)
                                {
                                    string id = targetID + "\t" + line;
                                    selected = Selection.Contains(id);
                                    g.DrawString(line, UiTools.StandardRegular, selected ? UiTools.YellowTextBrush : UiTools.StadardTextBrush, new PointF(indent, nextLine * lineHeight + bottom + 2));
                                    lineIds[displayedLineIndex] = id;
                                    displayedLineIndex++;
                                }
                                foreach (Key key in target.KeyFrames[i].Keys.Values)
                                {
                                    bool keySelected = selectedKeys.ContainsKey(VisibleKey.GetIndexKey(target, i, key.Time));
                                    int xx = 0;

                                    if (ghostingSelection && keySelected)
                                    {
                                        xx = TimeToPixel(key.GhostTime);
                                    }
                                    else
                                    {
                                        xx = TimeToPixel(key.Time);
                                    }

                                    if (xx >= left && xx < Width + (step / 2))
                                    {
                                        int yy = nextLine * lineHeight + bottom + 2;
                                        VisibleKey vk = new VisibleKey(target, i, line, key.Time, new Point(xx, yy));
                                        VisibleKeys.Add(vk);


                                        g.FillRectangle(keySelected ? UiTools.YellowTextBrush : UiTools.StadardTextBrush, new Rectangle(xx - (step / 2), yy, step, 10));
                                    }
                                }
                            }

                            if (target.Expanded)
                            {
                                nextLine++;
                                totalLines++;
                                if (nextLine * lineHeight + bottom + 2 + 16 > Height)
                                {
                                    displayLine = false;
                                }
                                else
                                {
                                    if (nextLine >= 0)
                                    {
                                        displayLine = true;
                                    }
                                }
                            }

                        }

                        if (!target.Expanded)
                        {
                            nextLine++;
                            totalLines++;

                            if (nextLine * lineHeight + bottom + 2 + 16 > Height)
                            {
                                displayLine = false;
                            }
                            else
                            {
                                if (nextLine >= 0)
                                {
                                    displayLine = true;
                                }
                            }
                        }

                        indent -= 20;

                    }


                    vScrollBarShown = false;
                    //Draw vertical scrollbar
                    if (displayLine == false)
                    {
                        Brush scrollBar = new SolidBrush(Color.FromArgb(112, 140, 186));
                        Pen scrollBarOutline = new Pen(scrollBar);
                        g.FillRectangle(background, new Rectangle(left - 20, bottom, 20, (Height - bottom)));

                        vScrollBarShown = true;


                        vScrollBarHieght = ((Height - bottom) * displayLines) / totalLines;
                        vScrollBarTop = ((Height - bottom) * startLine) / totalLines;
                        g.FillRectangle(scrollBar, new Rectangle(left - 20, vScrollBarTop + bottom, 20, vScrollBarHieght));

                        g.DrawLine(scrollBarOutline, left - 20, bottom, left - 20, Height);

                        scrollBarOutline.Dispose();
                        scrollBar.Dispose();
                    }
                    else
                    {
                        if (startLine != 0)
                        {
                            startLine = 0;
                            Invalidate();
                        }
                    }

                    // Draw time location
                    int tx = TimeToPixel(TweenPosition) - (step / 2);
                    if (tx >= (left - (step / 2)))
                    {
                        g.DrawLine(Pens.Yellow, tx, bottom, tx, Height - 8);
                        g.DrawLine(Pens.Yellow, tx + step, bottom, tx + step, Height - 8);
                        g.FillPolygon(UiTools.YellowTextBrush, new Point[] { new Point(tx - (step / 2), bottom - 8), new Point((int)(tx + (step * 1.5)), bottom - 8), new Point(tx + (step / 2), bottom) });
                    }

                    //Key Selection Rect
                    if (selectingKeys)
                    {
                        g.DrawRectangle(Pens.Yellow, selectRect);
                    }

                }
                else
                {
                    startLine = 0;
                }

                // Edge effect
                g.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));

                // Draw scrollbar
                g.DrawImageUnscaled(bmpScrollBackLeft, left, Height - 7);
                for (int j = left; j < Width - 10; j += 50)
                {
                    g.DrawImageUnscaled(bmpScrollBackMiddle, j, Height - 7);
                }
                g.DrawImageUnscaled(bmpScrollBackRight, Width - 10, Height - 7);




                g.DrawImageUnscaled(bmpScrollBarLeft, scrollbarStart, Height - 6);
                for (int j = scrollbarStart + 10; j < (scrollbarWidth + scrollbarStart) - 10; j += 50)
                {
                    g.DrawImageUnscaledAndClipped(bmpScrollBarMiddle, new Rectangle(j, Height - 6, Math.Min(50, ((scrollbarWidth + scrollbarStart) - j) - 10), 6));
                }
                g.DrawImageUnscaled(bmpScrollBarRight, (scrollbarStart + scrollbarWidth) - 10, Height - 6);

            }
            else
            {
                // Edge effect
                g.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
            }

            AddKey.Enabled = Selection.Count > 0;
            DelKey.Enabled = selectedKeys.Count > 0;


            lightBackground.Dispose();
            darkBackground.Dispose();
            background.Dispose();
            p.Dispose();


        }

        bool mouseDown = false;
        bool scrolling = false;
        int scrollbarWidth = 100;
        int scrollbarStart = 0;

        int vScrollBarHieght = 10;
        int vScrollBarTop = 0;

        Point pointDown;
        bool timeDrag = false;


        bool vScroolling = false;
        bool selectingKeys = false;
        bool wasSelectRectDrug = false;
        bool draggingKeys = false;
        bool firstMove = false;
        private void TimeLine_MouseDown(object sender, MouseEventArgs e)
        {
            pointDown = e.Location;

            if (e.Y > bottom && e.X < left - 20)
            {
                // Object tree list selection
                int index = (e.Y - bottom) / 16;

                if (e.X < 20)
                {
                    if (lineIds[index] != null)
                    {
                        ToggleItem(lineIds[index]);
                    }
                }
                else
                {
                    if (Control.ModifierKeys != Keys.Control)
                    {
                        Selection.Clear();
                    }
                    TourEditor.CurrentEditor = null;

                    if (lineIds[index] != null)
                    {
                        Selection.Add(lineIds[index]);

                        AnimationTarget at = FindTarget(lineIds[index]);
                        if (at != null)
                        {
                            TourEditor.CurrentEditor = at.Target.GetEditUI();
                        }

                    }

                    if (Control.MouseButtons == System.Windows.Forms.MouseButtons.Right && Tour != null && Tour.CurrentTourStop != null && Tour.CurrentTourStop.AnimationTargets != null && Tour.CurrentTourStop.AnimationTargets.Count > 0 && Selection.Count > 0)
                    {
                        ContextMenuStrip contextMenu = new ContextMenuStrip();

                        ToolStripMenuItem deleteItem = new ToolStripMenuItem(Language.GetLocalizedText(1279, "Remove From Timeline"));
                        ToolStripMenuItem addKey = new ToolStripMenuItem(Language.GetLocalizedText(1280, "Add Keyframe"));
                        ToolStripMenuItem deleteFrames = new ToolStripMenuItem(Language.GetLocalizedText(1281, "Delete Keyframes"));
                        ToolStripMenuItem copyPopertyKeyframes = new ToolStripMenuItem(Language.GetLocalizedText(1370, "Copy Property Keyframes"));
                        ToolStripMenuItem pastePropertyKeyframes = new ToolStripMenuItem(Language.GetLocalizedText(1372, "Paste Keyframes"));
                        pastePropertyKeyframes.Click += new EventHandler(pastePropertyKeyframes_Click);
                        copyPopertyKeyframes.Click += new EventHandler(copyPopertyKeyframes_Click);
                        deleteItem.Click += new EventHandler(deleteItem_Click);
                        deleteFrames.Click += new EventHandler(deleteFrames_Click);
                        addKey.Click += new EventHandler(addKey_Click);
                        contextMenu.Items.Add(addKey);
                        IDataObject dataObject = Clipboard.GetDataObject();
                        
                        
                        if (Selection.Count == 1 && !Selection[0].Contains("\t"))
                        {
                            if (dataObject.GetDataPresent(Key.ClipboardFormatProperties))
                            {
                                contextMenu.Items.Add(pastePropertyKeyframes);
                            }
                            contextMenu.Items.Add(copyPopertyKeyframes);
                        }                       

                        if (!Selection.Contains("Camera") && Selection.Count == 1 && !Selection[0].Contains("\t"))
                        {
                            contextMenu.Items.Add(deleteItem);
                        }
                        contextMenu.Show(Cursor.Position);
                    }
                }
                Refresh();
                return;
            }

            if (e.Y > Height - 8)
            {

                //scrollbarWidth = (int)((Width - left) * Math.Min(1, (secondsOnscreen / slideTime)));
                //scrollbarStart = ScrollPos + left;

                // Page Up
                if (e.X > (scrollbarWidth + scrollbarStart))
                {

                    ScrollPos = Math.Min(ScrollPos + pixelsPerSecond, (Width - left) - scrollbarWidth);

                    Refresh();
                    return;
                }

                if (e.X < scrollbarStart)
                {
                    ScrollPos = Math.Max(0, ScrollPos - pixelsPerSecond);
                    Refresh();
                    return;
                }
                scrolling = true;
                return;
            }

            if (e.Y < bottom && e.X > left)
            {
 
                SetTweenPosition(PixelToTime(e.X + (step / 2)));
                if (Control.MouseButtons == System.Windows.Forms.MouseButtons.Right && Tour != null && Tour.CurrentTourStop != null && Tour.CurrentTourStop.AnimationTargets != null && Tour.CurrentTourStop.AnimationTargets.Count > 0)
                {
                    ContextMenuStrip contextMenu = new ContextMenuStrip();
                    ToolStripMenuItem addKey = new ToolStripMenuItem(Language.GetLocalizedText(1280, "Add Keyframe"));
                    ToolStripMenuItem pasteKeys = new ToolStripMenuItem("Paste");
                    pasteKeys.Click += new EventHandler(pasteKeys_Click);

                    addKey.Click += new EventHandler(addKey_Click);
                    contextMenu.Items.Add(addKey);

                    IDataObject dataObject = Clipboard.GetDataObject();
                    if (dataObject.GetDataPresent(Key.ClipboardFormatSelection))
                    {
                        contextMenu.Items.Add(pasteKeys);
                    }

                    if (dataObject.GetDataPresent(Key.ClipboardFormatColumn))
                    {
                        ToolStripMenuItem pasteColumn = new ToolStripMenuItem(Language.GetLocalizedText(1373, "Paste at current time"));
                        pasteColumn.Click += new EventHandler(pasteColumn_Click);

                        contextMenu.Items.Add(pasteColumn);
                    }

                    contextMenu.Show(Cursor.Position);
                    return;
                }
                else
                {
                    timeDrag = true;
                }
                return;
            }

            if (e.X > left - 20 && e.X < left)
            {
                int y = e.Y - bottom;

                if (y < vScrollBarTop)
                {
                    if (startLine > 0)
                    {
                        startLine--;
                    }
                    Refresh();
                    return;
                }

                if (y > vScrollBarTop + vScrollBarHieght)
                {
                    startLine++;
                    Refresh();
                    return;
                }
                vScroolling = true;
                return;
            }

            if (e.X > left && e.Y > bottom && e.Y < Height - 8)
            {
                if (Control.ModifierKeys != Keys.Control)
                {
                    if (ClickedOnSelection(pointDown))
                    {
                        if (Control.MouseButtons == System.Windows.Forms.MouseButtons.Right && Tour != null && Tour.CurrentTourStop != null && Tour.CurrentTourStop.AnimationTargets != null && Tour.CurrentTourStop.AnimationTargets.Count > 0)
                        {
                            ContextMenuStrip contextMenu = new ContextMenuStrip();

                            ToolStripMenuItem deleteFrames = new ToolStripMenuItem(Language.GetLocalizedText(1281, "Delete Keyframes"));
                            ToolStripMenuItem copyKeys = new ToolStripMenuItem(Language.GetLocalizedText(1371, "Copy Selected Keyframes"));
                            deleteFrames.Click += new EventHandler(deleteFrames_Click);
                            copyKeys.Click += new EventHandler(copyKeys_Click);
                            contextMenu.Items.Add(copyKeys);
                            contextMenu.Items.Add(deleteFrames);
                            contextMenu.Show(Cursor.Position);
                            return;
                        }
                        else
                        {
                            draggingKeys = true;
                            mouseDown = true;
                            firstMove = true;
                            return;
                        }
                    }
                    selectedKeys.Clear();
                    Refresh();
                    if (Control.MouseButtons == System.Windows.Forms.MouseButtons.Right && Tour != null && Tour.CurrentTourStop != null && Tour.CurrentTourStop.AnimationTargets != null && Tour.CurrentTourStop.AnimationTargets.Count > 0)
                    {
                        ContextMenuStrip contextMenu = new ContextMenuStrip();
                        ToolStripMenuItem addKey = new ToolStripMenuItem(Language.GetLocalizedText(1280, "Add Keyframe"));
                        addKey.Click += new EventHandler(addKey_Click);
                        addKey.Enabled = (Selection.Count > 0);
                        contextMenu.Items.Add(addKey);

                        IDataObject dataObject = Clipboard.GetDataObject();
                        if (dataObject.GetDataPresent(Key.ClipboardFormatSelection))
                        {
                            ToolStripMenuItem pasteKeys = new ToolStripMenuItem(Language.GetLocalizedText(425, "Paste"));
                            pasteKeys.Click += new EventHandler(pasteKeys_Click);

                            contextMenu.Items.Add(pasteKeys);
                        }

                        if (dataObject.GetDataPresent(Key.ClipboardFormatColumn))
                        {
                            ToolStripMenuItem pasteColumn = new ToolStripMenuItem(Language.GetLocalizedText(1373, "Paste at current time"));
                            pasteColumn.Click += new EventHandler(pasteColumn_Click);

                            contextMenu.Items.Add(pasteColumn);
                        }


                        contextMenu.Show(Cursor.Position);
                        return;
                    }
                }
                // drag or click to select keys
                selectingKeys = true;
                wasSelectRectDrug = false;
            }

            mouseDown = true;

        }

        void pasteColumn_Click(object sender, EventArgs e)
        {
            PasteKeys(true);
        }

        void pastePropertyKeyframes_Click(object sender, EventArgs e)
        {
            PasteKeys(false);
        }

        private void PasteKeys(bool relative)
        {
            Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(1369, "Paste Keys"), tour));

            IDataObject dataObject = Clipboard.GetDataObject();
            bool missingTargets = false;

            string format = relative ? Key.ClipboardFormatColumn : Key.ClipboardFormatSelection;


            if (dataObject.GetDataPresent(format))
            {
                // add try catch block
                string xml = dataObject.GetData(format) as string;
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                System.Xml.XmlNode node = doc["CopiedKeys"];
                foreach (System.Xml.XmlNode child in node.ChildNodes)
                {
                    string targetID = Selection[0];
                    AnimationTarget target = GetTargetByID(targetID);
                    if (target != null)
                    {
                        target.PasteFromXML(child, relative, tour.CurrentTourStop.TweenPosition);
                    }
                    else
                    {
                        missingTargets = true;
                    }
                }
            }
            Refresh();
            if (missingTargets)
            {
                UiTools.ShowMessageBox("Some keys could not be pasted because corresponding targets were not found");
            }
        }

        void copyPopertyKeyframes_Click(object sender, EventArgs e)
        {
            if (tour != null && tour.CurrentTourStop != null)
            {
                if (Selection.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    using (System.IO.StringWriter textWriter = new System.IO.StringWriter(sb))
                    {
                        using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(textWriter))
                        {
                            writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                            writer.WriteStartElement("CopiedKeys");
                            string targetID = Selection[0];
                            AnimationTarget target = GetTargetByID(targetID);

                            target.SaveToXml(writer);

                            writer.WriteEndElement();
                        }
                    }
                    DataFormats.Format format = DataFormats.GetFormat(Key.ClipboardFormatProperties);

                    IDataObject dataObject = new DataObject();
                    dataObject.SetData(format.Name, false, sb.ToString());

                    Clipboard.SetDataObject(dataObject, false);



                    Refresh();
                }
            }
        }

        void pasteKeys_Click(object sender, EventArgs e)
        {
            Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(1369, "Paste Keys"), tour));

            IDataObject dataObject = Clipboard.GetDataObject();
            bool missingTargets = false;
            if (dataObject.GetDataPresent(Key.ClipboardFormatSelection))
            {
                // add try catch block
                string xml = dataObject.GetData(Key.ClipboardFormatSelection) as string;
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.LoadXml(xml);
                System.Xml.XmlNode node = doc["CopiedKeys"];
                foreach (System.Xml.XmlNode child in node.ChildNodes)
                {
                    string targetID = child.Attributes["TargetID"].Value;
                    AnimationTarget target = GetTargetByID(targetID);
                    if (target != null)
                    {
                        target.PasteFromXML(child, false, 0);
                    }
                    else
                    {
                        missingTargets = true;
                    }
                } 
            }
            Refresh();
            if (missingTargets)
            {
                UiTools.ShowMessageBox("Some keys could not be pasted because corresponding targets were not found");
            }
        }

        AnimationTarget GetTargetByID(string id)
        {
            foreach (AnimationTarget target in tour.CurrentTourStop.AnimationTargets)
            {
                if (target.TargetID == id)
                {
                    return target;
                }
            }
            return null;
        }

        private bool IsColumnSelection()
        {
            int keyTime = -1;

            foreach (VisibleKey vKey in selectedKeys.Values)
            {
                Key key = vKey.Target.GetKey(vKey.ParameterIndex, vKey.Time);
                if (key != null)
                {
                    if (keyTime == -1)
                    {
                        keyTime = KeyGroup.Quant(key.Time);
                    }

                    if (keyTime != KeyGroup.Quant(key.Time))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        void copyKeys_Click(object sender, EventArgs e)
        {
            bool columnSelection = IsColumnSelection();
        

            if (tour != null && tour.CurrentTourStop != null)
            {
                if (selectedKeys.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    using (System.IO.StringWriter textWriter = new System.IO.StringWriter(sb))
                    {
                        using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(textWriter))
                        {
                            writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                            writer.WriteStartElement("CopiedKeys");

                            foreach (AnimationTarget target in tour.CurrentTourStop.AnimationTargets)
                            {
                                target.SaveSelectedToXml(writer, selectedKeys);
                            }

                            writer.WriteEndElement();
                        }
                    }

                    DataFormats.Format format = DataFormats.GetFormat(Key.ClipboardFormatSelection);

                    IDataObject dataObject = new DataObject();
                    dataObject.SetData(format.Name, false, sb.ToString());

                    if (columnSelection)
                    {
                        // Add other data format
                        DataFormats.Format formatColumn = DataFormats.GetFormat(Key.ClipboardFormatColumn);
                        dataObject.SetData(formatColumn.Name, false, sb.ToString());
                    }

                    Clipboard.SetDataObject(dataObject, false);
                    Refresh();
                }
            }
        }


        void addKey_Click(object sender, EventArgs e)
        {
            NewKey();
        }

        private void TimeLine_MouseUp(object sender, MouseEventArgs e)
        {
            scrolling = false;
            timeDrag = false;
            vScroolling = false;

            if (draggingKeys)
            {
                draggingKeys = false;
                ghostingSelection = false;
                DragKeys(e.Location);
                KeyProperties.ShowProperties(selectedKeys);
                Refresh();
            }

            if (selectingKeys)
            {           
                selectingKeys = false;
                if (wasSelectRectDrug == false)
                {
                    if (Control.ModifierKeys != Keys.Control)
                    {
                        selectedKeys.Clear();
                    }
                    Point min = new Point( pointDown.X-5,  pointDown.Y-9);
                    Point max = new Point( pointDown.X+5,  pointDown.Y+5);
                    max.Offset(-min.X, -min.Y);
                    selectRect = new Rectangle(min, new Size(max));

                    foreach (VisibleKey vk in VisibleKeys)
                    {
                        if (selectRect.Contains(vk.Point))
                        {
                            if (!selectedKeys.ContainsKey(vk.IndexKey))
                            {
                                selectedKeys.Add(vk.IndexKey, vk);
                            }
                        }
                    }
                    
                }
                KeyProperties.ShowProperties(selectedKeys);
                this.Focus();
                Refresh();
            }
        }

        Rectangle selectRect = new Rectangle();

        Point lastMouse = new Point();

        private void TimeLine_MouseMove(object sender, MouseEventArgs e)
        {
         
            if (draggingKeys)
            {
                if (firstMove)
                {
                    firstMove = false;
                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1282, "Drag Keys on Timeline"), tour));
                }

                DragGhostKeys(e.Location);

                return;
            }

            if (timeDrag)
            {
                double pos = PixelToTime(e.X + (step / 2));
                if (pos != TweenPosition)
                {
                    SetTweenPosition(pos);
                    Earth3d.MainWindow.Render();
                    int oldScroll = offset + (pixelsPerSecond * startSecond);
                    EnsureVisible();
                    int newScroll = offset + (pixelsPerSecond * startSecond);
                    if (oldScroll != newScroll)
                    {
                        Point pnt = Cursor.Position;
                        Cursor.Position = new Point(pnt.X - Math.Max(-(Width-left),Math.Min(Width-left,(newScroll - oldScroll))), pnt.Y);
                    }
                }
            }

            if (selectingKeys)
            {
                selectedKeys.Clear();
                Point min = new Point(Math.Min(e.X, pointDown.X), Math.Min(e.Y, pointDown.Y));
                Point max = new Point(Math.Max(e.X, pointDown.X), Math.Max(e.Y, pointDown.Y));
                max.Offset(-min.X, -min.Y);
                selectRect = new Rectangle(min, new Size(max));

                foreach (VisibleKey vk in VisibleKeys)
                {
                    if (selectRect.Contains(vk.Point))
                    {
                        if (!selectedKeys.ContainsKey(vk.IndexKey))
                        {
                            selectedKeys.Add(vk.IndexKey, vk);
                        }
                    }
                }
                Refresh();
                wasSelectRectDrug = true;
                return;
            }

            if (vScroolling)
            {
                int dragDist = pointDown.Y - e.Y;
                double t = (double)vScrollBarHieght / (int)((Height - bottom) / 16);

                int m = (int)(dragDist / t);

                if (m != 0)
                {
                    startLine = Math.Min(this.totalLines - displayLines, Math.Max(0, (int)(startLine - m)));
                    Refresh();
                    pointDown = e.Location;
                    return;
                }

            }

            if (scrolling)
            {
                int dragDist = pointDown.X - e.X;
                if (dragDist != 0)
                {

                    ScrollPos = Math.Max(0, Math.Min(ScrollPos - dragDist, (Width - left) - scrollbarWidth));
                    Refresh();
                }
                pointDown = e.Location;
                return;
            }

            lastMouse = e.Location;
            lastMove = DateTime.Now;
           
        }

        DateTime lastMove = DateTime.Now;

        private void DragKeys(Point point)
        {
            bool collide = false;
            ghostingSelection = false;
            double first = 1;
            double last = 0;
            FindRange(ref first, ref last);

            Dictionary<string, VisibleKey> newSelection = new Dictionary<string, VisibleKey>();

            double moveDist = PixelToTime(point.X) - PixelToTime(pointDown.X);
            moveDist = Math.Min(1 - last, moveDist);

            moveDist = Math.Max(0 - first, moveDist);

            if (moveDist != 0)
            {
                foreach (VisibleKey vKey in selectedKeys.Values)
                {
                    Key key = vKey.Target.GetKey(vKey.ParameterIndex, vKey.Time);
                    if (key.Time != 0)
                    {
                        collide = collide | vKey.Target.MoveKey(vKey.ParameterIndex, key.Time, key.Time + moveDist);

                        vKey.Time =key.Time;
                    }
                    newSelection.Add(vKey.IndexKey, vKey);
                }

                if (collide)
                {
                    UiTools.ShowMessageBox(Language.GetLocalizedText(1283, "At least one key in the timeline was overwritten by the moved key(s). Use ctrl-Z to undo if this was not desired.z"));
                }
            }

            selectedKeys = newSelection;
        }

        bool ghostingSelection = false;
        private void DragGhostKeys(Point point)
        {
            double first = 1;
            double last = 0;
            FindRange(ref first, ref last);
            ghostingSelection = true;
            double moveDist = PixelToTime(point.X) - PixelToTime(pointDown.X);

            moveDist = Math.Min(1 - last, moveDist);

            moveDist = Math.Max(0 - first, moveDist);

            if (moveDist != 0)
            {
                foreach (VisibleKey vKey in selectedKeys.Values)
                {
                    Key key = vKey.Target.GetKey(vKey.ParameterIndex, vKey.Time);
                    if (key.Time != 0)
                    {
                        key.GhostTime = key.Time + moveDist;
                    }
                }
                Refresh();

            }

            int oldScroll = offset + (pixelsPerSecond * startSecond);
            EnsureVisible(PixelToTime(point.X));
            int newScroll = offset + (pixelsPerSecond * startSecond);
            if (oldScroll != newScroll)
            {
                Point pnt = Cursor.Position;
                Cursor.Position = new Point(pnt.X - Math.Max(-(Width - left), Math.Min(Width - left, (newScroll - oldScroll))), pnt.Y);
                pointDown.X -= Math.Max(-(Width - left), Math.Min(Width - left, (newScroll - oldScroll)));
            }

        }

        private void FindRange(ref double first, ref double last)
        {

            foreach (VisibleKey vKey in selectedKeys.Values)
            {
                Key key = vKey.Target.GetKey(vKey.ParameterIndex, vKey.Time);
                if (key.Time < first)
                {
                    first = Math.Max(0, key.Time);
                }
                if (key.Time > last)
                {
                    last = Math.Min(1, key.Time);
                }
            }
        }

        bool ClickedOnSelection(Point pntDown)
        {
            Point min = new Point(pntDown.X - 5, pntDown.Y - 9);
            Point max = new Point(pntDown.X + 5, pntDown.Y + 5);
            max.Offset(-min.X, -min.Y);
            selectRect = new Rectangle(min, new Size(max));

            foreach (VisibleKey vk in VisibleKeys)
            {
                if (selectRect.Contains(vk.Point))
                {
                    if (selectedKeys.ContainsKey(vk.IndexKey))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        VisibleKey HoverOnSelection(Point pntDown)
        {
            Point min = new Point(pntDown.X - 5, pntDown.Y - 9);
            Point max = new Point(pntDown.X + 5, pntDown.Y + 5);
            max.Offset(-min.X, -min.Y);
            selectRect = new Rectangle(min, new Size(max));

            foreach (VisibleKey vk in VisibleKeys)
            {
                if (selectRect.Contains(vk.Point))
                {
                    return vk;
                }
            }

            return null;
        }


        void deleteFrames_Click(object sender, EventArgs e)
        {
            DeleteKey();
        }

        void deleteItem_Click(object sender, EventArgs e)
        {
            if (tour != null && tour.CurrentTourStop != null)
            {
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1279, "Remove From Timeline"), tour));
                foreach (string id in Selection)
                {
                    RemoveItem(id);
                }

                selectedKeys.Clear();
                Selection.Clear();
                Refresh();
                KeyProperties.ShowProperties(selectedKeys);
            }
        }

        private void RemoveItem(string id)
        {
            if (Tour != null)
            {
                for (int i = tour.CurrentTourStop.AnimationTargets.Count - 1; i > -1; i--)
                {
                    if (tour.CurrentTourStop.AnimationTargets[i].TargetID == id)
                    {
                        Overlay overlay = tour.CurrentTourStop.AnimationTargets[i].Target as Overlay;
                        if (overlay != null)
                        {
                            overlay.AnimationTarget = null;
                        }

                        tour.CurrentTourStop.AnimationTargets.RemoveAt(i);
                    }
                }
            }
        }

        private void ToggleItem(string id)
        {
            if (Tour != null)
            {
                foreach (AnimationTarget target in tour.CurrentTourStop.AnimationTargets)
                {
                    if (target.Target.GetIndentifier() == id)
                    {
                        target.Expanded = !target.Expanded;
                        Selection.Clear();
                        Selection.Add(id);
                        return;
                    }
                }
            }
        }

        private void EnsureVisible()
        {
            int pos = TimeToPixel(TweenPosition);

            if (scrollbarWidth < (Width - left))
            {
                int looped = 50;
                while ((pos < left || pos > Width) && looped > 0)
                {
                    looped--;
                    if (pos > left)
                    {
                        ScrollPos = Math.Max(0, Math.Min(ScrollPos + scrollbarWidth, (Width - left) - scrollbarWidth));
                        Refresh();
                    }

                    if (pos < left)
                    {
                       ScrollPos = Math.Max(0, Math.Min(ScrollPos - scrollbarWidth, (Width - left) - scrollbarWidth));
                       Refresh();
                    }
                    pos = TimeToPixel(TweenPosition);
                }
            }
        }

        private void EnsureVisible(double timePos)
        {
            int pos = TimeToPixel(timePos);

            if (scrollbarWidth < (Width - left))
            {
                int looped = 50;
                while ((pos < left || pos > Width) && looped > 0)
                {
                    looped--;
                    if (pos > left)
                    {
                        ScrollPos = Math.Max(0, Math.Min(ScrollPos + scrollbarWidth, (Width - left) - scrollbarWidth));
                        Refresh();
                    }

                    if (pos < left)
                    {
                        ScrollPos = Math.Max(0, Math.Min(ScrollPos - scrollbarWidth, (Width - left) - scrollbarWidth));
                        Refresh();
                    }
                    pos = TimeToPixel(timePos);
                }
            }
        }

        private void SelectAllKeys()
        {
            this.selectedKeys.Clear();

            if (Tour != null && tour.CurrentTourStop != null)
            {
                foreach (AnimationTarget target in tour.CurrentTourStop.AnimationTargets)
                {
                    for (int i = 0; i < target.ParameterNames.Count; i++)
                    {
                        foreach (Key key in target.KeyFrames[i].Keys.Values)
                        {
                            VisibleKey vk = new VisibleKey(target, i, target.ParameterNames[i], key.Time, new Point(0, 0));
                            selectedKeys.Add(vk.IndexKey, vk);
                        }
                    }
                }
            }
            KeyProperties.ShowProperties(selectedKeys);
            Invalidate();
        }

        private void SelectColumnKeys()
        {
            this.selectedKeys.Clear();

            if (Tour != null && tour.CurrentTourStop != null)
            {
                foreach (AnimationTarget target in tour.CurrentTourStop.AnimationTargets)
                {
                    for (int i = 0; i < target.ParameterNames.Count; i++)
                    {
                        foreach (Key key in target.KeyFrames[i].Keys.Values)
                        {
                            if (KeyGroup.Quant(key.Time) == KeyGroup.Quant(TweenPosition))
                            {
                                VisibleKey vk = new VisibleKey(target, i, target.ParameterNames[i], key.Time, new Point(0, 0));

                                selectedKeys.Add(vk.IndexKey, vk);
                            }
                        }
                    }
                }
            }
            KeyProperties.ShowProperties(selectedKeys);
            Invalidate();
        }

        private void NextKey()
        {
            int currentTime = KeyGroup.Quant(this.QuantizeTimeToFrame( Tour.CurrentTourStop.TweenPosition + frameTime));
            int closestMatch = KeyGroup.Quant(1);
            double closestTime = 1;
            if (Tour != null && tour.CurrentTourStop != null)
            {
                foreach (AnimationTarget target in tour.CurrentTourStop.AnimationTargets)
                {
                    for (int i = 0; i < target.ParameterNames.Count; i++)
                    {
                        foreach (Key key in target.KeyFrames[i].Keys.Values)
                        {
                            if (KeyGroup.Quant(key.Time) > currentTime && KeyGroup.Quant(key.Time) < closestMatch)
                            {
                                closestMatch = KeyGroup.Quant(key.Time);
                                closestTime = key.Time;
                            }

                            if (key.Time > closestMatch)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            SetTweenPosition(closestTime);
            EnsureVisible();
            Invalidate();
        }

        private void PreviousKey()
        {
            int currentTime = KeyGroup.Quant(Tour.CurrentTourStop.TweenPosition);
            int closestMatch = 0;
            double closestTime = 0;

            if (Tour != null && tour.CurrentTourStop != null)
            {
                foreach (AnimationTarget target in tour.CurrentTourStop.AnimationTargets)
                {
                    for (int i = 0; i < target.ParameterNames.Count; i++)
                    {
                        foreach (Key key in target.KeyFrames[i].Keys.Values)
                        {
                            if (KeyGroup.Quant(key.Time) < currentTime && KeyGroup.Quant(key.Time) >= closestMatch)
                            {
                                closestMatch = KeyGroup.Quant(key.Time);
                                closestTime = key.Time;
                            }

                            if (KeyGroup.Quant(key.Time) > currentTime)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            SetTweenPosition(closestTime);
            EnsureVisible();
            Invalidate();
        }


        private void End_Click(object sender, EventArgs e)
        {
            SetTweenPosition(lastKeyTime);
            ScrollPos = (Width - left) - scrollbarWidth;
            Refresh();
        }

        private void Forward_Click(object sender, EventArgs e)
        {
            Earth3d.MainWindow.TourEdit.TourEditorUI.StartScrubbing(false);
        }

        private void Pause_Click(object sender, EventArgs e)
        { 
           Earth3d.MainWindow.TourEdit.TourEditorUI.PauseScrubbing(true);
        }
 
        private void Back_Click(object sender, EventArgs e)
        {
            Earth3d.MainWindow.TourEdit.TourEditorUI.StartScrubbing(true);
        }

        private void Start_Click(object sender, EventArgs e)
        {
            SetTweenPosition(0);
            ScrollPos = 0;
            Refresh();
        }

        private void TimeLine_Load(object sender, EventArgs e)
        {
            instances.Add(this);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.X < left)
            {
                if (vScrollBarShown)
                {
                    // Scroll the tree
                    if (e.Delta > 0)
                    {
                        if (startLine > 0)
                        {
                            startLine--;
                        }
                        Refresh();
                        return;
                    }

                    if (e.Delta < 0)
                    {
                        startLine = Math.Min(this.totalLines - displayLines, Math.Max(0, (int)(startLine + 1)));
                        Refresh();
                        return;
                    }
                }
                return;
            }
            else if (e.Y < bottom)
            {
                if (e.Delta < 0)
                {
                    tick = Math.Max(2, tick - 1);
                    Refresh();
                    return;
                }

                if (e.Delta > 0)
                {
                    tick = Math.Min(20, tick + 1);
                    Refresh();
                    return;
                }
            }
            else
            {
                // scroll the timeline

                // Page Up
                if (e.Delta < 0)
                {
                    ScrollPos = Math.Min(ScrollPos + pixelsPerSecond, (Width - left) - scrollbarWidth);
                    Refresh();
                    return;
                }

                if (e.Delta > 0)
                {
                    ScrollPos = Math.Max(0, ScrollPos - pixelsPerSecond);
                    Refresh();
                    return;
                }
            }



            //base.OnMouseWheel(e);
        }

        private AnimationTarget FindTarget(string id)
        {
            if (Tour != null)
            {
                return tour.CurrentTourStop.FindTarget(id);
            }
            return null;
        }

        private void AddKey_Click(object sender, EventArgs e)
        {
            NewKey();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //return base.ProcessCmdKey(ref msg, keyData);

            if ((keyData & Keys.Control) == Keys.Control)
            {
                keyData = keyData & (~Keys.Control);

                switch (keyData)
                {
                    case Keys.Z:
                        if (Earth3d.MainWindow.TourEdit != null)
                        {
                            Earth3d.MainWindow.TourEdit.UndoStep();
                        }
                        break;
                    case Keys.Y:
                        if (Earth3d.MainWindow.TourEdit != null)
                        {
                            Earth3d.MainWindow.TourEdit.RedoStep();
                        }
                        break;
                    case Keys.A:
                        SelectAllKeys();
                        break;
                    case Keys.Space:
                        SelectColumnKeys();
                        break;
                    case Keys.Right:
                        break;
                    case Keys.Left:
                        break;

                }
            }
            else if ((keyData & Keys.Alt) == Keys.Alt)
            {
                keyData = keyData & (~Keys.Alt);
                switch (keyData)
                {
                    case Keys.Right:
                        NextKey();
                        break;
                    case Keys.Left:
                        PreviousKey();
                        break;
                }
            }
            else
            {
                switch (keyData)
                {
                    case Keys.F1:
                        // Launch Help for Timeline
                        WebWindow.OpenUrl("http://www.worldwidetelescope.org/Learn/Authoring#timelineeditor", true);
                        break;
                    case Keys.Subtract:
                    case Keys.Delete:
                        DeleteKey();
                        break;
                    case Keys.Insert:
                    case Keys.Add:
                        NewKey();
                        break;
                    case Keys.Space:
                        break;
                    case Keys.Left:
                        LastFrame();
                        break;
                    case Keys.Right:
                        NextFrame();

                        break;

                }
            }
            return false;
        }

        public double QuantizeTimeToFrame(double time)
        {
            int frames = (int)((time * slideTime * 30) + .5);
            return (double)frames / (double) totalFrames;
        }

        private void LastFrame()
        {
            double newTween = QuantizeTimeToFrame(TweenPosition - frameTime);
 
            double dd = frameTime * 300;

            if (newTween < 0)
            {
                newTween = 0;
            }

            SetTweenPosition(newTween);
            EnsureVisible();
        }

        private void NextFrame()
        {
            double newTween = QuantizeTimeToFrame(TweenPosition + frameTime );
 
            double dd = newTween - TweenPosition;

            if (newTween > 1)
            {
                newTween = 1;
            }

            SetTweenPosition(newTween);
            EnsureVisible();
        }
        


        private void NewKey()
        {
            if (tour != null && tour.CurrentTourStop != null)
            {
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1284, "Add Keyframes"), tour));
                foreach (string id in Selection)
                {
                    NewKey(id);
                }
                Refresh();
            }
        }

        Key.KeyType CurrentKeyInType = Key.KeyType.Linear;
        Key.KeyType CurrentKeyOutType = Key.KeyType.Linear;

        private void NewKey(string id)
        {
            AnimationTarget target = null;
            if (id.Contains("\t"))
            {
                // Child
                string[] parts = id.Split(new char[] { '\t' });

                target = FindTarget(parts[0]);
                if (target != null)
                {
                    for (int i = 0; i < target.ParameterNames.Count; i++)
                    {
                        if (target.ParameterNames[i] == parts[1])
                        {
                            target.SetKeyFrame(i, Tour.CurrentTourStop.TweenPosition, CurrentKeyInType);
                            return;
                        }
                    }
                }

            }
            else
            {
                //parent
                target = FindTarget(id);

                if (target != null)
                {
                    target.SetKeyFrame(Tour.CurrentTourStop.TweenPosition, CurrentKeyInType);
                }
            }
        }

        private void DelKey_Click(object sender, EventArgs e)
        {
            DeleteKey();
        }

        private void DeleteKey()
        {
            if (tour != null && tour.CurrentTourStop != null)
            {
                if (selectedKeys.Count > 0)
                {
                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1281, "Delete Keyframes"), tour));
                    foreach (VisibleKey vk in selectedKeys.Values)
                    {
                        DeleteKey(vk);
                    }
                    Refresh();
                }
            }
            selectedKeys.Clear();
            KeyProperties.ShowProperties(selectedKeys);
        }

        private void DeleteKey(VisibleKey vk)
        {     
            AnimationTarget target = null;
            target = vk.Target;
            if (target != null)
            {
                int index = vk.ParameterIndex;
                target.DeleteKey(index, vk.Time);
            }
        }

        private void TimeLine_ControlAdded(object sender, ControlEventArgs e)
        {
            instances.Add(this);
        }

        private void TimeLine_ControlRemoved(object sender, ControlEventArgs e)
        {
            RemoveInstance(this);
        }

        public static void RemoveInstance(TimeLine timeline)
        {
            if (instances.Contains(timeline))
            {
                instances.Remove(timeline);
            }
        }

        private void TimeLine_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void TimeLine_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }



        private void TimeLine_MouseHover(object sender, EventArgs e)
        {

               
        }

        private void hoverTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = DateTime.Now - lastMove;

            if (ts.TotalMilliseconds > 500)
            {
                lastMove = DateTime.Now + new TimeSpan(100, 0, 0, 0);
                Point pnt = lastMouse;

                if (pnt.X > left && pnt.Y > bottom && pnt.Y < Height - 8)
                {
                    VisibleKey vk = HoverOnSelection(pnt);
                    if (vk != null)
                    {
                        //keyTips = new ToolTip();
                        //keyTips.InitialDelay = 1;
                        //keyTips.ReshowDelay = 1;
                        //keyTips.ShowAlways = true;
                        //keyTips.UseAnimation = true;
                        //keyTips.UseFading = true;
                        //keyTips.Active = true;
                        toolTip.Show(vk.PropertyName, this, pnt, 4000);
                        //toolTip.Show(vk.PropertyName, this);


                        return;
                    }
                }
            }
        }
    }
}

