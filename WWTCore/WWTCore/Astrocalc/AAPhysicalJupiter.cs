using System;
//
//Module : AAPHYSICALJUPITER.CPP
//Purpose: Implementation for the algorithms which obtain the physical parameters of the Jupiter
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


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAPhysicalJupiterDetails
{
//Constructors / Destructors
  public CAAPhysicalJupiterDetails()
  {
	  DE = 0;
	  DS = 0;
	  Geometricw1 = 0;
	  Geometricw2 = 0;
	  Apparentw1 = 0;
	  Apparentw2 = 0;
	  P = 0;
  }

//Member variables
  public double DE;
  public double DS;
  public double Geometricw1;
  public double Geometricw2;
  public double Apparentw1;
  public double Apparentw2;
  public double P;
}

public class  CAAPhysicalJupiter
{
//Static methods

  //////////////////////////////// Implementation ///////////////////////////////
  
  public static CAAPhysicalJupiterDetails Calculate(double JD)
  {
	//What will be the return value
	var details = new CAAPhysicalJupiterDetails();
  
	//Step 1
	var d = JD - 2433282.5;
	var T1 = d/36525;
	var alpha0 = 268.00 + 0.1061 *T1;
	var alpha0rad = CAACoordinateTransformation.DegreesToRadians(alpha0);
	var delta0 = 64.50 - 0.0164 *T1;
	var delta0rad = CAACoordinateTransformation.DegreesToRadians(delta0);
  
	//Step 2
	var W1 = CAACoordinateTransformation.MapTo0To360Range(17.710 + 877.90003539 *d);
	var W2 = CAACoordinateTransformation.MapTo0To360Range(16.838 + 870.27003539 *d);
  
	//Step 3
	var l0 = CAAEarth.EclipticLongitude(JD);
	var l0rad = CAACoordinateTransformation.DegreesToRadians(l0);
	var b0 = CAAEarth.EclipticLatitude(JD);
	var b0rad = CAACoordinateTransformation.DegreesToRadians(b0);
	var R = CAAEarth.RadiusVector(JD);
  
	//Step 4
	var l = CAAJupiter.EclipticLongitude(JD);
	var lrad = CAACoordinateTransformation.DegreesToRadians(l);
	var b = CAAJupiter.EclipticLatitude(JD);
	var brad = CAACoordinateTransformation.DegreesToRadians(b);
	var r = CAAJupiter.RadiusVector(JD);
  
	//Step 5
	var x = r *Math.Cos(brad)*Math.Cos(lrad) - R *Math.Cos(l0rad);
	var y = r *Math.Cos(brad)*Math.Sin(lrad) - R *Math.Sin(l0rad);
	var z = r *Math.Sin(brad) - R *Math.Sin(b0rad);
	var DELTA = Math.Sqrt(x *x + y *y + z *z);
  
	//Step 6
	l -= 0.012990 *DELTA/(r *r);
	lrad = CAACoordinateTransformation.DegreesToRadians(l);
  
	//Step 7
	x = r *Math.Cos(brad)*Math.Cos(lrad) - R *Math.Cos(l0rad);
	y = r *Math.Cos(brad)*Math.Sin(lrad) - R *Math.Sin(l0rad);
	z = r *Math.Sin(brad) - R *Math.Sin(b0rad);
	DELTA = Math.Sqrt(x *x + y *y + z *z);
  
	//Step 8
	var e0 = CAANutation.MeanObliquityOfEcliptic(JD);
	var e0rad = CAACoordinateTransformation.DegreesToRadians(e0);
  
	//Step 9
	var alphas = Math.Atan2(Math.Cos(e0rad)*Math.Sin(lrad) - Math.Sin(e0rad)*Math.Tan(brad), Math.Cos(lrad));
	var deltas = Math.Asin(Math.Cos(e0rad)*Math.Sin(brad) + Math.Sin(e0rad)*Math.Cos(brad)*Math.Sin(lrad));
  
	//Step 10
	details.DS = CAACoordinateTransformation.RadiansToDegrees(Math.Asin(-Math.Sin(delta0rad)*Math.Sin(deltas) - Math.Cos(delta0rad)*Math.Cos(deltas)*Math.Cos(alpha0rad - alphas)));
  
	//Step 11
	var u = y *Math.Cos(e0rad) - z *Math.Sin(e0rad);
	var v = y *Math.Sin(e0rad) + z *Math.Cos(e0rad);
	var alpharad = Math.Atan2(u, x);
	var alpha = CAACoordinateTransformation.RadiansToDegrees(alpharad);
	var deltarad = Math.Atan2(v, Math.Sqrt(x *x + u *u));
	var delta = CAACoordinateTransformation.RadiansToDegrees(deltarad);
	var xi = Math.Atan2(Math.Sin(delta0rad)*Math.Cos(deltarad)*Math.Cos(alpha0rad - alpharad) - Math.Sin(deltarad)*Math.Cos(delta0rad), Math.Cos(deltarad)*Math.Sin(alpha0rad - alpharad));
  
	//Step 12
	details.DE = CAACoordinateTransformation.RadiansToDegrees(Math.Asin(-Math.Sin(delta0rad)*Math.Sin(deltarad) - Math.Cos(delta0rad)*Math.Cos(deltarad)*Math.Cos(alpha0rad - alpharad)));
  
	//Step 13
	details.Geometricw1 = CAACoordinateTransformation.MapTo0To360Range(W1 - CAACoordinateTransformation.RadiansToDegrees(xi) - 5.07033 *DELTA);
	details.Geometricw2 = CAACoordinateTransformation.MapTo0To360Range(W2 - CAACoordinateTransformation.RadiansToDegrees(xi) - 5.02626 *DELTA);
  
	//Step 14
	var C = 57.2958 * (2 *r *DELTA + R *R - r *r - DELTA *DELTA)/(4 *r *DELTA);
	if (Math.Sin(lrad - l0rad) > 0)
	{
	  details.Apparentw1 = CAACoordinateTransformation.MapTo0To360Range(details.Geometricw1 + C);
	  details.Apparentw2 = CAACoordinateTransformation.MapTo0To360Range(details.Geometricw2 + C);
	}
	else
	{
	  details.Apparentw1 = CAACoordinateTransformation.MapTo0To360Range(details.Geometricw1 - C);
	  details.Apparentw2 = CAACoordinateTransformation.MapTo0To360Range(details.Geometricw2 - C);
	}
  
	//Step 15
	var NutationInLongitude = CAANutation.NutationInLongitude(JD);
	var NutationInObliquity = CAANutation.NutationInObliquity(JD);
	e0 += NutationInObliquity/3600;
	e0rad = CAACoordinateTransformation.DegreesToRadians(e0);
  
	//Step 16
	alpha += 0.005693*(Math.Cos(alpharad)*Math.Cos(l0rad)*Math.Cos(e0rad) + Math.Sin(alpharad)*Math.Sin(l0rad))/Math.Cos(deltarad);
	alpha = CAACoordinateTransformation.MapTo0To360Range(alpha);
	alpharad = CAACoordinateTransformation.DegreesToRadians(alpha);
	delta += 0.005693*(Math.Cos(l0rad)*Math.Cos(e0rad)*(Math.Tan(e0rad)*Math.Cos(deltarad) - Math.Sin(alpharad)*Math.Sin(deltarad)) + Math.Cos(alpharad)*Math.Sin(deltarad)*Math.Sin(l0rad));
	deltarad = CAACoordinateTransformation.DegreesToRadians(delta);
  
	//Step 17
	var NutationRA = CAANutation.NutationInRightAscension(alpha/15, delta, e0, NutationInLongitude, NutationInObliquity);
	var alphadash = alpha + NutationRA/3600;
	var alphadashrad = CAACoordinateTransformation.DegreesToRadians(alphadash);
	var NutationDec = CAANutation.NutationInDeclination(alpha/15, delta, e0, NutationInLongitude, NutationInObliquity);
	var deltadash = delta + NutationDec/3600;
	var deltadashrad = CAACoordinateTransformation.DegreesToRadians(deltadash);
	NutationRA = CAANutation.NutationInRightAscension(alpha0/15, delta0, e0, NutationInLongitude, NutationInObliquity);
	var alpha0dash = alpha0 + NutationRA/3600;
	var alpha0dashrad = CAACoordinateTransformation.DegreesToRadians(alpha0dash);
	NutationDec = CAANutation.NutationInDeclination(alpha0/15, delta0, e0, NutationInLongitude, NutationInObliquity);
	var delta0dash = delta0 + NutationDec/3600;
	var delta0dashrad = CAACoordinateTransformation.DegreesToRadians(delta0dash);
  
	//Step 18
	details.P = CAACoordinateTransformation.MapTo0To360Range(CAACoordinateTransformation.RadiansToDegrees(Math.Atan2(Math.Cos(delta0dashrad)*Math.Sin(alpha0dashrad - alphadashrad), Math.Sin(delta0dashrad)*Math.Cos(deltadashrad) - Math.Cos(delta0dashrad)*Math.Sin(deltadashrad)*Math.Cos(alpha0dashrad - alphadashrad))));
  
	return details;
  }
}
