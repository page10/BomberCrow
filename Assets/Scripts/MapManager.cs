using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject snowPilePrefab;
    public GameObject rockPrefab;
    public GameObject treePrefab;
    public GameObject groundPrefab; // New ground prefab
    public GameObject foodPrefab; // Pine cone prefab

    public static readonly Vector2 TileSize = Vector2.one;
    
    private Vector2Int foodPosition; // Stores the position of the hidden food
    //private Vector2Int playerStartPosition;

    public int width;
    public int height;

    private Tile[,] map;

    public void GenerateMap(out Vector2Int playerStartPosition)
    {
        map = new Tile[width, height];
        List<Vector2Int> snowPilePositions = new List<Vector2Int>(); // Track snow pile positions

        // First, create an L-shaped ground area at the starting point (top-left corner, for example)
        CreateLShapedGround(0, 0); // Starting position at top-left corner (0, 0)
        
        // Set the player's start position at the corner of the L-shaped ground
        playerStartPosition = new Vector2Int(0, 0);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Skip the L-shaped ground area
                if (IsInLShapedGround(x, y))
                {
                    continue;
                }

                TileType type;

                // Place rock tiles where both x and y are odd numbers
                if (x % 2 != 0 && y % 2 != 0)
                {
                    type = TileType.Rock;
                }
                else
                {
                    type = GetRandomTileType(); // Choose a random tile type (snow pile, tree, or ground)
                }

                //map[x, y] = new Tile(type);
                GameObject tileGO = InstantiateTile(type, x, y);
                tileGO.transform.SetParent(transform);
                map[x, y] = tileGO.GetComponent<Tile>(); //Get from GameObject, NOT create new

                // Track snow pile positions
                if (type == TileType.SnowPile)
                {
                    snowPilePositions.Add(new Vector2Int(x, y));
                }
            }
        }

        // Randomly select one snow pile to hide the food
        if (snowPilePositions.Count > 0)
        {
            foodPosition = snowPilePositions[Random.Range(0, snowPilePositions.Count)];
            Debug.Log("Food hidden at: " + foodPosition);
        }
    }
    
    // Instantiate the player character at the start position
    

// Creates an L-shaped ground area at the starting position
    private void CreateLShapedGround(int startX, int startY)
    {
        // Example L-shape: (3x3 ground tiles, with one corner missing)
        SetTileType(startX, startY, TileType.Ground);
        SetTileType(startX + 1, startY, TileType.Ground);
        SetTileType(startX + 2, startY, TileType.Ground);
        SetTileType(startX, startY + 1, TileType.Ground);
        SetTileType(startX, startY + 2, TileType.Ground);
    }

// Helper method to check if a tile is within the L-shaped ground area
    private bool IsInLShapedGround(int x, int y)
    {
        return (x == 0 && y <= 2) || (x <= 2 && y == 0); // L-shape occupies these positions
    }

    private void SetTileType(int x, int y, TileType type)
    {
        //map[x, y] = new Tile(type);
        GameObject tileGO = InstantiateTile(type, x, y);
        tileGO.transform.SetParent(transform);
        map[x, y] = tileGO.GetComponent<Tile>(); //get from gameobject, not create new.
    }


    // Method to instantiate the right tile prefab
    GameObject InstantiateTile(TileType type, int x, int y)
    {
        switch (type)
        {
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
    
    public bool IsInBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < map.GetLength(0) && position.y >= 0 && position.y < map.GetLength(1);
    }

    public bool IsDestructible(Vector2Int position)
    {
        // Only snow piles and trees are destructible
        TileType type = map[position.x, position.y].Type;
        return type == TileType.SnowPile || type == TileType.Tree;
    }

    // Call this function when snow pile or tree is destroyed
    public void ReplaceWithGround(int x, int y)
    {
        if (map[x, y].CanHurtByFire)
        {
            // Change the tile type to ground
            map[x, y].Type = TileType.Ground;

            // Destroy the old tile GameObject and instantiate ground
            Destroy(map[x, y].gameObject); // Assuming you track the tile's GameObject
            GameObject groundTile = InstantiateTile(TileType.Ground, x, y);
            groundTile.transform.SetParent(transform);
            map[x, y] = groundTile.GetComponent<Tile>(); //DON'T forget set new one to array.
            RevealFood(x, y); // Check if the food is revealed
        }
    }

    private TileType GetRandomTileType()
    {
        int rand = Random.Range(0, 100);

        // Adjust percentages to include ground tiles
        if (rand < 60)
        {
            return TileType.SnowPile;
        }
        else if (rand < 65)
        {
            return TileType.Tree;
        }
        else
        {
            return TileType.Ground; // Now we have some ground tiles at the start
        }
    }

    // Call this method when a snow pile is destroyed
    public void RevealFood(int x, int y)
    {
        if (x == foodPosition.x && y == foodPosition.y)
        {
            // Instantiate food when the correct snow pile is destroyed
            Instantiate(foodPrefab, new Vector3(x, y, 0), Quaternion.identity);
        }
    }

    // Check if the player collects the food
    public void CheckWinCondition(int x, int y)
    {
        if (x == foodPosition.x && y == foodPosition.y)
        {
            //todo 依赖关系，这里不应该直接结束游戏的，反正这个目前也不用了，因为像素级移动这个判断是不对的
            Debug.Log("Player picked up the food! Game won.");
            ScenesManager.Instance.LoadWinning();
        }
    }

    /// <summary>
    /// 胜利判断条件
    /// </summary>
    /// <param name="crowPos">乌鸦位置</param>
    /// <param name="radius">乌鸦和食物距离多少内算赢</param>
    /// <returns></returns>
    public bool CheckWinCondition(Vector2 crowPos, float radius = 0.1f) =>
        Mathf.Abs(Vector2.Distance(crowPos, foodPosition)) <= radius;
    
    public Vector2Int PositionInGrid(Vector2 pos) => PositionInGrid(pos.x, pos.y);
    public Vector2Int PositionInGrid(float posX, float posY)=>new Vector2Int(Mathf.RoundToInt(posX / TileSize.x), Mathf.RoundToInt(posY / TileSize.y));

    public Vector2 CenterOfPosition(Vector2 pos) => CenterOfPosition(pos.x, pos.y);
    public Vector2 CenterOfPosition(float posX, float posY)=>new Vector2(Mathf.RoundToInt(posX / TileSize.x) * TileSize.x, Mathf.RoundToInt(posY / TileSize.y) * TileSize.y);

    public bool IsMoveValid(Vector2 pos) => IsMoveValid(PositionInGrid(pos));
    public bool IsMoveValid(float x, float y) => IsMoveValid(PositionInGrid(x, y));
    
    public bool IsMoveValid(Vector2Int position) {
        // Check if position is within bounds
        if (position.x < 0 || position.x >= map.GetLength(0) || position.y < 0 || position.y >= map.GetLength(1)) {
            return false;  // Out of bounds
        }

        Tile currentTile = map[position.x, position.y];

        // Check if the tile is passable
        return currentTile.CanPass;
    }
    
    public TileType GetTileType(Vector2Int position) {
        return map[position.x, position.y].Type;
    }
}