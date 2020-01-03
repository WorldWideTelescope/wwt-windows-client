using AstroCalc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

#if WINDOWS_UWP
using VoTable= System.Object;
using XmlElement = Windows.Data.Xml.Dom.XmlElement;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;
#else
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;
#endif


namespace TerraViewer
{
    public partial class Folder : TerraViewer.IThumbnail
    {
        public override string ToString()
        {
            return nameField;
        }
        public static Folder LoadFromFile(string filename, bool versionDependent)
        {
            FileStream reader = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            Folder folder = LoadFromFileStream(reader, versionDependent);

            folder.loadedFilename = filename;
            reader.Close();
            return folder;
           
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private bool versionDependent = false;

        public bool VersionDependent
        {
            get { return versionDependent; }
            set
            {
                versionDependent = value;
                foreach (Folder folder in this.Folder1)
                {
                    folder.VersionDependent = versionDependent;
                }
            }
        }

        public static Folder LoadFromUrl(string url, bool versionDependent)
        {
            String dir = Properties.Settings.Default.CahceDirectory + "Data\\WTMLCACHE\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string filename = dir + Math.Abs(url.GetHashCode32()).ToString() + ".wtml";
            DataSetManager.DownloadFile(url, filename, false, versionDependent);
            try
            {
                if (File.Exists(filename))
                {
                    Folder temp = LoadFromFile(filename, versionDependent);
                    return temp;
                }
            }
            catch
            {
            }
            return null;
        }
        
        bool readOnly = true;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

        bool dirty = false;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool Dirty
        {
            get { return dirty; }
            set { dirty = value; }
        }
       
        private string loadedFilename = "";
       

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string LoadedFilename
        {
            get { return loadedFilename; }
            set { loadedFilename = value; }
        }


        public static Folder LoadFromFileStream(Stream stream, bool versionDependent)
        {
#if !WINDOWS_UWP
            XmlSerializer serializer = new XmlSerializer(typeof(Folder));
            Folder newFolder = (Folder)serializer.Deserialize(stream);
            newFolder.dirty = false;
            newFolder.VersionDependent = versionDependent;
            return newFolder;
#else
            XmlSerializer serializer = new XmlSerializer(typeof(Folder));
            Folder newFolder = (Folder)serializer.Deserialize(stream);
            newFolder.dirty = false;
            newFolder.VersionDependent = versionDependent;
            return newFolder;
#endif

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
#if !WINDOWS_UWP
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (FileStream writer = File.Open(filename,FileMode.Create,FileAccess.Write,FileShare.None))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Folder));
                serializer.Serialize(writer, this, ns);
                writer.Close();
            }
#else
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (FileStream writer = File.Open(filename,FileMode.Create,FileAccess.Write,FileShare.None))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Folder));
                serializer.Serialize(writer, this, ns);
                writer.Close();
            }
