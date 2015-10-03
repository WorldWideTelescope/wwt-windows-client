using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;

namespace TerraViewer
{
    public partial class XBoxConfig : Form
    {
        public XBoxConfig()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.Control.Text = Language.GetLocalizedText(1159, "Control");
            this.Binding.Text = Language.GetLocalizedText(1161, "Binding");
            this.Repeat.Text = Language.GetLocalizedText(1168, "Repeat");
            this.Property.Text = Language.GetLocalizedText(1166, "Property");
            this.BindTypeLabel.Text = Language.GetLocalizedText(1165, "Bind Type");
            this.BindingTargetTypeLabel.Text = Language.GetLocalizedText(1164, "Binding Target Type");
            this.label1.Text = Language.GetLocalizedText(1318, "Left Sholder");
            this.label2.Text = Language.GetLocalizedText(1319, "Left Trigger");
            this.label3.Text = Language.GetLocalizedText(1320, "Right Trigger");
            this.label6.Text = Language.GetLocalizedText(1321, "Right Sholder");
            this.label7.Text = Language.GetLocalizedText(1322, "A, B, X, Y Buttons");
            this.label8.Text = Language.GetLocalizedText(1323, "Right Thumb");
            this.label9.Text = Language.GetLocalizedText(1324, "D-Pad");
            this.label10.Text = Language.GetLocalizedText(1325, "Left Thumb");
            this.label11.Text = Language.GetLocalizedText(913, "Back");
            this.label12.Text = Language.GetLocalizedText(1326, "Start");
            this.modeLabel.Text = Language.GetLocalizedText(1327, "Mode");
            this.export.Text = Language.GetLocalizedText(1328, "Export");
            this.Import.Text = Language.GetLocalizedText(1329, "Import");
            this.RepeatCheckbox.Text = Language.GetLocalizedText(1168, "Repeat");
            this.ModeDependentMaps.Text = Language.GetLocalizedText(1330, "Use Mode Dependent Mappings");
            this.UseCustomMaps.Text = Language.GetLocalizedText(1331, "Use Custom Mappings");
            this.Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            this.OK.Text = Language.GetLocalizedText(156, "OK");
            this.Text = Language.GetLocalizedText(1332, "XBox 360 Controller Configuration");
        }

        static XBoxConfig()
        {
            if (!LoadMapPack())
            {


                for (var i = 0; i < 7; i++)
                {

                    var map = new XboxMap();
                    map.ControlMaps = MakeControlGroups();
                    if (i < 5)
                    {
                        map.Name = Enum.GetName(typeof(ImageSetType), (ImageSetType)i);
                    }
                    else if (i == 5)
                    {
                        map.Name = "Sky - Local Horizon Mode";
                    }
                    else
                    {
                        map.Name = "Modeless Xbox Controller Map";
                    }
                    map.Slot = i;
                    xboxMaps.Add(map);
                }
            }
            //LoadMaps();
        }


        static XBoxConfig master;

        public static void ShowSetupWindow()
        {
            if (master == null)
            {
                master = new XBoxConfig();
                master.Owner = Earth3d.MainWindow;
                master.Show();
            }
            else
            {
                master.Show();
                master.Activate();
            }
        }


        private void XBoxConfig_Load(object sender, EventArgs e)
        {
          

            ModeCombo.Items.AddRange(Enum.GetNames(typeof(ImageSetType)));
            ModeCombo.Items.Add("Sky - Local Horizon Mode");
            SetCustomControls(Properties.Settings.Default.XboxCustomMapping);
            SetupBindingCombos();

            UseCustomMaps.Checked = Properties.Settings.Default.XboxCustomMapping;
            ModeDependentMaps.Checked = Properties.Settings.Default.XboxModeMapping;
            ignoreChanges = 0;
        }
        

        public enum XboxButtons { DirectionPadUp, DirectionPadDown, DirectionPadLeft, DirectionPadRight, Start, Back, LeftThumbClick, RightThumbClick, LeftShoulder, RightShoulder, A, B, X, Y, LeftTrigger, RightTrigger, LeftThumbX, LeftThumbY, RightThumbX, RightThumbY };


