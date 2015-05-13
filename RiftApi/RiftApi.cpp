// RiftApi.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "RiftApi.h"


#include "OVR.h"
#include "Kernel/OVR_String.h"


using namespace OVR;


Ptr<DeviceManager> pManager;
Ptr<HMDDevice>     pHMD;
Ptr<SensorDevice>  pSensor;

SensorFusion       SFusion;

HMDInfo hmdInfo;

extern "C" struct RiftInfo
{

    // Size of the entire screen, in pixels.
    unsigned  HResolution;
    unsigned  VResolution; 
    // Physical dimensions of the active screen in meters. Can be used to calculate
    // projection center while considering IPD.
    float     HScreenSize;
	float	  VScreenSize;
    // Physical offset from the top of the screen to the eye center, in meters.
    // This will usually, but not necessarily be half of VScreenSize.
    float     VScreenCenter;
    // Distance from the eye to screen surface, in meters.
    // Useful for calculating FOV and projection.
    float     EyeToScreenDistance;
    // Distance between physical lens centers useful for calculating distortion center.
    float     LensSeparationDistance;
    // Configured distance between the user's eye centers, in meters. Defaults to 0.064.
    float     InterpupillaryDistance;
    
    // Radial distortion correction coefficients.
    // The distortion assumes that the input texture coordinates will be scaled
    // by the following equation:    
    //   uvResult = uvInput * (K0 + K1 * uvLength^2 + K2 * uvLength^4)
    // Where uvInput is the UV vector from the center of distortion in direction
    // of the mapped pixel, uvLength is the magnitude of that vector, and uvResult
    // the corresponding location after distortion.
    float DistortionK0;  
	float DistortionK1;
    float DistortionK2;
    float DistortionK3;

    float ChromaAbCorrection0;    
	float ChromaAbCorrection1;
    float ChromaAbCorrection2;
    float ChromaAbCorrection3;


    // Desktop coordinate position of the screen (can be negative; may not be present on all platforms)
    int     DesktopX;
	int		DesktopY;
};

extern "C" RIFTAPI_API int InitRiftApi()
{
	OVR::System::Init();
	pManager = *DeviceManager::Create();

	pHMD     = *pManager->EnumerateDevices<HMDDevice>().CreateDevice();
	if (!pHMD)
	{
		return 0 ;
	}

	pSensor  = *pHMD->GetSensor();

	// Get DisplayDeviceName, ScreenWidth/Height, etc..

	pHMD->GetDeviceInfo(&hmdInfo);

	if (pSensor)
	{
		SFusion.AttachToSensor(pSensor);
		SFusion.SetPrediction(0.03f);
	}

	if(pSensor)
	{
		return 1;
	}

	return 0;
}

float Heading = 0;
float Pitch = 0;
float Roll = 0;

extern "C" RIFTAPI_API int GetSensorSample()
{
   
	SFusion.GetOrientation().GetEulerAngles<Axis_Y, Axis_X, Axis_Z, Rotate_CCW, Handed_R>(&Heading, &Pitch, &Roll);
	

	return 1;
}


extern "C" RIFTAPI_API bool ResetRift()
{
	SFusion.Reset();
	return true;
}

extern "C" RIFTAPI_API float GetHeading()
{
	return Heading;
}

extern "C" RIFTAPI_API float GetPitch()
{
	return Pitch;
}

extern "C" RIFTAPI_API float GetRoll()
{
	return Roll;
}

extern "C" RIFTAPI_API char* GetDisplayName()
{
	return hmdInfo.DisplayDeviceName;
}

extern "C" RIFTAPI_API void GetRiftInfo(RiftInfo* riftInfo)
{
	riftInfo->DesktopX = hmdInfo.DesktopX;
	riftInfo->HResolution = hmdInfo.HResolution;
	riftInfo->VResolution = hmdInfo.VResolution;
	riftInfo->HScreenSize = hmdInfo.HScreenSize;
	riftInfo->VScreenSize = hmdInfo.VScreenSize;
	riftInfo->VScreenCenter = hmdInfo.VScreenCenter;
	riftInfo->EyeToScreenDistance = hmdInfo.EyeToScreenDistance;
	riftInfo->LensSeparationDistance = hmdInfo.LensSeparationDistance;
	riftInfo->InterpupillaryDistance = hmdInfo.InterpupillaryDistance;
	riftInfo->DistortionK0 = hmdInfo.DistortionK[0];
	riftInfo->DistortionK1 = hmdInfo.DistortionK[1];
	riftInfo->DistortionK2 = hmdInfo.DistortionK[2];
	riftInfo->DistortionK3 = hmdInfo.DistortionK[3];

	riftInfo->ChromaAbCorrection0 = hmdInfo.ChromaAbCorrection[0];
	riftInfo->ChromaAbCorrection1 = hmdInfo.ChromaAbCorrection[1];
	riftInfo->ChromaAbCorrection2 = hmdInfo.ChromaAbCorrection[2];
	riftInfo->ChromaAbCorrection3 = hmdInfo.ChromaAbCorrection[3];

	return ;
}

extern "C" RIFTAPI_API int CloseRiftApi(void)
{
	pSensor.Clear();
    pManager.Clear();
    OVR::System::Destroy();
	return 1;
}


//// This is the constructor of a class that has been exported.
//// see RiftApi.h for the class definition
//CRiftApi::CRiftApi()
//{
//	return;
//}



