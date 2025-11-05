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

    [SerializeField] Item itemVerao;
    [SerializeField] Item itemOutono;
    [SerializeField] Item itemPrimavera;
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
    }

    private void SetShopItens()
    {
        switch (Calendar_Controller.Instance.season)
        {
            case Season.Verao:
                sellItem = itemVerao;
                break;
            case Season.Outono:
                sellItem = itemOutono;
                break;
            case Season.Primavera:
                sellItem = itemPrimavera;
                break;
        }
        itemImage.sprite = sellItem.image;
        itemText.text = sellItem.itemName;
        itemPrice.text = $"$ {sellItem.buyValue}";
    }
    
    public void BuyItem()
    {
        if (Status_Controller.Instance.gold < sellItem.buyValue) return;

        Status_Controller.Instance.RemoveGold(sellItem.buyValue);
        InventoryManager.Instance.AddItem(sellItem);
    }

}
