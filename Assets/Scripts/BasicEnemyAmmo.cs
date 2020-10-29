using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyAmmo : MonoBehaviour
{
    public float speed;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, -speed);
        Destroy(gameObject, 10f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.GetComponent<Player>().TakeDamage();
        }
        Destroy(gameObject);
    }
}
