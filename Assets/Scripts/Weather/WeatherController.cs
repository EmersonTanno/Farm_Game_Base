using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance;
    public static event Action<WeatherEnum> OnWeatherChanged;
    public static event Action OnThunderFall;
    public static event Action OnRainFall;

    [SerializeField] List<SeasonRainProb> seasonRainProb;

    private WeatherEnum weather = WeatherEnum.SUN;
    private List<MonthWeather> weatherList = new List<MonthWeather>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // void Start()
    // {
    //     LogGeneratedWeather();
    // }

    void OnEnable()
    {
        Calendar_Controller.OnDayChange += CheckGeneratedWeather;
        Calendar_Controller.OnMonthChange += GenerateWeatherForTwoMonths;
        SaveSystem.OnLoadFinish += CheckGeneratedWeather;
        Time_Controll.OnHourChange += CheckHourWeather;

        //Calendar_Controller.OnDayChange += LogGeneratedWeather;
    }

    void OnDisable()
    {
        Calendar_Controller.OnDayChange -= CheckGeneratedWeather;
        Calendar_Controller.OnMonthChange -= GenerateWeatherForTwoMonths;
        SaveSystem.OnLoadFinish -= CheckGeneratedWeather;
        Time_Controll.OnHourChange -= CheckHourWeather;
    }



    private void CheckHourWeather()
    {
        int hour = Time_Controll.Instance.hours;

        WeatherEnum newWeather = GetWeatherForHour(hour);

        SetWeather(newWeather);
    }

    private WeatherEnum GetWeatherForHour(int hour)
    {
        WeatherEnum newWeather = WeatherEnum.SUN;

        if(weatherList.Count == 0)
        {
            return newWeather;
        }

        Calendar_Controller calendar = Calendar_Controller.Instance;

        List<DayWeather> dayWeather = weatherList.Find(i => i.month == calendar.month).days[calendar.day];

        foreach (var weatherEvent in dayWeather)
        {
            if (hour >= weatherEvent.startHour)
            {
                newWeather = weatherEvent.weather;
            }
            else
            {
                break;
            }
        }

        if (newWeather == WeatherEnum.RAIN || newWeather == WeatherEnum.TEMPEST)
        {
            StartCoroutine(WaterSoilWithRain());
        }

        return newWeather;
    }

    public void SetWeather(WeatherEnum newWeather)
    {
        if (weather == newWeather) return;

        weather = newWeather;

        if(weather == WeatherEnum.TEMPEST)
        {
            StartCoroutine(CallThunder());
        }

        OnWeatherChanged?.Invoke(weather);
    }

    public WeatherEnum GetWeather() => weather;

    #region Check and Generate Month Weathers Controllers
    private void CheckGeneratedWeather()
    {
        if(weatherList.Count == 0 || (!BootContext.IsLoadingGame && weatherList.Count == 0))
        {
            GenerateWeatherForTwoMonths();
        }
    }

    private void GenerateWeatherForTwoMonths()
    {
        Calendar_Controller calendar = Calendar_Controller.Instance;

        if(weatherList.Count == 0 || weatherList.Count > 2)
        {
            weatherList.Clear();
            GenerateMonthWeather(calendar.month);
            GenerateMonthWeather(calendar.GetMonth(calendar.month + 1));
        }
        else
        {
            weatherList.RemoveAt(0);
            GenerateMonthWeather(calendar.GetMonth(calendar.month + 1));
        }
    }


    #endregion

    #region Generate Weather Functions
    private void GenerateMonthWeather(int month)
    {
        MonthWeather newMonth = new MonthWeather
        {
            month = month
        };

        for (int i = 0; i < 30; i++)
        {
            newMonth.days.Add(GenerateDayWeather(Calendar_Controller.Instance.GetSeasonType(month)));
        }

        weatherList.Add(newMonth);
    }

    private List<DayWeather> GenerateDayWeather(Season season)
    {
        List<DayWeather> events = new List<DayWeather>();
        SeasonRainProb rainProb = GetSeasonRainProb(season);

        // Sempre começa o dia com sol
        events.Add(new DayWeather
        {
            weather = WeatherEnum.SUN,
            startHour = 6
        });

        // chance de dia totalmente ensolarado
        if (UnityEngine.Random.value < rainProb.sunnyDayChance)
            return events;

        // Quantidade de blocos de chuva (1 a 5)
        int rainQuantity = 1;
        while (UnityEngine.Random.value < rainProb.multipleRain && rainQuantity < 5)
        {
            rainQuantity++;
        }

        // Caso extremo: chuva o dia inteiro
        if (rainQuantity >= 5)
        {
            WeatherEnum type = GetWeatherType(rainProb.tempestProbability, season);

            events.Add(new DayWeather
            {
                weather = type,
                startHour = 8
            });

            return events;
        }

        int currentHour = 6;

        for (int i = 0; i < rainQuantity; i++)
        {
            int start = UnityEngine.Random.Range(currentHour + 1, 20);
            int duration = UnityEngine.Random.Range(1, 4);
            int end = Mathf.Min(start + duration, 23);

            WeatherEnum type = GetWeatherType(
                rainProb.tempestProbability,
                season
            );

            // começa chuva/neve
            events.Add(new DayWeather
            {
                weather = type,
                startHour = start
            });

            // possível mudança de intensidade no meio
            if (UnityEngine.Random.value < rainProb.tempestProbability)
            {
                WeatherEnum midType = GetWeatherType(
                    rainProb.tempestProbability * 2,
                    season
                );

                events.Add(new DayWeather
                {
                    weather = midType,
                    startHour = start + duration / 2
                });
            }

            // volta para sol
            events.Add(new DayWeather
            {
                weather = WeatherEnum.SUN,
                startHour = end
            });

            currentHour = end + UnityEngine.Random.Range(1, 3);

            if (currentHour >= 24)
                break;
        }

        return RecycleDayWeather(events);
    }

    private List<DayWeather> RecycleDayWeather(List<DayWeather> dayWeather)
    {
        List<DayWeather> cleaned = new List<DayWeather>();

        foreach (var weather in dayWeather)
        {
            if (cleaned.Count == 0 || cleaned[^1].weather != weather.weather)
            {
                cleaned.Add(weather);
            }
        }

        return cleaned;
    }

    private SeasonRainProb GetSeasonRainProb(Season season)
    {
        return seasonRainProb.Find(i => i.season == season);
    }

    private WeatherEnum GetWeatherType(float probability, Season season)
    {
        WeatherEnum type;
        if(season == Season.Inverno)
        {
            type = UnityEngine.Random.value < probability
                ? WeatherEnum.BLIZZARD
                : WeatherEnum.SNOW;
        }
        else
        {
            type = UnityEngine.Random.value < probability
                ? WeatherEnum.TEMPEST
                : WeatherEnum.RAIN;
        }

        return type;
    }
    #endregion

    #region Support Functions
    private IEnumerator WaterSoilWithRain()
    {
        yield return new WaitForSeconds(2f);

        OnRainFall?.Invoke();
    }

    private List<DayWeather> GetDayWeather(int day, int month)
    {
        MonthWeather monthWeather = weatherList.Find(i => i.month == month);

        if(monthWeather != null)
        {
            return monthWeather.days[day];
        }

        return null;
    }
    
    public bool WillRain()
    {
        Calendar_Controller calendar = Calendar_Controller.Instance;

        List<DayWeather> dayWeather = GetDayWeather(calendar.day, calendar.month);

        if (dayWeather == null) return false;

        return dayWeather.Exists(i => 
            i.weather == WeatherEnum.RAIN || 
            i.weather == WeatherEnum.TEMPEST
        );
    }

    private IEnumerator CallThunder()
    {
        while(GetWeather() == WeatherEnum.TEMPEST)
        {
            if(UnityEngine.Random.value > 0.6f)
            {
                OnThunderFall?.Invoke();
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(10, 30));
        }
    }
    #endregion

    #region Log
    private void LogGeneratedWeather()
    {
        Calendar_Controller calendar = Calendar_Controller.Instance;
        string dayWeather = $"PREVISÂO DO TEMPO {calendar.day} {weatherList[0].month} \n\n";

            foreach(DayWeather day in weatherList[0].days[calendar.day])
            {
                dayWeather += $"{day.weather} | {day.startHour} \n";
            }
            dayWeather += $"\n";
        
        Debug.Log(dayWeather);
    }

    private void LogDayWeatherPrevision()
    {
        Calendar_Controller calendar = Calendar_Controller.Instance;
        string dayWeather = $"PREVISÂO DO TEMPO {weatherList[0].month}/{calendar.year} \n\n";
        int count = 1;
        foreach(List<DayWeather> d in weatherList[0].days)
        {
            dayWeather += $"Clima do dia {count} \n";
            foreach(DayWeather day in d)
            {
                dayWeather += $"{day.weather} | {day.startHour} \n";
            }
            dayWeather += $"\n";
            count++;
        }
        Debug.Log(dayWeather);

        string dayWeather2 = $"PREVISÂO DO TEMPO {weatherList[1].month}/{calendar.year} \n\n";
        count = 1;
        foreach(List<DayWeather> d in weatherList[1].days)
        {
            dayWeather2 += $"Clima do dia {count} - \n";
            foreach(DayWeather day in d)
            {
                dayWeather2 += $"{day.weather} | {day.startHour} \n";
            }
            dayWeather2 += $"\n";
            count++;
        }
        Debug.Log(dayWeather2);
    }
    #endregion

    #region Save / Load
    public void Save(ref WeatherSaveData data)
    {
        data.weatherData = new WeatherSaveData().ConvertMonthWeatherDataToSave(weatherList) ;
    }

    public void Load(WeatherSaveData data)
    {
        weatherList = data.ConvertSaveToMonthWeatherData();
    }
    #endregion
}
