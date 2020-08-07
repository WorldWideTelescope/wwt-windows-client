using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Net;


namespace TerraViewer
{
    public enum UserLevel { Beginner, Intermediate, Advanced, Educator, Professional };
    public class TourDocument : IDisposable
    {
        int tourDirty = 0;
        public bool TourDirty
        {
            get { return tourDirty > 0; }
            set 
            {
                if (value)
                {
                    tourDirty++;
                }
                else
                {
                    tourDirty = 0;
                }

            }
        }

        public TourDocument()
        {
            id = Guid.NewGuid().ToString();
        }
        string workingDirectory = "";
        public string WorkingDirectory
        {
            get
            {
                if (String.IsNullOrEmpty(workingDirectory))
                {
                    workingDirectory = TourDocument.BaseWorkingDirectory + id.ToString() + @"\";
                }

                if (!Directory.Exists(workingDirectory))
                {
                    Directory.CreateDirectory(workingDirectory);
                }

                return workingDirectory;
            }
            set
            {
                workingDirectory = value;
            }
        }
        public static string BaseWorkingDirectory
        {
            get
            {
                return Properties.Settings.Default.CahceDirectory + @"Temp\";
            }

        }

        public static TourDocument FromFile(string filename, bool forEdit)
        {
            FileCabinet cab = new FileCabinet(filename, BaseWorkingDirectory);
            cab.Extract();
            TourDocument newTour =  TourDocument.FromXml(cab.MasterFile);
            if (forEdit)
            {
                newTour.SaveFileName = filename;
            }

            return newTour;
        }

        public void MergeTour(string filename)
        {
            FileCabinet cab = new FileCabinet(filename, BaseWorkingDirectory);
            cab.Extract(false, WorkingDirectory);

            MergeTourStopsFromXml(cab.MasterFile);
        }

        string saveFileName = null;

        public string SaveFileName
        {
            get { return saveFileName; }
            set { saveFileName = value; }
        }
        
        public bool OverWrite = false;
        public bool CollisionChecked = false;

        public bool DomeMode = false;

