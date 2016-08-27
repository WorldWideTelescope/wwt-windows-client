using System;
using System.Runtime.InteropServices;
using ovrBool = System.Byte;
using ovrTextureSwapChain = System.IntPtr;

namespace OculusWrap
{
	/// <summary>
	/// Base class containing types needed to interact with the unmanaged Oculus SDK.
	/// </summary>
	public abstract class OVRTypes
	{
		// The Oculus SDK is full of missing comments.
		// Ignore warnings regarding missing comments, in this class.
		#pragma warning disable 1591

		#region Constants and structures

		#region Definitions found in OVR_CAPI_Keys_h
		public const string OVR_KEY_USER                        = "User";              // string
		public const string OVR_KEY_NAME                        = "Name";              // string
		public const string OVR_KEY_GENDER                      = "Gender";            // string "Male", "Female", or "Unknown"
		public const string OVR_DEFAULT_GENDER                  = "Unknown";
		public const string OVR_KEY_PLAYER_HEIGHT               = "PlayerHeight";      // float meters
		public const float	OVR_DEFAULT_PLAYER_HEIGHT           = 1.778f;
		public const string OVR_KEY_EYE_HEIGHT                  = "EyeHeight";         // float meters
		public const float	OVR_DEFAULT_EYE_HEIGHT              = 1.675f;
		public const string OVR_KEY_NECK_TO_EYE_DISTANCE        = "NeckEyeDistance";   // float[2] meters
		public const float	OVR_DEFAULT_NECK_TO_EYE_HORIZONTAL  = 0.0805f;
		public const float	OVR_DEFAULT_NECK_TO_EYE_VERTICAL    = 0.075f;
		public const string OVR_KEY_EYE_TO_NOSE_DISTANCE        = "EyeToNoseDist";     // float[2] meters

		public const	string	OVR_PERF_HUD_MODE                       = "PerfHudMode";                       // int, allowed values are defined in enum ovrPerfHudMode
		public const	string	OVR_LAYER_HUD_MODE                      = "LayerHudMode";                      // int, allowed values are defined in enum ovrLayerHudMode
		public const	string	OVR_LAYER_HUD_CURRENT_LAYER             = "LayerHudCurrentLayer";              // int, The layer to show 
		public const	string	OVR_LAYER_HUD_SHOW_ALL_LAYERS           = "LayerHudShowAll";                   // bool, Hide other layers when the hud is enabled
		public const	string	OVR_DEBUG_HUD_STEREO_MODE               = "DebugHudStereoMode";                // allowed values are defined in enum DebugHudStereoMode
		public const	string	OVR_DEBUG_HUD_STEREO_GUIDE_INFO_ENABLE  = "DebugHudStereoGuideInfoEnable";     // bool
		public const	string	OVR_DEBUG_HUD_STEREO_GUIDE_SIZE         = "DebugHudStereoGuideSize2f";         // float[2]
		public const	string	OVR_DEBUG_HUD_STEREO_GUIDE_POSITION     = "DebugHudStereoGuidePosition3f";     // float[3]
		public const	string	OVR_DEBUG_HUD_STEREO_GUIDE_YAWPITCHROLL = "DebugHudStereoGuideYawPitchRoll3f"; // float[3]
		public const	string	OVR_DEBUG_HUD_STEREO_GUIDE_COLOR        = "DebugHudStereoGuideColor4f";        // float[4]
		#endregion

		#region Definitions found in OVR_VERSION_h
		
		/// <summary>
		/// Product version doesn't participate in semantic versioning.
		/// </summary>
		public const int OVR_PRODUCT_VERSION = 1;

		/// <summary>
		/// If you change these values then you need to also make sure to change LibOVR/Projects/Windows/LibOVR.props in parallel.
		/// </summary>
		public const int OVR_MAJOR_VERSION   = 1;
		public const int OVR_MINOR_VERSION   = 3;
		public const int OVR_PATCH_VERSION   = 2;
		public const int OVR_BUILD_NUMBER    = 0;

		/// <summary>
		/// This is the ((product * 100) + major) version of the service that the DLL is compatible with.
		/// When we backport changes to old versions of the DLL we update the old DLLs
		/// to move this version number up to the latest version.
		/// The DLL is responsible for checking that the service is the version it supports
		/// and returning an appropriate error message if it has not been made compatible.
		/// </summary>
		public const int OVR_DLL_COMPATIBLE_MAJOR_VERSION = 101;

		public const int OVR_FEATURE_VERSION = 0;

		public static readonly string OVR_VERSION_STRING = OVR_MAJOR_VERSION+"."+OVR_MINOR_VERSION+"."+OVR_PATCH_VERSION;

	    public static readonly string OVR_DETAILED_VERSION_STRING = OVR_MAJOR_VERSION+"."+OVR_MINOR_VERSION+"."+OVR_PATCH_VERSION+"."+OVR_BUILD_NUMBER;
		#endregion

		#region Definitions found in OVR_CAPI_Audio_h
		public const int OVR_AUDIO_MAX_DEVICE_STR_SIZE = 128;
		#endregion

		/// <summary>
		/// Specifies the maximum number of layers supported by ovr_SubmitFrame.
		/// </summary>
		/// <see cref="OVRBase.SubmitFrame"/>
		public const int MaxLayerCount = 16;

		/// <summary>
		/// Parameters for the ovr_Initialize call.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack=8)]
		public class InitParams
		{
			/// <summary>
			/// Flags from ovrInitFlags to override default behavior.
			/// Pass 0 for the defaults.
			/// </summary>
			/// <remarks>
			/// Combination of ovrInitFlags or 0
			/// </remarks>
			public InitFlags Flags;

			/// <summary>
			/// Request a specific minimum minor version of the LibOVR runtime.
			/// Flags must include ovrInit_RequestVersion or this will be ignored.
			/// </summary>
			public uint RequestedMinorVersion;

			/// <summary>
			/// Log callback function, which may be called at any time asynchronously from
			/// multiple threads until ovr_Shutdown() completes.
			/// 
			/// Pass null for no log callback.
			/// </summary>
			/// <remarks>
			/// Function pointer or 0
			/// </remarks>
			[MarshalAs(UnmanagedType.FunctionPtr)]
			public LogCallback LogCallback;

			/// <summary>
			/// User-supplied data which is passed as-is to LogCallback. Typically this 
			/// is used to store an application-specific pointer which is read in the 
			/// callback function.
			/// </summary>
			public IntPtr UserData;

			/// <summary>
			/// Number of milliseconds to wait for a connection to the server.
			/// 
			/// Pass 0 for the default timeout.
			/// </summary>
			/// <remarks>
			/// Timeout in Milliseconds or 0
			/// </remarks>
			public uint ConnectionTimeoutMS;
		}

		/// <summary>
		/// Provides information about the last error.
		/// </summary>
		/// <remarks>
		/// See ovr_GetLastErrorInfo.
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct ErrorInfo
		{
			/// <summary>
			/// The result from the last API call that generated an error ovrResult.
			/// </summary>
			public Result Result;
			
			/// <summary>
			/// A UTF8-encoded null-terminated English string describing the problem. 
			/// The format of this string is subject to change in future versions.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=512)]
			public string ErrorString;
		};

		/// <summary>
		/// Signature for the logging callback.
		/// </summary>
		/// <param name="userData">UserData is an arbitrary value specified by the user of ovrInitParams.</param>
		/// <param name="level">Level is one of the ovrLogLevel constants.</param>
		/// <param name="message">Message is a UTF8-encoded null-terminated string.</param>
		/// <see cref="InitParams"/>
		/// <seealso cref="LogLevel"/>
		/// <seealso cref="OVRBase.Initialize"/>
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void LogCallback(IntPtr userData, LogLevel level, string message);

		/// <summary>
		/// Flags for Initialize()
		/// </summary>
		public enum InitFlags
		{
			/// <summary>
			/// No flags specified.
			/// </summary>
			None			= 0x00000000,

			/// <summary>
			/// When a debug library is requested, a slower debugging version of the library will
			/// be run which can be used to help solve problems in the library and debug game code.
			/// </summary>
			Debug			= 0x00000001,

			/// <summary>
			/// When a version is requested, LibOVR runtime will respect the RequestedMinorVersion
			/// field and will verify that the RequestedMinorVersion is supported.
			/// </summary>
			RequestVersion	= 0x00000004,

