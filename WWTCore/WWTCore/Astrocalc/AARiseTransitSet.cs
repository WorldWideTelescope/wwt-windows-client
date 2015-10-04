using System;
//
//Module : AARISETRANSITSET.CPP
//Purpose: Implementation for the algorithms which obtain the Rise, Transit and Set times
//Created: PJN / 29-12-2003
//History: PJN / 15-10-2004 1. bValid variable is now correctly set in CAARiseTransitSet::Rise if the
//                          objects does actually rise and sets
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

///////////////////////////// Includes ////////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAARiseTransitSetDetails
{
//Constructors / Destructors
  public CAARiseTransitSetDetails()
  {
	  bValid = false;
	  Rise = 0;
	  Transit = 0;
	  Set = 0;
  }

//Member variables
  public bool bValid;
  public double Rise;
  public double Transit;
  public double Set;
}

public class  CAARiseTransitSet
{
//Static methods

  ///////////////////////////// Implementation //////////////////////////////////
  
  public static CAARiseTransitSetDetails Rise(double JD, double Alpha1, double Delta1, double Alpha2, double Delta2, double Alpha3, double Delta3, double Longitude, double Latitude, double h0)
  {
	//What will be the return value
	var details = new CAARiseTransitSetDetails {bValid = false};

      //Calculate the sidereal time
	var theta0 = CAASidereal.ApparentGreenwichSiderealTime(JD);
	theta0 *= 15; //Express it as degrees
  
	//Calculate deltat
	var deltaT = CAADynamicalTime.DeltaT(JD);
  
	//Convert values to radians
	var Delta2Rad = CAACoordinateTransformation.DegreesToRadians(Delta2);
	var LatitudeRad = CAACoordinateTransformation.DegreesToRadians(Latitude);
  
	//Convert the standard latitude to radians
	var h0Rad = CAACoordinateTransformation.DegreesToRadians(h0);
  
	var cosH0 = (Math.Sin(h0Rad) - Math.Sin(LatitudeRad)*Math.Sin(Delta2Rad)) / (Math.Cos(LatitudeRad) * Math.Cos(Delta2Rad));
  
	//Check that the object actually rises
	if ((cosH0 > 1) || (cosH0 < -1))
	  return details;
  
	var H0 = Math.Acos(cosH0);
	H0 = CAACoordinateTransformation.RadiansToDegrees(H0);
  
	var M0 = (Alpha2 *15 + Longitude - theta0) / 360;
	var M1 = M0 - H0/360;
	var M2 = M0 + H0/360;
  
	if (M0 > 1)
	  M0 -= 1;
	else if (M0 < 0)
	  M0 += 1;
  
	if (M1 > 1)
	  M1 -= 1;
	else if (M1 < 0)
	  M1 += 1;
  
	if (M2 > 1)
	  M2 -= 1;
	else if (M2 < 0)
	  M2 += 1;
  
	for (var i =0; i<2; i++)
	{
	  //Calculate the details of rising
  
	  var theta1 = theta0 + 360.985647 *M1;
	  theta1 = CAACoordinateTransformation.MapTo0To360Range(theta1);
  
	  var n = M1 + deltaT/86400;
  
	  var Alpha = CAAInterpolate.Interpolate(n, Alpha1, Alpha2, Alpha3);
	  var Delta = CAAInterpolate.Interpolate(n, Delta1, Delta2, Delta3);
  
	  var H = theta1 - Longitude - Alpha *15;
	  var Horizontal = CAACoordinateTransformation.Equatorial2Horizontal(H/15, Delta, Latitude);
  
	  var DeltaM = (Horizontal.Y - h0) / (360 *Math.Cos(CAACoordinateTransformation.DegreesToRadians(Delta))*Math.Cos(LatitudeRad)*Math.Sin(CAACoordinateTransformation.DegreesToRadians(H)));
	  M1 += DeltaM;
  
  
	  //Calculate the details of transit
  
	  theta1 = theta0 + 360.985647 *M0;
	  theta1 = CAACoordinateTransformation.MapTo0To360Range(theta1);
  
	  n = M0 + deltaT/86400;
  
	  Alpha = CAAInterpolate.Interpolate(n, Alpha1, Alpha2, Alpha3);
  
	  H = theta1 - Longitude - Alpha *15;
  
	  if (H < -180)
	  {
		  H+=360;
	  }
  
	  DeltaM = -H / 360;
	  M0 += DeltaM;
  
  
	  //Calculate the details of setting
  
	  theta1 = theta0 + 360.985647 *M2;
	  theta1 = CAACoordinateTransformation.MapTo0To360Range(theta1);
  
	  n = M2 + deltaT/86400;
  
	  Alpha = CAAInterpolate.Interpolate(n, Alpha1, Alpha2, Alpha3);
	  Delta = CAAInterpolate.Interpolate(n, Delta1, Delta2, Delta3);
  
	  H = theta1 - Longitude - Alpha *15;
	  Horizontal = CAACoordinateTransformation.Equatorial2Horizontal(H/15, Delta, Latitude);
  
	  DeltaM = (Horizontal.Y - h0) / (360 *Math.Cos(CAACoordinateTransformation.DegreesToRadians(Delta))*Math.Cos(LatitudeRad)*Math.Sin(CAACoordinateTransformation.DegreesToRadians(H)));
	  M2 += DeltaM;
	}
  
	//Finally before we exit, convert to hours
	details.bValid = true;
	details.Rise = M1 * 24;
	details.Set = M2 * 24;
	details.Transit = M0 * 24;
  
	return details;
  }
}
