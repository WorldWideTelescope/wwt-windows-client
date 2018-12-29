
using System;
using System.IO;
using System.Text;
using System.Xml;

namespace TerraViewer
{

    public class Config
    {
        public Config()
        {
#if !WINDOWS_UWP
            //We don't use this in UWP apps
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
            else if (File.Exists(@"c:\wwtconfig\config.xml"))
            {
                saveFilename = @"c:\wwtconfig\config.xml";
                ReadFromXML(saveFilename);
            }
#endif
        }

        public int MonitorCountX = 1;
        public int MonitorCountY = 1;
        public int MonitorX = 0;
        public int MonitorY = 0;
        public bool Master = true;
        public int Width = 1920;
        public int Height = 1200;
        public double Bezel = 1.07;

        public string ConfigFile;
        public string BlendFile;
        public string DistortionGrid;
        public int DistortionGridWidth = 1;
        public int DistortionGridHeight = 1;
        public bool UsingSgcWarpMap = false;
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
                return (!String.IsNullOrEmpty(BlendFile) && ( !String.IsNullOrEmpty(ConfigFile) || !String.IsNullOrEmpty(DistortionGrid)) );
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
        public float LeftFov;
        public float RightFov;
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
        public Matrix3d ViewMatrix = Matrix3d.Identity;

        public SharpDX.Matrix ProjectorMatrixSGC = SharpDX.Matrix.Identity;
#if !WINDOWS_UWP

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


