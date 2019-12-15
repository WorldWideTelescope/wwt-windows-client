using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TerraViewer.Healpix
{
    public class Zphi
    {
        /** Cosine of the colatitude, or z component of unit vector; Range [-1;1]. */
        public double z;

        /** Longitude in radians; Range [0; 2Pi]. */
        public double phi;


        public Zphi() { }

        public Zphi(double z_, double phi_)
        { z = z_; phi = phi_; }
        
        public Zphi(Vector3d v)
        { z = v.Z / v.Length(); phi = FastMath.atan2(v.Y, v.X); }

    
        public Zphi(Pointing ptg)
        { z = FastMath.cos(ptg.theta); phi = ptg.phi; }

        public String toString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("zphi("); s.Append(z);
            s.Append(","); s.Append(phi);
            s.Append(")");
            return s.ToString();
        }


    }
}