using System;
using System.Reflection;


namespace TerraViewer
{
    public enum AlignmentModes
    {
        algAltAz = 0,
        algPolar = 1,
        algGermanPolar = 2,
    }
    
    public enum TelescopeAxes
    {
        axisPrimary = 0,
        axisSecondary = 1,
        axisTertiary = 2,
    }

    public enum PierSide
    {
        pierEast = 0,
        pierWest = 1,
    }

    public enum EquatorialCoordinateType
    {
        equOther = 0,
        equLocalTopocentric = 1,
        equJ2000 = 2,
        equJ2050 = 3,
        equB1950 = 4,
    }

    public enum GuideDirections
    {
        guideNorth = 0,
        guideSouth = 1,
        guideEast = 2,
        guideWest = 3,
    }

    public enum DriveRates
    {
        driveSidereal = 0,
        driveLunar = 1,
        driveSolar = 2,
        driveKing = 3,
    }

 

    class AscomTelescope : IDisposable
    {
        object objScopeLateBound;
        readonly Type objTypeScope;


        public AscomTelescope(string telescopeID)
		{
			// Get Type Information 
            objTypeScope = Type.GetTypeFromProgID(telescopeID);
			
			// Create an instance of the serial object
            objScopeLateBound = Activator.CreateInstance(objTypeScope);
		}

        public static string ChooseTelescope(string telescopeID)
        {
            Type objTypeChooser = Type.GetTypeFromProgID("ASCOM.Utilities.Chooser");
			
			// Create an instance of the serial object
            object objChooserLateBound = Activator.CreateInstance(objTypeChooser);

            return (string)objTypeChooser.InvokeMember("Choose",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objChooserLateBound, new object[] { telescopeID });
          

        }

