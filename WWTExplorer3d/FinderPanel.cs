
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
        public IPlace Target = null;

        public override void Draw(UiGraphics g)
        {
            g.DrawImage(Properties.Resources.PropertiesBackgroundNoFinder, new Rectangle(-10, -10, 400, 535), new Rectangle(0, 0, 292, 315), GraphicsUnit.Pixel, SysColor.FromArgb(128, 255, 255, 255));

            if (Target == null)
            {
                return;
            }

            g.DrawString(Language.GetLocalizedText(277, "Finder Scope"), 8, SysColor.White, new RectangleF(0, 0, 400, 20), UiGraphics.TextAlignment.Left);
            g.DrawImage(Target.ThumbNail, 0, 21);
            g.DrawString(Target.Name, 12, SysColor.White, new RectangleF(0, 70, 400, 20), UiGraphics.TextAlignment.Left);
            g.DrawString(Language.GetLocalizedText(267, "Classification:"), 8, SysColor.White, new RectangleF(113, 21, 186, 17), UiGraphics.TextAlignment.Left);
            g.DrawString(FriendlyName(Target.Classification.ToString()), 8, SysColor.White, new RectangleF(113, 39, 186, 17), UiGraphics.TextAlignment.Left);
            g.DrawString(Language.GetLocalizedText(280, "in ") + Constellations.FullName(Target.Constellation),
                            8, SysColor.White, new RectangleF(113, 57, 200, 17), UiGraphics.TextAlignment.Left);
            g.DrawString(Language.GetLocalizedText(271, "RA : "), 8, SysColor.White, new RectangleF(6, 222, 31, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Coordinates.FormatHMS(Target.RA), 8, SysColor.White, new RectangleF(41, 222, 78, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Language.GetLocalizedText(270, "Dec : "), 8, SysColor.White, new RectangleF(6, 238, 31, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Coordinates.FormatDMSWide(Target.Dec), 8, SysColor.White, new RectangleF(41, 238, 78, 15), UiGraphics.TextAlignment.Left);

            Coordinates altAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(Target.RA, Target.Dec), SpaceTimeController.Location, SpaceTimeController.Now);
          
            g.DrawString(Language.GetLocalizedText(269, "Alt : "), 8, SysColor.White, new RectangleF(6, 254, 31, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Coordinates.FormatDMSWide(altAz.Alt), 8, SysColor.White, new RectangleF(41, 254, 78, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Language.GetLocalizedText(268, "Az :"), 8, SysColor.White, new RectangleF(6, 270, 31, 15), UiGraphics.TextAlignment.Left);
            g.DrawString(Coordinates.FormatDMSWide(altAz.Az), 8, SysColor.White, new RectangleF(41, 270, 78, 15), UiGraphics.TextAlignment.Left);
            

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
