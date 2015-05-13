
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
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
        private bool MultiChannelDome = false;
        private bool multiChannelGlobe = false;

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
            XmlDocument doc;

            if (!File.Exists(path))
            {
                return;
            }
            try
            {
                doc = new XmlDocument();
                doc.Load(path);

                XmlNode deviceConfigNode = doc.FirstChild.NextSibling;
                XmlNode config = deviceConfigNode.FirstChild;
                XmlNode deviceNode = config.FirstChild;

                MonitorCountX = Convert.ToInt32(deviceNode.Attributes["MonitorCountX"].Value.ToString());
                MonitorCountY = Convert.ToInt32(deviceNode.Attributes["MonitorCountY"].Value.ToString());
                MonitorX = Convert.ToInt32(deviceNode.Attributes["MonitorX"].Value.ToString());
                MonitorY = Convert.ToInt32(deviceNode.Attributes["MonitorY"].Value.ToString());
                Master = Convert.ToBoolean(deviceNode.Attributes["Master"].Value.ToString());

                if (deviceNode.Attributes["Width"] != null)
                {
                    Width = Convert.ToInt32(deviceNode.Attributes["Width"].Value.ToString());
                }
                if (deviceNode.Attributes["Height"] != null)
                {
                    Height = Convert.ToInt32(deviceNode.Attributes["Height"].Value.ToString());
                }

                if (deviceNode.Attributes["NodeID"] != null)
                {
                    NodeID = Convert.ToInt32(deviceNode.Attributes["NodeID"].Value.ToString());
                }

                if (deviceNode.Attributes["ClusterID"] != null)
                {
                    ClusterID = Convert.ToInt32(deviceNode.Attributes["ClusterID"].Value.ToString());
                }

                if (deviceNode.Attributes["NodeDiplayName"] != null)
                {
                    NodeDiplayName = deviceNode.Attributes["NodeDiplayName"].Value;
                }

                if (deviceNode.Attributes["Bezel"] != null)
                {
                    Bezel = Convert.ToDouble(deviceNode.Attributes["Bezel"].Value.ToString());
                }

                if (deviceNode.Attributes["Heading"] != null)
                {
                    Heading = Convert.ToSingle(deviceNode.Attributes["Heading"].Value.ToString());
                }

                if (deviceNode.Attributes["Pitch"] != null)
                {
                    Pitch = Convert.ToSingle(deviceNode.Attributes["Pitch"].Value.ToString());
                }

                if (deviceNode.Attributes["Roll"] != null)
                {
                    Roll = Convert.ToSingle(deviceNode.Attributes["Roll"].Value.ToString());
                }

                if (deviceNode.Attributes["UpFov"] != null)
                {
                    UpFov = Convert.ToSingle(deviceNode.Attributes["UpFov"].Value.ToString());
                }

                if (deviceNode.Attributes["DownFov"] != null)
                {
                    DownFov = Convert.ToSingle(deviceNode.Attributes["DownFov"].Value.ToString());
                }

                if (deviceNode.Attributes["DomeTilt"] != null)
                {
                    DomeTilt = Convert.ToSingle(deviceNode.Attributes["DomeTilt"].Value.ToString());
                }

                if (deviceNode.Attributes["DomeAngle"] != null)
                {
                    DomeAngle = Convert.ToSingle(deviceNode.Attributes["DomeAngle"].Value.ToString());
                }

                if (deviceNode.Attributes["DiffTilt"] != null)
                {
                    DiffTilt = Convert.ToSingle(deviceNode.Attributes["DiffTilt"].Value.ToString());
                }

                if (deviceNode.Attributes["Aspect"] != null)
                {
                    Aspect = Convert.ToSingle(deviceNode.Attributes["Aspect"].Value.ToString());
                }

                if (deviceNode.Attributes["MultiChannelDome"] != null)
                {
                    MultiChannelDome = Convert.ToBoolean(deviceNode.Attributes["MultiChannelDome"].Value.ToString());
                }

                if (deviceNode.Attributes["MultiChannelGlobe"] != null)
                {
                    MultiChannelGlobe = Convert.ToBoolean(deviceNode.Attributes["MultiChannelGlobe"].Value.ToString());
                }

                if (deviceNode.Attributes["ConfigFile"] != null)
                {
                    ConfigFile = deviceNode.Attributes["ConfigFile"].Value.ToString();
                    ParseConfigFile();
                }

                if (deviceNode.Attributes["BlendFile"] != null)
                {
                    BlendFile = deviceNode.Attributes["BlendFile"].Value.ToString();
                }

                if (deviceNode.Attributes["DistortionGrid"] != null)
                {
                    DistortionGrid = deviceNode.Attributes["DistortionGrid"].Value.ToString();
                }

                MultiProjector = !(String.IsNullOrEmpty(ConfigFile) || String.IsNullOrEmpty(BlendFile) || String.IsNullOrEmpty(DistortionGrid));
               // UseDistrotionAndBlend = !( String.IsNullOrEmpty(BlendFile) || String.IsNullOrEmpty(DistortionGrid));
               // MultiProjector = true;
            }
            catch
            {
            }


            return;
        }
        
        public Matrix3d ViewMatrix = Matrix3d.Identity;

        private void ParseConfigFile()
        {
            if (String.IsNullOrEmpty(ConfigFile))
            {
                return;
            }

            string[] configFileData = File.ReadAllLines(ConfigFile);
            for (int i=0; i < configFileData.Length; i++)
            {
                configFileData[i] = configFileData[i].Trim();
            }

            
            for (int i=0; i < configFileData.Length; i++)
            {
                if (configFileData[i].StartsWith("Frustum"))
                {
                    string[] frustParts = configFileData[i].Split( new char[] {' ',';'});
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

                if (configFileData[i].StartsWith("Rotate"))
                {
                    string[] frustParts = configFileData[i].Split(new char[] { ' ',';' });
                    float angle = 0;
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    if (frustParts.Length == 6)
                    {
                        angle = (float)(Convert.ToDouble(frustParts[1])/180*Math.PI);
                        x = Convert.ToSingle(frustParts[2]);
                        y = Convert.ToSingle(frustParts[3]);
                        z = Convert.ToSingle(frustParts[4]);
                        Matrix3d mat = new Matrix3d();
                        mat.Matrix11 = SharpDX.Matrix.RotationAxis(new SharpDX.Vector3(x, y, z), angle);
                        ViewMatrix = mat * ViewMatrix;
                    }
                }
            }

            MatrixValid = true;

        }

        string saveFilename = @"c:\wwtconfig\config.xml";

        public bool SaveToXml()
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
                sb.Append("<DeviceConfig>\r\n");
                {
                    sb.Append("<Config>\r\n");
                    sb.Append(String.Format("<Device ClusterID=\"{21}\" NodeID=\"{19}\" NodeDiplayName=\"{20}\" MonitorCountX=\"{0}\" MonitorCountY=\"{1}\" MonitorX=\"{2}\" MonitorY=\"{3}\" Master=\"{4}\" Width=\"{5}\" Height=\"{6}\" Bezel=\"{7}\" ConfigFile=\"{8}\" BlendFile=\"{9}\" DistortionGrid=\"{10}\" Heading=\"{11}\" Pitch=\"{12}\" Roll=\"{13}\" UpFov=\"{14}\" DownFov=\"{15}\" MultiChannelDome=\"{16}\" DomeTilt=\"{17}\" Aspect=\"{18}\" DiffTilt=\"{22}\" MultiChannelGlobe=\"{23}\" DomeAngle=\"{24}\"></Device>\r\n",
                        MonitorCountX.ToString(), MonitorCountY.ToString(), MonitorX, MonitorY, Master.ToString(), Width.ToString(), Height.ToString(), Bezel.ToString(), ConfigFile, BlendFile, DistortionGrid, Heading, Pitch, Roll, UpFov, DownFov, MultiChannelDome.ToString(), DomeTilt, Aspect, NodeID.ToString(), NodeDiplayName, ClusterID.ToString(), DiffTilt.ToString(), MultiChannelGlobe.ToString(), DomeAngle.ToString()));
                    sb.Append("</Config>\r\n");
                }
                sb.Append("</DeviceConfig>\r\n");

                // Create the file.
                using (FileStream fs = File.Create(saveFilename))
                {
                    Byte[] info =
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
