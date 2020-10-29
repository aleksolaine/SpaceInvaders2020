using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAmmo : MonoBehaviour
{
    public float speed;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed);
        Destroy(gameObject, 10f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
