using System;
using System.Diagnostics;
public static partial class GlobalMembersStdafx
{

	public static MoonCoefficient1[] g_MoonCoefficients1 = { new MoonCoefficient1(0, 0, 1, 0), new MoonCoefficient1(2, 0, -1, 0), new MoonCoefficient1(2, 0, 0, 0), new MoonCoefficient1(0, 0, 2, 0), new MoonCoefficient1(0, 1, 0, 0), new MoonCoefficient1(0, 0, 0, 2), new MoonCoefficient1(2, 0, -2, 0), new MoonCoefficient1(2, -1, -1, 0), new MoonCoefficient1(2, 0, 1, 0), new MoonCoefficient1(2, -1, 0, 0), new MoonCoefficient1(0, 1, -1, 0), new MoonCoefficient1(1, 0, 0, 0), new MoonCoefficient1(0, 1, 1, 0), new MoonCoefficient1(2, 0, 0, -2), new MoonCoefficient1(0, 0, 1, 2), new MoonCoefficient1(0, 0, 1, -2), new MoonCoefficient1(4, 0, -1, 0), new MoonCoefficient1(0, 0, 3, 0), new MoonCoefficient1(4, 0, -2, 0), new MoonCoefficient1(2, 1, -1, 0), new MoonCoefficient1(2, 1, 0, 0), new MoonCoefficient1(1, 0, -1, 0), new MoonCoefficient1(1, 1, 0, 0), new MoonCoefficient1(2, -1, 1, 0), new MoonCoefficient1(2, 0, 2, 0), new MoonCoefficient1(4, 0, 0, 0), new MoonCoefficient1(2, 0, -3, 0), new MoonCoefficient1(0, 1, -2, 0), new MoonCoefficient1(2, 0, -1, 2), new MoonCoefficient1(2, -1, -2, 0), new MoonCoefficient1(1, 0, 1, 0), new MoonCoefficient1(2, -2, 0, 0), new MoonCoefficient1(0, 1, 2, 0), new MoonCoefficient1(0, 2, 0, 0), new MoonCoefficient1(2, -2, -1, 0), new MoonCoefficient1(2, 0, 1, -2), new MoonCoefficient1(2, 0, 0, 2), new MoonCoefficient1(4, -1, -1, 0), new MoonCoefficient1(0, 0, 2, 2), new MoonCoefficient1(3, 0, -1, 0), new MoonCoefficient1(2, 1, 1, 0), new MoonCoefficient1(4, -1, -2, 0), new MoonCoefficient1(0, 2, -1, 0), new MoonCoefficient1(2, 2, -1, 0), new MoonCoefficient1(2, 1, -2, 0), new MoonCoefficient1(2, -1, 0, -2), new MoonCoefficient1(4, 0, 1, 0), new MoonCoefficient1(0, 0, 4, 0), new MoonCoefficient1(4, -1, 0, 0), new MoonCoefficient1(1, 0, -2, 0), new MoonCoefficient1(2, 1, 0, -2), new MoonCoefficient1(0, 0, 2, -2), new MoonCoefficient1(1, 1, 1, 0), new MoonCoefficient1(3, 0, -2, 0), new MoonCoefficient1(4, 0, -3, 0), new MoonCoefficient1(2, -1, 2, 0), new MoonCoefficient1(0, 2, 1, 0), new MoonCoefficient1(1, 1, -1, 0), new MoonCoefficient1(2, 0, 3, 0), new MoonCoefficient1(2, 0, -1, -2) };

