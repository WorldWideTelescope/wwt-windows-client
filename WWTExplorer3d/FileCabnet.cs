using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

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
        private int currentOffset;
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
            var fi = new FileInfo(filename);
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
                var fe = new FileEntry(filePart, (int)fi.Length);
                fe.Offset = currentOffset;
                FileList.Add(fe);
                FileDirectory.Add(filePart, fe);
                currentOffset += fe.Size;
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
            var sw = new StringWriter();
            using (var xmlWriter = new XmlTextWriter(sw))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                xmlWriter.WriteStartElement("FileCabinet");
                xmlWriter.WriteAttributeString("HeaderSize", "0x0BADFOOD");

                xmlWriter.WriteStartElement("Files");
                foreach (var entry in FileList)
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

            var data = sw.ToString();
  
            var header = Encoding.UTF8.GetBytes(data);

            var sizeText = String.Format("0x{0:x8}", header.Length);

            data = data.Replace("0x0BADFOOD", sizeText);

            // Yeah this looks redundant, but we needed the data with the replaced size
            header = Encoding.UTF8.GetBytes(data);

            var output = new FileStream(Filename, FileMode.Create);

            // Write Header
            output.Write(header, 0, header.Length);



            // Write each file
            foreach (var entry in FileList)
            {
                using (var fs = new FileStream(TempDirectory + "\\" + entry.Filename, FileMode.Open, FileAccess.Read))
                {
                    var buffer = new byte[entry.Size];
                    if (fs.Read(buffer, 0, entry.Size) != entry.Size)
                    {
                        throw new SystemException(Language.GetLocalizedText(214, "One of the files in the collection is missing, corrupt or inaccessable"));
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
                var doc = new XmlDocument();
                using (var fs = File.OpenRead(Filename))
                {

                    var buffer = new byte[256];
                    fs.Read(buffer, 0, 255);
                    string data = Encoding.UTF8.GetString(buffer);

                    var start = data.IndexOf("0x");
                    if (start == -1)
                    {
                        throw new SystemException(Language.GetLocalizedText(215, "Invalid File Format"));
                    }
                    int headerSize = Convert.ToInt32(data.Substring(start, 10), 16);

                    fs.Seek(0, SeekOrigin.Begin);


                    buffer = new byte[headerSize];
                    fs.Read(buffer, 0, headerSize);
                    data = Encoding.UTF8.GetString(buffer);
                    doc.LoadXml(data);

                    XmlNode cab = doc["FileCabinet"];
                    XmlNode files = cab["Files"];

                    var offset = headerSize;
                    FileList.Clear();
                    foreach (XmlNode child in files.ChildNodes)
                    {
                        var fe = new FileEntry(child.Attributes["Name"].Value, Convert.ToInt32(child.Attributes["Size"].Value));
                        fe.Offset = offset;
                        offset += fe.Size;
                        FileList.Add(fe);
                    }

                    foreach (var entry in FileList)
                    {
                        while (entry.Filename.StartsWith("\\"))
                        {
                            entry.Filename = entry.Filename.Substring(1);
                        }
                        var fullPath = TempDirectory + @"\" + entry.Filename;
                        var dir = fullPath.Substring(0, fullPath.LastIndexOf("\\"));
                        var file = fullPath.Substring(fullPath.LastIndexOf("\\") + 1);

                        if (!string.IsNullOrEmpty(targetDirectory) && entry.Filename.Contains("\\"))
                        {
                            fullPath = targetDirectory + "\\" + file;
                            dir = targetDirectory;
                        }

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        if (overwrite || !File.Exists(fullPath))
                        {
                            var fileOut = new FileStream(fullPath, FileMode.Create);
                            buffer = new byte[entry.Size];
                            fs.Seek(entry.Offset, SeekOrigin.Begin);
                            if (fs.Read(buffer, 0, entry.Size) != entry.Size)
                            {
                                throw new SystemException(Language.GetLocalizedText(214, "One of the files in the collection is missing, corrupt or inaccessable"));
                            }
                            fileOut.Write(buffer, 0, entry.Size);
                            fileOut.Close();
                        }
                    }


                    var x = offset;
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
                return null;
            }
        }
        public void ClearTempFiles()
        {
            foreach (var entry in FileList)
            {
                File.Delete(TempDirectory +"\\" + entry.Filename);
            }
        }

    }
}
