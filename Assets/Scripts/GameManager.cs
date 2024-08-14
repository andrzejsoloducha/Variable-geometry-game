using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
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
    private GenerateMap generateMap;
    private PlayerManager playerManager;
    
    private Queue<Player> redQ = new();
    private Queue<Player> blueQ = new();

    private void Start()
    {
        StartFreshGame();
        generateMap = FindObjectOfType<GenerateMap>();
        playerManager = FindObjectOfType<PlayerManager>();
    }

    public void StartFreshGame()
    {
        ClearParticleSystem();
        generateMap?.TriggerResetMap();
        playerManager?.TriggerResetPlayers();
        redQ.Clear();
        blueQ.Clear();
        var playersArray = GameObject.FindGameObjectsWithTag("Player");
        playersArray[0].GetComponent<Player>().current = true;
        RedTeam.ForEach(el => redQ.Enqueue(el));
        BlueTeam.Skip(1).ToList().ForEach(el => blueQ.Enqueue(el));
        timeLeftText = GameObject.Find("timeLeftText").GetComponent<Text>();
        currentTime = turnTime;
        //PathFinder.CalculatePathsNewRound();
        //OptAlgorithm();
        //(_, _) = PathFinder.FindPathsToEnemies(CurrentPlayer, PlayersComponents);
        //RaycastDetector.DetectEnemiesInSight(CurrentPlayer.transform.position);
    }

    public void ClearParticleSystem()
    {
        var particleSystems = FindObjectsOfType<ParticleSystem>();

        foreach (var ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void Update()
    {
        if (winner == null)
        {
            TickTimer();

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
            var qlearningagent = GameObject.Find("QLearningManager").GetComponent<QLearningAgent>();
            qlearningagent.EndEpisode();
            AnnounceWinner();
        }
    }

    private void AnnounceWinner()
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

    private void TickTimer()
    {
        currentTime -= 1 * Time.deltaTime;
        timeLeftText.text = currentTime.ToString("0.0");
    }


    public void ShootTarget(GameObject targetObject)
    {
        var targetPosition = targetObject.transform.position;
        CurrentPlayer.GetComponent<WeaponSwitcher>().SwitchWeaponTo(0); 
        CurrentPlayer.GetComponent<WeaponSwitcher>()
            .GetCurrentWeapon()?
            .GetComponent<Bazooka>()
            .RotateBazookaToPoint(targetPosition);
        CurrentPlayer.GetComponent<Player>().TryShoot(targetPosition);
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
        var qlearningagent = GameObject.Find("QLearningManager").GetComponent<QLearningAgent>();
        qlearningagent.EndEpisode();
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
            Debug.Log("END");


            var score = BlueTeam.Count() - RedTeam.Count();

            winner = score switch
            {
                > 0 => Team.Blue,
                0   => Team.None,
                < 0 => Team.Red
            };
            
            Debug.Log("WINNER: " + winner);
            StartFreshGame();
        }
        
        //if (CurrentPlayer.GetComponent<Player>().team == Team.Red)
        //{
        //    PathFinder.CalculatePathsNewRound();
        //    OptAlgorithm();
        //}
    }

    private void MlAlgorithm()
    {
        // todo
    }

    private void OptAlgorithm()
    { 
        var damagable = FindDamageableEnemies();
        var (killable, 
            shootable) = FindKillableAndShootableEnemies(damagable);

        if (killable.Count > 0)
        {
            Debug.Log(CurrentPlayer.name + "Found killable enemy, shooting! ");
            FindTheBestTargetAndKill(killable);
        }
        else if (killable.Count == 0 && shootable.Count > 0)
        {
            Debug.Log(CurrentPlayer.name + "Found the closest enemy, shooting! ");
            ShootTheClosestEnemy(shootable);
        }
        else
        {
            // don't move, build the closest one
            Debug.Log(CurrentPlayer.name + "you become Bob The Builder!");
        }

    }

    private static (List<(Vector3, GameObject)> killable, List<(Vector3, GameObject)> shootable) FindKillableAndShootableEnemies(Dictionary<Vector3, List<GameObject>> damagable)
    {
        var killable = new List<(Vector3, GameObject)>();
        var shootable = new List<(Vector3, GameObject)>();

        List<(Vector3, GameObject)> flattenedList = damagable.SelectMany(kvp => kvp.Value.Select(go
            => (kvp.Key, go))).ToList();
        foreach (var tuple in flattenedList)
        { 
            var enemyHealth = tuple.Item2.GetComponent<Player>().Health;
            var potentialDamage = DmgCalculator.CalculateBulletDamage(tuple.Item1, tuple.Item2.transform.position);
            if (potentialDamage >= enemyHealth)
            {
                killable.Add((tuple.Item1, tuple.Item2));
            }
            else
            {
                shootable.Add((tuple.Item1, tuple.Item2));
            }
        }
        return (killable, shootable);
    }

    private Dictionary<Vector3, List<GameObject>> FindDamageableEnemies()
    {
        var currPlayerPosition = CurrentPlayer.transform.position;
        var movementRange = 8;
        var damagable = new Dictionary<Vector3, List<GameObject>>();
        foreach (var point in FindValidPositions(currPlayerPosition, movementRange))
        {
            damagable.Add(point, RaycastDetector.DetectEnemiesInSight(point));
        }

        return damagable;
    }

    private void ShootTheClosestEnemy(List<(Vector3, GameObject)> shootable)
    {
        var currPlayerPos = CurrentPlayer.transform.position;
        float shortestDistance = mapWidth * mapHeight;
        var bestTarget = shootable.ElementAt(0);
        
        foreach (var tuple in shootable)
        {
            var distToEnemy = Vector3.Distance(currPlayerPos, tuple.Item2.transform.position);
            if (distToEnemy < shortestDistance)
            {
                bestTarget = tuple;
                shortestDistance = distToEnemy;
            }
        }
        var (_, pointToRun) = FindTheFarthestPositionToEscape(bestTarget.Item2.transform.position);
        ShootTarget(bestTarget.Item2);
        CurrentPlayer.transform.position = pointToRun;
    }

    private void FindTheBestTargetAndKill(List<(Vector3, GameObject)> killable)
    {
        var farthestPathSum = 0;
        var bestTarget = killable.ElementAt(0);
        Vector3 pointToRun = default;
        foreach (var tuple in killable)
        {
            int currFarthestPathSum;
            tuple.Item2.SetActive(false);
            (currFarthestPathSum, pointToRun) = FindTheFarthestPositionToEscape(tuple.Item2.transform.position);
            tuple.Item2.SetActive(true);
            if (currFarthestPathSum > farthestPathSum && killable.Count > 1)
            {
                farthestPathSum = currFarthestPathSum;
                bestTarget = (tuple.Item1, tuple.Item2);
            }
        }

        CurrentPlayer.transform.position = bestTarget.Item1; // run to the shooting position
        ShootTarget(bestTarget.Item2);
        CurrentPlayer.transform.position = pointToRun;
    }

    private bool BothTeamsActive => BlueTeam.Count > 0 && RedTeam.Count > 0;

    private (int mPaths, Vector3Int) FindTheFarthestPositionToEscape(Vector3 position)
    {
        const int movementRangeAfterShooting = 4;
        var positions = FindValidPositions(position, movementRangeAfterShooting);
        var l = 0;
        var r = positions.Count;
        Debug.Log("found valid positions: " + r);
        var m = 0;
        var mPaths = 0;
        while (l <= r)
        {
            m = l + (r - 1) / 2;
            mPaths = PathFinder.TotalPathToEnemies(positions[m]);
            var m1Paths = PathFinder.TotalPathToEnemies(positions[m - 1]);
            var m2Paths = PathFinder.TotalPathToEnemies(positions[m + 1]);
            if (m > 0 && mPaths < m1Paths)
            {
                r = m - 1;
            }
            else if (m < positions.Count && mPaths < m2Paths)
            {
                l = m + 1;
            }
            else
            {
                break;
            }
        }
        return (mPaths, positions[m]);
    }

    private List<Vector3Int> FindValidPositions(Vector3 playerPosition, int movementRange)
    {
        var tilemap = SceneManager
            .GetActiveScene()
            .GetRootGameObjects().ToList()
            .Find(go => go.name == "Grid")
            .GetComponentInChildren<Tilemap>();
        var validPoints = new List<Vector3Int>();
        var centerCell = tilemap.WorldToCell(playerPosition);

        for (var x = centerCell.x - movementRange; x < centerCell.x + movementRange; x++)
        {
            for (var y = centerCell.y - movementRange; y < centerCell.y + movementRange; y++)
            {
                var currentCell = new Vector3Int(x, y, 0);
                var cellBelow = new Vector3Int(x, y - 1, 0);
                if (tilemap.GetTile(cellBelow))
                {
                    validPoints.Add(currentCell);
                }

            }
        }
        return validPoints;
    }
}