using System;
using System.Collections.Generic;


namespace TerraViewer
{
    class Shaders
    {
    }


    public enum BasicEffect
    {
        TextureOnly,
        ColorOnly,
        TextureColorOpacity,
        ColoredText,
    }

    public enum PlanetSurfaceStyle
    {
        Diffuse,               // Ordinary planets
        DiffuseAndNight,       // Earth with night map
        Specular,              // Blinn-Phong specular + diffuse
        SpecularPass,          // Specular pass for Earth water areas
        Emissive,              // Sun
        Sky,                   // Sky background
        PlanetaryRings,        // Rings of Saturn
        LommelSeeliger,        // Appropriate for Moon and other dusty surfaces
    };

    public struct PlanetShaderKey
    {
        // Surface properties that may be modified by a texture map
        public enum SurfaceProperty
        {
            None = 0x0,
            Diffuse = 0x1,
            Specular = 0x2,
            Normal = 0x4
        }

        public enum ShaderFlags
        {
            None = 0x0,
            Atmosphere = 0x1,
            RingShadows = 0x2,
            TwoSidedLighting = 0x4,
            AlphaTexture = 0x8,
            PerVertexColor = 0x20,
        }

        public PlanetShaderKey(PlanetSurfaceStyle style, bool hasAtmosphere, int eclipseShadowCount)
        {
            this.style = style;
            flags = hasAtmosphere ? ShaderFlags.Atmosphere : ShaderFlags.None;
            this.eclipseShadowCount = eclipseShadowCount;
            overlayTextureCount = 0;
            lightCount = 1;
            textures = SurfaceProperty.Diffuse;
        }

        public bool hasTexture(SurfaceProperty p)
        {
            return (textures & p) == p;
        }

        public bool HasAtmosphere
        {
            get
            {
                return (flags & ShaderFlags.Atmosphere) == ShaderFlags.Atmosphere;
            }

            set
            {
                flags = value ? (flags | ShaderFlags.Atmosphere) : (flags & ~ShaderFlags.Atmosphere);
            }
        }

        public bool HasRingShadows
        {
            get
            {
                return (flags & ShaderFlags.RingShadows) == ShaderFlags.RingShadows;
            }

            set
            {
                flags = value ? (flags | ShaderFlags.RingShadows) : (flags & ~ShaderFlags.RingShadows);
            }
        }

        public bool TwoSidedLighting
        {
            get
            {
                return (flags & ShaderFlags.TwoSidedLighting) == ShaderFlags.TwoSidedLighting;
            }

            set
            {
                flags = value ? (flags | ShaderFlags.TwoSidedLighting) : (flags & ~ShaderFlags.TwoSidedLighting);
            }
        }

        public bool AlphaTexture
        {
            get
            {
                return (flags & ShaderFlags.AlphaTexture) == ShaderFlags.AlphaTexture;
            }

            set
            {
                flags = value ? (flags | ShaderFlags.AlphaTexture) : (flags & ~ShaderFlags.AlphaTexture);
            }
        }


        public PlanetSurfaceStyle style;
        public int eclipseShadowCount;
        public int overlayTextureCount;
        public int lightCount;
        public SurfaceProperty textures;
        public ShaderFlags flags;

        public override string ToString()
        {
            return String.Format("{0:x}-{1:x}-{2:x}-{3:x}-{4:x}-{5:x}",
                (uint)style,
                (uint)eclipseShadowCount,
                (uint)overlayTextureCount,
                (uint)lightCount,
                (uint)textures,
                (uint)flags);
        }
    }


    class PlanetShaderKeyComparer : IEqualityComparer<PlanetShaderKey>
    {
        public bool Equals(PlanetShaderKey k0, PlanetShaderKey k1)
        {
            return k0.style == k1.style &&
                   k0.eclipseShadowCount == k1.eclipseShadowCount &&
                   k0.overlayTextureCount == k1.overlayTextureCount &&
                   k0.lightCount == k1.lightCount &&
                   k0.textures == k1.textures &&
                   k0.flags == k1.flags;
        }

        public int GetHashCode(PlanetShaderKey k)
        {
            const int m = 756065179; // a big prime

            var hash = k.style.GetHashCode();
            hash = m * hash + k.eclipseShadowCount;
            hash = m * hash + k.overlayTextureCount;
            hash = m * hash + k.lightCount;
            hash = m * hash + k.textures.GetHashCode();
            hash = m * hash + k.flags.GetHashCode();
            return hash;
        }
    }


   
}
