using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class LevelGenerator : MonoBehaviour
{

    public Tilemap tilemap;
    public TileBase tile;

    public int width = 60;
    public int height = 40;

    void Start()
    {
        ClearMap();
        int[,] map = new int[width, height];
        float seed = Time.time;

        map = MapFunctions.GenerateArray(width, height, true);
        map = MapFunctions.PerlinNoiseCave(map, Random.Range(0.0001f, 0.4f), true);


        //Render the result
        MapFunctions.RenderMap(map, tilemap, tile);
    }

    public void ClearMap()
    {
        tilemap.ClearAllTiles();
    }
}
