using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tiles/Tiles Data Base")]
public class WorldTileDataBase : ScriptableObject
{
    public List<WorldTile> tiles = new();

    public WorldTile GetTile(int id)
    {
        return tiles.Find(t => t.id == id);
    }
}
