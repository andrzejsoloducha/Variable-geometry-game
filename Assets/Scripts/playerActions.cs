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

    public GameManager gameManager;

    //public GameObject[] allPlayers;

    public Rigidbody2D[] rigidbodies;
    public Vector2 boxSize = new Vector2(0.8f, 0.2f);
    public float castDistance = 0.5f;
    public LayerMask groundLayer;
    public int currentPlayer;


    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        rigidbodies = new Rigidbody2D[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            rigidbodies[i] = players[i].GetComponent<Rigidbody2D>();
        }

        groundLayer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        int currentPlayer = gameManager.currentPlayer;
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            rigidbodies[currentPlayer].velocity = new Vector2(rigidbodies[currentPlayer].velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rigidbodies[currentPlayer].velocity.y > 0f)
        {
            rigidbodies[currentPlayer].velocity = new Vector2(rigidbodies[currentPlayer].velocity.x, rigidbodies[currentPlayer].velocity.y * 0.5f);
        }

        Flip();

    }

    private void FixedUpdate()
    {
        rigidbodies[currentPlayer].velocity = new Vector2(horizontal * speed, rigidbodies[currentPlayer].velocity.y);
    }

    public bool isGrounded()
    {
        int currentPlayer = gameManager.currentPlayer;

        Vector2 position = rigidbodies[currentPlayer].transform.position;
        Vector2 size = rigidbodies[currentPlayer].GetComponent<BoxCollider2D>().size;

        Vector2 offset = new Vector2(0, -size.y / 2f);
        float radius = 0.1f;

        bool grounded = Physics2D.OverlapCircle(position + offset, radius, groundLayer);

        return grounded;
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