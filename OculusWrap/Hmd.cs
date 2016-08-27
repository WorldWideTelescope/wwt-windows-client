using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using ovrBool = System.Byte;
using ovrSession = System.IntPtr;
using ovrTextureSwapChain = System.IntPtr;
using ovrMirrorTexture = System.IntPtr;

namespace OculusWrap
{
	/// <summary>
	/// Provides access to methods for a detected head mounted display (HMD).
	/// 
	/// This head mounted display can be detected by calling Wrap.Hmd_Create or Wrap.Hmd_CreateDebug.
	/// </summary>
	public class Hmd	:IDisposable
	{
		/// <summary>
		/// Allocate a new object to handle a head mounted display.
		/// </summary>
		/// <param name="ovr">Interface to Oculus runtime methods.</param>
		/// <param name="hmdPtr">Pointer to the HMD structure, created by calling Oculus.Hmd_Create or Oculus.Hmd_CreateDebug.</param>
		public Hmd(OVRBase ovr, IntPtr hmdPtr)
		{
			if(ovr == null)
				throw new ArgumentNullException("ovr");
			if(hmdPtr == IntPtr.Zero)
				throw new ArgumentNullException("hmdPtr");

			OVR		= ovr;
			Session	= hmdPtr;

			// Retrieve the properties of the specified HMD.
			OVRTypes.HmdDesc hmdDesc;
			if(Environment.Is64BitProcess)
			{
				// Retrieve the 64 bit HmdDesc.
				// In the Oculus SDK, the 64 bit HmdDesc contains padding characters that aren't present in the 32 bit HmdDesc.
				// In order to treat both versions of the HmdDesc the same, the 64 bit HmdDesc is copied to a new 32 bit HmdDesc, here.
				OVRTypes.HmdDesc64 hmdDesc64;
				OVR.GetHmdDesc64(out hmdDesc64, hmdPtr);

				// Create a 32 bit HmdDesc, based on the 64 bit HmdDesc.
				hmdDesc = new OVRTypes.HmdDesc(hmdDesc64);
			}
			else
				OVR.GetHmdDesc32(out hmdDesc, hmdPtr);

			OvrHmd	= hmdDesc;
		}

		/// <summary>
		/// Yup, this is a finalizer. It finalizes stuff :p
		/// </summary>
		~Hmd()
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

			if(Session != IntPtr.Zero)
			{
				// Ensure that all created SwapTextureSets have been destroyed.
				foreach(TextureSwapChain textureSwapChain in CreatedTextureSwapChains)
				{
					// Prevent the object from removing itself from the CreatedTextureSwapChains set, when it is disposed.
					textureSwapChain.ObjectDisposed -= OnTextureSwapChainDisposed;

					textureSwapChain.Dispose();
				}

				// Ensure that all created MirrorTextures have been destroyed.
				foreach(MirrorTexture mirrorTexture in CreatedMirrorTextures)
				{
					// Prevent the object from removing itself from the CreatedMirrorTextures set, when it is disposed.
					mirrorTexture.ObjectDisposed -= OnMirrorTextureDisposed;

					mirrorTexture.Dispose();
				}

				CreatedTextureSwapChains.Clear();
				CreatedMirrorTextures.Clear();

				OVR.Destroy(Session);
				Session = IntPtr.Zero;
			}

			// Deallocate unmanaged memory again.
			FreeHGlobal(ref m_configDataPtr);
			FreeHGlobal(ref m_distortionMeshPtr);
			FreeHGlobal(ref m_textureSetPtr);
			FreeHGlobal(ref m_eyePosesPtr);
			FreeHGlobal(ref m_sensorSampleTimePtr);
			FreeHGlobal(ref m_viewScaleDescPtr);

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
		/// Returns the number of sensors. 
		///
		/// The number of sensors may change at any time, so this function should be called before use 
		/// as opposed to once on startup.
		/// </summary>
		/// <returns>
		/// Number of sensors. 
		/// </returns>
		public uint GetTrackerCount()
		{
			return OVR.GetTrackerCount(Session);
		}

		/// <summary>
		/// Returns a given sensor description.
		///
		/// It's possible that sensor desc [0] may indicate a unconnnected or non-pose tracked sensor, but 
		/// sensor desc [1] may be connected.
		/// </summary>
		/// <param name="trackerDescIndex">
		/// Specifies a sensor index. The valid indexes are in the range of 0 to the sensor count returned by ovr_GetTrackerCount.
		/// </param>
		/// <returns>An empty ovrTrackerDesc will be returned if trackerDescIndex is out of range.</returns>
		/// <see cref="OVRTypes.TrackerDesc"/>
		public OVRTypes.TrackerDesc GetTrackerDesc(uint trackerDescIndex)
		{
			return OVR.GetTrackerDesc(Session, trackerDescIndex);
		}

		/// <summary>
		/// Returns status information for the application.
		/// </summary>
		/// <param name="sessionStatus">Provides a SessionStatus that is filled in.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of 
		/// failure, use ovr_GetLastErrorInfo to get more information.
		/// </returns>
		public OVRTypes.Result GetSessionStatus(ref OVRTypes.SessionStatus sessionStatus)
		{
			return OVR.GetSessionStatus(Session, ref sessionStatus);
		}

		/// <summary>
		/// Sets the tracking origin type
		///
		/// When the tracking origin is changed, all of the calls that either provide
		/// or accept ovrPosef will use the new tracking origin provided.
		/// </summary>
		/// <param name="origin">Specifies an ovrTrackingOrigin to be used for all ovrPosef</param>
		/// <returns>
		/// Returns a Result indicating success or failure. 
		/// In the case of failure, use GetLastErrorInfo to get more information.
		/// </returns>
		/// <see cref="OVRTypes.TrackingOrigin"/>
		/// <see cref="GetTrackingOriginType"/>
		public OVRTypes.Result SetTrackingOriginType(OVRTypes.TrackingOrigin origin)
		{
			return OVR.SetTrackingOriginType(Session, origin);
		}

