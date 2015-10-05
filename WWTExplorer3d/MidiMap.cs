using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using MIDI;
using System.IO;
using System.Drawing;

namespace TerraViewer
{
    [Serializable]
    [XmlRoot("MIDIMap")]
    public class MidiMap
    {
        [XmlIgnore]
        public bool Dirty = false;

        [XmlIgnore]
        public bool Connected;

        [XmlIgnore]
        public MidiInput InputDevice;

        [XmlIgnore]
        public MidiOutput OutputDevice;

        [XmlIgnore]
        public List<ControlMap> AutoRepeatList = new List<ControlMap>();

        [XmlAttribute("Name")]
        public string Name = "";

        [XmlAttribute("DeviceImageUrl")]
        public string DeviceImageUrl = "";

        [XmlArray("ControlMaps")]
        public List<ControlMap> ControlMaps = new List<ControlMap>();

        public void ConnectDevice( int index, int outputIndex)
        {
            InputDevice = new MidiInput();
            if (InputDevice.Open(index))
            {
                InputDevice.MessageReceived += InputDevice_MessageReceived;
                InputDevice.Start();
                Connected = true;
            }
            else
            {
                InputDevice = null;
            }

            OutputDevice = new MidiOutput();
            if (OutputDevice.Open(outputIndex))
            {
                Connected = true;
            }
            else
            {
                OutputDevice = null;
            }

        }

        public void CloseDevice()
        {
            if (InputDevice != null)
            {
                InputDevice.Stop();
                InputDevice.Close();
                InputDevice = null;
            }
            if (OutputDevice != null)
            {
              
                OutputDevice.Close();
                OutputDevice = null;
            }
            Connected = false;
        }

        public void UpdateMapLinks()
        {
            foreach (var map in ControlMaps)
            {
                map.Owner = this;
                map.BindingA.Parent = map;
                map.BindingB.Parent = map;
            }
        }

        public event MidiMessageReceived UnhandledMessageReceived;
        public event MidiMessageReceived MessageReceived;
        void InputDevice_MessageReceived(object sender, MidiMessage message, int channel, int key, int value)
        {
            ControlMap map = null;

            map = FindMap(channel, key);


            if (MessageReceived != null)
            {
                MessageReceived.Invoke(this, message, channel, key, value);
            }

            if (map == null)
            {
                if (UnhandledMessageReceived != null)
                {
                    UnhandledMessageReceived.Invoke(this, message, channel, key, value);
                }
            }
            else
            {
                map.DispatchMessage(message, channel, key, value);
                if (map.AutoRepeat && map.KeyDown && !AutoRepeatList.Contains(map))
                {
                    AutoRepeatList.Add(map);
                }
                else if (map.AutoRepeat && !map.KeyDown && AutoRepeatList.Contains(map))
                {
                    AutoRepeatList.Remove(map);
                }

            }
        }

        public ControlMap FindMap(int channel, int key)
        {
            return ControlMaps.Find(delegate(ControlMap m) { return (key == m.ID && channel == m.Channel); });
        }

        
        public override string ToString()
        {
            return Name + (Connected ? " (Connected)" : " (Not Found)");
        }
    }

    [Serializable]
    [XmlRoot("XboxMap")]
    public class XboxMap
    {
        [XmlIgnore]
        public bool Dirty = false;

        [XmlAttribute("Name")]
        public string Name = "";

        [XmlAttribute("Slot")]
        public int Slot = 0;

        [XmlArray("ControlMaps")]
        public List<ControlMap> ControlMaps = new List<ControlMap>();

        public void UpdateMapLinks()
        {
            foreach (var map in ControlMaps)
            {
                map.Owner = this;
                map.BindingA.Parent = map;
                map.BindingB.Parent = map;
            }
        }
    }

    [Serializable]
    [XmlRoot("XboxMapPack")]
    public class XboxMapPack
    {
        [XmlArray("XboxMaps")]
        public List<XboxMap> Maps = new List<XboxMap>();
    }

    public enum ControlType { KeyPress, KeyUpDown, Slider, Knob, Jog }

    public enum ButtonType { Button, Checkbox, Slider };

    [Serializable]
    public class ControlMap
    {
        [XmlIgnore]
        public object Owner = null;

        [XmlAttribute("Name")]
        public string Name = "";

        [XmlAttribute("Channel")]
        public int Channel = 0;

        [XmlAttribute("ID")]
        public int ID = 0;

