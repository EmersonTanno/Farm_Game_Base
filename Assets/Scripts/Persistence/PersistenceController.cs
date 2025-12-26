using UnityEngine;

public class PersistenceController : MonoBehaviour
{
    public static PersistenceController Instance;
    private GridSaveData gridSaveData = new GridSaveData();
    public bool hasData = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneController.OnWarpStart += SaveFarm;
    }

    void OnDisable()
    {
        SceneController.OnWarpStart -= SaveFarm;
    }

    public void SaveFarm()
    {
        if(SceneInfo.Instance.location != SceneLocationEnum.FARM) return;

        gridSaveData.plants.Clear();
        hasData = true;

        Grid<int> originalGrid = TileMapController.Instance.GetGrid().GetOriginalGrid();
        Grid<TileMapPlantData> plantGrid = TileMapController.Instance.GetGrid().GetPlantGrid();

        for(int y = 0; y < originalGrid.GetHeight(); y++)
        {
            for(int x = 0; x < originalGrid.GetWidth(); x++)
            {
                int gridValue = originalGrid.GetGridObject(x, y);
                if(gridValue == 10 || gridValue == 11 || gridValue == 20)
                {
                    if(gridValue == 20)
                    {
                        var plant = plantGrid.GetGridObject(x, y);

                        gridSaveData.plants.Add(new PlantSaveData
                        {
                            x = x,
                            y = y,
                            gridValue = gridValue,
                            plantData = plant
                        });
                        continue;
                    }

                    gridSaveData.plants.Add(new PlantSaveData
                    {
                        x = x,
                        y = y,
                        gridValue = gridValue,
                    });

                }
            }
        }
    }

    public GridSaveData LoadGridSaveData()
    {
        return gridSaveData;
    }

}
