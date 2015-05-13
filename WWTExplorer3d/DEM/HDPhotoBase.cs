//------------------------------------------------------------------------------
// HDPhotoBase.cs
//
// <copyright company='Microsoft Corporation'> 
// Copyright (c) Microsoft Corporation. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential. 
// </copyright>
// Based on HD Photo Device Porting Kit v1.0 - November 2006, contact: HDphoto@microsoft.com
//
// Owner: macbork, 05.26.09
//
//------------------------------------------------------------------------------

namespace Microsoft.Maps.ElevationAdjustmentService.HDPhoto
{
	/// <summary>
	/// Class used for constants and static methods.
	/// </summary>
	internal abstract class HDPhotoBase
	{
		#region Constants
		// -------------------------------- from windowsmediaphoto.h --------------------------------------
		internal static readonly uint[] MaskBits = new uint[] { 0, 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095,
																	8191, 16383, 32767, 65535, 131071, 262143, 524287, 1048575, 
																	2097151, 4194303, 8388607, 16777215 };
		private struct Constant
		{
			internal static readonly uint[] g_Count = { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };
		}
		#endregion

		#region Fields
		/// <summary>
		/// Array with uncompressed bitestream
		/// </summary>
		protected byte[] data;
		#endregion

		#region Members
		protected static uint RotL(uint value, int shift)
		{
			System.Diagnostics.Debug.Assert(shift >= 0 && shift <32);

			return (value << shift) | ((value >> (32 - shift)) & MaskBits[shift]);
		}

		protected static void StrPost4x4Stage1(int[] array, int j, int iOffset)
		{
			StrPost4x4Stage1Split(array, j, array, j + 16, iOffset);
		}

		protected static void StrPost4x4Stage1Split(int[] p0, int i, int[] p1, int j, int iOffset)
		{
			int imOff = i - iOffset;
			int jmOff = j - iOffset;

			/** buttefly **/
			StrDCT2x2dn(ref p0[i + 12 + 0], ref p0[imOff + 72 + 0], ref p1[j + 4 + 0], ref p1[jmOff + 64 + 0]);
			StrDCT2x2dn(ref p0[i + 12 + 1], ref p0[imOff + 72 + 1], ref p1[j + 4 + 1], ref p1[jmOff + 64 + 1]);
			StrDCT2x2dn(ref p0[i + 12 + 2], ref p0[imOff + 72 + 2], ref p1[j + 4 + 2], ref p1[jmOff + 64 + 2]);
			StrDCT2x2dn(ref p0[i + 12 + 3], ref p0[imOff + 72 + 3], ref p1[j + 4 + 3], ref p1[jmOff + 64 + 3]);

			/** bottom right corner: -pi/8 rotation => -pi/8 rotation **/
			InvOddOddPost(ref p1[jmOff + 64 + 0], ref p1[jmOff + 64 + 1], ref p1[jmOff + 64 + 2], ref p1[jmOff + 64 + 3]);

			/** anti diagonal corners: rotation by -pi/8 **/
			IRotate1(ref p1[j + 4 + 2], ref p1[j + 4 + 3]);
			IRotate1(ref p1[j + 4 + 0], ref p1[j + 4 + 1]);
			IRotate1(ref p0[i + 72 + 1], ref p0[i + 72 + 3]);
			IRotate1(ref p0[i + 72 + 0], ref p0[i + 72 + 2]);

			/** butterfly **/
			i += 12;
			imOff += 72;
			j += 4;
			jmOff += 64;

			StrHSTdec1(ref p0[i], ref p1[jmOff]);
			StrHSTdec(ref p0[i++], ref p0[imOff++], ref p1[j++], ref p1[jmOff++]);
			StrHSTdec1(ref p0[i], ref p1[jmOff]);
			StrHSTdec(ref p0[i++], ref p0[imOff++], ref p1[j++], ref p1[jmOff++]);
			StrHSTdec1(ref p0[i], ref p1[jmOff]);
			StrHSTdec(ref p0[i++], ref p0[imOff++], ref p1[j++], ref p1[jmOff++]);
			StrHSTdec1(ref p0[i], ref p1[jmOff]);
			StrHSTdec(ref p0[i], ref p0[imOff], ref p1[j], ref p1[jmOff]);
		}

