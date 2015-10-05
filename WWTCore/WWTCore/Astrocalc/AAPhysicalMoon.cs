using System;
//
//Module : AAPHYSICALMOON.CPP
//Purpose: Implementation for the algorithms which obtain the physical parameters of the Moon
//Created: PJN / 17-01-2004
//History: PJN / 19-02-2004 1. The optical libration in longitude is now returned in the range -180 - 180 degrees
//
//Copyright (c) 2004 - 2007 by PJ Naughter (Web: www.naughter.com, Email: pjna@naughter.com)
//
//All rights reserved.
//
//Copyright / Usage Details:
//
//You are allowed to include the source code in any product (commercial, shareware, freeware or otherwise) 
//when your product is released in binary form. You are allowed to modify the source code in any way you want 
//except you cannot modify the copyright details at the top of each module. If you want to distribute source 
//code with your application, then you are only allowed to distribute versions released by the author. This is 
//to maintain a single distribution point for the source code. 
//
//
// Converted to c# and distributed with WWT by permision of PJ Naughter by Jonathan Fay
// Please refer to http://www.naughter.com/aa.html for orginal C++ versions
//

///////////////////////////////// Includes ////////////////////////////////////



/////////////////////// Includes //////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAPhysicalMoonDetails
{
//Constructors / Destructors
  public CAAPhysicalMoonDetails()
  {
	  ldash = 0;
	  bdash = 0;
	  ldash2 = 0;
	  bdash2 = 0;
	  l = 0;
	  b = 0;
	  P = 0;
  }

//Member variables
  public double ldash;
  public double bdash;
  public double ldash2;
  public double bdash2;
  public double l;
  public double b;
  public double P;
}

public class  CAASelenographicMoonDetails
{
//Constructors / Destructors
  public CAASelenographicMoonDetails()
  {
	  l0 = 0;
	  b0 = 0;
	  c0 = 0;
  }

//Member variables
  public double l0;
  public double b0;
  public double c0;
}

