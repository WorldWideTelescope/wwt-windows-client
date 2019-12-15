using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TerraViewer.Healpix
{
    public class HealpixUtils
    {
        static public void check(bool cond, String errtxt) 
        { if (!cond) throw new Exception(errtxt);
    }

        /** Integer base 2 logarithm.
         */
        static public int ilog2(long arg)
        {
            int res = (int)Math.Log(arg, 2);
            
            return res;
        }

        /** Integer square root.
         */
        static public int isqrt(long arg)
    {
        long res = (long)Math.Sqrt(((double)arg) + 0.5);
        if (arg < (1L << 50)) return (int)res;
        if (res * res > arg)
            --res;
        else if ((res + 1) * (res + 1) <= arg)
            ++res;
        return (int)res;
    }

    /** Computes the cosine of the angular distance between two z, phi positions
        on the unit sphere. */
    static public double cosdist_zphi(double z1, double phi1,
      double z2, double phi2)
    {
        return z1 * z2 + FastMath.cos(phi1 - phi2) * Math.Sqrt((1.0 - z1 * z1) * (1.0 - z2 * z2));
    }
    /** Computes the cosine of the angular distance between two z, phi positions
        on the unit sphere. */
    static public double cosdist_zphi(Zphi zp1, Zphi zp2)
    { return cosdist_zphi(zp1.z, zp1.phi, zp2.z, zp2.phi); }

   
    static public double fmodulo(double v1, double v2)
    {
        if (v1 >= 0)
            return (v1 < v2) ? v1 : v1 % v2;
        double tmp = v1 % v2 + v2;
        return (tmp == v2) ? 0d : tmp;
    }

    static public bool approx(float a, float b, float epsilon)
    { return Math.Abs(a - b) < (epsilon * Math.Abs(b)); }
    static public bool approx(double a, double b, double epsilon)
    { return Math.Abs(a - b) < (epsilon * Math.Abs(b)); }
}
}