        public bool IsTimelineTour()
        {
            foreach (TourStop stop in TourStops)
            {
                if (stop.KeyFramed)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsWebCompatible()
        {
            foreach (TourStop stop in TourStops)
            {
                if (stop.Target.BackgroundImageSet.DataSetType == ImageSetType.SolarSystem)
                {
                    return false;
                }
            }

            return true;
        }


        public static TourDocument FromXml(string filename)
        {

            TourDocument newTour = new TourDocument();
            newTour.filename = filename;
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);


            XmlNode root = doc["Tour"];


            newTour.id = root.Attributes["ID"].Value.ToString();
            newTour.Title = root.Attributes["Title"].Value.ToString();
            newTour.Author = root.Attributes["Author"].Value.ToString();

            if (root.Attributes["Descirption"] != null)
            {
                newTour.Description = root.Attributes["Descirption"].Value;
            }

            if (root.Attributes["Description"] != null)
            {
                newTour.Description = root.Attributes["Description"].Value;
            }

            if (root.Attributes["AuthorEmail"] != null)
            {
                newTour.authorEmail = root.Attributes["AuthorEmail"].Value;
            }

            if (root.Attributes["Keywords"] != null)
            {
                newTour.Keywords = root.Attributes["Keywords"].Value;
            }

            if (root.Attributes["OrganizationName"] != null)
            {
                newTour.OrgName = root.Attributes["OrganizationName"].Value;
            }

            if (root.Attributes["DomeMode"] != null)
            {
                newTour.DomeMode = bool.Parse(root.Attributes["DomeMode"].Value);
            }

            newTour.organizationUrl = root.Attributes["OrganizationUrl"].Value.ToString();
            newTour.level = (UserLevel)Enum.Parse(typeof(UserLevel), root.Attributes["UserLevel"].Value.ToString());
            newTour.type = (Classification)Enum.Parse(typeof(Classification), root.Attributes["Classification"].Value.ToString());
            newTour.taxonomy = root.Attributes["Taxonomy"].Value.ToString();

            bool timeLineTour = false;
            if (root.Attributes["TimeLineTour"] != null)
            {
                timeLineTour = bool.Parse(root.Attributes["TimeLineTour"].Value);
            }


            XmlNode TourStops = null;

            if (timeLineTour)
            {
                TourStops = root["TimeLineTourStops"];
            }
            else
            {
                TourStops = root["TourStops"];
            }


            foreach (XmlNode tourStop in TourStops)
            {
                newTour.AddTourStop(TourStop.FromXml(newTour, tourStop));
            }

            XmlNode Frames = root["ReferenceFrames"];

            if (Frames != null)
            {
                foreach (XmlNode frame in Frames)
                {
                    ReferenceFrame newFrame = new ReferenceFrame();
                    newFrame.InitializeFromXml(frame);
                    if (!LayerManager.AllMaps.ContainsKey(newFrame.Name))
                    {
                        LayerMap map = new LayerMap(newFrame.Name, ReferenceFrames.Custom);
                        map.Frame = newFrame;
                        map.LoadedFromTour = true;
                        LayerManager.AllMaps.Add(newFrame.Name, map);
                    }
                }
                LayerManager.ConnectAllChildren();
                LayerManager.LoadTree();
            }

            XmlNode Layers = root["Layers"];

            if (Layers != null)
            {
                foreach (XmlNode layer in Layers)
                {
                    Layer newLayer = Layer.FromXml(layer, true);
                    string fileName = newTour.WorkingDirectory + string.Format("{0}.txt", newLayer.ID.ToString());

                    // Overwite ISS layer if in a tour using the authored ISS details
                    if (LayerManager.LayerList.ContainsKey(newLayer.ID) && newLayer.ID == ISSLayer.ISSGuid)
                    {
                        LayerManager.DeleteLayerByID(newLayer.ID, true, false);
                    }

                    // Ask about merging other layers.
                    if (LayerManager.LayerList.ContainsKey(newLayer.ID) && newLayer.ID != ISSLayer.ISSGuid)
                    {
                        if (!newTour.CollisionChecked)
                        {
                            if (UiTools.ShowMessageBox(Language.GetLocalizedText(958, "There are layers with the same name. Overwrite existing layers?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                newTour.OverWrite = true;
                            }
                            else
                            {
                                newTour.OverWrite = false;

                            }
                            newTour.CollisionChecked = true;
                        }

                        if (newTour.OverWrite)
                        {
                            LayerManager.DeleteLayerByID(newLayer.ID, true, false);
                        }
                    }

                    try
                    {
                        newLayer.LoadedFromTour = true;
                        newLayer.LoadData(fileName);
                        LayerManager.Add(newLayer, false);
                    }
                    catch
                    {
                    }
                }
                LayerManager.LoadTree();
            }

            if (File.Exists(newTour.WorkingDirectory + "Author.png"))
            {
                newTour.authorImage = UiTools.LoadBitmap(newTour.WorkingDirectory + "Author.png");
            }

            newTour.tourDirty = 0;
            return newTour;
        }
        public void MergeTourStopsFromXml(string filename)
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(filename);


            XmlNode root = doc["Tour"];


            bool timeLineTour = false;
            if (root.Attributes["TimeLineTour"] != null)
            {
                timeLineTour = bool.Parse(root.Attributes["TimeLineTour"].Value);
            }


            XmlNode TourStops = null;

            if (timeLineTour)
            {
                TourStops = root["TimeLineTourStops"];
            }
            else
            {
                TourStops = root["TourStops"];
            }

            foreach (XmlNode tourStop in TourStops)
            {
                currentTourstopIndex++;

                TourStop ts = TourStop.FromXml(this, tourStop);
                ts.Id = Guid.NewGuid().ToString();
                InsertTourStop(ts);
            }

            XmlNode Frames = root["ReferenceFrames"];

            if (Frames != null)
            {
                foreach (XmlNode frame in Frames)
                {
                    ReferenceFrame newFrame = new ReferenceFrame();
                    newFrame.InitializeFromXml(frame);
                    if (!LayerManager.AllMaps.ContainsKey(newFrame.Name))
                    {
                        LayerMap map = new LayerMap(newFrame.Name, ReferenceFrames.Custom);
                        map.Frame = newFrame;
                        LayerManager.AllMaps.Add(newFrame.Name, map);
                    }
                }
                LayerManager.ConnectAllChildren();
                LayerManager.LoadTree();
            }



            XmlNode Layers = root["Layers"];

            if (Layers != null)
            {
                foreach (XmlNode layer in Layers)
                {
                    Layer newLayer = Layer.FromXml(layer, true);
                    string fileName = WorkingDirectory + string.Format("{0}.txt", newLayer.ID.ToString());

                    newLayer.LoadData(fileName);
                    LayerManager.Add(newLayer, false);

                }
                LayerManager.LoadTree();
            }
        }

        string tagId;

        public string TagId
        {
            get { return tagId; }
            set { tagId = value; }
        }


        public string TempFilename
        {
            get
            {
                return Properties.Settings.Default.CahceDirectory + @"Temp\" + id.ToString() + "\\" + id.ToString() + ".wwtxml";
            }
        }

        public void SaveToXml(bool forExport)
        {
            string outFile;
            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + @"Temp\"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + @"Temp\");
            }

            if (!forExport)
            {
                // Try with title first cleaned for filename
                if (String.IsNullOrEmpty(filename) && UiTools.CleanFileName(title) != null)
                {
                    filename = Properties.Settings.Default.CahceDirectory + @"Temp\" + UiTools.CleanFileName(this.title) + ".wwtxml";
                }

                //Use a guid if the Title is only non-legal characters or is empty
                if (String.IsNullOrEmpty(filename))
                {
                    filename = Properties.Settings.Default.CahceDirectory + @"Temp\" + id.ToString() + ".wwtxml";
                }
                outFile = filename;
            }
            else
            {
                
                 outFile = TempFilename;
                
            }

            WriteTourXML(outFile);

            if (authorImage != null)
            {
                UiTools.SaveBitmap(authorImage,AuthorThumbnailFilename,System.Drawing.Imaging.ImageFormat.Png);
            }

        }

        internal void WriteTourXML(string outFile)
        {
            using (XmlTextWriter xmlWriter = new XmlTextWriter(outFile, System.Text.Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                xmlWriter.WriteStartElement("Tour");

                xmlWriter.WriteAttributeString("ID", this.id);
                xmlWriter.WriteAttributeString("Title", this.title);
                xmlWriter.WriteAttributeString("Descirption", this.Description);
                xmlWriter.WriteAttributeString("Description", this.Description);
                xmlWriter.WriteAttributeString("RunTime", this.RunTime.TotalSeconds.ToString());
                xmlWriter.WriteAttributeString("Author", this.author);
                xmlWriter.WriteAttributeString("AuthorEmail", this.authorEmail);
                xmlWriter.WriteAttributeString("OrganizationUrl", this.organizationUrl);
                xmlWriter.WriteAttributeString("OrganizationName", this.OrgName);
                xmlWriter.WriteAttributeString("Keywords", this.Keywords);
                xmlWriter.WriteAttributeString("UserLevel", level.ToString());
                xmlWriter.WriteAttributeString("Classification", type.ToString());
                xmlWriter.WriteAttributeString("Taxonomy", taxonomy.ToString());
                xmlWriter.WriteAttributeString("DomeMode", DomeMode.ToString());
                bool timeLineTour = IsTimelineTour();
                xmlWriter.WriteAttributeString("TimeLineTour", timeLineTour.ToString());

                if (timeLineTour)
                {
                    xmlWriter.WriteStartElement("TimeLineTourStops");
                    foreach (TourStop stop in TourStops)
                    {
                        stop.SaveToXml(xmlWriter, true);
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteRaw(Properties.Resources.UpdateRequired);

                }           
                else
                {
                    xmlWriter.WriteStartElement("TourStops");
                    foreach (TourStop stop in TourStops)
                    {
                        stop.SaveToXml(xmlWriter, true);
                    }
                    xmlWriter.WriteEndElement();
                }


                List<Guid> masterList = CreateLayerMasterList();

                // This will now save and sync emtpy frames...
                List<ReferenceFrame> referencedFrames = GetReferenceFrameList();

                xmlWriter.WriteStartElement("ReferenceFrames");
                foreach (ReferenceFrame item in referencedFrames)
                {
                    item.SaveToXml(xmlWriter);
                }
                xmlWriter.WriteEndElement();


                xmlWriter.WriteStartElement("Layers");
                foreach (Guid id in masterList)
                {
                    if (LayerManager.LayerList.ContainsKey(id))
                    {
                        LayerManager.LayerList[id].SaveToXml(xmlWriter);
                    }
                }
                xmlWriter.WriteEndElement();


                xmlWriter.WriteFullEndElement();
                xmlWriter.Close();

            }
        }

        private List<ReferenceFrame> GetReferenceFrameList(List<Guid> masterList)
        {
            
            List<ReferenceFrame> list = new List<ReferenceFrame>();

            foreach (Guid id in masterList)
            {
                if (LayerManager.LayerList.ContainsKey(id))
                {
                    string frame = LayerManager.LayerList[id].ReferenceFrame;

                    while (!string.IsNullOrEmpty(frame))
                    {
                        if (LayerManager.AllMaps.ContainsKey(frame) && (LayerManager.AllMaps[frame].Frame.Reference == ReferenceFrames.Custom || LayerManager.AllMaps[frame].Frame.Reference == ReferenceFrames.Identity) && !list.Contains(LayerManager.AllMaps[frame].Frame))
                        {
                            list.Add(LayerManager.AllMaps[frame].Frame);
                            frame = LayerManager.AllMaps[frame].Frame.Parent;
                        }
                        else
                        {
                            frame = null;
                        }
                    }
                }
            }

            return list;

        }

        private List<ReferenceFrame> GetReferenceFrameList()
        {
            List<ReferenceFrame> list = new List<ReferenceFrame>();

            foreach (LayerMap lm in LayerManager.AllMaps.Values)
            {
                if ((lm.Frame.Reference == ReferenceFrames.Custom || lm.Frame.Reference == ReferenceFrames.Identity) && !list.Contains(lm.Frame) && lm.Frame.SystemGenerated == false)
                {
                    list.Add(lm.Frame);
                }
            }

            return list;
        }

        private List<Guid> CreateLayerMasterList()
        {
            List<Guid> masterList = new List<Guid>();
            foreach (TourStop stop in TourStops)
            {
                foreach (Guid id in stop.Layers.Keys)
                {
                    if (!masterList.Contains(id))
                    {
                        if (LayerManager.LayerList.ContainsKey(id))
                        {
                            masterList.Add(id);
                        }
                    }
                }
            }
            return masterList;
        }

        public string AuthorThumbnailFilename
        {
            get
            {
                return WorkingDirectory + "Author.Png";
            }
        }

        private int representativeThumbnailTourstop = 0;

        public string TourThumbnailFilename
        {
            get
            {
                if (representativeThumbnailTourstop < tourStops.Count)
                {
                    return tourStops[representativeThumbnailTourstop].TourStopThumbnailFilename;
                }
                else
                {
                    return null;
                }
            }
        }        

        //<?xml version="1.0" encoding="utf-16"?>
        //<ImportTourRequest>
        //<Tour>
        //<TourGUID>4B0392B9-050E-414B-A22F-9CD9BFCCB585</TourGUID>
        //<Title>Paul's blah de blah Tour</Title>
        //<Description>this is a very long and lengthy description of Pauls New Tour</Description>
        //<AttributesAndCredits>attrs and creds goes in this string</AttributesAndCredits>
        //<Author>
        //<Name>Paul Johns2</Name>
        //<EMailAddr>pauljoh2@microsoft.com</EMailAddr>
        //<URL>www.msn.com</URL>
        //<OtherEmails>otheremails@wherever.com  and more emails....</OtherEmails>
        //<ContactPhone>206-777-1233</ContactPhone>
        //<ContactText>any other contact info goes here</ContactText>
        //<Organization>
        //<OrgName>MicrosoftResearch</OrgName>
        //<URL>org url goes here (msrweb)</URL>
        //</Organization>
        //</Author>
        //<Keywords>
        //<Keyword>x</Keyword>
        //<Keyword>y</Keyword>
        //</Keywords>
        //<AstroObjects>
        //<AstroObject>M51</AstroObject>
        //<AstroObject>M11</AstroObject>
        //</AstroObjects>
        //<ImageTaxonomyHierarchy>
        //<ITH_Node>3.3</ITH_Node>
        //<ITH_Node>3.2.1.5</ITH_Node>
        //</ImageTaxonomyHierarchy>
        //<ExplicitTourLinks>
        //<TourGUID>FFF33807-5845-494B-A96B-C1105163160D</TourGUID>
        //<TourGUID>FFF3380--5845-494B-A96B-C1105163160E</TourGUID>
        //<TourGUID>4B0392B9-050E-414B-A22F-9CD9BFCCB578</TourGUID>
        //</ExplicitTourLinks>
        //</Tour>
        //</ImportTourRequest>

        public string GetXmlSubmitString()
        {
            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-16'");
                    xmlWriter.WriteStartElement("ImportTourRequest");
                    xmlWriter.WriteStartElement("Tour");
                    xmlWriter.WriteElementString("TourGUID", this.id);
                    xmlWriter.WriteElementString("Title", this.title);
                    xmlWriter.WriteElementString("Description", this.description);
                    xmlWriter.WriteElementString("AttributesAndCredits", this.attributesAndCredits);
                    xmlWriter.WriteStartElement("Author");
                    xmlWriter.WriteElementString("Name", this.author);
                    xmlWriter.WriteElementString("EMailAddr", this.authorEmail);
                    xmlWriter.WriteElementString("URL", this.authorUrl);
                    xmlWriter.WriteElementString("OtherEmails", this.authorEmailOther);
                    xmlWriter.WriteElementString("ContactPhone", this.authorPhone);
                    xmlWriter.WriteElementString("ContactText", this.authorContactText);
                    xmlWriter.WriteStartElement("Organization");
                    xmlWriter.WriteElementString("OrgName", this.orgName);
                    xmlWriter.WriteElementString("URL", this.organizationUrl);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteStartElement("Keywords");
                    foreach (string keyword in this.Keywords.Split(new char[] {';'}))
                    {
                        xmlWriter.WriteElementString("Keyword", keyword);
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("AstroObjects");
                    foreach (string obj in this.Objects.Split(new char[] {';'}))
                    {
                        xmlWriter.WriteElementString("AstroObject", obj);
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("ImageTaxonomyHierarchy");
                    foreach (string node in this.Taxonomy.Split(new char[] {';'}))
                    {
                        xmlWriter.WriteElementString("ITH_Node", node);
                    }
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteStartElement("ExplicitTourLinks");
                    foreach (string guid in this.ExplicitTourLinks)
                    {
                        xmlWriter.WriteElementString("TourGUID", guid);
                    }
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteElementString("LengthInSecs", ((int)(this.runTime.TotalSeconds+.9)).ToString());

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.Close();

                }
                sw.Close();
                return sw.ToString();
            }
        }

        public void SaveToFile(string saveFilename)
        {
            SaveToFile(saveFilename, false, false);
        }
        public void SaveToFile(string saveFilename, bool tempFile, bool excludeAudio)
        {
            CleanUp();
            GC.Collect();
            SaveToXml(false);

            FileCabinet fc = new FileCabinet(saveFilename, BaseWorkingDirectory);
            fc.PackageID = this.Id;
            if (!tempFile)
            {
                this.saveFileName = saveFilename;
            }

            fc.AddFile(filename);

            if (authorImage != null)
            {
                fc.AddFile(WorkingDirectory + "Author.Png");
            }

            foreach (TourStop stop in TourStops)
            {
                stop.AddFilesToCabinet(fc, excludeAudio);
            }

            List<Guid> masterList = CreateLayerMasterList();

            foreach (Guid id in masterList)
            {
                if (LayerManager.LayerList.ContainsKey(id))
                {
                    LayerManager.LayerList[id].AddFilesToCabinet(fc);
                }
            }

            fc.Package();
            TourDirty = false;
        }

        string saveID = "";

        string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                TourDirty = true;
            }
        }

        TimeSpan runTime = new TimeSpan();
        int lastDirtyCheck = 0;

        public TimeSpan RunTime
        {
            get
            {
                if (runTime.TotalSeconds == 0 || lastDirtyCheck != tourDirty)
                {
                    runTime = CalculateRunTime();
                    lastDirtyCheck = tourDirty;
                }
                return runTime;
            }
        }

        string description;

        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                TourDirty = true;
            }
        }
        string attributesAndCredits;

