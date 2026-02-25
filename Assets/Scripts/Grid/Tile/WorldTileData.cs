public struct WorldTileData
{
    public int baseTileId;
    public bool isWalkable;
    public bool canBePlanted;
    public bool isPath;
    public int npcId;
    public WarpTile warp;
    public WorldObjectID objectID;
    public ConstructionsType constructionID;
    public ShopObject shopObject;

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

    public WorldTileData WithObjectId(WorldObjectID value)
    {
        WorldTileData copy = this;
        copy.objectID = value;
        return copy;
    }

    public WorldTileData WithConstructionId(ConstructionsType value)
    {
        WorldTileData copy = this;
        copy.constructionID = value;
        return copy;
    }

    public WorldTileData WithShopObject(ShopObject value)
    {
        WorldTileData copy = this;
        copy.shopObject = value;
        return copy;
    }
}


