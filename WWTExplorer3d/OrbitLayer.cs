using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace TerraViewer
{
    public class OrbitLayer : Layer
    {
        List<ReferenceFrame> frames = new List<ReferenceFrame>();

        public List<ReferenceFrame> Frames
        {
            get { return frames; }
            set { frames = value; }
        }

        OrbitLayerUI primaryUI = null;
        public override LayerUI GetPrimaryUI()
        {
            if (primaryUI == null)
            {
                primaryUI = new OrbitLayerUI(this);
            }

            return primaryUI;
        }

        public OrbitLayer()
        {
        }

        public override void CleanUp()
        {
            foreach (ReferenceFrame frame in frames)
            {
                if (frame.Orbit != null)
                {
                    frame.Orbit.CleanUp();
                    frame.Orbit = null;
                }
            }
        }

        public override void WriteLayerProperties(System.Xml.XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("PointOpacity", PointOpacity.ToString());
            xmlWriter.WriteAttributeString("PointColor", SavedColor.Save(pointColor));

        }

        private double pointOpacity = 1;

        [LayerProperty]
        public double PointOpacity
        {
            get { return pointOpacity; }
            set
            {
                if (pointOpacity != value)
                {
                    version++;

                    pointOpacity = value;

                }
            }
        }

        Color pointColor = Color.Yellow;

        [LayerProperty]
        public Color PointColor
        {
            get { return pointColor; }
            set
            {
                if (pointColor != value)
                {
                    version++;
                    pointColor = value;

                }
            }
        }

        public override double[] GetParams()
        {
            double[] paramList = new double[6];
            paramList[0] = pointOpacity;
            paramList[1] = Color.R / 255;
            paramList[2] = Color.G / 255;
            paramList[3] = Color.B / 255;
            paramList[4] = Color.A / 255;
            paramList[5] = Opacity;


            return paramList;
        }

        public override string[] GetParamNames()
        {
            return new string[] { "PointOpacity", "Color.Red", "Color.Green", "Color.Blue", "Color.Alpha", "Opacity" };
        }

        public override BaseTweenType[] GetParamTypes()
        {
            return new BaseTweenType[] { BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear };
        }

        public override void SetParams(double[] paramList)
        {
            if (paramList.Length == 6)
            {
                pointOpacity = paramList[0];
                Opacity = (float)paramList[5];
                Color color = Color.FromArgb((int)(paramList[4] * 255), (int)(paramList[1] * 255), (int)(paramList[2] * 255), (int)(paramList[3] * 255));
                Color = color;
                
            }
        }


        public override void InitializeFromXml(System.Xml.XmlNode node)
        {
            PointOpacity = double.Parse(node.Attributes["PointOpacity"].Value);
            PointColor = SavedColor.Load(node.Attributes["PointColor"].Value);
            
        }


        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {
            Matrix3d matSaved = renderContext.World;
            renderContext.World = renderContext.WorldBaseNonRotating;

            foreach (ReferenceFrame frame in frames)
            {
                if (frame.ShowOrbitPath)
                {
                    if (frame.Orbit == null)
                    {
                        frame.Orbit = new Orbit(frame.Elements, 360, this.Color, 1, (float)renderContext.NominalRadius);
                    }
                    frame.Orbit.Draw3D(renderContext, opacity * this.Opacity, new Vector3d(0, 0, 0));
                }
            }
            renderContext.World = matSaved;
            return true;
        }

        string filename = "";

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            string fName = filename;

            bool copy = true;

            string fileName = fc.TempDirectory + string.Format("{0}\\{1}.txt", fc.PackageID, this.ID.ToString());
            string path = fName.Substring(0, fName.LastIndexOf('\\') + 1);
            string path2 = fileName.Substring(0, fileName.LastIndexOf('\\') + 1);

            if (copy)
            {
                if (!Directory.Exists(path2))
                {
                    Directory.CreateDirectory(path2);
                }
                if (File.Exists(fName) && !File.Exists(fileName))
                {
                    File.Copy(fName, fileName);
                }


            }

            if (File.Exists(fileName))
            {
                fc.AddFile(fileName);
            }

        }

        public override void LoadData(string path)
        {
            filename = path;
            if (File.Exists(filename))
            {
                string[] data = File.ReadAllLines(path);
                frames.Clear();
                for (int i = 0; i < data.Length; i += 2)
                {
                    int line1 = i;
                    int line2 = i + 1;
                    if (data[i].Length > 0)
                    {
                        ReferenceFrame frame = new ReferenceFrame();
                        if (data[i].Substring(0, 1) != "1")
                        {
                            line1++;
                            line2++;
                            frame.Name = data[i].Trim();
                            i++;
                        }
                        else if (data[i].Substring(0, 1) == "1")
                        {
                            frame.Name = data[i].Substring(2, 5);
                        }
                        else
                        {
                            i -= 2;
                            continue;
                        }

                        frame.Reference = ReferenceFrames.Custom;
                        frame.Oblateness = 0;
                        frame.ShowOrbitPath = true;
                        frame.ShowAsPoint = true;
                        frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
                        frame.Scale = 1;
                        frame.SemiMajorAxisUnits = AltUnits.Meters;
                        frame.MeanRadius = 10;
                        frame.Oblateness = 0;
                        frame.FromTLE(data[line1], data[line2], 398600441800000);
                        frames.Add(frame);
                    }
                    else
                    {
                        i -= 1;
                    }
                }
            }
        }
    }

    public class OrbitLayerUI : LayerUI
    {
        OrbitLayer layer = null;
        bool opened = true;

        public OrbitLayerUI(OrbitLayer layer)
        {
            this.layer = layer;
        }
        IUIServicesCallbacks callbacks = null;

        public override void SetUICallbacks(IUIServicesCallbacks callbacks)
        {
            this.callbacks = callbacks;
        }
        public override bool HasTreeViewNodes
        {
            get
            {
                return true;
            }
        }

        public override List<LayerUITreeNode> GetTreeNodes()
        {
            List<LayerUITreeNode> nodes = new List<LayerUITreeNode>();
            foreach (ReferenceFrame frame in layer.Frames)
            {

                LayerUITreeNode node = new LayerUITreeNode();
                node.Name = frame.Name;


                node.Tag = frame;
                node.Checked = frame.ShowOrbitPath;
                node.NodeSelected += new LayerUITreeNodeSelectedDelegate(node_NodeSelected);
                node.NodeChecked += new LayerUITreeNodeCheckedDelegate(node_NodeChecked);
                nodes.Add(node);
            }
            return nodes;
        }

        void node_NodeChecked(LayerUITreeNode node, bool newState)
        {
            ReferenceFrame frame = (ReferenceFrame)node.Tag;

            if (frame != null)
            {
                frame.ShowOrbitPath = newState;
            }
        }



        void node_NodeSelected(LayerUITreeNode node)
        {
            if (callbacks != null)
            {
                ReferenceFrame frame = (ReferenceFrame)node.Tag;

                Dictionary<String, String> rowData = new Dictionary<string, string>();

                rowData.Add("Name", frame.Name);
                rowData.Add("SemiMajor Axis", frame.SemiMajorAxis.ToString());
                rowData.Add("SMA Units", frame.SemiMajorAxisUnits.ToString());
                rowData.Add("Inclination", frame.Inclination.ToString());
                rowData.Add("Eccentricity", frame.Eccentricity.ToString());
                rowData.Add("Long of Asc. Node", frame.LongitudeOfAscendingNode.ToString());
                rowData.Add("Argument Of Periapsis", frame.ArgumentOfPeriapsis.ToString());
                rowData.Add("Epoch", frame.Epoch.ToString());
                rowData.Add("Mean Daily Motion", frame.MeanDailyMotion.ToString());
                rowData.Add("Mean Anomoly at Epoch", frame.MeanAnomolyAtEpoch.ToString());
                callbacks.ShowRowData(rowData);
            }
        }

        public override List<LayerUIMenuItem> GetNodeContextMenu(LayerUITreeNode node)
        {
            return base.GetNodeContextMenu(node);
        }
    }
}
