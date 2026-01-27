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

    [SerializeField] GameObject timeGroup;

    //Event
    public static event Action OnDayChange;
    public static event Action OnMonthChange;
    public static event Action OnYearChange;
    #endregion

    #region Core
    void Awake()
    {
        Instance = this;
    }
    
    private void Start() {
        UpdateCanvas();
    }
    #endregion

    #region Events
    void OnEnable()
    {
        Time_Controll.OnMidNightChange += ChangeDay;
        Status_Controller.OnFaint += ChangeDay;
    }

    void OnDisable()
    {
        Time_Controll.OnMidNightChange -= ChangeDay;
        Status_Controller.OnFaint -= ChangeDay;
    }
    #endregion

    #region Callendar Functions
    private void ChangeDay()
    {
        day++;
        OnDayChange?.Invoke();
        if (day > 30)
        {
            month++;
            day = 1;
            if (month > 4)
            {
                month = 1;
                OnYearChange?.Invoke();
            }
            SetSeason();
            OnMonthChange?.Invoke();
        }
        UpdateCanvas();
        Debug.Log(GetWeekDay());
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

    private void SetSeason()
    {
        switch (month)
        {
            case 1:
                season = Season.Verao;
                break;
            case 2:
                season = Season.Outono;
                break;
            case 3:
                season = Season.Inverno;
                break;
            case 4:
                season = Season.Primavera;
                break;
        }
    }

    public WeekDayEnum GetWeekDay()
    {
        int index = (day - 1) % 7;
        return (WeekDayEnum)index;
    }
    #endregion

    #region Ui
    public void ControllTimeGroup(bool setActive)
    {
        timeGroup.SetActive(setActive);
    }
    #endregion
}

