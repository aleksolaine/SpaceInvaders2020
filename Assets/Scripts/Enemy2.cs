using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : Enemy
{
    public override void Initialise(int row, int column, int id)
    {
        base.Initialise(row, column, id);
        equippedAmmo = basicAmmo;
    }

    //protected override IEnumerator AtBottom()
    //{
    //    atBottom = true;
    //    while (true)
    //    {
    //        Shoot();
    //        yield return new WaitForSeconds(1f);
    //    }
    //}



    protected override void OnDestroy()
    {
        if (GameManager.manager.levelManager.levelEnd) return;
        base.OnDestroy();
        GameManager.manager.levelManager.GetPoints(50);
    }

}