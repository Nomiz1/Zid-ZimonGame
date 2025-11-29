// ...existing code...
using UnityEngine;

public class playerCamera : MonoBehaviour
{
    public float cameraoffsetX = 0f;
    public float cameraoffsetY = 2f;
    public float cameraoffsetZ = -5f;
    public float positionX = 0f;
    public float positionY = 2f;
    public float positionZ = 0f;
    public bool useThirdPerson = true;
    public bool useFirstPerson = false;

    void Update()
    {
        // Kameran är alltid barn till spelaren
        if (transform.parent == null) return;

        if (useFirstPerson)
        {
            // Första person: placera kameran vid huvud (lokal position)
            transform.localPosition = new Vector3(positionX, positionY, positionZ);
            transform.localRotation = Quaternion.identity;
        }
        else if (useThirdPerson)
        {
            // Tredje person: placera kameran med offset bakom spelaren (lokal position)
            transform.localPosition = new Vector3(cameraoffsetX, cameraoffsetY, cameraoffsetZ);
            // Titta på spelaren (valfritt, kan tas bort om du vill ha fri kamera)
            transform.LookAt(transform.parent.position + Vector3.up);
        }
    }
}







