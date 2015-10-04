using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Drawing;
using TerraViewer.Properties;
using WWTThumbnails;


namespace TerraViewer
{
    public partial class Folder : IThumbnail
    {
        public override string ToString()
        {
            return nameField;
        }
        public static Folder LoadFromFile(string filename, bool versionDependent)
        {
            var reader = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            var folder = LoadFromFileStream(reader, versionDependent);

            folder.loadedFilename = filename;
            reader.Close();
            return folder;
           
        }
        [XmlIgnore]
        private bool versionDependent;

        public bool VersionDependent
        {
            get { return versionDependent; }
            set
            {
                versionDependent = value;
                foreach (var folder in Folder1)
                {
                    folder.VersionDependent = versionDependent;
                }
            }
        }

        public static Folder LoadFromUrl(string url, bool versionDependent)
        {
            var dir = Properties.Settings.Default.CahceDirectory + "Data\\WTMLCACHE\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var filename = dir + Math.Abs(url.GetHashCode32()) + ".wtml";
            DataSetManager.DownloadFile(url, filename, false, versionDependent);
            try
            {
                if (File.Exists(filename))
                {
                    var temp = LoadFromFile(filename, versionDependent);
                    return temp;
                }
            }
            catch
            {
            }
            return null;
        }
        
        bool readOnly = true;
        [XmlIgnore]
        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

        bool dirty;

        [XmlIgnore]
        public bool Dirty
        {
            get { return dirty; }
            set { dirty = value; }
        }
       
        private string loadedFilename = "";
       

        [XmlIgnore]
        public string LoadedFilename
        {
            get { return loadedFilename; }
            set { loadedFilename = value; }
        }


        public static Folder LoadFromFileStream(Stream stream, bool versionDependent)
        {
            var serializer = new XmlSerializer(typeof(Folder));
            var newFolder = (Folder)serializer.Deserialize(stream);
            newFolder.dirty = false;
            newFolder.VersionDependent = versionDependent;

            return newFolder;
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(loadedFilename))
            {
                SaveToFile(loadedFilename);
                dirty = false;
            }
            else
            {
                throw new Exception(Language.GetLocalizedText(218, "The folder was not loaded from a file"));
            }
        }

        public void SaveToFile(string filename)
        {
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (var writer = File.Open(filename,FileMode.Create,FileAccess.Write,FileShare.None))
            {
                var serializer = new XmlSerializer(typeof(Folder));
                serializer.Serialize(writer, this, ns);
                writer.Close();
            }            
        }

        public void AddChildFolder(Folder child)
        {
            folders.Add(child);
            dirty = true;
        }
        public void RemoveChild(Folder child)
        {
            folders.Remove(child);
            dirty = true;
        }

        public void AddChildPlace(Place child)
        {
            places.Add(child);
            dirty = true;
        }

        public void RemoveChild(Place child)
        {
            places.Remove(child);
            dirty = true;
        }

        public void RemoveChild(ImageSet child)
        {
            imagesets.Remove(child);
            
            dirty = true;
        }

        Bitmap thumbnail;
        [XmlIgnore]
        public Bitmap ThumbNail
        {
            get
            {
                if (thumbnail == null)
                {
                    try
                    {
                        thumbnail = UiTools.LoadThumbnailFromWeb(thumbnailField);
                    }
                    catch
                    {

                    }
                    if (thumbnail == null)
                    {
                        thumbnail = Resources.Folder;
                    }

                }
                return thumbnail;
            }
            set
            {
                if (thumbnail != null)
                {
                    thumbnail.Dispose();
                }
                thumbnail = value;
            }
        }

