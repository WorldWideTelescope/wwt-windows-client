using System;
using System.Collections.Generic;
using System.Text;
using SharpDX;

namespace TerraViewer
{
    public struct Coordinates
    {
        const double RC = (Math.PI / 180.0);
        const double RCRA = (Math.PI / 12.0);
        const float radius = 1;
        static public Vector3 GeoTo3d(double lat, double lng)
        {
            return new Vector3((float)(Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (float)(Math.Sin(lat * RC) * radius), (float)(Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius));

        }

        static public Vector3d GeoTo3dDouble(double lat, double lng)
        {
            return new Vector3d(Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius, Math.Sin(lat * RC) * radius, Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius);

        }
        static public Vector3d GeoTo3dDouble(double lat, double lng, double radius)
        {
            return new Vector3d(Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius, Math.Sin(lat * RC) * radius, Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius);

        }
        static public Vector3 GeoTo3d(double lat, double lng, double radius)
        {
            return new Vector3((float)((Math.Cos(lng * RC)) * (Math.Cos(lat * RC)) * radius), (float)((Math.Sin(lat * RC) * radius)), (float)((Math.Sin(lng * RC)) * (Math.Cos(lat * RC)) * radius));

        }

        static public Vector3 GalacticTo3d(double l, double b)
        {
            double[] result = GalactictoJ2000(l, b);
            return RADecTo3d(result[0]/15+12, result[1]);
        }

        static public Vector3d GalacticTo3dDouble(double l, double b)
        {
            double[] result = GalactictoJ2000(l, b);
            return RADecTo3d(result[0]/15+12, result[1], 1);
        }

        static public Vector3 RADecTo3d(double ra, double dec)
        {
            return new Vector3((float)(Math.Cos(ra * RCRA) * Math.Cos(dec * RC) * radius), (float)(Math.Sin(dec * RC) * radius), (float)(Math.Sin(ra * RCRA) * Math.Cos(dec * RC) * radius));

        }

        static public Vector3d RADecTo3d(double ra, double dec, double au)
        {
            return new Vector3d((Math.Cos(ra * RCRA) * Math.Cos(dec * RC) * au), (Math.Sin(dec * RC) * au), (Math.Sin(ra * RCRA) * Math.Cos(dec * RC) * au));

        }

        static public Vector3 RADecTo3d(double ra, double dec, Matrix mat)
        {
            return Vector3.TransformCoordinate(new Vector3((float)(Math.Cos(ra * RCRA) * Math.Cos(dec * RC) * radius), (float)(Math.Sin(dec * RC) * radius), (float)(Math.Sin(ra * RCRA) * Math.Cos(dec * RC) * radius)),mat);
        }

        static public Vector3 RADecTo3d(Coordinates point, double radius)
        {
            point.Dec = -point.Dec;
            return new Vector3((float)(Math.Cos(point.RA * RCRA) * Math.Cos(point.Dec * RC) * radius), (float)(Math.Sin(point.Dec * RC) * radius), (float)(Math.Sin(point.RA * RCRA) * Math.Cos(point.Dec * RC) * radius));

        }

        static public Vector3d RADecTo3dDouble(Coordinates point, double radius)
        {
            point.Dec = -point.Dec;
            return new Vector3d((Math.Cos(point.RA * RCRA) * Math.Cos(point.Dec * RC) * radius), (Math.Sin(point.Dec * RC) * radius), (Math.Sin(point.RA * RCRA) * Math.Cos(point.Dec * RC) * radius));

        }

        const double EarthRadius = 6371000;
        static public Vector3d SterographicTo3d(double x, double y, double radius, double standardLat, double meridean, double falseEasting, double falseNorthing, double scale, bool north )
        {
            double lat=90;
            double lng=0;

            x -= falseEasting;
            y -= falseNorthing;

           

            if (x != 0 || y != 0)
            {
                double re = (1 + Math.Sin(Math.Abs(standardLat) / 180 * Math.PI)) * EarthRadius / scale;
                double rere = re * re;
                double c1 = 180 / Math.PI;

                if (x == 0)
                {
                    lng = 90 * Math.Sign(y);
                }
                else
                {
                    lng = Math.Atan2(y , x) * c1;
                }

                //if (x < 0)
                //{
                //    lng = lng + (180 * Math.Sign(y));
                //}

                //if (lng > 180)
                //{
                //    lng -= 360;
                //}

                //if (lng < -180)
                //{
                //    lng += 360;
                //}

                double len = (x * x) + (y * y);
                lat = (rere - len) / (rere + len);
                lat = Math.Asin(lat) * c1;

                if (!north)
                {
                    lat = -lat;
                    lng = -lng;
                    meridean = -meridean;
                }
            }
            return Coordinates.GeoTo3dDouble(lat, 90 + lng + meridean, radius);
        }

        static public Vector2d RaDecToTan(Coordinates center, Coordinates point)
        {
            return RaDecToTan(new Vector2d(center.RA, center.Dec), new Vector2d(point.RA, point.Dec));
        }

        static public Vector2d RaDecToTan(Vector2d center, Vector2d point)
        {
            double lambda = point.X / 12 * Math.PI;
            double phi = point.Y / 180 * Math.PI;
            double lcenter = center.X / 12 * Math.PI;
            double pCenter = center.Y / 180 * Math.PI;


            double cosc = Math.Sin(pCenter) * Math.Sin(phi) + Math.Cos(pCenter) * Math.Cos(phi) * Math.Cos(lambda - lcenter);

            double x = Math.Cos(phi) * Math.Sin(lambda - lcenter) / cosc;
            double y = (Math.Cos(pCenter) * Math.Sin(phi) - Math.Sin(pCenter) * Math.Cos(phi) * Math.Cos(lambda - lcenter)) / cosc;

            return new Vector2d(x, y);
        }

        static public Vector2d TanToRaDec(Coordinates center, Vector2d point)
        {
            return TanToRaDec(new Vector2d(center.RA, center.Dec), point);
        }

        static public Vector2d TanToRaDec(Vector2d center, Vector2d point)
        {
            double x = point.X;
            double y = point.Y;
            double lcenter = center.X / 12 * Math.PI;
            double pCenter = center.Y / 180 * Math.PI;

            double p = Math.Sqrt(x * x + y * y);
            double c = Math.Atan(p);

            double phi = Math.Asin(Math.Cos(c) * Math.Sin(pCenter) + y * Math.Sin(c) * Math.Cos(pCenter) / p);

            double lambda = lcenter + Math.Atan2(x * Math.Sin(c), (p * Math.Cos(pCenter) * Math.Cos(c) - y * Math.Sin(pCenter) * Math.Sin(c)));

            return new Vector2d(lambda / Math.PI * 12, phi / Math.PI * 180);
        }


        static public Coordinates EquitorialToHorizon4(Coordinates equitorial, Coordinates location, DateTime utc)
        {
 
            double lon = location.Lng;



  
            double hour = utc.Hour + utc.Minute / 60.00 + utc.Second / 3600.0;


            double day = utc.Day + hour / 24.0;

            double fullDays = Math.Floor(day);

            int month = utc.Month;
            int year = utc.Year;
            if (month < 3)
            {
                year--;
                month += 12;
            }


            double gr;
            if (year + month / 100 + fullDays / 10000 >= 1582.1015)
            {
                gr = 2 - Math.Floor(year / 100.0) + Math.Floor(Math.Floor(year / 100.0) / 4);
            }
            else
            {
                gr = 0;
            }


            double julianDay = Math.Floor(365.25 * year) + Math.Floor(30.6001 * (month + 1)) + fullDays + 1720994.5 + gr;

            double julianDay2 = julianDay + hour / 24;
            double t = (julianDay - 2415020) / 36525;
            double ss = 6.6460656 + 2400.051 * t + 0.00002581 * t * t;
            double st = (ss / 24 - Math.Floor(ss / 24)) * 24;
            double gsth = Math.Floor(st);
            double gstm = Math.Floor((st - Math.Floor(st)) * 60);
            double gsts = ((st - Math.Floor(st)) * 60 - gstm) * 60;

            double sa = st + (day - Math.Floor(day)) * 24 * 1.002737908;

            sa = sa + (lon/15);

            if (sa < 0)
            {
                sa +=24;
            }

            if (sa > 24)
            {
                sa-=24;
            }
            double tsh = Math.Floor(sa);
            double tsm = Math.Floor((sa - Math.Floor(sa)) * 60);
            double tss = ((sa - Math.Floor(sa)) * 60 - tsm) * 60;

            return new Coordinates(0,0);

        }

        public double Distance(Coordinates pointB)
        {
            double y = this.Lat;
            double x = this.Lng * Math.Cos(y * RC);
            double y1 = pointB.Lat;
            double x1 = pointB.Lng * Math.Cos(y1 * RC);
            return Math.Sqrt((y - y1) * (y - y1) + (x - x1) * (x - x1));
        }

        public double Distance3d(Coordinates pointB)
        {
            Vector3d pnt1 = Coordinates.GeoTo3dDouble(pointB.Lat, pointB.Lng);
            Vector3d pnt2 = Coordinates.GeoTo3dDouble(this.Lat, this.Lng);

            Vector3d pntDiff = pnt1 - pnt2;

            return pntDiff.Length() / RC;
        }

        public double Angle(Coordinates pointB)
        {
            double y = this.Lat;
            double x = this.Lng * Math.Cos(y * RC);
            double y1 = pointB.Lat;
            double x1 = pointB.Lng * Math.Cos(y1 * RC);
            return Math.Atan2((y1 - y), (x1 - x));
        }

        static public Coordinates EquitorialToHorizon(Coordinates equitorial, Coordinates location, DateTime utc)
        {
            double hourAngle = MstFromUTC2(utc, location.Lng) - (equitorial.RA * 15);

            if (hourAngle < 0)
            {
                hourAngle += 360.00;
            }

            double ha = hourAngle * RC;
            double dec = equitorial.Dec * RC;
            double lat = (location.Lat) * RC;

            double sinAlt = Math.Sin(dec) * Math.Sin(lat) + Math.Cos(dec) * Math.Cos(lat) * Math.Cos(ha);

            double altitude = Math.Asin(sinAlt);

            double cosAzimith = (Math.Sin(dec) - Math.Sin(altitude)*Math.Sin(lat))/(Math.Cos(altitude)*Math.Cos(lat));
            double azimuth = Math.Acos(cosAzimith);



            Coordinates altAz = new Coordinates(azimuth,altitude);
            if (Math.Sin(ha) > 0)
            {
                altAz.Az = (360 - altAz.Az);
            }
            return altAz;
        }

        static public Coordinates HorizonToEquitorial(Coordinates altAz, Coordinates location, DateTime utc)
        {
            double hourAngle = MstFromUTC2(utc, location.Lng);// -(equitorial.RA * 15);

            double haLocal;
            double declination;
            AltAzToRaDec(altAz.Alt * RC, altAz.Az * RC, out haLocal, out declination, location.Lat * RC);

            double ha = (haLocal / RC);

            hourAngle += ha;

            if (hourAngle < 0)
            {
                hourAngle += 360.00;
            }
            if (hourAngle > 360)
            {
                hourAngle -= 360;
            }

            return Coordinates.FromRaDec(hourAngle / 15, declination / RC);
        }
        static void AltAzToRaDec(double Altitude, double Azimuth, out double hrAngle, out double dec, double Latitude)
        {
            Azimuth = Math.PI - Azimuth;
            if (Azimuth < 0)
            {
                Azimuth += Math.PI * 2;
            }
            hrAngle = Math.Atan2(Math.Sin(Azimuth), Math.Cos(Azimuth) * Math.Sin(Latitude) + Math.Tan(Altitude) * Math.Cos(Latitude));

            if (hrAngle < 0)
            {
                hrAngle += Math.PI*2;
            }
            dec = Math.Asin(Math.Sin(Latitude) * Math.Sin(Altitude) - Math.Cos(Latitude) * Math.Cos(Altitude) * Math.Cos(Azimuth));
        }
      
        static void AltAzToRaDec2(double alt, double az, out double hrAngle, out double dec, double lat)
        {
            if (alt == 0)
            {
                alt = .00000000001;
            }
            if (az == 0)
            {
                az = .00000000001;
            } 
            double sin_dec;
            double cos_lat = Math.Cos(lat);

            if (alt > Math.PI / 2.0)
            {
                alt = Math.PI - alt;
                az += Math.PI;
            }
            if (alt < -Math.PI / 2.0)
            {
                alt = -Math.PI - alt;
                az -= Math.PI;
            }

            sin_dec = Math.Sin(lat) * Math.Sin(alt) + cos_lat * Math.Cos(alt) * Math.Cos(az);
            dec = Math.Asin(sin_dec);

            if (cos_lat < .00001)
            {
                hrAngle = az + Math.PI;
            }
            else
            {
                double cos_lat_cos_dec = (cos_lat * Math.Cos(dec));
                double sin_alt_sinLat_sin_dec = Math.Sin(alt) - Math.Sin(lat) * sin_dec;

                double acosTarget = sin_alt_sinLat_sin_dec / cos_lat_cos_dec;
                double temp = 0;
                if (Math.Abs(acosTarget) < 1.1)
                {
                    if (acosTarget > 1)
                    {
                        acosTarget = 1.0;
                    }
                    if (acosTarget < -1)
                    {
                        acosTarget = -1.0;
                    }
                    temp = Math.Acos(acosTarget);
                }
                else
                {
                    temp = Math.PI;
                }
                if (double.IsNaN(temp))
                {
                    temp = Math.PI;
                }

                if (Math.Sin(az) > 0.0)
                {
                    hrAngle = Math.PI - temp;
                }
                else
                {
                    hrAngle = Math.PI + temp;
                }
            }
        }

        public static double MstFromUTC2(DateTime utc, double lng)
        {

            int year = utc.Year;
            int month = utc.Month;
            int day = utc.Day;
            int hour = utc.Hour;
            int minute = utc.Minute;
            double second = utc.Second + utc.Millisecond/1000.0;

            if (month == 1 || month == 2)
            {
                year -= 1;
                month += 12;
            }


            //int a = (int)(year / 100);
            //int b = (int)(a / 4.0);
            //int c = 2 - a + b;
            //int e = (int)(365.25 * (year + 4716));
            //int f = (int)(30.6001 * (month + 1));

            //double jd = c+day+e+f-1524.5;


            int a = (int)(year / 100);
            int b = 2 - a + (int)Math.Floor((double)(a / 4.0));
            int c = (int)Math.Floor(365.25 * year);
            int d = (int)Math.Floor(30.6001 * (month + 1));

            double julianDays;
            double jd2;
            double julianCenturies;
            double mst;

            julianDays = b + c + d - 730550.5 + day + (hour + minute / 60.00 + second / 3600.00) / 24.00;

            julianCenturies = julianDays / 36525.0d;
            mst = 280.46061837 + 360.98564736629d * julianDays + 0.000387933d * julianCenturies * julianCenturies - julianCenturies * julianCenturies * julianCenturies / 38710000 + lng;

            return MapTo0To360Range(mst);
        }

        

        public double RA
        {
            get
            {
                return (((ascention / Math.PI) * 12) + 12) % 24;
            }
            set
            {
                ascention = ((value+12)/12)* Math.PI;
            }
        }


        public double Dec
        {
            get
            {
                return declination/RC;
            }
            set
            {
                declination = value*RC;
            }
        }

        public double Lat
        {
            get
            {
                
                return declination / RC;
            }
            set
            {
                declination = value * RC;
            }
        }

        public double Lng
        {
            get
            {
                if (ascention == double.NaN)
                {
                    ascention = 0;
                }

                double lng = ascention / RC;

                if (lng <= 180)
                {
                    return lng;
                }
                else
                {
                    return (-180 + (180 - lng));
                }

            }
            //todo This was broken check callers to see what effect it had.
            set 
            {
                ascention = ((value*RC)+(Math.PI*2)%(Math.PI*2));
            }
        }

        public double Alt
        {
            get
            {
                return declination / RC;
            }
            set
            {
                declination = value * RC;
            }
        }
        public double Az
        {
            get
            {
                return ascention / RC;

            }
            set
            {
                ascention = value * RC;
            }
        }

        // Held in radians
        double ascention;
        double declination;

        public Coordinates(double ascention, double declination)
        {
            this.ascention = ascention + (Math.PI * 80) % (Math.PI * 2);
            this.declination = declination;
        }

        public override string ToString()
        {
            return string.Format("Lat: {0}, Lng: {1}", Lat, Lng);
        }

        static public Coordinates CartesianToSpherical(Vector3d vector)
        {
            double ascention;
            double declination;

            double radius = Math.Sqrt((double)vector.X * (double)vector.X + (double)vector.Y * (double)vector.Y + (double)vector.Z * (double)vector.Z);
            double XZ = Math.Sqrt((double)vector.X * (double)vector.X + (double)vector.Z * (double)vector.Z);

            declination = Math.Asin((double)vector.Y / radius);

            if (0 < vector.X)
            {
                ascention = Math.Asin((double)vector.Z / XZ);
            }
            else if (0 > vector.X)
            {
                ascention = Math.PI - Math.Asin((double)vector.Z / XZ);
            }
            else
            {
                ascention = 0;
            } 
               
            
            return new Coordinates(ascention, declination);

        }


        static public Coordinates CartesianToSpherical(Vector3 vector)
        {
            double ascention;
            double declination;

            double radius = Math.Sqrt((double)vector.X * (double)vector.X + (double)vector.Y * (double)vector.Y + (double)vector.Z * (double)vector.Z);
            double XZ = Math.Sqrt((double)vector.X * (double)vector.X + (double)vector.Z * (double)vector.Z);

            declination = Math.Asin((double)vector.Y / radius);

            if (0 < vector.X)
            {
                ascention = Math.Asin((double)vector.Z / XZ);
            }
            else if (0 > vector.X)
            {
                ascention = Math.PI - Math.Asin((double)vector.Z / XZ);
            }
            else
            {
                ascention = 0;
            } 
               
            
            return new Coordinates(ascention, declination);

        }

        const double SemiMajorAxis = 6378137.0;


        const double FL = 1 / 298.257222101;


        static public Vector3d XyzToGeo(Vector3d point)
        {

            double latitude;

            double longitude;

            double altitude;

            double sEccentricity2 = FL * (2.0 - FL);

            double sinLat;     // cache for sin of latitude
            double cosLat;     // cache for cos of latitude
            double curvature;  // latitude dependent curvature
            double radiusXY = Math.Sqrt(point.X * point.X + point.Y * point.Y); // (invariant)
            double opp = point.Z;   // opposite length of latitude (invariant)
            double adj;        // adjacent length of latitude
            double hypInv;     // inverse hypotenuse length of latitude

            double lat, lon, alt;

            // compute longitude exactly
            lon = Math.Atan2(point.X, point.Y);

            // compute initial guess for latitude and altitude
            adj = radiusXY * (1.0 - FL);
            hypInv = 1.0 / Math.Sqrt(opp * opp + adj * adj);

            lat = Math.Atan2(opp, adj);

            sinLat = opp * hypInv; // sinLat = System.Math.Sin(latitude);
            cosLat = adj * hypInv;

            curvature = SemiMajorAxis; // initial sphere guess works best: curvature = sRadius / sqrt(1.0 - sEccentricity2 * sinLat * sinLat);
            if (radiusXY > 1.0)
            {
                alt = radiusXY / cosLat - curvature;
            }
            else if (point.Z > 0.0)
            {
                alt = point.Z - SemiMajorAxis * (1.0 - FL);
            }
            else
            {
                alt = -point.Z - SemiMajorAxis * (1.0 - FL);
            }

            // first iteration
            adj = radiusXY * (1.0 - sEccentricity2 * curvature / (curvature + alt));
            hypInv = 1.0 / Math.Sqrt(opp * opp + adj * adj);

            lat = Math.Atan2(opp, adj);

            sinLat = opp * hypInv; // sinLat = System.Math.Sin(latitude);
            cosLat = adj * hypInv;

            curvature = SemiMajorAxis / Math.Sqrt(1.0 - sEccentricity2 * sinLat * sinLat);

            if (radiusXY > 1.0)
            {
                alt = radiusXY / cosLat - curvature;
            }
            else if (point.Z > 0.0)
            {
                alt = point.Z - SemiMajorAxis * (1.0 - FL);
            }
            else
            {
                alt = -point.Z - SemiMajorAxis * (1.0 - FL);
            }

            // second iteration
            adj = radiusXY * (1.0 - sEccentricity2 * curvature / (curvature + alt));
            hypInv = 1.0 / Math.Sqrt(opp * opp + adj * adj);


            lat = Math.Atan2(opp, adj);

            sinLat = opp * hypInv; // sinLat = System.Math.Sin(latitude);
            cosLat = adj * hypInv;

            curvature = SemiMajorAxis / Math.Sqrt(1.0 - sEccentricity2 * sinLat * sinLat);


            if (radiusXY > 1.0)
            {
                alt = radiusXY / cosLat - curvature;
            }
            else if (point.Z > 0.0)
            {
                alt = point.Z - SemiMajorAxis * (1.0 - FL);
            }
            else
            {
                alt = -point.Z - SemiMajorAxis * (1.0 - FL);
            }

            // third iteration
            adj = radiusXY * (1.0 - sEccentricity2 * curvature / (curvature + alt));
            hypInv = 1.0 / Math.Sqrt(opp * opp + adj * adj);


            lat = Math.Atan2(opp, adj);


            sinLat = opp * hypInv; // sinLat = System.Math.Sin(latitude);
            cosLat = adj * hypInv;

            curvature = SemiMajorAxis / Math.Sqrt(1.0 - sEccentricity2 * sinLat * sinLat);


            if (radiusXY > 1.0)
            {
                alt = radiusXY / cosLat - curvature;
            }
            else if (point.Z > 0.0)
            {
                alt = point.Z - SemiMajorAxis * (1.0 - FL);
            }
            else
            {
                alt = -point.Z - SemiMajorAxis * (1.0 - FL);
            }

            latitude = RadiansToDegrees(lat);
            longitude = RadiansToDegrees(lon);
            altitude = alt;

            return GeoTo3dDouble(latitude, longitude, 1 + (altitude / EarthRadius));
        }


        static public Vector3d XyzToGeo2(Vector3d point)
        {
            double A = SemiMajorAxis;
            double B;
            double d;
            double e;
            double f;
            double g;
            double p;
            double q;
            double r;
            double t;
            double v;
            double x = point.X;
            double y = point.Y;
            double z = point.Z;
            double zlong;

            double lat;
            double lng;
            double alt;

            /*
             *   1.0 compute semi-minor axis and set sign to that of z in order
             *       to get sign of Phi correct
             */
            B = A * (1 - FL);
            if (z < 0)
                B = -B;
            /*
             *   2.0 compute intermediate values for latitude
             */
            r = Math.Sqrt(x * x + y * y);
            e = (B * z - (A * A - B * B)) / (A * r);
            f = (B * z + (A * A - B * B)) / (A * r);
            /*
             *   3.0 find solution to:
             *       t^4 + 2*E*t^3 + 2*F*t - 1 = 0
             */
            p = (4 / 3) * (e * f + 1);
            q = 2 * (e * e - f * f);
            d = p * p * p + q * q;

            if (d >= 0)
            {
                v = Math.Pow((Math.Sqrt(d) - q), (1 / 3))
                 - Math.Pow((Math.Sqrt(d) + q), (1 / 3));
            }
            else
            {
                v = 2 * Math.Sqrt(-p)
                 * Math.Cos(Math.Acos(q / (p * Math.Sqrt(-p))) / 3);
            }
            /*
             *   4.0 improve v
             *       NOTE: not really necessary unless point is near pole
             */
            if (v * v < Math.Abs(p))
            {
                v = -(v * v * v + 2 * q) / (3 * p);
            }
            g = (Math.Sqrt(e * e + v) + e) / 2;
            t = Math.Sqrt(g * g + (f - v * g) / (2 * g - e)) - g;

            lat = Math.Atan((A * (1 - t * t)) / (2 * B * t));
            /*
             *   5.0 compute height above ellipsoid
             */
            alt = (r - A * t) * Math.Cos(lat) + (z - B) * Math.Sin(lat);
            /*
             *   6.0 compute longitude east of Greenwich
             */
            zlong = Math.Atan2(y, x);
            if (zlong < 0)
            {
                zlong = zlong + (Math.PI * 2);
            }

            lng = zlong;
            /*
             *   7.0 convert latitude and longitude to degrees
             */

            lng = RadiansToDegrees(lng);
            lat = RadiansToDegrees(lat);

            return GeoTo3dDouble(lat, lng, 1 + (alt / EarthRadius));
        }


        static public Vector2d CartesianToLatLng(Vector3d vector)
        {
            double rho = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
            double longitude = Math.Atan2(vector.Z, vector.X);
            double latitude = Math.Asin(vector.Y / rho);

            return new Vector2d(longitude * 180 / Math.PI, latitude * 180 / Math.PI);

        }

        static public Coordinates CartesianToSpherical2(Vector3 vector)
        {
		    double rho = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
		    double longitude = Math.Atan2(vector.Z, vector.X);
		    double latitude = Math.Asin(vector.Y / rho);

            return new Coordinates(longitude, latitude);

        }


        static public Coordinates CartesianToSpherical2(Vector3d vector)
        {
		    double rho = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
		    double longitude = Math.Atan2(vector.Z, vector.X);
		    double latitude = Math.Asin(vector.Y / rho);

            return new Coordinates(longitude, latitude);

        }  

        static public Coordinates CartesianToSpherical3(Vector3d vector)
        {
		    double rho = Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
		    double longitude = Math.Atan2(vector.Z, vector.X);
		    double latitude = Math.Asin(vector.Y / rho);

            return new Coordinates(longitude, latitude);

        }  

      
        static public string FormatDMS(double angle, bool sign)
        {

            try
            {
                if (double.IsNaN(angle))
                {
                    return "";
                }
                angle += (Math.Sign(angle) * .0001388888888889);
                int degrees = (int)angle;
                double minutes = (((angle - (double)(int)angle) * 60));
                double seconds = ((minutes - ((int)minutes)) * 60);
                if (sign)
                {
                    string signString = angle > 0 ? "+" : "-";
                    return String.Format("{3}{0:00;00}:{1:00}:{2:00}", degrees, Math.Abs((int)minutes), Math.Abs((int)seconds), signString);
                }
                else
                {
                    return String.Format("{0:00}:{1:00}:{2:00}", degrees, Math.Abs((int)minutes), Math.Abs((int)seconds));
                }
            }
            catch
            {
                return "";
            }
        }
        static public string FormatDMS(double angle)
        {
            try
            {
                if (double.IsNaN(angle))
                {
                    return "";
                }
                angle += (Math.Sign(angle) * .0001388888888889);
                int degrees = Math.Abs((int)angle);
                double minutes = (((angle - (double)(int)angle) * 60));
                double seconds = ((minutes - ((int)minutes)) * 60);
                string sign = angle < 0 ? "-" : "";
                return String.Format("{3}{0:00}:{1:00}:{2:00}", Math.Abs(degrees), Math.Abs((int)minutes), Math.Abs((int)seconds), sign);
            }
            catch
            {
                return "";
            }
        }
        static public string FormatDMSWide(double angle)
        {
            try
            {
                if (double.IsNaN(angle))
                {
                    return "";
                }
                angle += (Math.Sign(angle) * .0001388888888889);
                int degrees = Math.Abs((int)angle);
                double minutes = (((angle - (double)(int)angle) * 60));
                double seconds = ((minutes - ((int)minutes)) * 60);
                string sign = angle < 0 ? "-" : "";
                return String.Format("{3}{0:00} : {1:00} : {2:00}", degrees, Math.Abs((int)minutes), Math.Abs((int)seconds),sign);
            }
            catch
            {
                return "";
            }
        }
        static public string FormatHMS(double angle)
        {
            try
            {
                if (double.IsNaN(angle))
                {
                    return "";
                }
                angle += (Math.Sign(angle) * .0001388888888889);
                int degrees = (int)angle;
                double minutes = (((angle - (double)(int)angle) * 60));
                double seconds = ((minutes - ((int)minutes)) * 60);
                return String.Format("{0:00}h{1:00}m{2:00}s", degrees, Math.Abs((int)minutes), Math.Abs((int)seconds));
            }
            catch
            {
                return "";
            }
        }

        static public double ParseRA(string data, bool degrees)
        {

            data = data.Trim().ToLower();


            if (data.Contains("d") || data.Contains("°"))
            {
                degrees = true;
            }
            if (data.Contains("h") || data.Contains(":"))
            {
                degrees = false;
            }
            double ra = Parse(data) / (degrees ? 15 : 1);

            return Math.Max(Math.Min(ra, 24.00), 0);

        }


        static public bool ValidateRA(string data)
        {

            data = data.Trim().ToLower();

            bool degrees = false;
            if (data.Contains("d") || data.Contains("°"))
            {
                degrees = true;
            }

            try
            {
                data = data.Trim().ToLower();

                data = data.Replace("d ", "d").Replace("h ", "h").Replace("m ", "m").Replace("s ", "s").Replace("\' ", "\'").Replace("\" ", "\"");
                double val = 0;
                if (data.IndexOfAny(new char[] { ':', ' ', 'd', 'h', 'm', 's', '\'', '\"', '°' }) > -1)
                {
                    double hours = 0;
                    double minutes = 0;
                    double seconds = 0;
                    double sign = 0;
                    string[] parts = data.Split(new char[] { ':', ' ', 'd', 'h', 'm', 's', '\'', '\"', '°' });
                    if (parts.GetLength(0) > 0)
                    {
                        if (!String.IsNullOrEmpty(parts[0]))
                        {
                            hours = Math.Abs(Convert.ToDouble(parts[0]));
                            sign = Math.Sign(Convert.ToDouble(parts[0]));
                        }
                    }

                    if (parts.GetLength(0) > 1)
                    {
                        if (!String.IsNullOrEmpty(parts[1]))
                        {
                            minutes = Convert.ToDouble(parts[1]);
                        }
                    }

                    if (parts.GetLength(0) > 2)
                    {
                        if (!String.IsNullOrEmpty(parts[2]))
                        {
                            seconds = Convert.ToDouble(parts[2]);
                        }
                    }
                    if (sign == 0)
                    {
                        sign = 1;
                    }

                    val = sign * (hours + minutes / 60 + seconds / 3600);
                } 
                else
                {
                    val = Convert.ToDouble(data);

                }

                val = val * (degrees ? 1 : 15);
                return (val >= 0 && val <= 360);
            }
            catch
            {
                return false;
            }
  
        }

        static public bool Validate(string data)
        {

            data = data.Trim().ToLower();



            try
            {
                data = data.Trim().ToLower();

                data = data.Replace("d ", "d").Replace("m ", "m").Replace("s ", "s").Replace("\' ", "\'").Replace("\" ", "\"");
                double val = 0;
                if (data.IndexOfAny(new char[] { ':', ' ', 'd', 'm', 's', '\'', '\"', '°' }) > -1)
                {
                    double degrees = 0;
                    double minutes = 0;
                    double seconds = 0;
                    double sign = 0;
                    string[] parts = data.Split(new char[] { ':', ' ', 'd', 'm', 's', '\'', '\"', '°' });
                    if (parts.GetLength(0) > 0)
                    {
                        if (!String.IsNullOrEmpty(parts[0]))
                        {
                            degrees = Math.Abs(Convert.ToDouble(parts[0]));
                            sign = Math.Sign(Convert.ToDouble(parts[0]));
                        }
                    }

                    if (parts.GetLength(0) > 1)
                    {
                        if (!String.IsNullOrEmpty(parts[1]))
                        {
                            minutes = Convert.ToDouble(parts[1]);
                        }
                    }

                    if (parts.GetLength(0) > 2)
                    {
                        if (!String.IsNullOrEmpty(parts[2]))
                        {
                            seconds = Convert.ToDouble(parts[2]);
                        }
                    }
                    if (sign == 0)
                    {
                        sign = 1;
                    }

                    val = sign * (degrees + minutes / 60 + seconds / 3600);
                }
                else
                {
                    val = Convert.ToDouble(data);

                }

                return (val >= -360 && val <= 360);
            }
            catch
            {
                return false;
            }

        }

        static public double ParseDec(string data)
        {
            double dec = Parse(data);
            return Math.Max(Math.Min(dec, 90.00), -90);
            
        }

        static public bool ValidateDec(string data)
        {
            try
            {
                data = data.Trim().ToLower();

                data = data.Replace("d ", "d").Replace("h ", "h").Replace("m ", "m").Replace("s ", "s").Replace("\' ", "\'").Replace("\" ", "\"");

                if (data.IndexOfAny(new char[] { ':', ' ', 'd', 'h', 'm', 's', '\'', '\"', '°' }) > -1)
                {
                    double hours = 0;
                    double minutes = 0;
                    double seconds = 0;
                    double sign = 0;
                    string[] parts = data.Split(new char[] { ':', ' ', 'd', 'h', 'm', 's', '\'', '\"', '°' });
                    if (parts.GetLength(0) > 0)
                    {
                        if (!String.IsNullOrEmpty(parts[0]))
                        {
                            hours = Math.Abs(Convert.ToDouble(parts[0]));
                            sign = Math.Sign(Convert.ToDouble(parts[0]));
                        }
                    }

                    if (parts.GetLength(0) > 1)
                    {
                        if (!String.IsNullOrEmpty(parts[1]))
                        {
                            minutes = Convert.ToDouble(parts[1]);
                        }
                    }

                    if (parts.GetLength(0) > 2)
                    {
                        if (!String.IsNullOrEmpty(parts[2]))
                        {
                            seconds = Convert.ToDouble(parts[2]);
                        }
                    }
                    if (sign == 0)
                    {
                        sign = 1;
                    }

                    double val = sign * (hours + minutes / 60 + seconds / 3600);
                    return (val >= -90 && val <= 90);
                } 
                else
                {
                    double val = Convert.ToDouble(data);
                    return (val >= -90 && val <= 90);

                }
            }
            catch
            {
                return false;
            }
        } 

        static public double Parse(string data)
        {
            try
            {
                data = data.Trim().ToLower();

                data = data.Replace("d ", "d").Replace("h ", "h").Replace("m ", "m").Replace("s ", "s").Replace("\' ", "\'").Replace("\" ", "\"");

                if (data.IndexOfAny(new char[] { ':', ' ', 'd', 'h', 'm', 's', '\'', '\"','°' }) > -1)
                {
                    double hours = 0;
                    double minutes = 0;
                    double seconds = 0;
                    double sign = 0;
                    string[] parts = data.Split(new char[] { ':', ' ', 'd', 'h', 'm', 's', '\'', '\"', '°' });
                    if (parts.GetLength(0) > 0)
                    {
                        if (!String.IsNullOrEmpty(parts[0]))
                        {
                            hours = Math.Abs(Convert.ToDouble(parts[0]));
                            sign = Math.Sign(Convert.ToDouble(parts[0]));
                            if (parts[0].Contains("-"))
                            {
                                sign = -1;
                            }
                        }
                    }

                    if (parts.GetLength(0) > 1)
                    {
                        if (!String.IsNullOrEmpty(parts[1]))
                        {
                            minutes = Convert.ToDouble(parts[1]);
                        }
                    }

                    if (parts.GetLength(0) > 2)
                    {
                        if (!String.IsNullOrEmpty(parts[2]))
                        {
                            seconds = Convert.ToDouble(parts[2]);
                        }
                    }
                    if (sign == 0)
                    {
                        sign = 1;
                    }

                    return sign * (hours + minutes / 60 + seconds / 3600);
                }
                else
                {
                    bool sucsess = false;
                    double val =0;
                    sucsess = double.TryParse(data, out val);

                    return val;

                }
            }
            catch
            {
                return 0;
            }
        }   
        
        public static bool operator == (Coordinates one, Coordinates two)
        {
            if (!(one is Coordinates))
            {
                return !(two is Coordinates);
            }

            return one.Equals(two);

        }
        public override bool Equals(object obj)
        {
            if (!(obj is Coordinates))
            {
                return false;
            }
            Coordinates that = (Coordinates)obj;
            return (this.ascention == that.ascention && this.declination == that.declination);
        }

        public static bool operator != (Coordinates one, Coordinates two)
        {
            if (one.ascention == two.ascention && one.declination == two.declination)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public override int GetHashCode()
        {
            return ascention.GetHashCode() ^ declination.GetHashCode();
        }

        public static Coordinates FromRaDec(double ra, double dec)
        {
            return new Coordinates((ra-12)*15 * RC, dec * RC);
        }

        public static Coordinates FromLatLng(double lat, double lng)
        {
            return new Coordinates(lng * RC, lat * RC);
        }

        public static double DMSToDegrees(double Degrees, double Minutes, double Seconds)
        {
            return Degrees + Minutes / 60 + Seconds / 3600;
        }

        public static double DegreesToRadians(double Degrees)
        {
            return Degrees * 0.017453292519943295769236907684886;
        }

        public static double RadiansToDegrees(double Radians)
        {
            return Radians * 57.295779513082320876798154814105;
        }

        public static double RadiansToHours(double Radians)
        {
            return Radians * 3.8197186342054880584532103209403;
        }

        public static double HoursToRadians(double Hours)
        {
            return Hours * 0.26179938779914943653855361527329;
        }

        public static double HoursToDegrees(double Hours)
        {
            return Hours * 15;
        }

        public static double DegreesToHours(double Degrees)
        {
            return Degrees / 15;
        }

        public static double PI()
        {
            return 3.1415926535897932384626433832795;
        }

        public static double MapTo0To360Range(double Degrees)
        {
            return Degrees - Math.Floor(Degrees / 360.0) * 360.0;
        }

        public static double MapTo0To24Range(double HourAngle)
        {
            return HourAngle - Math.Floor(HourAngle / 24.0) * 24.0;
        }

        public static double MeanObliquityOfEcliptic(double JD)
        {
            double U = (JD - 2451545) / 3652500;
            double Usquared = U*U;
            double Ucubed = Usquared*U;
            double U4 = Ucubed*U;
            double U5 = U4*U;
            double U6 = U5*U;
            double U7 = U6*U;
            double U8 = U7*U;
            double U9 = U8*U;
            double U10 = U9*U;


            return DMSToDegrees(23, 26, 21.448)  - DMSToDegrees(0, 0, 4680.93) * U
                                               - DMSToDegrees(0, 0, 1.55) * Usquared
                                               + DMSToDegrees(0, 0, 1999.25) * Ucubed
                                               - DMSToDegrees(0, 0, 51.38) * U4
                                               - DMSToDegrees(0, 0, 249.67) * U5
                                               - DMSToDegrees(0, 0, 39.05) * U6 
                                               + DMSToDegrees(0, 0, 7.12) * U7
                                               + DMSToDegrees(0, 0, 27.87) * U8
                                               + DMSToDegrees(0, 0, 5.79) * U9
                                               + DMSToDegrees(0, 0, 2.45) * U10;
        }

        static double[][] RotationMatrix = null;

        public static double[] J2000toGalactic(double J2000RA, double J2000DEC)
        {
            double[] J2000pos = new double[] { Math.Cos(J2000RA / 180.0 * Math.PI) * Math.Cos(J2000DEC / 180.0 * Math.PI), Math.Sin(J2000RA / 180.0 * Math.PI) * Math.Cos(J2000DEC / 180.0 * Math.PI), Math.Sin(J2000DEC / 180.0 * Math.PI) };

            if (RotationMatrix == null)
            {
                RotationMatrix = new double[3][];
                RotationMatrix[0] = new double[] { -.0548755604, -.8734370902, -.4838350155 };
                RotationMatrix[1] = new double[] { .4941094279, -.4448296300, .7469822445 };
                RotationMatrix[2] = new double[] { -.8676661490, -.1980763734, .4559837762 };
            }



            double[] Galacticpos = new double[3];
            for (int i = 0; i < 3; i++)
            {
                Galacticpos[i] = J2000pos[0] * RotationMatrix[i][0] + J2000pos[1] * RotationMatrix[i][1] + J2000pos[2] * RotationMatrix[i][2];
            }

            double GalacticL2 = Math.Atan2(Galacticpos[1], Galacticpos[0]);
            if (GalacticL2 < 0)
            {
                GalacticL2 = GalacticL2 + 2 * Math.PI;
            }
            if (GalacticL2 > 2 * Math.PI)
            {
                GalacticL2 = GalacticL2 - 2 * Math.PI;
            }

            double GalacticB2 = Math.Atan2(Galacticpos[2], Math.Sqrt(Galacticpos[0] * Galacticpos[0] + Galacticpos[1] * Galacticpos[1]));

            return new double[] { GalacticL2 / Math.PI * 180.0, GalacticB2 / Math.PI * 180.0 };
        }




        public static double[] GalactictoJ2000(double GalacticL2, double GalacticB2)
        {
            double[] Galacticpos = new double[] { Math.Cos(GalacticL2 / 180.0 * Math.PI) * Math.Cos(GalacticB2 / 180.0 * Math.PI), Math.Sin(GalacticL2 / 180.0 * Math.PI) * Math.Cos(GalacticB2 / 180.0 * Math.PI), Math.Sin(GalacticB2 / 180.0 * Math.PI) };
            if (RotationMatrix == null)
            {
                RotationMatrix = new double[3][];
                RotationMatrix[0] = new double[] { -.0548755604, -.8734370902, -.4838350155 };
                RotationMatrix[1] = new double[] { .4941094279, -.4448296300, .7469822445 };
                RotationMatrix[2] = new double[] { -.8676661490, -.1980763734, .4559837762 };
            }

            double[] J2000pos = new double[3];
            for (int i = 0; i < 3; i++)
            {
                J2000pos[i] = Galacticpos[0] * RotationMatrix[0][i] + Galacticpos[1] * RotationMatrix[1][i] + Galacticpos[2] * RotationMatrix[2][i];
            }

            double J2000RA = Math.Atan2(J2000pos[1], J2000pos[0]);
            if (J2000RA < 0)
            {
                J2000RA = J2000RA + 2 * Math.PI;
            }
            if (J2000RA > 2 * Math.PI)
            {
                J2000RA = J2000RA - 2 * Math.PI;
            }

            double J2000DEC = Math.Atan2(J2000pos[2], Math.Sqrt(J2000pos[0] * J2000pos[0] + J2000pos[1] * J2000pos[1]));

            return new double[] { J2000RA / Math.PI * 180.0, J2000DEC / Math.PI * 180.0 };

        }
    }
}
