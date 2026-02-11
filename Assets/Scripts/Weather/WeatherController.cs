using System;
using System.Collections;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance;
    public static event Action<WeatherEnum> OnWeatherChanged;

    private WeatherEnum weather = WeatherEnum.SUN;
    private DayWeather dayWeather;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);
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

        SetWeather(
            dayWeather.baseWeather == WeatherEnum.SUN
            ? WeatherEnum.SUN
            : WeatherEnum.SUN
        );
        Debug.Log($"Clima do dia: {dayWeather.baseWeather} | {dayWeather.rainStartHour} - {dayWeather.rainEndHour}");
    }

    private void CheckHourWeather()
    {
        if (dayWeather.baseWeather == WeatherEnum.SUN)
            return;

        int hour = Time_Controll.Instance.hours;

        if (hour == dayWeather.rainStartHour)
        {
            SetWeather(dayWeather.baseWeather);
            StartCoroutine(WaterSoilWithRain());
        }

        if (hour == dayWeather.rainEndHour)
            SetWeather(WeatherEnum.SUN);
    }

    public void SetWeather(WeatherEnum newWeather)
    {
        if (weather == newWeather) return;

        weather = newWeather;
        OnWeatherChanged?.Invoke(weather);
    }

    public WeatherEnum GetWeather() => weather;

    private DayWeather GenerateDayWeather()
    {
        DayWeather dw = new DayWeather();

        float roll = UnityEngine.Random.value;

        if (roll < 0.05f)
            dw.baseWeather = WeatherEnum.TEMPEST;
        else if (roll < 0.30f)
            dw.baseWeather = WeatherEnum.RAIN;
        else
            dw.baseWeather = WeatherEnum.SUN;

        if (dw.baseWeather == WeatherEnum.SUN)
        {
            dw.rainStartHour = -1;
            dw.rainEndHour = -1;
            return dw;
        }

        dw.rainStartHour = UnityEngine.Random.Range(8, 16);
        dw.rainEndHour = UnityEngine.Random.Range(dw.rainStartHour + 2, 22);

        return dw;
    }

    private IEnumerator WaterSoilWithRain()
    {
        yield return new WaitForSeconds(2f);
        
        TileMapController.Instance.WaterSoilWithRain();
    }
}