        Rectangle bounds;
        [XmlIgnore]
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }


        [XmlIgnore]
        public bool IsImage
        {
            get { return false; }
        }

        [XmlIgnore]
        public bool IsTour
        {
            get { return false; }
        }

        [XmlIgnore]
        public bool IsFolder
        {
            get { return true; }
        }

        [XmlIgnore]
        public bool IsCloudCommunityItem
        {
            get
            {
                return communityIdField != 0 || permissionField > 0;
            }
        }
        private Folder proxyFolder;

        public void Refresh()
        {
            var temp = LoadFromUrl(Earth3d.MainWindow.PrepareUrl(urlField), VersionDependent);
            if (temp != null)
            {
                proxyFolder = temp;
            }
        }


        private DateTime lastUpdate = DateTime.Now.Subtract(new TimeSpan(1));
        [XmlIgnore]
        public object[] Children
        {
            get
            {
                if (String.IsNullOrEmpty(urlField))
                {
                    var returnList = new List<object>();
                    if (Folder1 != null)
                    {
                        returnList.AddRange(Folder1);
                    }
                    if (ImageSet != null)
                    {
                        returnList.AddRange(ImageSet);
                    }
                    if (Items != null)
                    {
                        returnList.AddRange(Items);
                    }
                    if (Tour != null)
                    {
                        returnList.AddRange(Tour);
                    }

                    if (LineSet != null)
                    {
                        returnList.AddRange(LineSet);
                    }
                    return returnList.ToArray();
                }
                var ts = lastUpdate.Subtract(DateTime.Now);
                // TOdo add add Move Complete Auto Update
                // todo add URL formating for Ambient Parameters
                // TODO remove true when perth fixes refresh type on server
                if ( true  || RefreshType == FolderRefreshType.ConditionalGet || proxyFolder == null || 
                     (RefreshType == FolderRefreshType.Interval && (Convert.ToInt32(refreshIntervalField) < ts.TotalSeconds)))
                {
                    Refresh();
                }

                return proxyFolder != null ? proxyFolder.Children : null;
            }
        }
    }

    public partial class Tour : IThumbnail, ITourResult
    {
        Bitmap thumbnail;
        [XmlIgnore]
        public Bitmap ThumbNail
        {
            get
            {
                if (thumbnail == null)
                {
                    try
                    {
                        thumbnail =
                            UiTools.LoadThumbnailFromWeb(!String.IsNullOrEmpty(thumbnailUrlField)
                                ? thumbnailUrlField
                                : String.Format("http://www.worldwidetelescope.org/wwtweb/GetTourThumbnail.aspx?GUID={0}", idField));
                    }
                    catch
                    {


                    }

                }
                return thumbnail;
            }
            set
            {
                if (thumbnail != null)
                {
                    thumbnail.Dispose();
                }
                thumbnail = value;
            }
        }

        [XmlIgnore]   
        public bool IsCloudCommunityItem
        {
            get
            {
                return MSRComponentId != 0;
            }
        }

        Rectangle bounds;
        [XmlIgnore]
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }

        [XmlIgnore]
        public string Name
        {
            get { return titleField; }
        }

        [XmlIgnore]
        public bool IsImage
        {
            get { return false; }
        }

        [XmlIgnore]
        public bool IsTour
        {
            get { return true; }
        }

        [XmlIgnore]
        public bool IsFolder
        {
            get { return false; }
        }

        [XmlIgnore]
        public object[] Children
        {
            get
            {
                return null;
            }
        }

        public bool ReadOnly
        {
            get { return true; }
        }

        #region ITourResult Members

        [XmlIgnore]
        public string AttributesAndCredits
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        [XmlIgnore]
        public string AuthorContactText
        {
            get
            {
                return authorEmailField;
            }
            set
            {
               
            }
        }

        [XmlIgnore]
        public string AuthorEmailOther
        {
            get
            {
                return "";
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }
        Bitmap authorImageBitmap;
        [XmlIgnore]
        public Bitmap AuthorImage
        {
            get
            {
                if (authorImageBitmap == null)
                {
                    try
                    {
                        authorImageBitmap =
                            UiTools.LoadThumbnailFromWeb(!String.IsNullOrEmpty(authorImageUrlField)
                                ? authorImageUrlField
                                : String.Format("http://www.worldwidetelescope.org/wwtweb/GetAuthorThumbnail.aspx?GUID={0}", idField));
                    }
                    catch
                    {
                    }
                }
                return authorImageBitmap;

                
            }
            set
            {
                authorImageBitmap = value;
            }
        }


        [XmlIgnore]
        public double LengthInSeconds
        {
            get { return lengthInSecsField; }
            set { }
        }

        [XmlIgnore]
        public string AuthorUrl
        {
            get
            {
                return authorURLField;
            }
            set
            {
                authorURLField = value;
            }
        }

        [XmlIgnore]
        public double AverageUserRating
        {
            get
            {
                return averageRatingField;
            }
            set
            {
                averageRatingField = value;
            }
        }

        [XmlIgnore]
        public string Id
        {
            get
            {
                return idField;
            }
            set
            {
                idField = value;
            }
        }

        [XmlIgnore]
        public string OrgName
        {
            get
            {
                return organizationNameField;
            }
            set
            {
                organizationNameField = value;
            }
        }

        [XmlIgnore]
        public string OrgUrl
        {
            get
            {
                return organizationUrlField;
            }
            set
            {
                organizationUrlField = value;
            }
        }

        #endregion
    }

    public partial class Place : IPlace
    {
        public static Place FromIPlace(IPlace place)
        {
            var newPlace = new Place();
            if (place.BackgroundImageSet != null)
            {
                newPlace.backgroundImageSetField = new PlaceBackgroundImageSet
                {
                    ImageSet = ImageSet.FromIImage(place.BackgroundImageSet)
                };
            }
            if (place.StudyImageset != null)
            {
                newPlace.foregroundImageSetField = new PlaceForegroundImageSet
                {
                    ImageSet = ImageSet.FromIImage(place.StudyImageset)
                };
            }
            newPlace.CamParams = place.CamParams;

            var names="";
            var delim = "";
            foreach (var name in place.Names)
            {
                names += delim;
                names += name;
                delim = ";";
            }
            newPlace.Name = names;      
            newPlace.Classification = place.Classification;
            newPlace.classificationFieldSpecified = true;
            newPlace.Type  = place.Type;
            newPlace.Constellation = place.Constellation;
            newPlace.Magnitude = place.Magnitude;
            newPlace.Distance = place.Distance;
            newPlace.AngularSize = place.ZoomLevel;
            newPlace.anglularSizeFieldSpecified = true;
            newPlace.Url = place.Url;
            newPlace.Thumbnail = place.Thumbnail;
            newPlace.Target = place.Target;
            newPlace.Tag = place.Tag;
            if (place.Type == ImageSetType.Sky)
            {
                newPlace.raFieldSpecified = true;
                newPlace.decFieldSpecified = true;
            }
            else
            {
                newPlace.latFieldSpecified = true;
                newPlace.lngFieldSpecified = true;
            }
            return newPlace;
          
        }
        private SolarSystemObjects target = SolarSystemObjects.Undefined;

        [GeneratedCode("xsd", "2.0.50727.42")]
        public SolarSystemObjects Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
        private object tag;

        [XmlIgnore]
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        Tour tour;
        [XmlIgnore]
        public Tour Tour
        {
            get { return tour; }
            set { tour = value; }
        }
        #region IPlace Members

        private string url;

        [XmlAttribute]
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        [XmlIgnore]
        IImageSet IPlace.BackgroundImageSet
        {
            get
            {
                return backgroundImageSetField != null ? backgroundImageSetField.ImageSet : null;
            }
            set
            {
                if (backgroundImageSetField == null)
                {
                    backgroundImageSetField = new PlaceBackgroundImageSet();
                }
                backgroundImageSetField.ImageSet = ImageSet.FromIImage(value);
            }
        }


        [XmlIgnore]    
        public bool IsCloudCommunityItem
        {
            get
            {
                return MSRComponentId != 0;
            }
        }

        [XmlIgnore]
        public CameraParameters CamParams
        {
            get
            {
                var cam =  new CameraParameters(latField, lngField, zoomLevelField, rotationField, angleField, (float)opacityField);
                if (raFieldSpecified)
                {
                    cam.RA = RA;
                }
                if (decFieldSpecified)
                {
                    cam.Dec = Dec;
                }

                cam.DomeAlt = domeAltField;
                cam.DomeAz = domeAzField;

                return cam;
            }
            set
            {
                latField = value.Lat;
                lngField = value.Lng;
                zoomLevelField = value.Zoom;
                rotationField = value.Rotation;
                angleField = value.Angle;
                opacityField = value.Opacity;
                raField = value.RA;
                decField = value.Dec;
                domeAltField = value.DomeAlt;
                domeAzField = value.DomeAz;
            }
        }

        [XmlIgnore]
        double IPlace.Dec
        {
            get
            {
                if (Classification == Classification.SolarSystem)
                {
                    var raDec = Planets.GetPlanetLocation(Name);
                    //if (raDec != null)
                    {
                        raField = raDec.RA;
                        decField = raDec.Dec;
                    }
                }
                return decField;
            }
            set
            {
                decField = value;
                decFieldSpecified = true;
            }
        }

        [XmlIgnore]
        double IPlace.Lat
        {
            get
            {
                return latField;
            }
            set
            {
                latField = value;
            }
        }

        [XmlIgnore]
        double IPlace.Lng
        {
            get
            {
                return lngField;
            }
            set
            {
                lngField = value;
            }
        }

        Vector3d location3d = new Vector3d(0, 0, 0);

        [XmlIgnore]
        public Vector3d Location3d
        {
            get
            {
                if (Type == ImageSetType.Planet || Type == ImageSetType.Earth)
                {
                    if ((location3d.X == 0 && location3d.Y == 0 && location3d.Z == 0))
                    {
                        location3d = Coordinates.RADecTo3d(Lng/15,Lat, 1);
                    }

                }
                else if (Classification == Classification.SolarSystem || (location3d.X == 0 && location3d.Y == 0 && location3d.Z == 0))
                {
                    location3d = Coordinates.RADecTo3d(RA, Dec, 1);
                }
                return location3d;
            }
        }

        [XmlIgnore]
        public string[] Names
        {
            get
            {
                if (string.IsNullOrEmpty(nameField))
                {
                    return ("").Split(new[] { ';' });
                }
                return nameField.Split(new[] { ';' });
            }
            set
            {
                nameField = UiTools.GetNamesStringFromArray(value);
            }
        }


        [XmlIgnore]
        double IPlace.RA
        {
            get
            {
                if (Classification == Classification.SolarSystem)
                {
                    var raDec = Planets.GetPlanetLocation(Name);

                    raField = raDec.RA;
                    decField = raDec.Dec;
                    distanceField = raDec.Distance;
                }
                return raField;
            }
            set
            {
                raField = value;
                raFieldSpecified = true;
            }
        }

        [XmlIgnore]
        double IPlace.ZoomLevel
        {
            get
            {
                return zoomLevelField;
            }
            set
            {
                zoomLevelField = value;
            }
        }

        #endregion

        #region IThumbnail Members

        Bitmap thumbNail;

        [XmlIgnore]
        public Bitmap ThumbNail
        {
            get
            {
                if (thumbNail == null)
                {
                    if (foregroundImageSetField != null)
                    {
                        if (foregroundImageSetField.ImageSet != null)
                        {
                            thumbNail = UiTools.LoadThumbnailFromWeb(foregroundImageSetField.ImageSet.ThumbnailUrl);
                            if (thumbNail != null)
                            {
                                return thumbNail;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(thumbnailField))
                    {
                        thumbNail = UiTools.LoadThumbnailFromWeb(thumbnailField);
                        if (thumbNail != null)
                        {
                            return thumbNail;
                        }             
                    }
                    if (Classification == Classification.Constellation)
                    {
                        if (!String.IsNullOrEmpty(constellationField))
                        {
                            if (Overview.ConstellationThumbnails.ContainsKey(constellationField))
                            {
                                //todo clone this
                                thumbNail = Overview.ConstellationThumbnails[constellationField];
                            }
                        }
                    }
                    else
                    {
                        thumbNail = WWTThmbnail.GetThumbnail(Name.Replace(" ", ""));
                        if (thumbNail == null)
                        {
                            var obj = Resources.ResourceManager.GetObject(Enum.GetName(typeof(Classification), Classification), Resources.Culture);
                            thumbNail = ((Bitmap)(obj));
                        }
                    }
                }

                return thumbNail;
            }
            set
            {
                if (thumbNail != null && Classification != Classification.Constellation)
                {
                    thumbNail.Dispose();
                }
                thumbNail = value;
            }
        }
        Rectangle bounds;

        [XmlIgnore]
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }
        [XmlIgnore]
        public bool ReadOnly
        {
            get { return true; }
        }
        [XmlIgnore]
        public bool IsImage
        {
            get
            {
                return (foregroundImageSetField != null && foregroundImageSetField.ImageSet != null)
                    || (backgroundImageSetField != null && backgroundImageSetField.ImageSet != null);
            }

        }

        [XmlIgnore]
        public bool IsTour
        {
            get { return tour != null; }
        }

        [XmlIgnore]
        public bool IsFolder
        {
            get { return false; }
        }

        [XmlIgnore]
        public object[] Children
        {
            get { return null; }
        }

        #endregion

        #region IPlace Members


        private double searchDistance;

        [XmlIgnore]
        public double SearchDistance
        {
            get { return searchDistance; }
            set { searchDistance = value; }
        }

        #endregion

        #region IPlace Members


        [XmlIgnore]
        Classification IPlace.Classification
        {
            get
            {
                return classificationField;
            }
            set
            {
                classificationField = value;
            }
        }

        [XmlIgnore]
        public ImageSetType Type
        {
            get
            {
                return dataSetTypeField;
            }
            set
            {
                dataSetTypeField = value;
            }
        }

        #endregion

        #region IPlace Members

        [XmlIgnore]
        public IImageSet StudyImageset
        {
            get
            {
                if (foregroundImageSetField != null)
                {
                    return foregroundImageSetField.ImageSet;
                }
                return null;
            }
            set
            {
                if (foregroundImageSetField == null)
                {
                    foregroundImageSetField = new PlaceForegroundImageSet();
                }

                foregroundImageSetField.ImageSet = ImageSet.FromIImage(value);
            }
        }

        #endregion

        #region IPlace Members



        #endregion
    }
    public partial class ImageSet : IImageSet , IThumbnail
    {
        [XmlIgnore]
        private WcsImage wcsImage;

        [XmlIgnore]
        public WcsImage WcsImage
        {
            get { return wcsImage; }
            set { wcsImage = value; }
        }

        static ushort nextID;
       
        private static ushort NextInternalID()
        {
            return nextID++;
        }

        ushort internalID;

        [XmlIgnore]
        public ushort InternalID
        {
            get
            {
                if (internalID == 0)
                {
                    internalID = ImageSetHelper.NextInternalID(Url);
                }
                return internalID;
            }
            set
            {
                internalID = value;
            }
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

        static public ImageSet FromIImage(IImageSet imageset)
        {
            if (imageset == null)
            {
                return null;
            }
            var newset = new ImageSet();
            IImageSet newImageset = newset;
            newImageset.BandPass = imageset.BandPass;
            newImageset.BaseLevel = imageset.BaseLevel;
            newImageset.BaseTileDegrees = imageset.BaseTileDegrees;
            newImageset.BottomsUp = imageset.BottomsUp;
            newImageset.CenterX = imageset.CenterX;
            newImageset.CenterY = imageset.CenterY;
            newImageset.CreditsText = imageset.CreditsText;
            newImageset.CreditsUrl = imageset.CreditsUrl;
            newImageset.DataSetType = imageset.DataSetType;
            newImageset.DefaultSet = imageset.DefaultSet;
            newImageset.ElevationModel = imageset.ElevationModel;
            newImageset.Extension = imageset.Extension;
            newImageset.Generic = imageset.Generic;
            newImageset.ImageSetID = imageset.ImageSetID;
            newImageset.Levels = imageset.Levels;
            newImageset.Matrix = imageset.Matrix;
            newImageset.Mercator = imageset.Mercator;
            newImageset.Name = imageset.Name;
            newImageset.OffsetX = imageset.OffsetX;
            newImageset.OffsetY = imageset.OffsetY;
            newImageset.Projection = imageset.Projection;
            newImageset.QuadTreeTileMap = imageset.QuadTreeTileMap;
            newImageset.Rotation = imageset.Rotation;
            newImageset.Sparse = imageset.Sparse;
            newImageset.ThumbnailUrl = imageset.ThumbnailUrl;
            newImageset.Url = imageset.Url;
            newImageset.WidthFactor = imageset.WidthFactor;
            newImageset.WcsImage = imageset.WcsImage;
            newImageset.MeanRadius = imageset.MeanRadius;
            newImageset.ReferenceFrame = imageset.ReferenceFrame;
            newImageset.DemUrl = imageset.DemUrl;
            return newset;
        }

        #region IImageSet Members

        [XmlIgnore]
        public int BaseLevel
        {
            get
            {
                return baseTileLevelField;
            }
            set
            {
                baseTileLevelField = value;
            }
        }

        [XmlIgnore]
        public double BaseTileDegrees
        {
            get
            {
                return baseDegreesPerTileField;
            }
            set
            {
                baseDegreesPerTileField = value;
            }
        }

        [XmlIgnore]
        bool IImageSet.BottomsUp
        {
            get
            {
                return bottomsUpField == ImageSetBottomsUp.True;
            }
            set
            {
                bottomsUpField = value ? ImageSetBottomsUp.True : ImageSetBottomsUp.False;
            }
        }

        [XmlIgnore]
        public string CreditsText
        {
            get
            {
                return creditsField;
            }
            set
            {
                creditsField = value;
            }
        }


        [XmlIgnore]
        public bool DefaultSet
        {
            get
            {
                return stockSetField == ImageSetStockSet.True;
            }
            set
            {
                stockSetField = value ? ImageSetStockSet.True : ImageSetStockSet.False;
                stockSetFieldSpecified = true;
            }
        }

        [XmlIgnore]
        bool IImageSet.ElevationModel
        {
            get
            {
                return elevationModelField == ImageSetElevationModel.True;
            }
            set
            {
                elevationModelField = value ? ImageSetElevationModel.True : ImageSetElevationModel.False;
                elevationModelFieldSpecified = true;
            }
        }

        [XmlIgnore]
        public string Extension
        {
            get
            {
                return fileTypeField;
            }
            set
            {
                fileTypeField = value;
            }
        }

        [XmlIgnore]
        bool IImageSet.Generic
        {
            get
            {
                return genericField == ImageSetGeneric.True;
            }
            set
            {
                genericField = value ? ImageSetGeneric.True : ImageSetGeneric.False;
            }
        }

        [XmlIgnore]
        public int ImageSetID
        {
            get
            {
                return Math.Abs(GetHash());
            }
            set
            {
            }
        }

        bool isMandelbrot;
        bool mandelChecked;

        [XmlIgnore]
        public bool IsMandelbrot
        {
            get
            {
                if (!mandelChecked)
                {
                    isMandelbrot = urlField.ToLower().Contains("mandel.aspx");
                }
                return isMandelbrot;
            }
        }

        [XmlIgnore]
        public int Levels
        {
            get
            {
                return tileLevelsField;
            }
            set
            {
                tileLevelsField = value;
            }
        }

        private Matrix3d matrix;

        [XmlIgnore]

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
            matrix.Multiply(Matrix3d.RotationX((((rotationField)) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationZ(((CenterY) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationY((((360 - CenterX) + 180) / 180f * Math.PI)));
        }

        [XmlIgnore]
        public bool Mercator
        {
            get
            {
                return (projectionField == ProjectionType.Mercator);
            }
            set
            {
                
            }
        }

        [XmlIgnore]
        public string QuadTreeTileMap
        {
            get
            {
                return quadTreeMapField;
            }
            set
            {
                quadTreeMapField = value;
            }
        }

        [XmlIgnore]
        bool IImageSet.Sparse
        {
            get
            {
                return sparseField == ImageSetSparse.True;
            }
            set
            {
                sparseField = value ? ImageSetSparse.True : ImageSetSparse.False;
            }
        }

        [XmlIgnore]
        public IImageSet StockImageSet
        {
            get
            {
                if (genericField == ImageSetGeneric.True || stockSetField != ImageSetStockSet.True)
                {
                    return this;
                }
                return new ImageSetHelper(DataSetType, BandPass);
            }
        }

        #endregion

        #region IThumbnail Members

        Bitmap thumbnail;

        [XmlIgnore]
        public Bitmap ThumbNail
        {
            get { return thumbnail ?? (thumbnail = UiTools.LoadThumbnailFromWeb(thumbnailUrlField)); }
            set
            {
                if (thumbnail != null)
                {
                    thumbnail.Dispose();
                }
                thumbnail = value;
            }
        }

        bool isCloudCommunityItem;
        [XmlIgnore]
        public bool IsCloudCommunityItem
        {
            get
            {
                return isCloudCommunityItem;
            }
        }
        Rectangle bounds;
        [XmlIgnore]
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }

        [XmlIgnore]
        public bool IsImage
        {
            get { return true; }
        }

        [XmlIgnore]
        public bool IsTour
        {
            get { return false; }
        }

        [XmlIgnore]
        public bool ReadOnly
        {
            get { return true; }
        }
        [XmlIgnore]
        public bool IsFolder
        {
            get { return false; }
        }

        [XmlIgnore]
        public object[] Children
        {
            get { return null; }
        }

        #endregion

        #region IImageSet Members
        private string demUrl;
        [XmlAttribute(DataType = "anyURI")]

        public string DemUrl
        {
            get
            {
                if (String.IsNullOrEmpty(demUrl) && projectionField == ProjectionType.Mercator)
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

        #endregion
    }
}
