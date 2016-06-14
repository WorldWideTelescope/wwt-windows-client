using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.IO;
using SharpDX;
using Color = System.Drawing.Color;
using System.Reflection;
using System.Xml;

namespace TerraViewer
{
    public enum ReferenceFrameTypes { FixedSherical, Orbital, Trajectory, Synodic /*,FixedRectangular*/ };
    public class ReferenceFrame : IAnimatable
    {
        public bool SystemGenerated = true; 
        // Calclulated
        public Vector3d Position;
        public double MeanAnomoly;
        public Matrix3d WorldMatrix;
        public double OrbitalYears;

        public bool ObservingLocation = false;
        // Serialized
        public string name;

        [LayerProperty]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string parent;

        [LayerProperty]
        public string Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        public ReferenceFrames reference = ReferenceFrames.Custom;

        [LayerProperty]
        public ReferenceFrames Reference
        {
            get { return reference; }
            set { reference = value; }
        }
        public bool parentsRoationalBase = false;

        [LayerProperty]
        public bool ParentsRoationalBase
        {
            get { return parentsRoationalBase; }
            set { parentsRoationalBase = value; }
        }
        public ReferenceFrameTypes referenceFrameType = ReferenceFrameTypes.FixedSherical;

        [LayerProperty]
        public ReferenceFrameTypes ReferenceFrameType
        {
            get { return referenceFrameType; }
            set { referenceFrameType = value; }
        }
        public double meanRadius = 1000;

        [LayerProperty]
        public double MeanRadius
        {
            get { return meanRadius; }
            set { meanRadius = value; }
        }
        public double oblateness = 0;

        [LayerProperty]
        public double Oblateness
        {
            get { return oblateness; }
            set { oblateness = value; }
        }
        public double heading;

        [LayerProperty]
        public double Heading
        {
            get { return heading; }
            set { heading = value; }
        }
        public double pitch;

        [LayerProperty]
        public double Pitch
        {
            get { return pitch; }
            set { pitch = value; }
        }
        public double roll;

        [LayerProperty]
        public double Roll
        {
            get { return roll; }
            set { roll = value; }
        }
        public double scale = 1;

        [LayerProperty]
        public double Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public double tilt;

        [LayerProperty]
        public double Tilt
        {
            get { return tilt; }
            set { tilt = value; }
        }

        public Vector3d translation;

        [LayerProperty] 
        public Vector3d Translation
        {
            get { return translation; }
            set { translation = value; }
        }


        // For Spherical Offset
        public double lat;

        [LayerProperty]
        public double Lat
        {
            get { return lat; }
            set { lat = value; }
        }
        public double lng;

        [LayerProperty]
        public double Lng
        {
            get { return lng; }
            set { lng = value; }
        }
        public double altitude;

        [LayerProperty]
        public double Altitude
        {
            get { return altitude; }
            set { altitude = value; }
        }


        // For Rotating frames
        public double rotationalPeriod; // Days

        [LayerProperty]
        public double RotationalPeriod
        {
            get { return rotationalPeriod; }
            set { rotationalPeriod = value; }
        }
        public double zeroRotationDate; // Julian decimal

        [LayerProperty]
        public double ZeroRotationDate
        {
            get { return zeroRotationDate; }
            set { zeroRotationDate = value; }
        }

        // For representing orbits & distant point location
        public Color representativeColor = Color.White; // Used for orbits and points
        
        public Color RepresentativeColor
        {
            get
            {
                return representativeColor;
            }
            set
            {
                if (value != representativeColor)
                {
                    representativeColor = value;
                    trajectoryLines = null;
                    orbit = null;
                }
            }
        }

        [LayerProperty]
        public virtual string RepresentativeColorValue
        {
            get
            {
                SavedColor saveCol = new SavedColor(representativeColor.ToArgb());
                return saveCol.Save();
            }
            set
            {
                Color newVal = SavedColor.Load(value);

                if (RepresentativeColor != newVal)
                {
                    RepresentativeColor = newVal;
                }
            }
        }

        public bool showAsPoint = false;

        [LayerProperty]
        public bool ShowAsPoint
        {
            get { return showAsPoint; }
            set { showAsPoint = value; }
        }
        public bool showOrbitPath = false;

        [LayerProperty]
        public bool ShowOrbitPath
        {
            get { return showOrbitPath; }
            set { showOrbitPath = value; }
        }
        public bool stationKeeping = true;

        [LayerProperty]
        public bool StationKeeping
        {
            get { return stationKeeping; }
            set { stationKeeping = value; }
        }

        public double semiMajorAxis; // a Au's

        [LayerProperty]
        public double SemiMajorAxis
        {
            get { return semiMajorAxis; }
            set { semiMajorAxis = value; }
        }
        public AltUnits semiMajorAxisUnits = AltUnits.Meters;

        [LayerProperty]
        public AltUnits SemiMajorAxisUnits
        {
            get { return semiMajorAxisUnits; }
            set { semiMajorAxisUnits = value; }
        }
        public double eccentricity; // e

