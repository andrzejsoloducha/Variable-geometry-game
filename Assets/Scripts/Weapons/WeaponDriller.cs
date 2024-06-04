using UnityEngine;
using UnityEngine.Tilemaps;
using Vector3 = System.Numerics.Vector3;

public class WeaponDriller : MonoBehaviour
{
    public LayerMask ground;
    public int maxTilesDestroyed = 5;
    private int tilesDestroyed;
    private readonly int reducedMoveSpeed = 3;
    public GameObject puffPrefab;

    private void ReducePlayerMoveSpeed(float moveSpeed)
    {
        GameManager.Instance.CurrentPlayer.GetComponent<Player>().MoveSpeed = moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.Instance.weaponUsed = true;
        if ((ground.value & (1 << collision.gameObject.layer)) != 0)
        {
            var tm = collision.gameObject.GetComponent<Tilemap>();
            var collidedTilePos= tm.WorldToCell(gameObject.transform.position);
            var inc = (int) Input.GetAxisRaw("Horizontal");
            collidedTilePos = new Vector3Int(collidedTilePos.x + inc, collidedTilePos.y, collidedTilePos.z);
            
            if (tilesDestroyed < maxTilesDestroyed)
            {
                // Destroy(tile);
                tm.SetTile(collidedTilePos, null);
                Instantiate(puffPrefab, transform.position, Quaternion.identity);
                ReducePlayerMoveSpeed(reducedMoveSpeed);
                GameManager.Instance.weaponUsed = true;
                tilesDestroyed++;
            }

            if (tilesDestroyed == maxTilesDestroyed)
            {
                ResetDriller();
            }
        }
    }

    public void ResetDriller()
    {
        tilesDestroyed = 0;
        ReducePlayerMoveSpeed(5);
        gameObject.SetActive(false);
    }

    public void FlipDriller(float direction)
    {
        if (direction > 0 && transform.localPosition.x < 0 ||
            direction < 0 && transform.localPosition.x > 0)
        {
            transform.localPosition = new UnityEngine.Vector3(
                -transform.localPosition.x, 
                transform.localPosition.y,
                transform.localPosition.z);
        }
    }
}