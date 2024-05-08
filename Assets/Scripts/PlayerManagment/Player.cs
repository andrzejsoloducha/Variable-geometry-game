using Interfaces;
using UnityEngine;

namespace PlayerManagment
{
    public class Player : MonoBehaviour, IMovable, IJumpable, IDamageable
    {
        public float JumpForce { get; set; }
        public int Health { get; set; }
        public int MoveRange { get; set; }
        private Rigidbody2D rb;
        public string Team { get; set; }
        private void Start()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            JumpForce = 5f;
            MoveRange = 10;
        }

        public void OnTurnStarted()
        {
            // turn starts
        }

        public void Move(Vector2 direction)
        {
            rb.MovePosition((Vector2)transform.position + direction * MoveRange);
        }

        public void Jump()
        { 
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }

        public void Die()
        {
            throw new System.NotImplementedException();
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                // player dies, maybe some animation later?
                // add update to team / event? 
                Destroy(gameObject);
            }
        }
    }
}