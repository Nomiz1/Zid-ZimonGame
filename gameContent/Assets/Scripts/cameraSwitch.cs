// This script manages switching between multiple Cinemachine cameras in a Unity scene.

using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class cameraSwitch : MonoBehaviour
{
    static List<CinemachineCamera> cameras = new List<CinemachineCamera>();
    public static CinemachineCamera ActiveCamera = null;
    public static bool IsActiveCamera(CinemachineCamera camera)
    {
        return camera == ActiveCamera;
    }

    public static void SwitchCamera(CinemachineCamera newCamera)
    {
        newCamera.Priority = 10;
        ActiveCamera = newCamera;

        foreach (CinemachineCamera cam in cameras)
        {
            if (cam != newCamera)
            {
                cam.Priority = 0;
            }
        }
    }

    public static void RegisterCamera(CinemachineCamera camera)
    {
        cameras.Add(camera);
    }
    public static void UnregisterCamera(CinemachineCamera camera)
    {
        cameras.Remove(camera);
    }
}
