using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance;
    public static event Action<WeatherEnum> OnWeatherChanged;

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

    void Start()
    {
        if(weatherList.Count == 0)
        {
            GenerateMonthWeather(1);
            GenerateMonthWeather(2);
        }

        LogDayWeatherPrevision();
    }

    private void GenerateMonthWeather(int month)
    {
        MonthWeather newMonth = new MonthWeather
        {
            month = month
        };

        for (int i = 0; i < 30; i++)
        {
            newMonth.days.Add(GenerateDayWeather());
        }

        weatherList.Add(newMonth);
    }

    void OnEnable()
    {
        //Calendar_Controller.OnDayChange += GenerateNewDay;
        Time_Controll.OnHourChange += CheckHourWeather;
    }

    void OnDisable()
    {
        //Calendar_Controller.OnDayChange -= GenerateNewDay;
        Time_Controll.OnHourChange -= CheckHourWeather;
    }

    // private void GenerateNewDay()
    // {
    //     weatherList = GenerateDayWeather();

    //     CheckHourWeather();
        
    //     LogDayWeatherPrevision();
    // }

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

    private void CheckHourWeather()
    {
        int hour = Time_Controll.Instance.hours;

        WeatherEnum newWeather = GetWeatherForHour(hour);

        SetWeather(newWeather);
    }

    private WeatherEnum GetWeatherForHour(int hour)
    {
        WeatherEnum newWeather = WeatherEnum.SUN;
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
        OnWeatherChanged?.Invoke(weather);
    }

    public WeatherEnum GetWeather() => weather;

    private List<DayWeather> GenerateDayWeather()
    {
        List<DayWeather> events = new List<DayWeather>();
        SeasonRainProb rainProb = GetSeasonRainProb(Calendar_Controller.Instance.season);

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
            WeatherEnum type = GetWeatherType(rainProb.tempestProbability, Calendar_Controller.Instance.season);

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
                Calendar_Controller.Instance.season
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
                    Calendar_Controller.Instance.season
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

        return events;
    }

    private IEnumerator WaterSoilWithRain()
    {
        yield return new WaitForSeconds(2f);
        
        TileMapController.Instance.WaterSoilWithRain();
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
    
}
