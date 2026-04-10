using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebtShopObject : ShopObject
{
    [SerializeField] public string debtDialogueIdIfHaveMoney;
    [SerializeField] public string debtDialogueIdIfNotHaveMoney;
    
    public override void OpenNPCShop()
    {
        IReadOnlyList<DebtData> debts = DebtController.Instance.GetAllActiveDebts();

        DebtData debt = debts.FirstOrDefault(d => 
            d.creditorNpcId == ownerNpcID && 
            d.state != DebtStateEnum.Paid
        );

        if (debt != null)
        {
            if(Status_Controller.Instance.gold >= debt.debtMarksToPay)
            {
                TriggerShopDialogue(dialogueShopID, debtDialogueIdIfHaveMoney, ownerNpcID);
            }
            else
            {
                TriggerShopDialogue(dialogueShopID, debtDialogueIdIfNotHaveMoney, ownerNpcID);
            }
        }
        else
        {
            base.OpenNPCShop();
        }
    }
}