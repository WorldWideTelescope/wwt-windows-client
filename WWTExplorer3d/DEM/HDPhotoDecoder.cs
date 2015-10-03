//------------------------------------------------------------------------------
// HDPhotoDecoder.cs
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

namespace Microsoft.Maps.ElevationAdjustmentService.HDPhoto
{
	internal sealed class HDPhotoDecoder : BitstreamDecoder
	{
		#region Constants
		private struct Constant
		{
			internal const byte IsDCChNumBits						= 1;

			internal const int CONTEXTX							= 8;
			internal const int CTDC								= 5;
			internal const int NUMVLCTABLES						= 21; // CONTEXTX * 2 + CTDC
			internal const int MODELWEIGHT						= 70;//90
			internal const int MAXTOTAL							= 32767; // 511 should be enough
			internal const int ORIENT_WEIGHT						= 4;
			/** Quantization related defines **/
			internal const int SHIFTZERO							= 1; /* >= 0 */
			//#define QPFRACBITS 2  /* or 0 only supported */
			internal const int AVG_NDIFF							= 3;
		    
			internal static readonly uint[] aAlphabet = {5,4,8,7,7,  12,6,6,12,6,6,7,7,  12,6,6,12,6,6,7,7};
			internal static readonly uint[] aRemap = { 2, 3, 4, 6, 10, 14 };
			internal static readonly uint[] aFixedLength = { 0, 0, 1, 2, 2, 2 };

			internal static readonly int[]   aWeight0 = { 240/*DC*/, 12/*LP*/, 1 };
			// the second dimension should be dropped, we're assuming one channel only
			internal static readonly int[][] aWeight1 =
			{
			    new[] { 0,240,120,80, 60,48,40,34, 30,27,24,22, 20,18,17,16 },
			    new[] { 0,12,6,4,     3,2,2,2,     2,1,1,1,     1,1,1,1 },
			    new[] { 0,16,8,5,     4,3,3,2,     2,2,2,1,     1,1,1,1 }
			};
			internal static readonly int[]   aWeight2 = { 120,37,2,/*420*/ 120,18,1/*422*/ };

			internal static readonly int[] grgiZigzagInv4x4_lowpass = { 0, 1, 4, 5, 2, 8, 6, 9,3, 12, 10, 7, 13, 11, 14, 15};
			internal static readonly int[] grgiZigzagInv4x4H = { 0, 1, 4, 5, 2, 8, 6, 9, 3, 12, 10, 7, 13, 11, 14, 15};
			internal static readonly int[] grgiZigzagInv4x4V = { 0, 4, 8, 5,  1, 12, 9, 6,  2, 13, 3, 15,  7, 10, 14, 11};

			internal static readonly int[] aRemap2 = {1,2,3,5,7,   1,2,3,5,7,   /*1,2,3,4,6,  */1,2,3,4,5 };

			internal static readonly int[] gSignificantRunBin = { -1,-1,-1,-1,	2,2,2,	1,1,1,1,	0,0,0,0 };
			internal static readonly uint[] gSignificantRunFixedLength = {	0,0,1,1,3,	0,0,1,1,2,	0,0,0,0,1 };

			internal static readonly QPManExp[] gs_QPRecipTable = {
				new QPManExp( 0x0, 0), // 0, invalid
				new QPManExp( 0x0, 0), // 1, lossless
				new QPManExp( 0x0, 1), // 2
				new QPManExp( 0xaaaaaaab, 1),
				new QPManExp( 0x0, 2), // 4
				new QPManExp( 0xcccccccd, 2),
				new QPManExp( 0xaaaaaaab, 2),
				new QPManExp( 0x92492493, 2),
				new QPManExp( 0x0, 3), // 8
				new QPManExp( 0xe38e38e4, 3),
				new QPManExp( 0xcccccccd, 3),
				new QPManExp( 0xba2e8ba3, 3),
				new QPManExp( 0xaaaaaaab, 3),
				new QPManExp( 0x9d89d89e, 3),
				new QPManExp( 0x92492493, 3),
				new QPManExp( 0x88888889, 3),
				new QPManExp( 0x0, 4), // 16
				new QPManExp( 0xf0f0f0f1, 4),
				new QPManExp( 0xe38e38e4, 4),
				new QPManExp( 0xd79435e6, 4),
				new QPManExp( 0xcccccccd, 4),
				new QPManExp( 0xc30c30c4, 4),
				new QPManExp( 0xba2e8ba3, 4),
				new QPManExp( 0xb21642c9, 4),
				new QPManExp( 0xaaaaaaab, 4),
				new QPManExp( 0xa3d70a3e, 4),
				new QPManExp( 0x9d89d89e, 4),
				new QPManExp( 0x97b425ee, 4),
				new QPManExp( 0x92492493, 4),
				new QPManExp( 0x8d3dcb09, 4),
				new QPManExp( 0x88888889, 4),
				new QPManExp( 0x84210843, 4)
			};

			/** permutation matrix tailored to the transform, nothing to do with ZZS **/
			internal static readonly int[][] dctIndex =
			{ 
				new[] {0,5,1,6, 10,12,8,14, 2,4,3,7, 9,13,11,15}, //AC 444
				new[] {0,5,1,6, 10,12,8,14, 2,4,3,7, 9,13,11,15}, //AC 420
				new[] {0,128,64,208, 32,240,48,224, 16,192,80,144, 112,176,96,160 } //DC 444
			};

			internal static readonly int[]  aTab = { 6, 9, 10, 12 };
			internal static readonly uint[] gFLC0 = { 0,2,1,2,2,0 };
			internal static readonly uint[] gOff0 = { 0,4,2,8,12,1 };
			internal static readonly uint[] gOut0 = { 0,15,3,12, 1,2,4,8, 5,6,9,10, 7,11,13,14 };

			//================================================================
			// Quantization index tables
			//================================================================
			internal static readonly int[] blkOffset = {0, 64, 16, 80, 128, 192, 144, 208, 32, 96, 48, 112, 160, 224, 176, 240};

			internal static readonly byte[] blkIdx = {1, 2, 3, 5, 6, 7, 9, 10, 11, 13, 14, 15};

			//================================================================
			// Color conversion index table
			//================================================================
			internal static readonly byte[][] idxCC=
			{
				new byte[] {0x00, 0x01, 0x05, 0x04,  0x40, 0x41, 0x45, 0x44,  0x80, 0x81, 0x85, 0x84,  0xc0, 0xc1, 0xc5, 0xc4 },
				new byte[] {0x02, 0x03, 0x07, 0x06,  0x42, 0x43, 0x47, 0x46,  0x82, 0x83, 0x87, 0x86,  0xc2, 0xc3, 0xc7, 0xc6 },
				new byte[] {0x0a, 0x0b, 0x0f, 0x0e,  0x4a, 0x4b, 0x4f, 0x4e,  0x8a, 0x8b, 0x8f, 0x8e,  0xca, 0xcb, 0xcf, 0xce },
				new byte[] {0x08, 0x09, 0x0d, 0x0c,  0x48, 0x49, 0x4d, 0x4c,  0x88, 0x89, 0x8d, 0x8c,  0xc8, 0xc9, 0xcd, 0xcc },

				new byte[] {0x10, 0x11, 0x15, 0x14,  0x50, 0x51, 0x55, 0x54,  0x90, 0x91, 0x95, 0x94,  0xd0, 0xd1, 0xd5, 0xd4 },
				new byte[] {0x12, 0x13, 0x17, 0x16,  0x52, 0x53, 0x57, 0x56,  0x92, 0x93, 0x97, 0x96,  0xd2, 0xd3, 0xd7, 0xd6 },
				new byte[] {0x1a, 0x1b, 0x1f, 0x1e,  0x5a, 0x5b, 0x5f, 0x5e,  0x9a, 0x9b, 0x9f, 0x9e,  0xda, 0xdb, 0xdf, 0xde },
				new byte[] {0x18, 0x19, 0x1d, 0x1c,  0x58, 0x59, 0x5d, 0x5c,  0x98, 0x99, 0x9d, 0x9c,  0xd8, 0xd9, 0xdd, 0xdc },

				new byte[] {0x20, 0x21, 0x25, 0x24,  0x60, 0x61, 0x65, 0x64,  0xa0, 0xa1, 0xa5, 0xa4,  0xe0, 0xe1, 0xe5, 0xe4 },
				new byte[] {0x22, 0x23, 0x27, 0x26,  0x62, 0x63, 0x67, 0x66,  0xa2, 0xa3, 0xa7, 0xa6,  0xe2, 0xe3, 0xe7, 0xe6 },
				new byte[] {0x2a, 0x2b, 0x2f, 0x2e,  0x6a, 0x6b, 0x6f, 0x6e,  0xaa, 0xab, 0xaf, 0xae,  0xea, 0xeb, 0xef, 0xee },
				new byte[] {0x28, 0x29, 0x2d, 0x2c,  0x68, 0x69, 0x6d, 0x6c,  0xa8, 0xa9, 0xad, 0xac,  0xe8, 0xe9, 0xed, 0xec },

				new byte[] {0x30, 0x31, 0x35, 0x34,  0x70, 0x71, 0x75, 0x74,  0xb0, 0xb1, 0xb5, 0xb4,  0xf0, 0xf1, 0xf5, 0xf4 },
				new byte[] {0x32, 0x33, 0x37, 0x36,  0x72, 0x73, 0x77, 0x76,  0xb2, 0xb3, 0xb7, 0xb6,  0xf2, 0xf3, 0xf7, 0xf6 },
				new byte[] {0x3a, 0x3b, 0x3f, 0x3e,  0x7a, 0x7b, 0x7f, 0x7e,  0xba, 0xbb, 0xbf, 0xbe,  0xfa, 0xfb, 0xff, 0xfe },
				new byte[] {0x38, 0x39, 0x3d, 0x3c,  0x78, 0x79, 0x7d, 0x7c,  0xb8, 0xb9, 0xbd, 0xbc,  0xf8, 0xf9, 0xfd, 0xfc }
			};


		}

