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
		/// <see cref="ovr_SubmitFrame"/>
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
		/// <seealso cref="ovr_Initialize"/>
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
			/// <see cref="ovr_GetTimeInSeconds"/>
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
		/// <see cref="ovr_GetTrackerDesc"/>
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
		/// <see cref="ovr_GetTrackingState"/>
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
		/// <see cref="ovr_GetRenderDesc"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateMirrorTextureDX"/>
		/// <see cref="ovr_CreateMirrorTextureGL"/>
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
		/// <see cref="ovr_SubmitFrame"/>
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
		/// <see cref="ovrMatrix4f_Projection"/>
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
		/// <see cref="ovr_Detect"/>
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
		/// <see cref="ovrTimewarpProjectionDesc_FromProjection"/>
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
		/// <see cref="ovrTextureSwapChain"/>
		/// <see cref="ovr_SubmitFrame"/>
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
		/// <see cref="ovrTextureSwapChain"/>
		/// <see cref="ovr_SubmitFrame"/>
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
		/// <see cref="ovrTextureSwapChain"/>
		/// <see cref="ovr_SubmitFrame"/>
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
		/// <see cref="ovr_GetSessionStatus"/>
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

	/// <summary>
	/// Base interface to Oculus runtime methods.
	/// 
	/// It's not recommended that you use this class directly, unless you want to create your own set of .NET wrapper classes.
	/// Instead, use the Wrap and Hmd classes, which take care of the marshalling of arguments.
	/// </summary>
    public abstract class OVRBase	:OVRTypes, IDisposable
    {
		/// <summary>
		/// Initializes the library, by loading the oculus runtime dll into memory.
		/// </summary>
		public OVRBase()
		{
			LoadLibrary();
		}

		/// <summary>
		/// Finalizer, cleaning up unmanaged resources.
		/// </summary>
		~OVRBase()
		{
			Dispose(false);
		}

		#region IDisposable Members
		/// <summary>
		/// Clean up unmanaged resources.
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

			// Unload the Oculus runtime dll from memory.
			UnloadLibrary();

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

		#region Oculus runtime initialization methods
		/// <summary>
		/// Loads the Oculus runtime dll into memory.
		/// </summary>
		protected abstract void LoadLibrary();

		/// <summary>
		/// Unloads the Oculus runtime library from memory.
		/// </summary>
		protected abstract void UnloadLibrary();
		#endregion

		// The Oculus SDK is full of missing comments.
		// Ignore warnings regarding missing comments, in this class.
		#pragma warning disable 1591

		#region Oculus SDK methods
		/// <summary>
		/// Detects Oculus Runtime and Device Status
		///
		/// Checks for Oculus Runtime and Oculus HMD device status without loading the LibOVRRT
		/// shared library.  This may be called before Initialize() to help decide whether or
		/// not to initialize LibOVR.
		/// </summary>
		/// <param name="timeoutMilliseconds">Specifies a timeout to wait for HMD to be attached or 0 to poll.</param>
		/// <returns>Returns a DetectResult object indicating the result of detection.</returns>
		/// <see cref="DetectResult"/>
		public abstract DetectResult Detect(int timeoutMilliseconds);

		/// <summary>
		/// Initializes all Oculus functionality.
		/// </summary>
		/// <param name="parameters">
		/// Initialize with extra parameters.
		/// Pass 0 to initialize with default parameters, suitable for released games.
		/// </param>
		/// <remarks>
		/// Library init/shutdown, must be called around all other OVR code.
		/// No other functions calls besides InitializeRenderingShim are allowed
		/// before Initialize succeeds or after Shutdown.
		/// 
		/// LibOVRRT shared library search order:
		///      1) Current working directory (often the same as the application directory).
		///      2) Module directory (usually the same as the application directory, but not if the module is a separate shared library).
		///      3) Application directory
		///      4) Development directory (only if OVR_ENABLE_DEVELOPER_SEARCH is enabled, which is off by default).
		///      5) Standard OS shared library search location(s) (OS-specific).
		/// </remarks>
		public abstract Result Initialize(InitParams parameters=null);

		/// <summary>
		/// Returns information about the most recent failed return value by the
		/// current thread for this library.
		/// 
		/// This function itself can never generate an error.
		/// The last error is never cleared by LibOVR, but will be overwritten by new errors.
		/// Do not use this call to determine if there was an error in the last API 
		/// call as successful API calls don't clear the last ErrorInfo.
		/// To avoid any inconsistency, GetLastErrorInfo should be called immediately
		/// after an API function that returned a failed ovrResult, with no other API
		/// functions called in the interim.
		/// </summary>
		/// <param name="errorInfo">The last ErrorInfo for the current thread.</param>
		/// <remarks>
		/// Allocate an ErrorInfo and pass this as errorInfo argument.
		/// </remarks>
		public abstract void GetLastErrorInfo(out ErrorInfo errorInfo);

		/// <summary>
		/// Returns version string representing libOVR version. Static, so
		/// string remains valid for app lifespan
		/// </summary>
		/// <remarks>
		/// Use Marshal.PtrToStringAnsi() to retrieve version string.
		/// </remarks>
		public abstract IntPtr GetVersionString();

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
		public abstract int TraceMessage(LogLevel level, string message);

		/// <summary>
		/// Shuts down all Oculus functionality.
		/// </summary>
		/// <remarks>
		/// No API functions may be called after Shutdown except Initialize.
		/// </remarks>
		public abstract void Shutdown();

		/// <summary>
		/// Returns information about the current HMD.
		/// 
		/// Initialize must have first been called in order for this to succeed, otherwise HmdDesc.Type
		/// will be reported as None.
		/// 
		/// Please note: This method will should only be called by a 32 bit process. 
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create, else NULL in which
		/// case this function detects whether an HMD is present and returns its info if so.
		/// </param>
		/// <param name="result">
		/// Returns an ovrHmdDesc. If the hmd is null and ovrHmdDesc::Type is None then
		/// no HMD is present.
		/// </param>
		public abstract void GetHmdDesc32(out HmdDesc result, ovrSession session);

		/// <summary>
		/// Returns information about the current HMD.
		/// 
		/// Initialize must have first been called in order for this to succeed, otherwise HmdDesc.Type
		/// will be reported as None.
		/// 
		/// Please note: This method will should only be called by a 64 bit process. 
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create, else NULL in which
		/// case this function detects whether an HMD is present and returns its info if so.
		/// </param>
		/// <param name="result">
		/// Returns an ovrHmdDesc. If the hmd is null and ovrHmdDesc::Type is None then
		/// no HMD is present.
		/// </param>
		public abstract void GetHmdDesc64(out HmdDesc64 result, ovrSession session);

		/// <summary>
		/// Returns the number of sensors. 
		///
		/// The number of sensors may change at any time, so this function should be called before use 
		/// as opposed to once on startup.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <returns>Returns unsigned int count.</returns>
		public abstract uint GetTrackerCount(ovrSession session);

		/// <summary>
		/// Returns a given sensor description.
		///
		/// It's possible that sensor desc [0] may indicate a unconnnected or non-pose tracked sensor, but 
		/// sensor desc [1] may be connected.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="trackerDescIndex">
		/// Specifies a sensor index. The valid indexes are in the range of 0 to the sensor count returned by GetTrackerCount.
		/// </param>
		/// <returns>An empty ovrTrackerDesc will be returned if trackerDescIndex is out of range.</returns>
		/// <see cref="TrackerDesc"/>
		/// <see cref="GetTrackerCount"/>
		public abstract TrackerDesc GetTrackerDesc(ovrSession session, uint trackerDescIndex);

		/// <summary>
		/// Creates a handle to a VR session.
		/// 
		/// Upon success the returned ovrSession must be eventually freed with Destroy when it is no longer needed.
		/// A second call to Create will result in an error return value if the previous Hmd has not been destroyed.
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
		/// ovrResult result = Create(ref session, ref luid);
		/// if(OVR_FAILURE(result))
		/// ...
		/// </code>
		/// </example>
		/// <see cref="Destroy"/>
		public abstract Result Create(ref ovrSession session, ref GraphicsLuid pLuid);

		/// <summary>
		/// Destroys the HMD.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		public abstract void Destroy(ovrSession session);

		/// <summary>
		/// Returns status information for the application.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="sessionStatus">Provides a SessionStatus that is filled in.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use GetLastErrorInfo 
		/// to get more information.
		/// Return values include but aren't limited to:
		/// - Result.Success: Completed successfully.
		/// - Result.ServiceConnection: The service connection was lost and the application must destroy the session.
		/// </returns>
		public abstract Result GetSessionStatus(ovrSession session, ref SessionStatus sessionStatus);

		/// <summary>
		/// Sets the tracking origin type
		///
		/// When the tracking origin is changed, all of the calls that either provide
		/// or accept ovrPosef will use the new tracking origin provided.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="origin">Specifies an ovrTrackingOrigin to be used for all ovrPosef</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. 
		/// In the case of failure, use GetLastErrorInfo to get more information.
		/// </returns>
		/// <see cref="TrackingOrigin"/>
		/// <see cref="GetTrackingOriginType"/>
		public abstract Result SetTrackingOriginType(ovrSession session, TrackingOrigin origin);

		/// <summary>
		/// Gets the tracking origin state
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <returns>Returns the TrackingOrigin that was either set by default, or previous set by the application.</returns>
		/// <see cref="TrackingOrigin"/>
		/// <see cref="SetTrackingOriginType"/>
		public abstract TrackingOrigin GetTrackingOriginType(ovrSession session);

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
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use
		/// GetLastErrorInfo to get more information. Return values include but aren't limited to:
		/// - Result.Success: Completed successfully.
		/// - Result.InvalidHeadsetOrientation: The headset was facing an invalid direction when attempting recentering, 
		///   such as facing vertically.
		/// </returns>
		/// <see cref="TrackingOrigin"/>
		/// <see cref="GetTrackerPose"/>
		public abstract Result RecenterTrackingOrigin(ovrSession session);
		
		/// <summary>
		/// Clears the ShouldRecenter status bit in ovrSessionStatus.
		///
		/// Clears the ShouldRecenter status bit in ovrSessionStatus, allowing further recenter 
		/// requests to be detected. Since this is automatically done by RecenterTrackingOrigin,
		/// this is only needs to be called when application is doing its own re-centering.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		public abstract void ClearShouldRecenterFlag(ovrSession session);

		/// <summary>
		/// Returns the ovrTrackerPose for the given sensor.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="trackerPoseIndex">Index of the sensor being requested.</param>
		/// <returns>
		/// Returns the requested ovrTrackerPose. An empty ovrTrackerPose will be returned if trackerPoseIndex is out of range.
		/// </returns>
		/// <see cref="GetTrackerCount"/>
		public abstract TrackerPose GetTrackerPose(ovrSession session, uint trackerPoseIndex);

		/// <summary>
		/// Returns the most recent input state for controllers, without positional tracking info.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="controllerType">Specifies which controller the input will be returned for.</param>
		/// <param name="inputState">Input state that will be filled in.</param>
		/// <returns>Returns Result.Success if the new state was successfully obtained.</returns>
		/// <see cref="ControllerType"/>
		public abstract Result GetInputState(ovrSession session, ControllerType controllerType, ref InputState inputState);
		
		/// <summary>
		/// Returns controller types connected to the system OR'ed together.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <returns>A bitmask of ControllerTypes connected to the system.</returns>
		/// <see cref="ControllerType"/>
		public abstract ControllerType GetConnectedControllerTypes(ovrSession session);

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
		/// Specifies an ovrSession previously returned by Create.
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
		/// <see cref="TrackingState"/>
		/// <see cref="GetEyePoses"/>
		/// <see cref="GetTimeInSeconds"/>
		public abstract void GetTrackingState(out TrackingState result, ovrSession session, double absTime, ovrBool latencyMarker);

		/// <summary>
		/// Turns on vibration of the given controller.
		///
		/// To disable vibration, call SetControllerVibration with an amplitude of 0.
		/// Vibration automatically stops after a nominal amount of time, so if you want vibration 
		/// to be continuous over multiple seconds then you need to call this function periodically.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="controllerType">Specifies controllers to apply the vibration to.</param>
		/// <param name="frequency">
		/// Specifies a vibration frequency in the range of 0.0 to 1.0. 
		/// Currently the only valid values are 0.0, 0.5, and 1.0 and other values will
		/// be clamped to one of these.
		/// </param>
		/// <param name="amplitude">Specifies a vibration amplitude in the range of 0.0 to 1.0.</param>
		/// <returns>Returns ovrSuccess upon success.</returns>
		/// <see cref="ControllerType"/>
		public abstract Result SetControllerVibration(ovrSession session, ControllerType controllerType, float frequency, float amplitude);

		// SDK Distortion Rendering
		//
		// All of rendering functions including the configure and frame functions
		// are not thread safe. It is OK to use ConfigureRendering on one thread and handle
		// frames on another thread, but explicit synchronization must be done since
		// functions that depend on configured state are not reentrant.
		//
		// These functions support rendering of distortion by the SDK.

		// ovrTextureSwapChain creation is rendering API-specific.
		// CreateTextureSwapChainDX and CreateTextureSwapChainGL can be found in the
		// rendering API-specific headers, such as OVR_CAPI_D3D.h and OVR_CAPI_GL.h


		/// <summary>
		/// Gets the number of buffers in an ovrTextureSwapChain.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain for which the length should be retrieved.</param>
		/// <param name="out_Length">Returns the number of buffers in the specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		public abstract Result GetTextureSwapChainLength(ovrSession session, ovrTextureSwapChain chain, out int out_Length);

		/// <summary>
		/// Gets the current index in an ovrTextureSwapChain.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain for which the index should be retrieved.</param>
		/// <param name="out_Index">Returns the current (free) index in specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		public abstract Result GetTextureSwapChainCurrentIndex(ovrSession session, ovrTextureSwapChain chain, out int out_Index);

		/// <summary>
		/// Gets the description of the buffers in an ovrTextureSwapChain
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain for which the description should be retrieved.</param>
		/// <param name="out_Desc">Returns the description of the specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		public abstract Result GetTextureSwapChainDesc(ovrSession session, ovrTextureSwapChain chain, ref TextureSwapChainDesc out_Desc);

		/// <summary>
		/// Commits any pending changes to an ovrTextureSwapChain, and advances its current index
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain to commit.</param>
		/// <returns>
		/// Returns an ovrResult for which the return code is negative upon error.
		/// Failures include but aren't limited to:
		///   - Result.TextureSwapChainFull: CommitTextureSwapChain was called too many times on a texture swapchain without calling submit to use the chain.
		/// </returns>
		/// <see cref="CreateTextureSwapChainDX"/>
		/// <see cref="CreateTextureSwapChainGL"/>
		public abstract Result CommitTextureSwapChain(ovrSession session, ovrTextureSwapChain chain);

		/// <summary>
		/// Destroys an ovrTextureSwapChain and frees all the resources associated with it.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain to destroy. If it is null then this function has no effect.</param>
		public abstract void DestroyTextureSwapChain(ovrSession session, ovrTextureSwapChain chain);

		// MirrorTexture creation is rendering API-specific.

		/// <summary>
		/// Destroys a mirror texture previously created by one of the mirror texture creation functions.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
		/// </param>
		/// <param name="mirrorTexture">
		/// Specifies the ovrTexture to destroy. If it is null then this function has no effect.
		/// </param>
		/// <see cref="CreateMirrorTextureDX"/>
		/// <see cref="CreateMirrorTextureGL"/>
		public abstract void DestroyMirrorTexture(ovrSession session, ovrMirrorTexture mirrorTexture);

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
		/// Specifies an ovrSession previously returned by Create.
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
		public abstract Sizei GetFovTextureSize(ovrSession session, EyeType eye, FovPort fov, float pixelsPerDisplayPixel);

		/// <summary>
		/// Computes the distortion viewport, view adjust, and other rendering parameters for
		/// the specified eye.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
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
		/// <see cref="EyeRenderDesc"/>
		public abstract EyeRenderDesc GetRenderDesc(ovrSession session, EyeType eyeType, FovPort fov);

		/// <summary>
		/// Submits layers for distortion and display.
		/// 
		/// SubmitFrame triggers distortion and processing which might happen asynchronously. 
		/// The function will return when there is room in the submission queue and surfaces
		/// are available. Distortion might or might not have completed.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
		/// </param>
		/// <param name="frameIndex">
		/// Specifies the targeted application frame index, or 0 to refer to one frame 
		/// after the last time SubmitFrame was called.
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
		///       this result should stop rendering new content, but continue to call SubmitFrame periodically
		///       until it returns a value other than ovrSuccess_NotVisible.
		///     - Result.DisplayLost: The session has become invalid (such as due to a device removal)
		///       and the shared resources need to be released (DestroyTextureSwapChain), the session needs to
		///       destroyed (Destroy) and recreated (Create), and new resources need to be created
		///       (CreateTextureSwapChainXXX). The application's existing private graphics resources do not
		///       need to be recreated unless the new Create call returns a different GraphicsLuid.
		///     - Result.TextureSwapChainInvalid: The ovrTextureSwapChain is in an incomplete or inconsistent state. 
		///       Ensure CommitTextureSwapChain was called at least once first.
		/// </returns>
		/// <remarks>
		/// layerPtrList must contain an array of pointers. 
		/// Each pointer must point to an object, which starts with a an LayerHeader property.
		/// </remarks>
		/// <see cref="GetPredictedDisplayTime"/>
		/// <see cref="ViewScaleDesc"/>
		/// <see cref="LayerHeader"/>
		public abstract Result SubmitFrame(ovrSession session, Int64 frameIndex, IntPtr viewScaleDesc, IntPtr layerPtrList, uint layerCount);

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
		/// Specifies an ovrSession previously returned by Create.
		/// </param>
		/// <param name="frameIndex">
		/// Identifies the frame the caller wishes to target.
		/// A value of zero returns the next frame index.
		/// </param>
		/// <returns>
		/// Returns the absolute frame midpoint time for the given frameIndex.
		/// </returns>
		/// <see cref="GetTimeInSeconds"/>
		public abstract double GetPredictedDisplayTime(ovrSession session, Int64 frameIndex);

		/// <summary>
		/// Returns global, absolute high-resolution time in seconds. 
		///
		/// The time frame of reference for this function is not specified and should not be
		/// depended upon.
		/// </summary>
		/// <returns>
		/// Returns seconds as a floating point value.
		/// </returns>
		/// <see cref="PoseStatef"/>
		public abstract double GetTimeInSeconds();

		/// <summary>
		/// Reads a boolean property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
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
		public abstract ovrBool GetBool(ovrSession session, string propertyName, ovrBool defaultVal);

		/// <summary>
		/// Writes or creates a boolean property.
		/// If the property wasn't previously a boolean property, it is changed to a boolean property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
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
		public abstract ovrBool SetBool(ovrSession session, string propertyName, ovrBool value);

		/// <summary>
		/// Reads an integer property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
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
		public abstract int GetInt(ovrSession session, string propertyName, int defaultVal);

		/// <summary>
		/// Writes or creates an integer property.
		/// 
		/// If the property wasn't previously an integer property, it is changed to an integer property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
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
		public abstract ovrBool SetInt(ovrSession session, string propertyName, int value);

		/// <summary>
		/// Reads a float property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
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
		public abstract float GetFloat(ovrSession session, string propertyName, float defaultVal);

		/// <summary>
		/// Writes or creates a float property.
		/// 
		/// If the property wasn't previously a float property, it's changed to a float property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
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
		public abstract ovrBool SetFloat(ovrSession session, string propertyName, float value);

		/// <summary>
		/// Reads a float array property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
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
		public abstract uint GetFloatArray(ovrSession session, string propertyName, float[] values, uint valuesCapacity);

		/// <summary>
		/// Writes or creates a float array property.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
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
		public abstract ovrBool SetFloatArray(ovrSession session, string propertyName, float[] values, uint valuesSize);

		/// <summary>
		/// Reads a string property.
		/// Strings are UTF8-encoded and null-terminated.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
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
		/// The return memory is guaranteed to be valid until next call to GetString or 
		/// until the HMD is destroyed, whichever occurs first.
		/// </returns>
		public abstract IntPtr GetString(ovrSession session, string propertyName, string defaultVal);

		/// <summary>
		/// Writes or creates a string property.
		/// Strings are UTF8-encoded and null-terminated.
		/// </summary>
		/// <param name="session">
		/// Specifies an ovrSession previously returned by Create.
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
		public abstract ovrBool SetString(IntPtr session, string propertyName,string value);

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
		/// <see cref="ProjectionModifier"/>
		public abstract Matrix4f Matrix4f_Projection(FovPort fov, float znear, float zfar, ProjectionModifier projectionModFlags);

		/// <summary>
		/// Extracts the required data from the result of ovrMatrix4f_Projection.
		/// </summary>
		/// <param name="projection">Specifies the project matrix from which to extract ovrTimewarpProjectionDesc.</param>
		/// <param name="projectionModFlags">A combination of the ProjectionModifier flags.</param>
		/// <returns>Returns the extracted ovrTimewarpProjectionDesc.</returns>
		/// <see cref="TimewarpProjectionDesc"/>
		public abstract TimewarpProjectionDesc TimewarpProjectionDesc_FromProjection(Matrix4f projection, ProjectionModifier projectionModFlags);

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
		public abstract Matrix4f Matrix4f_OrthoSubProjection(Matrix4f projection, Vector2f orthoScale, float orthoDistance, float hmdToEyeOffsetX);

		/// <summary>
		/// Computes offset eye poses based on headPose returned by TrackingState.
		/// </summary>
		/// <param name="headPose">
		/// Indicates the HMD position and orientation to use for the calculation.
		/// </param>
		/// <param name="hmdToEyeOffset">
		/// Can EyeRenderDesc.HmdToEyeOffset returned from 
		/// GetRenderDesc. For monoscopic rendering, use a vector that is the average 
		/// of the two vectors for both eyes.
		/// </param>
		/// <param name="outEyePoses">
		/// If outEyePoses are used for rendering, they should be passed to 
		/// SubmitFrame in LayerEyeFov.RenderPose or LayerEyeFovDepth.RenderPose.
		/// </param>
		public abstract void CalcEyePoses(Posef headPose, Vector3f[] hmdToEyeOffset, IntPtr outEyePoses);

		/// <summary>
		/// Returns the predicted head pose in HmdTrackingState and offset eye poses in outEyePoses. 
		/// 
		/// This is a thread-safe function where caller should increment frameIndex with every frame
		/// and pass that index where applicable to functions called on the rendering thread.
		/// Assuming outEyePoses are used for rendering, it should be passed as a part of ovrLayerEyeFov.
		/// The caller does not need to worry about applying HmdToEyeOffset to the returned outEyePoses variables.
		/// </summary>
		/// <param name="session">ovrSession previously returned by Create.</param>
		/// <param name="frameIndex">
		/// Specifies the targeted frame index, or 0 to refer to one frame after 
		/// the last time SubmitFrame was called.
		/// </param>
		/// <param name="latencyMarker">
		/// Specifies that this call is the point in time where
		/// the "App-to-Mid-Photon" latency timer starts from. If a given ovrLayer
		/// provides "SensorSampleTimestamp", that will override the value stored here.
		/// </param>
		/// <param name="hmdToEyeOffset">
		/// Can be EyeRenderDesc.HmdToEyeOffset returned from GetRenderDesc. 
		/// For monoscopic rendering, use a vector that is the average of the two vectors for both eyes.
		/// </param>
		/// <param name="outEyePoses">
		/// The predicted eye poses.
		/// </param>
		/// <param name="outSensorSampleTime">
		/// The time when this function was called. 
		/// May be null, in which case it is ignored.
		/// </param>
		public abstract void GetEyePoses(ovrSession session, Int64 frameIndex, ovrBool latencyMarker, Vector3f[] hmdToEyeOffset, IntPtr outEyePoses, IntPtr outSensorSampleTime);

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
		public abstract void Posef_FlipHandedness(ref Posef inPose, ref Posef outPose);

		/// <summary>
		/// Create Texture Swap Chain suitable for use with Direct3D 11 and 12.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="d3dPtr">
		/// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue 
		/// which must be the same one the application renders to the eye textures with.</param>
		/// <param name="desc">Specifies requested texture properties. See notes for more info about texture format.</param>
		/// <param name="out_TextureSwapChain">
		/// Returns the created ovrTextureSwapChain, which will be valid upon a successful return value, else it will be null.
		/// This texture chain must be eventually destroyed via DestroyTextureSwapChain before destroying the HMD with Destroy.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// GetLastErrorInfo to get more information.
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
		public abstract Result CreateTextureSwapChainDX(ovrSession session, IntPtr d3dPtr, ref TextureSwapChainDesc desc, ref ovrTextureSwapChain out_TextureSwapChain);

		/// <summary>
		/// Get a specific buffer within the chain as any compatible COM interface (similar to QueryInterface)
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="chain">Specifies an ovrTextureSwapChain previously returned by CreateTextureSwapChainDX</param>
		/// <param name="index">
		/// Specifies the index within the chain to retrieve. Must be between 0 and length (see GetTextureSwapChainLength),
		/// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex).
		/// </param>
		/// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
		/// <param name="out_Buffer">Returns the COM interface pointer retrieved.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// GetLastErrorInfo to get more information.
		/// </returns>
		public abstract Result GetTextureSwapChainBufferDX(ovrSession session, ovrTextureSwapChain chain, int index, Guid iid, ref IntPtr out_Buffer);

		/// <summary>
		/// Create Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
		///
		/// A second call to CreateMirrorTextureDX for a given ovrSession before destroying the first one
		/// is not supported and will result in an error return.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="d3dPtr">
		/// Specifies the application's D3D11Device to create resources with or the D3D12CommandQueue
		/// which must be the same one the application renders to the textures with.
		/// </param>
		/// <param name="desc">Specifies requested texture properties. See notes for more info about texture format.</param>
		/// <param name="out_MirrorTexture">
		/// Returns the created ovrMirrorTexture, which will be valid upon a successful return value, else it will be null.
		/// This texture must be eventually destroyed via DestroyMirrorTexture before destroying the HMD with Destroy.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// GetLastErrorInfo to get more information.
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
		public abstract Result CreateMirrorTextureDX(ovrSession session, IntPtr d3dPtr, ref MirrorTextureDesc desc, ref ovrMirrorTexture out_MirrorTexture);

		/// <summary>
		/// Get a the underlying buffer as any compatible COM interface (similar to QueryInterface) 
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="mirrorTexture">Specifies an ovrMirrorTexture previously returned by CreateMirrorTextureDX</param>
		/// <param name="iid">Specifies the interface ID of the interface pointer to query the buffer for.</param>
		/// <param name="out_Buffer">Returns the COM interface pointer retrieved.</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// GetLastErrorInfo to get more information.
		/// </returns>
		public abstract Result GetMirrorTextureBufferDX(ovrSession session, ovrMirrorTexture mirrorTexture, Guid iid, ref IntPtr out_Buffer);

		/// <summary>
		/// Creates a TextureSwapChain suitable for use with OpenGL.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="desc">Specifies the requested texture properties. See notes for more info about texture format.</param>
		/// <param name="out_TextureSwapChain">
		/// Returns the created ovrTextureSwapChain, which will be valid upon
		/// a successful return value, else it will be null. This texture swap chain must be eventually
		/// destroyed via DestroyTextureSwapChain before destroying the HMD with Destroy.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// GetLastErrorInfo to get more information.
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
		public abstract Result CreateTextureSwapChainGL(ovrSession session, TextureSwapChainDesc desc, out ovrTextureSwapChain out_TextureSwapChain);

		/// <summary>
		/// Get a specific buffer within the chain as a GL texture name
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="chain">Specifies an ovrTextureSwapChain previously returned by CreateTextureSwapChainGL</param>
		/// <param name="index">
		/// Specifies the index within the chain to retrieve. Must be between 0 and length (see GetTextureSwapChainLength)
		/// or may pass -1 to get the buffer at the CurrentIndex location. (Saving a call to GetTextureSwapChainCurrentIndex)
		/// </param>
		/// <param name="out_TexId">Returns the GL texture object name associated with the specific index requested</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// GetLastErrorInfo to get more information.
		/// </returns>
		public abstract Result GetTextureSwapChainBufferGL(ovrSession session, ovrTextureSwapChain chain, int index, out uint out_TexId);

		/// <summary>
		/// Creates a Mirror Texture which is auto-refreshed to mirror Rift contents produced by this application.
		///
		/// A second call to CreateMirrorTextureGL for a given ovrSession before destroying the first one
		/// is not supported and will result in an error return.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="desc">Specifies the requested mirror texture description.</param>
		/// <param name="out_MirrorTexture">
		/// Specifies the created ovrMirrorTexture, which will be valid upon a successful return value, else it will be null.
		/// This texture must be eventually destroyed via DestroyMirrorTexture before destroying the HMD with Destroy.
		/// </param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// GetLastErrorInfo to get more information.
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
		public abstract Result CreateMirrorTextureGL(ovrSession session, MirrorTextureDesc desc, out ovrMirrorTexture out_MirrorTexture);

		/// <summary>
		/// Get a the underlying buffer as a GL texture name
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by Create.</param>
		/// <param name="mirrorTexture">Specifies an ovrMirrorTexture previously returned by CreateMirrorTextureGL</param>
		/// <param name="out_TexId">Specifies the GL texture object name associated with the mirror texture</param>
		/// <returns>
		/// Returns an ovrResult indicating success or failure. In the case of failure, use 
		/// GetLastErrorInfo to get more information.
		/// </returns>
		public abstract Result GetMirrorTextureBufferGL(ovrSession session, ovrMirrorTexture mirrorTexture, out uint out_TexId);
		#endregion

		#pragma warning restore 1591
	}

	/// <summary>
	/// Raw interface to LibOVR methods.
	/// 
	/// It's not recommended that you use this class directly, unless you want to create your own set of .NET wrapper classes.
	/// Instead, use the Oculus and Hmd classes, which take care of the marshalling of arguments.
	/// </summary>
    public class OVR32	:OVRBase
    {
		/// <summary>
		/// Filename of the DllOVR wrapper file, which wraps the LibOvr.lib in a dll.
		/// </summary>
		public const string DllOvrDll = "LibOVRRT32_1.dll";

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
		/// <param name="library">Pointer to the previously loaded Oculus runtime module.</param>
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
		[DllImport("kernel32", SetLastError=true, CharSet = CharSet.Ansi)]
		private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

		/// <summary>
		/// Frees a previously loaded dll, from process memory.
		/// </summary>
		/// <param name="hModule">Pointer to the previously loaded library (This pointer comes from a call to LoadLibrary).</param>
		/// <returns>Returns true if the library was successfully freed.</returns>
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
		/// <see cref="DetectResult"/>
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
		/// <see cref="TrackerDesc"/>
		/// <see cref="ovr_GetTrackerCount"/>
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
		/// <see cref="ovr_Destroy"/>
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
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_GetTrackingOriginType"/>
		public override Result SetTrackingOriginType(ovrSession session, TrackingOrigin origin)
		{
			return ovr_SetTrackingOriginType(session, origin);
		}

		/// <summary>
		/// Gets the tracking origin state
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>Returns the TrackingOrigin that was either set by default, or previous set by the application.</returns>
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_SetTrackingOriginType"/>
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
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_GetTrackerPose"/>
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
		/// <see cref="ovr_GetTrackerCount"/>
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
		/// <see cref="ControllerType"/>
		public override Result GetInputState(ovrSession session, ControllerType controllerType, ref InputState inputState)
		{
			return ovr_GetInputState(session, controllerType, ref inputState);
		}
		
		/// <summary>
		/// Returns controller types connected to the system OR'ed together.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>A bitmask of ControllerTypes connected to the system.</returns>
		/// <see cref="ControllerType"/>
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
		/// <see cref="TrackingState"/>
		/// <see cref="ovr_GetEyePoses"/>
		/// <see cref="ovr_GetTimeInSeconds"/>
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
		/// <see cref="ControllerType"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateMirrorTextureDX"/>
		/// <see cref="ovr_CreateMirrorTextureGL"/>
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
		/// <see cref="EyeRenderDesc"/>
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
		/// <see cref="ovr_GetPredictedDisplayTime"/>
		/// <see cref="ViewScaleDesc"/>
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
		/// <see cref="ovr_GetTimeInSeconds"/>
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
		/// <see cref="PoseStatef"/>
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
		/// <see cref="ProjectionModifier"/>
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
		/// <see cref="TimewarpProjectionDesc"/>
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
		/// <see cref="ovr_GetTextureSwapChainLength"/>
		/// <see cref="ovr_GetTextureSwapChainCurrentIndex"/>
		/// <see cref="ovr_GetTextureSwapChainDesc"/>
		/// <see cref="ovr_GetTextureSwapChainBufferDX"/>
		/// <see cref="ovr_DestroyTextureSwapChain"/>
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
		/// <see cref="ovr_GetMirrorTextureBufferDX"/>
		/// <see cref="ovr_DestroyMirrorTexture"/>
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
		/// <see cref="ovr_GetTextureSwapChainLength"/>
		/// <see cref="ovr_GetTextureSwapChainCurrentIndex"/>
		/// <see cref="ovr_GetTextureSwapChainDesc"/>
		/// <see cref="ovr_GetTextureSwapChainBufferGL"/>
		/// <see cref="ovr_DestroyTextureSwapChain"/>
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
		/// <see cref="ovr_GetMirrorTextureBufferGL"/>
		/// <see cref="ovr_DestroyMirrorTexture"/>
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
		/// <see cref="DetectResult"/>
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
		/// <see cref="TrackerDesc"/>
		/// <see cref="ovr_GetTrackerCount"/>
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
		/// <see cref="ovr_Destroy"/>
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
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_GetTrackingOriginType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SetTrackingOriginType", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_SetTrackingOriginType(ovrSession session, TrackingOrigin origin);

		/// <summary>
		/// Gets the tracking origin state
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>Returns the TrackingOrigin that was either set by default, or previous set by the application.</returns>
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_SetTrackingOriginType"/>
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
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_GetTrackerPose"/>
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
		/// <see cref="ovr_GetTrackerCount"/>
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
		/// <see cref="ControllerType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetInputState", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_GetInputState(ovrSession session, ControllerType controllerType, ref InputState inputState);
		
		/// <summary>
		/// Returns controller types connected to the system OR'ed together.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>A bitmask of ControllerTypes connected to the system.</returns>
		/// <see cref="ControllerType"/>
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
		/// <see cref="TrackingState"/>
		/// <see cref="ovr_GetEyePoses"/>
		/// <see cref="ovr_GetTimeInSeconds"/>
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
		/// <see cref="ControllerType"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateMirrorTextureDX"/>
		/// <see cref="ovr_CreateMirrorTextureGL"/>
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
		/// <see cref="EyeRenderDesc"/>
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
		/// <see cref="ovr_GetPredictedDisplayTime"/>
		/// <see cref="ViewScaleDesc"/>
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
		/// <see cref="ovr_GetTimeInSeconds"/>
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
		/// <see cref="PoseStatef"/>
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
		/// <see cref="ProjectionModifier"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovrMatrix4f_Projection", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Matrix4f ovrMatrix4f_Projection(FovPort fov, float znear, float zfar, ProjectionModifier projectionModFlags);

		/// <summary>
		/// Extracts the required data from the result of ovrMatrix4f_Projection.
		/// </summary>
		/// <param name="projection">Specifies the project matrix from which to extract ovrTimewarpProjectionDesc.</param>
		/// <param name="projectionModFlags">A combination of the ProjectionModifier flags.</param>
		/// <returns>Returns the extracted ovrTimewarpProjectionDesc.</returns>
		/// <see cref="TimewarpProjectionDesc"/>
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
		/// <see cref="ovr_GetTextureSwapChainLength"/>
		/// <see cref="ovr_GetTextureSwapChainCurrentIndex"/>
		/// <see cref="ovr_GetTextureSwapChainDesc"/>
		/// <see cref="ovr_GetTextureSwapChainBufferDX"/>
		/// <see cref="ovr_DestroyTextureSwapChain"/>
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
		/// <see cref="ovr_GetMirrorTextureBufferDX"/>
		/// <see cref="ovr_DestroyMirrorTexture"/>
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
		/// <see cref="ovr_GetTextureSwapChainLength"/>
		/// <see cref="ovr_GetTextureSwapChainCurrentIndex"/>
		/// <see cref="ovr_GetTextureSwapChainDesc"/>
		/// <see cref="ovr_GetTextureSwapChainBufferGL"/>
		/// <see cref="ovr_DestroyTextureSwapChain"/>
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
		/// <see cref="ovr_GetMirrorTextureBufferGL"/>
		/// <see cref="ovr_DestroyMirrorTexture"/>
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

	/// <summary>
	/// Raw interface to LibOVR methods.
	/// 
	/// It's not recommended that you use this class directly, unless you want to create your own set of .NET wrapper classes.
	/// Instead, use the Oculus and Hmd classes, which take care of the marshalling of arguments.
	/// </summary>
    public class OVR64	:OVRBase
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
		/// <param name="library">Pointer to the previously loaded Oculus runtime module.</param>
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
		[DllImport("kernel32", SetLastError=true, CharSet = CharSet.Ansi)]
		private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

		/// <summary>
		/// Frees a previously loaded dll, from process memory.
		/// </summary>
		/// <param name="hModule">Pointer to the previously loaded library (This pointer comes from a call to LoadLibrary).</param>
		/// <returns>Returns true if the library was successfully freed.</returns>
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
		/// <see cref="DetectResult"/>
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
		/// <see cref="TrackerDesc"/>
		/// <see cref="ovr_GetTrackerCount"/>
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
		/// <see cref="ovr_Destroy"/>
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
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_GetTrackingOriginType"/>
		public override Result SetTrackingOriginType(ovrSession session, TrackingOrigin origin)
		{
			return ovr_SetTrackingOriginType(session, origin);
		}

		/// <summary>
		/// Gets the tracking origin state
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>Returns the TrackingOrigin that was either set by default, or previous set by the application.</returns>
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_SetTrackingOriginType"/>
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
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_GetTrackerPose"/>
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
		/// <see cref="ovr_GetTrackerCount"/>
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
		/// <see cref="ControllerType"/>
		public override Result GetInputState(ovrSession session, ControllerType controllerType, ref InputState inputState)
		{
			return ovr_GetInputState(session, controllerType, ref inputState);
		}
		
		/// <summary>
		/// Returns controller types connected to the system OR'ed together.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>A bitmask of ControllerTypes connected to the system.</returns>
		/// <see cref="ControllerType"/>
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
		/// <see cref="TrackingState"/>
		/// <see cref="ovr_GetEyePoses"/>
		/// <see cref="ovr_GetTimeInSeconds"/>
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
		/// <see cref="ControllerType"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateMirrorTextureDX"/>
		/// <see cref="ovr_CreateMirrorTextureGL"/>
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
		/// <see cref="EyeRenderDesc"/>
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
		/// <see cref="ovr_GetPredictedDisplayTime"/>
		/// <see cref="ViewScaleDesc"/>
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
		/// <see cref="ovr_GetTimeInSeconds"/>
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
		/// <see cref="PoseStatef"/>
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
		/// <see cref="ProjectionModifier"/>
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
		/// <see cref="TimewarpProjectionDesc"/>
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
		/// <see cref="ovr_GetTextureSwapChainLength"/>
		/// <see cref="ovr_GetTextureSwapChainCurrentIndex"/>
		/// <see cref="ovr_GetTextureSwapChainDesc"/>
		/// <see cref="ovr_GetTextureSwapChainBufferDX"/>
		/// <see cref="ovr_DestroyTextureSwapChain"/>
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
		/// <see cref="ovr_GetMirrorTextureBufferDX"/>
		/// <see cref="ovr_DestroyMirrorTexture"/>
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
		/// <see cref="ovr_GetTextureSwapChainLength"/>
		/// <see cref="ovr_GetTextureSwapChainCurrentIndex"/>
		/// <see cref="ovr_GetTextureSwapChainDesc"/>
		/// <see cref="ovr_GetTextureSwapChainBufferGL"/>
		/// <see cref="ovr_DestroyTextureSwapChain"/>
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
		/// <see cref="ovr_GetMirrorTextureBufferGL"/>
		/// <see cref="ovr_DestroyMirrorTexture"/>
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
		/// <see cref="DetectResult"/>
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
		/// <see cref="TrackerDesc"/>
		/// <see cref="ovr_GetTrackerCount"/>
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
		/// <see cref="ovr_Destroy"/>
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
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_GetTrackingOriginType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SetTrackingOriginType", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_SetTrackingOriginType(ovrSession session, TrackingOrigin origin);

		/// <summary>
		/// Gets the tracking origin state
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>Returns the TrackingOrigin that was either set by default, or previous set by the application.</returns>
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_SetTrackingOriginType"/>
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
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_GetTrackerPose"/>
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
		/// <see cref="ovr_GetTrackerCount"/>
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
		/// <see cref="ControllerType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetInputState", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Result ovr_GetInputState(ovrSession session, ControllerType controllerType, ref InputState inputState);
		
		/// <summary>
		/// Returns controller types connected to the system OR'ed together.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>A bitmask of ControllerTypes connected to the system.</returns>
		/// <see cref="ControllerType"/>
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
		/// <see cref="TrackingState"/>
		/// <see cref="ovr_GetEyePoses"/>
		/// <see cref="ovr_GetTimeInSeconds"/>
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
		/// <see cref="ControllerType"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
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
		/// <see cref="ovr_CreateMirrorTextureDX"/>
		/// <see cref="ovr_CreateMirrorTextureGL"/>
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
		/// <see cref="EyeRenderDesc"/>
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
		/// <see cref="ovr_GetPredictedDisplayTime"/>
		/// <see cref="ViewScaleDesc"/>
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
		/// <see cref="ovr_GetTimeInSeconds"/>
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
		/// <see cref="PoseStatef"/>
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
		/// <see cref="ProjectionModifier"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovrMatrix4f_Projection", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		private static extern Matrix4f ovrMatrix4f_Projection(FovPort fov, float znear, float zfar, ProjectionModifier projectionModFlags);

		/// <summary>
		/// Extracts the required data from the result of ovrMatrix4f_Projection.
		/// </summary>
		/// <param name="projection">Specifies the project matrix from which to extract ovrTimewarpProjectionDesc.</param>
		/// <param name="projectionModFlags">A combination of the ProjectionModifier flags.</param>
		/// <returns>Returns the extracted ovrTimewarpProjectionDesc.</returns>
		/// <see cref="TimewarpProjectionDesc"/>
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
		/// <see cref="ovr_GetTextureSwapChainLength"/>
		/// <see cref="ovr_GetTextureSwapChainCurrentIndex"/>
		/// <see cref="ovr_GetTextureSwapChainDesc"/>
		/// <see cref="ovr_GetTextureSwapChainBufferDX"/>
		/// <see cref="ovr_DestroyTextureSwapChain"/>
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
		/// <see cref="ovr_GetMirrorTextureBufferDX"/>
		/// <see cref="ovr_DestroyMirrorTexture"/>
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
		/// <see cref="ovr_GetTextureSwapChainLength"/>
		/// <see cref="ovr_GetTextureSwapChainCurrentIndex"/>
		/// <see cref="ovr_GetTextureSwapChainDesc"/>
		/// <see cref="ovr_GetTextureSwapChainBufferGL"/>
		/// <see cref="ovr_DestroyTextureSwapChain"/>
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
		/// <see cref="ovr_GetMirrorTextureBufferGL"/>
		/// <see cref="ovr_DestroyMirrorTexture"/>
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

/*
	/// <summary>
	/// Raw interface to LibOVR methods.
	/// 
	/// It's not recommended that you use this class directly, unless you want to create your own set of .NET wrapper classes.
	/// Instead, use the Oculus and Hmd classes, which take care of the marshalling of arguments.
	/// </summary>
    public class OVRUnsafe	:OVRTypes
    {
		/// <summary>
		/// Filename of the DllOVR wrapper file, which wraps the LibOvr.lib in a dll.
		/// </summary>
		public const string DllOvrDll = "DllOVR.dll";

		// The Oculus SDK is full of missing comments.
		// Ignore warnings regarding missing comments, in this class.
		#pragma warning disable 1591

		#region OculusWrap library helper methods
		/// <summary>
		/// Loads a dll into process memory.
		/// </summary>
		/// <param name="lpFileName">Filename to load.</param>
		/// <returns>Pointer to the loaded library.</returns>
		/// <remarks>
		/// This method is used to load the DllOVR.dll into memory, before calling any of it's DllImported methods.
		/// 
		/// This is done to allow loading an x86 version of the DllOvr.dll, for an x86 process, or an x64 version of it, 
		/// for an x64 process.
		/// </remarks>
		[DllImport("kernel32", SetLastError=true, CharSet = CharSet.Ansi)]
		public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

		/// <summary>
		/// Frees a previously loaded dll, from process memory.
		/// </summary>
		/// <param name="hModule">Pointer to the previously loaded library (This pointer comes from a call to LoadLibrary).</param>
		/// <returns>Returns true if the library was successfully freed.</returns>
		[DllImport("kernel32.dll", SetLastError=true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FreeLibrary(IntPtr hModule);
		#endregion

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
		/// <see cref="DetectResult"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_Detect", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern DetectResult ovr_Detect(int timeoutMilliseconds);

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
		public static extern Result ovr_Initialize(InitParams parameters=null);

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
		public static extern void ovr_GetLastErrorInfo(out ErrorInfo errorInfo);

		/// <summary>
		/// Returns version string representing libOVR version. Static, so
		/// string remains valid for app lifespan
		/// </summary>
		/// <remarks>
		/// Use Marshal.PtrToStringAnsi() to retrieve version string.
		/// </remarks>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetVersionString", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern IntPtr ovr_GetVersionString();

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
		public static extern int ovr_TraceMessage(LogLevel level, string message);

		/// <summary>
		/// Shuts down all Oculus functionality.
		/// </summary>
		/// <remarks>
		/// No API functions may be called after ovr_Shutdown except ovr_Initialize.
		/// </remarks>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_Shutdown", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern void ovr_Shutdown();

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
		public static extern void ovr_GetHmdDesc32(out HmdDesc result, ovrSession session);

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
		public static extern void ovr_GetHmdDesc64(out HmdDesc64 result, ovrSession session);

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
		public static extern uint ovr_GetTrackerCount(ovrSession session);

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
		/// <see cref="TrackerDesc"/>
		/// <see cref="ovr_GetTrackerCount"/>
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTrackerDesc", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern TrackerDesc ovr_GetTrackerDesc(ovrSession session, uint trackerDescIndex);

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
		/// <see cref="ovr_Destroy"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_Create", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_Create(ref ovrSession session, ref GraphicsLuid pLuid);

		/// <summary>
		/// Destroys the HMD.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_Destroy", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern void ovr_Destroy(ovrSession session);

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
		public static extern Result ovr_GetSessionStatus(ovrSession session, ref SessionStatus sessionStatus);

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
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_GetTrackingOriginType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SetTrackingOriginType", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_SetTrackingOriginType(ovrSession session, TrackingOrigin origin);

		/// <summary>
		/// Gets the tracking origin state
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>Returns the TrackingOrigin that was either set by default, or previous set by the application.</returns>
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_SetTrackingOriginType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTrackingOriginType", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern TrackingOrigin ovr_GetTrackingOriginType(ovrSession session);

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
		/// <see cref="TrackingOrigin"/>
		/// <see cref="ovr_GetTrackerPose"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_RecenterTrackingOrigin", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_RecenterTrackingOrigin(ovrSession session);
		
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
		public static extern void ovr_ClearShouldRecenterFlag(ovrSession session);

		/// <summary>
		/// Returns the ovrTrackerPose for the given sensor.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="trackerPoseIndex">Index of the sensor being requested.</param>
		/// <returns>
		/// Returns the requested ovrTrackerPose. An empty ovrTrackerPose will be returned if trackerPoseIndex is out of range.
		/// </returns>
		/// <see cref="ovr_GetTrackerCount"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTrackerPose", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern TrackerPose ovr_GetTrackerPose(ovrSession session, uint trackerPoseIndex);

		/// <summary>
		/// Returns the most recent input state for controllers, without positional tracking info.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="controllerType">Specifies which controller the input will be returned for.</param>
		/// <param name="inputState">Input state that will be filled in.</param>
		/// <returns>Returns Result.Success if the new state was successfully obtained.</returns>
		/// <see cref="ControllerType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetInputState", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_GetInputState(ovrSession session, ControllerType controllerType, ref InputState inputState);
		
		/// <summary>
		/// Returns controller types connected to the system OR'ed together.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <returns>A bitmask of ControllerTypes connected to the system.</returns>
		/// <see cref="ControllerType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetConnectedControllerTypes", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern ControllerType ovr_GetConnectedControllerTypes(ovrSession session);

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
		/// <see cref="TrackingState"/>
		/// <see cref="ovr_GetEyePoses"/>
		/// <see cref="ovr_GetTimeInSeconds"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTrackingState", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern void ovr_GetTrackingState(out TrackingState result, ovrSession session, double absTime, ovrBool latencyMarker);

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
		/// <see cref="ControllerType"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SetControllerVibration", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_SetControllerVibration(ovrSession session, ControllerType controllerType, float frequency, float amplitude);

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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTextureSwapChainLength", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_GetTextureSwapChainLength(ovrSession session, ovrTextureSwapChain chain, out int out_Length);

		/// <summary>
		/// Gets the current index in an ovrTextureSwapChain.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain for which the index should be retrieved.</param>
		/// <param name="out_Index">Returns the current (free) index in specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTextureSwapChainCurrentIndex", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_GetTextureSwapChainCurrentIndex(ovrSession session, ovrTextureSwapChain chain, out int out_Index);

		/// <summary>
		/// Gets the description of the buffers in an ovrTextureSwapChain
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain for which the description should be retrieved.</param>
		/// <param name="out_Desc">Returns the description of the specified chain.</param>
		/// <returns>Returns an ovrResult for which the return code is negative upon error. </returns>
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTextureSwapChainDesc", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_GetTextureSwapChainDesc(ovrSession session, ovrTextureSwapChain chain, [In, Out] ref TextureSwapChainDesc out_Desc);

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
		/// <see cref="ovr_CreateTextureSwapChainDX"/>
		/// <see cref="ovr_CreateTextureSwapChainGL"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_CommitTextureSwapChain", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_CommitTextureSwapChain(ovrSession session, ovrTextureSwapChain chain);

		/// <summary>
		/// Destroys an ovrTextureSwapChain and frees all the resources associated with it.
		/// </summary>
		/// <param name="session">Specifies an ovrSession previously returned by ovr_Create.</param>
		/// <param name="chain">Specifies the ovrTextureSwapChain to destroy. If it is null then this function has no effect.</param>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_DestroyTextureSwapChain", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern void ovr_DestroyTextureSwapChain(ovrSession session, ovrTextureSwapChain chain);

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
		/// <see cref="ovr_CreateMirrorTextureDX"/>
		/// <see cref="ovr_CreateMirrorTextureGL"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_DestroyMirrorTexture", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern void ovr_DestroyMirrorTexture(ovrSession session, ovrMirrorTexture mirrorTexture);

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
		public static extern Sizei ovr_GetFovTextureSize(ovrSession session, EyeType eye, FovPort fov, float pixelsPerDisplayPixel);

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
		/// <see cref="EyeRenderDesc"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetRenderDesc", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern EyeRenderDesc ovr_GetRenderDesc(ovrSession session, EyeType eyeType, FovPort fov);

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
		/// <see cref="ovr_GetPredictedDisplayTime"/>
		/// <see cref="ViewScaleDesc"/>
		/// <see cref="LayerHeader"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_SubmitFrame", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_SubmitFrame(ovrSession session, Int64 frameIndex, IntPtr viewScaleDesc, IntPtr layerPtrList, uint layerCount);

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
		/// <see cref="ovr_GetTimeInSeconds"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetPredictedDisplayTime", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern double ovr_GetPredictedDisplayTime(ovrSession session, Int64 frameIndex);

		/// <summary>
		/// Returns global, absolute high-resolution time in seconds. 
		///
		/// The time frame of reference for this function is not specified and should not be
		/// depended upon.
		/// </summary>
		/// <returns>
		/// Returns seconds as a floating point value.
		/// </returns>
		/// <see cref="PoseStatef"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_GetTimeInSeconds", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern double ovr_GetTimeInSeconds();

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
		public static extern ovrBool ovr_GetBool(ovrSession session, string propertyName, ovrBool defaultVal);

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
		public static extern ovrBool ovr_SetBool(ovrSession session, string propertyName, ovrBool value);

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
		public static extern int ovr_GetInt(ovrSession session, string propertyName, int defaultVal);

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
		public static extern ovrBool ovr_SetInt(ovrSession session, string propertyName, int value);

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
		public static extern float ovr_GetFloat(ovrSession session, string propertyName, float defaultVal);

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
		public static extern ovrBool ovr_SetFloat(ovrSession session, string propertyName, float value);

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
		public static extern uint ovr_GetFloatArray(ovrSession session, string propertyName, [MarshalAs(UnmanagedType.LPArray)] float[] values, uint valuesCapacity);

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
		public static extern ovrBool ovr_SetFloatArray(ovrSession session, string propertyName, [MarshalAs(UnmanagedType.LPArray)] float[] values, uint valuesSize);

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
		public static extern IntPtr ovr_GetString(ovrSession session, string propertyName, string defaultVal);

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
		public static extern ovrBool ovr_SetString(IntPtr session, string propertyName,string value);

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
		/// <see cref="ProjectionModifier"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovrMatrix4f_Projection", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Matrix4f ovrMatrix4f_Projection(FovPort fov, float znear, float zfar, ProjectionModifier projectionModFlags);

		/// <summary>
		/// Extracts the required data from the result of ovrMatrix4f_Projection.
		/// </summary>
		/// <param name="projection">Specifies the project matrix from which to extract ovrTimewarpProjectionDesc.</param>
		/// <param name="projectionModFlags">A combination of the ProjectionModifier flags.</param>
		/// <returns>Returns the extracted ovrTimewarpProjectionDesc.</returns>
		/// <see cref="TimewarpProjectionDesc"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovrTimewarpProjectionDesc_FromProjection", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern TimewarpProjectionDesc ovrTimewarpProjectionDesc_FromProjection(Matrix4f projection, ProjectionModifier projectionModFlags);

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
		public static extern Matrix4f ovrMatrix4f_OrthoSubProjection(Matrix4f projection, Vector2f orthoScale, float orthoDistance, float hmdToEyeOffsetX);

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
		public static extern void ovr_CalcEyePoses(Posef headPose, [MarshalAs(UnmanagedType.LPArray, SizeConst=2)] Vector3f[] hmdToEyeOffset, IntPtr outEyePoses);

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
		public static extern void ovr_GetEyePoses(ovrSession session, Int64 frameIndex, ovrBool latencyMarker, [MarshalAs(UnmanagedType.LPArray, SizeConst=2)] Vector3f[] hmdToEyeOffset, IntPtr outEyePoses, IntPtr outSensorSampleTime);

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
		public static extern void ovrPosef_FlipHandedness(ref Posef inPose, [In, Out] ref Posef outPose);

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
		/// <see cref="ovr_GetTextureSwapChainLength"/>
		/// <see cref="ovr_GetTextureSwapChainCurrentIndex"/>
		/// <see cref="ovr_GetTextureSwapChainDesc"/>
		/// <see cref="ovr_GetTextureSwapChainBufferDX"/>
		/// <see cref="ovr_DestroyTextureSwapChain"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_CreateTextureSwapChainDX", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_CreateTextureSwapChainDX(ovrSession session, IntPtr d3dPtr, ref TextureSwapChainDesc desc, [In, Out] ref ovrTextureSwapChain out_TextureSwapChain);

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
		public static extern Result ovr_GetTextureSwapChainBufferDX(ovrSession session, ovrTextureSwapChain chain, int index, Guid iid, [In, Out] ref IntPtr out_Buffer);

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
		/// <see cref="ovr_GetMirrorTextureBufferDX"/>
		/// <see cref="ovr_DestroyMirrorTexture"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_CreateMirrorTextureDX", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_CreateMirrorTextureDX(ovrSession session, IntPtr d3dPtr, ref MirrorTextureDesc desc, [In, Out] ref ovrMirrorTexture out_MirrorTexture);

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
		public static extern Result ovr_GetMirrorTextureBufferDX(ovrSession session, ovrMirrorTexture mirrorTexture, Guid iid, [In, Out] ref IntPtr out_Buffer);

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
		/// <see cref="ovr_GetTextureSwapChainLength"/>
		/// <see cref="ovr_GetTextureSwapChainCurrentIndex"/>
		/// <see cref="ovr_GetTextureSwapChainDesc"/>
		/// <see cref="ovr_GetTextureSwapChainBufferGL"/>
		/// <see cref="ovr_DestroyTextureSwapChain"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_CreateTextureSwapChainGL", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result  ovr_CreateTextureSwapChainGL(ovrSession session, TextureSwapChainDesc desc, [Out] out ovrTextureSwapChain out_TextureSwapChain);

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
		public static extern Result ovr_GetTextureSwapChainBufferGL(ovrSession session, ovrTextureSwapChain chain, int index, [Out] out uint out_TexId);

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
		/// <see cref="ovr_GetMirrorTextureBufferGL"/>
		/// <see cref="ovr_DestroyMirrorTexture"/>
		[SuppressUnmanagedCodeSecurity]
		[DllImport(DllOvrDll, EntryPoint="ovr_CreateMirrorTextureGL", SetLastError=false, CallingConvention=CallingConvention.Cdecl)]
		public static extern Result ovr_CreateMirrorTextureGL(ovrSession session, MirrorTextureDesc desc, [Out] out ovrMirrorTexture out_MirrorTexture);

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
		public static extern Result ovr_GetMirrorTextureBufferGL(ovrSession session, ovrMirrorTexture mirrorTexture, [Out] out uint out_TexId);
		#endregion

		#pragma warning restore 1591
	}
*/
}
