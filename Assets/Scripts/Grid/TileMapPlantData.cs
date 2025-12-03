using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TileMapPlantData
{
    public PlantType plant;
    public bool isWater;
    public int growthDays;
    public int dryDays;
    private bool isDead;

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
        return $"{plant} | {isWater} | {growthDays} | {dryDays}";
    }
}