        public static bool DispatchXboxEvent(XboxButtons id, double value)
        {
            var mode = (int)Earth3d.MainWindow.CurrentImageSet.DataSetType;

            if (Earth3d.MainWindow.CurrentImageSet.DataSetType == ImageSetType.Sky && Properties.Settings.Default.LocalHorizonMode)
            {
                mode = 5;
            }


            if (!Properties.Settings.Default.XboxModeMapping)
            {
                mode = 6;
            }

            var map = xboxMaps[mode].ControlMaps[(int)id];

            map.BindingA.DispatchMessage(MIDI.MidiMessage.NoteOn, -1, 0, value * 127);


            return !map.AutoRepeat;
        }

        private void TargetTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (ignoreChanges > 0)
            {
                return;
            }
            UpdatePropertyCombo();
            SetDirty();
        }

        private void UpdatePropertyCombo()
        {
            var tt = (BindingTargetType)TargetTypeCombo.SelectedIndex;

            TargetPropertyCombo.Items.Clear();
            TargetPropertyCombo.ClearText();
            filterLabel.Visible = false;
            filterList.Visible = false;
            var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;


            binding.TargetType = tt;

            IScriptable scriptInterface = null;
            var comboVisible = true;
            switch (tt)
            {
                case BindingTargetType.Setting:
                    scriptInterface = Settings.Active as IScriptable;
                    break;
                case BindingTargetType.SpaceTimeController:
                    scriptInterface = SpaceTimeController.ScriptInterface;
                    break;
                case BindingTargetType.Goto:
                    comboVisible = false;
                    break;

                case BindingTargetType.Layer:
                    scriptInterface = LayerManager.ScriptInterface;
                    break;
                
                case BindingTargetType.Navigation:
                    scriptInterface = Earth3d.MainWindow as IScriptable;
                    break;
                //case BindingTargetType.Actions:
                //    break;
                //case BindingTargetType.Key:
                //    break;
                //case BindingTargetType.Mouse:
                //    break;
                default:
                    break;
            }

            if (comboVisible)
            {
                TargetPropertyCombo.Visible = true;
                PropertyNameText.Visible = false;
                if (scriptInterface != null)
                {
                    switch (binding.BindingType)
                    {
                        case BindingType.Action:
                            TargetPropertyCombo.Items.Clear();
                            TargetPropertyCombo.Items.AddRange(scriptInterface.GetActions());
                            break;
                        case BindingType.Toggle:
                            TargetPropertyCombo.Items.Clear();
                            TargetPropertyCombo.Items.AddRange(UiTools.GetFilteredProperties(scriptInterface.GetProperties(), binding.BindingType));
                            break;
                        case BindingType.SyncValue:
                            TargetPropertyCombo.Items.Clear();
                            TargetPropertyCombo.Items.AddRange(scriptInterface.GetProperties());
                            break;
                        case BindingType.SetValue:
                            TargetPropertyCombo.Items.Clear();
                            TargetPropertyCombo.Items.AddRange(scriptInterface.GetProperties());
                            break;
                        default:
                            break;
                    }
                }
                TargetPropertyCombo.SelectedItem = binding.PropertyName;
            }
            else
            {
                PropertyNameText.Visible = true;
                TargetPropertyCombo.Visible = false;
                PropertyNameText.Text = binding.PropertyName;
            }

            BindTypeLabel.Visible = comboVisible;
            BindTypeCombo.Visible = comboVisible;
        }

        private void BindTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (ignoreChanges > 0)
            {
                return;
            }
            var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

            binding.BindingType = (BindingType)BindTypeCombo.SelectedIndex;
            UpdatePropertyCombo();
            SetDirty();
        }

        private void TargetPropertyCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (ignoreChanges > 0)
            {
                return;
            }

            filterLabel.Visible = false;
            filterList.Visible = false;

            var prop = TargetPropertyCombo.SelectedItem as ScriptableProperty;
            var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

            if (prop != null)
            {
                binding.PropertyName = prop.Name;
                binding.Max = prop.Max;
                binding.Min = prop.Min;
                binding.Integer = prop.Type == ScriptablePropertyTypes.Integer;

                if (binding.BindingType == BindingType.SetValue && prop.Type == ScriptablePropertyTypes.ConstellationFilter)
                {
                    filterLabel.Visible = true;
                    filterList.Visible = true;
                    filterList.Items.Clear();
                    var index = 0;
                    var selectedIndex = 0;
                    foreach (var name in ConstellationFilter.Families.Keys)
                    {
                        filterList.Items.Add(name);
                        if (name == binding.Value)
                        {
                            selectedIndex = index;
                        }
                        index++;
                    }
                    filterList.SelectedIndex = selectedIndex;
                }
            }
            else
            {
                if (TargetPropertyCombo.SelectedItem is string)
                {
                    binding.PropertyName = TargetPropertyCombo.SelectedItem.ToString();
                }
            }
            MapsView.SelectedItems[0].SubItems[1].Text = binding.ToString();
            MapsView.SelectedItems[0].SubItems[2].Text = binding.Parent.AutoRepeat ? "Repeat" : "Once";
            SetDirty();
        }

    

        private void PropertyNameText_TextChanged(object sender, EventArgs e)
        {
            if (ignoreChanges > 0)
            {
                return;
            }
            var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;
            binding.PropertyName = PropertyNameText.Text;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();           
        }

        static List<XboxMap> xboxMaps = new List<XboxMap>();

        static  List<ControlMap> MakeControlGroups()
        {
            var maps = new List<ControlMap>();

            foreach (XboxButtons button in Enum.GetValues(typeof(XboxButtons)))
            {
                var map = new ControlMap();
                map.Name = Enum.GetName(typeof(XboxButtons), button);
                map.ID = -1;

                switch (button)
                {
                    case XboxButtons.DirectionPadUp:
                    case XboxButtons.DirectionPadDown:
                    case XboxButtons.DirectionPadLeft:
                    case XboxButtons.DirectionPadRight:
                    case XboxButtons.Start:
                    case XboxButtons.Back:
                    case XboxButtons.LeftThumbClick:
                    case XboxButtons.RightThumbClick:
                    case XboxButtons.LeftShoulder:
                    case XboxButtons.RightShoulder:
                        map.BindingA.BindingType = BindingType.Toggle;
                        map.BindingA.HadnlerType = HandlerType.KeyPress;
                        break;
                    case XboxButtons.A:
                    case XboxButtons.B:
                    case XboxButtons.X:
                    case XboxButtons.Y:
                        map.BindingA.BindingType = BindingType.Toggle;
                        map.BindingA.HadnlerType = HandlerType.KeyPress;
                        break;

                    case XboxButtons.LeftTrigger:
                    case XboxButtons.RightTrigger:
                    case XboxButtons.LeftThumbX:
                    case XboxButtons.LeftThumbY:
                    case XboxButtons.RightThumbX:
                    case XboxButtons.RightThumbY:
                        map.BindingA.BindingType = BindingType.SetValue;
                        map.BindingA.HadnlerType = HandlerType.ValueChange;
                        break;
                    default:
                        break;
                }
                maps.Add(map);
            }
            return maps;
        }

        int selectedMode;

        void SetupMapList()
        {
            if (Properties.Settings.Default.XboxModeMapping)
            {
                if (ModeCombo.SelectedIndex == -1)
                {
                    ModeCombo.SelectedIndex = 0;
                }

                selectedMode = ModeCombo.SelectedIndex;
            }
            else
            {
                selectedMode = 6;
            }

            MapsView.BeginUpdate();
            MapsView.Items.Clear();
            ignoreChanges++;
            foreach (var cm in xboxMaps[selectedMode].ControlMaps)
            {
                var item = new ListViewItem(cm.Name);
                item.Tag = cm.BindingA;
                cm.BindingA.Parent = cm;
                item.SubItems.Add(cm.BindingA.ToString());
                item.SubItems.Add(cm.AutoRepeat ? "Repeat" : "Once");
                MapsView.Items.Add(item);
                item.Selected = true;
            }
            ignoreChanges--;
            MapsView.EndUpdate();
        }

        private void SetDirty()
        {
            if (selectedMode > -1)
            {
                xboxMaps[selectedMode].Dirty = true;
            }
        }

        int ignoreChanges = 1;

        private void SetupBindingCombos()
        {
            if (Properties.Settings.Default.XboxCustomMapping && MapsView.SelectedItems.Count > 0)
            {
                var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

                MapsView.EndUpdate();

                TargetTypeCombo.Items.Clear();
                TargetTypeCombo.Items.AddRange(UiTools.GetBindingTargetTypeList());
                TargetTypeCombo.SelectedIndex = (int)binding.TargetType;

                BindTypeCombo.Items.Clear();
                BindTypeCombo.Items.AddRange(Enum.GetNames(typeof(BindingType)));
                BindTypeCombo.SelectedIndex = (int)binding.BindingType;

                RepeatCheckbox.Checked = binding.Parent.AutoRepeat;

                UpdatePropertyCombo();
            }
            else
            {
                TargetTypeCombo.Items.Clear();
                BindTypeCombo.Items.Clear();
                TargetPropertyCombo.Items.Clear();
            }
        }

        private void XBoxConfig_FormClosed(object sender, FormClosedEventArgs e)
        {

            SaveDirtyMaps();

            master = null;
        }



        private void UseCustomMaps_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges > 0)
            {
                return;
            }
            Properties.Settings.Default.XboxCustomMapping = UseCustomMaps.Checked;

            SetCustomControls(Properties.Settings.Default.XboxCustomMapping);

        }

        private void ModeDependentMaps_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreChanges > 0)
            {
                return;
            }
            Properties.Settings.Default.XboxModeMapping = ModeDependentMaps.Checked;
            SetCustomControls(Properties.Settings.Default.XboxCustomMapping);
        }

        private void ModeCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (ignoreChanges > 0)
            {
                return;
            }
            SetCustomControls(Properties.Settings.Default.XboxCustomMapping);
        }

        private void SetCustomControls(bool enabled)
        {
            MapsView.Enabled = true;
            BindTypeCombo.Enabled = enabled;
            TargetPropertyCombo.Enabled = enabled;
            TargetTypeCombo.Enabled =enabled ;
            Property.Enabled = enabled;
            BindTypeLabel.Enabled = enabled;
            BindingTargetTypeLabel.Enabled = enabled;
            ModeDependentMaps.Enabled = enabled;
            ModeCombo.Visible = enabled & Properties.Settings.Default.XboxModeMapping;
            modeLabel.Visible = enabled & Properties.Settings.Default.XboxModeMapping;

            if (enabled)
            {
                SetupMapList();
            }
            else
            {
                MapsView.Items.Clear();
            }
        }


        Point pnt1;
        Point pnt2;
        bool mouseDown;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pnt1 = e.Location;
            mouseDown = true;
            pnt2 = e.Location;
            Refresh();

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                pnt2 = e.Location;
                Refresh();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            System.Diagnostics.Debug.Write("g.DrawLine(Pens.White," + pnt1.X.ToString() + "," + pnt1.Y.ToString() + "," + pnt2.X.ToString() + "," + pnt2.Y.ToString() + ");");

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            g.DrawLine(Pens.White, pnt1, pnt2);


            g.DrawLine(Pens.White, 80, 116, 80, 116);
            g.DrawLine(Pens.White, 127, 117, 77, 115);
            g.DrawLine(Pens.White, 81, 52, 120, 61);
            g.DrawLine(Pens.White, 140, 37, 154, 48);
            g.DrawLine(Pens.White, 191, 112, 185, 55);
            g.DrawLine(Pens.White, 241, 54, 258, 117);
            g.DrawLine(Pens.White, 282, 37, 296, 51);
            g.DrawLine(Pens.White, 320, 62, 370, 53);
            g.DrawLine(Pens.White, 319, 116, 368, 109);
            g.DrawLine(Pens.White, 270, 173, 252, 211);
            g.DrawLine(Pens.White, 175, 168, 180, 210);
        }

        private void MapsView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ignoreChanges > 0)
            {
                return;
            }
            SetupBindingCombos();
        }

        private void RepeatCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (MapsView.SelectedIndices.Count > 0)
            {
                var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;
                binding.Parent.AutoRepeat = RepeatCheckbox.Checked;
                MapsView.SelectedItems[0].SubItems[2].Text = binding.Parent.AutoRepeat ? "Repeat" : "Once";
                SetDirty();
            }
            
        }

        static string midiMapPath;
        public static string XboxMapPath
        {
            get
            {
                if (string.IsNullOrEmpty(midiMapPath))
                {
                    midiMapPath = string.Format("{0}\\WWT Xbox Controller Maps\\", System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                }
                return midiMapPath;
            }
        }

        private static void LoadMaps()
        {
            if (!Directory.Exists(XboxMapPath))
            {
                Directory.CreateDirectory(XboxMapPath);
            }

            foreach (var filename in Directory.GetFiles(XboxMapPath, "*.wwtxm"))
            {
                LoadMap(filename, false);
            }
        }

        public static XboxMap LoadMap(string filename, bool update)
        {
            var serializer = new XmlSerializer(typeof(XboxMap));
            var fs = new FileStream(filename, FileMode.Open);

            var map = (XboxMap)serializer.Deserialize(fs);

            
            map.Dirty = true;

            fs.Close();
            map.UpdateMapLinks();

            return map;

        }

        public static void SaveMap(XboxMap map, string filename)
        {
            var serializer = new XmlSerializer(typeof(XboxMap));
            var sw = new StreamWriter(filename);

            serializer.Serialize(sw, map);

            sw.Close();
        }

        private static bool LoadMapPack()
        {

            var filename = XboxMapPath + "XboxMapPack.wwtxmaps";

            if (!File.Exists(filename))
            {
                if (!Directory.Exists(XboxMapPath))
                {
                    Directory.CreateDirectory(XboxMapPath);
                }
                File.WriteAllText(filename, Properties.Resources.XboxDefaults);
            }

            return LoadMapPack(filename);
        }

        public static bool LoadMapPack(string filename)
        {
            if (!File.Exists(filename))
            {
                return false;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(XboxMapPack));
                var fs = new FileStream(filename, FileMode.Open);

                var maps = (XboxMapPack)serializer.Deserialize(fs);

                xboxMaps = maps.Maps;

                fs.Close();

                foreach (var map in maps.Maps)
                {
                    map.UpdateMapLinks();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static void SaveMapPack( string filename)
        {
            var serializer = new XmlSerializer(typeof(XboxMapPack));
            var sw = new StreamWriter(filename);
            var mapPack = new XboxMapPack();

            mapPack.Maps = xboxMaps;


            serializer.Serialize(sw, mapPack);

            sw.Close();
        }

        static DateTime lastSaveTime = DateTime.Now;

        private static void SaveDirtyMaps()
        {
            if (!Directory.Exists(XboxMapPath))
            {
                Directory.CreateDirectory(XboxMapPath);
            }

            var dirty = false;

            foreach (var map in xboxMaps)
            {
                if (map.Dirty)
                {
                    dirty = true;
                    //SaveMap(map, XboxMapPath + map.Name + ".wwtxm");
                    map.Dirty = false;
                }
            }

            if (dirty)
            {
                SaveMapPack(XboxMapPath + "XboxMapPack.wwtxmaps");
            }

            lastSaveTime = DateTime.Now;
        }

        private void Import_Click(object sender, EventArgs e)
        {
            if (selectedMode > -1)
            {
                var ofd = new OpenFileDialog();

                ofd.Filter = "WWT XBox 360 Controller Map (*.wwtxm)|*.wwtxm";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        xboxMaps[selectedMode].ControlMaps = LoadMap(ofd.FileName, true).ControlMaps;
                        xboxMaps[selectedMode].Dirty = true;
                        SetCustomControls(Properties.Settings.Default.XboxCustomMapping);
                    }
                    catch
                    {
                        UiTools.ShowMessageBox(Language.GetLocalizedText(697, "Could not open the file. Ensure it is a valid WorldWide Telescope configuration file."), Language.GetLocalizedText(698, "Open Configuration File"));
                    }
                }
            }
        }

        private void export_Click(object sender, EventArgs e)
        {
            if (selectedMode > -1)
            {
                var name = xboxMaps[selectedMode].Name;

                var sfd = new SaveFileDialog();

                sfd.Filter =  "WWT Xbox 360 Controller Map (*.wwtxm)|*.wwtxm";

                sfd.FileName = name + ".wwtxm";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    SaveMap(xboxMaps[selectedMode], sfd.FileName);
                }
            }
        }

        private void filterList_SelectionChanged(object sender, EventArgs e)
        {
            var prop = TargetPropertyCombo.SelectedItem as ScriptableProperty;
            var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

            if (prop != null)
            {
                if (binding.BindingType == BindingType.SetValue && prop.Type == ScriptablePropertyTypes.ConstellationFilter)
                {
                    binding.Value = filterList.SelectedItem.ToString();
                    SetDirty();
                }
            }

            MapsView.SelectedItems[0].SubItems[1].Text = binding.ToString();
            MapsView.SelectedItems[0].SubItems[2].Text = binding.Parent.AutoRepeat ? "Repeat" : "Once";
        }

        private void filterLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
