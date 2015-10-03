// -----------------------------------------------------------------------------
// <copyright file="DemTile.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <createdby>bretm</createdby><creationdate>2006-01-10</creationdate>
// -----------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Microsoft.Maps.ElevationAdjustmentService.HDPhoto
{
    /// <summary>
	/// Represents a Digital Elevation Map tile. 
	/// Copy of $\VirtualEarth\Main\Metropolis\Applications\DEMPipeline\DemPipeline\DemTile.cs
	/// with deleted functionality that is not needed for FL.
    /// </summary>
	internal class DemTile
	{
		/// <summary>
		/// An elevation value equal to 'NoData' represents a sample of 
		/// unknown elevation. For "double" type, use double.NaN instead.
		/// </summary>
		internal const int NoData = short.MinValue;

        readonly int width;                  // number of columns
        readonly int height;                 // number of rows
		int returned;				// 1 == true, and the object is invalid

		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
		short[,] altitude;          // altitude values in arbitrary linear scale

		/// <summary>
		/// Create a default 257x257 DEM tile at 1m per unit of altitude, 
		/// initialized to NoData.
		/// </summary>
		internal DemTile()	: this(257, 257)
		{
		}

        /// <summary>
        /// Constructs a new DemTile width the passed in size.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
		internal DemTile(int width, int height)
		{
			if (width < 0)
				throw new ArgumentOutOfRangeException("width");
			if (height < 0)
				throw new ArgumentOutOfRangeException("height");

			this.width = width;
			this.height = height;
			altitude = new short[height, width];
			for (var row = 0; row < height; row++)
				for (var col = 0; col < width; col++)
				{
					altitude[row, col] = NoData;
				}
		}

        /// <summary>
        /// Constructs a new DemTile and fills it with the passed in data.
        /// </summary>
        /// <param name="data"></param>
		internal DemTile(short[,] data)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			width = data.GetLength(1);
			height = data.GetLength(0);
			altitude = data;
		}

		/// <summary>
		/// If true, the DemTile is safe to use.  If false, the DemTile was returned to the Codec and cannot be used.
		/// </summary>
		internal bool IsValid
		{
			get { return returned == 0; }
			set
			{
				Interlocked.Exchange(ref returned, value ? 1 : 0);
				altitude = null;
			}
		}

        /// <summary>
        /// Returns true if the tile contains only one elevation value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		//[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "0#")]
		internal bool IsSingleValued(out short value)
		{
			if (!IsValid) throw new InvalidOperationException();
			value = altitude[0, 0];
			for (var y = 0; y < height; y++)
			{
				for (var x = 0; x < width; x++)
				{
					if (altitude[y, x] != value)
					{
						return false;
					}
				}
			}
			return true;
		}

        /// <summary>
        /// Retrieves the width of the DemTile.
        /// </summary>
		internal int Width
		{
			get { return width; }
		}

        /// <summary>
        /// Retrieves the height of the DemTile.
        /// </summary>
		internal int Height
		{
			get { return height; }
		}

        /// <summary>
        /// Make sure the row and column values are valid.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
		private void ValidateRowCol(int row, int col)
		{
			if (row < 0 || row >= height)
				throw new ArgumentOutOfRangeException("row");

			if (col < 0 || col >= width)
				throw new ArgumentOutOfRangeException("col");
		}

        /// <summary>
        /// Look up an individual altitude sample.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>Returns the altitude sample in arbitrary units.
        /// Use DemTile.AltitudeInMeters to obtain the value in meters.</returns>
		internal short this[int row, int col]
		{
			get
			{
				if (!IsValid) throw new InvalidOperationException();
				ValidateRowCol(row, col);
				return altitude[row, col];
			}
			set
			{
				if (!IsValid) throw new InvalidOperationException();
				altitude[row, col] = value;
			}
		}

		// non-linear altitude scale
		static readonly object[] scale =
		{
		    // description,		meters,		resolution,
		    "Marianas Trench",	-11030.0,	10.0,	// 10m per unit
		    "lowest city",		  -280.0,	0.1,	// 10cm per unit
		    "sea level",			 0.0,	0.01,	// 1cm per unit
		    "highest city",	      5099.0,	0.2,	// 20cm per unit
		    "Mount Everest",	  8872.0
		};
		const short scaleMinIndex = -32767;
		const short scaleMaxIndex = short.MaxValue;

		private static readonly double[] scaleMeters = CalculateScaleMeters();

		private static double[] CalculateScaleMeters()
		{
			var m = new double[scaleMaxIndex - scaleMinIndex + 1];
			var i = 0;
			var x0 = scaleMinIndex;
			while (i < scale.Length - 2)
			{
				var y0 = (double) scale[i + 1];
				var r0 = (double) scale[i + 2];
				var y1 = (double) scale[i + 4];
				short x1;
				if (i + 5 >= scale.Length)
					x1 = scaleMaxIndex;
				else
				{
					var r1 = (double) scale[i + 5];
					x1 = (short) Math.Round(x0 + 2 * (y1 - y0) / (r0 + r1));
				}
				var dx = x1 - x0;
				var dy = y1 - y0;
				var dr = 2 * (dy / dx - r0) / dx;
				for (var x = x0; x < x1; x++)
				{
					m[x - scaleMinIndex] = (x - x0) * (r0 + dr * (x - x0) / 2.0) + y0;
				}
				i += 3;
				x0 = x1;
			}
			m[m.Length - 1] =  (double) scale[scale.Length - 1];
			return m;
		}

        /// <summary>
        /// Converts a short index value into an elevation in meters.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
		internal static double IndexToMeters(short index)
		{
			if (index == NoData)
				return double.NaN;

			return scaleMeters[index - scaleMinIndex];
		}

        /// <summary>
        /// Converts a meter value to an index into the Dem tile elevation index.
        /// </summary>
        /// <param name="meters"></param>
        /// <returns></returns>
		internal static short MetersToIndex(double meters)
		{
			if (double.IsNaN(meters))
				return NoData;

			if (meters <= scaleMeters[0])
				return scaleMinIndex;

			if (meters >= scaleMeters[scaleMaxIndex - scaleMinIndex])
				return scaleMaxIndex;

			var index = Array.BinarySearch(scaleMeters, meters);
			if (index >= 0)
				return (short) (index + scaleMinIndex);

			var mid = (scaleMeters[~index] + scaleMeters[~index - 1]) / 2;
			if (meters < mid)
				return (short) (~index - 1 + scaleMinIndex);
            return (short) (~index + scaleMinIndex);
		}

		/// <summary>
		/// Look up an individual altitude sample.
		/// </summary>
		/// <returns>Returns the altitude sample in meters above sea level.</returns>
		internal double AltitudeInMeters(int row, int col)
		{
			// indexer does validation
			return IndexToMeters(this[row, col]);
		}

		/// <summary>
		/// Set an individual altitude sample in meters instead of arbitrary
		/// linear units.
		/// </summary>
		internal void SetAltitudeInMeters(int row, int col, double meters)
		{
			// indexer does validation
			this[row, col] = MetersToIndex(meters);
		}

        /// <summary>
        /// Retrieves the elevation from a specific location in the tile.
        /// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
		internal short[,] AltitudeBuffer
		{
			get { return altitude; }
			set { altitude = value; }
		}
	}
}
