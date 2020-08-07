
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TerraViewer.Healpix
{
    public class Fxyf: HealpixTables
    {

        /** x-coordinate within the basis pixel, range [0.0;1.0] */
        public double fx;
        /** y-coordinate within the basis pixel, range [0.0;1.0] */
        public double fy;
        /** index of the HEALPix basis pixel, range [0;11] */
        public int face;
        public Fxyf(double x, double y, int f)
        { fx = x; fy = y; face = f; }

        protected Fxyf(Hploc loc)
        {
            double z = loc.z, phi = loc.phi;

            double za = Math.Abs(z);
            double tt = HealpixUtils.fmodulo((phi * Constants.inv_halfpi), 4.0);// in [0,4)

            if (za <= Constants.twothird) // Equatorial region
            {
                double temp1 = 0.5 + tt;
                double temp2 = z * 0.75;
                double jp = temp1 - temp2; // index of  ascending edge line
                double jm = temp1 + temp2; // index of descending edge line
                long ifp = (long)jp;  // in {0,4}
                long ifm = (long)jm;
                long face_num = (ifp == ifm) ? (ifp | 4) : ((ifp < ifm) ? ifp : (ifm + 8));
                fx = HealpixUtils.fmodulo(jm, 1d);
                fy = 1d- HealpixUtils.fmodulo(jp, 1d);
                face = (int)face_num;
            }
            else // polar region, za > 2/3
            {
                int ntt = Math.Min(3, (int)tt);
                double tp = tt - ntt;
                double tmp = ((za < 0.99) || (!loc.have_sth)) ?
                              Math.Sqrt(3 * (1 - za)) :
                              loc.sth / Math.Sqrt((1d+ za) / 3d);

                double jp = tp * tmp; // increasing edge line index
                double jm = (1.0 - tp) * tmp; // decreasing edge line index
                if (jp >= 1d) jp = 1d; // for points too close to the boundary
                if (jm >= 1d) jm = 1d;
                if (z >= 0)
                { fx = 1d- jm; fy = 1d- jp; face = ntt; }
                else
                { fx = jp; fy = jm; face = ntt + 8; }
            }
        }

        public Fxyf(Vector3d v):this(new Hploc(v))
        { }

        protected Hploc toHploc()
        {
            Hploc loc = new Hploc();
            double jr = jrll[face] - fx - fy;

            double nr; double tmp;
            if (jr < 1)
            {
                nr = jr;
                tmp = nr * nr / 3d;
                loc.z = 1 - tmp;
                if (loc.z > 0.99) { loc.sth = Math.Sqrt(tmp * (2d- tmp)); loc.have_sth = true; }
            }
            else if (jr > 3)
            {
                nr = 4 - jr;
                tmp = nr * nr / 3d;
                loc.z = tmp - 1;
                if (loc.z < -0.99) { loc.sth = Math.Sqrt(tmp * (2d- tmp)); loc.have_sth = true; }
            }
            else
            {
                nr = 1;
                loc.z = (2 - jr) * 2d/ 3d;
            }

            tmp = jpll[face] * nr + fx - fy;
            if (tmp < 0) tmp += 8;
            if (tmp >= 8) tmp -= 8;
            loc.phi = (nr < 1e-15) ? 0 : (0.5 * Constants.halfpi * tmp) / nr;
            return loc;
        }
        public Vector3d toVec3()
        { return toHploc().toVec3(); }
    }
}