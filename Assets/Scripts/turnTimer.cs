using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class turnTimer : MonoBehaviour
{
    private float turnTime = 10.0f;
    private float currentTime = 0f;

    public int currentPlayer = 0;
    private int maxPlayers;
    public GameObject[] allPlayers;
    public bool actionTakenS;
    private playerActions scPlayerActions;
    private GameObject playerScript;


    [SerializeField] Text timeLeftText;

    void Start()
    {
        currentTime = turnTime;

        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        maxPlayers = allPlayers.Length;

        playerActions playerScript = GameObject.Find("playerActions").GetComponent<playerActions>();
        
    }

    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        timeLeftText.text = currentTime.ToString("0.0");

        if (playerScript)
        {
            bool actionTakenS = playerScript.actionTaken;
        }
        else
        {
            Debug.Log("no game object called scPlayerActions found");
        }

        if (currentTime <= 0 | actionTakenS)
        {
            EndTurn();
            SwitchPlayer();
        }
    }

    void EndTurn()
    {
        currentTime = turnTime;
        actionTakenS = false;
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