		/** Kron(Rotate(pi/8), Rotate(pi/8)) **/
		protected static void InvOddOddPost(ref int a, ref int b, ref int c, ref int d)
		{
			int t1, t2;
			/** butterflies **/
			d += a;
			c -= b;
			a -= (t1 = d >> 1);
			b += (t2 = c >> 1);

			/** rotate pi/4 **/
			a -= (b * 3 + 6) >> 3;
			b += (a * 3 + 2) >> 2;
			a -= (b * 3 + 4) >> 3;

			/** butterflies **/
			b -= t2;
			a += t1;
			c += b;
			d -= a;
		}

		/** 
			Hadamard+Scale transform
			for some strange reason, breaking up the function into two blocks, strHSTdec1 and strHSTdec
			seems to work faster
		**/
		protected static void StrHSTdec1(ref int a, ref int d)
		{
			/** different realization : does rescaling as well! **/
			a += d;
			d = (a >> 1) - d;
			a += (d * 3 + 0) >> 3;
			d += (a * 3 + 0) >> 4;
		}

		protected static void StrHSTdec(ref int pa, ref int pb, ref int pc, ref int pd)
		{
			/** different realization : does rescaling as well! **/
			int a, b, c, d;
			a = pa;
			b = pb;
			c = pc;
			d = pd;

			b -= c;
			a += (d * 3 + 4) >> 3;

			d -= (b >> 1);
			c = ((a - b) >> 1) - c;

			pc = d;
			pd = c;
			pa = a - c;
			pb = b + d;
		}

		/** 4-point post for boundaries **/
		protected static void StrPost4(ref int a, ref int b, ref int c, ref int d)
		{
			a += d;
			b += c;
			d -= ((a + 1) >> 1);
			c -= ((b + 1) >> 1);

			IRotate1(ref c, ref d);

			d += ((a + 1) >> 1);
			c += ((b + 1) >> 1);
			a -= d - ((d * 3 + 16) >> 5);
			b -= c - ((c * 3 + 16) >> 5);
			d += ((a * 3 + 8) >> 4);
			c += ((b * 3 + 8) >> 4);
			a += ((d * 3 + 16) >> 5);
			b += ((c * 3 + 16) >> 5);
		}

		protected static void IRotate1(ref int a, ref int b)
		{
			a -= ((b + 1) >> 1);
			b += ((a + 1) >> 1);
		}

		protected static void StrIDCT4x4Stage1(int[] array, int i)
		{
			/** top left corner, butterfly => butterfly **/
			StrDCT2x2up(ref array[i + 0], ref array[i + 1], ref array[i + 2], ref array[i + 3]);

			/** top right corner, -pi/8 rotation => butterfly **/
			InvOdd(ref array[i + 5], ref array[i + 4], ref array[i + 7], ref array[i + 6]);

			/** bottom left corner, butterfly => -pi/8 rotation **/
			InvOdd(ref array[i + 10], ref array[i + 8], ref array[i + 11], ref array[i + 9]);

			/** bottom right corner, -pi/8 rotation => -pi/8 rotation **/
			InvOddOdd(ref array[i + 15], ref array[i + 14], ref array[i + 13], ref array[i + 12]);

			/** FOURBUTTERFLY_HARDCODED1 **/
			StrDCT2x2dn(ref array[i + 0], ref array[i + 4], ref array[i + 8], ref array[i + 12]);
			StrDCT2x2dn(ref array[i + 1], ref array[i + 5], ref array[i + 9], ref array[i + 13]);
			StrDCT2x2dn(ref array[i + 2], ref array[i + 6], ref array[i + 10], ref array[i + 14]);
			StrDCT2x2dn(ref array[i + 3], ref array[i + 7], ref array[i + 11], ref array[i + 15]);
		}

