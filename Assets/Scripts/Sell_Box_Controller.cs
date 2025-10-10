using System.Collections.Generic;
using UnityEngine;

public class Sell_Box_Controller : MonoBehaviour
{

    public static Sell_Box_Controller Instance { get; private set; }
    private List<Item> sellItemsList = new List<Item>();

    void OnEnable()
    {
        Time_Controll.OnMidNightChange += SellItems;
    }

    void OnDisable()
    {
        Time_Controll.OnMidNightChange -= SellItems;
    }

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(Item newItem)
    {
        if (!newItem) return;
        sellItemsList.Add(newItem);
    }
    public void RemoveItem(Item itemToRemove)
    {
        sellItemsList.Remove(itemToRemove);
    }

    public void RemoveItemAt(int index)
    {
        if (index >= 0 && index < sellItemsList.Count)
            sellItemsList.RemoveAt(index);
    }
    
    public void SellItems()
    {
        int valor = 0;
        for (int i = 0; i < sellItemsList.Count; i++)
        {
            valor += sellItemsList[i].sellValue;
        }
        sellItemsList.Clear();
        Debug.Log(valor);
    }
}