        [LayerProperty]
        public double Eccentricity
        {
            get { return eccentricity; }
            set { eccentricity = value; }
        }
        public double inclination; // i

        [LayerProperty]
        public double Inclination
        {
            get { return inclination; }
            set { inclination = value; }
        }
        public double argumentOfPeriapsis; // w

        [LayerProperty]
        public double ArgumentOfPeriapsis
        {
            get { return argumentOfPeriapsis; }
            set { argumentOfPeriapsis = value; }
        }
        public double longitudeOfAscendingNode; // Omega

        [LayerProperty]
        public double LongitudeOfAscendingNode
        {
            get { return longitudeOfAscendingNode; }
            set { longitudeOfAscendingNode = value; }
        }
        public double meanAnomolyAtEpoch; // M

        [LayerProperty]
        public double MeanAnomolyAtEpoch
        {
            get { return meanAnomolyAtEpoch; }
            set { meanAnomolyAtEpoch = value; }
        }
        public double meanDailyMotion; // n .degrees day

        [LayerProperty]
        public double MeanDailyMotion
        {
            get { return meanDailyMotion; }
            set { meanDailyMotion = value; }
        }
        public double epoch; //   Standard Equinox

        [LayerProperty]
        public double Epoch
        {
            get { return epoch; }
            set { epoch = value; }
        }
      
        private Orbit orbit = null;

        public Orbit Orbit
        {
            get
            {
                return orbit;
            }
            set { orbit = value; }
        }
        public LineList trajectoryLines = null;
        CAAEllipticalObjectElements elements = new CAAEllipticalObjectElements();


        public List<TrajectorySample> Trajectory = new List<TrajectorySample>();

        public void ImportTrajectory(string filename)
        {
            Trajectory.Clear();
            string[] data = File.ReadAllLines(filename);
            foreach (string line in data)
            {
                Trajectory.Add(new TrajectorySample(line));
            }
        }

