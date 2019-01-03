
#if WINDOWS_UWP
using SysColor = TerraViewer.Color;
#else
using SysColor = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;
using RectangleF = System.Drawing.RectangleF;
using GraphicsUnit = System.Drawing.GraphicsUnit;
using System.Windows.Forms;
#endif
namespace TerraViewer
{
    public class FinderPanel : RingMenuPanel
    {
        public IPlace target = null;
        public IPlace Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }

        public Texture11 ZoomTexture { get; internal set; }

        public override void Draw(UiGraphics g)
        {
            g.DrawImage(Properties.Resources.PropertiesBackgroundNoFinder, new Rectangle(-10, -10, 400, 535), new Rectangle(0, 0, 292, 315), GraphicsUnit.Pixel, SysColor.FromArgb(128, 255, 255, 255));

            if (Target == null)
            {
                return;
            }

            if (ZoomTexture != null)
            {
                g.DrawImage(ZoomTexture, new Rectangle(0, 210, 380, 300), new Rectangle(0, 0, 506, 450), GraphicsUnit.Pixel);
            }

            g.DrawString(Language.GetLocalizedText(277, "Finder Scope"), 8, SysColor.White, new RectangleF(0, 0, 400, 20), UiGraphics.TextAlignment.Left);
            g.DrawImage(Target.ThumbNail, 0, 21);
            g.DrawString(Target.Name, 12, SysColor.White, new RectangleF(0, 70, 400, 20), UiGraphics.TextAlignment.Left);
            g.DrawString(Language.GetLocalizedText(267, "Classification:"), 8, SysColor.White, new RectangleF(113, 21, 186, 17), UiGraphics.TextAlignment.Left);
            g.DrawString(FriendlyName(Target.Classification.ToString()), 8, SysColor.White, new RectangleF(113, 39, 186, 17), UiGraphics.TextAlignment.Left);
            g.DrawString(Language.GetLocalizedText(280, "in ") + Constellations.FullName(Target.Constellation),
                            8, SysColor.White, new RectangleF(113, 57, 200, 17), UiGraphics.TextAlignment.Left);
            g.DrawString(Language.GetLocalizedText(271, "RA : "), 8, SysColor.White, new RectangleF(6, 122, 31, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Coordinates.FormatHMS(Target.RA), 8, SysColor.White, new RectangleF(41, 122, 78, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Language.GetLocalizedText(270, "Dec : "), 8, SysColor.White, new RectangleF(6, 138, 31, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Coordinates.FormatDMSWide(Target.Dec), 8, SysColor.White, new RectangleF(41, 138, 78, 15), UiGraphics.TextAlignment.Left);

            Coordinates altAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(Target.RA, Target.Dec), SpaceTimeController.Location, SpaceTimeController.Now);

            g.DrawString(Language.GetLocalizedText(269, "Alt : "), 8, SysColor.White, new RectangleF(6, 154, 31, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Coordinates.FormatDMSWide(altAz.Alt), 8, SysColor.White, new RectangleF(41, 154, 78, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Language.GetLocalizedText(268, "Az :"), 8, SysColor.White, new RectangleF(6, 170, 31, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Coordinates.FormatDMSWide(altAz.Az), 8, SysColor.White, new RectangleF(41, 170, 78, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Language.GetLocalizedText(265, "Magnitude:"), 8, SysColor.White, new RectangleF(125, 120, 91, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Target.Magnitude != 0 ? Target.Magnitude.ToString() : Language.GetLocalizedText(281, "n/a"), 8, SysColor.White, new RectangleF(223, 120, 78, 15), UiGraphics.TextAlignment.Left);

            g.DrawString(Language.GetLocalizedText(633, "Distance:"), 8, SysColor.White, new RectangleF(125, 136, 91, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Target.Distance != 0 ? Target.Magnitude.ToString() : Language.GetLocalizedText(281, "n/a"), 8, SysColor.White, new RectangleF(223, 136, 78, 15), UiGraphics.TextAlignment.Left);

            AstroCalc.RiseSetDetails details;
            string riseText;
            string setText;
            string transitText;

            if (target.Classification == Classification.SolarSystem)
            {

                double jNow = ((int)((int)SpaceTimeController.JNow) + .5);
                AstroCalc.AstroRaDec p1 = Planets.GetPlanetLocation(target.Name, jNow - 1);
                AstroCalc.AstroRaDec p2 = Planets.GetPlanetLocation(target.Name, jNow);
                AstroCalc.AstroRaDec p3 = Planets.GetPlanetLocation(target.Name, jNow + 1);

                int type = 0;
                switch (target.Name)
                {
                    case "Sun":
                        type = 1;
                        break;
                    case "Moon":
                        type = 2;
                        break;
                    default:
                        type = 0;
                        break;
                }
                details = AstroCalc.AstroCalc.GetRiseTrinsitSet(jNow, SpaceTimeController.Location.Lat, -SpaceTimeController.Location.Lng, p1.RA, p1.Dec, p2.RA, p2.Dec, p3.RA, p3.Dec, type);
            }
            else
            {
                details = AstroCalc.AstroCalc.GetRiseTrinsitSet(((int)SpaceTimeController.JNow) + .5, SpaceTimeController.Location.Lat, -SpaceTimeController.Location.Lng, target.RA, Target.Dec, target.RA, Target.Dec, target.RA, Target.Dec, 0);
            }

            riseText = details.bValidRise ? UiTools.FormatDecimalHours(details.Rise) : Language.GetLocalizedText(934, "Never Rises");
            transitText = details.bValidTransit ? UiTools.FormatDecimalHours(details.Transit) : Language.GetLocalizedText(934, "Never Rises");
            setText = details.bValidSet ? UiTools.FormatDecimalHours(details.Set) : Language.GetLocalizedText(935, "Never Sets");

            g.DrawString(Language.GetLocalizedText(273, "Rise:"), 8, SysColor.White, new RectangleF(125, 152, 31, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(riseText, 8, SysColor.White, new RectangleF(223, 152, 78, 15), UiGraphics.TextAlignment.Left);

            g.DrawString(Language.GetLocalizedText(275, "Transit:"), 8, SysColor.White, new RectangleF(125, 168, 31, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(transitText, 8, SysColor.White, new RectangleF(223, 168, 78, 15), UiGraphics.TextAlignment.Left);

            g.DrawString(Language.GetLocalizedText(274, "Set:"), 8, SysColor.White, new RectangleF(125, 185, 31, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(setText, 8, SysColor.White, new RectangleF(223, 185, 78, 15), UiGraphics.TextAlignment.Left);

        }

        private string FriendlyName(string name)
        {
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    name = name.Substring(0, i) + " " + name.Substring(i);
                    i++;
                }
            }
            return name;
        }
        public override void Navigate(int upDown, int leftRight)
        {
            base.Navigate(upDown, leftRight);
        }

        public override void Select()
        {
            base.Select();
        }
    }
}
