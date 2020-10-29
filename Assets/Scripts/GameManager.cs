using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    public LevelManager levelManager;
    public SoundManager soundManager;
    
    

    
    public SaveData saveData;
    public SaveData tempData;
    

    private void Awake()
    {
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
        levelManager = GetComponent<LevelManager>();
        soundManager = GetComponent<SoundManager>();
        Load();
        SceneManager.LoadScene(1);
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/save.xml"))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveData));
            FileStream file = new FileStream(Application.persistentDataPath + "/save.xml", FileMode.Open);
            SaveData loadedPlayerData = xmlSerializer.Deserialize(file) as SaveData;

            saveData = new SaveData()
            {
                levelInfo = loadedPlayerData.levelInfo,
                hp = loadedPlayerData.hp,
                score = loadedPlayerData.score,
                hiscore = loadedPlayerData.hiscore
            };
            file.Close();
        } else
        {
            saveData = new SaveData()
            {
                levelInfo = 1,
                hp = 10,
                score = 0,
                hiscore = 0
            };
        }
        tempData = new SaveData();
    }
    public void Save()
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(SaveData));
        FileStream file = File.Create(Application.persistentDataPath + "/save.xml");

        xmlSerializer.Serialize(file, saveData);

        file.Close();
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Onsceneloaded: " + scene.buildIndex);
        if (scene.buildIndex == 2)
        {
            tempData.CopyData(saveData);
            StartCoroutine(levelManager.GenerateLevel(tempData.levelInfo));
        }
    }

    public IEnumerator Wait(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
    }
}