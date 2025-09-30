using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlant", menuName = "Farming/Plant")]
public class PlantType : ScriptableObject
{
    public string plantName;
    public int growthTimeInDays;
    public string season;
    public Sprite[] growthStages;
}
