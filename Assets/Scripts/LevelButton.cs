using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    int level;

    // Start is called before the first frame update
    void Start()
    {
        level = transform.GetSiblingIndex() + 1;

        Text buttonText = gameObject.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        buttonText.text = "Level " + level;

        gameObject.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { EnterGameScene(level); });
    }

    void EnterGameScene(int level)
    {
        PlayerPrefs.SetInt("LEVEL", level);
        PlayerPrefs.SetInt("HEALTH", GameObject.Find("Menu").GetComponent<MenuManager>().baseHealth);
        PlayerPrefs.SetInt("ATTACK", GameObject.Find("Menu").GetComponent<MenuManager>().baseAttack);

        SceneManager.LoadSceneAsync("GameScene");
    }
}