			/// <summary>
			/// These bits are writable by user code.
			/// </summary>
			WritableBits	= 0x00ffffff
		}

		/// <summary>
		/// Logging levels
		/// </summary>
		/// <see cref="InitParams"/>
		/// <see cref="LogCallback"/>
		public enum LogLevel
		{
			/// <summary>
			/// Debug-level log event.
			/// </summary>
			Debug    = 0,

			/// <summary>
			/// Info-level log event.
			/// </summary>
			Info     = 1,

			/// <summary>
			/// Error-level log event.
			/// </summary>
			Error    = 2
		}

		/// <summary>
		/// Result codes, returned by calls to Oculus SDK.
		/// </summary>
		/// <remarks>
		/// Return codes with a value of 0 or greater are consider successful, 
		/// while return codes with values less than 0 are considered failures.
		/// </remarks>
		public enum Result
		{
			#region Successful results.
			/// <summary>
			/// This is a general success result. 
			/// </summary>
			Success = 0,
    
			/// <summary>
			/// Returned from a call to SubmitFrame. The call succeeded, but what the app
			/// rendered will not be visible on the HMD. Ideally the app should continue
			/// calling SubmitFrame, but not do any rendering. When the result becomes
			/// ovrSuccess, rendering should continue as usual.
			/// </summary>
			NotVisible = 1000,

			/// <summary>
			/// The HMD Firmware is out of date but is acceptable.
			/// </summary>
			HMDFirmwareMismatchSuccess     = 4100,

			/// <summary>
			/// The Tracker Firmware is out of date but is acceptable.
			/// </summary>
			TrackerFirmwareMismatchSuccess = 4101,

			/// <summary>
			/// The controller firmware is out of date but is acceptable.
			/// </summary>
			ControllerFirmwareMismatchSuccess = 4104,

			/// <summary>
			/// The tracker driver interface was not found. Can be a temporary error
			/// </summary>
			TrackerDriverNotFound      = 4105,
			#endregion

			#region General errors
			/// <summary>
			/// Failure to allocate memory.
			/// </summary>
			MemoryAllocationFailure = -1000,   

			/// <summary>
			/// Failure to create a socket.
			/// </summary>
			SocketCreationFailure   = -1001,   

			/// <summary>
			/// Invalid ovrSession parameter provided.
			/// </summary>
			InvalidSession          = -1002,   

			/// <summary>
			/// The operation timed out.
			/// </summary>
			Timeout                 = -1003,   

			/// <summary>
			/// The system or component has not been initialized.
			/// </summary>
			NotInitialized          = -1004,   

			/// <summary>
			/// Invalid parameter provided. See error info or log for details.
			/// </summary>
			InvalidParameter        = -1005,   

			/// <summary>
			/// Generic service error. See error info or log for details.
			/// </summary>
			ServiceError            = -1006,   

			/// <summary>
			/// The given HMD doesn't exist.
			/// </summary>
			NoHmd                   = -1007,   

			/// <summary>
			/// Function call is not supported on this hardware/software
			/// </summary>
			Unsupported                = -1009,

			/// <summary>
			/// Specified device type isn't available.
			/// </summary>
			DeviceUnavailable          = -1010,   

			/// <summary>
			/// The headset was in an invalid orientation for the requested operation (e.g. vertically oriented during ovr_RecenterPose).
			/// </summary>
			InvalidHeadsetOrientation  = -1011,

			/// <summary>
			/// The client failed to call ovr_Destroy on an active session before calling ovr_Shutdown. Or the client crashed.
			/// </summary>
			ClientSkippedDestroy       = -1012,

			/// <summary>
			/// The client failed to call ovr_Shutdown or the client crashed.
			/// </summary>
			ClientSkippedShutdown      = -1013,
			#endregion

			#region Audio error range, reserved for Audio errors.
			/// <summary>
			/// First Audio error.
			/// </summary>
			AudioReservedBegin		= -2000,   

			/// <summary>
			/// Failure to find the specified audio device.
			/// </summary>
			AudioDeviceNotFound		= -2001,

			/// <summary>
			/// Generic COM error.
			/// </summary>
			AudioComError			= -2002,

			/// <summary>
			/// Last Audio error.
			/// </summary>
			AudioReservedEnd		= -2999,   
			#endregion

			#region Initialization errors.
			/// <summary>
			/// Generic initialization error.
			/// </summary>
			Initialize              = -3000,   

			/// <summary>
			/// Couldn't load LibOVRRT.
			/// </summary>
			LibLoad                 = -3001,   

			/// <summary>
			/// LibOVRRT version incompatibility.
			/// </summary>
			LibVersion              = -3002,   

			/// <summary>
			/// Couldn't connect to the OVR Service.
			/// </summary>
			ServiceConnection       = -3003,   

			/// <summary>
			/// OVR Service version incompatibility.
			/// </summary>
			ServiceVersion          = -3004,   

			/// <summary>
			/// The operating system version is incompatible.
			/// </summary>
			IncompatibleOS          = -3005,   

			/// <summary>
			/// Unable to initialize the HMD display.
			/// </summary>
			DisplayInit             = -3006,   

			/// <summary>
			/// Unable to start the server. Is it already running?
			/// </summary>
			ServerStart             = -3007,   

			/// <summary>
			/// Attempting to re-initialize with a different version.
			/// </summary>
			Reinitialization        = -3008,   

			/// <summary>
			/// Chosen rendering adapters between client and service do not match
			/// </summary>
			MismatchedAdapters		= -3009,

			/// <summary>
			/// Calling application has leaked resources
			/// </summary>
			LeakingResources           = -3010,

			/// <summary>
			/// Client version too old to connect to service
			/// </summary>
			ClientVersion              = -3011,

			/// <summary>
			/// The operating system is out of date.
			/// </summary>
			OutOfDateOS                = -3012,

			/// <summary>
			/// The graphics driver is out of date.
			/// </summary>
			OutOfDateGfxDriver         = -3013,

			/// <summary>
			/// The graphics hardware is not supported
			/// </summary>
			IncompatibleGPU            = -3014,
 
			/// <summary>
			/// No valid VR display system found.
			/// </summary>
			NoValidVRDisplaySystem     = -3015,

			/// <summary>
			/// Feature or API is obsolete and no longer supported.
			/// </summary>
			Obsolete                   = -3016,

			/// <summary>
			/// No supported VR display system found, but disabled or driverless adapter found.
			/// </summary>
			DisabledOrDefaultAdapter   = -3017,

			/// <summary>
			/// The system is using hybrid graphics (Optimus, etc...), which is not support.
			/// </summary>
			HybridGraphicsNotSupported = -3018,

			/// <summary>
			/// Initialization of the DisplayManager failed.
			/// </summary>
			DisplayManagerInit         = -3019,

			/// <summary>
			/// Failed to get the interface for an attached tracker
			/// </summary>
			TrackerDriverInit          = -3020,
			#endregion

			#region Hardware Errors
			/// <summary>
			/// Headset has no bundle adjustment data.
			/// </summary>
			InvalidBundleAdjustment			= -4000,   

			/// <summary>
			/// The USB hub cannot handle the camera frame bandwidth.
			/// </summary>
			USBBandwidth					= -4001,   

			/// <summary>
			/// The USB camera is not enumerating at the correct device speed.
			/// </summary>
			USBEnumeratedSpeed				= -4002,

			/// <summary>
			/// Unable to communicate with the image sensor.
			/// </summary>
			ImageSensorCommError			= -4003,

			/// <summary>
			/// We use this to report various sensor issues that don't fit in an easily classifiable bucket.
			/// </summary>
			GeneralTrackerFailure			= -4004,

			/// <summary>
			/// A more than acceptable number of frames are coming back truncated.
			/// </summary>
			ExcessiveFrameTruncation		= -4005,

			/// <summary>
			/// A more than acceptable number of frames have been skipped.
			/// </summary>
			ExcessiveFrameSkipping			= -4006,

			/// <summary>
			/// The sensor is not receiving the sync signal (cable disconnected?).
			/// </summary>
			SyncDisconnected				= -4007,

			/// <summary>
			/// Failed to read memory from the sensor.
			/// </summary>
			TrackerMemoryReadFailure   = -4008,

			/// <summary>
			/// Failed to write memory from the sensor.
			/// </summary>
			TrackerMemoryWriteFailure  = -4009,

			/// <summary>
			/// Timed out waiting for a camera frame.
			/// </summary>
			TrackerFrameTimeout        = -4010,

