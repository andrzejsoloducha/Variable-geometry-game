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
    

    private void Start()
    {
        var playersArray = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in playersArray)
        {
            players.Add(player);
        }

        SetPlayerForTurn();
        timeLeftText = GameObject.Find("timeLeftText").GetComponent<Text>();
        currentTime = turnTime;
    }

    private void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        timeLeftText.text = currentTime.ToString("0.0");

        if (currentTime <= 0) // or performed an action
        {
            NextTurnProcedure();
        }
    }

    private void NextTurnProcedure()
    {
        currentTime = turnTime;
        currentPlayer.GetComponent<Player>().OnTurnEnd();
        currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
        // to do: implement Team component and add condition to switch between teams
        SetPlayerForTurn();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SetPlayerForTurn()
    {
        currentPlayer = players[currentPlayerIndex];
        currentPlayer.GetComponent<Player>().OnTurnStart();
    }
}