        [XmlAttribute("ControlType")]
        public ControlType ControlType = ControlType.KeyPress;
     
        [XmlAttribute("ButtonType")]
        public ButtonType ButtonType = ButtonType.Button;

        [XmlAttribute("AutoRepeat")]
        public bool AutoRepeat = false;

        [XmlAttribute("Mapped")]
        public bool Mapped = false;

        [XmlAttribute("X")]
        public float X = 0;

        [XmlAttribute("Y")]
        public float Y = 0;

        [XmlAttribute("Width")]
        public float Width = .1f;

        [XmlAttribute("Height")]
        public float Height = .1f;    
        
        [XmlIgnore]
        public bool KeyDown = false;

        [XmlElement("BindingA")]
        public ControlBinding BindingA = new ControlBinding();

        [XmlElement("BindingB")]
        public ControlBinding BindingB = new ControlBinding();

        [XmlIgnore]
        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
            }
        }

        internal double GetValue(MidiMessage message, int channel, int key)
        {
            return BindingA.GetValue(message, channel, key);
        }


        internal bool DispatchMessage(MidiMessage message, int channel, int key, int value)
        {
            var currentVal = false;

            if (ControlType == ControlType.KeyPress && (message == MidiMessage.NoteOff || value == 0))
            {
                // On note off we just turn off the mapping
                KeyDown = false;
                return currentVal;
            }

            if (ControlType == ControlType.KeyUpDown && (message == MidiMessage.NoteOff || value == 0))
            {
                currentVal = BindingB.DispatchMessage(message, channel, key, value);
                KeyDown = false;
                return currentVal;
            }

            if (ControlType == ControlType.Jog && (message == MidiMessage.Control && value < 64))
            {
                currentVal = BindingA.DispatchMessage(message, channel, key, value);
                return currentVal;
            }

            if (ControlType == ControlType.Jog && (message == MidiMessage.Control && value > 63))
            {
                currentVal = BindingB.DispatchMessage(message, channel, key, value);
                return currentVal;
            }

            currentVal = BindingA.DispatchMessage(message, channel, key, value);
            KeyDown = true;
            return currentVal;
        }
    }

    public enum BindingTargetType { Setting, Navigation, SpaceTimeController, Goto, Layer, Actions, Key, Mouse };

    public enum BindingType { Action, Toggle, SyncValue, SetValue };
    public enum HandlerType { None, KeyDown, KeyUp, KeyPress, ClockWise, CounterClockwise, ValueChange };

    [Serializable]
    public class ControlBinding
    {
        [XmlIgnore]
        public ControlMap Parent = null;

        [XmlAttribute("HandlerType")]
        public HandlerType HadnlerType = HandlerType.None;

        [XmlAttribute("Target")]
        public string Target = "";

        [XmlAttribute("TargetType")]
        public BindingTargetType TargetType = BindingTargetType.Setting;

        [XmlAttribute("BindingType")]
        public BindingType BindingType = BindingType.Action;

        [XmlAttribute("PropertyName")]
        public String PropertyName;

        [XmlAttribute("Value")]
        public String Value;

        [XmlAttribute("Min")]
        public double Min=0;

        [XmlAttribute("Max")]
        public double Max=1;

        [XmlAttribute("Integer")]
        public bool Integer = true;

        internal bool DispatchMessage(MidiMessage message, int channel, int key, double value)
        {
            var currentVal = false;

            var normalizedValue = value / 127.0;
            var scaledValue = Min + (Max - Min) * normalizedValue;
            var scaledValueInt = (int)scaledValue;

            IScriptable scriptInterface = null;
            switch (TargetType)
            {
                case BindingTargetType.Setting:
                    scriptInterface = Settings.Active as IScriptable;
                    break;
                case BindingTargetType.SpaceTimeController:
                    scriptInterface = SpaceTimeController.ScriptInterface;
                    break;
                case BindingTargetType.Goto:
                    Earth3d.MainWindow.GotoCatalogObject(PropertyName);
                    break;
                case BindingTargetType.Layer:
                    scriptInterface = LayerManager.ScriptInterface;
                    break;
                case BindingTargetType.Navigation:
                    scriptInterface = Earth3d.MainWindow as IScriptable;
                    break;
               //  case BindingTargetType.Actions:
               //     break;
               //case BindingTargetType.Key:
               //     break;
               // case BindingTargetType.Mouse:
               //     break;
                default:
                    break;
            }

            if (scriptInterface != null && !string.IsNullOrEmpty(PropertyName))
            {
                switch (BindingType)
                {
                    case BindingType.Action:
                        scriptInterface.InvokeAction(PropertyName, normalizedValue.ToString());
                        break;
                    case BindingType.Toggle:
                        currentVal = scriptInterface.ToggleProperty(PropertyName);
                        if (channel > -1)
                        {
                            ((MidiMap)Parent.Owner).OutputDevice.SendMessageShort((byte)(144 + channel), (byte)key, (byte)(currentVal ? 3 : 0));
                        }
                        break;
                    case BindingType.SyncValue:
                        scriptInterface.SetProperty(PropertyName, Integer ? scaledValueInt.ToString() : scaledValue.ToString());
                        break;
                    case BindingType.SetValue:
                        scriptInterface.SetProperty(PropertyName, Value);
                        break;
                    default:
                        break;
                }
            }
            return currentVal;
        }

        internal double GetValue(MidiMessage message, int channel, int key)
        {

            IScriptable scriptInterface = null;
            switch (TargetType)
            {
                case BindingTargetType.Setting:
                    scriptInterface = Settings.Active as IScriptable;
                    break;
                case BindingTargetType.SpaceTimeController:
                    scriptInterface = SpaceTimeController.ScriptInterface;
                    break;
                case BindingTargetType.Goto:
                    Earth3d.MainWindow.GotoCatalogObject(PropertyName);
                    break;
                case BindingTargetType.Layer:
                    scriptInterface = LayerManager.ScriptInterface;
                    break;
                case BindingTargetType.Navigation:
                    scriptInterface = Earth3d.MainWindow as IScriptable;
                    break;
                //  case BindingTargetType.Actions:
                //     break;
                //case BindingTargetType.Key:
                //     break;
                // case BindingTargetType.Mouse:
                //     break;
                default:
                    break;
            }

            double value = 0;

            if (scriptInterface != null && !string.IsNullOrEmpty(PropertyName))
            {
                try
                {
                    switch (BindingType)
                    {
                        case BindingType.Action:
                            // no values in actions
                            break;
                        case BindingType.Toggle:
                        case BindingType.SyncValue:
                            var val = scriptInterface.GetProperty(PropertyName);
                            if (!string.IsNullOrEmpty(val))
                            {
                                value = double.Parse(val);
                                value = (value - Min) / (Max - Min);
                                value = value * 127.0;
                                return value;
                            }
                            return 0;
                        case BindingType.SetValue:
                            // don't scale this
                            value = double.Parse(scriptInterface.GetProperty(PropertyName));
                            return value;
                        default:
                            break;
                    }
                }
                catch
                {
                }
            }

            return 0;
        }


        public override string ToString()
        {
            if (BindingType == BindingType.SetValue && !string.IsNullOrEmpty(Value))
            {
                 return TargetType + "." + BindingType + "." + PropertyName + " = " + Value;
            }
            return TargetType + "." + BindingType + "." + PropertyName;
        }
    }

    [Serializable]
    [XmlRoot("ButtonGroup")]
    public class ButtonGroup
    {
        [XmlIgnore]
        public bool Dirty = false;

        [XmlIgnore]
        public string LoadFilename = "";

        [XmlAttribute("Name")]
        public string Name = "";

        [XmlArray("Buttons")]
        public List<ControlMap> Buttons = new List<ControlMap>();

        public void Add(ControlMap map)
        {
            Buttons.Add(map);
            Dirty = true;
        }
        
        internal void Remove(ControlMap map)
        {
            Buttons.Remove(map);
            Dirty = true;
        }

        public static ButtonGroup FromFile(string filename)
        {
            var group = new ButtonGroup();

            if (File.Exists(filename))
            {
                try
                {
                    var serializer = new XmlSerializer(typeof(ButtonGroup));
                    var fs = new FileStream(filename, FileMode.Open);

                    group = (ButtonGroup)serializer.Deserialize(fs);
                   
                    fs.Close();
                }
                catch
                {
                }
            }
            group.LoadFilename = filename;
            return group;
        }

        public void SaveIfDirty()
        {
            if (Dirty)
            {
                Save(LoadFilename);
            }      
        }

        public void Save(string filename)
        {
            var serializer = new XmlSerializer(typeof(ButtonGroup));
            var sw = new StreamWriter(filename);

            serializer.Serialize(sw, this);

            sw.Close();
            Dirty = false;
        }


    }
}