        public virtual void SaveToXml(System.Xml.XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ReferenceFrame");
            xmlWriter.WriteAttributeString("Name", Name);
            xmlWriter.WriteAttributeString("Parent", Parent);
            xmlWriter.WriteAttributeString("ReferenceFrameType", ReferenceFrameType.ToString());
            xmlWriter.WriteAttributeString("Reference", Reference.ToString());
            xmlWriter.WriteAttributeString("ParentsRoationalBase", ParentsRoationalBase.ToString());
            xmlWriter.WriteAttributeString("MeanRadius", MeanRadius.ToString());
            xmlWriter.WriteAttributeString("Oblateness", Oblateness.ToString());
            xmlWriter.WriteAttributeString("Heading", Heading.ToString());
            xmlWriter.WriteAttributeString("Pitch", Pitch.ToString());
            xmlWriter.WriteAttributeString("Roll", Roll.ToString());
            xmlWriter.WriteAttributeString("Scale", Scale.ToString());
            xmlWriter.WriteAttributeString("Tilt", Tilt.ToString());
            xmlWriter.WriteAttributeString("Translation", Translation.ToString());
            if (ReferenceFrameType == ReferenceFrameTypes.FixedSherical)
            {
                xmlWriter.WriteAttributeString("Lat", Lat.ToString());
                xmlWriter.WriteAttributeString("Lng", Lng.ToString());
                xmlWriter.WriteAttributeString("Altitude", Altitude.ToString());
            }
            xmlWriter.WriteAttributeString("RotationalPeriod", RotationalPeriod.ToString());
            xmlWriter.WriteAttributeString("ZeroRotationDate", ZeroRotationDate.ToString());
            xmlWriter.WriteAttributeString("RepresentativeColor", SavedColor.Save(RepresentativeColor));
            xmlWriter.WriteAttributeString("ShowAsPoint", ShowAsPoint.ToString());
            xmlWriter.WriteAttributeString("ShowOrbitPath", ShowOrbitPath.ToString());

            xmlWriter.WriteAttributeString("StationKeeping", StationKeeping.ToString());

            if (ReferenceFrameType == ReferenceFrameTypes.Orbital)
            {
                xmlWriter.WriteAttributeString("SemiMajorAxis", SemiMajorAxis.ToString());
                xmlWriter.WriteAttributeString("SemiMajorAxisScale", this.SemiMajorAxisUnits.ToString());
                xmlWriter.WriteAttributeString("Eccentricity", Eccentricity.ToString());
                xmlWriter.WriteAttributeString("Inclination", Inclination.ToString());
                xmlWriter.WriteAttributeString("ArgumentOfPeriapsis", ArgumentOfPeriapsis.ToString());
                xmlWriter.WriteAttributeString("LongitudeOfAscendingNode", LongitudeOfAscendingNode.ToString());
                xmlWriter.WriteAttributeString("MeanAnomolyAtEpoch", MeanAnomolyAtEpoch.ToString());
                xmlWriter.WriteAttributeString("MeanDailyMotion", MeanDailyMotion.ToString());
                xmlWriter.WriteAttributeString("Epoch", Epoch.ToString());
            }

            if (ReferenceFrameType == ReferenceFrameTypes.Trajectory)
            {
                xmlWriter.WriteStartElement("Trajectory");

                foreach (TrajectorySample sample in Trajectory)
                {
                    string data = sample.ToString();
                    xmlWriter.WriteElementString("Sample", data);
                }
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }

        public virtual void InitializeFromXml(System.Xml.XmlNode node)
        {
            Name = node.Attributes["Name"].Value;
            Parent = node.Attributes["Parent"].Value;

            ReferenceFrameType = (ReferenceFrameTypes)Enum.Parse(typeof(ReferenceFrameTypes), node.Attributes["ReferenceFrameType"].Value);

            Reference = (ReferenceFrames)Enum.Parse(typeof(ReferenceFrames), node.Attributes["Reference"].Value);

            ParentsRoationalBase = Boolean.Parse(node.Attributes["ParentsRoationalBase"].Value);
            MeanRadius = Double.Parse(node.Attributes["MeanRadius"].Value);
            Oblateness = Double.Parse(node.Attributes["Oblateness"].Value);
            Heading = Double.Parse(node.Attributes["Heading"].Value);
            Pitch = Double.Parse(node.Attributes["Pitch"].Value);
            Roll = Double.Parse(node.Attributes["Roll"].Value);
            Scale = Double.Parse(node.Attributes["Scale"].Value);
            Tilt = Double.Parse(node.Attributes["Tilt"].Value);
            Translation = Vector3d.Parse(node.Attributes["Translation"].Value);
            if (ReferenceFrameType == ReferenceFrameTypes.FixedSherical)
            {
                Lat = Double.Parse(node.Attributes["Lat"].Value);
                Lng = Double.Parse(node.Attributes["Lng"].Value);
                Altitude = Double.Parse(node.Attributes["Altitude"].Value);
            }

            RotationalPeriod = Double.Parse(node.Attributes["RotationalPeriod"].Value);
            ZeroRotationDate = Double.Parse(node.Attributes["ZeroRotationDate"].Value);
            RepresentativeColor = SavedColor.Load(node.Attributes["RepresentativeColor"].Value);
            ShowAsPoint = Boolean.Parse(node.Attributes["ShowAsPoint"].Value);
            if (node.Attributes["StationKeeping"] != null)
            {
                StationKeeping = Boolean.Parse(node.Attributes["StationKeeping"].Value);
            }

            if (node.Attributes["ShowOrbitPath"] != null)
            {
                ShowOrbitPath = Boolean.Parse(node.Attributes["ShowOrbitPath"].Value);
            }

            if (ReferenceFrameType == ReferenceFrameTypes.Orbital)
            {
                SemiMajorAxis = Double.Parse(node.Attributes["SemiMajorAxis"].Value);
                SemiMajorAxisUnits = (AltUnits)Enum.Parse(typeof(AltUnits), node.Attributes["SemiMajorAxisScale"].Value);

                Eccentricity = Double.Parse(node.Attributes["Eccentricity"].Value);
                Inclination = Double.Parse(node.Attributes["Inclination"].Value);
                ArgumentOfPeriapsis = Double.Parse(node.Attributes["ArgumentOfPeriapsis"].Value);
                LongitudeOfAscendingNode = Double.Parse(node.Attributes["LongitudeOfAscendingNode"].Value);
                MeanAnomolyAtEpoch = Double.Parse(node.Attributes["MeanAnomolyAtEpoch"].Value);
                MeanDailyMotion = Double.Parse(node.Attributes["MeanDailyMotion"].Value);
                Epoch = Double.Parse(node.Attributes["Epoch"].Value);

            }
            if (ReferenceFrameType == ReferenceFrameTypes.Trajectory)
            {
                if (node["Trajectory"] != null)
                {
                    foreach (XmlNode child in node["Trajectory"])
                    {
                        Trajectory.Add(new TrajectorySample(child.InnerText));
                    }
                }
            }

            SystemGenerated = false;
        }

        public void FromTLE(string line1, string line2, double gravity)
        {
            bool isMpcTLE = false;
            if (line1.Substring(0, 8) == "1 99999U")
            {
                isMpcTLE = true;
                gravity = 132737773784;
                gravity *= 1000000000;
            }

            Epoch = SpaceTimeController.TwoLineDateToJulian(line1.Substring(18, 14));
            Eccentricity = double.Parse("0." + line2.Substring(26, 7));
            Inclination = double.Parse(line2.Substring(8, 8));
            LongitudeOfAscendingNode = double.Parse(line2.Substring(17, 8));
            ArgumentOfPeriapsis = double.Parse(line2.Substring(34, 8));
            double revs = double.Parse(line2.Substring(52, 11));
            MeanAnomolyAtEpoch = double.Parse(line2.Substring(43, 8));
            MeanDailyMotion = revs * 360.0;
            double part = (86400.0 / revs) / (Math.PI * 2.0);
            SemiMajorAxis = Math.Pow((part * part) * gravity, 1.0 / 3.0);
            SemiMajorAxisUnits = AltUnits.Meters;
            if (isMpcTLE)
            {
                SemiMajorAxis = double.Parse(line1.Substring(33, 10));
                MeanDailyMotion = double.Parse(line1.Substring(52, 11));
            }

        }

        /// <summary>
        /// This is a hack to save Orbits for use in older WWT versions. This is not for
        /// Interoperabilit with other System that ingest TLE Data
        /// </summary>
        /// <returns></returns>
        public string ToTLE()
        {
            //Epoch need to convert to TLE time string.
            // Ecentricity remove "0." from the begin and trim to 7 digits
            // Inclination decimal degrees 8 digits max
            // LOAN decimal degrees 8 digits
            // AOP 
            // mean anomoly at epoch 8 digits
            // Mean motion (revs per day) Compute
            // Convert Semi-major-axis to meters from storage unit
            // Compute revs

            StringBuilder line1 = new StringBuilder();
            
            line1.Append("1 99999U 00111AAA ");
            line1.Append(SpaceTimeController.JulianToTwoLineDate(Epoch));
            line1.Append(" ");
            // line1.Append("-.00000001");
            line1.Append(semiMajorAxis.ToString("0.0000e+00"));
            line1.Append(" 00000-0 ");
            line1.Append(meanDailyMotion.ToString("0.00000e+00"));
            //line1.Append("-00000-1 0 ");
            line1.Append("  001");
            line1.Append(ComputeTLECheckSum(line1.ToString()));
            line1.AppendLine("");
            StringBuilder line2 = new StringBuilder();

            line2.Append("2 99999 ");
            line2.Append(inclination.ToString("000.0000 ")); 
            line2.Append(longitudeOfAscendingNode.ToString("000.0000 "));
            line2.Append(eccentricity.ToString("0.0000000 ").Substring(2));
            line2.Append(argumentOfPeriapsis.ToString("000.0000 "));
            line2.Append(meanAnomolyAtEpoch.ToString("000.0000 "));
            line2.Append((meanDailyMotion/207732).ToString("0.00000e+00"));
            line2.Append("00001");
            line2.Append(ComputeTLECheckSum(line2.ToString()));
            line2.AppendLine("");
            return line1.ToString() + line2.ToString();
        }

        public static char ComputeTLECheckSum(string line)
        {
            if (line.Length != 68)
            {
                return '0';
            }

            int checksum = 0;
            for (int i = 0; i < 68; i++)
            {
                switch (line[i])
                {
                    case '1':
                        checksum += 1;
                        break;
                    case '2':
                        checksum += 2;
                        break;
                    case '3':
                        checksum += 3;
                        break;
                    case '4':
                        checksum += 4;
                        break;
                    case '5':
                        checksum += 5;
                        break;
                    case '6':
                        checksum += 6;
                        break;
                    case '7':
                        checksum += 7;
                        break;
                    case '8':
                        checksum += 8;
                        break;
                    case '9':
                        checksum += 9;
                        break;
                    case '-':
                        checksum += 1;

                        break;
                }
            }
            return (char)('0' + (char)(checksum % 10));
        }

        public static bool IsTLECheckSumGood(string line)
        {
            if (line.Length != 69)
            {
                return false;
            }

            int checksum = 0;
            for (int i = 0; i < 68; i++)
            {
                switch (line[i])
                {
                    case '1':
                        checksum += 1;
                        break;
                    case '2':
                        checksum += 2;
                        break;
                    case '3':
                        checksum += 3;
                        break;
                    case '4':
                        checksum += 4;
                        break;
                    case '5':
                        checksum += 5;
                        break;
                    case '6':
                        checksum += 6;
                        break;
                    case '7':
                        checksum += 7;
                        break;
                    case '8':
                        checksum += 8;
                        break;
                    case '9':
                        checksum += 9;
                        break;
                    case '-':
                        checksum += 1;

                        break;
                }
            }
            return ('0' + (char)(checksum % 10)) == line[68];

        }
        public CAAEllipticalObjectElements Elements
        {
            get
            {
                elements.a = UiTools.GetMeters(SemiMajorAxis, SemiMajorAxisUnits);
                elements.e = Eccentricity;
                elements.i = Inclination;
                //ArgumentOfPeriapsis += LongitudeOfAscendingNode;
                elements.w = ArgumentOfPeriapsis;
                elements.omega = LongitudeOfAscendingNode;
                elements.JDEquinox = Epoch;
                if (MeanDailyMotion == 0)
                {
                    elements.n = CAAElliptical.MeanMotionFromSemiMajorAxis(elements.a/(UiTools.KilometersPerAu));
                }
                else
                {
                    elements.n = MeanDailyMotion;
                }
                elements.T = Epoch - (MeanAnomolyAtEpoch / elements.n);
                return elements;
            }
            set { elements = value; }
        }

        public void ComputeFrame(RenderContext11 renderContext)
        {
            if (this.Reference == ReferenceFrames.Sandbox)
            {
                WorldMatrix = Matrix3d.Identity;
                return;
            }



            switch (ReferenceFrameType)
            {
                case ReferenceFrameTypes.Orbital:
                    ComputeOrbital(renderContext);
                    break;
                case ReferenceFrameTypes.FixedSherical:
                    ComputeFixedSherical(renderContext);
                    break;
                case ReferenceFrameTypes.Trajectory:
                    ComputeFrameTrajectory(renderContext);
                    break;
                case ReferenceFrameTypes.Synodic:
                    ComputeFrameSynodic(renderContext);
                    break;
                   
                //case ReferenceFrameTypes.FixedRectangular:
                //    ComputeFixedRectangular(renderContext);
                //    break;
                default:
                    break;
            }
        }

        public bool useRotatingParentFrame()
        {
            switch (ReferenceFrameType)
            {
                case ReferenceFrameTypes.Orbital:
                case ReferenceFrameTypes.Trajectory:
                case ReferenceFrameTypes.Synodic:
                    return false;
                default:
                    return true;
            }
        }

        private void ComputeFixedRectangular(RenderContext11 renderContext)
        {
            WorldMatrix = Matrix3d.Identity;
            WorldMatrix.Translate(Translation);
            WorldMatrix.Rotate(Quaternion.RotationYawPitchRoll((float)((Heading) / 180.0 * Math.PI), (float)(Pitch / 180.0 * Math.PI), (float)(Roll / 180.0 * Math.PI)));
            WorldMatrix.Scale(new Vector3d(Scale, Scale, Scale));
            WorldMatrix.Rotate(Quaternion.RotationYawPitchRoll((float)(Coordinates.MstFromUTC2(SpaceTimeController.Now, 0) / 180 * Math.PI), (float)0, (float)0));
        }
        private void ComputeFixedSherical(RenderContext11 renderContext)
        {
            if (ObservingLocation)
            {
                Lat = SpaceTimeController.Location.Lat;
                Lng = SpaceTimeController.Location.Lng;
                Altitude = SpaceTimeController.Altitude;
            }


            WorldMatrix = Matrix3d.Identity;
            WorldMatrix.Translate(Translation);
            double localScale = (1 / renderContext.NominalRadius) * Scale * MeanRadius;
            WorldMatrix.Scale(new Vector3d(localScale, localScale, localScale));
            //WorldMatrix.Scale(new Vector3d(1000, 1000, 1000));
            WorldMatrix.Rotate(Quaternion.RotationYawPitchRoll((float)((Heading) / 180.0 * Math.PI), (float)(Pitch / 180.0 * Math.PI), (float)(Roll / 180.0 * Math.PI)));
            WorldMatrix.Multiply(Matrix3d.RotationZ(-90.0 / 180.0 * Math.PI));
            if (RotationalPeriod != 0)
            {
                double rotationCurrent = (((SpaceTimeController.JNow - this.ZeroRotationDate) / RotationalPeriod) * Math.PI * 2) % (Math.PI * 2);
                WorldMatrix.Multiply(Matrix3d.RotationX(-rotationCurrent));
            }
            WorldMatrix.Translate(new Vector3d(1 + (Altitude / renderContext.NominalRadius), 0, 0));
            WorldMatrix.Multiply(Matrix3d.RotationZ(Lat / 180 * Math.PI));
            WorldMatrix.Multiply(Matrix3d.RotationY(-(Lng) / 180 * Math.PI));
        }

        private void ComputeFrameSynodic(RenderContext11 renderContext)
        {
            // A synodic frame is a rotating frame of reference in which
            // the x-axis is the direction between the bodies in a two-body
            // system. The origin is at the secondary body, and +x points
            // in the direction opposite the primary. The z-axis is in the orbital
            // plane and normal to x; it points in the direction of the instantaneous
            // orbital velocity of the secondary.

            // The origin is offset by then translation. The five libration points in a 
            // two-body system can be approximated by choosing different offsets. For
            // example, the Sun-Earth L2 point is approximated by using x = 1,500,000 km, y = 0 and z = 0.

            WorldMatrix = Matrix3d.Identity;
            double localScale = (1 / renderContext.NominalRadius) * Scale * MeanRadius;
            WorldMatrix.Scale(new Vector3d(localScale, localScale, localScale));
            WorldMatrix.Rotate(Quaternion.RotationYawPitchRoll((float)((Heading) / 180.0 * Math.PI), (float)(Pitch / 180.0 * Math.PI), (float)(Roll / 180.0 * Math.PI)));
            WorldMatrix.Translate(Translation / 6371.000);

            // Currently we assume the Sun-Earth system
            double jd = SpaceTimeController.JNow;
            double B = CAACoordinateTransformation.DegreesToRadians(CAAEarth.EclipticLatitude(jd));
            double L = CAACoordinateTransformation.DegreesToRadians(CAAEarth.EclipticLongitude(jd));
            Vector3d eclPos = new Vector3d(Math.Cos(L) * Math.Cos(B), Math.Sin(L) * Math.Cos(B), Math.Sin(B));

            // Just approximate the orbital velocity for now
            Vector3d eclVel = Vector3d.Cross(eclPos, new Vector3d(0.0, 0.0, 1.0));
            eclVel.Normalize();

            // Convert to WWT's coordinate convention by swapping Y and Z
            eclPos = new Vector3d(eclPos.X, eclPos.Z, eclPos.Y);
            eclVel = new Vector3d(eclVel.X, eclVel.Z, eclVel.Y);

            Vector3d xaxis = eclPos;
            Vector3d yaxis = Vector3d.Cross(eclVel, eclPos);
            Vector3d zaxis = eclVel;
            Matrix3d rotation = new Matrix3d(xaxis.X, yaxis.X, zaxis.X, 0, xaxis.Y, yaxis.Y, zaxis.Y, 0, xaxis.Z, yaxis.Z, zaxis.Z, 0, 0, 0, 0, 1);

            // Convert from ecliptic to J2000 EME (equatorial) system
            double earthObliquity = CAACoordinateTransformation.DegreesToRadians(Coordinates.MeanObliquityOfEcliptic(jd));
            rotation = rotation * Matrix3d.RotationX(-earthObliquity);

            WorldMatrix.Multiply(rotation);
        }

        private void ComputeFrameTrajectory(RenderContext11 renderContext)
        {
            Vector3d vector = new Vector3d();
            Vector3d point = GetTragectoryPoint(SpaceTimeController.JNow, out vector);

            Vector3d direction = vector;

            direction.Normalize();
            Vector3d up = point;
            up.Normalize();
            direction.Normalize();

            double dist = point.Length();
            double scaleFactor = 1.0;
            //scaleFactor = UiTools.KilometersPerAu * 1000;
            scaleFactor *= 1 / renderContext.NominalRadius;


            Matrix3d look = Matrix3d.LookAtLH(new Vector3d(0, 0, 0), direction, new Vector3d(0, 1, 0));
            look.Invert();

            WorldMatrix = Matrix3d.Identity;
            WorldMatrix.Translate(Translation);
            double localScale = (1 / renderContext.NominalRadius) * Scale * MeanRadius;
            WorldMatrix.Scale(new Vector3d(localScale, localScale, localScale));
            WorldMatrix.Rotate(Quaternion.RotationYawPitchRoll((float)((Heading) / 180.0 * Math.PI), (float)(Pitch / 180.0 * Math.PI), (float)(Roll / 180.0 * Math.PI))); 
            if (RotationalPeriod != 0)
            {
                double rotationCurrent = (((SpaceTimeController.JNow - this.ZeroRotationDate) / RotationalPeriod) * Math.PI * 2) % (Math.PI * 2);
                WorldMatrix.Multiply(Matrix3d.RotationX(-rotationCurrent));
            }

            point = Vector3d.Scale(point, scaleFactor);

            WorldMatrix.Translate(point);

            if (StationKeeping)
            {
                WorldMatrix = look * WorldMatrix;
            }
        }

        private Vector3d GetTragectoryPoint(double jNow, out Vector3d vector)
        {
            int min = 0;
            int max = Trajectory.Count - 1;

            Vector3d point = new Vector3d();

            vector = new Vector3d();

            int current = max / 2;

            bool found = false;

            while (!found)
            {
                if (current < 1)
                {
                    vector = Trajectory[0].Position - Trajectory[1].Position;
                    return Trajectory[0].Position;
                }


                if (current == Trajectory.Count - 1)
                {
                    vector = Trajectory[current - 1].Position - Trajectory[current].Position;
                    return Trajectory[current].Position;
                }

                if ((Trajectory[current - 1].Time <= jNow) && (Trajectory[current].Time > jNow))
                {
                    double denominator = Trajectory[current].Time - Trajectory[current - 1].Time;
                    double numerator = jNow - Trajectory[current - 1].Time;
                    double tween = numerator / denominator;
                    vector = Trajectory[current - 1].Position - Trajectory[current].Position;
                    point = Vector3d.Lerp(Trajectory[current - 1].Position, Trajectory[current].Position, tween);
                    return point;
                }

                if (Trajectory[current].Time < jNow)
                {
                    int next = current + (max - current + 1) / 2;
                    min = current;
                    current = next;
                }
                else
                    if (Trajectory[current - 1].Time > jNow)
                    {
                        int next = current - (current - min + 1) / 2;
                        max = current;
                        current = next;
                    }
            }

            return point;
        }

        private void ComputeOrbital(RenderContext11 renderContext)
        {
            CAAEllipticalObjectElements ee = Elements;
            Vector3d point = CAAElliptical.CalculateRectangular(SpaceTimeController.JNow, ee, out MeanAnomoly);

            Vector3d pointInstantLater = CAAElliptical.CalculateRectangular(ee, MeanAnomoly + .001);

            Vector3d direction = point - pointInstantLater;

            direction.Normalize();
            Vector3d up = point;
            up.Normalize();
            direction.Normalize();

            double dist = point.Length();
            double scaleFactor = 1.0;
            switch (SemiMajorAxisUnits)
            {
                case AltUnits.Meters:
                    scaleFactor = 1.0;
                    break;
                case AltUnits.Feet:
                    scaleFactor = 1.0 / 3.2808399;
                    break;
                case AltUnits.Inches:
                    scaleFactor = (1.0 / 3.2808399) / 12;
                    break;
                case AltUnits.Miles:
                    scaleFactor = 1609.344;
                    break;
                case AltUnits.Kilometers:
                    scaleFactor = 1000;
                    break;
                case AltUnits.AstronomicalUnits:
                    scaleFactor = UiTools.KilometersPerAu * 1000;
                    break;
                case AltUnits.LightYears:
                    scaleFactor = UiTools.AuPerLightYear * UiTools.KilometersPerAu * 1000;
                    break;
                case AltUnits.Parsecs:
                    scaleFactor = UiTools.AuPerParsec * UiTools.KilometersPerAu * 1000;
                    break;
                case AltUnits.MegaParsecs:
                    scaleFactor = UiTools.AuPerParsec * UiTools.KilometersPerAu * 1000 * 1000000;
                    break;
                case AltUnits.Custom:
                    scaleFactor = 1;
                    break;
                default:
                    break;
            }
            scaleFactor *= 1 / renderContext.NominalRadius;


            Matrix3d look = Matrix3d.LookAtLH(new Vector3d(0, 0, 0), direction, up);
            look.Invert();

            WorldMatrix = Matrix3d.Identity;
            WorldMatrix.Translate(Translation);


            double localScale = (1 / renderContext.NominalRadius) * Scale * MeanRadius;
            WorldMatrix.Scale(new Vector3d(localScale, localScale, localScale));
            WorldMatrix.Rotate(Quaternion.RotationYawPitchRoll((float)((Heading) / 180.0 * Math.PI), (float)(Pitch / 180.0 * Math.PI), (float)(Roll / 180.0 * Math.PI)));
            if (RotationalPeriod != 0)
            {
                double rotationCurrent = (((SpaceTimeController.JNow - this.ZeroRotationDate) / RotationalPeriod) * Math.PI * 2) % (Math.PI * 2);
                WorldMatrix.Multiply(Matrix3d.RotationX(-rotationCurrent));
            }

            point = Vector3d.Scale(point, scaleFactor);

            WorldMatrix.Translate(point);

            if (StationKeeping)
            {
                WorldMatrix = look * WorldMatrix;
            }

        }

        public bool SetProp(string name, string value)
        {
            Type thisType = this.GetType();
            PropertyInfo pi = thisType.GetProperty(name);
            bool safeToSet = false;
            Type layerPropType = typeof(LayerProperty);
            object[] attributes = pi.GetCustomAttributes(false);
            foreach (object var in attributes)
            {
                if (var.GetType() == layerPropType)
                {
                    safeToSet = true;
                    break;
                }
            }

            if (safeToSet)
            {
                //Convert.ChangeType(
                if (pi.PropertyType.BaseType == typeof(Enum))
                {
                    pi.SetValue(this, Enum.Parse(pi.PropertyType, value, true), null);
                }
                else if (pi.PropertyType == typeof(TimeSpan))
                {
                    pi.SetValue(this, TimeSpan.Parse(value), null);
                }
                else if (pi.PropertyType == typeof(Vector3d))
                {
                    pi.SetValue(this, Vector3d.Parse(value), null);
                }
                else
                {
                    pi.SetValue(this, Convert.ChangeType(value, pi.PropertyType), null);
                }
            }


            return safeToSet;
        }

        public bool SetProps(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);


            XmlNode root = doc["LayerApi"];

            XmlNode LayerNode = root["Frame"];
            foreach (XmlAttribute attrib in LayerNode.Attributes)
            {
                if (attrib.Name == "Class")
                {
                    continue;
                }
                if (!SetProp(attrib.Name, attrib.Value))
                {
                    return false;
                }
            }

            return true;
        }


