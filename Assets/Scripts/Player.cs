using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 10f;
    private bool[] isFacingRight;

    public GameManager gameManager;
    public GameObject bulletPrefab;

    public Rigidbody2D[] rigidbodies;
    public Vector2 boxSize = new Vector2(0.8f, 0.2f);
    public LayerMask groundLayer;
    public int currentPlayer;
    public GameObject[] playersArray;
    public GameObject bazookaPrefab;
    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> bazookas = new List<GameObject>();
    public string playerLayerName = "Player";
    public Vector3 scale;
    private string _team;
    public weaponScript weaponSc;


    void Start()
    {
        weaponSc = GameObject.Find("weaponHolder").GetComponent<weaponScript>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in playersArray)
        {
            players.Add(player);
        }

        rigidbodies = new Rigidbody2D[gameManager.totalPlayers];
        isFacingRight = new bool[gameManager.totalPlayers];

        for (int i = 0; i < gameManager.totalPlayers; i++)
        {
            rigidbodies[i] = playersArray[i].GetComponent<Rigidbody2D>();
            isFacingRight[i] = true;
        }
        groundLayer = LayerMask.GetMask("Ground");
        
    }

    void Update()
    {
        int currentPlayer = gameManager.currentPlayer;
        GameObject currPlayer = GetPlayer(currentPlayer);

        CheckPlayerInput(currentPlayer);
        Flip(currentPlayer);
    }

    private void CheckPlayerInput(int currentPlayer)
    {
        Rigidbody2D rb = rigidbodies[currentPlayer];

        horizontal = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            rigidbodies[currentPlayer].velocity = new Vector2(rigidbodies[currentPlayer].velocity.x, jumpingPower);
        }

        if (Input.GetButtonUp("Jump") && rigidbodies[currentPlayer].velocity.y > 0f)
        {
            rigidbodies[currentPlayer].velocity = new Vector2(rigidbodies[currentPlayer].velocity.x, rigidbodies[currentPlayer].velocity.y * 0.5f);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            weaponSc.ShootBazooka();
        }
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

    public string team
    {
        get { return _team; }
        set { _team = value.ToLower(); }
    }

    //private void ShootBazooka(int currentPlayer, List<GameObject> bazookas, GameObject bulletPrefab)
    //{
    //    if (bulletPrefab != null)
    //    {
    //        if (bazookas[currentPlayer] != null)
    //        {
    //            Transform shootingPoint = bazookas[currentPlayer].transform.GetChild(0);
    //            if (shootingPoint != null)
    //            {
    //                GameObject shootingBullet = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);

    //                Bullet Bullet = shootingBullet.GetComponent<Bullet>();
    //                if (Bullet != null)
    //                {
    //                    Bullet.SetShootingPoint(shootingPoint.position);
    //                }
    //            }
    //            else
    //            {
    //                Debug.LogError("No shooting_point found in bazooka");
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError("Bazooka has not been found");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("bulletPrefab is not assigned");
    //    }
    //}

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