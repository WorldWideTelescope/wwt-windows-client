using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace TerraViewer
{
    public class AnimationTarget
    {
        public enum AnimationTargetTypes { Overlay, Layer, ReferenceFrame, StockSkyOverlay, Setting, Camera };
        private IAnimatable target;

        private readonly TourStop owner;

        public AnimationTarget(TourStop owner)
        {
            this.owner = owner;
        }

        public IAnimatable Target
        {
            get
            {
                if (!string.IsNullOrEmpty(TargetID))
                {
                    try
                    {
                        switch (TargetType)
                        {
                            case AnimationTargetTypes.Overlay:
                                break;
                            case AnimationTargetTypes.Layer:
                                if (LayerManager.LayerList.ContainsKey(new Guid(TargetID)))
                                {
                                    target = LayerManager.LayerList[new Guid(TargetID)];
                                }
                                break;
                            case AnimationTargetTypes.ReferenceFrame:
                                if (LayerManager.AllMaps.ContainsKey(TargetID))
                                {
                                    target = LayerManager.AllMaps[TargetID].Frame;
                                }
                                break;
                            case AnimationTargetTypes.StockSkyOverlay:
                                target = owner.GetSettingAnimator(TargetID);
                                break;
                            case AnimationTargetTypes.Setting:
                                break;
                            case AnimationTargetTypes.Camera:
                                break;
                        }
                    }
                    catch
                    {
                    }
                }

                return target;
            }
            set { target = value; }
        }
        private string targetID = "";

        public string TargetID
        {
            get
            {
                if (String.IsNullOrEmpty(targetID) && target != null)
                {
                    targetID = target.GetIndentifier();
                }
                return targetID;
            }
            set { targetID = value; }
        }
        public AnimationTargetTypes TargetType = AnimationTargetTypes.Overlay;
        public List<String> ParameterNames = new List<string>();
        public List<BaseTweenType> ParameterTypes = new List<BaseTweenType>();
        public List<KeyGroup> KeyFrames = new List<KeyGroup>();
        public double[] CurrentParameters = new double[0];

        public bool Expanded = false;

        public void SetKeyFrame(double time, Key.KeyType keyIn)
        {
            if (Target == null)
            {
                return;
            }
            var qt = KeyGroup.Quant(time);

            CurrentParameters = Target.GetParams();
            
            ParameterNames.Clear();
            ParameterNames.AddRange(Target.GetParamNames());

            ParameterTypes.Clear();
            ParameterTypes.AddRange(Target.GetParamTypes());

            if (KeyFrames.Count != ParameterNames.Count)
            {
                KeyFrames.Clear();
                foreach (var t in ParameterNames)
                {
                    var kf = new KeyGroup {ParameterName = t};
                    KeyFrames.Add(kf);
                }
            }

            for (var i = 0; i < ParameterNames.Count; i++)
            {
                var key = new Key(time, CurrentParameters[i], keyIn, ParameterTypes[i]);
                if (KeyFrames[i].Keys.ContainsKey(qt))
                {
                    KeyFrames[i].Keys[qt].Value = CurrentParameters[i];
                }
                else
                {
                    KeyFrames[i].Keys.Add(qt, key);
                }
            }

        }

        public void SetKeyFrame(int index, double time, Key.KeyType keyIn)
        {
            if (Target == null)
            {
                return;
            }
            var qt = KeyGroup.Quant(time);

            CurrentParameters = Target.GetParams();
            ParameterNames.Clear();
            ParameterNames.AddRange(Target.GetParamNames());

            ParameterTypes.Clear();
            ParameterTypes.AddRange(Target.GetParamTypes());

            if (KeyFrames.Count != ParameterNames.Count)
            {
                KeyFrames.Clear();
                foreach (var name in ParameterNames)
                {
                    var kf = new KeyGroup {ParameterName = name};
                    KeyFrames.Add(kf);
                }
            }

            if (index < ParameterNames.Count)
            {
                var key = new Key(time, CurrentParameters[index], keyIn, ParameterTypes[index]);
                if (KeyFrames[index].Keys.ContainsKey(qt))
                {
                    KeyFrames[index].Keys[qt].Value = CurrentParameters[index];
                }
                else
                {
                    KeyFrames[index].Keys.Add(qt, key);
                }
            }

        }

        public void Tween(double tweenFactor)
        {
            if (Target == null)
            {
                return;
            }

            if (ParameterNames.Count != CurrentParameters.Length)
            {
                CurrentParameters = new double[ParameterNames.Count];
            }

            for (var i = 0; i < ParameterNames.Count; i++)
            {
                CurrentParameters[i] = KeyFrames[i].Tween(tweenFactor);
            }

            Target.SetParams(CurrentParameters);
        }

        public override string ToString()
        {
            if (Target != null)
            {
                return Target.GetName();
            }

            return base.ToString();
        }

        public void SaveToXml(XmlTextWriter xmlWriter)
        {
            if (Target != null)
            {
                TargetID = Target.GetIndentifier();
            }

            xmlWriter.WriteStartElement("KeyFrames");
            xmlWriter.WriteAttributeString("TargetID", TargetID);
            xmlWriter.WriteAttributeString("TargetType", TargetType.ToString());
            xmlWriter.WriteAttributeString("Expanded", Expanded.ToString());
            foreach (var keyFrames in KeyFrames)
            {
                keyFrames.SaveToXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();
        }

        public void SaveSelectedToXml(XmlTextWriter xmlWriter, Dictionary<string, VisibleKey> selectedKeys)
        {
            TargetID = Target.GetIndentifier();

            xmlWriter.WriteStartElement("KeyFrames");
            xmlWriter.WriteAttributeString("TargetID", TargetID);
            xmlWriter.WriteAttributeString("TargetType", TargetType.ToString());
            xmlWriter.WriteAttributeString("Expanded", Expanded.ToString());
            var index = 0;
            foreach (var keyFrames in KeyFrames)
            {
                keyFrames.SaveToXml(xmlWriter, selectedKeys, this, index);
                index++;
            }
            xmlWriter.WriteEndElement();
        }

        public void FromXml(XmlNode node)
        {
            TargetID = node.Attributes["TargetID"].Value;
            TargetType = (AnimationTargetTypes)Enum.Parse(typeof(AnimationTargetTypes), node.Attributes["TargetType"].Value, true);
            if (node.Attributes["Expanded"] != null)
            {
                Expanded = bool.Parse(node.Attributes["Expanded"].Value);
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                var kf = new KeyGroup();
                kf.FromXml(child);
                ParameterNames.Add(kf.ParameterName);
                KeyFrames.Add(kf);
            }
        }

        public void PasteFromXML(XmlNode node, bool atTime, double time)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                var kf = new KeyGroup();
                kf.FromXml(child);

                for (var i = 0; i < ParameterNames.Count; i++ )
                {
                    if (ParameterNames[i] == kf.ParameterName)
                    {
                        foreach (var key in kf.Keys.Values)
                        {
                            if (atTime)
                            {
                                key.Time = time;
                            }

                            KeyFrames[i].AddKey(key);
                        }
                    }
                }
            }
        }

        internal void DeleteKey(int i, double time)
        {
            KeyFrames[i].DeleteKey(time);
        }
        
        internal void DeleteKey(int i, int time)
        {
            KeyFrames[i].DeleteKey(time);
        }

        public Key GetKey(int i, double time)
        {
            return KeyFrames[i].GetKey(time);
        }

        internal bool MoveKey(int index, double time, double newTime)
        {
            return KeyFrames[index].MoveKey(time, newTime);
        }

        internal void ExtendTimeline(TimeSpan oldDuration, TimeSpan newDuration)
        {
            for (var i = 0; i < ParameterNames.Count; i++)
            {
                KeyFrames[i].ExtendTimeline(oldDuration, newDuration);
            }
        }

        public AnimationTarget Clone(Overlay newTarget)
        {
            var sb = new StringBuilder();
            using (var textWriter = new StringWriter(sb))
            {
                using (var writer = new XmlTextWriter(textWriter))
                {
                    writer.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");

                    SaveToXml(writer);
                }
            }

            var doc = new XmlDocument();
            doc.LoadXml(sb.ToString());

            var at = new AnimationTarget(owner);
            XmlNode node = doc["KeyFrames"];
            at.FromXml(node);
            at.Target = newTarget;
            at.TargetID = newTarget.GetIndentifier();
            return at;
        }
    }
   
}
