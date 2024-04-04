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

    public GameObject bazookaPrefab;

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
                GameObject playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
                Player playerComponent = playerObject.GetComponent<Player>();
                Rigidbody2D playerRigidbody2D = playerObject.AddComponent<Rigidbody2D>();
                BoxCollider2D playerCollider2D = playerObject.AddComponent<BoxCollider2D>();
                playerRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                
                playerObject.tag = "Player";
                playerObject.layer = playerLayer;

                if (i == 1)
                {
                    GameObject bazooka = Instantiate(bazookaPrefab, playerObject.transform.position, Quaternion.identity);
                    bazooka.transform.SetParent(playerObject.transform);
                    Vector3 bazookaOffset = new Vector3(0.3f, -0.15f, 0f);
                    bazooka.transform.localPosition = bazookaOffset;
                }


                SpriteRenderer playerSpriteRenderer = playerObject.GetComponent<SpriteRenderer>();
                if (playerSpriteRenderer == null)
                {
                    playerSpriteRenderer = playerObject.AddComponent<SpriteRenderer>();
                }

                if (gameManager.deathmatch == false)
                {
                    if (i % 2 != 0)
                    {
                        playerSpriteRenderer.color = Color.red;
                        playerObject.name = "Player" + i + "Red";
                        playerComponent.team = "red";
                    }
                    else
                    {
                        playerObject.name = "Player" + i + "Blue";
                        playerComponent.team = "blue";
                    }
                }

                availablePlaces.RemoveAt(index);
            }


            else
            {
                break;
            }
        }
    }
}