using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using MIDI;

namespace TerraViewer
{
    class MidiMapManager
    {
        public static Dictionary<string, MidiMap> Maps = new Dictionary<string, MidiMap>();


        public static void Startup()
        {
            LoadMaps();
        }

        public static void Shutdown()
        {
            CloseAllConnections();
            SaveDirtyMaps();
        }

        static DateTime lastSaveTime = DateTime.Now;


        private static void LoadMaps()
        {
            if (!Directory.Exists(MidiMapPath))
            {
                Directory.CreateDirectory(MidiMapPath);
            }

            foreach(string filename in Directory.GetFiles(MidiMapPath,"*.wwtmm"))
            {
                LoadMap(filename, false);
            }
            UpdateDevices();
        }

        private static void SaveDirtyMaps()
        {
            if (!Directory.Exists(MidiMapPath))
            {
                Directory.CreateDirectory(MidiMapPath);
            }

            foreach (MidiMap map in Maps.Values)
            {
                if (map.Dirty)
                {
                    SaveMap(map, midiMapPath + map.Name + ".wwtmm");
                    map.Dirty = false;
                }
            }
            lastSaveTime = DateTime.Now;

        }
        
        static int MidiInputCount = 0;
        static int ConnectionCheckCount = 0;
        public static void Heartbeat()
        {
         
            ConnectionCheckCount++;

            if (ConnectionCheckCount > 10)
            {
                ConnectionCheckCount = 0;
                ConnectionCheck();
                if ((DateTime.Now - lastSaveTime).TotalSeconds > 60)
                {
                    SaveDirtyMaps();
                }
            }

            ProcessAutoRepeats();
        }

        private static void ProcessAutoRepeats()
        {
            foreach (MidiMap map in Maps.Values)
            {
                if (map.AutoRepeatList.Count > 0)
                {
                    foreach (ControlMap cm in map.AutoRepeatList)
                    {
                        cm.DispatchMessage(MidiMessage.NoteOn, cm.Channel, cm.ID, 127);
                    }
                }
            }
        }

        private static void ConnectionCheck()
        {
            int count = MidiInput.Count;

            if (count != MidiInputCount)
            {
                UpdateDevices();
            }
            MidiInputCount = count;
        }

        private static void UpdateDevices()
        {
            int count = MidiInput.Count;
            CloseAllConnections();
            
            int index = 0;
            foreach (string dev in MidiInput.GetDeviceList())
            {
                if (Maps.ContainsKey(dev))
                {
                    MidiMap map = Maps[dev];
                    int outIndex = MidiOutput.GetDeviceIdByName(dev);
                    map.ConnectDevice(index, outIndex);
                }
                else
                {
                    // Can't find map so create a new default map
                    MidiMap map = new MidiMap();
                    map.Name = dev;
                    map.Dirty = true;
                    int outIndex = MidiOutput.GetDeviceIdByName(dev);
                    map.ConnectDevice(index, outIndex);
                    map.UpdateMapLinks();
                    Maps[map.Name] = map;
                }
                index++;
            }
            MidiSetup.UpdateDeviceList();
            
            MidiInputCount = count;
        }

        private static void CloseAllConnections()
        {
            // Close Existing Connections
            foreach (MidiMap map in Maps.Values)
            {
                map.CloseDevice();
            }
        }

        
        public static void LoadMap(string filename, bool update)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MidiMap));
            FileStream fs = new FileStream(filename, FileMode.Open);

            MidiMap map = (MidiMap)serializer.Deserialize(fs);

            Maps[map.Name] = map;
            map.Dirty = true;

            fs.Close();
            map.UpdateMapLinks();

            if (update)
            {
                UpdateDevices();
            }
        }

        public static void SaveMap(MidiMap map, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MidiMap));
            StreamWriter sw = new StreamWriter(filename);

            serializer.Serialize(sw, map);

            sw.Close();
        }

        static string  midiMapPath;
        public static string MidiMapPath
        {
            get
            {
                if (string.IsNullOrEmpty(midiMapPath))
                {
                    midiMapPath = string.Format("{0}\\WWT MIDI Controller Maps\\", System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                }
                return midiMapPath;
            }
        }
    }
}
