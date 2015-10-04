namespace TerraViewer
{
    public interface ISettings
    {
        bool ActualPlanetScale { get;  }
        int FovCamera { get;  }
        int FovEyepiece { get;  }
        int FovTelescope { get;  }
        double LocationAltitude { get;  }
        double LocationLat { get;  }
        double LocationLng { get;  }
        bool ShowClouds { get;  }
        bool ShowConstellationBoundries { get;  }
        bool ShowConstellationFigures { get;  }
        bool ShowConstellationSelection { get;  }
        bool ShowEcliptic { get;  }
        bool ShowElevationModel { get;  }
        bool ShowFieldOfView { get;  }
        bool ShowGrid { get;  }
        bool ShowHorizon { get;  }
        bool ShowHorizonPanorama { get;  }
        bool ShowMoonsAsPointSource { get;  }
        bool ShowSolarSystem { get;  }
        bool LocalHorizonMode { get;  }
        bool SolarSystemStars { get;  }
        bool SolarSystemMilkyWay { get;  }
        bool SolarSystemCosmos { get;  }
        bool SolarSystemCMB { get; }
        bool SolarSystemOrbits { get;  }
        bool SolarSystemOverlays { get;  }
        bool SolarSystemLighting { get;  }
        bool SolarSystemMultiRes { get;  }  
        int SolarSystemScale { get;  }
        bool SolarSystemMinorPlanets { get; }
        bool SolarSystemPlanets { get; }
        bool ShowEarthSky { get; }
        bool SolarSystemMinorOrbits { get;  }

        bool ShowEquatorialGridText { get; }
        bool ShowGalacticGrid { get; }
        bool ShowGalacticGridText { get; }
        bool ShowEclipticGrid { get; }
        bool ShowEclipticGridText { get; }
        bool ShowEclipticOverviewText { get; }
        bool ShowAltAzGrid { get; }
        bool ShowAltAzGridText { get; }
        bool ShowPrecessionChart { get; }
        bool ShowConstellationPictures { get; }
        bool ShowConstellationLabels { get; }
        string ConstellationsEnabled { get; }
        ConstellationFilter ConstellationFiguresFilter { get; }
        ConstellationFilter ConstellationBoundariesFilter { get; }
        ConstellationFilter ConstellationNamesFilter { get; }
        ConstellationFilter ConstellationArtFilter { get; }
        bool ShowSkyOverlays { get; }
        bool ShowConstellations { get; }    
        bool ShowSkyNode { get; }
        bool ShowSkyGrids{ get; }
        bool ShowSkyOverlaysIn3d{ get; }
        bool EarthCutawayView{ get; }
        bool ShowISSModel { get; }
        //bool MilkyWayModel { get; }
        bool GalacticMode { get; }

        int MinorPlanetsFilter { get; }
        int PlanetOrbitsFilter { get; }

        SettingParameter GetSetting(StockSkyOverlayTypes type);

    }

    public struct SettingParameter
    {
        public bool TargetState;
        public bool EdgeTrigger;
        public bool Animated;
        public double Opacity;
        public ConstellationFilter Filter;
        public SettingParameter(bool edgeTrigger, double opacity, bool targetState, ConstellationFilter filter)
        {
            EdgeTrigger = edgeTrigger;
            Opacity = opacity;
            TargetState = targetState;
            Filter = filter;
            Animated = false;
        }
    }
}
