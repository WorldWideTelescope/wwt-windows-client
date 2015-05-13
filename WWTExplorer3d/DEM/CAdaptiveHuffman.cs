//------------------------------------------------------------------------------
// CAdaptiveHuffman.cs
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

using System;
using System.Globalization;

namespace Microsoft.Maps.ElevationAdjustmentService.HDPhoto
{
	/// <summary>
	/// adaptive huffman encoding / decoding class
	/// </summary>
	internal class CAdaptiveHuffman
	{
		#region Constants
		private struct Constant
		{
			internal const int HUFFMAN_DECODE_ROOT_BITS			= 5;
			internal const int HUFFMAN_DECODE_ROOT_BITS_LOG		= 3;
			internal static readonly uint[] SIGN_BIT = { 0x0, 0x80, 0x8000, 0x800000, 0x80000000 };

			internal const int THRESHOLD							= 8;
			internal const int MEMORY								= 8;

			internal static readonly int[]  gMaxTables  = { 0,0,0,0, 1,2, 4,2, 2,2, 0,0,5 };
			internal static readonly int[]  gSecondDisc = { 0,0,0,0, 0,0, 1,0, 0,0, 0,0,1 };

			// Huffman lookup tables
			internal static readonly short[] g4HuffLookupTable =
			  new short[] { 19,19,19,19,27,27,27,27,10,10,10,10,10,10,10,10,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0 };

			internal static readonly short[][] g5HuffLookupTable = { 
			  new short[] { 28,28,36,36,19,19,19,19,10,10,10,10,10,10,10,10,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0 },
			  new short[] { 11,11,11,11,19,19,19,19,27,27,27,27,35,35,35,35,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0 }};

			internal static readonly short[][] g6HuffLookupTable = { 
			  new short[] { 13,29,44,44,19,19,19,19,34,34,34,34,34,34,34,34,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0 },
			  new short[] { 12,12,28,28,43,43,43,43,2,2,2,2,2,2,2,2,18,18,18,18,18,18,18,18,34,34,34,34,34,34,34,34,0,0,0,0,0,0,0,0,0,0,0,0 },
			  new short[] { 4,4,12,12,43,43,43,43,18,18,18,18,18,18,18,18,26,26,26,26,26,26,26,26,34,34,34,34,34,34,34,34,0,0,0,0,0,0,0,0,0,0,0,0 },
			  new short[] { 5,13,36,36,43,43,43,43,18,18,18,18,18,18,18,18,25,25,25,25,25,25,25,25,25,25,25,25,25,25,25,25,0,0,0,0,0,0,0,0,0,0,0,0 }};

