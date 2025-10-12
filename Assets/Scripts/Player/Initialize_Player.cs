using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_Player : MonoBehaviour
{
    public Item item1;
    public Item item2;
    public Item item3;
    public Item item4;
    public Item item5;

    void Start()
    {
        InventoryManager.Instance.AddItem(item1);
        InventoryManager.Instance.AddItem(item2);
        InventoryManager.Instance.AddItem(item3);
        InventoryManager.Instance.AddItem(item3);
        InventoryManager.Instance.AddItem(item3);
        InventoryManager.Instance.AddItem(item4);
        InventoryManager.Instance.AddItem(item4);
        InventoryManager.Instance.AddItem(item4);
        InventoryManager.Instance.AddItem(item5);
        InventoryManager.Instance.AddItem(item5);
        InventoryManager.Instance.AddItem(item5);
    }
}
