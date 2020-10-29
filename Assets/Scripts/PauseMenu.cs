using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void SaveAndExit()
    {
        GameManager.manager.levelManager.SaveAndExit();
    }
}