using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tools
{
    public static class RaycastDetector
    {
        private static readonly float Distance =
            Mathf.Sqrt((GameManager.Instance.mapWidth ^ 2 + GameManager.Instance.mapHeight ^ 2));
        
        public static List<GameObject> DetectEnemiesInSight(Vector3 point)
        {
            var currentTeam = GameManager.Instance.CurrentPlayer.GetComponent<Player>().team;
            var enemies = GameManager.Instance.Players.FindAll(
                go => go.GetComponent<Player>().team != currentTeam);
            var pointInRay = new Vector2(point.x, point.y);
            return (from enemy in enemies 
                let direction = enemy.gameObject.transform.position - point 
                let hit = Physics2D.Raycast(
                    pointInRay, 
                    direction,
                    Distance,
                    1 << LayerMask.NameToLayer("Ground")) 
                where !hit.collider select enemy.gameObject).ToList();
        }
    }
}