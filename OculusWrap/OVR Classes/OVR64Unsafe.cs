using System;
using System.Runtime.InteropServices;
using ovrBool = System.Byte;
using ovrSession = System.IntPtr;
using ovrTextureSwapChain = System.IntPtr;
using ovrMirrorTexture = System.IntPtr;
using System.Security;
using System.IO;
using System.ComponentModel;

namespace OculusWrap
{
	/// <summary>
	/// 64 bit implementation of Oculus runtime methods, loaded using an unsafe implementation.
	/// 
	/// The unsafe implementation provides a faster execution, by bypassing the managed security checks that are normally
	/// performed when transitioning from managed to unmanaged code.
	/// 
	/// This class will load the Oculus runtime methods from the 64 bit Oculus runtime dll.
	/// 
	/// It's not recommended that you use this class directly, unless you want to create your own set of .NET wrapper classes.
	/// Instead, use the Wrap and Hmd classes, which take care of the marshalling of arguments.
	/// </summary>
    public class OVR64Unsafe	:OVRBase
    {
		/// <summary>
		/// Filename of the DllOVR wrapper file, which wraps the LibOvr.lib in a dll.
		/// </summary>
		public const string DllOvrDll = "LibOVRRT64_1.dll";

		#region Oculus runtime initialization methods
		/// <summary>
		/// Loads the Oculus runtime dll into memory.
		/// </summary>
		/// <returns>Pointer to the loaded Oculus runtime module.</returns>
		protected override void LoadLibrary()
		{
			if(OculusRuntimeDll != null)
				return;

			// Check that the DllOvrDll file exists.
			bool exists = File.Exists(DllOvrDll);
			if(!exists)
				throw new DllNotFoundException("Unable to load the file \""+DllOvrDll+"\", the file wasn't found.");

			OculusRuntimeDll = LoadLibrary(DllOvrDll);
			if(OculusRuntimeDll == IntPtr.Zero)
			{
				int win32Error = Marshal.GetLastWin32Error();
				throw new Win32Exception(win32Error, "Unable to load the file \""+DllOvrDll+"\", LoadLibrary reported error code: "+win32Error+".");
			}
		}

		/// <summary>
		/// Unloads the Oculus runtime library from memory.
		/// </summary>
		protected override void UnloadLibrary()
		{
			if(OculusRuntimeDll == IntPtr.Zero)
				return;

			bool success = FreeLibrary(OculusRuntimeDll);
			if(success)
				OculusRuntimeDll = IntPtr.Zero;
		}
		#endregion

		#region Kernel32 Platform invoke methods
		/// <summary>
		/// Loads a dll into process memory.
		/// </summary>
		/// <param name="lpFileName">Filename to load.</param>
		/// <returns>Pointer to the loaded library.</returns>
		/// <remarks>
		/// This method is used to load the DllOVR.dll into memory, before calling any of it's DllImported methods.
		/// </remarks>
		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32", SetLastError=true, CharSet = CharSet.Ansi)]
		private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

		/// <summary>
		/// Frees a previously loaded dll, from process memory.
		/// </summary>
		/// <param name="hModule">Pointer to the previously loaded library (This pointer comes from a call to LoadLibrary).</param>
		/// <returns>Returns true if the library was successfully freed.</returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", SetLastError=true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeLibrary(IntPtr hModule);
		#endregion

		// The Oculus SDK is full of missing comments.
		// Ignore warnings regarding missing comments, in this class.
		#pragma warning disable 1591

		#region Oculus SDK methods
		/// <summary>
		/// Detects Oculus Runtime and Device Status
		///
		/// Checks for Oculus Runtime and Oculus HMD device status without loading the LibOVRRT
		/// shared library.  This may be called before ovr_Initialize() to help decide whether or
		/// not to initialize LibOVR.
		/// </summary>
		/// <param name="timeoutMilliseconds">Specifies a timeout to wait for HMD to be attached or 0 to poll.</param>
		/// <returns>Returns a DetectResult object indicating the result of detection.</returns>
		/// <see cref="OVRTypes.DetectResult"/>
		public override DetectResult Detect(int timeoutMilliseconds)
		{
			return ovr_Detect(timeoutMilliseconds);
		}

		/// <summary>
		/// Initializes all Oculus functionality.
		/// </summary>
		/// <param name="parameters">
		/// Initialize with extra parameters.
		/// Pass 0 to initialize with default parameters, suitable for released games.
		/// </param>
		/// <remarks>
		/// Library init/shutdown, must be called around all other OVR code.
		/// No other functions calls besides ovr_InitializeRenderingShim are allowed
		/// before ovr_Initialize succeeds or after ovr_Shutdown.
		/// 
		/// LibOVRRT shared library search order:
		///      1) Current working directory (often the same as the application directory).
		///      2) Module directory (usually the same as the application directory, but not if the module is a separate shared library).
		///      3) Application directory
		///      4) Development directory (only if OVR_ENABLE_DEVELOPER_SEARCH is enabled, which is off by default).
		///      5) Standard OS shared library search location(s) (OS-specific).
		/// </remarks>
		public override Result Initialize(InitParams parameters=null)
		{
			return ovr_Initialize(parameters);
		}

		/// <summary>
		/// Returns information about the most recent failed return value by the
		/// current thread for this library.
		/// 
		/// This function itself can never generate an error.
		/// The last error is never cleared by LibOVR, but will be overwritten by new errors.
		/// Do not use this call to determine if there was an error in the last API 
		/// call as successful API calls don't clear the last ErrorInfo.
		/// To avoid any inconsistency, ovr_GetLastErrorInfo should be called immediately
		/// after an API function that returned a failed ovrResult, with no other API
		/// functions called in the interim.
		/// </summary>
		/// <param name="errorInfo">The last ErrorInfo for the current thread.</param>
		/// <remarks>
		/// Allocate an ErrorInfo and pass this as errorInfo argument.
		/// </remarks>
		public override void GetLastErrorInfo(out ErrorInfo errorInfo)
		{
			ovr_GetLastErrorInfo(out errorInfo);
		}

		/// <summary>
		/// Returns version string representing libOVR version. Static, so
		/// string remains valid for app lifespan
		/// </summary>
		/// <remarks>
		/// Use Marshal.PtrToStringAnsi() to retrieve version string.
		/// </remarks>
		public override IntPtr GetVersionString()
		{
			return ovr_GetVersionString();
		}

		/// <summary>
		/// Send a message string to the system tracing mechanism if enabled (currently Event Tracing for Windows)
		/// </summary>
		/// <param name="level">
		/// One of the ovrLogLevel constants.
		/// </param>
		/// <param name="message">
		/// A UTF8-encoded null-terminated string.
		/// </param>
		/// <returns>
		/// Returns the length of the message, or -1 if message is too large
		/// </returns>
		public override int TraceMessage(LogLevel level, string message)
		{
			return ovr_TraceMessage(level, message);
		}

		/// <summary>
		/// Shuts down all Oculus functionality.
		/// </summary>
		/// <remarks>
		/// No API functions may be called after ovr_Shutdown except ovr_Initialize.
		/// </remarks>
		public override void Shutdown()
		{
			ovr_Shutdown();
		}

		/// <summary>
		/// Returns information about the current HMD.
		/// 
		/// ovr_Initialize must have first been called in order for this to succeed, otherwise HmdDesc.Type
		/// will be reported as None.
		/// 
		/// Please note: This method will should only be called by a 32 bit process. 
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create, else NULL in which
		/// case this function detects whether an HMD is present and returns its info if so.
		/// </param>
		/// <param name="result">
		/// Returns an ovrHmdDesc. If the hmd is null and ovrHmdDesc::Type is ovr_None then
		/// no HMD is present.
		/// </param>
		public override void GetHmdDesc32(out HmdDesc result, ovrSession session)
		{
			ovr_GetHmdDesc32(out result, session);
		}

		/// <summary>
		/// Returns information about the current HMD.
		/// 
		/// ovr_Initialize must have first been called in order for this to succeed, otherwise HmdDesc.Type
		/// will be reported as None.
		/// 
		/// Please note: This method will should only be called by a 64 bit process. 
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create, else NULL in which
		/// case this function detects whether an HMD is present and returns its info if so.
		/// </param>
		/// <param name="result">
		/// Returns an ovrHmdDesc. If the hmd is null and ovrHmdDesc::Type is ovr_None then
		/// no HMD is present.
		/// </param>
		public override void GetHmdDesc64(out HmdDesc64 result, ovrSession session)
		{
			ovr_GetHmdDesc64(out result, session);
		}

		/// <summary>
		/// Returns the number of sensors. 
		///
		/// The number of sensors may change at any time, so this function should be called before use 
		/// as opposed to once on startup.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>Returns unsigned int count.</returns>
		public override uint GetTrackerCount(ovrSession session)
		{
			return ovr_GetTrackerCount(session);
		}

		/// <summary>
		/// Returns a given sensor description.
		///
		/// It's possible that sensor desc [0] may indicate a unconnnected or non-pose tracked sensor, but 
		/// sensor desc [1] may be connected.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="trackerDescIndex">
		/// Specifies a sensor index. The valid indexes are in the range of 0 to the sensor count returned by ovr_GetTrackerCount.
		/// </param>
		/// <returns>An empty ovrTrackerDesc will be returned if trackerDescIndex is out of range.</returns>
		/// <see cref="OVRTypes.TrackerDesc"/>
		/// <see cref="GetTrackerCount"/>
		public override TrackerDesc GetTrackerDesc(ovrSession session, uint trackerDescIndex)
		{
			return ovr_GetTrackerDesc(session, trackerDescIndex);
		}

		/// <summary>
		/// Creates a handle to a VR session.
		/// 
		/// Upon success the returned ovrSession must be eventually freed with ovr_Destroy when it is no longer needed.
		/// A second call to ovr_Create will result in an error return value if the previous Hmd has not been destroyed.
		/// </summary>
		/// <param name="session">
		/// Provides a pointer to an ovrSession which will be written to upon success.
		/// </param>
		/// <param name="pLuid">
		/// Provides a system specific graphics adapter identifier that locates which
		/// graphics adapter has the HMD attached. This must match the adapter used by the application
		/// or no rendering output will be possible. This is important for stability on multi-adapter systems. An
		/// application that simply chooses the default adapter will not run reliably on multi-adapter systems.
		/// </param>
		/// <remarks>
		/// Call Marshal.PtrToStructure(...) to convert the IntPtr to the OVR.ovrHmd type.
		/// </remarks>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. Upon failure
		/// the returned pHmd will be null.
		/// </returns>
		/// <example>
		/// <code>
		/// ovrSession session
		/// ovrGraphicsLuid luid
		/// ovrResult result = ovr_Create(ref session, ref luid)
		/// if(OVR_FAILURE(result))
		/// ...
		/// </code>
		/// </example>
		/// <see cref="Destroy"/>
		public override Result Create(ref ovrSession session, ref GraphicsLuid pLuid)
		{
			return ovr_Create(ref session, ref pLuid);
		}

