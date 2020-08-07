using System;
using System.Collections.Generic;

namespace TerraViewer
{
#if WINDOWS_UWP
    using VoTable= System.Object;
#endif
    public interface IImageSet
    {
        string DemUrl { get; set; }
        BandPass BandPass { get; set; }
        int BaseLevel { get; set; }
        double BaseTileDegrees { get; set; }
        bool BottomsUp { get; set; }
        double CenterX { get; set; }
        double CenterY { get; set; }
        string CreditsText { get; set; }
        string CreditsUrl { get; set; }
        ImageSetType DataSetType { get; set; }
        bool DefaultSet { get; set; }
        bool ElevationModel { get; set; }
        string Extension { get; set; }
        bool Generic { get; set; }
        int GetHash();
        int ImageSetID { get; set; }
        bool IsMandelbrot { get; }
        int Levels { get; set; }
        Matrix3d Matrix { get; set; }
        bool Mercator { get; set; }
        string Name { get; set; }
        double OffsetX { get; set; }
        double OffsetY { get; set; }
        ProjectionType Projection { get; set; }
        string QuadTreeTileMap { get; set; }
        double Rotation { get; set; }
        bool Sparse { get; set; }
        IImageSet StockImageSet { get; }
        string ThumbnailUrl { get; set; }
        string Url { get; set; }
        double WidthFactor { get; set; }
        Object WcsImage { get; set; }
        double MeanRadius { get; set; }
        string ReferenceFrame { get; set; }
        UInt16 InternalID { get; set; }
        Dictionary<string, string> Properties { get; set; }
        VoTable TableMetadata { get; set; }
    }
}
