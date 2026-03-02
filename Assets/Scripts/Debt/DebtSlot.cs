using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebtSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI marksQuantity;
    [SerializeField] private TextMeshProUGUI percentage;
    [SerializeField] private TextMeshProUGUI limitDay;
    [SerializeField] private Button getButton;
    private DebtTypeData debtData = null;

    private event Action OnDebtGet;

    void OnEnable()
    {
        GameLanguageManager.OnLanguageChange += ReloadInfo;
    }

    void OnDisable()
    {
        GameLanguageManager.OnLanguageChange -= ReloadInfo;
    }

    public void SetSlotData(DebtTypeData newDebtData, bool buttonActive)
    {
        ResetSlot();

        debtData = newDebtData;

        ReloadInfo();
        
        getButton.interactable = buttonActive;
    }

    public void ResetSlot()
    {
        marksQuantity.text = null;
        percentage.text = null;
        limitDay.text = null;
        debtData = null;
    }

    public void GetDebt()
    {
        DebtController.Instance.CreateNewDebt(debtData.type, debtData.defaultInterest, debtData.amount, debtData.quantityDaysToPay, debtData.compoundInterest, debtData.maxDaysOver, debtData.creditorNpcId);
    
        OnDebtGet?.Invoke();
    }

    private void ReloadInfo()
    {
        GameLanguageManager gameLanguage = GameLanguageManager.Instance;
        Calendar_Controller calendar = Calendar_Controller.Instance;

        marksQuantity.text = $"Marks: {debtData.amount}";
        percentage.text = $"{gameLanguage.GetDebtShopMenuItemName("percentage")}: {debtData.defaultInterest}%";
        limitDay.text = $"{gameLanguage.GetDebtShopMenuItemName("dayLimit")}: {calendar.GetDate(debtData.quantityDaysToPay)}";   

        getButton.GetComponentInChildren<TextMeshProUGUI>().text = gameLanguage.GetDebtShopMenuItemName("getButton");
    }
}