using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ovrTextureSwapChain = System.IntPtr;

namespace OculusWrap
{
	/// <summary>
	/// Handles allocation and deallocation of the list of layer data.
	/// 
	/// This class helps reduce the number of unmanaged memory allocations, 
	/// by remembering which layers were allocated and only deallocating memory again, 
	/// when those layers change.
	/// 
	/// Having to manage a list of different types and different sizes of layers is a 
	/// new feature in Oculus SDK 0.6.0.0.
	/// </summary>
	public class Layers	:IDisposable
	{
		/// <summary>
		/// Yup, this is a finalizer. It finalizes stuff :p
		/// </summary>
		~Layers()
		{
			Dispose(false);
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

			if(!disposing)
			{
				Trace.WriteLine("The Layers with hashcode "+GetHashCode()+" was not disposed and will leak unmanaged memory.");

				// This class contains a list of unmanaged data, which cannot be deallocated when finalizing this class,
				// because the contained list could have been finalized before this class was finalized.
				return;
			}

			DeallocateUnmanaged();

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
		#endregion

		#region Public methods
		/// <summary>
		/// Add a new LayerEyeFov layer to the end of the list of layers.
		/// </summary>
		/// <returns>Added layer.</returns>
		public LayerEyeFov AddLayerEyeFov()
		{
			if(Disposed)
				throw new ObjectDisposedException("Layers");

			LayerEyeFov layer		= new LayerEyeFov();

			m_layers.Add(layer);

			// Indicate that the list of layers has been modified and the unmanaged data needs to be reallocated.
			m_layersChanged = true;

			return layer;
		}

		/// <summary>
		/// Add a new LayerQuad layer to the end of the list of layers.
		/// </summary>
		/// <returns>Added layer.</returns>
		public LayerQuad AddLayerQuadHeadLocked()
		{
			if(Disposed)
				throw new ObjectDisposedException("Layers");

			LayerQuad layer		= new LayerQuad();
			layer.Header.Type	= OVRTypes.LayerType.Quad;
			layer.Header.Flags	= OVRTypes.LayerFlags.HeadLocked;

			m_layers.Add(layer);

			// Indicate that the list of layers has been modified and the unmanaged data needs to be reallocated.
			m_layersChanged = true;

			return layer;
		}

		/// <summary>
		/// Add a new LayerQuad layer to the end of the list of layers.
		/// </summary>
		/// <returns>Added layer.</returns>
		public LayerQuad AddLayerQuadInWorld()
		{
			if(Disposed)
				throw new ObjectDisposedException("Layers");

			LayerQuad layer		= new LayerQuad();
			layer.Header.Type	= OVRTypes.LayerType.Quad;

			m_layers.Add(layer);

			// Indicate that the list of layers has been modified and the unmanaged data needs to be reallocated.
			m_layersChanged = true;

			return layer;
		}

		/// <summary>
		/// Remove the layer at the specified index.
		/// </summary>
		/// <param name="index">Index of the layer to remove.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the specified index is out of range.</exception>
		public void Remove(int index)
		{
			if(Disposed)
				throw new ObjectDisposedException("Layers");
			if(index >= m_layers.Count)
				throw new ArgumentOutOfRangeException("index", "Unable to remove layer index "+index+". Only "+m_layers.Count+" layers exist.");

			m_layers.RemoveAt(index);

			// Indicate that the list of layers has been modified and the unmanaged data needs to be reallocated.
			m_layersChanged = true;
		}

		/// <summary>
		/// Retrieves the layer at the specified index.
		/// </summary>
		/// <param name="index">Index of the layer to get.</param>
		/// <returns>Index at the specified layer.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the specified index is out of range.</exception>
		public object this[int index]
		{
			get
			{
				if(Disposed)
					throw new ObjectDisposedException("Layers");
				if(index < 0)
					throw new ArgumentOutOfRangeException("index", "Layer index "+index+" does not exist.");
				if(index >= m_layers.Count)
					throw new ArgumentOutOfRangeException("index", "Unable to remove layer index "+index+". Only "+m_layers.Count+" layers exist.");

				return m_layers[index];
			}
		}

		/// <summary>
		/// Returns the number of layers available.
		/// </summary>
		public int Count
		{
			get
			{
				if(Disposed)
					throw new ObjectDisposedException("Layers");

				return m_layers.Count;
			}
		}

		/// <summary>
		/// Retrieves the list of unmanaged layer data, to be passed to ovrHmd_SubmitFrame.
		/// </summary>
		/// <returns>List of unmanaged layer data.</returns>
		public IntPtr GetUnmanagedLayers()
		{
			if(Disposed)
				throw new ObjectDisposedException("Layers");

			// If the number of layers have changed, since the last time the unmanaged layers were retrieved.
			if(m_layersChanged)
			{
				AllocateUnmanaged();

				m_layersChanged	= false;
			}

			// Copy values from the list of layers to the list of unmanaged layer data.
			CopyToUnmanaged();

			return m_layerArrayData;
		}
		#endregion

		#region Private methods
		/// <summary>
		/// Allocate unmanaged memory for each layer.
		/// 
		/// Because each layer may be of a different type, the size of each layer may be different.
		/// </summary>
		private void AllocateUnmanaged()
		{
			// Clean up previously allocated layers.
			DeallocateUnmanaged();

			if(m_layers.Count > 0)
			{
				// Allocate memory for an unmanaged copy of the pointers to the layers of unmanaged data.
				int		pointerSize		= Marshal.SizeOf(typeof(IntPtr));
				m_layerArrayData		= Marshal.AllocHGlobal(pointerSize*m_layers.Count);

				// Allocate unmanaged data for each layer.
				for (int count=0; count<m_layers.Count; count++)
				{
					// If the layer is disabled, it will contain a null layer.
					if(m_layers[count] == null)
					{
						m_layerData.Add(IntPtr.Zero);
						continue;
					}

					//Type	layerType	= m_layers[count].GetType();
					Type	layerType	= m_layers[count].LayerType;
					int		layerSize	= Marshal.SizeOf(layerType);
					IntPtr	layerData	= Marshal.AllocHGlobal(layerSize);

					// Store the unmanaged memory needed to contain the current layer.
					m_layerData.Add(layerData);

					// Write the pointer, to the current layer, to the unmanaged layer array.
					Marshal.WriteIntPtr(m_layerArrayData, pointerSize*count, layerData);
				}
			}
		}

		/// <summary>
		/// Clean up previously allocated layers.
		/// </summary>
		private void DeallocateUnmanaged()
		{
			// Deallocate previously allocated unmanaged layer data.
			foreach(IntPtr layerData in m_layerData)
			{
				if(layerData != IntPtr.Zero)
					Marshal.FreeHGlobal(layerData);
			}
			
			if(m_layerArrayData != IntPtr.Zero)
			{
				// Deallocate the unmanaged copy of the pointers to unmanaged layer data.
				Marshal.FreeHGlobal(m_layerArrayData);
				m_layerArrayData	= IntPtr.Zero;
			}

			m_layerData.Clear();
		}

		/// <summary>
		/// Copies data from the list of layers to the list of unmanaged layer data.
		/// </summary>
		private void CopyToUnmanaged()
		{
			for(int layerIndex=0; layerIndex<m_layerData.Count; layerIndex++)
				Marshal.StructureToPtr(m_layers[layerIndex].Layer, m_layerData[layerIndex], false);
		}
		#endregion

		#region Private fields
		private List<ILayer>	m_layers			= new List<ILayer>();
		private List<IntPtr>	m_layerData			= new List<IntPtr>();
		private IntPtr			m_layerArrayData	= IntPtr.Zero;
		private bool			m_layersChanged		= false;
		#endregion
	}

