using System.Collections.Generic;

[System.Serializable]
public class DebtSaveData
{
    public List<DebtData> actualDebtList = new();
    public List<DebtData> historyDebtList = new();
}
