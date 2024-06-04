using UnityEngine;

namespace Interfaces
{
    public interface IShootable
    {
        void ShootBullet(Vector3 targetPoint = default);
    }
}
