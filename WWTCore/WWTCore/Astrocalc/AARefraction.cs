using System;
//
//Module : AAREFRACTION.CPP
//Purpose: Implementation for the algorithms which model Atmospheric Refraction
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

//////////////////////////////////// Includes /////////////////////////////////


////////////////////// Classes ////////////////////////////////////////////////

public class  CAARefraction
{
//Static methods

  /////////////////////////////////// Implementation ////////////////////////////
  
  public static double RefractionFromApparent(double Altitude, double Pressure)
  {
	  return RefractionFromApparent(Altitude, Pressure, 10);
  }
  public static double RefractionFromApparent(double Altitude)
  {
	  return RefractionFromApparent(Altitude, 1010, 10);
  }
//C++ TO C# CONVERTER NOTE: C# does not allow default values for parameters. Overloaded methods are inserted above.
//ORIGINAL LINE: static double RefractionFromApparent(double Altitude, double Pressure = 1010, double Temperature = 10)
  public static double RefractionFromApparent(double Altitude, double Pressure, double Temperature)
  {
	var @value = 1 / (Math.Tan(CAACoordinateTransformation.DegreesToRadians(Altitude + 7.31/(Altitude + 4.4)))) + 0.0013515;
	@value *= (Pressure/1010 * 283/(273+Temperature));
	@value /= 60;
	return @value;
  }
  public static double RefractionFromTrue(double Altitude, double Pressure)
  {
	  return RefractionFromTrue(Altitude, Pressure, 10);
  }
  public static double RefractionFromTrue(double Altitude)
  {
	  return RefractionFromTrue(Altitude, 1010, 10);
  }
//C++ TO C# CONVERTER NOTE: C# does not allow default values for parameters. Overloaded methods are inserted above.
//ORIGINAL LINE: static double RefractionFromTrue(double Altitude, double Pressure = 1010, double Temperature = 10)
  public static double RefractionFromTrue(double Altitude, double Pressure, double Temperature)
  {
	var @value = 1.02 / (Math.Tan(CAACoordinateTransformation.DegreesToRadians(Altitude + 10.3/(Altitude + 5.11)))) + 0.0019279;
	@value *= (Pressure/1010 * 283/(273+Temperature));
	@value /= 60;
	return @value;
  }
}
