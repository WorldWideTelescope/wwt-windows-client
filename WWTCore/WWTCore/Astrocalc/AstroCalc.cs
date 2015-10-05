// This is the main DLL file.


using System;




namespace AstroCalc
{
	public struct AstroRaDec
	{
		public AstroRaDec(double ra, double dec, double dist, bool shadow, bool eclipsed)
		{
			RA = ra;
			Dec = dec;
			Distance = dist;
			Shadow = shadow;
			Eclipsed = eclipsed;
		}
		public double RA;
		public double Dec;
		public double Distance;
		public bool Shadow;
		public bool Eclipsed;
	}
	public struct RiseSetDetails
	{
		//Constructors / Destructors
		  public RiseSetDetails(bool bValid, double Rise, double Transit, double Set, bool neverRises)
		  {
			  this.bValid = bValid;
			  this.Rise = Rise;
			  this.Transit = Transit;
			  this.Set = Set;
              bNeverRises = neverRises;
		  }

		//Member variables
		  public bool bValid;
		  public double Rise;
		  public double Transit;
		  public double Set;
          public bool bNeverRises;

	}
	public class AstroCalc
	{
		private static CAAGalileanMoonsDetails galDetails = new CAAGalileanMoonsDetails();
		private static CAAEllipticalPlanetaryDetails jupDetails = new CAAEllipticalPlanetaryDetails();
		private static CAAPhysicalJupiterDetails jupPhisical = new CAAPhysicalJupiterDetails();
		private static double jDateLast;
		public static AstroRaDec GetPlanet(double jDate, int planet, double locLat, double locLong, double locHeight)
		{


			locLong = -locLong;
			if (planet < 9)
			{
				var Details = CAAElliptical.Calculate(jDate, (EllipticalObject) planet);
				var corrected = CAAParallax.Equatorial2Topocentric(Details.ApparentGeocentricRA, Details.ApparentGeocentricDeclination, Details.ApparentGeocentricDistance, locLong, locLat, locHeight, jDate);
				return new AstroRaDec(corrected.X, corrected.Y, Details.ApparentGeocentricDistance, false, false);
			}
		    if (planet == 9)
		    {
		        var lat = CAAMoon.EclipticLatitude(jDate);
		        var lng = CAAMoon.EclipticLongitude(jDate);
		        var dis = CAAMoon.RadiusVector(jDate)/149598000;
		        var epsilon = CAANutation.TrueObliquityOfEcliptic(jDate);
		        var d = CAACoordinateTransformation.Ecliptic2Equatorial(lng, lat, epsilon);
		        var corrected = CAAParallax.Equatorial2Topocentric(d.X, d.Y, dis, locLong, locLat, locHeight, jDate);

		        return new AstroRaDec(corrected.X, corrected.Y, dis, false, false);

		    }
		    if (jDate != jDateLast)
		    {
		        jupDetails = CAAElliptical.Calculate(jDate, (EllipticalObject) 4);
		        jupPhisical = CAAPhysicalJupiter.Calculate(jDate);
		        var corrected = CAAParallax.Equatorial2Topocentric(jupDetails.ApparentGeocentricRA, jupDetails.ApparentGeocentricDeclination, jupDetails.ApparentGeocentricDistance, locLong, locLat, locHeight, jDate);
		        jupDetails.ApparentGeocentricRA = corrected.X;
		        jupDetails.ApparentGeocentricDeclination = corrected.Y;
		        galDetails = CAAGalileanMoons.Calculate(jDate);
		        jDateLast = jDate;
		    }


		    const double jupiterDiameter = 0.000954501;
		    var scale = (Math.Atan(.5 * (jupiterDiameter / jupDetails.ApparentGeocentricDistance))) / 3.1415927 * 180;

		    var raScale = (scale / Math.Cos(jupDetails.ApparentGeocentricDeclination / 180.0 * 3.1415927))/15;

		    double xMoon=0;
		    double yMoon=0;
		    double zMoon=0;
		    var shadow = false;
		    var eclipsed = false;

		    switch (planet)
		    {
		        case 10: // IO
		            xMoon = galDetails.Satellite1.ApparentRectangularCoordinates.X;
		            yMoon = galDetails.Satellite1.ApparentRectangularCoordinates.Y;
		            zMoon = galDetails.Satellite1.ApparentRectangularCoordinates.Z;
		            eclipsed = galDetails.Satellite1.bInEclipse;
		            shadow = galDetails.Satellite1.bInShadowTransit;
		            break;
		        case 11: //Europa
		            xMoon = galDetails.Satellite2.ApparentRectangularCoordinates.X;
		            yMoon = galDetails.Satellite2.ApparentRectangularCoordinates.Y;
		            zMoon = galDetails.Satellite2.ApparentRectangularCoordinates.Z;
		            eclipsed = galDetails.Satellite2.bInEclipse;
		            shadow = galDetails.Satellite2.bInShadowTransit;
		            break;
		        case 12: //Ganymede
		            xMoon = galDetails.Satellite3.ApparentRectangularCoordinates.X;
		            yMoon = galDetails.Satellite3.ApparentRectangularCoordinates.Y;
		            zMoon = galDetails.Satellite3.ApparentRectangularCoordinates.Z;
		            eclipsed = galDetails.Satellite3.bInEclipse;
		            shadow = galDetails.Satellite3.bInShadowTransit;
		            break;
		        case 13: //Callisto
		            xMoon = galDetails.Satellite4.ApparentRectangularCoordinates.X;
		            yMoon = galDetails.Satellite4.ApparentRectangularCoordinates.Y;
		            zMoon = galDetails.Satellite4.ApparentRectangularCoordinates.Z;
		            eclipsed = galDetails.Satellite4.bInEclipse;
		            shadow = galDetails.Satellite4.bInShadowTransit;
		            break;
		        case 14: // IO Shadow
		            xMoon = galDetails.Satellite1.ApparentShadowRectangularCoordinates.X;
		            yMoon = galDetails.Satellite1.ApparentShadowRectangularCoordinates.Y;
		            zMoon = galDetails.Satellite1.ApparentShadowRectangularCoordinates.Z*.9;
		            shadow = galDetails.Satellite1.bInShadowTransit;
		            break;
		        case 15: //Europa Shadow
		            xMoon = galDetails.Satellite2.ApparentShadowRectangularCoordinates.X;
		            yMoon = galDetails.Satellite2.ApparentShadowRectangularCoordinates.Y;
		            zMoon = galDetails.Satellite2.ApparentShadowRectangularCoordinates.Z*.9;
		            shadow = galDetails.Satellite2.bInShadowTransit;
		            break;
		        case 16: //Ganymede Shadow
		            xMoon = galDetails.Satellite3.ApparentShadowRectangularCoordinates.X;
		            yMoon = galDetails.Satellite3.ApparentShadowRectangularCoordinates.Y;
		            zMoon = galDetails.Satellite3.ApparentShadowRectangularCoordinates.Z*.9;
		            shadow = galDetails.Satellite3.bInShadowTransit;
		            break;
		        case 17: //Callisto Shadow
		            xMoon = galDetails.Satellite4.ApparentShadowRectangularCoordinates.X;
		            yMoon = galDetails.Satellite4.ApparentShadowRectangularCoordinates.Y;
		            zMoon = galDetails.Satellite4.ApparentShadowRectangularCoordinates.Z*.9;
		            shadow = galDetails.Satellite4.bInShadowTransit;
		            break;
		    }

		    var radians = jupPhisical.P /180.0 * 3.1415927;
		    double xTemp = xMoon * Math.Cos(radians) - yMoon * Math.Sin(radians);
		    double yTemp = xMoon * Math.Sin(radians) + yMoon * Math.Cos(radians);
		    xMoon = xTemp;
		    yMoon = yTemp;

		    return new AstroRaDec(jupDetails.ApparentGeocentricRA - (xMoon * raScale), jupDetails.ApparentGeocentricDeclination + yMoon * scale, jupDetails.ApparentGeocentricDistance + (zMoon *jupiterDiameter/2), shadow, eclipsed);
		}

