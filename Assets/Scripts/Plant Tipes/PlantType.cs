using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlant", menuName = "Farming/Plant")]
public class PlantType : ScriptableObject
{
    public string plantName;
    public int growthTimeInDays;
    public int maxDaysWWithoutWater;
    public string season;
    public Harvest_Type harvest;
    public int harvestMin;
    public int harvestMax;
    public Sprite[] growthStages;
}
