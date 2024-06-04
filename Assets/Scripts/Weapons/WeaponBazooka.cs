using Interfaces;
using UnityEngine;

public class Bazooka : MonoBehaviour, IShootable
{
    public Bullet bulletPrefab;

    public void ShootBullet(Vector3 targetPoint = default)
    {
        var bulletStartPoint = gameObject.transform.GetChild(0);
        var bullet = Instantiate(bulletPrefab, bulletStartPoint.position, bulletStartPoint.rotation);
        bullet.GetComponent<Bullet>().GetDirection(targetPoint);
    }

    public void RotateBazookaToMouseInput(Vector3 mousePosition)
    {
        var mousePos = Camera.main.ScreenToWorldPoint(mousePosition);
        var direction = mousePos - transform.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}