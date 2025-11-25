
using UnityEngine;


public class mainMenu : MonoBehaviour
{
    public static bool IsMainMenuActive = false;
    public Camera playerCamera;
    public float defaultHeight = 2f;

 
    void Start()
    {
        if (IsMainMenuActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //canMove = false;
            playerCamera.transform.SetParent(transform);
            playerCamera.transform.localRotation = Quaternion.identity;
            playerCamera.transform.localPosition = new Vector3(0f, defaultHeight * 0.5f, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsMainMenuActive)
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

