using System;
public static partial class GlobalMembersStdafx
{

	public static int mainTEST()
	{
        CAADate date;
        bool bLeap;
	  //Do a full round trip test on CAADate across a nice wide range. Note we should expect
	  //some printfs to appear during this test (Specifically a monotonic error for 15 October 1582
	  double prevJulian = -1;
	  for (var YYYY =-4712; YYYY<5000; YYYY++) //Change the end YYYY value if you would like to test a longer range
	  {
		if ((YYYY % 1000) == 0)
		  Console.Write("Doing date tests on year {0:D}\n", YYYY);
		for (var MMMM =1; MMMM<=12; MMMM++)
		{
		  bLeap = CAADate.IsLeap(YYYY, (YYYY >= 1582));
		  for (var DDDD =1; DDDD<=CAADate.DaysInMonth(MMMM, bLeap); DDDD++)
		  {
			var bGregorian = CAADate.AfterPapalReform(YYYY, MMMM, DDDD);
			 date = new CAADate(YYYY, MMMM, DDDD, 12, 0, 0, bGregorian);
			if ((date.Year() != YYYY) || (date.Month() != MMMM)|| (date.Day() != DDDD))
			  Console.Write("Round trip bug with date {0:D}/{1:D}/{2:D}\n", YYYY, MMMM, DDDD);
			var currentJulian = date.Julian();
			if ((prevJulian != -1) && ((prevJulian + 1) != currentJulian))
			  Console.Write("Julian Day monotonic bug with date {0:D}/{1:D}/{2:D}\n", YYYY, MMMM, DDDD);
			prevJulian = currentJulian;

			//Only do round trip tests between the Julian and Gregorian calendars after the papal 
			//reform. This is because the CAADate class does not support the propalactic Gregorian 
			//calendar, while it does fully support the propalactic Julian calendar.
			if (bGregorian)
			{
			  var GregorianDate = CAADate.JulianToGregorian(YYYY, MMMM, DDDD);
			  var JulianDate = CAADate.GregorianToJulian(GregorianDate.Year, GregorianDate.Month, GregorianDate.Day);
			  if ((JulianDate.Year != YYYY) || (JulianDate.Month != MMMM)|| (JulianDate.Day != DDDD))
				Console.Write("Round trip bug with Julian -> Gregorian Calendar {0:D}/{1:D}/{2:D}\n", YYYY, MMMM, DDDD);
			}
		  }
		}
	  }
	  Console.Write("Date tests completed\n");

	  //Test out the AADate class
	  date = new CAADate();
	  date.Set(2000, 1, 1, 12, 1, 2.3, true);
	  var Year = 0;
	  var Month = 0;
	  var Day = 0;
	  var Hour = 0;
	  var Minute = 0;
	  double Second = 0;
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);
	  var DaysInMonth = date.DaysInMonth();
	  var DaysInYear = date.DaysInYear();
	  bLeap = date.Leap();
	  var Julian = date.Julian();
	  var FractionalYear = date.FractionalYear();
	  var DayOfYear = date.DayOfYear();
	  var dow = date.DayOfWeek();
	  Year = date.Year();
	  Month = date.Month();
	  Day = date.Day();
	  Hour = date.Hour();
	  Minute = date.Minute();
	  Second = date.Second();
	  double Julian2 = date;

	  date.Set(1978, 11, 14, 0, 0, 0, true);
	  var DayNumber = (int)(date.DayOfYear());
	  CAADate.DayOfYearToDayAndMonth(DayNumber, date.Leap(), ref Day, ref Month);
	  Year = date.Year();

	  //Test out the AAEaster class
	  var easterDetails = CAAEaster.Calculate(1991, true);
	  var easterDetails2 = CAAEaster.Calculate(1818, true);
	  var easterDetails3 = CAAEaster.Calculate(179, false);

	  //Test out the AADynamicalTime class
	  date.Set(1977, 2, 18, 3, 37, 40, true);
	  var DeltaT = CAADynamicalTime.DeltaT(date.Julian());
	  date.Set(333, 2, 6, 6, 0, 0, false);
	  DeltaT = CAADynamicalTime.DeltaT(date.Julian());

	  //Test out the AAGlobe class
	  var rhosintheta = CAAGlobe.RhoSinThetaPrime(33.356111, 1706);
	  var rhocostheta = CAAGlobe.RhoCosThetaPrime(33.356111, 1706);
	  var RadiusOfLatitude = CAAGlobe.RadiusOfParallelOfLatitude(42);
	  var RadiusOfCurvature = CAAGlobe.RadiusOfCurvature(42);
	  var Distance = CAAGlobe.DistanceBetweenPoints(CAACoordinateTransformation.DMSToDegrees(48, 50, 11), CAACoordinateTransformation.DMSToDegrees(2, 20, 14, false), CAACoordinateTransformation.DMSToDegrees(38, 55, 17), CAACoordinateTransformation.DMSToDegrees(77, 3, 56));


	  var Distance1 = CAAGlobe.DistanceBetweenPoints(50, 0, 50, 60);
	  var Distance2 = CAAGlobe.DistanceBetweenPoints(50, 0, 50, 1);
	  var Distance3 = CAAGlobe.DistanceBetweenPoints(CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 0, CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 1);
	  var Distance4 = CAAGlobe.DistanceBetweenPoints(CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 0, CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 180);
	  var Distance5 = CAAGlobe.DistanceBetweenPoints(CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 0, CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 90);


	  //Test out the AASidereal class
	  date.Set(1987, 4, 10, 0, 0, 0, true);
	  var MST = CAASidereal.MeanGreenwichSiderealTime(date.Julian());
	  var AST = CAASidereal.ApparentGreenwichSiderealTime(date.Julian());
	  date.Set(1987, 4, 10, 19, 21, 0, true);
	  MST = CAASidereal.MeanGreenwichSiderealTime(date.Julian());

	  //Test out the AACoordinateTransformation class
	//C++ TO C# CONVERTER TODO TASK: Octal literals cannot be represented in C#:
	  var Ecliptic = CAACoordinateTransformation.Equatorial2Ecliptic(CAACoordinateTransformation.DMSToDegrees(7, 45, 18.946), CAACoordinateTransformation.DMSToDegrees(28, 01, 34.26), 23.4392911);
	  var Equatorial = CAACoordinateTransformation.Ecliptic2Equatorial(Ecliptic.X, Ecliptic.Y, 23.4392911);
	  var Galactic = CAACoordinateTransformation.Equatorial2Galactic(CAACoordinateTransformation.DMSToDegrees(17, 48, 59.74), CAACoordinateTransformation.DMSToDegrees(14, 43, 8.2, false));
	  var Equatorial2 = CAACoordinateTransformation.Galactic2Equatorial(Galactic.X, Galactic.Y);
	  date.Set(1987, 4, 10, 19, 21, 0, true);
	  AST = CAASidereal.ApparentGreenwichSiderealTime(date.Julian());
	  var LongtitudeAsHourAngle = CAACoordinateTransformation.DegreesToHours(CAACoordinateTransformation.DMSToDegrees(77, 3, 56));
	  var Alpha = CAACoordinateTransformation.DMSToDegrees(23, 9, 16.641);
	  var LocalHourAngle = AST - LongtitudeAsHourAngle - Alpha;
	  var Horizontal = CAACoordinateTransformation.Equatorial2Horizontal(LocalHourAngle, CAACoordinateTransformation.DMSToDegrees(6, 43, 11.61, false), CAACoordinateTransformation.DMSToDegrees(38, 55, 17));
	  var Equatorial3 = CAACoordinateTransformation.Horizontal2Equatorial(Horizontal.X, Horizontal.Y, CAACoordinateTransformation.DMSToDegrees(38, 55, 17));
	  var alpha2 = CAACoordinateTransformation.MapTo0To24Range(AST - Equatorial3.X - LongtitudeAsHourAngle);

	  //Test out the CAANutation class (on its own)
	  date.Set(1987, 4, 10, 0, 0, 0, true);
	  var Obliquity = CAANutation.MeanObliquityOfEcliptic(date.Julian());
	  var NutationInLongitude = CAANutation.NutationInLongitude(date.Julian());
		var NutationInEcliptic = CAANutation.NutationInObliquity(date.Julian());

