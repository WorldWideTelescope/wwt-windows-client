using System.Diagnostics;
using System;
//
//Module : AAECLIPSES.CPP
//Purpose: Implementation for the algorithms which obtain the principal characteristics of an eclipse of the Sun or the Moon
//Created: PJN / 21-01-2004
//History: PJN / 25-02-2004 1. Calculation of semi durations is now calculated only when required
//         PJN / 31-01-2005 1. Fixed a GCC compiler error related to missing include for memset. Thanks to Mika Heiskanen for 
//                          reporting this problem.
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

public class  CAASolarEclipseDetails
{
//Constructors / Destructors
  public CAASolarEclipseDetails()
  {
	  bEclipse = false;
	  TimeOfMaximumEclipse = 0;
	  F = 0;
	  u = 0;
	  gamma = 0;
	  GreatestMagnitude = 0;
  }

//Member variables
  public bool bEclipse;
  public double TimeOfMaximumEclipse;
  public double F;
  public double u;
  public double gamma;
  public double GreatestMagnitude;
}


public class  CAALunarEclipseDetails
{
//Constructors / Destructors
  public CAALunarEclipseDetails()
  {
	  bEclipse = false;
	  TimeOfMaximumEclipse = 0;
	  F = 0;
	  u = 0;
	  gamma = 0;
	  PenumbralRadii = 0;
	  UmbralRadii = 0;
	  PenumbralMagnitude = 0;
	  UmbralMagnitude = 0;
	  PartialPhaseSemiDuration = 0;
	  TotalPhaseSemiDuration = 0;
	  PartialPhasePenumbraSemiDuration = 0;
  }

//Member variables
  public bool bEclipse;
  public double TimeOfMaximumEclipse;
  public double F;
  public double u;
  public double gamma;
  public double PenumbralRadii;
  public double UmbralRadii;
  public double PenumbralMagnitude;
  public double UmbralMagnitude;
  public double PartialPhaseSemiDuration;
  public double TotalPhaseSemiDuration;
  public double PartialPhasePenumbraSemiDuration;
}

public class  CAAEclipses
{
//Static methods
  public static CAASolarEclipseDetails CalculateSolar(double k)
  {
#if _DEBUG
	double intp = 0;
	bool bSolarEclipse = (GlobalMembersStdafx.modf(k, ref intp) == 0);
	Debug.Assert(bSolarEclipse);
#endif

      double Mdash = 0;
	return Calculate(k, ref Mdash);
  }
  public static CAALunarEclipseDetails CalculateLunar(double k)
  {
#if _DEBUG
	double intp = 0;
	bool bSolarEclipse = (GlobalMembersStdafx.modf(k, ref intp) == 0);
	Debug.Assert(!bSolarEclipse);
#endif

      double Mdash = 0;
	CAASolarEclipseDetails solarDetails = Calculate(k, ref Mdash);
  
	//What will be the return value
	CAALunarEclipseDetails details = new CAALunarEclipseDetails();
	details.bEclipse = solarDetails.bEclipse;
	details.F = solarDetails.F;
	details.gamma = solarDetails.gamma;
	details.TimeOfMaximumEclipse = solarDetails.TimeOfMaximumEclipse;
	details.u = solarDetails.u;
  
	if (details.bEclipse)
	{
	  details.PenumbralRadii = 1.2848 + details.u;
	  details.UmbralRadii = 0.7403 - details.u;
	  double fgamma = Math.Abs(details.gamma);
	  details.PenumbralMagnitude = (1.5573 + details.u - fgamma) / 0.5450;
	  details.UmbralMagnitude = (1.0128 - details.u - fgamma) / 0.5450;
  
	  double p = 1.0128 - details.u;
	  double t = 0.4678 - details.u;
	  double n = 0.5458 + 0.0400 *Math.Cos(Mdash);
  
	  double gamma2 = details.gamma *details.gamma;
	  double p2 = p *p;
	  if (p2 >= gamma2)
		details.PartialPhaseSemiDuration = 60/n *Math.Sqrt(p2 - gamma2);
  
	  double t2 = t *t;
	  if (t2 >= gamma2)
		details.TotalPhaseSemiDuration = 60/n *Math.Sqrt(t2 - gamma2);
  
	  double h = 1.5573 + details.u;
	  double h2 = h *h;
	  if (h2 >= gamma2)
		details.PartialPhasePenumbraSemiDuration = 60/n *Math.Sqrt(h2 - gamma2);
	}
  
	return details;
  }

  //Tangible Process Only End
  
  
  //////////////////////////// Implementation ///////////////////////////////////
  
