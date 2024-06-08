using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tools;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public float turnTime;
    public float currentTime;
    public int mapWidth;
    public int mapHeight;
    public Text timeLeftText;

    public int totalPlayers;
    private Team? winner;
    private List<int> paths;
    private List<GameObject> enemiesWithPath;
    private List<GameObject> enemiesInSight;
    public Vector3 currentTarget;
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

    private List<Player> RedTeam => PlayersComponents
        .FindAll(player => player.team == Team.Red);

    private List<Player> BlueTeam => PlayersComponents
        .FindAll(player => player.team == Team.Blue);

    public bool weaponUsed;

    private Queue<Player> redQ = new();
    private Queue<Player> blueQ = new();

    private void Start()
    {
        var playersArray = GameObject.FindGameObjectsWithTag("Player");
        playersArray[0].GetComponent<Player>().current = true;
        RedTeam.ForEach(el => redQ.Enqueue(el));
        BlueTeam.Skip(1).ToList().ForEach(el => blueQ.Enqueue(el));
        timeLeftText = GameObject.Find("timeLeftText").GetComponent<Text>();
        currentTime = turnTime;
        PathFinder.Initialize();
        (paths, enemiesWithPath) = PathFinder.FindPathsToEnemies(CurrentPlayer, PlayersComponents);
        enemiesInSight = RaycastDetector.DetectEnemiesInSight(CurrentPlayer, PlayersComponents);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (enemiesInSight.Count > 0)
            {
                ShootClosestEnemy(enemiesInSight);
            }
        }
        if (winner == null)
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
            timeLeftText.text = "WINNER: " + winner;
            timeLeftText.horizontalOverflow = HorizontalWrapMode.Overflow;
            timeLeftText.color = winner switch
            {
                Team.Blue => Color.blue,
                Team.Red => Color.red,
                _ => timeLeftText.color
            };
        }
    }

    private void ShootClosestEnemy(List<GameObject> enemies)
    {
        while (true)
        {
            var minIndex = paths.IndexOf(paths.Min());
            foreach (var enemy in enemies)
            {
                if (enemy == enemiesWithPath[minIndex])
                {
                    currentTarget = enemiesWithPath[minIndex].transform.position;
                    CurrentPlayer.GetComponent<WeaponSwitcher>().SwitchWeaponTo(0); 
                    CurrentPlayer.GetComponent<WeaponSwitcher>().GetCurrentWeapon()?.GetComponent<Bazooka>().RotateBazookaToPoint(currentTarget);
                    CurrentPlayer.GetComponent<Player>().TryShoot(currentTarget);
                    return;
                } 
            }
            paths.RemoveAt(minIndex);
            enemiesWithPath.RemoveAt(minIndex);
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
            
            (paths, enemiesWithPath) = PathFinder.FindPathsToEnemies(CurrentPlayer, PlayersComponents);
            enemiesInSight = RaycastDetector.DetectEnemiesInSight(CurrentPlayer, PlayersComponents);
        }
        else
        {
            Debug.Log("END");

            var score = BlueTeam.Count() - RedTeam.Count();

            winner = score switch
            {
                > 0 => Team.Blue,
                0   => Team.None,
                < 0 => Team.Red
            };
            
            Debug.Log("WINNER: " + winner);
        }
    }

    private bool BothTeamsActive => BlueTeam.Count > 0 && RedTeam.Count > 0;
}