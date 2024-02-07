using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponMenu : MonoBehaviour
{
    public GameObject weaponCanvas;
    public KeyCode menuKey = KeyCode.Tab;

    public Button pistolButton;
    public Button bazookaButton;
    public Button buildingButton;

    public GameObject pistolPrefab;
    public GameObject bazookaPrefab;
    public GameObject buildingPrefab;

    private GameObject player;
    public GameManager gameManager;
    private Rigidbody2D[] rigidbodies;
    private int currentWeapon;

    void Start()
    {
        weaponCanvas.SetActive(false);

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        rigidbodies = new Rigidbody2D[players.Length];
        for (int i = 0; i < players.Length; i++)
        {
            rigidbodies[i] = players[i].GetComponent<Rigidbody2D>();
        }

        currentWeapon = 0;
        SwitchWeapon(pistolPrefab);
    }

    void Update()
    {
        if (Input.GetKeyDown(menuKey))
        {
            weaponCanvas.SetActive(!weaponCanvas.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponCanvas.activeInHierarchy)
        {
            currentWeapon = 0;
            SwitchWeapon(pistolPrefab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && weaponCanvas.activeInHierarchy)
        {
            currentWeapon = 1;
            SwitchWeapon(bazookaPrefab);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && weaponCanvas.activeInHierarchy)
        {
            currentWeapon = 2;
            SwitchWeapon(buildingPrefab);
        }
    }

    void SwitchWeapon(GameObject weaponPrefab)
    {
/*        //Destroy(weapon.gameObject);
        GameObject newWeapon = Instantiate(weaponPrefab, player.transform);

        //weapon = newWeapon.GetComponent<Weapon>();
        UpdateButtons();*/
    }

    void UpdateButtons()
    {
        switch (currentWeapon)
        {
            case 0:
                pistolButton.image.color = Color.green;
                bazookaButton.image.color = Color.white;
                buildingButton.image.color = Color.white;
                break;
            
            case 1:
                pistolButton.image.color = Color.white;
                bazookaButton.image.color = Color.green;
                buildingButton.image.color = Color.white;
                break;
            
            case 2:
                pistolButton.image.color = Color.white;
                bazookaButton.image.color = Color.white;
                buildingButton.image.color = Color.green;
                break;
        }
    }
}
