using UnityEngine;

public class TileMap
{
    #region States
    /*
    originalGrid:
        1 - Terra normal 1
        2 - Terra normal 2
        10 - Terra arada
        11 - Terra arada regada
        20 - Terra plantada

    objectGrid:
        1 - Cama
        2 - Caixa de Venda
    
    constructionGrid:
        1- Casa
    */
    #endregion

    #region Variables
    private Grid<int> originalGrid;
    private Grid<TileMapPlantData> plantGrid;
    private Grid<WorldObjectID> objectGrid;
    private Grid<ConstructionsType> constructionGrid;
    private Grid<bool> movementGrid;
    private Grid<WarpTile> warpGrid;
    private Grid<int> npcGrid;
    private Grid<bool> pathGrid;

    #endregion

    #region Defining Grids
    public TileMap(int width, int height, float cellSize, Vector3 originPosition)
    {
        originalGrid = new Grid<int>(width, height, cellSize, originPosition);
        plantGrid = new Grid<TileMapPlantData>(width, height, cellSize, originPosition);
        objectGrid = new Grid<WorldObjectID>(width, height, cellSize, originPosition);
        movementGrid = new Grid<bool>(width, height, cellSize, originPosition);
        constructionGrid = new Grid<ConstructionsType>(width, height, cellSize, originPosition);
        warpGrid = new Grid<WarpTile>(width, height, cellSize, originPosition);
        npcGrid = new Grid<int>(width, height, cellSize, originPosition);
        pathGrid = new Grid<bool>(width, height, cellSize, originPosition);
    }
    #endregion

    #region Get
    public Grid<int> GetOriginalGrid()
    {
        return originalGrid;
    }

    public Grid<TileMapPlantData> GetPlantGrid()
    {
        return plantGrid;
    }

    public Grid<WorldObjectID> GetObjectGrid()
    {
        return objectGrid;
    }

    public Grid<ConstructionsType> GetConstructionGrid()
    {
        return constructionGrid;
    }

    public Grid<bool> GetMovementGrid()
    {
        return movementGrid;
    }

    public Grid<WarpTile> GetWarpGrid()
    {
        return warpGrid;
    }

    public Grid<int> GetNpcGrid()
    {
        return npcGrid;
    }

    public Grid<bool> GetPathGrid()
    {
        return pathGrid;
    }
    #endregion

}
