using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TerraViewer.Properties;

namespace TerraViewer
{
    public delegate void ItemClickedEventHandler(object sender, Object e);

    public enum ThumbnailItemType { Category, Item };
    public enum ThumbnailSize { Small, Big };
    public partial class ThumbnailList : UserControl
    {
        public event ItemClickedEventHandler ItemHover;
        public event ItemClickedEventHandler ItemClicked;
        public event ItemClickedEventHandler ItemDoubleClicked;
        public event ItemClickedEventHandler ItemImageClicked;
        public event ItemClickedEventHandler ItemContextMenu;
        public event ItemClickedEventHandler AddNewItem;

        public ThumbnailList()
        {
            InitializeComponent();
            if (Height < 65)
            {
                Height = 65;
            }
            items = new List<Object>();
            SetStyle(ControlStyles.ResizeRedraw, true);

        }

        public void AddRange( IEnumerable<Object>  addItems)
        {
            items.AddRange(addItems);
            UpdatePaginator();
            Invalidate();    
        }
        public void Insert(Object newItem)
        {
            items.Insert(0,newItem);
            UpdatePaginator();
            Invalidate();
        }
        public void Add(Object newItem)
        {
            items.Add(newItem);
            UpdatePaginator();
            Invalidate();
        }

        public void Clear()
        {
            startIndex = 0;
            items.Clear();
            Invalidate();
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
                        VertSpacing = 75;
                        ThumbHeight = 65;
                        ThumbWidth = 180;
                        break;
                    case ThumbnailSize.Small:
                        HorzSpacing = 110;
                        VertSpacing = 75;
                        ThumbHeight = 65;
                        ThumbWidth = 110;
                        break;
                }
                UpdatePaginator();
                Invalidate();
            }
        }


        public int Count
        {
            get { return items.Count; }
        }

        private List<Object> items;

        public List<Object> Items
        {
            get { return items; }
            set { items = value; }
        }

        static readonly Bitmap bmpBackground = Resources.thumbBackground;
        static readonly Bitmap bmpBackgroundHover = Resources.ThumbBackgroundHover;
        static readonly Bitmap bmpBackgroundWide = Resources.thumbBackgroundWide;
        static readonly Bitmap bmpBackgroundWideHover = Resources.ThumbBackgroundWideHover;
        static Bitmap bmpDropInsertMarker = Resources.DragInsertMarker;

        public void PageChanged(object sender, PageChange e)
        {
            if (e == PageChange.Back)
            {
                if (startIndex > ItemsPerPage)
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
                if ((startIndex + ItemsPerPage) < (items.Count + (showAddButton ? 1 : 0)))
                {
                    startIndex += ItemsPerPage;
                }
            }

            if (e == PageChange.Last)
            {
                startIndex = (((items.Count - 1) + (showAddButton ? 1 : 0)) / ItemsPerPage) * ItemsPerPage;
            }
            Invalidate();
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

        int startIndex;

        int selectedItem = -1;
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
                return  startIndex / ItemsPerPage;
            }
        }


        public int PageCount
        {
            get
            {
                return Math.Max(1, ((items.Count + ItemsPerPage - 1) + (showAddButton ? 1 : 0)) / ItemsPerPage);
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
                    paginator.PageChanged -= PageChanged;
                }    
                
                paginator = value;
                
                if (paginator != null)
                {
                    paginator.PageChanged += PageChanged;
                }
            }
        }

        int HorzSpacing = 110;
        int VertSpacing = 75;
        int ThumbHeight = 65;
        int ThumbWidth = 110;
        float horzMultiple = 110;

        private void ThumbnailList_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            RowCount = Math.Max(Height / ThumbHeight, 1);
            ColCount = Math.Max(Width / HorzSpacing, 1);

            horzMultiple = ((float)Width+13) / ColCount;

            startIndex = (startIndex / ItemsPerPage) * ItemsPerPage;

            RectangleF rectf;
            var index = startIndex;
            for (var y = 0; y < rowCount; y++)
            {
                for (var x = 0; x < colCount; x++)
                {
                    if (index >= items.Count)
                    {

                        if (items.Count == 0 || showAddButton)
                        {
                            rectf = new RectangleF(x * horzMultiple + 3f, y * VertSpacing, ThumbWidth - 10, 64);
                            g.DrawImage(thumbnailSize == ThumbnailSize.Big ? bmpBackgroundWide : bmpBackground, (int)(x * horzMultiple), y * VertSpacing);

                            g.DrawString(showAddButton ? addText : emptyText, UiTools.StandardRegular, (addButtonHover && showAddButton) ? UiTools.YellowTextBrush : UiTools.StadardTextBrush, rectf, UiTools.StringFormatCenterCenter);
                            
                        }          
                        break;
                    }


                    rectf = new RectangleF(x * horzMultiple + 3f, y * VertSpacing, ThumbWidth-14, 64);
                    var textBrush = UiTools.StadardTextBrush;
                    if (index == hoverItem || (index == selectedItem && hoverItem == -1))
                    {
                        g.DrawImage(thumbnailSize == ThumbnailSize.Big ? bmpBackgroundWideHover : bmpBackgroundHover, (int)(x * horzMultiple), y * VertSpacing);
                        textBrush = UiTools.YellowTextBrush;
                    }
                    else
                    {
                        g.DrawImage(thumbnailSize == ThumbnailSize.Big ? bmpBackgroundWide : bmpBackground, (int)(x * horzMultiple), y * VertSpacing);
                    }

                    ((IThumbnail)items[index]).Bounds = RectangleToScreen(new Rectangle((int)(x * horzMultiple), y * VertSpacing, (int)horzMultiple, VertSpacing));
                    try
                    {
                        var bmpThumb = ((IThumbnail)items[index]).ThumbNail;
                        if (bmpThumb != null)
                        {
                            g.DrawImage(bmpThumb, new Rectangle((int)(x * horzMultiple) + 2, y * VertSpacing + 3,bmpThumb.Width,bmpThumb.Height), new Rectangle(0,0,bmpThumb.Width,bmpThumb.Height),GraphicsUnit.Pixel);
                            g.DrawRectangle(Pens.Black, (int)(x * horzMultiple) + 2, y * VertSpacing + 3, ((IThumbnail)items[index]).ThumbNail.Width, ((IThumbnail)items[index]).ThumbNail.Height);
                        }
                        
                    }
                        // TODO FIX this! 
                    catch
                    {
                    }

                    if (((IThumbnail)items[index]).IsImage)
                    {
                        g.DrawImage(Resources.InsertPictureHS, (int)(x * horzMultiple) + 79, y * VertSpacing + 1);
                    }
                    if (((IThumbnail)items[index]).IsTour)
                    {
                        g.DrawImage(Resources.TourIcon, (int)(x * horzMultiple) + 79, y * VertSpacing + 1);
                    }
                    g.DrawString(((IThumbnail)items[index]).Name, UiTools.StandardRegular, textBrush, rectf, UiTools.StringFormatThumbnails);


                    index++;
                }
                if (index >= items.Count)
                {
                    break;
                }
            }
        }

        bool showAddButton;

        public bool ShowAddButton
        {
            get { return showAddButton; }
            set { showAddButton = value; }
        }

        string emptyText = "No Results";

        public string EmptyAddText
        {
            get { return emptyText; }
            set { emptyText = value; }
        }

        string addText = "Add New Item";

        public string AddText
        {
            get { return addText; }
            set { addText = value; }
        }

        bool addButtonHover;

        private int GetItemIndexFromCursor(Point testPoint, out bool imageClicked)
        {
            imageClicked = false;
            var index = -1;
            var xpos = (int)(testPoint.X / horzMultiple);
            var xPart = (int)(testPoint.X % horzMultiple);
            if (xpos >= colCount)
            {
                return -1;
            }
            if (xpos < 0)
            {
                return -1;
            }

            var ypos = testPoint.Y / VertSpacing;
            var yPart = testPoint.Y % VertSpacing;
            if (ypos >= rowCount)
            {
                return -1;
            }
            
            if (ypos < 0)
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

            if (index > items.Count-1)
            {
                return -1;
            }

            if (((IThumbnail)items[index]).IsImage && yPart < 16 && xPart > 78)
            {
                imageClicked = true;
            }

            return index;
        }
            

        private void ThumbnailList_MouseClick(object sender, MouseEventArgs e)
        {
            bool imageClicked;
            var index = GetItemIndexFromCursor(e.Location, out imageClicked);
            if (index > -1)
            {
                if (e.Button != MouseButtons.Right)
                {
                    selectedItem = index;
                    if (imageClicked)
                    {
                        if (ItemImageClicked != null)
                        {

                            ItemImageClicked.Invoke(this, items[index]);
                        }
                        return;
                    }
                    if (ItemClicked != null)
                    {

                        ItemClicked.Invoke(this, items[index]);
                    }
                }
                else
                {
                    selectedItem = index;
                    // Context Menu
                    if (ItemContextMenu != null)
                    {
                        ItemContextMenu.Invoke(this, items[index]);
                    }
                }
            }
            else if (addButtonHover)
            {
                if (AddNewItem != null && ShowAddButton)
                {
                    AddNewItem.Invoke(this, null);
                }
            }
        }

        private void ThumbnailList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bool imageClicked;
            var index = GetItemIndexFromCursor(e.Location, out imageClicked);
            if (index > -1 && ItemDoubleClicked != null)
            {
                ItemDoubleClicked.Invoke(this, items[index]);
            }
 
        }

        private void ThumbnailList_MouseLeave(object sender, EventArgs e)
        {
            hoverItem = -1;
            addButtonHover = false;
            Invalidate();
            if (ItemHover != null)
            {
                ItemHover.Invoke(this, null);
            }

        }

        private void ThumbnailList_MouseMove(object sender, MouseEventArgs e)
        {
            bool imageClicked;
           
            var newHover = GetItemIndexFromCursor(e.Location, out imageClicked);
            if (hoverItem != newHover)
            {
                hoverItem = newHover;
                if (ItemHover != null)
                {
                    if (hoverItem > -1)
                    {
                        ItemHover.Invoke(this, items[hoverItem]);
                    }
                    else
                    {
                        ItemHover.Invoke(this, null);
                    }
                }
            }
            Invalidate();
        }

        private void ThumbnailList_MouseEnter(object sender, EventArgs e)
        {
            if (!Focused && !dontStealFocus)
            {
                if (Earth3d.MainWindow.IsWindowOrChildFocused() || Parent.Focused)
                {
                    Focus();
                }
            }
        }

        private void UpdatePaginator()
        {
            if (paginator != null)
            {
                paginator.TotalPages = PageCount;
                paginator.CurrentPage = CurrentPage;
            }
        }

        private void ThumbnailList_Scroll(object sender, ScrollEventArgs e)
        {
           
        }

        private void ThumbnailList_Load(object sender, EventArgs e)
        {
            MouseWheel += ThumbnailList_MouseWheel;
        }

        void ThumbnailList_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                if (e.Delta < 0)
                {
                    PageChanged(this, PageChange.Next);
                }
                else
                {
                    PageChanged(this, PageChange.Back);
                }
                UpdatePaginator();
            }
        }

        private void ThumbnailList_VisibleChanged(object sender, EventArgs e)
        {
            if (paginator != null)
            {
                paginator.Visible = Visible;
            }
        }

        private void ThumbnailList_KeyDown(object sender, KeyEventArgs e)
        {
            if (items.Count == 0)
            {
                return;
            }
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (selectedItem + ColCount < items.Count)
                    {
                        selectedItem += ColCount;
                    }
                    else
                    {
                        selectedItem = items.Count-1;
                    }
                    break;
                case Keys.End:
                    selectedItem = items.Count-1;
                    break;
                case Keys.Enter:
                    if (ItemClicked != null)
                    {

                        ItemClicked.Invoke(this, items[selectedItem]);
                    }
                    break;
                case Keys.Home:
                    selectedItem = 0;
                    break;
                case Keys.Left:
                    if (selectedItem > 0)
                    {
                        selectedItem--;
                    }
                    break;
                case Keys.Right:
                    if (selectedItem < items.Count-1)
                    {
                        selectedItem++;
                    }
                    break;          
                case Keys.PageDown:
                    PageChanged(this, PageChange.Next);
                    if (selectedItem + ColCount * RowCount < items.Count)
                    {
                        selectedItem += ColCount + RowCount;
                    }
                    else
                    {
                        selectedItem = items.Count - 1;
                    }
                    break;
                case Keys.PageUp:
                    PageChanged(this, PageChange.Back);
                    if (selectedItem - ColCount * RowCount > -1)
                    {
                        selectedItem -= ColCount * rowCount;
                    }
                    else
                    {
                        selectedItem = 0;
                    }
                    break;
                case Keys.Up:
                    if (selectedItem - ColCount > -1)
                    {
                        selectedItem -= ColCount;
                    }
                    else
                    {
                        selectedItem = 0;
                    }
                    break;
                 default:
                    break;
            }

            startIndex = ((selectedItem) / ItemsPerPage) * ItemsPerPage;
            hoverItem = selectedItem;

            Invalidate();
            UpdatePaginator();
        }

        public bool ShowPrevious()
        {
            MovePrevious();
            ShowCurrent();
            return true;
        }

        public void ShowCurrent()
        {
            if (ItemClicked != null)
            {
                if (selectedItem != -1)
                {
                    ItemClicked.Invoke(this, items[selectedItem]);
                }
            }
        }

        public bool ShowNext(bool fromStart, bool doubleClick)
        {
            var wrappedCount = 0;
            if ((items != null && items.Count > 0 ))
            {
                do
                {
                    if (fromStart)
                    {
                        selectedItem = 0;
                        fromStart = false;
                    }
                    else
                    {
                        if (selectedItem < items.Count - 1)
                        {
                            selectedItem++;
                        }
                        else
                        {
                            selectedItem = 0;
                            wrappedCount++;
                        }
                    }
                }
                while ((((IThumbnail)items[selectedItem]).IsFolder || (items[selectedItem]) is FolderUp) && wrappedCount < 2);

                if (wrappedCount > 1)
                {
                    return false;
                }

                if (doubleClick)
                {
                    if (ItemDoubleClicked != null)
                    {
                        ItemDoubleClicked.Invoke(this, items[selectedItem]);
                    }
                }
                else 
                {
                    if (ItemClicked != null)
                    {
                        if (selectedItem != -1)
                        {
                            ItemClicked.Invoke(this, items[selectedItem]);
                        }
                    }
                }
                startIndex = ((selectedItem) / ItemsPerPage) * ItemsPerPage;
                Invalidate();
                UpdatePaginator();
                return selectedItem > -1;
            }
            return false;

        }

        public bool MoveNext()
        {
            if ((items != null && items.Count > 0))
            {
                if (selectedItem < items.Count - 1)
                {
                    selectedItem++;
                }


                startIndex = ((selectedItem) / ItemsPerPage) * ItemsPerPage;
                Invalidate();
                UpdatePaginator();
                return selectedItem > -1;
            }
            return false;
        }

        public bool MovePrevious()
        {
            if ((items != null && items.Count > 0))
            {
                if (selectedItem > 0)
                {
                    selectedItem--;
                }

                startIndex = ((selectedItem) / ItemsPerPage) * ItemsPerPage;
                Invalidate();
                UpdatePaginator();
                return selectedItem > -1;
            }
            return false;
        }

        public void SelectItem()
        {
            if (ItemClicked != null)
            {
                ItemClicked.Invoke(this, items[selectedItem]);
            }
        }


        protected override bool ProcessDialogKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    return false;
            }
            return base.ProcessDialogKey(keyData);
        }
        internal object Selected
        {
            get
            {
                if (selectedItem > -1)
                {
                    return items[selectedItem];
                }
                return null;
            }
        }
        internal void RemoveSelected()
        {
            try
            {

                items.RemoveAt(selectedItem);
                selectedItem--;
                startIndex = ((selectedItem) / ItemsPerPage) * ItemsPerPage;
                Invalidate();
                hoverItem = -1;
            }
            catch
            {
            }
        }
        bool dontStealFocus;

        public bool DontStealFocus
        {
            get { return dontStealFocus; }
            set { dontStealFocus = value; }
        }

        private void ThumbnailList_MouseDown(object sender, MouseEventArgs e)
        {
            dontStealFocus = false;
        }

        private void ThumbnailList_MouseUp(object sender, MouseEventArgs e)
        {
           

        }
    }
}