			/// <summary>
			/// Truncated frame returned from sensor.
			/// </summary>
			TrackerTruncatedFrame      = -4011,

			/// <summary>
			/// The sensor driver has encountered a problem.
			/// </summary>
			TrackerDriverFailure       = -4012,

			/// <summary>
			/// The sensor wireless subsystem has encountered a problem.
			/// </summary>
			TrackerNRFFailure          = -4013,

			/// <summary>
			/// The hardware has been unplugged
			/// </summary>
			HardwareGone               = -4014,

			/// <summary>
			/// The nordic indicates that sync is enabled but it is not sending sync pulses
			/// </summary>
			NordicEnabledNoSync        = -4015,

			/// <summary>
			/// It looks like we're getting a sync signal, but no camera frames have been received
			/// </summary>
			NordicSyncNoFrames         = -4016,

			/// <summary>
			/// A catastrophic failure has occurred.  We will attempt to recover by resetting the device
			/// </summary>
			CatastrophicFailure        = -4017,

			/// <summary>
			/// The catastrophic recovery has timed out.
			/// </summary>
			CatastrophicTimeout        = -4018,

			/// <summary>
			/// Catastrophic failure has repeated too many times.
			/// </summary>
			RepeatCatastrophicFail     = -4019,

			/// <summary>
			/// Could not open handle for Rift device (likely already in use by another process).
			/// </summary>
			USBOpenDeviceFailure       = -4020,

			/// <summary>
			/// Unexpected HMD issues that don't fit a specific bucket.
			/// </summary>
			HMDGeneralFailure          = -4021,

			/// <summary>
			/// The HMD Firmware is out of date and is unacceptable.
			/// </summary>
			HMDFirmwareMismatchError	= -4100,

			/// <summary>
			/// The sensor Firmware is out of date and is unacceptable.
			/// </summary>
			TrackerFirmwareMismatch    = -4101,

			/// <summary>
			/// A bootloader HMD is detected by the service.
			/// </summary>
			BootloaderDeviceDetected   = -4102,

			/// <summary>
			/// The sensor calibration is missing or incorrect.
			/// </summary>
			TrackerCalibrationError    = -4103,

			/// <summary>
			/// The controller firmware is out of date and is unacceptable.
			/// </summary>
			ControllerFirmwareMismatch = -4104,

			/// <summary>
			/// Too many lost IMU samples.
			/// </summary>
			IMUTooManyLostSamples      = -4200,

			/// <summary>
			/// IMU rate is outside of the expected range.
			/// </summary>
			IMURateError               = -4201,

			/// <summary>
			/// A feature report has failed.
			/// </summary>
			FeatureReportFailure       = -4202,
	
			#endregion

			#region Synchronization Errors
			/// <summary>
			/// Requested async work not yet complete.
			/// </summary>
			Incomplete               = -5000,

			/// <summary>
			/// Requested async work was abandoned and result is incomplete.
			/// </summary>
			Abandoned                = -5001,
			#endregion

			#region Rendering errors
			/// <summary>
			/// In the event of a system-wide graphics reset or cable unplug this is returned to the app.
			/// </summary>
			DisplayLost                = -6000,

			/// <summary>
			/// ovr_CommitTextureSwapChain was called too many times on a texture swapchain without calling submit to use the chain.
			/// </summary>
			TextureSwapChainFull       = -6001,

			/// <summary>
			/// The ovrTextureSwapChain is in an incomplete or inconsistent state. Ensure ovr_CommitTextureSwapChain was called at least once first.
			/// </summary>
			TextureSwapChainInvalid    = -6002,

			/// <summary>
			/// Graphics device has been reset (TDR, etc...)
			/// </summary>
			GraphicsDeviceReset        = -6003,

			/// <summary>
			/// HMD removed from the display adapter
			/// </summary>
			DisplayRemoved             = -6004,

			/// <summary>
			/// Application declared itself as an invisible type and is not allowed to submit frames.
			/// </summary>
			ApplicationInvisible       = -6005,

			/// <summary>
			/// The given request is disallowed under the current conditions.
			/// </summary>
			Disallowed                 = -6006,
			#endregion

			#region Fatal Errors
			/// <summary>
			/// A runtime exception occurred. 
			/// The application is required to shutdown LibOVR and re-initialize it before this error state will be cleared.
			/// </summary>
			RuntimeException			= -7000,

			MetricsUnknownApp            = -90000,
			MetricsDuplicateApp          = -90001,
			MetricsNoEvents              = -90002,
			MetricsRuntime               = -90003,
			MetricsFile                  = -90004,
			MetricsNoClientInfo          = -90005,
			MetricsNoAppMetaData         = -90006,
			MetricsNoApp                 = -90007,
			MetricsOafFailure            = -90008,
			MetricsSessionAlreadyActive  = -90009,
			MetricsSessionNotActive      = -90010,
			#endregion
		}

		/// <summary>
		/// A 2D vector with integer components.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Vector2i
		{
			public Vector2i(int x, int y)
			{
				this.x	= x;
				this.y	= y;
			}

			public int x, y;
		}

		/// <summary>
		/// A 2D size with integer components.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Sizei
		{
			public Sizei(int width, int height)
			{
				this.Width	= width;
				this.Height	= height;
			}

			public int	Width, 
						Height;
		}

		/// <summary>
		/// A 2D rectangle with a position and size.
		/// All components are integers.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Recti
		{
			public Recti(Vector2i position, Sizei size)
			{
				Position	= position;
				Size		= size;
			}

			public Vector2i Position;
			public Sizei    Size;
		}

		/// <summary>
		/// A quaternion rotation.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Quaternionf
		{
			public Quaternionf(float x, float y, float z, float w)
			{
				this.X	= x;
				this.Y	= y;
				this.Z	= z;
				this.W	= w;
			}

			public float	X, 
							Y, 
							Z, 
							W;  
		}

		/// <summary>
		/// A 2D vector with float components.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Vector2f
		{
			public float	X, 
							Y;
		}