		/// <summary>
		/// Gets the tracking origin state
		/// </summary>
		/// <returns>
		/// Returns the TrackingOrigin that was either set by default, or previous set by the application.
		/// </returns>
		/// <see cref="OVRTypes.TrackingOrigin"/>
		/// <see cref="SetTrackingOriginType"/>
		public OVRTypes.TrackingOrigin GetTrackingOriginType()
		{
			return OVR.GetTrackingOriginType(Session);
		}

		/// <summary>
		/// Re-centers the sensor position and orientation.
		///
		/// This resets the (x,y,z) positional components and the yaw orientation component.
		/// The Roll and pitch orientation components are always determined by gravity and cannot
		/// be redefined. All future tracking will report values relative to this new reference position.
		/// If you are using ovrTrackerPoses then you will need to call GetTrackerPose after 
		/// this, because the sensor position(s) will change as a result of this.
		/// 
		/// The headset cannot be facing vertically upward or downward but rather must be roughly
		/// level otherwise this function will fail with ovrError_InvalidHeadsetOrientation.
		///
		/// For more info, see the notes on each ovrTrackingOrigin enumeration to understand how
		/// recenter will vary slightly in its behavior based on the current ovrTrackingOrigin setting.
		/// </summary>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use
		/// Wrap.GetLastError to get more information. Return values include but aren't limited to:
		/// - Result.Success: Completed successfully.
		/// - Result.InvalidHeadsetOrientation: The headset was facing an invalid direction when attempting recentering, 
		///   such as facing vertically.
		/// </returns>
		/// <see cref="OVRTypes.TrackingOrigin"/>
		/// <see cref="GetTrackerPose"/>
		public OVRTypes.Result RecenterTrackingOrigin()
		{
			return OVR.RecenterTrackingOrigin(Session);
		}

		/// <summary>
		/// Clears the ShouldRecenter status bit in ovrSessionStatus.
		///
		/// Clears the ShouldRecenter status bit in ovrSessionStatus, allowing further recenter 
		/// requests to be detected. Since this is automatically done by RecenterTrackingOrigin,
		/// this is only needs to be called when application is doing its own re-centering.
		/// </summary>
		public void ClearShouldRecenterFlag()
		{
			OVR.ClearShouldRecenterFlag(Session);
		}

		/// <summary>
		/// Returns the ovrTrackerPose for the given sensor.
		/// </summary>
		/// <param name="trackerPoseIndex">Index of the sensor being requested.</param>
		/// <returns>
		/// Returns the requested ovrTrackerPose. An empty ovrTrackerPose will be returned if trackerPoseIndex is out of range.
		/// </returns>
		/// <see cref="GetTrackerCount"/>
		public OVRTypes.TrackerPose GetTrackerPose(uint trackerPoseIndex)
		{
			return OVR.GetTrackerPose(Session, trackerPoseIndex);
		}

		/// <summary>
		/// Returns the most recent input state for controllers, without positional tracking info.
		/// Developers can tell whether the same state was returned by checking the PacketNumber.
		/// </summary>
		/// <param name="controllerTypeMask">
		/// Specifies which controllers the input will be returned for.
		/// </param>
		/// <returns>Filled in Input state.</returns>
		/// <see cref="OVRTypes.ControllerType"/>
		public OculusWrap.OVRTypes.InputState GetInputState(OVRTypes.ControllerType controllerTypeMask)
		{
			OVRTypes.InputState inputState = new OVRTypes.InputState();
			OVR.GetInputState(Session, controllerTypeMask, ref inputState);

			return inputState;
		}

		/// <summary>
		/// Returns controller types connected to the system OR'ed together.
		/// </summary>
		/// <returns>A bitmask of ControllerTypes connected to the system.</returns>
		/// <see cref="OVRTypes.ControllerType"/>
		public OVRTypes.ControllerType ovr_GetConnectedControllerTypes()
		{
			return OVR.GetConnectedControllerTypes(Session);
		}

		/// <summary>
		/// Returns tracking state reading based on the specified absolute system time.
		///
		/// Pass an absTime value of 0.0 to request the most recent sensor reading. In this case
		/// both PredictedPose and SamplePose will have the same value.
		///
		/// This may also be used for more refined timing of front buffer rendering logic, and so on.
		/// This may be called by multiple threads.
		/// </summary>
		/// <param name="absTime">
		/// Specifies the absolute future time to predict the return
		/// TrackingState value. Use 0 to request the most recent tracking state.
		/// </param>
		/// <param name="latencyMarker">
		/// Specifies that this call is the point in time where
		/// the "App-to-Mid-Photon" latency timer starts from. If a given ovrLayer
		/// provides "SensorSampleTimestamp", that will override the value stored here.
		/// </param>
		/// <returns>
		/// Returns the TrackingState that is predicted for the given absTime.
		/// </returns>
		/// <see cref="OVRTypes.TrackingState"/>
		/// <see cref="GetEyePoses(long, bool, OVRTypes.Vector3f[], ref OVRTypes.Posef[])"/>
		/// <see cref="GetEyePoses(long, bool, OVRTypes.Vector3f[], ref OVRTypes.Posef[], out double)"/>
		/// <see cref="Wrap.GetTimeInSeconds"/>
		public OculusWrap.OVRTypes.TrackingState GetTrackingState(double absTime, bool latencyMarker)
		{
			OculusWrap.OVRTypes.TrackingState trackingState;
			OVR.GetTrackingState(out trackingState, Session, absTime, latencyMarker ? (byte) 1 : (byte) 0);

			return trackingState;
		}

