using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using MIDI;

namespace TerraViewer
{
    public partial class MidiSetup : Form
    {
        public MidiSetup()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            Control.Text = Language.GetLocalizedText(1159, "Control");
            Channel.Text = Language.GetLocalizedText(1160, "Chan");
            ID.Text = Language.GetLocalizedText(782, "ID");
            Type.Text = Language.GetLocalizedText(613, "Type");
            Binding.Text = Language.GetLocalizedText(1161, "Binding");
            label1.Text = Language.GetLocalizedText(1162, "MIDI Devices");
            label2.Text = Language.GetLocalizedText(1163, "Control Bindings");
            label4.Text = Language.GetLocalizedText(1164, "Binding Target Type");
            BindTypeLabel.Text = Language.GetLocalizedText(1165, "Bind Type");
            label6.Text = Language.GetLocalizedText(1166, "Property");
            Monitor.Text = Language.GetLocalizedText(1167, "Monitor");
            DeviceProperties.Text = Language.GetLocalizedText(20, "Properties");
            RepeatCheckbox.Text = Language.GetLocalizedText(1168, "Repeat");
            Save.Text = Language.GetLocalizedText(168, "Save");
            LoadMap.Text = Language.GetLocalizedText(730, "Load");
            Text = Language.GetLocalizedText(1169, "Controller Setup");
        }

        public static void UpdateDeviceList()
        {
            if (master != null)
            {

                if (master.InvokeRequired)
                {
                    MethodInvoker updatePlace = delegate
                    {
                        master.UpdateDeviceListLocal();
                    };
                    try
                    {
                        master.Invoke(updatePlace);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    master.UpdateDeviceListLocal();
                }
            }
        }

        public static void ShowSetupWindow()
        {
            if (master == null)
            {
                master = new MidiSetup();
                master.Owner = Earth3d.MainWindow;
                master.Show();
            }
            else
            {
                master.Show();
                master.Activate();
            }
        }


        private void UpdateDeviceListLocal()
        {
            //bool itemFound = false;
            var device = DeviceList.SelectedItem as MidiMap;
            DeviceList.BeginUpdate();
            DeviceList.Items.Clear();
            foreach (var map in MidiMapManager.Maps.Values)
            {
                DeviceList.Items.Add(map);
            }

            DeviceList.SelectedItem = device;

            if (DeviceList.Items.Count > 0 && DeviceList.SelectedIndex == -1)
            {
                DeviceList.SelectedIndex = 0;
            }
            DeviceList.EndUpdate();
        }

        public static MidiSetup master;
        private void MidiSetup_Load(object sender, EventArgs e)
        {
            master = this;
            UpdateDeviceListLocal();
            Height = Properties.Settings.Default.MidiEditWindowHeight;
          

        }

      

        private void SetupBindingCombos()
        {
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                var midiMap = (MidiMap)DeviceList.SelectedItem;

                var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

                TargetTypeCombo.Items.Clear();
                TargetTypeCombo.Items.AddRange(UiTools.GetBindingTargetTypeList());
                TargetTypeCombo.SelectedIndex = (int)binding.TargetType;

                BindTypeCombo.Items.Clear();
                BindTypeCombo.Items.AddRange(Enum.GetNames(typeof(BindingType)));
                BindTypeCombo.SelectedIndex = (int)binding.BindingType;

                RepeatCheckbox.Checked = binding.Parent.AutoRepeat;
                UpdatePropertyCombo();
                DeviceImage.Invalidate();
            }
            else
            {
                TargetTypeCombo.Items.Clear();

                BindTypeCombo.Items.Clear();
                TargetPropertyCombo.ClearText();
            }
        }

