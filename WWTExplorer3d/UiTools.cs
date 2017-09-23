using Microsoft.Win32;
using System;
using System.Collections.Generic;

#if WINDOWS_UWP

#else
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Globalization;
using WwtDataUtils;
#endif
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;



namespace TerraViewer
{
#if !WINDOWS_UWP
    class MyWebClient : WebClient
    {
        public MyWebClient()
        {

        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = 120 * 1000;
            return w;
        }
    }
#endif
    class UiTools
    {
#if !WINDOWS_UWP
        static public System.Drawing.Font StandardSmall = new System.Drawing.Font("Segoe UI", 7, FontStyle.Regular);
        static public System.Drawing.Font StandardRegular = new System.Drawing.Font("Segoe UI", 8, FontStyle.Regular);
        static public System.Drawing.Font StandardLarge = new System.Drawing.Font("Segoe UI", 12, FontStyle.Regular);
        static public System.Drawing.Font StandardHuge = new System.Drawing.Font("Segoe UI", 15, FontStyle.Regular);
        static public System.Drawing.Font StandardGargantuan = new System.Drawing.Font("Segoe UI", 15, FontStyle.Regular);
        static public System.Drawing.Font TreeViewRegular = new System.Drawing.Font("Segoe UI", 8.25f, FontStyle.Regular);
        static public System.Drawing.Font TreeViewBold = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Bold);
        static public Brush StadardTextBrush = new SolidBrush(Color.White);
        static public Brush DisabledTextBrush = new SolidBrush(Color.Gray);
        static public Brush YellowTextBrush = new SolidBrush(Color.Yellow);
        static public StringFormat StringFormatBottomCenter = new StringFormat();
        static public StringFormat StringFormatBottomLeft = new StringFormat();
        static public StringFormat StringFormatTopLeft = new StringFormat();
        static public StringFormat StringFormatCenterCenter = new StringFormat();
        static public StringFormat StringFormatCenterLeft = new StringFormat();
        static public StringFormat StringFormatThumbnails = new StringFormat();
        static public Color TextBackground = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(105)))));
#endif

        static UiTools()
        {
#if !WINDOWS_UWP
            StringFormatBottomCenter.Alignment = StringAlignment.Center;
            StringFormatBottomCenter.LineAlignment = StringAlignment.Far;
            StringFormatBottomLeft.Alignment = StringAlignment.Near;
            StringFormatBottomLeft.LineAlignment = StringAlignment.Far;
            StringFormatBottomLeft.FormatFlags = StringFormatFlags.NoWrap;
            StringFormatBottomLeft.Trimming = StringTrimming.EllipsisCharacter;

            StringFormatBottomLeft.Alignment = StringAlignment.Near;
            StringFormatBottomLeft.LineAlignment = StringAlignment.Near;
            StringFormatBottomLeft.FormatFlags = StringFormatFlags.NoWrap;
            StringFormatBottomLeft.Trimming = StringTrimming.EllipsisCharacter;

            StringFormatCenterCenter.Alignment = StringAlignment.Center;
            StringFormatCenterCenter.LineAlignment = StringAlignment.Center;
            StringFormatCenterLeft.Alignment = StringAlignment.Near;
            StringFormatCenterLeft.LineAlignment = StringAlignment.Center;

            StringFormatThumbnails.Alignment = StringAlignment.Near;
            StringFormatThumbnails.LineAlignment = StringAlignment.Far;
            StringFormatThumbnails.Trimming = StringTrimming.EllipsisCharacter;
            StringFormatThumbnails.FormatFlags = StringFormatFlags.NoWrap;
            FormatProvider = new NumberFormatInfo();
            FormatProvider.NumberDecimalSeparator = ".";
            FormatProvider.NumberGroupSeparator = ",";
            FormatProvider.NumberGroupSizes = new int[] { 3 };

            List<Color> colorList = new List<Color>();
            foreach (KnownColor name in Enum.GetValues(typeof(KnownColor)))
            {
                Color c = Color.FromKnownColor(name);
                colorList.Add(c);
            }
            KnownColors = colorList.ToArray();
#endif
        }

        //        public static Color FromArgb(int color)
        //        {

        //#if !WINDOWS_UWP
        //            return Color.FromArgb(color);
        //#else
        //            return Windows.UI.Color.FromArgb((byte)(color >> 24), (byte)(color >> 16), (byte)(color >> 8), (byte)(color));
        //#endif
        //        }

        //        public static Color FromArgb(int alpha, Color color)
        //        {
        //#if !WINDOWS_UWP
        //            return Color.FromArgb(alpha,color);
        //#else
        //            return Color.FromArgb((byte)alpha, color.R, color.G, color.B);
        //#endif
        //        }




        public static int Gamma(int val, float gamma)
        {
            return (byte)Math.Min(255, (int)((255.0 * Math.Pow(val / 255.0, 1.0 / gamma)) + 0.5));
        }


        public static string GetMonthName(int month, bool shortName)
        {
            DateTime date = new DateTime(2000, month + 1, 1);

            if (shortName)
            {
                return date.ToString("MMM");
            }
            else
            {
                return date.ToString("MMMM");
            }
        }

        public static string GetDayName(int day, bool shortName)
        {
            DateTime date = new DateTime(2012, 4, (day % 7) + 1);

            if (shortName)
            {
                return date.ToString("ddd");
            }
            else
            {
                return date.ToString("dddd");
            }
        }

        public static string GetHourName(int hour)
        {
            DateTime date = new DateTime(2000, 1, 1, hour, 0, 0);
            return date.ToString("hh:mm tt");

        }

        public static string[] SplitString(string data, char delimiter)
        {
            if (delimiter == '\t')
            {
                return data.Split(new char[] { '\t' });
            }

            List<string> output = new List<string>();

            int nestingLevel = 0;

            int current = 0;
            int count = 0;
            int start = 0;
            bool inQuotes = false;
            while (current < data.Length)
            {
                if (data[current] == '(')
                {
                    nestingLevel++;
                }

                if (data[current] == ')')
                {
                    nestingLevel--;
                }

                if (data[current] == '\"')
                {
                    inQuotes = !inQuotes;
                }

                if (current == (data.Length - 1))
                {
                    count++;
                }

                if (current == (data.Length - 1) || (data[current] == delimiter && delimiter == '\t') || (!inQuotes && nestingLevel == 0 && data[current] == delimiter))
                {
                    output.Add(data.Substring(start, count).Replace("\"", ""));
                    start = current + 1;
                    count = 0;
                }
                else
                {
                    count++;
                }
                current++;
            }

            return output.ToArray();
        }
#if !WINDOWS_UWP
        public static double ParseAndValidateDouble(TextBox input, double defValue, ref bool failed)
        {
            bool sucsess = false;
            double result = defValue;
            sucsess = double.TryParse(input.Text, out result);

            if (sucsess)
            {
                input.BackColor = UiTools.TextBackground;
            }
            else
            {
                input.BackColor = Color.Red;
                failed = true;
            }

            return result;
        }

        public static double ParseAndValidateCoordinate(TextBox input, double defValue, ref bool failed)
        {
            bool sucsess = false;
            double result = defValue;
            sucsess = Coordinates.Validate(input.Text);



            if (sucsess)
            {
                result = Coordinates.Parse(input.Text);
                input.BackColor = UiTools.TextBackground;
            }
            else
            {
                input.BackColor = Color.Red;
                failed = true;
            }

            return result;
        }
