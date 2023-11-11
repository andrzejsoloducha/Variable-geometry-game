using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Rb_movement : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    public int currentPlayer = 0;
    public int maxPlayers;
    public GameObject[] allPlayers;
    public float turnTime = 5.0f;
    public float timer = 5.0f;
    private bool actionTaken = false;

    public Rigidbody2D rb;
    //[SerializeField] public Text timeLeftText;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    

    void Start()
    {
        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        maxPlayers = allPlayers.Length;
    }
    
    void Update()
    {
        rb = allPlayers[currentPlayer].GetComponent<Rigidbody2D>();
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            //only for tests
            actionTaken = true;
        }

        Flip();

        if (timer <= 0 | actionTaken)
        {
            EndTurn();
            SwitchPlayer();
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    void EndTurn()
    {
        timer = turnTime;
        actionTaken = false;
    }

    void SwitchPlayer()
    {
        currentPlayer += 1;

        if (currentPlayer >= maxPlayers)
        {
            currentPlayer = 0;
        }

        Rigidbody rb = allPlayers[currentPlayer].GetComponent<Rigidbody>();
    }
}