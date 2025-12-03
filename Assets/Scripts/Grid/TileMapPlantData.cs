using UnityEngine;

public class TileMapPlantData
{
    #region Variables
    public PlantType plant;
    public bool isWater;
    public int growthDays;
    public int dryDays;
    private bool isDead;
    #endregion

    #region Constructor
    public TileMapPlantData(PlantType plant, bool isWater)
    {
        this.plant = plant;
        this.isWater = isWater;
        growthDays = 0;
        dryDays = 0;
        isDead = false;
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
        if(isWater == true)
        {
            growthDays+=1;
            dryDays = 0;
            RemoveWater();
        } else
        {
            dryDays+=1;
            if(dryDays > plant.maxDaysWithoutWater)
            {
                Die();
            }
        }

        if(plant.season != Calendar_Controller.Instance.season)
        {
            Die();
        }
    }

    #endregion

    #region Die
    private void Die()
    {
        isDead = true;
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
