/*  
===============================================================================
 2007-2008 Copyright © Microsoft Corporation.  All rights reserved.
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
 OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
 LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 FITNESS FOR A PARTICULAR PURPOSE.
===============================================================================
*/
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

namespace TerraViewer
{
    public sealed class HttpUtility
    {

        #region Fields

        const string _hex = "0123456789ABCDEF";
        const string _chars = "<>;:.?=&@*+%/\\";
        static Hashtable _Entities;

        #endregion // Fields

        #region Constructors

        static HttpUtility()
        {

            _Entities = new Hashtable();
            _Entities.Add("nbsp", '\u00A0');
            _Entities.Add("iexcl", '\u00A1');
            _Entities.Add("cent", '\u00A2');
            _Entities.Add("pound", '\u00A3');
            _Entities.Add("curren", '\u00A4');
            _Entities.Add("yen", '\u00A5');
            _Entities.Add("brvbar", '\u00A6');
            _Entities.Add("sect", '\u00A7');
            _Entities.Add("uml", '\u00A8');
            _Entities.Add("copy", '\u00A9');
            _Entities.Add("ordf", '\u00AA');
            _Entities.Add("laquo", '\u00AB');
            _Entities.Add("not", '\u00AC');
            _Entities.Add("shy", '\u00AD');
            _Entities.Add("reg", '\u00AE');
            _Entities.Add("macr", '\u00AF');
            _Entities.Add("deg", '\u00B0');
            _Entities.Add("plusmn", '\u00B1');
            _Entities.Add("sup2", '\u00B2');
            _Entities.Add("sup3", '\u00B3');
            _Entities.Add("acute", '\u00B4');
            _Entities.Add("micro", '\u00B5');
            _Entities.Add("para", '\u00B6');
            _Entities.Add("middot", '\u00B7');
            _Entities.Add("cedil", '\u00B8');
            _Entities.Add("sup1", '\u00B9');
            _Entities.Add("ordm", '\u00BA');
            _Entities.Add("raquo", '\u00BB');
            _Entities.Add("frac14", '\u00BC');
            _Entities.Add("frac12", '\u00BD');
            _Entities.Add("frac34", '\u00BE');
            _Entities.Add("iquest", '\u00BF');
            _Entities.Add("Agrave", '\u00C0');
            _Entities.Add("Aacute", '\u00C1');
            _Entities.Add("Acirc", '\u00C2');
            _Entities.Add("Atilde", '\u00C3');
            _Entities.Add("Auml", '\u00C4');
            _Entities.Add("Aring", '\u00C5');
            _Entities.Add("AElig", '\u00C6');
            _Entities.Add("Ccedil", '\u00C7');
            _Entities.Add("Egrave", '\u00C8');
            _Entities.Add("Eacute", '\u00C9');
            _Entities.Add("Ecirc", '\u00CA');
            _Entities.Add("Euml", '\u00CB');
            _Entities.Add("Igrave", '\u00CC');
            _Entities.Add("Iacute", '\u00CD');
            _Entities.Add("Icirc", '\u00CE');
            _Entities.Add("Iuml", '\u00CF');
            _Entities.Add("ETH", '\u00D0');
            _Entities.Add("Ntilde", '\u00D1');
            _Entities.Add("Ograve", '\u00D2');
            _Entities.Add("Oacute", '\u00D3');
            _Entities.Add("Ocirc", '\u00D4');
            _Entities.Add("Otilde", '\u00D5');
            _Entities.Add("Ouml", '\u00D6');
            _Entities.Add("times", '\u00D7');
            _Entities.Add("Oslash", '\u00D8');
            _Entities.Add("Ugrave", '\u00D9');
            _Entities.Add("Uacute", '\u00DA');
            _Entities.Add("Ucirc", '\u00DB');
            _Entities.Add("Uuml", '\u00DC');
            _Entities.Add("Yacute", '\u00DD');
            _Entities.Add("THORN", '\u00DE');
            _Entities.Add("szlig", '\u00DF');
            _Entities.Add("agrave", '\u00E0');
            _Entities.Add("aacute", '\u00E1');
            _Entities.Add("acirc", '\u00E2');
            _Entities.Add("atilde", '\u00E3');
            _Entities.Add("auml", '\u00E4');
            _Entities.Add("aring", '\u00E5');
            _Entities.Add("aelig", '\u00E6');
            _Entities.Add("ccedil", '\u00E7');
            _Entities.Add("egrave", '\u00E8');
            _Entities.Add("eacute", '\u00E9');
            _Entities.Add("ecirc", '\u00EA');
            _Entities.Add("euml", '\u00EB');
            _Entities.Add("igrave", '\u00EC');
            _Entities.Add("iacute", '\u00ED');
            _Entities.Add("icirc", '\u00EE');
            _Entities.Add("iuml", '\u00EF');
            _Entities.Add("eth", '\u00F0');
            _Entities.Add("ntilde", '\u00F1');
            _Entities.Add("ograve", '\u00F2');
            _Entities.Add("oacute", '\u00F3');
            _Entities.Add("ocirc", '\u00F4');
            _Entities.Add("otilde", '\u00F5');
            _Entities.Add("ouml", '\u00F6');
            _Entities.Add("divide", '\u00F7');
            _Entities.Add("oslash", '\u00F8');
            _Entities.Add("ugrave", '\u00F9');
            _Entities.Add("uacute", '\u00FA');
            _Entities.Add("ucirc", '\u00FB');
            _Entities.Add("uuml", '\u00FC');
            _Entities.Add("yacute", '\u00FD');
            _Entities.Add("thorn", '\u00FE');
            _Entities.Add("yuml", '\u00FF');
            _Entities.Add("fnof", '\u0192');
            _Entities.Add("Alpha", '\u0391');
            _Entities.Add("Beta", '\u0392');
            _Entities.Add("Gamma", '\u0393');
            _Entities.Add("Delta", '\u0394');
            _Entities.Add("Epsilon", '\u0395');
            _Entities.Add("Zeta", '\u0396');
            _Entities.Add("Eta", '\u0397');
            _Entities.Add("Theta", '\u0398');
            _Entities.Add("Iota", '\u0399');
            _Entities.Add("Kappa", '\u039A');
            _Entities.Add("Lambda", '\u039B');
            _Entities.Add("Mu", '\u039C');
            _Entities.Add("Nu", '\u039D');
            _Entities.Add("Xi", '\u039E');
            _Entities.Add("Omicron", '\u039F');
            _Entities.Add("Pi", '\u03A0');
            _Entities.Add("Rho", '\u03A1');
            _Entities.Add("Sigma", '\u03A3');
            _Entities.Add("Tau", '\u03A4');
            _Entities.Add("Upsilon", '\u03A5');
            _Entities.Add("Phi", '\u03A6');
            _Entities.Add("Chi", '\u03A7');
            _Entities.Add("Psi", '\u03A8');
            _Entities.Add("Omega", '\u03A9');
            _Entities.Add("alpha", '\u03B1');
            _Entities.Add("beta", '\u03B2');
            _Entities.Add("gamma", '\u03B3');
            _Entities.Add("delta", '\u03B4');
            _Entities.Add("epsilon", '\u03B5');
            _Entities.Add("zeta", '\u03B6');
            _Entities.Add("eta", '\u03B7');
            _Entities.Add("theta", '\u03B8');
            _Entities.Add("iota", '\u03B9');
            _Entities.Add("kappa", '\u03BA');
            _Entities.Add("lambda", '\u03BB');
            _Entities.Add("mu", '\u03BC');
            _Entities.Add("nu", '\u03BD');
            _Entities.Add("xi", '\u03BE');
            _Entities.Add("omicron", '\u03BF');
            _Entities.Add("pi", '\u03C0');
            _Entities.Add("rho", '\u03C1');
            _Entities.Add("sigmaf", '\u03C2');
            _Entities.Add("sigma", '\u03C3');
            _Entities.Add("tau", '\u03C4');
            _Entities.Add("upsilon", '\u03C5');
            _Entities.Add("phi", '\u03C6');
            _Entities.Add("chi", '\u03C7');
            _Entities.Add("psi", '\u03C8');
            _Entities.Add("omega", '\u03C9');
            _Entities.Add("thetasym", '\u03D1');
            _Entities.Add("upsih", '\u03D2');
            _Entities.Add("piv", '\u03D6');
            _Entities.Add("bull", '\u2022');
            _Entities.Add("hellip", '\u2026');
            _Entities.Add("prime", '\u2032');
            _Entities.Add("Prime", '\u2033');
            _Entities.Add("oline", '\u203E');
            _Entities.Add("frasl", '\u2044');
            _Entities.Add("weierp", '\u2118');
            _Entities.Add("image", '\u2111');
            _Entities.Add("real", '\u211C');
            _Entities.Add("trade", '\u2122');
            _Entities.Add("alefsym", '\u2135');
            _Entities.Add("larr", '\u2190');
            _Entities.Add("uarr", '\u2191');
            _Entities.Add("rarr", '\u2192');
            _Entities.Add("darr", '\u2193');
            _Entities.Add("harr", '\u2194');
            _Entities.Add("crarr", '\u21B5');
            _Entities.Add("lArr", '\u21D0');
            _Entities.Add("uArr", '\u21D1');
            _Entities.Add("rArr", '\u21D2');
            _Entities.Add("dArr", '\u21D3');
            _Entities.Add("hArr", '\u21D4');
            _Entities.Add("forall", '\u2200');
            _Entities.Add("part", '\u2202');
            _Entities.Add("exist", '\u2203');
            _Entities.Add("empty", '\u2205');
            _Entities.Add("nabla", '\u2207');
            _Entities.Add("isin", '\u2208');
            _Entities.Add("notin", '\u2209');
            _Entities.Add("ni", '\u220B');
            _Entities.Add("prod", '\u220F');
            _Entities.Add("sum", '\u2211');
            _Entities.Add("minus", '\u2212');
            _Entities.Add("lowast", '\u2217');
            _Entities.Add("radic", '\u221A');
            _Entities.Add("prop", '\u221D');
            _Entities.Add("infin", '\u221E');
            _Entities.Add("ang", '\u2220');
            _Entities.Add("and", '\u2227');
            _Entities.Add("or", '\u2228');
            _Entities.Add("cap", '\u2229');
            _Entities.Add("cup", '\u222A');
            _Entities.Add("int", '\u222B');
            _Entities.Add("there4", '\u2234');
            _Entities.Add("sim", '\u223C');
            _Entities.Add("cong", '\u2245');
            _Entities.Add("asymp", '\u2248');
            _Entities.Add("ne", '\u2260');
            _Entities.Add("equiv", '\u2261');
            _Entities.Add("le", '\u2264');
            _Entities.Add("ge", '\u2265');
            _Entities.Add("sub", '\u2282');
            _Entities.Add("sup", '\u2283');
            _Entities.Add("nsub", '\u2284');
            _Entities.Add("sube", '\u2286');
            _Entities.Add("supe", '\u2287');
            _Entities.Add("oplus", '\u2295');
            _Entities.Add("otimes", '\u2297');
            _Entities.Add("perp", '\u22A5');
            _Entities.Add("sdot", '\u22C5');
            _Entities.Add("lceil", '\u2308');
            _Entities.Add("rceil", '\u2309');
            _Entities.Add("lfloor", '\u230A');
            _Entities.Add("rfloor", '\u230B');
            _Entities.Add("lang", '\u2329');
            _Entities.Add("rang", '\u232A');
            _Entities.Add("loz", '\u25CA');
            _Entities.Add("spades", '\u2660');
            _Entities.Add("clubs", '\u2663');
            _Entities.Add("hearts", '\u2665');
            _Entities.Add("diams", '\u2666');
            _Entities.Add("quot", '\u0022');
            _Entities.Add("amp", '\u0026');
            _Entities.Add("lt", '\u003C');
            _Entities.Add("gt", '\u003E');
            _Entities.Add("OElig", '\u0152');
            _Entities.Add("oelig", '\u0153');
            _Entities.Add("Scaron", '\u0160');
            _Entities.Add("scaron", '\u0161');
            _Entities.Add("Yuml", '\u0178');
            _Entities.Add("circ", '\u02C6');
            _Entities.Add("tilde", '\u02DC');
            _Entities.Add("ensp", '\u2002');
            _Entities.Add("emsp", '\u2003');
            _Entities.Add("thinsp", '\u2009');
            _Entities.Add("zwnj", '\u200C');
            _Entities.Add("zwj", '\u200D');
            _Entities.Add("lrm", '\u200E');
            _Entities.Add("rlm", '\u200F');
            _Entities.Add("ndash", '\u2013');
            _Entities.Add("mdash", '\u2014');
            _Entities.Add("lsquo", '\u2018');
            _Entities.Add("rsquo", '\u2019');
            _Entities.Add("sbquo", '\u201A');
            _Entities.Add("ldquo", '\u201C');
            _Entities.Add("rdquo", '\u201D');
            _Entities.Add("bdquo", '\u201E');
            _Entities.Add("dagger", '\u2020');
            _Entities.Add("Dagger", '\u2021');
            _Entities.Add("permil", '\u2030');
            _Entities.Add("lsaquo", '\u2039');
            _Entities.Add("rsaquo", '\u203A');
            _Entities.Add("euro", '\u20AC');
        }