	  //Test out the CAAParallactic class
	  var HourAngle = CAAParallactic.ParallacticAngle(-3, 10, 20);
	  var EclipticLongitude = CAAParallactic.EclipticLongitudeOnHorizon(5, 23.44, 51);
	  var EclipticAngle = CAAParallactic.AngleBetweenEclipticAndHorizon(5, 23.44, 51);
	  var Angle = CAAParallactic.AngleBetweenNorthCelestialPoleAndNorthPoleOfEcliptic(90, 0, 23.44);

	  //Test out the CAARefraction class
	  var R1 = CAARefraction.RefractionFromApparent(0.5);
	  var R2 = CAARefraction.RefractionFromTrue(0.5 - R1 + CAACoordinateTransformation.DMSToDegrees(0, 32, 0));
	  var R3 = CAARefraction.RefractionFromApparent(90);

	  //Test out the CAAAngularSeparation class
	  var AngularSeparation = CAAAngularSeparation.Separation(CAACoordinateTransformation.DMSToDegrees(14, 15, 39.7), CAACoordinateTransformation.DMSToDegrees(19, 10, 57), CAACoordinateTransformation.DMSToDegrees(13, 25, 11.6), CAACoordinateTransformation.DMSToDegrees(11, 9, 41, false));
	  var AngularSeparation2 = CAAAngularSeparation.Separation(CAACoordinateTransformation.DMSToDegrees(2, 0, 0), CAACoordinateTransformation.DMSToDegrees(0, 0, 0), CAACoordinateTransformation.DMSToDegrees(2, 0, 0), CAACoordinateTransformation.DMSToDegrees(0, 0, 0));
	  var AngularSeparation3 = CAAAngularSeparation.Separation(CAACoordinateTransformation.DMSToDegrees(2, 0, 0), CAACoordinateTransformation.DMSToDegrees(0, 0, 0), CAACoordinateTransformation.DMSToDegrees(14, 0, 0), CAACoordinateTransformation.DMSToDegrees(0, 0, 0));

	  var PA0 = CAAAngularSeparation.PositionAngle(CAACoordinateTransformation.DMSToDegrees(5, 32, 0.4), CAACoordinateTransformation.DMSToDegrees(0, 17, 56.9, false), CAACoordinateTransformation.DMSToDegrees(5, 36, 12.81), CAACoordinateTransformation.DMSToDegrees(1, 12, 7, false));

	  var PA1 = CAAAngularSeparation.PositionAngle(CAACoordinateTransformation.DMSToDegrees(5, 40, 45.52), CAACoordinateTransformation.DMSToDegrees(1, 56, 33.3, false), CAACoordinateTransformation.DMSToDegrees(5, 36, 12.81), CAACoordinateTransformation.DMSToDegrees(1, 12, 7, false));


	  var distance = CAAAngularSeparation.DistanceFromGreatArc(CAACoordinateTransformation.DMSToDegrees(5, 32, 0.4), CAACoordinateTransformation.DMSToDegrees(0, 17, 56.9, false), CAACoordinateTransformation.DMSToDegrees(5, 40, 45.52), CAACoordinateTransformation.DMSToDegrees(1, 56, 33.3, false), CAACoordinateTransformation.DMSToDegrees(5, 36, 12.81), CAACoordinateTransformation.DMSToDegrees(1, 12, 7, false));

	  var bType1 = false;
	  var separation = CAAAngularSeparation.SmallestCircle(CAACoordinateTransformation.DMSToDegrees(12, 41, 8.63), CAACoordinateTransformation.DMSToDegrees(5, 37, 54.2, false), CAACoordinateTransformation.DMSToDegrees(12, 52, 5.21), CAACoordinateTransformation.DMSToDegrees(4, 22, 26.2, false), CAACoordinateTransformation.DMSToDegrees(12, 39, 28.11), CAACoordinateTransformation.DMSToDegrees(1, 50, 3.7, false), ref bType1);

	  separation = CAAAngularSeparation.SmallestCircle(CAACoordinateTransformation.DMSToDegrees(9, 5, 41.44), CAACoordinateTransformation.DMSToDegrees(18, 30, 30), CAACoordinateTransformation.DMSToDegrees(9, 9, 29), CAACoordinateTransformation.DMSToDegrees(17, 43, 56.7), CAACoordinateTransformation.DMSToDegrees(8, 59, 47.14), CAACoordinateTransformation.DMSToDegrees(17, 49, 36.8), ref bType1);

	  Alpha = CAACoordinateTransformation.DMSToDegrees(2, 44, 11.986);
	  var Delta = CAACoordinateTransformation.DMSToDegrees(49, 13, 42.48);
	  var PA = CAAPrecession.AdjustPositionUsingUniformProperMotion((2462088.69-2451545)/365.25, Alpha, Delta, 0.03425, -0.0895);

	  var Precessed = CAAPrecession.PrecessEquatorial(PA.X, PA.Y, 2451545, 2462088.69);

	  Alpha = CAACoordinateTransformation.DMSToDegrees(2, 31, 48.704);
	  Delta = CAACoordinateTransformation.DMSToDegrees(89, 15, 50.72);
	  var PA2 = CAAPrecession.AdjustPositionUsingUniformProperMotion((2415020.3135-2451545)/365.25, Alpha, Delta, 0.19877, -0.0152);
	  //CAA2DCoordinate Precessed2 = CAAPrecession::PrecessEquatorialFK4(PA2.X, PA2.Y, 2451545, 2415020.3135);

	  var PM = CAAPrecession.EquatorialPMToEcliptic(0, 0, 0, 1, 1, 23);


	  var PA3 = CAAPrecession.AdjustPositionUsingMotionInSpace(2.64, -7.6, -1000, CAACoordinateTransformation.DMSToDegrees(6, 45, 8.871), CAACoordinateTransformation.DMSToDegrees(16, 42, 57.99, false), -0.03847, -1.2053);
	  var PA4 = CAAPrecession.AdjustPositionUsingUniformProperMotion(-1000, CAACoordinateTransformation.DMSToDegrees(6, 45, 8.871), CAACoordinateTransformation.DMSToDegrees(16, 42, 57.99, false), -0.03847, -1.2053);

	  var PA5 = CAAPrecession.AdjustPositionUsingMotionInSpace(2.64, -7.6, -12000, CAACoordinateTransformation.DMSToDegrees(6, 45, 8.871), CAACoordinateTransformation.DMSToDegrees(16, 42, 57.99, false), -0.03847, -1.2053);
	  var PA6 = CAAPrecession.AdjustPositionUsingUniformProperMotion(-12000, CAACoordinateTransformation.DMSToDegrees(6, 45, 8.871), CAACoordinateTransformation.DMSToDegrees(16, 42, 57.99, false), -0.03847, -1.2053);

	  Alpha = CAACoordinateTransformation.DMSToDegrees(2, 44, 11.986);
	  Delta = CAACoordinateTransformation.DMSToDegrees(49, 13, 42.48);
	  var PA7 = CAAPrecession.AdjustPositionUsingUniformProperMotion((2462088.69-2451545)/365.25, Alpha, Delta, 0.03425, -0.0895);
	  var EarthVelocity = CAAAberration.EarthVelocity(2462088.69);
	  var Aberration = CAAAberration.EquatorialAberration(PA7.X, PA7.Y, 2462088.69);
	  PA7.X += Aberration.X;
	  PA7.Y += Aberration.Y;
	  PA7 = CAAPrecession.PrecessEquatorial(PA7.X, PA7.Y, 2451545, 2462088.69);

	  Obliquity = CAANutation.MeanObliquityOfEcliptic(2462088.69);
	  NutationInLongitude = CAANutation.NutationInLongitude(2462088.69);
		NutationInEcliptic = CAANutation.NutationInObliquity(2462088.69);
	  var AlphaNutation = CAANutation.NutationInRightAscension(PA7.X, PA7.Y, Obliquity, NutationInLongitude, NutationInEcliptic);
	  var DeltaNutation = CAANutation.NutationInDeclination(PA7.X, PA7.Y, Obliquity, NutationInLongitude, NutationInEcliptic);
	  PA7.X += CAACoordinateTransformation.DMSToDegrees(0, 0, AlphaNutation/15);
	  PA7.Y += CAACoordinateTransformation.DMSToDegrees(0, 0, DeltaNutation);



