using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ovrSession = System.IntPtr;
using ovrMirrorTexture = System.IntPtr;

namespace OculusWrap
{
	/// <summary>
	/// Wrapper for the MirrorTexture type.
	/// </summary>
	public class MirrorTexture	:IDisposable
	{
		/// <summary>
		/// Creates a new MirrorTexture.
		/// </summary>
		/// <param name="ovr">Interface to Oculus runtime methods.</param>
		/// <param name="session">Session of the Hmd owning this mirror texture.</param>
		/// <param name="mirrorTexturePtr">Unmanaged mirror texture.</param>
		public MirrorTexture(OVRBase ovr, ovrSession session, IntPtr mirrorTexturePtr)
		{
			if(ovr == null)
				throw new ArgumentNullException("ovr");
			if(session == null)
				throw new ArgumentNullException("session");
			if(mirrorTexturePtr == IntPtr.Zero)
				throw new ArgumentNullException("mirrorTexturePtr");

			OVR					= ovr;
			Session				= session;
			MirrorTexturePtr	= mirrorTexturePtr;
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

			if(MirrorTexturePtr != IntPtr.Zero)
			{
				OVR.DestroyMirrorTexture(Session, MirrorTexturePtr);
				MirrorTexturePtr = IntPtr.Zero;

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
		public event Action<MirrorTexture> ObjectDisposed;
		#endregion

		#region Public methods
		/// <summary>
		/// Get a the underlying buffer as any compatible COM interface (similar to QueryInterface) 
		/// </summary>
		/// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
		/// <param name="buffer">Returns the COM interface pointer retrieved.</param>
		/// <returns>
		/// Returns a Result indicating success or failure. In the case of failure, use 
		/// Wrap.GetLastError to get more information.
		/// </returns>
		public OVRTypes.Result GetBufferDX(Guid iid, out IntPtr buffer)
		{
			buffer = IntPtr.Zero;
			return OVR.GetMirrorTextureBufferDX(Session, MirrorTexturePtr, iid, ref buffer);
		}

		/// <summary>
		/// Get a the underlying buffer as a GL texture name
		/// </summary>
		/// <param name="textureId">Specifies the GL texture object name associated with the mirror texture</param>
		/// <returns>
		/// Returns an OVRTypes.Result indicating success or failure. In the case of failure, use 
		/// Wrap.GetLastError to get more information.
		/// </returns>
		public OVRTypes.Result GetBufferGL(out uint textureId)
		{
			return OVR.GetMirrorTextureBufferGL(Session, MirrorTexturePtr, out textureId);
		}

		#endregion

		#region Public properties
		/// <summary>
		/// Pointer to unmanaged MirrorTexture.
		/// </summary>
		public ovrMirrorTexture MirrorTexturePtr
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
