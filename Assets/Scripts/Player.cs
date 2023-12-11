using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject // Inheritance
{
    private int food;

    protected override void Start()
    {
        food = GameManager.instance.playerFoodPoints; // Data persistence of food value between levels

        base.Start();
    }

    private void OnDisable() // Called when the player is disabled
    {
        // Data persistence: Store the value of food in the GameManager
        GameManager.instance.playerFoodPoints = food;
    }

    void Update()
    {
        if (!GameManager.instance.playersTurn) // If it's not the player's turn, don't go further 
        {
            return;
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0;
        }

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        Debug.Log($"Food: {food}");

        base.AttemptMove<T>(xDir, yDir);

        // RaycastHit2D hit;

        CheckIfGameOver(); // Check if food <= 0

        GameManager.instance.playersTurn = false;
    }

    protected override void OnCantMove<T>(T component)
    {
        throw new System.NotImplementedException();
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            GameManager.instance.GameOver();
        }
    }
}
