using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public Item[] itemsToPick;

    public void PickupItem(int id)
    {
        bool result = inventoryManager.AddItem(itemsToPick[id]);
        if (result)
        {
            Debug.Log("Item added");
        }
        else
        {
            Debug.Log("ITEM NOT ADDED");
        }
    }

    public void GetSelectedItem()
    {
        Item receivedItem = inventoryManager.UseSelectedItem();
        if (receivedItem)
        {
            Debug.Log(receivedItem.itemName);
        }
    }
}
