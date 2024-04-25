using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponScript : MonoBehaviour
{
    int totalWeapons;
    public int currentWeaponIndex;

    public GameManager gameManager;
    public GameObject[] weapons;
    public GameObject weaponHolder;
    public GameObject currentWeapon;
 
    public List<GameObject> bazookas = new List<GameObject>();
    public GameObject[] drillers;
    public GameObject[] constructors;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        totalWeapons = weaponHolder.transform.childCount;
        weapons = new GameObject[totalWeapons];

        GameObject[] bazookasObj = GameObject.FindGameObjectsWithTag("bazooka");
        foreach (GameObject bazooka in bazookasObj)
        {
            bazookas.Add(bazooka);
        }
        GameObject[] drillers = GameObject.FindGameObjectsWithTag("driller");
        GameObject[] constructors = GameObject.FindGameObjectsWithTag("constructor");
        Debug.Log("bazookas:" + bazookas.Count);
        for (int i = 0; i < totalWeapons; i++)
        {
            weapons[i] = weaponHolder.transform.GetChild(i).gameObject;
            weapons[i].SetActive(false);
        }
    }

    void Update()
    {
        int currentPlayer = gameManager.currentPlayer;
        GameObject currBazooka = bazookas[currentPlayer];
        currBazooka.SetActive(true);
    }

    public void ShootBazooka()
    {
        Debug.Log("bazooka shooting detected");
    }
}
