using System;
using System.Collections.Generic;
using System.Text;
#if WINDOWS_UWP
using VoTable= System.Object;
using XmlElement = Windows.Data.Xml.Dom.XmlElement;
#else
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Drawing;
using System.Xml;
#endif

// Written by Jonathan Fay
// Next Media Research
// Copyright Microsoft Corp


namespace TerraViewer
{
    /// <summary>
    /// Summary description for ImageSet.
    /// </summary>
    public class ImageSetHelper : TerraViewer.IImageSet
    {
        ProjectionType projection;

        public ProjectionType Projection
        {
            get { return projection; }
            set { projection = value; }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private object wcsImage;

        public object WcsImage
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

        int hash = 0;

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
        double centerX = 0;

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
        double centerY = 0;

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
        double rotation = 0;

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
        bool singleImage = false;

        public bool SingleImage
        {
            get { return singleImage; }
            set { singleImage = value; }
        }


        public static ImageSetHelper FromXMLNode(XmlNode node)
        {
            try
            {
                ImageSetType type = ImageSetType.Sky;

                ProjectionType projection = ProjectionType.Tangent;
                if (node.Attributes["DataSetType"] != null)
                {
                    type = (ImageSetType)Enum.Parse(typeof(ImageSetType), node.Attributes["DataSetType"].Value.ToString(), true);
                }

                BandPass bandPass = BandPass.Visible;

                if (node.Attributes["BandPass"] != null)
                {
                    bandPass = (BandPass) Enum.Parse(typeof(BandPass),node.Attributes["BandPass"].Value.ToString());
                }
                int wf = 1;
                if (node.Attributes["WidthFactor"] != null)
                {
                    wf = Convert.ToInt32(node.Attributes["WidthFactor"].Value);
                }

                if (node.Attributes["Generic"] == null || !Convert.ToBoolean(node.Attributes["Generic"].Value.ToString()))
                {

                    switch (node.Attributes["Projection"].Value.ToString().ToLower())
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

                    string fileType = node.Attributes["FileType"].Value.ToString();
                    if (!fileType.StartsWith("."))
                    {
                        fileType = "." + fileType;
                    }

                    string thumbnailUrl;

                    XmlNode thumbUrl = node["ThumbnailUrl"];
                    thumbnailUrl = thumbUrl.InnerText;

                    bool stockSet = false;
                    bool elevationModel = false;

                    if (node.Attributes["StockSet"] != null)
                    {
                        stockSet = Convert.ToBoolean(node.Attributes["StockSet"].Value.ToString());
                    }

                    if (node.Attributes["ElevationModel"] != null)
                    {
                        elevationModel = Convert.ToBoolean(node.Attributes["ElevationModel"].Value.ToString());
                    }

                    string demUrl = "";
                    if (node.Attributes["DemUrl"] != null)
                    {
                        demUrl = node.Attributes["DemUrl"].Value.ToString();
                    }

                    string alturl = "";

                    if (node.Attributes["AltUrl"] != null)
                    {
                        alturl = node.Attributes["AltUrl"].Value.ToString();
                    }


                    double offsetX = 0;

                    if (node.Attributes["OffsetX"] != null)
                    {
                        offsetX = Convert.ToDouble(node.Attributes["OffsetX"].Value.ToString());
                    }
          
                    double offsetY = 0;

                    if (node.Attributes["OffsetY"] != null)
                    {
                        offsetY = Convert.ToDouble(node.Attributes["OffsetY"].Value.ToString());
                    }

                    string creditText = "";

                    XmlNode credits = node["Credits"];

                    if (credits != null)
                    {
                        creditText = credits.InnerText;
                    }

                    string creditsUrl = "";

                    credits = node["CreditsUrl"];

                    if (credits != null)
                    {
                        creditsUrl = credits.InnerText;
                    }

                    double meanRadius = 0;

                    if (node.Attributes["MeanRadius"] != null)
                    {
                        meanRadius = Convert.ToDouble(node.Attributes["MeanRadius"].Value.ToString());
                    }
                    string referenceFrame = null;
                    if (node.Attributes["ReferenceFrame"] != null)
                    {
                        referenceFrame = node.Attributes["ReferenceFrame"].Value;
                    }


                    return new ImageSetHelper(node.Attributes["Name"].Value.ToString(), node.Attributes["Url"].Value.ToString(), type, bandPass, projection, Math.Abs(node.Attributes["Url"].Value.GetHashCode32()), Convert.ToInt32(node.Attributes["BaseTileLevel"].Value), Convert.ToInt32(node.Attributes["TileLevels"].Value), 256, Convert.ToDouble(node.Attributes["BaseDegreesPerTile"].Value), fileType, Convert.ToBoolean(node.Attributes["BottomsUp"].Value.ToString()), node.Attributes["QuadTreeMap"].Value.ToString(), Convert.ToDouble(node.Attributes["CenterX"].Value), Convert.ToDouble(node.Attributes["CenterY"].Value), Convert.ToDouble(node.Attributes["Rotation"].Value), Convert.ToBoolean(node.Attributes["Sparse"].Value.ToString()), thumbnailUrl, stockSet, elevationModel, wf, offsetX, offsetY, creditText, creditsUrl, demUrl, alturl, meanRadius, referenceFrame);
                }
                else
                {
                    return new ImageSetHelper(type, bandPass);
                }

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
            else
            {
                return name.Replace("Visible Imagery", "Mars Visible Imagery");
            }
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
                else
                {
                    return new ImageSetHelper(this.DataSetType, this.BandPass);
                }
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
            IImageSet b = (IImageSet)obj;

            return (b.GetHash() == this.GetHash() && b.DataSetType == this.DataSetType && b.BandPass == this.BandPass && b.Generic == this.Generic );
            
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
        bool matrixComputed = false;
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
        private bool sparse = false;

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
            this.name = "Generic";
            this.sparse = false;
            this.dataSetType = dataSetType;

            this.bandPass = bandPass;
            this.quadTreeTileMap = "";
            this.url = "";
            this.levels = 0;
            this.baseTileDegrees = 0;
            this.imageSetID = 0;
            this.extension = "";
            this.projection = ProjectionType.Equirectangular;
            this.bottomsUp = false;
            this.baseLevel = 0;
            this.mercator = (projection == ProjectionType.Mercator);
            this.centerX = 0;
            this.centerY = 0;
            this.rotation = 0;
            //todo add scale
            this.thumbnailUrl = "";

            matrix = Matrix3d.Identity;
            matrix.Multiply(Matrix3d.RotationX((((Rotation)) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationZ(((CenterY) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationY((((360 - CenterX) + 180) / 180f * Math.PI)));

            RenderEngine.AddImageSetToTable(this.GetHash(), this);

        }

        bool defaultSet = false;
        bool elevationModel = false;

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

        double offsetX = 0;

        public double OffsetX
        {
            get { return offsetX; }
            set { offsetX = value; }
        }


        double offsetY = 0;

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

        bool isMandelbrot = false;
        bool mandelChecked = false;
        public bool IsMandelbrot
        {
            get
            {
                if (!mandelChecked)
                {
                    isMandelbrot = this.url.ToLower().Contains("mandel.aspx");
                }
                return isMandelbrot;
            }
        }

        private Dictionary<string, string> properties = new Dictionary<string, string>();

        public Dictionary<string, string> Properties
        {
            get
            {
                return properties;
            }
            set
            {
                properties = value;
            }
        }

        public ImageSetHelper(string name, string url, ImageSetType dataSetType, BandPass bandPass, ProjectionType projection, int imageSetID, int baseLevel, int levels, int tileSize, double baseTileDegrees, string extension, bool bottomsUp, string quadTreeMap, double centerX, double centerY, double rotation, bool sparse, string thumbnailUrl, bool defaultSet, bool elevationModel, int wf, double offsetX, double offsetY, string credits, string creditsUrl, string demUrlIn, string alturl, double meanRadius, string referenceFrame)
        {
            this.ReferenceFrame = referenceFrame;
            this.MeanRadius = meanRadius;
            this.altUrl = alturl;
            this.demUrl = demUrlIn;
            this.creditsText = credits;
            this.creditsUrl = creditsUrl;
            this.offsetY = offsetY;
            this.offsetX = offsetX;
            this.widthFactor = wf;
            this.elevationModel = elevationModel;
            this.defaultSet = defaultSet;
            this.name = name;
            this.sparse = sparse;
            this.dataSetType = dataSetType;

            this.bandPass = bandPass;
            this.quadTreeTileMap = quadTreeMap;
            this.url = url;
            this.levels = levels;
            this.baseTileDegrees = baseTileDegrees;
            this.imageSetID = imageSetID;
            this.extension = extension;
            this.projection = projection;
            this.bottomsUp = bottomsUp;
            this.baseLevel = baseLevel;
            this.mercator = (projection == ProjectionType.Mercator);
            this.centerX = centerX;
            this.centerY = centerY;
            this.rotation = rotation;
            this.thumbnailUrl = thumbnailUrl;

            ComputeMatrix();
            //if (Earth3d.multiMonClient)
            {
                RenderEngine.AddImageSetToTable(this.GetHash(), this);
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
            StringBuilder sb = new StringBuilder();
            sb.Append(imageset.ImageSetID.ToString());
            sb.Append(@"\");
            sb.Append(level.ToString());
            sb.Append(@"\");
            sb.Append(y.ToString());
            sb.Append("_");
            sb.Append(x.ToString());

            return sb.ToString();
        }

        public static long GetTileKey(IImageSet imageset, int level, int x, int y, Tile parent)
        {
            #if !WINDOWS_UWP
            if (parent != null)
            {
                if (imageset.Projection.Equals(ProjectionType.Healpix))
                {
                    HealpixTile tile = (HealpixTile)parent;
                    int tileIndex;
                    if (tile.tileIndex == -1)
                    {
                        tileIndex = y * 2 + x;
                    }
                    else
                    {

                        tileIndex = tile.tileIndex * 4 + y * 2 + x;
                    }
                    return (long)imageset.InternalID + ((long)level << 16) + ((long)x << 21) + ((long)y << 42) + ((long)(tileIndex * 4 + y * 2 + x) << 50) + tile.Key;
                    //return 0L;
                }
                else
                {
                    return (long)imageset.InternalID + ((long)level << 16) + ((long)x << 21) + ((long)y << 42);
                }
            }
            else
            {
                if (imageset.Projection.Equals(ProjectionType.Healpix))
                {
                    return (long)imageset.InternalID + ((long)level << 16) + ((long)x << 21) + ((long)y << 42) + ((long)(x * 4 + y) << 50);
                }
                else
                {
                    return (long)imageset.InternalID + ((long)level << 16) + ((long)x << 21) + ((long)y << 42);
                }
            }
#else
            return (long)imageset.InternalID + ((long)level << 16) + ((long)x << 21) + ((long)y << 42);
#endif
        }


        public static Tile GetNewTile(IImageSet imageset, int level, int x, int y, Tile parent)
        {

            switch (imageset.Projection)
            {
                case ProjectionType.Mercator:
                    {
                        MercatorTile newTile = new MercatorTile(level, x, y, imageset, parent);
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
#if !WINDOWS_UWP
                case ProjectionType.Plotted:
                    {
                        return new PlotTile(level, x, y, imageset, parent);
                    }
                case ProjectionType.Healpix:
                    {
                        return new HealpixTile(level, x, y, imageset, parent);
                    }
#endif
                default:
                case ProjectionType.Tangent:
                    {
                        TangentTile newTile = new TangentTile(level, x, y, imageset, parent);
                        return newTile;
                    }
            }
        }

        static Dictionary<int, ushort> ImageIdMap = new Dictionary<int,ushort>();

        static ushort nextID = 1;
        public static ushort NextInternalID(string Url)
        {
            int id = Math.Abs(Url.GetHashCode32());

            if (!ImageIdMap.ContainsKey(id))
            {
                ImageIdMap[id] = nextID++;
            }

            return ImageIdMap[id];
            
        }

        ushort internalID = 0;



        public ushort InternalID
        {
            get
            {
                if (internalID == 0)
                {
                    internalID = ImageSetHelper.NextInternalID(this.Url);
                }
                return internalID;
            }
            set
            {
                internalID = value;
            }
        }

        private VoTable tableMetadata = null;

        public VoTable TableMetadata
        { 
            get
            {
                return tableMetadata;
            }
            set
            {
                tableMetadata = value;
            }
        }
    }
}