  protected static CAASolarEclipseDetails Calculate(double k, ref double Mdash)
  {
	//Are we looking for a solar or lunar eclipse
	double intp = 0;
    bool bSolarEclipse = (GlobalMembersStdafx.modf(k, ref intp) == 0);
  
	//What will be the return value
	CAASolarEclipseDetails details = new CAASolarEclipseDetails();
  
	//convert from K to T
	double T = k/1236.85;
	double T2 = T *T;
	double T3 = T2 *T;
	double T4 = T3 *T;
  
	double E = 1 - 0.002516 *T - 0.0000074 *T2;
  
	double M = CAACoordinateTransformation.MapTo0To360Range(2.5534 + 29.10535670 *k - 0.0000014 *T2 - 0.00000011 *T3);
	M = CAACoordinateTransformation.DegreesToRadians(M);
  
	Mdash = CAACoordinateTransformation.MapTo0To360Range(201.5643 + 385.81693528 *k + 0.0107582 *T2 + 0.00001238 *T3 - 0.000000058 *T4);
	Mdash = CAACoordinateTransformation.DegreesToRadians(Mdash);
  
	double omega = CAACoordinateTransformation.MapTo0To360Range(124.7746 - 1.56375588 *k + 0.0020672 *T2 + 0.00000215 *T3);
	omega = CAACoordinateTransformation.DegreesToRadians(omega);
  
	double F = CAACoordinateTransformation.MapTo0To360Range(160.7108 + 390.67050284 *k - 0.0016118 *T2 - 0.00000227 *T3 + 0.00000001 *T4);
	details.F = F;
	double Fdash = F - 0.02665 *Math.Sin(omega);
  
	F = CAACoordinateTransformation.DegreesToRadians(F);
	Fdash = CAACoordinateTransformation.DegreesToRadians(Fdash);
  
	//Do the first check to see if we have an eclipse
	if (Math.Abs(Math.Sin(F)) > 0.36)
	  return details;
  
	double A1 = CAACoordinateTransformation.MapTo0To360Range(299.77 + 0.107408 *k - 0.009173 *T2);
	A1 = CAACoordinateTransformation.DegreesToRadians(A1);
  
	details.TimeOfMaximumEclipse = CAAMoonPhases.MeanPhase(k);
  
	double DeltaJD = 0;
	if (bSolarEclipse)
	  DeltaJD += -0.4075 *Math.Sin(Mdash) + 0.1721 *E *Math.Sin(M);
	else
	  DeltaJD += -0.4065 *Math.Sin(Mdash) + 0.1727 *E *Math.Sin(M);
	DeltaJD += 0.0161 *Math.Sin(2 *Mdash) + -0.0097 *Math.Sin(2 *Fdash) + 0.0073 *E *Math.Sin(Mdash - M) + -0.0050 *E *Math.Sin(Mdash + M) + -0.0023 *Math.Sin(Mdash - 2 *Fdash) + 0.0021 *E *Math.Sin(2 *M) + 0.0012 *Math.Sin(Mdash + 2 *Fdash) + 0.0006 *E *Math.Sin(2 *Mdash + M) + -0.0004 *Math.Sin(3 *Mdash) + -0.0003 *E *Math.Sin(M + 2 *Fdash) + 0.0003 *Math.Sin(A1) + -0.0002 *E *Math.Sin(M - 2 *Fdash) + -0.0002 *E *Math.Sin(2 *Mdash - M) + -0.0002 *Math.Sin(omega);
  
	details.TimeOfMaximumEclipse += DeltaJD;
  
	double P = 0.2070 *E *Math.Sin(M) + 0.0024 *E *Math.Sin(2 *M) + -0.0392 *Math.Sin(Mdash) + 0.0116 *Math.Sin(2 *Mdash) + -0.0073 *E *Math.Sin(Mdash + M) + 0.0067 *E *Math.Sin(Mdash - M) + 0.0118 *Math.Sin(2 *Fdash);
  
	double Q = 5.2207 + -0.0048 *E *Math.Cos(M) + 0.0020 *E *Math.Cos(2 *M) + -0.3299 *Math.Cos(Mdash) + -0.0060 *E *Math.Cos(Mdash + M) + 0.0041 *E *Math.Cos(Mdash - M);
  
	double W = Math.Abs(Math.Cos(Fdash));
  
	details.gamma = (P *Math.Cos(Fdash) + Q *Math.Sin(Fdash))*(1 - 0.0048 *W);
  
	details.u = 0.0059 + 0.0046 *E *Math.Cos(M) + -0.0182 *Math.Cos(Mdash) + 0.0004 *Math.Cos(2 *Mdash) + -0.0005 *Math.Cos(M + Mdash);
  
	//Check to see if the eclipse is visible from the Earth's surface
	if (Math.Abs(details.gamma) > (1.5433 + details.u))
	  return details;
  
	//We have an eclipse at this time
	details.bEclipse = true;
  
	//In the case of a partial eclipse, calculate its magnitude
	double fgamma = Math.Abs(details.gamma);
	if (((fgamma > 0.9972) && (fgamma < 1.5433 + details.u)))
	  details.GreatestMagnitude = (1.5433 + details.u - fgamma) / (0.5461 + 2 *details.u);
  
	return details;
  }
}
