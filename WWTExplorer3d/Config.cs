
using System;
using System.Text;
using System.Xml;
using System.IO;
using SharpDX;

namespace TerraViewer
{

    public class Config
    {
        public Config()
        {
            
            if (File.Exists(@"c:\wwtconfig\config.xml"))
            {
                saveFilename = @"c:\wwtconfig\config.xml";
                ReadFromXML(saveFilename);
            }
            else if (File.Exists(@"c:\config.xml"))
            {
                saveFilename = @"c:\config.xml";
                ReadFromXML(saveFilename);
            }
            else  if (File.Exists(@"c:\wwtconfig\config.xml"))
            {
                saveFilename = @"c:\wwtconfig\config.xml";
                ReadFromXML(saveFilename);
            }
        }

        public int MonitorCountX=1;
        public int MonitorCountY=1;
        public int MonitorX=0;
        public int MonitorY=0;
        public bool Master = true;
        public int Width = 1920;
        public int Height = 1200;
        public double Bezel = 1.07;

        public string ConfigFile;
        public string BlendFile;
        public string DistortionGrid;
        public bool MultiProjector = false;
        public bool MatrixValid = false;
        private bool MultiChannelDome;
        private bool multiChannelGlobe;

        public bool MultiChannelDome1
        {
            get
            { 
                return MultiChannelDome | (MultiProjector && !MatrixValid);
            }
            set { MultiChannelDome = value; }
        }

        public bool MultiChannelGlobe
        {
            get
            {
                return multiChannelGlobe;
            }

            set
            {
                multiChannelGlobe = value;
            }
        }

        public bool UseDistrotionAndBlend
        {
            get
            {
                return !(String.IsNullOrEmpty(BlendFile) || String.IsNullOrEmpty(DistortionGrid));
            }
        }


        //Frustum
        public float Left;
        public float Right;
        public float Bottom;
        public float Top;
        public float Near;
        public float Far;


        public float Heading;
        public float Pitch;
        public float Roll;
        public float DomeTilt;
        public float DomeAngle;
        public float DiffTilt;
        public float UpFov;
        public float DownFov;
        public float Aspect = 1.39053104f;
        public int NodeID = -1;
        public int ClusterID = 0;
        private string nodeDiplayName = "";

        public float TotalDomeTilt
        {
            get
            {
                return DomeTilt + DiffTilt;
            }
        }

        public string NodeDiplayName
        {
            get
            {
                if (nodeDiplayName == null)
                {
                    return "Node " + NodeID;
                }
                return nodeDiplayName;
            }
            set { nodeDiplayName = value; }
        }



        public void ReadFromXML(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }
            try
            {
                var doc = new XmlDocument();
                doc.Load(path);

                var deviceConfigNode = doc.FirstChild.NextSibling;
                var config = deviceConfigNode.FirstChild;
                var deviceNode = config.FirstChild;

                MonitorCountX = Convert.ToInt32(deviceNode.Attributes["MonitorCountX"].Value);
                MonitorCountY = Convert.ToInt32(deviceNode.Attributes["MonitorCountY"].Value);
                MonitorX = Convert.ToInt32(deviceNode.Attributes["MonitorX"].Value);
                MonitorY = Convert.ToInt32(deviceNode.Attributes["MonitorY"].Value);
                Master = Convert.ToBoolean(deviceNode.Attributes["Master"].Value);

                if (deviceNode.Attributes["Width"] != null)
                {
                    Width = Convert.ToInt32(deviceNode.Attributes["Width"].Value);
                }
                if (deviceNode.Attributes["Height"] != null)
                {
                    Height = Convert.ToInt32(deviceNode.Attributes["Height"].Value);
                }

                if (deviceNode.Attributes["NodeID"] != null)
                {
                    NodeID = Convert.ToInt32(deviceNode.Attributes["NodeID"].Value);
                }

                if (deviceNode.Attributes["ClusterID"] != null)
                {
                    ClusterID = Convert.ToInt32(deviceNode.Attributes["ClusterID"].Value);
                }

                if (deviceNode.Attributes["NodeDiplayName"] != null)
                {
                    NodeDiplayName = deviceNode.Attributes["NodeDiplayName"].Value;
                }

                if (deviceNode.Attributes["Bezel"] != null)
                {
                    Bezel = Convert.ToDouble(deviceNode.Attributes["Bezel"].Value);
                }

                if (deviceNode.Attributes["Heading"] != null)
                {
                    Heading = Convert.ToSingle(deviceNode.Attributes["Heading"].Value);
                }

                if (deviceNode.Attributes["Pitch"] != null)
                {
                    Pitch = Convert.ToSingle(deviceNode.Attributes["Pitch"].Value);
                }

                if (deviceNode.Attributes["Roll"] != null)
                {
                    Roll = Convert.ToSingle(deviceNode.Attributes["Roll"].Value);
                }

                if (deviceNode.Attributes["UpFov"] != null)
                {
                    UpFov = Convert.ToSingle(deviceNode.Attributes["UpFov"].Value);
                }

                if (deviceNode.Attributes["DownFov"] != null)
                {
                    DownFov = Convert.ToSingle(deviceNode.Attributes["DownFov"].Value);
                }

                if (deviceNode.Attributes["DomeTilt"] != null)
                {
                    DomeTilt = Convert.ToSingle(deviceNode.Attributes["DomeTilt"].Value);
                }

                if (deviceNode.Attributes["DomeAngle"] != null)
                {
                    DomeAngle = Convert.ToSingle(deviceNode.Attributes["DomeAngle"].Value);
                }

                if (deviceNode.Attributes["DiffTilt"] != null)
                {
                    DiffTilt = Convert.ToSingle(deviceNode.Attributes["DiffTilt"].Value);
                }

                if (deviceNode.Attributes["Aspect"] != null)
                {
                    Aspect = Convert.ToSingle(deviceNode.Attributes["Aspect"].Value);
                }

                if (deviceNode.Attributes["MultiChannelDome"] != null)
                {
                    MultiChannelDome = Convert.ToBoolean(deviceNode.Attributes["MultiChannelDome"].Value);
                }

                if (deviceNode.Attributes["MultiChannelGlobe"] != null)
                {
                    MultiChannelGlobe = Convert.ToBoolean(deviceNode.Attributes["MultiChannelGlobe"].Value);
                }

                if (deviceNode.Attributes["ConfigFile"] != null)
                {
                    ConfigFile = deviceNode.Attributes["ConfigFile"].Value;
                    ParseConfigFile();
                }

                if (deviceNode.Attributes["BlendFile"] != null)
                {
                    BlendFile = deviceNode.Attributes["BlendFile"].Value;
                }

                if (deviceNode.Attributes["DistortionGrid"] != null)
                {
                    DistortionGrid = deviceNode.Attributes["DistortionGrid"].Value;
                }

                MultiProjector = !(String.IsNullOrEmpty(ConfigFile) || String.IsNullOrEmpty(BlendFile) || String.IsNullOrEmpty(DistortionGrid));
            }
            catch
            {
            }
        }
        
