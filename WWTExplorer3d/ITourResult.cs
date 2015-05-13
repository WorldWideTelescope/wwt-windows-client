using System;
using System.Drawing;
namespace TerraViewer
{
    public interface ITourResult
    {
        string AttributesAndCredits { get; set; }
        string Author { get; set; }
        string AuthorContactText { get; set; }
        string AuthorEmail { get; set; }
        string AuthorEmailOther { get; set; }
        System.Drawing.Bitmap AuthorImage { get; set; }
        string AuthorUrl { get; set; }
        double AverageUserRating { get; set; }
        string Description { get; set; }
        string Id { get; set; }
        string OrganizationUrl { get; set; }
        string OrgName { get; set; }
        string OrgUrl { get; set; }
        System.Drawing.Bitmap ThumbNail { get; set; }
        string Title { get; set; }
        Rectangle Bounds { get; set;}
        double LengthInSeconds { get; set;}
        string TourUrl { get; set; }
        string AuthorImageUrl { get; set; }
        string ThumbnailUrl { get; set; }
        string Keywords { get; set; }
        string RelatedTours { get; set; }
    }
}