		/* circulant buffer for 2 MB rows: current row and previous row */
		private class CWMIPredInfo
		{
			internal int iQPIndex;		// QP Index
			internal int iCBP;			// coded block pattern
			internal int iDC;				// DC of MB
			internal readonly int[] iAD = new int[6];
			//internal int[] piAD;			// AC of DC block: [2] 420UV [4] 422UV [6] elsewhere
		};

		//================================================================
		private struct CWMIQuantizer
		{
			internal byte iIndex;
			internal int iQP;
			internal int iOffset;
			internal int iMan;
			internal int iExp;
		};

		/* reciprocal (pMantissa, exponent) lookup table */
		internal struct QPManExp
		{
			internal int iMan;
			internal int iExp;

			internal QPManExp(uint iMan, int iExp)
			{
				this.iMan = (int)iMan;
				this.iExp = iExp;
			}
		};
		#endregion

		#region Fields
		private SimpleBitIO bitIO;

		//================================
		// 2 rows of MB buffer
		//================================
		PointerArray a0MBbuffer;
		PointerArray p0MBbuffer;
		PointerArray a1MBbuffer;
		PointerArray p1MBbuffer;

		private int cRow;        // row for current macro block
		private int cColumn;     // column for current macro block

		private int cmbWidth;    // macro block/image width
		private int cmbHeight;   // macro block/image height

		// current tile position
		private int cTileColumn;

		// tile boundary
		private bool m_bCtxLeft;
		private bool m_bCtxTop;

		private bool m_bResetRGITotals;
		private bool m_bResetContext;

	    //================================
		// circulant buffer for 2 MB rows: current row and previous row
		//================================
		CWMIPredInfo[] PredInfo;
		CWMIPredInfo[] PredInfoPrevRow;

		private CCodingContext m_pCodingContext;

		/** thumbnail decode **/
		private bool bDecodeHP;
		private bool bDecodeLP;

	    private readonly int[] iBlockDC = new int [16];	// assuming only one channel

		private int iOrientation;
	    private int iCBP;
		private int iDiffCBP;

		//private struct CWMITile	/* since there is only one tile */
		CWMIQuantizer pQuantizerDC;
		CWMIQuantizer pQuantizerLP;
		CWMIQuantizer pQuantizerHP;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
		private short[,] decompressedBits;	// set to null by the runtime
		#endregion

		#region Overridables
		/// <summary>
		/// Does the actual decompression
		/// </summary>
		protected override void PKImageDecode_Copy_WMP(SimpleBitIO simplebitIO)
		{
			this.bitIO = simplebitIO;

			ImageStrDecInit();
			ImageStrDecDecode();
		}
		#endregion

		#region internal Members
		/// <summary>
		/// Decodes DEM information contained in a HD Photo image.
		/// </summary>
		/// <param name="data2">Bytestream of HD Photo image.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Return")]
		internal short[,] Decode(byte[] data2)
		{
		    if (data2 != null)
			{
				this.data = data2;
				try
				{
					DecodeBitstream(ReadContainer());
				}
				catch
				{

					return null;
				}
				
			}

			return decompressedBits;
		}
		#endregion

		#region Private Members
		/// <summary>
		/// Main decoding loop
		/// </summary>
		private void ImageStrDecDecode()
		{
			SetROI();

			// assuming no thumbnails
			var cMBRow = ((cHeight - 1) + 16) >> 4;

			//================================
			// central rows
			for (cRow = 0; cRow <= cMBRow; cRow++)
			{
				cColumn = 0;
				InitMRPtr();
				
				/** zero out the transform coefficients (pull this out to once per MB row) **/
				for (uint i = 0; i < 16 * 16 * cmbWidth; i++)
				{
					p1MBbuffer[i] = 0;
				}

				ProcessMacroblockDec();
				AdvanceMRPtr();

				for (cColumn = 1; cColumn < cmbWidth; ++cColumn)
				{
					ProcessMacroblockDec();
					AdvanceMRPtr();
				}

				ProcessMacroblockDec();

				if (cRow != 0)
				{
					OutputMBRow();
				}
			
				AdvanceOneMBRow();
				SwapMRPtr();
			}
		}

		private void OutputMBRow()
		{
			const int cROIBottomY = 256;
			const int iFirstRow = 0;
			const int iFirstColumn = 0;
			var height = Math.Min(cROIBottomY + 1 - (cRow - 1) * 16, 16);
			const int iShift = 3;
			const int iBias = 3;

			OutputNChannel(iFirstRow, iFirstColumn, (int)cWidth, height, iShift, iBias);

        }

		// write one MB row of Y_ONLY/CF_ALPHA/YUV_444/N_CHANNEL to output buffer
		private void OutputNChannel(int iFirstRow, int iFirstColumn, int width, int height, int iShift, int iBias)
		{
			var rowOffset = (cRow - 1) << 4;

			for (var iRow = iFirstRow; iRow < height; iRow++)
			{
				for (var iColumn = iFirstColumn; iColumn < width; iColumn++)
				{
					decompressedBits[rowOffset + iRow, iColumn] = 
					(short)((a0MBbuffer[(uint)((iColumn >> 4) << 8) + Constant.idxCC[iRow][iColumn & 15]] + iBias) >> iShift);
				}
			}
		}

		private void SwapMRPtr()
		{
			var array = a0MBbuffer.Array;
			a0MBbuffer.Array = a1MBbuffer.Array;
			a1MBbuffer.Array = array;
		}

		/* advance to next MB row */
		private void AdvanceOneMBRow()
		{
		    // swap current row and previous row
			for (var i = 0; i < cmbWidth; i++)
			{
				CWMIPredInfo pPredInfo = PredInfo[i];
				PredInfo[i] = PredInfoPrevRow[i];
				PredInfoPrevRow[i] = pPredInfo;
			}
		}

		private void SetROI()
		{
			bDecodeHP = (sbSubband == SUBBAND.SB_ALL || sbSubband == SUBBAND.SB_NO_FLEXBITS);
			bDecodeLP = (sbSubband != SUBBAND.SB_DC_ONLY);
		}

		/* inverse transform and overlap possible part of a macroblock */
		private void ProcessMacroblockDec()
		{
		    var bottom = cRow == cmbHeight;
		    var bottomORright = (bottom || cColumn == cmbWidth);

	        if (!bottomORright)
			{
	            GetTilePos(cColumn, cRow);

				ReadPackets();

				DecodeMacroblockDC();

				if (bDecodeLP)
				{
					DecodeMacroblockLowpass();
				}

                PredDCACDec();
                                
                DequantizeMacroblock();

				if (bDecodeHP)
				{
					DecodeMacroblockHighpass();
					PredACDec();
				}
           
                /* keep necessary info for future prediction */
				UpdatePredInfo(cColumn);
			}

			Transform();
		}

