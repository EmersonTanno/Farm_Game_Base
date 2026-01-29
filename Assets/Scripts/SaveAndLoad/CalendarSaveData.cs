using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CalendarSaveData
{
    public CalendarSaveDataData calendar = new();
}

[System.Serializable]
public class CalendarSaveDataData
{
    public int day = 0;
    public int month = 0;
    public int year = 0;
    public Season season;
}
