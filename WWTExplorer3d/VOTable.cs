using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace TerraViewer
{
    public enum Primitives { VoBoolean, VoBit, VoUnsignedByte, VoShort, VoInt, VoLong, VoChar, VoUnicodeChar, VoFloat, VoDouble, VoFloatComplex, VoDoubleComplex , VoUndefined};

    public class VoTable
    {
        public Dictionary<string, VoColumn> Columns = new Dictionary<string, VoColumn>();
        public List<VoColumn> Column = new List<VoColumn>();
        public List<VoRow> Rows = new List<VoRow>();
        public string LoadFilename = "";
        public string Url;
        public string SampId = "";
        public VoRow SelectedRow = null;
        public VoTable(XmlDocument xml)
        {
            LoadFromXML(xml);
        }

        public VoTable(string filename)
        {
            LoadFilename = filename;
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            LoadFromXML(doc);
        }
        public bool error = false;
        public string errorText = "";
        public void LoadFromXML(XmlDocument xml)
        {
            XmlNode voTable = xml["VOTABLE"];

            if (voTable == null)
            {
                return;
            }
            int index = 0;
            try
            {
                XmlNode table = voTable["RESOURCE"]["TABLE"];
                if (table != null)
                {
                    foreach (XmlNode node in table.ChildNodes)
                    {
                        if (node.Name == "FIELD")
                        {
                            VoColumn col = new VoColumn(node, index++);
                            Columns.Add(col.Name, col);
                            Column.Add(col);
                        }
                    }
                }
            }
            catch
            {
                error = true;
                errorText = voTable["DESCRIPTION"].InnerText.ToString();
            }
            try
            {
                XmlNode tableData = voTable["RESOURCE"]["TABLE"]["DATA"]["TABLEDATA"];
                if (tableData != null)
                {
                    foreach (XmlNode node in tableData.ChildNodes)
                    {
                        if (node.Name == "TR")
                        {
                            VoRow row = new VoRow(this);
                            row.ColumnData = new object[Columns.Count];
                            index = 0;
                            foreach (XmlNode child in node.ChildNodes)
                            {
                                row.ColumnData[index++] = child.InnerText.Trim();
                            }
                            Rows.Add(row);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public bool Save(string filename)
        {
            if (String.IsNullOrEmpty(filename) || String.IsNullOrEmpty(LoadFilename))
            {
                return false;
            }
            try
            {
                File.Copy(LoadFilename, filename);
            }
            catch
            {
                return false;
            }
            return true;

        }
        public VoColumn GetColumnByUcd(string ucd)
        {
            foreach (VoColumn col in this.Columns.Values)
            {
                if (col.Ucd.Replace("_", ".").ToLower().Contains(ucd.ToLower()))
                {
                    return col;
                }
            }
            return null;
        }

        public VoColumn GetRAColumn()
        {
            foreach (VoColumn col in this.Columns.Values)
            {
                if (col.Ucd.ToLower().Contains("pos.eq.ra") || col.Ucd.ToLower().Contains("pos_eq_ra"))
                {
                    return col;
                }
            }
            foreach (VoColumn col in this.Columns.Values)
            {
                if (col.Name.ToLower().Contains("ra"))
                {
                    return col;
                }
            }

            return null;
        }

        public VoColumn GetDecColumn()
        {
            foreach (VoColumn col in this.Columns.Values)
            {
                if (col.Ucd.ToLower().Contains("pos.eq.dec") || col.Ucd.ToLower().Contains("pos_eq_dec"))
                {
                    return col;
                }
            }

            foreach (VoColumn col in this.Columns.Values)
            {
                if (col.Name.ToLower().Contains("dec"))
                {
                    return col;
                }
            }
            return null;
        }

        public VoColumn GetDistanceColumn()
        {
            foreach (VoColumn col in this.Columns.Values)
            {
                if (col.Ucd.ToLower().Contains("pos.distance") || col.Ucd.ToLower().Contains("pos_distance"))
                {
                    return col;
                }
            }
            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            bool first = true;
            // Copy header
            foreach (VoColumn col in this.Columns.Values)
            {
                if (first)
                {
                     first = false;
                }
                else
                {
                   sb.Append("\t");
                }

                sb.Append(col.Name);
            }
            sb.AppendLine("");

            // copy rows

            foreach (VoRow row in Rows)
            {
                first = true;
                foreach (object col in row.ColumnData)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        sb.Append("\t");
                    }

                    sb.Append(col.ToString());
                }
                sb.AppendLine("");
            }
            return sb.ToString();
        }

    }

    public class VoRow
    {
        public bool Selected = false;
        public VoTable Owner;
        public object[] ColumnData;
        public VoRow(VoTable owner)
        {
            Owner = owner;
        }
        public object this[int index]
        {
            get
            {
                if (index < 0 || index >= ColumnData.GetLength(0))
                {
                    return null;
                }
                return ColumnData[index];
            }
        }
        public object this[string key]
        {
            get
            {
                if (Owner.Columns[key] != null)
                {
                    return ColumnData[Owner.Columns[key].Index];
                }
                return null;
            }
        }
    }

    public class VoColumn
    {
        public VoColumn(XmlNode node, int index)
        {
            Index = index;
            if (node.Attributes["datatype"] != null)
            {
                this.Type = GetType(node.Attributes["datatype"].Value);
            }
            if (node.Attributes["ucd"] != null)
            {
                this.Ucd = node.Attributes["ucd"].Value;
            }
            if (node.Attributes["precision"] != null)
            {
                try
                {
                    this.Precision = Convert.ToInt32(node.Attributes["precision"].Value);
                }
                catch
                {
                }
            }
            if (node.Attributes["ID"] != null)
            {
                this.Id = node.Attributes["ID"].Value;
            }       
            
            if (node.Attributes["name"] != null)
            {
                this.Name = node.Attributes["name"].Value;
            }
            else
            {
                this.Name = this.Id;
            }

            if (node.Attributes["unit"] != null)
            {
                this.Unit = node.Attributes["unit"].Value;
            }

            
            if (node.Attributes["arraysize"] != null)
            {
                string[] split = node.Attributes["arraysize"].Value.Split(new char[] { 'x' });
                Dimentions = split.GetLength(0);
                Sizes = new int[split.GetLength(0)];
                int indexer = 0;
                foreach (string dim in split)
                {
                    if (!dim.Contains("*"))
                    {
                        Sizes[indexer++] = Convert.ToInt32(dim);
                    }
                    else
                    {
                        int len = 9999;
                        string lenString = dim.Replace("*","");
                        if (lenString.Length > 0)
                        {
                            len = Convert.ToInt32(lenString);
                        }
                        Sizes[indexer++] = len;
                        
                    }
                }
            }

        }
        public string Id = "";
        public Primitives Type;
        public int Precision = 0;
        public int Dimentions = 0;
        public int[] Sizes = null;
        public string Ucd = "";
        public string Unit = "";
        public string Name = "";
        public int Index;

        public static Primitives GetType(string type)
        {
            Primitives Type = Primitives.VoUndefined;
            switch (type)
            {
                case "boolean":
                    Type = Primitives.VoBoolean;
                    break;
                case "bit":
                    Type = Primitives.VoBit;
                    break;
                case "unsignedByte":
                    Type = Primitives.VoUnsignedByte;
                    break;
                case "short":
                    Type = Primitives.VoShort;
                    break;
                case "int":
                    Type = Primitives.VoInt;
                    break;
                case "long":
                    Type = Primitives.VoLong;
                    break;
                case "char":
                    Type = Primitives.VoChar;
                    break;
                case "unicodeChar":
                    Type = Primitives.VoUnicodeChar;
                    break;
                case "float":
                    Type = Primitives.VoFloat;
                    break;
                case "double":
                    Type = Primitives.VoDouble;
                    break;
                case "floatComplex":
                    Type = Primitives.VoFloatComplex;
                    break;
                case "doubleComplex":
                    Type = Primitives.VoDoubleComplex;
                    break;
                default:
                    Type = Primitives.VoUndefined;
                    break;

            }
            return Type;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