		/*************************************************************************
		  Top-level function to inverse tranform possible part of a macroblock
		*************************************************************************/
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private void Transform()
		{
			bool left = cColumn == 0, right = cColumn == cmbWidth;
			bool top = cRow == 0, bottom = cRow == cmbHeight;
			bool topORbottom = (top || bottom), leftORright = (left || right);
			var bottomORright = (bottom || right);

			//================================
			// second level inverse transform
			if (!bottomORright)
			{
				StrIDCT4x4Stage2(p1MBbuffer.Array, p1MBbuffer.Pointer);
			}

	        //================================
	        // first level inverse transform

	        if (!top)
	        {
	            for (var j = (left ? 32 : -96); j < (right ? 32 : 160); j += 64)
	            {
					StrIDCT4x4Stage1(p0MBbuffer.Array, p0MBbuffer.Pointer + j + 0);
					StrIDCT4x4Stage1(p0MBbuffer.Array, p0MBbuffer.Pointer + j + 16);
	            }
	        }

	        if (!bottom)
	        {
	            for (var j = (left ? 0 : -128); j < (right ? 0 : 128); j += 64)
	            {
					StrIDCT4x4Stage1(p1MBbuffer.Array, p1MBbuffer.Pointer + j + 0);
					StrIDCT4x4Stage1(p1MBbuffer.Array, p1MBbuffer.Pointer + j + 16);
	            }
	        }

		    //================================
		    // first level inverse overlap
		    if (OVERLAP.OL_NONE != olOverlap)
		    {
				if (leftORright)
				{
					var j = left ? 0 + 10 : -64 + 14;
					if (!top)
					{
						var k = j + p0MBbuffer.Pointer + 16;
						StrPost4(ref p0MBbuffer.Array[k + 0], ref p0MBbuffer.Array[k - 2], ref p0MBbuffer.Array[k + 6], ref p0MBbuffer.Array[k + 8]);
						StrPost4(ref p0MBbuffer.Array[k + 1], ref p0MBbuffer.Array[k - 1], ref p0MBbuffer.Array[k + 7], ref p0MBbuffer.Array[k + 9]);
						StrPost4(ref p0MBbuffer.Array[k + 16], ref p0MBbuffer.Array[k + 14], ref p0MBbuffer.Array[k + 22], ref p0MBbuffer.Array[k + 24]);
						StrPost4(ref p0MBbuffer.Array[k + 17], ref p0MBbuffer.Array[k + 15], ref p0MBbuffer.Array[k + 23], ref p0MBbuffer.Array[k + 25]);
					}
					if (!bottom)
					{
						var k = j + p1MBbuffer.Pointer;
						StrPost4(ref p1MBbuffer.Array[k + 0], ref p1MBbuffer.Array[k - 2], ref p1MBbuffer.Array[k + 6], ref p1MBbuffer.Array[k + 8]);
						StrPost4(ref p1MBbuffer.Array[k + 1], ref p1MBbuffer.Array[k - 1], ref p1MBbuffer.Array[k + 7], ref p1MBbuffer.Array[k + 9]);
					}
					if (!topORbottom)
					{
						StrPost4(ref p0MBbuffer.Array[p0MBbuffer.Pointer + 48 + j + 0], ref p0MBbuffer.Array[p0MBbuffer.Pointer + 48 + j - 2], ref p1MBbuffer.Array[p1MBbuffer.Pointer - 10 + j], ref p1MBbuffer.Array[p1MBbuffer.Pointer - 8 + j]);
						StrPost4(ref p0MBbuffer.Array[p0MBbuffer.Pointer + 48 + j + 1], ref p0MBbuffer.Array[p0MBbuffer.Pointer + 48 + j - 1], ref p1MBbuffer.Array[p1MBbuffer.Pointer - 9 + j], ref p1MBbuffer.Array[p1MBbuffer.Pointer - 7 + j]);
					}
				}

				if (top)
				{
					for (var j = (left ? 0 : -192); j < (right ? -64 : 64); j += 64)
					{
						var k = p1MBbuffer.Pointer + j;
						StrPost4(ref p1MBbuffer.Array[k + 5], ref p1MBbuffer.Array[k + 4], ref p1MBbuffer.Array[k + 64], ref p1MBbuffer.Array[k + 65]);
						StrPost4(ref p1MBbuffer.Array[k + 7], ref p1MBbuffer.Array[k + 6], ref p1MBbuffer.Array[k + 66], ref p1MBbuffer.Array[k + 67]);

						StrPost4x4Stage1(p1MBbuffer.Array, k, 0);
					}
				}
				else if (bottom)
				{
					for (var j = (left ? 0 : -192); j < (right ? -64 : 64); j += 64)
					{
						var k = p0MBbuffer.Pointer + j;
						StrPost4x4Stage1(p0MBbuffer.Array, 16 + k, 0);
						StrPost4x4Stage1(p0MBbuffer.Array, 32 + k, 0);

						k += 48;
						StrPost4(ref p0MBbuffer.Array[k + 15], ref p0MBbuffer.Array[k + 14], ref p0MBbuffer.Array[k + 74], ref p0MBbuffer.Array[k + 75]);
						StrPost4(ref p0MBbuffer.Array[k + 13], ref p0MBbuffer.Array[k + 12], ref p0MBbuffer.Array[k + 72], ref p0MBbuffer.Array[k + 73]);
					}
				}
				else
				{
					for (var j = (left ? 0 : -192); j < (right ? -64 : 64); j += 64)
					{
						var k = p0MBbuffer.Pointer + j;
						StrPost4x4Stage1(p0MBbuffer.Array, 16 + k, 0);
						StrPost4x4Stage1(p0MBbuffer.Array, 32 + k, 0);
						StrPost4x4Stage1Split(p0MBbuffer.Array, 48 + k, p1MBbuffer.Array, p1MBbuffer.Pointer + j, 0);
						StrPost4x4Stage1(p1MBbuffer.Array, p1MBbuffer.Pointer + j, 0);
					}
				}
		    }
		}

		/* info of current MB to be saved for future prediction */
		private void UpdatePredInfo(int mbX)
		{
			/* DC of DC block */
			PredInfo[mbX].iDC = iBlockDC[0];

			/* QP Index */
			PredInfo[mbX].iQPIndex = 0;

			/* first row and first column of ACs of DC block */
			var dst = PredInfo[mbX].iAD;

			/* first row of ACs */
			dst[0] = iBlockDC[1];
			dst[1] = iBlockDC[2];
			dst[2] = iBlockDC[3];

			/* first column of ACs */
			dst[3] = iBlockDC[4];
			dst[4] = iBlockDC[8];
			dst[5] = iBlockDC[12];
		}

		/*************************************************************************
			Frequency domain inverse AC prediction
		*************************************************************************/
		private void PredACDec()
		{
			/* AC prediction */
			// prediction only happens inside MB

			switch (2-iOrientation)
			{
				case 1:
			    {
			        // predict from top
			        foreach (byte t in Constant.blkIdx)
			        {
			            var k = p1MBbuffer.Pointer + (t << 4);
			            p1MBbuffer.Array[k +  2] += p1MBbuffer.Array[k - 16 +  2];
			            p1MBbuffer.Array[k + 10] += p1MBbuffer.Array[k - 16 + 10];
			            p1MBbuffer.Array[k +  9] += p1MBbuffer.Array[k - 16 +  9];
			        }
			        break;
			    }

			    case 0:
					// predict from left
					for (var j = 64; j < 256; j += 16)
					{
						var k = p1MBbuffer.Pointer + j;
						p1MBbuffer.Array[k + 1] += p1MBbuffer.Array[k - 64 + 1];
						p1MBbuffer.Array[k + 5] += p1MBbuffer.Array[k - 64 + 5];
						p1MBbuffer.Array[k + 6] += p1MBbuffer.Array[k - 64 + 6];
					}
					break;

			        // no prediction
			}
		}

