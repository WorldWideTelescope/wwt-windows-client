//------------------------------------------------------------------------------
// ContainerDecoder.cs
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
//using Microsoft.Maps.Core;

namespace Microsoft.Maps.ElevationAdjustmentService.HDPhoto
{
	internal abstract class ContainerDecoder : HDPhotoBase
	{
		#region Constants
		private struct Constant
		{
			/// <summary>
			/// TIFF header and its offset.
			/// </summary>
			internal static readonly byte[] TiffHeader = new byte[] { 0x49, 0x49 };
			internal const uint TiffHeaderOffset = 0;

			/// <summary>
			/// HD Photo header and its offset.
			/// </summary>
			internal const byte HDPhotoHeader = 0xBC;
			internal const uint HDPhotoHeaderOffset = 2;

			/// <summary>
			/// Supported version number.
			/// </summary>
			internal const byte HDPhotoVersion = 1;
			internal const uint HDPhotoVersionOffset = 3;

			/// <summary>
			/// First Image File Directory.
			/// </summary>
			internal const uint FirstIFDOffset = 4;

			/// <summary>
			/// Size of IFD entry.
			/// </summary>
			internal const uint IFDEntrySize = 12;

			// -------------------------------- from WMPGlue.h ------------------------------------------------
			internal const uint PK_pixfmtNul = 0x00000000;
			internal const uint PK_pixfmtHasAlpha = 0x00000010;
			internal const uint PK_pixfmtPreMul = 0x00000020;
			internal const uint PK_pixfmtBGR = 0x00000040;
			internal const uint PK_pixfmtNeedConvert = 0x80000000;

			// Guids of supported Pixel Formats
			internal static readonly Guid GUID_PKPixelFormat16bppGrayFixedPoint = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x13);
		}

		/// <summary>
		/// Copy of WMPMeta.h
		/// </summary>
		internal class WMPMeta
		{
			//================================================================
			// Container
			//================================================================
			//internal const int WMP_tagNull 0
			internal const int WMP_tagXMPMetadata = 0x2bc;
			internal const int WMP_tagIccProfile = 0x8773; // Need to use same tag as TIFF!!
			internal const int WMP_tagEXIFMetadata = 0x8769;

			internal const int WMP_tagPixelFormat = 0xbc01;
			internal const int WMP_tagTransformation = 0xbc02;
			internal const int WMP_tagCompression = 0xbc03;
			internal const int WMP_tagImageType = 0xbc04;

			internal const int WMP_tagImageWidth = 0xbc80;
			internal const int WMP_tagImageHeight = 0xbc81;

			internal const int WMP_tagWidthResolution = 0xbc82;
			internal const int WMP_tagHeightResolution = 0xbc83;

			internal const int WMP_tagImageOffset = 0xbcc0;
			internal const int WMP_tagImageByteCount = 0xbcc1;
			internal const int WMP_tagAlphaOffset = 0xbcc2;
			internal const int WMP_tagAlphaByteCount = 0xbcc3;
			internal const int WMP_tagImageDataDiscard = 0xbcc4;
			internal const int WMP_tagAlphaDataDiscard = 0xbcc5;

			internal struct WmpDEMisc
			{
				internal uint uImageOffset;
				internal uint uImageByteCount;
				internal uint uAlphaOffset;
				internal uint uAlphaByteCount;

				internal uint uOffPixelFormat;
				internal uint uOffImageByteCount;
				internal uint uOffAlphaOffset;
				internal uint uOffAlphaByteCount;
			}
		}

		// -------------------------------- from windowsmediaphoto.h --------------------------------------
		protected enum COLORFORMAT
		{
			Y_ONLY,
			YUV_420,
			YUV_422,
			YUV_444,
			CMYK,
			BAYER,
			N_CHANNEL,

			// these are external-only
			CF_RGB,
			CF_RGBE,
			CF_PALLETIZED,

			/* add new COLORFORMAT here */
			CFT_MAX
		};

