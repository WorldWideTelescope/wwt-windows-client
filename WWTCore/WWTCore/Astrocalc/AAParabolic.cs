using System;
//
//Module : AAPARABOLIC.CPP
//Purpose: Implementation for the algorithms for a parabolic orbit
//Created: PJN / 29-12-2003
//History: PJN / 31-01-2005 1. Fixed a bug in CAAParabolic::Calculate where the JD value was being used incorrectly
//                          in the loop. Thanks to Mika Heiskanen for reporting this problem.
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

////////////////////////////// Includes ///////////////////////////////////////


/////////////////////// Includes //////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAParabolicObjectElements
{
//Constructors / Destructors
  public CAAParabolicObjectElements()
  {
	  q = 0;
	  i = 0;
	  w = 0;
	  omega = 0;
	  JDEquinox = 0;
	  T = 0;
  }

//Member variables
  public double q;
  public double i;
  public double w;
  public double omega;
  public double JDEquinox;
  public double T;
}

public class  CAAParabolicObjectDetails
{
//Constructors / Destructors
  public CAAParabolicObjectDetails()
  {
	  HeliocentricEclipticLongitude = 0;
	  HeliocentricEclipticLatitude = 0;
	  TrueGeocentricRA = 0;
	  TrueGeocentricDeclination = 0;
	  TrueGeocentricDistance = 0;
	  TrueGeocentricLightTime = 0;
	  AstrometricGeocenticRA = 0;
	  AstrometricGeocentricDeclination = 0;
	  AstrometricGeocentricDistance = 0;
	  AstrometricGeocentricLightTime = 0;
	  Elongation = 0;
	  PhaseAngle = 0;
  }

//Member variables
  public CAA3DCoordinate HeliocentricRectangularEquatorial = new CAA3DCoordinate();
  public CAA3DCoordinate HeliocentricRectangularEcliptical = new CAA3DCoordinate();
  public double HeliocentricEclipticLongitude;
  public double HeliocentricEclipticLatitude;
  public double TrueGeocentricRA;
  public double TrueGeocentricDeclination;
  public double TrueGeocentricDistance;
  public double TrueGeocentricLightTime;
  public double AstrometricGeocenticRA;
  public double AstrometricGeocentricDeclination;
  public double AstrometricGeocentricDistance;
  public double AstrometricGeocentricLightTime;
  public double Elongation;
  public double PhaseAngle;
}

public class  CAAParabolic
{
//Static methods

  ////////////////////////////// Implementation /////////////////////////////////
  
