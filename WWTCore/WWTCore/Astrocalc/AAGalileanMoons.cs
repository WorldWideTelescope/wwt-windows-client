using System;
//
//Module : AAGALILEANMOONS.CPP
//Purpose: Implementation for the algorithms which obtain the positions of the 4 great moons of Jupiter
//Created: PJN / 06-01-2004
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

public class  CAAGalileanMoonDetail
{
//Constructors / Destructors
  public CAAGalileanMoonDetail()
  {
	  MeanLongitude = 0;
	  TrueLongitude = 0;
	  TropicalLongitude = 0;
	  EquatorialLatitude = 0;
	  r = 0;
	  bInTransit = false;
	  bInOccultation = false;
	  bInEclipse = false;
	  bInShadowTransit = false;
  }

//Member variables
  public double MeanLongitude;
  public double TrueLongitude;
  public double TropicalLongitude;
  public double EquatorialLatitude;
  public double r;
  public CAA3DCoordinate EclipticRectangularCoordinates = new CAA3DCoordinate();
  public CAA3DCoordinate TrueRectangularCoordinates = new CAA3DCoordinate();
  public CAA3DCoordinate ApparentRectangularCoordinates = new CAA3DCoordinate();
  public bool bInTransit;
  public bool bInOccultation;
  public bool bInEclipse;
  public bool bInShadowTransit;
  public CAA3DCoordinate ApparentShadowRectangularCoordinates = new CAA3DCoordinate();
}

public class  CAAGalileanMoonsDetails
{
//Member variables
  public CAAGalileanMoonDetail Satellite1 = new CAAGalileanMoonDetail();
  public CAAGalileanMoonDetail Satellite2 = new CAAGalileanMoonDetail();
  public CAAGalileanMoonDetail Satellite3 = new CAAGalileanMoonDetail();
  public CAAGalileanMoonDetail Satellite4 = new CAAGalileanMoonDetail();
}

public class  CAAGalileanMoons
{
//Static methods
  public static CAAGalileanMoonsDetails Calculate(double JD)
  {
	//Calculate the position of the Sun
	var sunlong = CAASun.GeometricEclipticLongitude(JD);
	var sunlongrad = CAACoordinateTransformation.DegreesToRadians(sunlong);
	var beta = CAASun.GeometricEclipticLatitude(JD);
	var betarad = CAACoordinateTransformation.DegreesToRadians(beta);
	var R = CAAEarth.RadiusVector(JD);
  
	//Calculate the the light travel time from Jupiter to the Earth
	double DELTA = 5;
	double PreviousEarthLightTravelTime = 0;
	var EarthLightTravelTime = CAAElliptical.DistanceToLightTime(DELTA);
	var JD1 = JD - EarthLightTravelTime;
	var bIterate = true;
	double x = 0;
	double y = 0;
	double z = 0;

    double l = 0;
    double lrad =0;
    double b = 0;
    double brad =0;
    double r = 0;

	while (bIterate)
	{
	  //Calculate the position of Jupiter
	  l = CAAJupiter.EclipticLongitude(JD1);
	  lrad = CAACoordinateTransformation.DegreesToRadians(l);
	  b = CAAJupiter.EclipticLatitude(JD1);
	  brad = CAACoordinateTransformation.DegreesToRadians(b);
	  r = CAAJupiter.RadiusVector(JD1);
  
	  x = r *Math.Cos(brad)*Math.Cos(lrad) + R *Math.Cos(sunlongrad);
	  y = r *Math.Cos(brad)*Math.Sin(lrad) + R *Math.Sin(sunlongrad);
	  z = r *Math.Sin(brad) + R *Math.Sin(betarad);
	  DELTA = Math.Sqrt(x *x + y *y + z *z);
	  EarthLightTravelTime = CAAElliptical.DistanceToLightTime(DELTA);
  
	  //Prepare for the next loop around
	  bIterate = (Math.Abs(EarthLightTravelTime - PreviousEarthLightTravelTime) > 2E-6); //2E-6 corresponds to 0.17 of a second
	  if (bIterate)
	  {
		JD1 = JD - EarthLightTravelTime;
		PreviousEarthLightTravelTime = EarthLightTravelTime;
	  }
	}
  
	//Calculate the details as seen from the earth
	var details1 = CalculateHelper(JD, sunlongrad, betarad, R);
	FillInPhenomenaDetails(ref details1.Satellite1);
	FillInPhenomenaDetails(ref details1.Satellite2);
	FillInPhenomenaDetails(ref details1.Satellite3);
	FillInPhenomenaDetails(ref details1.Satellite4);
  
	//Calculate the the light travel time from Jupiter to the Sun
	JD1 = JD - EarthLightTravelTime;
	l = CAAJupiter.EclipticLongitude(JD1);
	lrad = CAACoordinateTransformation.DegreesToRadians(l);
	b = CAAJupiter.EclipticLatitude(JD1);
	brad = CAACoordinateTransformation.DegreesToRadians(b);
	r = CAAJupiter.RadiusVector(JD1);
	x = r *Math.Cos(brad)*Math.Cos(lrad);
	y = r *Math.Cos(brad)*Math.Sin(lrad);
	z = r *Math.Sin(brad);
	DELTA = Math.Sqrt(x *x + y *y + z *z);
	var SunLightTravelTime = CAAElliptical.DistanceToLightTime(DELTA);
  
	//Calculate the details as seen from the Sun
	var details2 = CalculateHelper(JD + SunLightTravelTime - EarthLightTravelTime, sunlongrad, betarad, 0);
	FillInPhenomenaDetails(ref details2.Satellite1);
	FillInPhenomenaDetails(ref details2.Satellite2);
	FillInPhenomenaDetails(ref details2.Satellite3);
	FillInPhenomenaDetails(ref details2.Satellite4);
  
	//Finally transfer the required values from details2 to details1
	details1.Satellite1.bInEclipse = details2.Satellite1.bInOccultation;
	details1.Satellite2.bInEclipse = details2.Satellite2.bInOccultation;
	details1.Satellite3.bInEclipse = details2.Satellite3.bInOccultation;
	details1.Satellite4.bInEclipse = details2.Satellite4.bInOccultation;
	details1.Satellite1.bInShadowTransit = details2.Satellite1.bInTransit;
	details1.Satellite2.bInShadowTransit = details2.Satellite2.bInTransit;
	details1.Satellite3.bInShadowTransit = details2.Satellite3.bInTransit;
	details1.Satellite4.bInShadowTransit = details2.Satellite4.bInTransit;
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
//ORIGINAL LINE: details1.Satellite1.ApparentShadowRectangularCoordinates = details2.Satellite1.ApparentRectangularCoordinates;
	details1.Satellite1.ApparentShadowRectangularCoordinates = details2.Satellite1.ApparentRectangularCoordinates;
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
//ORIGINAL LINE: details1.Satellite2.ApparentShadowRectangularCoordinates = details2.Satellite2.ApparentRectangularCoordinates;
	details1.Satellite2.ApparentShadowRectangularCoordinates = details2.Satellite2.ApparentRectangularCoordinates;
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
//ORIGINAL LINE: details1.Satellite3.ApparentShadowRectangularCoordinates = details2.Satellite3.ApparentRectangularCoordinates;
	details1.Satellite3.ApparentShadowRectangularCoordinates = details2.Satellite3.ApparentRectangularCoordinates;
//C++ TO C# CONVERTER WARNING: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created if it does not yet exist:
//ORIGINAL LINE: details1.Satellite4.ApparentShadowRectangularCoordinates = details2.Satellite4.ApparentRectangularCoordinates;
	details1.Satellite4.ApparentShadowRectangularCoordinates = details2.Satellite4.ApparentRectangularCoordinates;
	return details1;
  }