	public static MoonCoefficient2[] g_MoonCoefficients2 = { new MoonCoefficient2(6288774, -20905355), new MoonCoefficient2(1274027, -3699111), new MoonCoefficient2(658314, -2955968), new MoonCoefficient2(213618, -569925), new MoonCoefficient2(-185116, 48888), new MoonCoefficient2(-114332, -3149), new MoonCoefficient2(58793, 246158), new MoonCoefficient2(57066, -152138), new MoonCoefficient2(53322, -170733), new MoonCoefficient2(45758, -204586), new MoonCoefficient2(-40923, -129620), new MoonCoefficient2(-34720, 108743), new MoonCoefficient2(-30383, 104755), new MoonCoefficient2(15327, 10321), new MoonCoefficient2(-12528, 0), new MoonCoefficient2(10980, 79661), new MoonCoefficient2(10675, -34782), new MoonCoefficient2(10034, -23210), new MoonCoefficient2(8548, -21636), new MoonCoefficient2(-7888, 24208), new MoonCoefficient2(-6766, 30824), new MoonCoefficient2(-5163, -8379), new MoonCoefficient2(4987, -16675), new MoonCoefficient2(4036, -12831), new MoonCoefficient2(3994, -10445), new MoonCoefficient2(3861, -11650), new MoonCoefficient2(3665, 14403), new MoonCoefficient2(-2689, -7003), new MoonCoefficient2(-2602, 0), new MoonCoefficient2(2390, 10056), new MoonCoefficient2(-2348, 6322), new MoonCoefficient2(2236, -9884), new MoonCoefficient2(-2120, 5751), new MoonCoefficient2(-2069, 0), new MoonCoefficient2(2048, -4950), new MoonCoefficient2(-1773, 4130), new MoonCoefficient2(-1595, 0), new MoonCoefficient2(1215, -3958), new MoonCoefficient2(-1110, 0), new MoonCoefficient2(-892, 3258), new MoonCoefficient2(-810, 2616), new MoonCoefficient2(759, -1897), new MoonCoefficient2(-713, -2117), new MoonCoefficient2(-700, 2354), new MoonCoefficient2(691, 0), new MoonCoefficient2(596, 0), new MoonCoefficient2(549, -1423), new MoonCoefficient2(537, -1117), new MoonCoefficient2(520, -1571), new MoonCoefficient2(-487, -1739), new MoonCoefficient2(-399, 0), new MoonCoefficient2(-381, -4421), new MoonCoefficient2(351, 0), new MoonCoefficient2(-340, 0), new MoonCoefficient2(330, 0), new MoonCoefficient2(327, 0), new MoonCoefficient2(-323, 1165), new MoonCoefficient2(299, 0), new MoonCoefficient2(294, 0), new MoonCoefficient2(0, 8752) };

	public static MoonCoefficient1[] g_MoonCoefficients3 = { new MoonCoefficient1(0, 0, 0, 1), new MoonCoefficient1(0, 0, 1, 1), new MoonCoefficient1(0, 0, 1, -1), new MoonCoefficient1(2, 0, 0, -1), new MoonCoefficient1(2, 0, -1, 1), new MoonCoefficient1(2, 0, -1, -1), new MoonCoefficient1(2, 0, 0, 1), new MoonCoefficient1(0, 0, 2, 1), new MoonCoefficient1(2, 0, 1, -1), new MoonCoefficient1(0, 0, 2, -1), new MoonCoefficient1(2, -1, 0, -1), new MoonCoefficient1(2, 0, -2, -1), new MoonCoefficient1(2, 0, 1, 1), new MoonCoefficient1(2, 1, 0, -1), new MoonCoefficient1(2, -1, -1, 1), new MoonCoefficient1(2, -1, 0, 1), new MoonCoefficient1(2, -1, -1, -1), new MoonCoefficient1(0, 1, -1, -1), new MoonCoefficient1(4, 0, -1, -1), new MoonCoefficient1(0, 1, 0, 1), new MoonCoefficient1(0, 0, 0, 3), new MoonCoefficient1(0, 1, -1, 1), new MoonCoefficient1(1, 0, 0, 1), new MoonCoefficient1(0, 1, 1, 1), new MoonCoefficient1(0, 1, 1, -1), new MoonCoefficient1(0, 1, 0, -1), new MoonCoefficient1(1, 0, 0, -1), new MoonCoefficient1(0, 0, 3, 1), new MoonCoefficient1(4, 0, 0, -1), new MoonCoefficient1(4, 0, -1, 1), new MoonCoefficient1(0, 0, 1, -3), new MoonCoefficient1(4, 0, -2, 1), new MoonCoefficient1(2, 0, 0, -3), new MoonCoefficient1(2, 0, 2, -1), new MoonCoefficient1(2, -1, 1, -1), new MoonCoefficient1(2, 0, -2, 1), new MoonCoefficient1(0, 0, 3, -1), new MoonCoefficient1(2, 0, 2, 1), new MoonCoefficient1(2, 0, -3, -1), new MoonCoefficient1(2, 1, -1, 1), new MoonCoefficient1(2, 1, 0, 1), new MoonCoefficient1(4, 0, 0, 1), new MoonCoefficient1(2, -1, 1, 1), new MoonCoefficient1(2, -2, 0, -1), new MoonCoefficient1(0, 0, 1, 3), new MoonCoefficient1(2, 1, 1, -1), new MoonCoefficient1(1, 1, 0, -1), new MoonCoefficient1(1, 1, 0, 1), new MoonCoefficient1(0, 1, -2, -1), new MoonCoefficient1(2, 1, -1, -1), new MoonCoefficient1(1, 0, 1, 1), new MoonCoefficient1(2, -1, -2, -1), new MoonCoefficient1(0, 1, 2, 1), new MoonCoefficient1(4, 0, -2, -1), new MoonCoefficient1(4, -1, -1, -1), new MoonCoefficient1(1, 0, 1, -1), new MoonCoefficient1(4, 0, 1, -1), new MoonCoefficient1(1, 0, -1, -1), new MoonCoefficient1(4, -1, 0, -1), new MoonCoefficient1(2, -2, 0, 1) };

