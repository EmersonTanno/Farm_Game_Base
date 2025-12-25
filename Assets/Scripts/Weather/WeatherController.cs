using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance;
    private WeatherEnum weather;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
        weather = WeatherEnum.RAIN;
    }

    public void SetWeather(WeatherEnum newWeather)
    {
        weather = newWeather;
    }

    public WeatherEnum GetWeather()
    {
        return weather;
    }
}