		/// <summary>
		/// A 3D vector with float components.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Vector3f
		{
			public Vector3f(float x, float y, float z)
			{
				this.X	= x;
				this.Y	= y;
				this.Z	= z;
			}

			public float	X, 
							Y, 
							Z;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct Matrix4f
		{
			public float M11;
			public float M12;
			public float M13;
			public float M14;
			public float M21;
			public float M22;
			public float M23;
			public float M24;
			public float M31;
			public float M32;
			public float M33;
			public float M34;
			public float M41;
			public float M42;
			public float M43;
			public float M44;
		}

		/// <summary>
		/// Position and orientation together.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct Posef
		{
			[MarshalAs(UnmanagedType.Struct)]
			public Quaternionf  Orientation;

			[MarshalAs(UnmanagedType.Struct)]
			public Vector3f		Position;    
		}

		/// <summary>
		/// A full pose (rigid body) configuration with first and second derivatives.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct PoseStatef
		{
			/// <summary>
			/// Position and orientation.
			/// </summary>
			public Posef		ThePose;

			/// <summary>
			/// Angular velocity in radians per second.
			/// </summary>
			public Vector3f		AngularVelocity;

			/// <summary>
			/// Velocity in meters per second.
			/// </summary>
			public Vector3f		LinearVelocity;

			/// <summary>
			/// Angular acceleration in radians per second per second.
			/// </summary>
			public Vector3f		AngularAcceleration;

			/// <summary>
			/// Acceleration in meters per second per second.
			/// </summary>
			public Vector3f		LinearAcceleration;

			/// <summary>
			/// Absolute time that this pose refers to.
			/// </summary>
			/// <see cref="OVRBase.GetTimeInSeconds"/>
			public double		TimeInSeconds; 
		}

		/// <summary>
		/// Field Of View (FOV) in tangent of the angle units.
		/// As an example, for a standard 90 degree vertical FOV, we would 
		/// have: { UpTan = tan(90 degrees / 2), DownTan = tan(90 degrees / 2) }.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack=4)]
		public struct FovPort
		{
			/// The tangent of the angle between the viewing vector and the top edge of the field of view.
			public float UpTan;

			/// The tangent of the angle between the viewing vector and the bottom edge of the field of view.
			public float DownTan;

			/// The tangent of the angle between the viewing vector and the left edge of the field of view.
			public float LeftTan;

			/// The tangent of the angle between the viewing vector and the right edge of the field of view.
			public float RightTan;
		}

		/// <summary>
		/// HMD capability bits reported by device.
		/// </summary>
		public enum HmdCaps
		{
			/// <summary>
			/// No flags.
			/// </summary>
			None						= 0x0000,

			// Read only flags

			/// <summary>
			/// Means HMD device is a virtual debug device.
			/// </summary>
			/// <remarks>
			/// (read only) 
			/// </remarks>
		    DebugDevice					= 0x0010,
		}

		/// <summary>
		/// Tracking capability bits reported by the device.
		/// Used with ovr_GetTrackingCaps.
		/// </summary>
		[Flags]
		public enum TrackingCaps
		{
			/// <summary>
			/// No flags.
			/// </summary>
			None				= 0x0000,

			/// <summary>
			/// Supports orientation tracking (IMU).
			/// </summary>
			Orientation			= 0x0010,

			/// <summary>
			/// Supports yaw drift correction via a magnetometer or other means.
			/// </summary>
			MagYawCorrection	= 0x0020,

			/// <summary>
			/// Supports positional tracking.
			/// </summary>
			Position			= 0x0040,
		}

		/// <summary>
		/// Specifies which eye is being used for rendering.
		/// This type explicitly does not include a third "NoStereo" option, as such is
		/// not required for an HMD-centered API.
		/// </summary>
		public enum EyeType
		{
			Left  = 0,
			Right = 1,
			Count = 2
		}

		/// <summary>
		/// Specifies the coordinate system TrackingState returns tracking poses in.
		/// Used with ovr_SetTrackingOriginType()
		/// </summary>
		public enum TrackingOrigin
		{
			/// <summary>
			/// Tracking system origin reported at eye (HMD) height
			/// 
			/// Prefer using this origin when your application requires
			/// matching user's current physical head pose to a virtual head pose
			/// without any regards to a the height of the floor. Cockpit-based,
			/// or 3rd-person experiences are ideal candidates.
			/// 
			/// When used, all poses in TrackingState are reported as an offset
			/// transform from the profile calibrated or recentered HMD pose.
			/// It is recommended that apps using this origin type call ovr_RecenterTrackingOrigin
			/// prior to starting the VR experience, but notify the user before doing so
			/// to make sure the user is in a comfortable pose, facing a comfortable
			/// direction.
			/// </summary>
			EyeLevel = 0,

			/// <summary>
			/// Tracking system origin reported at floor height
			/// 
			/// Prefer using this origin when your application requires the
			/// physical floor height to match the virtual floor height, such as
			/// standing experiences.
			/// 
			/// When used, all poses in TrackingState are reported as an offset
			/// transform from the profile calibrated floor pose. Calling ovr_RecenterTrackingOrigin
			/// will recenter the X &amp; Z axes as well as yaw, but the Y-axis (i.e. height) will continue
			/// to be reported using the floor height as the origin for all poses.
			/// </summary>
			FloorLevel = 1,

			/// <summary>
			/// Count of enumerated elements.
			/// </summary>
			Count = 2,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct GraphicsLuid
		{
			/// <summary>
			/// Public definition reserves space for graphics API-specific implementation 
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
			public byte[] Reserved;
		}

		/// <summary>
		/// Enumerates all HMD types that we support.
		/// </summary>
		public enum HmdType
		{
			None		= 0,    
			DK1			= 3,
			DKHD		= 4,    
			DK2			= 6,
			CB			= 8,
			Other		= 9,
			E3_2015		= 10,
			ES06		= 11,
			ES09		= 12,
			ES11		= 13,
			CV1			= 14,
		}

		/// <summary>
		/// This is a complete descriptor of the HMD.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct HmdDesc
		{
			/// <summary>
			/// Copy constructor used to convert an HmdDesc64 to an HmdDesc.
			/// </summary>
			/// <param name="source">HmdDesc64 to copy from.</param>
			public HmdDesc(HmdDesc64 source)
			{
				Type						= source.Type;
				ProductName					= source.ProductName;
				Manufacturer				= source.Manufacturer;
				VendorId					= source.VendorId;
				ProductId					= source.ProductId;
				SerialNumber				= source.SerialNumber;
				FirmwareMajor				= source.FirmwareMajor;
				FirmwareMinor				= source.FirmwareMinor;
				AvailableHmdCaps			= source.AvailableHmdCaps;
				DefaultHmdCaps				= source.DefaultHmdCaps;
				AvailableTrackingCaps		= source.AvailableTrackingCaps;
				DefaultTrackingCaps			= source.DefaultTrackingCaps;
				DefaultEyeFov				= source.DefaultEyeFov;
				MaxEyeFov					= source.MaxEyeFov;
				Resolution					= source.Resolution;
				DisplayRefreshRate			= source.DisplayRefreshRate;
			}

			/// <summary>
			/// The type of HMD.
			/// </summary>
			public HmdType Type;
    
			/// <summary>
			/// Product identification string (e.g. "Oculus Rift DK1").
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
			public byte[] ProductName;    

			/// <summary>
			/// HMD manufacturer identification string.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
			public byte[] Manufacturer;
    
			/// <summary>
			/// HID (USB) vendor identifier of the device.
			/// </summary>
			public short VendorId;

			/// <summary>
			/// HID (USB) product identifier of the device.
			/// </summary>
			public short ProductId;

			/// <summary>
			/// HMD serial number.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=24)]
			public byte[] SerialNumber;

			/// <summary>
			/// HMD firmware major version.
			/// </summary>
			public short FirmwareMajor;

			/// <summary>
			/// HMD firmware minor version.
			/// </summary>
			public short FirmwareMinor;

			/// <summary>
			/// Capability bits described by HmdCaps which the HMD currently supports.
			/// </summary>
			public HmdCaps AvailableHmdCaps;

			/// <summary>
			/// Capability bits described by HmdCaps which are default for the current Hmd.
			/// </summary>
			public HmdCaps DefaultHmdCaps;

			/// <summary>
			/// Capability bits described by TrackingCaps which the system currently supports.
			/// </summary>
			public TrackingCaps AvailableTrackingCaps;

			/// <summary>
			/// Capability bits described by ovrTrackingCaps which are default for the current system.
			/// </summary>
			public TrackingCaps DefaultTrackingCaps;

			/// <summary>
			/// Defines the recommended FOVs for the HMD.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public FovPort[] DefaultEyeFov;

			/// <summary>
			/// Defines the maximum FOVs for the HMD.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public FovPort[] MaxEyeFov;

			/// <summary>
			/// Resolution of the full HMD screen (both eyes) in pixels.
			/// </summary>
			public Sizei Resolution;

			/// <summary>
			/// Nominal refresh rate of the display in cycles per second at the time of HMD creation.
			/// </summary>
			public float DisplayRefreshRate;
		}

		/// <summary>
		/// 64 bit version of the HmdDesc.
		/// </summary>
		/// <remarks>
		/// This class is needed because the Oculus SDK defines padding fields on the 64 bit version of the Oculus SDK.
		/// </remarks>
		/// <see cref="HmdDesc"/>
		[StructLayout(LayoutKind.Sequential)]
		public struct HmdDesc64
		{
			/// <summary>
			/// The type of HMD.
			/// </summary>
			public HmdType Type;
    
			/// <summary>
			/// Internal struct paddding.
			/// </summary>
			private int Pad0;

			/// <summary>
			/// Product identification string (e.g. "Oculus Rift DK1").
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
			public byte[] ProductName;    

			/// <summary>
			/// HMD manufacturer identification string.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
			public byte[] Manufacturer;
    
			/// <summary>
			/// HID (USB) vendor identifier of the device.
			/// </summary>
			public short VendorId;

			/// <summary>
			/// HID (USB) product identifier of the device.
			/// </summary>
			public short ProductId;

			/// <summary>
			/// Sensor (and display) serial number.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=24)]
			public byte[] SerialNumber;

			/// <summary>
			/// Sensor firmware major version.
			/// </summary>
			public short FirmwareMajor;

			/// <summary>
			/// Sensor firmware minor version.
			/// </summary>
			public short FirmwareMinor;

			/// <summary>
			/// Capability bits described by HmdCaps which the HMD currently supports.
			/// </summary>
			public HmdCaps AvailableHmdCaps;

			/// <summary>
			/// Capability bits described by HmdCaps which are default for the current Hmd.
			/// </summary>
			public HmdCaps DefaultHmdCaps;

			/// <summary>
			/// Capability bits described by TrackingCaps which the system currently supports.
			/// </summary>
			public TrackingCaps AvailableTrackingCaps;

			/// <summary>
			/// Capability bits described by ovrTrackingCaps which are default for the current system.
			/// </summary>
			public TrackingCaps DefaultTrackingCaps;

			/// <summary>
			/// Defines the recommended FOVs for the HMD.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public FovPort[] DefaultEyeFov;

			/// <summary>
			/// Defines the maximum FOVs for the HMD.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public FovPort[] MaxEyeFov;

			/// <summary>
			/// Resolution of the full HMD screen (both eyes) in pixels.
			/// </summary>
			public Sizei Resolution;

			/// <summary>
			/// Nominal refresh rate of the display in cycles per second at the time of HMD creation.
			/// </summary>
			public float DisplayRefreshRate;

			/// <summary>
			/// Internal struct paddding.
			/// </summary>
			private int Pad1;
		}

		/// <summary>
		/// Bit flags describing the current status of sensor tracking.
		/// The values must be the same as in enum StatusBits
		/// </summary>
		/// <see cref="TrackingState"/>
		public enum StatusBits
		{
			/// <summary>
			/// No flags.
			/// </summary>
			None					= 0x0000,

			/// <summary>
			/// Orientation is currently tracked (connected and in use).
			/// </summary>
			OrientationTracked		= 0x0001,

			/// <summary>
			/// Position is currently tracked (false if out of range).
			/// </summary>
			PositionTracked			= 0x0002,   
		}

		/// <summary>
		/// Specifies the description of a single sensor.
		/// </summary>
		/// <see cref="OVRBase.GetTrackerDesc"/>
		[StructLayout(LayoutKind.Sequential)]
		public struct TrackerDesc
		{
			/// <summary>
			/// Sensor frustum horizontal field-of-view (if present).
			/// </summary>
			public float FrustumHFovInRadians;

			/// <summary>
			/// Sensor frustum vertical field-of-view (if present).
			/// </summary>
			public float FrustumVFovInRadians;
			
			/// <summary>
			/// Sensor frustum near Z (if present).
			/// </summary>
			public float FrustumNearZInMeters;

			/// <summary>
			/// Sensor frustum far Z (if present).
			/// </summary>
			public float FrustumFarZInMeters;
		}

		/// <summary>
		/// Specifies sensor flags.
		/// </summary>
		/// <see cref="TrackerPose"/>
		public enum TrackerFlags
		{
			/// <summary>
			/// No flags.
			/// </summary>
			None		= 0,

			/// <summary>
			/// The sensor is present, else the sensor is absent or offline.
			/// </summary>
			Connected   = 0x0020,

			/// <summary>
			/// The sensor has a valid pose, else the pose is unavailable. 
			/// This will only be set if TrackerFlags.Connected is set.
			/// </summary>
			PoseTracked = 0x0004,
		}

		/// <summary>
		/// Specifies the pose for a single sensor.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct TrackerPose
		{
			/// <summary>
			/// TrackerFlags
			/// </summary>
			public TrackerFlags	TrackerFlags;

			/// <summary>
			/// The sensor's pose. This pose includes sensor tilt (roll and pitch). 
			/// For a leveled coordinate system use LeveledPose.
			/// </summary>
			public Posef		Pose;

			/// <summary>
			/// The sensor's leveled pose, aligned with gravity. 
			/// This value includes position and yaw of the sensor, but not roll and pitch. It can be used as a reference point to render real-world objects in the correct location.
			/// </summary>
			public Posef		LeveledPose;
		}

		/// <summary>
		/// Tracking state at a given absolute time (describes predicted HMD pose etc).
		/// Returned by ovr_GetTrackingState.
		/// <see cref="OVRBase.GetTrackingState"/>
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct TrackingState
		{
			/// <summary>
			/// Predicted head pose (and derivatives) at the requested absolute time.
			/// </summary>
			public PoseStatef	HeadPose;

			/// <summary>
			/// HeadPose tracking status described by StatusBits.
			/// </summary>
			public StatusBits	StatusFlags;

			/// <summary>
			/// The most recent calculated pose for each hand when hand controller tracking is present.
			/// HandPoses[ovrHand_Left] refers to the left hand and HandPoses[ovrHand_Right] to the right hand.
			/// These values can be combined with ovrInputState for complete hand controller information.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public PoseStatef[]	HandPoses;

			/// <summary>
			/// HandPoses status flags described by StatusBits.
			/// Only OrientationTracked and PositionTracked are reported.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public StatusBits[] HandStatusFlags;

			/// <summary>
			/// The pose of the origin captured during calibration.
			/// 
			/// Like all other poses here, this is expressed in the space set by ovr_RecenterTrackingOrigin,
			/// and so will change every time that is called. This pose can be used to calculate
			/// where the calibrated origin lands in the new recentered space.
			/// 
			/// If an application never calls ovr_RecenterTrackingOrigin, expect this value to be the identity
			/// pose and as such will point respective origin based on TrackingOrigin requested when
			/// calling ovr_GetTrackingState.
			/// </summary>
			public Posef CalibratedOrigin;
		}

		/// <summary>
		/// Rendering information for each eye. Computed by ovr_GetRenderDesc() based on the
		/// specified FOV. Note that the rendering viewport is not included
		/// here as it can be specified separately and modified per frame by
		/// passing different Viewport values in the layer structure.
		/// </summary>
		/// <see cref="OVRBase.GetRenderDesc"/>
		[StructLayout(LayoutKind.Sequential, Pack=4)]
		public struct EyeRenderDesc
		{
			/// <summary>
			/// The eye index to which this instance corresponds.
			/// </summary>
    		public EyeType  Eye;

			/// <summary>
			/// The field of view.
			/// </summary>
			public FovPort	Fov;

			/// <summary>
			/// Distortion viewport.
			/// </summary>
			public Recti	DistortedViewport; 	        

			/// <summary>
			/// How many display pixels will fit in tan(angle) = 1.
			/// </summary>
			public Vector2f	PixelsPerTanAngleAtCenter;

			/// <summary>
			/// Translation of each eye, in meters.
			/// </summary>
			public Vector3f	HmdToEyeOffset;
		}

		/// <summary>
		/// The type of texture resource.
		/// </summary>
		/// <see cref="TextureSwapChainDesc"/>
		public enum TextureType
		{
			/// <summary>
			/// 2D textures.
			/// </summary>
			Texture2D,

			/// <summary>
			/// External 2D texture. 
			/// 
			/// Not used on PC.
			/// </summary>
			Texture2DExternal,

			/// <summary>
			/// Cube maps. 
			/// 
			/// Not currently supported on PC.
			/// </summary>
			TextureCube,

			/// <summary>
			/// Undocumented.
			/// </summary>
			TextureCount,
		}

		/// <summary>
		/// The bindings required for texture swap chain.
		///
		/// All texture swap chains are automatically bindable as shader
		/// input resources since the Oculus runtime needs this to read them.
		/// </summary>
		/// <see cref="TextureSwapChainDesc"/>
		public enum TextureBindFlags
		{
			None,

			/// <summary>
			/// The application can write into the chain with pixel shader.
			/// </summary>
			DX_RenderTarget = 0x0001,

			/// <summary>
			/// The application can write to the chain with compute shader.
			/// </summary>
			DX_UnorderedAccess = 0x0002,

			/// <summary>
			/// The chain buffers can be bound as depth and/or stencil buffers.
			/// </summary>
			DX_DepthStencil = 0x0004,
		}

		/// <summary>
		/// The format of a texture.
		/// </summary>
		/// <see cref="TextureSwapChainDesc"/>
		public enum TextureFormat
		{
			UNKNOWN,

			/// <summary>
			/// Not currently supported on PC. Would require a DirectX 11.1 device.
			/// </summary>
			B5G6R5_UNORM,

			/// <summary>
			/// Not currently supported on PC. Would require a DirectX 11.1 device.
			/// </summary>
			B5G5R5A1_UNORM,

			/// <summary>
			/// Not currently supported on PC. Would require a DirectX 11.1 device.
			/// </summary>
			B4G4R4A4_UNORM,

			R8G8B8A8_UNORM,
			R8G8B8A8_UNORM_SRGB,
			B8G8R8A8_UNORM,

			/// <summary>
			/// Not supported for OpenGL applications
			/// </summary>
			B8G8R8A8_UNORM_SRGB,

			/// <summary>
			/// Not supported for OpenGL applications
			/// </summary>
			B8G8R8X8_UNORM,

			/// <summary>
			/// Not supported for OpenGL applications
			/// </summary>
			B8G8R8X8_UNORM_SRGB,

			R16G16B16A16_FLOAT,
			D16_UNORM,
			D24_UNORM_S8_UINT,
			D32_FLOAT,
			D32_FLOAT_S8X24_UINT,
		}

		/// <summary>
		/// Misc flags overriding particular behaviors of a texture swap chain
		/// </summary>
		/// <see cref="TextureSwapChainDesc"/>
		public enum TextureMiscFlags
		{
			None = 0,

			/// <summary>
			/// DX only: The underlying texture is created with a TYPELESS equivalent of the
			/// format specified in the texture desc. The SDK will still access the
			/// texture using the format specified in the texture desc, but the app can
			/// create views with different formats if this is specified.
			/// </summary>
			DX_Typeless = 0x0001,

			/// <summary>
			/// DX only: Allow generation of the mip chain on the GPU via the GenerateMips
			/// call. This flag requires that RenderTarget binding also be specified.
			/// </summary>
			AllowGenerateMips = 0x0002,
		}

		/// <summary>
		/// Description used to create a texture swap chain.
		/// </summary>
		/// <see cref="OVRBase.CreateTextureSwapChainDX"/>
		/// <see cref="OVRBase.CreateTextureSwapChainGL"/>
		[StructLayout(LayoutKind.Sequential)]
		public struct TextureSwapChainDesc
		{
			public TextureType			Type;
			public TextureFormat		Format;

			/// <summary>
			/// Only supported with ovrTexture_2D. 
			/// Not supported on PC at this time.
			/// </summary>
			public int					ArraySize;

			public int					Width;

			public int					Height;

			public int					MipLevels;

			/// <summary>
			/// Current only supported on depth textures
			/// </summary>
			public int					SampleCount;

			/// <summary>
			/// Not buffered in a chain. For images that don't change
			/// </summary>
			public ovrBool				StaticImage;

			/// <summary>
			/// ovrTextureMiscFlags
			/// </summary>
			public TextureMiscFlags		MiscFlags;

			/// <summary>
			/// ovrTextureBindFlags. Not used for GL.
			/// </summary>
			public TextureBindFlags		BindFlags;
		}

		/// <summary>
		/// Description used to create a mirror texture.
		/// </summary>
		/// <see cref="OVRBase.CreateMirrorTextureDX"/>
		/// <see cref="OVRBase.CreateMirrorTextureGL"/>
		[StructLayout(LayoutKind.Sequential)]
		public struct MirrorTextureDesc
		{
			public TextureFormat		Format;
			public int					Width;
			public int					Height;
			public TextureMiscFlags		MiscFlags;
		}

		/// <summary>
		/// Describes button input types.
		/// Button inputs are combined; that is they will be reported as pressed if they are 
		/// pressed on either one of the two devices.
		/// The ovrButton_Up/Down/Left/Right map to both XBox D-Pad and directional buttons.
		/// The ovrButton_Enter and ovrButton_Return map to Start and Back controller buttons, respectively.
		/// </summary>
		public enum Button
		{    
			A         = 0x00000001,
			B         = 0x00000002,
			RThumb    = 0x00000004,
			RShoulder = 0x00000008,

			/// <summary>
			/// Bit mask of all buttons on the right Touch controller
			/// </summary>
			RMask     = A | B | RThumb | RShoulder,

			X         = 0x00000100,
			Y         = 0x00000200,
			LThumb    = 0x00000400,  
			LShoulder = 0x00000800,

			/// <summary>
			/// Bit mask of all buttons on the left Touch controller
			/// </summary>
			LMask     = X | Y | LThumb | LShoulder,

			// Navigation through DPad.
			Up        = 0x00010000,
			Down      = 0x00020000,
			Left      = 0x00040000,
			Right     = 0x00080000,

			/// <summary>
			/// Start on XBox controller.
			/// </summary>
			Enter     = 0x00100000, 

			/// <summary>
			/// Back on Xbox controller.
			/// </summary>
			Back      = 0x00200000, 

			/// <summary>
			/// Only supported by Remote.
			/// </summary>
			VolUp     = 0x00400000,

			/// <summary>
			/// Only supported by Remote.
			/// </summary>
			VolDown   = 0x00800000,

			Home      = 0x01000000,  
			Private   = VolUp | VolDown | Home,
		}

		/// <summary>
		/// Describes touch input types.
		/// These values map to capacitive touch values reported ovrInputState::Touch.
		/// Some of these values are mapped to button bits for consistency.
		/// </summary>
		public enum Touch
		{
			A              = Button.A,
			B              = Button.B,
			RThumb         = Button.RThumb,
			RIndexTrigger  = 0x00000010,

			/// <summary>
			/// Bit mask of all the button touches on the right controller
			/// </summary>
			RButtonMask    = A | B | RThumb | RIndexTrigger,

			X              = Button.X,
			Y              = Button.Y,
			LThumb         = Button.LThumb,
			LIndexTrigger  = 0x00001000,

			/// <summary>
			/// Bit mask of all the button touches on the left controller
			/// </summary>
			LButtonMask    = X | Y | LThumb | LIndexTrigger,

			// Finger pose state 
			// Derived internally based on distance, proximity to sensors and filtering.
			RIndexPointing = 0x00000020,
			RThumbUp       = 0x00000040,    

			/// <summary>
			/// Bit mask of all right controller poses
			/// </summary>
			RPoseMask      = RIndexPointing | RThumbUp,

			LIndexPointing = 0x00002000,
			LThumbUp       = 0x00004000,

			/// <summary>
			/// Bit mask of all left controller poses
			/// </summary>
			LPoseMask      = LIndexPointing | LThumbUp,
		}

		/// <summary>
		/// Specifies which controller is connected; multiple can be connected at once.
		/// </summary>
		[Flags]
		public enum ControllerType
		{
			None		= 0x00,
			LTouch		= 0x01,
			RTouch		= 0x02,
			Touch		= 0x03,
			Remote		= 0x04,
			XBox		= 0x10,

			/// <summary>
			/// Operate on or query whichever controller is active.
			/// </summary>
			Active		= 0xff
		}

		/// <summary>
		/// Provides names for the left and right hand array indexes.
		/// </summary>
		/// <see cref="InputState"/>
		/// <seealso cref="TrackingState"/>
		public enum HandType
		{
			Left  = 0,
			Right = 1,
			Count = 2,
		}

		/// <summary>
		/// InputState describes the complete controller input state, including Oculus Touch,
		/// and XBox gamepad. If multiple inputs are connected and used at the same time,
		/// their inputs are combined.
		/// </summary>
		[StructLayout(LayoutKind.Sequential, Pack=4)]
		public struct InputState
		{
			/// <summary>
			/// System type when the controller state was last updated.
			/// </summary>
			public double			TimeInSeconds;

			/// <summary>
			/// Values for buttons described by ovrButton.
			/// </summary>
			public uint				Buttons;

			/// <summary>
			/// Touch values for buttons and sensors as described by ovrTouch.
			/// </summary>
			public uint				Touches;
    
			/// <summary>
			/// Left and right finger trigger values (Hand.Left and Hand.Right), in the range 0.0 to 1.0f.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public float[]			IndexTrigger;
    
			/// <summary>
			/// Left and right hand trigger values (Hand.Left and Hand.Right), in the range 0.0 to 1.0f.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public float[]			HandTrigger;

			/// <summary>
			/// Horizontal and vertical thumbstick axis values (Hand.Left and Hand.Right), in the range -1.0f to 1.0f.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public Vector2f[]		Thumbstick;

			/// <summary>
			/// The type of the controller this state is for.
			/// </summary>
			public ControllerType	ControllerType;
		}

		/// <summary>
		/// Contains the data necessary to properly calculate position info for various layer types.
		/// </summary>
		/// <see cref="EyeRenderDesc"/>
		/// <see cref="OVRBase.SubmitFrame"/>
		[StructLayout(LayoutKind.Sequential, Pack=4)]
		public struct ViewScaleDesc
		{
			/// <summary>
			/// Translation of each eye.
			/// 
			/// The same value pair provided in EyeRenderDesc.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public Vector3f[]	HmdToEyeOffset;
			
			/// <summary>
			/// Ratio of viewer units to meter units.
			/// 
			/// Used to scale player motion into in-application units.
			/// In other words, it is how big an in-application unit is in the player's physical meters.
			/// For example, if the application uses inches as its units then HmdSpaceToWorldScaleInMeters would be 0.0254.
			/// Note that if you are scaling the player in size, this must also scale. So if your application
			/// units are inches, but you're shrinking the player to half their normal size, then
			/// HmdSpaceToWorldScaleInMeters would be 0.0254*2.0.
			/// </summary>
			public float		HmdSpaceToWorldScaleInMeters;
		}

		/// <summary>
		/// Enumerates modifications to the projection matrix based on the application's needs.
		/// </summary>
		/// <see cref="OVRBase.Matrix4f_Projection"/>
		public enum ProjectionModifier
		{
			/// <summary>
			/// Use for generating a default projection matrix that is:
		    /// * Right-handed.
			/// * Near depth values stored in the depth buffer are smaller than far depth values.
			/// * Both near and far are explicitly defined.
			/// * With a clipping range that is (0 to w).
			/// </summary>
			None				= 0x00,

			/// <summary>
			/// Enable if using left-handed transformations in your application.
			/// </summary>
			LeftHanded = 0x01,

			/// <summary>
			/// After the projection transform is applied, far values stored in the depth buffer will be less than closer depth values.
			/// NOTE: Enable only if the application is using a floating-point depth buffer for proper precision.
			/// </summary>
			FarLessThanNear		= 0x02,

			/// <summary>
			/// When this flag is used, the zfar value pushed into ovrMatrix4f_Projection() will be ignored
			/// NOTE: Enable only if ovrProjection_FarLessThanNear is also enabled where the far clipping plane will be pushed to infinity.
			/// </summary>
			FarClipAtInfinity	= 0x04,

			/// <summary>
			/// Enable if the application is rendering with OpenGL and expects a projection matrix with a clipping range of (-w to w).
			/// Ignore this flag if your application already handles the conversion from D3D range (0 to w) to OpenGL.
			/// </summary>
			ClipRangeOpenGL		= 0x08
		}


		/// <summary>
		/// Return values for ovr_Detect.
		/// </summary>
		/// <see cref="OVRBase.Detect"/>
		[StructLayout(LayoutKind.Sequential, Pack=8, Size=8)]
		public struct DetectResult
		{
			/// <summary>
			/// Is ovrFalse when the Oculus Service is not running.
			///   This means that the Oculus Service is either uninstalled or stopped.
			///   IsOculusHMDConnected will be ovrFalse in this case.
			/// Is ovrTrue when the Oculus Service is running.
			///   This means that the Oculus Service is installed and running.
			///   IsOculusHMDConnected will reflect the state of the HMD.
			/// </summary>
			public ovrBool IsOculusServiceRunning;

			/// <summary>
			/// Is ovrFalse when an Oculus HMD is not detected.
			///   If the Oculus Service is not running, this will be ovrFalse.
			/// Is ovrTrue when an Oculus HMD is detected.
			///   This implies that the Oculus Service is also installed and running.
			/// </summary>
			public ovrBool IsOculusHMDConnected;
		}

		/// <summary>
		/// Projection information for LayerEyeFovDepth.
		/// 
		/// Use the utility function ovrTimewarpProjectionDesc_FromProjection to
		/// generate this structure from the application's projection matrix.
		/// </summary>
		/// <see cref="OVRBase.TimewarpProjectionDesc_FromProjection"/>
		[StructLayout(LayoutKind.Sequential, Pack=4)]
		public struct TimewarpProjectionDesc
		{
			/// <summary>
			/// Projection matrix element [2][2].
			/// </summary>
			public float Projection22;

			/// <summary>
			/// Projection matrix element [2][3].
			/// </summary>
			public float Projection23;

			/// <summary>
			/// Projection matrix element [3][2].
			/// </summary>
			public float Projection32;
		}

		/// <summary>
		/// Describes layer types that can be passed to ovr_SubmitFrame.
		/// Each layer type has an associated struct, such as ovrLayerEyeFov.
		/// </summary>
		/// <see cref="LayerHeader"/>
		public enum LayerType
		{
			/// <summary>
			/// Layer is disabled.
			/// </summary>
			Disabled       = 0,

			/// <summary>
			/// Described by LayerEyeFov.
			/// </summary>
			EyeFov         = 1,

			/// <summary>
			/// Described by LayerQuad. 
			/// 
			/// Previously called QuadInWorld.
			/// </summary>
			Quad           = 3,

			// enum 4 used to be ovrLayerType_QuadHeadLocked. 
			// Instead, use ovrLayerType_Quad with ovrLayerFlag_HeadLocked.

			/// <summary>
			/// Described by LayerEyeMatrix.
			/// </summary>
			EyeMatrix      = 5,
		}

		/// <summary>
		/// Identifies flags used by LayerHeader and which are passed to ovr_SubmitFrame.
		/// </summary>
		/// <see cref="LayerHeader"/>
		[Flags]
		public enum LayerFlags
		{
			/// <summary>
			/// No layer flags specified.
			/// </summary>
			None						= 0x00,

			/// <summary>
			/// HighQuality enables 4x anisotropic sampling during the composition of the layer.
			/// The benefits are mostly visible at the periphery for high-frequency &amp; high-contrast visuals.
			/// For best results consider combining this flag with an ovrTextureSwapChain that has mipmaps and
			/// instead of using arbitrary sized textures, prefer texture sizes that are powers-of-two.
			/// Actual rendered viewport and doesn't necessarily have to fill the whole texture.
			/// </summary>
			HighQuality					= 0x01,

			/// <summary>
			/// TextureOriginAtBottomLeft: the opposite is TopLeft.
			/// 
			/// Generally this is false for D3D, true for OpenGL.
			/// </summary>
			TextureOriginAtBottomLeft	= 0x02,

			/// <summary>
			/// Mark this surface as "headlocked", which means it is specified
			/// relative to the HMD and moves with it, rather than being specified
			/// relative to sensor/torso space and remaining still while the head moves.
			/// 
			/// What used to be ovrLayerType_QuadHeadLocked is now LayerType.Quad plus this flag.
			/// However the flag can be applied to any layer type to achieve a similar effect.
			/// </summary>
			HeadLocked                = 0x04
		}

		/// <summary>
		/// Defines properties shared by all ovrLayer structs, such as LayerEyeFov.
		///
		/// LayerHeader is used as a base member in these larger structs.
		/// This struct cannot be used by itself except for the case that Type is LayerType_Disabled.
		/// </summary>
		/// <see cref="LayerType"/>
		/// <see cref="LayerFlags"/>
		[StructLayout(LayoutKind.Sequential)]
		public struct LayerHeader
		{
			/// <summary>
			/// Described by LayerType.
			/// </summary>
			public LayerType	Type;

			/// <summary>
			/// Described by LayerFlags.
			/// </summary>
			public LayerFlags	Flags;
		}

		/// <summary>
		/// Describes a layer that specifies a monoscopic or stereoscopic view.
		/// 
		/// This is the kind of layer that's typically used as layer 0 to ovr_SubmitFrame,
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
		/// <see cref="TextureSwapChain"/>
		/// <see cref="OVRBase.SubmitFrame"/>
		[StructLayout(LayoutKind.Sequential)]
		public struct LayerEyeFov
		{
			/// <summary>
			/// Header.Type must be LayerType_EyeFov.
			/// </summary>
			public LayerHeader		Header;

			/// <summary>
			/// TextureSwapChains for the left and right eye respectively.
			/// 
			/// The second one of which can be null for cases described above.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public IntPtr[]			ColorTexture;

 			/// <summary>
			/// Specifies the ColorTexture sub-rect UV coordinates.
			/// 
			/// Both Viewport[0] and Viewport[1] must be valid.
 			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public Recti[]			Viewport;

			/// <summary>
			/// The viewport field of view.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public FovPort[]		Fov;

			/// <summary>
			/// Specifies the position and orientation of each eye view, with the position specified in meters.
			/// RenderPose will typically be the value returned from ovr_CalcEyePoses,
			/// but can be different in special cases if a different head pose is used for rendering.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public Posef[]			RenderPose;

			/// <summary>
			/// Specifies the timestamp when the source ovrPosef (used in calculating RenderPose)
			/// was sampled from the SDK. Typically retrieved by calling ovr_GetTimeInSeconds
			/// around the instant the application calls ovr_GetTrackingState
			/// The main purpose for this is to accurately track app tracking latency.
			/// </summary>
			public double			SensorSampleTime;
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
		/// <see cref="TextureSwapChain"/>
		/// <see cref="OVRBase.SubmitFrame"/>
		[StructLayout(LayoutKind.Sequential)]
		public struct LayerEyeMatrix
		{
			/// <summary>
			/// Header.Type must be ovrLayerType_EyeMatrix.
			/// </summary>
			public LayerHeader		Header;

			/// <summary>
			/// TextureSwapChains for the left and right eye respectively.
			/// 
			/// The second one of which can be NULL for cases described above.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public IntPtr[]			ColorTexture;

			/// <summary>
			/// Specifies the ColorTexture sub-rect UV coordinates.
			/// Both Viewport[0] and Viewport[1] must be valid.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public Recti[]			Viewport;

			/// <summary>
			/// Specifies the position and orientation of each eye view, with the position specified in meters.
			/// RenderPose will typically be the value returned from ovr_CalcEyePoses,
			/// but can be different in special cases if a different head pose is used for rendering.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public Posef[]			RenderPose;

			/// <summary>
			/// Specifies the mapping from a view-space vector
			/// to a UV coordinate on the textures given above.
			/// P = (x,y,z,1)*Matrix
			/// TexU  = P.x/P.z
			/// TexV  = P.y/P.z
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public Matrix4f[] Matrix;

			/// <summary>
			/// Specifies the timestamp when the source ovrPosef (used in calculating RenderPose)
			/// was sampled from the SDK. Typically retrieved by calling ovr_GetTimeInSeconds
			/// around the instant the application calls ovr_GetTrackingState
			/// The main purpose for this is to accurately track app tracking latency.
			/// </summary>
			public double SensorSampleTime;
		};

		/// <summary>
		/// Describes a layer of Quad type, which is a single quad in world or viewer space.
		/// It is used for ovrLayerType_Quad. This type of layer represents a single
		/// object placed in the world and not a stereo view of the world itself.
		///
		/// A typical use of ovrLayerType_Quad is to draw a television screen in a room
		/// that for some reason is more convenient to draw as a layer than as part of the main
		/// view in layer 0. For example, it could implement a 3D popup GUI that is drawn at a
		/// higher resolution than layer 0 to improve fidelity of the GUI.
		///
		/// Quad layers are visible from both sides; they are not back-face culled.
		/// </summary>
		/// <see cref="TextureSwapChain"/>
		/// <see cref="OVRBase.SubmitFrame"/>
		[StructLayout(LayoutKind.Sequential)]
		public struct LayerQuad
		{
			/// <summary>
		    /// Header.Type must be ovrLayerType_Quad.
			/// </summary>
			public LayerHeader		Header;

			/// <summary>
			/// Contains a single image, never with any stereo view.
			/// </summary>
			public IntPtr			ColorTexture;

			/// <summary>
			/// Specifies the ColorTexture sub-rect UV coordinates.
			/// </summary>
			public Recti			Viewport;

			/// <summary>
			/// Specifies the orientation and position of the center point of a Quad layer type.
			/// The supplied direction is the vector perpendicular to the quad.
			/// The position is in real-world meters (not the application's virtual world,
			/// the physical world the user is in) and is relative to the "zero" position
			/// set by ovr_RecenterTrackingOrigin unless the ovrLayerFlag_HeadLocked flag is used.
			/// </summary>
			public Posef			QuadPoseCenter;

			/// <summary>
			/// Width and height (respectively) of the quad in meters.
			/// </summary>
			public Vector2f			QuadSize;
		}

		/// <summary>
		/// Performance HUD enables the HMD user to see information critical to
		/// the real-time operation of the VR application such as latency timing,
		/// and CPU &amp; GPU performance metrics
		/// </summary>
		/// <example>
		/// App can toggle performance HUD modes as such:
		/// 
		/// PerfHudMode perfHudMode = PerfHudMode.Hud_LatencyTiming;
		/// ovr_SetInt(Hmd, "PerfHudMode", (int) perfHudMode);
		/// </example>
		public enum PerfHudMode
		{

			/// <summary>
			/// Shows performance summary and headroom
			/// </summary>
			PerfSummary        = 1,

			/// <summary>
			/// Shows latency related timing info
			/// </summary>
			LatencyTiming      = 2,

			/// <summary>
			/// Shows render timing info for application
			/// </summary>
			AppRenderTiming    = 3,

			/// <summary>
			/// Shows render timing info for OVR compositor
			/// </summary>
			CompRenderTiming   = 4,

			/// <summary>
			/// Shows SDK &amp; HMD version Info
			/// </summary>
			VersionInfo        = 5,

			/// <summary>
			/// Count of enumerated elements.
			/// </summary>
			Count              = 6
		}

		/// <summary>
		/// Layer HUD enables the HMD user to see information about a layer
		/// <example>
		/// <code>
		///     App can toggle layer HUD modes as such:
		///         ovrLayerHudMode LayerHudMode = ovrLayerHud_Info;
		///         ovr_SetInt(Hmd, OVR_LAYER_HUD_MODE, (int)LayerHudMode);
		/// </code>
		/// </example>
		/// </summary>
		public enum ovrLayerHudMode
		{
			/// <summary>
			/// Turns off the layer HUD
			/// </summary>
			Off			= 0,

			/// <summary>
			/// Shows info about a specific layer
			/// </summary>
			Info		= 1,
		}

		/// <summary>
		/// Debug HUD is provided to help developers gauge and debug the fidelity of their app's
		/// stereo rendering characteristics. Using the provided quad and crosshair guides, 
		/// the developer can verify various aspects such as VR tracking units (e.g. meters),
		/// stereo camera-parallax properties (e.g. making sure objects at infinity are rendered
		/// with the proper separation), measuring VR geometry sizes and distances and more.
		///
		///     App can toggle the debug HUD modes as such:
		///     \code{.cpp}
		///     \endcode
		///
		/// The app can modify the visual properties of the stereo guide (i.e. quad, crosshair)
		/// using the ovr_SetFloatArray function. For a list of tweakable properties,
		/// see the OVR_DEBUG_HUD_STEREO_GUIDE_* keys in the OVR_CAPI_Keys.h header file.
		/// </summary>
		/// <example>
		/// ovrDebugHudStereoMode DebugHudMode = ovrDebugHudStereo.QuadWithCrosshair;
		/// ovr_SetInt(Hmd, OVR_DEBUG_HUD_STEREO_MODE, (int)DebugHudMode);
		/// </example>
		public enum DebugHudStereoMode
		{
			/// <summary>
			/// Turns off the Stereo Debug HUD
			/// </summary>
			Off                 = 0,

			/// <summary>
			/// Renders Quad in world for Stereo Debugging
			/// </summary>
			Quad                = 1,

			/// <summary>
			/// Renders Quad+crosshair in world for Stereo Debugging
			/// </summary>
			QuadWithCrosshair   = 2,

			/// <summary>
			/// Renders screen-space crosshair at infinity for Stereo Debugging
			/// </summary>
			CrosshairAtInfinity = 3,

			/// <summary>
			/// Count of enumerated elements
			/// </summary>
			Count
		}

		/// <summary>
		/// Specifies status information for the current session.
		/// </summary>
		/// <see cref="OVRBase.GetSessionStatus"/>
		[StructLayout(LayoutKind.Sequential)]
		public struct SessionStatus
		{
			/// <summary>
			/// True if the process has VR focus and thus is visible in the HMD.
			/// </summary>
			public ovrBool IsVisible;

			/// <summary>
			/// True if an HMD is present.
			/// </summary>
			public ovrBool HmdPresent;

			/// <summary>
			/// True if the HMD is on the user's head.
			/// </summary>
			public ovrBool HmdMounted;

			/// <summary>
			/// True if the session is in a display-lost state. See ovr_SubmitFrame.
			/// </summary>
			public ovrBool DisplayLost;

			/// <summary>
			/// True if the application should initiate shutdown.    
			/// </summary>
			public ovrBool ShouldQuit;

			/// <summary>
			/// True if UX has requested re-centering. 
			/// Must call ovr_ClearShouldRecenterFlag or ovr_RecenterTrackingOrigin.
			/// </summary>
			public ovrBool ShouldRecenter;
		}
		#endregion

		#pragma warning restore 1591
	}
}
