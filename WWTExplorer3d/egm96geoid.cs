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
        static byte[] data = ReadData();

		private static double Value(int row, int col)
		{
			int index = row * 2880 + col * 2;
			Debug.Assert(index >= 0 && index < data.Length - 1);
			short temp = unchecked((short) (256 * data[index] + data[index + 1]));
			return temp / 100.0;
		}

        static byte[] ReadData()
        {
            string name = "TerraViewer.WW15MGH.DAC";
            Stream fs = Earth3d.MainWindow.GetType().Assembly.GetManifestResourceStream(name);

            long len = fs.Length;
            BinaryReader br = new BinaryReader(fs);
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
			double y = 360.0 - 4.0 * latitude;
			int row = (int) Math.Floor(y);
			if (row < 0 || row > 719)
				throw new ArgumentOutOfRangeException("latLong");

			double x = 4.0 * longitude;
			while (x < 0) x += 1440;
			while (x >= 1440) x -= 1440;
			int col = (int) Math.Floor(x);
			int col1 = (col + 1) % 1440;

			double p = x - col;
			double q = y - row;
			double r = 1.0 - p;
			double s = 1.0 - q;

			return 
				Value(row, col) * r * s +
				Value(row, col1) * p * s +
				Value(row + 1, col) * r * q +
				Value(row + 1, col1) * p * q;
		}
	}
}
