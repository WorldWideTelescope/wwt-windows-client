//------------------------------------------------------------------------------
// AdaptiveModel.cs
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
	/// Context structures
	/// </summary>
	internal class CAdaptiveModel
	{
		#region Constants
		internal enum BAND
		{
			BAND_HEADER = 0,
			BAND_DC = 1,
			BAND_LP = 2,
			BAND_AC = 3,
			BAND_FL = 4
		};
		#endregion

		internal int[] m_iFlcState;
		internal int[] m_iFlcBits;
		internal BAND m_band;

		internal CAdaptiveModel()
		{
			m_iFlcState = new int[2];
			m_iFlcBits = new int[2];
		}

		internal void Reset()
		{
			m_iFlcState[0] = m_iFlcState[1] = 0;
			m_iFlcBits[0] = m_iFlcBits[1] = 0;
		}
	}
}
