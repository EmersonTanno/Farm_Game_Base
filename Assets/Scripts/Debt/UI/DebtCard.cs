using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebtCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI debtTitle;
    [SerializeField] TextMeshProUGUI debtMarksTaken;
    [SerializeField] TextMeshProUGUI debtFees;
    [SerializeField] TextMeshProUGUI debtAcquiredIn;
    [SerializeField] TextMeshProUGUI debtDaysRemaining;
    [SerializeField] TextMeshProUGUI debtLateDays;
    [SerializeField] TextMeshProUGUI debtFinalValue;
    [SerializeField] Button payButton;
    [SerializeField] Image timeImage;
    [SerializeField] GameObject lockImage;
    [SerializeField] GameObject noMarksImage;
    [SerializeField] Sprite timeSpritesFull;
    [SerializeField] Sprite timeSpritesMoreHalf;
    [SerializeField] Sprite timeSpritesMinusHalf;
    [SerializeField] Sprite timeSpritesEmpty;
    

    private DebtData debtData;

    #region Set Data
    public void SetDebtCard(DebtData debtData)
    {
        this.debtData = debtData;
        LoadDebtInfo();
    }
    #endregion

    #region UI
    private void LoadDebtInfo()
    {
        GameLanguageManager languageManager = GameLanguageManager.Instance;

        string baseText = languageManager.GetDebtItemName(debtData.debtType.ToString().ToLower());

        if (baseText.Contains("{0}"))
        {
            debtTitle.text = string.Format(baseText, NPCController.Instance.GetNPC(debtData.creditorNpcId).npcData.name);
        }
        else
        {
            debtTitle.text = baseText;
        }

        if(debtData.quantityMarksTaken != 0)
        {
            debtMarksTaken.text = string.Format(languageManager.GetDebtItemName("marksTaken"), debtData.quantityMarksTaken, debtData.quantityMarksTaken + Mathf.RoundToInt(debtData.quantityMarksTaken * (debtData.extraPercentageToPay/100f)));
        }
        else
        {
            debtMarksTaken.text = languageManager.GetDebtItemName("marksTaken0");
        }
        
        debtFees.text = string.Format(languageManager.GetDebtItemName("fees"), $"{debtData.extraPercentageToPay:D2}");
        
        debtAcquiredIn.text = string.Format(languageManager.GetDebtItemName("debtAcquired"), $"{debtData.startDay:D2}", Calendar_Controller.Instance.GetSeason(debtData.startMonth), $"{debtData.startYear:D2}");

        int daysRemaining = debtData.daysQuantityToPay - debtData.passedDays;
        debtDaysRemaining.text = string.Format(languageManager.GetDebtItemName("daysRemaining"), $"{daysRemaining:D2}");

        debtLateDays.text = string.Format(languageManager.GetDebtItemName("daysLate"), $"{debtData.daysOverdue:D2}");

        debtFinalValue.text = string.Format(languageManager.GetDebtItemName("finalDebt"), debtData.debtMarksToPay);

        payButton.GetComponentInChildren<TextMeshProUGUI>().text = languageManager.GetDebtItemName("payButton");
        bool isShark = debtData.debtType == DebtTypeEnum.SHARK;
        bool hasMoney = Status_Controller.Instance.gold >= debtData.debtMarksToPay;

        lockImage.SetActive(isShark);
        noMarksImage.SetActive(!hasMoney);

        payButton.interactable = !isShark && hasMoney;

        if(isShark || !hasMoney)
        {
            payButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }


        float remainingRatio = (float)(debtData.daysQuantityToPay - debtData.passedDays) / debtData.daysQuantityToPay;
        if (debtData.daysOverdue > 0 || remainingRatio <= 0)
        {
            timeImage.sprite = timeSpritesEmpty;
        }
        else if (remainingRatio <= 0.33f)
        {
            timeImage.sprite = timeSpritesMinusHalf;
        }
        else if (remainingRatio <= 0.66f)
        {
            timeImage.sprite = timeSpritesMoreHalf;
        }
        else
        {
            timeImage.sprite = timeSpritesFull;
        }  
    }
    #endregion

    #region Pay Debt
    public void PayDebt()
    {
        DebtController.Instance.PayDebt(debtData.id);
    }
    #endregion
}