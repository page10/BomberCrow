using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile: MonoBehaviour 
{
    public TileType Type;
    public bool CanPass;
    public bool CanHurtByFire;
    public bool HasFood = false;
    public bool HasItem = false;
    public ItemType? ItemType = null;

    public Tile(TileType type) {
        Type = type;
        // Same logic as before to determine properties
        switch (type) {
            case TileType.SnowPile:
                CanPass = true;
                CanHurtByFire = true;
                break;
            case TileType.Rock:
                CanPass = true;
                CanHurtByFire = false;
                break;
            case TileType.Tree:
                CanPass = true;
                CanHurtByFire = true;
                break;
            case TileType.Ground:
                CanPass = true;
                CanHurtByFire = false;
                break;
        }
    }
}

public enum TileType {
    SnowPile,   // Blocks movement, can be melted
    Rock,       // Blocks movement, immune to fire
    Tree,        // Blocks movement, can be destroyed
    Ground 
}

public enum ItemType {
    PineOil,
    PineNeedles,
    Chili,
    Match,
    Strawberry
}
