[System.Serializable]
public class TaxSaveData
{
    public TaxSaveDataData taxData = new();
}

[System.Serializable]
public class TaxSaveDataData
{
    public float taxRate;
    public int taxPaidDuringYear;
    public float anualTaxPercentage;
    public int anualSells;
}
