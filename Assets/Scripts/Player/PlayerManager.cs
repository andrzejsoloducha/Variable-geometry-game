using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class PlayerManager : MonoBehaviour
{
    public GameObject Tilemap;
    public GameObject playerPrefab;
    public GameManager gameManager;
    private List<Vector3> availablePlaces;
    public string playerLayerName = "Player";
    public List<Player> players;
    private Tilemap tilemap;

    void Start()
    {
        FindPlacesToRespawn();
        Respawn();
    }

    private void FindPlacesToRespawn()
    {
        availablePlaces = new List<Vector3>();
        tilemap = Tilemap.GetComponent<Tilemap>();

        for (int x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (int y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                Vector3 position = tilemap.GetCellCenterWorld(cell);

                if (tilemap.GetTile(cell) is null)
                {
                    availablePlaces.Add(position);
                }
            }
        }
    }

    private void Respawn()
    {
        var playerLayer = LayerMask.NameToLayer(playerLayerName);
        for (var i = 0; i < gameManager.totalPlayers; i++)
        {
            if (availablePlaces.Count > 0)
            {
                var index = Random.Range(0, availablePlaces.Count);

                var position = availablePlaces[index];
                var playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
                var playerRigidbody2D = playerObject.AddComponent<Rigidbody2D>();
                playerObject.AddComponent<BoxCollider2D>();
                playerRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                
                playerObject.tag = "Player";
                playerObject.layer = playerLayer;
                var playerSpriteRenderer = playerObject.GetComponent<SpriteRenderer>();
                if (playerSpriteRenderer == null)
                {
                    playerSpriteRenderer = playerObject.AddComponent<SpriteRenderer>();
                }
                
                if (i % 2 != 0)
                {
                    playerSpriteRenderer.color = Color.red;
                    playerObject.name = "Player_" + i + "_team_red";
                    //playerComponent.Team = "red";
                }
                else
                {
                    playerObject.name = "Player_" + i + "_team_blue";
                    //playerComponent.Team = "blue";
                }

                var playerComponent = playerObject.GetComponent<Player>();
                players.Add(playerComponent);
                availablePlaces.RemoveAt(index);
            }


            else
            {
                break;
            }
        }
    }
}