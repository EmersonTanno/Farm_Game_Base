using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Calendar_Controller : MonoBehaviour
{
    #region Variables
    //Day and Month
    public int day = 0;
    public int month = 0;
    public int year = 0;

    //Canvas
    [SerializeField] TextMeshProUGUI daysText;

    //Event
    public static event Action OnDayChange;
    public static event Action OnMonthChange;
    public static event Action OnYearChange;
    #endregion

    void Awake()
    {
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
        switch (month)
        {
            case 1:
                monthName = "Ver";
                break;
            case 2:
                monthName = "Out";
                break;
            case 3:
                monthName = "Inv";
                break;
            case 4:
                monthName = "Pri";
                break;
        }
        if (daysText != null)
        {
            daysText.text = $"Data: {day:D2}/{monthName}";
        }
    }
}

