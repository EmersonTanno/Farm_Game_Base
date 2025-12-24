using System.Collections.Generic;
using UnityEngine;

public class Sell_Box_Controller : MonoBehaviour
{
    public static Sell_Box_Controller Instance { get; private set; }

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
        Sell_Controller.Instance.AddItemToList(newItem);
    }
    public void RemoveItem(Item itemToRemove)
    {
        if (!itemToRemove) return;
        Sell_Controller.Instance.RemoveItemFromList(itemToRemove);
    }

    #endregion
}