		/// <summary>
		/// Turns on vibration of the given controller.
		///
		/// To disable vibration, call SetControllerVibration with an amplitude of 0.
		/// Vibration automatically stops after a nominal amount of time, so if you want vibration 
		/// to be continuous over multiple seconds then you need to call this function periodically.
		/// </summary>
		/// <param name="controllerTypeMask">Specifies controllers to apply the vibration to.</param>
		/// <param name="frequency">
		/// Specifies a vibration frequency in the range of 0.0 to 1.0. 
		/// Currently the only valid values are 0.0, 0.5, and 1.0 and other values will
		/// be clamped to one of these.
		/// </param>
		/// <param name="amplitude">Specifies a vibration amplitude in the range of 0.0 to 1.0.</param>
		/// <returns>Returns ovrSuccess upon success.</returns>
		/// <see cref="OVRTypes.ControllerType"/>
		public bool SetControllerVibration(OculusWrap.OVRTypes.ControllerType controllerTypeMask, float frequency, float amplitude)
		{
			OVRTypes.Result result = OVR.SetControllerVibration(Session, controllerTypeMask, frequency, amplitude);
			return result >= OVRTypes.Result.Success;
		}

		/// <summary>
		/// Calculates the recommended texture size for rendering a given eye within the HMD
		/// with a given FOV cone. Higher FOV will generally require larger textures to 
		/// maintain quality.
		///  - pixelsPerDisplayPixel specifies the ratio of the number of render target pixels 
		///    to display pixels at the center of distortion. 1.0 is the default value. Lower
		///    values can improve performance.
		/// </summary>
		public OculusWrap.OVRTypes.Sizei GetFovTextureSize(OculusWrap.OVRTypes.EyeType eye, OculusWrap.OVRTypes.FovPort fov, float pixelsPerDisplayPixel)
		{
			return OVR.GetFovTextureSize(Session, eye, fov, pixelsPerDisplayPixel);
		}

		/// <summary>
		/// Computes the distortion viewport, view adjust, and other rendering parameters for 
		/// the specified eye. This can be used instead of ovrHmd_ConfigureRendering to do 
		/// setup for client rendered distortion.
		/// </summary>
		public OculusWrap.OVRTypes.EyeRenderDesc GetRenderDesc(OculusWrap.OVRTypes.EyeType eyeType, OculusWrap.OVRTypes.FovPort fov)
		{
			return OVR.GetRenderDesc(Session, eyeType, fov);
		}

		/// <summary>
		/// Submits layers for distortion and display.
		/// 
		/// SubmitFrame triggers distortion and processing which might happen asynchronously. 
		/// The function will return when there is room in the submission queue and surfaces
		/// are available. Distortion might or might not have completed.
		/// </summary>
		/// <param name="frameIndex">
		/// Specifies the targeted frame index, or 0, to refer to one frame after the last
		/// time ovrHmd_SubmitFrame was called.
		/// </param>
		/// <param name="viewScaleDesc">
		/// Provides additional information needed only if layers list contains
		/// a LayerType.QuadInWorld or LayerType.QuadHeadLocked. 
		/// </param>
		/// <param name="layers">
		/// Specifies a list of Layers, which can include null entries to
		/// indicate that any previously shown layer at that index is to not be displayed.
		/// </param>
		/// <returns>
		/// Returns the result of the rendering.
		/// - Failure: an error occurred during rendering.
		/// - Success: rendering completed successfully.
		/// - NotVisible: rendering completed successfully but was not displayed on the HMD,
		///   usually because another application currently has ownership of the HMD. Applications receiving
		///   this result should stop rendering new content, but continue to call SubmitFrame periodically
		///   until it returns a value other than NotVisible.
		/// </returns>
		/// 
		/// <returns>
		/// Returns the result of the rendering.
		///     - Success: rendering completed successfully.
		///     - NotVisible: rendering completed successfully but was not displayed on the HMD,
		///       usually because another application currently has ownership of the HMD. Applications receiving
		///       this result should stop rendering new content, but continue to call SubmitFrame periodically
		///       until it returns a value other than NotVisible.
		///     - DisplayLost: The session has become invalid (such as due to a device removal)
		///       and the shared resources need to be disposed (OculusWrap.SwapTextureSet.Dispose) and 
		///       recreated (Wrap.Hmd_Create), and new resources need to be created (Hmd.CreateSwapTextureSetXXX). 
		///       The application's existing private graphics resources do not need to be recreated unless the 
		///       new Wrap.Hmd_Create call returns a different GraphicsLuid.
		/// </returns>
		public OVRTypes.Result SubmitFrame(uint frameIndex, OVRTypes.ViewScaleDesc viewScaleDesc, Layers layers)
		{
			IntPtr layerPtrList = layers.GetUnmanagedLayers();

			if(m_viewScaleDescPtr == IntPtr.Zero)
			{
				// Allocate the unmanaged ViewScaleDesc structure.
				int viewScaleDescSize	= Marshal.SizeOf(typeof(OVRTypes.ViewScaleDesc));
				m_viewScaleDescPtr		= Marshal.AllocHGlobal(viewScaleDescSize);
			}

			Marshal.StructureToPtr(viewScaleDesc, m_viewScaleDescPtr, false);

			OVRTypes.Result result = OVR.SubmitFrame(Session, frameIndex, m_viewScaleDescPtr, layerPtrList, (uint) layers.Count);
			return result;
		}

