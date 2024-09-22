using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    private int snowPileBlasted = 0;
    private int enemySlaughtered = 0;
    private int fruitPicked = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    

    public void AddSnowPileBlasted(int amount)
    {
        snowPileBlasted += amount;
    }

    public void AddEnemySlaughtered(int amount)
    {
        enemySlaughtered += amount;
    }

    public void AddFruitPicked(int amount)
    {
        fruitPicked += amount;
    }
    

    public int SnowPileBlasted => snowPileBlasted;

    public int EnemySlaughtered => enemySlaughtered;
    
    public int FruitPicked => fruitPicked;
}