		public static double GetJulianDay(double year, double month, double day)
		{

			return CAADate.DateToJD((int)year, (int)month, day, true);

		}

		public static AstroRaDec EclipticToJ2000(double l, double b, double jNow)
		{
			var radec = CAACoordinateTransformation.Ecliptic2Equatorial(l, b, CAANutation.TrueObliquityOfEcliptic(jNow));
			return new AstroRaDec(radec.X, radec.Y, 0, false, false);

		}

		public static AstroRaDec GalacticToJ2000(double l, double b)
		{
			var radec = CAACoordinateTransformation.Galactic2Equatorial(l, b);
			return new AstroRaDec(radec.X, radec.Y, 0, false, false);

		}
		public static AstroRaDec J2000ToGalactic(double ra, double dec)
		{
			var galactic = CAACoordinateTransformation.Equatorial2Galactic(ra, dec);
			return new AstroRaDec(galactic.X, galactic.Y, 0, false, false);

		}
		public static RiseSetDetails GetRiseTrinsitSet(double jd, double lat, double lng, double ra1, double dec1, double ra2, double dec2, double ra3, double dec3, int type)
		{
			var alt = -0.5667;

			switch (type)
			{
				case 0: // Planet or star
					alt = -0.5667;
					break;
				case 1: // sun
					alt = -0.8333;
					break;
				case 2:
					alt = 0.125;
					break;
			}
			var RiseTransitSetTime = CAARiseTransitSet.Rise(jd, ra1, dec1, ra2, dec2, ra3, dec3, lng, lat, alt);

            var neverRises = false;
            if (!RiseTransitSetTime.bValid)
            {
                neverRises = Math.Sign(lat) != Math.Sign(dec2);

            }

            return new RiseSetDetails(RiseTransitSetTime.bValid, RiseTransitSetTime.Rise, RiseTransitSetTime.Transit, RiseTransitSetTime.Set, neverRises);
		}
	}
}

