using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item: MonoBehaviour
{
    public ItemType Type;
    public Vector2Int GridPos { get; set; }

    public void Use(Character character)
    {
        switch (Type)
        {
            case ItemType.PineOil:
                character.BombRange += 1;
                break;
            case ItemType.Shoes:
                character.MoveSpeed += 1;
                break;
            case ItemType.BombAdder:
                character.BombCount += 1;
                break;
        }
    }
}

public enum ItemType {
    PineOil,
    Shoes,
    BombAdder
}

// public class PineOil : Item
// {
//     public override void Use(Character character)
//     {
//         character.BombRange += 1; // Increase the character's bomb range
//     }
// }
//
// public class Shoes : Item
// {
//     public override void Use(Character character)
//     {
//         character.MoveSpeed += 1; // Increase the character's speed
//     }
// }
//
// public class BombAdder : Item
// {
//     public override void Use(Character character)
//     {
//         character.BombCount += 1; // Increase the character's bomb count
//     }
// }
