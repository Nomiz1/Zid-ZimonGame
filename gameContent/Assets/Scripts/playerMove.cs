// This script handles player movement using keyboard and gamepad input, including movement sound effects.

using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER

public class PlayerMove : MonoBehaviour
{
    public float startPositionX = 0f;
    public float startPositionY = 0f;
    public float startPositionZ = 0f;
    public float moveSpeed;
    public float sprintSpeed;
    public bool isSprinting = false;
    public bool canMove = true;
    public AudioClip moveSound;
    private AudioSource audioSource;
    bool isMoving = false;

    void Start()
    {
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
        if (canMove)
        {
            Vector3 move = HandleInput();
            isMoving = move.sqrMagnitude > 0.01f;
            float speed = isSprinting ? sprintSpeed : moveSpeed;
            transform.Translate(move * speed * Time.deltaTime, Space.World);

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

        // Tangentbordskontroller

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
            if (moveInput.sqrMagnitude > 0.01f && !isSprinting)
            {
                move.x += moveInput.x;
                move.z += moveInput.y;
            }
            if (moveInput.sqrMagnitude > 0.01f && isSprinting)
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
#endif
