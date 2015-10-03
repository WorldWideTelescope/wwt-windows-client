using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TerraViewer.Callibration
{
    [Serializable]
    [XmlRoot("Callibration")]
    public class CalibrationInfo
    {
        [XmlElement]
        public double DomeSize = 1;

        [XmlElement]
        public ScreenTypes ScreenType = ScreenTypes.Spherical;

        [XmlElement]
        public double DomeTilt = 0;

        [XmlElement]
        public double BlendGamma = 2.2;
    
        [XmlElement]
        public int BlendMarkBlurAmount = 1;
  
        [XmlElement]
        public int BlendMarkBlurIterations = 1;

        [XmlElement]
        public bool UseConstraints = true;

        [XmlElement]
        public bool SolveRadialDistortion = true;

        [XmlElement]
        public int SolveParameters = 255;

        [XmlArray("Projectors")]
        public List<ProjectorEntry> Projectors = new List<ProjectorEntry>();

        [XmlArray("Edges")]
        public List<Edge> Edges = new List<Edge>();


        [XmlIgnoreAttribute]
        public Dictionary<int, ProjectorEntry> ProjLookup = new Dictionary<int, ProjectorEntry>();

        public void SyncLookupsAndOwners()
        {
            ProjLookup.Clear();
            foreach(var pe in Projectors)
            {
                ProjLookup.Add(pe.ID, pe);
            }

            foreach (var edge in Edges)
            {
                edge.Owner = this;
            }
        }
        public void AddEdge(Edge edge)
        {
            Edges.Add(edge);
            edge.Owner = this;
        }

        public string GetEdgeDisplayName(Edge edge)
        {
            return string.Format("{0} - {1}", ProjLookup[edge.Left], ProjLookup[edge.Right]);
        }
    }
}