        public HttpUtility()
        {
        }

        #endregion // Constructors

        #region Methods

        public static void HtmlAttributeEncode(string s, TextWriter output)
        {
            output.Write(HtmlAttributeEncode(s));
        }

        public static string HtmlAttributeEncode(string s)
        {
            if (null == s)
                return null;

            StringBuilder output = new StringBuilder();

            foreach (char c in s)
                switch (c)
                {
                    case '&':
                        output.Append("&");
                        break;
                    case '"':
                        output.Append("\"");
                        break;
                    default:
                        output.Append(c);
                        break;
                }

            return output.ToString();
        }

        public static string UrlDecode(string str)
        {
            return UrlDecode(str, Encoding.UTF8);
        }

        private static char[] GetChars(MemoryStream b, Encoding e)
        {
            return e.GetChars(b.GetBuffer(), 0, (int)b.Length);
        }

        public static string UrlDecode(string s, Encoding e)
        {
            if (null == s)
                return null;

            if (e == null)
                e = Encoding.UTF8;

            StringBuilder output = new StringBuilder();
            long len = s.Length;
            NumberStyles hexa = NumberStyles.HexNumber;
            MemoryStream bytes = new MemoryStream();

            for (int i = 0; i < len; i++)
            {
                if (s[i] == '%' && i + 2 < len)
                {
                    if (s[i + 1] == 'u' && i + 5 < len)
                    {
                        if (bytes.Length > 0)
                        {
                            output.Append(GetChars(bytes, e));
                            bytes.SetLength(0);
                        }

                        output.Append((char)Int32.Parse(s.Substring(i + 2, 4), hexa));
                        i += 5;
                    }
                    else
                    {
                        bytes.WriteByte((byte)Int32.Parse(s.Substring(i + 1, 2), hexa));
                        i += 2;
                    }
                    continue;
                }

                if (bytes.Length > 0)
                {
                    output.Append(GetChars(bytes, e));
                    bytes.SetLength(0);
                }

                if (s[i] == '+')
                {
                    output.Append(' ');
                }
                else
                {
                    output.Append(s[i]);
                }
            }

            if (bytes.Length > 0)
            {
                output.Append(GetChars(bytes, e));
            }

            bytes = null;
            return output.ToString();
        }

