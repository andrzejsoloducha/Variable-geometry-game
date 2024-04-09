using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float turnTime;
    public float currentTime;

    public int totalPlayers;
    public int currentPlayer = 0;
    public bool actionTaken = false;


    public int mapWidth = 60;
    public int mapHeight = 40;
    public int[,] map;

    public bool deathmatch = false;
}
