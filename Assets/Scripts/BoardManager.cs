using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    /* Board Manager procedurally generates a board each time the 
    player starts a new level based on the current level number */

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;
        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;
    public Count wallCount = new Count (5, 9); // Have between 5 and 9 walls per level
    public Count foodCount = new Count (1, 5); // Have 1 to 5 food items per level
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder; // boardHolder will be the container for all the GameObjects above
    
    /* gridPositions is a list of Vector3 x,y,z coordinates to keep track of the possible 
    positions to spawn GameObjects (enemies, food, walls) on the board */
    private List <Vector3> gridPositions = new List<Vector3>(); 

    void InitialiseList()
    {
        gridPositions.Clear(); // Clear the list

        /* Create a nested "for" loop to fill the gridPositions 
        List with all Vector3 coordinates of the board */
        
        for (int x = 1; x < columns - 1; x++ ) // for loop goes from (1,1,0) to (6,6,0)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f)); // add the Vector to the List
            }
        }
    }

    void BoardSetup() // Sets up the outer walls and the floor of the board
    {
        boardHolder = new GameObject("Board").transform;

        /* Create a nested "for" loop to lay out 
        the outer wall tiles and the floor */

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                // Randomly pick a "regular" floor tile from floorTiles and prepare to instantiate this GameObject
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                
                // Check if the (x,y) corresponds to an outerwall position (-1,-1 : 8,8)
                if (x==-1 || x==columns || y==-1 || y==rows)
                {
                    // If so, set toInstantiate to a "border" outerWallTile randomly chosen, from outerWallTiles[]
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                
                GameObject instance = Instantiate(
                    toInstantiate,
                    new Vector3 (x, y, 0f),
                    Quaternion.identity
                );

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition() // Used to place other objects (enemies, food, inner walls)
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex); // Make sure two objects cannot be spawned at the same position
        
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    /* Spawn tiles at a given random position */
    {
        int objectCount = Random.Range(minimum, maximum + 1); // How many of a given object are spawned? i.e walls, food...

        for (int i = 0; i < objectCount; i++) // As long as i < objectCount ...
        {
            // ... Randomly choose a position
            Vector3 randomPosition = RandomPosition();

            // ... Randomly choose a tile from the tileArray (array of GameObjects)
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            // Instantiate the randomly chosen tile at the randomly chosen position
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level) // The single public method, called by GameManager
    {
        BoardSetup(); // Set up the outer walls and the floor of the board
        InitialiseList(); // Store the possible spawn points in the gridPositions (list)

        // Randomly instantiate inner walls, food and enemies
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        
        int enemyCount = (int)Mathf.Log(level, 2f); // enemyCount increases logarithmically based on game level
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount); // No minimum or maximum is specified

        // The exit will always be at the top right corner of the board (7,7)
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