		/// <summary>
		/// Submits layers for distortion and display.
		///
		/// SubmitFrame triggers distortion and processing which might happen asynchronously. 
		/// The function will return when there is room in the submission queue and surfaces
		/// are available. Distortion might or might not have completed.
		/// </summary>
		/// <param name="frameIndex">
		/// Specifies the targeted frame index, or 0, to refer to one frame after the last
		/// time ovrHmd_SubmitFrame was called.
		/// </param>
		/// <param name="layers">
		/// Specifies a list of Layers, which can include null entries to
		/// indicate that any previously shown layer at that index is to not be displayed.
		/// </param>
		/// <returns>
		/// Returns the result of the rendering.
		/// - Failure: an error occurred during rendering.
		/// - Success: rendering completed successfully.
		/// - NotVisible: rendering completed successfully but was not displayed on the HMD,
		///   usually because another application currently has ownership of the HMD. Applications receiving
		///   this result should stop rendering new content, but continue to call SubmitFrame periodically
		///   until it returns a value other than NotVisible.
		/// </returns>
		public OVRTypes.Result SubmitFrame(uint frameIndex, Layers layers)
		{
			IntPtr layerPtrList = layers.GetUnmanagedLayers();

			OVRTypes.Result result = OVR.SubmitFrame(Session, frameIndex, IntPtr.Zero, layerPtrList, (uint) layers.Count);
			return result;
		}

		/// <summary>
		/// Gets the time of the specified frame midpoint.
		///
		/// Predicts the time at which the given frame will be displayed. The predicted time 
		/// is the middle of the time period during which the corresponding eye images will 
		/// be displayed. 
		/// 
		/// The application should increment frameIndex for each successively targeted frame,
		/// and pass that index to any relevent OVR functions that need to apply to the frame
		/// identified by that index. 
		/// 
		/// This function is thread-safe and allows for multiple application threads to target 
		/// their processing to the same displayed frame.
		/// </summary>
		/// <param name="frameIndex">
		/// Identifies the frame the caller wishes to target.
		/// A value of zero returns the next frame index.
		/// </param>
		/// <returns>
		/// Returns the absolute frame midpoint time for the given frameIndex.
		/// </returns>
		/// <see cref="Wrap.GetTimeInSeconds"/>
		public double GetPredictedDisplayTime(Int64 frameIndex)
		{
			return OVR.GetPredictedDisplayTime(Session, frameIndex);
		}

		/// <summary>
		/// Get boolean property. Returns first element if property is a boolean array.
		/// Returns defaultValue if property doesn't exist.
		/// </summary>
		public bool GetBool(string propertyName, bool defaultVal)
		{
			ovrBool result = OVR.GetBool(Session, propertyName, defaultVal ? (byte) 1 : (byte) 0);
			return result == 1;
		}

		/// <summary>
		/// Modify bool property; false if property doesn't exist or is readonly.
		/// </summary>
		public bool SetBool(string propertyName, bool value)
		{
			ovrBool result = OVR.SetBool(Session, propertyName, value ? (byte) 1 : (byte) 0);
			return result == 1;
		}
		
		/// <summary>
		/// Get integer property. Returns first element if property is an integer array.
		/// Returns defaultValue if property doesn't exist.
		/// </summary>
		public int GetInt(string propertyName, int defaultVal)
		{
			return OVR.GetInt(Session, propertyName, defaultVal);
		}
		
		/// <summary>
		/// Modify integer property; false if property doesn't exist or is readonly.
		/// </summary>
		public bool SetInt(string propertyName, int value)
		{
			ovrBool result = OVR.SetInt(Session, propertyName, value);
			return result == 1;
		}

		/// <summary>
		/// Get float property. Returns first element if property is a float array.
		/// Returns defaultValue if property doesn't exist.
		/// </summary>
		public float GetFloat(string propertyName, float defaultVal)
		{
			return OVR.GetFloat(Session, propertyName, defaultVal);
		}

		/// <summary>
		/// Modify float property; false if property doesn't exist or is readonly.
		/// </summary>
		public bool SetFloat(string propertyName, float value)
		{
			ovrBool result = OVR.SetFloat(Session, propertyName, value);
			return result == 1;
		}

		/// <summary>
		/// Get float[] property. Returns the number of elements filled in, 0 if property doesn't exist.
		/// Maximum of arraySize elements will be written.
		/// </summary>
		public uint GetFloatArray(string propertyName, ref float[] values)
		{
			uint result = OVR.GetFloatArray(Session, propertyName, values, (uint) values.Length);
			return result;
		}

		/// <summary>
		/// Modify float[] property; false if property doesn't exist or is readonly.
		/// </summary>
		public bool SetFloatArray(string propertyName, float[] values)
		{
			ovrBool result = OVR.SetFloatArray(Session, propertyName, values, (uint) values.Length);
			return result == 1;
		}

		/// <summary>
		/// Get string property. Returns first element if property is a string array.
		/// Returns defaultValue if property doesn't exist.
		/// String memory is guaranteed to exist until next call to GetString or GetStringArray, or HMD is destroyed.
		/// </summary>
		public string GetString(string propertyName, string defaultVal)
		{
			IntPtr resultPtr = OVR.GetString(Session, propertyName, defaultVal);
			string result = Marshal.PtrToStringAuto(resultPtr);
			return result;
		}

		/// <summary>
		/// Set string property
		/// </summary>
		public bool SetString(string propertyName, string value)
		{
			ovrBool result = OVR.SetString(Session, propertyName, value);
			return result == 1;
		}

