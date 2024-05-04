using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public float turnTime;
    public float currentTime;

    public int totalPlayers;
    public int currentPlayerIndex;
    public Player currentPlayer;
    private PlayerManager playerManager;
    private List<Player> players;
    public int mapWidth;
    public int mapHeight;
    public int[,] Map;
    public Text timeLeftText;

    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        SetCurrentPlayer();
        timeLeftText = GameObject.Find("timeLeftText").GetComponent<Text>();
        currentTime = turnTime;
    }

    void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        timeLeftText.text = currentTime.ToString("0.0");

        if (currentTime <= 0)
        {
            NextPlayerTurnProcedure();
        }
    }

    public void NextPlayerTurnProcedure()
    {
        currentTime = turnTime; // reset timer
        SwitchPlayer(); // increment current player index
        SetCurrentPlayer();
        NotifyCurrentPlayer();
    }

    private void NotifyCurrentPlayer()
    {
        currentPlayer.OnTurnStarted();
    }

    void SwitchPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
        // to do: switching between teams
    }

    private void SetCurrentPlayer()
    {
        currentPlayer = playerManager.players[currentPlayerIndex];
    }
}
