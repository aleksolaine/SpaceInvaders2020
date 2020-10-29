using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHolder : MonoBehaviour
{
    public Vector2 size;
    public float maxSpeed;
    public float speed;
    public Vector2 dir;
    public List<Enemy> bottomEnemies;

    
    // Start is called before the first frame update
    void Start()
    {
        dir = Vector2.left;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    public void CallDirectionChange()
    {
        StartCoroutine(ChangeDirection());
    }

    public void updateSpeed(int enemyCount)
    {
        speed = maxSpeed / enemyCount;
        //GameManager.manager.soundManager.ChangeMusicTempo(maxSpeed, speed);
    }

    private IEnumerator ChangeDirection()
    {
        Debug.Log("Direction change");
        Vector2 saveDir = dir;
        Vector2 savePos = transform.position;
        dir = Vector2.down;
        while (Vector2.Distance(savePos, transform.position) < 1f)
        {
            yield return null;
        }
        Debug.Log("Direction change done");
        dir = saveDir * -1;
        transform.position = savePos + Vector2.down;
    }

    public void StartShooting()
    { 
        StartCoroutine(ShootMachine());
    }

    private IEnumerator ShootMachine()
    {
        while (true)
        {
            float shootCounter = Random.Range(0.1f, 2f);
            yield return new WaitForSeconds(shootCounter);
            ChooseShooter();
        }
    }

    void ChooseShooter()
    {
        int shooterIndex = Random.Range(0, bottomEnemies.Count);
        try
        {
            bottomEnemies[shooterIndex].Shoot();
        } catch
        {
            //WHY DO NULL?????
            Debug.Log("Before: " + bottomEnemies.Count);
            bottomEnemies.RemoveAt(shooterIndex);
            Debug.Log("After: " + bottomEnemies.Count);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