		/// <summary>
		/// Returns the predicted head pose in HmdTrackingState and offset eye poses in outEyePoses. 
		/// 
		/// This is a thread-safe function where caller should increment frameIndex with every frame
		/// and pass that index where applicable to functions called on the rendering thread.
		/// Assuming outEyePoses are used for rendering, it should be passed as a part of ovrLayerEyeFov.
		/// The caller does not need to worry about applying HmdToEyeOffset to the returned outEyePoses variables.
		/// </summary>
		/// <param name="frameIndex">
		/// Specifies the targeted frame index, or 0 to refer to one frame after 
		/// the last time ovrHmd_SubmitFrame was called.
		/// </param>
		/// <param name="latencyMarker">
		/// Specifies that this call is the point in time where
		/// the "App-to-Mid-Photon" latency timer starts from. If a given ovrLayer
		/// provides "SensorSampleTimestamp", that will override the value stored here.
		/// </param>
		/// <param name="hmdToEyeViewOffset">
		/// Can be ovrEyeRenderDesc.HmdToEyeViewOffset returned from ovrHmd_GetRenderDesc. 
		/// For monoscopic rendering, use a vector that is the average of the two vectors for both eyes.
		/// </param>
		/// <param name="outEyePoses">
		/// The predicted eye poses.
		/// </param>
		public void GetEyePoses(long frameIndex, bool latencyMarker, OVRTypes.Vector3f[] hmdToEyeViewOffset, ref OVRTypes.Posef[] outEyePoses)
		{
			if(outEyePoses.Length != 2)
				throw new ArgumentException("The outEyePoses argument must contain 2 elements.", "outEyePoses");
			if(hmdToEyeViewOffset.Length != 2)
				throw new ArgumentException("The hmdToEyeViewOffset argument must contain 2 elements.", "hmdToEyeViewOffset");

			// Make sure that the m_eyePosesPtr is only allocated once and is reused in the future.
			if(m_eyePosesPtr == IntPtr.Zero)
			{
				// Allocate and copy managed struct to unmanaged memory.
				m_poseFSize		= Marshal.SizeOf(typeof(OVRTypes.Posef));
				m_eyePosesPtr	= Marshal.AllocHGlobal(m_poseFSize*2);
			}

			OVR.GetEyePoses(Session, frameIndex, latencyMarker ? (byte) 1 : (byte) 0, hmdToEyeViewOffset, m_eyePosesPtr, IntPtr.Zero);

			outEyePoses[0]		= (OVRTypes.Posef) Marshal.PtrToStructure(m_eyePosesPtr, typeof(OVRTypes.Posef));
			outEyePoses[1]		= (OVRTypes.Posef) Marshal.PtrToStructure(m_eyePosesPtr+m_poseFSize, typeof(OVRTypes.Posef));
		}

		/// <summary>
		/// Returns the predicted head pose in HmdTrackingState and offset eye poses in outEyePoses. 
		/// 
		/// This is a thread-safe function where caller should increment frameIndex with every frame
		/// and pass that index where applicable to functions called on the rendering thread.
		/// Assuming outEyePoses are used for rendering, it should be passed as a part of ovrLayerEyeFov.
		/// The caller does not need to worry about applying HmdToEyeOffset to the returned outEyePoses variables.
		/// </summary>
		/// <param name="frameIndex">
		/// Specifies the targeted frame index, or 0 to refer to one frame after 
		/// the last time ovrHmd_SubmitFrame was called.
		/// </param>
		/// <param name="latencyMarker">
		/// Specifies that this call is the point in time where
		/// the "App-to-Mid-Photon" latency timer starts from. If a given ovrLayer
		/// provides "SensorSampleTimestamp", that will override the value stored here.
		/// </param>
		/// <param name="hmdToEyeViewOffset">
		/// Can be ovrEyeRenderDesc.HmdToEyeViewOffset returned from ovrHmd_GetRenderDesc. 
		/// For monoscopic rendering, use a vector that is the average of the two vectors for both eyes.
		/// </param>
		/// <param name="outEyePoses">
		/// The predicted eye poses.
		/// </param>
		/// <param name="sensorSampleTime">
		/// The time when this function was called. 
		/// </param>
		public void GetEyePoses(long frameIndex, bool latencyMarker, OVRTypes.Vector3f[] hmdToEyeViewOffset, ref OVRTypes.Posef[] outEyePoses, out double sensorSampleTime)
		{
			if(outEyePoses.Length != 2)
				throw new ArgumentException("The outEyePoses argument must contain 2 elements.", "outEyePoses");
			if(hmdToEyeViewOffset.Length != 2)
				throw new ArgumentException("The hmdToEyeViewOffset argument must contain 2 elements.", "hmdToEyeViewOffset");

			// Make sure that the m_eyePosesPtr is only allocated once and is reused in the future.
			if(m_eyePosesPtr == IntPtr.Zero)
			{
				// Allocate and copy managed struct to unmanaged memory.
				m_poseFSize		= Marshal.SizeOf(typeof(OVRTypes.Posef));
				m_eyePosesPtr	= Marshal.AllocHGlobal(m_poseFSize*2);
			}

			// Make sure that the m_sensorSampleTimePtr is only allocated once and is reused in the future.
			if(m_sensorSampleTimePtr == IntPtr.Zero)
			{
				// Allocate and copy managed struct to unmanaged memory.
				m_sensorSampleTimeSize	= Marshal.SizeOf(typeof(double));
				m_sensorSampleTimePtr	= Marshal.AllocHGlobal(m_sensorSampleTimeSize);
			}

			OVR.GetEyePoses(Session, frameIndex, latencyMarker ? (byte) 1 : (byte) 0, hmdToEyeViewOffset, m_eyePosesPtr, m_sensorSampleTimePtr);

			outEyePoses[0]		= (OVRTypes.Posef) Marshal.PtrToStructure(m_eyePosesPtr, typeof(OVRTypes.Posef));
			outEyePoses[1]		= (OVRTypes.Posef) Marshal.PtrToStructure(m_eyePosesPtr+m_poseFSize, typeof(OVRTypes.Posef));
			sensorSampleTime	= (double) Marshal.PtrToStructure(m_sensorSampleTimePtr, typeof(double));
		}

