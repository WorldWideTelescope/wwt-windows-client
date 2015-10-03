//------------------------------------------------------------------------------
// <copyright file="EGM86Geoid.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <createdby>bretm</createdby><creationdate>2006-04-18</creationdate>
//------------------------------------------------------------------------------

using System;
using System.IO;
using System.Diagnostics;

namespace TerraViewer
{
    /// <summary>
    /// EGM96Geoid implementation of the Geoid base class.
    /// </summary>
	public class EGM96Geoid 
	{
        static readonly byte[] data = ReadData();

		private static double Value(int row, int col)
		{
			var index = row * 2880 + col * 2;
			Debug.Assert(index >= 0 && index < data.Length - 1);
			var temp = unchecked((short) (256 * data[index] + data[index + 1]));
			return temp / 100.0;
		}

        static byte[] ReadData()
        {
            var name = "TerraViewer.WW15MGH.DAC";
            var fs = Earth3d.MainWindow.GetType().Assembly.GetManifestResourceStream(name);

            var len = fs.Length;
            var br = new BinaryReader(fs);
            return br.ReadBytes((int)fs.Length);
            
        }
        /// <summary>
        /// Returns the EGM96Geoid height for the specified latitude, longitude.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
		public static double Height(double latitude, double longitude)
		{
			var y = 360.0 - 4.0 * latitude;
			var row = (int) Math.Floor(y);
			if (row < 0 || row > 719)
				throw new ArgumentOutOfRangeException("latLong");

			var x = 4.0 * longitude;
			while (x < 0) x += 1440;
			while (x >= 1440) x -= 1440;
			var col = (int) Math.Floor(x);
			var col1 = (col + 1) % 1440;

			var p = x - col;
			var q = y - row;
			var r = 1.0 - p;
			var s = 1.0 - q;

			return 
				Value(row, col) * r * s +
				Value(row, col1) * p * s +
				Value(row + 1, col) * r * q +
				Value(row + 1, col1) * p * q;
		}
	}
}
