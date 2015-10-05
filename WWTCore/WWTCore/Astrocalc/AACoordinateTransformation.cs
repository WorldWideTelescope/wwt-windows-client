using System;
using System.Diagnostics;
//
//Module : AACOORDINATETRANSFORMATION.CPP
//Purpose: Implementation for the algorithms which convert between the various celestial coordinate systems
//Created: PJN / 29-12-2003
//History: PJN / 14-02-2004 1. Fixed a "minus zero" bug in the function CAACoordinateTransformation::DMSToDegrees.
//                          The sign of the value is now taken explicitly from the new bPositive boolean
//                          parameter. Thanks to Patrick Wallace for reporting this problem.
//         PJN / 02-06-2005 1. Most of the angular conversion functions have now been reimplemented as simply
//                          numeric constants. All of the AA+ code has also been updated to use these new constants.
//         PJN / 25-01-2007 1. Fixed a minor compliance issue with GCC in the AACoordinateTransformation.h to do
//                          with the declaration of various methods. Thanks to Mathieu Peyréga for reporting this
//                          issue.
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

//////////////////////// Includes /////////////////////////////////////////////


/////////////////////// Includes //////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////
public struct CAA2DCoordinate
{
    public CAA2DCoordinate(double x, double y)
    {
        X = x;
        Y = y;
    }

    //Member variables
    public double X;
    public double Y;
}

public struct CAA3DCoordinate
{
    public CAA3DCoordinate(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    //member variables
    public double X;
    public double Y;
    public double Z;
}



public class  CAACoordinateTransformation
{
//Conversion functions

  /////////////////////// Implementation ////////////////////////////////////////
  
  public static CAA2DCoordinate Equatorial2Ecliptic(double Alpha, double Delta, double Epsilon)
  {
	Alpha = HoursToRadians(Alpha);
	Delta = DegreesToRadians(Delta);
	Epsilon = DegreesToRadians(Epsilon);
  
	var Ecliptic = new CAA2DCoordinate
	{
	    X = RadiansToDegrees(Math.Atan2(Math.Sin(Alpha)*Math.Cos(Epsilon) + Math.Tan(Delta)*Math.Sin(Epsilon), Math.Cos(Alpha)))
	};
      if (Ecliptic.X < 0)
	  Ecliptic.X += 360;
	Ecliptic.Y = RadiansToDegrees(Math.Asin(Math.Sin(Delta)*Math.Cos(Epsilon) - Math.Cos(Delta)*Math.Sin(Epsilon)*Math.Sin(Alpha)));
  
	return Ecliptic;
  }
	public static CAA2DCoordinate Ecliptic2Equatorial(double Lambda, double Beta, double Epsilon)
	{
	  Lambda = DegreesToRadians(Lambda);
	  Beta = DegreesToRadians(Beta);
	  Epsilon = DegreesToRadians(Epsilon);
	
	  var Equatorial = new CAA2DCoordinate
	  {
	      X =
	          RadiansToHours(Math.Atan2(Math.Sin(Lambda)*Math.Cos(Epsilon) - Math.Tan(Beta)*Math.Sin(Epsilon),
	              Math.Cos(Lambda)))
	  };
	    if (Equatorial.X < 0)
		Equatorial.X += 24;
	  Equatorial.Y = RadiansToDegrees(Math.Asin(Math.Sin(Beta)*Math.Cos(Epsilon) + Math.Cos(Beta)*Math.Sin(Epsilon)*Math.Sin(Lambda)));
	
		return Equatorial;
	}
	public static CAA2DCoordinate Equatorial2Horizontal(double LocalHourAngle, double Delta, double Latitude)
	{
	  LocalHourAngle = HoursToRadians(LocalHourAngle);
	  Delta = DegreesToRadians(Delta);
	  Latitude = DegreesToRadians(Latitude);
	
	  var Horizontal = new CAA2DCoordinate
	  {
	      X =
	          RadiansToDegrees(Math.Atan2(Math.Sin(LocalHourAngle),
	              Math.Cos(LocalHourAngle)*Math.Sin(Latitude) - Math.Tan(Delta)*Math.Cos(Latitude)))
	  };
	    if (Horizontal.X < 0)
		Horizontal.X += 360;
	  Horizontal.Y = RadiansToDegrees(Math.Asin(Math.Sin(Latitude)*Math.Sin(Delta) + Math.Cos(Latitude)*Math.Cos(Delta)*Math.Cos(LocalHourAngle)));
	
		return Horizontal;
	}
	public static CAA2DCoordinate Horizontal2Equatorial(double Azimuth, double Altitude, double Latitude)
	{
	  //Convert from degress to radians
	  Azimuth = DegreesToRadians(Azimuth);
	  Altitude = DegreesToRadians(Altitude);
	  Latitude = DegreesToRadians(Latitude);
	
	  var Equatorial = new CAA2DCoordinate
	  {
	      X =
	          RadiansToHours(Math.Atan2(Math.Sin(Azimuth),
	              Math.Cos(Azimuth)*Math.Sin(Latitude) + Math.Tan(Altitude)*Math.Cos(Latitude)))
	  };
	    if (Equatorial.X < 0)
		Equatorial.X += 24;
	  Equatorial.Y = RadiansToDegrees(Math.Asin(Math.Sin(Latitude)*Math.Sin(Altitude) - Math.Cos(Latitude)*Math.Cos(Altitude)*Math.Cos(Azimuth)));
	
		return Equatorial;
	}
	public static CAA2DCoordinate Equatorial2Galactic(double Alpha, double Delta)
	{
	  Alpha = 192.25 - HoursToDegrees(Alpha);
	  Alpha = DegreesToRadians(Alpha);
	  Delta = DegreesToRadians(Delta);
	
	  var Galactic = new CAA2DCoordinate
	  {
	      X =
	          RadiansToDegrees(Math.Atan2(Math.Sin(Alpha),
	              Math.Cos(Alpha)*Math.Sin(DegreesToRadians(27.4)) - Math.Tan(Delta)*Math.Cos(DegreesToRadians(27.4))))
	  };
	    Galactic.X = 303 - Galactic.X;
	  if (Galactic.X >= 360)
		Galactic.X -= 360;
	  Galactic.Y = RadiansToDegrees(Math.Asin(Math.Sin(Delta)*Math.Sin(DegreesToRadians(27.4)) + Math.Cos(Delta)*Math.Cos(DegreesToRadians(27.4))*Math.Cos(Alpha)));
	
		return Galactic;
	}
	public static CAA2DCoordinate Galactic2Equatorial(double l, double b)
	{
	  l -= 123;
	  l = DegreesToRadians(l);
	  b = DegreesToRadians(b);
	
	  var Equatorial = new CAA2DCoordinate
	  {
	      X =
	          RadiansToDegrees(Math.Atan2(Math.Sin(l),
	              Math.Cos(l)*Math.Sin(DegreesToRadians(27.4)) - Math.Tan(b)*Math.Cos(DegreesToRadians(27.4))))
	  };
	    Equatorial.X += 12.25;
	  if (Equatorial.X < 0)
		Equatorial.X += 360;
	  Equatorial.X = DegreesToHours(Equatorial.X);
	  Equatorial.Y = RadiansToDegrees(Math.Asin(Math.Sin(b)*Math.Sin(DegreesToRadians(27.4)) + Math.Cos(b)*Math.Cos(DegreesToRadians(27.4))*Math.Cos(l)));
	
		return Equatorial;
	}

//Inlined functions
  public static double DegreesToRadians(double Degrees)
  {
	return Degrees * 0.017453292519943295769236907684886;
  }

