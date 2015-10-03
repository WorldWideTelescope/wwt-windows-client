using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class KeyProperties : Form
    {

        static public void ShowProperties(Dictionary<string, VisibleKey> selectedKeys)
        {
            if (singleton == null)
            {
                if (selectedKeys == null || selectedKeys.Count == 0)
                {
                    return;
                }
                singleton = new KeyProperties();
                singleton.Owner = Earth3d.MainWindow;
            }


            if (selectedKeys == null || selectedKeys.Count == 0)
            {
                singleton.Hide();
            }
            else
            {
                singleton.Show();
                bool visible = false;
                foreach (var screen in Screen.AllScreens)
                {
                    if (screen.WorkingArea.Contains(singleton.DesktopLocation))
                    {
                        visible = true;
                        break;
                    }
                }
                if (!visible)
                {
                    Point parent = Earth3d.MainWindow.DesktopLocation;
                    singleton.Location = new Point(parent.X + 100, parent.Y +100);
                }

            }

            singleton.Keys = selectedKeys;
        }

        static public void HideProperties()
        {
            if (singleton != null)
            {
                singleton.Close();
            }
        }

        static KeyProperties singleton = null;
        public KeyProperties()
        {
            InitializeComponent();
        }

        private void SetUiStrings()
        {
            this.KeyTypeLabel.Text = Language.GetLocalizedText(1348, "Transition Function");
            this.TimeLabel.Text = Language.GetLocalizedText(1349, "Time");
            this.ValueLabel.Text = Language.GetLocalizedText(668, "Value");
            this.Text = Language.GetLocalizedText(1350, "Key Properties");
        }

        private Dictionary<string, VisibleKey> keys = null;

        public Dictionary<string, VisibleKey> Keys
        {
            get { return keys; }
            set
            {
                keys = value;
                if (keys != null)
                {
                    LoadTypeCombo();
                }
            }
        }

        bool initializing = false;

        private void KeyProperties_Load(object sender, EventArgs e)
        {
            LoadTypeCombo();
        }

        bool sameTimes = false;
        bool sameValues = false;
        double propValue = 0;
        double propTime = 0;
        private void LoadTypeCombo()
        {
            initializing = true;
            keyType.Items.Clear();

            int selectedIndex = 0;

            VisibleKey vKey = null;
            Key key = null;

            sameTimes = IsHomogeneousTime();
            sameValues = IsHomogeneousValue();



            Time.ReadOnly = !sameTimes;
            CurrentValue.ReadOnly = !sameValues;

            if (this.propTime == 0)
            {
                Time.ReadOnly = true;
            }

            if (keys != null && keys.Values.Count > 0)
            {
                vKey = keys.Values.First();
                key = vKey.Target.GetKey(vKey.ParameterIndex, vKey.Time);
                curveEditor1.P1 = key.P1;
                curveEditor1.P2 = key.P2;
                curveEditor1.P3 = key.P3;
                curveEditor1.P4 = key.P4;
                curveEditor1.CurveType = key.InFunction;
                curveEditor1.Invalidate();

                if (sameValues)
                {
                    propValue = key.Value;
                    CurrentValue.Text = key.Value.ToString();
                    ValueLabel.Text = vKey.PropertyName;
                }
                else
                {
                    CurrentValue.Text = Language.GetLocalizedText(1351, "Multiple");
                    ValueLabel.Text = Language.GetLocalizedText(668, "Value");
                }

                if (sameTimes)
                {
                    Time.Text = FormatTime(key.Time);
                    propTime = key.Time;
                }
                else
                {
                    Time.Text = Language.GetLocalizedText(1351, "Multiple");
                }

            }

            string selected = "";

 
            if (key != null)
            {
                selected = key.InFunction.ToString();
            }

            foreach (string name in Enum.GetNames(typeof(Key.KeyType)))
            {
                int index = keyType.Items.Add(name);
                if (name == selected)
                {
                    selectedIndex = index;
                }
            }

            keyType.SelectedIndex = selectedIndex;
            initializing = false;
            dirtyEdit = false;
        }

        private string FormatTime(double p)
        {
            if (Earth3d.MainWindow.TourEdit != null && Earth3d.MainWindow.TourEdit.Tour != null && Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop != null)
            {
                double timeInSeconds = (Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.Duration.TotalSeconds * p) + 1/60.0;
                int secondsPart = (int)timeInSeconds;
                int framesPart = (int)((timeInSeconds - secondsPart) * 30 );

                TimeSpan ts = TimeSpan.FromSeconds(secondsPart);

                return string.Format("{0:0#}:{1:0#}.{2:0#}", ts.Minutes, ts.Seconds, framesPart);
            }
            return p.ToString();

        }

        private void RefreshTween()
        {

            if (Earth3d.MainWindow.TourEdit != null && Earth3d.MainWindow.TourEdit.Tour != null && Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop != null)
            {
                float currentTween = Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.TweenPosition;
                Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.TweenPosition = 0;
                Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.TweenPosition = currentTween;

            }

        }

        private double ParseTime(string time)
        {
            try
            {
                if (Earth3d.MainWindow.TourEdit != null && Earth3d.MainWindow.TourEdit.Tour != null && Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop != null)
                {
                    string[] parts = time.Split(new char[] { '.' });

                    int frame = 0;

                    if (parts.Length > 1)
                    {
                        frame = int.Parse(parts[1]);
                    }

                    string[] minsec = parts[0].Split(new char[] { ':' });

                    int secondsPart = 0;
                    int minutesPart = 0;

                    if (minsec.Length > 1)
                    {
                        secondsPart = int.Parse(minsec[1]);
                        minutesPart = int.Parse(minsec[0]);
                    }
                    else
                    {
                        secondsPart = int.Parse(minsec[0]);
                    }

                    double totalTime = ((minutesPart * 60 * 30) + (secondsPart * 30) + frame) / (Earth3d.MainWindow.TourEdit.Tour.CurrentTourStop.Duration.TotalSeconds * 30);

                    return totalTime;
                }
            }
            catch
            {
            }

            return -1;
        }

        private bool IsHomogeneousTime()
        {
            if (keys != null)
            {
                
                int currentTime = 0;
                bool first = true;
                foreach (VisibleKey vk in keys.Values)
                {
                    Key key = vk.Target.GetKey(vk.ParameterIndex, vk.Time);
                    if (first)
                    {
                        currentTime = KeyGroup.Quant(key.Time); 
                        first = false;
                    }
                    else
                    {
                        if (KeyGroup.Quant(key.Time) != currentTime)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            return false;
        }

        private bool IsHomogeneousValue()
        {
            if (keys != null)
            {
                double currentValue = 0;
                bool first = true;
                foreach (VisibleKey vk in keys.Values)
                {
                    Key key = vk.Target.GetKey(vk.ParameterIndex, vk.Time);
                    if (first)
                    {
                        currentValue = key.Value;
                        first = false;
                    }
                    else
                    {
                        if (key.Value != currentValue)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            return false;
        }

        private void keyType_SelectionChanged(object sender, EventArgs e)
        {
            if (initializing)
            {
                return;
            }
            Key.KeyType type = (Key.KeyType)Enum.Parse(typeof(Key.KeyType), keyType.SelectedItem.ToString());

            curveEditor1.CurveType = type;

            if (keys != null)
            {
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1352, "Key Properties Edit"), Earth3d.MainWindow.TourEdit.Tour));
                foreach (VisibleKey vk in keys.Values)
                {
                    Key key = vk.Target.GetKey(vk.ParameterIndex, vk.Time);
                    if (key != null)
                    {
                        key.InFunction = type;
                        key.P1 = curveEditor1.P1;
                        key.P2 = curveEditor1.P2;
                        key.P3 = curveEditor1.P3;
                        key.P4 = curveEditor1.P4;
                    }
                }
                TimeLine.RefreshUi(false);
                RefreshTween();
            }
        }

        private void KeyProperties_FormClosed(object sender, FormClosedEventArgs e)
        {
            singleton = null;
        }

        private void curveEditor1_ValueChanged(object sender, EventArgs e)
        {
            if (initializing)
            {
                return;
            }
            if (keys != null)
            {
                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1352, "Key Properties Edit"), Earth3d.MainWindow.TourEdit.Tour));
                foreach (VisibleKey vk in keys.Values)
                {
                    Key key = vk.Target.GetKey(vk.ParameterIndex, vk.Time);
                    if (key != null)
                    {
                        key.InFunction = (Key.KeyType)Enum.Parse(typeof(Key.KeyType), keyType.SelectedItem.ToString());
                        key.P1 = curveEditor1.P1;
                        key.P2 = curveEditor1.P2;
                        key.P3 = curveEditor1.P3;
                        key.P4 = curveEditor1.P4;
                    }
                }
                RefreshTween();
            }
        }
        bool dirtyEdit = false;
        private void CurrentValue_Validating(object sender, CancelEventArgs e)
        {
            if (initializing || CurrentValue.Text == Language.GetLocalizedText(1351, "Multiple") || dirtyEdit == false)
            {
                return;
            }
            bool failed = false;
            
            double newValue = UiTools.ParseAndValidateDouble(CurrentValue, propValue, ref failed);

            if (newValue != propValue && !failed)
            {
                if (keys != null)
                {
                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1352, "Key Properties Edit"), Earth3d.MainWindow.TourEdit.Tour));
                    foreach (VisibleKey vk in keys.Values)
                    {
                        Key key = vk.Target.GetKey(vk.ParameterIndex, vk.Time);
                        if (key != null)
                        {
                            key.Value = newValue;
                        }
                    }
                }
                TimeLine.RefreshUi(false);
                RefreshTween();
            }
        }

        private void Time_TextChanged(object sender, EventArgs e)
        {
            dirtyEdit = true;
        }

        private void Time_Validating(object sender, CancelEventArgs e)
        {
            if (initializing || Time.Text == Language.GetLocalizedText(1351, "Multiple") || dirtyEdit == false)
            {
                return;
            }


            double newValue = ParseTime(Time.Text);

            if (newValue == -1)
            {
                Time.BackColor = Color.Red;
            }
            else
            {
                Time.BackColor = UiTools.TextBackground;
            }

            if (newValue != propTime)
            {
                if (keys != null)
                {
                    Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1352, "Key Properties Edit"), Earth3d.MainWindow.TourEdit.Tour));
                    foreach (VisibleKey vk in keys.Values)
                    {
                        Key key = vk.Target.GetKey(vk.ParameterIndex, vk.Time);
                        if (key != null)
                        {
                            vk.Target.MoveKey(vk.ParameterIndex, vk.Time, newValue);
                        }
                    }
                }
                TimeLine.RefreshUi(false);
                RefreshTween();
            }
        }

        private void CurrentValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                this.keyType.Focus();
            }
        }

        private void KeyProperties_Leave(object sender, EventArgs e)
        {

        }

        private void KeyProperties_Deactivate(object sender, EventArgs e)
        {
            ValidateChildren();
        }

        private void CurrentValue_TextChanged(object sender, EventArgs e)
        {
            dirtyEdit = true;
        }
    }
}
