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
    public bool grounded;

    public Vector2 boxSize = new Vector2(0.8f, 0.2f);
    public float castDistance = 0.5f;
    public LayerMask groundLayer;


    void Start()
    {
        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        maxPlayers = allPlayers.Length;

        groundLayer = LayerMask.GetMask("Ground");

        GameObject turnScript = GameObject.Find("turnTimer");
        turnTimer scTimer = turnScript.GetComponent<turnTimer>();
    }

    void Update()
    {
        if (scTimer)
        {
            int currPlayer = scTimer.currentPlayer;
        }
        else
        {
            Debug.Log("No game object called scTimer found");
        }

        rb = allPlayers[currPlayer].GetComponent<Rigidbody2D>();

        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        Flip();

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    public bool isGrounded()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }
    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.gameObject.CompareTag("Ground"))
    //    {
    //        Vector3 normal = other.GetContact(0).normal;
    //        if (normal == Vector3.up)
    //        {
    //            grounded = true;
    //        }
    //    }
    //}

    //private void OnCollisionExit2D(Collision2D other)
    //{
    //    if (other.gameObject.CompareTag("Ground"))
    //    {
    //        grounded = false;
    //    }
    //}

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