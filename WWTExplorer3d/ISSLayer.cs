using System;
using System.Drawing;
using System.IO;


namespace TerraViewer
{
    class ISSLayer : Object3dLayer
    {
        public ISSLayer()
        {
            ID = ISSGuid;
        }

        public static Guid ISSGuid = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
        delegate void BackInitDelegate();
        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {
            if (object3d == null && issmodel == null)
            {
                if (!loading)
                {
                    var worldView = renderContext.World * renderContext.View;
                    var v = worldView.Transform(Vector3d.Empty);
                    var scaleFactor = Math.Sqrt(worldView.M11 * worldView.M11 + worldView.M22 * worldView.M22 + worldView.M33 * worldView.M33) / 1;
                    var dist = v.Length();
                    var radius = scaleFactor;

                    // Calculate pixelsPerUnit which is the number of pixels covered
                    // by an object 1 AU at the distance of the planet center from
                    // the camera. This calculation works regardless of the projection
                    // type.
                    var viewportHeight = (int)renderContext.ViewPort.Height;
                    var p11 = renderContext.Projection.M11;
                    var p34 = renderContext.Projection.M34;
                    var p44 = renderContext.Projection.M44;
                    var w = Math.Abs(p34) * dist + p44;
                    var pixelsPerUnit = (float)(p11 / w) * viewportHeight;
                    var radiusInPixels = (float)(radius * pixelsPerUnit);
                    if (radiusInPixels > 0.5f)
                    {
                        BackInitDelegate initBackground = LoadBackground;
                        initBackground.BeginInvoke(null, null);
                    }
                }


               
            }

            object3d = issmodel;
            return base.Draw(renderContext, opacity, flat);
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled = Settings.Active.ShowISSModel;
            }
            set
            {
                Properties.Settings.Default.ShowISSModel = base.Enabled = value;
            }
        }

        public override LayerUI GetPrimaryUI()
        {
            return null;
        }

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            return;
        }

        public override void LoadData(string path)
        {
            return;
        }
        public override void CleanUp()
        {
           // base.CleanUp();
        }

        public static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[8192];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
        static bool loading;

        static Object3d issmodel;
        public static void LoadBackground()
        {
            if (loading)
            {
                return;
            }

            loading = true;
            var path = Properties.Settings.Default.CahceDirectory + @"\mdl\155\";
            var filename = path + "mdl.zip";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!File.Exists(filename))
            {
                DataSetManager.DownloadFile("http://www.worldwidetelescope.org/data/iss.zip", filename, false, true);
                
            }

            using (Stream s = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var zip = new ZipArchive(s);
                foreach (var zFile in zip.Files)
                {
                    Stream output = File.Open(path+zFile.Filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                    var input = zFile.GetFileStream();
                    CopyStream(input, output);
                    input.Close();
                    output.Close();
                }
            }

            filename = path + "mdl.3ds";
            if (File.Exists(filename))
            {
                var o3d = new Object3d(filename, true, false, true, Color.White);
                if (o3d != null)
                {
                    o3d.ISSLayer = true;
                    issmodel = o3d;
                }
            }
        }
    }
}
