using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Farming/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType type;
    public Sprite image;
    public bool stackable = true;
    public int maxStack;
    public ActionType action;
    public bool consume;
    public PlantType plant;
    public bool sell;
    public int buyValue;
    public int sellValue;
}

public enum ItemType
{
    Seed,
    Tool,
    Harvest
}

public enum ActionType
{
    Plant, //Plantar
    Water, //Regar
    Plowing, //Arar
    Consume, //Consumir
}