#endif
        }

        public void AddChildThumbnail(IThumbnail child)
        {
            if (child != null)
            {
                this.thumbnails.Add(child);
                dirty = true;
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

        public void AddChildImageSet(ImageSet child)
        {
            imagesets.Add(child);
        }

        Bitmap thumbnail = null;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

                    //todo uwp impliment folder icon
                    if (thumbnail == null)
                    {
                        thumbnail = Properties.Resources.Folder;
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
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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


        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsImage
        {
            get { return false; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsTour
        {
            get { return false; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsFolder
        {
            get { return true; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsCloudCommunityItem
        {
            get
            {
                return communityIdField != 0 || this.permissionField > 0;
            }
        }
        private Folder proxyFolder = null;

        public void Refresh()
        {
            Folder temp = Folder.LoadFromUrl(RenderEngine.Engine.PrepareUrl(urlField), VersionDependent);
            if (temp != null)
            {
                proxyFolder = temp;
            }
        }


        private DateTime lastUpdate = DateTime.Now.Subtract(new TimeSpan(1));
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public object[] Children
        {
            get
            {
                if (String.IsNullOrEmpty(urlField))
                {
                    List<object> returnList = new List<object>();
                    if (Folder1 != null)
                    {
                        returnList.AddRange(Folder1);
                    }
                    if (ImageSet != null)
                    {
                        returnList.AddRange(this.ImageSet);
                    }
                    if (Items != null)
                    {
                        returnList.AddRange(this.Items);
                    }
                    if (this.Tour != null)
                    {
                        returnList.AddRange(this.Tour);
                    }
                    if (LineSet != null)
                    {
                        returnList.AddRange(this.LineSet);
                    }
                    if (thumbnails != null)
                    {
                        returnList.AddRange(thumbnails);
                    }

                    return returnList.ToArray();
                }
                else
                {
                    TimeSpan ts = lastUpdate.Subtract(DateTime.Now);
                    // TOdo add add Move Complete Auto Update
                    // todo add URL formating for Ambient Parameters
                    // TODO remove true when perth fixes refresh type on server
                    if ( true  || RefreshType == FolderRefreshType.ConditionalGet || proxyFolder == null || 
                        (this.RefreshType == FolderRefreshType.Interval && (Convert.ToInt32(refreshIntervalField) < ts.TotalSeconds)))
                    {
                        Refresh();
                    }

                    if (proxyFolder != null)
                    {
                        return proxyFolder.Children;
                    }
                    else
                    {
                        return null;
                    }
                    
                }
            }
        }
    }

    public partial class Tour : TerraViewer.IThumbnail, TerraViewer.ITourResult
    {
        Bitmap thumbnail = null;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Bitmap ThumbNail
        {
            get
            {
                if (thumbnail == null)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(thumbnailUrlField))
                        {
                            thumbnail = UiTools.LoadThumbnailFromWeb(thumbnailUrlField);
                        }
                        else
                        {
                            thumbnail = UiTools.LoadThumbnailFromWeb(String.Format("http://www.worldwidetelescope.org/wwtweb/GetTourThumbnail.aspx?GUID={0}", idField));
                        }
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]   
        public bool IsCloudCommunityItem
        {
            get
            {
                return MSRComponentId != 0;
            }
        }

        Rectangle bounds;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string Name
        {
            get { return titleField; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsImage
        {
            get { return false; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsTour
        {
            get { return true; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsFolder
        {
            get { return false; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string AuthorContactText
        {
            get
            {
                return this.authorEmailField;
            }
            set
            {
               
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        Bitmap authorImageBitmap = null;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Bitmap AuthorImage
        {
            get
            {
                if (authorImageBitmap == null)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(authorImageUrlField))
                        {
                            authorImageBitmap = UiTools.LoadThumbnailFromWeb(authorImageUrlField);
                        }
                        else
                        {
                            authorImageBitmap = UiTools.LoadThumbnailFromWeb(String.Format("http://www.worldwidetelescope.org/wwtweb/GetAuthorThumbnail.aspx?GUID={0}", idField));
                        }
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


        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public double LengthInSeconds
        {
            get { return (double)lengthInSecsField; }
            set { }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string AuthorUrl
        {
            get
            {
                return this.authorURLField;
            }
            set
            {
                authorURLField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string OrgName
        {
            get
            {
                return this.organizationNameField;
            }
            set
            {
                organizationNameField = value;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
            Place newPlace = new Place();
            if (place.BackgroundImageSet != null)
            {
                newPlace.backgroundImageSetField = new PlaceBackgroundImageSet();
                newPlace.backgroundImageSetField.ImageSet = ImageSet.FromIImage(place.BackgroundImageSet);
            }
            if (place.StudyImageset != null)
            {
                newPlace.foregroundImageSetField = new PlaceForegroundImageSet();
                newPlace.foregroundImageSetField.ImageSet = ImageSet.FromIImage(place.StudyImageset);
            }
            newPlace.CamParams = place.CamParams;

            string names="";
            string delim = "";
            foreach (string name in place.Names)
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

        [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        Tour tour = null;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Tour Tour
        {
            get { return tour; }
            set { tour = value; }
        }
#region IPlace Members

        private string url;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        IImageSet IPlace.BackgroundImageSet
        {
            get
            {
                if (backgroundImageSetField != null)
                {
                    return backgroundImageSetField.ImageSet;
                }
                else
                {
                    return null;
                }
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


        [System.Xml.Serialization.XmlIgnoreAttribute()]    
        public bool IsCloudCommunityItem
        {
            get
            {
                return MSRComponentId != 0;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public CameraParameters CamParams
        {
            get
            {
                CameraParameters cam =  new CameraParameters(latField, lngField, zoomLevelField, rotationField, angleField, (float)opacityField);
                if (raFieldSpecified)
                {
                    cam.RA = this.RA;
                }
                if (decFieldSpecified)
                {
                    cam.Dec = this.Dec;
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        double IPlace.Dec
        {
            get
            {
                if (Classification == Classification.SolarSystem)
                {
                    AstroRaDec raDec = Planets.GetPlanetLocation(Name);
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Vector3d Location3d
        {
            get
            {
                if (Type == ImageSetType.Planet || Type == ImageSetType.Earth)
                {
                    if ((location3d.X == 0 && location3d.Y == 0 && location3d.Z == 0))
                    {
                        location3d = Coordinates.RADecTo3d(this.Lng/15,this.Lat, 1);
                    }

                }
                else if (Classification == Classification.SolarSystem || (location3d.X == 0 && location3d.Y == 0 && location3d.Z == 0))
                {
                    location3d = Coordinates.RADecTo3d(this.RA, this.Dec, 1);
                }
                return location3d;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public string[] Names
        {
            get
            {
                if (string.IsNullOrEmpty(nameField))
                {
                    return ("").Split(new char[] { ';' });
                }
                return nameField.Split(new char[] { ';' });
            }
            set
            {
                nameField = UiTools.GetNamesStringFromArray(value);
            }
        }


        [System.Xml.Serialization.XmlIgnoreAttribute()]
        double IPlace.RA
        {
            get
            {
                if (Classification == Classification.SolarSystem)
                {
                    AstroRaDec raDec = Planets.GetPlanetLocation(Name);

                    raField = raDec.RA;
                    decField = raDec.Dec;
                    this.distanceField = raDec.Distance;
                }
                return raField;
            }
            set
            {
                raField = value;
                raFieldSpecified = true;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        Bitmap thumbNail = null;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
                            thumbNail = ThumbnailCache.GetConstellationThumbnail(constellationField);
                        }
                    }
                    else
                    {
                        //todo uwp find anther way to do this.

                        thumbNail = UiTools.LoadThumbnailByName(Name);
                        if (thumbNail == null)
                        {
                            object obj = TerraViewer.Properties.Resources.ResourceManager.GetObject(Enum.GetName(typeof(Classification), Classification), global::TerraViewer.Properties.Resources.Culture);
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ReadOnly
        {
            get { return true; }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsImage
        {
            get
            {
                return (foregroundImageSetField != null && foregroundImageSetField.ImageSet != null)
                    || (backgroundImageSetField != null && backgroundImageSetField.ImageSet != null);
            }

        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsTour
        {
            get { return tour != null; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsFolder
        {
            get { return false; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public object[] Children
        {
            get { return null; }
        }

#endregion

#region IPlace Members


        private double searchDistance = 0;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public double SearchDistance
        {
            get { return searchDistance; }
            set { searchDistance = value; }
        }

#endregion

#region IPlace Members


        [System.Xml.Serialization.XmlIgnoreAttribute()]
        TerraViewer.Classification IPlace.Classification
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public IImageSet StudyImageset
        {
            get
            {
                if (foregroundImageSetField != null)
                {
                    return foregroundImageSetField.ImageSet;
                }
                else
                {
                    return null;
                }
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
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        private object wcsImage;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public object WcsImage
        {
            get { return wcsImage; }
            set { wcsImage = value; }
        }

        static ushort nextID = 0;
       
        private static ushort NextInternalID()
        {
            return nextID++;
        }

        ushort internalID = 0;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        private Dictionary<string, string> properties = new Dictionary<string, string>();

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        private VoTable tableMetadata = null;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        int hash = 0;

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
            ImageSet newset = new ImageSet();
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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


        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        bool IImageSet.ElevationModel
        {
            get
            {
                return this.elevationModelField == ImageSetElevationModel.True;
            }
            set
            {
                if (value)
                {
                    this.elevationModelField = ImageSetElevationModel.True;
                }
                else
                {
                    this.elevationModelField = ImageSetElevationModel.False;
                }
                elevationModelFieldSpecified = true;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public int ImageSetID
        {
            get
            {
                return Math.Abs(this.GetHash());
            }
            set
            {
            }
        }

        bool isMandelbrot = false;
        bool mandelChecked = false;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsMandelbrot
        {
            get
            {
                if (!mandelChecked)
                {
                    isMandelbrot = this.urlField.ToLower().Contains("mandel.aspx");
                }
                return isMandelbrot;
            }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]

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
            matrix.Multiply(Matrix3d.RotationX((((this.rotationField)) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationZ(((this.CenterY) / 180f * Math.PI)));
            matrix.Multiply(Matrix3d.RotationY((((360 - this.CenterX) + 180) / 180f * Math.PI)));
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public IImageSet StockImageSet
        {
            get
            {
                if (this.genericField == ImageSetGeneric.True || stockSetField != ImageSetStockSet.True)
                {
                    return this;
                }
                else
                {
                    return new ImageSetHelper(this.DataSetType, this.BandPass);
                }
            }
        }

#endregion

#region IThumbnail Members

        Bitmap thumbnail = null;

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Bitmap ThumbNail
        {
            get
            {
                if (thumbnail == null)
                {
                    thumbnail = UiTools.LoadThumbnailFromWeb(thumbnailUrlField);
                    if (thumbnail.Height != 45)
                    {
                        var temp = UiTools.MakeThumbnail(thumbnail);
                        thumbnail.Dispose();
                        thumbnail = temp;
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

        bool isCloudCommunityItem = false;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsCloudCommunityItem
        {
            get
            {
                return isCloudCommunityItem;
            }
        }
        Rectangle bounds;
        [System.Xml.Serialization.XmlIgnoreAttribute()]
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

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsImage
        {
            get { return true; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsTour
        {
            get { return false; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ReadOnly
        {
            get { return true; }
        }
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsFolder
        {
            get { return false; }
        }

        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public object[] Children
        {
            get { return null; }
        }

#endregion

#region IImageSet Members
        private string demUrl;
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "anyURI")]

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
