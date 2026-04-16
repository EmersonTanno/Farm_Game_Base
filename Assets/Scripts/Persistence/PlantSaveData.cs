[System.Serializable]
public class PlantSaveData
{
    [System.NonSerialized]
    public TileMapPlantData plantData;

    public int x;
    public int y;
    public int gridValue;

    public int plantId;
    public bool isPlown;
    public bool isWater;
    public int growthDays;
    public int dryDays;
    public bool isDead;
    public bool hasBeenHarvested;
}
