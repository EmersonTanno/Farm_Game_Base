using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance;
    public static event Action<WeatherEnum> OnWeatherChanged;

    private WeatherEnum weather = WeatherEnum.SUN;
    private List<DayWeather> dayWeather;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable()
    {
        Calendar_Controller.OnDayChange += GenerateNewDay;
        Time_Controll.OnHourChange += CheckHourWeather;
    }

    void OnDisable()
    {
        Calendar_Controller.OnDayChange -= GenerateNewDay;
        Time_Controll.OnHourChange -= CheckHourWeather;
    }

    private void GenerateNewDay()
    {
        dayWeather = GenerateDayWeather();

        if(dayWeather.Count == 0)
        {
            SetWeather(WeatherEnum.SUN);
        }
        LogDayWeatherPrevision();
    }

    private void LogDayWeatherPrevision()
    {
        foreach(DayWeather d in dayWeather)
        {
            Debug.Log($"Clima do dia: {d.baseWeather} | {d.rainStartHour} - {d.rainEndHour}");
        }
    }

    private void CheckHourWeather()
    {
        int hour = Time_Controll.Instance.hours;
        WeatherEnum newWeather = GetWeatherForHour(hour);

        SetWeather(newWeather);
    }

    private WeatherEnum GetWeatherForHour(int hour)
    {
        foreach (var weatherEvent in dayWeather)
        {
            if (hour == weatherEvent.rainStartHour)
            {
                StartCoroutine(WaterSoilWithRain());
            }

            if (hour >= weatherEvent.rainStartHour && hour < weatherEvent.rainEndHour)
            {
                return weatherEvent.baseWeather;
            }
        }

        return WeatherEnum.SUN;
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

        // 35% chance de sol total - talvez setar probabilidades diferentes para cada estação
        if (UnityEngine.Random.value < 0.35f)
            return events;

        // Quantidade de blocos de chuva (1 a 5)
        int rainQuantity = 1;
        while (UnityEngine.Random.value > 0.80f && rainQuantity < 5)
        {
            rainQuantity++;
        }

        // Se chegou no máximo → chuva longa ou tempestade
        if (rainQuantity >= 5)
        {
            WeatherEnum type = UnityEngine.Random.value > 0.7f 
                ? WeatherEnum.TEMPEST 
                : WeatherEnum.RAIN;

            events.Add(new DayWeather()
            {
                baseWeather = type,
                rainStartHour = 8,
                rainEndHour = 24
            });

            return events;
        }

        // Caso normal: múltiplos blocos distribuídos
        int currentHour = 6;

        for (int i = 0; i < rainQuantity; i++)
        {
            int start = UnityEngine.Random.Range(currentHour, 20);
            int duration = UnityEngine.Random.Range(1, 4); // 1 a 3 horas
            int end = Mathf.Min(start + duration, 24);

            events.Add(new DayWeather()
            {
                baseWeather = WeatherEnum.RAIN,
                rainStartHour = start,
                rainEndHour = end
            });

            currentHour = end + UnityEngine.Random.Range(1, 3); // espaço entre chuvas

            if (currentHour >= 23)
                break;
        }

        return events;
    }

    private IEnumerator WaterSoilWithRain()
    {
        yield return new WaitForSeconds(2f);
        
        TileMapController.Instance.WaterSoilWithRain();
    }
}
