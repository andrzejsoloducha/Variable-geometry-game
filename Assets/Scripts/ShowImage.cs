using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowImage : MonoBehaviour
{
    public Image imageToShow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Show the image
            imageToShow.enabled = true;
        }
        // Optionally, hide the image when the key is released
        else if (Input.GetKeyUp(KeyCode.P))
        {
            imageToShow.enabled = false;
        }
    }
}
