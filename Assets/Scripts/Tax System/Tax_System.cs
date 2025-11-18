using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tax_System : MonoBehaviour
{
    #region Variables
    public static Tax_System Instance { get; private set; }

    private float taxRate = 0.05f;

    //Anual Taxes
    private float anualTaxPercentage = 0.05f;
    private int anualSells = 0;
    private int anualTaxBase = 500;
    private int anualTaxFinal = 0;
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
        //Calendar_Controller.OnDayChange += test;
    }

    void OnDisable()
    {
        //Calendar_Controller.OnDayChange += test;
    }
    #endregion

    #region Sell Taxes
    public int ApplySellTaxes(int value)
    {
        return (int)(value * taxRate);
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



}
