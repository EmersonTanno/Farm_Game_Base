using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHarvest", menuName = "Farming/Harvest")]
public class Harvest_Type : ScriptableObject
{
    public string harvestName;
    public int value;
    public Sprite sprite;
    public Item harvestItem;
}
