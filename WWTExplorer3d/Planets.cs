using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text;
using AstroCalc;
using System.Threading;


namespace TerraViewer
{
    public enum SolarSystemObjects
    {
        Sun,
        Mercury,
        Venus,
        Mars,
        Jupiter,
        Saturn,
        Uranus,
        Neptune,
        Pluto,
        Moon,
        Io,
        Europa,
        Ganymede,
        Callisto,
        IoShadow,
        EuropaShadow,
        GanymedeShadow,
        CallistoShadow,
        SunEclipsed,
        Earth,
        Custom,
        Undefined = 65536
    };

    public struct BodyAngles
    {
        public double poleDec;
        public double poleRa;
        public double primeMeridian;
        public double rotationRate;
    };

    class OrbitTrace
    {
        struct OrbitPathPoint
        {
            public OrbitPathPoint(double jd, Vector3d position)
            {
                this.jd = jd;
                this.position = position;
            }
            public readonly double jd;
            public readonly Vector3d position;
        };

        SolarSystemObjects body;
        OrbitPathPoint[] points;
        uint pointCount;
        double pathDuration;
        double coverageDuration;

        //SharpDX.Direct3D11.Buffer vertexBuffer;
        //SharpDX.Direct3D11.VertexBufferBinding vertexBinding;
        GenVertexBuffer<SharpDX.Vector4> vertexBuffer;
        SharpDX.Vector4[] transferBuffer;

        const double coverageWindowDurationFactor = 1.5;

        public OrbitTrace(SolarSystemObjects body, uint pointCount, double pathDuration)
        {
            this.body = body;
            this.pathDuration = pathDuration;
            this.coverageDuration = this.pathDuration * coverageWindowDurationFactor;
            this.pointCount = (uint) (pointCount * coverageWindowDurationFactor);
        }

        public void update(double jd)
        {
            if (points == null || jd > points[0].jd || jd - pathDuration < points[points.Length - 1].jd)
            {
                if (points == null)
                {
                    points = new OrbitPathPoint[pointCount];
                }

                double t0 = jd + (pathDuration * (coverageWindowDurationFactor - 1.0) / 2.0);
                double dt = coverageDuration / (pointCount - 1);
                if (body == SolarSystemObjects.Earth)
                {
                    // Optimize calculation of Earth orbit points by omitting light time iteration
                    double semiMajorAxis = 1.0;

                    const double c = 299792.458;
                    double approxLightTimeSec = semiMajorAxis * UiTools.KilometersPerAu / c;
                    double approxLightTimeDays = approxLightTimeSec / 86400.0;

                    for (uint i = 0; i < pointCount; ++i)
                    {
                        double t = t0 - i * dt;
                        points[i] = new OrbitPathPoint(t, Planets.GetPlanetPositionDirect(body, t - approxLightTimeDays));
                    }
                }
                else
                {
                    for (uint i = 0; i < pointCount; ++i)
                    {
                        double t = t0 - i * dt;
                        if (body == SolarSystemObjects.Earth)
                            points[i] = new OrbitPathPoint(t, Planets.GetPlanetPositionDirect(body, t - 8 / 1440.0));
                        else
                            points[i] = new OrbitPathPoint(t, Planets.GetPlanet3dLocation(body, t));
                    }
                }

                updateVertexBuffer();
            }
        }

        public void render(RenderContext11 renderContext, Color color, Matrix3d worldMatrix, double jd, Vector3d positionNow, double duration)
        {
            duration = pathDuration;
            if (vertexBuffer != null)
            {
                double dt = coverageDuration / ((int) pointCount - 1);
                double t0 = points[0].jd;
                int firstPoint = (int)Math.Floor((t0 - jd) / dt);
                firstPoint = Math.Max(0, firstPoint);
                int lastPoint = (int)Math.Floor((t0 - (jd - duration)) / dt);
                lastPoint = Math.Min(points.Length - 1, lastPoint);
                int drawCount = lastPoint - firstPoint;

                double timeOffset = (t0 - jd) / coverageDuration;

                Matrix3d savedWorld = renderContext.World;
                renderContext.World = worldMatrix;

                renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineStrip;

                OrbitTraceShader.UseShader(renderContext, new SharpDX.Color(color.R, color.G, color.B, color.A), savedWorld, positionNow, timeOffset, 1.5);
                renderContext.SetVertexBuffer(vertexBuffer);
                renderContext.Device.ImmediateContext.Draw(drawCount, firstPoint);

                renderContext.World = savedWorld;
            }
        }

        private void updateVertexBuffer()
        {
            if (vertexBuffer == null)
            {
                transferBuffer = new SharpDX.Vector4[pointCount];
                vertexBuffer = new GenVertexBuffer<SharpDX.Vector4>(RenderContext11.PrepDevice, transferBuffer);
            }

            double t0 = points[0].jd;

            // Convert to single precision
            for (uint i = 0; i < pointCount; ++i)
            {
                OrbitPathPoint pathPoint = points[i];
                transferBuffer[i] = new SharpDX.Vector4((float)pathPoint.position.X,
                                                        (float)pathPoint.position.Y,
                                                        (float)pathPoint.position.Z,
                                                        (float)((t0 - pathPoint.jd) / coverageDuration));
            }

            vertexBuffer.Update(transferBuffer);
        }
    };

    class Planets
    {
        private static Texture11[] planetTextures;

        public static Texture11[] PlanetTextures
        {
            get
            {
                if (planetTextures == null)
                {
                    LoadPlanetTextures();
                }
                return Planets.planetTextures;
            }
        }
        public static Texture11[] planetTexturesMaps;

        private static PointSpriteSet planetsPointSet;

        // public static Texture earth;
        static double[] planetScales;
        static double[] planetDiameters;
        static double[] planetTilts;
        static Vector3d[,] orbits;
        static OrbitTrace[] orbitTraces;
        static SortedList<double, int> planetDrawOrder = new SortedList<double,int>();

        static Texture11 cloudTexture;
        static Texture11 ringsMap;

        // mu is the standard gravitational parameter GM, where G
        // is the gravitational constant and M is the mass of the
        // central body.
        const double muSun = 1.327124400188e11; // km^3/s^2
        const double muEarth = 3.9860044189e5;
        const double muMoon = 4.9027779e3;
        const double muJupiter = 1.26686534e8;

        public static Texture11 LoadPlanetTexture(Bitmap bmp)
        {
            Texture11 temp;
            //MemoryStream ms = new MemoryStream();
            //bmp.Save(ms, ImageFormat.Png);
            //ms.Seek(0, SeekOrigin.Begin);
            //temp = TextureLoader.FromStream(device, ms, bmp.Width, bmp.Height, 0, Usage.None, Format.Unknown, Pool.Managed, Filter.None, Filter.Box, 0);
            //ms.Dispose();
            //GC.SuppressFinalize(ms);

            temp = Texture11.FromBitmap(RenderContext11.PrepDevice, bmp);

            return temp;
        }
        public static Vector3d GetPlanet3dLocation(SolarSystemObjects target)
        {
            try
            {
                if ((int)target < 21)
                {
                    return planet3dLocations[(int)target];
                }
            }
            catch
            {
            }
            return new Vector3d(0, 0, 0);
        }

        public const float EarthCloudHeightMeters = 12800.0f;
        public static bool is8k = false;
        public static Texture11 CloudTexture
        {
            get
            {
                bool shouldBe8k = Properties.Settings.Default.CloudMap8k && !RenderContext11.Downlevel;

                if (cloudTexture == null || shouldBe8k != is8k)
                {
                    //if (cloudTexture != null)
                    //{
                    //    cloudTexture.Dispose();
                    //    cloudTexture = null;
                    //}

                    try
                    {
                        if (Properties.Settings.Default.CloudMap8k && !RenderContext11.Downlevel)
                        {
                            LoadCloudsBackground(true);
                        }
                        else
                        {
                           LoadCloudsBackground(false);
                        }
                    }
                    catch
                    {
                    }
                }
                return cloudTexture;
            }
        }

        delegate void BackInitDelegate();


        static Texture11 clouds8k = null;
        static Texture11 clouds = null;

        static bool loading8k = false;
        static bool loading = false;


        private static void LoadCloudsBackground(bool load8k)
        {
            if (load8k)
            {
                if (!loading8k)
                {
                    BackInitDelegate initBackground = LoadBackground8k;
                    initBackground.BeginInvoke(null, null);
                }
            }
            else
            {
                if (!loading)
                {
                    BackInitDelegate initBackground = LoadBackground;
                    initBackground.BeginInvoke(null, null);
                }
            }
        }

        private static void LoadBackground8k()
        {
            loading8k = true;
            string downloadName = Properties.Settings.Default.CahceDirectory + @"data\bigclouds.dds";
            try
            {
                if (clouds8k == null)
                {
                    DataSetManager.DownloadFile("http://www.worldwidetelescope.org/data/bigclouds.dds.png", downloadName, true, true);
                    clouds8k = Texture11.FromFile(downloadName);
                }
                cloudTexture = clouds8k;
            }
            catch
            {
                if (File.Exists(downloadName))
                {
                    File.Delete(downloadName);
                }
            }
            is8k = true;
            loading8k = false;
        }

        private static void LoadBackground()
        {
            loading = true;
            string downloadName = Properties.Settings.Default.CahceDirectory + @"data\bigclouds.png";
            try
            {
                if (clouds == null)
                {
                    DataSetManager.DownloadFile("http://www.worldwidetelescope.org/data/bigclouds.png", downloadName, true, true);
                    clouds = Texture11.FromFile(downloadName);
                }
                cloudTexture = clouds;
            }
            catch
            {
                if (File.Exists(downloadName))
                {
                    File.Delete(downloadName);
                }
            }
            is8k = false;
            loading = false;
        }


        static Texture11 RingsMap
        {
            get
            {
                if (ringsMap == null)
                {
                    ringsMap = LoadPlanetTexture(Properties.Resources.SaturnRings);
                }
                return ringsMap;
            }
        }


        public static double GetPlanet3dSufaceAltitude(SolarSystemObjects target)
        {
            try
            {
                if ((int)target < 21)
                {
                    return GetAdjustedPlanetRadius((int)target);
                }
            }
            catch
            {
            }
            return 0;
        }

        public static Vector3d GetPlanetTargetPoint(SolarSystemObjects target, double lat, double lng, double jNow)
        {
            Vector3d temp;
            if (jNow == 0)
            {
                temp = Planets.GetPlanet3dLocation(target);
            }
            else
            {
                temp = Planets.GetPlanet3dLocation(target, jNow);
            }
            temp.Add(Coordinates.RADecTo3d((lng / 15) + 6, lat, Planets.GetPlanet3dSufaceAltitude(target)));
            return temp;
        }

        public static Vector3d GetPlanet3dLocation(SolarSystemObjects target, double jNow)
        {
            // Directly calculate 3D position of all planets but Earth and Jupiter
            int id = (int)target;
            if ((id >= 1 && id <= 3) || (id >= 5 && id <= 8))
            {
                return GetPlanetPositionDirect(target, jNow);
            }

            try
            {
                Vector3d result = new Vector3d();
                AstroRaDec centerRaDec = AstroCalc.AstroCalc.GetPlanet(jNow, 0, 0, 0, -6378149);
                Vector3d center = Coordinates.RADecTo3d(centerRaDec.RA, centerRaDec.Dec, centerRaDec.Distance);
                if (target == SolarSystemObjects.Earth)
                {
                    result = new Vector3d(-center.X, -center.Y, -center.Z);
                }
                else
                {
                    AstroRaDec planet = AstroCalc.AstroCalc.GetPlanet(jNow, (int)target, 0, 0, -6378149);
                    result = Coordinates.RADecTo3d(planet.RA, planet.Dec, planet.Distance);
                    result.Subtract(center);
                }

                result.RotateX(Coordinates.MeanObliquityOfEcliptic(jNow) * RC);
                if (Settings.Active.SolarSystemScale != 1)
                {
                    switch (target)
                    {
                        case SolarSystemObjects.Moon:
                            {
                                Vector3d parent = GetPlanet3dLocation(SolarSystemObjects.Earth, jNow);
                                // Parent Centric
                                result.Subtract(parent);
                                result.Multiply(Settings.Active.SolarSystemScale / 2);
                                result.Add(parent);
                            }
                            break;
                        case SolarSystemObjects.Io:
                        case SolarSystemObjects.Europa:
                        case SolarSystemObjects.Ganymede:
                        case SolarSystemObjects.Callisto:
                            {
                                Vector3d parent = GetPlanet3dLocation(SolarSystemObjects.Jupiter, jNow);

                                // Parent Centric
                                result.Subtract(parent);
                                result.Multiply(Settings.Active.SolarSystemScale);
                                result.Add(parent);
                            }
                            break;

                        default:
                            break;
                    }
                }
                return result;
            }
            catch
            {
                return new Vector3d(0, 0, 0);
            }
        }
        public static AstroRaDec GetPlanetLocation(string name)
        {
            int id = GetPlanetIDFromName(name);
            if (id > -1)
            {
                if (planetLocations != null)
                {

                    return planetLocations[id];
                }
                else
                {
                    return AstroCalc.AstroCalc.GetPlanet(SpaceTimeController.JNow, id, SpaceTimeController.Location.Lat, SpaceTimeController.Location.Lng, SpaceTimeController.Altitude);
                }
            }
            return new AstroRaDec(0, 0, 1, false, false);
        }
        public static AstroRaDec GetPlanetLocation(string name, double jNow)
        {
            int id = GetPlanetIDFromName(name);


            return AstroCalc.AstroCalc.GetPlanet(jNow, id, SpaceTimeController.Location.Lat, SpaceTimeController.Location.Lng, SpaceTimeController.Altitude);

        }
        public static int GetPlanetIDFromName(string planetName)
        {
            switch (planetName)
            {
                case "Sun":
                    return 0;
                case "Mercury":
                    return 1;
                case "Venus":
                    return 2;
                case "Mars":
                    return 3;
                case "Jupiter":
                    return 4;
                case "Saturn":
                    return 5;
                case "Uranus":
                    return 6;
                case "Neptune":
                    return 7;
                case "Pluto":
                    return 8;
                case "Moon":
                    return 9;
                case "Io":
                    return 10;
                case "Europa":
                    return 11;
                case "Ganymede":
                    return 12;
                case "Callisto":
                    return 13;
                case "Earth":
                case "Solar Eclipse 2017":
                    return 19;
                default:
                    return -1;
            }
        }




