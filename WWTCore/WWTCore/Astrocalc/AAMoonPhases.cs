using System;
using System.Diagnostics;
//
//Module : AAMOONPHASES.CPP
//Purpose: Implementation for the algorithms which obtain the dates for the phases of the Moon
//Created: PJN / 11-01-2004
//History: PJN / 22-02-2004 1. Fixed a bug in the calculation of the phase type from the k value in
//                          CAAMoonPhases::TruePhase.
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

//////////////////////////// Includes /////////////////////////////////////////




/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAMoonPhases
{
//Static methods

  //////////////////////////// Implementation ///////////////////////////////////
  
  public static double K(double Year)
  {
	return 12.3685*(Year - 2000);
  }
  public static double MeanPhase(double k)
  {
	//convert from K to T
	double T = k/1236.85;
	double T2 = T *T;
	double T3 = T2 *T;
	double T4 = T3 *T;
  
	return 2451550.09766 + 29.530588861 *k + 0.00015437 *T2 - 0.000000150 *T3 + 0.00000000073 *T4;
  }
  public static double TruePhase(double k)
  {
	//What will be the return value
	double JD = MeanPhase(k);
  
	//convert from K to T
	double T = k/1236.85;
	double T2 = T *T;
	double T3 = T2 *T;
	double T4 = T3 *T;
  
	double E = 1 - 0.002516 *T - 0.0000074 *T2;
	double E2 = E *E;
  
	double M = CAACoordinateTransformation.MapTo0To360Range(2.5534 + 29.10535670 *k - 0.0000014 *T2 - 0.00000011 *T3);
	M = CAACoordinateTransformation.DegreesToRadians(M);
	double Mdash = CAACoordinateTransformation.MapTo0To360Range(201.5643 + 385.81693528 *k + 0.0107582 *T2 + 0.00001238 *T3 - 0.000000058 *T4);
	Mdash = CAACoordinateTransformation.DegreesToRadians(Mdash);
	double F = CAACoordinateTransformation.MapTo0To360Range(160.7108 + 390.67050284 *k - 0.0016118 *T2 - 0.00000227 *T3 + 0.00000001 *T4);
	F = CAACoordinateTransformation.DegreesToRadians(F);
	double omega = CAACoordinateTransformation.MapTo0To360Range(124.7746 - 1.56375588 *k + 0.0020672 *T2 + 0.00000215 *T3);
	omega = CAACoordinateTransformation.DegreesToRadians(omega);
	double A1 = CAACoordinateTransformation.MapTo0To360Range(299.77 + 0.107408 *k - 0.009173 *T2);
	A1 = CAACoordinateTransformation.DegreesToRadians(A1);
	double A2 = CAACoordinateTransformation.MapTo0To360Range(251.88 + 0.016321 *k);
	A2 = CAACoordinateTransformation.DegreesToRadians(A2);
	double A3 = CAACoordinateTransformation.MapTo0To360Range(251.83 + 26.651886 *k);
	A3 = CAACoordinateTransformation.DegreesToRadians(A3);
	double A4 = CAACoordinateTransformation.MapTo0To360Range(349.42 + 36.412478 *k);
	A4 = CAACoordinateTransformation.DegreesToRadians(A4);
	double A5 = CAACoordinateTransformation.MapTo0To360Range(84.66 + 18.206239 *k);
	A5 = CAACoordinateTransformation.DegreesToRadians(A5);
	double A6 = CAACoordinateTransformation.MapTo0To360Range(141.74 + 53.303771 *k);
	A6 = CAACoordinateTransformation.DegreesToRadians(A6);
	double A7 = CAACoordinateTransformation.MapTo0To360Range(207.14 + 2.453732 *k);
	A7 = CAACoordinateTransformation.DegreesToRadians(A7);
	double A8 = CAACoordinateTransformation.MapTo0To360Range(154.84 + 7.306860 *k);
	A8 = CAACoordinateTransformation.DegreesToRadians(A8);
	double A9 = CAACoordinateTransformation.MapTo0To360Range(34.52 + 27.261239 *k);
	A9 = CAACoordinateTransformation.DegreesToRadians(A9);
	double A10 = CAACoordinateTransformation.MapTo0To360Range(207.19 + 0.121824 *k);
	A10 = CAACoordinateTransformation.DegreesToRadians(A10);
	double A11 = CAACoordinateTransformation.MapTo0To360Range(291.34 + 1.844379 *k);
	A11 = CAACoordinateTransformation.DegreesToRadians(A11);
	double A12 = CAACoordinateTransformation.MapTo0To360Range(161.72 + 24.198154 *k);
	A12 = CAACoordinateTransformation.DegreesToRadians(A12);
	double A13 = CAACoordinateTransformation.MapTo0To360Range(239.56 + 25.513099 *k);
	A13 = CAACoordinateTransformation.DegreesToRadians(A13);
	double A14 = CAACoordinateTransformation.MapTo0To360Range(331.55 + 3.592518 *k);
	A14 = CAACoordinateTransformation.DegreesToRadians(A14);
  
	//convert to radians
	double kint=0;
    double kfrac = GlobalMembersStdafx.modf(k, ref kint);
	if (kfrac < 0)
	  kfrac = 1 + kfrac;
	if (kfrac == 0) //New Moon
	{
	  double DeltaJD = -0.40720 *Math.Sin(Mdash) + 0.17241 *E *Math.Sin(M) + 0.01608 *Math.Sin(2 *Mdash) + 0.01039 *Math.Sin(2 *F) + 0.00739 *E *Math.Sin(Mdash - M) + -0.00514 *E *Math.Sin(Mdash + M) + 0.00208 *E2 *Math.Sin(2 *M) + -0.00111 *Math.Sin(Mdash - 2 *F) + -0.00057 *Math.Sin(Mdash + 2 *F) + 0.00056 *E *Math.Sin(2 *Mdash + M) + -0.00042 *Math.Sin(3 *Mdash) + 0.00042 *E *Math.Sin(M + 2 *F) + 0.00038 *E *Math.Sin(M - 2 *F) + -0.00024 *E *Math.Sin(2 *Mdash - M) + -0.00017 *Math.Sin(omega) + -0.00007 *Math.Sin(Mdash + 2 *M) + 0.00004 *Math.Sin(2 *Mdash - 2 *F) + 0.00004 *Math.Sin(3 *M) + 0.00003 *Math.Sin(Mdash + M - 2 *F) + 0.00003 *Math.Sin(2 *Mdash + 2 *F) + -0.00003 *Math.Sin(Mdash + M + 2 *F) + 0.00003 *Math.Sin(Mdash - M + 2 *F) + -0.00002 *Math.Sin(Mdash - M - 2 *F) + -0.00002 *Math.Sin(3 *Mdash + M) + 0.00002 *Math.Sin(4 *Mdash);
	  JD += DeltaJD;
	}
	else if ((kfrac == 0.25) || (kfrac == 0.75)) //First Quarter or Last Quarter
	{
	  double DeltaJD = -0.62801 *Math.Sin(Mdash) + 0.17172 *E *Math.Sin(M) + -0.01183 *E *Math.Sin(Mdash + M) + 0.00862 *Math.Sin(2 *Mdash) + 0.00804 *Math.Sin(2 *F) + 0.00454 *E *Math.Sin(Mdash - M) + 0.00204 *E2 *Math.Sin(2 *M) + -0.00180 *Math.Sin(Mdash - 2 *F) + -0.00070 *Math.Sin(Mdash + 2 *F) + -0.00040 *Math.Sin(3 *Mdash) + -0.00034 *E *Math.Sin(2 *Mdash - M) + 0.00032 *E *Math.Sin(M + 2 *F) + 0.00032 *E *Math.Sin(M - 2 *F) + -0.00028 *E2 *Math.Sin(Mdash + 2 *M) + 0.00027 *E *Math.Sin(2 *Mdash + M) + -0.00017 *Math.Sin(omega) + -0.00005 *Math.Sin(Mdash - M - 2 *F) + 0.00004 *Math.Sin(2 *Mdash + 2 *F) + -0.00004 *Math.Sin(Mdash + M + 2 *F) + 0.00004 *Math.Sin(Mdash - 2 *M) + 0.00003 *Math.Sin(Mdash + M - 2 *F) + 0.00003 *Math.Sin(3 *M) + 0.00002 *Math.Sin(2 *Mdash - 2 *F) + 0.00002 *Math.Sin(Mdash - M + 2 *F) + -0.00002 *Math.Sin(3 *Mdash + M);
	  JD += DeltaJD;
  
	  double W = 0.00306 - 0.00038 *E *Math.Cos(M) + 0.00026 *Math.Cos(Mdash) - 0.00002 *Math.Cos(Mdash - M) + 0.00002 *Math.Cos(Mdash + M) + 0.00002 *Math.Cos(2 *F);
	  if (kfrac == 0.25) //First quarter
		JD += W;
	  else
		JD -= W;
	}
	else if (kfrac == 0.5) //Full Moon
	{
	  double DeltaJD = -0.40614 *Math.Sin(Mdash) + 0.17302 *E *Math.Sin(M) + 0.01614 *Math.Sin(2 *Mdash) + 0.01043 *Math.Sin(2 *F) + 0.00734 *E *Math.Sin(Mdash - M) + -0.00514 *E *Math.Sin(Mdash + M) + 0.00209 *E2 *Math.Sin(2 *M) + -0.00111 *Math.Sin(Mdash - 2 *F) + -0.00057 *Math.Sin(Mdash + 2 *F) + 0.00056 *E *Math.Sin(2 *Mdash + M) + -0.00042 *Math.Sin(3 *Mdash) + 0.00042 *E *Math.Sin(M + 2 *F) + 0.00038 *E *Math.Sin(M - 2 *F) + -0.00024 *E *Math.Sin(2 *Mdash - M) + -0.00017 *Math.Sin(omega) + -0.00007 *Math.Sin(Mdash + 2 *M) + 0.00004 *Math.Sin(2 *Mdash - 2 *F) + 0.00004 *Math.Sin(3 *M) + 0.00003 *Math.Sin(Mdash + M - 2 *F) + 0.00003 *Math.Sin(2 *Mdash + 2 *F) + -0.00003 *Math.Sin(Mdash + M + 2 *F) + 0.00003 *Math.Sin(Mdash - M + 2 *F) + -0.00002 *Math.Sin(Mdash - M - 2 *F) + -0.00002 *Math.Sin(3 *Mdash + M) + 0.00002 *Math.Sin(4 *Mdash);
	  JD += DeltaJD;
	}
	else
	{
	  Debug.Assert(false);
	}
  
	//Additional corrections for all phases
	double DeltaJD2 = 0.000325 *Math.Sin(A1) + 0.000165 *Math.Sin(A2) + 0.000164 *Math.Sin(A3) + 0.000126 *Math.Sin(A4) + 0.000110 *Math.Sin(A5) + 0.000062 *Math.Sin(A6) + 0.000060 *Math.Sin(A7) + 0.000056 *Math.Sin(A8) + 0.000047 *Math.Sin(A9) + 0.000042 *Math.Sin(A10) + 0.000040 *Math.Sin(A11) + 0.000037 *Math.Sin(A12) + 0.000035 *Math.Sin(A13) + 0.000023 *Math.Sin(A14);
	JD += DeltaJD2;
  
	return JD;
  }
}
