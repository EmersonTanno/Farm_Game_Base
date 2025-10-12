using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Calendar_Controller : MonoBehaviour
{
    public static Calendar_Controller Instance { get; private set; }

    #region Variables
    //Day and Month
    public int day = 0;
    public int month = 0;
    public int year = 0;
    public Season season;

    //Canvas
    [SerializeField] TextMeshProUGUI daysText;

    //Light
    [SerializeField] Light2D globalLight;

    //Event
    public static event Action OnDayChange;
    public static event Action OnMonthChange;
    public static event Action OnYearChange;
    #endregion

    void Awake()
    {
        Instance = this;
        UpdateCanvas();
    }

    void OnEnable()
    {
        Time_Controll.OnMidNightChange += ChangeDay;
    }

    void OnDisable()
    {
        Time_Controll.OnMidNightChange -= ChangeDay;
    }

    private void ChangeDay()
    {
        day++;
        if(day > 30)
        {
            month++;
            day = 1;
            OnMonthChange?.Invoke();
            if(month > 4)
            {
                month = 1;
                OnYearChange?.Invoke();
            }
        }
        OnDayChange?.Invoke();
        UpdateCanvas();
    }

    private void UpdateCanvas()
    {
        string monthName = "";
        Color newColor;
        switch (month)
        {
            case 1:
                monthName = "Ver";
                if (UnityEngine.ColorUtility.TryParseHtmlString("#FFCEBF", out newColor))
                {
                    season = Season.Verao;
                    globalLight.color = newColor;
                }
                break;
            case 2:
                monthName = "Out";
                if (UnityEngine.ColorUtility.TryParseHtmlString("#FFC899", out newColor))
                {
                    season = Season.Outono;
                    globalLight.color = newColor;
                }
                break;
            case 3:
                monthName = "Inv";
                if (UnityEngine.ColorUtility.TryParseHtmlString("#99DCFF", out newColor))
                {
                    season = Season.Inverno;
                    globalLight.color = newColor;
                }
                break;
            case 4:
                monthName = "Pri";
                if (UnityEngine.ColorUtility.TryParseHtmlString("#C2FFCA", out newColor))
                {
                    season = Season.Primavera;
                    globalLight.color = newColor;
                }
                break;
        }
        if (daysText != null)
        {
            daysText.text = $"Data: {day:D2}/{monthName}";
        }
    }
}

