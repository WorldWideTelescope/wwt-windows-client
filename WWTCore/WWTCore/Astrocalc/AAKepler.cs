using System;
//
//Module : AAKEPLER.CPP
//Purpose: Implementation for the algorithms which solve Kepler's equation
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

//////////////////// Includes /////////////////////////////////////////////////



public class  CAAKepler
{
//Static methods

  //////////////////// Implementation ///////////////////////////////////////////
  
  public static double Calculate(double M, double e)
  {
	  return Calculate(M, e, 53);
  }
//C++ TO C# CONVERTER NOTE: C# does not allow default values for parameters. Overloaded methods are inserted above.
//ORIGINAL LINE: static double Calculate(double M, double e, int nIterations = 53)
  public static double Calculate(double M, double e, int nIterations)
  {
	//Convert from degrees to radians
	M = CAACoordinateTransformation.DegreesToRadians(M);
	double PI = CAACoordinateTransformation.PI();
  
	double F = 1;
	if (M < 0)
	  F = -1;
	M = Math.Abs(M) / (2 * PI);
	M = (M - (int)(M))*2 *PI *F;
	if (M < 0)
	  M += 2 *PI;
	F = 1;
	if (M > PI)
	  F = -1;
	if (M > PI)
	  M = 2 *PI - M;
  
	double E = PI / 2;
	double scale = PI / 4;
	for (int i =0; i<nIterations; i++)
	{
	  double R = E - e *Math.Sin(E);
	  if (M > R)
		E += scale;
	  else
		E -= scale;
	  scale /= 2;
	}
  
	//Convert the result back to degrees
	return CAACoordinateTransformation.RadiansToDegrees(E) * F;
  }
}
