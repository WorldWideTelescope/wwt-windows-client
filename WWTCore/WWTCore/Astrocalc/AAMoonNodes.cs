using System;
//
//Module : AAMOONNODES.CPP
//Purpose: Implementation for the algorithms which obtain the dates when the Moon passes thro its nodes
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

//////////////////////////// Includes /////////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAMoonNodes
{
//Static methods

  //////////////////////////// Implementation ///////////////////////////////////
  
  public static double K(double Year)
  {
	return 13.4223*(Year - 2000.05);
  }
  public static double PassageThroNode(double k)
  {
	//convert from K to T
	var T = k/1342.23;
	var Tsquared = T *T;
	var Tcubed = Tsquared *T;
	var T4 = Tcubed *T;
  
	var D = CAACoordinateTransformation.MapTo0To360Range(183.6380 + 331.73735682 *k + 0.0014852 *Tsquared + 0.00000209 *Tcubed - 0.000000010 *T4);
	var M = CAACoordinateTransformation.MapTo0To360Range(17.4006 + 26.82037250 *k + 0.0001186 *Tsquared + 0.00000006 *Tcubed);
	var Mdash = CAACoordinateTransformation.MapTo0To360Range(38.3776 + 355.52747313 *k + 0.0123499 *Tsquared + 0.000014627 *Tcubed - 0.000000069 *T4);
	var omega = CAACoordinateTransformation.MapTo0To360Range(123.9767 - 1.44098956 *k + 0.0020608 *Tsquared + 0.00000214 *Tcubed - 0.000000016 *T4);
	var V = CAACoordinateTransformation.MapTo0To360Range(299.75 + 132.85 *T - 0.009173 *Tsquared);
	var P = CAACoordinateTransformation.MapTo0To360Range(omega + 272.75 - 2.3 *T);
	var E = 1 - 0.002516 *T - 0.0000074 *Tsquared;
  
	//convert to radians
	D = CAACoordinateTransformation.DegreesToRadians(D);
	var D2 = 2 *D;
	var D4 = D2 *D2;
	M = CAACoordinateTransformation.DegreesToRadians(M);
	Mdash = CAACoordinateTransformation.DegreesToRadians(Mdash);
	var Mdash2 = 2 *Mdash;
	omega = CAACoordinateTransformation.DegreesToRadians(omega);
	V = CAACoordinateTransformation.DegreesToRadians(V);
	P = CAACoordinateTransformation.DegreesToRadians(P);
  
	var JD = 2451565.1619 + 27.212220817 *k + 0.0002762 *Tsquared + 0.000000021 *Tcubed - 0.000000000088 *T4 - 0.4721 *Math.Sin(Mdash) - 0.1649 *Math.Sin(D2) - 0.0868 *Math.Sin(D2 - Mdash) + 0.0084 *Math.Sin(D2 + Mdash) - E *0.0083 *Math.Sin(D2 - M) - E *0.0039 *Math.Sin(D2 - M - Mdash) + 0.0034 *Math.Sin(Mdash2) - 0.0031 *Math.Sin(D2 - Mdash2) + E *0.0030 *Math.Sin(D2 + M) + E *0.0028 *Math.Sin(M - Mdash) + E *0.0026 *Math.Sin(M) + 0.0025 *Math.Sin(D4) + 0.0024 *Math.Sin(D) + E *0.0022 *Math.Sin(M + Mdash) + 0.0017 *Math.Sin(omega) + 0.0014 *Math.Sin(D4 - Mdash) + E *0.0005 *Math.Sin(D2 + M - Mdash) + E *0.0004 *Math.Sin(D2 - M + Mdash) - E *0.0003 *Math.Sin(D2 - M *M) + E *0.0003 *Math.Sin(D4 - M) + 0.0003 *Math.Sin(V) + 0.0003 *Math.Sin(P);
  
	return JD;
  }
}
