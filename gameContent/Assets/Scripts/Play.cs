using UnityEngine;

public class Play : MonoBehaviour
{
    public bool IsOnPlatform = false;
    
    // Kollar om spelaren är på plattformen "playButton"
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "playButton")
        {
            IsOnPlatform = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "playButton")
        {
            IsOnPlatform = false;
        }
    }


    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOnPlatform == true)
        {
            Debug.Log("Spelaren är på playButton-plattformen");
        }
    }
}

