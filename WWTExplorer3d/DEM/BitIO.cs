//------------------------------------------------------------------------------
// <copyright file="BitIO.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <createdby>bretm</createdby><creationdate>2005-11-29</creationdate>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Microsoft.MapPoint.Data.VirtualEarthTileDataStore.ElevationData.Compression
{
	/// <summary>
	/// Writes individual bits to a buffer of bytes.
	/// </summary>
    public class BitWriter
    {
		Stream stream;

		/// <summary>
		/// Constructor.
		/// </summary>
        public BitWriter(Stream stream)
        {
			if (stream == null)
				throw new ArgumentNullException("stream");

			this.stream = stream;
        }

        ushort bitBuffer;
        int bitCount;

		/// <summary>
		/// Write between 1 and 8 bits to the buffer.
		/// </summary>
		/// <param name="bits">The bits to write. The least-significant bits are
		/// used.</param>
		/// <param name="numberOfBits">The number of least-significant bits from
		/// the "bits" parameter to write.</param>
		/// <returns>Returns false iff the end of the buffer was exceeded.
		/// </returns>
        public void WriteBits(byte bits, int numberOfBits)
        {
			if (numberOfBits < 1 || numberOfBits > 8)
				throw new ArgumentOutOfRangeException("numberOfBits");

			bitBuffer |= (ushort) (bits << (16 - bitCount - numberOfBits));
			bitCount += numberOfBits;
			if (bitCount >= 8)
			{
				stream.WriteByte((byte) (bitBuffer >> 8));
				bitBuffer <<= 8;
				bitCount -= 8;
			}
        }

		/// <summary>
		/// Write out the partial final byte, if any.
		/// </summary>
		/// <returns>Returns the new buffer byte offset, one byte past the end
		/// of where bits were written.</returns>
        public void FlushBits()
        {
            if (bitCount > 0)
            {
				stream.WriteByte((byte) (bitBuffer >> 8));
            }
        }
    }

	/// <summary>
	/// Read individual bits one at a time from a buffer of bytes.
	/// Use instead of BitArray for stream-like behavior.
	/// </summary>
    public class BitReader
    {
		Stream stream;

		/// <summary>
		/// Constructor.
		/// </summary>
		public BitReader(Stream stream)
        {
			if (stream == null)
				throw new ArgumentNullException("stream");
			this.stream = stream;
        }

        byte bitBuffer;
        int bitCount;

		/// <summary>
		/// Read one bit from the buffer.
		/// </summary>
		public bool ReadBit()
        {
            if (bitCount == 0)
            {
				int nextByte = stream.ReadByte();
				if (nextByte == -1)
					throw new EndOfStreamException();
				bitBuffer = (byte) nextByte;
                bitCount = 8;
            }

            bool bit = bitBuffer >= 128;
            bitBuffer <<= 1;
            bitCount--;

            return bit;
        }
    }
}
