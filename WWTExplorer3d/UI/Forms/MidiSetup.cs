using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;

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
            this.Control.Text = Language.GetLocalizedText(1159, "Control");
            this.Channel.Text = Language.GetLocalizedText(1160, "Chan");
            this.ID.Text = Language.GetLocalizedText(782, "ID");
            this.Type.Text = Language.GetLocalizedText(613, "Type");
            this.Binding.Text = Language.GetLocalizedText(1161, "Binding");
            this.label1.Text = Language.GetLocalizedText(1162, "MIDI Devices");
            this.label2.Text = Language.GetLocalizedText(1163, "Control Bindings");
            this.label4.Text = Language.GetLocalizedText(1164, "Binding Target Type");
            this.BindTypeLabel.Text = Language.GetLocalizedText(1165, "Bind Type");
            this.label6.Text = Language.GetLocalizedText(1166, "Property");
            this.Monitor.Text = Language.GetLocalizedText(1167, "Monitor");
            this.DeviceProperties.Text = Language.GetLocalizedText(20, "Properties");
            this.RepeatCheckbox.Text = Language.GetLocalizedText(1168, "Repeat");
            this.Save.Text = Language.GetLocalizedText(168, "Save");
            this.LoadMap.Text = Language.GetLocalizedText(730, "Load");
            this.Text = Language.GetLocalizedText(1169, "Controller Setup");
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
            MidiMap device = DeviceList.SelectedItem as MidiMap;
            DeviceList.BeginUpdate();
            DeviceList.Items.Clear();
            foreach (MidiMap map in MidiMapManager.Maps.Values)
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
            this.Height = Properties.Settings.Default.MidiEditWindowHeight;
          

        }

      

        private void SetupBindingCombos()
        {
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;

                ControlBinding binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

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
                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;
                SetDeviceImage(midiMap.DeviceImageUrl);
            }
        }

        private void SetDeviceImage(string url)
        {
            string downloadPath = Properties.Settings.Default.CahceDirectory + @"Imagery\Cache\";
            string downloadName = Properties.Settings.Default.CahceDirectory + @"Imagery\Cache\" + Math.Abs(url.GetHashCode32()).ToString() + ".png";

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
            MidiMap map = (MidiMap)DeviceList.SelectedItem;
            MapsView.BeginUpdate();
            MapsView.Items.Clear();
            foreach (ControlMap cm in map.ControlMaps)
            {
                ListViewItem item = new ListViewItem(cm.Name);
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
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = Language.GetLocalizedText(1170, "WWT MIDI Controller Map (*.wwtmm)|*.wwtmm");

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string fileName = ofd.FileName;
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

            string device = DeviceList.SelectedItem.ToString();
            MidiMap map = (MidiMap)DeviceList.SelectedItem;

            SaveFileDialog sfd = new SaveFileDialog();

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
            ControlMap cm = ((ControlBinding)MapsView.SelectedItems[0].Tag).Parent;

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
                NewBinding nb = new NewBinding();

                nb.MidiMap = (MidiMap)DeviceList.SelectedItem;
                nb.ControlMap = new ControlMap();
                if (nb.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
            MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;
            midiMap.Dirty = true;
        }

        private void TargetTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            UpdatePropertyCombo();


        }

        private void UpdatePropertyCombo()
        {
            BindingTargetType tt = (BindingTargetType)TargetTypeCombo.SelectedIndex;

            TargetPropertyCombo.Items.Clear();
            TargetPropertyCombo.ClearText();
            filterLabel.Visible = false;
            filterList.Visible = false;
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;

                ControlBinding binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

                binding.TargetType = tt;

                IScriptable scriptInterface = null;
                bool comboVisible = true;
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
                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;

                ControlBinding binding = (ControlBinding)MapsView.SelectedItems[0].Tag;


                binding.PropertyName = PropertyNameText.Text;

            }
            
            UpdateSelectedControlMap();
        }

        private void TargetPropertyCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;

                ControlBinding binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

                ScriptableProperty prop = TargetPropertyCombo.SelectedItem as ScriptableProperty;

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
                        int index = 0;
                        int selectedIndex = 0;
                        foreach (string name in ConstellationFilter.Families.Keys)
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
                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;

                ControlBinding binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

                binding.BindingType = (BindingType)BindTypeCombo.SelectedIndex;
                UpdatePropertyCombo();

                MapsView.SelectedItems[0].SubItems[4].Text = binding.ToString();
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            if (UiTools.ShowMessageBox(Language.GetLocalizedText(1171, "This will remove this control map. Are you sure you want to do this?"), Language.GetLocalizedText(1172, "Remove Control Map"), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {

                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;

                ControlBinding binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

                midiMap.ControlMaps.Remove(binding.Parent);
                LoadControlMaps();
                
                midiMap.Dirty = true;
            }
        }

        private void RepeatCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;

                ControlBinding binding = (ControlBinding)MapsView.SelectedItems[0].Tag;
                binding.Parent.AutoRepeat = RepeatCheckbox.Checked;
            }
        }

        private void DeviceProperties_Click(object sender, EventArgs e)
        {
            if (DeviceList.SelectedIndex > -1 )
            {
                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;
                ControllerProperties props = new ControllerProperties();
                props.DeviceMap = midiMap;
                if (props.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
        bool ImageLoaded = false;

        float WidthFactor = 1;
        float HeightFactor = 1;

        int OffsetX = 0;
        int OffsetY = 0;

        private PointF ControlToImage(Point pntIN)
        {
            PointF pntOut = new PointF((pntIN.X - OffsetX) * WidthFactor, (pntIN.Y - OffsetY) * HeightFactor);
            return pntOut;
        }

        private Point ImageToControl(PointF pntIn)
        {
            Point pntOut = new Point((int)(pntIn.X / WidthFactor + OffsetX), (int)(pntIn.Y / HeightFactor + OffsetY));
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
                    float scaledHeight = DeviceImage.Image.Height * HeightFactor;
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
                    float scaledWidth = DeviceImage.Image.Width * WidthFactor;
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
            string data = e.Data.GetData("Text").ToString();
            if (data == MidiControlDataFormat && DragSource != null)
            {
                try
                {
                    MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;
                    DragSource.Mapped = true;
                    PointF pnt = ControlToImage(DeviceImage.PointToClient(new Point(e.X, e.Y)));
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

        private ControlMap DragSource = null;

        private void MapsView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (DeviceList.SelectedIndex > -1 && MapsView.SelectedIndices.Count > 0)
            {
                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;

                ControlBinding binding = (ControlBinding)MapsView.SelectedItems[0].Tag;
                DragSource = binding.Parent;
                DoDragDrop(MidiControlDataFormat, DragDropEffects.Copy);
            }
        }

        private void MidiSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.MidiEditWindowHeight = this.Height;
            TurnOffMonitoring();
        }

        private void DeviceImage_Paint(object sender, PaintEventArgs e)
        {
            ControlMap selectedMap = null;
            if (DeviceList.SelectedIndex > -1 && ImageLoaded)
            {
                if (MapsView.SelectedIndices.Count > 0)
                {
                    ControlBinding binding = (ControlBinding)MapsView.SelectedItems[0].Tag;
                    selectedMap = binding.Parent;
                }
                SolidBrush BackgroundBrush = new SolidBrush(Color.FromArgb(128, 10, 10, 45));
                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;

                foreach (ControlMap cm in midiMap.ControlMaps)
                {
                    if (cm.Mapped)
                    {
                        Brush textBrush = UiTools.StadardTextBrush;
                        if (cm == selectedMap)
                        {
                            textBrush = UiTools.YellowTextBrush;
                        }
                        SizeF sizeText = e.Graphics.MeasureString(cm.Name, UiTools.StandardLarge);
                        Point pnt = ImageToControl(new PointF(cm.X, cm.Y));
                        pnt.Offset(new Point((int)(-sizeText.Width / 2), (int)(-sizeText.Height / 2)));
                        Point pnt2 = ImageToControl(new PointF(cm.X + cm.Width, cm.Y + cm.Height));
                        Size size = new Size((int)sizeText.Width, (int)sizeText.Height);
                        e.Graphics.FillRectangle(BackgroundBrush, new Rectangle(pnt, size));
                        e.Graphics.DrawString(cm.Name, UiTools.StandardLarge, textBrush, pnt);
                       
                    }
                }
                BackgroundBrush.Dispose();
            }
        }

        MidiMap monitoringMap = null;
        private void Monitor_CheckedChanged(object sender, EventArgs e)
        {
            if (Monitor.Checked)
            {
                if (DeviceList.SelectedIndex > -1)
                {
                    monitoringMap = (MidiMap)DeviceList.SelectedItem;
                    if (monitoringMap.Connected)
                    {
                        monitoringMap.MessageReceived += new MIDI.MidiMessageReceived(monitoringMap_MessageReceived);
                        return;
                    }
                    else
                    {
                        monitoringMap = null;
                    }
                }
            }
            TurnOffMonitoring();
        }

        void TurnOffMonitoring()
        {
            if (monitoringMap != null)
            {
                monitoringMap.MessageReceived -= new MIDI.MidiMessageReceived(monitoringMap_MessageReceived);
                monitoringMap = null;
                Monitor.Checked = false;
            }
        }

        void monitoringMap_MessageReceived(object sender, MIDI.MidiMessage message, int channel, int key, int value)
        {
            if (monitoringMap != null)
            {
                ControlMap map = monitoringMap.FindMap(channel, key);

                if (map != null)
                {
                    MethodInvoker doIt = delegate
                    {
                        foreach (ListViewItem item in MapsView.Items)
                        {
                            ControlBinding cb = item.Tag as ControlBinding;
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

                    if (this.InvokeRequired)
                    {
                        try
                        {
                            this.Invoke(doIt);
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
                MidiMap midiMap = (MidiMap)DeviceList.SelectedItem;

                ControlBinding binding = (ControlBinding)MapsView.SelectedItems[0].Tag;

                ScriptableProperty prop = TargetPropertyCombo.SelectedItem as ScriptableProperty;

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