#endif
        static public Stream GetMemoryStreamFromUrl(string url)
        {
#if !WINDOWS_UWP
            WebClient client = new WebClient();
            Byte[] data = client.DownloadData(url);
            MemoryStream stream = new MemoryStream(data);

            return stream;
#else
            return null;
            //impliment Syncrhonouse web Get;
#endif
        }

        static public Color[] KnownColors;

        static public bool ValidateString(string instr, string regexstr)
        {
            instr = instr.Trim();
            System.Text.RegularExpressions.Regex pattern = new System.Text.RegularExpressions.Regex(regexstr);
            return pattern.IsMatch(instr);
        }

        static public int GetTransparentColor(int color, float opacity)
        {
            Color inColor = Color.FromArgb(color);
            Color outColor = Color.FromArgb((byte)(opacity * 255f), inColor);
            return outColor.ToArgb();
        }

#if !WINDOWS_UWP
        static public NumberFormatInfo FormatProvider = null;



        static public Bitmap LoadBitmap(string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    Bitmap bmpTemp = new Bitmap(filename);

                    Bitmap bmpReturn = new Bitmap(bmpTemp.Width, bmpTemp.Height);
                    Graphics g = Graphics.FromImage(bmpReturn);
                    g.DrawImage(bmpTemp, new Rectangle(0, 0, bmpTemp.Width, bmpTemp.Height), new Rectangle(0, 0, bmpTemp.Width, bmpTemp.Height), GraphicsUnit.Pixel);
                    g.Dispose();
                    GC.SuppressFinalize(g);
                    bmpTemp.Dispose();
                    GC.SuppressFinalize(bmpTemp);
                    return bmpReturn;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }
