using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Drawing.Drawing2D;

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
            Graphics g = e.Graphics;
            Brush b = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), Color.FromArgb(20, 30, 39), Color.FromArgb(41, 49, 73));
            Pen p = new Pen(Color.FromArgb(71, 84, 108));
            g.FillRectangle(b, this.ClientRectangle);
            p.Dispose();
            GC.SuppressFinalize(p);
            b.Dispose();
            GC.SuppressFinalize(b);
        }

        ButtonGroup buttonGroup = new ButtonGroup();

        private void Plus_Click(object sender, EventArgs e)
        {
            ControlMap map = new ControlMap();

            ButtonProperties props = new ButtonProperties();

            props.ButtonMap = map;

            if (props.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                buttonGroup.Add(map);
                Point pnt = FindFirstFreeSpot(new Size(140,33));
                map.X = pnt.X;
                map.Y = pnt.Y;
                AddButton(map);
                buttonGroup.Dirty = true;
            }

        }

        private Point FindFirstFreeSpot(Size size)
        {
            int yCount = Math.Max((this.Height / size.Height), 1);
            int xCount = Math.Max((this.Width / size.Width), 1);
            for (int x = 0; x < xCount; x++)
            {
                for (int y = 0; y < yCount; y++)
                {
                    Point testPoint = new Point(x * size.Width + 20, y * size.Height);
                    Rectangle rect = new Rectangle(testPoint, size);
                    bool conflict = false;
                    foreach (ControlMap button in buttonGroup.Buttons)
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
                        WwtButton button = new WwtButton();
                        map.Width = 140;
                        map.Height = 33;
                        button.Text = map.Name;
                        button.Location = new Point((int)map.X, (int)map.Y);
                        button.Click += new EventHandler(button_Click);
                        button.MouseDown += new MouseEventHandler(button_MouseDown);
                        button.MouseMove += new MouseEventHandler(button_MouseMove);
                        button.MouseUp += new MouseEventHandler(button_MouseUp);
                        button.Tag = map;

                        Controls.Add(button);
                    }
                    break;
                case ButtonType.Checkbox:
                    {
                        WWTCheckbox button = new WWTCheckbox();
                        map.Width = 140;
                        map.Height = 33;
                        //button.Parent = this;
                        button.Text = map.Name;
                        button.Location = new Point((int)map.X, (int)map.Y);
                        button.CheckedChanged += new EventHandler(button_CheckedChanged);
                        button.MouseDown += new MouseEventHandler(button_MouseDown);
                        button.MouseMove += new MouseEventHandler(button_MouseMove);
                        button.MouseUp += new MouseEventHandler(button_MouseUp);
                        button.Tag = map;

                        Controls.Add(button);
                    }
                    break;
                case ButtonType.Slider:
                    {
                        TrackButton button = new TrackButton();
                        map.Width = 140;
                        map.Height = 33;
                        //button.Parent = this;
                        button.LabelText = map.Name;
                        button.Location = new Point((int)map.X, (int)map.Y);
                        button.ValueChanged += new EventHandler(button_ValueChanged);
                        button.MouseDown += new MouseEventHandler(button_MouseDown);
                        button.MouseMove += new MouseEventHandler(button_MouseMove);
                        button.MouseUp += new MouseEventHandler(button_MouseUp);
                        button.Tag = map;
                        button.Value = (int)map.GetValue(MIDI.MidiMessage.NoteOn, -1, 0);
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
                UserControl button = sender as UserControl;
                if (button != null)
                {
                    Point posDelta = Point.Subtract(e.Location, new Size(mouseDownPoint.X, mouseDownPoint.Y));
                    button.Location = Point.Add(button.Location, new Size(posDelta.X, posDelta.Y));

                    ControlMap map = button.Tag as ControlMap;
                    map.X = button.Location.X;
                    map.Y = button.Location.Y;
                    buttonGroup.Dirty = true;
                }
            }
        }
        bool mouseDown = false;
        Point mouseDownPoint;
        void button_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownPoint = e.Location;
            mouseDown = false;
            
            if (Control.MouseButtons == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();

                ToolStripMenuItem properties = new ToolStripMenuItem("Properties");
                ToolStripMenuItem delete = new ToolStripMenuItem("Delete");
                ToolStripMenuItem edit = new ToolStripMenuItem("Edit Mode");
                properties.Click += new EventHandler(properties_Click);
                delete.Click += new EventHandler(delete_Click);
                edit.Click += new EventHandler(edit_Click);
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
                ControlMap map = (ControlMap)((WwtButton)sender).Tag;

                map.DispatchMessage(MIDI.MidiMessage.NoteOn, -1, 0, 127);
            }
        }

        bool ignoreEvent = false;
        void button_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreEvent)
            {
                ignoreEvent = true;
                WWTCheckbox checkbox = sender as WWTCheckbox;
                if (!editMode)
                {
                    ControlMap map = (ControlMap)((UserControl)sender).Tag;

                    checkbox.Checked = map.DispatchMessage(MIDI.MidiMessage.NoteOn, -1, 0, 127);
                }
                ignoreEvent = false;
            }
        }

        void button_ValueChanged(object sender, EventArgs e)
        {
            TrackButton tb = sender as TrackButton;

            if (!editMode)
            {
                ControlMap map = (ControlMap)((UserControl)sender).Tag;

                map.DispatchMessage(MIDI.MidiMessage.NoteOn, -1, 0, tb.Value);
            }
        }

        void properties_Click(object sender, EventArgs e)
        {
            UserControl button = (UserControl)((ToolStripMenuItem)sender).Tag;
            ControlMap map = (ControlMap)button.Tag;
            ButtonProperties props = new ButtonProperties();

            props.ButtonMap = map;

            if (props.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CleanupButton(button);
                AddButton(map);
                buttonGroup.Dirty = true;
            }
        }

        void delete_Click(object sender, EventArgs e)
        {
            UserControl button = (UserControl)((ToolStripMenuItem)sender).Tag;
            ControlMap map = (ControlMap)button.Tag;

            buttonGroup.Remove(map);
            CleanupButton(button);



        }

        private void CleanupButton(UserControl button)
        {
            button.Parent = null;

            button.MouseDown -= new MouseEventHandler(button_MouseDown);
            button.MouseMove -= new MouseEventHandler(button_MouseMove);
            button.MouseUp -= new MouseEventHandler(button_MouseUp);
            Controls.Remove(button);
            button.Dispose();
        }

        bool editMode = false;
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
                UserControl ucChild = child as UserControl;
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

            foreach (ControlMap map in buttonGroup.Buttons)
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
            if (Control.MouseButtons == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();

                ToolStripMenuItem edit = new ToolStripMenuItem("Edit Mode");
              
                edit.Click += new EventHandler(edit_Click);
                edit.Checked = editMode;
                contextMenu.Items.Add(edit);

                contextMenu.Show(Cursor.Position);
            }
        }
    }
}
