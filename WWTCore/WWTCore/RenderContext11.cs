using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using System.IO;


namespace TerraViewer
{
    public enum BlendMode
    {
        None,
        Alpha,
        Additive,
        PremultipliedAlpha,
        BlendFactorInverseAlpha,
        NoColorWrite
    };

    public enum DepthStencilMode
    {
        Off,
        ZReadOnly,
        ZWriteOnly,
        ZReadWrite
    };

    public enum TriangleCullMode
    {
        Off,
        CullClockwise,
        CullCounterClockwise
    };

    public struct Material
    {
        public System.Drawing.Color Diffuse;
        public System.Drawing.Color Ambient;
        public System.Drawing.Color Specular;
        public float SpecularSharpness;
        public float Opacity;
        public bool Default;
    }

    public class RenderContext11 : IDisposable
    {
        public static int MultiSampleCount = 4;
        public static SharpDX.DXGI.Format DefaultDepthStencilFormat = Format.D24_UNorm_S8_UInt;

        SwapChainDescription desc;
        private Device device;

        public static Device PrepDevice = null;

        public Device Device
        {
            get { return device; }
            set
            {
                device = value;
                PrepDevice = value;
                devContext = device.ImmediateContext;
            }
        }

        public DeviceContext devContext;
        SwapChain swapChain;
        Factory factory;
        Texture2D backBuffer;

        public Texture2D BackBuffer
        {
            get { return backBuffer; }
            set { backBuffer = value; }
        }
        RenderTargetView renderView;
        SamplerState sampler;

        SamplerState wrapSampler;
        SamplerState clampSampler;
        SamplerState transparentBorderSampler;

        Texture2D depthBuffer;
        DepthStencilView depthView;

        private BlendMode currentBlendMode = BlendMode.None;
        private SharpDX.Direct3D11.BlendState[] standardBlendStates;
        private DepthStencilMode currentDepthStencilMode = DepthStencilMode.Off;
        private DepthStencilState[] standardDepthStencilStates;
        private TriangleCullMode currentCullMode = TriangleCullMode.CullClockwise;
        private bool currentMultisampleState = true;
        private RasterizerState[] standardRasterizerStates;
        private ViewportF viewPort;

        public ViewportF ViewPort
        {
            get { return viewPort; }
            set
            {
                viewPort = value;
                devContext.Rasterizer.SetViewport(viewPort);
            }
        }

        public Viewport DisplayViewport
        {
            get { return displayViewPort; }
        }

        public double PerspectiveFov = Math.PI / 4;

        public static string VertexProfile = "vs_4_0";
        public static string PixelProfile = "ps_4_0";
        public static bool Downlevel = false;
        public static bool sRGB = false;

        public static SharpDX.DXGI.Format DefaultColorFormat
        {
            get { return sRGB ? Format.R8G8B8A8_UNorm_SRgb : Format.R8G8B8A8_UNorm; }
        }

        public static SharpDX.DXGI.Format DefaultTextureFormat
        {
            get { return sRGB ? Format.R8G8B8A8_UNorm_SRgb : Format.R8G8B8A8_UNorm; }
        }

        public RenderContext11(System.Windows.Forms.Control control, bool forceNoSRGB = false)
        {

            bool failed = true;
            while (failed)
            {
                try
                {
                    // SwapChain description
                    desc = new SwapChainDescription()
                    {
                        BufferCount = 1,
                        ModeDescription =
                            new ModeDescription(control.ClientSize.Width, control.ClientSize.Height,
                                                new Rational(60, 1), Format.R8G8B8A8_UNorm),
                        IsWindowed = true,
                        OutputHandle = control.Handle,
                        SampleDescription = new SampleDescription(MultiSampleCount, 0),
                        SwapEffect = SwapEffect.Discard,
                        Usage = Usage.RenderTargetOutput
                    };

                    FeatureLevel[] featureLevels = { FeatureLevel.Level_11_0, FeatureLevel.Level_10_1, FeatureLevel.Level_10_0, FeatureLevel.Level_9_3 };

                    //Enable this instead for downlevel testing.
                    //featureLevels = new FeatureLevel[] {  FeatureLevel.Level_9_3 };

                    // Create Device and SwapChain
                    Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, featureLevels, desc, out device, out swapChain);
                    failed = false;
                }
                catch
                {
                    if (MultiSampleCount != 1)
                    {
                        MultiSampleCount = 1;
                        AppSettings.SettingsBase["MultiSampling"] = 1;
                    }
                    else
                    {
                        throw new System.Exception("DX Init failed");
                    }
                    failed = true;
                }
            } 

            devContext = device.ImmediateContext;

            PrepDevice = device;


            if(device.FeatureLevel == FeatureLevel.Level_9_3)
            {
                PixelProfile = "ps_4_0_level_9_3";
                VertexProfile = "vs_4_0_level_9_3";
                Downlevel = true;
            }
            else if (device.FeatureLevel == FeatureLevel.Level_9_1)
            {
                PixelProfile = "ps_4_0_level_9_1";
                VertexProfile = "vs_4_0_level_9_1";
                Downlevel = true;
            }

            if (!Downlevel)
            {
                if (!forceNoSRGB)
                {
                    sRGB = true;
                }
                //dv1 = device.QueryInterface<Device1>();
                //dv1.MaximumFrameLatency = 1;
            }

            // Ignore all windows events
            factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(control.Handle, WindowAssociationFlags.IgnoreAll);

            // New RenderTargetView from the backbuffer
            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderView = new RenderTargetView(device, backBuffer);