  //////////////////////////////// Implementation ///////////////////////////////
  
  protected static CAAGalileanMoonsDetails CalculateHelper(double JD, double sunlongrad, double betarad, double R)
  {
	//What will be the return value
	var details = new CAAGalileanMoonsDetails();
  
	//Calculate the position of Jupiter decreased by the light travel time from Jupiter to the specified position
	double DELTA = 5;
	double PreviousLightTravelTime = 0;
	var LightTravelTime = CAAElliptical.DistanceToLightTime(DELTA);
	double x = 0;
	double y = 0;
	double z = 0;
      var JD1 = JD - LightTravelTime;
	var bIterate = true;
	while (bIterate)
	{
	  //Calculate the position of Jupiter
	  double l = CAAJupiter.EclipticLongitude(JD1);
	  double lrad = CAACoordinateTransformation.DegreesToRadians(l);
	  double b = CAAJupiter.EclipticLatitude(JD1);
	  double brad = CAACoordinateTransformation.DegreesToRadians(b);
	  double r = CAAJupiter.RadiusVector(JD1);
  
	  x = r *Math.Cos(brad)*Math.Cos(lrad) + R *Math.Cos(sunlongrad);
	  y = r *Math.Cos(brad)*Math.Sin(lrad) + R *Math.Sin(sunlongrad);
	  z = r *Math.Sin(brad) + R *Math.Sin(betarad);
	  DELTA = Math.Sqrt(x *x + y *y + z *z);
	  LightTravelTime = CAAElliptical.DistanceToLightTime(DELTA);
  
	  //Prepare for the next loop around
	  bIterate = (Math.Abs(LightTravelTime - PreviousLightTravelTime) > 2E-6); //2E-6 corresponds to 0.17 of a second
	  if (bIterate)
	  {
		JD1 = JD - LightTravelTime;
		PreviousLightTravelTime = LightTravelTime;
	  }
	}
  
	//Calculate Jupiter's Longitude and Latitude
	var lambda0 = Math.Atan2(y, x);
	var beta0 = Math.Atan(z/Math.Sqrt(x *x + y *y));
  
	var t = JD - 2443000.5 - LightTravelTime;
  
	//Calculate the mean longitudes
	var l1 = 106.07719 + 203.488955790 *t;
	var l1rad = CAACoordinateTransformation.DegreesToRadians(l1);
	var l2 = 175.73161 + 101.374724735 *t;
	var l2rad = CAACoordinateTransformation.DegreesToRadians(l2);
	var l3 = 120.55883 + 50.317609207 *t;
	var l3rad = CAACoordinateTransformation.DegreesToRadians(l3);
	var l4 = 84.44459 + 21.571071177 *t;
	var l4rad = CAACoordinateTransformation.DegreesToRadians(l4);
  
	//Calculate the perijoves
	var pi1 = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.MapTo0To360Range(97.0881 + 0.16138586 *t));
	var pi2 = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.MapTo0To360Range(154.8663 + 0.04726307 *t));
	var pi3 = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.MapTo0To360Range(188.1840 + 0.00712734 *t));
	var pi4 = CAACoordinateTransformation.DegreesToRadians(CAACoordinateTransformation.MapTo0To360Range(335.2868 + 0.00184000 *t));
  
	//Calculate the nodes on the equatorial plane of jupiter
	var w1 = 312.3346 - 0.13279386 *t;
	var w1rad = CAACoordinateTransformation.DegreesToRadians(w1);
	var w2 = 100.4411 - 0.03263064 *t;
	var w2rad = CAACoordinateTransformation.DegreesToRadians(w2);
	var w3 = 119.1942 - 0.00717703 *t;
	var w3rad = CAACoordinateTransformation.DegreesToRadians(w3);
	var w4 = 322.6186 - 0.00175934 *t;
	var w4rad = CAACoordinateTransformation.DegreesToRadians(w4);
  
	//Calculate the Principal inequality in the longitude of Jupiter
	var GAMMA = 0.33033 *Math.Sin(CAACoordinateTransformation.DegreesToRadians(163.679 + 0.0010512 *t)) + 0.03439 *Math.Sin(CAACoordinateTransformation.DegreesToRadians(34.486 - 0.0161731 *t));
  
	//Calculate the "phase of free libration"
	var philambda = CAACoordinateTransformation.DegreesToRadians(199.6766 + 0.17379190 *t);
  
	//Calculate the longitude of the node of the equator of Jupiter on the ecliptic
	var psi = CAACoordinateTransformation.DegreesToRadians(316.5182 - 0.00000208 *t);
  
	//Calculate the mean anomalies of Jupiter and Saturn
	var G = CAACoordinateTransformation.DegreesToRadians(30.23756 + 0.0830925701 *t + GAMMA);
	var Gdash = CAACoordinateTransformation.DegreesToRadians(31.97853 + 0.0334597339 *t);
  
	//Calculate the longitude of the perihelion of Jupiter
	var PI = CAACoordinateTransformation.DegreesToRadians(13.469942);
  
	//Calculate the periodic terms in the longitudes of the satellites
	var Sigma1 = 0.47259 *Math.Sin(2*(l1rad - l2rad)) + -0.03478 *Math.Sin(pi3 - pi4) + 0.01081 *Math.Sin(l2rad - 2 *l3rad + pi3) + 0.00738 *Math.Sin(philambda) + 0.00713 *Math.Sin(l2rad - 2 *l3rad + pi2) + -0.00674 *Math.Sin(pi1 + pi3 - 2 *PI - 2 *G) + 0.00666 *Math.Sin(l2rad - 2 *l3rad + pi4) + 0.00445 *Math.Sin(l1rad - pi3) + -0.00354 *Math.Sin(l1rad - l2rad) + -0.00317 *Math.Sin(2 *psi - 2 *PI) + 0.00265 *Math.Sin(l1rad - pi4) + -0.00186 *Math.Sin(G) + 0.00162 *Math.Sin(pi2 - pi3) + 0.00158 *Math.Sin(4*(l1rad - l2rad)) + -0.00155 *Math.Sin(l1rad - l3rad) + -0.00138 *Math.Sin(psi + w3rad - 2 *PI - 2 *G) + -0.00115 *Math.Sin(2*(l1rad - 2 *l2rad + w2rad)) + 0.00089 *Math.Sin(pi2 - pi4) + 0.00085 *Math.Sin(l1rad + pi3 - 2 *PI - 2 *G) + 0.00083 *Math.Sin(w2rad - w3rad) + 0.00053 *Math.Sin(psi - w2rad);
  
	var Sigma2 = 1.06476 *Math.Sin(2*(l2rad - l3rad)) + 0.04256 *Math.Sin(l1rad - 2 *l2rad + pi3) + 0.03581 *Math.Sin(l2rad - pi3) + 0.02395 *Math.Sin(l1rad - 2 *l2rad + pi4) + 0.01984 *Math.Sin(l2rad - pi4) + -0.01778 *Math.Sin(philambda) + 0.01654 *Math.Sin(l2rad - pi2) + 0.01334 *Math.Sin(l2rad - 2 *l3rad + pi2) + 0.01294 *Math.Sin(pi3 - pi4) + -0.01142 *Math.Sin(l2rad - l3rad) + -0.01057 *Math.Sin(G) + -0.00775 *Math.Sin(2*(psi - PI)) + 0.00524 *Math.Sin(2*(l1rad - l2rad)) + -0.00460 *Math.Sin(l1rad - l3rad) + 0.00316 *Math.Sin(psi - 2 *G + w3rad - 2 *PI) + -0.00203 *Math.Sin(pi1 + pi3 - 2 *PI - 2 *G) + 0.00146 *Math.Sin(psi - w3rad) + -0.00145 *Math.Sin(2 *G) + 0.00125 *Math.Sin(psi - w4rad) + -0.00115 *Math.Sin(l1rad - 2 *l3rad + pi3) + -0.00094 *Math.Sin(2*(l2rad - w2rad)) + 0.00086 *Math.Sin(2*(l1rad - 2 *l2rad + w2rad)) + -0.00086 *Math.Sin(5 *Gdash - 2 *G + CAACoordinateTransformation.DegreesToRadians(52.225)) + -0.00078 *Math.Sin(l2rad - l4rad) + -0.00064 *Math.Sin(3 *l3rad - 7 *l4rad + 4 *pi4) + 0.00064 *Math.Sin(pi1 - pi4) + -0.00063 *Math.Sin(l1rad - 2 *l3rad + pi4) + 0.00058 *Math.Sin(w3rad - w4rad) + 0.00056 *Math.Sin(2*(psi - PI - G)) + 0.00056 *Math.Sin(2*(l2rad - l4rad)) + 0.00055 *Math.Sin(2*(l1rad - l3rad)) + 0.00052 *Math.Sin(3 *l3rad - 7 *l4rad + pi3 + 3 *pi4) + -0.00043 *Math.Sin(l1rad - pi3) + 0.00041 *Math.Sin(5*(l2rad - l3rad)) + 0.00041 *Math.Sin(pi4 - PI) + 0.00032 *Math.Sin(w2rad - w3rad) + 0.00032 *Math.Sin(2*(l3rad - G - PI));
  
	var Sigma3 = 0.16490 *Math.Sin(l3rad - pi3) + 0.09081 *Math.Sin(l3rad - pi4) + -0.06907 *Math.Sin(l2rad - l3rad) + 0.03784 *Math.Sin(pi3 - pi4) + 0.01846 *Math.Sin(2*(l3rad - l4rad)) + -0.01340 *Math.Sin(G) + -0.01014 *Math.Sin(2*(psi - PI)) + 0.00704 *Math.Sin(l2rad - 2 *l3rad + pi3) + -0.00620 *Math.Sin(l2rad - 2 *l3rad + pi2) + -0.00541 *Math.Sin(l3rad - l4rad) + 0.00381 *Math.Sin(l2rad - 2 *l3rad + pi4) + 0.00235 *Math.Sin(psi - w3rad) + 0.00198 *Math.Sin(psi - w4rad) + 0.00176 *Math.Sin(philambda) + 0.00130 *Math.Sin(3*(l3rad - l4rad)) + 0.00125 *Math.Sin(l1rad - l3rad) + -0.00119 *Math.Sin(5 *Gdash - 2 *G + CAACoordinateTransformation.DegreesToRadians(52.225)) + 0.00109 *Math.Sin(l1rad - l2rad) + -0.00100 *Math.Sin(3 *l3rad - 7 *l4rad + 4 *pi4) + 0.00091 *Math.Sin(w3rad - w4rad) + 0.00080 *Math.Sin(3 *l3rad - 7 *l4rad + pi3 + 3 *pi4) + -0.00075 *Math.Sin(2 *l2rad - 3 *l3rad + pi3) + 0.00072 *Math.Sin(pi1 + pi3 - 2 *PI - 2 *G) + 0.00069 *Math.Sin(pi4 - PI) + -0.00058 *Math.Sin(2 *l3rad - 3 *l4rad + pi4) + -0.00057 *Math.Sin(l3rad - 2 *l4rad + pi4) + 0.00056 *Math.Sin(l3rad + pi3 - 2 *PI - 2 *G) + -0.00052 *Math.Sin(l2rad - 2 *l3rad + pi1) + -0.00050 *Math.Sin(pi2 - pi3) + 0.00048 *Math.Sin(l3rad - 2 *l4rad + pi3) + -0.00045 *Math.Sin(2 *l2rad - 3 *l3rad + pi4) + -0.00041 *Math.Sin(pi2 - pi4) + -0.00038 *Math.Sin(2 *G) + -0.00037 *Math.Sin(pi3 - pi4 + w3rad - w4rad) + -0.00032 *Math.Sin(3 *l3rad - 7 *l4rad + 2 *pi3 + 2 *pi4) + 0.00030 *Math.Sin(4*(l3rad - l4rad)) + 0.00029 *Math.Sin(l3rad + pi4 - 2 *PI - 2 *G) + -0.00028 *Math.Sin(w3rad + psi - 2 *PI - 2 *G) + 0.00026 *Math.Sin(l3rad - PI - G) + 0.00024 *Math.Sin(l2rad - 3 *l3rad + 2 *l4rad) + 0.00021 *Math.Sin(l3rad - PI - G) + -0.00021 *Math.Sin(l3rad - pi2) + 0.00017 *Math.Sin(2*(l3rad - pi3));
  
	var Sigma4 = 0.84287 *Math.Sin(l4rad - pi4) + 0.03431 *Math.Sin(pi4 - pi3) + -0.03305 *Math.Sin(2*(psi - PI)) + -0.03211 *Math.Sin(G) + -0.01862 *Math.Sin(l4rad - pi3) + 0.01186 *Math.Sin(psi - w4rad) + 0.00623 *Math.Sin(l4rad + pi4 - 2 *G - 2 *PI) + 0.00387 *Math.Sin(2*(l4rad - pi4)) + -0.00284 *Math.Sin(5 *Gdash - 2 *G + CAACoordinateTransformation.DegreesToRadians(52.225)) + -0.00234 *Math.Sin(2*(psi - pi4)) + -0.00223 *Math.Sin(l3rad - l4rad) + -0.00208 *Math.Sin(l4rad - PI) + 0.00178 *Math.Sin(psi + w4rad - 2 *pi4) + 0.00134 *Math.Sin(pi4 - PI) + 0.00125 *Math.Sin(2*(l4rad - G - PI)) + -0.00117 *Math.Sin(2 *G) + -0.00112 *Math.Sin(2*(l3rad - l4rad)) + 0.00107 *Math.Sin(3 *l3rad - 7 *l4rad + 4 *pi4) + 0.00102 *Math.Sin(l4rad - G - PI) + 0.00096 *Math.Sin(2 *l4rad - psi - w4rad) + 0.00087 *Math.Sin(2*(psi - w4rad)) + -0.00085 *Math.Sin(3 *l3rad - 7 *l4rad + pi3 + 3 *pi4) + 0.00085 *Math.Sin(l3rad - 2 *l4rad + pi4) + -0.00081 *Math.Sin(2*(l4rad - psi)) + 0.00071 *Math.Sin(l4rad + pi4 - 2 *PI - 3 *G) + 0.00061 *Math.Sin(l1rad - l4rad) + -0.00056 *Math.Sin(psi - w3rad) + -0.00054 *Math.Sin(l3rad - 2 *l4rad + pi3) + 0.00051 *Math.Sin(l2rad - l4rad) + 0.00042 *Math.Sin(2*(psi - G - PI)) + 0.00039 *Math.Sin(2*(pi4 - w4rad)) + 0.00036 *Math.Sin(psi + PI - pi4 - w4rad) + 0.00035 *Math.Sin(2 *Gdash - G + CAACoordinateTransformation.DegreesToRadians(188.37)) + -0.00035 *Math.Sin(l4rad - pi4 + 2 *PI - 2 *psi) + -0.00032 *Math.Sin(l4rad + pi4 - 2 *PI - G) + 0.00030 *Math.Sin(2 *Gdash - 2 *G + CAACoordinateTransformation.DegreesToRadians(149.15)) + 0.00029 *Math.Sin(3 *l3rad - 7 *l4rad + 2 *pi3 + 2 *pi4) + 0.00028 *Math.Sin(l4rad - pi4 + 2 *psi - 2 *PI) + -0.00028 *Math.Sin(2*(l4rad - w4rad)) + -0.00027 *Math.Sin(pi3 - pi4 + w3rad - w4rad) + -0.00026 *Math.Sin(5 *Gdash - 3 *G + CAACoordinateTransformation.DegreesToRadians(188.37)) + 0.00025 *Math.Sin(w4rad - w3rad) + -0.00025 *Math.Sin(l2rad - 3 *l3rad + 2 *l4rad) + -0.00023 *Math.Sin(3*(l3rad - l4rad)) + 0.00021 *Math.Sin(2 *l4rad - 2 *PI - 3 *G) + -0.00021 *Math.Sin(2 *l3rad - 3 *l4rad + pi4) + 0.00019 *Math.Sin(l4rad - pi4 - G) + -0.00019 *Math.Sin(2 *l4rad - pi3 - pi4) + -0.00018 *Math.Sin(l4rad - pi4 + G) + -0.00016 *Math.Sin(l4rad + pi3 - 2 *PI - 2 *G);
  
	details.Satellite1.MeanLongitude = CAACoordinateTransformation.MapTo0To360Range(l1);
	details.Satellite1.TrueLongitude = CAACoordinateTransformation.MapTo0To360Range(l1 + Sigma1);
	var L1 = CAACoordinateTransformation.DegreesToRadians(details.Satellite1.TrueLongitude);
  
	details.Satellite2.MeanLongitude = CAACoordinateTransformation.MapTo0To360Range(l2);
	details.Satellite2.TrueLongitude = CAACoordinateTransformation.MapTo0To360Range(l2 + Sigma2);
	var L2 = CAACoordinateTransformation.DegreesToRadians(details.Satellite2.TrueLongitude);
  
	details.Satellite3.MeanLongitude = CAACoordinateTransformation.MapTo0To360Range(l3);
	details.Satellite3.TrueLongitude = CAACoordinateTransformation.MapTo0To360Range(l3 + Sigma3);
	var L3 = CAACoordinateTransformation.DegreesToRadians(details.Satellite3.TrueLongitude);
  
	details.Satellite4.MeanLongitude = CAACoordinateTransformation.MapTo0To360Range(l4);
	details.Satellite4.TrueLongitude = CAACoordinateTransformation.MapTo0To360Range(l4 + Sigma4);
	var L4 = CAACoordinateTransformation.DegreesToRadians(details.Satellite4.TrueLongitude);
  
	//Calculate the periodic terms in the latitudes of the satellites
	var B1 = Math.Atan(0.0006393 *Math.Sin(L1 - w1rad) + 0.0001825 *Math.Sin(L1 - w2rad) + 0.0000329 *Math.Sin(L1 - w3rad) + -0.0000311 *Math.Sin(L1 - psi) + 0.0000093 *Math.Sin(L1 - w4rad) + 0.0000075 *Math.Sin(3 *L1 - 4 *l2rad - 1.9927 *Sigma1 + w2rad) + 0.0000046 *Math.Sin(L1 + psi - 2 *PI - 2 *G));
	details.Satellite1.EquatorialLatitude = CAACoordinateTransformation.RadiansToDegrees(B1);
  
	var B2 = Math.Atan(0.0081004 *Math.Sin(L2 - w2rad) + 0.0004512 *Math.Sin(L2 - w3rad) + -0.0003284 *Math.Sin(L2 - psi) + 0.0001160 *Math.Sin(L2 - w4rad) + 0.0000272 *Math.Sin(l1rad - 2 *l3rad + 1.0146 *Sigma2 + w2rad) + -0.0000144 *Math.Sin(L2 - w1rad) + 0.0000143 *Math.Sin(L2 + psi - 2 *PI - 2 *G) + 0.0000035 *Math.Sin(L2 - psi + G) + -0.0000028 *Math.Sin(l1rad - 2 *l3rad + 1.0146 *Sigma2 + w3rad));
	details.Satellite2.EquatorialLatitude = CAACoordinateTransformation.RadiansToDegrees(B2);
  
	var B3 = Math.Atan(0.0032402 *Math.Sin(L3 - w3rad) + -0.0016911 *Math.Sin(L3 - psi) + 0.0006847 *Math.Sin(L3 - w4rad) + -0.0002797 *Math.Sin(L3 - w2rad) + 0.0000321 *Math.Sin(L3 + psi - 2 *PI - 2 *G) + 0.0000051 *Math.Sin(L3 - psi + G) + -0.0000045 *Math.Sin(L3 - psi - G) + -0.0000045 *Math.Sin(L3 + psi - 2 *PI) + 0.0000037 *Math.Sin(L3 + psi - 2 *PI - 3 *G) + 0.0000030 *Math.Sin(2 *l2rad - 3 *L3 + 4.03 *Sigma3 + w2rad) + -0.0000021 *Math.Sin(2 *l2rad - 3 *L3 + 4.03 *Sigma3 + w3rad));
	details.Satellite3.EquatorialLatitude = CAACoordinateTransformation.RadiansToDegrees(B3);
  
	var B4 = Math.Atan(-0.0076579 *Math.Sin(L4 - psi) + 0.0044134 *Math.Sin(L4 - w4rad) + -0.0005112 *Math.Sin(L4 - w3rad) + 0.0000773 *Math.Sin(L4 + psi - 2 *PI - 2 *G) + 0.0000104 *Math.Sin(L4 - psi + G) + -0.0000102 *Math.Sin(L4 - psi - G) + 0.0000088 *Math.Sin(L4 + psi - 2 *PI - 3 *G) + -0.0000038 *Math.Sin(L4 + psi - 2 *PI - G));
	details.Satellite4.EquatorialLatitude = CAACoordinateTransformation.RadiansToDegrees(B4);
  
	//Calculate the periodic terms for the radius vector
	details.Satellite1.r = 5.90569 * (1 + (-0.0041339 *Math.Cos(2*(l1rad - l2rad)) + -0.0000387 *Math.Cos(l1rad - pi3) + -0.0000214 *Math.Cos(l1rad - pi4) + 0.0000170 *Math.Cos(l1rad - l2rad) + -0.0000131 *Math.Cos(4*(l1rad - l2rad)) + 0.0000106 *Math.Cos(l1rad - l3rad) + -0.0000066 *Math.Cos(l1rad + pi3 - 2 *PI - 2 *G)));
  
	details.Satellite2.r = 9.39657 * (1 + (0.0093848 *Math.Cos(l1rad - l2rad) + -0.0003116 *Math.Cos(l2rad - pi3) + -0.0001744 *Math.Cos(l2rad - pi4) + -0.0001442 *Math.Cos(l2rad - pi2) + 0.0000553 *Math.Cos(l2rad - l3rad) + 0.0000523 *Math.Cos(l1rad - l3rad) + -0.0000290 *Math.Cos(2*(l1rad - l2rad)) + 0.0000164 *Math.Cos(2*(l2rad - w2rad)) + 0.0000107 *Math.Cos(l1rad - 2 *l3rad + pi3) + -0.0000102 *Math.Cos(l2rad - pi1) + -0.0000091 *Math.Cos(2*(l1rad - l3rad))));
  
	details.Satellite3.r = 14.98832 * (1 + (-0.0014388 *Math.Cos(l3rad - pi3) + -0.0007919 *Math.Cos(l3rad - pi4) + 0.0006342 *Math.Cos(l2rad - l3rad) + -0.0001761 *Math.Cos(2*(l3rad - l4rad)) + 0.0000294 *Math.Cos(l3rad - l4rad) + -0.0000156 *Math.Cos(3*(l3rad - l4rad)) + 0.0000156 *Math.Cos(l1rad - l3rad) + -0.0000153 *Math.Cos(l1rad - l2rad) + 0.0000070 *Math.Cos(2 *l2rad - 3 *l3rad + pi3) + -0.0000051 *Math.Cos(l3rad + pi3 - 2 *PI - 2 *G)));
  
	details.Satellite4.r = 26.36273 * (1 + (-0.0073546 *Math.Cos(l4rad - pi4) + 0.0001621 *Math.Cos(l4rad - pi3) + 0.0000974 *Math.Cos(l3rad - l4rad) + -0.0000543 *Math.Cos(l4rad + pi4 - 2 *PI - 2 *G) + -0.0000271 *Math.Cos(2*(l4rad - pi4)) + 0.0000182 *Math.Cos(l4rad - PI) + 0.0000177 *Math.Cos(2*(l3rad - l4rad)) + -0.0000167 *Math.Cos(2 *l4rad - psi - w4rad) + 0.0000167 *Math.Cos(psi - w4rad) + -0.0000155 *Math.Cos(2*(l4rad - PI - G)) + 0.0000142 *Math.Cos(2*(l4rad - psi)) + 0.0000105 *Math.Cos(l1rad - l4rad) + 0.0000092 *Math.Cos(l2rad - l4rad) + -0.0000089 *Math.Cos(l4rad - PI - G) + -0.0000062 *Math.Cos(l4rad + pi4 - 2 *PI - 3 *G) + 0.0000048 *Math.Cos(2*(l4rad - w4rad))));
  
  
  
	//Calculate T0
	var T0 = (JD - 2433282.423)/36525;
  
	//Calculate the precession in longitude from Epoch B1950 to the date
	var P = CAACoordinateTransformation.DegreesToRadians(1.3966626 *T0 + 0.0003088 *T0 *T0);
  
	//Add it to L1 - L4 and psi
	L1 += P;
	details.Satellite1.TropicalLongitude = CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(L1));
	L2 += P;
	details.Satellite2.TropicalLongitude = CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(L2));
	L3 += P;
	details.Satellite3.TropicalLongitude = CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(L3));
	L4 += P;
	details.Satellite4.TropicalLongitude = CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(L4));
	psi += P;
  
	//Calculate the inclination of Jupiter's axis of rotation on the orbital plane
	var T = (JD - 2415020.5)/36525;
	var I = 3.120262 + 0.0006 *T;
	var Irad = CAACoordinateTransformation.DegreesToRadians(I);
  
	var X1 = details.Satellite1.r *Math.Cos(L1 - psi)*Math.Cos(B1);
	var X2 = details.Satellite2.r *Math.Cos(L2 - psi)*Math.Cos(B2);
	var X3 = details.Satellite3.r *Math.Cos(L3 - psi)*Math.Cos(B3);
	var X4 = details.Satellite4.r *Math.Cos(L4 - psi)*Math.Cos(B4);
	const double X5 = 0;
  
	var Y1 = details.Satellite1.r *Math.Sin(L1 - psi)*Math.Cos(B1);
	var Y2 = details.Satellite2.r *Math.Sin(L2 - psi)*Math.Cos(B2);
	var Y3 = details.Satellite3.r *Math.Sin(L3 - psi)*Math.Cos(B3);
	var Y4 = details.Satellite4.r *Math.Sin(L4 - psi)*Math.Cos(B4);
	const double Y5 = 0;
  
	var Z1 = details.Satellite1.r *Math.Sin(B1);
	var Z2 = details.Satellite2.r *Math.Sin(B2);
	var Z3 = details.Satellite3.r *Math.Sin(B3);
	var Z4 = details.Satellite4.r *Math.Sin(B4);
	const double Z5 = 1;
  
	//Now do the rotations, first for the ficticious 5th satellite, so that we can calculate D
	var omega = CAACoordinateTransformation.DegreesToRadians(CAAElementsPlanetaryOrbit.JupiterLongitudeAscendingNode(JD));
	var i = CAACoordinateTransformation.DegreesToRadians(CAAElementsPlanetaryOrbit.JupiterInclination(JD));
	double A6=0;
	double B6=0;
	double C6=0;
    var north = new CAA3DCoordinate();
	Rotations(X5, Y5, Z5, Irad, psi, i, omega, lambda0, beta0, ref A6, ref B6, ref C6, out north);
	var D = Math.Atan2(A6, C6);
  
	//Now calculate the values for satellite 1
	Rotations(X1, Y1, Z1, Irad, psi, i, omega, lambda0, beta0, ref A6, ref B6, ref C6, out details.Satellite1.EclipticRectangularCoordinates);
	details.Satellite1.TrueRectangularCoordinates.X = A6 *Math.Cos(D) - C6 *Math.Sin(D);
	details.Satellite1.TrueRectangularCoordinates.Y = A6 *Math.Sin(D) + C6 *Math.Cos(D);
	details.Satellite1.TrueRectangularCoordinates.Z = B6;
  
	//Now calculate the values for satellite 2
    Rotations(X2, Y2, Z2, Irad, psi, i, omega, lambda0, beta0, ref  A6, ref B6, ref  C6, out details.Satellite2.EclipticRectangularCoordinates);
	details.Satellite2.TrueRectangularCoordinates.X = A6 *Math.Cos(D) - C6 *Math.Sin(D);
	details.Satellite2.TrueRectangularCoordinates.Y = A6 *Math.Sin(D) + C6 *Math.Cos(D);
	details.Satellite2.TrueRectangularCoordinates.Z = B6;
  
	//Now calculate the values for satellite 3
    Rotations(X3, Y3, Z3, Irad, psi, i, omega, lambda0, beta0, ref A6, ref B6, ref C6, out details.Satellite3.EclipticRectangularCoordinates);
	details.Satellite3.TrueRectangularCoordinates.X = A6 *Math.Cos(D) - C6 *Math.Sin(D);
	details.Satellite3.TrueRectangularCoordinates.Y = A6 *Math.Sin(D) + C6 *Math.Cos(D);
	details.Satellite3.TrueRectangularCoordinates.Z = B6;
  
	//And finally for satellite 4
    Rotations(X4, Y4, Z4, Irad, psi, i, omega, lambda0, beta0, ref A6, ref B6, ref C6, out details.Satellite4.EclipticRectangularCoordinates);
	details.Satellite4.TrueRectangularCoordinates.X = A6 *Math.Cos(D) - C6 *Math.Sin(D);
	details.Satellite4.TrueRectangularCoordinates.Y = A6 *Math.Sin(D) + C6 *Math.Cos(D);
	details.Satellite4.TrueRectangularCoordinates.Z = B6;
  
	//apply the differential light-time correction
	details.Satellite1.ApparentRectangularCoordinates.X = details.Satellite1.TrueRectangularCoordinates.X + Math.Abs(details.Satellite1.TrueRectangularCoordinates.Z)/17295 *Math.Sqrt(1 - (details.Satellite1.TrueRectangularCoordinates.X/details.Satellite1.r)*(details.Satellite1.TrueRectangularCoordinates.X/details.Satellite1.r));
	details.Satellite1.ApparentRectangularCoordinates.Y = details.Satellite1.TrueRectangularCoordinates.Y;
	details.Satellite1.ApparentRectangularCoordinates.Z = details.Satellite1.TrueRectangularCoordinates.Z;
  
	details.Satellite2.ApparentRectangularCoordinates.X = details.Satellite2.TrueRectangularCoordinates.X + Math.Abs(details.Satellite2.TrueRectangularCoordinates.Z)/21819 *Math.Sqrt(1 - (details.Satellite2.TrueRectangularCoordinates.X/details.Satellite2.r)*(details.Satellite2.TrueRectangularCoordinates.X/details.Satellite2.r));
	details.Satellite2.ApparentRectangularCoordinates.Y = details.Satellite2.TrueRectangularCoordinates.Y;
	details.Satellite2.ApparentRectangularCoordinates.Z = details.Satellite2.TrueRectangularCoordinates.Z;
  
	details.Satellite3.ApparentRectangularCoordinates.X = details.Satellite3.TrueRectangularCoordinates.X + Math.Abs(details.Satellite3.TrueRectangularCoordinates.Z)/27558 *Math.Sqrt(1 - (details.Satellite3.TrueRectangularCoordinates.X/details.Satellite3.r)*(details.Satellite3.TrueRectangularCoordinates.X/details.Satellite3.r));
	details.Satellite3.ApparentRectangularCoordinates.Y = details.Satellite3.TrueRectangularCoordinates.Y;
	details.Satellite3.ApparentRectangularCoordinates.Z = details.Satellite3.TrueRectangularCoordinates.Z;
  
	details.Satellite4.ApparentRectangularCoordinates.X = details.Satellite4.TrueRectangularCoordinates.X + Math.Abs(details.Satellite4.TrueRectangularCoordinates.Z)/36548 *Math.Sqrt(1 - (details.Satellite4.TrueRectangularCoordinates.X/details.Satellite4.r)*(details.Satellite4.TrueRectangularCoordinates.X/details.Satellite4.r));
	details.Satellite4.ApparentRectangularCoordinates.Y = details.Satellite4.TrueRectangularCoordinates.Y;
	details.Satellite4.ApparentRectangularCoordinates.Z = details.Satellite4.TrueRectangularCoordinates.Z;
  
	//apply the perspective effect correction
	var W = DELTA/(DELTA + details.Satellite1.TrueRectangularCoordinates.Z/2095);
	details.Satellite1.ApparentRectangularCoordinates.X *= W;
	details.Satellite1.ApparentRectangularCoordinates.Y *= W;
  
	W = DELTA/(DELTA + details.Satellite2.TrueRectangularCoordinates.Z/2095);
	details.Satellite2.ApparentRectangularCoordinates.X *= W;
	details.Satellite2.ApparentRectangularCoordinates.Y *= W;
  
	W = DELTA/(DELTA + details.Satellite3.TrueRectangularCoordinates.Z/2095);
	details.Satellite3.ApparentRectangularCoordinates.X *= W;
	details.Satellite3.ApparentRectangularCoordinates.Y *= W;
  
	W = DELTA/(DELTA + details.Satellite4.TrueRectangularCoordinates.Z/2095);
	details.Satellite4.ApparentRectangularCoordinates.X *= W;
	details.Satellite4.ApparentRectangularCoordinates.Y *= W;
  
	return details;
  }
  protected static void Rotations(double X, double Y, double Z, double I, double psi, double i, double omega, double lambda0, double beta0, ref double A6, ref double B6, ref double C6, out CAA3DCoordinate eclipticCoord)
  {
	var phi = psi - omega;
  
	//Rotation towards Jupiter's orbital plane
	var A1 = X;
	var B1 = Y *Math.Cos(I) - Z *Math.Sin(I);
	var C1 = Y *Math.Sin(I) + Z *Math.Cos(I);
  
	//Rotation towards the ascending node of the orbit of jupiter
	var A2 = A1 *Math.Cos(phi) - B1 *Math.Sin(phi);
	var B2 = A1 *Math.Sin(phi) + B1 *Math.Cos(phi);
	var C2 = C1;
  
	//Rotation towards the plane of the ecliptic
	var A3 = A2;
	var B3 = B2 *Math.Cos(i) - C2 *Math.Sin(i);
	var C3 = B2 *Math.Sin(i) + C2 *Math.Cos(i);
  
	//Rotation towards the vernal equinox
	var A4 = A3 *Math.Cos(omega) - B3 *Math.Sin(omega);
	var B4 = A3 *Math.Sin(omega) + B3 *Math.Cos(omega);
	var C4 = C3;

    const double JupiterRadiiToAU = 1.0 / 2095.0; // Not exact, but this is the value used elsewhere in the calculation
    eclipticCoord.X = A4 * JupiterRadiiToAU;
    eclipticCoord.Y = B4 * JupiterRadiiToAU;
    eclipticCoord.Z = C4 * JupiterRadiiToAU;

	var A5 = A4 *Math.Sin(lambda0) - B4 *Math.Cos(lambda0);
	var B5 = A4 *Math.Cos(lambda0) + B4 *Math.Sin(lambda0);
	var C5 = C4;
  
	A6 = A5;
	B6 = C5 *Math.Sin(beta0) + B5 *Math.Cos(beta0);
	C6 = C5 *Math.Cos(beta0) - B5 *Math.Sin(beta0);
  }
  protected static void FillInPhenomenaDetails(ref CAAGalileanMoonDetail detail)
  {
	var Y1 = 1.071374 * detail.ApparentRectangularCoordinates.Y;
  
	var r = Y1 *Y1 + detail.ApparentRectangularCoordinates.X *detail.ApparentRectangularCoordinates.X;
  
	if (r < 1)
	{
	  if (detail.ApparentRectangularCoordinates.Z < 0)
	  {
		//Satellite nearer to Earth than Jupiter, so it must be a transit not an occultation
		detail.bInTransit = true;
		detail.bInOccultation = false;
	  }
	  else
	  {
		detail.bInTransit = false;
		detail.bInOccultation = true;
	  }
	}
	else
	{
	  detail.bInTransit = false;
	  detail.bInOccultation = false;
	}
  }
}
