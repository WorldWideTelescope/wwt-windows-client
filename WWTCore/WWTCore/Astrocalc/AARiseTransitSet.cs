using System;
//
//Module : AARISETRANSITSET.CPP
//Purpose: Implementation for the algorithms which obtain the Rise, Transit and Set times
//Created: PJN / 29-12-2003
//History: PJN / 15-10-2004 1. bValid variable is now correctly set in CAARiseTransitSet::Rise if the
//                          objects does actually rise and sets
//
//Copyright (c) 2003 - 2007 by PJ Naughter (Web: www.naughter.com, Email: pjna@naughter.com)
//
//All rights reserved.
//
//Copyright / Usage Details:
//
//You are allowed to include the source code in any product (commercial, shareware, freeware or otherwise) 
//when your product is released in binary form. You are allowed to modify the source code in any way you want 
//except you cannot modify the copyright details at the top of each module. If you want to distribute source 
//code with your application, then you are only allowed to distribute versions released by the author. This is 
//to maintain a single distribution point for the source code. 
//
//
// Converted to c# and distributed with WWT by permision of PJ Naughter by Jonathan Fay
// Please refer to http://www.naughter.com/aa.html for orginal C++ versions
//

///////////////////////////// Includes ////////////////////////////////////////


/////////////////////// Classes ///////////////////////////////////////////////

public class CAARiseTransitSetDetails
{
    //Constructors / Destructors
    public CAARiseTransitSetDetails()
    {
        RiseValid = false;
        SetValid = false;
        TransitValid = false;
        Rise = 0;
        Transit = 0;
        Set = 0;
    }

    //Member variables
    public double Rise;
    public double Transit;
    public double Set;
    internal bool RiseValid;
    internal bool SetValid;
    internal bool TransitAboveHorizon;
    internal bool TransitValid;
}

public class CAARiseTransitSet
{
    //Static methods

    ///////////////////////////// Implementation //////////////////////////////////


    static double ConstraintM(double M)
    {
        while (M > 1)
            M -= 1;
        while (M < 0)
            M += 1;

        return M;
    }

    static double CalculateTransit(double Alpha2, double theta0, double Longitude)
    {
        //Calculate and ensure the M0 is in the range 0 to +1
        double M0 = (Alpha2 * 15 + Longitude - theta0) / 360;
        M0 = ConstraintM(M0);

        return M0;
    }

    static void CalculateRiseSet(double M0, double cosH0, CAARiseTransitSetDetails details, ref double M1, ref double M2)
    {
        M1 = 0;
        M2 = 0;

        if ((cosH0 > -1) && (cosH0 < 1))
        {
            details.RiseValid = true;
            details.SetValid = true;
            details.TransitAboveHorizon = true;

            double H0 = Math.Acos(cosH0);
            H0 = CAACoordinateTransformation.RadiansToDegrees(H0);

            //Calculate and ensure the M1 and M2 is in the range 0 to +1
            M1 = M0 - H0 / 360;
            M2 = M0 + H0 / 360;

            M1 = ConstraintM(M1);
            M2 = ConstraintM(M2);
        }
        else if (cosH0 < 1)
        {
            details.TransitAboveHorizon = true;
        }
    }

    static void CorrectRAValuesForInterpolation(ref double Alpha1, ref double Alpha2, ref double Alpha3)
    {
        //Ensure the RA values are corrected for interpolation. Due to important Remark 2 by Meeus on Interopolation of RA values
        Alpha1 = CAACoordinateTransformation.MapTo0To24Range(Alpha1);
        Alpha2 = CAACoordinateTransformation.MapTo0To24Range(Alpha2);
        Alpha3 = CAACoordinateTransformation.MapTo0To24Range(Alpha3);
        if (Math.Abs(Alpha2 - Alpha1) > 12.0)
        {
            if (Alpha2 > Alpha1)
                Alpha1 += 24;
            else
                Alpha2 += 24;
        }
        if (Math.Abs(Alpha3 - Alpha2) > 12.0)
        {
            if (Alpha3 > Alpha2)
                Alpha2 += 24;
            else
                Alpha3 += 24;
        }
        if (Math.Abs(Alpha2 - Alpha1) > 12.0)
        {
            if (Alpha2 > Alpha1)
                Alpha1 += 24;
            else
                Alpha2 += 24;
        }
        if (Math.Abs(Alpha3 - Alpha2) > 12.0)
        {
            if (Alpha3 > Alpha2)
                Alpha2 += 24;
            else
                Alpha3 += 24;
        }
    }

    static void CalculateRiseHelper(CAARiseTransitSetDetails details, double theta0, double deltaT, double Alpha1, double Delta1, double Alpha2, double Delta2, double Alpha3, double Delta3, double Longitude, double Latitude, double LatitudeRad, double h0, ref double M1)
    {
        for (int i = 0; i < 2; i++)
        {
            //Calculate the details of rising
            if (details.RiseValid)
            {
                double theta1 = theta0 + 360.985647 * M1;
                theta1 = CAACoordinateTransformation.MapTo0To360Range(theta1);

                double n = M1 + deltaT / 86400;

                double Alpha = CAAInterpolate.Interpolate(n, Alpha1, Alpha2, Alpha3);
                double Delta = CAAInterpolate.Interpolate(n, Delta1, Delta2, Delta3);

                double H = theta1 - Longitude - Alpha * 15;
                CAA2DCoordinate Horizontal = CAACoordinateTransformation.Equatorial2Horizontal(H / 15, Delta, Latitude);

                double DeltaM = (Horizontal.Y - h0) / (360 * Math.Cos(CAACoordinateTransformation.DegreesToRadians(Delta)) * Math.Cos(LatitudeRad) * Math.Sin(CAACoordinateTransformation.DegreesToRadians(H)));
                M1 += DeltaM;

                if ((M1 < 0) || (M1 >= 1))
                    details.RiseValid = false;
            }
        }
    }

