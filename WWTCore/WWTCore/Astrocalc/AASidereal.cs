using System;
//
//Module : AASIDEREAL.CPP
//Purpose: Implementation for the algorithms which obtain sidereal time
//Created: PJN / 29-12-2003
//         PJN / 26-01-2007 1. Update to fit in with new layout of CAADate class
//         PJN / 28-01-2007 1. Minor updates to fit in with new layout of CAADate class
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

/////////////////////////////// Includes //////////////////////////////////////



/////////////////////// Classes ///////////////////////////////////////////////

public class  CAASidereal
{
//Static methods

	/////////////////////////////// Implementation ////////////////////////////////
	
	public static double MeanGreenwichSiderealTime(double JD)
	{
	  //Get the Julian day for the same day at midnight
	  var Year = 0;
	  var Month = 0;
	  var Day = 0;
	  var Hour = 0;
	  var Minute = 0;
	  double Second = 0;
	
	  var date = new CAADate();
	  date.Set(JD, CAADate.AfterPapalReform(JD));
	  date.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);
	  date.Set(Year, Month, Day, 0, 0, 0, date.InGregorianCalendar());
	  var JDMidnight = date.Julian();
	
	  //Calculate the sidereal time at midnight
	  var T = (JDMidnight - 2451545) / 36525;
	  var TSquared = T *T;
	  var TCubed = TSquared *T;
	  var Value = 100.46061837 + (36000.770053608 *T) + (0.000387933 *TSquared) - (TCubed/38710000);
	
	  //Adjust by the time of day
	  Value += (((Hour * 15) + (Minute * 0.25) + (Second * 0.0041666666666666666666666666666667)) * 1.00273790935);
	
	  Value = CAACoordinateTransformation.DegreesToHours(Value);
	
	  return CAACoordinateTransformation.MapTo0To24Range(Value);
	}

  public static double ApparentGreenwichSiderealTime(double JD)
  {
	var MeanObliquity = CAANutation.MeanObliquityOfEcliptic(JD);
	var TrueObliquity = MeanObliquity + CAANutation.NutationInObliquity(JD) / 3600;
	var NutationInLongitude = CAANutation.NutationInLongitude(JD);
  
	var Value = MeanGreenwichSiderealTime(JD) + (NutationInLongitude * Math.Cos(CAACoordinateTransformation.DegreesToRadians(TrueObliquity)) / 54000);
	return CAACoordinateTransformation.MapTo0To24Range(Value);
  }
}
