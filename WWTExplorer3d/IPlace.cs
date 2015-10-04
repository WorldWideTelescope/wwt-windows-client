using System;
using System.Xml.Serialization;

namespace TerraViewer
{
    [Serializable]
    [XmlType(AnonymousType = true)]
    public enum Classification
    {
        Star = 1,
        Supernova = 2,
        BlackHole = 4,
        NeutronStar = 8,
        DoubleStar = 16,
        MultipleStars = 32,
        Asterism = 64,
        Constellation = 128,
        OpenCluster = 256,
        GlobularCluster = 512,
        NebulousCluster = 1024,
        Nebula = 2048,
        EmissionNebula = 4096,
        PlanetaryNebula = 8192,
        ReflectionNebula = 16384,
        DarkNebula = 32768,
        GiantMolecularCloud = 65536,
        SupernovaRemnant = 131072,
        InterstellarDust = 262144,
        Quasar = 524288,
        Galaxy = 1048576,
        SpiralGalaxy = 2097152,
        IrregularGalaxy = 4194304,
        EllipticalGalaxy = 8388608,
        Knot = 16777216,
        PlateDefect = 33554432,
        ClusterOfGalaxies = 67108864,
        OtherNGC = 134217728,
        Unidentified = 268435456,
        SolarSystem = 536870912,
        Unfiltered = 1073741823,
        Stellar = 63,
        StellarGroupings = 2032,
        Nebulae = 523264,
        Galactic = 133693440,
        Other = 436207616
    };
    [Serializable]
    [XmlType(AnonymousType = true)]
    public enum ProjectionType { Mercator, Equirectangular, Tangent, Tan = Tangent, Toast, Spherical, SkyImage, Plotted };
 
    
    [Serializable]
    [XmlType(AnonymousType = true)]
    public enum BandPass { Gamma, XRay, Ultraviolet, Visible, HydrogenAlpha, IR, Microwave, Radio, VisibleNight };

    public interface IPlace : IThumbnail
    {
        IImageSet BackgroundImageSet { get; set; }
        IImageSet StudyImageset { get; set; }
        CameraParameters CamParams { get; set; }
        double Dec { get; set; }
        double Lat { get; set; }
        double Lng { get; set; }
        Vector3d Location3d { get; }
        string[] Names { get; set;}
        double Opacity { get; set; }
        double RA { get; set; }
        double ZoomLevel { get; set; }
        double SearchDistance { get; set; }
        Classification Classification { get; set; }
        ImageSetType Type { get; set; }
        string Constellation { get; set; }
        double Magnitude { get; set; }
        double Distance { get; set; }
        string Url { get; set; }
        string Thumbnail { get; set;}
        SolarSystemObjects Target { get; set;}
        object Tag { get; set; }
    }
}
