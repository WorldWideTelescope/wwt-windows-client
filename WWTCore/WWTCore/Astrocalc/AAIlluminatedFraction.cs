using System;
//
//Module : AAILLUMINATEDFRACTION.CPP
//Purpose: Implementation for the algorithms for a planet's Phase Angle, Illuminated Fraction and Magnitude
//Created: PJN / 29-12-2003
//History: PJN / 21-01-2005 1. Fixed a small but important error in the function PhaseAngle(r, R, Delta). The code 
//                          was producing incorrect results and raises acos DOMAIN errors and floating point exceptions 
//                          when calculating phase angles for the inner planets. Thanks to MICHAEL R. MEYER for 
//                          reporting this problem.
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

//////////////////// Includes /////////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAIlluminatedFraction
{
//Static methods

  //////////////////// Implementation ///////////////////////////////////////////
  
  public static double PhaseAngle(double r, double R, double Delta)
  {
	//Return the result
	return CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(Math.Acos((r *r + Delta *Delta - R *R) / (2 *r *Delta))));
  }
  public static double PhaseAngle(double R, double R0, double B, double L, double L0, double Delta)
  {
	//Convert from degrees to radians
	B = CAACoordinateTransformation.DegreesToRadians(B);
	L = CAACoordinateTransformation.DegreesToRadians(L);
	L0 = CAACoordinateTransformation.DegreesToRadians(L0);
  
	//Return the result
	return CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(Math.Acos((R - R0 *Math.Cos(B)*Math.Cos(L - L0))/Delta)));
  }
  public static double PhaseAngleRectangular(double x, double y, double z, double B, double L, double Delta)
  {
	//Convert from degrees to radians
	B = CAACoordinateTransformation.DegreesToRadians(B);
	L = CAACoordinateTransformation.DegreesToRadians(L);
	var cosB = Math.Cos(B);
  
	//Return the result
	return CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(Math.Acos((x *cosB *Math.Cos(L) + y *cosB *Math.Sin(L) + z *Math.Sin(B)) / Delta)));
  }
  public static double IlluminatedFraction(double PhaseAngle)
  {
	//Convert from degrees to radians
	PhaseAngle = CAACoordinateTransformation.DegreesToRadians(PhaseAngle);
  
	//Return the result
	return (1 + Math.Cos(PhaseAngle)) / 2;
  }
  public static double IlluminatedFraction(double r, double R, double Delta)
  {
	return (((r+Delta)*(r+Delta) - R *R) / (4 *r *Delta));
  }
  public static double MercuryMagnitudeMuller(double r, double Delta, double i)
  {
	var I_50 = i - 50;
	return 1.16 + 5 *Math.Log10(r *Delta) + 0.02838 *I_50 + 0.0001023 *I_50 *I_50;
  }
  public static double VenusMagnitudeMuller(double r, double Delta, double i)
  {
	return -4.00 + 5 *Math.Log10(r *Delta) + 0.01322 *i + 0.0000004247 *i *i *i;
  }
  public static double MarsMagnitudeMuller(double r, double Delta, double i)
  {
	return -1.3 + 5 *Math.Log10(r *Delta) + 0.01486 *i;
  }
  public static double JupiterMagnitudeMuller(double r, double Delta)
  {
	return -8.93 + 5 *Math.Log10(r *Delta);
  }
  public static double SaturnMagnitudeMuller(double r, double Delta, double DeltaU, double B)
  {
	//Convert from degrees to radians
	B = CAACoordinateTransformation.DegreesToRadians(B);
	var sinB = Math.Sin(B);
  
	return -8.68 + 5 *Math.Log10(r *Delta) + 0.044 *Math.Abs(DeltaU) - 2.60 *Math.Sin(Math.Abs(B)) + 1.25 *sinB *sinB;
  }
  public static double UranusMagnitudeMuller(double r, double Delta)
  {
	return -6.85 + 5 *Math.Log10(r *Delta);
  }
  public static double NeptuneMagnitudeMuller(double r, double Delta)
  {
	return -7.05 + 5 *Math.Log10(r *Delta);
  }
  public static double MercuryMagnitudeAA(double r, double Delta, double i)
  {
	var i2 = i *i;
	var i3 = i2 *i;
  
	return -0.42 + 5 *Math.Log10(r *Delta) + 0.0380 *i - 0.000273 *i2 + 0.000002 *i3;
  }
  public static double VenusMagnitudeAA(double r, double Delta, double i)
  {
	var i2 = i *i;
	var i3 = i2 *i;
  
	return -4.40 + 5 *Math.Log10(r *Delta) + 0.0009 *i + 0.000239 *i2 - 0.00000065 *i3;
  }
  public static double MarsMagnitudeAA(double r, double Delta, double i)
  {
	return -1.52 + 5 *Math.Log10(r *Delta) + 0.016 *i;
  }
  public static double JupiterMagnitudeAA(double r, double Delta, double i)
  {
	return -9.40 + 5 *Math.Log10(r *Delta) + 0.005 *i;
  }
  public static double SaturnMagnitudeAA(double r, double Delta, double DeltaU, double B)
  {
	//Convert from degrees to radians
	B = CAACoordinateTransformation.DegreesToRadians(B);
	var sinB = Math.Sin(B);
  
	return -8.88 + 5 *Math.Log10(r *Delta) + 0.044 *Math.Abs(DeltaU) - 2.60 *Math.Sin(Math.Abs(B)) + 1.25 *sinB *sinB;
  }
  public static double UranusMagnitudeAA(double r, double Delta)
  {
	return -7.19 + 5 *Math.Log10(r *Delta);
  }
  public static double NeptuneMagnitudeAA(double r, double Delta)
  {
	return -6.87 + 5 *Math.Log10(r *Delta);
  }
  public static double PlutoMagnitudeAA(double r, double Delta)
  {
	return -1.00 + 5 *Math.Log10(r *Delta);
  }
}
