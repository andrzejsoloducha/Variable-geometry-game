using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float turnTime;
    public float currentTime;

    public int totalPlayers;
    public int currentPlayer = 0;
    public bool actionTaken = false;


    public int mapWidth = 60;
    public int mapHeight = 40;
    public int[,] map;

    public bool deathmatch = false;
    public Text timeLeftText;

    void Start()
    {
        timeLeftText = GameObject.Find("timeLeftText").GetComponent<Text>();
        currentTime = turnTime;
    }

    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        timeLeftText.text = currentTime.ToString("0.0");

        if (currentTime <= 0 | actionTaken)
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
        currentPlayer = (currentPlayer + 1) % totalPlayers;
    }
}
