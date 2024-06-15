using System;
using System.Linq;
using Tools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    public float speed;
    public GameObject explosionPrefab;
    public float explosionRadius;
    private Vector2 direction;
    private Vector3 startingPoint;
    
    private void Start()
    {
        startingPoint = gameObject.transform.position;
        GameManager.Instance.weaponUsed = true;
        Destroy(gameObject, 4f);
    }

    public void GetDirection(Vector3 targetPoint = default)
    {
        if (targetPoint == default)
        {
            if (Camera.main)
            {
                targetPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        var directionNormalized = (targetPoint - transform.position).normalized;
        var rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.velocity = directionNormalized * speed;
        }
    }

    private void DestroyThisBullet()
    {
        Destroy(gameObject);
        GameManager.Instance.NextTurnProcedure();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Player"))
        {
            Explode(collision);
            DestroyThisBullet();
        }
    }

    private void Explode(Collision2D collision)
    { 
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        
        var tilemap = SceneManager
            .GetActiveScene()
            .GetRootGameObjects()
            .ToList()
            .Find(go => go.name == "Grid")
            .GetComponentInChildren<Tilemap>();
        
        var collisionObj = collision.collider.gameObject;

        var vectorSum = collision.contacts
            .ToList()
            .Select(cp2d => cp2d.point)
            .Aggregate(new Vector2(), (acc, val) => new Vector2(acc.x + val.x, acc.y + val.y));

        var nContacts = collision.contacts.Length;
        var avgContactPoint = new Vector2(vectorSum.x / nContacts, vectorSum.y / nContacts);
        
        if (collisionObj.GetComponent<Tilemap>() || collisionObj.GetComponent<Player>())
        {
            
            var cellPosition= tilemap.WorldToCell(new Vector3(avgContactPoint.x, avgContactPoint.y));
            tilemap.SetTile(cellPosition, null);
            var center = new Vector2(cellPosition.x, cellPosition.y);

            for (var x = cellPosition.x - explosionRadius; x <= cellPosition.x + explosionRadius; x += 0.5f)
            {
                for (var y = cellPosition.y - explosionRadius; y <= cellPosition.y + explosionRadius; y += 0.5f)
                {
                    var tile = new Vector2(x, y);
                    if (Vector2.Distance(tile, center) <= explosionRadius)
                    {
                        var toDelete = tilemap.WorldToCell(new Vector3(tile.x, tile.y));
                        tilemap.SetTile(toDelete, null);
                    }
                }
            }

            var explRadiusForPlayers = 0.95f * Mathf.Sqrt(2);
            GameManager.Instance.Players
                .FindAll(DistanceFromPointLessOrEqual(avgContactPoint, explRadiusForPlayers))
                .ForEach(HandleBulletDamage);
        }
    }

    private void HandleBulletDamage(GameObject go)
    {
        var player = go.GetComponent<Player>();
        var damage = DmgCalculator.CalculateBulletDamage(startingPoint, transform.position);
        player.TakeDamage(damage);
    }

    private Predicate<GameObject> DistanceFromPointLessOrEqual(Vector2 pt, float maxDist)
    {
        return player =>
        {
            var playerPt = player.transform.position;
            var playerX = playerPt.x;
            var playerY = playerPt.y;

            var playerPos2D = new Vector2(playerX, playerY);
            var dist = Vector2.Distance(pt, playerPos2D);

            return dist <= maxDist;
        };
    }
    
}