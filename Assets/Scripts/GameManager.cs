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
        OptAlgorithm();
        //(_, _) = PathFinder.FindPathsToEnemies(CurrentPlayer, PlayersComponents);
        //RaycastDetector.DetectEnemiesInSight(CurrentPlayer.transform.position);
    }

    private void Update()
    {
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
    

    private void ShootTarget(GameObject targetObject)
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
        }
        
        if (CurrentPlayer.GetComponent<Player>().team == Team.Red)
        {
            OptAlgorithm();
        }
        else
        {
            MlAlgorithm();
        }
    }

    private void MlAlgorithm()
    {
        // to do
    }

    private void OptAlgorithm()
    { 
        var damagable = FindDamageableEnemies();
        var (killable, 
            shootable) = FindKillableAndShootableEnemies(damagable);

        if (killable.Count > 0)
        {
            FindTheBestTargetAndKill(killable);
        }
        else if (killable.Count == 0 && shootable.Count > 0)
        {
            ShootTheClosestEnemy(shootable);
        }
        else
        {
            // don't move, build the closest one
            Debug.Log("Did not found any enemies near, you can become Bob The Builder!");
        }

    }

    private static (Dictionary<Vector3, GameObject> killable, Dictionary<Vector3, GameObject> shootable) FindKillableAndShootableEnemies(Dictionary<Vector3, List<GameObject>> damagable)
    {
        var killable = new Dictionary<Vector3, GameObject>();
        var shootable = new Dictionary<Vector3, GameObject>();
        foreach (var kvp in damagable)
        {
            foreach (var go in kvp.Value)
            {
                var enemyHealth = go.GetComponent<Player>().Health;
                var potentialDamage = DmgCalculator.CalculateBulletDamage(kvp.Key, go.transform.position);
                if (potentialDamage >= enemyHealth)
                {
                    killable.Add(kvp.Key, go);
                }
                else
                {
                    shootable.Add(kvp.Key, go);
                }
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

    private void ShootTheClosestEnemy(Dictionary<Vector3, GameObject> shootable)
    {
        var currPlayerPos = CurrentPlayer.transform.position;
        float shortestDistance = mapWidth * mapHeight;
        var bestTarget = shootable.ElementAt(0);
        
        foreach (var kvp in shootable)
        {
            var distToEnemy = Vector3.Distance(currPlayerPos, kvp.Value.transform.position);
            if (distToEnemy < shortestDistance)
            {
                bestTarget = kvp;
                shortestDistance = distToEnemy;
            }
        }
        bestTarget.Value.SetActive(false);
        var (_, pointToRun) = FindTheFarthestPositionToEscape(bestTarget.Value.transform.position);
        bestTarget.Value.SetActive(true);
        ShootTarget(bestTarget.Value);
        CurrentPlayer.transform.position = pointToRun;
    }

    private void FindTheBestTargetAndKill(Dictionary<Vector3, GameObject> killable)
    {
        var farthestPathSum = 0;
        var bestTarget = killable.ElementAt(0);
        Vector3 pointToRun = default;
        foreach (var kvp in killable)
        {
            int currFarthestPathSum;
            kvp.Value.SetActive(false);
            (currFarthestPathSum, pointToRun) = FindTheFarthestPositionToEscape(kvp.Value.transform.position);
            kvp.Value.SetActive(true);
            if (currFarthestPathSum > farthestPathSum && killable.Count > 1)
            {
                farthestPathSum = currFarthestPathSum;
                bestTarget = kvp;
            }
        }

        CurrentPlayer.transform.position = bestTarget.Key; // run to the shooting position
        ShootTarget(bestTarget.Value);
        CurrentPlayer.transform.position = pointToRun;
    }

    private bool BothTeamsActive => BlueTeam.Count > 0 && RedTeam.Count > 0;

    private (int mPaths, Vector3Int) FindTheFarthestPositionToEscape(Vector3 position)
    {
        const int movementRangeAfterShooting = 4;
        var positions = FindValidPositions(position, movementRangeAfterShooting);
        var l = 0;
        var r = positions.Count;
        (int, Vector3Int) ret = (0, default);
        while (l <= r)
        {
            var m = l + (r - 1) / 2;
            var mPaths = PathFinder.TotalPathToEnemies(positions[m]);
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
                ret = (mPaths, positions[m]);
                break;
            }
        }
        return ret;
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
                if (Vector3.Distance(centerCell, currentCell) <= movementRange)
                {
                    if (tilemap.HasTile(cellBelow))
                    {
                        validPoints.Add(currentCell);
                    }
                }
            }
        }
        return validPoints;
    }
}