  public static double CalculateBarkers(double W)
  {
	var S = W / 3;
	var bRecalc = true;
	while (bRecalc)
	{
	  var S2 = S *S;
	  var NextS = (2 *S2 *S + W) / (3 * (S2 + 1));
  
	  //Prepare for the next loop around
	  bRecalc = (Math.Abs(NextS - S) > 0.000001);
	  S = NextS;
	}
  
	return S;
  }
  public static CAAParabolicObjectDetails Calculate(double JD, CAAParabolicObjectElements elements)
  {
	var Epsilon = CAANutation.MeanObliquityOfEcliptic(elements.JDEquinox);
  
	var JD0 = JD;
  
	//What will be the return value
	var details = new CAAParabolicObjectDetails();
  
	Epsilon = CAACoordinateTransformation.DegreesToRadians(Epsilon);
	var omega = CAACoordinateTransformation.DegreesToRadians(elements.omega);
	var w = CAACoordinateTransformation.DegreesToRadians(elements.w);
	var i = CAACoordinateTransformation.DegreesToRadians(elements.i);
  
	var sinEpsilon = Math.Sin(Epsilon);
	var cosEpsilon = Math.Cos(Epsilon);
	var sinOmega = Math.Sin(omega);
	var cosOmega = Math.Cos(omega);
	var cosi = Math.Cos(i);
	var sini = Math.Sin(i);
  
	var F = cosOmega;
	var G = sinOmega * cosEpsilon;
	var H = sinOmega * sinEpsilon;
	var P = -sinOmega * cosi;
	var Q = cosOmega *cosi *cosEpsilon - sini *sinEpsilon;
	var R = cosOmega *cosi *sinEpsilon + sini *cosEpsilon;
	var a = Math.Sqrt(F *F + P *P);
	var b = Math.Sqrt(G *G + Q *Q);
	var c = Math.Sqrt(H *H + R *R);
	var A = Math.Atan2(F, P);
	var B = Math.Atan2(G, Q);
	var C = Math.Atan2(H, R);
  
	var SunCoord = CAASun.EquatorialRectangularCoordinatesAnyEquinox(JD, elements.JDEquinox);
  
	for (var j =0; j<2; j++)
	{
	  var W = 0.03649116245/(elements.q * Math.Sqrt(elements.q)) * (JD0 - elements.T);
	  var s = CalculateBarkers(W);
	  var v = 2 *Math.Atan(s);
	  var r = elements.q * (1 + s *s);
	  var x = r * a * Math.Sin(A + w + v);
	  var y = r * b * Math.Sin(B + w + v);
	  var z = r * c * Math.Sin(C + w + v);
  
	  if (j == 0)
	  {
		details.HeliocentricRectangularEquatorial.X = x;
		details.HeliocentricRectangularEquatorial.Y = y;
		details.HeliocentricRectangularEquatorial.Z = z;
  
		//Calculate the heliocentric ecliptic coordinates also
		var u = omega + v;
		var cosu = Math.Cos(u);
		var sinu = Math.Sin(u);
  
		details.HeliocentricRectangularEcliptical.X = r * (cosOmega *cosu - sinOmega *sinu *cosi);
		details.HeliocentricRectangularEcliptical.Y = r * (sinOmega *cosu + cosOmega *sinu *cosi);
		details.HeliocentricRectangularEcliptical.Z = r *sini *sinu;
  
		details.HeliocentricEclipticLongitude = Math.Atan2(y, x);
		details.HeliocentricEclipticLongitude = CAACoordinateTransformation.MapTo0To24Range(CAACoordinateTransformation.RadiansToDegrees(details.HeliocentricEclipticLongitude) / 15);
		details.HeliocentricEclipticLatitude = Math.Asin(z / r);
		details.HeliocentricEclipticLatitude = CAACoordinateTransformation.RadiansToDegrees(details.HeliocentricEclipticLatitude);
	  }
  
	  var psi = SunCoord.X + x;
	  var nu = SunCoord.Y + y;
	  var sigma = SunCoord.Z + z;
  
	  var Alpha = Math.Atan2(nu, psi);
	  Alpha = CAACoordinateTransformation.RadiansToDegrees(Alpha);
	  var Delta = Math.Atan2(sigma, Math.Sqrt(psi *psi + nu *nu));
	  Delta = CAACoordinateTransformation.RadiansToDegrees(Delta);
	  var Distance = Math.Sqrt(psi *psi + nu *nu + sigma *sigma);
  
	  if (j == 0)
	  {
		details.TrueGeocentricRA = CAACoordinateTransformation.MapTo0To24Range(Alpha / 15);
		details.TrueGeocentricDeclination = Delta;
		details.TrueGeocentricDistance = Distance;
		details.TrueGeocentricLightTime = CAAElliptical.DistanceToLightTime(Distance);
	  }
	  else
	  {
		details.AstrometricGeocenticRA = CAACoordinateTransformation.MapTo0To24Range(Alpha / 15);
		details.AstrometricGeocentricDeclination = Delta;
		details.AstrometricGeocentricDistance = Distance;
		details.AstrometricGeocentricLightTime = CAAElliptical.DistanceToLightTime(Distance);
  
		var RES = Math.Sqrt(SunCoord.X *SunCoord.X + SunCoord.Y *SunCoord.Y + SunCoord.Z *SunCoord.Z);
  
		details.Elongation = CAACoordinateTransformation.RadiansToDegrees(Math.Acos((RES *RES + Distance *Distance - r *r) / (2 * RES * Distance)));
		details.PhaseAngle = CAACoordinateTransformation.RadiansToDegrees(Math.Acos((r *r + Distance *Distance - RES *RES) / (2 * r * Distance)));
	  }
  
	  if (j == 0) //Prepare for the next loop around
		JD0 = JD - details.TrueGeocentricLightTime;
	}
  
	return details;
  }
}
