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
	double S = W / 3;
	bool bRecalc = true;
	while (bRecalc)
	{
	  double S2 = S *S;
	  double NextS = (2 *S2 *S + W) / (3 * (S2 + 1));
  
	  //Prepare for the next loop around
	  bRecalc = (Math.Abs(NextS - S) > 0.000001);
	  S = NextS;
	}
  
	return S;
  }
  public static CAAParabolicObjectDetails Calculate(double JD, CAAParabolicObjectElements elements)
  {
	double Epsilon = CAANutation.MeanObliquityOfEcliptic(elements.JDEquinox);
  
	double JD0 = JD;
  
	//What will be the return value
	CAAParabolicObjectDetails details = new CAAParabolicObjectDetails();
  
	Epsilon = CAACoordinateTransformation.DegreesToRadians(Epsilon);
	double omega = CAACoordinateTransformation.DegreesToRadians(elements.omega);
	double w = CAACoordinateTransformation.DegreesToRadians(elements.w);
	double i = CAACoordinateTransformation.DegreesToRadians(elements.i);
  
	double sinEpsilon = Math.Sin(Epsilon);
	double cosEpsilon = Math.Cos(Epsilon);
	double sinOmega = Math.Sin(omega);
	double cosOmega = Math.Cos(omega);
	double cosi = Math.Cos(i);
	double sini = Math.Sin(i);
  
	double F = cosOmega;
	double G = sinOmega * cosEpsilon;
	double H = sinOmega * sinEpsilon;
	double P = -sinOmega * cosi;
	double Q = cosOmega *cosi *cosEpsilon - sini *sinEpsilon;
	double R = cosOmega *cosi *sinEpsilon + sini *cosEpsilon;
	double a = Math.Sqrt(F *F + P *P);
	double b = Math.Sqrt(G *G + Q *Q);
	double c = Math.Sqrt(H *H + R *R);
	double A = Math.Atan2(F, P);
	double B = Math.Atan2(G, Q);
	double C = Math.Atan2(H, R);
  
	CAA3DCoordinate SunCoord = CAASun.EquatorialRectangularCoordinatesAnyEquinox(JD, elements.JDEquinox);
  
	for (int j =0; j<2; j++)
	{
	  double W = 0.03649116245/(elements.q * Math.Sqrt(elements.q)) * (JD0 - elements.T);
	  double s = CalculateBarkers(W);
	  double v = 2 *Math.Atan(s);
	  double r = elements.q * (1 + s *s);
	  double x = r * a * Math.Sin(A + w + v);
	  double y = r * b * Math.Sin(B + w + v);
	  double z = r * c * Math.Sin(C + w + v);
  
	  if (j == 0)
	  {
		details.HeliocentricRectangularEquatorial.X = x;
		details.HeliocentricRectangularEquatorial.Y = y;
		details.HeliocentricRectangularEquatorial.Z = z;
  
		//Calculate the heliocentric ecliptic coordinates also
		double u = omega + v;
		double cosu = Math.Cos(u);
		double sinu = Math.Sin(u);
  
		details.HeliocentricRectangularEcliptical.X = r * (cosOmega *cosu - sinOmega *sinu *cosi);
		details.HeliocentricRectangularEcliptical.Y = r * (sinOmega *cosu + cosOmega *sinu *cosi);
		details.HeliocentricRectangularEcliptical.Z = r *sini *sinu;
  
		details.HeliocentricEclipticLongitude = Math.Atan2(y, x);
		details.HeliocentricEclipticLongitude = CAACoordinateTransformation.MapTo0To24Range(CAACoordinateTransformation.RadiansToDegrees(details.HeliocentricEclipticLongitude) / 15);
		details.HeliocentricEclipticLatitude = Math.Asin(z / r);
		details.HeliocentricEclipticLatitude = CAACoordinateTransformation.RadiansToDegrees(details.HeliocentricEclipticLatitude);
	  }
  
	  double psi = SunCoord.X + x;
	  double nu = SunCoord.Y + y;
	  double sigma = SunCoord.Z + z;
  
	  double Alpha = Math.Atan2(nu, psi);
	  Alpha = CAACoordinateTransformation.RadiansToDegrees(Alpha);
	  double Delta = Math.Atan2(sigma, Math.Sqrt(psi *psi + nu *nu));
	  Delta = CAACoordinateTransformation.RadiansToDegrees(Delta);
	  double Distance = Math.Sqrt(psi *psi + nu *nu + sigma *sigma);
  
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
  
		double RES = Math.Sqrt(SunCoord.X *SunCoord.X + SunCoord.Y *SunCoord.Y + SunCoord.Z *SunCoord.Z);
  
		details.Elongation = CAACoordinateTransformation.RadiansToDegrees(Math.Acos((RES *RES + Distance *Distance - r *r) / (2 * RES * Distance)));
		details.PhaseAngle = CAACoordinateTransformation.RadiansToDegrees(Math.Acos((r *r + Distance *Distance - RES *RES) / (2 * r * Distance)));
	  }
  
	  if (j == 0) //Prepare for the next loop around
		JD0 = JD - details.TrueGeocentricLightTime;
	}
  
	return details;
  }
}
