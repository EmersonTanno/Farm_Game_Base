using System;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance;

    public static event Action<WeatherEnum> OnWeatherChanged;

    private WeatherEnum weather = WeatherEnum.SUN;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        Calendar_Controller.OnDayChange += Change;
    }

    private void Change()
    {
        SetWeather(WeatherEnum.TEMPEST);
    }

    public void SetWeather(WeatherEnum newWeather)
    {
        if (weather == newWeather) return;

        weather = newWeather;
        OnWeatherChanged?.Invoke(weather);
    }

    public WeatherEnum GetWeather() => weather;
}
