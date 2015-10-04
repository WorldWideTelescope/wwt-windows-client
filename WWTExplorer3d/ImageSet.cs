using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

// Written by Jonathan Fay
// Next Media Research
// Copyright Microsoft Corp
using System.Xml.Serialization;


namespace TerraViewer
{
    /// <summary>
    /// Summary description for ImageSet.
    /// </summary>
    public class ImageSetHelper : IImageSet
    {
        ProjectionType projection;

        public ProjectionType Projection
        {
            get { return projection; }
            set { projection = value; }
        }
        [XmlIgnore]
        private WcsImage wcsImage;

        public WcsImage WcsImage
        {
            get { return wcsImage; }
            set { wcsImage = value; }
        }

        

        private string referenceFrame;

        public string ReferenceFrame
        {
            get
            {
                return referenceFrame;
            }
            set
            {
                referenceFrame = value;
            }
        }

        int imageSetID;
        public int ImageSetID
        {
            get
            {
                return imageSetID;
            }
            set
            {
                imageSetID = value;
            }
        }

        double baseTileDegrees;
        public double BaseTileDegrees
        {
            get
            {
                return baseTileDegrees;
            }
            set
            {
                baseTileDegrees = value;
            }
        }

        double widthFactor = 1;

        public double WidthFactor
        {
            get { return widthFactor; }
            set { widthFactor = value; }
        }

        int hash;

        public int GetHash()
        {
            if (hash == 0)
            {
                hash = Url.GetHashCode32();
            }
            return hash;
        }

        public override int GetHashCode()
        {
            return GetHash();
        }

