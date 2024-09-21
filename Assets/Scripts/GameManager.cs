using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapManager mapManager;  // Assign this in the Unity Inspector
    public Camera mainCamera;      // Assign the main camera in the Unity Inspector
    [SerializeField] private GameObject playerPrefab;
    private GameObject playerCharacter;

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
        playerCharacter = Instantiate(playerPrefab, new Vector3(position.x, position.y, -1), Quaternion.identity);
        playerCharacter.transform.SetParent(transform);

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
    }
    
    private void MovePlayer(Vector2 direction) {
        Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(playerCharacter.transform.position.x), Mathf.RoundToInt(playerCharacter.transform.position.y));
        Vector2Int newPos = currentPos + new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));

        if (mapManager.IsMoveValid(newPos)) {  // Check if move is valid
            playerCharacter.transform.position = new Vector3(newPos.x, newPos.y, -1);  // Move the player
        }
    }
}
