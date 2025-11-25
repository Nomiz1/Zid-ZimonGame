
using UnityEngine;
using UnityEngine.AI;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public float xAxisOffset = 0f;
    [Tooltip("Offset från spelaren till kameran i tredje person (höger och vänster).")]
    public float yAxisOffset = 2f;
    public float zAxisOffset = -5f;
    // Offset beräknas dynamiskt i Update
    public bool useThirdPerson = true;
    [Tooltip("Om true används tredje person")]
    public bool useFirstPerson;
    public bool canMove = true;
    public float walkSpeed = 6f;
    public float gravity = 20f;
    public float defaultHeight = 2f;
    private float curSpeedX = 0f;
    private float curSpeedY = 0f;

    private Vector3 moveDirection = Vector3.zero;
    // Ingen rotationX behövs när musrörelse är avstängd
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Endast en vy kan vara aktiv
        if (useFirstPerson) useThirdPerson = false;
        if (useThirdPerson) useFirstPerson = false;

        if (playerCamera != null)
        {
            if (useFirstPerson)
            {
                playerCamera.transform.SetParent(transform);
                playerCamera.transform.localRotation = Quaternion.identity;
                playerCamera.transform.localPosition = new Vector3(0f, defaultHeight * 0.5f, 0f);
            }
            // Tredje person: kameran positioneras i Update
        }
    }

    void Update()
    {
        if (mainMenu.IsMainMenuActive)
        {
            return; // Ingen rörelse när huvudmenyn är aktiv
        }
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        float vertical = GetVerticalAxis();
        float horizontal = GetHorizontalAxis();
        if (!canMove)
        {
            curSpeedX = 0f;
            curSpeedY = 0f;
        }
        else
        {
            curSpeedX = walkSpeed * vertical;
            curSpeedY = walkSpeed * horizontal;
        }
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);
        // Hopp är inaktiverat
        moveDirection.y = movementDirectionY;
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            characterController.height = defaultHeight;
        }
        characterController.Move(moveDirection * Time.deltaTime);

        // Kameran följer efter spelaren
        if (useThirdPerson && playerCamera != null)
        {
            playerCamera.transform.SetParent(null); // Frikoppla från spelaren
            Vector3 thirdPersonOffset = new Vector3(xAxisOffset, yAxisOffset, zAxisOffset);
            Vector3 desiredPos = transform.position + thirdPersonOffset;
            playerCamera.transform.position = desiredPos;
            playerCamera.transform.LookAt(transform.position + Vector3.up * 1.5f);
        }
        else if (useFirstPerson && playerCamera != null)
        {
            // Se till att kameran är barn till spelaren och placerad rätt
            if (playerCamera.transform.parent != transform)
                playerCamera.transform.SetParent(transform);
            playerCamera.transform.localPosition = new Vector3(0f, defaultHeight * 0.5f, 0f);
            playerCamera.transform.localRotation = Quaternion.identity;
        }
    }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
   
    private float GetVerticalAxis()
    {
        float v = 0f;
        if (Keyboard.current != null)
        {
            v += Keyboard.current.wKey.isPressed ? 1f : 0f;
            v -= Keyboard.current.sKey.isPressed ? 1f : 0f;
        }
        if (Gamepad.current != null)
        {
            v += Gamepad.current.leftStick.ReadValue().y;
        }
        return Mathf.Clamp(v, -1f, 1f);
    }

    private float GetHorizontalAxis()
    {
        float h = 0f;
        if (Keyboard.current != null)
        {
            h += Keyboard.current.dKey.isPressed ? 1f : 0f;
            h -= Keyboard.current.aKey.isPressed ? 1f : 0f;
        }
        if (Gamepad.current != null)
        {
            h += Gamepad.current.leftStick.ReadValue().x;
        }
        return Mathf.Clamp(h, -1f, 1f);
    }
#else
    private float GetVerticalAxis() => Input.GetAxis("Vertical");
    private float GetHorizontalAxis() => Input.GetAxis("Horizontal");
    // Hopp är inaktiverat
#endif
}