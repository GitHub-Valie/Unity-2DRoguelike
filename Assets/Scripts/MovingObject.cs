using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer; // Layer on which Collision is checked

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2d;
    private float inverseMoveTime;
    private bool isMoving;

    protected virtual void Start() // Protected methods can be called from derived classes
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    // Move
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null && !isMoving) // If nothing was hit ...
        {
            StartCoroutine(SmoothMovement(end)); // ... use SmoothMovement
            return true; // ... Move returns true
        }

        return false; // Otherwise, Move returns false
    }

    // Coroutine used to move units from one space to the next (end)
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        isMoving = true; // Set isMoving to true to enable movement

        // Logic to calculate the remaining distance to move
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // While the remaining distance is higher than ~0 ...
        while (sqrRemainingDistance > float.Epsilon)
        {
            // ... Continue moving
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime * Time.deltaTime);
            rb2d.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        
        rb2d.MovePosition(end);
        isMoving = false;
        // Debug.Log("Done moving");
}

    /* AttemptMove: specify the type of component we expect our unit 
    to interact with if blocked (Player for Enemies, Wall for Player) */
    protected virtual void AttemptMove<T>(int xDir, int yDir) where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        // Case 1: If nothing was hit by the Linecast in Move ...
        if (hit.transform == null)
        {
            return; // ... Return and do not execute the code below
        }

        /* Case 2: If something was hit, get a component reference to the 
        component of type T attached to the object that was hit */
        T hitComponent = hit.transform.GetComponent<T>();

        /* Case 2.A: If canMove is not true and hitComponent is not null
        The moving object has hit something and is blocked */
        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent); // OnCantMove handles this case depending on the inheriting class
        }
    }

    // OnCantMove: handles what happens when a moving object hits something that's blocking 
    protected abstract void OnCantMove<T>(T component) where T : Component;
}
