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
                if (TourPlayer.Playing)
                {
                    switch(key)
                    {
                        case "LocationLat":
                            return Settings.Active.LocationLat;
                        case "LocationLng":
                            return Settings.Active.LocationLng;
                        case "LocationAltitude":
                            return Settings.Active.LocationAltitude;
                    }
                }

                return Properties.Settings.Default[key];
            }
            set
            {
                Properties.Settings.Default[key] = value;
            }
        }
    }
}
