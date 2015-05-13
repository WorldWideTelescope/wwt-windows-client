//------------------------------------------------------------------------------
// CCodingContext.cs
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
	/// Adaptive context model
	/// </summary>
	internal class CCodingContext
	{
		#region Constants
		/** scan model **/
		internal struct CAdaptiveScan {
			internal int		uTotal;
			internal int		uScan;
		};

		internal class CCBPModel
		{
			internal int[] m_iCount0 = new int[2];
			internal int[] m_iCount1 = new int[2];
			internal int[] m_iState = new int[2];
		};
		#endregion

		#region Fields
		//BitIOInfo * m_pIODC;
		//BitIOInfo * m_pIOLP;
		//BitIOInfo * m_pIOAC;
		//BitIOInfo * m_pIOFL;

		/** adaptive huffman structs **/
		internal CAdaptiveHuffman m_pAdaptHuffCBPCY;
		internal CAdaptiveHuffman m_pAdaptHuffCBPCY1;
		internal CAdaptiveHuffman[] m_pAHexpt;

		internal CAdaptiveScan[] m_aScanLowpass = new CAdaptiveScan[16];
		internal CAdaptiveScan[] m_aScanHoriz = new CAdaptiveScan[16];
		internal CAdaptiveScan[] m_aScanVert = new CAdaptiveScan[16];

		/** Adaptive bit reduction model **/
		internal CAdaptiveModel m_aModelAC;
		internal CAdaptiveModel m_aModelLP;
		internal CAdaptiveModel m_aModelDC;

		/** Adaptive lowpass CBP model **/
		internal int m_iCBPCountZero;
		internal int m_iCBPCountMax;

		/** Adaptive AC CBP model **/
		internal CCBPModel m_aCBPModel;

		/** Trim flex bits - externally set **/
		internal int m_iTrimFlexBits;

		//internal bool m_bInROI;  // inside ROI (for region decode and compressed domain cropping)?
		#endregion Fields

		#region Members
		internal CCodingContext()
		{
			m_aModelAC = new CAdaptiveModel();
			m_aModelLP = new CAdaptiveModel();
			m_aModelDC = new CAdaptiveModel();

			m_aCBPModel = new CCBPModel();
		}

		internal void ResetCodingContext()
		{
			// reset bit reduction models
			m_aModelAC.Reset();
			m_aModelAC.m_band = CAdaptiveModel.BAND.BAND_AC;

			m_aModelLP.Reset();
			m_aModelLP.m_band = CAdaptiveModel.BAND.BAND_LP;
			m_aModelLP.m_iFlcBits[0] = m_aModelLP.m_iFlcBits[1] = 4;

			m_aModelDC.Reset();
			m_aModelDC.m_band = CAdaptiveModel.BAND.BAND_DC;
			m_aModelDC.m_iFlcBits[0] = m_aModelDC.m_iFlcBits[1] = 8;

			// reset CBP models
			m_iCBPCountMax = m_iCBPCountZero = 1;

			m_aCBPModel.m_iCount0[0] = m_aCBPModel.m_iCount0[1] = -4;
			m_aCBPModel.m_iCount1[0] = m_aCBPModel.m_iCount1[1] = 4;
			m_aCBPModel.m_iState[0] = m_aCBPModel.m_iState[1] = 0;
		}
		#endregion Members
	}
}
