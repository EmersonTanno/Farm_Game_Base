public static class WorldTileFactory
{
    public static WorldTileData CreateDefault(WorldTile tile)
    {
        return new WorldTileData
        {
            baseTileId = tile.id,
            isWalkable = tile.isWalkable,
            canBePlanted = tile.canBePlanted,
            isPath = tile.isPath,
            npcId = "",
        };
    }
}
