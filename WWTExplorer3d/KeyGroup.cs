using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TerraViewer
{
    public class KeyGroup
    {
        public String ParameterName = "Anonymous";
        public SortedList<int, Key> Keys = new SortedList<int, Key>();

        static int quatized = 60 * 60 * 10000;

        public static int Quant(double time)
        {
            return (int)(time * quatized +.9);
        }

        public double Tween(double tweenFactor)
        {

            Key first = Keys[0];
            Key last = Keys[0];
            //walk list and tween keys
            foreach (Key key in Keys.Values)
            {
                if (key.Time > first.Time && key.Time < tweenFactor)
                {
                    first = key;
                    last = first;
                }
                if (key.Time >= tweenFactor)
                {
                    last = key;
                    break;
                }
            }

            if (last.Time < tweenFactor)
            {
                first = last;
            }

            //todo add support for multiple curves
            double keyTween = 0;
            if (last.Time != first.Time)
            {
                keyTween = (tweenFactor - first.Time) / (last.Time - first.Time);

            }

            keyTween = last.EaseCurve(keyTween);

            switch (last.BaseTweenType)
            {
                 case BaseTweenType.Constant:
                    return first.Value;
                 case BaseTweenType.Power:
                    return Math.Pow(2, Math.Log(last.Value, 2) * keyTween + Math.Log(first.Value, 2) * (1.0 - keyTween));
                case BaseTweenType.Linear:
                    break;
                case BaseTweenType.Log:
                    break;
                case BaseTweenType.Boolean:
                    break;

                case BaseTweenType.PlanetID:
                    return last.Value == first.Value ? last.Value : 20;

                default:
                    break;
            }

            return (first.Value * (1 - keyTween)) + (last.Value * keyTween);
        }

        public void SaveToXml(System.Xml.XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Keys");
            xmlWriter.WriteAttributeString("ParameterName", ParameterName.ToString());
            foreach (Key key in Keys.Values)
            {
                key.SaveToXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }

        public void SaveToXml(System.Xml.XmlTextWriter xmlWriter, Dictionary<string, VisibleKey> selectedKeys, AnimationTarget target, int parameterIndex)
        {
            bool anythingToWrite = false;
            foreach (Key key in Keys.Values)
            {
                if (selectedKeys.ContainsKey(VisibleKey.GetIndexKey(target, parameterIndex, key.Time)))
                {
                    anythingToWrite = true;
                }
            }

            if (anythingToWrite)
            {
                xmlWriter.WriteStartElement("Keys");
                xmlWriter.WriteAttributeString("ParameterName", ParameterName.ToString());
                foreach (Key key in Keys.Values)
                {
                    if (selectedKeys.ContainsKey(VisibleKey.GetIndexKey(target, parameterIndex, key.Time)))
                    {
                        key.SaveToXml(xmlWriter);
                    }
                }
                xmlWriter.WriteEndElement();
            }
        }

        public void FromXml(System.Xml.XmlNode node)
        {
            ParameterName = node.Attributes["ParameterName"].Value;
            foreach (System.Xml.XmlNode child in node.ChildNodes)
            {
                Key key = new Key(child);
                Keys.Add(Quant(key.Time), key);
            }
        }

        internal void DeleteKey(double time)
        {
            if (time > 0)
            {
                // Cant delete zero keys..
                if (Keys.ContainsKey(Quant(time)))
                {
                    Keys.Remove(Quant(time));
                }
            }
        }

        internal void DeleteKey(int time)
        {
            if (time > 0)
            {
                // Cant delete zero keys..
                if (Keys.ContainsKey(time))
                {
                    Keys.Remove(time);
                }
            }
        }

        public Key GetKey(double time)
        {
            int k = Quant(time);
            if (Keys.ContainsKey(k))
            {
                return Keys[k];
            }
            return null;
        }

        internal bool MoveKey(double time, double newTime)
        {
            bool collide = false;
            Key key = GetKey(time);
            DeleteKey(time);
            key.Time = newTime;
            int k = Quant(newTime);

            if (Keys.ContainsKey(k))
            {
                collide = true;
                Keys.Remove(k);
            }
            Keys.Add(k, key);

            return collide;
        }

        internal void ExtendTimeline(TimeSpan oldDuration, TimeSpan newDuration)
        {
            double factor = oldDuration.TotalSeconds / newDuration.TotalSeconds;

            SortedList<int, Key> newKeys = new SortedList<int, Key>();

            foreach (Key key in Keys.Values)
            {
                key.Time = key.Time * factor;
                if (!newKeys.ContainsKey(Quant(key.Time)) && key.Time <= 1)
                {
                    newKeys.Add(Quant(key.Time), key);
                }
            }
            Keys = newKeys;
        }

        internal bool AddKey(Key newKey)
        {
            bool collide = false;

            int k = Quant(newKey.Time);

            if (Keys.ContainsKey(k))
            {
                collide = true;
                Keys.Remove(k);
            }
            Keys.Add(k, newKey);

            return collide;
        }
    }
}
