using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class mainMenu : MonoBehaviour
{
    public static bool IsMainMenuActive = false;
    public Camera playerCamera;
    public bool isMainMenu = true;
    public bool canMove = false;
    public bool gravityEnabled = false;
    public float defaultHeight = 2f;

 
    void Start()
    {
        if (isMainMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            canMove = false;
            gravityEnabled = false;
            playerCamera.transform.SetParent(transform);
            playerCamera.transform.localRotation = Quaternion.identity;
            playerCamera.transform.localPosition = new Vector3(0f, defaultHeight * 0.5f, 0f);
            IsMainMenuActive = true;
        }
        else
        {
            IsMainMenuActive = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (isMainMenu)
        {
            // Nollställ all rörelse
            if (TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}