        public string AttributesAndCredits
        {
            get { return attributesAndCredits; }
            set
            {
                attributesAndCredits = value;
                TourDirty = true;
            }
        } 
        
        string authorEmailOther;

        public string AuthorEmailOther
        {
            get { return authorEmailOther; }
            set
            {
                authorEmailOther = value;
                TourDirty = true;
            }
        } 

        string authorEmail;

        public string AuthorEmail
        {
            get { return authorEmail; }
            set
            {
                authorEmail = value;
                TourDirty = true;
            }
        } 

        string authorUrl;

        public string AuthorUrl
        {
            get { return authorUrl; }
            set
            {
                authorUrl = value;
                TourDirty = true;
            }
        } 

        string authorPhone;

        public string AuthorPhone
        {
            get { return authorPhone; }
            set
            {
                authorPhone = value;
                TourDirty = true;
            }
        }

        string authorContactText;

        public string AuthorContactText
        {
            get { return authorContactText; }
            set
            {
                authorContactText = value;
                TourDirty = true;
            }
        }

        string orgName = "None";

        public string OrgName
        {
            get { return orgName; }
            set
            {
                orgName = value;
                TourDirty = true;
            }
        }

        string orgUrl;

        public string OrgUrl
        {
            get { return orgUrl; }
            set
            {
                orgUrl = value;
                TourDirty = true;
            }
        }

