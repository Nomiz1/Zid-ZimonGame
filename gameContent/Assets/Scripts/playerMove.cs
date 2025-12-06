
// This script handles player movement using keyboard and gamepad input, including movement sound effects.

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviour
{
    // Fält för att lagra input mellan Update och FixedUpdate
    private Vector3 moveInput;
    private float currentSpeed;
    private bool jumpPressedFlag;
    private Rigidbody rb;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundMask = ~0; // Alla lager som standard
    private bool isGrounded;
    public float startPositionX = 0f;
    public float startPositionY = 0f;
    public float startPositionZ = 0f;
    public float moveSpeed;
    public float sprintSpeed;
    public bool isSprinting = false;
    public bool canMove = true;
    public float gravity = -20f;
    public float jumpForce = 10f;
    public AudioClip moveSound;
    private AudioSource audioSource;
    bool isMoving = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.freezeRotation = true;

        // Sätt initial position
        transform.position = new Vector3(startPositionX, startPositionY, startPositionZ);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Kolla om spelaren är på marken
        isGrounded = CheckIfGrounded();

        // Läs in all input
        ReadInput();

        // Spela ljud om spelaren rör sig
        isMoving = new Vector3(moveInput.x, 0, moveInput.z).sqrMagnitude > 0.01f;
        HandleMoveSound();
    }

    // Läser in all input och sparar i fält
    void ReadInput()
    {
        var keyboard = Keyboard.current;
        var gamepad = Gamepad.current;

        // Rörelseinput
        moveInput = Vector3.zero;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed) moveInput.z += 1f;
            if (keyboard.sKey.isPressed) moveInput.z -= 1f;
            if (keyboard.aKey.isPressed) moveInput.x -= 1f;
            if (keyboard.dKey.isPressed) moveInput.x += 1f;
            isSprinting = Mouse.current != null && Mouse.current.rightButton.isPressed;
        }
        if (gamepad != null)
        {
            Vector2 stick = gamepad.leftStick.ReadValue();
            moveInput.x += stick.x;
            moveInput.z += stick.y;
            isSprinting = gamepad.buttonWest.isPressed;
        }
        if (moveInput.magnitude > 1f) moveInput.Normalize();

        // Hoppinput
        bool jumpKey = false;
        bool jumpPad = false;
        if (keyboard != null)
            jumpKey = keyboard.spaceKey.wasPressedThisFrame;
        if (gamepad != null)
            jumpPad = gamepad.buttonSouth.wasPressedThisFrame;
        jumpPressedFlag = jumpKey || jumpPad;

        // Sätt aktuell hastighet
        currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
    }
    private bool CheckIfGrounded()
        {
        float rayLength = 0.5f; // Standardvärde, kan justeras i editorn
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            rayLength = col.bounds.extents.y + groundCheckDistance;
        }
        return Physics.Raycast(transform.position, Vector3.down, rayLength, groundMask);
        }

        // Hanterar ljud när spelaren rör sig
    private void HandleMoveSound()
        {
            if (isMoving && moveSound != null)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = moveSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.isPlaying && audioSource.clip == moveSound)
                    audioSource.Stop();
            }
        }
    void FixedUpdate()
    {
        // Rigidbody och freezeRotation hanteras i Start()

        Vector3 velocity = new Vector3(moveInput.x * currentSpeed, rb.linearVelocity.y, moveInput.z * currentSpeed);

        // Hoppa
        if (jumpPressedFlag && isGrounded)
        {
            velocity.y = jumpForce;
            jumpPressedFlag = false; // Nollställ så att vi inte hoppar flera gånger på samma knapptryck
        }

        rb.linearVelocity = velocity;

    }
}

    