	  //Try out the AA kepler class
	  var E0 = CAAKepler.Calculate(5, 0.1, 100);
	  var E02 = CAAKepler.Calculate(5, 0.9, 100);
	  //double E03 = CAAKepler::Calculate(

	  //Try out the binary star class
	  var bsdetails = CAABinaryStar.Calculate(1980, 41.623, 1934.008, 0.2763, 0.907, 59.025, 23.717, 219.907);
	  var ApparentE = CAABinaryStar.ApparentEccentricity(0.2763, 59.025, 219.907);


	  //Test out the CAAStellarMagnitudes class
	  var CombinedMag = CAAStellarMagnitudes.CombinedMagnitude(1.96, 2.89);
	  double[] Mags = { 4.73, 5.22, 5.60 };
	  var CombinedMag2 = CAAStellarMagnitudes.CombinedMagnitude(3, Mags);
	  var BrightnessRatio = CAAStellarMagnitudes.BrightnessRatio(0.14, 2.12);
	  var MagDiff = CAAStellarMagnitudes.MagnitudeDifference(BrightnessRatio);
	  var MagDiff2 = CAAStellarMagnitudes.MagnitudeDifference(500);


	  //Test out the CAAVenus class
	  var VenusLong = CAAVenus.EclipticLongitude(2448976.5);
	  var VenusLat = CAAVenus.EclipticLatitude(2448976.5);
	  var VenusRadius = CAAVenus.RadiusVector(2448976.5);


	  //Test out the CAAMercury class
	  var MercuryLong = CAAMercury.EclipticLongitude(2448976.5);
	  var MercuryLat = CAAMercury.EclipticLatitude(2448976.5);
	  var MercuryRadius = CAAMercury.RadiusVector(2448976.5);


	  //Test out the CAAEarth class
	  var EarthLong = CAAEarth.EclipticLongitude(2448908.5);
	  var EarthLat = CAAEarth.EclipticLatitude(2448908.5);
	  var EarthRadius = CAAEarth.RadiusVector(2448908.5);

	  var EarthLong2 = CAAEarth.EclipticLongitudeJ2000(2448908.5);
	  var EarthLat2 = CAAEarth.EclipticLatitudeJ2000(2448908.5);


	  //Test out the CAASun class
	  var SunLong = CAASun.GeometricEclipticLongitude(2448908.5);
	  var SunLat = CAASun.GeometricEclipticLatitude(2448908.5);

	  var SunLongCorrection = CAAFK5.CorrectionInLongitude(SunLong, SunLat, 2448908.5);
	  var SunLatCorrection = CAAFK5.CorrectionInLatitude(SunLong, 2448908.5);

	  SunLong = CAASun.ApparentEclipticLongitude(2448908.5);
	  SunLat = CAASun.ApparentEclipticLatitude(2448908.5);
	  Equatorial = CAACoordinateTransformation.Ecliptic2Equatorial(SunLong, SunLat, CAANutation.TrueObliquityOfEcliptic(2448908.5));

	  var SunCoord = CAASun.EclipticRectangularCoordinatesMeanEquinox(2448908.5);
	  var SunCoord2 = CAASun.EclipticRectangularCoordinatesJ2000(2448908.5);
	  var SunCoord3 = CAASun.EquatorialRectangularCoordinatesJ2000(2448908.5);
	  var SunCoord4 = CAASun.EquatorialRectangularCoordinatesB1950(2448908.5);
	  var SunCoord5 = CAASun.EquatorialRectangularCoordinatesAnyEquinox(2448908.5, 2467616.0);

	  //Test out the CAAMars class
	  var MarsLong = CAAMars.EclipticLongitude(2448935.500683);
	  var MarsLat = CAAMars.EclipticLatitude(2448935.500683);
	  var MarsRadius = CAAMars.RadiusVector(2448935.500683);

	  //Test out the CAAJupiter class
	  var JupiterLong = CAAJupiter.EclipticLongitude(2448972.50068);
	  var JupiterLat = CAAJupiter.EclipticLatitude(2448972.50068);
	  var JupiterRadius = CAAJupiter.RadiusVector(2448972.50068);

	  //Test out the CAANeptune class
	  var NeptuneLong = CAANeptune.EclipticLongitude(2448935.500683);
	  var NeptuneLat = CAANeptune.EclipticLatitude(2448935.500683);
	  var NeptuneRadius = CAANeptune.RadiusVector(2448935.500683);

	  //Test out the CAAUranus class
	  var UranusLong = CAAUranus.EclipticLongitude(2448976.5);
	  var UranusLat = CAAUranus.EclipticLatitude(2448976.5);
	  var UranusRadius = CAAUranus.RadiusVector(2448976.5);

	  //Test out the CAASaturn class
	  var SaturnLong = CAASaturn.EclipticLongitude(2448972.50068);
	  var SaturnLat = CAASaturn.EclipticLatitude(2448972.50068);
	  var SaturnRadius = CAASaturn.RadiusVector(2448972.50068);

	  //Test out the CAAPluto class
	  var PlutoLong = CAAPluto.EclipticLongitude(2448908.5);
	  var PlutoLat = CAAPluto.EclipticLatitude(2448908.5);
	  var PlutoRadius = CAAPluto.RadiusVector(2448908.5);

	  //Test out the CAAMoon class
	  var MoonLong = CAAMoon.EclipticLongitude(2448724.5);
	  var MoonLat = CAAMoon.EclipticLatitude(2448724.5);
	  var MoonRadius = CAAMoon.RadiusVector(2448724.5);
	  var MoonParallax = CAAMoon.RadiusVectorToHorizontalParallax(MoonRadius);
	  var MoonMeanAscendingNode = CAAMoon.MeanLongitudeAscendingNode(2448724.5);
	  var TrueMeanAscendingNode = CAAMoon.TrueLongitudeAscendingNode(2448724.5);
	  var MoonMeanPerigee = CAAMoon.MeanLongitudePerigee(2448724.5);

	  //Test out the CAAPlanetPerihelionAphelion class
	  var VenusK = CAAPlanetPerihelionAphelion.VenusK(1978.79);
	  var VenusPerihelion = CAAPlanetPerihelionAphelion.VenusPerihelion(VenusK);

	  var MarsK = CAAPlanetPerihelionAphelion.MarsK(2032);
	  var MarsAphelion = CAAPlanetPerihelionAphelion.MarsAphelion(MarsK);

	  var SaturnK = CAAPlanetPerihelionAphelion.SaturnK(1925);
	  var SaturnAphelion = CAAPlanetPerihelionAphelion.SaturnAphelion(SaturnK);
	  SaturnK = CAAPlanetPerihelionAphelion.SaturnK(1940);
	  var SaturnPerihelion = CAAPlanetPerihelionAphelion.SaturnPerihelion(SaturnK);

	  var UranusK = CAAPlanetPerihelionAphelion.UranusK(1750);
	  var UranusAphelion = CAAPlanetPerihelionAphelion.UranusAphelion(UranusK);
	  UranusK = CAAPlanetPerihelionAphelion.UranusK(1890);
	  var UranusPerihelion = CAAPlanetPerihelionAphelion.UranusPerihelion(UranusK);
	  UranusK = CAAPlanetPerihelionAphelion.UranusK(2060);
	  UranusPerihelion = CAAPlanetPerihelionAphelion.UranusPerihelion(UranusK);

	  var EarthPerihelion = CAAPlanetPerihelionAphelion.EarthPerihelion(-10, true);
	  var EarthPerihelion2 = CAAPlanetPerihelionAphelion.EarthPerihelion(-10, false);


	  //Test out the CAAMoonPerigeeApogee
	  var MoonK = CAAMoonPerigeeApogee.K(1988.75);
	  var MoonApogee = CAAMoonPerigeeApogee.MeanApogee(-148.5);
	  var MoonApogee2 = CAAMoonPerigeeApogee.TrueApogee(-148.5);
	  var MoonApogeeParallax = CAAMoonPerigeeApogee.ApogeeParallax(-148.5);
	  var MoonApogeeDistance = CAAMoon.HorizontalParallaxToRadiusVector(MoonApogeeParallax);

