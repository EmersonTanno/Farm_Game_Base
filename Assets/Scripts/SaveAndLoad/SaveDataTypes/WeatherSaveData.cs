using System.Collections.Generic;

[System.Serializable]
public class WeatherSaveData
{
    public List<MonthWeatherSaveData> weatherData = new List<MonthWeatherSaveData>();

    public List<MonthWeatherSaveData> ConvertMonthWeatherDataToSave(List<MonthWeather> list)
    {
        List<MonthWeatherSaveData> newList = new List<MonthWeatherSaveData>();

        foreach (MonthWeather month in list)
        {
            MonthWeatherSaveData newMonth = new MonthWeatherSaveData
            {
                month = month.month
            };

            foreach (List<DayWeather> dayWeather in month.days)
            {
                DayWeatherList newDay = new DayWeatherList();

                foreach (DayWeather day in dayWeather)
                {
                    newDay.day.Add(new DayWeatherSaveData
                    {
                        weather = day.weather,
                        startHour = day.startHour
                    });
                }

                newMonth.days.Add(newDay);
            }

            newList.Add(newMonth);
        }

        return newList;
    }

    public List<MonthWeather> ConvertSaveToMonthWeatherData()
    {
        List<MonthWeather> newList = new List<MonthWeather>();

        foreach (MonthWeatherSaveData month in weatherData)
        {
            MonthWeather newMonth = new MonthWeather
            {
                month = month.month
            };

            foreach (DayWeatherList dayWeatherList in month.days)
            {
                List<DayWeather> newDay = new List<DayWeather>();

                foreach (DayWeatherSaveData day in dayWeatherList.day)
                {
                    newDay.Add(new DayWeather
                    {
                        weather = day.weather,
                        startHour = day.startHour
                    });
                }

                newMonth.days.Add(newDay);
            }

            newList.Add(newMonth);
        }

        return newList;
    }
}

[System.Serializable]
public class MonthWeatherSaveData
{
    public int month;
    public List<DayWeatherList> days = new List<DayWeatherList>();
}

[System.Serializable]
public struct DayWeatherSaveData
{
    public WeatherEnum weather;
    public int startHour;
}

[System.Serializable]
public class DayWeatherList
{
    public List<DayWeatherSaveData> day = new List<DayWeatherSaveData>();
}
