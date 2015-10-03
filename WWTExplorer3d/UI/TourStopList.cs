using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public delegate void TourStopClickedEventHandler(object sender, TourStop e);

    public partial class TourStopList : UserControl
    {
        public event TourStopClickedEventHandler ItemHover;
        public event TourStopClickedEventHandler ItemClicked;
        public event TourStopClickedEventHandler ItemDoubleClicked;
        public event TourStopClickedEventHandler AddNewSlide;
        public event TourStopClickedEventHandler ShowEndPosition;
        public event TourStopClickedEventHandler ShowStartPosition;

        public TourStopList()
        {
            InitializeComponent();
            if (Height < 75)
            {
                Height = 75;
            }
            //Items = new List<TourStop>();
            SetStyle(ControlStyles.ResizeRedraw, true);

        }


        ThumbnailSize thumbnailSize = ThumbnailSize.Small;

        public ThumbnailSize ThumbnailSize
        {
            get { return thumbnailSize; }
            set
            {
                thumbnailSize = value;
                switch (value)
                {
                    case ThumbnailSize.Big:
                        HorzSpacing = 180;
                        VertSpacing = 85;
                        ThumbHeight = 75;
                        ThumbWidth = 180;
                        break;
                    case ThumbnailSize.Small:
                        HorzSpacing = 144;
                        VertSpacing = 85;
                        ThumbHeight = 75;
                        ThumbWidth = 110;
                        break;
                }
                UpdatePaginator();
                Refresh();
            }
        }

        public TourDocument Tour = null;

        private List<TourStop> Items
        {
            get
            {
                if (Tour != null)
                {
                    return Tour.TourStops;
                }
                else
                {
                    return new List<TourStop>();
                }
            }
        }


        static readonly Bitmap bmpBackground = global::TerraViewer.Properties.Resources.thumbBackground;
        static readonly Bitmap bmpBackgroundHover = global::TerraViewer.Properties.Resources.ThumbBackgroundHover;
        static readonly Bitmap bmpBackgroundWide = global::TerraViewer.Properties.Resources.thumbBackgroundWide;
        static readonly Bitmap bmpBackgroundWideHover = global::TerraViewer.Properties.Resources.ThumbBackgroundWideHover;
        static readonly Bitmap bmpDropInsertMarker = global::TerraViewer.Properties.Resources.DragInsertMarker;
        static readonly Bitmap bmpMasterMarker = global::TerraViewer.Properties.Resources.master;

        static readonly Bitmap bmpScrollBackLeft = global::TerraViewer.Properties.Resources.scroll_background_left;
        static readonly Bitmap bmpScrollBackMiddle = global::TerraViewer.Properties.Resources.scroll_background_middle;
        static readonly Bitmap bmpScrollBackRight = global::TerraViewer.Properties.Resources.scroll_background_right;

        static readonly Bitmap bmpScrollBarLeft = global::TerraViewer.Properties.Resources.scroll_bar_left;
        static readonly Bitmap bmpScrollBarMiddle = global::TerraViewer.Properties.Resources.scroll_bar_middle;
        static readonly Bitmap bmpScrollBarRight = global::TerraViewer.Properties.Resources.scroll_bar_right;

        static readonly Bitmap bmpPunchInOutBlock = global::TerraViewer.Properties.Resources.PunchInOutBlock;
        static readonly Bitmap bmpPunchIn = global::TerraViewer.Properties.Resources.PunchIn;
        static readonly Bitmap bmpPunchOut = global::TerraViewer.Properties.Resources.PunchOut;

        static readonly Bitmap transCrossFade = global::TerraViewer.Properties.Resources.TransCrossfade;
        static readonly Bitmap TransCut = global::TerraViewer.Properties.Resources.TransCut;
        static readonly Bitmap TransFadeIn = global::TerraViewer.Properties.Resources.TransFadeIn;
        static readonly Bitmap TransFadeOut = global::TerraViewer.Properties.Resources.TransFadeOut;
        static readonly Bitmap TransFadeOutIn = global::TerraViewer.Properties.Resources.TransFadeOutIn;
        static readonly Bitmap TransArrow = global::TerraViewer.Properties.Resources.TransArrow;
        static readonly Bitmap TransHighlight = global::TerraViewer.Properties.Resources.TransHighlight;

        public void PageChanged(object sender, PageChange e)
        {
            if (e == PageChange.Back)
            {
                if (startIndex >= ItemsPerPage)
                {
                    startIndex -= ItemsPerPage;
                }
                else
                {
                    startIndex = 0;
                }
            }
            if (e == PageChange.First)
            {
                startIndex = 0;
            }

            if (e == PageChange.Next)
            {
                if ((startIndex + ItemsPerPage) <= Items.Count)
                {
                    startIndex += ItemsPerPage;
                }
            }

            if (e == PageChange.Last)
            {
                startIndex = ((Items.Count - 1) / ItemsPerPage) * ItemsPerPage;
            }
            Refresh();
        }
        int totalItems;

        public void EnsureAddVisible()
        {
            if ((Items.Count >= ItemsPerPage))
            {
                startIndex = (Items.Count - ItemsPerPage) + 1;
            }
            Refresh();
        }

        public void EnsureSelectedVisible()
        {
            if ((Items.Count > ItemsPerPage))
            {
                if (selectedItem > ItemsPerPage / 2)
                {
                    startIndex = selectedItem - ItemsPerPage / 2;
                }
            }
            Refresh();
        }

        public int FindItem(TourStop ts)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i] == ts)
                {
                    return i;
                }
            }
            return -1;
        }

        public int TotalItems
        {
            get { return totalItems; }
            set
            {
                if (totalItems != value)
                {
                    totalItems = value;
                    UpdatePaginator();
                }
            }
        }

        int rowCount = 1;

        public int RowCount
        {
            get { return rowCount; }
            set
            {
                if (rowCount != value)
                {
                    rowCount = value;
                    UpdatePaginator();
                }
            }
        }
        int colCount = 6;

        public int ColCount
        {
            get { return colCount; }
            set
            {
                if (colCount != value)
                {
                    colCount = value;
                    UpdatePaginator();
                }
            }
        }

        private bool multiSelectMode;

        public bool AllowMultipleSelection
        {
            get { return multiSelectMode; }
            set { multiSelectMode = value; }
        }

        private bool multipleSelection;

        public bool MultipleSelection
        {
            get { return multipleSelection; }
            set { multipleSelection = value; }
        }


        public void SelectAll()
        {
            if (multiSelectMode)
            {
                SelectedItems.Clear();
                for (var i = 0; i < Items.Count; i++)
                {
                    SelectedItems.Add(i, Items[i]);
                    selectedItem = i;
                }

                if (Items.Count > 0)
                {
                    if (ItemClicked != null)
                    {
                        ItemClicked.Invoke(this, Items[0]);
                    }
                }
                Refresh();
            }
        }

        public SortedList<int, TourStop> SelectedItems = new SortedList<int, TourStop>();


        int startIndex;

        int selectedItem = -1;

        public int SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    SelectedItems.Clear();
                    if (selectedItem > -1)
                    {
                        SelectedItems.Add(selectedItem, Items[selectedItem]);
                    }
                    Refresh();
                }
            }
        }
        int hoverItem = -1;

        public int ItemsPerPage
        {
            get
            {
                return rowCount * colCount;
            }
        }



        public int CurrentPage
        {
            get
            {
                return startIndex / ItemsPerPage;
            }
        }


        public int PageCount
        {
            get
            {
                return Math.Max(1, ((Items.Count + ItemsPerPage)) / ItemsPerPage);
            }
        }

        Paginator paginator;

        public Paginator Paginator
        {
            get
            {
                return paginator;
            }
            set
            {
                if (paginator != null)
                {
                    paginator.PageChanged -= new PageChangedEventHandler(PageChanged);
                }

                paginator = value;

                if (paginator != null)
                {
                    paginator.PageChanged += new PageChangedEventHandler(PageChanged);
                }
            }
        }

        int HorzSpacing = 110;
        int VertSpacing = 75;
        int ThumbHeight = 65;
        int ThumbWidth = 110;
        float horzMultiple = 110;
        bool TransitionHighlighted;

        private void TourStopList_Paint(object sender, PaintEventArgs e)
        {
            multipleSelection = SelectedItems.Count > 1;
            if (selectedItem != -1 && Items.Count > 0 && SelectedItems.Count == 0)
            {
                selectedItem = Math.Min(selectedItem, Items.Count - 1);
                SelectedItems.Add(selectedItem, Items[selectedItem]);
            }


            var g = e.Graphics;
            RowCount = Math.Max(Height / ThumbHeight, 1);
            ColCount = Math.Max(Width / HorzSpacing, 1);
            TotalItems = Items.Count;

            horzMultiple = HorzSpacing;

            var index = startIndex;
            var p = new Pen(Color.FromArgb(62, 73, 92));
            g.DrawRectangle(p, new Rectangle(0, 0, Width - 1, Height - 1));
            p.Dispose();

            // Draw scrollbar
            g.DrawImageUnscaled(bmpScrollBackLeft, 0, 0);
            for (var j = 10; j < Width - 10; j += 50)
            {
                g.DrawImageUnscaled(bmpScrollBackMiddle, j, 0);
            }
            g.DrawImageUnscaled(bmpScrollBackRight, Width - 10, 0);
            var scrollbarWidth = Width;
            var scrollbarStart = 0;

            scrollbarWidth = Math.Max(14, (int)(Math.Min(1.0f, (float)ItemsPerPage / (float)(TotalItems + 1)) * (float)Width));
            scrollbarStart = (int)(((float)startIndex / (float)(TotalItems + 1)) * (float)Width);


            g.DrawImageUnscaled(bmpScrollBarLeft, scrollbarStart, 1);
            for (var j = scrollbarStart + 10; j < (scrollbarWidth + scrollbarStart) - 10; j += 50)
            {
                g.DrawImageUnscaledAndClipped(bmpScrollBarMiddle, new Rectangle(j, 1, Math.Min(50, ((scrollbarWidth + scrollbarStart) - j) - 10), 6));
            }
            g.DrawImageUnscaled(bmpScrollBarRight, (scrollbarStart + scrollbarWidth) - 10, 1);



            for (var y = 0; y < rowCount; y++)
            {
                for (var x = 0; x < colCount; x++)
                {
                    var rectf = new RectangleF((float)x * horzMultiple + 47f, y * VertSpacing + 20, ThumbWidth - 10, 64);
                    var textBrush = UiTools.StadardTextBrush;

                    if (index >= Items.Count)
                    {
                        if (this.dragDropItemLocation == -2)
                        {
                            g.DrawImage(bmpDropInsertMarker, (int)((float)x * horzMultiple) + 34, (int)((float)y * VertSpacing) + 20);
                            //g.DrawRectangle(Pens.Yellow, new Rectangle((int)((float)x * horzMultiple), (int)((float)y * VertSpacing + 1), 1, VertSpacing - 22));
                        }

                        if (showAddButton)
                        {
                            if (addButtonHover && !dragging)
                            {
                                g.DrawImage(thumbnailSize == ThumbnailSize.Big ? bmpBackgroundWideHover : bmpBackgroundHover, (int)((float)x * horzMultiple + 44), y * VertSpacing + 20);
                                textBrush = UiTools.YellowTextBrush;
                            }
                            else
                            {
                                g.DrawImage(thumbnailSize == ThumbnailSize.Big ? bmpBackgroundWide : bmpBackground, (int)((float)x * horzMultiple + 44), y * VertSpacing + 20);
                            }

                            // Changed + 33f to + 21f to accomodate slightly longer "add new slide" string: Peter

                            rectf = new RectangleF((float)x * horzMultiple + 55f, y * VertSpacing, ThumbWidth - 17, 62);

                            // Changed "Add a slide" to "Add New Slide" in order to pick up a localized string: Peter

                            g.DrawString(Language.GetLocalizedText(426, "Add New Slide"), UiTools.StandardRegular, textBrush, rectf, UiTools.StringFormatThumbnails);
                        }
                        break;
                    }
                    // Removed hover because of confusion with selection and current slide
                    //if (index == hoverItem || (index == selectedItem && hoverItem == -1))
                    //                   if (index == selectedItem && !dragging && !TransitionHighlighted)
                    if (SelectedItems.ContainsKey(index) && !dragging && (!TransitionHighlighted || multipleSelection))
                    {
                        g.DrawImage(thumbnailSize == ThumbnailSize.Big ? bmpBackgroundWideHover : bmpBackgroundHover, (int)((float)x * horzMultiple + 44), y * VertSpacing + 20);
                        textBrush = UiTools.YellowTextBrush;
                        if (Items[index].Owner.EditMode && !multipleSelection)
                        {
                            g.DrawImage(bmpPunchInOutBlock, (int)((float)x * horzMultiple + 44), y * VertSpacing + 10);

                            if (Items[index].TweenPosition < .5f)
                            {
                                g.DrawImage(bmpPunchIn, (int)((float)x * horzMultiple + 44), y * VertSpacing + 10);
                            }
                            else
                            {
                                g.DrawImage(bmpPunchOut, (int)((float)x * horzMultiple + 135), y * VertSpacing + 10);
                            }
                        }
                    }
                    else
                    {
                        g.DrawImage(thumbnailSize == ThumbnailSize.Big ? bmpBackgroundWide : bmpBackground, (int)((float)x * horzMultiple + 44), y * VertSpacing + 20);
                    }

                    if (index == selectedItem && !dragging && TransitionHighlighted && !multipleSelection)
                    {
                        g.DrawImage(TransHighlight, (int)((float)x * horzMultiple + 3), y * VertSpacing + 20);
                    }

                    Bitmap bmpTrans = null;

                    switch (Items[index].Transition)
                    {
                        default:
                        case TransitionType.Slew:
                            bmpTrans = TransArrow;
                            break;
                        case TransitionType.CrossCut:
                            bmpTrans = TransCut;
                            break;
                        case TransitionType.CrossFade:
                            bmpTrans = transCrossFade;
                            break;
                        case TransitionType.FadeOut:
                            bmpTrans = TransFadeOut;
                            break;
                        case TransitionType.FadeIn:
                            bmpTrans = TransFadeIn;
                            break;
                        case TransitionType.FadeOutIn:
                            bmpTrans = TransFadeOutIn;
                            break;
                    }

                    g.DrawImage(bmpTrans, (int)((float)x * horzMultiple + 5), y * VertSpacing + 22);


                    try
                    {
                        g.DrawImage(Items[index].Thumbnail, (int)((float)x * horzMultiple) + 46, y * VertSpacing + 23);
                        g.DrawRectangle(Pens.Black, (int)((float)x * horzMultiple + 10) + 36, y * VertSpacing + 23, Items[index].Thumbnail.Width, Items[index].Thumbnail.Height);

                    }
                    // TODO FIX this! 
                    catch
                    {
                    }

                    if (Items[index].MasterSlide)
                    {
                        g.DrawImage(bmpMasterMarker, (int)((float)x * horzMultiple + 46), y * VertSpacing + 20);
                    }


                    var rectTime = new RectangleF((float)x * horzMultiple + 47f, y * VertSpacing + 87, ThumbWidth - 10, 15);
                    if (Items[index].Description != null)
                    {
                        g.DrawString(Items[index].Description, UiTools.StandardRegular, textBrush, rectf, UiTools.StringFormatThumbnails);
                    }
                    var duration = Items[index].Duration;
                    var durationString = String.Format("{0}:{1:00}.{2}", duration.Minutes, duration.Seconds, duration.Milliseconds / 10);

                    g.DrawString(durationString, UiTools.StandardRegular, textBrush, rectTime, UiTools.StringFormatBottomCenter);

                    if (index == this.dragDropItemLocation)
                    {
                        g.DrawImage(bmpDropInsertMarker, (int)((float)x * horzMultiple) + 34, (int)((float)y * VertSpacing) + 20);
                        //g.DrawRectangle(Pens.Yellow, new Rectangle((int)((float)x * horzMultiple), (int)((float)y * VertSpacing + 1), 1, VertSpacing - 22));
                    }

                    if (Properties.Settings.Default.ShowSlideNumbers)
                    {
                        var rectSlideNumber = new RectangleF((float)x * horzMultiple + 47f, y * VertSpacing + 7, ThumbWidth - 10, 15);


                        g.DrawString(index.ToString(), UiTools.StandardRegular, textBrush, rectSlideNumber, UiTools.StringFormatBottomCenter);
                    }

                    index++;
                }
                if (index >= Items.Count)
                {
                    break;
                }
            }
        }

        bool showAddButton = true;

        public bool ShowAddButton
        {
            get { return showAddButton; }
            set
            {
                showAddButton = value;
                this.Refresh();
            }
        }

        bool addButtonHover;

        private int GetItemIndexFromCursor(Point testPoint)
        {
            var index = -1;
            var xpos = (int)((float)testPoint.X / horzMultiple);
            if (xpos >= colCount)
            {
                return -2;
            }

            var ypos = (testPoint.Y - 20) / VertSpacing;
            if (ypos >= rowCount)
            {
                return -1;
            }

            index = startIndex + ypos * colCount + xpos;

            if (index == Items.Count)
            {
                addButtonHover = true;
            }
            else
            {
                addButtonHover = false;
            }

            if (index > Items.Count - 1)
            {
                return -2;
            }

            return index;
        }

        private Point GetTimeEditLocation(Point testPoint)
        {
            var index = -1;
            var xpos = (int)((float)testPoint.X / horzMultiple);
            var xResidual = (int)((float)testPoint.X % horzMultiple);
            if (xpos >= colCount)
            {
                return new Point(-100, -100);
            }

            var ypos = (testPoint.Y - 20) / VertSpacing;
            var yResidual = (testPoint.Y - 20) % VertSpacing;
            if (ypos >= rowCount)
            {
                return new Point(-100, -100);
            }

            index = startIndex + ypos * colCount + xpos;


            if (index > Items.Count - 1)
            {
                return new Point(-100, -100);
            }

            if (yResidual < 48)
            {
                return new Point(-100, -100);
            }

            return new Point((int)((float)xpos * horzMultiple) + 34, ypos * VertSpacing + 20);
        }

        public enum HitPosition { Default, Drag, EditName, EditTime, StartPosition, EndPosition, Transition };
        private HitPosition GetCusorSytleFromCursorPosition(Point testPoint)
        {
            var index = -1;
            var xpos = (int)((float)testPoint.X / horzMultiple);
            var xResidual = (int)((float)(testPoint.X) % horzMultiple) - 34;


            var ypos = (testPoint.Y - 20) / VertSpacing;
            var yResidual = (testPoint.Y - 20) % VertSpacing;
            if (ypos >= rowCount)
            {
                return HitPosition.Default;
            }

            index = startIndex + ypos * colCount + xpos;


            if (index > Items.Count - 1)
            {
                return HitPosition.Default;
            }

            if (xpos >= colCount)
            {
                return HitPosition.Default;
            }

            if (xResidual < 0 && index == selectedItem)
            {
                return HitPosition.Transition;
            }

            if (testPoint.Y > 10 && testPoint.Y < 20)
            {
                if (xResidual < 20)
                {
                    return HitPosition.StartPosition;
                }
                if (xResidual > (horzMultiple - 54))
                {
                    return HitPosition.EndPosition;
                }
            }

            if (yResidual < 49)
            {
                return HitPosition.Drag;
            }

            if (yResidual < 68)
            {
                return HitPosition.EditName;
            }

            if (xResidual > 20 && xResidual < (horzMultiple - 54))
            {
                return HitPosition.EditTime;
            }
            return HitPosition.Default;
        }

        DurrationEditor timeEditor;
        PopupTextEditor textEditor;

        private HitPosition hitType;

        public HitPosition HitType
        {
            get { return hitType; }
            set { hitType = value; }
        }



        private void TourStopList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Y < 10)
            {
                return;
            }
            var extendingRange = false;
            var index = GetItemIndexFromCursor(e.Location);
            if (index > -1)
            {

                if (multiSelectMode)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        if (SelectedItems.ContainsKey(index) && SelectedItems.Count > 1)
                        {
                            // We are right clicking on a multi selection.. Set focus only
                            selectedItem = index;
                            return;
                        }
                    }


                    if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                    {
                        // No control key means clear existing selection and replace
                        SelectedItems.Clear();
                    }

                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                        extendingRange = true;
                        // Add Range to selection
                        var a = Math.Min(index, selectedItem);
                        var b = Math.Max(index, selectedItem);

                        for (var i = a; i <= b; i++)
                        {
                            if (!SelectedItems.ContainsKey(i))
                            {
                                SelectedItems.Add(i, Items[i]);
                            }
                        }
                    }
                    else
                    {
                        // Simple Selection
                        if (SelectedItems.ContainsKey(index))
                        {
                            // Remove an item only if there is more than one selection.
                            if (SelectedItems.Count > 1)
                            {
                                SelectedItems.Remove(index);
                            }
                        }
                        else
                        {
                            SelectedItems.Add(index, Items[index]);
                        }
                    }

                    multipleSelection = (SelectedItems.Count > 1);
                }
                else
                {
                    multipleSelection = false;
                    SelectedItems.Clear();
                    SelectedItems.Add(index, Items[index]);
                }

                // In multiselect case this acts as focus
                if (!extendingRange)
                {
                    selectedItem = index;
                }

                if (((Control.ModifierKeys & Keys.Control) == Keys.Control) || ((Control.ModifierKeys & Keys.Shift) == Keys.Shift))
                {
                    // Early exit if multi-selecting
                    return;
                }

                hitType = this.GetCusorSytleFromCursorPosition(e.Location);

                if (ItemClicked != null)
                {
                    ItemClicked.Invoke(this, Items[index]);
                }

                if (Tour.EditMode)
                {
                    if (hitType == HitPosition.EditTime)
                    {
                        var item = GetItemIndexFromCursor(e.Location);
                        var pnt = GetTimeEditLocation(e.Location);
                        if (timeEditor != null)
                        {
                            timeEditor.Close();
                            timeEditor.Dispose();
                            timeEditor = null;
                        }
                        timeEditor = new DurrationEditor();
                        timeEditor.target = Items[index];
                        timeEditor.Show();
                        timeEditor.Focus();

                        pnt = PointToScreen(pnt);
                        pnt.X += 4;
                        pnt.Y += 65;
                        timeEditor.Location = pnt;
                    }

                    if (hitType == HitPosition.EditName)
                    {
                        var item = GetItemIndexFromCursor(e.Location);
                        var pnt = GetTimeEditLocation(e.Location);
                        if (textEditor != null)
                        {
                            textEditor.Close();
                            textEditor.Dispose();
                            textEditor = null;
                        }
                        textEditor = new PopupTextEditor();
                        textEditor.target = Items[index];
                        textEditor.Show();
                        textEditor.Focus();

                        pnt = PointToScreen(pnt);
                        pnt.X += 15;
                        pnt.Y += 49;
                        textEditor.Location = pnt;
                    }

                    if (hitType == HitPosition.StartPosition)
                    {
                        if (ShowStartPosition != null)
                        {

                            ShowStartPosition.Invoke(this, Items[index]);
                        }
                    }
                    if (hitType == HitPosition.EndPosition)
                    {
                        if (ShowStartPosition != null)
                        {
                            ShowEndPosition.Invoke(this, Items[index]);
                        }
                    }

                    if (hitType == HitPosition.Transition)
                    {
                        var transPopup = new TransitionsPopup();
                        transPopup.Target = Items[index];
                        transPopup.TargetWasChanged += new EventHandler(transPopup_TargetWasChanged);
                        var pnt = new Point((int)((int)(e.X / horzMultiple) * horzMultiple), Height);

                        transPopup.Show();
                        transPopup.Focus();
                        pnt = PointToScreen(pnt);
                        pnt.X -= 10;
                        pnt.Y -= 18;
                        transPopup.Location = pnt;

                    }
                }
            }
            else if (addButtonHover)
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Right)
                {
                    if (Tour.EditMode && showAddButton)
                    {
                        if (AddNewSlide != null)
                        {
                            AddNewSlide.Invoke(this, null);
                        }
                    }
                }
                else
                {
                    SelectedItems.Clear();
                    SelectedItem = -1;
                }
            }
            else if (index == -2)
            {
                SelectedItems.Clear();
                SelectedItem = -1;
                if (ItemClicked != null)
                {
                    ItemClicked.Invoke(this, null);
                }
            }
        }

        void transPopup_TargetWasChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void TourStopList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var index = GetItemIndexFromCursor(e.Location);
            if (index > -1 && ItemDoubleClicked != null)
            {
                ItemDoubleClicked.Invoke(this, Items[index]);
            }

        }

        private void TourStopList_MouseLeave(object sender, EventArgs e)
        {
            hoverItem = -1;
            TransitionHighlighted = false;
            addButtonHover = false;
            Refresh();
            if (ItemHover != null)
            {
                ItemHover.Invoke(this, null);
            }

        }

        bool mouseDown;
        bool scrolling;
        int scrollStartIndex;
        bool dragging;
        int dragDropItemLocation = -1;
        int dragItem = -1;
        Point pointDown;
        DragThumbnail dragThumbnail;
        private void TourStopList_MouseDown(object sender, MouseEventArgs e)
        {
            pointDown = e.Location;
            if (timeEditor != null)
            {
                timeEditor.Close();
                timeEditor.Dispose();
                timeEditor = null;
            }
            if (e.Y < 10)
            {
                var scrollbarWidth = Width;
                var scrollbarStart = 0;

                scrollbarWidth = Math.Max(14, (int)(Math.Min(1.0f, (float)ItemsPerPage / (float)(TotalItems + 1)) * (float)Width));
                scrollbarStart = (int)(((float)startIndex / (float)(TotalItems + 1)) * (float)Width);

                // Page Up
                if (e.X > (scrollbarWidth + scrollbarStart))
                {

                    startIndex = Math.Min((TotalItems + 1) - ItemsPerPage, startIndex + ItemsPerPage);

                    return;
                }

                if (e.X < scrollbarStart)
                {
                    startIndex = Math.Max(0, startIndex - ItemsPerPage);
                    Refresh();
                    return;
                }

                scrollStartIndex = startIndex;
                scrolling = true;
            }

            mouseDown = true;
        }

        private void TourStopList_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
            mouseDown = false;
            scrolling = false;
            if (dragThumbnail != null)
            {
                if (dragItem > -1 && dragItem != dragDropItemLocation)
                {
                    if (dragDropItemLocation != -1)
                    {
                        if (dragDropItemLocation == -2)
                        {
                            dragDropItemLocation = Items.Count;
                        }

                        var dragItems = new List<TourStop>();
                        foreach (var itemId in SelectedItems.Keys)
                        {
                            dragItems.Add(Items[itemId]);
                        }

                        //TourStop ts = Items[dragItem];
                        if (Control.ModifierKeys != Keys.Control)
                        {
                            Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(550, "Move Slide"), Tour));

                            foreach (var ts in dragItems)
                            {
                                Items.Remove(ts);
                            }

                            var itemCount = 0;
                            foreach (var selIndex in SelectedItems.Keys)
                            {
                                if (dragDropItemLocation > dragItem)
                                {
                                    itemCount++;
                                }
                            }

                            dragDropItemLocation -= itemCount;
                            SelectedItems.Clear();

                            foreach (var ts in dragItems)
                            {
                                Items.Insert(dragDropItemLocation, ts);
                                SelectedItems.Add(dragDropItemLocation, Items[dragDropItemLocation]);
                                dragDropItemLocation++;
                            }
                        }
                        else
                        {
                            Undo.Push(new UndoTourSlidelistChange(Language.GetLocalizedText(539, "Drag Copy"), Tour));
                            SelectedItems.Clear();
                            foreach (var ts in dragItems)
                            {
                                Items.Insert(dragDropItemLocation, ts.Copy());
                                SelectedItems.Add(dragDropItemLocation, Items[dragDropItemLocation]);
                                dragDropItemLocation++;
                            }
                        }

                        Tour.CurrentTourstopIndex = selectedItem = dragDropItemLocation;

                        Refresh();
                    }
                }
                dragThumbnail.Close();
                dragThumbnail.Dispose();
                dragThumbnail = null;
            }
            dragDropItemLocation = -1;
            dragItem = -1;
        }

        private void TourStopList_MouseMove(object sender, MouseEventArgs e)
        {
            if (scrolling)
            {
                var scrollbarWidth = Width;
                var scrollbarStart = 0;

                var scrollUnit = (1f / (float)(TotalItems + 1) * Width);

                scrollbarWidth = Math.Max(14, (int)(Math.Min(1.0f, (float)ItemsPerPage / (float)(TotalItems + 1)) * (float)Width));
                scrollbarStart = (int)(((float)startIndex / (float)(TotalItems + 1)) * (float)Width);


                var dragDist = pointDown.X - e.X;

                startIndex = Math.Max(0, Math.Min((TotalItems + 1) - (ItemsPerPage), scrollStartIndex - (int)(dragDist / scrollUnit)));
                Refresh();

                return;
            }

            if (mouseDown)
            {
                if (Tour.EditMode)
                {
                    if (dragging)
                    {
                        var pntTemp = Control.MousePosition;
                        pntTemp.Offset(-48, -27);
                        dragThumbnail.Location = pntTemp;
                        dragDropItemLocation = GetItemIndexFromCursor(e.Location);
                        Refresh();
                    }
                    else
                    {
                        var pntTest = pointDown;
                        pntTest.Offset(-e.Location.X, -e.Location.Y);

                        if (Math.Abs(pntTest.X) > 5 || Math.Abs(pntTest.Y) > 5)
                        {
                            dragItem = GetItemIndexFromCursor(e.Location);
                            if (dragItem > -1)
                            {
                                dragging = true;
                                dragThumbnail = new DragThumbnail();
                                var pntTemp = this.PointToScreen(Control.MousePosition);
                                pntTemp.Offset(-48, -27);
                                dragThumbnail.Location = pntTemp;
                                dragThumbnail.Thumbnail = Items[dragItem].Thumbnail;
                                dragThumbnail.Show();
                            }
                            else
                            {
                                mouseDown = false;
                            }
                        }
                    }
                }
            }
            else
            {
                var newHover = GetItemIndexFromCursor(e.Location);
                if (hoverItem != newHover)
                {
                    hoverItem = newHover;
                    if (ItemHover != null)
                    {
                        if (hoverItem > -1)
                        {
                            ItemHover.Invoke(this, Items[hoverItem]);
                        }
                        else
                        {
                            ItemHover.Invoke(this, null);
                        }
                    }
                }
                Refresh();
                TransitionHighlighted = false;
                switch (GetCusorSytleFromCursorPosition(e.Location))
                {
                    case HitPosition.Default:
                        Cursor.Current = Cursors.Default;
                        break;
                    case HitPosition.Drag:
                        Cursor.Current = Cursors.Hand;
                        break;
                    case HitPosition.EditName:
                        Cursor.Current = Cursors.IBeam;
                        break;
                    case HitPosition.EditTime:
                        Cursor.Current = Cursors.IBeam;
                        break;
                    case HitPosition.StartPosition:
                    case HitPosition.EndPosition:
                        Cursor.Current = Cursors.Arrow;
                        break;
                    case HitPosition.Transition:
                        Cursor.Current = Cursors.Arrow;
                        TransitionHighlighted = true;
                        break;
                    default:
                        Cursor.Current = Cursors.Default;
                        break;
                }
            }
        }

        private void TourStopList_MouseEnter(object sender, EventArgs e)
        {

        }

        private void UpdatePaginator()
        {
            if (paginator != null)
            {
                paginator.TotalPages = PageCount;
                paginator.CurrentPage = CurrentPage;
            }
        }

        private void timeEdit_Leave(object sender, EventArgs e)
        {
 
        }

        private void timeEdit_Validated(object sender, EventArgs e)
        {

        }

        private void TourStopList_Leave(object sender, EventArgs e)
        {

        }

        private void TourStopList_Load(object sender, EventArgs e)
        {
            MouseWheel += new MouseEventHandler(TourStopList_MouseWheel);
        }

        void TourStopList_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (e.Delta < 0)
                {
                    this.PageChanged(this, PageChange.Next);
                }
                else
                {
                    this.PageChanged(this, PageChange.Back);
                }
                UpdatePaginator();
            }
        }

        private void TourStopList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.A:
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        SelectAll();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
