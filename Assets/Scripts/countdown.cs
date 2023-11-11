using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class countdown : MonoBehaviour
{
    public float startTime = 5.0f;
    private float currentTime = 0f;

    [SerializeField] Text timeLeftText;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        startTime -= 1 * Time.deltaTime;
        timeLeftText.text = startTime.ToString("0");
    }
}