        protected string url;
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
            }
        }

        protected string demUrl = "";
        public string DemUrl
        {
            get
            {
                if (String.IsNullOrEmpty(demUrl) && projection == ProjectionType.Mercator)
                {
                           
                    return "http://ecn.t{S}.tiles.virtualearth.net/tiles/d{Q}.elv?g=1&n=z";
                }
                return demUrl;
            }
            set
            {
                demUrl = value;
            }
        }

        string extension;
        public string Extension
        {
            get
            {
                return extension;
            }
            set
            {
                extension = value;
            }
        }

        int levels;
        public int Levels
        {
            get
            {
                return levels;
            }
            set
            {
                levels = value;
            }
        }
        bool mercator;
        bool bottomsUp;

        public bool BottomsUp
        {
            get { return bottomsUp; }
            set { bottomsUp = value; }
        }

        public bool Mercator
        {
            get { return mercator; }
            set { mercator = value; }
        }

        int baseLevel = 1;

        public int BaseLevel
        {
            get { return baseLevel; }
            set { baseLevel = value; }
        }

        string quadTreeTileMap = "0123";

        public string QuadTreeTileMap
        {
            get { return quadTreeTileMap; }
            set { quadTreeTileMap = value; }
        }
        double centerX;

        public double CenterX
        {
            get { return centerX; }
            set
            {
                if (centerX != value)
                {
                    centerX = value;
                    ComputeMatrix();
                }
            }
        }
        double centerY;

        public double CenterY
        {
            get { return centerY; }
            set
            {
                if (centerY != value)
                {
                    centerY = value;
                    ComputeMatrix();
                }
            }
        }
        double rotation;

        public double Rotation
        {
            get { return rotation; }
            set
            {
                if (rotation != value)
                {
                    rotation = value;
                    ComputeMatrix();
                }
            }
        }

        private double meanRadius;

        public double MeanRadius
        {
            get { return meanRadius; }
            set { meanRadius = value; }
        }


        ImageSetType dataSetType = ImageSetType.Earth;
        BandPass bandPass = BandPass.Visible;

        public BandPass BandPass
        {
            get { return bandPass; }
            set { bandPass = value; }
        }

        public ImageSetType DataSetType
        {
            get { return dataSetType; }
            set { dataSetType = value; }
        }

        string altUrl = "";

        public string AltUrl
        {
            get { return altUrl; }
            set { altUrl = value; }
        }
        bool singleImage;

        public bool SingleImage
        {
            get { return singleImage; }
            set { singleImage = value; }
        }


        public static ImageSetHelper FromXMLNode(XmlNode node)
        {
            try
            {
                var type = ImageSetType.Sky;



                var projection = ProjectionType.Tangent;
                if (node.Attributes["DataSetType"] != null)
                {
                    type = (ImageSetType)Enum.Parse(typeof(ImageSetType), node.Attributes["DataSetType"].Value, true);
                }

                var bandPass = BandPass.Visible;

                if (node.Attributes["BandPass"] != null)
                {
                    bandPass = (BandPass) Enum.Parse(typeof(BandPass),node.Attributes["BandPass"].Value);
                }
                var wf = 1;
                if (node.Attributes["WidthFactor"] != null)
                {
                    wf = Convert.ToInt32(node.Attributes["WidthFactor"].Value);
                }

                if (node.Attributes["Generic"] == null || !Convert.ToBoolean(node.Attributes["Generic"].Value))
                {

                    switch (node.Attributes["Projection"].Value.ToLower())
                    {
                        case "tan":
                        case "tangent":
                            projection = ProjectionType.Tangent;
                            break;
                        case "mercator":
                            projection = ProjectionType.Mercator;
                            break;
                        case "equirectangular":
                            projection = ProjectionType.Equirectangular;
                            break;
                        case "toast":
                            projection = ProjectionType.Toast;
                            break;
                        case "spherical":
                            projection = ProjectionType.Spherical;
                            break;
                        case "plotted":
                            projection = ProjectionType.Plotted;
                            break;
                        case "skyimage":
                            projection = ProjectionType.SkyImage;
                            break;
                    }

                    var fileType = node.Attributes["FileType"].Value;
                    if (!fileType.StartsWith("."))
                    {
                        fileType = "." + fileType;
                    }

                    string thumbnailUrl;

                    XmlNode thumbUrl = node["ThumbnailUrl"];
                    thumbnailUrl = thumbUrl.InnerText;

                    var stockSet = false;
                    var elevationModel = false;

                    if (node.Attributes["StockSet"] != null)
                    {
                        stockSet = Convert.ToBoolean(node.Attributes["StockSet"].Value);
                    }

                    if (node.Attributes["ElevationModel"] != null)
                    {
                        elevationModel = Convert.ToBoolean(node.Attributes["ElevationModel"].Value);
                    }

                    var demUrl = "";
                    if (node.Attributes["DemUrl"] != null)
                    {
                        demUrl = node.Attributes["DemUrl"].Value;
                    }

                    var alturl = "";

                    if (node.Attributes["AltUrl"] != null)
                    {
                        alturl = node.Attributes["AltUrl"].Value;
                    }


                    double offsetX = 0;

                    if (node.Attributes["OffsetX"] != null)
                    {
                        offsetX = Convert.ToDouble(node.Attributes["OffsetX"].Value);
                    }
          
                    double offsetY = 0;

                    if (node.Attributes["OffsetY"] != null)
                    {
                        offsetY = Convert.ToDouble(node.Attributes["OffsetY"].Value);
                    }

                    var creditText = "";

                    XmlNode credits = node["Credits"];

                    if (credits != null)
                    {
                        creditText = credits.InnerText;
                    }

                    var creditsUrl = "";

                    credits = node["CreditsUrl"];

                    if (credits != null)
                    {
                        creditsUrl = credits.InnerText;
                    }

                    double meanRadius = 0;

                    if (node.Attributes["MeanRadius"] != null)
                    {
                        meanRadius = Convert.ToDouble(node.Attributes["MeanRadius"].Value);
                    }
                    string referenceFrame = null;
                    if (node.Attributes["ReferenceFrame"] != null)
                    {
                        referenceFrame = node.Attributes["ReferenceFrame"].Value;
                    }


                    return new ImageSetHelper(node.Attributes["Name"].Value, node.Attributes["Url"].Value, type, bandPass, projection, Math.Abs(node.Attributes["Url"].Value.GetHashCode32()), Convert.ToInt32(node.Attributes["BaseTileLevel"].Value), Convert.ToInt32(node.Attributes["TileLevels"].Value), 256, Convert.ToDouble(node.Attributes["BaseDegreesPerTile"].Value), fileType, Convert.ToBoolean(node.Attributes["BottomsUp"].Value), node.Attributes["QuadTreeMap"].Value, Convert.ToDouble(node.Attributes["CenterX"].Value), Convert.ToDouble(node.Attributes["CenterY"].Value), Convert.ToDouble(node.Attributes["Rotation"].Value), Convert.ToBoolean(node.Attributes["Sparse"].Value), thumbnailUrl, stockSet, elevationModel, wf, offsetX, offsetY, creditText, creditsUrl, demUrl, alturl, meanRadius, referenceFrame);
                }
                return new ImageSetHelper(type, bandPass);
            }
            catch
            {
                return null;
            }
        }

        public static void SaveToXml(XmlTextWriter xmlWriter, IImageSet imageset, string alternateUrl)
        {
            xmlWriter.WriteStartElement("ImageSet");

            xmlWriter.WriteAttributeString("Generic", imageset.Generic.ToString());
            xmlWriter.WriteAttributeString("DataSetType", imageset.DataSetType.ToString());
            xmlWriter.WriteAttributeString("BandPass", imageset.BandPass.ToString());
            if (!imageset.Generic)
            {
                xmlWriter.WriteAttributeString("Name", imageset.Name);

                if (String.IsNullOrEmpty(alternateUrl))
                {
                    xmlWriter.WriteAttributeString("Url",  imageset.Url);
                }
                else
                {
                    xmlWriter.WriteAttributeString("Url",  alternateUrl);
                }
                xmlWriter.WriteAttributeString("DemUrl", imageset.DemUrl);
                xmlWriter.WriteAttributeString("BaseTileLevel", imageset.BaseLevel.ToString());
                xmlWriter.WriteAttributeString("TileLevels", imageset.Levels.ToString());
                xmlWriter.WriteAttributeString("BaseDegreesPerTile", imageset.BaseTileDegrees.ToString());
                xmlWriter.WriteAttributeString("FileType", imageset.Extension);
                xmlWriter.WriteAttributeString("BottomsUp", imageset.BottomsUp.ToString());
                xmlWriter.WriteAttributeString("Projection", imageset.Projection.ToString());
                xmlWriter.WriteAttributeString("QuadTreeMap", imageset.QuadTreeTileMap);
                xmlWriter.WriteAttributeString("CenterX", imageset.CenterX.ToString());
                xmlWriter.WriteAttributeString("CenterY", imageset.CenterY.ToString());
                xmlWriter.WriteAttributeString("OffsetX", imageset.OffsetX.ToString());
                xmlWriter.WriteAttributeString("OffsetY", imageset.OffsetY.ToString());
                xmlWriter.WriteAttributeString("Rotation", imageset.Rotation.ToString());
                xmlWriter.WriteAttributeString("Sparse", imageset.Sparse.ToString());
                xmlWriter.WriteAttributeString("ElevationModel", imageset.ElevationModel.ToString());
                xmlWriter.WriteAttributeString("StockSet", imageset.DefaultSet.ToString());
                xmlWriter.WriteAttributeString("WidthFactor", imageset.WidthFactor.ToString());
                xmlWriter.WriteAttributeString("MeanRadius", imageset.MeanRadius.ToString());
                xmlWriter.WriteAttributeString("ReferenceFrame", imageset.ReferenceFrame);
                if (String.IsNullOrEmpty(alternateUrl))
                {
                    xmlWriter.WriteElementString("ThumbnailUrl", imageset.ThumbnailUrl);
                }
                else
                {
                    xmlWriter.WriteElementString("ThumbnailUrl", imageset.Url);
                }
            }
            xmlWriter.WriteEndElement();
        }

        public override string ToString()
        {
            if (DefaultSet)
            {
                return name + " *";
            }
            return name.Replace("Visible Imagery", "Mars Visible Imagery");
        }

        //todo figure out the place for this...
        public IImageSet StockImageSet
        {
            get
            {
                if (generic || !defaultSet)
                {
                    return this;
                }
                return new ImageSetHelper(DataSetType, BandPass);
            }
        }




        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!(obj is IImageSet))
            {
                return false;
            }
            var b = (IImageSet)obj;

            return (b.GetHash() == GetHash() && b.DataSetType == DataSetType && b.BandPass == BandPass && b.Generic == Generic );
            
        }

        private Matrix3d matrix;

        public Matrix3d Matrix
        {
            get
            {
                if (!matrixComputed)
                {
                    ComputeMatrix();
                }
                return matrix;
            }
            set { matrix = value; }
        }
        bool matrixComputed;
        private void ComputeMatrix()
        {
            matrixComputed = true;
            matrix = Matrix3d.Identity;
            matrix.Multiply(Matrix3d.RotationX((((Rotation)) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationZ(((CenterY) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationY((((360 - CenterX) + 180) / 180f * Math.PI)));
        }

        private string name = "";

        public string Name
        {
            get
            {
                return name;
            }
            set { name = value; }
        }
        private bool sparse;

        public bool Sparse
        {
            get { return sparse; }
            set { sparse = value; }
        }
        private string thumbnailUrl = "";

        public string ThumbnailUrl
        {
            get { return thumbnailUrl; }
            set { thumbnailUrl = value; }
        }
        private bool generic;

        public bool Generic
        {
            get { return generic; }
            set { generic = value; }
        }
        public ImageSetHelper(ImageSetType dataSetType, BandPass bandPass)
        {
            generic = true;
            name = "Generic";
            sparse = false;
            this.dataSetType = dataSetType;

            this.bandPass = bandPass;
            quadTreeTileMap = "";
            url = "";
            levels = 0;
            baseTileDegrees = 0;
            imageSetID = 0;
            extension = "";
            projection = ProjectionType.Equirectangular;
            bottomsUp = false;
            baseLevel = 0;
            mercator = (projection == ProjectionType.Mercator);
            centerX = 0;
            centerY = 0;
            rotation = 0;
            //todo add scale
            thumbnailUrl = "";

            matrix = Matrix3d.Identity;
            matrix.Multiply(Matrix3d.RotationX((((Rotation)) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationZ(((CenterY) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationY((((360 - CenterX) + 180) / 180f * Math.PI)));

            Earth3d.AddImageSetToTable(GetHash(), this);

        }

        bool defaultSet;
        bool elevationModel;

        public bool ElevationModel
        {
            get { return elevationModel; }
            set { elevationModel = value; }
        }
        public bool DefaultSet
        {
            get { return defaultSet; }
            set { defaultSet = value; }
        }

        double offsetX;

        public double OffsetX
        {
            get { return offsetX; }
            set { offsetX = value; }
        }


        double offsetY;

        public double OffsetY
        {
            get { return offsetY; }
            set { offsetY = value; }
        }


        string creditsText;

        public string CreditsText
        {
            get { return creditsText; }
            set { creditsText = value; }
        }
        string creditsUrl;

        public string CreditsUrl
        {
            get { return creditsUrl; }
            set { creditsUrl = value; }
        }

        bool isMandelbrot;
        bool mandelChecked = false;
        public bool IsMandelbrot
        {
            get
            {
                if (!mandelChecked)
                {
                    isMandelbrot = url.ToLower().Contains("mandel.aspx");
                }
                return isMandelbrot;
            }
        }


        public ImageSetHelper(string name, string url, ImageSetType dataSetType, BandPass bandPass, ProjectionType projection, int imageSetID, int baseLevel, int levels, int tileSize, double baseTileDegrees, string extension, bool bottomsUp, string quadTreeMap, double centerX, double centerY, double rotation, bool sparse, string thumbnailUrl, bool defaultSet, bool elevationModel, int wf, double offsetX, double offsetY, string credits, string creditsUrl, string demUrlIn, string alturl, double meanRadius, string referenceFrame)
        {
            ReferenceFrame = referenceFrame;
            MeanRadius = meanRadius;
            altUrl = alturl;
            demUrl = demUrlIn;
            creditsText = credits;
            this.creditsUrl = creditsUrl;
            this.offsetY = offsetY;
            this.offsetX = offsetX;
            widthFactor = wf;
            this.elevationModel = elevationModel;
            this.defaultSet = defaultSet;
            this.name = name;
            this.sparse = sparse;
            this.dataSetType = dataSetType;

            this.bandPass = bandPass;
            quadTreeTileMap = quadTreeMap;
            this.url = url;
            this.levels = levels;
            this.baseTileDegrees = baseTileDegrees;
            this.imageSetID = imageSetID;
            this.extension = extension;
            this.projection = projection;
            this.bottomsUp = bottomsUp;
            this.baseLevel = baseLevel;
            mercator = (projection == ProjectionType.Mercator);
            this.centerX = centerX;
            this.centerY = centerY;
            this.rotation = rotation;
            this.thumbnailUrl = thumbnailUrl;




            ComputeMatrix();
            //if (Earth3d.multiMonClient)
            {
                Earth3d.AddImageSetToTable(GetHash(), this);
            }
        }



        // URL parameters
            //{0} ImageSetID
            //{1} level
            //{2} x tile id
            //{3} y tile id
            //{4} quadtree address (VE style)
            //{5} quadtree address (Google maps style)
            //{6} top left corner RA
            //{7} top left corner Dec
            //{8} bottom right corner RA
            //{9} bottom right corner dec
            //{10} bottom left corner RA
            //{11} bottom left corner dec
            //{12} top right corner RA
            //{13} top right corner dec



        public static string GetTileKeyString(IImageSet imageset, int level, int x, int y)
        {
            var sb = new StringBuilder();
            sb.Append(imageset.ImageSetID.ToString());
            sb.Append(@"\");
            sb.Append(level.ToString());
            sb.Append(@"\");
            sb.Append(y.ToString());
            sb.Append("_");
            sb.Append(x.ToString());

            return sb.ToString();
        }

        public static long GetTileKey(IImageSet imageset, int level, int x, int y)
        {
            return imageset.InternalID + ((long)level << 16) + ((long)x << 21) + ((long)y << 42);
        }


        public static Tile GetNewTile(IImageSet imageset, int level, int x, int y, Tile parent)
        {

            switch (imageset.Projection)
            {
                case ProjectionType.Mercator:
                    {
                        var newTile = new MercatorTile(level, x, y, imageset, parent);
                        return newTile;
                    }
                case ProjectionType.Equirectangular:
                    {
                        return new EquirectangularTile(level, x, y, imageset, parent);
                    }
                case ProjectionType.Spherical:
                    {
                        return new SphericalTile(level, x, y, imageset, parent);
                    }
                
                case ProjectionType.Toast:
                    {
                        return new ToastTile(level, x, y, imageset, parent);
                    }
                case ProjectionType.SkyImage:
                    {
                        return new SkyImageTile(level, x, y, imageset, parent);
                    }
                case ProjectionType.Plotted:
                    {
                        return new PlotTile(level, x, y, imageset, parent);
                    }
                default:
                case ProjectionType.Tangent:
                    {
                        var newTile = new TangentTile(level, x, y, imageset, parent);
                        return newTile;
                    }
            }
        }

        static readonly Dictionary<int, ushort> ImageIdMap = new Dictionary<int,ushort>();

        static ushort nextID = 1;
        public static ushort NextInternalID(string Url)
        {
            var id = Math.Abs(Url.GetHashCode32());

            if (!ImageIdMap.ContainsKey(id))
            {
                ImageIdMap[id] = nextID++;
            }

            return ImageIdMap[id];
            
        }

        ushort internalID;



        public ushort InternalID
        {
            get
            {
                if (internalID == 0)
                {
                    internalID = NextInternalID(Url);
                }
                return internalID;
            }
            set
            {
                internalID = value;
            }
        }

    }
}
