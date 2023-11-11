using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class turnTimer : MonoBehaviour
{
    public float turnTime = 5.0f;
    private float currentTime = 0f;
    private bool actionTaken;

    [SerializeField] Text timeLeftText;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = turnTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        timeLeftText.text = currentTime.ToString("0");

        actionTaken = playerActions.actionTaken
        if (currentTime <= 0)
        {
            currentTime = 0;
        }
    }
}