		protected static void StrIDCT4x4Stage2(int[] array, int i)
		{
			/** bottom left corner, butterfly => -pi/8 rotation **/
			InvOdd(ref array[i + 32], ref array[i + 48], ref array[i + 96], ref array[i + 112]);

			/** top right corner, -pi/8 rotation => butterfly **/
			InvOdd(ref array[i + 128], ref array[i + 192], ref array[i + 144], ref array[i + 208]);

			/** bottom right corner, -pi/8 rotation => -pi/8 rotation **/
			InvOddOdd(ref array[i + 160], ref array[i + 224], ref array[i + 176], ref array[i + 240]);

			/** top left corner, butterfly => butterfly **/
			StrDCT2x2up(ref array[i + 0], ref array[i + 64], ref array[i + 16], ref array[i + 80]);

			/** FOURBUTTERFLY **/
			/** 2x2 dct of a group of 4**/
			StrDCT2x2dn(ref array[i + 0], ref array[i + 192], ref array[i + 48], ref array[i + 240]);
			StrDCT2x2dn(ref array[i + 64], ref array[i + 128], ref array[i + 112], ref array[i + 176]);
			StrDCT2x2dn(ref array[i + 16], ref array[i + 208], ref array[i + 32], ref array[i + 224]);
			StrDCT2x2dn(ref array[i + 80], ref array[i + 144], ref array[i + 96], ref array[i + 160]);
		}

		/* need to swap b and c */
		/* rounding behavior: [0 0 0 0] <-> [+ - - -]
			[+ + + +] <-> [+3/4 - - -]
			[- - - -] <-> [- - - -] */
		protected static void StrDCT2x2dn(ref int a, ref int b, ref int c, ref int d)
		{
			int C = c, t;

			a += d;
			b -= C;
			t = ((a - b) >> 1);
			c = t - d;
			d = t - C;
			a -= d;
			b += c;
		}

		protected static void StrDCT2x2up(ref int a, ref int b, ref int c, ref int d)
		{
			int C = c, t;

			a += d;
			b -= C;
			t = ((a - b + 1) >> 1);
			c = t - d;
			d = t - C;
			a -= d;
			b += c;
		}

		/** Kron(Rotate(-pi/8), [1 1; 1 -1]/sqrt(2)) **/
		/** [D C A B] => [a b c d] **/
		protected static void InvOdd(ref int a, ref int b, ref int c, ref int d)
		{
			/** butterflies **/
			b += d;
			a -= c;
			d -= (b) >> 1;
			c += (a + 1) >> 1;

			/** rotate pi/8 **/
			IRotate2(ref a, ref b);
			IRotate2(ref c, ref d);

			/** butterflies **/
			c -= (b + 1) >> 1;
			d = ((a + 1) >> 1) - d;
			b += c;
			a -= d;
		}

		/** Kron(Rotate(pi/8), Rotate(pi/8)) **/
		protected static void InvOddOdd(ref int a, ref int b, ref int c, ref int d)
		{
			int t1, t2;

			/** butterflies **/
			d += a;
			c -= b;
			a -= (t1 = d >> 1);
			b += (t2 = c >> 1);

			/** rotate pi/4 **/
			a -= (b * 3 + 3) >> 3;
			b += (a * 3 + 3) >> 2;
			a -= (b * 3 + 4) >> 3;

			/** butterflies **/
			b -= t2;
			a += t1;
			c += b;
			d -= a;

			/** sign flips **/
			b = -b;
			c = -c;
		}

		protected static void IRotate2(ref int a, ref int b)
		{
			a -= ((b * 3 + 4) >> 3);
			b += ((a * 3 + 4) >> 3);  // this works well too
		}

		protected static int Saturate32(int x)
		{
			if ((uint)(x + 16) >= 32)
			{
				if (x < 0)
				{
					x = -16;
				}
				else
				{
					x = 15;
				}
			}

			return x;
		}

		/*************************************************************************
			CBP
		*************************************************************************/
		protected static int NumOnes(int i)
		{
			int retval = 0;
			i = i & 0xffff;
			while (i != 0)
			{
				retval += (int)Constant.g_Count[i & 0xf];
				i >>= 4;
			}
			return retval;
		}


		#endregion
	}
}

/*
 * assuming:
 * 
 * m_bSecondary = false
 * uiTileX and uiTileY are all zeros
 * cNumBitIO = 0
 * cNumOfSliceMinus1V = 0;
 * bfBitstreamFormat = SPATIAL
 * bDecodeFullFrame = false
 * number of channels = 1
 * cThumbnailScale < 16
 * pSC->m_Dparam->cThumbnailScale = 1 @ invTransformMacroblock
 * pSC->pTile[pSC->cTileColumn].cBitsLP = 0
 * cfColorFormat == Y_ONLY
 * iQIndexLP = 0
 * iQIndexHP = 0
 * bTranscode = 0
 * pContext->m_bInROI = true
 * cPostProcStrength = 0
*/