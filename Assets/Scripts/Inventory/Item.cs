using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Farming/Item")]
public class Item : ScriptableObject
{
    [Header("Gameplay")]
    public Sprite image;
    public ItemType type;
    public int maxStack;

    [Header("UI")]
    public bool stackable = true;

    [Header("Both")]
    public string itemName;
    public ActionType action;
}

public enum ItemType {
    Seed,
    Tool
}

public enum ActionType {
    Plant, //Plantar
    Water, //Regar
    Plowing //Arar
}
