using System;
using UnityEngine;

public class Tax_System : MonoBehaviour
{
    #region Variables
    public static Tax_System Instance { get; private set; }

    private float taxRate = 0.05f;
    private int taxPaidDuringYear = 0;

    //Anual Taxes
    private float anualTaxPercentage = 0.05f;
    private int anualSells = 0;
    private int anualTaxBase = 500;
    private int anualTaxFinal = 0;


    public static event Action OnTaxChange;
    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    void OnEnable()
    {
        Calendar_Controller.OnMonthChange += SetSellTaxes;
         Calendar_Controller.OnDayChange += SetSellTaxes;
    }

    void OnDisable()
    {
        Calendar_Controller.OnMonthChange += SetSellTaxes;
    }
    #endregion

    #region Buy Taxes
    public int ApplyBuyTaxes(int value)
    {
        return (int)(value + (value * (taxRate/2)));
    }
    #endregion

    #region Sell Taxes
    public int ApplySellTaxes(int value)
    {
        return (int)(value * taxRate);
    }

    public void SetSellTaxes()
    {
        int lucky = Status_Controller.Instance.GetLucky();

        int baseMin = 5;
        int baseMax = 35;

        int minPercent = baseMin + (10 - lucky);
        int maxPercent = baseMax - lucky;

        minPercent = Mathf.Clamp(minPercent, baseMin, baseMax);
        maxPercent = Mathf.Clamp(maxPercent, minPercent, baseMax);

        int taxPercent = UnityEngine.Random.Range(minPercent, maxPercent + 1);

        taxRate = taxPercent / 100f;

        Debug.Log($"Sorte: {lucky} | Faixa: {minPercent}% - {maxPercent}% | Imposto do dia: {taxPercent}%");

        OnTaxChange?.Invoke();
    }

    public void UpdateTaxPaidDuringYear(int value)
    {
        taxPaidDuringYear += value;
    }
    #endregion

    #region Anual Taxes
    public int CalculateAnualTax()
    {
        anualTaxFinal = (int)(anualTaxBase + (anualSells * anualTaxPercentage));
        return anualTaxFinal;
    }

    public void AddSellItemsValueToAnualSells(int value)
    {
        anualSells += value;
    }
    #endregion

    #region Get
    public int GetTax()
    {
        return (int)(taxRate * 100);
    }

    public int GetTaxPaidDuringYear()
    {
        return taxPaidDuringYear;
    }
    #endregion
}
