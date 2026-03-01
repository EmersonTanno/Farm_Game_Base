using System;
using System.Collections.Generic;
using UnityEngine;

public class DebtController : MonoBehaviour
{
    private List<DebtData> actualDebtList = new List<DebtData>();
    private List<DebtData> historyDebtList = new List<DebtData>();

    #region Create Debt
    private void CreateNewDebt(DebtTypeEnum type, int extraPercentageToPay, int quantityMarksTaken, int finalDay, int finalMonth, int finalYear, int interestPercentage, int maxDaysOver, int creditorNpcId = -1)
    {
        DebtData newDebt = new DebtData
        {
            id =  $"DEBT_{type}_{Guid.NewGuid():N}",
            debtType = type,
            creditorNpcId = creditorNpcId,
            extraPercentageToPay = extraPercentageToPay,

            quantityMarksTaken = quantityMarksTaken,
            debtMarksToPay = quantityMarksTaken + Mathf.RoundToInt(quantityMarksTaken * (extraPercentageToPay/100f)),

            startDay = Calendar_Controller.Instance.day,
            startMonth = Calendar_Controller.Instance.month,
            startYear = Calendar_Controller.Instance.year,

            finalDay = finalDay,
            finalMonth = finalMonth,
            finalYear = finalYear,

            interestPercentage = interestPercentage,
            
            maxDaysOver = maxDaysOver
        };

        actualDebtList.Add(newDebt);
    }
    #endregion

    #region Get Debts
    public IReadOnlyList<DebtData> GetAllActiveDebts()
    {
        return actualDebtList;
    }

    public List<DebtData> GetAllDefeatedDebts()
    {
        return actualDebtList.FindAll(d => d.state == DebtStateEnum.Defeated);
    }

    public List<DebtData> GetAllHistoryDebts()
    {
        return historyDebtList;
    }

    private DebtData GetDebt(string debtId)
    {
        return actualDebtList.Find(i => i.id == debtId);
    }
    #endregion

    #region Move Debt
    private void MoveDebt(string debtId, List<DebtData> targetList)
    {
        DebtData debt = actualDebtList.Find(i => i.id == debtId);

        if (debt == null)
            return;

        targetList.Add(debt);
        actualDebtList.Remove(debt);
    }
    #endregion

    #region Pay Debt
    private void PayDebt(string debtId)
    {
        DebtData debt = actualDebtList.Find(i => i.id == debtId);

        if (debt == null)
        {
            Debug.LogWarning($"Debt {debtId} not found.");
            return;
        }

        debt.state = DebtStateEnum.Paid;

        MoveDebt(debtId, historyDebtList);
    }
    #endregion

    #region Pass Day
    private void PassDay()
    {
        if (actualDebtList.Count == 0)
            return;

        for (int i = actualDebtList.Count - 1; i >= 0; i--)
        {
            DebtData debt = actualDebtList[i];

            if (IsPastDue(debt) && debt.state == DebtStateEnum.Active)
            {
                debt.state = DebtStateEnum.Defeated;
            }

            if(debt.state == DebtStateEnum.Defeated)
            {
                debt.daysOverdue++;
                if (debt.daysOverdue == 1 || debt.daysOverdue % 7 == 0)
                {
                    debt.debtMarksToPay += Mathf.RoundToInt(debt.debtMarksToPay * (debt.interestPercentage/100f));
                }
            }
        }
    }

    private bool IsPastDue(DebtData debt)
    {
        var calendar = Calendar_Controller.Instance;

        if (calendar.year > debt.finalYear)
            return true;

        if (calendar.year == debt.finalYear)
        {
            if (calendar.month > debt.finalMonth)
                return true;

            if (calendar.month == debt.finalMonth)
            {
                if (calendar.day > debt.finalDay)
                    return true;
            }
        }

        return false;
    }
    #endregion

}