        string author;

        public string Author
        {
            get { return author; }
            set
            {
                author = value;
                TourDirty = true;
            }
        }
        string authorImageUrl;

        public string AuthorImageUrl
        {
            get { return authorImageUrl; }
            set
            {
                authorImageUrl = value;
                TourDirty = true;
            }
        }
        Bitmap authorImage;

        public Bitmap AuthorImage
        {
            get { return authorImage; }
            set
            {
                authorImage = value;
                TourDirty = true;
            }
        }

        string organizationUrl;

        public string OrganizationUrl
        {
            get { return organizationUrl; }
            set
            {
                organizationUrl = value;
                TourDirty = true;
            }
        }


        String filename;

        public String FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        UserLevel level;

        public UserLevel Level
        {
            get { return level; }
            set
            {
                level = value;
                TourDirty = true;
            }
        }

        Classification type;

        public Classification Type
        {
            get { return type; }
            set
            {
                type = value;
                TourDirty = true;
            }
        }

        string taxonomy = "";

        public string Taxonomy
        {
            get { return taxonomy; }
            set
            {
                taxonomy = value;
                TourDirty = true;
            }
        }
        string keywords = "";

        public string Keywords
        {
            get { return keywords; }
            set
            {
                keywords = value;
                TourDirty = true;
            }
        }
        string objects = "";

