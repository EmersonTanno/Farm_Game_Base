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
    }

    void OnDisable()
    {
        DebtController.OnDebtCreation -= DebtCreationNotification;
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
            
            while(!canChangeInfo)
                yield return null;
        }

        actualRoutine = null;
    }

    private void SetNotificationData(DebtData debt)
    {
        titleText.text = debt.debtType.ToString();
        debtValueText.text = $"O - {debt.debtMarksToPay}";
        finalDateText.text = Calendar_Controller.Instance.GetDate(debt.daysQuantityToPay);
    }

    public void CanChangeInfo()
    {
        canChangeInfo = true;
    }
}