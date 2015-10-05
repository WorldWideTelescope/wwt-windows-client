using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Threading;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using TerraViewer.Properties;
using Color = System.Drawing.Color;
using Device = SharpDX.Direct3D11.Device;
using Matrix = SharpDX.Matrix;
using Vector3 = SharpDX.Vector3;
namespace TerraViewer
{
    class Grids
    {
        static PositionTextured[] auroraPoints;
        static double f1 = .5;
        static double f2 = 2;
        static double f3 = .5;
        static double f4 = .1;
        static double f5 = .1;
        static double f6 = .1;   
        static readonly Random rnd = new Random(123343);
        public static bool DrawAuroraBorialis(RenderContext11 renderContext, float opacity)
        {
            if (auroraPoints == null)
            {
                auroraPoints = new PositionTextured[7202];
            }
            {

                var mat = Matrix3d.RotationX((float)(7.7 / 360.0 * (Math.PI * 2)));
                mat.Multiply(Matrix3d.RotationY((float)(-133.4 / 360.0 * (Math.PI * 2))));

                var index = 0;
                double lat = 80;
                f1 += (rnd.NextDouble() * 2 - 1) / 100;
                f2 += (rnd.NextDouble() * 2 - 1) / 200;
                f3 += (rnd.NextDouble() * 2 - 1) / 300;
                f4 += (rnd.NextDouble() * 2 - 1) / 100;
                f5 += (rnd.NextDouble() * 2 - 1) / 100;
                f6 += (rnd.NextDouble() * 2 - 1) / 100;
               
                f1 = Math.Min(.5,Math.Max(-.5,f1));
                f2 = Math.Min(.25,Math.Max(-.25,f1));
                f3 = Math.Min(.25,Math.Max(-.25,f1));
                f4 = Math.Min(.25,Math.Max(-.25,f1));
                f5 = Math.Min(.25, Math.Max(-.25, f1));
                f6 = Math.Min(.25, Math.Max(-.25, f1));
              

                for (double lng = 0; lng <= 360.05; lng += .1)
                {
                    var t = lng / 180 * Math.PI;

                    lat = 80 + Math.Sin(t) * f1 + Math.Cos(t * 2) * f2 + Math.Sin(t * 5) * f3 + Math.Cos(t * 7) * f4 + Math.Sin(t * 11) * f5 + Math.Cos(t * 13) * f6;


                    auroraPoints[index].Position = Vector3d.TransformCoordinate(Coordinates.GeoTo3dDouble(lat, lng, 1.0001),mat);
                    auroraPoints[index].Tu = (float)(lng / 360);
                    auroraPoints[index].Tv = 0;
                    index++;
                    auroraPoints[index].Position = Vector3d.TransformCoordinate(Coordinates.GeoTo3dDouble(lat - .1, lng, 1.0200), mat);
                    auroraPoints[index].Tu = (float)(lng / 360);
                    auroraPoints[index].Tv = 1;
                    index++;
                }

            }


            //todo11 port this
            //Device device = renderContext.Device;

            //device.RenderState.CullMode = Cull.None;
            //device.RenderState.AlphaBlendEnable = true;
            //device.RenderState.SourceBlend = Microsoft.DirectX.Direct3D.Blend.SourceAlpha;
            //device.RenderState.DestinationBlend = Microsoft.DirectX.Direct3D.Blend.One;

            //device.RenderState.ColorWriteEnable = ColorWriteEnable.RedGreenBlueAlpha;
            //device.TextureState[0].ColorOperation = TextureOperation.Modulate;
            //device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
            //device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
            //device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
            //device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
            //device.TextureState[0].AlphaArgument2 = TextureArgument.Diffuse;

            ////
            //device.TextureState[1].ColorOperation = TextureOperation.Modulate;
            //device.TextureState[1].ColorArgument1 = TextureArgument.Current;
            //device.TextureState[1].ColorArgument2 = TextureArgument.Constant;
            //device.TextureState[1].AlphaOperation = TextureOperation.Modulate;
            //device.TextureState[1].AlphaArgument1 = TextureArgument.Current;
            //device.TextureState[1].AlphaArgument2 = TextureArgument.Constant;
            //device.TextureState[1].ConstantColorValue = (int)Color.FromArgb((int)(255 * opacity), 255, 255, 255).ToArgb();
            //bool zBufferEnabled = device.RenderState.ZBufferEnable;
            //bool zBufferWrite = device.RenderState.ZBufferWriteEnable;
            //device.RenderState.ZBufferEnable = true;
            //device.RenderState.ZBufferWriteEnable = false;
            //device.SetTexture(0, null);
            //device.VertexFormat = CustomVertex.PositionTextured.Format;

            //device.PixelShader = AuroraShader.Shader;
            //AuroraShader.GreenCenter = .3f;
            //AuroraShader.RedCenter = .7f;
            //AuroraShader.Factor1 = 2f;
            //AuroraShader.Factor2 = .1f;
            //AuroraShader.Factor3 = 2f;



            //device.DrawUserPrimitives(PrimitiveType.TriangleStrip, auroraPoints.Length - 2, auroraPoints);
            //device.PixelShader = null;
            //device.RenderState.AlphaBlendEnable = false;
            //device.RenderState.ColorWriteEnable = ColorWriteEnable.RedGreenBlue;

            //device.RenderState.ZBufferEnable = zBufferEnabled;
            //device.RenderState.ZBufferWriteEnable = zBufferWrite;

            //// Restore texture states
            //device.TextureState[0].ColorOperation = TextureOperation.Modulate;
            //device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
            //device.TextureState[0].ColorArgument2 = TextureArgument.Diffuse;
            //device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
            //device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
            //device.TextureState[0].AlphaArgument2 = TextureArgument.Diffuse;

            ////
            //device.TextureState[1].ColorOperation = TextureOperation.Disable;
            //device.TextureState[1].ColorArgument1 = TextureArgument.Current;
            //device.TextureState[1].ColorArgument2 = TextureArgument.Constant;
            return true;
        }

        public static void PrepGrids(RenderContext11 renderContext)
        {
            MakePrecessionChart();
            MakeAltAzGridText();
            MakeEclipticGridText();
            MakeGalacticGridText();
            MakeEquitorialGridText();
            MakeEclipticText();
        }

        
        public static void CleanupGrids()
        {
            if (altAzLineList != null)
            {
                altAzLineList.Clear();
                altAzLineList = null;
            }

            if (precLineList != null)
            {
                precLineList.Clear();
                precLineList = null;
            }

            if (eclipticLineList != null)
            {
                eclipticLineList.Clear();
                eclipticLineList = null;
            }

            if (galLineList != null)
            {
                galLineList.Clear();
                galLineList = null;
            }

            if (equLineList != null)
            {
                equLineList.Clear();
                equLineList = null;
            }

            if (eclipticOverviewLineList != null)
            {
                eclipticOverviewLineList.Clear();
                eclipticOverviewLineList = null;
            }

            if (EclipOvTextBatch != null)
            {
                EclipOvTextBatch.Dispose();
                EclipOvTextBatch = null;
            }
            if (EclipticTextBatch != null)
            {
                EclipticTextBatch.Dispose();
                EclipticTextBatch = null;
            }
            if (GalTextBatch != null)
            {
                GalTextBatch.Dispose();
                GalTextBatch = null;
            }

            if (EquTextBatch != null)
            {
                EquTextBatch.Dispose();
                EquTextBatch = null;
            }
            if (PrecTextBatch != null)
            {
                PrecTextBatch.Dispose();
                PrecTextBatch = null;
            }
            if (AltAzTextBatch != null)
            {
                AltAzTextBatch.Dispose();
                AltAzTextBatch = null;
            }
        }


        static SimpleLineList11 precLineList;

        static Text3dBatch PrecTextBatch;
        public static bool DrawPrecessionChart(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            MakePrecessionChart();

            PrecTextBatch.Draw(renderContext, opacity, drawColor);
            
            precLineList.DrawLines(renderContext, opacity, drawColor);
            
            return true;
        }

        private static void MakePrecessionChart()
        {
            var obliquity = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow);
            var mat = Matrix3d.RotationX((obliquity / 360.0 * (Math.PI * 2)));
            var col = Color.White;
            if (precLineList == null)
            {
                precLineList = new SimpleLineList11();
                precLineList.DepthBuffered = false;

                for (double l = 0; l < 360; l++)
                {
                    var b = 90 - obliquity;
                    precLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3d((l + 1) / 15, b, 1), mat));
                }