        public Matrix3d ViewMatrix = Matrix3d.Identity;

        private void ParseConfigFile()
        {
            if (String.IsNullOrEmpty(ConfigFile))
            {
                return;
            }

            var configFileData = File.ReadAllLines(ConfigFile);
            for (var i=0; i < configFileData.Length; i++)
            {
                configFileData[i] = configFileData[i].Trim();
            }

            
            foreach (string t in configFileData)
            {
                if (t.StartsWith("Frustum"))
                {
                    var frustParts = t.Split( new[] {' ',';'});
                    if (frustParts.Length == 8)
                    {
                        Left = Convert.ToSingle(frustParts[1]);
                        Right = Convert.ToSingle(frustParts[2]);
                        Bottom = Convert.ToSingle(frustParts[3]);
                        Top = Convert.ToSingle(frustParts[4]);
                        Near = Convert.ToSingle(frustParts[5]);
                        Far = Convert.ToSingle(frustParts[6]);
                    }
                }

                if (t.StartsWith("Rotate"))
                {
                    var frustParts = t.Split(new[] { ' ',';' });
                    if (frustParts.Length == 6)
                    {
                        var angle = (float)(Convert.ToDouble(frustParts[1])/180*Math.PI);
                        var x = Convert.ToSingle(frustParts[2]);
                        var y = Convert.ToSingle(frustParts[3]);
                        var z = Convert.ToSingle(frustParts[4]);
                        var mat = new Matrix3d
                        {
                            Matrix11 = Matrix.RotationAxis(new Vector3(x, y, z), angle)
                        };
                        ViewMatrix = mat * ViewMatrix;
                    }
                }
            }

            MatrixValid = true;

        }

        readonly string saveFilename = @"c:\wwtconfig\config.xml";

        public bool SaveToXml()
        {
            try
            {
                var sb = new StringBuilder();

                sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
                sb.Append("<DeviceConfig>\r\n");
                {
                    sb.Append("<Config>\r\n");
                    sb.Append(String.Format("<Device ClusterID=\"{21}\" NodeID=\"{19}\" NodeDiplayName=\"{20}\" MonitorCountX=\"{0}\" MonitorCountY=\"{1}\" MonitorX=\"{2}\" MonitorY=\"{3}\" Master=\"{4}\" Width=\"{5}\" Height=\"{6}\" Bezel=\"{7}\" ConfigFile=\"{8}\" BlendFile=\"{9}\" DistortionGrid=\"{10}\" Heading=\"{11}\" Pitch=\"{12}\" Roll=\"{13}\" UpFov=\"{14}\" DownFov=\"{15}\" MultiChannelDome=\"{16}\" DomeTilt=\"{17}\" Aspect=\"{18}\" DiffTilt=\"{22}\" MultiChannelGlobe=\"{23}\" DomeAngle=\"{24}\"></Device>\r\n",
                        MonitorCountX, MonitorCountY, MonitorX, MonitorY, Master, Width, Height, Bezel, ConfigFile, BlendFile, DistortionGrid, Heading, Pitch, Roll, UpFov, DownFov, MultiChannelDome, DomeTilt, Aspect, NodeID, NodeDiplayName, ClusterID, DiffTilt, MultiChannelGlobe, DomeAngle));
                    sb.Append("</Config>\r\n");
                }
                sb.Append("</DeviceConfig>\r\n");

                // Create the file.
                using (var fs = File.Create(saveFilename))
                {
                    var info =
                        new UTF8Encoding(true).GetBytes(sb.ToString());

                    fs.Write(info, 0, info.Length);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
