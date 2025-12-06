// This script handles player movement using keyboard and gamepad input, including movement sound effects.

using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public float startPositionX = 0f;
    public float startPositionY = 0f;
    public float startPositionZ = 0f;
    public float moveSpeed;
    public float sprintSpeed;
    public bool isSprinting = false;
    public bool canMove = true;
    public float gravity = -20f;
    public float jumpForce = 10f;
    private float verticalVelocity = 0f;
    public AudioClip moveSound;
    private AudioSource audioSource;
    bool isMoving = false;


    void Start()
    {
        // Sätt initial position
        verticalVelocity = 0f;
        transform.position = new Vector3(startPositionX, startPositionY, startPositionZ);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // Stoppa spelaren vid marknivå (y=0)
        if (transform.position.y <= 0f && verticalVelocity < 0f)
        {
            Vector3 pos = transform.position;
            pos.y = 0f;
            transform.position = pos;
            verticalVelocity = 0f;
        }
        var keyboard = Keyboard.current;
        var gamepad = Gamepad.current;

        bool jumpPressed = (keyboard != null && keyboard.spaceKey.wasPressedThisFrame) ||
                           (gamepad != null && gamepad.buttonSouth.wasPressedThisFrame);

        // Enkel hopp/gravitation utan ground check
        if (jumpPressed && Mathf.Abs(verticalVelocity) < 0.01f) {
            verticalVelocity = jumpForce;
        } else {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (canMove)
        {
            Vector3 move = HandleInput();
            isMoving = new Vector3(move.x, 0, move.z).sqrMagnitude > 0.01f;
            float speed = isSprinting ? sprintSpeed : moveSpeed;

            // Flytta på markplanet med hastighet, och lägg till vertikal rörelse separat
            Vector3 movement = new Vector3(move.x * speed, verticalVelocity, move.z * speed);
            transform.Translate(movement * Time.deltaTime, Space.World);

            // Spela ljud om spelaren rör sig
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
    }

    Vector3 HandleInput()
    {
        Vector3 move = Vector3.zero;

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            isSprinting = Mouse.current.rightButton.isPressed;
            if (keyboard.wKey.isPressed)
                move.z += 1f;
            if (keyboard.sKey.isPressed)
                move.z -= 1f;
            if (keyboard.aKey.isPressed)
                move.x -= 1f;
            if (keyboard.dKey.isPressed)
                move.x += 1f;
        }

        // Gamepadkontroller
        if (Gamepad.current != null)
        {
            isSprinting = Gamepad.current.buttonWest.isPressed;
            Vector2 moveInput = Gamepad.current.leftStick.ReadValue();
            if (moveInput.sqrMagnitude > 0.01f)
            {
                move.x += moveInput.x;
                move.z += moveInput.y;
            }
        }

        // Normalisera rörelsevektorn så att diagonala rörelser inte är snabbare
        if (move.magnitude > 1f)
            move.Normalize();

        return move;
    }
}
