using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Item Data Base")]
public class ItemDataBase : ScriptableObject
{
    public List<Item> items = new();

    public Item GetItem(int id)
    {
        return items.Find(t => t.id == id);
    }
}