	  MoonK = CAAMoonPerigeeApogee.K(1990.9);
	  var MoonPerigee = CAAMoonPerigeeApogee.MeanPerigee(-120);
	  var MoonPerigee2 = CAAMoonPerigeeApogee.TruePerigee(-120);
	  MoonK = CAAMoonPerigeeApogee.K(1930.0);
	  var MoonPerigee3 = CAAMoonPerigeeApogee.TruePerigee(-927);
	  var MoonPerigeeParallax = CAAMoonPerigeeApogee.PerigeeParallax(-927);
	  var MoonRadiusVector = CAAMoon.HorizontalParallaxToRadiusVector(MoonPerigeeParallax);
	  var MoonRadiusVector2 = CAAMoon.HorizontalParallaxToRadiusVector(0.991990);
	  var MoonParallax2 = CAAMoon.RadiusVectorToHorizontalParallax(MoonRadiusVector2);

	  //Test out the CAAEclipticalElements class
	  var ed1 = CAAEclipticalElements.Calculate(47.1220, 151.4486, 45.7481, 2358042.5305, 2433282.4235);
	  var ed2 = CAAEclipticalElements.Calculate(11.93911, 186.24444, 334.04096, 2433282.4235, 2451545.0);
	  var ed3 = CAAEclipticalElements.FK4B1950ToFK5J2000(11.93911, 186.24444, 334.04096);
	  var ed4 = CAAEclipticalElements.FK4B1950ToFK5J2000(145, 186.24444, 334.04096);


	  //Test out the CAAEquationOfTime class
	  var E = CAAEquationOfTime.Calculate(2448908.5);


	  //Test out the CAAPhysicalSun class
	  var psd = CAAPhysicalSun.Calculate(2448908.50068);
	  var JED = CAAPhysicalSun.TimeOfStartOfRotation(1699);


	  //Test out the CAAEquinoxesAndSolstices class
	  var JuneSolstice = CAAEquinoxesAndSolstices.SummerSolstice(1962);


