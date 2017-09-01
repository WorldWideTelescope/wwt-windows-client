using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    class SettingsWrapper : IAppSettings
    {
        object IAppSettings.this[string key]
        {
            get
            {
                return Properties.Settings.Default[key];
            }
            set
            {
                Properties.Settings.Default[key] = value;
            }
        }
    }
}