		protected enum BITDEPTH_BITS
		{
			// regular ones
			BD_1, //White is foreground
			BD_8,
			BD_16,
			BD_16S,
			BD_16F,
			BD_32,
			BD_32S,
			BD_32F,

			// irregular ones
			BD_5,
			BD_10,
			BD_565,

			/* add new BITDEPTH_BITS here */
			BDB_MAX,

			BD_1alt = 0xf, //Black is foreground
		};
		#endregion

		#region Fields from tagPKImageDecode
		// WMP:
		protected WMPMeta.WmpDEMisc wmiDEMisc;

		struct PKPixelInfo
		{
			internal Guid pGUIDPixFmt;

			internal uint cChannel;
			internal COLORFORMAT cfColorFormat;
			internal BITDEPTH_BITS bdBitDepth;
			internal uint cbitUnit;

			internal ulong grBit;

			// TIFF
			internal uint uInterpretation;
			internal uint uSamplePerPixel;
			internal uint uBitsPerSample;
			internal uint uSampleFormat;

			internal PKPixelInfo(Guid pGUIDPixFmt, uint cChannel, COLORFORMAT cfColorFormat,
								BITDEPTH_BITS bdBitDepth, uint cbitUnit, ulong grBit, uint uInterpretation, 
								uint uSamplePerPixel, uint uBitsPerSample, uint uSampleFormat)
			{
				this.pGUIDPixFmt = pGUIDPixFmt;
				this.cChannel = cChannel;
				this.cfColorFormat = cfColorFormat;
				this.bdBitDepth = bdBitDepth;
				this.cbitUnit = cbitUnit;
				this.grBit = grBit;
				this.uInterpretation = uInterpretation;
				this.uSamplePerPixel = uSamplePerPixel;
				this.uBitsPerSample = uBitsPerSample;
				this.uSampleFormat = uSampleFormat;
			}
		}

		private PKPixelInfo[] pixelInfo = new PKPixelInfo[]
		{
			// format for DEM
		    new PKPixelInfo(Constant.GUID_PKPixelFormat16bppGrayFixedPoint, 1, COLORFORMAT.Y_ONLY, BITDEPTH_BITS.BD_16S, 16, Constant.PK_pixfmtNul,   1, 1, 16, 2)
		};
		#endregion
		
