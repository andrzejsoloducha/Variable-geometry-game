using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class turnTimer : MonoBehaviour
{
    public float turnTime = 5.0f;
    private float currentTime = 0f;
    public bool actionTaken;
    public int currentPlayer;
    private int maxPlayers;
    private GameObject[] allPlayers;
    public playerActions playerScript;

    [SerializeField] Text timeLeftText;

    void Start()
    {
        currentTime = turnTime;

        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        maxPlayers = allPlayers.Length;

        playerScript = GetComponent<playerActions>();
    }

    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        timeLeftText.text = currentTime.ToString("0");

        

        if (currentTime <= 0 | playerScript.actionTaken)
        {
            EndTurn();
            SwitchPlayer();
        }
    }

    void EndTurn()
    {
        currentTime = turnTime;
        actionTaken = false;
    }

    void SwitchPlayer()
    {
        currentPlayer += 1;

        if (currentPlayer >= maxPlayers)
        {
            currentPlayer = 0;
        }
    }
}

