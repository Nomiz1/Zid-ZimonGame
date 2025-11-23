using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Camera playerCamera;
    public bool enableMouseLook = true;
    [Tooltip("Multiplier for mouse look sensitivity. Increase to look faster.")]
    public float mouseSensitivity = 0.05f;
    [Tooltip("Invert Y axis for mouse look.")]
    public bool invertY = false;
    [Tooltip("Offset från spelaren till kameran i tredje person (bakåt och uppåt).")]
    public Vector3 thirdPersonOffset = new Vector3(0f, 2f, -5f);
    [Tooltip("Om true används tredje person, annars första person.")]
    public bool useThirdPerson = true;
    [Tooltip("If true, apply vertical look (pitch) to the player transform instead of only the camera.")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 3f;
    public float gravity = 20f;
    public float lookSpeed = 10f;
    public float lookXLimit = 45f;
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
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
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = GetRunInput();
        float vertical = GetVerticalAxis();
        float horizontal = GetHorizontalAxis();

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * vertical : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * horizontal : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (GetJumpInput() && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        if (GetCrouchInput() && canMove)
        {
            characterController.height = crouchHeight;
            walkSpeed = crouchSpeed;
            runSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 12f;
            runSpeed = 12f;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove && enableMouseLook)
        {
            Vector2 mouseDelta = GetMouseDelta();
            float ySign = invertY ? 1f : -1f;
            float yDelta = mouseDelta.y * mouseSensitivity * ySign * lookSpeed;
            float xDelta = mouseDelta.x * mouseSensitivity * lookSpeed;

            if (useThirdPerson && playerCamera != null)
            {
                // Rotera spelaren i Y-led (vänster/höger) med musen
                transform.rotation *= Quaternion.Euler(0, xDelta, 0);
                // Justera kamerans vinkel (upp/ner) med musen
                rotationX += yDelta;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

                // Beräkna önskad position bakom spelaren
                Quaternion camRot = Quaternion.Euler(rotationX, transform.eulerAngles.y, 0);
                Vector3 desiredPos = transform.position + camRot * thirdPersonOffset;
                playerCamera.transform.position = desiredPos;
                playerCamera.transform.LookAt(transform.position + Vector3.up * 1.5f);
            }
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

    private bool GetJumpInput()
    {
        if (Keyboard.current != null)
            return Keyboard.current.spaceKey.isPressed;
        if (Gamepad.current != null)
            return Gamepad.current.buttonSouth.isPressed;
        return false;
    }

    private bool GetCrouchInput()
    {
        if (Keyboard.current != null)
            return Keyboard.current.rKey.isPressed;
        return false;
    }

    private Vector2 GetMouseDelta()
    {
        if (Mouse.current != null)
        {
            return Mouse.current.delta.ReadValue();
        }
        return Vector2.zero;
    }
#else
    private bool GetRunInput() => Input.GetKey(KeyCode.LeftShift);
    private float GetVerticalAxis() => Input.GetAxis("Vertical");
    private float GetHorizontalAxis() => Input.GetAxis("Horizontal");
    private bool GetJumpInput() => Input.GetButton("Jump");
    private bool GetCrouchInput() => Input.GetKey(KeyCode.R);
    private Vector2 GetMouseDelta() => new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
}