	/// <summary>
	/// Defines properties shared by all ovrLayer structs, such as LayerEyeFov.
	///
	/// LayerHeader is used as a base member in these larger structs.
	/// This struct cannot be used by itself except for the case that Type is LayerType_Disabled.
	/// </summary>
	/// <see cref="OVRTypes.LayerType"/>
	/// <see cref="OVRTypes.LayerFlags"/>
	public class LayerHeader
	{
		/// <summary>
		/// Described by LayerType.
		/// </summary>
		public OVRTypes.LayerType Type
		{
			get;
			set;
		}

		/// <summary>
		/// Described by LayerFlags.
		/// </summary>
		public OVRTypes.LayerFlags Flags
		{
			get;
			set;
		}
	}

	/// <summary>
	/// Defines the methods needed to convert the managed class to a struct, which can be marshaled to unmanaged memory.
	/// </summary>
	public interface ILayer
	{
		/// <summary>
		/// Retrieve the layer structure, which is needed to marshal the managed structure to unmanaged memory.
		/// </summary>
		/// <returns>Layer sturcture needed to marshal the managed structure to unmanaged memory.</returns>
		object Layer
		{
			get;
		}

		/// <summary>
		/// Retrieves the type of layer returned by the call to the GetLayer method.
		/// </summary>
		Type LayerType
		{
			get;
		}
	}

