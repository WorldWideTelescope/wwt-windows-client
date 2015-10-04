using System;
using System.Drawing;
using System.Windows.Forms;
using TerraViewer.Properties;

namespace TerraViewer
{
    public delegate void ClickedEventHandler(object sender, EventArgs e);
    public enum Placement { Top, Bottom };
    public enum State { Rest, Hover, Push, Disabled };
    public enum Direction { Expanding, Collapsing };

    public partial class PinUp : UserControl
    {
        public event ClickedEventHandler Clicked;
        public PinUp()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            InitializeComponent();
        }
        static readonly Bitmap[,,] bitmaps = new Bitmap[2,2,4];
        const int TopPin = 0;
        const int BottomPin = 1;
        const int Expanding = 0;
        const int Collapsing = 1;
        const int Rest = 0;
        const int Hover = 1;
        const int Push = 2;
        const int Disabled = 3;
      
        static PinUp()
        {
            bitmaps[TopPin,Expanding,Rest] = Resources.PinT_E_R;
            bitmaps[TopPin,Expanding,Hover] = Resources.PinT_E_H;
            bitmaps[TopPin,Expanding,Push] = Resources.PinT_E_P;
            bitmaps[TopPin,Expanding,Push] = Resources.PinT_E_D;
            bitmaps[TopPin, Collapsing, Rest] = Resources.PinT_C_R;
            bitmaps[TopPin, Collapsing, Hover] = Resources.PinT_C_H;
            bitmaps[TopPin, Collapsing, Push] = Resources.PinT_C_P;
            bitmaps[TopPin, Collapsing, Push] = Resources.PinT_C_D;
            bitmaps[BottomPin, Expanding, Rest] = Resources.PinB_E_R;
            bitmaps[BottomPin, Expanding, Hover] = Resources.PinB_E_H;
            bitmaps[BottomPin, Expanding, Push] = Resources.PinB_E_P;
            bitmaps[BottomPin, Expanding, Push] = Resources.PinB_E_D;
            bitmaps[BottomPin, Collapsing, Rest] = Resources.PinB_C_R;
            bitmaps[BottomPin, Collapsing, Hover] = Resources.PinB_C_H;
            bitmaps[BottomPin, Collapsing, Push] = Resources.PinB_C_P;
            bitmaps[BottomPin, Collapsing, Push] = Resources.PinB_C_D;
        }

        private Placement placement = Placement.Top;

        public Placement Placement
        {
            get { return placement; }
            set
            {
                if (placement != value)
                {
                    placement = value;
                    Refresh();
                }
            }
        }
        private Direction direction = Direction.Collapsing;

        public Direction Direction
        {
            get { return direction; }
            set
            {
                if (direction != value)
                {
                    direction = value;
                    Refresh();
                }  
            }
        }
        private State state = State.Rest;

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



        private void PinUp_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bitmaps[(int)Placement, (int)Direction, (int)State], 0, 0);
        }

        private void PinUp_MouseClick(object sender, MouseEventArgs e)
        {
            if (Clicked != null)
            {
                Clicked.Invoke(this, new EventArgs());
            }
        }

        private void PinUp_MouseEnter(object sender, EventArgs e)
        {
            if (Enabled)
            {
                state = State.Hover;
                Refresh();
            }

        }

        private void PinUp_MouseLeave(object sender, EventArgs e)
        {
            if (Enabled)
            {
                state = State.Rest;
                Refresh();
            }
        }

        private void PinUp_MouseDown(object sender, MouseEventArgs e)
        {
            if (Enabled)
            {
                state = State.Push;
                Refresh();
            }            

        }

        private void PinUp_MouseUp(object sender, MouseEventArgs e)
        {
            if (Enabled)
            {
                state = State.Rest;
                Refresh();
            }                 
        }
    }
}
