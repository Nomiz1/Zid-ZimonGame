
using UnityEngine;
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
    public float walkSpeed = 6f;
    public float runSpeed = 12f; 
    public float gravity = 20f;
    public float lookSpeed = 10f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;

    private Vector3 moveDirection = Vector3.zero;
    // Ingen rotationX behövs när musrörelse är avstängd
    private CharacterController characterController;

    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // Första person: gör kameran till barn och placera vid huvud. Tredje person: positioneras i Update.
        if (playerCamera != null && !useThirdPerson)
        {
            if (playerCamera.transform.parent != transform)
                playerCamera.transform.SetParent(transform);
            playerCamera.transform.localRotation = Quaternion.identity;
            playerCamera.transform.localPosition = new Vector3(0f, defaultHeight * 0.5f, 0f);
        }
    }

    void Update()
    {
        // WASD-rörelse
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        // Om main menu är aktiv, tillåt ingen rörelse
        if (mainMenu.IsMainMenuActive)
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }
        bool isRunning = GetRunInput();
        float vertical = GetVerticalAxis();
        float horizontal = GetHorizontalAxis();
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * vertical : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * horizontal : 0;
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
            walkSpeed = 12f;
            runSpeed = 12f;
        }
        characterController.Move(moveDirection * Time.deltaTime);

        // Kameran följer efter spelaren
        if (useThirdPerson && playerCamera != null)
        {
            Vector3 thirdPersonOffset = new Vector3(xAxisOffset, yAxisOffset, zAxisOffset);
            Vector3 desiredPos = transform.position + thirdPersonOffset;
            playerCamera.transform.position = desiredPos;
            playerCamera.transform.LookAt(transform.position + Vector3.up * 1.5f);
        }
    }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
    private bool GetRunInput()
    {
        if (Keyboard.current != null)
            return Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed;
        if (Gamepad.current != null)
            return Gamepad.current.leftStickButton.isPressed;
        return false;
    }

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
    private bool GetRunInput() => Input.GetKey(KeyCode.LeftShift);
    private float GetVerticalAxis() => Input.GetAxis("Vertical");
    private float GetHorizontalAxis() => Input.GetAxis("Horizontal");
    // Hopp är inaktiverat
#endif
}