            // Create Depth Buffer & View
            depthBuffer = new Texture2D(device, new Texture2DDescription()
            {
                Format = DefaultDepthStencilFormat,
                ArraySize = 1,
                MipLevels = 1,
                Width = control.ClientSize.Width,
                Height = control.ClientSize.Height,
                SampleDescription = new SampleDescription(MultiSampleCount, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            depthView = new DepthStencilView(device, depthBuffer);

            if (Downlevel)
            {
                sampler = new SamplerState(device, new SamplerStateDescription()
                {
                    Filter = Filter.Anisotropic,
                    AddressU = TextureAddressMode.Clamp,
                    AddressV = TextureAddressMode.Clamp,
                    AddressW = TextureAddressMode.Wrap,
                    BorderColor = SharpDX.Color.Black,
                    ComparisonFunction = Comparison.Never,
                    MaximumAnisotropy = 16,
                    MipLodBias = 0,
                    MinimumLod = 0,
                    MaximumLod = float.MaxValue,
                });
            }
            else
            {
                sampler = new SamplerState(device, new SamplerStateDescription()
                {
                    Filter = Filter.Anisotropic,
                    AddressU = TextureAddressMode.Clamp,
                    AddressV = TextureAddressMode.Clamp,
                    AddressW = TextureAddressMode.Wrap,
                    BorderColor = SharpDX.Color.Black,
                    ComparisonFunction = Comparison.Never,
                    MaximumAnisotropy = 16,
                    MipLodBias = 0,
                    MinimumLod = 0,
                    MaximumLod = 16,
                });
            }

            devContext.PixelShader.SetSampler(0, sampler);
            // Prepare All the stages
            displayViewPort = new Viewport(0, 0, control.ClientSize.Width, control.ClientSize.Height, 0.0f, 1.0f);
            ViewPort = displayViewPort;
            devContext.OutputMerger.SetTargets(depthView, renderView);

            initializeStates();
        }

        Device1 dv1 = null;

        public void SetLatency(int frames)
        {
            if (dv1 != null)
            {
                dv1.MaximumFrameLatency = frames;
            }
        }

        private Viewport displayViewPort;

        public void SetDisplayRenderTargets()
        {
            devContext.OutputMerger.ResetTargets();
            ViewPort = displayViewPort;
            devContext.OutputMerger.SetTargets(depthView, renderView);
            currentTargetView = renderView;
            currentDepthView = depthView;

        }
        RenderTargetView currentTargetView;
        DepthStencilView currentDepthView;


        // Return true if this vertex instancing is supported
        public static bool SupportsInstancing
        {
            get
            {
                return PrepDevice.FeatureLevel >= FeatureLevel.Level_9_3;
            }
        }



        public void SetOffscreenRenderTargets(RenderTargetTexture targetTexture, DepthBuffer depthBuffer)
        {
            currentTargetView = targetTexture.renderView;

            if (depthBuffer != null)
            {
                currentDepthView = depthBuffer.DepthView;
            }
            else
            {
                currentDepthView = null;
            }

            devContext.OutputMerger.ResetTargets();
            ViewPort = new Viewport(0, 0, targetTexture.Width, targetTexture.Height, 0.0f, 1.0f);

            if (depthBuffer != null)
            {
                devContext.OutputMerger.SetTargets(depthBuffer.DepthView, targetTexture.renderView);
            }
            else
            {
                devContext.OutputMerger.SetTargets(targetTexture.renderView);

            }

        }

        public void SetOffscreenRenderTargets(RenderTargetView targetTextureView, DepthStencilView depthBufferView, int width, int height)
        {
            currentTargetView = targetTextureView;

            if (depthBuffer != null)
            {
                currentDepthView = depthBufferView;
            }
            else
            {
                currentDepthView = null;
            }

            devContext.OutputMerger.ResetTargets();
            ViewPort = new Viewport(0, 0, width, height, 0.0f, 1.0f);

            if (depthBuffer != null)
            {
                devContext.OutputMerger.SetTargets(depthBufferView, targetTextureView);
            }
            else
            {
                devContext.OutputMerger.SetTargets(targetTextureView);

            }

        }

        public Bitmap GetScreenBitmap()
        {
            MemoryStream ms = new MemoryStream();

            if (MultiSampleCount != 1)
            {

                Texture2D tex2 = new Texture2D(RenderContext11.PrepDevice, new Texture2DDescription()
                {
                    Format = RenderContext11.DefaultColorFormat,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = (int)ViewPort.Width,
                    Height = (int)ViewPort.Height,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.RenderTarget,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                });

                devContext.ResolveSubresource(backBuffer, 0, tex2, 0, RenderContext11.DefaultColorFormat);
                Texture2D.ToStream(devContext, tex2, ImageFileFormat.Png, ms);
                tex2.Dispose();
                GC.SuppressFinalize(tex2);
            }
            else
            {
                Texture2D.ToStream(devContext, backBuffer, ImageFileFormat.Png, ms);
            }

            ms.Seek(0, SeekOrigin.Begin);

            Bitmap bmp = new Bitmap(ms);

            ms.Close();
            ms.Dispose();

            return bmp;
        }

        public Texture11 GetScreenTexture()
        {
            Texture2D tex2 = new Texture2D(RenderContext11.PrepDevice, new Texture2DDescription()
            {
                Format = RenderContext11.DefaultColorFormat,
                ArraySize = 1,
                MipLevels = 1,
                Width = (int)ViewPort.Width,
                Height = (int)ViewPort.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            devContext.ResolveSubresource(backBuffer, 0, tex2, 0, RenderContext11.DefaultColorFormat);

            return new Texture11(tex2);

        }

        public void ClearRenderTarget(SharpDX.Color color)
        {
            if (currentDepthView != null)
            {
                devContext.ClearDepthStencilView(currentDepthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            }
            devContext.ClearRenderTargetView(currentTargetView, color);
        }

        public void ClearStencilOnly()
        {
            if (currentDepthView != null)
            {
                devContext.ClearDepthStencilView(currentDepthView, DepthStencilClearFlags.Stencil, 1.0f, 255);
                devContext.ClearDepthStencilView(currentDepthView, DepthStencilClearFlags.Stencil, 1.0f, 0);
            }
        }

        public void Resize(System.Windows.Forms.Control control)
        {
            if (control.ClientSize.Width * control.ClientSize.Height == 0)
            {
                return;
            }

            if (swapChain != null)
            {
                if (renderView != null)
                {
                    renderView.Dispose();
                }
                renderView = null;
                if (backBuffer != null)
                {
                    backBuffer.Dispose();
                }
                backBuffer = null;
                if (depthView != null)
                {
                    depthView.Dispose();
                }
                if (depthBuffer != null)
                {
                    depthBuffer.Dispose();
                }
                depthView = null;
                depthBuffer = null;


                swapChain.ResizeBuffers(1, control.ClientSize.Width, control.ClientSize.Height, DefaultColorFormat, SwapChainFlags.None);

                // New RenderTargetView from the backbuffer
                backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
                renderView = new RenderTargetView(device, backBuffer);

                // Create Depth Buffer & View
                depthBuffer = new Texture2D(device, new Texture2DDescription()
                {
                    Format = DefaultDepthStencilFormat,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = control.ClientSize.Width,
                    Height = control.ClientSize.Height,
                    SampleDescription = backBuffer.Description.SampleDescription,
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                });

                depthView = new DepthStencilView(device, depthBuffer);

                devContext.OutputMerger.SetTargets(depthView, renderView);


                displayViewPort = new Viewport(0, 0, control.ClientSize.Width, control.ClientSize.Height, 0.0f, 1.0f);
                ViewPort = displayViewPort;
            }
        }


        public void Dispose()
        {
            // Release all resources
            renderView.Dispose();
            backBuffer.Dispose();
            devContext.ClearState();
            devContext.Flush();
            device.Dispose();
            devContext.Dispose();
            swapChain.Dispose();
            factory.Dispose();
        }

        public void SetIndexBuffer(IndexBuffer11 indexBuffer)
        {
            if (indexBuffer != null)
            {
                devContext.InputAssembler.SetIndexBuffer(indexBuffer.IndexBuffer, indexBuffer.format, 0);
            }
            else
            {
                devContext.InputAssembler.SetIndexBuffer(null, Format.R32_UInt, 0);
            }
        }

        public void SetVertexBuffer(IVertexBuffer11 vertexBuffer)
        {
            if (vertexBuffer != null)
            {
                devContext.InputAssembler.SetVertexBuffers(0, vertexBuffer.VertexBufferBinding);
            }
            //else
            //{
            //    devContext.InputAssembler.SetVertexBuffers(0, null);
            //}
        }

        public void SetVertexBuffer(int slot, IVertexBuffer11 vertexBuffer)
        {
            if (vertexBuffer != null)
            {
                devContext.InputAssembler.SetVertexBuffers(slot, vertexBuffer.VertexBufferBinding);
            }
        }

        public void SetVertexBuffer(VertexBufferBinding vertexBufferBind)
        {
            devContext.InputAssembler.SetVertexBuffers(0, vertexBufferBind);
        }

        public ImageSetType RenderType = ImageSetType.Sky;
        private Matrix3d view;

        private static bool shadersEnabled = true;
        public bool ShadersEnabled
        {
            get { return shadersEnabled; }
        }

        public Matrix3d View
        {
            get { return view; }
            set
            {
                view = value;
                frustumDirty = true;
                transformStateDirty = true;
            }
        }
        private Matrix3d viewBase;

        public Matrix3d ViewBase
        {
            get { return viewBase; }
            set
            {
                viewBase = value;
            }
        }

        private Matrix3d projection;

        public Matrix3d Projection
        {
            get { return projection; }
            set
            {
                projection = value;
                frustumDirty = true;
                transformStateDirty = true;
            }
        }
        private Matrix3d world;

        public Matrix3d World
        {
            get { return world; }
            set
            {
                world = value;
                frustumDirty = true;
                transformStateDirty = true;
            }
        }
        private Matrix3d worldBase;

        public Matrix3d WorldBase
        {
            get { return worldBase; }
            set
            {
                worldBase = value;
            }
        }
        private Matrix3d worldBaseNonRotating;

        public Matrix3d WorldBaseNonRotating
        {
            get { return worldBaseNonRotating; }
            set
            {
                worldBaseNonRotating = value;
            }
        }

        private double nominalRadius = 6378137.0;

        public double NominalRadius
        {
            get { return nominalRadius; }
            set { nominalRadius = value; }
        }

        Texture11 mainTexture = null;
        public Texture11 MainTexture
        {
            get { return mainTexture; }
            set
            {
                if (value != null)
                {
                    mainTexture = value;
                    Device.ImmediateContext.PixelShader.SetShaderResource(0, mainTexture.ResourceView);
                    //textureStateDirty = true;
                }
            }
        }



        public RenderContext11(Device device)
        {
            Device = device;
            devContext = device.ImmediateContext;
        }


        bool transformStateDirty = true;
        bool lightingStateDirty = true;
        bool shadowStateDirty = true;
        bool textureStateDirty = true;
        bool frustumDirty = true;
        PlaneD[] frustum = new PlaneD[6];

        public PlaneD[] Frustum
        {
            get
            {
                //if (frustumDirty)
                //{
                //    MakeFrustum();
                //}
                return frustum;
            }
        }

        public Vector3d CameraPosition;

        private System.Drawing.Color ambientLightColor = System.Drawing.Color.Black;
        public System.Drawing.Color AmbientLightColor
        {
            get
            {
                return ambientLightColor;
            }

            set
            {
                ambientLightColor = value;
                lightingStateDirty = true;
            }
        }

        private System.Drawing.Color hemiLightColor = System.Drawing.Color.Black;
        public System.Drawing.Color HemisphereLightColor
        {
            get
            {
                return hemiLightColor;
            }

            set
            {
                hemiLightColor = value;
                lightingStateDirty = true;
            }
        }

        private Vector3d hemiLightUp;
        public Vector3d HemisphereLightUp
        {
            get
            {
                return hemiLightUp;
            }

            set
            {
                hemiLightUp = value;
                lightingStateDirty = true;
            }
        }


        private System.Drawing.Color sunlightColor = System.Drawing.Color.White;
        public System.Drawing.Color SunlightColor
        {
            get
            {
                return sunlightColor;
            }

            set
            {
                sunlightColor = value;
                lightingStateDirty = true;
            }
        }

        private Vector3d sunPosition;
        public Vector3d SunPosition
        {
            get
            {
                return sunPosition;
            }

            set
            {
                sunPosition = value;
                lightingStateDirty = true;
            }
        }

        private System.Drawing.Color reflectedLightColor = System.Drawing.Color.Black;
        public System.Drawing.Color ReflectedLightColor
        {
            get
            {
                return reflectedLightColor;
            }

            set
            {
                if (reflectedLightColor != value)
                {
                    reflectedLightColor = value;
                    lightingStateDirty = true;
                }
            }
        }

        private Vector3d reflectedLightPosition;
        public Vector3d ReflectedLightPosition
        {
            get
            {
                return reflectedLightPosition;
            }

            set
            {
                reflectedLightPosition = value;
                lightingStateDirty = true;
            }
        }

        // Radius of a planet casting a shadow; zero when there's no shadow
        private double occludingPlanetRadius = 0.0;
        public double OccludingPlanetRadius
        {
            get
            {
                return occludingPlanetRadius;
            }

            set
            {
                occludingPlanetRadius = value;
            }
        }

        private Vector3d occludingPlanetPosition;
        public Vector3d OccludingPlanetPosition
        {
            get
            {
                return occludingPlanetPosition;
            }

            set
            {
                occludingPlanetPosition = value;
            }
        }

        private bool lightingEnabled = true;
        public bool LightingEnabled
        {
            get
            {
                return lightingEnabled;
            }

            set
            {
                // Always sync fixed function state
                if (shadersEnabled)
                {
                    if (value != lightingEnabled)
                    {
                        lightingStateDirty = true;
                    }
                }

                lightingEnabled = value;
            }
        }

        private bool twoSidedLighting = false;
        public bool TwoSidedLighting
        {
            get
            {
                return twoSidedLighting;
            }

            set
            {
                if (value != twoSidedLighting)
                {
                    twoSidedLighting = value;
                    lightingStateDirty = true;
                }
            }
        }

        private Vector3d localCenter = Vector3d.Empty;
        public Vector3d LocalCenter
        {
            get
            {
                return localCenter;
            }

            set
            {
                if (localCenter != value)
                {
                    localCenter = value;
                    transformStateDirty = true;
                }
            }
        }

        private Matrix[] eclipseShadowMatrices = new Matrix[PlanetShader11.MaxEclipseShadows];
        public void SetEclipseShadowMatrix(int shadowIndex, Matrix m)
        {
            eclipseShadowMatrices[shadowIndex] = m;
            shadowStateDirty = true;
        }

        private PlanetShader11 shader;
        public PlanetShader11 Shader
        {
            get
            {
                return shader;
            }

            set
            {
                if (shader != value)
                {
                    shader = value;
                    if (shader != null)
                    {
                        shader.use(Device.ImmediateContext);
                    }
                    else
                    {
                        // TODO DX11
                        //Device.VertexShader = null;
                        //Device.PixelShader = null;
                    }

                    transformStateDirty = true;
                    lightingStateDirty = true;
                    shadowStateDirty = true;
                }
            }
        }

        // *** TODO DX11
        // *** Double precision to single precision conversion functions
        //     The current functions in double3d.cs convert to the old Managed DX
        //     structures. We should switch to the SharpDX structures.
        //
        static Vector3 fromDouble(Vector3d v)
        {
            return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        }


        private void updateShaderTransformLightingConstants()
        {
            bool updateRequired = transformStateDirty || lightingStateDirty || shadowStateDirty;

            if (shader != null && updateRequired)
            {
                Matrix3d invWorld = World;
                invWorld.Invert();

                // Transform the sun position from world coordinates to planet-fixed coordinates
                // It's OK to use single-precision here because we're ultimately just interested
                // in the direction to the Sun.
                Vector3 sunDirectionObj = fromDouble(Vector3d.TransformCoordinate(sunPosition, invWorld));
                if (sunDirectionObj.Length() > 0)
                {
                    sunDirectionObj.Normalize();
                }

                // Both the transform state and lighting state affect the light direction,
                // so always update it.
                shader.SunDirection = sunDirectionObj;

                if (shader.Key.lightCount > 1)
                {
                    Vector3 reflectedDirectionObj = fromDouble(Vector3d.TransformCoordinate(reflectedLightPosition, invWorld));
                    if (reflectedDirectionObj.Length() > 0)
                    {
                        reflectedDirectionObj.Normalize();
                    }

                    shader.SetLightDirection(1, reflectedDirectionObj);
                }

                Vector3 hemiLightUpObj = fromDouble(Vector3d.TransformCoordinate(hemiLightUp, invWorld));
                hemiLightUpObj.Normalize();
                shader.HemiLightUpDirection = hemiLightUpObj;

                if (lightingStateDirty)
                {
                    shader.SetLightColor(0, new Vector3(sunlightColor.R / 255.0f, sunlightColor.G / 255.0f, sunlightColor.B / 255.0f));

                    if (shader.Key.lightCount > 1)
                    {
                        shader.SetLightColor(1, new Vector3(reflectedLightColor.R / 255.0f, reflectedLightColor.G / 255.0f,

reflectedLightColor.B / 255.0f));
                    }

                    shader.HemiLightColor = new Vector3(hemiLightColor.R / 255.0f, hemiLightColor.G / 255.0f, hemiLightColor.B /

255.0f);

                    shader.AmbientLightColor = new Vector3(ambientLightColor.R / 255.0f, ambientLightColor.G / 255.0f,

ambientLightColor.B / 255.0f);
                }

                if (transformStateDirty)
                {
                    Matrix3d worldViewMatrix = World * View;
                    shader.WorldViewMatrix = worldViewMatrix.Matrix;

                    // Set the combined world/view/projection matrix in the shader
                    Matrix wvp = (worldViewMatrix * Projection).Matrix;
                    shader.WVPMatrix = wvp;

                    // For view-dependent lighting (e.g. specular), we need the position of the camera
                    // in the planet-fixed coordinate system.
                    Matrix3d invWorldView = worldViewMatrix;
                    invWorldView.Invert();

                    Vector3d cameraPositionObj = Vector3d.TransformCoordinate(new Vector3d(0.0, 0.0, 0.0), invWorldView);
                    shader.CameraPosition = cameraPositionObj.Vector3;

                    shader.AtmosphereCenter = localCenter.Vector3;
                }

                if (shadowStateDirty)
                {
                    for (int shadowIndex = 0; shadowIndex < PlanetShader11.MaxEclipseShadows; ++shadowIndex)
                    {
                        shader.SetEclipseShadowMatrix(shadowIndex, eclipseShadowMatrices[shadowIndex]);
                    }
                }

                transformStateDirty = false;
                lightingStateDirty = false;
                shadowStateDirty = false;
            }
        }


        int currentMode = -1;

        public void PreDraw()
        {
            updateShaderTransformLightingConstants();
            if (textureStateDirty)
            {
                if (mainTexture != null && shader != null)
                {
                    shader.MainTexture = mainTexture.ResourceView;
                }
                textureStateDirty = false;
            }

            if (shader != null)
            {
                shader.use(devContext);
            }

            if (currentMode != (int)currentDepthStencilMode)
            {
                devContext.OutputMerger.DepthStencilState = standardDepthStencilStates[(int)currentDepthStencilMode];
                currentMode = (int)currentDepthStencilMode;
            }
        }

        public static void ComputeFrustum(Matrix3d projection, PlaneD[] frustum)
        {
            // Left plane 
            frustum[0].A = projection.M14 + projection.M11;
            frustum[0].B = projection.M24 + projection.M21;
            frustum[0].C = projection.M34 + projection.M31;
            frustum[0].D = projection.M44 + projection.M41;

            // Right plane 
            frustum[1].A = projection.M14 - projection.M11;
            frustum[1].B = projection.M24 - projection.M21;
            frustum[1].C = projection.M34 - projection.M31;
            frustum[1].D = projection.M44 - projection.M41;

            // Top plane 
            frustum[2].A = projection.M14 - projection.M12;
            frustum[2].B = projection.M24 - projection.M22;
            frustum[2].C = projection.M34 - projection.M32;
            frustum[2].D = projection.M44 - projection.M42;

            // Bottom plane 
            frustum[3].A = projection.M14 + projection.M12;
            frustum[3].B = projection.M24 + projection.M22;
            frustum[3].C = projection.M34 + projection.M32;
            frustum[3].D = projection.M44 + projection.M42;

            // Near plane 
            frustum[4].A = projection.M13;
            frustum[4].B = projection.M23;
            frustum[4].C = projection.M33;
            frustum[4].D = projection.M43;

            // Far plane 
            frustum[5].A = projection.M14 - projection.M13;
            frustum[5].B = projection.M24 - projection.M23;
            frustum[5].C = projection.M34 - projection.M33;
            frustum[5].D = projection.M44 - projection.M43;

            // Normalize planes 
            for (int i = 0; i < 6; i++)
            {
                frustum[i].Normalize();
            }
        }

        public void MakeFrustum()
        {
            Matrix3d viewProjection = World * View * Projection;

            Matrix3d inverseWorld = World;
            inverseWorld.Invert();

            ComputeFrustum(viewProjection, frustum);
            frustumDirty = false;
        }


        // Set up a shader for the specified material properties and the
        // current lighting environment.
        public void SetMaterial(Material material, Texture11 diffuseTex, Texture11 specularTex, Texture11 normalMap, float opacity)
        {
            PlanetSurfaceStyle surfaceStyle = PlanetSurfaceStyle.Diffuse;
            if (material.Specular != System.Drawing.Color.Black)
            {
                surfaceStyle = PlanetSurfaceStyle.Specular;
            }

            // Force the emissive style when lighting is disabled
            if (!lightingEnabled)
            {
                surfaceStyle = PlanetSurfaceStyle.Emissive;
            }

            PlanetShaderKey key = new PlanetShaderKey(surfaceStyle, false, 0);
            if (reflectedLightColor != System.Drawing.Color.Black)
            {
                key.lightCount = 2;
            }

            key.textures = 0;
            if (diffuseTex != null)
            {
                key.textures |= PlanetShaderKey.SurfaceProperty.Diffuse;
            }

            if (specularTex != null)
            {
                key.textures |= PlanetShaderKey.SurfaceProperty.Specular;
            }

            if (normalMap != null)
            {
                key.textures |= PlanetShaderKey.SurfaceProperty.Normal;
            }

            key.TwoSidedLighting = twoSidedLighting;

            SetupPlanetSurfaceEffect(key, material.Opacity * opacity);

            shader.DiffuseColor = new Vector4(material.Diffuse.R / 255.0f, material.Diffuse.G / 255.0f, material.Diffuse.B / 255.0f, material.Opacity*opacity);
            if (surfaceStyle == PlanetSurfaceStyle.Specular || surfaceStyle == PlanetSurfaceStyle.SpecularPass)
            {
                shader.SpecularColor = new Vector4(material.Specular.R / 255.0f, material.Specular.G / 255.0f, material.Specular.B / 255.0f, 0.0f);
                shader.SpecularPower = material.SpecularSharpness;
            }

            if (diffuseTex != null)
            {
                shader.MainTexture = diffuseTex.ResourceView;
            }

            if (specularTex != null)
            {
                shader.SpecularTexture = specularTex.ResourceView;
            }

            if (normalMap != null)
            {
                shader.NormalTexture = normalMap.ResourceView;
            }
        }


        // Returns a shader used to draw the planet surface (or null if no shader was used)
        public PlanetShader11 SetupPlanetSurfaceEffect(PlanetShaderKey key, float opacity)
        {
            return SetupPlanetSurfaceEffectShader(key, opacity);
        }

        public void DisableEffect()
        {
            Shader = null;
            Device.ImmediateContext.VertexShader.Set(null);
            Device.ImmediateContext.PixelShader.Set(null);
        }

        public void SetupBasicEffect(BasicEffect e, float opacity, System.Drawing.Color color)
        {
            Vector4 correctedColor;
            if (sRGB)
            {
                // sRGB correction for alpha. Even though alpha should be linear, we apply a
                // gamma correction because it's multiplied with color values. The right thing
                // to do is probably to use pre-multiplied alpha throughout WWT and not gamma
                // correct alpha.
                opacity = (float)Math.Pow(opacity, 2.2);
                correctedColor = new Vector4((float)Math.Pow(color.R / 255.0f, 2.2), 
                                             (float)Math.Pow(color.G / 255.0f, 2.2),
                                             (float)Math.Pow(color.B / 255.0f, 2.2), opacity);
            }
            else
            {
                correctedColor = new Vector4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, opacity);
            }

            switch (e)
            {
                case BasicEffect.TextureOnly:
                    {
                        PlanetShaderKey key = new PlanetShaderKey(PlanetSurfaceStyle.Emissive, false, 0);
                        SetupPlanetSurfaceEffect(key, opacity);
                        Shader.DiffuseColor = new Vector4(1.0f, 1.0f, 1.0f, opacity);
                    }
                    break;

                case BasicEffect.ColorOnly:
                    {
                        PlanetShaderKey key = new PlanetShaderKey(PlanetSurfaceStyle.Emissive, false, 0);
                        key.textures = 0;
                        SetupPlanetSurfaceEffect(key, opacity);
                        Shader.DiffuseColor = correctedColor;
                    }
                    break;

                case BasicEffect.TextureColorOpacity:
                    {
                        PlanetShaderKey key = new PlanetShaderKey(PlanetSurfaceStyle.Emissive, false, 0);
                        SetupPlanetSurfaceEffect(key, opacity);
                        Shader.DiffuseColor = correctedColor;
                    }
                    break;

                case BasicEffect.ColoredText:
                    {
                        PlanetShaderKey key = new PlanetShaderKey(PlanetSurfaceStyle.Emissive, false, 0);
                        key.AlphaTexture = true;
                        SetupPlanetSurfaceEffect(key, opacity);
                        Shader.DiffuseColor = correctedColor;
                    }
                    break;

                default:
                    Shader = null;
                    break;
            }

        }

        private PlanetShader11 SetupPlanetSurfaceEffectShader(PlanetShaderKey key, float opacity)
        {
            PlanetShader11 shader = PlanetShader11.GetPlanetShader(Device, key);

            // If we've got a shader, make it active on the device and set the
            // shader constants.
            if (shader != null)
            {
                shader.use(Device.ImmediateContext);

                // Set the combined world/view/projection matrix in the shader
                Matrix3d worldMatrix = World;
                Matrix3d viewMatrix = View;

                Matrix wvp = (worldMatrix * viewMatrix * Projection).Matrix;
                shader.WVPMatrix = wvp;
                shader.DiffuseColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

                Matrix3d invWorld = worldMatrix;
                invWorld.Invert();

                // For view-dependent lighting (e.g. specular), we need the position of the camera
                // in the planet-fixed coordinate system.
                Matrix3d worldViewMatrix = worldMatrix * viewMatrix;
                Matrix3d invWorldView = worldViewMatrix;
                invWorldView.Invert();
                shader.WorldViewMatrix = worldViewMatrix.Matrix;

                Vector3d cameraPositionObj = Vector3d.TransformCoordinate(new Vector3d(0.0, 0.0, 0.0), invWorldView);
                shader.CameraPosition = cameraPositionObj.Vector3;
            }

            Shader = shader;

            return shader;
        }

     

        public void Present(bool frameSync)
        {
            swapChain.Present(frameSync ? 1 : 0, PresentFlags.None);
            
        }

        public void SetFullScreenState(bool fullScreen)
        {
            swapChain.SetFullscreenState(fullScreen, null);
        }

        public void SaveBackBuffer(string filename, ImageFileFormat format)
        {
            if (MultiSampleCount != 1)
            {
                Texture2D tex2 = new Texture2D(RenderContext11.PrepDevice, new Texture2DDescription()
                {
                    Format = RenderContext11.DefaultColorFormat,
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = (int)ViewPort.Width,
                    Height = (int)ViewPort.Height,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                });

                devContext.ResolveSubresource(backBuffer, 0, tex2, 0, RenderContext11.DefaultColorFormat);

                Texture2D.ToFile(devContext, tex2, format, filename);
                tex2.Dispose();
                GC.SuppressFinalize(tex2);

            }
            else
            {
                Texture2D.ToFile(devContext, backBuffer, format, filename);
            }
      }


        public BlendMode BlendMode
        {
            get
            {
                return currentBlendMode;
            }

            set
            {
                if (value != currentBlendMode)
                {
                    currentBlendMode = value;
                    devContext.OutputMerger.BlendState = standardBlendStates[(int)currentBlendMode];
                }
            }
        }


        public Color4 BlendFactor
        {
            set
            {
                device.ImmediateContext.OutputMerger.BlendFactor = value;
            }
        }


        public DepthStencilMode DepthStencilMode
        {
            get
            {
                return currentDepthStencilMode;
            }

            set
            {
                if (value != currentDepthStencilMode)
                {
                    currentDepthStencilMode = value;
                    if (currentMode != (int)currentDepthStencilMode)
                    {
                        devContext.OutputMerger.DepthStencilState = standardDepthStencilStates[(int)currentDepthStencilMode];
                        currentMode = (int)currentDepthStencilMode;
                    }
                }
            }
        }


        // TODO: Extend this method with more commonly used rasterizer states
        // If the number of possible state combinations becomes large, switch
        // to a limited set of state blocks
        public void setRasterizerState(TriangleCullMode cull, bool multisample = true)
        {
            if (cull != currentCullMode || multisample != currentMultisampleState)
            {
                currentCullMode = cull;
                currentMultisampleState = multisample;
                if (multisample)
                {
                    devContext.Rasterizer.State = standardRasterizerStates[(int)currentCullMode];
                }
                else
                {
                    devContext.Rasterizer.State = standardRasterizerStates[3];
                }
            }
        }


        // Set sampler state for the pixel shader
        public void setSamplerState(int index, SamplerState state)
        {
            devContext.PixelShader.SetSampler(index, state);
        }


        // Initialize all states blocks of various types used by WWT
        private void initializeStates()
        {
            initializeBlendStates();
            initializeDepthStencilStates();
            initializeRasterizerStates();
            initializeSamplerStates();

            currentCullMode = TriangleCullMode.CullCounterClockwise;
            devContext.Rasterizer.State = standardRasterizerStates[(int)currentCullMode];
        }

        private void initializeBlendStates()
        {
            standardBlendStates = new SharpDX.Direct3D11.BlendState[6];

            BlendStateDescription templateDesc = new BlendStateDescription();
            templateDesc.AlphaToCoverageEnable = false;
            templateDesc.IndependentBlendEnable = false;
            templateDesc.RenderTarget[0] = new RenderTargetBlendDescription
            {
                IsBlendEnabled = false,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.Zero,
                BlendOperation = BlendOperation.Add,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.Zero,
                AlphaBlendOperation = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.Red | ColorWriteMaskFlags.Blue | ColorWriteMaskFlags.Green,
            };

            standardBlendStates[(int)BlendMode.None] = new SharpDX.Direct3D11.BlendState(device, templateDesc);

            templateDesc.RenderTarget[0].IsBlendEnabled = true;
            templateDesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            templateDesc.RenderTarget[0].DestinationBlend = BlendOption.One;
            standardBlendStates[(int)BlendMode.Additive] = new SharpDX.Direct3D11.BlendState(device, templateDesc);

            // "Normal" alpha blending
            templateDesc.RenderTarget[0].IsBlendEnabled = true;
            templateDesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            templateDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            standardBlendStates[(int)BlendMode.Alpha] = new SharpDX.Direct3D11.BlendState(device, templateDesc);

            // Premultiplied alpha blending, i.e. source color unmodified by source alpha
            templateDesc.RenderTarget[0].IsBlendEnabled = true;
            templateDesc.RenderTarget[0].SourceBlend = BlendOption.One;
            templateDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            standardBlendStates[(int)BlendMode.PremultipliedAlpha] = new SharpDX.Direct3D11.BlendState(device, templateDesc);

            // Source *= blend factor, dest *= inv source alpha
            templateDesc.RenderTarget[0].IsBlendEnabled = true;
            templateDesc.RenderTarget[0].SourceBlend = BlendOption.BlendFactor;
            templateDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            standardBlendStates[(int)BlendMode.BlendFactorInverseAlpha] = new SharpDX.Direct3D11.BlendState(device, templateDesc);    
        
            // Don't write any color, just the z-buffer
            templateDesc.RenderTarget[0].IsBlendEnabled = true;
            templateDesc.RenderTarget[0].SourceBlend = BlendOption.Zero;
            templateDesc.RenderTarget[0].DestinationBlend = BlendOption.One;
            standardBlendStates[(int)BlendMode.NoColorWrite] = new SharpDX.Direct3D11.BlendState(device, templateDesc);    
       
        
        }

        private void initializeDepthStencilStates()
        {
            standardDepthStencilStates = new SharpDX.Direct3D11.DepthStencilState[4];

            DepthStencilStateDescription templateDesc = new DepthStencilStateDescription
            {
                IsDepthEnabled = false,
                DepthComparison = Comparison.Never,
                DepthWriteMask = SharpDX.Direct3D11.DepthWriteMask.Zero,
                IsStencilEnabled = false
            };

            standardDepthStencilStates[(int)DepthStencilMode.Off] = new DepthStencilState(device, templateDesc);
            templateDesc.DepthComparison = Comparison.LessEqual;
            templateDesc.IsDepthEnabled = true;
            standardDepthStencilStates[(int)DepthStencilMode.ZReadOnly] = new DepthStencilState(device, templateDesc);

            templateDesc.IsDepthEnabled = false;
            templateDesc.DepthWriteMask = DepthWriteMask.All;
            standardDepthStencilStates[(int)DepthStencilMode.ZWriteOnly] = new DepthStencilState(device, templateDesc);

            templateDesc.IsDepthEnabled = true;
            templateDesc.DepthWriteMask = DepthWriteMask.All;
            standardDepthStencilStates[(int)DepthStencilMode.ZReadWrite] = new DepthStencilState(device, templateDesc);
        }

        private void initializeRasterizerStates()
        {
            standardRasterizerStates = new SharpDX.Direct3D11.RasterizerState[4];

            RasterizerStateDescription templateDesc = new RasterizerStateDescription
            {
                FillMode = SharpDX.Direct3D11.FillMode.Solid,
                CullMode = SharpDX.Direct3D11.CullMode.Back,
                IsFrontCounterClockwise = false,
                DepthBias = 0,
                DepthBiasClamp = 0.0f,
                SlopeScaledDepthBias = 0.0f,
                IsDepthClipEnabled = true,
                IsScissorEnabled = false,
                IsMultisampleEnabled = true,
                IsAntialiasedLineEnabled = true
            };

            templateDesc.CullMode = CullMode.None;
            standardRasterizerStates[(int)TriangleCullMode.Off] = new RasterizerState(device, templateDesc);

            templateDesc.CullMode = CullMode.Back;
            templateDesc.IsFrontCounterClockwise = true;
            standardRasterizerStates[(int)TriangleCullMode.CullClockwise] = new RasterizerState(device, templateDesc);

            templateDesc.CullMode = CullMode.Back;
            templateDesc.IsFrontCounterClockwise = false;
            standardRasterizerStates[(int)TriangleCullMode.CullCounterClockwise] = new RasterizerState(device, templateDesc);

            // Special rasterizer state with multisample off. Used for drawing anti-aliased lines without
            // MSAA, i.e. the lines are drawn with edge anti-aliasing. This produces better results on NVIDIA
            // GPUs, especially when alpha be
            templateDesc.CullMode = CullMode.None;
            templateDesc.IsMultisampleEnabled = false;
            standardRasterizerStates[3] = new RasterizerState(device, templateDesc);
        }

        private void initializeSamplerStates()
        {
            SamplerStateDescription templateDesc = new SamplerStateDescription
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = new Color4(0.0f, 0.0f, 0.0f, 0.0f),
                Filter = Filter.Anisotropic,
                MaximumAnisotropy = 16,
                MinimumLod = 0,
                MaximumLod = 32,
                MipLodBias = 0
            };

            if (Downlevel)
            {
                templateDesc.MaximumLod = float.MaxValue;
            }



            wrapSampler = new SamplerState(device, templateDesc);

            templateDesc.AddressU = TextureAddressMode.Clamp;
            templateDesc.AddressV = TextureAddressMode.Clamp;
            templateDesc.AddressW = TextureAddressMode.Clamp;
            clampSampler = new SamplerState(device, templateDesc);

            templateDesc.AddressU = TextureAddressMode.Border;
            templateDesc.AddressV = TextureAddressMode.Border;
            templateDesc.AddressW = TextureAddressMode.Border;
            templateDesc.BorderColor = new SharpDX.Color4(0.0f, 0.0f, 0.0f, 0.0f);
            transparentBorderSampler = new SamplerState(device, templateDesc);

            // Set up standard samplers
            // TODO: The sampler index constants should be defined somewhere; right now,
            // the indexes are chosen to match the shader generator.
            setSamplerState(3, transparentBorderSampler);
        }

        public SamplerState WrapSampler
        {
            get
            {
                return wrapSampler;
            }
        }

        public SamplerState ClampSampler
        {
            get
            {
                return clampSampler;
            }
        }

        public SamplerState TransparentBorderSampler
        {
            get
            {
                return transparentBorderSampler;
            }
        }
    }
}