	/// <summary>
	/// Describes a layer that specifies a monoscopic or stereoscopic view.
	/// 
	/// This is the kind of layer that's typically used as layer 0 to Hmd.SubmitFrame,
	/// as it is the kind of layer used to render a 3D stereoscopic view.
	///
	/// Three options exist with respect to mono/stereo texture usage:
	///    - ColorTexture[0] and ColorTexture[1] contain the left and right stereo renderings, respectively. 
	///      Viewport[0] and Viewport[1] refer to ColorTexture[0] and ColorTexture[1], respectively.
	///    - ColorTexture[0] contains both the left and right renderings, ColorTexture[1] is NULL, 
	///      and Viewport[0] and Viewport[1] refer to sub-rects with ColorTexture[0].
	///    - ColorTexture[0] contains a single monoscopic rendering, and Viewport[0] and 
	///      Viewport[1] both refer to that rendering.
	/// </summary>
	/// <see cref="Hmd.SubmitFrame(uint, Layers)"/>
	/// <see cref="Hmd.SubmitFrame(uint, OVRTypes.ViewScaleDesc, Layers)"/>
	public class LayerEyeFov	:ILayer
	{
		/// <summary>
		/// Creates a new LayerEyeFov.
		/// </summary>
		public LayerEyeFov()
		{
			Header			= new LayerHeader();
			Header.Type		= OVRTypes.LayerType.EyeFov;
			ColorTexture	= new IntPtr[2];
			Viewport		= new OVRTypes.Recti[2];
			Fov				= new OVRTypes.FovPort[2];
			RenderPose		= new OVRTypes.Posef[2];
		}

		#region ILayer implementation
		/// <summary>
		/// Retrieve the OVRTypes.LayerEyeFov, which is needed to marshal the managed structure to unmanaged memory.
		/// </summary>
		/// <returns>OVRTypes.LayerEyeFov containing the same values as this LayerEyeFov.</returns>
		object ILayer.Layer
		{
			get
			{
				OVRTypes.LayerEyeFov layer	= new OVRTypes.LayerEyeFov();
				layer.Header.Type		= this.Header.Type;
				layer.Header.Flags		= this.Header.Flags;
				layer.ColorTexture		= this.ColorTexture;
				layer.Viewport			= this.Viewport;
				layer.Fov				= this.Fov;
				layer.RenderPose		= this.RenderPose;
				layer.SensorSampleTime	= this.SensorSampleTime;

				return layer;
			}
		}

		/// <summary>
		/// Retrieves the type of layer returned by the call to the GetLayer method.
		/// </summary>
		Type ILayer.LayerType
		{
			get
			{
				return typeof(OVRTypes.LayerEyeFov);
			}
		}
		#endregion

		#region Public properties
		/// <summary>
		/// Header.Type must be LayerType_EyeFov.
		/// </summary>
		public LayerHeader Header
		{
			get;
			set;
		}

		/// <summary>
		/// TextureSwapChains for the left and right eye respectively.
		/// 
		/// The second one of which can be null for cases described above.
		/// </summary>
		public ovrTextureSwapChain[] ColorTexture
		{
			get;
			set;
		}

 		/// <summary>
		/// Specifies the ColorTexture sub-rect UV coordinates.
		/// 
		/// Both Viewport[0] and Viewport[1] must be valid.
 		/// </summary>
		public OVRTypes.Recti[] Viewport
		{
			get;
			set;
		}

		/// <summary>
		/// The viewport field of view.
		/// </summary>
		public OVRTypes.FovPort[] Fov
		{
			get;
			set;
		}

		/// <summary>
		/// Specifies the position and orientation of each eye view, with the position specified in meters.
		/// RenderPose will typically be the value returned from ovr_CalcEyePoses,
		/// but can be different in special cases if a different head pose is used for rendering.
		/// </summary>
		public OVRTypes.Posef[] RenderPose
		{
			get;
			set;
		}

