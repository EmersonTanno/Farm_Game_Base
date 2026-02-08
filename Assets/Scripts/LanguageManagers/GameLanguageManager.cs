using System.Collections.Generic;
using UnityEngine;

class GameLanguageManager : MonoBehaviour
{
    public static GameLanguageManager Instance;

    private Dictionary<string, ItemLanguage> itemsLanguageMap;
    private Dictionary<string, SellInfoLanguageItem> sellMenuLanguageMap;

    void Awake()
    {
        Instance = this;

        LoadLanguageFile();
    }

    private void LoadLanguageFile()
    {
        TextAsset itemsJson = Resources.Load<TextAsset>("Languages/items_names");
        TextAsset sellMenuJson = Resources.Load<TextAsset>("Languages/sell_menu");

        if (itemsJson == null)
        {
            Debug.LogError("Language file for items not found!");
            return;
        }
        
        if (sellMenuJson == null)
        {
            Debug.LogError("Language file for sell menu not found!");
            return;
        }

        var itemsData = JsonUtility.FromJson<ItemLanguageData>(itemsJson.text);
        var sellMenuData = JsonUtility.FromJson<SellInfoLanguageData>(sellMenuJson.text);

        itemsLanguageMap = new Dictionary<string, ItemLanguage>();
        sellMenuLanguageMap = new Dictionary<string, SellInfoLanguageItem>();

        foreach (var item in itemsData.items)
        {
            itemsLanguageMap[item.key] = item;
        }

        foreach (var item in sellMenuData.items)
        {
            sellMenuLanguageMap[item.key] = item;
        }
    }


    public string GetItemName(Item item)
    {   
        string itemName;
        switch(GameConfigurations.Instance.gameLanguage)
        {
            case LanguageEnum.Potugues:
                itemName = itemsLanguageMap[$"item_{item.id}"].pt;
                break;
            case LanguageEnum.Ingles:
                itemName = itemsLanguageMap[$"item_{item.id}"].en;
                break;
            default:
                itemName = itemsLanguageMap[$"item_{item.id}"].pt;
                break;
        }
        return itemName;
    }

    public string GetSellMenuItemName(string item)
    {
        string itemName;
        switch(GameConfigurations.Instance.gameLanguage)
        {
            case LanguageEnum.Potugues:
                itemName = sellMenuLanguageMap[item].pt;
                break;
            case LanguageEnum.Ingles:
                itemName = sellMenuLanguageMap[item].en;
                break;
            default:
                itemName = sellMenuLanguageMap[item].pt;
                break;
        }
        return itemName;
    }
}