        public string Objects
        {
            get { return objects; }
            set
            {
                objects = value;
                TourDirty = true;
            }
        }
        bool editMode = false;

        public bool EditMode
        {
            get { return editMode; }
            set
            {
                editMode = value;
            }
        }

        public List<String> ExplicitTourLinks = new List<string>();
        public List<String> ImplicitTourLinks = new List<string>();

        List<TourStop> tourStops = new List<TourStop>();

        public List<TourStop> TourStops
        {
            get { return tourStops; }
            set { tourStops = value; }
        }

        public event EventHandler CurrentTourstopChanged;

        int currentTourstopIndex = -1;

        public int CurrentTourstopIndex
        {
            get { return currentTourstopIndex; }
            set 
            {
                if (value < tourStops.Count)
                {
                    if (currentTourstopIndex != value)
                    {
                        currentTourstopIndex = value;
                        if (CurrentTourstopChanged != null)
                        {
                            CurrentTourstopChanged.Invoke(this, new EventArgs());
                        }
                    }
                }
                else
                {
                    currentTourstopIndex = -1;
                }
            }
        }

        public void AddTourStop(TourStop ts)
        {
            ts.Owner = this;

            TourStops.Add(ts);
            currentTourstopIndex = tourStops.Count - 1;
            TourDirty = true;
        }

