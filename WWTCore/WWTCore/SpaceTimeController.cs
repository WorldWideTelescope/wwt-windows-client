using System;
using System.Collections.Generic;
using System.Text;

namespace TerraViewer
{
    public  enum SpaceTimeActions { Slower, Faster, PauseTime, SetRealtimeNow, NextSiderealDay, LastSiderealDay, NextDay, LastDay, NextSiderealYear, LastSidrealYear, NextYear, LastYear };
    public class SpaceTimeController : IScriptable
    {
        static public SpaceTimeController ScriptInterface = new SpaceTimeController();

        static public void UpdateClock()
        {
            if (syncToClock)
            {
                DateTime justNow = SpaceTimeController.MetaNow;

                if (timeRate != 1.0)
                {
                    TimeSpan ts = justNow - last;
                    long ticks = (long)(ts.Ticks * timeRate);
                    offset = offset.Add(new TimeSpan(ticks));
                }
                last = justNow;
                try
                {
                    now = justNow.Add(offset);
                    nowUtc = now.ToUniversalTime();
                }
                catch
                {
                    now = new DateTime(1, 12, 25, 23, 59, 59);
                    offset = now - SpaceTimeController.MetaNow;
                    nowUtc = now.ToUniversalTime();

                }

                if (now.Year > 4000)
                {
                    now = new DateTime(4000, 12, 31, 23, 59, 59);
                    offset = now - SpaceTimeController.MetaNow;
                    nowUtc = now.ToUniversalTime();
                }

                if (now.Year < 1)
                {
                    now = new DateTime(0, 12, 25, 23, 59, 59);
                    offset = now - SpaceTimeController.MetaNow;
                    nowUtc = now.ToUniversalTime();
                }

            }
        }

        private static void AdjustNow(TimeSpan ts)
        {

            try
            {
                now = now.Add(ts);
                nowUtc = now.ToUniversalTime();
                offset = now - SpaceTimeController.MetaNow;
            }
            catch
            {
                now = new DateTime(1, 12, 25, 23, 59, 59);
                nowUtc = now.ToUniversalTime();
                offset = now - SpaceTimeController.MetaNow;
            }

            if (now.Year > 4000)
            {
                now = new DateTime(4000, 12, 31, 23, 59, 59);
                nowUtc = now.ToUniversalTime();
                offset = now - SpaceTimeController.MetaNow;
            }

            if (now.Year < 1)
            {
                now = new DateTime(0, 12, 25, 23, 59, 59);
                nowUtc = now.ToUniversalTime();
                offset = now - SpaceTimeController.MetaNow;
            }
        }

        static public DateTime GetTimeForFutureTime(double delta)
        {
            try
            {
                if (syncToClock)
                {
                    DateTime future = Now.Add(new TimeSpan((long)((delta * 10000000) * timeRate)));
                    return future;
                }
                else
                {
                    return Now;
                }
            }
            catch
            {
                return Now;
            }
        }
        static public double GetJNowForFutureTime(double delta)
        {
            try
            {
                if (syncToClock)
                {
                    DateTime future = Now.Add(new TimeSpan((long)((delta * 10000000) * timeRate)));
                    return UtcToJulian(future);
                }
                else
                {
                    return UtcToJulian(Now);
                }
            }
            catch
            {
                return UtcToJulian(Now);
            }
        }
        static public DateTime Now
        {
            get
            {
                return nowUtc;
            }
            set
            {
                now = value.ToLocalTime();
                offset = now - SpaceTimeController.MetaNow;
                last = SpaceTimeController.MetaNow;
                nowUtc = now.ToUniversalTime();
            }
        }
        static DateTime last = SpaceTimeController.MetaNow;


        static DateTime metaNow = DateTime.Now;

        public static double FramesPerSecond = 30;

        public static bool FrameDumping = false;
        public static bool CancelFrameDump = false;

        public static int CurrentFrameNumber = 0;
        public static int TotalFrames = 0;

        public static Int64 factor = (HiResTimer.Frequency * 1000) / TimeSpan.FromMilliseconds(1000.0).Ticks;

        public static DateTime MetaNow
        {
            get
            {
                //if (FrameDumping)
                {
                    return metaNow;
                }
                //else
                //{
                //    return DateTime.Now;
                //}
            }
            set
            {
                if (!FrameDumping)
                {
                    metaNow = value;
                }
            }
        }



        public static Int64 MetaNowTickCount
        {
            get
            {
                if (FrameDumping)
                {

                    // todo fix ticks to proper ratio

                    return (Int64)metaNow.Ticks * factor / 1000;
                }
                else
                {
                    return HiResTimer.TickCount;
                }
            }
        }

        public static void NextFrame()
        {
            metaNow += TimeSpan.FromMilliseconds(1000.0 / FramesPerSecond);
            CurrentFrameNumber++;
        }

        public static void SyncTime()
        {
            offset = new TimeSpan();
            now = DateTime.Now;
            nowUtc = now.ToUniversalTime();
            syncToClock = true;
        }

        static TimeSpan offset = new TimeSpan();

