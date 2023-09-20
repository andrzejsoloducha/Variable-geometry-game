using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PlayerGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject playerPrefab;
    public Vector3Int cellPosition;
    public int n = 1;
    public MapFunctions mapFunctions;

    private List<Vector3> availablePlaces;

    void Start()
    {
        availablePlaces = new List<Vector3>();
        tilemap = GetComponent<Tilemap>();

        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                Vector3 position = tilemap.GetCellCenterWorld(cell);

                if (tilemap.GetTile(cell) is null)
                //if (tilemap.HasTile(cell))
                {
                    availablePlaces.Add(position);
                }
            }
        }
        Respawn();
    }

    void Respawn()
    {
        for (int i = 0; i < n; i++)
        {
            if (availablePlaces.Count > 0)
            {
                int index = Random.Range(0, availablePlaces.Count);

                Vector3 position = availablePlaces[index];
                Instantiate(playerPrefab, position, Quaternion.identity);
                availablePlaces.RemoveAt(index);
            }
            else
            {
                break;
            }
        }
    }
}