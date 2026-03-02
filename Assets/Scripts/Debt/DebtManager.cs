using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebtManager : MonoBehaviour
{
    private bool isActive = false;
    private string languageKey = "";
    [SerializeField] DebtController controller;
    [SerializeField] List<DebtTypeData> bankDebts;
    [SerializeField] List<DebtTypeData> sharkDebts;
    [SerializeField] List<DebtTypeData> cityDebts;

    [SerializeField] GameObject debtCanvas;
    [SerializeField] TextMeshProUGUI debtTitle;
    [SerializeField] List<DebtSlot> slots;

    void OnEnable()
    {
        GameLanguageManager.OnLanguageChange += ReloadLangauge;
    }

    void OnDisable()
    {
        GameLanguageManager.OnLanguageChange -= ReloadLangauge;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            if(!isActive)
                SetSharkDebts();
            else 
                DeactivateDebtWindow();
        }
    }

    private void SetSharkDebts()
    {
        if(isActive) return;
        isActive = true;
        debtCanvas.SetActive(isActive);
        languageKey = "shark";
        debtTitle.text = GameLanguageManager.Instance.GetDebtShopMenuItemName(languageKey);
        for(int i = 0; i < sharkDebts.Count; i++)
        {
            slots[i].SetSlotData(sharkDebts[i], true);
        }
    }

    public void DeactivateDebtWindow()
    {
        if(!isActive) return;

        isActive = false;
        debtCanvas.SetActive(isActive);
    }

    private void ReloadLangauge()
    {
        debtTitle.text = GameLanguageManager.Instance.GetDebtShopMenuItemName(languageKey);
    }
}