using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource fxSource;
    public static SoundManager instance = null; // Singleton
    public float lowPitch = .9f;
    public float highPitch = 1.1f;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySingle(AudioClip clip)
    {
        fxSource.clip = clip;
        fxSource.Play();
    }

    public void RandomizeSfx (params AudioClip[] clips)
    {
        // Takes an array of audio to generate a random index
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitch, highPitch);

        fxSource.pitch = randomPitch;
        fxSource.clip = clips[randomIndex];
        fxSource.Play();
    }

}
