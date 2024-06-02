using Interfaces;
using UnityEngine;

public class Bazooka : MonoBehaviour, IShootable
{
    public Bullet bulletPrefab;

    public void ShootBullet()
    {
        var bulletStartPoint = gameObject.transform.GetChild(0);
        Instantiate(bulletPrefab, bulletStartPoint.position, bulletStartPoint.rotation);
    }
}