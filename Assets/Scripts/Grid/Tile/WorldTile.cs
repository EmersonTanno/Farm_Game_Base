using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/World Tile")]
public class WorldTile : Tile
{
    public int id;
    public bool isWarp;
    public bool IsWalkable;
    public WarpTile warp;
}