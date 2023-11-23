using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class turnTimer : MonoBehaviour
{
    public float turnTime = 5.0f;
    private float currentTime = 0f;

    public int currentPlayer = 0;
    private int maxPlayers;
    public GameObject[] allPlayers;
    public GameObject playerScript;
    public playerActions playerSc;

    [SerializeField] Text timeLeftText;

    void Start()
    {
        currentTime = turnTime;

        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        maxPlayers = allPlayers.Length;

        GameObject playerScript = GameObject.Find("playerActions");
        playerActions playerSc = playerScript.GetComponent<playerActions>();
        
    }

    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        timeLeftText.text = currentTime.ToString("0");

        if (playerSc)
        {
            bool actionTaken = playerSc.actionTaken;
        } else
        {
            Debug.Log("No game object called playerSc found");
        }

        if (currentTime <= 0 | playerSc.actionTaken)
        {
            EndTurn();
            SwitchPlayer();
        }
    }

    void EndTurn()
    {
        currentTime = turnTime;
        playerSc.actionTaken = false;
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

