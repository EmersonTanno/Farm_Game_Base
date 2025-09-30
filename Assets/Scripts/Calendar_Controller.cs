using System;
using TMPro;
using UnityEngine;

public class Calendar_Controller : MonoBehaviour
{
    public int day = 0;
    public int month = 0;
    public static event Action OnDayChange;
    [SerializeField] TextMeshProUGUI daysText;

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
