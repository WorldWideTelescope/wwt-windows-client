using System.Drawing;
using System.Xml;

namespace TerraViewer
{
    public class GreatCirlceRouteLayer : Layer
    {
        TriangleList triangleList;
        public override void CleanUp()
        {
            if (triangleList != null)
            {
                triangleList.Clear();
            }
            triangleList = null;
            base.CleanUp();
        }

        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {
            
            if (triangleList == null)
            {
                InitializeRoute(renderContext);
            }
            triangleList.JNow = percentComplete / 100;
            triangleList.Draw(renderContext, opacity * Opacity, TriangleList.CullMode.CounterClockwise);



            return true;

        }

        private void InitializeRoute(RenderContext11 renderContext)
        {
            triangleList = new TriangleList();
            triangleList.Decay = 1000;
            triangleList.Sky = Astronomical;
            triangleList.TimeSeries = true;
            triangleList.DepthBuffered = false;
            triangleList.AutoTime = false;

            var steps = 500;

            var start = Coordinates.GeoTo3dDouble(latStart, lngStart);
            var end = Coordinates.GeoTo3dDouble(latEnd, lngEnd);
            var dir = end - start;
            dir.Normalize();

            var startNormal = start;
            startNormal.Normalize();

            var left = Vector3d.Cross(startNormal, dir);
            var right = Vector3d.Cross(dir, startNormal);
            left.Normalize();
            right.Normalize();

            left.Multiply(.001*width);
            right.Multiply(.001 * width);

            var lastLeft = new Vector3d();
            var lastRight = new Vector3d();
            var firstTime = true;
            for (var i = 0; i <= steps; i++)
            {
                var v = Vector3d.Lerp(start, end, i / (float)steps);
                v.Normalize();
                var cl = v;
                var cr = v;

                cl.Add(left);
                cr.Add(right);
                
                if (!firstTime)
                {
                    triangleList.AddQuad(lastRight, lastLeft, cr, cl, Color, new Dates(i / (float)steps, 2));
                }
                else
                {
                   firstTime = false;
                }

                lastLeft = cl;
                lastRight = cr;


            }

        }

        public override double[] GetParams()
        {
            var paramList = new double[6];
            paramList[0] = percentComplete;
            paramList[1] = Color.R / 255;
            paramList[2] = Color.G / 255;
            paramList[3] = Color.B / 255;
            paramList[4] = Color.A / 255;
            paramList[5] = Opacity;


            return paramList;

        }

        public override string[] GetParamNames()
        {
            return new[] { "Percentage" , "Color.Red", "Color.Green", "Color.Blue", "Color.Alpha", "Opacity" };
        }
        public override BaseTweenType[] GetParamTypes()
        {
            return new[] { BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear };
        }

        public override void SetParams(double[] paramList)
        {
            if (paramList.Length == 6)
            {
                percentComplete = paramList[0];
                Opacity = (float)paramList[5];
                var color = Color.FromArgb((int)(paramList[4] * 255), (int)(paramList[1] * 255), (int)(paramList[2] * 255), (int)(paramList[3] * 255));
                Color = color;
            }
        }

        private double latStart;

        [LayerProperty]
        public double LatStart
        {
            get { return latStart; }
            set
            {
                if (latStart != value)
                {
                    latStart = value;
                    version++;
                }
           }
        }
        private double lngStart;

        [LayerProperty]
        public double LngStart
        {
            get { return lngStart; }
            set
            {
                if (lngStart != value)
                {
                    lngStart = value;
                    version++;
                }
            }
        }
        private double latEnd;

        [LayerProperty]
        public double LatEnd
        {
            get { return latEnd; }
            set
            {
                if (latEnd != value)
                {
                    latEnd = value;
                    version++;
                }
            }
        }
        private double lngEnd;

        [LayerProperty]
        public double LngEnd
        {
            get { return lngEnd; }
            set
            {
                if (lngEnd != value)
                {
                    lngEnd = value;
                    version++;
                }
            }
        }

        private double width = 4;

        [LayerProperty]
        public double Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    version++;
                }
            }
        }

        private double percentComplete = 100;

        [LayerProperty]
        public double PercentComplete
        {
            get { return percentComplete; }
            set
            {
                if (percentComplete != value)
                {
                    percentComplete = value;
                    version++;
                }
            }
        }

        public override void WriteLayerProperties(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("LatStart", LatStart.ToString());
            xmlWriter.WriteAttributeString("LngStart", LngStart.ToString());
            xmlWriter.WriteAttributeString("LatEnd", LatEnd.ToString());
            xmlWriter.WriteAttributeString("LngEnd", LngEnd.ToString());
            xmlWriter.WriteAttributeString("Width", Width.ToString());
            xmlWriter.WriteAttributeString("PercentComplete", PercentComplete.ToString());

        }



        public override void InitializeFromXml(XmlNode node)
        {
            latStart = double.Parse(node.Attributes["LatStart"].Value);
            lngStart = double.Parse(node.Attributes["LngStart"].Value);
            latEnd = double.Parse(node.Attributes["LatEnd"].Value);
            lngEnd = double.Parse(node.Attributes["LngEnd"].Value);
            width = double.Parse(node.Attributes["Width"].Value);
            percentComplete = double.Parse(node.Attributes["PercentComplete"].Value);
        }
    }
}