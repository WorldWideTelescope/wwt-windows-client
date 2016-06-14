using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

using Vector3 = SharpDX.Vector3;
using Matrix = SharpDX.Matrix;
using System.IO.Compression;

namespace TerraViewer
{
    class MinorPlanets
    {
        public static List<CAAEllipticalObjectElements> MPCList = new List<CAAEllipticalObjectElements>();
        
        public static void ReadMPSCoreFile(string filename)
        {
            if (true) //(File.Exists(Properties.Settings.Default.CahceDirectory + "\\data\\mpc.bin"))
            {
                ReadFromBin();
                return;
            }
            //else
            //{
            //    Stream s = File.OpenRead(filename);
            //    MPCList.Clear();
            //    StreamReader sr = new StreamReader(s);
            //    bool dataFound = false;
            //    while (!sr.EndOfStream)
            //    {
            //        string data = sr.ReadLine();
            //        if (dataFound && data.Length > 150)
            //        {
            //            CAAEllipticalObjectElements ee = new CAAEllipticalObjectElements();

            //            ee.a = Convert.ToDouble(data.Substring(92, 11));
            //            ee.e = Convert.ToDouble(data.Substring(70, 9));
            //            ee.i = Convert.ToDouble(data.Substring(59, 9));
            //            ee.omega = Convert.ToDouble(data.Substring(48, 9));
            //            ee.w = Convert.ToDouble(data.Substring(37, 9));
            //            ee.JDEquinox = UnpackEpoch(data.Substring(20, 5));
            //            double M = Convert.ToDouble(data.Substring(26, 9));
            //            double n = Convert.ToDouble(data.Substring(80, 11));
            //            ee.T = ee.JDEquinox - (M / n);
            //            MPCList.Add(ee);
            //        }
            //        else
            //        {
            //            if (data.Length > 3 && data.Substring(0, 4) == "----")
            //            {
            //                dataFound = true;
            //            }
            //        }
            //    }
            //    sr.Close();

            //    WriteBinaryMPCData(Properties.Settings.Default.CahceDirectory + "\\data\\mpc.bin");
            //}
        }

