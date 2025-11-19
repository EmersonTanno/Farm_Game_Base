using TMPro;
using UnityEngine;

public class Shop_Manager : MonoBehaviour
{
    #region Variables
    public static Shop_Manager Instance { get; private set; }
    public bool shopActive = false;
    [SerializeField] GameObject shopCanvas;
    [SerializeField] TextMeshProUGUI totalPriceText;

    [SerializeField] Item[] itemVerao;
    [SerializeField] Item[] itemOutono;
    [SerializeField] Item[] itemPrimavera;
    [SerializeField] Item[] itemInverno;

    [SerializeField] GameObject[] itemSlosts;

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
        Calendar_Controller.OnMonthChange += SetShopItens;
        ShopSlot.OnAddRemoveItem += ReloadTotalPrice;
    }

    void OnDisable()
    {
        Calendar_Controller.OnMonthChange -= SetShopItens;
        ShopSlot.OnAddRemoveItem -= ReloadTotalPrice;
    }
    #endregion

    #region Canvas
    public void ActivateDeactivateShop()
    {
        if (Time_Controll.Instance.bedActive || Player_Controller.Instance.CheckPlayerActions() || InventoryManager.Instance.inventoryActive) return;

        shopActive = !shopActive;

        if (shopActive)
        {
            Time_Controll.Instance.PauseTime();
        }
        else
        {
            Time_Controll.Instance.UnpauseTime();
        }
        shopCanvas.SetActive(shopActive);
        ActivateSlots(shopActive);
        ReloadTotalPrice();
    }

    private void ActivateSlots(bool status)
    {
        if (status)
        {
            for (int i = 0; i < itemSlosts.Length; i++)
            {
                ShopSlot shopSlot = itemSlosts[i].GetComponent<ShopSlot>();
                if (shopSlot.GetSellItem())
                {
                    itemSlosts[i].SetActive(true);
                    shopSlot.Reset();
                }
                else
                {
                    itemSlosts[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < itemSlosts.Length; i++)
            {
                itemSlosts[i].SetActive(false);
            }
        }
    }

    private void SetShopItens()
    {
        switch (Calendar_Controller.Instance.season)
        {
            case Season.Verao:
                SetSeasonShop(itemVerao);
                break;
            case Season.Outono:
                SetSeasonShop(itemOutono);
                break;
            case Season.Primavera:
                SetSeasonShop(itemPrimavera);
                break;
            case Season.Inverno:
                SetSeasonShop(itemInverno);
                break;
        }
    }

    private void SetSeasonShop(Item[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            ShopSlot shopSlot = itemSlosts[i].GetComponent<ShopSlot>();
            shopSlot.SetItem(items[i]);
        }
    }
    #endregion

    #region Shop
    public void BuyItems()
    {
        Status_Controller.Instance.RemoveGold(totalPrice);

        for (int i = 0; i < itemSlosts.Length; i++)
        {
            GameObject obj = itemSlosts[i];
            if (obj == null) continue;

            ShopSlot slot = obj.GetComponent<ShopSlot>();
            if (slot == null) continue;

            Item item = slot.GetSellItem();
            if (item == null) continue;

            if (slot.GetQuantity() > 0)
            {
                for (int x = 0; x < slot.GetQuantity(); x++)
                {
                    InventoryManager.Instance.AddItem(slot.GetSellItem());
                }
            }
        }

        ActivateDeactivateShop();
    }
    
    private void ReloadTotalPrice()
    {
        totalPrice = 0;
        for (int i = 0; i < itemSlosts.Length; i++)
        {
            GameObject obj = itemSlosts[i];
            if (obj == null) continue;

            ShopSlot slot = obj.GetComponent<ShopSlot>();
            if (slot == null) continue;

            Item item = slot.GetSellItem();
            if (item == null) continue;

            totalPrice += slot.GetBuyValue() * slot.GetQuantity();
        }
        totalPriceText.text = $"${totalPrice}";
    }
    #endregion
}
