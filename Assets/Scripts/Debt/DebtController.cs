using System;
using System.Collections.Generic;
using UnityEngine;

public class DebtController : MonoBehaviour
{
    private List<DebtData> actualDebtList = new List<DebtData>();
    private List<DebtData> defeatedDebtList = new List<DebtData>();
    private List<DebtData> historyDebtList = new List<DebtData>();

    private void CreateNewDebt(DebtTypeEnum type, float extraPercentageToPay, int quantityMarksTaken, int finalDay, Season finalSeason, int finalYear, float interestPercentage, int maxDaysOver)
    {
        DebtData newDebt = new DebtData
        {
            id =  $"DEBT_{type}_{Guid.NewGuid():N}",
            debtType = type,
            extraPercentageToPay = extraPercentageToPay,

            quantityMarksTaken = quantityMarksTaken,
            debtMarksToPay = quantityMarksTaken + (int)(quantityMarksTaken * extraPercentageToPay),

            startDay = Calendar_Controller.Instance.day,
            startSeason = Calendar_Controller.Instance.season,
            startYear = Calendar_Controller.Instance.year,

            finalDay = finalDay,
            finalSeason = finalSeason,
            finalYear = finalYear,

            interestPercentage = interestPercentage,
            
            maxDaysOver = maxDaysOver
        };

        actualDebtList.Add(newDebt);
    }

    private DebtData GetDebt(string debtId)
    {
        return actualDebtList.Find(i => i.id == debtId);
    }

    private void MoveDebt(string debtId, List<DebtData> targetList)
    {
        DebtData debt = actualDebtList.Find(i => i.id == debtId);

        if (debt == null)
            return;

        targetList.Add(debt);
        actualDebtList.Remove(debt);
    }

    private void PayDebt(string debtId)
    {
        DebtData debt = actualDebtList.Find(i => i.id == debtId);

        if (debt == null)
        {
            Debug.LogWarning($"Debt {debtId} not found.");
            return;
        }

        debt.paid = true;

        MoveDebt(debtId, historyDebtList);
    }

}