        public void InsertTourStop(TourStop ts)
        {
            ts.Owner = this;
            if (currentTourstopIndex > -1)
            {
                TourStops.Insert(currentTourstopIndex, ts);
            }
            else
            {
                TourStops.Add(ts);
                currentTourstopIndex = tourStops.Count - 1;
            }
            TourDirty = true;
        }

        public void InsertAfterTourStop(TourStop ts)
        {
            ts.Owner = this;
            if (currentTourstopIndex > -1 || currentTourstopIndex < TourStops.Count)
            {
                TourStops.Insert(currentTourstopIndex+1, ts);
            }
            else
            {
                TourStops.Add(ts);
                currentTourstopIndex = tourStops.Count - 1;
            }
            TourDirty = true;
        }

        public void RemoveTourStop(TourStop ts)
        {
            tourStops.Remove(ts);
            if (currentTourstopIndex > tourStops.Count - 1)
            {
                currentTourstopIndex--;
            }
            TourDirty = true;
        }

        private TimeSpan CalculateRunTime()
        {
            double totalTime = 0.0;
            for (int i = 0; i < tourStops.Count; i++)
            {
                totalTime += (double)(tourStops[i].Duration.TotalMilliseconds / 1000);
                if (i > 0)
                {
                    switch (tourStops[i].Transition)
                    {
                        case TransitionType.Slew:
                            if (tourStops[i].Target.BackgroundImageSet == null || (tourStops[i - 1].Target.BackgroundImageSet.DataSetType == tourStops[i].Target.BackgroundImageSet.DataSetType
                                && ((tourStops[i - 1].Target.BackgroundImageSet.DataSetType != ImageSetType.SolarSystem) || (tourStops[i - 1].Target.Target == tourStops[i].Target.Target))))
                            {
                                CameraParameters start = tourStops[i - 1].EndTarget == null ? tourStops[i - 1].Target.CamParams : tourStops[i - 1].EndTarget.CamParams;
                                ViewMoverSlew slew = new ViewMoverSlew(start, tourStops[i].Target.CamParams);
                                totalTime += slew.MoveTime;
                            }
                            break;
                        case TransitionType.CrossCut:
                            break;
                        case TransitionType.CrossFade:
                            break;
                        case TransitionType.FadeOut:
                            break;
                        default:
                            break;
                    }
                }
            }
            return TimeSpan.FromSeconds(totalTime);
        }

        public double ElapsedTimeTillTourstop(int index)
        {
            if (index == 0 && index >= tourStops.Count)
            {
                return 0;
            }
            double totalTime = 0.0;
            for (int i = 0; i < index; i++)
            {
                totalTime += (double)(tourStops[i].Duration.TotalMilliseconds / 1000);
                if (i > 0)
                {
                    switch (tourStops[i].Transition)
                    {
                        case TransitionType.Slew:
                            CameraParameters start = tourStops[i - 1].EndTarget == null ? tourStops[i - 1].Target.CamParams : tourStops[i - 1].EndTarget.CamParams;
                            if (tourStops[i - 1].Target.BackgroundImageSet.DataSetType == tourStops[i].Target.BackgroundImageSet.DataSetType
                                && ((tourStops[i - 1].Target.BackgroundImageSet.DataSetType != ImageSetType.SolarSystem) || (tourStops[i - 1].Target.Target == tourStops[i].Target.Target)))
                            {
                                ViewMoverSlew slew = new ViewMoverSlew(start, tourStops[i].Target.CamParams);
                                totalTime += slew.MoveTime;
                            }
                            break;
                        case TransitionType.CrossCut:
                            break;
                        case TransitionType.CrossFade:
                            break;
                        case TransitionType.FadeOut:
                            break;
                        default:
                            break;
                    }
                }
            }
            return totalTime;
        }

        public double ElapsedTimeSinceLastMaster(int index, out TourStop masterOut)
        {
            masterOut = null;
            if (index == 0 && index >= tourStops.Count)
            {
                return 0;
            }
            double totalTime = 0.0;
            for (int i = 0; i < index; i++)
            {
                if (tourStops[i].MasterSlide)
                {
                    totalTime = 0;
                    masterOut = tourStops[i];
                }

                totalTime += (double)(tourStops[i].Duration.TotalMilliseconds / 1000);
                if (i > 0 && !tourStops[i].MasterSlide)
                {
                    switch (tourStops[i].Transition)
                    {
                        case TransitionType.Slew:
                            CameraParameters start = tourStops[i - 1].EndTarget == null ? tourStops[i - 1].Target.CamParams : tourStops[i - 1].EndTarget.CamParams;
                            if (tourStops[i - 1].Target.BackgroundImageSet.DataSetType == tourStops[i].Target.BackgroundImageSet.DataSetType
                                && ((tourStops[i - 1].Target.BackgroundImageSet.DataSetType != ImageSetType.SolarSystem) || (tourStops[i - 1].Target.Target == tourStops[i].Target.Target)))
                            {
                                ViewMoverSlew slew = new ViewMoverSlew(start, tourStops[i].Target.CamParams);
                                totalTime += slew.MoveTime;
                            }
                            break;
                        case TransitionType.CrossCut:
                            break;
                        case TransitionType.CrossFade:
                            break;
                        case TransitionType.FadeOut:
                            break;
                        default:
                            break;
                    }
                }
            }
            return totalTime;
        }

