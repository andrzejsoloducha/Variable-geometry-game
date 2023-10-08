using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnsLogic : MonoBehaviour
{
    public int currentPlayer  = 0;
    public int maxPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
    public float turnTime = 30;
    public float timer = 30;
    private bool actionTaken = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0 | actionTaken)
        {
            EndTurn();
            SwitchPlayer();
        }
        else
        {
            timer -= Time.deltaTime;
        }

    }

    void EndTurn()
    {
        timer = turnTime;
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