        static DateTime now = DateTime.Now;
        static DateTime nowUtc = DateTime.Now.ToUniversalTime();
        static CAADate converter = new CAADate();
        static public double JNow
        {
            get
            {
                return UtcToJulian(Now);
            }
            //set
            //{
            //    converter.Set(value, true);
            //    int Year = 0;
            //    int Month = 0;
            //    int Day = 0;
            //    int Hour = 0;
            //    int Minute = 0;
            //    double Second = 0;
            //    converter.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);
            //    int Ms = ((int)(Second * 1000)) % 1000;
            //    Now = new DateTime(Year, Month, Day, Hour, Minute, (int)Second, Ms);
            //}
        }

        static bool syncToClock = true;

        public static bool SyncToClock
        {
            get { return SpaceTimeController.syncToClock; }
            set
            {
                if (SpaceTimeController.syncToClock != value)
                {
                    SpaceTimeController.syncToClock = value;
                    if (value)
                    {
                        last = DateTime.Now;
                        offset = now - DateTime.Now;
                    }
                    else
                    {
                        now = DateTime.Now.Add(offset);
                        nowUtc = now.ToUniversalTime();
                    }
                }
            }
        }

        static private double timeRate = 1;

        static public double TimeRate
        {
            get { return timeRate; }
            set { timeRate = value; }
        }

        static private Coordinates location;
        static private double altitude;

        public static double Altitude
        {
            get { return SpaceTimeController.altitude; }
            set { SpaceTimeController.altitude = value; }
        }

        static public Coordinates Location
        {
            get
            {
                //if (location == null)
                {
                    location = Coordinates.FromLatLng((double)AppSettings.SettingsBase["LocationLat"], (double)AppSettings.SettingsBase["LocationLng"]);
                    altitude = (double)AppSettings.SettingsBase["LocationAltitude"];
                }
                return location;
            }
            set
            {
                if ((double)AppSettings.SettingsBase["LocationLat"] != value.Lat)
                {
                    AppSettings.SettingsBase["LocationLat"] = value.Lat;
                }

                if ((double)AppSettings.SettingsBase["LocationLng"] != value.Lng)
                {
                    AppSettings.SettingsBase["LocationLng"] = value.Lng;
                }
                location = value;
            }
        }

        public static double UtcToJulian(DateTime utc)
        {
            int year = utc.Year;
            int month = utc.Month;
            double day = utc.Day;
            double hour = utc.Hour;
            double minute = utc.Minute;
            double second = utc.Second + utc.Millisecond / 1000.0;
            double dblDay = day + (hour / 24.0) + (minute / 1440.0) + (second / 86400.0);
            return AstroCalc.AstroCalc.GetJulianDay(year, month, dblDay);
        }

        public static DateTime JulianToUtc (double jDate)
        {
            CAADate date = new CAADate();
            date.Set(jDate, true);
            int year=0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            double second = 0;
            date.Get(ref year, ref month, ref day, ref hour, ref minute, ref second);

            double ms = (second - ((int)second)) * 1000;

            return new DateTime(year, month, day, hour, minute, (int)second, (int)ms);
        }

        public static double TwoLineDateToJulian(string p)
        {
            bool pre1950 = Convert.ToInt32(p.Substring(0, 1)) < 6;
            int year = Convert.ToInt32((pre1950 ? " 20" : "19") + p.Substring(0, 2));
            double days = Convert.ToInt32(p.Substring(2, 3));
            double fraction = Convert.ToDouble(p.Substring(5));

            TimeSpan ts = TimeSpan.FromDays(days - 1 + fraction);
            DateTime date = new DateTime(year, 1, 1, 0, 0, 0, 0);
            date += ts;
            return UtcToJulian(date);
        }

        public static string JulianToTwoLineDate(double jDate)
        {
            return DateToTwoLineDate(JulianToUtc(jDate));
        }

        public static string DateToTwoLineDate(DateTime date)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(date.Year % 100);

            double day = date.DayOfYear + date.Hour / 24 + date.Minute / 60 / 24 + date.Second / 60 / 60 / 24 + date.Millisecond / 1000 / 60 / 60 / 24;

            string sDay = day.ToString("000.00000000");

            sb.Append(sDay);

            return sb.ToString();
        }

        public static bool DoneDumping()
        {

            if (!FrameDumping || CancelFrameDump || (SpaceTimeController.CurrentFrameNumber >= SpaceTimeController.TotalFrames))
            {
                return true;
            }
            return false;
        }

        public static void Faster()
        {
            SpaceTimeController.SyncToClock = true;
            if (TimeRate > .9)
            {
                TimeRate *= 1.1;
            }
            else if (TimeRate < -1)
            {
                TimeRate /= 1.1;
            }
            else
            {
                TimeRate = 1.0;
            }

            if (TimeRate > 1000000000)
            {
                TimeRate = 1000000000;
            }
        }


