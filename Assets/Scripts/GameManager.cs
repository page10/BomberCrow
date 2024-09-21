using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapManager mapManager;  // Assign this in the Unity Inspector
    public Camera mainCamera;      // Assign the main camera in the Unity Inspector
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject fireBallPrefab;
    
    private GameObject character;
    
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
        character = Instantiate(playerPrefab, new Vector3(position.x, position.y, -1), Quaternion.identity);
        character.transform.SetParent(transform);

    }

    private void Update()
    {
        HandleInput();
    }
    
    private void HandleInput() {
        if (Input.GetKeyDown(KeyCode.W)) {  // Move up
            MovePlayer(Vector2.up);
        } else if (Input.GetKeyDown(KeyCode.S)) {  // Move down
            MovePlayer(Vector2.down);
        } else if (Input.GetKeyDown(KeyCode.A)) {  // Move left
            MovePlayer(Vector2.left);
        } else if (Input.GetKeyDown(KeyCode.D)) {  // Move right
            MovePlayer(Vector2.right);
        }
        
        // Fireball placement input
        if (Input.GetKeyDown(KeyCode.Space) && _currentFireBalls.Count < maxFireballs)
        {
            PlaceFireball();
        }
    }
    
    private void PlaceFireball() {
        Vector2 characterPosition = character.transform.position; // Get crow's position
        //todo incorrect position.
        // Check if the tile is passable and if we can place the fireball
        if (mapManager.IsMoveValid(new Vector2Int(Mathf.RoundToInt(characterPosition.x), Mathf.RoundToInt(characterPosition.y))))
        {
            GameObject go = Instantiate(fireBallPrefab, characterPosition, Quaternion.identity);
            Fireball fb = go.GetComponent<Fireball>();
            fb.Set(FireballExploded);
            Debug.Log("fireball placed at " + characterPosition);
            //currentFireballs++; // Increment the active fireball count
        }
        // todo 0921
    }
    
    // Call this when a fireball explodes and is removed
    public void FireballExploded(Fireball bomb)
    {
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
                }
                else beObstucled = true;

                if (beObstucled) break;
                r++;
            }
        }
        
        foreach (Vector2Int g in toBeGround)
        {
            mapManager.ReplaceWithGround(g.x, g.y); // Destroy the tile (replace with ground)
        }
        
        
        //remove from list
        _currentFireBalls.Remove(bomb);
    }
    
    private void MovePlayer(Vector2 direction) {
        Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(character.transform.position.x), Mathf.RoundToInt(character.transform.position.y));
        Vector2Int newPos = currentPos + new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));

        if (mapManager.IsMoveValid(newPos)) {  // Check if move is valid
            character.transform.position = new Vector3(newPos.x, newPos.y, -1);  
        }
    }
}