        public static string UrlDecode(byte[] bytes, Encoding e)
        {
            if (bytes == null)
                return null;

            return UrlDecode(bytes, 0, bytes.Length, e);
        }

        private static int GetInt(byte b)
        {
            char c = Char.ToUpper((char)b);
            if (c >= '0' && c <= '9')
                return c - '0';

            if (c < 'A' || c > 'F')
                return 0;

            return (c - 'A' + 10);
        }

        private static char GetChar(byte[] bytes, int offset, int length)
        {
            int value = 0;
            int end = length + offset;
            for (int i = offset; i < end; i++)
                value = (value << 4) + GetInt(bytes[offset]);

            return (char)value;
        }

        public static string UrlDecode(byte[] bytes, int offset, int count, Encoding e)
        {
            if (bytes == null || count == 0)
                return null;

            if (bytes == null)
                throw new ArgumentNullException("bytes");

            if (offset < 0 || offset > bytes.Length)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || offset + count > bytes.Length)
                throw new ArgumentOutOfRangeException("count");

            StringBuilder output = new StringBuilder();
            MemoryStream acc = new MemoryStream();

            int end = count + offset;
            for (int i = offset; i < end; i++)
            {
                if (bytes[i] == '%' && i + 2 < count)
                {
                    if (bytes[i + 1] == (byte)'u' && i + 5 < end)
                    {
                        if (acc.Length > 0)
                        {
                            output.Append(GetChars(acc, e));
                            acc.SetLength(0);
                        }
                        output.Append(GetChar(bytes, offset + 2, 4));
                        i += 5;
                    }
                    else
                    {
                        acc.WriteByte((byte)GetChar(bytes, offset + 1, 2));
                        i += 2;
                    }
                    continue;
                }

                if (acc.Length > 0)
                {
                    output.Append(GetChars(acc, e));
                    acc.SetLength(0);
                }

                if (bytes[i] == '+')
                {
                    output.Append(' ');
                }
                else
                {
                    output.Append((char)bytes[i]);
                }
            }