        public static void Slower()
        {
            SpaceTimeController.SyncToClock = true;
            if (SpaceTimeController.TimeRate < -.9)
            {
                SpaceTimeController.TimeRate *= 1.1;
            }
            else if (SpaceTimeController.TimeRate > 1)
            {
                SpaceTimeController.TimeRate /= 1.1;
            }
            else
            {
                SpaceTimeController.TimeRate = -1.0;
            }

            if (SpaceTimeController.TimeRate < -1000000000)
            {
                SpaceTimeController.TimeRate = -1000000000;
            }
        }

        public static void SetRealtimeNow()
        {
            SyncToClock = true;
            SyncTime();
            TimeRate = 1.0;
        }

        public static void PauseTime()
        {
            SpaceTimeController.SyncToClock = !SpaceTimeController.SyncToClock;
        }

        public static void InvokeAction(string actionString, string value)
        {
            try
            {
                SpaceTimeActions action = (SpaceTimeActions)Enum.Parse(typeof(SpaceTimeActions), actionString, true);

                switch (action)
                {
                    case SpaceTimeActions.Slower:
                        Slower();
                        break;
                    case SpaceTimeActions.Faster:
                        Faster();
                        break;
                    case SpaceTimeActions.PauseTime:
                        PauseTime();
                        break;
                    case SpaceTimeActions.SetRealtimeNow:
                        SetRealtimeNow();
                        break;
                    case SpaceTimeActions.NextSiderealDay:
                        NextSiderealDay();
                        break;
                    case SpaceTimeActions.LastSiderealDay:
                        LastSiderealDay();
                        break;
                    case SpaceTimeActions.NextDay:
                        NextDay();
                        break;
                    case SpaceTimeActions.LastDay:
                        LastDay();
                        break;
                    case SpaceTimeActions.NextSiderealYear:
                        NextSiderealYear();
                        break;
                    case SpaceTimeActions.LastSidrealYear:
                        LastSidrealYear();
                        break;
                    case SpaceTimeActions.NextYear:
                        NextYear();
                        break;
                    case SpaceTimeActions.LastYear:
                        LastYear();
                        break;
                    default:
                        break;
                }
            }
            catch
            {
            }
        }



        private static void LastSidrealYear()
        {
            AdjustNow(TimeSpan.FromDays(-365.256363004));
        }

        private static void NextSiderealYear()
        {
            AdjustNow(TimeSpan.FromDays(365.256363004));
        }

        private static void LastYear()
        {
            if (DateTime.IsLeapYear(now.Year))
            {
                if (now.Month > 2)
                {
                    AdjustNow(TimeSpan.FromDays(-366));
                }
            }
            AdjustNow(TimeSpan.FromDays(-365));

        }

        private static void NextYear()
        {
            if (DateTime.IsLeapYear(now.Year))
            {
                if (now.Month < 3)
                {
                    AdjustNow(TimeSpan.FromDays(366));
                }
            }
            AdjustNow(TimeSpan.FromDays(365));
        }

        private static void LastDay()
        {
            AdjustNow(TimeSpan.FromDays(-1));
        }

        private static void NextDay()
        {
            AdjustNow(TimeSpan.FromDays(1));
        }

        private static void LastSiderealDay()
        {
            AdjustNow(TimeSpan.FromHours(-23.93447));
        }

        private static void NextSiderealDay()
        {
            AdjustNow(TimeSpan.FromHours(23.93447));
        }

        public static string[] GetActionsList()
        {
            return Enum.GetNames(typeof(SpaceTimeActions));
        }


        #region IScriptable Members

        ScriptableProperty[] IScriptable.GetProperties()
        {
            List<ScriptableProperty> props = new List<ScriptableProperty>();

            props.Add(new ScriptableProperty("TimeRate", ScriptablePropertyTypes.Double, ScriptablePropertyScale.Log, -1000000, 1000000, false));
            props.Add(new ScriptableProperty("Pause", ScriptablePropertyTypes.BlendState, ScriptablePropertyScale.Linear, -90, +90, true));
            return props.ToArray();
        }

        string[] IScriptable.GetActions()
        {
            return GetActionsList();
        }

        void IScriptable.InvokeAction(string name, string value)
        {
            SpaceTimeController.InvokeAction(name, value);
        }

        void IScriptable.SetProperty(string name, string value)
        {
            switch (name.ToLower())
            {
                case "timerate":
                    SpaceTimeController.TimeRate = double.Parse(value);
                    break;
                case "pause":
                    SpaceTimeController.SyncToClock = !bool.Parse(value);
                    break;
            }
            return;
        }

        string IScriptable.GetProperty(string name)
        {
            switch (name.ToLower())
            {
                case "timerate":
                    return SpaceTimeController.TimeRate.ToString();

                case "pause":
                    return SpaceTimeController.SyncToClock.ToString();

            }
            return null;
        }

        bool IScriptable.ToggleProperty(string name)
        {
            switch (name.ToLower())
            {

                case "pause":
                    SpaceTimeController.SyncToClock = !SpaceTimeController.SyncToClock;
                    return !SpaceTimeController.syncToClock;
            }
            return false;
        }

        #endregion
    }
}
