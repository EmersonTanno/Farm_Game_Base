using System.Collections.Generic;
using UnityEngine;

class GameLanguageManager : MonoBehaviour
{
    public static GameLanguageManager Instance;

    private Dictionary<string, ItemLanguage> itemsLanguageMap;

    void Awake()
    {
        Instance = this;

        LoadLanguageFile();
    }

    private void LoadLanguageFile()
    {
        TextAsset json = Resources.Load<TextAsset>("Languages/items_names");

        if (json == null)
        {
            Debug.LogError("Language file not found!");
            return;
        }

        var data = JsonUtility.FromJson<ItemLanguageData>(json.text);

        itemsLanguageMap = new Dictionary<string, ItemLanguage>();

        foreach (var item in data.items)
        {
            itemsLanguageMap[item.key] = item;
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
}