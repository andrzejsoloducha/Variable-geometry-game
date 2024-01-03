using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float turnTime = 5.0f;
    public float currentTime = 0f;

    public int totalPlayers = 3;
    public int currentPlayer = 0;
    public bool actionTaken = false;


    public int mapWidth = 60;
    public int mapHeight = 40;
    public int[,] map;
}
