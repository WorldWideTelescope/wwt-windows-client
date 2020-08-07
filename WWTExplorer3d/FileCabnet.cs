using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if WINDOWS_UWP
using XmlElement = Windows.Data.Xml.Dom.XmlElement;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;
#else
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Drawing;
using System.Xml;
#endif
namespace TerraViewer
{
    public class FileEntry
    {
        public string Filename;
        public int Size;
        public int Offset;
        public FileEntry(string filename, int size)
        {
            Filename = filename;
            Size = size;
        }
        public override string ToString()
        {
            return Filename;
        }
    }

    public class FileCabinet
    {
        protected List<FileEntry> FileList;
        Dictionary<string, FileEntry> FileDirectory;
        public string Filename;
        public string TempDirectory;
        private int currentOffset = 0;
        private string packageID = "";

        public string PackageID
        {
            get { return packageID; }
            set { packageID = value; }
        }

        public FileCabinet(string filename, string directory)
        {
            ClearFileList();
            Filename = filename;
            TempDirectory = directory;
        }


        public void AddFile(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            string filePart;
            if (filename.ToLower().StartsWith(TempDirectory.ToLower()))
            {
                filePart = filename.Substring(TempDirectory.Length);
            }
            else
            {
                filePart = filename.Substring(filename.LastIndexOf(@"\") + 1);
            }

            if (!FileDirectory.ContainsKey(filePart))
            {
                try
                {
                    FileEntry fe = new FileEntry(filePart, (int)fi.Length);
                    fe.Offset = currentOffset;
                    FileList.Add(fe);
                    FileDirectory.Add(filePart, fe);
                    currentOffset += fe.Size;
                }
                catch
                {
                  //UiTools.ShowMessageBox("File not found: " + filePart);
                }
            }
        }

        public void ClearFileList()
        {
            if (FileList == null)
            {
                FileList = new List<FileEntry>();
            }
            if (FileDirectory == null)
            {
                FileDirectory = new Dictionary<string, FileEntry>();
            }
            FileList.Clear();
            FileDirectory.Clear();
            currentOffset = 0;
        }
        
        public void Package()
        {
            StringWriter sw = new StringWriter();
            using (XmlTextWriter xmlWriter = new XmlTextWriter(sw))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                xmlWriter.WriteStartElement("FileCabinet");
                xmlWriter.WriteAttributeString("HeaderSize", "0x0BADFOOD");

                xmlWriter.WriteStartElement("Files");
                foreach (FileEntry entry in FileList)
                {
                    xmlWriter.WriteStartElement("File");
                    xmlWriter.WriteAttributeString("Name", entry.Filename);
                    xmlWriter.WriteAttributeString("Size", entry.Size.ToString());
                    xmlWriter.WriteAttributeString("Offset", entry.Offset.ToString());
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xmlWriter.WriteFullEndElement();
                xmlWriter.Close();

            }

            string data = sw.ToString();
  
            byte[] header = Encoding.UTF8.GetBytes(data);

            string sizeText = String.Format("0x{0:x8}", header.Length);

            data = data.Replace("0x0BADFOOD", sizeText);

            // Yeah this looks redundant, but we needed the data with the replaced size
            header = Encoding.UTF8.GetBytes(data);

            FileStream output = new FileStream(Filename, FileMode.Create);

            // Write Header
            output.Write(header, 0, header.Length);



            // Write each file
            foreach (FileEntry entry in FileList)
            {
                using (FileStream fs = new FileStream(TempDirectory + "\\" + entry.Filename, FileMode.Open, FileAccess.Read))
                {
                    byte[] buffer = new byte[entry.Size];
                    if (fs.Read(buffer, 0, entry.Size) != entry.Size)
                    {
                        
                        throw new System.Exception(Language.GetLocalizedText(214, "One of the files in the collection is missing, corrupt or inaccessable"));
                    }
                    output.Write(buffer, 0, entry.Size);
                    fs.Close();
                }
            }

            output.Close();

        }

        public void Extract()
        {
            Extract(true, null);

        }

        public void Extract(bool overwrite, string targetDirectory)
        {
            try
            {
                string data;
                XmlDocument doc = new XmlDocument();
                int headerSize = 0;
                using (FileStream fs = File.OpenRead(Filename))
                {

                    byte[] buffer = new byte[256];
                    fs.Read(buffer, 0, 255);
                    data = Encoding.UTF8.GetString(buffer);

                    int start = data.IndexOf("0x");
                    if (start == -1)
                    {
                        throw new System.Exception(Language.GetLocalizedText(215, "Invalid File Format"));
                    }
                    headerSize = Convert.ToInt32(data.Substring(start, 10), 16);

                    fs.Seek(0, SeekOrigin.Begin);


                    buffer = new byte[headerSize];
                    fs.Read(buffer, 0, headerSize);
                    data = Encoding.UTF8.GetString(buffer);
                    doc.LoadXml(data);

                    XmlNode cab = doc.GetChildByName("FileCabinet");
                    XmlNode files = cab["Files"];

                    int offset = headerSize;
                    FileList.Clear();
                    foreach (XmlNode child in files.ChildNodes)
                    {
                        FileEntry fe = new FileEntry(child.Attributes["Name"].Value.ToString(), Convert.ToInt32(child.Attributes["Size"].Value));
                        fe.Offset = offset;
                        offset += fe.Size;
                        FileList.Add(fe);
                    }
                    bool isMaster = !overwrite;
                    foreach (FileEntry entry in FileList)
                    {
                        if (isMaster)
                        {
                       //     entry.Filename = Guid.NewGuid().ToString() + ".xml";
                        }

                        while (entry.Filename.StartsWith("\\"))
                        {
                            entry.Filename = entry.Filename.Substring(1);
                        }
                        string fullPath = TempDirectory + @"\" + entry.Filename;
                        string dir = fullPath.Substring(0, fullPath.LastIndexOf("\\"));
                        string file = fullPath.Substring(fullPath.LastIndexOf("\\") + 1);

                        if (!string.IsNullOrEmpty(targetDirectory) && entry.Filename.Contains("\\"))
                        {
                            fullPath = targetDirectory + "\\" + file;
                            dir = targetDirectory;
                        }

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        //// if we are merging alias the old GUID based tour file to a new one.
                        //if (isMaster && !overwrite && File.Exists(fullPath))
                        //{
                        //    Guid newFileGuid = Guid.NewGuid();

                        //    file = newFileGuid.ToString() + ".xml";

                        //    fullPath.

                        //}

                        if (overwrite || !File.Exists(fullPath))
                        {
                            FileStream fileOut = new FileStream(fullPath, FileMode.Create);
                            buffer = new byte[entry.Size];
                            fs.Seek(entry.Offset, SeekOrigin.Begin);
                            if (fs.Read(buffer, 0, entry.Size) != entry.Size)
                            {
                                throw new System.Exception(Language.GetLocalizedText(214, "One of the files in the collection is missing, corrupt or inaccessable"));
                            }
                            fileOut.Write(buffer, 0, entry.Size);
                            fileOut.Close();
                        }

                        isMaster = false;
                    }


                    int x = offset;
                    fs.Close();
                }
            }
            catch
            {
              //  UiTools.ShowMessageBox("The data cabinet file was not found. WWT will now download all data from network.");
            }

        }

        public string MasterFile
        {
            get
            {
                if (FileList.Count > 0)
                {
                    return TempDirectory + FileList[0].Filename.Substring(FileList[0].Filename.LastIndexOf("\\")+1);
                }
                else
                {
                    return null;
                }
            }
        }
        public void ClearTempFiles()
        {
            foreach (FileEntry entry in FileList)
            {
                File.Delete(TempDirectory +"\\" + entry.Filename);
            }
        }

    }
}
