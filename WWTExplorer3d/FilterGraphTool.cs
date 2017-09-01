using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    public class FilterGraphTool : IUiController
    {
        SpreadSheetLayer layer = null;

        int domainColumn = -1;

        public int DomainColumn
        {
            get { return domainColumn; }
            set { domainColumn = value; }
        }


        private int targetColumn = 0;

        public int TargetColumn
        {
            get { return targetColumn; }
            set { targetColumn = value; }
        }

        private int denominatorColumn = -1;

        public int DenominatorColumn
        {
            get { return denominatorColumn; }
            set { denominatorColumn = value; }
        }
        ChartTypes chartType = ChartTypes.Histogram;

        public ChartTypes ChartType
        {
            get { return chartType; }
            set { chartType = value; }
        }
  

        private StatTypes statType = StatTypes.Count;

        public StatTypes StatType
        {
            get { return statType; }
            set { statType = value; }
        }

        public FilterGraphTool(SpreadSheetLayer layer)
        {
            this.layer = layer;
            targetColumn = layer.AltColumn;
        }

        private DateFilter dateFilter = DateFilter.None;

        public DateFilter DateFilter
        {
            get { return dateFilter; }
            set { dateFilter = value; }
        }


        #region IUiController Members

        Texture11 texture = null;
        int Width = 500;
        int Height = 200;
        int Top = 0;
        int Left = 0;
        ColumnStats stats = new ColumnStats();

        public ColumnStats Stats
        {
            get { return stats; }
            set { stats = value; }
        }

        public void PreRender(RenderEngine renderEngine)
        {

        }

        public void Render(RenderEngine renderEngine)
        {
            //todo11 reanble this
            if (texture == null)
            {
                Bitmap bmp = null;
                bmp = GetChartImageBitmap(renderEngine);
                bmp.Dispose();
            }


            Sprite2d.Draw2D(renderEngine.RenderContext11, texture, new SizeF(texture.Width, texture.Height), new PointF(0, 0), 0, new PointF(Left + texture.Width / 2, Top + texture.Height / 2), Color.White);

 
            if (!String.IsNullOrEmpty(HoverText))
            {
                Rectangle recttext = new Rectangle((int)(hoverPoint.X + 15), (int)(hoverPoint.Y - 8), 0, 0);
             }

            
            return;
        }

        private Bitmap GetChartImageBitmap(RenderEngine renderEngine)
        {
            Bitmap bmp = null;
            if (chartType == ChartTypes.Histogram)
            {
                if (!Stats.Computed)
                {
                    Stats = layer.GetSingleColumnHistogram(TargetColumn);
                }
                bmp = GetBarChartBitmap(Stats);

                texture = Texture11.FromBitmap( bmp, 0);

            }
                
            else if (chartType == ChartTypes.BarChart)
            {
                if (!Stats.Computed)
                {
                    Stats = layer.GetDomainValueBarChart(domainColumn, targetColumn, denominatorColumn, statType);
                }
                bmp = GetBarChartBitmap(Stats);


                texture = Texture11.FromBitmap(bmp, 0);

            }
            else if (chartType == ChartTypes.TimeChart)
            {
                if (!Stats.Computed)
                {
                    Stats = layer.GetDateHistogram(TargetColumn, DateFilter);
                }
                bmp = GetBarChartBitmap(Stats);

                texture = Texture11.FromBitmap(bmp, 0);

            }

            Width = bmp.Width;
            Height = bmp.Height;
            Top = (int)renderEngine.RenderContext11.ViewPort.Height - (Height + 120);
            Left = (int)renderEngine.RenderContext11.ViewPort.Width / 2 - (Width / 2);
            
            return bmp;
        }

        public void ComputeChart()
        {
            if (chartType == ChartTypes.Histogram)
            {
                if (!Stats.Computed)
                {
                    Stats = layer.GetSingleColumnHistogram(TargetColumn);
                }
            }

            else if (chartType == ChartTypes.BarChart)
            {
                if (!Stats.Computed)
                {
                    Stats = layer.GetDomainValueBarChart(domainColumn, targetColumn, denominatorColumn, statType);
                }

            }
            else if (chartType == ChartTypes.TimeChart)
            {
                if (!Stats.Computed)
                {
                    Stats = layer.GetDateHistogram(TargetColumn, DateFilter);
                }

            }
        }

        Point hoverPoint = new Point();
        String hoverText = "";

        public String HoverText
        {
            get { return hoverText; }
            set { hoverText = value; }
        }
        void CleanUp()
        {
            if (texture != null)
            {
                texture.Dispose();
                texture = null;
            }
        }

        public static Bitmap GetHistogramBitmap(ColumnStats stats)
        {
            Bitmap bmp = new Bitmap(stats.Buckets, 150);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.FromArgb(128, 68, 82, 105));
            Pen pen = new Pen(Color.FromArgb(127, 137, 157));
            double logMax = Math.Log(stats.HistogramMax);
            if (stats.Histogram != null)
            {
                for (int i = 0; i < stats.Histogram.Length; i++)
                {
                    double height = Math.Log(stats.Histogram[i]) / logMax;
                    if (height < 0)
                    {
                        height = 0;
                    }


                    g.DrawLine(Pens.White, new System.Drawing.Point(i, 150), new System.Drawing.Point(i, (int)(150 - (height * 150))));
                }
            }
            pen.Dispose();
            g.Flush();
            g.Dispose();

            return bmp;
        }
        Rectangle[] barHitTest = null;

        int ScrollPosition = 0;
        int MaxUnits = 50;
       // int TotalUnits = 50;
        bool ScrollBarVisible = false;

        int sortType = 0; // 0 = A-Z, 1 = Z-A, 2= 0-9, 3 = 9-0 

        string title = "";

        public string Title
        {
            get { return (stats.DomainColumn > -1 ? layer.Table.Header[stats.DomainColumn] + " : " : "") + layer.Table.Header[stats.TargetColumn] + " " + stats.DomainStatType.ToString() + ((stats.DomainStatType == StatTypes.Ratio) ? (" to " + layer.Table.Header[stats.DemoninatorColumn]) : ""); }
            set { title = value; }
        }

        public Bitmap GetBarChartBitmap(ColumnStats stats)
        {
            int Chrome = 25;
            
            int height = 150;
            int count = Math.Min(MaxUnits, stats.Buckets);
            int border = 10;
            int colWidth = Math.Min(30, (int)(1000 / count));
            int width = count * colWidth;
            Width = width + 2 * border;
            Bitmap bmp = new Bitmap(Width, height + border * 2 + 20 + Chrome);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.FromArgb(128, 5, 75, 35));
            
            bool anythingSelected = false;
            double selectedAmount = 0;
            double totalAmount = 0;

            if (stats.Selected != null)
            {
                for (int i = 0; i < stats.Buckets; i++)
                {
                    if (stats.Selected[i])
                    {
                        anythingSelected = true;
                        selectedAmount += stats.Histogram[i];
                    }
                    totalAmount += stats.Histogram[i];
                }
            }


            // Draw title
            string text = (stats.DomainColumn > -1 ? layer.Table.Header[stats.DomainColumn] + " : " : "" )+ layer.Table.Header[stats.TargetColumn] + " " + stats.DomainStatType.ToString() + ((stats.DomainStatType == StatTypes.Ratio) ? (" to " + layer.Table.Header[stats.DemoninatorColumn]) : "");

            title = text;

            if (anythingSelected && stats.DomainStatType != StatTypes.Ratio)
            {
                text += String.Format("       {0:p1} Selected", selectedAmount / totalAmount);
            }
            g.DrawString(text, UiTools.StandardGargantuan, Brushes.White, new PointF(border, 0));

            string sort = "AZ";

            switch (sortType)
            {
                case 0:
                    sort = "AZ";
                    break;
                case 1:
                    sort = "ZA";
                    break;
                case 2:
                    sort = "09";
                    break;
                case 3:
                    sort = "90";
                    break;
            }

            if (chartType == ChartTypes.BarChart)
            {
                g.DrawString(sort, UiTools.StandardLarge, Brushes.White, new PointF(Width - 25, 0));
            }
            
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
            drawFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.DirectionVertical;
            drawFormat.Alignment = StringAlignment.Near;
            drawFormat.LineAlignment = StringAlignment.Center;
            
            SolidBrush brush = new SolidBrush(Color.FromArgb(20, 128, 255));
            Brush selectedBrush = Brushes.Yellow;
            //Brushes.White;
            Pen pen = new Pen(Color.FromArgb(20, 128, 255));
            double logMax = Math.Log(stats.HistogramMax);


            int end = Math.Min(stats.Buckets, ScrollPosition + MaxUnits);

            if (stats.Histogram != null)
            {
                barHitTest = new Rectangle[stats.Buckets];
                for (int i = ScrollPosition; i < end; i++)
                {
                    int pos = i - ScrollPosition;

                    double val = stats.Histogram[i] / (stats.HistogramMax * 1.05);
                    if (val < 0)
                    {
                        val = 0;
                    }

                    barHitTest[i] = new Rectangle((int)(pos * colWidth) + border, border + Chrome, colWidth, (int)(height));
                    Rectangle rect = new Rectangle((int)(pos * colWidth) + border, (int)(height - (val * height)) + border + Chrome, colWidth, (int)(val * height));
                    if (stats.Selected[i])
                    {
                        g.FillRectangle(selectedBrush,rect);
                    }
                    else
                    {
                        g.FillRectangle(brush, rect);
                    }

                    if (stats.DomainColumn > -1 || (stats.DomainValues != null && stats.DomainValues.Length > pos))
                    {
                        g.DrawString(stats.DomainValues[i], UiTools.StandardLarge, Brushes.White, new RectangleF((pos * colWidth) + border, border + Chrome, colWidth, height), drawFormat);
                    }
                }

                ScrollBarVisible = false;
                if (MaxUnits < stats.Buckets)
                {
                    int ScrollAreaWidth = Width - (2 * border);
                    // Scroll bars are needed
                    ScrollBarVisible = true;

                    int scrollWidth = (int)((double)MaxUnits / (double)stats.Buckets * ScrollAreaWidth) +2;

                    int scrollStart = (int)((double)ScrollPosition/ (double)stats.Buckets * ScrollAreaWidth);

                    scrollUnitPixelRatio = (double)ScrollAreaWidth / (double)stats.Buckets;

                    g.DrawLine(Pens.White, new Point(border, height + 22 + Chrome), new Point(border + ScrollAreaWidth, height + 22 + Chrome));

                    Rectangle rect = new Rectangle(border + scrollStart, height + 15 + Chrome, scrollWidth, 15);
                    g.FillRectangle(brush, rect);

                }

            }

            brush.Dispose();
            pen.Dispose();
            g.Flush();
            g.Dispose();

            return bmp;
        }

        bool scrolling = false;
        int scrollLastMouseX = 0;
        int scrollPositionMouseDown = 0;
        double scrollUnitPixelRatio = 1;
        bool capture = false;
        int lastClick = -1;
        public bool MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.X > Left && (e.X - Left) < Width)
            {
                if (e.Y > Top && (e.Y - Top) < Height)
                {
                    int x = e.X - Left;
                    int y = e.Y - Top;
                    int i = 0;
                    if (e.Button == MouseButtons.Right)
                    {
                        ContextMenuStrip contextMenu = new ContextMenuStrip();

                        ToolStripMenuItem closeMenu = new ToolStripMenuItem(Language.GetLocalizedText(212, "Close"));
                        ToolStripMenuItem copyMenu = new ToolStripMenuItem(Language.GetLocalizedText(428, "Copy"));
                        ToolStripMenuItem domainColumn = new ToolStripMenuItem(Language.GetLocalizedText(1271, "Domain Column"));

                        ToolStripMenuItem sortOrder = new ToolStripMenuItem(Language.GetLocalizedText(1272, "Sort Order"));

                        ToolStripMenuItem sortOrderAZ = new ToolStripMenuItem(Language.GetLocalizedText(1273, "Alpha Ascending"));
                        ToolStripMenuItem sortOrderZA = new ToolStripMenuItem(Language.GetLocalizedText(1274, "Alpha Descending"));
                        ToolStripMenuItem sortOrder09 = new ToolStripMenuItem(Language.GetLocalizedText(1275, "Numeric Increasing"));
                        ToolStripMenuItem sortOrder90 = new ToolStripMenuItem(Language.GetLocalizedText(1276, "Numeric Decreasing"));

                        sortOrderAZ.Click += new EventHandler(sortOrderAZ_Click);
                        sortOrderZA.Click += new EventHandler(sortOrderZA_Click);
                        sortOrder09.Click += new EventHandler(sortOrder09_Click);
                        sortOrder90.Click += new EventHandler(sortOrder90_Click);

                        sortOrder.DropDownItems.Add(sortOrderAZ);
                        sortOrder.DropDownItems.Add(sortOrderZA);
                        sortOrder.DropDownItems.Add(sortOrder09);
                        sortOrder.DropDownItems.Add(sortOrder90);


                        closeMenu.Click += new EventHandler(closeMenu_Click);
                        copyMenu.Click += new EventHandler(copyMenu_Click);
                        domainColumn.DropDownOpening += new EventHandler(domainColumn_DropDownOpening);

                        contextMenu.Items.Add(closeMenu);
                        contextMenu.Items.Add(copyMenu);
                        if (chartType != ChartTypes.Histogram)
                        {
                            contextMenu.Items.Add(sortOrder);
                            contextMenu.Items.Add(domainColumn);
                        }
                        contextMenu.Show(Cursor.Position);
                    }
                    else
                    {
                        if (barHitTest != null)
                        {
                            foreach (Rectangle rect in barHitTest)
                            {
                                if (rect.Contains(x, y))
                                {
                                    if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                                    {
                                        for (int j = 0; j < Stats.Buckets; j++)
                                        {
                                            if (i != j)
                                            {
                                                Stats.Selected[j] = false;
                                            }
                                        }
                                    }
                                    else
                                    {

                                    }

                                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                                    {
                                        if (lastClick > -1)
                                        {
                                            int dir = lastClick > i ? -1 : 1;

                                            for (int j = lastClick; j != i; j += dir)
                                            {
                                                Stats.Selected[j] = true;
                                            }
                                        }
                                        else
                                        {
                                            lastClick = i;
                                        }
                                    }
                                    else
                                    {
                                        lastClick = i;
                                    }
                                    Stats.Selected[i] = !Stats.Selected[i];
                                    //layer.filters.Add(stats);
                                    layer.CleanUp();
                                    CleanUp();
                                    break;
                                }
                                i++;
                            }
                        }
                    }

                    if (MaxUnits < Stats.Buckets)
                    {
                        int Chrome = 25;
                        int border = 10;
                        int height = 150;
                        int ScrollAreaWidth = Width - (2 * border);
                        // Scroll bars are needed
                        ScrollBarVisible = true;

                        int scrollWidth = (int)((double)MaxUnits / (double)Stats.Buckets * ScrollAreaWidth);

                        int scrollStart = (int)((double)ScrollPosition / (double)Stats.Buckets * ScrollAreaWidth);

                        Rectangle rect = new Rectangle(border + scrollStart, height + 15 + Chrome, scrollWidth, 15);

                        if (rect.Contains(x, y))
                        {
                            scrolling = true;
                            scrollLastMouseX = e.X;
                            scrollPositionMouseDown = ScrollPosition;
                        }

                    }

                    Rectangle sortRect = new Rectangle(Width - 25, 2, 16, 16);
                    if (sortRect.Contains(x, y) && chartType == ChartTypes.BarChart)
                    {
                        // Clicked on sort
                        sortType = (sortType + 1) % 4;
                        Stats.Sort(sortType);
                        CleanUp();
                    }

                    capture = true;
                    return true;
                }
            }
            return false;
        }

        void sortOrder90_Click(object sender, EventArgs e)
        {
            sortType = 3;
            Stats.Sort(sortType);
            CleanUp();
        }

        void sortOrder09_Click(object sender, EventArgs e)
        {
            sortType = 2;
            Stats.Sort(sortType);
            CleanUp();
        }

        void sortOrderZA_Click(object sender, EventArgs e)
        {
            sortType = 1;
            Stats.Sort(sortType);
            CleanUp();
        }

        void sortOrderAZ_Click(object sender, EventArgs e)
        {
            sortType = 0;
            Stats.Sort(sortType);
            CleanUp();
        }

        void domainColumn_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            int index = 0;
            if (item.DropDownItems.Count == 0)
            {
                foreach (string col in layer.Header)
                {
                    ToolStripMenuItem domainColumn = new ToolStripMenuItem(col);
                    domainColumn.Click += new EventHandler(domainColumn_Click);
                    item.DropDownItems.Add(domainColumn);
                    domainColumn.Checked = Stats.DomainColumn == index;
                    domainColumn.Tag = index;
                    index++;
                }
            }
        }

        void domainColumn_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            Stats.Computed = false;
            domainColumn = (int)item.Tag;
            CleanUp();
        }

        void copyMenu_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetImage(GetChartImageBitmap(RenderEngine.Engine));
        }

        void closeMenu_Click(object sender, EventArgs e)
        {
            CleanUp();
            Earth3d.MainWindow.UiController = null;
            layer.Filters.Remove(this);
            layer.CleanUp();
            ((SpreadSheetLayerUI)layer.GetPrimaryUI()).UpdateNodes();
        }

        public bool MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (capture)
            {
                capture = false;
                scrolling = false;
                return true;
            }
            return false;
        }

        public bool MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (capture)
            {
                if (scrolling)
                {
                    ScrollPosition = Math.Min(Stats.Buckets-MaxUnits,Math.Max(0,(int)(scrollPositionMouseDown + (e.X - scrollLastMouseX) / scrollUnitPixelRatio)));
                    CleanUp();
                }


                return true;
            }
            return false;
        }

        public bool MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.X > Left && (e.X - Left) < Width)
            {
                if (e.Y > Top && (e.Y - Top) < Height)
                {
                  
                    return true;
                }
            }
            return false;
        }

        public bool Click(object sender, EventArgs e)
        {

            return false;
        }

        public bool MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.X > Left && (e.X - Left) < Width)
            {
                if (e.Y > Top && (e.Y - Top) < Height)
                {
                    return true;


                }
            }
        
            return false;
        }

        public bool KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            return false;
        }

        public bool KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            return false;
        }

        public bool Hover(System.Drawing.Point pnt)
        {
            if (pnt.X > Left && (pnt.X - Left) < Width)
            {
                if (pnt.Y > Top && (pnt.Y - Top) < Height)
                {
                    int x = pnt.X - Left;
                    int y = pnt.Y - Top;
                    int i = 0;
                    HoverText = null;
                    if (barHitTest != null)
                    {
                        foreach (Rectangle rect in barHitTest)
                        {
                            if (rect.Contains(x, y))
                            {
                                hoverPoint = new Point(x, y);
                                double bucketSize = ((Stats.Max - Stats.Min) / Stats.Buckets);
                                double start = Stats.Min + bucketSize * i;
                                double end = start + bucketSize;
                                if (chartType == ChartTypes.Histogram)
                                {
                                    HoverText = String.Format("{0}-{1} : Count : {2}", start, end, Stats.Histogram[i]);
                                }
                                else
                                {
                                    if (Stats.DomainStatType == StatTypes.Ratio)
                                    {
                                        HoverText = String.Format("{0:p1}", Stats.Histogram[i]);
                                    }
                                    else
                                    {
                                        HoverText = String.Format("{2}", start, end, Stats.Histogram[i]);
                                    }

                                }
                                break;
                            }
                            i++;
                        }
                    }
                    
                    return true;
                }
            }

            HoverText = "";
            return false;
        }

        #endregion
    }

    public class DxUiElement
    {
        public Rectangle Rect;
        
    }

    public enum StatTypes { Count, Average, Median, Sum, Min, Max, Ratio };
    public enum ChartTypes { Histogram, BarChart, LineChart, Scatter, TimeChart };
}