        public string GetProp(string name)
        {
            Type thisType = this.GetType();
            PropertyInfo pi = thisType.GetProperty(name);
            bool safeToGet = false;
            Type layerPropType = typeof(LayerProperty);
            object[] attributes = pi.GetCustomAttributes(false);
            foreach (object var in attributes)
            {
                if (var.GetType() == layerPropType)
                {
                    safeToGet = true;
                    break;
                }
            }

            if (safeToGet)
            {
                return pi.GetValue(this, null).ToString();
            }


            return null;
        }

        public string GetProps()
        {
            MemoryStream ms = new MemoryStream();
            using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, System.Text.Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                xmlWriter.WriteStartElement("LayerApi");
                xmlWriter.WriteElementString("Status", "Success");
                xmlWriter.WriteStartElement("Frame");
                xmlWriter.WriteAttributeString("Class", this.GetType().ToString().Replace("TerraViewer.", ""));


                Type thisType = this.GetType();
                PropertyInfo[] properties = thisType.GetProperties();

                Type layerPropType = typeof(LayerProperty);

                foreach (PropertyInfo pi in properties)
                {
                    bool safeToGet = false;

                    object[] attributes = pi.GetCustomAttributes(false);
                    foreach (object var in attributes)
                    {
                        if (var.GetType() == layerPropType)
                        {
                            safeToGet = true;
                            break;
                        }
                    }

                    if (safeToGet)
                    {
                        xmlWriter.WriteAttributeString(pi.Name, pi.GetValue(this, null).ToString());
                    }

                }
                xmlWriter.WriteEndElement();
                xmlWriter.WriteFullEndElement();
                xmlWriter.Close();

            }
            byte[] data = ms.GetBuffer();
            return Encoding.UTF8.GetString(data);

        }

