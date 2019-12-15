using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TerraViewer.Healpix
{
    public class Hploc
    {
        public double z, phi, sth;
        public bool have_sth;

        /** Default constructor. */
        public Hploc() { }
        public Hploc(Vector3d v)
        {
            double xl = 1/ v.Length();
            z = v.Z * xl;
            phi = FastMath.atan2(v.Y, v.X);
            if (Math.Abs(z) > 0.99)
            {
                sth = Math.Sqrt(v.X * v.X + v.Y * v.Y) * xl;
                have_sth = true;
            }
        }
        public Hploc(Zphi zphi)
        {
            z = zphi.z;
            phi = zphi.phi;
            have_sth = false;
        }
        public Hploc(Pointing ptg) 
        {
                    HealpixUtils.check((ptg.theta>=0d)&&(ptg.theta<=Math.PI),
              "invalid theta value");
            z = FastMath.cos(ptg.theta);
            phi = ptg.phi;
            if (Math.Abs(z)>0.99)
              {
              sth = FastMath.sin(ptg.theta);
              have_sth=true;
      }
}

        public Zphi toZphi()
        { return new Zphi(z, phi); }

        public Pointing toPointing()
        {
            double st = have_sth ? sth : Math.Sqrt((1.0 - z) * (1.0 + z));
            return new Pointing(FastMath.atan2(st, z), phi);
        }

        public Vector3d toVec3()
        {
            double st = have_sth ? sth : Math.Sqrt((1.0 - z) * (1.0 + z));
            double x = st * FastMath.cos(phi);
            double y = st * FastMath.sin(phi);
            return new Vector3d(-x, z, -y);
            //return new Vector3d(x, z, y); //for planet

            //switch (Earth3d.DebugValue)
            //{
            //    case 1: return new Vector3d(x, z, y);
            //    case 2: return new Vector3d(x, z, -y);
            //    case 3: return new Vector3d(-x, z, -y);
            //    case 4: return new Vector3d(-x, z, y);
            //    case 5: return new Vector3d(x, -z, -y);
            //    case 6: return new Vector3d(-x, z, -y);
            //    default: return new Vector3d(x, z, y);

            //}
            //Reversed the Z and Y axes

        }
    }
}