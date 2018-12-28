using System;
using System.Collections.Generic;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using SharpDX;
using System.Runtime.InteropServices;
using System.Reflection;
#if WINDOWS_UWP
using SysColor = TerraViewer.Color;
#else
using SysColor = System.Drawing.Color;
#endif

namespace TerraViewer
{
    public class PlanetShader11
    {        
        public const int MaxLightSources = 3;
        public const int MaxEclipseShadows = 4;
        public const int MaxOverlayTextures = 2;

        private PixelShader pixelShader = null;
        private VertexShader vertexShader = null;
        private GeometryShader geometryShader = null;
        private byte[] pixelShaderBytecode;
        private byte[] vertexShaderBytecode;
        private byte[] geometryShaderBytecode;


        private PlanetShaderKey key;

        private static PlanetShaderKeyComparer shaderLibraryComparer = null;
        private static Dictionary<PlanetShaderKey, PlanetShader11> shaderLibrary = null;

        public enum StandardVertexLayout
        {
            Position,
            PositionNormal,
            PositionNormalTex,
            PositionNormalTex2,
            PositionNormalTexTangent,
            Count
        };


        [StructLayout(LayoutKind.Explicit, Size = 1056)]
        public struct PlanetShaderConstants
        {
            [FieldOffset(0)]
            // Combined world/view/projection matrix
            public Matrix WorldViewProjection;

            [FieldOffset(64)]
            // Combined world/view matrix
            public Matrix WorldView;

            [FieldOffset(128)]
            // Opacity value; currently unused, and may be replaced by diffuseColor alpha
            public float Opacity;

            [FieldOffset(144)]
            // Sun direction in object coordinates (normalized); sun direction
            // may be required by both vertex shader and pixel shader.
            public Vector3 SunDirection;

            [FieldOffset(160)]
            // Camera position in object coordinates; camera position
            // may be required by both vertex shader and pixel shader
            public Vector3 CameraPosition;

            [FieldOffset(176)]
            // Rayleigh scattering coefficients. In units of planet radius ^ -1
            public Vector3 RayleighScatterCoeff;

            [FieldOffset(192)]
            // Height of the atmosphere in units of planet radius
            public float AtmosphereHeight;

            [FieldOffset(208)]
            // Reference radius for computing atmosphere density, in units of planet radius
            public float AtmosphereZeroRadius;

            [FieldOffset(224)]
            // Inverse scale height of the atmosphere, in units of planet radius ^ -1
            public float AtmosphereInvScaleHeight;

            [FieldOffset(240)]
            // Atmosphere center. Typically the origin except when using localCenter for planet tiles
            public Vector3 AtmosphereCenter;

            [FieldOffset(256)]
            public Matrix EclipseShadow0;
            [FieldOffset(320)]
            public Matrix EclipseShadow1;
            [FieldOffset(384)]
            public Matrix EclipseShadow2;
            [FieldOffset(448)]
            public Matrix EclipseShadow3;
            [FieldOffset(512)]
            public Matrix OverlayTextureMatrix0;
            [FieldOffset(576)]
            public Matrix OverlayTextureMatrix1;
            [FieldOffset(640)]
            public Matrix OverlayTextureMatrix2;
            [FieldOffset(704)]
            public Matrix OverlayTextureMatrix3;
            [FieldOffset(768)]
            public Vector3 LightDirection0;
            [FieldOffset(784)]
            public Vector3 LightDirection1;
            [FieldOffset(800)]
            public Vector3 LightDirection2;
            [FieldOffset(816)]
            public Vector3 LightDirection3;
            [FieldOffset(832)]
            public Vector3 LightColor0;
            [FieldOffset(848)]
            public Vector3 LightColor1;
            [FieldOffset(864)]
            public Vector3 LightColor2;
            [FieldOffset(880)]
            public Vector3 LightColor3;
            [FieldOffset(896)]
            public Vector3 AmbientLightColor;

            [FieldOffset(912)]
            // Diffuse material color
            public Vector4 DiffuseColor;

            [FieldOffset(928)]
            public Vector3 HemiLightColor;
            [FieldOffset(944)]
            public Vector3 HemiLightUp;

            [FieldOffset(960)]
            // Specular material color
            public Vector4 SpecularLightColor;

            [FieldOffset(976)]
            // Phong exponent for the surface material
            public float SpecularPower;

            [FieldOffset(992)]
            public Vector4 OverlayColor0;

            [FieldOffset(1008)]
            public Vector4 OverlayColor1;

            [FieldOffset(1024)]
            public Vector4 OverlayColor2;

            [FieldOffset(1040)]
            public Vector4 OverlayColor3;
        }

        private SharpDX.Direct3D11.InputLayout[] layoutCache = new SharpDX.Direct3D11.InputLayout[(int)StandardVertexLayout.Count];

        private SharpDX.Direct3D11.Buffer constantBuffer;
        private PlanetShaderConstants constants;

        // Set this value to true while debugging to force reloading of all shaders
        private static bool regenerateShaders = false;

        public static void CompileAndSaveShaders()
        {
            PlanetShaderKey k = new PlanetShaderKey(PlanetSurfaceStyle.Emissive, false, 0);
            saveToLibrary(k);
            k.textures = PlanetShaderKey.SurfaceProperty.Diffuse;
            saveToLibrary(k);

            for (int lightCount = 0; lightCount < 2; ++lightCount)
            {
                int maxShadows = Math.Min(lightCount, 1);
                for (int shadowCount = 0; shadowCount < maxShadows; ++shadowCount)
                {
                    for (int tex = 0; tex < 2; ++tex)
                    {
                        k.eclipseShadowCount = shadowCount;
                        k.lightCount = lightCount;
                        k.textures = tex == 0 ? PlanetShaderKey.SurfaceProperty.None : PlanetShaderKey.SurfaceProperty.Diffuse;
                        k.style = PlanetSurfaceStyle.Diffuse;
                        saveToLibrary(k);
                        k.style = PlanetSurfaceStyle.Sky;
                        saveToLibrary(k);
                        k.style = PlanetSurfaceStyle.Specular;
                        saveToLibrary(k);
                        k.style = PlanetSurfaceStyle.SpecularPass;
                        saveToLibrary(k);
                        k.style = PlanetSurfaceStyle.PlanetaryRings;
                        saveToLibrary(k);
                        k.style = PlanetSurfaceStyle.DiffuseAndNight;
                        saveToLibrary(k);
                    }
                }
            }

        }

        // Build a shader for all supported feature levels and save its
        // bytecode to the shader library.
        private static void saveToLibrary(PlanetShaderKey k)
        {
            var planetShader = new PlanetShader11(k);
            foreach (var level in ShaderLibrary.ShaderLevels)
            {
                planetShader.BuildShader(level.vertexProfile, level.pixelProfile);
            }
        }

        private PlanetShader11(PlanetShaderKey k)
        {
            key = k;
            constants = new PlanetShaderConstants();
        }

        // Get the input signature (used for constructing an InputLayout)
        public byte[] InputSignature
        {
            get
            {
                return vertexShaderBytecode;
            }
        }

        public VertexShader VertexShader
        {
            get
            {
                return vertexShader;
            }
        }

        public GeometryShader GeometryShader
        {
            get
            {
                return geometryShader;
            }
        }

        public PixelShader PixelShader
        {
            get
            {
                return pixelShader;
            }

        }

        public int EclipseShadowCount
        {
            get
            {
                return key.eclipseShadowCount;
            }
        }

        public PlanetShaderKey Key
        {
            get
            {
                return key;
            }
        }

        public void use(DeviceContext context)
        {
            //pass.Apply(context);

            // Copy data to constant buffer
            context.UpdateSubresource(ref constants, constantBuffer);

            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.PixelShader.SetConstantBuffer(0, constantBuffer);
            context.GeometryShader.SetConstantBuffer(0, constantBuffer);
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);
            context.GeometryShader.Set(geometryShader);

            // Set vertex shader
        }


        /// <summary>
        /// Get the planet shader for the specified surface style and defaults for other
        /// properties (no atmosphere, no shadows.)
        /// </summary>
        /// <param name="device">Direct3D device</param>
        /// <param name="style">Planet surface style</param>
        /// <returns>A Direct3D shader to produce the requested effect.</returns>
        public static PlanetShader11 GetPlanetShader(Device device, PlanetSurfaceStyle style, int eclipseShadowCount)
        {
            return GetPlanetShader(device, new PlanetShaderKey(style, false, eclipseShadowCount));
        }

        /// <summary>
        /// Get the planet shader for the specified shader key. The shader is created
        /// if it's not already in the shader library.
        /// </summary>
        /// <param name="device">Direct3D device</param>
        /// <param name="key">Planet surface shader key</param>
        /// <returns>A Direct3D shader to produce the requested effect.</returns>
        public static PlanetShader11 GetPlanetShader(Device device, PlanetShaderKey key)
        {
            PlanetShader11 shader;

            if (shaderLibrary == null || regenerateShaders)
            {
                shaderLibraryComparer = new PlanetShaderKeyComparer();
                shaderLibrary = new Dictionary<PlanetShaderKey, PlanetShader11>(shaderLibraryComparer);
                regenerateShaders = false;
            }

            // Retrieve the shader from the library of compiled shaders;
            // if the shader isn't in the shader library, create it and
            // add it.
            if (!shaderLibrary.TryGetValue(key, out shader))
            {
                if (shader == null)
                {
                    // Shader was not retrieved from the cache of precompiled shaders
                    shader = new PlanetShader11(key);
                    shader.BuildShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
                    shader.Realize(device);

                    shaderLibrary[key] = shader;
                }
            }
            return shader;
        }

        public float Opacity
        {
            set
            {
                constants.Opacity = value;
            }
        }

        public Matrix WVPMatrix
        {
            set
            {
                constants.WorldViewProjection = Matrix.Transpose(value);
            }
        }

        public Matrix WorldViewMatrix
        {
            set
            {
                constants.WorldView = Matrix.Transpose(value);
            }
        }

        public void SetEclipseShadowMatrix(int shadowIndex, Matrix m)
        {
            switch (shadowIndex)
            {
                case 0: constants.EclipseShadow0 = Matrix.Transpose(m); break;
                case 1: constants.EclipseShadow1 = Matrix.Transpose(m); break;
                case 2: constants.EclipseShadow2 = Matrix.Transpose(m); break;
                case 3: constants.EclipseShadow3 = Matrix.Transpose(m); break;
            }
        }

        public void SetOverlayTextureMatrix(int overlayIndex, Matrix m)
        {
            switch (overlayIndex)
            {
                case 0: constants.OverlayTextureMatrix0 = Matrix.Transpose(m); break;
                case 1: constants.OverlayTextureMatrix1 = Matrix.Transpose(m); break;
                case 2: constants.OverlayTextureMatrix2 = Matrix.Transpose(m); break;
                case 3: constants.OverlayTextureMatrix3 = Matrix.Transpose(m); break;
            }
        }

        public void SetOverlayTextureColor(int overlayIndex, SysColor color)
        {
            Vector4 colorf = new Vector4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
            switch (overlayIndex)
            {
                case 0: constants.OverlayColor0 = colorf; break;
                case 1: constants.OverlayColor1 = colorf; break;
                case 2: constants.OverlayColor2 = colorf; break;
                case 3: constants.OverlayColor3 = colorf; break;
            }
        }

        public void SetOverlayTexture(int overlayIndex, ShaderResourceView overlayResourceView)
        {
            RenderContext11.PrepDevice.ImmediateContext.PixelShader.SetShaderResource(overlayTextureSamplerIndex(overlayIndex), overlayResourceView);
        }

        public ShaderResourceView MainTexture
        {
            set
            {
                RenderContext11.PrepDevice.ImmediateContext.PixelShader.SetShaderResource(0, value);
            }
        }

        public ShaderResourceView RingTexture
        {
            set
            {
                RenderContext11.PrepDevice.ImmediateContext.PixelShader.SetShaderResource(ringShadowSamplerIndex(), value);
            }
        }

        public ShaderResourceView EclipseTexture
        {
            set
            {
                RenderContext11.PrepDevice.ImmediateContext.PixelShader.SetShaderResource(eclipseShadowSamplerIndex(), value);
            }
        }

        public ShaderResourceView NormalTexture
        {
            set
            {
                RenderContext11.PrepDevice.ImmediateContext.PixelShader.SetShaderResource(normalSamplerIndex(), value);
            }
        }

        public ShaderResourceView SpecularTexture
        {
            set
            {
                RenderContext11.PrepDevice.ImmediateContext.PixelShader.SetShaderResource(specularSamplerIndex(), value);
            }
        }

        public ShaderResourceView NightTexture
        {
            set
            {
                RenderContext11.PrepDevice.ImmediateContext.PixelShader.SetShaderResource(nightSamplerIndex(), value);
            }
        }


        public Vector3 SunDirection
        {
            set
            {
                constants.SunDirection = value;
                constants.LightDirection0 = value;
            }
        }

        // Light 0 is always the sun
        public void SetLightDirection(int lightIndex, Vector3 v)
        {
            if (lightIndex == 0)
            {
                // Special handling for light zero, which is always the Sun
                SunDirection = v;
            }
            else
            {                
                switch (lightIndex)
                {
                    case 0: constants.LightDirection0 = v; break;
                    case 1: constants.LightDirection1 = v; break;
                    case 2: constants.LightDirection2 = v; break;
                    case 3: constants.LightDirection3 = v; break;
                }
            }
        }

        public void SetLightColor(int lightIndex, Vector3 v)
        {
            switch (lightIndex)
            {
                case 0: constants.LightColor0 = v; break;
                case 1: constants.LightColor1 = v; break;
                case 2: constants.LightColor2 = v; break;
                case 3: constants.LightColor3 = v; break;
            }
        }

        public Vector3 AmbientLightColor
        {
            set
            {
                constants.AmbientLightColor = value;
            }
        }

        public Vector3 HemiLightColor
        {
            set
            {
                constants.HemiLightColor = value;
            }
        }

        public Vector3 HemiLightUpDirection
        {
            set
            {
                constants.HemiLightUp = value;
            }
        }

        public Vector3 CameraPosition
        {
            set
            {
                constants.CameraPosition = value;
            }
        }

        public Vector4 DiffuseColor
        {
            set
            {
                constants.DiffuseColor = value;
            }
        }

        public Vector4 SpecularColor
        {
            set
            {
                constants.SpecularLightColor = value;
            }
        }

        public float SpecularPower
        {
            set
            {
                constants.SpecularPower = value;
            }
        }

        public float AtmosphereHeight
        {
            set
            {
                constants.AtmosphereHeight = value;
            }
        }

        public float AtmosphereZeroHeight
        {
            set
            {
                constants.AtmosphereZeroRadius = value;
            }
        }

        public float AtmosphereInvScaleHeight
        {
            set
            {
                constants.AtmosphereInvScaleHeight = value;
            }
        }

        public Vector3 RayleighScatterCoeff
        {
            set
            {
                constants.RayleighScatterCoeff = value;
            }
        }

        public Vector3 AtmosphereCenter
        {
            set
            {
                constants.AtmosphereCenter = value;
            }
        }


        // **** Sampler indices ****

        public static int diffuseSamplerIndex()
        {
            return 0;
        }

        public static int specularSamplerIndex()
        {
            return 1;
        }

        public static int nightSamplerIndex()
        {
            return 2;
        }

        public static int normalSamplerIndex()
        {
            return 3;
        }

        public static int eclipseShadowSamplerIndex()
        {
            return 4;
        }

        public static int ringShadowSamplerIndex()
        {
            return 5;
        }

        public static int overlayTextureSamplerIndex(int overlayIndex)
        {
            return 6 + overlayIndex;
        }


        // **** Texture coordinate (interpolator) indices ****

        private static int eclipseShadowCoordIndex(int shadowIndex)
        {
            // TODO: should be a function of shader key
            return shadowIndex + 4;
        }

        private static int ringShadowCoordIndex()
        {
            // TODO: should be a function of shader key
            return 3;
        }

        private static int overlayTexCoordIndex(int overlayIndex)
        {
            // TODO: should be a function of shader key
            return overlayIndex + 7;
        }


        // Texture sampler assignments used by the shader generator:
        //    0 : Diffuse texture
        //    1 : Specular texture
        //    2 : Night map
        //    3 : Normal map
        //    4 : Eclipse shadow
        //    5 : Ring shadow
        //    6 : Overlay texture #1
        //    7 : Overlay texture #2
        //
        // Vertex shader outputs:
        //    COLOR0        : Diffuse color
        //    COLOR1        : Specular color
        //    TEXCOORD0     : Base texture coordinate
        //    TEXCOORD1     : Atmosphere inscatter
        //    TEXCOORD2     : Atmosphere transmissivity
        //    TEXCOORD3     : Ring shadow texture coordinate
        //    TEXCOORD[4-6] : Eclipse shadow texture coordinates (max. of 3 shadows)
        //    TEXCOORD[7-8] : Overlay texture coordinate OR tangent vector

        private string declareConstant(string type, string name, int offset)
        {
            return " " + type + " " + name + " : packoffset(c" + offset + ");\n";
            //return " " + type + " " + name + ";\n";
        }

