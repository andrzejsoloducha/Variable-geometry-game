using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class playerActions : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 10f;
    private bool[] isFacingRight;

    public GameManager gameManager;

    public Rigidbody2D[] rigidbodies;
    public Vector2 boxSize = new Vector2(0.8f, 0.2f);
    public LayerMask groundLayer;
    public int currentPlayer;
    public GameObject[] players;
    public GameObject player;
    public string playerLayerName = "Player";
    public Vector3 scale;
    //private Vector3[] localScales;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log("Number of players: " + players.Length);
        rigidbodies = new Rigidbody2D[players.Length];
        isFacingRight = new bool[players.Length];

        // debugging players[]
        //for (int i = 0; i < players.Length; i++)
        //{
        //    if (playersList[i] != null && playersList[i].activeSelf)
        //    {
        //        Debug.Log("znalaz³em aktywnego gracza: " + playersList[i].name);
        //    }
        //    else
        //    {
        //        Debug.LogWarning("znalazlem nulla jebanego");
        //    }
        //}
        for (int i = 0; i < players.Length; i++)
        {
            rigidbodies[i] = players[i].GetComponent<Rigidbody2D>();
            isFacingRight[i] = true;
        }
        groundLayer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        int currentPlayer = gameManager.currentPlayer;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject player = players[currentPlayer]; // DLACZEGO TO KURWA NULLUJE

        Rigidbody2D rb = rigidbodies[currentPlayer];
        Debug.Log("Current player index: " + currentPlayer);
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            rigidbodies[currentPlayer].velocity = new Vector2(rigidbodies[currentPlayer].velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rigidbodies[currentPlayer].velocity.y > 0f)
        {
            rigidbodies[currentPlayer].velocity = new Vector2(rigidbodies[currentPlayer].velocity.x, rigidbodies[currentPlayer].velocity.y * 0.5f);
        }

        if (player != null)
        {
            Flip(currentPlayer, player);
        }
        else
        {
            Debug.LogError("player jest nullem");
        }
        //if (players.Length > 0 && playersList[currentPlayer] != null)
        //{
        //    GameObject player = playersList[currentPlayer];
        //    Debug.Log("Player name: " + player.name);
        //    Flip(currentPlayer, player);
        //}
        //else
        //{
        //    Debug.LogError("players[] jest jebanym nullem???");
        //}


    }

    private void FixedUpdate()
    {
        int currentPlayer = gameManager.currentPlayer;
        rigidbodies[currentPlayer].velocity = new Vector2(horizontal * speed, rigidbodies[currentPlayer].velocity.y);
    }

    public bool isGrounded()
    {
        int currentPlayer = gameManager.currentPlayer;
        int playerLayer = LayerMask.NameToLayer(playerLayerName);

        Vector2 position = rigidbodies[currentPlayer].transform.position;
        Vector2 size = rigidbodies[currentPlayer].GetComponent<BoxCollider2D>().size;

        Vector2 offset = new Vector2(0, -size.y / 2f);
        float radius = 0.1f;

        bool grounded = Physics2D.OverlapCircle(position + offset, radius, groundLayer);

        if (!grounded)
        {
            RaycastHit2D hit = Physics2D.Raycast(position + offset, Vector2.down, size.y / 2f + 0.1f, playerLayer); // tu jest coœ zjebane, do poprawki potem
            if (hit.collider != null)
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
                grounded = true;
            }
        }

        return grounded;
    }


    private void Flip(int currentPlayer, GameObject player)
    {
        if ((isFacingRight[currentPlayer] && horizontal < 0f) || (!isFacingRight[currentPlayer] && horizontal > 0f))
        {
            //Vector3 currentRotation = player.transform.localEulerAngles;
            //currentRotation.y += 180f;
            //player.transform.localEulerAngles = currentRotation;

            isFacingRight[currentPlayer] = !isFacingRight[currentPlayer];
            Vector3 scale = player.transform.localScale;
            scale.x *= -1f;
            player.transform.localScale = scale;
        }
    }
}