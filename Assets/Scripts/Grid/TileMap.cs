using UnityEngine;

public class TileMap
{
    #region States
    /*
    originalGrid:
        0 - Terra normal 1
        1 - Terra normal 2
        10 - Terra arada
        11 - Terra arada regada
        20 - Terra plantada
    */
    #endregion

    #region Variables
    private Grid<int> originalGrid;
    private Grid<TileMapPlantData> plantGrid;

    #endregion

    #region Defining Grids
    public TileMap(int width, int height, float cellSize, Vector3 originPosition)
    {
        originalGrid = new Grid<int>(width, height, cellSize, originPosition);
        plantGrid = new Grid<TileMapPlantData>(width, height, cellSize, originPosition);
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
    #endregion

}
