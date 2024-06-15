using UnityEngine;

namespace Tools
{
    public static class DmgCalculator
    {
        public static int CalculateBulletDamage(Vector3 startPoint, Vector3 endPoint)
        {
            const int minDamage = 30;
            const int maxDamage = 65;
            const int maxDistance = 12;
            var distance = Vector3.Distance(startPoint, endPoint);

            if (distance <= 5)
            {
                return maxDamage;
            }

            if (distance >= maxDistance)
            {
                return minDamage;
            }

            const int damageRange = maxDamage - minDamage;
            const int slope = -damageRange / maxDistance;

            return (int)(maxDamage + slope * distance);
        }
    }
}