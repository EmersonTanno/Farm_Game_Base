[System.Serializable]
public class LanguageItem
{
    public string key;
    public string pt;
    public string en;
}

[System.Serializable]
public class MainMenuLanguageData
{
    public LanguageItem[] items;
}