		/// <summary>
		/// Specifies the timestamp when the source ovrPosef (used in calculating RenderPose)
		/// was sampled from the SDK. Typically retrieved by calling ovr_GetTimeInSeconds
		/// around the instant the application calls ovr_GetTrackingState
		/// The main purpose for this is to accurately track app tracking latency.
		/// </summary>
		public double SensorSampleTime
		{
			get;
			set;
		}
		#endregion
	}

	/// <summary>
	/// Describes a layer that specifies a monoscopic or stereoscopic view.
	/// This uses a direct 3x4 matrix to map from view space to the UV coordinates.
	/// It is essentially the same thing as ovrLayerEyeFov but using a much
	/// lower level. This is mainly to provide compatibility with specific apps.
	/// Unless the application really requires this flexibility, it is usually better
	/// to use ovrLayerEyeFov.
	///
	/// Three options exist with respect to mono/stereo texture usage:
	///    - ColorTexture[0] and ColorTexture[1] contain the left and right stereo renderings, respectively.
	///      Viewport[0] and Viewport[1] refer to ColorTexture[0] and ColorTexture[1], respectively.
	///    - ColorTexture[0] contains both the left and right renderings, ColorTexture[1] is null,
	///      and Viewport[0] and Viewport[1] refer to sub-rects with ColorTexture[0].
	///    - ColorTexture[0] contains a single monoscopic rendering, and Viewport[0] and
	///      Viewport[1] both refer to that rendering.
	/// </summary>
	/// <see cref="Hmd.SubmitFrame(uint, Layers)"/>
	/// <see cref="Hmd.SubmitFrame(uint, OVRTypes.ViewScaleDesc, Layers)"/>
	public class LayerEyeMatrix	:ILayer
	{
		/// <summary>
		/// Creates a new LayerEyeMatrix.
		/// </summary>
		public LayerEyeMatrix()
		{
			Header			= new LayerHeader();
			Header.Type		= OVRTypes.LayerType.EyeMatrix;
			ColorTexture	= new IntPtr[2];
			Viewport		= new OVRTypes.Recti[2];
			RenderPose		= new OVRTypes.Posef[2];
			Matrix			= new OVRTypes.Matrix4f[2];
		}

		#region ILayer implementation
		/// <summary>
		/// Retrieve the OVRTypes.LayerEyeFov, which is needed to marshal the managed structure to unmanaged memory.
		/// </summary>
		/// <returns>OVRTypes.LayerEyeFov containing the same values as this LayerEyeFov.</returns>
		object ILayer.Layer
		{
			get
			{
				OVRTypes.LayerEyeMatrix layer	= new OVRTypes.LayerEyeMatrix();
				layer.Header.Type			= this.Header.Type;
				layer.Header.Flags			= this.Header.Flags;
				layer.ColorTexture			= this.ColorTexture;
				layer.Viewport				= this.Viewport;
				layer.RenderPose			= this.RenderPose;
				layer.Matrix				= this.Matrix;
				layer.SensorSampleTime		= this.SensorSampleTime;

				return layer;
			}
		}

		/// <summary>
		/// Retrieves the type of layer returned by the call to the GetLayer method.
		/// </summary>
		Type ILayer.LayerType
		{
			get
			{
				return typeof(OVRTypes.LayerEyeMatrix);
			}
		}
		#endregion

		#region Public properties
		/// <summary>
		/// Header.Type must be LayerType_EyeFov.
		/// </summary>
		public LayerHeader Header
		{
			get;
			set;
		}

		/// <summary>
		/// ovrTextureSwapChains for the left and right eye respectively.
		/// 
		/// The second one of which can be null for cases described above.
		/// </summary>
		public ovrTextureSwapChain[] ColorTexture
		{
			get;
			set;
		}

 		/// <summary>
		/// Specifies the ColorTexture sub-rect UV coordinates.
		/// 
		/// Both Viewport[0] and Viewport[1] must be valid.
 		/// </summary>
		public OVRTypes.Recti[] Viewport
		{
			get;
			set;
		}

		/// <summary>
		/// Specifies the position and orientation of each eye view, with the position specified in meters.
		/// RenderPose will typically be the value returned from ovr_CalcEyePoses,
		/// but can be different in special cases if a different head pose is used for rendering.
		/// </summary>
		public OVRTypes.Posef[] RenderPose
		{
			get;
			set;
		}

		/// <summary>
		/// Specifies the mapping from a view-space vector
		/// to a UV coordinate on the textures given above.
		/// P = (x,y,z,1)*Matrix
		/// TexU  = P.x/P.z
		/// TexV  = P.y/P.z
		/// </summary>
		public OVRTypes.Matrix4f[] Matrix
		{
			get;
			set;
		}

