using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public float turnTime;
    public float currentTime;

    public int mapWidth;
    public int mapHeight;
    public int[,] Map;
    public Text timeLeftText;

    public int totalPlayers;
    public int currentPlayerIndex;
    public GameObject currentPlayer;
    public List<GameObject> players;
    private PlayerInputHandler playerInputHandler;
    

    private void Start()
    {
        playerInputHandler.CheckKeyboardInput();
        playerInputHandler = gameObject.GetComponent<PlayerInputHandler>();
        var playersArray = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playersArray)
        {
            players.Add(player);
            Debug.Log("player added:" + player.name);
        }

        if (players.Count == 0)
        {
            Debug.LogWarning("No players were found or player components missing");
        }

        SetCurrentPlayer();
        timeLeftText = GameObject.Find("timeLeftText").GetComponent<Text>();
        currentTime = turnTime;
    }

    private void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        timeLeftText.text = currentTime.ToString("0.0");

        if (currentTime <= 0) // add action_taken boolean
        {
            currentTime = turnTime;
            //var lastPlayer = currentPlayerIndex;
            currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
            // to do: implement Team component and add condition to switch between teams
            SetCurrentPlayer();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SetCurrentPlayer()
    {
        currentPlayer = players[currentPlayerIndex];
        if (playerInputHandler)
        {
            playerInputHandler.SetPlayer(currentPlayer);
        }
    }

    private void FixedUpdate()
    {
    }
}