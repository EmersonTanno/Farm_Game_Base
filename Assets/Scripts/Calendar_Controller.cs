using UnityEngine;

public class Calendar_Controller : MonoBehaviour
{
    public int day = 0;
    public int month = 0;
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
    }
}