		/*************************************************************************
			DecodeMacroblockHighpass
		*************************************************************************/
		private void DecodeMacroblockHighpass()
		{
			/** reset adaptive scan totals **/
			if (m_bResetRGITotals)
			{
				var iWeight = 2 * 16;
				m_pCodingContext.m_aScanHoriz[0].uTotal = m_pCodingContext.m_aScanVert[0].uTotal = Constant.MAXTOTAL;
				for (var k = 1; k < 16; k++)
				{
					m_pCodingContext.m_aScanHoriz[k].uTotal = m_pCodingContext.m_aScanVert[k].uTotal = iWeight;
					iWeight -= 2;
				}
			}

			DecodeCBP();
			/* Coded Block Pattern (CBP) prediction */
			PredInfo[cColumn].iCBP = iCBP = PredCBPCDec(iDiffCBP, m_pCodingContext.m_aCBPModel); 

			DecodeCoeffs();

			if (m_bResetContext)
			{
				AdaptHighpassDec();
			}
		}

		/*************************************************************************
			GetCoeffs
		*************************************************************************/
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void DecodeCoeffs()
		{
		    int[] aLaplacianMean = { 0, 0};
			
			var iCBPCY = iCBP;

			/** set scan arrays and other MB level constants **/
			CCodingContext.CAdaptiveScan[] pScan = iOrientation == 1 ? m_pCodingContext.m_aScanVert : m_pCodingContext.m_aScanHoriz;

			var iIndex = 0;

			for (var iBlock = 0; iBlock < 4; iBlock++)
			{
				for (var iSubblock = 0; iSubblock < 4; iSubblock++, iIndex++, iCBPCY >>= 1)
				{
					var pCoeffs = new PointerArray(p1MBbuffer,Constant.blkOffset[iIndex & 0xf]);

			        /** read AC values **/
			        var iNumNonZero = DecodeBlockAdaptive((iCBPCY & 1) != 0,ref pCoeffs, ref pScan);
					if (iNumNonZero > 16)
					{
						throw new ArgumentOutOfRangeException("Corrupted bitstream in DecodeCoeffs");
					}
					aLaplacianMean[0] += iNumNonZero;
			    }
			}

			/** update model at end of MB **/
			UpdateModelMB(aLaplacianMean, m_pCodingContext.m_aModelAC);
		}

		/*************************************************************************
			DecodeBlockAdaptive
		*************************************************************************/
		private int DecodeBlockAdaptive(bool bNoSkip, ref PointerArray pCoeffs, ref CCodingContext.CAdaptiveScan[] pScan)
		{
			int iNumNonzero = 0, iFlex = m_pCodingContext.m_aModelAC.m_iFlcBits[0] - m_pCodingContext.m_iTrimFlexBits;

			if (iFlex < 0 || sbSubband == SUBBAND.SB_NO_FLEXBITS)
			{
				iFlex = 0;
			}

			if (bNoSkip) 
			{
				var iQP1 = pQuantizerHP.iQP << m_pCodingContext.m_aModelAC.m_iFlcBits[0];
				iNumNonzero = DecodeBlockHighpass(iQP1, ref pCoeffs, ref pScan);
			}
			if (iFlex != 0)
			{
				if (pQuantizerHP.iQP + m_pCodingContext.m_iTrimFlexBits == 1)
				{ // only iTrim = 0, pQuantizerHP.iQP = 1 is legal
					System.Diagnostics.Debug.Assert(m_pCodingContext.m_iTrimFlexBits == 0);
					System.Diagnostics.Debug.Assert(pQuantizerHP.iQP == 1);

					for (var k = 1; k < 16; k++)
					{
						if (pCoeffs[(uint)Constant.dctIndex[0][k]] < 0)
						{
							int fine = bitIO.GetBit16((uint)iFlex);
							pCoeffs[(uint)Constant.dctIndex[0][k]] -= fine;
						}
						else if (pCoeffs[(uint)Constant.dctIndex[0][k]] > 0)
						{
							int fine = bitIO.GetBit16((uint)iFlex);
							pCoeffs[(uint)Constant.dctIndex[0][k]] += fine;
						}
						else
						{
							var fine = bitIO.GetBit16s((uint)iFlex);
							pCoeffs[(uint)Constant.dctIndex[0][k]] = fine;
						}
					}
				}
				else
				{
					var iQP1 = pQuantizerHP.iQP << m_pCodingContext.m_iTrimFlexBits;
					for (var k = 1; k < 16; k++)
					{
						var kk = pCoeffs[(uint)Constant.dctIndex[0][k]];
						if (kk < 0)
						{
							int fine = bitIO.GetBit16((uint)iFlex);
							pCoeffs[(uint)Constant.dctIndex[0][k]] -= iQP1 * fine;
						}
						else if (kk > 0)
						{
							int fine = bitIO.GetBit16((uint)iFlex);
							pCoeffs[(uint)Constant.dctIndex[0][k]] += iQP1 * fine;
						}
						else
						{
							var fine = bitIO.GetBit16s((uint)iFlex);
							pCoeffs[(uint)Constant.dctIndex[0][k]] = iQP1 * fine;
						}
					}
				}
			}

			return iNumNonzero;
		}

		/*************************************************************************
			DecodeBlockHighpass : Combines DecodeBlock and InverseScanAdaptive
		*************************************************************************/
		private int DecodeBlockHighpass(int iQP, ref PointerArray pCoeffs, ref CCodingContext.CAdaptiveScan[] pScan)
		{
			var iNumNonzero = 1;

			uint iLoc = 1;
			int iIndex, iSign;

			/** first symbol **/
			DecodeFirstIndex(out iIndex, out iSign, m_pCodingContext.m_pAHexpt[Constant.CTDC + Constant.CONTEXTX + 0]);
			var iSR = (iIndex & 1);
			var iSRn = iIndex >> 2;

			var iCont = iSR & iSRn;
			var iLevel = (iQP ^ iSign) - iSign;
			if ((iIndex & 2) != 0 /* iSL */) {
				iLevel *= DecodeSignificantAbsLevel(m_pCodingContext.m_pAHexpt[6 + Constant.CTDC + Constant.CONTEXTX + iCont]);
			}
			if (iSR == 0) {
				iLoc += (uint)DecodeSignificantRun((int)(15 - iLoc), m_pCodingContext.m_pAHexpt[0]);
			}
			iLoc &= 0xf;
			pCoeffs[(uint)pScan[iLoc].uScan] = iLevel;
			pScan[iLoc].uTotal++;
			if ((iLoc != 0) && (pScan[iLoc].uTotal > pScan[iLoc - 1].uTotal))
			{
				var cTemp = pScan[iLoc];
				pScan[iLoc] = pScan[iLoc - 1];
				pScan[iLoc - 1] = cTemp;
			}
			iLoc = (iLoc + 1) & 0xf;

			while (iSRn != 0) {
				iSR = iSRn & 1;
				if (iSR == 0) {
					iLoc += (uint)DecodeSignificantRun((int)(15 - iLoc), m_pCodingContext.m_pAHexpt[0]);
					if (iLoc >= 16)
						return 16;
				}
				DecodeIndex(out iIndex, out iSign, (int)(iLoc + 1), m_pCodingContext.m_pAHexpt[Constant.CTDC + Constant.CONTEXTX + iCont + 1]);
				iSRn = iIndex >> 1;

				System.Diagnostics.Debug.Assert(iSRn >= 0 && iSRn < 3);
				iCont &= iSRn;  /** huge difference! **/
				iLevel = (iQP ^ iSign) - iSign;
				if ((iIndex & 1) != 0 /* iSL */)
				{
					iLevel *= DecodeSignificantAbsLevel(m_pCodingContext.m_pAHexpt[6 + Constant.CTDC + Constant.CONTEXTX + iCont]);
				}
		       
				pCoeffs[(uint)pScan[iLoc].uScan] = iLevel;
				pScan[iLoc].uTotal++;
				if ((iLoc != 0) && (pScan[iLoc].uTotal > pScan[iLoc - 1].uTotal))
				{
					var cTemp = pScan[iLoc];
					pScan[iLoc] = pScan[iLoc - 1];
					pScan[iLoc - 1] = cTemp;
				}

				iLoc = (iLoc + 1) & 0xf;
				iNumNonzero++;
			}
			return iNumNonzero;
		}

