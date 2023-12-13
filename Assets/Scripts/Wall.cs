using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    /* Wall script allows the player to destroy wall tiles blocking the way */
    public Sprite dmgSprite; // Displayed when the player damages the wall
    public int hp = 4; // Wall health point
    public AudioClip chopSound1;
    public AudioClip chopSound2;
    
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DamageWall(int loss)
    {
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        spriteRenderer.sprite = dmgSprite; // Visual feedback when damaging the wall
        hp -= loss;
        if (hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
