using System;
//
//Module : AAGLOBE.CPP
//Purpose: Implementation for the algorithms for the Earth's Globe
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

/////////////////////////// Includes //////////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAGlobe
{
//Static methods

	/////////////////////////// Implementation ////////////////////////////////////
	
	public static double RhoSinThetaPrime(double GeographicalLatitude, double Height)
	{
	  GeographicalLatitude = CAACoordinateTransformation.DegreesToRadians(GeographicalLatitude);
	
	  var U = Math.Atan(0.99664719 * Math.Tan(GeographicalLatitude));
	  return 0.99664719 * Math.Sin(U) + (Height/6378149 * Math.Sin(GeographicalLatitude));
	}
	public static double RhoCosThetaPrime(double GeographicalLatitude, double Height)
	{
	  //Convert from degress to radians
	  GeographicalLatitude = CAACoordinateTransformation.DegreesToRadians(GeographicalLatitude);
	
	  var U = Math.Atan(0.99664719 * Math.Tan(GeographicalLatitude));
	  return Math.Cos(U) + (Height/6378149 * Math.Cos(GeographicalLatitude));
	}
	public static double RadiusOfParallelOfLatitude(double GeographicalLatitude)
	{
	  //Convert from degress to radians
	  GeographicalLatitude = CAACoordinateTransformation.DegreesToRadians(GeographicalLatitude);
	
	  var sinGeo = Math.Sin(GeographicalLatitude);
	  return (6378.14 * Math.Cos(GeographicalLatitude)) / (Math.Sqrt(1 - 0.0066943847614084 *sinGeo *sinGeo));
	}
	public static double RadiusOfCurvature(double GeographicalLatitude)
	{
	  //Convert from degress to radians
	  GeographicalLatitude = CAACoordinateTransformation.DegreesToRadians(GeographicalLatitude);
	
	  var sinGeo = Math.Sin(GeographicalLatitude);
	  return (6378.14 * (1 - 0.0066943847614084)) / Math.Pow((1 - 0.0066943847614084 * sinGeo * sinGeo), 1.5);
	}
	public static double DistanceBetweenPoints(double GeographicalLatitude1, double GeographicalLongitude1, double GeographicalLatitude2, double GeographicalLongitude2)
	{
	  //Convert from degress to radians
	  GeographicalLatitude1 = CAACoordinateTransformation.DegreesToRadians(GeographicalLatitude1);
	  GeographicalLatitude2 = CAACoordinateTransformation.DegreesToRadians(GeographicalLatitude2);
	  GeographicalLongitude1 = CAACoordinateTransformation.DegreesToRadians(GeographicalLongitude1);
	  GeographicalLongitude2 = CAACoordinateTransformation.DegreesToRadians(GeographicalLongitude2);
	
	  var F = (GeographicalLatitude1 + GeographicalLatitude2) / 2;
	  var G = (GeographicalLatitude1 - GeographicalLatitude2) / 2;
	  var lambda = (GeographicalLongitude1 - GeographicalLongitude2) / 2;
	  var sinG = Math.Sin(G);
	  var cosG = Math.Cos(G);
	  var cosF = Math.Cos(F);
	  var sinF = Math.Sin(F);
	  var sinLambda = Math.Sin(lambda);
	  var cosLambda = Math.Cos(lambda);
	  var S = (sinG *sinG *cosLambda *cosLambda) + (cosF *cosF *sinLambda *sinLambda);
	  var C = (cosG *cosG *cosLambda *cosLambda) + (sinF *sinF *sinLambda *sinLambda);
	  var w = Math.Atan(Math.Sqrt(S/C));
	  var R = Math.Sqrt(S *C)/w;
	  var D = 2 *w *6378.14;
	  var Hprime = (3 *R - 1) / (2 *C);
	  var Hprime2 = (3 *R + 1) / (2 *S);
	  const double f = 0.0033528131778969144060323814696721;
	
	  return D * (1 + (f *Hprime *sinF *sinF *cosG *cosG) - (f *Hprime2 *cosF *cosF *sinG *sinG));
	}
}