	  var MarchEquinox2 = CAAEquinoxesAndSolstices.SpringEquinox(1996);
	  date.Set(MarchEquinox2, true);
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);
	  var JuneSolstice2 = CAAEquinoxesAndSolstices.SummerSolstice(1996);
	  date.Set(JuneSolstice2, true);
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);
	  var SeptemberEquinox2 = CAAEquinoxesAndSolstices.AutumnEquinox(1996);
	  date.Set(SeptemberEquinox2, true);
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);
	  var DecemberSolstice2 = CAAEquinoxesAndSolstices.WinterSolstice(1996);
	  date.Set(DecemberSolstice2, true);
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);

	  DecemberSolstice2 = CAAEquinoxesAndSolstices.WinterSolstice(2000);
	  date.Set(DecemberSolstice2, true);
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);

	  DecemberSolstice2 = CAAEquinoxesAndSolstices.WinterSolstice(1997);
	  date.Set(DecemberSolstice2, true);
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);

	  DecemberSolstice2 = CAAEquinoxesAndSolstices.WinterSolstice(2003);
	  date.Set(DecemberSolstice2, true);
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);

	  JuneSolstice2 = CAAEquinoxesAndSolstices.SummerSolstice(2003);
	  date.Set(JuneSolstice2, true);
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);

	  var SpringLength = CAAEquinoxesAndSolstices.LengthOfSpring(2000);
	  var SummerLength = CAAEquinoxesAndSolstices.LengthOfSummer(2000);
	  var AutumnLength = CAAEquinoxesAndSolstices.LengthOfAutumn(2000);
	  var WinterLength = CAAEquinoxesAndSolstices.LengthOfWinter(2000);

	  SpringLength = CAAEquinoxesAndSolstices.LengthOfSpring(-2000);
	  SummerLength = CAAEquinoxesAndSolstices.LengthOfSummer(-2000);
	  AutumnLength = CAAEquinoxesAndSolstices.LengthOfAutumn(-2000);
	  WinterLength = CAAEquinoxesAndSolstices.LengthOfWinter(-2000);

	  SpringLength = CAAEquinoxesAndSolstices.LengthOfSpring(4000);
	  SummerLength = CAAEquinoxesAndSolstices.LengthOfSummer(4000);
	  AutumnLength = CAAEquinoxesAndSolstices.LengthOfAutumn(4000);
	  WinterLength = CAAEquinoxesAndSolstices.LengthOfWinter(4000);


	  //Test out the CAAElementsPlanetaryOrbit class
	  var Mer_L = CAAElementsPlanetaryOrbit.MercuryMeanLongitude(2475460.5);
	  var Mer_a = CAAElementsPlanetaryOrbit.MercurySemimajorAxis(2475460.5);
	  var Mer_e = CAAElementsPlanetaryOrbit.MercuryEccentricity(2475460.5);
	  var Mer_i = CAAElementsPlanetaryOrbit.MercuryInclination(2475460.5);
	  var Mer_omega = CAAElementsPlanetaryOrbit.MercuryLongitudeAscendingNode(2475460.5);
	  var mer_pi = CAAElementsPlanetaryOrbit.MercuryLongitudePerihelion(2475460.5);

	  var Ven_L = CAAElementsPlanetaryOrbit.VenusMeanLongitude(2475460.5);
	  var Ven_a = CAAElementsPlanetaryOrbit.VenusSemimajorAxis(2475460.5);
	  var Ven_e = CAAElementsPlanetaryOrbit.VenusEccentricity(2475460.5);
	  var Ven_i = CAAElementsPlanetaryOrbit.VenusInclination(2475460.5);
	  var Ven_omega = CAAElementsPlanetaryOrbit.VenusLongitudeAscendingNode(2475460.5);
	  var Ven_pi = CAAElementsPlanetaryOrbit.VenusLongitudePerihelion(2475460.5);

	  var Ea_L = CAAElementsPlanetaryOrbit.EarthMeanLongitude(2475460.5);
	  var Ea_a = CAAElementsPlanetaryOrbit.EarthSemimajorAxis(2475460.5);
	  var Ea_e = CAAElementsPlanetaryOrbit.EarthEccentricity(2475460.5);
	  var Ea_i = CAAElementsPlanetaryOrbit.EarthInclination(2475460.5);
	  var Ea_pi = CAAElementsPlanetaryOrbit.EarthLongitudePerihelion(2475460.5);

	  var Mars_L = CAAElementsPlanetaryOrbit.MarsMeanLongitude(2475460.5);
	  var Mars_a = CAAElementsPlanetaryOrbit.MarsSemimajorAxis(2475460.5);
	  var Mars_e = CAAElementsPlanetaryOrbit.MarsEccentricity(2475460.5);
	  var Mars_i = CAAElementsPlanetaryOrbit.MarsInclination(2475460.5);
	  var Mars_omega = CAAElementsPlanetaryOrbit.MarsLongitudeAscendingNode(2475460.5);
	  var Mars_pi = CAAElementsPlanetaryOrbit.MarsLongitudePerihelion(2475460.5);

	  var Jup_L = CAAElementsPlanetaryOrbit.JupiterMeanLongitude(2475460.5);
	  var Jup_a = CAAElementsPlanetaryOrbit.JupiterSemimajorAxis(2475460.5);
	  var Jup_e = CAAElementsPlanetaryOrbit.JupiterEccentricity(2475460.5);
	  var Jup_i = CAAElementsPlanetaryOrbit.JupiterInclination(2475460.5);
	  var Jup_omega = CAAElementsPlanetaryOrbit.JupiterLongitudeAscendingNode(2475460.5);
	  var Jup_pi = CAAElementsPlanetaryOrbit.JupiterLongitudePerihelion(2475460.5);

	  var Sat_L = CAAElementsPlanetaryOrbit.SaturnMeanLongitude(2475460.5);
	  var Sat_a = CAAElementsPlanetaryOrbit.SaturnSemimajorAxis(2475460.5);
	  var Sat_e = CAAElementsPlanetaryOrbit.SaturnEccentricity(2475460.5);
	  var Sat_i = CAAElementsPlanetaryOrbit.SaturnInclination(2475460.5);
	  var Sat_omega = CAAElementsPlanetaryOrbit.SaturnLongitudeAscendingNode(2475460.5);
	  var Sat_pi = CAAElementsPlanetaryOrbit.SaturnLongitudePerihelion(2475460.5);

	  var Ura_L = CAAElementsPlanetaryOrbit.UranusMeanLongitude(2475460.5);
	  var Ura_a = CAAElementsPlanetaryOrbit.UranusSemimajorAxis(2475460.5);
	  var Ura_e = CAAElementsPlanetaryOrbit.UranusEccentricity(2475460.5);
	  var Ura_i = CAAElementsPlanetaryOrbit.UranusInclination(2475460.5);
	  var Ura_omega = CAAElementsPlanetaryOrbit.UranusLongitudeAscendingNode(2475460.5);
	  var Ura_pi = CAAElementsPlanetaryOrbit.UranusLongitudePerihelion(2475460.5);

	  var Nep_L = CAAElementsPlanetaryOrbit.NeptuneMeanLongitude(2475460.5);
	  var Nep_a = CAAElementsPlanetaryOrbit.NeptuneSemimajorAxis(2475460.5);
	  var Nep_e = CAAElementsPlanetaryOrbit.NeptuneEccentricity(2475460.5);
	  var Nep_i = CAAElementsPlanetaryOrbit.NeptuneInclination(2475460.5);
	  var Nep_omega = CAAElementsPlanetaryOrbit.NeptuneLongitudeAscendingNode(2475460.5);
	  var Nep_pi = CAAElementsPlanetaryOrbit.NeptuneLongitudePerihelion(2475460.5);


	  var Mer_L2 = CAAElementsPlanetaryOrbit.MercuryMeanLongitudeJ2000(2475460.5);
	  var Mer_i2 = CAAElementsPlanetaryOrbit.MercuryInclinationJ2000(2475460.5);
	  var Mer_omega2 = CAAElementsPlanetaryOrbit.MercuryLongitudeAscendingNodeJ2000(2475460.5);
	  var mer_pi2 = CAAElementsPlanetaryOrbit.MercuryLongitudePerihelionJ2000(2475460.5);

	  var Ven_L2 = CAAElementsPlanetaryOrbit.VenusMeanLongitudeJ2000(2475460.5);
	  var Ven_i2 = CAAElementsPlanetaryOrbit.VenusInclinationJ2000(2475460.5);
	  var Ven_omega2 = CAAElementsPlanetaryOrbit.VenusLongitudeAscendingNodeJ2000(2475460.5);
	  var Ven_pi2 = CAAElementsPlanetaryOrbit.VenusLongitudePerihelionJ2000(2475460.5);

	  var Ea_L2 = CAAElementsPlanetaryOrbit.EarthMeanLongitudeJ2000(2475460.5);
	  var Ea_i2 = CAAElementsPlanetaryOrbit.EarthInclinationJ2000(2475460.5);
	  var Ea_omega2 = CAAElementsPlanetaryOrbit.EarthLongitudeAscendingNodeJ2000(2475460.5);
	  var Ea_pi2 = CAAElementsPlanetaryOrbit.EarthLongitudePerihelionJ2000(2475460.5);

	  var Mars_L2 = CAAElementsPlanetaryOrbit.MarsMeanLongitudeJ2000(2475460.5);
	  var Mars_i2 = CAAElementsPlanetaryOrbit.MarsInclinationJ2000(2475460.5);
	  var Mars_omega2 = CAAElementsPlanetaryOrbit.MarsLongitudeAscendingNodeJ2000(2475460.5);
	  var Mars_pi2 = CAAElementsPlanetaryOrbit.MarsLongitudePerihelionJ2000(2475460.5);

	  var Jup_L2 = CAAElementsPlanetaryOrbit.JupiterMeanLongitudeJ2000(2475460.5);
	  var Jup_i2 = CAAElementsPlanetaryOrbit.JupiterInclinationJ2000(2475460.5);
	  var Jup_omega2 = CAAElementsPlanetaryOrbit.JupiterLongitudeAscendingNodeJ2000(2475460.5);
	  var Jup_pi2 = CAAElementsPlanetaryOrbit.JupiterLongitudePerihelionJ2000(2475460.5);

	  var Sat_L2 = CAAElementsPlanetaryOrbit.SaturnMeanLongitudeJ2000(2475460.5);
	  var Sat_i2 = CAAElementsPlanetaryOrbit.SaturnInclinationJ2000(2475460.5);
	  var Sat_omega2 = CAAElementsPlanetaryOrbit.SaturnLongitudeAscendingNodeJ2000(2475460.5);
	  var Sat_pi2 = CAAElementsPlanetaryOrbit.SaturnLongitudePerihelionJ2000(2475460.5);

	  var Ura_L2 = CAAElementsPlanetaryOrbit.UranusMeanLongitudeJ2000(2475460.5);
	  var Ura_i2 = CAAElementsPlanetaryOrbit.UranusInclinationJ2000(2475460.5);
	  var Ura_omega2 = CAAElementsPlanetaryOrbit.UranusLongitudeAscendingNodeJ2000(2475460.5);
	  var Ura_pi2 = CAAElementsPlanetaryOrbit.UranusLongitudePerihelionJ2000(2475460.5);

	  var Nep_L2 = CAAElementsPlanetaryOrbit.NeptuneMeanLongitudeJ2000(2475460.5);
	  var Nep_i2 = CAAElementsPlanetaryOrbit.NeptuneInclinationJ2000(2475460.5);
	  var Nep_omega2 = CAAElementsPlanetaryOrbit.NeptuneLongitudeAscendingNodeJ2000(2475460.5);
	  var Nep_pi2 = CAAElementsPlanetaryOrbit.NeptuneLongitudePerihelionJ2000(2475460.5);

	  var MoonGeocentricElongation = CAAMoonIlluminatedFraction.GeocentricElongation(8.97922, 13.7684, 1.377194, 8.6964);
	  var MoonPhaseAngle = CAAMoonIlluminatedFraction.PhaseAngle(MoonGeocentricElongation, 368410, 149971520);
	  var MoonIlluminatedFraction = CAAMoonIlluminatedFraction.IlluminatedFraction(MoonPhaseAngle);
	  var MoonPositionAngle = CAAMoonIlluminatedFraction.PositionAngle(CAACoordinateTransformation.DMSToDegrees(1, 22, 37.9), 8.6964, 134.6885/15, 13.7684);

	  var VenusDetails = CAAElliptical.Calculate(2448976.5, EllipticalObject.VENUS);

	  var SunDetails = CAAElliptical.Calculate(2453149.5, EllipticalObject.SUN);

	  var elements = new CAAEllipticalObjectElements();
	  elements.a = 2.2091404;
	  elements.e = 0.8502196;
	  elements.i = 11.94524;
	  elements.omega = 334.75006;
	  elements.w = 186.23352;
	  elements.T = 2448192.5 + 0.54502;
	  elements.JDEquinox = 2451544.5;
	  var details = CAAElliptical.Calculate(2448170.5, elements);

	  var Velocity1 = CAAElliptical.InstantaneousVelocity(1, 17.9400782);
	  var Velocity2 = CAAElliptical.VelocityAtPerihelion(0.96727426, 17.9400782);
	  var Velocity3 = CAAElliptical.VelocityAtAphelion(0.96727426, 17.9400782);

	  var Length = CAAElliptical.LengthOfEllipse(0.96727426, 17.9400782);

	  var Mag1 = CAAElliptical.MinorPlanetMagnitude(3.34, 1.6906631928, 0.12, 2.6154983761, 120);
	  var Mag2 = CAAElliptical.CometMagnitude(5.5, 0.378, 10, 0.658);
	  var Mag3 = CAAElliptical.CometMagnitude(5.5, 1.1017, 10, 1.5228);

	  var elements2 = new CAAParabolicObjectElements();
	  elements2.q = 1.487469;
	  elements2.i = 0; //unknown
	  elements2.omega = 0; //unknown
	  elements2.w = 0; //unknown
	  elements2.T = 2450917.9358;
	  elements2.JDEquinox = 2451030.5;
	  var details2 = CAAParabolic.Calculate(2451030.5, elements2);


	  var elements3 = new CAAEllipticalObjectElements();
	  elements3.a = 17.9400782;
	  elements3.e = 0.96727426;
	  elements3.i = 0; //Not required
	  elements3.omega = 0; //Not required
	  elements3.w = 111.84644;
	  elements3.T = 2446470.5 + 0.45891;
	  elements3.JDEquinox = 0; //Not required
	  var nodedetails = CAANodes.PassageThroAscendingNode(elements3);
	  var nodedetails2 = CAANodes.PassageThroDescendingNode(elements3);

	  var elements4 = new CAAParabolicObjectElements();
	  elements4.q = 1.324502;
	  elements4.i = 0; //Not required
	  elements4.omega = 0; //Not required
	  elements4.w = 154.9103;
	  elements4.T = 2447758.5 + 0.2910;
	  elements4.JDEquinox = 0; //Not required
	  var nodedetails3 = CAANodes.PassageThroAscendingNode(elements4);
	  var nodedetails4 = CAANodes.PassageThroDescendingNode(elements4);


	  var elements5 = new CAAEllipticalObjectElements();
	  elements5.a = 0.723329820;
	  elements5.e = 0.00678195;
	  elements5.i = 0; //Not required
	  elements5.omega = 0; //Not required
	  elements5.w = 54.778485;
	  elements5.T = 2443873.704;
	  elements5.JDEquinox = 0; //Not required
	  var nodedetails5 = CAANodes.PassageThroAscendingNode(elements5);

	  var MoonK2 = CAAMoonNodes.K(1987.37);
	  var MoonJD = CAAMoonNodes.PassageThroNode(-170);


	  var Y = CAAInterpolate.Interpolate(0.18125, 0.884226, 0.877366, 0.870531);

	  double NM = 0;
	  var YM = CAAInterpolate.Extremum(1.3814294, 1.3812213, 1.3812453, ref NM);

	  var N0 = CAAInterpolate.Zero(-1693.4, 406.3, 2303.2);

	  var N02 = CAAInterpolate.Zero2(-2, 3, 2);

	  var Y2 = CAAInterpolate.Interpolate(0.2777778, 36.125, 24.606, 15.486, 8.694, 4.133);

	  var N03 = CAAInterpolate.Zero(CAACoordinateTransformation.DMSToDegrees(1, 11, 21.23, false), CAACoordinateTransformation.DMSToDegrees(0, 28, 12.31, false), CAACoordinateTransformation.DMSToDegrees(0, 16, 7.02), CAACoordinateTransformation.DMSToDegrees(1, 1, 0.13), CAACoordinateTransformation.DMSToDegrees(1, 45, 46.33));

	  var N04 = CAAInterpolate.Zero(CAACoordinateTransformation.DMSToDegrees(0, 28, 12.31, false), CAACoordinateTransformation.DMSToDegrees(0, 16, 7.02), CAACoordinateTransformation.DMSToDegrees(1, 1, 0.13));

	  var N05 = CAAInterpolate.Zero2(-13, -2, 3, 2, -5);

	  var Y3 = CAAInterpolate.InterpolateToHalves(1128.732, 1402.835, 1677.247, 1951.983);

	  double[] X1 = { 29.43, 30.97, 27.69, 28.11, 31.58, 33.05 };
	  double[] Y1 = { 0.4913598528, 0.5145891926, 0.4646875083, 0.4711658342, 0.5236885653, 0.5453707057 };
	  var Y4 = CAAInterpolate.LagrangeInterpolate(30, 6,  X1,  Y1);
	  var Y5 = CAAInterpolate.LagrangeInterpolate(0, 6,  X1,  Y1);
	  var Y6 = CAAInterpolate.LagrangeInterpolate(90, 6,  X1,  Y1);


	  var Alpha1 = CAACoordinateTransformation.DMSToDegrees(2, 42, 43.25);
	  var Alpha2 = CAACoordinateTransformation.DMSToDegrees(2, 46, 55.51);
	//C++ TO C# CONVERTER TODO TASK: Octal literals cannot be represented in C#:
	  var Alpha3 = CAACoordinateTransformation.DMSToDegrees(2, 51, 07.69);

	//C++ TO C# CONVERTER TODO TASK: Octal literals cannot be represented in C#:
	  var Delta1 = CAACoordinateTransformation.DMSToDegrees(18, 02, 51.4);
	  var Delta2 = CAACoordinateTransformation.DMSToDegrees(18, 26, 27.3);
	  var Delta3 = CAACoordinateTransformation.DMSToDegrees(18, 49, 38.7);

	  var RiseTransitSetTime = CAARiseTransitSet.Rise(2447240.5, Alpha1, Delta1, Alpha2, Delta2, Alpha3, Delta3, 71.0833, 42.3333, -0.5667);

	  var Kpp = CAAPlanetaryPhenomena.K(1993.75, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);
      var MercuryInferiorConjunction = CAAPlanetaryPhenomena.Mean(Kpp, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);
      var MercuryInferiorConjunction2 = CAAPlanetaryPhenomena.True(Kpp, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);

      var Kpp2 = CAAPlanetaryPhenomena.K(2125.5, CAAPlanetaryPhenomena.PlanetaryObject.SATURN, CAAPlanetaryPhenomena.EventType.CONJUNCTION);
      var SaturnConjunction = CAAPlanetaryPhenomena.Mean(Kpp2, CAAPlanetaryPhenomena.PlanetaryObject.SATURN, CAAPlanetaryPhenomena.EventType.CONJUNCTION);
      var SaturnConjunction2 = CAAPlanetaryPhenomena.True(Kpp2, CAAPlanetaryPhenomena.PlanetaryObject.SATURN, CAAPlanetaryPhenomena.EventType.CONJUNCTION);

      var MercuryWesternElongation = CAAPlanetaryPhenomena.True(Kpp, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.WESTERN_ELONGATION);
      var MercuryWesternElongationValue = CAAPlanetaryPhenomena.ElongationValue(Kpp, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, false);

      var MarsStation2 = CAAPlanetaryPhenomena.True(-2, CAAPlanetaryPhenomena.PlanetaryObject.MARS, CAAPlanetaryPhenomena.EventType.STATION2);

      var MercuryK = CAAPlanetaryPhenomena.K(1631.8, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);
      var MercuryIC = CAAPlanetaryPhenomena.True(MercuryK, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);

      var VenusKpp = CAAPlanetaryPhenomena.K(1882.9, CAAPlanetaryPhenomena.PlanetaryObject.VENUS, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);
      var VenusIC = CAAPlanetaryPhenomena.True(VenusKpp, CAAPlanetaryPhenomena.PlanetaryObject.VENUS, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);

      var MarsKpp = CAAPlanetaryPhenomena.K(2729.65, CAAPlanetaryPhenomena.PlanetaryObject.MARS, CAAPlanetaryPhenomena.EventType.OPPOSITION);
      var MarsOP = CAAPlanetaryPhenomena.True(MarsKpp, CAAPlanetaryPhenomena.PlanetaryObject.MARS, CAAPlanetaryPhenomena.EventType.OPPOSITION);

      var JupiterKpp = CAAPlanetaryPhenomena.K(-5, CAAPlanetaryPhenomena.PlanetaryObject.JUPITER, CAAPlanetaryPhenomena.EventType.OPPOSITION);
      var JupiterOP = CAAPlanetaryPhenomena.True(JupiterKpp, CAAPlanetaryPhenomena.PlanetaryObject.JUPITER, CAAPlanetaryPhenomena.EventType.OPPOSITION);

      var SaturnKpp = CAAPlanetaryPhenomena.K(-5, CAAPlanetaryPhenomena.PlanetaryObject.SATURN, CAAPlanetaryPhenomena.EventType.OPPOSITION);
      var SaturnOP = CAAPlanetaryPhenomena.True(SaturnKpp, CAAPlanetaryPhenomena.PlanetaryObject.SATURN, CAAPlanetaryPhenomena.EventType.OPPOSITION);

      var UranusKpp = CAAPlanetaryPhenomena.K(1780.6, CAAPlanetaryPhenomena.PlanetaryObject.URANUS, CAAPlanetaryPhenomena.EventType.OPPOSITION);
      var UranusOP = CAAPlanetaryPhenomena.True(UranusKpp, CAAPlanetaryPhenomena.PlanetaryObject.URANUS, CAAPlanetaryPhenomena.EventType.OPPOSITION);

      var NeptuneKpp = CAAPlanetaryPhenomena.K(1846.5, CAAPlanetaryPhenomena.PlanetaryObject.NEPTUNE, CAAPlanetaryPhenomena.EventType.OPPOSITION);
      var NeptuneOP = CAAPlanetaryPhenomena.True(NeptuneKpp, CAAPlanetaryPhenomena.PlanetaryObject.NEPTUNE, CAAPlanetaryPhenomena.EventType.OPPOSITION);

	  var TopocentricDelta = CAAParallax.Equatorial2TopocentricDelta(CAACoordinateTransformation.DMSToDegrees(22, 38, 7.25), -15.771083, 0.37276, CAACoordinateTransformation.DMSToDegrees(7, 47, 27)*15, CAACoordinateTransformation.DMSToDegrees(33, 21, 22), 1706, 2452879.63681);
	  var Topocentric = CAAParallax.Equatorial2Topocentric(CAACoordinateTransformation.DMSToDegrees(22, 38, 7.25), -15.771083, 0.37276, CAACoordinateTransformation.DMSToDegrees(7, 47, 27)*15, CAACoordinateTransformation.DMSToDegrees(33, 21, 22), 1706, 2452879.63681);


	  var distance2 = CAAParallax.ParallaxToDistance(CAACoordinateTransformation.DMSToDegrees(0, 59, 27.7));
	  var parallax2 = CAAParallax.DistanceToParallax(distance2);

	  var TopocentricDetails = CAAParallax.Ecliptic2Topocentric(CAACoordinateTransformation.DMSToDegrees(181, 46, 22.5), CAACoordinateTransformation.DMSToDegrees(2, 17, 26.2), CAACoordinateTransformation.DMSToDegrees(0, 16, 15.5), CAAParallax.ParallaxToDistance(CAACoordinateTransformation.DMSToDegrees(0, 59, 27.7)), CAACoordinateTransformation.DMSToDegrees(23, 28, 0.8), 0, CAACoordinateTransformation.DMSToDegrees(50, 5, 7.8), 0, 2452879.150858);

	  var k = CAAIlluminatedFraction.IlluminatedFraction(0.724604, 0.983824, 0.910947);
	  var pa1 = CAAIlluminatedFraction.PhaseAngle(0.724604, 0.983824, 0.910947);
	  var pa = CAAIlluminatedFraction.PhaseAngle(0.724604, 0.983824, -2.62070, 26.11428, 88.35704, 0.910947);
	  var k2 = CAAIlluminatedFraction.IlluminatedFraction(pa);
	  var pa2 = CAAIlluminatedFraction.PhaseAngleRectangular(0.621746, -0.664810, -0.033134, -2.62070, 26.11428, 0.910947);
	  var k3 = CAAIlluminatedFraction.IlluminatedFraction(pa2);

	  var VenusMag = CAAIlluminatedFraction.VenusMagnitudeMuller(0.724604, 0.910947, 72.96);
	  var VenusMag2 = CAAIlluminatedFraction.VenusMagnitudeAA(0.724604, 0.910947, 72.96);

	  var SaturnMag = CAAIlluminatedFraction.SaturnMagnitudeMuller(9.867882, 10.464606, 4.198, 16.442);
	  var SaturnMag2 = CAAIlluminatedFraction.SaturnMagnitudeAA(9.867882, 10.464606, 4.198, 16.442);


	  var MarsDetails = CAAPhysicalMars.Calculate(2448935.500683);

	  var JupiterDetails = CAAPhysicalJupiter.Calculate(2448972.50068);

	  //The example as given in the book
	  var GalileanDetails = CAAGalileanMoons.Calculate(2448972.50068);

	  //Calculate the Eclipse Disappearance of Satellite 1 on February 1 2004 at 13:32 UCT
	  var JD = 2453037.05903;
	  int i;
	  for (i =0; i<10; i++)
	  {
		var GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  //Calculate the Shadow Egress of Satellite 1 on February 2  2004 at 13:07 UT
	  JD = 2453038.04236;
	  for (i =0; i<10; i++)
	  {
		var GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  //Calculate the Shadow Ingress of Satellite 4 on February 6 2004 at 22:59 UCT
	  JD = 2453042.45486;
	  for (i =0; i<10; i++)
	  {
		var GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  //Calculate the Shadow Egress of Satellite 4 on February 7 2004 at 2:41 UCT
	  JD = 2453042.61042;
	  for (i =0; i<10; i++)
	  {
		var GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  //Calculate the Transit Ingress of Satellite 4 on February 7 2004 at 5:07 UCT
	  JD = 2453042.71181;
	  for (i =0; i<10; i++)
	  {
		var GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  //Calculate the Transit Egress of Satellite 4 on February 7 2004 at 7:46 UT
	  JD = 2453042.82222;
	  for (i =0; i<10; i++)
	  {
		var GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  var saturnrings = CAASaturnRings.Calculate(2448972.50068);

	  var saturnMoons = CAASaturnMoons.Calculate(2451439.50074);

	  var ApproxK = CAAMoonPhases.K(1977.13);
	  var NewMoonJD = CAAMoonPhases.TruePhase(-283);

	  var ApproxK2 = CAAMoonPhases.K(2044);
	  var LastQuarterJD = CAAMoonPhases.TruePhase(544.75);

	  var MoonDeclinationK = CAAMoonMaxDeclinations.K(1988.95);

	  var MoonNorthDec = CAAMoonMaxDeclinations.TrueGreatestDeclination(-148, true);
	  var MoonNorthDecValue = CAAMoonMaxDeclinations.TrueGreatestDeclinationValue(-148, true);

	  var MoonSouthDec = CAAMoonMaxDeclinations.TrueGreatestDeclination(659, false);
	  var MoonSouthDecValue = CAAMoonMaxDeclinations.TrueGreatestDeclinationValue(659, false);

	  var MoonNorthDec2 = CAAMoonMaxDeclinations.TrueGreatestDeclination(-26788, true);
	  var MoonNorthDecValue2 = CAAMoonMaxDeclinations.TrueGreatestDeclinationValue(-26788, true);

	  var sd1 = CAADiameters.SunSemidiameterA(1);
	  var sd2 = CAADiameters.SunSemidiameterA(2);

	  var sd3 = CAADiameters.VenusSemidiameterA(1);
	  var sd4 = CAADiameters.VenusSemidiameterA(2);
	  var sd5 = CAADiameters.VenusSemidiameterB(1);
	  var sd6 = CAADiameters.VenusSemidiameterB(2);

	  var sd11 = CAADiameters.MarsSemidiameterA(1);
	  var sd12 = CAADiameters.MarsSemidiameterA(2);
	  var sd13 = CAADiameters.MarsSemidiameterB(1);
	  var sd14 = CAADiameters.MarsSemidiameterB(2);

	  var sd15 = CAADiameters.JupiterEquatorialSemidiameterA(1);
	  var sd16 = CAADiameters.JupiterEquatorialSemidiameterA(2);
	  var sd17 = CAADiameters.JupiterEquatorialSemidiameterB(1);
	  var sd18 = CAADiameters.JupiterEquatorialSemidiameterB(2);

	  var sd19 = CAADiameters.JupiterPolarSemidiameterA(1);
	  var sd20 = CAADiameters.JupiterPolarSemidiameterA(2);
	  var sd21 = CAADiameters.JupiterPolarSemidiameterB(1);
	  var sd22 = CAADiameters.JupiterPolarSemidiameterB(2);

	  var sd23 = CAADiameters.SaturnEquatorialSemidiameterA(1);
	  var sd24 = CAADiameters.SaturnEquatorialSemidiameterA(2);
	  var sd25 = CAADiameters.SaturnEquatorialSemidiameterB(1);
	  var sd26 = CAADiameters.SaturnEquatorialSemidiameterB(2);

	  var sd27 = CAADiameters.SaturnPolarSemidiameterA(1);
	  var sd28 = CAADiameters.SaturnPolarSemidiameterA(2);
	  var sd29 = CAADiameters.SaturnPolarSemidiameterB(1);
	  var sd30 = CAADiameters.SaturnPolarSemidiameterB(2);

	  var sd31 = CAADiameters.ApparentSaturnPolarSemidiameterA(1, 16.442);
	  var sd32 = CAADiameters.ApparentSaturnPolarSemidiameterA(2, 16.442);

	  var sd33 = CAADiameters.UranusSemidiameterA(1);
	  var sd34 = CAADiameters.UranusSemidiameterA(2);
	  var sd35 = CAADiameters.UranusSemidiameterB(1);
	  var sd36 = CAADiameters.UranusSemidiameterB(2);

	  var sd37 = CAADiameters.NeptuneSemidiameterA(1);
	  var sd38 = CAADiameters.NeptuneSemidiameterA(2);
	  var sd39 = CAADiameters.NeptuneSemidiameterB(1);
	  var sd40 = CAADiameters.NeptuneSemidiameterB(2);

	  var sd41 = CAADiameters.PlutoSemidiameterB(1);
	  var sd42 = CAADiameters.PlutoSemidiameterB(2);

	  var sd43 = CAADiameters.GeocentricMoonSemidiameter(368407.9);
	  var sd44 = CAADiameters.GeocentricMoonSemidiameter(368407.9 - 10000);

	  var sd45 = CAADiameters.TopocentricMoonSemidiameter(368407.9, 5, 0, 33.356111, 1706);
	  var sd46 = CAADiameters.TopocentricMoonSemidiameter(368407.9, 5, 6, 33.356111, 1706);
	  var sd47 = CAADiameters.TopocentricMoonSemidiameter(368407.9 - 10000, 5, 0, 33.356111, 1706);
	  var sd48 = CAADiameters.TopocentricMoonSemidiameter(368407.9 - 10000, 5, 6, 33.356111, 1706);

	  var sd49 = CAADiameters.AsteroidDiameter(4, 0.04);
	  var sd50 = CAADiameters.AsteroidDiameter(4, 0.08);
	  var sd51 = CAADiameters.AsteroidDiameter(6, 0.04);
	  var sd53 = CAADiameters.AsteroidDiameter(6, 0.08);
	  var sd54 = CAADiameters.ApparentAsteroidDiameter(1, 250);
	  var sd55 = CAADiameters.ApparentAsteroidDiameter(1, 1000);

	  var MoonDetails = CAAPhysicalMoon.CalculateGeocentric(2448724.5);
	  var MoonDetail2 = CAAPhysicalMoon.CalculateTopocentric(2448724.5, 10, 52);
	  var CAASelenographicMoonDetails = CAAPhysicalMoon.CalculateSelenographicPositionOfSun(2448724.5);

	  var AltitudeOfSun = CAAPhysicalMoon.AltitudeOfSun(2448724.5, -20, 9.7);
	  var TimeOfSunrise = CAAPhysicalMoon.TimeOfSunrise(2448724.5, -20, 9.7);
	  var TimeOfSunset = CAAPhysicalMoon.TimeOfSunset(2448724.5, -20, 9.7);

	  var EclipseDetails = CAAEclipses.CalculateSolar(-82);
	  var EclipseDetails2 = CAAEclipses.CalculateSolar(118);
	  var EclipseDetails3 = CAAEclipses.CalculateLunar(-328.5);
	  var EclipseDetails4 = CAAEclipses.CalculateLunar(-30.5); //No lunar eclipse
	  EclipseDetails4 = CAAEclipses.CalculateLunar(-29.5); //No lunar eclipse
	  EclipseDetails4 = CAAEclipses.CalculateLunar(-28.5); //Aha, found you!

	  var JulianDate1 = CAAMoslemCalendar.MoslemToJulian(1421, 1, 1);
	  var GregorianDate1 = CAADate.JulianToGregorian(JulianDate1.Year, JulianDate1.Month, JulianDate1.Day);
	  var JulianDate2 = CAADate.GregorianToJulian(GregorianDate1.Year, GregorianDate1.Month, GregorianDate1.Day);
	  var MoslemDate = CAAMoslemCalendar.JulianToMoslem(JulianDate2.Year, JulianDate2.Month, JulianDate2.Day);
	  bLeap = CAAMoslemCalendar.IsLeap(1421);

	  MoslemDate = CAAMoslemCalendar.JulianToMoslem(2006, 12, 31);
	  var OriginalMoslemDate = CAAMoslemCalendar.MoslemToJulian(MoslemDate.Year, MoslemDate.Month, MoslemDate.Day);
	  MoslemDate = CAAMoslemCalendar.JulianToMoslem(2007, 1, 1);
	  OriginalMoslemDate = CAAMoslemCalendar.MoslemToJulian(MoslemDate.Year, MoslemDate.Month, MoslemDate.Day);

	  var JulianDate3 = CAADate.GregorianToJulian(1991, 8, 13);
	  var MoslemDate2 = CAAMoslemCalendar.JulianToMoslem(JulianDate3.Year, JulianDate3.Month, JulianDate3.Day);
	  var JulianDate4 = CAAMoslemCalendar.MoslemToJulian(MoslemDate2.Year, MoslemDate2.Month, MoslemDate2.Day);
	  var GregorianDate2 = CAADate.JulianToGregorian(JulianDate4.Year, JulianDate4.Month, JulianDate4.Day);

	  var JewishDate = CAAJewishCalendar.DateOfPesach(1990);
	  bLeap = CAAJewishCalendar.IsLeap(JewishDate.Year);
	  bLeap = CAAJewishCalendar.IsLeap(5751);
	  var DaysInJewishYear = CAAJewishCalendar.DaysInYear(JewishDate.Year);
	  DaysInJewishYear = CAAJewishCalendar.DaysInYear(5751);


	  var elements6 = new CAANearParabolicObjectElements();
	  elements6.q = 0.921326;
	  elements6.e = 1;
	  elements6.i = 0; //unknown
	  elements6.omega = 0; //unknown
	  elements6.w = 0; //unknown
	  elements6.T = 0;
	  elements6.JDEquinox = 0;
	  var details3 = CAANearParabolic.Calculate(138.4783, elements6);

	  elements6.q = 0.1;
	  elements6.e = 0.987;
	  details3 = CAANearParabolic.Calculate(254.9, elements6);

	  elements6.q = 0.123456;
	  elements6.e = 0.99997;
	  details3 = CAANearParabolic.Calculate(-30.47, elements6);

	  elements6.q = 3.363943;
	  elements6.e = 1.05731;
	  details3 = CAANearParabolic.Calculate(1237.1, elements6);

	  elements6.q = 0.5871018;
	  elements6.e = 0.9672746;
	  details3 = CAANearParabolic.Calculate(20, elements6);

	  details3 = CAANearParabolic.Calculate(0, elements6);

	  var ed5 = CAAEclipticalElements.Calculate(131.5856, 242.6797, 138.6637, 2433282.4235, 2448188.500000 + 0.6954-63.6954);
	  var ed6 = CAAEclipticalElements.Calculate(131.5856, 242.6797, 138.6637, 2433282.4235, 2433282.4235);
	  var ed7 = CAAEclipticalElements.FK4B1950ToFK5J2000(131.5856, 242.6797, 138.6637);

	  elements6.q = 0.93858;
	  elements6.e = 1.000270;
	  elements6.i = ed5.i;
	  elements6.omega = ed5.omega;
	  elements6.w = ed5.w;
	  elements6.T = 2448188.500000 + 0.6954;
	  elements6.JDEquinox = elements6.T;
	  var details4 = CAANearParabolic.Calculate(elements6.T-63.6954, elements6);

	  return 0;
	}

	#if _MSC_VER
	//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
	//#pragma warning(pop)
	#endif
}


#if _MSC_VER
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma warning(push) //We're not interested in unreferrenced variables in this test app
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma warning(disable : 4101)
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma warning(disable : 4189)
#endif
