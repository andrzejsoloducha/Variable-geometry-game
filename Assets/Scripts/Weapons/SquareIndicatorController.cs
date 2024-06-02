using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class SquareIndicatorController : MonoBehaviour
{
    private SpriteRenderer Spr => GetComponent<SpriteRenderer>();

    private Tilemap tilemap => SceneManager.GetActiveScene()
        .GetRootGameObjects()
        .ToList()
        .Find(go => go.name == "Grid")
        .GetComponentInChildren<Tilemap>();

    private void Awake()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }

    private void LateUpdate()
    {

        const float alpha = 0.5f;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var gridPosition = tilemap.WorldToCell(mousePosition);
        transform.position = new Vector3 ((float)(gridPosition.x + 0.5), (float)(gridPosition.y + 0.5));

        if (IsValidTilePlacement(gridPosition))
        {
            Spr.color = Color.green;
            var color = Spr.color;
            color.a = alpha;
            Spr.color = color;
        }
        else
        {
            Spr.color = Color.red;
            var color = Spr.color;
            color.a = alpha;
            Spr.color = color;
        }
    }
    
    private bool IsValidTilePlacement(Vector3Int gridPosition)
    {
        return !tilemap.HasTile(gridPosition);
    }
}
