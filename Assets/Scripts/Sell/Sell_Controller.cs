using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Sell_Controller : MonoBehaviour
{
    public static Sell_Controller Instance { get; private set; }

    #region Variables
    [SerializeField] GameObject sellUi;
    [SerializeField] TextMeshProUGUI totalText;
    [SerializeField] TextMeshProUGUI taxText;
    [SerializeField] TextMeshProUGUI gainText;

    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
    }
    #endregion


    public void SellItems(List<Item> sellItemsList)
    {
        int gainedValue = 0;
        int taxedValue = 0;
        int totalValue = 0;

        for (int i = 0; i < sellItemsList.Count; i++)
        {
            totalValue += sellItemsList[i].sellValue;
            taxedValue += Tax_System.Instance.ApplySellTaxes(sellItemsList[i].sellValue);
            gainedValue += sellItemsList[i].sellValue - Tax_System.Instance.ApplySellTaxes(sellItemsList[i].sellValue);
        }

        sellUi.SetActive(true);

        gainText.text = $"Recebido: ${gainedValue}";
        taxText.text = $"Taxas: -${taxedValue}";
        totalText.text = $"Total: ${totalValue}";

        Tax_System.Instance.AddSellItemsValueToAnualSells(totalValue);
        Status_Controller.Instance.AddGold(gainedValue);
    }

    private IEnumerator DeactivateUi()
    {
        yield return new WaitForSeconds(3f);
        sellUi.SetActive(false);
    }
}
