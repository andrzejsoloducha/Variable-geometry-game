using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tools
{
    public static class RaycastDetector
    {
        private static readonly float Distance =
            Mathf.Sqrt((GameManager.Instance.mapWidth ^ 2 + GameManager.Instance.mapHeight ^ 2));
        
        public static List<GameObject> DetectEnemiesInSight(Vector3Int point)
        {
            var currentTeam = GameManager.Instance.CurrentPlayer.GetComponent<Player>().team;
            var enemies = GameManager.Instance.Players.FindAll(
                go => go.GetComponent<Player>().team != currentTeam);
            var pointInRay = new Vector3(point.x, point.y, 0);
            var list = new List<GameObject>();
            foreach (var enemy in enemies)
            {
                var direction = enemy.gameObject.transform.position - point;
                var distance = Vector3.Distance(enemy.gameObject.transform.position, point);
                var hit = Physics2D.Raycast(pointInRay, direction, distance, 1 << LayerMask.NameToLayer("Ground"));
                if (!hit.collider) list.Add(enemy.gameObject);
            }

            return list;
        }
    }
}