#endif
        static Dictionary<string, Bitmap> ThunbnailMap = new Dictionary<string, Bitmap>();
        static public Bitmap LoadThumbnailByName(string name)
        {
            string thumbUrl = "http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=" + name.Replace(" ", "");

            var t=  LoadThumbnailFromWeb(thumbUrl);

            if (t == null)
            {
                System.Diagnostics.Debug.WriteLine(name);
            }

            return t;
        }

        static public Bitmap LoadThumbnailFromWeb(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return null;
            }

            if (ThunbnailMap.ContainsKey(url))
            {
                return ThunbnailMap[url];
            }
            

            if (url.StartsWith(@"http://www.worldwidetelescope.org/hst/") || url.StartsWith(@"http://www.worldwidetelescope.org/thumbnails/") || url.StartsWith(@"http://www.worldwidetelescope.org/spitzer/"))
            {
                string name = url.Substring(url.LastIndexOf("/") + 1).Replace(".jpg", "").Replace(".png", "");

                Bitmap bmp = ThumbnailCache.LoadThumbnail(name);
                if (bmp != null)
                {
                    return ThunbnailMap[url] = bmp;
                }

            }

            if (url.StartsWith(@"http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=") || url.StartsWith(@"http://www.worldwidetelescope.org/spitzer/"))
            {
                string name = url.Replace("http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=", "");

                Bitmap bmp = ThumbnailCache.LoadThumbnail(name);
                if (bmp != null)
                {
                    return ThunbnailMap[url] = bmp;
                }

            }

            int id = Math.Abs(url.GetHashCode32());

            string filename = id.ToString() + ".jpg";

            return ThunbnailMap[url] = LoadThumbnailFromWeb(url, filename);
        }

        static public Texture11 TextureFromBitmap(Bitmap bmp)
        {
            if (bmp==null)
            {
                return null;
            }

#if WINDOWS_UWP
            if (!string.IsNullOrWhiteSpace(bmp.FileName))
            {
                return new Texture11(bmp.FileName);
            }
            
            if (bmp.Stream != null)
            {
                return new Texture11(bmp.Stream);
            }

            return null;
#else
            return Texture11.FromBitmap(bmp);
#endif

        }

        static public Bitmap LoadThumbnailFromWeb(string url, string filename)
        {

            try
            {
                string fullPath = Properties.Settings.Default.CahceDirectory + @"thumbnails\" + filename;
                DataSetManager.DownloadFile(CacheProxy.GetCacheUrl(url), fullPath, true, true);
#if !WINDOWS_UWP
                return UiTools.LoadBitmap(Properties.Settings.Default.CahceDirectory + @"thumbnails\" + filename);
#else
                return new Bitmap(fullPath);
#endif
            }
            catch
            {
                return null;
            }

        }

        static public string GetNamesStringFromArray(string[] array)
        {
            string names = "";
            string delim = "";
            foreach (string name in array)
            {
                names += delim;
                names += name;
                delim = ";";
            }
            return names;
        }
#if !WINDOWS_UWP

        static public ScriptableProperty[] GetFilteredProperties(ScriptableProperty[] properties, BindingType bindType)
        {
            if (bindType == BindingType.Toggle)
            {
                //filter by toggle
                List<ScriptableProperty> filtered = new List<ScriptableProperty>();

                foreach (ScriptableProperty prop in properties)
                {
                    if (prop.Togglable)
                    {
                        filtered.Add(prop);
                    }
                }
                return filtered.ToArray();

            }
            else
            {
                return properties;
            }


        }


        //Stream version to avoid file locks
        static public Bitmap LoadBitmapFromStreamFile(string filename)
        {
            try
            {
                Bitmap bmpTemp = null;
                using (Stream stream = File.Open(filename, System.IO.FileMode.Open))
                {
                    Image tempImg = Image.FromStream(stream);
                    bmpTemp = new Bitmap(tempImg);
                    tempImg.Dispose();
                    GC.SuppressFinalize(tempImg);
                    stream.Close();
                    stream.Dispose();
                    GC.SuppressFinalize(stream);
                }


                return bmpTemp;
            }
            catch
            {
                return null;
            }

        }

        static public void SaveBitmap(Bitmap bmp, string filename, System.Drawing.Imaging.ImageFormat format)
        {
            using (Stream stream = File.Open(filename, FileMode.Create))
            {
                bmp.Save(stream, format);
                stream.Close();
            }
        }

        static public byte[] LoadBlob(string filename)
        {

            FileInfo fi = new FileInfo(filename);

            if (fi == null)
            {
                return null;
            }

            byte[] blob = new byte[(int)fi.Length];
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                fs.Read(blob, 0, (int)fi.Length);
                fs.Close();
            }

            return blob;
        }

        public static string[] GetBindingTargetTypeList()
        {
            List<String> types = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                types.Add(Enum.GetName(typeof(BindingTargetType), (BindingTargetType)i));
            }

            return types.ToArray();
        }



        static public string MakeGrayScaleImage(string filename)
        {
            string gsFilename = filename + ".gs.png";
            if (File.Exists(gsFilename))
            {
                return gsFilename;
            }

            Bitmap bmpIn = new Bitmap(filename);

            FastBitmap fastBmp = new FastBitmap(bmpIn);
            int width = bmpIn.Width;
            int height = bmpIn.Height;
            int stride = width;
            fastBmp.LockBitmap();
            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    int indexY = y;
                    PixelData* pData = fastBmp[0, y];
                    for (int x = 0; x < width; x++)
                    {
                        PixelData pixel = *pData;

                        int val = (int)Gamma((Gamma(pixel.green, 2.2f) * .6152 + Gamma(pixel.red, 2.2f) * .2126 + Gamma(pixel.blue, 2.2f) * .1722), 1 / 2.2f);

                        *pData++ = new PixelData(val, val, val, pixel.alpha);
                    }
                }
            }

            fastBmp.UnlockBitmap();

            bmpIn.Save(gsFilename, ImageFormat.Png);

            bmpIn.Dispose();
            return gsFilename;
        }

        public static double Gamma(double val, float gamma)
        {
            return Math.Min(255, ((255.0 * Math.Pow(val / 255.0, 1.0 / gamma))));
        }

        static public Color RgbOnlyColor(Color colorInput)
        {
            return Color.FromArgb(colorInput.R, colorInput.G, colorInput.B);
        }

        static public string CleanFileName(string filename)
        {
            char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
            int index = -1;
            while ((index = filename.IndexOfAny(invalidChars)) != -1)
            {
                filename = filename.Remove(index);
            }

            if (filename.Length == 0)
            {
                return null;
            }
            return filename;
        }
        public static bool IsEmail(string Email)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(Email))
                return (true);
            else
                return (false);
        }

        public static bool IsUrl(string Url)
        {
            string strRegex = "^(https?://)"
            + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@ 
            + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184 
            + "|" // allows either IP or domain 
            + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www. 
            + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." // second level domain 
            + "[a-z]{2,6})" // first level domain- .com or .museum 
            + "(:[0-9]{1,4})?" // port number- :80 
            + "((/?)|" // a slash isn't required if there is no file name 
            + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
            Regex re = new Regex(strRegex);

            if (re.IsMatch(Url))
                return (true);
            else
                return (false);
        }

        static WebClient client = new WebClient();
        /// <summary>
        /// This sends a http message to the server but does not wait for a response
        /// </summary>
        /// <param name="url"></param>
        public static void SendAsyncWebMessage(string url)
        {
            try
            {
                client.DownloadStringAsync(new Uri(url));
            }
            catch
            {
            }
        }
        public static DialogResult ShowMessageBox(string text, string title, MessageBoxButtons buttons, MessageBoxIcon Icon)
        {
            // todo make sure this is safe for projector servers
            if (Earth3d.MainWindow != null && Earth3d.MainWindow.Config != null && Earth3d.MainWindow.Config.Master == false)
            {
                return DialogResult.OK;
            }
            Earth3d.HideSplash = true;
            return MessageBox.Show(text, title, buttons, Icon);
        }
        public static DialogResult ShowMessageBox(string text, string title, MessageBoxButtons buttons)
        {
            if (Earth3d.MainWindow != null && Earth3d.MainWindow.Config != null && Earth3d.MainWindow.Config.Master == false)
            {
                return DialogResult.OK;
            }
            Earth3d.HideSplash = true;
            return MessageBox.Show(text, title, buttons);
        }
        public static DialogResult ShowMessageBox(string text, string title)
        {
            if (Earth3d.MainWindow != null && Earth3d.MainWindow.Config != null && Earth3d.MainWindow.Config.Master == false)
            {
                return DialogResult.OK;
            }
            Earth3d.HideSplash = true;
            return MessageBox.Show(text, title);
        }

        public static DialogResult ShowMessageBox(string text)
        {
            if (Earth3d.MainWindow != null && Earth3d.MainWindow.Config != null && Earth3d.MainWindow.Config.Master == false)
            {
                return DialogResult.OK;
            }
            Earth3d.HideSplash = true;
            return MessageBox.Show(text, Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hwnd, StringBuilder text, int length);
        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();

        public static Control GetFocusControl()
        {
            IntPtr focus = GetFocus();

            Control control = Control.FromHandle(focus);
            return control;
        }
        public static bool IsAppFocused()
        {
            return (GetFocusControl() != null);
        }
#else
        public static void ShowMessageBox(string text, string title = null)
        {

        }

#endif

        public const double KilometersPerAu = 149598000;
        public const double AuPerParsec = 206264.806;
        public const double AuPerLightYear = 63239.6717;
        public const double SSMUnitConversion = 370; // No idea where this fudge factors comes from
        public static double SolarSystemToMeters(double SolarSystemCameraDistance)
        {
            return SolarSystemCameraDistance * UiTools.KilometersPerAu * SSMUnitConversion;
        }

        public static double MetersToSolarSystemDistance(double meters)
        {
            return meters / SSMUnitConversion * UiTools.KilometersPerAu;
        }

        public static double GetMeters(double distance, AltUnits units)
        {
            double scaleFactor = 1.0;

            switch (units)
            {
                case AltUnits.Meters:
                    scaleFactor = 1.0;
                    break;
                case AltUnits.Feet:
                    scaleFactor = 1.0 / 3.2808399;
                    break;
                case AltUnits.Inches:
                    scaleFactor = (1.0 / 3.2808399) / 12;
                    break;
                case AltUnits.Miles:
                    scaleFactor = 1609.344;
                    break;
                case AltUnits.Kilometers:
                    scaleFactor = 1000;
                    break;
                case AltUnits.AstronomicalUnits:
                    scaleFactor = UiTools.KilometersPerAu * 1000;
                    break;
                case AltUnits.LightYears:
                    scaleFactor = UiTools.AuPerLightYear * UiTools.KilometersPerAu * 1000;
                    break;
                case AltUnits.Parsecs:
                    scaleFactor = UiTools.AuPerParsec * UiTools.KilometersPerAu * 1000;
                    break;
                case AltUnits.MegaParsecs:
                    scaleFactor = UiTools.AuPerParsec * UiTools.KilometersPerAu * 1000 * 1000000;
                    break;
                case AltUnits.Custom:
                    scaleFactor = 1;
                    break;
                default:
                    break;
            }

            return distance * scaleFactor;
        }

        public static double MetersToZoom(double meters)
        {
            return ((meters / 1000 / SSMUnitConversion) - 0.000001) / 4 * 9;
        }

        public static string FormatDistancePlain(double distance)
        {
            if (distance < .0001)
            {
                return distance.ToString("#.######");
            }
            else
                if (distance < .0001)
            {
                return distance.ToString("#.#####");
            }
            else
                    if (distance < .001)
            {
                return distance.ToString("#.####");
            }
            else
                        if (distance < .01)
            {
                return distance.ToString("#.###");
            }
            else
                            if (distance < .1)
            {
                return distance.ToString("#.##");
            }
            else
            {
                return distance.ToString("###,###,###,###.#");
            }

        }


        // Distance is stored in AU in WWT but is displayed in KM AU, LY, MPC
        public static string FormatDistance(double distance)
        {

            if (distance < .1)
            {
                // Kilometers
                double km = (distance * KilometersPerAu);

                if (km < 10)
                {
                    double m = (int)(km * 1000);
                    return m.ToString("###,###,###,###.#") + " m";
                }
                else
                {
                    km = (int)km;
                    return km.ToString("###,###,###,###.#") + " km";
                }
            }
            else if (distance < (10))
            {
                //Units in u
                double au = ((int)(distance * 10 + .5)) / 10.0;
                return au.ToString("###,###,###,###.#") + " au";
            }
            else if (distance < (AuPerLightYear / 10.0))
            {
                //Units in u
                double au = (int)(distance);
                return au.ToString("###,###,###,###.#") + " au";
            }
            else if (distance < (AuPerLightYear * 10))
            {
                // Units in lightyears
                double ly = ((int)((distance * 10) / AuPerLightYear)) / 10.0;
                return ly.ToString("###,###,###,###.#") + " ly";
            }
            else if (distance < (AuPerLightYear * 1000000))
            {
                // Units in lightyears
                double ly = ((int)((distance) / AuPerLightYear));
                return ly.ToString("###,###,###,###.#") + " ly";
            }
            else if (distance < (AuPerParsec * 10000000))
            {
                double mpc = ((int)((distance * 10) / (AuPerParsec * 1000000.0))) / 10.0;
                return mpc.ToString("###,###,###,###.#") + " Mpc";
            }
            else if (distance < (AuPerParsec * 1000000000))
            {
                double mpc = ((int)((distance) / (AuPerParsec * 1000000.0)));
                return mpc.ToString("###,###,###,###.#") + " Mpc";
            }
            else
            {
                double mpc = ((int)((distance * 10) / (AuPerParsec * 1000000000.0))) / 10.0;
                return mpc.ToString("###,###,###,###.#") + " Gpc";
            }
        }

        public static string FormatDecimalHours(double dayFraction)
        {
            TimeSpan ts = DateTime.UtcNow - DateTime.Now;

            double day = (dayFraction - ts.TotalHours) + 0.0083333334;
            while (day > 24)
            {
                day -= 24;
            }
            while (day < 0)
            {
                day += 24;
            }
            int hours = (int)day;
            int minutes = (int)((day * 60.0) - ((double)hours * 60.0));
            int seconds = (int)((day * 3600) - (((double)hours * 3600) + ((double)minutes * 60.0)));

            return string.Format("{0:00}:{1:00}", hours, minutes, seconds);
            //return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
#if !WINDOWS_UWP
        public static Bitmap MakeThumbnail(Bitmap imgOrig)
        {

            try
            {
                Bitmap bmpThumb = new Bitmap(96, 45);

                Graphics g = Graphics.FromImage(bmpThumb);

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                double imageAspect = ((double)imgOrig.Width) / (imgOrig.Height);

                double clientAspect = ((double)bmpThumb.Width) / bmpThumb.Height;

                int cw = bmpThumb.Width;
                int ch = bmpThumb.Height;

                if (imageAspect < clientAspect)
                {
                    ch = (int)((double)cw / imageAspect);
                }
                else
                {
                    cw = (int)((double)ch * imageAspect);
                }

                int cx = (bmpThumb.Width - cw) / 2;
                int cy = ((bmpThumb.Height - ch) / 2);// - 1;
                Rectangle destRect = new Rectangle(cx, cy, cw, ch);//+ 1);

                Rectangle srcRect = new Rectangle(0, 0, imgOrig.Width, imgOrig.Height);
                g.DrawImage(imgOrig, destRect, srcRect, System.Drawing.GraphicsUnit.Pixel);
                g.Dispose();
                GC.SuppressFinalize(g);
                return bmpThumb;
            }
            catch
            {
                Bitmap bmp = new Bitmap(96, 45);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.Blue);

                g.DrawString("Can't Capture", UiTools.StandardSmall, UiTools.StadardTextBrush, new PointF(3, 15));
                return bmp;
            }
        }



        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SystemParametersInfo(UInt32 action, UInt32 uParam, String vParam, UInt32 winIni);

        private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
        private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
        private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

        public static void SetWallpaper(String path)
        {
            RegistryKey desktop = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);


            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);

            desktop.SetValue("WallpaperStyle", 2);

            desktop.SetValue("TileWallpaper", 1);

            desktop.Close();


        }

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int which);

        [DllImport("user32.dll")]
        public static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
                         int X, int Y, int width, int height, uint flags);

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
        private static IntPtr HWND_TOP = IntPtr.Zero;
        private const int SWP_SHOWWINDOW = 64; // 0x0040

        public static void ShowFullScreen(Control form, bool doubleWide, int screenID)
        {
            Screen screenTarget;

            doubleWide = false;

            if (screenID == -1)
            {
                screenTarget = Screen.FromControl(form);
            }
            else
            {
                screenTarget = Screen.AllScreens[screenID];
            }

            int height = screenTarget.Bounds.Height;
            int width = screenTarget.Bounds.Width;

            if (Properties.Settings.Default.FullScreenHeight != 0 && Properties.Settings.Default.FullScreenWidth != 0)
            {
                //screen.Bounds
                SetWindowPos(form.Handle, HWND_TOP, Properties.Settings.Default.FullScreenX, Properties.Settings.Default.FullScreenY, Properties.Settings.Default.FullScreenWidth, Properties.Settings.Default.FullScreenHeight, SWP_SHOWWINDOW);
            }
            else
            {
                //screen.Bounds
                SetWindowPos(form.Handle, HWND_TOP, screenTarget.Bounds.X, screenTarget.Bounds.Y, doubleWide ? width * 2 : width, height, SWP_SHOWWINDOW);
            }
        }

        [DllImport("user32.dll")]
        private static extern bool MessageBeep(int uType);

        public static void Beep()
        {
            MessageBeep(-1);
        }


        // EMF Clipboard

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool OpenClipboard(IntPtr hwnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool EmptyClipboard();
        [DllImport("user32.dll")]
        internal static extern IntPtr SetClipboardData(uint format, IntPtr h);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetClipboardData(uint format);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsClipboardFormatAvailable(uint format);
        static uint CF_ENHMETAFILE = 14;

        public static bool IsMetaFileAvailable()
        {
            return IsClipboardFormatAvailable(CF_ENHMETAFILE);
        }

        public static Bitmap GetMetafileFromClipboard()
        {
            OpenClipboard(IntPtr.Zero);
            IntPtr hemf = GetClipboardData(CF_ENHMETAFILE);
            CloseClipboard();
            if (hemf != IntPtr.Zero)
            {
                Metafile mf = new Metafile(hemf, true);
                Bitmap b = new Bitmap(mf.Width, mf.Height);
                Graphics g = Graphics.FromImage(b);
                GraphicsUnit unit = GraphicsUnit.Millimeter;
                RectangleF rsrc = mf.GetBounds(ref unit);
                g.PageUnit = GraphicsUnit.Pixel;
                g.DrawImage(mf, 0, 0, mf.Width, mf.Height);
                g.Dispose();
                GC.SuppressFinalize(g);
                return b;
            }
            return null;
        }

        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        public static bool volInit = false;

        public static void SetSystemVolume(float fVol)
        {
            if (!volInit)
            {
                // This may fail if there is no sound card present
                try
                {
                    vol = new EndpointVolume();
                }
                catch
                {
                    vol = null;
                }
            }

            // only try this is sound was initialized
            if (vol != null)
            {
                vol.MasterVolume = fVol;
            }
        }

        public static float GetSystemVolume()
        {

            uint dwVolume = 0;
            waveOutGetVolume(IntPtr.Zero, out dwVolume);

            return dwVolume & 0x0000ffff / ushort.MaxValue;
        }

        static EndpointVolume vol = null;

        private static void SetVolume(int value)
        {
            try
            {


            }
            catch (Exception)
            {
            }
        }


#endif

        public static Stream GetResourceStream(string name)
        {
#if WINDOWS_UWP
            //todo UWP Get resource files from content directory
            return null;
#else
            return Earth3d.MainWindow.GetType().Assembly.GetManifestResourceStream(name);
#endif
        }
    }




    public static class WWTExtensions
    {
#if !WINDOWS_UWP
        public static unsafe int GetHashCode32(this String s)
        {
            fixed (char* str = s.ToCharArray())
            {
                char* chPtr = str;
                int num = 0x15051505;
                int num2 = num;
                int* numPtr = (int*)chPtr;
                for (int i = s.Length; i > 0; i -= 4)
                {
                    num = (((num << 5) + num) + (num >> 0x1b)) ^ numPtr[0];
                    if (i <= 2)
                    {
                        break;
                    }
                    num2 = (((num2 << 5) + num2) + (num2 >> 0x1b)) ^ numPtr[1];
                    numPtr += 2;
                }
                return (num + (num2 * 0x5d588b65));
            }
        }
#else
        public static int GetHashCode32(this String s)
        {
            return s.GetHashCode();
        }
#endif
    }

