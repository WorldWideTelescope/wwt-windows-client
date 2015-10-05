//
//Module : AAJEWISHCALENDAR.CPP
//Purpose: Implementation for the algorithms which convert between the Gregorian and Julian calendars and the Jewish calendar
//Created: PJN / 04-02-2004
//History: PJN / 28-01-2007 1. Minor updates to fit in with new layout of CAADate class
//
//Copyright (c) 2004 - 2007 by PJ Naughter (Web: www.naughter.com, Email: pjna@naughter.com)
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

public class  CAAJewishCalendar
{
//Static methods

  //////////////////////////// Implementation ///////////////////////////////////
  
  public static CAACalendarDate DateOfPesach(int Year)
  {
	  return DateOfPesach(Year, true);
  }
//C++ TO C# CONVERTER NOTE: C# does not allow default values for parameters. Overloaded methods are inserted above.
//ORIGINAL LINE: static CAACalendarDate DateOfPesach(int Year, bool bGregorianCalendar = true)
  public static CAACalendarDate DateOfPesach(int Year, bool bGregorianCalendar)
  {
	//What will be the return value
	var Pesach = new CAACalendarDate();
  
	var C = CAADate.INT(Year / 100.0);
	var S = CAADate.INT((3 *C - 5) / 4.0);
	if (bGregorianCalendar == false)
	  S = 0;
	var A = Year + 3760;
	var a = (12 *Year + 12) % 19;
	var b = Year % 4;
	var Q = -1.904412361576 + 1.554241796621 *a + 0.25 *b - 0.003177794022 *Year + S;
	var INTQ = CAADate.INT(Q);
	var j = (INTQ + 3 *Year + 5 *b+ 2 - S) % 7;
	var r = Q - INTQ;
  
	if ((j == 2) || (j == 4) || (j == 6))
	  Pesach.Day = INTQ + 23;
	else if ((j == 1) && (a > 6) && (r >= 0.632870370))
	  Pesach.Day = INTQ + 24;
	else if ((j == 0) && (a > 11) && (r >= 0.897723765))
	  Pesach.Day = INTQ + 23;
	else
	  Pesach.Day = INTQ + 22;
  
	if (Pesach.Day > 31)
	{
	  Pesach.Month = 4;
	  Pesach.Day -= 31;
	}
	else
	  Pesach.Month = 3;
  
	Pesach.Year = A;
  
	return Pesach;
  }
  public static bool IsLeap(int Year)
  {
	var ymod19 = Year % 19;
  
	return (ymod19 == 0) || (ymod19 == 3) || (ymod19 == 6) || (ymod19 == 8) || (ymod19 == 11) || (ymod19 == 14) || (ymod19 == 17);
  }
  public static int DaysInYear(int Year)
  {
	//Find the previous civil year corresponding to the specified jewish year
	var CivilYear = Year - 3761;
  
	//Find the date of the next Jewish Year in that civil year
	var CurrentPesach = DateOfPesach(CivilYear);
	var bGregorian = CAADate.AfterPapalReform(CivilYear, CurrentPesach.Month, CurrentPesach.Day);
	var CurrentYear = new CAADate(CivilYear, CurrentPesach.Month, CurrentPesach.Day, bGregorian);
  
	var NextPesach = DateOfPesach(CivilYear+1);
	var NextYear = new CAADate(CivilYear+1, NextPesach.Month, NextPesach.Day, bGregorian);
  
	return (int)(NextYear - CurrentYear);
  }
}
