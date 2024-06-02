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

        private static void InitializeDistances()
        {
            for (var x = 0; x < MapWidth; x++)
            {
                for (var y = 0; y < MapHeight; y++)
                {
                    var i = GetTileIndex(x, y);

                    CheckAndSetDistance(i, x + 1, y); // Right
                    CheckAndSetDistance(i, x - 1, y); // Left
                    CheckAndSetDistance(i, x, y + 1); // Up
                    CheckAndSetDistance(i, x, y - 1); // Down
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
            return Tilemap.GetTile(cellPosition) ? 2.0f : 1.0f;
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

        private static List<int> GetPath(int start, int end)
        {
            if (_nextNodes[start, end] == -1)
                return null;

            var path = new List<int>();
            var current = start;

            while (current != end)
            {
                path.Add(current);
                current = _nextNodes[current, end];
            }

            path.Add(end);
            return path;
        }
        
        public static void FindPathsToEnemies(GameObject currentPlayer, List<Player> players)
        {
            InitializeDistances();
            RunFloydWarshall();
            var playerPos = currentPlayer.transform.position;
            var start = GetTileIndex((int)playerPos.x, (int)playerPos.y);

            foreach (var enemy in players)
            {
                if (currentPlayer.GetComponent<Player>().team != enemy.team)
                {
                    var enemyPos = enemy.gameObject.transform.position;
                    var end = GetTileIndex((int)enemyPos.x, (int)enemyPos.y);

                    var path = GetPath(start, end);
                    if (path != null)
                    {
                        Debug.Log($"Path from player to enemy at ({enemyPos.x}, {enemyPos.y}): {string.Join(" -> ", path)}");
                    }
                    else
                    {
                        Debug.Log($"No path found from player to enemy at ({enemyPos.x}, {enemyPos.y})");
                    }
                }
            }
        }
    }
}