        private void ParseConfigFile()
        {
            if (String.IsNullOrEmpty(ConfigFile))
            {
                return;
            }

            if (ConfigFile.ToLower().EndsWith(".sgc"))
            {
                ReadSGCFile(ConfigFile);
                return;
            }

            string[] configFileData = File.ReadAllLines(ConfigFile);
            for (int i = 0; i < configFileData.Length; i++)
            {
                configFileData[i] = configFileData[i].Trim();
            }


            for (int i = 0; i < configFileData.Length; i++)
            {
                if (configFileData[i].StartsWith("Frustum"))
                {
                    string[] frustParts = configFileData[i].Split(new char[] { ' ', ';' });
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
                    string[] frustParts = configFileData[i].Split(new char[] { ' ', ';' });
                    float angle = 0;
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    if (frustParts.Length == 6)
                    {
                        angle = (float)(Convert.ToDouble(frustParts[1]) / 180 * Math.PI);
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



        //It's a binary file with the following format
        //fileid: 3c
        //version: B
        //distortionType: I
        //Quaternion: 4f
        //position: 3f
        //fov: 4f
        //mesh width: I
        //mesh height: I
        //vertices: 3f per vertex
        //numindices: I
        //indices: I per index

        public Vector6[,] DistortionGridVertices = null;

        

        void ReadSGCFile(string filename)
        {
            FileStream s = new FileStream(filename, FileMode.Open);
            BinaryReader br = new BinaryReader(s);
            byte[] fileID = br.ReadBytes(3);
            byte version = br.ReadByte();
            Int32 distortionType = br.ReadInt32();
            float[] quat = new float[4];
            for (int i = 0; i < 4; i++)
            {
                quat[i] = br.ReadSingle();
            }

            float[] pos = new float[3];

            for (int i = 0; i < 3; i++)
            {
                pos[i] = br.ReadSingle();
            }

            float up = br.ReadSingle();
            float down = br.ReadSingle();
            float left = br.ReadSingle();
            float right = br.ReadSingle();

            int meshWidth = br.ReadInt32();
            int meshHeight = br.ReadInt32();

            DistortionGridVertices = new Vector6[meshWidth, meshHeight];

            SharpDX.Quaternion quatIn = new SharpDX.Quaternion(quat[0], quat[1], quat[2], quat[3]);

            ProjectorMatrixSGC = SharpDX.Matrix.RotationQuaternion(quatIn);

            SharpDX.Matrix mToggle_YZ = new SharpDX.Matrix(  -1, 0, 0, 0,
                                                             0, -1, 0, 0,
                                                             0, 0, 1, 0,
                                                             0, 0, 0, 1);
            ProjectorMatrixSGC =   mToggle_YZ * ProjectorMatrixSGC * mToggle_YZ;
            ProjectorMatrixSGC.Invert();
            
           

            Vector4d quaternion = new Vector4d(quat[0], quat[1], quat[2], quat[3]);

            
            double roll=0;
            double heading=0;
            double pitch=0;

            ToEulerianAngle(quaternion, out pitch, out roll, out heading);
           
            float num2 = (float)(((2.0) / (1.0 / Math.Tan((Math.Abs(up) / 180.0) * Math.PI))) / 2.0);
            float num3 = (float)(((2.0) / (1.0 / Math.Tan((Math.Abs(down) / 180.0) * Math.PI))) / 2.0);
            float num4 = (float)(((2.0) / (1.0 / Math.Tan((Math.Abs(right) / 180.0) * Math.PI))) / 2.0);
            float num5 = (float)(((2.0) / (1.0 / Math.Tan((Math.Abs(left) / 180.0) * Math.PI))) / 2.0);

            Aspect = (num4 + num5) / (num2 + num3);

            UpFov = Math.Abs(up);
            DownFov = Math.Abs(down);

            RightFov = Math.Abs(right);
            LeftFov = Math.Abs(left);

            Heading = (float)heading;
            Pitch = (float)pitch;
            Roll = (float)roll;

            DistortionGridWidth = meshWidth;
            DistortionGridHeight = meshHeight;

            UsingSgcWarpMap = true;

            for (int y = 0; y < meshHeight; y++)
            {
                for (int x = 0; x < meshWidth; x++)
                {
                    DistortionGridVertices[x, y] = new Vector6(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                }
            }

            int indexCount = br.ReadInt32();

            int[] indices = new int[indexCount];

            for (int i = 0; i < indexCount; i++)
            {
                indices[i] = br.ReadInt32();
            }

            int t = meshHeight * meshWidth;

            br.Close();
        }

        static void ToEulerianAngle(Vector4d self, out double bank, out double attitude, out double heading)
        {
            double t = self.X * self.Y + self.Z * self.W;
            if (t > 0.4999)
            {
                heading = 2 * Math.Atan2(self.X, self.W);
                attitude = Math.PI / 2;
                bank = 0;
            }
            else if (t < -0.4999)
            {
                heading = -2 * Math.Atan2(self.X, self.W);
                attitude = -Math.PI / 2;
                bank = 0;
            }
            else
            {
                double sqx = self.X * self.X;
                double sqy = self.Y * self.Y;
                double sqz = self.Z * self.Z;
                heading = Math.Atan2(2 * self.Y * self.W - 2 * self.X * self.Z, 1 - 2 * sqy - 2 * sqz);
                attitude = Math.Asin(2 * t);
                bank = Math.Atan2(2 * self.X * self.W - 2 * self.Y * self.Z, 1 - 2 * sqx - 2 * sqz);
            }
            bank = bank / Math.PI * 180;
            attitude = attitude / Math.PI * 180;
            heading = heading / Math.PI * 180;

            return;





            //double ysqr = q.Y * q.Y;

            //// roll (x-axis rotation)
            //double t0 = +2.0 * (q.W * q.X + q.Y * q.Z);
            //double t1 = +1.0 - 2.0 * (q.X * q.X + ysqr);
            //roll = Math.Atan2(t0, t1) / Math.PI * 180;

            //// pitch (y-axis rotation)
            //double t2 = +2.0 * (q.W * q.Y - q.Z * q.X);
            //t2 = ((t2 > 1.0) ? 1.0 : t2);
            //t2 = ((t2 < -1.0) ? -1.0 : t2);
            //pitch = Math.Asin(t2) / Math.PI * 180;

            //// yaw (z-axis rotation)
            //double t3 = +2.0 * (q.W * q.Z + q.X * q.Y);
            //double t4 = +1.0 - 2.0 * (ysqr + q.Z * q.Z);
            //yaw = Math.Atan2(t3, t4) / Math.PI * 180;
        }

        static void ToEulerianAngle2(Vector4d q, out double roll, out double pitch, out double yaw)
        {
            double ysqr = q.Y * q.Y;

            // roll (x-axis rotation)
            double t0 = +2.0 * (q.W * q.X + q.Y * q.Z);
            double t1 = +1.0 - 2.0 * (q.X * q.X + ysqr);
            roll = Math.Atan2(t0, t1) / Math.PI * 180;

            // pitch (y-axis rotation)
            double t2 = +2.0 * (q.W * q.Y - q.Z * q.X);
            t2 = ((t2 > 1.0) ? 1.0 : t2);
            t2 = ((t2 < -1.0) ? -1.0 : t2);
            pitch = Math.Asin(t2) / Math.PI * 180;

            // yaw (z-axis rotation)
            double t3 = +2.0 * (q.W * q.Z + q.X * q.Y);
            double t4 = +1.0 - 2.0 * (ysqr + q.Z * q.Z);
            yaw = Math.Atan2(t3, t4) / Math.PI * 180;
        }

        string saveFilename = @"c:\wwtconfig\config.xml";
#endif
        public bool SaveToXml()
        {
#if !WINDOWS_UWP
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
#endif
            return true;
        }
    }
    public struct Vector6
    {
        public float X;
        public float Y;
        public float Z;
        public float T;
        public float U;
        public float V;
        public Vector6(float x, float y, float z, float t, float u, float v)
        {
            X = x;
            Y = y;
            Z = z;
            U = u;
            V = v;
            T = t;

        }
    }

    public class GridSampler
    {
        Vector6[,] vertices = null;

        public int Width = 0;
        public int Height = 0;

        public int GridWidth = 0;
        public int GridHeight = 0;
        public float xFac = 1;
        public float yFac = 1;

        public GridSampler(int width, int height, int gridWidth, int gridHeight, Vector6[,] verts)
        {
            vertices = verts;
            Width = width;
            Height = height;
            GridHeight = gridHeight;
            GridWidth = gridWidth;

            xFac = ((float)GridWidth - 1) / (Width - 1);
            yFac = ((float)GridHeight - 1) / (Height - 1);
        }

        public SharpDX.Vector2 Sample(int x, int y)
        {
            float xf = x * xFac;
            float yf = y * yFac;

            int x1 = (int)xf; // Whole part
            int y1 = (int)yf; // Whole Part

            float xa = xf - x1; //remainder
            float ya = yf - y1; //remainder

            //interpolate left to right
            float ut = (vertices[x1, y1].T * (1 - xa)) + (vertices[Math.Min(GridWidth - 1, x1 + 1), y1].T * xa);
            float ub = (vertices[x1, Math.Min(GridHeight - 1, y1 + 1)].T * (1 - xa)) + (vertices[Math.Min(GridWidth - 1, x1 + 1), Math.Min(GridHeight - 1, y1 + 1)].T * xa);

            float vt = (vertices[x1, y1].U * (1 - xa)) + (vertices[Math.Min(GridWidth - 1, x1 + 1), y1].U * xa);
            float vb = (vertices[x1, Math.Min(GridHeight - 1, y1 + 1)].U * (1 - xa)) + (vertices[Math.Min(GridWidth - 1, x1 + 1), Math.Min(GridHeight - 1, y1 + 1)].U * xa);

            return new SharpDX.Vector2(ut * (1 - ya) + (ub * ya), vt * (1 - ya) + (vb * ya));
        }
    }
}
