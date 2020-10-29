using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int numberOfEnemies;                 //How many enemies to spawn
    public bool levelEnd;                       //True when level has ended, helps with cleaning the game board
    public Vector2[][] enemySpawnPoints;        //Matrix of coordinates where enemies can be spawned
    public int[][] enemyIdentifiers;            //Matrix that signifies which enemy is where, corresponding to the above spawnpoint matrix
    public GameObject[] enemyPrefabs;           //All possible enemy prefabs to spawn
    public GameObject[] coverPrefabs;           //All possible COVERS to spawn
    public EnemyHolder enemyHolder;             //Gameobject that acts as parent for all enemy gameobjects
    public GameObject[] bonusShips;             //All bonus spoint ship prefabs
    public GameObject[] bonusSpawns;            //Spawn points (left & right) for bonus ships
    public List<EnemyInfo> enemiesToSpawn;      //List of which enemies to spawn with only metadata
    public List<GameObject> enemiesOnBoard;     //List of all enemies' gameobjects currently active on the board TÄSTÄ PITÄS HANKKIUTUA EROON
    public GameObject levelEndText;             //Text to indicate a level's been passed
    public Text score;                          //Current score displayed on screen
    public Text hiScore;                        //Current hi-score displayed on screen
    private Text levelStartText;
    public GameObject HPHolder;                 //Empty gameobject holding the remaining HP images
    public GameObject pausePanel;               //Transparent UI panel to display when game is paused
    private GameObject startPanel;
    public class EnemyInfo
    {
        public int row, column, type, ID;   //Coordinates, type and individual identifier of an enemy
        public EnemyInfo(int row, int column, int type)
        {
            this.row = row;
            this.column = column;
            this.type = type;
        }
    }

    private void Start()
    {
        enemiesToSpawn = new List<EnemyInfo>();
        enemiesOnBoard = new List<GameObject>();

        //Initialise the array with all potential spawn points for enemies
        enemySpawnPoints = new Vector2[19][];
        int x = 0;
        for (float i = -9f; i < 10; i++)
        {
            enemySpawnPoints[x] = new Vector2[9];
            int y = 0;

            for (float j = 4f; j > -5; j--)
            {
                enemySpawnPoints[x][y] = new Vector2(i, j);
                y++;
            }
            x++;
        }
    }

    //Set up a level to be played
    public IEnumerator GenerateLevel(int levelInfo)
    {
        Time.timeScale = 0;
        pausePanel = GameObject.Find("PausePanel");
        //Hide pausePanel until needed
        pausePanel.SetActive(false);
        startPanel = GameObject.Find("StartPanel");
        startPanel.GetComponentInChildren<Text>().text = "Level " + levelInfo;
        levelEndText = GameObject.Find("LevelCompleteText");
        //Hide the levelEndText until needed
        levelEndText.SetActive(false);
        //Make sure levelEnd is false to enable normal gameplay
        levelEnd = false;
        
        
        
        bonusSpawns = GameObject.FindGameObjectsWithTag("BonusSpawn");
        score = GameObject.Find("Score").GetComponent<Text>();
        score.text = GameManager.manager.saveData.score.ToString();
        hiScore = GameObject.Find("Hi-Score").GetComponent<Text>();
        hiScore.text = GameManager.manager.saveData.hiscore.ToString();
        HPHolder = GameObject.Find("HPHolder");

        SpawnEnemies(levelInfo);

        StartCoroutine(GameManager.manager.soundManager.ChangeMusicTempo());

        //Find all covers left from previous sessions and destroy them to avoid duplicates and/or destroyed covers at start of level
        GameObject[] coversToReplace = GameObject.FindGameObjectsWithTag("Cover");
        foreach (GameObject cover in coversToReplace)
        {
            Destroy(cover);
        }

        //Spawn covers TEMPORARY
        float x_test = -8;
        while (x_test < 8.5)
        {
            Instantiate(coverPrefabs[0], new Vector2(x_test, -6), Quaternion.identity);
            x_test += (16/3);
        }

        //Hide all HP icons that have indexes above current HP
        for(int x = GameManager.manager.saveData.hp; x < 10; x++)
        {
            HPHolder.transform.GetChild(x)
            .gameObject.SetActive(false);
        }

        //Find all the enemies that can shoot
        AssignAllShootRoles();

        //Start spawning bonus ships at random intervals
        StartCoroutine(BonusShipSpawner());
        yield return new WaitForSecondsRealtime(2);

        startPanel.SetActive(false);

        //Unpause level MAYBE MAKE A LEVEL START PANEL TO SHOW THAT GETS HIDDEN HERE
        Time.timeScale = 1;
        //Make sure player has control of their ship THIS COULD BE DONE WITH NEW INPUT SYSTEM
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().canControl = true;
        enemyHolder.StartShooting();
    }

    private void SpawnEnemies(int levelInfo)
    {
        enemyHolder = GameObject.Find("EnemyHolder").GetComponent<EnemyHolder>();

        //Clear the lists of enemies and the coordinate system of possible past level entries
        enemiesToSpawn.Clear();
        enemiesOnBoard.Clear();
        enemyIdentifiers = new int[19][];
        for (int x = 0; x < 19; x++)
        {
            enemyIdentifiers[x] = new int[9];
        }

        if (levelInfo > 0)
        {
            BasicSpawn();
        } else
        {
            CustomSpawn(levelInfo);
        }

        enemyHolder.updateSpeed(enemiesOnBoard.Count);
    }

    void BasicSpawn()
    {
        int id = 1;
        int type;
        for (int row = 0; row < 5; row++)
        {
            switch (row)
            {
                case 0:
                    type = 2;
                    break;
                case 1:
                    type = 1;
                    break;
                case 2:
                    type = 1;
                    break;
                case 3:
                    type = 0;
                    break;
                default:
                    type = 0;
                    break;
            }
            for (int column = 5; column < 16; column++)
            {
                enemyIdentifiers[column][row] = id;
                enemiesToSpawn.Add(new EnemyInfo(row, column, type));
                id++;
            }
        }

        //Spawn enemies as children of enemyHolder, give the enemy objects the data of themselves and add them on the list of active enemies
        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            GameObject nextEnemy = Instantiate(enemyPrefabs[enemiesToSpawn[i].type], enemyHolder.transform);
            nextEnemy.transform.localPosition = enemySpawnPoints[enemiesToSpawn[i].column][enemiesToSpawn[i].row];
            nextEnemy.GetComponent<Enemy>().Initialise(enemiesToSpawn[i].row, enemiesToSpawn[i].column, enemiesOnBoard.Count);
            enemiesOnBoard.Add(nextEnemy);
        }
    }

    void CustomSpawn(int levelInfo)
    {
        //Find number of enemies to spawn TEMPORARY
        numberOfEnemies = NumberOfEnemies(levelInfo);


        //Decide where to spawn them and avoid duplicates TEMPORARY
        for (int i = 1; i <= numberOfEnemies; i++)
        {
            int row, column;

            do
            {
                row = Random.Range(0, 9);
                column = Random.Range(0, 19);
            } while (enemyIdentifiers[column][row] > 0);

            enemyIdentifiers[column][row] = i;
            enemiesToSpawn.Add(new EnemyInfo(row, column, 0));
        }

        //Spawn enemies as children of enemyHolder, give the enemy objects the data of themselves and add them on the list of active enemies
        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            GameObject nextEnemy = Instantiate(enemyPrefabs[enemiesToSpawn[i].type], enemyHolder.transform);
            nextEnemy.transform.localPosition = enemySpawnPoints[enemiesToSpawn[i].column][enemiesToSpawn[i].row];
            nextEnemy.GetComponent<Enemy>().Initialise(enemiesToSpawn[i].row, enemiesToSpawn[i].column, enemiesOnBoard.Count);
            enemiesOnBoard.Add(nextEnemy);
        }
    }

    int NumberOfEnemies(int levelInfo)
    {
        if (levelInfo > 0)
        {
            return 55;
        } else
        {
            return Mathf.Clamp(levelInfo, 0, 171);
        }
    }

    public void AssignAllShootRoles()
    {
        for (int x = 0; x < 19; x++)
        {
            RefreshEnemiesStatus(x);
        }
    }
    //Finds the lowest enemy on a column and assigns it the role of a shooter
    //If it can't find an enemy on that column, it will check if there are any enemies left and end the level if there are none
    public void RefreshEnemiesStatus(int column)
    {
        int lowestEnemyRow = -1;
        for (int j = 8; j > -1; j--)
        {
            if (enemyIdentifiers[column][j] > 0)
            {
                lowestEnemyRow = j;
                break;
            }
        }
        if (lowestEnemyRow > -1)
        {
            foreach (GameObject enemy in enemiesOnBoard)
            {
                if (enemy.GetComponent<Enemy>().id == enemyIdentifiers[column][lowestEnemyRow] - 1)
                {
                    enemy.GetComponent<Enemy>().IsBottom();
                }
            }
            enemyHolder.updateSpeed(enemiesOnBoard.Count);
        } else if (enemiesOnBoard.Count == 0)
        {
            StopAllCoroutines();
            StartCoroutine(LevelComplete());
        }
        
    }
    public void StartBonusShipSpawner()
    {
        StartCoroutine(BonusShipSpawner());
    }
    private IEnumerator BonusShipSpawner()
    {
        //Start counter from zero
        float bonusCounter = 0f;
        //Find a random time to spawn between 2 and 19 seconds
        float timeToSpawn = Random.Range(2, 20);
        Debug.Log("TimeToSpawn = " + timeToSpawn);
        //Wait until spawn time is reached
        while (bonusCounter < timeToSpawn)
        {
            yield return new WaitForSeconds(0.33f);
            bonusCounter += 0.33f;
        }
        //Randomise whether ship spawns from left or right
        int bonusSpawnIndex = Random.Range(0, 2);
        //Randomise which ship gets spawned
        int shipToSpawn = Random.Range(0, bonusShips.Length);
        //Spawn selected ship at selected spawn
        GameObject bonusShip = Instantiate(bonusShips[shipToSpawn], bonusSpawns[bonusSpawnIndex].transform);
        bonusShip.transform.localPosition = Vector2.zero;
    }

    private IEnumerator LevelComplete()
    {
        enemyHolder.StopAllCoroutines();
        //Take control away from player
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().canControl = false;
        //Display correct levelEndText
        levelEndText.GetComponent<Text>().text = "Level " + GameManager.manager.saveData.levelInfo + " Complete!";
        levelEndText.SetActive(true);

        //Freeze game and show text for 2 seconds
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2f);

        //Sort out data and load scene for next level
        GameManager.manager.tempData.levelInfo++;
        GameManager.manager.saveData.CopyData(GameManager.manager.tempData);
        levelEnd = true;
        SceneManager.LoadScene(2);
    }

    //When player's lost, reset all relevant data and return to main menu
    public void GameOver()
    {
        levelEnd = true;
        Time.timeScale = 0;
        Debug.Log("GAME OVER");
        GameManager.manager.saveData.hp = 10;
        GameManager.manager.saveData.levelInfo = 1;
        GameManager.manager.saveData.score = 0;
        GameManager.manager.tempData.CopyData(GameManager.manager.saveData);
        SceneManager.LoadScene(1);
    }

    //Reward player with points and increase hiscore if score is bigger
    public void GetPoints(int points)
    {
        GameManager.manager.tempData.score += points;
        score.text = GameManager.manager.tempData.score.ToString();
        if (GameManager.manager.tempData.score > GameManager.manager.tempData.hiscore)
        {
            GameManager.manager.tempData.hiscore = GameManager.manager.tempData.score;
            hiScore.text = GameManager.manager.tempData.hiscore.ToString();
        }
    }

    //End the current level, revert back to data at the start of the level and save it in a file
    //Then load main menu
    public void SaveAndExit()
    {
        levelEnd = true;
        Time.timeScale = 0;
        GameManager.manager.tempData.CopyData(GameManager.manager.saveData);
        //NEED TO SAVE PLAYERDATA TO GAMEMANAGER AND SAVE FILE
        GameManager.manager.Save();
        SceneManager.LoadScene(1);
    }

    //Pause time and make pausePanel visible
    public void Pause()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }
}
