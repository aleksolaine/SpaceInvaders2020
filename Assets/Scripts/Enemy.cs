using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public GameObject basicAmmo;

    protected GameObject equippedAmmo;
    public int row, column, id;
    protected float actionCounter;
    protected bool atBottom;

    public virtual void Initialise(int row, int column, int id)
    {
        this.row = row;
        this.column = column;
        this.id = id;
    }

    public void Shoot()
    {
        Instantiate(equippedAmmo, transform.position, Quaternion.identity);
    }

    public void IsBottom()
    {
        atBottom = true;
        GameManager.manager.levelManager.enemyHolder.bottomEnemies.Add(this);
    }

    //protected abstract IEnumerator AtBottom();

    protected virtual void OnDestroy()
    {
        GameManager.manager.levelManager.enemiesOnBoard.Remove(gameObject);
        GameManager.manager.levelManager.enemyHolder.bottomEnemies.Remove(this);
        GameManager.manager.levelManager.enemyIdentifiers[column][row] = 0;
        GameManager.manager.levelManager.RefreshEnemiesStatus(column);
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") && atBottom)
        {
            gameObject.GetComponentInParent<EnemyHolder>().CallDirectionChange();
        }
        else if (collision.CompareTag("Earth")) 
        {
            Destroy(gameObject);
        }
    }
}