		/// <summary>
		/// Destroys the HMD.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		public override void Destroy(ovrSession session)
		{
			ovr_Destroy(session);
		}

		/// <summary>
		/// Returns status information for the application.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="sessionStatus">Provides a SessionStatus that is filled in.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use ovr_GetLastErrorInfo 
		/// to get more information.
		/// Return values include but aren't limited to:
		/// - Result.Success: Completed successfully.
		/// - Result.ServiceConnection: The service connection was lost and the application must destroy the session.
		/// </returns>
		public override Result GetSessionStatus(ovrSession session, ref SessionStatus sessionStatus)
		{
			return ovr_GetSessionStatus(session, ref sessionStatus);
		}

		/// <summary>
		/// Sets the tracking origin type
		///
		/// When the tracking origin is changed, all of the calls that either provide
		/// or accept ovrPosef will use the new tracking origin provided.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="origin">Specifies an ovrTrackingOrigin to be used for all ovrPosef</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. 
		/// In the case of failure, use ovr_GetLastErrorInfo to get more information.
		/// </returns>
		/// <see cref="OVRTypes.TrackingOrigin"/>
		/// <see cref="GetTrackingOriginType"/>
		public override Result SetTrackingOriginType(ovrSession session, TrackingOrigin origin)
		{
			return ovr_SetTrackingOriginType(session, origin);
		}

		/// <summary>
		/// Gets the tracking origin state
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>Returns the TrackingOrigin that was either set by default, or previous set by the application.</returns>
		/// <see cref="OVRTypes.TrackingOrigin"/>
		/// <see cref="SetTrackingOriginType"/>
		public override TrackingOrigin GetTrackingOriginType(ovrSession session)
		{
			return ovr_GetTrackingOriginType(session);
		}

		/// <summary>
		/// Re-centers the sensor position and orientation.
		///
		/// This resets the (x,y,z) positional components and the yaw orientation component.
		/// The Roll and pitch orientation components are always determined by gravity and cannot
		/// be redefined. All future tracking will report values relative to this new reference position.
		/// If you are using ovrTrackerPoses then you will need to call ovr_GetTrackerPose after 
		/// this, because the sensor position(s) will change as a result of this.
		/// 
		/// The headset cannot be facing vertically upward or downward but rather must be roughly
		/// level otherwise this function will fail with ovrError_InvalidHeadsetOrientation.
		///
		/// For more info, see the notes on each ovrTrackingOrigin enumeration to understand how
		/// recenter will vary slightly in its behavior based on the current ovrTrackingOrigin setting.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use
		/// ovr_GetLastErrorInfo to get more information. Return values include but aren't limited to:
		/// - Result.Success: Completed successfully.
		/// - Result.InvalidHeadsetOrientation: The headset was facing an invalid direction when attempting recentering, 
		///   such as facing vertically.
		/// </returns>
		/// <see cref="OVRTypes.TrackingOrigin"/>
		/// <see cref="GetTrackerPose"/>
		public override Result RecenterTrackingOrigin(ovrSession session)
		{
			return ovr_RecenterTrackingOrigin(session);
		}
		
		/// <summary>
		/// Clears the ShouldRecenter status bit in ovrSessionStatus.
		///
		/// Clears the ShouldRecenter status bit in ovrSessionStatus, allowing further recenter 
		/// requests to be detected. Since this is automatically done by ovr_RecenterTrackingOrigin,
		/// this is only needs to be called when application is doing its own re-centering.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		public override void ClearShouldRecenterFlag(ovrSession session)
		{
			ovr_ClearShouldRecenterFlag(session);
		}

		/// <summary>
		/// Returns the ovrTrackerPose for the given sensor.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="trackerPoseIndex">Index of the sensor being requested.</param>
		/// <returns>
		/// Returns the requested ovrTrackerPose. An empty ovrTrackerPose will be returned if trackerPoseIndex is out of range.
		/// </returns>
		/// <see cref="GetTrackerCount"/>
		public override TrackerPose GetTrackerPose(ovrSession session, uint trackerPoseIndex)
		{
			return ovr_GetTrackerPose(session, trackerPoseIndex);
		}

		/// <summary>
		/// Returns the most recent input state for controllers, without positional tracking info.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="controllerType">Specifies which controller the input will be returned for.</param>
		/// <param name="inputState">Input state that will be filled in.</param>
		/// <returns>Returns Result.Success if the new state was successfully obtained.</returns>
		/// <see cref="OVRTypes.ControllerType"/>
		public override Result GetInputState(ovrSession session, ControllerType controllerType, ref InputState inputState)
		{
			return ovr_GetInputState(session, controllerType, ref inputState);
		}
		
