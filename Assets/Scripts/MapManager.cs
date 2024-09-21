using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {
    public GameObject snowPilePrefab;
    public GameObject rockPrefab;
    public GameObject treePrefab;
    public GameObject groundPrefab;  // New ground prefab

    public int width;
    public int height;

    private Tile[,] map;

    public void GenerateMap() {
        map = new Tile[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                TileType type = GetRandomTileType();
                map[x, y] = new Tile(type);

                // Instantiate the correct prefab based on the tile type
                GameObject tileGO = InstantiateTile(type, x, y);
                tileGO.transform.SetParent(transform);
            }
        }
    }

    // Method to instantiate the right tile prefab
    GameObject InstantiateTile(TileType type, int x, int y) {
        switch (type) {
            case TileType.SnowPile:
                return Instantiate(snowPilePrefab, new Vector3(x, y, 0), Quaternion.identity);
            case TileType.Rock:
                return Instantiate(rockPrefab, new Vector3(x, y, 0), Quaternion.identity);
            case TileType.Tree:
                return Instantiate(treePrefab, new Vector3(x, y, 0), Quaternion.identity);
            case TileType.Ground:
                return Instantiate(groundPrefab, new Vector3(x, y, 0), Quaternion.identity);
            default:
                return null;
        }
    }

    // Call this function when snow pile or tree is destroyed
    public void ReplaceWithGround(int x, int y) {
        if (map[x, y].Type == TileType.SnowPile || map[x, y].Type == TileType.Tree) {
            // Change the tile type to ground
            map[x, y].Type = TileType.Ground;

            // Destroy the old tile GameObject and instantiate ground
            Destroy(map[x, y].gameObject);  // Assuming you track the tile's GameObject
            GameObject groundTile = InstantiateTile(TileType.Ground, x, y);
            groundTile.transform.SetParent(transform);
        }
    }
    
    private TileType GetRandomTileType() {
        int rand = Random.Range(0, 100);

        // Adjust percentages to include ground tiles
        if (rand < 40) {
            return TileType.SnowPile;
        } else if (rand < 45) {
            return TileType.Tree;
        } else if (rand < 65) {
            return TileType.Rock;
        } else {
            return TileType.Ground;  // Now we have some ground tiles at the start
        }
    }

}

