using System.Collections.Generic;
using UnityEngine;

class GameLanguageManager : MonoBehaviour
{
    public static GameLanguageManager Instance;

    private Dictionary<string, FieldLanguage> itemsLanguageMap = new Dictionary<string, FieldLanguage>();
    private Dictionary<string, FieldLanguage> sellMenuLanguageMap = new Dictionary<string, FieldLanguage>();
    private Dictionary<string, FieldLanguage> shopMenuLanguageMap = new Dictionary<string, FieldLanguage>();
    private Dictionary<string, FieldLanguage> sleepMenuLanguageMap = new Dictionary<string, FieldLanguage>();

    void Awake()
    {
        Instance = this;

        LoadLanguageFile();
    }

    private void LoadLanguageFile()
    {
        itemsLanguageMap = LoadFile("Languages/items_names");
        sellMenuLanguageMap = LoadFile("Languages/sell_menu");
        shopMenuLanguageMap = LoadFile("Languages/shop_menu");
        sleepMenuLanguageMap = LoadFile("Languages/sleep_menu");
    }

    private Dictionary<string, FieldLanguage> LoadFile(string filePath)
    {
        var map = new Dictionary<string, FieldLanguage>();
        TextAsset itemsJson = Resources.Load<TextAsset>(filePath);

        if (itemsJson == null)
        {
            Debug.LogError($"Language file for path '{filePath}' not found!");
            return null;
        }

        var itemsData = JsonUtility.FromJson<FieldLanguageData>(itemsJson.text);

        foreach (var item in itemsData.items)
        {
            map[item.key] = item;
        }

        return map;
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

    public string GetShopMenuItemName(string item)
    {
        string itemName;
        switch(GameConfigurations.Instance.gameLanguage)
        {
            case LanguageEnum.Potugues:
                itemName = shopMenuLanguageMap[item].pt;
                break;
            case LanguageEnum.Ingles:
                itemName = shopMenuLanguageMap[item].en;
                break;
            default:
                itemName = shopMenuLanguageMap[item].pt;
                break;
        }
        return itemName;
    }

    public string GetSleepMenuItemName(string item)
    {
        string itemName;
        switch(GameConfigurations.Instance.gameLanguage)
        {
            case LanguageEnum.Potugues:
                itemName = sleepMenuLanguageMap[item].pt;
                break;
            case LanguageEnum.Ingles:
                itemName = sleepMenuLanguageMap[item].en;
                break;
            default:
                itemName = sleepMenuLanguageMap[item].pt;
                break;
        }
        return itemName;
    }
}