        public TourStop GetMasterSlideForIndex(int index)
        {
            int master = -1;
            for (int i = 0; i < index; i++)
            {
                if (tourStops[i].MasterSlide)
                {
                    master = i;
                }
            }
            if (master == -1)
            {
                return null; 
            }

            return tourStops[master];
        }

        public int GetTourStopIndexByID(string id)
        {
            if (id == "" || id == "Next")
            {
                return currentTourstopIndex++;
            }

            int index = 0;
            foreach (TourStop stop in tourStops)
            {
                if (stop.Id == id)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public void CleanUp()
        {
            foreach (TourStop stop in TourStops)
            {
                stop.CleanUp();
            }
            if (textureList != null)
            {
                foreach (Texture11 texture in textureList.Values)
                {
                    if (texture != null)
                    {
                        texture.Dispose();
                        GC.SuppressFinalize(texture);
                    }
                }
                textureList.Clear();
            }
           
        }

        public bool ProjectorServer = false;

        private Dictionary<string, Texture11> textureList;
        public Texture11 GetCachedTexture(string filename, bool colorKey)
        {

            if (textureList == null)
            {
                textureList = new Dictionary<string, Texture11>();
            }

            if (textureList.ContainsKey(filename))
            {
                return textureList[filename];
            }
            Texture11 texture;

            if (ProjectorServer)
            {
                
                // Synce Layers
                try
                {
                    string dir = Path.GetDirectoryName(filename);
                    string name = Path.GetFileName(filename);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    if (!File.Exists(filename))
                    {
                        WebClient client = new WebClient();
                        string url = string.Format("http://{0}:5050/Configuration/images/tour/{1}", NetControl.MasterAddress, name);
                        Byte[] data = client.DownloadData(url);
                        File.WriteAllBytes(filename, data);
                    }
                }
                catch
                {
                }
            }


            if (colorKey)
            {
                //todo11 Add Color Key support
                texture = Texture11.FromFile(filename);
            }
            else
            {
                texture = Texture11.FromFile(filename);
            }
            

            textureList[filename] = texture;

            return texture;

        }


        public TourStop CurrentTourStop
        {
            get
            {
                if (currentTourstopIndex > -1)
                {
                    return TourStops[currentTourstopIndex];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                int i = 0;
                foreach (TourStop stop in TourStops)
	            {
                    if (stop == value)
                    {
                        if (currentTourstopIndex > -1 && currentTourstopIndex != i)
                        {
                            TourStops[currentTourstopIndex].CleanUp();
                        }

                        if (currentTourstopIndex != i)
                        {
                            CurrentTourstopIndex = i;
                            
                        }

                        break;
                    }
            		i++;
	            }
            }
        }
        public bool DontCleanUpTempFiles = false;
        public void ClearTempFiles()
        {
            try
            {
                if (!DontCleanUpTempFiles)
                {
                    DirectoryInfo di = new DirectoryInfo(workingDirectory);
                    if (di.Exists)
                    {
                        foreach (FileInfo fi in di.GetFiles())
                        {
                            File.Delete(fi.FullName);
                        }
                    }
                    Directory.Delete(WorkingDirectory, true);
                    if (!string.IsNullOrEmpty(filename))
                    {
                        File.Delete(filename);
                    }
                }
            }
            catch
            {
            }
        }

        public static void ClearTempDirectory()
        {
            try
            {

                DirectoryInfo di = new DirectoryInfo(Properties.Settings.Default.CahceDirectory + @"Temp\");
                if (di.Exists)
                {
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        File.Delete(fi.FullName);
                    }

                    foreach (DirectoryInfo childDir in di.GetDirectories())
                    {
                        try
                        {
                            childDir.Delete(true);
                        }
                        catch
                        {
                        }
                    }
                }

            }

            catch
            {
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            CleanUp();
            ClearTempFiles();
        }

        #endregion

        internal byte[] GetSlideListXML()
        {
            StringBuilder sb = new StringBuilder();

            XmlWriter xw = XmlWriter.Create(sb);

            xw.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xw.WriteStartElement("SlideList");


            int index = 0;
            foreach (TourStop ts in this.tourStops)
            {
                xw.WriteStartElement("Slide");
                xw.WriteElementString("Index", (index++).ToString());
                xw.WriteElementString("Description", ts.Description);
                xw.WriteElementString("Time", ts.Duration.ToString());
                xw.WriteElementString("Thumbnauil", ts.TourStopThumbnailFilename.Substring(ts.TourStopThumbnailFilename.LastIndexOf("\\") + 1));
                xw.WriteEndElement();
            }
            
            xw.WriteEndElement();
            xw.Close();

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        internal static byte[] GetEmptySlideListXML()
        {
            StringBuilder sb = new StringBuilder();

            XmlWriter xw = XmlWriter.Create(sb);

            xw.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
            xw.WriteStartElement("SlideList");



           xw.WriteStartElement("Slide");
           xw.WriteElementString("Index", "-1");
           xw.WriteElementString("Description", "No Tour Loaded");
           xw.WriteElementString("Time", "0");
           xw.WriteElementString("Thumbnauil", "");
           xw.WriteEndElement();


            xw.WriteEndElement();
            xw.Close();

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
    public class UndoTourSlidelistChange : TerraViewer.IUndoStep
    {
        List<TourStop> undoList;
        List<TourStop> redoList;

        int currentIndex = 0;
        string actionText = "";

        public string ActionText
        {
            get { return actionText; }
            set { actionText = value; }
        }
        TourDocument targetTour = null;
        public UndoTourSlidelistChange(string text, TourDocument tour)
        {
            undoList = new List<TourStop>();

            for (int i = 0; i < tour.TourStops.Count; i++)
            {
                undoList.Add(tour.TourStops[i]);
            }
            
            currentIndex = tour.CurrentTourstopIndex;
            actionText = text;
            targetTour = tour;
            targetTour.TourDirty = true;
        }

        public void Undo()
        {

            redoList = targetTour.TourStops;
            targetTour.TourStops = undoList;
            targetTour.CurrentTourstopIndex = currentIndex;
            targetTour.TourDirty = true;

        }

        public void Redo()
        {
            undoList = targetTour.TourStops;
            targetTour.TourStops = redoList;
            targetTour.CurrentTourstopIndex = currentIndex;
            targetTour.TourDirty = true;
        }

        override public string ToString()
        {
            return actionText;
        }
    }

    public class UndoTourPropertiesChange : TerraViewer.IUndoStep
    {

        string actionText = "";

        public string ActionText
        {
            get { return actionText; }
            set { actionText = value; }
        }
        TourDocument targetTour = null;
        string undoTitle;
        string undoAuthor;
        string undoAuthorEmail;
        string undoDescription;
        Bitmap undoAuthorImage;
        string undoOrganizationUrl;
        string undoOrgName;
        string undoKeywords;
        string undoTaxonomy;
        bool undoDomeMode;
        UserLevel undoLevel;
        string redoTitle;
        string redoAuthor;
        string redoAuthorEmail;
        string redoDescription;
        Bitmap redoAuthorImage;
        string redoOrganizationUrl;
        string redoOrgName;
        string redoKeywords;
        string redoTaxonomy;
        bool redoDomeMode;
        UserLevel redoLevel;
        public UndoTourPropertiesChange(string text, TourDocument tour)
        {
            undoTitle = tour.Title;
            undoAuthor = tour.Author;
            undoAuthorEmail = tour.AuthorEmail;
            undoDescription = tour.Description;
            undoAuthorImage = tour.AuthorImage;
            undoOrganizationUrl = tour.OrganizationUrl;
            undoOrgName = tour.OrgName;
            undoKeywords = tour.Keywords;
            undoTaxonomy = tour.Taxonomy;
            undoLevel = tour.Level;
            undoDomeMode = tour.DomeMode;

            actionText = text;
            targetTour = tour;
            targetTour.TourDirty = true;
        }

        public void Undo()
        {
            redoTitle = targetTour.Title;
            redoAuthor = targetTour.Author;
            redoAuthorEmail = targetTour.AuthorEmail;
            redoDescription = targetTour.Description;
            redoAuthorImage = targetTour.AuthorImage;
            redoOrganizationUrl = targetTour.OrganizationUrl;
            redoOrgName = targetTour.OrgName;
            redoKeywords = targetTour.Keywords;
            redoTaxonomy = targetTour.Taxonomy;
            redoLevel = targetTour.Level;
            redoDomeMode = targetTour.DomeMode;

            targetTour.Title = undoTitle;
            targetTour.Author = undoAuthor;
            targetTour.AuthorEmail = undoAuthorEmail;
            targetTour.Description = undoDescription;
            targetTour.AuthorImage = undoAuthorImage;
            targetTour.OrganizationUrl = undoOrganizationUrl;
            targetTour.OrgName = undoOrgName;
            targetTour.Keywords = undoKeywords;
            targetTour.Taxonomy = undoTaxonomy;
            targetTour.Level = undoLevel;
            targetTour.DomeMode = undoDomeMode;
            targetTour.TourDirty = true;


        }

        public void Redo()
        {
            targetTour.Title = redoTitle;
            targetTour.Author = redoAuthor;
            targetTour.AuthorEmail = redoAuthorEmail;
            targetTour.Description = redoDescription;
            targetTour.AuthorImage = redoAuthorImage;
            targetTour.OrganizationUrl = redoOrganizationUrl;
            targetTour.OrgName = redoOrgName;
            targetTour.Keywords = redoKeywords;
            targetTour.Taxonomy = redoTaxonomy;
            targetTour.Level = redoLevel;    
            targetTour.TourDirty = true;
            targetTour.DomeMode = redoDomeMode;
           

        }

        override public string ToString()
        {
            return actionText;
        }
    }
}
