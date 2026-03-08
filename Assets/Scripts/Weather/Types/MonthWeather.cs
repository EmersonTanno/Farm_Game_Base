using System.Collections.Generic;
[System.Serializable]
public class MonthWeather
{
    public int month;
    public List<List<DayWeather>> days = new List<List<DayWeather>>();
}