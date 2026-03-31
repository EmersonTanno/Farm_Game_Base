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
    }
    public TileMapPlantData(PlantType plant, bool isWater, bool isPlown, int growthDays, int dryDays, bool isDead)
    {
        this.plant = plant;
        this.isWater = isWater;
        this.isPlown = isPlown;
        this.growthDays = growthDays;
        this.dryDays = dryDays;
        this.isDead = isDead;
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

            if (plant.season != Calendar_Controller.Instance.season)
            {
                Die();
            }
        }
        else
        {
            if (isPlown && (isWater || willRain))
            {
                Debug.Log("Choveu");
                isWater = false;
                return;
            }
            else if (isPlown)
            {
                if (Random.value < 0.5f)
                {
                    Debug.Log("Resetou");
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

    #region
    public TileBase GetStageTile()
    {
        int stageIndex = -1;

        if (growthDays >= plant.growthTimeInDays)
        {
            stageIndex = 4;
        }
        else if (!isWater && growthDays < plant.growthTimeInDays / 2)
        {
            stageIndex = 0;
        }
        else if (isWater && growthDays < plant.growthTimeInDays / 2)
        {
            stageIndex = 1;
        }
        else if (!isWater && growthDays >= plant.growthTimeInDays / 2)
        {
            stageIndex = 2;
        }
        else if (isWater && growthDays >= plant.growthTimeInDays / 2)
        {
            stageIndex = 3;
        }
        else
        {
            stageIndex = 5;
        }
        
        if(stageIndex != -1)
            return plant.plantStages[stageIndex];

        return null;
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
    #endregion

    #region Reset
    public void ResetTile()
    {
        plant = null;
        isWater = false;
        growthDays = 0;
        dryDays = 0;
        isDead = false;
    }
    #endregion
}
