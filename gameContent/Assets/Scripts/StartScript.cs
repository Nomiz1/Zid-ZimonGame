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
    [Tooltip("Local offset of the camera relative to the player (x,y,z).")]
    public Vector3 cameraOffset = new Vector3(0f, 1f, 0f);
    [Tooltip("If true, apply vertical look (pitch) to the player transform instead of only the camera.")]
    public bool pitchPlayer = false;
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
        // Ensure the camera is a child of the player and locked to the player's local position
        if (playerCamera != null)
        {
            if (playerCamera.transform.parent != transform)
                playerCamera.transform.SetParent(transform);
            playerCamera.transform.localRotation = Quaternion.identity;
            // Use configurable offset; default keeps camera at roughly eye level
            playerCamera.transform.localPosition = cameraOffset != Vector3.zero ? cameraOffset : new Vector3(0f, defaultHeight * 0.5f, 0f);
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
            if (playerCamera != null)
                playerCamera.transform.localPosition = new Vector3(0f, crouchHeight * 0.5f, 0f);
        }
        else
        {
            characterController.height = defaultHeight;
            walkSpeed = 12f;
            runSpeed = 12f;
            if (playerCamera != null)
                playerCamera.transform.localPosition = new Vector3(0f, defaultHeight * 0.5f, 0f);
        }

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove && enableMouseLook)
        {
            Vector2 mouseDelta = GetMouseDelta();
            float ySign = invertY ? 1f : -1f;
            float yDelta = mouseDelta.y * mouseSensitivity * ySign * lookSpeed;
            float xDelta = mouseDelta.x * mouseSensitivity * lookSpeed;

            if (pitchPlayer)
            {
                // Apply yaw first
                transform.rotation *= Quaternion.Euler(0, xDelta, 0);
                // Apply pitch to the player transform
                rotationX += yDelta;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                float yaw = transform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(rotationX, yaw, 0);
                if (playerCamera != null)
                    playerCamera.transform.localRotation = Quaternion.identity;
            }
            else
            {
                rotationX += yDelta;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                if (playerCamera != null)
                    playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
                transform.rotation *= Quaternion.Euler(0, xDelta, 0);
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