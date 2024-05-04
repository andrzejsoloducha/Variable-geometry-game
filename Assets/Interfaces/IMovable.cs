﻿using UnityEngine;

public interface IMovable
{
    void Move(Vector2 direction);
    public int MoveRange { get; set; }
}