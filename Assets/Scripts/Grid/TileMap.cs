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
    */
    #endregion

    #region Variables
    private Grid<int> originalGrid;
    private Grid<TileMapPlantData> plantGrid;
    private Grid<int> objectGrid;

    #endregion

    #region Defining Grids
    public TileMap(int width, int height, float cellSize, Vector3 originPosition)
    {
        originalGrid = new Grid<int>(width, height, cellSize, originPosition);
        plantGrid = new Grid<TileMapPlantData>(width, height, cellSize, originPosition);
        objectGrid = new Grid<int>(width, height, cellSize, originPosition);
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

    public Grid<int> GetObjectGrid()
    {
        return objectGrid;
    }
    #endregion

}