        public static string GetNameFrom3dId(int id)
        {
            switch (id)
            {
                case 0:
                    return "Sun";
                case 1:
                    return "Mercury";
                case 2:
                    return "Venus";
                case 3:
                    return "Mars";
                case 4:
                    return "Jupiter";
                case 5:
                    return "Saturn";
                case 6:
                    return "Uranus";
                case 7:
                    return "Neptune";
                case 8:
                    return "Pluto";
                case 9:
                    return "Moon";
                case 10:
                    return "Io (Jupiter)";
                case 11:
                    return "Europa (Jupiter)";
                case 12:
                    return "Ganymede (Jupiter)";
                case 13:
                    return "Callisto (Jupiter)";
                case 19:
                  //  return "Virtual Earth Aerial";
                    return "Bing Maps Aerial";
                default:
                    return "";
            }
        }
        public static string GetLocalPlanetName(string planetName)
        {
            switch (planetName)
            {
                case "Sun":
                    return Language.GetLocalizedText(291, "Sun");
                case "Mercury":
                    return Language.GetLocalizedText(292, "Mercury");
                case "Venus":
                    return Language.GetLocalizedText(293, "Venus");
                case "Mars":
                    return Language.GetLocalizedText(294, "Mars");
                case "Jupiter":
                    return Language.GetLocalizedText(295, "Jupiter");
                case "Saturn":
                    return Language.GetLocalizedText(296, "Saturn");
                case "Uranus":
                    return Language.GetLocalizedText(297, "Uranus");
                case "Neptune":
                    return Language.GetLocalizedText(298, "Neptune");
                case "Pluto":
                    return Language.GetLocalizedText(299, "Pluto");
                case "Moon":
                    return Language.GetLocalizedText(300, "Moon");
                case "Io":
                    return Language.GetLocalizedText(301, "Io");
                case "Europa":
                    return Language.GetLocalizedText(302, "Europa");
                case "Ganymede":
                    return Language.GetLocalizedText(303, "Ganymede");
                case "Callisto":
                    return Language.GetLocalizedText(304, "Callisto");
                default:
                    return planetName;
            }
        }

        public static bool ShowActualSize = Properties.Settings.Default.ActualPlanetScale;
        protected const double RC = (Math.PI / 180.0);
        public static double[] planetOrbitalYears;
        private static Vector3d[] planet3dLocations;

        public static Color[] planetColors;
        public static double[] planetRotationPeriod;

        static double jNow = 0;

        // Values taken from version 10 of the SPICE planetary constants file, updated
        // October 21, 2011: ftp://naif.jpl.nasa.gov/pub/naif/generic_kernels/pck/pck00010.tpc
        //
        // Precession rates for rotation angles are currently not stored.
        //
        // All angles are in degrees.

        static BodyAngles[] planetAngles =
        {
            new BodyAngles { poleRa = 286.13,     poleDec =  63.87,     primeMeridian =  84.176,  rotationRate =   14.18440    }, // Sun
            new BodyAngles { poleRa = 281.0097,   poleDec =  61.4143,   primeMeridian = 329.548,  rotationRate =    6.1385025  }, // Mercury
            new BodyAngles { poleRa = 272.76,     poleDec =  67.16,     primeMeridian = 160.20,   rotationRate =   -1.4813688 }, // Venus
            new BodyAngles { poleRa = 317.68143,  poleDec =  52.88650,  primeMeridian = 176.630,  rotationRate =  350.89198226 }, // Mars
            new BodyAngles { poleRa = 268.056595, poleDec =  64.495303, primeMeridian = 284.95,   rotationRate =  870.5360000 }, // Jupiter
            new BodyAngles { poleRa =  40.589,    poleDec =  83.537,    primeMeridian =  38.90,   rotationRate =  810.7939024 }, // Saturn
            new BodyAngles { poleRa = 257.311,    poleDec = -15.175,    primeMeridian = 203.81,   rotationRate = -501.1600928 }, // Uranus
            new BodyAngles { poleRa = 299.36,     poleDec =  43.46,     primeMeridian = 253.18,   rotationRate =  536.3128492 }, // Neptune
            new BodyAngles { poleRa = 132.993,    poleDec =  -6.163,    primeMeridian = 302.695,  rotationRate =   56.3625225 }, // Pluto
            new BodyAngles { poleRa = 269.9949,   poleDec =  66.5392,   primeMeridian =  38.3213, rotationRate =   13.17635815 }, // Moon
            new BodyAngles { poleRa = 268.05,     poleDec =  64.50,     primeMeridian = 200.39,   rotationRate =  203.4889538 }, // Io
            new BodyAngles { poleRa = 268.08,     poleDec =  64.51,     primeMeridian =  36.022,  rotationRate =  101.3747235 }, // Europa
            new BodyAngles { poleRa = 268.20,     poleDec =  64.57,     primeMeridian =  44.064,  rotationRate =   50.3176081 }, // Ganymede
            new BodyAngles { poleRa = 268.72,     poleDec =  64.83,     primeMeridian = 259.51,   rotationRate =   21.5710715 }, // Callisto
            new BodyAngles { poleRa =   0.0,      poleDec = 0.0, primeMeridian = 0.0, rotationRate = 0.0 }, // UNUSED - IoShadow
            new BodyAngles { poleRa =   0.0,      poleDec = 0.0, primeMeridian = 0.0, rotationRate = 0.0 }, // UNUSED - EuropaShadow
            new BodyAngles { poleRa =   0.0,      poleDec = 0.0, primeMeridian = 0.0, rotationRate = 0.0 }, // UNUSED - GanymedeShadow
            new BodyAngles { poleRa =   0.0,      poleDec = 0.0, primeMeridian = 0.0, rotationRate = 0.0 }, // UNUSED - CallistoShadow
            new BodyAngles { poleRa =   0.0,      poleDec = 0.0, primeMeridian = 0.0, rotationRate = 0.0 }, // UNUSED - SunEclipsed
            new BodyAngles { poleRa =   0.0,      poleDec = 90.0, primeMeridian = 190.147, rotationRate = 360.9856235 } // Earth
        };

        static BlendState[] orbitBlendStates = 
        {
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1),
            new BlendState(true,1000,1)
        };

        public static void UpdatePlanetLocations(bool threeDee)
        {

            if (planetDiameters == null)
            {
                planetDiameters = new double[20];
                planetDiameters[0] = 0.009291568;
                planetDiameters[1] = 0.0000325794793734425;
                planetDiameters[2] = 0.0000808669220531394;
                planetDiameters[3] = 0.0000453785605596396;
                planetDiameters[4] = 0.000954501;
                planetDiameters[5] = 0.000802173;
                planetDiameters[6] = 0.000339564;
                planetDiameters[7] = 0.000324825;
                planetDiameters[8] = 0.0000152007379777805;
                planetDiameters[9] = 0.0000232084653538149;
                planetDiameters[10] = 0.0000243519298386342;
                planetDiameters[11] = 0.0000208692629580609;
                planetDiameters[12] = 0.0000351742670356556;
                planetDiameters[13] = 0.0000322263666626559;
                planetDiameters[14] = 0.0000243519298386342;
                planetDiameters[15] = 0.0000208692629580609;
                planetDiameters[16] = 0.0000351742670356556;
                planetDiameters[17] = 0.0000322263666626559;
                planetDiameters[18] = 0.009291568 * 2;
                planetDiameters[19] = 0.00008556264121178090;
            }
            if (planetColors == null)
            {
                planetColors = new Color[20];
                planetColors[0] = Color.Yellow;
                planetColors[1] = Color.White;
                planetColors[2] = Color.LightYellow;
                planetColors[3] = Color.OrangeRed;
                planetColors[4] = Color.Orange;
                planetColors[5] = Color.DarkGoldenrod;
                planetColors[6] = Color.LightBlue;
                planetColors[7] = Color.Blue;
                planetColors[8] = Color.White;
                planetColors[9] = Color.White;
                planetColors[10] = Color.FromArgb(255, 255, 192);
                planetColors[11] = Color.FromArgb(224, 224, 216);
                planetColors[12] = Color.FromArgb(192, 160, 128);
                planetColors[13] = Color.FromArgb(128, 104, 72);
                planetColors[14] = Color.LightYellow;
                planetColors[15] = Color.LightYellow;
                planetColors[16] = Color.LightYellow;
                planetColors[17] = Color.LightYellow;
                planetColors[18] = Color.White;
                planetColors[19] = Color.LightBlue;

            }

            if (planetTilts == null)
            {
                planetTilts = new double[20];
                planetTilts[0] = 0.0;
                planetTilts[1] = 0.01;
                planetTilts[2] = 177.4;
                planetTilts[3] = 25.19;
                planetTilts[4] = 3.13;
                planetTilts[5] = 26.73;
                planetTilts[6] = 97.77;
                planetTilts[7] = 28.32;
                planetTilts[8] = 119.61;
                planetTilts[9] = 23.439;
                planetTilts[10] = 2.21;
                planetTilts[11] = 0;
                planetTilts[12] = -0.33;
                planetTilts[13] = 0;
                planetTilts[14] = 0;
                planetTilts[15] = 0;
                planetTilts[16] = 0;
                planetTilts[17] = 0;
                planetTilts[18] = 0;
                planetTilts[19] = 23.5;
            }

            planetTilts[19] = obliquity / RC;

            if (planetRotationPeriod == null)
            {
                planetRotationPeriod = new double[20];
                planetRotationPeriod[0] = 25.37995;
                planetRotationPeriod[1] = 58.6462;
                planetRotationPeriod[2] = 243.0187;
                planetRotationPeriod[3] = 1.02595675;
                planetRotationPeriod[4] = 0.41007;
                planetRotationPeriod[5] = 0.426;
                planetRotationPeriod[6] = -0.71833;
                planetRotationPeriod[7] = 0.67125;
                planetRotationPeriod[8] = -6.38718;
                planetRotationPeriod[9] = 27.3;
                planetRotationPeriod[10] = 1.769137786;
                planetRotationPeriod[11] = 3.551;
                planetRotationPeriod[12] = 7.155;
                planetRotationPeriod[13] = 16.69;
                planetRotationPeriod[14] = 0;
                planetRotationPeriod[15] = 0;
                planetRotationPeriod[16] = 0;
                planetRotationPeriod[17] = 0;
                planetRotationPeriod[18] = 0;
                planetRotationPeriod[19] = .99726968;
            }

            if (planetScales == null)
            {
                planetScales = new double[20];
            }



            if (planet3dLocations == null)
            {
                planet3dLocations = new Vector3d[20];
            }

            if (Settings.Active.ActualPlanetScale)
            {
                planetScales[0] = .5f;
                planetScales[1] = .25f;
                planetScales[2] = .25f;
                planetScales[3] = .25f;
                planetScales[4] = .25f;
                planetScales[5] = .5f;
                planetScales[6] = .25f;
                planetScales[7] = .25f;
                planetScales[8] = .25f;
                planetScales[9] = .25f;
                planetScales[10] = .25f;
                planetScales[11] = .25f;
                planetScales[12] = .25f;
                planetScales[13] = .25f;
                planetScales[14] = .25f;
                planetScales[15] = .25f;
                planetScales[16] = .25f;
                planetScales[17] = .25f;
                planetScales[18] = .5f;
                planetScales[19] = .25f;

            }
            else
            {
                for (int i = 0; i < 20; i++)
                {
                    if (i < 10)
                    {
                        planetScales[i] = .25f;
                    }
                    else
                    {
                        planetScales[i] = .1f;
                    }
                }

                // Make Sun and Saturn bigger
                planetScales[0] = .5f;
                planetScales[5] = .5f;
                planetScales[18] = .5f;

            }

            planetDrawOrder.Clear();
            // Initialized in declaration
            //planetLocations = new AstroRaDec[20];
            jNow = SpaceTimeController.JNow;
            Vector3d center = new Vector3d(0, 0, 0);
            int planetCenter = 0;
            if (planetCenter > -1)
            {
                AstroRaDec centerRaDec = AstroCalc.AstroCalc.GetPlanet(jNow, planetCenter, threeDee ? 0 : SpaceTimeController.Location.Lat, threeDee ? 0 : SpaceTimeController.Location.Lng, threeDee ? -6378149 : SpaceTimeController.Altitude);
                center = Coordinates.RADecTo3d(centerRaDec.RA, centerRaDec.Dec, centerRaDec.Distance);
            }
            planet3dLocations[19] = new Vector3d(-center.X, -center.Y, -center.Z);
            planet3dLocations[19].RotateX(obliquity);
            for (int i = 0; i < 18; i++)
            {
                planetLocations[i] = AstroCalc.AstroCalc.GetPlanet(jNow, i, threeDee ? 0 : SpaceTimeController.Location.Lat, threeDee ? 0 : SpaceTimeController.Location.Lng, threeDee ? -6378149 : SpaceTimeController.Altitude);
                planet3dLocations[i] = Coordinates.RADecTo3d(planetLocations[i].RA, planetLocations[i].Dec, planetLocations[i].Distance);

                //planet3dLocations[i] = new Vector3d(planet3dLocations[i].X - center.X,
                //                                        planet3dLocations[i].Y - center.Y,
                //                                        planet3dLocations[i].Z - center.Z);
                planet3dLocations[i].Subtract(center);

                planet3dLocations[i].RotateX(obliquity);

                // Directly calculate 3D position of all planets but Earth and Jupiter
                if ((i >= 1 && i <= 3) || (i >= 5 && i <= 8))
                {
                    planet3dLocations[i] = GetPlanetPositionDirect((SolarSystemObjects)i, jNow);
                }

                if (Settings.Active.ActualPlanetScale)
                {
                    planetScales[i] = (2 * Math.Atan(.5 * (planetDiameters[i] / planetLocations[i].Distance))) / Math.PI * 180;
                    if (i == 5 || i == 0)
                    {
                        planetScales[i] *= 2;
                    }
                    //if ((i > 9 && i != 18) && Properties.Settings.Default.ShowMoonsAsPointSource) // Jupiters moons should be bigger
                    //{
                    //    planetScales[i] *= 2;
                    //}
                }
                if (Settings.Active.SolarSystemScale != 1)
                {

                    SolarSystemObjects id = (SolarSystemObjects)i;
                    switch (id)
                    {
                        case SolarSystemObjects.Moon:
                            {
                                Vector3d parent = (planet3dLocations[(int)SolarSystemObjects.Earth]);
                                // Parent Centric
                                planet3dLocations[i].Subtract(parent);
                                planet3dLocations[i].Multiply(Settings.Active.SolarSystemScale / 2);
                                planet3dLocations[i].Add(parent);
                            }
                            break;
                        case SolarSystemObjects.Io:
                        case SolarSystemObjects.Europa:
                        case SolarSystemObjects.Ganymede:
                        case SolarSystemObjects.Callisto:
                            {
                                Vector3d parent = (planet3dLocations[(int)SolarSystemObjects.Jupiter]);
                                // Parent Centric
                                planet3dLocations[i].Subtract(parent);
                                planet3dLocations[i].Multiply(Settings.Active.SolarSystemScale);
                                planet3dLocations[i].Add(parent);
                            }
                            break;

                        default:
                            break;
                    }
                }


                planetDrawOrder.Add(-planetLocations[i].Distance, i);
            }




            planetLocations[18] = planetLocations[0];
            planetScales[18] = planetScales[0];

            lastUpdate = SpaceTimeController.Now;

            // Update orbits
            if (orbitTraces == null)
            {
                // We have two methods of drawing planet orbits: 
                //    1) ellipses representing the osculating Keplerian elements
                //    2) orbit 'traces' which plot the actual path an object has traveled over some period of time
                //
                // Ellipses can be rendered very efficiently by offloading most of the computation from the CPU to the GPU. 
                // However, the orbital elements of significantly perturbed bodies vary enough that the osculating ellipse
                // appears to pulse when the time rate is accelerated. This is confusing to most people, so we avoid it by 
                // using traces for the outer planets (which perturb each other) and the Earth (where the orbit is affected
                // by the Moon.)
                orbitTraces = new OrbitTrace[20];
                orbitTraces[(int)SolarSystemObjects.Earth]   = new OrbitTrace(SolarSystemObjects.Earth,   100,   1    * 365.25);
                orbitTraces[(int)SolarSystemObjects.Saturn]  = new OrbitTrace(SolarSystemObjects.Saturn,  100,  29.45 * 365.25);
                orbitTraces[(int)SolarSystemObjects.Uranus]  = new OrbitTrace(SolarSystemObjects.Uranus,  100,  84.32 * 365.25);
                orbitTraces[(int)SolarSystemObjects.Neptune] = new OrbitTrace(SolarSystemObjects.Neptune, 100, 165    * 365.25);
                orbitTraces[(int)SolarSystemObjects.Pluto]   = new OrbitTrace(SolarSystemObjects.Pluto,   100, 248    * 365.25);
            }
            for (int i = 0; i < 20; ++i)
            {
                if (orbitTraces[i] != null)
                {
                    orbitTraces[i].update(jNow);
                }
            }
        }
        static Matrix3d eclipticTilt = Matrix3d.RotationX(23.5f * (float)RC);
        //static Matrix eclipticTilt = Matrix.Identity;

