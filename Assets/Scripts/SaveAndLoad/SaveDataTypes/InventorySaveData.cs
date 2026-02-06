using System.Collections.Generic;

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