		/// <summary>
		/// Create Texture Swap Chain suitable for use with Direct3D 11 and 12.
		/// </summary>
		/// <param name="d3dPtr">
		/// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue 
		/// which must be the same one the application renders to the eye textures with.</param>
		/// <param name="textureSwapChainDescription">Specifies requested texture properties. See notes for more info about texture format.</param>
		/// <param name="textureSwapChain">
		/// Returns the created TextureSwapChain, which will be valid upon a successful return value, else it will be null.
		/// This texture swap chain must be eventually disposed before disposing the Hmd.
		/// </param>
		/// <returns>
		/// Returns a Result indicating success or failure. In the case of failure, use 
		/// Wrap.GetLastError to get more information.
		/// </returns>
		/// <remarks>
		/// The texture format provided in desc should be thought of as the format the distortion-compositor will use for the
		/// ShaderResourceView when reading the contents of the texture. To that end, it is highly recommended that the application
		/// requests texture swapchain formats that are in sRGB-space (e.g. OVR_FORMAT_R8G8B8A8_UNORM_SRGB) as the compositor
		/// does sRGB-correct rendering. As such, the compositor relies on the GPU's hardware sampler to do the sRGB-to-linear
		/// conversion. If the application still prefers to render to a linear format (e.g. OVR_FORMAT_R8G8B8A8_UNORM) while handling the
		/// linear-to-gamma conversion via HLSL code, then the application must still request the corresponding sRGB format and also use
		/// the ovrTextureMisc_DX_Typeless flag in the ovrTextureSwapChainDesc's Flag field. This will allow the application to create
		/// a RenderTargetView that is the desired linear format while the compositor continues to treat it as sRGB. Failure to do so
		/// will cause the compositor to apply unexpected gamma conversions leading to gamma-curve artifacts. The ovrTextureMisc_DX_Typeless
		/// flag for depth buffer formats (e.g. OVR_FORMAT_D32_FLOAT) is ignored as they are always converted to be typeless.
		/// </remarks>
		/// <see cref="TextureSwapChain.GetLength"/>
		/// <see cref="TextureSwapChain.GetCurrentIndex"/>
		/// <see cref="TextureSwapChain.GetDescription"/>
		/// <see cref="TextureSwapChain.GetBufferDX"/>
		public OVRTypes.Result CreateTextureSwapChainDX(IntPtr d3dPtr, OVRTypes.TextureSwapChainDesc textureSwapChainDescription, out TextureSwapChain textureSwapChain)
		{
			ovrTextureSwapChain textureSwapChainPtr = IntPtr.Zero;
			OVRTypes.Result result = OVR.CreateTextureSwapChainDX(Session, d3dPtr, ref textureSwapChainDescription, ref textureSwapChainPtr);
			if(result < OVRTypes.Result.Success)
			{
				textureSwapChain = null;
				return result;
			}

			textureSwapChain				 = new TextureSwapChain(OVR, Session, textureSwapChainPtr);
			textureSwapChain.ObjectDisposed += OnTextureSwapChainDisposed;

			CreatedTextureSwapChains.Add(textureSwapChain);

			return result;
		}

		/// <summary>
		/// Create Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
		///
		/// A second call to ovr_CreateMirrorTextureDX for a given ovrSession before destroying the first one
		/// is not supported and will result in an error return.
		/// </summary>
		/// <param name="d3dPtr">
		/// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue
		/// which must be the same one the application renders to the textures with.
		/// </param>
		/// <param name="mirrorTextureDescription">Specifies requested texture properties. See notes for more info about texture format.</param>
		/// <param name="mirrorTexture">
		/// Returns the created MirrorTexture, which will be valid upon a successful return value, else it will be null.
		/// This mirror texture must be eventually disposed before disposing the Hmd.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// Wrap.GetLastError to get more information.
		/// </returns>
		/// <remarks>
		/// The texture format provided in desc should be thought of as the format the compositor will use for the RenderTargetView when
		/// writing into mirror texture. To that end, it is highly recommended that the application requests a mirror texture format that is
		/// in sRGB-space (e.g. OVRTypes.TextureFormat.R8G8B8A8_UNORM_SRGB) as the compositor does sRGB-correct rendering. If however the application wants
		/// to still read the mirror texture as a linear format (e.g. OVRTypes.TextureFormat.OVR_FORMAT_R8G8B8A8_UNORM) and handle the sRGB-to-linear conversion in
		/// HLSL code, then it is recommended the application still requests an sRGB format and also use the ovrTextureMisc_DX_Typeless flag in the
		/// ovrMirrorTextureDesc's Flags field. This will allow the application to bind a ShaderResourceView that is a linear format while the
		/// compositor continues to treat is as sRGB. Failure to do so will cause the compositor to apply unexpected gamma conversions leading to 
		/// gamma-curve artifacts.
		/// </remarks>
		/// <see cref="MirrorTexture.GetBufferDX"/>
		public OVRTypes.Result CreateMirrorTextureDX(IntPtr d3dPtr, OVRTypes.MirrorTextureDesc mirrorTextureDescription, out MirrorTexture mirrorTexture)
		{
			ovrMirrorTexture mirrorTexturePtr = IntPtr.Zero;
			OVRTypes.Result result = OVR.CreateMirrorTextureDX(Session, d3dPtr, ref mirrorTextureDescription, ref mirrorTexturePtr);
			if(result < OVRTypes.Result.Success)
			{
				mirrorTexture = null;
				return result;
			}

			mirrorTexture					 = new MirrorTexture(OVR, Session, mirrorTexturePtr);
			mirrorTexture.ObjectDisposed	+= OnMirrorTextureDisposed;

			CreatedMirrorTextures.Add(mirrorTexture);

			return result;
		}

