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
    }

    private void UpdateCanvas()
    {
        string monthName = GetSeason(month);
        if (daysText != null)
        {
            daysText.text = $"{day:D2}/{monthName}";
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

    #region Get Info
    public string GetSeason(int month)
    {
        if(month <= 0)
        {
            return "";
        }
        if(month > 4)
        {
            month %= 4;
        }
        switch (month)
        {
            case 1:
                return "Ver";
            case 2:
                return "Out";
            case 3:
                return "Inv";
            case 4:
                return "Pri";
            default:
                return "";
        }
    }

    public string GetDate(int overDays = 0)
    {
        int returnDay = day + overDays;
        int returnMonth = month;
        int returnYear = year;

        // Ajusta dias que passam de 30
        while (returnDay > 30)
        {
            returnDay -= 30;
            returnMonth++;

            if (returnMonth > 4) // passou o último mês
            {
                returnMonth = 1;
                returnYear++;
            }
        }

        return $"{returnDay} / {GetSeason(returnMonth)} / {returnYear}";
    }
    #endregion

    #region Save / Load
    public void Save(ref CalendarSaveData data)
    {
        data.calendar.day = day;
        data.calendar.month = month;
        data.calendar.year = year;
        data.calendar.season = season;
    }

    public void Load(CalendarSaveData data)
    {
        day = data.calendar.day;
        month = data.calendar.month;
        year = data.calendar.year;
        season = data.calendar.season;
        UpdateCanvas();
    }
    #endregion
}

