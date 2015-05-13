//------------------------------------------------------------------------------
// BitstreamDecoder.cs
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
	internal class BitstreamDecoder : ContainerDecoder
	{
		#region Constants
		private struct Constant
		{
			// WMI Header
			/// <summary>
			/// GDI Signature: "WMPHOTO".
			/// </summary>
			internal static readonly byte[] GDISignature = new byte[] { 0x57, 0x4D, 0x50, 0x48, 0x4F, 0x54, 0x4F, 0x00 };
			internal const uint GDISignatureOffset = 0;

			/// <summary>
			/// Codec version. Only least sgnificant 4 bits are important
			/// </summary>
			internal const byte CodecVersion					= 1;

			// Sizes of fields, see page 33
			internal const byte CodecVersionNumBits			= 4;
			internal const byte SubVersionNumBits				= 4;
			internal const byte TilingFlagNumBits				= 1;
			internal const byte BitStreamFormatNumBits		= 1;
			internal const byte OriantationNumBits			= 3;
			internal const byte IndexTablePresentFlagNumBits	= 1;
			internal const byte OverlapNumBits				= 2;

			internal const byte ShortHeaderFlagNumBits		= 1;
			internal const byte WindowingFlagNumBits			= 1;
			internal const byte TrimFlexbitsFlagNumBits		= 1;
			internal const byte TileStretchFlagNumBits		= 1;
			internal const byte ShortHeaderNumBits			= 16;
			internal const byte LongHeaderNumBits				= 32;
			internal const byte SourceBitDepthNumBits			= 4;

			// Image Plane Header
			internal const byte ColorFormatNumBits			= 3;
			internal const byte NoScaledFlagNumBits			= 1;
			internal const byte BandsPresentNumBits			= 4;
			internal const byte DCFrameUniformNumBits			= 1;
			internal const byte DCQuantNumBits				= 8;
			internal const byte UseDCQuantizerNumBits			= 1;
			internal const byte LPFrameUniformNumBits			= 1;
			internal const byte UseLPQuantizerNumBits			= 1;
			internal const byte HPFrameUniformNumBits			= 1;
		}

		protected enum BITSTREAMFORMAT
		{
			SPATIAL = 0,     // spatial order
			FREQUENCY,       // frequency order
		};

		// rotation and flip
		private enum ORIENTATION
		{
			// CRW: Clock Wise 90% Rotation; FlipH: Flip Horizontally;  FlipV: Flip Vertically
			// Peform rotation FIRST!
			//                CRW FlipH FlipV
			O_NONE = 0,    // 0    0     0
			O_FLIPV,       // 0    0     1
			O_FLIPH,       // 0    1     0
			O_FLIPVH,      // 0    1     1
			O_RCW,         // 1    0     0
			O_RCW_FLIPV,   // 1    0     1
			O_RCW_FLIPH,   // 1    1     0
			O_RCW_FLIPVH,  // 1    1     1
			/* add new ORIENTATION here */
			O_MAX
		};

		internal enum OVERLAP
		{
			OL_NONE = 0,
			OL_ONE,
			OL_TWO,

			/* add new OVERLAP here */
			OL_MAX
		};

		protected enum SUBBAND {
			SB_ALL = 0,             // keep all subbands
			SB_NO_FLEXBITS,     // skip flex bits
			SB_NO_HIGHPASS,     // skip highpass
			SB_DC_ONLY,         // skip lowpass and highpass, DC only
			SB_ISOLATED,        // not decodable
			/* add new SUBBAND here */ SB_MAX
		};

		#endregion

		#region Fields

		// WMI Header
		private bool bTilingPresent, bInscribed, bTileStretch, bAbbreviatedHeader;
		protected BITSTREAMFORMAT bfBitstreamFormat; // bitstream layout
		protected bool bScaledArith; // lossless mode
		protected OVERLAP olOverlap;
		protected BITDEPTH_BITS bdBitDepth;
		protected uint cWidth, cHeight;
		private uint cExtraPixelsTop, cExtraPixelsBottom, cExtraPixelsLeft, cExtraPixelsRight;
		protected bool bTrimFlexbitsFlag;

		// Image Plane Header
		protected COLORFORMAT cfColorFormat; // color format
		protected SUBBAND sbSubband; // subbands
		protected uint uQPMode;      // 0/1: no dquant/with dquant, first bit for DC, second bit for LP, third bit for HP
		protected uint cNumChannels;
		protected byte uiQPIndexDC;
		protected byte uiQPIndexLP;
		protected byte uiQPIndexHP;

	    // tiling info
		protected uint  cNumOfSliceMinus1V;     // # of vertical slices
		protected uint cNumOfSliceMinus1H;     // # of horizontal slices

		#endregion

		#region Overridables
		protected virtual void PKImageDecode_Copy_WMP(SimpleBitIO bitIO)
		{ }
		#endregion

		#region Members
		protected void DecodeBitstream(uint offset)
		{
			SimpleBitIO bitIO = new SimpleBitIO(data, offset);

			ReadWMIHeader(bitIO);

			ReadImagePlaneHeader(bitIO);

			PKImageDecode_Copy_WMP(bitIO);
		}

		/// <summary>
		/// Reads header of image (p.33)
		/// </summary>
		/// <param name="bitIO">Stream to read the header from</param>
		private void ReadWMIHeader(SimpleBitIO bitIO)
		{
			// check signature
			for (int i=0; i<Constant.GDISignature.Length; i++)
			{
				if ((byte)bitIO.GetBit32_SB(8) != Constant.GDISignature[i])
				{
					throw new ArgumentOutOfRangeException("bitIO", "GDISignature[" + i + "]");
				}
			}

			// check codec version and ignore subversion
			if (bitIO.GetBit32_SB(Constant.CodecVersionNumBits) != Constant.CodecVersion)
			{
				throw new ArgumentOutOfRangeException("bitIO", "CodecVersion");
			}

			// ignore subversion
			bitIO.GetBit32_SB(Constant.CodecVersionNumBits);

			// 9 primary parameters
			// tiling present
			bTilingPresent = bitIO.GetBit32_SB(Constant.TilingFlagNumBits) == 1;
			if (bTilingPresent)
			{
				throw new ArgumentOutOfRangeException("bitIO", "bTilingPresent = " + bTilingPresent);
			}

			// bitstream layout
			bfBitstreamFormat = (BITSTREAMFORMAT)bitIO.GetBit32_SB(Constant.BitStreamFormatNumBits);
			if (bfBitstreamFormat != BITSTREAMFORMAT.SPATIAL)
			{
				throw new ArgumentOutOfRangeException("bitIO", "bfBitstreamFormat = " + bfBitstreamFormat);
			}
			// presentation orientation
			// ORIENTATION oOrientation = bitIO.GetBit32_SB(Constant.OriantationNumBits);
			bitIO.GetBit32_SB(Constant.OriantationNumBits);

			// bool bIndexTable = bitIO.GetBit32_SB(Constant.IndexTablePresentFlagNumBits) == 1;
			bitIO.GetBit32_SB(Constant.IndexTablePresentFlagNumBits);

			// overlap
			olOverlap = (OVERLAP)bitIO.GetBit32_SB(Constant.OverlapNumBits);
			if (olOverlap == OVERLAP.OL_MAX)
			{
				throw new ArgumentOutOfRangeException("bitIO", "olOverlap = " + olOverlap);
			}

			// 11 some other parameters
			// short words for size and tiles
			bAbbreviatedHeader = bitIO.GetBit32_SB(Constant.ShortHeaderFlagNumBits) == 1;
			bitIO.GetBit32_SB(1);//pSCP->bdBitDepth = (BITDEPTH)GetBit32_SB(1); // long word
			bitIO.GetBit32_SB(1);//pSCP->bdBitDepth = BD_LONG; // remove when optimization is done
			// windowing
			bInscribed = bitIO.GetBit32_SB(Constant.WindowingFlagNumBits) == 1;
			if (bInscribed)
			{
				throw new ArgumentOutOfRangeException("bitIO", "bInscribed = " + bInscribed);
			}

			// trim flexbits flag
			bTrimFlexbitsFlag = bitIO.GetBit32_SB(Constant.TrimFlexbitsFlagNumBits) == 1;
			// tile stretching flag
			bTileStretch = bitIO.GetBit32_SB(Constant.TileStretchFlagNumBits) == 1;
			if (bTileStretch)
			{
				throw new ArgumentOutOfRangeException("bitIO", "bTileStretch = " + bTileStretch);
			}

			bitIO.GetBit32_SB(2);//GetBit32_SB(2);  // padding / reserved bits
			
			// 10 - informational
			bitIO.GetBit32_SB(4);//pII->cfColorFormat = GetBit32_SB(pSB, 4); // source color format
			// source bit depth
			bdBitDepth = (BITDEPTH_BITS)bitIO.GetBit32_SB(Constant.SourceBitDepthNumBits);
			// DEM Only
			if (bdBitDepth != BITDEPTH_BITS.BD_16S)
			{
				throw new ArgumentOutOfRangeException("bitIO", "bdBitDepth = " + bdBitDepth);
			}

			if (BITDEPTH_BITS.BD_1alt == bdBitDepth)
			{
			    bdBitDepth = BITDEPTH_BITS.BD_1;
			}

			//// 12 - Variable length fields
			//// size
			cWidth  = bitIO.GetBit32_SB((uint)(bAbbreviatedHeader ? Constant.ShortHeaderNumBits : Constant.LongHeaderNumBits)) + 1;
			cHeight = bitIO.GetBit32_SB((uint)(bAbbreviatedHeader ? Constant.ShortHeaderNumBits : Constant.LongHeaderNumBits)) + 1;
			cExtraPixelsTop = cExtraPixelsLeft = cExtraPixelsBottom = cExtraPixelsRight = 0;
			if (bInscribed == false && ((cWidth & 0xf) != 0))
				cExtraPixelsRight = 0x10 - (cWidth & 0xF);
			if (bInscribed == false && ((cHeight & 0xf) != 0))
				cExtraPixelsBottom = 0x10 - (cHeight & 0xF);

			//// tiling (note: if tiling is added check code in HDPhotoDecoder.CheckTilePos
			cNumOfSliceMinus1V = cNumOfSliceMinus1H = 0;

			if(((cWidth + cExtraPixelsLeft + cExtraPixelsRight) & 0xf) + ((cHeight + cExtraPixelsTop + cExtraPixelsBottom) & 0xf) != 0)
			{
		        if (((cWidth & 0xf) + (cHeight & 0xf) + cExtraPixelsLeft + cExtraPixelsTop != 0) ||
					(cWidth <= cExtraPixelsRight || cHeight <= cExtraPixelsBottom))
				{
					throw new ArgumentOutOfRangeException("bitIO", "WMP_errInvalidParameter 07");
				}

				cWidth -= cExtraPixelsRight;
				cHeight -= cExtraPixelsBottom;
		    }
		}

		/// <summary>
		/// Reads header of FIRST PLANE only (p.39, HD Photo Bitstream Specification)
		/// </summary>
		/// <param name="bitIO">Stream to read the image plane header from</param>
		private void ReadImagePlaneHeader(SimpleBitIO bitIO)
		{
			// internal color format
			cfColorFormat = (COLORFORMAT)bitIO.GetBit32_SB(Constant.ColorFormatNumBits);
			if (cfColorFormat != COLORFORMAT.Y_ONLY /* accept DEM only! */)
			{
				throw new ArgumentOutOfRangeException("bitIO", "cfColorFormat = " + cfColorFormat);
			}

			// lossless mode
			bScaledArith = bitIO.GetBit32_SB(Constant.NoScaledFlagNumBits) == 1;

			// subbands
			sbSubband = (SUBBAND)bitIO.GetBit32_SB(Constant.BandsPresentNumBits);
			if (sbSubband != SUBBAND.SB_ALL)
			{
				throw new ArgumentOutOfRangeException("bitIO", "sbSubband = " + sbSubband);
			}

			cNumChannels = 1;

			bitIO.GetBit32_SB(8);//        pSCP->nLenMantissaOrShift = (U8)GetBit32_SB(pSB, 8);
			
			// quantization
			uQPMode = 0;
			// DC uniform
			if (bitIO.GetBit32_SB(Constant.DCFrameUniformNumBits) == 1)
			{
				uQPMode += (uint)BitstreamDecoder.ReadQuantizerSB(out uiQPIndexDC, bitIO) << 3;
			}
			else
			{
				uQPMode++;
			}

			if (sbSubband != SUBBAND.SB_DC_ONLY)
			{
				if (bitIO.GetBit32_SB(Constant.UseDCQuantizerNumBits) == 0)
				{ // don't use DC QP
					uQPMode += 0x200;
					if (bitIO.GetBit32_SB(Constant.LPFrameUniformNumBits) == 1) // LP uniform
					{
						uQPMode += (uint)BitstreamDecoder.ReadQuantizerSB(out uiQPIndexLP, bitIO) << 5;
					}
					else
					{
						uQPMode += 2;
					}
				}
				else
				{
					uQPMode += ((uQPMode & 1) << 1) + ((uQPMode & 0x18) << 2);
				}

				if (sbSubband != SUBBAND.SB_NO_HIGHPASS)
				{
					if (bitIO.GetBit32_SB(Constant.UseLPQuantizerNumBits) == 0)
					{ // don't use LP QP
						uQPMode += 0x400;
						if (bitIO.GetBit32_SB(Constant.HPFrameUniformNumBits) == 1) // HP uniform
						{
							uQPMode += (uint)BitstreamDecoder.ReadQuantizerSB(out uiQPIndexHP, bitIO) << 7;
						}
						else
						{
							uQPMode += 4;
						}
					}
					else
					{
						uQPMode += ((uQPMode & 2) << 1) + ((uQPMode & 0x60) << 2);
					}
				}
			}

			if (sbSubband == SUBBAND.SB_DC_ONLY)
			{
				uQPMode |= 0x200;
			}
			else
				if (sbSubband == SUBBAND.SB_NO_HIGHPASS)
				{
					uQPMode |= 0x400;
				}

			if ((uQPMode & 0x600) == 0)
			{
				throw new ArgumentOutOfRangeException("bitIO", "Frame level QPs must be specified independently!");
			}

			bitIO.Flush();
		}

		private static byte ReadQuantizerSB(out byte pQPIndex, SimpleBitIO bitIO)
		{
			// Y
			pQPIndex = (byte)bitIO.GetBit32_SB(Constant.DCQuantNumBits);

			return 0;
		}

		#endregion
	}
}
