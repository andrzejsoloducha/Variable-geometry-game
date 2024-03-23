using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PlayerGenerator : MonoBehaviour
{
    public Tilemap tilemap;
    public GameObject playerPrefab;
    public Vector3Int cellPosition;

    public GameManager gameManager;
    private List<Vector3> availablePlaces;

    public string playerLayerName = "Player";

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        int playerLayer = LayerMask.NameToLayer(playerLayerName);
        for (int i = 0; i < gameManager.totalPlayers; i++)
        {
            if (availablePlaces.Count > 0)
            {
                int index = Random.Range(0, availablePlaces.Count);

                Vector3 position = availablePlaces[index];
                GameObject clonePlayer = Instantiate(playerPrefab, position, Quaternion.identity);
                Rigidbody2D playerRigidbody2D = clonePlayer.AddComponent<Rigidbody2D>();
                BoxCollider2D playerCollider2D = clonePlayer.AddComponent<BoxCollider2D>();
                playerRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                clonePlayer.name = "Player" + i;
                clonePlayer.tag = "Player";
                availablePlaces.RemoveAt(index);
                clonePlayer.layer = playerLayer;

                SpriteRenderer playerSpriteRenderer = clonePlayer.GetComponent<SpriteRenderer>();
                if (playerSpriteRenderer == null)
                {
                    playerSpriteRenderer = clonePlayer.AddComponent<SpriteRenderer>();
                }

                if (i % 2 != 0)
                {
                    playerSpriteRenderer.color = Color.red;
                }
            }


            else
            {
                break;
            }
        }
    }
}