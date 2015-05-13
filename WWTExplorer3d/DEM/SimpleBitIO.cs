//------------------------------------------------------------------------------
// SimpleBitIO.cs
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
	internal class SimpleBitIO
	{
		private byte bAccumulator;			// set to 0 by the runtime
		private uint longAccumulator;		// set to 0 by the runtime
		private uint cBitLeft;				// set to 0 by the runtime
		private uint offset;
		private byte[] data;

		internal SimpleBitIO(byte[] data, uint offset)
		{
			this.data = data;
			this.offset = offset;
		}

		internal int cBitsUsed
		{
			get
			{
				return 8 - (int)cBitLeft;
			}
		}

		/// <summary>
		/// Returns uint built from cBits starting at current location.
		/// The decoding is performed as defined for 'uimsbf' on p.32
		/// Code taken from strcodec.c
		/// </summary>
		internal uint GetBit32_SB(uint cBits)
		{
			uint rc = 0;

			while (cBitLeft < cBits)
			{
				rc <<= (ushort)cBitLeft;
				rc |= (uint)(bAccumulator >> (ushort)(8 - cBitLeft));

				cBits -= cBitLeft;

				bAccumulator = data[offset++];
				cBitLeft = 8;
			}

			rc <<= (ushort)cBits;
			rc |= (uint)(bAccumulator >> (ushort)(8 - cBits));
			bAccumulator <<= (ushort)cBits;
			cBitLeft -= (ushort)cBits;

			return rc;
		}

		internal void Flush()
		{
			longAccumulator = 0;
			bAccumulator = 0;
			cBitLeft = 0;
		}

		internal byte NextByte()
		{
			return data[offset++];
		}

		/// <summary>
		/// Returns ushort created from cBits most important bits
		/// </summary>
		/// <param name="cBits"></param>
		/// <returns></returns>
		internal ushort GetBit16(uint cBits)
		{
			System.Diagnostics.Debug.Assert(cBits <= 16);

			while (cBitLeft < cBits)
			{
				longAccumulator = ((longAccumulator & (ushort)HDPhotoBase.MaskBits[cBitLeft]) << 8) + data[offset++];
				cBitLeft += 8;
			}

			cBitLeft -= cBits;
			return (ushort)((longAccumulator >> (byte)cBitLeft) & (ushort)HDPhotoBase.MaskBits[cBits]);
		}

		/** this function returns cBits if zero is read, or a signed value if first cBits are not all zero **/
		internal int GetBit16s(uint cBits)
		{
			int iRet = (int)PeekBit16(cBits + 1);
			iRet = ((iRet >> 1) ^ (-(iRet & 1))) + (iRet & 1);
			GetBit16((uint)(cBits + ((iRet != 0) ? 1 : 0)));
			
			return iRet;
		}

		/// <summary>
		/// Returns ushort created from cBits most important bits, does not modify state of bitIO
		/// Haev to make this trick, because this peeking may try to reach outside of the bitstream!
		/// </summary>
		/// <param name="cBits"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "offset", Justification = "To simplify comparizons with the original code"),]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "longAccumulator", Justification = "To simplify comparizons with the original code")] 
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "cBitLeft", Justification = "To simplify comparizons with the original code")]
		internal ushort PeekBit16(uint cBits)
		{
			System.Diagnostics.Debug.Assert(cBits <= 16);

			uint offset = this.offset;
			uint longAccumulator = this.longAccumulator;
			int cBitLeft = (int)this.cBitLeft;

			while ((cBitLeft < cBits) && offset<data.Length)
			{
				longAccumulator = ((longAccumulator & (ushort)HDPhotoBase.MaskBits[cBitLeft]) << 8) + data[offset++];
				cBitLeft += 8;
			}

			if (cBitLeft < cBits)
			{	
				// in case we're trying to reach beyond the bitstream
				return (ushort)((longAccumulator << (byte)(cBits - cBitLeft)) & (ushort)HDPhotoBase.MaskBits[cBits]);
			}
			else
			{
				return (ushort)((longAccumulator >> (byte)(cBitLeft - cBits)) & (ushort)HDPhotoBase.MaskBits[cBits]);
			}
		}

		/// <summary>
		/// Returns uint created from cBits most important bits
		/// </summary>
		/// <param name="cBits"></param>
		/// <returns></returns>
		internal uint GetBit32(uint cBits)
		{
			System.Diagnostics.Debug.Assert(cBits <= 32);

			uint res = 0;

			if (cBits > 16)
			{
				res = GetBit16(16);
				cBits -= 16;
				res <<= (byte)cBits;
			}

			res |= GetBit16(cBits);
			
			return res;
		}

		/// <summary>
		/// Returns bool created from first most important bit
		/// </summary>
		/// <returns></returns>
		internal bool GetBool16()
		{
			return GetBit16(1) == 1;
		}
	}
}
