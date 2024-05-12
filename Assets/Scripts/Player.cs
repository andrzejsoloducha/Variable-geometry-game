using Interfaces;
using UnityEngine;

public class Player : MonoBehaviour, IMovable, IJumpable, IDamageable
{
    public float JumpForce { get; set; }
    public int Health { get; set; }

    public int MoveSpeed { get; set; }
    private Rigidbody2D rb;
    private bool isFacingRight;
    public string Team { get; set; }
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        JumpForce = 5f;
        MoveSpeed = 10;
    }

    public void OnTurnStarted()
    {
        // turn starts
    }

    public void Move(float direction)
    {
        rb.velocity = new Vector2(direction * MoveSpeed, rb.velocity.y);
    }

    public void TryJump()
    {
        if (CanJump())
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
        }
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
            Die();
        }
    }

    public void Flip(float direction)
    {
        if ((isFacingRight && direction < 0f) || (!isFacingRight && direction > 0f))
        {
            isFacingRight = !isFacingRight;
            var spr = gameObject.GetComponent<SpriteRenderer>();
            if (spr)
            {
                spr.flipX = !spr.flipX;
            }
            else
            {
                Debug.LogWarning("Sprite renderer not found on the player");
            }
        }

    }

    private bool CanJump()
    {
        var playerLayer = LayerMask.NameToLayer("Player");
        Vector2 position = rb.transform.position;
        var size = rb.GetComponent<BoxCollider2D>().size;
        var offset = new Vector2(0, -size.y / 2f);
        const float radius = 0.1f;

        var groundLayer = LayerMask.GetMask("Ground");
        bool canJump = Physics2D.OverlapCircle(position + offset, radius, groundLayer);
        if (!canJump)
        {
            var hit = Physics2D.Raycast(position + offset, Vector2.down, size.y / 2f + 0.1f, playerLayer);
            canJump = hit.collider;
        }

        return canJump;
    }
}