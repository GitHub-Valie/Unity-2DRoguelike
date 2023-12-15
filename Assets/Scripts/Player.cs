using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject // Inheritance
{
    public float restartLevelDelay = 1f;
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public Text foodText;

    private int food;
    private Animator animator;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

#if UNITY_IOS || UNITY_ANDROID || UNITY_IPHONE
    private Vector2 touchOrigin = -Vector2.one; // Used to store the x,y location of screen where the finger first touched the screen

#endif

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints; // Data persistence of food value between levels
        foodText.text = $"Food: {food}";
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
    
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
        {
            vertical = 0;
        }
    
#elif UNITY_IOS || UNITY_ANDROID || UNITY_IPHONE

// horizontal and vertical are set in the touch-screen specific code below

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get a reference to the first touch

            if (touch.phase == TouchPhase.Began) // If touch phase has just began ...
            {
                touchOrigin = touch.position; // ... Set touchOrigin equal to touch's x,y position
            }

            // If the finger has lifted off the screen and touchOrigin is within the boundaries of the touch screen
            else if (touch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = touch.position;
                
                // In which direction? Calculate the difference between the begining and the end of touch
                float xDelta = touchEnd.x - touchOrigin.x;
                float yDelta = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1; // Set touchOrigin.x back to -1 (off screen) so that the conditional does not repeatedly evaluate to true 
            
                // Was the finger movement more horizontal? more vertical?
                if (Mathf.Abs(xDelta) > Mathf.Abs(yDelta))
                {
                    // Finger movement is more horizontal than vertical
                    horizontal = xDelta > 0 ? 1 : -1; // x > 0 : move right / x < 0 : move left
                }
                else 
                {
                    // Finger movement is more vertical than it is horizontal
                    vertical = yDelta > 0 ? 1 : -1; // y > 0 : move up / y < 0 : move down
                }
            }
        }        

#endif

        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = $"Food: {food}";

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        Move(xDir, yDir, out hit);
        if (hit.transform == null) // If the player was able to move...
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        CheckIfGameOver(); // Check if food <= 0

        GameManager.instance.playersTurn = false;
    }

    // Handles cases where player is blocked by a wall
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
        // throw new System.NotImplementedException();
    }

    public void HitLoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = $"- {loss} Food: {food}";
        CheckIfGameOver();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = $"+ {pointsPerFood} Food: {food}";
            
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = $"+ {pointsPerFood} Food: {food}";
            
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            
            other.gameObject.SetActive(false);
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            // Play game over sound, stop music source
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            
            GameManager.instance.GameOver();
        }
    }
}
