//
//Module : AAEASTER.CPP
//Purpose: Implementation for the algorithms which calculate the date of Easter
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

/////////////////////////////////// Includes //////////////////////////////////


///////////////////// Classes /////////////////////////////////////////////////

public class  CAAEasterDetails
{
//Constructors / Destructors
  public CAAEasterDetails()
  {
	  Month = 0;
	  Day = 0;
  }

//Member variables
  public int Month;
  public int Day;
}

public class  CAAEaster
{
//Static methods

  /////////////////////////////////// Implementation ////////////////////////////
  
  public static CAAEasterDetails Calculate(int nYear, bool GregorianCalendar)
  {
	var details = new CAAEasterDetails();
  
	if (GregorianCalendar)
	{
	  var a = nYear % 19;
	  var b = nYear / 100;
	  var c = nYear % 100;
	  var d = b / 4;
	  var e = b % 4;
	  var f = (b+8) / 25;
	  var g = (b - f + 1) / 3;
	  var h = (19 *a + b - d - g + 15) % 30;
	  var i = c / 4;
	  var k = c % 4;
	  var l = (32 + 2 *e + 2 *i - h -k) % 7;
	  var m = (a + 11 *h +22 *l) / 451;
	  var n = (h + l - 7 *m + 114) / 31;
	  var p = (h + l - 7 *m + 114) % 31;
	  details.Month = n;
	  details.Day = p + 1;
	}
	else
	{
	  var a = nYear % 4;
	  var b = nYear % 7;
	  var c = nYear % 19;
	  var d = (19 *c + 15) % 30;
	  var e = (2 *a + 4 *b - d + 34) % 7;
	  var f = (d + e + 114) / 31;
	  var g = (d + e + 114) % 31;
	  details.Month = f;
	  details.Day = g + 1;
	}
  
	return details;
  }
}
