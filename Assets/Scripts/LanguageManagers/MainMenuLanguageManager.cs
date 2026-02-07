using System.Collections.Generic;
using UnityEngine;

public class MainMenuLanguageManager : MonoBehaviour
{
    public static MainMenuLanguageManager Instance;

    private Dictionary<string, LanguageItem> languageMap;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadLanguageFile();
    }

    private void LoadLanguageFile()
    {
        TextAsset json = Resources.Load<TextAsset>("Languages/main_menu");

        if (json == null)
        {
            Debug.LogError("Language file not found!");
            return;
        }

        var data = JsonUtility.FromJson<MainMenuLanguageData>(json.text);

        languageMap = new Dictionary<string, LanguageItem>();

        foreach (var item in data.items)
        {
            languageMap[item.key] = item;
        }
    }

    public string GetText(string key)
    {
        if (!languageMap.ContainsKey(key))
            return $"#{key}";

        var item = languageMap[key];

        return GameConfigurations.Instance.gameLanguage == LanguageEnum.Potugues
            ? item.pt
            : item.en;
    }
}
