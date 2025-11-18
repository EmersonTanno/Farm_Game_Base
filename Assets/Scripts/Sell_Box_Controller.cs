using System.Collections.Generic;
using UnityEngine;

public class Sell_Box_Controller : MonoBehaviour
{
    public static Sell_Box_Controller Instance { get; private set; }

    #region Variables
    private List<Item> sellItemsList = new List<Item>();
    #endregion

    #region Events
    void OnEnable()
    {
        Time_Controll.OnMidNightChange += SellItems;
    }

    void OnDisable()
    {
        Time_Controll.OnMidNightChange -= SellItems;
    }
    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
    }
    #endregion

    #region Sell Box Functions
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
        int tax = 0;
        for (int i = 0; i < sellItemsList.Count; i++)
        {
            valor += sellItemsList[i].sellValue;
        }
        sellItemsList.Clear();

        Tax_System.Instance.AddSellItemsValueToAnualSells(valor);
        Debug.Log($"valor: {valor}");
        tax = Tax_System.Instance.ApplySellTaxes(valor);

        valor -= tax; 
        Debug.Log($"tax: {tax}");
        Debug.Log($"valor final: {valor}");

        Status_Controller.Instance.AddGold(valor);
    }
    #endregion
}
