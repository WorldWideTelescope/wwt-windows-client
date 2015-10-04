using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SharpDX;
using Vector3 = SharpDX.Vector3;
using Matrix = SharpDX.Matrix;

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

        private static void ReadFromBin()
        {
            MPCList = new List<CAAEllipticalObjectElements>();
            var filename = Properties.Settings.Default.CahceDirectory + "\\data\\mpc.bin";
            DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=mpcbin", filename, false, true);
            var fs = new FileStream(filename, FileMode.Open);
            var len = fs.Length;
            var br = new BinaryReader(fs);
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
            var fs = new FileStream(filename, FileMode.Create);
            var bw = new BinaryWriter(fs);
            foreach (var ee in MPCList)
            {
                ee.WriteBin(bw);
            }
            bw.Close();
            fs.Close();
        }

        public static double UnpackEpoch(string packed)
        {
            var year = GetMappedDatePart(packed[0])*100+Convert.ToInt32(packed.Substring(1,2));

            var date = new DateTime(year, GetMappedDatePart(packed[3]), GetMappedDatePart(packed[4]));
            return SpaceTimeController.UtcToJulian(date);
        }

        public static int GetMappedDatePart(char packed)
        {
            if (packed >= '1' && packed <= '9')
            {
                return (packed - '1') + 1;
            }
            return (packed - 'A') + 10;
        }

        static bool initBegun;

        static readonly BlendState[] mpcBlendStates = new BlendState[7];
        // ** Begin 
        public static void DrawMPC3D(RenderContext11 renderContext, float opacity, Vector3d centerPoint)
        {
            var zoom = Earth3d.MainWindow.ZoomFactor;
            var distAlpha = ((Math.Log(Math.Max(1, zoom), 4)) - 15.5) * 90;
  
            var alpha = Math.Min(255, Math.Max(0, (int)distAlpha));


            if (alpha > 254)
            {
                return;
            }


            if (mpcVertexBuffer == null)
            {

                for (var i = 0; i < 7; i++)
                {
                    mpcBlendStates[i] = new BlendState(false, 1000, 0);
                }

                if (!initBegun)
                {
                    var initDelegate = new MethodInvoker(StartInit);

                    initDelegate.BeginInvoke(null, null);
                    initBegun = true;
                }
                return;
            }


            renderContext.DepthStencilMode = DepthStencilMode.Off;


            renderContext.setRasterizerState(TriangleCullMode.Off);

      
            var offset = Matrix3d.Translation(-centerPoint);
            var world = renderContext.World * offset;
            var matrixWVP = (world * renderContext.View * renderContext.Projection).Matrix11;
            matrixWVP.Transpose();


            var cam = Vector3.TransformCoordinate(renderContext.CameraPosition.Vector311, Matrix.Invert(renderContext.World.Matrix11));

  

            renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, Grids.StarProfile.ResourceView);
            renderContext.BlendMode = BlendMode.Additive;

            if (mpcVertexBuffer != null)
            {
                for (var i = 0; i < 7; i++)
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
                            DownlevelKeplerPointSpriteShader11.Constants.Scale = new Vector2(100f, 100f);
                            DownlevelKeplerPointSpriteShader11.Constants.ViewportScale = new Vector2(2f / renderContext.ViewPort.Width, 2f / renderContext.ViewPort.Height);
                            DownlevelKeplerPointSpriteShader11.Constants.CameraPosition = new Vector4(cam, 1);
                            DownlevelKeplerPointSpriteShader11.Use(renderContext.devContext, mpcVertexBuffer[i].Instanced);
                        }
                        else
                        {

                            KeplerPointSpriteShader11.Constants.JNow = (float)(SpaceTimeController.JNow - KeplerVertex.baseDate);
                            KeplerPointSpriteShader11.Constants.Opacity = opacity * mpcBlendStates[i].Opacity;
                            KeplerPointSpriteShader11.Constants.MM = 0;
                            KeplerPointSpriteShader11.Constants.WorldViewProjection = matrixWVP;
                            KeplerPointSpriteShader11.Constants.Scale = new Vector2(100f, 100f);
                            KeplerPointSpriteShader11.Constants.ViewportScale = new Vector2(2f / renderContext.ViewPort.Width, 2 / renderContext.ViewPort.Height);
                            KeplerPointSpriteShader11.Constants.CameraPosition = new Vector4(cam, 1);
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
                    var mpcVertexBufferTemp = new KeplerPointSpriteSet[7];
                    ReadMPSCoreFile(@"c:\mpc\MPCORB.DAT");

                    mpcCount = MPCList.Count;
                    //KeplerVertexBuffer11 temp = new KeplerVertexBuffer11(mpcCount, RenderContext11.PrepDevice);

                    var lists = new List<KeplerVertex>[7];
                    for (var i = 0; i < 7; i++)
                    {
                        lists[i] = new List<KeplerVertex>();
                    }

                    foreach (var ee in MPCList)
                    {
                        var listID = 0;
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

                        var vert = new KeplerVertex();
                        vert.Fill(ee);

                        lists[listID].Add(vert);
                    }

                    for (var i = 0; i < 7; i++)
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

        static readonly Mutex MpcMutex = new Mutex();

        static KeplerPointSpriteSet[] mpcVertexBuffer;
        static int mpcCount;
    }


   
}
