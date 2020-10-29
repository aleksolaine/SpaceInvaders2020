using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int levelInfo;
    public int hp;
    public int score;
    public int hiscore;

    public void CopyData(SaveData copyFrom)
    {
        levelInfo = copyFrom.levelInfo;
        hp = copyFrom.hp;
        score = copyFrom.score;
        hiscore = copyFrom.hiscore;
    }
    public void NewGame()
    {
        levelInfo = 1;
        hp = 10;
        score = 0;
    }
}