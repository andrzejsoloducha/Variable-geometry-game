using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameManager gameManager;
    public List<Vector3> availablePlaces;
    public string playerLayerName = "Player";
    public List<GameObject> players = new();
    public Tilemap tilemap;
    
    private void Start()
    {
        FindPlacesToRespawn();
        Respawn();
    }

    public void ResetPlayers()
    {
        var playersList = GameManager.Instance.Players;
        foreach (var player in playersList)
        {
            if (player)
            {
                Destroy(player);
            }
        }
        
        FindPlacesToRespawn();
        Respawn();
    }

    private void FindPlacesToRespawn()
    {
        availablePlaces = new List<Vector3>();
        tilemap = tilemap.GetComponent<Tilemap>();

        for (var x = tilemap.cellBounds.xMin; x < tilemap.cellBounds.xMax; x++)
        {
            for (var y = tilemap.cellBounds.yMin; y < tilemap.cellBounds.yMax; y++)
            {
                var cell = new Vector3Int(x, y, 0);
                var position = tilemap.GetCellCenterWorld(cell);

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
                playerRigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            
                playerObject.tag = "Player";
                playerObject.layer = playerLayer;
                var playerSpriteRenderer = playerObject.GetComponent<SpriteRenderer>();
                if (!playerSpriteRenderer)
                {
                    playerSpriteRenderer = playerObject.AddComponent<SpriteRenderer>();
                }

                var playerComponent = playerObject.GetComponent<Player>();
                if (i % 2 != 0)
                {
                    playerSpriteRenderer.color = Color.red;
                    playerObject.name = "Player_" + i;
                    playerComponent.team = Team.Red;
                }
                else
                {
                    playerObject.name = "Player_" + i;
                    playerComponent.team = Team.Blue;
                }

                players.Add(playerObject);
                availablePlaces.RemoveAt(index);
            }
            else
            {
                break;
            }
        }
    }
}