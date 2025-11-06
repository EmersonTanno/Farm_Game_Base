using UnityEngine;
using TMPro;
using System;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image itemImage;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TMP_InputField itemQuantity;

    private Item sellItem;
    private int quantity = 0;
    public static event Action OnAddRemoveItem;

    public Item GetSellItem()
    {
        return sellItem;
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
        itemText.text = $"{sellItem.itemName} ${sellItem.buyValue}";
        itemQuantity.text = quantity.ToString();
    }

    public void BuyItem()
    {
        Shop_Manager.Instance.BuyItem(this);
    }

    public void AddItemToCart()
    {
        if (Shop_Manager.Instance.totalPrice + sellItem.buyValue > Status_Controller.Instance.gold)
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
                SetQuantityText(quantity);
                return;
            }
        }

        quantity = int.Parse(itemQuantity.text);
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
}