  public static double RadiansToDegrees(double Radians)
  {
	return Radians * 57.295779513082320876798154814105;
  }

  public static double RadiansToHours(double Radians)
  {
	return Radians * 3.8197186342054880584532103209403;
  }

  public static double HoursToRadians(double Hours)
  {
	return Hours * 0.26179938779914943653855361527329;
  }

  public static double HoursToDegrees(double Hours)
  {
	return Hours * 15;
  }

  public static double DegreesToHours(double Degrees)
  {
	return Degrees / 15;
  }

  public static double PI()
  {
	return 3.1415926535897932384626433832795;
  }

  public static double MapTo0To360Range(double Degrees)
  {
      return Degrees - Math.Floor(Degrees / 360.0) * 360.0;
  }

  public static double MapTo0To24Range(double HourAngle)
  {
      return HourAngle - Math.Floor(HourAngle / 24.0) * 24.0;
  }

  public static double DMSToDegrees(double Degrees, double Minutes, double Seconds)
  {
	  return DMSToDegrees(Degrees, Minutes, Seconds, true);
  }
//C++ TO C# CONVERTER NOTE: C# does not allow default values for parameters. Overloaded methods are inserted above.
//ORIGINAL LINE: static double DMSToDegrees(double Degrees, double Minutes, double Seconds, bool bPositive = true)
  public static double DMSToDegrees(double Degrees, double Minutes, double Seconds, bool bPositive)
  {
	//validate our parameters
	if (!bPositive)
	{
	  Debug.Assert(Degrees >= 0); //All parameters should be non negative if the "bPositive" parameter is false
	  Debug.Assert(Minutes >= 0);
	  Debug.Assert(Seconds >= 0);
	}
  
	if (bPositive)
	  return Degrees + Minutes/60 + Seconds/3600;
      return -Degrees - Minutes/60 - Seconds/3600;
  }
}
