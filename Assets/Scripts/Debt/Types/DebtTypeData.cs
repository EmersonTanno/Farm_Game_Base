using UnityEngine;

[CreateAssetMenu]
public class DebtTypeData : ScriptableObject
{
    public DebtTypeEnum type;
    public string displayName;
    public int defaultInterest;
    public int amount;
    public int compoundInterest;
    public int maxDaysOver;
    public int quantityDaysToPay;
    public int creditorNpcId = -1;

    public DebtData ToDebtData()
    {
        var calendar = Calendar_Controller.Instance;

        DebtData newDebt = new DebtData
        {
            //id = $"DEBT_{type}_{Guid.NewGuid():N}",

            debtType = type,
            creditorNpcId = creditorNpcId,

            // Base
            quantityMarksTaken = amount,

            // Total com juros inicial
            debtMarksToPay = amount + Mathf.RoundToInt(amount * (defaultInterest / 100f)),

            // Data inicial
            startDay = calendar.day,
            startMonth = calendar.month,
            startYear = calendar.year,

            // Tempo
            daysQuantityToPay = quantityDaysToPay,

            // Juros
            interestPercentage = compoundInterest,

            // Limite de atraso
            maxDaysOver = maxDaysOver
        };

        return newDebt;
    }
}