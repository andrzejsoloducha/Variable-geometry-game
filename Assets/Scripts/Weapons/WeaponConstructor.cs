using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class WeaponConstructor : MonoBehaviour
{
    public TileBase tilePrefab;
    public GameObject squareIndicatorPrefab;
    private GameObject sqIndicator;
    public int maxTileCount;
    private int remainingTileCount;

    private Tilemap tilemap => SceneManager.GetActiveScene()
            .GetRootGameObjects()
            .ToList()
            .Find(go => go.name == "Grid")
            .GetComponentInChildren<Tilemap>();

    private void OnEnable()
    {
        remainingTileCount = maxTileCount;
        sqIndicator = Instantiate(squareIndicatorPrefab, transform.position, Quaternion.identity);
    }

    private void OnDisable()
    {
        gameObject.SetActive(false);
        Destroy(sqIndicator);
    }

    public void OnMouseInput()
    {
        if (gameObject.activeSelf)
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var gridPosition = tilemap.WorldToCell(mousePosition);
        
            if (Input.GetMouseButtonDown(0) && IsValidTilePlacement(gridPosition))
            {
                tilemap.SetTile(gridPosition, tilePrefab);
                remainingTileCount--;
            
                if (remainingTileCount == 0)
                {
                    OnDisable();
                    GameManager.Instance.NextTurnProcedure();
                }
                if (Mathf.Approximately(GameManager.Instance.currentTime, 0.01f))
                {
                    OnDisable();
                }
            }
        }

    }
    private bool IsValidTilePlacement(Vector3Int gridPosition)
    {
        return !tilemap.HasTile(gridPosition);
    }

}