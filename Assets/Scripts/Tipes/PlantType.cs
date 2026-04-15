using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewPlant", menuName = "Farming/Plant")]
public class PlantType : ScriptableObject
{
    public int id;
    public string plantName;
    public int growthTimeInDays;
    public int maxDaysWithoutWater;
    public Season season;
    public bool multipleHarvests = false;
    public int growthTimeAfterFirstHarvest = -1;
    public Item harvest;
    public int harvestMin;
    public int harvestMax;
    public Sprite[] growthStages;
    public TileBase[] plantStages;
}