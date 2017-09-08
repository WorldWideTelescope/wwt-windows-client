#if WINDOWS_UWP
namespace TerraViewer
{
    //
    // Summary:
    //     Specifies constants that define which mouse button was pressed.

    public enum MouseButtons
    {
        //
        // Summary:
        //     No mouse button was pressed.
        None = 0,
        //
        // Summary:
        //     The left mouse button was pressed.
        Left = 1048576,
        //
        // Summary:
        //     The right mouse button was pressed.
        Right = 2097152,
        //
        // Summary:
        //     The middle mouse button was pressed.
        Middle = 4194304,
        //
        // Summary:
        //     The first XButton was pressed.
        XButton1 = 8388608,
        //
        // Summary:
        //     The second XButton was pressed.
        XButton2 = 16777216
    }

    public class Cursor
    {
        static Cursor current = Cursors.Arrow;
        public static Cursor Current
        {
            get
            {
                return current;
            }
            set
            {
                Current = value;
            }
        }
    }

    //
    // Summary:
    //     Provides a collection of System.Windows.Forms.Cursor objects for use by a Windows
    //     Forms application.
    public class Cursors
    {
        static Cursor Empty = new Cursor();
        //
        // Summary:
        //     Gets the cursor that appears when an application starts.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears when
        //     an application starts.
        public static Cursor AppStarting
        {
            get
            {
                return Empty;
            }
        }
         //
        // Summary:
        //     Gets the cursor that appears during wheel operations when the mouse is moving
        //     and the window is scrolling horizontally and vertically downward and to the left.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears during
        //     wheel operations when the mouse is moving and the window is scrolling horizontally
        //     and vertically downward and to the left.
        public static Cursor PanSW
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears during wheel operations when the mouse is moving
        //     and the window is scrolling vertically in a downward direction.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears during
        //     wheel operations when the mouse is moving and the window is scrolling vertically
        //     in a downward direction.
        public static Cursor PanSouth
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears during wheel operations when the mouse is moving
        //     and the window is scrolling horizontally and vertically downward and to the right.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears during
        //     wheel operations when the mouse is moving and the window is scrolling horizontally
        //     and vertically downward and to the right.
        public static Cursor PanSE
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears during wheel operations when the mouse is moving
        //     and the window is scrolling horizontally and vertically upward and to the left.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears during
        //     wheel operations when the mouse is moving and the window is scrolling horizontally
        //     and vertically upward and to the left.
        public static Cursor PanNW
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears during wheel operations when the mouse is moving
        //     and the window is scrolling vertically in an upward direction.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears during
        //     wheel operations when the mouse is moving and the window is scrolling vertically
        //     in an upward direction.
        public static Cursor PanNorth
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears during wheel operations when the mouse is moving
        //     and the window is scrolling horizontally and vertically upward and to the right.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears during
        //     wheel operations when the mouse is moving and the window is scrolling horizontally
        //     and vertically upward and to the right.
        public static Cursor PanNE
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears during wheel operations when the mouse is moving
        //     and the window is scrolling horizontally to the right.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears during
        //     wheel operations when the mouse is moving and the window is scrolling horizontally
        //     to the right.
        public static Cursor PanEast
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears during wheel operations when the mouse is not moving,
        //     but the window can be scrolled in a vertical direction.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears during
        //     wheel operations when the mouse is not moving.
        public static Cursor NoMoveVert
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears during wheel operations when the mouse is not moving,
        //     but the window can be scrolled in a horizontal direction.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears during
        //     wheel operations when the mouse is not moving.
        public static Cursor NoMoveHoriz
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears during wheel operations when the mouse is not moving,
        //     but the window can be scrolled in both a horizontal and vertical direction.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears during
        //     wheel operations when the mouse is not moving.
        public static Cursor NoMove2D
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears when the mouse is positioned over a vertical splitter
        //     bar.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears when
        //     the mouse is positioned over a vertical splitter bar.
        public static Cursor VSplit
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears when the mouse is positioned over a horizontal splitter
        //     bar.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears when
        //     the mouse is positioned over a horizontal splitter bar.
        public static Cursor HSplit
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the Help cursor, which is a combination of an arrow and a question mark.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the Help cursor.
        public static Cursor Help
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the wait cursor, typically an hourglass shape.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the wait cursor.
        public static Cursor WaitCursor
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the up arrow cursor, typically used to identify an insertion point.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the up arrow cursor.
        public static Cursor UpArrow
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the two-headed horizontal (west/east) sizing cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the two-headed horizontal (west/east)
        //     sizing cursor.
        public static Cursor SizeWE
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the two-headed diagonal (northwest/southeast) sizing cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the two-headed diagonal (northwest/southeast)
        //     sizing cursor.
        public static Cursor SizeNWSE
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the two-headed vertical (north/south) sizing cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the two-headed vertical (north/south)
        //     sizing cursor.
        public static Cursor SizeNS
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the two-headed diagonal (northeast/southwest) sizing cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents two-headed diagonal (northeast/southwest)
        //     sizing cursor.
        public static Cursor SizeNESW
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the four-headed sizing cursor, which consists of four joined arrows that
        //     point north, south, east, and west.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the four-headed sizing cursor.
        public static Cursor SizeAll
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that indicates that a particular region is invalid for the current
        //     operation.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that indicates that
        //     a particular region is invalid for the current operation.
        public static Cursor No
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the I-beam cursor, which is used to show where the text cursor appears when
        //     the mouse is clicked.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the I-beam cursor.
        public static Cursor IBeam
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the default cursor, which is usually an arrow cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the default cursor.
        public static Cursor Default
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the crosshair cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the crosshair cursor.
        public static Cursor Cross
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the arrow cursor.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the arrow cursor.
        public static Cursor Arrow
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the cursor that appears during wheel operations when the mouse is moving
        //     and the window is scrolling horizontally to the left.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the cursor that appears during
        //     wheel operations when the mouse is moving and the window is scrolling horizontally
        //     to the left.
        public static Cursor PanWest
        {
            get
            {
                return Empty;
            }
        }
        //
        // Summary:
        //     Gets the hand cursor, typically used when hovering over a Web link.
        //
        // Returns:
        //     The System.Windows.Forms.Cursor that represents the hand cursor.
        public static Cursor Hand
        {
            get
            {
                return Empty;
            }
        }
    }

