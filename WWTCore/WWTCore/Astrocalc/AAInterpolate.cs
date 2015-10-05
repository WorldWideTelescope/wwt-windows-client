using System;
//
//Module : AAINTERPOLATE.CPP
//Purpose: Implementation for the algorithms for Interpolation
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
////////////////////// Includes ///////////////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class  CAAInterpolate
{
//Static methods

  ////////////////////// Implementation /////////////////////////////////////////
  
  public static double Interpolate(double n, double Y1, double Y2, double Y3)
  {
	var a = Y2 - Y1;
	var b = Y3 - Y2;
	var c = Y1 + Y3 - 2 *Y2;
  
	return Y2 + n / 2 * (a + b + n *c);
  }
  public static double Interpolate(double n, double Y1, double Y2, double Y3, double Y4, double Y5)
  {
	var A = Y2 - Y1;
	var B = Y3 - Y2;
	var C = Y4 - Y3;
	var D = Y5 - Y4;
	var E = B - A;
	var F = C - B;
	var G = D - C;
	var H = F - E;
	var J = G - F;
	var K = J - H;
  
	var N2 = n *n;
	var N3 = N2 *n;
	var N4 = N3 *n;
  
	return Y3 + n*((B+C)/2 - (H+J)/12) + N2*(F/2 - K/24) + N3*((H+J)/12) + N4*(K/24);
  }
  public static double InterpolateToHalves(double Y1, double Y2, double Y3, double Y4)
  {
	return (9*(Y2 + Y3) - Y1 - Y4) / 16;
  }
  public static double LagrangeInterpolate(double X, int n, double[] pX, double[] pY)
  {
	double V = 0;
  
	for (var i =1; i<=n; i++)
	{
	  double C = 1;
	  for (var j =1; j<=n; j++)
	  {
		if (j != i)
		  C = C*(X - pX[j-1]) / (pX[i-1] - pX[j-1]);
	  }
  
	  V += C *pY[i - 1];
	}
  
	return V;
  }
  public static double Extremum(double Y1, double Y2, double Y3, ref double nm)
  {
	var a = Y2 - Y1;
	var b = Y3 - Y2;
	var c = Y1 + Y3 - 2 *Y2;
  
	var ab = a + b;
  
	nm = -ab/(2 *c);
	return (Y2 - ((ab *ab)/(8 *c)));
  }
  public static double Extremum(double Y1, double Y2, double Y3, double Y4, double Y5, ref double nm)
  {
	var A = Y2 - Y1;
	var B = Y3 - Y2;
	var C = Y4 - Y3;
	var D = Y5 - Y4;
	var E = B - A;
	var F = C - B;
	var G = D - C;
	var H = F - E;
	var J = G - F;
	var K = J - H;
  
	var bRecalc = true;
	double nmprev = 0;
	nm = nmprev;
	while (bRecalc)
	{
	  var NMprev2 = nmprev *nmprev;
	  var NMprev3 = NMprev2 *nmprev;
	  nm = (6 *B + 6 *C - H - J +3 *NMprev2*(H+J) + 2 *NMprev3 *K) / (K - 12 *F);
  
	  bRecalc = (Math.Abs(nm - nmprev) > 1E-12);
	  if (bRecalc)
		nmprev = nm;
	}
  
	return Interpolate(nm, Y1, Y2, Y3, Y4, Y5);
  }
  public static double Zero(double Y1, double Y2, double Y3)
  {
	var a = Y2 - Y1;
	var b = Y3 - Y2;
	var c = Y1 + Y3 - 2 *Y2;
  
	var bRecalc = true;
	double n0prev = 0;
	var n0 = n0prev;
	while (bRecalc)
	{
	  n0 = -2 *Y2/(a + b + c *n0prev);
  
	  bRecalc = (Math.Abs(n0 - n0prev) > 1E-12);
	  if (bRecalc)
		n0prev = n0;
	}
  
	return n0;
  }
  public static double Zero(double Y1, double Y2, double Y3, double Y4, double Y5)
  {
	var A = Y2 - Y1;
	var B = Y3 - Y2;
	var C = Y4 - Y3;
	var D = Y5 - Y4;
	var E = B - A;
	var F = C - B;
	var G = D - C;
	var H = F - E;
	var J = G - F;
	var K = J - H;
  
	var bRecalc = true;
	double n0prev = 0;
	var n0 = n0prev;
	while (bRecalc)
	{
	  var n0prev2 = n0prev *n0prev;
	  var n0prev3 = n0prev2 *n0prev;
	  var n0prev4 = n0prev3 *n0prev;
  
	  n0 = (-24 *Y3 + n0prev2*(K - 12 *F) - 2 *n0prev3*(H+J) - n0prev4 *K)/(2*(6 *B + 6 *C - H - J));
  
	  bRecalc = (Math.Abs(n0 - n0prev) > 1E-12);
	  if (bRecalc)
		n0prev = n0;
	}
  
	return n0;
  }
  public static double Zero2(double Y1, double Y2, double Y3)
  {
	var a = Y2 - Y1;
	var b = Y3 - Y2;
	var c = Y1 + Y3 - 2 *Y2;
  
	var bRecalc = true;
	double n0prev = 0;
	var n0 = n0prev;
	while (bRecalc)
	{
	  var deltan0 = - (2 *Y2 + n0prev*(a + b + c *n0prev)) / (a + b + 2 *c *n0prev);
	  n0 = n0prev + deltan0;
  
	  bRecalc = (Math.Abs(deltan0) > 1E-12);
	  if (bRecalc)
		n0prev = n0;
	}
  
	return n0;
  }
  public static double Zero2(double Y1, double Y2, double Y3, double Y4, double Y5)
  {
	var A = Y2 - Y1;
	var B = Y3 - Y2;
	var C = Y4 - Y3;
	var D = Y5 - Y4;
	var E = B - A;
	var F = C - B;
	var G = D - C;
	var H = F - E;
	var J = G - F;
	var K = J - H;
	var M = K / 24;
	var N = (H + J)/12;
	var P = F/2 - M;
	var Q = (B+C)/2 - N;
  
	var bRecalc = true;
	double n0prev = 0;
	var n0 = n0prev;
	while (bRecalc)
	{
	  var n0prev2 = n0prev *n0prev;
	  var n0prev3 = n0prev2 *n0prev;
	  var n0prev4 = n0prev3 *n0prev;
  
	  var deltan0 = - (M * n0prev4 + N *n0prev3 + P *n0prev2 + Q *n0prev + Y3) / (4 *M *n0prev3 + 3 *N *n0prev2 + 2 *P *n0prev + Q);
	  n0 = n0prev + deltan0;
  
	  bRecalc = (Math.Abs(deltan0) > 1E-12);
	  if (bRecalc)
		n0prev = n0;
	}
  
	return n0;
  }
}
