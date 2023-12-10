using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;

    // Loader checks if the GameManager has already been instantiated
    void Awake()
    {
        if (GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
    }
}
