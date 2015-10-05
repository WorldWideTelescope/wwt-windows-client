using System;
//
//Module : AADIAMETERS.CPP
//Purpose: Implementation for the algorithms for the semi diameters of the Sun, Moon, Planets and Asteroids
//Created: PJN / 15-01-2004
//History: None
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

//////////////////// Includes /////////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAADiameters
{
//Static methods
  //Tangible Process Only End
  
  
  //////////////////// Implementation ///////////////////////////////////////////
  
  public static double SunSemidiameterA(double Delta)
  {
	return 959.63/Delta;
  }
  public static double MercurySemidiameterA(double Delta)
  {
	return 3.34/Delta;
  }
  public static double VenusSemidiameterA(double Delta)
  {
	return 8.41/Delta;
  }
  public static double MarsSemidiameterA(double Delta)
  {
	return 4.68/Delta;
  }
  public static double JupiterEquatorialSemidiameterA(double Delta)
  {
	return 98.47/Delta;
  }
  public static double JupiterPolarSemidiameterA(double Delta)
  {
	return 91.91/Delta;
  }
  public static double SaturnEquatorialSemidiameterA(double Delta)
  {
	return 83.33/Delta;
  }
  public static double SaturnPolarSemidiameterA(double Delta)
  {
	return 74.57/Delta;
  }
  public static double ApparentSaturnPolarSemidiameterA(double Delta, double B)
  {
	var cosB = Math.Cos(CAACoordinateTransformation.DegreesToRadians(B));
	return SaturnPolarSemidiameterA(Delta)*Math.Sqrt(1 - 0.199197 *cosB *cosB);
  }
  public static double UranusSemidiameterA(double Delta)
  {
	return 34.28/Delta;
  }
  public static double NeptuneSemidiameterA(double Delta)
  {
	return 36.56/Delta;
  }
  public static double MercurySemidiameterB(double Delta)
  {
	return 3.36/Delta;
  }
  public static double VenusSemidiameterB(double Delta)
  {
	return 8.34/Delta;
  }
  public static double MarsSemidiameterB(double Delta)
  {
	return 4.68/Delta;
  }
  public static double JupiterEquatorialSemidiameterB(double Delta)
  {
	return 98.44/Delta;
  }
  public static double JupiterPolarSemidiameterB(double Delta)
  {
	return 92.06/Delta;
  }
  public static double SaturnEquatorialSemidiameterB(double Delta)
  {
	return 82.73/Delta;
  }
  public static double SaturnPolarSemidiameterB(double Delta)
  {
	return 73.82/Delta;
  }
  public static double ApparentSaturnPolarSemidiameterB(double Delta, double B)
  {
	var cosB = Math.Cos(CAACoordinateTransformation.DegreesToRadians(B));
	return SaturnPolarSemidiameterB(Delta)*Math.Sqrt(1 - 0.203800 *cosB *cosB);
  }
  public static double UranusSemidiameterB(double Delta)
  {
	return 35.02/Delta;
  }
  public static double NeptuneSemidiameterB(double Delta)
  {
	return 33.50/Delta;
  }
  public static double PlutoSemidiameterB(double Delta)
  {
	return 2.07/Delta;
  }
  public static double GeocentricMoonSemidiameter(double Delta)
  {
	return CAACoordinateTransformation.RadiansToDegrees(0.272481 *6378.14/Delta)*3600;
  }
  public static double TopocentricMoonSemidiameter(double DistanceDelta, double Delta, double H, double Latitude, double Height)
  {
	//Convert to radians
	H = CAACoordinateTransformation.HoursToRadians(H);
	Delta = CAACoordinateTransformation.DegreesToRadians(Delta);
  
	var pi = Math.Asin(6378.14/DistanceDelta);
	var A = Math.Cos(Delta)*Math.Sin(H);
	var B = Math.Cos(Delta)*Math.Cos(H) - CAAGlobe.RhoCosThetaPrime(Latitude, Height)*Math.Sin(pi);
	var C = Math.Sin(Delta) - CAAGlobe.RhoSinThetaPrime(Latitude, Height)*Math.Sin(pi);
	var q = Math.Sqrt(A *A + B *B + C *C);
  
	var s = CAACoordinateTransformation.DegreesToRadians(GeocentricMoonSemidiameter(DistanceDelta)/3600);
	return CAACoordinateTransformation.RadiansToDegrees(Math.Asin(Math.Sin(s)/q))*3600;
  }
  public static double AsteroidDiameter(double H, double A)
  {
	var x = 3.12 - H/5 - 0.217147 *Math.Log(A);
	return Math.Pow(10.0, x);
  }
  public static double ApparentAsteroidDiameter(double Delta, double d)
  {
	return 0.0013788 *d/Delta;
  }
}

