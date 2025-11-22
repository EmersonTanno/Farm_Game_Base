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

    private int gainedValue = 0;
    private int taxedValue = 0;
    private int totalValue = 0;
    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
    }
    #endregion


    public void UpdateSellUi(int receivedValue, int taxValue)
    {
        sellUi.SetActive(true);
        gainText.text = $"Recebido: ${receivedValue}";
        taxText.text = $"Taxas: -${taxValue}";
        totalText.text = $"Total: ${totalValue}";
    }
}
