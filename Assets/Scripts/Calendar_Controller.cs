using UnityEngine;

public class Calendar_Controller : MonoBehaviour
{
    void OnEnable()
    {
        Time_Controll.OnTimeChanged += UpdateCalendar;
    }

    void OnDisable()
    {
        Time_Controll.OnTimeChanged -= UpdateCalendar;
    }

    private void UpdateCalendar(int hours, int minutes)
    {
        Debug.Log($"Calendário recebeu: {hours:D2}:{minutes:D2}");
    }
}
