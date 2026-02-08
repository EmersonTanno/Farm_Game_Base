[System.Serializable]
public class SellInfoLanguageItem
{
    public string key;
    public string pt;
    public string en;
}

[System.Serializable]
public class SellInfoLanguageData
{
    public SellInfoLanguageItem[] items;
}