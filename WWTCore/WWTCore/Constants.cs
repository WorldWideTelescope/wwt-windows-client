using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    public class Constants
    {
    }

    public class PulseMe
    {
        public delegate void PulseForUpdateDel();

        public static PulseForUpdateDel PulseForUpdate;

        public static void PulseForSettingsUpdate()
        {
            if (PulseForUpdate != null)
            {
                PulseForUpdate();
            }
        }
    }

    public class AppSettings
    {
        public static ApplicationSettingsBase SettingsBase = null;
    }

    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public enum ImageSetType { Earth, Planet, Sky, Panorama, SolarSystem, Sandbox };


    public enum StockSkyOverlayTypes { Empty, EquatorialGrid, EquatorialGridText, GalacticGrid, GalacticGridText, EclipticGrid, EclipticGridText, EclipticOverview, EclipticOverviewText, PrecessionChart, AltAzGrid, AltAzGridText, ConstellationFigures, ConstellationBoundaries, ConstellationFocusedOnly, ConstellationNames, ConstellationPictures, FadeToBlack, FadeToLogo, FadeToGradient, ScreenBroadcast, FadeRemoteOnly, SkyGrids, Constellations, SolarSystemStars, SolarSystemMilkyWay, SolarSystemCosmos, SolarSystemOrbits, SolarSystemPlanets, SolarSystemAsteroids, SolarSystemLighting, SolarSystemMinorOrbits, ShowEarthCloudLayer, ShowElevationModel, ShowAtmosphere, MultiResSolarSystemBodies, AuroraBorialis, EarthCutAway, Show3dCities, ShowSolarSystem, Clouds8k, FiledOfView, ShowISSModel, SolarSystemCMB, VolumetricMilkyWay, MPCZone1, MPCZone2, MPCZone3, MPCZone4, MPCZone5, MPCZone6, MPCZone7, OrbitFilters };

}
