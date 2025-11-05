using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Shop_Manager : MonoBehaviour
{
    #region Variables
    public static Shop_Manager Instance { get; private set; }
    public bool shopActive = false;
    private Item sellItem;
    [SerializeField] GameObject shopCanvas;
    [SerializeField] UnityEngine.UI.Image itemImage;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TextMeshProUGUI itemPrice;

    [SerializeField] Item[] itemVerao;
    [SerializeField] Item[] itemOutono;
    [SerializeField] Item[] itemPrimavera;
    [SerializeField] Item[] itemInverno;

    [SerializeField] GameObject[] itemSlosts;
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
    }

    void OnDisable()
    {
        Calendar_Controller.OnMonthChange -= SetShopItens;
    }
    #endregion

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
    }

    private void ActivateSlots(bool status)
    {
        if (status)
        {
            for (int i = 0; i < itemSlosts.Length; i++)
            {
                GameObject slot = itemSlosts[i];
                ShopSlot shopSlot = itemSlosts[i].GetComponent<ShopSlot>();
                if (shopSlot.GetSellItem())
                {
                    itemSlosts[i].SetActive(true);
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
        for(int i = 0; i < items.Length; i++)
        {
            ShopSlot shopSlot = itemSlosts[i].GetComponent<ShopSlot>();
            shopSlot.SetItem(items[i]);
        }
    }
    
    public void BuyItem(ShopSlot slot)
    {
        Item buiyngItem = slot.GetSellItem();

        if (Status_Controller.Instance.gold < buiyngItem.buyValue)
        {
            Debug.Log("Deu merda");
            return;
        } 

        Status_Controller.Instance.RemoveGold(buiyngItem.buyValue);
        InventoryManager.Instance.AddItem(buiyngItem);
    }

}
