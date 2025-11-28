
using UnityEngine;

public class playerCamera : MonoBehaviour
{
    public float cameraoffsetX = 0f;
    public float cameraoffsetY = 2f;
    public float cameraoffsetZ = -5f;
    public float positionX = 0f;
    public float positionY = 2f;
    public float positionZ = 0f;
    public bool useThirdPerson = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!useThirdPerson)
        {
            transform.SetParent(transform.parent);
            transform.localRotation = Quaternion.identity;
            transform.localPosition = new Vector3(positionX, positionY, positionZ);
        }
        else
        {
            transform.SetParent(transform.parent);
            Vector3 cameraOffset = new Vector3(cameraoffsetX, cameraoffsetY, cameraoffsetZ);
            transform.position = transform.parent.position + cameraOffset; // Positionera kameran med offset
            transform.LookAt(transform.position, Vector3.up * 1.5f); // Rikta kameran mot spelaren
            transform.localRotation = Quaternion.identity; // Nollställ rotationen
            
        }

       


    }
}



