using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class WeatherParticleSystemController : MonoBehaviour
{
    private ParticleSystem ps;
    public WeatherEnum selectedWeather;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void OnEnable()
    {
        WeatherController.OnWeatherChanged += OnWeatherChanged;
        WarpController.OnWarpEnd += OnSceneChanged;
    }

    void OnDisable()
    {
        WeatherController.OnWeatherChanged -= OnWeatherChanged;
        WarpController.OnWarpEnd -= OnSceneChanged;
    }

    void OnWeatherChanged(WeatherEnum weather)
    {
        UpdateWeatherParticleSystem(weather, false);
    }

    void OnSceneChanged()
    {
        UpdateWeatherParticleSystem(WeatherController.Instance.GetWeather(), true);
    }

    void UpdateWeatherParticleSystem(WeatherEnum weather, bool prewarm)
    {
        bool canRain =
            weather == selectedWeather &&
            SceneInfo.Instance.sceneType != ScenesTypeEnum.inside;

        var main = ps.main;

        main.prewarm = prewarm;

        if (canRain && !ps.isPlaying)
        {
            StartCoroutine(DelayRainStart());
        }
        else if (!canRain && ps.isPlaying && prewarm == true)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        else if (!canRain && ps.isPlaying && prewarm == false)
        {
            ps.Stop();
        }
    }

    private IEnumerator DelayRainStart()
    {
        yield return new WaitForSeconds(1f);
        ps.Play();
    }
}
