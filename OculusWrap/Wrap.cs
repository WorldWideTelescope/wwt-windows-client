using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using ovrBool = System.Byte;

namespace OculusWrap
{
	/// <summary>
	/// Provides access to methods that are independent of an HMD, as well as a method used to create a new Hmd object.
	/// </summary>
	public class Wrap	:IDisposable
	{
		/// <summary>
		/// Creates a new Wrap instance.
		/// </summary>
		/// <param name="useUnsafeImplementation">
		/// When set to true, the Oculus runtime will be loaded using the unsafe implementation.
		/// 
		/// The unsafe implementation provides a faster execution, by bypassing the managed security checks that are normally
		/// performed when transitioning from managed to unmanaged code. 
		/// 
		/// Use the unsafe implementation with extreme care. Incorrect use can create security weaknesses.
		/// </param>
		/// <see cref="https://msdn.microsoft.com/en-us/library/62a3eyh4(v=vs.100).aspx"/>
		public Wrap(bool useUnsafeImplementation=false)
		{
			if(useUnsafeImplementation)
			{
				if(Environment.Is64BitProcess)
					OVR = new OVR64Unsafe();
				else
					OVR = new OVR32Unsafe();
			}
			else
			{
				if(Environment.Is64BitProcess)
					OVR = new OVR64();
				else
					OVR = new OVR32();
			}
		}

		/// <summary>
		/// Cleans up unmanaged resources.
		/// </summary>
		~Wrap()
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

			// Ensure that all created HMDs have been disposed.
			foreach(Hmd hmd in CreatedHmds)
			{
				if(!hmd.Disposed)
					hmd.Dispose();
			}

			CreatedHmds.Clear();

			if(Initialized)
				Shutdown();

			// Deallocate unmanaged memory again.
			FreeHGlobal(ref m_eyePosesPtr);

			OVR.Dispose();
			OVR = null;

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

		/// <summary>
		/// Interface to Oculus runtime methods.
		/// </summary>
		private OVRBase OVR
		{
			get;
			set;
		}

		#region Public methods
		/// <summary>
		/// Detects Oculus Runtime and Device Status
		///
		/// Checks for Oculus Runtime and Oculus HMD device status without loading the LibOVRRT
		/// shared library.  This may be called before ovr_Initialize() to help decide whether or
		/// not to initialize LibOVR.
		/// </summary>
		/// <param name="timeoutMsec">Specifies a timeout to wait for HMD to be attached or 0 to poll.</param>
		/// <returns>Returns a DetectResult object indicating the result of detection.</returns>
		/// <see cref="OVRTypes.DetectResult"/>
		public OVRTypes.DetectResult Detect(int timeoutMsec)
		{
			return OVR.Detect(timeoutMsec);
		}

		/// <summary>
		/// Initializes all Oculus functionality.
		/// </summary>
		/// <param name="initializationParameters">
		/// Initialization parameters to pass to the ovr_Initialize call.
		/// </param>
		/// <remarks>
		/// Library init/shutdown, must be called around all other OVR code.
		/// No other functions calls besides ovr_InitializeRenderingShim are allowed
		/// before ovr_Initialize succeeds or after ovr_Shutdown.
		/// </remarks>
		public bool Initialize(OVRTypes.InitParams initializationParameters=null)
		{
			if(Initialized)
				throw new InvalidOperationException("The Oculus wrapper has already been initialized.");
/*
			// Ensure that the DllOvr.dll is loaded, using the bitness matching that of the current process.
			LoadDllOvr();
*/
			OVRTypes.Result success = OVR.Initialize(initializationParameters);
			if(success < OVRTypes.Result.Success)
				return false;

			Initialized = true;

			return true;
		}

		/// <summary>
		/// Shuts down all Oculus functionality.
		/// </summary>
		public bool Shutdown()
		{
			if(!Initialized && !RenderingShimInitialized)
				return true;

			OVR.Shutdown();
/*
			// Unload previously loaded DllOVr.dll.
			UnloadDllOvr();
*/
			Initialized					= false;
			RenderingShimInitialized	= false;

			return true;
		}

		/// <summary>
		/// Returns version string representing libOVR version. Static, so
		/// string remains valid for app lifespan
		/// </summary>
		/// <remarks>
		/// Use Marshal.PtrToStringAnsi() to retrieve version string.
		/// </remarks>
		public string GetVersionString()
		{
			IntPtr versionPtr = OVR.GetVersionString();
			string version = Marshal.PtrToStringAnsi(versionPtr);
			return version;
		}

		/// <summary>
		/// Send a message string to the system tracing mechanism if enabled (currently Event Tracing for Windows)
		/// </summary>
		/// <param name="level">
		/// One of the ovrLogLevel constants.
		/// </param>
		/// <param name="message">
		/// A string.
		/// </param>
		/// <returns>
		/// Returns the length of the message, or -1 if message is too large
		/// </returns>
		public int TraceMessage(OVRTypes.LogLevel level, string message)
		{
			return OVR.TraceMessage(level, message);
		}

