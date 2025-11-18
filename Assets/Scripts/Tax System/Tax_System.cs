using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tax_System : MonoBehaviour
{
    #region Variables
    public static Tax_System Instance { get; private set; }

    public float taxRate = 0;

    //Anual Taxes
    private int anualSells = 0;
    private int anualTaxBase = 500;
    private int anualTaxFinal = 0;
    #endregion

    void Awake()
    {
        Instance = this;
    }

    public int calculateAnualTax()
    {
        anualTaxFinal = (int)(anualTaxBase + (anualSells * 0.05));
        return anualTaxFinal;
    }


}