    public class Control
    {
        public static Keys ModifierKeys
        {
            get
            {
                var shift = Windows.UI.Xaml.Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Shift).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
                var cntrl = Windows.UI.Xaml.Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Control).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);
                var alt = Windows.UI.Xaml.Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.Menu).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

                Keys mods = Keys.None;

                if (shift)
                {
                    mods = mods | Keys.Shift;
                }

                if (alt)
                {
                    mods = mods | Keys.Alt;
                }

                if (cntrl)
                {
                    mods = mods | Keys.Control;
                }

                return mods;
            }
        }
    }
    public class MouseEventArgs
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Forms.MouseEventArgs class.
        //
        // Parameters:
        //   button:
        //     One of the System.Windows.Forms.MouseButtons values that indicate which mouse
        //     button was pressed.
        //
        //   clicks:
        //     The number of times a mouse button was pressed.
        //
        //   x:
        //     The x-coordinate of a mouse click, in pixels.
        //
        //   y:
        //     The y-coordinate of a mouse click, in pixels.
        //
        //   delta:
        //     A signed count of the number of detents the wheel has rotated.
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, Point location)
        {
            Button = button;
            Clicks = clicks;
            X = x;
            Y = y;
            Delta = delta;
            Location = location;
        }

        //
        // Summary:
        //     Gets which mouse button was pressed.
        //
        // Returns:
        //     One of the System.Windows.Forms.MouseButtons values.
        public MouseButtons Button { get; set; }
        //
        // Summary:
        //     Gets the number of times the mouse button was pressed and released.
        //
        // Returns:
        //     An System.Int32 that contains the number of times the mouse button was pressed
        //     and released.
        public int Clicks { get; set; }
        //
        // Summary:
        //     Gets the x-coordinate of the mouse during the generating mouse event.
        //
        // Returns:
        //     The x-coordinate of the mouse, in pixels.
        public int X { get; set; }
        //
        // Summary:
        //     Gets the y-coordinate of the mouse during the generating mouse event.
        //
        // Returns:
        //     The y-coordinate of the mouse, in pixels.
        public int Y { get; set; }
        //
        // Summary:
        //     Gets a signed count of the number of detents the mouse wheel has rotated, multiplied
        //     by the WHEEL_DELTA constant. A detent is one notch of the mouse wheel.
        //
        // Returns:
        //     A signed count of the number of detents the mouse wheel has rotated, multiplied
        //     by the WHEEL_DELTA constant.
        public int Delta { get; set; }
        //
        // Summary:
        //     Gets the location of the mouse during the generating mouse event.
        //
        // Returns:
        //     A System.Drawing.Point that contains the x- and y- mouse coordinates, in pixels,
        //     relative to the upper-left corner of the form.
        public Point Location { get; set; }
    }


    public class KeyEventArgs
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Windows.Forms.KeyEventArgs class.
        //
        // Parameters:
        //   keyData:
        //     A System.Windows.Forms.Keys representing the key that was pressed, combined with
        //     any modifier flags that indicate which CTRL, SHIFT, and ALT keys were pressed
        //     at the same time. Possible values are obtained be applying the bitwise OR (|)
        //     operator to constants from the System.Windows.Forms.Keys enumeration.
        public KeyEventArgs(Keys keyData)
        {
            keyCode = keyData;
        }
        private Keys keyCode;
        //
        // Summary:
        //     Gets a value indicating whether the ALT key was pressed.
        //
        // Returns:
        //     true if the ALT key was pressed; otherwise, false.
        public virtual bool Alt
        {
            get
            {
                return keyCode.HasFlag(Keys.Alt);
            }
        }
        //
        // Summary:
        //     Gets a value indicating whether the CTRL key was pressed.
        //
        // Returns:
        //     true if the CTRL key was pressed; otherwise, false.
        public bool Control
        {
            get
            {
                return keyCode.HasFlag(Keys.Control);
            }
        }
        //
        // Summary:
        //     Gets or sets a value indicating whether the event was handled.
        //
        // Returns:
        //     true to bypass the control's default handling; otherwise, false to also pass
        //     the event along to the default control handler.
        public bool Handled { get; set; }
        //
        // Summary:
        //     Gets the keyboard code for a System.Windows.Forms.Control.KeyDown or System.Windows.Forms.Control.KeyUp
        //     event.
        //
        // Returns:
        //     A System.Windows.Forms.Keys value that is the key code for the event.
        public Keys KeyCode
        {
            get
            {
                return (Keys)((int)keyCode & 255);
            }
        }
        //
        // Summary:
        //     Gets the keyboard value for a System.Windows.Forms.Control.KeyDown or System.Windows.Forms.Control.KeyUp
        //     event.
        //
        // Returns:
        //     The integer representation of the System.Windows.Forms.KeyEventArgs.KeyCode property.
        public int KeyValue
        {
            get
            {
                return ((int)keyCode & 255);
            }
        }
        //
        // Summary:
        //     Gets the key data for a System.Windows.Forms.Control.KeyDown or System.Windows.Forms.Control.KeyUp
        //     event.
        //
        // Returns:
        //     A System.Windows.Forms.Keys representing the key code for the key that was pressed,
        //     combined with modifier flags that indicate which combination of CTRL, SHIFT,
        //     and ALT keys was pressed at the same time.
        public Keys KeyData
        {
            get
            {
                return keyCode;
            }
        }
        //
        // Summary:
        //     Gets the modifier flags for a System.Windows.Forms.Control.KeyDown or System.Windows.Forms.Control.KeyUp
        //     event. The flags indicate which combination of CTRL, SHIFT, and ALT keys was
        //     pressed.
        //
        // Returns:
        //     A System.Windows.Forms.Keys value representing one or more modifier flags.
        public Keys Modifiers
        {
            get
            {
                return (Keys)((int)keyCode & ~255);
            }
        }
        //
        // Summary:
        //     Gets a value indicating whether the SHIFT key was pressed.
        //
        // Returns:
        //     true if the SHIFT key was pressed; otherwise, false.
        public virtual bool Shift
        {
            get
            {
                return keyCode.HasFlag(Keys.Shift);
            }
        }
    }

    public enum Keys
    {
        //
        // Summary:
        //     The bitmask to extract modifiers from a key value.
        Modifiers = -65536,
        //
        // Summary:
        //     No key pressed.
        None = 0,
        //
        // Summary:
        //     The left mouse button.
        LButton = 1,
        //
        // Summary:
        //     The right mouse button.
        RButton = 2,
        //
        // Summary:
        //     The CANCEL key.
        Cancel = 3,
        //
        // Summary:
        //     The middle mouse button (three-button mouse).
        MButton = 4,
        //
        // Summary:
        //     The first x mouse button (five-button mouse).
        XButton1 = 5,
        //
        // Summary:
        //     The second x mouse button (five-button mouse).
        XButton2 = 6,
        //
        // Summary:
        //     The BACKSPACE key.
        Back = 8,
        //
        // Summary:
        //     The TAB key.
        Tab = 9,
        //
        // Summary:
        //     The LINEFEED key.
        LineFeed = 10,
        //
        // Summary:
        //     The CLEAR key.
        Clear = 12,
        //
        // Summary:
        //     The RETURN key.
        Return = 13,
        //
        // Summary:
        //     The ENTER key.
        Enter = 13,
        //
        // Summary:
        //     The SHIFT key.
        ShiftKey = 16,
        //
        // Summary:
        //     The CTRL key.
        ControlKey = 17,
        //
        // Summary:
        //     The ALT key.
        Menu = 18,
        //
        // Summary:
        //     The PAUSE key.
        Pause = 19,
        //
        // Summary:
        //     The CAPS LOCK key.
        Capital = 20,
        //
        // Summary:
        //     The CAPS LOCK key.
        CapsLock = 20,
        //
        // Summary:
        //     The IME Kana mode key.
        KanaMode = 21,
        //
        // Summary:
        //     The IME Hanguel mode key. (maintained for compatibility; use HangulMode)
        HanguelMode = 21,
        //
        // Summary:
        //     The IME Hangul mode key.
        HangulMode = 21,
        //
        // Summary:
        //     The IME Junja mode key.
        JunjaMode = 23,
        //
        // Summary:
        //     The IME final mode key.
        FinalMode = 24,
        //
        // Summary:
        //     The IME Hanja mode key.
        HanjaMode = 25,
        //
        // Summary:
        //     The IME Kanji mode key.
        KanjiMode = 25,
        //
        // Summary:
        //     The ESC key.
        Escape = 27,
        //
        // Summary:
        //     The IME convert key.
        IMEConvert = 28,
        //
        // Summary:
        //     The IME nonconvert key.
        IMENonconvert = 29,
        //
        // Summary:
        //     The IME accept key, replaces System.Windows.Forms.Keys.IMEAceept.
        IMEAccept = 30,
        //
        // Summary:
        //     The IME accept key. Obsolete, use System.Windows.Forms.Keys.IMEAccept instead.
        IMEAceept = 30,
        //
        // Summary:
        //     The IME mode change key.
        IMEModeChange = 31,
        //
        // Summary:
        //     The SPACEBAR key.
        Space = 32,
        //
        // Summary:
        //     The PAGE UP key.
        Prior = 33,
        //
        // Summary:
        //     The PAGE UP key.
        PageUp = 33,
        //
        // Summary:
        //     The PAGE DOWN key.
        Next = 34,
        //
        // Summary:
        //     The PAGE DOWN key.
        PageDown = 34,
        //
        // Summary:
        //     The END key.
        End = 35,
        //
        // Summary:
        //     The HOME key.
        Home = 36,
        //
        // Summary:
        //     The LEFT ARROW key.
        Left = 37,
        //
        // Summary:
        //     The UP ARROW key.
        Up = 38,
        //
        // Summary:
        //     The RIGHT ARROW key.
        Right = 39,
        //
        // Summary:
        //     The DOWN ARROW key.
        Down = 40,
        //
        // Summary:
        //     The SELECT key.
        Select = 41,
        //
        // Summary:
        //     The PRINT key.
        Print = 42,
        //
        // Summary:
        //     The EXECUTE key.
        Execute = 43,
        //
        // Summary:
        //     The PRINT SCREEN key.
        Snapshot = 44,
        //
        // Summary:
        //     The PRINT SCREEN key.
        PrintScreen = 44,
        //
        // Summary:
        //     The INS key.
        Insert = 45,
        //
        // Summary:
        //     The DEL key.
        Delete = 46,
        //
        // Summary:
        //     The HELP key.
        Help = 47,
        //
        // Summary:
        //     The 0 key.
        D0 = 48,
        //
        // Summary:
        //     The 1 key.
        D1 = 49,
        //
        // Summary:
        //     The 2 key.
        D2 = 50,
        //
        // Summary:
        //     The 3 key.
        D3 = 51,
        //
        // Summary:
        //     The 4 key.
        D4 = 52,
        //
        // Summary:
        //     The 5 key.
        D5 = 53,
        //
        // Summary:
        //     The 6 key.
        D6 = 54,
        //
        // Summary:
        //     The 7 key.
        D7 = 55,
        //
        // Summary:
        //     The 8 key.
        D8 = 56,
        //
        // Summary:
        //     The 9 key.
        D9 = 57,
        //
        // Summary:
        //     The A key.
        A = 65,
        //
        // Summary:
        //     The B key.
        B = 66,
        //
        // Summary:
        //     The C key.
        C = 67,
        //
        // Summary:
        //     The D key.
        D = 68,
        //
        // Summary:
        //     The E key.
        E = 69,
        //
        // Summary:
        //     The F key.
        F = 70,
        //
        // Summary:
        //     The G key.
        G = 71,
        //
        // Summary:
        //     The H key.
        H = 72,
        //
        // Summary:
        //     The I key.
        I = 73,
        //
        // Summary:
        //     The J key.
        J = 74,
        //
        // Summary:
        //     The K key.
        K = 75,
        //
        // Summary:
        //     The L key.
        L = 76,
        //
        // Summary:
        //     The M key.
        M = 77,
        //
        // Summary:
        //     The N key.
        N = 78,
        //
        // Summary:
        //     The O key.
        O = 79,
        //
        // Summary:
        //     The P key.
        P = 80,
        //
        // Summary:
        //     The Q key.
        Q = 81,
        //
        // Summary:
        //     The R key.
        R = 82,
        //
        // Summary:
        //     The S key.
        S = 83,
        //
        // Summary:
        //     The T key.
        T = 84,
        //
        // Summary:
        //     The U key.
        U = 85,
        //
        // Summary:
        //     The V key.
        V = 86,
        //
        // Summary:
        //     The W key.
        W = 87,
        //
        // Summary:
        //     The X key.
        X = 88,
        //
        // Summary:
        //     The Y key.
        Y = 89,
        //
        // Summary:
        //     The Z key.
        Z = 90,
        //
        // Summary:
        //     The left Windows logo key (Microsoft Natural Keyboard).
        LWin = 91,
        //
        // Summary:
        //     The right Windows logo key (Microsoft Natural Keyboard).
        RWin = 92,
        //
        // Summary:
        //     The application key (Microsoft Natural Keyboard).
        Apps = 93,
        //
        // Summary:
        //     The computer sleep key.
        Sleep = 95,
        //
        // Summary:
        //     The 0 key on the numeric keypad.
        NumPad0 = 96,
        //
        // Summary:
        //     The 1 key on the numeric keypad.
        NumPad1 = 97,
        //
        // Summary:
        //     The 2 key on the numeric keypad.
        NumPad2 = 98,
        //
        // Summary:
        //     The 3 key on the numeric keypad.
        NumPad3 = 99,
        //
        // Summary:
        //     The 4 key on the numeric keypad.
        NumPad4 = 100,
        //
        // Summary:
        //     The 5 key on the numeric keypad.
        NumPad5 = 101,
        //
        // Summary:
        //     The 6 key on the numeric keypad.
        NumPad6 = 102,
        //
        // Summary:
        //     The 7 key on the numeric keypad.
        NumPad7 = 103,
        //
        // Summary:
        //     The 8 key on the numeric keypad.
        NumPad8 = 104,
        //
        // Summary:
        //     The 9 key on the numeric keypad.
        NumPad9 = 105,
        //
        // Summary:
        //     The multiply key.
        Multiply = 106,
        //
        // Summary:
        //     The add key.
        Add = 107,
        //
        // Summary:
        //     The separator key.
        Separator = 108,
        //
        // Summary:
        //     The subtract key.
        Subtract = 109,
        //
        // Summary:
        //     The decimal key.
        Decimal = 110,
        //
        // Summary:
        //     The divide key.
        Divide = 111,
        //
        // Summary:
        //     The F1 key.
        F1 = 112,
        //
        // Summary:
        //     The F2 key.
        F2 = 113,
        //
        // Summary:
        //     The F3 key.
        F3 = 114,
        //
        // Summary:
        //     The F4 key.
        F4 = 115,
        //
        // Summary:
        //     The F5 key.
        F5 = 116,
        //
        // Summary:
        //     The F6 key.
        F6 = 117,
        //
        // Summary:
        //     The F7 key.
        F7 = 118,
        //
        // Summary:
        //     The F8 key.
        F8 = 119,
        //
        // Summary:
        //     The F9 key.
        F9 = 120,
        //
        // Summary:
        //     The F10 key.
        F10 = 121,
        //
        // Summary:
        //     The F11 key.
        F11 = 122,
        //
        // Summary:
        //     The F12 key.
        F12 = 123,
        //
        // Summary:
        //     The F13 key.
        F13 = 124,
        //
        // Summary:
        //     The F14 key.
        F14 = 125,
        //
        // Summary:
        //     The F15 key.
        F15 = 126,
        //
        // Summary:
        //     The F16 key.
        F16 = 127,
        //
        // Summary:
        //     The F17 key.
        F17 = 128,
        //
        // Summary:
        //     The F18 key.
        F18 = 129,
        //
        // Summary:
        //     The F19 key.
        F19 = 130,
        //
        // Summary:
        //     The F20 key.
        F20 = 131,
        //
        // Summary:
        //     The F21 key.
        F21 = 132,
        //
        // Summary:
        //     The F22 key.
        F22 = 133,
        //
        // Summary:
        //     The F23 key.
        F23 = 134,
        //
        // Summary:
        //     The F24 key.
        F24 = 135,
        //
        // Summary:
        //     The NUM LOCK key.
        NumLock = 144,
        //
        // Summary:
        //     The SCROLL LOCK key.
        Scroll = 145,
        //
        // Summary:
        //     The left SHIFT key.
        LShiftKey = 160,
        //
        // Summary:
        //     The right SHIFT key.
        RShiftKey = 161,
        //
        // Summary:
        //     The left CTRL key.
        LControlKey = 162,
        //
        // Summary:
        //     The right CTRL key.
        RControlKey = 163,
        //
        // Summary:
        //     The left ALT key.
        LMenu = 164,
        //
        // Summary:
        //     The right ALT key.
        RMenu = 165,
        //
        // Summary:
        //     The browser back key (Windows 2000 or later).
        BrowserBack = 166,
        //
        // Summary:
        //     The browser forward key (Windows 2000 or later).
        BrowserForward = 167,
        //
        // Summary:
        //     The browser refresh key (Windows 2000 or later).
        BrowserRefresh = 168,
        //
        // Summary:
        //     The browser stop key (Windows 2000 or later).
        BrowserStop = 169,
        //
        // Summary:
        //     The browser search key (Windows 2000 or later).
        BrowserSearch = 170,
        //
        // Summary:
        //     The browser favorites key (Windows 2000 or later).
        BrowserFavorites = 171,
        //
        // Summary:
        //     The browser home key (Windows 2000 or later).
        BrowserHome = 172,
        //
        // Summary:
        //     The volume mute key (Windows 2000 or later).
        VolumeMute = 173,
        //
        // Summary:
        //     The volume down key (Windows 2000 or later).
        VolumeDown = 174,
        //
        // Summary:
        //     The volume up key (Windows 2000 or later).
        VolumeUp = 175,
        //
        // Summary:
        //     The media next track key (Windows 2000 or later).
        MediaNextTrack = 176,
        //
        // Summary:
        //     The media previous track key (Windows 2000 or later).
        MediaPreviousTrack = 177,
        //
        // Summary:
        //     The media Stop key (Windows 2000 or later).
        MediaStop = 178,
        //
        // Summary:
        //     The media play pause key (Windows 2000 or later).
        MediaPlayPause = 179,
        //
        // Summary:
        //     The launch mail key (Windows 2000 or later).
        LaunchMail = 180,
        //
        // Summary:
        //     The select media key (Windows 2000 or later).
        SelectMedia = 181,
        //
        // Summary:
        //     The start application one key (Windows 2000 or later).
        LaunchApplication1 = 182,
        //
        // Summary:
        //     The start application two key (Windows 2000 or later).
        LaunchApplication2 = 183,
        //
        // Summary:
        //     The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
        OemSemicolon = 186,
        //
        // Summary:
        //     The OEM 1 key.
        Oem1 = 186,
        //
        // Summary:
        //     The OEM plus key on any country/region keyboard (Windows 2000 or later).
        Oemplus = 187,
        //
        // Summary:
        //     The OEM comma key on any country/region keyboard (Windows 2000 or later).
        Oemcomma = 188,
        //
        // Summary:
        //     The OEM minus key on any country/region keyboard (Windows 2000 or later).
        OemMinus = 189,
        //
        // Summary:
        //     The OEM period key on any country/region keyboard (Windows 2000 or later).
        OemPeriod = 190,
        //
        // Summary:
        //     The OEM question mark key on a US standard keyboard (Windows 2000 or later).
        OemQuestion = 191,
        //
        // Summary:
        //     The OEM 2 key.
        Oem2 = 191,
        //
        // Summary:
        //     The OEM tilde key on a US standard keyboard (Windows 2000 or later).
        Oemtilde = 192,
        //
        // Summary:
        //     The OEM 3 key.
        Oem3 = 192,
        //
        // Summary:
        //     The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
        OemOpenBrackets = 219,
        //
        // Summary:
        //     The OEM 4 key.
        Oem4 = 219,
        //
        // Summary:
        //     The OEM pipe key on a US standard keyboard (Windows 2000 or later).
        OemPipe = 220,
        //
        // Summary:
        //     The OEM 5 key.
        Oem5 = 220,
        //
        // Summary:
        //     The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
        OemCloseBrackets = 221,
        //
        // Summary:
        //     The OEM 6 key.
        Oem6 = 221,
        //
        // Summary:
        //     The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).
        OemQuotes = 222,
        //
        // Summary:
        //     The OEM 7 key.
        Oem7 = 222,
        //
        // Summary:
        //     The OEM 8 key.
        Oem8 = 223,
        //
        // Summary:
        //     The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000
        //     or later).
        OemBackslash = 226,
        //
        // Summary:
        //     The OEM 102 key.
        Oem102 = 226,
        //
        // Summary:
        //     The PROCESS KEY key.
        ProcessKey = 229,
        //
        // Summary:
        //     Used to pass Unicode characters as if they were keystrokes. The Packet key value
        //     is the low word of a 32-bit virtual-key value used for non-keyboard input methods.
        Packet = 231,
        //
        // Summary:
        //     The ATTN key.
        Attn = 246,
        //
        // Summary:
        //     The CRSEL key.
        Crsel = 247,
        //
        // Summary:
        //     The EXSEL key.
        Exsel = 248,
        //
        // Summary:
        //     The ERASE EOF key.
        EraseEof = 249,
        //
        // Summary:
        //     The PLAY key.
        Play = 250,
        //
        // Summary:
        //     The ZOOM key.
        Zoom = 251,
        //
        // Summary:
        //     A constant reserved for future use.
        NoName = 252,
        //
        // Summary:
        //     The PA1 key.
        Pa1 = 253,
        //
        // Summary:
        //     The CLEAR key.
        OemClear = 254,
        //
        // Summary:
        //     The bitmask to extract a key code from a key value.
        KeyCode = 65535,
        //
        // Summary:
        //     The SHIFT modifier key.
        Shift = 65536,
        //
        // Summary:
        //     The CTRL modifier key.
        Control = 131072,
        //
        // Summary:
        //     The ALT modifier key.
        Alt = 262144
    }



    public struct Point
    {
        //
        // Summary:
        //     Represents a System.Drawing.Point that has System.Drawing.Point.X and System.Drawing.Point.Y
        //     values set to zero.
        public static readonly Point Empty = new Point(0, 0);

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Point class from a System.Drawing.Size.
        //
        // Parameters:
        //   sz:
        //     A System.Drawing.Size that specifies the coordinates for the new System.Drawing.Point.
        //public Point(Size sz);
        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Point class using coordinates
        //     specified by an integer value.
        //
        // Parameters:
        //   dw:
        //     A 32-bit integer that specifies the coordinates for the new System.Drawing.Point.
        public Point(int dw)
        {
            X = dw >> 16;
            Y = dw | 0xFFFF;
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Point class with the specified
        //     coordinates.
        //
        // Parameters:
        //   x:
        //     The horizontal position of the point.
        //
        //   y:
        //     The vertical position of the point.
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        //
        // Summary:
        //     Gets a value indicating whether this System.Drawing.Point is empty.
        //
        // Returns:
        //     true if both System.Drawing.Point.X and System.Drawing.Point.Y are 0; otherwise,
        //     false.

        public bool IsEmpty
        {
            get
            {
                return X != 0 && Y != 0;
            }
        }


        //
        // Summary:
        //     Gets or sets the x-coordinate of this System.Drawing.Point.
        //
        // Returns:
        //     The x-coordinate of this System.Drawing.Point.
        public int X { get; set; }
        //
        // Summary:
        //     Gets or sets the y-coordinate of this System.Drawing.Point.
        //
        // Returns:
        //     The y-coordinate of this System.Drawing.Point.
        public int Y { get; set; }

        //
        // Summary:
        //     Adds the specified System.Drawing.Size to the specified System.Drawing.Point.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.Point to add.
        //
        //   sz:
        //     The System.Drawing.Size to add
        //
        // Returns:
        //     The System.Drawing.Point that is the result of the addition operation.
       // public static Point Add(Point pt, Size sz);
        //
        // Summary:
        //     Converts the specified System.Drawing.PointF to a System.Drawing.Point by rounding
        //     the values of the System.Drawing.PointF to the next higher integer values.
        //
        // Parameters:
        //   value:
        //     The System.Drawing.PointF to convert.
        //
        // Returns:
        //     The System.Drawing.Point this method converts to.
       // public static Point Ceiling(PointF value);
        //
        // Summary:
        //     Converts the specified System.Drawing.PointF to a System.Drawing.Point object
        //     by rounding the System.Drawing.Point values to the nearest integer.
        //
        // Parameters:
        //   value:
        //     The System.Drawing.PointF to convert.
        //
        // Returns:
        //     The System.Drawing.Point this method converts to.
       // public static Point Round(PointF value);
        //
        // Summary:
        //     Returns the result of subtracting specified System.Drawing.Size from the specified
        //     System.Drawing.Point.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.Point to be subtracted from.
        //
        //   sz:
        //     The System.Drawing.Size to subtract from the System.Drawing.Point.
        //
        // Returns:
        //     The System.Drawing.Point that is the result of the subtraction operation.
       // public static Point Subtract(Point pt, Size sz);

        //
        // Summary:
        //     Converts the specified System.Drawing.PointF to a System.Drawing.Point by truncating
        //     the values of the System.Drawing.Point.
        //
        // Parameters:
        //   value:
        //     The System.Drawing.PointF to convert.
        //
        // Returns:
        //     The System.Drawing.Point this method converts to.
        public static Point Truncate(Windows.Foundation.Point value)
        {
            return new Point((int)value.X, (int)value.Y);
        }
        //
        // Summary:
        //     Specifies whether this System.Drawing.Point contains the same coordinates as
        //     the specified System.Object.
        //
        // Parameters:
        //   obj:
        //     The System.Object to test.
        //
        // Returns:
        //     true if obj is a System.Drawing.Point and has the same coordinates as this System.Drawing.Point.
        public override bool Equals(object obj)
        {
            Point p = (Point)obj;
            return (p.X == X && p.Y == Y);
        }

        //
        // Summary:
        //     Returns a hash code for this System.Drawing.Point.
        //
        // Returns:
        //     An integer value that specifies a hash value for this System.Drawing.Point.
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode());
        }
        //
        // Summary:
        //     Translates this System.Drawing.Point by the specified amount.
        //
        // Parameters:
        //   dx:
        //     The amount to offset the x-coordinate.
        //
        //   dy:
        //     The amount to offset the y-coordinate.
        //public void Offset(int dx, int dy);
        //
        // Summary:
        //     Translates this System.Drawing.Point by the specified System.Drawing.Point.
        //
        // Parameters:
        //   p:
        //     The System.Drawing.Point used offset this System.Drawing.Point.
        //public void Offset(Point p);
        //
        // Summary:
        //     Converts this System.Drawing.Point to a human-readable string.
        //
        // Returns:
        //     A string that represents this System.Drawing.Point.
        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }

        //
        // Summary:
        //     Translates a System.Drawing.Point by a given System.Drawing.Size.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.Point to translate.
        //
        //   sz:
        //     A System.Drawing.Size that specifies the pair of numbers to add to the coordinates
        //     of pt.
        //
        // Returns:
        //     The translated System.Drawing.Point.
        //public static Point operator +(Point pt, Size sz);
        //
        // Summary:
        //     Translates a System.Drawing.Point by the negative of a given System.Drawing.Size.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.Point to translate.
        //
        //   sz:
        //     A System.Drawing.Size that specifies the pair of numbers to subtract from the
        //     coordinates of pt.
        //
        // Returns:
        //     A System.Drawing.Point structure that is translated by the negative of a given
        //     System.Drawing.Size structure.
        //public static Point operator -(Point pt, Size sz);
        //
        // Summary:
        //     Compares two System.Drawing.Point objects. The result specifies whether the values
        //     of the System.Drawing.Point.X and System.Drawing.Point.Y properties of the two
        //     System.Drawing.Point objects are equal.
        //
        // Parameters:
        //   left:
        //     A System.Drawing.Point to compare.
        //
        //   right:
        //     A System.Drawing.Point to compare.
        //
        // Returns:
        //     true if the System.Drawing.Point.X and System.Drawing.Point.Y values of left
        //     and right are equal; otherwise, false.
        //public static bool operator ==(Point left, Point right);
        //
        // Summary:
        //     Compares two System.Drawing.Point objects. The result specifies whether the values
        //     of the System.Drawing.Point.X or System.Drawing.Point.Y properties of the two
        //     System.Drawing.Point objects are unequal.
        //
        // Parameters:
        //   left:
        //     A System.Drawing.Point to compare.
        //
        //   right:
        //     A System.Drawing.Point to compare.
        //
        // Returns:
        //     true if the values of either the System.Drawing.Point.X properties or the System.Drawing.Point.Y
        //     properties of left and right differ; otherwise, false.
        //public static bool operator !=(Point left, Point right);

        //
        // Summary:
        //     Converts the specified System.Drawing.Point structure to a System.Drawing.PointF
        //     structure.
        //
        // Parameters:
        //   p:
        //     The System.Drawing.Point to be converted.
        //
        // Returns:
        //     The System.Drawing.PointF that results from the conversion.
        public static implicit operator PointF(Point p)
        {
            return new PointF(p.X, p.Y);
        }

        //
        // Summary:
        //     Converts the specified System.Drawing.Point structure to a System.Drawing.Size
        //     structure.
        //
        // Parameters:
        //   p:
        //     The System.Drawing.Point to be converted.
        //
        // Returns:
        //     The System.Drawing.Size that results from the conversion.
       // public static explicit operator Size(Point p);
    }

    //
    // Summary:
    //     Stores a set of four floating-point numbers that represent the location and size
    //     of a rectangle. For more advanced region functions, use a System.Drawing.Region
    //     object.
    public struct RectangleF
    {
        //
        // Summary:
        //     Represents an instance of the System.Drawing.RectangleF class with its members
        //     uninitialized.
        public static readonly RectangleF Empty = new Rectangle(0, 0, 0, 0);

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.RectangleF class with the specified
        //     location and size.
        //
        // Parameters:
        //   location:
        //     A System.Drawing.PointF that represents the upper-left corner of the rectangular
        //     region.
        //
        //   size:
        //     A System.Drawing.SizeF that represents the width and height of the rectangular
        //     region.
        public RectangleF(PointF location, SizeF size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.RectangleF class with the specified
        //     location and size.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the upper-left corner of the rectangle.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the rectangle.
        //
        //   width:
        //     The width of the rectangle.
        //
        //   height:
        //     The height of the rectangle.
        public RectangleF(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        //
        // Summary:
        //     Gets the x-coordinate that is the sum of System.Drawing.RectangleF.X and System.Drawing.RectangleF.Width
        //     of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The x-coordinate that is the sum of System.Drawing.RectangleF.X and System.Drawing.RectangleF.Width
        //     of this System.Drawing.RectangleF structure.
        public float Right
        {
            get
            {
                return X + Width;
            }
        }
        //
        // Summary:
        //     Gets the y-coordinate of the top edge of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The y-coordinate of the top edge of this System.Drawing.RectangleF structure.
        public float Top
        {
            get
            {
                return Y;
            }
        }
        //
        // Summary:
        //     Gets the x-coordinate of the left edge of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The x-coordinate of the left edge of this System.Drawing.RectangleF structure.
        public float Left
        {
            get
            {
                return X;
            }
        }
        //
        // Summary:
        //     Gets or sets the height of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The height of this System.Drawing.RectangleF structure. The default is 0.
        public float Height { get; set; }
        //
        // Summary:
        //     Gets or sets the width of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The width of this System.Drawing.RectangleF structure. The default is 0.
        public float Width { get; set; }
        //
        // Summary:
        //     Gets or sets the y-coordinate of the upper-left corner of this System.Drawing.RectangleF
        //     structure.
        //
        // Returns:
        //     The y-coordinate of the upper-left corner of this System.Drawing.RectangleF structure.
        //     The default is 0.
        public float Y { get; set; }
        //
        // Summary:
        //     Gets or sets the x-coordinate of the upper-left corner of this System.Drawing.RectangleF
        //     structure.
        //
        // Returns:
        //     The x-coordinate of the upper-left corner of this System.Drawing.RectangleF structure.
        //     The default is 0.
        public float X { get; set; }
        //
        // Summary:
        //     Gets or sets the size of this System.Drawing.RectangleF.
        //
        // Returns:
        //     A System.Drawing.SizeF that represents the width and height of this System.Drawing.RectangleF
        //     structure.

        public SizeF Size
        {
            get
            {
                return new SizeF(Width, Height);
            }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }
        //
        // Summary:
        //     Gets or sets the coordinates of the upper-left corner of this System.Drawing.RectangleF
        //     structure.
        //
        // Returns:
        //     A System.Drawing.PointF that represents the upper-left corner of this System.Drawing.RectangleF
        //     structure.

        public PointF Location
        {
            get
            {
                return new PointF(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        //
        // Summary:
        //     Gets the y-coordinate that is the sum of System.Drawing.RectangleF.Y and System.Drawing.RectangleF.Height
        //     of this System.Drawing.RectangleF structure.
        //
        // Returns:
        //     The y-coordinate that is the sum of System.Drawing.RectangleF.Y and System.Drawing.RectangleF.Height
        //     of this System.Drawing.RectangleF structure.

        public float Bottom
        {
            get
            {
                return Top + Height;
            }
        }
        //
        // Summary:
        //     Tests whether the System.Drawing.RectangleF.Width or System.Drawing.RectangleF.Height
        //     property of this System.Drawing.RectangleF has a value of zero.
        //
        // Returns:
        //     This property returns true if the System.Drawing.RectangleF.Width or System.Drawing.RectangleF.Height
        //     property of this System.Drawing.RectangleF has a value of zero; otherwise, false.

        public bool IsEmpty
        {
            get
            {
                return X == 0 && Y == 0 && Width == 0 && Height == 0;
            }
        }

        //
        // Summary:
        //     Creates a System.Drawing.RectangleF structure with upper-left corner and lower-right
        //     corner at the specified locations.
        //
        // Parameters:
        //   left:
        //     The x-coordinate of the upper-left corner of the rectangular region.
        //
        //   top:
        //     The y-coordinate of the upper-left corner of the rectangular region.
        //
        //   right:
        //     The x-coordinate of the lower-right corner of the rectangular region.
        //
        //   bottom:
        //     The y-coordinate of the lower-right corner of the rectangular region.
        //
        // Returns:
        //     The new System.Drawing.RectangleF that this method creates.
        public static RectangleF FromLTRB(float left, float top, float right, float bottom)
        {
            return new RectangleF(left, top, right - left, bottom - top);
        }
        // Summary:
        //     Creates and returns an enlarged copy of the specified System.Drawing.RectangleF
        //     structure. The copy is enlarged by the specified amount and the original rectangle
        //     remains unmodified.
        //
        // Parameters:
        //   rect:
        //     The System.Drawing.RectangleF to be copied. This rectangle is not modified.
        //
        //   x:
        //     The amount to enlarge the copy of the rectangle horizontally.
        //
        //   y:
        //     The amount to enlarge the copy of the rectangle vertically.
        //
        // Returns:
        //     The enlarged System.Drawing.RectangleF.
        //public static RectangleF Inflate(RectangleF rect, float x, float y);
        //
        // Summary:
        //     Returns a System.Drawing.RectangleF structure that represents the intersection
        //     of two rectangles. If there is no intersection, and empty System.Drawing.RectangleF
        //     is returned.
        //
        // Parameters:
        //   a:
        //     A rectangle to intersect.
        //
        //   b:
        //     A rectangle to intersect.
        //
        // Returns:
        //     A third System.Drawing.RectangleF structure the size of which represents the
        //     overlapped area of the two specified rectangles.
        //public static RectangleF Intersect(RectangleF a, RectangleF b);
        //
        // Summary:
        //     Creates the smallest possible third rectangle that can contain both of two rectangles
        //     that form a union.
        //
        // Parameters:
        //   a:
        //     A rectangle to union.
        //
        //   b:
        //     A rectangle to union.
        //
        // Returns:
        //     A third System.Drawing.RectangleF structure that contains both of the two rectangles
        //     that form the union.
        //public static RectangleF Union(RectangleF a, RectangleF b);
        //
        // Summary:
        //     Determines if the specified point is contained within this System.Drawing.RectangleF
        //     structure.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the point to test.
        //
        //   y:
        //     The y-coordinate of the point to test.
        //
        // Returns:
        //     This method returns true if the point defined by x and y is contained within
        //     this System.Drawing.RectangleF structure; otherwise false.
        public bool Contains(float x, float y)
        {
            return (x > X && x < (X + Width) && y > Y && y < (Y + Height));
        }
        //
        // Summary:
        //     Determines if the specified point is contained within this System.Drawing.RectangleF
        //     structure.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.PointF to test.
        //
        // Returns:
        //     This method returns true if the point represented by the pt parameter is contained
        //     within this System.Drawing.RectangleF structure; otherwise false.
        public bool Contains(PointF pt)
        {
            return (pt.X > X && pt.X < (X + Width) && pt.Y > Y && pt.Y < (Y + Height));
        }
        //
        // Summary:
        //     Determines if the rectangular region represented by rect is entirely contained
        //     within this System.Drawing.RectangleF structure.
        //
        // Parameters:
        //   rect:
        //     The System.Drawing.RectangleF to test.
        //
        // Returns:
        //     This method returns true if the rectangular region represented by rect is entirely
        //     contained within the rectangular region represented by this System.Drawing.RectangleF;
        //     otherwise false.
        //public bool Contains(RectangleF rect);
        //
        // Summary:
        //     Tests whether obj is a System.Drawing.RectangleF with the same location and size
        //     of this System.Drawing.RectangleF.
        //
        // Parameters:
        //   obj:
        //     The System.Object to test.
        //
        // Returns:
        //     This method returns true if obj is a System.Drawing.RectangleF and its X, Y,
        //     Width, and Height properties are equal to the corresponding properties of this
        //     System.Drawing.RectangleF; otherwise, false.
        public override bool Equals(object obj)         
        {
            RectangleF right = (RectangleF)obj;

            return X == right.X && Y == right.Y && Width == right.Width && Height == right.Height;
        }
        //
        // Summary:
        //     Gets the hash code for this System.Drawing.RectangleF structure. For information
        //     about the use of hash codes, see Object.GetHashCode.
        //
        // Returns:
        //     The hash code for this System.Drawing.RectangleF.
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }
        //
        // Summary:
        //     Enlarges this System.Drawing.RectangleF by the specified amount.
        //
        // Parameters:
        //   size:
        //     The amount to inflate this rectangle.
        //public void Inflate(SizeF size);
        //
        // Summary:
        //     Enlarges this System.Drawing.RectangleF structure by the specified amount.
        //
        // Parameters:
        //   x:
        //     The amount to inflate this System.Drawing.RectangleF structure horizontally.
        //
        //   y:
        //     The amount to inflate this System.Drawing.RectangleF structure vertically.
        //public void Inflate(float x, float y);
        //
        // Summary:
        //     Replaces this System.Drawing.RectangleF structure with the intersection of itself
        //     and the specified System.Drawing.RectangleF structure.
        //
        // Parameters:
        //   rect:
        //     The rectangle to intersect.
        //public void Intersect(RectangleF rect);
        //
        // Summary:
        //     Determines if this rectangle intersects with rect.
        //
        // Parameters:
        //   rect:
        //     The rectangle to test.
        //
        // Returns:
        //     This method returns true if there is any intersection.
        //public bool IntersectsWith(RectangleF rect);
        //
        // Summary:
        //     Adjusts the location of this rectangle by the specified amount.
        //
        // Parameters:
        //   x:
        //     The amount to offset the location horizontally.
        //
        //   y:
        //     The amount to offset the location vertically.
        //public void Offset(float x, float y);
        //
        // Summary:
        //     Adjusts the location of this rectangle by the specified amount.
        //
        // Parameters:
        //   pos:
        //     The amount to offset the location.
        // public void Offset(PointF pos);
        //
        // Summary:
        //     Converts the Location and System.Drawing.Size of this System.Drawing.RectangleF
        //     to a human-readable string.
        //
        // Returns:
        //     A string that contains the position, width, and height of this System.Drawing.RectangleF
        //     structure. For example, "{X=20, Y=20, Width=100, Height=50}".
        public override string ToString()
        {
            return string.Format("({0},{1}, {2}, {3})", X, Y, Width, Height);
        }

        //
        // Summary:
        //     Tests whether two System.Drawing.RectangleF structures have equal location and
        //     size.
        //
        // Parameters:
        //   left:
        //     The System.Drawing.RectangleF structure that is to the left of the equality operator.
        //
        //   right:
        //     The System.Drawing.RectangleF structure that is to the right of the equality
        //     operator.
        //
        // Returns:
        //     This operator returns true if the two specified System.Drawing.RectangleF structures
        //     have equal System.Drawing.RectangleF.X, System.Drawing.RectangleF.Y, System.Drawing.RectangleF.Width,
        //     and System.Drawing.RectangleF.Height properties.
        public static bool operator ==(RectangleF left, RectangleF right)
        {
            return left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
        }
        //
        // Summary:
        //     Tests whether two System.Drawing.RectangleF structures differ in location or
        //     size.
        //
        // Parameters:
        //   left:
        //     The System.Drawing.RectangleF structure that is to the left of the inequality
        //     operator.
        //
        //   right:
        //     The System.Drawing.RectangleF structure that is to the right of the inequality
        //     operator.
        //
        // Returns:
        //     This operator returns true if any of the System.Drawing.RectangleF.X , System.Drawing.RectangleF.Y,
        //     System.Drawing.RectangleF.Width, or System.Drawing.RectangleF.Height properties
        //     of the two System.Drawing.Rectangle structures are unequal; otherwise false.
        public static bool operator !=(RectangleF left, RectangleF right)
        {
            return left.X != right.X || left.Y != right.Y || left.Width != right.Width || left.Height != right.Height;
        }

        //
        // Summary:
        //     Converts the specified System.Drawing.Rectangle structure to a System.Drawing.RectangleF
        //     structure.
        //
        // Parameters:
        //   r:
        //     The System.Drawing.Rectangle structure to convert.
        //
        // Returns:
        //     The System.Drawing.RectangleF structure that is converted from the specified
        //     System.Drawing.Rectangle structure.
        public static implicit operator RectangleF(Rectangle r)
        {
            return new RectangleF(r.X, r.Y, r.Width, r.Height);
        }
    }

    //
    // Summary:
    //     Stores a set of four integers that represent the location and size of a rectangle

    public struct Rectangle
    {
        //
        // Summary:
        //     Represents a System.Drawing.Rectangle structure with its properties left uninitialized.
        public static readonly Rectangle Empty = new Rectangle(0,0,0,0);

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Rectangle class with the specified
        //     location and size.
        //
        // Parameters:
        //   location:
        //     A System.Drawing.Point that represents the upper-left corner of the rectangular
        //     region.
        //
        //   size:
        //     A System.Drawing.Size that represents the width and height of the rectangular
        //     region.
        public Rectangle(Point location, Size size)
        {
            X = location.X;
            Y = location.Y;
            Width = size.Width;
            Height = size.Height;
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Rectangle class with the specified
        //     location and size.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the upper-left corner of the rectangle.
        //
        //   y:
        //     The y-coordinate of the upper-left corner of the rectangle.
        //
        //   width:
        //     The width of the rectangle.
        //
        //   height:
        //     The height of the rectangle.
        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        //
        // Summary:
        //     Gets the x-coordinate that is the sum of System.Drawing.Rectangle.X and System.Drawing.Rectangle.Width
        //     property values of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The x-coordinate that is the sum of System.Drawing.Rectangle.X and System.Drawing.Rectangle.Width
        //     of this System.Drawing.Rectangle.
        public int Right
        {
            get
            {
                return X + Width;
            }
        }
        //
        // Summary:
        //     Gets the y-coordinate of the top edge of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The y-coordinate of the top edge of this System.Drawing.Rectangle structure.

        public int Top
        {
            get
            {
                return Y;
            }
        }
        //
        // Summary:
        //     Gets the x-coordinate of the left edge of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The x-coordinate of the left edge of this System.Drawing.Rectangle structure.
        public int Left
        {
            get
            {
                return X;
            }
        }
        //
        // Summary:
        //     Gets or sets the height of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The height of this System.Drawing.Rectangle structure. The default is 0.
        public int Height { get; set; }
        //
        // Summary:
        //     Gets or sets the width of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The width of this System.Drawing.Rectangle structure. The default is 0.
        public int Width { get; set; }
        //
        // Summary:
        //     Gets or sets the y-coordinate of the upper-left corner of this System.Drawing.Rectangle
        //     structure.
        //
        // Returns:
        //     The y-coordinate of the upper-left corner of this System.Drawing.Rectangle structure.
        //     The default is 0.
        public int Y { get; set; }
        //
        // Summary:
        //     Gets or sets the x-coordinate of the upper-left corner of this System.Drawing.Rectangle
        //     structure.
        //
        // Returns:
        //     The x-coordinate of the upper-left corner of this System.Drawing.Rectangle structure.
        //     The default is 0.
        public int X { get; set; }
        //
        // Summary:
        //     Gets or sets the size of this System.Drawing.Rectangle.
        //
        // Returns:
        //     A System.Drawing.Size that represents the width and height of this System.Drawing.Rectangle
        //     structure.

        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }
        //
        // Summary:
        //     Gets or sets the coordinates of the upper-left corner of this System.Drawing.Rectangle
        //     structure.
        //
        // Returns:
        //     A System.Drawing.Point that represents the upper-left corner of this System.Drawing.Rectangle
        //     structure.

        public Point Location
        {
            get
            {
                return new Point(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }       
        
        //
        // Summary:
        //     Gets the y-coordinate that is the sum of the System.Drawing.Rectangle.Y and System.Drawing.Rectangle.Height
        //     property values of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The y-coordinate that is the sum of System.Drawing.Rectangle.Y and System.Drawing.Rectangle.Height
        //     of this System.Drawing.Rectangle.

        public int Bottom
        {
            get
            {
                return Top + Height;
            }
        }
        //
        // Summary:
        //     Tests whether all numeric properties of this System.Drawing.Rectangle have values
        //     of zero.
        //
        // Returns:
        //     This property returns true if the System.Drawing.Rectangle.Width, System.Drawing.Rectangle.Height,
        //     System.Drawing.Rectangle.X, and System.Drawing.Rectangle.Y properties of this
        //     System.Drawing.Rectangle all have values of zero; otherwise, false.
        public bool IsEmpty
        {
            get
            {
                return X == 0 && Y == 0 && Width == 0 && Height == 0;
            }
        }

        //
        // Summary:
        //     Converts the specified System.Drawing.RectangleF structure to a System.Drawing.Rectangle
        //     structure by rounding the System.Drawing.RectangleF values to the next higher
        //     integer values.
        //
        // Parameters:
        //   value:
        //     The System.Drawing.RectangleF structure to be converted.
        //
        // Returns:
        //     Returns a System.Drawing.Rectangle.
        //public static Rectangle Ceiling(RectangleF value);
        //
        // Summary:
        //     Creates a System.Drawing.Rectangle structure with the specified edge locations.
        //
        // Parameters:
        //   left:
        //     The x-coordinate of the upper-left corner of this System.Drawing.Rectangle structure.
        //
        //   top:
        //     The y-coordinate of the upper-left corner of this System.Drawing.Rectangle structure.
        //
        //   right:
        //     The x-coordinate of the lower-right corner of this System.Drawing.Rectangle structure.
        //
        //   bottom:
        //     The y-coordinate of the lower-right corner of this System.Drawing.Rectangle structure.
        //
        // Returns:
        //     The new System.Drawing.Rectangle that this method creates.
        public static Rectangle FromLTRB(int left, int top, int right, int bottom)
        {
            return new Rectangle(left, top, right - left, bottom - top);
        }
        //
        // Summary:
        //     Creates and returns an enlarged copy of the specified System.Drawing.Rectangle
        //     structure. The copy is enlarged by the specified amount. The original System.Drawing.Rectangle
        //     structure remains unmodified.
        //
        // Parameters:
        //   rect:
        //     The System.Drawing.Rectangle with which to start. This rectangle is not modified.
        //
        //   x:
        //     The amount to inflate this System.Drawing.Rectangle horizontally.
        //
        //   y:
        //     The amount to inflate this System.Drawing.Rectangle vertically.
        //
        // Returns:
        //     The enlarged System.Drawing.Rectangle.
        //public static Rectangle Inflate(Rectangle rect, int x, int y);
        //
        // Summary:
        //     Returns a third System.Drawing.Rectangle structure that represents the intersection
        //     of two other System.Drawing.Rectangle structures. If there is no intersection,
        //     an empty System.Drawing.Rectangle is returned.
        //
        // Parameters:
        //   a:
        //     A rectangle to intersect.
        //
        //   b:
        //     A rectangle to intersect.
        //
        // Returns:
        //     A System.Drawing.Rectangle that represents the intersection of a and b.
        // public static Rectangle Intersect(Rectangle a, Rectangle b);
        //
        // Summary:
        //     Converts the specified System.Drawing.RectangleF to a System.Drawing.Rectangle
        //     by rounding the System.Drawing.RectangleF values to the nearest integer values.
        //
        // Parameters:
        //   value:
        //     The System.Drawing.RectangleF to be converted.
        //
        // Returns:
        //     The rounded interger value of the System.Drawing.Rectangle.
        //public static Rectangle Round(RectangleF value);
        //
        // Summary:
        //     Converts the specified System.Drawing.RectangleF to a System.Drawing.Rectangle
        //     by truncating the System.Drawing.RectangleF values.
        //
        // Parameters:
        //   value:
        //     The System.Drawing.RectangleF to be converted.
        //
        // Returns:
        //     The truncated value of the System.Drawing.Rectangle.
        //public static Rectangle Truncate(RectangleF value);
        //
        // Summary:
        //     Gets a System.Drawing.Rectangle structure that contains the union of two System.Drawing.Rectangle
        //     structures.
        //
        // Parameters:
        //   a:
        //     A rectangle to union.
        //
        //   b:
        //     A rectangle to union.
        //
        // Returns:
        //     A System.Drawing.Rectangle structure that bounds the union of the two System.Drawing.Rectangle
        //     structures.
        //public static Rectangle Union(Rectangle a, Rectangle b);
        //
        // Summary:
        //     Determines if the specified point is contained within this System.Drawing.Rectangle
        //     structure.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.Point to test.
        //
        // Returns:
        //     This method returns true if the point represented by pt is contained within this
        //     System.Drawing.Rectangle structure; otherwise false.
        //public bool Contains(Point pt);
        //
        // Summary:
        //     Determines if the rectangular region represented by rect is entirely contained
        //     within this System.Drawing.Rectangle structure.
        //
        // Parameters:
        //   rect:
        //     The System.Drawing.Rectangle to test.
        //
        // Returns:
        //     This method returns true if the rectangular region represented by rect is entirely
        //     contained within this System.Drawing.Rectangle structure; otherwise false.
        //public bool Contains(Rectangle rect);
        //
        // Summary:
        //     Determines if the specified point is contained within this System.Drawing.Rectangle
        //     structure.
        //
        // Parameters:
        //   x:
        //     The x-coordinate of the point to test.
        //
        //   y:
        //     The y-coordinate of the point to test.
        //
        // Returns:
        //     This method returns true if the point defined by x and y is contained within
        //     this System.Drawing.Rectangle structure; otherwise false.
        //public bool Contains(int x, int y);
        //
        // Summary:
        //     Tests whether obj is a System.Drawing.Rectangle structure with the same location
        //     and size of this System.Drawing.Rectangle structure.
        //
        // Parameters:
        //   obj:
        //     The System.Object to test.
        //
        // Returns:
        //     This method returns true if obj is a System.Drawing.Rectangle structure and its
        //     System.Drawing.Rectangle.X, System.Drawing.Rectangle.Y, System.Drawing.Rectangle.Width,
        //     and System.Drawing.Rectangle.Height properties are equal to the corresponding
        //     properties of this System.Drawing.Rectangle structure; otherwise, false.

        public override bool Equals(object obj)
        {
            Rectangle right = (Rectangle)obj;

            return X == right.X && Y == right.Y && Width == right.Width && Height == right.Height;
        }

        //
        // Summary:
        //     Returns the hash code for this System.Drawing.Rectangle structure. For information
        //     about the use of hash codes, see System.Object.GetHashCode .
        //
        // Returns:
        //     An integer that represents the hash code for this rectangle.
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }

        //
        // Summary:
        //     Enlarges this System.Drawing.Rectangle by the specified amount.
        //
        // Parameters:
        //   width:
        //     The amount to inflate this System.Drawing.Rectangle horizontally.
        //
        //   height:
        //     The amount to inflate this System.Drawing.Rectangle vertically.
        //public void Inflate(int width, int height);
        //
        // Summary:
        //     Enlarges this System.Drawing.Rectangle by the specified amount.
        //
        // Parameters:
        //   size:
        //     The amount to inflate this rectangle.
        //public void Inflate(Size size);
        //
        // Summary:
        //     Replaces this System.Drawing.Rectangle with the intersection of itself and the
        //     specified System.Drawing.Rectangle.
        //
        // Parameters:
        //   rect:
        //     The System.Drawing.Rectangle with which to intersect.
        //public void Intersect(Rectangle rect);
        //
        // Summary:
        //     Determines if this rectangle intersects with rect.
        //
        // Parameters:
        //   rect:
        //     The rectangle to test.
        //
        // Returns:
        //     This method returns true if there is any intersection, otherwise false.
        //public bool IntersectsWith(Rectangle rect);
        //
        // Summary:
        //     Adjusts the location of this rectangle by the specified amount.
        //
        // Parameters:
        //   x:
        //     The horizontal offset.
        //
        //   y:
        //     The vertical offset.
        //public void Offset(int x, int y);
        //
        // Summary:
        //     Adjusts the location of this rectangle by the specified amount.
        //
        // Parameters:
        //   pos:
        //     Amount to offset the location.
        //public void Offset(Point pos);
        //
        // Summary:
        //     Converts the attributes of this System.Drawing.Rectangle to a human-readable
        //     string.
        //
        // Returns:
        //     A string that contains the position, width, and height of this System.Drawing.Rectangle
        //     structure ¾ for example, {X=20, Y=20, Width=100, Height=50}
        public override string ToString()
        {
            return string.Format("({0},{1}, {2}, {3})", X, Y, Width, Height);
        }

        //
        // Summary:
        //     Tests whether two System.Drawing.Rectangle structures have equal location and
        //     size.
        //
        // Parameters:
        //   left:
        //     The System.Drawing.Rectangle structure that is to the left of the equality operator.
        //
        //   right:
        //     The System.Drawing.Rectangle structure that is to the right of the equality operator.
        //
        // Returns:
        //     This operator returns true if the two System.Drawing.Rectangle structures have
        //     equal System.Drawing.Rectangle.X, System.Drawing.Rectangle.Y, System.Drawing.Rectangle.Width,
        //     and System.Drawing.Rectangle.Height properties.
        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.X == right.X && left.Y == right.Y && left.Width == right.Width && left.Height == right.Height;
        }
        //
        // Summary:
        //     Tests whether two System.Drawing.Rectangle structures differ in location or size.
        //
        // Parameters:
        //   left:
        //     The System.Drawing.Rectangle structure that is to the left of the inequality
        //     operator.
        //
        //   right:
        //     The System.Drawing.Rectangle structure that is to the right of the inequality
        //     operator.
        //
        // Returns:
        //     This operator returns true if any of the System.Drawing.Rectangle.X, System.Drawing.Rectangle.Y,
        //     System.Drawing.Rectangle.Width or System.Drawing.Rectangle.Height properties
        //     of the two System.Drawing.Rectangle structures are unequal; otherwise false.
        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return left.X != right.X || left.Y != right.Y || left.Width != right.Width || left.Height != right.Height;
        }
    }


    //
    // Summary:
    //     Stores an ordered pair of floating-point numbers, typically the width and height
    //     of a rectangle.
    public struct SizeF
    {
        //
        // Summary:
        //     Gets a System.Drawing.SizeF structure that has a System.Drawing.SizeF.Height
        //     and System.Drawing.SizeF.Width value of 0.
        //
        // Returns:
        //     A System.Drawing.SizeF structure that has a System.Drawing.SizeF.Height and System.Drawing.SizeF.Width
        //     value of 0.
        public static readonly SizeF Empty = new SizeF(0,0);

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.SizeF structure from the specified
        //     existing System.Drawing.SizeF structure.
        //
        // Parameters:
        //   size:
        //     The System.Drawing.SizeF structure from which to create the new System.Drawing.SizeF
        //     structure.
        //public SizeF(SizeF size);
        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.SizeF structure from the specified
        //     System.Drawing.PointF structure.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.PointF structure from which to initialize this System.Drawing.SizeF
        //     structure.
        public SizeF(PointF pt)
        {
            Width = pt.X;
            Height = pt.Y;
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.SizeF structure from the specified
        //     dimensions.
        //
        // Parameters:
        //   width:
        //     The width component of the new System.Drawing.SizeF structure.
        //
        //   height:
        //     The height component of the new System.Drawing.SizeF structure.
        public SizeF(float width, float height)
        {
            Width = width;
            Height = height;
        }

        //
        // Summary:
        //     Gets a value that indicates whether this System.Drawing.SizeF structure has zero
        //     width and height.
        //
        // Returns:
        //     This property returns true when this System.Drawing.SizeF structure has both
        //     a width and height of zero; otherwise, false.

        public bool IsEmpty
        {
            get { return Width == 0 && Height == 0; }
        }
        //
        // Summary:
        //     Gets or sets the horizontal component of this System.Drawing.SizeF structure.
        //
        // Returns:
        //     The horizontal component of this System.Drawing.SizeF structure, typically measured
        //     in pixels.
        public float Width { get; set; }
        //
        // Summary:
        //     Gets or sets the vertical component of this System.Drawing.SizeF structure.
        //
        // Returns:
        //     The vertical component of this System.Drawing.SizeF structure, typically measured
        //     in pixels.
        public float Height { get; set; }

        //
        // Summary:
        //     Adds the width and height of one System.Drawing.SizeF structure to the width
        //     and height of another System.Drawing.SizeF structure.
        //
        // Parameters:
        //   sz1:
        //     The first System.Drawing.SizeF structure to add.
        //
        //   sz2:
        //     The second System.Drawing.SizeF structure to add.
        //
        // Returns:
        //     A System.Drawing.SizeF structure that is the result of the addition operation.
        public static SizeF Add(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }
        //
        // Summary:
        //     Subtracts the width and height of one System.Drawing.SizeF structure from the
        //     width and height of another System.Drawing.SizeF structure.
        //
        // Parameters:
        //   sz1:
        //     The System.Drawing.SizeF structure on the left side of the subtraction operator.
        //
        //   sz2:
        //     The System.Drawing.SizeF structure on the right side of the subtraction operator.
        //
        // Returns:
        //     A System.Drawing.SizeF structure that is a result of the subtraction operation.
        public static SizeF Subtract(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz2.Width - sz1.Width, sz2.Height - sz1.Height);
        }

        //
        // Summary:
        //     Tests to see whether the specified object is a System.Drawing.SizeF structure
        //     with the same dimensions as this System.Drawing.SizeF structure.
        //
        // Parameters:
        //   obj:
        //     The System.Object to test.
        //
        // Returns:
        //     This method returns true if obj is a System.Drawing.SizeF and has the same width
        //     and height as this System.Drawing.SizeF; otherwise, false.
        public override bool Equals(object obj)
        {
            SizeF size = (SizeF)obj;

            return (size.Width == Width && size.Height == Height);
        }

        //
        // Summary:
        //     Returns a hash code for this System.Drawing.Size structure.
        //
        // Returns:
        //     An integer value that specifies a hash value for this System.Drawing.Size structure.
        public override int GetHashCode()
        {
            return (Width.GetHashCode() ^ Height.GetHashCode());
        }

        //
        // Summary:
        //     Converts a System.Drawing.SizeF structure to a System.Drawing.PointF structure.
        //
        // Returns:
        //     Returns a System.Drawing.PointF structure.
        public PointF ToPointF()
        {
            return new PointF(Width, Height);
        }


        //
        // Summary:
        //     Converts a System.Drawing.SizeF structure to a System.Drawing.Size structure.
        //
        // Returns:
        //     Returns a System.Drawing.Size structure.
        //public Size ToSize();
        //
        // Summary:
        //     Creates a human-readable string that represents this System.Drawing.SizeF structure.
        //
        // Returns:
        //     A string that represents this System.Drawing.SizeF structure.

        public override string ToString()
        {
            return string.Format("({0},{1})", Width, Height);
        }

        //
        // Summary:
        //     Adds the width and height of one System.Drawing.SizeF structure to the width
        //     and height of another System.Drawing.SizeF structure.
        //
        // Parameters:
        //   sz1:
        //     The first System.Drawing.SizeF structure to add.
        //
        //   sz2:
        //     The second System.Drawing.SizeF structure to add.
        //
        // Returns:
        //     A System.Drawing.Size structure that is the result of the addition operation.
        public static SizeF operator +(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz2.Width + sz1.Width, sz2.Height + sz1.Height);
        }
        //
        // Summary:
        //     Subtracts the width and height of one System.Drawing.SizeF structure from the
        //     width and height of another System.Drawing.SizeF structure.
        //
        // Parameters:
        //   sz1:
        //     The System.Drawing.SizeF structure on the left side of the subtraction operator.
        //
        //   sz2:
        //     The System.Drawing.SizeF structure on the right side of the subtraction operator.
        //
        // Returns:
        //     A System.Drawing.SizeF that is the result of the subtraction operation.
        public static SizeF operator -(SizeF sz1, SizeF sz2)
        {
            return new SizeF(sz2.Width - sz1.Width, sz2.Height - sz1.Height);
        }
        //
        // Summary:
        //     Tests whether two System.Drawing.SizeF structures are equal.
        //
        // Parameters:
        //   sz1:
        //     The System.Drawing.SizeF structure on the left side of the equality operator.
        //
        //   sz2:
        //     The System.Drawing.SizeF structure on the right of the equality operator.
        //
        // Returns:
        //     This operator returns true if sz1 and sz2 have equal width and height; otherwise,
        //     false.
        public static bool operator ==(SizeF sz1, SizeF sz2)
        {
            return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
        }
        //
        // Summary:
        //     Tests whether two System.Drawing.SizeF structures are different.
        //
        // Parameters:
        //   sz1:
        //     The System.Drawing.SizeF structure on the left of the inequality operator.
        //
        //   sz2:
        //     The System.Drawing.SizeF structure on the right of the inequality operator.
        //
        // Returns:
        //     This operator returns true if sz1 and sz2 differ either in width or height; false
        //     if sz1 and sz2 are equal.
        public static bool operator !=(SizeF sz1, SizeF sz2)
        {
            return sz1.Width != sz2.Width || sz1.Height != sz2.Height;
        }

        //
        // Summary:
        //     Converts the specified System.Drawing.SizeF structure to a System.Drawing.PointF
        //     structure.
        //
        // Parameters:
        //   size:
        //     The System.Drawing.SizeF structure to be converted
        //
        // Returns:
        //     The System.Drawing.PointF structure to which this operator converts.
        public static explicit operator PointF(SizeF size)
        {
            return new PointF(size.Width, size.Height);
        }
    }

    //
    // Summary:
    //     Represents an ordered pair of floating-point x- and y-coordinates that defines
    //     a point in a two-dimensional plane.

    public struct PointF
    {
        //
        // Summary:
        //     Represents a new instance of the System.Drawing.PointF class with member data
        //     left uninitialized.
        public static readonly PointF Empty;

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.PointF class with the specified
        //     coordinates.
        //
        // Parameters:
        //   x:
        //     The horizontal position of the point.
        //
        //   y:
        //     The vertical position of the point.
        public PointF(float x, float y)
        {
            X = x;
            Y = y;
        }

        //
        // Summary:
        //     Gets a value indicating whether this System.Drawing.PointF is empty.
        //
        // Returns:
        //     true if both System.Drawing.PointF.X and System.Drawing.PointF.Y are 0; otherwise,
        //     false.
        public bool IsEmpty
        {
            get
            {
                return X == 0 && Y == 0;
            }
        }
        //
        // Summary:
        //     Gets or sets the x-coordinate of this System.Drawing.PointF.
        //
        // Returns:
        //     The x-coordinate of this System.Drawing.PointF.
        public float X { get; set; }
        //
        // Summary:
        //     Gets or sets the y-coordinate of this System.Drawing.PointF.
        //
        // Returns:
        //     The y-coordinate of this System.Drawing.PointF.
        public float Y { get; set; }

        //
        // Summary:
        //     Translates a given System.Drawing.PointF by a specified System.Drawing.SizeF.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.PointF to translate.
        //
        //   sz:
        //     The System.Drawing.SizeF that specifies the numbers to add to the coordinates
        //     of pt.
        //
        // Returns:
        //     The translated System.Drawing.PointF.
        public static PointF Add(PointF pt, SizeF sz)
        {
            return new PointF(pt.X + sz.Width, pt.X + sz.Height);
        }
        //
        // Summary:
        //     Translates a given System.Drawing.PointF by the specified System.Drawing.Size.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.PointF to translate.
        //
        //   sz:
        //     The System.Drawing.Size that specifies the numbers to add to the coordinates
        //     of pt.
        //
        // Returns:
        //     The translated System.Drawing.PointF.
        //public static PointF Add(PointF pt, Size sz)
       
        //
        // Summary:
        //     Translates a System.Drawing.PointF by the negative of a specified size.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.PointF to translate.
        //
        //   sz:
        //     The System.Drawing.Size that specifies the numbers to subtract from the coordinates
        //     of pt.
        //
        // Returns:
        //     The translated System.Drawing.PointF.
       // public static PointF Subtract(PointF pt, Size sz);
        //
        // Summary:
        //     Translates a System.Drawing.PointF by the negative of a specified size.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.PointF to translate.
        //
        //   sz:
        //     The System.Drawing.SizeF that specifies the numbers to subtract from the coordinates
        //     of pt.
        //
        // Returns:
        //     The translated System.Drawing.PointF.
       // public static PointF Subtract(PointF pt, SizeF sz);
        //
        // Summary:
        //     Specifies whether this System.Drawing.PointF contains the same coordinates as
        //     the specified System.Object.
        //
        // Parameters:
        //   obj:
        //     The System.Object to test.
        //
        // Returns:
        //     This method returns true if obj is a System.Drawing.PointF and has the same coordinates
        //     as this System.Drawing.Point.
        public override bool Equals(object obj)
        {
            PointF pnt = (PointF)obj;
            return pnt.X == X && pnt.Y == Y;
        }
        //
        // Summary:
        //     Returns a hash code for this System.Drawing.PointF structure.
        //
        // Returns:
        //     An integer value that specifies a hash value for this System.Drawing.PointF structure.
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        //
        // Summary:
        //     Converts this System.Drawing.PointF to a human readable string.
        //
        // Returns:
        //     A string that represents this System.Drawing.PointF.
        public override string ToString()
        {
            return string.Format("({0},{1})", X, Y);
        }

        //
        // Summary:
        //     Translates a System.Drawing.PointF by a given System.Drawing.Size.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.PointF to translate.
        //
        //   sz:
        //     A System.Drawing.Size that specifies the pair of numbers to add to the coordinates
        //     of pt.
        //
        // Returns:
        //     Returns the translated System.Drawing.PointF.
        //public static PointF operator +(PointF pt, Size sz);

        //
        // Summary:
        //     Translates the System.Drawing.PointF by the specified System.Drawing.SizeF.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.PointF to translate.
        //
        //   sz:
        //     The System.Drawing.SizeF that specifies the numbers to add to the x- and y-coordinates
        //     of the System.Drawing.PointF.
        //
        // Returns:
        //     The translated System.Drawing.PointF.
        //public static PointF operator +(PointF pt, SizeF sz);
        //
        // Summary:
        //     Translates a System.Drawing.PointF by the negative of a specified System.Drawing.SizeF.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.PointF to translate.
        //
        //   sz:
        //     The System.Drawing.SizeF that specifies the numbers to subtract from the coordinates
        //     of pt.
        //
        // Returns:
        //     The translated System.Drawing.PointF.
        //public static PointF operator -(PointF pt, SizeF sz);
        //
        // Summary:
        //     Translates a System.Drawing.PointF by the negative of a given System.Drawing.Size.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.PointF to translate.
        //
        //   sz:
        //     The System.Drawing.Size that specifies the numbers to subtract from the coordinates
        //     of pt.
        //
        // Returns:
        //     The translated System.Drawing.PointF.
        //public static PointF operator -(PointF pt, Size sz);
        //
        // Summary:
        //     Compares two System.Drawing.PointF structures. The result specifies whether the
        //     values of the System.Drawing.PointF.X and System.Drawing.PointF.Y properties
        //     of the two System.Drawing.PointF structures are equal.
        //
        // Parameters:
        //   left:
        //     A System.Drawing.PointF to compare.
        //
        //   right:
        //     A System.Drawing.PointF to compare.
        //
        // Returns:
        //     true if the System.Drawing.PointF.X and System.Drawing.PointF.Y values of the
        //     left and right System.Drawing.PointF structures are equal; otherwise, false.
        public static bool operator ==(PointF left, PointF right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        //
        // Summary:
        //     Determines whether the coordinates of the specified points are not equal.
        //
        // Parameters:
        //   left:
        //     A System.Drawing.PointF to compare.
        //
        //   right:
        //     A System.Drawing.PointF to compare.
        //
        // Returns:
        //     true to indicate the System.Drawing.PointF.X and System.Drawing.PointF.Y values
        //     of left and right are not equal; otherwise, false.
        public static bool operator !=(PointF left, PointF right)
        {
            return left.X != right.X || left.Y != right.Y;
        }
    }

    public static class ExtensionsForUwp
    {
        public static void Close(this System.IO.BinaryWriter bw)
        {

        }

        public static void Close(this System.IO.BinaryReader br)
        {

        }
        public static void Close(this System.IO.StreamReader br)
        {

        }

        public static void Close(this System.IO.StreamWriter br)
        {

        }
        public static void Close(this System.IO.StringWriter br)
        {

        }
        public static void Close(this System.IO.FileStream br)
        {

        }
        public static void Close(this SharpDX.DataStream br)
        {

        }
        public static void Close(this System.IO.Stream br)
        {

        }


        public static byte[] GetBuffer(this System.IO.MemoryStream ms)
        {
            System.ArraySegment<byte> segment = new System.ArraySegment<byte>();

            bool sucsess = ms.TryGetBuffer(out segment);

            if (sucsess)
            {
                return segment.Array;
            }

            return new byte[0];
        }
   
        //public static int ToArgb (this Windows.UI.Color color)
        //{
        //    return (int)((uint)color.A << 24 | (uint)color.R << 16 | (uint)color.G << 8 | (uint)color.B);
        //}

      
    }
    //
    // Summary:
    //     Stores an ordered pair of integers, which specify a System.Drawing.Size.Height
    //     and System.Drawing.Size.Width.

    public struct Size
    {
        //
        // Summary:
        //     Gets a System.Drawing.Size structure that has a System.Drawing.Size.Height and
        //     System.Drawing.Size.Width value of 0.
        //
        // Returns:
        //     A System.Drawing.Size that has a System.Drawing.Size.Height and System.Drawing.Size.Width
        //     value of 0.
        public static readonly Size Empty = new Size(0, 0);

        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Size structure from the specified
        //     System.Drawing.Point structure.
        //
        // Parameters:
        //   pt:
        //     The System.Drawing.Point structure from which to initialize this System.Drawing.Size
        //     structure.
        public Size(Point pt)
        {
            Width = pt.X;
            Height = pt.Y;
        }
        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Size structure from the specified
        //     dimensions.
        //
        // Parameters:
        //   width:
        //     The width component of the new System.Drawing.Size.
        //
        //   height:
        //     The height component of the new System.Drawing.Size.
        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        //
        // Summary:
        //     Tests whether this System.Drawing.Size structure has width and height of 0.
        //
        // Returns:
        //     This property returns true when this System.Drawing.Size structure has both a
        //     width and height of 0; otherwise, false.

        public bool IsEmpty
        {
            get { return Width == 0 && Height == 0; }
        }
        //
        // Summary:
        //     Gets or sets the horizontal component of this System.Drawing.Size structure.
        //
        // Returns:
        //     The horizontal component of this System.Drawing.Size structure, typically measured
        //     in pixels.
        public int Width { get; set; }
        //
        // Summary:
        //     Gets or sets the vertical component of this System.Drawing.Size structure.
        //
        // Returns:
        //     The vertical component of this System.Drawing.Size structure, typically measured
        //     in pixels.
        public int Height { get; set; }

        //
        // Summary:
        //     Adds the width and height of one System.Drawing.Size structure to the width and
        //     height of another System.Drawing.Size structure.
        //
        // Parameters:
        //   sz1:
        //     The first System.Drawing.Size structure to add.
        //
        //   sz2:
        //     The second System.Drawing.Size structure to add.
        //
        // Returns:
        //     A System.Drawing.Size structure that is the result of the addition operation.
        public static Size Add(Size sz1, Size sz2)
        {
            return new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
        }
        //
        // Summary:
        //     Converts the specified System.Drawing.SizeF structure to a System.Drawing.Size
        //     structure by rounding the values of the System.Drawing.Size structure to the
        //     next higher integer values.
        //
        // Parameters:
        //   value:
        //     The System.Drawing.SizeF structure to convert.
        //
        // Returns:
        //     The System.Drawing.Size structure this method converts to.
        //public static Size Ceiling(SizeF value);
        //
        // Summary:
        //     Converts the specified System.Drawing.SizeF structure to a System.Drawing.Size
        //     structure by rounding the values of the System.Drawing.SizeF structure to the
        //     nearest integer values.
        //
        // Parameters:
        //   value:
        //     The System.Drawing.SizeF structure to convert.
        //
        // Returns:
        //     The System.Drawing.Size structure this method converts to.
        //public static Size Round(SizeF value);
        //
        // Summary:
        //     Subtracts the width and height of one System.Drawing.Size structure from the
        //     width and height of another System.Drawing.Size structure.
        //
        // Parameters:
        //   sz1:
        //     The System.Drawing.Size structure on the left side of the subtraction operator.
        //
        //   sz2:
        //     The System.Drawing.Size structure on the right side of the subtraction operator.
        //
        // Returns:
        //     A System.Drawing.Size structure that is a result of the subtraction operation.
        public static Size Subtract(Size sz1, Size sz2)
        {
            return new Size(sz2.Width - sz1.Width, sz2.Height - sz1.Height);
        }
        //
        // Summary:
        //     Converts the specified System.Drawing.SizeF structure to a System.Drawing.Size
        //     structure by truncating the values of the System.Drawing.SizeF structure to the
        //     next lower integer values.
        //
        // Parameters:
        //   value:
        //     The System.Drawing.SizeF structure to convert.
        //
        // Returns:
        //     The System.Drawing.Size structure this method converts to.
        //public static Size Truncate(SizeF value);
        //
        // Summary:
        //     Tests to see whether the specified object is a System.Drawing.Size structure
        //     with the same dimensions as this System.Drawing.Size structure.
        //
        // Parameters:
        //   obj:
        //     The System.Object to test.
        //
        // Returns:
        //     true if obj is a System.Drawing.Size and has the same width and height as this
        //     System.Drawing.Size; otherwise, false.
        public override bool Equals(object obj)
        {
            Size size = (Size)obj;

            return (size.Width == Width && size.Height == Height);
        }
        //
        // Summary:
        //     Returns a hash code for this System.Drawing.Size structure.
        //
        // Returns:
        //     An integer value that specifies a hash value for this System.Drawing.Size structure.
        public override int GetHashCode()
        {
            return (Width.GetHashCode() ^ Height.GetHashCode());
        }
        //
        // Summary:
        //     Creates a human-readable string that represents this System.Drawing.Size structure.
        //
        // Returns:
        //     A string that represents this System.Drawing.Size.
        public override string ToString()
        {
            return string.Format("({0},{1})", Width, Height);
        }

        //
        // Summary:
        //     Adds the width and height of one System.Drawing.Size structure to the width and
        //     height of another System.Drawing.Size structure.
        //
        // Parameters:
        //   sz1:
        //     The first System.Drawing.Size to add.
        //
        //   sz2:
        //     The second System.Drawing.Size to add.
        //
        // Returns:
        //     A System.Drawing.Size structure that is the result of the addition operation.
        public static Size operator +(Size sz1, Size sz2){
            return new Size(sz2.Width + sz1.Width, sz2.Height + sz1.Height);
        }
        //
        // Summary:
        //     Subtracts the width and height of one System.Drawing.Size structure from the
        //     width and height of another System.Drawing.Size structure.
        //
        // Parameters:
        //   sz1:
        //     The System.Drawing.Size structure on the left side of the subtraction operator.
        //
        //   sz2:
        //     The System.Drawing.Size structure on the right side of the subtraction operator.
        //
        // Returns:
        //     A System.Drawing.Size structure that is the result of the subtraction operation.
        public static Size operator -(Size sz1, Size sz2)
        {
            return new Size(sz2.Width - sz1.Width, sz2.Height - sz1.Height);
        }

        //
        // Summary:
        //     Tests whether two System.Drawing.Size structures are equal.
        //
        // Parameters:
        //   sz1:
        //     The System.Drawing.Size structure on the left side of the equality operator.
        //
        //   sz2:
        //     The System.Drawing.Size structure on the right of the equality operator.
        //
        // Returns:
        //     true if sz1 and sz2 have equal width and height; otherwise, false.
        public static bool operator ==(Size sz1, Size sz2)
        {
            return sz1.Width == sz2.Width && sz1.Height == sz2.Height;
        }
        //
        // Summary:
        //     Tests whether two System.Drawing.Size structures are different.
        //
        // Parameters:
        //   sz1:
        //     The System.Drawing.Size structure on the left of the inequality operator.
        //
        //   sz2:
        //     The System.Drawing.Size structure on the right of the inequality operator.
        //
        // Returns:
        //     true if sz1 and sz2 differ either in width or height; false if sz1 and sz2 are
        //     equal.
        public static bool operator !=(Size sz1, Size sz2)
        {
            return sz1.Width != sz2.Width || sz1.Height != sz2.Height;
        }

        //
        // Summary:
        //     Converts the specified System.Drawing.Size structure to a System.Drawing.SizeF
        //     structure.
        //
        // Parameters:
        //   p:
        //     The System.Drawing.Size structure to convert.
        //
        // Returns:
        //     The System.Drawing.SizeF structure to which this operator converts.
        public static implicit operator SizeF(Size p)
        {
            return new Size(p.Width, p.Height);
        }
        //
        // Summary:
        //     Converts the specified System.Drawing.Size structure to a System.Drawing.Point
        //     structure.
        //
        // Parameters:
        //   size:
        //     The System.Drawing.Size structure to convert.
        //
        // Returns:
        //     The System.Drawing.Point structure to which this operator converts.
        public static explicit operator Point(Size size)
        {
            return new Point(size.Width, size.Height);
        }
    }
}
#endif