		/// <summary>
		/// Creates a handle to an HMD.
		/// 
		/// Upon success the returned Hmd must be eventually freed with Dispose() when it is no longer needed.
		/// </summary>
		/// <param name="graphicsLuid">
		/// Provides a system specific graphics adapter identifier that locates which
		/// graphics adapter has the HMD attached. This must match the adapter used by the application
		/// or no rendering output will be possible. This is important for stability on multi-adapter systems. An
		/// application that simply chooses the default adapter will not run reliably on multi-adapter systems.
		/// </param>
		public Hmd Hmd_Create(out OVRTypes.GraphicsLuid graphicsLuid)
		{
			IntPtr	hmdPtr	= IntPtr.Zero;
			graphicsLuid	= new OVRTypes.GraphicsLuid();
			OVRTypes.Result result = OVR.Create(ref hmdPtr, ref graphicsLuid);
			if(result < OVRTypes.Result.Success)
				return null;

			Hmd hmd = new Hmd(OVR, hmdPtr);

			// Ensure that this created HMD is disposed, when this Wrap class is being disposed.
			CreatedHmds.Add(hmd);

			return hmd;
		}

		/// <summary>
		/// Returns last error for HMD state. Returns null for no error.
		/// 
		/// String is valid until next call or GetLastError or HMD is destroyed.
		/// Pass null hmd to get global errors (during create etc).
		/// </summary>
		public OVRBase.ErrorInfo GetLastError()
		{
			OVRTypes.ErrorInfo errorInfo;

			OVR.GetLastErrorInfo(out errorInfo);

			return errorInfo;
		}

		/// <summary>
		/// Used to generate projection from ovrEyeDesc::Fov.
		/// </summary>
		public OVRBase.Matrix4f Matrix4f_Projection(OVRBase.FovPort fov, float znear, float zfar, OVRBase.ProjectionModifier projectionModifier)
		{
			return OVR.Matrix4f_Projection(fov, znear, zfar, projectionModifier);
		}

		/// <summary>
		/// Extracts the required data from the result of ovrMatrix4f_Projection.
		/// </summary>
		/// <param name="projection">Specifies the project matrix from which to extract ovrTimewarpProjectionDesc.</param>
		/// <param name="projectionModFlags">A combination of the ProjectionModifier flags.</param>
		/// <returns>Returns the extracted ovrTimewarpProjectionDesc.</returns>
		/// <see cref="OVRTypes.TimewarpProjectionDesc"/>
		public OVRBase.TimewarpProjectionDesc TimewarpProjectionDesc_FromProjection(OVRBase.Matrix4f projection, OVRBase.ProjectionModifier projectionModFlags)
		{
			return OVR.TimewarpProjectionDesc_FromProjection(projection, projectionModFlags);
		}

		/// <summary>
		/// Used for 2D rendering, Y is down
		/// orthoScale = 1.0f / pixelsPerTanAngleAtCenter
		/// orthoDistance = distance from camera, such as 0.8m
		/// </summary>
		public OVRBase.Matrix4f Matrix4f_OrthoSubProjection(OVRBase.Matrix4f projection, OVRBase.Vector2f orthoScale, float orthoDistance, float eyeViewAdjustX)
		{
			return OVR.Matrix4f_OrthoSubProjection(projection, orthoScale, orthoDistance, eyeViewAdjustX);
		}

		/// <summary>
		/// Computes offset eye poses based on headPose returned by ovrTrackingState.
		/// </summary>
		/// <param name="headPose">
		/// Indicates the HMD position and orientation to use for the calculation.
		/// </param>
		/// <param name="hmdToEyeViewOffset">
		/// Can be ovrEyeRenderDesc.HmdToEyeViewOffset returned from 
		/// ovrHmd_GetRenderDesc. For monoscopic rendering, use a vector that is the average 
		/// of the two vectors for both eyes.
		/// </param>
		/// <param name="outEyePoses">
		/// If outEyePoses are used for rendering, they should be passed to 
		/// SubmitFrame in LayerEyeFov.RenderPose or LayerEyeFovDepth.RenderPose.
		/// </param>
		public void CalcEyePoses(OVRBase.Posef headPose, OVRBase.Vector3f[] hmdToEyeViewOffset, ref OVRBase.Posef[] outEyePoses)
		{
			if(hmdToEyeViewOffset.Length != 2)
				throw new ArgumentException("The hmdToEyeViewOffset argument must contain 2 elements.", "hmdToEyeViewOffset");
			if(outEyePoses.Length != 2)
				throw new ArgumentException("The outEyePoses argument must contain 2 elements.", "outEyePoses");

			if(m_eyePosesPtr == IntPtr.Zero)
			{
				// Allocate and copy managed struct to unmanaged memory.
				m_poseFSize		= Marshal.SizeOf(typeof(OVRBase.Posef));
				m_eyePosesPtr	= Marshal.AllocHGlobal(m_poseFSize*2);
			}

			OVR.CalcEyePoses(headPose, hmdToEyeViewOffset, m_eyePosesPtr);

			outEyePoses[0] = (OVRBase.Posef) Marshal.PtrToStructure(m_eyePosesPtr, typeof(OVRBase.Posef));
			outEyePoses[1] = (OVRBase.Posef) Marshal.PtrToStructure(m_eyePosesPtr+m_poseFSize, typeof(OVRBase.Posef));
		}

