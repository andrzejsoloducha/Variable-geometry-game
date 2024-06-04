using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public static class RaycastDetector
    {
        private static readonly float Distance =
            Mathf.Sqrt((GameManager.Instance.mapWidth ^ 2 + GameManager.Instance.mapHeight ^ 2));
        public static List<GameObject> DetectEnemiesInSight(GameObject currentPlayer, List<Player> players)
        {
            var enemiesInSight = new List<GameObject>();
            foreach (var enemy in players)
            {
                if (currentPlayer.GetComponent<Player>().team != enemy.team)
                {
                    Vector2 direction = enemy.gameObject.transform.position - currentPlayer.transform.position;
                    var hit = Physics2D.Raycast(currentPlayer.transform.position, direction, Distance,
                        1 << LayerMask.NameToLayer("Ground"));
                    if (!hit.collider)
                    {
                        enemiesInSight.Add(enemy.gameObject);
                    }
                }
            }
            return enemiesInSight;
        }
    }
}