using System;
using TMPro;
using UnityEngine;

public class Calendar_Controller : MonoBehaviour
{
    #region Variables
    //Day and Month
    public int day = 0;
    public int month = 0;

    //Canvas
    [SerializeField] TextMeshProUGUI daysText;

    //Event
    public static event Action OnDayChange;
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
        Debug.Log($"Dia: {day}");
        day++;
        OnDayChange?.Invoke();
        UpdateCanvas();
    }

    private void UpdateCanvas()
    {
        if (daysText != null)
        {
            daysText.text = $"Data: {day:D2}/{month:D2}";
        }
    }
}
