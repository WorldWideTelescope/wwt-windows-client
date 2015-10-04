using System;
//
//Module : AARPRECESSION.CPP
//Purpose: Implementation for the algorithms for Precession
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


/////////////////////// Includes //////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAPrecession
{
//Static methods
  public static CAA2DCoordinate PrecessEquatorial(double Alpha, double Delta, double JD0, double JD)
  {
	var T = (JD0 - 2451545.0) / 36525;
	var Tsquared = T *T;
	var t = (JD - JD0) / 36525;
	var tsquared = t *t;
	var tcubed = tsquared * t;
  
	//Now convert to radians
	Alpha = CAACoordinateTransformation.HoursToRadians(Alpha);
	Delta = CAACoordinateTransformation.DegreesToRadians(Delta);
  
	var sigma = (2306.2181 + 1.39656 *T - 0.000139 *Tsquared)*t + (0.30188 - 0.0000344 *T)*tsquared + 0.017988 *tcubed;
	sigma = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, sigma));
  
	var zeta = (2306.2181 + 1.39656 *T - 0.000138 *Tsquared)*t + (1.09468 + 0.000066 *T)*tsquared + 0.018203 *tcubed;
	zeta = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, zeta));
  
	var phi = (2004.3109 - 0.8533 *T - 0.000217 *Tsquared)*t - (0.42665 + 0.000217 *T)*tsquared - 0.041833 *tcubed;
	phi = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, phi));
  
	var A = Math.Cos(Delta) * Math.Sin(Alpha + sigma);
	var B = Math.Cos(phi)*Math.Cos(Delta)*Math.Cos(Alpha + sigma) - Math.Sin(phi)*Math.Sin(Delta);
	var C = Math.Sin(phi)*Math.Cos(Delta)*Math.Cos(Alpha + sigma) + Math.Cos(phi)*Math.Sin(Delta);
  
	var @value = new CAA2DCoordinate();
	@value.X = CAACoordinateTransformation.RadiansToHours(Math.Atan2(A, B) + zeta);
	if (@value.X < 0)
	  @value.X += 24;
	@value.Y = CAACoordinateTransformation.RadiansToDegrees(Math.Asin(C));
  
	return @value;
  }
  public static CAA2DCoordinate PrecessEquatorialFK4(double Alpha, double Delta, double JD0, double JD)
  {
	var T = (JD0 - 2415020.3135) / 36524.2199;
	var t = (JD - JD0) / 36524.2199;
	var tsquared = t *t;
	var tcubed = tsquared * t;
  
	//Now convert to radians
	Alpha = CAACoordinateTransformation.HoursToRadians(Alpha);
	Delta = CAACoordinateTransformation.DegreesToRadians(Delta);
  
	var sigma = (2304.250 + 1.396 *T)*t + 0.302 *tsquared + 0.018 *tcubed;
	sigma = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, sigma));
  
	var zeta = 0.791 *tsquared + 0.001 *tcubed;
	zeta = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, zeta));
	zeta += sigma;
  
	var phi = (2004.682 - 0.853 *T)*t - 0.426 *tsquared - 0.042 *tcubed;
	phi = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, phi));
  
	var A = Math.Cos(Delta) * Math.Sin(Alpha + sigma);
	var B = Math.Cos(phi)*Math.Cos(Delta)*Math.Cos(Alpha + sigma) - Math.Sin(phi)*Math.Sin(Delta);
	var C = Math.Sin(phi)*Math.Cos(Delta)*Math.Cos(Alpha + sigma) + Math.Cos(phi)*Math.Sin(Delta);
  
	var @value = new CAA2DCoordinate();
	@value.X = CAACoordinateTransformation.RadiansToHours(Math.Atan2(A, B) + zeta);
	if (@value.X < 0)
	  @value.X += 24;
	@value.Y = CAACoordinateTransformation.RadiansToDegrees(Math.Asin(C));
  
	return @value;
  }
  public static CAA2DCoordinate PrecessEcliptic(double Lambda, double Beta, double JD0, double JD)
  {
	var T = (JD0 - 2451545.0) / 36525;
	var Tsquared = T *T;
	var t = (JD - JD0) / 36525;
	var tsquared = t *t;
	var tcubed = tsquared * t;
  
	//Now convert to radians
	Lambda = CAACoordinateTransformation.DegreesToRadians(Lambda);
	Beta = CAACoordinateTransformation.DegreesToRadians(Beta);
  
	var eta = (47.0029 - 0.06603 *T + 0.000598 *Tsquared)*t + (-0.03302 + 0.000598 *T)*tsquared + 0.00006 *tcubed;
	eta = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, eta));
  
	var pi = 174.876384 *3600 + 3289.4789 *T + 0.60622 *Tsquared - (869.8089 + 0.50491 *T)*t + 0.03536 *tsquared;
	pi = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, pi));
  
	var p = (5029.0966 + 2.22226 *T - 0.000042 *Tsquared)*t + (1.11113 - 0.000042 *T)*tsquared - 0.000006 *tcubed;
	p = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, p));
  
	var A = Math.Cos(eta)*Math.Cos(Beta)*Math.Sin(pi - Lambda) - Math.Sin(eta)*Math.Sin(Beta);
	var B = Math.Cos(Beta)*Math.Cos(pi - Lambda);
	var C = Math.Cos(eta)*Math.Sin(Beta) + Math.Sin(eta)*Math.Cos(Beta)*Math.Sin(pi - Lambda);
  
	var @value = new CAA2DCoordinate();
	@value.X = CAACoordinateTransformation.RadiansToDegrees(p + pi - Math.Atan2(A, B));
	if (@value.X < 0)
	  @value.X += 360;
	@value.Y = CAACoordinateTransformation.RadiansToDegrees(Math.Asin(C));
  
	return @value;
  }
  public static CAA2DCoordinate EquatorialPMToEcliptic(double Alpha, double Delta, double Beta, double PMAlpha, double PMDelta, double Epsilon)
  {
	//Convert to radians
	Epsilon = CAACoordinateTransformation.DegreesToRadians(Epsilon);
	Alpha = CAACoordinateTransformation.HoursToRadians(Alpha);
	Delta = CAACoordinateTransformation.DegreesToRadians(Delta);
	Beta = CAACoordinateTransformation.DegreesToRadians(Beta);
  
	var cosb = Math.Cos(Beta);
	var sinEpsilon = Math.Sin(Epsilon);
  
	var @value = new CAA2DCoordinate();
	@value.X = (PMDelta *sinEpsilon *Math.Cos(Alpha) + PMAlpha *Math.Cos(Delta)*(Math.Cos(Epsilon)*Math.Cos(Delta) + sinEpsilon *Math.Sin(Delta)*Math.Sin(Alpha)))/(cosb *cosb);
	@value.Y = (PMDelta*(Math.Cos(Epsilon)*Math.Cos(Delta) + sinEpsilon *Math.Sin(Delta)*Math.Sin(Alpha)) - PMAlpha *sinEpsilon *Math.Cos(Alpha)*Math.Cos(Delta))/cosb;
  
	return @value;
  }

  ///////////////////////////////// Implementation //////////////////////////////
  
  public static CAA2DCoordinate AdjustPositionUsingUniformProperMotion(double t, double Alpha, double Delta, double PMAlpha, double PMDelta)
  {
	var @value = new CAA2DCoordinate();
	@value.X = Alpha + (PMAlpha * t / 3600);
	@value.Y = Delta + (PMDelta * t / 3600);
  
	return @value;
  }
  public static CAA2DCoordinate AdjustPositionUsingMotionInSpace(double r, double DeltaR, double t, double Alpha, double Delta, double PMAlpha, double PMDelta)
  {
	//Convert DeltaR from km/s to Parsecs / Year
	DeltaR /= 977792;
  
	//Convert from seconds of time to Radians / Year
	PMAlpha /= 13751;
  
	//Convert from seconds of arc to Radians / Year
	PMDelta /= 206265;
  
	//Now convert to radians
	Alpha = CAACoordinateTransformation.HoursToRadians(Alpha);
	Delta = CAACoordinateTransformation.DegreesToRadians(Delta);
  
	var x = r * Math.Cos(Delta) * Math.Cos(Alpha);
	var y = r * Math.Cos(Delta) * Math.Sin(Alpha);
	var z = r * Math.Sin(Delta);
  
	var DeltaX = x/r *DeltaR - z *PMDelta *Math.Cos(Alpha) - y *PMAlpha;
	var DeltaY = y/r *DeltaR - z *PMDelta *Math.Sin(Alpha) + x *PMAlpha;
	var DeltaZ = z/r *DeltaR + r *PMDelta *Math.Cos(Delta);
  
	x += t *DeltaX;
	y += t *DeltaY;
	z += t *DeltaZ;
  
	var @value = new CAA2DCoordinate();
	@value.X = CAACoordinateTransformation.RadiansToHours(Math.Atan2(y, x));
	if (@value.X < 0)
	  @value.X += 24;
  
	@value.Y = CAACoordinateTransformation.RadiansToDegrees(Math.Atan2(z, Math.Sqrt(x *x + y *y)));
  
	return @value;
  }
}
