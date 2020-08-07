using System;
using System.Collections.Generic;
using System.Linq;


namespace TerraViewer
{

    public class XmlTextWriter : IDisposable
    {
        System.IO.StringWriter sw = null;
        System.IO.Stream stream = null;
        public XmlTextWriter(System.IO.StringWriter stringWriter)
        {
            sw = stringWriter;
        }

        public XmlTextWriter(System.IO.Stream streamIn, System.Text.Encoding encoding)
        {
            this.stream = streamIn;
        }

        System.Text.Encoding encoding = System.Text.Encoding.UTF8;

        public XmlTextWriter(string filename, System.Text.Encoding encoding)
        {
            this.encoding = encoding;

            stream = System.IO.File.Create(filename);

        }


        public XmlTextWriter()
        {

        }

        public string Body = "<?xml version='1.0' encoding='UTF-8'?>\r\n";
        public Formatting Formatting = Formatting.Indented;
        Stack<string> elementStack = new Stack<string>();
        bool pending = false;
        string currentName = "";
        Dictionary<string, string> attributes = new Dictionary<string, string>();
        string value = "";
        void PushNewElement(string name)
        {
            //write pending element and attributes

            WritePending(false);

            //Push new attribute on to stack
            elementStack.Push(name);

            //setup pending structures
            pending = true;
            currentName = name;
        }

        private bool WritePending(bool fullClose)
        {
            bool closed = true;
            if (pending)
            {
                for (int i = 1; i < elementStack.Count; i++)
                {
                    Body += "  ";
                }

                Body += "<" + currentName;
                if (attributes.Count > 0)
                {
                    foreach (string key in attributes.Keys)
                    {
                        Body += string.Format(" {0}=\"{1}\"", key, attributes[key]);
                    }
                }

                if (!string.IsNullOrEmpty(value))
                {
                    Body += ">";
                    closed = false;
                    if (!string.IsNullOrEmpty(value))
                    {
                        Body += value;
                    }
                }
                else
                {
                    if (fullClose)
                    {
                        Body += " />\r\n";
                        closed = true;
                    }
                    else
                    {
                        Body += ">\r\n";
                    }
                }

                pending = false;
                currentName = "";
                value = "";
                attributes = new Dictionary<string, string>();
                return closed;
            }

            return false;
        }

        public void WriteProcessingInstruction(string v1, string v2)
        {

        }

        public void WriteStartElement(string name)
        {
            PushNewElement(name);
        }

        public void WriteAttributeString(string key, object value)
        {
            if (value != null)
            {
                attributes[key] = value.ToString().Replace("&", "&amp;");
            }
        }

        public void WriteEndElement()
        {
            if (!WritePending(true))
            {
                for (int i = 1; i < elementStack.Count; i++)
                {
                    Body += "  ";
                }
                Body += string.Format("</{0}>\r\n", elementStack.Pop());
            }
            else
            {
                elementStack.Pop();
            }
        }

        public void WriteString(string text)
        {
            value = text.Replace("&", "&amp;");
        }

        public void WriteFullEndElement()
        {
            WritePending(false);
            for (int i = 1; i < elementStack.Count; i++)
            {
                Body += "  ";
            }
            Body += string.Format("</{0}>\r\n", elementStack.Pop());
        }

        public void Close()
        {
            if (sw != null)
            {
                sw.Write(Body);
            }

            if (stream != null)
            {
                var sw = new System.IO.StreamWriter(stream, encoding);
                sw.Write(Body);
                sw.Dispose();
            }
        }

        public void WriteElementString(string name, string value)
        {
            WriteStartElement(name);
            WriteValue(value.Replace("&", "&amp;"));
            WriteEndElement();
        }

        public void WriteValue(string val)
        {
            value = val.Replace("&", "&amp;");
        }


        public void WriteCData(string htmlDescription)
        {
            value = string.Format("<![CDATA[{0}]]>", htmlDescription);
        }

        public void Dispose()
        {

        }
    }

    public enum Formatting { Indented = 1 };
}