		#region Members
		internal ContainerDecoder()
		{
			wmiDEMisc = new WMPMeta.WmpDEMisc();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
		protected uint ReadContainer()
		{
			// check TIFF header
			if (data[Constant.TiffHeaderOffset] != Constant.TiffHeader[0] ||
				data[Constant.TiffHeaderOffset + 1] != Constant.TiffHeader[1])
			{
				throw new ArgumentOutOfRangeException("Not TIFF format!");
			}

			// check HDPhoto header			
			if (data[Constant.HDPhotoHeaderOffset] != Constant.HDPhotoHeader)
			{
				throw new ArgumentOutOfRangeException("Not HD Photo format!");
			}

			// check HDPhoto version
			if (data[Constant.HDPhotoVersionOffset] != Constant.HDPhotoVersion)
			{
				throw new ArgumentOutOfRangeException("Incompatible HD Photo version!");
			}

			// read Image File Directories
			uint firstIDFOffset = BitConverter.ToUInt32(data, (int)Constant.FirstIFDOffset);
			if (firstIDFOffset == 0)
			{
				throw new ArgumentOutOfRangeException("There must be at least one IFD in a HD Photo file!");
			}

			ParsePFD(firstIDFOffset);

			return wmiDEMisc.uImageOffset;
		}

		private void ParsePFD(uint offset)
		{
			if (offset != 0)
			{
				// read # of IFD entries (p.14)
				ushort cPFDEntry = BitConverter.ToUInt16(data, (int)offset);
				offset += 2;

				if (cPFDEntry == 0 || cPFDEntry == ushort.MaxValue)
				{
					throw new ArgumentOutOfRangeException("offset", "cPFDEntry = " + cPFDEntry);
				}

				for (int i = 0; i < cPFDEntry; i++)
				{
					ushort uTag  = BitConverter.ToUInt16(data, (int)offset);
					ushort uType = BitConverter.ToUInt16(data, (int)offset + 2);
					uint uCount  = BitConverter.ToUInt16(data, (int)offset + 4);
					uint uValue  = BitConverter.ToUInt16(data, (int)offset + 8);

					ParsePFDEntry(uTag, uType, uCount, uValue, data);

					offset += Constant.IFDEntrySize;
				}

				ParsePFD(BitConverter.ToUInt32(data, (int)offset));
			}
		}

		private void PixelFormatLookup(Guid guid)
		{
			foreach (PKPixelInfo pi in pixelInfo)
			{
				if (guid.Equals(pi.pGUIDPixFmt))
				{
					return;
				}
			}

			throw new ArgumentOutOfRangeException("guid", "Unknown guid");
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		internal void ParsePFDEntry(ushort uTag, ushort uType, uint uCount, uint uValue, byte[] data)
		{
			switch (uTag)
			{
				case WMPMeta.WMP_tagPixelFormat:
					byte[] guidBytes = new byte[16];
					for (int i = 0; i < 16; i++)
					{
						guidBytes[i] = data[uValue + i];
					}
					Guid guid = new Guid(guidBytes);

					// check if this format is supported
					PixelFormatLookup(guid);
					break;

				case WMPMeta.WMP_tagImageWidth:
				case WMPMeta.WMP_tagImageHeight:
					if (0 == uValue)
					{
						throw new ArgumentOutOfRangeException("uTag", "WMP_tagImageHeight");
					}
					break;

				case WMPMeta.WMP_tagImageOffset:
					if (1 != uCount)
					{
						throw new ArgumentOutOfRangeException("uTag", "WMP_tagImageOffset");
					}
					wmiDEMisc.uImageOffset = uValue;
					break;

				case WMPMeta.WMP_tagImageByteCount:
					if (1 != uCount)
					{
						throw new ArgumentOutOfRangeException("uTag", "WMP_tagImageByteCount");
					}
					wmiDEMisc.uImageByteCount = uValue;
					break;

				case WMPMeta.WMP_tagAlphaOffset:
					if (1 != uCount)
					{
						throw new ArgumentOutOfRangeException("uTag", "WMP_tagAlphaOffset");
					}
					wmiDEMisc.uAlphaOffset = uValue;
					break;

				case WMPMeta.WMP_tagAlphaByteCount:
					if (1 != uCount)
					{
						throw new ArgumentOutOfRangeException("uTag", "WMP_tagAlphaByteCount");
					}
					wmiDEMisc.uAlphaByteCount = uValue;
					break;

				case WMPMeta.WMP_tagWidthResolution:
					if (1 != uCount)
					{
						throw new ArgumentOutOfRangeException("uTag", "WMP_tagWidthResolution");
					}
					//fResX = FloatBits.ToSingle(uValue);
					break;

				case WMPMeta.WMP_tagHeightResolution:
					if (1 != uCount)
					{
						throw new ArgumentOutOfRangeException("uTag", "WMP_tagHeightResolution");
					}
					//fResX = FloatBits.ToSingle(uValue);
					break;

				case WMPMeta.WMP_tagXMPMetadata:
				case WMPMeta.WMP_tagIccProfile:
				case WMPMeta.WMP_tagEXIFMetadata:
				case WMPMeta.WMP_tagTransformation:
				case WMPMeta.WMP_tagCompression:
				case WMPMeta.WMP_tagImageType:
				case WMPMeta.WMP_tagImageDataDiscard:
				case WMPMeta.WMP_tagAlphaDataDiscard:
					break;

				default:
					// unrecognized WMP_, we hope to continue without problems
					Exception ex = new ArgumentOutOfRangeException("uTag", "Unrecognized PDF entry");
                    //ElevationAdjustmentServicePlugin.LogException(
                    //    new ExceptionInfo(ex),
                    //    "uTag = " + uTag + ", uType = " + uType + ", uCount = " + uCount
                    //    + ", uValue = " + uValue,
                    //    true);
					break;
			}
		}
		#endregion
	}
}
