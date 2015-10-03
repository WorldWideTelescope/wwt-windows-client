using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;

namespace TerraViewer
{

    public class Key
    {
        public const string ClipboardFormatSelection = "WorldWideTelescope.KeySelection";
        public const string ClipboardFormatProperties = "WorldWideTelescope.KeyProperties";
        public const string ClipboardFormatColumn = "WorldWideTelescope.KeyColumn";
        public enum KeyType { Linear, Exponential, EaseIn, EaseOut, EaseInOut, Instant, Custom };
        public double Time;
        public double Value;
        public KeyType InFunction;
        public BaseTweenType BaseTweenType = BaseTweenType.Linear;
        public double P1 = 0;
        public double P2 = 0;
        public double P3 = 1;
        public double P4 = 1;

        public double GhostTime = 0;

        public Key(double time, double value, KeyType inFunction, BaseTweenType baseTweenType)
        {
            Time = time;
            Value = value;
            InFunction = inFunction;
            BaseTweenType = baseTweenType;
        }

        public virtual void SaveToXml(System.Xml.XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Key");
            xmlWriter.WriteAttributeString("Time", Time.ToString());
            xmlWriter.WriteAttributeString("Value", Value.ToString());
            xmlWriter.WriteAttributeString("InFunction", InFunction.ToString());
            xmlWriter.WriteAttributeString("BaseTweenType", BaseTweenType.ToString());
            xmlWriter.WriteAttributeString("P1", P1.ToString());
            xmlWriter.WriteAttributeString("P2", P2.ToString());
            xmlWriter.WriteAttributeString("P3", P3.ToString());
            xmlWriter.WriteAttributeString("P4", P4.ToString());
            xmlWriter.WriteEndElement();
        }

        public Key(System.Xml.XmlNode node)
        {
            Time = double.Parse(node.Attributes["Time"].Value);
            Value = double.Parse(node.Attributes["Value"].Value);
            InFunction = (KeyType)Enum.Parse(typeof(KeyType), node.Attributes["InFunction"].Value);
            if (node.Attributes["BaseTweenType"] != null)
            {
                BaseTweenType = (BaseTweenType)Enum.Parse(typeof(BaseTweenType), node.Attributes["BaseTweenType"].Value);
            }

            if (node.Attributes["P1"] != null)
            {
                P1 = double.Parse(node.Attributes["P1"].Value);
                P2 = double.Parse(node.Attributes["P2"].Value);
                P3 = double.Parse(node.Attributes["P3"].Value);
                P4 = double.Parse(node.Attributes["P4"].Value);
            }
        }

        const double factor = 0.1085712344;

        public double EaseCurve(double alpha)
        {
            switch (InFunction)
            {
                case KeyType.Linear:
                    return alpha;
                case KeyType.Exponential:
                    return Math.Pow(alpha, 2);
                case KeyType.EaseIn:
                    return ((1 - alpha) * Math.Sinh(alpha / (factor * 2)) / 100.0) + alpha * alpha;
                case KeyType.EaseOut:
                    return (alpha * (1 - Math.Sinh((1.0 - alpha) / (factor * 2)) / 100.0)) + (1.0 - alpha) * alpha;
                case KeyType.EaseInOut:
                    if (alpha < .5)
                    {
                        return Math.Sinh(alpha / factor) / 100.0;
                    }
                    else
                    {
                        return 1.0 - (Math.Sinh((1.0 - alpha) / factor) / 100.0);
                    }
                case KeyType.Custom:
                    {
                        ks.ControlPoint1 = new System.Windows.Point(P1, P2);
                        ks.ControlPoint2 = new System.Windows.Point(P3, P4);

                        return ks.GetSplineProgress(alpha);
                        //return ComputeSpline(first, new Vector2d(P1, P2), new Vector2d(P3, P4), last, alpha).Y;
                    }
                case KeyType.Instant:
                    {
                        if (alpha > .999)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }         
                default:
                    return alpha;
            }
        }

        public static double EaseCurve(KeyType function, double alpha, double p1, double p2, double p3, double p4)
        {
            switch (function)
            {
                case KeyType.Linear:
                    return alpha;
                case KeyType.Exponential:
                    return Math.Pow(alpha, 2);
                case KeyType.EaseIn:
                    return ((1 - alpha) * Math.Sinh(alpha / (factor * 2)) / 100.0) + alpha * alpha;
                case KeyType.EaseOut:
                    return (alpha * (1 - Math.Sinh((1.0 - alpha) / (factor * 2)) / 100.0)) + (1.0 - alpha) * alpha;
                case KeyType.EaseInOut:
                    if (alpha < .5)
                    {
                        return Math.Sinh(alpha / factor) / 100.0;
                    }
                    else
                    {
                        return 1.0 - (Math.Sinh((1.0 - alpha) / factor) / 100.0);
                    }
                case KeyType.Custom:
                    {
                        ks.ControlPoint1 = new System.Windows.Point(p1, p2);
                        ks.ControlPoint2 = new System.Windows.Point(p3, p4);

                        return ks.GetSplineProgress(alpha);
                        //return ComputeSpline(first, new Vector2d(P1, P2), new Vector2d(P3, P4), last, alpha).Y;
                    }
                case KeyType.Instant:
                    {
                        if (alpha > .999)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                default:
                    return alpha;
            }
        }

        static Vector2d first = new Vector2d(0, 0);      
        static Vector2d last = new Vector2d(1, 1);

        static readonly KeySpline ks = new KeySpline();

        private Vector2d ComputeSpline(Vector2d begin, Vector2d control1, Vector2d control2, Vector2d end, double tween)
        {
            var A1 = Vector2d.Lerp(begin, control1, tween);
            var A2 = Vector2d.Lerp(control1, control2, tween);
            var A3 = Vector2d.Lerp(control2, end, tween);

            var B1 = Vector2d.Lerp(A1, A2, tween);
            var B2 = Vector2d.Lerp(A2, A3, tween);
            return Vector2d.Lerp(B1, B2, tween);
        }
    }
}