public class  CAAPhysicalMoon
{
//Static methods
  public static CAAPhysicalMoonDetails CalculateGeocentric(double JD)
  {
	double Lambda=0;
	double Beta=0;
	double epsilon=0;
	var Equatorial = new CAA2DCoordinate();
	return CalculateHelper(JD, ref Lambda, ref Beta, ref epsilon, ref Equatorial);
  }
  public static CAAPhysicalMoonDetails CalculateTopocentric(double JD, double Longitude, double Latitude)
  {
	//First convert to radians
	Longitude = CAACoordinateTransformation.DegreesToRadians(Longitude);
	Latitude = CAACoordinateTransformation.DegreesToRadians(Latitude);
  
	double Lambda=0;
	double Beta=0;
	double epsilon=0;
	var Equatorial = new CAA2DCoordinate();
	var details = CalculateHelper(JD, ref Lambda, ref Beta, ref epsilon, ref Equatorial);
  
	var R = CAAMoon.RadiusVector(JD);
	var pi = CAAMoon.RadiusVectorToHorizontalParallax(R);
	var Alpha = CAACoordinateTransformation.HoursToRadians(Equatorial.X);
	var Delta = CAACoordinateTransformation.DegreesToRadians(Equatorial.Y);
  
	var AST = CAASidereal.ApparentGreenwichSiderealTime(JD);
	var H = CAACoordinateTransformation.HoursToRadians(AST) - Longitude - Alpha;
  
	var Q = Math.Atan2(Math.Cos(Latitude)*Math.Sin(H), Math.Cos(Delta)*Math.Sin(Latitude) - Math.Sin(Delta)*Math.Cos(Latitude)*Math.Cos(H));
	var Z = Math.Acos(Math.Sin(Delta)*Math.Sin(Latitude) + Math.Cos(Delta)*Math.Cos(Latitude)*Math.Cos(H));
	var pidash = pi*(Math.Sin(Z) + 0.0084 *Math.Sin(2 *Z));
  
	var Prad = CAACoordinateTransformation.DegreesToRadians(details.P);
  
	var DeltaL = -pidash *Math.Sin(Q - Prad)/Math.Cos(CAACoordinateTransformation.DegreesToRadians(details.b));
	details.l += DeltaL;
	var DeltaB = pidash *Math.Cos(Q - Prad);
	details.b += DeltaB;
	details.P += DeltaL *Math.Sin(CAACoordinateTransformation.DegreesToRadians(details.b)) - pidash *Math.Sin(Q)*Math.Tan(Delta);
  
	return details;
  }
  public static CAASelenographicMoonDetails CalculateSelenographicPositionOfSun(double JD)
  {
	var R = CAAEarth.RadiusVector(JD)*149597970;
	var Delta = CAAMoon.RadiusVector(JD);
	var lambda0 = CAASun.ApparentEclipticLongitude(JD);
	var lambda = CAAMoon.EclipticLongitude(JD);
	var beta = CAAMoon.EclipticLatitude(JD);
  
	var lambdah = CAACoordinateTransformation.MapTo0To360Range(lambda0 + 180 + Delta/R *57.296 *Math.Cos(CAACoordinateTransformation.DegreesToRadians(beta))*Math.Sin(CAACoordinateTransformation.DegreesToRadians(lambda0 - lambda)));
	var betah = Delta/R *beta;
  
	//What will be the return value
	var details = new CAASelenographicMoonDetails();
  
	//Calculate the optical libration
	double omega=0;
    double DeltaU = 0;
    double sigma = 0;
    double I = 0;
    double rho = 0;
    double ldash0 = 0;
    double bdash0 = 0;
    double ldash20 = 0;
    double bdash20 = 0;
    double epsilon = 0;
	CalculateOpticalLibration(JD, lambdah, betah, ref ldash0, ref bdash0, ref ldash20, ref bdash20, ref epsilon, ref omega, ref DeltaU, ref sigma, ref I, ref rho);
  
	details.l0 = ldash0 + ldash20;
	details.b0 = bdash0 + bdash20;
	details.c0 = CAACoordinateTransformation.MapTo0To360Range(450 - details.l0);
	return details;
  }
  public static double AltitudeOfSun(double JD, double Longitude, double Latitude)
  {
	//Calculate the selenographic details
	var selenographicDetails = CalculateSelenographicPositionOfSun(JD);
  
	//convert to radians
	Latitude = CAACoordinateTransformation.DegreesToRadians(Latitude);
	Longitude = CAACoordinateTransformation.DegreesToRadians(Longitude);
	selenographicDetails.b0 = CAACoordinateTransformation.DegreesToRadians(selenographicDetails.b0);
	selenographicDetails.c0 = CAACoordinateTransformation.DegreesToRadians(selenographicDetails.c0);
  
	return CAACoordinateTransformation.RadiansToDegrees(Math.Asin(Math.Sin(selenographicDetails.b0)*Math.Sin(Latitude) + Math.Cos(selenographicDetails.b0)*Math.Cos(Latitude)*Math.Sin(selenographicDetails.c0 + Longitude)));
  }
  public static double TimeOfSunrise(double JD, double Longitude, double Latitude)
  {
	return SunriseSunsetHelper(JD, Longitude, Latitude, true);
  }
  public static double TimeOfSunset(double JD, double Longitude, double Latitude)
  {
	return SunriseSunsetHelper(JD, Longitude, Latitude, false);
  }

