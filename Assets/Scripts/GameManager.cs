using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapManager mapManager;  // Assign this in the Unity Inspector
    public Camera mainCamera;      // Assign the main camera in the Unity Inspector
    [SerializeField] private GameObject playerPrefab;

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
        // Assuming you have a player character prefab
        GameObject player = Instantiate(playerPrefab, new Vector3(position.x, position.y, -1), Quaternion.identity);
        player.transform.SetParent(transform);
    }
    
}