		/// <summary>
		/// Creates a TextureSwapChain suitable for use with OpenGL.
		/// </summary>
		/// <param name="textureSwapChainDescription">Specifies the requested texture properties. See notes for more info about texture format.</param>
		/// <param name="textureSwapChain">
		/// Returns the created ovrTextureSwapChain, which will be valid upon
		/// a successful return value, else it will be null. This texture swap chain must be eventually 
		/// disposed before disposing the Hmd.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// Wrap.GetLastError to get more information.
		/// </returns>
		/// <remarks>
		/// The format provided should be thought of as the format the distortion compositor will use when reading
		/// the contents of the texture. To that end, it is highly recommended that the application requests texture swap chain
		/// formats that are in sRGB-space (e.g. Format.R8G8B8A8_UNORM_SRGB) as the distortion compositor does sRGB-correct
		/// rendering. Furthermore, the app should then make sure "glEnable(GL_FRAMEBUFFER_SRGB);" is called before rendering
		/// into these textures. Even though it is not recommended, if the application would like to treat the texture as a linear
		/// format and do linear-to-gamma conversion in GLSL, then the application can avoid calling "glEnable(GL_FRAMEBUFFER_SRGB);",
		/// but should still pass in an sRGB variant for the format. Failure to do so will cause the distortion compositor
		/// to apply incorrect gamma conversions leading to gamma-curve artifacts.		
		/// </remarks>
		/// <see cref="TextureSwapChain.GetLength"/>
		/// <see cref="TextureSwapChain.GetCurrentIndex"/>
		/// <see cref="TextureSwapChain.GetDescription"/>
		/// <see cref="TextureSwapChain.GetBufferGL"/>
		public OVRTypes.Result CreateTextureSwapChainGL(OVRTypes.TextureSwapChainDesc textureSwapChainDescription, out TextureSwapChain textureSwapChain)
		{
			ovrTextureSwapChain textureSwapChainPtr = IntPtr.Zero;
			OVRTypes.Result result = OVR.CreateTextureSwapChainGL(Session, textureSwapChainDescription, out textureSwapChainPtr);
			if(result < OVRTypes.Result.Success)
			{
				textureSwapChain = null;
				return result;
			}

			textureSwapChain				 = new TextureSwapChain(OVR, Session, textureSwapChainPtr);
			textureSwapChain.ObjectDisposed += OnTextureSwapChainDisposed;

			CreatedTextureSwapChains.Add(textureSwapChain);

			return result;
		}

		/// <summary>
		/// Creates a Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
		///
		/// A second call to ovr_CreateMirrorTextureGL for a given ovrSession before destroying the first one
		/// is not supported and will result in an error return.
		/// </summary>
		/// <param name="mirrorTextureDescription">Specifies the requested mirror texture description.</param>
		/// <param name="mirrorTexture">
		/// Specifies the created ovrMirrorTexture, which will be valid upon a successful return value, else it will be null.
		/// This texture must be eventually destroyed via ovr_DestroyMirrorTexture before destroying the HMD with ovr_Destroy.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		/// <remarks>
		/// The format provided should be thought of as the format the distortion compositor will use when writing into the mirror
		/// texture. It is highly recommended that mirror textures are requested as sRGB formats because the distortion compositor
		/// does sRGB-correct rendering. If the application requests a non-sRGB format (e.g. R8G8B8A8_UNORM) as the mirror texture,
		/// then the application might have to apply a manual linear-to-gamma conversion when reading from the mirror texture.
		/// Failure to do so can result in incorrect gamma conversions leading to gamma-curve artifacts and color banding.
		/// </remarks>
		/// <see cref="MirrorTexture.GetBufferGL"/>
		public OVRTypes.Result CreateMirrorTextureGL(OVRTypes.MirrorTextureDesc mirrorTextureDescription, out MirrorTexture mirrorTexture)
		{
			ovrMirrorTexture mirrorTexturePtr = IntPtr.Zero;
			OVRTypes.Result result = OVR.CreateMirrorTextureGL(Session, mirrorTextureDescription, out mirrorTexturePtr);
			if(result < OVRTypes.Result.Success)
			{
				mirrorTexture = null;
				return result;
			}

			mirrorTexture					 = new MirrorTexture(OVR, Session, mirrorTexturePtr);
			mirrorTexture.ObjectDisposed	+= OnMirrorTextureDisposed;

			CreatedMirrorTextures.Add(mirrorTexture);

			return result;
		}
		#endregion

		#region OvrHmd properties
		/// <summary>
		/// This HMD's type.
		/// </summary>
		public OculusWrap.OVRTypes.HmdType Type
		{
			get
			{
				return OvrHmd.Type;
			}
		}
    
		/// <summary>
		/// Name string describing the product: "Oculus Rift DK1", etc.
		/// </summary>
		public string ProductName
		{
			get
			{
				string result = Encoding.ASCII.GetString(OvrHmd.ProductName).TrimEnd((char) 0);
				return result;
			}
		}

		/// <summary>
		/// HMD manufacturer identification string.
		/// </summary>
		public string Manufacturer
		{
			get
			{
				string result = Encoding.ASCII.GetString(OvrHmd.Manufacturer).TrimEnd((char) 0);
				return result;
			}
		}
    
		/// <summary>
		/// HID Vendor and ProductId of the device.
		/// </summary>
		public short VendorId
		{
			get
			{
				return OvrHmd.VendorId;
			}
		}

		/// <summary>
		/// HID (USB) product identifier of the device.
		/// </summary>
		public short ProductId
		{
			get
			{
				return OvrHmd.ProductId;
			}
		}

		/// <summary>
		/// Sensor (and display) serial number.
		/// </summary>
		public string SerialNumber
		{
			get
			{
				string result = Encoding.ASCII.GetString(OvrHmd.SerialNumber).TrimEnd((char) 0);
				return result;
			}
		}

