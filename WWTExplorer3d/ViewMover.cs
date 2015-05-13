using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TerraViewer
{
    public enum InterpolationType { Linear, EaseIn, EaseOut, EaseInOut, Exponential, Default };
    public struct CameraParameters
    {
        public double Lat;
        public double Lng;
        public double Zoom;
        public double Rotation;
        public double Angle;
        public bool RaDec;
        public double Opacity;
        public Vector3d ViewTarget;
        public SolarSystemObjects Target;
        public string TargetReferenceFrame;

        public double DomeAlt;
        public double DomeAz;

        public CameraParameters(double lat, double lng, double zoom, double rotation, double angle, float opactity)
        {
            Lat = lat;
            Lng = lng;
            Zoom = zoom;
            Rotation = rotation;
            Angle = angle;
            RaDec = false;
            Opacity = opactity;
            ViewTarget = new Vector3d(0, 0, 0);
            Target = SolarSystemObjects.Custom;
            TargetReferenceFrame = "";
            DomeAlt = 0;
            DomeAz = 0;
        }
        public double RA
        {
            get
            {
                return ((((180 - (Lng - 180)) / 360) * 24.0) % 24);
            }
            set
            {
                Lng = 180 - ((value) / 24.0 * 360) - 180;
                RaDec = true;
            }
        }
        public double Dec
        {
            get
            {
                return Lat;
            }
            set
            {
                Lat = value;
            }
        }
        public string ToToken()
        {
            MemoryStream ms = new MemoryStream();

            BinaryWriter bw = new BinaryWriter(ms);

            bw.Write(Lat);
            bw.Write(Lng);
            bw.Write(Zoom);
            bw.Write(Rotation);
            bw.Write(Angle);
            bw.Write(RaDec);
            bw.Write((float)Opacity);
            bw.Write(ViewTarget.X);
            bw.Write(ViewTarget.Y);
            bw.Write(ViewTarget.Z);
            bw.Write((int)Target);
            bw.Write((float)DomeAlt);
            bw.Write((float)DomeAz);
            bw.Close();

            Byte[] data = ms.ToArray();
            StringBuilder sb = new StringBuilder();

            foreach (byte b in data)
            {
                sb.Append(b.ToString("X2"));
            }

            string token = sb.ToString();

            token = token.Replace("00000000", "G");
            token = token.Replace("0000000", "H");
            token = token.Replace("000000", "I");
            token = token.Replace("00000", "J");
            token = token.Replace("0000", "K");
            token = token.Replace("000", "L");
            token = token.Replace("00", "M");

            return token;

        }

        public static CameraParameters FromToken(string token)
        {
            CameraParameters cam = new CameraParameters();

            token = token.Replace("G", "00000000");
            token = token.Replace("H", "0000000");
            token = token.Replace("I","000000" );
            token = token.Replace("J", "00000");
            token = token.Replace("K", "0000");
            token = token.Replace("L", "000");
            token = token.Replace("M", "00");

            try
            {
                byte[] data = new byte[token.Length / 2];
                for (int i = 0; i < token.Length; i += 2)
                {
                    data[i/2] = byte.Parse(token.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                }
                MemoryStream ms = new MemoryStream(data);
                BinaryReader br = new BinaryReader(ms);
                cam.Lat = br.ReadDouble();
                cam.Lng = br.ReadDouble();
                cam.Zoom = br.ReadDouble();
                cam.Rotation = br.ReadDouble();
                cam.Angle = br.ReadDouble();
                cam.RaDec = br.ReadBoolean();
                cam.Opacity = br.ReadSingle();
                cam.ViewTarget.X = br.ReadDouble();
                cam.ViewTarget.Y = br.ReadDouble();
                cam.ViewTarget.Z = br.ReadDouble();
                cam.Target = (SolarSystemObjects)br.ReadInt32();
                try
                {
                    cam.DomeAlt = br.ReadSingle();
                    cam.DomeAz = br.ReadSingle();
                }
                catch
                {
                }
                br.Close();

            }
            catch
            {
            }
            return cam;
        }

        public static CameraParameters Interpolate(CameraParameters from, CameraParameters to, double alphaIn, InterpolationType type, bool fastDirectionMove)
        {
            CameraParameters result = new CameraParameters();
            double alpha = EaseCurve(alphaIn, type);
            double alphaBIn = Math.Min(1.0, alphaIn * 2);
            double alphaB = EaseCurve(alphaBIn, type);
            result.Angle = to.Angle * alpha + from.Angle * (1.0 - alpha);
            result.Rotation = to.Rotation * alpha + from.Rotation * (1.0 - alpha);
            if (fastDirectionMove)
            {
                result.Lat = to.Lat * alphaB + from.Lat * (1.0 - alphaB);
                result.Lng = to.Lng * alphaB + from.Lng * (1.0 - alphaB);
            }
            else
            {
                result.Lat = to.Lat * alpha + from.Lat * (1.0 - alpha);
                result.Lng = to.Lng * alpha + from.Lng * (1.0 - alpha);
            }
            result.Zoom = Math.Pow(2, Math.Log(to.Zoom,2) * alpha + Math.Log(from.Zoom,2) * (1.0 - alpha));
            result.Opacity = (float)(to.Opacity * alpha + from.Opacity * (1.0 - alpha));
            result.ViewTarget = Vector3d.Lerp(from.ViewTarget, to.ViewTarget, alpha);

            result.DomeAlt = to.DomeAlt * alpha + from.DomeAlt * (1.0 - alpha);
            result.DomeAz = to.DomeAz * alpha + from.DomeAz * (1.0 - alpha);


            result.TargetReferenceFrame = to.TargetReferenceFrame;
            if (to.Target == from.Target)
            {
                result.Target = to.Target;
            }
            else
            {
                result.Target = SolarSystemObjects.Custom;
            }
            return result;
        }

        public static CameraParameters InterpolateGreatCircle(CameraParameters from, CameraParameters to, double alphaIn, InterpolationType type)
        {
            CameraParameters result = new CameraParameters();
            double alpha = EaseCurve(alphaIn, type);
            double alphaBIn = Math.Min(1.0, alphaIn * 2);
            double alphaB = EaseCurve(alphaBIn, type);
            result.Angle = to.Angle * alpha + from.Angle * (1.0 - alpha);
            result.Rotation = to.Rotation * alpha + from.Rotation * (1.0 - alpha);

            Vector3d left = Coordinates.GeoTo3dDouble(from.Lat, from.Lng);
            Vector3d right = Coordinates.GeoTo3dDouble(to.Lat, to.Lng);

            Vector3d mid = Vector3d.Slerp(left, right, alpha);

            Vector2d midV2 = Coordinates.CartesianToLatLng(mid);

            result.Lat = midV2.Y;
            result.Lng = midV2.X;
            
            result.Zoom = Math.Pow(2, Math.Log(to.Zoom, 2) * alpha + Math.Log(from.Zoom, 2) * (1.0 - alpha));
            result.Opacity = (float)(to.Opacity * alpha + from.Opacity * (1.0 - alpha));
            result.ViewTarget = Vector3d.Lerp(from.ViewTarget, to.ViewTarget, alpha);

            result.DomeAlt = to.DomeAlt * alpha + from.DomeAlt * (1.0 - alpha);
            result.DomeAz = to.DomeAz * alpha + from.DomeAz * (1.0 - alpha);


            result.TargetReferenceFrame = to.TargetReferenceFrame;
            if (to.Target == from.Target)
            {
                result.Target = to.Target;
            }
            else
            {
                result.Target = SolarSystemObjects.Custom;
            }
            return result;
        }

        const double factor = 0.1085712344;
        public static double EaseCurve(double alpha, InterpolationType type)
        {
           // =100-SINH(A29/Factor*PI())

            switch (type)
            {
                case InterpolationType.Linear:
                    return alpha;            
                case InterpolationType.Exponential:
                    return Math.Pow(alpha, 2);
                case InterpolationType.EaseIn:
                    return ((1 - alpha) * Math.Sinh(alpha / (factor * 2)) / 100.0) + alpha * alpha;
                case InterpolationType.EaseOut:
                    return (alpha * (1-Math.Sinh((1.0 - alpha) / (factor * 2)) / 100.0)) + (1.0 - alpha) * alpha;
                case InterpolationType.EaseInOut:
                    if (alpha < .5)
                    {
                        return Math.Sinh(alpha / factor) / 100.0;
                    }
                    else
                    {
                        return 1.0 - (Math.Sinh((1.0 - alpha) / factor) / 100.0);
                    }
                default:
                    return alpha;
            }
        }

        public static bool operator ==(CameraParameters c1, CameraParameters c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(CameraParameters c1, CameraParameters c2)
        {
            return !c1.Equals(c2);
        }


        public override bool Equals(object obj)
        {
            if (obj is CameraParameters)
            {
                CameraParameters cam = (CameraParameters)obj;

                if (Math.Abs(cam.Angle - this.Angle) > .01 || Math.Abs(cam.Lat - this.Lat) > (cam.Zoom /10000) || Math.Abs(cam.Lng -this.Lng) > (cam.Zoom /10000) || Math.Abs(cam.Rotation -this.Rotation) > .1 || Math.Abs(cam.Zoom - this.Zoom) > (Math.Abs(cam.Zoom - this.Zoom)/1000) || cam.TargetReferenceFrame != this.TargetReferenceFrame || cam.DomeAlt != this.DomeAlt || cam.DomeAz != this.DomeAlt )
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetFuzzyHash(double hashinput, int mult)
        {
                int h = (int)(hashinput * mult);
                return h.GetHashCode();   
        }  
        
        public override int GetHashCode()
        {
            return
             GetFuzzyHash(Lat, 10000)
            ^ GetFuzzyHash(Lng, 10000)
            ^ GetFuzzyHash(Zoom, 10000)
            ^ GetFuzzyHash(Rotation, 10000)
            ^ GetFuzzyHash(Angle, 10000)
            ^ RaDec.GetHashCode()
            ^ GetFuzzyHash(Opacity, 10000)
            ^ GetFuzzyHash(ViewTarget.X, 10000)
            ^ GetFuzzyHash(ViewTarget.Y, 10000)
            ^ GetFuzzyHash(ViewTarget.Z, 10000)
            ^ Target.GetHashCode()
            ^ TargetReferenceFrame.GetHashCode()
            ^ GetFuzzyHash(DomeAlt, 100)
            ^ GetFuzzyHash(DomeAz, 100);
        }


    }

    


    class ViewMoverKenBurnsStyle : IViewMover
    {
        //public bool Dampened = false;

        public InterpolationType InterpolationType = InterpolationType.Linear;
        
        public bool FastDirectionMove = false;
        CameraParameters from;
        CameraParameters to;

        DateTime fromDateTime;
        DateTime toDateTime;
        Int64 fromTime;
        double toTargetTime = 0;
        TimeSpan dateTimeSpan;
        public ViewMoverKenBurnsStyle(CameraParameters from, CameraParameters to, double time, DateTime fromDateTime, DateTime toDateTime, InterpolationType type)
        {
            InterpolationType = type;
            if (Math.Abs(from.Lng - to.Lng) > 180)
            {
                if (from.Lng > to.Lng)
                {
                    from.Lng -= 360;
                }
                else
                {
                    from.Lng += 360;
                }
            }

            this.fromDateTime = fromDateTime;
            this.toDateTime = toDateTime;

            dateTimeSpan = toDateTime - fromDateTime;

            this.from = from;
            this.to = to;
            fromTime = SpaceTimeController.MetaNowTickCount;
            toTargetTime = time;

        }
        bool complete = false;
        bool midpointFired = false;
        public bool Complete
        {
            get
            {
                //Int64 elapsed = HiResTimer.TickCount - fromTime;
                //double elapsedSeconds = ((double)elapsed) / HiResTimer.Frequency;
                //return (elapsedSeconds > toTargetTime);
                return complete;
            }
        }
        public CameraParameters CurrentPosition
        {
            get
            {
                Int64 elapsed = SpaceTimeController.MetaNowTickCount - fromTime;
                double elapsedSeconds = ((double)elapsed) / HiResTimer.Frequency;

                double alpha = elapsedSeconds / (toTargetTime );

                if (!midpointFired && alpha >= .5)
                {
                    midpointFired = true;

                    if (midpoint != null)
                    {
                        midpoint.Invoke(this, new EventArgs());
                    }
                }
                if (alpha > 1.0)
                {
                    alpha = 1.0;
                    complete = true;
                    return to;
                }

                if (Earth3d.MainWindow.Space && Settings.Active.GalacticMode)
                {

                    return CameraParameters.InterpolateGreatCircle(from, to, alpha, InterpolationType);
                }
                else
                {

                    return CameraParameters.Interpolate(from, to, alpha, InterpolationType, FastDirectionMove);
                }
            }
        }

        public DateTime CurrentDateTime
        {
            get
            {
                Int64 elapsed = SpaceTimeController.MetaNowTickCount - fromTime;
                if (elapsed < 0)
                {
                    fromTime = SpaceTimeController.MetaNowTickCount;
                    elapsed = 1;
                }
                double elapsedSeconds = ((double)elapsed) / HiResTimer.Frequency;

                double alpha = elapsedSeconds / (toTargetTime > 0 ? toTargetTime : .00001);

                double delta = dateTimeSpan.TotalSeconds * alpha;

                return fromDateTime.AddSeconds(delta);
            }
        }

        public event EventHandler midpoint;

        event EventHandler IViewMover.Midpoint
        {
            add
            {
                midpoint += value;
            }
            remove
            {
                midpoint -= value;
            }
        }

        public double MoveTime
        {
            get { return toTargetTime; }
        }


    }

    
    class ViewMoverSlew : IViewMover
    {
        CameraParameters from;
        CameraParameters fromTop;
        CameraParameters to;
        CameraParameters toTop;
        Int64 fromTime;
        double upTargetTime = 0;
        double downTargetTime = 0;
        double toTargetTime = 0;
        double upTimeFactor = .6;
        double downTimeFactor = .6;
        double travelTimeFactor = 7.0;
        public ViewMoverSlew(CameraParameters from, CameraParameters to)
        {
            Init(from, to);
        }
        
        public ViewMoverSlew(CameraParameters from, CameraParameters to, double upDowFactor)
        {
            upTimeFactor = downTimeFactor = upDowFactor;
            Init(from, to);
        }   
        
        public void Init(CameraParameters from, CameraParameters to)
        {      
            if (Math.Abs(from.Lng - to.Lng) > 180)
            {
                if (from.Lng > to.Lng)
                {
                    from.Lng -= 360;
                }
                else
                {
                    from.Lng += 360;
                }
            }

            if (to.Zoom <= 0)
            {
                to.Zoom = 360;
            }
            if (from.Zoom <= 0)
            {
                from.Zoom = 360;
            }
            this.from = from;
            this.to = to;
            fromTime = SpaceTimeController.MetaNowTickCount;
            double zoomUpTarget = 360.0;
            double travelTime;

            double lngDist = Math.Abs(from.Lng - to.Lng);
            double latDist = Math.Abs(from.Lat - to.Lat);
            double distance = Math.Sqrt(latDist * latDist + lngDist * lngDist);

            if (Earth3d.MainWindow.Space)
            {
                zoomUpTarget = (distance / 3) * 20;
            }
            else
            {
                zoomUpTarget = (distance / 3)*3;
            }

            if (zoomUpTarget > 360.0)
            {
                zoomUpTarget = 360.0;
            }

            if (zoomUpTarget < from.Zoom)
            {
                zoomUpTarget = from.Zoom;
            }

            if (Earth3d.MainWindow.Space)
            {
                travelTime = (distance / 180.0) * (360 / zoomUpTarget) * travelTimeFactor;
            }
            else
            {
                travelTime = (distance / 180.0) * (75 / zoomUpTarget) * travelTimeFactor;
            }
            double rotateTime = Math.Max(Math.Abs(from.Angle - to.Angle), Math.Abs(from.Rotation - to.Rotation)) ;


            double logDistUp = Math.Max(Math.Abs(Math.Log(zoomUpTarget, 2) - Math.Log(from.Zoom, 2)), rotateTime);
            upTargetTime = upTimeFactor * logDistUp;
            downTargetTime = upTargetTime + travelTime;
            double logDistDown = Math.Abs(Math.Log(zoomUpTarget, 2) - Math.Log(to.Zoom, 2));
            toTargetTime = downTargetTime + Math.Max((downTimeFactor * logDistDown),rotateTime);

            fromTop = from;
            fromTop.Zoom = zoomUpTarget;
            fromTop.Angle = (from.Angle + to.Angle) / 2.0; //todo make short wrap arounds..
            fromTop.Rotation = (from.Rotation + to.Rotation) / 2.0;
            fromTop.DomeAlt = (from.DomeAlt + to.DomeAlt) / 2.0;
            fromTop.DomeAz = (from.DomeAz + to.DomeAz) / 2.0;

            toTop = to;
            toTop.Zoom = fromTop.Zoom;
            toTop.Angle = fromTop.Angle;
            toTop.Rotation = fromTop.Rotation;
            toTop.DomeAlt = fromTop.DomeAlt;
            toTop.DomeAz = fromTop.DomeAz;
        }

        bool midpointFired = false;
        bool complete = false;
        public bool Complete
        {
            get
            {
                //Int64 elapsed = HiResTimer.TickCount - fromTime;
                //double elapsedSeconds = ((double)elapsed) / HiResTimer.Frequency;
                //return (elapsedSeconds > toTargetTime);
                return complete;
            }
        }
        public CameraParameters CurrentPosition
        {
            get
            {
                Int64 elapsed = SpaceTimeController.MetaNowTickCount - fromTime;
                double elapsedSeconds = ((double)elapsed) / HiResTimer.Frequency;

                if (elapsedSeconds < upTargetTime)
                {
                    // Log interpolate from from to fromTop
                    return CameraParameters.Interpolate(from, fromTop, elapsedSeconds / upTargetTime, InterpolationType.EaseInOut, false);
                }
                else if (elapsedSeconds < downTargetTime)
                {
                    elapsedSeconds -= upTargetTime;
                    // interpolate linear fromTop and toTop
                    if (Earth3d.MainWindow.Space && Settings.Active.GalacticMode)
                    {

                        return CameraParameters.InterpolateGreatCircle(fromTop, toTop, elapsedSeconds / (downTargetTime - upTargetTime), InterpolationType.EaseInOut);
                    }
                    
                    return CameraParameters.Interpolate(fromTop, toTop, elapsedSeconds / (downTargetTime - upTargetTime), InterpolationType.EaseInOut, false);
                }
                else
                {
                    if (!midpointFired )
                    {
                        midpointFired = true;

                        if (midpoint != null)
                        {
                            midpoint.Invoke(this, new EventArgs());
                        }
                  
                    }
                    elapsedSeconds -= downTargetTime;
                    // Interpolate log from toTop and to
                    double alpha = elapsedSeconds / (toTargetTime - downTargetTime);
                    if (alpha > 1.0)
                    {
                        alpha = 1.0;
                        complete = true;
                        return to;
                    }
                    return CameraParameters.Interpolate(toTop, to, alpha, InterpolationType.EaseInOut, false);
                }
            }
        }

        public DateTime CurrentDateTime
        {
            get
            {
                SpaceTimeController.UpdateClock();
                return SpaceTimeController.Now;
            }
        }

        public event EventHandler midpoint;

        event EventHandler IViewMover.Midpoint
        {
            add 
            {
                midpoint += value;
            }
            remove 
            {
                midpoint -= value;    
            }
        }

        public double MoveTime
        {
            get { return toTargetTime; }
        }
    }

    public class KeyframeMover : IViewMover, IAnimatable
    {
        CameraParameters currentCamera = new CameraParameters();
        double jDate = 1;
        double durration = 10;

        bool complete = false;

        public string ReferenceFrame
        {
            get
            {
                return currentCamera.TargetReferenceFrame;
            }
            set
            {
                currentCamera.TargetReferenceFrame = value;
            }
        }

        public bool Complete
        {
            get { return complete; }
            set { complete = value; }
        }

        public CameraParameters CurrentPosition
        {
            get
            {
                return currentCamera;
            }
            set
            {
                currentCamera = value;
            }
        }
        static CAADate converter = new CAADate();
        public DateTime CurrentDateTime
        {
            get
            {
                converter.Set(jDate, true);
                int Year = 0;
                int Month = 0;
                int Day = 0;
                int Hour = 0;
                int Minute = 0;
                double Second = 0;
                converter.Get(ref Year, ref Month, ref Day, ref Hour, ref Minute, ref Second);
                int Ms = ((int)(Second * 1000)) % 1000;
                return new DateTime(Year, Month, Day, Hour, Minute, (int)Second, Ms);
            }
            set
            {
                jDate = SpaceTimeController.UtcToJulian(value);
            }
        }

        public event EventHandler Midpoint;

        public double MoveTime
        {
            get
            {
                return durration;
            }
            set
            {
                durration = value;
            }
        }

        public double[] GetParams()
        {
            currentCamera = Earth3d.MainWindow.viewCamera;
            jDate = SpaceTimeController.JNow;
            double[] paramList = new double[13];
            paramList[0] = jDate;     
            paramList[1] = currentCamera.Lat;
            paramList[2] = currentCamera.Lng;
            paramList[3] = currentCamera.Zoom;
            paramList[4] = currentCamera.Rotation;
            paramList[5] = currentCamera.Angle;
            paramList[6] = currentCamera.Opacity;
            paramList[7] = currentCamera.ViewTarget.X;
            paramList[8] = currentCamera.ViewTarget.Y;
            paramList[9] = currentCamera.ViewTarget.Z;
            paramList[10] = (int)currentCamera.Target;
            paramList[11] = currentCamera.DomeAlt;
            paramList[12] = currentCamera.DomeAz;

            return paramList;
        }

        public string[] GetParamNames()
        {
            return new string[] { "DateTime", "Lat", "Lng", "Zoom", "Rotation", "Angle", "Opacity", "ViewTarget.X", "ViewTarget.Y", "ViewTarget.Z", "SolarSystemTarget", "Dome.Alt", "Dome.Az" };
        }

        public BaseTweenType[] GetParamTypes()
        {
            return new BaseTweenType[] { BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Power, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.Linear, BaseTweenType.PlanetID, BaseTweenType.Linear, BaseTweenType.Linear };
        }

        public void SetParams(double[] paramList)
        {
            if (paramList.Length == 13)
            {
                jDate = paramList[0];
                currentCamera.Lat = paramList[1];
                currentCamera.Lng = paramList[2];
                currentCamera.Zoom = paramList[3];
                currentCamera.Rotation = paramList[4];
                currentCamera.Angle = paramList[5];
                currentCamera.Opacity = paramList[6];
                currentCamera.ViewTarget = new Vector3d(paramList[7], paramList[8], paramList[9]);
                currentCamera.Target = (SolarSystemObjects)(int)(paramList[10]+.5);
                currentCamera.DomeAlt = paramList[11];
                currentCamera.DomeAz = paramList[12];
            }
        }

        public string GetIndentifier()
        {
            return "Camera";
        }

        public string GetName()
        {
            return "Camera";
        }

        public IUiController GetEditUI()
        {
            return null;
        }
    }
}
