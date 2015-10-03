using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{

    [DefaultEvent("SelectionChanged")]

    public partial class WwtCombo : UserControl
    {
        public event SelectionChangedEventHandler SelectionChanged;

        public WwtCombo()
        {
            InitializeComponent();
           
            Text = "";
        }

        public enum ComboType { List, Filter, DateTime };

        private ComboType type = ComboType.List;

        public ComboType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }


        void filterDropDown_SelectionChanged(object sender, EventArgs e)
        {
            SetText();
            Refresh();
            if (SelectionChanged != null)
            {
                SelectionChanged.Invoke(this, new EventArgs());
            }
        }

        public Classification Filter
        {
            get
            {
                if (filterStyle)
                {
                    return filterDropDown.FilterValue;
                }
                else
                {
                    return Classification.Unfiltered;
                }
            }
            set
            {
                if (filterStyle)
                {
                    filterDropDown.FilterValue = value;
                    SetText();
                }
            }
        }

        private DateTime dateTimeValue = DateTime.Now;

        public DateTime DateTimeValue
        {
            get { return dateTimeValue; }
            set
            {
                dateTimeValue = value;
                SetText();
            }
        }

        private bool masterTime = true;

        public bool MasterTime
        {
            get { return masterTime; }
            set { masterTime = value; }
        }

    

        private void SetText()
        {
            try
            {
                if (filterStyle)
                {
                    if (filterDropDown != null)
                    {
                        Tag = filterDropDown.FilterValue;
                        if (filterDropDown.FilterValue == Classification.Unfiltered)
                        {
                            Text = "All";
                        }
                        else
                        {
                            Text = "Filtered";
                        }
                    }
                }
                else
                {
                    if (type == ComboType.DateTime)
                    {
                        if (Properties.Settings.Default.ShowUTCTime || !masterTime)
                        {
                            Text = dateTimeValue.ToString("yyyy/MM/dd   HH:mm:ss");
                        }
                        else
                        {
                            Text = dateTimeValue.ToLocalTime().ToString("yyyy/MM/dd   HH:mm:ss");
                        }
                    }
                    else
                    {
                        if (filterDropDown != null)
                        {
                            if (filterDropDown.SelectedItem == null)
                            {
                                Text = "";
                            }
                            else
                            {
                                Text = filterDropDown.SelectedItem.ToString();
                            }
                        }
                    }
                }
                Refresh();
            }
            catch
            {
            }
        }

        static Bitmap[] edit = new Bitmap[4];
     
        static Bitmap[] drop = new Bitmap[4];

        static WwtCombo()
        {
            edit[0] = global::TerraViewer.Properties.Resources.EditRest;
            edit[1] = global::TerraViewer.Properties.Resources.EditHover;
            edit[2] = global::TerraViewer.Properties.Resources.EditPressed;
            edit[3] = global::TerraViewer.Properties.Resources.EditDisabled;

            drop[0] = global::TerraViewer.Properties.Resources.DropRest;
            drop[1] = global::TerraViewer.Properties.Resources.DropHover;
            drop[2] = global::TerraViewer.Properties.Resources.DropPressed;
            drop[3] = global::TerraViewer.Properties.Resources.DropDisabled;    
        }

        FilterDropDown filterDropDown;



        public FilterDropDown FilterDropDownList
        {
            get
            {
                if (filterDropDown == null)
                {
                    filterDropDown = new FilterDropDown();
                    if (this.Parent is Form)
                    {
                        filterDropDown.Owner = (Form)this.Parent;
                    }
   
                    filterDropDown.SelectionChanged += new SelectionChangedEventHandler(filterDropDown_SelectionChanged);
                }
                return filterDropDown;
            }
            
        }

        bool filterStyle = false;

        public bool FilterStyle
        {
            get { return filterStyle; }
            set
            {
                filterStyle = value;
            }
        }


        private void WwtCombo_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (Enabled)
            {
                g.DrawImage(edit[(int)State], new Rectangle(0, 0, Width - 26, 33), new Rectangle(0, 0, Width - 26, 33), GraphicsUnit.Pixel);
                g.DrawImage(drop[(int)State], Width - 26, 0);
            }
            else
            {
                g.DrawImage(edit[(int)State.Disabled], new Rectangle(0, 0, Width - 26, 33), new Rectangle(0, 0, Width - 26, 33), GraphicsUnit.Pixel);
                g.DrawImage(drop[(int)State.Disabled], Width - 26, 0);
            }
            RectangleF rectText = new RectangleF(6, 9, Width - 32, Height-19);
            g.DrawString(this.Text,UiTools.StandardRegular,UiTools.StadardTextBrush,rectText,UiTools.StringFormatThumbnails);


        }


        public object SelectedItem
        {
            get
            {

                return filterDropDown.SelectedItem;
            }
            set
            { 
                int index = 0;
                foreach (object ob in Items)
                {
                    if (value is string)
                    {
                        if (ob.ToString() == value.ToString())
                        {
                            SelectedIndex = index;
                            return;
                        }
                    }
                    else if (ob == value)
                    {
                        SelectedIndex = index;
                        return;
                    }
                    index++;
                }
                SelectedIndex = -1;
            }

        }

        public void ClearText()
        {
            Text = "";
        }

        public int SelectedIndex
        {
            get
            {
                return FilterDropDownList.SelectedIndex;
            }
            set
            {
                FilterDropDownList.SelectedIndex = value;
                if (SelectedIndex > -1)
                {
                    SetText();
                }
            }
        }

        [MergableProperty(false)]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        //[DesignerSerializationVisibility(2)]
        [Localizable(true)]
        public ListBox.ObjectCollection Items
        {
            get
            {
                return FilterDropDownList.Items;
            }
        }


        private void WwtCombo_MouseDown(object sender, MouseEventArgs e)
        {
            State = State.Push;
            //if (e.X > (Width - 21) && e.X < (Width - 3))
            if (e.X > 3 && e.X < (Width - 3))
            {
                if (type == ComboType.DateTime)
                {
                    if (DatePopup.Current != null)
                    {
                        DatePopup.Current.Close();
                        DatePopup.Current = null;
                    }
                    else
                    {
                        DatePopup.Current = new DatePopup();
                        DatePopup.Current.MasterClock = MasterTime;
                        DatePopup.Current.Show();
                        DatePopup.Current.Location = Parent.PointToScreen(this.Location);
                        DatePopup.Current.Top += 30;
                        DatePopup.Current.DateChanged += new EventHandler(Current_DateChanged);
                        if (!MasterTime)
                        {
                            DatePopup.Current.ScratchTime = this.DateTimeValue;
                        }
                    }
                }
                else
                {
                    if (filterStyle)
                    {
                        FilterDropDownList.FilterValue = (Classification)Tag;
                    }
                    FilterDropDownList.FilterType = filterStyle;
                    FilterDropDownList.Show();
                    FilterDropDownList.Location = Parent.PointToScreen(this.Location);
                    FilterDropDownList.Top += 30;
                    if (FilterDropDownList.Width < Width)
                    {
                        FilterDropDownList.Width = Width;
                    }
                    Rectangle rect = Screen.GetBounds(this);

                    if (FilterDropDownList.Bottom > rect.Bottom)
                    {
                        FilterDropDownList.Top -= (FilterDropDownList.Bottom) - rect.Bottom;
                    }
                }
                //FilterDropDownList.Left = Location.X;

            }
        }

        void Current_DateChanged(object sender, EventArgs e)
        {
            if (DatePopup.Current != null)
            {
                DateTimeValue = DatePopup.Current.ScratchTime;
            }
        }

        State state = State.Rest;

        public State State
        {
            get { return state; }
            set 
            {
                if (state != value)
                {
                    state = value;
                    Refresh();
                }
            }
        }

        private void WwtCombo_MouseEnter(object sender, EventArgs e)
        {
            if (Enabled)
            {
                State = State.Hover;
            }

        }

        private void WwtCombo_MouseLeave(object sender, EventArgs e)
        {
            if (Enabled)
            {
                State = State.Rest;
            }
        }

        private void WwtCombo_MouseUp(object sender, MouseEventArgs e)
        {
            if (Enabled)
            {
                State = State.Rest;
            }
        }
    }
}
