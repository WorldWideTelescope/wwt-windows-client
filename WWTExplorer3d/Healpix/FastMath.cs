using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TerraViewer.Healpix
{
    /**
     * Code from Healpix Java package
     */
    public class FastMath
    {
        private static  double mulsign(double x, double y)
        { return  Math.Sign(y) * x; }

        /** Checks if the argument is a NaN or not. */
        private static  bool isnan(double d)
        { return d != d; }

        /** Checks if the argument is either positive or negative infinity. */
        private static bool isinf(double d)
        { return Math.Abs(d) == Double.PositiveInfinity; }

        private static  double sign(double d) { return Math.Sign(d); }

        private static  double atanhelper(double s)
        {
            double t = s * s;
            double u = -1.88796008463073496563746e-05;
            u = u * t + (0.000209850076645816976906797);
            u = u * t + (-0.00110611831486672482563471);
            u = u * t + (0.00370026744188713119232403);
            u = u * t + (-0.00889896195887655491740809);
            u = u * t + (0.016599329773529201970117);
            u = u * t + (-0.0254517624932312641616861);
            u = u * t + (0.0337852580001353069993897);
            u = u * t + (-0.0407629191276836500001934);
            u = u * t + (0.0466667150077840625632675);
            u = u * t + (-0.0523674852303482457616113);
            u = u * t + (0.0587666392926673580854313);
            u = u * t + (-0.0666573579361080525984562);
            u = u * t + (0.0769219538311769618355029);
            u = u * t + (-0.090908995008245008229153);
            u = u * t + (0.111111105648261418443745);
            u = u * t + (-0.14285714266771329383765);
            u = u * t + (0.199999999996591265594148);
            u = u * t + (-0.333333333333311110369124);

            return u * t * s + s;
        }

        private static  double atan2k(double y, double x)
        {
            double q = 0;

            if (x < 0) { x = -x; q = -2; }
            if (y > x) { double t = x; x = y; y = -t; q += 1; }

            return atanhelper(y / x) + q * (Math.PI / 2);
        }

        /** This method calculates the arc tangent of y/x in radians, using
            the signs of the two arguments to determine the quadrant of the
            result. The results may have maximum error of 2 ulps. */
        public static  double atan2(double y, double x)
        {
            double r = atan2k(Math.Abs(y), x);

            r = mulsign(r, x);
            if (isinf(x) || x == 0)
                r = Math.PI / 2 - (isinf(x) ? (sign(x) * (Math.PI / 2)) : 0);
            if (isinf(y))
                r = Math.PI / 2 - (isinf(x) ? (sign(x) * (Math.PI * 1 / 4)) : 0);
            if (y == 0)
                r = (sign(x) == -1 ? Math.PI : 0);
            return isnan(x) || isnan(y) ? Double.NaN : mulsign(r, y);
        }

        /** This method calculates the arc sine of x in radians. The return
            value is in the range [-pi/2, pi/2]. The results may have
            maximum error of 3 ulps. */
        public static double asin(double d)
        { return mulsign(atan2k(Math.Abs(d), Math.Sqrt((1 + d) * (1 - d))), d); }

        /** This method calculates the arc cosine of x in radians. The
            return value is in the range [0, pi]. The results may have
            maximum error of 3 ulps. */
        public static double acos(double d)
        {
            return mulsign(atan2k(Math.Sqrt((1 + d) * (1 - d)), Math.Abs(d)), d)
              + (d < 0 ? Math.PI : 0);
        }

        /** Returns the arc tangent of an angle. The results may have
            maximum error of 2 ulps. */
        public static  double atan(double s)
        {
            int q = 0;
            if (s < 0) { s = -s; q = 2; }
            if (s > 1) { s = 1.0 / s; q |= 1; }

            double t = atanhelper(s);

            if ((q & 1) != 0) t = 1.570796326794896557998982 - t;
            if ((q & 2) != 0) t = -t;

            return t;
        }

        private static readonly double PI4_A = .7853981554508209228515625;
        private static readonly double PI4_B
          = .794662735614792836713604629039764404296875e-8;
        private static readonly double PI4_C
          = .306161699786838294306516483068750264552437361480769e-16;
        private static readonly double M_1_PI = 0.3183098861837906715377675267450287;

        private  static double sincoshelper(double d)
        {
            double s = d * d;
            double u = -7.97255955009037868891952e-18;
            u = u * s + 2.81009972710863200091251e-15;
            u = u * s - 7.64712219118158833288484e-13;
            u = u * s + 1.60590430605664501629054e-10;
            u = u * s - 2.50521083763502045810755e-08;
            u = u * s + 2.75573192239198747630416e-06;
            u = u * s - 0.000198412698412696162806809;
            u = u * s + 0.00833333333333332974823815;
            u = u * s - 0.166666666666666657414808;
            return s * u * d + d;
        }

        /** Returns the trigonometric sine of an angle. The results may
            have maximum error of 2 ulps. */
        public static  double sin(double d)
        {
            double u = d * M_1_PI;
            long q = (long)(u < 0 ? u - 0.5 : u + 0.5);

            double x = 4d* q;
            d -= x * PI4_A;
            d -= x * PI4_B;
            d -= x * PI4_C;

            if ((q & 1) != 0) d = -d;

            return sincoshelper(d);
        }

        /** Returns the trigonometric cosine of an angle. The results may
            have maximum error of 2 ulps. */
        public static  double cos(double d)
        {
            double u = d * M_1_PI - 0.5;
            long q = 1 + 2 * (long)(u < 0 ? u - 0.5 : u + 0.5);

            double x = 2d* q;
            d -= x * PI4_A;
            d -= x * PI4_B;
            d -= x * PI4_C;

            if ((q & 2) == 0) d = -d;

            return sincoshelper(d);
        }
    }
}