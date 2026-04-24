using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class Shop_Manager : MonoBehaviour
{
    public static Shop_Manager Instance { get; private set; }

    #region Variables
    [Header("UI")]
    [SerializeField] GameObject shopCanvas;
    [SerializeField] TextMeshProUGUI totalPriceText;
    [SerializeField] Transform slotContainer;
    [SerializeField] GameObject slotPrefab;
    [SerializeField] TextMeshProUGUI buyButtonText;
    [SerializeField] TextMeshProUGUI exitButtonText;

    [Header("Itens por Estação")]
    [SerializeField] Item[] itemVerao;
    [SerializeField] Item[] itemOutono;
    [SerializeField] Item[] itemPrimavera;
    [SerializeField] Item[] itemInverno;
    [SerializeField] ShopDataBase shopDB;

    private List<ShopSlot> activeSlots = new List<ShopSlot>();
    public bool shopActive = false;
    public int totalPrice = 0;

    private ShopTypeEnum shopType = ShopTypeEnum.None;

    public static Action OnShopClose;
    public static event Action<Item[]> OnBuyItems;
    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //SetShopItens();
        SetCanvaLanguage();
    }

    void OnEnable()
    {
        //Tax_System.OnTaxChange += SetShopItens;
        //Calendar_Controller.OnMonthChange += SetShopItens;
        ShopSlot.OnAddRemoveItem += ReloadTotalPrice;
        GameLanguageManager.OnLanguageChange += SetCanvaLanguage;
        GameLanguageManager.OnLanguageChange += SetShopItens;
        DialogueManager.OnDialogueShopRequest += ActivateShop;
    }

    void OnDisable()
    {
        //Tax_System.OnTaxChange -= SetShopItens;
        //Calendar_Controller.OnMonthChange -= SetShopItens;
        ShopSlot.OnAddRemoveItem -= ReloadTotalPrice;
        GameLanguageManager.OnLanguageChange -= SetCanvaLanguage;
        GameLanguageManager.OnLanguageChange -= SetShopItens;
        DialogueManager.OnDialogueShopRequest -= ActivateShop;
    }
    #endregion

    #region Open / Close Shop
    public void ActivateShop(ShopTypeEnum openShopType)
    {
        UnityEngine.Debug.Log("Initializing Shop");
        if (Time_Controll.Instance.bedActive ||
            Player_Controller.Instance.CheckPlayerActions() ||
            InventoryManager.Instance.inventoryActive)
            return;
        
        shopType = openShopType;

        SetShopItens();

        shopActive = true;
        shopCanvas.SetActive(shopActive);
        Time_Controll.Instance.PauseTimer();
        ReloadTotalPrice();
    }

    public void DeactivateShop()
    {
        if (Time_Controll.Instance.bedActive ||
            Player_Controller.Instance.CheckPlayerActions() ||
            InventoryManager.Instance.inventoryActive)
            return;
        
        shopActive = false;
        if(GameSession.Instance.gameState != GameState.Cutscene && GameSession.Instance.gameState != GameState.PausedCutscene && GameSession.Instance.gameState != GameState.Dialogue && GameSession.Instance.gameState != GameState.PausedDialogue)
        {
            Time_Controll.Instance.UnpauseTimer();
        }
        shopCanvas.SetActive(shopActive);

        shopType = ShopTypeEnum.None;
        
        OnShopClose?.Invoke();
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
        //Adicionar aqui uma nova variável, provavelmente uma acessível pelo Shop_Manager inteiro para salvar o tipo de shop que está sendo acessado
        //com isso define o tipo de shop e qual será aberto dependendo de qual foi selecionado
        if(shopType == ShopTypeEnum.DefaultShop)
        {
            switch (Calendar_Controller.Instance.season)
            {
                case Season.Verao:
                    //CreateSlots(shopDB.GetShopData("defaultVerao").shopItems);
                    CreateSlots(itemVerao);
                    break;
                case Season.Outono:
                    //CreateSlots(shopDB.GetShopData("defaultOutono").shopItems);
                    CreateSlots(itemOutono);
                    break;
                case Season.Primavera:
                    //CreateSlots(shopDB.GetShopData("defaultPrimavera").shopItems);
                    CreateSlots(itemPrimavera);
                    break;
                case Season.Inverno:
                    //CreateSlots(shopDB.GetShopData("defaultInverno").shopItems);
                    CreateSlots(itemInverno);
                    break;
            }
        }
        else
        {
            
        }
    }
    #endregion

    #region Buy
    public void BuyItems()
    {
        if (Status_Controller.Instance.gold < totalPrice)
        {
            return;
        }

        List<Item> items = new List<Item>();

        foreach (var slot in activeSlots)
        {
            Item item = slot.GetSellItem();
            if (item == null) continue;

            for (int i = 0; i < slot.GetQuantity(); i++)
            {
                items.Add(item);
            }
        }

        Status_Controller.Instance.RemoveGold(totalPrice);

        OnBuyItems?.Invoke(items.ToArray());

        ResetQuantity();
        DeactivateShop();
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

    #region Canvas
    private void SetCanvaLanguage()
    {
        buyButtonText.text = GameLanguageManager.Instance.GetShopMenuItemName("buy");
        exitButtonText.text = GameLanguageManager.Instance.GetShopMenuItemName("exit");
    }
    #endregion
}
