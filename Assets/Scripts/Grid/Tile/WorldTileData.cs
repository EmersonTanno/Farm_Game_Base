using UnityEngine;

public struct WorldTileData
{
    public int baseTileId;
    public bool isWalkable;
    public bool canBePlanted;
    public bool isPath;
    public int npcId;

    public WorldTileData WithBaseTileId(int value)
    {
        WorldTileData copy = this;
        copy.baseTileId = value;
        return copy;
    }

    public WorldTileData WithIsWalkable(bool value)
    {
        WorldTileData copy = this;
        copy.isWalkable = value;
        return copy;
    }

    public WorldTileData WithIsPath(bool value)
    {
        WorldTileData copy = this;
        copy.isPath = value;
        return copy;
    }
}


