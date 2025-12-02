using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class switchPerspective : MonoBehaviour
{
    public CinemachineCamera firstPersonCamera;
    public CinemachineCamera thirdPersonCamera;
  

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            cameraSwitch.SwitchCamera(firstPersonCamera);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            cameraSwitch.SwitchCamera(thirdPersonCamera);
        }
    }
}
