//------------------------------------------------------------------------------
// PointerArray.cs
//
// <copyright company='Microsoft Corporation'> 
// Copyright (c) Microsoft Corporation. All Rights Reserved. 
// Information Contained Herein is Proprietary and Confidential. 
// </copyright>
//
// Owner: macbork, 05.26.09
//
//------------------------------------------------------------------------------

namespace Microsoft.Maps.ElevationAdjustmentService.HDPhoto
{
	/// <summary>
	/// The purpose of this class is to implement array that can start in the middle of another one
	/// </summary>
	internal class PointerArray
	{
		private int[] array;
		private uint ptr;

		internal PointerArray(int size, int ptr)
		{
			array = new int[size];
			this.ptr = (uint)ptr;
		}

		internal PointerArray(int[] array, int ptr)
		{
			this.array = array;
			this.ptr = (uint)ptr;
		}

		internal PointerArray(PointerArray pa, int ptr)
		{
			this.array = pa.array;
			this.ptr = pa.ptr + (uint)ptr;
		}

		internal void IncrementPointer(int i)
		{
			ptr = (uint)((int)ptr + i);
		}

		internal int this[uint index]
		{
			get
			{
				return array[ptr+index];
			}
			set
			{
				array[ptr+index] = value;
			}
		}

		internal int[] Array
		{
			get
			{
				return array;
			}
			set
			{
				array = value;
			}
		}

		internal int Pointer
		{
			get
			{
				return (int)ptr;
			}
		}
	}
}