        public static bool IsPlatformInstalled()
        {
            try
            {
                Type objTypeChooser = Type.GetTypeFromProgID("ASCOM.Utilities.Chooser");
                // Create an instance of the serial object
                object objChooserLateBound = Activator.CreateInstance(objTypeChooser);
                objTypeChooser.InvokeMember("DeviceType",
                BindingFlags.Default | BindingFlags.SetProperty,
                null, objChooserLateBound, new object[] { "Telescope" });
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        #region _Telescope Members

        public void AbortSlew()
        {
            objTypeScope.InvokeMember("AbortSlew",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objScopeLateBound, new object[] { });
        }

        public AlignmentModes AlignmentMode
        {
            get
            {
                return (AlignmentModes)objTypeScope.InvokeMember("AlignmentMode", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public double Altitude
        {
            get
            {
                return (double)objTypeScope.InvokeMember("Altitude", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            } 
        }

        public double ApertureArea
        {
            get
            {
               return (double)objTypeScope.InvokeMember("ApertureArea", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public double ApertureDiameter
        {
            get
            {
                return (double)objTypeScope.InvokeMember("ApertureDiameter", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }   
        }

        public bool AtHome
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("AtHome", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool AtPark
        {
            get
            {
                try
                {
                    return (bool)objTypeScope.InvokeMember("AtPark", BindingFlags.Default | BindingFlags.GetProperty,
                        null, objScopeLateBound, new object[] { });
                }
                catch
                {
                    return false;
                }
            }
        }

        public object AxisRates(TelescopeAxes Axis)
        {
            try
            {
                return objTypeScope.InvokeMember("AxisRates",
                    BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] { (int)Axis });
            }
            catch
            {
                return null;
            }
        }

        public double Azimuth
        {
            get
            {
                return (double)objTypeScope.InvokeMember("Azimuth", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanFindHome
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanFindHome", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanMoveAxis(TelescopeAxes Axis)
        {

            return (bool)objTypeScope.InvokeMember("CanMoveAxis", BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] {(int)Axis });
        }

        public bool CanPark
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanPark", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanPulseGuide
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanPulseGuide", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSetDeclinationRate
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSetDeclinationRate", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSetGuideRates
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSetGuideRates", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSetPark
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSetPark", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSetPierSide
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSetPierSide", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSetRightAscensionRate
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSetRightAscensionRate", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSetTracking
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSetTracking", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSlew
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSlew", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSlewAltAz
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSlewAltAz", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSlewAltAzAsync
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSlewAltAzAsync", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSlewAsync
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSlewAsync", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSync
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSync", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanSyncAltAz
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanSyncAltAz", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool CanUnpark
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("CanUnpark", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public void CommandBlind(string Command, bool Raw)
        {
            objTypeScope.InvokeMember("CommandBlind", BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] { Command, Raw });
        }

        public bool CommandBool(string Command, bool Raw)
        {
            return (bool) objTypeScope.InvokeMember("CommandBool", BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] { Command, Raw });
        }

        public string CommandString(string Command, bool Raw)
        {
            return (string)objTypeScope.InvokeMember("CommandString", BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] { Command, Raw });
        }

        public bool Connected
        {
            get
            {
                var connected = (bool)objTypeScope.InvokeMember("Connected", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
                return connected;
            }
            set
            {
                object[] Parameters = { value };
                objTypeScope.InvokeMember("Connected", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, Parameters);

            }
        }

        public double Declination
        {
            get
            {
                return (double)objTypeScope.InvokeMember("Declination", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public double DeclinationRate
        {
            get
            {
                return (double)objTypeScope.InvokeMember("DeclinationRate", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("DeclinationRate", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public string Description
        {
            get
            {
                return (string)objTypeScope.InvokeMember("Description", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public PierSide DestinationSideOfPier(double rightAscension, double declination)
        {
            return (PierSide)objTypeScope.InvokeMember("DestinationSideOfPier", BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] {rightAscension, declination});
        }

        public bool DoesRefraction
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("DoesRefraction", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("DoesRefraction", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public string DriverInfo
        {
            get
            {
                return (string)objTypeScope.InvokeMember("DriverInfo", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public string DriverVersion
        {
            get
            {
                return (string)objTypeScope.InvokeMember("DriverVersion", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public EquatorialCoordinateType EquatorialSystem
        {
            get
            {
                return (EquatorialCoordinateType)objTypeScope.InvokeMember("EquatorialSystem", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public void FindHome()
        {
            objTypeScope.InvokeMember("FindHome", BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] {  });   
        }

        public double FocalLength
        {
            get
            {
                return (double)objTypeScope.InvokeMember("FocalLength", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public double GuideRateDeclination
        {
            get
            {
                return (double)objTypeScope.InvokeMember("GuideRateDeclination", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("GuideRateDeclination", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public double GuideRateRightAscension
        {
            get
            {
                return (double)objTypeScope.InvokeMember("GuideRateRightAscension", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("GuideRateRightAscension", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public short InterfaceVersion
        {
            get
            {
                return (short)objTypeScope.InvokeMember("InterfaceVersion", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public bool IsPulseGuiding
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("IsPulseGuiding", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public void MoveAxis(TelescopeAxes Axis, double Rate)
        {
            objTypeScope.InvokeMember("MoveAxis", BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] { Axis, Rate });        }

        public string Name
        {
            get
            {
                return (string)objTypeScope.InvokeMember("Name", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public void Park()
        {
            objTypeScope.InvokeMember("Park", BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] {  });        }

        public void PulseGuide(GuideDirections Direction, int Duration)
        {
            objTypeScope.InvokeMember("PulseGuide", BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] { (int)Direction, Duration });  
        }

        public double RightAscension
        {
            get
            {
                return (double)objTypeScope.InvokeMember("rightAscension", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public double RightAscensionRate
        {
            get
            {
                return (double)objTypeScope.InvokeMember("RightAscensionRate", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("RightAscensionRate", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public void SetPark()
        {
            objTypeScope.InvokeMember("SetPark", BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] {  });        }

        public void SetupDialog()
        {
            objTypeScope.InvokeMember("SetupDialog", BindingFlags.Default | BindingFlags.InvokeMethod,
                    null, objScopeLateBound, new object[] {  }); 
        }

        public PierSide SideOfPier
        {
            get
            {
                return (PierSide)objTypeScope.InvokeMember("SideOfPier", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("SideOfPier", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { (int)value });
            }
        }

        public double SiderealTime
        {
            get
            {
                return (double)objTypeScope.InvokeMember("SiderealTime", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public double SiteElevation
        {
            get
            {
                return (double)objTypeScope.InvokeMember("SiteElevation", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("SiteElevation", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public double SiteLatitude
        {
            get
            {
                return (double)objTypeScope.InvokeMember("SiteLatitude", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("SiteLatitude", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public double SiteLongitude
        {
            get
            {
                return (double)objTypeScope.InvokeMember("SiteLongitude", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("SiteLongitude", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public short SlewSettleTime
        {
            get
            {
                return (short)objTypeScope.InvokeMember("SlewSettleTime", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("SlewSettleTime", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public void SlewToAltAz(double azimuth, double altitude)
        {
            objTypeScope.InvokeMember("SlewToAltAz",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objScopeLateBound, new object[] { azimuth, altitude });
        }

        public void SlewToAltAzAsync(double azimuth, double altitude)
        {
            objTypeScope.InvokeMember("SlewToAltAzAsync",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objScopeLateBound, new object[] { azimuth, altitude });
        }

        public void SlewToCoordinates(double rightAscension, double declination)
        {
            Patterns.ActIgnoringExceptions(() => objTypeScope.InvokeMember("SlewToCoordinates",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objScopeLateBound, new object[] {rightAscension, declination}));
        }

        public void SlewToCoordinatesAsync(double rightAscension, double declination)
        {
            Patterns.ActIgnoringExceptions(() => objTypeScope.InvokeMember("SlewToCoordinatesAsync",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objScopeLateBound, new object[] {rightAscension, declination}));
        }

        public void SlewToTarget()
        {
            Patterns.ActIgnoringExceptions(() => objTypeScope.InvokeMember("SlewToTarget",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objScopeLateBound, new object[] {}));
        }

        public void SlewToTargetAsync()
        {
            objTypeScope.InvokeMember("SlewToTargetAsync",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objScopeLateBound, new object[] { });
        }

        public bool Slewing
        {
            get
            {
                return (bool)objTypeScope.InvokeMember("Slewing", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
        }

        public void SyncToAltAz(double azimuth, double altitude)
        {
            objTypeScope.InvokeMember("SyncToAltAz",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objScopeLateBound, new object[] { azimuth, altitude });
        }

        public void SyncToCoordinates(double rightAscension, double declination)
        {
            objTypeScope.InvokeMember("SyncToCoordinates",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objScopeLateBound, new object[] { rightAscension, declination });
        }

        public void SyncToTarget()
        {
            objTypeScope.InvokeMember("SyncToTarget",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objScopeLateBound, new object[] { });
        }

        public double TargetDeclination
        {
            get
            {
                return (double)objTypeScope.InvokeMember("TargetDeclination", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("TargetDeclination", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public double TargetRightAscension
        {
            get
            {
                return (double)objTypeScope.InvokeMember("TargetRightAscension", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("TargetRightAscension", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public bool Tracking
        {
            get
            {
                try
                {
                    return (bool)objTypeScope.InvokeMember("Tracking", BindingFlags.Default | BindingFlags.GetProperty,
                        null, objScopeLateBound, new object[] { });
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                Patterns.ActIgnoringExceptions(() => objTypeScope.InvokeMember("Tracking", BindingFlags.Default | BindingFlags.SetProperty, null, objScopeLateBound, new object[] {value}));
            }
        }

        public DriveRates TrackingRate
        {
            get
            {
                return (DriveRates)objTypeScope.InvokeMember("TrackingRate", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("TrackingRate", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public object TrackingRates
        {
            get
            {
                return objTypeScope.InvokeMember("TrackingRates", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }

        }

        public DateTime UTCDate
        {
            get
            {
                return (DateTime)objTypeScope.InvokeMember("UTCDate", BindingFlags.Default | BindingFlags.GetProperty,
                    null, objScopeLateBound, new object[] { });
            }
            set
            {
                objTypeScope.InvokeMember("UTCDate", BindingFlags.Default | BindingFlags.SetProperty,
                    null, objScopeLateBound, new object[] { value });
            }
        }

        public void Unpark()
        {
            objTypeScope.InvokeMember("Unpark",
                BindingFlags.Default | BindingFlags.InvokeMethod,
                null, objScopeLateBound, new object[] { });
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (this.objScopeLateBound != null)
            {
                objScopeLateBound = null;
            }
        }

        #endregion
    }
}
