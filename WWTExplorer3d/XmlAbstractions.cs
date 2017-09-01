using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    static public class XmlAbstractions
    {

#if WINDOWS_UWP
        public static XmlNode GetChildByName(this Windows.Data.Xml.Dom.XmlDocument doc, string name)
        {
            var children = doc.ChildNodes;

            foreach (var child in children)
            {
                if(child.NodeName == name)
                {
                    return new XmlNode(child);
                }
            }

            return null;
        }

        public static void Load(this Windows.Data.Xml.Dom.XmlDocument doc, string filename)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            var asfi = storageFolder.GetFileAsync(filename);

            var aa = asfi.AsTask();
            var file = aa.Result;

            string xml =  Windows.Storage.FileIO.ReadTextAsync(file).AsTask().Result;

            doc.LoadXml(xml);
        }


#else
        public static System.Xml.XmlNode GetChildByName(this System.Xml.XmlDocument doc, string name)
        {
            return doc[name];
        }
#endif
        public static Type BaseType(this Type type)
        {
#if WINDOWS_UWP
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }
    }
}
