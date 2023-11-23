using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class playerActions : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    public int maxPlayers;
    public GameObject[] allPlayers;

    public bool actionTaken = false;

    public GameObject turnScript;
    public turnTimer scTimer;
    private int currPlayer = 0;
    public Rigidbody2D rb;
    //[SerializeField] public Text timeLeftText;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    

    void Start()
    {
        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        maxPlayers = allPlayers.Length;

        GameObject turnScript = GameObject.Find("turnTimer");
        turnTimer scTimer = turnScript.GetComponent<turnTimer>();

    }
    
    void Update()
    {
        if (scTimer) {
            int currPlayer = scTimer.currentPlayer;
        } else
        {
            Debug.Log("No game object called turnScript found");
        }
        
        rb = allPlayers[currPlayer].GetComponent<Rigidbody2D>();

        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            //only for tests
            //actionTaken = true;
        }

        Flip();

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
}