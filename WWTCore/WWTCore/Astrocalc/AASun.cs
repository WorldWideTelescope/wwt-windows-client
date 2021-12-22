using System;
//
//Module : AASUN.CPP
//Purpose: Implementation for the algorithms which obtain the position of the Sun
//Created: PJN / 29-12-2003
//History: PJN / 17-01-2007 1. Changed name of CAASun::ApparentEclipticLongtitude to 
//                          CAASun::ApparentEclipticLongitude. Thanks to Mathieu Peyréga for reporting this
//                          typo!.
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

//////////////////////////// Includes /////////////////////////////////////////


/////////////////////// Includes //////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAASun
{
//Static methods

  //////////////////////////// Implementation ///////////////////////////////////
  
  public static double GeometricEclipticLongitude(double JD)
  {
	return CAACoordinateTransformation.MapTo0To360Range(CAAEarth.EclipticLongitude(JD) + 180);
  }
  public static double GeometricEclipticLatitude(double JD)
  {
	return -CAAEarth.EclipticLatitude(JD);
  }
  public static double GeometricEclipticLongitudeJ2000(double JD)
  {
	return CAACoordinateTransformation.MapTo0To360Range(CAAEarth.EclipticLongitudeJ2000(JD) + 180);
  }
  public static double GeometricEclipticLatitudeJ2000(double JD)
  {
	return -CAAEarth.EclipticLatitudeJ2000(JD);
  }
  public static double GeometricFK5EclipticLongitude(double JD)
  {
	//Convert to the FK5 stystem
	double Longitude = GeometricEclipticLongitude(JD);
	double Latitude = GeometricEclipticLatitude(JD);
	Longitude += CAAFK5.CorrectionInLongitude(Longitude, Latitude, JD);
  
	return Longitude;
  }
  public static double GeometricFK5EclipticLatitude(double JD)
  {
	//Convert to the FK5 stystem
	double Longitude = GeometricEclipticLongitude(JD);
	double Latitude = GeometricEclipticLatitude(JD);
	double SunLatCorrection = CAAFK5.CorrectionInLatitude(Longitude, JD);
	Latitude += SunLatCorrection;
  
	return Latitude;
  }
  public static double ApparentEclipticLongitude(double JD)
  {
	double Longitude = GeometricFK5EclipticLongitude(JD);
  
	//Apply the correction in longitude due to nutation
	Longitude += CAACoordinateTransformation.DMSToDegrees(0, 0, CAANutation.NutationInLongitude(JD));
  
	//Apply the correction in longitude due to aberration
	double R = CAAEarth.RadiusVector(JD);
	Longitude -= CAACoordinateTransformation.DMSToDegrees(0, 0, 20.4898 / R);
  
	return Longitude;
  }
  public static double ApparentEclipticLatitude(double JD)
  {
	return GeometricFK5EclipticLatitude(JD);
  }
  public static CAA3DCoordinate EclipticRectangularCoordinatesMeanEquinox(double JD)
  {
	double Longitude = CAACoordinateTransformation.DegreesToRadians(GeometricFK5EclipticLongitude(JD));
	double Latitude = CAACoordinateTransformation.DegreesToRadians(GeometricFK5EclipticLatitude(JD));
	double R = CAAEarth.RadiusVector(JD);
	double epsilon = CAACoordinateTransformation.DegreesToRadians(CAANutation.MeanObliquityOfEcliptic(JD));
  
	CAA3DCoordinate @value = new CAA3DCoordinate();
	@value.X = R * Math.Cos(Latitude) * Math.Cos(Longitude);
	@value.Y = R * (Math.Cos(Latitude) * Math.Sin(Longitude) * Math.Cos(epsilon) - Math.Sin(Latitude) * Math.Sin(epsilon));
	@value.Z = R * (Math.Cos(Latitude) * Math.Sin(Longitude) * Math.Sin(epsilon) + Math.Sin(Latitude) * Math.Cos(epsilon));
  
	return @value;
  }
  public static CAA3DCoordinate EclipticRectangularCoordinatesJ2000(double JD)
  {
	double Longitude = GeometricEclipticLongitudeJ2000(JD);
	Longitude = CAACoordinateTransformation.DegreesToRadians(Longitude);
	double Latitude = GeometricEclipticLatitudeJ2000(JD);
	Latitude = CAACoordinateTransformation.DegreesToRadians(Latitude);
	double R = CAAEarth.RadiusVector(JD);
  
	CAA3DCoordinate @value = new CAA3DCoordinate();
	double coslatitude = Math.Cos(Latitude);
	@value.X = R * coslatitude * Math.Cos(Longitude);
	@value.Y = R * coslatitude * Math.Sin(Longitude);
	@value.Z = R * Math.Sin(Latitude);
  
	return @value;
  }
  public static CAA3DCoordinate EquatorialRectangularCoordinatesJ2000(double JD)
  {
	CAA3DCoordinate @value = EclipticRectangularCoordinatesJ2000(JD);
	@value = CAAFK5.ConvertVSOPToFK5J2000(@value);
  
	return @value;
  }
  public static CAA3DCoordinate EquatorialRectangularCoordinatesB1950(double JD)
  {
	CAA3DCoordinate @value = EclipticRectangularCoordinatesJ2000(JD);
	@value = CAAFK5.ConvertVSOPToFK5B1950(@value);
  
	return @value;
  }
  public static CAA3DCoordinate EquatorialRectangularCoordinatesAnyEquinox(double JD, double JDEquinox)
  {
	CAA3DCoordinate @value = EquatorialRectangularCoordinatesJ2000(JD);
	@value = CAAFK5.ConvertVSOPToFK5AnyEquinox(@value, JDEquinox);
  
	return @value;
  }
}
