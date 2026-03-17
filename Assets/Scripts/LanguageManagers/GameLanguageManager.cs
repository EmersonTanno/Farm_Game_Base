using System;
using System.Collections.Generic;
using UnityEngine;

class GameLanguageManager : MonoBehaviour
{
    public static GameLanguageManager Instance;

    private Dictionary<string, FieldLanguage> itemsLanguageMap = new Dictionary<string, FieldLanguage>();
    private Dictionary<string, FieldLanguage> sellMenuLanguageMap = new Dictionary<string, FieldLanguage>();
    private Dictionary<string, FieldLanguage> shopMenuLanguageMap = new Dictionary<string, FieldLanguage>();
    private Dictionary<string, FieldLanguage> sleepMenuLanguageMap = new Dictionary<string, FieldLanguage>();
    private Dictionary<string, FieldLanguage> timeMenuLanguageMap = new Dictionary<string, FieldLanguage>();
    private Dictionary<string, FieldLanguage> pauseMenuLanguageMap = new Dictionary<string, FieldLanguage>();
    private Dictionary<string, FieldLanguage> debtShopMenuLanguageMap = new Dictionary<string, FieldLanguage>();
    private Dictionary<string, FieldLanguage> callendarLanguageMap = new Dictionary<string, FieldLanguage>();
    private Dictionary<string, FieldLanguage> debtLanguageMap = new Dictionary<string, FieldLanguage>();

    public static event Action OnLanguageChange;

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
        timeMenuLanguageMap = LoadFile("Languages/time_menu");
        pauseMenuLanguageMap = LoadFile("Languages/pause_menu");
        debtShopMenuLanguageMap = LoadFile("Languages/debtShop_menu");
        callendarLanguageMap = LoadFile("Languages/callendar");
        debtLanguageMap = LoadFile("Languages/debt");
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

    private string GetLangageFromDictionary(string item, Dictionary<string, FieldLanguage> dictionary)
    {
        if (string.IsNullOrEmpty(item))
        {
            Debug.LogWarning("Language key is null or empty.");
            return "INVALID_KEY";
        }

        if (dictionary == null)
        {
            Debug.LogError("Language dictionary is null.");
            return "DICT_NULL";
        }

        if (!dictionary.TryGetValue(item, out FieldLanguage field))
        {
            Debug.LogWarning($"Language key '{item}' not found.");
            return $"MISSING_{item}";
        }

        switch (GameConfigurations.Instance.gameLanguage)
        {
            case LanguageEnum.Potugues:
                return field.pt;

            case LanguageEnum.Ingles:
                return field.en;

            default:
                Debug.LogWarning("Language not mapped. Falling back to Portuguese.");
                return field.pt;
        }
    }

    public string GetItemName(Item item)
    {
        return GetLangageFromDictionary($"item_{item.id}", itemsLanguageMap);
    }

    public string GetSellMenuItemName(string item)
    {
        return GetLangageFromDictionary(item, sellMenuLanguageMap);
    }

    public string GetShopMenuItemName(string item)
    {
        return GetLangageFromDictionary(item, shopMenuLanguageMap);
    }

    public string GetSleepMenuItemName(string item)
    {
        return GetLangageFromDictionary(item, sleepMenuLanguageMap);
    }

    public string GetTimeMenuItemName(string item)
    {
        return GetLangageFromDictionary(item, timeMenuLanguageMap);
    }

    public string GetPauseMenuItemName(string item)
    {
        return GetLangageFromDictionary(item, pauseMenuLanguageMap);
    }

    public string GetDebtShopMenuItemName(string item)
    {  
        return GetLangageFromDictionary(item, debtShopMenuLanguageMap);
    }

    public string GetCallendarItemName(string item)
    {  
        return GetLangageFromDictionary(item, callendarLanguageMap);
    }

    public string GetDebtItemName(string item)
    {  
        return GetLangageFromDictionary(item, debtLanguageMap);
    }

    public void ChangeGameLanguage()
    {
        OnLanguageChange?.Invoke();
    }
}