		/* CBP prediction for 16 x 16 MB */
		/* block index */
		/*  0  1  4  5 */
		/*  2  3  6  7 */
		/*  8  9 12 13 */
		/* 10 11 14 15 */
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "cbp", Justification = "To simplify comparizons with the original code")]
		private int PredCBPCDec(int cbp, CCodingContext.CCBPModel pModel)
		{
		    const int iNDiff = Constant.AVG_NDIFF;

			if (pModel.m_iState[0] == 0) {
				if(m_bCtxLeft) {
					if (m_bCtxTop) {
						cbp ^= 1;
					}
					else {
						var iTopCBP = PredInfoPrevRow[cColumn].iCBP;
						cbp ^= (iTopCBP >> 10) & 1; // left: top(10) => 0
					}
				}
				else {
					var iLeftCBP = PredInfo[cColumn - 1].iCBP;
					cbp ^= ((iLeftCBP >> 5) & 1); // left(5) => 0
				}

				cbp ^= (0x02 & (cbp << 1)); // 0 => 1
				cbp ^= (0x10 & (cbp << 3)); // 1 => 4
				cbp ^= (0x20 & (cbp << 1)); // 4 => 5

				cbp ^= ((cbp & 0x33) << 2);
				cbp ^= ((cbp & 0xcc) << 6);
				cbp ^= ((cbp & 0x3300) << 2);

			}
			else if (pModel.m_iState[0] == 2) {
				cbp ^= 0xffff;
			}

			int iNOrig = NumOnes(cbp);

			pModel.m_iCount0[0] += iNOrig - iNDiff;
			pModel.m_iCount0[0] = Saturate32(pModel.m_iCount0[0]);

			pModel.m_iCount1[0] += 16 - iNOrig - iNDiff;
			pModel.m_iCount1[0] = Saturate32(pModel.m_iCount1[0]);

			if (pModel.m_iCount0[0] < 0) {
				if (pModel.m_iCount0[0] < pModel.m_iCount1[0]) {
					pModel.m_iState[0] = 1;
				}
				else {
					pModel.m_iState[0] = 2;
				}
			}
			else if (pModel.m_iCount1[0] < 0) {
				pModel.m_iState[0] = 2;
			}
			else {
				pModel.m_iState[0] = 0;
			}
			return cbp;
		}

		/*************************************************************************
			DecodeCBP
		*************************************************************************/
		private void DecodeCBP()
		{
			var iCBPCY = 0;
			var iNumCBP = m_pCodingContext.m_pAdaptHuffCBPCY1.GetHuffShort(bitIO);

			m_pCodingContext.m_pAdaptHuffCBPCY1.m_iDiscriminant += m_pCodingContext.m_pAdaptHuffCBPCY1.m_pDelta[(uint)iNumCBP];

			switch (iNumCBP) {
			    case 2:
			        iNumCBP = bitIO.GetBit16(2);
			        if (iNumCBP == 0)
			            iNumCBP = 3;
			        else if (iNumCBP == 1)
			            iNumCBP = 5;
			        else {
						iNumCBP = Constant.aTab[iNumCBP * 2 + bitIO.GetBit16(1) - 4];
			        }
			        break;
			    case 1:
					iNumCBP = 1 << bitIO.GetBit16(2);
			        break;
			    case 3:
					iNumCBP = 0xf ^ (1 << bitIO.GetBit16(2));
			        break;
			    case 4:
			        iNumCBP = 0xf;
					break;
			}

			for (var iBlock = 0; iBlock < 4; iBlock++)
			{
			    if ((iNumCBP & (1 << iBlock)) != 0)
				{
			        var iNumBlockCBP =  m_pCodingContext.m_pAdaptHuffCBPCY.GetHuffShort(bitIO);
			        var val = (uint)iNumBlockCBP + 1/*, iCode1*/;

					m_pCodingContext.m_pAdaptHuffCBPCY.m_iDiscriminant += m_pCodingContext.m_pAdaptHuffCBPCY.m_pDelta[(uint)iNumBlockCBP];
			        iNumBlockCBP = 0;

					if (val >= 6)
					{ // chroma present
						System.Diagnostics.Debug.Assert(false); // just checking if this is ever called
					}
					var iCode1 = Constant.gOff0[val];
					if (Constant.gFLC0[val] != 0)
					{
						iCode1 += bitIO.GetBit16(Constant.gFLC0[val]);
					}
					iNumBlockCBP += (int)Constant.gOut0[iCode1];

			        iCBPCY |= (iNumBlockCBP << (iBlock * 4));
			    }
			}

			iDiffCBP = iCBPCY;
		}

		private void DequantizeMacroblock()
		{
			//dequantize DC
			p1MBbuffer[0] = iBlockDC[0] * pQuantizerDC.iQP;

			// dequantize LP
			if (sbSubband != SUBBAND.SB_DC_ONLY)
			{
				for (var i = 1; i < 16; i++)
				{
					p1MBbuffer[(uint)Constant.dctIndex[2][i]] = iBlockDC[i] * pQuantizerLP.iQP;
				}
			}
		}

		/* frequency domain inverse DCAC prediction */
		private void PredDCACDec()
		{
			var iDCACPredMode = GetDCACPredMode(cColumn);
			var iDCPredMode = (iDCACPredMode & 0x3);
			var iADPredMode = (iDCACPredMode & 0xC);

			// TODO macbork: remove some pointers
			var pOrg = iBlockDC;// current DC block

			/* DC prediction */
			if(iDCPredMode == 1){ // predict DC from top
			    pOrg[0] += PredInfoPrevRow[cColumn].iDC;
			}
			else if(iDCPredMode == 0){ // predict DC from left
			    pOrg[0] += PredInfo[cColumn - 1].iDC;
			}
			else if(iDCPredMode == 2){// predict DC from top&left
			    pOrg[0] += (PredInfo[cColumn - 1].iDC + PredInfoPrevRow[cColumn].iDC) >> 1;
			}

			/* AD prediction */
			if(iADPredMode == 4)
			{// predict AD from top
			    var pRef = PredInfoPrevRow[cColumn].iAD;
				pOrg[4] += pRef[3];
				pOrg[8] += pRef[4];
				pOrg[12] += pRef[5];
			}
			else 
				if(iADPredMode == 0)
				{// predict AD from left
					var pRef = PredInfo[cColumn - 1].iAD;
					pOrg[1] += pRef[0];
					pOrg[2] += pRef[1];
					pOrg[3] += pRef[2];
				}

			iOrientation = 2 - GetACPredMode();
		}

		/* get DCAC prediction mode: 0(from left) 1(from top) 2(none) */
		private int GetDCACPredMode(int mbX)
		{
			int iDCMode, iADMode = 2;  // DC: 0(left) 1(top) 2(mean) 3(no)
									   // AD: 0(left) 1(top) 2(no)

			if (m_bCtxLeft && m_bCtxTop)
			{ // topleft corner, no prediction
			    iDCMode = 3;
			}
			else 
			    if(m_bCtxLeft)
			    {
			        iDCMode = 1; // left column, predict from top
			    }
			    else 
			        if (m_bCtxTop)
			        {
			            iDCMode = 0; // top row, predict from left
			        }
			        else{
			            int iL = PredInfo[mbX - 1].iDC, iT = PredInfoPrevRow[mbX].iDC, iTL = PredInfoPrevRow[mbX - 1].iDC;

					    var StrH = Math.Abs(iTL - iL);
					    var StrV = Math.Abs(iTL - iT);
						iDCMode = (StrH * Constant.ORIENT_WEIGHT < StrV ? 1 : (StrV * Constant.ORIENT_WEIGHT < StrH ? 0 : 2));
			        }

			if (iDCMode == 1 && 0 == PredInfoPrevRow[mbX].iQPIndex)
				iADMode = 1;
			if (iDCMode == 0 && 0 == PredInfo[mbX - 1].iQPIndex)
				iADMode = 0;

			return (iDCMode + (iADMode << 2));
		}

		/* get AC prediction mode: 0(from left) 1(from top) 2(none) */
		private int GetACPredMode()
		{
			var StrH = Math.Abs(iBlockDC[1]) + Math.Abs(iBlockDC[2]) + Math.Abs(iBlockDC[3]);
			var StrV = Math.Abs(iBlockDC[4]) + Math.Abs(iBlockDC[8]) + Math.Abs(iBlockDC[12]);

			return (StrH * Constant.ORIENT_WEIGHT < StrV ? 1 : (StrV * Constant.ORIENT_WEIGHT < StrH ? 0 : 2));
		}

		/*************************************************************************
			DecodeSecondStageCoeff
		*************************************************************************/
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "cbp", Justification = "To simplify comparizons with the original code")]
		private void DecodeMacroblockLowpass()
		{
			var pScan = m_pCodingContext.m_aScanLowpass;
		    var iModelBits = m_pCodingContext.m_aModelLP.m_iFlcBits[0];
			var aRLCoeffs = new int[32];
		    var aLaplacianMean = new int[2];
		    var cbp = 0;
		    
			/** reset adaptive scan totals **/
			if (m_bResetRGITotals)
			{
				var iWeight = 2 * 16;
			
				pScan[0].uTotal = Constant.MAXTOTAL;
				for (var k = 1; k < 16; k++)
				{					
					pScan[k].uTotal = iWeight;
					iWeight -= 2;
				}
			}

		    cbp |= bitIO.GetBit16(1);

	        var pCoeffs = iBlockDC;

	        if ((cbp & 1) != 0) 
			{
	            int iNumNonzero = DecodeBlock(aRLCoeffs);

			    aLaplacianMean[0] += iNumNonzero;
			    int iIndex = 1;

			    for (var k = 0; k < iNumNonzero; k++)
				{
			        iIndex += aRLCoeffs[k * 2];
					pCoeffs[pScan[iIndex].uScan] = aRLCoeffs[k * 2 + 1];
			        pScan[iIndex].uTotal++;
			        if (pScan[iIndex].uTotal > pScan[iIndex - 1].uTotal)
					{
			            var cTemp = pScan[iIndex];
			            pScan[iIndex] = pScan[iIndex - 1];
			            pScan[iIndex - 1] = cTemp;
			        }
			        iIndex++;
			    }
			}

			if (iModelBits != 0)
			{
				var iMask = (1 << iModelBits) - 1;
				for (var k = 1; k < 16; k++)
				{
					if (pCoeffs[k] != 0)
					{
						var r1 = RotL((uint)pCoeffs[k], iModelBits);
						pCoeffs[k] = ((int)r1 ^ bitIO.GetBit16((uint)iModelBits)) - ((int)r1 & iMask);
					}
					else
					{
						int r1 = bitIO.PeekBit16((uint)iModelBits + 1);
						pCoeffs[k] = ((r1 >> 1) ^ (-(r1 & 1))) + (r1 & 1);
						bitIO.GetBit16((uint)(iModelBits + ((pCoeffs[k] != 0) ? 1 : 0)));
					}
				}
			}

			UpdateModelMB(aLaplacianMean, m_pCodingContext.m_aModelLP);

			if (m_bResetContext)
			{
				AdaptLowpassDec();
			}
		}

		/*************************************************************************
			UpdateModelMB : update adaptive model at end of macroblock
			(for lowest resolution only)
		*************************************************************************/
		private static void UpdateModelMB(int[] iLaplacianMean, CAdaptiveModel pModel)
		{
			iLaplacianMean[0] *= Constant.aWeight0[pModel.m_band - CAdaptiveModel.BAND.BAND_DC];
			iLaplacianMean[1] *= Constant.aWeight1[pModel.m_band - CAdaptiveModel.BAND.BAND_DC][0];
			if (pModel.m_band == CAdaptiveModel.BAND.BAND_AC)
			{
				iLaplacianMean[1] >>= 4;
			}

			const int j = 0;
			var iLM = iLaplacianMean[j];
			var iMS = pModel.m_iFlcState[j];
			var iDelta = (iLM - Constant.MODELWEIGHT) >> 2;

			if (iDelta <= -8)
			{
				iDelta += 4;
				if (iDelta < -16)
					iDelta = -16;
				iMS += iDelta;
				if (iMS < -8) {
					if (pModel.m_iFlcBits[j] == 0)
						iMS = -8;
					else {
						iMS = 0;
						pModel.m_iFlcBits[j]--;
					}
				}
			}
			else if (iDelta >= 8)
			{
				iDelta -= 4;
				if (iDelta > 15)
					iDelta = 15;
				iMS += iDelta;
				if (iMS > 8) {
					if (pModel.m_iFlcBits[j] >= 15)
					{
						pModel.m_iFlcBits[j] = 15;
						iMS = 8;
					}
					else
					{
						iMS = 0;
						pModel.m_iFlcBits[j]++;
					}
				}
			}
			pModel.m_iFlcState[j] = iMS;
		}

		private int DecodeBlock(int[] aLocalCoef)
		{
			var iLocation = 1;
			var iNumNonzero = 1;

			int iIndex, iSign;
			//** first symbol **/
			DecodeFirstIndex (out iIndex, out iSign, m_pCodingContext.m_pAHexpt[Constant.CTDC+0]);
			var iSR = iIndex & 1;
			var iSRn = iIndex >> 2;

			var iCont = iSR & iSRn;
			if ((iIndex & 2) != 0 /* iSL */)
			{
				aLocalCoef[1] = (DecodeSignificantAbsLevel(m_pCodingContext.m_pAHexpt[6 + Constant.CTDC + iCont]) ^ iSign) - iSign;
			}
			else
			{
				aLocalCoef[1] = 1 | iSign; // 0 -> 1; -1 -> -1
			}
			aLocalCoef[0] = 0;
			if (iSR == 0)
			{
				aLocalCoef[0] = DecodeSignificantRun(15 - iLocation, m_pCodingContext.m_pAHexpt[0]);
			}
			iLocation += aLocalCoef[0] + 1;

			while (iSRn != 0)
			{
				iSR = iSRn & 1;
				aLocalCoef[iNumNonzero * 2] = 0;
				if (iSR == 0)
				{
					aLocalCoef[iNumNonzero * 2] = DecodeSignificantRun(15 - iLocation, m_pCodingContext.m_pAHexpt[0]);
				}
				iLocation += aLocalCoef[iNumNonzero * 2] + 1;
				DecodeIndex(out iIndex, out iSign, iLocation, m_pCodingContext.m_pAHexpt[Constant.CTDC + iCont + 1]);
				iSRn = iIndex >> 1;

				System.Diagnostics.Debug.Assert(iSRn >= 0 && iSRn < 3);
				iCont &= iSRn;  /** huge difference! **/
				if ((iIndex & 1) != 0 /* iSL */)
				{
					aLocalCoef[iNumNonzero * 2 + 1] =
						(DecodeSignificantAbsLevel(m_pCodingContext.m_pAHexpt[6 + Constant.CTDC + iCont]) ^ iSign) - iSign;
				}
				else
				{
					aLocalCoef[iNumNonzero * 2 + 1] = (1 | iSign); // 0 -> 1; -1 -> -1 (was 1 + (iSign * 2))
				}
				iNumNonzero++;
			}
			return iNumNonzero;
		}

		/*************************************************************************
			Experimental code -- decodeBlock
			SR = <0 1 2> == <last, nonsignificant, significant run>
			alphabet 12:
				pAHexpt[0] == <SR', SL, SR | first symbol>
			alphabet 6:
				pAHexpt[1] == <SR', SL | continuous>
				pAHexpt[2] == <SR', SL | continuous>
			alphabet 4:
				pAHexpt[3] == <SR', SL | 2 free slots> (SR may be last or insignificant only)
			alphabet f(run) (this can be extended to 6 contexts - SL and SR')
				pAHexpt[4] == <run | continuous>
			alphabet f(lev) (this can be extended to 9 contexts)
				pAHexpt[5-6] == <lev | continuous> first symbol
				pAHexpt[7-8] == <lev | continuous> condition on SRn no use
		*************************************************************************/
		private int DecodeSignificantRun(int iMaxRun, CAdaptiveHuffman pAHexpt)
		{
			if (iMaxRun < 5)
			{
				if (iMaxRun == 1 || bitIO.GetBool16())
				{
					return 1;
				}
			    if (iMaxRun == 2 || bitIO.GetBool16())
			    {
			        return 2;
			    }
			    if (iMaxRun == 3 || bitIO.GetBool16())
			    {
			        return 3;
			    }
			    return 4;
			}

			var iIndex = pAHexpt.GetHuffShort(bitIO);
			//this always uses table 0
			iIndex += Constant.gSignificantRunBin[iMaxRun] * 5;
			var iRun = Constant.aRemap2[iIndex];
			var iFLC = Constant.gSignificantRunFixedLength[iIndex];
			if (iFLC != 0)
			{
				iRun += bitIO.GetBit16(iFLC);
			}
			
			return iRun;
		}

		private void DecodeFirstIndex(out int pIndex, out int pSign, CAdaptiveHuffman pAHexpt)
		{
			pIndex = pAHexpt.GetHuff(bitIO);
			pAHexpt.m_iDiscriminant  += pAHexpt.m_pDelta[(uint)pIndex];
			pAHexpt.m_iDiscriminant1 += pAHexpt.m_pDelta1[(uint)pIndex];
			pSign = -bitIO.GetBit16(1);
		}

		private void DecodeIndex(out int pIndex, out int pSign, int iLoc, CAdaptiveHuffman pAHexpt)
		{
			if (iLoc < 15)
			{
				pIndex = pAHexpt.GetHuffShort(bitIO);
				pAHexpt.m_iDiscriminant  += pAHexpt.m_pDelta[(uint)pIndex];
				pAHexpt.m_iDiscriminant1 += pAHexpt.m_pDelta1[(uint)pIndex];
				pSign = -bitIO.GetBit16(1);
			}
			else 
				if (iLoc == 15)
				{
					if (!bitIO.GetBool16())
					{
						pIndex = 0;
					}
					else
						if (!bitIO.GetBool16())
						{
							pIndex = 2;
						}
						else
						{
							pIndex = 1 + 2 * bitIO.GetBit16(1);
						}
					pSign = -bitIO.GetBit16(1);
				}
			else { //if (iLoc == 16) { /* deterministic */
				int iSL = bitIO.GetBit16(1 + 1);
				pIndex = iSL >> 1;
				pSign = -(iSL & 1);
			}
		}

		/*************************************************************************
			8 bit YUV 420 macroblock decode function with 4x4 transform
			Index order is as follows:
			Y:              U:      V:
			 0  1  4  5     16 17   20 21
			 2  3  6  7     18 19   22 23
			 8  9 12 13
			10 11 14 15

			DCAC coefficients stored for 4x4 - offsets (x == no storage)
			Y:
			x x x [0..3]
			x x x [4..7]
			x x x [8..11]
			[16..19] [20..23] [24..27] [28..31,12..15]

			U, V:
			x [0..3]
			[8..11] [4..7,12..15]
		*************************************************************************/
		private void DecodeMacroblockDC()
		{
			var aLaplacianMean = new int[2];
			var iModelBits = m_pCodingContext.m_aModelDC.m_iFlcBits[0];

			for (var i = 0; i < 16; i++)
			{
				iBlockDC[i] = 0;
			}

			if (cfColorFormat == COLORFORMAT.Y_ONLY || cfColorFormat == COLORFORMAT.CMYK || cfColorFormat == COLORFORMAT.N_CHANNEL)
			{
			    var iQDCY = 0;
			    /** get luminance DC **/
			    if (bitIO.GetBit16(Constant.IsDCChNumBits) == 1)
				{
					iQDCY = DecodeSignificantAbsLevel(m_pCodingContext.m_pAHexpt[3]) - 1;
			        aLaplacianMean[0] += 1;
			    }
			    if (iModelBits != 0) {
			        iQDCY = (iQDCY << iModelBits) | bitIO.GetBit16((uint)iModelBits);
			    }
				if ((iQDCY != 0) && bitIO.GetBool16())
				{
					iQDCY = -iQDCY;
				}
			    iBlockDC[0] = iQDCY;
			}

			UpdateModelMB(cfColorFormat, (int)cNumChannels, aLaplacianMean, m_pCodingContext.m_aModelDC);
		}

		/*************************************************************************
			UpdateModelMB : update adaptive model at end of macroblock
			(for lowest resolution only)
		*************************************************************************/
		private static void UpdateModelMB(COLORFORMAT cf, int iChannels, int[] iLaplacianMean, 
																			CAdaptiveModel pModel)
		{
			iLaplacianMean[0] *= Constant.aWeight0[pModel.m_band - CAdaptiveModel.BAND.BAND_DC];
			if (cf == COLORFORMAT.YUV_420)
			{
				iLaplacianMean[1] *= Constant.aWeight2[pModel.m_band - CAdaptiveModel.BAND.BAND_DC];
			}
			else if (cf == COLORFORMAT.YUV_422)
			{
				iLaplacianMean[1] *= Constant.aWeight2[3 + (pModel.m_band) - CAdaptiveModel.BAND.BAND_DC];
			}
			else
			{
				iLaplacianMean[1] *= Constant.aWeight1[pModel.m_band - CAdaptiveModel.BAND.BAND_DC][iChannels - 1];
				if (pModel.m_band == CAdaptiveModel.BAND.BAND_AC)
					iLaplacianMean[1] >>= 4;
			}

			for (var j = 0; j < 2; j++)
			{
				var iLM = iLaplacianMean[j];
				var iMS = pModel.m_iFlcState[j];
				var iDelta = (iLM - Constant.MODELWEIGHT) >> 2;

				if (iDelta <= -8)
				{
					iDelta += 4;
					if (iDelta < -16)
						iDelta = -16;
					iMS += iDelta;
					if (iMS < -8)
					{
						if (pModel.m_iFlcBits[j] == 0)
							iMS = -8;
						else
						{
							iMS = 0;
							pModel.m_iFlcBits[j]--;
						}
					}
				}
				else if (iDelta >= 8)
				{
					iDelta -= 4;
					if (iDelta > 15)
						iDelta = 15;
					iMS += iDelta;
					if (iMS > 8)
					{
						if (pModel.m_iFlcBits[j] >= 15)
						{
							pModel.m_iFlcBits[j] = 15;
							iMS = 8;
						}
						else
						{
							iMS = 0;
							pModel.m_iFlcBits[j]++;
						}
					}
				}
				pModel.m_iFlcState[j] = iMS;
				if (cf == COLORFORMAT.Y_ONLY)
					break;
			}
		}
		
		private int DecodeSignificantAbsLevel(CAdaptiveHuffman pAHexpt)
		{
			int iFixed, iLevel;

			var iIndex = (uint)pAHexpt.GetHuff(bitIO);
			System.Diagnostics.Debug.Assert( iIndex <= 6 );

			pAHexpt.m_iDiscriminant += pAHexpt.m_pDelta[iIndex];
			if (iIndex < 2)
			{
				iLevel = (int)(iIndex + 2);
			}
			else if (iIndex < 6)
			{
				iFixed = (int)Constant.aFixedLength[iIndex];
				iLevel = (int)Constant.aRemap[iIndex] + bitIO.GetBit16((uint)iFixed);
			}
			else
			{
				iFixed = bitIO.GetBit16(4) + 4;
				if (iFixed == 19)
				{
					iFixed += bitIO.GetBit16(2);
					if (iFixed == 22)
					{
						iFixed += bitIO.GetBit16(3);
					}
				}
				iLevel = 2 + (1 << (byte)iFixed);
				iIndex = bitIO.GetBit32((uint)iFixed);
				iLevel += (int)iIndex;
			}

			return iLevel;
		}

		private void ReadPackets()
		{
			if (cColumn == 0 && cRow == 0)		// start of a new horizontal slice
			{
				ReadPacketHeader();
				
				m_pCodingContext.m_iTrimFlexBits = bTrimFlexbitsFlag ? bitIO.GetBit16(4) : 0;

				// reset coding contexts
				ResetCodingContextDec();
			}
		}

		private void ResetCodingContextDec()
		{
			/** set flags **/
			m_pCodingContext.m_pAdaptHuffCBPCY.m_bInitialize = false;
			m_pCodingContext.m_pAdaptHuffCBPCY1.m_bInitialize = false;
			for (var k = 0; k < Constant.NUMVLCTABLES; k++)
				m_pCodingContext.m_pAHexpt[k].m_bInitialize = false;

			// reset VLC tables
			AdaptLowpassDec();
			AdaptHighpassDec();

			// reset zigzag patterns, totals
			InitZigzagScan();
			// reset bit reduction and cbp models
			m_pCodingContext.ResetCodingContext();
		}
	
		/*************************************************************************
			Initialize zigzag scan parameters
		*************************************************************************/
		private void InitZigzagScan()
		{
			if (null != m_pCodingContext)
			{
				for (var i = 0; i < 16; i++)
				{
					m_pCodingContext.m_aScanLowpass[i].uScan = Constant.grgiZigzagInv4x4_lowpass[i];
					m_pCodingContext.m_aScanHoriz[i].uScan = Constant.dctIndex[0][Constant.grgiZigzagInv4x4H[i]];
					m_pCodingContext.m_aScanVert[i].uScan = Constant.dctIndex[0][Constant.grgiZigzagInv4x4V[i]];
				}
			}
		}

		private void AdaptHighpassDec()
		{
			m_pCodingContext.m_pAdaptHuffCBPCY.AdaptDiscriminant();
			m_pCodingContext.m_pAdaptHuffCBPCY1.AdaptDiscriminant();

			for (var kk = 0; kk < Constant.CONTEXTX; kk++)
			{
				m_pCodingContext.m_pAHexpt[kk + Constant.CONTEXTX + Constant.CTDC].AdaptDiscriminant();
			}
		}

		/*************************************************************************
			Adapt
		*************************************************************************/
		private void AdaptLowpassDec()
		{
			for (var kk = 0; kk < Constant.CONTEXTX + Constant.CTDC; kk++)
			{
				m_pCodingContext.m_pAHexpt[kk].AdaptDiscriminant();
			}
		}

		// packet header: 00000000 00000000 00000001 ?????xxx
		// xxx:           000(spatial) 001(DC) 010(AD) 011(AC) 100(FL) 101-111(reserved)
		// ?????:         (iTileY * cNumOfSliceV + iTileX) % 32
		private void ReadPacketHeader()
		{
			var b = bitIO.NextByte();
			System.Diagnostics.Debug.Assert(b == 0x0);
			if (b != 0x0)
			{
				throw new ArgumentOutOfRangeException("ReadPacketHeader error " + b);
			}

			b = bitIO.NextByte();
			System.Diagnostics.Debug.Assert(b == 0x0);
			if (b != 0x0)
			{
				throw new ArgumentOutOfRangeException("ReadPacketHeader error " + b);
			}

			b = bitIO.NextByte();
			System.Diagnostics.Debug.Assert(b == 0x1);
			if (b != 0x1)
			{
				throw new ArgumentOutOfRangeException("ReadPacketHeader error " + b);
			}

			b = bitIO.NextByte();
			System.Diagnostics.Debug.Assert(b == 0x0);
			if (b != 0x0)
			{
				throw new ArgumentOutOfRangeException("ReadPacketHeader error " + b);
			}
		}

		private void GetTilePos(int mbX, int mbY)
		{
			if (mbX == 0)
			{ // left image boundary
				cTileColumn = 0;
			}

			m_bCtxLeft = mbX == 0;
			m_bCtxTop = mbY == 0;

			m_bResetContext = m_bResetRGITotals = (((mbX - 0) & 0xf) == 0);
			if (cTileColumn == cNumOfSliceMinus1V)	// cNumOfSliceMinus1V is alwyas 0 for DEM
			{ // last tile column
				if (mbX + 1 == cmbWidth)
				{
					m_bResetContext = true;
				}
			}
			else
			{
				if (mbX + 1 == 0)
				{
					m_bResetContext = true;
				}
			}
		}

		private void AdvanceMRPtr()
		{
			p0MBbuffer.IncrementPointer(16 * 16);
			p1MBbuffer.IncrementPointer(16 * 16);
		}

		private void InitMRPtr()
		{
			// assuming only one image plane
			p0MBbuffer = new PointerArray(a0MBbuffer, 0);
			p1MBbuffer = new PointerArray(a1MBbuffer, 0);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body")]
		private void ImageStrDecInit()
		{
			InitializeStrDec();
			
			a0MBbuffer = new PointerArray(16 * 16 * cmbWidth, 0);
			a1MBbuffer = new PointerArray(16 * 16 * cmbWidth, 0);

			StrIODecInit();
			StrDecInit();

			decompressedBits = new short[cWidth,cHeight];
		}

		private void InitializeStrDec()
		{
			cmbWidth = ((int)cWidth + 15) / 16;
			cmbHeight = ((int)cHeight + 15) / 16;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void StrIODecInit()
		{
			if (bitIO.NextByte() != 0xff)
			{
				throw new ArgumentOutOfRangeException("Corrupted bitstream in StrIODecInit!");
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		private void StrDecInit()
		{
			AllocatePredInfo();

			if ((uQPMode & 1) == 0)
			{ // DC frame uniform quantization
				pQuantizerDC = new CWMIQuantizer {iIndex = uiQPIndexDC};
			    FormatQuantizer(ref pQuantizerDC);
			}
			if (sbSubband != SUBBAND.SB_DC_ONLY)
			{
				if ((uQPMode & 2) == 0)
				{ // LP frame uniform quantization
					pQuantizerLP = new CWMIQuantizer();
					if ((uQPMode & 0x200) == 0) // use DC quantizer
					{
						throw new ArgumentOutOfRangeException("StrDecInit 1");
					}
				    pQuantizerLP.iIndex = uiQPIndexLP;
				    FormatQuantizer(ref pQuantizerLP);
				}

				if (sbSubband != SUBBAND.SB_NO_HIGHPASS)
				{
					if ((uQPMode & 4) == 0)
					{ // HP frame uniform quantization
						pQuantizerHP = new CWMIQuantizer();

						if ((uQPMode & 0x400) == 0) // use LP quantizer
						{
							throw new ArgumentOutOfRangeException("StrDecInit 2");
						}
					    pQuantizerHP.iIndex = uiQPIndexHP;
					    FormatQuantizer(ref pQuantizerHP);
					}
				}
			}

			AllocateCodingContextDec();
		}

		private void FormatQuantizer(ref CWMIQuantizer pQuantizer)
		{
			RemapQP(ref pQuantizer, Constant.SHIFTZERO);
		}

		/*************************************************************************
			QPRemapping
		*************************************************************************/
		private void RemapQP(ref CWMIQuantizer pQP, int iShift)
		{
			var uiQPIndex = pQP.iIndex;

			if(uiQPIndex == 0) 
			{
				throw new ArgumentOutOfRangeException("pQP", "uiQPIndex = " + uiQPIndex);
			}
		    if (!bScaledArith)
		    {
		        throw new ArgumentOutOfRangeException("pQP", "bScaledArith = " + bScaledArith);
		    }
		    int man, exp;

		    if (pQP.iIndex < 16)
		    {
		        man = pQP.iIndex;
		        exp = iShift;
		    }
		    else
		    {
		        man = 16 + (pQP.iIndex & 0xf);
		        exp = ((pQP.iIndex >> 4) - 1) + iShift;
		    }
		        
		    pQP.iQP = man << exp;
		    pQP.iMan = Constant.gs_QPRecipTable[man].iMan;
		    pQP.iExp = Constant.gs_QPRecipTable[man].iExp + exp;
		    pQP.iOffset = ((pQP.iQP * 3 + 1) >> 3);
		}

		private void AllocatePredInfo()
		{
			PredInfo = new CWMIPredInfo[cmbWidth];
			PredInfoPrevRow = new CWMIPredInfo[cmbWidth];

			for (var i = 0; i < cmbWidth; i++)
			{
				PredInfo[i] = new CWMIPredInfo();
				PredInfoPrevRow[i] = new CWMIPredInfo();
			}
		}

		private void AllocateCodingContextDec()
		{
			m_pCodingContext = new CCodingContext {m_pAHexpt = new CAdaptiveHuffman[Constant.NUMVLCTABLES]};

		    var iCBPSize = (cfColorFormat == COLORFORMAT.Y_ONLY || cfColorFormat == COLORFORMAT.N_CHANNEL || 
							cfColorFormat == COLORFORMAT.CMYK) ? 5 : 9;

			/** allocate adaptive Huffman encoder **/
			m_pCodingContext.m_pAdaptHuffCBPCY = new CAdaptiveHuffman();
			m_pCodingContext.m_pAdaptHuffCBPCY.InitializeAH(iCBPSize);
			
			m_pCodingContext.m_pAdaptHuffCBPCY1 = new CAdaptiveHuffman();
			m_pCodingContext.m_pAdaptHuffCBPCY1.InitializeAH(5);

			for (var k = 0; k < Constant.NUMVLCTABLES; k++)
			{
				m_pCodingContext.m_pAHexpt[k] = new CAdaptiveHuffman();
				m_pCodingContext.m_pAHexpt[k].InitializeAH((int)Constant.aAlphabet[k]);
			}

			ResetCodingContextDec();
		}
		#endregion

	}
}
