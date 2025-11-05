using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image itemImage;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TextMeshProUGUI itemPrice;

    private Item sellItem;

    public Item GetSellItem()
    {
        return sellItem;
    }
    private void SetSellItem(Item item)
    {
        sellItem = item;
    }

    public void SetItem(Item item)
    {
        SetSellItem(item);

        itemImage.sprite = sellItem.image;
        itemText.text = sellItem.itemName;
        itemPrice.text = $"$ {sellItem.buyValue}";
    }

    public void BuyItem()
    {
        Shop_Manager.Instance.BuyItem(this);
    }
}
