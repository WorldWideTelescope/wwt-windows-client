using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace TerraViewer
{
    public class Color
    {
        public static Color MediumPurple { get { return Color.FromArgb(0xFF, 0x93, 0x70, 0xDB); } }
        public static Color MediumSeaGreen { get { return Color.FromArgb(0xFF, 0x3C, 0xB3, 0x71); } }
        public static Color MediumSlateBlue { get { return Color.FromArgb(0xFF, 0x7B, 0x68, 0xEE); } }
        public static Color MediumSpringGreen { get { return Color.FromArgb(0xFF, 0x00, 0xFA, 0x9A); } }
        public static Color MediumTurquoise { get { return Color.FromArgb(0xFF, 0x48, 0xD1, 0xCC); } }
        public static Color MediumVioletRed { get { return Color.FromArgb(0xFF, 0xC7, 0x15, 0x85); } }
        public static Color MidnightBlue { get { return Color.FromArgb(0xFF, 0x19, 0x19, 0x70); } }
        public static Color MediumOrchid { get { return Color.FromArgb(0xFF, 0xBA, 0x55, 0xD3); } }
        public static Color MintCream { get { return Color.FromArgb(0xFF, 0xF5, 0xFF, 0xFA); } }
        public static Color Moccasin { get { return Color.FromArgb(0xFF, 0xFF, 0xE4, 0xB5); } }
        public static Color NavajoWhite { get { return Color.FromArgb(0xFF, 0xFF, 0xDE, 0xAD); } }
        public static Color Navy { get { return Color.FromArgb(0xFF, 0x00, 0x00, 0x80); } }
        public static Color OldLace { get { return Color.FromArgb(0xFF, 0xFD, 0xF5, 0xE6); } }
        public static Color Olive { get { return Color.FromArgb(0xFF, 0x80, 0x80, 0x00); } }
        public static Color OliveDrab { get { return Color.FromArgb(0xFF, 0x6B, 0x8E, 0x23); } }
        public static Color Orange { get { return Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00); } }
        public static Color MistyRose { get { return Color.FromArgb(0xFF, 0xFF, 0xE4, 0xE1); } }
        public static Color OrangeRed { get { return Color.FromArgb(0xFF, 0xFF, 0x45, 0x00); } }
        public static Color MediumBlue { get { return Color.FromArgb(0xFF, 0x00, 0x00, 0xCD); } }
        public static Color Maroon { get { return Color.FromArgb(0xFF, 0x80, 0x00, 0x00); } }
        public static Color LightBlue { get { return Color.FromArgb(0xFF, 0xAD, 0xD8, 0xE6); } }
        public static Color LightCoral { get { return Color.FromArgb(0xFF, 0xF0, 0x80, 0x80); } }
        public static Color LightGoldenrodYellow { get { return Color.FromArgb(0xFF, 0xFA, 0xFA, 0xD2); } }
        public static Color LightGreen { get { return Color.FromArgb(0xFF, 0x90, 0xEE, 0x90); } }
        public static Color LightGray { get { return Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3); } }
        public static Color LightPink { get { return Color.FromArgb(0xFF, 0xFF, 0xB6, 0xC1); } }
        public static Color LightSalmon { get { return Color.FromArgb(0xFF, 0xFF, 0xA0, 0x7A); } }
        public static Color MediumAquamarine { get { return Color.FromArgb(0xFF, 0x66, 0xCD, 0xAA); } }
        public static Color LightSeaGreen { get { return Color.FromArgb(0xFF, 0x20, 0xB2, 0xAA); } }
        public static Color LightSlateGray { get { return Color.FromArgb(0xFF, 0x77, 0x88, 0x99); } }
        public static Color LightSteelBlue { get { return Color.FromArgb(0xFF, 0xB0, 0xC4, 0xDE); } }
        public static Color LightYellow { get { return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xE0); } }
        public static Color Lime { get { return Color.FromArgb(0xFF, 0x00, 0xFF, 0x00); } }
        public static Color LimeGreen { get { return Color.FromArgb(0xFF, 0x32, 0xCD, 0x32); } }
        public static Color Linen { get { return Color.FromArgb(0xFF, 0xFA, 0xF0, 0xE6); } }
        public static Color Magenta { get { return Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF); } }
        public static Color LightSkyBlue { get { return Color.FromArgb(0xFF, 0x87, 0xCE, 0xFA); } }
        public static Color LemonChiffon { get { return Color.FromArgb(0xFF, 0xFF, 0xFA, 0xCD); } }
        public static Color Orchid { get { return Color.FromArgb(0xFF, 0xDA, 0x70, 0xD6); } }
        public static Color PaleGreen { get { return Color.FromArgb(0xFF, 0x98, 0xFB, 0x98); } }
        public static Color SlateBlue { get { return Color.FromArgb(0xFF, 0x6A, 0x5A, 0xCD); } }
        public static Color SlateGray { get { return Color.FromArgb(0xFF, 0x70, 0x80, 0x90); } }
        public static Color Snow { get { return Color.FromArgb(0xFF, 0xFF, 0xFA, 0xFA); } }
        public static Color SpringGreen { get { return Color.FromArgb(0xFF, 0x00, 0xFF, 0x7F); } }
        public static Color SteelBlue { get { return Color.FromArgb(0xFF, 0x46, 0x82, 0xB4); } }
        public static Color Tan { get { return Color.FromArgb(0xFF, 0xD2, 0xB4, 0x8C); } }
        public static Color Teal { get { return Color.FromArgb(0xFF, 0x00, 0x80, 0x80); } }
        public static Color SkyBlue { get { return Color.FromArgb(0xFF, 0x87, 0xCE, 0xEB); } }
        public static Color Thistle { get { return Color.FromArgb(0xFF, 0xD8, 0xBF, 0xD8); } }
        public static Color Turquoise { get { return Color.FromArgb(0xFF, 0x40, 0xE0, 0xD0); } }
        public static Color Violet { get { return Color.FromArgb(0xFF, 0xEE, 0x82, 0xEE); } }
        public static Color Wheat { get { return Color.FromArgb(0xFF, 0xF5, 0xDE, 0xB3); } }
        public static Color White { get { return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF); } }
        public static Color WhiteSmoke { get { return Color.FromArgb(0xFF, 0xF5, 0xF5, 0xF5); } }
        public static Color Yellow { get { return Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00); } }
        public static Color YellowGreen { get { return Color.FromArgb(0xFF, 0x9A, 0xCD, 0x32); } }
        public static Color Tomato { get { return Color.FromArgb(0xFF, 0xFF, 0x63, 0x47); } }
        public static Color PaleGoldenrod { get { return Color.FromArgb(0xFF, 0xEE, 0xE8, 0xAA); } }
        public static Color Silver { get { return Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0); } }
        public static Color SeaShell { get { return Color.FromArgb(0xFF, 0xFF, 0xF5, 0xEE); } }
        public static Color PaleTurquoise { get { return Color.FromArgb(0xFF, 0xAF, 0xEE, 0xEE); } }
        public static Color PaleVioletRed { get { return Color.FromArgb(0xFF, 0xDB, 0x70, 0x93); } }
        public static Color PapayaWhip { get { return Color.FromArgb(0xFF, 0xFF, 0xEF, 0xD5); } }
        public static Color PeachPuff { get { return Color.FromArgb(0xFF, 0xFF, 0xDA, 0xB9); } }
        public static Color Peru { get { return Color.FromArgb(0xFF, 0xCD, 0x85, 0x3F); } }
        public static Color Pink { get { return Color.FromArgb(0xFF, 0xFF, 0xC0, 0xCB); } }
        public static Color Plum { get { return Color.FromArgb(0xFF, 0xDD, 0xA0, 0xDD); } }
        public static Color Sienna { get { return Color.FromArgb(0xFF, 0xA0, 0x52, 0x2D); } }
        public static Color PowderBlue { get { return Color.FromArgb(0xFF, 0xB0, 0xE0, 0xE6); } }
        public static Color Red { get { return Color.FromArgb(0xFF, 0xFF, 0x00, 0x00); } }
        public static Color RosyBrown { get { return Color.FromArgb(0xFF, 0xBC, 0x8F, 0x8F); } }
        public static Color RoyalBlue { get { return Color.FromArgb(0xFF, 0x41, 0x69, 0xE1); } }
        public static Color SaddleBrown { get { return Color.FromArgb(0xFF, 0x8B, 0x45, 0x13); } }
        public static Color Salmon { get { return Color.FromArgb(0xFF, 0xFA, 0x80, 0x72); } }
        public static Color SandyBrown { get { return Color.FromArgb(0xFF, 0xF4, 0xA4, 0x60); } }
        public static Color SeaGreen { get { return Color.FromArgb(0xFF, 0x2E, 0x8B, 0x57); } }
        public static Color Purple { get { return Color.FromArgb(0xFF, 0x80, 0x00, 0x80); } }
        public static Color LawnGreen { get { return Color.FromArgb(0xFF, 0x7C, 0xFC, 0x00); } }
        public static Color LightCyan { get { return Color.FromArgb(0xFF, 0xE0, 0xFF, 0xFF); } }
        public static Color Lavender { get { return Color.FromArgb(0xFF, 0xE6, 0xE6, 0xFA); } }
        public static Color DarkKhaki { get { return Color.FromArgb(0xFF, 0xBD, 0xB7, 0x6B); } }
        public static Color DarkGreen { get { return Color.FromArgb(0xFF, 0x00, 0x64, 0x00); } }
        public static Color DarkGray { get { return Color.FromArgb(0xFF, 0xA9, 0xA9, 0xA9); } }
        public static Color DarkGoldenrod { get { return Color.FromArgb(0xFF, 0xB8, 0x86, 0x0B); } }
        public static Color DarkCyan { get { return Color.FromArgb(0xFF, 0x00, 0x8B, 0x8B); } }
        public static Color DarkBlue { get { return Color.FromArgb(0xFF, 0x00, 0x00, 0x8B); } }
        public static Color Cyan { get { return Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF); } }
        public static Color Crimson { get { return Color.FromArgb(0xFF, 0xDC, 0x14, 0x3C); } }
        public static Color Cornsilk { get { return Color.FromArgb(0xFF, 0xFF, 0xF8, 0xDC); } }
        public static Color LavenderBlush { get { return Color.FromArgb(0xFF, 0xFF, 0xF0, 0xF5); } }
        public static Color Coral { get { return Color.FromArgb(0xFF, 0xFF, 0x7F, 0x50); } }
        public static Color Chocolate { get { return Color.FromArgb(0xFF, 0xD2, 0x69, 0x1E); } }
        public static Color Chartreuse { get { return Color.FromArgb(0xFF, 0x7F, 0xFF, 0x00); } }
        public static Color DarkMagenta { get { return Color.FromArgb(0xFF, 0x8B, 0x00, 0x8B); } }
        public static Color CadetBlue { get { return Color.FromArgb(0xFF, 0x5F, 0x9E, 0xA0); } }
        public static Color Brown { get { return Color.FromArgb(0xFF, 0xA5, 0x2A, 0x2A); } }
        public static Color BlueViolet { get { return Color.FromArgb(0xFF, 0x8A, 0x2B, 0xE2); } }
        public static Color Blue { get { return Color.FromArgb(0xFF, 0x00, 0x00, 0xFF); } }
        public static Color BlanchedAlmond { get { return Color.FromArgb(0xFF, 0xFF, 0xEB, 0xCD); } }
        public static Color Black { get { return Color.FromArgb(0xFF, 0x00, 0x00, 0x00); } }
        public static Color Bisque { get { return Color.FromArgb(0xFF, 0xFF, 0xE4, 0xC4); } }
        public static Color Beige { get { return Color.FromArgb(0xFF, 0xF5, 0xF5, 0xDC); } }
        public static Color Azure { get { return Color.FromArgb(0xFF, 0xF0, 0xFF, 0xFF); } }
        public static Color Aquamarine { get { return Color.FromArgb(0xFF, 0x7F, 0xFF, 0xD4); } }
        public static Color Aqua { get { return Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF); } }
        public static Color AntiqueWhite { get { return Color.FromArgb(0xFF, 0xFA, 0xEB, 0xD7); } }
        public static Color AliceBlue { get { return Color.FromArgb(0xFF, 0xF0, 0xF8, 0xFF); } }
        public static Color Transparent { get { return Color.FromArgb(0xFF, 0xF0, 0xF8, 0xFF); } }
        public static Color BurlyWood { get { return Color.FromArgb(0xFF, 0xDE, 0xB8, 0x87); } }
        public static Color DarkOliveGreen { get { return Color.FromArgb(0xFF, 0x55, 0x6B, 0x2F); } }
        public static Color CornflowerBlue { get { return Color.FromArgb(0xFF, 0x64, 0x95, 0xED); } }
        public static Color DarkOrchid { get { return Color.FromArgb(0xFF, 0x99, 0x32, 0xCC); } }
        public static Color Khaki { get { return Color.FromArgb(0xFF, 0xF0, 0xE6, 0x8C); } }
        public static Color Ivory { get { return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xF0); } }
        public static Color DarkOrange { get { return Color.FromArgb(0xFF, 0xFF, 0x8C, 0x00); } }
        public static Color Indigo { get { return Color.FromArgb(0xFF, 0x4B, 0x00, 0x82); } }
        public static Color IndianRed { get { return Color.FromArgb(0xFF, 0xCD, 0x5C, 0x5C); } }
        public static Color HotPink { get { return Color.FromArgb(0xFF, 0xFF, 0x69, 0xB4); } }
        public static Color Honeydew { get { return Color.FromArgb(0xFF, 0xF0, 0xFF, 0xF0); } }
        public static Color GreenYellow { get { return Color.FromArgb(0xFF, 0xAD, 0xFF, 0x2F); } }
        public static Color Green { get { return Color.FromArgb(0xFF, 0x00, 0x80, 0x00); } }
        public static Color Gray { get { return Color.FromArgb(0xFF, 0x80, 0x80, 0x80); } }
        public static Color Goldenrod { get { return Color.FromArgb(0xFF, 0xDA, 0xA5, 0x20); } }
        public static Color GhostWhite { get { return Color.FromArgb(0xFF, 0xF8, 0xF8, 0xFF); } }
        public static Color Gainsboro { get { return Color.FromArgb(0xFF, 0xDC, 0xDC, 0xDC); } }
        public static Color Fuchsia { get { return Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF); } }
        public static Color Gold { get { return Color.FromArgb(0xFF, 0xFF, 0xD7, 0x00); } }
        public static Color FloralWhite { get { return Color.FromArgb(0xFF, 0xFF, 0xFA, 0xF0); } }
        public static Color DarkRed { get { return Color.FromArgb(0xFF, 0x8B, 0x00, 0x00); } }
        public static Color DarkSalmon { get { return Color.FromArgb(0xFF, 0xE9, 0x96, 0x7A); } }
        public static Color DarkSeaGreen { get { return Color.FromArgb(0xFF, 0x8F, 0xBC, 0x8F); } }
        public static Color ForestGreen { get { return Color.FromArgb(0xFF, 0x22, 0x8B, 0x22); } }
        public static Color DarkSlateGray { get { return Color.FromArgb(0xFF, 0x2F, 0x4F, 0x4F); } }
        public static Color DarkTurquoise { get { return Color.FromArgb(0xFF, 0x00, 0xCE, 0xD1); } }
        public static Color DarkSlateBlue { get { return Color.FromArgb(0xFF, 0x48, 0x3D, 0x8B); } }
        public static Color DeepPink { get { return Color.FromArgb(0xFF, 0xFF, 0x14, 0x93); } }
        public static Color DeepSkyBlue { get { return Color.FromArgb(0xFF, 0x00, 0xBF, 0xFF); } }
        public static Color DimGray { get { return Color.FromArgb(0xFF, 0x69, 0x69, 0x69); } }
        public static Color DodgerBlue { get { return Color.FromArgb(0xFF, 0x1E, 0x90, 0xFF); } }
        public static Color Firebrick { get { return Color.FromArgb(0xFF, 0xB2, 0x22, 0x22); } }
        public static Color DarkViolet { get { return Color.FromArgb(0xFF, 0x94, 0x00, 0xD3); } }

        private int color = 0;

        public Color(int col)
        {
            color = col;
        }

        //
        // Summary:
        //     Gets the green component value of this System.Drawing.Color structure.
        //
        // Returns:
        //     The green component value of this System.Drawing.Color.
        public byte G
        {
            get
            {
                return (byte)(color >> 8);
            }
        }
        //
        // Summary:
        //     Gets a value indicating whether this System.Drawing.Color structure is a named
        //     color or a member of the System.Drawing.KnownColor enumeration.
        //
        // Returns:
        //     true if this System.Drawing.Color was created by using either the System.Drawing.Color.FromName(System.String)
        //     method or the System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor)
        //     method; otherwise, false.
        public bool IsNamedColor { get; }
        //
        // Summary:
        //     Specifies whether this System.Drawing.Color structure is uninitialized.
        //
        // Returns:
        //     This property returns true if this color is uninitialized; otherwise, false.
        public bool IsEmpty { get; }
        //
        // Summary:
        //     Gets a value indicating whether this System.Drawing.Color structure is a predefined
        //     color. Predefined colors are represented by the elements of the System.Drawing.KnownColor
        //     enumeration.
        //
        // Returns:
        //     true if this System.Drawing.Color was created from a predefined color by using
        //     either the System.Drawing.Color.FromName(System.String) method or the System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor)
        //     method; otherwise, false.
        public bool IsKnownColor { get; }
        //
        // Summary:
        //     Gets the alpha component value of this System.Drawing.Color structure.
        //
        // Returns:
        //     The alpha component value of this System.Drawing.Color.
        public byte A
        {
            get
            {
                return (byte)(color >> 24);
            }
        }
        //
        // Summary:
        //     Gets the blue component value of this System.Drawing.Color structure.
        //
        // Returns:
        //     The blue component value of this System.Drawing.Color.
        public byte B
        {
            get
            {
                return (byte)(color);
            }
        }
        //
        // Summary:
        //     Gets the red component value of this System.Drawing.Color structure.
        //
        // Returns:
        //     The red component value of this System.Drawing.Color.
        public byte R
        {
            get
            {
                return (byte)(color >> 16);
            }
        }
        //
        // Summary:
        //     Gets a value indicating whether this System.Drawing.Color structure is a system
        //     color. A system color is a color that is used in a Windows display element. System
        //     colors are represented by elements of the System.Drawing.KnownColor enumeration.
        //
        // Returns:
        //     true if this System.Drawing.Color was created from a system color by using either
        //     the System.Drawing.Color.FromName(System.String) method or the System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor)
        //     method; otherwise, false.
        public bool IsSystemColor { get; }
        //
        // Summary:
        //     Gets the name of this System.Drawing.Color.
        //
        // Returns:
        //     The name of this System.Drawing.Color.
        public string Name { get; }

        //
        // Summary:
        //     Creates a System.Drawing.Color structure from the specified 8-bit color values
        //     (red, green, and blue). The alpha value is implicitly 255 (fully opaque). Although
        //     this method allows a 32-bit value to be passed for each color component, the
        //     value of each component is limited to 8 bits.
        //
        // Parameters:
        //   red:
        //     The red component value for the new System.Drawing.Color. Valid values are 0
        //     through 255.
        //
        //   green:
        //     The green component value for the new System.Drawing.Color. Valid values are
        //     0 through 255.
        //
        //   blue:
        //     The blue component value for the new System.Drawing.Color. Valid values are 0
        //     through 255.
        //
        // Returns:
        //     The System.Drawing.Color that this method creates.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     red, green, or blue is less than 0 or greater than 255.
        public static Color FromArgb(int red, int green, int blue)
        {
            return new Color((int)(((uint)255 << 24) | ((uint)red << 16) | ((uint)green << 8) | (uint)blue));
        }
        //
        // Summary:
        //     Creates a System.Drawing.Color structure from the specified System.Drawing.Color
        //     structure, but with the new specified alpha value. Although this method allows
        //     a 32-bit value to be passed for the alpha value, the value is limited to 8 bits.
        //
        // Parameters:
        //   alpha:
        //     The alpha value for the new System.Drawing.Color. Valid values are 0 through
        //     255.
        //
        //   baseColor:
        //     The System.Drawing.Color from which to create the new System.Drawing.Color.
        //
        // Returns:
        //     The System.Drawing.Color that this method creates.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     alpha is less than 0 or greater than 255.
        public static Color FromArgb(int alpha, Color baseColor)
        {
            return new Color((int)(((uint)alpha << 24) | ((uint)baseColor.R << 16) | ((uint)baseColor.G << 8) |(uint)baseColor.B));
        }
        //
        // Summary:
        //     Creates a System.Drawing.Color structure from the four ARGB component (alpha,
        //     red, green, and blue) values. Although this method allows a 32-bit value to be
        //     passed for each component, the value of each component is limited to 8 bits.
        //
        // Parameters:
        //   alpha:
        //     The alpha component. Valid values are 0 through 255.
        //
        //   red:
        //     The red component. Valid values are 0 through 255.
        //
        //   green:
        //     The green component. Valid values are 0 through 255.
        //
        //   blue:
        //     The blue component. Valid values are 0 through 255.
        //
        // Returns:
        //     The System.Drawing.Color that this method creates.
        //
        // Exceptions:
        //   T:System.ArgumentException:
        //     alpha, red, green, or blue is less than 0 or greater than 255.
        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            return new Color((int)((uint)(alpha << 24) | (uint)(red << 16) | (uint)(green << 8) | (uint)blue));
        }
        //
        // Summary:
        //     Creates a System.Drawing.Color structure from a 32-bit ARGB value.
        //
        // Parameters:
        //   argb:
        //     A value specifying the 32-bit ARGB value.
        //
        // Returns:
        //     The System.Drawing.Color structure that this method creates.
        public static Color FromArgb(int argb)
        {
            return new Color(argb);
        }
        //
        // Summary:
        //     Creates a System.Drawing.Color structure from the specified predefined color.
        //
        // Parameters:
        //   color:
        //     An element of the System.Drawing.KnownColor enumeration.
        //
        // Returns:
        //     The System.Drawing.Color that this method creates.
        //public static Color FromKnownColor(KnownColor color);
        //
        // Summary:
        //     Creates a System.Drawing.Color structure from the specified name of a predefined
        //     color.
        //
        // Parameters:
        //   name:
        //     A string that is the name of a predefined color. Valid names are the same as
        //     the names of the elements of the System.Drawing.KnownColor enumeration.
        //
        // Returns:
        //     The System.Drawing.Color that this method creates.
        public static Color FromName(string name)
        {
            name = name.ToLower();
            Type type = typeof(Color); // MyClass is static class with static properties
            foreach (var t in type.GetProperties())
            {
                if (t.Name.ToLower() == name)
                {
                    return (Color)t.GetValue(null, null);
                }
            }
            return Color.Black;
        }
        //
        // Summary:
        //     Tests whether the specified object is a System.Drawing.Color structure and is
        //     equivalent to this System.Drawing.Color structure.
        //
        // Parameters:
        //   obj:
        //     The object to test.
        //
        // Returns:
        //     true if obj is a System.Drawing.Color structure equivalent to this System.Drawing.Color
        //     structure; otherwise, false.
        public override bool Equals(object obj)
        {
            return (((Color)obj).color == color);
        }
        //
        // Summary:
        //     Gets the hue-saturation-brightness (HSB) brightness value for this System.Drawing.Color
        //     structure.
        //
        // Returns:
        //     The brightness of this System.Drawing.Color. The brightness ranges from 0.0 through
        //     1.0, where 0.0 represents black and 1.0 represents white.
        //public float GetBrightness();
        //
        // Summary:
        //     Returns a hash code for this System.Drawing.Color structure.
        //
        // Returns:
        //     An integer value that specifies the hash code for this System.Drawing.Color.
        public override int GetHashCode()
        {
            return color.GetHashCode();
        }
        //
        // Summary:
        //     Gets the hue-saturation-brightness (HSB) hue value, in degrees, for this System.Drawing.Color
        //     structure.
        //
        // Returns:
        //     The hue, in degrees, of this System.Drawing.Color. The hue is measured in degrees,
        //     ranging from 0.0 through 360.0, in HSB color space.
       // public float GetHue();
        //
        // Summary:
        //     Gets the hue-saturation-brightness (HSB) saturation value for this System.Drawing.Color
        //     structure.
        //
        // Returns:
        //     The saturation of this System.Drawing.Color. The saturation ranges from 0.0 through
        //     1.0, where 0.0 is grayscale and 1.0 is the most saturated.
       // public float GetSaturation();
        //
        // Summary:
        //     Gets the 32-bit ARGB value of this System.Drawing.Color structure.
        //
        // Returns:
        //     The 32-bit ARGB value of this System.Drawing.Color.
        public int ToArgb()
        {
            return color;
        }
        //
        // Summary:
        //     Gets the System.Drawing.KnownColor value of this System.Drawing.Color structure.
        //
        // Returns:
        //     An element of the System.Drawing.KnownColor enumeration, if the System.Drawing.Color
        //     is created from a predefined color by using either the System.Drawing.Color.FromName(System.String)
        //     method or the System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor)
        //     method; otherwise, 0.
       // public KnownColor ToKnownColor();
        //
        // Summary:
        //     Converts this System.Drawing.Color structure to a human-readable string.
        //
        // Returns:
        //     A string that is the name of this System.Drawing.Color, if the System.Drawing.Color
        //     is created from a predefined color by using either the System.Drawing.Color.FromName(System.String)
        //     method or the System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor)
        //     method; otherwise, a string that consists of the ARGB component names and their
        //     values.
        public override string ToString()
        {
            return string.Format("A={0}, R={1}, G={2}, b={3}", A, R, G, B);
        }

        //
        // Summary:
        //     Tests whether two specified System.Drawing.Color structures are equivalent.
        //
        // Parameters:
        //   left:
        //     The System.Drawing.Color that is to the left of the equality operator.
        //
        //   right:
        //     The System.Drawing.Color that is to the right of the equality operator.
        //
        // Returns:
        //     true if the two System.Drawing.Color structures are equal; otherwise, false.
        public static bool operator ==(Color left, Color right)
        {
            return left.color == right.color;
        }
        //
        // Summary:
        //     Tests whether two specified System.Drawing.Color structures are different.
        //
        // Parameters:
        //   left:
        //     The System.Drawing.Color that is to the left of the inequality operator.
        //
        //   right:
        //     The System.Drawing.Color that is to the right of the inequality operator.
        //
        // Returns:
        //     true if the two System.Drawing.Color structures are different; otherwise, false.
        public static bool operator !=(Color left, Color right)
        {
            return left.color != right.color;
        }
    }
}
