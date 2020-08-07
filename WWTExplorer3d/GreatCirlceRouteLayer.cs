using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if WINDOWS_UWP
using XmlElement = Windows.Data.Xml.Dom.XmlElement;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;
#else
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Drawing;
using System.Xml;
#endif

namespace TerraViewer
{
    public class GreatCirlceRouteLayer : Layer
    {
        TriangleList triangleList = null;
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
            triangleList.Draw(renderContext, opacity * this.Opacity, TriangleList.CullMode.CounterClockwise);



            return true;

        }

        private void InitializeRoute(RenderContext11 renderContext)
        {
            triangleList = new TriangleList();
            triangleList.Decay = 1000;
            triangleList.Sky = this.Astronomical;
            triangleList.TimeSeries = true;
            triangleList.DepthBuffered = false;
            triangleList.AutoTime = false;

            int steps = 500;

            Vector3d start = Coordinates.GeoTo3dDouble(latStart, lngStart);
            Vector3d end = Coordinates.GeoTo3dDouble(latEnd, lngEnd);
            Vector3d dir = end - start;
            dir.Normalize();

            Vector3d startNormal = start;
            startNormal.Normalize();

            Vector3d left = Vector3d.Cross(startNormal, dir);
            Vector3d right = Vector3d.Cross(dir, startNormal);
            left.Normalize();
            right.Normalize();

            left.Multiply(.001*width);
            right.Multiply(.001 * width);

            Vector3d lastLeft = new Vector3d();
            Vector3d lastRight = new Vector3d();
            bool firstTime = true;
            for (int i = 0; i <= steps; i++)
            {
                Vector3d v = Vector3d.Lerp(start, end, i / (float)steps);
                v.Normalize();
                Vector3d cl = v;
                Vector3d cr = v;

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
            double[] paramList = new double[6];
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
            return new string[] { "Percentage" , "Color.Red", "Color.Green", "Color.Blue", "Color.Alpha", "Opacity" };
        }
        public override BaseTweenType[] GetParamTypes()
        {
            return new BaseTweenType[] { BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear };
        }

        public override void SetParams(double[] paramList)
        {
            if (paramList.Length == 6)
            {
                percentComplete = paramList[0];
                Opacity = (float)paramList[5];
                Color color = Color.FromArgb((int)(paramList[4] * 255), (int)(paramList[1] * 255), (int)(paramList[2] * 255), (int)(paramList[3] * 255));
                Color = color;
            }
        }

        private double latStart = 0;

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
        private double lngStart = 0;

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
        private double latEnd = 0;

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
        private double lngEnd = 0;

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