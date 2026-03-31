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
    [SerializeField] private GameObject payedStamp;

    private bool canChangeInfo = true;

    private List<DebtNotificationData> debts = new List<DebtNotificationData>();
    private Coroutine actualRoutine = null;

    void OnEnable()
    {
        DebtController.OnDebtCreation += DebtCreationNotification;
        DebtController.OnDebtPayment += DebtPayNotification;
        GameLanguageManager.OnLanguageChange += ReloadData;
    }

    void OnDisable()
    {
        DebtController.OnDebtCreation -= DebtCreationNotification;
        DebtController.OnDebtPayment -= DebtPayNotification;
        GameLanguageManager.OnLanguageChange -= ReloadData;
    }

    private void DebtCreationNotification(DebtData debt)
    {
        debts.Add(new DebtNotificationData(debt, false));

        if(actualRoutine == null)
            actualRoutine = StartCoroutine(ShowDebtNotification());
    }

    private void DebtPayNotification(DebtData debt)
    {
        if(DebtController.Instance.GetDebtListActive())
        {
            return;
        }

        debts.Add(new DebtNotificationData(debt, true));

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

    private void SetNotificationData(DebtNotificationData debt)
    {
        GameLanguageManager gameLanguageManager = GameLanguageManager.Instance;

        string baseText = gameLanguageManager.GetDebtItemName(debt.debt.debtType.ToString().ToLower());

        if (baseText.Contains("{0}"))
        {
            NPC npc = NPCController.Instance.GetNPC(debt.debt.creditorNpcId);
            if(npc == null)
            {
                titleText.text = string.Format(baseText, "???");
            }
            else
            {
                titleText.text = string.Format(baseText, npc.npcData.name);
            }
        }
        else
        {
            titleText.text = baseText;
        }


        debtValueText.text = $"O - {debt.debt.debtMarksToPay}";

        if(debt.isPayment)
        {
            payedStamp.SetActive(true);
            finalDateText.text = $"{gameLanguageManager.GetDebtItemName("until")}---";
        }
        else
        {
            payedStamp.SetActive(false);
            finalDateText.text = $"{gameLanguageManager.GetDebtItemName("until")}{Calendar_Controller.Instance.GetDate(debt.debt.daysQuantityToPay)}";
        }
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