        private void BuildShader(string vertexProfile, string pixelProfile)
        {
            // Just the defaults right now...
            ShaderFlags shaderFlags = 0;

            PlanetSurfaceStyle style = key.style;

#if !WINDOWS_UWP
            System.Console.WriteLine("Building shader: " + key.ToString());
#endif
            bool hasVertexColors = (key.flags & PlanetShaderKey.ShaderFlags.PerVertexColor) == PlanetShaderKey.ShaderFlags.PerVertexColor;
            bool needCameraSpacePosition = key.eclipseShadowCount > 0 || key.HasRingShadows;
            bool needObjectSpacePosition = true;
            bool needSurfaceNormals = style != PlanetSurfaceStyle.Emissive;
            bool needSurfaceTangents = needSurfaceNormals && key.hasTexture(PlanetShaderKey.SurfaceProperty.Normal);

            string vsIn;                // Vertex shader input
            string vsOut;                // Vertex shader output 
            string gsOut;               //Geometry shader output
            string psIn;                //  pixel shader input
            string shaderConstantDecl = "";  // Shader constant declarations
            string textureDecl = "";
            string vertexShaderText = "";    // Vertex shader source
            string pixelShaderText = "";     // Pixel shader source
            string samplerStateText = "";


            samplerStateText =
                "SamplerState MainTextureSampler : register(s0) {\n" +
                "    Filter = MIN_MAG_MIP_LINEAR;\n" +
                "    AddressU = Clamp;\n" +
                "    AddressV = Clamp;\n" +
                "};\n";

            samplerStateText +=
                "SamplerState ClampTextureSampler : register(s1) {\n" +
                "    Filter = MIN_MAG_MIP_LINEAR;\n" +
                "    AddressU = Clamp;\n" +
                "    AddressV = Clamp;\n" +
                "};\n";

            if (key.HasRingShadows)
            {
                samplerStateText +=
                    "SamplerState RingShadowTextureSampler : register(s2) {\n" +
                    "    Filter = MIN_MAG_MIP_LINEAR;\n" +
                    "    AddressU = Border;\n" +
                    "    AddressV = Border;\n" +
                    "};\n";
            }

            if (key.overlayTextureCount > 0)
            {
                samplerStateText +=
                    "SamplerState TransparentBorderSampler : register(s3) {\n" +
                    "    Filter = MIN_MAG_MIP_LINEAR;\n" +
                    "    AddressU = Border;\n" +
                    "    AddressV = Border;\n" +
                    //"    BorderColor = float4(0, 0, 0, 0);\n" +
                    "};\n";
            }

            // Generate the vertex shader source

            // Generate the vertex attribute set


            if (style == PlanetSurfaceStyle.Emissive)
            {
                vsIn =
                    " struct VS_IN                                 \n" +
                    " {                                            \n" +
                    "     float4 ObjPos   : POSITION;              \n" + // Object space position 
                    "     float2 TexCoord : TEXCOORD0;             \n";
                if (key.overlayTextureCount > 0)
                {
                    vsIn +=
                        "     float2 LongLatCoord : TEXCOORD1;     \n";
                }

                if (hasVertexColors)
                {
                    vsIn +=
                        "     float4 Color : COLOR0;                \n";
                }
                if (RenderContext11.ExternalProjection)
                {
                    vsIn +=
                            "     uint   instId   : SV_InstanceID;       \n  ";
                }
                vsIn +=
                    " };                                           \n";
            }
            else
            {
                vsIn =
                    " struct VS_IN                                 \n" +
                    " {                                            \n" +
                    "     float4 ObjPos   : POSITION;              \n" + // Object space position 
                    "     float2 TexCoord : TEXCOORD0;             \n" +
                    "     float3 ObjNormal: NORMAL;                \n";  // Object space normal
                if (key.overlayTextureCount > 0)
                {
                    vsIn +=
                        "     float2 LongLatCoord : TEXCOORD1;     \n";
                }
                else if (needSurfaceTangents)
                {
                    vsIn +=
                        "     float3 ObjTangent : TANGENT;       \n";
                }

                if (hasVertexColors)
                {
                    vsIn +=
                        "     float4 Color : COLOR0;                \n";
                }
                if (RenderContext11.ExternalProjection)
                {
                    vsIn +=
                        "     uint   instId   : SV_InstanceID;       \n  ";
                }
                vsIn +=
                    " };                                           \n";
            }

            // Generate the vertex shader output/pixel shader input

            psIn =
                    " struct VS_OUT                                \n" +
                    " {                                            \n" +
                    "     float4 ProjPos   : SV_POSITION;          \n" + // Projected space position 
                    "     float2 TexCoord  : TEXCOORD0;            \n";

            if (needSurfaceNormals)
            {
                psIn +=
                    "     float3 ObjNormal : TEXCOORD1_CENTROID;        \n";
            }

            if (needSurfaceTangents)
            {
                psIn +=
                    "     float3 ObjTangent : TEXCOORD7;               \n";
            }

            if (needObjectSpacePosition)
            {
                psIn +=
                    "     float3 ObjPos : TEXCOORD2_CENTROID;           \n";
            }

            if (hasVertexColors)
            {
                // NOTE: Overlaps with atmospheric scattering; this will not be a problem
                // when moving to DX11.
                psIn +=
                    "     float4 Color : COLOR;\n";
            }

            if (key.HasAtmosphere)
            {
                // Centroid sampling is implied for color interpolators. This is
                // important, as the inscatter amount can vary rapidly at the limb
                // of a planet.
                psIn +=
                    "     float4 AtmosphereInscatter : COLOR;    \n" +
                    "     float4 AtmosphereExtinction : COLOR1;  \n";
            }
            for (int shadowIndex = 0; shadowIndex < key.eclipseShadowCount; ++shadowIndex)
            {
                psIn +=
                    "     float4 EclipseShadowCoord" + shadowIndex + " : TEXCOORD" + eclipseShadowCoordIndex(shadowIndex) + ";   \n";
            }
            if (key.HasRingShadows || key.style == PlanetSurfaceStyle.PlanetaryRings)
            {
                psIn +=
                    "     float4 RingShadowCoord : TEXCOORD" + ringShadowCoordIndex() + ";   \n";
            }
            for (int overlayIndex = 0; overlayIndex < key.overlayTextureCount; ++overlayIndex)
            {
                psIn +=
                    "     float2 OverlayTexCoord" + overlayIndex + " : TEXCOORD" + overlayTexCoordIndex(overlayIndex) + ";  \n";
            }

            vsOut = psIn;
            gsOut = psIn;

            if (RenderContext11.ExternalProjection)
            {
                vsOut +=
                  //   "     uint        viewId  : TEXCOORD" + (key.overlayTextureCount + 1) + ";               \n";
                     "     uint        viewId  : TEXCOORD" + (8) + ";               \n";
                gsOut +=
                     "     uint        rtvId  : SV_RenderTargetArrayIndex;               \n";
            } 

            psIn +=
                    " };                                           \n";

            vsOut +=
                " };                                           \n";

            gsOut +=
                " };                                           \n";

            gsOut = gsOut.Replace("struct VS_OUT", "struct GS_OUT");

            bool buildGeometryShader = false;
            string geometryShaderSource = "";
            if (RenderContext11.ExternalProjection)
            {
                buildGeometryShader = true;
                geometryShaderSource = MakePassThruGeometry(vsOut, gsOut);
            }

            shaderConstantDecl =
                declareConstant("float4x4", "matWVP", 0) +
                declareConstant("float", "opacity", 8) +
                declareConstant("float3", "sunDirection", 9) +
                declareConstant("float3", "cameraPosition", 10);

            if (needCameraSpacePosition)
            {
                shaderConstantDecl +=
                    declareConstant("float4x4", "matWV", 4);
            }

            if (key.HasAtmosphere)
            {
                shaderConstantDecl +=
                    declareConstant("float3", "rayleighScatterCoeff", 11) +
                    declareConstant("float", "atmosphereHeight", 12) +
                    declareConstant("float", "atmosphereZeroRadius", 13) +
                    declareConstant("float", "atmosphereInvScaleHeight", 14) +
                    declareConstant("float3", "atmosphereCenter", 15);
            }

            for (int shadowIndex = 0; shadowIndex < key.eclipseShadowCount; ++shadowIndex)
            {
                shaderConstantDecl +=
                    declareConstant("float4x4", "matEclipseShadow" + shadowIndex, 16 + 4 * shadowIndex);
            }

            for (int overlayIndex = 0; overlayIndex < key.overlayTextureCount; ++overlayIndex)
            {
                shaderConstantDecl +=
                    declareConstant("float4x4", "matOverlayTexture" + overlayIndex, 32 + 4 * overlayIndex);
            }

            if (RenderContext11.ExternalProjection)
            {
                // Generate the body of the vertex shader
                vertexShaderText +=
                    " VS_OUT VSMain(VS_IN In)              \n" +
                    " {                                            \n" +
                    "     VS_OUT Out;                              \n" +

                    "     int idx = In.instId % 2;                  \n" +
                    "     float4 pp = mul(In.ObjPos,  matWVP );      \n" + // Transform vertex into                  
                                                                          //                  "     Out.ProjPos = mul(p, viewProjection[idx]); \n" +
                    "     Out.ProjPos = mul(pp, viewProjection[idx]); \n" +
                    "     Out.viewId = idx;                        \n" +
                    "     Out.TexCoord = In.TexCoord;              \n";
            }
            else
            {

                // Generate the body of the vertex shader
                vertexShaderText +=
                        " VS_OUT VSMain(VS_IN In)              \n" +
                        " {                                            \n" +
                        "     VS_OUT Out;                              \n" +
                        "     Out.ProjPos = mul(In.ObjPos, matWVP);    \n" + // Transform vertex from world into device coordinates
                        "     Out.TexCoord = In.TexCoord;              \n";

            }
            if (needCameraSpacePosition)
            {
                vertexShaderText +=
                    "     float4 cameraSpacePos = mul(In.ObjPos, matWV);\n";
            }

            if (needSurfaceNormals)
            {
                vertexShaderText +=
                    "     Out.ObjNormal = In.ObjNormal;\n";
            }

            if (needObjectSpacePosition)
            {
                vertexShaderText +=
                    "     Out.ObjPos = In.ObjPos.xyz;\n";
            }

            if (needSurfaceTangents)
            {
                vertexShaderText +=
                    "     Out.ObjTangent = In.ObjTangent;\n";
            }

            if (hasVertexColors)
            {
                vertexShaderText +=
                    "     Out.Color = In.Color;\n";
            }

            if (key.HasAtmosphere)
            {
                // Compute two values:
                //    inscattering along the path from the eye to vertex
                //    total extinction on the path from sun to vertex to eye
                //
                // Both inscattering and extinction are wavelength dependent, so
                // the values are 3-vectors, with each component representing one
                // wavelength of light (680nm, 550nm, and 440nm for red, green,
                // and blue.)
                //
                // The scattering integral is approximated by summing a small number
                // of discrete samples along the view ray.
                //
                // Currently not taken into account are:
                //   - Absorption (not a major factor in Earth's atmosphere)
                //   - Mie scattering
                //   - Rayleigh and Mie phase functions

                vertexShaderText +=
                    "     float3 extinctionCoeff = rayleighScatterCoeff; // +mieScatterCoeff     \n" +
                    "     float3 objPos = In.ObjPos + atmosphereCenter;                          \n" +
                    "     float3 camPos = cameraPosition + atmosphereCenter;                     \n" +
                    "     float atmRadius = 1.0 + atmosphereHeight;                              \n" +
                    "     float3 v = objPos - camPos;                                            \n" +
                    "     float a = dot(v, v);                                                   \n" +
                    "     float b = dot(v, camPos);                                              \n" +
                    "     float c = dot(camPos, camPos) - atmRadius * atmRadius;                 \n" +
                    "     float t = max(0.0, (-b - sqrt(b * b - a * c)) / a);                    \n" +
                    "     float3 atmIntersection = camPos + t * v;                               \n" +
                    "     float atmDistance = distance(atmIntersection, objPos);                 \n" +
                    "     float3 inscatter = 0.0;                                                \n" +
                    "     float opticalDepth = 0.0;                                              \n" +
                    "     float3 p = atmIntersection;                                            \n" +   // p : sample point
                    "     float3 step = (objPos - p) / 5.0;                                      \n" +
                    "     for (int i = 0; i < 5; ++i) {                                          \n" +
                    "         float pDotS = dot(p, sunDirection);                                \n" +
                    "         b = pDotS;                                                         \n" +   // Compute transmissivity along ray from sample point to sun; path clamped at sky
                    "         c = dot(p, p) - atmRadius * atmRadius;                             \n" +
                    "         float u = -b + sqrt(max(0.0, b * b - c));                                    \n" +        // u : distance from point to sky
                    "         float hMid = length(p + sunDirection * (u * 0.5)) - atmosphereZeroRadius;\n" +  // hMid : height at midpoint of sample point sky path
                    "         float tau = u * exp(min(50.0, -atmosphereInvScaleHeight * hMid));  \n" +
                    "         float distToCenter = length(p - pDotS * sunDirection);             \n" +
                    "         float blockedFraction = 1.0 - clamp((distToCenter - 0.8 * atmosphereZeroRadius) / (0.2 * atmosphereZeroRadius), 0.0, 1.0);\n" +
                    "         c = dot(p, p) - atmosphereZeroRadius * atmosphereZeroRadius;       \n" +
                    "         if (b * b - c >= 0.0 && -sqrt(abs(b * b - c)) > b) tau = max(tau, 50.0 * blockedFraction);\n" +   // Block all light when ray intersects the planet
                    "         float h = length(p) - atmosphereZeroRadius;                        \n" +
                    "         float d = exp(-atmosphereInvScaleHeight * h);                      \n" +
                    "         inscatter += d * rayleighScatterCoeff * exp(-(opticalDepth + tau) * extinctionCoeff);\n" +
                    "         opticalDepth += d * atmDistance / 5.0;                             \n" +
                    "         p = p + step;                                                      \n" +   // Step the sample point
                    "     }                                                                      \n" +
                    "     float hMid = length((atmIntersection + objPos) * 0.5) - atmosphereZeroRadius;\n" +
                    "     Out.AtmosphereExtinction.rgb = exp(-opticalDepth * extinctionCoeff);   \n" +
                    "     Out.AtmosphereExtinction.a = hMid;                                     \n" +
                    "     float mu = dot(v, sunDirection) / length(v);                           \n" +
                    "     float3 inscatterColor = pow(inscatter * atmDistance / 5.0, 1.0);       \n"; // adjust exponent for atmosphere 'gamma'

                if (key.style == PlanetSurfaceStyle.Sky)
                {
                    // For the sky shader, the atmosphere opacity is adjusted based on the amount of
                    // inscattered light. This prevents the stars from showing during the daytime.
                    vertexShaderText +=
                    "     Out.AtmosphereInscatter = float4(inscatterColor, opacity * dot(float3(2.0, 3.0, 4.0), inscatterColor));\n";
                }
                else
                {
                    vertexShaderText +=
                    "     Out.AtmosphereInscatter = float4(inscatterColor, 0.0);\n";
                }

            }

            for (int shadowIndex = 0; shadowIndex < key.eclipseShadowCount; ++shadowIndex)
            {
                vertexShaderText +=
                    "    Out.EclipseShadowCoord" + shadowIndex + " = mul(cameraSpacePos, matEclipseShadow" + shadowIndex + ");\n";
            }

            if (key.HasRingShadows)
            {
                vertexShaderText +=
                    "    float ringPlaneIntersection = In.ObjPos.y / -sunDirection.y;\n" +
                    "    float3 ringPlaneIntersectionPoint = In.ObjPos.xyz + sunDirection * ringPlaneIntersection;\n" +
                    "    Out.RingShadowCoord = float4(ringPlaneIntersectionPoint.x, ringPlaneIntersectionPoint.z, ringPlaneIntersection, 0.0);\n";
            }
            else if (key.style == PlanetSurfaceStyle.PlanetaryRings)
            {
                vertexShaderText +=
                    "    float3 v1 = cross(sunDirection, float3(0, 1, 0));\n" +
                    "    v1 = normalize(v1);                              \n" +
                    "    float3 v2 = cross(v1, sunDirection);             \n" +
                    "    Out.RingShadowCoord = float4(dot(v1, In.ObjPos), dot(v2, In.ObjPos), dot(sunDirection, In.ObjPos), 0.0);\n";
            }

            // Transform the long/lat coord to get overlay texture coordinates
            for (int overlayIndex = 0; overlayIndex < key.overlayTextureCount; ++overlayIndex)
            {
                vertexShaderText +=
                    "    Out.OverlayTexCoord" + overlayIndex + " = mul(float4(In.LongLatCoord.x, In.LongLatCoord.y, 1.0, 0.0), matOverlayTexture" + overlayIndex + ").xy;\n";
            }
           
            vertexShaderText +=
                "     return Out;                              \n" + // Transfer color
                " }                                            \n";

            bool viewDependentLighting = false;
            if (style == PlanetSurfaceStyle.Specular ||
                style == PlanetSurfaceStyle.SpecularPass ||
                style == PlanetSurfaceStyle.LommelSeeliger)
            {
                viewDependentLighting = true;
            }

            bool hasSpecular = false;
            if (style == PlanetSurfaceStyle.Specular ||
                style == PlanetSurfaceStyle.SpecularPass)
            {
                hasSpecular = true;
            }

            bool hasHemisphereLight = true;

            // Generate the pixel shader source

            const int PixelShaderConstantStart = 48;

            // Generate the pixel shader constants
            for (int i = 0; i < key.lightCount; ++i)
            {
                shaderConstantDecl +=
                    declareConstant("float3", "lightDirection" + i, PixelShaderConstantStart + 0 + i) +
                    declareConstant("float3", "lightColor" + i, PixelShaderConstantStart + 4 + i);
            }

            shaderConstantDecl +=
                declareConstant("float3", "ambientLightColor", PixelShaderConstantStart + 8) +
                declareConstant("float4", "diffuseColor", PixelShaderConstantStart + 9); 

            // Constants for hemisphere lighting
            if (hasHemisphereLight)
            {
                shaderConstantDecl +=
                    declareConstant("float3", "hemiLightColor", PixelShaderConstantStart + 10) +
                    declareConstant("float3", "hemiLightUp", PixelShaderConstantStart + 11);
            }

            textureDecl +=
                    "Texture2D mainTexture : register(" + RenderContext11.PixelProfile + ", t0);         \n";

            if (style == PlanetSurfaceStyle.DiffuseAndNight)
            {
                textureDecl +=
                    "Texture2D nightTexture : register(" + RenderContext11.PixelProfile + ", t2);        \n";
            }

            if (style == PlanetSurfaceStyle.Specular ||
                style == PlanetSurfaceStyle.SpecularPass)
            {
                shaderConstantDecl +=
                    declareConstant("float4", "specularColor", PixelShaderConstantStart + 12) +
                    declareConstant("float", "specularPower", PixelShaderConstantStart + 13);
                if (key.hasTexture(PlanetShaderKey.SurfaceProperty.Specular))
                {
                    textureDecl +=
                        "Texture2D specularTexture : register(" + RenderContext11.PixelProfile + ", t" + specularSamplerIndex() + ");\n";
                }
            }

            if (needSurfaceTangents)
            {
                textureDecl +=
                    "Texture2D normalTexture : register(" + RenderContext11.PixelProfile + ", t" + normalSamplerIndex() + ");\n";
            }

            if (key.eclipseShadowCount > 0)
            {
                textureDecl +=
                    "Texture2D eclipseTexture : register(" + RenderContext11.PixelProfile + ", t" + eclipseShadowSamplerIndex() + ");\n";

            }

            if (key.HasRingShadows)
            {
                textureDecl +=
                    "Texture2D ringTexture : register(" + RenderContext11.PixelProfile + ", t" + ringShadowSamplerIndex() + ");\n";
            }

            for (int overlayIndex = 0; overlayIndex < key.overlayTextureCount; ++overlayIndex)
            {
                textureDecl +=
                    "Texture2D overlayTexture" + overlayIndex + " : register(" + RenderContext11.PixelProfile + ", t" + overlayTextureSamplerIndex(overlayIndex) + ");\n";
                shaderConstantDecl += declareConstant("float4", "overlayColor" + overlayIndex, PixelShaderConstantStart + 14 + overlayIndex);
            }

            // End of pixel shader constants

//            pixelShaderText +=
//                " struct PS_OUT {\n" +
//                "     float4 color : SV_Target0;\n" +
////                "     float depth : SV_Depth;\n" +
//                " };\n";
            if (key.TwoSidedLighting)
            {
                pixelShaderText +=
                    " float4 PSMain(VS_OUT In, bool face : SV_IsFrontFace) : SV_TARGET \n" +
                    " {                                                     \n";
            }
            else
            {
                pixelShaderText +=
                    " float4 PSMain(VS_OUT In) : SV_TARGET                     \n" +
                    " {                                             \n";
            }

            // localDiffuseColor is the _unlit_ surface color, i.e. the albedo
            if (key.hasTexture(PlanetShaderKey.SurfaceProperty.Diffuse))
            {
                if (key.AlphaTexture)
                {
                    // Use only the alpha component from the texture
                    pixelShaderText +=
                        "     float4 localDiffuseColor = float4(diffuseColor.rgb, diffuseColor.a * mainTexture.Sample(MainTextureSampler, In.TexCoord).a);\n";
                }
                else
                {
                    pixelShaderText +=
                        "     float4 localDiffuseColor = diffuseColor * mainTexture.Sample(MainTextureSampler, In.TexCoord);\n";
                }
            }
            else
            {
                pixelShaderText +=
                    "     float4 localDiffuseColor = diffuseColor;\n";
            }

            if (hasVertexColors)
            {
                pixelShaderText +=
                    "     localDiffuseColor *= In.Color;\n";
            }

            if (style == PlanetSurfaceStyle.Specular)
            {
                if (key.hasTexture(PlanetShaderKey.SurfaceProperty.Specular))
                {
                    pixelShaderText +=
                        " float3 localSpecularColor = specularColor.rgb * specularTexture.Sample(MainTextureSampler, In.TexCoord).rgb;\n";
                }
                else
                {
                    pixelShaderText +=
                        " float3 localSpecularColor = specularColor.rgb;                         \n";
                }
            }
            else if (style == PlanetSurfaceStyle.SpecularPass)
            {
                pixelShaderText +=
                    " float3 localSpecularColor = specularColor.rgb * mainTexture.Sample(MainTextureSampler, In.TexCoord).rgb;\n";
            }

            // Merge in color from any overlay textures
            for (int overlayIndex = 0; overlayIndex < key.overlayTextureCount; ++overlayIndex)
            {
                pixelShaderText +=
                    "     {\n" +
                    "         float4 overlayCombinedColor = overlayColor" + overlayIndex + " * overlayTexture" + overlayIndex + ".Sample(TransparentBorderSampler, In.OverlayTexCoord" + overlayIndex + ");       \n" +
                    "         localDiffuseColor.rgb = lerp(localDiffuseColor.rgb, overlayCombinedColor.rgb, overlayCombinedColor.a);    \n" +
                    "     }\n";
            }

            // Get the surface normal at this pixel. The components are linearly interpolated,
            // so we need to normalize it.
            if (style != PlanetSurfaceStyle.Emissive && style != PlanetSurfaceStyle.PlanetaryRings)
            {
                string normalSign = "";
                if (key.TwoSidedLighting)
                    normalSign = " * (face ? 1 : -1)";

                if (needSurfaceTangents)
                {
                    // Normal mapping is enabled. Compute the normal to use for lighting by
                    // sampling the normal map and rotating it from the local tangent space into
                    // object space.
                    pixelShaderText +=
                        "     float3 N = normalize(In.ObjNormal)" + normalSign + ";\n" +
                        "     float3 T = normalize(In.ObjTangent);\n" +
                        "     float3 B = cross(N, T);\n" +
                        "     float3 Nsurf = normalTexture.Sample(MainTextureSampler, In.TexCoord) * 2.0 - 1.0;\n" +
                        "     N = normalize(mul(Nsurf, float3x3(T, B, N)));\n";
                }
                else
                {
                    pixelShaderText +=
                        "     float3 N = normalize(In.ObjNormal)" + normalSign + ";\n";
                }
            }

            // Calculate lighting based on surface normal, view direction, and reflectance model
            if (viewDependentLighting)
            {
                // Only compute the view vector if it's actually required
                pixelShaderText +=
                    "     float3 viewVector = normalize(cameraPosition - In.ObjPos);\n";
            }

            if (style == PlanetSurfaceStyle.Emissive || style == PlanetSurfaceStyle.PlanetaryRings)
            {
                pixelShaderText +=
                    "     float3 litDiffuseColor = localDiffuseColor.rgb;\n";
            }
            else
            {
                // Declare and initiliaze the total diffuse and specular light contributions
                pixelShaderText +=
                    "     float3 litDiffuseColor = ambientLightColor;\n";
                if (hasHemisphereLight)
                {
                    pixelShaderText +=
                        "     litDiffuseColor += hemiLightColor * (0.5 * (1.0 + dot(N, hemiLightUp)));\n";
                }

                if (hasSpecular)
                {
                    pixelShaderText +=
                        "     float3 litSpecularColor = 0.0;\n";
                }

                // Compute N * V (constant for all light sources)
                if (style == PlanetSurfaceStyle.LommelSeeliger)
                {
                    pixelShaderText +=
                        "     float NdotV = dot(N, viewVector);\n";
                }

                // Compute the contribution from each light source and sum them
                for (int lightIndex = 0; lightIndex < key.lightCount; ++lightIndex)
                {
                    pixelShaderText += "     {\n";
                    pixelShaderText +=
                        "         float3 L = lightDirection" + lightIndex + ";\n" +
                        "         float3 lightColor = lightColor" + lightIndex + ";\n";
                    pixelShaderText +=
                        "         float NdotL = dot(N, L);\n";

                    // Diffuse contribution
                    if (style == PlanetSurfaceStyle.LommelSeeliger)
                    {
                        pixelShaderText +=
                            "         float d = NdotL / max(0.001, NdotV + NdotL);                         \n" +
                            "         litDiffuseColor += max(0.0, d) * lightColor; \n";
                    }
                    else
                    {
                        pixelShaderText +=
                            "         litDiffuseColor += max(0.0, NdotL) * lightColor;\n";
                    }

                    // Specular contribution
                    if (style == PlanetSurfaceStyle.Specular ||
                        style == PlanetSurfaceStyle.SpecularPass)
                    {
                        pixelShaderText +=
                            "         float3 halfAngleVec = normalize(viewVector + L);\n" +
                            "         float specularFactor = pow(max(0.0, dot(N, halfAngleVec)), specularPower) * smoothstep(-0.1, 0.0, NdotL); \n" +
                            "         litSpecularColor += specularFactor * localSpecularColor.rgb * lightColor;\n";
                    }

                    pixelShaderText += "     }\n";
                }

                // Modify the total diffuse light from all sources by the diffuse color at this pixel
                pixelShaderText += "     litDiffuseColor *= localDiffuseColor.rgb;\n";
            }

            // Combine the total diffuse and specular contributions from all light sources
            // with the surface color.
            if (style == PlanetSurfaceStyle.Specular)
            {
                pixelShaderText +=
                    "     float4 color = float4(litDiffuseColor, localDiffuseColor.a) + float4(litSpecularColor, 0.0);\n";
            }
            else if (style == PlanetSurfaceStyle.SpecularPass)
            {
                pixelShaderText +=
                    "     float4 color = float4(litSpecularColor, 1.0);\n";
            }
            else if (style == PlanetSurfaceStyle.DiffuseAndNight)
            {
                pixelShaderText +=
                    "     float4 day = diffuseColor;                            \n" +
                    "     float4 night = nightTexture.Sample(MainTextureSampler, In.TexCoord);   night.rgb = pow(night.rgb, 2.2);   \n" +
                    "     float4 color = lerp(night, day, float4(litDiffuseColor, localDiffuseColor.a));\n";
            }
            else
            {
                pixelShaderText +=
                    "     float4 color = float4(litDiffuseColor, localDiffuseColor.a);\n";
            }

            pixelShaderText +=
                "    float4 shadow = 1.0;   \n";

            bool hasShadows = key.eclipseShadowCount > 0 || key.HasRingShadows || key.style == PlanetSurfaceStyle.PlanetaryRings;

            // TODO: Shadowing should be handled in the light source loop
            // Not a problem for current lighting situations, however.
            if (hasShadows)
            {
                for (int shadowIndex = 0; shadowIndex < key.eclipseShadowCount; ++shadowIndex)
                {
                    string shadowCoord = "In.EclipseShadowCoord" + shadowIndex;
                    pixelShaderText +=
                        "     shadow *= eclipseTexture.Sample(ClampTextureSampler, " + shadowCoord + ".xy / " + shadowCoord + ".w);   \n";
                }
                if (key.HasRingShadows)
                {
                    // Using fixed values appropriate for Saturn's rings:
                    //   inner radius = 1.113 * Rsat
                    //   outer radius = 2.25 * Rsat
                    //   difference: 1.137
                    pixelShaderText +=
                        "     if (In.RingShadowCoord.z > 0.0) {                                               \n" +
                        "         float ringTexCoord = 1.0 - (length(In.RingShadowCoord.xy) - 1.113) / 1.137; \n" +
                        "         shadow.rgb *= 1.0 - ringTexture.Sample(RingShadowTextureSampler, ringTexCoord).a;        \n" +
                        "     }   \n";
                }
                if (key.style == PlanetSurfaceStyle.PlanetaryRings)
                {
                    pixelShaderText +=
                        "    shadow.rgb *= In.RingShadowCoord.z > 0.0 ? 1.0 : step(1.0, length(In.RingShadowCoord.xy));    \n";
                }

                pixelShaderText +=
                    "    color *= shadow;   \n";
            }

            //pixelShaderText +=
            //        "    PS_OUT Out;\n";
            if (key.HasAtmosphere)
            {
                pixelShaderText +=
 //                   "    Out.color = color * float4(In.AtmosphereExtinction.rgb, 1.0) + In.AtmosphereInscatter * shadow * (In.AtmosphereExtinction.a < -0.001 ? 0.0 : 1.0);   \n";
                    "    color = color * float4(In.AtmosphereExtinction.rgb, 1.0) + In.AtmosphereInscatter * shadow * (In.AtmosphereExtinction.a < -0.001 ? 0.0 : 1.0);   \n";
            }
            //else
            //{
            //    pixelShaderText +=
            //        "    Out.color = color;\n";
            //        "    Out.color = color;\n";
            //}

            /*
            pixelShaderText +=
                "     Out.depth = 0.0;\n";
            */

            //pixelShaderText +=
            //    "     return Out;\n";



            pixelShaderText +=
                "     return color;\n";
 
            pixelShaderText +=
                    " }\n";


            shaderConstantDecl =
                "cbuffer all {\n" +
                shaderConstantDecl +
                "};\n";

            if (RenderContext11.ExternalProjection)
            {
                shaderConstantDecl += (
                // A constant buffer that stores each set of view and projection matrices in column-major format.
                  "                                                        \n" +
                  "  cbuffer ViewProjectionConstantBuffer : register(b1)    \n" +
                  "  {                                                      \n" +
                  "                  float4x4 viewProjection[2];            \n" +
                  "  };                                                     \n" +
                  "                                                         \n");
            }

            string pixelShaderSource = psIn + "\n" +
                                       shaderConstantDecl + "\n" +
                                       textureDecl + "\n" +
                                       samplerStateText + "\n" +
                                       pixelShaderText + "\n";
            string vertexShaderSource = vsIn + "\n" +
                                        vsOut + "\n" +
                                        shaderConstantDecl + "\n" +
                                        vertexShaderText;

            pixelShaderBytecode = ShaderLibrary.Instance.getShaderBytecode(key.ToString(), pixelShaderSource, "PSMain", pixelProfile);
            vertexShaderBytecode = ShaderLibrary.Instance.getShaderBytecode(key.ToString(), vertexShaderSource, "VSMain", vertexProfile);
            if (RenderContext11.ExternalProjection && buildGeometryShader)
            {
                geometryShaderBytecode  = ShaderLibrary.Instance.getShaderBytecode(key.ToString(), geometryShaderSource, "GSMain", "gs_4_0");
            }
        }

        private string MakePassThruGeometry(string vsOut, string gsOut)
        {

            string code = vsOut;

            code += " \n";
            code += gsOut;

            code += " \n";

            code +=
             "   [maxvertexcount(3)]                                                                                              \n" +
             "   void GSMain(triangle VS_OUT input[3], inout TriangleStream< GS_OUT > outStream)         \n" +
             "   {                                                                                                                \n" +
             "                   GS_OUT output;                                                                     \n" +
             "       [unroll(3)]                                                                                                  \n" +
             "       for (int i = 0; i< 3; ++i)                                                                                   \n" +
             "       {                                                                                                            \n";
            //"           output.pos = input[i].pos;                                                                               \n" +
            //"           output.color = input[i].color;                                                                           \n" +
            //"           output.rtvId = input[i].instId;                                                                          \n" +

            string[] inputs = GetMembersList(vsOut);
            string[] outputs = GetMembersList(gsOut);

            if (inputs.Length != outputs.Length)
            {
                throw new InvalidOperationException("Input and output shader structures must have the same number of members and have colons");
            }

            for(int i = 0; i < inputs.Length; i++)
            {
                code += string.Format("     output.{0} = input[i].{1}; \n", outputs[i], inputs[i]);
            }

            code +=
             "           outStream.Append(output);                                                                                \n" +
             "       }                                                                                                            \n" +
             "   } \n";

            return code;      
        }

        private static string[] GetMembersList(string structCode)
        {
            string[] lines = structCode.Split(new char[] { '\n' });

            List<string> members = new List<string>();
            foreach(string line in lines)
            {
                if (line.Contains(":"))
                {
                    string[] parts = line.Split(new char[] { ' ' });
                    string name="";
                    foreach(string part in parts)
                    {
                        if (part == ":")
                        {
                            members.Add(name);
                            break;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(part))
                            {
                                name = part;
                            }
                        }
                    }
                }
            }

