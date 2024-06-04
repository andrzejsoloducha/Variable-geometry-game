using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GenerateMap : MonoBehaviour
{

    public Tilemap tilemap;
    public TileBase tile;

    public GameManager gameManager;
    private int width;
    private int height;


    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        width = gameManager.mapWidth;
        height = gameManager.mapHeight;
        ClearMap();
        var seed = Time.time;

        var map = GenerateArray(width, height, true);
        map = PerlinNoiseCave(map, Random.Range(0.0001f, 0.4f), true);

        RenderMap(map, tilemap, tile);
    }


    private void ClearMap()
    {
        tilemap.ClearAllTiles();
    }


    private static int[,] GenerateArray(int width, int height, bool empty)
    {
        var map = new int[width, height];
        for (var x = 0; x < map.GetUpperBound(0); x++)
        {
            for (var y = 0; y < map.GetUpperBound(1); y++)
            {
                if (empty)
                {
                    map[x, y] = 0;
                }
                else
                {
                    map[x, y] = 1;
                }
            }
        }
        return map;
    }


    private static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile)
     {
        for (var x = 0; x < map.GetUpperBound(0); x++)
        {
            for (var y = 0; y < map.GetUpperBound(1); y++)
            {
                if (map[x, y] == 1) // 1 = tile, 0 = no tile
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                    tilemap.SetColliderType(new Vector3Int(x, y, 0), Tile.ColliderType.Sprite);
                }

                if (map[x, y] == 2) // tutaj jakiś layer nie do rozjebania
                {
                    tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }
     }

    private static int[,] PerlinNoise(int[,] map, float seed)
    {
        var reduction = 0.5f;

        for (var x = 0; x < map.GetUpperBound(0); x++)
        {
            var newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, seed) - reduction) * map.GetUpperBound(1));
            newPoint += (map.GetUpperBound(1) / 2);

            for (var y = newPoint; y >= 0; y--)
            {
                map[x, y] = 1;
            }
        }
        return map;
    }


    public static int[,] PerlinNoiseSmooth(int[,] map, float seed, int interval)
    {
        if (interval > 1)
        {
            var reduction = 0.5f;

            var noiseX = new List<int>();
            var noiseY = new List<int>();

            for (var x = 0; x < map.GetUpperBound(0); x += interval)
            {
                var newPoint = Mathf.FloorToInt((Mathf.PerlinNoise(x, (seed * reduction))) * map.GetUpperBound(1));
                noiseY.Add(newPoint);
                noiseX.Add(x);
            }

            var points = noiseY.Count;

            for (var i = 1; i < points; i++)
            {
                var currentPos = new Vector2Int(noiseX[i], noiseY[i]);
                var lastPos = new Vector2Int(noiseX[i - 1], noiseY[i - 1]);

                Vector2 diff = currentPos - lastPos;
                var heightChange = diff.y / interval;
                float currHeight = lastPos.y;

                for (var x = lastPos.x; x < currentPos.x; x++)
                {
                    for (var y = Mathf.FloorToInt(currHeight); y > 0; y--)
                    {
                        map[x, y] = 1;
                    }
                    currHeight += heightChange;
                }
            }
        }
        else
        {
            map = PerlinNoise(map, seed);
        }

        return map;
    }


    private static int[,] PerlinNoiseCave(int[,] map, float modifier, bool edgesAreWalls)
    {
        for (var x = 0; x < map.GetUpperBound(0); x++)
        {
            for (var y = 0; y < map.GetUpperBound(1); y++)
            {

                if (edgesAreWalls && (x == 0 || y == 0 || x == map.GetUpperBound(0) - 1 || y == map.GetUpperBound(1) - 1))
                {
                    map[x, y] = 2;
                }
                else
                {
                    var newPoint = Mathf.RoundToInt(Mathf.PerlinNoise(x * modifier, y * modifier));
                    map[x, y] = newPoint;
                }
            }
        }
        return map;
    }
}