  protected static double SunriseSunsetHelper(double JD, double Longitude, double Latitude, bool bSunrise)
  {
	var JDResult = JD;
	var Latituderad = CAACoordinateTransformation.DegreesToRadians(Latitude);
	double h;
	do
	{
	  h = AltitudeOfSun(JDResult, Longitude, Latitude);
	  var DeltaJD = h/(12.19075 *Math.Cos(Latituderad));
	  if (bSunrise)
		JDResult -= DeltaJD;
	  else
		JDResult += DeltaJD;
	}
	while (Math.Abs(h) > 0.001);
  
	return JDResult;
  }
  protected static CAAPhysicalMoonDetails CalculateHelper(double JD, ref double Lambda, ref double Beta, ref double epsilon, ref CAA2DCoordinate Equatorial)
  {
	//What will be the return value
	var details = new CAAPhysicalMoonDetails();
  
	//Calculate the initial quantities
	Lambda = CAAMoon.EclipticLongitude(JD);
	Beta = CAAMoon.EclipticLatitude(JD);
  
	//Calculate the optical libration
	double omega=0;
	double DeltaU=0;
	double sigma=0;
	double I=0;
	double rho=0;
	CalculateOpticalLibration(JD, Lambda, Beta, ref details.ldash, ref details.bdash, ref details.ldash2, ref details.bdash2, ref epsilon, ref omega, ref DeltaU, ref sigma, ref I, ref rho);
	var epsilonrad = CAACoordinateTransformation.DegreesToRadians(epsilon);
  
	//Calculate the total libration
	details.l = details.ldash + details.ldash2;
	details.b = details.bdash + details.bdash2;
	var b = CAACoordinateTransformation.DegreesToRadians(details.b);
  
	//Calculate the position angle
	var V = omega + DeltaU + CAACoordinateTransformation.DegreesToRadians(sigma)/Math.Sin(I);
	var I_rho = I + CAACoordinateTransformation.DegreesToRadians(rho);
	var X = Math.Sin(I_rho)*Math.Sin(V);
	var Y = Math.Sin(I_rho)*Math.Cos(V)*Math.Cos(epsilonrad) - Math.Cos(I_rho)*Math.Sin(epsilonrad);
	var w = Math.Atan2(X, Y);
  
	Equatorial = CAACoordinateTransformation.Ecliptic2Equatorial(Lambda, Beta, epsilon);
	var Alpha = CAACoordinateTransformation.HoursToRadians(Equatorial.X);
  
	details.P = CAACoordinateTransformation.RadiansToDegrees(Math.Asin(Math.Sqrt(X *X + Y *Y)*Math.Cos(Alpha - w)/(Math.Cos(b))));
  
	return details;
  }

  //////////////////////////////// Implementation ///////////////////////////////
  
  protected static void CalculateOpticalLibration(double JD, double Lambda, double Beta, ref double ldash, ref double bdash, ref double ldash2, ref double bdash2, ref double epsilon, ref double omega, ref double DeltaU, ref double sigma, ref double I, ref double rho)
  {
	//Calculate the initial quantities
	var Lambdarad = CAACoordinateTransformation.DegreesToRadians(Lambda);
	var Betarad = CAACoordinateTransformation.DegreesToRadians(Beta);
	I = CAACoordinateTransformation.DegreesToRadians(1.54242);
	DeltaU = CAACoordinateTransformation.DegreesToRadians(CAANutation.NutationInLongitude(JD)/3600);
	var F = CAACoordinateTransformation.DegreesToRadians(CAAMoon.ArgumentOfLatitude(JD));
	omega = CAACoordinateTransformation.DegreesToRadians(CAAMoon.MeanLongitudeAscendingNode(JD));
	epsilon = CAANutation.MeanObliquityOfEcliptic(JD) + CAANutation.NutationInObliquity(JD)/3600;
  
	//Calculate the optical librations
	var W = Lambdarad - DeltaU/3600 - omega;
	var A = Math.Atan2(Math.Sin(W)*Math.Cos(Betarad)*Math.Cos(I) - Math.Sin(Betarad)*Math.Sin(I), Math.Cos(W)*Math.Cos(Betarad));
	ldash = CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(A) - CAACoordinateTransformation.RadiansToDegrees(F));
	if (ldash > 180)
	  ldash -= 360;
	bdash = Math.Asin(-Math.Sin(W)*Math.Cos(Betarad)*Math.Sin(I) - Math.Sin(Betarad)*Math.Cos(I));
  
