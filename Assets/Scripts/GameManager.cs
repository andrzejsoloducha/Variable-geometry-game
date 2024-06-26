using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    private Team? _winner;

    public List<GameObject> Players => SceneManager.GetActiveScene()
        .GetRootGameObjects()
        .ToList()
        .FindAll(go => go.name.StartsWith("Player"));

    private List<Player> PlayersComponents => Players
        .Select(go => go.GetComponent<Player>())
        .ToList();

    public GameObject CurrentPlayer => PlayersComponents
        .Find(player => player.current)
        ?.gameObject;

    public List<Player> redTeam => PlayersComponents
        .FindAll(player => player.team == Team.Red);
    
    public List<Player> blueTeam => PlayersComponents
        .FindAll(player => player.team == Team.Blue);

    public bool weaponUsed;

    private List<Player> originalRoster;
    
    Queue<Player> redQ = new();
    Queue<Player> blueQ = new();

    private void Start()
    {
        var playersArray = GameObject.FindGameObjectsWithTag("Player");
        playersArray[0].GetComponent<Player>().current = true;
        redTeam.ForEach(el => redQ.Enqueue(el));
        blueTeam.Skip(1).ToList().ForEach(el => blueQ.Enqueue(el));
        originalRoster = PlayersComponents;
        timeLeftText = GameObject.Find("timeLeftText").GetComponent<Text>();
        currentTime = turnTime;
    }

    private void Update()
    {
        if (_winner == null)
        {
            currentTime -= 1 * Time.deltaTime;
            timeLeftText.text = currentTime.ToString("0.0");

            if (currentTime <= 0 || !BothTeamsActive)
            {
                NextTurnProcedure();
            }
            else
            {
                CheckForDrowningPlayers();
            }
        }
        else
        {
            timeLeftText.text = "WINNER: " + _winner;
            timeLeftText.horizontalOverflow = HorizontalWrapMode.Overflow;
            timeLeftText.color = _winner switch
            {
                Team.Blue => Color.blue,
                Team.Red => Color.red,
                _ => timeLeftText.color
            };
        }
    }

    private void CheckForDrowningPlayers()
    {
        const float waterLevel = -5f;

        Players
            .FindAll(GameObjectBelowY(waterLevel))
            .ForEach(KillByDrowning);
    }

    private void KillByDrowning(GameObject playerGameObject)
    {
        playerGameObject
            .GetComponent<Player>()
            .TakeDamage(666);
        
        Debug.Log(playerGameObject.name + " drowned in deep ocean...");
    }

    private Predicate<GameObject> GameObjectBelowY(float y)
    {
        return obj => obj.transform.position.y < y;
    }


    // ReSharper disable Unity.PerformanceAnalysis
    public void NextTurnProcedure([CanBeNull] Player kamikaze = null)
    {
        weaponUsed = false;
        if (BothTeamsActive)
        {
            var prevPlayer = kamikaze ? kamikaze : CurrentPlayer.GetComponent<Player>();
            
            currentTime = turnTime;

            Player nextPlayer;

            if (prevPlayer.team == Team.Blue)
            {
                if (!kamikaze)
                {
                    blueQ.Enqueue(prevPlayer);
                }
                nextPlayer = redQ.Dequeue();
            }
            else
            {
                if (!kamikaze)
                {
                    redQ.Enqueue(prevPlayer);
                }
                nextPlayer = blueQ.Dequeue();
            }

            prevPlayer.current = false;
            nextPlayer.current = true;
        }
        else
        {
            // została jedna drużyna - szukamy zwycijodfjsd
            Debug.Log("END");

            var score = blueTeam.Count() - redTeam.Count();

            _winner = score switch
            {
                > 0 => Team.Blue,
                0   => Team.None,
                < 0 => Team.Red
            };
            
            Debug.Log("WINNER: " + _winner);
        }
    }

    private bool BothTeamsActive => blueTeam.Count > 0 && redTeam.Count > 0;

}