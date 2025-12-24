using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class Shop_Manager : MonoBehaviour
{
    public static Shop_Manager Instance { get; private set; }

    #region Variables
    [Header("UI")]
    [SerializeField] GameObject shopCanvas;
    [SerializeField] TextMeshProUGUI totalPriceText;
    [SerializeField] Transform slotContainer;
    [SerializeField] GameObject slotPrefab;

    [Header("Itens por Estação")]
    [SerializeField] Item[] itemVerao;
    [SerializeField] Item[] itemOutono;
    [SerializeField] Item[] itemPrimavera;
    [SerializeField] Item[] itemInverno;

    private List<ShopSlot> activeSlots = new List<ShopSlot>();
    public bool shopActive = false;
    public int totalPrice = 0;
    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetShopItens();
    }

    void OnEnable()
    {
        Tax_System.OnTaxChange += SetShopItens;
        Calendar_Controller.OnMonthChange += SetShopItens;
        ShopSlot.OnAddRemoveItem += ReloadTotalPrice;
    }

    void OnDisable()
    {
        Tax_System.OnTaxChange -= SetShopItens;
        Calendar_Controller.OnMonthChange -= SetShopItens;
        ShopSlot.OnAddRemoveItem -= ReloadTotalPrice;
    }
    #endregion

    #region Open / Close Shop
    public void ActivateDeactivateShop()
    {
        if (Time_Controll.Instance.bedActive ||
            Player_Controller.Instance.CheckPlayerActions() ||
            InventoryManager.Instance.inventoryActive)
            return;

        shopActive = !shopActive;

        if (shopActive)
            Time_Controll.Instance.PauseTime();
        else
            Time_Controll.Instance.UnpauseTime();

        shopCanvas.SetActive(shopActive);

        ReloadTotalPrice();
    }
    #endregion

    #region Creating Slots / Setting Ttens
    private void ClearSlots()
    {
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        activeSlots.Clear();
    }

    private void CreateSlots(Item[] items)
    {
        ClearSlots();

        for (int i = 0; i < items.Length; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotContainer);
            ShopSlot slot = obj.GetComponent<ShopSlot>();
            slot.SetItem(items[i]);
            activeSlots.Add(slot);
        }
    }

    private void SetShopItens()
    {
        switch (Calendar_Controller.Instance.season)
        {
            case Season.Verao:
                CreateSlots(itemVerao);
                break;
            case Season.Outono:
                CreateSlots(itemOutono);
                break;
            case Season.Primavera:
                CreateSlots(itemPrimavera);
                break;
            case Season.Inverno:
                CreateSlots(itemInverno);
                break;
        }
    }
    #endregion

    #region Buy
    public void BuyItems()
    {
        Status_Controller.Instance.RemoveGold(totalPrice);

        foreach (var slot in activeSlots)
        {
            if (slot.GetSellItem() == null) continue;

            for (int i = 0; i < slot.GetQuantity(); i++)
                InventoryManager.Instance.AddItem(slot.GetSellItem());
        }

        ResetQuantity();
        ActivateDeactivateShop();
    }
    #endregion

    #region Total Controll
    private void ReloadTotalPrice()
    {
        totalPrice = 0;

        foreach (var slot in activeSlots)
        {
            if (slot.GetSellItem() == null) continue;
            totalPrice += slot.GetBuyValue() * slot.GetQuantity();
        }

        totalPriceText.text = $"${totalPrice}";
    }

    private void ResetQuantity()
    {
        for(int i = 0; i < activeSlots.Count; i++)
        {
            ShopSlot slot = activeSlots[i].GetComponent<ShopSlot>();
            slot.Reset();
        }
        ReloadTotalPrice();
    }
    #endregion
}
