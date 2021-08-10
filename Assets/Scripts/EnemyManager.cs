using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public int attack;
    public int health;
    public int type;
    public Slider healthSlider;

    int level;

    
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("LEVEL"))
        {
            PlayerPrefs.SetInt("LEVEL", 1);
        }
        level = PlayerPrefs.GetInt("LEVEL");

        //Set enemy power for level (very simple execution just for testing)
        health = 100 + level * 20;
        attack = 10 + level * 5;

        healthSlider.maxValue = health;
        healthSlider.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        healthSlider.value = health;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