            return members.ToArray();
        }

        // Actually create the Direct3D resources for this shader on a device
        private void Realize(Device device)
        {
            if (pixelShaderBytecode != null)
            {
                pixelShader = new PixelShader(RenderContext11.PrepDevice, pixelShaderBytecode);
            }
            if (vertexShaderBytecode != null)
            {
                vertexShader = new VertexShader(RenderContext11.PrepDevice, vertexShaderBytecode);
            }
            if (geometryShaderBytecode != null)
            {
                geometryShader = new GeometryShader(RenderContext11.PrepDevice, geometryShaderBytecode);
            }
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<PlanetShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }


        public SharpDX.Direct3D11.InputLayout inputLayout(StandardVertexLayout vertexType)
        {
            int index = (int) vertexType;
            var device = RenderContext11.PrepDevice;

            if (layoutCache[index] == null)
            {
                SharpDX.Direct3D11.InputLayout layout = null;

                switch (vertexType)
                {
                    case StandardVertexLayout.Position:
                        layout = new SharpDX.Direct3D11.InputLayout(device, InputSignature, new[]
                           {
                               new SharpDX.Direct3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float,     0, 0),
                           });
                        break;
                    case StandardVertexLayout.PositionNormal:
                        layout = new SharpDX.Direct3D11.InputLayout(device, InputSignature, new[]
                           {
                               new SharpDX.Direct3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float,     0, 0),
                               new SharpDX.Direct3D11.InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32_Float,      12, 0),
                           });
                        break;
                    case StandardVertexLayout.PositionNormalTex:
                        layout = new SharpDX.Direct3D11.InputLayout(device, InputSignature, new[]
                           {
                               new SharpDX.Direct3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float,     0, 0),
                               new SharpDX.Direct3D11.InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32_Float,      12, 0),
                               new SharpDX.Direct3D11.InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float,       24, 0)
                           });
                        break;
                    case StandardVertexLayout.PositionNormalTex2:
                        layout = new SharpDX.Direct3D11.InputLayout(device, InputSignature, new[]
                           {
                               new SharpDX.Direct3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float,     0, 0),
                               new SharpDX.Direct3D11.InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32_Float,      12, 0),
                               new SharpDX.Direct3D11.InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float,       24, 0),
                               new SharpDX.Direct3D11.InputElement("TEXCOORD", 1, SharpDX.DXGI.Format.R32G32_Float,       32, 0)
                           });
                        break;
                    case StandardVertexLayout.PositionNormalTexTangent:
                        layout = new SharpDX.Direct3D11.InputLayout(device, InputSignature, new[]
                           {
                               new SharpDX.Direct3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float,     0, 0),
                               new SharpDX.Direct3D11.InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32_Float,      12, 0),
                               new SharpDX.Direct3D11.InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float,       24, 0),
                               new SharpDX.Direct3D11.InputElement("TANGENT", 0, SharpDX.DXGI.Format.R32G32B32_Float,        32, 0)
                           });
                        break;
                }

                layoutCache[index] = layout;
            }

            return layoutCache[index];
        }

    }


    public sealed class ShaderLibrary
    {
        private static ShaderLibrary instance = InitShaderLibrary();
        private Dictionary<string, byte[]> compiledShaders;
        private bool AttemptedLoadingPrecompiledShaders = false;

        public struct ShaderLevel
        {
            public FeatureLevel featureLevel;
            public string vertexProfile;
            public string pixelProfile;
            public string geometryProfile;
        }

        private ShaderLevel[] shaderLevels = 
        {
            new ShaderLevel { featureLevel = FeatureLevel.Level_10_0, vertexProfile = "vs_4_0", pixelProfile = "ps_4_0", geometryProfile = "gs_4_0" },                                    
            new ShaderLevel { featureLevel = FeatureLevel.Level_9_3, vertexProfile = "vs_4_0_level_9_3", pixelProfile = "ps_4_0_level_9_3", geometryProfile = null },                                    
            new ShaderLevel { featureLevel = FeatureLevel.Level_9_1, vertexProfile = "vs_4_0_level_9_1", pixelProfile = "ps_4_0_level_9_1", geometryProfile = null },
        };

        public static ShaderLevel[] ShaderLevels
        {
            get
            {
                return instance.shaderLevels;
            }
        }

        private ShaderLibrary()
        {
            if (UsePrecompiledShaders)
            {
                compiledShaders = LoadPrecompiledShaderLibrary();
            }
            else
            {
                compiledShaders = new Dictionary<string, byte[]>();
            }
        }

        public static ShaderLibrary InitShaderLibrary()
        {
            instance = new ShaderLibrary();
            return instance;
        }

        public static void DumpShaderLibrary()
        {
            if (!UsePrecompiledShaders)
            {
                ShaderBundle.CompileAndSaveShaders();
                PlanetShader11.CompileAndSaveShaders();
                instance.SavePrecompiledShaders();
            }
        }

        public static ShaderLibrary Instance
        {
            get
            {
                return instance;
            }
        }

        // Set this to false when developing so that updates to shaders are visible 
        // without rebuilding the compiled shader library.
        public static bool UsePrecompiledShaders;

        void SavePrecompiledShaders()
        {

#if !WINDOWS_UWP
            System.IO.FileStream fs = null;
            try
            {
                string shaderFile = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\wwtshaders.bin";
                System.Diagnostics.Debug.WriteLine("Writing shader library to " + shaderFile);
                fs = System.IO.File.OpenWrite(shaderFile);
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(fs, compiledShaders);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error writing precompiled shader file: " + e.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
#endif
        }


        Dictionary<string, byte[]> LoadPrecompiledShaderLibrary(System.IO.Stream stream)
        {

#if !WINDOWS_UWP
            Dictionary<String, byte[]> shaders = null;

            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            try
            {
                shaders = (Dictionary<String, byte[]>)formatter.Deserialize(stream);
            }
            catch (System.Runtime.Serialization.SerializationException e)
            {
                System.Console.WriteLine("OOPS: " + e.Message);
            }

            return shaders;
#else
            return null;
#endif
        }

        Dictionary<string, byte[]> LoadPrecompiledShaderLibrary()
        {

#if !WINDOWS_UWP
            Dictionary<String, byte[]> shaders = null;
            System.IO.FileStream fs = null;
            try
            {
                System.Diagnostics.Debug.WriteLine("Loading precompiled shaders from " + ShaderLibrary.PrecompiledShaderFile);
                fs = System.IO.File.OpenRead(ShaderLibrary.PrecompiledShaderFile);
                shaders = LoadPrecompiledShaderLibrary(fs);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error reading precompiled shaders: " + e.Message);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            } return shaders;
#else
            return null;
#endif
        }

        public static string PrecompiledShaderFile
        {
            get
            {
                return AppSettings.SettingsBase["CahceDirectory"].ToString() + @"data/shadercache.bin";
            }
        }

        // Compile shader source and insert it into the list of compiled shaders
        // using the specified key.
        public byte[] compileShader(string key, string sourceText, string entryPoint, string profile)
        {
            byte[] bytecode = null;
            try
            {

                bytecode = ShaderBytecode.Compile(sourceText, entryPoint, profile);
            }
            catch (SharpDX.CompilationException e)
            {

#if !WINDOWS_UWP
                System.Console.WriteLine(e.Message);
#endif
            }

            string libraryKey = profile + key;
            compiledShaders[libraryKey] = bytecode;

            return bytecode;
        }

        // Get the bytecode for the shader with the specified key
        public byte[] getShaderBytecode(string key, string sourceText, string entryPoint, string profile)
        {
            string libraryKey = profile + key;
            if (UsePrecompiledShaders)
            {
                return compiledShaders[libraryKey];
            }
            else
            {
                return compileShader(key, sourceText, entryPoint, profile);
            }
        }
    }

    public abstract class ShaderBundle
    {
        public static void CompileAndSaveShaders()
        {

#if !WINDOWS_UWP
            Type[] constructorSignature = {};

            Assembly thisAssembly = Assembly.GetAssembly(typeof(ShaderBundle));
            var allTypes = thisAssembly.GetTypes();
            var shaderClasses = new List<Type>();
            foreach (var t in allTypes)
            {
                if (t.IsSubclassOf(typeof(ShaderBundle)) && !t.IsAbstract)
                {
                    foreach (var level in ShaderLibrary.ShaderLevels)
                    {
                        var constructor = t.GetConstructor(constructorSignature);
                        ShaderBundle bundle = (ShaderBundle)constructor.Invoke(null);
                        if (bundle.isFeatureLevelSupported(level.featureLevel))
                        {
                            bundle.CompileShader(level.vertexProfile, level.pixelProfile);
                            /*
                            ShaderLibrary.Instance.setShaderBytecode(bundle.Key, vertexProfiles[i], bundle.VertexShaderBytecode);
                            ShaderLibrary.Instance.setShaderBytecode(bundle.Key, pixelProfiles[i], bundle.PixelShaderBytecode);
                            if (bundle.GeometryShaderBytecode != null)
                            {
                                ShaderLibrary.Instance.setShaderBytecode(bundle.Key, geometryProfiles[i], bundle.GeometryShaderBytecode);
                            }
                             */ 
                        }
                    }
                }
            }
#endif
        }

        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private GeometryShader geometryShader;
        private byte[] vertexShaderBytecode;
        private byte[] pixelShaderBytecode;
        private byte[] geometryShaderBytecode;

        protected VertexShader CompiledVertexShader
        {
            get
            {
                return vertexShader;
            }
        }

        protected PixelShader CompiledPixelShader
        {
            get
            {
                return pixelShader;
            }
        }

        protected GeometryShader CompiledGeometryShader
        {
            get
            {
                return geometryShader;
            }
        }

        protected abstract string GetPixelShaderSource(string profile);
        protected abstract string GetVertexShaderSource(string profile);

        protected virtual string GetGeometryShaderSource(string profile)
        {
            return null;
        }

        public virtual bool isFeatureLevelSupported(FeatureLevel level)
        {
            if (GetGeometryShaderSource("gs_4_0") == null)
            {
                return true;
            }
            else
            {
                return level >= FeatureLevel.Level_10_0;
            }
        }

        public byte[] VertexShaderBytecode
        {
            get
            {
                return vertexShaderBytecode;
            }
        }

        public byte[] PixelShaderBytecode
        {
            get
            {
                return pixelShaderBytecode;
            }
        }

        public byte[] GeometryShaderBytecode
        {
            get
            {
                return geometryShaderBytecode;
            }
        }

        public string Key
        {
            get
            {
                // For the 'fixed' shaders in WWT, the precompiled shader cache key is just the class name
                return this.GetType().Name;
            }
        }

        public void CompileShader(string vertexProfile, string pixelProfile)
        {
            string vertexShaderText = GetVertexShaderSource(vertexProfile);
            vertexShaderBytecode = ShaderLibrary.Instance.getShaderBytecode(Key, vertexShaderText, "VS", vertexProfile);
            vertexShader = new VertexShader(RenderContext11.PrepDevice, vertexShaderBytecode);

            string pixelShaderText = GetPixelShaderSource(pixelProfile);
            pixelShaderBytecode = ShaderLibrary.Instance.getShaderBytecode(Key, pixelShaderText, "PS", pixelProfile);
            pixelShader = new PixelShader(RenderContext11.PrepDevice, pixelShaderBytecode);

            string geometryShaderProfile = "gs_4_0";
            string geometryShaderText = GetGeometryShaderSource(geometryShaderProfile);
            if (geometryShaderText != null)
            {
                geometryShaderBytecode = ShaderLibrary.Instance.getShaderBytecode(Key, geometryShaderText, "GS", geometryShaderProfile);
                geometryShader = new GeometryShader(RenderContext11.PrepDevice, geometryShaderBytecode);
            }
        }

        public ShaderBundle()
        {
        }

    }


    public class SimpleLineShader11 : ShaderBundle
    {
        private static SimpleLineShader11 instance;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static SysColor Color
        {
            set
            {
                constants.Color = new SharpDX.Color4(value.R / 255.0f, value.G / 255.0f, value.B / 255.0f, value.A / 255.0f); ;
            }
        }

        public static Matrix WVPMatrix
        {
            set
            {
                constants.WorldViewProjection = value;
            }
        }

        public static Vector3 CameraPosition
        {
            set
            {
                constants.CameraPosition = new Vector4(value, 1);
            }
        }

        public static bool Sky
        {
            set
            {
                constants.Sky  = value ? -1 : 1;
            }
        }

        public static bool ShowFarSide
        {
            set
            {
                constants.Sky = value ? 1 : 0;
            }
        }

        private static LineShaderConstants constants;

        private static InputLayout layout;

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                    });
            }

            context.InputAssembler.InputLayout = layout;
           
            context.VertexShader.Set(instance.CompiledVertexShader);

            context.VertexShader.SetConstantBuffer(0, constantBuffer);

            context.UpdateSubresource(ref constants, constantBuffer);

            context.PixelShader.Set(instance.CompiledPixelShader);

            if (RenderContext11.ExternalProjection)
            {
                context.GeometryShader.SetConstantBuffer(0, constantBuffer);

                context.GeometryShader.SetShader(instance.CompiledGeometryShader, null, 0);
                RenderContext11.UpdateProjectionConstantBuffers();
            }
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                " struct VS_OUT                                \n" +
                " {                                            \n" +
                "     float4 ProjPos  : SV_POSITION;           \n" + // Projected space position 
                "     float4 Color    : COLOR;                 \n" +
                " };                                           \n" +
                "                                              \n" +
                " float4 PS(VS_OUT In) : SV_Target             \n" +
                " {                                            \n" +
                "     clip(In.Color.a == 0 ? -1 : 1);          \n" +
                "     return In.Color;                         \n" +
                " }                                            ";
        }

        static SharpDX.Direct3D11.Buffer constantBuffer;

        protected override string GetVertexShaderSource(string profile)
        {
            string source =
                    " float4x4 matWVP;                          \n" +
                    " float4 camPos : POSITION;                 \n" +
                    " float4 color : COLOR;                     \n" +
                    " float1 sky;                               \n" +
                    " float1 showFarSide;                       \n" +
                    " struct VS_IN                              \n" +
                    " {                                         \n" +
                    "     float4 ObjPos   : POSITION;           \n "; // Object space position 
            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint   instId   : SV_InstanceID;         \n";
            }
            source +=
                    " };                                           \n" +
                    "                                              \n" +
                    " struct VS_OUT                                \n" +
                    " {                                            \n" +
                    "     float4 ProjPos  : SV_POSITION;              \n" + // Projected space position 
                    "     float4 Color    : COLOR;                 \n";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint        viewId  : TEXCOORD0;          \n" +
                     " };                                            \n" +
                     " cbuffer ViewProjectionConstantBuffer : register(b1) \n" +
                     " {                                             \n" +
                     "      float4x4 viewProjection[2];              \n";
            }

            source +=
                    " };                                           \n" +
                    "                                              \n" +
                    " VS_OUT VS( VS_IN In )                         \n" +
                    " {                                            \n" +
                    "     VS_OUT Out;                              \n" +
                    "     float dotCam = dot((camPos.xyz - In.ObjPos.xyz), In.ObjPos.xyz);   \n";
                   
            if (RenderContext11.ExternalProjection)
            {
                source +=
                    "     int idx = In.instId % 2;             \n" +
                    "     Out.viewId = idx;                     \n" +
                    "     float4 p = mul(In.ObjPos,  matWVP );  \n" +
                    "     Out.ProjPos = mul(p, viewProjection[idx]); \n";
            }
            else
            {
                source +=
                    "     Out.ProjPos = mul(In.ObjPos,  matWVP );  \n";  
            }

            source +=
                             
                    "     if (showFarSide == 0 && (dotCam * sky) < 0 )  \n" +
                    "     {                                             \n" +
                    "        Out.Color = 0;                             \n" +
                    "     }                                             \n" +
                    "     else                                          \n" +
                    "     {                                             \n" +
                    "        Out.Color = color;                         \n" +
                    "     }                                             \n" +
                    "     return Out;                                   \n" + // Transfer color
                    " }                                                 \n";

            return source;
        }

        protected override string GetGeometryShaderSource(string profile)
        {
            return

            "    struct GeometryShaderInput                                                                                 \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n     " +
            "        float4 color   : COLOR0;                                                                               \n    " +
            "        uint instId  : TEXCOORD0;                                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // Per-vertex data passed to the rasterizer.                                                               \n " +
            "    struct GeometryShaderOutput                                                                                \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n  " +
            "        float4 color   : COLOR0;                                                                               \n   " +
            "        uint rtvId   : SV_RenderTargetArrayIndex;                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // This geometry shader is a pass-through that leaves the geometry unmodified                              \n " +
            "    // and sets the render target array index.                                                                 \n " +
            "    [maxvertexcount(2)]                                                                                        \n " +
            "    void GS(line GeometryShaderInput input[2], inout LineStream<GeometryShaderOutput> outStream)          \n " +
            "    {                                                                                                          \n " +
            "        GeometryShaderOutput output;                                                                           \n " +
            "        [unroll(2)]                                                                                            \n " +
            "        for (int i = 0; i< 2; ++i)                                                                             \n " +
            "        {                                                                                                      \n " +
            "            output.pos = input[i].pos;                                                                         \n " +
            "            output.color = input[i].color;                                                                     \n " +
            "            output.rtvId = input[i].instId;                                                                    \n " +
            "            outStream.Append(output);                                                                          \n " +
            "        }                                                                                                      \n " +
            "   }                                                                                                           \n" +
            "                                                                                                               \n ";
        }

        static void initialize()
        {
            instance = new SimpleLineShader11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<LineShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }

    public class SimpleGeometryShader11 : ShaderBundle
    {
        private static SimpleGeometryShader11 instance;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static SysColor Color
        {
            set
            {
                constants.Color = new SharpDX.Color4(value.R / 255.0f, value.G / 255.0f, value.B / 255.0f, value.A / 255.0f); ;
            }
        }

        public static Matrix WVPMatrix
        {
            set
            {
                constants.WorldViewProjection = value;
            }
        }

       

        private static GeometryShaderConstants constants;

        private static InputLayout layout;

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {     
                        new SharpDX.Direct3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                        new SharpDX.Direct3D11.InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 16, 0),
                        new SharpDX.Direct3D11.InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 20, 0),
                    });

            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(instance.CompiledVertexShader);

            context.VertexShader.SetConstantBuffer(0, constantBuffer);

            context.UpdateSubresource(ref constants, constantBuffer);

            context.PixelShader.Set(instance.CompiledPixelShader);

            if (RenderContext11.ExternalProjection)
            {
                context.GeometryShader.SetConstantBuffer(0, constantBuffer);

                context.GeometryShader.SetShader(instance.CompiledGeometryShader, null, 0);
                RenderContext11.UpdateProjectionConstantBuffers();
            }
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                " struct VS_OUT                                \n" +
                " {                                            \n" +
                "     float4 ProjPos  : SV_POSITION;           \n" + // Projected space position 
                "     float4 Color    : COLOR;                 \n" +
                "	  float2 tex      : TEXCOORD;              \n" +
                " };                                           \n" +
                "                                              \n" +
                "    \n" +
                " Texture2D picture;     \n" +
                " SamplerState pictureSampler;   \n" +
                " float4 PS(VS_OUT In) : SV_Target             \n" +
                " {                                            \n" +
                " 	return picture.Sample(pictureSampler, In.tex) * In.Color;   \n" +
                " }                                            ";
        }

        static SharpDX.Direct3D11.Buffer constantBuffer;

        protected override string GetVertexShaderSource(string profile)
        {
            string source =
                    " float4x4 matWVP;                                          \n" +
                    " float4 color : COLOR;                                     \n" +
                    " struct VS_IN                                              \n" +
                    " {                                                         \n" +
                    "     float4 ObjPos   : POSITION;                           \n" + // Object space position 
                    "     float4 Color    : COLOR;                              \n" +
                    "	  float2 tex      : TEXCOORD;                           \n";
            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint   instId   : SV_InstanceID;         \n";
            }
            source +=
                    " };                                                        \n" +
                    "                                                           \n" +
                    " struct VS_OUT                                             \n" +
                    " {                                                         \n" +
                    "     float4 ProjPos  : SV_POSITION;                        \n   " + // Projected space position 
                    "     float4 Color    : COLOR;                              \n" +
                    "	  float2 tex      : TEXCOORD;                           \n";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint        viewId  : TEXCOORD1;          \n" +
                     " };                                            \n" +
                     " cbuffer ViewProjectionConstantBuffer : register(b1) \n" +
                     " {                                             \n" +
                     "      float4x4 viewProjection[2];              \n";
            }

            source +=
                    " };                                                        \n" +
                    "                                                           \n" +
                    " VS_OUT VS( VS_IN In )                                     \n" +
                    " {                                                         \n" +
                    "     VS_OUT Out;                                           \n";
            if (RenderContext11.ExternalProjection)
            {
                source +=
                    "     int idx = In.instId % 2;             \n" +
                    "     Out.viewId = idx;                     \n" +
                    "     float4 p = mul(In.ObjPos,  matWVP );  \n" +
                    "     Out.ProjPos = mul(p, viewProjection[idx]); \n";
            }
            else
            {
                source +=
                    "     Out.ProjPos = mul(In.ObjPos,  matWVP );  \n";
            }

            source +=
                    "     Out.tex = In.tex;                                     \n" +
                    "     Out.Color = color * In.Color;                         \n" +
                    "     return Out;                                           \n" + // Transfer color
                    " }                                                         \n";

            return source;
        }

        protected override string GetGeometryShaderSource(string profile)
        {
            return

            "    struct GeometryShaderInput                                                                                 \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n     " +
            "        float4 color   : COLOR0;                                                                               \n    " +
            "	     float2 tex     : TEXCOORD;                           \n" +
            "        uint instId    : TEXCOORD1;                                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // Per-vertex data passed to the rasterizer.                                                               \n " +
            "    struct GeometryShaderOutput                                                                                \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n  " +
            "        float4 color   : COLOR0;                                                                               \n   " +
            "	     float2 tex     : TEXCOORD;                           \n" +
            "        uint rtvId   : SV_RenderTargetArrayIndex;                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // This geometry shader is a pass-through that leaves the geometry unmodified                              \n " +
            "    // and sets the render target array index.                                                                 \n " +
            "    [maxvertexcount(3)]                                                                                        \n " +
            "    void GS(triangle GeometryShaderInput input[3], inout TriangleStream<GeometryShaderOutput> outStream)          \n " +
            "    {                                                                                                          \n " +
            "        GeometryShaderOutput output;                                                                           \n " +
            "        [unroll(3)]                                                                                            \n " +
            "        for (int i = 0; i< 3; ++i)                                                                             \n " +
            "        {                                                                                                      \n " +
            "            output.pos = input[i].pos;                                                                         \n " +
            "            output.color = input[i].color;                                                                     \n " +
            "            output.tex = input[i].tex;                                                                     \n " +
            "            output.rtvId = input[i].instId;                                                                    \n " +
            "            outStream.Append(output);                                                                          \n " +
            "        }                                                                                                      \n " +
            "   }                                                                                                           \n" +
            "                                                                                                               \n ";
        }

        static void initialize()
        {
            instance = new SimpleGeometryShader11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<LineShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }



    [StructLayout(LayoutKind.Explicit, Size = 80)]
    public struct GeometryShaderConstants
    {
        [FieldOffset(0)]
        public Matrix WorldViewProjection;
        [FieldOffset(64)]
        public Color4 Color;
      
    }

    [StructLayout(LayoutKind.Explicit, Size = 112)]
    public struct LineShaderConstants
    {
        [FieldOffset(0)]
        public Matrix WorldViewProjection;
        [FieldOffset(64)]
        public Vector4 CameraPosition;
        [FieldOffset(80)]
        public Color4 Color;
        [FieldOffset(96)]
        public float Sky;
        [FieldOffset(100)]
        public float ShowFarSide;
    }

    public class LineShaderNormalDates11 : ShaderBundle
    {
        private static LineShaderNormalDates11 instance;
        public static LineShaderNormalDatesConstants Constants;
        private static InputLayout layout;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }


        public static Matrix WVPMatrix
        {
            set
            {
                Constants.WorldViewProjection = value;
            }
        }

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32_Float, 12, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 24, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 28, 0),
                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);

            context.VertexShader.SetConstantBuffer(0, contantBuffer);

            context.UpdateSubresource(ref Constants, contantBuffer);

            context.PixelShader.Set(PixelShader);

            if (RenderContext11.ExternalProjection)
            {
                context.GeometryShader.SetConstantBuffer(0, contantBuffer);

                context.GeometryShader.SetShader(instance.CompiledGeometryShader, null, 0);
                RenderContext11.UpdateProjectionConstantBuffers();
            }
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                " struct VS_OUT                                \n" +
                " {                                            \n" +
                "     float4 ProjPos  : SV_POSITION;              \n" + // Projected space position 
                "     float4 Color    : COLOR;                 \n" +
                " };                                           \n" +
                "                                              \n" +
                " float4 PS(VS_OUT In) : SV_Target               \n" +
                " {                                            \n" +
                "     return In.Color;                         \n" +
                " }                                            ";
        }

        static SharpDX.Direct3D11.Buffer contantBuffer;

        protected override string GetVertexShaderSource(string profile)
        {
            string source =
                  " float4x4 matWVP;              " +
                  " float4 camPos : POSITION;  ;                               " +
                  " float1 opacity;              " +
                  " float1 jNow;                               " +
                  " float1 decay;                               " +
                  " float1 sky;                         " +
                  " float1 showFarSide;                         " +
                  " struct VS_IN                                 " +
                  " {                                            " +
                  "     float4 ObjPos   : POSITION;              " + // Object space position 
                  "     float4 ObjNor   : NORMAL;               " + // Object space position 
                  "     float4 Color    : COLOR;                 " + // Vertex color       
                  "     float2 Time   : TEXCOORD0;              "; // Object Point size 
            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint   instId   : SV_InstanceID;         \n";
            }
            source +=
                  " };                                           " +
                  "                                              " +
                  " struct VS_OUT                                " +
                  " {                                            " +
                  "     float4 ProjPos  : SV_POSITION;              " + // Projected space position 
                  "     float4 Color    : COLOR;                 ";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint        viewId  : TEXCOORD0;          \n" +
                     " };                                            \n" +
                     " cbuffer ViewProjectionConstantBuffer : register(b1) \n" +
                     " {                                             \n" +
                     "      float4x4 viewProjection[2];              \n";
            }

            source +=
                  " };                                           " +
                  "                                              " +
                  " VS_OUT VS( VS_IN In )                      " +
                  " {                                            " +
                  "     float dotCam = dot((camPos.xyz - In.ObjPos.xyz), In.ObjNor.xyz);   " +
                  "     VS_OUT Out;                              " +
                  "     float dAlpha = 1;                         " +
                  "     if ( decay > 0)                           " +
                  "     {                                        " +
                  "          dAlpha = 1 - ((jNow - In.Time.y) / decay);          " +
                  "          if (dAlpha > 1 )           " +
                  "          {                                     " +
                  "               dAlpha = 1;                     " +
                  "          }                                    " +
                  "                                               " +
                  "     }                                        ";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                    "     int idx = In.instId % 2;             \n" +
                    "     Out.viewId = idx;                     \n" +
                    "     float4 p = mul(In.ObjPos,  matWVP );  \n" +
                    "     Out.ProjPos = mul(p, viewProjection[idx]); \n";
            }
            else
            {
                source +=
                    "     Out.ProjPos = mul(In.ObjPos,  matWVP );  \n";
            }

            source +=
                  "     if (showFarSide == 0 && (dotCam * sky) < 0 || (jNow < In.Time.x && decay > 0))   " +
                  "     {                                        " +
                  "        Out.Color.a = 0;                     " +
                  "     }                                        " +
                  "     else                                     " +
                  "     {                                        " +
                  "        Out.Color.a = In.Color.a * dAlpha * opacity;    " +
                  "     }                                        " +
                  "     Out.Color.r = In.Color.r;              " +
                  "     Out.Color.g = In.Color.g;              " +
                  "     Out.Color.b = In.Color.b;              " +
                  "     return Out;                              " + // Transfer color
                  " }                                            ";

            return source;
        }

        protected override string GetGeometryShaderSource(string profile)
        {
            return

            "    struct GeometryShaderInput                                                                                 \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n     " +
            "        float4 color   : COLOR0;                                                                               \n    " +
            "        uint instId  : TEXCOORD0;                                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // Per-vertex data passed to the rasterizer.                                                               \n " +
            "    struct GeometryShaderOutput                                                                                \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n  " +
            "        float4 color   : COLOR0;                                                                               \n   " +
            "        uint rtvId   : SV_RenderTargetArrayIndex;                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // This geometry shader is a pass-through that leaves the geometry unmodified                              \n " +
            "    // and sets the render target array index.                                                                 \n " +
            "    [maxvertexcount(2)]                                                                                        \n " +
            "    void GS(line GeometryShaderInput input[2], inout LineStream<GeometryShaderOutput> outStream)          \n " +
            "    {                                                                                                          \n " +
            "        GeometryShaderOutput output;                                                                           \n " +
            "        [unroll(2)]                                                                                            \n " +
            "        for (int i = 0; i< 2; ++i)                                                                             \n " +
            "        {                                                                                                      \n " +
            "            output.pos = input[i].pos;                                                                         \n " +
            "            output.color = input[i].color;                                                                     \n " +
            "            output.rtvId = input[i].instId;                                                                    \n " +
            "            outStream.Append(output);                                                                          \n " +
            "        }                                                                                                      \n " +
            "   }                                                                                                           \n" +
            "                                                                                                               \n ";
        }

        static void initialize()
        {
            instance = new LineShaderNormalDates11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            contantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<LineShaderNormalDatesConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }

    public class TriangleShaderNormalDates11 : ShaderBundle
    {
        private static TriangleShaderNormalDates11 instance;
        public static LineShaderNormalDatesConstants Constants;
        private static InputLayout layout;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }


        public static Matrix WVPMatrix
        {
            set
            {
                Constants.WorldViewProjection = value;
            }
        }

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32_Float, 12, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 24, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 28, 0),
                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);

            context.VertexShader.SetConstantBuffer(0, contantBuffer);

            context.UpdateSubresource(ref Constants, contantBuffer);

            context.PixelShader.Set(PixelShader);

            if (RenderContext11.ExternalProjection)
            {
                context.GeometryShader.SetConstantBuffer(0, contantBuffer);

                context.GeometryShader.SetShader(instance.CompiledGeometryShader, null, 0);
                RenderContext11.UpdateProjectionConstantBuffers();
            }
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                " struct VS_OUT                                \n" +
                " {                                            \n" +
                "     float4 ProjPos  : SV_POSITION;              \n" + // Projected space position 
                "     float4 Color    : COLOR;                 \n" +
                " };                                           \n" +
                "                                              \n" +
                " float4 PS(VS_OUT In) : SV_Target               \n" +
                " {                                            \n" +
                "     return In.Color;                         \n" +
                " }                                            ";
        }

        static SharpDX.Direct3D11.Buffer contantBuffer;

        protected override string GetVertexShaderSource(string profile)
        {
            string source =
                  " float4x4 matWVP;              " +
                  " float4 camPos : POSITION;  ;                               " +
                  " float1 opacity;              " +
                  " float1 jNow;                               " +
                  " float1 decay;                               " +
                  " float1 sky;                         " +
                  " float1 showFarSide;                         " +
                  " struct VS_IN                                 " +
                  " {                                            " +
                  "     float4 ObjPos   : POSITION;              " + // Object space position 
                  "     float4 ObjNor   : NORMAL;               " + // Object space position 
                  "     float4 Color    : COLOR;                 " + // Vertex color       
                  "     float2 Time   : TEXCOORD0;              "; // Object Point size 
            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint   instId   : SV_InstanceID;         \n";
            }
            source +=
                  " };                                           " +
                  "                                              " +
                  " struct VS_OUT                                " +
                  " {                                            " +
                  "     float4 ProjPos  : SV_POSITION;              " + // Projected space position 
                  "     float4 Color    : COLOR;                 ";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint        viewId  : TEXCOORD0;          \n" +
                     " };                                            \n" +
                     " cbuffer ViewProjectionConstantBuffer : register(b1) \n" +
                     " {                                             \n" +
                     "      float4x4 viewProjection[2];              \n";
            }

            source +=
                  " };                                           " +
                  "                                              " +
                  " VS_OUT VS( VS_IN In )                      " +
                  " {                                            " +
                  "     float dotCam = dot((camPos.xyz - In.ObjPos.xyz), In.ObjNor.xyz);   " +
                  "     VS_OUT Out;                              " +
                  "     float dAlpha = 1;                         " +
                  "     if ( decay > 0)                           " +
                  "     {                                        " +
                  "          dAlpha = 1 - ((jNow - In.Time.y) / decay);          " +
                  "          if (dAlpha > 1 )           " +
                  "          {                                     " +
                  "               dAlpha = 1;                     " +
                  "          }                                    " +
                  "                                               " +
                  "     }                                        ";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                    "     int idx = In.instId % 2;             \n" +
                    "     Out.viewId = idx;                     \n" +
                    "     float4 p = mul(In.ObjPos,  matWVP );  \n" +
                    "     Out.ProjPos = mul(p, viewProjection[idx]); \n";
            }
            else
            {
                source +=
                    "     Out.ProjPos = mul(In.ObjPos,  matWVP );  \n";
            }

            source +=
                  "     if (showFarSide == 0 && (dotCam * sky) < 0 || (jNow < In.Time.x && decay > 0))   " +
                  "     {                                        " +
                  "        Out.Color.a = 0;                     " +
                  "     }                                        " +
                  "     else                                     " +
                  "     {                                        " +
                  "        Out.Color.a = In.Color.a * dAlpha * opacity;    " +
                  "     }                                        " +
                  "     Out.Color.r = In.Color.r;              " +
                  "     Out.Color.g = In.Color.g;              " +
                  "     Out.Color.b = In.Color.b;              " +
                  "     return Out;                              " + // Transfer color
                  " }                                            ";

            return source;
        }

        protected override string GetGeometryShaderSource(string profile)
        {
            return

            "    struct GeometryShaderInput                                                                                 \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n     " +
            "        float4 color   : COLOR0;                                                                               \n    " +
            "        uint instId  : TEXCOORD0;                                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // Per-vertex data passed to the rasterizer.                                                               \n " +
            "    struct GeometryShaderOutput                                                                                \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n  " +
            "        float4 color   : COLOR0;                                                                               \n   " +
            "        uint rtvId   : SV_RenderTargetArrayIndex;                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // This geometry shader is a pass-through that leaves the geometry unmodified                              \n " +
            "    // and sets the render target array index.                                                                 \n " +
            "    [maxvertexcount(3)]                                                                                        \n " +
            "    void GS(triangle GeometryShaderInput input[3], inout TriangleStream<GeometryShaderOutput> outStream)          \n " +
            "    {                                                                                                          \n " +
            "        GeometryShaderOutput output;                                                                           \n " +
            "        [unroll(3)]                                                                                            \n " +
            "        for (int i = 0; i< 3; ++i)                                                                             \n " +
            "        {                                                                                                      \n " +
            "            output.pos = input[i].pos;                                                                         \n " +
            "            output.color = input[i].color;                                                                     \n " +
            "            output.rtvId = input[i].instId;                                                                    \n " +
            "            outStream.Append(output);                                                                          \n " +
            "        }                                                                                                      \n " +
            "   }                                                                                                           \n" +
            "                                                                                                               \n ";
        }

        static void initialize()
        {
            instance = new TriangleShaderNormalDates11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            contantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<LineShaderNormalDatesConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }
    }



    [StructLayout(LayoutKind.Explicit, Size = 112)]
    public struct LineShaderNormalDatesConstants
    {
        [FieldOffset(0)]
        public Matrix WorldViewProjection;
        [FieldOffset(64)]
        public Vector4 CameraPosition;
        [FieldOffset(80)]
        public float Opacity;
        [FieldOffset(84)]
        public float JNow;
        [FieldOffset(88)]
        public float Decay;
        [FieldOffset(92)]
        public float Sky;
        [FieldOffset(96)]
        public float ShowFarSide;
    }

    public class PointShaderDates11 : ShaderBundle
    {
        private static PointShaderDates11 instance;
        public static PointShaderDatesConstants Constants;
        private static InputLayout layout;
        static SharpDX.Direct3D11.Buffer contantBuffer;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }
        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }


        public static Matrix WVPMatrix
        {
            set
            {
                Constants.WorldViewProjection = value;
            }
        }


        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float, 12, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 20, 0),

                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);

            context.VertexShader.SetConstantBuffer(0, contantBuffer);

            context.UpdateSubresource(ref Constants, contantBuffer);

            context.PixelShader.Set(PixelShader);
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                " struct VS_OUT                                \n" +
                " {                                            \n" +
                "     float4 ProjPos  : SV_POSITION;              \n" + // Projected space position 
                "     float4 Color    : COLOR;                 \n" +
                " };                                           \n" +
                "                                              \n" +
                " float4 PS(VS_OUT In) : SV_Target               \n" +
                " {                                            \n" +
                "     return In.Color;                         \n" +
                " }                                            ";
        }

        protected override string GetVertexShaderSource(string profile)
        {
            return
                   " float4x4 matWVP;              " +
                    " float4 camPos : POSITION;                               " +
                    " float1 opacity;                               " +
                    " float1 jNow;                               " +
                    " float1 decay;                               " +
                    " float1 sky;                         " +
                    " float1 showFarSide;                         " +
                    " float1 scale;                               " +
                    " float2 viewportScale;                               " +
                   " struct VS_IN                                 " +
                    " {                                            " +
                    "     float4 ObjPos   : POSITION;              " + // Object space position 
                    "     float1 PointSize   : PSIZE;              " + // Object Point size 
                    "     float4 Color    : COLOR;                 " + // Vertex color                 
                    "     float2 Time   : TEXCOORD0;              " + // Object Point size 
                    " };                                           " +
                    "                                              " +
                    " struct VS_OUT                                " +
                    " {                                            " +
                    "     float4 ProjPos  : SV_POSITION;              " + // Projected space position 
                    "     float4 Color    : COLOR;                 " +
                    "     float1 PointSize   : PSIZE;              " + // Object Point size 
                    " };                                           " +
                    "                                              " +
                    " VS_OUT VS( VS_IN In )                      " +
                    " {                                            " +
                    "     float dotCam = dot((camPos.xyz - In.ObjPos.xyz), In.ObjPos.xyz);   " +
                    "     float dist = distance(In.ObjPos, camPos.xyz);   " +
                    "     VS_OUT Out;                              " +
                    "     float dAlpha = 1;                         " +
                    "     if ( decay > 0)                           " +
                    "     {                                        " +
                    "          dAlpha = 1 - ((jNow - In.Time.y) / decay);          " +
                    "          if (dAlpha > 1 )           " +
                    "          {                                     " +
                    "               dAlpha = 1;                     " +
                    "          }                                    " +
                    "                                               " +
                    "     }                                        " +
                    "     Out.ProjPos = mul(In.ObjPos,  matWVP );  " + // Transform vertex into
                    "     if (showFarSide == 0 && (dotCam * sky) < 0 || (jNow < In.Time.x && decay > 0))   " +
                    "     {                                        " +
                    "        Out.Color.a = 0;                     " +
                    "     }                                        " +
                    "     else                                     " +
                    "     {                                        " +
                    "        Out.Color.a = In.Color.a * dAlpha * opacity;    " +
                    "     }                                                                  " +
                    "     Out.Color.r = In.Color.r;                                          " +
                    "     Out.Color.g = In.Color.g;                                          " +
                    "     Out.Color.b = In.Color.b;                                          " +
                    "     if ( scale > 0)                                                   " +
                    "     {                                                                 " +
                    "       Out.PointSize = viewportScale * (scale * In.PointSize / dist);  " +  //todo11 need max here?
                    "     }                                                                 " +
                    "     else                                                              " +
                    "     {                                                                 " +
                    "       Out.PointSize =  viewportScale * (-scale * In.PointSize);" +
                    "     }                                        " +
                     " if (Out.PointSize > 256)                     " +
                     " {                                            " +
                     "      Out.PointSize = 256;                                        " +
                     " }                                            " +
                     "     return Out;                              " + // Transfer color
                     " }                                            ";                                        
        }

        static void initialize()
        {
            instance = new PointShaderDates11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            contantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<PointShaderDatesConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }
    }



    [StructLayout(LayoutKind.Explicit, Size = 112)]
    public struct PointShaderDatesConstants
    {
        [FieldOffset(0)]
        public Matrix WorldViewProjection;
        [FieldOffset(64)]
        public Vector4 CameraPosition;
        [FieldOffset(80)]
        public float Opacity;
        [FieldOffset(84)]
        public float JNow;
        [FieldOffset(88)]
        public float Decay;
        [FieldOffset(92)]
        public float Sky;
        [FieldOffset(96)]
        public float ShowFarSide;
        [FieldOffset(100)]
        public float Scale;
        [FieldOffset(104)]
        public Vector2 ViewportScale;
    }


    public class AnaglyphStereoShader : ShaderBundle
    {
        private static AnaglyphStereoShader instance;
        private static AnaglyphStereoShaderConstants constants;
        private static InputLayout layout;
        static SharpDX.Direct3D11.Buffer contantBuffer;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static Color4 Color
        {
            set
            {
                constants.Color = value;
            }
        }

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 16, 0),
                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);

            context.VertexShader.SetConstantBuffer(0, contantBuffer);

            context.UpdateSubresource(ref constants, contantBuffer);

            context.PixelShader.Set(PixelShader);
        }

        protected override string  GetPixelShaderSource(string profile)
        {
            return
                "struct PS_IN                                \n" +
                    "{                                          \n" +
                    "	float4 pos : SV_POSITION;               \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "	float4 color : COLOR0;               \n" +
                    "};                                             \n" +
                "    \n" +
                " Texture2D picture;     \n" +
                " SamplerState pictureSampler;   \n" +
                "    \n" +
                "    \n" +
                "    \n" +
                " float4 PS( PS_IN input ) : SV_Target   \n" +
                " {     \n" +
                " 	return picture.Sample(pictureSampler, input.tex) * input.color;   \n" +
             
                " }   \n" +  
                "    ";
        }


        protected override string  GetVertexShaderSource(string profile)
        {

            return
                    "struct VS_IN                               \n" +
                    "{                                          \n" +
                    "	float4 pos : POSITION;                  \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "};                                         \n" +
                    "                                           \n" +
                    "struct PS_IN                                \n" +
                    "{                                          \n" +
                    "	float4 pos : SV_POSITION;               \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                   "	float4 color : COLOR0;               \n" +
                    "};                                             \n" +
                    "                                           \n" +
                    "float4 color;                    \n" +
                    "PS_IN VS( VS_IN input )                    \n" +
                    "{                                          \n " +
                    "	PS_IN output = (PS_IN)0;                \n" +
                    "	  " +
                    "	output.pos = input.pos;    \n" +
                    "	output.tex = input.tex;                     \n" +
                    "	output.color = color;                                        \n" +
                    "	return output;                          \n" +
                    "}                                          \n" +
                    "                                           \n" +
                    "                                           \n";
        }

        static void initialize()
        {
            instance = new AnaglyphStereoShader();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            contantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<AnaglyphStereoShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct AnaglyphStereoShaderConstants
    {
        [FieldOffset(0)]
        public Color4 Color;
    }

    public class InterlineStereoShader : ShaderBundle
    {
        private static InterlineStereoShader instance;
        private static InterlineStereoShaderConstants constants;
        private static InputLayout layout;
        static SharpDX.Direct3D11.Buffer contantBuffer;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static float Lines
        {
            set
            {
                constants.Lines = value;
            }
        }

        public static float Odd
        {
            set
            {
                constants.Odd = value;
            }
        }

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 16, 0),
                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);

            context.PixelShader.Set(PixelShader);
          
            context.PixelShader.SetConstantBuffer(0, contantBuffer);

            context.UpdateSubresource(ref constants, contantBuffer);
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                "struct PS_IN                                \n" +
                "{                                          \n" +
                "	float4 pos : SV_POSITION;               \n" +
                "	float2 tex : TEXCOORD;                  \n" +
                "};                                             \n" +
                "    \n" +
                "   float lines;                                                                                                                                \n" +
                "   float odd;                                                                                                                                \n" +
                " Texture2D rightImg;     \n" +
                " SamplerState rightSamp;   \n" +
                " Texture2D leftImg;     \n" +
                "    \n" +
                "    \n" +
                "    \n" +
                " float4 PS( PS_IN input ) : SV_Target   \n" +
                " {     \n" +
                "      // Add small offset to prevent unpredictable behavior when sample points exactly align with pixels\n" +
                "      float lineNumber = floor(input.tex.y * lines + 0.1) + odd;             \n" +
                "      if (lineNumber / 2 - floor(lineNumber / 2) < 0.5)                      \n" +
                "      {                                                                      \n" +
                " 	        return rightImg.Sample(rightSamp, input.tex);                     \n" +
                "      }                                                                      \n" +
                "      else                                                                   \n" +
                "      {                                                                      \n" +
                " 	        return leftImg.Sample(rightSamp, input.tex);                \n" +
                "      }                                                                      \n" +
                " }   \n" +
                "    ";
        }

        protected override string GetVertexShaderSource(string profile)
        {
            return
                    "struct VS_IN                               \n" +
                    "{                                          \n" +
                    "	float4 pos : POSITION;                  \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "};                                         \n" +
                    "                                           \n" +
                    "struct PS_IN                                \n" +
                    "{                                          \n" +
                    "	float4 pos : SV_POSITION;               \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "};                                             \n" +
                    "                                           \n" +
                    "float4 color;                    \n" +
                    "PS_IN VS( VS_IN input )                    \n" +
                    "{                                          \n " +
                    "	PS_IN output = (PS_IN)0;                \n" +
                    "	  " +
                    "	output.pos = input.pos;    \n" +
                    "	output.tex = input.tex;                     \n" +
                    "	return output;                          \n" +
                    "}                                          \n" +
                    "                                           \n" +
                    "                                           \n";
        }

        static void initialize()
        {
            instance = new InterlineStereoShader();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            contantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<InterlineStereoShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }


    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct InterlineStereoShaderConstants
    {
        [FieldOffset(0)]
        public float Lines;
        [FieldOffset(4)]
        public float Odd;
    }

    public class RiftStereoShader : ShaderBundle
    {
        private static RiftStereoShader instance;
        public static RiftStereoShaderConstants constants;
        private static InputLayout layout;
        static SharpDX.Direct3D11.Buffer contantBuffer;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 16, 0),
                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);

            context.PixelShader.Set(PixelShader);

            context.PixelShader.SetConstantBuffer(0, contantBuffer);

            context.UpdateSubresource(ref constants, contantBuffer);
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
              "  struct PS_IN                                                                    \n" +
              "  {                                                                               \n" +
              "  	float4 pos : SV_POSITION;                                                    \n" +
              "  	float2 tex : TEXCOORD;                                                       \n" +
              "  };                                                                              \n" +
              "  Texture2D leftImg;                                                              \n" +
              "  Texture2D rightImg;                                                             \n" +
              "  SamplerState rightSamp;                                                         \n" +
              "                                                                                  \n" +                                                                                  
              "  float2 LensCenterLeft;                                                          \n" +
              "  float2 LensCenterRight;                                                         \n" +
              "  float2 ScreenCenterLeft;                                                        \n" +
              "  float2 ScreenCenterRight;                                                       \n" +
              "  float2 Scale;                                                                   \n" +
              "  float2 ScaleIn;                                                                 \n" +
              "  float4 HmdWarpParam;                                                            \n" +
              "  // Scales input texture coordinates for distortion.                             \n" +
              "  float2 HmdWarp(float2 in01, float2 LensCenter, float colorScale)                \n" +
              "  {                                                                               \n" +
              "     float2 theta = (in01 - LensCenter) * ScaleIn; // Scales to [-1, 1]           \n" +
              "     float rSq = theta.x * theta.x + theta.y * theta.y;                           \n" +
              "     float2 rvector= theta * (HmdWarpParam.x + HmdWarpParam.y * rSq +             \n" +
              "     HmdWarpParam.z * rSq * rSq +                                                 \n" +
              "     HmdWarpParam.w * rSq * rSq * rSq);                                           \n" +
              "     return LensCenter + Scale * rvector * colorScale;                            \n" +
              "  }                                                                               \n" +
              "                                                                                  \n" +                                                                                  
              "  float4 PS( PS_IN input ) : SV_Target                                            \n" +
              "  {                                                                               \n" +
              "         float2 tc;                                                               \n" +
              "         float red;                                                               \n" +
              "         float green;                                                             \n" +
              "         float blue;                                                              \n" +
              "      if (input.tex.x < .5)                                                       \n" +
              "      {                                                                           \n" +
              "           tc = HmdWarp(float2(input.tex.x*2,input.tex.y),LensCenterLeft, 1);     \n" +
              "           if (tc.x < 0 || tc.x > 1 || tc.y < 0 || tc.y > 1)                      \n" +
              "           {                                                                      \n" +
              "               return 0;                                                          \n" +
              "           }                                                                      \n" +
              "                                                                                  \n " +
              " 	      green =  leftImg.Sample(rightSamp, tc).g;                              \n" +
              "                                                                                  \n " +
              "           tc = HmdWarp(float2(input.tex.x*2,input.tex.y),LensCenterLeft, 1.01);   \n" +
              "           if (tc.x < 0 || tc.x > 1 || tc.y < 0 || tc.y > 1)                      \n" +
              "           {                                                                      \n" +
              "               return 0;                                                          \n" +
              "           }                                                                      \n" +
              "                                                                                  \n " +
              " 	      blue =  leftImg.Sample(rightSamp, tc).b;                               \n" +
              "                                                                                  \n " +
              "           tc = HmdWarp(float2(input.tex.x*2,input.tex.y),LensCenterLeft, .995);  \n" +
              "           if (tc.x < 0 || tc.x > 1 || tc.y < 0 || tc.y > 1)                      \n" +
              "           {                                                                      \n" +
              "               return 0;                                                          \n" +
              "           }                                                                      \n" +
              "                                                                                  \n " +
              " 	      red =  leftImg.Sample(rightSamp, tc).r;                                \n" + 
              " 	      return float4(red,green,blue,1);                                   \n" + 
              "      }                                                                           \n" +
              "      else                                                                        \n" +
              "      {                                                                           \n" +
              "           tc = HmdWarp(float2((input.tex.x-.5)*2,input.tex.y), LensCenterRight,1); \n" +
              "           if (tc.x < 0 || tc.x > 1 || tc.y < 0 || tc.y > 1)                      \n" +
              "           {                                                                      \n" +
              "               return 0;                                                          \n" +
              "           }                                                                      \n" +
              "                                                                                  \n " +
              " 	      green =  rightImg.Sample(rightSamp, tc).g;                                \n" + 
               "                                                                                  \n " +
              "           tc = HmdWarp(float2((input.tex.x-.5)*2,input.tex.y), LensCenterRight,1.01); \n" +
              "           if (tc.x < 0 || tc.x > 1 || tc.y < 0 || tc.y > 1)                      \n" +
              "           {                                                                      \n" +
              "               return 0;                                                          \n" +
              "           }                                                                      \n" +
              "                                                                                  \n " +
              " 	      blue =  rightImg.Sample(rightSamp, tc).b;                                \n" + 
              "                                                                                  \n " +
              "           tc = HmdWarp(float2((input.tex.x-.5)*2,input.tex.y), LensCenterRight,.995); \n" +
              "           if (tc.x < 0 || tc.x > 1 || tc.y < 0 || tc.y > 1)                      \n" +
              "           {                                                                      \n" +
              "               return 0;                                                          \n" +
              "           }                                                                      \n" +
              "                                                                                  \n " +
              " 	      red =  rightImg.Sample(rightSamp, tc).r;                                \n" + 
              " 	      return float4(red,green,blue,1);                                   \n" + 
              "                                                                                  \n " +
              "      }                                                                           \n" +
              "  };                                                                              \n";
             
        }

        //protected override string GetPixelShaderSource(string profile)
        //{
        //    return
        //      "  struct PS_IN                                                                    \n" +
        //      "  {                                                                               \n" +
        //      "  	float4 pos : SV_POSITION;                                                    \n" +
        //      "  	float2 tex : TEXCOORD;                                                       \n" +
        //      "  };                                                                              \n" +
        //      "  Texture2D leftImg;                                                              \n" +
        //      "  Texture2D rightImg;                                                             \n" +
        //      "  SamplerState rightSamp;                                                         \n" +
        //      "                                                                                  \n" +
        //      "  float2 LensCenterLeft;                                                          \n" +
        //      "  float2 LensCenterRight;                                                         \n" +
        //      "  float2 ScreenCenterLeft;                                                        \n" +
        //      "  float2 ScreenCenterRight;                                                       \n" +
        //      "  float2 Scale;                                                                   \n" +
        //      "  float2 ScaleIn;                                                                 \n" +
        //      "  float4 HmdWarpParam;                                                            \n" +
        //      "  // Scales input texture coordinates for distortion.                             \n" +
        //      "  float2 HmdWarp(float2 in01, float2 LensCenter)                                  \n" +
        //      "  {                                                                               \n" +
        //      "     float2 theta = (in01 - LensCenter) * ScaleIn; // Scales to [-1, 1]           \n" +
        //      "     float rSq = theta.x * theta.x + theta.y * theta.y;                           \n" +
        //      "     float2 rvector= theta * (HmdWarpParam.x + HmdWarpParam.y * rSq +             \n" +
        //      "     HmdWarpParam.z * rSq * rSq +                                                 \n" +
        //      "     HmdWarpParam.w * rSq * rSq * rSq);                                           \n" +
        //      "     return LensCenter + Scale * rvector;                                         \n" +
        //      "  }                                                                               \n" +
        //      "                                                                                  \n" +
        //      "  float4 PS( PS_IN input ) : SV_Target                                            \n" +
        //      "  {                                                                               \n" +
        //      "         float2 tc;                                                               \n" +
        //      "      if (input.tex.x < .5)                                                       \n" +
        //      "      {                                                                           \n" +
        //      "           tc = HmdWarp(float2(input.tex.x*2,input.tex.y),LensCenterLeft);        \n" +
        //      "           if (tc.x < 0 || tc.x > 1 || tc.y < 0 || tc.y > 1)                      \n" +
        //      "           {                                                                      \n" +
        //      "               return 0;                                                          \n" +
        //      "           }                                                                      \n" +
        //      "                                                                                  \n " +
        //      " 	        return leftImg.Sample(rightSamp, tc);                                \n" +
        //      "      }                                                                           \n" +
        //      "      else                                                                        \n" +
        //      "      {                                                                           \n" +
        //      "           tc = HmdWarp(float2((input.tex.x-.5)*2,input.tex.y), LensCenterRight); \n" +
        //      "           if (tc.x < 0 || tc.x > 1 || tc.y < 0 || tc.y > 1)                      \n" +
        //      "           {                                                                      \n" +
        //      "               return 0;                                                          \n" +
        //      "           }                                                                      \n" +
        //      "                                                                                  \n " +
        //      " 	        return rightImg.Sample(rightSamp, tc);                               \n" +
        //      "      }                                                                           \n" +
        //      "  };                                                                              \n";

        //}

        protected override string GetVertexShaderSource(string profile)
        {
            return
                    "struct VS_IN                               \n" +
                    "{                                          \n" +
                    "	float4 pos : POSITION;                  \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "};                                         \n" +
                    "                                           \n" +
                    "struct PS_IN                               \n" +
                    "{                                          \n" +
                    "	float4 pos : SV_POSITION;               \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "};                                         \n" +
                    "                                           \n" +
                    "float4 color;                              \n" +
                    "PS_IN VS( VS_IN input )                    \n" +
                    "{                                          \n " +
                    "	PS_IN output = (PS_IN)0;                \n" +
                    "	                                        \n " +
                    "	output.pos = input.pos;                 \n" +
                    "	output.tex = input.tex;                 \n" +
                    "	return output;                          \n" +
                    "}                                          \n" +
                    "                                           \n" +
                    "                                           \n";
        }

        static void initialize()
        {
            instance = new RiftStereoShader();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            contantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<RiftStereoShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }


    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct RiftStereoShaderConstants
    {
        [FieldOffset(0)]
        public Vector2 LensCenterLeft;
        [FieldOffset(8)]
        public Vector2 LensCenterRight;
        [FieldOffset(16)]
        public Vector2 ScreenCenterLeft;
        [FieldOffset(24)]
        public Vector2 ScreenCenterRight;
        [FieldOffset(32)]
        public Vector2 Scale;
        [FieldOffset(40)]
        public Vector2 ScaleIn;
        [FieldOffset(48)]
        public Vector4 HmdWarpParam;
    }


    public class SideBySideStereoShader : ShaderBundle
    {
        private static SideBySideStereoShader instance;
        private static InputLayout layout;


        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }
        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 16, 0),
                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);

            context.PixelShader.Set(PixelShader);
        }

        protected override string  GetPixelShaderSource(string profile)
        {
            return
                "struct PS_IN                                \n" +
                "{                                          \n" +
                "	float4 pos : SV_POSITION;               \n" +
                "	float2 tex : TEXCOORD;                  \n" +
                "};                                             \n" +
                "    \n" +
                 " Texture2D rightImg;     \n" +
                " SamplerState rightSamp;   \n" +
                " Texture2D leftImg;     \n" +
                "    \n" +
                "    \n" +
                "    \n" +
                " float4 PS( PS_IN input ) : SV_Target   \n" +
                " {     \n" +
                "      if (input.tex.x < .5)                                                                                                                              \n" +
                "      {                                                                                                                                                        \n" +
                " 	        return rightImg.Sample(rightSamp, float2(input.tex.x*2,input.tex.y));   \n" +
                "      }                                                                                                                                                        \n" +
                "      else                                                                                                                                                     \n" +
                "      {                                                                                                                                                        \n" +
                " 	        return leftImg.Sample(rightSamp, float2((input.tex.x-.5)*2,input.tex.y));   \n" +
                "      }                                                                                                                                                        \n" +
                " }   \n" +
                "    ";
        }

        protected override string GetVertexShaderSource(string profile)
        {
            return
                    "struct VS_IN                               \n" +
                    "{                                          \n" +
                    "	float4 pos : POSITION;                  \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "};                                         \n" +
                    "                                           \n" +
                    "struct PS_IN                                \n" +
                    "{                                          \n" +
                    "	float4 pos : SV_POSITION;               \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "};                                             \n" +
                    "                                           \n" +
                    "float4 color;                    \n" +
                    "PS_IN VS( VS_IN input )                    \n" +
                    "{                                          \n " +
                    "	PS_IN output = (PS_IN)0;                \n" +
                    "	  " +
                    "	output.pos = input.pos;    \n" +
                    "	output.tex = input.tex;                     \n" +
                    "	return output;                          \n" +
                    "}                                          \n" +
                    "                                           \n" +
                    "                                           \n";
        }

        static void initialize()
        {
            instance = new SideBySideStereoShader();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
        }

    }



    public class WarpOutputShader: ShaderBundle
    {
        private static WarpOutputShader instance;

        protected static ShaderBytecode pixelShaderByteCodeNoTexture;
        protected static PixelShader pixelShaderNoTexture;

        static SharpDX.Direct3D11.Buffer contantBuffer;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }
        
        private static InputLayout layout;

        public static void Use(DeviceContext context, bool texture, float opacity = 1.0f)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 20, 0),

                   });
            }

            constants.Opacity = opacity;

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(instance.CompiledVertexShader);

            context.VertexShader.SetConstantBuffer(0, contantBuffer);

            context.UpdateSubresource(ref constants, contantBuffer);



            //context.UpdateSubresource(ref matWVP, contantBuffer);

            if (texture)
            {
                context.PixelShader.Set(instance.CompiledPixelShader);
            }
            else
            {
                if (pixelShaderNoTexture == null)
                {
                    MakePixelShaderNoTexture();
                }

                context.PixelShader.Set(pixelShaderNoTexture);
            }

            if (RenderContext11.ExternalProjection)
            {
                context.GeometryShader.SetConstantBuffer(0, contantBuffer);

                context.GeometryShader.SetShader(instance.CompiledGeometryShader, null, 0);
                RenderContext11.UpdateProjectionConstantBuffers();
            }
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                "struct PS_IN                                \n" +
                    "{                                          \n" +
                    "	float4 pos : SV_POSITION;               \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "	float4 color : COLOR;               \n" +
                    "};                                             \n" +
                "    \n" +
                " Texture2D picture;     \n" +
                " SamplerState pictureSampler;   \n" +
                "    \n" +
                "    \n" +
                "    \n" +
                " float4 PS( PS_IN input ) : SV_Target   \n" +
                " {     \n" +
                " 	return picture.Sample(pictureSampler, input.tex) * input.color;   \n" +
                //" 	return float4(1,0,1,0);   \n" +
                //" 	return picture.Sample(pictureSampler, input.tex);   \n" +
                " }   \n" +
                "    ";

        }

        private static void MakePixelShaderNoTexture()
        {
            string shaderText =
                "struct PS_IN                                \n" +
                    "{                                          \n" +
                    "	float4 pos : SV_POSITION;               \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "	float4 color : COLOR;               \n" +
                    "};                                             \n" +
                "    \n" +
                " Texture2D picture;     \n" +
                " SamplerState pictureSampler;   \n" +
                "    \n" +
                "    \n" +
                "    \n" +
                " float4 PS( PS_IN input ) : SV_Target   \n" +
                " {     \n" +
                " 	return input.color;   \n" +
                " }   \n" +
                "    ";
        
            pixelShaderByteCodeNoTexture = ShaderBytecode.Compile(shaderText, "PS", RenderContext11.PixelProfile);
            pixelShaderNoTexture = new PixelShader(RenderContext11.PrepDevice, pixelShaderByteCodeNoTexture);


        }

        protected override string  GetVertexShaderSource(string profile)
        {

            string source =
                   "float4x4 matWVP;                          \n" +
                    "float1 opacity;                             \n" +
                    "struct VS_IN                               \n" +
                    "{                                          \n" +
                    "	float4 pos : POSITION;                  \n" +
                    "	float4 color : COLOR;                   \n" +
                    "	float2 tex : TEXCOORD;                  \n";
            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint   instId   : SV_InstanceID;         \n";
            }
            source +=
                    "};                                         \n" +
                    "                                           \n" +
                    "struct PS_IN                                \n" +
                    "{                                          \n" +
                    "	float4 pos : SV_POSITION;               \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                   "	float4 color : COLOR;               \n";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint        viewId  : TEXCOORD1;          \n" +
                     " };                                            \n" +
                     " cbuffer ViewProjectionConstantBuffer : register(b1) \n" +
                     " {                                             \n" +
                     "      float4x4 viewProjection[2];              \n";
            }

            source +=
                    "};                                             \n" +
                    "                                           \n" +
                    "PS_IN VS( VS_IN input )                    \n" +
                    "{                                          \n " +
                    "	PS_IN output = (PS_IN)0;                \n" +
                    "	  ";
            if (RenderContext11.ExternalProjection)
            {
                source +=
                    "     int idx = input.instId % 2;             \n" +
                    "     output.viewId = idx;                     \n" +
                    "     float4 p = mul(input.pos,  matWVP );  \n" +
                    "     output.pos = mul(p, viewProjection[idx]); \n";
            }
            else
            {
                source +=
                    "     output.pos = mul(input.pos,  matWVP );  \n";
            }
            source +=
                    "	output.tex = input.tex;                     \n" +
                    "	output.color = input.color;                                        \n" +
                    "   output.color.a = input.color.a * opacity; \n" +
                    "	return output;                          \n" +
                    "}                                          \n" +
                    "                                           \n" +
                    "                                           \n";
            return source;
        }

        protected override string GetGeometryShaderSource(string profile)
         {
            return

            "    struct GeometryShaderInput                                                                                 \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n     " +
            "	     float2 tex     : TEXCOORD;                           \n" +
            "        float4 color   : COLOR0;                                                                               \n    " +
            "        uint instId    : TEXCOORD1;                                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // Per-vertex data passed to the rasterizer.                                                               \n " +
            "    struct GeometryShaderOutput                                                                                \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n  " +
            "	     float2 tex     : TEXCOORD;                           \n" +
            "        float4 color   : COLOR0;                                                                               \n   " +
            "        uint rtvId   : SV_RenderTargetArrayIndex;                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // This geometry shader is a pass-through that leaves the geometry unmodified                              \n " +
            "    // and sets the render target array index.                                                                 \n " +
            "    [maxvertexcount(3)]                                                                                        \n " +
            "    void GS(triangle GeometryShaderInput input[3], inout TriangleStream<GeometryShaderOutput> outStream)          \n " +
            "    {                                                                                                          \n " +
            "        GeometryShaderOutput output;                                                                           \n " +
            "        [unroll(3)]                                                                                            \n " +
            "        for (int i = 0; i< 3; ++i)                                                                             \n " +
            "        {                                                                                                      \n " +
            "            output.pos = input[i].pos;                                                                         \n " +
            "            output.color = input[i].color;                                                                     \n " +
            "            output.tex = input[i].tex;                                                                     \n " +
            "            output.rtvId = input[i].instId;                                                                    \n " +
            "            outStream.Append(output);                                                                          \n " +
            "        }                                                                                                      \n " +
            "   }                                                                                                           \n" +
            "                                                                                                               \n ";
        }

        private static WarpShaderConstants constants;

        static Matrix matWVP;

        public static Matrix MatWVP
        {
            set
            {
                constants.MatWVP = value;
            }
        }

        static void initialize()
        {
            instance = new WarpOutputShader();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            contantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<WarpShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }

    [StructLayout(LayoutKind.Explicit, Size = 80)]
    public struct WarpShaderConstants
    {
        [FieldOffset(0)]
        public Matrix MatWVP;
        [FieldOffset(64)]
        public float Opacity;
       
    }


    public class WarpOutputShaderWithBlendTexture : ShaderBundle
    {
        private static WarpOutputShaderWithBlendTexture instance;

        static SharpDX.Direct3D11.Buffer contantBuffer;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }
        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        private static InputLayout layout;

        public static void Use(DeviceContext context, bool texture, float opacity = 1.0f)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 20, 0),
                   });
            }

            constants.Opacity = opacity;

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(instance.CompiledVertexShader);

            context.VertexShader.SetConstantBuffer(0, contantBuffer);

            context.UpdateSubresource(ref constants, contantBuffer);

            context.PixelShader.Set(instance.CompiledPixelShader);

        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                "struct PS_IN                                                                              \n" +
                    "{                                                                                     \n" +
                    "	float4 pos : SV_POSITION;                                                          \n" +
                    "	float4 color : COLOR;                                                              \n" +
                    "	float2 tex : TEXCOORD;                                                             \n" +
                    "	float2 tex1 : TEXCOORD1;                                                           \n" +
                    "};                                                                                    \n" +
                "                                                                                          \n" +
                " Texture2D picture  : register(t0);                                                       \n" +
                " Texture2D blend  : register(t1);                                                         \n" +
                " SamplerState pictureSampler;                                                             \n" +
                "                                                                                          \n" +
                "                                                                                          \n" +
                "                                                                                          \n" +
                " float4 PS( PS_IN input ) : SV_Target                                                     \n" +
                " {                                                                                        \n" +
                "      float2 blendTex = input.tex1;        \n" +
                "      float4 color = blend.Sample( pictureSampler, blendTex);                             \n" +
                "      return picture.Sample(pictureSampler, input.tex) * color;                           \n" +
                " }                                                                                        \n" +
                "                                                                            ";

        }


        protected override string GetVertexShaderSource(string profile)
        {

            string source =
                    "float4x4 matWVP;                              \n" +
                    "struct VS_IN                                 \n" +
                    "{                                            \n" +
                    "	float4 pos : POSITION;                    \n" +
                    "	float4 color : COLOR;                     \n" +
                    "	float2 tex : TEXCOORD;                    \n" +
                    "};                                           \n" +
                    "                                             \n" +
                    "struct PS_IN                                 \n" +
                    "{                                            \n" +
                    "	float4 pos : SV_POSITION;                 \n" +
                     "	float4 color : COLOR;                     \n" +
                     "	float2 tex : TEXCOORD;                    \n" +
                    "	float2 tex1 : TEXCOORD1;                                                             \n" +
                    "};                                           \n" +
                    "                                             \n" +
                    "PS_IN VS( VS_IN input )                      \n" +
                    "{                                            \n" +
                    "	PS_IN output;                         \n" +
                    "	                                          \n "+
                    "   output.pos = mul(input.pos,  matWVP );    \n" +
                    "   output.tex1 = float2((input.pos.x+.5)/1.0, 1-(input.pos.y+.5)/1.0);" +
                    "	output.tex = input.tex;                   \n" +
                    "	output.color = input.color;               \n" +
                    "	return output;                            \n" +
                    "}                                            \n" +
                    "                                             \n" +
                    "                                             \n";
            return source;
        }



        private static WarpShaderConstants constants;

        static Matrix matWVP;

        public static Matrix MatWVP
        {
            set
            {
                constants.MatWVP = value;
            }
        }

        static void initialize()
        {
            instance = new WarpOutputShaderWithBlendTexture();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            contantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<WarpShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }
    }


    public class EllipseShader11 : ShaderBundle
    {
        private static EllipseShader11 instance;
        private static EllipseShaderConstants constants;
        private static InputLayout layout;
        static SharpDX.Direct3D11.Buffer constantBuffer;


        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }
        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static Color4 Color
        {
            set
            {
                constants.Color = value;
            }
        }

        public static Matrix MatWVP
        {
            set
            {
                constants.MatWVP = value;
            }
        }

        public static float SemiMajorAxis
        {
            set
            {
                constants.SemiMajorAxis = value;
            }
        }
        public static float Eccentricity
        {
            set
            {
                constants.Eccentricity = value;
            }
        }
        public static float EccentricAnomaly
        {
            set
            {
                constants.EccentricAnomaly = value;
            }
        }

        public static Matrix MatPositionWVP
        {
            set
            {
                constants.MatPositionWVP = value;
            }
        }

        public static Vector3d PositionNow
        {
            set
            {
                constants.PositionNow.X = (float)value.X;
                constants.PositionNow.Y = (float)value.Y;
                constants.PositionNow.Z = (float)value.Z;
                constants.PositionNow.W = 1;

            }
        }


        public void UseShader(RenderContext11 renderContext, double semiMajorAxis, double eccentricity, double eccentricAnomaly, SharpDX.Color color, Matrix3d world, Vector3d positionNow)
        {
            SemiMajorAxis = (float)semiMajorAxis;
            Eccentricity = (float)eccentricity;
            EccentricAnomaly = (float)eccentricAnomaly;

            Color = color;
            PositionNow = positionNow;

            Matrix matrixWVP = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            if (RenderContext11.ExternalProjection)
            {
                matrixWVP = matrixWVP * RenderContext11.ExternalScalingFactor;
            }

            matrixWVP.Transpose();
            MatWVP = matrixWVP;

            Matrix positionWVP = (world * renderContext.View * renderContext.Projection).Matrix11;
            if (RenderContext11.ExternalProjection)
            {
                positionWVP = positionWVP * RenderContext11.ExternalScalingFactor;
            }

            positionWVP.Transpose();

            MatPositionWVP = positionWVP;
  
            Use(renderContext.devContext);
        }
  

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);

            context.VertexShader.SetConstantBuffer(0, constantBuffer);

            context.UpdateSubresource(ref constants, constantBuffer);

            context.PixelShader.Set(PixelShader);

            if (RenderContext11.ExternalProjection)
            {
                context.GeometryShader.SetConstantBuffer(0, constantBuffer);

                context.GeometryShader.SetShader(instance.CompiledGeometryShader, null, 0);
                RenderContext11.UpdateProjectionConstantBuffers();
            }
        }

        

        protected override string GetPixelShaderSource(string profile)
        {
            return
                " struct VS_OUT                                \n" +
                " {                                            \n" +
                "     float4 ProjPos  : SV_POSITION;              \n" + // Projected space position 
                "     float4 Color    : COLOR;                 \n" +
                " };                                           \n" +
                "                                              \n" +
                " float4 PS(VS_OUT In) : SV_Target               \n" +
                " {                                            \n" +
                "     return In.Color;                         \n" +
                " }                                            ";
        }


        protected override string GetVertexShaderSource(string profile)
        {
            string source =
              " float4x4 matWVP;                             \n" +
              " float4x4 matPositionWVP;                     \n" +
              " float4 positionNow;                          \n" +
              " float4 color;                                \n" +
              " float semiMajorAxis;                         \n" +
              " float eccentricity;                          \n" +
              " float eccentricAnomaly;                      \n" +
             
        
              "\n" +
              " struct VS_IN                                 \n" +
              " {                                            \n" +
              "     float4 angle   : POSITION;               \n";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint   instId   : SV_InstanceID;         \n";
            }
            source +=
              " };                                           \n" +
              "                                              \n" +
              " struct VS_OUT                                \n" +
              " {                                            \n" +
              "     float4 ProjPos   : SV_POSITION;             \n" + // Projected space position 
              "     float4 Color     : COLOR;                \n";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint        viewId  : TEXCOORD0;          \n" +
                     " };                                            \n" +
                     " cbuffer ViewProjectionConstantBuffer : register(b1) \n" +
                     " {                                             \n" +
                     "      float4x4 viewProjection[2];              \n";
            }

            source +=
              " };                                           \n" +
              "\n" +
              " VS_OUT VS(VS_IN In)                        \n" +
              " {                                            \n" +
              "     VS_OUT Out;                              \n";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                    "     int idx = In.instId % 2;             \n" +
                    "     Out.viewId = idx;                     \n";
            }


            source +=
              "                                             \n" +
              "     float fade = (1.0 - In.angle.x);         \n" +
              "     float PI = 3.1415927;                    \n" +
              "     float E = eccentricAnomaly - In.angle.x * 2 * PI;                    \n" +
              "     float2 semiAxes = float2(1.0, sqrt(1.0 - eccentricity * eccentricity)) * semiMajorAxis;   \n" +
              "     float2 planePos = semiAxes * float2(cos(E) - eccentricity, sin(E));  \n" +
              "     if (In.angle.x == 0.0)                                               \n";
            if (RenderContext11.ExternalProjection)
            {
                source +=
              "     {                                                                               \n" +
              "         float4 p = mul(positionNow, matPositionWVP);                                \n" +
              "         Out.ProjPos = mul(p, viewProjection[idx]);                                  \n" +
              "     }                                                                               \n" +
              "     else                                                                            \n" +
              "     {                                                                               \n" +
              "         float4 p = mul(float4(planePos.x, planePos.y, 0.0, 1.0), matWVP);           \n" +
              "         Out.ProjPos = mul(p, viewProjection[idx]);                                  \n" +
              "     }                                                                               \n";
            }
            else
            {
                source +=
              "         Out.ProjPos = mul(positionNow, matPositionWVP);                  \n" +
              "     else                                                                 \n" +
              "         Out.ProjPos = mul(float4(planePos.x, planePos.y, 0.0, 1.0), matWVP); \n";

            }

            source +=
              "     Out.Color = float4(color.rgb, fade * color.a);                       \n" +
              "     return Out;                                                          \n" +
              " }\n";

            return source;
        }

        protected override string GetGeometryShaderSource(string profile)
        {
            return

            "    struct GeometryShaderInput                                                                                 \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n     " +
            "        float4 color   : COLOR0;                                                                               \n    " +
            "        uint instId  : TEXCOORD0;                                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // Per-vertex data passed to the rasterizer.                                                               \n " +
            "    struct GeometryShaderOutput                                                                                \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n  " +
            "        float4 color   : COLOR0;                                                                               \n   " +
            "        uint rtvId   : SV_RenderTargetArrayIndex;                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // This geometry shader is a pass-through that leaves the geometry unmodified                              \n " +
            "    // and sets the render target array index.                                                                 \n " +
            "    [maxvertexcount(2)]                                                                                        \n " +
            "    void GS(line GeometryShaderInput input[2], inout LineStream<GeometryShaderOutput> outStream)          \n " +
            "    {                                                                                                          \n " +
            "        GeometryShaderOutput output;                                                                           \n " +
            "        [unroll(2)]                                                                                            \n " +
            "        for (int i = 0; i< 2; ++i)                                                                             \n " +
            "        {                                                                                                      \n " +
            "            output.pos = input[i].pos;                                                                         \n " +
            "            output.color = input[i].color;                                                                     \n " +
            "            output.rtvId = input[i].instId;                                                                    \n " +
            "            outStream.Append(output);                                                                          \n " +
            "        }                                                                                                      \n " +
            "   }                                                                                                           \n" +
            "                                                                                                               \n ";
        }

        static void initialize()
        {
            instance = new EllipseShader11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<EllipseShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }

    [StructLayout(LayoutKind.Explicit, Size = 176)]
    public struct EllipseShaderConstants
    {
        [FieldOffset(0)]
        public Matrix MatWVP;
        [FieldOffset(64)]
        public Matrix MatPositionWVP;
        [FieldOffset(128)]
        public Vector4 PositionNow;
        [FieldOffset(144)]
        public Color4 Color;
        [FieldOffset(160)]
        public float SemiMajorAxis;
        [FieldOffset(164)]
        public float Eccentricity;
        [FieldOffset(168)]
        public float EccentricAnomaly;
    }


    // The orbit trace shader is used for plotting orbits that fade
    // from opaque to transparent over time. The input vertices are a
    // position plus a time offset that is used to calculate the 
    // opacity. A head point at the exact current position of the
    // orbiting object will generally be used instead of the first
    // position in the vertex buffer.
    public class OrbitTraceShader : ShaderBundle
    {
        private static OrbitTraceShader instance;
        private static OrbitPathShaderConstants constants;
        private static InputLayout layout;
        static SharpDX.Direct3D11.Buffer constantBuffer;


        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }
        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static Color4 Color
        {
            set
            {
                constants.Color = value;
            }
        }

        public static Matrix MatWVP
        {
            set
            {
                constants.MatWVP = value;
            }
        }

        public static float TimeOffset
        {
            set
            {
                constants.TimeOffset = value;
            }
        }

        public static float CoverageDuration
        {
            // Coverate duration is a multiple of the window duration (which is typically the
            // orbital period of the body.)
            set
            {
                constants.CoverageDuration = value;
            }
        }

        public static Matrix MatPositionWVP
        {
            set
            {
                constants.MatPositionWVP = value;
            }
        }

        public static Vector3d PositionNow
        {
            set
            {
                constants.PositionNow.X = (float)value.X;
                constants.PositionNow.Y = (float)value.Y;
                constants.PositionNow.Z = (float)value.Z;
                constants.PositionNow.W = 1;
            }
        }


        public static void UseShader(RenderContext11 renderContext, SharpDX.Color color, Matrix3d world, Vector3d positionNow, double timeOffset, double coverageDuration)
        {
            TimeOffset = (float)timeOffset;
            CoverageDuration = (float)coverageDuration;

            Color = color;
            PositionNow = positionNow;

            Matrix matrixWVP = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            if (RenderContext11.ExternalProjection)
            {
                matrixWVP = matrixWVP * RenderContext11.ExternalScalingFactor;
            }

            matrixWVP.Transpose();
            MatWVP = matrixWVP;

            Matrix positionWVP = (world * renderContext.View * renderContext.Projection).Matrix11;
            if (RenderContext11.ExternalProjection)
            {
                positionWVP = positionWVP * RenderContext11.ExternalScalingFactor;
            }
            positionWVP.Transpose();

            MatPositionWVP = positionWVP;

            Use(renderContext.devContext);
        }


        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("TIME", 0, SharpDX.DXGI.Format.R32_Float, 12, 0),
                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);

            context.VertexShader.SetConstantBuffer(0, constantBuffer);

            context.UpdateSubresource(ref constants, constantBuffer);

            context.PixelShader.Set(PixelShader);

            if (RenderContext11.ExternalProjection)
            {
                context.GeometryShader.SetConstantBuffer(0, constantBuffer);

                context.GeometryShader.SetShader(instance.CompiledGeometryShader, null, 0);
                RenderContext11.UpdateProjectionConstantBuffers();
            }
        }



        protected override string GetPixelShaderSource(string profile)
        {
            return
                " struct VS_OUT                                \n" +
                " {                                            \n" +
                "     float4 ProjPos  : SV_POSITION;              \n" + // Projected space position 
                "     float4 Color    : COLOR;                 \n" +
                " };                                           \n" +
                "                                              \n" +
                " float4 PS(VS_OUT In) : SV_Target               \n" +
                " {                                            \n" +
                "     return In.Color;                         \n" +
                " }                                            ";
        }


        protected override string GetVertexShaderSource(string profile)
        {
            string source =
              " float4x4 matWVP;                             \n" +
              " float4x4 matPositionWVP;                     \n" +
              " float4 positionNow;                          \n" +
              " float4 color;                                \n" +
              " float timeOffset;                            \n" +
              "\n" +
              " struct VS_IN                                 \n" +
              " {                                            \n" +
              "     float4 position   : POSITION;            \n" +
              "     float time        : TIME;                \n"; 
            
            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint   instId   : SV_InstanceID;         \n";
            }
            source +=
              " };                                           \n" +
              "                                              \n" +
              " struct VS_OUT                                \n" +
              " {                                            \n" +
              "     float4 ProjPos   : SV_POSITION;          \n" + // Projected space position 
              "     float4 Color     : COLOR;                \n";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                     "     uint        viewId  : TEXCOORD0;          \n" +
                     " };                                            \n" +
                     " cbuffer ViewProjectionConstantBuffer : register(b1) \n" +
                     " {                                             \n" +
                     "      float4x4 viewProjection[2];              \n";
            }

            source +=
              " };                                           \n" +
              "\n" +
              " VS_OUT VS(VS_IN In)                        \n" +
              " {                                            \n" +
              "     VS_OUT Out;                              \n";

            if (RenderContext11.ExternalProjection)
            {
                source +=
                    "     int idx = In.instId % 2;             \n" +
                    "     Out.viewId = idx;                     \n";
            }


            source +=
              "                                                                         \n" +
              "     float fade = 1.0 - (In.time - timeOffset) * 1.5;                       \n" +
              "     if (In.time - timeOffset < 0)                                        \n";


            if (RenderContext11.ExternalProjection)
            {
                source +=
                    "     {                                                                  \n" +
                    "       float4 p = mul(positionNow,  matPositionWVP );                  \n" +
                    "       Out.ProjPos = mul(p, viewProjection[idx]);                      \n" +
                    "     }                                                                 \n" +
                    "     else                                                              \n" +
                    "     {                                                                 \n" +
                    "       float4 p = mul(In.position,  matWVP );                           \n" +
                    "       Out.ProjPos = mul(p, viewProjection[idx]);                      \n" +
                    "     }                                                                 \n";
            }
            else
            {
                source +=
                    "         Out.ProjPos = mul(positionNow, matPositionWVP);                  \n" +
                    "     else                                                                 \n" +
                    "         Out.ProjPos = mul(In.position, matWVP);                             \n";
            }

            source +=


              "     Out.Color = float4(color.rgb, fade * color.a);                       \n" +
              "     return Out;                                                          \n" +
              " }\n";

            return source;
        }

        protected override string GetGeometryShaderSource(string profile)
        {
            return

            "    struct GeometryShaderInput                                                                                 \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n     " +
            "        float4 color   : COLOR0;                                                                               \n    " +
            "        uint instId  : TEXCOORD0;                                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // Per-vertex data passed to the rasterizer.                                                               \n " +
            "    struct GeometryShaderOutput                                                                                \n " +
            "    {                                                                                                          \n " +
            "        float4 pos     : SV_POSITION;                                                                          \n  " +
            "        float4 color   : COLOR0;                                                                               \n   " +
            "        uint rtvId   : SV_RenderTargetArrayIndex;                                                              \n " +
            "    };                                                                                                         \n " +
            "                                                                                                               \n " +
            "    // This geometry shader is a pass-through that leaves the geometry unmodified                              \n " +
            "    // and sets the render target array index.                                                                 \n " +
            "    [maxvertexcount(2)]                                                                                        \n " +
            "    void GS(line GeometryShaderInput input[2], inout LineStream<GeometryShaderOutput> outStream)          \n " +
            "    {                                                                                                          \n " +
            "        GeometryShaderOutput output;                                                                           \n " +
            "        [unroll(2)]                                                                                            \n " +
            "        for (int i = 0; i< 2; ++i)                                                                             \n " +
            "        {                                                                                                      \n " +
            "            output.pos = input[i].pos;                                                                         \n " +
            "            output.color = input[i].color;                                                                     \n " +
            "            output.rtvId = input[i].instId;                                                                    \n " +
            "            outStream.Append(output);                                                                          \n " +
            "        }                                                                                                      \n " +
            "   }                                                                                                           \n" +
            "                                                                                                               \n ";
        }

        static void initialize()
        {
            instance = new OrbitTraceShader();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<EllipseShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }

    [StructLayout(LayoutKind.Explicit, Size = 192)]
    public struct OrbitPathShaderConstants
    {
        [FieldOffset(0)]
        public Matrix MatWVP;
        [FieldOffset(64)]
        public Matrix MatPositionWVP;
        [FieldOffset(128)]
        public Vector4 PositionNow;
        [FieldOffset(144)]
        public Color4 Color;
        [FieldOffset(160)]
        public float TimeOffset;
        [FieldOffset(176)]
        public float CoverageDuration;
    }


    public class PointSpriteShader11 : ShaderBundle
    {
        private static PointSpriteShader11 instance;
        private static SharpDX.Direct3D11.Buffer constantBuffer;


        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static GeometryShader GeometryShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledGeometryShader;
            }
        }

        public static Color4 Color
        {
            set
            {
                constants.Color = value;
            }
        }

        // PointScaleFactors is a vector (A, B, C)
        // The point scale is computed as sqrt(A + B/d + C/d^2) where d is
        // the distance to the vertex in camera space.
        public static Vector3 PointScaleFactors
        {
            set
            {
                constants.PointScaleFactors = new Vector4(value.X, value.Y, value.Z, 0.0f);
            }
        }

        public static Vector2 ViewportScale
        {
            set
            {
                constants.ViewportScale = value;
            }
        }

        public static Matrix WVPMatrix
        {
            set
            {
                constants.WorldViewProjection = value;
            }
        }

        public static float MinPointSize
        {
            set
            {
                constants.MinPointSize = value;
            }
        }

        private static PointSpriteShaderConstants constants;
        private static InputLayout layout;

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 12, 0),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float, 16, 0)
                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.GeometryShader.Set(GeometryShader);
            context.GeometryShader.SetConstantBuffer(0, constantBuffer);
            context.PixelShader.Set(PixelShader);

            context.UpdateSubresource(ref constants, constantBuffer);
        }

        static string VertexInput
        {
            get
            {
                string source =
                    " struct VS_IN                                 \n" +
                    " {                                            \n" +
                    "     float4 Position   : POSITION;            \n" +
                    "     float4 Color      : COLOR;               \n" +
                    "     float PointSize   : POINTSIZE;           \n";
                if (RenderContext11.ExternalProjection)
                {
                    source +=
                         "     uint   instId   : SV_InstanceID;         \n";
                }
                source +=
                        " };                                           \n";

                return source;
            }
        }

        static string GeometryInput
        {
            get
            {
                string source = 
                    " struct GS_IN                                 \n" +
                    " {                                            \n" +
                    "     float4 Center     : POSITION;            \n" +
                    "     float2 PointSize  : POINTSIZE;           \n" +
                    "     float4 Color      : COLOR;               \n";
                if (RenderContext11.ExternalProjection)
                {
                    source +=
                         "       uint        viewId  : TEXCOORD1;          \n";
                }
                source +=
                        " };                                           \n";

                return source;
            }
        }


        static string PixelInput
        {
            get
            {
                string source =
                    " struct PS_IN                                 \n" +
                    " {                                            \n" +
                    "     float4 ProjPos   : SV_POSITION;          \n" +
                    "     float4 Color     : COLOR;                \n" +
                    "     float2 TexCoord  : TEXCOORD0;            \n";

                if (RenderContext11.ExternalProjection)
                {
                    source +=
                    "       uint rtvId   : SV_RenderTargetArrayIndex;           \n";
                }

                source +=
                    " };                                           \n";

                return source;
            }
        }

        static string ConstantDeclarations
        {
            get
            {
                string source =
                    " float4x4 matWVP;                             \n" +
                    " float4 color : COLOR;                        \n" +
                    " float4 pointScaleFactors;                    \n" +
                    " float2 viewportScale;                        \n" +
                    " float minPointSize;                          \n";

                if (RenderContext11.ExternalProjection)
                {
                    source +=
                        " cbuffer ViewProjectionConstantBuffer : register(b1) \n" +
                         " {                                             \n" +
                         "      float4x4 viewProjection[2];              \n" +
                         " };                                            \n";
                }

                return source;
            }
        }
        
        protected override string GetVertexShaderSource(string profile)
        {
            string source =
                    ConstantDeclarations +
                    VertexInput +
                    GeometryInput +
                    "                                                                       \n" +
                    " GS_IN VS( VS_IN In )                                                  \n" +
                    " {                                                                     \n" +
                    "     GS_IN Out;                                                        \n";
            if (RenderContext11.ExternalProjection)
            {
                source +=
                    "     int idx = In.instId % 2;                                          \n" +
                    "     Out.viewId = idx;                                                 \n" +
                    "     float4 p = mul(In.Position,  matWVP );                            \n" +
                    "     float4 center = mul(p, viewProjection[idx]);                      \n" +
                    "     Out.Center = center;                                              \n";
            }
            else
            {
                source +=
                    "     float4 center = mul(In.Position, matWVP);                         \n" +
                    "     Out.Center = center;                                              \n";
            }

            source +=

                    "     float d = 1.0 / length(center.xyz);                                       \n" +
                    "     float pointScale = sqrt(pointScaleFactors.x + pointScaleFactors.y * d + pointScaleFactors.z * d * d); \n" +
                    "     float a = pointScale * In.PointSize;                                      \n" +
                    "     Out.PointSize = max(minPointSize, In.PointSize * pointScale) * viewportScale;            \n" +
                    "     Out.Color = In.Color * color;                                                     \n" +
                    "     Out.Color.a = max(0, min(1, (1024-a)/512)) * Out.Color.a;                         \n" +
                    "     return Out;                                                                       \n" +
                    " }                                                                                     \n";

            return source;
        }

        protected override string GetGeometryShaderSource(string profile)
        {
            string source =
                GeometryInput +
                PixelInput +
                " [maxvertexcount(4)]                                                                           \n" +
                " void GS(point GS_IN In[1], inout TriangleStream<PS_IN> stream)                                \n" +
                " {                                                                                             \n" +
                "     float4 center = In[0].Center;                                                             \n" +
                "     float2 halfSize = 0.5 * In[0].PointSize * center.w;                                       \n" +
                "     float4 p0 = center + float4( halfSize.x, -halfSize.y, 0.0f, 0.0f);                        \n" +
                "     float4 p1 = center + float4( halfSize.x,  halfSize.y, 0.0f, 0.0f);                        \n" +
                "     float4 p2 = center + float4(-halfSize.x, -halfSize.y, 0.0f, 0.0f);                        \n" +
                "     float4 p3 = center + float4(-halfSize.x,  halfSize.y, 0.0f, 0.0f);                        \n" +
                "     if ( In[0].Color.a < .002f)                                                               \n" +
                "     {                                                                                         \n" +
                "           return;                                                                             \n" +
                "     }                                                                                         \n";

            source +=
                "     PS_IN Out;                                                                                \n";
            if (RenderContext11.ExternalProjection)
            {
                source +=
                "     Out.rtvId = In[0].viewId;                                                                    \n ";
            }

            source +=  
                "     Out.Color = In[0].Color;                                                                  \n" +
                "     Out.ProjPos = p0;                                                                         \n" +
                "     Out.TexCoord = float2(1, 1);                                                              \n" +
                "     stream.Append(Out);                                                                       \n" +
                "     Out.ProjPos = p1;                                                                         \n" +
                "     Out.TexCoord = float2(1, 0);                                                              \n" +
                "     stream.Append(Out);                                                                       \n" +
                "     Out.ProjPos = p2;                                                                         \n" +
                "     Out.TexCoord = float2(0, 1);                                                              \n" +
                "     stream.Append(Out);                                                                       \n" +
                "     Out.ProjPos = p3;                                                                         \n" +
                "     Out.TexCoord = float2(0, 0);                                                              \n" +
                "     stream.Append(Out);                                                                       \n" +
                " }                                                                                             \n";

            return source;
        }

        //protected override string GetGeometryShaderSource(string profile)
        //{
        //    return
        //        GeometryInput +
        //        PixelInput +
        //        " [maxvertexcount(4)]                                                    \n" +
        //        " void GS(point GS_IN In[1], inout TriangleStream<PS_IN> stream)         \n" +
        //        " {                                                                      \n" +
        //        "     float4 center = In[0].Center;                                      \n" +
        //        "     float2 halfSize = 0.5 * In[0].PointSize * center.w;                \n" +
        //        "     float4 p0 = center + float4( halfSize.x, -halfSize.y, 0.0f, 0.0f); \n" +
        //        "     float4 p1 = center + float4( halfSize.x,  halfSize.y, 0.0f, 0.0f); \n" +
        //        "     float4 p2 = center + float4(-halfSize.x, -halfSize.y, 0.0f, 0.0f); \n" +
        //        "     float4 p3 = center + float4(-halfSize.x,  halfSize.y, 0.0f, 0.0f); \n" +
        //        //"     if ( In[0].Color.a < .008f)                        \n" +
        //        //"     {                                        \n" +
        //        //"           return;                            \n" +
        //        //"     }                                        \n" +
        //        //"     col.r = halfSize.y/1024;                       \n" +
        //        //"     col.a = alpha;                           \n" +
        //        "     PS_IN Out;                               \n" +
        //        "     Out.Color = In[0].Color                  \n" +
        //        "     Out.ProjPos = p0;                        \n" +
        //        "     Out.TexCoord = float2(1, 1);             \n" +
        //        "     stream.Append(Out);                      \n" +
        //        "     Out.ProjPos = p1;                        \n" +
        //        "     Out.TexCoord = float2(1, 0);             \n" +
        //        "     stream.Append(Out);                      \n" +
        //        "     Out.ProjPos = p2;                        \n" +
        //        "     Out.TexCoord = float2(0, 1);             \n" +
        //        "     stream.Append(Out);                      \n" +
        //        "     Out.ProjPos = p3;                        \n" +
        //        "     Out.TexCoord = float2(0, 0);             \n" +
        //        "     stream.Append(Out);                      \n" +
        //        " }                                            \n";
        //}

        //protected string GetGeometryShaderSourceOld(string profile)
        //{
        //    return
        //        GeometryInput +
        //        PixelInput +
        //        " [maxvertexcount(4)]                                                    \n" +
        //        " void GS(point GS_IN In[1], inout TriangleStream<PS_IN> stream)         \n" +
        //        " {                                                                      \n" +
        //        "     float4 center = In[0].Center;                                      \n" +
        //        "     float2 halfSize = 0.5 * In[0].PointSize * center.w;                \n" +
        //        "     float4 p0 = center + float4( halfSize.x, -halfSize.y, 0.0f, 0.0f); \n" +
        //        "     float4 p1 = center + float4( halfSize.x,  halfSize.y, 0.0f, 0.0f); \n" +
        //        "     float4 p2 = center + float4(-halfSize.x, -halfSize.y, 0.0f, 0.0f); \n" +
        //        "     float4 p3 = center + float4(-halfSize.x,  halfSize.y, 0.0f, 0.0f); \n" +
        //        "     PS_IN Out;                               \n" +
        //        "     Out.Color = In[0].Color;                 \n" +
        //        "     Out.ProjPos = p0;                        \n" +
        //        "     Out.TexCoord = float2(1, 1);             \n" +
        //        "     stream.Append(Out);                      \n" +
        //        "     Out.ProjPos = p1;                        \n" +
        //        "     Out.TexCoord = float2(1, 0);             \n" +
        //        "     stream.Append(Out);                      \n" +
        //        "     Out.ProjPos = p2;                        \n" +
        //        "     Out.TexCoord = float2(0, 1);             \n" +
        //        "     stream.Append(Out);                      \n" +
        //        "     Out.ProjPos = p3;                        \n" +
        //        "     Out.TexCoord = float2(0, 0);             \n" +
        //        "     stream.Append(Out);                      \n" +
        //        " }                                            \n";
        //}


        protected override string GetPixelShaderSource(string profile)
        {
            return
                PixelInput +
                "Texture2D starTexture;                        \n" +
                "SamplerState starSampler;                     \n" +
                "                                              \n" +
                " float4 PS( PS_IN In ) : SV_Target            \n" +
                " {                                            \n" +
                "    float4 color = In.Color * starTexture.Sample(starSampler, In.TexCoord);\n" +
                "    clip( color.a == 0 ? -1:1 );      \n" +
                "    return  color;                      \n" +
               " }                                            ";
        }

        static void initialize()
        {
            instance = new PointSpriteShader11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<PointSpriteShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }

    [StructLayout(LayoutKind.Explicit, Size = 112)]
    public struct PointSpriteShaderConstants
    {
        [FieldOffset(0)]
        public Matrix WorldViewProjection;

        [FieldOffset(64)]
        public Color4 Color;

        [FieldOffset(80)]
        public Vector4 PointScaleFactors;

        [FieldOffset(96)]
        public Vector2 ViewportScale;

        [FieldOffset(104)]      
        public float MinPointSize;
    }


    public abstract class TimeSeriesPointSpriteShaderBase : ShaderBundle
    {
        // Constants shared between different time series point sprite
        // shaders (downlevel and D3D10+)
        protected static SharpDX.Direct3D11.Buffer constantBuffer;

        public static TimeSeriesPointSpriteShaderConstants Constants;

        public static Color4 Color
        {
            set
            {
                Constants.Color = value;
            }
        }

        // PointScaleFactors is a vector (A, B, C)
        // The point scale is computed as sqrt(A + B/d + C/d^2) where d is
        // the distance to the vertex in camera space.
        public static Vector3 PointScaleFactors
        {
            set
            {
                Constants.PointScaleFactors = new Vector4(value.X, value.Y, value.Z, 0.0f);
            }
        }

        public static Vector2 ViewportScale
        {
            set
            {
                Constants.ViewportScale = value;
            }
        }

        public static Matrix WVPMatrix
        {
            set
            {
                Constants.WorldViewProjection = value;
            }
        }
    }


    public class TimeSeriesPointSpriteShader11 : TimeSeriesPointSpriteShaderBase
    {
        private static TimeSeriesPointSpriteShader11 instance;

        private static InputLayout layout;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static GeometryShader GeometryShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledGeometryShader;
            }
        }

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float, 12, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 20, 0),

                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.GeometryShader.Set(GeometryShader);
          
            context.PixelShader.Set(PixelShader);

            context.UpdateSubresource(ref Constants, constantBuffer);
        }

        static string VertexInput =
            " struct VS_IN                                 \n" +
            " {                                            \n" +
            "     float4 Position   : POSITION;            \n" +
            "     float PointSize   : POINTSIZE;           \n" +
            "     float4 Color      : COLOR;               \n" +
            "     float2 Time   : TEXCOORD0;               \n" +  
            " };                                           \n";

        static string GeometryInput =
            " struct GS_IN                                 \n" +
            " {                                            \n" +
            "     float4 Center     : POSITION;            \n" +
            "     float2 PointSize  : POINTSIZE;           \n" +
            "     float4 Color      : COLOR;               \n" +
            " };                                           \n";

        static string PixelInput =
            " struct PS_IN                                 \n" +
            " {                                            \n" +
            "     float4 ProjPos   : SV_POSITION;          \n" +
            "     float4 Color     : COLOR;                \n" +
            "     float2 TexCoord  : TEXCOORD0;            \n" +
            " };                                           \n";

        static string ConstantDeclarations =
            " float4x4 matWVP;                             \n" +
            " float4 color : COLOR;                        \n" +
            " float4 pointScaleFactors;                    \n" +
            " float4 camPos : POSITION;                    \n" +
            " float2 viewportScale;                        \n" +
            " float1 opacity;                              \n" +
            " float1 jNow;                                 \n" +
            " float1 decay;                                \n" +
            " float1 sky;                                  \n" +
            " float1 showFarSide;                          \n" +
            " float1 scale;                                \n";
                                                           
        protected override string GetVertexShaderSource(string profile)
        {
            return
                    ConstantDeclarations +
                    VertexInput +
                    GeometryInput +
                    "                                                                                                           \n" +
                    " GS_IN VS( VS_IN In )                                                                                      \n" +
                    " {                                                                                                         \n" +
                    "     GS_IN Out;                                                                                            \n" +
                    "     float dotCam = dot((camPos.xyz - In.Position.xyz), In.Position.xyz);                                  \n" +                   
                    "     float dist = distance(In.Position.xyz, camPos.xyz);                                                   \n" +
                    "     float dAlpha = 1;                                                                                     \n" +
                    "     if ( decay > 0)                                                                                       \n" +
                    "     {                                                                                                     \n" +
                    "          dAlpha = 1 - ((jNow - In.Time.y) / decay);                                                       \n " +
                    "          if (dAlpha > 1 )                                                                                 \n" +
                    "          {                                                                                                \n" +
                    "               dAlpha = 1;                                                                                 \n" +
                    "          }                                                                                                \n" +
                    "     }                                                                                                     \n" +
                    "                                                                                                           \n" +
                    "     float4 center = mul(In.Position, matWVP);                                                             \n" +
                    "     Out.Center = center;                                                                                  \n" +
                    "     float d = 1.0 / length(center.xyz);                                                                   \n" +
                    "     float pointScale = sqrt(pointScaleFactors.x + pointScaleFactors.y * d + pointScaleFactors.z * d * d); \n" +
                    "     Out.PointSize = max(1.0, In.PointSize * pointScale * scale) * viewportScale;                          \n" +
                    "     if ( scale > 0)                                                                                       \n" +
                    "     {                                                                                                     \n" +
                    "       Out.PointSize =  viewportScale * max(1.0, (scale * (In.PointSize )/ dist)) ; \n" +
                    "     }                                                                                                     \n" +
                    "     else                                                                                                  \n" +
                    "     {                                                                                                     \n" +
                    "       Out.PointSize = viewportScale * max(1.0,(-scale *In.PointSize ));                                  \n" +
                    "     }                                                                                                     \n" +
                    "     Out.Color = In.Color * color;                                                                         \n" +
                    "     if (showFarSide == 0 && (dotCam * sky) < 0 || (jNow < In.Time.x && decay > 0))                        \n" +
                    "     {                                                                                                     \n" +
                    "        Out.Color.a = 0;                                                                                   \n" +
                    "     }                                                                                                     \n" +
                    "     else                                                                                                  \n" +
                    "     {                                                                                                     \n" +
                    "        Out.Color.a = In.Color.a * dAlpha * opacity;                                                       \n" +
                    "     }                                                                                                     \n" +
                    "     return Out;                                                                                           \n" +
                    " }                                                                                                         \n";
        }

        protected override string GetGeometryShaderSource(string profile)
        {
            return
                GeometryInput +
                PixelInput +
                " [maxvertexcount(4)]                                                    \n" +
                " void GS(point GS_IN In[1], inout TriangleStream<PS_IN> stream)         \n" +
                " {                                                                      \n" +
                "     float4 center = In[0].Center;                                      \n" +
                "     float2 halfSize = 0.5 * In[0].PointSize * center.w;                \n" +
                "     float4 p0 = center + float4( halfSize.x, -halfSize.y, 0.0f, 0.0f); \n" +
                "     float4 p1 = center + float4( halfSize.x,  halfSize.y, 0.0f, 0.0f); \n" +
                "     float4 p2 = center + float4(-halfSize.x, -halfSize.y, 0.0f, 0.0f); \n" +
                "     float4 p3 = center + float4(-halfSize.x,  halfSize.y, 0.0f, 0.0f); \n" +
                "     PS_IN Out;                               \n" +
                "     Out.Color = In[0].Color;                 \n" +
                "     Out.ProjPos = p0;                        \n" +
                "     Out.TexCoord = float2(1, 1);             \n" +
                "     stream.Append(Out);                      \n" +
                "     Out.ProjPos = p1;                        \n" +
                "     Out.TexCoord = float2(1, 0);             \n" +
                "     stream.Append(Out);                      \n" +
                "     Out.ProjPos = p2;                        \n" +
                "     Out.TexCoord = float2(0, 1);             \n" +
                "     stream.Append(Out);                      \n" +
                "     Out.ProjPos = p3;                        \n" +
                "     Out.TexCoord = float2(0, 0);             \n" +
                "     stream.Append(Out);                      \n" +
                " }                                            \n";
        }

        protected override string  GetPixelShaderSource(string profile)
        {
            return
                PixelInput +
                "Texture2D starTexture;                        \n" +
                "SamplerState starSampler;                     \n" +
                "                                              \n" +
                " float4 PS( PS_IN In ) : SV_Target            \n" +
                " {                                            \n" +
                "     return In.Color * starTexture.Sample(starSampler, In.TexCoord);\n" +
                " }                                            ";
        }

        static void initialize()
        {
            instance = new TimeSeriesPointSpriteShader11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<TimeSeriesPointSpriteShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }


    [StructLayout(LayoutKind.Explicit, Size = 144)]
    public struct TimeSeriesPointSpriteShaderConstants
    {
        [FieldOffset(0)]
        public Matrix WorldViewProjection;

        [FieldOffset(64)]
        public Color4 Color;

        [FieldOffset(80)]
        public Vector4 PointScaleFactors;
      
        [FieldOffset(96)]
        public Vector4 CameraPosition;

        [FieldOffset(112)]
        public Vector2 ViewportScale;
        
        [FieldOffset(120)]
        public float Opacity;
        
        [FieldOffset(124)]
        public float JNow;
        
        [FieldOffset(128)]
        public float Decay;
        
        [FieldOffset(132)]
        public float Sky;
        
        [FieldOffset(136)]
        public float ShowFarSide;
        
        [FieldOffset(140)]
        public float Scale;
    }

    public class TimeSeriesColumnChartShader11 : ShaderBundle
    {
        private static TimeSeriesColumnChartShader11 instance;
        private static SharpDX.Direct3D11.Buffer constantBuffer;
        private static SharpDX.Direct3D11.Buffer geometryConstantBuffer;
        public static TimeSeriesPointSpriteShaderConstants Constants;
        private static InputLayout layout;


        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }
        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

    
        public static Color4 Color
        {
            set
            {
                Constants.Color = value;
            }
        }



        public static Vector2 ViewportScale
        {
            set
            {
                Constants.ViewportScale = value;
            }
        }

        public static Matrix WVPMatrix
        {
            set
            {
                Constants.WorldViewProjection = value;
            }
        }

        private static bool colChart = false;

        public static bool ColChart
        {
            set { colChart = value; }
        }

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float, 12, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 20, 0),

                    });
            }


            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.GeometryShader.Set(instance.CompiledGeometryShader);

            context.GeometryShader.SetConstantBuffer(0, geometryConstantBuffer);

            context.UpdateSubresource(ref Constants.WorldViewProjection, geometryConstantBuffer);

            context.PixelShader.Set(instance.CompiledPixelShader);

            context.UpdateSubresource(ref Constants, constantBuffer);
        }

        static string VertexInput =
            " struct VS_IN                                 \n" +
            " {                                            \n" +
            "     float4 Position   : POSITION;            \n" +
            "     float PointSize   : POINTSIZE;           \n" +
            "     float4 Color      : COLOR;               \n" +
            "     float2 Time   : TEXCOORD0;              " + // Object Point size 
            " };                                           \n";

        static string GeometryInput =
            " struct GS_IN                                 \n" +
            " {                                            \n" +
            "     float4 Center     : POSITION;            \n" +
            "     float2 PointSize  : POINTSIZE;           \n" +
            "     float4 Color      : COLOR;               \n" +
            " };                                           \n";

        static string PixelInput =
            " struct PS_IN                                 \n" +
            " {                                            \n" +
            "     float4 ProjPos   : SV_POSITION;          \n" +
            "     float4 Color     : COLOR;                \n" +
            " };                                           \n";

        static string ConstantDeclarations =
            " float4x4 matWVP;                             \n" +
            " float4 color : COLOR;                        \n" +
            " float4 pointScaleFactors;                    \n" +
            " float4 camPos : POSITION;                    \n" +
            " float2 viewportScale;                        \n" +
            " float1 opacity;                              \n" +
            " float1 jNow;                                 \n" +
            " float1 decay;                                \n" +
            " float1 sky;                                  \n" +
            " float1 showFarSide;                          \n" +
            " float1 scale;                                \n";

        protected override string  GetVertexShaderSource(string profile)
        {
            return
                    ConstantDeclarations +
                    VertexInput +
                    GeometryInput +
                // "                                                                                                           \n" +
                //" GS_IN VS( VS_IN In )                                                                                      \n" +
                //" {                                                                                                         \n" +
                //"     GS_IN Out;                                                                                            \n" +
                //"     Out.Center = In.Position;                                                                                  \n" +
                //"     Out.PointSize = 1;                                 \n" +
                //"     Out.Color = In.Color;                                                                         \n" +
                //"     return Out;                                                                                           \n" +
                //" }                                                                                                         \n";

                    " GS_IN VS( VS_IN In )                                                                                      \n" +
                    " {                                                                                                         \n" +
                    "     GS_IN Out;                                                                                            \n" +
                    //"     float3 poNorm = normalize(In.Position.xyz);                                                                                             \n" +
                    //"     Out.Center = float4(lerp(poNorm, In.Position.xyz,float3(scale,scale,scale)),1);                                                                                             \n" +
                    //"                                                                                                  \n" +
                    "     Out.Center = In.Position;                                                                                  \n" +
                    "     float dotCam = dot((camPos.xyz - Out.Center.xyz), Out.Center.xyz);                                  \n" +
                    "     float dist = distance(Out.Center.xyz, camPos.xyz);                                                       \n" +
                    "     float dAlpha = 1;                                                                                     \n" +
                    "     if ( decay > 0)                                                                                       \n" +
                    "     {                                                                                                     \n" +
                    "          dAlpha = 1 - ((jNow - In.Time.y) / decay);                                                       \n " +
                    "          if (dAlpha > 1 )                                                                                 \n" +
                    "          {                                                                                                \n" +
                    "               dAlpha = 1;                                                                                 \n" +
                    "          }                                                                                                \n" +
                    "     }                                                                                                     \n" +
                    "                                                                                                           \n" +
                                       "     Out.PointSize = float2(In.PointSize * scale, In.PointSize * scale);                          \n" +
                     //"     if ( scale > 0)                                                                                       \n" +
                    //"     {                                                                                                     \n" +
                    //"       Out.PointSize =  viewportScale * max(1.0, (scale * (In.PointSize )/ dist)) ; \n" +
                    //"     }                                                                                                     \n" +
                    //"     else                                                                                                  \n" +
                    //"     {                                                                                                     \n" +
                    //"       Out.PointSize = viewportScale * max(1.0,(-scale *In.PointSize ));                                  \n" +
                    //"     }                                                                                                     \n" +
                    "     Out.Color = In.Color * color;                                                                         \n" +
                    "     if (showFarSide == 0 && (dotCam * sky) < 0 || (jNow < In.Time.x && decay > 0))                        \n" +
                    "     {                                                                                                     \n" +
                    "        Out.Color.a = 0;                                                                                   \n" +
                    "     }                                                                                                     \n" +
                    "     else                                                                                                  \n" +
                    "     {                                                                                                     \n" +
                    "        Out.Color.a = In.Color.a * dAlpha * opacity;                                                       \n" +
                    "     }                                                                                                     \n" +
                    "     return Out;                                                                                           \n" +
                    " }      ";
        }



        protected override string GetGeometryShaderSource(string profile)
        {
            return
                GeometryInput +
                PixelInput +
                " float4x4 matWVP;                                                       \n" +
                " [maxvertexcount(20)]                                                    \n" +
                " void GS(point GS_IN In[1], inout TriangleStream<PS_IN> stream)         \n" +
                " {                                                                      \n" +
                "     float3 center = In[0].Center.xyz;                                  \n" +
                "     float3 centerNorm = normalize(center);                             \n" +
                "     float3 north = float3(0,1,0);                                      \n" +
                "     float3 right = cross(centerNorm, north);                           \n" +
                "     right = normalize(right);                                          \n" +
                "     float3 up = cross(centerNorm, right);                              \n" +
                "     float3 upleft = up - right;                                        \n" +
                "     upleft = normalize(upleft);                                        \n" +
                "     upleft = upleft * .0005 * In[0].PointSize.x;                                            \n" +
                "     float3 upright = up + right;                                       \n" +
                "     upright = normalize(upright);                                      \n" +
                "     upright = upright * .0005 * In[0].PointSize.x;                                          \n" +
                "                                                                        \n" +
                "     float4 t0 = mul(float4(center + upright,1), matWVP);               \n" +
                "     float4 t1 = mul(float4(center  + upleft,1), matWVP);               \n" +
                "     float4 t2 = mul(float4(center  - upleft,1), matWVP);               \n" +
                "     float4 t3 = mul(float4(center  - upright,1), matWVP);              \n" +
                "                                                                        \n" +
                "     float4 b0 =  mul(float4(normalize(center + upright),1), matWVP);   \n" +
                "     float4 b1 =  mul(float4(normalize(center  + upleft),1), matWVP);   \n" +
                "     float4 b2 =  mul(float4(normalize(center  - upleft),1), matWVP);   \n" +
                "     float4 b3 =  mul(float4(normalize(center  - upright),1), matWVP);  \n" +
                "     float alpha = In[0].Color.a;                                       \n" +
                "     PS_IN Out;                                                         \n" +

                //top
                "     Out.Color = In[0].Color;                                           \n" +
                "     Out.ProjPos = t0;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = t2;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = t1;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = t3;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     stream.RestartStrip();                                             \n" +

                //face0
                "     Out.Color = float4(In[0].Color.rgb*.9,alpha);                      \n" +
                "     Out.ProjPos = b0;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = t0;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = b1;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = t1;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     stream.RestartStrip();                                             \n" +

                //face1
                "     Out.Color = float4(In[0].Color.rgb*.7,alpha);                      \n" +
                "     Out.ProjPos = b1;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = t1;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = b3;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = t3;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     stream.RestartStrip();                                             \n" +

                //face2
                "     Out.Color = float4(In[0].Color.rgb*.9,alpha);                      \n" +
                "     Out.ProjPos = b3;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = t3;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = b2;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = t2;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     stream.RestartStrip();                                             \n" +

                //face3
                "     Out.Color = float4(In[0].Color.rgb*.7,alpha);                      \n" +
                "     Out.ProjPos = b2;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = t2;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = b0;                                                  \n" +
                "     stream.Append(Out);                                                \n" +
                "     Out.ProjPos = t0;                                                  \n" +
                "     stream.Append(Out);                                                \n" +


                " }                                                                      \n";
        }

        protected override string  GetPixelShaderSource(string profile)
        {
            return
                PixelInput +
                "                                              \n" +
                " float4 PS( PS_IN In ) : SV_Target            \n" +
                " {                                            \n" +
                "     return In.Color;                          \n" +
                " }                                            ";
        }

        static void initialize()
        {
            instance = new TimeSeriesColumnChartShader11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<TimeSeriesPointSpriteShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            geometryConstantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }

    public class TimeSeriesColumnChartShaderNGon11 : ShaderBundle
    {
        private static TimeSeriesColumnChartShaderNGon11 instance;
        private static SharpDX.Direct3D11.Buffer constantBuffer;
        private static SharpDX.Direct3D11.Buffer geometryConstantBuffer;
        public static TimeSeriesPointSpriteShaderConstants Constants;
        private static InputLayout layout;


        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }
        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }


        public static Color4 Color
        {
            set
            {
                Constants.Color = value;
            }
        }



        public static Vector2 ViewportScale
        {
            set
            {
                Constants.ViewportScale = value;
            }
        }

        public static Matrix WVPMatrix
        {
            set
            {
                Constants.WorldViewProjection = value;
            }
        }

        private static bool colChart = false;

        public static bool ColChart
        {
            set { colChart = value; }
        }

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float, 12, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 20, 0),

                    });
            }


            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(Shader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.GeometryShader.Set(instance.CompiledGeometryShader);

            context.GeometryShader.SetConstantBuffer(0, geometryConstantBuffer);

            context.UpdateSubresource(ref Constants.WorldViewProjection, geometryConstantBuffer);

            context.PixelShader.Set(instance.CompiledPixelShader);

            context.UpdateSubresource(ref Constants, constantBuffer);
        }

        static string VertexInput =
            " struct VS_IN                                 \n" +
            " {                                            \n" +
            "     float4 Position   : POSITION;            \n" +
            "     float PointSize   : POINTSIZE;           \n" +
            "     float4 Color      : COLOR;               \n" +
            "     float2 Time   : TEXCOORD0;              " + // Object Point size 
            " };                                           \n";

        static string GeometryInput =
            " struct GS_IN                                 \n" +
            " {                                            \n" +
            "     float4 Center     : POSITION;            \n" +
            "     float2 PointSize  : POINTSIZE;           \n" +
            "     float4 Color      : COLOR;               \n" +
            " };                                           \n";

        static string PixelInput =
            " struct PS_IN                                 \n" +
            " {                                            \n" +
            "     float4 ProjPos   : SV_POSITION;          \n" +
            "     float4 Color     : COLOR;                \n" +
            " };                                           \n";

        static string ConstantDeclarations =
            " float4x4 matWVP;                             \n" +
            " float4 color : COLOR;                        \n" +
            " float4 pointScaleFactors;                    \n" +
            " float4 camPos : POSITION;                    \n" +
            " float2 viewportScale;                        \n" +
            " float1 opacity;                              \n" +
            " float1 jNow;                                 \n" +
            " float1 decay;                                \n" +
            " float1 sky;                                  \n" +
            " float1 showFarSide;                          \n" +
            " float1 scale;                                \n";

        protected override string GetVertexShaderSource(string profile)
        {
            return
                    ConstantDeclarations +
                    VertexInput +
                    GeometryInput +

                    " GS_IN VS( VS_IN In )                                                                                      \n" +
                    " {                                                                                                         \n" +
                    "     GS_IN Out;                                                                                            \n" +
                     "     Out.Center = In.Position;                                                                                  \n" +
                    "     float dotCam = dot((camPos.xyz - Out.Center.xyz), Out.Center.xyz);                                  \n" +
                    "     float dist = distance(Out.Center.xyz, camPos.xyz);                                                       \n" +
                    "     float dAlpha = 1;                                                                                     \n" +
                    "     if ( decay > 0)                                                                                       \n" +
                    "     {                                                                                                     \n" +
                    "          dAlpha = 1 - ((jNow - In.Time.y) / decay);                                                       \n " +
                    "          if (dAlpha > 1 )                                                                                 \n" +
                    "          {                                                                                                \n" +
                    "               dAlpha = 1;                                                                                 \n" +
                    "          }                                                                                                \n" +
                    "     }                                                                                                     \n" +
                    "                                                                                                           \n" +
                    "     Out.PointSize = float2(In.PointSize * scale, In.PointSize * scale);                          \n" +
                    "     Out.Color = In.Color * color;                                                                         \n" +
                    "     if (showFarSide == 0 && (dotCam * sky) < 0 || (jNow < In.Time.x && decay > 0))                        \n" +
                    "     {                                                                                                     \n" +
                    "        Out.Color.a = 0;                                                                                   \n" +
                    "     }                                                                                                     \n" +
                    "     else                                                                                                  \n" +
                    "     {                                                                                                     \n" +
                    "        Out.Color.a = In.Color.a * dAlpha * opacity;                                                       \n" +
                    "     }                                                                                                     \n" +
                    "     return Out;                                                                                           \n" +
                    " }      ";
        }


        protected override string GetGeometryShaderSource(string profile)
        {
            return
                GeometryInput +
                PixelInput +
                " float4x4 matWVP;                                                       \n" +
                " [maxvertexcount(102)]                                                   \n" +
                " void GS(point GS_IN In[1], inout TriangleStream<PS_IN> stream)         \n" +
                " {                                                                      \n" +
                "     float3 center = In[0].Center.xyz;                                  \n" +
                "     float3 centerNorm = normalize(center);                             \n" +
                "     float3 north = float3(0,1,0);                                      \n" +
                "     float3 right = cross(centerNorm, north);                           \n" +
                "     right = normalize(right);                                          \n" +
                "     float3 up = cross(centerNorm, right);                              \n" +
                "     float rad = .0005 * In[0].PointSize.x;                             \n" +
                "     float4 cp = mul(float4(center,1), matWVP);                                                                   \n" +
                "     float4 cpn = mul(float4(centerNorm,1), matWVP);                                                                   \n" +
                "     float4 upp = mul(float4(center+up*rad, 1), matWVP);                    \n" +
                "     float4 rp = mul(float4(center+right*rad, 1), matWVP);                  \n" +
                "     float2 cp2 = cp.xy/cp.w;                                           \n" +
                "     float2 upp2 = upp.xy/upp.w;                                        \n" +
                "     float2 rp2 = rp.xy/rp.w;                                           \n" +
                "     float l1 = length(cp2 - upp2);                                     \n" +
                "     float l2 = length(cp2 - rp2);                                      \n" +
                "     float ml = max(l1,l2);                                             \n" +
                "     float alpha = In[0].Color.a;                                       \n" +
                "     PS_IN Out;                                                         \n" +
                "     float3 d = cross(centerNorm, up);                                  \n" +
                "     int N = max(5,min(ml*800,20));                                     \n" +
                "     if ((cp.w < 0 && cpn.w < 0) || alpha == 0) { N=0; }                                 \n" +
                "     float step = 6.283185 / max(1,N);                                         \n" +
                "     float angle = 0;                                                   \n" +
                "     for(int i = 0; i <= N; i++)                                        \n" +
                "     {                                                                  \n" +
                "       float3 t = rad*cos(angle)*up + rad*sin(angle) * d + center;      \n" +
                "       float3 b = normalize(t);                                         \n" +
                "       Out.Color = float4(In[0].Color.rgb* .4 + In[0].Color.rgb* sin(angle/2)*.5,alpha);         \n" +
                "       Out.ProjPos = mul(float4(b,1), matWVP);                          \n" +
                "       stream.Append(Out);                                              \n" +
                "       Out.ProjPos =  mul(float4(t,1), matWVP);                         \n" +
                "       stream.Append(Out);                                              \n" +
                "       angle += step;                                                   \n" +
                "     }                                                                  \n" +
                "                                                                        \n" +
                "     for( i = 0; i < N; i++)                                            \n" +
                "     {                                                                  \n" +
                "       float3 t = rad*cos(angle)*up + rad*sin(angle) * d + center;      \n" +
                "       float3 ta = rad*cos(angle+step)*up + rad*sin(angle+step) * d + center;       \n" +
                "       Out.Color = float4(In[0].Color.rgb,alpha);                       \n" +
            //    "       Out.Color = float4(In[0].Color.rgb * (1.0/N), alpha);            \n" +
                "       Out.ProjPos = mul(float4(ta,1), matWVP);                          \n" +
                "       stream.Append(Out);                                              \n" +
                "       Out.ProjPos =  mul(float4(t,1), matWVP);                         \n" +
                "       stream.Append(Out);                                              \n" +
                "       Out.ProjPos =  mul(float4(center,1), matWVP);                    \n" +
                "       stream.Append(Out);                                              \n" +
                "       stream.RestartStrip();                                           \n" +
                "                                                                        \n" +
                "                                                                        \n" +
                "                                                                        \n" +
                "       angle += step;                                                   \n" +
                "     }                                                                  \n" +
                " }                                                                      \n";
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                PixelInput +
                "                                              \n" +
                " float4 PS( PS_IN In ) : SV_Target            \n" +
                " {                                            \n" +
                "     return In.Color;                          \n" +
                " }                                            ";
        }

        static void initialize()
        {
            instance = new TimeSeriesColumnChartShaderNGon11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<TimeSeriesPointSpriteShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            geometryConstantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }
    }



    public class KeplerPointSpriteShader11 : ShaderBundle
    {
        private static KeplerPointSpriteShader11 instance;
        private static SharpDX.Direct3D11.Buffer constantBuffer;
        public static KeplerPointSpriteShaderConstants Constants;
        private static InputLayout layout;


        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static GeometryShader GeometryShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledGeometryShader;
            }
        }

     

        public static Matrix WVPMatrix
        {
            set
            {
                Constants.WorldViewProjection = value;
            }
        }


        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32_Float, 12, 0),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float, 24, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 28, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 32, 0),
                        new InputElement("TEXCOORD", 1, SharpDX.DXGI.Format.R32G32_Float, 40, 0),
                        new InputElement("TEXCOORD", 2, SharpDX.DXGI.Format.R32G32_Float, 48, 0),
                        new InputElement("TEXCOORD", 3, SharpDX.DXGI.Format.R32G32_Float, 56, 0),

                    });
            }

            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(instance.CompiledVertexShader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.GeometryShader.Set(instance.CompiledGeometryShader);
            context.PixelShader.Set(instance.CompiledPixelShader);

            context.UpdateSubresource(ref Constants, constantBuffer);
        }

        static string VertexInput =
            " struct VS_IN                                 \n" +
             " {                                             \n" +
            "     float4 ABC   : POSITION;                   \n" + // A,B,C 
            "     float4 abc   : NORMAL;                     \n" + // a,b,c 
            "     float1 PointSize   : POINTSIZE;            \n" + // AbsMag 
            "     float4 Color    : COLOR;                   \n" + // Vertex color                 
            "     float2 we   : TEXCOORD0;                   \n" + // w & e 
            "     float2 nT   : TEXCOORD1;                   \n" + // n & T 
            "     float2 az   : TEXCOORD2;                   \n" + // z 
            "     float2 orbit   : TEXCOORD3;                \n" + // orbit
            " };                                             \n";

        static string GeometryInput =
            " struct GS_IN                                 \n" +
            " {                                            \n" +
            "     float4 ProjPos     : POSITION;            \n" +
            "     float2 PointSize  : POINTSIZE;           \n" +
            "     float4 Color      : COLOR;               \n" +
            " };                                           \n";

        static string PixelInput =
            " struct PS_IN                                 \n" +
            " {                                            \n" +
            "     float4 ProjPos   : SV_POSITION;          \n" +
            "     float4 Color     : COLOR;                \n" +
            "     float2 TexCoord  : TEXCOORD0;            \n" +
            " };                                           \n";

        static string ConstantDeclarations =
            " float4x4 matWVP;                             \n" +
            " float4 camPos : POSITION;                    \n" +
            " float1 jNow;                                 \n" +
            " float2 scaling;                              \n" +
            " float1 opacity;                              \n" +
            " float1 MM;                                   \n" +
            " float2 viewportScaling;                      \n";

        protected override string  GetVertexShaderSource(string profile)
        {
            return
                    ConstantDeclarations +
                    VertexInput +
                    GeometryInput +
                    " GS_IN VS( VS_IN In )                                           \n" +
                    " {                                                                \n" +
                    "     GS_IN Out;                                                   \n" +
                    "     float M = In.nT.x * (jNow - In.nT.y) * 0.01745329251994;     \n" +
                    "     float e = In.we.y;                                           \n" +
                    "     float a = In.az.x;                                           \n" +
                    "     float PI = 3.1415926535897932384;                            \n" +
                    "     float w = In.we.x* 0.01745329251994;                         \n" +
                    "     float F = 1;                                                 \n" +
                    "     if (M < 0)                                                   \n" +
                    "       F = -1;                                                    \n" +
                    "     M = abs(M) / (2 * PI);                                       \n" +
                     "     M = (M - (int)(M))*2 *PI *F;                                \n" +
                    "     if (MM != 0)                                                 \n" +
                    "     {                                                            \n" +
                    "       M = MM + (1- In.orbit.x) *2 *PI;                           \n" +
                    "       if (M > (2*PI))                                            \n" +
                    "           M = M - (2*PI);                                        \n" +
                    "     }                                                            \n" +                 
                    "                                                                  \n" +
                    "     if (M < 0)                                                   \n" +
                    "       M += 2 *PI;                                                \n" +
                    "     F = 1;                                                       \n" +
                    "     if (M > PI)                                                  \n" +
                    "        F = -1;                                                   \n" +
                    "     if (M > PI)                                                  \n" +
                    "       M = 2 *PI - M;                                             \n" +
                    "                                                                  \n" +
                    "     float E = PI / 2;                                            \n" +
                    "     float scale = PI / 4;                                        \n" +
                    "     for (int i =0; i<23; i++)                                    \n" +
                    "     {                                                            \n" +
                    "       float R = E - e *sin(E);                                   \n" +
                    "       if (M > R)                                                 \n" +
                    "      	E += scale;                                                \n" +
                    "       else                                                       \n" +
                    "     	E -= scale;                                                \n" +
                    "       scale /= 2;                                                \n" +
                    "     }                                                            \n" +
                    "      E = E * F;                                                  \n" +

                    // Kepler's Equation; 5 Newton-Raphson steps will converge for eccentricity
                    // values of minor planets.
                    /*
                    "     float E = M;                                                 \n" +
                    "     for (int i = 0; i < 5; i++)                                  \n" +
                    "         E += (M - E + e * sin(E)) / (1 - e * cos(E));            \n" +
                    "     E = E * F;                                                   \n" +
                     */ 

                    "                                                                  \n" +
                    "     float v = 2 * atan(sqrt((1 + e) / (1 -e )) * tan(E/2));      \n" +
                    "     float r = a * (1-e * cos(E));                                \n" +
                    "                                                                  \n" +
                    "     float4 pnt;                                                  \n" +
                    "     pnt.x = r * In.abc.x * sin(In.ABC.x + w + v);                \n" +
                    "     pnt.z = r * In.abc.y * sin(In.ABC.y + w + v);                \n" +
                    "     pnt.y = r * In.abc.z * sin(In.ABC.z + w + v);                \n" +
                    "     pnt.w = 1;                                                   \n" +
                    "                                                                  \n" +
                    "     float dist = distance(pnt, camPos.xyz);                      \n" +
                    "     Out.ProjPos = mul(pnt,  matWVP );                            \n" + // Transform vertex into
                    "     Out.Color.a = opacity * (1-(In.orbit.x));                    \n" +
                    "     Out.Color.r = In.Color.r;                                    \n" +
                    "     Out.Color.g = In.Color.g;                                    \n" +
                    "     Out.Color.b = In.Color.b;                                    \n" +
                    "       Out.PointSize = viewportScaling * max(1.0, scaling.x * (In.PointSize / dist));   \n" +
                     //" if (Out.PointSize.x > 256)                                        \n" +
                     //" {                                                               \n" +
                     //"      Out.PointSize = 256;                                       \n" +
                     //" }                                                               \n" +
                     "     return Out;                                                 \n" + // Transfer color
                    " }                                                                \n";
        }

        protected override string GetGeometryShaderSource(string profile)
        {
            return
                GeometryInput +
                PixelInput +
                " [maxvertexcount(4)]                                                    \n" +
                " void GS(point GS_IN In[1], inout TriangleStream<PS_IN> stream)         \n" +
                " {                                                                      \n" +
                "     float4 center = In[0].ProjPos;                                      \n" +
                "     float2 halfSize = 0.5 * In[0].PointSize * center.w;                \n" +
                "     float4 p0 = center + float4( halfSize.x, -halfSize.y, 0.0f, 0.0f); \n" +
                "     float4 p1 = center + float4( halfSize.x,  halfSize.y, 0.0f, 0.0f); \n" +
                "     float4 p2 = center + float4(-halfSize.x, -halfSize.y, 0.0f, 0.0f); \n" +
                "     float4 p3 = center + float4(-halfSize.x,  halfSize.y, 0.0f, 0.0f); \n" +
                "     PS_IN Out;                               \n" +
                "     Out.Color = In[0].Color;                 \n" +
                "     Out.ProjPos = p0;                        \n" +
                "     Out.TexCoord = float2(1, 1);             \n" +
                "     stream.Append(Out);                      \n" +
                "     Out.ProjPos = p1;                        \n" +
                "     Out.TexCoord = float2(1, 0);             \n" +
                "     stream.Append(Out);                      \n" +
                "     Out.ProjPos = p2;                        \n" +
                "     Out.TexCoord = float2(0, 1);             \n" +
                "     stream.Append(Out);                      \n" +
                "     Out.ProjPos = p3;                        \n" +
                "     Out.TexCoord = float2(0, 0);             \n" +
                "     stream.Append(Out);                      \n" +
                " }                                            \n";
        }

        protected override string  GetPixelShaderSource(string profile)
        {
            return
                PixelInput +
                "Texture2D starTexture;                        \n" +
                "SamplerState starSampler;                     \n" +
                "                                              \n" +
                " float4 PS( PS_IN In ) : SV_Target            \n" +
                " {                                            \n" +
                "     return In.Color * starTexture.Sample(starSampler, In.TexCoord);\n" +
                " }                                            ";
        }

        static void initialize()
        {
            instance = new KeplerPointSpriteShader11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<KeplerPointSpriteShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }

    [StructLayout(LayoutKind.Explicit, Size = 112)]
    public struct DownlevelKeplerPointSpriteShaderConstants
    {
        [FieldOffset(0)]
        public Matrix WorldViewProjection;

        [FieldOffset(64)]
        public Vector4 CameraPosition;

        [FieldOffset(80)]
        public float JNow;

        [FieldOffset(84)]
        public Vector2 Scale;

        [FieldOffset(92)]
        public float Opacity;

        [FieldOffset(96)]
        public float MM;

        [FieldOffset(100)]
        public Vector2 ViewportScale;

    }


    [StructLayout(LayoutKind.Explicit, Size = 112)]
    public struct KeplerPointSpriteShaderConstants
    {
        [FieldOffset(0)]
        public Matrix WorldViewProjection;

        [FieldOffset(64)]
        public Vector4 CameraPosition;

        [FieldOffset(80)]
        public float JNow;

        [FieldOffset(84)]
        public Vector2 Scale;
     
        [FieldOffset(92)]
        public float Opacity;
        
        [FieldOffset(96)]
        public float MM;

        [FieldOffset(100)]
        public Vector2 ViewportScale;

    }

    public class DownlevelKeplerPointSpriteShader11 : ShaderBundle
    {
        private static DownlevelKeplerPointSpriteShader11 instance;
        private static SharpDX.Direct3D11.Buffer constantBuffer;
        public static KeplerPointSpriteShaderConstants Constants;
        private static InputLayout layout;
        private static InputLayout instancedLayout;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static Matrix WVPMatrix
        {
            set
            {
                Constants.WorldViewProjection = value;
            }
        }

        public static void Use(DeviceContext context, bool instanced)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("POSITION", 1, SharpDX.DXGI.Format.R32G32B32_Float, 12, 0),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float, 24, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 28, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 32, 0),
                        new InputElement("TEXCOORD", 1, SharpDX.DXGI.Format.R32G32_Float, 40, 0),
                        new InputElement("TEXCOORD", 2, SharpDX.DXGI.Format.R32G32_Float, 48, 0),
                        new InputElement("TEXCOORD", 3, SharpDX.DXGI.Format.R32G32_Float, 56, 0),
                        new InputElement("CORNER", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 64, 0),

                    });
            }

            if (instancedLayout == null)
            {
                // Layout from VertexShader input signature
                instancedLayout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("CORNER",    0, SharpDX.DXGI.Format.R8G8B8A8_UNorm,   0, 0, InputClassification.PerVertexData,   0),
                        new InputElement("POSITION",  0, SharpDX.DXGI.Format.R32G32B32_Float,  0, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("POSITION",  1, SharpDX.DXGI.Format.R32G32B32_Float, 12, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float,       24, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("COLOR",     0, SharpDX.DXGI.Format.R8G8B8A8_UNorm,  28, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("TEXCOORD",  0, SharpDX.DXGI.Format.R32G32_Float,    32, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("TEXCOORD",  1, SharpDX.DXGI.Format.R32G32_Float,    40, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("TEXCOORD",  2, SharpDX.DXGI.Format.R32G32_Float,    48, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("TEXCOORD",  3, SharpDX.DXGI.Format.R32G32_Float,    56, 1, InputClassification.PerInstanceData, 1)
                    });
            }


            if (instanced)
            {
                context.InputAssembler.InputLayout = instancedLayout;
            }
            else
            {
                context.InputAssembler.InputLayout = layout;
            }

            context.VertexShader.Set(instance.CompiledVertexShader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.PixelShader.Set(instance.CompiledPixelShader);
            context.UpdateSubresource(ref Constants, constantBuffer);
        }

        static string VertexInput =
            " struct VS_IN                                 \n" +
             " {                                             \n" +
            "     float3 ABC   : POSITION;                   \n" + // A,B,C 
            "     float3 abc   : POSITION1;                     \n" + // a,b,c 
            "     float1 PointSize   : POINTSIZE;            \n" + // AbsMag 
            "     float4 Color    : COLOR;                   \n" + // Vertex color                 
            "     float2 we   : TEXCOORD0;                   \n" + // w & e 
            "     float2 nT   : TEXCOORD1;                   \n" + // n & T 
            "     float2 az   : TEXCOORD2;                   \n" + // z 
            "     float2 orbit   : TEXCOORD3;                \n" + // orbit
            "     float4 Corner     : CORNER;                \n" +
            " };                                             \n";


        static string PixelInput =
            " struct PS_IN                                 \n" +
            " {                                            \n" +
            "     float4 ProjPos   : SV_POSITION;          \n" +
            "     float4 Color     : COLOR;                \n" +
            "     float2 TexCoord  : TEXCOORD0;            \n" +
            " };                                           \n";

        static string ConstantDeclarations =
            " float4x4 matWVP;                             \n" +
            " float4 camPos           ;                    \n" +
            " float1 jNow;                                 \n" +
            " float2 scaling;                              \n" +
            " float1 opacity;                              \n" +
            " float1 MM;                                   \n" +
            " float2 viewportScaling;                      \n";

            

        protected override string  GetVertexShaderSource(string profile)
        {
            return
                    ConstantDeclarations +
                    VertexInput +
                    PixelInput + 
                    " PS_IN VS( VS_IN In )                                           \n" +
                    " {                                                                \n" +
                    "     PS_IN Out;                                                   \n" +
                    "     float M = In.nT.x * (jNow - In.nT.y) * 0.01745329251994;     \n" +
                    "     float e = In.we.y;                                           \n" +
                    "     float a = In.az.x;                                           \n" +
                    "     float PI = 3.1415926535897932384;                            \n" +
                    "     float w = In.we.x* 0.01745329251994;                         \n" +
                    "     float F = 1;                                                 \n" +
                    "     if (M < 0)                                                   \n" +
                    "       F = -1;                                                    \n" +
                    "     M = abs(M) / (2 * PI);                                       \n" +
                     "     M = (M - (int)(M))*2 *PI *F;                                \n" +
                    "     if (MM != 0)                                                 \n" +
                    "     {                                                            \n" +
                    "       M = MM + (1- In.orbit.x) *2 *PI;                           \n" +
                    "       if (M > (2*PI))                                            \n" +
                    "           M = M - (2*PI);                                        \n" +
                    "     }                                                            \n" +
                    "                                                                  \n" +
                    "     if (M < 0)                                                   \n" +
                    "       M += 2 *PI;                                                \n" +
                    "     F = 1;                                                       \n" +
                    "     if (M > PI)                                                  \n" +
                    "        F = -1;                                                   \n" +
                    "     if (M > PI)                                                  \n" +
                    "       M = 2 *PI - M;                                             \n" +
                    "                                                                  \n" +
                    "     float E = M;                                                 \n" +
                    "     E += (M - E + e * sin(E)) / (1 - e * cos(E));                \n" +
                    "     E += (M - E + e * sin(E)) / (1 - e * cos(E));                \n" +
                    "     E += (M - E + e * sin(E)) / (1 - e * cos(E));                \n" +
                    "     E += (M - E + e * sin(E)) / (1 - e * cos(E));                \n" +
                    "     E += (M - E + e * sin(E)) / (1 - e * cos(E));                \n" +

                // Loop does not work correctly on downlevel (9.3) devices; replaced with
                // unrolled Newton-Raphson iteration above.
                /*
                "     float E = PI / 2;                                            \n" +
                "     float scale = PI / 4;                                        \n" +
                "     for (int i =0; i<3; i++)                                    \n" +
                "     {                                                            \n" +
                "       float R = E - e *sin(E);                                   \n" +
                "       if (M > R)                                                 \n" +
                "      	E += scale;                                                \n" +
                "       else                                                       \n" +
                "     	E -= scale;                                                \n" +
                "       scale /= 2;                                                \n" +
                "     }                                                            \n" +
                 */

                    "      E = E * F;                                                  \n" +
                    "                                                                  \n" +
                    "     float v = 2 * atan(sqrt((1 + e) / (1 -e )) * tan(E/2));      \n" +
                    "     float r = a * (1-e * cos(E));                                \n" +
                    "                                                                  \n" +
                    "     float4 pnt;                                                  \n" +
                    "     pnt.x = r * In.abc.x * sin(In.ABC.x + w + v);                \n" +
                    "     pnt.z = r * In.abc.y * sin(In.ABC.y + w + v);                \n" +
                    "     pnt.y = r * In.abc.z * sin(In.ABC.z + w + v);                \n" +
                    "     pnt.w = 1;                                                   \n" +
                    "                                                                  \n" +
                    "     float dist = distance(pnt.xyz, camPos.xyz);                      \n" +
                    "     float4 center  = mul(pnt,  matWVP );                            \n" + // Transform vertex into
                    "     Out.Color.a = opacity * (1-(In.orbit.x));                    \n" +
                    "     Out.Color.r = In.Color.r;                                    \n" +
                    "     Out.Color.g = In.Color.g;                                    \n" +
                    "     Out.Color.b = In.Color.b;                                    \n" +
                    "     float2 pointSize = viewportScaling * max(1.0, (scaling.x * (In.PointSize )/ dist));                   \n" +
                    "     Out.TexCoord = In.Corner.zw;                                                                          \n" +
                    "     float2 offset = (pointSize * center.w) * (In.Corner.xy - 0.5);                                        \n" +                   
                    "     Out.ProjPos = center + float4( offset.x, offset.y, 0.0f, 0.0f);     \n" +  
                    "     return Out;                                                 \n" + // Transfer color
                    " }                                                                \n";

        }

       

        protected override string GetPixelShaderSource(string profile)
        {
            return
                PixelInput +
                "Texture2D starTexture;                        \n" +
                "SamplerState starSampler;                     \n" +
                "                                              \n" +
                " float4 PS( PS_IN In ) : SV_Target            \n" +
                " {                                            \n" +
                "     return In.Color * starTexture.Sample(starSampler, In.TexCoord);\n" +
                " }                                            ";
        }

        static void initialize()
        {
            instance = new DownlevelKeplerPointSpriteShader11();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<DownlevelKeplerPointSpriteShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }
    }


    public class CompatibilityPointSpriteShader : ShaderBundle
    {
        private static CompatibilityPointSpriteShader instance;
        private static SharpDX.Direct3D11.Buffer constantBuffer;

        public struct Vertex
        {
            public float X;
            public float Y;
            public float Z;
            public uint color;
            public float size;
            public uint corner;

            public SysColor Color
            {
                get
                {
                    return SysColor.FromArgb((byte)(color >> 24), (byte)color, (byte)(color >> 8), (byte)(color >> 16));
                }
                set
                {
                    color = (uint)(((uint)value.A) << 24) | (((uint)value.B) << 16) | (((uint)value.G) << 8) | (((uint)value.R));
                }
            }   
        }

        public struct CornerVertex
        {
            public uint corner;
        }

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static Color4 Color
        {
            set
            {
                constants.Color = value;
            }
        }

        // PointScaleFactors is a vector (A, B, C)
        // The point scale is computed as sqrt(A + B/d + C/d^2) where d is
        // the distance to the vertex in camera space.
        public static Vector3 PointScaleFactors
        {
            set
            {
                constants.PointScaleFactors = new Vector4(value.X, value.Y, value.Z, 0.0f);
            }
        }

        public static Vector2 ViewportScale
        {
            set
            {
                constants.ViewportScale = value;
            }
        }

        public static Matrix WVPMatrix
        {
            set
            {
                constants.WorldViewProjection = value;
            }
        }

        public static float MinPointSize
        {
            set
            {
                constants.MinPointSize = value;
            }
        }

        private static PointSpriteShaderConstants constants;
        private static InputLayout layout;
        private static InputLayout instancedLayout;

        public static void Use(DeviceContext context, bool instanced)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 12, 0),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float, 16, 0),
                        new InputElement("CORNER", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 20, 0)
                    });
            }

            if (instancedLayout == null)
            {
                instancedLayout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("CORNER",    0, SharpDX.DXGI.Format.R8G8B8A8_UNorm,   0, 0, InputClassification.PerVertexData,   0),
                        new InputElement("POSITION",  0, SharpDX.DXGI.Format.R32G32B32_Float,  0, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("COLOR",     0, SharpDX.DXGI.Format.R8G8B8A8_UNorm,  12, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float,       16, 1, InputClassification.PerInstanceData, 1)
                    });
            }

            if (instanced)
            {
                context.InputAssembler.InputLayout = instancedLayout;
            }
            else
            {
                context.InputAssembler.InputLayout = layout;
            }

            context.VertexShader.Set(instance.CompiledVertexShader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.GeometryShader.Set(null);
            context.PixelShader.Set(instance.CompiledPixelShader);

            context.UpdateSubresource(ref constants, constantBuffer);
        }

        // The offset from center and the texture coordinate at the corner
        // are packed into the attribute Corner. The offset is in xy, the texture
        // coordinate in zw. These values are either 0, 1, or -1 so 8-bit precision
        // is adequate

        static string VertexInput =
            " struct VS_IN                                 \n" +
            " {                                            \n" +
            "     float4 Position   : POSITION;            \n" +
            "     float4 Color      : COLOR;               \n" +
            "     float PointSize   : POINTSIZE;           \n" +
            "     float4 Corner     : CORNER;              \n" +
            " };                                           \n";

        static string PixelInput =
            " struct PS_IN                                 \n" +
            " {                                            \n" +
            "     float4 ProjPos   : SV_POSITION;          \n" +
            "     float4 Color     : COLOR;                \n" +
            "     float2 TexCoord  : TEXCOORD0;            \n" +
            " };                                           \n";

        static string ConstantDeclarations =
            " float4x4 matWVP;                             \n" +
            " float4 color : COLOR;                        \n" +
            " float4 pointScaleFactors;                    \n" +
            " float2 viewportScale;                        \n" +
            " float minPointSize;                          \n";

        protected override string GetVertexShaderSource(string profile)
        {
            return
                    ConstantDeclarations +
                    VertexInput +
                    PixelInput +
                    "                                                 \n" +
                    " PS_IN VS( VS_IN In )                            \n" +
                    " {                                               \n" +
                    "     PS_IN Out;                                  \n" +
                    "     float4 center = mul(In.Position, matWVP);   \n" +
                    "     float d = 1.0 / length(center.xyz);         \n" +
                    "     float pointScale = sqrt(pointScaleFactors.x + pointScaleFactors.y * d + pointScaleFactors.z * d * d); \n" +
                    "     float2 pointSize = max(minPointSize, In.PointSize * pointScale) * viewportScale;                               \n" +
                    "     float2 offset = (pointSize * center.w) * (In.Corner.xy  - 0.5);                                       \n" +
                    "     Out.Color = In.Color * color;                                                                         \n" +
                    "     Out.TexCoord = In.Corner.zw;                                                                          \n" +
                    "     Out.ProjPos = center + float4( offset.x, offset.y, 0.0f, 0.0f);                                       \n" +
                    "     return Out;                                 \n" +
                    " }                                               \n";
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                PixelInput +
                "Texture2D spriteTexture;                      \n" +
                "SamplerState spriteSampler;                   \n" +
                "                                              \n" +
                " float4 PS( PS_IN In ) : SV_Target            \n" +
                " {                                            \n" +
                "     return In.Color * spriteTexture.Sample(spriteSampler, In.TexCoord);\n" +
                " }                                            ";
        }

        static void initialize()
        {
            instance = new CompatibilityPointSpriteShader();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<PointSpriteShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

    }


    public class DownlevelTimeSeriesPointSpriteShader : TimeSeriesPointSpriteShaderBase
    {
        private static DownlevelTimeSeriesPointSpriteShader instance;
        private static InputLayout layout;
        private static InputLayout instancedLayout;

        public struct Vertex
        {
            public Vector3 Position;
            public float PointSize;
            public uint color;
            public SysColor Color
            {
                get
                {
                    return SysColor.FromArgb((byte)(color >> 24), (byte)color, (byte)(color >> 8), (byte)(color >> 16));
                }
                set
                {
                    color = (uint)(((uint)value.A) << 24) | (((uint)value.B) << 16) | (((uint)value.G) << 8) | (((uint)value.R));
                }
            }
            public float Tu;
            public float Tv;
            public Vertex(Vector3 position, float size, float time, uint color)
            {
                Position = position;
                PointSize = size;
                Tu = time;
                Tv = 0;
                corner = 0;
                this.color = color;
            }

            public uint corner;
        }


        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        public static GeometryShader GeometryShader
        {
            get
            {
                return null;
            }
        }


        public static void Use(DeviceContext context, bool instanced)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float, 12, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 16, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 20, 0),
                        new InputElement("CORNER", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, 28, 0),
                    });
            }

            if (instancedLayout == null)
            {
                instancedLayout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("CORNER",    0, SharpDX.DXGI.Format.R8G8B8A8_UNorm,   0, 0, InputClassification.PerVertexData,   0),
                        new InputElement("POSITION",  0, SharpDX.DXGI.Format.R32G32B32_Float,  0, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("POINTSIZE", 0, SharpDX.DXGI.Format.R32_Float,       12, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("COLOR",     0, SharpDX.DXGI.Format.R8G8B8A8_UNorm,  16, 1, InputClassification.PerInstanceData, 1),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float,     20, 1, InputClassification.PerInstanceData, 1),
                    });
            }
            if (instanced)
            {
                context.InputAssembler.InputLayout = instancedLayout;
            }
            else
            {
                context.InputAssembler.InputLayout = layout;
            }

            context.VertexShader.Set(instance.CompiledVertexShader);
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.GeometryShader.Set(null);

            context.PixelShader.Set(instance.CompiledPixelShader);

            context.UpdateSubresource(ref Constants, constantBuffer);
        }

        static string VertexInput =
            " struct VS_IN                                 \n" +
            " {                                            \n" +
            "     float4 Position   : POSITION;            \n" +
            "     float PointSize   : POINTSIZE;           \n" +
            "     float4 Color      : COLOR;               \n" +
            "     float2 Time       : TEXCOORD0;           \n" +
            "     float4 Corner     : CORNER;              \n" +
            " };                                           \n";

        static string PixelInput =
            " struct PS_IN                                 \n" +
            " {                                            \n" +
            "     float4 ProjPos   : SV_POSITION;          \n" +
            "     float4 Color     : COLOR;                \n" +
            "     float2 TexCoord  : TEXCOORD0;            \n" +
            " };                                           \n";

        static string ConstantDeclarations =
            " float4x4 matWVP;                             \n" +
            " float4 color : COLOR;                        \n" +
            " float4 pointScaleFactors;                    \n" +
            " float4 camPos : POSITION;                    \n" +
            " float2 viewportScale;                        \n" +
            " float1 opacity;                              \n" +
            " float1 jNow;                                 \n" +
            " float1 decay;                                \n" +
            " float1 sky;                                  \n" +
            " float1 showFarSide;                          \n" +
            " float1 scale;                                \n";

        protected override string GetVertexShaderSource(string profile)
        {
            return
                    ConstantDeclarations +
                    VertexInput +
                    PixelInput +
                    "                                                                                                           \n" +
                    " PS_IN VS( VS_IN In )                                                                                      \n" +
                    " {                                                                                                         \n" +
                    "     PS_IN Out;                                                                                            \n" +
                    "     float dotCam = dot((camPos.xyz - In.Position.xyz), In.Position.xyz);                                  \n" +
                    "     float dist = distance(In.Position.xyz, camPos.xyz);                                                       \n" +
                    "     float dAlpha = 1;                                                                                     \n" +
                    "     if ( decay > 0)                                                                                       \n" +
                    "     {                                                                                                     \n" +
                    "          dAlpha = 1 - ((jNow - In.Time.y) / decay);                                                       \n " +
                    "          if (dAlpha > 1 )                                                                                 \n" +
                    "          {                                                                                                \n" +
                    "               dAlpha = 1;                                                                                 \n" +
                    "          }                                                                                                \n" +
                    "     }                                                                                                     \n" +
                    "                                                                                                           \n" +
                    "     float4 center = mul(In.Position, matWVP);                                                             \n" +
                    "     float d = 1.0 / length(center.xyz);                                                                   \n" +
                    "     float pointScale = sqrt(pointScaleFactors.x + pointScaleFactors.y * d + pointScaleFactors.z * d * d); \n" +
                    "     float2 pointSize = max(1.0, In.PointSize * pointScale * scale) * viewportScale;                       \n" +
                    "     if ( scale > 0)                                                                                       \n" +
                    "     {                                                                                                     \n" +
                    "       pointSize =  viewportScale * max(1.0, (scale * (In.PointSize )/ dist));                             \n" +
                    "     }                                                                                                     \n" +
                    "     else                                                                                                  \n" +
                    "     {                                                                                                     \n" +
                    "       pointSize = viewportScale * max(1.0,(-scale *In.PointSize ));                                       \n" +
                    "     }                                                                                                     \n" +
                    "     Out.Color = In.Color * color;                                                                         \n" +
                    "     if (showFarSide == 0 && (dotCam * sky) < 0 || (jNow < In.Time.x && decay > 0))                        \n" +
                    "     {                                                                                                     \n" +
                    "        Out.Color.a = 0;                                                                                   \n" +
                    "     }                                                                                                     \n" +
                    "     else                                                                                                  \n" +
                    "     {                                                                                                     \n" +
                    "        Out.Color.a = In.Color.a * dAlpha * opacity;                                                       \n" +
                    "     }                                                                                                     \n" +
                    "     Out.TexCoord = In.Corner.zw;                                                                          \n" +
                    "     float2 offset = (pointSize * center.w) * (In.Corner.xy  - 0.5);                                       \n" +
                    "     Out.ProjPos = center + float4( offset.x, offset.y, 0.0f, 0.0f);                                       \n" +
                    "     return Out;                                                                                           \n" +
                    " }                                                                                                         \n";
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                PixelInput +
                "Texture2D starTexture;                        \n" +
                "SamplerState starSampler;                     \n" +
                "                                              \n" +
                " float4 PS( PS_IN In ) : SV_TARGET            \n" +
                " {                                            \n" +
                "     return In.Color * starTexture.Sample(starSampler, In.TexCoord);\n" +
                " }                                            ";
        }

        static void initialize()
        {
            instance = new DownlevelTimeSeriesPointSpriteShader();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
            constantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<TimeSeriesPointSpriteShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }
    }


    public class HDRPixelShader
    {
        protected static ShaderBytecode pixelShaderByteCode;
        protected static PixelShader pixelShader;

    
        public static PixelShader PixelShader
        {
            get
            {
                // All LineShader* classes use the same very simple pixel shader
                return pixelShader;
            }
        }

        public static HDRShaderConstants constants;
     
        public static void Use(DeviceContext context)
        {

            if (PixelShader == null)
            {
                MakePixelShader();
            }


            if (contantBuffer == null)
            {
                contantBuffer = new SharpDX.Direct3D11.Buffer(RenderContext11.PrepDevice, Utilities.SizeOf<HDRShaderConstants>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            }
            context.PixelShader.Set(pixelShader);
          
            context.PixelShader.SetConstantBuffer(0, contantBuffer);

            context.UpdateSubresource(ref constants, contantBuffer);

        }


        static string ConstantDeclarations =
                   " float4 tint : COLOR;       \n" +     
                   " float1 a;                  \n" +
                   " float1 b;                  \n" +
                   " float1 c;                  \n" +
                   " float1 d;                  \n" +
                   " float1 opacity;            \n";
 
       
        private static void MakePixelShader()
        {
            string shaderText =
                ConstantDeclarations +
             

                "Texture2D spriteTexture;                      \n" +
                "SamplerState spriteSampler;                   \n" +
                " struct VS_OUT                                \n" +
                " {                                            \n" +
                "	    float4 pos : SV_POSITION;               \n" +
                "	    float2 tex : TEXCOORD;                  \n" +
                "	    float4 color : COLOR;                   \n" +
                " };                                           \n" +
                "                                              \n" +
                " float4 PS(VS_OUT In) : SV_Target               \n" +
                " {                                            \n" +
                "   float4 col = spriteTexture.Sample(spriteSampler, In.tex); \n " + 
                "   float4 color;                              \n" +
                "   float1 diff = max(2*b, 0.01); //      put a floor on this \n" +
                "   float1 slope = 1/diff;                                    \n" +

                "   color.r =  max( min( (col.r-(a-b))*slope, 1), 0);                  \n" +
                "   color.g =  max( min( (col.g-(a-b))*slope, 1), 0);                  \n" +
                "   color.b = max( min( (col.b-(a-b))*slope, 1), 0);                   \n" +
                "   color.a = col.a * opacity;                                         \n" +
                "   return color * tint;                                                      \n" +
                " }                                            ";

            pixelShaderByteCode = ShaderBytecode.Compile(shaderText, "PS", RenderContext11.PixelProfile);
            pixelShader = new PixelShader(RenderContext11.PrepDevice, pixelShaderByteCode);

        }

        static SharpDX.Direct3D11.Buffer contantBuffer;

        

    }



    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct HDRShaderConstants
    {
        [FieldOffset(0)]
        public Color4 tint;
        [FieldOffset(16)]
        public float a;
        [FieldOffset(20)]
        public float b;
        [FieldOffset(24)]
        public float c;
        [FieldOffset(28)]
        public float d;
        [FieldOffset(32)]
        public float opacity;
        
    }


    public class SimpleShader : ShaderBundle
    {
        private static SimpleShader instance;

        public static VertexShader Shader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledVertexShader;
            }

        }

        public static PixelShader PixelShader
        {
            get
            {
                if (instance == null)
                {
                    initialize();
                }
                return instance.CompiledPixelShader;
            }
        }

        private static InputLayout layout;

        public static void Use(DeviceContext context)
        {
            if (instance == null)
            {
                initialize();
            }

            if (layout == null)
            {
                // Layout from VertexShader input signature
                layout = new InputLayout(RenderContext11.PrepDevice, instance.VertexShaderBytecode, new[]
                    {
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 16, 0),
                   });
            }


            context.InputAssembler.InputLayout = layout;

            context.VertexShader.Set(instance.CompiledVertexShader);

            context.PixelShader.Set(instance.CompiledPixelShader);

        
        }

        protected override string GetPixelShaderSource(string profile)
        {
            return
                "struct PS_IN                                \n" +
                    "{                                          \n" +
                    "	float4 pos : SV_POSITION;               \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "};                                             \n" +
                "    \n" +
                " Texture2D picture;     \n" +
                " SamplerState pictureSampler;   \n" +
                "    \n" +
                "    \n" +
                "    \n" +
                " float4 PS( PS_IN input ) : SV_Target   \n" +
                " {     \n" +
                " 	return picture.Sample(pictureSampler, input.tex);   \n" +
                //" 	return float4(1,1,1,0);   \n" +
                " }   \n" +
                "    ";

        }


        protected override string GetVertexShaderSource(string profile)
        {

            return
                    "struct VS_IN                               \n" +
                    "{                                          \n" +
                    "	float4 pos : POSITION;                  \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "};                                         \n" +
                    "                                           \n" +
                    "struct PS_IN                                \n" +
                    "{                                          \n" +
                    "	float4 pos : SV_POSITION;               \n" +
                    "	float2 tex : TEXCOORD;                  \n" +
                    "};                                             \n" +
                    "                                           \n" +
                    "PS_IN VS( VS_IN input )                    \n" +
                    "{                                          \n " +
                    "	PS_IN output = (PS_IN)0;                \n" +
                    "	  " +
                    "   output.pos = input.pos;  " +
                    "	output.tex = input.tex;                     \n" +
                    "	return output;                          \n" +
                   
                    "}                                          \n" +
                    "                                           \n" +
                    "                                           \n";
        }

        static void initialize()
        {
            instance = new SimpleShader();
            instance.CompileShader(RenderContext11.VertexProfile, RenderContext11.PixelProfile);
        }

    }


}