		/// <summary>
		/// Sensor firmware version.
		/// </summary>
		public short FirmwareMajor
		{
			get
			{
				return OvrHmd.FirmwareMajor;
			}
		}

		/// <summary>
		/// HMD firmware minor version.
		/// </summary>
		public short FirmwareMinor
		{
			get
			{
				return OvrHmd.FirmwareMinor;
			}
		}

		/// <summary>
		/// Capability bits described by HmdCaps which the HMD currently supports.
		/// </summary>
		public OVRTypes.HmdCaps AvailableHmdCaps
		{
			get
			{
				return OvrHmd.AvailableHmdCaps;
			}
		}

		/// <summary>
		/// Capability bits described by HmdCaps which are default for the current Hmd.
		/// </summary>
		public OVRTypes.HmdCaps DefaultHmdCaps
		{
			get
			{
				return OvrHmd.DefaultHmdCaps;
			}
		}

		/// <summary>
		/// Capability bits described by TrackingCaps which the system currently supports.
		/// </summary>
		public OVRTypes.TrackingCaps AvailableTrackingCaps
		{
			get
			{
				return OvrHmd.AvailableTrackingCaps;
			}
		}

		/// <summary>
		/// Capability bits described by ovrTrackingCaps which are default for the current system.
		/// </summary>
		public OVRTypes.TrackingCaps DefaultTrackingCaps
		{
			get
			{
				return OvrHmd.DefaultTrackingCaps;
			}
		}

		/// <summary>
		/// These define the recommended and maximum optical FOVs for the HMD.    
		/// </summary>
		public OVRTypes.FovPort[] DefaultEyeFov
		{
			get
			{
				return OvrHmd.DefaultEyeFov;
			}
		}

		/// <summary>
		/// Defines the maximum FOVs for the HMD.
		/// </summary>
		public OVRTypes.FovPort[] MaxEyeFov
		{
			get
			{
				return OvrHmd.MaxEyeFov;
			}
		}

		/// <summary>
		/// Resolution of the full HMD screen (both eyes) in pixels.
		/// </summary>
		public OVRTypes.Sizei Resolution
		{
			get
			{
				return OvrHmd.Resolution;
			}
		}
		#endregion

		#region Private helper methods
		/// <summary>
		/// Frees the memory at the specified pointer and sets the pointer to IntPtr.Zero.
		/// </summary>
		/// <param name="pointer">Pointer to memory that will be freed.</param>
		/// <remarks>
		/// This method is used to free memory previously allocated by a call to Marshal.AllocHGlobal(...).
		/// </remarks>
		private void FreeHGlobal(ref IntPtr pointer)
		{
			if(pointer != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(pointer);
				pointer = IntPtr.Zero;
			}
		}
		#endregion

		#region Private event subscriptions
		/// <summary>
		/// Remove the texture swap chain from the list of texture swap chains to dispose.
		/// </summary>
		/// <param name="textureSwapChain">Texture swap chain that was disposed.</param>
		private void OnTextureSwapChainDisposed(TextureSwapChain textureSwapChain)
		{
			CreatedTextureSwapChains.Remove(textureSwapChain);

			textureSwapChain.ObjectDisposed -= OnTextureSwapChainDisposed;
		}

		/// <summary>
		/// Remove the mirror texture from the list of mirror textures to dispose.
		/// </summary>
		/// <param name="mirrorTexture">Mirror texture that was disposed.</param>
		private void OnMirrorTextureDisposed(MirrorTexture mirrorTexture)
		{
			CreatedMirrorTextures.Remove(mirrorTexture);

			mirrorTexture.ObjectDisposed -= OnMirrorTextureDisposed;
		}
		#endregion

		#region Private properties
		/// <summary>
		/// Pointer to the native session object.
		/// </summary>
		private ovrSession Session
		{
			get;
			set;
		}

		/// <summary>
		/// .NET wrapped Hmd structure, containing information about the head mounted display.
		/// </summary>
		private OVRTypes.HmdDesc OvrHmd
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

		/// <summary>
		/// Set of created TextureSwapChains.
		/// </summary>
		/// <remarks>
		/// This set is used to ensure that all created TextureSwapChains are destroyed.
		/// </remarks>
		private HashSet<TextureSwapChain> CreatedTextureSwapChains
		{
			get
			{
				return m_createdSwapTextureSets;
			}
		}

		/// <summary>
		/// Set of created MirrorTextures.
		/// </summary>
		/// <remarks>
		/// This set is used to ensure that all created MirrorTextures are destroyed.
		/// </remarks>
		private HashSet<MirrorTexture> CreatedMirrorTextures
		{
			get
			{
				return m_createdMirrorTextures;
			}
		}

		#endregion

		#region Private unmanaged memory fields
		private IntPtr	m_configDataPtr			= IntPtr.Zero;
		private	IntPtr	m_texturesPtr			= IntPtr.Zero;
		private	IntPtr	m_mirrorTexturePtr		= IntPtr.Zero;
		private	IntPtr	m_distortionMeshPtr		= IntPtr.Zero;
		private IntPtr	m_textureSetPtr			= IntPtr.Zero;
		private IntPtr	m_eyePosesPtr			= IntPtr.Zero;
		private IntPtr	m_sensorSampleTimePtr	= IntPtr.Zero;
		private IntPtr	m_viewScaleDescPtr		= IntPtr.Zero;

		private int m_poseFSize				= 0;
		private int m_sensorSampleTimeSize	= 0;
		#endregion

		#region Private managed memory fields
		private HashSet<TextureSwapChain>	m_createdSwapTextureSets	= new HashSet<TextureSwapChain>();
		private HashSet<MirrorTexture>		m_createdMirrorTextures		= new HashSet<MirrorTexture>();
		#endregion
	}
}
