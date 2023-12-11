using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Static instance of GameManager which allows it to be accessed by any other script
    public float turnDelay = .1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn;

    private int level = 4;
    private List<Enemy> enemies; // Keeps track of enemies and their movement
    private bool enemiesMoving;

    void Awake()
    {
        // Singleton pattern: a single instance of GameManager is allowed at any given time
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject); // Persistence between scenes
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        enemies.Clear(); // Clear the list of enemies upon scene setup
        boardScript.SetupScene(level);
    }

    public void GameOver()
    {
        Debug.Log("Game is over");
        enabled = false;
    }

    void Update()
    {
        if (playersTurn || enemiesMoving) // Return as long as it's playersTurn or enemies are moving
        {
            return;
        }

        // If it's not playersTurn and enemies are not moving...
        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script) // Adding enemies to the list allows the GameManager to issue orders to them
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies() // Moves enemies one at a time, in sequence
    {
        enemiesMoving = true;

        // Wait for player's turn
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++) // for each enemy in enemies list, MoveEnemy
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime); // moveTime is inherited from MovingObject
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}