#if !WINDOWS_UWP

    public class EndpointVolume
    {

        const int DEVICE_STATE_ACTIVE = 0x00000001;
        const int DEVICE_STATE_DISABLE = 0x00000002;
        const int DEVICE_STATE_NOTPRESENT = 0x00000004;
        const int DEVICE_STATE_UNPLUGGED = 0x00000008;
        const int DEVICE_STATEMASK_ALL = 0x0000000f;
        [DllImport("ole32.Dll")]

        static public extern uint CoCreateInstance(ref Guid clsid,

        [MarshalAs(UnmanagedType.IUnknown)] object inner,

        uint context,

        ref Guid uuid,

        [MarshalAs(UnmanagedType.IUnknown)] out object rReturnedComObject);

        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"),

        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

        public interface IAudioEndpointVolume
        {
            int RegisterControlChangeNotify(DelegateMixerChange pNotify);
            int UnregisterControlChangeNotify(DelegateMixerChange pNotify);
            int GetChannelCount(ref uint pnChannelCount);
            int SetMasterVolumeLevel(float fLevelDB, Guid pguidEventContext);
            int SetMasterVolumeLevelScalar(float fLevel, Guid pguidEventContext);
            int GetMasterVolumeLevel(ref float pfLevelDB);
            int GetMasterVolumeLevelScalar(ref float pfLevel);
            int SetChannelVolumeLevel(uint nChannel, float fLevelDB, Guid pguidEventContext);
            int SetChannelVolumeLevelScalar(uint nChannel, float fLevel, Guid pguidEventContext);
            int GetChannelVolumeLevel(uint nChannel, ref float pfLevelDB);
            int GetChannelVolumeLevelScalar(uint nChannel, ref float pfLevel);
            int SetMute(bool bMute, ref Guid pguidEventContext);
            int GetMute(ref bool pbMute);
            int GetVolumeStepInfo(ref uint pnStep, ref uint pnStepCount);
            int VolumeStepUp(Guid pguidEventContext);
            int VolumeStepDown(Guid pguidEventContext);
            int QueryHardwareSupport(ref uint pdwHardwareSupportMask);
            int GetVolumeRange(ref float pflVolumeMindB, ref float pflVolumeMaxdB, ref float pflVolumeIncrementdB);
        }

        [Guid("0BD7A1BE-7A1A-44DB-8397-CC5392387B5E"),

        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

        public interface IMMDeviceCollection
        {
            int GetCount(ref uint pcDevices);
            int Item(uint nDevice, ref IntPtr ppDevice);

        }

        [Guid("D666063F-1587-4E43-81F1-B948E807363F"),

        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

        public interface IMMDevice
        {
            int Activate(ref Guid iid, uint dwClsCtx, IntPtr pActivationParams, ref IntPtr ppInterface);
            int OpenPropertyStore(int stgmAccess, ref IntPtr ppProperties);
            int GetId(ref string ppstrId);
            int GetState(ref int pdwState);
        }

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"),


        InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

        public interface IMMDeviceEnumerator
        {
            int EnumAudioEndpoints(EDataFlow dataFlow, int dwStateMask, ref IntPtr ppDevices);
            int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, ref IntPtr ppEndpoint);
            int GetDevice(string pwstrId, ref IntPtr ppDevice);
            int RegisterEndpointNotificationCallback(IntPtr pClient);
            int UnregisterEndpointNotificationCallback(IntPtr pClient);
        }

        [Flags]

        enum CLSCTX : uint
        {

            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_INPROC_SERVER16 = 0x8,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_INPROC_HANDLER16 = 0x20,
            CLSCTX_RESERVED1 = 0x40,
            CLSCTX_RESERVED2 = 0x80,
            CLSCTX_RESERVED3 = 0x100,
            CLSCTX_RESERVED4 = 0x200,
            CLSCTX_NO_CODE_DOWNLOAD = 0x400,
            CLSCTX_RESERVED5 = 0x800,
            CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
            CLSCTX_NO_FAILURE_LOG = 0x4000,
            CLSCTX_DISABLE_AAA = 0x8000,
            CLSCTX_ENABLE_AAA = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
            CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
            CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
            CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
        }

        public enum EDataFlow
        {

            eRender,

            eCapture,

            eAll,

            EDataFlow_enum_count

        }

        public enum ERole
        {

            eConsole,

            eMultimedia,

            eCommunications,

            ERole_enum_count

        }

        object oEnumerator = null;

        IMMDeviceEnumerator iMde = null;

        object oDevice = null;

        IMMDevice imd = null;

        object oEndPoint = null;

        IAudioEndpointVolume iAudioEndpoint = null;


        public delegate void DelegateMixerChange();
        public delegate void MixerChangedEventHandler();


        public EndpointVolume()
        {

            const uint CLSCTX_INPROC_SERVER = 1;

            Guid clsid = new Guid("BCDE0395-E52F-467C-8E3D-C4579291692E");

            Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");

            oEnumerator = null;

            uint hResult = CoCreateInstance(ref clsid, null, CLSCTX_INPROC_SERVER, ref IID_IUnknown, out oEnumerator);

            if (hResult != 0 || oEnumerator == null)
            {

                throw new Exception("CoCreateInstance() pInvoke failed");

            }

            iMde = oEnumerator as IMMDeviceEnumerator;

            if (iMde == null)
            {

                throw new Exception("COM cast failed to IMMDeviceEnumerator");

            }

            IntPtr pDevice = IntPtr.Zero;

            int retVal = iMde.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eConsole, ref pDevice);

            if (retVal != 0)
            {

                throw new Exception("IMMDeviceEnumerator.GetDefaultAudioEndpoint()");

            }

            int dwStateMask = DEVICE_STATE_ACTIVE | DEVICE_STATE_NOTPRESENT | DEVICE_STATE_UNPLUGGED;

            IntPtr pCollection = IntPtr.Zero;

            retVal = iMde.EnumAudioEndpoints(EDataFlow.eRender, dwStateMask, ref pCollection);

            if (retVal != 0)
            {

                throw new Exception("IMMDeviceEnumerator.EnumAudioEndpoints()");

            }

            oDevice = System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(pDevice);

            imd = oDevice as IMMDevice;

            if (imd == null)
            {

                throw new Exception("COM cast failed to IMMDevice");

            }

            Guid iid = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");

            uint dwClsCtx = (uint)CLSCTX.CLSCTX_ALL;

            IntPtr pActivationParams = IntPtr.Zero;

            IntPtr pEndPoint = IntPtr.Zero;

            retVal = imd.Activate(ref iid, dwClsCtx, pActivationParams, ref pEndPoint);

            if (retVal != 0)
            {

                throw new Exception("IMMDevice.Activate()");

            }

            oEndPoint = System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(pEndPoint);

            iAudioEndpoint = oEndPoint as IAudioEndpointVolume;

            if (iAudioEndpoint == null)
            {

                throw new Exception("COM cast failed to IAudioEndpointVolume");

            }
        }



        public virtual void Dispose()
        {

            if (iAudioEndpoint != null)
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(iAudioEndpoint);

                iAudioEndpoint = null;

            }

            if (oEndPoint != null)
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oEndPoint);

                oEndPoint = null;

            }

            if (imd != null)
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(imd);

                imd = null;

            }

            if (oDevice != null)
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oDevice);

                oDevice = null;

            }

            if (iMde != null)
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(iMde);

                iMde = null;

            }

            if (oEnumerator != null)
            {

                System.Runtime.InteropServices.Marshal.ReleaseComObject(oEnumerator);

                oEnumerator = null;

            }

        }


        private void MixerChange()
        {


        }


        public bool Mute
        {

            get
            {

                bool mute = false;

                int retVal = iAudioEndpoint.GetMute(ref mute);

                if (retVal != 0)
                {

                    throw new Exception("IAudioEndpointVolume.GetMute() failed!");

                }

                return mute;

            }

            set
            {

                Guid nullGuid = Guid.Empty;

                bool mute = value;

                // TODO

                // Problem #2 : This function always terminate with an internal error!

                int retVal = iAudioEndpoint.SetMute(mute, ref nullGuid);

                if (retVal != 0)
                {
                    throw new Exception("IAudioEndpointVolume.SetMute() failed!");
                }
            }

        }

        public float MasterVolume
        {

            get
            {
                float level = 0.0F;

                int retVal = iAudioEndpoint.GetMasterVolumeLevelScalar(ref level);

                if (retVal != 0)
                {
                    throw new Exception("IAudioEndpointVolume.GetMasterVolumeLevelScalar()");
                }

                return level;
            }

            set
            {

                float level = value;

                Guid nullGuid;

                nullGuid = Guid.Empty;

                int retVal = iAudioEndpoint.SetMasterVolumeLevelScalar(level, nullGuid);

                if (retVal != 0)
                {

                    throw new Exception("IAudioEndpointVolume.SetMasterVolumeLevelScalar()");
                }
            }
        }


        public void VolumeUp()
        {

            Guid nullGuid;

            nullGuid = Guid.Empty;

            int retVal = iAudioEndpoint.VolumeStepUp(nullGuid);

            if (retVal != 0)
            {
                throw new Exception("IAudioEndpointVolume.SetMute()");
            }
        }

        public void VolumeDown()
        {
            Guid nullGuid;
            nullGuid = Guid.Empty;
            int retVal = iAudioEndpoint.VolumeStepDown(nullGuid);
        }
    }
