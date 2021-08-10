using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject firstChoice;
    public GameObject secondChoice;

    GameObject garbage;
    GameObject player;
    public GameObject enemyContainer;
    public int enemyCountMax;
    private int enemyCount = 0;
    int start;

    int level;

    public GameObject[] buttons;
    public GameObject[] blocks;
    public GameObject[] pieces;
    public GameObject[] enemies;

    public GameObject winPanel;
    public Text winMoney;
    public Text winBM;
    public GameObject losePanel;

    public GameState _state;

    // Start is called before the first frame update
    void Start()
    {
        garbage = GameObject.Find("Garbage");
        player = GameObject.Find("Player");
        StartSetup();
        _state = GameState.startup;
        start = 0;
        if (!PlayerPrefs.HasKey("LEVEL"))
        {
            PlayerPrefs.SetInt("LEVEL", 1);
        }
        level = PlayerPrefs.GetInt("LEVEL");

        //Set enemy count for level (very simple execution just for testing)
        enemyCountMax = (int) Mathf.Round(level / 2);
    }

    // Update is called once per frame
    void Update()
    {
        switch (_state)
        {
            case GameState.startup:
                StartSetup();
                start++;
                if (start > 20)
                {
                    CreateEnemies();
                    _state = GameState.chooseBlocks;
                }
                break;
            case GameState.chooseBlocks:
                if (firstChoice != null && secondChoice != null)
                {
                    int temp = secondChoice.GetComponent<BlockManager>().colour;

                    secondChoice.transform.GetChild(1).transform.SetParent(firstChoice.transform);
                    secondChoice.GetComponent<BlockManager>().colour = firstChoice.GetComponent<BlockManager>().colour;
                    firstChoice.transform.GetChild(1).transform.SetParent(secondChoice.transform);
                    firstChoice.GetComponent<BlockManager>().colour = temp;

                    firstChoice = null;
                    secondChoice = null;

                    _state = GameState.moveMatch;
                }

                // Check if Lost
                if (player.GetComponent<PlayerManager>().health <= 0)
                {
                    _state = GameState.lose;
                    losePanel.transform.localPosition = Vector3.zero;
                }               
                break;
            case GameState.moveMatch:
                if (PiecesMoving())
                {
                    DisableButtons();
                }
                else
                {
                    EnableButtons();
                    if (AnyMatch())
                    {
                        MoveMatches();
                    }
                    else
                    {
                        _state = GameState.enemyMove;
                    }
                }
                break;
            case GameState.enemyMove:
                EnemyAttack();
                //Check if Win
                if (enemies.Length == 0)
                {
                    if (enemyCount < enemyCountMax - 1)
                    {
                        enemyCount++;
                        CreateEnemies();
                    }
                    else
                    {
                        _state = GameState.win;
                        if (PlayFabController.PFC.level == level)
                        {
                            PlayFabController.PFC.StartCloudUpdatePlayerStats(level + 1);
                        }
                        //Give reward
                        int money = 0;
                        int buildMat = 0;
                        for (int i = 0; i < level; i++)
                        {
                            PlayFabController.PFC.GrantItem();
                            money += 100;
                            buildMat += 20;
                        }
                        winMoney.text = "Money: " + money.ToString();
                        winBM.text = "Building Material: " + buildMat.ToString();
                        winPanel.transform.localPosition = Vector3.zero;
                        PlayFabController.PFC.StartCloudGetPlayerStats();
                    }
                }
                else { _state = GameState.chooseBlocks; }
                break;
            case GameState.lose:
                break;
            case GameState.win:
                break;
        }
    }

    public void EnableButtons()
    {
        buttons = GameObject.FindGameObjectsWithTag("button");

        foreach (GameObject button in buttons)
        {
            button.GetComponent<Button>().interactable = true;
        }
    }

    public void DisableButtons()
    {
        buttons = GameObject.FindGameObjectsWithTag("button");

        foreach (GameObject button in buttons)
        {
            button.GetComponent<Button>().interactable = false;
        }
    }

    public bool AnyMatch()
    {
        blocks = GameObject.FindGameObjectsWithTag("block");

        bool anyMatch = false;
        foreach (GameObject block in blocks)
        {
            if (block.GetComponent<BlockManager>().matched)
            {
                anyMatch = true;
            }
        }

        return anyMatch;
    }

    public void MoveMatches()
    {
        blocks = GameObject.FindGameObjectsWithTag("block");

        foreach (GameObject block in blocks)
        {
            if (block.GetComponent<BlockManager>().matched)
            {
                block.transform.GetChild(1).transform.SetParent(garbage.transform);
            }
        }
    }

    public void StartSetup()
    {
        blocks = GameObject.FindGameObjectsWithTag("block");

        foreach (GameObject block in blocks)
        {
            if (block.GetComponent<BlockManager>().matched)
            {
                GameObject.Destroy(block.transform.GetChild(1).gameObject);
                block.GetComponent<BlockManager>().CreatePiece();
            }
        }
    }

    public bool PiecesMoving()
    {
        pieces = GameObject.FindGameObjectsWithTag("piece");

        List<bool> anyMoving = new List<bool>();
        foreach (GameObject piece in pieces)
        {
            anyMoving.Add(piece.GetComponent<PieceManager>().moving);
        }
        bool moving = anyMoving.Contains(true);
        return moving;
            
    }

    public void EnemyAttack()
    {
        enemies = GameObject.FindGameObjectsWithTag("enemy");

        if (enemies.Length > 0)
        {
            GameObject enemyAttacking = enemies[Random.Range(0, enemies.Length)];
            GameObject bullet = (GameObject)Resources.Load("enemy/bullet", typeof(GameObject));
            GameObject bulletInstance = Instantiate(bullet) as GameObject;
            bulletInstance.transform.SetParent(enemyAttacking.transform);
            bulletInstance.transform.localPosition = Vector3.zero;
        }
    }

    public void CreateEnemies()
    {
        GameObject enemyContainerInstance = Instantiate(enemyContainer) as GameObject;
        //enemyContainerInstance.transform.position = new Vector3(0, 20, 0);

        // Instantiate enemy in container
        GameObject enemy0 = (GameObject)Resources.Load("enemy/0", typeof(GameObject));
        GameObject enemy0Instance = Instantiate(enemy0) as GameObject;
        enemy0Instance.transform.SetParent(enemyContainerInstance.transform.GetChild(0).transform);
        enemy0Instance.transform.localPosition = Vector3.zero;

        GameObject enemy1 = (GameObject)Resources.Load("enemy/1", typeof(GameObject));
        GameObject enemy1Instance = Instantiate(enemy1) as GameObject;
        enemy1Instance.transform.SetParent(enemyContainerInstance.transform.GetChild(1).transform);
        enemy1Instance.transform.localPosition = Vector3.zero;

        GameObject enemy2 = (GameObject)Resources.Load("enemy/1", typeof(GameObject));
        GameObject enemy2Instance = Instantiate(enemy2) as GameObject;
        enemy2Instance.transform.SetParent(enemyContainerInstance.transform.GetChild(2).transform);
        enemy2Instance.transform.localPosition = Vector3.zero;
    }

    public void ReturnToMenu()
    {
        PlayFabController.PFC.StartCloudGetPlayerStats();
        SceneManager.LoadSceneAsync("MenuScene");
    }
}
