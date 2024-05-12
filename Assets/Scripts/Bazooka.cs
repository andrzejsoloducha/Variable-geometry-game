using Interfaces;
using UnityEngine;
using Weapons;

public class Bazooka : MonoBehaviour, IShootable
{
    public Bullet bulletPrefab;

    public void Shoot(Bullet bullet)
    {
        Bullet launchedBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    }
}