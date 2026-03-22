using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebtNotification : MonoBehaviour
{
    [SerializeField] private Animator myAnimator;
    [SerializeField] private GameObject debtNotification;
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
            debtNotification.SetActive(true);
            canChangeInfo = false;

            SetNotificationData(debts[0]);
            myAnimator.SetTrigger("Active");
            debts.RemoveAt(0);

            while(!canChangeInfo || GameSession.Instance.gameState == GameState.Paused)
                yield return null;
        }

        debtNotification.SetActive(false);
        actualRoutine = null;
    }

    private void SetNotificationData(DebtData debt)
    {
        GameLanguageManager gameLanguageManager = GameLanguageManager.Instance;

        string baseText = gameLanguageManager.GetDebtItemName(debt.debtType.ToString().ToLower());

        if (baseText.Contains("{0}"))
        {
            titleText.text = string.Format(baseText, NPCController.Instance.GetNPC(debt.creditorNpcId).npcData.name);
        }
        else
        {
            titleText.text = baseText;
        }
        debtValueText.text = $"O - {debt.debtMarksToPay}";
        finalDateText.text = $"{gameLanguageManager.GetDebtItemName("until")}{Calendar_Controller.Instance.GetDate(debt.daysQuantityToPay)}";
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