        static int lastPlanetCenterID = -2;
        static int orbitalSampleRate = 256;
        static double obliquity = 23.5 * RC;
        static Mutex OrbitsMutex = new Mutex();

        public static void UpdateOrbits(int planetCenter)
        {
            try
            {
                OrbitsMutex.WaitOne();

                obliquity = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) * RC;
                if (planetCenter != lastPlanetCenterID)
                {
                    orbits = null;
                }
                lastPlanetCenterID = planetCenter;
                if (orbits == null)
                {
                    if (planetCenter < 0)
                    {
                        eclipticTilt = Matrix3d.Identity;
                    }
                    else
                    {
                        eclipticTilt = Matrix3d.RotationX((float)obliquity);
                    }

                    if (planetOrbitalYears == null)
                    {
                        planetOrbitalYears = new double[20];
                        planetOrbitalYears[0] = 1;
                        planetOrbitalYears[1] = .241;
                        planetOrbitalYears[2] = .615;
                        planetOrbitalYears[3] = 1.881;
                        planetOrbitalYears[4] = 11.87;
                        planetOrbitalYears[5] = 29.45;
                        planetOrbitalYears[6] = 84.07;
                        planetOrbitalYears[7] = 164.9;
                        planetOrbitalYears[8] = 248.1;
                        planetOrbitalYears[9] = 27.3 / 365.25;
                        planetOrbitalYears[10] = 16.6890184 / 365.25;
                        planetOrbitalYears[11] = 3.551181 / 365.25;
                        planetOrbitalYears[12] = 7.15455296 / 365.25;
                        planetOrbitalYears[13] = 16.6890184 / 365.25;
                        planetOrbitalYears[19] = 1;

                    }
                  //  if (!ReadOrbits())
                    {
                        orbits = new Vector3d[20, orbitalSampleRate];

                        for (int i = 1; i < 20; i++)
                        {
                            if (i < 9 || i == 19)
                            {
                                for (int j = 0; j < orbitalSampleRate; j++)
                                {
                                    int centerId = planetCenter;
                                    double now = jNow + ((planetOrbitalYears[i] * 365.25 / (orbitalSampleRate)) * (double)(j - (orbitalSampleRate / 2)));
                                    Vector3d center = new Vector3d(0, 0, 0);

                                    if (i == (int)SolarSystemObjects.Moon)
                                    {
                                        centerId = -1;
                                    }
                                    else if (i > 9 && i < 14)
                                    {
                                        centerId = (int)SolarSystemObjects.Jupiter;
                                    }

                                    if (centerId > -1)
                                    {
                                        AstroRaDec centerRaDec = AstroCalc.AstroCalc.GetPlanet(now, centerId, 0, 0, -6378149);
                                        center = Coordinates.RADecTo3d(centerRaDec.RA, centerRaDec.Dec, centerRaDec.Distance);
                                    }


                                    if (i != 19)
                                    {
                                        AstroRaDec planetRaDec = AstroCalc.AstroCalc.GetPlanet(now, i, 0, 0, -6378149);
                                        // todo move to double precition for less trucation
                                        orbits[i, j] = Coordinates.RADecTo3d(planetRaDec.RA, planetRaDec.Dec, planetRaDec.Distance);
                                        orbits[i, j].Subtract(center);
                                    }
                                    else
                                    {
                                        orbits[i, j] = new Vector3d(-center.X, -center.Y, -center.Z);
                                    }
                                    //obliquity = Coordinates.MeanObliquityOfEcliptic(jNow) * RC;

                                    // todo is the tilt right?
                                    //if (i != (int)SolarSystemObjects.Moon && !((i > 9 && i < 14)))
                                    {
                                        orbits[i, j].RotateX(obliquity);
                                    }
                                }
                                orbits[i, orbitalSampleRate - 1] = orbits[i, 0];
                            }
                        }
                     //   DumpOrbitsFile();
                    }
                }
            }
            finally
            {
                OrbitsMutex.ReleaseMutex();
            }
        }

        public static bool ReadOrbits()
        {

            string filename = Properties.Settings.Default.CahceDirectory + @"data\orbits.bin";
            DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=orbitsbin", filename, false, true);
            FileStream fs = null;
            BinaryReader br = null;
            long len = 0;
            try
            {
                fs = new FileStream(filename, FileMode.Open);
                len = fs.Length;
                br = new BinaryReader(fs);
                orbits = new Vector3d[20, orbitalSampleRate];

                for (int i = 1; i < 20; i++)
                {
                    if (i < 9 || i == 19)
                    {
                        for (int j = 0; j < orbitalSampleRate; j++)
                        {

                            orbits[i, j] = new Vector3d(br.ReadDouble(), br.ReadDouble(), br.ReadDouble());
                        }
                    }
                }
            }
            catch
            {
                orbits = null;
                return false;
            }
            finally
            {
                if (br != null)
                {
                    br.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
            return true;
        }


        public static void DumpOrbitsFile()
        {
            string filename = Properties.Settings.Default.CahceDirectory + @"data\orbits.bin";

            if (orbits != null)
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                for (int i = 1; i < 20; i++)
                {
                    if (i < 9 || i == 19)
                    {
                        for (int j = 0; j < orbitalSampleRate; j++)
                        {
                            bw.Write(orbits[i, j].X);
                            bw.Write(orbits[i, j].Y);
                            bw.Write(orbits[i, j].Z);
                        }
                    }
                }
                bw.Close();
                fs.Close();
            }
        }

        public static bool DrawPlanets(RenderContext11 renderContext, float opacity)
        {
            if (planetTextures == null)
            {
                LoadPlanetTextures();
            }

            // Get Moon Phase
            double elong = GeocentricElongation(planetLocations[9].RA, planetLocations[9].Dec, planetLocations[0].RA, planetLocations[0].Dec);
            double raDif = planetLocations[9].RA - planetLocations[0].RA;
            if (planetLocations[9].RA < planetLocations[0].RA)
            {
                raDif += 24;
            }
            double phaseAngle = PhaseAngle(elong, planetLocations[9].Distance, planetLocations[0].Distance);
            double limbAngle = PositionAngle(planetLocations[9].RA, planetLocations[9].Dec, planetLocations[0].RA, planetLocations[0].Dec);

            if (raDif < 12)
            {
                phaseAngle += 180;
            }

            // Check for solar eclipse

            double dista = (Math.Abs(planetLocations[9].RA - planetLocations[0].RA) * 15) * Math.Cos(Coordinates.DegreesToRadians(planetLocations[0].Dec));
            double distb = Math.Abs(planetLocations[9].Dec - planetLocations[0].Dec);
            double sunMoonDist = Math.Sqrt(dista * dista + distb * distb);

            double coronaOpacity = 0;

            double moonEffect = (planetScales[9] / 2 - sunMoonDist);

            int darkLimb = Math.Min(6, (int)(sunMoonDist * 6));

            if (moonEffect > (planetScales[0] / 4))
            {
                coronaOpacity = Math.Min(1.0, (moonEffect - (planetScales[0] / 4)) / .001);
                DrawPlanet(renderContext, 18, (float)coronaOpacity * opacity);
            }


            foreach (int planetId in planetDrawOrder.Values)
            {
                if (planetId == 9)
                {
                    DrawPlanetPhase(renderContext, planetId, phaseAngle, limbAngle, darkLimb, opacity);
                }
                else
                {
                    DrawPlanet(renderContext, planetId, opacity);
                }

            }

            
            return true;
        }

        private static void LoadPlanetTextures()
        {
            

            planetTextures = new Texture11[20];
            planetTextures[0] = LoadPlanetTexture(Properties.Resources.sun);
            planetTextures[1] = LoadPlanetTexture(Properties.Resources.mercury);
            planetTextures[2] = LoadPlanetTexture(Properties.Resources.venus);
            planetTextures[3] = LoadPlanetTexture(Properties.Resources.mars);
            planetTextures[4] = LoadPlanetTexture(Properties.Resources.jupiter);
            planetTextures[5] = LoadPlanetTexture(Properties.Resources.saturn);
            planetTextures[6] = LoadPlanetTexture(Properties.Resources.uranus);
            planetTextures[7] = LoadPlanetTexture(Properties.Resources.neptune);
            planetTextures[8] = LoadPlanetTexture(Properties.Resources.pluto);
            planetTextures[9] = LoadPlanetTexture(Properties.Resources.moon);
            planetTextures[10] = LoadPlanetTexture(Properties.Resources.io);
            planetTextures[11] = LoadPlanetTexture(Properties.Resources.europa);
            planetTextures[12] = LoadPlanetTexture(Properties.Resources.ganymede);
            planetTextures[13] = LoadPlanetTexture(Properties.Resources.callisto);
            planetTextures[14] = LoadPlanetTexture(Properties.Resources.pointsourcemoon);
            planetTextures[15] = LoadPlanetTexture(Properties.Resources.moonshadow);
            planetTextures[18] = LoadPlanetTexture(Properties.Resources.sunCorona);
            planetTextures[19] = LoadPlanetTexture(Properties.Resources.earth);
        }

        private static void LoadPlanetTextureMaps()
        {
            planetTexturesMaps = new Texture11[22];
            planetTexturesMaps[0] = LoadPlanetTexture(Properties.Resources.sunMap);
            planetTexturesMaps[1] = LoadPlanetTexture(Properties.Resources.mercuryMap);
            planetTexturesMaps[2] = LoadPlanetTexture(Properties.Resources.venusMap);
            planetTexturesMaps[3] = LoadPlanetTexture(Properties.Resources.marsMap);
            planetTexturesMaps[4] = LoadPlanetTexture(Properties.Resources.jupiterMap);
            planetTexturesMaps[5] = LoadPlanetTexture(Properties.Resources.saturnMap);
            planetTexturesMaps[6] = LoadPlanetTexture(Properties.Resources.uranusMap);
            planetTexturesMaps[7] = LoadPlanetTexture(Properties.Resources.neptuneMap);
            planetTexturesMaps[8] = LoadPlanetTexture(Properties.Resources.plutoMap);
            planetTexturesMaps[9] = LoadPlanetTexture(Properties.Resources.moonMap);
            planetTexturesMaps[10] = LoadPlanetTexture(Properties.Resources.ioMap);
            planetTexturesMaps[11] = LoadPlanetTexture(Properties.Resources.europaMap);
            planetTexturesMaps[12] = LoadPlanetTexture(Properties.Resources.ganymedeMap);
            planetTexturesMaps[13] = LoadPlanetTexture(Properties.Resources.callistoMap);
            planetTexturesMaps[19] = LoadPlanetTexture(Properties.Resources.earthMap);
            planetTexturesMaps[20] = LoadPlanetTexture(Properties.Resources.earthMapNight);
            planetTexturesMaps[21] = LoadPlanetTexture(Properties.Resources.earthsMapSpec);
        }
        static SortedList<double, int> drawOrder = new SortedList<double, int>();
        public static bool DrawPlanets3D(RenderContext11 renderContext, float opacity, Vector3d centerPoint)
        {
            InitPlanetResources();

            renderContext.LightingEnabled = Settings.Active.SolarSystemLighting;
            
            if (!Properties.Settings.Default.SolarSystemPlanets.TargetState)
            {
                return true;
            }

            drawOrder.Clear();
            
            Vector3d camera = new Vector3d(renderContext.CameraPosition);
            for (int planetId = 0; planetId < 14; planetId++)
            {
                if (!planetLocations[planetId].Eclipsed)
                {
                    Vector3d distVector = camera - (planet3dLocations[planetId] - centerPoint);

                    if (!drawOrder.ContainsKey(-distVector.Length()))
                    {
                        drawOrder.Add(-distVector.Length(), planetId);
                    }
                }
            }

            Vector3d distVectorEarth = camera - (planet3dLocations[(int)SolarSystemObjects.Earth] - centerPoint);
            if (!drawOrder.ContainsKey(-distVectorEarth.Length()))
            {
                drawOrder.Add(-distVectorEarth.Length(), (int)SolarSystemObjects.Earth);
            }
            renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;

            renderContext.setRasterizerState(TriangleCullMode.CullCounterClockwise);
            foreach (int planetId in drawOrder.Values)
            {
                DrawPlanet3d(renderContext, planetId, centerPoint, opacity);               
            }

            double distss = UiTools.SolarSystemToMeters(Earth3d.MainWindow.SolarSystemCameraDistance);


            //double val1 = Math.Log(200000000, 10)-7.3;
            //double val2 = Math.Log(20000000, 10)-7.3;

            float moonFade = (float)Math.Min(1, Math.Max(Math.Log(distss, 10) - 7.3, 0));
            

            float fade = (float)Math.Min(1, Math.Max(Math.Log(distss, 10) - 8.6, 0));
            //
            // Sync orbit state

            if (Settings.Active.PlanetOrbitsFilter != Properties.Settings.Default.PlanetOrbitsFilter)
            {
                Properties.Settings.Default.PlanetOrbitsFilter = Settings.Active.PlanetOrbitsFilter;
            }

            if (Properties.Settings.Default.SolarSystemOrbits.State ) // && fade > 0)
            {
                renderContext.DepthStencilMode = DepthStencilMode.ZReadOnly;
                renderContext.BlendMode = BlendMode.Alpha;
                renderContext.setRasterizerState(TriangleCullMode.Off, false);
                
                int orbitColor = Color.FromArgb((int)(Properties.Settings.Default.SolarSystemOrbits.Opacity * 255), Properties.Settings.Default.SolarSystemOrbitColor).ToArgb();                
                
                for (int ii = 1; ii < 10; ii++)
                {
                    int id = ii;

                    if (ii == 9)
                    {
                        id = 19;
                    }

                    double angle = Math.Atan2(planet3dLocations[id].Z, planet3dLocations[id].X);


                    orbitBlendStates[id].TargetState = (Properties.Settings.Default.PlanetOrbitsFilter & (int)Math.Pow(2, id)) != 0;

                    if (orbitBlendStates[id].State)
                    {
                        if (Properties.Settings.Default.UsePlanetColorsForOrbits)
                        {
                            DrawSingleOrbit(renderContext, UiTools.GetTransparentColor(planetColors[id].ToArgb(), Properties.Settings.Default.SolarSystemOrbits.Opacity * fade * orbitBlendStates[id].Opacity), id, centerPoint, angle, planet3dLocations[id]);
                        }
                        else
                        {
                            DrawSingleOrbit(renderContext, orbitColor, id, centerPoint, angle, planet3dLocations[id]);
                        }
                    }
                }

                // Show the orbit of the Moon
                orbitBlendStates[(int)SolarSystemObjects.Moon].TargetState = (Properties.Settings.Default.PlanetOrbitsFilter & (int)Math.Pow(2, (int)SolarSystemObjects.Moon)) != 0;
                if (orbitBlendStates[(int)SolarSystemObjects.Moon].State)
                {

                    int id = (int)SolarSystemObjects.Moon;
                    DrawSingleOrbit(renderContext, UiTools.GetTransparentColor(planetColors[id].ToArgb(), Properties.Settings.Default.SolarSystemOrbits.Opacity * moonFade * orbitBlendStates[(int)SolarSystemObjects.Moon].Opacity), id, centerPoint, 0.0, planet3dLocations[id]);
                }

                // Show orbits of the Galilean satellites
                {
                    double deltaT = 1.0 / 1440.0 * 0.1;

                    // Compute the positions of the Galilean satellites at two times; we need
                    // the second in order to estimate the velocity.
                    CAAGalileanMoonsDetails gal0 = CAAGalileanMoons.Calculate(jNow);
                    CAAGalileanMoonsDetails gal1 = CAAGalileanMoons.Calculate(jNow - deltaT);
                    CAA3DCoordinate[] position0 = 
                    { gal0.Satellite1.EclipticRectangularCoordinates,
                      gal0.Satellite2.EclipticRectangularCoordinates,
                      gal0.Satellite3.EclipticRectangularCoordinates,
                      gal0.Satellite4.EclipticRectangularCoordinates };
                    CAA3DCoordinate[] position1 = 
                    { gal1.Satellite1.EclipticRectangularCoordinates,
                      gal1.Satellite2.EclipticRectangularCoordinates,
                      gal1.Satellite3.EclipticRectangularCoordinates,
                      gal1.Satellite4.EclipticRectangularCoordinates };

                    SolarSystemObjects[] galileans = { SolarSystemObjects.Io, SolarSystemObjects.Europa, SolarSystemObjects.Ganymede, SolarSystemObjects.Callisto };
                    for (int i = 0; i < 4; ++i)
                    {
                        int id = (int)galileans[i];
                        orbitBlendStates[id].TargetState = (Properties.Settings.Default.PlanetOrbitsFilter & (int)Math.Pow(2, id)) != 0;
                        if (orbitBlendStates[id].State)
                        {
                            // Estimate velocity through differences
                            Vector3d r0 = new Vector3d(position0[i].X, position0[i].Z, position0[i].Y);
                            Vector3d r1 = new Vector3d(position1[i].X, position1[i].Z, position1[i].Y);

                            Vector3d v = (r0 - r1) / deltaT;

                            KeplerianElements elements = stateVectorToKeplerian(r0, v, muJupiter);

                            DrawSingleOrbit(renderContext, UiTools.GetTransparentColor(planetColors[id].ToArgb(), Properties.Settings.Default.SolarSystemOrbits.Opacity * moonFade * orbitBlendStates[id].Opacity), id, centerPoint, 0.0, planet3dLocations[id], elements);
                        }
                    }
                }

                renderContext.setRasterizerState(TriangleCullMode.Off, true);
            }

            renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;
            renderContext.BlendMode = BlendMode.None;

            return true;
        }

        public static void InitPlanetResources()
        {
            OrbitsMutex.WaitOne();
            try
            {
                if (planetTexturesMaps == null)
                {
                    LoadPlanetTextureMaps();
                }
                if (sphereIndexBuffers == null)
                {
                    InitSphere();
                }

                if (ringsVertexBuffer == null)
                {
                    InitRings();
                }
            }
            finally
            {
                OrbitsMutex.ReleaseMutex();
            }
        }

        // Compute the brightness of the sky due to atmospheric scattering. This function returns
        // a value between 0 (completely dark) and 1 (full brightness) rather than a physical quantity
        // such as radiance. The brightness factor may be used to adjust the opacity of layers containing
        // stars, the Milky Way, and other astronomical objects that are not visible during the daytime sky.
        public static float CalculateSkyBrightnessFactor(Matrix3d viewMatrix, Vector3d centerPoint)
        {
            double skyBrightness = 0.0;

            if (Settings.Active.ShowEarthSky)
            {
                SolarSystemObjects[] bodiesWithAtmospheres = { SolarSystemObjects.Earth, SolarSystemObjects.Mars };

                // Get the position of the camera in heliocentric coordinates
                Matrix3d m = viewMatrix;
                m.Invert();
                Vector3d centerOffset = m.Transform(Vector3d.Empty);
                Vector3d heliocentricCameraPosition = centerOffset - -centerPoint;

                foreach (SolarSystemObjects planetID in bodiesWithAtmospheres)
                {
                    Vector3d sunPosition = (planet3dLocations[0] - planet3dLocations[(int) planetID]);

                    // Get the camera's altitude and the phase angle (which tells us whether it's day or night)
                    Vector3d cameraToPlanet = heliocentricCameraPosition - planet3dLocations[(int) planetID];
                    double altitudeKm = cameraToPlanet.Length() * UiTools.KilometersPerAu - GetPlanetRadiusInMeters((int) planetID) / 1000;
                    double cosSunAngle = Vector3d.Dot(cameraToPlanet, sunPosition) / (cameraToPlanet.Length() * sunPosition.Length());

                    double brightness = Math.Min(1.0, 20.0 * Math.Max(cosSunAngle + 0.05, 0.0)) * Math.Min(1.0, Math.Max(0.0, 1.0 - altitudeKm / 40.0));
                    skyBrightness = Math.Max(skyBrightness, brightness);
                }
            }

            return (float) skyBrightness;
        }


        // Get the position of a Solar System object using a 'direct' calculation that
        // avoids including an aberration correction.
        //
        // The returned position is in ecliptic coordinate system with the origin at the center
        // of the parent body (i.e. the Sun for planets, a planet for moons). The position of moons
        // is _not_ modified by the SolarSystemScale, making it possible to use function to
        // a calculate valid Keplerian elements.
        public static Vector3d GetPlanetPositionDirect(SolarSystemObjects id, double jd)
        {
            double L = 0.0; 
            double B = 0.0; 
            double R = 0.0; 

            switch (id)
            {
                case SolarSystemObjects.Mercury:
                    L = CAAMercury.EclipticLongitude(jd);
                    B = CAAMercury.EclipticLatitude(jd);
                    R = CAAMercury.RadiusVector(jd);
                    break;
                case SolarSystemObjects.Venus:
                    L = CAAVenus.EclipticLongitude(jd);
                    B = CAAVenus.EclipticLatitude(jd);
                    R = CAAVenus.RadiusVector(jd);
                    break;
                case SolarSystemObjects.Earth:
                    {
                        //double x = HiResTimer.TickCount;
                        L = CAAEarth.EclipticLongitude(jd);
                        B = CAAEarth.EclipticLatitude(jd);
                        R = CAAEarth.RadiusVector(jd);
                        //x = (HiResTimer.TickCount - x) / HiResTimer.Frequency;
                        //System.Console.WriteLine("Earth orbit time: " + x * 1000.0 + "ms");
                    }
                    break;
                case SolarSystemObjects.Mars:
                    L = CAAMars.EclipticLongitude(jd);
                    B = CAAMars.EclipticLatitude(jd);
                    R = CAAMars.RadiusVector(jd);
                    break;
                case SolarSystemObjects.Jupiter:
                    L = CAAJupiter.EclipticLongitude(jd);
                    B = CAAJupiter.EclipticLatitude(jd);
                    R = CAAJupiter.RadiusVector(jd);
                    break;
                case SolarSystemObjects.Saturn:
                    L = CAASaturn.EclipticLongitude(jd);
                    B = CAASaturn.EclipticLatitude(jd);
                    R = CAASaturn.RadiusVector(jd);
                    break;
                case SolarSystemObjects.Uranus:
                    L = CAAUranus.EclipticLongitude(jd);
                    B = CAAUranus.EclipticLatitude(jd);
                    R = CAAUranus.RadiusVector(jd);
                    break;
                case SolarSystemObjects.Neptune:
                    L = CAANeptune.EclipticLongitude(jd);
                    B = CAANeptune.EclipticLatitude(jd);
                    R = CAANeptune.RadiusVector(jd);
                    break;
                case SolarSystemObjects.Pluto:
                    L = CAAPluto.EclipticLongitude(jd);
                    B = CAAPluto.EclipticLatitude(jd);
                    R = CAAPluto.RadiusVector(jd);
                    break;
                case SolarSystemObjects.Moon:
                    L = CAAMoon.EclipticLongitude(jd);
				    B = CAAMoon.EclipticLatitude(jd);
				    R = CAAMoon.RadiusVector(jd)/149598000;
                    break;
                case SolarSystemObjects.Io:
                    {
                        CAAGalileanMoonsDetails galileanInfo = CAAGalileanMoons.Calculate(jd);
                        CAA3DCoordinate position = galileanInfo.Satellite1.EclipticRectangularCoordinates;
                        return new Vector3d(position.X, position.Z, position.Y);
                    }
                case SolarSystemObjects.Europa:
                    {
                        CAAGalileanMoonsDetails galileanInfo = CAAGalileanMoons.Calculate(jd);
                        CAA3DCoordinate position = galileanInfo.Satellite2.EclipticRectangularCoordinates;
                        return new Vector3d(position.X, position.Z, position.Y);
                    }
                case SolarSystemObjects.Ganymede:
                    {
                        CAAGalileanMoonsDetails galileanInfo = CAAGalileanMoons.Calculate(jd);
                        CAA3DCoordinate position = galileanInfo.Satellite3.EclipticRectangularCoordinates;
                        return new Vector3d(position.X, position.Z, position.Y);
                    }
                case SolarSystemObjects.Callisto:
                    {
                        CAAGalileanMoonsDetails galileanInfo = CAAGalileanMoons.Calculate(jd);
                        CAA3DCoordinate position = galileanInfo.Satellite4.EclipticRectangularCoordinates;
                        return new Vector3d(position.X, position.Z, position.Y);
                    }
            }

            // Enabling this code transforms planet positions from the mean ecliptic/equinox of
            // date to the J2000 ecliptic. It is necessary because the VSOP87D series used
            // for planet positions is in the mean-of-date frame. The transformation is currently
            // disabled in order to better match planet positions calculated elsewhere in the code.
            //CAA2DCoordinate prec = CAAPrecession.PrecessEcliptic(L, B, jd, 2451545.0);
            //L = prec.X;
            //B = prec.Y;

            L = CAACoordinateTransformation.DegreesToRadians(L);
            B = CAACoordinateTransformation.DegreesToRadians(B);
            Vector3d eclPos = new Vector3d(Math.Cos(L) * Math.Cos(B), Math.Sin(L) * Math.Cos(B), Math.Sin(B)) * R;

            // Transform from the ecliptic of date to the J2000 ecliptic; this transformation should be deleted
            // once the precession is turned one.
            double eclipticOfDateRotation = (Coordinates.MeanObliquityOfEcliptic(jd) - Coordinates.MeanObliquityOfEcliptic(2451545.0)) * RC;
            eclPos.RotateX(eclipticOfDateRotation);

            return new Vector3d(eclPos.X, eclPos.Z, eclPos.Y);
        }

        // Keplerian elements defined here use eccentric anomaly instead of mean anomaly and
        // have all orbital plane angles converted to a rotation matrix.
        private struct KeplerianElements
        {
            public double a;
            public double e;
            public double E;
            public Matrix3d orientation;
        }

        private static KeplerianElements stateVectorToKeplerian(Vector3d position, Vector3d velocity, double mu)
        {
            // Work in units of km and seconds
            Vector3d r = position * UiTools.KilometersPerAu;
            Vector3d v = velocity / 86400.0 * UiTools.KilometersPerAu;

            double rmag = r.Length();
            double vmag = v.Length();

            double sma = 1.0 / (2.0 / rmag - vmag * vmag / mu);

            // h is the orbital angular momentum vector
            Vector3d h = Vector3d.Cross(r, v);

            // ecc is the eccentricity vector, which points from the
            // planet at periapsis to the center point.
            Vector3d ecc = Vector3d.Cross(v, h) / mu - r / rmag;
            double e = ecc.Length();

            h.Normalize();
            ecc.Normalize();

            // h, s, and ecc are orthogonal vectors that define a coordinate
            // system. h is normal to the orbital plane.
            Vector3d s = Vector3d.Cross(h, ecc);

            // Calculate the sine and cosine of the true anomaly
            r.Normalize();
            double cosNu = Vector3d.Dot(ecc, r);
            double sinNu = Vector3d.Dot(s, r);

            // Compute the eccentric anomaly
            double E = Math.Atan2(Math.Sqrt(1 - e * e) * sinNu, e + cosNu);

            // Mean anomaly not required
            // double M = E - e * Math.Sin(E);

            KeplerianElements elements = new KeplerianElements();

            // Create a rotation matrix given the three orthogonal vectors:
            //   ecc - eccentricity vector
            //   s   - in the orbital plane, perpendicular to ecc
            //   h   - angular momentum vector, normal to orbital plane
            elements.orientation = new Matrix3d(ecc.X, ecc.Y, ecc.Z, 0.0,
                                                s.X, s.Y, s.Z, 0.0,
                                                h.X, h.Y, h.Z, 0.0,
                                                0.0, 0.0, 0.0, 1.0);
            elements.a = sma;
            elements.e = e;
            elements.E = E;

            return elements;
        }


        private static void DrawSingleOrbit(RenderContext11 renderContext, int eclipticColor, int id, Vector3d centerPoint, double xstartAngle, Vector3d planetNow)
        {
            double mu = 0.0;
            switch (id)
            {
                case (int)SolarSystemObjects.Moon:
                    mu = muEarth + muMoon;
                    break;

                case (int)SolarSystemObjects.Io:
                case (int)SolarSystemObjects.Europa:
                case (int)SolarSystemObjects.Ganymede:
                case (int)SolarSystemObjects.Callisto:
                    mu = muJupiter;
                    break;

                default:
                    mu = muSun;
                    break;
            }

            // Estimate velocity through differences
            double deltaT = 1.0 / 1440.0 * 0.1;
            Vector3d r0 = GetPlanetPositionDirect((SolarSystemObjects)id, jNow);
            Vector3d r1 = GetPlanetPositionDirect((SolarSystemObjects)id, jNow - deltaT);

            Vector3d v = (r0 - r1) / deltaT;

            KeplerianElements elements = stateVectorToKeplerian(r0, v, mu);

            DrawSingleOrbit(renderContext, eclipticColor, id, centerPoint, xstartAngle, planetNow, elements);
        }


        private static void DrawSingleOrbit(RenderContext11 renderContext, int eclipticColor, int id, Vector3d centerPoint, double xstartAngle, Vector3d planetNow, KeplerianElements el)
        {
            double scaleFactor;
            switch (id)
            {
                case (int)SolarSystemObjects.Moon:
                    if (Settings.Active.SolarSystemScale > 1)
                        scaleFactor = Settings.Active.SolarSystemScale / 2;
                    else
                        scaleFactor = 1.0;
                    break;

                case (int)SolarSystemObjects.Io:
                case (int)SolarSystemObjects.Europa:
                case (int)SolarSystemObjects.Ganymede:
                case (int)SolarSystemObjects.Callisto:
                    scaleFactor = Settings.Active.SolarSystemScale;
                    break;

                default:
                    scaleFactor = 1.0;
                    break;
            }

            Vector3d translation = -centerPoint;
            if (id == (int)SolarSystemObjects.Moon)
            {
                translation += planet3dLocations[(int)SolarSystemObjects.Earth];
            }
            else if (id == (int)SolarSystemObjects.Io || id == (int)SolarSystemObjects.Europa || id == (int)SolarSystemObjects.Ganymede || id == (int)SolarSystemObjects.Callisto)
            {
                translation += planet3dLocations[(int)SolarSystemObjects.Jupiter];
            }

            Vector3d currentPosition = planetNow - centerPoint;
            if (orbitTraces[id] != null)
            {
                Matrix3d worldMatrix = Matrix3d.Translation(translation) * renderContext.World;
                orbitTraces[id].render(renderContext, Color.FromArgb(eclipticColor), worldMatrix, jNow, currentPosition, 0.0);
            }
            else
            {
                Matrix3d worldMatrix = el.orientation * Matrix3d.Translation(translation) * renderContext.World;
                EllipseRenderer.DrawEllipse(renderContext, el.a / UiTools.KilometersPerAu * scaleFactor, el.e, el.E, Color.FromArgb(eclipticColor), worldMatrix, currentPosition);
            }
        }


        static DateTime lastUpdate = new DateTime();
     

        public static bool Lighting = true;
        public static bool IsPlanetInFrustum(RenderContext11 renderContext, float rad)
        {
            rad *= 2;
            PlaneD[] frustum = renderContext.Frustum;
            Vector3d center = new Vector3d(0, 0, 0);
            Vector4d centerV4 = new Vector4d(0, 0, 0, 1f);
            for (int i = 0; i < 6; i++)
            {
                if (frustum[i].Dot(centerV4) + rad < 0)
                {
                    return false;
                }
            }
            return true;
        }

        private static void RestoreDefaultMaterialState(RenderContext11 renderContext)
        {
            //todo11 new state management required or not...
            //Device device = renderContext.Device;
            //device.Material = UiTools.DefaultMaterial;
            //device.RenderState.SpecularEnable = false;
            //device.VertexShader = null;
            //device.PixelShader = null;
        }


        // Returns a shader used to draw the planet surface (or null if no shader was used)
        public static PlanetShader11 SetupPlanetSurfaceEffect(RenderContext11 renderContext, PlanetShaderKey key, float opacity)
        {
            
            return SetupPlanetSurfaceEffectShader(renderContext, key, opacity);
            
        }

        private static PlanetShader11 SetupPlanetSurfaceEffectShader(RenderContext11 renderContext, PlanetShaderKey key, float opacity)
        {

            PlanetShader11 shader = PlanetShader11.GetPlanetShader(renderContext.Device, key);

            // If we've got a shader, make it active on the device and set the
            // shader constants.
            if (shader != null)
            {
                shader.use(renderContext.devContext);

                // Set the combined world/view/projection matrix in the shader
                Matrix3d worldMatrix = renderContext.World;
                Matrix3d viewMatrix = renderContext.View;

                SharpDX.Matrix wvp = (worldMatrix * viewMatrix * renderContext.Projection).Matrix11;
                shader.WVPMatrix = wvp;
                shader.DiffuseColor = new SharpDX.Vector4(1.0f, 1.0f, 1.0f, opacity);

                Matrix3d invWorld = worldMatrix;
                invWorld.Invert();
                
                // For view-dependent lighting (e.g. specular), we need the position of the camera
                // in the planet-fixed coordinate system.
                Matrix3d worldViewMatrix = worldMatrix * viewMatrix;
                Matrix3d invWorldView = worldViewMatrix;
                invWorldView.Invert();

                Vector3d cameraPositionObj = Vector3d.TransformCoordinate(new Vector3d(0.0, 0.0, 0.0), invWorldView);
                shader.CameraPosition = cameraPositionObj.Vector311;
            }

            renderContext.Shader = shader;

            return shader;
        }
             
        private static float GetPlanetAtmosphereScaleHeight(int planetID)
        {
            float h0 = 0.0f;

            if (planetID == 0x13)
            {
                h0 = 8.0f;
            }
            else if (planetID == 3)
            {
                h0 = 8.0f;
            }

            // 2x real scale height for improved appearance when gamma
            // correction isn't available
            if (!RenderContext11.sRGB)
            {
                h0 *= 2.0f;
            }

            return h0;
        }


        private static float CalcSkyGeometryHeight(int planetID)
        {
            // The sky shell geometry needs to extend out to the point where the atmosphere
            // becomes invisible. The atmosphere density falls off exponentially with height,
            // and we use this to estimate the sky shell radius. The radius would need to be
            // increased for atmospheres that are extremely dense at the surface.
            float minVisible = RenderContext11.sRGB ? (1.0f / 2048.0f) : (1.0f / 256.0f);
            float planetRadiusKm = (float)GetPlanetRadiusInMeters(planetID) / 1000.0f;
            return (float)(-GetPlanetAtmosphereScaleHeight(planetID) * Math.Log(minVisible)) / planetRadiusKm;
        }

        // Set shader constants for rendering with atmospheric scattering enabled.
        // 
        // geometryScaleFactor is generally 1.0, but should be set to some other value
        // when the world*view transformation includes a scale factor. This is the case
        // when drawing the sky geometry, which is just WWT's standard unit sphere
        // scaled up slightly.
        private static void SetAtmosphereConstants(RenderContext11 renderContext, int planetID, float geometryScaleFactor, float skyHeight)
        {
            if (renderContext.Shader == null)
            {
                // Atmospheres are only drawn when shaders are enabled
                return;
            }
            
            // Per-planet atmosphere constants. Rayleigh scattering coefficients are realistic for Earth
            // but invented for Mars. The fraction of light scattered per unit length is proportional to
            // 1/wavelength^4 in Rayleigh scattering. The invented constants for Mars are meant to simulate
            // the pinkish color of the Martian atmosphere, which is actually due to suspended dust particles
            // rather than Rayleigh scattering.
            SharpDX.Vector3 earthRayleighCoeff = new SharpDX.Vector3(5.8f, 13.5f, 33.1f) * 1.8f;
            SharpDX.Vector3 marsRayleighCoeff = new SharpDX.Vector3(13.0f, 9.5f, 8.1f) * 0.85f;

            float planetRadiusKm = (float)GetPlanetRadiusInMeters(planetID) / 1000.0f;

            // Calculate atmosphere constants
            float atmosphereScaleHeight = GetPlanetAtmosphereScaleHeight(planetID) / planetRadiusKm;
            SharpDX.Vector3 rayleighCoeff = earthRayleighCoeff;
            if (planetID == 3)
            {
                rayleighCoeff = marsRayleighCoeff;
            }

            PlanetShader11 atmosphereShader = renderContext.Shader;
            atmosphereShader.AtmosphereHeight = skyHeight;
            atmosphereShader.AtmosphereZeroHeight = 1.0f / geometryScaleFactor;
            atmosphereShader.AtmosphereInvScaleHeight = 1.0f / atmosphereScaleHeight; ;
            atmosphereShader.RayleighScatterCoeff = rayleighCoeff;
            atmosphereShader.AtmosphereCenter = new SharpDX.Vector3();
        }

        private static SharpDX.Matrix bias = SharpDX.Matrix.Scaling(new SharpDX.Vector3(.5f, .5f, .5f)) * SharpDX.Matrix.Translation(new SharpDX.Vector3(.5f, .5f, .5f));
        private static void DrawPlanet3d(RenderContext11 renderContext, int planetID, Vector3d centerPoint, float opacity)
        {
            // Assume that KML is only used for Earth
            KmlLabels kmlMarkers = null;
            if (planetID == (int)SolarSystemObjects.Earth)
            {
                kmlMarkers = Earth3d.MainWindow.KmlMarkers;
                if (kmlMarkers != null)
                {
                    kmlMarkers.ClearGroundOverlays();
                }
            }

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldBase = renderContext.WorldBase;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;

            double radius = GetAdjustedPlanetRadius(planetID);
                    
            SetupPlanetMatrix(renderContext, planetID, centerPoint, true);

            LayerManager.PreDraw(renderContext, 1.0f, false, Enum.GetName(typeof(SolarSystemObjects), (SolarSystemObjects)planetID), true);

            float planetWidth = 1;

            if (planetID == (int)SolarSystemObjects.Saturn)
            {
                planetWidth = 3;
            }

            if (IsPlanetInFrustum(renderContext, planetWidth))
            {
                // Save all matrices modified by SetupMatrix...
                Matrix3d matOld2 = renderContext.World;
                Matrix3d matOldBase2 = renderContext.WorldBase;
                Matrix3d matOldNonRotating2 = renderContext.WorldBaseNonRotating;

                renderContext.World = matOld;
                renderContext.WorldBase = matOldBase;
                renderContext.WorldBaseNonRotating = matOldNonRotating;
                SetupMatrixForPlanetGeometry(renderContext, planetID, centerPoint, true);

                Vector3d loc = planet3dLocations[planetID] - centerPoint;
                loc.Subtract(renderContext.CameraPosition);
                double dist = loc.Length();
                double sizeIndexParam = (2 * Math.Atan(.5 * (radius / dist))) / Math.PI * 180;

                // Calculate pixelsPerUnit which is the number of pixels covered
                // by an object 1 AU at the distance of the planet center from
                // the camera. This calculation works regardless of the projection
                // type.
                float viewportHeight = renderContext.ViewPort.Height;
                double p11 = renderContext.Projection.M11;
                double p34 = renderContext.Projection.M34;
                double p44 = renderContext.Projection.M44;
                double w = Math.Abs(p34) * dist + p44;
                float pixelsPerUnit = (float)(p11 / w) * viewportHeight;
                float planetRadiusInPixels = (float)(radius * pixelsPerUnit);

                int sizeIndex = 0;
                if (sizeIndexParam > 10.5)
                {
                    sizeIndex = 0;
                }
                else if (sizeIndexParam > 3.9)
                {
                    sizeIndex = 1;
                }
                else if (sizeIndexParam > .72)
                {
                    sizeIndex = 2;
                }
                else if (sizeIndexParam > 0.057)
                {
                    sizeIndex = 3;
                }
                else
                {
                    sizeIndex = 4;
                }

                int eclipseShadowCount = 0;
                bool hasRingShadowsOnPlanet = false;

                // No shadows should be drawn if Solar System Lighting is OFF
                if (Settings.Active.SolarSystemLighting)
                {
                    // Eclipse shadow setup for Earth
                    if (planetID == (int)SolarSystemObjects.Earth)
                    {
                        if (sizeIndex < 2)
                        {
                            float width = Settings.Active.SolarSystemScale * .00001f;

                            SetupShadow(renderContext, centerPoint, width, SolarSystemObjects.Moon, 0);

                            eclipseShadowCount = 1;
                        }
                    }

                    if (planetID == (int)SolarSystemObjects.Moon)
                    {
                        if (sizeIndex < 4)
                        {
                            double earthDist = Math.Abs((planet3dLocations[(int)SolarSystemObjects.Sun] - planet3dLocations[(int)SolarSystemObjects.Earth]).Length());
                            double moonDist = Math.Abs((planet3dLocations[(int)SolarSystemObjects.Sun] - planet3dLocations[(int)SolarSystemObjects.Moon]).Length());

                            if (moonDist > earthDist)
                            {

                                float width = Settings.Active.SolarSystemScale * .00028f;
                                SetupShadow(renderContext, centerPoint, width, SolarSystemObjects.Earth, 0);
                                eclipseShadowCount = 1;
                            }
                        }

                    }


                    // Eclipse shadow setup for Jupiter
                    // Shadow widths based only on moon diameter in relation to Moon diameter
                    if (planetID == (int)SolarSystemObjects.Jupiter && sizeIndex < 3)
                    {
                        int p = 0;
                        float width;

                        if (planetLocations[(int)SolarSystemObjects.Callisto].Shadow)
                        {
                            width = Settings.Active.SolarSystemScale * .0000139f;
                            SetupShadow(renderContext, centerPoint, width, SolarSystemObjects.Callisto, p++);
                        }

                        if (planetLocations[(int)SolarSystemObjects.Ganymede].Shadow)
                        {
                            width = Settings.Active.SolarSystemScale * .0000152f;
                            SetupShadow(renderContext, centerPoint, width, SolarSystemObjects.Ganymede, p++);
                        }

                        if (planetLocations[(int)SolarSystemObjects.Io].Shadow)
                        {
                            width = Settings.Active.SolarSystemScale * .0000105f;
                            SetupShadow(renderContext, centerPoint, width, SolarSystemObjects.Io, p++);
                        }

                        if (planetLocations[(int)SolarSystemObjects.Europa].Shadow)
                        {
                            width = Settings.Active.SolarSystemScale * .000009f;
                            SetupShadow(renderContext, centerPoint, width, SolarSystemObjects.Europa, p++);
                        }

                        eclipseShadowCount = p;
                    }

                    // Ring shadows on Saturn
                    if (planetID == (int)SolarSystemObjects.Saturn && Settings.Active.SolarSystemLighting)
                    {
                        hasRingShadowsOnPlanet = true;
                    }

                    //ShadowStuff end
                }

                // Get the position of the Sun relative to the planet center; the position is
                // in the world coordinate frame.
                Vector3d sunPosition = (planet3dLocations[0] - planet3dLocations[planetID]);
                renderContext.SunPosition = sunPosition;

                if (sizeIndex < 4)
                {
                    bool atmosphereOn = (planetID == 19 || planetID == 3) && Settings.Active.ShowEarthSky;

                    if (planetID == 0 && sizeIndex > 1)
                    {
                        //todo11
                        //device.RenderState.Clipping = false;
                        DrawPointPlanet(renderContext, new Vector3d(0, 0, 0), (float)Math.Min(1.6, sizeIndexParam) * 2, planetColors[planetID], true, opacity);
                        
                        //todo11
                        //device.RenderState.Clipping = true;
                    }

                    PlanetSurfaceStyle surfaceStyle = PlanetSurfaceStyle.Diffuse;
                    if (planetID == (int)SolarSystemObjects.Sun || !Settings.Active.SolarSystemLighting)
                    {
                        surfaceStyle = PlanetSurfaceStyle.Emissive;
                    }
                    else if (planetID == (int)SolarSystemObjects.Moon || planetID == (int)SolarSystemObjects.Mercury)
                    {
                        // Use Lommel-Seeliger photometric model for regolith covered bodies
                        //   surfaceStyle = PlanetSurfaceStyle.LommelSeeliger;
                    }

                    float skyGeometryHeight = CalcSkyGeometryHeight(planetID);

                    PlanetShaderKey shaderKey = new PlanetShaderKey(surfaceStyle, atmosphereOn, eclipseShadowCount);
                    shaderKey.HasRingShadows = hasRingShadowsOnPlanet;
                    if (kmlMarkers != null)
                    {
                        shaderKey.overlayTextureCount = Math.Min(kmlMarkers.GroundOverlayCount, PlanetShader11.MaxOverlayTextures);
                    }

                    SetupPlanetSurfaceEffect(renderContext, shaderKey, opacity);
                    renderContext.LocalCenter = Vector3d.Empty;

                    // Set up any overlay textures
                    if (shaderKey.overlayTextureCount > 0)
                    {
                        if (renderContext.ShadersEnabled)
                        {
                            kmlMarkers.SetupGroundOverlays(renderContext);
                        }
                    }

                    if (shaderKey.eclipseShadowCount > 0)
                    {
                        renderContext.Shader.EclipseTexture = PlanetShadow.ResourceView;
                    }

                    if (shaderKey.HasRingShadows)
                    {
                        renderContext.Shader.RingTexture = RingsMap.ResourceView;
                    }

                    if (atmosphereOn)
                    {
                        SetAtmosphereConstants(renderContext, planetID, 1.0f, skyGeometryHeight);
                    }

                    // Disable overlays after we're done rendering the planet surface (i.e. we don't want
                    // them on any rings, sky shell, or cloud layer.)
                    shaderKey.overlayTextureCount = 0;

                    renderContext.setRasterizerState(TriangleCullMode.CullCounterClockwise);
                    if (planetID == 19 && !Settings.Active.SolarSystemMultiRes && Settings.Active.SolarSystemLighting)
                    {
                        shaderKey.style = PlanetSurfaceStyle.DiffuseAndNight;
                        SetupPlanetSurfaceEffect(renderContext, shaderKey, opacity);
                        if (atmosphereOn)
                        {
                            SetAtmosphereConstants(renderContext, planetID, 1.0f, skyGeometryHeight);
                        }

                        // Set eclipse constants
                        float eclipseShadowWidth = Settings.Active.SolarSystemScale * .00001f;
                        if (shaderKey.eclipseShadowCount > 0)
                        {
                            SetupShadow(renderContext, centerPoint, eclipseShadowWidth, SolarSystemObjects.Moon, 0);
                            renderContext.Shader.EclipseTexture = PlanetShadow.ResourceView;
                        }

                        renderContext.Shader.NightTexture = planetTexturesMaps[20].ResourceView;

                        renderContext.BlendMode = BlendMode.None;
                        DrawSphere(renderContext, planetTexturesMaps[planetID], planetID, sizeIndex, false, false, shaderKey);

                        // Set up for rendering specular pass
                        shaderKey.style = PlanetSurfaceStyle.SpecularPass;
                        SetupPlanetSurfaceEffect(renderContext, shaderKey, opacity);
                        if (shaderKey.eclipseShadowCount > 0)
                        {
                            SetupShadow(renderContext, centerPoint, eclipseShadowWidth, SolarSystemObjects.Moon, 0);
                            renderContext.Shader.EclipseTexture = PlanetShadow.ResourceView;
                        }
                        if (atmosphereOn)
                        {
                            SetAtmosphereConstants(renderContext, planetID, 1.0f, skyGeometryHeight);
                        }

                        // Set up specular properties; this should eventually be abstracted by RenderContext
                        renderContext.Shader.SpecularPower = 50.0f;
                        renderContext.Shader.SpecularColor = new SharpDX.Vector4(0.5f, 0.5f, 0.5f, 0.0f);

                        renderContext.BlendMode = BlendMode.Additive;
                        DrawSphere(renderContext, planetTexturesMaps[21], planetID, sizeIndex, false, true, shaderKey);
                        renderContext.BlendMode = BlendMode.None;
                    }
                    else
                    {
                        renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;

                        renderContext.PreDraw();
                        DrawSphere(renderContext, planetTexturesMaps[planetID], planetID, sizeIndex, false, true, shaderKey);
                    }

                    // Render the sky geometry. This is a sphere slightly larger than the planet where the
                    // only light is from atmospheric scattering. We'll render inside of the sphere only.
                    if (atmosphereOn)
                    {
                        // The sky geometry needs to be slightly larger than the planet itself, so adjust the
                        // world matrix by a scale factor a bit greater than 1.0
                        Matrix3d savedWorld = renderContext.World;
                        Matrix3d world = renderContext.World;
                        float skyRadius = skyGeometryHeight + 1.0f;
                        world.ScalePrepend(new Vector3d(skyRadius, skyRadius, skyRadius));
                        renderContext.World = world;

                        shaderKey.style = PlanetSurfaceStyle.Sky;
                        PlanetShader11 atmosphereShader = SetupPlanetSurfaceEffect(renderContext, shaderKey, 1.0f);
                        renderContext.Shader = atmosphereShader;

                        // Set the shadow texture if there's an eclipse
                        if (shaderKey.eclipseShadowCount > 0)
                        {
                            atmosphereShader.EclipseTexture = PlanetShadow.ResourceView;
                        }

                        // Atmosphere height is relative to the size of the sphere being rendered. Since we're
                        // actually rendering the shell geometry here, we just use a value of 0 for the height
                        // and a lower value for the zero height.
                        SetAtmosphereConstants(renderContext, planetID, 1.0f + skyGeometryHeight, 0.0f);

                        // We're rendering just the atmosphere; there's no contribution from a surface, so set the
                        // diffuse color to black.
                        atmosphereShader.DiffuseColor = new SharpDX.Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                        atmosphereShader.Opacity = 0.0f;

                        //renderContext.BlendMode = BlendMode.Alpha;
                        // Fade out the sky geometry as its screen size gets thin; this prevents terrible
                        // aliasing that occurs.
                        float skyShellPixels = (float)(skyGeometryHeight * radius * pixelsPerUnit);
                        float blendFactor = Math.Min(3.0f, skyShellPixels) / 3.0f;

                        renderContext.BlendMode = BlendMode.BlendFactorInverseAlpha;
                        renderContext.BlendFactor = new SharpDX.Color4(blendFactor, blendFactor, blendFactor, 1.0f);

                        renderContext.DepthStencilMode = DepthStencilMode.ZReadOnly;

                        //Render the _inside_ of the sphere. No need for multires or layers when drawing the sky
                        renderContext.setRasterizerState(TriangleCullMode.CullClockwise);
                        DrawSphere(renderContext, planetTexturesMaps[planetID], planetID, 0/*sizeIndex*/, true, false, shaderKey);
                            
                        // Reset device state
                        renderContext.setRasterizerState(TriangleCullMode.CullCounterClockwise);
                        renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;
                        renderContext.BlendMode = BlendMode.None;

                        // Restore the world matrix
                        renderContext.World = savedWorld;
                    }

                    RestoreDefaultMaterialState(renderContext);

                    if (planetID == 5)
                    {
                        PlanetShaderKey ringsKey = new PlanetShaderKey(PlanetSurfaceStyle.PlanetaryRings, false, 0);
                        SetupPlanetSurfaceEffect(renderContext, ringsKey, 1.0f);
                        DrawRings(renderContext);
                        RestoreDefaultMaterialState(renderContext);
                    }
                }
                else
                {
                    if (planetID == 0)
                    {
                        DrawPointPlanet(renderContext, new Vector3d(0, 0, 0), (float)(10 * planetDiameters[planetID]), planetColors[planetID], true, opacity);
                    }
                    else if (planetID < (int)SolarSystemObjects.Moon || planetID == (int)SolarSystemObjects.Earth)
                    {
                        float size = (float)(800 * planetDiameters[planetID]);
                        DrawPointPlanet(renderContext, new Vector3d(0, 0, 0), (float)Math.Max(.05, Math.Min(.1f, size)), planetColors[planetID], true, opacity);
                    }
                    else if (sizeIndexParam > .002)
                    {
                        float size = (float)(800 * planetDiameters[planetID]);
                        DrawPointPlanet(renderContext, new Vector3d(0, 0, 0), (float)Math.Max(.05, Math.Min(.1f, size)), planetColors[planetID], true, opacity);
                    }
                }

                // Restore all matrices modified by SetupMatrix...
                renderContext.World = matOld2;
                renderContext.WorldBase = matOldBase2;
                renderContext.WorldBaseNonRotating = matOldNonRotating2;
            }

            {
                Vector3d sunPosition = (planet3dLocations[0] - centerPoint);
                Vector3d planetPosition = planet3dLocations[planetID] - centerPoint;
                renderContext.SunPosition = sunPosition;
                renderContext.ReflectedLightPosition = planetPosition;
                if (planetID == (int)SolarSystemObjects.Earth)
                {
                    renderContext.ReflectedLightColor = Color.FromArgb(50, 70, 90);
                    renderContext.HemisphereLightColor = Color.FromArgb(100, 100, 100);
                }
                else
                {
                    renderContext.ReflectedLightColor = Color.Black;
                    renderContext.HemisphereLightColor = Color.Black;
                }

                renderContext.OccludingPlanetPosition = planetPosition;
                renderContext.OccludingPlanetRadius = planetDiameters[planetID] / 2.0;
            }

            
            LayerManager.Draw(renderContext, 1.0f, false, Enum.GetName(typeof(SolarSystemObjects), (SolarSystemObjects)planetID), true, false);
            
            // Reset render context state
            renderContext.ReflectedLightColor = Color.Black;
            renderContext.HemisphereLightColor = Color.Black;
            
            renderContext.World = matOld;
            Earth3d.MainWindow.MakeFrustum();
            
            RestoreDefaultMaterialState(renderContext);
            renderContext.setRasterizerState(TriangleCullMode.CullClockwise);
        }


        // Compute the rotation of a planet at the J2000 epoch.
        //
        // The rotation at some instant in can be computed by multiplying the
        // the returned matrix by Y(W * t)
        public static Matrix3d GetPlanetOrientationAtEpoch(int planetID)
        {
            Matrix3d m = Matrix3d.Identity;

            // Rotational elements for the planets are in the form used by the
            // IAU Working Group on Cartographic Coordinates and Rotational Elements:
            //
            // a : Right ascension of north pole
            // d : Declination of north pole
            // W0 : Prime meridian angle at epoch J2000.0
            //
            // The canonical Euler angle sequence is: Z(a - 90) * X(90 - d) * Z(W0)
            //
            // The following transformations are required to convert it to a rotation for WWT:
            //    * WWT uses a coordinate system with +Y = ecliptic north, +X = equinox of J2000
            //      This system is rotated 90 degrees about the X axis from the standard ecliptic
            //      system based on the Earth Mean Equinox of J2000 (EMEJ2000)
            //    * WWT uses row vectors instead of column vectors, so the order of transformations
            //      is reversed
            //    * WWT has planet maps with longitude 0 at the edge rather than the middle. This
            //      requires an extra 180 degrees to be added to W0

            const double obliquityOfEcliptic = 23.4392794;

            if (planetID == (int)SolarSystemObjects.Earth)
            {
                // Different calculation for Earth, since the meridian offset
                // is already included in the Mean Sidereal Time function.
                m.Multiply(Matrix3d.RotationX(obliquityOfEcliptic * RC)); // equatorial to ecliptic transformation
            }
            else
            {
                m.Multiply(Matrix3d.RotationX(-90 * RC));    // 90 degree rotation from WWT coord sys

                m.Multiply(Matrix3d.RotationZ((180 + planetAngles[planetID].primeMeridian) * RC));
                m.Multiply(Matrix3d.RotationX(( 90 - planetAngles[planetID].poleDec) * RC));
                m.Multiply(Matrix3d.RotationZ((planetAngles[planetID].poleRa - 90) * RC));
                m.Multiply(Matrix3d.RotationX(obliquityOfEcliptic * RC)); // equatorial to ecliptic transformation

                m.Multiply(Matrix3d.RotationX(90 * RC));     // 90 degree rotation back to WWT coord sys
            }

            return m;
        }

        public static void SetupPlanetMatrix(RenderContext11 renderContext, int planetID,  Vector3d centerPoint, bool makeFrustum)
        {
            Matrix3d matNonRotating = renderContext.World;

            SetupMatrixForPlanetGeometry(renderContext, planetID, centerPoint, makeFrustum);

            if (planetID == (int)SolarSystemObjects.Sun)
            {
                // Don't apply the Sun's orientation to its non-rotating frame; this means that
                // the Sun's reference frame will be the ecliptic frame.
                double radius = GetAdjustedPlanetRadius(planetID);
                matNonRotating.Scale(new Vector3d(radius, radius, radius));
                Vector3d translation = planet3dLocations[planetID] - centerPoint;
                matNonRotating.Multiply(Matrix3d.Translation(translation));
                renderContext.WorldBaseNonRotating = matNonRotating;
            }
        }
        public static Matrix3d EarthMatrix = new Matrix3d();
        public static Matrix3d EarthMatrixInv = new Matrix3d();

        private static double SetupMatrixForPlanetGeometry(RenderContext11 renderContext, int planetID, Vector3d centerPoint, bool makeFrustum)
        {
            double radius = GetAdjustedPlanetRadius(planetID);


            double rotationCurrent = 0;
            if (planetID == (int)SolarSystemObjects.Earth)
            {
                rotationCurrent = Coordinates.MstFromUTC2(SpaceTimeController.Now, 0) / 180.0 * Math.PI;
            }
            else
            {
                rotationCurrent = (((jNow - 2451545.0) / planetRotationPeriod[planetID]) * Math.PI * 2) % (Math.PI * 2);
            }

            if (planetID == (int)SolarSystemObjects.Moon)
            {
                rotationCurrent -= Math.PI / 2;
            }

            Matrix3d matLocal = renderContext.World;
            Matrix3d matNonRotating = renderContext.World;
            Vector3d translation = planet3dLocations[planetID] - centerPoint;

            Matrix3d orientationAtEpoch = GetPlanetOrientationAtEpoch(planetID);

            matLocal.Scale(new Vector3d(radius, radius, radius));
            matLocal.Multiply(Matrix3d.RotationY(-rotationCurrent));
            matLocal.Multiply(orientationAtEpoch);


            if (planetID == (int)Earth3d.MainWindow.viewCamera.Target)
            {
                EarthMatrix = Matrix3d.Identity;
                EarthMatrix.Multiply(Matrix3d.RotationY(-rotationCurrent));
                EarthMatrix.Multiply(orientationAtEpoch);

                EarthMatrixInv = EarthMatrix;
                EarthMatrixInv.Invert();
            }

            matLocal.Multiply(Matrix3d.Translation(translation));
            renderContext.World = matLocal;
            renderContext.WorldBase = renderContext.World;
            renderContext.NominalRadius = GetPlanetRadiusInMeters(planetID);


            if (makeFrustum)
            {
                Earth3d.MainWindow.MakeFrustum();
            }
            matNonRotating.Scale(new Vector3d(radius, radius, radius));
            matNonRotating.Multiply(orientationAtEpoch);
            matNonRotating.Multiply(Matrix3d.Translation(translation));
            renderContext.WorldBaseNonRotating = matNonRotating;
            return rotationCurrent;
        }


        public static void DrawPointPlanet(RenderContext11 renderContext, Vector3d location, float size, Color color, bool zOrder, float opacity)
        {
            var vertices = new PositionColorSize[1];

            vertices[0].Position = location.Vector311;
            vertices[0].size = size * 200;
            vertices[0].Color = color;

            if (planetsPointSet == null)
            {
                planetsPointSet = new PointSpriteSet(renderContext.Device, vertices);
                planetsPointSet.PointScaleFactors = new SharpDX.Vector3(1.0f, 0.0f, 0.0f);
            }
            else
            {
                planetsPointSet.updateVertexBuffer(vertices);
            }

            renderContext.BlendMode = BlendMode.Additive;
            renderContext.DepthStencilMode = zOrder ? DepthStencilMode.ZReadOnly : DepthStencilMode.Off;
            renderContext.setRasterizerState(TriangleCullMode.Off);

            planetsPointSet.Draw(renderContext, 1, Grids.StarProfile, opacity);

            renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;
            renderContext.BlendMode = BlendMode.Alpha;
        }

        private static Vector3d SetupShadow(RenderContext11 renderContext, Vector3d centerPoint, float width, SolarSystemObjects shadowCaster, int shadowIndex)
        {
            SharpDX.Direct3D11.Device device = renderContext.Device;

            if (PlanetShadow == null)
            {
                PlanetShadow = Texture11.FromBitmap(device, Properties.Resources.planetShadow);
            }

            Matrix3d invViewCam = renderContext.View;
            invViewCam.Invert();

            Vector3d sun = planet3dLocations[0];
            sun.Subtract(centerPoint);

            Vector3d moon = planet3dLocations[(int)shadowCaster];
            moon.Subtract(centerPoint);

            Matrix3d biasd = Matrix3d.Scaling(0.5, 0.5, 0.5) * Matrix3d.Translation(new Vector3d(0.5, 0.5, 0.5));

            Matrix3d mat =
                invViewCam *
                Matrix3d.LookAtLH(sun, moon, new Vector3d(0, 1, 0)) *
                Matrix3d.PerspectiveFovLH(width, 1, 0.001f, 200f) *
                biasd;

            renderContext.SetEclipseShadowMatrix(shadowIndex, mat.Matrix11);

            return centerPoint;
        }

        private static Texture11 ringShadow;

        public static double GetAdjustedPlanetRadius(int planetID)
        {
            if (planetDiameters == null)
            {
                return 1;
            }

            if (planetID > planetDiameters.Length - 1)
            {
                planetID = (int)SolarSystemObjects.Earth;
            }


            double diameter = planetDiameters[planetID];


            double radius = (double)(diameter / 2.0);
            if (planetID != 0)
            {
                radius = radius * (1 + (3 * (Settings.Active.SolarSystemScale - 1)));
            }
            else
            {
                radius = radius * (1 + (.30 * (Settings.Active.SolarSystemScale - 1)));
            }

            return radius;
        }

        public static double GetPlanetRadiusInMeters(int planetID)
        {
            if (planetID > planetDiameters.Length - 1)
            {
                planetID = (int)SolarSystemObjects.Earth;
            }


            double diameter = planetDiameters[planetID];


            return (diameter / 2.0) * UiTools.KilometersPerAu * 1000;
            
        }

        static AstroRaDec[] planetLocations = new AstroRaDec[20];

        private static void DrawPlanet(RenderContext11 renderContext, int planetID, float opacity)
        {
            AstroRaDec planetPosition = planetLocations[planetID];

            Texture11 currentPlanetTexture;

            if (planetScales[planetID] < (Earth3d.MainWindow.ZoomFactor / 6.0) / 300)
            {
                if (planetID < 14)
                {
                    Vector3d point = Coordinates.RADecTo3d(planetPosition.RA - 12, planetPosition.Dec, .9);
                    DrawPointPlanet(renderContext, point, .1f, planetColors[planetID], false, opacity);
                }
                return;
            }


            if (planetID < 10 || planetID == 18)
            {
                currentPlanetTexture = planetTextures[planetID];
            }
            else if (planetID < 14)
            {
                if (planetLocations[planetID].Eclipsed)
                {
                    currentPlanetTexture = planetTextures[15];

                }
                else
                {

                    currentPlanetTexture = planetTextures[planetID];
                }
            }
            else
            {
                if (!planetLocations[planetID].Shadow)
                {
                    return;
                }
                //Shadows of moons
                currentPlanetTexture = planetTextures[15];
            }


            float radius = (float)(planetScales[planetID] / 2.0);
            float raRadius = (float)(radius / Math.Cos(planetPosition.Dec / 180.0 * Math.PI));
            
            drawPlanetPoints[0].Position = Coordinates.RADecTo3d((planetPosition.RA + (raRadius / 15)) - 12, planetPosition.Dec + radius, 1).Vector4;
            drawPlanetPoints[0].Tu = 0;
            drawPlanetPoints[0].Tv = 0;
            drawPlanetPoints[0].Color = Color.FromArgb((int)(opacity * 255), Color.White);
            drawPlanetPoints[1].Position = Coordinates.RADecTo3d((planetPosition.RA - (raRadius / 15)) - 12, planetPosition.Dec + radius, 1).Vector4;
            drawPlanetPoints[1].Tu = 1;
            drawPlanetPoints[1].Tv = 0;
            drawPlanetPoints[1].Color = Color.FromArgb((int)(opacity * 255), Color.White);
            drawPlanetPoints[2].Position = Coordinates.RADecTo3d((planetPosition.RA + (raRadius / 15)) - 12, planetPosition.Dec - radius, 1).Vector4;
            drawPlanetPoints[2].Tu = 0;
            drawPlanetPoints[2].Tv = 1;
            drawPlanetPoints[2].Color = Color.FromArgb((int)(opacity * 255), Color.White);
            drawPlanetPoints[3].Position = Coordinates.RADecTo3d((planetPosition.RA - (raRadius / 15)) - 12, planetPosition.Dec - radius, 1).Vector4;
            drawPlanetPoints[3].Tu = 1;
            drawPlanetPoints[3].Tv = 1;
            drawPlanetPoints[3].Color = Color.FromArgb((int)(opacity * 255), Color.White);


            renderContext.DepthStencilMode = DepthStencilMode.Off;
            Sprite2d.Draw(renderContext, drawPlanetPoints, 4, currentPlanetTexture, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);

        }
        static PositionColoredTextured[] drawPlanetPoints = new PositionColoredTextured[4];

        private static void DrawPlanetPhase(RenderContext11 renderContext, int planetID, double phase, double angle, int dark, float opacity)
        {
            Texture11 currentPlanetTexture;
            AstroRaDec planetPosition = planetLocations[planetID];

            if (planetID < 10)
            {
                currentPlanetTexture =  planetTextures[planetID];
            }
            else if (planetID < 14)
            {
                if (planetLocations[planetID].Eclipsed)
                {
                    currentPlanetTexture = planetTextures[15];
                }
                else
                {
                    currentPlanetTexture = planetTextures[planetID];
                }
            }
            else
            {
                if (!planetLocations[planetID].Shadow)
                {
                    return;
                }
                //Shadows of moons
                currentPlanetTexture =  planetTextures[15];
            }


            float radius = (float)(planetScales[planetID] / 2.0);
            float raRadius = (float)(radius / Math.Cos(planetPosition.Dec / 180.0 * Math.PI));


            int index = 0;


            double top = radius;
            double bottom = -radius;
            double left = +(radius / 15);
            double right = -(radius / 15);

            double width = left - right;
            double height = bottom - top;

            Color rightColor = Color.FromArgb((int)(opacity*255), dark * 16, dark * 16, dark * 16);
            Color leftColor = Color.FromArgb((int)(opacity * 255), dark, dark, dark);

            double phaseFactor = Math.Sin(Coordinates.DegreesToRadians(phase + 90));
            if (phase < 180)
            {
                rightColor = leftColor;
                leftColor = Color.FromArgb((int)(opacity * 255), dark * 16, dark * 16, dark * 16);
                //  phaseFactor = -phaseFactor;
            }

            double rotation = -Math.Cos(planetPosition.RA / 12 * Math.PI) * 23.5;

            SharpDX.Matrix matrix = SharpDX.Matrix.Identity;
            matrix = SharpDX.Matrix.Multiply(matrix, SharpDX.Matrix.RotationX((float)(((rotation)) / 180f * Math.PI)));
            matrix = SharpDX.Matrix.Multiply(matrix, SharpDX.Matrix.RotationZ((float)((planetPosition.Dec) / 180f * Math.PI)));
            matrix = SharpDX.Matrix.Multiply(matrix, SharpDX.Matrix.RotationY((float)(((360 - (planetPosition.RA * 15)) + 180) / 180f * Math.PI)));

            double step = 1.0 / planetSegments;
            for (int i = 0; i <= planetSegments; i++)
            {

                double y = i * (1.0 / planetSegments);
                double yf = (y - .5) * 2;
                double x = Math.Sqrt(1 - ((yf) * (yf)));
                double xt;
                x = x * phaseFactor;
                x = ((width / 2) + (x * width / 2)) - width / 80;
                if (x > width)
                {
                    x = width;
                }
                if (x < 0)
                {
                    x = 0;
                }
                xt = x / width;
                double x1 = Math.Sqrt(1 - ((yf) * (yf)));
                double x1t;
                x1 = x1 * phaseFactor;
                x1 = ((width / 2) + (x1 * width / 2)) + width / 80;
                if (x1 > width)
                {
                    x1 = width;
                }
                if (x1 < 0)
                {
                    x1 = 0;
                }
                x1t = x1 / width;

                points[index].Pos3 = Coordinates.RADecTo3d(left, top + y * height, matrix);
                points[index].Tu = 0;
                points[index].Tv = (float)y;
                points[index].Color = leftColor;
                points[index + 1].Pos3 = Coordinates.RADecTo3d(left - x, top + y * height, matrix);
                points[index + 1].Tu = (float)xt;
                points[index + 1].Tv = (float)y;
                points[index + 1].Color = leftColor;
                points[index + 2].Pos3 = Coordinates.RADecTo3d(left - x1, top + y * height, matrix);
                points[index + 2].Tu = (float)x1t;
                points[index + 2].Tv = (float)y;
                points[index + 2].Color = rightColor;
                points[index + 3].Pos3 = Coordinates.RADecTo3d(right, top + y * height, matrix);
                points[index + 3].Tu = 1;
                points[index + 3].Tv = (float)y;
                points[index + 3].Color = rightColor;

                index += 4;
                //points
            }

            index = 0;
            for (int yy = 0; yy < planetSegments; yy++)
            {
                for (int xx = 0; xx < 3; xx++)
                {
                    triangles[index] = points[yy * 4 + xx];
                    triangles[index + 1] = points[yy * 4 + (xx + 1)];
                    triangles[index + 2] = points[((yy + 1) * 4) + (xx)];
                    triangles[index + 3] = points[yy * 4 + (xx + 1)];
                    triangles[index + 4] = points[((yy + 1) * 4) + (xx + 1)];
                    triangles[index + 5] = points[((yy + 1) * 4) + (xx)];
                    index += 6;
                }

            }

            renderContext.DepthStencilMode = DepthStencilMode.Off;
            Sprite2d.Draw(renderContext, triangles, triangles.Length, currentPlanetTexture, SharpDX.Direct3D.PrimitiveTopology.TriangleList);

        }
        static int planetSegments = 128;
        static PositionColoredTextured[] points = new PositionColoredTextured[4 * (planetSegments + 1)];
        static PositionColoredTextured[] triangles = new PositionColoredTextured[18 * (planetSegments)];

        static SharpDX.Vector4 ToVector4(SharpDX.Vector3 vector)
        {
            return new SharpDX.Vector4(vector, 1);
        }

        static double GeocentricElongation(double ObjectAlpha, double ObjectDelta, double SunAlpha, double SunDelta)
        {
            //Convert the RA's to radians
            ObjectAlpha = Coordinates.DegreesToRadians(ObjectAlpha * 15);
            SunAlpha = Coordinates.DegreesToRadians(SunAlpha * 15);

            //Convert the declinations to radians
            ObjectDelta = Coordinates.DegreesToRadians(ObjectDelta);
            SunDelta = Coordinates.DegreesToRadians(SunDelta);

            //Return the result
            return Coordinates.RadiansToDegrees(Math.Acos(Math.Sin(SunDelta) * Math.Sin(ObjectDelta) + Math.Cos(SunDelta) * Math.Cos(ObjectDelta) * Math.Cos(SunAlpha - ObjectAlpha)));
        }

        static double PhaseAngle(double GeocentricElongation, double EarthObjectDistance, double EarthSunDistance)
        {
            //Convert from degrees to radians
            GeocentricElongation = Coordinates.DegreesToRadians(GeocentricElongation);

            //Return the result
            return Coordinates.MapTo0To360Range(Coordinates.RadiansToDegrees(Math.Atan2(EarthSunDistance * Math.Sin(GeocentricElongation), EarthObjectDistance - EarthSunDistance * Math.Cos(GeocentricElongation))));
        }
        static double PositionAngle(double Alpha0, double Delta0, double Alpha, double Delta)
        {
            //Convert to radians
            Alpha0 = Coordinates.HoursToRadians(Alpha0);
            Alpha = Coordinates.HoursToRadians(Alpha);
            Delta0 = Coordinates.DegreesToRadians(Delta0);
            Delta = Coordinates.DegreesToRadians(Delta);

            return Coordinates.MapTo0To360Range(Coordinates.RadiansToDegrees(Math.Atan2(Math.Cos(Delta0) * Math.Sin(Alpha0 - Alpha), Math.Sin(Delta0) * Math.Cos(Delta) - Math.Cos(Delta0) * Math.Sin(Delta) * Math.Cos(Alpha0 - Alpha))));
        }

        static Texture11 PlanetShadow;
        static void DrawSphere(RenderContext11 renderContext, Texture11 texture, int planetID, int sphereIndex, bool noMultires, bool drawLayers, PlanetShaderKey shaderKey)
        {
            bool useMultires = Settings.Active.SolarSystemMultiRes && !noMultires;

            // Let an override layer take the place of the standard layer
            IImageSet defaultLayer = null;
            Color defaultLayerColor = Color.White;
            if (drawLayers)
            {
                string referenceFrameName = Enum.GetName(typeof(SolarSystemObjects), (SolarSystemObjects)planetID);
                foreach (Layer layer in LayerManager.AllMaps[referenceFrameName].Layers)
                {
                    if (layer.Enabled && layer is ImageSetLayer)
                    {
                        if (((ImageSetLayer)layer).OverrideDefaultLayer)
                        {
                            defaultLayer = ((ImageSetLayer)layer).ImageSet;
                            defaultLayerColor = layer.Color;
                            useMultires = true;
                            break;
                        }
                    }
                }
            }

            if (useMultires)
            {
                renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;

                if (sphereIndex < 4)
                {
                    IImageSet planet = null;

                    if (defaultLayer != null)
                    {
                        planet = defaultLayer;
                    }
                    else
                    {
                        string planetName = GetNameFrom3dId(planetID);
                        string imageSetName = planetName == "Mars" ? "Visible Imagery" : planetName;
                        planet = Earth3d.MainWindow.GetImagesetByName(imageSetName);
                    }

                    if (planet != null)
                    {
                        SharpDX.Vector4 normColor = new SharpDX.Vector4(defaultLayerColor.R, defaultLayerColor.G, defaultLayerColor.B, defaultLayerColor.A) * (1.0f / 255.0f);
                        if (RenderContext11.sRGB)
                        {
                            normColor.X = (float) Math.Pow(normColor.X, 2.2);
                            normColor.Y = (float)Math.Pow(normColor.Y, 2.2);
                            normColor.Z = (float)Math.Pow(normColor.Z, 2.2);
                        }
                        renderContext.Shader.DiffuseColor = normColor;
                        renderContext.setRasterizerState(TriangleCullMode.CullClockwise);
                        Earth3d.MainWindow.DrawTiledSphere(planet, 100, Color.White);

                        if (planetID == (int)SolarSystemObjects.Earth && Properties.Settings.Default.ShowClouds.State)
                        {
                            if (CloudTexture != null)
                            {
                                PlanetShaderKey cloudShaderKey = new PlanetShaderKey(PlanetSurfaceStyle.Diffuse, Settings.Active.ShowEarthSky, 0);
                                cloudShaderKey.eclipseShadowCount = shaderKey.eclipseShadowCount;
                                cloudShaderKey.HasAtmosphere = shaderKey.HasAtmosphere;

                                if (!Settings.Active.SolarSystemLighting)
                                {
                                    cloudShaderKey.style = PlanetSurfaceStyle.Emissive;
                                }

                                SetupPlanetSurfaceEffect(renderContext, cloudShaderKey, Properties.Settings.Default.ShowClouds.Opacity);
                                SetAtmosphereConstants(renderContext, planetID, 1.0f, CalcSkyGeometryHeight(planetID));

                                renderContext.MainTexture = CloudTexture;

                                Matrix3d savedWorld = renderContext.World;
                                double cloudScale = 1.0 + (EarthCloudHeightMeters) / 6378100.0;
                                renderContext.World = Matrix3d.Scaling(cloudScale, cloudScale, cloudScale) * renderContext.World;

                                renderContext.setRasterizerState(TriangleCullMode.CullCounterClockwise);
                                renderContext.DepthStencilMode = DepthStencilMode.Off;
                                renderContext.BlendMode = BlendMode.Alpha;

                                DrawFixedResolutionSphere(renderContext, sphereIndex);

                                renderContext.World = savedWorld;

                                renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;
                                renderContext.BlendMode = BlendMode.None;
                            }
                        }

                        return;
                    }
                }
            }

            renderContext.MainTexture = texture;

            renderContext.Device.ImmediateContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            renderContext.Device.ImmediateContext.InputAssembler.InputLayout = renderContext.Shader.inputLayout(PlanetShader11.StandardVertexLayout.PositionNormalTex2);

            renderContext.PreDraw();

            renderContext.SetVertexBuffer(sphereVertexBuffers[sphereIndex]);
            renderContext.SetIndexBuffer(sphereIndexBuffers[sphereIndex]);
            renderContext.devContext.DrawIndexed(sphereIndexBuffers[sphereIndex].Count, 0, 0);
        }


        public static void DrawFixedResolutionSphere(RenderContext11 renderContext, int sphereIndex)
        {
            if (sphereVertexBuffers != null && sphereVertexBuffers[sphereIndex] != null)
            {
                renderContext.Device.ImmediateContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
                renderContext.Device.ImmediateContext.InputAssembler.InputLayout = renderContext.Shader.inputLayout(PlanetShader11.StandardVertexLayout.PositionNormalTex2);

                renderContext.PreDraw();
                renderContext.SetVertexBuffer(sphereVertexBuffers[sphereIndex]);
                renderContext.SetIndexBuffer(sphereIndexBuffers[sphereIndex]);
                renderContext.devContext.DrawIndexed(sphereIndexBuffers[sphereIndex].Count, 0, 0);
            }
        }


        static int[] triangleCountSphere = null;
        static int[] vertexCountSphere = null;
         static int maxSubDivisionsX = 192*2;
        static int maxSubDivisionsY = 96*2;
       
        static VertexBuffer11[] sphereVertexBuffers = null;
        static IndexBuffer11[] sphereIndexBuffers = null;
        const int sphereCount = 4;
        static void InitSphere()
        {
            
            if (sphereIndexBuffers != null)
            {
                foreach (IndexBuffer11 indexBuf in sphereIndexBuffers)
                {
                    indexBuf.Dispose();
                    GC.SuppressFinalize(indexBuf);
                }
            }
            if (sphereVertexBuffers != null)
            {
                foreach (VertexBuffer11 vertBuf in sphereVertexBuffers)
                {
                    vertBuf.Dispose();
                    GC.SuppressFinalize(vertBuf);
                }
            }
            sphereVertexBuffers = new VertexBuffer11[sphereCount];
            sphereIndexBuffers = new IndexBuffer11[sphereCount];

            triangleCountSphere = new int[sphereCount];
            vertexCountSphere = new int[sphereCount];

            int countX = maxSubDivisionsX;
            int countY = maxSubDivisionsY;

            for (int sphereIndex = 0; sphereIndex < sphereCount; sphereIndex++)
            {
                triangleCountSphere[sphereIndex] = countX * countY * 2;
                vertexCountSphere[sphereIndex] = (countX + 1) * (countY + 1);
                sphereVertexBuffers[sphereIndex] = new VertexBuffer11(typeof(PositionNormalTexturedX2), ((countX + 1) * (countY + 1)),RenderContext11.PrepDevice);

                sphereIndexBuffers[sphereIndex] = new IndexBuffer11(typeof(int), countX * countY * 6, RenderContext11.PrepDevice);

                double lat, lng;

                int index = 0;
                double latMin = 90;
                double latMax = -90;
                double lngMin = -180;
                double lngMax = 180;


                // Create a vertex buffer 
                PositionNormalTexturedX2[] verts = (PositionNormalTexturedX2[])sphereVertexBuffers[sphereIndex].Lock(0, 0); // Lock the buffer (which will return our structs)
                int x1, y1;

                double latDegrees = latMax - latMin;
                double lngDegrees = lngMax - lngMin;

                double textureStepX = 1.0f / countX;
                double textureStepY = 1.0f / countY;
                for (y1 = 0; y1 <= countY; y1++)
                {

                    if (y1 != countY)
                    {
                        lat = latMax - (textureStepY * latDegrees * (double)y1);
                    }
                    else
                    {
                        lat = latMin;
                    }

                    for (x1 = 0; x1 <= countX; x1++)
                    {
                        if (x1 != countX)
                        {
                            lng = lngMin + (textureStepX * lngDegrees * (double)x1);
                        }
                        else
                        {
                            lng = lngMax;
                        }
                        index = y1 * (countX + 1) + x1;
                        verts[index].Position = Coordinates.GeoTo3dDouble(lat, lng);// Add Altitude mapping here
                        verts[index].Normal = verts[index].Position;// with altitude will come normal recomputer from adjacent triangles
                        verts[index].Tu = (float)(x1 * textureStepX);
                        verts[index].Tv = (float)(1f - (y1 * textureStepY));
                    }
                }
                sphereVertexBuffers[sphereIndex].Unlock();
                int[] indexArray = (int[])sphereIndexBuffers[sphereIndex].Lock();
                for (y1 = 0; y1 < countY; y1++)
                {
                    bool bWinding = (y1 % 2) == 0;
                    for (x1 = 0; x1 < countX; x1++)
                    {
                        index = (y1 * countX * 6) + 6 * x1;
                        if (bWinding)
                        {
                            // First triangle in quad
                            indexArray[index] = (int)(y1 * (countX + 1) + x1); //A
                            indexArray[index + 2] = (int)(y1 * (countX + 1) + (x1 + 1));//B
                            indexArray[index + 1] = (int)((y1 + 1) * (countX + 1) + x1);//C

                            // Second triangle in quad
                            indexArray[index + 3] = (int)(y1 * (countX + 1) + (x1 + 1));//B
                            indexArray[index + 5] = (int)((y1 + 1) * (countX + 1) + (x1 + 1)); //D
                            indexArray[index + 4] = (int)((y1 + 1) * (countX + 1) + x1);// C
                        }
                        else
                        {
                            // First triangle in quad
                            indexArray[index] = (int)(y1 * (countX + 1) + x1); //A
                            indexArray[index + 2] = (int)((y1 + 1) * (countX + 1) + (x1 + 1)); //D
                            indexArray[index + 1] = (int)((y1 + 1) * (countX + 1) + x1);//C

                            // Second triangle in quad
                            indexArray[index + 3] = (int)(y1 * (countX + 1) + (x1 + 1));//B
                            indexArray[index + 5] = (int)((y1 + 1) * (countX + 1) + (x1 + 1)); //D
                            indexArray[index + 4] = (int)(y1 * (countX + 1) + x1); //A
                        }
                    }
                }
                sphereIndexBuffers[sphereIndex].Unlock();
                countX /= 2;
                countY /= 2;
            }

          
        }


        const int subDivisionsRings = 192;

        static int triangleCountRings = subDivisionsRings * 2;
        static VertexBuffer11 ringsVertexBuffer = null;

        // Various input layouts used in 3D solar system mode
        // TODO Replace with an input layout cache

        static void DrawRings(RenderContext11 renderContext)
        {
            renderContext.setRasterizerState(TriangleCullMode.Off);

            PlanetShaderKey ringsKey = new PlanetShaderKey(PlanetSurfaceStyle.PlanetaryRings, false, 0);
            SetupPlanetSurfaceEffect(renderContext, ringsKey, 1.0f);
            renderContext.Shader.MainTexture = RingsMap.ResourceView;

            renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleStrip;
            renderContext.Device.ImmediateContext.InputAssembler.InputLayout = renderContext.Shader.inputLayout(PlanetShader11.StandardVertexLayout.PositionNormalTex);

            renderContext.PreDraw();             
            renderContext.SetVertexBuffer(ringsVertexBuffer);
            renderContext.devContext.Draw((triangleCountRings+2), 0);

            renderContext.setRasterizerState(TriangleCullMode.CullCounterClockwise);
        }



        static void InitRings()
        {    
            if (ringsVertexBuffer != null)
            {
                ringsVertexBuffer.Dispose();
                GC.SuppressFinalize(ringsVertexBuffer);
                ringsVertexBuffer = null;
            }
            double inner = 1.113;
            double outer = 2.25;

            ringsVertexBuffer = new VertexBuffer11(typeof(PositionNormalTextured), ((subDivisionsRings + 1) * 4), RenderContext11.PrepDevice);

            triangleCountRings = (subDivisionsRings) * 2;
            var verts = (PositionNormalTexturedX2[])ringsVertexBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)

            double radStep = Math.PI * 2.0 / (double)subDivisionsRings;
            int index = 0;
            for (int x = 0; x <= subDivisionsRings; x += 2)
            {
                double rads1 = x * radStep;
                double rads2 = (x + 1) * radStep;

                verts[index].Position = new Vector3d((Math.Cos(rads1) * inner), 0, (Math.Sin(rads1) * inner));
                verts[index].Normal = new Vector3d(0, 1, 0);
                verts[index].Tu = 1;
                verts[index].Tv = 0;
                index++;

                verts[index].Position = new Vector3d((Math.Cos(rads1) * outer), 0, (Math.Sin(rads1) * outer));
                verts[index].Normal = new Vector3d(0, 1, 0);
                verts[index].Tu = 0;
                verts[index].Tv = 0;
                index++;
                verts[index].Position = new Vector3d((Math.Cos(rads2) * inner), 0, (Math.Sin(rads2) * inner));
                verts[index].Normal = new Vector3d(0, 1, 0);
                verts[index].Tu = 1;
                verts[index].Tv = 1;
                index++;
                verts[index].Position = new Vector3d((Math.Cos(rads2) * outer), 0, (Math.Sin(rads2) * outer));
                verts[index].Normal = new Vector3d(0, 1, 0);
                verts[index].Tu = 0;
                verts[index].Tv = 1;
                index++;

            }
            ringsVertexBuffer.Unlock();
        }
    }
}
