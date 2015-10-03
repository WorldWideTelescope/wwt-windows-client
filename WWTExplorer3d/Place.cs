using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using TerraViewer.org.worldwidetelescope.www;
using AstroCalc;



namespace TerraViewer
{
    

	/// <summary>
	/// Summary description for Place.
	/// </summary>
	public class TourPlace : IDisposable, TerraViewer.IThumbnail, TerraViewer.IPlace
	{
		public TourPlace()
		{
		}

        private object tag;

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        
        private string thumbnail;

        public string Thumbnail
        {
            get { return thumbnail; }
            set { thumbnail = value; }
        }

        private string name;

        public string Name
        {
            get { return Names[0]; }
   //         set { name = value; }
        }
        public string[] Names
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    return ("").Split(new char[] { ';' });
                }
                return name.Split(new char[] { ';' });
            }
            set
            {
                name = UiTools.GetNamesStringFromArray(value);
            }
        }

        private CameraParameters camParams = new CameraParameters(0.0, 0.0, -1.0, 0, 0, 100);

        public CameraParameters CamParams
        {
            get
            {
                if (Classification == Classification.SolarSystem && camParams.Target != SolarSystemObjects.Custom)
                {
                    var raDec = Planets.GetPlanetLocation(Name);
                    camParams.RA = raDec.RA;
                    camParams.Dec = raDec.Dec;
                    this.distnace = raDec.Distance;
                }

                return camParams;
            }
            set { camParams = value; }
        }

        public void UpdatePlanetLocation(double jNow)
        {
            if (Target != SolarSystemObjects.Undefined && Target != SolarSystemObjects.Custom)
            {
                camParams.ViewTarget = Planets.GetPlanetTargetPoint(Target, Lat, Lng, jNow);
            }
        }

        Vector3d location3d = new Vector3d(0, 0, 0);

        public Vector3d Location3d
        {
            get
            {
                if (Classification == Classification.SolarSystem || (location3d.X == 0 && location3d.Y == 0 && location3d.Z == 0))
                {
                    location3d = Coordinates.RADecTo3d(this.RA, this.Dec, 1);
                }
                return location3d;
            }
        }

        public double Lat
        {
            get { return CamParams.Lat; }
            set { camParams.Lat = value; }
        }

        public double Lng
        {
            get { return CamParams.Lng; }
            set { camParams.Lng = value; }
        }

        public double Opacity
        {
            get { return CamParams.Opacity; }
            set { camParams.Opacity = value; }
        }
        public string HtmlDescription = "";

        private string constellation = "";

        public string Constellation
        {
            get { return constellation; }
            set { constellation = value; }
        }
        private Classification classification = Classification.Galaxy;

        public Classification Classification
        {
            get { return classification; }
            set { classification = value; }
        }
        private ImageSetType type = ImageSetType.Sky;

        public ImageSetType Type
        {
            get { return type; }
            set { type = value; }
        }
        private double magnitude;

        public double Magnitude
        {
            get { return magnitude; }
            set { magnitude = value; }
        }
        private double distnace;

        public double Distance
        {
            get { return distnace; }
            set { distnace = value; }
        }
        /// <summary>
        /// Angular Size in Arc Seconds
        /// </summary>
        public double AngularSize = 60;


        public double ZoomLevel
        {
            get { return CamParams.Zoom; }
            set { camParams.Zoom = value; }
        }

        Bitmap thumbNail;

        private IImageSet studyImageset;

        public IImageSet StudyImageset
        {
            get { return studyImageset; }
            set { studyImageset = value; }
        }


        private IImageSet backgroundImageSet;

        public IImageSet BackgroundImageSet
        {
            get { return backgroundImageSet; }
            set
            {
                if (value != null)
                {
                    Type = value.DataSetType;
                }
                backgroundImageSet = value;
            }
        }

        private double searchDistance;

        public double SearchDistance
        {
            get { return searchDistance; }
            set { searchDistance = value; }
        }

        double elevation = 50;

        public double Elevation
        {
            get { return elevation; }
            set { elevation = value; }
        }

        public Bitmap ThumbNail
        {
            get 
            {
                if (thumbNail == null)
                {
                    if (Classification == Classification.Constellation)
                    {
                        if (!String.IsNullOrEmpty(constellation) )
                        {
                            if (Overview.ConstellationThumbnails.ContainsKey(constellation))
                            {
                                //todo clone this
                                thumbNail = Overview.ConstellationThumbnails[constellation];
                            }
                        }      
                    }
                    else
                    {
                        thumbNail = WWTThumbnails.WWTThmbnail.GetThumbnail(Name.Replace(" ", ""));
                        if (thumbNail == null)
                        {
                            var obj = global::TerraViewer.Properties.Resources.ResourceManager.GetObject(Enum.GetName(typeof(Classification), Classification), global::TerraViewer.Properties.Resources.Culture);
                            thumbNail = ((System.Drawing.Bitmap)(obj));
                        }
                    }
                    //Stream s = this.GetType().Assembly.GetManifestResourceStream(String.Format("TerraViewer.Properties.Resources.{0}", Enum.GetName(typeof(DataSetType), Type)));
                    //thumbNail = new Bitmap( s );
                    //s.Close();
                }

                return thumbNail;
            }
            set 
            {
                if (thumbNail != null && Classification != Classification.Constellation)
                {
                    thumbNail.Dispose();
                    GC.SuppressFinalize(thumbNail);
                }
                thumbNail = value;
            }
        }

        public double RA
        {
            get
            {
                return CamParams.RA;
            }
            set
            {
                camParams.RA = value;
            }
        }


        public double Dec
        {
            get { return CamParams.Dec; }
            set { camParams.Dec = value; }
        }


        public TourPlace(string name, double lat, double lng, Classification classification, string constellation, ImageSetType type, double zoomFactor)
        {
            ZoomLevel = zoomFactor;
            this.constellation = constellation;
            this.name = name;
            if (type == ImageSetType.Sky || type == ImageSetType.SolarSystem)
            {
                camParams.RA = lng;
            }
            else
            {
                Lng = lng;
            }

            Lat = lat;
            Classification = classification;

            Type = type;
        }

        public TourPlace(string name, CameraParameters camParams, Classification classification, string constellation, ImageSetType type, SolarSystemObjects target)
        {
            this.constellation = constellation;
            this.name = name;
            Classification = classification;
            this.camParams = camParams;
            Type = type;
            Target = target;
        }

		public TourPlace(string input, bool sky)
        {

            var sa = input.Split('\t');

            if (sky)
            {
                name = sa[0];


                Lat = Convert.ToDouble(sa[3]);
                if (sky)
                {
                    camParams.RA = Convert.ToDouble(sa[2])/15;
                    Type = ImageSetType.Sky;
                }
                else
                {
                    Lng = 180 - ((Convert.ToDouble(sa[2]) / 24.0 * 360) - 180);
                    Type = ImageSetType.Earth;
                }
                var type = sa[1];
                type = type.Replace(" ", "");
                type = type.Replace("StarCluster", "Cluster");
                type = type.Replace("TripleStar", "MultipleStars");
                type = type.Replace("HubbleImage", "Unidentified");
                Classification = (Classification)Enum.Parse(typeof(Classification), type);
                if (sa.Length > 4)
                {
                    try
                    {
                        if (sa[4].ToUpper() != "NULL" && sa[4] != "")
                        {
                            magnitude = Convert.ToDouble(sa[4]);
                        }
                    }
                    catch
                    {
                    }
                }
                if (sa.Length > 5)
                {
                    constellation = sa[5].ToUpper();
                }
                if (sa.Length > 6)
                {
                    try
                    {
                        ZoomLevel = Convert.ToDouble(sa[6]);
                    }
                    catch
                    {
                    }
                }
                if (sa.Length > 7)
                {
                    try
                    {
                        distnace = Convert.ToDouble(sa[7]);
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                name = sa[0];
                  Lat = (float)Convert.ToDouble(sa[1]);
                Lng = (float)Convert.ToDouble(sa[2]);
                Type = ImageSetType.Earth;
                if (sa.Length > 3)
                {
                    elevation = Convert.ToDouble(sa[3]);
                }
            }
		}
		public override string ToString()
		{
			return name;
		}


        #region IDisposable Members

        public void Dispose()
        {
            if (thumbNail != null && Classification != Classification.Constellation)
            {
                thumbNail.Dispose();
                GC.SuppressFinalize(thumbNail);
            }
        }


        #endregion

        internal void SaveToXml(System.Xml.XmlTextWriter xmlWriter, string elementName)
        {

            xmlWriter.WriteStartElement(elementName);
            xmlWriter.WriteAttributeString("Name", name);
            xmlWriter.WriteAttributeString("DataSetType", this.Type.ToString());
            if (this.Type == ImageSetType.Sky)
            {
                xmlWriter.WriteAttributeString("RA", camParams.RA.ToString());
                xmlWriter.WriteAttributeString("Dec", camParams.Dec.ToString());
            }
            else
            {
                xmlWriter.WriteAttributeString("Lat", Lat.ToString());
                xmlWriter.WriteAttributeString("Lng", Lng.ToString());
            }

            xmlWriter.WriteAttributeString("Constellation", constellation);
            xmlWriter.WriteAttributeString("Classification", Classification.ToString());
            xmlWriter.WriteAttributeString("Magnitude", magnitude.ToString());
            xmlWriter.WriteAttributeString("Distance", distnace.ToString());
            xmlWriter.WriteAttributeString("AngularSize", AngularSize.ToString());
            xmlWriter.WriteAttributeString("ZoomLevel", ZoomLevel.ToString());
            xmlWriter.WriteAttributeString("Rotation", camParams.Rotation.ToString());
            xmlWriter.WriteAttributeString("Angle", camParams.Angle.ToString());
            xmlWriter.WriteAttributeString("Opacity", camParams.Opacity.ToString());
            xmlWriter.WriteAttributeString("Target", Target.ToString());
            xmlWriter.WriteAttributeString("ViewTarget", camParams.ViewTarget.ToString());
            xmlWriter.WriteAttributeString("TargetReferenceFrame", camParams.TargetReferenceFrame);
            xmlWriter.WriteAttributeString("DomeAlt", camParams.DomeAlt.ToString());
            xmlWriter.WriteAttributeString("DomeAz", camParams.DomeAz.ToString());
            xmlWriter.WriteStartElement("Description");
            xmlWriter.WriteCData(HtmlDescription);
            xmlWriter.WriteEndElement();

 
            if (backgroundImageSet != null)
            {
                xmlWriter.WriteStartElement("BackgroundImageSet");
                ImageSetHelper.SaveToXml(xmlWriter, backgroundImageSet, "");
                xmlWriter.WriteEndElement();
            }

            if (studyImageset != null)
            {
                ImageSetHelper.SaveToXml(xmlWriter, studyImageset, "");
            }
            xmlWriter.WriteEndElement();
        }

        internal static TourPlace FromXml(XmlNode place)
        {
            var newPlace = new TourPlace();

            newPlace.name = place.Attributes["Name"].Value;
            newPlace.Type = (ImageSetType)Enum.Parse(typeof(ImageSetType), place.Attributes["DataSetType"].Value);
            if (newPlace.Type == ImageSetType.Sky)
            {
                newPlace.camParams.RA = Convert.ToDouble(place.Attributes["RA"].Value);
                newPlace.camParams.Dec = Convert.ToDouble(place.Attributes["Dec"].Value);
            }
            else
            {
                newPlace.Lat = Convert.ToDouble(place.Attributes["Lat"].Value);
                newPlace.Lng = Convert.ToDouble(place.Attributes["Lng"].Value);
            }
 
            newPlace.constellation = place.Attributes["Constellation"].Value;
            newPlace.Classification = (Classification)Enum.Parse(typeof(Classification), place.Attributes["Classification"].Value);
            newPlace.magnitude = Convert.ToDouble(place.Attributes["Magnitude"].Value);
            if (place.Attributes["Magnitude"] != null)
            {
                newPlace.magnitude = Convert.ToDouble(place.Attributes["Magnitude"].Value);
            }
            newPlace.AngularSize = Convert.ToDouble(place.Attributes["AngularSize"].Value);
            newPlace.ZoomLevel = Convert.ToDouble(place.Attributes["ZoomLevel"].Value);
            newPlace.camParams.Rotation = Convert.ToDouble(place.Attributes["Rotation"].Value);
            newPlace.camParams.Angle = Convert.ToDouble(place.Attributes["Angle"].Value);
            if (place.Attributes["Opacity"] != null)
            {
                newPlace.camParams.Opacity = Convert.ToSingle(place.Attributes["Opacity"].Value);
            }
            else
            {
                newPlace.camParams.Opacity = 100;
            }

            if (place.Attributes["Target"] != null)
            {
                newPlace.Target = (SolarSystemObjects)Enum.Parse(typeof(SolarSystemObjects), place.Attributes["Target"].Value);
            }

            if (place.Attributes["ViewTarget"] != null)
            {
                newPlace.camParams.ViewTarget = Vector3d.Parse(place.Attributes["ViewTarget"].Value);
            }

            if (place.Attributes["TargetReferenceFrame"] != null)
            {
                newPlace.camParams.TargetReferenceFrame = place.Attributes["TargetReferenceFrame"].Value;
            }

            if (place.Attributes["DomeAlt"] != null)
            {
                newPlace.camParams.DomeAlt = Convert.ToDouble(place.Attributes["DomeAlt"].Value);
            }

            if (place.Attributes["DomeAz"] != null)
            {
                newPlace.camParams.DomeAz = Convert.ToDouble(place.Attributes["DomeAz"].Value);
            }

            XmlNode descriptionNode = place["Description"];
            if (descriptionNode != null)
            {
                newPlace.HtmlDescription = descriptionNode.Value;
            }

            XmlNode backgroundImageSet = place["BackgroundImageSet"];
            if (backgroundImageSet != null)
            {
                XmlNode imageSet = backgroundImageSet["ImageSet"];
                
                var ish = ImageSetHelper.FromXMLNode(imageSet);

                if (!String.IsNullOrEmpty(ish.Url) && Earth3d.ReplacementImageSets.ContainsKey(ish.Url))
                {
                    newPlace.backgroundImageSet = Earth3d.ReplacementImageSets[ish.Url];
                }
                else
                {
                    newPlace.backgroundImageSet = ish;
                }
            }
                        
            XmlNode study = place["ImageSet"];
            if (study != null)
            {
                var ish = ImageSetHelper.FromXMLNode(study);

                if (!String.IsNullOrEmpty(ish.Url) && Earth3d.ReplacementImageSets.ContainsKey(ish.Url))
                {
                    newPlace.studyImageset = Earth3d.ReplacementImageSets[ish.Url];

                }
                else
                {
                    newPlace.studyImageset = ish;
                }
            }
            return newPlace;
        }
        internal static TourPlace FromAstroObjectsRow(AstroObjectsDataset.spGetAstroObjectsRow row)
        {
            var newPlace = new TourPlace();

            var seperator = "";

            var name = "";
            
            if (!row.IsPopularName1Null() && !String.IsNullOrEmpty(row.PopularName1) )
            {
                name = ProperCaps(row.PopularName1);
                seperator = ";";
            }

            if (!row.IsMessierNameNull() && !String.IsNullOrEmpty(row.MessierName))
            {
                name = name + seperator + row.MessierName;
                seperator = ";";
            }

            if (!row.IsNGCNameNull() && !String.IsNullOrEmpty(row.NGCName))
            {
                name = name + seperator + row.NGCName;
                seperator = ";";
            }

            newPlace.name = name;
            newPlace.Type = ImageSetType.Sky;
            newPlace.Lat = row.Dec2000;
            newPlace.Lng = row.Ra2000/15;
            newPlace.constellation = Constellations.Abbreviation(row.ConstellationName);
            newPlace.Classification = Classification.Galaxy; //(Classification)Enum.Parse(typeof(Classification), place.Attributes["Classification"].Value);
            newPlace.magnitude = row.IsVisualMagnitudeNull() ? row.VisualMagnitude : 0;
            newPlace.AngularSize = 0; // todo fix this
            newPlace.ZoomLevel = .00009;
            return newPlace;
        }   
        static string ProperCaps(string name)
        {
            var list = name.Split(new char[] {' '});
 
            var ProperName = "";

            foreach(var part in list)
            {
                ProperName = ProperName + part[0].ToString().ToUpper() + (part.Length > 1 ? part.Substring(1).ToLower() : "") + " ";
            }

            return ProperName.Trim();
        
        }

        #region IThumbnail Members

        Rectangle bounds; 
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

        #endregion
        public bool IsImage
        {
            get
            {
                return studyImageset != null || backgroundImageSet != null;
            }
        }

        public bool IsTour
        {
            get { return false; }
        }

        public bool IsFolder
        {
            get { return false; }
        }

        public object[] Children
        {
            get { return null; }
        }

        #region IThumbnail Members


        public bool ReadOnly
        {
            get { return true; }
        }

        #endregion

        #region IPlace Members

        public SolarSystemObjects Target
        {
            get
            {
                return camParams.Target;
            }
            set
            {
                camParams.Target = value;
            }
        }

        #endregion

        #region IThumbnail Members


        public bool IsCloudCommunityItem
        {
            get { return false; }
        }

        #endregion
    }
}