        public double[] GetParams()
        {
            //todo this is for spherical fixed, need to add orbital..
            double[] paramList = new double[12];
            paramList[0] = MeanRadius;
            paramList[1] = Oblateness;
            paramList[2] = Heading;
            paramList[3] = Pitch;
            paramList[4] = Roll;
            paramList[5] = Scale;
            paramList[6] = Lat;
            paramList[7] = Lng;
            paramList[8] = Altitude;
            paramList[9] = translation.X;
            paramList[10] = translation.Y;
            paramList[11] = translation.Z;

            return paramList;
        }

        public string[] GetParamNames()
        {
            return new string[] 
            { 
                "MeanRadius",
                "Oblateness",
                "Heading", 
                "Pitch", 
                "Roll",
                "Scale",
                "Latitude",
                "Longitude", 
                "Altitude",
                "Translate.X",
                "Translate.Y",
                "Translate.Z"
            };
        }

        public BaseTweenType[] GetParamTypes()
        {
            return new BaseTweenType[]
            { 
                BaseTweenType.Power,
                BaseTweenType.Linear,
                BaseTweenType.Linear, 
                BaseTweenType.Linear,
                BaseTweenType.Linear,
                BaseTweenType.Power,
                BaseTweenType.Linear,
                BaseTweenType.Linear, 
                BaseTweenType.Linear, 
                BaseTweenType.Linear,
                BaseTweenType.Linear,
                BaseTweenType.Linear
            };
        }

