using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Static instance of GameManager which allows it to be accessed by any other script
    public float levelStartDelay = 2f; // Time to wait before starting levels
    public float turnDelay = .1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn;

    private Text levelText;
    private GameObject levelImage;
    private int level = 0;
    private List<Enemy> enemies; // Keeps track of enemies and their movement
    private bool enemiesMoving;
    private bool doingSetup;

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
        InitGame(); // Call InitGame to initialize the first level
    }

    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = $"Day {level}";
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear(); // Clear the list of enemies upon scene setup
        boardScript.SetupScene(level);
    }

    public void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    // OnLevelFinishedLoading is called each time a scene is loaded
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        level++; // Add one to level
        InitGame(); // Call InitGame to clear enemies list and setup a new level
    }

    void OnEnable()
    {
        /* Tell OnLevelFinishedLoading to listen for a scene change event 
        as soon as this script is enabled */
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        /* Tell OnLevelFinishedLoading to stop listening for a scene change
        event as soon as this script is disabled */
        // Always have an unsubscription for every delegate you subscribe to
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        Debug.Log("Level finished loading");
    }

    public void GameOver()
    {
        levelImage.SetActive(true);
        levelText.text = $"After {level} days, you starved.";
        enabled = false;
    }

    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup) // Return as long as it's playersTurn or enemies are moving
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