                for (double l = -12000; l < 13000; l += 2000)
                {

                    var b = 90 - obliquity;
                    var p = -((l - 2000) / 25772 * 24) - 6;
                    precLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3d(p, b - .5, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3d(p, b + .5, 1), mat));
                }
            }
            if (PrecTextBatch == null)
            {
                PrecTextBatch = new Text3dBatch(80);


                for (double l = -12000; l < 13000; l += 2000)
                {
                    var b = 90 - obliquity + 3;

                    var p = -((l - 2000) / 25772 * 24) - 6;
                    var text = l.ToString();

                    if (l == 0)
                    {
                        b = 90 - obliquity + 2;
                        text = "1 CE";
                    }
                    else if (l < 0)
                    {

                        text = "  " + (Math.Abs(l)) + " BCE";
                    }
                    else
                    {
                        text = (Math.Abs(l)) + " CE";
                    }

                    if (text.Length == 9)
                    {
                        text = "   " + text;
                    }

                    PrecTextBatch.Add(new Text3d(Vector3d.TransformCoordinate(Coordinates.RADecTo3d(p, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3d(p + .01, b, 1), mat), text, 80, .00009));
                }
            }
            return;
        }
 
        static SimpleLineList11 altAzLineList;
        public static bool DrawAltAzGrid(RenderContext11 renderContext, float opacity, Color drawColor)
        {
        
            
            var zenithAltAz = new Coordinates(0, 0);
            var zenith = Coordinates.HorizonToEquitorial(zenithAltAz, SpaceTimeController.Location, SpaceTimeController.Now);
   
            var raPart = -((zenith.RA - 6) / 24.0 * (Math.PI * 2));
            var decPart = -(((zenith.Dec)) / 360.0 * (Math.PI * 2));
            var raText = Coordinates.FormatDMS(zenith.RA);
            var mat = Matrix3d.RotationY((float)-raPart);
            mat.Multiply(Matrix3d.RotationX((float)decPart));
            mat.Invert();

            if (altAzLineList == null)
            {
                altAzLineList = new SimpleLineList11();
                altAzLineList.DepthBuffered = false;

                var col = drawColor;
                var color = UiTools.GetTransparentColor(col.ToArgb(), .5f * opacity * (col.A / 255.0f));
                var colorBright = UiTools.GetTransparentColor(col.ToArgb(), opacity * (col.A / 255.0f));
                for (double l = 0; l < 360; l += 10)
                {
                    for (double b = -80; b < 80; b += 2)
                    {
                        altAzLineList.AddLine(Coordinates.RADecTo3d(l / 15, b, 1), Coordinates.RADecTo3d(l / 15, b+2, 1));
                    }
                }

                for (double b = -80; b <= 80; b += 10)
                {
                    for (double l = 0; l < 360; l += 5)
                    {
                        var c = color;

                        if (b == 0)
                        {
                            c = colorBright;
                        }

                        altAzLineList.AddLine(Coordinates.RADecTo3d(l / 15, b, 1), Coordinates.RADecTo3d((l + 5) / 15, b, 1));

                    }
                }

                var counter = 0;
                for (double l = 0; l < 360; l += 1)
                {

                    var b = 0.25;
                    switch (counter % 10)
                    {
                        case 0:
                            counter++;
                            continue;
                        case 5:
                            b = .5;
                            break;
                    }
                    counter++;

                    altAzLineList.AddLine(Coordinates.RADecTo3d(l / 15, b, 1), Coordinates.RADecTo3d(l  / 15, -b, 1));
                }

                counter = 0;
                for (double l = 0; l < 360; l += 90)
                {
                    counter = 0;
                    for (double b = -80; b <= 80; b += 1)
                    {
                        var width = 0.5 / 2;
                        switch (counter % 10)
                        {
                            case 0:
                                counter++;
                                continue;
                            case 5:
                                width = .5;
                                break;
                        }
                        counter++;

                        altAzLineList.AddLine(Coordinates.RADecTo3d((l + width) / 15, b, 1), Coordinates.RADecTo3d((l - width) / 15, b, 1));
                    }
                }
            }
            var matOld = renderContext.WorldBase;

            var matOldWorld = renderContext.World;
            var matOldWorldBase = renderContext.WorldBase;

            renderContext.World = renderContext.WorldBase = mat * renderContext.World;

            altAzLineList.DrawLines(renderContext, opacity, drawColor);

            renderContext.WorldBase = matOldWorldBase;
            renderContext.World = matOldWorld;
            return true;
        }

        static Text3dBatch AltAzTextBatch;
        public static bool DrawAltAzGridText(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            var zenithAltAz = new Coordinates(0, 0);
            var zenith = Coordinates.HorizonToEquitorial(zenithAltAz, SpaceTimeController.Location, SpaceTimeController.Now);

            var raPart = -((zenith.RA - 6) / 24.0 * (Math.PI * 2));
            var decPart = -(((zenith.Dec)) / 360.0 * (Math.PI * 2));
            var raText = Coordinates.FormatDMS(zenith.RA);
            var mat = Matrix3d.RotationY((float)-raPart);
            mat.Multiply(Matrix3d.RotationX((float)decPart));
            mat.Invert();

            MakeAltAzGridText();

            var matOld = renderContext.WorldBase;

            var matOldWorld = renderContext.World;
            var matOldWorldBase = renderContext.WorldBase;

            renderContext.World = renderContext.WorldBase = mat * renderContext.World;

            AltAzTextBatch.Draw(renderContext, opacity, drawColor);

            renderContext.WorldBase = matOldWorldBase;
            renderContext.World = matOldWorld;

            return true;
        }

        private static void MakeAltAzGridText()
        {
            var drawColor = Color.White;

            var index = 0;
            if (AltAzTextBatch == null)
            {
                AltAzTextBatch = new Text3dBatch(80);
                for (double l = 0; l < 360; l += 10)
                {
                    var text = "       " + l;
                    if (l < 10)
                    {
                        text = "   " + l;
                    }
                    else if (l < 100)
                    {
                        text = "     " + l;
                    }
                    var lc = 360 - l;
                    AltAzTextBatch.Add(new Text3d(Coordinates.RADecTo3d(lc / 15 - 6, .4, 1), Coordinates.RADecTo3d(lc / 15 - 6, .5, 1), text, 80, .00006));
                }

                index = 0;
                for (double l = 0; l < 360; l += 90)
                {

                    for (double b = -80; b <= 80; b += 10)
                    {
                        if (b == 0)
                        {
                            continue;
                        }
                        var text = b.ToString();
                        if (b > 0)
                        {
                            text = "  +" + b;
                            AltAzTextBatch.Add(new Text3d(Coordinates.RADecTo3d(l / 15, b - .4, 1), Coordinates.RADecTo3d(l / 15, b - .3, 1), text, 80, .00006));
                        }
                        else
                        {
                            text = "  - " + text.Substring(1);
                            AltAzTextBatch.Add(new Text3d(Coordinates.RADecTo3d(l / 15, b + .4, 1), Coordinates.RADecTo3d(l / 15, b + .5, 1), text, 80, .00006));
                        }
                        index++;
                    }
                }
            }
            return;
        }

        static SimpleLineList11 eclipticLineList;

        public static bool DrawEclipticGrid(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            if (eclipticLineList == null)
            {
                eclipticLineList = new SimpleLineList11();
                eclipticLineList.DepthBuffered = false;

                var obliquity = Coordinates.MeanObliquityOfEcliptic(2451545);
                var mat = Matrix3d.RotationX((obliquity / 360.0 * (Math.PI * 2)));

                var col = drawColor;
                var color = UiTools.GetTransparentColor(col.ToArgb(), .5f * opacity * (col.A / 255.0f));
                var colorBright = UiTools.GetTransparentColor(col.ToArgb(), opacity * (col.A / 255.0f));
                for (double l = 0; l < 360; l += 10)
                {
                    for (double b = -80; b < 80; b += 2)
                    {
                        eclipticLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15, b + 2, 1), mat));
                    }
                }

                for (double b = -80; b <= 80; b += 10)
                {
                    for (double l = 0; l < 360; l += 5)
                    {
                        var c = color;

                        if (b == 0)
                        {
                            c = colorBright;
                        }

                        eclipticLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3d((l + 5) / 15, b, 1), mat));
                    }
                }

                var counter = 0;
                for (double l = 0; l < 360; l += 1)
                {

                    var b = 0.25;
                    switch (counter % 10)
                    {
                        case 0:
                            counter++;
                            continue;
                        case 5:
                            b = .5;
                            break;
                    }
                    counter++;

                    eclipticLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15, -b, 1), mat));

                }

                counter = 0;
                for (double l = 0; l < 360; l += 90)
                {
                    counter = 0;
                    for (double b = -80; b <= 80; b += 1)
                    {
                        var width = 0.5 / 2;
                        switch (counter % 10)
                        {
                            case 0:
                                counter++;
                                continue;
                            case 5:
                                width = .5;
                                break;
                        }
                        counter++;

                        eclipticLineList.AddLine(Vector3d.TransformCoordinate(Coordinates.RADecTo3d((l + width) / 15, b, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3d((l - width) / 15, b, 1), mat));
                    }
                }
            }

            eclipticLineList.DrawLines(renderContext, opacity, drawColor);
            
            return true;
        }
        static Text3dBatch EclipticTextBatch;
        public static bool DrawEclipticGridText(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            MakeEclipticGridText();

            EclipticTextBatch.Draw(renderContext, opacity, drawColor);

            return true;
        }

        private static void MakeEclipticGridText()
        {
            var drawColor = Color.White;
            var obliquity = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow);
            var mat = Matrix3d.RotationX((float)(obliquity / 360.0 * (Math.PI * 2)));

            if (EclipticTextBatch == null)
            {
                EclipticTextBatch = new Text3dBatch(80);
                for (double l = 0; l < 360; l += 10)
                {
                    var text = "       " + l;
                    if (l < 10)
                    {
                        text = "   " + l;
                    }
                    else if (l < 100)
                    {
                        text = "     " + l;
                    }
                    EclipticTextBatch.Add(new Text3d(Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15 + 12, .4, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15 + 12, .5, 1), mat), text, 80, .00006));
                }

                for (double l = 0; l < 360; l += 90)
                {

                    for (double b = -80; b <= 80; b += 10)
                    {
                        if (b == 0)
                        {
                            continue;
                        }
                        var text = b.ToString();
                        if (b > 0)
                        {
                            text = "  +" + b;
                            EclipticTextBatch.Add(new Text3d(Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15, b - .4, 1 ), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15, b - .3, 1), mat), text, 80, .00006));
                        }
                        else
                        {
                            text = "  - " + text.Substring(1);
                            EclipticTextBatch.Add(new Text3d(Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15, b + .4, 1), mat), Vector3d.TransformCoordinate(Coordinates.RADecTo3d(l / 15, b + .5, 1), mat), text, 80, .00006));
                        }
                    }
                }
            }
            return;
        }

        static SimpleLineList11 galLineList;

        public static bool DrawGalacticGrid(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            if (galLineList == null)
            {
                galLineList = new SimpleLineList11();
                galLineList.DepthBuffered = false;

                var col = drawColor;
                var color = UiTools.GetTransparentColor(col.ToArgb(), .5f * opacity * (col.A / 255.0f));
                var colorBright = UiTools.GetTransparentColor(col.ToArgb(), opacity * (col.A / 255.0f));
                for (double l = 0; l < 360; l += 10)
                {
                    for (double b = -80; b < 80; b += 2)
                    {
                        galLineList.AddLine(Coordinates.GalacticTo3dDouble(l, b), Coordinates.GalacticTo3dDouble(l, b + 2));
                    }
                }

                for (double b = -80; b <= 80; b += 10)
                {
                    for (double l = 0; l < 360; l += 5)
                    {
                        var c = color;

                        if (b == 0)
                        {
                            c = colorBright;
                        }

                        galLineList.AddLine(Coordinates.GalacticTo3dDouble(l, b), Coordinates.GalacticTo3dDouble(l + 5, b));
                    }
                }

                var counter = 0;
                for (double l = 0; l < 360; l += 1)
                {

                    var b = 0.25;
                    switch (counter % 10)
                    {
                        case 0:
                            counter++;
                            continue;
                        case 5:
                            b = .5;
                            break;
                    }
                    counter++;

                    galLineList.AddLine(Coordinates.GalacticTo3dDouble(l, b), Coordinates.GalacticTo3dDouble(l, -b));
                }

                counter = 0;
                for (double l = 0; l < 360; l += 90)
                {
                    counter = 0;
                    for (double b = -80; b <= 80; b += 1)
                    {
                        var width = 0.5 / 2;
                        switch (counter % 10)
                        {
                            case 0:
                                counter++;
                                continue;
                            case 5:
                                width = .5;
                                break;
                        }
                        counter++;

                        galLineList.AddLine(Coordinates.GalacticTo3dDouble(l + width, b), Coordinates.GalacticTo3dDouble(l - width, b));
                    }
                }
            }
            
            galLineList.DrawLines(renderContext, opacity, drawColor);
            
            return true;
        }
        static Text3dBatch GalTextBatch;
        public static bool DrawGalacticGridText(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            MakeGalacticGridText();

            GalTextBatch.Draw(renderContext, opacity, drawColor);
            return true;
        }

        private static void MakeGalacticGridText()
        {
            if (GalTextBatch == null)
            {

                GalTextBatch = new Text3dBatch(80);
                for (var l = 0; l < 360; l += 10)
                {
                    var text = "       " + l;
                    if (l < 10)
                    {
                        text = "   " + l;
                    }
                    else if (l < 100)
                    {
                        text = "     " + l;
                    }
                    GalTextBatch.Add(new Text3d(Coordinates.GalacticTo3dDouble(l, 0.4), Coordinates.GalacticTo3dDouble(l, 0.5), text, 80, .00006));
                }

                for (double l = 0; l < 360; l += 90)
                {

                    for (double b = -80; b <= 80; b += 10)
                    {
                        if (b == 0)
                        {
                            continue;
                        }
                        var text = b.ToString();
                        if (b > 0)
                        {
                            text = "  +" + b;
                            GalTextBatch.Add(new Text3d(Coordinates.GalacticTo3dDouble(l, b - .4), Coordinates.GalacticTo3dDouble(l, b - .3), text, 80, .00006));
                        }
                        else
                        {
                            text = "  - " + text.Substring(1);
                            GalTextBatch.Add(new Text3d(Coordinates.GalacticTo3dDouble(l, b + .4), Coordinates.GalacticTo3dDouble(l, b + .5), text, 80, .00006));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Planet Grids
        /// </summary>
        static SimpleLineList11 planetLineList;

        public static bool DrawPlanetGrid(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            if (planetLineList == null)
            {
                planetLineList = new SimpleLineList11();
                planetLineList.DepthBuffered = false;

                var col = drawColor;
                var color = UiTools.GetTransparentColor(col.ToArgb(), .5f * opacity * (col.A / 255.0f));
                var colorBright = UiTools.GetTransparentColor(col.ToArgb(), opacity * (col.A / 255.0f));
                for (double lng = 0; lng < 360; lng += 10)
                {
                    for (double lat = -80; lat < 80; lat += 2)
                    {
                        planetLineList.AddLine(Coordinates.GeoTo3dDouble( lat, lng), Coordinates.GeoTo3dDouble( lat + 2, lng));
                    }
                }

                for (double lat = -80; lat <= 80; lat += 10)
                {
                    for (double l = 0; l < 360; l += 5)
                    {
                        var c = color;

                        if (lat == 0)
                        {
                            c = colorBright;
                        }

                        planetLineList.AddLine(Coordinates.GeoTo3dDouble( lat, l), Coordinates.GeoTo3dDouble( lat, l + 5));
                    }
                }

                var counter = 0;
                for (double lng = 0; lng < 360; lng += 1)
                {

                    var lat = 0.25;
                    switch (counter % 10)
                    {
                        case 0:
                            counter++;
                            continue;
                        case 5:
                            lat = .5;
                            break;
                    }
                    counter++;

                    planetLineList.AddLine(Coordinates.GeoTo3dDouble( lat, lng), Coordinates.GeoTo3dDouble( -lat, lng));
                }

                counter = 0;
                for (double lng = 0; lng < 360; lng += 90)
                {
                    counter = 0;
                    for (double b = -80; b <= 80; b += 1)
                    {
                        var width = 0.5 / 2;
                        switch (counter % 10)
                        {
                            case 0:
                                counter++;
                                continue;
                            case 5:
                                width = .5;
                                break;
                        }
                        counter++;

                        planetLineList.AddLine(Coordinates.GeoTo3dDouble( b, lng + width), Coordinates.GeoTo3dDouble( b, lng - width));
                    }
                }
            }
            planetLineList.aaFix = false;
            planetLineList.DepthBuffered = false;
            planetLineList.Sky = false;
            planetLineList.DrawLines(renderContext, opacity, drawColor);

            return true;
        }
        static Text3dBatch PlanetTextBatch;
        public static bool DrawPlanetGridText(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            MakePlanetGridText();

            PlanetTextBatch.Draw(renderContext, opacity, drawColor);
            return true;
        }

        private static void MakePlanetGridText()
        {
            if (PlanetTextBatch == null)
            {

                PlanetTextBatch = new Text3dBatch(80);
                for (var lng = -180; lng < 180; lng += 10)
                {
                    var text = "       " + lng;
                    if (lng < 10)
                    {
                        text = "   " + lng;
                    }
                    else if (lng < 100)
                    {
                        text = "     " + lng;
                    }
                    PlanetTextBatch.Add(new Text3d(Coordinates.GeoTo3dDouble( 0.4, lng), Coordinates.GeoTo3dDouble( 0.5, lng), text, -80, .00006));
                }

                for (double lng = 0; lng < 360; lng += 90)
                {

                    for (double lat = -80; lat <= 80; lat += 10)
                    {
                        if (lat == 0)
                        {
                            continue;
                        }
                        var text = lat.ToString();
                        if (lat > 0)
                        {
                            text = "  +" + lat;
                            PlanetTextBatch.Add(new Text3d(Coordinates.GeoTo3dDouble( lat - .4, lng), Coordinates.GeoTo3dDouble( lat - .3, lng), text, -80, .00006));
                        }
                        else
                        {
                            text = "  - " + text.Substring(1);
                            PlanetTextBatch.Add(new Text3d(Coordinates.GeoTo3dDouble( lat + .4, lng), Coordinates.GeoTo3dDouble( lat + .5, lng), text, -80, .00006));
                        }
                    }
                }
            }
        }

        // end planet Grids



        static SimpleLineList11 equLineList;
        public static bool DrawEquitorialGrid(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            if (equLineList == null)
            {
                equLineList = new SimpleLineList11();
                equLineList.DepthBuffered = false;



                var col = drawColor;
                var color = UiTools.GetTransparentColor(col.ToArgb(), .5f * opacity * (col.A / 255.0f));
                var colorBright = UiTools.GetTransparentColor(col.ToArgb(), opacity * (col.A / 255.0f));
                for (double hour = 0; hour < 24; hour++)
                {
                    for (double dec = -80; dec < 80; dec += 2)
                    {
                        equLineList.AddLine(Coordinates.RADecTo3d(hour, dec, 1), Coordinates.RADecTo3d(hour, dec + 2, 1));
                    }
                }


                for (double dec = -80; dec <= 80; dec += 10)
                {
                    for (double hour = 0; hour < 24; hour += .2)
                    {
                        var c = color;

                        if (dec == 0)
                        {
                            c = colorBright;
                        }

                        equLineList.AddLine(Coordinates.RADecTo3d(hour, dec, 1), Coordinates.RADecTo3d(hour + .2, dec, 1));
                        //todo fix for color bright
                    }
                }


                var counter = 0;
                for (double ra = 0; ra < 24; ra += .25)
                {
                    var dec = 0.5;

                    switch (counter % 4)
                    {
                        case 0:
                            counter++;
                            continue;
                        case 3:
                        case 1:
                            dec = .25;
                            break;
                    }
                    counter++;

                    equLineList.AddLine(Coordinates.RADecTo3d(ra, dec, 1), Coordinates.RADecTo3d(ra, -dec, 1));
                }
                counter = 0;
                for (double ra = 0; ra < 24; ra += 3)
                {
                    counter = 0;
                    for (double dec = -80; dec <= 80; dec += 1)
                    {
                        var width = 0.5 / 30;
                        switch (counter % 10)
                        {
                            case 0:
                                counter++;
                                continue;
                            case 5:
                                width = .5 / 15;
                                break;
                        }

                        counter++;

                        equLineList.AddLine(Coordinates.RADecTo3d(ra + width, dec, 1), Coordinates.RADecTo3d(ra - width, dec, 1));
                    }
                }
            }

            equLineList.DrawLines(renderContext, opacity, drawColor);

            return true;
        }
     
        
        static Text3dBatch EquTextBatch;
        public static bool DrawEquitorialGridText(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            MakeEquitorialGridText();

            EquTextBatch.Draw(renderContext, opacity, drawColor);
            return true;
        }

        private static void MakeEquitorialGridText()
        {
            if (EquTextBatch == null)
            {
                EquTextBatch = new Text3dBatch(80);
                var index = 0;

                for (var ra = 0; ra < 24; ra++)
                {
                    var text = ra + " hr";
                    if (ra < 10)
                    {
                        text = "  " + ra + " hr";
                    }

                    EquTextBatch.Add(new Text3d(Coordinates.RADecTo3d(ra + 12.005, 0.4, 1), Coordinates.RADecTo3d(ra + 12.005, 0.5, 1), text, 80, .00006));
                }

                index = 0;
                for (double ra = 0; ra < 24; ra += 3)
                {

                    for (double dec = -80; dec <= 80; dec += 10)
                    {
                        if (dec == 0)
                        {
                            continue;
                        }
                        var text = dec.ToString();
                        if (dec > 0)
                        {
                            text = "  +" + dec;
                            EquTextBatch.Add(new Text3d(Coordinates.RADecTo3d(ra + 12, dec - .4, 1), Coordinates.RADecTo3d(ra + 12, dec - .3, 1), text, 80, .00006));
                        }
                        else
                        {
                            text = "  - " + text.Substring(1);
                            EquTextBatch.Add(new Text3d(Coordinates.RADecTo3d(ra + 12, dec + .4, 1), Coordinates.RADecTo3d(ra + 12, dec + .5, 1), text, 80, .00006));
                        }

                        index++;
                    }
                }
            }
        }

       
        static int EclipticCount;
        static int EclipticYear;

        static readonly double[] monthDays = new double[] { 31, 28.2421, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        static readonly string[] monthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        static SimpleLineList11 eclipticOverviewLineList;

       
        public static bool DrawEcliptic(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            var col = drawColor;

            var year = SpaceTimeController.Now.Year;

            if (eclipticOverviewLineList == null || year != EclipticYear)
            {


                if (eclipticOverviewLineList != null)
                {
                    eclipticOverviewLineList.Clear();
                    GC.SuppressFinalize(eclipticOverviewLineList);
                    eclipticOverviewLineList = null;
                }

                EclipticYear = year;
                var obliquity = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow);
                var mat = Matrix3d.RotationX((obliquity / 360.0 * (Math.PI * 2)));


                var daysPerYear = 365.25;



                if (DateTime.IsLeapYear(year))
                {
                    monthDays[1] = 29;

                    daysPerYear = 366;
                }
                else
                {
                    monthDays[1] = 28;
                    daysPerYear = 365;
                }
                var count = 2 * (int)daysPerYear;
                EclipticCount = (int)daysPerYear;
                var jYear = SpaceTimeController.UtcToJulian(new DateTime(year, 1, 1, 12, 0, 0));


                var index = 0;
                double d = 0;

                eclipticOverviewLineList = new SimpleLineList11();
                eclipticOverviewLineList.DepthBuffered = false;
                for (var m = 0; m < 12; m++)
                {
                    var daysThisMonth = (int)monthDays[m];
                    for (var i = 0; i < daysThisMonth; i++)
                    {
                        var sunRaDec = Planets.GetPlanetLocation("Sun", jYear);

                        var sunEcliptic = CAACoordinateTransformation.Equatorial2Ecliptic(sunRaDec.RA, sunRaDec.Dec, obliquity);

                        d = sunEcliptic.X;

                        double width = .005f;
                        if (i == 0)
                        {
                            width = .01f;
                        }
                        var dd = d + 180;

                        eclipticOverviewLineList.AddLine(
                                    Vector3d.TransformCoordinate(new Vector3d((Math.Cos((dd * Math.PI * 2.0) / 360)),
                                    width,
                                    (Math.Sin((dd * Math.PI * 2.0) / 360))), mat),
                                    Vector3d.TransformCoordinate(new Vector3d((Math.Cos((dd * Math.PI * 2.0) / 360)),
                                    -width,
                                    (Math.Sin((dd * Math.PI * 2.0) / 360))), mat)
                                                         );


                        index++;
                        jYear += 1;
                    }
                    d += monthDays[m];
                }
                
            }


            eclipticOverviewLineList.DrawLines(renderContext, opacity, drawColor);
            return true;
        }

        static int EclipticTextYear;
        static Text3dBatch EclipOvTextBatch;
        public static bool DrawEclipticText(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            MakeEclipticText();

            EclipOvTextBatch.Draw(renderContext, opacity, drawColor);

            return true;
        }

        private static void MakeEclipticText()
        {
            var year = SpaceTimeController.Now.Year;

            if (EclipOvTextBatch == null)
            {
                EclipOvTextBatch = new Text3dBatch(80);

                EclipticTextYear = year;
                var obliquity = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow);
                var mat = Matrix3d.RotationX((obliquity / 360.0 * (Math.PI * 2)));

                var daysPerYear = 365.25;

                if (DateTime.IsLeapYear(year))
                {
                    monthDays[1] = 29;

                    daysPerYear = 366;
                }
                else
                {
                    monthDays[1] = 28;
                    daysPerYear = 365;
                }
                var count = 2 * (int)daysPerYear;
                EclipticCount = (int)daysPerYear;
                var jYear = SpaceTimeController.UtcToJulian(new DateTime(year, 1, 1, 12, 0, 0));


                var index = 0;
                double d = 0;

                for (var m = 0; m < 12; m++)
                {
                    var daysThisMonth = (int)monthDays[m];
                    for (var i = 0; i < daysThisMonth; i++)
                    {
                        var sunRaDec = Planets.GetPlanetLocation("Sun", jYear);

                        var sunEcliptic = CAACoordinateTransformation.Equatorial2Ecliptic(sunRaDec.RA, sunRaDec.Dec, obliquity);

                        d = sunEcliptic.X;

                        var dd = d + 180;

                        if (i == daysThisMonth / 2)
                        {
                            var center = Vector3d.TransformCoordinate(new Vector3d((Math.Cos((dd * Math.PI * 2.0) / 360)),
                                                         .025f,
                                                         (Math.Sin((dd * Math.PI * 2.0) / 360))), mat);
                            var up = Vector3d.TransformCoordinate(new Vector3d((Math.Cos((dd * Math.PI * 2.0) / 360)),
                                                         .045f,
                                                         (Math.Sin((dd * Math.PI * 2.0) / 360))), mat);
                            up.Subtract(center);

                            up.Normalize();
                            EclipOvTextBatch.Add(new Text3d(center, up, monthNames[m], 80, .000159375));

                        }


                        index++;

                        index++;
                        jYear += 1;
                    }
                    d += monthDays[m];
                }
            }
        }

    
        static String[] directions = null;

        public static bool DrawHorizon(RenderContext11 renderContext, float opacity)
        {
 
            var zenithAltAz = new Coordinates(0, 0);
            zenithAltAz.Alt = 89.999999;
            zenithAltAz.Az = 0.0000001;

            var zenith = Coordinates.HorizonToEquitorial(zenithAltAz, SpaceTimeController.Location, SpaceTimeController.Now);
            var raPart = (zenith.RA / 24.0 * (Math.PI * 2));
            var decPart = ((90 - zenith.Dec) / 360.0 * (Math.PI * 2));
            var raText = Coordinates.FormatDMS(zenith.RA);
            var mat = Matrix.RotationZ((float)decPart);
            mat = Matrix.Multiply(mat, Matrix.RotationY((float)-raPart));



            var color = Color.FromArgb((int)(255f*opacity), 0, 0, 32);
            var count = 90;


            var points = new PositionColoredTextured[(count*2 + 3)];

            points[0].Position = new Vector4(Vector3.TransformCoordinate(new Vector3(0, -1, 0), mat), 1);

            points[0].Color = color;

            var index = 1;

            for (var i = 0; i < count + 1; i++)
            {

                points[index].Position = new Vector4(Vector3.TransformCoordinate(new Vector3((float)(.9 * Math.Cos((i * Math.PI * 2.0) / count)),
                                                 0,
                                                 (float)(.9 * Math.Sin((i * Math.PI * 2.0) / count))), mat), 1);


                points[index++].Color = color;

                points[index++] = points[0];

            }

            Sprite2d.Draw(renderContext, points, points.Length, null, PrimitiveTopology.TriangleStrip);

            //todo11 enable text
            //device.DrawUserPrimitives(PrimitiveType.TriangleFan, count, points);

            //device.RenderState.CullMode = Cull.Clockwise;
            //device.TextureState[0].ColorOperation = oldTexOp;


            //device.RenderState.AlphaBlendEnable = false;
            //device.RenderState.ColorWriteEnable = ColorWriteEnable.RedGreenBlue;

            //// Draw Direction indicators
            //Vector3[] cardinals = new Vector3[8];
            //if (directions == null)
            //{
            //    directions = new string[] { Language.GetLocalizedText(241, "North"), Language.GetLocalizedText(242, "North-East"), Language.GetLocalizedText(243, "East"), Language.GetLocalizedText(245, "South-East"), Language.GetLocalizedText(246, "South"), Language.GetLocalizedText(247, "South-West"), Language.GetLocalizedText(248, "West"), Language.GetLocalizedText(249, "North-West") };
            //}

            //cardinals[0] = Coordinates.RADecTo3d(Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(-.1, 0), SpaceTimeController.Location, SpaceTimeController.Now), .8);
            //cardinals[1] = Coordinates.RADecTo3d(Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(-.1, 45), SpaceTimeController.Location, SpaceTimeController.Now), .8);
            //cardinals[2] = Coordinates.RADecTo3d(Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(-.1, 90), SpaceTimeController.Location, SpaceTimeController.Now), .8);
            //cardinals[3] = Coordinates.RADecTo3d(Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(-.1, 135), SpaceTimeController.Location, SpaceTimeController.Now), .8);
            //cardinals[4] = Coordinates.RADecTo3d(Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(-.1, 180), SpaceTimeController.Location, SpaceTimeController.Now), .8);
            //cardinals[5] = Coordinates.RADecTo3d(Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(-.1, 225), SpaceTimeController.Location, SpaceTimeController.Now), .8);
            //cardinals[6] = Coordinates.RADecTo3d(Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(-.1, 270), SpaceTimeController.Location, SpaceTimeController.Now), .8);
            //cardinals[7] = Coordinates.RADecTo3d(Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(-.1, 315), SpaceTimeController.Location, SpaceTimeController.Now), .8);

            //for (int i = 0; i < cardinals.GetLength(0); i++)
            //{

            //    cardinals[i] = Vector3.Project(cardinals[i], device.Viewport, device.Transform.Projection, device.Transform.View, device.Transform.World);
            //}

            //sprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortTexture);
            //for (int i = 0; i < cardinals.GetLength(0); i++)
            //{
            //    if (Math.Abs(Earth3d.MainWindow.Az - i * 45) < 120 || Math.Abs((Earth3d.MainWindow.Az - 360) - i * 45) < 120)
            //    {
            //        Rectangle recttext = new Rectangle((int)(cardinals[i].X), (int)(cardinals[i].Y), 0, 0);
            //        Earth3d.MainWindow.labelFont.DrawText(sprite, directions[i], recttext,
            //        DrawTextFormat.NoClip | DrawTextFormat.Center, System.Drawing.Color.White);
            //    }

            //}
            //sprite.End();




            return true;
        }

        public static void DumpStars()
        {
            var tiles = new List<ToastTools>();
            var targetLevel = 6;

            while (targetLevel-- > -1)
            {

                foreach (var star in stars)
                {
                    var tile = ToastTools.GetTileForLevelPoint(targetLevel, star.Dec, (360 - (star.RA * 15)) - 180);

                    if (tile.Tag == null)
                    {
                        tile.Tag = new List<Star>();
                        tiles.Add(tile);
                    }
                    ((List<Star>)tile.Tag).Add(star);
                }

                foreach (var tile in tiles)
                {
                    //open file

                    var path = string.Format("c:\\dumpstars\\{0}\\{2}", tile.Level, tile.X, tile.Y);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    var sw = new StreamWriter(string.Format("c:\\dumpstars\\{0}\\{2}\\{1}_{2}.txt", tile.Level, tile.X, tile.Y));

                    //loop thru stars
                    var starList = (List<Star>)tile.Tag;

                    //write header
                    Star.WriteHeader(sw);

                    //write data
                    foreach (var star in starList)
                    {
                        star.WriteText(sw);
                    }

                    //close file
                    sw.Close();
                }
            }

        }

        private static int currentSearchID;

        public static IPlace FindClosestObject(Vector3d orig, Vector3d ray)
        {
            currentSearchID++;
            var thisId = currentSearchID;

            var target = Earth3d.MainWindow.viewCamera.ViewTarget;

            if (stars == null)
            {
                return null;
            }
            var closest = stars[0].Place;
            double maxDot = -2;

            var zoom = Earth3d.MainWindow.ZoomFactor;
            var distAlpha = ((Math.Log(Math.Max(1, zoom), 4)) - 15.5) * 90;

            var alpha = Math.Min(255, Math.Max(0, (int)distAlpha));

            //        alpha = 0;

            if (alpha < 3)
            {


                foreach (var star in stars)
                {
                    var temp = star.Position - orig;

                    temp.Normalize();

                    var dot = Vector3d.Dot(ray, temp);

                    if (dot > maxDot)
                    {
                        closest = star.Place;
                        maxDot = dot;
                    }
                    if (currentSearchID != thisId)
                    {
                        return null;
                    }
                }

                var match = Search.FindCatalogObject(closest.Name);
                if (match != null)
                {
                    closest = match;
                }



                var index = 0;
                orig.Add(target);
                foreach (var s in Enum.GetNames(typeof(SolarSystemObjects)))
                {
                    if (index <= (int)SolarSystemObjects.Callisto || index == (int)SolarSystemObjects.Earth)
                    {
                        var temp = Planets.GetPlanet3dLocation((SolarSystemObjects)Planets.GetPlanetIDFromName(s));
                        temp.Subtract(target);
                        temp.Subtract(orig);

                        temp.Normalize();
                        var dot = Vector3d.Dot(ray, temp);
                        if (dot > .9)
                        {
                            var tmp = 1 - dot;
                            tmp = tmp / 5;
                            dot = 1 - tmp;

                            if (dot > maxDot)
                            {
                                //todo localization
                                closest = Search.FindCatalogObject(s);
                                maxDot = dot;
                            }
                        }
                    }
                    index++;
                }
            }
            else
            {
                foreach (var gal in cosmos)
                {
                    var temp = gal.Position - orig;

                    temp.Normalize();

                    var dot = Vector3d.Dot(ray, temp);

                    if (dot > maxDot)
                    {
                        closest = gal.Place;
                        maxDot = dot;
                    }
                    if (currentSearchID != thisId)
                    {
                        return null;
                    }
                }

                if (customCosmos != null)
                {
                    foreach (var gal in customCosmos)
                    {
                        var temp = gal.Position - orig;

                        temp.Normalize();

                        var dot = Vector3d.Dot(ray, temp);

                        if (dot > maxDot)
                        {
                            closest = gal.Place;
                            maxDot = dot;
                        }
                        if (currentSearchID != thisId)
                        {
                            return null;
                        }
                    }
                }

            }

            return closest;
        }
        
        public static IPlace FindClosestMatch(string constellationID, double ra, double dec, double maxRadius)
        {
            currentSearchID++;
            var thisId = currentSearchID;

            if (stars == null)
            {
                return null;
            }

            
            IPlace closest = null;
            
            var zoom = Earth3d.MainWindow.ZoomFactor;

            if (zoom > 30)
            {
                return null;
            }



            var minDistance = 360.0 * 360.0;
            IPlace closestPlace = null;
            foreach (var star in stars)
            {
                var distanceRa = (ra - star.RA) * Math.Cos(dec / 180 * Math.PI) * 15;
                var distanceDec = (dec - star.Dec);
                var distanceSquared = (distanceDec * distanceDec) + (distanceRa * distanceRa);
                if (distanceSquared < minDistance)
                {
                    minDistance = distanceSquared;
                    closestPlace = star.Place;
                }
                if (currentSearchID != thisId)
                {
                    return null;
                }      
            }
            if (closest != null)
            {
                var match = Search.FindCatalogObject(closest.Name);
                if (match != null)
                {
                    closest = match;
                }
            }



            foreach (var gal in cosmos)
            {
                var distanceRa = (ra - gal.RA) * Math.Cos(dec / 180 * Math.PI) * 15;
                var distanceDec = (dec - gal.Dec);
                var distanceSquared = (distanceDec * distanceDec) + (distanceRa * distanceRa);
                if (distanceSquared < minDistance)
                {
                    minDistance = distanceSquared;
                    closestPlace = gal.Place;
                }
                if (currentSearchID != thisId)
                {
                    return null;
                }
            }


            if ((maxRadius * maxRadius) > minDistance)
            {
                return closestPlace;
            }
            return null;
        }
       

        public static void DrawStars3D(RenderContext11 renderContext, float opacity)
        {
            var zoom = Earth3d.MainWindow.ZoomFactor;
            //double distAlpha = ((Math.Log(Math.Max(1, zoom), 4)) - 15.5) * 90;



            var distAlpha = Math.Max(Math.Min(255, (Math.Log(zoom) - 15.5) * 40.8), 0);

            var alpha = Math.Min(255, Math.Max(0, (int)distAlpha));
            if (alpha > 254)
            {
                return;
            }

            var filterCount = Math.Max(0, Math.Min(starCount, (int)Math.Pow(1.1236, Properties.Settings.Default.ImageQuality / 2.5 + 60.0) - 1150));
            var d = Properties.Settings.Default.ImageQuality / 2.5 + 60.0;

            renderContext.BlendMode = BlendMode.Additive;
            renderContext.DepthStencilMode = DepthStencilMode.Off;
            renderContext.setRasterizerState(TriangleCullMode.Off);

            alpha = (int)((255 - alpha) * opacity);
            
            if (starSprites == null)
            {
                InitStarVertexBuffer(renderContext.Device);
            }

            starSprites.Draw(renderContext, filterCount, StarProfile, alpha / 255.0f);
            renderContext.BlendMode = BlendMode.Alpha;
        }


        public static void InitStarVertexBuffer(Device device)
        {
            starMutex.WaitOne();
            try
            {
                if (starSprites == null)
                {
                    InitializeStarDB(false);

                    var ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

                    var count = stars.Count;
                    var index = 0;
                    starCount = count;

                    var points = new PositionColorSize[stars.Count];
                    foreach (var star in stars)
                    {
                        var pos = Coordinates.RADecTo3d(star.RA, star.Dec, star.Distance);
                        pos.RotateX(ecliptic);
                        star.Position = pos;

                        var posf = pos.Vector3;
                        points[index].X = posf.X;
                        points[index].Y = posf.Y;
                        points[index].Z = posf.Z;
                        points[index].Color = star.Col;

                        var radDec = (1200000) / Math.Pow(1.6, star.AbsoluteMagnitude);
                        points[index].size = (float)radDec;
                        index++;
                    }

                    starSprites = new PointSpriteSet(device, points);
                }
            }
            finally
            {
                starMutex.ReleaseMutex();
            }
        }

        private static PointSpriteSet starSprites;

        static int starCount;

        static PositionColorSizeVertexBuffer11 starVertexBuffer2D;
        static int starCount2d;

        static Texture11 starProfile;

        public static Texture11 StarProfile
        {
            get
            {
                if (starProfile == null)
                {
                    starProfile = Planets.LoadPlanetTexture( Resources.StarProfile);
                }
                return starProfile;
            }
            set { starProfile = value; }
        }
        static double limitingMagnitude = 16;
        public static void DrawStars(RenderContext11 renderContext, float opacity)
        {
            if (starVertexBuffer2D == null)
            {
                InitializeStarDB(false);

                var count = stars.Count;
                var index = 0;
                starCount2d = count;

                starVertexBuffer2D = new PositionColorSizeVertexBuffer11( count, RenderContext11.PrepDevice);

                var points = (PositionColorSize[])starVertexBuffer2D.Lock(0, 0); // Lock the buffer (which will return our structs)
                foreach (var star in stars)
                {
                    var pos = Coordinates.RADecTo3d(star.RA, star.Dec, 1f);
                    points[index].Position = pos.Vector3;
                    points[index].Color = star.Col;
                    var radDec = (.5) / Math.Pow(1.6, star.Magnitude);
                    points[index].size = (float)radDec;
                    index++;
                }
                starVertexBuffer2D.Unlock();
            }

            renderContext.SetVertexBuffer(starVertexBuffer2D);
            renderContext.BlendMode = BlendMode.Additive;
            renderContext.DepthStencilMode = DepthStencilMode.Off;
            renderContext.setRasterizerState(TriangleCullMode.Off);
            var mvp = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            mvp.Transpose();
            PointSpriteShader11.WVPMatrix = mvp;
            PointSpriteShader11.Color = SharpDX.Color.White;
            PointSpriteShader11.ViewportScale = new Vector2(2.0f / renderContext.ViewPort.Width, 2.0f / renderContext.ViewPort.Height);
            PointSpriteShader11.PointScaleFactors = new Vector3(0.0f, 0.0f, 10000.0f);
            PointSpriteShader11.Use(renderContext.Device.ImmediateContext);

            renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, StarProfile.ResourceView);

            var filterCount = Math.Max(0, Math.Min(starCount, (int)Math.Pow(1.1236, Properties.Settings.Default.ImageQuality/2.5+60.0)-1150));
            var d =  Properties.Settings.Default.ImageQuality / 2.5 + 60.0;

            renderContext.devContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
            renderContext.devContext.Draw(filterCount, 0);

            renderContext.Device.ImmediateContext.GeometryShader.Set(null);

        } 
  
        static List<Star> stars;
        static readonly Dictionary<int, Star> hipparcosIndex = new Dictionary<int, Star>();
        static readonly Mutex starMutex = new Mutex();
        static readonly Mutex cosmosMutex = new Mutex();

        static public Coordinates GetHipparcosStarCoordinates(int id)
        {
            if (hipparcosIndex.ContainsKey(id))
            {
                return hipparcosIndex[id].Coordinates;
            }
            return new Coordinates();
        }

        static public void InitializeStarDB(bool TextFile)
        {
            starMutex.WaitOne();
            try
            {
                if (stars == null)
                {
                    if (TextFile)
                    {

                        if (stars == null)
                        {
                            stars = new List<Star>();
                            var filename = Properties.Settings.Default.CahceDirectory + @"data\hipparcos.txt";
                            DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=hipparcos", filename, false, true);
                            var sr = new StreamReader(filename);
                            string line;
                            Star star;
                            while (sr.Peek() >= 0)
                            {
                                line = sr.ReadLine();

                                star = new Star(line);
                                if (star.Magnitude < limitingMagnitude && star.Par > .001)
                                {
                                    stars.Add(star);
                                    hipparcosIndex.Add(star.ID, star);
                                }
                            }
                            sr.Close();

                            //// Write Binary file
                            //DumpStarBinaryFile(@"c:\hip.bin");

                        }
                    }
                    else
                    {
                        if (stars == null)
                        {
                            stars = new List<Star>();
                            var filename = Properties.Settings.Default.CahceDirectory + @"data\hip.bin";
                            DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=hipbin", filename, false, true);
                            var fs = new FileStream(filename, FileMode.Open);
                            var len = fs.Length;
                            var br = new BinaryReader(fs);
                            Star star;
                            try
                            {
                                while (fs.Position < len)
                                {
                                    star = new Star(br);
                                    stars.Add(star);
                                    hipparcosIndex.Add(star.ID, star);
                                }
                            }
                            catch
                            {
                            }
                            br.Close();
                            fs.Close();
                        }
                    }
                }
            }
            finally
            {
                starMutex.ReleaseMutex();
            }
        }

        public static void DumpStarBinaryFile(string filename)
        {
            if (stars != null)
            {
                var fs = new FileStream(filename,FileMode.Create);
                var bw = new BinaryWriter(fs);
                foreach(var star in stars)
                {
                    star.WriteBin(bw);
                }
                bw.Close();
                fs.Close();
            }
        }
        public static void DumpGalaxyBinaryFile(string filename)
        {
            if (cosmos != null)
            {
                var fs = new FileStream(filename, FileMode.Create);
                var bw = new BinaryWriter(fs);
                foreach (var galaxy in cosmos)
                {
                    galaxy.WriteBin(bw);
                }
                bw.Close();
                fs.Close();
            }
        }

        static PointSpriteSet milkyWaySprites;
        static PointSpriteSet occludingSprites;
        // static PositionColorSizeVertexBuffer11 galaxyVertexBuffer = null;
        static InputLayout galaxyImageInputLayout;
        static int galaxyPointCount;
        static Texture11 galacticCloud;
        static Texture11 milkyWayImage;
        public static void DrawGalaxy3D(RenderContext11 renderContext, float opacity)
        {
            var device = renderContext.Device;

            // Calculate the closest view angle 
            var viewAngle = (Earth3d.ViewPoint - renderContext.CameraPosition).Vector3;

            viewAngle.Normalize();

            // Draw Milky Way image
            var fadeOut = (float)(Math.Min(.1, Math.Abs(Vector3.Dot(viewAngle, MilkyWayNormal))) * 10);

            if (milkyWaySprites == null || !Properties.Settings.Default.MilkyWayModel)
            {
                DrawGalaxyImage(renderContext, 1 * opacity);
                return;
            }
            DrawGalaxyImage(renderContext, .75f * fadeOut);

            var lowestIndex = -1;
            float lowestValue = 100000;

            for (var i = 0; i < 6; i++)
            {
                var value = Vector3.Dot(viewAngle, viewPoints[i]);

                if (value < lowestValue)
                {
                    lowestIndex = i;
                    lowestValue = value;
                }
            }

            var zoom = Earth3d.MainWindow.ZoomFactor;

            if (zoom < 619926335)
            {
                lowestIndex = 6;
            }


            renderContext.DepthStencilMode = DepthStencilMode.Off;
            renderContext.setRasterizerState(TriangleCullMode.Off);
            renderContext.BlendMode = BlendMode.Alpha;


            milkyWaySprites.TintColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
            milkyWaySprites.Draw(renderContext, galaxyPointCount, galacticCloud, 1f, MilkyWayIndexBuffers[lowestIndex]);


            // Opacity does not work well on sprites. Fade the entire galaxy by drawing black over it if partially opacity
            if (opacity > 0 && opacity < 1)
            {

                var color = Color.FromArgb( UiTools.Gamma(255 - (int)(opacity * 255), 1 / 2.2f), Color.Black);

                fadePoints[0].X = 0;
                fadePoints[0].Y = renderContext.ViewPort.Height;
                fadePoints[0].Z = 0;
                fadePoints[0].Tu = 0;
                fadePoints[0].Tv = 1;
                fadePoints[0].W = 1;
                fadePoints[0].Color = color;
                fadePoints[1].X = 0;
                fadePoints[1].Y = 0;
                fadePoints[1].Z = 0;
                fadePoints[1].Tu = 0;
                fadePoints[1].Tv = 0;
                fadePoints[1].W = 1;
                fadePoints[1].Color = color;
                fadePoints[2].X = renderContext.ViewPort.Width;
                fadePoints[2].Y = renderContext.ViewPort.Height;
                fadePoints[2].Z = 0;
                fadePoints[2].Tu = 1;
                fadePoints[2].Tv = 1;
                fadePoints[2].W = 1;
                fadePoints[2].Color = color;
                fadePoints[3].X = renderContext.ViewPort.Width;
                fadePoints[3].Y = 0;
                fadePoints[3].Z = 0;
                fadePoints[3].Tu = 1;
                fadePoints[3].Tv = 0;
                fadePoints[3].W = 1;
                fadePoints[3].Color = color;

                Sprite2d.DrawForScreen(renderContext, fadePoints, 4, null, PrimitiveTopology.TriangleStrip);
            }
        }
        static readonly PositionColoredTextured[] fadePoints = new PositionColoredTextured[4];

        public static void DrawGalaxyImage(RenderContext11 renderContext, float opacity)
        {
            var device = renderContext.Device;
            if (galaxyImageIndexBuffer == null)
            {
                CreateGalaxyImage(device);
            }

            renderContext.setRasterizerState(TriangleCullMode.Off);
            renderContext.BlendMode = BlendMode.Additive;

            var zoom = Earth3d.MainWindow.ZoomFactor;
            var log = Math.Log(Math.Max(1, zoom), 4);
            var distAlpha = ((log) - 14) * 128;

            var alpha = (int)( Math.Min(255, (int)Math.Max(0,distAlpha))*opacity);

            renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, milkyWayImage.ResourceView);
            renderContext.SetupBasicEffect(BasicEffect.TextureColorOpacity, opacity, Color.FromArgb(alpha, alpha, alpha, alpha));
            if (galaxyImageInputLayout == null)
            {
                galaxyImageInputLayout = new InputLayout(device, renderContext.Shader.InputSignature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32_Float,     0, 0),
                    new InputElement("TEXCOORD", 0, Format.R32G32_Float,       12, 0),   
                });
            }

            renderContext.Device.ImmediateContext.InputAssembler.InputLayout = galaxyImageInputLayout;
            renderContext.SetVertexBuffer(galaxyImageVertexBuffer);
            renderContext.SetIndexBuffer(galaxyImageIndexBuffer);
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            renderContext.PreDraw();
            device.ImmediateContext.DrawIndexed(galaxyImageTriangleCount * 3, 0, 0);
        }

        public static Vector3[] viewPoints = new Vector3[7];

        public static Vector3 MilkyWayNormal = new Vector3();

        public static void MakeMilkyWay(Device device)
        {
            starMutex.WaitOne();
            if (milkyWaySprites == null)
            {
                //Compute MW Normal
                MilkyWayNormal = GetMWPoint(0, 1, 0).Vector3;
                MilkyWayNormal.Normalize();


                CreateGalaxyImage(device);

                var pointList = new List<PositionColorSize>();

                if (galacticCloud == null)
                {
                    galacticCloud = Planets.LoadPlanetTexture(Resources.StarProfileAlpha);
                }


                var url = "http://cdn.worldwidetelescope.org/wwtweb/catalog.aspx?q=MilkyWayModel.pts";
                var filename = string.Format(@"{0}data\MilkyWayModel.pnts", Properties.Settings.Default.CahceDirectory);
                DataSetManager.DownloadFile(url, filename, false, true);

                if (!File.Exists(filename))
                {
                    return;
                    var count = 0;

                    count = MakeSampleSprites(pointList, @"c:\milkyway\MilkyWayMap.png", 1, 1, Color.White, 1, 25, 1, 1);

                    //// Arm Glow
                    //count = MakeGalaxySprites(pointList, @"c:\milkyway\diskdensity.tif", 16, 4, Color.FromArgb(25, 103, 112, 150), 20, 5, 1, 1);

                    //// Core Bar
                    //count = MakeBarSprites(pointList, @"c:\milkyway\BAR.tif", 16, 4, Color.FromArgb(128, 241, 230, 160), 20, 5, .7, 1);

                    //// Blue Star Forming Regions
                    //count = MakeGalaxySprites(pointList, @"c:\milkyway\blueSFR.tif", 16, 4, Color.FromArgb(35, 50, 50, 255), 20, 5, .7, 1);

                    //// Bright Stars
                    count = MakeGalaxySprites(pointList, @"c:\milkyway\brightstars.tif", 1, .1f, Color.FromArgb(255, 255, 255, 255), 5, 128, 4, 50);

                    //// H2 Regions
                    //count = MakeGalaxySprites(pointList, @"c:\milkyway\h2Regions.tif", 16, 4, Color.FromArgb(35, 255, 50, 50), 20, 5, .5f, 1);

                    //// Dust Lanes
                    ////count = MakeGalaxySprites(pointList, @"c:\milkyway\Dustlanes.tif", 4, 3, Color.FromArgb(128, 47, 30, 13), 10, 5, .5, 2);

                    count = MakeGalaxySprites(pointList, @"c:\milkyway\Dustlanes.tif", 4, 3, Color.FromArgb(128, 0, 0, 0), 10, 5, 1, 2);

                    // Save File
                    WritePointListFile(pointList, filename);

                }
                pointList = ReadPointListFile(filename);


                // Make indexes for difference angles around the milkyway
                var angleIndex = 0;
                for (double angle = 0; angle <= 540; angle += 90)
                {
                    var list = new List<SortSprite>();
                    var viewPoint = GetMWPoint(Math.Cos(angle/180*Math.PI) * 1000, 0, Math.Sin(angle/180*Math.PI) * 1000).Vector3;

                    switch ((int)angle)
                    {
                        case 360: // Top
                            viewPoint = GetMWPoint(0, 1000, 0).Vector3;
                            break;
                        case 450: // Bottom
                            viewPoint = GetMWPoint(0, -1000, 0).Vector3;
                            break;
                        case 540:
                            viewPoint = new Vector3(0, 0, 0);
                            break;
                    }


                    /////see viewpoints
                    /////
                    //PositionColorSize vert = new PositionColorSize();

                    //vert.X = (float)viewPoint.X;
                    //vert.Y = (float)viewPoint.Y;
                    //vert.Z = (float)viewPoint.Z;
                    //vert.Color = Color.Red;
                    //vert.size = 10000000000;
                    //pointList.Add(vert);

                    /////end viewpoints




                    viewPoints[angleIndex] = viewPoint;
                    viewPoints[angleIndex].Normalize();


                    var indexFilename = "";
                    var reverse = false;
                    switch (angleIndex)
                    {
                        case 0:
                            indexFilename = filename + ".index0";
                            break;
                        case 1:
                            indexFilename = filename + ".index1";
                            break;
                        case 2:
                            indexFilename = filename + ".index0";
                            reverse = true;
                            break;
                        case 3:
                            indexFilename = filename + ".index1";
                            reverse = true;
                            break;
                        case 4:
                            indexFilename = filename + ".index2";
                            break;
                        case 5:
                            indexFilename = filename + ".index2";
                            reverse = true;
                            break;
                        case 6:
                        default:
                            indexFilename = filename + ".indexC";
                            break;
                    }

                    uint[] indexes = null;

                    if (!File.Exists(indexFilename))
                    {

                        uint index = 0;

                        foreach (var pcs in pointList)
                        {
                            list.Add(new SortSprite(index, Vector3.Subtract(pcs.Position, viewPoint).LengthSquared()));

                            index++;
                        }

                        list.Sort();
                        var indexCount = list.Count;
                        indexes = new uint[indexCount];
                        for (var i = 0; i < indexCount; i++)
                        {
                            indexes[i] = list[i].Index;
                        }

                        if (!reverse)
                        {
                            WriteIndexFile(indexes, indexFilename);
                        }
                    }
                    else
                    {
                        indexes = ReadIndexFile(indexFilename, reverse);
                    }

                    MilkyWayIndexBuffers[angleIndex] = new IndexBuffer11(device, indexes);

                    angleIndex++; 
                } 
                
                var points = pointList.ToArray();

                galaxyPointCount = pointList.Count;

                milkyWaySprites = new PointSpriteSet(device, points);

            }
            starMutex.ReleaseMutex();
        }

        public static uint[] ReadIndexFile(string filename, bool reverse)
        {
            Stream stream = File.OpenRead(filename);
            var gStream = new GZipStream(stream, CompressionMode.Decompress);
            var br = new BinaryReader(gStream);

            var count = br.ReadInt32();

            var list = new uint[count];

            if (reverse)
            {
                for (var i = count-1; i != 0; i--)
                {
                    list[i] = br.ReadUInt32();
                }
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    list[i] = br.ReadUInt32();
                }
            }

            br.Close();

            return list;
        }

        public static void WriteIndexFile(uint[] list, string filename)
        {
            Stream stream = File.OpenWrite(filename);

            var gStream = new GZipStream(stream, CompressionMode.Compress);
            var bw = new BinaryWriter(gStream);

            var count = list.Length;
            bw.Write(count);

            for (var i = 0; i < count; i++ )
            {
                bw.Write(list[i]);
            }
            bw.Close();
        }

        public static List<PositionColorSize> ReadPointListFile(string filename)
        {
            var points = new List<PositionColorSize>();
            Stream stream = File.OpenRead(filename);
            var gStream = new GZipStream(stream,CompressionMode.Decompress);
            var br = new BinaryReader(gStream);

            var count = br.ReadInt32();

            for (var i = 0; i < count; i++)
            {
                points.Add(PositionColorSize.Load(br));
            }

            br.Close();

            return points;
        }

        public static void WritePointListFile(List<PositionColorSize> pointList, string filename)
        {
            Stream stream = File.OpenWrite(filename);

            var gStream = new GZipStream(stream, CompressionMode.Compress);
            var bw = new BinaryWriter(gStream);

            bw.Write(pointList.Count);

            foreach (var point in pointList)
            {
                point.Save(bw);
            }
            bw.Close();
        }

        public static IndexBuffer11[] MilkyWayIndexBuffers = new IndexBuffer11[13];

        public static int MakeSampleSprites(List<PositionColorSize> pointList, string imageFile, int step, float pointSize, Color color, int countFactor, int threashold, double verticalScaling, double spread)
        {
            var spriteCount = 0;

            var ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

            var bmp = new Bitmap(imageFile);

            var rnd = new Random(1231232);
            var scaleFactor = 60800000.0;

            galaxyPointCount = bmp.Width * bmp.Height;
            Vector3d point;

            double centerX = 0;
            var centerY = (28 / 64.0) * bmp.Height / 2.0;
            var fFac = 128.0 / bmp.Height;

            scaleFactor = scaleFactor / (bmp.Height / 128);

            for (var y = 0; y < bmp.Height; y += step)
            {
                for (var x = 0; x < bmp.Width; x += step)
                {
                    var col = bmp.GetPixel(x, y);

                    double luminance = Math.Max(col.R,Math.Max(col.G, col.B));

                    if (luminance > threashold)
                    {
                        col = Color.FromArgb((int)luminance/2, col);

                        var variance = Math.Log(luminance - 19, 2) / 200;
                        variance = 100;

                        var d = Math.Sqrt((x - (bmp.Width / 2)) * (x - (bmp.Width / 2)) + (y - (bmp.Height / 2)) * (y - (bmp.Height / 2)));
                        double height = 1; ;


                        d = d * fFac;


                        if (d < 8)
                        {
                            var sp = Math.Cos(d / (8) * Math.PI / 2) * .6 + .5;
                            height = sp / 24;
                        }
                        else
                        {
                            d = d * 2.5;
                            height = ( 1 / (d * d)) * 8;
                        }

                        var pntScaleDisk = (float)Math.Max(1, height / .02f);

                        //height = height * 1520 * verticalScaling;
                        height = height * 3000 * verticalScaling;

                        var count = (int)Math.Max(1,(luminance / 255 * countFactor));

                        if (rnd.NextDouble() > .05)
                        {
                            count = 0;
                        }

 
                        while (count > 0)
                        {
                            //point = new Vector3d(-(((x + (rnd.NextDouble() - .5) * step * spread) - bmp.Width / 2) - centerX) * scaleFactor, (((rnd.NextDouble() * height - (height / 2)))) * scaleFactor, (((y + (rnd.NextDouble() - .5) * step * spread) - bmp.Height / 2) - centerY) * scaleFactor);
                            point = new Vector3d(-(((x ) - bmp.Width / 2) - centerX) * scaleFactor, (((rnd.NextDouble() * height - (height / 2)))) * scaleFactor, (((y ) - bmp.Height / 2) - centerY) * scaleFactor);
                            var vert = new PositionColorSize();
                            point.RotateY(213.0 / 180 * Math.PI);
                            point.RotateZ((-62.87175) / 180 * Math.PI);
                            point.RotateY((-192.8595083) / 180 * Math.PI);
                            point.RotateX(ecliptic);
                            vert.X = (float)point.X;
                            vert.Y = (float)point.Y;
                            vert.Z = (float)point.Z;
                            if (color == Color.White)
                            {
                                vert.Color = col;
                            }
                            else
                            {
                                vert.Color = color;
                            }
                            vert.size = (float)(scaleFactor * variance * (2 * ((luminance / 96) + .5) ) ) * pointSize * pntScaleDisk;
                            pointList.Add(vert);
                            count--;
                            spriteCount++;
                        }
                    }
                }
            }
            bmp.Dispose();
            return spriteCount;
        }

        public static int MakeGalaxySprites(List<PositionColorSize> pointList, string imageFile, int step, float pointSize, Color color, int countFactor, int threashold, double verticalScaling, double spread)
        {
            var spriteCount = 0;

            var ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

            var bmp = new Bitmap(imageFile);

            var rnd = new Random(1231232);
            var scaleFactor = 60800000.0;

            galaxyPointCount = bmp.Width * bmp.Height;
            Vector3d point;

            double centerX = 0;
            var centerY = (28 / 64.0) * bmp.Height / 2.0;
            var fFac = 128.0 / bmp.Height;

            scaleFactor = scaleFactor / (bmp.Height / 128);

            for (var y = 0; y < bmp.Height; y += step)
            {
                for (var x = 0; x < bmp.Width; x += step)
                {
                    var col = bmp.GetPixel(x, y);

                    var luminance = (double)col.R;
                    if (luminance > threashold)
                    {
                        var variance = Math.Log(luminance - 19, 2) / 200;
                        variance = 100;

                        var d = Math.Sqrt((x - (bmp.Width / 2)) * (x - (bmp.Width / 2)) + (y - (bmp.Height / 2)) * (y - (bmp.Height / 2)));
                        double height = 1; ;

                        d = d * fFac;


                        if (d < 8)
                        {
                            var sp = Math.Cos(d / (8) * Math.PI / 2) * .6 + .5;
                            height = sp / 24;
                        }
                        else
                        {
                            d = d * 2.5;
                            height = (1 / (d * d)) * 8;
                        }

                        //height = height * 1520 * verticalScaling;
                        height = height * 3000 * verticalScaling;

                        var count = (int)Math.Max(1,(luminance / 255 * countFactor));

                        while (count > 0)
                        {
                            point = new Vector3d(-(((x + (rnd.NextDouble() - .5) * step * spread) - bmp.Width / 2) - centerX) * scaleFactor, (((rnd.NextDouble() * height - (height / 2)))) * scaleFactor, (((y + (rnd.NextDouble() - .5) * step * spread) - bmp.Height / 2) - centerY) * scaleFactor);
                            var vert = new PositionColorSize();
                            point.RotateY(213.0 / 180 * Math.PI);
                            point.RotateZ((-62.87175) / 180 * Math.PI);
                            point.RotateY((-192.8595083) / 180 * Math.PI);
                            point.RotateX(ecliptic);
                            vert.X = (float)point.X;
                            vert.Y = (float)point.Y;
                            vert.Z = (float)point.Z;
                            vert.Color = color;
                            vert.size = (float)(scaleFactor * variance * 2 + (rnd.NextDouble() * 1)) * pointSize;
                            pointList.Add(vert);
                            count--;
                            spriteCount++;
                        }
                    }
                }
            }
            bmp.Dispose();
            return spriteCount;
        }

        public static int MakeBarSprites(List<PositionColorSize> pointList, string imageFile, int step, float pointSize, Color color, int countFactor, int threashold, double verticalScaling, double spread)
        {
            var spriteCount = 0;

            var ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

            var bmp = new Bitmap(imageFile);

            var rnd = new Random(1231232);
            var scaleFactor = 60800000.0;

            galaxyPointCount = bmp.Width * bmp.Height;
            Vector3d point;

            double centerX = 0;
            var centerY = (28 / 64.0) * bmp.Height / 2.0;
            var fFac = 128.0 / bmp.Height;

            scaleFactor = scaleFactor / (bmp.Height / 128);

            for (var y = 0; y < bmp.Height; y += step)
            {
                for (var x = 0; x < bmp.Width; x += step)
                {
                    var col = bmp.GetPixel(x, y);

                    var luminance = (double)col.R;
                    if (luminance > threashold)
                    {
                        double variance = 100;
                        var height = luminance; 


                        height = height * verticalScaling;

                        var count = (int)Math.Max(1, (luminance / 255 * countFactor));

                        while (count > 0)
                        {
                            point = new Vector3d(-(((x + (rnd.NextDouble() - .5) * step * spread) - bmp.Width / 2) - centerX) * scaleFactor, (((rnd.NextDouble() * height - (height / 2)))) * scaleFactor, (((y + (rnd.NextDouble() - .5) * step * spread) - bmp.Height / 2) - centerY) * scaleFactor);
                            var vert = new PositionColorSize();
                            point.RotateY(213.0 / 180 * Math.PI);
                            point.RotateZ((-62.87175) / 180 * Math.PI);
                            point.RotateY((-192.8595083) / 180 * Math.PI);
                            point.RotateX(ecliptic);
                            vert.X = (float)point.X;
                            vert.Y = (float)point.Y;
                            vert.Z = (float)point.Z;
                            vert.Color = color;
                            vert.size = (float)(scaleFactor * variance * 2 + (rnd.NextDouble() * 1)) * pointSize;
                            pointList.Add(vert);
                            count--;
                            spriteCount++;
                        }
                    }
                }
            }
            bmp.Dispose();
            return spriteCount;
        }

        static double eclip;

        public static Vector3d GetMWPoint(double x, double y, double z)
        {
            if (eclip == 0)
            {
                eclip = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;
            }

            var scaleFactor = 60800000.0;
            var point = new Vector3d(x * scaleFactor, y * scaleFactor, z * scaleFactor);

            point.RotateY(213.0 / 180 * Math.PI);
            point.RotateZ((-62.87175) / 180 * Math.PI);
            point.RotateY((-192.8595083) / 180 * Math.PI);
            point.RotateX(eclip);
            return point;
        }


        public static void MakeOccludingSprites(Device device)
        {
            starMutex.WaitOne();

            if (occludingSprites == null)
            {
                var ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

                if (galacticCloud == null)
                {
                    galacticCloud = Planets.LoadPlanetTexture(Resources.StarProfileAlpha);
                }


                //if (galacticCloud == null)
                //{
                //    galacticCloud = Texture11.FromFile(@"c:\milkyway\diskdensity.tif");
                //}

                var bmp = new Bitmap(@"c:\milkyway\dustlanes.tif");
             
                var rnd = new Random(1231232);
                var scaleFactor = 60800000.0;

                galaxyPointCount = bmp.Width * bmp.Height;

                Vector3d point;
                var pointList = new List<PositionColorSize>();
                double centerX = 0;
                var centerY = (28 / 64.0) * bmp.Height / 2.0;
                var fFac = 128.0 / bmp.Height;

                scaleFactor = scaleFactor / (bmp.Height / 128.0);

                var step = 2;

                for (var y = 0; y < bmp.Height; y += step)
                {
                    for (var x = 0; x < bmp.Width; x += step)
                    {
                        var col = bmp.GetPixel(x, y);

                
                        // int icol = (new SharpDX.Color(col.R, col.G, col.B, 255)).ToRgba();

                        var luminance = (double)col.R;
                        if (luminance > 5)
                        {
                            var variance = Math.Log(luminance - 19, 2) / 200;
                            variance = 100;

                            var d = Math.Sqrt((x - (bmp.Width / 2)) * (x - (bmp.Width / 2)) + (y - (bmp.Height / 2)) * (y - (bmp.Height / 2)));
                            double height = 1; ;
                            if (d < 15 / fFac)
                            {
                                var sp = Math.Sqrt(1 - ((d / 16 / fFac) * (d / 16 / fFac)));
                                height = sp * 20 * fFac;
                            }
                            else
                            {
                                d = d / 20;
                                height = ((variance) * 1 / (d * d));
                            }

                            height = height * 720;

                            var count = ((int)variance) + 1;
                            count = (int)(luminance / 10);
                            while (count > 0)
                            {
                                point = new Vector3d(-(((x + (rnd.NextDouble() - .5)) - bmp.Width / 2) - centerX) * scaleFactor, (((rnd.NextDouble() * height - (height / 2)))) * scaleFactor, (((y + (rnd.NextDouble() - .5)) - bmp.Height / 2) - centerY) * scaleFactor);
                                var vert = new PositionColorSize();
                                point.RotateY(213.0 / 180 * Math.PI);
                                point.RotateZ((-62.87175) / 180 * Math.PI);
                                point.RotateY((-192.8595083) / 180 * Math.PI);
                                point.RotateX(ecliptic);
                                vert.X = (float)point.X;
                                vert.Y = (float)point.Y;
                                vert.Z = (float)point.Z;
                                vert.Color = Color.Black;
                                vert.size = (float)(scaleFactor * variance * 2 + (rnd.NextDouble() * 1)) /40;
                                pointList.Add(vert);
                                count--;
                            }
                        }
                    }
                }
              
               // galaxyPointCount = pointList.Count;

                // var points = new PositionColorSize[galaxyPointCount];

                var points = pointList.ToArray();

                //index = 0;
                //foreach (PositionColorSize pnt in pointList)
                //{
                //    points[index++] = pnt;
                //}


                occludingSprites = new PointSpriteSet(device, points);
            }

            starMutex.ReleaseMutex();
        }

        public static void MakeGalaxySprites(Device device)
        {
            return;
            starMutex.WaitOne();

            if (milkyWaySprites == null)
            {
                var ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

                if (galacticCloud == null)
                {
                    galacticCloud = Planets.LoadPlanetTexture(Resources.StarProfileAlpha);
                }


                //if (galacticCloud == null)
                //{
                //    galacticCloud = Texture11.FromFile(@"c:\milkyway\diskdensity.tif");
                //}

                var bmp = new Bitmap(@"c:\milkyway\diskdensity.tif");
                var bmpCol = new Bitmap(@"c:\milkyway\galaxy.png");

                var rnd = new Random(1231232);
                var scaleFactor = 60800000.0; 

                galaxyPointCount = bmp.Width * bmp.Height;

                Vector3d point;
                var pointList = new List<PositionColorSize>();
                double centerX = 0;
                var centerY = (28/64.0)* bmp.Height/2.0;
                var fFac = 128.0 / bmp.Height;

                scaleFactor = scaleFactor / (bmp.Height / 128);

                var step = 16;

                for (var y = 0; y < bmp.Height; y += step)
                {
                    for (var x = 0; x < bmp.Width; x += step)
                    {
                        var col = bmp.GetPixel(x, y);

                        var col1 = bmpCol.GetPixel(x, y);

                       // int icol = (new SharpDX.Color(col.R, col.G, col.B, 255)).ToRgba();

                        var luminance = (double)col.R;
                        if (luminance > 5)
                        {
                            var variance = Math.Log(luminance - 19, 2)/200;
                            variance = 100;

                            var d = Math.Sqrt((x - (bmp.Width / 2)) * (x - (bmp.Width / 2)) + (y - (bmp.Height / 2)) * (y - (bmp.Height / 2)));
                            double height = 1; ;
                            if (d < 15/fFac)
                            {
                                var sp = Math.Sqrt(1 - ((d / 16 / fFac) * (d / 16 / fFac)));
                                height = sp * 20;
                            }
                            else
                            {
                                d = d / 20;
                                height = ((variance) * 1 / (d * d));
                            }

                            height = height *1520;

                            var count = ((int)variance) + 1;
                            count = (int)(luminance/20);
                            while (count > 0)
                            {
                                point = new Vector3d(-(((x + (rnd.NextDouble() - .5) * step) - bmp.Width / 2) - centerX) * scaleFactor, (((rnd.NextDouble() * height - (height / 2)))) * scaleFactor, (((y + (rnd.NextDouble() - .5) * step) - bmp.Height / 2) - centerY) * scaleFactor);
                                var vert = new PositionColorSize();
                                point.RotateY(213.0 / 180 * Math.PI);
                                point.RotateZ((-62.87175) / 180 * Math.PI);
                                point.RotateY((-192.8595083) / 180 * Math.PI);
                                point.RotateX(ecliptic);
                                vert.X = (float) point.X;
                                vert.Y = (float) point.Y;
                                vert.Z = (float) point.Z;
                                vert.Color = col1;
                                vert.size = (float)(scaleFactor * variance * 2 + (rnd.NextDouble() * 1)) *4;
                                pointList.Add(vert);
                                count--;
                            }
                        }
                    }
                }

                galaxyPointCount = pointList.Count;

               // var points = new PositionColorSize[galaxyPointCount];

                var points = pointList.ToArray();

                //index = 0;
                //foreach (PositionColorSize pnt in pointList)
                //{
                //    points[index++] = pnt;
                //}


                milkyWaySprites = new PointSpriteSet(device, points);
            }

            starMutex.ReleaseMutex();
        }

        public static void MakeGalaxySpritesOld(Device device)
        {
            starMutex.WaitOne();

            if (milkyWaySprites == null)
            {
                var ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

                if (galacticCloud == null)
                {
                    galacticCloud = Planets.LoadPlanetTexture(Resources.GalacticCloud);
                }

                var bmp = Resources.milkyWay;

                var rnd = new Random(1231232);
                var scaleFactor = 60800000.0;

                galaxyPointCount = bmp.Width * bmp.Height;
                var index = 0;
                Vector3d point;
                var pointList = new List<PositionColorSize>();
                double centerX = 0;
                double centerY = 28;

                for (var y = 0; y < bmp.Height; y++)
                {
                    for (var x = 0; x < bmp.Width; x++)
                    {
                        var col = bmp.GetPixel(x, y);
                        var icol = (new SharpDX.Color(col.R, col.G, col.B, 255)).ToRgba();

                        var luminance = (0.265 * col.R + 0.670 * col.G + 0.065 * col.B);
                        if (luminance > 5)
                        {
                            var variance = Math.Log(luminance - 19, 2);

                            var d = Math.Sqrt((x - (bmp.Width / 2)) * (x - (bmp.Width / 2)) + (y - (bmp.Height / 2)) * (y - (bmp.Height / 2)));
                            double height = 1; ;
                            if (d < 15)
                            {
                                var sp = Math.Sqrt(1 - ((d / 16) * (d / 16)));
                                height = sp * 20;
                            }
                            else
                            {
                                d = d / 20;
                                height = ((variance) * 1 / (d * d));
                            }

                            height = height / 3;

                            var count = ((int)variance) + 1;
                            while (count > 0)
                            {
                                point = new Vector3d(-(((x + (rnd.NextDouble() - .5)) - bmp.Width / 2) - centerX) * scaleFactor, (((rnd.NextDouble() * height - (height / 2)))) * scaleFactor, (((y + (rnd.NextDouble() - .5)) - bmp.Height / 2) - centerY) * scaleFactor);
                                var vert = new PositionColorSize();
                                point.RotateY(213.0 / 180 * Math.PI);
                                point.RotateZ((-62.87175) / 180 * Math.PI);
                                point.RotateY((-192.8595083) / 180 * Math.PI);
                                point.RotateX(ecliptic);
                                vert.X = (float)point.X;
                                vert.Y = (float)point.Y;
                                vert.Z = (float)point.Z;
                                vert.Color = Color.FromArgb(255, col);
                                vert.size = (float)(scaleFactor * variance * 2 + (rnd.NextDouble() * 1));
                                pointList.Add(vert);
                                count--;
                            }
                        }
                    }
                }
                CreateGalaxyImage(device);

                galaxyPointCount = pointList.Count;

                var points = new PositionColorSize[galaxyPointCount];

                index = 0;
                foreach (var pnt in pointList)
                {
                    points[index++] = pnt;
                }

                // Randomize for sorting..
                rnd = new Random();

                for (var ii = 0; ii < pointList.Count; ii++)
                {
                    var temp = points[ii];

                    index = rnd.Next(pointList.Count - 1);
                    points[ii] = points[index];
                    points[index] = temp;
                }

                milkyWaySprites = new PointSpriteSet(device, points);
            }

            starMutex.ReleaseMutex();
        }


        static PositionTexturedVertexBuffer11 galaxyImageVertexBuffer;
        static IndexBuffer11 galaxyImageIndexBuffer;
        static int galaxyImageTriangleCount;
        private static void CreateGalaxyImage(Device device)
        {
            if (milkyWayImage == null)
            {
                milkyWayImage = Planets.LoadPlanetTexture(Resources.milkywaybar);

               // milkyWayImage = Texture11.FromFile(@"c:\milkyway\MilkyWayMap.png");
            }

            if (galaxyImageIndexBuffer != null)
            {
                galaxyImageIndexBuffer.Dispose();
                GC.SuppressFinalize(galaxyImageIndexBuffer);
                galaxyImageIndexBuffer = null;
            }
            var subdivs = 50;

            galaxyImageIndexBuffer = new IndexBuffer11(typeof(short), subdivs * subdivs * 6, device);

            double lat, lng;

            var index = 0;
            double latMin = 64;
            double latMax = -64;
            double lngMin = -64;
            double lngMax = 64;

            //// Create a vertex buffer 
            galaxyImageVertexBuffer = new PositionTexturedVertexBuffer11((subdivs + 1) * (subdivs + 1), device);
            var verts = (PositionTextured[])galaxyImageVertexBuffer.Lock(0, 0);

            int x1, y1;
            var latDegrees = latMax - latMin;
            var lngDegrees = lngMax - lngMin;
            var scaleFactor = 60800000.0;
            var ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;
            Vector3d point;

            double textureStepX = 1.0f / subdivs;
            double textureStepY = 1.0f / subdivs;
            for (y1 = 0; y1 <= subdivs; y1++)
            {

                if (y1 != subdivs)
                {
                    lat = latMax - (textureStepY * latDegrees * y1);
                }
                else
                {
                    lat = latMin;
                }

                for (x1 = 0; x1 <= subdivs; x1++)
                {
                    if (x1 != subdivs)
                    {
                        lng = lngMin + (textureStepX * lngDegrees * x1);
                    }
                    else
                    {
                        lng = lngMax;
                    }
                    index = y1 * (subdivs + 1) + x1;
                    point = new Vector3d(lng * scaleFactor, 0, (lat - 28) * scaleFactor);
                    point.RotateY(213.0 / 180 * Math.PI);
                    point.RotateZ((-62.87175) / 180 * Math.PI);
                    point.RotateY((-192.8595083) / 180 * Math.PI);
                    point.RotateX(ecliptic);
                    verts[index].Position = point;
                    verts[index].Tu = (float)(1f - x1 * textureStepX);
                    verts[index].Tv = (float)(/*1f - */(y1 * textureStepY));
                }
            }
            galaxyImageVertexBuffer.Unlock();
            galaxyImageTriangleCount = (subdivs) * (subdivs) * 2;
            var indexArray = (short[])galaxyImageIndexBuffer.Lock();

            for (y1 = 0; y1 < subdivs; y1++)
            {
                for (x1 = 0; x1 < subdivs; x1++)
                {
                    index = (y1 * subdivs * 6) + 6 * x1;
                    // First triangle in quad
                    indexArray[index] = (short)(y1 * (subdivs + 1) + x1);
                    indexArray[index + 2] = (short)((y1 + 1) * (subdivs + 1) + x1);
                    indexArray[index + 1] = (short)(y1 * (subdivs + 1) + (x1 + 1));

                    // Second triangle in quad
                    indexArray[index + 3] = (short)(y1 * (subdivs + 1) + (x1 + 1));
                    indexArray[index + 5] = (short)((y1 + 1) * (subdivs + 1) + x1);
                    indexArray[index + 4] = (short)((y1 + 1) * (subdivs + 1) + (x1 + 1));
                }
            }
            galaxyImageIndexBuffer.Unlock();
        }

        static PointSpriteSet[] cosmosSprites;
        static Texture11[] galaxyTextures;
        static int[] galaxyVertexCounts;
        static bool largeSet = true;

        public static void DrawCosmos3D(RenderContext11 renderContext, float opacity)
        {
            var device = renderContext.Device;
            var zoom = Earth3d.MainWindow.ZoomFactor;
            var distAlpha = ((Math.Log(Math.Max(1, zoom), 4)) - 15.5) * 90;

            var alpha =  Math.Min(255, Math.Max(0, (int)distAlpha));

            if (alpha < 3)
            {
                return;
            }

            InitCosmosVertexBuffer();
   
            if (galaxyTextures == null)
            {
                if (largeSet)
                {
                    galaxyTextures = new Texture11[256];
                    for (var i = 0; i < 256; i++)
                    {
                        var name = string.Format("TerraViewer.Cosmos.gal_{0:0000}.jpg", i);
                        var stream = Earth3d.MainWindow.GetType().Assembly.GetManifestResourceStream(name);
                        galaxyTextures[i] = Texture11.FromStream(device, stream);
                        stream.Close();
                        stream.Dispose();  
                    }
                }
                else
                {
                    galaxyTextures = new Texture11[20];
                    galaxyTextures[0] = Planets.LoadPlanetTexture(Resources.g000);
                    galaxyTextures[1] = Planets.LoadPlanetTexture(Resources.g001);
                    galaxyTextures[2] = Planets.LoadPlanetTexture(Resources.g002);
                    galaxyTextures[3] = Planets.LoadPlanetTexture(Resources.g003);
                    galaxyTextures[4] = Planets.LoadPlanetTexture(Resources.g004);
                    galaxyTextures[5] = Planets.LoadPlanetTexture(Resources.g005);
                    galaxyTextures[6] = Planets.LoadPlanetTexture(Resources.g006);
                    galaxyTextures[7] = Planets.LoadPlanetTexture(Resources.g007);
                    galaxyTextures[8] = Planets.LoadPlanetTexture(Resources.g008);
                    galaxyTextures[9] = Planets.LoadPlanetTexture(Resources.g009);
                    galaxyTextures[10] = Planets.LoadPlanetTexture(Resources.g010);
                    galaxyTextures[11] = Planets.LoadPlanetTexture(Resources.g011);
                    galaxyTextures[12] = Planets.LoadPlanetTexture(Resources.g012);
                    galaxyTextures[13] = Planets.LoadPlanetTexture(Resources.g013);
                    galaxyTextures[14] = Planets.LoadPlanetTexture(Resources.g014);
                    galaxyTextures[15] = Planets.LoadPlanetTexture(Resources.g015);
                    galaxyTextures[16] = Planets.LoadPlanetTexture(Resources.g016);
                    galaxyTextures[17] = Planets.LoadPlanetTexture(Resources.g017);
                    galaxyTextures[18] = Planets.LoadPlanetTexture(Resources.g018);
                    galaxyTextures[19] = Planets.LoadPlanetTexture(Resources.g019);
                }
            }

            renderContext.setRasterizerState(TriangleCullMode.Off);
            renderContext.BlendMode = BlendMode.Additive;

            var max = (int)(Math.Pow(Properties.Settings.Default.ImageQuality, 2.849485002) / 20);
            var count = largeSet ? 256 : 20;
            for (var i = 0; i < count; i++)
            {
                cosmosSprites[i].MinPointSize = 1;
                cosmosSprites[i].Draw(renderContext, Math.Min(max, galaxyVertexCounts[i]), galaxyTextures[i], (alpha * opacity) / 255.0f);
            }

        }      


        public static void InitCosmosVertexBuffer()
        {
            cosmosMutex.WaitOne();
            try
            {
                if (cosmosSprites == null)
                {
                    InitializeCosmos();
                    CreateCosmosVertexBuffer();
                }
            }
            finally
            {
                cosmosMutex.ReleaseMutex();
            }
        }

        private static void CreateCosmosVertexBuffer()
        {
            var device = RenderContext11.PrepDevice;

            var buckerCount = largeSet ? 256 : 20;

            if (cosmosSprites != null)
            {
                for (var ij = 0; ij < buckerCount; ij++)
                {
                    if (cosmosSprites[ij] != null)
                    {
                        cosmosSprites[ij].Dispose();
                        cosmosSprites[ij] = null;
                    }
                }
            }
            cosmosSprites = null;
            var ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;
            var points = new PositionColorSize[buckerCount][];
            cosmosSprites = new PointSpriteSet[buckerCount];
            var indexList = new int[buckerCount];
            for (var i = 0; i < buckerCount; i++)
            {
                var count = galaxyVertexCounts[i];
                indexList[i] = 0;
                points[i] = new PositionColorSize[count];
            }

            foreach (var galaxy in cosmos)
            {
                var bucket = galaxy.eTypeBucket;
                var index = indexList[bucket];

                var pos = Coordinates.RADecTo3d(galaxy.RA, galaxy.Dec, (galaxy.Distance * UiTools.AuPerParsec * 1000000.0) / .73);
                pos.RotateX(ecliptic);
                galaxy.Position = pos;
                points[bucket][index].X = (float)pos.X;
                points[bucket][index].Y = (float)pos.Y;
                points[bucket][index].Z = (float)pos.Z;
                points[bucket][index].Color = Color.White;
                points[bucket][index].size = 1000000000f * galaxy.Size;
                indexList[bucket]++;
            }
            for (var i = 0; i < buckerCount; i++)
            {
                cosmosSprites[i] = new PointSpriteSet(device, points[i]);
            }
        }


        public static bool CheckCosmosFile()
        {
            var filename = Properties.Settings.Default.CahceDirectory + @"cosmos\cosmos.bin";
            return File.Exists(filename);
        }
        static List<Galaxy> cosmos;
        public static void InitializeCosmos()
        {
            var max = (int)Math.Pow(Properties.Settings.Default.ImageQuality, 2.849485002);

            if (cosmos == null)
            {
                galaxyVertexCounts = new int[largeSet? 256 : 20];
                if (false)
                {
                    cosmos = new List<Galaxy>();
                     var filename = string.Format("{0}\\Cosmos\\cosmos.txt",Properties.Settings.Default.CahceDirectory);
                     var sr = new StreamReader(filename);
                    string line;
                    Galaxy galaxy;
                    while (sr.Peek() >= 0)
                    {
                        line = sr.ReadLine();
                        try
                        {

                            galaxy = new Galaxy(line, false);
                            galaxyVertexCounts[galaxy.eTypeBucket]++;
                            cosmos.Add(galaxy);
                        }
                        catch
                        {
                        }
                    }
                    sr.Close();
                    var index = 0;
                    // Randomize for sorting..
                    var rnd = new Random();

                    for (var ii = 0; ii < cosmos.Count; ii++)
                    {
                        var temp = cosmos[ii];

                        index = rnd.Next(cosmos.Count - 1);
                        cosmos[ii] = cosmos[index];
                        cosmos[index] = temp;
                    }

                    //// Write Binary file
                    DumpGalaxyBinaryFile(Properties.Settings.Default.CahceDirectory + @"cosmos\cosmos.bin");
                }
                if (cosmos == null)
                {
                    cosmos = new List<Galaxy>();
                    //string filename = Properties.Settings.Default.CahceDirectory + @"cosmos\cosmos.bin";
                    //DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=cosmosbin", filename, false, true);
                    //FileStream fs = new FileStream(filename, FileMode.Open);

                    // From Resrouce
                    var name = "TerraViewer.Cosmos.cosmos.bin";
                    var fs = Earth3d.MainWindow.GetType().Assembly.GetManifestResourceStream(name);

                    var len = fs.Length;
                    var br = new BinaryReader(fs);
                    Galaxy galaxy;
                    try
                    {
                        var count = 0; 
                        while (fs.Position < len & count < max)
                        {
                            galaxy = new Galaxy(br);
                            cosmos.Add(galaxy);
                            galaxyVertexCounts[galaxy.eTypeBucket]++;
                            count++;
                        }
                    }
                    catch
                    {
                    }
                    br.Close();
                    fs.Close();
                }
            }
        }


        static PointSpriteSet[] customCosmosSprites;
        static Texture11[] customGalaxyTextures = null;
        static int[] customGalaxyVertexCounts;
        

        public static void DrawCustomCosmos3D(RenderContext11 renderContext, float opacity)
        {
            if (customCosmosSprites == null)
            {
                return;
            }

            var device = renderContext.Device;
            var zoom = Earth3d.MainWindow.ZoomFactor;
            var distAlpha = ((Math.Log(Math.Max(1, zoom), 4)) - 15.5) * 90;

            var alpha = Math.Min(255, Math.Max(0, (int)distAlpha));

            if (alpha < 3)
            {
                return;
            }

            //InitCosmosVertexBuffer();

            if (galaxyTextures == null)
            {
                if (largeSet)
                {
                    galaxyTextures = new Texture11[256];
                    for (var i = 0; i < 256; i++)
                    {
                        var name = string.Format("TerraViewer.Cosmos.gal_{0:0000}.jpg", i);
                        var stream = Earth3d.MainWindow.GetType().Assembly.GetManifestResourceStream(name);
                        galaxyTextures[i] = Texture11.FromStream(device, stream);
                        stream.Close();
                        stream.Dispose();
                    }
                }
                else
                {
                    galaxyTextures = new Texture11[20];
                    galaxyTextures[0] = Planets.LoadPlanetTexture(Resources.g000);
                    galaxyTextures[1] = Planets.LoadPlanetTexture(Resources.g001);
                    galaxyTextures[2] = Planets.LoadPlanetTexture(Resources.g002);
                    galaxyTextures[3] = Planets.LoadPlanetTexture(Resources.g003);
                    galaxyTextures[4] = Planets.LoadPlanetTexture(Resources.g004);
                    galaxyTextures[5] = Planets.LoadPlanetTexture(Resources.g005);
                    galaxyTextures[6] = Planets.LoadPlanetTexture(Resources.g006);
                    galaxyTextures[7] = Planets.LoadPlanetTexture(Resources.g007);
                    galaxyTextures[8] = Planets.LoadPlanetTexture(Resources.g008);
                    galaxyTextures[9] = Planets.LoadPlanetTexture(Resources.g009);
                    galaxyTextures[10] = Planets.LoadPlanetTexture(Resources.g010);
                    galaxyTextures[11] = Planets.LoadPlanetTexture(Resources.g011);
                    galaxyTextures[12] = Planets.LoadPlanetTexture(Resources.g012);
                    galaxyTextures[13] = Planets.LoadPlanetTexture(Resources.g013);
                    galaxyTextures[14] = Planets.LoadPlanetTexture(Resources.g014);
                    galaxyTextures[15] = Planets.LoadPlanetTexture(Resources.g015);
                    galaxyTextures[16] = Planets.LoadPlanetTexture(Resources.g016);
                    galaxyTextures[17] = Planets.LoadPlanetTexture(Resources.g017);
                    galaxyTextures[18] = Planets.LoadPlanetTexture(Resources.g018);
                    galaxyTextures[19] = Planets.LoadPlanetTexture(Resources.g019);
                }
            }

            renderContext.setRasterizerState(TriangleCullMode.Off);
            renderContext.BlendMode = BlendMode.Additive;

            var count = customLargeSet ? 256 : 20;
            for (var i = 0; i < count; i++)
            {
                customCosmosSprites[i].MinPointSize = 2;
                customCosmosSprites[i].Draw(renderContext, customGalaxyVertexCounts[i], galaxyTextures[i], (alpha * opacity) / 255.0f);
            }

        }    


        /// <summary>
        /// Custom Cosmos
        /// </summary>
        /// <param name="table"></param>
        /// <param name="raCol"></param>
        /// <param name="decCol"></param>
        /// <param name="distanceCol"></param>
        /// <param name="eClassCol"></param>

        static List<Galaxy> customCosmos;
        public static void InitializeCustomCosmos(VoTable table, int raCol, int decCol, int distanceCol, int eClassCol)
        {

            customGalaxyVertexCounts = new int[customLargeSet ? 256 : 20];
            customCosmos = new List<Galaxy>();
            Galaxy galaxy;
            foreach(var row in table.Rows)
            {
                var line = string.Format("{0}\t{1}\t{2}\t{3}",row[raCol],row[decCol],row[distanceCol],row[eClassCol]);
                galaxy = new Galaxy(line, false);
                customGalaxyVertexCounts[galaxy.eTypeBucket]++;
                customCosmos.Add(galaxy);
            }
            // Randomize for sorting..
            var rnd = new Random();
            //int index = 0;
            //for (int ii = 0; ii < cosmos.Count; ii++)
            //{
            //    Galaxy temp = cosmos[ii];

            //    index = rnd.Next(cosmos.Count - 1);
            //    customCosmos[ii] = cosmos[index];
            //    customCosmos[index] = temp;
            //}

            CreateCustomCosmosVertexBuffer();
        }

        static float min = float.MaxValue;
        static float max = float.MinValue;
        public static void InitializeCustomCosmos(string filename)
        {
            Stream fs = File.OpenRead(filename);
            //Stream fs = File.OpenRead(@"c:\andyted\galaxies.csv");
            var sr = new StreamReader(fs);
            
            //burn header
            sr.ReadLine();

            var buckets = customLargeSet ? 256 : 20;
            customGalaxyVertexCounts = new int[buckets];
            customCosmos = new List<Galaxy>();
            Galaxy galaxy;

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                galaxy = new Galaxy(line);
                if (galaxy.Type < min)
                {
                    min = galaxy.Type;
                }

                if (galaxy.Type > max)
                {
                    max = galaxy.Type;
                }

                customCosmos.Add(galaxy);
            }

            var scale = buckets / (max - min);

            foreach (var gal in customCosmos)
            {
                gal.eTypeBucket = (int)Math.Max(0, Math.Min(buckets-1, (gal.Type - min) * scale));

                customGalaxyVertexCounts[gal.eTypeBucket]++;
            }

            fs.Close();

            CreateCustomCosmosVertexBuffer();
        }

        static bool customLargeSet = true;
        private static void CreateCustomCosmosVertexBuffer()
        {
            var device = RenderContext11.PrepDevice;

            var bucketCount = customLargeSet ? 256 : 20;

            if (customCosmosSprites != null)
            {
                for (var ij = 0; ij < bucketCount; ij++)
                {
                    if (customCosmosSprites[ij] != null)
                    {
                        customCosmosSprites[ij].Dispose();
                        customCosmosSprites[ij] = null;
                    }
                }
            }
            customCosmosSprites = null;
            var ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;
            var points = new PositionColorSize[bucketCount][];
            customCosmosSprites = new PointSpriteSet[bucketCount];
            var indexList = new int[bucketCount];
            for (var i = 0; i < bucketCount; i++)
            {
                var count = customGalaxyVertexCounts[i];
                indexList[i] = 0;
                points[i] = new PositionColorSize[count];
            }

            foreach (var galaxy in customCosmos)
            {
                var bucket = galaxy.eTypeBucket;
                var index = indexList[bucket];

                var pos = Coordinates.RADecTo3d(galaxy.RA, galaxy.Dec, (galaxy.Distance * UiTools.AuPerParsec * 1000000.0) / .73);
                pos.RotateX(ecliptic);
                galaxy.Position = pos;
                points[bucket][index].X = (float)pos.X;
                points[bucket][index].Y = (float)pos.Y;
                points[bucket][index].Z = (float)pos.Z;
                points[bucket][index].Color = Color.White;
                points[bucket][index].size = 1000000000f * galaxy.Size;
                indexList[bucket]++;
            }
            for (var i = 0; i < bucketCount; i++)
            {
                customCosmosSprites[i] = new PointSpriteSet(device, points[i]);
            }
        }

        internal static bool DownloadCosmosFile()
        {
            var filename = Properties.Settings.Default.CahceDirectory + @"data\cosmos.bin";

            if (!FileDownload.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=cosmosbin", filename, false))
            {
                return true;
            }
            return false;
        }
 

   



     

        static TriangleList structure;

        static VertexBuffer11 ringsVertexBuffer = null;

        static readonly double[] zones = { 1, 0.994506357, 0.546382044, 0.191649663, 0 };
        static readonly Color[] zoneColor = { Color.Green, Color.Yellow, Color.Orange, Color.Red };

        internal static void DrawEarthStructure(RenderContext11 renderContext, float opacity)
        {

            if (structure == null)
            {
                var subDivisionsRings = 600;

                structure = new TriangleList();


                var radStep = Math.PI * 2.0 / subDivisionsRings;

                for (var i = 0; i < 4; i++)
                {

                    var inner = zones[i + 1];
                    var outer = zones[i];
                    for (var x = 0; x < subDivisionsRings; x += 1)
                    {


                        if (x > (subDivisionsRings / 2)-1)
                        {
                            var col = zoneColor[i];
                            col = Color.FromArgb(col.A, (int)(col.R * .7), (int)(col.G * .7), (int)(col.B * .7f));

                            var rads1 = x * radStep + Math.PI / 2;
                            var rads2 = (x + 1) * radStep + Math.PI / 2;
                            var v1 = new Vector3d((Math.Cos(rads1) * inner), (Math.Sin(rads1) * inner), 0);
                            var v2 = new Vector3d((Math.Cos(rads1) * outer), (Math.Sin(rads1) * outer), 0);
                            var v3 = new Vector3d((Math.Cos(rads2) * inner), (Math.Sin(rads2) * inner), 0);
                            var v4 = new Vector3d((Math.Cos(rads2) * outer), (Math.Sin(rads2) * outer), 0);
                            structure.AddQuad(v1, v2, v3, v4, col, new Dates());
                        }
                        else
                        {
                            var rads1 = x * radStep;
                            var rads2 = (x + 1) * radStep;
                            var v1 = new Vector3d(0, (Math.Cos(rads1) * inner), (Math.Sin(rads1) * inner));
                            var v2 = new Vector3d(0, (Math.Cos(rads1) * outer), (Math.Sin(rads1) * outer));
                            var v3 = new Vector3d(0, (Math.Cos(rads2) * inner), (Math.Sin(rads2) * inner));
                            var v4 = new Vector3d(0, (Math.Cos(rads2) * outer), (Math.Sin(rads2) * outer));
                            structure.AddQuad(v1, v2, v3, v4, zoneColor[i], new Dates());

                        }
                    }
                }
            }
            structure.DepthBuffered = true;
            structure.WriteZbuffer = true;
            structure.Draw(renderContext, opacity, TriangleList.CullMode.None);
        }
    }

    public class Galaxy
    {
        public float RA;
        public float Dec;
        public float Distance;
        public float Type;
        public int eTypeBucket;
        public float Size = 5;
        public long SdssID = 0;
        public Galaxy(string line, bool largeSet)
        {
            line = line.Replace("  ", " ");
            var values = line.Split(new[] { '\t', ' ', ',' });
            SdssID = Convert.ToInt64(values[0]);
            RA = Convert.ToSingle(values[1])/15;
            Dec = Convert.ToSingle(values[2]);
            Distance = Convert.ToSingle(values[3]);
            if(largeSet)
            {
                Type = Convert.ToSingle(values[4]);
                eTypeBucket = GetEType(Type);
            }
            else
            {
                Size = Convert.ToSingle(values[4])*30;
                Type = Convert.ToSingle(values[6]);
                eTypeBucket = Convert.ToInt32(values[7]);
            }

        }

        public Galaxy(string line)
        {
            line = line.Replace("  ", " ");
            var values = line.Split(new[] { '\t', ' ', ',' });
            SdssID = Convert.ToInt64(values[0]);
            RA = Convert.ToSingle(values[1]) / 15;
            Dec = Convert.ToSingle(values[2]);
            Distance = Convert.ToSingle(values[3]);
            Size = 500;
            Type = Convert.ToSingle(values[4]);

        }

        public IPlace Place
        {
            get
            {
                var place = new TourPlace("SDSS "+SdssID, Dec, RA, Classification.Galaxy, Earth3d.MainWindow.ConstellationCheck.FindConstellationForPoint(RA, Dec), ImageSetType.SolarSystem, -1);
                place.Magnitude = 0;
                place.Distance = (Distance * UiTools.AuPerParsec * 1000000.0)/.73;
                return place;
            }
        }
        public void WriteBin(BinaryWriter bw)
        {
            bw.Write(SdssID);
            bw.Write(RA);
            bw.Write(Dec);
            bw.Write(Distance);
            bw.Write((byte)eTypeBucket);
            bw.Write(Size);
        }

        public Galaxy(BinaryReader br)
        {
            SdssID = br.ReadInt64();
            RA = br.ReadSingle();
            Dec = br.ReadSingle();
            Distance = br.ReadSingle();
            //Type = br.ReadSingle();
            eTypeBucket = br.ReadByte();
            Size = br.ReadSingle();
        }

        public Vector3d Position;

        static readonly float[] eTypeBuckets = new float[] { -3, -0.1860f, -0.1680f, -0.1580f, -0.1500f, -0.1430f, -0.1370f, -0.1300f, -0.1230f, -0.1150f, -0.1040f, -0.0890f, -0.0680f, -0.0420f, -0.0110f, 0.0240f, 0.0640f, 0.1110f, 0.1690f, 0.2520f, 3 };
        public static int GetEType(float value)
        {
            var a = 0;
            var b = eTypeBuckets.Length - 1;
 
            while (b - a > 1)
            {
                var m = (a + b) / 2;
                if (value > eTypeBuckets[m])
                {
                    a = m;
                }
                else
                {
                    b = m;
                }
            }
            return a;
        }
    }

    public struct SortSprite : IEquatable<SortSprite>, IComparable<SortSprite>

    {
        public SortSprite(uint index, float distance)
        {
            Index = index;
            Distance = distance;
        }
        public uint Index;
        public float Distance;

        public bool Equals(SortSprite other)
        {
            return other.Distance == Distance;
        }

        public int CompareTo(SortSprite other)
        {
            return -Distance.CompareTo( other.Distance);
        }
    }

    public class Star
    {
        public double Magnitude;
        public double RA;
        public double Dec;
        public double BMV;
        public string Name
        {
            get
            {
                return "HIP" + ID;
            }
        }
        public Coordinates Coordinates
        {
            get
            {
                return Coordinates.FromRaDec(RA, Dec);
            }
        }

        public int ID;
        public double AbsoluteMagnitude;
        public double Par;
        public double Distance;
        public Color Col;
        public Vector3d Position;

        public IPlace Place
        {
            get
            {
                var place = new TourPlace(Name, Dec, RA, Classification.Star, Earth3d.MainWindow.ConstellationCheck.FindConstellationForPoint(RA, Dec),ImageSetType.SolarSystem,-1);
                place.Magnitude = Magnitude;
                place.Distance = Distance;
                return place;
            }
        }

        public void WriteBin(BinaryWriter bw)
        {
            bw.Write(ID);
            bw.Write((float)RA);
            bw.Write((float)Dec);
            bw.Write((Int16)(Magnitude*1000));
            bw.Write((Int16)(BMV * 1000));
            bw.Write((UInt16)(Par*100));
        }

        public void WriteText(StreamWriter sw)
        {
            sw.Write(ID + "\t");
            sw.Write(RA + "\t");
            sw.Write(Dec + "\t");
            sw.Write(Magnitude + "\t");
            sw.WriteLine(String.Format("#{0:X}{1:X}{2:X}{3:X}", Col.A, Col.R, Col.G, Col.B) + "\t");

        }

        public static void WriteHeader(StreamWriter sw)
        {
            sw.Write("ID" + "\t");
            sw.Write("RA" + "\t");
            sw.Write("Dec" + "\t");
            sw.Write("Mag" + "\t");
            sw.WriteLine("Color" + "\t");

        }
        public Star(BinaryReader br)
        {
            ID =  br.ReadInt32();
            RA = br.ReadSingle();
            Dec = br.ReadSingle();
            Magnitude = ((double)br.ReadInt16())/1000;
            BMV = ((double)br.ReadInt16()) / 1000;
            Par = ((double)br.ReadUInt16()) / 100;
            MakeColor(BMV);
            MakeDistanceAndMagnitude();
        }

        public Star(string input)
        {

            var sa = input.Split('\t');


            ID = Int32.Parse(sa[0].Replace("HIP",""));

            Dec = Convert.ToDouble(sa[3]);

            RA = Convert.ToDouble(sa[2])/15;


            if (sa.Length > 4)
            {
                try
                {
                    if (sa[4].ToUpper() != "NULL" && sa[4] != "")
                    {
                        Magnitude = Convert.ToDouble(sa[4]);
                    }
                }
                catch
                {
                }
            }
            if (sa.Length > 5)
            {
                try
                {
                    BMV = Convert.ToDouble(sa[5]);
                    MakeColor(BMV);

                }
                catch
                {
                }
            }
            if (sa.Length > 6)
            {
                Par = Convert.ToDouble(sa[6]);
                MakeDistanceAndMagnitude();
            }
		}

        private void MakeDistanceAndMagnitude()
        {
            Distance = 1 / (Par / 1000);
            AbsoluteMagnitude = Magnitude - 5 * ((Math.Log10(Distance) - 1));
            //Convert to AU
            Distance *= 206264.806;
        }

        private void MakeColor(double bmv)
        {
            var c = 0xFFFFFFFF;
            if (bmv <= -0.32) { c = 0xFFA2B8FF; }
            else if (bmv <= -0.31) { c = 0xFFA3B8FF; }
            else if (bmv <= -0.3) { c = 0xFFA4BAFF; }
            else if (bmv <= -0.3) { c = 0xFFA5BAFF; }
            else if (bmv <= -0.28) { c = 0xFFA7BCFF; }
            else if (bmv <= -0.26) { c = 0xFFA9BDFF; }
            else if (bmv <= -0.24) { c = 0xFFABBFFF; }
            else if (bmv <= -0.2) { c = 0xFFAFC2FF; }
            else if (bmv <= -0.16) { c = 0xFFB4C6FF; }
            else if (bmv <= -0.14) { c = 0xFFB6C8FF; }
            else if (bmv <= -0.12) { c = 0xFFB9CAFF; }
            else if (bmv <= -0.09) { c = 0xFFBCCDFF; }
            else if (bmv <= -0.06) { c = 0xFFC1D0FF; }
            else if (bmv <= 0) { c = 0xFFCAD6FF; }
            else if (bmv <= 0.06) { c = 0xFFD2DCFF; }
            else if (bmv <= 0.14) { c = 0xFFDDE4FF; }
            else if (bmv <= 0.19) { c = 0xFFE3E8FF; }
            else if (bmv <= 0.31) { c = 0xFFF2F2FF; }
            else if (bmv <= 0.36) { c = 0xFFF9F6FF; }
            else if (bmv <= 0.43) { c = 0xFFFFF9FC; }
            else if (bmv <= 0.54) { c = 0xFFFFF6F3; }
            else if (bmv <= 0.59) { c = 0xFFFFF3EB; }
            else if (bmv <= 0.63) { c = 0xFFFFF1E7; }
            else if (bmv <= 0.66) { c = 0xFFFFEFE1; }
            else if (bmv <= 0.74) { c = 0xFFFFEEDD; }
            else if (bmv <= 0.82) { c = 0xFFFFEAD5; }
            else if (bmv <= 0.92) { c = 0xFFFFE4C4; }
            else if (bmv <= 1.15) { c = 0xFFFFDFB8; }
            else if (bmv <= 1.3) { c = 0xFFFFDDB4; }
            else if (bmv <= 1.41) { c = 0xFFFFD39D; }
            else if (bmv <= 1.48) { c = 0xFFFFCD91; }
            else if (bmv <= 1.52) { c = 0xFFFFC987; }
            else if (bmv <= 1.55) { c = 0xFFFFC57F; }
            else if (bmv <= 1.56) { c = 0xFFFFC177; }
            else if (bmv <= 1.61) { c = 0xFFFFBD71; }
            else if (bmv <= 1.72) { c = 0xFFFFB866; }
            else if (bmv <= 1.84) { c = 0xFFFFB25B; }
            else if (bmv <= 2) { c = 0xFFFFAD51; }
            Col = Color.FromArgb((int)c);
        }
    }



    //public struct StarVertex
    //{
    //    public SharpDX.Vector3 Position;
    //    public float PointSize;
    //    public int Color;
    //    public StarVertex(SharpDX.Vector3 position, float size, int color)
    //    {
    //        Position = position;
    //        PointSize = size;
    //        Color = color;
    //    }
    //}
}
