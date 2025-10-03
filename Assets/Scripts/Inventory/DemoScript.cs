using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScript : MonoBehaviour
{
    //public InventoryManager inventoryManager;
    public Item[] itemsToPick;

    public void PickupItem(int id)
    {
        bool result = InventoryManager.Instance.AddItem(itemsToPick[id]);
    }

    public void GetSelectedItem()
    {
        Item receivedItem = InventoryManager.Instance.UseSelectedItem();
    }
}
