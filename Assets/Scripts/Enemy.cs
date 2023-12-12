using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    private Transform target; // Used to store the player's position
    private bool skipMove;
    private Animator animator;

    protected override void Start()
    {
        // The enemy script adds itself to the list of enemies in the GameManager
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start(); // box collider, rb2d, inverseMoveTime
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    // In this case, pass in the Player component which the enemy will interact with (move towards, attack)
    {
        if (skipMove) // If skipMove is true, set it to false and do nothing else
        {
            skipMove = false;
            return; // Enemy will not attempt move this turn
        }

        base.AttemptMove<T>(xDir, yDir);
        
        skipMove = true; // Once the enemy attempts to move, set skipMove to true
    }

    public void MoveEnemy() // Called by the GameManager when it issues the order to move to the enemies
    {
        // Implement logic to move enemy
        // Debug.Log("Enemy is moving");

        int xDir = 0;
        int yDir = 0;

        /* Check if the difference between the target's x-axis transform position 
        and the enemy's x-axis transform position is less than epsilon (~0) */
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            /* If that's the case, the enemy and the target player are in the same column 
            In that case, check if the target is above the enemy */
            yDir = target.position.y > transform.position.y ? 1 : -1; // If so, move up 1, otherwise, move down -1
        }
        else
        {
            /* If the target and the enemy are not on the same column, move left or right, accordingly:
            If the target is to the right of the enemy, move right 1, otherwise move left -1 */
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<Player>(xDir, yDir); // Attempt move towards the player using xDir, yDir
    }

    // Finally, what happens if the enemy attempts to move where the player is standing?
    protected override void OnCantMove<T>(T component) 
    {
        // Enemy's overriden OnCantMove handles cases where the enemy is blocked by the player
        
        // Implement logic to hit player and make them lose food  
        Player hitPlayer = component as Player;
        hitPlayer.HitLoseFood(playerDamage);
        animator.SetTrigger("enemyAttack");
        
        // Debug.Log("Enemy is attacking player");
        // throw new System.NotImplementedException();
    }
}