    static void CalculateSetHelper(CAARiseTransitSetDetails details, double theta0, double deltaT, double Alpha1, double Delta1, double Alpha2, double Delta2, double Alpha3, double Delta3, double Longitude, double Latitude, double LatitudeRad, double h0, ref double M2)
    {
        for (int i = 0; i < 2; i++)
        {
            //Calculate the details of setting
            if (details.SetValid)
            {
                double theta1 = theta0 + 360.985647 * M2;
                theta1 = CAACoordinateTransformation.MapTo0To360Range(theta1);

                double n = M2 + deltaT / 86400;

                double Alpha = CAAInterpolate.Interpolate(n, Alpha1, Alpha2, Alpha3);
                double Delta = CAAInterpolate.Interpolate(n, Delta1, Delta2, Delta3);

                double H = theta1 - Longitude - Alpha * 15;
                CAA2DCoordinate Horizontal = CAACoordinateTransformation.Equatorial2Horizontal(H / 15, Delta, Latitude);

                double DeltaM = (Horizontal.Y - h0) / (360 * Math.Cos(CAACoordinateTransformation.DegreesToRadians(Delta)) * Math.Cos(LatitudeRad) * Math.Sin(CAACoordinateTransformation.DegreesToRadians(H)));
                M2 += DeltaM;

                if ((M2 < 0) || (M2 >= 1))
                    details.SetValid = false;
            }
        }
    }

    static void CalculateTransitHelper(CAARiseTransitSetDetails details, double theta0, double deltaT, double Alpha1, double Alpha2, double Alpha3, double Longitude, ref double M0)
    {
        for (int i = 0; i < 2; i++)
        {
            //Calculate the details of transit
            if (details.TransitValid)
            {
                double theta1 = theta0 + 360.985647 * M0;
                theta1 = CAACoordinateTransformation.MapTo0To360Range(theta1);

                double n = M0 + deltaT / 86400;

                double Alpha = CAAInterpolate.Interpolate(n, Alpha1, Alpha2, Alpha3);

                double H = theta1 - Longitude - Alpha * 15;
                H = CAACoordinateTransformation.MapTo0To360Range(H);
                if (H > 180)
                    H -= 360;

                double DeltaM = -H / 360;
                M0 += DeltaM;

                if (M0 < 0 || M0 >= 1)
                    details.TransitValid = false;
            }
        }
    }

    /// <summary>
    /// Updated to use new Code from CAA 
    /// </summary>
    /// <param name="JD"></param>
    /// <param name="Alpha1"></param>
    /// <param name="Delta1"></param>
    /// <param name="Alpha2"></param>
    /// <param name="Delta2"></param>
    /// <param name="Alpha3"></param>
    /// <param name="Delta3"></param>
    /// <param name="Longitude"></param>
    /// <param name="Latitude"></param>
    /// <param name="h0"></param>
    /// <returns></returns>
    public static CAARiseTransitSetDetails Compute(double JD, double Alpha1, double Delta1, double Alpha2, double Delta2, double Alpha3, double Delta3, double Longitude, double Latitude, double h0)
    {
        //What will be the return value
        CAARiseTransitSetDetails details = new CAARiseTransitSetDetails();
        details.RiseValid = false;
        details.SetValid = false;
        details.TransitValid = true;
        details.TransitAboveHorizon = false;

        //Calculate the sidereal time
        double theta0 = CAASidereal.ApparentGreenwichSiderealTime(JD);
        theta0 *= 15; //Express it as degrees

        //Calculate deltat
        double deltaT = CAADynamicalTime.DeltaT(JD);

        //Convert values to radians
        double Delta2Rad = CAACoordinateTransformation.DegreesToRadians(Delta2);
        double LatitudeRad = CAACoordinateTransformation.DegreesToRadians(Latitude);

        //Convert the standard latitude to radians
        double h0Rad = CAACoordinateTransformation.DegreesToRadians(h0);

        //Calculate cosH0
        double cosH0 = (Math.Sin(h0Rad) - Math.Sin(LatitudeRad) * Math.Sin(Delta2Rad)) / (Math.Cos(LatitudeRad) * Math.Cos(Delta2Rad));

        //Calculate M0
        double M0 = CalculateTransit(Alpha2, theta0, Longitude);

        //Calculate M1 & M2
        double M1 = 0;
        double M2 = 0;
        CalculateRiseSet(M0, cosH0, details, ref M1, ref M2);

        //Ensure the RA values are corrected for interpolation. Due to important Remark 2 by Meeus on Interopolation of RA values
        CorrectRAValuesForInterpolation(ref Alpha1, ref Alpha2, ref Alpha3);

        //Do the main work
        CalculateTransitHelper(details, theta0, deltaT, Alpha1, Alpha2, Alpha3, Longitude, ref M0);
        CalculateRiseHelper(details, theta0, deltaT, Alpha1, Delta1, Alpha2, Delta2, Alpha3, Delta3, Longitude, Latitude, LatitudeRad, h0, ref M1);
        CalculateSetHelper(details, theta0, deltaT, Alpha1, Delta1, Alpha2, Delta2, Alpha3, Delta3, Longitude, Latitude, LatitudeRad, h0, ref M2);

        details.Rise = details.RiseValid ? (M1 * 24) : 0.0;
        details.Set = details.SetValid ? (M2 * 24) : 0.0;
        details.Transit = details.TransitValid ? (M0 * 24) : 0.0;

        return details;
    }
}
