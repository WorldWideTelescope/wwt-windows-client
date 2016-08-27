using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ovrSession = System.IntPtr;
using ovrTextureSwapChain = System.IntPtr;

namespace OculusWrap
{
	/// <summary>
	/// Wrapper for the TextureSwapChain type.
	/// </summary>
	public class TextureSwapChain	:IDisposable
	{
		/// <summary>
		/// Creates a new TextureSwapChain.
		/// </summary>
		/// <param name="ovr">Interface to Oculus runtime methods.</param>
		/// <param name="session">Session of the Hmd owning this texture swap chain.</param>
		/// <param name="textureSwapChainPtr">Unmanaged texture swap chain.</param>
		public TextureSwapChain(OVRBase ovr, ovrSession session, ovrTextureSwapChain textureSwapChainPtr)
		{
			if(ovr == null)
				throw new ArgumentNullException("ovr");
			if(session == null)
				throw new ArgumentNullException("session");
			if(textureSwapChainPtr == IntPtr.Zero)
				throw new ArgumentNullException("textureSwapChain");

			OVR					= ovr;
			Session				= session;
			TextureSwapChainPtr	= textureSwapChainPtr;
		}

		#region IDisposable Members
		/// <summary>
		/// Clean up the allocated HMD.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose pattern implementation of dispose method.
		/// </summary>
		/// <param name="disposing">True if disposing, false if finalizing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if(Disposed)
				return;

			if(TextureSwapChainPtr != IntPtr.Zero)
			{
				OVR.DestroyTextureSwapChain(Session, TextureSwapChainPtr);
				TextureSwapChainPtr = IntPtr.Zero;

				// Notify subscribers that this object has been disposed.
				if(ObjectDisposed != null)
					ObjectDisposed(this);
			}

			GC.SuppressFinalize(this);

			Disposed = true;
		}

		/// <summary>
		/// Describes if the object has been disposed.
		/// </summary>
		public bool Disposed
		{
			get;
			private set;
		}

		/// <summary>
		/// Notifies subscribers when this object has been disposed.
		/// </summary>
		public event Action<TextureSwapChain> ObjectDisposed;
		#endregion

		#region Public methods
		/// <summary>
		/// Gets the number of buffers in the TextureSwapChain.
		/// </summary>
		/// <param name="length">Returns the number of buffers in the specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		public OVRTypes.Result GetLength(out int length)
		{
			if(Disposed)
				throw new ObjectDisposedException("TextureSwapChain");

			return OVR.GetTextureSwapChainLength(Session, TextureSwapChainPtr, out length);
		}

		/// <summary>
		/// Gets the current index in the TextureSwapChain.
		/// </summary>
		/// <param name="index">Returns the current (free) index in specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		public OVRTypes.Result GetCurrentIndex(out int index)
		{
			if(Disposed)
				throw new ObjectDisposedException("TextureSwapChain");

			return OVR.GetTextureSwapChainCurrentIndex(Session, TextureSwapChainPtr, out index);
		}

		/// <summary>
		/// Gets the description of the buffers in the TextureSwapChain
		/// </summary>
		/// <param name="textureSwapChainDescription">Returns the description of the specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		public OVRTypes.Result GetDescription(out OVRTypes.TextureSwapChainDesc textureSwapChainDescription)
		{
			if(Disposed)
				throw new ObjectDisposedException("TextureSwapChain");

			OVRTypes.TextureSwapChainDesc textureSwapChainDesc = new OVRTypes.TextureSwapChainDesc();

			OVRTypes.Result result = OVR.GetTextureSwapChainDesc(Session, TextureSwapChainPtr, ref textureSwapChainDesc);
			textureSwapChainDescription = textureSwapChainDesc;

			return result;
		}

		/// <summary>
		/// Commits any pending changes to a TextureSwapChain, and advances its current index
		/// </summary>
		/// <returns>
		/// Returns an ovrResult for which the return code is negative upon error.
		/// Failures include but aren't limited to:
		///   - Result.TextureSwapChainFull: ovr_CommitTextureSwapChain was called too many times on a texture swapchain without calling submit to use the chain.
		/// </returns>
		public OVRTypes.Result Commit()
		{
			if(Disposed)
				throw new ObjectDisposedException("TextureSwapChain");

			return OVR.CommitTextureSwapChain(Session, TextureSwapChainPtr);
		}

		/// <summary>
		/// Get a specific buffer within the chain as any compatible COM interface (similar to QueryInterface)
		/// </summary>
		/// <param name="index">
		/// Specifies the index within the chain to retrieve. Must be between 0 and length (see GetTextureSwapChainLength),
		/// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex).
		/// </param>
		/// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
		/// <param name="buffer">Returns the COM interface pointer retrieved.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// Wrap.GetLastError to get more information.
		/// </returns>
		public OVRTypes.Result GetBufferDX(int index, Guid iid, out IntPtr buffer)
		{
			buffer = IntPtr.Zero;
			return OVR.GetTextureSwapChainBufferDX(Session, TextureSwapChainPtr, index, iid, ref buffer);
		}

		/// <summary>
		/// Get a specific buffer within the chain as a GL texture name
		/// </summary>
		/// <param name="index">
		/// Specifies the index within the chain to retrieve. Must be between 0 and length (see GetTextureSwapChainLength)
		/// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex)
		/// </param>
		/// <param name="textureId">Returns the GL texture object name associated with the specific index requested</param>
		/// <returns>
		/// Returns an OVRTypes.Result indicating success or failure. In the case of failure, use 
		/// Wrap.GetLastError to get more information.
		/// </returns>
		public OVRTypes.Result GetBufferGL(int index, out uint textureId)
		{
			return OVR.GetTextureSwapChainBufferGL(Session, TextureSwapChainPtr, index, out textureId);
		}

		#endregion

		#region Public properties
		/// <summary>
		/// Pointer to unmanaged SwapTextureSet.
		/// </summary>
		public ovrTextureSwapChain TextureSwapChainPtr
		{
			get;
			private set;
		}
		#endregion

		#region Private properties
		/// <summary>
		/// Session of the Hmd owning this texture swap chain.
		/// </summary>
		private ovrSession Session
		{
			get;
			set;
		}

		/// <summary>
		/// Interface to Oculus runtime methods.
		/// </summary>
		private OVRBase OVR
		{
			get;
			set;
		}
		#endregion
	}
}
