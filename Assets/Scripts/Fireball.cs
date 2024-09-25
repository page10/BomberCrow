using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.Serialization;

public class Fireball : MonoBehaviour
{
    public float explosionDelay = 2f; // Time before explosion
    //public int explosionRange = 2; // Explosion range (cross pattern)
    private float timer = 0f;
    private bool isExploding = false;

    /// <summary>
    /// What do i do while explosive is call some method from GameManager.
    /// </summary>
    private Action<Fireball> _onExplosive = null;

    //don't create any reference of Manager in SomeObject.
    // private MapManager mapManager; // Reference to the map manager
    public Vector2Int GridPos { get; private set; } // Position on the grid

    void Start()
    {
        //mapManager = FindObjectOfType<MapManager>(); // Find the map manager in the scene
        GridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= explosionDelay && !isExploding)
        {
            Explode();
        }
    }

    /// <summary>
    /// Init fireball's gameplay data here.
    /// </summary>
    /// <param name="onExplosive"></param>
    public void Set(Action<Fireball> onExplosive)
    {
        _onExplosive = onExplosive;
    }

    private void Explode()
    {
        isExploding = true;

        // Cross pattern explosion
        _onExplosive?.Invoke(this);

        // After explosion, destroy the fireball object
        Destroy(gameObject);
    }

    // Dependence is wrong. Map and Characters are GameManager's buddies.
    // private void HandleExplosionAt(Vector2Int position)
    // {
    //     // Check if the tile is within bounds and if it can be destroyed
    //     if (mapManager.IsInBounds(position))
    //     {
    //         if (mapManager.IsDestructible(position))
    //         {
    //             mapManager.ReplaceWithGround(position.x, position.y); // Destroy the tile (replace with ground)
    //         }
    //         
    //     }
    // }
    
}
