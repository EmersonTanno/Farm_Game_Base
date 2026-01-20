using UnityEngine;

public struct WorldTileData
{
    public int baseTileId;
    public bool isWalkable;
    public bool canBePlanted;
    public bool isPath;
    public int npcId;
    public WarpTile warp;

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

    public WorldTileData WithNPCId(int value)
    {
        WorldTileData copy = this;
        copy.npcId = value;
        return copy;
    }

    public WorldTileData WithWarp(WarpTile value)
    {
        WorldTileData copy = this;
        copy.warp = value;
        return copy;
    }
}


