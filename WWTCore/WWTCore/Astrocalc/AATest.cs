using System;
public static partial class GlobalMembersStdafx
{

#if !WINDOWS_UWP
    public static int mainTEST()
	{
        CAADate date;
        bool bLeap;
	  //Do a full round trip test on CAADate across a nice wide range. Note we should expect
	  //some printfs to appear during this test (Specifically a monotonic error for 15 October 1582
	  double prevJulian = -1;
	  for (int YYYY =-4712; YYYY<5000; YYYY++) //Change the end YYYY value if you would like to test a longer range
	  {
		if ((YYYY % 1000) == 0)
		  Console.Write("Doing date tests on year {0:D}\n", YYYY);
		for (int MMMM =1; MMMM<=12; MMMM++)
		{
		  bLeap = CAADate.IsLeap(YYYY, (YYYY >= 1582));
		  for (int DDDD =1; DDDD<=CAADate.DaysInMonth(MMMM, bLeap); DDDD++)
		  {
			bool bGregorian = CAADate.AfterPapalReform(YYYY, MMMM, DDDD);
			 date = new CAADate(YYYY, MMMM, DDDD, 12, 0, 0, bGregorian);
			if ((date.Year() != YYYY) || (date.Month() != MMMM)|| (date.Day() != DDDD))
			  Console.Write("Round trip bug with date {0:D}/{1:D}/{2:D}\n", YYYY, MMMM, DDDD);
			double currentJulian = date.Julian();
			if ((prevJulian != -1) && ((prevJulian + 1) != currentJulian))
			  Console.Write("Julian Day monotonic bug with date {0:D}/{1:D}/{2:D}\n", YYYY, MMMM, DDDD);
			prevJulian = currentJulian;

			//Only do round trip tests between the Julian and Gregorian calendars after the papal 
			//reform. This is because the CAADate class does not support the propalactic Gregorian 
			//calendar, while it does fully support the propalactic Julian calendar.
			if (bGregorian)
			{
			  CAACalendarDate GregorianDate = CAADate.JulianToGregorian(YYYY, MMMM, DDDD);
			  CAACalendarDate JulianDate = CAADate.GregorianToJulian(GregorianDate.Year, GregorianDate.Month, GregorianDate.Day);
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
	  int Year = 0;
	  int Month = 0;
	  int Day = 0;
	  int Hour = 0;
	  int Minute = 0;
	  double Second = 0;
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);
	  int DaysInMonth = date.DaysInMonth();
	  int DaysInYear = date.DaysInYear();
	  bLeap = date.Leap();
	  double Julian = date.Julian();
	  double FractionalYear = date.FractionalYear();
	  double DayOfYear = date.DayOfYear();
	  CAADate.DAY_OF_WEEK dow = date.DayOfWeek();
	  Year = date.Year();
	  Month = date.Month();
	  Day = date.Day();
	  Hour = date.Hour();
	  Minute = date.Minute();
	  Second = date.Second();
	  double Julian2 = date;

	  date.Set(1978, 11, 14, 0, 0, 0, true);
	  int DayNumber = (int)(date.DayOfYear());
	  CAADate.DayOfYearToDayAndMonth(DayNumber, date.Leap(), ref Day, ref Month);
	  Year = date.Year();

	  //Test out the AAEaster class
	  CAAEasterDetails easterDetails = CAAEaster.Calculate(1991, true);
	  CAAEasterDetails easterDetails2 = CAAEaster.Calculate(1818, true);
	  CAAEasterDetails easterDetails3 = CAAEaster.Calculate(179, false);

	  //Test out the AADynamicalTime class
	  date.Set(1977, 2, 18, 3, 37, 40, true);
	  double DeltaT = CAADynamicalTime.DeltaT(date.Julian());
	  date.Set(333, 2, 6, 6, 0, 0, false);
	  DeltaT = CAADynamicalTime.DeltaT(date.Julian());

	  //Test out the AAGlobe class
	  double rhosintheta = CAAGlobe.RhoSinThetaPrime(33.356111, 1706);
	  double rhocostheta = CAAGlobe.RhoCosThetaPrime(33.356111, 1706);
	  double RadiusOfLatitude = CAAGlobe.RadiusOfParallelOfLatitude(42);
	  double RadiusOfCurvature = CAAGlobe.RadiusOfCurvature(42);
	  double Distance = CAAGlobe.DistanceBetweenPoints(CAACoordinateTransformation.DMSToDegrees(48, 50, 11), CAACoordinateTransformation.DMSToDegrees(2, 20, 14, false), CAACoordinateTransformation.DMSToDegrees(38, 55, 17), CAACoordinateTransformation.DMSToDegrees(77, 3, 56));


	  double Distance1 = CAAGlobe.DistanceBetweenPoints(50, 0, 50, 60);
	  double Distance2 = CAAGlobe.DistanceBetweenPoints(50, 0, 50, 1);
	  double Distance3 = CAAGlobe.DistanceBetweenPoints(CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 0, CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 1);
	  double Distance4 = CAAGlobe.DistanceBetweenPoints(CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 0, CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 180);
	  double Distance5 = CAAGlobe.DistanceBetweenPoints(CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 0, CAACoordinateTransformation.DMSToDegrees(89, 59, 0), 90);


	  //Test out the AASidereal class
	  date.Set(1987, 4, 10, 0, 0, 0, true);
	  double MST = CAASidereal.MeanGreenwichSiderealTime(date.Julian());
	  double AST = CAASidereal.ApparentGreenwichSiderealTime(date.Julian());
	  date.Set(1987, 4, 10, 19, 21, 0, true);
	  MST = CAASidereal.MeanGreenwichSiderealTime(date.Julian());

	  //Test out the AACoordinateTransformation class
	//C++ TO C# CONVERTER TODO TASK: Octal literals cannot be represented in C#:
	  CAA2DCoordinate Ecliptic = CAACoordinateTransformation.Equatorial2Ecliptic(CAACoordinateTransformation.DMSToDegrees(7, 45, 18.946), CAACoordinateTransformation.DMSToDegrees(28, 01, 34.26), 23.4392911);
	  CAA2DCoordinate Equatorial = CAACoordinateTransformation.Ecliptic2Equatorial(Ecliptic.X, Ecliptic.Y, 23.4392911);
	  CAA2DCoordinate Galactic = CAACoordinateTransformation.Equatorial2Galactic(CAACoordinateTransformation.DMSToDegrees(17, 48, 59.74), CAACoordinateTransformation.DMSToDegrees(14, 43, 8.2, false));
	  CAA2DCoordinate Equatorial2 = CAACoordinateTransformation.Galactic2Equatorial(Galactic.X, Galactic.Y);
	  date.Set(1987, 4, 10, 19, 21, 0, true);
	  AST = CAASidereal.ApparentGreenwichSiderealTime(date.Julian());
	  double LongtitudeAsHourAngle = CAACoordinateTransformation.DegreesToHours(CAACoordinateTransformation.DMSToDegrees(77, 3, 56));
	  double Alpha = CAACoordinateTransformation.DMSToDegrees(23, 9, 16.641);
	  double LocalHourAngle = AST - LongtitudeAsHourAngle - Alpha;
	  CAA2DCoordinate Horizontal = CAACoordinateTransformation.Equatorial2Horizontal(LocalHourAngle, CAACoordinateTransformation.DMSToDegrees(6, 43, 11.61, false), CAACoordinateTransformation.DMSToDegrees(38, 55, 17));
	  CAA2DCoordinate Equatorial3 = CAACoordinateTransformation.Horizontal2Equatorial(Horizontal.X, Horizontal.Y, CAACoordinateTransformation.DMSToDegrees(38, 55, 17));
	  double alpha2 = CAACoordinateTransformation.MapTo0To24Range(AST - Equatorial3.X - LongtitudeAsHourAngle);

	  //Test out the CAANutation class (on its own)
	  date.Set(1987, 4, 10, 0, 0, 0, true);
	  double Obliquity = CAANutation.MeanObliquityOfEcliptic(date.Julian());
	  double NutationInLongitude = CAANutation.NutationInLongitude(date.Julian());
		double NutationInEcliptic = CAANutation.NutationInObliquity(date.Julian());

	  //Test out the CAAParallactic class
	  double HourAngle = CAAParallactic.ParallacticAngle(-3, 10, 20);
	  double EclipticLongitude = CAAParallactic.EclipticLongitudeOnHorizon(5, 23.44, 51);
	  double EclipticAngle = CAAParallactic.AngleBetweenEclipticAndHorizon(5, 23.44, 51);
	  double Angle = CAAParallactic.AngleBetweenNorthCelestialPoleAndNorthPoleOfEcliptic(90, 0, 23.44);

	  //Test out the CAARefraction class
	  double R1 = CAARefraction.RefractionFromApparent(0.5);
	  double R2 = CAARefraction.RefractionFromTrue(0.5 - R1 + CAACoordinateTransformation.DMSToDegrees(0, 32, 0));
	  double R3 = CAARefraction.RefractionFromApparent(90);

	  //Test out the CAAAngularSeparation class
	  double AngularSeparation = CAAAngularSeparation.Separation(CAACoordinateTransformation.DMSToDegrees(14, 15, 39.7), CAACoordinateTransformation.DMSToDegrees(19, 10, 57), CAACoordinateTransformation.DMSToDegrees(13, 25, 11.6), CAACoordinateTransformation.DMSToDegrees(11, 9, 41, false));
	  double AngularSeparation2 = CAAAngularSeparation.Separation(CAACoordinateTransformation.DMSToDegrees(2, 0, 0), CAACoordinateTransformation.DMSToDegrees(0, 0, 0), CAACoordinateTransformation.DMSToDegrees(2, 0, 0), CAACoordinateTransformation.DMSToDegrees(0, 0, 0));
	  double AngularSeparation3 = CAAAngularSeparation.Separation(CAACoordinateTransformation.DMSToDegrees(2, 0, 0), CAACoordinateTransformation.DMSToDegrees(0, 0, 0), CAACoordinateTransformation.DMSToDegrees(14, 0, 0), CAACoordinateTransformation.DMSToDegrees(0, 0, 0));

	  double PA0 = CAAAngularSeparation.PositionAngle(CAACoordinateTransformation.DMSToDegrees(5, 32, 0.4), CAACoordinateTransformation.DMSToDegrees(0, 17, 56.9, false), CAACoordinateTransformation.DMSToDegrees(5, 36, 12.81), CAACoordinateTransformation.DMSToDegrees(1, 12, 7, false));

	  double PA1 = CAAAngularSeparation.PositionAngle(CAACoordinateTransformation.DMSToDegrees(5, 40, 45.52), CAACoordinateTransformation.DMSToDegrees(1, 56, 33.3, false), CAACoordinateTransformation.DMSToDegrees(5, 36, 12.81), CAACoordinateTransformation.DMSToDegrees(1, 12, 7, false));


	  double distance = CAAAngularSeparation.DistanceFromGreatArc(CAACoordinateTransformation.DMSToDegrees(5, 32, 0.4), CAACoordinateTransformation.DMSToDegrees(0, 17, 56.9, false), CAACoordinateTransformation.DMSToDegrees(5, 40, 45.52), CAACoordinateTransformation.DMSToDegrees(1, 56, 33.3, false), CAACoordinateTransformation.DMSToDegrees(5, 36, 12.81), CAACoordinateTransformation.DMSToDegrees(1, 12, 7, false));

	  bool bType1 = false;
	  double separation = CAAAngularSeparation.SmallestCircle(CAACoordinateTransformation.DMSToDegrees(12, 41, 8.63), CAACoordinateTransformation.DMSToDegrees(5, 37, 54.2, false), CAACoordinateTransformation.DMSToDegrees(12, 52, 5.21), CAACoordinateTransformation.DMSToDegrees(4, 22, 26.2, false), CAACoordinateTransformation.DMSToDegrees(12, 39, 28.11), CAACoordinateTransformation.DMSToDegrees(1, 50, 3.7, false), ref bType1);

	  separation = CAAAngularSeparation.SmallestCircle(CAACoordinateTransformation.DMSToDegrees(9, 5, 41.44), CAACoordinateTransformation.DMSToDegrees(18, 30, 30), CAACoordinateTransformation.DMSToDegrees(9, 9, 29), CAACoordinateTransformation.DMSToDegrees(17, 43, 56.7), CAACoordinateTransformation.DMSToDegrees(8, 59, 47.14), CAACoordinateTransformation.DMSToDegrees(17, 49, 36.8), ref bType1);

	  Alpha = CAACoordinateTransformation.DMSToDegrees(2, 44, 11.986);
	  double Delta = CAACoordinateTransformation.DMSToDegrees(49, 13, 42.48);
	  CAA2DCoordinate PA = CAAPrecession.AdjustPositionUsingUniformProperMotion((2462088.69-2451545)/365.25, Alpha, Delta, 0.03425, -0.0895);

	  CAA2DCoordinate Precessed = CAAPrecession.PrecessEquatorial(PA.X, PA.Y, 2451545, 2462088.69);

	  Alpha = CAACoordinateTransformation.DMSToDegrees(2, 31, 48.704);
	  Delta = CAACoordinateTransformation.DMSToDegrees(89, 15, 50.72);
	  CAA2DCoordinate PA2 = CAAPrecession.AdjustPositionUsingUniformProperMotion((2415020.3135-2451545)/365.25, Alpha, Delta, 0.19877, -0.0152);
	  //CAA2DCoordinate Precessed2 = CAAPrecession::PrecessEquatorialFK4(PA2.X, PA2.Y, 2451545, 2415020.3135);

	  CAA2DCoordinate PM = CAAPrecession.EquatorialPMToEcliptic(0, 0, 0, 1, 1, 23);


	  CAA2DCoordinate PA3 = CAAPrecession.AdjustPositionUsingMotionInSpace(2.64, -7.6, -1000, CAACoordinateTransformation.DMSToDegrees(6, 45, 8.871), CAACoordinateTransformation.DMSToDegrees(16, 42, 57.99, false), -0.03847, -1.2053);
	  CAA2DCoordinate PA4 = CAAPrecession.AdjustPositionUsingUniformProperMotion(-1000, CAACoordinateTransformation.DMSToDegrees(6, 45, 8.871), CAACoordinateTransformation.DMSToDegrees(16, 42, 57.99, false), -0.03847, -1.2053);

	  CAA2DCoordinate PA5 = CAAPrecession.AdjustPositionUsingMotionInSpace(2.64, -7.6, -12000, CAACoordinateTransformation.DMSToDegrees(6, 45, 8.871), CAACoordinateTransformation.DMSToDegrees(16, 42, 57.99, false), -0.03847, -1.2053);
	  CAA2DCoordinate PA6 = CAAPrecession.AdjustPositionUsingUniformProperMotion(-12000, CAACoordinateTransformation.DMSToDegrees(6, 45, 8.871), CAACoordinateTransformation.DMSToDegrees(16, 42, 57.99, false), -0.03847, -1.2053);

	  Alpha = CAACoordinateTransformation.DMSToDegrees(2, 44, 11.986);
	  Delta = CAACoordinateTransformation.DMSToDegrees(49, 13, 42.48);
	  CAA2DCoordinate PA7 = CAAPrecession.AdjustPositionUsingUniformProperMotion((2462088.69-2451545)/365.25, Alpha, Delta, 0.03425, -0.0895);
	  CAA3DCoordinate EarthVelocity = CAAAberration.EarthVelocity(2462088.69);
	  CAA2DCoordinate Aberration = CAAAberration.EquatorialAberration(PA7.X, PA7.Y, 2462088.69);
	  PA7.X += Aberration.X;
	  PA7.Y += Aberration.Y;
	  PA7 = CAAPrecession.PrecessEquatorial(PA7.X, PA7.Y, 2451545, 2462088.69);

	  Obliquity = CAANutation.MeanObliquityOfEcliptic(2462088.69);
	  NutationInLongitude = CAANutation.NutationInLongitude(2462088.69);
		NutationInEcliptic = CAANutation.NutationInObliquity(2462088.69);
	  double AlphaNutation = CAANutation.NutationInRightAscension(PA7.X, PA7.Y, Obliquity, NutationInLongitude, NutationInEcliptic);
	  double DeltaNutation = CAANutation.NutationInDeclination(PA7.X, PA7.Y, Obliquity, NutationInLongitude, NutationInEcliptic);
	  PA7.X += CAACoordinateTransformation.DMSToDegrees(0, 0, AlphaNutation/15);
	  PA7.Y += CAACoordinateTransformation.DMSToDegrees(0, 0, DeltaNutation);



	  //Try out the AA kepler class
	  double E0 = CAAKepler.Calculate(5, 0.1, 100);
	  double E02 = CAAKepler.Calculate(5, 0.9, 100);
	  //double E03 = CAAKepler::Calculate(

	  //Try out the binary star class
	  CAABinaryStarDetails bsdetails = CAABinaryStar.Calculate(1980, 41.623, 1934.008, 0.2763, 0.907, 59.025, 23.717, 219.907);
	  double ApparentE = CAABinaryStar.ApparentEccentricity(0.2763, 59.025, 219.907);


	  //Test out the CAAStellarMagnitudes class
	  double CombinedMag = CAAStellarMagnitudes.CombinedMagnitude(1.96, 2.89);
	  double[] Mags = { 4.73, 5.22, 5.60 };
	  double CombinedMag2 = CAAStellarMagnitudes.CombinedMagnitude(3, Mags);
	  double BrightnessRatio = CAAStellarMagnitudes.BrightnessRatio(0.14, 2.12);
	  double MagDiff = CAAStellarMagnitudes.MagnitudeDifference(BrightnessRatio);
	  double MagDiff2 = CAAStellarMagnitudes.MagnitudeDifference(500);


	  //Test out the CAAVenus class
	  double VenusLong = CAAVenus.EclipticLongitude(2448976.5);
	  double VenusLat = CAAVenus.EclipticLatitude(2448976.5);
	  double VenusRadius = CAAVenus.RadiusVector(2448976.5);


	  //Test out the CAAMercury class
	  double MercuryLong = CAAMercury.EclipticLongitude(2448976.5);
	  double MercuryLat = CAAMercury.EclipticLatitude(2448976.5);
	  double MercuryRadius = CAAMercury.RadiusVector(2448976.5);


	  //Test out the CAAEarth class
	  double EarthLong = CAAEarth.EclipticLongitude(2448908.5);
	  double EarthLat = CAAEarth.EclipticLatitude(2448908.5);
	  double EarthRadius = CAAEarth.RadiusVector(2448908.5);

	  double EarthLong2 = CAAEarth.EclipticLongitudeJ2000(2448908.5);
	  double EarthLat2 = CAAEarth.EclipticLatitudeJ2000(2448908.5);


	  //Test out the CAASun class
	  double SunLong = CAASun.GeometricEclipticLongitude(2448908.5);
	  double SunLat = CAASun.GeometricEclipticLatitude(2448908.5);

	  double SunLongCorrection = CAAFK5.CorrectionInLongitude(SunLong, SunLat, 2448908.5);
	  double SunLatCorrection = CAAFK5.CorrectionInLatitude(SunLong, 2448908.5);

	  SunLong = CAASun.ApparentEclipticLongitude(2448908.5);
	  SunLat = CAASun.ApparentEclipticLatitude(2448908.5);
	  Equatorial = CAACoordinateTransformation.Ecliptic2Equatorial(SunLong, SunLat, CAANutation.TrueObliquityOfEcliptic(2448908.5));

	  CAA3DCoordinate SunCoord = CAASun.EclipticRectangularCoordinatesMeanEquinox(2448908.5);
	  CAA3DCoordinate SunCoord2 = CAASun.EclipticRectangularCoordinatesJ2000(2448908.5);
	  CAA3DCoordinate SunCoord3 = CAASun.EquatorialRectangularCoordinatesJ2000(2448908.5);
	  CAA3DCoordinate SunCoord4 = CAASun.EquatorialRectangularCoordinatesB1950(2448908.5);
	  CAA3DCoordinate SunCoord5 = CAASun.EquatorialRectangularCoordinatesAnyEquinox(2448908.5, 2467616.0);

	  //Test out the CAAMars class
	  double MarsLong = CAAMars.EclipticLongitude(2448935.500683);
	  double MarsLat = CAAMars.EclipticLatitude(2448935.500683);
	  double MarsRadius = CAAMars.RadiusVector(2448935.500683);

	  //Test out the CAAJupiter class
	  double JupiterLong = CAAJupiter.EclipticLongitude(2448972.50068);
	  double JupiterLat = CAAJupiter.EclipticLatitude(2448972.50068);
	  double JupiterRadius = CAAJupiter.RadiusVector(2448972.50068);

	  //Test out the CAANeptune class
	  double NeptuneLong = CAANeptune.EclipticLongitude(2448935.500683);
	  double NeptuneLat = CAANeptune.EclipticLatitude(2448935.500683);
	  double NeptuneRadius = CAANeptune.RadiusVector(2448935.500683);

	  //Test out the CAAUranus class
	  double UranusLong = CAAUranus.EclipticLongitude(2448976.5);
	  double UranusLat = CAAUranus.EclipticLatitude(2448976.5);
	  double UranusRadius = CAAUranus.RadiusVector(2448976.5);

	  //Test out the CAASaturn class
	  double SaturnLong = CAASaturn.EclipticLongitude(2448972.50068);
	  double SaturnLat = CAASaturn.EclipticLatitude(2448972.50068);
	  double SaturnRadius = CAASaturn.RadiusVector(2448972.50068);

	  //Test out the CAAPluto class
	  double PlutoLong = CAAPluto.EclipticLongitude(2448908.5);
	  double PlutoLat = CAAPluto.EclipticLatitude(2448908.5);
	  double PlutoRadius = CAAPluto.RadiusVector(2448908.5);

	  //Test out the CAAMoon class
	  double MoonLong = CAAMoon.EclipticLongitude(2448724.5);
	  double MoonLat = CAAMoon.EclipticLatitude(2448724.5);
	  double MoonRadius = CAAMoon.RadiusVector(2448724.5);
	  double MoonParallax = CAAMoon.RadiusVectorToHorizontalParallax(MoonRadius);
	  double MoonMeanAscendingNode = CAAMoon.MeanLongitudeAscendingNode(2448724.5);
	  double TrueMeanAscendingNode = CAAMoon.TrueLongitudeAscendingNode(2448724.5);
	  double MoonMeanPerigee = CAAMoon.MeanLongitudePerigee(2448724.5);

	  //Test out the CAAPlanetPerihelionAphelion class
	  int VenusK = CAAPlanetPerihelionAphelion.VenusK(1978.79);
	  double VenusPerihelion = CAAPlanetPerihelionAphelion.VenusPerihelion(VenusK);

	  int MarsK = CAAPlanetPerihelionAphelion.MarsK(2032);
	  double MarsAphelion = CAAPlanetPerihelionAphelion.MarsAphelion(MarsK);

	  int SaturnK = CAAPlanetPerihelionAphelion.SaturnK(1925);
	  double SaturnAphelion = CAAPlanetPerihelionAphelion.SaturnAphelion(SaturnK);
	  SaturnK = CAAPlanetPerihelionAphelion.SaturnK(1940);
	  double SaturnPerihelion = CAAPlanetPerihelionAphelion.SaturnPerihelion(SaturnK);

	  int UranusK = CAAPlanetPerihelionAphelion.UranusK(1750);
	  double UranusAphelion = CAAPlanetPerihelionAphelion.UranusAphelion(UranusK);
	  UranusK = CAAPlanetPerihelionAphelion.UranusK(1890);
	  double UranusPerihelion = CAAPlanetPerihelionAphelion.UranusPerihelion(UranusK);
	  UranusK = CAAPlanetPerihelionAphelion.UranusK(2060);
	  UranusPerihelion = CAAPlanetPerihelionAphelion.UranusPerihelion(UranusK);

	  double EarthPerihelion = CAAPlanetPerihelionAphelion.EarthPerihelion(-10, true);
	  double EarthPerihelion2 = CAAPlanetPerihelionAphelion.EarthPerihelion(-10, false);


	  //Test out the CAAMoonPerigeeApogee
	  double MoonK = CAAMoonPerigeeApogee.K(1988.75);
	  double MoonApogee = CAAMoonPerigeeApogee.MeanApogee(-148.5);
	  double MoonApogee2 = CAAMoonPerigeeApogee.TrueApogee(-148.5);
	  double MoonApogeeParallax = CAAMoonPerigeeApogee.ApogeeParallax(-148.5);
	  double MoonApogeeDistance = CAAMoon.HorizontalParallaxToRadiusVector(MoonApogeeParallax);

	  MoonK = CAAMoonPerigeeApogee.K(1990.9);
	  double MoonPerigee = CAAMoonPerigeeApogee.MeanPerigee(-120);
	  double MoonPerigee2 = CAAMoonPerigeeApogee.TruePerigee(-120);
	  MoonK = CAAMoonPerigeeApogee.K(1930.0);
	  double MoonPerigee3 = CAAMoonPerigeeApogee.TruePerigee(-927);
	  double MoonPerigeeParallax = CAAMoonPerigeeApogee.PerigeeParallax(-927);
	  double MoonRadiusVector = CAAMoon.HorizontalParallaxToRadiusVector(MoonPerigeeParallax);
	  double MoonRadiusVector2 = CAAMoon.HorizontalParallaxToRadiusVector(0.991990);
	  double MoonParallax2 = CAAMoon.RadiusVectorToHorizontalParallax(MoonRadiusVector2);

	  //Test out the CAAEclipticalElements class
	  CAAEclipticalElementDetails ed1 = CAAEclipticalElements.Calculate(47.1220, 151.4486, 45.7481, 2358042.5305, 2433282.4235);
	  CAAEclipticalElementDetails ed2 = CAAEclipticalElements.Calculate(11.93911, 186.24444, 334.04096, 2433282.4235, 2451545.0);
	  CAAEclipticalElementDetails ed3 = CAAEclipticalElements.FK4B1950ToFK5J2000(11.93911, 186.24444, 334.04096);
	  CAAEclipticalElementDetails ed4 = CAAEclipticalElements.FK4B1950ToFK5J2000(145, 186.24444, 334.04096);


	  //Test out the CAAEquationOfTime class
	  double E = CAAEquationOfTime.Calculate(2448908.5);


	  //Test out the CAAPhysicalSun class
	  CAAPhysicalSunDetails psd = CAAPhysicalSun.Calculate(2448908.50068);
	  double JED = CAAPhysicalSun.TimeOfStartOfRotation(1699);


	  //Test out the CAAEquinoxesAndSolstices class
	  double JuneSolstice = CAAEquinoxesAndSolstices.SummerSolstice(1962);


	  double MarchEquinox2 = CAAEquinoxesAndSolstices.SpringEquinox(1996);
	  date.Set(MarchEquinox2, true);
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);
	  double JuneSolstice2 = CAAEquinoxesAndSolstices.SummerSolstice(1996);
	  date.Set(JuneSolstice2, true);
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);
	  double SeptemberEquinox2 = CAAEquinoxesAndSolstices.AutumnEquinox(1996);
	  date.Set(SeptemberEquinox2, true);
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);
	  double DecemberSolstice2 = CAAEquinoxesAndSolstices.WinterSolstice(1996);
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

	  double SpringLength = CAAEquinoxesAndSolstices.LengthOfSpring(2000);
	  double SummerLength = CAAEquinoxesAndSolstices.LengthOfSummer(2000);
	  double AutumnLength = CAAEquinoxesAndSolstices.LengthOfAutumn(2000);
	  double WinterLength = CAAEquinoxesAndSolstices.LengthOfWinter(2000);

	  SpringLength = CAAEquinoxesAndSolstices.LengthOfSpring(-2000);
	  SummerLength = CAAEquinoxesAndSolstices.LengthOfSummer(-2000);
	  AutumnLength = CAAEquinoxesAndSolstices.LengthOfAutumn(-2000);
	  WinterLength = CAAEquinoxesAndSolstices.LengthOfWinter(-2000);

	  SpringLength = CAAEquinoxesAndSolstices.LengthOfSpring(4000);
	  SummerLength = CAAEquinoxesAndSolstices.LengthOfSummer(4000);
	  AutumnLength = CAAEquinoxesAndSolstices.LengthOfAutumn(4000);
	  WinterLength = CAAEquinoxesAndSolstices.LengthOfWinter(4000);


	  //Test out the CAAElementsPlanetaryOrbit class
	  double Mer_L = CAAElementsPlanetaryOrbit.MercuryMeanLongitude(2475460.5);
	  double Mer_a = CAAElementsPlanetaryOrbit.MercurySemimajorAxis(2475460.5);
	  double Mer_e = CAAElementsPlanetaryOrbit.MercuryEccentricity(2475460.5);
	  double Mer_i = CAAElementsPlanetaryOrbit.MercuryInclination(2475460.5);
	  double Mer_omega = CAAElementsPlanetaryOrbit.MercuryLongitudeAscendingNode(2475460.5);
	  double mer_pi = CAAElementsPlanetaryOrbit.MercuryLongitudePerihelion(2475460.5);

	  double Ven_L = CAAElementsPlanetaryOrbit.VenusMeanLongitude(2475460.5);
	  double Ven_a = CAAElementsPlanetaryOrbit.VenusSemimajorAxis(2475460.5);
	  double Ven_e = CAAElementsPlanetaryOrbit.VenusEccentricity(2475460.5);
	  double Ven_i = CAAElementsPlanetaryOrbit.VenusInclination(2475460.5);
	  double Ven_omega = CAAElementsPlanetaryOrbit.VenusLongitudeAscendingNode(2475460.5);
	  double Ven_pi = CAAElementsPlanetaryOrbit.VenusLongitudePerihelion(2475460.5);

	  double Ea_L = CAAElementsPlanetaryOrbit.EarthMeanLongitude(2475460.5);
	  double Ea_a = CAAElementsPlanetaryOrbit.EarthSemimajorAxis(2475460.5);
	  double Ea_e = CAAElementsPlanetaryOrbit.EarthEccentricity(2475460.5);
	  double Ea_i = CAAElementsPlanetaryOrbit.EarthInclination(2475460.5);
	  double Ea_pi = CAAElementsPlanetaryOrbit.EarthLongitudePerihelion(2475460.5);

	  double Mars_L = CAAElementsPlanetaryOrbit.MarsMeanLongitude(2475460.5);
	  double Mars_a = CAAElementsPlanetaryOrbit.MarsSemimajorAxis(2475460.5);
	  double Mars_e = CAAElementsPlanetaryOrbit.MarsEccentricity(2475460.5);
	  double Mars_i = CAAElementsPlanetaryOrbit.MarsInclination(2475460.5);
	  double Mars_omega = CAAElementsPlanetaryOrbit.MarsLongitudeAscendingNode(2475460.5);
	  double Mars_pi = CAAElementsPlanetaryOrbit.MarsLongitudePerihelion(2475460.5);

	  double Jup_L = CAAElementsPlanetaryOrbit.JupiterMeanLongitude(2475460.5);
	  double Jup_a = CAAElementsPlanetaryOrbit.JupiterSemimajorAxis(2475460.5);
	  double Jup_e = CAAElementsPlanetaryOrbit.JupiterEccentricity(2475460.5);
	  double Jup_i = CAAElementsPlanetaryOrbit.JupiterInclination(2475460.5);
	  double Jup_omega = CAAElementsPlanetaryOrbit.JupiterLongitudeAscendingNode(2475460.5);
	  double Jup_pi = CAAElementsPlanetaryOrbit.JupiterLongitudePerihelion(2475460.5);

	  double Sat_L = CAAElementsPlanetaryOrbit.SaturnMeanLongitude(2475460.5);
	  double Sat_a = CAAElementsPlanetaryOrbit.SaturnSemimajorAxis(2475460.5);
	  double Sat_e = CAAElementsPlanetaryOrbit.SaturnEccentricity(2475460.5);
	  double Sat_i = CAAElementsPlanetaryOrbit.SaturnInclination(2475460.5);
	  double Sat_omega = CAAElementsPlanetaryOrbit.SaturnLongitudeAscendingNode(2475460.5);
	  double Sat_pi = CAAElementsPlanetaryOrbit.SaturnLongitudePerihelion(2475460.5);

	  double Ura_L = CAAElementsPlanetaryOrbit.UranusMeanLongitude(2475460.5);
	  double Ura_a = CAAElementsPlanetaryOrbit.UranusSemimajorAxis(2475460.5);
	  double Ura_e = CAAElementsPlanetaryOrbit.UranusEccentricity(2475460.5);
	  double Ura_i = CAAElementsPlanetaryOrbit.UranusInclination(2475460.5);
	  double Ura_omega = CAAElementsPlanetaryOrbit.UranusLongitudeAscendingNode(2475460.5);
	  double Ura_pi = CAAElementsPlanetaryOrbit.UranusLongitudePerihelion(2475460.5);

	  double Nep_L = CAAElementsPlanetaryOrbit.NeptuneMeanLongitude(2475460.5);
	  double Nep_a = CAAElementsPlanetaryOrbit.NeptuneSemimajorAxis(2475460.5);
	  double Nep_e = CAAElementsPlanetaryOrbit.NeptuneEccentricity(2475460.5);
	  double Nep_i = CAAElementsPlanetaryOrbit.NeptuneInclination(2475460.5);
	  double Nep_omega = CAAElementsPlanetaryOrbit.NeptuneLongitudeAscendingNode(2475460.5);
	  double Nep_pi = CAAElementsPlanetaryOrbit.NeptuneLongitudePerihelion(2475460.5);


	  double Mer_L2 = CAAElementsPlanetaryOrbit.MercuryMeanLongitudeJ2000(2475460.5);
	  double Mer_i2 = CAAElementsPlanetaryOrbit.MercuryInclinationJ2000(2475460.5);
	  double Mer_omega2 = CAAElementsPlanetaryOrbit.MercuryLongitudeAscendingNodeJ2000(2475460.5);
	  double mer_pi2 = CAAElementsPlanetaryOrbit.MercuryLongitudePerihelionJ2000(2475460.5);

	  double Ven_L2 = CAAElementsPlanetaryOrbit.VenusMeanLongitudeJ2000(2475460.5);
	  double Ven_i2 = CAAElementsPlanetaryOrbit.VenusInclinationJ2000(2475460.5);
	  double Ven_omega2 = CAAElementsPlanetaryOrbit.VenusLongitudeAscendingNodeJ2000(2475460.5);
	  double Ven_pi2 = CAAElementsPlanetaryOrbit.VenusLongitudePerihelionJ2000(2475460.5);

	  double Ea_L2 = CAAElementsPlanetaryOrbit.EarthMeanLongitudeJ2000(2475460.5);
	  double Ea_i2 = CAAElementsPlanetaryOrbit.EarthInclinationJ2000(2475460.5);
	  double Ea_omega2 = CAAElementsPlanetaryOrbit.EarthLongitudeAscendingNodeJ2000(2475460.5);
	  double Ea_pi2 = CAAElementsPlanetaryOrbit.EarthLongitudePerihelionJ2000(2475460.5);

	  double Mars_L2 = CAAElementsPlanetaryOrbit.MarsMeanLongitudeJ2000(2475460.5);
	  double Mars_i2 = CAAElementsPlanetaryOrbit.MarsInclinationJ2000(2475460.5);
	  double Mars_omega2 = CAAElementsPlanetaryOrbit.MarsLongitudeAscendingNodeJ2000(2475460.5);
	  double Mars_pi2 = CAAElementsPlanetaryOrbit.MarsLongitudePerihelionJ2000(2475460.5);

	  double Jup_L2 = CAAElementsPlanetaryOrbit.JupiterMeanLongitudeJ2000(2475460.5);
	  double Jup_i2 = CAAElementsPlanetaryOrbit.JupiterInclinationJ2000(2475460.5);
	  double Jup_omega2 = CAAElementsPlanetaryOrbit.JupiterLongitudeAscendingNodeJ2000(2475460.5);
	  double Jup_pi2 = CAAElementsPlanetaryOrbit.JupiterLongitudePerihelionJ2000(2475460.5);

	  double Sat_L2 = CAAElementsPlanetaryOrbit.SaturnMeanLongitudeJ2000(2475460.5);
	  double Sat_i2 = CAAElementsPlanetaryOrbit.SaturnInclinationJ2000(2475460.5);
	  double Sat_omega2 = CAAElementsPlanetaryOrbit.SaturnLongitudeAscendingNodeJ2000(2475460.5);
	  double Sat_pi2 = CAAElementsPlanetaryOrbit.SaturnLongitudePerihelionJ2000(2475460.5);

	  double Ura_L2 = CAAElementsPlanetaryOrbit.UranusMeanLongitudeJ2000(2475460.5);
	  double Ura_i2 = CAAElementsPlanetaryOrbit.UranusInclinationJ2000(2475460.5);
	  double Ura_omega2 = CAAElementsPlanetaryOrbit.UranusLongitudeAscendingNodeJ2000(2475460.5);
	  double Ura_pi2 = CAAElementsPlanetaryOrbit.UranusLongitudePerihelionJ2000(2475460.5);

	  double Nep_L2 = CAAElementsPlanetaryOrbit.NeptuneMeanLongitudeJ2000(2475460.5);
	  double Nep_i2 = CAAElementsPlanetaryOrbit.NeptuneInclinationJ2000(2475460.5);
	  double Nep_omega2 = CAAElementsPlanetaryOrbit.NeptuneLongitudeAscendingNodeJ2000(2475460.5);
	  double Nep_pi2 = CAAElementsPlanetaryOrbit.NeptuneLongitudePerihelionJ2000(2475460.5);

	  double MoonGeocentricElongation = CAAMoonIlluminatedFraction.GeocentricElongation(8.97922, 13.7684, 1.377194, 8.6964);
	  double MoonPhaseAngle = CAAMoonIlluminatedFraction.PhaseAngle(MoonGeocentricElongation, 368410, 149971520);
	  double MoonIlluminatedFraction = CAAMoonIlluminatedFraction.IlluminatedFraction(MoonPhaseAngle);
	  double MoonPositionAngle = CAAMoonIlluminatedFraction.PositionAngle(CAACoordinateTransformation.DMSToDegrees(1, 22, 37.9), 8.6964, 134.6885/15, 13.7684);

	  CAAEllipticalPlanetaryDetails VenusDetails = CAAElliptical.Calculate(2448976.5, EllipticalObject.VENUS);

	  CAAEllipticalPlanetaryDetails SunDetails = CAAElliptical.Calculate(2453149.5, EllipticalObject.SUN);

	  CAAEllipticalObjectElements elements = new CAAEllipticalObjectElements();
	  elements.a = 2.2091404;
	  elements.e = 0.8502196;
	  elements.i = 11.94524;
	  elements.omega = 334.75006;
	  elements.w = 186.23352;
	  elements.T = 2448192.5 + 0.54502;
	  elements.JDEquinox = 2451544.5;
	  CAAEllipticalObjectDetails details = CAAElliptical.Calculate(2448170.5, elements);

	  double Velocity1 = CAAElliptical.InstantaneousVelocity(1, 17.9400782);
	  double Velocity2 = CAAElliptical.VelocityAtPerihelion(0.96727426, 17.9400782);
	  double Velocity3 = CAAElliptical.VelocityAtAphelion(0.96727426, 17.9400782);

	  double Length = CAAElliptical.LengthOfEllipse(0.96727426, 17.9400782);

	  double Mag1 = CAAElliptical.MinorPlanetMagnitude(3.34, 1.6906631928, 0.12, 2.6154983761, 120);
	  double Mag2 = CAAElliptical.CometMagnitude(5.5, 0.378, 10, 0.658);
	  double Mag3 = CAAElliptical.CometMagnitude(5.5, 1.1017, 10, 1.5228);

	  CAAParabolicObjectElements elements2 = new CAAParabolicObjectElements();
	  elements2.q = 1.487469;
	  elements2.i = 0; //unknown
	  elements2.omega = 0; //unknown
	  elements2.w = 0; //unknown
	  elements2.T = 2450917.9358;
	  elements2.JDEquinox = 2451030.5;
	  CAAParabolicObjectDetails details2 = CAAParabolic.Calculate(2451030.5, elements2);


	  CAAEllipticalObjectElements elements3 = new CAAEllipticalObjectElements();
	  elements3.a = 17.9400782;
	  elements3.e = 0.96727426;
	  elements3.i = 0; //Not required
	  elements3.omega = 0; //Not required
	  elements3.w = 111.84644;
	  elements3.T = 2446470.5 + 0.45891;
	  elements3.JDEquinox = 0; //Not required
	  CAANodeObjectDetails nodedetails = CAANodes.PassageThroAscendingNode(elements3);
	  CAANodeObjectDetails nodedetails2 = CAANodes.PassageThroDescendingNode(elements3);

	  CAAParabolicObjectElements elements4 = new CAAParabolicObjectElements();
	  elements4.q = 1.324502;
	  elements4.i = 0; //Not required
	  elements4.omega = 0; //Not required
	  elements4.w = 154.9103;
	  elements4.T = 2447758.5 + 0.2910;
	  elements4.JDEquinox = 0; //Not required
	  CAANodeObjectDetails nodedetails3 = CAANodes.PassageThroAscendingNode(elements4);
	  CAANodeObjectDetails nodedetails4 = CAANodes.PassageThroDescendingNode(elements4);


	  CAAEllipticalObjectElements elements5 = new CAAEllipticalObjectElements();
	  elements5.a = 0.723329820;
	  elements5.e = 0.00678195;
	  elements5.i = 0; //Not required
	  elements5.omega = 0; //Not required
	  elements5.w = 54.778485;
	  elements5.T = 2443873.704;
	  elements5.JDEquinox = 0; //Not required
	  CAANodeObjectDetails nodedetails5 = CAANodes.PassageThroAscendingNode(elements5);

	  double MoonK2 = CAAMoonNodes.K(1987.37);
	  double MoonJD = CAAMoonNodes.PassageThroNode(-170);


	  double Y = CAAInterpolate.Interpolate(0.18125, 0.884226, 0.877366, 0.870531);

	  double NM = 0;
	  double YM = CAAInterpolate.Extremum(1.3814294, 1.3812213, 1.3812453, ref NM);

	  double N0 = CAAInterpolate.Zero(-1693.4, 406.3, 2303.2);

	  double N02 = CAAInterpolate.Zero2(-2, 3, 2);

	  double Y2 = CAAInterpolate.Interpolate(0.2777778, 36.125, 24.606, 15.486, 8.694, 4.133);

	  double N03 = CAAInterpolate.Zero(CAACoordinateTransformation.DMSToDegrees(1, 11, 21.23, false), CAACoordinateTransformation.DMSToDegrees(0, 28, 12.31, false), CAACoordinateTransformation.DMSToDegrees(0, 16, 7.02), CAACoordinateTransformation.DMSToDegrees(1, 1, 0.13), CAACoordinateTransformation.DMSToDegrees(1, 45, 46.33));

	  double N04 = CAAInterpolate.Zero(CAACoordinateTransformation.DMSToDegrees(0, 28, 12.31, false), CAACoordinateTransformation.DMSToDegrees(0, 16, 7.02), CAACoordinateTransformation.DMSToDegrees(1, 1, 0.13));

	  double N05 = CAAInterpolate.Zero2(-13, -2, 3, 2, -5);

	  double Y3 = CAAInterpolate.InterpolateToHalves(1128.732, 1402.835, 1677.247, 1951.983);

	  double[] X1 = { 29.43, 30.97, 27.69, 28.11, 31.58, 33.05 };
	  double[] Y1 = { 0.4913598528, 0.5145891926, 0.4646875083, 0.4711658342, 0.5236885653, 0.5453707057 };
	  double Y4 = CAAInterpolate.LagrangeInterpolate(30, 6,  X1,  Y1);
	  double Y5 = CAAInterpolate.LagrangeInterpolate(0, 6,  X1,  Y1);
	  double Y6 = CAAInterpolate.LagrangeInterpolate(90, 6,  X1,  Y1);


	  double Alpha1 = CAACoordinateTransformation.DMSToDegrees(2, 42, 43.25);
	  double Alpha2 = CAACoordinateTransformation.DMSToDegrees(2, 46, 55.51);
	//C++ TO C# CONVERTER TODO TASK: Octal literals cannot be represented in C#:
	  double Alpha3 = CAACoordinateTransformation.DMSToDegrees(2, 51, 07.69);

	//C++ TO C# CONVERTER TODO TASK: Octal literals cannot be represented in C#:
	  double Delta1 = CAACoordinateTransformation.DMSToDegrees(18, 02, 51.4);
	  double Delta2 = CAACoordinateTransformation.DMSToDegrees(18, 26, 27.3);
	  double Delta3 = CAACoordinateTransformation.DMSToDegrees(18, 49, 38.7);

	  CAARiseTransitSetDetails RiseTransitSetTime = CAARiseTransitSet.Compute(2447240.5, Alpha1, Delta1, Alpha2, Delta2, Alpha3, Delta3, 71.0833, 42.3333, -0.5667);

	  double Kpp = CAAPlanetaryPhenomena.K(1993.75, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);
      double MercuryInferiorConjunction = CAAPlanetaryPhenomena.Mean(Kpp, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);
      double MercuryInferiorConjunction2 = CAAPlanetaryPhenomena.True(Kpp, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);

      double Kpp2 = CAAPlanetaryPhenomena.K(2125.5, CAAPlanetaryPhenomena.PlanetaryObject.SATURN, CAAPlanetaryPhenomena.EventType.CONJUNCTION);
      double SaturnConjunction = CAAPlanetaryPhenomena.Mean(Kpp2, CAAPlanetaryPhenomena.PlanetaryObject.SATURN, CAAPlanetaryPhenomena.EventType.CONJUNCTION);
      double SaturnConjunction2 = CAAPlanetaryPhenomena.True(Kpp2, CAAPlanetaryPhenomena.PlanetaryObject.SATURN, CAAPlanetaryPhenomena.EventType.CONJUNCTION);

      double MercuryWesternElongation = CAAPlanetaryPhenomena.True(Kpp, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.WESTERN_ELONGATION);
      double MercuryWesternElongationValue = CAAPlanetaryPhenomena.ElongationValue(Kpp, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, false);

      double MarsStation2 = CAAPlanetaryPhenomena.True(-2, CAAPlanetaryPhenomena.PlanetaryObject.MARS, CAAPlanetaryPhenomena.EventType.STATION2);

      double MercuryK = CAAPlanetaryPhenomena.K(1631.8, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);
      double MercuryIC = CAAPlanetaryPhenomena.True(MercuryK, CAAPlanetaryPhenomena.PlanetaryObject.MERCURY, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);

      double VenusKpp = CAAPlanetaryPhenomena.K(1882.9, CAAPlanetaryPhenomena.PlanetaryObject.VENUS, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);
      double VenusIC = CAAPlanetaryPhenomena.True(VenusKpp, CAAPlanetaryPhenomena.PlanetaryObject.VENUS, CAAPlanetaryPhenomena.EventType.INFERIOR_CONJUNCTION);

      double MarsKpp = CAAPlanetaryPhenomena.K(2729.65, CAAPlanetaryPhenomena.PlanetaryObject.MARS, CAAPlanetaryPhenomena.EventType.OPPOSITION);
      double MarsOP = CAAPlanetaryPhenomena.True(MarsKpp, CAAPlanetaryPhenomena.PlanetaryObject.MARS, CAAPlanetaryPhenomena.EventType.OPPOSITION);

      double JupiterKpp = CAAPlanetaryPhenomena.K(-5, CAAPlanetaryPhenomena.PlanetaryObject.JUPITER, CAAPlanetaryPhenomena.EventType.OPPOSITION);
      double JupiterOP = CAAPlanetaryPhenomena.True(JupiterKpp, CAAPlanetaryPhenomena.PlanetaryObject.JUPITER, CAAPlanetaryPhenomena.EventType.OPPOSITION);

      double SaturnKpp = CAAPlanetaryPhenomena.K(-5, CAAPlanetaryPhenomena.PlanetaryObject.SATURN, CAAPlanetaryPhenomena.EventType.OPPOSITION);
      double SaturnOP = CAAPlanetaryPhenomena.True(SaturnKpp, CAAPlanetaryPhenomena.PlanetaryObject.SATURN, CAAPlanetaryPhenomena.EventType.OPPOSITION);

      double UranusKpp = CAAPlanetaryPhenomena.K(1780.6, CAAPlanetaryPhenomena.PlanetaryObject.URANUS, CAAPlanetaryPhenomena.EventType.OPPOSITION);
      double UranusOP = CAAPlanetaryPhenomena.True(UranusKpp, CAAPlanetaryPhenomena.PlanetaryObject.URANUS, CAAPlanetaryPhenomena.EventType.OPPOSITION);

      double NeptuneKpp = CAAPlanetaryPhenomena.K(1846.5, CAAPlanetaryPhenomena.PlanetaryObject.NEPTUNE, CAAPlanetaryPhenomena.EventType.OPPOSITION);
      double NeptuneOP = CAAPlanetaryPhenomena.True(NeptuneKpp, CAAPlanetaryPhenomena.PlanetaryObject.NEPTUNE, CAAPlanetaryPhenomena.EventType.OPPOSITION);

	  CAA2DCoordinate TopocentricDelta = CAAParallax.Equatorial2TopocentricDelta(CAACoordinateTransformation.DMSToDegrees(22, 38, 7.25), -15.771083, 0.37276, CAACoordinateTransformation.DMSToDegrees(7, 47, 27)*15, CAACoordinateTransformation.DMSToDegrees(33, 21, 22), 1706, 2452879.63681);
	  CAA2DCoordinate Topocentric = CAAParallax.Equatorial2Topocentric(CAACoordinateTransformation.DMSToDegrees(22, 38, 7.25), -15.771083, 0.37276, CAACoordinateTransformation.DMSToDegrees(7, 47, 27)*15, CAACoordinateTransformation.DMSToDegrees(33, 21, 22), 1706, 2452879.63681);


	  double distance2 = CAAParallax.ParallaxToDistance(CAACoordinateTransformation.DMSToDegrees(0, 59, 27.7));
	  double parallax2 = CAAParallax.DistanceToParallax(distance2);

	  CAATopocentricEclipticDetails TopocentricDetails = CAAParallax.Ecliptic2Topocentric(CAACoordinateTransformation.DMSToDegrees(181, 46, 22.5), CAACoordinateTransformation.DMSToDegrees(2, 17, 26.2), CAACoordinateTransformation.DMSToDegrees(0, 16, 15.5), CAAParallax.ParallaxToDistance(CAACoordinateTransformation.DMSToDegrees(0, 59, 27.7)), CAACoordinateTransformation.DMSToDegrees(23, 28, 0.8), 0, CAACoordinateTransformation.DMSToDegrees(50, 5, 7.8), 0, 2452879.150858);

	  double k = CAAIlluminatedFraction.IlluminatedFraction(0.724604, 0.983824, 0.910947);
	  double pa1 = CAAIlluminatedFraction.PhaseAngle(0.724604, 0.983824, 0.910947);
	  double pa = CAAIlluminatedFraction.PhaseAngle(0.724604, 0.983824, -2.62070, 26.11428, 88.35704, 0.910947);
	  double k2 = CAAIlluminatedFraction.IlluminatedFraction(pa);
	  double pa2 = CAAIlluminatedFraction.PhaseAngleRectangular(0.621746, -0.664810, -0.033134, -2.62070, 26.11428, 0.910947);
	  double k3 = CAAIlluminatedFraction.IlluminatedFraction(pa2);

	  double VenusMag = CAAIlluminatedFraction.VenusMagnitudeMuller(0.724604, 0.910947, 72.96);
	  double VenusMag2 = CAAIlluminatedFraction.VenusMagnitudeAA(0.724604, 0.910947, 72.96);

	  double SaturnMag = CAAIlluminatedFraction.SaturnMagnitudeMuller(9.867882, 10.464606, 4.198, 16.442);
	  double SaturnMag2 = CAAIlluminatedFraction.SaturnMagnitudeAA(9.867882, 10.464606, 4.198, 16.442);


	  CAAPhysicalMarsDetails MarsDetails = CAAPhysicalMars.Calculate(2448935.500683);

	  CAAPhysicalJupiterDetails JupiterDetails = CAAPhysicalJupiter.Calculate(2448972.50068);

	  //The example as given in the book
	  CAAGalileanMoonsDetails GalileanDetails = CAAGalileanMoons.Calculate(2448972.50068);

	  //Calculate the Eclipse Disappearance of Satellite 1 on February 1 2004 at 13:32 UCT
	  double JD = 2453037.05903;
	  int i;
	  for (i =0; i<10; i++)
	  {
		CAAGalileanMoonsDetails GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  //Calculate the Shadow Egress of Satellite 1 on February 2  2004 at 13:07 UT
	  JD = 2453038.04236;
	  for (i =0; i<10; i++)
	  {
		CAAGalileanMoonsDetails GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  //Calculate the Shadow Ingress of Satellite 4 on February 6 2004 at 22:59 UCT
	  JD = 2453042.45486;
	  for (i =0; i<10; i++)
	  {
		CAAGalileanMoonsDetails GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  //Calculate the Shadow Egress of Satellite 4 on February 7 2004 at 2:41 UCT
	  JD = 2453042.61042;
	  for (i =0; i<10; i++)
	  {
		CAAGalileanMoonsDetails GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  //Calculate the Transit Ingress of Satellite 4 on February 7 2004 at 5:07 UCT
	  JD = 2453042.71181;
	  for (i =0; i<10; i++)
	  {
		CAAGalileanMoonsDetails GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  //Calculate the Transit Egress of Satellite 4 on February 7 2004 at 7:46 UT
	  JD = 2453042.82222;
	  for (i =0; i<10; i++)
	  {
		CAAGalileanMoonsDetails GalileanDetails1 = CAAGalileanMoons.Calculate(JD);
		JD += (1.0/1440);
	  }

	  CAASaturnRingDetails saturnrings = CAASaturnRings.Calculate(2448972.50068);

	  CAASaturnMoonsDetails saturnMoons = CAASaturnMoons.Calculate(2451439.50074);

	  double ApproxK = CAAMoonPhases.K(1977.13);
	  double NewMoonJD = CAAMoonPhases.TruePhase(-283);

	  double ApproxK2 = CAAMoonPhases.K(2044);
	  double LastQuarterJD = CAAMoonPhases.TruePhase(544.75);

	  double MoonDeclinationK = CAAMoonMaxDeclinations.K(1988.95);

	  double MoonNorthDec = CAAMoonMaxDeclinations.TrueGreatestDeclination(-148, true);
	  double MoonNorthDecValue = CAAMoonMaxDeclinations.TrueGreatestDeclinationValue(-148, true);

	  double MoonSouthDec = CAAMoonMaxDeclinations.TrueGreatestDeclination(659, false);
	  double MoonSouthDecValue = CAAMoonMaxDeclinations.TrueGreatestDeclinationValue(659, false);

	  double MoonNorthDec2 = CAAMoonMaxDeclinations.TrueGreatestDeclination(-26788, true);
	  double MoonNorthDecValue2 = CAAMoonMaxDeclinations.TrueGreatestDeclinationValue(-26788, true);

	  double sd1 = CAADiameters.SunSemidiameterA(1);
	  double sd2 = CAADiameters.SunSemidiameterA(2);

	  double sd3 = CAADiameters.VenusSemidiameterA(1);
	  double sd4 = CAADiameters.VenusSemidiameterA(2);
	  double sd5 = CAADiameters.VenusSemidiameterB(1);
	  double sd6 = CAADiameters.VenusSemidiameterB(2);

	  double sd11 = CAADiameters.MarsSemidiameterA(1);
	  double sd12 = CAADiameters.MarsSemidiameterA(2);
	  double sd13 = CAADiameters.MarsSemidiameterB(1);
	  double sd14 = CAADiameters.MarsSemidiameterB(2);

	  double sd15 = CAADiameters.JupiterEquatorialSemidiameterA(1);
	  double sd16 = CAADiameters.JupiterEquatorialSemidiameterA(2);
	  double sd17 = CAADiameters.JupiterEquatorialSemidiameterB(1);
	  double sd18 = CAADiameters.JupiterEquatorialSemidiameterB(2);

	  double sd19 = CAADiameters.JupiterPolarSemidiameterA(1);
	  double sd20 = CAADiameters.JupiterPolarSemidiameterA(2);
	  double sd21 = CAADiameters.JupiterPolarSemidiameterB(1);
	  double sd22 = CAADiameters.JupiterPolarSemidiameterB(2);

	  double sd23 = CAADiameters.SaturnEquatorialSemidiameterA(1);
	  double sd24 = CAADiameters.SaturnEquatorialSemidiameterA(2);
	  double sd25 = CAADiameters.SaturnEquatorialSemidiameterB(1);
	  double sd26 = CAADiameters.SaturnEquatorialSemidiameterB(2);

	  double sd27 = CAADiameters.SaturnPolarSemidiameterA(1);
	  double sd28 = CAADiameters.SaturnPolarSemidiameterA(2);
	  double sd29 = CAADiameters.SaturnPolarSemidiameterB(1);
	  double sd30 = CAADiameters.SaturnPolarSemidiameterB(2);

	  double sd31 = CAADiameters.ApparentSaturnPolarSemidiameterA(1, 16.442);
	  double sd32 = CAADiameters.ApparentSaturnPolarSemidiameterA(2, 16.442);

	  double sd33 = CAADiameters.UranusSemidiameterA(1);
	  double sd34 = CAADiameters.UranusSemidiameterA(2);
	  double sd35 = CAADiameters.UranusSemidiameterB(1);
	  double sd36 = CAADiameters.UranusSemidiameterB(2);

	  double sd37 = CAADiameters.NeptuneSemidiameterA(1);
	  double sd38 = CAADiameters.NeptuneSemidiameterA(2);
	  double sd39 = CAADiameters.NeptuneSemidiameterB(1);
	  double sd40 = CAADiameters.NeptuneSemidiameterB(2);

	  double sd41 = CAADiameters.PlutoSemidiameterB(1);
	  double sd42 = CAADiameters.PlutoSemidiameterB(2);

	  double sd43 = CAADiameters.GeocentricMoonSemidiameter(368407.9);
	  double sd44 = CAADiameters.GeocentricMoonSemidiameter(368407.9 - 10000);

	  double sd45 = CAADiameters.TopocentricMoonSemidiameter(368407.9, 5, 0, 33.356111, 1706);
	  double sd46 = CAADiameters.TopocentricMoonSemidiameter(368407.9, 5, 6, 33.356111, 1706);
	  double sd47 = CAADiameters.TopocentricMoonSemidiameter(368407.9 - 10000, 5, 0, 33.356111, 1706);
	  double sd48 = CAADiameters.TopocentricMoonSemidiameter(368407.9 - 10000, 5, 6, 33.356111, 1706);

	  double sd49 = CAADiameters.AsteroidDiameter(4, 0.04);
	  double sd50 = CAADiameters.AsteroidDiameter(4, 0.08);
	  double sd51 = CAADiameters.AsteroidDiameter(6, 0.04);
	  double sd53 = CAADiameters.AsteroidDiameter(6, 0.08);
	  double sd54 = CAADiameters.ApparentAsteroidDiameter(1, 250);
	  double sd55 = CAADiameters.ApparentAsteroidDiameter(1, 1000);

	  CAAPhysicalMoonDetails MoonDetails = CAAPhysicalMoon.CalculateGeocentric(2448724.5);
	  CAAPhysicalMoonDetails MoonDetail2 = CAAPhysicalMoon.CalculateTopocentric(2448724.5, 10, 52);
	  CAASelenographicMoonDetails CAASelenographicMoonDetails = CAAPhysicalMoon.CalculateSelenographicPositionOfSun(2448724.5);

	  double AltitudeOfSun = CAAPhysicalMoon.AltitudeOfSun(2448724.5, -20, 9.7);
	  double TimeOfSunrise = CAAPhysicalMoon.TimeOfSunrise(2448724.5, -20, 9.7);
	  double TimeOfSunset = CAAPhysicalMoon.TimeOfSunset(2448724.5, -20, 9.7);

	  CAASolarEclipseDetails EclipseDetails = CAAEclipses.CalculateSolar(-82);
	  CAASolarEclipseDetails EclipseDetails2 = CAAEclipses.CalculateSolar(118);
	  CAALunarEclipseDetails EclipseDetails3 = CAAEclipses.CalculateLunar(-328.5);
	  CAALunarEclipseDetails EclipseDetails4 = CAAEclipses.CalculateLunar(-30.5); //No lunar eclipse
	  EclipseDetails4 = CAAEclipses.CalculateLunar(-29.5); //No lunar eclipse
	  EclipseDetails4 = CAAEclipses.CalculateLunar(-28.5); //Aha, found you!

	  CAACalendarDate JulianDate1 = CAAMoslemCalendar.MoslemToJulian(1421, 1, 1);
	  CAACalendarDate GregorianDate1 = CAADate.JulianToGregorian(JulianDate1.Year, JulianDate1.Month, JulianDate1.Day);
	  CAACalendarDate JulianDate2 = CAADate.GregorianToJulian(GregorianDate1.Year, GregorianDate1.Month, GregorianDate1.Day);
	  CAACalendarDate MoslemDate = CAAMoslemCalendar.JulianToMoslem(JulianDate2.Year, JulianDate2.Month, JulianDate2.Day);
	  bLeap = CAAMoslemCalendar.IsLeap(1421);

	  MoslemDate = CAAMoslemCalendar.JulianToMoslem(2006, 12, 31);
	  CAACalendarDate OriginalMoslemDate = CAAMoslemCalendar.MoslemToJulian(MoslemDate.Year, MoslemDate.Month, MoslemDate.Day);
	  MoslemDate = CAAMoslemCalendar.JulianToMoslem(2007, 1, 1);
	  OriginalMoslemDate = CAAMoslemCalendar.MoslemToJulian(MoslemDate.Year, MoslemDate.Month, MoslemDate.Day);

	  CAACalendarDate JulianDate3 = CAADate.GregorianToJulian(1991, 8, 13);
	  CAACalendarDate MoslemDate2 = CAAMoslemCalendar.JulianToMoslem(JulianDate3.Year, JulianDate3.Month, JulianDate3.Day);
	  CAACalendarDate JulianDate4 = CAAMoslemCalendar.MoslemToJulian(MoslemDate2.Year, MoslemDate2.Month, MoslemDate2.Day);
	  CAACalendarDate GregorianDate2 = CAADate.JulianToGregorian(JulianDate4.Year, JulianDate4.Month, JulianDate4.Day);

	  CAACalendarDate JewishDate = CAAJewishCalendar.DateOfPesach(1990);
	  bLeap = CAAJewishCalendar.IsLeap(JewishDate.Year);
	  bLeap = CAAJewishCalendar.IsLeap(5751);
	  int DaysInJewishYear = CAAJewishCalendar.DaysInYear(JewishDate.Year);
	  DaysInJewishYear = CAAJewishCalendar.DaysInYear(5751);


	  CAANearParabolicObjectElements elements6 = new CAANearParabolicObjectElements();
	  elements6.q = 0.921326;
	  elements6.e = 1;
	  elements6.i = 0; //unknown
	  elements6.omega = 0; //unknown
	  elements6.w = 0; //unknown
	  elements6.T = 0;
	  elements6.JDEquinox = 0;
	  CAANearParabolicObjectDetails details3 = CAANearParabolic.Calculate(138.4783, elements6);

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

	  CAAEclipticalElementDetails ed5 = CAAEclipticalElements.Calculate(131.5856, 242.6797, 138.6637, 2433282.4235, 2448188.500000 + 0.6954-63.6954);
	  CAAEclipticalElementDetails ed6 = CAAEclipticalElements.Calculate(131.5856, 242.6797, 138.6637, 2433282.4235, 2433282.4235);
	  CAAEclipticalElementDetails ed7 = CAAEclipticalElements.FK4B1950ToFK5J2000(131.5856, 242.6797, 138.6637);

	  elements6.q = 0.93858;
	  elements6.e = 1.000270;
	  elements6.i = ed5.i;
	  elements6.omega = ed5.omega;
	  elements6.w = ed5.w;
	  elements6.T = 2448188.500000 + 0.6954;
	  elements6.JDEquinox = elements6.T;
	  CAANearParabolicObjectDetails details4 = CAANearParabolic.Calculate(elements6.T-63.6954, elements6);

	  return 0;
	}

#endif
}
