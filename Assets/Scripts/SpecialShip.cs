using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialShip : MonoBehaviour
{
    private bool firstWall = true;
    // Start is called before the first frame update
    void Start()
    {
        Vector2 dir = new Vector2(0f - transform.position.x, 0).normalized;
        GetComponent<Rigidbody2D>().velocity = dir * 5;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            if (firstWall)
            {
                firstWall = false;
            }
            else
            {
                GameManager.manager.levelManager.StartBonusShipSpawner();
                Destroy(gameObject, 2f);
            }
        }
        //THIS VERY BANDAID FIX
        else if (collision.GetComponent<BasicAmmo>())
        {
            Debug.Log("ALL THE POINTS");
            GameManager.manager.levelManager.GetPoints(500);
            GameManager.manager.levelManager.StartBonusShipSpawner();
            Destroy(gameObject);
        }
    }
}