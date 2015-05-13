using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Xml;



namespace TerraViewer
{
    public class LayerContainer : IDisposable
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

        public LayerContainer()
        {
            id = Guid.NewGuid().ToString();
        }

        public Guid SoloGuid = Guid.Empty;

        public string TopLevel = "";

        string workingDirectory = "";
        public string WorkingDirectory
        {
            get
            {
                if (String.IsNullOrEmpty(workingDirectory))
                {
                    workingDirectory = LayerContainer.BaseWorkingDirectory + id.ToString() + @"\";
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
                return Properties.Settings.Default.CahceDirectory + @"LayerTemp\";
            }

        }

        public static LayerContainer FromFile(string filename, bool forEdit, string parentFrame, bool referenceFrameRightClick)
        {
            FileCabinet cab = new FileCabinet(filename, BaseWorkingDirectory);
            cab.Extract();
            LayerContainer newDoc = LayerContainer.FromXml(cab.MasterFile, parentFrame,  referenceFrameRightClick);
            if (forEdit)
            {
                newDoc.SaveFileName = filename;
            }

            return newDoc;
        }

        string saveFileName = null;

        public string SaveFileName
        {
            get { return saveFileName; }
            set { saveFileName = value; }
        }

        public bool OverWrite = false;
        public bool CollisionChecked = false;
        public static LayerContainer FromXml(string filename, string parentFrame, bool referenceFrameRightClick)
        {
            LayerContainer newDoc = new LayerContainer();
            newDoc.filename = filename;
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            XmlNode root = doc["LayerContainer"];
            newDoc.id = root.Attributes["ID"].Value.ToString();

            XmlNode Layers = root["Layers"];

            bool loadAtNewParent = false;
            if (!referenceFrameRightClick || Layers.ChildNodes.Count != 1)
            {
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
            }
            else
            {
                loadAtNewParent = true;
            }


            if (Layers != null)
            {
                foreach (XmlNode layer in Layers)
                {
                    Layer newLayer = Layer.FromXml(layer, true);
                    string fileName = newDoc.WorkingDirectory + string.Format("{0}.txt", newLayer.ID.ToString());


                    if (LayerManager.LayerList.ContainsKey(newLayer.ID) && newLayer.ID != ISSLayer.ISSGuid)
                    {
                        if (!newDoc.CollisionChecked)
                        {
                            if (UiTools.ShowMessageBox(Language.GetLocalizedText(958, "There are layers with the same name. Overwrite existing layers?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                            {
                                newDoc.OverWrite = true;
                            }
                            else
                            {
                                newDoc.OverWrite = false;

                            }
                            newDoc.CollisionChecked = true;
                        }

                        if (newDoc.OverWrite)
                        {
                            LayerManager.DeleteLayerByID(newLayer.ID, true, false);
                        }
                    }

                    newLayer.LoadData(fileName);

                    if (loadAtNewParent)
                    {
                        newLayer.ReferenceFrame = parentFrame;
                    }

                    LayerManager.Add(newLayer, false);
                    newDoc.LastLoadedLayer = newLayer;

                }
                LayerManager.LoadTree();
            }

            newDoc.tourDirty = 0;
            return newDoc;

        }

        public Layer LastLoadedLayer = null;

        public string TempFilename
        {
            get
            {
                return Properties.Settings.Default.CahceDirectory + @"LayerTemp\" + id.ToString() + "\\" + id.ToString() + ".wwtxml";
            }
        }

        public void SaveToXml(bool forExport)
        {
            string outFile;
            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + @"LayerTemp\"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + @"LayerTemp\");
            }

            if (!forExport)
            {
                //Use a guid if the Title is only non-legal characters or is empty
                if (String.IsNullOrEmpty(filename))
                {
                    filename = Properties.Settings.Default.CahceDirectory + @"LayerTemp\" + id.ToString() + ".wwtxml";
                }
                outFile = filename;
            }
            else
            {
                
                 outFile = TempFilename;
                
            }

            using (XmlTextWriter xmlWriter = new XmlTextWriter(outFile, System.Text.Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                xmlWriter.WriteStartElement("LayerContainer");
                xmlWriter.WriteAttributeString("ID", this.id);
              
                List<Guid> masterList = null;

                if (!string.IsNullOrEmpty(TopLevel))
                {
                    masterList = CreateLayerMasterListFromFrame(TopLevel);
                }
                else
                {
                    masterList = CreateLayerMasterList();
                }
               
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

            foreach (Layer layer in LayerManager.LayerList.Values)
            {
                if (SoloGuid == Guid.Empty || SoloGuid == layer.ID)
                {
                    masterList.Add(layer.ID);
                }
            }

            return masterList;
        }

        private List<Guid> CreateLayerMasterListFromFrame(string name)
        {
            List<Guid> masterList = new List<Guid>();

            LayerMap map = LayerManager.AllMaps[name];
            if (map != null)
            {
                foreach (Layer layer in LayerManager.LayerList.Values)
                {
                    if (SoloGuid == Guid.Empty || SoloGuid == layer.ID)
                    {
                        masterList.Add(layer.ID);
                    }
                }
            }
            return masterList;
        }

        public void AddMaps(LayerMap map, List<Guid> list)
        {
            foreach(Layer layer in map.Layers)
            {
                list.Add(layer.ID);
            }

            foreach(LayerMap child in map.ChildMaps.Values)
            {
                AddMaps(child,list);
            }
        }


        public void SaveToFile(string saveFilename)
        {
            GC.Collect();
            SaveToXml(false);

            FileCabinet fc = new FileCabinet(saveFilename, BaseWorkingDirectory);
            fc.PackageID = this.Id;
            this.saveFileName = saveFilename;
            fc.AddFile(filename);
 
            List<Guid> masterList = CreateLayerMasterList();

            string path = fc.TempDirectory + string.Format("{0}", fc.PackageID);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }



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

        string id;

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

     

        String filename;

        public String FileName
        {
            get { return filename; }
            set { filename = value; }
        }

     
        public bool DontCleanUpTempFiles = false;
        public void ClearTempFiles()
        {
            try
            {
                if (!DontCleanUpTempFiles)
                {
                    DirectoryInfo di = new DirectoryInfo(WorkingDirectory);
                    if (di.Exists)
                    {
                        foreach (FileInfo fi in di.GetFiles())
                        {
                            File.Delete(fi.FullName);
                        }
                    }
                    Directory.Delete(WorkingDirectory, true);
                    File.Delete(filename);
                }
            }
            catch
            {
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
           
            ClearTempFiles();
        }

        #endregion
    }
    
}