		/// <summary>
		/// Specifies the timestamp when the source OVRTypes.Posef (used in calculating RenderPose)
		/// was sampled from the SDK. Typically retrieved by calling ovr_GetTimeInSeconds
		/// around the instant the application calls ovr_GetTrackingState
		/// The main purpose for this is to accurately track app tracking latency.
		/// </summary>
		public double SensorSampleTime
		{
			get;
			set;
		}
		#endregion
	}

	/// <summary>
	/// Describes a layer of Quad type, which is a single quad in world or viewer space.
	/// It is used for both ovrLayerType_QuadInWorld and ovrLayerType_QuadHeadLocked.
	/// This type of layer represents a single object placed in the world and not a stereo
	/// view of the world itself. 
	///
	/// A typical use of ovrLayerType_QuadInWorld is to draw a television screen in a room
	/// that for some reason is more convenient to draw as a layer than as part of the main
	/// view in layer 0. For example, it could implement a 3D popup GUI that is drawn at a 
	/// higher resolution than layer 0 to improve fidelity of the GUI.
	///
	/// A use of ovrLayerType_QuadHeadLocked might be to implement a debug HUD visible in 
	/// the HMD.
	///
	/// Quad layers are visible from both sides; they are not back-face culled.
	/// </summary>
	/// <see cref="Hmd.SubmitFrame(uint, Layers)"/>
	/// <see cref="Hmd.SubmitFrame(uint, OVRTypes.ViewScaleDesc, Layers)"/>
	public class LayerQuad	:ILayer
	{
		/// <summary>
		/// Creates a new LayerQuad.
		/// </summary>
		public LayerQuad()
		{
			Header			= new LayerHeader();
			Header.Type		= OVRTypes.LayerType.Quad;
			ColorTexture	= IntPtr.Zero;
			Viewport		= new OVRTypes.Recti();
			QuadPoseCenter	= new OVRTypes.Posef();
			QuadSize		= new OVRTypes.Vector2f();
		}

		#region ILayer implementation
		/// <summary>
		/// Retrieve the OVRTypes.LayerEyeFov, which is needed to marshal the managed structure to unmanaged memory.
		/// </summary>
		/// <returns>OVRTypes.LayerEyeFov containing the same values as this LayerEyeFov.</returns>
		object ILayer.Layer
		{
			get
			{
				OVRTypes.LayerQuad layer		= new OVRTypes.LayerQuad();
				layer.Header.Type		= this.Header.Type;
				layer.Header.Flags		= this.Header.Flags;
				layer.ColorTexture		= this.ColorTexture;
				layer.Viewport			= this.Viewport;
				layer.QuadPoseCenter	= this.QuadPoseCenter;
				layer.QuadSize			= this.QuadSize;

				return layer;
			}
		}

		/// <summary>
		/// Retrieves the type of layer returned by the call to the GetLayer method.
		/// </summary>
		Type ILayer.LayerType
		{
			get
			{
				return typeof(OVRTypes.LayerQuad);
			}
		}
		#endregion

		#region Public properties
		/// <summary>
		/// Header.Type must be LayerType.Quad.
		/// </summary>
		public LayerHeader Header
		{
			get;
			set;
		}

		/// <summary>
		/// Contains a single image, never with any stereo view.
		/// </summary>
		public ovrTextureSwapChain ColorTexture
		{
			get;
			set;
		}

 		/// <summary>
		/// Specifies the ColorTexture sub-rect UV coordinates.
 		/// </summary>
		public OVRTypes.Recti Viewport
		{
			get;
			set;
		}

		/// <summary>
		/// Specifies the orientation and position of the center point of a Quad layer type.
		/// The supplied direction is the vector perpendicular to the quad.
		/// The position is in real-world meters (not the application's virtual world,
		/// the physical world the user is in) and is relative to the "zero" position
		/// set by ovr_RecenterTrackingOrigin unless the ovrLayerFlag_HeadLocked flag is used.
		/// </summary>
		public OVRTypes.Posef QuadPoseCenter
		{
			get;
			set;
		}

		/// <summary>
		/// Width and height (respectively) of the quad in meters.
		/// </summary>
		public OVRTypes.Vector2f QuadSize
		{
			get;
			set;
		}
		#endregion
	}
}
