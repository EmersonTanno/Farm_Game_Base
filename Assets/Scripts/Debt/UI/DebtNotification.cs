using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebtNotification : MonoBehaviour
{
    [SerializeField] private Animator myAnimator;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI debtValueText;
    [SerializeField] private TextMeshProUGUI finalDateText;

    private bool canChangeInfo = true;

    private List<DebtData> debts = new List<DebtData>();
    private Coroutine actualRoutine = null;

    void OnEnable()
    {
        DebtController.OnDebtCreation += DebtCreationNotification;
        GameLanguageManager.OnLanguageChange += ReloadData;
    }

    void OnDisable()
    {
        DebtController.OnDebtCreation -= DebtCreationNotification;
        GameLanguageManager.OnLanguageChange -= ReloadData;
    }

    private void DebtCreationNotification(DebtData debt)
    {
        debts.Add(debt);

        if(actualRoutine == null)
            actualRoutine = StartCoroutine(ShowDebtNotification());
    }

    private IEnumerator ShowDebtNotification()
    {
        while(debts.Count > 0)
        {
            canChangeInfo = false;

            SetNotificationData(debts[0]);
            myAnimator.SetTrigger("Active");
            debts.RemoveAt(0);

            while(!canChangeInfo || GameSession.Instance.gameState == GameState.Paused)
                yield return null;
        }

        actualRoutine = null;
    }

    private void SetNotificationData(DebtData debt)
    {
        titleText.text = GameLanguageManager.Instance.GetDebtItemName(debt.debtType.ToString().ToLower());
        debtValueText.text = $"O - {debt.debtMarksToPay}";
        finalDateText.text = Calendar_Controller.Instance.GetDate(debt.daysQuantityToPay);
    }

    private void ReloadData()
    {
        if(debts.Count > 0)
        {
            SetNotificationData(debts[0]);
        }
    }

    public void CanChangeInfo()
    {
        canChangeInfo = true;
    }
}