	public static double[] g_MoonCoefficients4 = { 5128122, 280602, 277693, 173237, 55413, 46271, 32573, 17198, 9266, 8822, 8216, 4324, 4200, -3359, 2463, 2211, 2065, -1870, 1828, -1794, -1749, -1565, -1491, -1475, -1410, -1344, -1335, 1107, 1021, 833, 777, 671, 607, 596, 491, -451, 439, 422, 421, -366, -351, 331, 315, 302, -283, -229, 223, 223, -220, -220, -185, 181, -177, 176, 166, -164, 132, -119, 115, 107};
}
//
//Module : AAMOON.CPP
//Purpose: Implementation for the algorithms which obtain the position of the Moon
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

//////////////////////////////// Includes /////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAMoon
{
//Static methods

  /////////////////////////////// Implementation ////////////////////////////////
  
  public static double MeanLongitude(double JD)
  {
	var T = (JD - 2451545) / 36525;
	var Tsquared = T *T;
	var Tcubed = Tsquared *T;
	var T4 = Tcubed *T;
	return CAACoordinateTransformation.MapTo0To360Range(218.3164477 + 481267.88123421 *T - 0.0015786 *Tsquared + Tcubed/538841 - T4/65194000);
  }
  public static double MeanElongation(double JD)
  {
	var T = (JD - 2451545) / 36525;
	var Tsquared = T *T;
	var Tcubed = Tsquared *T;
	var T4 = Tcubed *T;
	return CAACoordinateTransformation.MapTo0To360Range(297.8501921 + 445267.1114034 *T - 0.0018819 *Tsquared + Tcubed/545868 - T4/113065000);
  }
  public static double MeanAnomaly(double JD)
  {
	var T = (JD - 2451545) / 36525;
	var Tsquared = T *T;
	var Tcubed = Tsquared *T;
	var T4 = Tcubed *T;
	return CAACoordinateTransformation.MapTo0To360Range(134.9633964 + 477198.8675055 *T + 0.0087414 *Tsquared + Tcubed/69699 - T4/14712000);
  }
  public static double ArgumentOfLatitude(double JD)
  {
	var T = (JD - 2451545) / 36525;
	var Tsquared = T *T;
	var Tcubed = Tsquared *T;
	var T4 = Tcubed *T;
	return CAACoordinateTransformation.MapTo0To360Range(93.2720950 + 483202.0175233 *T - 0.0036539 *Tsquared - Tcubed/3526000 + T4/863310000);
  }
  public static double MeanLongitudeAscendingNode(double JD)
  {
	var T = (JD - 2451545) / 36525;
	var Tsquared = T *T;
	var Tcubed = Tsquared *T;
	var T4 = Tcubed *T;
	return CAACoordinateTransformation.MapTo0To360Range(125.0445479 - 1934.1362891 *T + 0.0020754 *Tsquared + Tcubed/467441 - T4/60616000);
  }
  public static double MeanLongitudePerigee(double JD)
  {
	var T = (JD - 2451545) / 36525;
	var Tsquared = T *T;
	var Tcubed = Tsquared *T;
	var T4 = Tcubed *T;
	return CAACoordinateTransformation.MapTo0To360Range(83.3532465 + 4069.0137287 *T - 0.0103200 *Tsquared - Tcubed/80053 + T4/18999000);
  }
  public static double TrueLongitudeAscendingNode(double JD)
  {
	var TrueAscendingNode = MeanLongitudeAscendingNode(JD);
  
	var D = MeanElongation(JD);
	D = CAACoordinateTransformation.DegreesToRadians(D);
	var M = CAAEarth.SunMeanAnomaly(JD);
	M = CAACoordinateTransformation.DegreesToRadians(M);
	var Mdash = MeanAnomaly(JD);
	Mdash = CAACoordinateTransformation.DegreesToRadians(Mdash);
	var F = ArgumentOfLatitude(JD);
	F = CAACoordinateTransformation.DegreesToRadians(F);
  
	//Add the principal additive terms
	TrueAscendingNode -= 1.4979 *Math.Sin(2*(D - F));
	TrueAscendingNode -= 0.1500 *Math.Sin(M);
	TrueAscendingNode -= 0.1226 *Math.Sin(2 *D);
	TrueAscendingNode += 0.1176 *Math.Sin(2 *F);
	TrueAscendingNode -= 0.0801 *Math.Sin(2*(Mdash-F));
  
	return CAACoordinateTransformation.MapTo0To360Range(TrueAscendingNode);
  }

  public static double EclipticLongitude(double JD)
  {
	var Ldash = MeanLongitude(JD);
	var LdashDegrees = Ldash;
	Ldash = CAACoordinateTransformation.DegreesToRadians(Ldash);
	var D = MeanElongation(JD);
	D = CAACoordinateTransformation.DegreesToRadians(D);
	var M = CAAEarth.SunMeanAnomaly(JD);
	M = CAACoordinateTransformation.DegreesToRadians(M);
	var Mdash = MeanAnomaly(JD);
	Mdash = CAACoordinateTransformation.DegreesToRadians(Mdash);
	var F = ArgumentOfLatitude(JD);
	F = CAACoordinateTransformation.DegreesToRadians(F);
  
	var E = CAAEarth.Eccentricity(JD);
	var T = (JD - 2451545) / 36525;
  
	var A1 = CAACoordinateTransformation.MapTo0To360Range(119.75 + 131.849 *T);
	A1 = CAACoordinateTransformation.DegreesToRadians(A1);
	var A2 = CAACoordinateTransformation.MapTo0To360Range(53.09 + 479264.290 *T);
	A2 = CAACoordinateTransformation.DegreesToRadians(A2);
	var A3 = CAACoordinateTransformation.MapTo0To360Range(313.45 + 481266.484 *T);
	A3 = CAACoordinateTransformation.DegreesToRadians(A3);
  
	var nLCoefficients =GlobalMembersStdafx.g_MoonCoefficients1.Length;
	Debug.Assert(GlobalMembersStdafx.g_MoonCoefficients2.Length == nLCoefficients);
	double SigmaL = 0;
	for (var i =0; i<nLCoefficients; i++)
	{
	  var ThisSigma = GlobalMembersStdafx.g_MoonCoefficients2[i].A * Math.Sin(GlobalMembersStdafx.g_MoonCoefficients1[i].D *D + GlobalMembersStdafx.g_MoonCoefficients1[i].M *M + GlobalMembersStdafx.g_MoonCoefficients1[i].Mdash *Mdash + GlobalMembersStdafx.g_MoonCoefficients1[i].F *F);
  
	  if (GlobalMembersStdafx.g_MoonCoefficients1[i].M != 0)
		ThisSigma *= E;
  
	  SigmaL += ThisSigma;
	}
  
	//Finally the additive terms
	SigmaL += 3958 *Math.Sin(A1);
	SigmaL += 1962 *Math.Sin(Ldash - F);
	SigmaL += 318 *Math.Sin(A2);
  
	//And finally apply the nutation in longitude
	var NutationInLong = CAANutation.NutationInLongitude(JD);
  
	return CAACoordinateTransformation.MapTo0To360Range(LdashDegrees + SigmaL/1000000 + NutationInLong/3600);
  }
  public static double EclipticLatitude(double JD)
  {
	var Ldash = MeanLongitude(JD);
	Ldash = CAACoordinateTransformation.DegreesToRadians(Ldash);
	var D = MeanElongation(JD);
	D = CAACoordinateTransformation.DegreesToRadians(D);
	var M = CAAEarth.SunMeanAnomaly(JD);
	M = CAACoordinateTransformation.DegreesToRadians(M);
	var Mdash = MeanAnomaly(JD);
	Mdash = CAACoordinateTransformation.DegreesToRadians(Mdash);
	var F = ArgumentOfLatitude(JD);
	F = CAACoordinateTransformation.DegreesToRadians(F);
  
	var E = CAAEarth.Eccentricity(JD);
	var T = (JD - 2451545) / 36525;
  
	var A1 = CAACoordinateTransformation.MapTo0To360Range(119.75 + 131.849 *T);
	A1 = CAACoordinateTransformation.DegreesToRadians(A1);
	var A2 = CAACoordinateTransformation.MapTo0To360Range(53.09 + 479264.290 *T);
	A2 = CAACoordinateTransformation.DegreesToRadians(A2);
	var A3 = CAACoordinateTransformation.MapTo0To360Range(313.45 + 481266.484 *T);
	A3 = CAACoordinateTransformation.DegreesToRadians(A3);
  
	var nBCoefficients =GlobalMembersStdafx.g_MoonCoefficients3.Length;
	Debug.Assert(GlobalMembersStdafx.g_MoonCoefficients4.Length == nBCoefficients);
	double SigmaB = 0;
	for (var i =0; i<nBCoefficients; i++)
	{
	  var ThisSigma = GlobalMembersStdafx.g_MoonCoefficients4[i] * Math.Sin(GlobalMembersStdafx.g_MoonCoefficients3[i].D *D + GlobalMembersStdafx.g_MoonCoefficients3[i].M *M + GlobalMembersStdafx.g_MoonCoefficients3[i].Mdash *Mdash + GlobalMembersStdafx.g_MoonCoefficients3[i].F *F);
  
	  if (GlobalMembersStdafx.g_MoonCoefficients3[i].M != 0)
		ThisSigma *= E;
  
	  SigmaB += ThisSigma;
	}
  
	//Finally the additive terms
	SigmaB -= 2235 *Math.Sin(Ldash);
	SigmaB += 382 *Math.Sin(A3);
	SigmaB += 175 *Math.Sin(A1 - F);
	SigmaB += 175 *Math.Sin(A1 + F);
	SigmaB += 127 *Math.Sin(Ldash - Mdash);
	SigmaB -= 115 *Math.Sin(Ldash + Mdash);
  
	return SigmaB/1000000;
  }
  public static double RadiusVector(double JD)
  {
	var Ldash = MeanLongitude(JD);
	Ldash = CAACoordinateTransformation.DegreesToRadians(Ldash);
	var D = MeanElongation(JD);
	D = CAACoordinateTransformation.DegreesToRadians(D);
	var M = CAAEarth.SunMeanAnomaly(JD);
	M = CAACoordinateTransformation.DegreesToRadians(M);
	var Mdash = MeanAnomaly(JD);
	Mdash = CAACoordinateTransformation.DegreesToRadians(Mdash);
	var F = ArgumentOfLatitude(JD);
	F = CAACoordinateTransformation.DegreesToRadians(F);
  
	var E = CAAEarth.Eccentricity(JD);
	var T = (JD - 2451545) / 36525;
  
	var A1 = CAACoordinateTransformation.MapTo0To360Range(119.75 + 131.849 *T);
	A1 = CAACoordinateTransformation.DegreesToRadians(A1);
	var A2 = CAACoordinateTransformation.MapTo0To360Range(53.09 + 479264.290 *T);
	A2 = CAACoordinateTransformation.DegreesToRadians(A2);
	var A3 = CAACoordinateTransformation.MapTo0To360Range(313.45 + 481266.484 *T);
	A3 = CAACoordinateTransformation.DegreesToRadians(A3);
  
	var nRCoefficients = GlobalMembersStdafx.g_MoonCoefficients1.Length;
	Debug.Assert(GlobalMembersStdafx.g_MoonCoefficients2.Length == nRCoefficients);
	double SigmaR = 0;
	for (var i =0; i<nRCoefficients; i++)
	{
	  var ThisSigma = GlobalMembersStdafx.g_MoonCoefficients2[i].B * Math.Cos(GlobalMembersStdafx.g_MoonCoefficients1[i].D *D + GlobalMembersStdafx.g_MoonCoefficients1[i].M *M + GlobalMembersStdafx.g_MoonCoefficients1[i].Mdash *Mdash + GlobalMembersStdafx.g_MoonCoefficients1[i].F *F);
	  if (GlobalMembersStdafx.g_MoonCoefficients1[i].M != 0)
		ThisSigma *= E;
  
	  SigmaR += ThisSigma;
	}
  
	return 385000.56 + SigmaR/1000;
  }

  public static double RadiusVectorToHorizontalParallax(double RadiusVector)
  {
	return CAACoordinateTransformation.RadiansToDegrees(Math.Asin(6378.14 / RadiusVector));
  }
  public static double HorizontalParallaxToRadiusVector(double Parallax)
  {
	return 6378.14 / Math.Sin(CAACoordinateTransformation.DegreesToRadians(Parallax));
  }
}



//////////////////////////////// Macros / Defines /////////////////////////////

public class MoonCoefficient1
{
    public MoonCoefficient1(double d, double m, double mdash, double f)
    {
        D = d;
        M = m;
        Mdash = mdash;
        F = f;
    }
    public double D;
    public double M;
    public double Mdash;
    public double F;
}

public class MoonCoefficient2
{
    public MoonCoefficient2(double a, double b)
    {
        A = a;
        B = b;
    }
    public double A;
    public double B;
}
