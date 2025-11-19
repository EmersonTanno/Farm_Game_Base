using UnityEngine;
using TMPro;
using System;

public class ShopSlot : MonoBehaviour
{
    #region Variables
    [SerializeField] UnityEngine.UI.Image itemImage;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TMP_InputField itemQuantity;
    [SerializeField] Transform itemQuantityPos;
    private Vector2 itemQuantityStartPos;

    private Item sellItem;
    private int quantity = 0;
    private int buyValue = 0;

    //Events
    public static event Action OnAddRemoveItem;
    #endregion

    void Start()
    {
        itemQuantityStartPos = itemQuantityPos.position;
    }

    #region Shop Functions
    public Item GetSellItem()
    {
        return sellItem;
    }

    public int GetBuyValue()
    {
        return buyValue;
    }

    public int GetQuantity()
    {
        return quantity;
    }
    private void SetSellItem(Item item)
    {
        sellItem = item;
    }

    public void SetItem(Item item)
    {
        SetSellItem(item);

        itemImage.sprite = sellItem.image;
        buyValue = sellItem.buyValue + Tax_System.Instance.ApplySellTaxes(sellItem.buyValue);
        itemText.text = $"{sellItem.itemName} ${buyValue}";
        itemQuantity.text = quantity.ToString();
    }

    public void AddItemToCart()
    {
        if (Shop_Manager.Instance.totalPrice + buyValue > Status_Controller.Instance.gold)
        {
            return;
        }
        quantity++;
        SetQuantityText(quantity);
        OnAddRemoveItem?.Invoke();
    }

    public void RemoveItemfromCart()
    {
        if (quantity <= 0) return;
        quantity--;
        SetQuantityText(quantity);
        OnAddRemoveItem?.Invoke();
    }

    public void SetInputQuantity()
    {
        int newQuantity = int.Parse(itemQuantity.text);
        if (newQuantity > quantity)
        {
            newQuantity -= quantity;
            if (Shop_Manager.Instance.totalPrice + (sellItem.buyValue * newQuantity) > Status_Controller.Instance.gold)
            {
                itemQuantityPos.position = itemQuantityStartPos;
                SetQuantityText(quantity);
                return;
            }
        }

        quantity = int.Parse(itemQuantity.text);
        itemQuantityPos.position = itemQuantityStartPos;
        OnAddRemoveItem?.Invoke();
    }

    private void SetQuantityText(int newQuantity)
    {
        itemQuantity.text = newQuantity.ToString(); 
    }

    public void Reset()
    {
        quantity = 0;
        SetQuantityText(quantity);
    }
    #endregion
}
