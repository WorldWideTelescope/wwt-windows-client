using System;
//
//Module : AAPHYSICALMARS.CPP
//Purpose: Implementation for the algorithms which obtain the physical parameters of Mars
//Created: PJN / 04-01-2004
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


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAPhysicalMarsDetails
{
  public CAAPhysicalMarsDetails()
  {
	  DE = 0;
	  DS = 0;
	  w = 0;
	  P = 0;
	  X = 0;
	  k = 0;
	  q = 0;
	  d = 0;
  }

//Member variables
  public double DE;
  public double DS;
  public double w;
  public double P;
  public double X;
  public double k;
  public double q;
  public double d;
}

public class  CAAPhysicalMars
{
//Static methods

  //////////////////////////////// Implementation ///////////////////////////////
  
  public static CAAPhysicalMarsDetails Calculate(double JD)
  {
	//What will be the return value
	var details = new CAAPhysicalMarsDetails();
  
	//Step 1
	var T = (JD - 2451545) / 36525;
	var Lambda0 = 352.9065 + 1.17330 *T;
	var Lambda0rad = CAACoordinateTransformation.DegreesToRadians(Lambda0);
	var Beta0 = 63.2818 - 0.00394 *T;
	var Beta0rad = CAACoordinateTransformation.DegreesToRadians(Beta0);
  
	//Step 2
	var l0 = CAAEarth.EclipticLongitude(JD);
	var l0rad = CAACoordinateTransformation.DegreesToRadians(l0);
	var b0 = CAAEarth.EclipticLatitude(JD);
	var b0rad = CAACoordinateTransformation.DegreesToRadians(b0);
	var R = CAAEarth.RadiusVector(JD);
  
	double PreviousLightTravelTime = 0;
	double LightTravelTime = 0;
	double x = 0;
	double y = 0;
	double z = 0;
	var bIterate = true;
	double DELTA = 0;
	double l = 0;
	double lrad = 0;
	double b = 0;
      double r = 0;
	while (bIterate)
	{
	  var JD2 = JD - LightTravelTime;
  
	  //Step 3
	  l = CAAMars.EclipticLongitude(JD2);
	  lrad = CAACoordinateTransformation.DegreesToRadians(l);
	  b = CAAMars.EclipticLatitude(JD2);
	  double brad = CAACoordinateTransformation.DegreesToRadians(b);
	  r = CAAMars.RadiusVector(JD2);
  
	  //Step 4
	  x = r *Math.Cos(brad)*Math.Cos(lrad) - R *Math.Cos(l0rad);
	  y = r *Math.Cos(brad)*Math.Sin(lrad) - R *Math.Sin(l0rad);
	  z = r *Math.Sin(brad) - R *Math.Sin(b0rad);
	  DELTA = Math.Sqrt(x *x + y *y + z *z);
	  LightTravelTime = CAAElliptical.DistanceToLightTime(DELTA);
  
	  //Prepare for the next loop around
	  bIterate = (Math.Abs(LightTravelTime - PreviousLightTravelTime) > 2E-6); //2E-6 correponds to 0.17 of a second
	  if (bIterate)
		PreviousLightTravelTime = LightTravelTime;
	}
  
	//Step 5
	var lambdarad = Math.Atan2(y, x);
	var lambda = CAACoordinateTransformation.RadiansToDegrees(lambdarad);
	var betarad = Math.Atan2(z, Math.Sqrt(x *x + y *y));
	var beta = CAACoordinateTransformation.RadiansToDegrees(betarad);
  
	//Step 6
	details.DE = CAACoordinateTransformation.RadiansToDegrees(Math.Asin(-Math.Sin(Beta0rad)*Math.Sin(betarad) - Math.Cos(Beta0rad)*Math.Cos(betarad)*Math.Cos(Lambda0rad - lambdarad)));
  
	//Step 7
	var N = 49.5581 + 0.7721 *T;
	var Nrad = CAACoordinateTransformation.DegreesToRadians(N);
  
	var ldash = l - 0.00697/r;
	var ldashrad = CAACoordinateTransformation.DegreesToRadians(ldash);
	var bdash = b - 0.000225*(Math.Cos(lrad - Nrad)/r);
	var bdashrad = CAACoordinateTransformation.DegreesToRadians(bdash);
  
	//Step 8
	details.DS = CAACoordinateTransformation.RadiansToDegrees(Math.Asin(-Math.Sin(Beta0rad)*Math.Sin(bdashrad) - Math.Cos(Beta0rad)*Math.Cos(bdashrad)*Math.Cos(Lambda0rad - ldashrad)));
  
	//Step 9
	var W = CAACoordinateTransformation.MapTo0To360Range(11.504 + 350.89200025*(JD - LightTravelTime - 2433282.5));
  
	//Step 10
	var e0 = CAANutation.MeanObliquityOfEcliptic(JD);
	var e0rad = CAACoordinateTransformation.DegreesToRadians(e0);
	var PoleEquatorial = CAACoordinateTransformation.Ecliptic2Equatorial(Lambda0, Beta0, e0);
	var alpha0rad = CAACoordinateTransformation.HoursToRadians(PoleEquatorial.X);
	var delta0rad = CAACoordinateTransformation.DegreesToRadians(PoleEquatorial.Y);
  
	//Step 11
	var u = y *Math.Cos(e0rad) - z *Math.Sin(e0rad);
	var v = y *Math.Sin(e0rad) + z *Math.Cos(e0rad);
	var alpharad = Math.Atan2(u, x);
	var alpha = CAACoordinateTransformation.RadiansToHours(alpharad);
	var deltarad = Math.Atan2(v, Math.Sqrt(x *x + u *u));
	var delta = CAACoordinateTransformation.RadiansToDegrees(deltarad);
	var xi = Math.Atan2(Math.Sin(delta0rad)*Math.Cos(deltarad)*Math.Cos(alpha0rad - alpharad) - Math.Sin(deltarad)*Math.Cos(delta0rad), Math.Cos(deltarad)*Math.Sin(alpha0rad - alpharad));
  
	//Step 12
	details.w = CAACoordinateTransformation.MapTo0To360Range(W - CAACoordinateTransformation.RadiansToDegrees(xi));
  
	//Step 13
	var NutationInLongitude = CAANutation.NutationInLongitude(JD);
	var NutationInObliquity = CAANutation.NutationInObliquity(JD);
  
	//Step 14
	lambda += 0.005693 *Math.Cos(l0rad - lambdarad)/Math.Cos(betarad);
	beta += 0.005693 *Math.Sin(l0rad - lambdarad)*Math.Sin(betarad);
  
	//Step 15
	Lambda0 += NutationInLongitude/3600;
	Lambda0rad = CAACoordinateTransformation.DegreesToRadians(Lambda0);
	lambda += NutationInLongitude/3600;
	lambdarad = CAACoordinateTransformation.DegreesToRadians(lambda);
	e0 += NutationInObliquity/3600;
	e0rad = CAACoordinateTransformation.DegreesToRadians(e0rad);
  
	//Step 16
	var ApparentPoleEquatorial = CAACoordinateTransformation.Ecliptic2Equatorial(Lambda0, Beta0, e0);
	var alpha0dash = CAACoordinateTransformation.HoursToRadians(ApparentPoleEquatorial.X);
	var delta0dash = CAACoordinateTransformation.DegreesToRadians(ApparentPoleEquatorial.Y);
	var ApparentMars = CAACoordinateTransformation.Ecliptic2Equatorial(lambda, beta, e0);
	var alphadash = CAACoordinateTransformation.HoursToRadians(ApparentMars.X);
	var deltadash = CAACoordinateTransformation.DegreesToRadians(ApparentMars.Y);
  
	//Step 17
	details.P = CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(Math.Atan2(Math.Cos(delta0dash)*Math.Sin(alpha0dash - alphadash), Math.Sin(delta0dash)*Math.Cos(deltadash) - Math.Cos(delta0dash)*Math.Sin(deltadash)*Math.Cos(alpha0dash - alphadash))));
  
	//Step 18
	var SunLambda = CAASun.GeometricEclipticLongitude(JD);
	var SunBeta = CAASun.GeometricEclipticLatitude(JD);
	var SunEquatorial = CAACoordinateTransformation.Ecliptic2Equatorial(SunLambda, SunBeta, e0);
	details.X = CAAMoonIlluminatedFraction.PositionAngle(SunEquatorial.X, SunEquatorial.Y, alpha, delta);
  
	//Step 19
	details.d = 9.36 / DELTA;
	details.k = CAAIlluminatedFraction.IlluminatedFraction(r, R, DELTA);
	details.q = (1 - details.k)*details.d;
  
	return details;
  }
}
