using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Sell_Controller : MonoBehaviour
{
    public static Sell_Controller Instance { get; private set; }

    #region Variables
    [SerializeField] GameObject sellUi;
    public bool active = false;
    [SerializeField] TextMeshProUGUI totalText;
    [SerializeField] TextMeshProUGUI taxText;
    [SerializeField] TextMeshProUGUI gainText;
    [SerializeField] GameObject sellContentContainer;
    [SerializeField] GameObject sellContentSlot;

    private Dictionary<Item, int> sellControlDict = new Dictionary<Item, int>();

    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
    }
    #endregion

    public void SellItems(List<Item> sellItemsList)
    {
        int gainedValue = 0;
        int taxedValue = 0;
        int totalValue = 0;

        sellControlDict.Clear();

        foreach (var item in sellItemsList)
        {
            totalValue += item.sellValue;

            int tax = Tax_System.Instance.ApplySellTaxes(item.sellValue);
            taxedValue += tax;
            gainedValue += item.sellValue - tax;

            AddItem(item);
        }

        ActivateDeactivateUi();
        
        RefreshSellContentUI();

        gainText.text = $"Recebido: ${gainedValue}";
        taxText.text = $"Taxas: -${taxedValue}";
        totalText.text = $"Total: ${totalValue}";

        Tax_System.Instance.AddSellItemsValueToAnualSells(totalValue);
        Status_Controller.Instance.AddGold(gainedValue);
    }


    private void RefreshSellContentUI()
    {
        foreach (Transform child in sellContentContainer.transform)
            Destroy(child.gameObject);

        if (sellControlDict.Count == 0) return;

        foreach (var kvp in sellControlDict)
        {
            Item item = kvp.Key;
            int quantity = kvp.Value;

            GameObject newSlot = Instantiate(
                sellContentSlot,
                sellContentContainer.transform
            );

            Sell_Content_Slot slot = newSlot.GetComponent<Sell_Content_Slot>();
            slot.SetInfo(item, quantity);
        }
    }


    public void ActivateDeactivateUi()
    {
        if(active == true)
        {
            active = false;
            Time_Controll.Instance.UnpauseTime();
        } else
        {
            active = true;
        }
        
        sellUi.SetActive(active);
    }

    public void AddItem(Item newItem)
    {
        if (!newItem) return;

        if (sellControlDict.ContainsKey(newItem))
            sellControlDict[newItem]++;
        else
            sellControlDict[newItem] = 1;
    }

    public void RemoveItem(Item item)
    {
        if (!sellControlDict.ContainsKey(item)) return;

        sellControlDict[item]--;

        if (sellControlDict[item] <= 0)
            sellControlDict.Remove(item);
    }
}
