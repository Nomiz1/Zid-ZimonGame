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



       void Start() 
    {
        playerPosition = new Vector3(positionX, positionY, positionZ);
        transform.position = playerPosition; // Sätt initial position
    }

    void Update()
    {

        if (canMove)
        {
            // Tangentbordskontroller
            if (UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.wKey.isPressed)
                    
                    playerPosition.z += moveSpeed * Time.deltaTime;

                if (UnityEngine.InputSystem.Keyboard.current.sKey.isPressed)
                    
                    playerPosition.z -= moveSpeed * Time.deltaTime;

                if (UnityEngine.InputSystem.Keyboard.current.aKey.isPressed)
                    
                    playerPosition.x -= moveSpeed * Time.deltaTime;

                if (UnityEngine.InputSystem.Keyboard.current.dKey.isPressed)
                    
                    playerPosition.x += moveSpeed * Time.deltaTime;
            }
            // Gamepadkontroller
            if (UnityEngine.InputSystem.Gamepad.current != null)
            {
                Vector2 moveInput = UnityEngine.InputSystem.Gamepad.current.leftStick.ReadValue();
                playerPosition.x += moveInput.x * moveSpeed * Time.deltaTime;
                playerPosition.z += moveInput.y * moveSpeed * Time.deltaTime;
            }
            transform.position = playerPosition;


        }

       


#endif
        
    }
       
}