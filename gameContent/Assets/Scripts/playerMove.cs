using JetBrains.Annotations;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER // Använd den nya Input System-paketet

public class playerMove: MonoBehaviour 
{
    public Vector3 playerPosition;
    public float positionX = 0f;
    public float positionY = 0f;
    public float positionZ = 0f;
    public float moveSpeed = 5f;
    public bool canMove = true;
    public AudioClip moveSound;
    private AudioSource audioSource;
    bool isMoving = false;



    void Start() 
    {
        playerPosition = new Vector3(positionX, positionY, positionZ);
        transform.position = playerPosition; // Sätt initial position
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {

        if (canMove)
        {
            isMoving = false;
            // Tangentbordskontroller
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed)
                {
                    playerPosition.z += moveSpeed * Time.deltaTime;
                    isMoving = true;
                }
                if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed)
                {
                    playerPosition.z -= moveSpeed * Time.deltaTime;
                    isMoving = true;
                }
                if (UnityEngine.InputSystem.Keyboard.current.aKey.isPressed)
                {
                    playerPosition.x -= moveSpeed * Time.deltaTime;
                    isMoving = true;
                }
                if (UnityEngine.InputSystem.Keyboard.current.dKey.isPressed)
                {
                    playerPosition.x += moveSpeed * Time.deltaTime;
                    isMoving = true;
                }
            }
            // Gamepadkontroller
            if (UnityEngine.InputSystem.Gamepad.current != null)
            {
                Vector2 moveInput = UnityEngine.InputSystem.Gamepad.current.leftStick.ReadValue();
                if (moveInput.sqrMagnitude > 0.01f)
                {
                    playerPosition.x += moveInput.x * moveSpeed * Time.deltaTime;
                    playerPosition.z += moveInput.y * moveSpeed * Time.deltaTime;
                    isMoving = true;
                }
            }
            transform.position = playerPosition;

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

       


#endif
        
    }
       
}