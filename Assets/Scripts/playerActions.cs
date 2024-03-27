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
    public GameObject[] playersArray;
    public GameObject player;
    private List<GameObject> players = new List<GameObject> ();
    public string playerLayerName = "Player";
    public Vector3 scale;
    //private Vector3[] localScales;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");
        //Debug.Log("Number of players: " + players.Length);

        foreach (GameObject player in playersArray)
        {
            players.Add(player);
        }

        rigidbodies = new Rigidbody2D[playersArray.Length];
        isFacingRight = new bool[playersArray.Length];

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
        for (int i = 0; i < playersArray.Length; i++)
        {
            rigidbodies[i] = playersArray[i].GetComponent<Rigidbody2D>();
            isFacingRight[i] = true;
        }
        groundLayer = LayerMask.GetMask("Ground");
    }

    public GameObject GetPlayer(int index)
    {
        if (index >= 0 && index < players.Count)
        {
            return players[index];
        }
        else
        {
            Debug.LogError("invalid player index, returning null");
            return null;
        }
    }

    void Update()
    {
        int currentPlayer = gameManager.currentPlayer;

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

        Flip(currentPlayer);
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


    private void Flip(int currentPlayer)
    {
        if ((isFacingRight[currentPlayer] && horizontal < 0f) || (!isFacingRight[currentPlayer] && horizontal > 0f))
        {
            //isFacingRight[currentPlayer] = !isFacingRight[currentPlayer];
            //Vector3 scale = currPlayer.transform.localScale;
            //scale.x *= -1f;
            //currPlayer.transform.localScale = scale;

            //Vector3 currentRotation = currPlayer.transform.localEulerAngles;
            //currentRotation.y += 180f;
            //currPlayer.transform.localEulerAngles = currentRotation;

            GameObject currPlayer = GetPlayer(currentPlayer);
            SpriteRenderer spriteRenderer = currPlayer.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
            else
            {
                Debug.LogWarning("sprite renderer component not found on the player");
            }
        }
    }
}