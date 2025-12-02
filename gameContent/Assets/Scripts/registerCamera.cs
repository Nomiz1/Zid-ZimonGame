using UnityEngine;
using Unity.Cinemachine;

public class registerCamera : MonoBehaviour
{
    private void OnEnable()
    {
        cameraSwitch.RegisterCamera(GetComponent<CinemachineCamera>());
    }
    private void OnDisable()
    {
        cameraSwitch.UnregisterCamera(GetComponent<CinemachineCamera>());
    }
}
