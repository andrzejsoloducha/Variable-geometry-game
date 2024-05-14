using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private Player player;
    private GameManager gameManager;
    private float direction;

    public void SetPlayer(Player currentPlayer)
    {
        player = currentPlayer;
    }

    private void FixedUpdate()
    {
        CheckKeyboardInput();
    }

    private void CheckKeyboardInput()
    {
        CheckForLeftRightMovement();
        CheckForJump();
        
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void CheckForJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            player.TryJump();
        }
    }

    private void CheckForLeftRightMovement()
    {
        direction = Input.GetAxisRaw("Horizontal");
        //player.Flip(direction);
        player.Move(direction);
    }
}