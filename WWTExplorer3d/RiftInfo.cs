namespace TerraViewer
{
    using System;

    public struct RiftInfo
    {
        // Size of the entire screen, in pixels.
        public UInt32 HResolution;
        public UInt32 VResolution;
        // Physical dimensions of the active screen in meters. Can be used to calculate
        // projection center while considering IPD.
        public float HScreenSize;
        public float VScreenSize;
        // Physical offset from the top of the screen to the eye center, in meters.
        // This will usually, but not necessarily be half of VScreenSize.
        public float VScreenCenter;
        // Distance from the eye to screen surface, in meters.
        // Useful for calculating FOV and projection.
        public float EyeToScreenDistance;
        // Distance between physical lens centers useful for calculating distortion center.
        public float LensSeparationDistance;
        // Configured distance between the user's eye centers, in meters. Defaults to 0.064.
        public float InterpupillaryDistance;

        // Radial distortion correction coefficients.
        // The distortion assumes that the input texture coordinates will be scaled
        // by the following equation:    
        //   uvResult = uvInput * (K0 + K1 * uvLength^2 + K2 * uvLength^4)
        // Where uvInput is the UV vector from the center of distortion in direction
        // of the mapped pixel, uvLength is the magnitude of that vector, and uvResult
        // the corresponding location after distortion.
        public float DistortionK0;
        public float DistortionK1;
        public float DistortionK2;
        public float DistortionK3;

        public float ChromaAbCorrection0;
        public float ChromaAbCorrection1;
        public float ChromaAbCorrection2;
        public float ChromaAbCorrection3;

        // Desktop coordinate position of the screen (can be negative; may not be present on all platforms)
        public int DesktopX;
        public int DesktopY;
    }
}
