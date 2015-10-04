using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using MIDI;

namespace TerraViewer
{
    public partial class ButtonGroupControl : UserControl
    {
        public ButtonGroupControl()
        {
            InitializeComponent();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            Brush b = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), Color.FromArgb(20, 30, 39), Color.FromArgb(41, 49, 73));
            var p = new Pen(Color.FromArgb(71, 84, 108));
            g.FillRectangle(b, ClientRectangle);
            p.Dispose();
            GC.SuppressFinalize(p);
            b.Dispose();
            GC.SuppressFinalize(b);
        }

        ButtonGroup buttonGroup = new ButtonGroup();

        private void Plus_Click(object sender, EventArgs e)
        {
            var map = new ControlMap();

            var props = new ButtonProperties();

            props.ButtonMap = map;

            if (props.ShowDialog() == DialogResult.OK)
            {
                buttonGroup.Add(map);
                var pnt = FindFirstFreeSpot(new Size(140,33));
                map.X = pnt.X;
                map.Y = pnt.Y;
                AddButton(map);
                buttonGroup.Dirty = true;
            }

        }

        private Point FindFirstFreeSpot(Size size)
        {
            var yCount = Math.Max((Height / size.Height), 1);
            var xCount = Math.Max((Width / size.Width), 1);
            for (var x = 0; x < xCount; x++)
            {
                for (var y = 0; y < yCount; y++)
                {
                    var testPoint = new Point(x * size.Width + 20, y * size.Height);
                    var rect = new Rectangle(testPoint, size);
                    var conflict = false;
                    foreach (var button in buttonGroup.Buttons)
                    {
                        if (rect.IntersectsWith(button.Bounds))
                        {
                            conflict = true;
                            break;
                        }
                    }
                    if (!conflict)
                    {
                        return testPoint;
                    }

                }
            }

            return new Point(20, 10);
        }

        private void AddButton(ControlMap map)
        {
            switch (map.ButtonType)
            {
                case ButtonType.Button:
                    {
                        var button = new WwtButton();
                        map.Width = 140;
                        map.Height = 33;
                        button.Text = map.Name;
                        button.Location = new Point((int)map.X, (int)map.Y);
                        button.Click += button_Click;
                        button.MouseDown += button_MouseDown;
                        button.MouseMove += button_MouseMove;
                        button.MouseUp += button_MouseUp;
                        button.Tag = map;

                        Controls.Add(button);
                    }
                    break;
                case ButtonType.Checkbox:
                    {
                        var button = new WWTCheckbox();
                        map.Width = 140;
                        map.Height = 33;
                        //button.Parent = this;
                        button.Text = map.Name;
                        button.Location = new Point((int)map.X, (int)map.Y);
                        button.CheckedChanged += button_CheckedChanged;
                        button.MouseDown += button_MouseDown;
                        button.MouseMove += button_MouseMove;
                        button.MouseUp += button_MouseUp;
                        button.Tag = map;

                        Controls.Add(button);
                    }
                    break;
                case ButtonType.Slider:
                    {
                        var button = new TrackButton();
                        map.Width = 140;
                        map.Height = 33;
                        //button.Parent = this;
                        button.LabelText = map.Name;
                        button.Location = new Point((int)map.X, (int)map.Y);
                        button.ValueChanged += button_ValueChanged;
                        button.MouseDown += button_MouseDown;
                        button.MouseMove += button_MouseMove;
                        button.MouseUp += button_MouseUp;
                        button.Tag = map;
                        button.Value = (int)map.GetValue(MidiMessage.NoteOn, -1, 0);
                        Controls.Add(button);
                    }
                    break;
                default:
                    break;
            }

            EnableUserChildren(!editMode);
        }

     



        void button_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseDown && editMode)
            {

            }
            mouseDown = false;
        }

        void button_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown && editMode)
            {
                var button = sender as UserControl;
                if (button != null)
                {
                    var posDelta = Point.Subtract(e.Location, new Size(mouseDownPoint.X, mouseDownPoint.Y));
                    button.Location = Point.Add(button.Location, new Size(posDelta.X, posDelta.Y));

                    var map = button.Tag as ControlMap;
                    map.X = button.Location.X;
                    map.Y = button.Location.Y;
                    buttonGroup.Dirty = true;
                }
            }
        }
        bool mouseDown;
        Point mouseDownPoint;
        void button_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownPoint = e.Location;
            mouseDown = false;
            
            if (MouseButtons == MouseButtons.Right)
            {
                var contextMenu = new ContextMenuStrip();

                var properties = new ToolStripMenuItem("Properties");
                var delete = new ToolStripMenuItem("Delete");
                var edit = new ToolStripMenuItem("Edit Mode");
                properties.Click += properties_Click;
                delete.Click += delete_Click;
                edit.Click += edit_Click;
                edit.Checked = editMode;
                properties.Tag = sender as UserControl;
                delete.Tag = sender as UserControl;
                contextMenu.Items.Add(edit);
                contextMenu.Items.Add(properties);
                contextMenu.Items.Add(delete);
                contextMenu.Show(Cursor.Position);
            }
            else
            {
                if (editMode)
                {

                    mouseDown = true;
                }
            }

        }

        void edit_Click(object sender, EventArgs e)
        {
            ToggleEditMode();
        }

        void button_Click(object sender, EventArgs e)
        {
            if (!editMode)
            {
                var map = (ControlMap)((WwtButton)sender).Tag;

                map.DispatchMessage(MidiMessage.NoteOn, -1, 0, 127);
            }
        }

        bool ignoreEvent;
        void button_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreEvent)
            {
                ignoreEvent = true;
                var checkbox = sender as WWTCheckbox;
                if (!editMode)
                {
                    var map = (ControlMap)((UserControl)sender).Tag;

                    checkbox.Checked = map.DispatchMessage(MidiMessage.NoteOn, -1, 0, 127);
                }
                ignoreEvent = false;
            }
        }

        void button_ValueChanged(object sender, EventArgs e)
        {
            var tb = sender as TrackButton;

            if (!editMode)
            {
                var map = (ControlMap)((UserControl)sender).Tag;

                map.DispatchMessage(MidiMessage.NoteOn, -1, 0, tb.Value);
            }
        }

        void properties_Click(object sender, EventArgs e)
        {
            var button = (UserControl)((ToolStripMenuItem)sender).Tag;
            var map = (ControlMap)button.Tag;
            var props = new ButtonProperties();

            props.ButtonMap = map;

            if (props.ShowDialog() == DialogResult.OK)
            {
                CleanupButton(button);
                AddButton(map);
                buttonGroup.Dirty = true;
            }
        }

        void delete_Click(object sender, EventArgs e)
        {
            var button = (UserControl)((ToolStripMenuItem)sender).Tag;
            var map = (ControlMap)button.Tag;

            buttonGroup.Remove(map);
            CleanupButton(button);



        }

        private void CleanupButton(UserControl button)
        {
            button.Parent = null;

            button.MouseDown -= button_MouseDown;
            button.MouseMove -= button_MouseMove;
            button.MouseUp -= button_MouseUp;
            Controls.Remove(button);
            button.Dispose();
        }

        bool editMode;
        private void EditButtons_Click(object sender, EventArgs e)
        {
            ToggleEditMode();
           
        }

        private void ToggleEditMode()
        {
            editMode = !editMode;
            EditButtons.ForeColor = editMode ? Color.Green : Color.White;
            buttonGroup.Dirty = true;

            EnableUserChildren(!editMode);
        }

        private void EnableUserChildren(bool enabled)
        {
            foreach (Control child in Controls)
            {
                var ucChild = child as UserControl;
                if (ucChild != null)
                {
                    EnableChildren(ucChild, enabled);
                }
            }
        }

        private static void EnableChildren(UserControl ucChild, bool enabled)
        {
            foreach (Control grandChild in ucChild.Controls)
            {
                grandChild.Enabled = enabled;
            }
        }

        public void LoadButtons(string filename)
        {
            buttonGroup = ButtonGroup.FromFile(filename);

            foreach (var map in buttonGroup.Buttons)
            {
                AddButton(map);
            }
        }


        internal void Save()
        {
            buttonGroup.SaveIfDirty();
        }

        private void ButtonGroupControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseButtons == MouseButtons.Right)
            {
                var contextMenu = new ContextMenuStrip();

                var edit = new ToolStripMenuItem("Edit Mode");
              
                edit.Click += edit_Click;
                edit.Checked = editMode;
                contextMenu.Items.Add(edit);

                contextMenu.Show(Cursor.Position);
            }
        }
    }
}
