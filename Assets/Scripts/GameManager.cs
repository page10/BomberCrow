using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapManager mapManager;  // Assign this in the Unity Inspector
    public Camera mainCamera;      // Assign the main camera in the Unity Inspector
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject fireBallPrefab;
    public GameObject explosionCenterPrefab;
    public GameObject explosionLinePrefab;  
    public GameObject explosionEndPrefab;
    
    private Crow _character;
    private Vector2 CrowGridPos => _character ? mapManager.PositionInGrid(_character.transform.position) : Vector2.zero;
    
    public float explosionDelay = 2f;   // Delay before fireball explodes
    public int maxFireballs = 1;        // Max fireballs that can be placed
    //private int currentFireballs = 0;   // Currently placed fireballs

    // List of fireballs. You may detonate all bombs by controller while PowerUp has been taken.
    private List<Fireball> _currentFireBalls = new List<Fireball>();

    void Start() {
        // Generate the map
        mapManager.GenerateMap(out Vector2Int playerStartPosition);
        PlacePlayerCharacter(playerStartPosition);

        // Center the camera
        CenterCamera();
    }

    void CenterCamera() {
        // Calculate the center point of the map
        float mapCenterX = mapManager.width / 2f - 0.5f;
        float mapCenterY = mapManager.height / 2f - 0.5f;

        // Set the camera's position to the center of the map
        mainCamera.transform.position = new Vector3(mapCenterX, mapCenterY, mainCamera.transform.position.z);
    }
    
    private void PlacePlayerCharacter(Vector2Int position) {
        GameObject go = Instantiate(playerPrefab, new Vector3(position.x, position.y, -1), Quaternion.identity);
        _character = go.GetComponent<Crow>();
        _character.TryMove(MoveDirection.Down, 0);
        _character.transform.SetParent(transform);
    }

    private void Update()
    {
        HandleInput();
    }
    
    private void HandleInput()
    {
        float delta = Time.deltaTime;
        if (Input.GetKey(KeyCode.W)) {  // Move up
            MovePlayer(MoveDirection.Up, delta);
        } else if (Input.GetKey(KeyCode.S)) {  // Move down
            MovePlayer(MoveDirection.Down, delta);
        } else if (Input.GetKey(KeyCode.A)) {  // Move left
            MovePlayer(MoveDirection.Left, delta);
        } else if (Input.GetKey(KeyCode.D)) {  // Move right
            MovePlayer(MoveDirection.Right, delta);
        }
        
        // Fireball placement input
        if (Input.GetKeyDown(KeyCode.Space) && _currentFireBalls.Count < maxFireballs)
        {
            PlaceFireball();
        }
    }
    
    private void PlaceFireball() {
        //Vector2 characterPosition = _character.transform.position; // Get crow's position
        //todo incorrect position.
        // Check if the tile is passable and if we can place the fireball
        if (mapManager.IsMoveValid(CrowGridPos))
        {
            Vector2 ballPos = mapManager.CenterOfPosition(CrowGridPos); //炸弹放在乌鸦所在单元格中心
            GameObject go = Instantiate(fireBallPrefab, new Vector3(ballPos.x, ballPos.y, -1), Quaternion.identity);
            Fireball fb = go.GetComponent<Fireball>();
            fb.Set(FireballExploded);
            Debug.Log("fireball placed at " + ballPos);
            _currentFireBalls.Add(fb);
            //currentFireballs++; // Increment the active fireball count
        }
        // todo 0921
    }
    
    // Call this when a fireball explodes and is removed
    public void FireballExploded(Fireball bomb)
    {
        _currentFireBalls.Remove(bomb);
        // Check if the tile is within bounds and if it can be destroyed
        int bombRange = bomb.explosionRange;
        Vector2Int[] dir = new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down};
        
        List<Vector2Int> toBeGround = new List<Vector2Int> { bomb.GridPos };
        foreach (Vector2Int d in dir)
        {
            int r = 1;
            while (r <= bombRange)
            {
                Vector2Int grid = bomb.GridPos + d * r;
                bool beObstucled = false;
                if (mapManager.IsInBounds(grid))
                {
                    if (mapManager.IsDestructible(grid))
                    {
                        toBeGround.Add(grid);
                        beObstucled = true;
                    }
                    // todo Handle damage to the player or enemies
                    // Check if the crow is within the explosion range
                    Vector2Int crowPosition = new Vector2Int(Mathf.RoundToInt(_character.transform.position.x), Mathf.RoundToInt(_character.transform.position.y));
                    if (crowPosition == grid)
                    {
                        EndGame();
                    }
                }
                else beObstucled = true;

                if (beObstucled) break;
                r++;
            }
        }
        
        // Instantiate the explosion center prefab at the bomb's position
        Instantiate(explosionCenterPrefab, bomb.transform.position + new Vector3(0, 0, -1), Quaternion.identity);

        // Instantiate the explosion line and end prefabs in each direction
        Vector2Int[] directions = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        foreach (Vector2Int direction in directions)
        {
            for (int i = 1; i <= bomb.explosionRange; i++)
            {
                Vector2Int gridPos = bomb.GridPos + direction * i;
                Vector3 worldPos = new Vector3(gridPos.x, gridPos.y, -1);

                // Check if the grid position is within the map bounds
                if (!mapManager.IsInBounds(gridPos))
                {
                    // The grid position is outside the map bounds, skip this iteration
                    continue;
                }         
    
                // Check if the tile is a rock
                if (mapManager.GetTileType(gridPos) == TileType.Rock)
                {
                    // Stop the explosion from spreading further in this direction
                    break;
                }
                
                if (i == bomb.explosionRange)
                {
                    // Instantiate the explosion end prefab at the end of the explosion
                    GameObject explosionEnd = Instantiate(explosionEndPrefab, worldPos, Quaternion.identity);

                    // Rotate the explosion end based on its direction
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    explosionEnd.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180));
                }
                else
                {
                    // Instantiate the explosion line prefab along the explosion
                    GameObject explosionLine = Instantiate(explosionLinePrefab, worldPos, Quaternion.identity);

                    // Rotate the explosion line based on its direction
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    explosionLine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + 180));
                }
            }
        }
        
        // Check if the crow is within the explosion range
        
        foreach (Vector2Int g in toBeGround)
        {
            mapManager.ReplaceWithGround(g.x, g.y); // Destroy the tile (replace with ground)
        }
        
        
        //remove from list
        _currentFireBalls.Remove(bomb);
    }
    
    private void EndGame()
    {
        // game over logic here
        Debug.Log("Game Over");
    }
    
    private void MovePlayer(MoveDirection dir, float delta) {
        if (!_character || dir == MoveDirection.None) return;

        const float squeezeRate = 0.1f;
        float checkOffsetX = squeezeRate * MapManager.TileSize.x * 0.5f;
        float checkOffsetY = squeezeRate * MapManager.TileSize.y * 0.5f;
        float bodyX = _character.bodySize.x * 0.5f;
        float bodyY = _character.bodySize.y * 0.5f;
        Vector3 dest = _character.TryMove(dir, delta);
        //根据方向获得具体的要检查的点，如果2个点都可过，则移动生效，这里的squeezeRate是一个挤过去的倍率，是为了手感
        Vector2[] checkPoints = new Vector2[] { Vector2.zero ,Vector2.zero};
        switch (dir)
        {
            case MoveDirection.Up:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.up * bodyY + Vector3.left * checkOffsetX,
                    dest + Vector3.up * bodyY + Vector3.right * checkOffsetX
                };
                dest = new Vector3(mapManager.CenterOfPosition(dest).x, dest.y, dest.z);
                break;
            case MoveDirection.Down:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.down * bodyY + Vector3.left * checkOffsetX,
                    dest + Vector3.down * bodyY + Vector3.right * checkOffsetX
                };
                dest = new Vector3(mapManager.CenterOfPosition(dest).x, dest.y, dest.z);
                break;
            case MoveDirection.Left:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.left * bodyX + Vector3.up * checkOffsetY,
                    dest + Vector3.left * bodyX + Vector3.down * checkOffsetY
                };
                dest = new Vector3(dest.x, mapManager.CenterOfPosition(dest).y, dest.z);
                break;
            case MoveDirection.Right:
                checkPoints = new Vector2[]
                {
                    dest + Vector3.right * bodyX + Vector3.up * checkOffsetY,
                    dest + Vector3.right * bodyX + Vector3.down * checkOffsetY
                };
                dest = new Vector3(dest.x, mapManager.CenterOfPosition(dest).y, dest.z);
                break;
        }

        bool canMove = true;
        foreach (Vector2 point in checkPoints)
        {
            if (!mapManager.IsMoveValid(point))
            {
                canMove = false;
                break;
            }
        }

        if (canMove)
        {
            _character.transform.position = dest;
            if (mapManager.CheckWinCondition(dest))
            {
                Debug.Log("Player picked up the food! Game won.");
                ScenesManager.Instance.LoadWinning();
            }
        }

        

        // Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(_character.transform.position.x), Mathf.RoundToInt(_character.transform.position.y));
        // Vector2Int newPos = currentPos + new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));

        // if (mapManager.IsMoveValid(newPos)) {  // Check if move is valid
        //     _character.transform.position = new Vector3(newPos.x, newPos.y, -1);  
        // }
        //
        // // Check if the player has won
        // mapManager.CheckWinCondition(newPos.x, newPos.y);
    }
}
