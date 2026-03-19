using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebtManager : MonoBehaviour
{
    private bool isActive = false;
    private string languageKey = "shark";
    [SerializeField] DebtController controller;
    [SerializeField] List<DebtTypeData> bankDebts;
    [SerializeField] List<DebtTypeData> sharkDebts;
    [SerializeField] List<DebtTypeData> cityDebts;

    [SerializeField] GameObject debtCanvas;
    [SerializeField] TextMeshProUGUI debtTitle;
    [SerializeField] List<DebtSlot> slots;

    public static event Action OnDebtWindowClose;

    void OnEnable()
    {
        GameLanguageManager.OnLanguageChange += ReloadLangauge;
        DebtSlot.OnDebtGet += DeactivateDebtWindow;
        DialogueManager.OnDialogueDebtRequest += ActivateDebtShop;
    }

    void OnDisable()
    {
        GameLanguageManager.OnLanguageChange -= ReloadLangauge;
        DebtSlot.OnDebtGet -= DeactivateDebtWindow;
        DialogueManager.OnDialogueDebtRequest -= ActivateDebtShop;
    }

    private void ActivateDebtShop(DebtTypeEnum debtType, int npcId)
    {
        switch(debtType)
        {
            case DebtTypeEnum.SHARK:
                SetSharkDebts(npcId);
                break;

            case DebtTypeEnum.BANK:
                SetBankDebts();
                break;
            
            case DebtTypeEnum.CITY:

                break;
            
            default:
                OnDebtWindowClose?.Invoke();
                break;
        }
    }

    private void SetDebts(List<DebtTypeData> debts, string debtType,int npcId = -1)
    {
        if(isActive) return;
        isActive = true;
        debtCanvas.SetActive(isActive);
        languageKey = debtType;
        debtTitle.text = GameLanguageManager.Instance.GetDebtShopMenuItemName(debtType);

        for(int i = 0; i < debts.Count; i++)
        {
            slots[i].SetSlotData(debts[i], DebtController.Instance.CheckIfCanCreateDebt(debts[i].ToDebtData()), npcId);
        }
    }

    private void SetSharkDebts(int npcId)
    {
        SetDebts(sharkDebts, "shark", npcId);
    }

    private void SetBankDebts()
    {
        SetDebts(bankDebts, "bank");
    }

    public void DeactivateDebtWindow()
    {
        if(!isActive) return;

        isActive = false;
        debtCanvas.SetActive(isActive);
        OnDebtWindowClose?.Invoke();
    }

    private void ReloadLangauge()
    {
        debtTitle.text = GameLanguageManager.Instance.GetDebtShopMenuItemName(languageKey);
    }
}