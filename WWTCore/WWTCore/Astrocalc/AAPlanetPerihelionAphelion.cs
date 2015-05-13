using System;
//
//Module : AAPLANETPERIHELIONAPHELION.CPP
//Purpose: Implementation for the algorithms which obtain the dates of Perihelion and Aphelion of the planets
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

///////////////////////////////// Includes ////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAPlanetPerihelionAphelion
{
//Static methods

  ///////////////////////////////// Implementation //////////////////////////////
  
  public static int MercuryK(double Year)
  {
	return (int)(4.15201*(Year - 2000.12));
  }
  public static double MercuryPerihelion(int k)
  {
	return 2451590.257 + 87.96934963 *k;
  }
  public static double MercuryAphelion(int k)
  {
	double kdash = k + 0.5;
	return 2451590.257 + 87.96934963 *kdash;
  }

  public static int VenusK(double Year)
  {
	return (int)(1.62549*(Year - 2000.53));
  }
  public static double VenusPerihelion(int k)
  {
	double kdash = k;
	double ksquared = kdash * kdash;
	return 2451738.233 + 224.7008188 *kdash - 0.0000000327 *ksquared;
  }
  public static double VenusAphelion(int k)
  {
	double kdash = k + 0.5;
	double ksquared = kdash * kdash;
	return 2451738.233 + 224.7008188 *kdash - 0.0000000327 *ksquared;
  }

  public static int EarthK(double Year)
  {
	return (int)(0.99997*(Year - 2000.01));
  }
  public static double EarthPerihelion(int k)
  {
	  return EarthPerihelion(k, false);
  }
//C++ TO C# CONVERTER NOTE: C# does not allow default values for parameters. Overloaded methods are inserted above.
//ORIGINAL LINE: static double EarthPerihelion(int k, bool bBarycentric = false)
  public static double EarthPerihelion(int k, bool bBarycentric)
  {
	double kdash = k;
	double ksquared = kdash * kdash;
	double JD = 2451547.507 + 365.2596358 *kdash + 0.0000000156 *ksquared;
  
	if (!bBarycentric)
	{
	  //Apply the corrections
	  double A1 = CAACoordinateTransformation.MapTo0To360Range(328.41 + 132.788585 *k);
	  A1 = CAACoordinateTransformation.DegreesToRadians(A1);
	  double A2 = CAACoordinateTransformation.MapTo0To360Range(316.13 + 584.903153 *k);
	  A2 = CAACoordinateTransformation.DegreesToRadians(A2);
	  double A3 = CAACoordinateTransformation.MapTo0To360Range(346.20 + 450.380738 *k);
	  A3 = CAACoordinateTransformation.DegreesToRadians(A3);
	  double A4 = CAACoordinateTransformation.MapTo0To360Range(136.95 + 659.306737 *k);
	  A4 = CAACoordinateTransformation.DegreesToRadians(A4);
	  double A5 = CAACoordinateTransformation.MapTo0To360Range(249.52 + 329.653368 *k);
	  A5 = CAACoordinateTransformation.DegreesToRadians(A5);
  
	  JD += 1.278 *Math.Sin(A1);
	  JD -= 0.055 *Math.Sin(A2);
	  JD -= 0.091 *Math.Sin(A3);
	  JD -= 0.056 *Math.Sin(A4);
	  JD -= 0.045 *Math.Sin(A5);
	}
  
	return JD;
  }
  public static double EarthAphelion(int k)
  {
	  return EarthAphelion(k, false);
  }
//C++ TO C# CONVERTER NOTE: C# does not allow default values for parameters. Overloaded methods are inserted above.
//ORIGINAL LINE: static double EarthAphelion(int k, bool bBarycentric = false)
  public static double EarthAphelion(int k, bool bBarycentric)
  {
	double kdash = k + 0.5;
	double ksquared = kdash * kdash;
	double JD = 2451547.507 + 365.2596358 *kdash + 0.0000000156 *ksquared;
  
	if (!bBarycentric)
	{
	  //Apply the corrections
	  double A1 = CAACoordinateTransformation.MapTo0To360Range(328.41 + 132.788585 *k);
	  A1 = CAACoordinateTransformation.DegreesToRadians(A1);
	  double A2 = CAACoordinateTransformation.MapTo0To360Range(316.13 + 584.903153 *k);
	  A2 = CAACoordinateTransformation.DegreesToRadians(A2);
	  double A3 = CAACoordinateTransformation.MapTo0To360Range(346.20 + 450.380738 *k);
	  A3 = CAACoordinateTransformation.DegreesToRadians(A3);
	  double A4 = CAACoordinateTransformation.MapTo0To360Range(136.95 + 659.306737 *k);
	  A4 = CAACoordinateTransformation.DegreesToRadians(A4);
	  double A5 = CAACoordinateTransformation.MapTo0To360Range(249.52 + 329.653368 *k);
	  A5 = CAACoordinateTransformation.DegreesToRadians(A5);
  
	  JD -= 1.352 *Math.Sin(A1);
	  JD += 0.061 *Math.Sin(A2);
	  JD += 0.062 *Math.Sin(A3);
	  JD += 0.029 *Math.Sin(A4);
	  JD += 0.031 *Math.Sin(A5);
	}
  
	return JD;
  }

  public static int MarsK(double Year)
  {
	return (int)(0.53166*(Year - 2001.78));
  }
  public static double MarsPerihelion(int k)
  {
	double kdash = k;
	double ksquared = kdash * kdash;
	return 2452195.026 + 686.9957857 *kdash - 0.0000001187 *ksquared;
  }
  public static double MarsAphelion(int k)
  {
	double kdash = k + 0.5;
	double ksquared = kdash * kdash;
	return 2452195.026 + 686.9957857 *kdash - 0.0000001187 *ksquared;
  }

  public static int JupiterK(double Year)
  {
	return (int)(0.08430*(Year - 2011.20));
  }
  public static double JupiterPerihelion(int k)
  {
	double kdash = k;
	double ksquared = kdash * kdash;
	return 2455636.936 + 4332.897065 *kdash + 0.0001367 *ksquared;
  }
  public static double JupiterAphelion(int k)
  {
	double kdash = k + 0.5;
	double ksquared = kdash * kdash;
	return 2455636.936 + 4332.897065 *kdash + 0.0001367 *ksquared;
  }

  public static int SaturnK(double Year)
  {
	return (int)(0.03393*(Year - 2003.52));
  }
  public static double SaturnPerihelion(int k)
  {
	double kdash = k;
	double ksquared = kdash * kdash;
	return 2452830.12 + 10764.21676 *kdash + 0.000827 *ksquared;
  }
  public static double SaturnAphelion(int k)
  {
	double kdash = k + 0.5;
	double ksquared = kdash * kdash;
	return 2452830.12 + 10764.21676 *kdash + 0.000827 *ksquared;
  }

  public static int UranusK(double Year)
  {
	return (int)(0.01190*(Year - 2051.1));
  }
  public static double UranusPerihelion(int k)
  {
	double kdash = k;
	double ksquared = kdash * kdash;
	return 2470213.5 + 30694.8767 *kdash - 0.00541 *ksquared;
  }
  public static double UranusAphelion(int k)
  {
	double kdash = k + 0.5;
	double ksquared = kdash * kdash;
	return 2470213.5 + 30694.8767 *kdash - 0.00541 *ksquared;
  }

  public static int NeptuneK(double Year)
  {
	return (int)(0.00607*(Year - 2047.5));
  }
  public static double NeptunePerihelion(int k)
  {
	double kdash = k;
	double ksquared = kdash * kdash;
	return 2468895.1 + 60190.33 *kdash + 0.03429 *ksquared;
  }
  public static double NeptuneAphelion(int k)
  {
	double kdash = k + 0.5;
	double ksquared = kdash * kdash;
	return 2468895.1 + 60190.33 *kdash + 0.03429 *ksquared;
  }
}
