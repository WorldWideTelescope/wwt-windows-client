// Copyright Microsoft Copr 2006
// Written by Jonathan Fay

using System.Collections.Generic;

namespace TerraViewer
{
    /// <summary>
    /// Summary description for LineSet.
    /// </summary>

    public class Lineset
	{
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<Linepoint> Points;

		public Lineset(string name)
		{
			this.name = name;

            Points = new List<Linepoint>();
		}
         
		public void Add(double ra, double dec, PointType pointType, string name)
        {
            Points.Add(new Linepoint(ra, dec, pointType, name));
		}
	}
    public class Linepoint
    {
        public double RA;
        public double Dec;
        public PointType PointType;
        public string Name = null;

        public Linepoint(double ra, double dec, PointType type, string name)
        {
            RA = ra;
            Dec = dec;
            PointType = type;
            Name = name;
        }
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return Coordinates.FormatDMS((((((RA )) / 360) * 24.0 + 12) % 24)) + ", " + Coordinates.FormatDMS(Dec) + ", " + PointType.ToString();
            }
            else
            {
                return Name + ", " + PointType.ToString();
            }
        }
    }
    public enum PointType { Move, Line, Dash, Start };
}
