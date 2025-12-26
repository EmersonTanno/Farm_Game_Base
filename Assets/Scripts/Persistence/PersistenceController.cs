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
        hasData = true;

        gridSaveData.originalGrid = TileMapController.Instance.GetGrid().GetOriginalGrid();
        gridSaveData.plantGrid = TileMapController.Instance.GetGrid().GetPlantGrid();
    }

    public GridSaveData LoadGridSaveData()
    {
        return gridSaveData;
    }

}
