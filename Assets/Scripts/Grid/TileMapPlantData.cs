using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapPlantData
{
    #region Variables
    public PlantType plant;
    public bool isPlown;
    public bool isWater;
    public int growthDays;
    public int dryDays;
    public bool isDead;
    public bool hasBeenHarvested = false;
    #endregion

    #region Constructor
    public TileMapPlantData(PlantType plant, bool isWater, bool isPlown)
    {
        this.plant = plant;
        this.isWater = isWater;
        this.isPlown = isPlown;
        growthDays = 0;
        dryDays = 0;
        isDead = false;
        hasBeenHarvested = false;
    }
    public TileMapPlantData(PlantType plant, bool isWater, bool isPlown, int growthDays, int dryDays, bool isDead, bool hasBeenHarvested)
    {
        this.plant = plant;
        this.isWater = isWater;
        this.isPlown = isPlown;
        this.growthDays = growthDays;
        this.dryDays = dryDays;
        this.isDead = isDead;
        this.hasBeenHarvested = hasBeenHarvested;
    }

    public override string ToString()
    {
        return $"Plant {plant} | Water: {isWater} | Grow: {growthDays} | Dry: {dryDays} | Dead: {isDead}";
    }
    #endregion

    #region Water
    public void PutWater()
    {
        isWater = true;
    }

    public void RemoveWater()
    {
        isWater = false;
    }
    #endregion

    #region Day Controll
    public void PassDay()
    {
        bool willRain = WeatherController.Instance.WillRain();
        if (plant != null)
        {
            if (plant.season != Calendar_Controller.Instance.season)
            {
                Die();
                return;
            }

            if (isWater || willRain)
            {
                growthDays += 1;
                dryDays = 0;

                if (isWater)
                    RemoveWater();

                return;
            }
            else
            {
                dryDays += 1;
                if (dryDays > plant.maxDaysWithoutWater)
                {
                    Die();
                }
            }            
        }
        else
        {
            if (isPlown && (isWater || willRain))
            {
                isWater = false;
                return;
            }
            else if (isPlown)
            {
                if (Random.value < 0.5f)
                {
                    isPlown = false;
                }
            }
        }
    }
    #endregion

    #region Die
    private void Die()
    {
        isDead = true;
    }
    #endregion

    #region Get Tile
    public TileBase GetStageTile() //ainda em testes
    {
        if (plant == null) return null;

        int stageIndex;

        // primeiro crescimento
        if (!hasBeenHarvested)
        {
            if (growthDays >= plant.growthTimeInDays)
            {
                stageIndex = 4;
            }
            else if (growthDays < plant.growthTimeInDays / 2)
            {
                stageIndex = isWater ? 1 : 0;
            }
            else
            {
                stageIndex = isWater ? 3 : 2;
            }
        }
        // regrowth (multi harvest)
        else if (plant.multipleHarvests)
        {
            int regrowDays = growthDays - plant.growthTimeInDays;

            if (regrowDays >= plant.growthTimeAfterFirstHarvest)
            {
                stageIndex = 4;
            }
            else
            {
                stageIndex = isWater ? 3 : 2;
            }
        }
        else
        {
            // fallback de segurança
            stageIndex = 4;
        }

        return plant.plantStages[stageIndex];
    }
    #endregion

    #region Plant
    public void SetPlant(PlantType plant, bool water)
    {
        this.plant = plant;
        isWater = water;
    }
    #endregion

    #region Harvest
    public bool CanHarvest()
    {
        if(plant == null) return false;

        if(growthDays >= plant.growthTimeInDays)
        {
            return true;
        }

        return false;
    }

    public void HarvestTile()
    {
        hasBeenHarvested = true;

        if(plant.multipleHarvests)
        {
            hasBeenHarvested = true;
            growthDays = plant.growthTimeInDays;
        }
        else
        {
            ResetTile();
        }
    }
    #endregion

    #region Reset
    public void ResetTile()
    {
        plant = null;
        isWater = false;
        growthDays = 0;
        dryDays = 0;
        isDead = false;
        hasBeenHarvested = false;
    }
    #endregion
}