	//Calculate the physical librations
	var T = (JD - 2451545.0)/36525;
	var K1 = 119.75 + 131.849 *T;
	K1 = CAACoordinateTransformation.DegreesToRadians(K1);
	var K2 = 72.56 + 20.186 *T;
	K2 = CAACoordinateTransformation.DegreesToRadians(K2);
  
	var M = CAAEarth.SunMeanAnomaly(JD);
	M = CAACoordinateTransformation.DegreesToRadians(M);
	var Mdash = CAAMoon.MeanAnomaly(JD);
	Mdash = CAACoordinateTransformation.DegreesToRadians(Mdash);
	var D = CAAMoon.MeanElongation(JD);
	D = CAACoordinateTransformation.DegreesToRadians(D);
	var E = CAAEarth.Eccentricity(JD);
  
	rho = -0.02752 *Math.Cos(Mdash) + -0.02245 *Math.Sin(F) + 0.00684 *Math.Cos(Mdash - 2 *F) + -0.00293 *Math.Cos(2 *F) + -0.00085 *Math.Cos(2 *F - 2 *D) + -0.00054 *Math.Cos(Mdash - 2 *D) + -0.00020 *Math.Sin(Mdash + F) + -0.00020 *Math.Cos(Mdash + 2 *F) + -0.00020 *Math.Cos(Mdash - F) + 0.00014 *Math.Cos(Mdash + 2 *F - 2 *D);
  
	sigma = -0.02816 *Math.Sin(Mdash) + 0.02244 *Math.Cos(F) + -0.00682 *Math.Sin(Mdash - 2 *F) + -0.00279 *Math.Sin(2 *F) + -0.00083 *Math.Sin(2 *F - 2 *D) + 0.00069 *Math.Sin(Mdash - 2 *D) + 0.00040 *Math.Cos(Mdash + F) + -0.00025 *Math.Sin(2 *Mdash) + -0.00023 *Math.Sin(Mdash + 2 *F) + 0.00020 *Math.Cos(Mdash - F) + 0.00019 *Math.Sin(Mdash - F) + 0.00013 *Math.Sin(Mdash + 2 *F - 2 *D) + -0.00010 *Math.Cos(Mdash - 3 *F);
  
	var tau = 0.02520 *E *Math.Sin(M) + 0.00473 *Math.Sin(2 *Mdash - 2 *F) + -0.00467 *Math.Sin(Mdash) + 0.00396 *Math.Sin(K1) + 0.00276 *Math.Sin(2 *Mdash - 2 *D) + 0.00196 *Math.Sin(omega) + -0.00183 *Math.Cos(Mdash - F) + 0.00115 *Math.Sin(Mdash - 2 *D) + -0.00096 *Math.Sin(Mdash - D) + 0.00046 *Math.Sin(2 *F - 2 *D) + -0.00039 *Math.Sin(Mdash - F) + -0.00032 *Math.Sin(Mdash - M - D) + 0.00027 *Math.Sin(2 *Mdash - M - 2 *D) + 0.00023 *Math.Sin(K2) + -0.00014 *Math.Sin(2 *D) + 0.00014 *Math.Cos(2 *Mdash - 2 *F) + -0.00012 *Math.Sin(Mdash - 2 *F) + -0.00012 *Math.Sin(2 *Mdash) + 0.00011 *Math.Sin(2 *Mdash - 2 *M - 2 *D);
  
	ldash2 = -tau + (rho *Math.Cos(A) + sigma *Math.Sin(A))*Math.Tan(bdash);
	bdash = CAACoordinateTransformation.RadiansToDegrees(bdash);
	bdash2 = sigma *Math.Cos(A) - rho *Math.Sin(A);
  }
}
