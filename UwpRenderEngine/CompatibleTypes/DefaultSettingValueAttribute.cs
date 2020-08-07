using System;

namespace TerraViewer.Properties
{
    internal class DefaultSettingValueAttribute : Attribute
    {
        public string DefaultValue;
        public DefaultSettingValueAttribute(string defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}