        public void SetParams(double[] paramList)
        {
            if (paramList.Length > 8)
            {
                MeanRadius = paramList[0];
                Oblateness = paramList[1];
                Heading = paramList[2];
                Pitch = paramList[3];
                Roll = paramList[4];
                Scale = paramList[5];
                Lat = paramList[6];
                Lng = paramList[7];
                Altitude = paramList[8];
            }

            if (paramList.Length == 12)
            {
                translation.X = paramList[9];
                translation.Y = paramList[10];
                translation.Z = paramList[11];
            }
        }

        public string GetIndentifier()
        {
            return name;
        }

        public string GetName()
        {
            return name;
        }

        public IUiController GetEditUI()
        {
            return null;
        }
    }

    public class TrajectorySample
    {
        public double Time;
        public double X;
        public double Y;
        public double Z;
        public double H;
        public double P;
        public double R;
        public TrajectorySample(double time, double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            Time = time;
        }

        public Vector3d Position
        {
            get
            {
                return new Vector3d(X*1000, Z*1000, Y*1000);
            }
        }

        public TrajectorySample(string line)
        {
            line = line.Replace("  ", " ");
            line = line.Replace("  ", " ");
            line = line.Replace("  ", " ");

            string[] parts = line.Split(new char[] { ' ', '\t', ',' });

            if (parts.Length > 3)
            {
                Time = double.Parse(parts[0]);
                X = double.Parse(parts[1]);
                Y = double.Parse(parts[2]);
                Z = double.Parse(parts[3]);
            }
            if (parts.Length > 6)
            {
                H = double.Parse(parts[4]);
                P = double.Parse(parts[5]);
                R = double.Parse(parts[6]);
            }
        }

        public override string ToString()
        {
            if (H == 0 && P == 0 && R == 0)
            {
                return string.Format("{0} {1} {2} {3}", Time, X, Y, Z);
            }
            else
            {
                return string.Format("{0} {1} {2} {3} {4} {5} {6}", Time, X, Y, Z, H, P, R);
            }

        }
    }
}
