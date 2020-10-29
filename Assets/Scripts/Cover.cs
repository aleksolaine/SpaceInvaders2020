using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public Sprite[] coverSprites;
    private int health = 3;

    private void Start()
    {
        health = 3;
        GetComponent<SpriteRenderer>().sprite = coverSprites[3];
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
        {
            TakeDamage();
        } else
        {
            GetDestroyed();
        }
    }

    private void TakeDamage()
    {
        health--;
        if (health <= 0)
        {
            GetDestroyed();
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = coverSprites[health];
        }
    }

    private void GetDestroyed()
    {
        GetComponent<SpriteRenderer>().sprite = coverSprites[0];
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
