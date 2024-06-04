using System;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, IMovable, IJumpable, IDamageable

{
    public float JumpForce { get; private set; }
    public int Health { get; set; }

    public float MoveSpeed { get; set; }
    private Rigidbody2D rb;
    private bool isFacingRight;
    private WeaponSwitcher weaponSwitcher;
    private Vector3 lastDirection = Vector3.zero;
    public Team team;
    public bool current; // indicates current player
    
    private GameObject CurrentWeapon()
    {
        return GameManager.Instance.CurrentPlayer.GetComponent<WeaponSwitcher>().GetCurrentWeapon();
    }

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (!rb)
        {
            Debug.LogWarning("rb is null at start!");
        }
        JumpForce = 6.5f;
        MoveSpeed = 5f;
        Health = 100;
    }

    public void TryMove()
    {
        var direction = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(direction * MoveSpeed, rb.velocity.y);
    }

    public void TryJump()
    {
        if (CanJump())
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce);
        }
    }
    public void ExtendedJump()
    {
        if (rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    public void OnPlayerDeath(Player player)
    {
        if (player.current)
        {
            GameManager.Instance.NextTurnProcedure(player);
        }
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage.");
        if (Health <= 0)
        {
            OnPlayerDeath(gameObject.GetComponent<Player>());
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
    public void TryShoot(Vector3 targetPoint = default)
    {
        CurrentWeapon().GetComponent<Bazooka>().ShootBullet(targetPoint);
    }
}