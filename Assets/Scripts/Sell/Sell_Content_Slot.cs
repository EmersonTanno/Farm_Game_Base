using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Sell_Content_Slot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI itemName; 
    [SerializeField] TextMeshProUGUI itemQuantity; 
    [SerializeField] Image itemSprite;

    public void SetInfo(Item item, int quantity)
    {
        itemName.text = GameLanguageManager.Instance.GetItemName(item);
        itemQuantity.text = $"x{quantity}";
        itemSprite.sprite = item.image;
    }
}
