using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TerraViewer.Healpix
{
   public  class Pointing
    {
        /** Colatitude in radians (0 is North Pole; Pi is South Pole) */
        public double theta;

        /** Longitude in radians */
        public double phi;

        /** Default constructor */
        public Pointing() { }

        public Pointing(Pointing ptg)
        { this.theta = ptg.theta; this.phi = ptg.phi; }

        /** Simple constructor initializing both values.
            @param theta in radians [0,Pi]
            @param phi in radians [0,2*Pi] */
        public Pointing(double theta, double phi)
        { this.theta = theta; this.phi = phi; }

        public Pointing(Vector3d vec)
        {
            theta = FastMath.atan2(Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y), vec.Z);
            phi = FastMath.atan2(vec.Y, vec.X);
            if (phi < 0d) phi += 2 * Math.PI;
            if (phi >= 2 * Math.PI) phi -= 2 * Math.PI;
        }


        public Pointing(Zphi zphi)
        {
            double xy = Math.Sqrt((1d- zphi.z) * (1d+ zphi.z));
            theta = FastMath.atan2(xy, zphi.z); phi = zphi.phi;
        }

        /** Normalize theta range */
        public void normalizeTheta()
        {
            theta = HealpixUtils.fmodulo(theta, 2 * Math.PI);
            if (theta > Math.PI)
            {
                phi += Math.PI;
                theta = 2 * Math.PI - theta;
            }
        }

        /** Normalize theta and phi ranges */
        public void normalize()
        {
            normalizeTheta();
            phi = HealpixUtils.fmodulo(phi, 2 * Math.PI);
        }

        public String toString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("ptg("); s.Append(theta);
            s.Append(","); s.Append(phi);
            s.Append(")");
            return s.ToString();
        }


    }
}