#endif

#region Colors
    //public static class SystemColors
    //{
    //    public static Color MediumPurple { get { return Color.FromArgb(0xFF, 0x93, 0x70, 0xDB); } }
    //    public static Color MediumSeaGreen { get { return Color.FromArgb(0xFF, 0x3C, 0xB3, 0x71); } }
    //    public static Color MediumSlateBlue { get { return Color.FromArgb(0xFF, 0x7B, 0x68, 0xEE); } }
    //    public static Color MediumSpringGreen { get { return Color.FromArgb(0xFF, 0x00, 0xFA, 0x9A); } }
    //    public static Color MediumTurquoise { get { return Color.FromArgb(0xFF, 0x48, 0xD1, 0xCC); } }
    //    public static Color MediumVioletRed { get { return Color.FromArgb(0xFF, 0xC7, 0x15, 0x85); } }
    //    public static Color MidnightBlue { get { return Color.FromArgb(0xFF, 0x19, 0x19, 0x70); } }
    //    public static Color MediumOrchid { get { return Color.FromArgb(0xFF, 0xBA, 0x55, 0xD3); } }
    //    public static Color MintCream { get { return Color.FromArgb(0xFF, 0xF5, 0xFF, 0xFA); } }
    //    public static Color Moccasin { get { return Color.FromArgb(0xFF, 0xFF, 0xE4, 0xB5); } }
    //    public static Color NavajoWhite { get { return Color.FromArgb(0xFF, 0xFF, 0xDE, 0xAD); } }
    //    public static Color Navy { get { return Color.FromArgb(0xFF, 0x00, 0x00, 0x80); } }
    //    public static Color OldLace { get { return Color.FromArgb(0xFF, 0xFD, 0xF5, 0xE6); } }
    //    public static Color Olive { get { return Color.FromArgb(0xFF, 0x80, 0x80, 0x00); } }
    //    public static Color OliveDrab { get { return Color.FromArgb(0xFF, 0x6B, 0x8E, 0x23); } }
    //    public static Color Orange { get { return Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00); } }
    //    public static Color MistyRose { get { return Color.FromArgb(0xFF, 0xFF, 0xE4, 0xE1); } }
    //    public static Color OrangeRed { get { return Color.FromArgb(0xFF, 0xFF, 0x45, 0x00); } }
    //    public static Color MediumBlue { get { return Color.FromArgb(0xFF, 0x00, 0x00, 0xCD); } }
    //    public static Color Maroon { get { return Color.FromArgb(0xFF, 0x80, 0x00, 0x00); } }
    //    public static Color LightBlue { get { return Color.FromArgb(0xFF, 0xAD, 0xD8, 0xE6); } }
    //    public static Color LightCoral { get { return Color.FromArgb(0xFF, 0xF0, 0x80, 0x80); } }
    //    public static Color LightGoldenrodYellow { get { return Color.FromArgb(0xFF, 0xFA, 0xFA, 0xD2); } }
    //    public static Color LightGreen { get { return Color.FromArgb(0xFF, 0x90, 0xEE, 0x90); } }
    //    public static Color LightGray { get { return Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3); } }
    //    public static Color LightPink { get { return Color.FromArgb(0xFF, 0xFF, 0xB6, 0xC1); } }
    //    public static Color LightSalmon { get { return Color.FromArgb(0xFF, 0xFF, 0xA0, 0x7A); } }
    //    public static Color MediumAquamarine { get { return Color.FromArgb(0xFF, 0x66, 0xCD, 0xAA); } }
    //    public static Color LightSeaGreen { get { return Color.FromArgb(0xFF, 0x20, 0xB2, 0xAA); } }
    //    public static Color LightSlateGray { get { return Color.FromArgb(0xFF, 0x77, 0x88, 0x99); } }
    //    public static Color LightSteelBlue { get { return Color.FromArgb(0xFF, 0xB0, 0xC4, 0xDE); } }
    //    public static Color LightYellow { get { return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xE0); } }
    //    public static Color Lime { get { return Color.FromArgb(0xFF, 0x00, 0xFF, 0x00); } }
    //    public static Color LimeGreen { get { return Color.FromArgb(0xFF, 0x32, 0xCD, 0x32); } }
    //    public static Color Linen { get { return Color.FromArgb(0xFF, 0xFA, 0xF0, 0xE6); } }
    //    public static Color Magenta { get { return Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF); } }
    //    public static Color LightSkyBlue { get { return Color.FromArgb(0xFF, 0x87, 0xCE, 0xFA); } }
    //    public static Color LemonChiffon { get { return Color.FromArgb(0xFF, 0xFF, 0xFA, 0xCD); } }
    //    public static Color Orchid { get { return Color.FromArgb(0xFF, 0xDA, 0x70, 0xD6); } }
    //    public static Color PaleGreen { get { return Color.FromArgb(0xFF, 0x98, 0xFB, 0x98); } }
    //    public static Color SlateBlue { get { return Color.FromArgb(0xFF, 0x6A, 0x5A, 0xCD); } }
    //    public static Color SlateGray { get { return Color.FromArgb(0xFF, 0x70, 0x80, 0x90); } }
    //    public static Color Snow { get { return Color.FromArgb(0xFF, 0xFF, 0xFA, 0xFA); } }
    //    public static Color SpringGreen { get { return Color.FromArgb(0xFF, 0x00, 0xFF, 0x7F); } }
    //    public static Color SteelBlue { get { return Color.FromArgb(0xFF, 0x46, 0x82, 0xB4); } }
    //    public static Color Tan { get { return Color.FromArgb(0xFF, 0xD2, 0xB4, 0x8C); } }
    //    public static Color Teal { get { return Color.FromArgb(0xFF, 0x00, 0x80, 0x80); } }
    //    public static Color SkyBlue { get { return Color.FromArgb(0xFF, 0x87, 0xCE, 0xEB); } }
    //    public static Color Thistle { get { return Color.FromArgb(0xFF, 0xD8, 0xBF, 0xD8); } }
    //    public static Color Turquoise { get { return Color.FromArgb(0xFF, 0x40, 0xE0, 0xD0); } }
    //    public static Color Violet { get { return Color.FromArgb(0xFF, 0xEE, 0x82, 0xEE); } }
    //    public static Color Wheat { get { return Color.FromArgb(0xFF, 0xF5, 0xDE, 0xB3); } }
    //    public static Color White { get { return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF); } }
    //    public static Color WhiteSmoke { get { return Color.FromArgb(0xFF, 0xF5, 0xF5, 0xF5); } }
    //    public static Color Yellow { get { return Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00); } }
    //    public static Color YellowGreen { get { return Color.FromArgb(0xFF, 0x9A, 0xCD, 0x32); } }
    //    public static Color Tomato { get { return Color.FromArgb(0xFF, 0xFF, 0x63, 0x47); } }
    //    public static Color PaleGoldenrod { get { return Color.FromArgb(0xFF, 0xEE, 0xE8, 0xAA); } }
    //    public static Color Silver { get { return Color.FromArgb(0xFF, 0xC0, 0xC0, 0xC0); } }
    //    public static Color SeaShell { get { return Color.FromArgb(0xFF, 0xFF, 0xF5, 0xEE); } }
    //    public static Color PaleTurquoise { get { return Color.FromArgb(0xFF, 0xAF, 0xEE, 0xEE); } }
    //    public static Color PaleVioletRed { get { return Color.FromArgb(0xFF, 0xDB, 0x70, 0x93); } }
    //    public static Color PapayaWhip { get { return Color.FromArgb(0xFF, 0xFF, 0xEF, 0xD5); } }
    //    public static Color PeachPuff { get { return Color.FromArgb(0xFF, 0xFF, 0xDA, 0xB9); } }
    //    public static Color Peru { get { return Color.FromArgb(0xFF, 0xCD, 0x85, 0x3F); } }
    //    public static Color Pink { get { return Color.FromArgb(0xFF, 0xFF, 0xC0, 0xCB); } }
    //    public static Color Plum { get { return Color.FromArgb(0xFF, 0xDD, 0xA0, 0xDD); } }
    //    public static Color Sienna { get { return Color.FromArgb(0xFF, 0xA0, 0x52, 0x2D); } }
    //    public static Color PowderBlue { get { return Color.FromArgb(0xFF, 0xB0, 0xE0, 0xE6); } }
    //    public static Color Red { get { return Color.FromArgb(0xFF, 0xFF, 0x00, 0x00); } }
    //    public static Color RosyBrown { get { return Color.FromArgb(0xFF, 0xBC, 0x8F, 0x8F); } }
    //    public static Color RoyalBlue { get { return Color.FromArgb(0xFF, 0x41, 0x69, 0xE1); } }
    //    public static Color SaddleBrown { get { return Color.FromArgb(0xFF, 0x8B, 0x45, 0x13); } }
    //    public static Color Salmon { get { return Color.FromArgb(0xFF, 0xFA, 0x80, 0x72); } }
    //    public static Color SandyBrown { get { return Color.FromArgb(0xFF, 0xF4, 0xA4, 0x60); } }
    //    public static Color SeaGreen { get { return Color.FromArgb(0xFF, 0x2E, 0x8B, 0x57); } }
    //    public static Color Purple { get { return Color.FromArgb(0xFF, 0x80, 0x00, 0x80); } }
    //    public static Color LawnGreen { get { return Color.FromArgb(0xFF, 0x7C, 0xFC, 0x00); } }
    //    public static Color LightCyan { get { return Color.FromArgb(0xFF, 0xE0, 0xFF, 0xFF); } }
    //    public static Color Lavender { get { return Color.FromArgb(0xFF, 0xE6, 0xE6, 0xFA); } }
    //    public static Color DarkKhaki { get { return Color.FromArgb(0xFF, 0xBD, 0xB7, 0x6B); } }
    //    public static Color DarkGreen { get { return Color.FromArgb(0xFF, 0x00, 0x64, 0x00); } }
    //    public static Color DarkGray { get { return Color.FromArgb(0xFF, 0xA9, 0xA9, 0xA9); } }
    //    public static Color DarkGoldenrod { get { return Color.FromArgb(0xFF, 0xB8, 0x86, 0x0B); } }
    //    public static Color DarkCyan { get { return Color.FromArgb(0xFF, 0x00, 0x8B, 0x8B); } }
    //    public static Color DarkBlue { get { return Color.FromArgb(0xFF, 0x00, 0x00, 0x8B); } }
    //    public static Color Cyan { get { return Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF); } }
    //    public static Color Crimson { get { return Color.FromArgb(0xFF, 0xDC, 0x14, 0x3C); } }
    //    public static Color Cornsilk { get { return Color.FromArgb(0xFF, 0xFF, 0xF8, 0xDC); } }
    //    public static Color LavenderBlush { get { return Color.FromArgb(0xFF, 0xFF, 0xF0, 0xF5); } }
    //    public static Color Coral { get { return Color.FromArgb(0xFF, 0xFF, 0x7F, 0x50); } }
    //    public static Color Chocolate { get { return Color.FromArgb(0xFF, 0xD2, 0x69, 0x1E); } }
    //    public static Color Chartreuse { get { return Color.FromArgb(0xFF, 0x7F, 0xFF, 0x00); } }
    //    public static Color DarkMagenta { get { return Color.FromArgb(0xFF, 0x8B, 0x00, 0x8B); } }
    //    public static Color CadetBlue { get { return Color.FromArgb(0xFF, 0x5F, 0x9E, 0xA0); } }
    //    public static Color Brown { get { return Color.FromArgb(0xFF, 0xA5, 0x2A, 0x2A); } }
    //    public static Color BlueViolet { get { return Color.FromArgb(0xFF, 0x8A, 0x2B, 0xE2); } }
    //    public static Color Blue { get { return Color.FromArgb(0xFF, 0x00, 0x00, 0xFF); } }
    //    public static Color BlanchedAlmond { get { return Color.FromArgb(0xFF, 0xFF, 0xEB, 0xCD); } }
    //    public static Color Black { get { return Color.FromArgb(0xFF, 0x00, 0x00, 0x00); } }
    //    public static Color Bisque { get { return Color.FromArgb(0xFF, 0xFF, 0xE4, 0xC4); } }
    //    public static Color Beige { get { return Color.FromArgb(0xFF, 0xF5, 0xF5, 0xDC); } }
    //    public static Color Azure { get { return Color.FromArgb(0xFF, 0xF0, 0xFF, 0xFF); } }
    //    public static Color Aquamarine { get { return Color.FromArgb(0xFF, 0x7F, 0xFF, 0xD4); } }
    //    public static Color Aqua { get { return Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF); } }
    //    public static Color AntiqueWhite { get { return Color.FromArgb(0xFF, 0xFA, 0xEB, 0xD7); } }
    //    public static Color AliceBlue { get { return Color.FromArgb(0xFF, 0xF0, 0xF8, 0xFF); } }
    //    public static Color Transparent { get { return Color.FromArgb(0xFF, 0xF0, 0xF8, 0xFF); } }
    //    public static Color BurlyWood { get { return Color.FromArgb(0xFF, 0xDE, 0xB8, 0x87); } }
    //    public static Color DarkOliveGreen { get { return Color.FromArgb(0xFF, 0x55, 0x6B, 0x2F); } }
    //    public static Color CornflowerBlue { get { return Color.FromArgb(0xFF, 0x64, 0x95, 0xED); } }
    //    public static Color DarkOrchid { get { return Color.FromArgb(0xFF, 0x99, 0x32, 0xCC); } }
    //    public static Color Khaki { get { return Color.FromArgb(0xFF, 0xF0, 0xE6, 0x8C); } }
    //    public static Color Ivory { get { return Color.FromArgb(0xFF, 0xFF, 0xFF, 0xF0); } }
    //    public static Color DarkOrange { get { return Color.FromArgb(0xFF, 0xFF, 0x8C, 0x00); } }
    //    public static Color Indigo { get { return Color.FromArgb(0xFF, 0x4B, 0x00, 0x82); } }
    //    public static Color IndianRed { get { return Color.FromArgb(0xFF, 0xCD, 0x5C, 0x5C); } }
    //    public static Color HotPink { get { return Color.FromArgb(0xFF, 0xFF, 0x69, 0xB4); } }
    //    public static Color Honeydew { get { return Color.FromArgb(0xFF, 0xF0, 0xFF, 0xF0); } }
    //    public static Color GreenYellow { get { return Color.FromArgb(0xFF, 0xAD, 0xFF, 0x2F); } }
    //    public static Color Green { get { return Color.FromArgb(0xFF, 0x00, 0x80, 0x00); } }
    //    public static Color Gray { get { return Color.FromArgb(0xFF, 0x80, 0x80, 0x80); } }
    //    public static Color Goldenrod { get { return Color.FromArgb(0xFF, 0xDA, 0xA5, 0x20); } }
    //    public static Color GhostWhite { get { return Color.FromArgb(0xFF, 0xF8, 0xF8, 0xFF); } }
    //    public static Color Gainsboro { get { return Color.FromArgb(0xFF, 0xDC, 0xDC, 0xDC); } }
    //    public static Color Fuchsia { get { return Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF); } }
    //    public static Color Gold { get { return Color.FromArgb(0xFF, 0xFF, 0xD7, 0x00); } }
    //    public static Color FloralWhite { get { return Color.FromArgb(0xFF, 0xFF, 0xFA, 0xF0); } }
    //    public static Color DarkRed { get { return Color.FromArgb(0xFF, 0x8B, 0x00, 0x00); } }
    //    public static Color DarkSalmon { get { return Color.FromArgb(0xFF, 0xE9, 0x96, 0x7A); } }
    //    public static Color DarkSeaGreen { get { return Color.FromArgb(0xFF, 0x8F, 0xBC, 0x8F); } }
    //    public static Color ForestGreen { get { return Color.FromArgb(0xFF, 0x22, 0x8B, 0x22); } }
    //    public static Color DarkSlateGray { get { return Color.FromArgb(0xFF, 0x2F, 0x4F, 0x4F); } }
    //    public static Color DarkTurquoise { get { return Color.FromArgb(0xFF, 0x00, 0xCE, 0xD1); } }
    //    public static Color DarkSlateBlue { get { return Color.FromArgb(0xFF, 0x48, 0x3D, 0x8B); } }
    //    public static Color DeepPink { get { return Color.FromArgb(0xFF, 0xFF, 0x14, 0x93); } }
    //    public static Color DeepSkyBlue { get { return Color.FromArgb(0xFF, 0x00, 0xBF, 0xFF); } }
    //    public static Color DimGray { get { return Color.FromArgb(0xFF, 0x69, 0x69, 0x69); } }
    //    public static Color DodgerBlue { get { return Color.FromArgb(0xFF, 0x1E, 0x90, 0xFF); } }
    //    public static Color Firebrick { get { return Color.FromArgb(0xFF, 0xB2, 0x22, 0x22); } }
    //    public static Color DarkViolet { get { return Color.FromArgb(0xFF, 0x94, 0x00, 0xD3); } }

    //}
#endregion
}


