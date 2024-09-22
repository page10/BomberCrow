using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    
    private int _snowPileBlasted = 0;
    private int _enemySlaughtered = 0;
    private int _fruitPicked = 0;
    

    public void AddSnowPileBlasted(int amount)
    {
        _snowPileBlasted += amount;
    }

    public void AddEnemySlaughtered(int amount)
    {
        _enemySlaughtered += amount;
    }

    public void AddFruitPicked(int amount)
    {
        _fruitPicked += amount;
    }

    public int SnowPileBlasted => _snowPileBlasted;

    public int EnemySlaughtered => _enemySlaughtered;
    
    public int FruitPicked => _fruitPicked;
}
