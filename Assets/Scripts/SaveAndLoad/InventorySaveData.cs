using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items = new();
}


[System.Serializable]
public class ItemSaveData
{
    public int itemId;
    public int quantity;
    public int slot;
}
