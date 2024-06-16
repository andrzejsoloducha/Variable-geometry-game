using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Tools
{
    public static class PathFinder
    {
        private static readonly int MapWidth = GameManager.Instance.mapWidth;
        private static readonly int MapHeight = GameManager.Instance.mapHeight;
        private static int _numVertices;
        private static float[,] _dist;
        private static int[,] _nextNodes;
        private const float TerrainMultiplier = 5.0f;

        private static Tilemap Tilemap => SceneManager.GetActiveScene()
            .GetRootGameObjects()
            .ToList()
            .Find(go => go.name == "Grid")
            .GetComponentInChildren<Tilemap>();

        public static void Initialize()
        {
            _numVertices = MapWidth * MapHeight;

            _dist = new float[_numVertices, _numVertices];
            _nextNodes = new int[_numVertices, _numVertices];

            for (var i = 0; i < _numVertices; i++)
            {
                for (var j = 0; j < _numVertices; j++)
                {
                    if (i == j)
                        _dist[i, j] = 0;
                    else
                        _dist[i, j] = Mathf.Infinity;

                    _nextNodes[i, j] = -1;
                }
            }
        }

        public static void InitializeDistances()
        {
            for (var x = 0; x < MapWidth; x++)
            {
                for (var y = 0; y < MapHeight; y++)
                {
                    var i = GetTileIndex(x, y);

                    CheckAndSetDistance(i, x + 1, y); // prawo
                    CheckAndSetDistance(i, x - 1, y); // lewo
                    CheckAndSetDistance(i, x, y + 1); // gura
                    CheckAndSetDistance(i, x, y - 1); // duł
                }
            }
        }

        private static void CheckAndSetDistance(int i, int x, int y)
        {
            if (IsValidTile(x, y))
            {
                var j = GetTileIndex(x, y);
                var weight = GetTileWeight(x, y);

                _dist[i, j] = weight;
                _nextNodes[i, j] = j;
            }
        }

        private static bool IsValidTile(int x, int y)
        {
            return x >= 0 && x < MapWidth && y >= 0 && y < MapHeight;
        }

        private static int GetTileIndex(int x, int y)
        {
            return y * MapWidth + x;
        }

        private static float GetTileWeight(int x, int y)
        {
            var cellPosition = Tilemap.WorldToCell(
                new Vector3(x, y, 0));
            return Tilemap.GetTile(cellPosition) ? TerrainMultiplier : 1.0f;
        }

        public static void CalculatePathsNewRound()
        {
            Initialize();
            InitializeDistances();
            RunFloydWarshall();
        }

        private static void RunFloydWarshall()
        {
            for (var k = 0; k < _numVertices; k++)
            {
                for (var i = 0; i < _numVertices; i++)
                {
                    for (var j = 0; j < _numVertices; j++)
                    {
                        if (_dist[i, j] > _dist[i, k] + _dist[k, j])
                        {
                            _dist[i, j] = _dist[i, k] + _dist[k, j];
                            _nextNodes[i, j] = _nextNodes[i, k];
                        }
                    }
                }
            }
        }

        private static (int steps, float totalWeight) GetPath(int start, int end)
        {
            if (_nextNodes[start, end] == -1)
                return (0, 0);

            var path = new List<int>();
            var totalWeight = 0f;
            var current = start;

            while (current != end)
            {
                path.Add(current);
                var nextTile = _nextNodes[current, end];
                totalWeight += GetTileWeightFromIndex(nextTile);
                current = nextTile;
            }

            path.Add(end);
            totalWeight += GetTileWeightFromIndex(end);
            return (path.Count, totalWeight);
        }

        private static float GetTileWeightFromIndex(int nextTile)
        {
            var x = nextTile % MapWidth;
            var y = nextTile / MapWidth;
            return GetTileWeight(x, y);
        }

        public static (List<int>, List<GameObject>) FindPathsToEnemies(GameObject currentPlayer, List<Player> players)
        {
            var playerPos = currentPlayer.transform.position;
            var start = GetTileIndex((int)playerPos.x, (int)playerPos.y);
            var paths = new List<int>();
            var enemies = new List<GameObject>();

            foreach (var enemy in players)
            {
                if (currentPlayer.GetComponent<Player>().team != enemy.team)
                {
                    var enemyPos = enemy.gameObject.transform.position;
                    var end = GetTileIndex((int)enemyPos.x, (int)enemyPos.y);

                    var (path, totalWeight) = GetPath(start, end);
                    if (path != 0)
                    {
                        paths.Add(path);
                        enemies.Add(enemy.gameObject);
                    }
                }
            }

            return (paths, enemies);
        }
        
        public static int TotalPathToEnemies(Vector3Int point)
        {
            var start = GetTileIndex(point.x, point.y);
            var paths = new List<int>();
            var currentTeam = GameManager.Instance.CurrentPlayer.GetComponent<Player>().team;
            var enemies = GameManager.Instance.Players.FindAll(
                go => go.GetComponent<Player>().team != currentTeam);

            foreach (var enemy in enemies)
            {
                var enemyPos = enemy.gameObject.transform.position;
                var end = GetTileIndex((int)enemyPos.x, (int)enemyPos.y);

                var (path, totalWeight) = GetPath(start, end);
                if (path != 0)
                {
                    paths.Add(path);
                }
            }

            return paths.Sum();
        }
    }
}