        public static void DownloadNewMPSCoreFile()
        {
            string filename = Properties.Settings.Default.CahceDirectory + "\\data\\MPCCORB.DAT.gz";
            string url = "http://www.minorplanetcenter.net/iau/MPCORB/MPCORB.DAT.gz";

            if (!FileDownload.DownloadFile(url, filename, true))
            {
                return;
            }
            try
            {
                Stream fs = File.OpenRead(filename);
                Stream s = new GZipStream(fs, CompressionMode.Decompress);
                MPCList.Clear();
                StreamReader sr = new StreamReader(s);
                bool dataFound = false;
                while (!sr.EndOfStream)
                {
                    string data = sr.ReadLine();
                    if (dataFound && data.Length > 150)
                    {
                        CAAEllipticalObjectElements ee = new CAAEllipticalObjectElements();

                        ee.a = Convert.ToDouble(data.Substring(92, 11));
                        ee.e = Convert.ToDouble(data.Substring(70, 9));
                        ee.i = Convert.ToDouble(data.Substring(59, 9));
                        ee.omega = Convert.ToDouble(data.Substring(48, 9));
                        ee.w = Convert.ToDouble(data.Substring(37, 9));
                        ee.JDEquinox = UnpackEpoch(data.Substring(20, 5));
                        double M = Convert.ToDouble(data.Substring(26, 9));
                        double n = Convert.ToDouble(data.Substring(80, 11));
                        ee.T = ee.JDEquinox - (M / n);
                        MPCList.Add(ee);
                    }
                    else
                    {
                        if (data.Length > 3 && data.Substring(0, 4) == "----")
                        {
                            dataFound = true;
                        }
                    }
                }
                sr.Close();

                WriteBinaryMPCData(Properties.Settings.Default.CahceDirectory + "\\data\\mpc.bin");
            }
            catch
            {
                UiTools.ShowMessageBox(Language.GetLocalizedText(1003, "The image file did not download or is invalid."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
            }
        }

        private static void ReadFromBin()
        {
            MPCList = new List<CAAEllipticalObjectElements>();
            string filename = Properties.Settings.Default.CahceDirectory + "\\data\\mpc.bin";
            DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=mpcbin", filename, false, true);
            FileStream fs = new FileStream(filename, FileMode.Open);
            long len = fs.Length;
            BinaryReader br = new BinaryReader(fs);
            CAAEllipticalObjectElements ee;
            try
            {
                while (fs.Position < len)
                {
                    ee = new CAAEllipticalObjectElements(br);
                    MPCList.Add(ee);
                }
            }
            catch
            {
            }
            br.Close();
            fs.Close();
        }

        private static void WriteBinaryMPCData(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            foreach (CAAEllipticalObjectElements ee in MPCList)
            {
                ee.WriteBin(bw);
            }
            bw.Close();
            fs.Close();
        }

        public static double UnpackEpoch(string packed)
        {
            int year = GetMappedDatePart(packed[0])*100+Convert.ToInt32(packed.Substring(1,2));

            DateTime date = new DateTime(year, GetMappedDatePart(packed[3]), GetMappedDatePart(packed[4]));
            return SpaceTimeController.UtcToJulian(date);
        }

        public static int GetMappedDatePart(char packed)
        {
            if (packed >= '1' && packed <= '9')
            {
                return (packed - '1') + 1;
            }
            else
            {
                return (packed - 'A') + 10;
            }
        }
        static bool initBegun = false;

        static BlendState[] mpcBlendStates = new BlendState[7];
        // ** Begin 
        public static void DrawMPC3D(RenderContext11 renderContext, float opacity, Vector3d centerPoint)
        {
            double zoom = Earth3d.MainWindow.ZoomFactor;
            double distAlpha = ((Math.Log(Math.Max(1, zoom), 4)) - 15.5) * 90;
  
            int alpha = Math.Min(255, Math.Max(0, (int)distAlpha));


            if (alpha > 254)
            {
                return;
            }


            if (mpcVertexBuffer == null)
            {

                for (int i = 0; i < 7; i++)
                {
                    mpcBlendStates[i] = new BlendState(false, 1000, 0);
                }

                if (!initBegun)
                {
                    MethodInvoker initDelegate = new MethodInvoker(StartInit);

                    initDelegate.BeginInvoke(null, null);
                    initBegun = true;
                }
                return;
            }


            renderContext.DepthStencilMode = DepthStencilMode.Off;


            renderContext.setRasterizerState(TriangleCullMode.Off);

      
            Matrix3d offset = Matrix3d.Translation(-centerPoint);
            Matrix3d world = renderContext.World * offset;
            Matrix matrixWVP = (world * renderContext.View * renderContext.Projection).Matrix11;
            matrixWVP.Transpose();


            Vector3 cam = Vector3.TransformCoordinate(renderContext.CameraPosition.Vector311, Matrix.Invert(renderContext.World.Matrix11));

  

            renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, Grids.StarProfile.ResourceView);
            renderContext.BlendMode = BlendMode.Additive;

            if (mpcVertexBuffer != null)
            {
                for (int i = 0; i < 7; i++)
                {
                    mpcBlendStates[i].TargetState = ((Properties.Settings.Default.MinorPlanetsFilter & (int)Math.Pow(2, i)) != 0);

                    if (mpcBlendStates[i].State)
                    {
                        if (mpcVertexBuffer[i].Downlevel)
                        {
                            DownlevelKeplerPointSpriteShader11.Constants.JNow = (float)(SpaceTimeController.JNow - KeplerVertex.baseDate);
                            DownlevelKeplerPointSpriteShader11.Constants.Opacity = opacity * mpcBlendStates[i].Opacity;
                            DownlevelKeplerPointSpriteShader11.Constants.MM = 0;
                            DownlevelKeplerPointSpriteShader11.Constants.WorldViewProjection = matrixWVP;
                            DownlevelKeplerPointSpriteShader11.Constants.Scale = new SharpDX.Vector2(100f, 100f);
                            DownlevelKeplerPointSpriteShader11.Constants.ViewportScale = new SharpDX.Vector2(2f / renderContext.ViewPort.Width, 2f / renderContext.ViewPort.Height);
                            DownlevelKeplerPointSpriteShader11.Constants.CameraPosition = new SharpDX.Vector4(cam, 1);
                            DownlevelKeplerPointSpriteShader11.Use(renderContext.devContext, mpcVertexBuffer[i].Instanced);
                        }
                        else
                        {

                            KeplerPointSpriteShader11.Constants.JNow = (float)(SpaceTimeController.JNow - KeplerVertex.baseDate);
                            KeplerPointSpriteShader11.Constants.Opacity = opacity * mpcBlendStates[i].Opacity;
                            KeplerPointSpriteShader11.Constants.MM = 0;
                            KeplerPointSpriteShader11.Constants.WorldViewProjection = matrixWVP;
                            KeplerPointSpriteShader11.Constants.Scale = new SharpDX.Vector2(100f, 100f);
                            KeplerPointSpriteShader11.Constants.ViewportScale = new SharpDX.Vector2(2f / renderContext.ViewPort.Width, 2 / renderContext.ViewPort.Height);
                            KeplerPointSpriteShader11.Constants.CameraPosition = new SharpDX.Vector4(cam, 1);
                            KeplerPointSpriteShader11.Use(renderContext.devContext);
                        }

                        mpcVertexBuffer[i].Draw(renderContext, mpcVertexBuffer[i].Count, null, opacity);
                    }
                }
            }
            renderContext.BlendMode = BlendMode.Alpha;

            renderContext.Device.ImmediateContext.GeometryShader.Set(null);
        }

    
        private static void StartInit()
        {
            
            InitMPCVertexBuffer();
        }

        public static void InitMPCVertexBuffer()
        {
            MpcMutex.WaitOne();
            try
            {
                if (mpcVertexBuffer == null)
                {
                    KeplerPointSpriteSet[] mpcVertexBufferTemp = new KeplerPointSpriteSet[7];
                    MinorPlanets.ReadMPSCoreFile(@"c:\mpc\MPCORB.DAT");

                    mpcCount = MinorPlanets.MPCList.Count;
                    //KeplerVertexBuffer11 temp = new KeplerVertexBuffer11(mpcCount, RenderContext11.PrepDevice);

                    List<KeplerVertex>[] lists = new List<KeplerVertex>[7];
                    for (int i = 0; i < 7; i++)
                    {
                        lists[i] = new List<KeplerVertex>();
                    }

                    foreach (CAAEllipticalObjectElements ee in MinorPlanets.MPCList)
                    {
                        int listID = 0;
                        if (ee.a < 2.5)
                        {
                            listID = 0;
                        }
                        else if (ee.a < 2.83)
                        {
                            listID = 1;
                        }
                        else if (ee.a < 2.96)
                        {
                            listID = 2;
                        }
                        else if (ee.a < 3.3)
                        {
                            listID = 3;
                        }
                        else if (ee.a < 5)
                        {
                            listID = 4;
                        }
                        else if (ee.a < 10)
                        {
                            listID = 5;
                        }
                        else
                        {
                            listID = 6;
                        }

                        KeplerVertex vert = new KeplerVertex();
                        vert.Fill(ee);

                        lists[listID].Add(vert);
                    }

                    for (int i = 0; i < 7; i++)
                    {
                        mpcVertexBufferTemp[i] = new KeplerPointSpriteSet(RenderContext11.PrepDevice, lists[i].ToArray());
                    }

                    mpcVertexBuffer = mpcVertexBufferTemp;
                }
            }
            finally
            {
                MpcMutex.ReleaseMutex();
            }
        }

        static Mutex MpcMutex = new Mutex();

        static KeplerPointSpriteSet[] mpcVertexBuffer = null;
        static int mpcCount = 0;
    }


   
}
