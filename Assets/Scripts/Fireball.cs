using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float explosionDelay = 2f; // Time before explosion
    public int explosionRange = 2; // Explosion range (cross pattern)
    private float timer = 0f;
    private bool isExploding = false;

    private MapManager mapManager; // Reference to the map manager
    private Vector2Int fireballPosition; // Position on the grid

    void Start()
    {
        mapManager = FindObjectOfType<MapManager>(); // Find the map manager in the scene
        fireballPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= explosionDelay && !isExploding)
        {
            Explode();
        }
    }

    private void Explode()
    {
        isExploding = true;

        // Cross pattern explosion
        HandleExplosionAt(fireballPosition);
        HandleExplosionAt(fireballPosition + Vector2Int.right);
        HandleExplosionAt(fireballPosition + Vector2Int.left);
        HandleExplosionAt(fireballPosition + Vector2Int.up);
        HandleExplosionAt(fireballPosition + Vector2Int.down);

        // After explosion, destroy the fireball object
        Destroy(gameObject);
    }

    private void HandleExplosionAt(Vector2Int position)
    {
        // Check if the tile is within bounds and if it can be destroyed
        if (mapManager.IsInBounds(position))
        {
            if (mapManager.IsDestructible(position))
            {
                mapManager.ReplaceWithGround(position.x, position.y); // Destroy the tile (replace with ground)
            }
            // todo Handle damage to the player or enemies
        }
    }
    
}
