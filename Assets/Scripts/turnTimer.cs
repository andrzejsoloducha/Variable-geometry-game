using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class turnTimer : MonoBehaviour
{
    public GameManager gameManager;
    [SerializeField] Text timeLeftText;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        gameManager.currentTime = gameManager.turnTime;
    }

    void Update()
    {
        gameManager.currentTime -= 1 * Time.deltaTime;
        timeLeftText.text = gameManager.currentTime.ToString("0.0");

        if (gameManager.currentTime <= 0 | gameManager.actionTaken)
        {
            EndTurn();
            SwitchPlayer();
        }
    }

    void EndTurn()
    {
        gameManager.currentTime = gameManager.turnTime;
        gameManager.actionTaken = false;
    }

    void SwitchPlayer()
    {
        gameManager.currentPlayer += 1;

        if (gameManager.currentPlayer >= gameManager.totalPlayers)
        {
            gameManager.currentPlayer = 0;
        }
    }
}

