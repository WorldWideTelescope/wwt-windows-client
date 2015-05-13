using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TerraViewer.Callibration
{
    [Serializable]
    public class Edge
    {
        [XmlAttribute("Left")]
        public int Left = 0;
        [XmlAttribute("Right")]
        public int Right = 0;
        [XmlArray("EdgePoints")]
        public List<EdgePoint> Points = new List<EdgePoint>();

        public Edge()
        {
        }
        public Edge(int left, int right)
        {
            Left = left;
            Right = right;
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public int selectedPoint = -1;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public CalibrationInfo Owner = null;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public int SelectedPoint
        {
            get
            {
                if (selectedPoint >= Points.Count)
                {
                    selectedPoint = -1;
                }
                return selectedPoint;
            }

            set { selectedPoint = value; }
        }

        public override string ToString()
        {
            return Owner.GetEdgeDisplayName(this);
        }

    }
}
