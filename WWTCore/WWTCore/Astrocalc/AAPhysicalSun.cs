using System;
//
//Module : AAPHYSICALSUN.CPP
//Purpose: Implementation for the algorithms which obtain the physical parameters of the Sun
//Created: PJN / 29-12-2003
//History: PJN / 16-06-2004 1) Fixed a typo in the calculation of SunLongDash in CAAPhysicalSun::Calculate.
//                          Thanks to Brian Orme for spotting this problem.
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


//////////////////////////// Classes //////////////////////////////////////////

public class  CAAPhysicalSunDetails
{
//Constructors / Destructors
  public CAAPhysicalSunDetails()
  {
	  P = 0;
	  B0 = 0;
	  L0 = 0;
  }

//Member variables
  public double P;
  public double B0;
  public double L0;
}

public class  CAAPhysicalSun
{
//Static methods

  //////////////////////////////// Implementation ///////////////////////////////
  
  public static CAAPhysicalSunDetails Calculate(double JD)
  {
	var theta = CAACoordinateTransformation.MapTo0To360Range((JD - 2398220) * 360 / 25.38);
	var I = 7.25;
	var K = 73.6667 + 1.3958333*(JD - 2396758)/36525;
  
	//Calculate the apparent longitude of the sun (excluding the effect of nutation)
	var L = CAAEarth.EclipticLongitude(JD);
	var R = CAAEarth.RadiusVector(JD);
	var SunLong = L + 180 - CAACoordinateTransformation.DMSToDegrees(0, 0, 20.4898 / R);
	var SunLongDash = SunLong + CAACoordinateTransformation.DMSToDegrees(0, 0, CAANutation.NutationInLongitude(JD));
  
	var epsilon = CAANutation.TrueObliquityOfEcliptic(JD);
  
	//Convert to radians
	epsilon = CAACoordinateTransformation.DegreesToRadians(epsilon);
	SunLong = CAACoordinateTransformation.DegreesToRadians(SunLong);
	SunLongDash = CAACoordinateTransformation.DegreesToRadians(SunLongDash);
	K = CAACoordinateTransformation.DegreesToRadians(K);
	I = CAACoordinateTransformation.DegreesToRadians(I);
	theta = CAACoordinateTransformation.DegreesToRadians(theta);
  
	var x = Math.Atan(-Math.Cos(SunLong)*Math.Tan(epsilon));
	var y = Math.Atan(-Math.Cos(SunLong - K)*Math.Tan(I));
  
	var details = new CAAPhysicalSunDetails
	{
	    P = CAACoordinateTransformation.RadiansToDegrees(x + y),
	    B0 = CAACoordinateTransformation.RadiansToDegrees(Math.Asin(Math.Sin(SunLong - K)*Math.Sin(I)))
	};

      var eta = Math.Atan(Math.Tan(SunLong - K)*Math.Cos(I));
	details.L0 = CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(eta - theta));
  
	return details;
  }
  public static double TimeOfStartOfRotation(int C)
  {
	var JED = 2398140.2270 + 27.2752316 *C;
  
	var M = CAACoordinateTransformation.MapTo0To360Range(281.96 + 26.882476 *C);
	M = CAACoordinateTransformation.DegreesToRadians(M);
  
	JED += (0.1454 *Math.Sin(M) - 0.0085 *Math.Sin(2 *M) - 0.0141 *Math.Cos(2 *M));
  
	return JED;
  }
}
