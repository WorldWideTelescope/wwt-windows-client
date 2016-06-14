using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Xml.Serialization;
using System.Xml;

namespace TerraViewer.Callibration
{
    [Serializable]
    public class ProjectorEntry 
    {
        public ProjectorEntry()
        {
            ViewProjection = new Projection();
            ViewTransform = new Transform();
            ProjectorProjection = new Projection();
            ProjectorTransform = new Transform();
            SolvedProjection = new Projection();
            SolvedTransform = new Transform();
        }

        [XmlAttribute("Name")]
        public string Name = "";

        [XmlAttribute("ID")]
        public int ID = 0;
            
        [XmlAttribute("UseGrid")]
        public bool UseGrid = false;
      
        [XmlAttribute("Width")]
        public int Width = 1920;

        [XmlAttribute("Height")]
        public int Height = 1080;
                
        [XmlArray("BlendPolygon")]
        public List<BlendPoint> BlendPolygon = new List<BlendPoint>();

        [XmlArray("Neighbors")]
        public List<int> Neighbors = new List<int>();

        [XmlArray("Constraints")]
        public List<GroundTruthPoint> Constraints = new List<GroundTruthPoint>();

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public int SelectedBlendPoint = -1;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public int SelectedGroundTruth = -1;

        [XmlElement("ViewProjection")]
        public Projection ViewProjection;

        [XmlElement("ViewTransform")]
        public Transform ViewTransform;

        [XmlElement("ProjectorProjection")]
        public Projection ProjectorProjection;

        [XmlElement("ProjectorTransform")]
        public Transform ProjectorTransform;

        [XmlElement("SolvedProjection")]
        public Projection SolvedProjection;

        [XmlElement("SolvedTransform")]
        public Transform SolvedTransform;

        [XmlElement("WhiteBalance")]
        public WhiteBalance WhiteBalance = new WhiteBalance();
        
        public override string ToString()
        {
            return Name;
        }
    }

    [Serializable]  
    public class WhiteBalance
    {
        [XmlAttribute("Red")]
        public int Red=255;
        [XmlAttribute("Green")]
        public int Green=255;
        [XmlAttribute("Blue")]
        public int Blue=255;
    }

    [Serializable]  
    public class Projection
    {
        [XmlAttribute("Fov")]
        public double FOV = 75;

        [XmlAttribute("Aspect")]
        public double Aspect = 1.7777778;

        [XmlAttribute("XOffset")]
        public double XOffset = 0;

        [XmlAttribute("YOffset")]
        public double YOffset = 0;
     
        [XmlAttribute("RadialCenterX")]     
        public double RadialCenterX = 0;
        
        [XmlAttribute("RadialCenterY")]
        public double RadialCenterY = 0;
    
        [XmlAttribute("RadialAmountX")]
        public double RadialAmountX = 0;

        [XmlAttribute("RadialAmountY")]
        public double RadialAmountY = 0;

        public Projection Copy()
        {
            Projection proj = new Projection();
            proj.FOV = this.FOV;
            proj.Aspect = this.Aspect;
            proj.XOffset = this.XOffset;
            proj.YOffset = this.YOffset;
            proj.RadialAmountX = this.RadialAmountX;
            proj.RadialAmountY = this.RadialAmountY;
            proj.RadialCenterX = this.RadialCenterX;
            proj.RadialCenterY = this.RadialCenterY;

            return proj;
        }
    }

    [Serializable]
    public class Transform
    {
        [XmlAttribute("X")]
        public double X = 0;
        [XmlAttribute("Y")]
        public double Y = 0;
        [XmlAttribute("Z")]
        public double Z = 0;

        [XmlAttribute("Heading")]
        public double Heading = 0;
        [XmlAttribute("Pitch")]
        public double Pitch = 0;
        [XmlAttribute("Roll")]
        public double Roll = 0;

        public Transform Copy()
        {
            Transform tran = new Transform();

            tran.X = this.X;
            tran.Y = this.Y;
            tran.Z = this.Z;
            tran.Heading = this.Heading;
            tran.Pitch = this.Pitch;
            tran.Roll = this.Roll;

            return tran;
        }
    }


    [Serializable]  
    public class EdgePoint
    {
        [XmlElement("Left")]
        public GroundTruthPoint Left;
        [XmlElement("Right")]
        public GroundTruthPoint Right;
        [XmlAttribute("Blend")]
        public bool Blend = true;
        public override string ToString()
        {
            return string.Format("({0},{1}) ({2},{3}){4}", Left.X, Left.Y, Right.X, Right.Y, Blend ? "-B" : "");
        } 
    }

    public enum AxisTypes { Alt, Az, Both, Edge }
    [Serializable]
    public class GroundTruthPoint
    {
        public static int nextID = 0;


        [XmlAttribute("ID")]
        public int ID = nextID++;
        [XmlAttribute("AxisType")]
        public AxisTypes AxisType = AxisTypes.Both;
        [XmlAttribute("Alt")]
        public double Alt = 0;
        [XmlAttribute("Az")]
        public double Az = 0;
        [XmlAttribute("X")]
        public double X = 0;
        [XmlAttribute("Y")]
        public double Y = 0;
        [XmlAttribute("Weight")]
        public double Weight = 1;
        public override string ToString()
        {
            switch (AxisType)
            {
                case AxisTypes.Alt:
                     return string.Format("alt={2}, x={0},y={1}, weight={3}", X, Y, Alt, Weight);

                case AxisTypes.Az:
                    return string.Format("az={2}, x={0},y={1}, weight={3}", X, Y, Az, Weight);

                case AxisTypes.Both:
                    return string.Format("alt={2},az={3}, x={0},y={1}, weight={4}", X, Y, Alt, Az, Weight);

                default:
                    return string.Format("x={0},y={1},alt={2},az={3}, weight={2}, type={3}", X, Y, Alt, Az, AxisType.ToString());
            }
        }
    }

    [Serializable]
    public class BlendPoint
    {
        [XmlAttribute("X")]
        public double X = 0;
        [XmlAttribute("Y")]
        public double Y = 0;
        [XmlAttribute("Softness")]
        public double Softness = 0;
        public override string ToString()
        {
            return string.Format("x={0},y={1}", X, Y);
        }
      
    }
}