		/// <summary>
		/// Returns global, absolute high-resolution time in seconds. This is the same
		/// value as used in sensor messages.
		/// </summary>
		public double GetTimeInSeconds()
		{
			return OVR.GetTimeInSeconds();
		}

		/// <summary>
		/// Tracking poses provided by the SDK come in a right-handed coordinate system. If an application
		/// is passing in ovrProjection_LeftHanded into Matrix4f_Projection, then it should also use
		/// this function to flip the HMD tracking poses to be left-handed.
		///
		/// While this utility function is intended to convert a left-handed ovrPosef into a right-handed
		/// coordinate system, it will also work for converting right-handed to left-handed since the
		/// flip operation is the same for both cases.
		/// </summary>
		/// <param name="pose">Pose that is right-handed</param>
		public OVRBase.Posef Posef_FlipHandedness(OVRBase.Posef pose)
		{
			OVRTypes.Posef inputOutputPose	= pose;
			OVR.Posef_FlipHandedness(ref inputOutputPose, ref inputOutputPose);

			return inputOutputPose;
		}

/*
		/// <summary>
		/// Waits until the specified absolute time.
		/// </summary>
		/// <remarks>
		/// This function may be removed in a future version.
		/// </remarks>
		public double WaitTillTime(double absTime)
		{
			return OVRBase.WaitTillTime(absTime);
		}
*/
		#endregion

		#region Public properties
		/// <summary>
		/// Determines if InitializeRenderingShim() has been called, without calling Shutdown().
		/// </summary>
		public bool RenderingShimInitialized
		{
			get;
			private set;
		}

		/// <summary>
		/// Determines if Initialize() has been called, without calling Shutdown().
		/// </summary>
		public bool Initialized
		{
			get;
			private set;
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

		/*
		#region Private methods
		/// <summary>
		/// This method is used to load the DllOVR.dll into memory, before calling any of it's DllImported methods.
		/// 
		/// This is done to allow loading an x86 version of the DllOvr.dll, for an x86 process, or an x64 version of it, 
		/// for an x64 process.
		/// </summary>
		private void LoadDllOvr()
		{
			if(DllOvrPtr == IntPtr.Zero)
			{
				// Retrieve the folder of the OculusWrap.dll.
				string executingAssemblyFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

				string subfolder;

				if(Environment.Is64BitProcess)
					subfolder = "x64";
				else
					subfolder = "x86";

				string filename = Path.Combine(executingAssemblyFolder, subfolder, OVR. DllOvrDll);

				// Check that the DllOvrDll file exists.
				bool exists = File.Exists(filename);
				if(!exists)
					throw new DllNotFoundException("Unable to load the file \""+filename+"\", the file wasn't found.");

				DllOvrPtr = OVR.LoadLibrary(filename);
				if(DllOvrPtr == IntPtr.Zero)
				{
					int win32Error = Marshal.GetLastWin32Error();
					throw new Win32Exception(win32Error, "Unable to load the file \""+filename+"\", LoadLibrary reported error code: "+win32Error+".");
				}
			}
		}

		/// <summary>
		/// Frees previously loaded DllOvr.dll, from process memory.
		/// </summary>
		private void UnloadDllOvr()
		{
			if(DllOvrPtr != IntPtr.Zero)
			{
				bool success = OVR.FreeLibrary(DllOvrPtr);
				if(success)
					DllOvrPtr = IntPtr.Zero;
			}
		}
		#endregion
		*/
		#region Private properties
		/// <summary>
		/// Pointer to the DllOvr module, after it has been loaded, using a call to OVR.LoadLibrary.
		/// </summary>
		private IntPtr DllOvrPtr
		{
			get;
			set;
		}

		/// <summary>
		/// Set of created HMDs.
		/// </summary>
		/// <remarks>
		/// This set is used to ensure that all created HMDs are also disposed.
		/// </remarks>
		private HashSet<Hmd> CreatedHmds
		{
			get
			{
				return m_createdHmds;
			}
		}
		#endregion

		#region Private fields
		private HashSet<Hmd> m_createdHmds = new HashSet<Hmd>();
		private IntPtr m_eyePosesPtr = IntPtr.Zero;

		private int m_poseFSize = 0;
		#endregion
	}
}
