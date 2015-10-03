using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ShapefileTools
{    
    /// <summary>
    /// Represents the contents of shapefile header.
    /// File structure can be seen in Table 1 at page 4 of the ESRI documentation
    /// available at http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf
    /// </summary>
    public class FileHeader
    {
        // The main file header is 100 bytes long. 
        public const int headerLength = 100;
        // File code must be 9994 for shapefiles.
        public const int shapefileDefaultCode = 9994;
        private int fileCode;
        private int fileLength;
        private int fileVersion;
        private int shapeType;
        private double xMin;
        private double yMin;
        private double xMax;
        private double yMax;
        private double zMin;
        private double mMin;
        private double mMax;
        private double zMax;
        public Projection ProjectionInfo;
        public GeographicCoordinateSystem CoordinateReferenceSystem;



        public FileHeader()
        {
        }

        /// <summary>
        /// The value for file length is the total length of the file in 16-bit words (including the fifty
        /// 16-bit words that make up the header).
        /// </summary>
        public int FileLength
        {
            get { return this.fileLength; }
            set { this.fileLength = value; }
        }

        /// <summary>
        /// Specifies the version of the shapefile.
        /// </summary>
        public int Version
        {
            get { return this.fileVersion; }
            set { this.fileVersion = value; }
        }

        /// <summary>
        /// Specifies the shape type for the file.
        /// All the non-Null shapes in a shapefile are required to be of the same shape type.
        /// </summary>
        public int ShapeType
        {
            get { return this.shapeType; }
            set { this.shapeType = value; }
        }

        /// <summary>
        /// The minimum x-position (longitude) of the bounding
        /// box for the shapefile (in decimal degrees).
        /// </summary>
        public double XMin
        {
            get { return this.xMin; }
            set { this.xMin = value; }
        }

        /// <summary>
        /// The minimum z-position within the shapefile.
        /// </summary>
        public double ZMin
        {
            get { return this.zMin; }
            set { this.zMin = value; }
        }

        /// <summary>
        /// The minimum m value within the shapefile.
        /// </summary>
        public double MMin
        {
            get { return this.mMin; }
            set { this.mMin = value; }
        }


        /// <summary>
        /// The maximum z-position within the shapefile.
        /// </summary>
        public double ZMax
        {
            get { return this.zMax; }
            set { this.zMax = value; }
        }

        /// <summary>
        /// The maximum m value within the shapefile.
        /// </summary>
        public double MMax
        {
            get { return this.mMax; }
            set { this.mMax = value; }
        }

        /// <summary>
        /// The minimum y-position (latitude) of the bounding
        /// box for the shapefile (in decimal degrees).
        /// </summary>
        public double YMin
        {
            get { return this.yMin; }
            set { this.yMin = value; }
        }

        /// <summary>
        /// The maximum x-position (longitude) of the bounding
        /// box for the shapefile (in decimal degrees).
        /// </summary>      
        public double XMax
        {
            get { return this.xMax; }
            set { this.xMax = value; }
        }

        /// <summary>
        /// Indicates the maximum y-position (latitude) of the bounding
        /// box for the shapefile (in decimal degrees).
        /// </summary>
        public double YMax
        {
            get { return this.yMax; }
            set { this.yMax = value; }
        }

        public int FileCode
        {
            get { return this.fileCode; }
            set { this.fileCode = value; }
        }

    }

    public class GeographicCoordinateSystem {
        public string Name;
        public RefDatum Datum;
        public PriMem PrimeMeridian;
        public Unit Units;
    
    }

    public class RefDatum{
        public string Name;
        public RefSpheroid Spheroid;


    }

    public class RefSpheroid {
        public string Name;
        public double InverseFlatteningRatio;
        public double Axis;
 
    }

    public class PriMem {
        public string Name;
        public double Longitude;
    }

    public class Unit {
            public string Name;
            public double ConversionFactor;
    }

    public class Projection {
        public string Name;
        public Unit Units;
        public List<ProjectionParameter> Parameters;

        public double GetParameter(string name)
        {
            name = name.ToLower();
            foreach (var param in Parameters)
            {
                if (param.Name.ToLower() == name)
                {
                    return param.Value;
                }
            }
            return 0;
        }
    }

    public class ProjectionParameter {
        public string Name;
        public double Value;

        public ProjectionParameter() { }

        public ProjectionParameter(string name, double val) {
            this.Name = name;
            this.Value = val;
        
        }
    
    }
}
