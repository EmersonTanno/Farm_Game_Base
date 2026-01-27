using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCRoutine
{
    [Header("Time")]
    public int startHour;
    public int startMinute;

    
    [Header("Conditions")]
    public List<WeekDayEnum> validDays; // vazio = todos
    public List<WeatherEnum> validWeather; // vazio = qualquer clima
    public int minHearts = 0;

    [Header("Action")]
    public SceneLocationEnum targetLocation;
    public Vector2Int targetPosition;
    public NPCSide finalSide;

    [Header("Priority")]
    public int priority = 0;
}