        private void MapsView_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetupBindingCombos();
            
        }


        private void DeviceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeviceSelected();
            TurnOffMonitoring();
        }

        private void DeviceSelected()
        {
            LoadControlMaps();
            if (DeviceList.SelectedIndex > -1)
            {
                var midiMap = (MidiMap)DeviceList.SelectedItem;
                SetDeviceImage(midiMap.DeviceImageUrl);
            }
        }

        private void SetDeviceImage(string url)
        {
            var downloadPath = Properties.Settings.Default.CahceDirectory + @"Imagery\Cache\";
            var downloadName = Properties.Settings.Default.CahceDirectory + @"Imagery\Cache\" + Math.Abs(url.GetHashCode32()) + ".png";

            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }

            DataSetManager.DownloadFile(url, downloadName, true, true);
           



            ImageLoaded = false;
            DeviceImage.ImageLocation = downloadName;

        }

        private void LoadControlMaps()
        {
            var map = (MidiMap)DeviceList.SelectedItem;
            MapsView.BeginUpdate();
            MapsView.Items.Clear();
            foreach (var cm in map.ControlMaps)
            {
                var item = new ListViewItem(cm.Name);
                item.Tag = cm.BindingA;
                cm.BindingA.Parent = cm;
                item.SubItems.Add(cm.Channel.ToString());
                item.SubItems.Add(cm.ID.ToString());
                item.SubItems.Add(cm.BindingA.HadnlerType.ToString());
                item.SubItems.Add(cm.BindingA.ToString());
                MapsView.Items.Add(item);

                if (cm.BindingB.HadnlerType != HandlerType.None)
                {
                    item = new ListViewItem(cm.Name);
                    item.Tag = cm.BindingB;
                    cm.BindingB.Parent = cm;
                    item.SubItems.Add(cm.Channel.ToString());
                    item.SubItems.Add(cm.ID.ToString());
                    item.SubItems.Add(cm.BindingB.HadnlerType.ToString());
                    item.SubItems.Add(cm.BindingB.ToString());
                    MapsView.Items.Add(item);
                }
            }
            MapsView.EndUpdate();
        }



        private void Load_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();

            ofd.Filter = Language.GetLocalizedText(1170, "WWT MIDI Controller Map (*.wwtmm)|*.wwtmm");

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var fileName = ofd.FileName;
                try
                {
                    MidiMapManager.LoadMap(fileName, true);
                    
                }
                catch
                {
                    UiTools.ShowMessageBox(Language.GetLocalizedText(697, "Could not open the file. Ensure it is a valid WorldWide Telescope configuration file."), Language.GetLocalizedText(698, "Open Configuration File"));
                }

            }
        }
 

        private void Save_Click(object sender, EventArgs e)
        {
            if (DeviceList.SelectedItem == null)
            {
                return;
            }

            var device = DeviceList.SelectedItem.ToString();
            var map = (MidiMap)DeviceList.SelectedItem;

            var sfd = new SaveFileDialog();

            sfd.Filter = Language.GetLocalizedText(1170, "WWT MIDI Controller Map (*.wwtmm)|*.wwtmm");

            sfd.FileName = device + ".wwtmm";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                MidiMapManager.SaveMap(map, sfd.FileName);
            }
        }

        private void MidiSetup_FormClosed(object sender, FormClosedEventArgs e)
        {
            master = null;
        }

        private void MapsView_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {

        }

        private void MapsView_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            var cm = ((ControlBinding)MapsView.SelectedItems[0].Tag).Parent;

            if (!string.IsNullOrEmpty(e.Label))
            {
                cm.Name = e.Label;
                ((MidiMap)DeviceList.SelectedItem).Dirty = true;
                DeviceImage.Invalidate();
            }
            else
            {
                e.CancelEdit = true;
            }
        }

        private void NewBinding_Click(object sender, EventArgs e)
        {
            if (DeviceList.SelectedItem != null)
            {
                var nb = new NewBinding();

                nb.MidiMap = (MidiMap)DeviceList.SelectedItem;
                nb.ControlMap = new ControlMap();
                if (nb.ShowDialog() == DialogResult.OK)
                {
                    nb.MidiMap.ControlMaps.Add(nb.ControlMap);
                    nb.ControlMap.Owner = nb.MidiMap;
                    LoadControlMaps();
                    SelectControlMap(nb.ControlMap);
                    ((MidiMap)DeviceList.SelectedItem).Dirty = true;
                    MapsView.Items[MapsView.Items.Count - 1].Selected = true;
                    MapsView.EnsureVisible(MapsView.Items.Count - 1);
                }
            }
        }

        private void SelectControlMap(ControlMap target)
        {
            foreach (ListViewItem item in MapsView.Items)
            {
                if (item.Tag == target)
                {
                    item.Selected = true;
                    return;
                }
            }
        }

       
        private void UpdateSelectedControlMap()
        {
            var midiMap = (MidiMap)DeviceList.SelectedItem;
            midiMap.Dirty = true;
        }

        private void TargetTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            UpdatePropertyCombo();


        }

        private void UpdatePropertyCombo()
        {
            var tt = (BindingTargetType)TargetTypeCombo.SelectedIndex;

            TargetPropertyCombo.Items.Clear();
            TargetPropertyCombo.ClearText();
            filterLabel.Visible = false;
            filterList.Visible = false;
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                var midiMap = (MidiMap)DeviceList.SelectedItem;

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
                    BindTypeCombo.Visible = true;
                    BindTypeLabel.Visible = true;
                                     

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
                    BindTypeCombo.Visible = false;
                    BindTypeLabel.Visible = false;
     
                }
                MapsView.SelectedItems[0].SubItems[4].Text = binding.ToString();
                UpdateSelectedControlMap();
            }
        }

        

        private void PropertyNameText_TextChanged(object sender, EventArgs e)
        {
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                var midiMap = (MidiMap)DeviceList.SelectedItem;

                var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;


                binding.PropertyName = PropertyNameText.Text;

            }
            
            UpdateSelectedControlMap();
        }

        private void TargetPropertyCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                var midiMap = (MidiMap)DeviceList.SelectedItem;

                var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

                var prop = TargetPropertyCombo.SelectedItem as ScriptableProperty;

                filterLabel.Visible = false;
                filterList.Visible = false;

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
                MapsView.SelectedItems[0].SubItems[4].Text = binding.ToString();
            }
        }

        private void BindTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                var midiMap = (MidiMap)DeviceList.SelectedItem;

                var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

                binding.BindingType = (BindingType)BindTypeCombo.SelectedIndex;
                UpdatePropertyCombo();

                MapsView.SelectedItems[0].SubItems[4].Text = binding.ToString();
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (UiTools.ShowMessageBox(Language.GetLocalizedText(1171, "This will remove this control map. Are you sure you want to do this?"), Language.GetLocalizedText(1172, "Remove Control Map"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                var midiMap = (MidiMap)DeviceList.SelectedItem;

                var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

                midiMap.ControlMaps.Remove(binding.Parent);
                LoadControlMaps();
                
                midiMap.Dirty = true;
            }
        }

        private void RepeatCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                var midiMap = (MidiMap)DeviceList.SelectedItem;

                var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;
                binding.Parent.AutoRepeat = RepeatCheckbox.Checked;
            }
        }

        private void DeviceProperties_Click(object sender, EventArgs e)
        {
            if (DeviceList.SelectedIndex > -1 )
            {
                var midiMap = (MidiMap)DeviceList.SelectedItem;
                var props = new ControllerProperties();
                props.DeviceMap = midiMap;
                if (props.ShowDialog() == DialogResult.OK)
                {
                    DeviceSelected();
                }
                
            }
        }

        private void DeviceImage_Resize(object sender, EventArgs e)
        {
            UpdateImageTranform();
        }

        // These are used for mapping display of button & sliders on the device images.
        bool ImageLoaded;

        float WidthFactor = 1;
        float HeightFactor = 1;

        int OffsetX;
        int OffsetY;

        private PointF ControlToImage(Point pntIN)
        {
            var pntOut = new PointF((pntIN.X - OffsetX) * WidthFactor, (pntIN.Y - OffsetY) * HeightFactor);
            return pntOut;
        }

        private Point ImageToControl(PointF pntIn)
        {
            var pntOut = new Point((int)(pntIn.X / WidthFactor + OffsetX), (int)(pntIn.Y / HeightFactor + OffsetY));
            return pntOut;
        }

        private void DeviceImage_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                DeviceImage.AllowDrop = true;
                ImageLoaded = true;

                UpdateImageTranform();



            }
            else
            {
                DeviceImage.AllowDrop = false;
                ImageLoaded = false;
            }
        }

        private void UpdateImageTranform()
        {
            if (ImageLoaded)
            {
                float aspectImage = DeviceImage.Image.Width / DeviceImage.Image.Height;
                float aspectControl = DeviceImage.Width / DeviceImage.Height;
                if (aspectImage > aspectControl)
                {
                    WidthFactor = DeviceImage.Width / (float)DeviceImage.Image.Width;
                    HeightFactor = WidthFactor;
                    var scaledHeight = DeviceImage.Image.Height * HeightFactor;
                    OffsetX = 0;
                    OffsetY = (int)(Math.Abs(DeviceImage.Height - scaledHeight) / 2);
                    // Factor down to unit of 1
                    WidthFactor = 1 / (WidthFactor * DeviceImage.Image.Width);
                    HeightFactor = WidthFactor;
                }
                else
                {
                    HeightFactor = DeviceImage.Height / (float)DeviceImage.Image.Height;
                    WidthFactor = HeightFactor;
                    var scaledWidth = DeviceImage.Image.Width * WidthFactor;
                    OffsetY = 0;
                    OffsetX = (int)(Math.Abs(DeviceImage.Width - scaledWidth) / 2);
                    // Factor down to units of 1
                    HeightFactor = 1 / (HeightFactor * DeviceImage.Image.Height);
                    WidthFactor = HeightFactor;
                }
            }
        }

        private void DeviceImage_Click(object sender, EventArgs e)
        {

        }

        private void DeviceImage_DragDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData("Text").ToString();
            if (data == MidiControlDataFormat && DragSource != null)
            {
                try
                {
                    var midiMap = (MidiMap)DeviceList.SelectedItem;
                    DragSource.Mapped = true;
                    var pnt = ControlToImage(DeviceImage.PointToClient(new Point(e.X, e.Y)));
                    DragSource.X = pnt.X;
                    DragSource.Y = pnt.Y;
                    DragSource.Width = .1f;
                    DragSource.Height = .1f;
                    DeviceImage.Invalidate();
                    midiMap.Dirty = true;
                    
                }
                catch
                {
                }
            }

        }

        const string MidiControlDataFormat = "WorldWideTelescope.MidiControl";

        private void DeviceImage_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Text"))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
 
        }

        private void DeviceImage_DragLeave(object sender, EventArgs e)
        {

        }

        private void DeviceImage_DragOver(object sender, DragEventArgs e)
        {

        }

        private ControlMap DragSource;

        private void MapsView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                var midiMap = (MidiMap)DeviceList.SelectedItem;

                var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;
                DragSource = binding.Parent;
                DoDragDrop(MidiControlDataFormat, DragDropEffects.Copy);
            }
        }

        private void MidiSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.MidiEditWindowHeight = Height;
            TurnOffMonitoring();
        }

        private void DeviceImage_Paint(object sender, PaintEventArgs e)
        {
            ControlMap selectedMap = null;
            if (DeviceList.SelectedIndex > -1 && ImageLoaded)
            {
                if (MapsView.SelectedIndices.Count > 0)
                {
                    var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;
                    selectedMap = binding.Parent;
                }
                var BackgroundBrush = new SolidBrush(Color.FromArgb(128, 10, 10, 45));
                var midiMap = (MidiMap)DeviceList.SelectedItem;

                foreach (var cm in midiMap.ControlMaps)
                {
                    if (cm.Mapped)
                    {
                        var textBrush = UiTools.StadardTextBrush;
                        if (cm == selectedMap)
                        {
                            textBrush = UiTools.YellowTextBrush;
                        }
                        var sizeText = e.Graphics.MeasureString(cm.Name, UiTools.StandardLarge);
                        var pnt = ImageToControl(new PointF(cm.X, cm.Y));
                        pnt.Offset(new Point((int)(-sizeText.Width / 2), (int)(-sizeText.Height / 2)));
                        var pnt2 = ImageToControl(new PointF(cm.X + cm.Width, cm.Y + cm.Height));
                        var size = new Size((int)sizeText.Width, (int)sizeText.Height);
                        e.Graphics.FillRectangle(BackgroundBrush, new Rectangle(pnt, size));
                        e.Graphics.DrawString(cm.Name, UiTools.StandardLarge, textBrush, pnt);
                       
                    }
                }
                BackgroundBrush.Dispose();
            }
        }

        MidiMap monitoringMap;
        private void Monitor_CheckedChanged(object sender, EventArgs e)
        {
            if (Monitor.Checked)
            {
                if (DeviceList.SelectedIndex > -1)
                {
                    monitoringMap = (MidiMap)DeviceList.SelectedItem;
                    if (monitoringMap.Connected)
                    {
                        monitoringMap.MessageReceived += monitoringMap_MessageReceived;
                        return;
                    }
                    monitoringMap = null;
                }
            }
            TurnOffMonitoring();
        }

        void TurnOffMonitoring()
        {
            if (monitoringMap != null)
            {
                monitoringMap.MessageReceived -= monitoringMap_MessageReceived;
                monitoringMap = null;
                Monitor.Checked = false;
            }
        }

        void monitoringMap_MessageReceived(object sender, MidiMessage message, int channel, int key, int value)
        {
            if (monitoringMap != null)
            {
                var map = monitoringMap.FindMap(channel, key);

                if (map != null)
                {
                    MethodInvoker doIt = delegate
                    {
                        foreach (ListViewItem item in MapsView.Items)
                        {
                            var cb = item.Tag as ControlBinding;
                            if (cb != null)
                            {
                                if (cb.Parent == map)
                                {
                                    item.Selected = true;
                                    MapsView.EnsureVisible(item.Index);
                                    break;
                                }
                            }
                        }
                    };

                    if (InvokeRequired)
                    {
                        try
                        {
                            Invoke(doIt);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        doIt();
                    } 
                }
            }
        }

        private void filterList_SelectionChanged(object sender, EventArgs e)
        {
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                var midiMap = (MidiMap)DeviceList.SelectedItem;

                var binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

                var prop = TargetPropertyCombo.SelectedItem as ScriptableProperty;

                if (prop != null)
                {
                    if (binding.BindingType == BindingType.SetValue && prop.Type == ScriptablePropertyTypes.ConstellationFilter)
                    {
                        binding.Value = filterList.SelectedItem.ToString();
                    }
                    
                }
                MapsView.SelectedItems[0].SubItems[4].Text = binding.ToString();
            }
        }
    }
}
