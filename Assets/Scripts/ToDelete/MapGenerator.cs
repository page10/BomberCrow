// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class MapGenerator : MonoBehaviour
// {
//     static int mapHeight = 10;
//     static int mapWidth = 14;
//     
//     Tile[,] logicMap = new Tile[mapWidth, mapHeight];
//     
//     void GenerateMap(int width, int height) {
//         // Generate all tiles first
//         for (int x = 0; x < width; x++) {
//             for (int y = 0; y < height; y++) {
//                 // Randomly assign a tile type (excluding food and item tiles)
//                 TileType type = GetRandomTileType();
//                 map[x, y] = new Tile(type);
//             }
//         }
//
//         // Place food under a snow pile
//         PlaceFood();
//
//         // Place all the items
//         PlaceItems();
//     }
//
//     void PlaceFood() {
//         // Find a random snow pile to hide the food
//         bool foodPlaced = false;
//         while (!foodPlaced) {
//             int x = Random.Range(0, width);
//             int y = Random.Range(0, height);
//
//             if (map[x, y].Type == TileType.SnowPile) {
//                 map[x, y].HasFood = true;  // Add a property to indicate food presence
//                 foodPlaced = true;
//             }
//         }
//     }
//
//     void PlaceItems() {
//         // List of items to place
//         List<ItemType> items = new List<ItemType> {
//             ItemType.PineOil, ItemType.PineNeedles, ItemType.Chili, ItemType.Match, ItemType.Strawberry
//         };
//
//         foreach (var item in items) {
//             bool itemPlaced = false;
//             while (!itemPlaced) {
//                 int x = Random.Range(0, width);
//                 int y = Random.Range(0, height);
//
//                 // Only place items on non-rock, non-food tiles
//                 if (map[x, y].Type != TileType.Rock && !map[x, y].HasFood && !map[x, y].HasItem) {
//                     map[x, y].HasItem = true;  // Add a property to indicate item presence
//                     map[x, y].ItemType = item;  // Store which item is on this tile
//                     itemPlaced = true;
//                 }
//             }
//         }
//     }
//
//
//     TileType GetRandomTileType() {
//         int rand = Random.Range(0, 100);
//         if (rand < 50) return TileType.SnowPile;
//         else if (rand < 80) return TileType.Tree;
//         else return TileType.Rock;
//     }
//     
//     public void HandleFireballImpact(int x, int y) {
//         Tile impactedTile = logicMap[x, y];
//         if (impactedTile.CanHurtByFire) {
//             DestroyTile(x, y);
//         }
//     }
//
//     void DestroyTile(int x, int y) {
//         // Replace the tile with an empty space or other tile type
//         logicMap[x, y] = null;  // Empty tile
//     }
//
//
// }
