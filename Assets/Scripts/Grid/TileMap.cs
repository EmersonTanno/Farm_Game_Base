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
    private Grid<WorldTileData> grid;
    private Grid<TileMapPlantData> plantGrid;

    #endregion

    #region Defining Grids
    public TileMap(int width, int height, float cellSize, Vector3 originPosition)
    {
        grid = new Grid<WorldTileData>(width, height, cellSize, originPosition);
        plantGrid = new Grid<TileMapPlantData>(width, height, cellSize, originPosition);
    }
    #endregion

    #region Get
    public Grid<WorldTileData> GetGrid()
    {
        return grid;
    }

    public Grid<TileMapPlantData> GetPlantGrid()
    {
        return plantGrid;
    }
    #endregion

}