			internal static readonly short[][] g7HuffLookupTable = {
			  new short[] { 45,53,36,36,27,27,27,27,2,2,2,2,2,2,2,2,10,10,10,10,10,10,10,10,18,18,18,18,18,18,18,18,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
			  new short[] { -32736,37,28,28,19,19,19,19,10,10,10,10,10,10,10,10,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,5,6,0,0,0,0,0,0,0,0,0,0,0,0 }};

			internal static readonly short[][] g8HuffLookupTable = {
			  new short[] { 53,21,28,28,11,11,11,11,43,43,43,43,59,59,59,59,2,2,2,2,2,2,2,2,34,34,34,34,34,34,34,34,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
			  new short[] { 52,52,20,20,3,3,3,3,11,11,11,11,27,27,27,27,35,35,35,35,43,43,43,43,58,58,58,58,58,58,58,58,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }};

			internal static readonly short[][] g9HuffLookupTable = {
			  new short[] { 13,29,37,61,20,20,68,68,3,3,3,3,51,51,51,51,41,41,41,41,41,41,41,41,41,41,41,41,41,41,41,41,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
			  new short[] { -32736,53,28,28,11,11,11,11,19,19,19,19,43,43,43,43,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,-32734,4,7,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }};

			internal static readonly short[][] g12HuffLookupTable = {
			  new short[] { -32736,5,76,76,37,53,69,85,43,43,43,43,91,91,91,91,57,57,57,57,57,57,57,57,57,57,57,57,57,57,57,57,-32734,1,2,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
			  new short[] { -32736,85,13,53,4,4,36,36,43,43,43,43,67,67,67,67,75,75,75,75,91,91,91,91,58,58,58,58,58,58,58,58,2,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
			  new short[] { -32736,37,92,92,11,11,11,11,43,43,43,43,59,59,59,59,67,67,67,67,75,75,75,75,2,2,2,2,2,2,2,2,-32734,-32732,2,3,6,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
			  new short[] { -32736,29,37,69,3,3,3,3,43,43,43,43,59,59,59,59,75,75,75,75,91,91,91,91,10,10,10,10,10,10,10,10,-32734,10,2,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 },
			  new short[] { -32736,93,28,28,60,60,76,76,3,3,3,3,43,43,43,43,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,-32734,-32732,-32730,2,4,8,6,10,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 }};

			internal static readonly int[] g5DeltaTable = { 0,-1,0,1,1 };

			internal static readonly int[] g6DeltaTable = {
				-1, 1, 1, 1, 0, 1,
				-2, 0, 0, 2, 0, 0,
				-1,-1, 0, 1,-2, 0
			};


			internal static readonly int[] g7DeltaTable = { 1,0,-1,-1,-1,-1,-1 };

			internal static readonly int[] g8DeltaTable = { -1,0,1,1,-1,0,1,1 };

			internal static readonly int[] g9DeltaTable = { 2,2,1,1,-1,-2,-2,-2,-3 };

			internal static readonly int[] g12DeltaTable = {
				1, 1, 1, 1, 1, 0, 0,-1, 2, 1, 0, 0,
				2, 2,-1,-1,-1, 0,-2,-1, 0, 0,-2,-1,
			   -1, 1, 0, 2, 0, 0, 0, 0,-2, 0, 1, 1,
				0, 1, 0, 1,-2, 0,-1,-1,-2,-1,-2,-2
			};
		}
		#endregion

		internal struct Huffman
		{
			internal short[] m_hufDecTable;

			// It is critical that encTable be 32 bits and decTable be 16 bits for 
			// the given huffman routines to work
			internal int m_alphabetSize;
		};

		private int m_iNSymbols;
		internal PointerArray m_pDelta, m_pDelta1;
		private int m_iTableIndex;
		private Huffman m_pHuffman;
		internal bool m_bInitialize;

		internal int m_iDiscriminant, m_iDiscriminant1;
		private int m_iUpperBound;
		private int m_iLowerBound;

		/*************************************************************************
			Initialize an adaptive huffman table
		*************************************************************************/
		internal void InitializeAH(int iNSymbols)
		{
			if (iNSymbols > 255 || iNSymbols <= 0)
			{
				throw new ArgumentOutOfRangeException("iNSymbols", "iNSymbols = " + iNSymbols);
			}

			this.m_iNSymbols = iNSymbols;

			m_pDelta = null;
			m_iDiscriminant = m_iUpperBound = m_iLowerBound = 0;

			m_pHuffman = new Huffman();
		}

		internal int GetHuff(SimpleBitIO bitIO)
		{
			int iSymbol = m_pHuffman.m_hufDecTable[bitIO.PeekBit16(Constant.HUFFMAN_DECODE_ROOT_BITS)];

			if (iSymbol < 0)
			{
				bitIO.GetBit16(Constant.HUFFMAN_DECODE_ROOT_BITS);
			}
			else
			{
				bitIO.GetBit16((uint)(iSymbol & ((1 << Constant.HUFFMAN_DECODE_ROOT_BITS_LOG) - 1)));
			}

			int iSymbolHuff = iSymbol >> Constant.HUFFMAN_DECODE_ROOT_BITS_LOG;

			if (iSymbolHuff < 0)
			{
				iSymbolHuff = iSymbol;
				while ((iSymbolHuff = m_pHuffman.m_hufDecTable[iSymbolHuff + Constant.SIGN_BIT[2] + bitIO.GetBit16(1)]) < 0)
					;
			}
			return iSymbolHuff;
		}

		/*************************************************************************
			Huffman decoding with short tables
		*************************************************************************/
		internal int GetHuffShort(SimpleBitIO bitIO)
		{
			int iSymbol = m_pHuffman.m_hufDecTable[bitIO.PeekBit16(Constant.HUFFMAN_DECODE_ROOT_BITS)];
			System.Diagnostics.Debug.Assert(iSymbol >= 0);

			bitIO.GetBit16((uint)(iSymbol & ((1 << (byte)Constant.HUFFMAN_DECODE_ROOT_BITS_LOG) - 1)));
			return iSymbol >> Constant.HUFFMAN_DECODE_ROOT_BITS_LOG;
		}


		/**********************************************************************
		  Adapt fixed length codes based on discriminant
		**********************************************************************/
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		internal void AdaptDiscriminant()
		{
			bool bChange = false;

			if (!m_bInitialize)
			{
				m_bInitialize = true;
				m_iDiscriminant = m_iDiscriminant1 = 0;
				m_iTableIndex = Constant.gSecondDisc[m_iNSymbols];
			}

			int dL, dH;
			dL = dH = m_iDiscriminant;
			if (Constant.gSecondDisc[m_iNSymbols] != 0) 
			{
				dH = m_iDiscriminant1;
			}

			if (dL < m_iLowerBound) 
			{
				m_iTableIndex--;
				bChange = true;
			}
			else 
				if (dH > m_iUpperBound)
				{
					m_iTableIndex++;
					bChange = true;
				}

			if (bChange) 
			{
				/** if initialization is fixed, we can exit on !bChange **/
				m_iDiscriminant = 0;
				m_iDiscriminant1 = 0;
			}

			if (m_iDiscriminant < -Constant.THRESHOLD * Constant.MEMORY)
			{
				m_iDiscriminant = -Constant.THRESHOLD * Constant.MEMORY;
			}
			else 
				if (m_iDiscriminant > Constant.THRESHOLD * Constant.MEMORY)
				{
					m_iDiscriminant = Constant.THRESHOLD * Constant.MEMORY;
				}

			if (m_iDiscriminant1 < -Constant.THRESHOLD * Constant.MEMORY)
			{
				m_iDiscriminant1 = -Constant.THRESHOLD * Constant.MEMORY;
			}
			else 
				if (m_iDiscriminant1 > Constant.THRESHOLD * Constant.MEMORY)
				{
					m_iDiscriminant1 = Constant.THRESHOLD * Constant.MEMORY;
				}

			int t = m_iTableIndex;
			System.Diagnostics.Debug.Assert(t >= 0);
			System.Diagnostics.Debug.Assert(t < Constant.gMaxTables[m_iNSymbols]);

			m_iLowerBound = (t == 0) ? (-1 << 31) : -Constant.THRESHOLD;
			m_iUpperBound = (t == Constant.gMaxTables[m_iNSymbols] - 1) ? (1 << 30) : Constant.THRESHOLD;

			switch (m_iNSymbols) {

				case 4:
					m_pHuffman.m_hufDecTable = Constant.g4HuffLookupTable;
					break;

				case 5:
					m_pDelta = new PointerArray(Constant.g5DeltaTable,0);
					m_pHuffman.m_hufDecTable = Constant.g5HuffLookupTable[t];
					break;

				case 6:
					m_pDelta1 = new PointerArray(Constant.g6DeltaTable, m_iNSymbols * (t - ((t + 1) == Constant.gMaxTables[m_iNSymbols] ? 1 : 0)));
					m_pDelta = new PointerArray(Constant.g6DeltaTable, (t - 1 + ((t == 0) ? 1 : 0)) * m_iNSymbols);
					m_pHuffman.m_hufDecTable = Constant.g6HuffLookupTable[t];
					break;

				case 7:
					m_pDelta = new PointerArray(Constant.g7DeltaTable, 0);
					m_pHuffman.m_hufDecTable = Constant.g7HuffLookupTable[t];
					break;

				case 8:
					m_pHuffman.m_hufDecTable = Constant.g8HuffLookupTable[0];
					break;

				case 9:
					m_pDelta = new PointerArray(Constant.g9DeltaTable, 0);
					m_pHuffman.m_hufDecTable = Constant.g9HuffLookupTable[t];
					break;

				case 12:
					m_pDelta1 = new PointerArray(Constant.g12DeltaTable, m_iNSymbols * (t - ((t + 1) == Constant.gMaxTables[m_iNSymbols] ? 1 : 0)));
					m_pDelta = new PointerArray(Constant.g12DeltaTable, (t - 1 + ((t == 0) ? 1 : 0)) * m_iNSymbols);
					m_pHuffman.m_hufDecTable = Constant.g12HuffLookupTable[t];
					break;

				default:
					throw new ArgumentOutOfRangeException("Undefined fixed length table in AdaptDiscriminant()");
			}
		}

		#region Test Members
//#if DEBUG
//        internal void PrintCAdaptiveHuffman(System.IO.IsolatedStorage.IsolatedStorageFileStream file, string name, int j)
//        {
//            int i;
			
//            HDPhotoDecoder.WriteLine(file, name + "[" + j + "]");
//            HDPhotoDecoder.WriteLine(file, "m_iNSymbols m_iTableIndex m_iDiscriminant m_iDiscriminant1 m_iUpperBound m_iLowerBound m_bInitialize");
//            HDPhotoDecoder.WriteLine(file, m_iNSymbols + " " + m_iTableIndex + " " + m_iDiscriminant + " " + m_iDiscriminant1 + " " + m_iUpperBound + " " + m_iLowerBound + " " + (m_bInitialize ? 1 : 0));

//            HDPhotoDecoder.WriteLine(file, "m_pHuffman");
//            HDPhotoDecoder.WriteLine(file, m_pHuffman.m_alphabetSize.ToString(CultureInfo.InvariantCulture));
//            for (i = 0; i < 40; i++)
//                HDPhotoDecoder.WriteLine(file, m_pHuffman.m_hufDecTable[i].ToString(CultureInfo.InvariantCulture));

//            HDPhotoDecoder.WriteLine(file, "m_pDelta");
//            if (m_pDelta == null)
//                HDPhotoDecoder.WriteLine(file, "null");
//            else
//                for (i = 0; i < 5; i++)
//                    HDPhotoDecoder.WriteLine(file, m_pDelta[(uint)i].ToString(CultureInfo.InvariantCulture));
//        }
//#endif
		#endregion
	}
}
