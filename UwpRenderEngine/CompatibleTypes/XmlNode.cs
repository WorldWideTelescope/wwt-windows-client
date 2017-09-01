using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace TerraViewer
{
    public class XmlNode
    {
        public IXmlNode Node = null;
        internal String Value
        {
            get
            {
                return (string)Node.NodeValue;
            }
        }

        public XmlNode(IXmlNode node)
        {
            Node = node;

        }

        public string Name
        {
            get
            {
                return (string)Node.LocalName;
            }
        }
        public string LocalName
        {
            get
            {
                return (string)Node.LocalName;
            }
        }
        public XmlNode this[string key]
        {
            get
            {
                var children = Node.ChildNodes;
                foreach(var child in children)
                {
                    if ((string)child.LocalName == key)
                    {
                        return new XmlNode(child);
                    }
                }
                return null;
            }
        }

        public XmlNode SelectSingleNode(string query)
        {
            return new XmlNode(Node.SelectSingleNode(query));
        }

        public List<XmlNode> ChildNodes
        {
            get
            {
                List<XmlNode> childList = new List<XmlNode>();
                var children = Node.ChildNodes;
                foreach (var child in children)
                {
                    if (child.LocalName != null)
                    {
                        childList.Add(new XmlNode(child));
                    }
                }
                return childList;
            }
        }

        public XmlNode FirstChild
        {
            get
            {
                return new XmlNode(Node.FirstChild);
            }
        }

        public string InnerText
        {
            get
            {
                return Node.InnerText;
            }
        }

        //todo verify this is correct mapping
        public string InnerXml
        {
            get
            {
                return Node.InnerText;
            }
        }

        public XmlAttrributeList Attributes
        {
            get
            {
                return new XmlAttrributeList(Node.Attributes);
            }
        }
    }

    public class XmlAttrributeList
    {
        public XmlNamedNodeMap Map = null;

        public XmlAttrributeList(XmlNamedNodeMap map)
        {
            Map = map;

        }

        public XmlNode this[string key]
        {
            get
            {
                IXmlNode attribute = Map.GetNamedItem(key);
                if (attribute != null)
                {
                    return new XmlNode(attribute);
                }
                return null;
            }
        }

         public XmlNode GetNamedItem(string key)
        {
            return new XmlNode(Map.GetNamedItem(key));
        }

    }

}
