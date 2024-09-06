using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtils
{
    ///<summary>
    /// [-1,1]      [1,1]
    ///       [0,0]
    /// [-1,-1]     [1,-1]
    ///</summary>
    public static Vector3 GetCameraCenterDirection(float imageWidth,float imageHeight,float fov,Quaternion rotation)
    {
        float x = imageWidth/2f - 0.5f;
        float y = imageHeight/2f - 0.5f;
        var M_PI = Mathf.PI;
        var half =  Mathf.Tan(fov / 2 * M_PI / 180);
        float imageAspectRatio = imageWidth / imageHeight; // assuming width > height 
        float Px = (2 * ((x + 0.5f) / imageWidth) - 1) *  half* imageAspectRatio; 
        float Py = (1 - 2 * ((y + 0.5f) / imageHeight)) * half; 

        Vector3 rayOrigin =new Vector3(0, 0, 0); 
        Matrix4x4 cameraToWorld = Matrix4x4.TRS(Vector3.zero,rotation,new Vector3(1,1,-1)); 
        Vector3 rayOriginWorld, rayPWorld; 
        rayOriginWorld = cameraToWorld.MultiplyVector(rayOrigin); 
        rayPWorld = cameraToWorld.MultiplyVector(new Vector3(Px, Py, -1)); 
        Vector3 rayDirection = rayPWorld - rayOriginWorld; 
        rayDirection.Normalize();
        return rayDirection;
    }
}
