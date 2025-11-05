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

    private void setSellItem(Item item)
    {
        sellItem = item;
    }

    public void setItem(Item item)
    {
        setSellItem(item);

        itemImage.sprite = sellItem.image;
        itemText.text = sellItem.itemName;
        itemPrice.text = $"$ {sellItem.buyValue}";
    }
}
