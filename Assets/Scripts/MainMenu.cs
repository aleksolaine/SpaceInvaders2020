using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 100, 30), "Continue"))
        {
            SceneManager.LoadScene(2);
        }
        if (GUI.Button(new Rect(10, 110, 100, 30), "New Game"))
        {
            GameManager.manager.saveData.NewGame();
            SceneManager.LoadScene(2);
        }
        if (GUI.Button(new Rect(10, 150, 100, 30), "Options"))
        {
            Debug.Log("Options");
        }
        if (GUI.Button(new Rect(10, 190, 100, 30), "Quit Game"))
        {
            GameManager.manager.QuitGame();
        }
    }
}