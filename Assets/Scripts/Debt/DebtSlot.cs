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

    public static event Action OnDebtGet;

    void OnEnable()
    {
        GameLanguageManager.OnLanguageChange += ReloadInfo;
    }

    void OnDisable()
    {
        GameLanguageManager.OnLanguageChange -= ReloadInfo;
    }

    public void SetSlotData(DebtTypeData newDebtData, bool buttonActive, string npcId = "")
    {
        ResetSlot();

        debtData = newDebtData;
        if(npcId != "")
            debtData.creditorNpcId = npcId;

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
        bool canTakeDebt = DebtController.Instance.CreateNewDebt(debtData.type, debtData.defaultInterest, debtData.amount, debtData.quantityDaysToPay, debtData.compoundInterest, debtData.maxDaysOver, debtData.creditorNpcId);
        if(canTakeDebt)
            Status_Controller.Instance.AddGold(debtData.amount);
            
        OnDebtGet?.Invoke();
    }

    private void ReloadInfo()
    {
        GameLanguageManager gameLanguage = GameLanguageManager.Instance;
        Calendar_Controller calendar = Calendar_Controller.Instance;

        marksQuantity.text = $"O - {debtData.amount}";
        percentage.text = $"{gameLanguage.GetDebtShopMenuItemName("percentage")}: {debtData.defaultInterest}%";
        limitDay.text = $"{gameLanguage.GetDebtShopMenuItemName("dayLimit")}: {calendar.GetDate(debtData.quantityDaysToPay)}";   

        getButton.GetComponentInChildren<TextMeshProUGUI>().text = gameLanguage.GetDebtShopMenuItemName("getButton");
    }
}