		/// <summary>
		/// Returns controller types connected to the system OR'ed together.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>A bitmask of ControllerTypes connected to the system.</returns>
		/// <see cref="OVRTypes.ControllerType"/>
		public override ControllerType GetConnectedControllerTypes(ovrSession session)
		{
			return ovr_GetConnectedControllerTypes(session);
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
		/// <param name="result">Returns the TrackingState that is predicted for the given absTime.</param>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="absTime">
		/// Specifies the absolute future time to predict the return
		/// TrackingState value. Use 0 to request the most recent tracking state.
		/// </param>
		/// <param name="latencyMarker">
		/// Specifies that this call is the point in time where
		/// the "App-to-Mid-Photon" latency timer starts from. If a given ovrLayer
		/// provides "SensorSampleTimestamp", that will override the value stored here.
		/// </param>
		/// <see cref="OVRTypes.TrackingState"/>
		/// <see cref="GetEyePoses"/>
		/// <see cref="GetTimeInSeconds"/>
		public override void GetTrackingState(out TrackingState result, ovrSession session, double absTime, ovrBool latencyMarker)
		{
			ovr_GetTrackingState(out result, session, absTime, latencyMarker);
		}

		/// <summary>
		/// Turns on vibration of the given controller.
		///
		/// To disable vibration, call ovr_SetControllerVibration with an amplitude of 0.
		/// Vibration automatically stops after a nominal amount of time, so if you want vibration 
		/// to be continuous over multiple seconds then you need to call this function periodically.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="controllerType">Specifies controllers to apply the vibration to.</param>
		/// <param name="frequency">
		/// Specifies a vibration frequency in the range of 0.0 to 1.0. 
		/// Currently the only valid values are 0.0, 0.5, and 1.0 and other values will
		/// be clamped to one of these.
		/// </param>
		/// <param name="amplitude">Specifies a vibration amplitude in the range of 0.0 to 1.0.</param>
		/// <returns>Returns ovrSuccess upon success.</returns>
		/// <see cref="OVRTypes.ControllerType"/>
		public override Result SetControllerVibration(ovrSession session, ControllerType controllerType, float frequency, float amplitude)
		{
			return ovr_SetControllerVibration(session, controllerType, frequency, amplitude);
		}

		// SDK Distortion Rendering
		//
		// All of rendering functions including the configure and frame functions
		// are not thread safe. It is OK to use ConfigureRendering on one thread and handle
		// frames on another thread, but explicit synchronization must be done since
		// functions that depend on configured state are not reentrant.
		//
		// These functions support rendering of distortion by the SDK.

		// ovrTextureSwapChain creation is rendering API-specific.
		// ovr_CreateTextureSwapChainDX and ovr_CreateTextureSwapChainGL can be found in the
		// rendering API-specific headers, such as OVR_CAPI_D3D.h and OVR_CAPI_GL.h


		/// <summary>
		/// Gets the number of buffers in an ovrTextureSwapChain.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain for which the length should be retrieved.</param>
		/// <param name="out_Length">Returns the number of buffers in the specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		public override Result GetTextureSwapChainLength(ovrSession session, ovrTextureSwapChain chain, out int out_Length)
		{
			return ovr_GetTextureSwapChainLength(session, chain, out out_Length);
		}

		/// <summary>
		/// Gets the current index in an ovrTextureSwapChain.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain for which the index should be retrieved.</param>
		/// <param name="out_Index">Returns the current (free) index in specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		public override Result GetTextureSwapChainCurrentIndex(ovrSession session, ovrTextureSwapChain chain, out int out_Index)
		{
			return ovr_GetTextureSwapChainCurrentIndex(session, chain, out out_Index);
		}

		/// <summary>
		/// Gets the description of the buffers in an ovrTextureSwapChain
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain for which the description should be retrieved.</param>
		/// <param name="out_Desc">Returns the description of the specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		public override Result GetTextureSwapChainDesc(ovrSession session, ovrTextureSwapChain chain, ref TextureSwapChainDesc out_Desc)
		{
			return ovr_GetTextureSwapChainDesc(session, chain, ref out_Desc);
		}

		/// <summary>
		/// Commits any pending changes to an ovrTextureSwapChain, and advances its current index
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain to commit.</param>
		/// <returns>
		/// Returns an ovrResult for which the return code is negative upon error.
		/// Failures include but aren't limited to:
		///   - Result.TextureSwapChainFull: ovr_CommitTextureSwapChain was called too many times on a texture swapchain without calling submit to use the chain.
		/// </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		public override Result CommitTextureSwapChain(ovrSession session, ovrTextureSwapChain chain)
		{
			return ovr_CommitTextureSwapChain(session, chain);
		}

		/// <summary>
		/// Destroys an ovrTextureSwapChain and frees all the resources associated with it.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain to destroy. If it is null then this function has no effect.</param>
		public override void DestroyTextureSwapChain(ovrSession session, ovrTextureSwapChain chain)
		{
			ovr_DestroyTextureSwapChain(session, chain);
		}

		// MirrorTexture creation is rendering API-specific.

		/// <summary>
		/// Destroys a mirror texture previously created by one of the mirror texture creation functions.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="mirrorTexture">
		/// Specifies the ovrTexture to destroy. If it is null then this function has no effect.
		/// </param>
		/// <see cref="CreateMirrorTextureDX"/>
		/// <see cref="CreateMirrorTextureGL"/>
		public override void DestroyMirrorTexture(ovrSession session, ovrMirrorTexture mirrorTexture)
		{
			ovr_DestroyMirrorTexture(session, mirrorTexture);
		}

		/// <summary>
		/// Calculates the recommended viewport size for rendering a given eye within the HMD
		/// with a given FOV cone. 
		/// 
		/// Higher FOV will generally require larger textures to maintain quality.
		/// Apps packing multiple eye views together on the same texture should ensure there are
		/// at least 8 pixels of padding between them to prevent texture filtering and chromatic
		/// aberration causing images to leak between the two eye views.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="eye">
		/// Specifies which eye (left or right) to calculate for.
		/// </param>
		/// <param name="fov">
		/// Specifies the ovrFovPort to use.
		/// </param>
		/// <param name="pixelsPerDisplayPixel">
		/// pixelsPerDisplayPixel Specifies the ratio of the number of render target pixels 
		/// to display pixels at the center of distortion. 1.0 is the default value. Lower
		/// values can improve performance, higher values give improved quality.
		/// </param>
		/// <returns>
		/// Returns the texture width and height size.
		/// </returns>
		public override Sizei GetFovTextureSize(ovrSession session, EyeType eye, FovPort fov, float pixelsPerDisplayPixel)
		{
			return ovr_GetFovTextureSize(session, eye, fov, pixelsPerDisplayPixel);
		}

		/// <summary>
		/// Computes the distortion viewport, view adjust, and other rendering parameters for
		/// the specified eye.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="eyeType">
		/// Specifies which eye (left or right) for which to perform calculations.
		/// </param>
		/// <param name="fov">
		/// Specifies the FovPort to use.
		/// </param>
		/// <returns>
		/// Returns the computed EyeRenderDesc for the given eyeType and field of view.
		/// </returns>
		/// <see cref="OVRTypes.EyeRenderDesc"/>
		public override EyeRenderDesc GetRenderDesc(ovrSession session, EyeType eyeType, FovPort fov)
		{
			return ovr_GetRenderDesc(session, eyeType, fov);
		}

		/// <summary>
		/// Submits layers for distortion and display.
		/// 
		/// ovr_SubmitFrame triggers distortion and processing which might happen asynchronously. 
		/// The function will return when there is room in the submission queue and surfaces
		/// are available. Distortion might or might not have completed.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="frameIndex">
		/// Specifies the targeted application frame index, or 0 to refer to one frame 
		/// after the last time ovr_SubmitFrame was called.
		/// </param>
		/// <param name="viewScaleDesc">
		/// Provides additional information needed only if layerPtrList contains
		/// an ovrLayerType_Quad. If null, a default version is used based on the current configuration and a 1.0 world scale.
		/// </param>
		/// <param name="layerPtrList">
		/// Specifies a list of ovrLayer pointers, which can include null entries to
		/// indicate that any previously shown layer at that index is to not be displayed.
		/// Each layer header must be a part of a layer structure such as ovrLayerEyeFov or ovrLayerQuad,
		/// with Header.Type identifying its type. A null layerPtrList entry in the array indicates the 
		/// absence of the given layer.
		/// </param>
		/// <param name="layerCount">
		/// Indicates the number of valid elements in layerPtrList. The maximum supported layerCount 
		/// is not currently specified, but may be specified in a future version.
		/// </param>
		/// <returns>
		/// Returns an ovrResult for which the return code is negative upon error and positive
		/// upon success. Return values include but aren't limited to:
		///     - Result.Success: rendering completed successfully.
		///     - Result.NotVisible: rendering completed successfully but was not displayed on the HMD,
		///       usually because another application currently has ownership of the HMD. Applications receiving
		///       this result should stop rendering new content, but continue to call ovr_SubmitFrame periodically
		///       until it returns a value other than ovrSuccess_NotVisible.
		///     - Result.DisplayLost: The session has become invalid (such as due to a device removal)
		///       and the shared resources need to be released (ovr_DestroyTextureSwapChain), the session needs to
		///       destroyed (ovr_Destroy) and recreated (ovr_Create), and new resources need to be created
		///       (ovr_CreateTextureSwapChainXXX). The application's existing private graphics resources do not
		///       need to be recreated unless the new ovr_Create call returns a different GraphicsLuid.
		///     - Result.TextureSwapChainInvalid: The ovrTextureSwapChain is in an incomplete or inconsistent state. 
		///       Ensure ovr_CommitTextureSwapChain was called at least once first.
		/// </returns>
		/// <remarks>
		/// layerPtrList must contain an array of pointers. 
		/// Each pointer must point to an object, which starts with a an LayerHeader property.
		/// </remarks>
		/// <see cref="GetPredictedDisplayTime"/>
		/// <see cref="OVRTypes.ViewScaleDesc"/>
		/// <see cref="LayerHeader"/>
		public override Result SubmitFrame(ovrSession session, Int64 frameIndex, IntPtr viewScaleDesc, IntPtr layerPtrList, uint layerCount)
		{
			return ovr_SubmitFrame(session, frameIndex, viewScaleDesc, layerPtrList, layerCount);
		}

		// Gets the ovrFrameTiming for the given frame index.
		//
		// The application should increment frameIndex for each successively targeted frame,
		// and pass that index to any relevent OVR functions that need to apply to the frame
		// identified by that index.
		//
		// This function is thread-safe and allows for multiple application threads to target
		// their processing to the same displayed frame.

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
		/// 
		/// In the even that prediction fails due to various reasons (e.g. the display being off
		/// or app has yet to present any frames), the return value will be current CPU time.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="frameIndex">
		/// Identifies the frame the caller wishes to target.
		/// A value of zero returns the next frame index.
		/// </param>
		/// <returns>
		/// Returns the absolute frame midpoint time for the given frameIndex.
		/// </returns>
		/// <see cref="GetTimeInSeconds"/>
		public override double GetPredictedDisplayTime(ovrSession session, Int64 frameIndex)
		{
			return ovr_GetPredictedDisplayTime(session, frameIndex);
		}

		/// <summary>
		/// Returns global, absolute high-resolution time in seconds. 
		///
		/// The time frame of reference for this function is not specified and should not be
		/// depended upon.
		/// </summary>
		/// <returns>
		/// Returns seconds as a floating point value.
		/// </returns>
		/// <see cref="OVRTypes.PoseStatef"/>
		public override double GetTimeInSeconds()
		{
			return ovr_GetTimeInSeconds();
		}

		/// <summary>
		/// Reads a boolean property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="defaultVal">
		/// Specifes the value to return if the property couldn't be read.
		/// </param>
		/// <returns>
		/// Returns the property interpreted as a boolean value. 
		/// Returns defaultVal if the property doesn't exist.
		/// </returns>
		public override ovrBool GetBool(ovrSession session, string propertyName, ovrBool defaultVal)
		{
			return ovr_GetBool(session, propertyName, defaultVal);
		}

		/// <summary>
		/// Writes or creates a boolean property.
		/// If the property wasn't previously a boolean property, it is changed to a boolean property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="value">
		/// The value to write.
		/// </param>
		/// <returns>
		/// Returns true if successful, otherwise false. 
		/// A false result should only occur if the property name is empty or if the property is read-only.
		/// </returns>
		public override ovrBool SetBool(ovrSession session, string propertyName, ovrBool value)
		{
			return ovr_SetBool(session, propertyName, value);
		}

		/// <summary>
		/// Reads an integer property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="defaultVal">
		/// Specifes the value to return if the property couldn't be read.
		/// </param>
		/// <returns>
		/// Returns the property interpreted as an integer value. 
		/// Returns defaultVal if the property doesn't exist.
		/// </returns>
		public override int GetInt(ovrSession session, string propertyName, int defaultVal)
		{
			return ovr_GetInt(session, propertyName, defaultVal);
		}

		/// <summary>
		/// Writes or creates an integer property.
		/// 
		/// If the property wasn't previously an integer property, it is changed to an integer property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="value">
		/// The value to write.
		/// </param>
		/// <returns>
		/// Returns true if successful, otherwise false. 
		/// A false result should only occur if the property name is empty or if the property is read-only.
		/// </returns>
		public override ovrBool SetInt(ovrSession session, string propertyName, int value)
		{
			return ovr_SetInt(session, propertyName, value);
		}

		/// <summary>
		/// Reads a float property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="defaultVal">
		/// Specifes the value to return if the property couldn't be read.
		/// </param>
		/// <returns>
		/// Returns the property interpreted as an float value. 
		/// Returns defaultVal if the property doesn't exist.
		/// </returns>
		public override float GetFloat(ovrSession session, string propertyName, float defaultVal)
		{
			return ovr_GetFloat(session, propertyName, defaultVal);
		}

		/// <summary>
		/// Writes or creates a float property.
		/// 
		/// If the property wasn't previously a float property, it's changed to a float property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="value">
		/// The value to write.
		/// </param>
		/// <returns>
		/// Returns true if successful, otherwise false. 
		/// A false result should only occur if the property name is empty or if the property is read-only.
		/// </returns>
		public override ovrBool SetFloat(ovrSession session, string propertyName, float value)
		{
			return ovr_SetFloat(session, propertyName, value);
		}

		/// <summary>
		/// Reads a float array property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="values">
		/// An array of float to write to.
		/// </param>
		/// <param name="valuesCapacity">
		/// Specifies the maximum number of elements to write to the values array.
		/// </param>
		/// <returns>
		/// Returns the number of elements read, or 0 if property doesn't exist or is empty.
		/// </returns>
		public override uint GetFloatArray(ovrSession session, string propertyName, float[] values, uint valuesCapacity)
		{
			return ovr_GetFloatArray(session, propertyName, values, valuesCapacity);
		}

		/// <summary>
		/// Writes or creates a float array property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="values">
		/// An array of float to write from.
		/// </param>
		/// <param name="valuesSize">
		/// Specifies the number of elements to write.
		/// </param>
		/// <returns>
		/// Returns true if successful, otherwise false. 
		/// A false result should only occur if the property name is empty or if the property is read-only.
		/// </returns>
		public override ovrBool SetFloatArray(ovrSession session, string propertyName, float[] values, uint valuesSize)
		{
			return ovr_SetFloatArray(session, propertyName, values, valuesSize);
		}

		/// <summary>
		/// Reads a string property.
		/// Strings are UTF8-encoded and null-terminated.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="defaultVal">
		/// Specifes the value to return if the property couldn't be read.
		/// </param>
		/// <returns>
		/// Returns the string property if it exists. 
		/// 
		/// Otherwise returns defaultVal, which can be specified as null.
		/// The return memory is guaranteed to be valid until next call to ovr_GetString or 
		/// until the HMD is destroyed, whichever occurs first.
		/// </returns>
		public override IntPtr GetString(ovrSession session, string propertyName, string defaultVal)
		{
			return ovr_GetString(session, propertyName, defaultVal);
		}

		/// <summary>
		/// Writes or creates a string property.
		/// Strings are UTF8-encoded and null-terminated.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="value">
		/// The string property, which only needs to be valid for the duration of the call.
		/// </param>
		/// <returns>
		/// Returns true if successful, otherwise false. 
		/// 
		/// A false result should only occur if the property name is empty or if the property is read-only.
		/// </returns>
		public override ovrBool SetString(IntPtr session, string propertyName,string value)
		{
			return ovr_SetString(session, propertyName, value);
		}

		/// <summary>
		/// Used to generate projection from ovrEyeDesc::Fov.
		/// </summary>
		/// <param name="fov">
		/// Specifies the ovrFovPort to use.
		/// </param>
		/// <param name="znear">
		/// Distance to near Z limit.
		/// </param>
		/// <param name="zfar">
		/// Distance to far Z limit.
		/// </param>
		/// <param name="projectionModFlags">
		/// A combination of the ProjectionModifier flags.
		/// </param>
		/// <returns>
		/// Returns the calculated projection matrix.
		/// </returns>
		/// <see cref="OVRTypes.ProjectionModifier"/>
		public override Matrix4f Matrix4f_Projection(FovPort fov, float znear, float zfar, ProjectionModifier projectionModFlags)
		{
			return ovrMatrix4f_Projection(fov, znear, zfar, projectionModFlags);
		}

		/// <summary>
		/// Extracts the required data from the result of ovrMatrix4f_Projection.
		/// </summary>
		/// <param name="projection">Specifies the project matrix from which to extract ovrTimewarpProjectionDesc.</param>
		/// <param name="projectionModFlags">A combination of the ProjectionModifier flags.</param>
		/// <returns>Returns the extracted ovrTimewarpProjectionDesc.</returns>
		/// <see cref="OVRTypes.TimewarpProjectionDesc"/>
		public override TimewarpProjectionDesc TimewarpProjectionDesc_FromProjection(Matrix4f projection, ProjectionModifier projectionModFlags)
		{
			return ovrTimewarpProjectionDesc_FromProjection(projection, projectionModFlags);
		}

		/// <summary>
		/// Generates an orthographic sub-projection.
		///
		/// Used for 2D rendering, Y is down.
		/// </summary>
		/// <param name="projection">
		/// The perspective matrix that the orthographic matrix is derived from.
		/// </param>
		/// <param name="orthoScale">
		/// Equal to 1.0f / pixelsPerTanAngleAtCenter.
		/// </param>
		/// <param name="orthoDistance">
		/// Equal to the distance from the camera in meters, such as 0.8m.
		/// </param>
		/// <param name="hmdToEyeOffsetX">
		/// Specifies the offset of the eye from the center.
		/// </param>
		/// <returns>
		/// Returns the calculated projection matrix.
		/// </returns>
		public override Matrix4f Matrix4f_OrthoSubProjection(Matrix4f projection, Vector2f orthoScale, float orthoDistance, float hmdToEyeOffsetX)
		{
			return ovrMatrix4f_OrthoSubProjection(projection, orthoScale, orthoDistance, hmdToEyeOffsetX);
		}

		/// <summary>
		/// Computes offset eye poses based on headPose returned by TrackingState.
		/// </summary>
		/// <param name="headPose">
		/// Indicates the HMD position and orientation to use for the calculation.
		/// </param>
		/// <param name="hmdToEyeOffset">
		/// Can EyeRenderDesc.HmdToEyeOffset returned from 
		/// ovr_GetRenderDesc. For monoscopic rendering, use a vector that is the average 
		/// of the two vectors for both eyes.
		/// </param>
		/// <param name="outEyePoses">
		/// If outEyePoses are used for rendering, they should be passed to 
		/// ovr_SubmitFrame in LayerEyeFov.RenderPose or LayerEyeFovDepth.RenderPose.
		/// </param>
		public override void CalcEyePoses(Posef headPose, Vector3f[] hmdToEyeOffset, IntPtr outEyePoses)
		{
			ovr_CalcEyePoses(headPose, hmdToEyeOffset, outEyePoses);
		}

		/// <summary>
		/// Returns the predicted head pose in HmdTrackingState and offset eye poses in outEyePoses. 
		/// 
		/// This is a thread-safe function where caller should increment frameIndex with every frame
		/// and pass that index where applicable to functions called on the rendering thread.
		/// Assuming outEyePoses are used for rendering, it should be passed as a part of ovrLayerEyeFov.
		/// The caller does not need to worry about applying HmdToEyeOffset to the returned outEyePoses variables.
		/// </summary>
		/// <param name="session">ovrSession previously returned by ovr_Create.</param>
		/// <param name="frameIndex">
		/// Specifies the targeted frame index, or 0 to refer to one frame after 
		/// the last time ovr_SubmitFrame was called.
		/// </param>
		/// <param name="latencyMarker">
		/// Specifies that this call is the point in time where
		/// the "App-to-Mid-Photon" latency timer starts from. If a given ovrLayer
		/// provides "SensorSampleTimestamp", that will override the value stored here.
		/// </param>
		/// <param name="hmdToEyeOffset">
		/// Can be EyeRenderDesc.HmdToEyeOffset returned from ovr_GetRenderDesc. 
		/// For monoscopic rendering, use a vector that is the average of the two vectors for both eyes.
		/// </param>
		/// <param name="outEyePoses">
		/// The predicted eye poses.
		/// </param>
		/// <param name="outSensorSampleTime">
		/// The time when this function was called. 
		/// May be null, in which case it is ignored.
		/// </param>
		public override void GetEyePoses(ovrSession session, Int64 frameIndex, ovrBool latencyMarker, Vector3f[] hmdToEyeOffset, IntPtr outEyePoses, IntPtr outSensorSampleTime)
		{
			ovr_GetEyePoses(session, frameIndex, latencyMarker, hmdToEyeOffset, outEyePoses, outSensorSampleTime);
		}

		/// <summary>
		/// Tracking poses provided by the SDK come in a right-handed coordinate system. If an application
		/// is passing in ovrProjection_LeftHanded into ovrMatrix4f_Projection, then it should also use
		/// this function to flip the HMD tracking poses to be left-handed.
		///
		/// While this utility function is intended to convert a left-handed ovrPosef into a right-handed
		/// coordinate system, it will also work for converting right-handed to left-handed since the
		/// flip operation is the same for both cases.
		/// </summary>
		/// <param name="inPose">inPose that is right-handed</param>
		/// <param name="outPose">outPose that is requested to be left-handed (can be the same pointer to inPose)</param>
		public override void Posef_FlipHandedness(ref Posef inPose, ref Posef outPose)
		{
			ovrPosef_FlipHandedness(ref inPose, ref outPose);
		}

		/// <summary>
		/// Create Texture Swap Chain suitable for use with Direct3D 11 and 12.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="d3dPtr">
		/// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue 
		/// which must be the same one the application renders to the eye textures with.</param>
		/// <param name="desc">Specifies requested texture properties. See notes for more info about texture format.</param>
		/// <param name="out_TextureSwapChain">
		/// Returns the created ovrTextureSwapChain, which will be valid upon a successful return value, else it will be null.
		/// This texture chain must be eventually destroyed via ovr_DestroyTextureSwapChain before destroying the HMD with ovr_Destroy.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
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
		/// <see cref="GetTextureSwapChainLength"/>
		/// <see cref="GetTextureSwapChainCurrentIndex"/>
		/// <see cref="GetTextureSwapChainDesc"/>
		/// <see cref="GetTextureSwapChainBufferDX"/>
		/// <see cref="DestroyTextureSwapChain"/>
		public override Result CreateTextureSwapChainDX(ovrSession session, IntPtr d3dPtr, ref TextureSwapChainDesc desc, ref ovrTextureSwapChain out_TextureSwapChain)
		{
			return ovr_CreateTextureSwapChainDX(session, d3dPtr, ref desc, ref out_TextureSwapChain);
		}

		/// <summary>
		/// Get a specific buffer within the chain as any compatible COM interface (similar to QueryInterface)
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies an ovrTextureSwapChain previously returned by ovr_CreateTextureSwapChainDX</param>
		/// <param name="index">
		/// Specifies the index within the chain to retrieve. Must be between 0 and length (see ovr_GetTextureSwapChainLength),
		/// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex).
		/// </param>
		/// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
		/// <param name="out_Buffer">Returns the COM interface pointer retrieved.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		public override Result GetTextureSwapChainBufferDX(ovrSession session, ovrTextureSwapChain chain, int index, Guid iid, ref IntPtr out_Buffer)
		{
			return ovr_GetTextureSwapChainBufferDX(session, chain, index, iid, ref out_Buffer);
		}

		/// <summary>
		/// Create Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
		///
		/// A second call to ovr_CreateMirrorTextureDX for a given ovrSession before destroying the first one
		/// is not supported and will result in an error return.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="d3dPtr">
		/// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue
		/// which must be the same one the application renders to the textures with.
		/// </param>
		/// <param name="desc">Specifies requested texture properties. See notes for more info about texture format.</param>
		/// <param name="out_MirrorTexture">
		/// Returns the created ovrMirrorTexture, which will be valid upon a successful return value, else it will be null.
		/// This texture must be eventually destroyed via ovr_DestroyMirrorTexture before destroying the HMD with ovr_Destroy.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		/// <remarks>
		/// The texture format provided in desc should be thought of as the format the compositor will use for the RenderTargetView when
		/// writing into mirror texture. To that end, it is highly recommended that the application requests a mirror texture format that is
		/// in sRGB-space (e.g. OVR.TextureFormat.R8G8B8A8_UNORM_SRGB) as the compositor does sRGB-correct rendering. If however the application wants
		/// to still read the mirror texture as a linear format (e.g. OVR.TextureFormat.OVR_FORMAT_R8G8B8A8_UNORM) and handle the sRGB-to-linear conversion in
		/// HLSL code, then it is recommended the application still requests an sRGB format and also use the ovrTextureMisc_DX_Typeless flag in the
		/// ovrMirrorTextureDesc's Flags field. This will allow the application to bind a ShaderResourceView that is a linear format while the
		/// compositor continues to treat is as sRGB. Failure to do so will cause the compositor to apply unexpected gamma conversions leading to 
		/// gamma-curve artifacts.
		/// </remarks>
		/// <see cref="GetMirrorTextureBufferDX"/>
		/// <see cref="DestroyMirrorTexture"/>
		public override Result CreateMirrorTextureDX(ovrSession session, IntPtr d3dPtr, ref MirrorTextureDesc desc, ref ovrMirrorTexture out_MirrorTexture)
		{
			return ovr_CreateMirrorTextureDX(session, d3dPtr, ref desc, ref out_MirrorTexture);
		}

		/// <summary>
		/// Get a the underlying buffer as any compatible COM interface (similar to QueryInterface) 
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="mirrorTexture">Specifies an ovrMirrorTexture previously returned by ovr_CreateMirrorTextureDX</param>
		/// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
		/// <param name="out_Buffer">Returns the COM interface pointer retrieved.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		public override Result GetMirrorTextureBufferDX(ovrSession session, ovrMirrorTexture mirrorTexture, Guid iid, ref IntPtr out_Buffer)
		{
			return ovr_GetMirrorTextureBufferDX(session, mirrorTexture, iid, ref out_Buffer);
		}

		/// <summary>
		/// Creates a TextureSwapChain suitable for use with OpenGL.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="desc">Specifies the requested texture properties. See notes for more info about texture format.</param>
		/// <param name="out_TextureSwapChain">
		/// Returns the created ovrTextureSwapChain, which will be valid upon
		/// a successful return value, else it will be null. This texture swap chain must be eventually
		/// destroyed via ovr_DestroyTextureSwapChain before destroying the HMD with ovr_Destroy.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		/// <remarks>
		/// The format provided should be thought of as the format the distortion compositor will use when reading
		/// the contents of the texture. To that end, it is highly recommended that the application requests texture swap chain
		/// formats that are in sRGB-space (e.g. Format.R8G8B8A8_UNORM_SRGB) as the distortion compositor does sRGB-correct
		/// rendering. Furthermore, the app should then make sure "glEnable(GL_FRAMEBUFFER_SRGB)" is called before rendering
		/// into these textures. Even though it is not recommended, if the application would like to treat the texture as a linear
		/// format and do linear-to-gamma conversion in GLSL, then the application can avoid calling "glEnable(GL_FRAMEBUFFER_SRGB)",
		/// but should still pass in an sRGB variant for the format. Failure to do so will cause the distortion compositor
		/// to apply incorrect gamma conversions leading to gamma-curve artifacts.		
		/// </remarks>
		/// <see cref="GetTextureSwapChainLength"/>
		/// <see cref="GetTextureSwapChainCurrentIndex"/>
		/// <see cref="GetTextureSwapChainDesc"/>
		/// <see cref="GetTextureSwapChainBufferGL"/>
		/// <see cref="DestroyTextureSwapChain"/>
		public override Result CreateTextureSwapChainGL(ovrSession session, TextureSwapChainDesc desc, out ovrTextureSwapChain out_TextureSwapChain)
		{
			return ovr_CreateTextureSwapChainGL(session, desc, out out_TextureSwapChain);
		}

		/// <summary>
		/// Get a specific buffer within the chain as a GL texture name
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies an ovrTextureSwapChain previously returned by ovr_CreateTextureSwapChainGL</param>
		/// <param name="index">
		/// Specifies the index within the chain to retrieve. Must be between 0 and length (see ovr_GetTextureSwapChainLength)
		/// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex)
		/// </param>
		/// <param name="out_TexId">Returns the GL texture object name associated with the specific index requested</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		public override Result GetTextureSwapChainBufferGL(ovrSession session, ovrTextureSwapChain chain, int index, out uint out_TexId)
		{
			return ovr_GetTextureSwapChainBufferGL(session, chain, index, out out_TexId);
		}

		/// <summary>
		/// Creates a Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
		///
		/// A second call to ovr_CreateMirrorTextureGL for a given ovrSession before destroying the first one
		/// is not supported and will result in an error return.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="desc">Specifies the requested mirror texture description.</param>
		/// <param name="out_MirrorTexture">
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
		/// <see cref="GetMirrorTextureBufferGL"/>
		/// <see cref="DestroyMirrorTexture"/>
		public override Result CreateMirrorTextureGL(ovrSession session, MirrorTextureDesc desc, out ovrMirrorTexture out_MirrorTexture)
		{
			return ovr_CreateMirrorTextureGL(session, desc, out out_MirrorTexture);
		}

		/// <summary>
		/// Get a the underlying buffer as a GL texture name
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="mirrorTexture">Specifies an ovrMirrorTexture previously returned by ovr_CreateMirrorTextureGL</param>
		/// <param name="out_TexId">Specifies the GL texture object name associated with the mirror texture</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		public override Result GetMirrorTextureBufferGL(ovrSession session, ovrMirrorTexture mirrorTexture, out uint out_TexId)
		{
			return ovr_GetMirrorTextureBufferGL(session, mirrorTexture, out out_TexId);
		}
		#endregion

		#region Oculus SDK Platform invoke methods
		/// <summary>
		/// Detects Oculus Runtime and Device Status
		///
		/// Checks for Oculus Runtime and Oculus HMD device status without loading the LibOVRRT
		/// shared library.  This may be called before ovr_Initialize() to help decide whether or
		/// not to initialize LibOVR.
		/// </summary>
		/// <param name="timeoutMilliseconds">Specifies a timeout to wait for HMD to be attached or 0 to poll.</param>
		/// <returns>Returns a DetectResult object indicating the result of detection.</returns>
		/// <see cref="OVRTypes.DetectResult"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_Detect", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern DetectResult ovr_Detect(int timeoutMilliseconds);

		/// <summary>
		/// Initializes all Oculus functionality.
		/// </summary>
		/// <param name="parameters">
		/// Initialize with extra parameters.
		/// Pass 0 to initialize with default parameters, suitable for released games.
		/// </param>
		/// <remarks>
		/// Library init/shutdown, must be called around all other OVR code.
		/// No other functions calls besides ovr_InitializeRenderingShim are allowed
		/// before ovr_Initialize succeeds or after ovr_Shutdown.
		/// 
		/// LibOVRRT shared library search order:
		///      1) Current working directory (often the same as the application directory).
		///      2) Module directory (usually the same as the application directory, but not if the module is a separate shared library).
		///      3) Application directory
		///      4) Development directory (only if OVR_ENABLE_DEVELOPER_SEARCH is enabled, which is off by default).
		///      5) Standard OS shared library search location(s) (OS-specific).
		/// </remarks>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_Initialize", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_Initialize(InitParams parameters=null);

		/// <summary>
		/// Returns information about the most recent failed return value by the
		/// current thread for this library.
		/// 
		/// This function itself can never generate an error.
		/// The last error is never cleared by LibOVR, but will be overwritten by new errors.
		/// Do not use this call to determine if there was an error in the last API 
		/// call as successful API calls don't clear the last ErrorInfo.
		/// To avoid any inconsistency, ovr_GetLastErrorInfo should be called immediately
		/// after an API function that returned a failed ovrResult, with no other API
		/// functions called in the interim.
		/// </summary>
		/// <param name="errorInfo">The last ErrorInfo for the current thread.</param>
		/// <remarks>
		/// Allocate an ErrorInfo and pass this as errorInfo argument.
		/// </remarks>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetLastErrorInfo", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovr_GetLastErrorInfo(out ErrorInfo errorInfo);

		/// <summary>
		/// Returns version string representing libOVR version. Static, so
		/// string remains valid for app lifespan
		/// </summary>
		/// <remarks>
		/// Use Marshal.PtrToStringAnsi() to retrieve version string.
		/// </remarks>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetVersionString", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern IntPtr ovr_GetVersionString();

		/// <summary>
		/// Send a message string to the system tracing mechanism if enabled (currently Event Tracing for Windows)
		/// </summary>
		/// <param name="level">
		/// One of the ovrLogLevel constants.
		/// </param>
		/// <param name="message">
		/// A UTF8-encoded null-terminated string.
		/// </param>
		/// <returns>
		/// Returns the length of the message, or -1 if message is too large
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_TraceMessage", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern int ovr_TraceMessage(LogLevel level, string message);

		/// <summary>
		/// Shuts down all Oculus functionality.
		/// </summary>
		/// <remarks>
		/// No API functions may be called after ovr_Shutdown except ovr_Initialize.
		/// </remarks>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_Shutdown", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovr_Shutdown();

		/// <summary>
		/// Returns information about the current HMD.
		/// 
		/// ovr_Initialize must have first been called in order for this to succeed, otherwise HmdDesc.Type
		/// will be reported as None.
		/// 
		/// Please note: This method will should only be called by a 32 bit process. 
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create, else NULL in which
		/// case this function detects whether an HMD is present and returns its info if so.
		/// </param>
		/// <param name="result">
		/// Returns an ovrHmdDesc. If the hmd is null and ovrHmdDesc::Type is ovr_None then
		/// no HMD is present.
		/// </param>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetHmdDesc", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovr_GetHmdDesc32(out HmdDesc result, ovrSession session);

		/// <summary>
		/// Returns information about the current HMD.
		/// 
		/// ovr_Initialize must have first been called in order for this to succeed, otherwise HmdDesc.Type
		/// will be reported as None.
		/// 
		/// Please note: This method will should only be called by a 64 bit process. 
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create, else NULL in which
		/// case this function detects whether an HMD is present and returns its info if so.
		/// </param>
		/// <param name="result">
		/// Returns an ovrHmdDesc. If the hmd is null and ovrHmdDesc::Type is ovr_None then
		/// no HMD is present.
		/// </param>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetHmdDesc", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovr_GetHmdDesc64(out HmdDesc64 result, ovrSession session);

		/// <summary>
		/// Returns the number of sensors. 
		///
		/// The number of sensors may change at any time, so this function should be called before use 
		/// as opposed to once on startup.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>Returns unsigned int count.</returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTrackerCount", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern uint ovr_GetTrackerCount(ovrSession session);

		/// <summary>
		/// Returns a given sensor description.
		///
		/// It's possible that sensor desc [0] may indicate a unconnnected or non-pose tracked sensor, but 
		/// sensor desc [1] may be connected.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="trackerDescIndex">
		/// Specifies a sensor index. The valid indexes are in the range of 0 to the sensor count returned by ovr_GetTrackerCount.
		/// </param>
		/// <returns>An empty ovrTrackerDesc will be returned if trackerDescIndex is out of range.</returns>
		/// <see cref="OVRTypes.TrackerDesc"/>
		/// <see cref="GetTrackerCount"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTrackerDesc", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern TrackerDesc ovr_GetTrackerDesc(ovrSession session, uint trackerDescIndex);

		/// <summary>
		/// Creates a handle to a VR session.
		/// 
		/// Upon success the returned ovrSession must be eventually freed with ovr_Destroy when it is no longer needed.
		/// A second call to ovr_Create will result in an error return value if the previous Hmd has not been destroyed.
		/// </summary>
		/// <param name="session">
		/// Provides a pointer to an ovrSession which will be written to upon success.
		/// </param>
		/// <param name="pLuid">
		/// Provides a system specific graphics adapter identifier that locates which
		/// graphics adapter has the HMD attached. This must match the adapter used by the application
		/// or no rendering output will be possible. This is important for stability on multi-adapter systems. An
		/// application that simply chooses the default adapter will not run reliably on multi-adapter systems.
		/// </param>
		/// <remarks>
		/// Call Marshal.PtrToStructure(...) to convert the IntPtr to the OVR.ovrHmd type.
		/// </remarks>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. Upon failure
		/// the returned pHmd will be null.
		/// </returns>
		/// <example>
		/// <code>
		/// ovrSession session;
		/// ovrGraphicsLuid luid;
		/// ovrResult result = ovr_Create(ref session, ref luid);
		/// if(OVR_FAILURE(result))
		/// ...
		/// </code>
		/// </example>
		/// <see cref="Destroy"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_Create", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_Create(ref ovrSession session, ref GraphicsLuid pLuid);

		/// <summary>
		/// Destroys the HMD.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_Destroy", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovr_Destroy(ovrSession session);

		/// <summary>
		/// Returns status information for the application.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="sessionStatus">Provides a SessionStatus that is filled in.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use ovr_GetLastErrorInfo 
		/// to get more information.
		/// Return values include but aren't limited to:
		/// - Result.Success: Completed successfully.
		/// - Result.ServiceConnection: The service connection was lost and the application must destroy the session.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetSessionStatus", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_GetSessionStatus(ovrSession session, ref SessionStatus sessionStatus);

		/// <summary>
		/// Sets the tracking origin type
		///
		/// When the tracking origin is changed, all of the calls that either provide
		/// or accept ovrPosef will use the new tracking origin provided.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="origin">Specifies an ovrTrackingOrigin to be used for all ovrPosef</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. 
		/// In the case of failure, use ovr_GetLastErrorInfo to get more information.
		/// </returns>
		/// <see cref="OVRTypes.TrackingOrigin"/>
		/// <see cref="GetTrackingOriginType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SetTrackingOriginType", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_SetTrackingOriginType(ovrSession session, TrackingOrigin origin);

		/// <summary>
		/// Gets the tracking origin state
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>Returns the TrackingOrigin that was either set by default, or previous set by the application.</returns>
		/// <see cref="OVRTypes.TrackingOrigin"/>
		/// <see cref="SetTrackingOriginType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTrackingOriginType", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern TrackingOrigin ovr_GetTrackingOriginType(ovrSession session);

		/// <summary>
		/// Re-centers the sensor position and orientation.
		///
		/// This resets the (x,y,z) positional components and the yaw orientation component.
		/// The Roll and pitch orientation components are always determined by gravity and cannot
		/// be redefined. All future tracking will report values relative to this new reference position.
		/// If you are using ovrTrackerPoses then you will need to call ovr_GetTrackerPose after 
		/// this, because the sensor position(s) will change as a result of this.
		/// 
		/// The headset cannot be facing vertically upward or downward but rather must be roughly
		/// level otherwise this function will fail with ovrError_InvalidHeadsetOrientation.
		///
		/// For more info, see the notes on each ovrTrackingOrigin enumeration to understand how
		/// recenter will vary slightly in its behavior based on the current ovrTrackingOrigin setting.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use
		/// ovr_GetLastErrorInfo to get more information. Return values include but aren't limited to:
		/// - Result.Success: Completed successfully.
		/// - Result.InvalidHeadsetOrientation: The headset was facing an invalid direction when attempting recentering, 
		///   such as facing vertically.
		/// </returns>
		/// <see cref="OVRTypes.TrackingOrigin"/>
		/// <see cref="GetTrackerPose"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_RecenterTrackingOrigin", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_RecenterTrackingOrigin(ovrSession session);
		
		/// <summary>
		/// Clears the ShouldRecenter status bit in ovrSessionStatus.
		///
		/// Clears the ShouldRecenter status bit in ovrSessionStatus, allowing further recenter 
		/// requests to be detected. Since this is automatically done by ovr_RecenterTrackingOrigin,
		/// this is only needs to be called when application is doing its own re-centering.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_ClearShouldRecenterFlag", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovr_ClearShouldRecenterFlag(ovrSession session);

		/// <summary>
		/// Returns the ovrTrackerPose for the given sensor.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="trackerPoseIndex">Index of the sensor being requested.</param>
		/// <returns>
		/// Returns the requested ovrTrackerPose. An empty ovrTrackerPose will be returned if trackerPoseIndex is out of range.
		/// </returns>
		/// <see cref="GetTrackerCount"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTrackerPose", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern TrackerPose ovr_GetTrackerPose(ovrSession session, uint trackerPoseIndex);

		/// <summary>
		/// Returns the most recent input state for controllers, without positional tracking info.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="controllerType">Specifies which controller the input will be returned for.</param>
		/// <param name="inputState">Input state that will be filled in.</param>
		/// <returns>Returns Result.Success if the new state was successfully obtained.</returns>
		/// <see cref="OVRTypes.ControllerType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetInputState", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_GetInputState(ovrSession session, ControllerType controllerType, ref InputState inputState);
		
		/// <summary>
		/// Returns controller types connected to the system OR'ed together.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>A bitmask of ControllerTypes connected to the system.</returns>
		/// <see cref="OVRTypes.ControllerType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetConnectedControllerTypes", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern ControllerType ovr_GetConnectedControllerTypes(ovrSession session);

		/// <summary>
		/// Returns tracking state reading based on the specified absolute system time.
		///
		/// Pass an absTime value of 0.0 to request the most recent sensor reading. In this case
		/// both PredictedPose and SamplePose will have the same value.
		///
		/// This may also be used for more refined timing of front buffer rendering logic, and so on.
		/// This may be called by multiple threads.
		/// </summary>
		/// <param name="result">Returns the TrackingState that is predicted for the given absTime.</param>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="absTime">
		/// Specifies the absolute future time to predict the return
		/// TrackingState value. Use 0 to request the most recent tracking state.
		/// </param>
		/// <param name="latencyMarker">
		/// Specifies that this call is the point in time where
		/// the "App-to-Mid-Photon" latency timer starts from. If a given ovrLayer
		/// provides "SensorSampleTimestamp", that will override the value stored here.
		/// </param>
		/// <see cref="OVRTypes.TrackingState"/>
		/// <see cref="GetEyePoses"/>
		/// <see cref="GetTimeInSeconds"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTrackingState", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovr_GetTrackingState(out TrackingState result, ovrSession session, double absTime, ovrBool latencyMarker);

		/// <summary>
		/// Turns on vibration of the given controller.
		///
		/// To disable vibration, call ovr_SetControllerVibration with an amplitude of 0.
		/// Vibration automatically stops after a nominal amount of time, so if you want vibration 
		/// to be continuous over multiple seconds then you need to call this function periodically.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="controllerType">Specifies controllers to apply the vibration to.</param>
		/// <param name="frequency">
		/// Specifies a vibration frequency in the range of 0.0 to 1.0. 
		/// Currently the only valid values are 0.0, 0.5, and 1.0 and other values will
		/// be clamped to one of these.
		/// </param>
		/// <param name="amplitude">Specifies a vibration amplitude in the range of 0.0 to 1.0.</param>
		/// <returns>Returns ovrSuccess upon success.</returns>
		/// <see cref="OVRTypes.ControllerType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SetControllerVibration", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_SetControllerVibration(ovrSession session, ControllerType controllerType, float frequency, float amplitude);

		// SDK Distortion Rendering
		//
		// All of rendering functions including the configure and frame functions
		// are not thread safe. It is OK to use ConfigureRendering on one thread and handle
		// frames on another thread, but explicit synchronization must be done since
		// functions that depend on configured state are not reentrant.
		//
		// These functions support rendering of distortion by the SDK.

		// ovrTextureSwapChain creation is rendering API-specific.
		// ovr_CreateTextureSwapChainDX and ovr_CreateTextureSwapChainGL can be found in the
		// rendering API-specific headers, such as OVR_CAPI_D3D.h and OVR_CAPI_GL.h


		/// <summary>
		/// Gets the number of buffers in an ovrTextureSwapChain.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain for which the length should be retrieved.</param>
		/// <param name="out_Length">Returns the number of buffers in the specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTextureSwapChainLength", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_GetTextureSwapChainLength(ovrSession session, ovrTextureSwapChain chain, out int out_Length);

		/// <summary>
		/// Gets the current index in an ovrTextureSwapChain.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain for which the index should be retrieved.</param>
		/// <param name="out_Index">Returns the current (free) index in specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTextureSwapChainCurrentIndex", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_GetTextureSwapChainCurrentIndex(ovrSession session, ovrTextureSwapChain chain, out int out_Index);

		/// <summary>
		/// Gets the description of the buffers in an ovrTextureSwapChain
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain for which the description should be retrieved.</param>
		/// <param name="out_Desc">Returns the description of the specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTextureSwapChainDesc", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_GetTextureSwapChainDesc(ovrSession session, ovrTextureSwapChain chain, [In, Out] ref TextureSwapChainDesc out_Desc);

		/// <summary>
		/// Commits any pending changes to an ovrTextureSwapChain, and advances its current index
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain to commit.</param>
		/// <returns>
		/// Returns an ovrResult for which the return code is negative upon error.
		/// Failures include but aren't limited to:
		///   - Result.TextureSwapChainFull: ovr_CommitTextureSwapChain was called too many times on a texture swapchain without calling submit to use the chain.
		/// </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_CommitTextureSwapChain", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_CommitTextureSwapChain(ovrSession session, ovrTextureSwapChain chain);

		/// <summary>
		/// Destroys an ovrTextureSwapChain and frees all the resources associated with it.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain to destroy. If it is null then this function has no effect.</param>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_DestroyTextureSwapChain", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovr_DestroyTextureSwapChain(ovrSession session, ovrTextureSwapChain chain);

		// MirrorTexture creation is rendering API-specific.

		/// <summary>
		/// Destroys a mirror texture previously created by one of the mirror texture creation functions.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="mirrorTexture">
		/// Specifies the ovrTexture to destroy. If it is null then this function has no effect.
		/// </param>
		/// <see cref="CreateMirrorTextureDX"/>
		/// <see cref="CreateMirrorTextureGL"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_DestroyMirrorTexture", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovr_DestroyMirrorTexture(ovrSession session, ovrMirrorTexture mirrorTexture);

		/// <summary>
		/// Calculates the recommended viewport size for rendering a given eye within the HMD
		/// with a given FOV cone. 
		/// 
		/// Higher FOV will generally require larger textures to maintain quality.
		/// Apps packing multiple eye views together on the same texture should ensure there are
		/// at least 8 pixels of padding between them to prevent texture filtering and chromatic
		/// aberration causing images to leak between the two eye views.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="eye">
		/// Specifies which eye (left or right) to calculate for.
		/// </param>
		/// <param name="fov">
		/// Specifies the ovrFovPort to use.
		/// </param>
		/// <param name="pixelsPerDisplayPixel">
		/// pixelsPerDisplayPixel Specifies the ratio of the number of render target pixels 
		/// to display pixels at the center of distortion. 1.0 is the default value. Lower
		/// values can improve performance, higher values give improved quality.
		/// </param>
		/// <returns>
		/// Returns the texture width and height size.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetFovTextureSize", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Sizei ovr_GetFovTextureSize(ovrSession session, EyeType eye, FovPort fov, float pixelsPerDisplayPixel);

		/// <summary>
		/// Computes the distortion viewport, view adjust, and other rendering parameters for
		/// the specified eye.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="eyeType">
		/// Specifies which eye (left or right) for which to perform calculations.
		/// </param>
		/// <param name="fov">
		/// Specifies the FovPort to use.
		/// </param>
		/// <returns>
		/// Returns the computed EyeRenderDesc for the given eyeType and field of view.
		/// </returns>
		/// <see cref="OVRTypes.EyeRenderDesc"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetRenderDesc", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern EyeRenderDesc ovr_GetRenderDesc(ovrSession session, EyeType eyeType, FovPort fov);

		/// <summary>
		/// Submits layers for distortion and display.
		/// 
		/// ovr_SubmitFrame triggers distortion and processing which might happen asynchronously. 
		/// The function will return when there is room in the submission queue and surfaces
		/// are available. Distortion might or might not have completed.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="frameIndex">
		/// Specifies the targeted application frame index, or 0 to refer to one frame 
		/// after the last time ovr_SubmitFrame was called.
		/// </param>
		/// <param name="viewScaleDesc">
		/// Provides additional information needed only if layerPtrList contains
		/// an ovrLayerType_Quad. If null, a default version is used based on the current configuration and a 1.0 world scale.
		/// </param>
		/// <param name="layerPtrList">
		/// Specifies a list of ovrLayer pointers, which can include null entries to
		/// indicate that any previously shown layer at that index is to not be displayed.
		/// Each layer header must be a part of a layer structure such as ovrLayerEyeFov or ovrLayerQuad,
		/// with Header.Type identifying its type. A null layerPtrList entry in the array indicates the 
		/// absence of the given layer.
		/// </param>
		/// <param name="layerCount">
		/// Indicates the number of valid elements in layerPtrList. The maximum supported layerCount 
		/// is not currently specified, but may be specified in a future version.
		/// </param>
		/// <returns>
		/// Returns an ovrResult for which the return code is negative upon error and positive
		/// upon success. Return values include but aren't limited to:
		///     - Result.Success: rendering completed successfully.
		///     - Result.NotVisible: rendering completed successfully but was not displayed on the HMD,
		///       usually because another application currently has ownership of the HMD. Applications receiving
		///       this result should stop rendering new content, but continue to call ovr_SubmitFrame periodically
		///       until it returns a value other than ovrSuccess_NotVisible.
		///     - Result.DisplayLost: The session has become invalid (such as due to a device removal)
		///       and the shared resources need to be released (ovr_DestroyTextureSwapChain), the session needs to
		///       destroyed (ovr_Destroy) and recreated (ovr_Create), and new resources need to be created
		///       (ovr_CreateTextureSwapChainXXX). The application's existing private graphics resources do not
		///       need to be recreated unless the new ovr_Create call returns a different GraphicsLuid.
		///     - Result.TextureSwapChainInvalid: The ovrTextureSwapChain is in an incomplete or inconsistent state. 
		///       Ensure ovr_CommitTextureSwapChain was called at least once first.
		/// </returns>
		/// <remarks>
		/// layerPtrList must contain an array of pointers. 
		/// Each pointer must point to an object, which starts with a an LayerHeader property.
		/// </remarks>
		/// <see cref="GetPredictedDisplayTime"/>
		/// <see cref="OVRTypes.ViewScaleDesc"/>
		/// <see cref="LayerHeader"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SubmitFrame", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_SubmitFrame(ovrSession session, Int64 frameIndex, IntPtr viewScaleDesc, IntPtr layerPtrList, uint layerCount);

		// Gets the ovrFrameTiming for the given frame index.
		//
		// The application should increment frameIndex for each successively targeted frame,
		// and pass that index to any relevent OVR functions that need to apply to the frame
		// identified by that index.
		//
		// This function is thread-safe and allows for multiple application threads to target
		// their processing to the same displayed frame.

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
		/// 
		/// In the even that prediction fails due to various reasons (e.g. the display being off
		/// or app has yet to present any frames), the return value will be current CPU time.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="frameIndex">
		/// Identifies the frame the caller wishes to target.
		/// A value of zero returns the next frame index.
		/// </param>
		/// <returns>
		/// Returns the absolute frame midpoint time for the given frameIndex.
		/// </returns>
		/// <see cref="GetTimeInSeconds"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetPredictedDisplayTime", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern double ovr_GetPredictedDisplayTime(ovrSession session, Int64 frameIndex);

		/// <summary>
		/// Returns global, absolute high-resolution time in seconds. 
		///
		/// The time frame of reference for this function is not specified and should not be
		/// depended upon.
		/// </summary>
		/// <returns>
		/// Returns seconds as a floating point value.
		/// </returns>
		/// <see cref="OVRTypes.PoseStatef"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTimeInSeconds", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern double ovr_GetTimeInSeconds();

		/// <summary>
		/// Reads a boolean property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="defaultVal">
		/// Specifes the value to return if the property couldn't be read.
		/// </param>
		/// <returns>
		/// Returns the property interpreted as a boolean value. 
		/// Returns defaultVal if the property doesn't exist.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetBool", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern ovrBool ovr_GetBool(ovrSession session, string propertyName, ovrBool defaultVal);

		/// <summary>
		/// Writes or creates a boolean property.
		/// If the property wasn't previously a boolean property, it is changed to a boolean property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="value">
		/// The value to write.
		/// </param>
		/// <returns>
		/// Returns true if successful, otherwise false. 
		/// A false result should only occur if the property name is empty or if the property is read-only.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SetBool", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern ovrBool ovr_SetBool(ovrSession session, string propertyName, ovrBool value);

		/// <summary>
		/// Reads an integer property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="defaultVal">
		/// Specifes the value to return if the property couldn't be read.
		/// </param>
		/// <returns>
		/// Returns the property interpreted as an integer value. 
		/// Returns defaultVal if the property doesn't exist.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetInt", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern int ovr_GetInt(ovrSession session, string propertyName, int defaultVal);

		/// <summary>
		/// Writes or creates an integer property.
		/// 
		/// If the property wasn't previously an integer property, it is changed to an integer property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="value">
		/// The value to write.
		/// </param>
		/// <returns>
		/// Returns true if successful, otherwise false. 
		/// A false result should only occur if the property name is empty or if the property is read-only.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SetInt", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern ovrBool ovr_SetInt(ovrSession session, string propertyName, int value);

		/// <summary>
		/// Reads a float property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="defaultVal">
		/// Specifes the value to return if the property couldn't be read.
		/// </param>
		/// <returns>
		/// Returns the property interpreted as an float value. 
		/// Returns defaultVal if the property doesn't exist.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetFloat", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern float ovr_GetFloat(ovrSession session, string propertyName, float defaultVal);

		/// <summary>
		/// Writes or creates a float property.
		/// 
		/// If the property wasn't previously a float property, it's changed to a float property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="value">
		/// The value to write.
		/// </param>
		/// <returns>
		/// Returns true if successful, otherwise false. 
		/// A false result should only occur if the property name is empty or if the property is read-only.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SetFloat", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern ovrBool ovr_SetFloat(ovrSession session, string propertyName, float value);

		/// <summary>
		/// Reads a float array property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="values">
		/// An array of float to write to.
		/// </param>
		/// <param name="valuesCapacity">
		/// Specifies the maximum number of elements to write to the values array.
		/// </param>
		/// <returns>
		/// Returns the number of elements read, or 0 if property doesn't exist or is empty.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetFloatArray", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern uint ovr_GetFloatArray(ovrSession session, string propertyName, [MarshalAs(UnmanagedType.LPArray)] float[] values, uint valuesCapacity);

		/// <summary>
		/// Writes or creates a float array property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="values">
		/// An array of float to write from.
		/// </param>
		/// <param name="valuesSize">
		/// Specifies the number of elements to write.
		/// </param>
		/// <returns>
		/// Returns true if successful, otherwise false. 
		/// A false result should only occur if the property name is empty or if the property is read-only.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SetFloatArray", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern ovrBool ovr_SetFloatArray(ovrSession session, string propertyName, [MarshalAs(UnmanagedType.LPArray)] float[] values, uint valuesSize);

		/// <summary>
		/// Reads a string property.
		/// Strings are UTF8-encoded and null-terminated.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="defaultVal">
		/// Specifes the value to return if the property couldn't be read.
		/// </param>
		/// <returns>
		/// Returns the string property if it exists. 
		/// 
		/// Otherwise returns defaultVal, which can be specified as null.
		/// The return memory is guaranteed to be valid until next call to ovr_GetString or 
		/// until the HMD is destroyed, whichever occurs first.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetString", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern IntPtr ovr_GetString(ovrSession session, string propertyName, string defaultVal);

		/// <summary>
		/// Writes or creates a string property.
		/// Strings are UTF8-encoded and null-terminated.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by ovr_Create.
		/// </param>
		/// <param name="propertyName">
		/// The name of the property, which needs to be valid for only the call.
		/// </param>
		/// <param name="value">
		/// The string property, which only needs to be valid for the duration of the call.
		/// </param>
		/// <returns>
		/// Returns true if successful, otherwise false. 
		/// 
		/// A false result should only occur if the property name is empty or if the property is read-only.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SetString", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern ovrBool ovr_SetString(IntPtr session, string propertyName,string value);

		/// <summary>
		/// Used to generate projection from ovrEyeDesc::Fov.
		/// </summary>
		/// <param name="fov">
		/// Specifies the ovrFovPort to use.
		/// </param>
		/// <param name="znear">
		/// Distance to near Z limit.
		/// </param>
		/// <param name="zfar">
		/// Distance to far Z limit.
		/// </param>
		/// <param name="projectionModFlags">
		/// A combination of the ProjectionModifier flags.
		/// </param>
		/// <returns>
		/// Returns the calculated projection matrix.
		/// </returns>
		/// <see cref="OVRTypes.ProjectionModifier"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovrMatrix4f_Projection", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Matrix4f ovrMatrix4f_Projection(FovPort fov, float znear, float zfar, ProjectionModifier projectionModFlags);

		/// <summary>
		/// Extracts the required data from the result of ovrMatrix4f_Projection.
		/// </summary>
		/// <param name="projection">Specifies the project matrix from which to extract ovrTimewarpProjectionDesc.</param>
		/// <param name="projectionModFlags">A combination of the ProjectionModifier flags.</param>
		/// <returns>Returns the extracted ovrTimewarpProjectionDesc.</returns>
		/// <see cref="OVRTypes.TimewarpProjectionDesc"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovrTimewarpProjectionDesc_FromProjection", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern TimewarpProjectionDesc ovrTimewarpProjectionDesc_FromProjection(Matrix4f projection, ProjectionModifier projectionModFlags);

		/// <summary>
		/// Generates an orthographic sub-projection.
		///
		/// Used for 2D rendering, Y is down.
		/// </summary>
		/// <param name="projection">
		/// The perspective matrix that the orthographic matrix is derived from.
		/// </param>
		/// <param name="orthoScale">
		/// Equal to 1.0f / pixelsPerTanAngleAtCenter.
		/// </param>
		/// <param name="orthoDistance">
		/// Equal to the distance from the camera in meters, such as 0.8m.
		/// </param>
		/// <param name="hmdToEyeOffsetX">
		/// Specifies the offset of the eye from the center.
		/// </param>
		/// <returns>
		/// Returns the calculated projection matrix.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovrMatrix4f_OrthoSubProjection", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Matrix4f ovrMatrix4f_OrthoSubProjection(Matrix4f projection, Vector2f orthoScale, float orthoDistance, float hmdToEyeOffsetX);

		/// <summary>
		/// Computes offset eye poses based on headPose returned by TrackingState.
		/// </summary>
		/// <param name="headPose">
		/// Indicates the HMD position and orientation to use for the calculation.
		/// </param>
		/// <param name="hmdToEyeOffset">
		/// Can EyeRenderDesc.HmdToEyeOffset returned from 
		/// ovr_GetRenderDesc. For monoscopic rendering, use a vector that is the average 
		/// of the two vectors for both eyes.
		/// </param>
		/// <param name="outEyePoses">
		/// If outEyePoses are used for rendering, they should be passed to 
		/// ovr_SubmitFrame in LayerEyeFov.RenderPose or LayerEyeFovDepth.RenderPose.
		/// </param>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_CalcEyePoses", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovr_CalcEyePoses(Posef headPose, [MarshalAs(UnmanagedType.LPArray, SizeConst=2)] Vector3f[] hmdToEyeOffset, IntPtr outEyePoses);

		/// <summary>
		/// Returns the predicted head pose in HmdTrackingState and offset eye poses in outEyePoses. 
		/// 
		/// This is a thread-safe function where caller should increment frameIndex with every frame
		/// and pass that index where applicable to functions called on the rendering thread.
		/// Assuming outEyePoses are used for rendering, it should be passed as a part of ovrLayerEyeFov.
		/// The caller does not need to worry about applying HmdToEyeOffset to the returned outEyePoses variables.
		/// </summary>
		/// <param name="session">ovrSession previously returned by ovr_Create.</param>
		/// <param name="frameIndex">
		/// Specifies the targeted frame index, or 0 to refer to one frame after 
		/// the last time ovr_SubmitFrame was called.
		/// </param>
		/// <param name="latencyMarker">
		/// Specifies that this call is the point in time where
		/// the "App-to-Mid-Photon" latency timer starts from. If a given ovrLayer
		/// provides "SensorSampleTimestamp", that will override the value stored here.
		/// </param>
		/// <param name="hmdToEyeOffset">
		/// Can be EyeRenderDesc.HmdToEyeOffset returned from ovr_GetRenderDesc. 
		/// For monoscopic rendering, use a vector that is the average of the two vectors for both eyes.
		/// </param>
		/// <param name="outEyePoses">
		/// The predicted eye poses.
		/// </param>
		/// <param name="outSensorSampleTime">
		/// The time when this function was called. 
		/// May be null, in which case it is ignored.
		/// </param>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetEyePoses", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovr_GetEyePoses(ovrSession session, Int64 frameIndex, ovrBool latencyMarker, [MarshalAs(UnmanagedType.LPArray, SizeConst=2)] Vector3f[] hmdToEyeOffset, IntPtr outEyePoses, IntPtr outSensorSampleTime);

		/// <summary>
		/// Tracking poses provided by the SDK come in a right-handed coordinate system. If an application
		/// is passing in ovrProjection_LeftHanded into ovrMatrix4f_Projection, then it should also use
		/// this function to flip the HMD tracking poses to be left-handed.
		///
		/// While this utility function is intended to convert a left-handed ovrPosef into a right-handed
		/// coordinate system, it will also work for converting right-handed to left-handed since the
		/// flip operation is the same for both cases.
		/// </summary>
		/// <param name="inPose">inPose that is right-handed</param>
		/// <param name="outPose">outPose that is requested to be left-handed (can be the same pointer to inPose)</param>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovrPosef_FlipHandedness", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern void ovrPosef_FlipHandedness(ref Posef inPose, [In, Out] ref Posef outPose);

		/// <summary>
		/// Create Texture Swap Chain suitable for use with Direct3D 11 and 12.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="d3dPtr">
		/// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue 
		/// which must be the same one the application renders to the eye textures with.</param>
		/// <param name="desc">Specifies requested texture properties. See notes for more info about texture format.</param>
		/// <param name="out_TextureSwapChain">
		/// Returns the created ovrTextureSwapChain, which will be valid upon a successful return value, else it will be null.
		/// This texture chain must be eventually destroyed via ovr_DestroyTextureSwapChain before destroying the HMD with ovr_Destroy.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
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
		/// <see cref="GetTextureSwapChainLength"/>
		/// <see cref="GetTextureSwapChainCurrentIndex"/>
		/// <see cref="GetTextureSwapChainDesc"/>
		/// <see cref="GetTextureSwapChainBufferDX"/>
		/// <see cref="DestroyTextureSwapChain"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_CreateTextureSwapChainDX", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_CreateTextureSwapChainDX(ovrSession session, IntPtr d3dPtr, ref TextureSwapChainDesc desc, [In, Out] ref ovrTextureSwapChain out_TextureSwapChain);

		/// <summary>
		/// Get a specific buffer within the chain as any compatible COM interface (similar to QueryInterface)
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies an ovrTextureSwapChain previously returned by ovr_CreateTextureSwapChainDX</param>
		/// <param name="index">
		/// Specifies the index within the chain to retrieve. Must be between 0 and length (see ovr_GetTextureSwapChainLength),
		/// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex).
		/// </param>
		/// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
		/// <param name="out_Buffer">Returns the COM interface pointer retrieved.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTextureSwapChainBufferDX", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_GetTextureSwapChainBufferDX(ovrSession session, ovrTextureSwapChain chain, int index, Guid iid, [In, Out] ref IntPtr out_Buffer);

		/// <summary>
		/// Create Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
		///
		/// A second call to ovr_CreateMirrorTextureDX for a given ovrSession before destroying the first one
		/// is not supported and will result in an error return.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="d3dPtr">
		/// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue
		/// which must be the same one the application renders to the textures with.
		/// </param>
		/// <param name="desc">Specifies requested texture properties. See notes for more info about texture format.</param>
		/// <param name="out_MirrorTexture">
		/// Returns the created ovrMirrorTexture, which will be valid upon a successful return value, else it will be null.
		/// This texture must be eventually destroyed via ovr_DestroyMirrorTexture before destroying the HMD with ovr_Destroy.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		/// <remarks>
		/// The texture format provided in desc should be thought of as the format the compositor will use for the RenderTargetView when
		/// writing into mirror texture. To that end, it is highly recommended that the application requests a mirror texture format that is
		/// in sRGB-space (e.g. OVR.TextureFormat.R8G8B8A8_UNORM_SRGB) as the compositor does sRGB-correct rendering. If however the application wants
		/// to still read the mirror texture as a linear format (e.g. OVR.TextureFormat.OVR_FORMAT_R8G8B8A8_UNORM) and handle the sRGB-to-linear conversion in
		/// HLSL code, then it is recommended the application still requests an sRGB format and also use the ovrTextureMisc_DX_Typeless flag in the
		/// ovrMirrorTextureDesc's Flags field. This will allow the application to bind a ShaderResourceView that is a linear format while the
		/// compositor continues to treat is as sRGB. Failure to do so will cause the compositor to apply unexpected gamma conversions leading to 
		/// gamma-curve artifacts.
		/// </remarks>
		/// <see cref="GetMirrorTextureBufferDX"/>
		/// <see cref="DestroyMirrorTexture"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_CreateMirrorTextureDX", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_CreateMirrorTextureDX(ovrSession session, IntPtr d3dPtr, ref MirrorTextureDesc desc, [In, Out] ref ovrMirrorTexture out_MirrorTexture);

		/// <summary>
		/// Get a the underlying buffer as any compatible COM interface (similar to QueryInterface) 
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="mirrorTexture">Specifies an ovrMirrorTexture previously returned by ovr_CreateMirrorTextureDX</param>
		/// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
		/// <param name="out_Buffer">Returns the COM interface pointer retrieved.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetMirrorTextureBufferDX", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_GetMirrorTextureBufferDX(ovrSession session, ovrMirrorTexture mirrorTexture, Guid iid, [In, Out] ref IntPtr out_Buffer);

		/// <summary>
		/// Creates a TextureSwapChain suitable for use with OpenGL.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="desc">Specifies the requested texture properties. See notes for more info about texture format.</param>
		/// <param name="out_TextureSwapChain">
		/// Returns the created ovrTextureSwapChain, which will be valid upon
		/// a successful return value, else it will be null. This texture swap chain must be eventually
		/// destroyed via ovr_DestroyTextureSwapChain before destroying the HMD with ovr_Destroy.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
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
		/// <see cref="GetTextureSwapChainLength"/>
		/// <see cref="GetTextureSwapChainCurrentIndex"/>
		/// <see cref="GetTextureSwapChainDesc"/>
		/// <see cref="GetTextureSwapChainBufferGL"/>
		/// <see cref="DestroyTextureSwapChain"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_CreateTextureSwapChainGL", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result  ovr_CreateTextureSwapChainGL(ovrSession session, TextureSwapChainDesc desc, [Out] out ovrTextureSwapChain out_TextureSwapChain);

		/// <summary>
		/// Get a specific buffer within the chain as a GL texture name
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies an ovrTextureSwapChain previously returned by ovr_CreateTextureSwapChainGL</param>
		/// <param name="index">
		/// Specifies the index within the chain to retrieve. Must be between 0 and length (see ovr_GetTextureSwapChainLength)
		/// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex)
		/// </param>
		/// <param name="out_TexId">Returns the GL texture object name associated with the specific index requested</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTextureSwapChainBufferGL", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_GetTextureSwapChainBufferGL(ovrSession session, ovrTextureSwapChain chain, int index, [Out] out uint out_TexId);

		/// <summary>
		/// Creates a Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
		///
		/// A second call to ovr_CreateMirrorTextureGL for a given ovrSession before destroying the first one
		/// is not supported and will result in an error return.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="desc">Specifies the requested mirror texture description.</param>
		/// <param name="out_MirrorTexture">
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
		/// <see cref="GetMirrorTextureBufferGL"/>
		/// <see cref="DestroyMirrorTexture"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_CreateMirrorTextureGL", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_CreateMirrorTextureGL(ovrSession session, MirrorTextureDesc desc, [Out] out ovrMirrorTexture out_MirrorTexture);

		/// <summary>
		/// Get a the underlying buffer as a GL texture name
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="mirrorTexture">Specifies an ovrMirrorTexture previously returned by ovr_CreateMirrorTextureGL</param>
		/// <param name="out_TexId">Specifies the GL texture object name associated with the mirror texture</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// ovr_GetLastErrorInfo to get more information.
		/// </returns>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetMirrorTextureBufferGL", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_GetMirrorTextureBufferGL(ovrSession session, ovrMirrorTexture mirrorTexture, [Out] out uint out_TexId);
		#endregion

		#pragma warning restore 1591

		#region Private properties
		/// <summary>
		/// Pointer to the Oculus runtime module, after it has been loaded, using a call to LoadLibrary.
		/// </summary>
		private IntPtr OculusRuntimeDll
		{
			get;
			set;
		}
		#endregion
	}
}
