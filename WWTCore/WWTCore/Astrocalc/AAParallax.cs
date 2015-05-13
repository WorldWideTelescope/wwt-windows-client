using System;
public static partial class GlobalMembersStdafx
{



	//////////////////////// Macros / Defines ///////////////////////////////////////////////

	public static double g_AAParallax_C1 = Math.Sin(CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, 8.794)));
}
//
//Module : AAPARALLAX.CPP
//Purpose: Implementation for the algorithms which convert a geocentric set of coordinates to their topocentric equivalent
//Created: PJN / 29-12-2003
//History: None
//
//Copyright (c) 2003 - 2007 by PJ Naughter (Web: www.naughter.com, Email: pjna@naughter.com)
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

/////////////////////////////////  Includes  //////////////////////////////////


/////////////////////// Includes //////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAATopocentricEclipticDetails
{
//Constructors / Destructors
  public CAATopocentricEclipticDetails()
  {
	  Lambda = 0;
	  Beta = 0;
	  Semidiameter = 0;
  }

//Member variables
  public double Lambda;
  public double Beta;
  public double Semidiameter;
}

public class  CAAParallax
{
//Conversion functions
  public static CAA2DCoordinate Equatorial2TopocentricDelta(double Alpha, double Delta, double Distance, double Longitude, double Latitude, double Height, double JD)
  {
	double RhoSinThetaPrime = CAAGlobe.RhoSinThetaPrime(Latitude, Height);
	double RhoCosThetaPrime = CAAGlobe.RhoCosThetaPrime(Latitude, Height);
  
	//Calculate the Sidereal time
	double theta = CAASidereal.ApparentGreenwichSiderealTime(JD);
  
	//Convert to radians
	Delta = CAACoordinateTransformation.DegreesToRadians(Delta);
	double cosDelta = Math.Cos(Delta);
  
	//Calculate the Parallax
	double pi = Math.Asin(GlobalMembersStdafx.g_AAParallax_C1 / Distance);
  
	//Calculate the hour angle
	double H = CAACoordinateTransformation.HoursToRadians(theta - Longitude/15 - Alpha);
	double cosH = Math.Cos(H);
	double sinH = Math.Sin(H);
  
	CAA2DCoordinate DeltaTopocentric = new CAA2DCoordinate();
	DeltaTopocentric.X = CAACoordinateTransformation.RadiansToHours(-pi *RhoCosThetaPrime *sinH/cosDelta);
	DeltaTopocentric.Y = CAACoordinateTransformation.RadiansToDegrees(-pi*(RhoSinThetaPrime *cosDelta - RhoCosThetaPrime *cosH *Math.Sin(Delta)));
	return DeltaTopocentric;
  }
  public static CAA2DCoordinate Equatorial2Topocentric(double Alpha, double Delta, double Distance, double Longitude, double Latitude, double Height, double JD)
  {
	double RhoSinThetaPrime = CAAGlobe.RhoSinThetaPrime(Latitude, Height);
	double RhoCosThetaPrime = CAAGlobe.RhoCosThetaPrime(Latitude, Height);
  
	//Calculate the Sidereal time
	double theta = CAASidereal.ApparentGreenwichSiderealTime(JD);
  
	//Convert to radians
	Delta = CAACoordinateTransformation.DegreesToRadians(Delta);
	double cosDelta = Math.Cos(Delta);
  
	//Calculate the Parallax
	double pi = Math.Asin(GlobalMembersStdafx.g_AAParallax_C1 / Distance);
	double sinpi = Math.Sin(pi);
  
	//Calculate the hour angle
	double H = CAACoordinateTransformation.HoursToRadians(theta - Longitude/15 - Alpha);
	double cosH = Math.Cos(H);
	double sinH = Math.Sin(H);
  
	//Calculate the adjustment in right ascension
	double DeltaAlpha = Math.Atan2(-RhoCosThetaPrime *sinpi *sinH, cosDelta - RhoCosThetaPrime *sinpi *cosH);
  
	CAA2DCoordinate Topocentric = new CAA2DCoordinate();
	Topocentric.X = CAACoordinateTransformation.MapTo0To24Range(Alpha + CAACoordinateTransformation.RadiansToHours(DeltaAlpha));
	Topocentric.Y = CAACoordinateTransformation.RadiansToDegrees(Math.Atan2((Math.Sin(Delta) - RhoSinThetaPrime *sinpi) * Math.Cos(DeltaAlpha), cosDelta - RhoCosThetaPrime *sinpi *cosH));
  
	return Topocentric;
  }
	public static CAATopocentricEclipticDetails Ecliptic2Topocentric(double Lambda, double Beta, double Semidiameter, double Distance, double Epsilon, double Longitude, double Latitude, double Height, double JD)
	{
	  double S = CAAGlobe.RhoSinThetaPrime(Latitude, Height);
	  double C = CAAGlobe.RhoCosThetaPrime(Latitude, Height);
	
	  //Convert to radians
	  Lambda = CAACoordinateTransformation.DegreesToRadians(Lambda);
	  Beta = CAACoordinateTransformation.DegreesToRadians(Beta);
	  Epsilon = CAACoordinateTransformation.DegreesToRadians(Epsilon);
	  Longitude = CAACoordinateTransformation.DegreesToRadians(Longitude);
	  Latitude = CAACoordinateTransformation.DegreesToRadians(Latitude);
	  Semidiameter = CAACoordinateTransformation.DegreesToRadians(Semidiameter);
	  double sine = Math.Sin(Epsilon);
	  double cose = Math.Cos(Epsilon);
	  double cosBeta = Math.Cos(Beta);
	  double sinBeta = Math.Sin(Beta);
	
	  //Calculate the Sidereal time
	  double theta = CAASidereal.ApparentGreenwichSiderealTime(JD);
	  theta = CAACoordinateTransformation.HoursToRadians(theta);
	  double sintheta = Math.Sin(theta);
	
	  //Calculate the Parallax
	  double pi = Math.Asin(GlobalMembersStdafx.g_AAParallax_C1 / Distance);
	  double sinpi = Math.Sin(pi);
	
	  double N = Math.Cos(Lambda)*cosBeta - C *sinpi *Math.Cos(theta);
	
	  CAATopocentricEclipticDetails Topocentric = new CAATopocentricEclipticDetails();
	  Topocentric.Lambda = Math.Atan2(Math.Sin(Lambda)*cosBeta - sinpi*(S *sine + C *cose *sintheta), N);
	  double cosTopocentricLambda = Math.Cos(Topocentric.Lambda);
	  Topocentric.Beta = Math.Atan(cosTopocentricLambda*(sinBeta - sinpi*(S *cose - C *sine *sintheta)) / N);
	  Topocentric.Semidiameter = Math.Asin(cosTopocentricLambda *Math.Cos(Topocentric.Beta)*Math.Sin(Semidiameter) / N);
	
	  //Convert back to degrees
	  Topocentric.Semidiameter = CAACoordinateTransformation.RadiansToDegrees(Topocentric.Semidiameter);
	  Topocentric.Lambda = CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(Topocentric.Lambda));
	  Topocentric.Beta = CAACoordinateTransformation.RadiansToDegrees(Topocentric.Beta);
	
		return Topocentric;
	}

  public static double ParallaxToDistance(double Parallax)
  {
	return GlobalMembersStdafx.g_AAParallax_C1 / Math.Sin(CAACoordinateTransformation.DegreesToRadians(Parallax));
  }

  //////////////////////// Implementation /////////////////////////////////////////////////
  
  public static double DistanceToParallax(double Distance)
  {
	double pi = Math.Asin(GlobalMembersStdafx.g_AAParallax_C1 / Distance);
	return CAACoordinateTransformation.RadiansToDegrees(pi);
  }
}