            if (acc.Length > 0)
            {
                output.Append(GetChars(acc, e));
            }

            acc = null;
            return output.ToString();
        }

        public static byte[] UrlDecodeToBytes(byte[] bytes)
        {
            if (bytes == null)
                return null;

            return UrlDecodeToBytes(bytes, 0, bytes.Length);
        }

        public static byte[] UrlDecodeToBytes(string str)
        {
            return UrlDecodeToBytes(str, Encoding.UTF8);
        }

        public static byte[] UrlDecodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return null;

            if (e == null)
                throw new ArgumentNullException("e");

            return UrlDecodeToBytes(e.GetBytes(str));
        }

        public static byte[] UrlDecodeToBytes(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return null;

            int len = bytes.Length;
            if (offset < 0 || offset >= len)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || offset <= len - count)
                throw new ArgumentOutOfRangeException("count");

            ArrayList result = new ArrayList();
            int end = offset + count;
            for (int i = offset; i < end; i++)
            {
                char c = (char)bytes[i];
                if (c == '+')
                    c = ' ';
                else if (c == '%' && i < end - 2)
                {
                    c = GetChar(bytes, i, 2);
                    i += 2;
                }
                result.Add((byte)c);
            }

            return (byte[])result.ToArray(typeof(byte));
        }

        public static string UrlEncode(string str)
        {
            return UrlEncode(str, Encoding.UTF8);
        }

        public static string UrlEncode(string s, Encoding Enc)
        {
            if (s == null)
                return null;

            if (s == "")
                return "";

            byte[] bytes = Enc.GetBytes(s);
            return Encoding.UTF8.GetString(UrlEncodeToBytes(bytes, 0, bytes.Length), 0, bytes.Length);
        }

        public static string UrlEncode(byte[] bytes)
        {
            if (bytes == null)
                return null;

            if (bytes.Length == 0)
                return "";

            return Encoding.UTF8.GetString(UrlEncodeToBytes(bytes, 0, bytes.Length), 0, bytes.Length);
        }

        public static string UrlEncode(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return null;

            if (bytes.Length == 0)
                return "";

            return Encoding.UTF8.GetString(UrlEncodeToBytes(bytes, offset, count), 0, bytes.Length);
        }

        public static byte[] UrlEncodeToBytes(string str)
        {
            return UrlEncodeToBytes(str, Encoding.UTF8);
        }

        public static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return null;

            if (str == "")
                return new byte[0];

            byte[] bytes = e.GetBytes(str);
            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        public static byte[] UrlEncodeToBytes(byte[] bytes)
        {
            if (bytes == null)
                return null;

            if (bytes.Length == 0)
                return new byte[0];

            return UrlEncodeToBytes(bytes, 0, bytes.Length);
        }

        static char[] hexChars = "0123456789ABCDEF".ToCharArray();

        public static byte[] UrlEncodeToBytes(byte[] bytes, int offset, int count)
        {
            if (bytes == null)
                return null;

            int len = bytes.Length;
            if (len == 0)
                return new byte[0];

            if (offset < 0 || offset >= len)
                throw new ArgumentOutOfRangeException("offset");

            if (count < 0 || offset < len - count)
                throw new ArgumentOutOfRangeException("count");

            ArrayList result = new ArrayList();
            int end = offset + count;
            for (int i = offset; i < end; i++)
            {
                char c = (char)bytes[i];
                if (c == ' ')
                    result.Add((byte)'+');
                else if ((c < '0' && c != '-' && c != '.') ||
                    (c < 'A' && c > '9') ||
                    (c > 'Z' && c < 'a' && c != '_') ||
                    (c > 'z'))
                {
                    result.Add((byte)'%');
                    int idx = ((int)c) >> 4;
                    result.Add((byte)hexChars[idx]);
                    idx = ((int)c) & 0x0F;
                    result.Add((byte)hexChars[idx]);
                }
                else
                {
                    result.Add((byte)c);
                }
            }

            return (byte[])result.ToArray(typeof(byte));
        }

        public static string UrlEncodeUnicode(string str)
        {
            if (str == null)
                return null;

            StringBuilder result = new StringBuilder();
            int end = str.Length;
            for (int i = 0; i < end; i++)
            {
                int idx;
                char c = str[i];
                if (c == ' ')
                {
                    result.Append('+');
                    continue;
                }

                if (c > 255)
                {
                    result.Append("%u");
                    idx = ((int)c) >> 24;
                    result.Append(hexChars[idx]);
                    idx = (((int)c) >> 16) & 0x0F;
                    result.Append(hexChars[idx]);
                    idx = (((int)c) >> 8) & 0x0F;
                    result.Append(hexChars[idx]);
                    idx = ((int)c) & 0x0F;
                    result.Append(hexChars[idx]);
                    continue;
                }

                if ((c < '0' && c != '-' && c != '.') ||
                    (c < 'A' && c > '9') ||
                    (c > 'Z' && c < 'a' && c != '_') ||
                    (c > 'z'))
                {
                    result.Append('%');
                    idx = ((int)c) >> 4;
                    result.Append(hexChars[idx]);
                    idx = ((int)c) & 0x0F;
                    result.Append(hexChars[idx]);
                    continue;
                }

                result.Append(c);
            }

            return result.ToString();
        }

        public static byte[] UrlEncodeUnicodeToBytes(string str)
        {
            if (str == null)
                return null;

            if (str == "")
                return new byte[0];

            return Encoding.UTF8.GetBytes(UrlEncodeUnicode(str));
        }

        /// <summary>
        /// Decodes an HTML-encoded string and returns the decoded string.
        /// </summary>
        /// <param _Name="s">The HTML string to decode. </param>
        /// <returns>The decoded _Text.</returns>
        public static string HtmlDecode(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            bool insideEntity = false; // used to indicate that we are in a potential entity
            string entity = String.Empty;
            StringBuilder output = new StringBuilder();

            foreach (char c in s)
            {
                switch (c)
                {
                    case '&':
                        output.Append(entity);
                        entity = "&";
                        insideEntity = true;
                        break;
                    case ';':
                        if (!insideEntity)
                        {
                            output.Append(c);
                            break;
                        }

                        entity += c;
                        int length = entity.Length;
                        if (length >= 2 && entity[1] == '#' && entity[2] != ';')
                        {
                            entity = ((char)Int32.Parse(entity.Substring(2, entity.Length - 3))).ToString();
                        }
                        else if (length > 1 && _Entities.ContainsKey(entity.Substring(1, entity.Length - 2)))
                        {
                            entity = _Entities[entity.Substring(1, entity.Length - 2)].ToString();
                        }
                        output.Append(entity);
                        entity = String.Empty;
                        insideEntity = false;
                        break;
                    default:
                        if (insideEntity)
                            entity += c;
                        else
                            output.Append(c);
                        break;
                }
            }
            output.Append(entity);
            return output.ToString();
        }

        /// <summary>
        /// Decodes an HTML-encoded string and sends the resulting output to a TextWriter output stream.
        /// </summary>
        /// <param _Name="s">The HTML string to decode</param>
        /// <param _Name="output">The TextWriter output stream containing the decoded string. </param>
        public static void HtmlDecode(string s, TextWriter output)
        {
            output.Write(HtmlDecode(s));
        }

        /// <summary>
        /// HTML-encodes a string and returns the encoded string.
        /// </summary>
        /// <param _Name="s">The _Text string to encode. </param>
        /// <returns>The HTML-encoded _Text.</returns>
        public static string HtmlEncode(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            StringBuilder output = new StringBuilder();

            foreach (char c in s)
                switch (c)
                {
                    case '&':
                        output.Append("&");
                        break;
                    case '>':
                        output.Append(">");
                        break;
                    case '<':
                        output.Append("<");
                        break;
                    case '"':
                        output.Append("\"");
                        break;
                    default:
                        if ((int)c > 128)
                        {
                            output.Append("&#");
                            output.Append(((int)c).ToString());
                            output.Append(";");
                        }
                        else
                            output.Append(c);
                        break;
                }
            return output.ToString();
        }

        /// <summary>
        /// HTML-encodes a string and sends the resulting output to a TextWriter output stream.
        /// </summary>
        /// <param _Name="s">The string to encode. </param>
        /// <param _Name="output">The TextWriter output stream containing the encoded string. </param>
        public static void HtmlEncode(string s, TextWriter output)
        {
            output.Write(HtmlEncode(s));
        }

        #endregion // Methods
    }
}