using PlayerManagment;
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
    public int mapWidth;
    public int mapHeight;
    public int[,] Map;
    public Text timeLeftText;

    private void Start()
    {
        SetCurrentPlayer();
        timeLeftText = GameObject.Find("timeLeftText").GetComponent<Text>();
        currentTime = turnTime;
    }

    private void Update()
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

    private void SwitchPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % totalPlayers;
        // to do: switching between teams
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void SetCurrentPlayer()
    {
        //Debug.Log("Players count: " + playerManager.players.Count);
        currentPlayer = playerManager.players[currentPlayerIndex];
        
    }
}
