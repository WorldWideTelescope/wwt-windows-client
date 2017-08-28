#if WINDOWS_UWP
using System;

namespace TerraViewer
{
    public class SerializableAttribute : Attribute
    {
    }

    public class XmlTypeAttribute : Attribute
    {
        public bool AnonymousType { get; set; }
    }

}
#endif