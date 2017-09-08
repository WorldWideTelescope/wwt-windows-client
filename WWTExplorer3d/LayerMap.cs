using System.Collections.Generic;

namespace TerraViewer
{
    public class LayerMap
    {
        public LayerMap(string name, ReferenceFrames reference)
        {
            Name = name;
            Frame.Reference = reference;
            double radius = 6371000;

            switch (reference)
            {
                case ReferenceFrames.Sky:
                    break;
                case ReferenceFrames.Ecliptic:
                    break;
                case ReferenceFrames.Galactic:
                    break;
                case ReferenceFrames.Sun:
                    radius = 696000000;
                    break;
                case ReferenceFrames.Mercury:
                    radius = 2439700;
                    break;
                case ReferenceFrames.Venus:
                    radius = 6051800;
                    break;
                case ReferenceFrames.Earth:
                    radius = 6371000;
                    break;
                case ReferenceFrames.Mars:
                    radius = 3390000;
                    break;
                case ReferenceFrames.Jupiter:
                    radius = 69911000;
                    break;
                case ReferenceFrames.Saturn:
                    radius = 58232000;
                    break;
                case ReferenceFrames.Uranus:
                    radius = 25362000;
                    break;
                case ReferenceFrames.Neptune:
                    radius = 24622000;
                    break;
                case ReferenceFrames.Pluto:
                    radius = 1161000;
                    break;
                case ReferenceFrames.Moon:
                    radius = 1737100;
                    break;
                case ReferenceFrames.Io:
                    radius = 1821500;
                    break;
                case ReferenceFrames.Europa:
                    radius = 1561000;
                    break;
                case ReferenceFrames.Ganymede:
                    radius = 2631200;
                    break;
                case ReferenceFrames.Callisto:
                    radius = 2410300;
                    break;
                case ReferenceFrames.Custom:
                    Frame.SystemGenerated = false;
                    break;
                case ReferenceFrames.Identity:
                    break;
                case ReferenceFrames.Sandbox:
                    radius = 1;
                    break;
                default:
                    break;
            }
            Frame.MeanRadius = radius;

        }
        public Dictionary<string, LayerMap> ChildMaps = new Dictionary<string, LayerMap>();
        public void AddChild(LayerMap child)
        {
            child.Parent = this;
            ChildMaps.Add(child.Name, child);
        }

        public LayerMap Parent = null;
        public List<Layer> Layers = new List<Layer>();
        public bool Open = false;
        public bool Enabled = true;
        public bool LoadedFromTour = false;
        public string Name
        {
            get { return Frame.Name; }
            set { Frame.Name = value; }
        }


        public ReferenceFrame Frame = new ReferenceFrame();
        public void ComputeFrame(RenderContext11 renderContext)
        {
            if (Frame.Reference == ReferenceFrames.Custom | Frame.Reference == ReferenceFrames.Sandbox)
            {
                Frame.ComputeFrame(renderContext);
            }
        }

        public override string ToString()
        {
            return Name;
        }


    }
}
