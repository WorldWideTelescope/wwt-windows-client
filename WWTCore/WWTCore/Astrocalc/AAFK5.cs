using System;
//
//Module : AAFK5.CPP
//Purpose: Implementation for the algorithms to convert to the FK5 standard reference frame
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

/////////////////////// Includes //////////////////////////////////////////////


/////////////////////// Includes //////////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAFK5
{
//Static methods

  /////////////////////// Implementation ////////////////////////////////////////
  
  public static double CorrectionInLongitude(double Longitude, double Latitude, double JD)
  {
	var T = (JD - 2451545) / 36525;
	var Ldash = Longitude - 1.397 *T - 0.00031 *T *T;
  
	//Convert to radians
	Ldash = CAACoordinateTransformation.DegreesToRadians(Ldash);
	Longitude = CAACoordinateTransformation.DegreesToRadians(Longitude);
	Latitude = CAACoordinateTransformation.DegreesToRadians(Latitude);
  
	var @value = -0.09033 + 0.03916*(Math.Cos(Ldash) + Math.Sin(Ldash))*Math.Tan(Latitude);
	return CAACoordinateTransformation.DMSToDegrees(0, 0, @value);
  }
  public static double CorrectionInLatitude(double Longitude, double JD)
  {
	var T = (JD - 2451545) / 36525;
	var Ldash = Longitude - 1.397 *T - 0.00031 *T *T;
  
	//Convert to radians
	Ldash = CAACoordinateTransformation.DegreesToRadians(Ldash);
	Longitude = CAACoordinateTransformation.DegreesToRadians(Longitude);
  
	var @value = 0.03916*(Math.Cos(Ldash) - Math.Sin(Ldash));
	return CAACoordinateTransformation.DMSToDegrees(0, 0, @value);
  }
  public static CAA3DCoordinate ConvertVSOPToFK5J2000(CAA3DCoordinate @value)
  {
	var result = new CAA3DCoordinate
	{
	    X = @value.X + 0.000000440360*@value.Y - 0.000000190919*@value.Z,
	    Y = -0.000000479966*@value.X + 0.917482137087*@value.Y - 0.397776982902*@value.Z,
	    Z = 0.397776982902*@value.Y + 0.917482137087*@value.Z
	};

      return result;
  }
  public static CAA3DCoordinate ConvertVSOPToFK5B1950(CAA3DCoordinate @value)
  {
	var result = new CAA3DCoordinate
	{
	    X = 0.999925702634*@value.X + 0.012189716217*@value.Y + 0.000011134016*@value.Z,
	    Y = -0.011179418036*@value.X + 0.917413998946*@value.Y - 0.397777041885*@value.Z,
	    Z = -0.004859003787*@value.X + 0.397747363646*@value.Y + 0.917482111428*@value.Z
	};

      return result;
  }
  public static CAA3DCoordinate ConvertVSOPToFK5AnyEquinox(CAA3DCoordinate @value, double JDEquinox)
  {
	var t = (JDEquinox - 2451545.0) / 36525;
	var tsquared = t *t;
	var tcubed = tsquared * t;
  
	var sigma = 2306.2181 *t + 0.30188 *tsquared + 0.017988 *tcubed;
	sigma = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, sigma));
  
	var zeta = 2306.2181 *t + 1.09468 *tsquared + 0.018203 *tcubed;
	zeta = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, zeta));
  
	var phi = 2004.3109 *t - 0.42665 *tsquared - 0.041833 *tcubed;
	phi = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.DMSToDegrees(0, 0, phi));
  
	var cossigma = Math.Cos(sigma);
	var coszeta = Math.Cos(zeta);
	var cosphi = Math.Cos(phi);
	var sinsigma = Math.Sin(sigma);
	var sinzeta = Math.Sin(zeta);
	var sinphi = Math.Sin(phi);
  
	var xx = cossigma * coszeta * cosphi -sinsigma *sinzeta;
	var xy = sinsigma * coszeta + cossigma * sinzeta * cosphi;
	var xz = cossigma * sinphi;
	var yx = -cossigma * sinzeta - sinsigma * coszeta * cosphi;
	var yy = cossigma * coszeta - sinsigma * sinzeta * cosphi;
	var yz = -sinsigma * sinphi;
	var zx = -coszeta * sinphi;
	var zy = -sinzeta * sinphi;
	var zz = cosphi;
  
	var result = new CAA3DCoordinate
	{
	    X = xx*@value.X + yx*@value.Y + zx*@value.Z,
	    Y = xy*@value.X + yy*@value.Y + zy*@value.Z,
	    Z = xz*@value.X + yz*@value.Y + zz*@value.Z
	};

      return result;
  }
}
