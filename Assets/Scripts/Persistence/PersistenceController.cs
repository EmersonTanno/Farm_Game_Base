using System;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceController : MonoBehaviour
{
    public static PersistenceController Instance;
    private GridSaveData gridSaveData = new GridSaveData();
    public bool hasData = false;

    public static event Action OnDayChangeFinish;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        SceneController.OnWarpStart += SaveFarm;
        Calendar_Controller.OnDayChange += PassDay;
        WeatherController.OnRainFall += WaterSoilWithRain;
    }

    void OnDisable()
    {
        SceneController.OnWarpStart -= SaveFarm;
        Calendar_Controller.OnDayChange -= PassDay;
        WeatherController.OnRainFall -= WaterSoilWithRain;
    }

    public void SaveFarm()
    {
        if(SceneInfo.Instance.location != SceneLocationEnum.FARM) return;

        gridSaveData.plants.Clear();

        Grid<WorldTileData> originalGrid = TileMapController.Instance.GetGrid().GetGrid();
        Grid<TileMapPlantData> plantGrid = TileMapController.Instance.GetGrid().GetPlantGrid();

        for(int y = 0; y < originalGrid.GetHeight(); y++)
        {
            for(int x = 0; x < originalGrid.GetWidth(); x++)
            {
                int gridValue = originalGrid.GetGridObject(x, y).baseTileId;
                TileMapPlantData plantData = plantGrid.GetGridObject(x, y);
                if(plantData != null)
                {
                    gridSaveData.plants.Add(new PlantSaveData
                    {
                        x = x,
                        y = y,
                        gridValue = gridValue,
                        

                        plantId = plantData.plant != null ? plantData.plant.id : -1,
                        isPlown = plantData.isPlown,
                        isWater = plantData.isWater,
                        growthDays = plantData.plant != null ? plantData.growthDays : 0,
                        dryDays = plantData.plant != null ? plantData.dryDays : 0,
                        isDead = plantData.isDead,

                        plantData = plantData,
                    });
                    continue;
                }
            }
        }

        hasData = gridSaveData.plants.Count > 0;
    }

    public void WaterSoilWithRain()
    {
        foreach (PlantSaveData plant in gridSaveData.plants)
        {
            if (plant.plantData != null)
            {
                plant.plantData.PutWater();
            }
        }
    }

    public GridSaveData LoadGridSaveData()
    {
        return gridSaveData;
    }

    public void PassDay()
    {
        foreach (PlantSaveData plant in gridSaveData.plants)
        {
            if (plant.plantData != null)
            {
                plant.plantData.PassDay();
            }
        }

        OnDayChangeFinish?.Invoke();
    }

    public void Save(ref FarmSaveData data)
    {
        data.plants = gridSaveData.plants;

        List<PlantSaveData> plantsToSave = new List<PlantSaveData>();

        foreach(PlantSaveData plant in gridSaveData.plants)
        {
            PlantSaveData newPlantData = new PlantSaveData();
            newPlantData.gridValue = plant.gridValue;
            newPlantData.growthDays = plant.plantData.growthDays;
            newPlantData.isDead = plant.plantData.isDead;
            newPlantData.dryDays = plant.plantData.dryDays;
            newPlantData.isPlown = plant.plantData.isPlown;
            newPlantData.isWater = plant.plantData.isWater;
            newPlantData.hasBeenHarvested = plant.plantData.hasBeenHarvested;
            newPlantData.plantData = plant.plantData;
            newPlantData.plantId = plant.plantId;
            newPlantData.x = plant.x;
            newPlantData.y = plant.y;

            plantsToSave.Add(newPlantData);
        }

        data.plants = plantsToSave;
    }

    public void Load(FarmSaveData data)
    {
        gridSaveData.plants = data.plants;
        foreach(PlantSaveData plant in gridSaveData.plants)
        {
            TileMapPlantData plantData;
            if(plant.plantId == -1)
            {
                plantData = new TileMapPlantData(null, plant.isWater, plant.isPlown, plant.growthDays, plant.dryDays, plant.isDead, plant.hasBeenHarvested);
            }
            else
            {
                plantData = new TileMapPlantData(PlantsDataBaseController.Instance.GetPlantType(plant.plantId), plant.isWater, plant.isPlown, plant.growthDays, plant.dryDays, plant.isDead, plant.hasBeenHarvested);
            }
            plant.plantData = plantData;
        }
        hasData = gridSaveData.plants.Count > 0;
    }
}
