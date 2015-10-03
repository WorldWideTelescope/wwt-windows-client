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

            foreach(var filename in Directory.GetFiles(MidiMapPath,"*.wwtmm"))
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

            foreach (var map in Maps.Values)
            {
                if (map.Dirty)
                {
                    SaveMap(map, midiMapPath + map.Name + ".wwtmm");
                    map.Dirty = false;
                }
            }
            lastSaveTime = DateTime.Now;

        }
        
        static int MidiInputCount;
        static int ConnectionCheckCount;
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
            foreach (var map in Maps.Values)
            {
                if (map.AutoRepeatList.Count > 0)
                {
                    foreach (var cm in map.AutoRepeatList)
                    {
                        cm.DispatchMessage(MidiMessage.NoteOn, cm.Channel, cm.ID, 127);
                    }
                }
            }
        }

        private static void ConnectionCheck()
        {
            var count = MidiInput.Count;

            if (count != MidiInputCount)
            {
                UpdateDevices();
            }
            MidiInputCount = count;
        }

        private static void UpdateDevices()
        {
            var count = MidiInput.Count;
            CloseAllConnections();
            
            var index = 0;
            foreach (var dev in MidiInput.GetDeviceList())
            {
                if (Maps.ContainsKey(dev))
                {
                    var map = Maps[dev];
                    var outIndex = MidiOutput.GetDeviceIdByName(dev);
                    map.ConnectDevice(index, outIndex);
                }
                else
                {
                    // Can't find map so create a new default map
                    var map = new MidiMap();
                    map.Name = dev;
                    map.Dirty = true;
                    var outIndex = MidiOutput.GetDeviceIdByName(dev);
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
            foreach (var map in Maps.Values)
            {
                map.CloseDevice();
            }
        }

        
        public static void LoadMap(string filename, bool update)
        {
            var serializer = new XmlSerializer(typeof(MidiMap));
            var fs = new FileStream(filename, FileMode.Open);

            var map = (MidiMap)serializer.Deserialize(fs);

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
            var serializer = new XmlSerializer(typeof(MidiMap));
            var sw = new StreamWriter(filename);

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
